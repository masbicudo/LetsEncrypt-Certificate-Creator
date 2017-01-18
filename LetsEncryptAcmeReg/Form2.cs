using ACMESharp.Vault.Model;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ACMESharp.ACME;

namespace LetsEncryptAcmeReg
{
    public partial class Form2 : Form
    {
        readonly Acme acme = new Acme();
        readonly WizardBindableModel model = new WizardBindableModel();
        readonly ManagerBindableModel managerModel = new ManagerBindableModel();

        public Form2()
        {
            InitializeComponent();

            var mo = this.model;
            var ma = this.managerModel;

            Action init = null;

            init += () => mo.Registrations.Value = this.acme.GetRegistrations();

            // Control bindings:
            //      These are relations between the controls on the form
            //      and the bindable objects.
            init += BindHelper.Bind(mo.Email, this.cmbRegistration);
            init += BindHelper.Bind(mo.Domain, this.cmbDomain);
            init += BindHelper.Bind(mo.Challenge, this.cmbChallenge);
            init += BindHelper.Bind(mo.Target, this.txtChallengeTarget);
            init += BindHelper.Bind(mo.Key, this.txtChallengeTarget);
            init += BindHelper.Bind(mo.SiteRoot, this.txtChallengeTarget);
            init += BindHelper.Bind(mo.Certificate, this.txtChallengeTarget);
            init += BindHelper.Bind(mo.Issuer, this.txtChallengeTarget);
            init += BindHelper.Bind(mo.CertificateType, this.cmbCertificateType);
            init += BindHelper.Bind(mo.Password, this.txtPassword);
            init += BindHelper.Bind(mo.ShowPassword, this.chkShowPassword);

            init += BindHelper.Bind(mo.AutoAddDomain, this.chkAutoAddDomain);
            init += BindHelper.Bind(mo.AutoCreateChallenge, this.chkAutoCreateChallenge);
            init += BindHelper.Bind(mo.AutoSaveChallenge, this.chkAutoSaveChallenge);
            init += BindHelper.Bind(mo.AutoCommitChallenge, this.chkAutoCommit);
            init += BindHelper.Bind(mo.AutoTestChallenge, this.chkAutoTest);
            init += BindHelper.Bind(mo.AutoValidateChallenge, this.chkAutoValidate);
            init += BindHelper.Bind(mo.AutoCreateCertificate, this.chkAutoCreateCertificate);
            init += BindHelper.Bind(mo.AutoSubmitCertificate, this.chkAutoSubmit);
            init += BindHelper.Bind(mo.AutoSaveCertificate, this.chkAutoSaveCertificate);

            init += BindHelper.Bind(mo.CurrentRegistration, this.lstRegistrations, (RegistrationItem i) => i?.RegistrationInfo);
            init += BindHelper.Bind(mo.CurrentRegistration, this.cmbRegistration, (RegistrationItem i) => i?.RegistrationInfo);

            init += BindHelper.Bind(ma.Challenge, this.lstChallenges);

            // Custom collection bindings:
            init += BindHelper.Bind(mo.Registrations,
                () => this.cmbRegistration.Items.AsArray((RegistrationItem x) => x.RegistrationInfo),
                v => this.cmbRegistration.SetItems(v.AsArray((RegistrationInfo x) => new RegistrationItem(x))));

            init += BindHelper.Bind(mo.Registrations,
                () => this.lstRegistrations.Items.AsArray((RegistrationItem x) => x.RegistrationInfo),
                v => this.lstRegistrations.SetItems(v.AsArray((RegistrationInfo x) => new RegistrationItem(x))));

            init += BindHelper.Bind(mo.Domains,
                () => this.cmbDomain.Items.AsArray((object o) => o.ToString()),
                v => this.cmbDomain.SetItems(v));

            init += BindHelper.Bind(mo.Domains,
                () => this.lstDomains.Items.AsArray((object o) => o.ToString()),
                v => this.lstDomains.SetItems(v));

            init += BindHelper.Bind(ma.Challenges,
                () => this.lstChallenges.Items.AsArray((object o) => o.ToString()),
                v => this.lstChallenges.SetItems(v));

            init += BindHelper.Bind(ma.Certificates,
                () => this.lstCertificates.Items.AsArray((object o) => o.ToString()),
                v => this.lstCertificates.SetItems(v));

            // Complex relations:
            //      These relations are built by using expressions.
            //      Every bindable object in the right hand side will
            //      have a changed event added automatically.
            BindHelper.Assign(mo.IsEmailValid, () => !string.IsNullOrWhiteSpace(mo.Email.Value));
            BindHelper.Assign(mo.IsRegistrationCreated, () => mo.CurrentRegistration.Value != null);
            BindHelper.Assign(mo.Domains, () => this.acme.GetDomainsByEmail(mo.Email.Value).OrderBy(x => x).ToArray());
            BindHelper.Assign(mo.IsDomainValid, () => !string.IsNullOrWhiteSpace(mo.Domain.Value) && mo.IsEmailValid.Value);
            BindHelper.Assign(mo.IsDomainCreated, () => mo.Domains.Value.Any(i => i == mo.Domain.Value));
            BindHelper.Assign(mo.IsChallengeValid, () => !string.IsNullOrWhiteSpace(mo.Challenge.Value) && mo.IsDomainValid.Value);

            // Manual changed events:
            mo.IsEmailValid.Changed += v => this.btnRegister.Enabled = v;
            mo.IsRegistrationCreated.Changed += v => this.lnkTOS.Enabled = v;
            mo.IsDomainValid.Changed += v => this.btnAddDomain.Enabled = v;
            mo.IsChallengeValid.Changed += v => this.btnCreateChallenge.Enabled = v;

            Action<string> targetKeyChallengeSiteRootOnChanged = v =>
            {
                var b = !string.IsNullOrWhiteSpace(mo.Target.Value)
                        && !string.IsNullOrWhiteSpace(mo.Key.Value)
                        && (mo.Challenge.Value != "http-01" || !string.IsNullOrWhiteSpace(mo.SiteRoot.Value));
                if (b) this.chkAutoSaveChallenge.Enabled = this.btnSaveChallenge.Enabled = true;
            };
            mo.Target.Changed += targetKeyChallengeSiteRootOnChanged;
            mo.Key.Changed += targetKeyChallengeSiteRootOnChanged;
            mo.Challenge.Changed += targetKeyChallengeSiteRootOnChanged;
            mo.SiteRoot.Changed += targetKeyChallengeSiteRootOnChanged;

            init();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var newRegistration = this.acme.RegisterOrGet(this.model.Email.Value);
            var newList = this.model.Registrations.Value.Concat(new[] { newRegistration }).ToArray();
            this.model.Registrations.Value = newList;
        }

