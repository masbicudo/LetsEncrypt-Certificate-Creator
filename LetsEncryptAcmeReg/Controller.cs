using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.POSH;
using ACMESharp.Vault.Model;
using JetBrains.Annotations;
using LibGit2Sharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Signature = LibGit2Sharp.Signature;

namespace LetsEncryptAcmeReg
{
    public class Controller
    {
        private readonly Acme acme;

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
            init += mo.CurrentCertificate.BindExpression(() => this.CurrentCertificate_Value(mo.CurrentRegistration.Value, mo.Domain.Value, mo.Certificate.Value));

            init += mo.Target.BindExpression(() => mo.CurrentChallenge.Value._(v => (v.Challenge as HttpChallenge)._(c => c.FileUrl)) ?? "");
            init += mo.Key.BindExpression(() => mo.CurrentChallenge.Value._(v => v.Challenge._(c => (c.Answer as HttpChallengeAnswer)._(a => a.KeyAuthorization))) ?? "");
            init += mo.FileRelativePath.BindExpression(() => mo.CurrentChallenge.Value._(v => (v.Challenge as HttpChallenge)._(c => c.FilePath)._(s => s.Replace('/', '\\'))) ?? "");

            init += mo.FilePath.BindExpression(() => this.FilePath_Value(mo.ChallengeHasFile.Value, mo.SiteRoot.Value, mo.FileRelativePath.Value));

            init += mo.CanRegister.BindExpression(() => mo.IsEmailValid.Value);
            init += mo.CanAcceptTos.BindExpression(() => this.CanAcceptTos_Value(mo.CurrentRegistration.Value));
            init += mo.CanAddDomain.BindExpression(() => mo.IsDomainValid.Value && !mo.IsDomainCreated.Value);
            init += mo.CanCreateChallenge.BindExpression(() => mo.IsChallengeValid.Value);
            init += mo.CanSaveChallenge.BindExpression(() => mo.ChallengeHasFile.Value);
            init += mo.CanCommitChallenge.BindExpression(() => this.CanCommitChallenge_Value(this.Model.SiteRoot.Value));
            init += mo.CanTestChallenge.BindExpression(() => mo.IsTargetValid.Value && mo.IsKeyValid.Value);
            init += mo.CanUpdateStatus.BindExpression(() => mo.CurrentAuthState.Value != null);
            init += mo.CanValidateChallenge.BindExpression(() => false);
            init += mo.CanCreateCertificate.BindExpression(() => mo.CurrentAuthState.Value != null && mo.CurrentAuthState.Value.Status == "valid");
            init += mo.CanSubmitCertificate.BindExpression(() => false);
            init += mo.CanGetIssuerCertificate.BindExpression(() => false);
            init += mo.CanSaveCertificate.BindExpression(() => false);

            init += mo.Files.BindExpression(() => this.Files_Value(mo.SiteRoot.Value, mo.FileRelativePath.Value, mo.UpdateCname.Value, mo.UpdateConfigYml.Value));

            // when the key changes, the domain must be tested again
            mo.Key.Changed += s => mo.CanValidateChallenge.Value = false;

            return init;
        }

        private CertificateInfo CurrentCertificate_Value(RegistrationInfo regInfo, string domain, string certRef)
        {
            var result = this.acme
                .GetCertificates(regInfo, domain)
                .SingleOrDefault(x => x.Alias == certRef);

            return result;
        }

        private bool CanCommitChallenge_Value(string siteRoot)
            => CatchError(() => Path.IsPathRooted(siteRoot) && Repository.IsValid(siteRoot));

        private string FilePath_Value(bool hasFile, string siteRoot, string indexRelativePath)
            => CatchError(() => hasFile ? Path.Combine(siteRoot, indexRelativePath) : "");

        private string[] Files_Value(string siteRoot, string indexRelative, bool updateCname, bool updateConfigYml)
        {
            return CatchError(() =>
            {
                if (new[] { siteRoot, indexRelative }.AnyNullOrWhiteSpace())
                    return new string[0];

                var items = Enumerable.Empty<string>();
                items = items.Append(Path.Combine(siteRoot, indexRelative, "index.html"));
                if (updateCname) items = items.Append(Path.Combine(siteRoot, "CNAME"));
                if (updateConfigYml) items = items.Append(Path.Combine(siteRoot, "_config.yml"));
                return items.Where(x => x != null).ToArray();
            });
        }

        private bool CanAcceptTos_Value(RegistrationInfo regInfo)
            => CatchError(() => regInfo?.Registration != null && regInfo.Registration.TosAgreementUri == null);

        private AuthorizationState CurrentAuthState_Value([CanBeNull] RegistrationInfo regInfo, [CanBeNull] string dns)
            => CatchError(() => regInfo == null || string.IsNullOrWhiteSpace(dns) ? null : this.acme.GetIdentifiers(regInfo, dns).Select(ii => ii.Authorization).SingleOrDefault());

        private AuthorizeChallenge CurrentChallenge_Value(AuthorizationState authState, string challengeType)
            => CatchError(() => authState?.Challenges?.SingleOrDefault(x => x.Type == challengeType));

