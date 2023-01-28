using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

[assembly: System.Reflection.AssemblyTitle("JieJie.NET GUI Application")]

namespace JIEJIE
{
    /// <summary>
    /// JIEJIE.NET的GUI窗口对象
    /// </summary>
    public partial class frmJieJieNetGUI : Form
    {
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmJieJieNetGUI());
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();


        public frmJieJieNetGUI()
        {
            InitializeComponent();
        }

        private JieJieProject _Project = new JieJieProject ();

        private void frmJieJieNetGUI_Load(object sender, EventArgs e)
        {
            RefreshConfigView();
            RecencFileNameList.Instance.LoadConfigFile();

            RefreshLoadFileMenus();
            txtMapXmlFile.Items.Clear();
            txtMapXmlFile.Items.AddRange(
                RecencFileNameList.Instance.GetRecenFileNames("Map.xml").ToArray());
        }

        private void RefreshLoadFileMenus()
        {
            for(int iCount = this.btnLoadConfig.DropDownItems.Count - 1; iCount >= 2; iCount --)
            {
                this.btnLoadConfig.DropDownItems.RemoveAt(iCount);
            }
            var fns = RecencFileNameList.Instance.GetRecenFileNames("LoadConfig");
            if( fns.Count == 0 )
            {
                this.btnLoadConfig.DropDownItems.Add("No recent files").Enabled = false ;
            }
            else
            {
                foreach( var fn in fns )
                {
                    var menuItem = new System.Windows.Forms.ToolStripMenuItem(fn);
                    this.btnLoadConfig.DropDownItems.Add(menuItem);
                    menuItem.Click += MenuItem_Click;
                }
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            var fn = ((System.Windows.Forms.ToolStripMenuItem)sender).Text;
            if (fn != null && fn.Length > 0 && File.Exists(fn))
            {
                LoadProjectFile(fn);
            }
        }

        /// <summary>
        /// 最近使用的文件名列表
        /// </summary>
        private class RecencFileNameList
        {
            public static readonly RecencFileNameList Instance = new RecencFileNameList();

            public bool Modified = false;

            private Dictionary<string, List<string>> _Values = new Dictionary<string, List<string>>();
            public List<string> GetRecenFileNames(string groupName )
            {
                if (groupName == null || groupName.Length == 0)
                {
                    throw new ArgumentNullException("groupName");
                }
                List<string> list = null;
                if( _Values.TryGetValue( groupName , out list ) == false )
                {
                    list = new List<string>();
                    _Values[groupName] = list;
                }
                return list;
            }
            public bool AddItem(string groupName , string fileName )
            {
                if(groupName == null || groupName.Length == 0 )
                {
                    throw new ArgumentNullException("groupName");
                }
                if( fileName == null ||fileName.Length == 0 )
                {
                    throw new ArgumentNullException("fileName");
                }
                var list = GetRecenFileNames(groupName);
                int index = list.IndexOf(fileName);
                if(index > 0 )
                {
                    list.RemoveAt(index);
                    list.Insert(0 ,fileName);
                    this.Modified = true;
                    return true;
                }
                else if(index < 0 )
                {
                    list.Insert(0, fileName);
                    this.Modified = true;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// 配置文件名
            /// </summary>
            private static readonly string CfgFileName = Path.Combine(
                Path.GetTempPath(), 
                Path.GetFileName(Application.ExecutablePath) + ".recentfiles.txt");
            /// <summary>
            /// 保存配置文件
            /// </summary>
            public void SaveConfigFile()
            {
                this.Modified = false;
                using (var writer = new System.IO.StreamWriter(CfgFileName, false, Encoding.UTF8))
                {
                    foreach (var item in this._Values)
                    {
                        writer.WriteLine("[" + item.Key + "]");
                        int index = 0;
                        foreach (var fn in item.Value)
                        {
                            writer.WriteLine("FileName" + Convert.ToString(index++) + "=" + fn);
                        }
                    }
                }
            }
            /// <summary>
            /// 加载配置文件
            /// </summary>
            public void LoadConfigFile()
            {
                this.Modified = false;
                this._Values.Clear();
                if( File.Exists( CfgFileName ))
                {
                    List<string> list = null;
                    using (var reader = new System.IO.StreamReader(CfgFileName, Encoding.UTF8, true))
                    {
                        string line = reader.ReadLine();
                        while(line != null )
                        {
                            line = line.Trim();
                            if (line.Length > 2 )
                            {
                                if (line[0] == '[' && line[line.Length - 1] == ']')
                                {
                                    var gn = line.Substring(1, line.Length - 2);
                                    list = new List<string>();
                                    this._Values[gn] = list;
                                }
                                else if ( list != null && line.StartsWith("FileName", StringComparison.Ordinal))
                                {
                                    int index = line.IndexOf('=');
                                    if (index > 0)
                                    {
                                        string fn = line.Substring(index + 1);
                                        if(fn.Length > 0 )
                                        {
                                            list.Add(fn);
                                        }
                                    }
                                }
                            }
                            line = reader.ReadLine();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 使用一个 RichTextBox来模拟命令行输出界面
        /// </summary>
        private class RichTextBoxConsole : JIEJIE.MyConsole, IDisposable
        {
            private delegate void VoidHandler();
            private System.Windows.Forms.RichTextBox _box = null;
            public RichTextBoxConsole(System.Windows.Forms.RichTextBox box)
            {
                if (box == null)
                {
                    throw new ArgumentNullException("box");
                }
                this._box = box;
                this._box.BackColor = base.ToColor(ConsoleColor.Black);
                this._box.Font = new Font(System.Windows.Forms.Control.DefaultFont.Name, 12);
                this._box.HideSelection = false;
                this._box.ShowSelectionMargin = true;

            }
            public override bool SupportKeyboardInput
            {
                get
                {
                    return false;
                }
            }
            public void Dispose()
            {
                this._box = null;
            }
            private ConsoleColor _BackColor = ConsoleColor.Black;

            public override ConsoleColor BackgroundColor
            {
                get
                {
                    return this._BackColor;
                }
                set
                {
                    this._BackColor = value;
                }
            }
            private ConsoleColor _ForeColor = ConsoleColor.White;
            public override ConsoleColor ForegroundColor
            {
                get
                {
                    return this._ForeColor;
                }
                set
                {
                    this._ForeColor = value;
                }
            }
            public override void ResetColor()
            {
                this._BackColor = ConsoleColor.Black;
                this._ForeColor = ConsoleColor.White;
            }

            public override string Title
            {
                get
                {
                    string result = null;
                    if (this._box.InvokeRequired)
                    {
                        this._box.Invoke(new VoidHandler(delegate ()
                        {
                            result = this._box.FindForm()?.Text;
                        }));
                    }
                    else
                    {
                        result = this._box.FindForm()?.Text;
                    }
                    return result;
                }
                set
                {
                    if (this._box.InvokeRequired)
                    {
                        this._box.Invoke(new VoidHandler(delegate ()
                        {
                            var frm = this._box.FindForm();
                            if (frm != null)
                            {
                                frm.Text = value;
                            }
                        }));
                    }
                    else
                    {
                        var frm = this._box.FindForm();
                        if (frm != null)
                        {
                            frm.Text = value;
                        }
                    }
                }
            }
            public override bool EnsureNewLine()
            {
                if (this._IsNewLine)
                {
                    return false;
                }
                else
                {
                    this.Write(Environment.NewLine);
                    return true;
                }
            }
            private bool _IsNewLine = true;
            private void InnerWrite(string txt)
            {
                if (txt != null && txt.Length > 0)
                {
                    this._box.SelectionLength = 0;
                    this._box.SelectionStart = this._box.TextLength;
                    this._box.SelectionBackColor = base.ToColor(this._BackColor);
                    this._box.SelectionColor = base.ToColor(this._ForeColor);
                    this._box.SelectedText = txt;
                    this._IsNewLine = txt[txt.Length - 1] == '\r' || txt[txt.Length - 1] == '\n';
                    this._box.SelectionStart = this._box.TextLength;
                    this._box.ScrollToCaret();
                }
            }
            public override void Write(string value)
            {
                if (value != null && value.Length > 0)
                {
                    if (this._box.InvokeRequired)
                    {
                        this._box.Invoke(new VoidHandler(delegate ()
                        {
                            InnerWrite(value);
                        }));
                    }
                    else
                    {
                        InnerWrite(value);
                    }
                }
            }
            public override void WriteLine(string value)
            {
                if (value == null || value.Length == 0)
                {
                    this.Write(Environment.NewLine);
                }
                else
                {
                    this.Write(value + Environment.NewLine);
                }
            }
            public override void WriteLine()
            {
                this.Write(Environment.NewLine);
            }
        }//private class RichTextBoxConsole : JIEJIE.MyConsole , IDisposable

        private void btnBrowseInputAssembly_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "*.dll;*.exe|*.dll;*.exe";
                dlg.CheckFileExists = true;
                dlg.FileName = this.txtInputAssembly.Text;
                if(dlg.ShowDialog( this ) == DialogResult.OK )
                {
                    this.txtInputAssembly.Text = dlg.FileName;
                    RefreshLanguageList();
                }
            }
        }

        private void RefreshLanguageList()
        {
            var culs = DCUtils.GetSupporttedCultures(this.txtInputAssembly.Text);
            cboLanguage.Items.Clear();
            if( culs != null )
            {
                cboLanguage.Items.Add("Default");
                foreach( var item in culs )
                {
                    cboLanguage.Items.Add(item.Key + " # " + item.Value);
                }
            }
        }

        private void txtInputAssembly_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                RefreshLanguageList();
            }
        }

        private void txtInputAssembly_Leave(object sender, EventArgs e)
        {
            RefreshLanguageList();
        }

        private void btnBrowseSDKPath_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.ShowNewFolderButton = false;
                dlg.SelectedPath = txtSDKPath.Text;
                dlg.RootFolder = Environment.SpecialFolder.MyComputer;
                if( dlg.ShowDialog( this ) == DialogResult.OK )
                {
                    txtSDKPath.Text = dlg.SelectedPath;
                }
            }
        }

        private void btnBrowseOutputPath_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.ShowNewFolderButton = true;
                dlg.SelectedPath = txtOutputPath.Text;
                dlg.RootFolder = Environment.SpecialFolder.MyComputer;
                if( dlg.ShowDialog( this ) == DialogResult.OK )
                {
                    txtOutputPath.Text = dlg.SelectedPath;
                }
            }
        }