        private void lnkTOS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.acme.GetTOS(this.model.Email.Value));
        }

        private void btnAddDomain_Click(object sender, EventArgs e)
        {
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == this.tabPage2)
            {
            }
        }

        private void lstRegistrations_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.model.Domains.Value = this.acme.GetDomainsByRegistration(this.model.CurrentRegistration.Value);
        }

        private void lstDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            var challenges = this.acme.GetChallengesByDomain(this.model.CurrentRegistration.Value, this.model.Domain.Value)
                .Select(c => c.Type)
                .ToArray();
            this.managerModel.Challenges.Value = challenges;

            var certs = this.acme.GetCertificates(this.model.CurrentRegistration.Value, this.model.Domain.Value)
                .Select(c => c.Alias)
                .ToArray();
            this.managerModel.Certificates.Value = certs;
        }
    }

    public class RegistrationItem :
        IEquatable<RegistrationInfo>,
        IEquatable<RegistrationItem>
    {
        public RegistrationInfo RegistrationInfo { get; }

        public RegistrationItem(RegistrationInfo registrationInfo)
        {
            this.RegistrationInfo = registrationInfo;
        }

        public bool Equals(RegistrationInfo other)
        {
            if (other == null) return this.RegistrationInfo == null;
            if (this.RegistrationInfo == null) return false;
            return other.Id == this.RegistrationInfo.Id;
        }

        public bool Equals(RegistrationItem other)
        {
            if (other?.RegistrationInfo == null) return this.RegistrationInfo == null;
            if (this.RegistrationInfo == null) return false;
            return other.RegistrationInfo.Id == this.RegistrationInfo.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as RegistrationInfo)
                || this.Equals(obj as RegistrationItem)
                || base.Equals(obj);
        }

        public override string ToString()
        {
            return string.Join("; ", this.RegistrationInfo.Registration.Contacts.Select(x => x?.Substring(7)));
        }

        public static explicit operator RegistrationInfo(RegistrationItem reg)
        {
            return reg.RegistrationInfo;
        }

        public static explicit operator RegistrationItem(RegistrationInfo reg)
        {
            return new RegistrationItem(reg);
        }
    }
}