        /// <summary>
        /// Runs the given delegate inside a try/catch block and logs the exception if any.
        /// </summary>
        public T CatchError<T>(Func<T> func)
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

        /// <summary>
        /// Runs the given delegate inside a try/catch block and logs the exception if any.
        /// </summary>
        public void CatchError(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                this.Error(ex);
            }
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

            if (retry.Value == null)
                retry.Value = 3;

            while (retry.Value != null)
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

                // stop when there is no more remaining retries
                if (retry.Value == 0)
                    break;
                int retryCount = (int)retry.Value;
                retry.Value = Interlocked.Decrement(ref retryCount);

                while (timer.Value.HasValue && timer.Value > 0)
                {
                    if (!isAuto.Value)
                    {
                        timer.Value = null; // stop timer, indicating that this is not running
                        break;
                    }

                    // Using a stopwatch to measure the elapsed time more perfectly.
                    // If too much time passes, it indicates that the program was suspended:
                    //  - debugging the software
                    //  - computer sleeping
                    // in this case the stopwatch will indicate a very large delay,
                    // but we will not use the default amount of time instead.
                    var start = Stopwatch.GetTimestamp();
                    var waitTime = Math.Min(50, timer.Value.Value);
                    await Task.Delay(waitTime);
                    var total = (int)((double)(Stopwatch.GetTimestamp() - start) / Stopwatch.Frequency * 1000);
                    var dec = total < waitTime * 2 ? total : waitTime;
                    timer.Value -= dec;
                }

