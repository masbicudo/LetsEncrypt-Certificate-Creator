using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.POSH;
using ACMESharp.Vault.Model;
using JetBrains.Annotations;
using LibGit2Sharp;
using Signature = LibGit2Sharp.Signature;

namespace LetsEncryptAcmeReg
{
    public class Controller
    {
        private readonly Acme acme;
        public static int MaxRetries = 3;

        public Controller(Acme acme)
        {
            this.acme = acme;
            this.Model = new WizardBindableModel();
            this.ManagerModel = new ManagerBindableModel();
        }

        public WizardBindableModel Model { get; }
        public ManagerBindableModel ManagerModel { get; }
        public Action<Exception> Error { get; set; }
        public Action<string> Warn { get; set; }
        public Action<string> Success { get; set; }
        public Action<string> Log { get; set; }

        public Action Initialize()
        {
            var mo = this.Model;
            var ma = this.ManagerModel;
            Action init = null;

            // Primary initialization:
            init += () => mo.Registrations.Value = this.acme.GetRegistrations();

            // Complex relations:
            //      These relations are built by using expressions.
            //      Every bindable object in the right hand side will
            //      have a changed event added automatically.
            init += mo.IsEmailValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Email.Value));
            init += mo.IsRegistrationCreated.BindExpression(() => mo.CurrentRegistration.Value != null);
            init += mo.Domains.BindExpression(() => this.acme.GetDomainsByEmail(mo.Email.Value).OrderBy(x => x).ToArray());
            init += mo.IsDomainValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Domain.Value) && mo.IsEmailValid.Value);
            init += mo.IsDomainCreated.BindExpression(() => mo.Domains.Value._S(v => v.Any(i => i == mo.Domain.Value)) ?? false);
            init += mo.IsChallengeValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Challenge.Value) && mo.IsDomainValid.Value);
            init += mo.IsChallengeCreated.BindExpression(() => mo.CurrentAuthState.Value._(v => v.Challenges._S(c => c.Any(x => x.Challenge != null))) ?? false);
            init += mo.IsTargetValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Target.Value) && mo.IsChallengeCreated.Value);
            init += mo.IsKeyValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Key.Value) && mo.IsChallengeCreated.Value);
            init += mo.IsSiteRootValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.SiteRoot.Value) && mo.IsChallengeCreated.Value);
            init += mo.ChallengeHasFile.BindExpression(() => mo.IsTargetValid.Value && mo.IsKeyValid.Value
                    && (mo.Challenge.Value != "http-01" || !string.IsNullOrWhiteSpace(mo.SiteRoot.Value)));

            init += mo.CurrentAuthState.BindExpression(() => this.CurrentAuthState_Value(mo.CurrentRegistration.Value, mo.Domain.Value));
            init += mo.CurrentChallenge.BindExpression(() => this.CurrentChallenge_Value(mo.CurrentAuthState.Value, mo.Challenge.Value));

            init += mo.Target.BindExpression(() => mo.CurrentChallenge.Value._(v => (v.Challenge as HttpChallenge)._(c => c.FileUrl)) ?? "");
            init += mo.Key.BindExpression(() => mo.CurrentChallenge.Value._(v => v.Challenge._(c => (c.Answer as HttpChallengeAnswer)._(a => a.KeyAuthorization))) ?? "");
            init += mo.FileRelativePath.BindExpression(() => mo.CurrentChallenge.Value._(v => (v.Challenge as HttpChallenge)._(c => c.FilePath)._(s => s.Replace('/', '\\'))) ?? "");

            init += mo.FilePath.BindExpression(() => mo.ChallengeHasFile.Value ? Path.Combine(mo.SiteRoot.Value, mo.FileRelativePath.Value) : "");

            init += mo.CanRegister.BindExpression(() => mo.IsEmailValid.Value);
            init += mo.CanAcceptTos.BindExpression(() => this.CanAcceptTos_Value(mo.CurrentRegistration.Value));
            init += mo.CanAddDomain.BindExpression(() => mo.IsDomainValid.Value && !mo.IsDomainCreated.Value);
            init += mo.CanCreateChallenge.BindExpression(() => mo.IsChallengeValid.Value);
            init += mo.CanSaveChallenge.BindExpression(() => mo.ChallengeHasFile.Value);

            init += mo.Files.BindExpression(() => this.Files_Value(mo.SiteRoot.Value, mo.FileRelativePath.Value));

            return init;
        }

        private string[] Files_Value(string siteRoot, string indexRelative)
        {
            if (siteRoot == null)
                return new string[0];

            return new[]
            {
                indexRelative == null ? "" : Path.Combine(siteRoot, indexRelative, "index.html"),
                Path.Combine(siteRoot, "CNAME"),
                Path.Combine(siteRoot, "_config.yml"),
            };
        }

        private bool CanAcceptTos_Value(RegistrationInfo regInfo)
            => CatchError(() => regInfo?.Registration != null && regInfo.Registration.TosAgreementUri == null);

        private AuthorizationState CurrentAuthState_Value([CanBeNull] RegistrationInfo regInfo, [CanBeNull] string dns)
            => CatchError(() => regInfo == null || string.IsNullOrWhiteSpace(dns) ? null : this.acme.GetIdentifiers(regInfo, dns)?.SingleOrDefault());

        private AuthorizeChallenge CurrentChallenge_Value(AuthorizationState authState, string challengeType)
            => CatchError(() => authState?.Challenges?.SingleOrDefault(x => x.Type == challengeType));

        private T CatchError<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                this.Error(ex);
            }
            return default(T);
        }

        public async Task TosLink()
        {
            await Task.Delay(1);

            Process.Start(this.acme.GetTos(this.Model.Email.Value));
        }

        private async Task AutoCaller(Bindable<bool> enabled, Bindable<bool> isAuto, Bindable<int?> retry, Bindable<int?> timer, Action action, [CanBeNull] Bindable<bool> isNextAuto, [CanBeNull] Func<Task> next)
        {
            if (!enabled.Value)
                return;

            for (retry.Value = 1; retry.Value <= MaxRetries; retry.Value++)
            {
                try
                {
                    timer.Value = 0; // this indicates that the action is running
                    action();
                    timer.Value = null; // stop timer, indicating that this is not running
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                    timer.Value = 30000 - 1; // an exception starts a timer to wait until the next automatic retry
                }

                while (timer.Value.HasValue && timer.Value.Value > 0)
                {
                    if (!isAuto.Value)
                    {
                        timer.Value = null; // stop timer, indicating that this is not running
                        break;
                    }

                    var waitTime = Math.Min(50, timer.Value.Value);
                    await Task.Delay(waitTime);
                    timer.Value -= waitTime;
                }

                if (!isAuto.Value || timer.Value == null)
                    break;
            }

            if (isNextAuto?.Value == true && next != null)
            {
                await Task.Delay(1);
                await next();
            }
        }

        public async Task Register()
        {
            await AutoCaller(
                this.Model.CanRegister,
                this.Model.AutoRegister,
                this.Model.AutoRegisterRetry,
                this.Model.AutoRegisterTimer,
                () =>
                {
                    var newRegistration = this.acme.Register(this.Model.Email.Value);
                    if (newRegistration != null)
                    {
                        var newList = this.Model.Registrations.Value.Concat(new[] { newRegistration }).ToArray();
                        this.Model.Registrations.Value = newList;
                        this.Success("Registration created.");
                    }
                    else
                        throw new InvalidOperationException("E-mail already used in another registration.");
                },
                this.Model.AutoAcceptTos,
                this.AcceptTos);
        }

        public async Task AcceptTos()
        {
            await AutoCaller(
                this.Model.CanAcceptTos,
                this.Model.AutoAcceptTos,
                this.Model.AutoAcceptTosRetry,
                this.Model.AutoAcceptTosTimer,
                () =>
                {
                    this.Model.CurrentRegistration.Value = this.acme.AcceptTos(this.Model.CurrentRegistration.Value);
                },
                this.Model.AutoAddDomain,
                this.AddDomain);
        }

        public async Task AddDomain()
        {
            await AutoCaller(
                this.Model.CanAddDomain,
                this.Model.AutoAddDomain,
                this.Model.AutoAddDomainRetry,
                this.Model.AutoAddDomainTimer,
                () =>
                {
                    // the btnAddDomain should be disabled when something already exists,
                    // so the following function is always used to create a new identity
                    var states = this.acme.GetOrCreateIdentifier(
                        this.Model.CurrentRegistration.Value,
                        this.Model.Domain.Value
                        );

                    if (states.Length == 1)
                        this.Model.CurrentAuthState.Value = states[0];

                    if (states.Length == 0)
                        throw new Exception("Identity could not be created.");

                    if (states.Length > 1)
                        this.Warn(Messages.MultipleIdentities);

                    this.Model.CurrentAuthState.Value = states[0];
                },
                this.Model.AutoCreateChallenge,
                this.CreateChallenge);
        }

        public async Task CreateChallenge()
        {
            await AutoCaller(
                this.Model.CanCreateChallenge,
                this.Model.AutoCreateChallenge,
                this.Model.AutoCreateChallengeRetry,
                this.Model.AutoCreateChallengeTimer,
                () =>
                {
                    var idref = this.acme.GetIdentifierAlias(
                        this.Model.CurrentRegistration.Value,
                        this.Model.Domain.Value);

                    var state =
                        new CompleteChallenge
                        {
                            Handler = "Manual",
                            IdentifierRef = idref,
                            ChallengeType = this.Model.Challenge.Value,
                            Repeat = SwitchParameter.Present,
                            Regenerate = SwitchParameter.Present,
                        }
                        .GetValue<AuthorizationState>();

                    this.Model.CurrentAuthState.Value = state;
                },
                this.Model.AutoSaveChallenge,
                this.SaveChallenge);
        }

        public async Task SaveChallenge()
        {
            await AutoCaller(
                this.Model.CanSaveChallenge,
                this.Model.AutoSaveChallenge,
                this.Model.AutoSaveChallengeRetry,
                this.Model.AutoSaveChallengeTimer,
                () =>
                {
                    Directory.CreateDirectory(this.Model.FilePath.Value);

                    using (var fs = File.Open(Path.Combine(this.Model.FilePath.Value, "index.html"), FileMode.Create, FileAccess.ReadWrite))
                    using (var sw = new StreamWriter(fs))
                        sw.Write(this.Model.Key);
                },
                this.Model.AutoCommitChallenge,
                this.CommitChallenge);
        }

        public async Task CommitChallenge()
        {
            await AutoCaller(
                this.Model.CanCommitChallenge,
                this.Model.AutoCommitChallenge,
                this.Model.AutoCommitChallengeRetry,
                this.Model.AutoCommitChallengeTimer,
                () =>
                {
                    try
                    {
                        using (var repo = new Repository(this.Model.FilePath.Value))
                        {
                            // Stage the file
                            repo.Index.Add(Path.Combine(this.Model.FilePath.Value, "index.html"));

                            // Create the committer's signature and commit
                            Signature author = new Signature("James", "@jugglingnutcase", DateTime.Now);
                            Signature committer = author;

                            // Commit to the repository
                            Commit commit = repo.Commit("Here's a commit i made!", author, committer);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception:RepoActions:StageChanges " + ex.Message);
                    }
                },
                this.Model.AutoTestChallenge,
                this.TestChallenge);
        }

        public async Task TestChallenge()
        {
            await AutoCaller(
                this.Model.CanTestChallenge,
                this.Model.AutoTestChallenge,
                this.Model.AutoTestChallengeRetry,
                this.Model.AutoTestChallengeTimer,
                () =>
                {
                    throw new NotImplementedException();
                },
                this.Model.AutoValidateChallenge,
                this.Validate);
        }

        public async Task Validate()
        {
            await AutoCaller(
                this.Model.CanValidateChallenge,
                this.Model.AutoValidateChallenge,
                this.Model.AutoValidateChallengeRetry,
                this.Model.AutoValidateChallengeTimer,
                () =>
                {
                    throw new NotImplementedException();
                },
                this.Model.AutoCreateCertificate,
                this.CreateCertificate);
        }

        public async Task CreateCertificate()
        {
            await AutoCaller(
                this.Model.CanCreateCertificate,
                this.Model.AutoCreateCertificate,
                this.Model.AutoCreateCertificateRetry,
                this.Model.AutoCreateCertificateTimer,
                () =>
                {
                    throw new NotImplementedException();
                },
                this.Model.AutoSubmitCertificate,
                this.SubmitCertificate);
        }

        public async Task SubmitCertificate()
        {
            await AutoCaller(
                this.Model.CanSubmitCertificate,
                this.Model.AutoSubmitCertificate,
                this.Model.AutoSubmitCertificateRetry,
                this.Model.AutoSubmitCertificateTimer,
                () =>
                {
                    throw new NotImplementedException();
                },
                this.Model.AutoSaveCertificate,
                this.SaveCertificate);
        }

        public async Task SaveCertificate()
        {
            await AutoCaller(
                this.Model.CanSaveCertificate,
                this.Model.AutoSaveCertificate,
                this.Model.AutoSaveCertificateRetry,
                this.Model.AutoSaveCertificateTimer,
                () =>
                {
                    throw new NotImplementedException();
                },
                null,
                null);
        }

        public async Task RegistrationListChanged()
        {
            await Task.Delay(1);

            this.Model.Domains.Value = this.acme.GetDomainsByRegistration(this.Model.CurrentRegistration.Value);
        }

        public async Task DomainListChanged()
        {
            await Task.Delay(1);

            var states = this.acme.GetIdentifiers(this.Model.CurrentRegistration.Value, this.Model.Domain.Value);

            if (states.Length == 0)
                this.Error(new Exception(Messages.DomainNotFound));

            else if (states.Length > 1)
                this.Warn(Messages.MultipleIdentities);

            else if (states.Length == 1)
            {
                var challenges = this.acme.GetChallengesByDomain(this.Model.CurrentRegistration.Value, this.Model.Domain.Value)
                    .Select(c => c.Type)
                    .ToArray();
                this.ManagerModel.Challenges.Value = challenges;

                var certs = this.acme.GetCertificates(this.Model.CurrentRegistration.Value, this.Model.Domain.Value)
                    .Select(c => c.Alias)
                    .ToArray();
                this.ManagerModel.Certificates.Value = certs;
            }
        }
    }
}