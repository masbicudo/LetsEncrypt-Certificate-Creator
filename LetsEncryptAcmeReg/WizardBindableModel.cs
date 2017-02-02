using System;
using ACMESharp;
using ACMESharp.Vault.Model;

namespace LetsEncryptAcmeReg
{
    public class WizardBindableModel
    {
        // regex for valid lines in this file:
        // public Bindable(<(?><(?<c>)|[^<>)]+|(?:>|$)(?<-c>))*(?:>|$))\s(\S*)\s\{ get; \} = new Bindable\1\(nameof\(\2\)\);

        public Bindable<DateTime> Now { get; } = new Bindable<DateTime>(nameof(Now));
        public Bindable<DateTime> Date { get; } = new Bindable<DateTime>(nameof(Date), flags: BindableOptions.EqualMeansUnchanged);

        public Bindable<string> Email { get; } = new Bindable<string>(nameof(Email), flags: BindableOptions.EqualMeansUnchanged);
        public Bindable<string> Domain { get; } = new Bindable<string>(nameof(Domain), flags: BindableOptions.EqualMeansUnchanged);
        public Bindable<string> Challenge { get; } = new Bindable<string>(nameof(Challenge));
        public Bindable<string> Target { get; } = new Bindable<string>(nameof(Target));
        public Bindable<string> Key { get; } = new Bindable<string>(nameof(Key));
        public Bindable<string> SiteRoot { get; } = new Bindable<string>(nameof(SiteRoot));
        public Bindable<string> Certificate { get; } = new Bindable<string>(nameof(Certificate), flags: BindableOptions.EqualMeansUnchanged);
        public Bindable<string> Issuer { get; } = new Bindable<string>(nameof(Issuer));
        public Bindable<CertType> CertificateType { get; } = new Bindable<CertType>(nameof(CertificateType));
        public Bindable<string> Password { get; } = new Bindable<string>(nameof(Password));
        public Bindable<bool> ShowPassword { get; } = new Bindable<bool>(nameof(ShowPassword));
        public Bindable<string> GitUserName { get; } = new Bindable<string>(nameof(GitUserName));
        public Bindable<string> GitPassword { get; } = new Bindable<string>(nameof(GitPassword));
        public Bindable<string> SavePath { get; } = new Bindable<string>(nameof(SavePath));

        public Bindable<string> ExpandedSavePath { get; } = new Bindable<string>(nameof(ExpandedSavePath));
        public Bindable<string> FileRelativePath { get; } = new Bindable<string>(nameof(FileRelativePath));
        public Bindable<string> FilePath { get; } = new Bindable<string>(nameof(FilePath));
        public Bindable<string[]> Files { get; } = new Bindable<string[]>(nameof(Files));
        public Bindable<RegistrationInfo> CurrentRegistration { get; } = new Bindable<RegistrationInfo>(nameof(CurrentRegistration), flags: BindableOptions.EqualMeansUnchanged);
        public Bindable<AuthorizationState> CurrentAuthState { get; } = new Bindable<AuthorizationState>(nameof(CurrentAuthState));
        public Bindable<AuthorizeChallenge> CurrentChallenge { get; } = new Bindable<AuthorizeChallenge>(nameof(CurrentChallenge));
        public Bindable<CertificateInfo> CurrentCertificate { get; } = new Bindable<CertificateInfo>(nameof(CurrentCertificate));
        public Bindable<string[]> Certificates { get; } = new Bindable<string[]>(nameof(Certificates));

        public Bindable<bool> UpdateConfigYml { get; } = new Bindable<bool>(nameof(UpdateConfigYml));
        public Bindable<bool> UpdateCname { get; } = new Bindable<bool>(nameof(UpdateCname));

