using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.Vault.Model;

namespace LetsEncryptAcmeReg
{
    public class ManagerBindableModel
    {
        // regex for valid lines in this file:
        // public Bindable(<(?><(?<c>)|[^<>)]+|(?:>|$)(?<-c>))*(?:>|$))\s(\S*)\s\{ get; \} = new Bindable\1\(nameof\(\2\)\);

        public Bindable<string> Email { get; } = new Bindable<string>(nameof(Email));
        public Bindable<string> Domain { get; } = new Bindable<string>(nameof(Domain));
        public Bindable<string> Challenge { get; } = new Bindable<string>(nameof(Challenge));
        public Bindable<string> Target { get; } = new Bindable<string>(nameof(Target));
        public Bindable<string> Key { get; } = new Bindable<string>(nameof(Key));
        public Bindable<string> SiteRoot { get; } = new Bindable<string>(nameof(SiteRoot));
        public Bindable<string> Certificate { get; } = new Bindable<string>(nameof(Certificate));
        public Bindable<string> Issuer { get; } = new Bindable<string>(nameof(Issuer));
        public Bindable<string> CertificateType { get; } = new Bindable<string>(nameof(CertificateType));
        public Bindable<string> Password { get; } = new Bindable<string>(nameof(Password));
        public Bindable<bool> ShowPassword { get; } = new Bindable<bool>(nameof(ShowPassword));

        public Bindable<string> FileRelativePath { get; } = new Bindable<string>(nameof(FileRelativePath));
        public Bindable<string> FilePath { get; } = new Bindable<string>(nameof(FilePath));
        public Bindable<RegistrationInfo> CurrentRegistration { get; } = new Bindable<RegistrationInfo>(nameof(CurrentRegistration));
        public Bindable<AuthorizationState> CurrentAuthState { get; } = new Bindable<AuthorizationState>(nameof(CurrentAuthState));
        public Bindable<AuthorizeChallenge> CurrentChallenge { get; } = new Bindable<AuthorizeChallenge>(nameof(CurrentChallenge));

        public Bindable<bool> IsEmailValid { get; } = new Bindable<bool>(nameof(IsEmailValid));
        public Bindable<bool> IsDomainValid { get; } = new Bindable<bool>(nameof(IsDomainValid));
        public Bindable<bool> IsChallengeValid { get; } = new Bindable<bool>(nameof(IsChallengeValid));
        public Bindable<bool> IsTargetValid { get; } = new Bindable<bool>(nameof(IsTargetValid));
        public Bindable<bool> IsKeyValid { get; } = new Bindable<bool>(nameof(IsKeyValid));
        public Bindable<bool> IsSiteRootValid { get; } = new Bindable<bool>(nameof(IsSiteRootValid));
        public Bindable<bool> IsCertificateValid { get; } = new Bindable<bool>(nameof(IsCertificateValid));
        public Bindable<bool> IsIssuerValid { get; } = new Bindable<bool>(nameof(IsIssuerValid));
        public Bindable<bool> IsTypeValid { get; } = new Bindable<bool>(nameof(IsTypeValid));
        public Bindable<bool> IsPasswordValid { get; } = new Bindable<bool>(nameof(IsPasswordValid));

        public Bindable<bool> AutoAddDomain { get; } = new Bindable<bool>(nameof(AutoAddDomain));
        public Bindable<bool> AutoCreateChallenge { get; } = new Bindable<bool>(nameof(AutoCreateChallenge));
        public Bindable<bool> AutoSaveChallenge { get; } = new Bindable<bool>(nameof(AutoSaveChallenge));
        public Bindable<bool> AutoCommitChallenge { get; } = new Bindable<bool>(nameof(AutoCommitChallenge));
        public Bindable<bool> AutoTestChallenge { get; } = new Bindable<bool>(nameof(AutoTestChallenge));
        public Bindable<bool> AutoValidateChallenge { get; } = new Bindable<bool>(nameof(AutoValidateChallenge));
        public Bindable<bool> AutoCreateCertificate { get; } = new Bindable<bool>(nameof(AutoCreateCertificate));
        public Bindable<bool> AutoSubmitCertificate { get; } = new Bindable<bool>(nameof(AutoSubmitCertificate));
        public Bindable<bool> AutoSaveCertificate { get; } = new Bindable<bool>(nameof(AutoSaveCertificate));

        public Bindable<bool> IsRegistrationCreated { get; } = new Bindable<bool>(nameof(IsRegistrationCreated));
        public Bindable<bool> IsDomainCreated { get; } = new Bindable<bool>(nameof(IsDomainCreated));
        public Bindable<string[]> Domains { get; } = new Bindable<string[]>(nameof(Domains));
        public Bindable<RegistrationInfo[]> Registrations { get; } = new Bindable<RegistrationInfo[]>(nameof(Registrations));

        public Bindable<string[]> Challenges { get; } = new Bindable<string[]>(nameof(Challenges));
        public Bindable<string[]> Certificates { get; } = new Bindable<string[]>(nameof(Certificates));
        public Bindable<bool> IsChallengeCreated { get; } = new Bindable<bool>(nameof(IsChallengeCreated));
        public Bindable<bool> ChallengeHasFile { get; } = new Bindable<bool>(nameof(ChallengeHasFile));
    }
}