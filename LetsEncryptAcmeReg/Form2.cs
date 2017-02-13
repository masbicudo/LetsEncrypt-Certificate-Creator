using ACMESharp.Vault.Model;
using LetsEncryptAcmeReg.SSG;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public partial class Form2 : Form,
        IUIServices,
        ITooltipCreator
    {
        private readonly Controller controller;
        private readonly Acme acme = new Acme();
        private readonly ToolTipManager tooltip = new ToolTipManager();

        public Form2()
        {
            InitializeComponent();

            // Controller
            this.controller = new Controller(this.acme)
            {
                Error = this.Error,
                Warn = this.Warn,
                Log = this.Log,
                Success = this.Success,
            };

            var mo = this.controller.Model;
            var ma = this.controller.ManagerModel;
            var mc = this.controller.CertViewModel;

            // Tool tips
            this.ToolTipFor(this.btnRegister, Messages.ToolTipForRegister);
            mo.TOSLink.Changed += s => this.DataTipFor(this.lnkTOS, s, ">,v,>v,>v,>^,<v,<^");
            this.ToolTipFor(this.btnAcceptTos, Messages.ToolTipForAcceptTos);
            this.ToolTipFor(this.btnAddDomain, Messages.ToolTipForAddDomain);
            this.ToolTipFor(this.cmbDomain, Messages.ToolTipForDomain);

            this.ToolTipFor(this.btnUpdateStatus, Messages.ToolTipForUpdateStatus);

            this.ToolTipFor(this.btnRegister, Messages.ToolTipForRegister);

            this.ToolTipFor(this.lnkGitLabCertHelp, Messages.ToolTipForGitLabCertHelp, "v,>,<,^,>v,>^,<v,<^");

            this.txtSiteRoot.MouseMove += this.txtSiteRoot_MouseMove;
            this.txtSiteRoot.MouseEnter += this.txtSiteRoot_MouseEnter;
            this.txtSiteRoot.MouseLeave += this.txtSiteRoot_MouseLeave;

            // Model bindings

            BindResult init = this.controller.Initialize();

            // Control bindings:
            //      These are relations between the controls on the form
            //      and the bindable objects.
            init += mo.Email.BindControl(this.cmbRegistration);
            init += mo.Domain.BindControl(this.cmbDomain);
            init += mo.Challenge.BindControl(this.cmbChallenge);
            init += mo.Target.BindControl(this.txtChallengeTarget);
            init += mo.Key.BindControl(this.txtChallengeKey);
            init += mo.SiteRoot.BindControl(this.txtSiteRoot);
            init += mo.Certificate.BindControl(this.cmbCertificate, s => s?.StartsWith("[new] ") == true ? s.Substring(6) : s);
            init += mo.Issuer.BindControl(this.txtIssuer);
            init += mo.CertificateType.BindControl(this.cmbCertificateType, StringToCertTypeEnum, CertTypeEnumToString);
            init += mo.Password.BindControl(this.txtPassword);
            init += mo.ShowPassword.BindControl(this.chkShowPassword);
            init += mo.GitUserName.BindControl(this.txtGitUserName);
            init += mo.GitPassword.BindControl(this.txtGitPassword);
            init += mo.SavePath.BindControl(this.txtSavePath);

            init += mo.AutoRegister.BindControl(this.chkAutoRegister);
            init += mo.AutoAcceptTos.BindControl(this.chkAutoAcceptTos);
            init += mo.AutoAddDomain.BindControl(this.chkAutoAddDomain);
            init += mo.AutoCreateChallenge.BindControl(this.chkAutoCreateChallenge);
            init += mo.AutoSaveChallenge.BindControl(this.chkAutoSaveChallenge);
            init += mo.AutoCommitChallenge.BindControl(this.chkAutoCommit);
            init += mo.AutoTestChallenge.BindControl(this.chkAutoTest);
            init += mo.AutoValidateChallenge.BindControl(this.chkAutoValidate);
            init += mo.AutoUpdateStatus.BindControl(this.chkAutoUpdateStatus);
            init += mo.AutoCreateCertificate.BindControl(this.chkAutoCreateCertificate);
            init += mo.AutoSubmitCertificate.BindControl(this.chkAutoSubmit);
            init += mo.AutoGetIssuerCertificate.BindControl(this.chkAutoGetIssuerCert);
            init += mo.AutoSaveOrShowCertificate.BindControl(this.chkAutoSaveOrShowCertificate);

            init += mo.CurrentRegistration.Bind(this.lstRegistrations, (RegistrationItem i) => i?.RegistrationInfo);
            init += mo.CurrentRegistration.BindControl(this.cmbRegistration, (RegistrationItem i) => i?.RegistrationInfo);

            init += mo.Domain.BindControl(this.lstDomains);

            init += ma.Challenge.BindControl(this.lstChallenges);

            // Cert view controls
            init += mc.Certificate.BindControl(this.cmbAllCerts);
            init += mc.Certificates.Bind(() => this.cmbAllCerts.Items.OfType<string>().ToArray());
            init += BindHelper.BindExpression(() => this.cmbAllCerts.SetItems(mc.Certificates.Value));
            init += mc.CertificateType.BindControl(this.cmbCertViewType, StringToCertTypeEnum, CertTypeEnumToString);
            init += mc.Base64Data.BindControl(this.txtCertBase64Data);

            // 
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnRegister, mo.AutoRegisterRetry.Value, mo.AutoRegisterTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnAcceptTos, mo.AutoAcceptTosRetry.Value, mo.AutoAcceptTosTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnAddDomain, mo.AutoAddDomainRetry.Value, mo.AutoAddDomainTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnCreateChallenge, mo.AutoCreateChallengeRetry.Value, mo.AutoCreateChallengeTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnSaveChallenge, mo.AutoSaveChallengeRetry.Value, mo.AutoSaveChallengeTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnCommitChallenge, mo.AutoCommitChallengeRetry.Value, mo.AutoCommitChallengeTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnTestChallenge, mo.AutoTestChallengeRetry.Value, mo.AutoTestChallengeTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnValidate, mo.AutoValidateChallengeRetry.Value, mo.AutoValidateChallengeTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnUpdateStatus, mo.AutoUpdateStatusRetry.Value, mo.AutoUpdateStatusTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnCreateCertificate, mo.AutoCreateCertificateRetry.Value, mo.AutoCreateCertificateTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnSubmit, mo.AutoSubmitCertificateRetry.Value, mo.AutoSubmitCertificateTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnGetIssuerCert, mo.AutoGetIssuerCertificateRetry.Value, mo.AutoGetIssuerCertificateTimer.Value));
            init += BindHelper.BindExpression(() => this.RetryToolTip(this.btnSaveCertificate, mo.AutoSaveOrShowCertificateRetry.Value, mo.AutoSaveOrShowCertificateTimer.Value));
            //init += BindHelper.BindExpression(() => RetryToolTip(this.btnShowCertificate, null, null));

            // Custom collection bindings:
            init += mo.Registrations.Bind(
                () => this.cmbRegistration.Items.AsArray((RegistrationItem x) => x.RegistrationInfo),
                v => this.cmbRegistration.SetItems(v.AsArray((RegistrationInfo x) => new RegistrationItem(x))));

            init += mo.Registrations.Bind(
                () => this.lstRegistrations.Items.AsArray((RegistrationItem x) => x.RegistrationInfo),
                v => this.lstRegistrations.SetItems(v.AsArray((RegistrationInfo x) => new RegistrationItem(x))));

            init += mo.Domains.Bind
                (() => this.cmbDomain.Items.AsArray((object o) => o.ToString()),
                v => this.cmbDomain.SetItems(v));

            init += mo.Domains.Bind(
                () => this.lstDomains.Items.AsArray((object o) => o.ToString()),
                v => this.lstDomains.SetItems(v));

            init += ma.Challenges.Bind(
                () => this.lstChallenges.Items.AsArray((object o) => o.ToString()),
                v => this.lstChallenges.SetItems(v));

            init += ma.Certificates.Bind(
                () => this.lstCertificates.Items.OfType<string>().ToArray(),
                v => this.lstCertificates.SetItems(v));

            init += mo.Certificates.Bind(() => this.cmbCertificate.Items.OfType<string>().ToArray());
            init += BindHelper.BindExpression(() => this.SetItemsOf_cmbCertificate(mo.Certificates.Value, mo.Domain.Value, mo.Date.Value));

            init += mo.SsgName.BindControl(this.cmbSsg);
            init += mo.SsgTypes.Bind(
                () => this.cmbSsg.Items.OfType<string>().ToArray(),
                v => this.cmbSsg.SetItems(v));

            // Manual changed events:
            mo.CanRegister.Changed += v => this.btnRegister.Enabled = v;
            mo.CanAcceptTos.Changed += v => this.btnAcceptTos.Enabled = v;
            mo.IsRegistrationCreated.Changed += v => this.lnkTOS.Enabled = v;
            mo.CanAddDomain.Changed += v => this.btnAddDomain.Enabled = v;
            mo.CanCreateChallenge.Changed += v => this.btnCreateChallenge.Enabled = v;
            mo.CanSaveChallenge.Changed += v => this.btnSaveChallenge.Enabled = v;
            mo.CanCommitChallenge.Changed += v => this.btnCommitChallenge.Enabled = v;
            mo.CanTestChallenge.Changed += v => this.btnTestChallenge.Enabled = v;
            mo.CanValidateChallenge.Changed += v => this.btnValidate.Enabled = v;
            mo.CanUpdateStatus.Changed += v => this.btnUpdateStatus.Enabled = v;
            mo.CanCreateCertificate.Changed += v => this.btnCreateCertificate.Enabled = v;
            mo.CanSubmitCertificate.Changed += v => this.btnSubmit.Enabled = v;
            mo.CanGetIssuerCertificate.Changed += v => this.btnGetIssuerCert.Enabled = v;
            mo.CanSaveCertificate.Changed += v => this.btnSaveCertificate.Enabled = v;
            mo.CanShowCertificate.Changed += v => this.btnShowCertificate.Enabled = v;
            mo.ShowPassword.Changed += b => this.txtPassword.UseSystemPasswordChar = !b;

            mo.IsPasswordEnabled.Changed += v => this.txtPassword.Enabled = this.chkShowPassword.Enabled = v;

            mo.Files.Changed += this.UpdateFiles;

            mo.CurrentAuthState.Changing += CurrentAuthState_Changing;
            mo.CurrentAuthState.Changed += CurrentAuthState_Changed;

            init.InitAction?.Invoke();

            this.lstCertDomains.ItemCheck += (s, a) =>
            {
                if (this.lstCertDomains.Items[a.Index].ToString() == mo.Domain.Value)
                    a.NewValue = CheckState.Checked;
            };
            mo.CurrentIdentifier.Changed += s => this.tableCertDomains.Hide();

            this.tableCertDomains.Width = this.Width / 2;
        }

        #region Bind events

        private void SetItemsOf_cmbCertificate(string[] certList, string domain, DateTime now)
        {
            var newItem = new CreateNewCertItem(domain, now);

            if (certList == null)
                return;

            // ReSharper disable once RedundantEnumerableCastCall
            var items = certList.Cast<object>();
            if (!certList.Contains(newItem.Name))
                items = items.Append(newItem);

            this.cmbCertificate.SetItems(items.ToArray());
        }

        private static string CertTypeEnumToString(CertType arg)
        {
            switch (arg)
            {
                case CertType.KeyPEM: return "Key PEM";
                case CertType.CsrPEM: return "Csr PEM";
                case CertType.CertificatePEM: return "Certificate PEM";
                case CertType.CertificateDER: return "Certificate DER";
                case CertType.IssuerPEM: return "Issuer PEM";
                case CertType.IssuerDER: return "Issuer DER";
                case CertType.Pkcs12: return "Pkcs12";
                default: return "";
            }
        }

        private static CertType StringToCertTypeEnum(string li)
        {
            var str = li?.Replace(" ", "");
            CertType val;
            Enum.TryParse(str, false, out val);
            return val;
        }

        private void CurrentAuthState_Changed(ACMESharp.AuthorizationState obj)
        {
            // if the value has changed, then it means that we must try to update the value
            // but only if it has been submited already
            Action whatToDo = async () =>
            {
                await this.controller.CatchError(async () => { await this.controller.UpdateStatusOnce(); });
            };

            if (this.controller.Model.CurrentChallenge.Value?.SubmitDate != null
                && this.controller.Model.CurrentChallenge.Value?.IsPending() == true)
                if (this.IsHandleCreated)
                    this.BeginInvoke(whatToDo);
                else
                    whatToDo();
        }

        private void CurrentAuthState_Changing(Bindable<ACMESharp.AuthorizationState> sender, ACMESharp.AuthorizationState value, ACMESharp.AuthorizationState prev, ref bool cancel)
        {
            // if values are equal, then we must cancel the change event from happening
            cancel |= sender.Version > 0
                     && (value == prev
                         ||
                         value != null && prev != null
                         && value.Status == prev.Status
                         && value.Challenges.All(c => prev.Challenges.Any(pc =>
                             c.Type == pc.Type
                             && c.Token == pc.Token
                             && c.SubmitDate == pc.SubmitDate)));
        }

        private void UpdateFiles(string[] v)
        {
            v = v ?? new string[0];
            this.txtSiteRoot.Tag = string.Join("\r\n", v);
            this.UpdateFiles();
        }

        #endregion

        #region UI tips and messages

        public void ToolTipFor(Control ctl, string message, string positionPreferences = ">,v,^,<,>v,>^,<v,<^")
        {
            var tt = this.tooltip.ToolTipFor(ctl, "Help")
                .AutoPopup(message, useMarkdown: true);
            tt.PositionPreferences = positionPreferences;
            tt.BorderColor = Color.DodgerBlue;
            tt.Margin = 1;
        }

        private void DataTipFor(Control ctl, string text, string positionPreferences = ">,v,^,<,>v,>^,<v,<^", bool auto = true, bool useMarkdown = false)
        {
            var tt = this.tooltip.ToolTipFor(ctl, "Data");
            tt.BorderColor = Color.Crimson;
            tt.PositionPreferences = positionPreferences;
            tt.Margin = 1;

            if (auto) tt.AutoPopup(text, useMarkdown);
            else tt.ShowMessage(text, useMarkdown);
        }

        private void SetActionToolTip(Control ctl, string msg)
        {
            if (msg != null)
            {
                if ((string)this.tooltip.Tag != msg)
                {
                    var tt = this.tooltip.ToolTipFor(ctl, "Action");
                    tt.BorderColor = Color.Gold;
                    tt.PriorityOrder = -1; // less = more priority
                    tt.ShowMessage(msg, useMarkdown: true);
                }
                this.tooltip.Tag = msg;
            }
            else
                this.tooltip.ToolTipFor(ctl, "Action").Hide();
        }

        private void RetryToolTip(Control ctl, int? retry, int? timer)
        {
            if (retry.HasValue && timer.HasValue && timer / 1000 > 0)
                SetActionToolTip(ctl, $"Retry #{retry + 1} in {timer / 1000 + 1}s");
            else
                SetActionToolTip(ctl, null);
        }

        private void Error(Exception ex)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.SelectionColor = Color.Crimson;
            this.richTextBox1.SelectedText = ex.Message.Trim() + "\n";
            this.richTextBox1.ScrollToCaret();
        }

        private void Warn(string message)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.SelectionColor = Color.DarkOrange;
            this.richTextBox1.SelectedText = message.Trim() + "\n";
            this.richTextBox1.ScrollToCaret();
        }

        private void Success(string message)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.SelectionColor = Color.ForestGreen;
            this.richTextBox1.SelectedText = message.Trim() + "\n";
            this.richTextBox1.ScrollToCaret();
        }

        private void Log(string message)
        {
            this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.SelectionColor = Color.DarkSlateGray;
            this.richTextBox1.SelectedText = message.Trim() + "\n";
            this.richTextBox1.ScrollToCaret();
        }

        #endregion

        private async void btnRegister_Click(object sender, EventArgs e)
            => await this.controller.Register();

        private async void btnAcceptTos_Click(object sender, EventArgs e)
            => await this.controller.AcceptTos();

        private async void lnkTOS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => await this.controller.OpenTosInBrowser();

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

        private async void btnUpdateStatus_Click(object sender, EventArgs e)
            => await this.controller.UpdateStatus();

        private async void btnCreateCertificate_Click(object sender, EventArgs e)
            => await this.controller.CreateCertificate();

        private async void btnSubmit_Click(object sender, EventArgs e)
            => await this.controller.SubmitCertificate();

        private async void btnGetIssuerCert_Click(object sender, EventArgs e)
            => await this.controller.GetIssuerCertificate();

        private async void btnSaveCertificate_Click(object sender, EventArgs e)
            => await this.controller.SaveCertificate();

        private void btnShowCertificate_Click(object sender, EventArgs e)
        {
            this.controller.ViewCertificate(this.controller.Model.Certificate.Value, this.controller.Model.CertificateType.Value);
            this.tabControl1.SelectedTab = this.tabPage3;
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
            this.DataTipFor(this.txtSiteRoot, (string)this.txtSiteRoot.Tag, ">,v,^,<");
        }

        private void richTextBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var rtb = (RichTextBox)sender;
            var pos = rtb.GetCharIndexFromPosition(e.Location);
            var line = rtb.GetLineFromCharIndex(pos);
            var start = rtb.GetFirstCharIndexFromLine(line);
            var end = rtb.GetFirstCharIndexFromLine(line + 1) - 1;

            if (end - start < 0)
                return;

            //if (rtb.SelectionStart != start)
            //    rtb.SelectionStart = start;
            //if (rtb.SelectionLength != end - start)
            //    rtb.SelectionLength = end - start;
            var text = rtb.Text.Substring(start, end - start);
            var pt = rtb.GetPositionFromCharIndex(end);
            var tt = this.tooltip.ToolTipFor(rtb)
                .ShowMessageAt(Messages.MapMessageToExplanation(text), pt);
            tt.PositionPreferences = ">,^";
            tt.Margin = 1;
        }

        private void richTextBox1_MouseLeave(object sender, EventArgs e)
        {
            var rtb = (RichTextBox)sender;
            this.tooltip.ToolTipFor(rtb)
                .Hide();
        }

        private void btnDeleteRegistration_Click(object sender, EventArgs e)
        {
            this.controller.DeleteCurrentRegistration();
        }

        private void btnDeleteDomain_Click(object sender, EventArgs e)
        {
            this.controller.DeleteCurrentDomain();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.controller.Model.Now.Value = DateTime.Now;
        }


        private void btnChangeSavePath_Click(object sender, EventArgs e)
        {
            var path = this.controller.Model.ExpandedSavePath.Value;
            var fname = Path.GetFileName(path);
            var ext = Controller.GetExt(this.controller.Model.CertificateType.Value);

            var save = new SaveFileDialog
            {
                CheckPathExists = true,
                InitialDirectory = Path.GetDirectoryName(path),
                FileName = fname,
                CreatePrompt = false,
                DefaultExt = ext,
                Filter = $"Certificate | *.{ext}",
                RestoreDirectory = true,
            };

            if (save.ShowDialog(this) == DialogResult.OK)
                this.controller.Model.SavePath.Value = save.FileName;
        }

        internal class CreateNewCertItem
        {
            private readonly string domain;
            private readonly DateTime now;

            public CreateNewCertItem(string domain, DateTime now)
            {
                this.domain = domain;
                this.now = now;
            }

            public override string ToString()
            {
                return $"[new] {this.Name}";
            }

            public string Name => $"{this.domain} - {this.now.Date.ToString("yyyy-MM-dd")}";
        }

        private void btnCopyCertBase64Data_Click(object sender, EventArgs e)
        {
            var value = this.controller.CertViewModel.Base64Data.Value;
            if (!string.IsNullOrWhiteSpace(value))
                Clipboard.SetText(value);
        }

        private void lnkGitLabCertHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.controller.OpenPemConcatHelpInBrowser().RunSynchronously();
        }

        private void btnHideCertDomains_Click(object sender, EventArgs e)
        {
            this.HideAndSaveCertDomains();
        }

        private void btnCertDomains_Click(object sender, EventArgs e)
        {
            if (this.tableCertDomains.Visible)
                this.HideAndSaveCertDomains();
            else
                this.ShowCertDomains();
        }

        private void HideAndSaveCertDomains()
        {
            var mo = this.controller.Model;
            mo.CertificateDomains.Value = this.lstCertDomains.CheckedItems.AsArray((object o) => o.ToString())
                .Except(new[] { mo.Domain.Value })
                .Where(x => x != null)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            this.tableCertDomains.Visible = false;
        }

        private void ShowCertDomains()
        {
            var mo = this.controller.Model;
            this.lstCertDomains.SetItems(
                (mo.AvailableDomains.Value ?? new string[0]).Append(mo.Domain.Value)
                    .Where(x => x != null)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray(),
                (mo.CertificateDomains.Value ?? new string[0]).Append(mo.Domain.Value)
                    .Where(x => x != null)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray());

            this.tableCertDomains.Visible = true;
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            this.tableCertDomains.Width = this.Width / 2;
        }

        public IControlCreatorAndBinder CreatePanelForSsg()
        {
            return new WinFormsControlCreatorAndBinder(new FlowLayoutAppender(this.flowSsgControls), this);
        }
    }
}
