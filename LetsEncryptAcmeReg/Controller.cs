using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.POSH;
using ACMESharp.Vault.Model;
using JetBrains.Annotations;
using LetsEncryptAcmeReg.SSG;
using LibGit2Sharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Signature = LibGit2Sharp.Signature;
#pragma warning disable 1998

namespace LetsEncryptAcmeReg
{
    public class Controller :
        ISsgController
    {
        private readonly Acme acme;
        private static readonly Lazy<Type[]> _ssgTypes = new Lazy<Type[]>(GetSsgTypes, LazyThreadSafetyMode.ExecutionAndPublication);

        public Controller(Acme acme, IUIServices uiServices)
        {
            this.acme = acme;
            this.UIServices = uiServices;
            this.Model = new WizardBindableModel();
            this.ManagerModel = new ManagerBindableModel();
            this.CertViewModel = new CertViewBindableModel();
        }

        public WizardBindableModel Model { get; }
        public ManagerBindableModel ManagerModel { get; }
        public CertViewBindableModel CertViewModel { get; }

        public Action<Exception> Error { get; set; }
        public Action<string> Warn { get; set; }
        public Action<string> Success { get; set; }
        public Action<string> Log { get; set; }

        public IUIServices UIServices { get; }

        public BindResult Initialize()
        {
            var mo = this.Model;
            var ma = this.ManagerModel;
            var mc = this.CertViewModel;
            BindResult init = BindResult.Null;

            // Primary initialization:
            init += new BindResult(() => mo.Registrations.Value = this.acme.GetRegistrations());
            init += new BindResult(() => mo.Now.Value = DateTime.Now);
            init += mo.Date.BindExpression(() => mo.Now.Value.Date);
            init += mo.TOSLink.BindExpression(() => this.acme.GetTos(mo.Registrations.Value, mo.Email.Value));
            mo.Domain.Changed += strings => mo.Certificate.Value = "";

            // Collections
            init += mo.Domains.BindExpression(() => this.acme.GetDomainsByEmail(mo.Registrations.Value, mo.Email.Value).OrderBy(x => x).ToArray());
            init += mo.Certificates.BindExpression(() => this.Certifcates_value(mo.CurrentRegistration.Value, mo.CurrentIdentifier.Value));

            // Complex relations:
            //      These relations are built by using expressions.
            //      Every bindable reference in the right hand side
            //      have a changed event added automatically.
            init += mo.IsEmailValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Email.Value));
            init += mo.IsRegistrationCreated.BindExpression(() => mo.CurrentRegistration.Value != null);
            init += mo.IsDomainValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Domain.Value) && mo.IsEmailValid.Value);
            init += mo.IsDomainCreated.BindExpression(() => mo.Domains.Value._S(v => v.Any(i => i == mo.Domain.Value)) ?? false);
            init += mo.IsChallengeValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Challenge.Value) && mo.IsDomainValid.Value);
            init += mo.IsChallengeCreated.BindExpression(() => mo.CurrentAuthState.Value._(v => v.Challenges._S(c => c.Any(x => x.Challenge != null))) ?? false);
            init += mo.IsTargetValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Target.Value) && mo.IsChallengeCreated.Value);
            init += mo.IsKeyValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.Key.Value) && mo.IsChallengeCreated.Value);
            init += mo.IsSiteRootValid.BindExpression(() => !string.IsNullOrWhiteSpace(mo.SiteRoot.Value) && mo.IsChallengeCreated.Value);
            init += mo.ChallengeHasFile.BindExpression(() => mo.IsTargetValid.Value && mo.IsKeyValid.Value
                    && (mo.Challenge.Value != "http-01" || !string.IsNullOrWhiteSpace(mo.SiteRoot.Value)));

            init += mo.CurrentIdentifier.BindExpression(() => this.CurrentIdentifier_Value(mo.CurrentRegistration.Value, mo.Domain.Value));
            init += mo.CurrentAuthState.BindExpression(() => this.CurrentAuthState_Value(mo.CurrentIdentifier.Value));
            init += mo.CurrentChallenge.BindExpression(() => this.CurrentChallenge_Value(mo.CurrentAuthState.Value, mo.Challenge.Value));
            init += mo.CurrentCertificate.BindExpression(() => this.CurrentCertificate_Value(mo.CurrentRegistration.Value, mo.Domain.Value, mo.Certificate.Value));
            init += mo.X509Certificate.BindExpression(() => this.X509Certificate_Value(mo.CurrentCertificate.Value));

            init += mo.Target.BindExpression(() => mo.CurrentChallenge.Value._(v => (v.Challenge as HttpChallenge)._(c => c.FileUrl)) ?? "");
            init += mo.Key.BindExpression(() => mo.CurrentChallenge.Value._(v => v.Challenge._(c => (c.Answer as HttpChallengeAnswer)._(a => a.KeyAuthorization))) ?? "");
            init += mo.FileRelativePath.BindExpression(() => mo.CurrentChallenge.Value._(v => (v.Challenge as HttpChallenge)._(c => c.FilePath)._(s => s.Replace('/', '\\'))) ?? "");

            init += mo.FilePath.BindExpression(() => this.FilePath_Value(mo.ChallengeHasFile.Value, mo.SiteRoot.Value, mo.FileRelativePath.Value));

            init += mo.Issuer.BindExpression(() => mo.CurrentCertificate.Value.With(v => v != null ? v.IssuerSerialNumber : ""));

            init += mo.CanRegister.BindExpression(() => this.CanRegister_Value(mo.Registrations.Value, mo.Email.Value, mo.IsEmailValid.Value));
            init += mo.CanAcceptTos.BindExpression(() => this.CanAcceptTos_Value(mo.CurrentRegistration.Value));
            init += mo.CanAddDomain.BindExpression(() => mo.CurrentRegistration.Value != null && mo.IsDomainValid.Value && !mo.IsDomainCreated.Value);
            init += mo.CanCreateChallenge.BindExpression(() => mo.CurrentIdentifier.Value != null && mo.IsChallengeValid.Value);
            init += mo.CanSaveChallenge.BindExpression(() => mo.ChallengeHasFile.Value);
            init += mo.CanCommitChallenge.BindExpression(() => this.CanCommitChallenge_Value(mo.SiteRoot.Value));
            init += mo.CanTestChallenge.BindExpression(() => mo.IsTargetValid.Value && mo.IsKeyValid.Value);
            init += mo.CanUpdateStatus.BindExpression(() => mo.CurrentAuthState.Value != null);
            init += mo.CanValidateChallenge.BindExpression(() => false); // this value is changed after testing the challenge (it's not bound to anything)
            init += mo.CanCreateCertificate.BindExpression(() => mo.CurrentAuthState.Value.With(v => v != null && v.Status == "valid"));
            init += mo.CanSubmitCertificate.BindExpression(() => mo.CurrentCertificate.Value.With(v => v != null && v.CertificateRequest == null && string.IsNullOrWhiteSpace(v.IssuerSerialNumber)));
            init += mo.CanGetIssuerCertificate.BindExpression(() => mo.CurrentCertificate.Value.With(v => v != null && v.CertificateRequest != null && string.IsNullOrWhiteSpace(v.IssuerSerialNumber)));
            init += mo.CanSaveCertificate.BindExpression(() => mo.CurrentCertificate.Value.With(v => v != null && !string.IsNullOrWhiteSpace(v.IssuerSerialNumber)) && mo.CertificateType.Value != 0);

            var viewableCertTypes = new[] { CertType.CertificatePEM, CertType.CsrPEM, CertType.IssuerPEM, CertType.KeyPEM };
            init += mo.CanShowCertificate.BindExpression(() => mo.CurrentCertificate.Value.With(v => v != null && !string.IsNullOrWhiteSpace(v.IssuerSerialNumber)) && Array.IndexOf(viewableCertTypes, mo.CertificateType.Value) >= 0);

            init += mo.IsPasswordEnabled.BindExpression(() => mo.CertificateType.Value == CertType.Pkcs12);

            init += mo.SsgTypes.BindExpression(() => _ssgTypes.Value.Select(SsgNameFromType).ToArray());
            init += mo.CurrentSsg.BindExpression(() => this.CurrentSsg_Value(mo.SsgName.Value));

            init += mo.ExpandedSavePath.BindExpression(() => this.ExpandedSavePath_Value(mo.SavePath.Value, mo.CertificateType.Value, mo.Certificate.Value));

            init += mo.AvailableDomains.BindExpression(() => mo.CurrentRegistration.Value == null ? new string[0] : this.acme.GetIdentifiers(mo.CurrentRegistration.Value, null).Where(c => c.Authorization.Status == "valid").Select(c => c.Dns).ToArray());

            // Certificate Viewer

            init += mc.CurrentCertificate.BindExpression(() => this.CurrentCertificate_Value(null, null, mc.Certificate.Value));
            init += mc.Certificates.BindExpression(() => this.acme.GetCertificates(null, null).Where(c => c.IssuerSerialNumber != null).Select(c => c.Alias).ToArray());
            init += mc.TextAssets.BindExpression(() => this.acme.GetTextAssets(mc.Certificate.Value, false));
            init += mc.Base64Data.BindExpression(() => mc.TextAssets.Value == null ? null : mc.TextAssets.Value.GetAsset(mc.CertificateType.Value));

            // when the key changes, the domain must be tested again
            mo.Key.Changed += s => mo.CanValidateChallenge.Value = false;

            mo.CurrentSsg.Changing += this.CurrentSsg_Changing;
            mo.CurrentSsg.Changed += this.CurrentSsg_Changed;

            return init;
        }

        private string[] Certifcates_value(RegistrationInfo registrationInfo, IdentifierInfo domain)
        {
            if (domain == null)
                return new string[0];

            return this.acme.GetCertificates(registrationInfo, domain?.Alias).Select(c => c.Alias).ToArray();
        }

        private X509Certificate2 X509Certificate_Value(CertificateInfo certInfo)
        {
            if (certInfo == null)
                return null;

            var textAssets = this.acme.GetTextAssets(certInfo.Alias, noThrow: true);
            var pem = textAssets.GetAsset(CertType.CertificatePEM);

            if (pem == null)
                return null;

            byte[] certBuffer = GetBytesFromPEM(pem, "CERTIFICATE");
            var certificate = new X509Certificate2(certBuffer);

            return certificate;
        }

        byte[] GetBytesFromPEM(string pemString, string section)
        {
            var header = String.Format("-----BEGIN {0}-----", section);
            var footer = String.Format("-----END {0}-----", section);

            var start = pemString.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += header.Length;
            var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

            if (end < 0)
                return null;

            return Convert.FromBase64String(pemString.Substring(start, end));
        }

        private bool CanRegister_Value(RegistrationInfo[] regs, string email, bool isEmailValid)
        {
            if (!isEmailValid)
                return false;
            if (regs.Any(x => x.Registration.Contacts.Any(c => c == $"mailto:{email}")))
                return false;
            return true;
        }

        private void CurrentSsg_Changing(Bindable<ISsg> sender, ISsg value, ISsg prev, ref bool cancel)
        {
            prev?.Dispose();
            this.UIServices.ClearPanelForSsg();
        }

        private void CurrentSsg_Changed(ISsg ssg)
        {
            if (ssg == null)
                return;

            var panel = this.UIServices.CreatePanelForSsg();
            var ok = ssg.Initialize(this, this.Model, panel);

            if (ok)
            {

            }
        }

        [NotNull]
        private static string SsgNameFromType([NotNull] Type t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));

            if (t == typeof(BaseSsg))
                return "[None/Manual]";

            return t.Name.EndsWith("SSG", StringComparison.InvariantCultureIgnoreCase)
                ? t.Name.Substring(0, t.Name.Length - 3)
                : t.Name;
        }

        [CanBeNull]
        private ISsg CurrentSsg_Value([CanBeNull] string value)
        {
            var ssgType = _ssgTypes.Value.FirstOrDefault(t => SsgNameFromType(t) == value);

            if (ssgType == null)
                return null;

            var ssg = (ISsg)Activator.CreateInstance(ssgType);
            return ssg;
        }

        private static Type[] GetSsgTypes()
        {
            var type = typeof(ISsg);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p != typeof(ISsg) && p != typeof(BaseSsg))
                .Prepend(typeof(BaseSsg))
                .ToArray();
            return types;
        }

        private string ExpandedSavePath_Value(string savePath, CertType certType, string cert)
        {
            if (string.IsNullOrWhiteSpace(savePath))
                savePath = Environment.CurrentDirectory;

            if (certType == 0)
                return null;

            if (string.IsNullOrWhiteSpace(cert))
                return null;

            var ext = GetExt(certType);

            var pathExpanded = Environment.ExpandEnvironmentVariables(savePath);
            string path;
            string fname;
            if (!Directory.Exists(pathExpanded))
            {
                path = Path.GetDirectoryName(pathExpanded) + "\\";
                fname = Path.GetFileName(pathExpanded);
                if (string.IsNullOrWhiteSpace(fname))
                    fname = cert + "." + ext;
                else if (Path.GetExtension(fname) != "." + ext)
                    fname += "." + ext;
            }
            else if (!pathExpanded.EndsWith("\\"))
            {
                path = pathExpanded + "\\";
                fname = cert + "." + ext;
            }
            else
            {
                path = pathExpanded;
                fname = cert + "." + ext;
            }

            var fullName = Path.Combine(path, fname);

            return fullName;
        }

        internal static string GetExt(CertType certType)
        {
            string ext;
            switch (certType)
            {
                case CertType.KeyPEM:
                    ext = "pem";
                    break;
                case CertType.CsrPEM:
                    ext = "pem";
                    break;
                case CertType.CertificatePEM:
                    ext = "pem";
                    break;
                case CertType.CertificateDER:
                    ext = "der";
                    break;
                case CertType.IssuerPEM:
                    ext = "pem";
                    break;
                case CertType.IssuerDER:
                    ext = "der";
                    break;
                case CertType.Pkcs12:
                    ext = "pfx";
                    break;
                default:
                    ext = "";
                    break;
            }
            return ext;
        }

        [CanBeNull]
        private CertificateInfo CurrentCertificate_Value([CanBeNull] RegistrationInfo regInfo, [CanBeNull] string domain, string certRef)
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

        private bool CanAcceptTos_Value(RegistrationInfo regInfo)
            => CatchError(() => regInfo?.Registration != null && regInfo.Registration.TosAgreementUri == null);

        private IdentifierInfo CurrentIdentifier_Value([CanBeNull] RegistrationInfo regInfo, [CanBeNull] string dns)
            => CatchError(() => regInfo == null || string.IsNullOrWhiteSpace(dns) ? null : this.acme.GetIdentifiers(regInfo, dns).SingleOrDefault());

        private AuthorizationState CurrentAuthState_Value([CanBeNull] IdentifierInfo idInfo)
            => CatchError(() => idInfo?.Authorization);

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

        public async Task OpenTosInBrowser()
        {
            await Task.Delay(1);
            Process.Start(this.Model.TOSLink.Value);
        }

        public async Task OpenPemConcatHelpInBrowser()
        {
            await Task.Delay(1);
            Process.Start("https://www.digicert.com/ssl-support/pem-ssl-creation.htm");
        }

        /// <summary>
        /// Takes care of the automatic execution of the action, retries, timer between retries, and calling the next actions.
        /// </summary>
        /// <param name="enabled">
        /// Indicates whether the action can be executed or not.
        /// Note that this will not prevent the next action from being executed.
        /// </param>
        /// <param name="isAuto">
        /// Indicates whether the action should retry automatically.
        /// </param>
        /// <param name="retry">
        /// Number of retries for this action.
        /// If null, the default value will be used.
        /// </param>
        /// <param name="timer">
        /// Timer used between retries to indicate the amount of time remaining until next retry.
        /// </param>
        /// <param name="action">
        /// The delegate of the action to be executed.
        /// </param>
        /// <param name="isNextAuto">
        /// Indicates whether the next action should be executed automatically when the current action succeeds.
        /// If the current action is disabled the execution does not happen, but it is considered as a success,
        /// and thus the next action is called.
        /// </param>
        /// <param name="next">
        /// The next action to execute when the current action succeeds.
        /// </param>
        /// <returns></returns>
        private async Task AutoCaller(Bindable<bool> enabled, Bindable<bool> isAuto, Bindable<int?> retry, Bindable<int?> timer, Func<Task> action, [CanBeNull] Bindable<bool> isNextAuto, [CanBeNull] Func<Task> next)
        {
            // If the number of retries is already set use it.
            // Otherwsie use the default value.
            if (retry.Value == null)
                retry.Value = 3;

            // If the timer is already set, then there is another task in place.
            // Aborting current task, and leaving a message for the other task to restart as soon as possible.
            if (timer.Value != null)
            {
                timer.Value = 0;
                return;
            }

            bool ok = false;
            while (enabled.Value && retry.Value != null)
            {
                try
                {
                    timer.Value = 0; // this indicates that the action is running
                    await action();
                    timer.Value = null; // stop timer, indicating that this is not running
                    ok = true;
                    break;
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

            timer.Value = null; // stop timer
            retry.Value = null; // indicates that there is no more retries

            if (ok && isNextAuto?.Value == true && next != null)
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
                async () =>
                {
                    var newRegistration = this.acme.Register(this.Model.Registrations.Value, this.Model.Email.Value);
                    if (newRegistration != null)
                    {
                        var newList = this.Model.Registrations.Value.Concat(new[] { newRegistration }).ToArray();
                        this.Model.Registrations.Value = newList;
                        this.Model.CurrentRegistration.Value = newRegistration;
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
                async () =>
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
                async () =>
                {
                    // the btnAddDomain should be disabled when something already exists,
                    // so the following function is always used to create a new identity
                    var states = this.acme.GetOrCreateIdentifier(
                        this.Model.CurrentRegistration.Value,
                        this.Model.Domain.Value
                        );

                    if (states.Length == 0)
                        throw new Exception("Identity could not be created.");

                    if (states.Length > 1)
                        this.Warn(Messages.MultipleIdentities);

                    this.Model.Domains.Value = this.Model.Domains.Value?.Append(states[0].Dns).Sort().Distinct().ToArray();
                    this.Model.Domain.Value = states[0].Dns;
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
                async () =>
                {
                    if (this.Model.CurrentIdentifier.Value == null)
                        return;

                    var idref = this.acme.GetIdentifierAlias(
                        this.Model.CurrentRegistration.Value,
                        this.Model.Domain.Value);

                    var state =
                        new CompleteChallenge
                        {
                            Handler = "Manual",
                            IdentifierRef = idref,
                            ChallengeType = this.Model.Challenge.Value,
                            Force = SwitchParameter.Present,
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
                async () =>
                {
                    var ssg = this.Model.CurrentSsg.Value;
                    if (ssg != null)
                    {
                        ssg.Patch();
                    }
                    else
                    {

                    }
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

                        // Stage the files
                        var filesToAdd = this.Model.Files.Value
                            .Select(p => PathUtils.CreateRelativePath(p, this.Model.SiteRoot.Value))
                            .ToArray();

                        foreach (var eachPath in filesToAdd)
                            repo.Index.Add(eachPath.TrimStart('\\', '/'));

                        // Create the committer's signature and commit
                        Signature author = new Signature(username, email, DateTime.Now);
                        Signature committer = author;

                        bool ok = false;
                        try
                        {
                            // Commit to the repository
                            Commit commit = repo.Commit(
                                Messages.CommitMessage,
                                author,
                                committer);
                            ok = true;
                        }
                        catch (EmptyCommitException)
                        {
                            // ignore empty commit exception
                        }

                        if (ok)
                        {
                            // Push to origin
                            var remote = repo.Network.Remotes["origin"];
                            var options = new PushOptions();
                            var credentials = new UsernamePasswordCredentials { Username = username, Password = password };
                            options.CredentialsProvider = (url, usernameFromUrl, types) => credentials;
                            var pushRefSpec = @"refs/heads/master";
                            repo.Network.Push(remote, pushRefSpec, options);
                        }
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
                async () =>
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
                                    this.Success?.Invoke("Test: Ok - Challenge file is publicly visible.");
                                    return;
                                }
                                this.Model.CanValidateChallenge.Value = false;
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
                async () =>
                {
                    var idref = this.acme.GetIdentifierAlias(this.Model.CurrentRegistration.Value, this.Model.CurrentAuthState.Value.Identifier);
                    var state = new SubmitChallenge
                    {
                        IdentifierRef = idref,
                        ChallengeType = "http-01",
                        Force = SwitchParameter.Present
                    }.GetValue<AuthorizationState>();
                    this.Model.CurrentAuthState.Value = state;
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
                    if (this.Model.CurrentAuthState.Value.Status != "valid")
                    {
                        // cannot retry after this error: an invalid status can never be undone
                        this.Model.AutoCreateCertificateRetry.Value = 0;
                        throw new Exception($"Status is '{this.Model.CurrentAuthState.Value.Status}', can't continue as it is not 'valid'.");
                    }

                    if (string.IsNullOrWhiteSpace(this.Model.Certificate.Value))
                        throw new Exception($"Certificate name is empty.");

                    var certificateInfo = this.acme.GetCertificate(this.Model.Certificate.Value);
                    if (certificateInfo?.IdentifierDns == this.Model.Domain.Value)
                        throw new Exception($"Certificate name is already used by another domain.");

                    var idref = this.acme.GetIdentifierAlias(this.Model.CurrentRegistration.Value, this.Model.CurrentAuthState.Value.Identifier);

                    if (certificateInfo == null)
                    {
                        var altAliases = this.Model.CertificateDomains.Value
                            ?.Select(d => this.acme.GetIdentifierAlias(this.Model.CurrentRegistration.Value, d))
                            ?.ToArray();
                        new NewCertificate
                        {
                            IdentifierRef = idref,
                            Alias = this.Model.Certificate.Value,
                            Generate = SwitchParameter.Present,
                            AlternativeIdentifierRefs = altAliases,
                        }
                        .GetValue<CertificateInfo>();
                    }

                    this.Model.Certificates.Value =
                        this.acme.GetCertificates(this.Model.CurrentRegistration.Value, this.Model.Domain.Value)
                            .Select(c => c.Alias)
                            .ToArray();

                    // updates the certificate value
                    // force is used because the bindable object is flagged not to fire events if value does not chage.
                    this.Model.Certificate.Update(force: true);
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
                            this.Model.CurrentCertificate.Value =
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
                    if (this.Model.CurrentAuthState.Value.Status == "valid" && this.Model.CurrentCertificate.Value.IssuerSerialNumber == null)
                    {
                        var certificateInfo =
                            new UpdateCertificate { CertificateRef = this.Model.Certificate.Value }
                                .GetValue<CertificateInfo>();

                        this.Model.CurrentCertificate.Value = certificateInfo;

                        if (string.IsNullOrEmpty(certificateInfo.IssuerSerialNumber))
                            throw new Exception(Messages.IssuerSerialNumberNotSet);

                        // # SUCCESS ! ! !

                        // the certificate in now complete
                        // updating the list of certificates of the Viewer
                        var newList = this.CertViewModel.Certificates.Value.Concat(new[] { certificateInfo.Alias }).OrderBy(x => x).ToArray();
                        this.CertViewModel.Certificates.Value = newList;

                        // success message
                        this.Success?.Invoke(Messages.SuccessGetIssuerCert);
                    }
                },
                this.Model.AutoSaveOrShowCertificate,
                this.SaveCertificate);
        }

        public async Task SaveCertificate()
        {
            await AutoCaller(
                this.Model.CanSaveCertificate,
                this.Model.AutoSaveOrShowCertificate,
                this.Model.AutoSaveOrShowCertificateRetry,
                this.Model.AutoSaveOrShowCertificateTimer,
                async () =>
                {
                    var cmd = new GetCertificate
                    {
                        CertificateRef = this.Model.Certificate.Value,
                        Overwrite = SwitchParameter.Present
                    };

                    var path = this.Model.ExpandedSavePath.Value;
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                        this.CatchError(() => Directory.CreateDirectory(dir));

                    switch (this.Model.CertificateType.Value)
                    {
                        case CertType.KeyPEM:
                            cmd.ExportKeyPEM = path;
                            break;
                        case CertType.CsrPEM:
                            cmd.ExportCsrPEM = path;
                            break;
                        case CertType.CertificatePEM:
                            cmd.ExportCertificatePEM = path;
                            break;
                        case CertType.CertificateDER:
                            cmd.ExportCertificateDER = path;
                            break;
                        case CertType.IssuerPEM:
                            cmd.ExportIssuerPEM = path;
                            break;
                        case CertType.IssuerDER:
                            cmd.ExportIssuerDER = path;
                            break;
                        case CertType.Pkcs12:
                            cmd.ExportPkcs12 = path;
                            cmd.CertificatePassword = this.Model.Password.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(this.Model.CertificateType));
                    }

                    cmd.Run();

                    this.Success?.Invoke("Certificate file saved.");
                }, null, null);
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
        public async Task UpdateStatusOnce()
        {
            if (this.Model.CurrentAuthState.Value == null)
                return;

            var idref = this.acme.GetIdentifierAlias(this.Model.CurrentRegistration.Value, this.Model.CurrentAuthState.Value.Identifier);
            var newState = new UpdateIdentifier { IdentifierRef = idref }.GetValue<AuthorizationState>();

            this.Model.CurrentAuthState.SetOnce(newState);
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

        public void ViewCertificate(string certRef, CertType certType)
        {
            this.CertViewModel.Certificate.Value = certRef;
            this.CertViewModel.CertificateType.Value = certType;
        }
    }
}