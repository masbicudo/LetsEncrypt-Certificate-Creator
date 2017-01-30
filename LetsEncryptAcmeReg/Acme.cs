using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Net;
using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.POSH;
using ACMESharp.POSH.Util;
using ACMESharp.Vault.Model;
using ACMESharp.Vault.Profile;
using ACMESharp.Vault.Util;
using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    public class Acme
    {
        public VaultInfo Vault()
        {
            var vlt = new GetVault().GetValue<VaultInfo>()
                      ??
                      new InitializeVault { BaseUri = "https://acme-v01.api.letsencrypt.org/" }
                          .GetValue<VaultInfo>();
            return vlt;
        }

        public RegistrationInfo[] GetRegistrations()
        {
            var vaultInfo = this.Vault();
            var registrationInfos = vaultInfo.Registrations.Values.ToArray();
            return registrationInfos;
        }

        public string[] GetDomainsByEmail(string email)
        {
            var v = Vault();

            var r = v.Registrations.Values
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

        public RegistrationInfo Register(string email)
        {
            var emails = email.Split(';').Select(s => $"mailto:{s?.Trim()}").ToArray();
            var regInfo = GetRegistrationByEmail(emails);
            regInfo = regInfo != null ? null : this.CreateOrUpdateRegistration(null, emails);
            return regInfo;
        }

        private RegistrationInfo GetRegistrationByEmail(string[] emails)
        {
            var v = Vault();
            var regInfo = v.Registrations.Values
                .FirstOrDefault(x => x.Registration.Contacts.Intersect(emails).Any());
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
            using (var vlt = VaultHelper.GetVault(null))
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

        public bool IdentifierExists(string idref)
        {
            IDictionary[] allIds = new GetIdentifier()
                .GetValues()
                .Where(x => x != null)
                .Select(x => (x as object).ToDictionary()).ToArray();

            return allIds
                .Any(x => (x["Alias"] ?? "").ToString() == idref);
        }

        private IdentifierInfo[] GetAllIdentifiers(RegistrationInfo regInfo)
        {
            return this.GetAllIdentifiers().Where(x => x.RegistrationRef == regInfo?.Id).ToArray();
        }

        private IdentifierInfo[] GetAllIdentifiers()
        {
            using (var vlt = VaultHelper.GetVault(null))
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
                using (var vlt = VaultHelper.GetVault(null))
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
        public IdentifierInfo[] GetIdentifiers([NotNull] RegistrationInfo regInfo, [NotNull] string domain)
        {
            if (regInfo == null) throw new ArgumentNullException(nameof(regInfo));
            if (domain == null) throw new ArgumentNullException(nameof(domain));

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
            using (var vlt = VaultHelper.GetVault(null))
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                var alias2 = alias;
                var iis = v.Identifiers.Values.Where(x => (alias2 == null || x.Alias == alias2) && (dns == null || x.Dns == dns));
                if (regInfo == null)
                    return iis.ToArray();

                var iisOfReg = iis.Where(ii => ii.RegistrationRef == regInfo.Id);
                var iisOfRegArray = iisOfReg as IdentifierInfo[] ?? iisOfReg.ToArray();
                if (iisOfRegArray.Any())
                    return iisOfRegArray.ToArray();

                iis = v.Identifiers.Values.Where(x => x.Alias == alias2 || x.Dns == dns);

                if (!allowCreation)
                    return new IdentifierInfo[0];

                if (iis.Any())
                    throw new InvalidOperationException(Messages.CannotCreateIdentifierDomainAlreadyUsed);

                // if alias was not provided, get a valid one that is not used alrady
                var count = v.Identifiers.Values.Count();
                for (int i = 0; i < 100; i++)
                {
                    alias = "dns" + (count + i + 1);
                    if (!v.Identifiers.ContainsKey(alias))
                        break;
                }

                // see ACMESharp.POSH.NewIdentifier
                AuthorizationState authzState = null;
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

                    authzState = c.AuthorizeIdentifier(dns);
                    iiNew.Authorization = authzState;

                    if (v.Identifiers == null)
                        v.Identifiers = new EntityDictionary<IdentifierInfo>();

                    v.Identifiers.Add(iiNew);
                }

                vlt.SaveVault(v);

                return new[] { iiNew };
            }
        }

        public string GetTos(string email)
        {
            var v = this.Vault();
            var r = v.Registrations.Values
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
        public CertificateInfo[] GetCertificates(RegistrationInfo regInfo, string domain)
        {
            var v = this.Vault();

            var allIds = this.GetAllIdentifiers(regInfo);

            var identifierInfo = allIds.SingleOrDefault(x => x != null && (x.Dns ?? "") == domain);

            if (identifierInfo == null)
                return new CertificateInfo[0];

            var result = v.Certificates?.Values?.Where(c => c.IdentifierRef == identifierInfo.Id).ToArray();
            return result ?? new CertificateInfo[0];
        }

        public void DeleteRegistration(RegistrationInfo value)
        {
            if (value == null)
                return;

            using (var vlt = VaultHelper.GetVault(null))
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

            using (var vlt = VaultHelper.GetVault(null))
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
    }
}