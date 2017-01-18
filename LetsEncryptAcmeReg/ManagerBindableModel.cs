using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.Vault.Model;

namespace LetsEncryptAcmeReg
{
    public class ManagerBindableModel
    {
        public Bindable<string> Email { get; } = new Bindable<string>();
        public Bindable<string> Domain { get; } = new Bindable<string>();
        public Bindable<string> Challenge { get; } = new Bindable<string>();
        public Bindable<string> Target { get; } = new Bindable<string>();
        public Bindable<string> Key { get; } = new Bindable<string>();
        public Bindable<string> SiteRoot { get; } = new Bindable<string>();
        public Bindable<string> Certificate { get; } = new Bindable<string>();
        public Bindable<string> Issuer { get; } = new Bindable<string>();
        public Bindable<string> CertificateType { get; } = new Bindable<string>();
        public Bindable<string> Password { get; } = new Bindable<string>();
        public Bindable<bool> ShowPassword { get; } = new Bindable<bool>();

        public Bindable<RegistrationInfo> CurrentRegistration { get; } = new Bindable<RegistrationInfo>();

        public Bindable<bool> IsEmailValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsDomainValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsChallengeValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsTargetValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsKeyValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsSiteRootValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsCertificateValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsIssuerValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsTypeValid { get; } = new Bindable<bool>();
        public Bindable<bool> IsPasswordValid { get; } = new Bindable<bool>();

        public Bindable<bool> AutoAddDomain { get; } = new Bindable<bool>();
        public Bindable<bool> AutoCreateChallenge { get; } = new Bindable<bool>();
        public Bindable<bool> AutoSaveChallenge { get; } = new Bindable<bool>();
        public Bindable<bool> AutoCommitChallenge { get; } = new Bindable<bool>();
        public Bindable<bool> AutoTestChallenge { get; } = new Bindable<bool>();
        public Bindable<bool> AutoValidateChallenge { get; } = new Bindable<bool>();
        public Bindable<bool> AutoCreateCertificate { get; } = new Bindable<bool>();
        public Bindable<bool> AutoSubmitCertificate { get; } = new Bindable<bool>();
        public Bindable<bool> AutoSaveCertificate { get; } = new Bindable<bool>();

        public Bindable<bool> IsRegistrationCreated { get; } = new Bindable<bool>();
        public Bindable<bool> IsDomainCreated { get; } = new Bindable<bool>();
        public Bindable<string[]> Domains { get; } = new Bindable<string[]>();
        public Bindable<RegistrationInfo[]> Registrations { get; } = new Bindable<RegistrationInfo[]>();

        public Bindable<string[]> Challenges { get; } = new Bindable<string[]>();
        public Bindable<string[]> Certificates { get; } = new Bindable<string[]>();
    }
}