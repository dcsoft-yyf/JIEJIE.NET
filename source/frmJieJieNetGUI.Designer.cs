namespace JIEJIE
{
    partial class frmJieJieNetGUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmJieJieNetGUI));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSaveConfig = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCopySelectedText = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRun = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAbout = new System.Windows.Forms.ToolStripButton();
            this.ctlProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkDeleteTempFile = new System.Windows.Forms.CheckBox();
            this.chkTestUseNGen = new System.Windows.Forms.CheckBox();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.btnBrowseOutputPath = new System.Windows.Forms.Button();
            this.btnBrowseSnkFile = new System.Windows.Forms.Button();
            this.txtSNKFile = new System.Windows.Forms.TextBox();
            this.chkOutputMapXml = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInputAssembly = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkEncryptStrings = new System.Windows.Forms.CheckBox();
            this.chkObfuscateControlFlow = new System.Windows.Forms.CheckBox();
            this.chkDebugMode = new System.Windows.Forms.CheckBox();
            this.chkMergeAssemblies = new System.Windows.Forms.CheckBox();
            this.chkRemoveMember = new System.Windows.Forms.CheckBox();
            this.btnBrowseSDKPath = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.chkObfuscateMemberOrder = new System.Windows.Forms.CheckBox();
            this.txtSDKPath = new System.Windows.Forms.TextBox();
            this.chkRename = new System.Windows.Forms.CheckBox();
            this.chkEncryptResources = new System.Windows.Forms.CheckBox();
            this.groupRename = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPrefixForTypeRename = new System.Windows.Forms.TextBox();
            this.txtPrefixForMemberRename = new System.Windows.Forms.TextBox();
            this.btnBrowseInputAssembly = new System.Windows.Forms.Button();
            this.grpEncryptResources = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtResourceNameNeedEncrypt = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtTranslateResult = new System.Windows.Forms.RichTextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCopySelectedTextForStackTrace = new System.Windows.Forms.Button();
            this.btnTranslateStackTrace = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCallStackTrace = new System.Windows.Forms.TextBox();
            this.btnBrowseMapXmlFile = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.btnLoadConfig = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnLoadConfigFileUseBrowse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.txtMapXmlFile = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupRename.SuspendLayout();
            this.grpEncryptResources.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(793, 723);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtLog);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(785, 697);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Obfuscation";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLog.ForeColor = System.Drawing.Color.White;
            this.txtLog.HideSelection = false;
            this.txtLog.Location = new System.Drawing.Point(3, 356);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(779, 338);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtInputAssembly);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.chkRename);
            this.panel1.Controls.Add(this.chkEncryptResources);
            this.panel1.Controls.Add(this.groupRename);
            this.panel1.Controls.Add(this.btnBrowseInputAssembly);
            this.panel1.Controls.Add(this.grpEncryptResources);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(779, 353);
            this.panel1.TabIndex = 8;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoadConfig,
            this.btnSaveConfig,
            this.toolStripSeparator1,
            this.btnCopySelectedText,
            this.toolStripSeparator2,
            this.btnRun,
            this.btnStop,
            this.toolStripSeparator3,
            this.btnAbout,
            this.ctlProgress});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.ShowItemToolTips = false;
            this.toolStrip1.Size = new System.Drawing.Size(779, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveConfig.Image")));
            this.btnSaveConfig.ImageTransparentColor = System.Drawing.Color.Red;
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(125, 22);
            this.btnSaveConfig.Text = "Save config file...";
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfigFile_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCopySelectedText
            // 
            this.btnCopySelectedText.Image = ((System.Drawing.Image)(resources.GetObject("btnCopySelectedText.Image")));
            this.btnCopySelectedText.ImageTransparentColor = System.Drawing.Color.Red;
            this.btnCopySelectedText.Name = "btnCopySelectedText";
            this.btnCopySelectedText.Size = new System.Drawing.Size(135, 22);
            this.btnCopySelectedText.Text = "Copy selected text";
            this.btnCopySelectedText.Click += new System.EventHandler(this.btnCopySelectedText_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRun
            // 
            this.btnRun.Image = ((System.Drawing.Image)(resources.GetObject("btnRun.Image")));
            this.btnRun.ImageTransparentColor = System.Drawing.Color.White;
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(50, 22);
            this.btnRun.Text = "Run";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(55, 22);
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAbout
            // 
            this.btnAbout.Image = ((System.Drawing.Image)(resources.GetObject("btnAbout.Image")));
            this.btnAbout.ImageTransparentColor = System.Drawing.Color.White;
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(63, 22);
            this.btnAbout.Text = "About";
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // ctlProgress
            // 
            this.ctlProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ctlProgress.Name = "ctlProgress";
            this.ctlProgress.Size = new System.Drawing.Size(150, 22);
            this.ctlProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ctlProgress.Value = 50;
            this.ctlProgress.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkDeleteTempFile);
            this.groupBox2.Controls.Add(this.chkTestUseNGen);
            this.groupBox2.Controls.Add(this.txtOutputPath);
            this.groupBox2.Controls.Add(this.btnBrowseOutputPath);
            this.groupBox2.Controls.Add(this.btnBrowseSnkFile);
            this.groupBox2.Controls.Add(this.txtSNKFile);
            this.groupBox2.Controls.Add(this.chkOutputMapXml);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(9, 224);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(765, 122);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output";
            // 
            // chkDeleteTempFile
            // 
            this.chkDeleteTempFile.AutoSize = true;
            this.chkDeleteTempFile.Location = new System.Drawing.Point(17, 94);
            this.chkDeleteTempFile.Name = "chkDeleteTempFile";
            this.chkDeleteTempFile.Size = new System.Drawing.Size(126, 16);
            this.chkDeleteTempFile.TabIndex = 7;
            this.chkDeleteTempFile.Text = "Delete temp files";
            this.chkDeleteTempFile.UseVisualStyleBackColor = true;
            // 
            // chkTestUseNGen
            // 
            this.chkTestUseNGen.AutoSize = true;
            this.chkTestUseNGen.Location = new System.Drawing.Point(268, 94);
            this.chkTestUseNGen.Name = "chkTestUseNGen";
            this.chkTestUseNGen.Size = new System.Drawing.Size(270, 16);
            this.chkTestUseNGen.TabIndex = 8;
            this.chkTestUseNGen.Text = "Test result file by ngen.exe/crossgen.exe";
            this.chkTestUseNGen.UseVisualStyleBackColor = true;
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputPath.Location = new System.Drawing.Point(104, 20);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(574, 21);
            this.txtOutputPath.TabIndex = 1;
            // 
            // btnBrowseOutputPath
            // 
            this.btnBrowseOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutputPath.Location = new System.Drawing.Point(684, 19);
            this.btnBrowseOutputPath.Name = "btnBrowseOutputPath";
            this.btnBrowseOutputPath.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOutputPath.TabIndex = 2;
            this.btnBrowseOutputPath.Text = "Browse...";
            this.btnBrowseOutputPath.UseVisualStyleBackColor = true;
            this.btnBrowseOutputPath.Click += new System.EventHandler(this.btnBrowseOutputPath_Click);
            // 
            // btnBrowseSnkFile
            // 
            this.btnBrowseSnkFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSnkFile.Location = new System.Drawing.Point(684, 44);
            this.btnBrowseSnkFile.Name = "btnBrowseSnkFile";
            this.btnBrowseSnkFile.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseSnkFile.TabIndex = 5;
            this.btnBrowseSnkFile.Text = "Browse...";
            this.btnBrowseSnkFile.UseVisualStyleBackColor = true;
            this.btnBrowseSnkFile.Click += new System.EventHandler(this.btnBrowseSnkFile_Click);
            // 
            // txtSNKFile
            // 
            this.txtSNKFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSNKFile.Location = new System.Drawing.Point(104, 45);
            this.txtSNKFile.Name = "txtSNKFile";
            this.txtSNKFile.Size = new System.Drawing.Size(574, 21);
            this.txtSNKFile.TabIndex = 4;
            // 
            // chkOutputMapXml
            // 
            this.chkOutputMapXml.AutoSize = true;
            this.chkOutputMapXml.Location = new System.Drawing.Point(17, 72);
            this.chkOutputMapXml.Name = "chkOutputMapXml";
            this.chkOutputMapXml.Size = new System.Drawing.Size(168, 16);
            this.chkOutputMapXml.TabIndex = 6;
            this.chkOutputMapXml.Text = "Save rename map XML file";
            this.chkOutputMapXml.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Output path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "SNK file:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input assembly:";
            // 
            // txtInputAssembly
            // 
            this.txtInputAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputAssembly.Location = new System.Drawing.Point(108, 31);
            this.txtInputAssembly.Name = "txtInputAssembly";
            this.txtInputAssembly.Size = new System.Drawing.Size(579, 21);
            this.txtInputAssembly.TabIndex = 2;
            this.txtInputAssembly.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInputAssembly_KeyDown);
            this.txtInputAssembly.Leave += new System.EventHandler(this.txtInputAssembly_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cboLanguage);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.chkEncryptStrings);
            this.groupBox1.Controls.Add(this.chkObfuscateControlFlow);
            this.groupBox1.Controls.Add(this.chkDebugMode);
            this.groupBox1.Controls.Add(this.chkMergeAssemblies);
            this.groupBox1.Controls.Add(this.chkRemoveMember);
            this.groupBox1.Controls.Add(this.btnBrowseSDKPath);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkObfuscateMemberOrder);
            this.groupBox1.Controls.Add(this.txtSDKPath);
            this.groupBox1.Location = new System.Drawing.Point(330, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 157);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Location = new System.Drawing.Point(96, 130);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(272, 20);
            this.cboLanguage.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "UI language:";
            // 
            // chkEncryptStrings
            // 
            this.chkEncryptStrings.AutoSize = true;
            this.chkEncryptStrings.Location = new System.Drawing.Point(13, 48);
            this.chkEncryptStrings.Name = "chkEncryptStrings";
            this.chkEncryptStrings.Size = new System.Drawing.Size(108, 16);
            this.chkEncryptStrings.TabIndex = 3;
            this.chkEncryptStrings.Text = "Encrypt string";
            this.chkEncryptStrings.UseVisualStyleBackColor = true;
            // 
            // chkObfuscateControlFlow
            // 
            this.chkObfuscateControlFlow.AutoSize = true;
            this.chkObfuscateControlFlow.Location = new System.Drawing.Point(212, 48);
            this.chkObfuscateControlFlow.Name = "chkObfuscateControlFlow";
            this.chkObfuscateControlFlow.Size = new System.Drawing.Size(156, 16);
            this.chkObfuscateControlFlow.TabIndex = 4;
            this.chkObfuscateControlFlow.Text = "Obfuscate control flow";
            this.chkObfuscateControlFlow.UseVisualStyleBackColor = true;
            // 
            // chkDebugMode
            // 
            this.chkDebugMode.AutoSize = true;
            this.chkDebugMode.Location = new System.Drawing.Point(212, 92);
            this.chkDebugMode.Name = "chkDebugMode";
            this.chkDebugMode.Size = new System.Drawing.Size(84, 16);
            this.chkDebugMode.TabIndex = 8;
            this.chkDebugMode.Text = "Debug mode";
            this.chkDebugMode.UseVisualStyleBackColor = true;
            // 
            // chkMergeAssemblies
            // 
            this.chkMergeAssemblies.AutoSize = true;
            this.chkMergeAssemblies.Location = new System.Drawing.Point(13, 92);
            this.chkMergeAssemblies.Name = "chkMergeAssemblies";
            this.chkMergeAssemblies.Size = new System.Drawing.Size(186, 16);
            this.chkMergeAssemblies.TabIndex = 7;
            this.chkMergeAssemblies.Text = "Merge referenced assemblies";
            this.chkMergeAssemblies.UseVisualStyleBackColor = true;
            // 
            // chkRemoveMember
            // 
            this.chkRemoveMember.AutoSize = true;
            this.chkRemoveMember.Location = new System.Drawing.Point(212, 70);
            this.chkRemoveMember.Name = "chkRemoveMember";
            this.chkRemoveMember.Size = new System.Drawing.Size(102, 16);
            this.chkRemoveMember.TabIndex = 6;
            this.chkRemoveMember.Text = "Remove member";
            this.chkRemoveMember.UseVisualStyleBackColor = true;
            // 
            // btnBrowseSDKPath
            // 
            this.btnBrowseSDKPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSDKPath.Location = new System.Drawing.Point(363, 20);
            this.btnBrowseSDKPath.Name = "btnBrowseSDKPath";
            this.btnBrowseSDKPath.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseSDKPath.TabIndex = 2;
            this.btnBrowseSDKPath.Text = "Browse...";
            this.btnBrowseSDKPath.UseVisualStyleBackColor = true;
            this.btnBrowseSDKPath.Click += new System.EventHandler(this.btnBrowseSDKPath_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "SDK Path:";
            // 
            // chkObfuscateMemberOrder
            // 
            this.chkObfuscateMemberOrder.AutoSize = true;
            this.chkObfuscateMemberOrder.Location = new System.Drawing.Point(13, 70);
            this.chkObfuscateMemberOrder.Name = "chkObfuscateMemberOrder";
            this.chkObfuscateMemberOrder.Size = new System.Drawing.Size(156, 16);
            this.chkObfuscateMemberOrder.TabIndex = 5;
            this.chkObfuscateMemberOrder.Text = "Obfuscate member order";
            this.chkObfuscateMemberOrder.UseVisualStyleBackColor = true;
            // 
            // txtSDKPath
            // 
            this.txtSDKPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSDKPath.Location = new System.Drawing.Point(78, 20);
            this.txtSDKPath.Name = "txtSDKPath";
            this.txtSDKPath.Size = new System.Drawing.Size(279, 21);
            this.txtSDKPath.TabIndex = 1;
            // 
            // chkRename
            // 
            this.chkRename.AutoSize = true;
            this.chkRename.Location = new System.Drawing.Point(17, 58);
            this.chkRename.Name = "chkRename";
            this.chkRename.Size = new System.Drawing.Size(60, 16);
            this.chkRename.TabIndex = 4;
            this.chkRename.Text = "Rename";
            this.chkRename.UseVisualStyleBackColor = true;
            // 
            // chkEncryptResources
            // 
            this.chkEncryptResources.AutoSize = true;
            this.chkEncryptResources.Location = new System.Drawing.Point(17, 145);
            this.chkEncryptResources.Name = "chkEncryptResources";
            this.chkEncryptResources.Size = new System.Drawing.Size(126, 16);
            this.chkEncryptResources.TabIndex = 6;
            this.chkEncryptResources.Text = "Encrypt resources";
            this.chkEncryptResources.UseVisualStyleBackColor = true;
            // 
            // groupRename
            // 
            this.groupRename.Controls.Add(this.label5);
            this.groupRename.Controls.Add(this.label6);
            this.groupRename.Controls.Add(this.txtPrefixForTypeRename);
            this.groupRename.Controls.Add(this.txtPrefixForMemberRename);
            this.groupRename.Location = new System.Drawing.Point(9, 60);
            this.groupRename.Name = "groupRename";
            this.groupRename.Size = new System.Drawing.Size(315, 79);
            this.groupRename.TabIndex = 5;
            this.groupRename.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Prefix for type rename:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(155, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "Prefix for member rename:";
            // 
            // txtPrefixForTypeRename
            // 
            this.txtPrefixForTypeRename.Location = new System.Drawing.Point(183, 20);
            this.txtPrefixForTypeRename.Name = "txtPrefixForTypeRename";
            this.txtPrefixForTypeRename.Size = new System.Drawing.Size(122, 21);
            this.txtPrefixForTypeRename.TabIndex = 1;
            // 
            // txtPrefixForMemberRename
            // 
            this.txtPrefixForMemberRename.Location = new System.Drawing.Point(183, 45);
            this.txtPrefixForMemberRename.Name = "txtPrefixForMemberRename";
            this.txtPrefixForMemberRename.Size = new System.Drawing.Size(122, 21);
            this.txtPrefixForMemberRename.TabIndex = 3;
            // 
            // btnBrowseInputAssembly
            // 
            this.btnBrowseInputAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseInputAssembly.Location = new System.Drawing.Point(693, 31);
            this.btnBrowseInputAssembly.Name = "btnBrowseInputAssembly";
            this.btnBrowseInputAssembly.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseInputAssembly.TabIndex = 3;
            this.btnBrowseInputAssembly.Text = "Browse...";
            this.btnBrowseInputAssembly.UseVisualStyleBackColor = true;
            this.btnBrowseInputAssembly.Click += new System.EventHandler(this.btnBrowseInputAssembly_Click);
            // 
            // grpEncryptResources
            // 
            this.grpEncryptResources.Controls.Add(this.label8);
            this.grpEncryptResources.Controls.Add(this.txtResourceNameNeedEncrypt);
            this.grpEncryptResources.Location = new System.Drawing.Point(9, 145);
            this.grpEncryptResources.Name = "grpEncryptResources";
            this.grpEncryptResources.Size = new System.Drawing.Size(315, 72);
            this.grpEncryptResources.TabIndex = 7;
            this.grpEncryptResources.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(215, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "Specify resource name need encrypt:";
            // 
            // txtResourceNameNeedEncrypt
            // 
            this.txtResourceNameNeedEncrypt.Location = new System.Drawing.Point(17, 45);
            this.txtResourceNameNeedEncrypt.Name = "txtResourceNameNeedEncrypt";
            this.txtResourceNameNeedEncrypt.Size = new System.Drawing.Size(288, 21);
            this.txtResourceNameNeedEncrypt.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtTranslateResult);
            this.tabPage2.Controls.Add(this.splitter1);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(810, 697);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Translate stack trace";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtTranslateResult
            // 
            this.txtTranslateResult.BackColor = System.Drawing.Color.Black;
            this.txtTranslateResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTranslateResult.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtTranslateResult.ForeColor = System.Drawing.Color.White;
            this.txtTranslateResult.Location = new System.Drawing.Point(3, 290);
            this.txtTranslateResult.Name = "txtTranslateResult";
            this.txtTranslateResult.ReadOnly = true;
            this.txtTranslateResult.Size = new System.Drawing.Size(804, 404);
            this.txtTranslateResult.TabIndex = 10;
            this.txtTranslateResult.Text = "";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(3, 287);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(804, 3);
            this.splitter1.TabIndex = 9;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtMapXmlFile);
            this.panel2.Controls.Add(this.btnCopySelectedTextForStackTrace);
            this.panel2.Controls.Add(this.btnTranslateStackTrace);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.txtCallStackTrace);
            this.panel2.Controls.Add(this.btnBrowseMapXmlFile);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(804, 284);
            this.panel2.TabIndex = 8;
            // 
            // btnCopySelectedTextForStackTrace
            // 
            this.btnCopySelectedTextForStackTrace.Location = new System.Drawing.Point(134, 255);
            this.btnCopySelectedTextForStackTrace.Name = "btnCopySelectedTextForStackTrace";
            this.btnCopySelectedTextForStackTrace.Size = new System.Drawing.Size(192, 23);
            this.btnCopySelectedTextForStackTrace.TabIndex = 9;
            this.btnCopySelectedTextForStackTrace.Text = "Copy selected text";
            this.btnCopySelectedTextForStackTrace.UseVisualStyleBackColor = true;
            this.btnCopySelectedTextForStackTrace.Click += new System.EventHandler(this.btnCopySelectedTextForStackTrace_Click);
            // 
            // btnTranslateStackTrace
            // 
            this.btnTranslateStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTranslateStackTrace.Location = new System.Drawing.Point(5, 255);
            this.btnTranslateStackTrace.Name = "btnTranslateStackTrace";
            this.btnTranslateStackTrace.Size = new System.Drawing.Size(123, 23);
            this.btnTranslateStackTrace.TabIndex = 8;
            this.btnTranslateStackTrace.Text = "Translate";
            this.btnTranslateStackTrace.UseVisualStyleBackColor = true;
            this.btnTranslateStackTrace.Click += new System.EventHandler(this.btnTranslateStackTrace_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "Map xml file:";
            // 
            // txtCallStackTrace
            // 
            this.txtCallStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCallStackTrace.Location = new System.Drawing.Point(5, 69);
            this.txtCallStackTrace.Multiline = true;
            this.txtCallStackTrace.Name = "txtCallStackTrace";
            this.txtCallStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCallStackTrace.Size = new System.Drawing.Size(790, 180);
            this.txtCallStackTrace.TabIndex = 7;
            this.txtCallStackTrace.WordWrap = false;
            // 
            // btnBrowseMapXmlFile
            // 
            this.btnBrowseMapXmlFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseMapXmlFile.Location = new System.Drawing.Point(720, 13);
            this.btnBrowseMapXmlFile.Name = "btnBrowseMapXmlFile";
            this.btnBrowseMapXmlFile.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseMapXmlFile.TabIndex = 5;
            this.btnBrowseMapXmlFile.Text = "Browse...";
            this.btnBrowseMapXmlFile.UseVisualStyleBackColor = true;
            this.btnBrowseMapXmlFile.Click += new System.EventHandler(this.btnBrowseMapXmlFile_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 12);
            this.label10.TabIndex = 6;
            this.label10.Text = "Call stack trace";
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoadConfigFileUseBrowse,
            this.toolStripSeparator4});
            this.btnLoadConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadConfig.Image")));
            this.btnLoadConfig.ImageTransparentColor = System.Drawing.Color.Red;
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(127, 22);
            this.btnLoadConfig.Text = "Load config file";
            // 
            // btnLoadConfigFileUseBrowse
            // 
            this.btnLoadConfigFileUseBrowse.Name = "btnLoadConfigFileUseBrowse";
            this.btnLoadConfigFileUseBrowse.Size = new System.Drawing.Size(180, 22);
            this.btnLoadConfigFileUseBrowse.Text = "Browse...";
            this.btnLoadConfigFileUseBrowse.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(177, 6);
            // 
            // txtMapXmlFile
            // 
            this.txtMapXmlFile.FormattingEnabled = true;
            this.txtMapXmlFile.Location = new System.Drawing.Point(119, 14);
            this.txtMapXmlFile.Name = "txtMapXmlFile";
            this.txtMapXmlFile.Size = new System.Drawing.Size(595, 20);
            this.txtMapXmlFile.TabIndex = 10;
            // 
            // frmJieJieNetGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 723);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmJieJieNetGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JIEJIE.NET from https://github.com/dcsoft-yyf/JIEJIE.NET";
            this.Load += new System.EventHandler(this.frmJieJieNetGUI_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupRename.ResumeLayout(false);
            this.groupRename.PerformLayout();
            this.grpEncryptResources.ResumeLayout(false);
            this.grpEncryptResources.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnBrowseInputAssembly;
        private System.Windows.Forms.TextBox txtInputAssembly;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseOutputPath;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowseSnkFile;
        private System.Windows.Forms.TextBox txtSNKFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowseSDKPath;
        private System.Windows.Forms.TextBox txtSDKPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPrefixForMemberRename;
        private System.Windows.Forms.TextBox txtPrefixForTypeRename;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkEncryptStrings;
        private System.Windows.Forms.CheckBox chkObfuscateControlFlow;
        private System.Windows.Forms.CheckBox chkEncryptResources;
        private System.Windows.Forms.CheckBox chkObfuscateMemberOrder;
        private System.Windows.Forms.CheckBox chkRename;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkRemoveMember;
        private System.Windows.Forms.GroupBox groupRename;
        private System.Windows.Forms.CheckBox chkMergeAssemblies;
        private System.Windows.Forms.CheckBox chkDebugMode;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox grpEncryptResources;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtResourceNameNeedEncrypt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkTestUseNGen;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnBrowseMapXmlFile;
        private System.Windows.Forms.RichTextBox txtTranslateResult;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnTranslateStackTrace;
        private System.Windows.Forms.TextBox txtCallStackTrace;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkDeleteTempFile;
        private System.Windows.Forms.CheckBox chkOutputMapXml;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSaveConfig;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnCopySelectedText;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnRun;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripProgressBar ctlProgress;
        private System.Windows.Forms.ToolStripButton btnAbout;
        private System.Windows.Forms.Button btnCopySelectedTextForStackTrace;
        private System.Windows.Forms.ToolStripDropDownButton btnLoadConfig;
        private System.Windows.Forms.ToolStripMenuItem btnLoadConfigFileUseBrowse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ComboBox txtMapXmlFile;
    }
}