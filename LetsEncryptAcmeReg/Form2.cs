using ACMESharp.Vault.Model;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public partial class Form2 : Form
    {
        private readonly Controller controller;
        private readonly Acme acme = new Acme();
        private readonly ToolTipManager tooltip = new ToolTipManager();

        public Form2()
        {
            InitializeComponent();

            this.txtSiteRoot.MouseMove += this.txtSiteRoot_MouseMove;
            this.txtSiteRoot.MouseEnter += this.txtSiteRoot_MouseEnter;
            this.txtSiteRoot.MouseLeave += this.txtSiteRoot_MouseLeave;

            var tt = this.tooltip.ToolTipFor(this.chkConfigYml)
                .AutoPopup("Updates or creates a ` _config.yml ` file,\r\nwith instructions to _**not ignore**_ the\r\npath ` .well-known\\ `.", useMarkdown: true);
            tt.BorderColor = Color.DodgerBlue;

            var tt2 = this.tooltip.ToolTipFor(this.chkCname)
                .AutoPopup($"Updates or creates a ` CNAME ` file,\r\nwith the name of the ***selected domain***.", useMarkdown: true);
            tt2.BorderColor = Color.DodgerBlue;

            this.controller = new Controller(this.acme)
            {
                Error = this.Error,
                Warn = this.Warn,
                Log = this.Log,
                Success = this.Success,
            };

            var mo = this.controller.Model;
            var ma = this.controller.ManagerModel;

            var init = this.controller.Initialize();

            // Control bindings:
            //      These are relations between the controls on the form
            //      and the bindable objects.
            init += mo.Email.Bind(this.cmbRegistration);
            init += mo.Domain.Bind(this.cmbDomain);
            init += mo.Challenge.Bind(this.cmbChallenge);
            init += mo.Target.Bind(this.txtChallengeTarget);
            init += mo.Key.Bind(this.txtChallengeKey);
            init += mo.SiteRoot.Bind(this.txtSiteRoot);
            init += mo.Certificate.Bind(this.cmbCertificate);
            init += mo.Issuer.Bind(this.txtIssuer);
            init += mo.CertificateType.Bind(this.cmbCertificateType);
            init += mo.Password.Bind(this.txtPassword);
            init += mo.ShowPassword.Bind(this.chkShowPassword);

            init += mo.AutoRegister.Bind(this.chkAutoRegister);
            init += mo.AutoAcceptTos.Bind(this.chkAutoAcceptTos);
            init += mo.AutoAddDomain.Bind(this.chkAutoAddDomain);
            init += mo.AutoCreateChallenge.Bind(this.chkAutoCreateChallenge);
            init += mo.AutoSaveChallenge.Bind(this.chkAutoSaveChallenge);
            init += mo.AutoCommitChallenge.Bind(this.chkAutoCommit);
            init += mo.AutoTestChallenge.Bind(this.chkAutoTest);
            init += mo.AutoValidateChallenge.Bind(this.chkAutoValidate);
            init += mo.AutoCreateCertificate.Bind(this.chkAutoCreateCertificate);
            init += mo.AutoSubmitCertificate.Bind(this.chkAutoSubmit);
            init += mo.AutoSaveCertificate.Bind(this.chkAutoSaveCertificate);

            init += mo.CurrentRegistration.Bind(this.lstRegistrations, (RegistrationItem i) => i?.RegistrationInfo);
            init += mo.CurrentRegistration.Bind(this.cmbRegistration, (RegistrationItem i) => i?.RegistrationInfo);

            init += ma.Challenge.Bind(this.lstChallenges);

            init += mo.UpdateConfigYml.Bind(this.chkConfigYml);
            init += mo.UpdateCname.Bind(this.chkCname);

            // 
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnRegister, mo.AutoRegisterRetry.Value, mo.AutoRegisterTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnAcceptTos, mo.AutoAcceptTosRetry.Value, mo.AutoAcceptTosTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnAddDomain, mo.AutoAddDomainRetry.Value, mo.AutoAddDomainTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnCreateChallenge, mo.AutoCreateChallengeRetry.Value, mo.AutoCreateChallengeTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnSaveChallenge, mo.AutoSaveChallengeRetry.Value, mo.AutoSaveChallengeTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnCommitChallenge, mo.AutoCommitChallengeRetry.Value, mo.AutoCommitChallengeTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnTestChallenge, mo.AutoTestChallengeRetry.Value, mo.AutoTestChallengeTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnValidate, mo.AutoValidateChallengeRetry.Value, mo.AutoValidateChallengeTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnCreateCertificate, mo.AutoCreateCertificateRetry.Value, mo.AutoCreateCertificateTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnSubmit, mo.AutoSubmitCertificateRetry.Value, mo.AutoSubmitCertificateTimer.Value));
            init += BindHelper.BindExpression(() => RetryToolTip(this.btnSaveCertificate, mo.AutoSaveCertificateRetry.Value, mo.AutoSaveCertificateTimer.Value));
            //init += BindHelper.BindExpression(() => RetryToolTip(this.btnShowCertificate, null, null));

            // Custom collection bindings:
            init += mo.Registrations.Bind(() => this.cmbRegistration.Items.AsArray((RegistrationItem x) => x.RegistrationInfo),
                v => this.cmbRegistration.SetItems(v.AsArray((RegistrationInfo x) => new RegistrationItem(x))));

            init += mo.Registrations.Bind(() => this.lstRegistrations.Items.AsArray((RegistrationItem x) => x.RegistrationInfo),
                v => this.lstRegistrations.SetItems(v.AsArray((RegistrationInfo x) => new RegistrationItem(x))));

            init += mo.Domains.Bind(() => this.cmbDomain.Items.AsArray((object o) => o.ToString()),
                v => this.cmbDomain.SetItems(v));

            init += mo.Domains.Bind(() => this.lstDomains.Items.AsArray((object o) => o.ToString()),
                v => this.lstDomains.SetItems(v));

            init += ma.Challenges.Bind(() => this.lstChallenges.Items.AsArray((object o) => o.ToString()),
                v => this.lstChallenges.SetItems(v));

            init += ma.Certificates.Bind(() => this.lstCertificates.Items.AsArray((object o) => o.ToString()),
                v => this.lstCertificates.SetItems(v));

            // Manual changed events:
            mo.CanRegister.Changed += v => this.btnRegister.Enabled = v;
            mo.CanAcceptTos.Changed += v => this.btnAcceptTos.Enabled = v;
            mo.IsRegistrationCreated.Changed += v => this.lnkTOS.Enabled = v;
            mo.CanAddDomain.Changed += v => this.btnAddDomain.Enabled = v;
            mo.CanCreateChallenge.Changed += v => this.btnCreateChallenge.Enabled = v;
            mo.CanSaveChallenge.Changed += v => this.btnSaveChallenge.Enabled = v;

            mo.Files.Changed += this.UpdateFiles;

            init();
        }

        private void UpdateFiles(string[] v)
        {
            v = v ?? new string[0];
            this.txtSiteRoot.Tag = string.Join("\r\n", v);
            this.UpdateFiles();
        }

        private void SetToolTip(Control ctl, string msg)
        {
            if (msg != null)
            {
                if ((string)this.tooltip.Tag != msg)
                {
                    var tt = this.tooltip.ToolTipFor(ctl).ShowMessage(msg, useMarkdown: true);
                    tt.BorderColor = Color.Gold;
                }
                this.tooltip.Tag = msg;
            }
            else
                this.tooltip.ToolTipFor(ctl).Hide();
        }

        private void RetryToolTip(Control ctl, int? retry, int? timer)
        {
            if (retry.HasValue && timer.HasValue && retry <= Controller.MaxRetries && timer / 1000 + 1 > 0)
                SetToolTip(ctl, $"Retry {retry}/{Controller.MaxRetries} in {timer / 1000 + 1}s");
            else
                SetToolTip(ctl, null);
        }

        private void Error(Exception ex)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.ForeColor = Color.Crimson;
            this.richTextBox1.SelectedText = ex.Message.Trim() + "\n";
            this.richTextBox1.ScrollToCaret();
        }

        private void Warn(string message)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.ForeColor = Color.DarkOrange;
            this.richTextBox1.SelectedText = message.Trim() + "\n";
            this.richTextBox1.ScrollToCaret();
        }

        private void Success(string message)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.ForeColor = Color.ForestGreen;
            this.richTextBox1.SelectedText = message.Trim() + "\n";
            this.richTextBox1.ScrollToCaret();
        }

        private void Log(string message)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.ForeColor = Color.DarkSlateGray;
            this.richTextBox1.SelectedText = message.Trim() + "\n";
            this.richTextBox1.ScrollToCaret();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
            => await this.controller.Register();

        private async void btnAcceptTos_Click(object sender, EventArgs e)
            => await this.controller.AcceptTos();

        private async void lnkTOS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => await this.controller.TosLink();

        private async void btnAddDomain_Click(object sender, EventArgs e)
            => await this.controller.AddDomain();

        private async void btnCreateChallenge_Click(object sender, EventArgs e)
            => await this.controller.CreateChallenge();

        private async void lstRegistrations_SelectedIndexChanged(object sender, EventArgs e)
            => await this.controller.RegistrationListChanged();

        private async void lstDomains_SelectedIndexChanged(object sender, EventArgs e)
            => await this.controller.DomainListChanged();

        private async void btnSaveChallenge_Click(object sender, EventArgs e)
            => await this.controller.SaveChallenge();

        private async void btnCommitChallenge_Click(object sender, EventArgs e)
            => await this.controller.CommitChallenge();

        private async void btnTestChallenge_Click(object sender, EventArgs e)
            => await this.controller.TestChallenge();

        private async void btnValidate_Click(object sender, EventArgs e)
            => await this.controller.Validate();

        private async void btnCreateCertificate_Click(object sender, EventArgs e)
            => await this.controller.CreateCertificate();

        private async void btnSubmit_Click(object sender, EventArgs e)
            => await this.controller.SubmitCertificate();

        private async void btnSaveCertificate_Click(object sender, EventArgs e)
            => await this.controller.SaveCertificate();

        private void btnShowCertificate_Click(object sender, EventArgs e)
        {

        }

        private void txtSiteRoot_MouseLeave(object sender, EventArgs e)
        {
            this.tooltip.ToolTipFor(this.txtSiteRoot).Hide();
        }

        private void txtSiteRoot_MouseEnter(object sender, EventArgs e)
        {
            this.UpdateFiles();
        }

        private void txtSiteRoot_MouseMove(object sender, MouseEventArgs e)
        {
            this.UpdateFiles();
        }

        private void UpdateFiles()
        {
            var tt = this.tooltip.ToolTipFor(this.txtSiteRoot);
            tt.BorderColor = Color.Crimson;
            tt.PositionPreferences = ">,v,^,<";
            tt.ShowMessage((string)this.txtSiteRoot.Tag, useMarkdown: false);
        }
    }
}
