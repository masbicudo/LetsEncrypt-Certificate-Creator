using ACMESharp;
using ACMESharp.ACME;
using ACMESharp.POSH;
using ACMESharp.Vault.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACMESharp.Vault;

namespace LetsEncryptAcmeReg
{
    public partial class frm : Form
    {
        // Based on:
        //  - https://github.com/ebekker/ACMESharp
        //  - https://gist.github.com/nul800sebastiaan/31b000874ffa69f4c0af
        // Read more at:
        //  - https://cultiv.nl/blog/lets-encrypt-on-windows-revisited/
        // Related issues:
        //  - https://github.com/ebekker/ACMESharp/issues/205
        private readonly Registrator reg = new Registrator();

        public frm()
        {
            InitializeComponent();
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            var data = this.reg.Register(new RegistrationOptions
            {
                Domain = txtDomain.Text,
                Email = txtEmail.Text,
            });

            var canProceed = data != null;
            if (canProceed)
            {
                // udating key textbox
                this.txtKey.Text = data.Key;

                // udating root textbox
                var path = data.Path.Replace('/', '\\');
                EventHandler txtSiteRootOnTextChanged =
                    (o, args) => labFullPath.Text = Path.Combine(txtSiteRoot.Text, path);
                var prevEvent = txtSiteRoot.Tag as EventHandler;
                if (prevEvent != null)
                    txtSiteRoot.TextChanged -= prevEvent;
                txtSiteRoot.Tag = txtSiteRootOnTextChanged;
                txtSiteRoot.TextChanged += txtSiteRootOnTextChanged;
                txtSiteRootOnTextChanged(null, null);

                // udating challenge textbox
                txtChallenge.Text = data.Url;
            }
            else
            {
                this.listBox1.Items.Add("Already registered");
            }

            foreach (Control ctl in this.panel2.Controls)
            {
                ctl.Enabled = canProceed;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(this.labFullPath.Text);

                using (
                    var fs = File.Open(Path.Combine(this.labFullPath.Text, "index.html"), FileMode.Create,
                        FileAccess.ReadWrite))
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(this.txtKey.Text);
                }
            }
            catch
            {
            }
        }

        private async void btnComplete_Click(object sender, EventArgs e)
        {
            string sURL = this.txtChallenge.Text;
            bool onceMessage = false;
            if (!string.IsNullOrEmpty(sURL))
                while (true)
                {
                    var wrGETURL = WebRequest.Create(sURL + "/index.html") as HttpWebRequest;
                    //WebProxy myProxy = new WebProxy("myproxy", 80);
                    //myProxy.BypassProxyOnLocal = true;
                    //wrGETURL.Proxy = myProxy;
                    wrGETURL.Proxy = WebRequest.GetSystemWebProxy();

                    HttpWebResponse response;
                    try
                    {
                        response = wrGETURL.GetResponse() as HttpWebResponse;
                    }
                    catch (WebException ex)
                    {
                        response = ex.Response as HttpWebResponse;
                    }

                    if (response.StatusCode == HttpStatusCode.OK)
                        using (Stream objStream = response.GetResponseStream())
                        {
                            var objReader = new StreamReader(objStream);
                            var str = objReader.ReadToEnd();

                            if (str == this.txtKey.Text)
                                break;
                        }

                    await Task.Delay(1000);
                    if (!onceMessage)
                        this.listBox1.Items.Add("Waiting for file to be uploaded.");
                    onceMessage = true;
                }

            var idref = Registrator.GetIdentifier(this.txtDomain.Text);
            var state = new UpdateIdentifier {IdentifierRef = idref}.GetValue<AuthorizationState>();
            if (state.Status != "valid")
            {
                state =
                    new SubmitChallenge {IdentifierRef = idref, ChallengeType = "http-01"}.GetValue<AuthorizationState>();
                int countPending = 0;
                while (state.Status == "pending")
                {
                    this.listBox1.Items.Add("Status is still 'pending', waiting for it to change...");
                    await Task.Delay((countPending + 1)*1000);
                    state = new UpdateIdentifier {IdentifierRef = idref}.GetValue<AuthorizationState>();
                    countPending++;
                }
            }

            if (state.Status == "valid")
            {
                var certificateInfo = new GetCertificate {CertificateRef = "cert1"}.GetValue<CertificateInfo>();

                if (certificateInfo == null)
                    new NewCertificate {IdentifierRef = idref, Alias = "cert1", Generate = SwitchParameter.Present}
                        .GetValue<CertificateInfo>();
                // NOTE: If you have existing keys you can use them as well, this is good to do if you want to use HPKP
                // new NewCertificate { IdentifierRef = idref, Alias = "cert1", KeyPemFile = "path\\to\\key.pem", CsrPemFile = "path\\to\\csr.pem" }.Run();
                //certificateInfo = new SubmitCertificate { PkiTool = BouncyCastleProvider.PROVIDER_NAME, CertificateRef = "cert1" }.GetValue<CertificateInfo>();
                certificateInfo =
                    new SubmitCertificate {CertificateRef = "cert1", Force = SwitchParameter.Present}
                        .GetValue<CertificateInfo>();
                while (string.IsNullOrEmpty(certificateInfo.IssuerSerialNumber))
                {
                    await Task.Delay(1000);
                    this.listBox1.Items.Add("IssuerSerialNumber is not set yet, waiting for it to be populated...");
                    certificateInfo = new UpdateCertificate {CertificateRef = "cert1"}.GetValue<CertificateInfo>();
                }

                this.listBox1.Items.Add(
                    $"The certificate information was generated by LetsEncrypt and stored.");
            }
            else
            {
                this.listBox1.Items.Add(
                    $"Status is '{state.Status}', can't continue as it is not 'valid'.");
            }
        }

