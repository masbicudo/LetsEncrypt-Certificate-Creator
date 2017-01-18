﻿namespace LetsEncryptAcmeReg
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCreateChallenge = new System.Windows.Forms.Button();
            this.cmbChallenge = new System.Windows.Forms.ComboBox();
            this.btnAddDomain = new System.Windows.Forms.Button();
            this.cmbDomain = new System.Windows.Forms.ComboBox();
            this.chkAutoAddDomain = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnChallengeFile = new System.Windows.Forms.Button();
            this.txtChallengeKey = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtChallengeTarget = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtChallengeFile = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.chkAutoSaveChallenge = new System.Windows.Forms.CheckBox();
            this.btnSaveChallenge = new System.Windows.Forms.Button();
            this.btnCommitChallenge = new System.Windows.Forms.Button();
            this.btnTestChallenge = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.chkAutoCommit = new System.Windows.Forms.CheckBox();
            this.chkAutoTest = new System.Windows.Forms.CheckBox();
            this.chkAutoValidate = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbCertificate = new System.Windows.Forms.ComboBox();
            this.btnCreateCertificate = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnShowCertificate = new System.Windows.Forms.Button();
            this.btnSaveCertificate = new System.Windows.Forms.Button();
            this.chkAutoSaveCertificate = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbCertificateType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtIssuer = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.chkAutoCreateChallenge = new System.Windows.Forms.CheckBox();
            this.chkAutoCreateCertificate = new System.Windows.Forms.CheckBox();
            this.chkAutoSubmit = new System.Windows.Forms.CheckBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.lnkTOS = new System.Windows.Forms.LinkLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.cmbRegistration = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.lstCertificates = new System.Windows.Forms.ListBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lstChallenges = new System.Windows.Forms.ListBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lstRegistrations = new System.Windows.Forms.ListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lstDomains = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(9, 9);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(606, 423);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(598, 397);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Wizard";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnCreateChallenge, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.cmbChallenge, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnAddDomain, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.cmbDomain, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.chkAutoAddDomain, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.cmbCertificate, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnCreateCertificate, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.chkAutoCreateChallenge, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkAutoCreateCertificate, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.chkAutoSubmit, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnSubmit, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnRegister, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.cmbRegistration, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(592, 391);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btnCreateChallenge
            // 
            this.btnCreateChallenge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateChallenge.AutoSize = true;
            this.btnCreateChallenge.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCreateChallenge.Location = new System.Drawing.Point(542, 66);
            this.btnCreateChallenge.Margin = new System.Windows.Forms.Padding(1);
            this.btnCreateChallenge.Name = "btnCreateChallenge";
            this.btnCreateChallenge.Size = new System.Drawing.Size(49, 23);
            this.btnCreateChallenge.TabIndex = 10;
            this.btnCreateChallenge.Text = "Create";
            this.btnCreateChallenge.UseVisualStyleBackColor = true;
            // 
            // cmbChallenge
            // 
            this.cmbChallenge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbChallenge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChallenge.FormattingEnabled = true;
            this.cmbChallenge.Items.AddRange(new object[] {
            "http-01"});
            this.cmbChallenge.Location = new System.Drawing.Point(97, 67);
            this.cmbChallenge.Margin = new System.Windows.Forms.Padding(1);
            this.cmbChallenge.Name = "cmbChallenge";
            this.cmbChallenge.Size = new System.Drawing.Size(418, 21);
            this.cmbChallenge.TabIndex = 9;
            // 
            // btnAddDomain
            // 
            this.btnAddDomain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddDomain.AutoSize = true;
            this.btnAddDomain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddDomain.Location = new System.Drawing.Point(542, 41);
            this.btnAddDomain.Margin = new System.Windows.Forms.Padding(1);
            this.btnAddDomain.Name = "btnAddDomain";
            this.btnAddDomain.Size = new System.Drawing.Size(49, 23);
            this.btnAddDomain.TabIndex = 5;
            this.btnAddDomain.Text = "Add";
            this.btnAddDomain.UseVisualStyleBackColor = true;
            this.btnAddDomain.Click += new System.EventHandler(this.btnAddDomain_Click);
            // 
            // cmbDomain
            // 
            this.cmbDomain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDomain.FormattingEnabled = true;
            this.cmbDomain.Location = new System.Drawing.Point(97, 42);
            this.cmbDomain.Margin = new System.Windows.Forms.Padding(1);
            this.cmbDomain.Name = "cmbDomain";
            this.cmbDomain.Size = new System.Drawing.Size(418, 21);
            this.cmbDomain.TabIndex = 4;
            this.cmbDomain.Text = "www.masbicudo.com";
            // 
            // chkAutoAddDomain
            // 
            this.chkAutoAddDomain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoAddDomain.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoAddDomain.AutoSize = true;
            this.chkAutoAddDomain.Checked = true;
            this.chkAutoAddDomain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoAddDomain.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoAddDomain.Location = new System.Drawing.Point(517, 41);
            this.chkAutoAddDomain.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoAddDomain.Name = "chkAutoAddDomain";
            this.chkAutoAddDomain.Size = new System.Drawing.Size(23, 23);
            this.chkAutoAddDomain.TabIndex = 11;
            this.chkAutoAddDomain.Text = ">";
            this.chkAutoAddDomain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoAddDomain.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Domain";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Registration E-mail";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 71);
            this.label3.Margin = new System.Windows.Forms.Padding(1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Challenge";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnChallengeFile, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtChallengeKey, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtChallengeTarget, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtChallengeFile, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(96, 90);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(420, 94);
            this.tableLayoutPanel2.TabIndex = 11;
            // 
            // btnChallengeFile
            // 
            this.btnChallengeFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChallengeFile.AutoSize = true;
            this.btnChallengeFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnChallengeFile.Location = new System.Drawing.Point(393, 45);
            this.btnChallengeFile.Margin = new System.Windows.Forms.Padding(1);
            this.btnChallengeFile.Name = "btnChallengeFile";
            this.btnChallengeFile.Size = new System.Drawing.Size(26, 23);
            this.btnChallengeFile.TabIndex = 10;
            this.btnChallengeFile.Text = "...";
            this.btnChallengeFile.UseVisualStyleBackColor = true;
            // 
            // txtChallengeKey
            // 
            this.txtChallengeKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtChallengeKey, 2);
            this.txtChallengeKey.Location = new System.Drawing.Point(57, 23);
            this.txtChallengeKey.Margin = new System.Windows.Forms.Padding(1);
            this.txtChallengeKey.Name = "txtChallengeKey";
            this.txtChallengeKey.ReadOnly = true;
            this.txtChallengeKey.Size = new System.Drawing.Size(362, 20);
            this.txtChallengeKey.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1, 26);
            this.label5.Margin = new System.Windows.Forms.Padding(1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Key";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 4);
            this.label4.Margin = new System.Windows.Forms.Padding(1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Target";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtChallengeTarget
            // 
            this.txtChallengeTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtChallengeTarget, 2);
            this.txtChallengeTarget.Location = new System.Drawing.Point(57, 1);
            this.txtChallengeTarget.Margin = new System.Windows.Forms.Padding(1);
            this.txtChallengeTarget.Name = "txtChallengeTarget";
            this.txtChallengeTarget.ReadOnly = true;
            this.txtChallengeTarget.Size = new System.Drawing.Size(362, 20);
            this.txtChallengeTarget.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, 50);
            this.label6.Margin = new System.Windows.Forms.Padding(1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Local root";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtChallengeFile
            // 
            this.txtChallengeFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChallengeFile.Location = new System.Drawing.Point(57, 46);
            this.txtChallengeFile.Margin = new System.Windows.Forms.Padding(1);
            this.txtChallengeFile.Name = "txtChallengeFile";
            this.txtChallengeFile.Size = new System.Drawing.Size(334, 20);
            this.txtChallengeFile.TabIndex = 15;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 8;
            this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel3, 3);
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.chkAutoSaveChallenge, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnSaveChallenge, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnCommitChallenge, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnTestChallenge, 5, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnValidate, 7, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkAutoCommit, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkAutoTest, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkAutoValidate, 6, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 69);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(420, 25);
            this.tableLayoutPanel3.TabIndex = 17;
            // 
            // chkAutoSaveChallenge
            // 
            this.chkAutoSaveChallenge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoSaveChallenge.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoSaveChallenge.AutoSize = true;
            this.chkAutoSaveChallenge.Checked = true;
            this.chkAutoSaveChallenge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSaveChallenge.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoSaveChallenge.Location = new System.Drawing.Point(1, 1);
            this.chkAutoSaveChallenge.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoSaveChallenge.Name = "chkAutoSaveChallenge";
            this.chkAutoSaveChallenge.Size = new System.Drawing.Size(23, 23);
            this.chkAutoSaveChallenge.TabIndex = 12;
            this.chkAutoSaveChallenge.Text = ">";
            this.chkAutoSaveChallenge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoSaveChallenge.UseVisualStyleBackColor = true;
            // 
            // btnSaveChallenge
            // 
            this.btnSaveChallenge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveChallenge.AutoSize = true;
            this.btnSaveChallenge.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveChallenge.Location = new System.Drawing.Point(26, 1);
            this.btnSaveChallenge.Margin = new System.Windows.Forms.Padding(1);
            this.btnSaveChallenge.Name = "btnSaveChallenge";
            this.btnSaveChallenge.Size = new System.Drawing.Size(78, 23);
            this.btnSaveChallenge.TabIndex = 10;
            this.btnSaveChallenge.Text = "Save";
            this.btnSaveChallenge.UseVisualStyleBackColor = true;
            // 
            // btnCommitChallenge
            // 
            this.btnCommitChallenge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommitChallenge.AutoSize = true;
            this.btnCommitChallenge.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCommitChallenge.Location = new System.Drawing.Point(131, 1);
            this.btnCommitChallenge.Margin = new System.Windows.Forms.Padding(1);
            this.btnCommitChallenge.Name = "btnCommitChallenge";
            this.btnCommitChallenge.Size = new System.Drawing.Size(78, 23);
            this.btnCommitChallenge.TabIndex = 10;
            this.btnCommitChallenge.Text = "Commit";
            this.btnCommitChallenge.UseVisualStyleBackColor = true;
            // 
            // btnTestChallenge
            // 
            this.btnTestChallenge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestChallenge.AutoSize = true;
            this.btnTestChallenge.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTestChallenge.Location = new System.Drawing.Point(236, 1);
            this.btnTestChallenge.Margin = new System.Windows.Forms.Padding(1);
            this.btnTestChallenge.Name = "btnTestChallenge";
            this.btnTestChallenge.Size = new System.Drawing.Size(78, 23);
            this.btnTestChallenge.TabIndex = 10;
            this.btnTestChallenge.Text = "Test";
            this.btnTestChallenge.UseVisualStyleBackColor = true;
            // 
            // btnValidate
            // 
            this.btnValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValidate.AutoSize = true;
            this.btnValidate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnValidate.Location = new System.Drawing.Point(341, 1);
            this.btnValidate.Margin = new System.Windows.Forms.Padding(1);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(78, 23);
            this.btnValidate.TabIndex = 10;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            // 
            // chkAutoCommit
            // 
            this.chkAutoCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoCommit.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoCommit.AutoSize = true;
            this.chkAutoCommit.Checked = true;
            this.chkAutoCommit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoCommit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoCommit.Location = new System.Drawing.Point(106, 1);
            this.chkAutoCommit.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoCommit.Name = "chkAutoCommit";
            this.chkAutoCommit.Size = new System.Drawing.Size(23, 23);
            this.chkAutoCommit.TabIndex = 11;
            this.chkAutoCommit.Text = ">";
            this.chkAutoCommit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoCommit.UseVisualStyleBackColor = true;
            // 
            // chkAutoTest
            // 
            this.chkAutoTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoTest.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoTest.AutoSize = true;
            this.chkAutoTest.Checked = true;
            this.chkAutoTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoTest.Location = new System.Drawing.Point(211, 1);
            this.chkAutoTest.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoTest.Name = "chkAutoTest";
            this.chkAutoTest.Size = new System.Drawing.Size(23, 23);
            this.chkAutoTest.TabIndex = 11;
            this.chkAutoTest.Text = ">";
            this.chkAutoTest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoTest.UseVisualStyleBackColor = true;
            // 
            // chkAutoValidate
            // 
            this.chkAutoValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoValidate.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoValidate.AutoSize = true;
            this.chkAutoValidate.Checked = true;
            this.chkAutoValidate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoValidate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoValidate.Location = new System.Drawing.Point(316, 1);
            this.chkAutoValidate.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoValidate.Name = "chkAutoValidate";
            this.chkAutoValidate.Size = new System.Drawing.Size(23, 23);
            this.chkAutoValidate.TabIndex = 11;
            this.chkAutoValidate.Text = ">";
            this.chkAutoValidate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoValidate.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 190);
            this.label7.Margin = new System.Windows.Forms.Padding(1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Certificate";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbCertificate
            // 
            this.cmbCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCertificate.FormattingEnabled = true;
            this.cmbCertificate.Location = new System.Drawing.Point(97, 186);
            this.cmbCertificate.Margin = new System.Windows.Forms.Padding(1);
            this.cmbCertificate.Name = "cmbCertificate";
            this.cmbCertificate.Size = new System.Drawing.Size(418, 21);
            this.cmbCertificate.TabIndex = 1;
            // 
            // btnCreateCertificate
            // 
            this.btnCreateCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateCertificate.AutoSize = true;
            this.btnCreateCertificate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCreateCertificate.Location = new System.Drawing.Point(542, 185);
            this.btnCreateCertificate.Margin = new System.Windows.Forms.Padding(1);
            this.btnCreateCertificate.Name = "btnCreateCertificate";
            this.btnCreateCertificate.Size = new System.Drawing.Size(49, 23);
            this.btnCreateCertificate.TabIndex = 10;
            this.btnCreateCertificate.Text = "Create";
            this.btnCreateCertificate.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.cmbCertificateType, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.txtIssuer, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtPassword, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.chkShowPassword, 2, 2);
            this.tableLayoutPanel4.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(96, 234);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 5;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(420, 92);
            this.tableLayoutPanel4.TabIndex = 13;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 4;
            this.tableLayoutPanel4.SetColumnSpan(this.tableLayoutPanel5, 3);
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.btnShowCertificate, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnSaveCertificate, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.chkAutoSaveCertificate, 1, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 67);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(420, 25);
            this.tableLayoutPanel5.TabIndex = 18;
            // 
            // btnShowCertificate
            // 
            this.btnShowCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowCertificate.AutoSize = true;
            this.btnShowCertificate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnShowCertificate.Location = new System.Drawing.Point(375, 1);
            this.btnShowCertificate.Margin = new System.Windows.Forms.Padding(1);
            this.btnShowCertificate.Name = "btnShowCertificate";
            this.btnShowCertificate.Size = new System.Drawing.Size(44, 23);
            this.btnShowCertificate.TabIndex = 10;
            this.btnShowCertificate.Text = "Show";
            this.btnShowCertificate.UseVisualStyleBackColor = true;
            // 
            // btnSaveCertificate
            // 
            this.btnSaveCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCertificate.AutoSize = true;
            this.btnSaveCertificate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveCertificate.Location = new System.Drawing.Point(331, 1);
            this.btnSaveCertificate.Margin = new System.Windows.Forms.Padding(1);
            this.btnSaveCertificate.Name = "btnSaveCertificate";
            this.btnSaveCertificate.Size = new System.Drawing.Size(42, 23);
            this.btnSaveCertificate.TabIndex = 10;
            this.btnSaveCertificate.Text = "Save";
            this.btnSaveCertificate.UseVisualStyleBackColor = true;
            // 
            // chkAutoSaveCertificate
            // 
            this.chkAutoSaveCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoSaveCertificate.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoSaveCertificate.AutoSize = true;
            this.chkAutoSaveCertificate.Checked = true;
            this.chkAutoSaveCertificate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSaveCertificate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoSaveCertificate.Location = new System.Drawing.Point(306, 1);
            this.chkAutoSaveCertificate.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoSaveCertificate.Name = "chkAutoSaveCertificate";
            this.chkAutoSaveCertificate.Size = new System.Drawing.Size(23, 23);
            this.chkAutoSaveCertificate.TabIndex = 11;
            this.chkAutoSaveCertificate.Text = ">";
            this.chkAutoSaveCertificate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoSaveCertificate.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1, 27);
            this.label8.Margin = new System.Windows.Forms.Padding(1);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Type";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cmbCertificateType
            // 
            this.cmbCertificateType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.SetColumnSpan(this.cmbCertificateType, 2);
            this.cmbCertificateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCertificateType.FormattingEnabled = true;
            this.cmbCertificateType.Items.AddRange(new object[] {
            "http-01"});
            this.cmbCertificateType.Location = new System.Drawing.Point(56, 23);
            this.cmbCertificateType.Margin = new System.Windows.Forms.Padding(1);
            this.cmbCertificateType.Name = "cmbCertificateType";
            this.cmbCertificateType.Size = new System.Drawing.Size(363, 21);
            this.cmbCertificateType.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1, 49);
            this.label9.Margin = new System.Windows.Forms.Padding(1);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Password";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIssuer
            // 
            this.txtIssuer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.SetColumnSpan(this.txtIssuer, 2);
            this.txtIssuer.Location = new System.Drawing.Point(56, 1);
            this.txtIssuer.Margin = new System.Windows.Forms.Padding(1);
            this.txtIssuer.Name = "txtIssuer";
            this.txtIssuer.ReadOnly = true;
            this.txtIssuer.Size = new System.Drawing.Size(363, 20);
            this.txtIssuer.TabIndex = 13;
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(56, 46);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(1);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(260, 20);
            this.txtPassword.TabIndex = 15;
            // 
            // chkShowPassword
            // 
            this.chkShowPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Location = new System.Drawing.Point(318, 47);
            this.chkShowPassword.Margin = new System.Windows.Forms.Padding(1);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new System.Drawing.Size(101, 17);
            this.chkShowPassword.TabIndex = 16;
            this.chkShowPassword.Text = "Show password";
            this.chkShowPassword.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1, 4);
            this.label10.Margin = new System.Windows.Forms.Padding(1);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Issuer";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkAutoCreateChallenge
            // 
            this.chkAutoCreateChallenge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoCreateChallenge.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoCreateChallenge.AutoSize = true;
            this.chkAutoCreateChallenge.Checked = true;
            this.chkAutoCreateChallenge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoCreateChallenge.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoCreateChallenge.Location = new System.Drawing.Point(517, 66);
            this.chkAutoCreateChallenge.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoCreateChallenge.Name = "chkAutoCreateChallenge";
            this.chkAutoCreateChallenge.Size = new System.Drawing.Size(23, 23);
            this.chkAutoCreateChallenge.TabIndex = 11;
            this.chkAutoCreateChallenge.Text = ">";
            this.chkAutoCreateChallenge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoCreateChallenge.UseVisualStyleBackColor = true;
            // 
            // chkAutoCreateCertificate
            // 
            this.chkAutoCreateCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoCreateCertificate.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoCreateCertificate.AutoSize = true;
            this.chkAutoCreateCertificate.Checked = true;
            this.chkAutoCreateCertificate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoCreateCertificate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoCreateCertificate.Location = new System.Drawing.Point(517, 185);
            this.chkAutoCreateCertificate.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoCreateCertificate.Name = "chkAutoCreateCertificate";
            this.chkAutoCreateCertificate.Size = new System.Drawing.Size(23, 23);
            this.chkAutoCreateCertificate.TabIndex = 11;
            this.chkAutoCreateCertificate.Text = ">";
            this.chkAutoCreateCertificate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoCreateCertificate.UseVisualStyleBackColor = true;
            // 
            // chkAutoSubmit
            // 
            this.chkAutoSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoSubmit.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkAutoSubmit.AutoSize = true;
            this.chkAutoSubmit.Checked = true;
            this.chkAutoSubmit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSubmit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoSubmit.Location = new System.Drawing.Point(517, 210);
            this.chkAutoSubmit.Margin = new System.Windows.Forms.Padding(1);
            this.chkAutoSubmit.Name = "chkAutoSubmit";
            this.chkAutoSubmit.Size = new System.Drawing.Size(23, 23);
            this.chkAutoSubmit.TabIndex = 11;
            this.chkAutoSubmit.Text = ">";
            this.chkAutoSubmit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAutoSubmit.UseVisualStyleBackColor = true;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.AutoSize = true;
            this.btnSubmit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSubmit.Location = new System.Drawing.Point(542, 210);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(1);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(49, 23);
            this.btnSubmit.TabIndex = 10;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            // 
            // btnRegister
            // 
            this.btnRegister.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegister.AutoSize = true;
            this.btnRegister.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.btnRegister, 2);
            this.btnRegister.Location = new System.Drawing.Point(517, 1);
            this.btnRegister.Margin = new System.Windows.Forms.Padding(1);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(74, 23);
            this.btnRegister.TabIndex = 2;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.lnkTOS, 0, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(96, 25);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(420, 15);
            this.tableLayoutPanel6.TabIndex = 17;
            // 
            // lnkTOS
            // 
            this.lnkTOS.AutoSize = true;
            this.lnkTOS.Location = new System.Drawing.Point(1, 1);
            this.lnkTOS.Margin = new System.Windows.Forms.Padding(1);
            this.lnkTOS.Name = "lnkTOS";
            this.lnkTOS.Size = new System.Drawing.Size(85, 13);
            this.lnkTOS.TabIndex = 16;
            this.lnkTOS.TabStop = true;
            this.lnkTOS.Text = "Terms of service";
            this.lnkTOS.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTOS_LinkClicked);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.richTextBox1, 4);
            this.richTextBox1.Location = new System.Drawing.Point(3, 329);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(586, 59);
            this.richTextBox1.TabIndex = 14;
            this.richTextBox1.Text = "";
            // 
            // cmbRegistration
            // 
            this.cmbRegistration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRegistration.FormattingEnabled = true;
            this.cmbRegistration.Location = new System.Drawing.Point(97, 2);
            this.cmbRegistration.Margin = new System.Windows.Forms.Padding(1);
            this.cmbRegistration.Name = "cmbRegistration";
            this.cmbRegistration.Size = new System.Drawing.Size(418, 21);
            this.cmbRegistration.TabIndex = 4;
            this.cmbRegistration.Text = "masbicudo@gmail.com";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(598, 397);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Manager";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 3;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.lstRegistrations, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.label12, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.lstDomains, 1, 1);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(586, 385);
            this.tableLayoutPanel7.TabIndex = 1;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.lstCertificates, 0, 3);
            this.tableLayoutPanel8.Controls.Add(this.label14, 0, 2);
            this.tableLayoutPanel8.Controls.Add(this.label13, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.lstChallenges, 0, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(439, 0);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 4;
            this.tableLayoutPanel7.SetRowSpan(this.tableLayoutPanel8, 2);
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(147, 385);
            this.tableLayoutPanel8.TabIndex = 3;
            // 
            // lstCertificates
            // 
            this.lstCertificates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCertificates.FormattingEnabled = true;
            this.lstCertificates.IntegralHeight = false;
            this.lstCertificates.Location = new System.Drawing.Point(3, 208);
            this.lstCertificates.Name = "lstCertificates";
            this.lstCertificates.Size = new System.Drawing.Size(141, 174);
            this.lstCertificates.TabIndex = 3;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 192);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(141, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "Certificates";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(141, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Challenges";
            // 
            // lstChallenges
            // 
            this.lstChallenges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstChallenges.FormattingEnabled = true;
            this.lstChallenges.IntegralHeight = false;
            this.lstChallenges.Location = new System.Drawing.Point(3, 16);
            this.lstChallenges.Name = "lstChallenges";
            this.lstChallenges.Size = new System.Drawing.Size(141, 173);
            this.lstChallenges.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(140, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Registrations";
            // 
            // lstRegistrations
            // 
            this.lstRegistrations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstRegistrations.FormattingEnabled = true;
            this.lstRegistrations.IntegralHeight = false;
            this.lstRegistrations.Location = new System.Drawing.Point(3, 16);
            this.lstRegistrations.Name = "lstRegistrations";
            this.lstRegistrations.Size = new System.Drawing.Size(140, 366);
            this.lstRegistrations.TabIndex = 0;
            this.lstRegistrations.SelectedIndexChanged += new System.EventHandler(this.lstRegistrations_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(149, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(287, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Domains";
            // 
            // lstDomains
            // 
            this.lstDomains.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDomains.FormattingEnabled = true;
            this.lstDomains.IntegralHeight = false;
            this.lstDomains.Location = new System.Drawing.Point(149, 16);
            this.lstDomains.Name = "lstDomains";
            this.lstDomains.Size = new System.Drawing.Size(287, 366);
            this.lstDomains.TabIndex = 0;
            this.lstDomains.SelectedIndexChanged += new System.EventHandler(this.lstDomains_SelectedIndexChanged);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "Form2";
            this.Text = "LetsEncrypt Certification Wizard";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCreateChallenge;
        private System.Windows.Forms.ComboBox cmbChallenge;
        private System.Windows.Forms.Button btnAddDomain;
        private System.Windows.Forms.ComboBox cmbDomain;
        private System.Windows.Forms.CheckBox chkAutoAddDomain;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnChallengeFile;
        private System.Windows.Forms.TextBox txtChallengeKey;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtChallengeTarget;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtChallengeFile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox chkAutoSaveChallenge;
        private System.Windows.Forms.Button btnSaveChallenge;
        private System.Windows.Forms.Button btnCommitChallenge;
        private System.Windows.Forms.Button btnTestChallenge;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.CheckBox chkAutoCommit;
        private System.Windows.Forms.CheckBox chkAutoTest;
        private System.Windows.Forms.CheckBox chkAutoValidate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbCertificate;
        private System.Windows.Forms.Button btnCreateCertificate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Button btnShowCertificate;
        private System.Windows.Forms.Button btnSaveCertificate;
        private System.Windows.Forms.CheckBox chkAutoSaveCertificate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbCertificateType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtIssuer;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkShowPassword;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkAutoCreateChallenge;
        private System.Windows.Forms.CheckBox chkAutoCreateCertificate;
        private System.Windows.Forms.CheckBox chkAutoSubmit;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.LinkLabel lnkTOS;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListBox lstRegistrations;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox lstDomains;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ListBox lstChallenges;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.ListBox lstCertificates;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cmbRegistration;
    }
}