        public Bindable<bool> IsRegistrationCreated { get; } = new Bindable<bool>(nameof(IsRegistrationCreated));
        public Bindable<bool> IsDomainCreated { get; } = new Bindable<bool>(nameof(IsDomainCreated));
        public Bindable<bool> IsChallengeCreated { get; } = new Bindable<bool>(nameof(IsChallengeCreated));
        public Bindable<bool> ChallengeHasFile { get; } = new Bindable<bool>(nameof(ChallengeHasFile));
        public Bindable<string[]> Domains { get; } = new Bindable<string[]>(nameof(Domains));
        public Bindable<RegistrationInfo[]> Registrations { get; } = new Bindable<RegistrationInfo[]>(nameof(Registrations));

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
        public Bindable<bool> IsPasswordEnabled { get; } = new Bindable<bool>(nameof(IsPasswordEnabled));

        public Bindable<bool> AutoRegister { get; } = new Bindable<bool>(nameof(AutoRegister));
        public Bindable<bool> AutoAcceptTos { get; } = new Bindable<bool>(nameof(AutoAcceptTos));
        public Bindable<bool> AutoAddDomain { get; } = new Bindable<bool>(nameof(AutoAddDomain));
        public Bindable<bool> AutoCreateChallenge { get; } = new Bindable<bool>(nameof(AutoCreateChallenge));
        public Bindable<bool> AutoSaveChallenge { get; } = new Bindable<bool>(nameof(AutoSaveChallenge));
        public Bindable<bool> AutoCommitChallenge { get; } = new Bindable<bool>(nameof(AutoCommitChallenge));
        public Bindable<bool> AutoTestChallenge { get; } = new Bindable<bool>(nameof(AutoTestChallenge));
        public Bindable<bool> AutoValidateChallenge { get; } = new Bindable<bool>(nameof(AutoValidateChallenge));
        public Bindable<bool> AutoUpdateStatus { get; } = new Bindable<bool>(nameof(AutoUpdateStatus));
        public Bindable<bool> AutoCreateCertificate { get; } = new Bindable<bool>(nameof(AutoCreateCertificate));
        public Bindable<bool> AutoSubmitCertificate { get; } = new Bindable<bool>(nameof(AutoSubmitCertificate));
        public Bindable<bool> AutoGetIssuerCertificate { get; } = new Bindable<bool>(nameof(AutoGetIssuerCertificate));
        public Bindable<bool> AutoSaveOrShowCertificate { get; } = new Bindable<bool>(nameof(AutoSaveOrShowCertificate));

        public Bindable<int?> AutoRegisterTimer { get; } = new Bindable<int?>(nameof(AutoRegisterTimer));
        public Bindable<int?> AutoAcceptTosTimer { get; } = new Bindable<int?>(nameof(AutoAcceptTosTimer));
        public Bindable<int?> AutoAddDomainTimer { get; } = new Bindable<int?>(nameof(AutoAddDomainTimer));
        public Bindable<int?> AutoCreateChallengeTimer { get; } = new Bindable<int?>(nameof(AutoCreateChallengeTimer));
        public Bindable<int?> AutoSaveChallengeTimer { get; } = new Bindable<int?>(nameof(AutoSaveChallengeTimer));
        public Bindable<int?> AutoCommitChallengeTimer { get; } = new Bindable<int?>(nameof(AutoCommitChallengeTimer));
        public Bindable<int?> AutoTestChallengeTimer { get; } = new Bindable<int?>(nameof(AutoTestChallengeTimer));
        public Bindable<int?> AutoValidateChallengeTimer { get; } = new Bindable<int?>(nameof(AutoValidateChallengeTimer));
        public Bindable<int?> AutoUpdateStatusTimer { get; } = new Bindable<int?>(nameof(AutoUpdateStatusTimer));
        public Bindable<int?> AutoCreateCertificateTimer { get; } = new Bindable<int?>(nameof(AutoCreateCertificateTimer));
        public Bindable<int?> AutoSubmitCertificateTimer { get; } = new Bindable<int?>(nameof(AutoSubmitCertificateTimer));
        public Bindable<int?> AutoGetIssuerCertificateTimer { get; } = new Bindable<int?>(nameof(AutoGetIssuerCertificateTimer));
        public Bindable<int?> AutoSaveOrShowCertificateTimer { get; } = new Bindable<int?>(nameof(AutoSaveOrShowCertificateTimer));

