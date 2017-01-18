using System;
using System.Linq;
using System.Management.Automation;
using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.POSH;
using ACMESharp.POSH.Util;
using ACMESharp.Vault.Model;
using ACMESharp.Vault.Profile;

namespace LetsEncryptAcmeReg
{
    public class Acme
    {
        public VaultInfo Vault()
        {
            VaultInfo vlt = new GetVault().GetValue<VaultInfo>()
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

        public RegistrationInfo RegisterOrGet(string email)
        {
            var emails = email.Split(';').Select(s => $"mailto:{s?.Trim()}").ToArray();

            var v = Vault();

            var r = v.Registrations.Values
                .FirstOrDefault(x => x.Registration.Contacts.Intersect(emails).Any());

            if (r != null)
                return r;

            new NewRegistration { Contacts = new[] { $"mailto:{email}" } }.Run();
            r = new UpdateRegistration { AcceptTos = SwitchParameter.Present }.GetValue<RegistrationInfo>();
            return r;
        }

        private IdentifierInfo[] GetAllIdentifiers(RegistrationInfo regInfo)
        {
            using (var vlt = ACMESharp.POSH.Util.VaultHelper.GetVault(null))
            {
                vlt.OpenStorage();
                var v = vlt.LoadVault();

                if (v.Identifiers == null || v.Identifiers.Count < 1)
                    return new IdentifierInfo[0];

                return v.Identifiers.Values.Where(x => x.RegistrationRef == regInfo.Id).ToArray();
            }
        }

        public string GetIdentifier(RegistrationInfo regInfo, string domain)
        {
            var allIds = this.GetAllIdentifiers(regInfo);

            var identifierInfo = allIds.SingleOrDefault(x => x != null && (x.Dns ?? "") == domain);

            string idref = identifierInfo?.Alias;
            if (identifierInfo == null)
                idref = "dns" + (allIds.Count(x => x != null) + 1);

            return idref;
        }

        public string GetTOS(string email)
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
            var idref = this.GetIdentifier(regInfo, domain);
            var state = new GetIdentifier { IdentifierRef = idref }.GetValue<AuthorizationState>();
            return state.Challenges.ToArray();
        }

        public CertificateInfo[] GetCertificates(RegistrationInfo regInfo, string domain)
        {
            var v = this.Vault();

            var allIds = this.GetAllIdentifiers(regInfo);

            var identifierInfo = allIds.SingleOrDefault(x => x != null && (x.Dns ?? "") == domain);

            var result = v.Certificates?.Values?.Where(c => c.IdentifierRef == identifierInfo.Id).ToArray();
            return result ?? new CertificateInfo[0];
        }
    }
}