                if (!isAuto.Value || timer.Value == null)
                    break;
            }

            retry.Value = null; // indicates that there is no more retries

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
                        this.Model.CurrentAuthState.Value = states[0].Authorization;

                    if (states.Length == 0)
                        throw new Exception("Identity could not be created.");

                    if (states.Length > 1)
                        this.Warn(Messages.MultipleIdentities);

                    this.Model.CurrentAuthState.Value = states[0].Authorization;
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
                        sw.Write(this.Model.Key.Value);

                    if (this.Model.UpdateConfigYml.Value)
                        using (var fs = File.Open(Path.Combine(this.Model.SiteRoot.Value, "_config.yml"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            string allText;
                            using (var sr = new StreamReader(fs, Encoding.UTF8, false, 1024, true))
                                allText = sr.ReadToEnd();

                            if (!allText.Contains(@""".well-known"""))
                                using (var sw = new StreamWriter(fs))
                                    sw.Write(@"
# Handling Reading
include:      ["".well-known""]
");
                        }

                    if (this.Model.UpdateCname.Value)
                        using (var fs = File.Open(Path.Combine(this.Model.SiteRoot.Value, "CNAME"), FileMode.Create, FileAccess.ReadWrite))
                        using (var sw = new StreamWriter(fs))
                            sw.Write(this.Model.Domain.Value);
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
                async () =>
                {
                    using (var repo = new Repository(this.Model.SiteRoot.Value))
                    {
                        var username = this.Model.GitUserName.Value;
                        var password = this.Model.GitPassword.Value;
                        var email = this.Model.Email.Value;

                        // Stage the file
                        repo.Index.Add(Path.Combine(this.Model.FileRelativePath.Value, "index.html"));

                        if (this.Model.UpdateCname.Value)
                            repo.Index.Add("CNAME");

                        if (this.Model.UpdateConfigYml.Value)
                            repo.Index.Add("_config.yml");

                        // Create the committer's signature and commit
                        Signature author = new Signature(username, email, DateTime.Now);
                        Signature committer = author;

                        try
                        {
                            // Commit to the repository
                            Commit commit = repo.Commit(
                                "Let's Encrypt HTTP challenge files.",
                                author,
                                committer);
                        }
                        catch (EmptyCommitException)
                        {
                            // ignore empty commit exception
                        }

                        // Push to origin
                        var remote = repo.Network.Remotes["origin"];
                        var options = new PushOptions();
                        var credentials = new UsernamePasswordCredentials { Username = username, Password = password };
                        options.CredentialsProvider = (url, usernameFromUrl, types) => credentials;
                        var pushRefSpec = @"refs/heads/master";
                        repo.Network.Push(remote, pushRefSpec, options);
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
                    var wrGetUrl = WebRequest.Create($"{(this.Model.CurrentChallenge.Value?.Challenge as HttpChallenge)?.FileUrl}/index.html") as HttpWebRequest;

                    if (wrGetUrl == null)
                        throw new Exception("Could not create web request object.");

                    //WebProxy myProxy = new WebProxy("myproxy", 80);
                    //myProxy.BypassProxyOnLocal = true;
                    //wrGETURL.Proxy = myProxy;
                    wrGetUrl.Proxy = WebRequest.GetSystemWebProxy();

                    HttpWebResponse response;
                    try
                    {
                        response = wrGetUrl.GetResponse() as HttpWebResponse;
                    }
                    catch (WebException ex)
                    {
                        response = ex.Response as HttpWebResponse;
                        this.Warn?.Invoke(ex.Message);
                    }

                    if (response?.StatusCode == HttpStatusCode.OK)
                        using (Stream objStream = response.GetResponseStream())
                            if (objStream != null)
                            {
                                var objReader = new StreamReader(objStream);
                                var str = objReader.ReadToEnd();

                                if (str == this.Model.Key.Value)
                                {
                                    this.Model.CanValidateChallenge.Value = true;
                                    return;
                                }
                            }

                    throw new Exception("Waiting for file to be uploaded.");
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
                    var idref = this.Model.CurrentAuthState.Value.Identifier;
                    var state = new SubmitChallenge { IdentifierRef = idref, ChallengeType = "http-01" }.GetValue<AuthorizationState>();
                    this.Model.CurrentAuthState.Value = state;

                    // number of times to retry updating the status
                    this.Model.AutoUpdateStatusRetry.Value = 5;
                },
                this.Model.AutoUpdateStatus,
                this.UpdateStatus);
        }

        public async Task UpdateStatus()
        {
            await AutoCaller(
                this.Model.CanUpdateStatus,
                this.Model.AutoUpdateStatus,
                this.Model.AutoUpdateStatusRetry,
                this.Model.AutoUpdateStatusTimer,
                this.UpdateStatusOnce,
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
                async () =>
                {
                    if (this.Model.CurrentAuthState.Value.Status == "valid")
                    {
                        var certificateInfo =
                            new GetCertificate { CertificateRef = this.Model.Certificate.Value }.GetValue<CertificateInfo>();
                        var idref = this.Model.CurrentAuthState.Value.Identifier;

                        if (certificateInfo == null)
                            new NewCertificate
                            {
                                IdentifierRef = idref,
                                Alias = this.Model.Certificate.Value,
                                Generate = SwitchParameter.Present
                            }
                            .GetValue<CertificateInfo>();
                    }
                    else
                    {
                        // cannot retry after this error: an invalid status can never be undone
                        this.Model.AutoCreateCertificateRetry.Value = 0;
                        throw new Exception($"Status is '{this.Model.CurrentAuthState.Value.Status}', can't continue as it is not 'valid'.");
                    }
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
                async () =>
                {
                    if (this.Model.CurrentAuthState.Value.Status == "valid")
                    {
                        var certificateInfo = new GetCertificate { CertificateRef = this.Model.Certificate.Value }.GetValue<CertificateInfo>();
                        // NOTE: If you have existing keys you can use them as well, this is good to do if you want to use HPKP
                        // new NewCertificate { IdentifierRef = idref, Alias = "cert1", KeyPemFile = "path\\to\\key.pem", CsrPemFile = "path\\to\\csr.pem" }.Run();
                        //certificateInfo = new SubmitCertificate { PkiTool = BouncyCastleProvider.PROVIDER_NAME, CertificateRef = "cert1" }.GetValue<CertificateInfo>();
                        if (certificateInfo.CertificateRequest == null)
                            certificateInfo =
                                new SubmitCertificate
                                {
                                    CertificateRef = this.Model.Certificate.Value,
                                    Force = SwitchParameter.Present
                                }
                                .GetValue<CertificateInfo>();
                    }
                },
                this.Model.AutoGetIssuerCertificate,
                this.GetIssuerCertificate);
        }

        public async Task GetIssuerCertificate()
        {
            await AutoCaller(
                this.Model.CanGetIssuerCertificate,
                this.Model.AutoGetIssuerCertificate,
                this.Model.AutoGetIssuerCertificateRetry,
                this.Model.AutoGetIssuerCertificateTimer,
                async () =>
                {
                    if (this.Model.CurrentAuthState.Value.Status == "valid")
                    {
                        var certificateInfo =
                            new UpdateCertificate { CertificateRef = this.Model.Certificate.Value }
                                .GetValue<CertificateInfo>();

                        if (string.IsNullOrEmpty(certificateInfo.IssuerSerialNumber))
                            throw new Exception("IssuerSerialNumber was not set.");

                        this.Success?.Invoke(
                            $"The certificate information was generated by LetsEncrypt and stored.");
                    }
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

        /// <summary>
        /// Updates the authorization status to know whether the submission succeded or not.
        /// Though this is intended for after submission, it can be called before without side effects.
        /// </summary>
        public void UpdateStatusOnce()
        {
            if (this.Model.CurrentAuthState.Value == null)
                return;

            var idref = this.Model.CurrentAuthState.Value.Identifier;
            var newState = new UpdateIdentifier { IdentifierRef = idref }.GetValue<AuthorizationState>();
            this.Model.CurrentAuthState.Value = newState;
        }

        public void DeleteCurrentRegistration()
        {
            this.acme.DeleteRegistration(this.Model.CurrentRegistration.Value);
            this.Model.Registrations.Value = this.acme.GetRegistrations();
        }

        public void DeleteCurrentDomain()
        {
            this.acme.DeleteDomain(this.Model.CurrentRegistration.Value, this.Model.Domain.Value);
            this.Model.Domains.Value = this.acme.GetDomainsByRegistration(this.Model.CurrentRegistration.Value);
        }
    }
}