        public Bindable<int?> AutoRegisterRetry { get; } = new Bindable<int?>(nameof(AutoRegisterRetry));
        public Bindable<int?> AutoAcceptTosRetry { get; } = new Bindable<int?>(nameof(AutoAcceptTosRetry));
        public Bindable<int?> AutoAddDomainRetry { get; } = new Bindable<int?>(nameof(AutoAddDomainRetry));
        public Bindable<int?> AutoCreateChallengeRetry { get; } = new Bindable<int?>(nameof(AutoCreateChallengeRetry));
        public Bindable<int?> AutoSaveChallengeRetry { get; } = new Bindable<int?>(nameof(AutoSaveChallengeRetry));
        public Bindable<int?> AutoCommitChallengeRetry { get; } = new Bindable<int?>(nameof(AutoCommitChallengeRetry));
        public Bindable<int?> AutoTestChallengeRetry { get; } = new Bindable<int?>(nameof(AutoTestChallengeRetry));
        public Bindable<int?> AutoValidateChallengeRetry { get; } = new Bindable<int?>(nameof(AutoValidateChallengeRetry));
        public Bindable<int?> AutoUpdateStatusRetry { get; } = new Bindable<int?>(nameof(AutoUpdateStatusRetry), flags: BindableOptions.AllowRecursiveSets);
        public Bindable<int?> AutoCreateCertificateRetry { get; } = new Bindable<int?>(nameof(AutoCreateCertificateRetry));
        public Bindable<int?> AutoSubmitCertificateRetry { get; } = new Bindable<int?>(nameof(AutoSubmitCertificateRetry));
        public Bindable<int?> AutoGetIssuerCertificateRetry { get; } = new Bindable<int?>(nameof(AutoGetIssuerCertificateRetry));
        public Bindable<int?> AutoSaveOrShowCertificateRetry { get; } = new Bindable<int?>(nameof(AutoSaveOrShowCertificateRetry));

        public Bindable<bool> CanRegister { get; } = new Bindable<bool>(nameof(CanRegister));
        public Bindable<bool> CanAcceptTos { get; } = new Bindable<bool>(nameof(CanAcceptTos));
        public Bindable<bool> CanAddDomain { get; } = new Bindable<bool>(nameof(CanAddDomain));
        public Bindable<bool> CanCreateChallenge { get; } = new Bindable<bool>(nameof(CanCreateChallenge));
        public Bindable<bool> CanSaveChallenge { get; } = new Bindable<bool>(nameof(CanSaveChallenge));
        public Bindable<bool> CanCommitChallenge { get; } = new Bindable<bool>(nameof(CanCommitChallenge));
        public Bindable<bool> CanTestChallenge { get; } = new Bindable<bool>(nameof(CanTestChallenge));
        public Bindable<bool> CanValidateChallenge { get; } = new Bindable<bool>(nameof(CanValidateChallenge));
        public Bindable<bool> CanUpdateStatus { get; } = new Bindable<bool>(nameof(CanUpdateStatus));
        public Bindable<bool> CanCreateCertificate { get; } = new Bindable<bool>(nameof(CanCreateCertificate));
        public Bindable<bool> CanSubmitCertificate { get; } = new Bindable<bool>(nameof(CanSubmitCertificate));
        public Bindable<bool> CanGetIssuerCertificate { get; } = new Bindable<bool>(nameof(CanGetIssuerCertificate));
        public Bindable<bool> CanSaveCertificate { get; } = new Bindable<bool>(nameof(CanSaveCertificate));
        public Bindable<bool> CanShowCertificate { get; } = new Bindable<bool>(nameof(CanShowCertificate));
    }
}