        private void btnSaveCert_Click(object sender, EventArgs e)
        {
            new GetCertificate
            {
                CertificateRef = "cert1",
                ExportPkcs12 = this.txtCertFile.Text,
                CertificatePassword = this.txtPassword.Text,
                Overwrite = SwitchParameter.Present
            }.Run();

            this.listBox1.Items.Add("Certificate saved.");
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            var save = new SaveFileDialog();
            save.CheckPathExists = true;
            save.InitialDirectory = Path.GetDirectoryName(txtCertFile.Text);
            save.FileName = Path.GetFileName(txtCertFile.Text);
            save.CreatePrompt = false;
            save.DefaultExt = "pfx";
            save.Filter = "Certificate | *.pfx";
            if (save.ShowDialog(this) == DialogResult.OK)
            {
                txtCertFile.Text = save.FileName;
            }
        }

        private void btnCertText_Click(object sender, EventArgs e)
        {
            //using (var vlt = ACMESharp.POSH.Util.VaultHelper.GetVault(null))
            //{
            //    vlt.OpenStorage();
            //    var v = vlt.LoadVault();

            //    if (v.Registrations == null || v.Registrations.Count < 1)
            //        throw new InvalidOperationException("No registrations found");

            //    var ri = v.Registrations[0];
            //    var r = ri.Registration;

            //    var ci = v.Certificates.GetByRef("cert1", throwOnMissing: false);

            //    var keyPemAsset = vlt.GetAsset(VaultAssetType.KeyPem, ci.KeyPemFile);
            //    var crtPemAsset = vlt.GetAsset(VaultAssetType.CrtPem, ci.CrtPemFile);
            //    var isuPemAsset = vlt.GetAsset(VaultAssetType.IssuerPem,
            //        v.IssuerCertificates[ci.IssuerSerialNumber].CrtPemFile);

            //    var asset = vlt.GetAsset(VaultAssetType.KeyPem, ci.CrtPemFile);
            //    using (Stream s = vlt.LoadAsset(asset),
            //        fs = new FileStream(target, mode))
            //    {
            //        s.CopyTo(fs);
            //    }
            //}
        }

        internal class Registrator
        {
            public VaultInfo Vault()
            {
                VaultInfo vlt = new GetVault().GetValue<VaultInfo>()
                                ??
                                new InitializeVault {BaseUri = "https://acme-v01.api.letsencrypt.org/"}
                                    .GetValue<VaultInfo>();
                return vlt;
            }

            public RegistrationData Register(RegistrationOptions registrationOptions)
            {
                var v = Vault();

                var r = v.Registrations.Values
                    .Single(x => x.Registration.Contacts.Any(c => c == $"mailto:{registrationOptions.Email}"));

                if (r == null)
                {
                    new NewRegistration {Contacts = new[] {$"mailto:{registrationOptions.Email}"}}.Run();
                    new UpdateRegistration {AcceptTos = SwitchParameter.Present}.Run();
                }

                var idref = GetIdentifier(registrationOptions.Domain);

                AuthorizationState state;
                state = IdentifierExists(idref)
                    ? new GetIdentifier {IdentifierRef = idref}.GetValue<AuthorizationState>()
                    : new NewIdentifier {Dns = registrationOptions.Domain, Alias = idref}.GetValue<AuthorizationState>();

                //using (var vlt = ACMESharp.POSH.Util.VaultHelper.GetVault())
                //{
                //    vlt.OpenStorage(true);
                //    vlt.LoadVault();
                //}

                state = new GetIdentifier {IdentifierRef = idref}.GetValue<AuthorizationState>();

                state =
                    new CompleteChallenge
                    {
                        Handler = "Manual",
                        IdentifierRef = idref,
                        ChallengeType = "http-01",
                        Repeat = SwitchParameter.Present,
                        Regenerate = SwitchParameter.Present
                    }
                        .GetValue<AuthorizationState>();

                var challenge =
                    state.Challenges.Where(x => x.Type == "http-01")
                        .Select(x => x.Challenge)
                        .Single() as HttpChallenge;

                var challengeAnswer = challenge?.Answer as HttpChallengeAnswer;
                if (challengeAnswer != null)
                    return new RegistrationData
                    {
                        Url = challenge.FileUrl,
                        Key = challengeAnswer.KeyAuthorization,
                        Path = challenge.FilePath
                    };

                return null;
            }

            public static bool IdentifierExists(string idref)
            {
                IDictionary[] allIds = new GetIdentifier()
                    .GetValues()
                    .Where(x => x != null)
                    .Select(x => (x as object).ToDictionary()).ToArray();

                return allIds
                    .Any(x => (x["Alias"] ?? "").ToString() == idref);
            }

            public static string GetIdentifier(string domain)
            {
                IDictionary[] allIds = new GetIdentifier()
                    .GetValues().Select(x => (x as object).ToDictionary()).ToArray();

                string idref = allIds
                    .Where(x => x != null)
                    .Where(x => (x["Dns"] ?? "").ToString() == domain)
                    .Select(x => (x["Alias"] ?? "").ToString())
                    .SingleOrDefault();

                if (idref == null)
                {
                    idref = "dns" + (allIds.Count(x => x != null) + 1);
                }

                return idref;
            }
        }

        internal class RegistrationOptions
        {
            public string Domain { get; set; }
            public string Email { get; set; }
        }

        internal class RegistrationData
        {
            public string Url { get; set; }
            public string Key { get; set; }
            public string Path { get; set; }
        }

    }
}