        private void btnBrowseSnkFile_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "*.snk|*.snk";
                dlg.CheckFileExists = true;
                dlg.FileName = txtSNKFile.Text;
                if( dlg.ShowDialog( this ) == DialogResult.OK )
                {
                    txtSNKFile.Text = dlg.FileName;
                }
            }
        }
        private string _LastConfigFileName = null;
        private void btnLoadConfig_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "*.xml|*.xml";
                dlg.CheckFileExists = true;
                if(this._LastConfigFileName != null )
                {
                    dlg.FileName = this._LastConfigFileName;
                }
                if( dlg.ShowDialog( this ) == DialogResult.OK )
                {
                    LoadProjectFile(dlg.FileName);
                }
            }
        }
        /// <summary>
        /// 加载项目文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        private void LoadProjectFile( string fileName )
        {
            this._Project.LoadConfigFile(fileName);
            this._LastConfigFileName = fileName;
            RefreshConfigView();
            if (RecencFileNameList.Instance.AddItem("LoadConfig",fileName))
            {
                RefreshLoadFileMenus();
            }
        }
        private void RefreshConfigView()
        {
            this.txtInputAssembly.Text = this._Project.InputAssemblyFileName;
            this.chkRename.Checked = this._Project.Switchs.Rename;
            this.txtPrefixForMemberRename.Text = this._Project.PrefixForMemberRename;
            this.txtPrefixForTypeRename.Text = this._Project.PrefixForTypeRename;
            this.txtSDKPath.Text = this._Project.SDKDirectory;
            this.txtSNKFile.Text = this._Project.SnkFileName;
            this.chkEncryptResources.Checked = this._Project.Switchs.Resources;
            this.chkEncryptStrings.Checked = this._Project.Switchs.Strings;
            this.txtResourceNameNeedEncrypt.Text = this._Project.ResourceNameNeedEncrypt;
            //this.chkEncryptAllocStack.Checked = this._Project.Switchs.AllocationCallStack;
            this.chkObfuscateControlFlow.Checked = this._Project.Switchs.ControlFlow;
            this.chkObfuscateMemberOrder.Checked = this._Project.Switchs.MemberOrder;
            this.chkRemoveMember.Checked = this._Project.Switchs.RemoveMember;
            this.chkMergeAssemblies.Checked = this._Project.MergeFileNames == "*";
            this.chkDebugMode.Checked = this._Project.DebugMode;
            this.chkDeleteTempFile.Checked = this._Project.DeleteTempFile;
            this.txtOutputPath.Text = this._Project.OutputAssemblyFileName;
            this.chkOutputMapXml.Checked = this._Project.OutpuptMapXml;
            this.chkTestUseNGen.Checked = this._Project.TestUseNGen;
            RefreshLanguageList();
            for(int iCount = 1 ;iCount < cboLanguage.Items.Count; iCount ++)
            {
                string item = (string)cboLanguage.Items[iCount];
                item = item.Substring(0, item.IndexOf('#')).Trim();
                if( this._Project.UILanguageName == item )
                {
                    cboLanguage.SelectedIndex = iCount;
                    break;
                }
            }
        }
        private void ApplyConfigToProject()
        {
            this._Project.InputAssemblyFileName = this.txtInputAssembly.Text.Trim();
            this._Project.Switchs.Rename = this.chkRename.Checked;
            this._Project.PrefixForMemberRename = this.txtPrefixForMemberRename.Text.Trim();
            this._Project.PrefixForTypeRename = this.txtPrefixForTypeRename.Text.Trim();
            this._Project.SDKDirectory = this.txtSDKPath.Text.Trim();
            this._Project.SnkFileName = this.txtSNKFile.Text.Trim();
            this._Project.Switchs.Strings = this.chkEncryptStrings.Checked;
            this._Project.ResourceNameNeedEncrypt = this.txtResourceNameNeedEncrypt.Text.Trim();
            this._Project.Switchs.Resources = this.chkEncryptResources.Checked;
            this._Project.Switchs.Strings = this.chkEncryptStrings.Checked;
            //this._Project.Switchs.AllocationCallStack = this.chkEncryptAllocStack.Checked;
            this._Project.Switchs.MemberOrder = this.chkObfuscateMemberOrder.Checked;
            this._Project.Switchs.RemoveMember = this.chkRemoveMember.Checked;
            this._Project.Switchs.ControlFlow = this.chkObfuscateControlFlow.Checked;
            this._Project.MergeFileNames = this.chkMergeAssemblies.Checked ? "*" : null;
            this._Project.DebugMode = this.chkDebugMode.Checked;
            this._Project.DeleteTempFile = this.chkDeleteTempFile.Checked;
            this._Project.OutputAssemblyFileName = this.txtOutputPath.Text.Trim();
            this._Project.TestUseNGen = this.chkTestUseNGen.Checked;
            if(cboLanguage.SelectedIndex > 0 )
            {
                var item = (string)cboLanguage.SelectedItem;
                this._Project.UILanguageName = item.Substring(0, item.IndexOf("#")).Trim();
            }
            else
            {
                this._Project.UILanguageName = null;
            }
        }
        private void btnSaveConfigFile_Click(object sender, EventArgs e)
        {
            ApplyConfigToProject();
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "*.xml|*.xml";
                if( this._LastConfigFileName != null )
                {
                    dlg.FileName = this._LastConfigFileName;
                }
                dlg.OverwritePrompt = true;
                if(dlg.ShowDialog( this ) == DialogResult.OK )
                {
                    this._Project.SaveConfigFile(dlg.FileName);
                    this._LastConfigFileName = dlg.FileName;
                }
            }
        }
        private System.ComponentModel.BackgroundWorker _Worker = null;
        private void btnRun_Click(object sender, EventArgs e)
        {
            if( CheckFileNameTextBox( this.txtInputAssembly , "Please input source assembly file name at first.") == false )
            {
                return;
            }
            var w = this._Worker;
            this._Worker = null;
            if(w != null )
            {
                w.CancelAsync();
                w.Dispose();
            }
            this.txtLog.Clear();
            this._Worker = new BackgroundWorker();
            this._Worker.DoWork += _Worker_DoWork;
            this._Worker.RunWorkerCompleted += _Worker_RunWorkerCompleted;
            this._Worker.WorkerSupportsCancellation = true;
            this.Cursor = Cursors.WaitCursor;
            ApplyConfigToProject();
            MyConsole.SetInstance(new RichTextBoxConsole(this.txtLog));
            this.btnRun.Enabled = false;
            this.btnStop.Enabled = true;
            this.ctlProgress.Visible = true;
            this.btnLoadConfig.Enabled = false;
            this.btnSaveConfig.Enabled = false;
            this._Worker.RunWorkerAsync();
        }

        private void _Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MyConsole.SetInstance(null);
            this.btnRun.Enabled = true;
            this.btnStop.Enabled = false;
            this._Worker = null;
            this.Cursor = Cursors.Default;
            this.ctlProgress.Visible = false;
            this.btnLoadConfig.Enabled = true;
            this.btnSaveConfig.Enabled = true;
        }

        private void _Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this._Project.Run();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if(this._Worker != null )
            {
                this._Worker.CancelAsync();
                this.btnStop.Enabled = false;
                this.btnRun.Enabled = true;
            }
            this.ctlProgress.Visible = false;
            this.btnLoadConfig.Enabled = true;
            this.btnSaveConfig.Enabled = true;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if(RecencFileNameList.Instance.Modified )
            {
                RecencFileNameList.Instance.SaveConfigFile();
            }
            if(this._Worker != null )
            {
                this._Worker.CancelAsync();
                this._Worker = null;
            }
            base.OnClosing(e);
        }

        private void btnBrowseMapXmlFile_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "*.map.xml|*.map.xml|*.xml|*.xml";
                dlg.CheckFileExists = true;
                dlg.FileName = txtMapXmlFile.Text;
                if(dlg.ShowDialog( this ) == DialogResult.OK )
                {
                    txtMapXmlFile.Text = dlg.FileName;
                }
            }
        }

        private void btnTranslateStackTrace_Click(object sender, EventArgs e)
        {
            if (CheckFileNameTextBox(
                this.txtMapXmlFile,
                "Please input rename map xml file which create by JIEJIE.NET.") == false)
            {
                return;
            }
            if (RecencFileNameList.Instance.AddItem("Map.xml", this.txtMapXmlFile.Text.Trim()))
            {
                this.txtMapXmlFile.Items.Clear();
                this.txtMapXmlFile.Items.AddRange(
                    RecencFileNameList.Instance.GetRecenFileNames("Map.xml").ToArray());
            }
            this.txtTranslateResult.Clear();
            MyConsole.SetInstance(new RichTextBoxConsole(this.txtTranslateResult));
            var strResult = DCJieJieNetEngine.TranslateStackTraceUseMapXml(this.txtMapXmlFile.Text.Trim(), txtCallStackTrace.Text);
            DCJieJieNetEngine.ConoleWriteStackTrace(strResult);
            MyConsole.SetInstance(null);
        }

        /// <summary>
        /// 检查文件输入框中的文件是否存在
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool CheckFileNameTextBox( System.Windows.Forms.Control txt , string msg )
        {
            var fn = txt.Text.Trim();
            if( fn == null || fn.Length == 0 || File.Exists( fn )== false )
            {
                MessageBox.Show(this, msg);
                txt.Focus();
                txt.Select();
                return false;
            }
            return true;
        }

        private void btnCopySelectedText_Click(object sender, EventArgs e)
        {
            txtLog.Copy();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "JIEJIE.NET v" + DCJieJieNetEngine.ProductVersion + " from https://github.com/dcsoft-yyf/JIEJIE.NET ");
        }

        private void btnCopySelectedTextForStackTrace_Click(object sender, EventArgs e)
        {
            this.txtTranslateResult.Copy();
        }
    }
}
