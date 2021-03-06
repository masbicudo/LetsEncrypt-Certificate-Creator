﻿using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.POSH;
using ACMESharp.POSH.Util;
using ACMESharp.Vault;
using ACMESharp.Vault.Model;
using ACMESharp.Vault.Util;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace LetsEncryptAcmeReg
{
    public class Acme
    {
        public Acme()
        {
            VaultHelper.CustomVaultGetter = this.GetCustomVault;
        }

        public string VaultLocation { get; set; }

        private IVault GetCustomVault()
        {
            var rootPath = this.GetVaultPath();

            var vlt = new ACMESharp.Vault.Providers.LocalDiskVault();
            var ok = false;
            try
            {
                vlt.RootPath = rootPath;
                vlt.BypassEFS = true;
                vlt.CreatePath = true;
                vlt.Init();
                ok = true;
                return vlt;
            }
            finally
            {
                if (!ok) vlt.Dispose();
            }
        }

        private string GetVaultPath()
        {
            if (string.IsNullOrWhiteSpace(this.VaultLocation))
                throw new ArgumentNullException("Vault path is null.");

            return Environment.ExpandEnvironmentVariables(this.VaultLocation);
        }

        private static IVault GetVault()
        {
            return VaultHelper.GetVault(null);
        }

        public VaultInfo GetVaultInfo()
        {
            VaultInfo vlt = null;
            for (int it = 0; it < 2 && vlt == null; it++)
            {
                vlt = new GetVault().GetValue<VaultInfo>();

                if (vlt == null)
                    new InitializeVault { BaseUri = "https://acme-v01.api.letsencrypt.org/" }.Run();

                vlt = new GetVault().GetValue<VaultInfo>();
            }
            return vlt;
        }

        public RegistrationInfo[] GetRegistrations()
        {
            var vaultInfo = this.GetVaultInfo();
            var registrationInfos = vaultInfo.Registrations?.Values?.ToArray();
            return registrationInfos ?? new RegistrationInfo[0];
        }

        public string[] GetDomainsByEmail(RegistrationInfo[] regs, string email)
        {
            var r = regs
                .FirstOrDefault(x => x.Registration.Contacts.Any(c => c == $"mailto:{email}"));

            if (r != null)
            {
                var allids = this.GetAllIdentifiers(r);
                return allids.Select(x => (x.Dns ?? "").ToString()).ToArray();
            }

            return new string[0];
        }

        public string[] GetDomainsByRegistration(RegistrationInfo reg)
        {
            var allids = this.GetAllIdentifiers(reg);
            return allids.Select(x => (x.Dns ?? "").ToString()).ToArray();
        }

        public RegistrationInfo Register(RegistrationInfo[] regs, string email)
        {
            var emails = email.Split(';').Select(s => $"mailto:{s?.Trim()}").ToArray();
            var regInfo = this.GetRegistrationByEmail(regs, emails);
            regInfo = regInfo != null ? null : this.CreateOrUpdateRegistration(null, emails);
            return regInfo;
        }

        private RegistrationInfo GetRegistrationByEmail(RegistrationInfo[] regs, string[] emails)
        {
            var regInfo = regs.FirstOrDefault(x => x.Registration.Contacts.Intersect(emails).Any());
            return regInfo;
        }

        public RegistrationInfo AcceptTos(RegistrationInfo regInfo)
        {
            if (regInfo?.Registration != null && regInfo.Registration.TosAgreementUri == null)
                return this.CreateOrUpdateRegistration(regInfo, acceptTos: true);

            return null;
        }

        private RegistrationInfo CreateOrUpdateRegistration(RegistrationInfo regInfo, string[] emails = null, bool acceptTos = false)
        {
            using (var vlt = GetVault())
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                bool isNew = regInfo == null;
                if (isNew)
                    regInfo = new RegistrationInfo
                    {
                        Id = EntityHelper.NewId(),
                        SignerProvider = "RS256",
                    };
                else
                {
                    regInfo = v.Registrations[regInfo.Id];
                }

                using (var c = ClientHelper.GetClient(v, regInfo))
                {
                    c.Init();
                    c.GetDirectory(true);

                    if (isNew)
                    {
                        c.Register(emails);
                        if (acceptTos) c.UpdateRegistration(agreeToTos: true);
                    }
                    else
                        c.UpdateRegistration(agreeToTos: acceptTos, contacts: emails);

                    regInfo.Registration = c.Registration;

                    if (v.Registrations == null)
                        v.Registrations = new EntityDictionary<RegistrationInfo>();

                    if (isNew) v.Registrations.Add(regInfo);
                }

                vlt.SaveVault(v);

                return regInfo;
            }
        }

        public IdentifierInfo FindIdentifier(string idref)
        {
            var result = GetAllIdentifiers()
                .SingleOrDefault(x => (x.Alias ?? "").ToString() == idref);
            return result;
        }

        [NotNull]
        [ItemNotNull]
        private IdentifierInfo[] GetAllIdentifiers([CanBeNull] RegistrationInfo regInfo)
        {
            var all = this.GetAllIdentifiers();
            return regInfo == null ? all : all.Where(x => x.RegistrationRef == regInfo.Id).ToArray();
        }

        [NotNull]
        [ItemNotNull]
        private IdentifierInfo[] GetAllIdentifiers()
        {
            using (var vlt = GetVault())
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                if (v.Identifiers == null || v.Identifiers.Count < 1)
                    return new IdentifierInfo[0];

                return v.Identifiers.Values.ToArray();
            }
        }

        [CanBeNull]
        public string GetIdentifierAlias(RegistrationInfo regInfo, string domain)
        {
            var allIds = this.GetAllIdentifiers();

            var identifierInfo = allIds.SingleOrDefault(x => x != null && (x.Dns ?? "") == domain);

            //if (identifierInfo?.RegistrationRef == regInfo?.Id)
            string idref = identifierInfo?.Alias;
            if (identifierInfo == null)
            {
                using (var vlt = GetVault())
                {
                    vlt.OpenStorage();
                    var v = vlt.LoadVault();

                    var count = v.Identifiers.Values.Count();
                    for (int i = 0; i < 100; i++)
                    {
                        idref = "dns" + (count + i + 1);
                        if (!v.Identifiers.ContainsKey(idref))
                            return idref;
                    }
                }
            }

            return idref;
        }

        [NotNull]
        [ItemNotNull]
        public IdentifierInfo[] GetIdentifiers([NotNull] RegistrationInfo regInfo, [CanBeNull] string domain)
        {
            if (regInfo == null) throw new ArgumentNullException(nameof(regInfo));

            var states = this.GetOrCreateIdentifiers(regInfo, null, domain, allowCreation: false);
            return states;
        }

        [NotNull]
        [ItemNotNull]
        public IdentifierInfo[] GetOrCreateIdentifier([NotNull] RegistrationInfo regInfo, [NotNull] string domain)
        {
            if (regInfo == null) throw new ArgumentNullException(nameof(regInfo));
            if (domain == null) throw new ArgumentNullException(nameof(domain));

            var states = this.GetOrCreateIdentifiers(regInfo, null, domain, true);
            return states;
        }

        [NotNull]
        [ItemNotNull]
        private IdentifierInfo[] GetOrCreateIdentifiers([CanBeNull] RegistrationInfo regInfo, [CanBeNull] string alias, [CanBeNull] string dns, bool allowCreation = true)
        {
            using (var vlt = GetVault())
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                var alias2 = alias;
                var iis0 = v.Identifiers?.Values;

                if (iis0 != null)
                {
                    var iis = iis0.Where(x => (alias2 == null || x.Alias == alias2) && (dns == null || x.Dns == dns));

                    if (regInfo == null)
                        return iis.ToArray();

                    var iisOfReg = iis.Where(ii => ii.RegistrationRef == regInfo.Id).ToArray();
                    if (iisOfReg.Length > 0)
                        return iisOfReg;

                    iis = iis0.Where(x => x.Alias == alias2 || x.Dns == dns);

                    if (!allowCreation)
                        return new IdentifierInfo[0];

                    if (iis.Any())
                        throw new InvalidOperationException(Messages.CannotCreateIdentifier_DomainAlreadyUsed);
                }
                else if (!allowCreation)
                    return new IdentifierInfo[0];

                if (regInfo == null)
                    throw new InvalidOperationException(Messages.CannotCreateIdentifier_NoRegistrationProvided);

                v.Identifiers = v.Identifiers ?? new EntityDictionary<IdentifierInfo>();

                // if alias was not provided, get a valid one that is not used alrady
                var count = v.Identifiers.Values.Count();
                for (int i = 0; i < 100; i++)
                {
                    alias = "dns" + (count + i + 1);
                    if (!v.Identifiers.ContainsKey(alias))
                        break;
                }

                // see ACMESharp.POSH.NewIdentifier
                var iiNew = new IdentifierInfo
                {
                    Id = EntityHelper.NewId(),
                    Alias = alias,
                    RegistrationRef = regInfo.Id,
                    Dns = dns,
                };

                using (var c = ClientHelper.GetClient(v, regInfo))
                {
                    c.Init();
                    c.GetDirectory(true);

                    var authzState = c.AuthorizeIdentifier(dns);
                    iiNew.Authorization = authzState;

                    v.Identifiers.Add(iiNew);
                }

                vlt.SaveVault(v);

                return new[] { iiNew };
            }
        }

        public string GetTos(RegistrationInfo[] regs, string email)
        {
            var r = regs
                .Where(x => x.Registration.Contacts.Any(c => c == $"mailto:{email}"))
                .Select(x => x.Registration.TosLinkUri)
                .FirstOrDefault();
            return r;
        }

        public AuthorizeChallenge[] GetChallengesByDomain(RegistrationInfo regInfo, string domain)
        {
            var states = this.GetOrCreateIdentifiers(regInfo, null, domain, false).Select(ii => ii.Authorization);
            return states.FirstOrDefault()?.Challenges.ToArray() ?? new AuthorizeChallenge[0];
        }

        [NotNull]
        [ItemNotNull]
        public CertificateInfo[] GetCertificates([CanBeNull] RegistrationInfo regInfo, [CanBeNull] params string[] domain)
        {
            var v = this.GetVaultInfo();

            if (regInfo == null && domain == null)
                return v.Certificates?.Values.ToArray() ?? new CertificateInfo[0];

            var allIds = this.GetAllIdentifiers(regInfo);

            var identifierInfo = domain == null
                || domain.Length == 0 ?
                    allIds :
                    allIds.Where(x => Array.IndexOf(domain, x.Dns) >= 0 || Array.IndexOf(domain, x.Alias) >= 0);

            var result = v.Certificates?.Values?.Where(c => identifierInfo.Any(i => c.IdentifierRef == i.Id)).ToArray();
            return result ?? new CertificateInfo[0];
        }

        [CanBeNull]
        public CertificateInfo GetCertificate(string certRef)
        {
            var v = this.GetVaultInfo();
            if (v.Certificates == null || v.Certificates.Count < 1)
                return null;

            var ci = v.Certificates.GetByRef(certRef, throwOnMissing: false);
            return ci;
        }

        public void DeleteRegistration(RegistrationInfo value)
        {
            if (value == null)
                return;

            using (var vlt = GetVault())
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                // removing the associated identifiers
                var identsToRemove = v.Identifiers.Values?
                    .Where(x => x.RegistrationRef == value.Id);
                if (identsToRemove != null)
                    foreach (var ident in identsToRemove)
                    {
                        // removing associated certificates
                        var certsToRemove = v.Certificates.Values?
                            .Where(x => x.IdentifierRef == ident.Id);
                        if (certsToRemove != null)
                            foreach (var cert in certsToRemove)
                            {
                                // removing the certificate
                                v.Certificates?.Remove(cert.Id);
                            }

                        // removing the domain
                        v.Identifiers?.Remove(ident.Id);
                    }

                // removing the registration
                v.Registrations?.Remove(value.Id);

                vlt.SaveVault(v);
            }
        }

        public void DeleteDomain(RegistrationInfo regInfo, string domain)
        {
            if (domain == null)
                return;

            if (regInfo == null)
                return;

            using (var vlt = GetVault())
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                var idents = this.GetIdentifiers(regInfo, domain);
                foreach (var ident in idents)
                {
                    // removing associated certificates
                    var certsToRemove = v.Certificates?.Values?
                        .Where(x => x.IdentifierRef == ident.Id);
                    if (certsToRemove != null)
                        foreach (var cert in certsToRemove)
                        {
                            // removing the certificate
                            v.Certificates.Remove(cert.Id);
                        }

                    // removing the domain
                    v.Identifiers?.Remove(ident.Id);
                }

                vlt.SaveVault(v);
            }
        }

        public AcmeTextAssets GetTextAssets(string certRef, bool noThrow = false)
        {
            using (var vlt = GetVault())
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                if (string.IsNullOrWhiteSpace(certRef))
                    return null;

                if (v.Certificates == null || v.Certificates.Count < 1)
                    throw new InvalidOperationException("No certificates found");

                var ci = v.Certificates.GetByRef(certRef, throwOnMissing: false);
                if (ci == null)
                    throw new ItemNotFoundException("Unable to find a Certificate for the given reference");

                var ta = GetTextAssets(ci, vlt, v, noThrow);

                return ta;
            }
        }

        private static AcmeTextAssets GetTextAssets(CertificateInfo ci, IVault vlt, VaultInfo v, bool noThrow = false)
        {
            var result = new AcmeTextAssets
            {
                KeyPem = GetKeyPem(ci, vlt, noThrow),
                CsrPem = GetCsrPem(ci, vlt, noThrow),
                CrtPem = GetCrtPem(ci, vlt, noThrow),
                CrtDer = GetCrtDer(ci, vlt, noThrow),
                IssuerPem = GetIssuerPem(ci, v, vlt, noThrow),
                IssuerDer = GetIssuerDer(ci, v, vlt, noThrow),
            };
            return result;
        }

        private static string GetIssuerDer(CertificateInfo ci, VaultInfo v, IVault vlt, bool noThrow = false)
        {
            if (ci.CertificateRequest == null || string.IsNullOrEmpty(ci.CrtDerFile))
                if (noThrow) return null;
                else throw new InvalidOperationException("Cannot export CRT; CSR hasn't been submitted or CRT hasn't been retrieved");
            if (string.IsNullOrEmpty(ci.IssuerSerialNumber) || !v.IssuerCertificates.ContainsKey(ci.IssuerSerialNumber))
                if (noThrow) return null;
                else throw new InvalidOperationException("Issuer certificate hasn't been resolved");
            return GetAssetText(vlt, VaultAssetType.IssuerDer,
                v.IssuerCertificates[ci.IssuerSerialNumber].CrtDerFile);
        }

        private static string GetIssuerPem(CertificateInfo ci, VaultInfo v, IVault vlt, bool noThrow = false)
        {
            if (ci.CertificateRequest == null || string.IsNullOrEmpty(ci.CrtPemFile))
                if (noThrow) return null;
                else throw new InvalidOperationException("Cannot export CRT; CSR hasn't been submitted or CRT hasn't been retrieved");
            if (string.IsNullOrEmpty(ci.IssuerSerialNumber) || !v.IssuerCertificates.ContainsKey(ci.IssuerSerialNumber))
                if (noThrow) return null;
                else throw new InvalidOperationException("Issuer certificate hasn't been resolved");
            return GetAssetText(vlt, VaultAssetType.IssuerPem,
                v.IssuerCertificates[ci.IssuerSerialNumber].CrtPemFile);
        }

        private static string GetCrtDer(CertificateInfo ci, IVault vlt, bool noThrow = false)
        {
            if (ci.CertificateRequest == null || string.IsNullOrEmpty(ci.CrtDerFile))
                if (noThrow) return null;
                else throw new InvalidOperationException("Cannot export CRT; CSR hasn't been submitted or CRT hasn't been retrieved");
            return GetAssetText(vlt, VaultAssetType.CrtDer, ci.CrtDerFile);
        }

        private static string GetCrtPem(CertificateInfo ci, IVault vlt, bool noThrow = false)
        {
            if (ci.CertificateRequest == null || string.IsNullOrEmpty(ci.CrtPemFile))
                if (noThrow) return null;
                else throw new InvalidOperationException("Cannot export CRT; CSR hasn't been submitted or CRT hasn't been retrieved");
            return GetAssetText(vlt, VaultAssetType.CrtPem, ci.CrtPemFile);
        }

        private static string GetCsrPem(CertificateInfo ci, IVault vlt, bool noThrow = false)
        {
            if (string.IsNullOrEmpty(ci.CsrPemFile))
                if (noThrow) return null;
                else throw new InvalidOperationException("Cannot export CSR; it hasn't been imported or generated");
            return GetAssetText(vlt, VaultAssetType.CsrPem, ci.CsrPemFile);
        }

        private static string GetKeyPem(CertificateInfo ci, IVault vlt, bool noThrow = false)
        {
            if (string.IsNullOrEmpty(ci.KeyPemFile))
                if (noThrow) return null;
                else throw new InvalidOperationException("Cannot export private key; it hasn't been imported or generated");
            return GetAssetText(vlt, VaultAssetType.KeyPem, ci.KeyPemFile);
        }

        public static string GetAssetText(IVault vlt, VaultAssetType type, string name)
        {
            var asset = vlt.GetAsset(type, name);
            using (Stream s = vlt.LoadAsset(asset))
            using (var reader = new StreamReader(s))
                return reader.ReadToEnd();
        }

        public CertificateInfo[] GetAllCertificates()
        {
            using (var vlt = GetVault())
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                if (v.Registrations == null || v.Registrations.Count < 1)
                    throw new InvalidOperationException("No registrations found");

                var ri = v.Registrations[0];
                var r = ri.Registration;

                return v.Certificates.Values.ToArray();
            }
        }

        public AuthorizationState SetupChallenge(string identifyerRef, string challengeType = "http-01")
        {
            using (var vlt = GetVault())
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                if (v.Registrations == null || v.Registrations.Count < 1)
                    throw new InvalidOperationException("No registrations found");

                var ri = v.Registrations[0];
                var r = ri.Registration;

                var ii = v.Identifiers.Values.ToArray()
                    .SingleOrDefault(x => x.Alias == identifyerRef);

                if (ii.Challenges == null)
                    ii.Challenges = new Dictionary<string, AuthorizeChallenge>();

                if (ii.ChallengeCompleted == null)
                    ii.ChallengeCompleted = new Dictionary<string, DateTime?>();

                var authzState = ii.Authorization;

                var c = ClientHelper.GetClient(v, ri);

                var provider = ChallengeDecoderExtManager.GetProvider(challengeType);
                //if (provider == null)
                //    throw new NotSupportedException("no provider exists for requested challenge type")
                //        .With("challengeType", challengeType);

                var authzChallenge = authzState.Challenges.FirstOrDefault(x => x.Type == challengeType);
                using (var decoder = provider.GetDecoder(authzState.IdentifierPart, authzChallenge.ChallengePart))
                {
                    authzChallenge.Challenge = decoder.Decode(authzState.IdentifierPart, authzChallenge.ChallengePart, c.Signer);

                    //if (authzChallenge.Challenge == null)
                    //    throw new InvalidDataException("challenge decoder produced no output");
                }

                c.HandleChallenge(authzState, challengeType, "Manual", null);
                ii.ChallengeCompleted[challengeType] = DateTime.Now;

                ii.Challenges[challengeType] = authzChallenge;

                vlt.SaveVault(v);

                return authzState;
            }
        }
    }
}