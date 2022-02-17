/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;

#if ! DOTNETCORE
[assembly: AssemblyDescription("Protect your .NET software copyright powerfull.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DCSoft")]
[assembly: AssemblyProduct("JieJie.NET")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(JIEJIE.DCJieJieNetEngine.ProductVersion)]
#endif

namespace JIEJIE
{
    /// <summary>
    /// 项目对象
    /// </summary>
    internal class JieJieProject
    {
        /// <summary>
        /// 输入的程序集文件名
        /// </summary>
        public string InputAssemblyFileName = null;
        /// <summary>
        /// 指定的输出的程序集的文件名或者目录名
        /// </summary>
        public string OutputAssemblyFileName = null;
        /// <summary>
        /// 指定使用的临时目录
        /// </summary>
        public string InputTempPath = null;
        /// <summary>
        /// 任务结束后命令行界面是否暂停
        /// </summary>
        public bool PauseAtLast = false;
        /// <summary>
        /// 合并的程序集文件名
        /// </summary>
        public string MergeFileNames = null;
        /// <summary>
        /// 自定义指令
        /// </summary>
        public Dictionary<string, string> CustomInstructions = null;
        /// <summary>
        /// 执行调用堆栈翻译的映射XML文件名
        /// </summary>
        public string TranslateStackTraceUseMapXml = null;
        /// <summary>
        /// SNK文件名
        /// </summary>
        public string SnkFileName = null;
        /// <summary>
        /// 开关
        /// </summary>
        public JieJieSwitchs Switchs = new JieJieSwitchs();
        /// <summary>
        /// 指定的.NET SDK安装目录
        /// </summary>
        public string SDKDirectory = null;
        /// <summary>
        /// 类型重命名使用的前缀
        /// </summary>
        public string PrefixForTypeRename = "_jiejie";
        /// <summary>
        /// 类型成员重命名使用的前缀
        /// </summary>
        public string PrefixForMemberRename = "_jj";
        /// <summary>
        /// 要加密的内嵌资源名
        /// </summary>
        public string ResourceNameNeedEncrypt = null;
        /// <summary>
        /// 是否输出映射文件
        /// </summary>
        public bool OutpuptMapXml = true;
        /// <summary>
        /// 完成后是否删除临时目录
        /// </summary>
        public bool DeleteTempFile = false;
        /// <summary>
        /// 是否为调试模式
        /// </summary>
        public bool DebugMode = false;
        /// <summary>
        /// 用户界面语言
        /// </summary>
        public string UILanguageName = null;
        /// <summary>
        /// 使用ngen.exe/crossgen.exe测试输出结果
        /// </summary>
        public bool TestUseNGen = true;
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>操作是否成功</returns>
        public bool LoadConfigFile(string fileName)
        {
            if (fileName == null || fileName.Length == 0)
            {
                throw new ArgumentNullException("fileName");
            }
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException(fileName);
            }
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(fileName);
            if (xmlDoc.DocumentElement.Name == "JieJie.Net.Config"
                && xmlDoc.DocumentElement.GetAttribute("Version") == "1.0")
            {
                foreach (System.Xml.XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "InputAssemblyFileName": this.InputAssemblyFileName = node.InnerText; break;
                        case "OutputAssemblyFileName": this.OutputAssemblyFileName = node.InnerText; break;
                        case "InputTempPath": this.InputTempPath = node.InnerText; break;
                        case "PauseAtLast": this.PauseAtLast = StringToBoolean(node.InnerText, true); break;
                        case "MergeFileNames": this.MergeFileNames = node.InnerText; break;
                        case "CustomInstructions":
                            {
                                this.CustomInstructions = new Dictionary<string, string>
                                    (System.StringComparer.CurrentCultureIgnoreCase);
                                foreach (System.Xml.XmlNode node2 in node.ChildNodes)
                                {
                                    if (node2.Name == "Item")
                                    {
                                        var e2 = (System.Xml.XmlElement)node2;
                                        var strName = e2.GetAttribute("Name");
                                        var strV = e2.GetAttribute("Value");
                                        if (strName != null
                                            && strName.Length > 0
                                            && strV != null
                                            && strV.Length > 0)
                                        {
                                            this.CustomInstructions[strName] = strV;
                                        }
                                    }
                                }
                            }
                            break;
                        case "TranslateStackTraceUseMapXml": this.TranslateStackTraceUseMapXml = node.InnerText; break;
                        case "SnkFileName": this.SnkFileName = node.InnerText; break;
                        case "Switchs":
                            {
                                this.Switchs = new JieJieSwitchs();
                                foreach (System.Xml.XmlNode node2 in node.ChildNodes)
                                {
                                    switch (node2.Name)
                                    {
                                        case "ControlFlow": this.Switchs.ControlFlow = StringToBoolean(node2.InnerText, true); break;
                                        case "Strings": this.Switchs.Strings = StringToBoolean(node2.InnerText, true); break;
                                        case "Resources": this.Switchs.Resources = StringToBoolean(node2.InnerText, true); break;
                                        //case "AllocationCallStack": this.Switchs.AllocationCallStack = StringToBoolean(node2.InnerText, true); break;
                                        case "MemberOrder": this.Switchs.MemberOrder = StringToBoolean(node2.InnerText, true); break;
                                        case "Rename": this.Switchs.Rename = StringToBoolean(node2.InnerText, true); break;
                                        case "RemoveMember": this.Switchs.RemoveMember = StringToBoolean(node2.InnerText, true); break;
                                    }
                                }
                            }
                            break;
                        case "SDKDirectory": this.SDKDirectory = node.InnerText; break;
                        case "UILanguageName": this.UILanguageName = node.InnerText; break;
                        case "PrefixForTypeRename": this.PrefixForTypeRename = node.InnerText; break;
                        case "PrefixForMemberRename": this.PrefixForMemberRename = node.InnerText; break;
                        case "ResourceNameNeedEncrypt": this.ResourceNameNeedEncrypt = node.InnerText; break;
                        case "OutpuptMapXml": this.OutpuptMapXml = StringToBoolean(node.InnerText, true); break;
                        case "DeleteTempFile": this.DeleteTempFile = StringToBoolean(node.InnerText, false); break;
                        case "DebugMode": this.DebugMode = StringToBoolean(node.InnerText, false); break;
                        case "TestUseNGen": this.TestUseNGen = StringToBoolean(node.InnerText, true);break;
                    }
                }
                return true;
            }
            return false;
        }
        private bool StringToBoolean(string v, bool defaultValue)
        {
            if (v == null || v.Length == 0)
            {
                return defaultValue;
            }
            try
            {
                if (string.Compare(v, "true", true) == 0)
                {
                    return true;
                }
                if (string.Compare(v, "false", true) == 0)
                {
                    return false;
                }
                return Convert.ToBoolean(defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 保存配置信息到文件中
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void SaveConfigFile(string fileName)
        {
            if (fileName == null || fileName.Length == 0)
            {
                throw new ArgumentNullException("fileName");
            }
            using (var writer = new System.Xml.XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 3;
                writer.IndentChar = ' ';
                writer.WriteStartDocument();
                writer.WriteStartElement("JieJie.Net.Config");
                writer.WriteAttributeString("Version", "1.0");
                if (this.InputAssemblyFileName != null)
                {
                    writer.WriteElementString("InputAssemblyFileName", this.InputAssemblyFileName);
                }
                if (this.OutputAssemblyFileName != null)
                {
                    writer.WriteElementString("OutputAssemblyFileName", this.OutputAssemblyFileName);
                }
                if (this.InputTempPath != null)
                {
                    writer.WriteElementString("InputTempPath", this.InputTempPath);
                }
                writer.WriteElementString("PauseAtLast", this.PauseAtLast.ToString());
                if (this.MergeFileNames != null)
                {
                    writer.WriteElementString("MergeFileNames", this.MergeFileNames);
                }
                if (this.CustomInstructions != null)
                {
                    writer.WriteStartElement("CustomInstructions");
                    foreach (var item in this.CustomInstructions)
                    {
                        writer.WriteStartElement("Item");
                        writer.WriteAttributeString("Name", item.Key);
                        writer.WriteAttributeString("Value", item.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                if (this.TranslateStackTraceUseMapXml != null)
                {
                    writer.WriteElementString("TranslateStackTraceUseMapXml", this.TranslateStackTraceUseMapXml);
                }
                if (this.SnkFileName != null)
                {
                    writer.WriteElementString("SnkFileName", this.SnkFileName);
                }
                if (this.Switchs != null)
                {
                    writer.WriteStartElement("Switchs");
                    writer.WriteElementString("ControlFlow", Convert.ToString(this.Switchs.ControlFlow));
                    writer.WriteElementString("Strings", Convert.ToString(this.Switchs.Strings));
                    writer.WriteElementString("Resources", Convert.ToString(this.Switchs.Resources));
                    //writer.WriteElementString("AllocationCallStack", Convert.ToString(this.Switchs.AllocationCallStack));
                    writer.WriteElementString("MemberOrder", Convert.ToString(this.Switchs.MemberOrder));
                    writer.WriteElementString("Rename", Convert.ToString(this.Switchs.Rename));
                    writer.WriteElementString("RemoveMember", Convert.ToString(this.Switchs.RemoveMember));
                    writer.WriteEndElement();
                }
                if (this.SDKDirectory != null)
                {
                    writer.WriteElementString("SDKDirectory", this.SDKDirectory);
                }
                if (this.UILanguageName != null)
                {
                    writer.WriteElementString("UILanguageName", this.UILanguageName);
                }
                if (this.PrefixForTypeRename != null)
                {
                    writer.WriteElementString("PrefixForTypeRename", this.PrefixForTypeRename);
                }
                if (this.PrefixForMemberRename != null)
                {
                    writer.WriteElementString("PrefixForMemberRename", this.PrefixForMemberRename);
                }
                if (this.ResourceNameNeedEncrypt != null)
                {
                    writer.WriteElementString("ResourceNameNeedEncrypt", this.ResourceNameNeedEncrypt);
                }
                writer.WriteElementString("OutpuptMapXml", this.OutpuptMapXml.ToString());
                writer.WriteElementString("DeleteTempFile", this.DeleteTempFile.ToString());
                writer.WriteElementString("DebugMode", this.DebugMode.ToString());
                writer.WriteElementString("TestUseNGen", this.TestUseNGen.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
         
        /// <summary>
        /// 运行项目
        /// </summary>
        public void Run()
        {
            if (this.TranslateStackTraceUseMapXml != null && this.TranslateStackTraceUseMapXml.Length > 0)
            {
                DCJieJieNetEngine.ConsoleTranslateStackTraceUseMapXml(this.TranslateStackTraceUseMapXml);
                return;
            }
            try
            {
                MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                MyConsole.Instance.WriteLine(@"
*****************************************************************************************
           __   __   _______        __   __   _______   .__   __.  _______ .___________.
          |  | |  | |   ____|      |  | |  | |   ____|  |  \ |  | |   ____||           |
          |  | |  | |  |__         |  | |  | |  |__     |   \|  | |  |__   `---|  |----`
    .--.  |  | |  | |   __|  .--.  |  | |  | |   __|    |  . `  | |   __|      |  |     
    |  `--'  | |  | |  |____ |  `--'  | |  | |  |____ __|  |\   | |  |____     |  |     
     \______/  |__| |_______| \______/  |__| |_______(__)__| \__| |_______|    |__|     

     From https://github.com/dcsoft-yyf/JIEJIE.NET
     JieJie.NET v" + DCJieJieNetEngine.ProductVersion + @" ,encrypt .NET assembly file, help you protect copyright.");
                var strInfoMore = @"     Jie(2)Jie(4) in chinese is a kind of transparet magic protect shield.
     Author:yuan yong fu . mail: 28348092@qq.com
     Last update 2021-8-26
     You can use this software free forever,but CAN NOT modify source code anytime.
     Any good idears you can write to 28348092@qq.com.
     Support command line argument :
        input =[required,default argument,Full path of input .NET assembly file , can be .exe or .dll, currenttly only support .NET framework 2.0 or later]
        output=[optional,Full path of output .NET assmebly file , if it is empty , then use sub direcotry '\jiejie.net_result'.]
        snk   =[optional,Full path of snk file. It use to add strong name to output assembly file.]
        switch=[optional,multi-switch split by ',',also can be define in [System.Reflection.ObfuscationAttribute.Feature]. It support :
                +/-contorlfow  = enable/disable obfuscate control flow in method body.
                +/-strings     = enable/disable encrypt string value.
                +/-resources   = enable/disable encrypt resources data.
                +/-memberorder = enable/disable member list order in type.
                +/-rename      = enable/disable rename type or member's name.
                +/-allocationcallstack = enable/disable encrypt string value allocation callstack.
                +/-removemember= enable/disable remove unused const field and properties.
            ]
        pause    =[optional,pause the console after finish process.]
        debugmode=[optional,Allow show some debug info text.]
        sdkpath  =[optional,set the direcotry full name of ildasm.exe.]
        merge    =[optional,some .net assembly file to merge to the result file. '*' for all referenced assembly files.]
        .corflags=[optional, it is a integer flag,'3' for 32-bit process without strong name signature, '1' for 64-bit wihout strong name, '9' for 32-bit with strong name ,'10' for 64-bit with strong name.]
        .subsystem=[optional, it a integer value, '2' for application in GUI mode.'3' for application in console mode.]
        outputmapxml=[optional, true/false,speicfy out put a xml file contains class/member's old name and new name.]
        prefixfortyperename=[optional, the prefix use to rename type name.]
        prefixformemberrename=[optional,the prefix use to rename type's member name.]
        deletetempfile=[optional,delete template file after job finshed.default is false.]
        ResourceNameNeedEncrypt=[optional,specify resouce allow to encrypted.support regular expression.]
     Example 1, protect d:\a.dll ,this will modify dll file.
        >JIEJIE.NET.exe d:\a.dll  
     Exmaple 2, anlyse d:\a.dll , and write result to another dll file with strong name. enable obfuscate control flow and not encript resources.
        >JIEJIE.NET.exe input=d:\a.dll output=d:\publish\a.dll snk=d:\source\company.snk switch=+contorlfow,-resources
**************************** MADE IN CHINA **********************************************";
                var startDir = Path.GetDirectoryName(typeof(DCJieJieNetEngine).Assembly.Location);
                string inputDir = null;
                if (this.InputAssemblyFileName != null && this.InputAssemblyFileName.Length > 0)
                {
                    this.InputTempPath = null;
                    if (File.Exists(this.InputAssemblyFileName) == false)
                    {
                        MyConsole.Instance.WriteError("Can not find file '" + this.InputAssemblyFileName + "'.");
                        return;
                    }
                    MyConsole.Instance.Title = "JIEJIE.NET - " + this.InputAssemblyFileName;
                    inputDir = Path.GetDirectoryName(this.InputAssemblyFileName);
                    this.InputTempPath = null;
                }
                else if (this.InputTempPath != null && this.InputTempPath.Length > 0)
                {
                    if (Directory.Exists(this.InputTempPath) == false)
                    {
                        MyConsole.Instance.WriteError("Can not find path '" + this.InputTempPath + "'.");
                        return;
                    }
                    MyConsole.Instance.Title = "JIEJIE.NET use IL - " + this.InputTempPath;
                    inputDir = this.InputTempPath;
                    this.InputAssemblyFileName = null;
                }
                else
                {
                    MyConsole.Instance.ResetColor();
                    MyConsole.Instance.WriteLine(strInfoMore);
                    return;
                }
                MyConsole.Instance.WriteLine("**************************** MADE IN CHINA **********************************************");
                MyConsole.Instance.ResetColor();
                System.AppDomain taskDomain = null;
                var eng = new DCJieJieNetEngine();
                eng.Switchs = this.Switchs;
                eng.SDKDirectory = this.SDKDirectory;
                eng.PrefixForMemberRename = this.PrefixForMemberRename;
                eng.PrefixForTypeRename = this.PrefixForTypeRename;
                eng.ResourceNameNeedEncrypt = this.ResourceNameNeedEncrypt;
                eng.OutpuptMapXml = this.OutpuptMapXml;
                eng.DeleteTempFile = this.DeleteTempFile;
                eng.DebugMode = this.DebugMode;
                eng.SnkFileName = this.SnkFileName;
                eng._UILanguageName = this.UILanguageName;
                if (string.Compare(startDir, inputDir, true) != 0 && eng.Switchs.Rename)
                {
#if DOTNETCORE
                    DCILTypeReference._AsmLoader = new System.Runtime.Loader.AssemblyLoadContext("jiejie.net temp");
#else
                    // 使用跨域来解决来源程序集不在同一个路径的问题
                    System.AppDomainSetup appSetup = new System.AppDomainSetup();
                    appSetup.PrivateBinPath = inputDir;
                    appSetup.ApplicationName = "jiejie.net temp";
                    appSetup.ApplicationBase = inputDir;
                    if (AppDomain.CurrentDomain.ApplicationTrust != null)
                    {
                        appSetup.ApplicationTrust = AppDomain.CurrentDomain.ApplicationTrust;
                    }
                    taskDomain = System.AppDomain.CreateDomain("jiejie.net temp", AppDomain.CurrentDomain.Evidence, appSetup);
                    var taskEng =(DCJieJieNetEngine)taskDomain.CreateInstanceFromAndUnwrap(
                        typeof(DCJieJieNetEngine).Assembly.Location,
                        typeof(DCJieJieNetEngine).FullName);
                    taskEng.BindCurrentDomain_AssemblyResolve();
                    eng.CopytSettingsTo(taskEng);
                    if (MyConsole.Instance.IsNativeConsole == false)
                    {
                        taskEng.SetConsoleInstance(MyConsole.Instance);
                    }
                    eng.Dispose();
                    eng = taskEng;
                    
#endif
                }
                if (this.InputAssemblyFileName != null)
                {
                    if (eng.LoadAssemblyFile(this.InputAssemblyFileName, this.MergeFileNames) == false)
                    {
                        return;
                    }
                }
                else
                {
                    if (eng.LoadILFromTempPath(this.InputTempPath) == false)
                    {
                        return;
                    }
                }
                eng.SetDocumentCustomInstructions(this.CustomInstructions);
                eng.HandleDocument();
                if (this.OutputAssemblyFileName == null || this.OutputAssemblyFileName.Length == 0)
                {
                    var dir = Path.Combine(Path.GetDirectoryName(this.InputAssemblyFileName), "jiejie.net_result");
                    if (Directory.Exists(dir) == false)
                    {
                        Directory.CreateDirectory(dir);
                    }
                    this.OutputAssemblyFileName = Path.Combine(
                        dir,
                        Path.GetFileName(eng.GetDocumentAssemblyFileName()));
                }
                else if (System.IO.Directory.Exists(this.OutputAssemblyFileName))
                {
                    this.OutputAssemblyFileName = Path.Combine(
                        this.OutputAssemblyFileName,
                        Path.GetFileName(eng.GetDocumentAssemblyFileName()));
                }
                eng.SaveAssemblyFile(this.OutputAssemblyFileName, this.TestUseNGen);
                if (this.DeleteTempFile)
                {
                    // 删除临时文件
                    eng.DeleteTemplateDirecotry();
                }
                eng.Dispose();
                eng = null;
#if !DOTNETCORE
                if (taskDomain != null)
                {
                    System.AppDomain.Unload(taskDomain);
                }
#endif
            }
            catch (System.Exception ext)
            {
                MyConsole.Instance.WriteError(ext.ToString());
            }
            finally
            {
                if (this.PauseAtLast && MyConsole.Instance.SupportKeyboardInput)
                {
                    DCUtils.EatAllConsoleKey();
                    MyConsole.Instance.WriteLine("##########  All finished, press any key to continue ############");
                    MyConsole.Instance.ReadKey();
                }
            }
        }
         
    }


    /// <summary>
    /// 命令行输出界面
    /// </summary>
    [Serializable]
    internal class MyConsole : System.MarshalByRefObject
    {
        private static MyConsole _Instance = new MyConsole();
        /// <summary>
        /// 对象静态实例
        /// </summary>
        public static MyConsole Instance
        {
            get
            {
                return _Instance;
            }
        }
        /// <summary>
        /// 设置对象静态实例
        /// </summary>
        /// <param name="instance"></param>
        public static void SetInstance(MyConsole instance)
        {
            if (instance == null)
            {
                _Instance = new MyConsole();
            }
            else
            {
                _Instance = instance;
            }
        }
        public MyConsole()
        {

        }
        /// <summary>
        /// 确保在新的一行开始
        /// </summary>
        /// <returns>操作是否产生换行行为</returns>
        public virtual bool EnsureNewLine()
        {
            return false;
        }

        public bool IsNativeConsole
        {
            get
            {
                return this.GetType() == typeof(MyConsole);
            }
        }
        /// <summary>
        /// 是否支持键盘输入
        /// </summary>
        public virtual bool SupportKeyboardInput
        {
            get
            {
                return true;
            }
        }
        /*
        
                //
    // 摘要:
    //     指定定义控制台前景色和背景色的常数。
    public enum ConsoleColor
    {
        //
        // 摘要:
        //     黑色。
        Black = 0,
        //
        // 摘要:
        //     藏蓝色。
        DarkBlue = 1,
        //
        // 摘要:
        //     深绿色。
        DarkGreen = 2,
        //
        // 摘要:
        //     深紫色（深蓝绿色）。
        DarkCyan = 3,
        //
        // 摘要:
        //     深红色。
        DarkRed = 4,
        //
        // 摘要:
        //     深紫红色。
        DarkMagenta = 5,
        //
        // 摘要:
        //     深黄色（赭色）。
        DarkYellow = 6,
        //
        // 摘要:
        //     灰色。
        Gray = 7,
        //
        // 摘要:
        //     深灰色。
        DarkGray = 8,
        //
        // 摘要:
        //     蓝色。
        Blue = 9,
        //
        // 摘要:
        //     绿色。
        Green = 10,
        //
        // 摘要:
        //     青色（蓝绿色）。
        Cyan = 11,
        //
        // 摘要:
        //     红色。
        Red = 12,
        //
        // 摘要:
        //     紫红色。
        Magenta = 13,
        //
        // 摘要:
        //     黄色。
        Yellow = 14,
        //
        // 摘要:
        //     白色。
        White = 15
    }

            */
        private static System.Drawing.Color[] _ConsoleColors = null;
        /// <summary>
        /// 将命令行颜色值转换为标准颜色值
        /// </summary>
        /// <param name="c">命令行颜色值</param>
        /// <returns>标准颜色值</returns>
        protected System.Drawing.Color ToColor(System.ConsoleColor c)
        {
            if (_ConsoleColors == null)
            {
                lock (typeof(MyConsole))
                {
                    _ConsoleColors = new System.Drawing.Color[] {
                            System.Drawing.Color.FromArgb( 12,12,12) ,
                            System.Drawing.Color.FromArgb( 0 , 55, 218) ,
                            System.Drawing.Color.FromArgb( 19 , 161 , 14 ) ,
                            System.Drawing.Color.FromArgb( 58 , 150 , 221) ,
                            System.Drawing.Color.FromArgb( 197, 15 , 31 ) ,
                            System.Drawing.Color.FromArgb( 136 , 23 , 152 )  ,
                            System.Drawing.Color.FromArgb( 193,156,0),
                            System.Drawing.Color.FromArgb( 204,204,204) ,
                            System.Drawing.Color.FromArgb( 118,118,118) ,
                            System.Drawing.Color.FromArgb( 59,120,255) ,
                            System.Drawing.Color.FromArgb( 22,198,12),
                            System.Drawing.Color.FromArgb(97,214,214) ,
                            System.Drawing.Color.FromArgb(231,72,86),
                            System.Drawing.Color.FromArgb(180,0,158) ,
                            System.Drawing.Color.FromArgb(249,241,165) ,
                            System.Drawing.Color.FromArgb(242,242,242)
                        };

                    //var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Console", false);
                    //if (reg != null)
                    //{
                    //    var indexMap = new ConsoleColor[16];
                    //    indexMap[0] = ConsoleColor.Black;
                    //    indexMap[4] = ConsoleColor.DarkBlue;
                    //    indexMap[2] = ConsoleColor.DarkGreen;
                    //    indexMap[6] = ConsoleColor.DarkCyan;
                    //    indexMap[1] = ConsoleColor.DarkRed;
                    //    indexMap[5] = ConsoleColor.DarkMagenta;
                    //    indexMap[3] = ConsoleColor.DarkYellow;
                    //    indexMap[7] = ConsoleColor.Gray;
                    //    indexMap[8] = ConsoleColor.DarkGray;
                    //    indexMap[12] = ConsoleColor.Blue;
                    //    indexMap[10] = ConsoleColor.Green;
                    //    indexMap[14] = ConsoleColor.Cyan;
                    //    indexMap[9] = ConsoleColor.Red;
                    //    indexMap[13] = ConsoleColor.Magenta;
                    //    indexMap[11] = ConsoleColor.Yellow;
                    //    indexMap[15] = ConsoleColor.White;
                    //    for (int iCount = 0; iCount < _ConsoleColors.Length; iCount++)
                    //    {
                    //        var cv = reg.GetValue("ColorTable" + iCount.ToString("00"));
                    //        if (cv is int)
                    //        {
                    //            _ConsoleColors[(int)indexMap[iCount]] = System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb((int)cv));
                    //        }
                    //    }
                    //    reg.Close();
                    //}
                    ////foreach( var item in Enum.GetValues( typeof(System.ConsoleColor)))
                    ////{
                    ////    var v =  _ConsoleColors[(int)item].ToArgb().ToString("X6").Substring( 2 );
                    ////    System.Diagnostics.Debug.WriteLine("<br />" + Convert.ToInt32( item ) +" " + item.ToString() + "<span style='width:100px;background-color:#" + v + "'>11111111</span>");
                    ////}
                }

            }
            if (c >= ConsoleColor.Black && c <= ConsoleColor.White)
            {
                return _ConsoleColors[(int)c];
            }
            else
            {
                return System.Drawing.Color.Black;
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title
        {
            get
            {
                return Console.Title;
            }
            set
            {
                Console.Title = value;
            }
        }
        
        public virtual bool KeyAvailable
        {
            get
            {
                return Console.KeyAvailable;
            }
        }
        public virtual string ReadLine()
        {
            return Console.ReadLine();
        }
        public virtual ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
        public virtual int CursorLeft
        {
            get
            {
                return Console.CursorLeft;
            }
            set
            {
                Console.CursorLeft = value;
            }
        }

        public virtual int CursorTop
        {
            get
            {
                return Console.CursorTop;
            }
            set
            {
                Console.CursorTop = value;
            }
        }

        public virtual ConsoleColor BackgroundColor
        {
            get
            {
                return Console.BackgroundColor;
            }
            set
            {
                Console.BackgroundColor = value;
            }
        }
        public virtual ConsoleColor ForegroundColor
        {
            get
            {
                return Console.ForegroundColor;
            }
            set
            {
                Console.ForegroundColor = value;
            }
        }
        public virtual void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
        public virtual void WriteLine()
        {
            Console.WriteLine();
        }
        public virtual void Write(string value)
        {
            Console.Write(value);
        }
        public virtual void ResetColor()
        {
            Console.ResetColor();
        }
        public virtual void WriteError(string msg)
        {
            this.ForegroundColor = ConsoleColor.Red;
            this.WriteLine(msg);
            this.ResetColor();
        }
    }

    /// <summary>
    /// options of protect
    /// </summary>
    [Serializable]
    internal class JieJieSwitchs : System.MarshalByRefObject
    {
        public JieJieSwitchs()
        {

        }
        public JieJieSwitchs(string args, JieJieSwitchs baseOptions)
        {
            if (baseOptions != null)
            {
                this.ControlFlow = baseOptions.ControlFlow;
                this.Resources = baseOptions.Resources;
                this.Strings = baseOptions.Strings;
            }
            if (args != null)
            {
                var items = args.Split(',');
                foreach (var item in items)
                {
                    var item2 = item.Trim().ToLower();
                    switch (item2)
                    {
                        case "+contorlfow": this.ControlFlow = true; break;
                        case "-controlfow": this.ControlFlow = false; break;
                        case "+strings": this.Strings = true; break;
                        case "-strings": this.Strings = false; break;
                        case "+resources": this.Resources = true; break;
                        case "-resources": this.Resources = false; break;
                        case "+allocationcallstack": this.AllocationCallStack = true; break;
                        case "-allocationcallstack": this.AllocationCallStack = false; break;
                        case "+memberorder": this.MemberOrder = true; break;
                        case "-memberorder": this.MemberOrder = false; break;
                        case "+rename": this.Rename = true; break;
                        case "-rename": this.Rename = false; break;
                        case "+removemember": this.RemoveMember = true; break;
                        case "-removemember": this.RemoveMember = false; break;
                    }
                }
            }
        }
        /// <summary>
        /// 混淆代码执行流程
        /// </summary>
        public bool ControlFlow = true;
        /// <summary>
        /// 加密字符串
        /// </summary>
        public bool Strings = true;
        /// <summary>
        /// 加密资源文件
        /// </summary>
        public bool Resources = true;
        /// <summary>
        /// 隐藏字符串创建调用堆栈
        /// </summary>
        public bool AllocationCallStack = false;
        /// <summary>
        /// 混淆类型成员顺序
        /// </summary>
        public bool MemberOrder = true;
        /// <summary>
        /// 重命名
        /// </summary>
        public bool Rename = true;
        /// <summary>
        /// 删除无作用的类型成员
        /// </summary>
        public bool RemoveMember = true;
    }
     
    [Serializable]
    internal class DCJieJieNetEngine : System.MarshalByRefObject, IDisposable
    {
        public const string ProductVersion = "1.2022.2.4";


        public DCJieJieNetEngine(DCILDocument doc)
        {
            this.Document = doc;
        }
        public DCJieJieNetEngine()
        {
        }
        /// <summary>
        /// 设置命令行界面操作接口
        /// </summary>
        /// <param name="instance">对象实例</param>
        public void SetConsoleInstance(MyConsole instance)
        {
            MyConsole.SetInstance(instance);
        }
         
#if ! DOTNETCORE
        public void BindCurrentDomain_AssemblyResolve()
        {
            System.AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmName = typeof(JieJieProject).Assembly.GetName().Name;
            if (args.Name.StartsWith(asmName))
            {
                return typeof(JieJieProject).Assembly;
            }
            return null;
        }

#endif

        /// <summary>
        ///  复制系统配置
        /// </summary>
        /// <param name="eng">复制输出对象</param>
        public void CopytSettingsTo(DCJieJieNetEngine eng)
        {
            eng.ContentEncoding = this.ContentEncoding;
            eng.DebugMode = this.DebugMode;
            eng.PrefixForTypeRename = this.PrefixForTypeRename;
            eng.PrefixForMemberRename = this.PrefixForMemberRename;
            eng.SDKDirectory = this.SDKDirectory;
            eng.SnkFileName = this.SnkFileName;
            if (this.Switchs != null)
            {
                eng.Switchs.AllocationCallStack = this.Switchs.AllocationCallStack;
                eng.Switchs.ControlFlow = this.Switchs.ControlFlow;
                eng.Switchs.MemberOrder = this.Switchs.MemberOrder;
                eng.Switchs.Rename = this.Switchs.Rename;
                eng.Switchs.Resources = this.Switchs.Resources;
                eng.Switchs.Strings = this.Switchs.Strings;
            }
            eng.TempDirectory = this.TempDirectory;
            eng._UILanguageDisplayName = this._UILanguageDisplayName;
            eng._UILanguageName = this._UILanguageName;
            eng.DeleteTempFile = this.DeleteTempFile;
            eng.OutpuptMapXml = this.OutpuptMapXml;
            eng.ResourceNameNeedEncrypt = this.ResourceNameNeedEncrypt;
        }
        /// <summary>
        /// 关闭对象
        /// </summary>
        public void Dispose()
        {
            DCILTypeReference.ClearGlobalBuffer();

            DCUtils.ClearStringTable();
            if (this.Document != null)
            {
                this.Document.Dispose();
                this.Document = null;
            }
            if (this._AllClasses != null)
            {
                this._AllClasses.Clear();
                this._AllClasses = null;
            }
            if (this._Type_JIEJIEHelper != null)
            {
                this._Type_JIEJIEHelper.LocalClass?.Dispose();
                this._Type_JIEJIEHelper.Dispose();
                this._Type_JIEJIEHelper = null;
            }
            this.Switchs = null;
            this._CallOperCodes?.Clear();
            this._CallOperCodes = null;
            this._AllClasses?.Clear();
            //this._AllBaseTypes.Clear();
            //this._IDGenForClass = null;
            _NativeTypeMethods.Clear();
            _Native_BaseMethods.Clear();
            //this._RuntimeSwitchs.Clear();
            if (this._Int32ValueContainer != null)
            {
                this._Int32ValueContainer.Dispose();
                this._Int32ValueContainer = null;
            }
            if (this._RFHContainer != null)
            {
                this._RFHContainer.Dispose();
                this._RFHContainer = null;
            }
            if (this._ByteDataContainer != null)
            {
                this._ByteDataContainer.Dispose();
                this._ByteDataContainer = null;
            }
            if (this._Int32ValueString != null)
            {
                this._Int32ValueString.Dispose();
                this._Int32ValueString = null;
            }

            DCILTypeReference.ClearCacheNativeTypes();
            if (this._Cache_BaseMethods != null)
            {
                foreach (var item in this._Cache_BaseMethods)
                {
                    item.Key.Dispose();
                    foreach (var m in item.Value)
                    {
                        m.Dispose();
                    }
                    item.Value.Clear();
                }
                this._Cache_BaseMethods.Clear();
                this._Cache_BaseMethods = null;
            }
            if (this._CallOperCodes != null)
            {
                this._CallOperCodes.Clear();
                this._CallOperCodes = null;
            }
            this._NewPNameGen = null;
            this.ContentEncoding = null;
            this.SnkFileName = null;
            this._InputAssemblyDirectory = null;
            this._InputAssemblyFileName = null;
            this.TempDirectory = null;
            DCILMethod.ClearNewLabelIDCache();
            GC.Collect();
        }
        /// <summary>
        /// 删除临时目录
        /// </summary>
        public void DeleteTemplateDirecotry()
        {
            if (this.TempDirectory != null
                && this.TempDirectory.Length > 0
                && Directory.Exists(this.TempDirectory))
            {
                ConsoleWriteTask();
                MyConsole.Instance.Write("Deleting template directory : " + this.TempDirectory);
                DCUtils.CleanDirectory(this.TempDirectory);
                Directory.Delete(this.TempDirectory, true);
                this.TempDirectory = null;
            }
        }

        private bool WriteDocumentCommentXml(string descFileName)
        {
            try
            {
                int tick = Environment.TickCount;
                var names = new SortedDictionary<string, string>();
                foreach (var cls in this.GetAllClasses())
                {
                    if (cls.RenameState == DCILRenameState.Renamed)
                    {
                        continue;
                    }
                    if (cls.Parent is DCILClass)
                    {
                        var npc = (DCILClass)cls.Parent;
                        if (npc.RenameState == DCILRenameState.Renamed)
                        {
                            continue;
                        }
                    }
                    var ms = new Dictionary<string, DCILObject>();
                    var tn = cls.NameWithNested;
                    names["T:" + tn] = tn;
                    foreach (var m in cls.ChildNodes)
                    {
                        if (m is DCILMethod)
                        {
                            var m2 = (DCILMethod)m;
                            if (m2.RenameState == DCILRenameState.Renamed)
                            {
                                continue;
                            }
                            if (m2.IsSpecialname)
                            {
                                if (m2.Name.StartsWith("get_") || m2.Name.StartsWith("set_"))
                                {
                                    continue;
                                }
                            }
                            if (m2.HasStyle("private") || m2.HasStyle("assembly"))
                            {
                                continue;
                            }
                            var str = new StringBuilder();
                            var writer = new DCILWriter(str);
                            if (m2.Name == ".ctor" || m2.Name == ".cctor")
                            {
                                str.Append("M:" + tn + ".#" + m2.Name.Substring(1));
                            }
                            else
                            {
                                str.Append("M:" + tn + "." + m2.Name);
                            }
                            if (m2.ParametersCount > 0)
                            {
                                str.Append("(");
                                for (int iCount = 0; iCount < m2.Parameters.Count; iCount++)
                                {
                                    if (iCount > 0)
                                    {
                                        str.Append(',');
                                    }
                                    var p = m2.Parameters[iCount];
                                    //if (p.IsOut)
                                    //{
                                    //    str.Append("out ");
                                    //}
                                    if (p.ValueType.IsPrimitive && p.ValueType.NativeType != null)
                                    {
                                        str.Append(p.ValueType.NativeType.FullName);
                                    }
                                    else
                                    {
                                        p.ValueType.WriteToForSignString(writer, m2.GenericParamters);
                                    }
                                }
                                str.Append(")");
                            }
                            var strSign = str.ToString();
                            names[strSign] = strSign;
                        }
                        else if (m is DCILField)
                        {
                            var f = (DCILField)m;
                            if (f.RenameState == DCILRenameState.Renamed)
                            {
                                continue;
                            }
                            if (f.Name == "value__" && cls.IsEnum)
                            {
                                continue;
                            }
                            if (f.HasStyle("private") || f.HasStyle("assembly"))
                            {
                                continue;
                            }
                            names["F:" + tn + "." + m.Name] = m.Name;
                        }
                        else if (m is DCILProperty)
                        {
                            var p = (DCILProperty)m;
                            if (p.RenameState == DCILRenameState.Renamed)
                            {
                                continue;
                            }
                            names["P:" + tn + "." + p.Name] = p.Name;
                        }
                        else if (m is DCILEvent)
                        {
                            var p = (DCILEvent)m;
                            if (p.RenameState == DCILRenameState.Renamed)
                            {
                                continue;
                            }
                            names["E:" + tn + "." + m.Name] = m.Name;
                        }
                    }
                }

                int removeCount = 0;
                int totalMembers = 0;
                using (var writer = new System.Xml.XmlTextWriter(descFileName, Encoding.UTF8))
                {
                    writer.Formatting = System.Xml.Formatting.Indented;
                    writer.Indentation = 3;
                    writer.IndentChar = ' ';
                    writer.WriteStartDocument();
                    writer.WriteStartElement("doc");
                    foreach (System.Xml.XmlNode node1 in this.Document.CommentXmlDoc.DocumentElement.ChildNodes)
                    {
                        if (node1.Name == "members")
                        {
                            writer.WriteStartElement("members");
                            foreach (System.Xml.XmlNode node2 in node1.ChildNodes)
                            {
                                if (node2 is System.Xml.XmlElement)
                                {
                                    totalMembers++;
                                    var e2 = (System.Xml.XmlElement)node2;
                                    string name = e2.GetAttribute("name");
                                    if (names.ContainsKey(name))
                                    {
                                        node2.WriteTo(writer);
                                    }
                                    else
                                    {
                                        removeCount++;
                                    }
                                }
                            }
                            writer.WriteFullEndElement();//</members>
                        }
                        else
                        {
                            node1.WriteTo(writer);
                        }
                    }
                    writer.WriteEndElement();//</doc>
                    writer.WriteEndDocument();
                }//using
                tick = Math.Abs(Environment.TickCount - tick);
                var percent = Convert.ToDouble(removeCount * 100.0 / totalMembers).ToString("0.00");
                ConsoleWriteTask();
                MyConsole.Instance.WriteLine("Write comment XML file \"" + descFileName + "\" ,remove " + removeCount + "(" + percent + "%)members,span " + tick + " milliseconds.");
                return true;
            }
            catch (System.Exception ext)
            {
                MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                MyConsole.Instance.WriteLine(ext.ToString());
                MyConsole.Instance.ResetColor();
            }
            return false;
        }

        public static void ConsoleTranslateStackTraceUseMapXml(string mapXmlFileName)
        {
            if (mapXmlFileName == null || mapXmlFileName.Length == 0)
            {
                return;
            }

            if (File.Exists(mapXmlFileName) == false)
            {
                MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                MyConsole.Instance.WriteLine("Can not find file " + mapXmlFileName);
                MyConsole.Instance.ResetColor();
                return;
            }

            MyConsole.Instance.WriteLine("JIEJIE.NET translate stack trace use MAP.XML ");
            MyConsole.Instance.WriteLine("MAP xml file : " + mapXmlFileName);
            MyConsole.Instance.Write("Please paste or input stack trace text and press ");
            MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
            MyConsole.Instance.BackgroundColor = ConsoleColor.Black;
            MyConsole.Instance.Write("ESC");
            MyConsole.Instance.ResetColor();
            MyConsole.Instance.WriteLine(" to finish:");
            var strBuffer = new System.Text.StringBuilder();
            while (true)
            {
                var info = MyConsole.Instance.ReadKey();
                if (info.Key == ConsoleKey.Escape)
                {
                    break;
                }
                if (info.KeyChar > 7)
                {
                    strBuffer.Append(info.KeyChar.ToString());
                    if (info.Key == ConsoleKey.Enter)
                    {
                        MyConsole.Instance.WriteLine();
                    }
                }
            }
            var strResult = DCJieJieNetEngine.TranslateStackTraceUseMapXml(mapXmlFileName, strBuffer.ToString());
            MyConsole.Instance.WriteLine();
            MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
            MyConsole.Instance.WriteLine(" ########## Translate result ##########");
            ConoleWriteStackTrace(strResult);
            MyConsole.Instance.WriteLine("### Press Enter to exit. ###");
            MyConsole.Instance.ReadLine();
        }

        public static void ConoleWriteStackTrace( string strResult )
        {
            if(strResult == null || strResult.Length == 0 )
            {
                return;
            }
            MyConsole.Instance.ResetColor();
            var reader = new StringReader(strResult);
            string strLine = reader.ReadLine();
            while (strLine != null)
            {
                int index = strLine.IndexOf('(');
                if (index > 0)
                {
                    var strMethodName = strLine.Substring(0, index);
                    int index2 = strMethodName.LastIndexOf('.');
                    if (index2 > 0)
                    {
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                        MyConsole.Instance.Write(strMethodName.Substring(0, index2));
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                        MyConsole.Instance.Write(strMethodName.Substring(index2));
                    }
                    else
                    {
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                        MyConsole.Instance.Write(strLine.Substring(0, index));
                    }
                    MyConsole.Instance.ResetColor();
                    MyConsole.Instance.Write(strLine.Substring(index));
                    MyConsole.Instance.WriteLine();
                }
                else
                {
                    MyConsole.Instance.WriteLine(strLine);
                }
                strLine = reader.ReadLine();
            }
            reader.Close();
        }

        public static string TranslateStackTraceUseMapXml(string mapXmlFileName, string strSourctStackTrace)
        {
            if (strSourctStackTrace == null || strSourctStackTrace.Length == 0)
            {
                return null;
            }
            if (mapXmlFileName == null || mapXmlFileName.Length == 0)
            {
                throw new ArgumentNullException("mapXmlFileName");
            }
            if (File.Exists(mapXmlFileName) == false)
            {
                throw new FileNotFoundException(mapXmlFileName);
            }
            var doc = new System.Xml.XmlDocument();
            doc.Load(mapXmlFileName);
            //var maps = new Dictionary<string, string>();
            var maps = new Dictionary<string, List<MethodMapInfo>>();
            var clsMaps = new Dictionary<string, string>();
            var shortClsMaps = new Dictionary<string, string>();
            foreach (System.Xml.XmlNode node in doc.DocumentElement)
            {
                if (node.Name == "method")
                {
                    var e = (System.Xml.XmlElement)node;
                    var newName = e.GetAttribute("newname");
                    if (newName != null
                        && newName.Length > 0)
                    {
                        List<MethodMapInfo> infos = null;
                        if (maps.TryGetValue(newName, out infos) == false)
                        {
                            infos = new List<MethodMapInfo>();
                            maps[newName] = infos;
                        }
                        var info = new MethodMapInfo();
                        info.OldSign = e.GetAttribute("oldsign");
                        info.NewShortParamters = e.GetAttribute("newshort");
                        var newSign = e.GetAttribute("newsign");
                        info.NewParamters = newSign.Substring(newName.Length);
                        infos.Add(info);
                    }
                }
                else if (node.Name == "class")
                {
                    var e = (System.Xml.XmlElement)node;
                    var oldName = e.GetAttribute("oldname");
                    var newName = e.GetAttribute("newname");
                    var sn = DCUtils.GetShortName(newName);
                    shortClsMaps[sn] = oldName;
                    clsMaps[newName] = oldName;
                }
            }//foreach

            var doc3 = new DCILDocument();
            var reader = new DCILReader(strSourctStackTrace, doc3);
            var strResult = new System.Text.StringBuilder();
            while (reader.HasContentLeft())
            {
                var strWord = reader.ReadWord();
                if (strWord == null)
                {
                    break;
                }
                if (maps.ContainsKey(strWord) == false)
                {
                    string clsName = null;
                    string methodName = null;
                    if (strWord.IndexOf('.') > 0 && reader.PeekContentChar() == '(')
                    {
                        methodName = DCUtils.GetShortName(strWord, out clsName);
                    }
                    else if (reader.PeekContentChar() == ':')
                    {
                        if (reader.ReadContentChar() == ':' && reader.Read() == ':')
                        {
                            clsName = strWord;
                            methodName = reader.ReadWord();
                        }
                    }
                    if (clsName != null && methodName != null)
                    {
                        if (maps.ContainsKey(clsName + "." + methodName))
                        {
                            strWord = clsName + "." + methodName;
                        }
                        else
                        {
                            if (clsMaps.ContainsKey(clsName))
                            {
                                strResult.Append(clsMaps[clsName]);
                            }
                            else
                            {
                                strResult.Append(clsName);
                            }
                            strResult.Append('.');
                            strResult.Append(methodName);
                            int lineIndex = reader.CurrentLineIndex();
                            TranslateMethodParameter(reader, strResult, clsMaps, shortClsMaps);
                            if (lineIndex == reader.CurrentLineIndex())
                            {
                                strResult.Append(reader.ReadLineTrim());
                            }
                            strResult.AppendLine();
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                if (maps.ContainsKey(strWord))
                {
                    // 直接命中名称
                    var infos = maps[strWord];
                    var strMethodName = strWord;
                    var strPs = new StringBuilder();

                    while (reader.HasContentLeft())
                    {
                        strWord = reader.ReadWord();
                        if (strWord == ")")
                        {
                            strPs.Append(strWord);
                            //reader.MoveNextLine();
                            break;
                        }
                        else if (strWord == "(")
                        {
                            strPs.Append(strWord);
                            string pvt = null;
                            while (reader.HasContentLeft())
                            {
                                strWord = reader.ReadWord();
                                if (strWord == ")")
                                {
                                    strPs.Append(strWord);
                                    break;
                                }
                                if (pvt == null)
                                {
                                    pvt = strWord;
                                    strPs.Append(strWord);
                                }
                                else if (strWord == ",")
                                {
                                    strPs.Append(strWord);
                                    pvt = null;
                                }
                            }
                            break;
                        }
                        strPs.Append(strWord);
                    }
                    var strPsList = strPs.ToString();
                    bool find = false;
                    foreach (var info in infos)
                    {
                        if (strPsList == info.NewParamters || strPsList == info.NewShortParamters)
                        {
                            strResult.Append(info.OldSign);
                            find = true;
                            break;
                        }
                    }//foreach
                    if (find == false)
                    {
                        strResult.Append(infos[0].OldSign + "[!! Maby !!]");
                    }
                    strResult.Append(reader.ReadLineTrim());
                    strResult.AppendLine();
                }
            }//while
            var strResultStr = strResult.ToString();
            return strResultStr;
        }
        /// <summary>
        /// 翻译函数的参数信息
        /// </summary>
        /// <param name="reader">文本读取器</param>
        /// <param name="result">转换结果输出</param>
        /// <param name="clsMaps">类全名映射表</param>
        /// <param name="shortClsMaps">类短名称映射表</param>
        private static void TranslateMethodParameter(
            DCILReader reader,
            StringBuilder result,
            Dictionary<string, string> clsMaps,
            Dictionary<string, string> shortClsMaps)
        {
            string currentType = null;
            int lineIndex = reader.CurrentLineIndex();
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == ")" || lineIndex != reader.CurrentLineIndex())
                {
                    result.Append(strWord);
                    break;
                }
                else if (strWord == "<" || strWord == ">" || strWord == "(" || strWord == "[" || strWord == "]" || strWord == "&")
                {
                    result.Append(strWord);
                }
                else if (strWord == ",")
                {
                    result.Append(strWord);
                    currentType = null;
                }
                else
                {
                    if (currentType == null)
                    {
                        currentType = strWord;
                        string oldType = null;
                        if (clsMaps.TryGetValue(currentType, out oldType))
                        {
                            result.Append(oldType);
                        }
                        else if (shortClsMaps.TryGetValue(currentType, out oldType))
                        {
                            result.Append(oldType);
                        }
                        else
                        {
                            result.Append(currentType);
                        }
                    }
                    else
                    {
                        result.Append(' ');
                        result.Append(strWord);
                    }
                }
            }//while
            string text = result.ToString();

        }
        private class MethodMapInfo
        {
            public string OldSign = null;
            public string NewParamters = null;
            public string NewShortParamters = null;

        }


        public void WriteMapXml2(System.Xml.XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("jiejie.net.map");
            var clses = new List<DCILClass>(this.Document.GetAllClassesUseCache().Values);
            var maps = new SortedDictionary<string, string>();
            var methods = new List<DCILMethod>();
            foreach (var cls in clses)
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        var method = (DCILMethod)item;
                        if (method.RenameState == DCILRenameState.Renamed)
                        {
                            //if (method.OldName == "StaticEncrypt")
                            //{

                            //}
                            methods.Add(method);
                            //var oldSign = method.OldSignatureForMap;
                            //var newSign = method.GetSignatureForMap();
                            //if (oldSign != newSign)
                            //{
                            //    if(maps.ContainsKey( newSign ))
                            //    {

                            //    }
                            //    maps[newSign] = oldSign;
                            //}
                            //else
                            //{

                            //}
                        }
                    }
                }
            }
            methods.Sort(delegate (DCILMethod a, DCILMethod b)
            {
                return string.Compare(a.Name, b.Name, true);
            });
            writer.WriteAttributeString("methodCount", methods.Count.ToString());
            foreach (var method in methods)
            {
                writer.WriteStartElement("method");
                writer.WriteAttributeString("newsign", method.GetSignatureForMap());
                writer.WriteAttributeString("oldsign", method.OldSignatureForMap);
                writer.WriteAttributeString("newshort", method.GetParamterListString(true));
                writer.WriteAttributeString("newname", ((DCILClass)method.Parent).GetNameWithNested('.') + "." + method.Name);
                //if(method.Parameters != null &&  method.Parameters.Count > 0 )
                //{

                //}
                writer.WriteEndElement();
            }
            foreach (var cls in clses)
            {
                if (cls.RenameState == DCILRenameState.Renamed)
                {
                    writer.WriteStartElement("class");
                    writer.WriteAttributeString("newname", cls.GetNameWithNested('.', false));
                    writer.WriteAttributeString("oldname", cls.GetNameWithNested('.', true));
                    //writer.WriteAttributeString("newshort", DCUtils.GetShortName(cls.Name));
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
        
        private static string _PathOfildasm = null;
        /// <summary>
        /// 是否输出新旧名称对应XML文件
        /// </summary>
        public bool OutpuptMapXml = false;

        /// <summary>
        /// 保存程序集文件
        /// </summary>
        /// <param name="asmFileName">程序集文件</param>
        /// <param name="checkUseNgen">是否使用 Ngen.exe 进行验证</param>
        /// <returns>操作是否成功</returns>
        public bool SaveAssemblyFile(string asmFileName, bool checkUseNgen)
        {
            //this.Document.DisplayMethodRefCount();

            if (asmFileName == null || asmFileName.Length == 0)
            {
                throw new ArgumentNullException("asmFileName");
            }
            if (Directory.Exists(Path.GetDirectoryName(asmFileName)) == false)
            {
                throw new DirectoryNotFoundException(Path.GetDirectoryName(asmFileName));
            }
            ConsoleWriteTask();
            MyConsole.Instance.WriteLine("Saving assembly to " + asmFileName);
            var ilFileName = Path.Combine(this.Document.RootPath, "result_" + Path.GetFileName(asmFileName) + ".il");
            MyConsole.Instance.WriteLine("    Writing IL codes to " + ilFileName);
            this.Document.WriteToFile(ilFileName, this.ContentEncoding);
            if (_PathOfildasm == null)
            {
                _PathOfildasm = Path.GetDirectoryName(typeof(string).Assembly.Location);
#if DOTNETCORE
                if ( File.Exists( Path.Combine( _PathOfildasm , "ilasm.exe")) == false )
                {
                    _PathOfildasm = null;
                    if (Directory.Exists(@"C:\Windows\Microsoft.NET\Framework64"))
                    {
                        _PathOfildasm = @"C:\Windows\Microsoft.NET\Framework64";
                    }
                    else if( Directory.Exists(@"C:\Windows\Microsoft.NET\Framework"))
                    {
                        _PathOfildasm = @"C:\Windows\Microsoft.NET\Framework";
                    }
                    if( _PathOfildasm != null )
                    {
                        var list = new List<string>(Directory.GetDirectories(_PathOfildasm));
                        _PathOfildasm = null;
                        list.Sort();
                        for( int iCount = list.Count -1; iCount >=0;iCount -- )
                        {
                            var item = Path.GetFileName( list[iCount]);
                            if( item.Length > 4 && item[0]=='v' && char.IsDigit( item[1]))
                            {
                                _PathOfildasm = list[iCount];
                                break;
                            }
                        }
                    }
                    if( _PathOfildasm == null )
                    {
                        throw new Exception("Can not find ilasm.exe path.");
                    }
                }
#endif
            }

            var asmTempFileName = Path.Combine(this.TempDirectory, "Temp_" + Path.GetFileName(asmFileName));
            if (File.Exists(asmTempFileName))
            {
                File.Delete(asmTempFileName);
            }
            // 生成临时程序集文件
            var ilAsmArgs = "\"" + ilFileName + "\"  \"/output:" + asmTempFileName + "\"";
            if (asmTempFileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                ilAsmArgs = ilAsmArgs + " /exe";
            }
            else
            {
                ilAsmArgs = ilAsmArgs + " /dll";
            }
            if (this.Document.Win32ResData != null && this.Document.Win32ResData.Length > 0)
            {
                var resFileName = Path.ChangeExtension(ilFileName, ".res");
                if (File.Exists(resFileName) == false)
                {
                    File.WriteAllBytes(resFileName, this.Document.Win32ResData);
                }
                ilAsmArgs = ilAsmArgs + " \"/resource=" + resFileName + "\"";
            }
            if (this.Document.RuntimeVersion != null && this.Document.RuntimeVersion.Length > 0)
            {
                ilAsmArgs = ilAsmArgs + " /mdv=" + this.Document.RuntimeVersion + " /msv:" + this.Document.RuntimeVersion;
            }
            if (this.DebugMode == false)
            {
                ilAsmArgs = ilAsmArgs + " /quiet";
            }
            DCUtils.RunExe(Path.Combine(_PathOfildasm, "ilasm.exe"), ilAsmArgs);
            //if (this.DebugMode)
            //{
            //    ResourceFileHelper.RunExe(Path.Combine(_PathOfildasm, "ilasm.exe"), "\"" + ilFileName + "\" /dll  \"/output:" + asmTempFileName + "\"");
            //}
            //else
            //{
            //    ResourceFileHelper.RunExe(Path.Combine(_PathOfildasm, "ilasm.exe"), "\"" + ilFileName + "\" /dll  \"/output:" + asmTempFileName + "\" /quiet");
            //}
            if (File.Exists(asmTempFileName))
            {
                if (this.SnkFileName != null && this.SnkFileName.Length > 0 && File.Exists(this.SnkFileName))
                {
                    DCUtils.RunExe(Path.Combine(this.SDKDirectory, "sn.exe"), "-Ra \"" + asmTempFileName + "\" " + this.SnkFileName);
                }
                if (checkUseNgen)
                {
                    ConsoleWriteTask();
#if !DOTNETCORE // .NET Core not support ngen.exe
                    MyConsole.Instance.WriteLine("Testing by ngen.exe...");
                    string pathNgen = Path.Combine(_PathOfildasm, "ngen.exe");
                    if (this.Document.Value_CorFlags > 0 && ((this.Document.Value_CorFlags & 2) == 2))
                    {
                        // 32位PE文件
                        if (_PathOfildasm.Contains("Framework64"))
                        {
                            pathNgen = _PathOfildasm.Replace("Framework64", "Framework");
                            pathNgen = Path.Combine(pathNgen, "ngen.exe");
                        }
                    }
                    if (File.Exists(pathNgen))
                    {
                        DCUtils.RunExe(pathNgen, "install \"" + asmTempFileName + "\"  /NoDependencies");
                        DCUtils.RunExe(pathNgen, "uninstall \"" + asmTempFileName + "\"");
                    }
                    else
                    {
                        MyConsole.Instance.WriteLine("can not find file : " + pathNgen);
                    }
#else
                    MyConsole.Instance.WriteLine("Testing by crossgen.exe...");
                    CrossGenHelper hp = new CrossGenHelper();
                    hp.TestByCrossGen(this.Document, asmTempFileName);
                    //Console.WriteLine(".NET Core not support ngen.exe");
#endif
                }
                File.Copy(asmTempFileName, asmFileName, true);
                File.Delete(asmTempFileName);
                if (this.Switchs.Rename && this.OutpuptMapXml)
                {
                    ConsoleWriteTask();
                    var fn2 = asmFileName + ".map.xml";
                    using (var writer = new System.Xml.XmlTextWriter(fn2, Encoding.UTF8))
                    {
                        writer.Formatting = System.Xml.Formatting.Indented;
                        writer.IndentChar = ' ';
                        writer.Indentation = 3;
                        this.WriteMapXml2(writer);
                    }
                    MyConsole.Instance.WriteLine("Write rename map xml to\"" + fn2 + "\".");
                }
                if (this.Document.Content_DepsJson != null && this.Document.Content_DepsJson.Length > 0)
                {
                    var fn2 = Path.ChangeExtension(asmFileName, ".deps.json");
                    MyConsole.Instance.WriteLine("    Write " + fn2);
                    File.WriteAllBytes(fn2, this.Document.Content_DepsJson);
                }
                if (this.Document.CommentXmlDoc != null && this.Document.CommentXmlDoc.DocumentElement?.Name == "doc")
                {
                    // clean document comment xml file.
                    var comXmlFileName = Path.ChangeExtension(asmFileName, ".xml");
                    WriteDocumentCommentXml(comXmlFileName);
                }

                MyConsole.Instance.WriteLine();
                ConsoleWriteTask();
                MyConsole.Instance.Write("Job finished, final save to :");
                MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                MyConsole.Instance.WriteLine(asmFileName);
                MyConsole.Instance.ResetColor();
                if (this._SourceAssemblyFileSize > 0)
                {
                    MyConsole.Instance.Write(" Source file size : " + DCUtils.FormatByteSize(this._SourceAssemblyFileSize) + " ,");
                }
                var newFileLength = new FileInfo(asmFileName).Length;
                MyConsole.Instance.WriteLine(" Result file size : " + DCUtils.FormatByteSize(newFileLength));

                return true;
            }
            else
            {
                ConsoleWriteTask();
                MyConsole.Instance.WriteLine("Job failed.");
                return false;
            }
        }
        private string _InputAssemblyFileName = null;
        private string _InputAssemblyDirectory = null;
        internal string _UseAnotherExeName = null;
        private int _SourceAssemblyFileSize = 0;

        /// <summary>
        /// 从一个临时目录中加载IL文件
        /// </summary>
        /// <param name="rootPath">临时目录</param>
        /// <returns>操作是否成功</returns>
        public bool LoadILFromTempPath(string rootPath)
        {
            if (rootPath == null || rootPath.Length == 0)
            {
                throw new ArgumentNullException("rootPath");
            }
            if (Directory.Exists(rootPath) == false)
            {
                throw new DirectoryNotFoundException(rootPath);
            }
            this._SourceAssemblyFileSize = 0;
            if (this.ContentEncoding == null)
            {
                this.ContentEncoding = Encoding.UTF8;
            }
            if (this.SDKDirectory == null
                || this.SDKDirectory.Length == 0
                || System.IO.Directory.Exists(this.SDKDirectory) == false)
            {
                this.SDKDirectory = DCUtils.GetSDKDir();
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write(" Loading document in IL format from '" + rootPath + "'.");
            var documents = new List<DCILDocument>();
            foreach (var subDir in Directory.GetDirectories(rootPath))
            {
                var sn = Path.GetFileName(subDir);
                var ilFileName = Path.Combine(subDir, sn + ".dll.il");
                string asmFileName = sn + ".dll";
                if (File.Exists(ilFileName) == false)
                {
                    ilFileName = Path.Combine(subDir, sn + ".exe.il");
                    asmFileName = sn + ".exe";
                }
                if (File.Exists(ilFileName))
                {
                    MyConsole.Instance.Write(Environment.NewLine + "      Loading '" + ilFileName + "' ...");
                    int tick = Environment.TickCount;
                    var doc = new DCILDocument();
                    doc.LoadByReader(ilFileName, this.ContentEncoding);
                    doc.AssemblyFileName = asmFileName;
                    documents.Add(doc);
                    tick = Math.Abs(tick - Environment.TickCount);
                    MyConsole.Instance.Write(" span " + tick + " millisenconds , get " + doc.Classes.Count + " classes.");
                }
            }
            if (documents.Count > 1)
            {
                // 根据程序集之间相互引用关系来获得主程序集对象
                var mainDocs = new List<DCILDocument>(documents);
                for (int iCount = mainDocs.Count - 1; iCount >= 0; iCount--)
                {
                    var mainDoc = mainDocs[iCount];
                    bool beRef = false;
                    foreach (var doc in documents)
                    {
                        if (doc != mainDoc)
                        {
                            foreach (var asm in doc.Assemblies)
                            {
                                if (asm.IsExtern && string.Compare(asm.Name, mainDoc.Name, true) == 0)
                                {
                                    beRef = true;
                                    break;
                                }
                            }
                        }
                        if (beRef)
                        {
                            break;
                        }
                    }
                    if (beRef)
                    {
                        mainDocs.RemoveAt(iCount);
                    }
                }
                if (mainDocs.Count > 0)
                {
                    documents.Remove(mainDocs[0]);
                    documents.Insert(0, mainDocs[0]);
                }
                MyConsole.Instance.WriteLine();
                ConsoleWriteTask();
                MyConsole.Instance.WriteLine("Merging assembly files ...");
                // 进行文档合并
                this.Document = DCILDocument.MergeDocuments(documents);
            }
            else
            {
                this.Document.UpdateCustomAttributeValues();
            }
            return true;
        }
        /// <summary>
        /// 获得文档程序集文件名
        /// </summary>
        /// <returns>文件名</returns>
        public string GetDocumentAssemblyFileName()
        {
            return this.Document?.AssemblyFileName;
        }
        /// <summary>
        /// 加载程序集文件
        /// </summary>
        /// <param name="asmFileName">程序集文件名</param>
        /// <returns>操作是否成功</returns>
        public bool LoadAssemblyFile(string asmFileName, string mergeAsmFileNames)
        {
            if (asmFileName == null || asmFileName.Length == 0)
            {
                throw new ArgumentNullException("asmFileName");
            }
            if (File.Exists(asmFileName) == false)
            {
                throw new FileNotFoundException(asmFileName);
            }
            this._SourceAssemblyFileSize = 0;
            if (this.ContentEncoding == null)
            {
                this.ContentEncoding = Encoding.UTF8;
            }
            if (this.SDKDirectory == null
                || this.SDKDirectory.Length == 0
                || System.IO.Directory.Exists(this.SDKDirectory) == false)
            {
                this.SDKDirectory = DCUtils.GetSDKDir();
            }
            if (Directory.Exists(this.TempDirectory) == false)
            {
                Directory.CreateDirectory(this.TempDirectory);
            }
            else
            {
                DCUtils.CleanDirectory(this.TempDirectory);
            }
            int tick = Environment.TickCount;
            //ConsoleWriteTask();
            string ilFileName = null;

            if (asmFileName.EndsWith(".il", StringComparison.OrdinalIgnoreCase))
            {
                // 已经存在 il 文件。
                ilFileName = asmFileName;
                var doc2 = new DCILDocument();
                doc2.LoadByReader(ilFileName, this.ContentEncoding);
                this.Document = doc2;
            }
            else
            {
                //this._SourceAssemblyFileSize = (int)(new FileInfo(asmFileName).Length);

                // 调用 ildasm.exe 将程序集文件反编译为 il 文件。
                var ildasmExeFileName = Path.Combine(this.SDKDirectory, "ildasm.exe");
                //Console.WriteLine("Loading main assembly file " + asmFileName);
                var mafns = new List<string>();
                if (mergeAsmFileNames != null)
                {
                    foreach (var item in mergeAsmFileNames.Split(';'))
                    {
                        var item2 = item.Trim();
                        if (item2.Length > 0)
                        {
                            mafns.Add(item2);
                        }
                    }
                }
                var documents = new List<DCILDocument>();
                LoadAssemblyDocuments(documents, asmFileName, ildasmExeFileName, mafns.Count > 0 ? mafns.ToArray() : null);
                this._SourceAssemblyFileSize = 0;
                foreach (var doc in documents)
                {
                    this._SourceAssemblyFileSize += doc.FileSize;
                }
                if (this._UseAnotherExeName != null)
                {
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                    MyConsole.Instance.BackgroundColor = ConsoleColor.Black;
                    MyConsole.Instance.WriteLine();
#if DOTNETCORE
                    MyConsole.Instance.WriteLine("   Warring!!! This program not support .NET Framework,please use '" + this._UseAnotherExeName + "'.");
#else
                    MyConsole.Instance.WriteLine("   Warring!!! This program not support .NET Core,please use '" + this._UseAnotherExeName + "'.");
#endif
                    MyConsole.Instance.ResetColor();
                    return false;
                }
                if (documents.Count > 1)
                {
                    ConsoleWriteTask();
                    MyConsole.Instance.WriteLine("Merging assembly files ...");
                    // 进行文档合并
                    this.Document = DCILDocument.MergeDocuments(documents);
                }
                else
                {
                    this.Document = documents[0];
                    this.Document.UpdateCustomAttributeValues();
                }
                var fileDepsJson = Path.ChangeExtension(asmFileName, "deps.json");
                if (File.Exists(fileDepsJson))
                {
                    this.Document.Content_DepsJson = File.ReadAllBytes(fileDepsJson);
                }
                this._InputAssemblyFileName = asmFileName;
                this._InputAssemblyDirectory = Path.GetDirectoryName(this._InputAssemblyFileName);
                //if(EventAfterLoadAssembly != null )
                //{
                //    EventAfterLoadAssembly(this, null);
                //}
            }
            return true;
        }

        //public string SpecifyOutputPath = null;

        //public static EventHandler EventAfterLoadAssembly = null;

        /// <summary>
        /// 加载程序集文档
        /// </summary>
        /// <param name="documents">文档列表</param>
        /// <param name="asmFileName">程序集文件名</param>
        /// <param name="ildasmExeFileName">ildasm可执行文件名</param>
        /// <param name="mergeAsmFileNames">合并的程序集文件名数组</param>
        private void LoadAssemblyDocuments(
            List<DCILDocument> documents,
            string asmFileName,
            string ildasmExeFileName,
            string[] mergeAsmFileNames)
        {
            string sfn = Path.GetFileNameWithoutExtension(asmFileName);
            foreach (var item in documents)
            {
                if (string.Compare(sfn, Path.GetFileNameWithoutExtension(item.AssemblyFileName), true) == 0)
                {
                    // 已经加载了相同名称的程序集，不重复加载。
                    return;
                }
            }
            var outputPath = Path.Combine(this.TempDirectory, Path.GetFileNameWithoutExtension(asmFileName));
            if (Directory.Exists(outputPath) == false)
            {
                Directory.CreateDirectory(outputPath);
            }
            ConsoleWriteTask();
            MyConsole.Instance.WriteLine("Loading assembly file " + asmFileName);
            var ilFileName = Path.Combine(outputPath, Path.GetFileName(asmFileName) + ".il");
            DCUtils.RunExe(
                    ildasmExeFileName,
                    "\"" + asmFileName + "\" /forward /UTF8 \"/output=" + ilFileName + "\" /nobar");
            string rootDir = Path.GetDirectoryName(asmFileName);
            foreach (var dir in Directory.GetDirectories(rootDir))
            {
                // 反编译资源DLL文件
                var resDllFileName = Path.Combine(
                        dir,
                        Path.GetFileNameWithoutExtension(asmFileName) + DCILMResource.EXT_Resources + ".dll");
                if (File.Exists(resDllFileName))
                {
                    var tempFileName = Path.Combine(outputPath, Path.GetFileName(dir));
                    if (Directory.Exists(tempFileName) == false)
                    {
                        Directory.CreateDirectory(tempFileName);
                    }
                    tempFileName = Path.Combine(tempFileName, Path.GetFileNameWithoutExtension(resDllFileName) + ".il");
                    DCUtils.RunExe(
                        ildasmExeFileName,
                        "\"" + resDllFileName + "\" /forward /UTF8 \"/output=" + tempFileName + "\"");
                }
            }
            int tick = Environment.TickCount;
            //ConsoleWriteTask();
            MyConsole.Instance.Write("    Anlysing IL file...");
            using (var reader = new System.IO.StreamReader(ilFileName, Encoding.UTF8, true))
            {
                // 检查.NET库类型
                var line = reader.ReadLine();
                string strHeader = ".assembly extern";
                int lineCount = 0;
                while (line != null && lineCount < 1000)
                {
                    lineCount++;
                    line = line.Trim();
                    if (line.StartsWith(strHeader, StringComparison.Ordinal))
                    {
                        var asmName = line.Substring(strHeader.Length).Trim();
#if DOTNETCORE
                        if( asmName == "mscorlib")
                        {
                            this._UseAnotherExeName = "JieJie.Net.exe";
                            return ;
                        }
                        else
                        {
                            break;
                        }

#else
                        if (asmName == "System.Runtime")
                        {
                            this._UseAnotherExeName = "JieJie.NETForCore.exe";
                            return;
                        }
                        else
                        {
                            break;
                        }
#endif
                    }
                    line = reader.ReadLine();
                }
            }

            var doc = new DCILDocument();
            doc.LoadByReader(ilFileName, this.ContentEncoding);
            doc.AssemblyFileName = asmFileName;
            doc.FileSize = (int)(new System.IO.FileInfo(asmFileName).Length);
            doc.RootPath = Path.GetDirectoryName(ilFileName);
            doc.FileName = ilFileName;
            var xmlFileName = Path.ChangeExtension(asmFileName, ".XML");
            if (File.Exists(xmlFileName))
            {
                try
                {
                    var xml = new System.Xml.XmlDocument();
                    xml.Load(xmlFileName);
                    doc.CommentXmlDoc = xml;
                }
                catch (System.Exception ext)
                {
                    MyConsole.Instance.WriteLine("      Fail to load " + xmlFileName + " . " + ext.Message);
                }
            }
            MyConsole.Instance.WriteLine(" span " + Math.Abs(Environment.TickCount - tick) + " milliseconds. get " + doc.Classes.Count + " classes.");
            documents.Add(doc);
            if (mergeAsmFileNames != null && mergeAsmFileNames.Length > 0)
            {
                // 准备合并程序集
                string asmBasePath = Path.GetDirectoryName(asmFileName);
                foreach (var mafn in mergeAsmFileNames)
                {
                    if (mafn == "*")
                    {
                        // 遇到通配符 * ，表示加载所有引用的程序集
                        foreach (var asm in doc.Assemblies)
                        {
                            if (asm.IsExtern)
                            {
                                if (DCUtils.IsSystemAsseblyName(asm.Name))
                                {
                                    // 不加载系统DLL
                                    continue;
                                }
                                var fn2 = Path.Combine(asmBasePath, asm.Name + ".dll");
                                if (File.Exists(fn2) == false)
                                {
                                    fn2 = Path.Combine(asmBasePath, asm.Name + ".exe");
                                }
                                if (File.Exists(fn2))
                                {
                                    LoadAssemblyDocuments(documents, fn2, ildasmExeFileName, mergeAsmFileNames);
                                    if (this._UseAnotherExeName != null)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 参数中指明了要合并的程序集文件名
                        var fn = mafn;
                        if (Path.IsPathRooted(fn) == false)
                        {
                            fn = Path.Combine(asmBasePath, fn);
                        }
                        if (File.Exists(fn))
                        {
                            LoadAssemblyDocuments(documents, fn, ildasmExeFileName, mergeAsmFileNames);
                            if (this._UseAnotherExeName != null)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void HandleDocument()
        {
            this.UpdateRuntimeSwitchs();

            this._CallOperCodes = new List<DCILOperCode_HandleMethod>();
            this.SelectUILanguage();
            var allMethods = this.Document.GetAllMethodHasOperCodes();
            this.AddClassJIEJIEHelper();
            if (this.Switchs.Resources)
            {
                // 加密资源包装类型
                this.ApplyResouceContainerClass();
            }
            // 加密内嵌程序集资源
            this.EncryptEmbeddedResource(allMethods);
            if (this.Switchs.ControlFlow)
            {
                // 加密部分字符数值
                this.EncryptCharValue(allMethods);
            }
            // 加密数组定义
            this.Encrypt_ArrayDefine(allMethods);
            // 加密 lock()/using() 结构。
            this.Encrypt_Lock_Using_Structure(allMethods);
            var clses = new List<DCILClass>(this.Document.Classes);
            foreach (var cls in clses)
            {
                HandleClass(cls);
            }
            if (this.Switchs.ControlFlow)
            {
                // 加密typeof()结构
                this.EncryptTypeHandle(allMethods);
                // 加密枚举类型的方法参数数据类型
                this.EncryptMethodParamterEnumValue();
            }
            if (this.Switchs.Strings)
            {
                // 加密字符串
                this.EncryptStringValues(allMethods);
                //this.HandleCollectStringValue();
            }
            this.AddDatasClass();
            this._RFHContainer?.Commit( this );
            this.Document.FixDomState();
            if (this.Switchs.ControlFlow)
            {
                // 加密流程
                this.ObfuscateControlFlow();
                if (this._CallOperCodes != null && this._CallOperCodes.Count > 0)
                {
                    ChangeCallOperCodes(this._CallOperCodes);
                    this._CallOperCodes.Clear();
                    this._CallOperCodes = null;
                }
            }
            if (this.Switchs.MemberOrder)
            {
                //DCUtils.ObfuseListOrder(this.Document.Classes);
            }

            bool hasRenamed = false;
            if (this.Switchs.Rename)
            {
                hasRenamed = this.RenameClasses() > 0;
            }
            if (hasRenamed || this.Document._HasMergeDocuments)
            {
                var attrs = this.Document.GetAllCustomAttributesUseCache();
                foreach (var attr in attrs)
                {
                    attr.UpdateBinaryValueForLocalClassRename();
                }
            }
            if (this.Switchs.ControlFlow)
            {
                this.CollectStatcMethod();
                //this.ObfuscateControlFlow();
                //if (this._CallOperCodes != null && this._CallOperCodes.Count > 0)
                //{
                //    ChangeCallOperCodes(this._CallOperCodes);
                //    this._CallOperCodes.Clear();
                //    this._CallOperCodes = null;
                //}
                if (this._Int32ValueContainer != null)
                {
                    this._Int32ValueContainer.Commit( this );
                    DCUtils.ObfuseListOrder(this._Int32ValueContainer._Class.ChildNodes);
                }
                //if (this._RFHContainer != null)
                //{
                //    ObfuscateMethodOperCodes(this._RFHContainer._Class.Method_Cctor);

                //    //ObfuscateOperCodeList(
                //    //    this._RFHContainer._Class.Method_Cctor,
                //    //    this._RFHContainer._Class.Method_Cctor.OperCodes,
                //    //    false,
                //    //    null);
                //}
            }
            
            if (this.Switchs.RemoveMember)
            {
                RemoveMember();
            }
            foreach (var asm in this.Document.Assemblies)
            {
                if (asm.IsExtern == false && asm.HasCustomAttributes)
                {
                    for (int iCount = asm.CustomAttributes.Count - 1; iCount >= 0; iCount--)
                    {
                        if (asm.CustomAttributes[iCount].AttributeTypeName
                            == "System.Runtime.CompilerServices.InternalsVisibleToAttribute")
                        {
                            asm.CustomAttributes.RemoveAt(iCount);
                        }
                    }
                    break;
                }
            }
        }
        private bool ChangeEnumParameterToInteger(DCILMethod method)
        {
            if (method != null && method.HasGenericStyle == false)
            {
                if (method.OperCodes != null && method.OperCodes.Count < 20)
                {
                    bool changed = true;
                    foreach (var item2 in method.OperCodes)
                    {
                        if (item2 is DCILOperCode_HandleMethod || item2 is DCILOperCode_Try_Catch_Finally)
                        {
                            changed = false;
                            break;
                        }
                    }
                    if (changed)
                    {
                        changed = false;
                        if (method.Parameters != null && method.Parameters.Count > 0)
                        {
                            var ps2 = new List<DCILMethodParamter>(method.Parameters);
                            for (int iCount = ps2.Count - 1; iCount >= 0; iCount--)
                            {
                                var p = ps2[iCount];
                                var eit = p.ValueType?.LocalClass?.GetEnumItemValueType();
                                if (eit != null)
                                {
                                    p = p.Clone();
                                    p.ValueType = eit;
                                    ps2[iCount] = p;
                                    changed = true;
                                    break;
                                }
                            }
                            if (changed)
                            {
                                method.Parameters = ps2;
                            }
                        }
                        var eit2 = method.ReturnTypeInfo?.LocalClass?.GetEnumItemValueType();
                        if (eit2 != null)
                        {
                            method.ReturnTypeInfo = eit2;
                            changed = true;
                        }
                        return changed;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 将所有的无构造函数的类中的内部静态函数合并到新的类型中。减少静态函数功能模块的关联。
        /// </summary>
        private void CollectStatcMethod()
        {
            var methods = new List<DCILMethod>();
            var clses = this.GetAllClasses();
            foreach (var cls in clses)
            {
                if (cls.InnerGenerate)
                {
                    continue;
                }
                if (cls.Parent != null)
                {
                    continue;
                }
                bool hasCotr = false;
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod && (item.Name == ".ctor" || item.Name == ".cctor"))
                    {
                        // 类型有构造函数的则不处理
                        hasCotr = true;
                        break;
                    }
                    if (item is DCILField || item is DCILEvent)
                    {
                        // 类型有字段或者事件的不处理。
                        hasCotr = true;
                        break;
                    }
                }
                if (hasCotr)
                {
                    continue;
                }
                for (int iCount = cls.ChildNodes.Count - 1; iCount >= 0; iCount--)
                {
                    var item = cls.ChildNodes[iCount];
                    if (item is DCILMethod)
                    {
                        var method = (DCILMethod)item;
                        if (method.IsStatic
                            && method.ParentMember == null
                            && method.HasGenericStyle == false
                            && method.Pinvokeimpl == null
                            && method.HasCustomAttributes == false
                            && (method.OperCodes != null || method.OperCodes.Count > 0))
                        {
                            if (method.RenameState == DCILRenameState.Renamed || method.IsPublic == false)
                            {
                                //if (method.Name == "CreateFromLinearGradientBrush")
                                //{

                                //}
                                var cls2 = (DCILClass)method.Parent;
                                if (cls2.NestedClasses != null && cls2.NestedClasses.Count > 0)
                                {
                                    continue;
                                }
                                // 只有重命名的未公开的函数才分离。
                                methods.Add(method);
                                //cls.ChildNodes.RemoveAt(iCount);
                                //method.Parent = null;
                            }
                        }
                    }
                }
            }//foreach
            if (methods.Count > 0)
            {
                for (int iCount = methods.Count - 1; iCount >= 0; iCount--)
                {
                    var method = methods[iCount];
                    bool moveFlag = true;
                    var cls = (DCILClass)method.Parent;
                    method.OperCodes.EnumDeeply(method, delegate (EnumOperCodeArgs args)
                  {
                      if (args.Current is DCILOperCode_HandleMethod)
                      {
                          var method4 = ((DCILOperCode_HandleMethod)args.Current).InvokeInfo.LocalMethod;
                          if (method4 != null
                              && method4.Parent == cls
                              && method4.IsPublic == false)
                          {
                              // 调用了同类型下的私有成员函数，则不分离
                              if (methods.Contains(method4) == false)
                              {
                                  moveFlag = false;
                              }
                          }
                      }
                  });
                    if (moveFlag)
                    {
                        cls.ChildNodes.Remove(method);
                    }
                    else
                    {
                        methods.RemoveAt(iCount);
                    }
                }
                if (methods.Count == 0)
                {
                    return;
                }
                methods.Sort(new DCILMethod.NameCompaer(true));

                var newClass = new DCILClass();
                newClass.Styles = new List<string>();
                newClass.Styles.AddRange(new string[] { "private", "auto", "ansi", "abstract", "sealed", "beforefieldinit" });
                newClass._Name = _ClassNamePrefix + "_jiejienet_sm";
                newClass.BaseType = new DCILTypeReference("[" + this.Document.LibName_mscorlib + "]System.Object", DCILTypeMode.Primitive);
                if (this._IDGenForClass != null)
                {
                    newClass.ChangeName(this._IDGenForClass.GenerateIDForClass(newClass.Name, newClass));
                }
                var idgen2 = new IDGenerator(this.PrefixForTypeRename, this.PrefixForMemberRename);
                idgen2.DebugMode = this.DebugMode;
                for (int iCount = methods.Count - 1; iCount >= 0; iCount--)
                {
                    var method = methods[iCount];
                    string oldClsName = method.OwnerClass.OldName == null ? method.OwnerClass.Name : method.OwnerClass.OldName;
                    string oldMethodName = method.OldName == null ? method.Name : method.OldName;
                    if (method.Styles[0] == "private")
                    {
                        method.Styles[0] = "assembly";
                    }
                    if (this.Switchs.Rename)
                    {
                        method.ChangeName(idgen2.GenerateIDForMember(
                            oldMethodName,
                            method));
                    }
                    else
                    {
                        method._Name = method.Name + "_" + iCount;
                    }
                    newClass.ChildNodes.Add(method);
                    method.Parent = newClass;
                    if (this.DebugMode)
                    {
                        MyConsole.Instance.WriteLine("            Move static methods : " + oldClsName + "::" + oldMethodName);
                    }
                }//for
                DCUtils.ObfuseListOrder(newClass.ChildNodes);
                this.Document.Classes.Add(newClass);
                ConsoleWriteTask();
                MyConsole.Instance.WriteLine("Move " + methods.Count + " static methods.");
            }
        }

        private void ChangeCallOperCodes(List<DCILOperCode_HandleMethod> callCodes)
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Packaging member property...");
            int tick = Environment.TickCount;
            var methods = new Dictionary<DCILMethod, int>();
            foreach (var item in callCodes)
            {
                var lm = item.InvokeInfo?.LocalMethod;
                if (lm == null
                    || lm.RenameState == DCILRenameState.Renamed)
                {
                    continue;
                }
                if (lm.Name.StartsWith("get_", StringComparison.Ordinal)
                    && lm.ParametersCount == 0
                    && lm.ParentMember is DCILProperty)
                {
                    if (methods.ContainsKey(lm))
                    {
                        methods[lm]++;
                    }
                    else
                    {
                        methods[lm] = 1;
                    }
                }
                else if (lm.Name.StartsWith("set_", StringComparison.Ordinal)
                    && lm.ParametersCount == 1
                    && lm.ParentMember is DCILProperty)
                {
                    if (methods.ContainsKey(lm))
                    {
                        methods[lm]++;
                    }
                    else
                    {
                        methods[lm] = 1;
                    }
                }
            }//foreach
            //int idCounter = 0;
            int newMethodCount = 0;
            int changeCodeCount = 0;
            foreach (var item in methods)
            {
                //if( item.Key.Name == "get_Document"
                //    && item.Key.Parent.Name == "DCSoft.Writer.Controls.WriterControl")
                //{

                //}
                if (item.Value > 5)
                {
                    var method = item.Key;
                    if (method.IsFinal == false)
                    {
                        if (method.IsVirtual || method.IsNewslot || method.IsAbstract)
                        {
                            continue;
                        }
                    }
                    if (method.OperCodes.Count > 20)
                    {
                        continue;
                    }
                    bool tooManyCode = false;
                    foreach (var code2 in method.OperCodes)
                    {
                        if (code2 is DCILOperCode_Try_Catch_Finally)
                        {
                            tooManyCode = true;
                            break;
                        }
                    }
                    if (tooManyCode)
                    {
                        continue;
                    }

                    var cls = (DCILClass)method.Parent;

                    var newMethod = method.SimpleClone();
                    cls.HasInnerGenerateMethod = true;
                    //if (this.Switchs.Rename)
                    //{
                    //    if (cls.idGenForMember == null)
                    //    {
                    //        cls.idGenForMember = new IDGenerator(this.PrefixForTypeRename, this.PrefixForMemberRename);
                    //    }
                    //    method.ChangeName(cls.idGenForMember.GenerateIDForMember(method.Name, method));
                    //}
                    //else
                    //{
                    //    method._Name = "__jiejie_net_" + Convert.ToString(idCounter++);
                    //}
                    //method._Name = "__jiejie_net_" + Convert.ToString(idCounter++);
                    method.RenameState = DCILRenameState.NotHandled;
                    method.ObfuscationSettings = null;
                    //if (this.Switchs.Rename)
                    //{
                    //    method._Name = "_" + Convert.ToString(newMethodCount++);
                    //}
                    //else
                    //{ 
                    method._Name = "__jiejie_net_" + newMethod.Name + "_" + item.Value;// Convert.ToString(idCounter++);
                    //}
                    //else
                    //{
                    //    method._Name = this._IDGenForClass.GenerateIDForMember(newMethod.Name, newMethod);//  "_" + Convert.ToString(newMethodCount);
                    //}
                    method.RemoveStyle_specialname();
                    if (method.Styles != null)
                    {
                        for (int iCount = 0; iCount < method.Styles.Count; iCount++)
                        {
                            if (method.Styles[iCount] == "public")
                            {
                                method.Styles[iCount] = "assembly";
                                break;
                            }
                        }
                    }
                    method.CustomAttributes = null;
                    method.InnerGenerate = true;
                    if (method.FlagsForPrivate)
                    {
                        method.Styles.Insert(0, "private");
                    }
                    else
                    {
                        method.Styles.Insert(0, "assembly");
                    }

                    ChangeEnumParameterToInteger(method);

                    cls.ChildNodes.Add(newMethod);
                    if (method.ParentMember is DCILProperty)
                    {
                        var p = (DCILProperty)method.ParentMember;
                        if (p.Method_Get?.LocalMethod == method)
                        {
                            p.Method_Get = p.Method_Get.SimpleClone();
                            p.Method_Get.LocalMethod = newMethod;
                        }
                        else if (p.Method_Set?.LocalMethod == method)
                        {
                            p.Method_Set = p.Method_Set.SimpleClone();
                            p.Method_Set.LocalMethod = newMethod;
                        }
                    }
                    method.ParentMember = null;
                    newMethodCount++;
                    changeCodeCount += item.Value;
                }
            }//foreach
            tick = Environment.TickCount - tick;
            MyConsole.Instance.WriteLine(" create " + newMethodCount + " methods , change " + changeCodeCount + " call/callvirt instructions. span " + tick + " milliseconds.");
        }

        private void RemoveMember()
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Removing Type members ...");
            SortedDictionary<string, System.Tuple<int, int>> counters = null;
            if (this.DebugMode)
            {
                counters = new SortedDictionary<string, System.Tuple<int, int>>();
            }
            int tick = Environment.TickCount;
            int removeCount = 0;
            foreach (var cls in this.Document.Classes)
            {
                var localCount = 0;
                var localPCount = 0;
                if (cls.RuntimeSwitchs == null)
                {
                    cls.RuntimeSwitchs = this.Switchs;
                }
                if (cls.RuntimeSwitchs.RemoveMember)
                {
                    if (cls.IsEnum && cls.RenameState == DCILRenameState.Renamed)
                    {
                        // 删除被混淆的枚举类型的常量
                        for (int iCount = cls.ChildNodes.Count - 1; iCount >= 0; iCount--)
                        {
                            var field = cls.ChildNodes[iCount] as DCILField;
                            if (field != null && field.IsConst)
                            {
                                cls.ChildNodes.RemoveAt(iCount);
                                removeCount++;
                                localCount++;
                            }
                        }
                    }
                    else
                    {
                        bool removeProperty = (cls.ImplementsInterfaces == null || cls.ImplementsInterfaces.Count == 0);
                        for (int iCount = cls.ChildNodes.Count - 1; iCount >= 0; iCount--)
                        {
                            var node = cls.ChildNodes[iCount];
                            if (node is DCILField)
                            {
                                var field = (DCILField)node;
                                if (field.IsConst && field.RenameState == DCILRenameState.Renamed)
                                {
                                    // 删除被混淆的常量
                                    cls.ChildNodes.RemoveAt(iCount);
                                    removeCount++;
                                    localCount++;
                                }
                            }
                            else if (removeProperty && node is DCILProperty)
                            {
                                var p = (DCILProperty)node;
                                bool removeFlag = false;
                                var mGet = p.Method_Get?.LocalMethod;
                                if (mGet != null
                                    && mGet.RenameState == DCILRenameState.Renamed
                                    && mGet.IsInOverrideList == false)
                                {
                                    removeFlag = true;
                                }
                                if (removeFlag == false)
                                {
                                    var mSet = p.Method_Set?.LocalMethod;
                                    if (mSet != null
                                        && mSet.RenameState == DCILRenameState.Renamed
                                        && mSet.IsInOverrideList == false)
                                    {
                                        removeFlag = true;
                                    }
                                }
                                if (removeFlag)
                                {
                                    mGet?.RemoveStyle("specialname");
                                    p.Method_Set?.LocalMethod?.RemoveStyle("specialname");
                                    cls.ChildNodes.RemoveAt(iCount);
                                    removeCount++;
                                    localPCount++;
                                }
                            }
                        }
                    }
                }
                if ((localCount > 0 || localPCount > 0) && counters != null)
                {
                    if (cls.OldName == null)
                    {
                        counters[cls.Name] = new Tuple<int, int>(localCount, localPCount);
                    }
                    else
                    {
                        counters[cls.OldName] = new Tuple<int, int>(localCount, localPCount);
                    }
                }

            }
            if (counters != null)
            {
                foreach (var item in counters)
                {
                    if (item.Value.Item2 == 0)
                    {
                        MyConsole.Instance.WriteLine("     " + item.Key + " \t remove " + item.Value.Item1 + " const fields.");
                    }
                    else
                    {
                        MyConsole.Instance.WriteLine("     " + item.Key + " \t remove " + item.Value.Item1 + " const fields, " + item.Value.Item2 + " properties.");
                    }
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
            MyConsole.Instance.WriteLine(" remove " + removeCount + " members , span " + tick + " milliseconds.");
        }

        /// <summary>
        /// 内容编码格式
        /// </summary>
        public Encoding ContentEncoding = Encoding.UTF8;

        /// <summary>
        /// SDK安装目录
        /// </summary>
        public string SDKDirectory = null;

        /// <summary>
        /// 强名称使用的SNK文件
        /// </summary>
        public string SnkFileName = null;
        /// <summary>
        /// 临时目录
        /// </summary>
        public string TempDirectory = Path.Combine(Path.GetTempPath(), "JieJie.NET_Temp");
        /// <summary>
        /// 重命名类型使用的前缀
        /// </summary>
        public string PrefixForTypeRename = "_jiejie";
        /// <summary>
        /// 重名称类型的成员使用的前缀
        /// </summary>
        public string PrefixForMemberRename = "_jj";
        /// <summary>
        /// 是否删除临时文件
        /// </summary>
        public bool DeleteTempFile = false;

        private int _IndexCounter = 0;
        public int AllocIndex()
        {
            return this._IndexCounter++;
        }
        private static readonly string _ClassNamePrefix = "__DC20210205._";
        internal void SetDocumentCustomInstructions(Dictionary<string, string> cis)
        {
            this.Document.CustomInstructions = cis;
        }
        public DCILDocument Document = null;
        private List<DCILClass> _AllClasses = null;
        /// <summary>
        /// 获得文档中所有的类型成员
        /// </summary>
        /// <returns></returns>
        public List<DCILClass> GetAllClasses()
        {
            if (this._AllClasses == null)
            {
                this._AllClasses = new List<DCILClass>();
                InnerGetAllClasses(this.Document.Classes, this._AllClasses);
            }
            return this._AllClasses;
        }
        private void InnerGetAllClasses(List<DCILClass> clses, List<DCILClass> list)
        {
            foreach (var cls in clses)
            {
                list.Add(cls);
                if (cls.NestedClasses != null && cls.NestedClasses.Count > 0)
                {
                    InnerGetAllClasses(cls.NestedClasses, list);
                }
            }
        }


        public string _UILanguageName = null;
        public string _UILanguageDisplayName = null;
        /// <summary>
        /// 提示用户选择用户界面语言
        /// </summary>
        public void SelectUILanguage()
        {
            //this._UILanguageName = null;
            var allResFiles = this.Document.Resouces;
            if (allResFiles.Count > 0)
            {
                var culs = this.Document.GetSupportCultures();
                if (culs != null && culs.Length > 0)
                {
                    ConsoleWriteTask();
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                    MyConsole.Instance.WriteLine("Please select UI language you want:");
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                    for (int iCount = 0; iCount < culs.Length; iCount++)
                    {
                        MyConsole.Instance.WriteLine("    " + iCount + ":" + culs[iCount].Name + " " + culs[iCount].DisplayName);
                    }
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                    MyConsole.Instance.Write("Please input index,press enter to use default,");
                    MyConsole.Instance.ResetColor();
                    if( this._UILanguageName != null && this._UILanguageName.Length > 0 )
                    {
                        bool find = false;
                        foreach(var cul in culs )
                        {
                            if( cul.Name == this._UILanguageName )
                            {
                                find = true;
                                this._UILanguageDisplayName = cul.DisplayName;
                                break;
                            }
                        }
                        if(find == false )
                        {
                            this._UILanguageDisplayName = null;
                            this._UILanguageName = null;
                        }
                    }
                    if (MyConsole.Instance.SupportKeyboardInput == false 
                        || string.IsNullOrEmpty(this._UILanguageName) == false)
                    {
                        MyConsole.Instance.WriteLine();
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                        if (this._UILanguageName == null)
                        {
                            MyConsole.Instance.WriteLine("OK,you already select default language.");
                            MyConsole.Instance.ResetColor();
                        }
                        else
                        {
                            MyConsole.Instance.WriteLine("OK,you already select [" + this._UILanguageName + "-" + this._UILanguageDisplayName + "].");
                            MyConsole.Instance.ResetColor();
                            foreach (var item in this.Document.Resouces.Values)
                            {
                                item.ChangeLanguage(this._UILanguageName);
                            }
                        }
                        return;
                    }
                    DCUtils.EatAllConsoleKey();
                    int countDown = 15;
                    int left = MyConsole.Instance.CursorLeft;
                    int top = MyConsole.Instance.CursorTop;
                    while (true)
                    {
                        if (MyConsole.Instance.KeyAvailable)
                        {
                            var key = MyConsole.Instance.ReadKey();
                            int index = key.KeyChar - '0';
                            if (index >= 0 && index < culs.Length)
                            {
                                MyConsole.Instance.WriteLine();
                                this._UILanguageName = culs[index].Name;
                                this._UILanguageDisplayName = culs[index].DisplayName;
                                foreach (var item in this.Document.Resouces.Values)
                                {
                                    item.ChangeLanguage(this._UILanguageName);
                                }
                            }
                            break;
                        }
                        else
                        {
                            MyConsole.Instance.CursorLeft = left;
                            MyConsole.Instance.CursorTop = top;
                            countDown--;
                            if (countDown < 0)
                            {
                                break;
                            }
                            MyConsole.Instance.Write("(" + countDown.ToString("00") + "):");
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    MyConsole.Instance.WriteLine();
                }
            }
        }
        private void ConsoleWriteTask()
        {
            MyConsole.Instance.EnsureNewLine();
            MyConsole.Instance.BackgroundColor = ConsoleColor.Green;
            MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
            MyConsole.Instance.Write("[");
            MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
            MyConsole.Instance.BackgroundColor = ConsoleColor.Black;
            MyConsole.Instance.Write("Task");
            MyConsole.Instance.BackgroundColor = ConsoleColor.Green;
            MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
            MyConsole.Instance.Write("]");
            MyConsole.Instance.ResetColor();
            MyConsole.Instance.Write(" ");
        }
         
        private void GetAllBaseType(object rootType, System.Collections.ArrayList list)
        {
            if (list.Contains(rootType) == false)
            {
                list.Add(rootType);
            }
            if (rootType is System.Type)
            {
                var nt = (System.Type)rootType;
                if (nt.BaseType != null)
                {
                    GetAllBaseType(nt.BaseType, list);
                }
                var its = nt.GetInterfaces();
                if (its != null && its.Length > 0)
                {
                    foreach (var it in its)
                    {
                        GetAllBaseType(it, list);
                    }
                }
            }
            else if (rootType is DCILTypeReference)
            {
                var dt = (DCILTypeReference)rootType;
                if (dt.LocalClass != null)
                {
                    if (dt.LocalClass.BaseType != null)
                    {
                        GetAllBaseType(dt.LocalClass.BaseType, list);
                    }
                    var its = dt.LocalClass.ImplementsInterfaces;
                    if (its != null && its.Count > 0)
                    {
                        foreach (var it in its)
                        {
                            GetAllBaseType(it, list);
                        }
                    }
                }
            }
        }
        private static readonly Dictionary<Type, List<DCILMethod>> _NativeTypeMethods
            = new Dictionary<Type, List<DCILMethod>>();
        /// <summary>
        /// 根据函数重载关系来进行重命名操作
        /// </summary>
        /// <param name="allClasses"></param>
        /// <param name="idGen"></param>
        /// <returns></returns>
        private int RenameByOverrideList(List<DCILClass> allClasses, IDGenerator idGen)
        {
            int renameCount = 0;
            var resultMap = new MethodOverrideMap();
            foreach (var cls in allClasses)
            {
                if (cls.IsMulticastDelegate)
                {
                    continue;
                }
                if (cls.IsEnum)
                {
                    continue;
                }
                if (cls.IsImport)
                {
                    continue;
                }
                cls.GenericParamters?.ClearRuntimeType();
                var baseTypes = cls.GetBaseTypeAndImplementsInterfaces();
                if (baseTypes == null || baseTypes.Count == 0)
                {
                    continue;
                }
                var baseMethods = GetBaseMethodsDeeply(cls);
                var handledNativeTypes = new List<Type>();
                var methodsFromInterface = new List<DCILMethod>();// 源自接口的函数对象列表
                var nativeBaseFianlMethods = new List<DCILMethod>();
                var baseTypeMethodMap = new SortedDictionary<string, List<DCILMethod>>();
                foreach (var method in baseMethods)
                {
                    if (method.IsNative && method.IsFinal)
                    {
                        method.RenameState = DCILRenameState.NotHandled;
                        nativeBaseFianlMethods.Add(method);
                    }
                    else
                    {
                        var key = method.GetSignString();
                        List<DCILMethod> list3 = null;
                        if (baseTypeMethodMap.TryGetValue(key, out list3) == false)
                        {
                            list3 = new List<DCILMethod>();
                            baseTypeMethodMap[key] = list3;
                        }
                        list3.Add(method);
                        if (method.IsFromInterface)
                        {
                            methodsFromInterface.Add(method);
                        }
                    }
                }//foreach
                methodsFromInterface.Sort(DCILMethod.ComparerByName);

                resultMap.AnalyseSetOverrideMethodList(cls, baseTypeMethodMap, methodsFromInterface);// SetOverrideMethodList(cls, resultMap, baseTypeMethodMap, methodsFromInterface);
                if (methodsFromInterface.Count > 0 && cls.IsInterface == false)
                {

                    // 还有源自接口的函数尚未实现，则在基类中查找
                    bool needHandle = false;
                    foreach (var item9 in methodsFromInterface)
                    {
                        if (item9.RenameState == DCILRenameState.NotHandled)
                        {
                            needHandle = true;
                            break;
                        }
                    }
                    if (needHandle)
                    {
                        if (nativeBaseFianlMethods.Count > 0)
                        {
                            // 搜索原生态的未重载的函数
                            for (int iCount = methodsFromInterface.Count - 1; iCount >= 0; iCount--)
                            {
                                var im = methodsFromInterface[iCount];
                                var key11 = im.GetSignString();
                                foreach (var item9 in nativeBaseFianlMethods)
                                {
                                    var key12 = item9.GetSignString();
                                    if (key11 == key12)
                                    {
                                        im.RenameState = DCILRenameState.Preserve;
                                        methodsFromInterface.RemoveAt(iCount);
                                        break;
                                    }
                                }
                            }
                        }
                        if (methodsFromInterface.Count > 0)
                        {
                            var curClass = cls;
                            while (curClass != null)
                            {
                                foreach (var item in curClass.ChildNodes)
                                {
                                    if (item is DCILMethod && DCILMethod.IsCtorOrCctor(item.Name) == false)
                                    {
                                        var method = (DCILMethod)item;
                                        if (method.IsInstance)
                                        {
                                            foreach (var item2 in methodsFromInterface)
                                            {
                                                if (item2.Name == method.Name)
                                                {
                                                    if (item2.MatchSign(method))
                                                    {
                                                        method.RenameState = DCILRenameState.Preserve;
                                                        methodsFromInterface.Remove(item2);
                                                        if (methodsFromInterface.Count == 0)
                                                        {
                                                            goto EndCheckInterfaceMethod;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                curClass = curClass.BaseType?.LocalClass;
                            }
                            EndCheckInterfaceMethod:;
                            if (methodsFromInterface.Count > 0)
                            {
                                foreach (var item in baseTypeMethodMap)
                                {
                                    foreach (var im in methodsFromInterface)
                                    {
                                        if (item.Value.Contains(im) && item.Value.Count > 1)
                                        {
                                            foreach (var item4 in item.Value)
                                            {
                                                if (item4.RenameState == DCILRenameState.Preserve || item4.IsNative)
                                                {
                                                    foreach (var item5 in item.Value)
                                                    {
                                                        item5.RenameState = DCILRenameState.Preserve;
                                                    }
                                                    break;
                                                }
                                            }
                                            methodsFromInterface.Remove(im);
                                            break;
                                        }
                                    }
                                }
                            }
                            if (methodsFromInterface.Count > 0)
                            {
                                // 还是存在接口函数未实现，则认为是基础的原生类型实现了，则接口函数不能重名
                                foreach (var item9 in methodsFromInterface)
                                {
                                    item9.RenameState = DCILRenameState.Preserve;
                                }
                            }
                        }
                    }
                }
            }//foreach
            var lists = new HashSet<RefMethodList>();
            foreach (var item in resultMap.Values)
            {
                item.RemoveSameItem();
                if (item.Count > 1)
                {
                    if (item.Count > 3)
                    {
                        // 排除重复项目,减少后期运算量
                        for (int iCount4 = item.Count - 1; iCount4 >= 0; iCount4--)
                        {
                            if (item.IndexOf(item[iCount4]) != iCount4)
                            {
                                item.RemoveAt(iCount4);
                            }
                        }
                        if (item.Count > 1)
                        {
                            lists.Add(item);
                        }
                    }
                    else
                    {
                        lists.Add(item);
                    }
                }
                foreach (var method in item)
                {
                    method.IsInOverrideList = true;
                }
            }
            int nullCount = 0;
            var methodGrounds = new List<RefMethodList>(lists);
            for (int iCount = methodGrounds.Count - 1; iCount >= 0; iCount--)
            {
                var methods = methodGrounds[iCount];
                bool noRename = false;
                foreach (var item2 in methods)
                {
                    if (item2.IsNative)
                    {
                        // 如果重载列表中只要有一个源自外部的函数，则全部不重名。
                        noRename = true;
                        break;
                    }
                }
                if (noRename == false)
                {
                    foreach (var item2 in methods)
                    {
                        if (item2.RenameState == DCILRenameState.Preserve)
                        {
                            noRename = true;
                            break;
                        }
                        if (AllowRename(item2) == false)
                        {
                            // 如果重载列表中只要有一个明确不重名，则全部不重名。
                            noRename = true;
                            break;
                        }
                    }
                }
                if (noRename)
                {
                    foreach (var item3 in methods)
                    {
                        item3.RenameState = DCILRenameState.Preserve;
                    }
                    methodGrounds[iCount] = null;
                    nullCount++;
                }
            }
            // 不重名区域扩散
            for (int iCount = methodGrounds.Count - 1; iCount >= 0; iCount--)
            {
                var methods = methodGrounds[iCount];
                if (methods != null)
                {
                    foreach (var item in methods)
                    {
                        if (item.RenameState == DCILRenameState.Preserve)
                        {
                            foreach (var item2 in methods)
                            {
                                item2.RenameState = DCILRenameState.Preserve;
                            }
                            methodGrounds[iCount] = null;
                            nullCount++;
                            break;
                        }
                    }
                }
            }
            if (nullCount != methodGrounds.Count)
            {
                // 经过两轮循环后还存在的函数就是可以重命名了。
                for (int iCount = methodGrounds.Count - 1; iCount >= 0; iCount--)
                {
                    var methods = methodGrounds[iCount];
                    if (methods != null)
                    {
                        bool hadRenamed = false;
                        foreach (var method in methods)
                        {
                            if (method.RenameState == DCILRenameState.Renamed)
                            {
                                hadRenamed = true;
                                foreach (var item2 in methods)
                                {
                                    if (item2 != method)
                                    {
                                        item2.ChangeName(method.Name);
                                        renameCount++;
                                    }
                                }
                                break;
                            }
                        }
                        if (hadRenamed == false)
                        {
                            var newName = idGen.GenerateIDForMember(methods[0].Name, methods[0]);
                            foreach (var method in methods)
                            {
                                if (method.RenameState == DCILRenameState.NotHandled)
                                {
                                    method.ChangeName(newName);
                                    renameCount++;
                                }
                            }
                        }
                    }
                }
            }
            resultMap.ClearData();
            methodGrounds.Clear();
            return renameCount;
        }

        private static readonly Dictionary<Type, List<DCILMethod>> _Native_BaseMethods
            = new Dictionary<Type, List<DCILMethod>>();
        /// <summary>
        /// 获得原生类型的基础类型的成员函数列表
        /// </summary>
        /// <param name="rootType"></param>
        /// <param name="gpvs"></param>
        /// <returns></returns>
        private List<DCILMethod> GetBaseMethodsByNativeType(Type rootType, List<DCILTypeReference> gpvs)
        {
            List<DCILMethod> resultList = null;
            if (rootType.IsGenericType == false && _Native_BaseMethods.TryGetValue(rootType, out resultList))
            {
                return resultList;
            }
            resultList = new List<DCILMethod>();
            if (rootType.IsGenericType == false)
            {
                _Native_BaseMethods[rootType] = resultList;
            }
            var gps = DCILGenericParamterList.CreateByNativeType(rootType);
            if (gps != null)
            {
                gps.SetRuntimeType(gpvs);
            }
            var ms = rootType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var m in ms)
            {
                if (m.IsAssembly || m.IsPrivate)
                {
                    continue;
                }
                resultList.Add(new DCILMethod(m, this.Document, gps));
            }
            var its = rootType.GetInterfaces();
            if (its != null && its.Length > 0)
            {
                foreach (var it in its)
                {
                    if (it.IsPublic)
                    {
                        var type2 = DCILTypeReference.CreateByNativeType(it, this.Document);
                        type2 = type2.Transform(gps);
                        var nt2 = type2.SearchNativeType();
                        if (nt2 != null)
                        {
                            var list2 = GetBaseMethodsByNativeType(nt2, type2.GenericParamters);
                            resultList.AddRange(list2);
                        }
                    }
                }//foreach
            }
            return resultList;
        }

        private Dictionary<DCILClass, List<DCILMethod>> _Cache_BaseMethods
            = new Dictionary<DCILClass, List<DCILMethod>>();
        /// <summary>
        /// 获得类型的基础类型和实现接口的所有成员函数
        /// </summary>
        /// <param name="rootClass"></param>
        /// <returns></returns>
        private List<DCILMethod> GetBaseMethodsDeeply(DCILClass rootClass)
        {
            List<DCILMethod> resultList = null;
            if (rootClass.HasGenericParamters == false
                && _Cache_BaseMethods.TryGetValue(rootClass, out resultList))
            {
                return resultList;
            }
            resultList = new List<DCILMethod>();
            if (rootClass.HasGenericParamters == false)
            {
                _Cache_BaseMethods[rootClass] = resultList;
            }
            var baseTypes = rootClass.GetBaseTypeAndImplementsInterfaces();
            foreach (var baseType in baseTypes)
            {
                List<DCILTypeReference> gpvs = null;
                if (baseType.GenericParamters != null && baseType.GenericParamters.Count > 0)
                {
                    gpvs = new List<DCILTypeReference>();
                    var item2 = baseType.Clone();
                    item2.GenericParamters = new List<DCILTypeReference>();
                    foreach (var vt in baseType.GenericParamters)
                    {
                        gpvs.Add(vt.Transform(rootClass.GenericParamters));
                    }
                }
                if (baseType.HasLibraryName)
                {
                    var nt = baseType.SearchNativeType(this._InputAssemblyDirectory);
                    if (nt != null)
                    {
                        var list5 = GetBaseMethodsByNativeType(nt, gpvs);
                        resultList.AddRange(list5);
                    }
                }
                else
                {
                    var localClass = baseType.LocalClass;
                    if (localClass == null)
                    {
                        localClass = this.Document.GetClassByName(baseType.Name);
                    }
                    if (localClass == null)
                    {
                        continue;
                    }
                    localClass.GenericParamters?.SetRuntimeType(gpvs);
                    bool isAllPreserved = rootClass.IsInterface || rootClass.IsImport;
                    if (isAllPreserved == false)
                    {
                        var os = rootClass.ObfuscationSettings;
                        if (os != null && os.Exclude && os.ApplyToMembers)
                        {
                            // 指明全部都保留名称
                            isAllPreserved = true;
                        }
                    }
                    var allMethods = new List<DCILMethod>();
                    foreach (var item in localClass.ChildNodes)
                    {
                        if (item is DCILMethod)
                        {
                            var method = (DCILMethod)item;
                            if (isAllPreserved || (method.IsInstance && (method.IsVirtual || method.IsAbstract || method.IsNewslot)))
                            {
                                method.IsFromInterface = localClass.IsInterface;
                                if (method.GenericParamters != null && method.GenericParamters.Count > 0)
                                {
                                    foreach (var item3 in method.GenericParamters)
                                    {
                                        item3.RuntimeType = null;
                                    }
                                }
                                resultList.Add(method);
                            }
                        }
                    }//foreach
                    var list4 = GetBaseMethodsDeeply(localClass);
                    resultList.AddRange(list4);
                }
            }
            return resultList;
        }

        /// <summary>
        /// 判断方法是否允许重命名
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool AllowRename(DCILMethod method)
        {
            if (method.Pinvokeimpl != null && method.Pinvokeimpl.Length > 0)
            {
                // 是从外部导入的平台调用API，则不改名
                return false;
            }
            if (method.Name == "Main")
            {
                // 入口函数，不重命名。
                return false;
            }
            if (method.ObfuscationSettings != null
                && method.ObfuscationSettings.Exclude)
            {
                // 函数本身指定不改名
                return false;
            }
            if (method.ParentMember != null)
            {
                // 所在的属性和事件指明不改名
                var os2 = method.ParentMember.ObfuscationSettings;
                if (os2 != null && os2.Exclude && os2.ApplyToMembers)
                {
                    return false;
                }
            }
            var cls = (DCILClass)method.Parent;
            if (cls != null)
            {
                var os3 = cls.ObfuscationSettings;
                if (os3 != null && os3.Exclude && os3.ApplyToMembers)
                {
                    // 所在的类型指定所有成员不改名
                    return false;
                }
            }

            return true;
        }

        private class MethodOverrideMap : Dictionary<DCILMethod, RefMethodList>
        {
            public void ClearData()
            {
                foreach (var item in this)
                {
                    item.Value.Clear();
                }
                base.Clear();
            }
            /// <summary>
            /// 分析函数的重载关系，获得相关清单
            /// </summary>
            /// <param name="rootClass"></param>
            /// <param name="baseTypeMethodMap"></param>
            /// <param name="methodFromInterfaces"></param>
            public void AnalyseSetOverrideMethodList(
                DCILClass rootClass,
                IDictionary<string, List<DCILMethod>> baseTypeMethodMap,
                List<DCILMethod> methodFromInterfaces)
            {
                foreach (var item in rootClass.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        var method = (DCILMethod)item;
                        if (DCILMethod.IsCtorOrCctor(method.Name))
                        {
                            method.RenameState = DCILRenameState.Preserve;
                            continue;
                        }
                        if (method.IsInstance == false)
                        {
                            continue;
                        }
                        if (method._Override != null && methodFromInterfaces != null && methodFromInterfaces.Count > 0)
                        {
                            if (method._Override.LocalMethod != null)
                            {
                                int index10 = methodFromInterfaces.IndexOf(method._Override.LocalMethod);
                                if (index10 >= 0)
                                {
                                    methodFromInterfaces.RemoveAt(index10);
                                }
                            }
                            else
                            {
                                var ownerTypeName = method._Override.OwnerType.ToString();
                                var methodName = method._Override.MethodName;
                                foreach (var im in methodFromInterfaces)
                                {
                                    if (im.Parent is DCILClass)
                                    {
                                        var cls9 = (DCILClass)im.Parent;
                                        if (cls9.GetNameWithNestedPlus(true) == ownerTypeName
                                            && im.Name == methodName)
                                        {
                                            var sign10 = method._Override.GetSignString(true);
                                            if (sign10 == im.GetSignString())
                                            {
                                                methodFromInterfaces.Remove(im);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            continue;
                        }
                        if (method.IsVirtual || method.IsAbstract || method.IsNewslot)
                        {
                            var key = method.GetSignString();
                            List<DCILMethod> baseMethods = null;
                            if (baseTypeMethodMap.TryGetValue(key, out baseMethods))
                            {
                                var refMethods = this.AddInfos(baseMethods, method);
                                if (methodFromInterfaces != null)
                                {
                                    foreach (var item10 in refMethods)
                                    {
                                        var index11 = methodFromInterfaces.IndexOf(item10);
                                        if (index11 >= 0)
                                        {
                                            methodFromInterfaces.RemoveAt(index11);
                                        }
                                    }
                                }
                            }//if
                        }//if
                    }//if
                }//foreach
                if (methodFromInterfaces.Count > 0)
                {
                    var curClass = rootClass.BaseType?.LocalClass;
                    while (curClass != null)
                    {
                        foreach (var item in curClass.ChildNodes)
                        {
                            if (item is DCILMethod)
                            {
                                var method = (DCILMethod)item;
                                if (method.IsInstance && method.IsPublic)
                                {
                                    foreach (var im in methodFromInterfaces)
                                    {
                                        if (im.Name == method.Name && im.MatchSign(method))
                                        {
                                            AddInfo(im, method);
                                            methodFromInterfaces.Remove(im);
                                            if (methodFromInterfaces.Count == 0)
                                            {
                                                goto EndHandleMethodFromInterface;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        curClass = curClass.BaseType?.LocalClass;
                    }
                    EndHandleMethodFromInterface:;
                }
            }

            public RefMethodList AddInfos(List<DCILMethod> baseMethods, DCILMethod method)
            {
                RefMethodList refMethods = null;
                if (this.TryGetValue(method, out refMethods) == false)
                {
                    refMethods = new RefMethodList();
                    this[method] = refMethods;
                    refMethods.Add(method);
                }
                refMethods.AddRange(baseMethods);
                foreach (var baseMethod in baseMethods)
                {
                    RefMethodList list2 = null;
                    if (this.TryGetValue(baseMethod, out list2) == false)
                    {
                        this[baseMethod] = refMethods;
                    }
                    else if (list2 != refMethods)
                    {
                        foreach (var item4 in list2)
                        {
                            if (refMethods.Contains(item4) == false)
                            {
                                if (item4.IsNative)
                                {
                                    refMethods.Insert(0, item4);
                                }
                                else
                                {
                                    refMethods.Add(item4);
                                }
                            }
                        }
                        this[baseMethod] = refMethods;
                    }
                    else
                    {
                        refMethods.Add(baseMethod);
                    }
                }
                return refMethods;
            }
            public void AddInfo(DCILMethod baseMethod, DCILMethod method)
            {
                RefMethodList list2 = null;
                if (this.TryGetValue(baseMethod, out list2) == false)
                {
                    list2 = new RefMethodList();
                    this[baseMethod] = list2;
                    list2.Add(baseMethod);
                }
                list2.Add(method);
            }
        }
        private class RefMethodList : List<DCILMethod>
        {
            /// <summary>
            /// 删除重复的项目
            /// </summary>
            public void RemoveSameItem()
            {
                DCUtils.RemoveSameItem<DCILMethod>(this);
            }
            public override string ToString()
            {
                var result = new StringBuilder();
                result.Append("[" + this.Count + "]");
                foreach (var item in this)
                {
                    if (result.Length > 0)
                    {
                        result.Append(',');
                    }
                    if (item.IsNative)
                    {
                        result.Append(item._NativeMethod.DeclaringType.Name);
                    }
                    else
                    {
                        var name = item.Parent?.Name;
                        if (name != null)
                        {
                            int index = name.LastIndexOf('.');
                            if (index > 0)
                            {
                                result.Append(name.Substring(index + 1));
                            }
                            else
                            {
                                result.Append(name);
                            }
                        }
                    }
                    if (result.Length > 50)
                    {
                        break;
                    }
                }
                return result.ToString();
            }
        }
        private IDGenerator _IDGenForClass = null;

        public int RenameClasses()
        {
            IDGenerator.GenCountBase = 0;
            ConsoleWriteTask();
            MyConsole.Instance.Write("Renaming.....");

            int curPos = 0;
            if (MyConsole.Instance.IsNativeConsole)
            {
                curPos = MyConsole.Instance.CursorLeft * MyConsole.Instance.CursorTop;
            }

            int tick = Environment.TickCount;
            var idGenForMember = new IDGenerator(this.PrefixForTypeRename, this.PrefixForMemberRename);
            idGenForMember.DebugMode = this.DebugMode;

            var allClasses = GetAllClasses();
            foreach (var cls in allClasses)
            {
                if (cls.IsImport)
                {
                    foreach (var item in cls.ChildNodes)
                    {
                        if (item is DCILMemberInfo)
                        {
                            ((DCILMemberInfo)item).RenameState = DCILRenameState.Preserve;
                        }
                    }
                    continue;
                }
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        var m = (DCILMemberInfo)item;
                        if (item is DCILField && cls.InnerGenerate)
                        {
                            // 内部产生的类型，不保留旧名称
                            continue;
                        }
                        m.OldSignatureForMap = m.GetSignatureForMap();
                    }
                }//foreach
            }//foreach

            // 根据函数重载关系来重命名。
            int result = RenameByOverrideList(allClasses, idGenForMember);

            IDGenerator.GenCountBase = idGenForMember.GenCount + 1;
            var countBaseGenMember = idGenForMember.GenCount + 1;
            // 执行函数的重命名
            this._IDGenForClass = new IDGenerator(this.PrefixForTypeRename, this.PrefixForMemberRename);
            this._IDGenForClass.DebugMode = this.DebugMode;
            int genCountStart = _Random.Next(10, 100);
            int nestedClassCounter = 0;
            foreach (var cls in allClasses)
            {
                if (cls.IsImport)
                {
                    // 外界导入的COM接口，则不改名
                    continue;
                }
                var clsOs = cls.ObfuscationSettings;
                //cls.RemoveObfuscationAttribute();
                idGenForMember.GenCount = countBaseGenMember;
                if (clsOs == null || clsOs.Exclude == false)
                {
                    cls.OldName = cls.Name;
                    if (cls.Parent is DCILClass)
                    {
                        nestedClassCounter++;
                        idGenForMember.GenCount = countBaseGenMember + nestedClassCounter;
                        // 内嵌类型
                        var newName = idGenForMember.GenerateIDForMember(cls.Name, cls);
                        cls.ChangeName(newName);
                    }
                    else
                    {
                        var newName = this._IDGenForClass.GenerateIDForClass(cls.OldName, cls);
                        //if (cls.Parent is DCILClass)
                        //{
                        //    // 内嵌的类型，则去除命名空间
                        //    int index4 = newName.LastIndexOf('.');
                        //    if (index4 > 0)
                        //    {
                        //        newName = newName.Substring(index4 + 1);
                        //    }
                        //}
                        cls.ChangeName(newName);
                    }
                    result++;
                }
                else
                {
                    cls.RenameState = DCILRenameState.Preserve;
                }
                idGenForMember.GenCount = countBaseGenMember;
                //continue;
                if (clsOs == null || clsOs.Exclude == false || clsOs.ApplyToMembers == false)
                {
                    if (cls.IsMulticastDelegate)// .BaseType?.Name == "System.MulticastDelegate")
                    {
                        continue;
                    }
                    // 重命名成员
                    if (idGenForMember.DebugMode == false)
                    {
                        // 对于类型的成员方法按照参数签名进行分组
                        var methodGroups = new Dictionary<string, List<DCILMethod>>();
                        foreach (var item in cls.ChildNodes)
                        {
                            if (item is DCILMethod)
                            {
                                var method = (DCILMethod)item;
                                if (method.RenameState == DCILRenameState.NotHandled)
                                {
                                    if (AllowRename(method))
                                    {
                                        if (method.HasGenericStyle)
                                        {
                                            method.OldName = method.Name;
                                            method.ChangeName(idGenForMember.GenerateIDForMember(method.OldName, method));
                                            result++;
                                            continue;
                                        }
                                        var sign = method.GetParamterListString(false, false);
                                        List<DCILMethod> list = null;
                                        if (methodGroups.TryGetValue(sign, out list) == false)
                                        {
                                            list = new List<DCILMethod>();
                                            methodGroups[sign] = list;
                                        }
                                        list.Add(method);
                                    }
                                    else
                                    {
                                        method.RenameState = DCILRenameState.Preserve;
                                    }
                                }
                            }
                        }
                        if (methodGroups.Count > 0)
                        {
                            // 重名称成员方法。尽量做到成员方法名称重复
                            int maxGroupSize = 0;
                            foreach (var group in methodGroups.Values)
                            {
                                maxGroupSize = Math.Max(maxGroupSize, group.Count);
                            }
                            for (int iCount = 0; iCount < maxGroupSize; iCount++)
                            {
                                var newName = idGenForMember.GenerateIDForMember(null, null);
                                foreach (var group in methodGroups.Values)
                                {
                                    if (iCount < group.Count)
                                    {
                                        var method = group[iCount];
                                        method.OldName = method.Name;
                                        method.ChangeName(newName);
                                        result++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 调试模式下普通的重命名成员方法
                        foreach (var item in cls.ChildNodes)
                        {
                            if (item is DCILMethod)
                            {
                                var method = (DCILMethod)item;

                                if (method.RenameState == DCILRenameState.NotHandled)
                                {
                                    if (AllowRename(method))
                                    {
                                        method.OldName = method.Name;
                                        method.ChangeName(idGenForMember.GenerateIDForMember(method.OldName, method));
                                        result++;
                                    }
                                    else
                                    {
                                        method.RenameState = DCILRenameState.Preserve;
                                    }
                                }
                            }
                        }
                    }
                    // 重命名成员字段
                    foreach (var item in cls.ChildNodes)
                    {
                        if (item is DCILField)
                        {
                            var field = (DCILField)item;
                            if (field.ObfuscationSettings == null || field.ObfuscationSettings.Exclude == false)
                            {
                                if (cls.IsEnum && field.Name == "value__" && field.HasStyle("specialname"))
                                {
                                    continue;
                                }
                                field.OldName = field.Name;
                                field.ChangeName(idGenForMember.GenerateIDForMember(field.OldName, field));
                                result++;
                            }
                        }
                    }
                }
                else if (cls.HasInnerGenerateMethod)
                {
                    foreach (var item in cls.ChildNodes)
                    {
                        if (item is DCILMethod)
                        {
                            var method = (DCILMethod)item;
                            if (method.InnerGenerate)
                            {
                                method.OldName = method.Name;
                                method.ChangeName(idGenForMember.GenerateIDForMember(method.OldName, method));
                            }
                        }
                    }
                }
            }//foreach

            var eba = new DCILCustomAttribute();
            eba.InvokeInfo = new DCILInvokeMethodInfo();
            eba.AttributeTypeName = typeof(System.ComponentModel.EditorBrowsableAttribute).FullName;// "System.ComponentModel.EditorBrowsableAttribute";
            eba.InvokeInfo.OwnerType = new DCILTypeReference(typeof(System.ComponentModel.EditorBrowsableAttribute), this.Document);
            eba.InvokeInfo.MethodName = ".ctor";
            eba.InvokeInfo.IsInstance = true;
            eba.InvokeInfo.ReturnType = DCILTypeReference.Type_Void;
            eba.InvokeInfo.Paramters = new List<DCILMethodParamter>();
            eba.InvokeInfo.Paramters.Add(new DCILMethodParamter());
            eba.InvokeInfo.Paramters[0].ValueType = new DCILTypeReference(typeof(System.ComponentModel.EditorBrowsableState), this.Document);
            eba.BinaryValue = new byte[] { 01, 00, 01, 00, 00, 00, 00, 00 };


            int totalCount_cls = 0;
            int handleCount_cls = 0;
            int totalCount_field = 0;
            int handleCount_field = 0;
            int totalCount_Method = 0;
            int handleCount_Method = 0;
            int removeCount_PropertyEvent = 0;
            var childNeedRemove = new List<DCILObject>();
            foreach (var cls in allClasses)
            {
                if (cls.InnerGenerate == false)
                {
                    totalCount_cls++;
                    if (cls.RenameState == DCILRenameState.Renamed)
                    {
                        handleCount_cls++;
                    }
                }
                cls.RemoveObfuscationAttribute();
                bool memberAddEBA = true;
                if (cls.AddEditorBrowsableAttributeForRename(eba))
                {
                    memberAddEBA = false;
                }
                else if (cls.HasStyle("public") == false)
                {
                    memberAddEBA = false;
                }
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        var itemMember = (DCILMemberInfo)item;
                        itemMember.RemoveObfuscationAttribute();
                        if (memberAddEBA && (itemMember.HasStyle("family") || itemMember.HasStyle("public")))
                        {
                            itemMember.AddEditorBrowsableAttributeForRename(eba);
                        }
                        if (item is DCILField)
                        {
                            totalCount_field++;
                            if (((DCILField)item).RenameState == DCILRenameState.Renamed)
                            {
                                handleCount_field++;
                            }
                        }
                        else if (item is DCILMethod)
                        {
                            totalCount_Method++;
                            var method = (DCILMethod)item;
                            if (method.RenameState == DCILRenameState.Renamed)
                            {
                                handleCount_Method++;
                                RenameMethodParameter(method);
                                if (method.ParentMember != null)
                                {
                                    childNeedRemove.Add(method.ParentMember);
                                }
                            }
                            else if (cls.RenameState == DCILRenameState.Renamed && DCILMethod.IsCtorOrCctor(method.Name))
                            {
                                RenameMethodParameter(method);
                            }
                        }
                    }
                }

                if (childNeedRemove.Count > 0)
                {
                    removeCount_PropertyEvent += childNeedRemove.Count;
                    foreach (var item in childNeedRemove)
                    {
                        cls.ChildNodes.Remove(item);
                    }
                    childNeedRemove.Clear();
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
            if (MyConsole.Instance.IsNativeConsole)
            {
                if (MyConsole.Instance.CursorLeft * MyConsole.Instance.CursorTop != curPos)
                {
                    MyConsole.Instance.WriteLine();
                }
            }
            MyConsole.Instance.WriteLine(" span " + tick + " milliseconds.");
            var strResult = new StringBuilder();
            if (handleCount_cls > 0)
            {
                AddFinalResultMessage(strResult, "    Type renamed", totalCount_cls, handleCount_cls);
            }
            if (handleCount_Method > 0)
            {
                AddFinalResultMessage(strResult, "    Method renamed", totalCount_Method, handleCount_Method);
            }
            if (handleCount_field > 0)
            {
                AddFinalResultMessage(strResult, "    Field renamed", totalCount_field, handleCount_field);
            }
            if (removeCount_PropertyEvent > 0)
            {
                AddFinalResultMessage(strResult, "    Property/Event removed", 0, removeCount_PropertyEvent);
            }
            if (strResult.Length > 0)
            {
                MyConsole.Instance.WriteLine(strResult.ToString());
            }
            //int warringCount = 0;
            //foreach( var cls in allClasses)
            //{
            //    if( cls.RenameState == DCILRenameState.Renamed )
            //    {
            //        foreach( var item in cls.ChildNodes )
            //        {
            //            if(item is DCILMemberInfo)
            //            {
            //                var info = (DCILMemberInfo)item;
            //                if(info.RenameState != DCILRenameState.Renamed && info.Name != ".ctor" && info.Name != ".cctor")
            //                {
            //                    Console.WriteLine("   未重命名:" + cls.GetNameWithNested('.', true) + "=>" + cls.GetNameWithNested('.') +  "::" + info.Name );
            //                    if(warringCount ++ > 500)
            //                    {
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            return result;
        }

        internal class NumberNameGen : IDisposable
        {
            public NumberNameGen(string prefix, int num = 0)
            {
                this._Prefix = prefix;
                if (num > 0)
                {
                    this._Values = new List<string>(num);
                }
            }
            public string GetName(int index)
            {
                if (index >= this._Values.Count)
                {
                    for (int iCount = this._Values.Count; iCount <= index; iCount++)
                    {
                        if (this._Prefix == null || this._Prefix.Length == 0)
                        {
                            this._Values.Add(iCount.ToString());
                        }
                        else
                        {
                            this._Values.Add(this._Prefix + iCount.ToString());
                        }
                    }
                }
                return this._Values[index];
            }
            private string _Prefix = null;
            private List<string> _Values = new List<string>();

            public void Dispose()
            {
                if (this._Values != null)
                {
                    this._Values.Clear();
                    this._Values = null;
                }
            }
        }

        private NumberNameGen _NewPNameGen = new NumberNameGen("p");
        /// <summary>
        /// 重命名函数的参数名
        /// </summary>
        /// <param name="method"></param>
        private void RenameMethodParameter(DCILMethod method)
        {
            if (method.Parameters != null && method.Parameters.Count > 0)
            {
                int pCount = 0;
                var maps = new Dictionary<string, string>();
                foreach (var p in method.Parameters)
                {
                    var newPName = _NewPNameGen.GetName(pCount++);// "p" + Convert.ToString(pCount++);
                    maps[p.Name] = newPName;
                    p.Name = newPName;
                }
                var codes = method.GetAllOperCodes<DCILOperCode>();
                if (codes != null && codes.Count > 0)
                {
                    foreach (var code in codes)
                    {
                        var operCodeValue = code.OperCodeValue;
                        if (operCodeValue == DCILOpCodeValue.Ldarg_S
                            || operCodeValue == DCILOpCodeValue.Ldarga_S
                            || operCodeValue == DCILOpCodeValue.Starg_S)
                        {
                            string newName = null;
                            if (maps.TryGetValue(code.OperData, out newName))
                            {
                                code.OperData = newName;
                            }
                        }
                    }
                }
            }
            //if( method.Locals != null && method.Locals.Count > 0 )
            //{
            //    var names = new HashSet<string>();
            //    foreach( var item in method.Locals )
            //    {
            //        if( item.Name != null && item.Name.Length > 0 )
            //        {
            //            names.Add(item.Name);
            //        }
            //    }
            //    int nameCounter = 0;
            //    foreach ( var item in method.Locals)
            //    {
            //        if( item.Name == null || item.Name.Length == 0 )
            //        {
            //            while( true )
            //            {
            //                string newName = "loc" + Convert.ToString(nameCounter++);
            //                if( names.Contains( newName ) == false )
            //                {
            //                    item.Name = newName;
            //                    names.Add(newName);
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //    method.OperCodes.EnumDeeply(delegate (DCILOperCode code) {
            //        if (code.OperData != null && code.OperData.Length > 0)
            //        {
            //            if (code.OperCode == "ldloc"
            //            || code.OperCode == "ldloc.s"
            //            || code.OperCode == "stloc"
            //            || code.OperCode == "stloc.s")
            //            {
            //                int index = 0;
            //                if( int.TryParse( code.OperData , out index ))
            //                {
            //                    if( index >= 0 && index < method.Locals.Count )
            //                    {
            //                        code.OperData = method.Locals[index].Name;
            //                    }
            //                }
            //            }
            //        }
            //    });
            //}
        }

        private void AddFinalResultMessage(StringBuilder str, string title, int totalCount, int handleCount)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if (title == null || title.Length == 0)
            {
                throw new ArgumentNullException("title");
            }
            str.Append(title);
            if (title.Length < 30)
            {
                str.Append('.', 30 - title.Length);
            }
            if (totalCount > 0)
            {
                string strCount = handleCount.ToString() + "/" + totalCount.ToString();
                str.Append(strCount);
                if (strCount.Length < 15)
                {
                    str.Append(' ', 15 - strCount.Length);
                }
                double rate = handleCount * 100.0 / totalCount;
                str.AppendLine(" (" + rate.ToString("0.00") + "%)");
            }
            else
            {
                str.AppendLine(handleCount.ToString());
            }
        }

        private void InnerGetAllBaseType(DCILClass start, List<DCILTypeReference> list)
        {
            if (start.BaseType != null)
            {
                list.Add(start.BaseType);
            }
            if (start.ImplementsInterfaces != null && start.ImplementsInterfaces.Count > 0)
            {
                list.AddRange(start.ImplementsInterfaces);
            }
            if (start.BaseType.LocalClass != null)
            {
                InnerGetAllBaseType(start.BaseType.LocalClass, list);
            }
            if (start.ImplementsInterfaces != null && start.ImplementsInterfaces.Count > 0)
            {
                foreach (var item in start.ImplementsInterfaces)
                {
                    if (item.LocalClass != null)
                    {
                        InnerGetAllBaseType(item.LocalClass, list);
                    }
                }
            }
        }
        private int _ModifiedCount = 0;

        /// <summary>
        /// 是否处于调试模式
        /// </summary>
        public bool DebugMode = false;
        /// <summary>
        /// 全局选项
        /// </summary>
        public JieJieSwitchs Switchs = new JieJieSwitchs();

        private void HandleMethod(DCILMethod method)
        {
            var opts = method.RuntimeSwitchs;
            if (opts == null)
            {
                opts = this.Switchs;
            }
            if (opts.AllocationCallStack 
                && method.ReturnTypeInfo == DCILTypeReference.Type_String
                && ((DCILClass)method.Parent ).InnerGenerate == false )
            {
                var targetMethod =(DCILMethod) this._Type_JIEJIEHelper.LocalClass.ChildNodes.GetByName("CloneStringCrossThead");
                // 加密关键字符串对象创建调用堆栈
                for (int ilIndex = method.OperCodes.Count - 1; ilIndex >= 0; ilIndex--)
                {
                    var ilcode = method.OperCodes[ilIndex];
                    if (ilcode.OperCodeValue == DCILOpCodeValue.Ret)
                    {
                        method.ILCodesModified = true;
                        method.OperCodes.Insert(
                            ilIndex,
                            new DCILOperCode_HandleMethod(method.GenNewLabelID(), "call", targetMethod));
                            //new DCILOperCode(
                            //    "IL_zzzzz",
                            //    "call",
                            //    "string " + _ClassName_JIEJIEHelper + "::CloneStringCrossThead(string)"));
                        _ModifiedCount++;
                        break;
                    }
                }
            }
            //if (opts.ControlFlow && method.OperCodeSpecifyStructure == false)
            //{
            //    if (method.OperCodes != null && method.OperCodes.Count > 0)
            //    {
            //        ObfuscateOperCodeList(method);
            //    }
            //}
        }

        private void HandleClass(DCILClass cls)
        {
            if (cls.IsImport)
            {
                //COM导入的类型不进行任何处理
                return;
            }
            var opts = cls.RuntimeSwitchs;
            if (opts.Resources)
            {
                ChangeComponentResourceManager(cls);
            }
            if (opts.MemberOrder)
            {
                ObfuseClassMembers(cls);
            }
            foreach (var item in cls.ChildNodes)
            {
                if (item is DCILMethod)
                {
                    HandleMethod((DCILMethod)item);
                }
                else if (item is DCILClass)
                {
                    HandleClass((DCILClass)item);
                }
            }
            if (cls.NestedClasses != null && cls.NestedClasses.Count > 0)
            {
                foreach (var cls2 in cls.NestedClasses)
                {
                    HandleClass(cls2);
                }
            }
        }

        private static readonly Dictionary<int, string> _IndexedName = new Dictionary<int, string>();
        public static string GetIndexName(int index)
        {
            string v = null;
            if (_IndexedName.TryGetValue(index, out v) == false)
            {
                v = "_" + index;
                _IndexedName[index] = v;
            }
            return v;
        }

        private ByteArrayDataContainer _ByteDataContainer = new ByteArrayDataContainer();


        private class ByteArrayDataContainer : IDisposable
        {
            public string FullClassName = DCJieJieNetEngine._ClassNamePrefix + "ByteArrayDataContainer";

            private List<int> _FieldIndexs = new List<int>();
            //public string GetFieldName(byte[] data)
            //{
            //    var index = IndexOf(data);
            //    if (_FieldIndexs.Contains(index) == false)
            //    {
            //        _FieldIndexs.Add(index);
            //    }
            //    return FullClassName + "::_" + index;
            //}
            public DCILOperCode_HandleMethod GetOperCode(string labelID, byte[] data)
            {
                int index = IndexOf(data);
                var code = new DCILOperCode_HandleMethod();
                code.LabelID = labelID;
                code.SetOperCode("call");
                code.InvokeInfo = new DCILInvokeMethodInfo();
                code.InvokeInfo.IsInstance = false;
                code.InvokeInfo.ReturnType = DCILTypeReference.Type_Boolean.Clone();
                code.InvokeInfo.ReturnType.ArrayAndPointerSettings = "[]";
                code.InvokeInfo.OwnerType = new DCILTypeReference(FullClassName, DCILTypeMode.Class);
                code.InvokeInfo.MethodName = GetIndexName(index);// "_" + index;
                _MyCodes.Add(code);
                return code;
            }
            private List<DCILOperCode_HandleMethod> _MyCodes = new List<DCILOperCode_HandleMethod>();
            public string GetMethodName(byte[] data)
            {
                return GetMethodName(IndexOf(data));
            }
            private string GetMethodName(int index)
            {
                return FullClassName + "::_" + index;
            }
            public bool HasData()
            {
                return _Datas.Count > 0;
            }
            public void Dispose()
            {
                if (this._Datas != null)
                {
                    this._Datas.Clear();
                    this._Datas = null;
                }
                if (this.LocalClass != null)
                {
                    this.LocalClass.Dispose();
                    this.LocalClass = null;
                }
                this.FullClassName = null;
            }
            private List<byte[]> _Datas = new List<byte[]>();
            public DCILClass LocalClass = null;
            private int IndexOf(byte[] bsData)
            {
                if (bsData == null || bsData.Length == 0)
                {
                    throw new ArgumentNullException("bsData");
                }
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    var item = _Datas[iCount];
                    if (item == bsData)
                    {
                        return iCount;
                    }
                    if (item.Length == bsData.Length)
                    {
                        continue;
                    }
                    int len = item.Length;
                    bool equals = true;
                    for (int iCount2 = 0; iCount2 < len; iCount2++)
                    {
                        if (item[iCount2] != bsData[iCount2])
                        {
                            equals = false;
                            break;
                        }
                    }
                    if (equals)
                    {
                        return iCount;
                    }
                }
                _Datas.Add(bsData);
                return _Datas.Count - 1;
            }

            public DCILClass WriteTo(DCILDocument document, DCJieJieNetEngine eng)
            {
                var str = new StringBuilder();
                var LibName_mscorlib = document.LibName_mscorlib;
                str.AppendLine(".class private auto ansi abstract sealed beforefieldinit " + FullClassName + " extends[" + LibName_mscorlib + "]System.Object");
                str.AppendLine("{");
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    str.AppendLine(@".class nested private explicit ansi sealed _DATA" + iCount + " extends[" + LibName_mscorlib + @"]System.ValueType
{
	.pack 1
    .size " + _Datas[iCount].Length + @"
}");
                }
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    str.AppendLine(".field assembly static initonly valuetype " + FullClassName + "/_DATA" + iCount + " _Field" + iCount + " at I_BDC" + iCount);
                }
                if (_FieldIndexs != null && _FieldIndexs.Count > 0)
                {
                    foreach (var index in _FieldIndexs)
                    {
                        str.AppendLine("	.field private static initonly uint8[] _" + index);
                    }
                    str.AppendLine(@".method private hidebysig specialname rtspecialname static  void .cctor () cil managed 
{
	.maxstack 8");
                    var tempMethod = new DCILMethod();
                    //var lableGen = new ILLabelIDGen();
                    foreach (var index in _FieldIndexs)
                    {
                        var field = eng.Int32ValueContainer.GetField(_Datas[index].Length);
                        if (field == null)
                        {
                            str.AppendLine(tempMethod.GenNewLabelID() + ": ldc.i4 " + _Datas[index].Length);
                        }
                        else
                        {
                            str.AppendLine(tempMethod.GenNewLabelID() + ": ldsfld int32 " + field.Parent.Name + "::" + field.Name);
                        }
                        str.AppendLine(tempMethod.GenNewLabelID() + ": newarr [" + LibName_mscorlib + "]System.Byte");
                        str.AppendLine(tempMethod.GenNewLabelID() + ": dup");
                        str.AppendLine(tempMethod.GenNewLabelID() + ": ldtoken field valuetype " + FullClassName + @"/_DATA" + index + " " + FullClassName + @"::_Field" + index);
                        //str.AppendLine(tempMethod.GenNewLabelID() + ": ldc.i4.0");
                        //str.AppendLine(tempMethod.GenNewLabelID() + ": call void " + _ClassName_JIEJIEHelper + @"::MyInitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle,int32)");
                        str.AppendLine(tempMethod.GenNewLabelID() + ": call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)");
                        str.AppendLine(tempMethod.GenNewLabelID() + ": stsfld uint8[] " + FullClassName + "::_" + index);
                    }
                    str.AppendLine(tempMethod.GenNewLabelID() + ": ret");
                    str.AppendLine("}// end of method .cctor ");
                }
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    if (_FieldIndexs.Contains(iCount))
                    {
                        continue;
                    }
                    str.AppendLine(@".method public hidebysig static uint8[] _" + iCount + @"() cil managed 
{
	.maxstack 3
	//.locals init (
	//	[0] uint8[]
	//)
	IL_0000: nop
	IL_0001: ldc.i4 " + _Datas[iCount].Length + @"
	IL_0002: newarr [" + LibName_mscorlib + @"]System.Byte
	IL_0007: dup
	IL_0008: ldtoken field valuetype " + FullClassName + @"/_DATA" + iCount + " " + FullClassName + @"::_Field" + iCount + @"
	//IL_0009: ldc.i4 0
    //IL_000d: call void " + _ClassName_JIEJIEHelper + "::MyInitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle, int32)
	IL_000d: call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)
	//IL_0012: stloc.0
	//IL_0013: br.s IL_0015
	//IL_0015: ldloc.0
	IL_0016: ret
}
");
                }
                str.AppendLine("}");
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    var item = new DCILData();
                    item._Name = "I_BDC" + iCount;
                    item.DataType = "bytearray";
                    item.Value = _Datas[iCount];
                    document.ILDatas.Add(item);
                    //datas[item.Name] = item;
                }
                var cls = new DCILClass(str.ToString(), document);
                document.Classes.Add(cls);
                document.ClearCacheForAllClasses();
                //var datas = new Dictionary<string, DCILData>();
                this.LocalClass = cls;
                cls.RuntimeSwitchs = eng.Switchs;
                var clses = document.GetAllClassesUseCache();
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        ((DCILMemberInfo)item).CacheInfo(document, clses);
                    }
                }
                if (_MyCodes.Count > 0)
                {
                    var t2 = new DCILTypeReference(cls);
                    foreach (var code in this._MyCodes)
                    {
                        code.InvokeInfo.LocalMethod = (DCILMethod)cls.ChildNodes.GetByName(code.InvokeInfo.MethodName);
                        code.InvokeInfo.OwnerType = t2;
                    }
                }
                return cls;
            }
        }

        public void ApplyResouceContainerClass()
        {
            int tick = Environment.TickCount;
            var cls_resIndex = new Dictionary<DCILClass, int>();
            var allRes = this.Document.GetNodeIndexs<DCILMResource>();
            var allResNames = new List<string>();
            var bmpTypeName = this.Document.GetTypeNameWithLibraryName(
               "System.Drawing.Bitmap",
               typeof(System.Drawing.Bitmap).Assembly.GetName().Name);
            foreach (var cls in this.Document.Classes)
            {
                if (cls.IsResoucePackage() == false)
                {
                    continue;
                }
                var resName = cls.Name + DCILMResource.EXT_Resources;// DCILDocument.EXT_resources;
                DCILMResource res = null;
                if (this.Document.Resouces.TryGetValue(resName, out res))
                {
                    if (res.ResourceValues == null || res.ResourceValues.Count == 0)
                    {
                        continue;
                    }
                    var resValues = new List<DCILMResource.MResourceItem>(res.ResourceValues.Values);
                    DCUtils.ObfuseListOrder(resValues);
                    var hasBmpValue = res.HasBmpValue;
                    var strNewClassCode = new StringBuilder();
                    var clsName = cls.Name;
                    strNewClassCode.AppendLine(".class " + clsName + " extends System.Object");
                    strNewClassCode.AppendLine("{");
                    //var strDataID = AllocID();
                    strNewClassCode.AppendLine("");
                    strNewClassCode.AppendLine(".field private static initonly uint8[] _Datas");
                    if (hasBmpValue)
                    {
                        foreach (var item in resValues)
                        {
                            if (item.IsBmp)
                            {
                                strNewClassCode.AppendLine(".field private static class " + bmpTypeName + " _" + item.Name);
                            }
                        }
                    }

                    strNewClassCode.AppendLine(@"
    .method private hidebysig specialname rtspecialname static  void .cctor () cil managed 
    {
	    .maxstack 8
	IL_0000: nop
	IL_0001: call uint8[] " + this._ByteDataContainer.GetMethodName(res.EncryptData()) + @"()
	IL_0006: stsfld uint8[] " + clsName + "::_Datas");

                    int labelCount = 100;
                    labelCount += 5; strNewClassCode.AppendLine("IL_" + labelCount.ToString("X4") + ": ret");
                    strNewClassCode.AppendLine("}");
                    if (hasBmpValue)
                    {
                        foreach (var item in resValues)
                        {
                            if (item.IsBmp)
                            {
                                strNewClassCode.AppendLine(@"
.method public hidebysig static class " + bmpTypeName + @" get_" + item.Name + @"() cil managed 
{
	.maxstack 4
	.locals init (
		[0] bool,
		[1] class " + bmpTypeName + @"
	)
	IL_0000: nop
	IL_0001: ldsfld class " + bmpTypeName + " " + clsName + "::_" + item.Name + @"
	IL_0006: ldnull
	IL_0007: ceq
	IL_0009: stloc.0
	IL_000a: ldloc.0
	IL_000b: brfalse.s IL_002d
	IL_000d: nop
	IL_000e: ldsfld uint8[] " + clsName + @"::_Datas
	IL_0013: ldc.i4 " + item.StartIndex + @"
	IL_0018: ldc.i4 " + item.BSLength + @"
	IL_001d: ldc.i4 " + item.Key + @"
	IL_0022: call class " + bmpTypeName + @" " + _ClassName_JIEJIEHelper + @"::GetBitmap(uint8[], int32, int32, int32)
	IL_0027: stsfld class " + bmpTypeName + @" " + clsName + @"::_" + item.Name + @"
	IL_002c: nop

	// return _Bmp1;
	IL_002d: ldsfld class " + bmpTypeName + @" " + clsName + @"::_" + item.Name + @"
	IL_0032: stloc.1
	// (no C# code)
	IL_0033: br.s IL_0035

	IL_0035: ldloc.1
	IL_0036: ret
}
");
                            }
                        }
                    }
                    foreach (var item in resValues)
                    {
                        if (item.IsBmp == false)
                        {
                            strNewClassCode.AppendLine(@"  .method assembly hidebysig static  string get_" + item.Name + @"() cil managed 
  {
	.maxstack 4
	//.locals init (
	//	[0] string
	//)
	IL_0000: nop
	IL_0001: ldsfld uint8[] " + clsName + @"::_Datas
	IL_0006: ldc.i4 " + item.StartIndex + @"
	IL_000b: ldc.i4 " + item.BSLength + @"
	IL_0010: ldc.i4 " + item.Key + @"
	IL_0015: call string " + _ClassName_JIEJIEHelper + @"::GetString(uint8[], int32, int32, int32)
	//IL_001a: stloc.0
	//IL_001b: br.s IL_001d
	//IL_001d: ldloc.0
	IL_001e: ret
   }");
                        }
                    }
                    strNewClassCode.AppendLine("}");
                    var strCodeText = strNewClassCode.ToString();
                    var newCls = new DCILClass(strCodeText, this.Document);
                    this.UpdateRuntimeSwitchs_Class(newCls, this.Switchs);
                    newCls.InnerGenerate = false;
                    cls.CustomAttributes = null;
                    cls.ChildNodes = newCls.ChildNodes;
                    cls.ObfuscationSettings = null;
                    foreach (DCILObject item in cls.ChildNodes)
                    {
                        item.Parent = cls;
                    }
                    this._ModifiedCount++;
                    this.Document.Resouces.Remove(resName);
                    var fn = Path.Combine(this.Document.RootPath, resName);
                    //if (File.Exists(fn))
                    //{
                    //    File.Delete(fn);
                    //}
                }
            }//for
        }

        private static readonly string LibNameForComponentResourceManager 
            = typeof(System.ComponentModel.ComponentResourceManager).Assembly.GetName().Name;
        public bool ChangeComponentResourceManager(DCILClass cls)
        {
            if (cls.BaseType != null && cls.IsInterface == false)
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod && item.Name == "InitializeComponent")
                    {
                        var method = (DCILMethod)item;
                        if (method.ReturnTypeInfo != DCILTypeReference.Type_Void)
                        {
                            continue;
                        }
                        DCILMResource res = null;
                        if (this.Document.Resouces.TryGetValue(cls.Name + DCILMResource.EXT_Resources, out res) == false)
                        {
                            continue;
                        }
                        if (method.Locals != null
                            && method.Locals.Count > 0
                            //&& method.Locals[0].ValueType.Name == "System.ComponentModel.ComponentResourceManager"
                            && method.OperCodes != null
                            && method.OperCodes.Count > 10)
                        {
                            int localIndex = -1;
                            for (int iCount2 = 0; iCount2 < method.Locals.Count; iCount2++)
                            {
                                var tn = method.Locals[iCount2].ValueType.Name;
                                if (tn == "System.ComponentModel.ComponentResourceManager"
                                    || tn == "System.Resources.ResourceManager")
                                {
                                    localIndex = iCount2;
                                    break;
                                }
                            }
                            if (localIndex < 0)
                            {
                                return false;
                            }
                            var codes = method.OperCodes;
                            int maxLen = Math.Min(10, codes.Count - 5);
                            for (int iCount = 0; iCount < maxLen; iCount++)
                            {
                                var ldtoken = codes[iCount] as DCILOperCode_LdToken;
                                if (ldtoken != null
                                    && ldtoken.ClassType?.Name == cls.Name
                                    && codes[iCount + 1].OperCodeValue == DCILOpCodeValue.Call
                                    && codes[iCount + 2].OperCodeValue == DCILOpCodeValue.Newobj)
                                {
                                    var code2 = codes[iCount + 2] as DCILOperCode_HandleMethod;
                                    if (code2.InvokeInfo.MethodName == ".ctor"
                                        && (code2.InvokeInfo.OwnerType.Name == "System.ComponentModel.ComponentResourceManager"
                                        || code2.InvokeInfo.OwnerType.Name == "System.Resources.ResourceManager"))
                                    {
                                        var bsWrite = GetBytesForWrite(res.Data);// GetGZipCompressedContentIfNeed(bs);
                                        string clsName = _ClassNamePrefix + "Res" + AllocIndex();
                                        string strNewClassCode = FixTypeLibNameForNetCore(_Code_Template_ComponentResourceManager);
                                        //strNewClassCode = strNewClassCode.Replace("mscorlib", this.Document.LibName_mscorlib);
                                        strNewClassCode = strNewClassCode.Replace("#CLASSNAME#", clsName);
                                        //strNewClassCode = strNewClassCode.Replace(
                                        //    "[mscorlib]System.Resources.ResourceSet",
                                        //    this.Document.GetTypeNameWithLibraryName("System.Resources.ResourceSet"));
                                        strNewClassCode = strNewClassCode.Replace(
                                            "[System]System.ComponentModel.ComponentResourceManager",
                                            this.Document.GetTypeNameWithLibraryName("System.ComponentModel.ComponentResourceManager",
                                            LibNameForComponentResourceManager));
                                        strNewClassCode = strNewClassCode.Replace("#ENCRYKEY#", bsWrite.Item2.ToString());
                                        strNewClassCode = strNewClassCode.Replace("#GETDATA#", this._ByteDataContainer.GetMethodName(bsWrite.Item1));
                                        if (bsWrite.Item3 == false)
                                        {
                                            strNewClassCode = strNewClassCode.Replace("#GZIPED#", "0");
                                        }
                                        else
                                        {
                                            strNewClassCode = strNewClassCode.Replace("#GZIPED#", "1");
                                        }

                                        var resCls = new DCILClass(strNewClassCode, this.Document);
                                        this.Document.Classes.Add(resCls);
                                        this.UpdateRuntimeSwitchs_Class(resCls, this.Switchs);
                                        _ModifiedCount++;
                                        code2.InvokeInfo = new DCILInvokeMethodInfo();
                                        code2.InvokeInfo.ReturnType = DCILTypeReference.Type_Void;
                                        code2.InvokeInfo.OwnerType = new DCILTypeReference(resCls.Name, DCILTypeMode.Class);
                                        code2.InvokeInfo.MethodName = ".ctor";
                                        code2.InvokeInfo.IsInstance = true;
                                        codes.RemoveAt(iCount + 1);
                                        codes.RemoveAt(iCount);
                                        method.Locals[localIndex].ValueType = new DCILTypeReference(resCls.Name, DCILTypeMode.Class);
                                        this.Document.Resouces.Remove(res.Name);
                                        var newType = new DCILTypeReference(resCls);
                                        foreach (var code in codes)
                                        {
                                            if (code.OperCodeValue == DCILOpCodeValue.Callvirt)
                                            {
                                                var callCode = (DCILOperCode_HandleMethod)code;
                                                if (callCode.MatchTypeAndMethod(
                                                    "System.ComponentModel.ComponentResourceManager",
                                                    "ApplyResources",
                                                    2))
                                                {
                                                    callCode.ChangeTarget(newType, "MyApplyResources");
                                                }
                                                else if (callCode.MatchTypeAndMethod("System.Resources.ResourceManager", "GetString", 1))
                                                {
                                                    callCode.ChangeTarget(newType, "MyGetString");
                                                }
                                            }
                                        }
                                        return true;
                                    }
                                }
                            }
                        }
                        if (res != null)
                        {

                        }
                        break;
                    }
                }
            }
            return false;
        }

        public static System.Tuple<byte[], byte, bool> GetBytesForWrite(byte[] bs)
        {
            var ms = new System.IO.MemoryStream();
            var stream = new System.IO.Compression.GZipStream(
                ms,
                System.IO.Compression.CompressionMode.Compress);
            stream.Write(bs, 0, bs.Length);
            stream.Close();
            var bsResult = ms.ToArray();
            var rate = (double)bs.Length / (double)bsResult.Length;
            bool gzip = bs.Length - bsResult.Length > 512 && rate > 2;
            if (gzip == false)
            {
                // 压缩造成的效益不够大
                bsResult = bs;
            }
            var key = (byte)(_Random.Next(100, 234));
            byte key2 = key;
            var bsResultLength = bsResult.Length;
            for (int iCount = 0; iCount < bsResultLength; iCount++, key2++)
            {
                bsResult[iCount] = (byte)(bsResult[iCount] ^ key2);
            }
            return new Tuple<byte[], byte, bool>(bsResult, key, gzip);
        }
        public void AddDatasClass()
        {
            if (this._ByteDataContainer != null && this._ByteDataContainer.HasData())
            {
                var cls = this._ByteDataContainer.WriteTo(this.Document, this);
                if (cls != null)
                {
                    UpdateRuntimeSwitchs_Class(cls, this.Switchs);
                    HandleClass(cls);
                }
            }
        }

        private void AddClassJIEJIEHelper()
        {
            var code = FixTypeLibNameForNetCore(_Code_Template_JIEJIEHelper);
            var cls = new DCILClass(code, this.Document);
            this.Document.Classes.Add(cls);
            this.Document.ClearCacheForAllClasses();
            this._Type_JIEJIEHelper = new DCILTypeReference(_ClassName_JIEJIEHelper, DCILTypeMode.Class);
            this._Type_JIEJIEHelper.LocalClass = cls;
            UpdateRuntimeSwitchs_Class(cls, this.Switchs);
        }
        private static SortedDictionary<string, string> _NetCore_Type_LibName = null;
        internal string FixTypeLibNameForNetCore(string strCode)
        {
#if DOTNETCORE
            if (_NetCore_Type_LibName == null)
            {
                _NetCore_Type_LibName = new SortedDictionary<string, string>();
                _NetCore_Type_LibName[typeof(string).FullName] = "System.Runtime";
                _NetCore_Type_LibName[typeof(System.Diagnostics.DebuggerBrowsableAttribute).FullName] = "System.Diagnostics.Debug";
                _NetCore_Type_LibName[typeof(System.Threading.Thread).FullName] = "System.Threading.Thread";
                _NetCore_Type_LibName[typeof(System.IO.MemoryStream).FullName] = "System.Runtime.Extensions";
                _NetCore_Type_LibName[typeof(System.Drawing.Bitmap).FullName] = "System.Drawing.Common";
                _NetCore_Type_LibName[typeof(System.Collections.Generic.List<>).FullName] = "System.Collections";
                var asms = System.Runtime.Loader.AssemblyLoadContext.Default.Assemblies;
                foreach (var asm in asms)
                {
                    var asmName = asm.GetName().Name;
                    if (asmName != null && asmName.StartsWith("System"))
                    {
                        var ts = asm.GetForwardedTypes();
                        if (ts != null && ts.Length > 0)
                        {
                            foreach (var t in ts)
                            {
                                var fn2 = DCUtils.GetFullName(t);
                                if (_NetCore_Type_LibName.ContainsKey(fn2) == false)
                                {
                                    _NetCore_Type_LibName[fn2] = asmName;
                                }
                            }
                        }
                    }
                }
            }
            
            var strResult = new System.Text.StringBuilder();
            var strChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.`0123456789";
            int lastContentIndex = 0;
            int lastSearchIndex = 0;
            int codeLength = strCode.Length;
            while( lastSearchIndex < codeLength  )
            {
                int index = strCode.IndexOf('[', lastSearchIndex);
                if( index >= 0 )
                {
                    lastSearchIndex = index + 1;
                    int index2 = strCode.IndexOf(']', index);
                    if (index2 > index + 1)
                    {
                        lastSearchIndex = index2 + 1;
                        var libName = strCode.Substring(index + 1, index2 - index - 1);
                        bool isSymbol = true;
                        foreach (var c in libName)
                        {
                            if (strChars.IndexOf(c) < 0)
                            {
                                isSymbol = false;
                                break;
                            }
                        }
                        if (isSymbol)
                        {
                            for (int iCount = index2 + 1; iCount < codeLength; iCount++)
                            {
                                if (strChars.IndexOf(strCode[iCount]) < 0)
                                {
                                    lastSearchIndex = iCount;
                                    if( iCount == index2 + 1 )
                                    {
                                        break;
                                    }
                                    var typeName = strCode.Substring(index2 + 1, iCount - index2 - 1);
                                    if( typeName.Length == 0 )
                                    {

                                    }
                                    if( typeName == "System.Drawing.Bitmap")
                                    {

                                    }
                                    if (typeName.Length > 0)
                                    {
                                        if( index > lastContentIndex )
                                        {
                                            var str6 = strCode.Substring(lastContentIndex, index - lastContentIndex);
                                            strResult.Append(str6);
                                            lastContentIndex = iCount;
                                        }
                                        var newLibName = this.Document.GetLibraryName(typeName);
                                        if (newLibName == null)
                                        {
                                            if (_NetCore_Type_LibName.TryGetValue(typeName, out newLibName) == false)
                                            {
                                                newLibName = this.Document.LibName_mscorlib;
                                            }
                                        }
                                        strResult.Append("[" + newLibName + "]");
                                        strResult.Append(typeName);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else if (index2 == index + 1)
                    {
                    }
                    else
                    { 
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            if( lastContentIndex < codeLength )
            {
                strResult.Append(strCode, lastContentIndex, codeLength - lastContentIndex);
            }
            return strResult.ToString();
#else
            return strCode;
#endif
        }
        private DCILTypeReference _Type_JIEJIEHelper = null;
        private static readonly string _ClassName_JIEJIEHelper = "__DC20211119.JIEJIEHelper";

        //******************************************************************************************
        //******************************************************************************************
        //******************************************************************************************

        private static readonly string _Code_Template_JIEJIEHelper = @"
.class private auto ansi abstract sealed beforefieldinit __DC20211119.JIEJIEHelper
	extends [mscorlib]System.Object
{
	// Nested Types
	.class nested private auto ansi sealed beforefieldinit SMF_ResStream
		extends [mscorlib]System.IO.Stream
	{
		// Fields
		.field private uint8[] _Content
		.field private int64 _Position

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor (
				uint8[] bs
			) cil managed 
		{
			// Method begins at RVA 0x26234
			// Code size 150 (0x96)
			.maxstack 5
			.locals init (
				[0] int32 gzipLen,
				[1] bool,
				[2] class [mscorlib]System.IO.MemoryStream msSource,
				[3] class [System]System.IO.Compression.GZipStream gm
			)

			IL_0000: ldarg.0
			IL_0001: ldnull
			IL_0002: stfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0007: ldarg.0
			IL_0008: ldc.i4.0
			IL_0009: conv.i8
			IL_000a: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_000f: ldarg.0
			IL_0010: call instance void [mscorlib]System.IO.Stream::.ctor()
			IL_0015: nop
			IL_0016: nop
			IL_0017: ldarg.1
			IL_0018: ldc.i4.0
			IL_0019: call int32 [mscorlib]System.BitConverter::ToInt32(uint8[], int32)
			IL_001e: stloc.0
			IL_001f: ldloc.0
			IL_0020: ldc.i4.0
			IL_0021: ceq
			IL_0023: stloc.1
			IL_0024: ldloc.1
			IL_0025: brfalse.s IL_0052

			IL_0027: nop
			IL_0028: ldarg.0
			IL_0029: ldarg.1
			IL_002a: ldlen
			IL_002b: conv.i4
			IL_002c: ldc.i4.4
			IL_002d: sub
			IL_002e: newarr [mscorlib]System.Byte
			IL_0033: stfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0038: ldarg.1
			IL_0039: ldc.i4.4
			IL_003a: ldarg.0
			IL_003b: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0040: ldc.i4.0
			IL_0041: ldarg.0
			IL_0042: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0047: ldlen
			IL_0048: conv.i4
			IL_0049: call void [mscorlib]System.Array::Copy(class [mscorlib]System.Array, int32, class [mscorlib]System.Array, int32, int32)
			IL_004e: nop
			IL_004f: nop
			IL_0050: br.s IL_0095

			IL_0052: nop
			IL_0053: ldarg.1
			IL_0054: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
			IL_0059: stloc.2
			IL_005a: ldloc.2
			IL_005b: ldc.i4.4
			IL_005c: conv.i8
			IL_005d: callvirt instance void [mscorlib]System.IO.Stream::set_Position(int64)
			IL_0062: nop
			IL_0063: ldloc.2
			IL_0064: ldc.i4.0
			IL_0065: newobj instance void [System]System.IO.Compression.GZipStream::.ctor(class [mscorlib]System.IO.Stream, valuetype [System]System.IO.Compression.CompressionMode)
			IL_006a: stloc.3
			IL_006b: ldarg.0
			IL_006c: ldloc.0
			IL_006d: newarr [mscorlib]System.Byte
			IL_0072: stfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0077: ldloc.3
			IL_0078: ldarg.0
			IL_0079: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_007e: ldc.i4.0
			IL_007f: ldloc.0
			IL_0080: callvirt instance int32 [mscorlib]System.IO.Stream::Read(uint8[], int32, int32)
			IL_0085: pop
			IL_0086: ldloc.3
			IL_0087: callvirt instance void [mscorlib]System.IO.Stream::Close()
			IL_008c: nop
			IL_008d: ldloc.2
			IL_008e: callvirt instance void [mscorlib]System.IO.Stream::Close()
			IL_0093: nop
			IL_0094: nop

			IL_0095: ret
		} // end of method SMF_ResStream::.ctor

		.method public hidebysig specialname virtual 
			instance bool get_CanRead () cil managed 
		{
			// Method begins at RVA 0x262d8
			// Code size 7 (0x7)
			.maxstack 1
			.locals init (
				[0] bool
			)

			IL_0000: nop
			IL_0001: ldc.i4.1
			IL_0002: stloc.0
			IL_0003: br.s IL_0005

			IL_0005: ldloc.0
			IL_0006: ret
		} // end of method SMF_ResStream::get_CanRead

		.method public hidebysig specialname virtual 
			instance bool get_CanSeek () cil managed 
		{
			// Method begins at RVA 0x262ec
			// Code size 7 (0x7)
			.maxstack 1
			.locals init (
				[0] bool
			)

			IL_0000: nop
			IL_0001: ldc.i4.1
			IL_0002: stloc.0
			IL_0003: br.s IL_0005

			IL_0005: ldloc.0
			IL_0006: ret
		} // end of method SMF_ResStream::get_CanSeek

		.method public hidebysig specialname virtual 
			instance bool get_CanWrite () cil managed 
		{
			// Method begins at RVA 0x26300
			// Code size 7 (0x7)
			.maxstack 1
			.locals init (
				[0] bool
			)

			IL_0000: nop
			IL_0001: ldc.i4.0
			IL_0002: stloc.0
			IL_0003: br.s IL_0005

			IL_0005: ldloc.0
			IL_0006: ret
		} // end of method SMF_ResStream::get_CanWrite

		.method public hidebysig specialname virtual 
			instance int64 get_Length () cil managed 
		{
			// Method begins at RVA 0x26314
			// Code size 15 (0xf)
			.maxstack 1
			.locals init (
				[0] int64
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0007: ldlen
			IL_0008: conv.i4
			IL_0009: conv.i8
			IL_000a: stloc.0
			IL_000b: br.s IL_000d

			IL_000d: ldloc.0
			IL_000e: ret
		} // end of method SMF_ResStream::get_Length

		.method public hidebysig specialname virtual 
			instance int64 get_Position () cil managed 
		{
			// Method begins at RVA 0x26330
			// Code size 12 (0xc)
			.maxstack 1
			.locals init (
				[0] int64
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0007: stloc.0
			IL_0008: br.s IL_000a

			IL_000a: ldloc.0
			IL_000b: ret
		} // end of method SMF_ResStream::get_Position

		.method public hidebysig specialname virtual 
			instance void set_Position (
				int64 'value'
			) cil managed 
		{
			// Method begins at RVA 0x26348
			// Code size 9 (0x9)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldarg.1
			IL_0003: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0008: ret
		} // end of method SMF_ResStream::set_Position

		.method public hidebysig virtual 
			instance void Flush () cil managed 
		{
			// Method begins at RVA 0x26352
			// Code size 2 (0x2)
			.maxstack 8

			IL_0000: nop
			IL_0001: ret
		} // end of method SMF_ResStream::Flush

		.method public hidebysig virtual 
			instance int32 Read (
				uint8[] buffer,
				int32 offset,
				int32 count
			) cil managed 
		{
			// Method begins at RVA 0x26358
			// Code size 127 (0x7f)
			.maxstack 5
			.locals init (
				[0] int32 len,
				[1] bool,
				[2] bool,
				[3] int32 endIndex,
				[4] int32 iCount,
				[5] bool,
				[6] int32
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0007: ldlen
			IL_0008: conv.i4
			IL_0009: conv.i8
			IL_000a: ldarg.0
			IL_000b: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0010: sub
			IL_0011: conv.i4
			IL_0012: stloc.0
			IL_0013: ldloc.0
			IL_0014: ldarg.3
			IL_0015: cgt
			IL_0017: stloc.1
			IL_0018: ldloc.1
			IL_0019: brfalse.s IL_001f

			IL_001b: nop
			IL_001c: ldarg.3
			IL_001d: stloc.0
			IL_001e: nop

			IL_001f: ldloc.0
			IL_0020: ldc.i4.0
			IL_0021: cgt
			IL_0023: stloc.2
			IL_0024: ldloc.2
			IL_0025: brfalse.s IL_0077

			IL_0027: nop
			IL_0028: ldarg.0
			IL_0029: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_002e: ldarg.0
			IL_002f: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0034: ldarg.1
			IL_0035: ldarg.2
			IL_0036: conv.i8
			IL_0037: ldloc.0
			IL_0038: conv.i8
			IL_0039: call void [mscorlib]System.Array::Copy(class [mscorlib]System.Array, int64, class [mscorlib]System.Array, int64, int64)
			IL_003e: nop
			IL_003f: ldarg.2
			IL_0040: ldloc.0
			IL_0041: add
			IL_0042: stloc.3
			IL_0043: ldarg.2
			IL_0044: stloc.s 4
			IL_0046: br.s IL_005c
			// loop start (head: IL_005c)
				IL_0048: nop
				IL_0049: ldarg.1
				IL_004a: ldloc.s 4
				IL_004c: ldarg.1
				IL_004d: ldloc.s 4
				IL_004f: ldelem.u1
				IL_0050: ldc.i4.s 123
				IL_0052: xor
				IL_0053: conv.u1
				IL_0054: stelem.i1
				IL_0055: nop
				IL_0056: ldloc.s 4
				IL_0058: ldc.i4.1
				IL_0059: add
				IL_005a: stloc.s 4

				IL_005c: ldloc.s 4
				IL_005e: ldloc.3
				IL_005f: clt
				IL_0061: stloc.s 5
				IL_0063: ldloc.s 5
				IL_0065: brtrue.s IL_0048
			// end loop

			IL_0067: ldarg.0
			IL_0068: ldarg.0
			IL_0069: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_006e: ldloc.0
			IL_006f: conv.i8
			IL_0070: add
			IL_0071: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0076: nop

			IL_0077: ldloc.0
			IL_0078: stloc.s 6
			IL_007a: br.s IL_007c

			IL_007c: ldloc.s 6
			IL_007e: ret
		} // end of method SMF_ResStream::Read

		.method public hidebysig virtual 
			instance int64 Seek (
				int64 offset,
				valuetype [mscorlib]System.IO.SeekOrigin origin
			) cil managed 
		{
			// Method begins at RVA 0x263e4
			// Code size 78 (0x4e)
			.maxstack 3
			.locals init (
				[0] valuetype [mscorlib]System.IO.SeekOrigin,
				[1] int64
			)

			IL_0000: nop
			IL_0001: ldarg.2
			IL_0002: stloc.0
			IL_0003: ldloc.0
			IL_0004: switch (IL_0017, IL_0020, IL_0030)

			IL_0015: br.s IL_0043

			IL_0017: ldarg.0
			IL_0018: ldarg.1
			IL_0019: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_001e: br.s IL_0043

			IL_0020: ldarg.0
			IL_0021: ldarg.0
			IL_0022: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0027: ldarg.1
			IL_0028: add
			IL_0029: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_002e: br.s IL_0043

			IL_0030: ldarg.0
			IL_0031: ldarg.0
			IL_0032: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0037: ldlen
			IL_0038: conv.i4
			IL_0039: conv.i8
			IL_003a: ldarg.1
			IL_003b: sub
			IL_003c: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0041: br.s IL_0043

			IL_0043: ldarg.0
			IL_0044: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0049: stloc.1
			IL_004a: br.s IL_004c

			IL_004c: ldloc.1
			IL_004d: ret
		} // end of method SMF_ResStream::Seek

		.method public hidebysig virtual 
			instance void SetLength (
				int64 'value'
			) cil managed 
		{
			// Method begins at RVA 0x2643e
			// Code size 7 (0x7)
			.maxstack 8

			IL_0000: nop
			IL_0001: newobj instance void [mscorlib]System.NotImplementedException::.ctor()
			IL_0006: throw
		} // end of method SMF_ResStream::SetLength

		.method public hidebysig virtual 
			instance void Write (
				uint8[] buffer,
				int32 offset,
				int32 count
			) cil managed 
		{
			// Method begins at RVA 0x26446
			// Code size 7 (0x7)
			.maxstack 8

			IL_0000: nop
			IL_0001: newobj instance void [mscorlib]System.NotImplementedException::.ctor()
			IL_0006: throw
		} // end of method SMF_ResStream::Write

		// Properties
		.property instance bool CanRead()
		{
			.get instance bool __DC20211119.JIEJIEHelper/SMF_ResStream::get_CanRead()
		}
		.property instance bool CanSeek()
		{
			.get instance bool __DC20211119.JIEJIEHelper/SMF_ResStream::get_CanSeek()
		}
		.property instance bool CanWrite()
		{
			.get instance bool __DC20211119.JIEJIEHelper/SMF_ResStream::get_CanWrite()
		}
		.property instance int64 Length()
		{
			.get instance int64 __DC20211119.JIEJIEHelper/SMF_ResStream::get_Length()
		}
		.property instance int64 Position()
		{
			.get instance int64 __DC20211119.JIEJIEHelper/SMF_ResStream::get_Position()
			.set instance void __DC20211119.JIEJIEHelper/SMF_ResStream::set_Position(int64)
		}

	} // end of class SMF_ResStream


	// Fields
	.field private static class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __SMF_Contents
	.field private static initonly class [mscorlib]System.Reflection.Assembly ThisAssembly
	.field private static class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_Thread
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner
	.field private static string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_CurrentValue

	// Methods
	.method private hidebysig static 
		class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> SMF_GetContents () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 42 (0x2a)
		.maxstack 2
		.locals init (
			[0] class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> result,
			[1] bool,
			[2] class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>
		)

		IL_0000: nop
		IL_0001: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: ldnull
		IL_0009: ceq
		IL_000b: stloc.1
		IL_000c: ldloc.1
		IL_000d: brfalse.s IL_0024

		IL_000f: nop
		IL_0010: newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>::.ctor()
		IL_0015: stloc.0
		IL_0016: ldloc.0
		IL_0017: call void __DC20211119.JIEJIEHelper::SMF_InitContent(class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>)
		IL_001c: nop
		IL_001d: ldloc.0
		IL_001e: stsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
		IL_0023: nop

		IL_0024: ldloc.0
		IL_0025: stloc.2
		IL_0026: br.s IL_0028

		IL_0028: ldloc.2
		IL_0029: ret
	} // end of method JIEJIEHelper::SMF_GetContents

	.method private hidebysig static 
		void SMF_InitContent (
			class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> contents
		) cil managed 
	{
		// Method begins at RVA 0x2086
		// Code size 2 (0x2)
		.maxstack 8

		IL_0000: nop
		IL_0001: ret
	} // end of method JIEJIEHelper::SMF_InitContent

	.method public hidebysig static 
		class [mscorlib]System.Reflection.ManifestResourceInfo SMF_GetManifestResourceInfo (
			class [mscorlib]System.Reflection.Assembly asm,
			string resourceName
		) cil managed 
	{
		// Method begins at RVA 0x208c
		// Code size 59 (0x3b)
		.maxstack 3
		.locals init (
			[0] bool,
			[1] class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> dic,
			[2] bool,
			[3] class [mscorlib]System.Reflection.ManifestResourceInfo
		)

		IL_0000: nop
		IL_0001: ldsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
		IL_0006: ldarg.0
		IL_0007: callvirt instance bool [mscorlib]System.Object::Equals(object)
		IL_000c: stloc.0
		IL_000d: ldloc.0
		IL_000e: brfalse.s IL_002f

		IL_0010: nop
		IL_0011: call class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::SMF_GetContents()
		IL_0016: stloc.1
		IL_0017: ldloc.1
		IL_0018: ldarg.1
		IL_0019: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>::ContainsKey(!0)
		IL_001e: stloc.2
		IL_001f: ldloc.2
		IL_0020: brfalse.s IL_002e

		IL_0022: nop
		IL_0023: ldarg.0
		IL_0024: ldarg.1
		IL_0025: ldc.i4.1
		IL_0026: newobj instance void [mscorlib]System.Reflection.ManifestResourceInfo::.ctor(class [mscorlib]System.Reflection.Assembly, string, valuetype [mscorlib]System.Reflection.ResourceLocation)
		IL_002b: stloc.3
		IL_002c: br.s IL_0039

		IL_002e: nop

		IL_002f: ldarg.0
		IL_0030: ldarg.1
		IL_0031: callvirt instance class [mscorlib]System.Reflection.ManifestResourceInfo [mscorlib]System.Reflection.Assembly::GetManifestResourceInfo(string)
		IL_0036: stloc.3
		IL_0037: br.s IL_0039

		IL_0039: ldloc.3
		IL_003a: ret
	} // end of method JIEJIEHelper::SMF_GetManifestResourceInfo

	.method public hidebysig static 
		string[] SMF_GetManifestResourceNames (
			class [mscorlib]System.Reflection.Assembly asm
		) cil managed 
	{
		// Method begins at RVA 0x20d4
		// Code size 53 (0x35)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> dic,
			[2] string[]
		)

		IL_0000: nop
		IL_0001: ldsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
		IL_0006: ldarg.0
		IL_0007: callvirt instance bool [mscorlib]System.Object::Equals(object)
		IL_000c: stloc.0
		IL_000d: ldloc.0
		IL_000e: brfalse.s IL_002a

		IL_0010: nop
		IL_0011: call class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::SMF_GetContents()
		IL_0016: stloc.1
		IL_0017: ldloc.1
		IL_0018: callvirt instance class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<!0, !1> class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>::get_Keys()
		IL_001d: newobj instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
		IL_0022: call instance !0[] class [mscorlib]System.Collections.Generic.List`1<string>::ToArray()
		IL_0027: stloc.2
		IL_0028: br.s IL_0033

		IL_002a: ldarg.0
		IL_002b: callvirt instance string[] [mscorlib]System.Reflection.Assembly::GetManifestResourceNames()
		IL_0030: stloc.2
		IL_0031: br.s IL_0033

		IL_0033: ldloc.2
		IL_0034: ret
	} // end of method JIEJIEHelper::SMF_GetManifestResourceNames

	.method public hidebysig static 
		class [mscorlib]System.IO.Stream SMF_GetManifestResourceStream (
			class [mscorlib]System.Reflection.Assembly asm,
			string resourceName
		) cil managed 
	{
		// Method begins at RVA 0x2118
		// Code size 64 (0x40)
		.maxstack 3
		.locals init (
			[0] bool,
			[1] class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> dic,
			[2] uint8[] bsContent,
			[3] bool,
			[4] class [mscorlib]System.IO.Stream
		)

		IL_0000: nop
		IL_0001: ldsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
		IL_0006: ldarg.0
		IL_0007: callvirt instance bool [mscorlib]System.Object::Equals(object)
		IL_000c: stloc.0
		IL_000d: ldloc.0
		IL_000e: brfalse.s IL_0032

		IL_0010: nop
		IL_0011: call class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::SMF_GetContents()
		IL_0016: stloc.1
		IL_0017: ldnull
		IL_0018: stloc.2
		IL_0019: ldloc.1
		IL_001a: ldarg.1
		IL_001b: ldloca.s 2
		IL_001d: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>::TryGetValue(!0, !1&)
		IL_0022: stloc.3
		IL_0023: ldloc.3
		IL_0024: brfalse.s IL_0031

		IL_0026: nop
		IL_0027: ldloc.2
		IL_0028: newobj instance void __DC20211119.JIEJIEHelper/SMF_ResStream::.ctor(uint8[])
		IL_002d: stloc.s 4
		IL_002f: br.s IL_003d

		IL_0031: nop

		IL_0032: ldarg.0
		IL_0033: ldarg.1
		IL_0034: callvirt instance class [mscorlib]System.IO.Stream [mscorlib]System.Reflection.Assembly::GetManifestResourceStream(string)
		IL_0039: stloc.s 4
		IL_003b: br.s IL_003d

		IL_003d: ldloc.s 4
		IL_003f: ret
	} // end of method JIEJIEHelper::SMF_GetManifestResourceStream

	.method public hidebysig static 
		class [mscorlib]System.IO.Stream SMF_GetManifestResourceStream2 (
			class [mscorlib]System.Reflection.Assembly asm,
			class [mscorlib]System.Type t,
			string resourceName
		) cil managed 
	{
		// Method begins at RVA 0x2164
		// Code size 125 (0x7d)
		.maxstack 4
		.locals init (
			[0] bool,
			[1] char,
			[2] bool,
			[3] bool,
			[4] class [mscorlib]System.IO.Stream
		)

		IL_0000: nop
		IL_0001: ldarg.2
		IL_0002: brfalse.s IL_000f

		IL_0004: ldarg.2
		IL_0005: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: br.s IL_0010

		IL_000f: ldc.i4.1

		IL_0010: stloc.0
		IL_0011: ldloc.0
		IL_0012: brfalse.s IL_0025

		IL_0014: nop
		IL_0015: ldc.i4.s 114
		IL_0017: stloc.1
		IL_0018: ldloca.s 1
		IL_001a: call instance string [mscorlib]System.Char::ToString()
		IL_001f: newobj instance void [mscorlib]System.ArgumentNullException::.ctor(string)
		IL_0024: throw

		IL_0025: ldarg.1
		IL_0026: ldnull
		IL_0027: ceq
		IL_0029: stloc.2
		IL_002a: ldloc.2
		IL_002b: brfalse.s IL_003e

		IL_002d: nop
		IL_002e: ldc.i4.s 116
		IL_0030: stloc.1
		IL_0031: ldloca.s 1
		IL_0033: call instance string [mscorlib]System.Char::ToString()
		IL_0038: newobj instance void [mscorlib]System.ArgumentNullException::.ctor(string)
		IL_003d: throw

		IL_003e: ldsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
		IL_0043: ldarg.0
		IL_0044: callvirt instance bool [mscorlib]System.Object::Equals(object)
		IL_0049: stloc.3
		IL_004a: ldloc.3
		IL_004b: brfalse.s IL_006e

		IL_004d: nop
		IL_004e: ldarg.0
		IL_004f: ldarg.1
		IL_0050: callvirt instance string [mscorlib]System.Type::get_FullName()
		IL_0055: ldc.i4.s 46
		IL_0057: stloc.1
		IL_0058: ldloca.s 1
		IL_005a: call instance string [mscorlib]System.Char::ToString()
		IL_005f: ldarg.2
		IL_0060: call string [mscorlib]System.String::Concat(string, string, string)
		IL_0065: call class [mscorlib]System.IO.Stream __DC20211119.JIEJIEHelper::SMF_GetManifestResourceStream(class [mscorlib]System.Reflection.Assembly, string)
		IL_006a: stloc.s 4
		IL_006c: br.s IL_007a

		IL_006e: ldarg.0
		IL_006f: ldarg.1
		IL_0070: ldarg.2
		IL_0071: callvirt instance class [mscorlib]System.IO.Stream [mscorlib]System.Reflection.Assembly::GetManifestResourceStream(class [mscorlib]System.Type, string)
		IL_0076: stloc.s 4
		IL_0078: br.s IL_007a

		IL_007a: ldloc.s 4
		IL_007c: ret
	} // end of method JIEJIEHelper::SMF_GetManifestResourceStream2

	.method public hidebysig static 
		string String_Trim (
			string v
		) cil managed 
	{
		// Method begins at RVA 0x21f0
		// Code size 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: callvirt instance string [mscorlib]System.String::Trim()
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method JIEJIEHelper::String_Trim

	.method public hidebysig static 
		class [mscorlib]System.Type Object_GetType (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x2208
		// Code size 12 (0xc)
		.maxstack 1
		.locals init (
			[0] class [mscorlib]System.Type
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: callvirt instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method JIEJIEHelper::Object_GetType

	.method public hidebysig static 
		void MyInitializeArray (
			class [mscorlib]System.Array v,
			valuetype [mscorlib]System.RuntimeFieldHandle fldHandle,
			int32 encKey
		) cil managed 
	{
		// Method begins at RVA 0x2220
		// Code size 137 (0x89)
		.maxstack 3
		.locals init (
			[0] bool,
			[1] uint8* ptr,
			[2] uint8[] pinned,
			[3] int32* ptr2,
			[4] int32* ptr3,
			[5] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array, valuetype [mscorlib]System.RuntimeFieldHandle)
		IL_0008: nop
		IL_0009: ldarg.0
		IL_000a: callvirt instance int32 [mscorlib]System.Array::get_Length()
		IL_000f: ldc.i4.4
		IL_0010: blt.s IL_0018

		IL_0012: ldarg.2
		IL_0013: ldc.i4.0
		IL_0014: ceq
		IL_0016: br.s IL_0019

		IL_0018: ldc.i4.1

		IL_0019: stloc.0
		IL_001a: ldloc.0
		IL_001b: brfalse.s IL_0020

		IL_001d: nop
		IL_001e: br.s IL_0088

		IL_0020: ldarg.0
		IL_0021: castclass uint8[]
		IL_0026: dup
		IL_0027: stloc.2
		IL_0028: brfalse.s IL_002f

		IL_002a: ldloc.2
		IL_002b: ldlen
		IL_002c: conv.i4
		IL_002d: brtrue.s IL_0034

		IL_002f: ldc.i4.0
		IL_0030: conv.u
		IL_0031: stloc.1
		IL_0032: br.s IL_003d

		IL_0034: ldloc.2
		IL_0035: ldc.i4.0
		IL_0036: ldelema [mscorlib]System.Byte
		IL_003b: conv.u
		IL_003c: stloc.1

		IL_003d: nop
		IL_003e: ldloc.1
		IL_003f: stloc.3
		IL_0040: ldloc.3
		IL_0041: ldarg.0
		IL_0042: callvirt instance int32 [mscorlib]System.Array::get_Length()
		IL_0047: conv.r8
		IL_0048: ldc.r8 4
		IL_0051: div
		IL_0052: call float64 [mscorlib]System.Math::Floor(float64)
		IL_0057: conv.i4
		IL_0058: conv.i
		IL_0059: ldc.i4.4
		IL_005a: mul
		IL_005b: add
		IL_005c: ldc.i4.4
		IL_005d: sub
		IL_005e: stloc.s 4
		IL_0060: br.s IL_0077
		// loop start (head: IL_0077)
			IL_0062: nop
			IL_0063: ldloc.s 4
			IL_0065: dup
			IL_0066: ldind.i4
			IL_0067: ldarg.2
			IL_0068: xor
			IL_0069: stind.i4
			IL_006a: ldarg.2
			IL_006b: ldc.i4.s 13
			IL_006d: add
			IL_006e: starg.s encKey
			IL_0070: nop
			IL_0071: ldloc.s 4
			IL_0073: ldc.i4.4
			IL_0074: sub
			IL_0075: stloc.s 4

			IL_0077: ldloc.s 4
			IL_0079: ldloc.3
			IL_007a: clt.un
			IL_007c: ldc.i4.0
			IL_007d: ceq
			IL_007f: stloc.s 5
			IL_0081: ldloc.s 5
			IL_0083: brtrue.s IL_0062
		// end loop

		IL_0085: nop
		IL_0086: ldnull
		IL_0087: stloc.2

		IL_0088: ret
	} // end of method JIEJIEHelper::MyInitializeArray

	.method public hidebysig static 
		string Int32_ToString (
			int32& v
		) cil managed 
	{
		// Method begins at RVA 0x22b8
		// Code size 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance string [mscorlib]System.Int32::ToString()
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method JIEJIEHelper::Int32_ToString

	.method public hidebysig static 
		bool String_Equality (
			string a,
			string b
		) cil managed 
	{
		// Method begins at RVA 0x22d0
		// Code size 13 (0xd)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call bool [mscorlib]System.String::Equals(string, string)
		IL_0008: stloc.0
		IL_0009: br.s IL_000b

		IL_000b: ldloc.0
		IL_000c: ret
	} // end of method JIEJIEHelper::String_Equality

	.method public hidebysig static 
		string String_ConcatObject (
			object a,
			object b
		) cil managed 
	{
		// Method begins at RVA 0x22ec
		// Code size 13 (0xd)
		.maxstack 2
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call string [mscorlib]System.String::Concat(object, object)
		IL_0008: stloc.0
		IL_0009: br.s IL_000b

		IL_000b: ldloc.0
		IL_000c: ret
	} // end of method JIEJIEHelper::String_ConcatObject

	.method public hidebysig static 
		string String_Concat3Object (
			object a,
			object b,
			object c
		) cil managed 
	{
		// Method begins at RVA 0x2308
		// Code size 14 (0xe)
		.maxstack 3
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call string [mscorlib]System.String::Concat(object, object, object)
		IL_0009: stloc.0
		IL_000a: br.s IL_000c

		IL_000c: ldloc.0
		IL_000d: ret
	} // end of method JIEJIEHelper::String_Concat3Object

	.method public hidebysig static 
		string String_Concat3String (
			string a,
			string b,
			string c
		) cil managed 
	{
		// Method begins at RVA 0x2324
		// Code size 14 (0xe)
		.maxstack 3
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call string [mscorlib]System.String::Concat(string, string, string)
		IL_0009: stloc.0
		IL_000a: br.s IL_000c

		IL_000c: ldloc.0
		IL_000d: ret
	} // end of method JIEJIEHelper::String_Concat3String

	.method public hidebysig static 
		string Object_ToString (
			object v
		) cil managed 
	{
		// Method begins at RVA 0x2340
		// Code size 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: callvirt instance string [mscorlib]System.Object::ToString()
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method JIEJIEHelper::Object_ToString

	.method public hidebysig static 
		bool String_IsNullOrEmpty (
			string v
		) cil managed 
	{
		// Method begins at RVA 0x2358
		// Code size 28 (0x1c)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldnull
		IL_0003: cgt.un
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: brfalse.s IL_0016

		IL_0009: nop
		IL_000a: ldarg.0
		IL_000b: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_0010: ldc.i4.0
		IL_0011: ceq
		IL_0013: stloc.1
		IL_0014: br.s IL_001a

		IL_0016: ldc.i4.1
		IL_0017: stloc.1
		IL_0018: br.s IL_001a

		IL_001a: ldloc.1
		IL_001b: ret
	} // end of method JIEJIEHelper::String_IsNullOrEmpty

	.method public hidebysig static 
		void Monitor_Enter (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x2380
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call void [mscorlib]System.Threading.Monitor::Enter(object)
		IL_0007: nop
		IL_0008: ret
	} // end of method JIEJIEHelper::Monitor_Enter

	.method public hidebysig static 
		void Monitor_Enter2 (
			object a,
			bool& lockTaken
		) cil managed 
	{
		// Method begins at RVA 0x238a
		// Code size 10 (0xa)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
		IL_0008: nop
		IL_0009: ret
	} // end of method JIEJIEHelper::Monitor_Enter2

	.method public hidebysig static 
		void Monitor_Exit (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x2395
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call void [mscorlib]System.Threading.Monitor::Exit(object)
		IL_0007: nop
		IL_0008: ret
	} // end of method JIEJIEHelper::Monitor_Exit

	.method public hidebysig static 
		string String_Concat (
			string a,
			string b
		) cil managed 
	{
		// Method begins at RVA 0x23a0
		// Code size 13 (0xd)
		.maxstack 2
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call string [mscorlib]System.String::Concat(string, string)
		IL_0008: stloc.0
		IL_0009: br.s IL_000b

		IL_000b: ldloc.0
		IL_000c: ret
	} // end of method JIEJIEHelper::String_Concat

	.method public hidebysig static 
		void MyDispose (
			class [mscorlib]System.IDisposable obj
		) cil managed 
	{
		// Method begins at RVA 0x23b9
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: callvirt instance void [mscorlib]System.IDisposable::Dispose()
		IL_0007: nop
		IL_0008: ret
	} // end of method JIEJIEHelper::MyDispose

	.method public hidebysig static 
		string CloneStringCrossThead (
			string txt
		) cil managed 
	{
		// Method begins at RVA 0x23c4
		// Code size 163 (0xa3)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] string,
			[2] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: brfalse.s IL_000f

		IL_0004: ldarg.0
		IL_0005: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: br.s IL_0010

		IL_000f: ldc.i4.1

		IL_0010: stloc.0
		IL_0011: ldloc.0
		IL_0012: brfalse.s IL_001c

		IL_0014: nop
		IL_0015: ldarg.0
		IL_0016: stloc.1
		IL_0017: br IL_00a1

		IL_001c: nop
		.try
		{
			IL_001d: nop
			IL_001e: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0023: call void [mscorlib]System.Threading.Monitor::Enter(object)
			IL_0028: nop
			IL_0029: ldarg.0
			IL_002a: volatile.
			IL_002c: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
			IL_0031: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
			IL_0036: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
			IL_003b: pop
			IL_003c: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0041: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0046: pop
			IL_0047: volatile.
			IL_0049: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_004e: ldnull
			IL_004f: ceq
			IL_0051: stloc.2
			IL_0052: ldloc.2
			IL_0053: brfalse.s IL_007c

			IL_0055: nop
			IL_0056: ldnull
			IL_0057: ldftn void __DC20211119.JIEJIEHelper::CloneStringCrossThead_Thread()
			IL_005d: newobj instance void [mscorlib]System.Threading.ThreadStart::.ctor(object, native int)
			IL_0062: newobj instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
			IL_0067: volatile.
			IL_0069: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_006e: volatile.
			IL_0070: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_0075: callvirt instance void [mscorlib]System.Threading.Thread::Start()
			IL_007a: nop
			IL_007b: nop

			IL_007c: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0081: ldc.i4.s 100
			IL_0083: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
			IL_0088: pop
			IL_0089: volatile.
			IL_008b: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
			IL_0090: stloc.1
			IL_0091: leave.s IL_00a1
		} // end .try
		finally
		{
			IL_0093: nop
			IL_0094: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0099: call void [mscorlib]System.Threading.Monitor::Exit(object)
			IL_009e: nop
			IL_009f: nop
			IL_00a0: endfinally
		} // end handler

		IL_00a1: ldloc.1
		IL_00a2: ret
	} // end of method JIEJIEHelper::CloneStringCrossThead

	.method private hidebysig static 
		void CloneStringCrossThead_Thread () cil managed 
	{
		// Method begins at RVA 0x2484
		// Code size 134 (0x86)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool,
			[2] bool
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: br.s IL_005d
			// loop start (head: IL_005d)
				IL_0004: nop
				IL_0005: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
				IL_000a: ldc.i4 1000
				IL_000f: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
				IL_0014: ldc.i4.0
				IL_0015: ceq
				IL_0017: stloc.0
				IL_0018: ldloc.0
				IL_0019: brfalse.s IL_001e

				IL_001b: nop
				IL_001c: br.s IL_0061

				IL_001e: volatile.
				IL_0020: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
				IL_0025: ldnull
				IL_0026: cgt.un
				IL_0028: stloc.1
				IL_0029: ldloc.1
				IL_002a: brfalse.s IL_0046

				IL_002c: nop
				IL_002d: volatile.
				IL_002f: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
				IL_0034: callvirt instance char[] [mscorlib]System.String::ToCharArray()
				IL_0039: newobj instance void [mscorlib]System.String::.ctor(char[])
				IL_003e: volatile.
				IL_0040: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
				IL_0045: nop

				IL_0046: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
				IL_004b: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
				IL_0050: pop
				IL_0051: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
				IL_0056: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
				IL_005b: pop
				IL_005c: nop

				IL_005d: ldc.i4.1
				IL_005e: stloc.2
				IL_005f: br.s IL_0004
			// end loop

			IL_0061: nop
			IL_0062: leave.s IL_0085
		} // end .try
		finally
		{
			IL_0064: nop
			IL_0065: ldnull
			IL_0066: volatile.
			IL_0068: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_006d: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0072: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0077: pop
			IL_0078: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
			IL_007d: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0082: pop
			IL_0083: nop
			IL_0084: endfinally
		} // end handler

		IL_0085: ret
	} // end of method JIEJIEHelper::CloneStringCrossThead_Thread

	.method public hidebysig static 
		string GetString (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x2528
		// Code size 81 (0x51)
		.maxstack 4
		.locals init (
			[0] int32 chrsLength,
			[1] char[] chrs,
			[2] int32 iCount,
			[3] int32 bi,
			[4] int32 v,
			[5] bool,
			[6] string
		)

		IL_0000: nop
		IL_0001: ldarg.2
		IL_0002: ldc.i4.2
		IL_0003: div
		IL_0004: stloc.0
		IL_0005: ldloc.0
		IL_0006: newarr [mscorlib]System.Char
		IL_000b: stloc.1
		IL_000c: ldc.i4.0
		IL_000d: stloc.2
		IL_000e: br.s IL_003a
		// loop start (head: IL_003a)
			IL_0010: nop
			IL_0011: ldarg.1
			IL_0012: ldloc.2
			IL_0013: ldc.i4.2
			IL_0014: mul
			IL_0015: add
			IL_0016: stloc.3
			IL_0017: ldarg.0
			IL_0018: ldloc.3
			IL_0019: ldelem.u1
			IL_001a: ldc.i4.8
			IL_001b: shl
			IL_001c: ldarg.0
			IL_001d: ldloc.3
			IL_001e: ldc.i4.1
			IL_001f: add
			IL_0020: ldelem.u1
			IL_0021: add
			IL_0022: stloc.s 4
			IL_0024: ldloc.s 4
			IL_0026: ldarg.3
			IL_0027: xor
			IL_0028: stloc.s 4
			IL_002a: ldloc.1
			IL_002b: ldloc.2
			IL_002c: ldloc.s 4
			IL_002e: conv.u2
			IL_002f: stelem.i2
			IL_0030: nop
			IL_0031: ldloc.2
			IL_0032: ldc.i4.1
			IL_0033: add
			IL_0034: stloc.2
			IL_0035: ldarg.3
			IL_0036: ldc.i4.1
			IL_0037: add
			IL_0038: starg.s key

			IL_003a: ldloc.2
			IL_003b: ldloc.0
			IL_003c: clt
			IL_003e: stloc.s 5
			IL_0040: ldloc.s 5
			IL_0042: brtrue.s IL_0010
		// end loop

		IL_0044: ldloc.1
		IL_0045: newobj instance void [mscorlib]System.String::.ctor(char[])
		IL_004a: stloc.s 6
		IL_004c: br.s IL_004e

		IL_004e: ldloc.s 6
		IL_0050: ret
	} // end of method JIEJIEHelper::GetString

	.method public hidebysig static 
		class [System.Drawing]System.Drawing.Bitmap GetBitmap (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x2588
		// Code size 66 (0x42)
		.maxstack 5
		.locals init (
			[0] uint8[] bs,
			[1] class [mscorlib]System.IO.MemoryStream ms,
			[2] class [System.Drawing]System.Drawing.Bitmap bmp,
			[3] int32 iCount,
			[4] bool,
			[5] class [System.Drawing]System.Drawing.Bitmap
		)

		IL_0000: nop
		IL_0001: ldarg.2
		IL_0002: newarr [mscorlib]System.Byte
		IL_0007: stloc.0
		IL_0008: ldc.i4.0
		IL_0009: stloc.3
		IL_000a: br.s IL_0022
		// loop start (head: IL_0022)
			IL_000c: nop
			IL_000d: ldloc.0
			IL_000e: ldloc.3
			IL_000f: ldarg.0
			IL_0010: ldarg.1
			IL_0011: ldloc.3
			IL_0012: add
			IL_0013: ldelem.u1
			IL_0014: ldarg.3
			IL_0015: xor
			IL_0016: conv.u1
			IL_0017: stelem.i1
			IL_0018: nop
			IL_0019: ldloc.3
			IL_001a: ldc.i4.1
			IL_001b: add
			IL_001c: stloc.3
			IL_001d: ldarg.3
			IL_001e: ldc.i4.1
			IL_001f: add
			IL_0020: starg.s key

			IL_0022: ldloc.3
			IL_0023: ldarg.2
			IL_0024: clt
			IL_0026: stloc.s 4
			IL_0028: ldloc.s 4
			IL_002a: brtrue.s IL_000c
		// end loop

		IL_002c: ldloc.0
		IL_002d: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0032: stloc.1
		IL_0033: ldloc.1
		IL_0034: newobj instance void [System.Drawing]System.Drawing.Bitmap::.ctor(class [mscorlib]System.IO.Stream)
		IL_0039: stloc.2
		IL_003a: ldloc.2
		IL_003b: stloc.s 5
		IL_003d: br.s IL_003f

		IL_003f: ldloc.s 5
		IL_0041: ret
	} // end of method JIEJIEHelper::GetBitmap

	.method public hidebysig static 
		class [mscorlib]System.Resources.ResourceSet LoadResourceSet (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x25d8
		// Code size 30 (0x1e)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.IO.Stream 'stream',
			[1] class [mscorlib]System.Resources.ResourceSet result,
			[2] class [mscorlib]System.Resources.ResourceSet
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call class [mscorlib]System.IO.Stream __DC20211119.JIEJIEHelper::GetStream(uint8[], uint8, bool)
		IL_0009: stloc.0
		IL_000a: ldloc.0
		IL_000b: newobj instance void [mscorlib]System.Resources.ResourceSet::.ctor(class [mscorlib]System.IO.Stream)
		IL_0010: stloc.1
		IL_0011: ldloc.0
		IL_0012: callvirt instance void [mscorlib]System.IO.Stream::Close()
		IL_0017: nop
		IL_0018: ldloc.1
		IL_0019: stloc.2
		IL_001a: br.s IL_001c

		IL_001c: ldloc.2
		IL_001d: ret
	} // end of method JIEJIEHelper::LoadResourceSet

	.method private hidebysig static 
		class [mscorlib]System.IO.Stream GetStream (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x2604
		// Code size 169 (0xa9)
		.maxstack 4
		.locals init (
			[0] int32 len,
			[1] class [mscorlib]System.IO.MemoryStream ms,
			[2] int32 iCount,
			[3] bool,
			[4] bool,
			[5] class [System]System.IO.Compression.GZipStream 'stream',
			[6] uint8[] bsTemp,
			[7] bool,
			[8] bool,
			[9] class [mscorlib]System.IO.Stream
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldlen
		IL_0003: conv.i4
		IL_0004: stloc.0
		IL_0005: ldc.i4.0
		IL_0006: stloc.2
		IL_0007: br.s IL_001e
		// loop start (head: IL_001e)
			IL_0009: nop
			IL_000a: ldarg.0
			IL_000b: ldloc.2
			IL_000c: ldarg.0
			IL_000d: ldloc.2
			IL_000e: ldelem.u1
			IL_000f: ldarg.1
			IL_0010: xor
			IL_0011: conv.u1
			IL_0012: stelem.i1
			IL_0013: nop
			IL_0014: ldloc.2
			IL_0015: ldc.i4.1
			IL_0016: add
			IL_0017: stloc.2
			IL_0018: ldarg.1
			IL_0019: ldc.i4.1
			IL_001a: add
			IL_001b: conv.u1
			IL_001c: starg.s key

			IL_001e: ldloc.2
			IL_001f: ldloc.0
			IL_0020: clt
			IL_0022: stloc.3
			IL_0023: ldloc.3
			IL_0024: brtrue.s IL_0009
		// end loop

		IL_0026: ldnull
		IL_0027: stloc.1
		IL_0028: ldarg.2
		IL_0029: stloc.s 4
		IL_002b: ldloc.s 4
		IL_002d: brfalse.s IL_0098

		IL_002f: nop
		IL_0030: ldarg.0
		IL_0031: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0036: ldc.i4.0
		IL_0037: newobj instance void [System]System.IO.Compression.GZipStream::.ctor(class [mscorlib]System.IO.Stream, valuetype [System]System.IO.Compression.CompressionMode)
		IL_003c: stloc.s 5
		IL_003e: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor()
		IL_0043: stloc.1
		IL_0044: ldc.i4 1024
		IL_0049: newarr [mscorlib]System.Byte
		IL_004e: stloc.s 6
		IL_0050: br.s IL_007f
		// loop start (head: IL_007f)
			IL_0052: nop
			IL_0053: ldloc.s 5
			IL_0055: ldloc.s 6
			IL_0057: ldc.i4.0
			IL_0058: ldloc.s 6
			IL_005a: ldlen
			IL_005b: conv.i4
			IL_005c: callvirt instance int32 [mscorlib]System.IO.Stream::Read(uint8[], int32, int32)
			IL_0061: stloc.0
			IL_0062: ldloc.0
			IL_0063: ldc.i4.0
			IL_0064: cgt
			IL_0066: stloc.s 7
			IL_0068: ldloc.s 7
			IL_006a: brfalse.s IL_007b

			IL_006c: nop
			IL_006d: ldloc.1
			IL_006e: ldloc.s 6
			IL_0070: ldc.i4.0
			IL_0071: ldloc.0
			IL_0072: callvirt instance void [mscorlib]System.IO.Stream::Write(uint8[], int32, int32)
			IL_0077: nop
			IL_0078: nop
			IL_0079: br.s IL_007e

			IL_007b: nop
			IL_007c: br.s IL_0084

			IL_007e: nop

			IL_007f: ldc.i4.1
			IL_0080: stloc.s 8
			IL_0082: br.s IL_0052
		// end loop

		IL_0084: ldloc.s 5
		IL_0086: callvirt instance void [mscorlib]System.IO.Stream::Close()
		IL_008b: nop
		IL_008c: ldloc.1
		IL_008d: ldc.i4.0
		IL_008e: conv.i8
		IL_008f: callvirt instance void [mscorlib]System.IO.Stream::set_Position(int64)
		IL_0094: nop
		IL_0095: nop
		IL_0096: br.s IL_00a1

		IL_0098: nop
		IL_0099: ldarg.0
		IL_009a: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_009f: stloc.1
		IL_00a0: nop

		IL_00a1: ldloc.1
		IL_00a2: stloc.s 9
		IL_00a4: br.s IL_00a6

		IL_00a6: ldloc.s 9
		IL_00a8: ret
	} // end of method JIEJIEHelper::GetStream

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x26bc
		// Code size 65 (0x41)
		.maxstack 1

		IL_0000: ldnull
		IL_0001: stsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
		IL_0006: ldtoken __DC20211119.JIEJIEHelper
		IL_000b: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
		IL_0010: callvirt instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()
		IL_0015: stsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
		IL_001a: ldnull
		IL_001b: volatile.
		IL_001d: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
		IL_0022: ldc.i4.0
		IL_0023: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_0028: stsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
		IL_002d: ldc.i4.0
		IL_002e: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_0033: stsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
		IL_0038: ldnull
		IL_0039: volatile.
		IL_003b: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
		IL_0040: ret
	} // end of method JIEJIEHelper::.cctor

} // end of class __DC20211119.JIEJIEHelper

";

        //******************************************************************************************
        //******************************************************************************************
        //******************************************************************************************
        //******************************************************************************************

        private static readonly string _Code_Template_ComponentResourceManager = @"
.class private auto ansi #CLASSNAME# extends [System]System.ComponentModel.ComponentResourceManager implements [mscorlib]System.IDisposable
{
  .field private class [mscorlib]System.Resources.ResourceSet _Data

.method assembly hidebysig 
	instance void MyApplyResources (
		object v2,
		string objectName
	) cil managed 
{
	.maxstack 8

	IL_0000: ldarg.0
	IL_0001: ldarg.1
	IL_0002: ldarg.2
	IL_0003: ldnull
	IL_0004: callvirt instance void [System]System.ComponentModel.ComponentResourceManager::ApplyResources(object, string, class [mscorlib]System.Globalization.CultureInfo)
	IL_0009: ret
}

.method assembly hidebysig 
	instance string MyGetString (
		string objectName
	) cil managed 
{
	.maxstack 8

	IL_0000: ldarg.0
	IL_0001: ldarg.1
	IL_0004: callvirt instance string [mscorlib]System.Resources.ResourceManager::GetString(string)
	IL_0009: ret
}


.method public hidebysig specialname rtspecialname 
	instance void .ctor () cil managed 
{
	// Method begins at RVA 0x2808
	// Code size 44 (0x2c)
	.maxstack 8
    IL_0000: ldarg.0
	IL_0001: call instance void [System]System.ComponentModel.ComponentResourceManager::.ctor()
	IL_0006: nop
	IL_0007: nop
	IL_0008: ldarg.0
	IL_0009: call uint8[] #GETDATA#()
	IL_000e: ldc.i4 #ENCRYKEY#
	IL_0013: ldc.i4.#GZIPED#
	IL_0014: call class [mscorlib]System.Resources.ResourceSet " + _ClassName_JIEJIEHelper + @"::LoadResourceSet(uint8[], uint8, bool)
	IL_0019: stfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	IL_002b: ret
} 

 .method public hidebysig virtual 
	instance class [mscorlib]System.Resources.ResourceSet GetResourceSet (
		class [mscorlib]System.Globalization.CultureInfo culture,
		bool createIfNotExists,
		bool tryParents
	) cil managed 
{
	// Method begins at RVA 0x27d8
	// Code size 12 (0xc)
	.maxstack 1
	.locals init (
		[0] class [mscorlib]System.Resources.ResourceSet
	)

	IL_0000: nop
	IL_0001: ldarg.0
	IL_0002: ldfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	IL_0007: stloc.0
	IL_0008: br.s IL_000a

	IL_000a: ldloc.0
	IL_000b: ret
}

  .method family hidebysig virtual 
	instance class [mscorlib]System.Resources.ResourceSet InternalGetResourceSet (
		class [mscorlib]System.Globalization.CultureInfo culture,
		bool createIfNotExists,
		bool tryParents
	) cil managed 
{
	// Method begins at RVA 0x27f0
	// Code size 12 (0xc)
	.maxstack 1
	.locals init (
		[0] class [mscorlib]System.Resources.ResourceSet
	)

	IL_0000: nop
	IL_0001: ldarg.0
	IL_0002: ldfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	IL_0007: stloc.0
	IL_0008: br.s IL_000a

	IL_000a: ldloc.0
	IL_000b: ret
} 

    .method public final hidebysig newslot virtual 
	    instance void Dispose () cil managed 
    {
	    // Method begins at RVA 0x2838
	    // Code size 36 (0x24)
	    .maxstack 2
	    .locals init (
		    [0] bool
	    )

	    IL_0000: nop
	    IL_0001: ldarg.0
	    IL_0002: ldfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	    IL_0007: ldnull
	    IL_0008: cgt.un
	    IL_000a: stloc.0
	    IL_000b: ldloc.0
	    IL_000c: brfalse.s IL_0023

	    IL_000e: nop
	    IL_000f: ldarg.0
	    IL_0010: ldfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	    IL_0015: callvirt instance void [mscorlib]System.Resources.ResourceSet::Close()
	    IL_001a: nop
	    IL_001b: ldarg.0
	    IL_001c: ldnull
	    IL_001d: stfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	    IL_0022: nop

	    IL_0023: ret
    } 

} // end of class WindowsFormsApp1.MyResourceManager";

        /// <summary>
        /// 混淆类型成员的顺序
        /// </summary>
        /// <param name="cls"></param>
        private void ObfuseClassMembers(DCILClass cls)
        {
            if (cls.IsInterface && cls.HasCustomAttributes)
            {
                foreach (var attr in cls.CustomAttributes)
                {
                    if (attr.AttributeTypeName == "System.Runtime.InteropServices.InterfaceTypeAttribute"
                        || attr.AttributeTypeName == "System.Runtime.InteropServices.StructLayoutAttribute")
                    {
                        // 对COM公开的接口,成员顺序不改变
                        return;
                    }
                }
            }
            if (cls.IsValueType)
            {
                // 对于结构体不混淆成员次序
                return;
            }

            var fields = new List<DCILObject>();
            var events = new List<DCILObject>();
            var methods = new List<DCILObject>();
            var properties = new List<DCILObject>();
            var nestedCls = new List<DCILObject>();
            var others = new List<DCILObject>();
            foreach (var item in cls.ChildNodes)
            {
                if (item is DCILField)
                {
                    fields.Add(item);
                }
                else if (item is DCILEvent)
                {
                    events.Add(item);
                }
                else if (item is DCILMethod)
                {
                    methods.Add(item);
                }
                else if (item is DCILProperty)
                {
                    properties.Add(item);
                }
                else if (item is DCILClass)
                {
                    nestedCls.Add(item);
                }
                else
                {
                    others.Add(item);
                }
            }
            DCUtils.ObfuseListOrder(fields);
            DCUtils.ObfuseListOrder(events);
            DCUtils.ObfuseListOrder(methods);
            DCUtils.ObfuseListOrder(properties);
            DCUtils.ObfuseListOrder(nestedCls);
            cls.ChildNodes.Clear();
            cls.ChildNodes.AddRange(nestedCls);
            cls.ChildNodes.AddRange(fields);
            cls.ChildNodes.AddRange(properties);
            cls.ChildNodes.AddRange(events);
            cls.ChildNodes.AddRange(methods);
            cls.ChildNodes.AddRange(others);
            this._ModifiedCount++;
        }
        private static readonly string _SwitchPrefix = "JIEJIE.NET.SWITCH:";
         
        private void UpdateRuntimeSwitchs()
        {
            if (this.Switchs == null)
            {
                this.Switchs = new JieJieSwitchs();
            }
            foreach (var cls in this.Document.Classes)
            {
                UpdateRuntimeSwitchs_Class(cls, this.Switchs);
            }
        }

        private void UpdateRuntimeSwitchs_Class(DCILClass cls, JieJieSwitchs parent)
        {
            JieJieSwitchs result = null;
            string strFeature = cls.ObfuscationSettings?.Feature;
            if (strFeature != null
                && strFeature.StartsWith(_SwitchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string strSettings = strFeature.Substring(_SwitchPrefix.Length);
                result = new JieJieSwitchs(strSettings, parent);
            }
            else
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILField)
                    {
                        var field = (DCILField)item;
                        if (field.IsConst && field.ValueType == DCILTypeReference.Type_String)
                        {
                            var cv = field.ConstValue;
                            if (cv != null && cv.Length > 3 && cv[0] == '"')
                            {
                                cv = cv.Substring(1, cv.Length - 2);
                                if (cv.StartsWith(_SwitchPrefix, StringComparison.OrdinalIgnoreCase))
                                {
                                    string strSettings = cv.Substring(_SwitchPrefix.Length);
                                    result = new JieJieSwitchs(strSettings, parent);
                                    break;
                                }
                            }
                        }
                    }
                }//foreach
                if (result == null)
                {
                    result = parent;
                }
            }
            cls.RuntimeSwitchs = result;
            if (cls.NestedClasses != null && cls.NestedClasses.Count > 0)
            {
                foreach (var cls2 in cls.NestedClasses)
                {
                    UpdateRuntimeSwitchs_Class(cls2, cls.RuntimeSwitchs);
                }
            }
            if (cls.ChildNodes != null && cls.ChildNodes.Count > 0)
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        UpdateRuntimeSwitchs_Method((DCILMethod)item, cls.RuntimeSwitchs);
                    }
                }
            }
        }

        private void UpdateRuntimeSwitchs_Method(DCILMethod method, JieJieSwitchs parent)
        {
            JieJieSwitchs result = null;
            var strFeature = method.ObfuscationSettings?.Feature;
            if (strFeature != null
                && strFeature.StartsWith(_SwitchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string strSettings = strFeature.Substring(_SwitchPrefix.Length);
                result = new JieJieSwitchs(strSettings, parent);
            }
            else if (method.OperCodes != null && method.OperCodes.Count > 0)
            {
                int len = method.OperCodes.Count;// Math.Min(1000, method.OperCodes.Count);
                for (int iCount = 0; iCount < len; iCount++)
                {
                    if (method.OperCodes[iCount] is DCILOperCode_LoadString)
                    {
                        var code = ((DCILOperCode_LoadString)method.OperCodes[iCount]);
                        var strCode = code.Value;
                        if (strCode != null && strCode.StartsWith(_SwitchPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            //code.OperCode = "\"\"";
                            code.ReplaceCode = new DCILOperCode(
                                null,
                                "ldsfld",
                                "string [" + this.Document.LibName_mscorlib + "]System.String::Empty");
                            code.Value = string.Empty;
                            string strSettings = strCode.Substring(_SwitchPrefix.Length);
                            result = new JieJieSwitchs(strSettings, parent);
                            break;
                        }
                    }
                }
            }
            if (result == null)
            {
                result = parent;
            }
            method.RuntimeSwitchs = result;
        }
        private void CollectOperCode<T>(DCILOperCodeList list, List<T> result) where T : DCILOperCode
        {
            foreach (var item in list)
            {
                if (item is T)
                {
                    result.Add((T)item);
                }
                else if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var code2 = (DCILOperCode_Try_Catch_Finally)item;
                    if (code2._Try != null && code2._Try.OperCodes != null)
                    {
                        CollectOperCode<T>(code2._Try.OperCodes, result);
                    }
                    if (code2._Catchs != null && code2._Catchs.Count > 0)
                    {
                        foreach (var item2 in code2._Catchs)
                        {
                            if (item2.OperCodes != null && item2.OperCodes.Count > 0)
                            {
                                CollectOperCode<T>(item2.OperCodes, result);
                            }
                        }
                    }
                    if (code2._Finally != null && code2._Finally.OperCodes != null)
                    {
                        CollectOperCode<T>(code2._Finally.OperCodes, result);
                    }
                    if (code2._fault != null && code2._fault.OperCodes != null)
                    {
                        CollectOperCode<T>(code2._fault.OperCodes, result);
                    }
                }
            }
        }

        private int Encrypt_ArrayDefine_Items(DCILMethod method, DCILOperCodeList items)
        {
            int result = 0;
            int itemsCount = items.Count;
            for (int codeIndex = 0; codeIndex < itemsCount; codeIndex++)
            {
                if (items[codeIndex] is DCILOperCode_Try_Catch_Finally)
                {
                    var tcf = (DCILOperCode_Try_Catch_Finally)items[codeIndex];
                    if (tcf._Try != null && tcf._Try.OperCodes != null)
                    {
                        result += Encrypt_ArrayDefine_Items(method, tcf._Try.OperCodes);
                    }
                    if (tcf._Catchs != null)
                    {
                        foreach (var item2 in tcf._Catchs)
                        {
                            if (item2.OperCodes != null)
                            {
                                result += Encrypt_ArrayDefine_Items(method, item2.OperCodes);
                            }
                        }
                    }
                    if (tcf._Finally != null && tcf._Finally.OperCodes != null)
                    {
                        result += Encrypt_ArrayDefine_Items(method, tcf._Finally.OperCodes);
                    }
                    if (tcf._fault != null && tcf._fault.OperCodes != null)
                    {
                        result += Encrypt_ArrayDefine_Items(method, tcf._fault.OperCodes);
                    }
                    continue;
                }
                var callCode = items[codeIndex] as DCILOperCode_HandleMethod;
                if (callCode != null
                    && callCode.MatchTypeAndMethod(
                        "System.Runtime.CompilerServices.RuntimeHelpers",
                        "InitializeArray",
                        2))
                {
                    method.MaxstackFix = 1;
                    var ldTokenCode = items[codeIndex - 1] as DCILOperCode_LdToken;
                    if (ldTokenCode != null && ldTokenCode.FieldReference.LocalField != null)
                    {
                        var fieldIndex = this.RFHContainer.GetFieldIndex(ldTokenCode.FieldReference.LocalField);
                        if (fieldIndex >= 0)
                        {
                            items[codeIndex - 1] = this.Int32ValueContainer.GetOperCode(ldTokenCode.LabelID, fieldIndex); ;
                            items.Insert(
                                codeIndex,
                                new DCILOperCode_HandleMethod(
                                    method.GenNewLabelID(),
                                    "call",
                                    this.RFHContainer._Method_GetHandle));
                            codeIndex++;
                        }
                    }
                    int encKey = 0;
                    for (int iCount = 0; iCount < 8; iCount++)
                    {
                        var index2 = codeIndex - iCount;
                        if (index2 >= 0)
                        {
                            var code10 = items[index2];
                            if (code10.OperCodeValue == DCILOpCodeValue.Newarr)
                            {
                                var clst = ((DCILOperCode_HandleClass)code10).ClassType;
                                if (clst != null && clst.Name == "System.Byte")
                                {
                                    // 定义字节数组
                                    var data = ldTokenCode.FieldReference.LocalField?.ReferenceData;
                                    if (data != null)
                                    {
                                        if (data.XORKey == 0)
                                        {
                                            data.XORKey = _Random.Next();
                                        }
                                        encKey = data.XORKey;
                                    }
                                }
                            }
                            else if ((code10.OperCodeValue == DCILOpCodeValue.Ldc_I4
                                || code10.OperCodeValue == DCILOpCodeValue.Ldc_I4_S))
                            {
                                var intValue = DCUtils.ConvertToInt32(code10.OperData);
                                this.Int32ValueContainer.ChangeOperCode(items, index2, intValue);
                                break;
                            }
                        }
                    }//for
                    items.Insert(codeIndex, this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), encKey));
                    callCode.ChangeTarget(this._Type_JIEJIEHelper, "MyInitializeArray");
                    codeIndex++;
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// 加密数组定义
        /// </summary>
        /// <param name="allMethods"></param>
        private void Encrypt_ArrayDefine(List<DCILMethod> allMethods)
        {
            if (this.Switchs.ControlFlow == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("MyInitializeArray");
                return;
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting array defines ...");
            int startTick = Environment.TickCount;
            int totalCount = 0;
            foreach (var method in allMethods)
            {
                if (method.RuntimeSwitchs.ControlFlow && method.OperCodes != null)
                {
                    totalCount += Encrypt_ArrayDefine_Items(method, method.OperCodes);
                }
            }
            if (totalCount == 0)
            {
                MyConsole.Instance.WriteLine(" do noting.");
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("MyInitializeArray");
            }
            else
            {
                MyConsole.Instance.WriteLine(" change " + totalCount
                    + " array defines,span " 
                    + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
        }
        /// <summary>
        /// 加密 lock/using 语法结构。
        /// </summary>
        /// <param name="allMethods"></param>
        private void Encrypt_Lock_Using_Structure(List<DCILMethod> allMethods)
        {
            if (this.Switchs.ControlFlow == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("Monitor_Enter");
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("Monitor_Enter2");
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("MyDispose");
                return;
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting lock()/using() structure...");
            int startTick = Environment.TickCount;
            bool hasOneP = false;
            bool hasTwoP = false;
            bool hasMyDispose = false;
            int totalCount = 0;
            foreach (var method in allMethods)
            {
                if (method.RuntimeSwitchs.ControlFlow == false)
                {
                    continue;
                }
                //if(method.Name == "RemoveTaskByTaskID")
                //{

                //}
                method.EnumOperCodes(delegate (EnumOperCodeArgs args)
                {
                    if (args.Current is DCILOperCode_HandleMethod)
                    {
                        var callCode = (DCILOperCode_HandleMethod)args.Current;
                        var targetMethod = callCode.InvokeInfo;
                        if (callCode.OperCodeValue == DCILOpCodeValue.Callvirt
                            && callCode.MatchTypeAndMethod("System.IDisposable", "Dispose", 0))
                        {
                            var preCode = args.OwnerList.SafeGet(args.CurrentCodeIndex - 1);
                            if (preCode != null && preCode.IsPrefixOperCode())
                            {
                                return;
                            }
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "MyDispose");
                            callCode.InvokeInfo.IsInstance = false;
                            callCode.SetOperCode("call");
                            hasMyDispose = true;
                            totalCount++;
                        }
                        else if (callCode.OperCodeValue == DCILOpCodeValue.Call
                            && targetMethod.OwnerType.Name == "System.Threading.Monitor"
                            && targetMethod.MethodName == "Enter")
                        {
                            if (targetMethod.ParametersCount == 1)
                            {
                                hasOneP = true;
                                callCode.ChangeTarget(this._Type_JIEJIEHelper, "Monitor_Enter");
                                totalCount++;
                            }
                            else if (targetMethod.ParametersCount == 2)
                            {
                                hasTwoP = true;
                                callCode.ChangeTarget(this._Type_JIEJIEHelper, "Monitor_Enter2");
                                totalCount++;
                            }
                        }
                    }
                });
            }
            if (hasOneP == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("Monitor_Enter");
            }
            if (hasTwoP == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("Monitor_Enter2");
            }
            if (hasMyDispose == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("MyDispose");
            }
            if (totalCount == 0)
            {
                MyConsole.Instance.WriteLine(" do nothings.");
            }
            else
            {
                MyConsole.Instance.WriteLine(" change " + totalCount
                    + " call/callvirt instructions.span " 
                    + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
        }
        /// <summary>
        /// 加密字符串操作
        /// </summary>
        /// <param name="allMethods"></param>
        internal void EncryptStringValues(List<DCILMethod> allMethods)
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting strings ...");
            var emptyILCode = new DCILOperCode(
                null,
                "ldsfld",
                "string [" + this.Document.LibName_mscorlib + "]System.String::Empty");
            var codeFields = new Dictionary<string, DCILOperCode_HandleField>();
            int totalCount = 0;
            int startTick = Environment.TickCount;

            var listTemp = new List<DCILMethod>(allMethods);
            foreach (var item in this._Type_JIEJIEHelper.LocalClass.ChildNodes)
            {
                if (item is DCILMethod)
                {
                    listTemp.Add((DCILMethod)item);
                }
            }
            allMethods = listTemp;

            foreach (var method in allMethods)
            {
                if (method.RuntimeSwitchs.Strings == false)
                {
                    continue;
                }
                method.OperCodes.EnumDeeply(method, delegate (EnumOperCodeArgs args)
                {
                    if (args.Current is DCILOperCode_LoadString)
                    {
                        totalCount++;
                        var ldCode = (DCILOperCode_LoadString)args.Current;
                        if (ldCode.Value.Length == 0)
                        {
                            ldCode.ReplaceCode = emptyILCode;
                        }
                        else
                        {
                            DCILOperCode_HandleField codeField = null;
                            if (codeFields.TryGetValue(ldCode.Value, out codeField) == false)
                            {
                                var field = new DCILField();
                                field.AddStyles("public", "static", "initonly");
                                field.ValueType = DCILTypeReference.Type_String;
                                field._Name = null;
                                codeField = new DCILOperCode_HandleField(null, "ldsfld", new DCILFieldReference(field));
                                codeFields[ldCode.Value] = codeField;
                            }
                            ldCode.ReplaceCode = codeField;
                            ldCode.Value = null;
                            ldCode.OperData = null;
                        }
                    }
                });
            }
            if (totalCount == 0)
            {
                MyConsole.Instance.WriteLine("Do nothings.");
            }
            else if (codeFields.Count == 0)
            {
                MyConsole.Instance.WriteLine(" Handle " + totalCount + " empty string values.");
            }
            else
            {
                var fields = new List<DCILField>();
                var strValues = new Dictionary<DCILField, string>();
                foreach (var item in codeFields)
                {
                    fields.Add(item.Value.Value.LocalField);
                    strValues[item.Value.Value.LocalField] = item.Key;
                }
                DCUtils.ObfuseListOrder(fields);
                int classCount = 0;
                var rnd = new System.Random();
                //string[] fieldNames = null;
                while (fields.Count > 0)
                {
                    var clsName = _ClassNamePrefix + "Strings" + Convert.ToString(classCount++);
                    var keyOffset = rnd.Next(10000, 99999);
                    var strNewClassILCode = @"
.class private auto ansi abstract sealed beforefieldinit MyClass extends [mscorlib]System.Object
{
.method private hidebysig specialname rtspecialname static void .cctor () cil managed 
{
	.maxstack 3
	.locals init (
		[0] uint8[] datas
	)
}// end of method

.method private hidebysig static string dcsoft (
		uint8[] datas,
		int64 key
	) cil managed 
{
	// Method begins at RVA 0x2bf4
	// Code size 118 (0x76)
	.maxstack 6
	.locals init (
		[0] int32 key2,
		[1] int32 length,
		[2] int32 startIndex,
		[3] char[] 'array',
		[4] int32 i,
		[5] int32 index2,
		[6] bool,
		[7] string
	)

	// (no C# code)
	IL_0000: nop
	// int num = (int)(key & 0xFFFF) ^ 0x270F;
	IL_0001: ldarg.1
	IL_0002: ldc.i4 65535
	IL_0007: conv.i8
	IL_0008: and
	IL_0009: conv.i4
	IL_000a: ldc.i4 " + keyOffset + @"
	IL_000f: xor
	IL_0010: stloc.0
	// key >>= 16;
	IL_0011: ldarg.1
	IL_0012: ldc.i4.s 16
	IL_0014: shr
	IL_0015: starg.s key
	// int num2 = (int)(key & 0xFFFFF);
	IL_0017: ldarg.1
	IL_0018: ldc.i4 1048575
	IL_001d: conv.i8
	IL_001e: and
	IL_001f: conv.i4
	IL_0020: stloc.1
	// key >>= 24;
	IL_0021: ldarg.1
	IL_0022: ldc.i4.s 24
	IL_0024: shr
	IL_0025: starg.s key
	// int num3 = (int)key;
	IL_0027: ldarg.1
	IL_0028: conv.i4
	IL_0029: stloc.2
	// char[] array = new char[num2];
	IL_002a: ldloc.1
	IL_002b: newarr [mscorlib]System.Char
	IL_0030: stloc.3
	// int num4 = 0;
	IL_0031: ldc.i4.0
	IL_0032: stloc.s 4
	// (no C# code)
	IL_0034: br.s IL_005e
	// loop start (head: IL_005e)
		IL_0036: nop
		// int num5 = num4 + num3 << 2;
		IL_0037: ldloc.s 4
		IL_0039: ldloc.2
		IL_003a: add
		IL_003b: ldc.i4.1
		IL_003c: shl
		IL_003d: stloc.s 5
		// array[num4] = (char)(((datas[num5] << 8) + datas[num5 + 1]) ^ num);
		IL_003f: ldloc.3
		IL_0040: ldloc.s 4
		IL_0042: ldarg.0
		IL_0043: ldloc.s 5
		IL_0045: ldelem.u1
		IL_0046: ldc.i4.8
		IL_0047: shl
		IL_0048: ldarg.0
		IL_0049: ldloc.s 5
		IL_004b: ldc.i4.1
		IL_004c: add
		IL_004d: ldelem.u1
		IL_004e: add
		IL_004f: ldloc.0
		IL_0050: xor
		IL_0051: conv.u2
		IL_0052: stelem.i2
		// (no C# code)
		IL_0053: nop
		// num4++;
		IL_0054: ldloc.s 4
		IL_0056: ldc.i4.1
		IL_0057: add
		IL_0058: stloc.s 4
		// num++;
		IL_005a: ldloc.0
		IL_005b: ldc.i4.1
		IL_005c: add
		IL_005d: stloc.0

		// while (num4 < num2)
		IL_005e: ldloc.s 4
		IL_0060: ldloc.1
		IL_0061: clt
		IL_0063: stloc.s 6
		IL_0065: ldloc.s 6
		IL_0067: brtrue.s IL_0036
	// end loop

	// return new string(array);
	IL_0069: ldloc.3
	IL_006a: newobj instance void [mscorlib]System.String::.ctor(char[])
	IL_006f: stloc.s 7
	// (no C# code)
	IL_0071: br.s IL_0073

	IL_0073: ldloc.s 7
	IL_0075: ret
}
}// end of class";
                    strNewClassILCode = strNewClassILCode.Replace("[mscorlib]", "[" + this.Document.LibName_mscorlib + "]");
                    var newClass = new DCILClass(strNewClassILCode, this.Document);
                    newClass._Name = _ClassNamePrefix + "Strings" + Convert.ToString(fields.Count);
                    var operCodes = new DCILOperCodeList();
                    newClass.Method_Cctor.OperCodes = operCodes;
                    var methodCctor = newClass.Method_Cctor;
                    var methodDecrypt = this.Document.CacheDCILInvokeMethodInfo(
                        new DCILInvokeMethodInfo((DCILMethod)newClass.ChildNodes.GetByName("dcsoft")));

                    this.Document.Classes.Add(newClass);
                    this.Document.ClearCacheForAllClasses();
                    this.UpdateRuntimeSwitchs_Class(newClass, this.Switchs);

                    //if (fieldNames == null)
                    //{
                    //    fieldNames = new string[110];
                    //    for (int iCount = 0; iCount < fieldNames.Length; iCount++)
                    //    {
                    //        fieldNames[iCount] = GetIndexName(iCount);// "_" + iCount.ToString();
                    //    }
                    //}
                    int itemNum = Math.Min(rnd.Next(50, 100), fields.Count);
                    var lstDatas = new List<byte>();
                    //var lbGen = new ILLabelIDGen();
                    for (int iCount = 0; iCount < itemNum; iCount++)
                    {
                        var field = fields[iCount];
                        field._Name = GetIndexName(iCount);// fieldNames[iCount];// "_" + iCount.ToString();
                        field.Parent = newClass;
                        newClass.ChildNodes.Add(field);
                        var strValue = strValues[field];
                        var bsContent = System.Text.Encoding.UTF8.GetBytes(strValue);
                        var itemEncryptKey = rnd.Next(10000, ushort.MaxValue - 10000);
                        long longKey = lstDatas.Count / 2;
                        longKey = (longKey << 24) + strValue.Length;
                        longKey = (longKey << 16) + (ushort)(itemEncryptKey ^ keyOffset);
                        var key = itemEncryptKey;
                        foreach (var c in strValue)
                        {
                            ushort v3 = (ushort)(c ^ key);
                            lstDatas.Add((byte)(v3 >> 8));
                            lstDatas.Add((byte)(v3 & 0xff));
                            key++;
                        }
                        operCodes.AddItem(methodCctor.GenNewLabelID(), "ldloc.0");
                        operCodes.AddItem(methodCctor.GenNewLabelID(), "ldc.i8", longKey.ToString());
                        operCodes.Add(new DCILOperCode_HandleMethod(methodCctor.GenNewLabelID(), "call", methodDecrypt));
                        operCodes.Add(new DCILOperCode_HandleField(methodCctor.GenNewLabelID(), "stsfld", new DCILFieldReference(field)));
                    }
                    operCodes.Insert(0, new DCILOperCode(methodCctor.GenNewLabelID(), "nop"));
                    operCodes.Insert(1, this._ByteDataContainer.GetOperCode(methodCctor.GenNewLabelID(), lstDatas.ToArray()));
                    operCodes.Insert(2, new DCILOperCode(methodCctor.GenNewLabelID(), "stloc.0"));
                    operCodes.AddItem(methodCctor.GenNewLabelID(), "ret");
                    fields.RemoveRange(0, itemNum);
                    if (this.Switchs.ControlFlow)
                    {
                        ObfuscateMethodOperCodes(methodCctor);
                    }
                }//while
                codeFields.Clear();
                MyConsole.Instance.WriteLine(" Handle " + totalCount + " ldstr instructions, " 
                    + strValues.Count + " string values , span "
                    + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
        }
        /// <summary>
        /// 需要进行加密的资源名称，支持正则表达式
        /// </summary>
        public string ResourceNameNeedEncrypt = null;

        internal void EncryptEmbeddedResource(List<DCILMethod> allMethod)
        {
            var rootCls = this._Type_JIEJIEHelper.LocalClass;
            bool isCancel = false;
            if (this.Document.RuntimeVersion != null)
            {
                if (this.Document.RuntimeVersion.StartsWith("v2.", StringComparison.OrdinalIgnoreCase)
                    || this.Document.RuntimeVersion.StartsWith("2."))
                {
                    // 遇到.NET2.0,则考虑是否取消加密内嵌资源，因为对于.NET2.0,类型System.Reflection.ManifestResourceInfo的构造函数是内部的，无法外部调用。
                    isCancel = true;
                    if (this.Document._CachedInvokeMethods != null)
                    {
                        bool hasMethod = false;
                        foreach (var item in this.Document._CachedInvokeMethods.Values)
                        {
                            if (item.MethodName == "GetManifestResourceInfo")
                            {
                                hasMethod = true;
                                break;
                            }
                        }
                        if (hasMethod == false)
                        {
                            // 所有的代码都没有调用GetManifestResourceInfo(),可以认为不会创建ManifestResourceInfo，可以加密内嵌资源。
                            isCancel = false;
                            rootCls.ChildNodes.RemoveByName("SMF_GetManifestResourceInfo");
                        }
                    }
                }
            }
            if (isCancel || this.Switchs.ControlFlow == false)
            {
                // 不需要处理任何数据,删除无用的功能模块
                for (int iCount = rootCls.ChildNodes.Count - 1; iCount >= 0; iCount--)
                {
                    if (rootCls.ChildNodes[iCount].Name.StartsWith("SMF_"))
                    {
                        rootCls.ChildNodes.RemoveAt(iCount);
                    }
                }
                var cls2 = rootCls.GetNestedClass("SMF_ResStream");
                if (cls2 != null)
                {
                    rootCls.NestedClasses.Remove(cls2);
                }
                return;
            }
            // 收集要处理的数据
            var datas = new Dictionary<string, DCILMResource>();
            System.Text.RegularExpressions.Regex nameRegex = null;
            if (this.ResourceNameNeedEncrypt != null && this.ResourceNameNeedEncrypt.Length > 0)
            {
                nameRegex = new System.Text.RegularExpressions.Regex(this.ResourceNameNeedEncrypt);
            }
            foreach (var item in this.Document.Resouces)
            {
                if (nameRegex != null && nameRegex.IsMatch(item.Key))
                {
                    // 匹配正则表达式
                    datas[item.Key] = item.Value;
                }
                else if (item.Key.EndsWith(".resources", StringComparison.OrdinalIgnoreCase) == false
                    && item.Key.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) == false
                    && item.Key.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) == false
                    && item.Value.Data != null
                    && item.Value.Data.Length > 0)
                {
                    datas[item.Key] = item.Value;
                }
            }//foreach
            if (datas.Count == 0)
            {
                // 没有任何要处理的数据
                return;
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting embedded resoruces ...");
            int startTick = Environment.TickCount;

            int changeCount = 0;
            foreach (var method in allMethod)
            {
                method.EnumOperCodes(delegate (EnumOperCodeArgs args)
                {
                    if (args.Current.OperCodeValue == DCILOpCodeValue.Callvirt
                        && args.Current is DCILOperCode_HandleMethod)
                    {
                        var hmcode = (DCILOperCode_HandleMethod)args.Current;
                        var targetMethod = hmcode.InvokeInfo;
                        string newMethodName = null;
                        if (targetMethod.OwnerType.Name == "System.Reflection.Assembly")
                        {
                            if (targetMethod.MethodName == "GetManifestResourceStream")
                            {
                                if (targetMethod.ParametersCount == 1)
                                {
                                    newMethodName = "SMF_GetManifestResourceStream";
                                }
                                else if (targetMethod.ParametersCount == 2)
                                {
                                    newMethodName = "SMF_GetManifestResourceStream2";
                                }
                            }
                            else if (targetMethod.MethodName == "GetManifestResourceNames")
                            {
                                newMethodName = "SMF_GetManifestResourceNames";
                            }
                            else if (targetMethod.MethodName == "GetManifestResourceInfo")
                            {
                                newMethodName = "SMF_GetManifestResourceInfo";
                            }
                        }
                        if (newMethodName != null)
                        {
                            targetMethod = targetMethod.Clone();
                            targetMethod.LocalMethod = (DCILMethod)this._Type_JIEJIEHelper.LocalClass.ChildNodes.GetByName(newMethodName);
                            targetMethod.OwnerType = this._Type_JIEJIEHelper;
                            targetMethod.IsInstance = false;
                            hmcode.SetOperCode("call");
                            hmcode.InvokeInfo = targetMethod;
                            changeCount++;
                        }
                    }
                });
            }
            if (changeCount == 0)
            {
                // 不需要处理任何数据,删除无用的功能模块
                for (int iCount = rootCls.ChildNodes.Count - 1; iCount >= 0; iCount--)
                {
                    if (rootCls.ChildNodes[iCount].Name.StartsWith("SMF_"))
                    {
                        rootCls.ChildNodes.RemoveAt(iCount);
                    }
                }
                var cls2 = rootCls.GetNestedClass("SMF_ResStream");
                if (cls2 != null)
                {
                    rootCls.NestedClasses.Remove(cls2);
                }
                MyConsole.Instance.WriteLine(" Do nothing.");
            }
            else
            {
                byte xorKey = (byte)(new System.Random().Next(100, 233));
                var methodRead = rootCls.GetNestedClass("SMF_ResStream")?.ChildNodes.GetByName("Read") as DCILMethod;
                if (methodRead != null)
                {
                    foreach (var item in methodRead.OperCodes)
                    {
                        if (item.OperCode.StartsWith("ldc.i4") && item.OperData == "123")
                        {
                            item.OperData = xorKey.ToString();
                            break;
                        }
                    }
                }
                var methodInitContent = (DCILMethod)rootCls.ChildNodes.GetByName("SMF_InitContent");
                var codesInit = methodInitContent.OperCodes;
                codesInit.Clear();
                //var lbGen = new ILLabelIDGen();
                codesInit.AddItem(methodInitContent.GenNewLabelID(), "nop");
                foreach (var item in datas)
                {
                    codesInit.AddItem(methodInitContent.GenNewLabelID(), "ldarg.0");
                    var resName = item.Key;
                    if (resName.StartsWith("'"))
                    {
                        resName = resName.Substring(1, resName.Length - 2);
                    }
                    codesInit.Add(new DCILOperCode_LoadString(methodInitContent.GenNewLabelID(), resName));
                    var data = item.Value.Data;
                    for (int iCount = 0; iCount < data.Length; iCount++)
                    {
                        // 首先是加密
                        data[iCount] = (byte)(data[iCount] ^ xorKey);
                    }
                    int gzipLen = 0;
                    if (data.Length > 50 * 1024)
                    {
                        var ms2 = new System.IO.MemoryStream();
                        var gm2 = new System.IO.Compression.GZipStream(ms2, System.IO.Compression.CompressionMode.Compress);
                        gm2.Write(data, 0, data.Length);
                        gm2.Flush();
                        gm2.Close();
                        var data2 = ms2.ToArray();
                        ms2.Close();
                        float rate = (float)data2.Length / (float)data.Length;
                        if (rate < 0.6)
                        {
                            // 压缩比率比较大，则采用GZIP压缩
                            gzipLen = data.Length;
                            data = data2;
                        }
                    }
                    var dataWrite = new byte[data.Length + 4];
                    var bsLen = BitConverter.GetBytes(gzipLen);
                    Array.Copy(bsLen, dataWrite, 4);
                    Array.Copy(data, 0, dataWrite, 4, data.Length);
                    codesInit.Add(this._ByteDataContainer.GetOperCode(methodInitContent.GenNewLabelID(), dataWrite));
                    codesInit.AddItem(methodInitContent.GenNewLabelID(), "callvirt", "instance void class [" + this.Document.LibName_mscorlib + "]System.Collections.Generic.Dictionary`2<string, uint8[]>::set_Item(!0, !1)");
                    codesInit.AddItem(methodInitContent.GenNewLabelID(), "nop");
                    item.Value.Dispose();
                    this.Document.Resouces.Remove(item.Key);
                }
                codesInit.AddItem(methodInitContent.GenNewLabelID(), "ret");
                MyConsole.Instance.WriteLine(" encrypt " + datas.Count + " resources ,span " + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
        }

        internal void EncryptCharValue(List<DCILMethod> methods)
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting char values ...");
            int codeCount = 0;
            int tick = Environment.TickCount;
            var strCodes = new HashSet<string>(new string[] {
                "add","and","beq","beq.s","bgt","bgt.s","ble","ble.s","blt","blt.s","bne","bne.s","ceq","cgt","clt" });
            foreach (var method in methods)
            {
                if (method.RuntimeSwitchs.Strings == false)
                {
                    continue;
                }
                method.EnumOperCodes(delegate (EnumOperCodeArgs args)
                {
                    if (args.Current.OperCodeValue == DCILOpCodeValue.Ldc_I4
                    || args.Current.OperCodeValue == DCILOpCodeValue.Ldc_I4_S)
                    {
                        var intv = DCUtils.ConvertToInt32(args.Current.OperData);
                        if (intv > 0 && intv <= char.MaxValue)
                        {
                            var preCode = args.OwnerList.SafeGet(args.CurrentCodeIndex - 1);
                            var nextCode = args.OwnerList.SafeGet(args.CurrentCodeIndex + 1);
                            var vt = args.Method.GetResultValueTypeForLoad(preCode);
                            if (preCode != null && vt == DCILTypeReference.Type_Char)
                            {
                                if (nextCode != null && strCodes.Contains(nextCode.OperCode))
                                {
                                    //args.OwnerList[args.CurrentCodeIndex] = this.Int32ValueContainer.GetOperCode(args.Current.LabelID, intv);
                                    if (this.Int32ValueContainer.ChangeOperCode(args.OwnerList, args.CurrentCodeIndex, intv))
                                    {
                                        codeCount++;
                                    }
                                }
                            }
                            else if (nextCode != null && nextCode.OperCode != null)
                            {
                                vt = args.Method.GetTargetValueTypeForSet(nextCode);
                                if (vt == DCILTypeReference.Type_Char)
                                {
                                    //args.OwnerList[args.CurrentCodeIndex] = this.Int32ValueContainer.GetOperCode(args.Current.LabelID, intv);
                                    if (this.Int32ValueContainer.ChangeOperCode(args.OwnerList, args.CurrentCodeIndex, intv))
                                    {
                                        codeCount++;
                                    }
                                }
                            }
                        }
                    }
                });
            }
            tick = Math.Abs(Environment.TickCount - tick);
            MyConsole.Instance.WriteLine(" change " + codeCount + " ldc.i4 instructions, span " + tick + " milliseconds.");
        }
        /// <summary>
        /// 混淆成员方法代码控制流程
        /// </summary>
        internal void ObfuscateControlFlow()
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Obfuscate control flow ...");
            int methodCount = 0;
            int tick = Environment.TickCount;
            var methods = this.Document.GetAllMethodHasOperCodes();
            if(methods != null && methods.Count > 0 )
            {
                foreach(var method in methods )
                {
                    if(method.RuntimeSwitchs.ControlFlow && method.OperCodeSpecifyStructure == false )
                    {
                        if(ObfuscateMethodOperCodes( method ))
                        {
                            methodCount++;
                        }
                    }
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
            MyConsole.Instance.WriteLine(" handle " + methodCount + " methods, span " + tick + " milliseconds.");
        }

        /// <summary>
        /// 加密方法参数中的枚举类型数值
        /// </summary>
        internal void EncryptMethodParamterEnumValue()
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting enum paramter values ...");
            int codeCount = 0;
            int tick = Environment.TickCount;
            this.Document.EnumAllOperCodes(delegate (EnumOperCodeArgs args)
            {
                if (args.Current is DCILOperCode_HandleMethod && args.CurrentCodeIndex > 0)
                {
                    var preCode = args.OwnerList[args.CurrentCodeIndex - 1];
                    if (preCode.OperCodeValue == DCILOpCodeValue.Ldc_I4
                        || preCode.OperCodeValue == DCILOpCodeValue.Ldc_I4_S)
                    {
                        var code = (DCILOperCode_HandleMethod)args.Current;
                        if (code.InvokeInfo != null && code.InvokeInfo.ParametersCount > 0)
                        {
                            var vt = code.InvokeInfo.Paramters[code.InvokeInfo.ParametersCount - 1].ValueType;
                            if (vt.Mode == DCILTypeMode.ValueType)
                            {
                                var intValue = DCUtils.ConvertToInt32(preCode.OperData);
                                //args.OwnerList[args.CurrentCodeIndex - 1] = this.Int32ValueContainer.GetOperCode(preCode.LabelID, intValue);
                                if (this.Int32ValueContainer.ChangeOperCode(args.OwnerList, args.CurrentCodeIndex - 1, intValue))
                                {
                                    codeCount++;
                                }
                            }
                        }
                    }
                }
            });
            tick = Math.Abs(Environment.TickCount - tick);
            MyConsole.Instance.WriteLine(" change " + codeCount + " call/callvirt instructions, span " + tick + " milliseconds.");
        }

        internal class OperCodeReference
        {
            public OperCodeReference(DCILMethod method, DCILOperCode code, DCILOperCodeList list, int index)
            {
                this.Method = method;
                this.Code = code;
                this.OwnerList = list;
                this.Index = index;
            }
            public DCILMethod Method = null;
            public DCILOperCode Code = null;
            public DCILOperCodeList OwnerList = null;
            public int Index = 0;
        }
        public void EncryptTypeHandle(List<DCILMethod> allMethods)
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting typeof() instructions ...");
            int tick = Environment.TickCount;

            var strNativeCodes = new Dictionary<DCILTypeReference, List<OperCodeReference>>();
            foreach (var method in allMethods)
            {
                if (method.RuntimeSwitchs.ControlFlow == false)
                {
                    continue;
                }
                method.EnumOperCodes(delegate (EnumOperCodeArgs args)
                {
                    var code = args.Current;
                    if (code is DCILOperCode_LdToken && args.CurrentCodeIndex < args.OwnerList.Count - 1)
                    {
                        var nextCode = args.OwnerList[args.CurrentCodeIndex + 1];
                        if (nextCode.OperCodeValue == DCILOpCodeValue.Call && nextCode is DCILOperCode_HandleMethod)
                        {
                            var mInfo = ((DCILOperCode_HandleMethod)nextCode).InvokeInfo;
                            if (mInfo.OwnerType.Name == "System.Type" && mInfo.MethodName == "GetTypeFromHandle")
                            {
                                var ldtoken = (DCILOperCode_LdToken)code;
                                if (ldtoken.ClassType != null)
                                {
                                    if (ldtoken.ClassType.IsGenericType
                                    || ldtoken.ClassType.Mode == DCILTypeMode.GenericTypeInMethodDefine
                                    || ldtoken.ClassType.Mode == DCILTypeMode.GenericTypeInTypeDefine)
                                    {
                                        return;
                                    }
                                    List<OperCodeReference> list4 = null;
                                    if (strNativeCodes.TryGetValue(ldtoken.ClassType, out list4) == false)
                                    {
                                        list4 = new List<OperCodeReference>();
                                        strNativeCodes[ldtoken.ClassType] = list4;
                                    }
                                    list4.Add(new OperCodeReference(method, code, args.OwnerList, args.CurrentCodeIndex));
                                }
                            }
                        }
                    }
                });
            }
            if (strNativeCodes.Count > 0)
            {
                var types = new List<DCILTypeReference>(strNativeCodes.Keys);
                DCUtils.ObfuseListOrder(types);
                var strILCode = @"
.class private auto ansi abstract sealed beforefieldinit __DC20210205._RuntimeTypeHandleContainer extends[mscorlib]System.Object
{
	.field private static initonly valuetype [mscorlib]System.RuntimeTypeHandle[] _Handles

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		.maxstack 4
		//.locals init (
		//	[0] valuetype [mscorlib]System.RuntimeTypeHandle h
		//)

		IL_0000: newobj instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.RuntimeTypeHandle>::.ctor()
		//IL_0005: ldloca.s 0
		//IL_0007: initobj [mscorlib]System.RuntimeFieldHandle
		//IL_000d: dup
		//IL_000e: ldloc.0
		//IL_000f: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.RuntimeTypeHandle>::Add(!0)
		IL_0014: callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.RuntimeTypeHandle>::ToArray()
		IL_0019: stsfld valuetype [mscorlib]System.RuntimeTypeHandle[] __DC20210205._RuntimeTypeHandleContainer::_Handles
		IL_001e: ret
	}
	.method public hidebysig static class [mscorlib]System.Type GetTypeInstance(
			int32 index
		) cil managed 
	{
		.maxstack 8
		IL_0000: ldsfld valuetype [mscorlib]System.RuntimeTypeHandle[] __DC20210205._RuntimeTypeHandleContainer::_Handles
		IL_0005: ldarg.0
		IL_0006: ldelem [mscorlib]System.RuntimeTypeHandle
        IL_0007: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
		IL_000b: ret
	}
}";
                strILCode = FixTypeLibNameForNetCore(strILCode);
                var clsContainer = new DCILClass(strILCode, this.Document);
                this.Document.Classes.Add(clsContainer);
                this.UpdateRuntimeSwitchs_Class(clsContainer, this.Switchs);
                var strListTypeName = this.Document.GetTypeNameWithLibraryName("System.Collections.Generic.List`1", null, typeof(System.Collections.Generic.List<>));
                var strAddCode = "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::Add(!0)";
                var addOperCode = new DCILOperCode(null, "callvirt", "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::Add(!0)");
                var method_Cctor = clsContainer.Method_Cctor;
                var operCodes = method_Cctor.OperCodes;
                var methodGetHandle = (DCILMethod)clsContainer.ChildNodes.GetByName("GetTypeInstance");
                //var labelGen = new ILLabelIDGen();
                methodGetHandle.CacheInfo(this.Document, this.Document.GetAllClassesUseCache());
                operCodes.Clear();
                operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "newobj", "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::.ctor()"));
                var array = new DCILField[types.Count];
                int fieldIndex = -1;
                int totalOperCodes = 0;
                foreach (var type in types)
                {
                    fieldIndex++;
                    operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "dup", null));
                    operCodes.Add(new DCILOperCode_LdToken(method_Cctor.GenNewLabelID(), type));
                    operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "callvirt", strAddCode));
                    var list = strNativeCodes[type];
                    totalOperCodes += list.Count;
                    foreach (var codeInfo in list)
                    {
                        codeInfo.OwnerList[codeInfo.Index] = this.Int32ValueContainer.GetOperCode(codeInfo.Code.LabelID, fieldIndex);
                        codeInfo.OwnerList[codeInfo.Index + 1] = new DCILOperCode_HandleMethod(codeInfo.Method.GenNewLabelID(), "call", methodGetHandle);
                    }
                }
                operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "callvirt", "instance !0[] class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::ToArray()"));
                operCodes.Add(new DCILOperCode_HandleField(method_Cctor.GenNewLabelID(), "stsfld", new DCILFieldReference((DCILField)clsContainer.ChildNodes.GetByName("_Handles"))));
                operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "ret", null));
                //ObfuscateMethodOperCodes(clsContainer.Method_Cctor);
                this.Document.ClearCacheForAllClasses();
                if(this.Switchs.ControlFlow )
                {
                    ObfuscateMethodOperCodes(method_Cctor);
                }
                MyConsole.Instance.WriteLine(" handle " + types.Count + " types , " + totalOperCodes + " instructions , span " + (Environment.TickCount - tick) + " milliseconds.");
            }
        }


        private DCRuntimeFieldHandleContainer _RFHContainer = null;
        public DCRuntimeFieldHandleContainer RFHContainer
        {
            get
            {
                if (this._RFHContainer == null)
                {
                    this._RFHContainer = new DCRuntimeFieldHandleContainer(this, this.Document);
                }
                return this._RFHContainer;
            }
        }
        internal class DCRuntimeFieldHandleContainer : IDisposable
        {
            public DCRuntimeFieldHandleContainer(DCJieJieNetEngine eng, DCILDocument document)
            {
                var strILCode = @"
.class private auto ansi abstract sealed beforefieldinit __DC20210205._RuntimeFieldHandleContainer extends [mscorlib]System.Object
{
	.field private static initonly valuetype [mscorlib]System.RuntimeFieldHandle[] _Handles

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		.maxstack 3
		.locals init (
			[0] valuetype [mscorlib]System.RuntimeFieldHandle h
		)

		IL_0000: newobj instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.RuntimeFieldHandle>::.ctor()
		//IL_0005: ldloca.s 0
		//IL_0007: initobj [mscorlib]System.RuntimeFieldHandle
		//IL_000d: dup
		//IL_000e: ldloc.0
		//IL_000f: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.RuntimeFieldHandle>::Add(!0)
		IL_0014: callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.RuntimeFieldHandle>::ToArray()
		IL_0019: stsfld valuetype [mscorlib]System.RuntimeFieldHandle[] __DC20210205._RuntimeFieldHandleContainer::_Handles
		IL_001e: ret
	}
	.method public hidebysig static 
		valuetype [mscorlib]System.RuntimeFieldHandle GetHandle (
			int32 index
		) cil managed 
	{
		// Method begins at RVA 0x207b
		// Code size 12 (0xc)
		.maxstack 8

		IL_0000: ldsfld valuetype [mscorlib]System.RuntimeFieldHandle[] __DC20210205._RuntimeFieldHandleContainer::_Handles
		IL_0005: ldarg.0
		IL_0006: ldelem [mscorlib]System.RuntimeFieldHandle
		IL_000b: ret
	}
}";
                strILCode = eng.FixTypeLibNameForNetCore(strILCode);
                this._Class = new DCILClass(strILCode, document);
                var fields = new List<DCILField>();
                foreach (var cls in document.GetAllClassesUseCache().Values)
                {
                    foreach (var item in cls.ChildNodes)
                    {
                        if (item is DCILField)
                        {
                            var field = (DCILField)item;
                            if (field.ReferenceData != null)//.DataLabel != null && field.DataLabel.Length > 0 )
                            {
                                fields.Add(field);
                            }
                        }
                    }
                }
                DCUtils.ObfuseListOrder(fields);
                for (int iCount = fields.Count - 1; iCount >= 0; iCount--)
                {
                    _FieldIndexs[fields[iCount]] = iCount;
                }
                document.Classes.Add(_Class);
                eng.UpdateRuntimeSwitchs_Class(this._Class, eng.Switchs);
                this._Class.OwnerDocument = document;
                this._Class.RuntimeSwitchs = eng.Switchs;
                document.ClearCacheForAllClasses();
                this._Method_GetHandle = (DCILMethod)this._Class.ChildNodes.GetByName("GetHandle");
                this._RFH = new DCILTypeReference(typeof(System.RuntimeFieldHandle), document);
            }
            public void Dispose()
            {
                if (this._Class != null)
                {
                    this._Class.Dispose();
                    this._Class = null;
                }
                this._Method_GetHandle = null;
                this._RFH = null;
                if (this._FieldIndexs != null)
                {
                    this._FieldIndexs.Clear();
                    this._FieldIndexs = null;
                }
            }
            public void Commit( DCJieJieNetEngine eng )
            {
                var doc = this._Class.OwnerDocument;
                var strListTypeName = doc.GetTypeNameWithLibraryName("System.Collections.Generic.List`1", null, typeof(System.Collections.Generic.List<>));
                var strAddCode = "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::Add(!0)";
                var addOperCode = new DCILOperCode(null, "callvirt", "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::Add(!0)");
                var method = this._Class.Method_Cctor;
                method.ObfuscateControlFlowFlag = false;
                var operCodes = method.OperCodes;
                operCodes.Clear();
                //var labelGen = new ILLabelIDGen();
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "newobj", "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::.ctor()"));
                var array = new DCILField[_FieldIndexs.Count];
                foreach (var item in _FieldIndexs)
                {
                    array[item.Value] = item.Key;
                }
                foreach (var item in array)
                {
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "dup", null));
                    operCodes.Add(new DCILOperCode_LdToken(method.GenNewLabelID(), new DCILFieldReference(item)));
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "callvirt", strAddCode));
                }
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "callvirt", "instance !0[] class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::ToArray()"));
                operCodes.Add(new DCILOperCode_HandleField(method.GenNewLabelID(), "stsfld", new DCILFieldReference((DCILField)this._Class.ChildNodes.GetByName("_Handles"))));
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "ret", null));
                if(eng.Switchs.ControlFlow)
                {
                    eng.ObfuscateMethodOperCodes(method);
                }
                //this._Class.Method_Cctor.CacheInfo(doc, doc.GetAllClassesUseCache());
            }

            public DCILMethod _Method_GetHandle = null;
            private Dictionary<DCILField, int> _FieldIndexs = new Dictionary<DCILField, int>();

            private DCILTypeReference _RFH = null;
            public DCILClass _Class = null;
            public int GetFieldIndex(DCILField field)
            {
                int v = 0;
                if (_FieldIndexs.TryGetValue(field, out v))
                {
                    return v;
                }
                else if (_FieldIndexs.Count < 10000)
                {
                    v = _FieldIndexs.Count;
                    _FieldIndexs[field] = v;
                    return v;
                }
                else
                {
                    // 超出处理范围
                    return -1;
                }
            }
        }

        private DCInt32ValueContainer _Int32ValueContainer = null;
        public DCInt32ValueContainer Int32ValueContainer
        {
            get
            {
                if (this._Int32ValueContainer == null)
                {
                    this._Int32ValueContainer = new DCInt32ValueContainer(this.Document, this);
                }
                return this._Int32ValueContainer;
            }
        }
        internal class DCInt32ValueContainer : IDisposable
        {
            public DCInt32ValueContainer(DCILDocument document, DCJieJieNetEngine eng)
            {
                if (document == null)
                {
                    throw new ArgumentNullException("document");
                }
                this._Class = new DCILClass(@"
.class private auto ansi abstract sealed beforefieldinit __DC20210205._Int32ValueContainer extends[" + document.LibName_mscorlib + @"]System.Object
{
    .method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		.maxstack 5
        IL_999999: ret
    }
}", document);
                document.Classes.Add(this._Class);
                document.ClearCacheForAllClasses();
                eng.UpdateRuntimeSwitchs_Class(this._Class , eng.Switchs);
                this.GetField(Environment.TickCount);
            }
            public void Dispose()
            {
                if (this._Class != null)
                {
                    this._Class.Dispose();
                    this._Class = null;
                }
                if (this._Fields != null)
                {
                    this._Fields.Clear();
                    this._Fields = null;
                }
            }
            public DCILClass _Class = null;
            private Dictionary<int, DCILField> _Fields = new Dictionary<int, DCILField>();
            public void Commit(DCJieJieNetEngine eng )
            {
                var method = this._Class.Method_Cctor;
                method.ObfuscateControlFlowFlag = false;
                var operCodes = method.OperCodes;
                operCodes.Clear();
                var list = new List<DCILField>(this._Fields.Values);
                DCUtils.ObfuseListOrder(list);
                int currentValue = Environment.TickCount;
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "ldc.i8", DCUtils.GetInt32ValueString(currentValue)));
                int listCount = list.Count - 1;
                for (int iCount = 0; iCount <= listCount; iCount++)
                {
                    var field = list[iCount];
                    int v = field.InnerTag - currentValue;
                    currentValue = field.InnerTag;
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "ldc.i8", DCUtils.GetInt32ValueString(v)));
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "add", null));
                    if (iCount < listCount)
                    {
                        operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "dup", null));
                    }
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "conv.i4", null));
                    operCodes.Add(new DCILOperCode_HandleField(method.GenNewLabelID(), "stsfld", new DCILFieldReference(field)));
                }
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "ret", null));
                if(eng.Switchs.ControlFlow )
                {
                    eng.ObfuscateMethodOperCodes(method);
                }
            }

            public DCILField GetField(int v)
            {
                DCILField field = null;
                if (this._Fields.TryGetValue(v, out field) == false)
                {
                    if (this._Fields.Count > 10000)
                    {
                        // 超出处理范围
                        return null;
                    }
                    field = new DCILField();
                    field.Parent = this._Class;
                    field.AddStyles("public", "static", "initonly");
                    field.ValueType = DCILTypeReference.Type_Int32;
                    //field._Name = GetIndexName(  this._Class.ChildNodes.Count );
                    if (v == int.MaxValue)
                    {
                        field._Name = "_IntMaxValue";
                    }
                    else if (v == int.MinValue)
                    {
                        field._Name = "_IntMinValue";
                    }
                    else if (v >= 0)
                    {
                        field._Name = "_" + this._Class.ChildNodes.Count + "_" + v;
                    }
                    else
                    {
                        field._Name = "_" + this._Class.ChildNodes.Count + "_N_" + Convert.ToString(-v);
                    }
                    field.InnerTag = v;
                    this._Class.ChildNodes.Add(field);
                    this._Fields[v] = field;
                }
                return field;
            }
            /// <summary>
            /// 修改指令
            /// </summary>
            /// <param name="list">指令列表</param>
            /// <param name="index">要修改的序号</param>
            /// <param name="intValue">整数数值</param>
            /// <returns>是否修改了指令列表</returns>
            public bool ChangeOperCode(DCILOperCodeList list, int index, int intValue)
            {
                var field = GetField(intValue);
                if (field != null)
                {
                    var oldCode = list[index];
                    var newCode = new DCILOperCode_HandleField(oldCode.LabelID, "ldsfld", new DCILFieldReference(field));
                    list[index] = newCode;
                    if (oldCode.OperCodeValue != DCILOpCodeValue.Ldc_I4)
                    {
                        //newCode.BitSizeChanged = true;
                        list.ItemBitSizeChanged = true;
                        list.ChangeShortInstruction();
                    }
                    return true;
                }
                return false;
            }

            public DCILOperCode GetOperCode(string labelID, int v)
            {
                var field = GetField(v);
                if (field == null)
                {
                    var code = new DCILOperCode(
                                labelID,
                                DCILOperCodeDefine._ldc_i4,
                                DCUtils.GetInt32ValueString(v));
                    //code.BitSizeChanged = true;
                    return code;
                }
                else
                {
                    var result = new DCILOperCode_HandleField(
                        labelID, 
                        DCILOperCodeDefine._ldsfld ,
                        new DCILFieldReference(field));
                    //result.BitSizeChanged = true;
                    return result;
                }
            }
        }
         
        private static readonly Random _Random = new Random();
         
        public bool ObfuscateMethodOperCodes(DCILMethod method)
        {
            if(method == null || method.ObfuscateControlFlowFlag == true )
            {
                return false;
            }
            method.ObfuscateControlFlowFlag = true;
            if( ObfuscateOperCodeListNew2( method , method.OperCodes , false , null , 0 ))
            //if (ObfuscateOperCodeList(method, method.OperCodes, false, null))
            {
                method.Maxstack += 3;
                //method.OperCodes.ChangeShortInstruction();
                return true;
            }
            return false;
        }

        //private ILLabelIDGen _LabelGen_MyInitializeArray = new ILLabelIDGen ("IL_MIA");
        private void ChangeSpecifyCallTarget(DCILOperCodeList items, DCILMethod method)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }
            if (this._Type_JIEJIEHelper == null)
            {
                throw new NotSupportedException(_ClassName_JIEJIEHelper);
            }
            if (method.Name == ".cctor")
            {
                // 遇到静态构造函数，则加密整数数值。
                for (int iCount = items.Count - 1; iCount >= 0; iCount--)
                {
                    var code = items[iCount];
                    if (code.OperCodeValue == DCILOpCodeValue.Ldc_I4)
                    {
                        var intValue = DCUtils.ConvertToInt32(code.OperData);
                        this.Int32ValueContainer.ChangeOperCode(items, iCount, intValue);
                        //items[iCount] = this.Int32ValueContainer.GetOperCode(code.LabelID, intValue);
                    }
                }
            }
        }

        private List<DCILOperCode_HandleMethod> _CallOperCodes = new List<DCILOperCode_HandleMethod>();
         
        private NumberNameGen _Int32ValueString = new NumberNameGen(null);
        private static List<string> codes_br_2 = new List<string>(new string[] { "ble", "bgt", "blt", "bne" });
        private static List<string> codes_brs_2 = new List<string>(new string[] { "ble.s", "bgt.s", "blt.s", "bne.s" });
        private static List<string> codes_max_2 = new List<string>(new string[] { "add", "sub", "div", "mul", "xor", "and" });


        public bool ObfuscateOperCodeListNew2(DCILMethod method, DCILOperCodeList items, bool isInTryBlock, DCILOperCodeList parentList, int level)
        {
            if (items == null || items.Count == 0)
            {
                return false;
            }
            //if(method.Name == "WriterViewControl_InnerOnPaint")
            //{

            //}
            bool result = false;
            bool isMethodOfInt32ValueContainer = method.Parent == this._Int32ValueContainer?._Class;

            if (isMethodOfInt32ValueContainer == false)
            {
                foreach (var item in items)
                {
                    if (item is DCILOperCode_Try_Catch_Finally)
                    {
                        var tcf = (DCILOperCode_Try_Catch_Finally)item;
                        if (tcf._Try != null && ObfuscateOperCodeListNew2(method, tcf._Try.OperCodes, true, items, level + 1))
                        {
                            items.ChangeShortInstruction();
                            result = true;
                        }
                        if (method.OwnerClass?.Name != _ClassName_JIEJIEHelper)
                        {
                            ChangeSpecifyCallTarget(tcf._Finally?.OperCodes, method);
                        }
                    }
                }
                if (method.OwnerClass?.Name != _ClassName_JIEJIEHelper)
                {
                    ChangeSpecifyCallTarget(items, method);
                }
            }
            foreach (var item in items)
            {
                if (item.OperCodeValue == DCILOpCodeValue.Call
                    || item.OperCodeValue == DCILOpCodeValue.Callvirt)
                {
                    var callCode = item as DCILOperCode_HandleMethod;
                    if (callCode != null && callCode.InvokeInfo != null)
                    {
                        var info = callCode.InvokeInfo;
                        var methodName = info.MethodName;
                        if (
                            methodName.StartsWith("get_", StringComparison.Ordinal)
                            || methodName.StartsWith("set_", StringComparison.Ordinal)
                            || methodName.StartsWith("add_", StringComparison.Ordinal)
                            || methodName.StartsWith("remove_", StringComparison.Ordinal)
                            )
                        {
                            var lm = info.LocalMethod;
                            if (lm != null && lm.IsInstance && lm.HasGenericStyle == false)
                            {
                                if (lm.Parent != method.Parent)
                                {
                                    lm.FlagsForPrivate = false;
                                }
                                this._CallOperCodes.Add(callCode);
                            }
                        }
                    }
                }
            }
            //var groupMaxLen = 20;// _Random.Next(20, 30);
            var groupMaxLen = _Random.Next(20, 30);
            if (groupMaxLen >= items.Count)
            {
                // 指令数量太少，不管了。
                if (result)
                {
                    //ChangeJumpCode(items);
                }
                return result;
            }
            int oldItemsCount = items.Count;
            //DCILOperCode retCode = null;
            //if (items[items.Count - 1].OperCode == "ret")
            //{
            //    retCode = items[items.Count - 1];
            //    items[items.Count - 1] = new DCILOperCode(this._labelGenGlobal.Gen(), "br", retCode.LabelID);
            //    //items.RemoveAt(items.Count - 1);
            //}
            var lastCode = items[items.Count - 1];
            if (lastCode.HasLabelID())
            {
                items.RemoveAt(items.Count - 1);
            }
            else
            {
                lastCode = null;
            }
            // 进行指令分组
            var groups = new List<DCILOperCodeList>();
            var newGroup = new DCILOperCodeList();
            groups.Add(newGroup);
            int stackLevel = 0;
            int byteSize = 0;
            int itemsCount = items.Count;
            //DCILOperCode preCode = null;
            //if (method.Name == "SplitQuoted" && method.Parent.Name == "Open_ICSharpCode_SharpZipLib_Core.NameFilter")
            //{

            //}
            //var def_br = DCILOperCodeDefine.GetDefine("br");
            //var def_br_s = DCILOperCodeDefine.GetDefine("br.s");
            //var def_pop = DCILOperCodeDefine.GetDefine("pop");
            //var def_dup = DCILOperCodeDefine.GetDefine("dup");
            //var def_brtrue = DCILOperCodeDefine.GetDefine("brtrue");
            //var def_brtrue_s = DCILOperCodeDefine.GetDefine("brtrue.s");

            var middleLabels = new List<string>();
            for (int iCount = 0; iCount < itemsCount; iCount++)
            {
                var item = items[iCount];
                if (newGroup.Count == 0 && item.HasLabelID() == false)
                {
                    newGroup.Add(new DCILOperCode(method.GenNewLabelID(), "nop", null));
                }
                newGroup.Add(item);

                stackLevel += item.StackOffset;
                byteSize += item.ByteSize;
                //if(newGroup.Count > groupMaxLen)
                //{
                //    if(item.NativeDefine == def_br || item.NativeDefine == def_br_s)
                //    {
                //        newGroup = new DCILOperCodeList();
                //        groups.Add(newGroup);
                //        preCode = item;
                //        continue;
                //    }
                //}
                if (stackLevel < 0)
                {

                }
                if ((item.NativeDefine == DCILOperCodeDefine._br_s || item.NativeDefine == DCILOperCodeDefine._br)
                    && iCount < itemsCount - 2
                    && items[iCount + 2].LabelID == item.OperData)
                {
                    if (items[iCount + 1].StackOffset == 1)// .OperCode == "ldc.i4.1" || items[iCount + 1].OperCode == "ldc.i4.0")
                    {
                        // 可能遇到一种特殊的结构
                        var lbl2 = items[iCount + 1].LabelID;
                        bool matchJump = false;
                        for (int iCount2 = iCount - 1; iCount2 >= 0; iCount2--)
                        {
                            var def3 = items[iCount2].NativeDefine;
                            if (def3.WithoutOperData == false
                                && (def3.ExtCodeType == ILOperCodeType.Jump || def3.ExtCodeType == ILOperCodeType.JumpShort)
                                && items[iCount2].OperData == lbl2)
                            {
                                matchJump = true;
                                break;
                            }
                        }
                        if (matchJump)
                        {
                            // 确定为特殊结构
                            newGroup.Add(items[iCount + 1]);
                            //newGroup.Add(items[iCount + 2]);
                            //stackLevel -= items[iCount + 2].StackOffset;
                            iCount += 1;
                            continue;
                        }
                    }
                }
                if (item.NativeDefine == DCILOperCodeDefine._dup)
                {
                    if (items.SafeGetNativeDefine(iCount + 1) == DCILOperCodeDefine._brtrue
                        || items.SafeGetNativeDefine(iCount + 1) == DCILOperCodeDefine._brtrue_s)
                    {
                        if (items.SafeGetNativeDefine(iCount + 2) == DCILOperCodeDefine._pop)
                        {
                            if (items.SafeGetNativeDefine(iCount + 3) == DCILOperCodeDefine._br
                                || items.SafeGetNativeDefine(iCount + 3) == DCILOperCodeDefine._br_s)
                            {
                                // 识别出一种特殊语法结构
                                var brItem = items[iCount + 3];
                                var labelID = brItem.OperData;
                                for (iCount++; iCount < itemsCount; iCount++)
                                {
                                    if (items[iCount].LabelID == labelID)
                                    {
                                        iCount--;
                                        //preCode = items[iCount - 1];
                                        newGroup = new DCILOperCodeList();
                                        groups.Add(newGroup);
                                        stackLevel = 0;
                                        byteSize = 0;
                                        break;
                                    }
                                    else
                                    {
                                        newGroup.Add(items[iCount]);
                                    }
                                }
                                continue;
                            }
                        }
                    }
                }
                if (stackLevel == 0)
                {

                }
                if (newGroup.Count > groupMaxLen
                    && stackLevel == 0
                    && item.IsPrefixOperCode() == false)
                {
                    newGroup = new DCILOperCodeList();
                    groups.Add(newGroup);
                }
            }

            if (groups[groups.Count - 1].Count == 0)
            {
                // 删除最后一个空白组
                groups.RemoveAt(groups.Count - 1);
            }
            if (groups.Count == 1)
            {
                // 只分得一组，无需混淆
                if (lastCode != null)
                {
                    items.Add(lastCode);
                }
                if (items.Count > 100)
                {
                    //var dt = items.GetDebugTextForStackOffset();
                    return ObfuscateOperCodeList_Rude(method, items, isInTryBlock, parentList, level);
                }
                return false;
            }
            var lastGroup = groups[groups.Count - 1];
            if (lastCode != null)
            {
                lastGroup.AddItem(method.GenNewLabelID(), DCILOperCodeDefine._br , lastCode.LabelID);
            }
            var resultGroups = new List<DCILOperCodeList>(groups);
            var nullCount = (int)_Random.Next(groups.Count, (int)(groups.Count * 1.5));
            for (int iCount = 0; iCount < nullCount; iCount++)
            {
                resultGroups.Add(null);
            }
            DCUtils.ObfuseListOrder(resultGroups);

            var resultList = new DCILOperCodeList();
            resultList.AddItem(method.GenNewLabelID(), "nop");
            var firstGIndex = resultGroups.IndexOf(groups[0]);
            if (isMethodOfInt32ValueContainer)
            {
                resultList.AddItem(
                    method.GenNewLabelID(),
                    DCILOperCodeDefine._ldc_i4,
                    DCUtils.GetInt32ValueString(firstGIndex));
            }
            else
            {
                resultList.Add(this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), firstGIndex));
            }
            method.MaxstackFix = 1;
            //var startLabels = new List<string>();
            //for(int iCount = 0;iCount < 4;iCount ++)
            //{
            //    //startLabels.Add(resultList.AddItem(method.GenNewLabelID(), "nop").LabelID);
            //    startLabels.Add(
            //        resultList.AddItem(
            //            method.GenNewLabelID(),
            //            "ldc.i4.s" , 
            //            DCUtils.GetInt32ValueString( _Random.Next( 0 , 100))).LabelID);
            //    resultList.AddItem(method.GenNewLabelID(), "pop" );
            //}
            string switchLabel = method.GenNewLabelID();
            //resultList.AddItem(method.GenNewLabelID(), "ldc.i4.1");
            //resultList.AddItem(method.GenNewLabelID(), "brtrue", switchLabel);
            resultList.AddItem(method.GenNewLabelID(), "br", switchLabel);
            //resultList.AddItem(method.GenNewLabelID(), "dup");
            string strPopLabel = resultList.AddItem(method.GenNewLabelID(), "pop").LabelID;
            var startSwitch = new DCILOperCode_Switch(switchLabel);// method.GenNewLabelID());
            resultList.Add(startSwitch);
            //startLabels.Add( startSwitch.LabelID );
            foreach (var group in resultGroups)
            {
                if (group == null)
                {
                    var g2 = groups[_Random.Next(0, groups.Count)];
                    startSwitch.TargetLabels.Add(g2.FirstLabelID);
                    continue;
                }
                if (group[0] is DCILOperCode_Try_Catch_Finally)
                {
                    group.Insert(0, new DCILOperCode(method.GenNewLabelID(), DCILOperCodeDefine._nop));
                }
                startSwitch.TargetLabels.Add(group.FirstLabelID);
                if (group != lastGroup)
                {
                    int gIndex = groups.IndexOf(group);
                    var nextGroup = groups[gIndex + 1];
                    gIndex = resultGroups.IndexOf(nextGroup);

                    if (isMethodOfInt32ValueContainer)
                    {
                        group.Add(DCILOperCode.Gen_ldci4_Code(method.GenNewLabelID(), gIndex));
                    }
                    else
                    {
                        group.Add(this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), gIndex));
                    }
                    if (_Random.Next(0, 100) > 50)
                    {
                        group.AddItem(method.GenNewLabelID(), DCILOperCodeDefine._br, startSwitch.LabelID);
                    }
                    else
                    {
                        if (isMethodOfInt32ValueContainer)
                        {
                            group.Add(DCILOperCode.Gen_ldci4_Code(method.GenNewLabelID(), _Random.Next(0, groups.Count)));
                        }
                        else
                        {
                            group.Add(this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), _Random.Next(0, groups.Count)));
                        }
                        group.AddItem(method.GenNewLabelID(), DCILOperCodeDefine._br, strPopLabel);
                    }
                    //group.AddItem(method.GenNewLabelID(), DCILOperCodeDefine._br, startLabels[_Random.Next(0, startLabels.Count)]);// startSwitch.LabelID);
                }
                resultList.AddRange(group);
            }
            if (lastCode != null)
            {
                resultList.Add(lastCode);
            }
            items.Clear();
            items.AddRange(resultList);
            items.OnModified();
            items.ChangeShortInstruction();
            int addCount = items.Count - oldItemsCount;
            return true;
        }
         
        private bool ObfuscateOperCodeList_Rude(DCILMethod method, DCILOperCodeList items, bool isInTryBlock, DCILOperCodeList parentList, int level)
        {

            if (items == null || items.Count == 0)
            {
                return false;
            }
            //if(method.Name == "WriterViewControl_InnerOnPaint")
            //{

            //}
            bool result = false;
            bool isMethodOfInt32ValueContainer = method.Parent == this._Int32ValueContainer?._Class;

            var groupMaxLen = 20;// _Random.Next(20, 30);
            if (groupMaxLen >= items.Count)
            {
                // 指令数量太少，不管了。
                if (result)
                {
                    //ChangeJumpCode(items);
                }
                return result;
            }
            int oldItemsCount = items.Count;
            //DCILOperCode retCode = null;
            //if (items[items.Count - 1].OperCode == "ret")
            //{
            //    retCode = items[items.Count - 1];
            //    items[items.Count - 1] = new DCILOperCode(this._labelGenGlobal.Gen(), "br", retCode.LabelID);
            //    //items.RemoveAt(items.Count - 1);
            //}
            var lastCode = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            // 进行指令分组
            var groups = new List<DCILOperCodeList>();
            var newGroup = new DCILOperCodeList();
            groups.Add(newGroup);
            int stackLevel = 0;
            int itemsCount = items.Count;
            DCILOperCode preCode = null;
            for (int iCount = 0; iCount < itemsCount; iCount++)
            {
                var item = items[iCount];
                if (newGroup.Count == 0 && item.HasLabelID() == false)
                {
                    newGroup.Add(new DCILOperCode(method.GenNewLabelID(), "nop", null));
                }
                newGroup.Add(item);

                stackLevel += item.StackOffset;

                if (newGroup.Count > groupMaxLen
                    //&& stackLevel == 0 
                    && item.IsPrefixOperCode() == false)
                {
                    newGroup = new DCILOperCodeList();
                    groups.Add(newGroup);
                }
                preCode = item;
            }

            if (groups[groups.Count - 1].Count == 0)
            {
                // 删除最后一个空白组
                groups.RemoveAt(groups.Count - 1);
            }
            if (groups.Count == 1)
            {
                // 只分得一组，无需混淆
                items.Add(lastCode);
                return false;
            }

            for (int iCount = 0; iCount < groups.Count - 1; iCount++)
            {
                groups[iCount].NextGroup = groups[iCount + 1];
            }
            var firstGroup = groups[0];
            string strSwitchLabelID = method.GenNewLabelID();
            var strSwitch = new StringBuilder();
            foreach (var g in groups)
            {
                if (strSwitch.Length > 0)
                {
                    strSwitch.Append(',');
                }
                strSwitch.Append(g[0].LabelID);
            }
            if (method.MaxstackFix < level * 3)
            {
                method.MaxstackFix = level * 3;
            }
            var extLocalIndex = int.MinValue;
            var extLocalValue = int.MinValue;
            DCILOperCodeList firstGroupShadow = null;
            if (isMethodOfInt32ValueContainer == false)
            {
                firstGroupShadow = new DCILOperCodeList();
                foreach (var item in firstGroup)
                {
                    if ((item is DCILOperCode_Try_Catch_Finally) == false)
                    {
                        var item2 = item.Clone(method.GenNewLabelID());// this._labelGenGlobal.Gen());
                        if (item2.OperCodeValue == DCILOpCodeValue.Ldc_I4
                            || item2.OperCodeValue == DCILOpCodeValue.Ldc_I4_S)
                        {
                            var v2 = DCUtils.ConvertToInt32(item2.OperData);
                            item2.SetOperCode("ldc.i4");
                            item2.OperData = DCUtils.GetInt32ValueString(v2 + _Random.Next(-100, 100));
                        }
                        if (codes_br_2.Contains(item2.OperCode))
                        {
                            item2.SetOperCode(codes_br_2[_Random.Next(0, codes_br_2.Count - 1)]);
                        }
                        else if (codes_brs_2.Contains(item2.OperCode))
                        {
                            item2.SetOperCode(codes_brs_2[_Random.Next(0, codes_brs_2.Count - 1)]);
                        }
                        else if (codes_max_2.Contains(item2.OperCode))
                        {
                            item2.SetOperCode(codes_max_2[_Random.Next(0, codes_max_2.Count - 1)]);
                        }
                        //else if(item2.OperCode == "ldc.i4.0")
                        //{
                        //    item2.OperCode = "ldc.i4.s";
                        //    item2.OperData = _Random.Next(-200, 200).ToString();
                        //}
                        else if (item2.OperCodeValue == DCILOpCodeValue.Brtrue
                            || item2.OperCodeValue == DCILOpCodeValue.Brtrue_S)
                        {
                            item2.SetOperCode("brfalse");
                        }
                        else if (item2.OperCodeValue == DCILOpCodeValue.Brfalse
                            || item2.OperCodeValue == DCILOpCodeValue.Brfalse_S)
                        {
                            item2.SetOperCode("brtrue");
                        }
                        //else if( item2.OperCode == "ldarg" || item2.OperCode.StartsWith("ldarg."))
                        //{
                        //    item2.OperCode = "ldc.i4";
                        //    item2.OperData = "10";
                        //}
                        firstGroupShadow.Add(item2);
                    }

                    //if(item is DCILOperCode_HandleMethod)
                    //{
                    //    break;
                    //}
                }
                if (isMethodOfInt32ValueContainer == false)
                {
                    extLocalIndex = method.AddExtLocalsIndex();
                    extLocalValue = _Random.Next(0, 2);
                }
                firstGroup.Insert(0, this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), extLocalValue));
                firstGroup.Insert(1, new DCILOperCode(method.GenNewLabelID(), "stloc", DCUtils.GetInt32ValueString(extLocalIndex)));

                firstGroupShadow.Insert(0, this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), extLocalValue == 0 ? 1 : 0));
                firstGroupShadow.Insert(1, new DCILOperCode(method.GenNewLabelID(), "stloc", DCUtils.GetInt32ValueString(extLocalIndex)));

                firstGroupShadow.NextGroup = firstGroup.NextGroup;

                groups.Add(firstGroupShadow);
            }
            else
            {
            }

            var resultList = new DCILOperCodeList();
            DCUtils.ObfuseListOrder(groups);
            string idForLastCode = lastCode.LabelID;
            if (idForLastCode == null || idForLastCode.Length == 0)
            {
                idForLastCode = method.GenNewLabelID();
            }

            if (firstGroupShadow != null)
            {
                var flag = _Random.Next(30, 100);
                var retLabelID = method.GenNewLabelID();
                resultList.Add(this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), flag));
                if (_Random.Next(0, 100) > 50)
                {
                    // 直接命中
                    resultList.AddItem(method.GenNewLabelID(), "ldc.i4", DCUtils.GetInt32ValueString(flag - 10));
                    resultList.AddItem(method.GenNewLabelID(), "bgt", firstGroup.FirstLabelID);
                    resultList.AddItem(method.GenNewLabelID(), "br", firstGroupShadow.FirstLabelID);
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "1");
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + firstGroup.FirstLabelID + "," + firstGroupShadow.FirstLabelID + ")");
                }
                else
                {
                    // 间接命中
                    resultList.AddItem(method.GenNewLabelID(), "ldc.i4", DCUtils.GetInt32ValueString(flag + 10));
                    resultList.AddItem(method.GenNewLabelID(), "bgt", firstGroupShadow.FirstLabelID);
                    resultList.AddItem(method.GenNewLabelID(), "br", firstGroup.FirstLabelID);
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "1");
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + firstGroupShadow.FirstLabelID + "," + firstGroup.FirstLabelID + ")");
                }

            }
            else
            {
                resultList.AddItem(method.GenNewLabelID(), "br", firstGroup.FirstLabelID);

            }

            int codeCount = 0;
            string lastLabelID = null;
            foreach (var group2 in groups)
            {
                foreach (var item2 in group2)
                {
                    codeCount++;
                    resultList.Add(item2);
                    if (extLocalValue != int.MinValue)
                    {
                        bool addCode = false;
                        if (item2 is DCILOperCode_HandleMethod)
                        {
                            //var methodName = ((DCILOperCode_HandleMethod)item2).InvokeInfo?.MethodName;
                            //if (methodName == "MoveNext")
                            //{
                            //    var nextCode = group2.GetNextCode(item2);
                            //    if(nextCode != null && ( nextCode.OperCode == "brtrue" || nextCode.OperCode == "brtrue.s"))
                            //    {
                            //        if( lastLabelID == null )
                            //        {
                            //            lastLabelID = nextCode.OperData;
                            //        }
                            //        addCode = true;
                            //    }
                            //}
                        }
                        else if (
                            codes_brs_2.Contains(item2.OperCode)
                            || codes_br_2.Contains(item2.OperCode)
                            || item2.OperCodeValue == DCILOpCodeValue.Brtrue
                            || item2.OperCodeValue == DCILOpCodeValue.Brtrue_S
                            || item2.OperCodeValue == DCILOpCodeValue.Brfalse
                            || item2.OperCodeValue == DCILOpCodeValue.Brfalse_S)
                        {
                            addCode = true;
                        }
                        if (addCode)
                        {
                            //if( lastLabelID == null )
                            {
                                lastLabelID = item2.OperData;
                            }
                            resultList.AddItem(method.GenNewLabelID(), "ldloc", DCUtils.GetInt32ValueString(extLocalIndex));
                            if (extLocalValue == 0)
                            {
                                resultList.AddItem(method.GenNewLabelID(), "brtrue", lastLabelID);// firstGroupShadow.FirstLabelID);
                            }
                            else
                            {
                                resultList.AddItem(method.GenNewLabelID(), "brfalse", lastLabelID);// firstGroupShadow.FirstLabelID);
                            }
                            codeCount = 0;
                            lastLabelID = item2.OperData;
                        }
                    }

                }

                if (group2 == firstGroupShadow)
                {
                    //continue;
                }
                if (group2.NextGroup == null)
                {
                    resultList.AddItem(method.GenNewLabelID(), "br", idForLastCode);
                }
                else
                {
                    resultList.AddItem(method.GenNewLabelID(), "br", group2.NextGroup[0].LabelID);
                }
            }

            if (lastCode.HasLabelID() == false)
            {
                resultList.AddItem(idForLastCode, "nop");
            }
            resultList.Add(lastCode);
            items.Clear();
            items.AddRange(resultList);
            items.OnModified();
            items.ChangeShortInstruction();
            int addCount = items.Count - oldItemsCount;
            return true;
        }
    }

    internal class DCILInvokeMethodInfo : IEqualsValue<DCILInvokeMethodInfo> , IDisposable
    {
        public DCILInvokeMethodInfo()
        {

        }
        public int LineIndex = 0;
        public readonly bool SimpleMode = false;
        public string modreq = null;
        public DCILInvokeMethodInfo(DCILMethod method , bool simpleMode = false)
        {
            if(method == null )
            {
                throw new ArgumentNullException("method");
            }
            this.OwnerType = new DCILTypeReference((DCILClass)method.Parent);
            this.LocalMethod = method;
            this.ReturnType = method.ReturnTypeInfo;
            this.Paramters = method.Parameters;
            this.MethodName = method.Name;
            this.SimpleMode = simpleMode;
            this.OwnerType = new DCILTypeReference((DCILClass)method.Parent);
        }

        public DCILInvokeMethodInfo(DCILReader reader, bool simpleMode = false)
        {
            //this.LineIndex = reader.CurrentLineIndex();
            if (simpleMode)
            {
                this.SimpleMode = simpleMode;
                this.ReturnType = DCILTypeReference.GetPrimitiveType("void");
                this.OwnerType = DCILTypeReference.Load(reader);
                if (reader.MatchText("::"))
                {
                    reader.Position += 2;
                    this.MethodName = reader.ReadWord();
                }
                return;
            }
            var strWord = reader.ReadWord();
            if (strWord == "instance")
            {
                this.IsInstance = true;
                strWord = reader.ReadWord();
            }
            else
            {
                this.IsInstance = false;
            }
            if (DCILTypeReference.IsStartWord(strWord))
            {
                this.ReturnType = DCILTypeReference.Load(strWord, reader);
                if (reader.PeekWord() == "modreq")
                {
                    reader.ReadWord();
                    this.modreq = reader.ReadStyleExtValue();
                }

                this.OwnerType = DCILTypeReference.Load(reader);
                if (reader.MatchText("::"))
                {
                    reader.Position += 2;
                    this.MethodName = reader.ReadWord();
                }
                if (reader.PeekContentChar() == '<')
                {
                    this.GenericParamters = new List<DCILTypeReference>();
                    reader.ReadContentChar();
                    while (reader.HasContentLeft())
                    {
                        var chr = reader.PeekContentChar();
                        if (chr == '>')
                        {
                            reader.ReadContentChar();
                            break;
                        }
                        else if (chr == ',')
                        {
                            reader.ReadContentChar();
                            continue;
                        }
                        var pt = DCILTypeReference.Load(reader);
                        if (pt != null)
                        {
                            this.GenericParamters.Add(pt);
                        }
                    }
                }
                if (reader.PeekContentChar() == '(')
                {
                    reader.ReadContentChar();
                    this.Paramters = DCILMethodParamter.ReadParameters(reader, false);
                }

                return;

            }

            reader.ReadLine();
        }
        public void Dispose()
        {
            if( this.GenericParamters != null )
            {
                this.GenericParamters.Clear();
                this.GenericParamters = null;
            }
            this.LocalMethod = null;
            this.MethodName = null;
            this.modreq = null;
            this.OwnerType = null;
            if(this.Paramters != null )
            {
                foreach( var item in this.Paramters )
                {
                    item.Dispose();
                }
                this.Paramters.Clear();
                this.Paramters = null;
            }
            this.ReturnType = null;
            if(this.Styles != null )
            {
                this.Styles.Clear();
                this.Styles = null;
            }
        }
        public DCILInvokeMethodInfo SimpleClone()
        {
            return (DCILInvokeMethodInfo)this.MemberwiseClone();
        }
        public DCILInvokeMethodInfo Clone()
        {
            var result = (DCILInvokeMethodInfo)this.MemberwiseClone();
            if (this.Paramters != null)
            {
                result.Paramters = new List<DCILMethodParamter>();
                foreach (var p in this.Paramters)
                {
                    result.Paramters.Add(p.Clone());
                }
            }
            if (this.GenericParamters != null)
            {
                result.GenericParamters = new List<DCILTypeReference>();
                foreach (var p in this.GenericParamters)
                {
                    result.GenericParamters.Add(p);
                }
            }
            return result;
        }
        public void CacheTypeReference(DCILDocument document)
        {
            if (this.ReturnType != null)
            {
                this.ReturnType = document.CacheTypeReference(this.ReturnType);
            }
            if (this.OwnerType != null)
            {
                this.OwnerType = document.CacheTypeReference(this.OwnerType);
            }
            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
            {
                for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                {
                    this.GenericParamters[iCount] = document.CacheTypeReference(this.GenericParamters[iCount]);
                }
            }
            if (this.Paramters != null && this.Paramters.Count > 0)
            {
                foreach (var item in this.Paramters)
                {
                    item.ValueType = document.CacheTypeReference(item.ValueType);
                }
            }
        }
        public void UpdateLocalInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            UpdateLocalInfo(this.OwnerType?.LocalClass);

        }
        public void UpdateLocalInfo(DCILClass cls)
        {
            if (DCILMethod.IsCtorOrCctor(this.MethodName))
            {
                return;
            }
            if (this.OwnerType.IsArray)
            {
                return;
            }
            if (cls != null)
            {
                //Dictionary<string, DCILTypeReference> gpValues = null;
                DCILGenericParamterList gps = cls.GenericParamters;
                if (this.OwnerType.IsGenericType)
                {
                    //if( gpValues == null )
                    //{
                    //    gpValues = new Dictionary<string, DCILTypeReference>();
                    //}
                    if (this.OwnerType.LocalClass != null)
                    {
                        gps = this.OwnerType.LocalClass.GenericParamters;
                    }
                    else
                    {
                        gps = DCILGenericParamterList.CreateByNativeType(this.OwnerType.SearchNativeType());
                    }
                    if (gps != null)
                    {
                        gps.SetRuntimeType(this.OwnerType.GenericParamters);
                    }
                }
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod && item.Name == this.MethodName)
                    {
                        var method = (DCILMethod)item;
                        if (DCUtils.EqualsListCount(method.Parameters, this.Paramters)
                            && DCUtils.EqualsListCount(method.GenericParamters, this.GenericParamters))
                        {
                            if (method.HasGenericStyle)
                            {
                                var mgps = method.GenericParamters;
                                if (mgps != null && mgps.Count > 0)
                                {
                                    mgps.SetRuntimeType(this.GenericParamters);
                                    gps = DCILGenericParamterList.Merge(gps, mgps);
                                }
                            }
                            if (this.SimpleMode == false && method.ReturnTypeInfo.EqualsValue(this.ReturnType, gps) == false)
                            {
                                continue;
                            }
                            if (this.Paramters != null && this.Paramters.Count > 0)
                            {
                                bool flag = true;
                                for (int iCount = this.Paramters.Count - 1; iCount >= 0; iCount--)
                                {
                                    if (method.Parameters[iCount].ValueType.EqualsValue(this.Paramters[iCount].ValueType, gps) == false)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    this.LocalMethod = method;
                                    break;
                                }
                            }
                            else
                            {
                                this.LocalMethod = method;
                                break;
                            }
                        }
                    }
                }
            }
            if (this.LocalMethod == null && this.OwnerType.HasLibraryName == false)
            {
                // 未找到本地IL中定义的方法。
            }
        }

        public override bool Equals(object obj)
        {
            return EqualsValue(obj as DCILInvokeMethodInfo);
        }
        public bool EqualsValue(DCILInvokeMethodInfo info2)
        {
            if (info2 == null)
            {
                return false;
            }
            if (info2 == this)
            {
                return true;
            }
            if (this.MethodName != info2.MethodName)
            {
                return false;
            }
            int len1 = this.Styles == null ? 0 : this.Styles.Count;
            int len2 = info2.Styles == null ? 0 : info2.Styles.Count;
            if (len1 != len2)
            {
                return false;
            }
            for (int iCount = 0; iCount < len1; iCount++)
            {
                if (this.Styles[iCount] != info2.Styles[iCount])
                {
                    return false;
                }
            }
            if (DCILTypeReference.StaticEquals(this.ReturnType, info2.ReturnType) == false)
            {
                return false;
            }
            if (DCILTypeReference.StaticEquals(this.OwnerType, info2.OwnerType) == false)
            {
                return false;
            }
            if (DCUtils.EqualsList<DCILTypeReference>(this.GenericParamters, info2.GenericParamters) == false)
            {
                return false;
            }
            if (DCILMethodParamter.EqualsList(this.Paramters, info2.Paramters, false, false) == false)
            {
                return false;
            }
            return true;
        }
        private int _HashCode = 0;
        public override int GetHashCode()
        {
            if (this._HashCode == 0)
            {
                if (this.Styles != null && this.Styles.Count > 0)
                {
                    foreach (var item in this.Styles)
                    {
                        this._HashCode += item.GetHashCode();
                    }
                }
                this._HashCode += this.IsInstance.GetHashCode();
                if (this.ReturnType != null)
                {
                    this._HashCode += this.ReturnType.GetHashCode();
                }
                if (this.OwnerType != null)
                {
                    this._HashCode += this.OwnerType.GetHashCode();
                }
                if (this.MethodName != null && this.MethodName.Length > 0)
                {
                    this._HashCode += this.MethodName.GetHashCode();
                }
                if (this.GenericParamters != null)
                {
                    foreach (var item in this.GenericParamters)
                    {
                        this._HashCode += item.GetHashCode();
                    }
                }
                if (this.Paramters != null)
                {
                    foreach (var item in this.Paramters)
                    {
                        this._HashCode += item.ComputeHashCode(true);
                    }
                }
            }
            return this._HashCode;
        }

        public string GetSignString(bool addMethodName)
        {
            return DCILMethod.InnerGetSignString(
                this.ReturnType,
                addMethodName ? this.MethodName : null,
                this.GenericParamters,
                this.OwnerType.LocalClass?.GenericParamters,
                this.Paramters);
        }

        public List<string> Styles = null;
        public bool IsInstance = false;
        public DCILTypeReference ReturnType = DCILTypeReference.Type_Void;
        public DCILTypeReference OwnerType = null;
        public string MethodName = null;
        public DCILMethod LocalMethod = null;
        public List<DCILTypeReference> GenericParamters = null;
        public List<DCILMethodParamter> Paramters = null;
        public int ParametersCount
        {
            get
            {
                if (this.Paramters == null)
                {
                    return 0;
                }
                else
                {
                    return this.Paramters.Count;
                }
            }
        }

        public void WriteTo(DCILWriter writer)
        {
            var strMethodName = this.MethodName;
            if (this.LocalMethod != null)
            {
                strMethodName = this.LocalMethod.Name;
            }
            if (this.SimpleMode)
            {
                this.OwnerType.WriteTo(writer);
                writer.Write("::");
                writer.Write(strMethodName);
                return;
            }
            if (this.IsInstance)
            {
                writer.Write("instance ");
            }
            var lmt = this.LocalMethod?.ReturnTypeInfo;
            if(lmt != null 
                && lmt.Mode != DCILTypeMode.GenericTypeInMethodDefine 
                && lmt.Mode != DCILTypeMode.GenericTypeInTypeDefine
                && lmt.IsGenericType == false )
            {
               
                lmt.WriteTo(writer);
            }
            else
            {
                this.ReturnType.WriteTo(writer);
            }
            if (this.modreq != null && this.modreq.Length > 0)
            {
                writer.Write(" modreq(");
                writer.Write(this.modreq);
                writer.Write(") ");
            }
            writer.Write(' ');
            var cls = this.LocalMethod?.OwnerClass;
            if (cls != null && cls.HasGenericParamters == false)
            {
                writer.Write(cls.NameWithNested);
            }
            else
            {
                this.OwnerType.WriteTo(writer);
            }
            writer.Write("::");
            writer.Write(strMethodName);
            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
            {
                writer.Write('<');
                for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(',');
                    }
                    this.GenericParamters[iCount].WriteTo(writer);
                }
                writer.Write('>');
            }
            writer.Write('(');
            var ps = (this.LocalMethod == null || this.LocalMethod.HasGenericStyle )? this.Paramters : this.LocalMethod.Parameters;
            if (ps != null && ps.Count > 0)
            {
                bool hasAdd = false;
                foreach (var item in ps)
                {
                    if (hasAdd)
                    {
                        writer.Write(',');
                    }
                    hasAdd = true;
                    //if (item.IsClassValueType)
                    //{
                    //    str.Append("class ");
                    //}
                    item.ValueType.WriteTo(writer);
                    //if (item.Name != null && item.Name.Length > 0)
                    //{
                    //    writer.Write(' ');
                    //    writer.Write(item.Name);
                    //}
                }
            }
            writer.Write(')');
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            WriteTo(new DCILWriter(str));
            return str.ToString();
        }
    }
    internal class DCILWriter
    {
        private static readonly string[] _WhitespaceString = null;
        static DCILWriter()
        {
            _WhitespaceString = new string[50];
            _WhitespaceString[0] = string.Empty;
            for (int iCount = 0; iCount < _WhitespaceString.Length; iCount++)
            {
                _WhitespaceString[iCount] = new string(' ', iCount);
            }
        }
        public DCILWriter(TextWriter w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }
            this._BaseWriter = w;
        }
        public DCILWriter(StringBuilder w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }
            this._BaseWriter = new StringWriter(w);
            this._StringBuilder = w;
        }
        public override string ToString()
        {
            if (this._StringBuilder != null)
            {
                return this._StringBuilder.ToString();
            }
            else
            {
                return base.ToString();
            }
        }

        private readonly TextWriter _BaseWriter = null;
        private readonly StringBuilder _StringBuilder = null;

        private static readonly string _hexs = "0123456789ABCDEF";
        private char[] _HexsBuffer = null;

        public void WriteHexs(byte[] bs, int lineHeadWhitespaceNum = 16)
        {
            if (bs != null && bs.Length > 0)
            {
                var len = bs.Length;
                int bufferSize = (int)(bs.Length * 6);
                if (_HexsBuffer == null || bufferSize > _HexsBuffer.Length)
                {
                    _HexsBuffer = new char[bufferSize];
                }
                var position = 0;
                for (int iCount = 0; iCount < len; iCount++)
                {
                    var b = bs[iCount];
                    _HexsBuffer[position++] = _hexs[(b >> 4)];
                    _HexsBuffer[position++] = _hexs[b & 0xf];
                    _HexsBuffer[position++] = ' ';
                    if (iCount > 0 && iCount < len - 1 && (iCount % 16) == 0)
                    {
                        _HexsBuffer[position++] = '\r';
                        _HexsBuffer[position++] = '\n';
                        if (lineHeadWhitespaceNum > 0)
                        {
                            int endIndex = position + lineHeadWhitespaceNum;
                            for (; position < endIndex; position++)
                            {
                                _HexsBuffer[position] = ' ';
                            }
                        }
                    }
                }
                if (this._StringBuilder != null)
                {
                    this._StringBuilder.Append(_HexsBuffer, 0, position);
                }
                else
                {
                    this._BaseWriter.Write(_HexsBuffer, 0, position);
                }
            }
        }

        public void WriteObjects2(System.Collections.IEnumerable objs)
        {
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    if (obj is DCILObject)
                    {
                        ((DCILObject)obj).WriteTo(this);
                    }
                }
            }
        }

        public void WriteObjects(List<DCILObject> objs)
        {
            if (objs != null && objs.Count > 0)
            {
                int len = objs.Count;
                for(int iCount = 0;iCount < len; iCount ++)
                {
                    objs[iCount].WriteTo(this);
                }
            }
        }
        public void Write(string txt)
        {
            EnsureIndent();
            this._IsNewLine = false;
            if (this._StringBuilder != null)
            {
                this._StringBuilder.Append(txt);
            }
            else
            {
                _BaseWriter.Write(txt);
            }
        }
        public void Write(char c)
        {
            EnsureIndent();
            this._IsNewLine = false;
            if (_StringBuilder != null)
            {
                this._StringBuilder.Append(c);
            }
            else
            {
                _BaseWriter.Write(c.ToString());
            }
        }
        public void WriteWhitespace(int num)
        {
            if (this._StringBuilder != null)
            {
                this._StringBuilder.Append(' ', num);
            }
            else
            {
                if (num >= 50)
                {
                    _BaseWriter.Write(new string(' ', num));
                }
                else
                {
                    _BaseWriter.Write(_WhitespaceString[num]);
                }
            }
        }
        private void EnsureIndent()
        {
            if (this._IsNewLine && this._IndentLevel > 0)
            {
                if (this._StringBuilder != null)
                {
                    _StringBuilder.Append(_WhitespaceString[_IndentLevel * 3]);
                }
                else
                {
                    _BaseWriter.Write(_WhitespaceString[_IndentLevel * 3]);
                }
                this._IsNewLine = false;
            }
        }
        public void EnsureNewLine()
        {
            if (this._IsNewLine == false)
            {
                this.WriteLine();
            }
        }

        public void WriteLine(string txt)
        {

            this.EnsureIndent();
            if (this._StringBuilder != null)
            {
                _StringBuilder.AppendLine(txt);
                this._IsNewLine = true;
            }
            else
            {
                _BaseWriter.WriteLine(txt);
                this._IsNewLine = true;
            }
        }
        internal bool _IsNewLine = true;

        public void WriteLine()
        {
            if (this._StringBuilder != null)
            {
                this._StringBuilder.AppendLine();
                this._IsNewLine = true;
            }
            else
            {
                _BaseWriter.WriteLine();
                this._IsNewLine = true;
            }
        }

        public void WriteStartGroup()
        {
            this.EnsureNewLine();
            this.WriteLine("{");
            this._IndentLevel++;
        }
        public void WriteEndGroup()
        {
            this.EnsureNewLine();
            this._IndentLevel--;
            this.WriteLine("}");
        }
        private int _IndentLevel = 0;
        public void ChangeIndentLevel(int step)
        {
            this._IndentLevel += step;
        }
    }

    internal class DCILReader
    {
        public DCILReader(string fileName, System.Text.Encoding encoding, DCILDocument doc)
        {
            this.FileName = fileName;
            using (var reader = new System.IO.StreamReader(fileName, encoding, true))
            {
                this.SetSourceText(reader.ReadToEnd());
                this.Document = doc;
            }
        }
        private DCILReader()
        {

        }
        public DCILReader(string txt, DCILDocument doc)
        {
            if (txt == null || txt.Length == 0)
            {
                throw new ArgumentNullException("txt");
            }
            this.SetSourceText(txt);
            this.Document = doc;
        }

        private Dictionary<DCILField, string> _FieldReferenceDataLabels = null;
        /// <summary>
        /// 添加字段对象引用的数据块信息
        /// </summary>
        /// <param name="field">字段对象</param>
        /// <param name="dataLabel">数据块编号</param>
        public void AddReferenceDataLabel(DCILField field , string dataLabel )
        {
            if(this._FieldReferenceDataLabels == null )
            {
                this._FieldReferenceDataLabels = new Dictionary<DCILField, string>();
            }
            this._FieldReferenceDataLabels[field] = dataLabel;
        }

        /// <summary>
        /// 更新字段对象引用的数据块对象
        /// </summary>
        /// <param name="document"></param>
        public void UpdateFieldReferenceData(DCILDocument document )
        {
            if( this._FieldReferenceDataLabels != null 
                && document != null 
                && document.ILDatas != null 
                && document.ILDatas.Count > 0 )
            {
                var dataMaps = new Dictionary<string, DCILData>();
                foreach( var item in document.ILDatas )
                {
                    dataMaps[item.Name] = item;
                }
                foreach( var item in this._FieldReferenceDataLabels )
                {
                    dataMaps.TryGetValue(item.Value, out item.Key.ReferenceData);
                }
            }
        }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName = null;
        public List<DCILField> FieldsReferenceData = null;
        public void Close()
        {
            this._Text = null;
            if (this._StringTable != null)
            {
                this._StringTable.Clear();
                this._StringTable = null;
            }
            this._Position = 0;
            if( this._FieldReferenceDataLabels != null )
            {
                this._FieldReferenceDataLabels.Clear();
                this._FieldReferenceDataLabels = null;
            }
        }
        public int NumOfOperCode = 0;
        public int NumOfMethod = 0;
        public int NumOfField = 0;
        public int NumOfClass = 0;
        public int NumOfProperty = 0;

        public readonly DCILDocument Document = null;

        private const string _Str_SplitChars = "{}(),=<>&*[]:";

        private static readonly string[] _SplitChars = GetSplitWords();
     
        private static string[] GetSplitWords()
        {
            var result = new string[127];
            foreach (var c in _Str_SplitChars)
            {
                result[c] = new string(c, 1);
            }
            return result;
        }
        private static readonly DCILReader _ReaderForSplit = new DCILReader();

        public char GetChar(int index)
        {
            if (index >= 0 && index < this._Length)
            {
                return this._Text[index];
            }
            else
            {
                return char.MinValue;
            }
        }

        public char PeekContentChar()
        {
            for (int iCount = this._Position; iCount < this._Length; iCount++)
            {
                if (IsWhiteSpace(this._Text[iCount]) == false)
                {
                    return this._Text[iCount];
                }
            }
            return char.MinValue;
        }

        public char ReadContentChar()
        {
            var list = new List<char>();
            for (; this._Position < this._Length; this._Position++)
            {
                if (IsWhiteSpace(this._Text[this._Position]) == false)
                {
                    return this._Text[this._Position++];
                }
            }
            return char.MinValue;
        }

        private int _LastLineIndex = 0;
        private int _LastLineIndex_Position = 0;
        public unsafe int CurrentLineIndex()
        {
            if (this._Position < this._LastLineIndex_Position)
            {
                this._LastLineIndex = 0;
                this._LastLineIndex_Position = 0;
            }
            int result = this._LastLineIndex ;
            fixed ( char * pFirst = this._Text)
            {
                char* pStart = pFirst + this._LastLineIndex_Position ;
                char* pEnd = pFirst + this._Position;
                while( pStart < pEnd )
                {
                    if( *pStart == '\r')
                    {
                        result++;
                    }
                    pStart++;
                }
                this._LastLineIndex = result;
                this._LastLineIndex_Position = (int)(pStart - pFirst);
            }
            return result;
        }

        public string PeekWord()
        {
            int pos = this._Position;
            var result = this.ReadWord();
            this._Position = pos;
            return result;
        }

        private const int CharType_None = 0;
        private const int CharType_Spliter = 1;
        private const int CharType_Whitespace = 2;

        private static readonly byte[] _CharTypes = InitCharTypes();
        private static byte[] InitCharTypes()
        {
            var types = new byte[127];
            for (int iCount = 0; iCount < types.Length; iCount++)
            {
                if (_Str_SplitChars.IndexOf((char)iCount) >= 0)
                {
                    types[iCount] = CharType_Spliter;
                }
                else if (iCount == ' ' || iCount == '\r' || iCount == '\n' || iCount == '\t')
                {
                    types[iCount] = CharType_Whitespace;
                }
                else
                {
                    types[iCount] = CharType_None;
                }
            }
            return types;
        }
       
        private static readonly bool[] _IsOperCodeChars = BuildOperCodeChars();
        private static bool[] BuildOperCodeChars()
        {
            var result = new bool[127];
            for(int c = 0; c < result.Length; c ++ )
            {
                result[c] = c >= 'a' && c <= 'z' || c >= '0' && c <= '9' || c == '.';
            }
            return result;
        }

        private System.Collections.Generic.Dictionary<long, DCILOperCodeDefine> _OperCodeTable = null;

        public DCILOperCodeDefine ReadOperCode()
        {
            var thisText = this._Text;
            var thisLength = this._Length;
            for (; this._Position < thisLength; this._Position++)
            {
                var c = thisText[this._Position];
                if (c < 127 && _IsOperCodeChars[c])
                {
                    for (int startPos = this._Position; this._Position < thisLength; this._Position++)
                    {
                        c = thisText[this._Position];
                        if (c >= 127 || _IsOperCodeChars[c] == false)
                        {
                            long key = ((long)(this._Position - startPos) << 32) + startPos;
                            DCILOperCodeDefine result = null;
                            if (this._OperCodeTable.TryGetValue(key, out result) == false)
                            {
                                string strCode = thisText.Substring(startPos, this._Position - startPos);
                                result = DCILOperCodeDefine.GetDefine(strCode);
                                this._OperCodeTable[key] = result;
                            }
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        public string ReadWord()
        {
            var currentItem_EndPosition = -1;
            var currentItem_Length = 0;
            int textLength = this._Length;
            var bodyText = this._Text;
            Retry:;
            for (; this._Position < textLength; this._Position++)
            {
                char c = bodyText[this._Position];
                if ( c !=' ' && c !='\t' && c !='\r' && c !='\n')// IsWhiteSpace(this._Text[this._Position]) == false)
                {
                    bool isInGroup = false;
                    for (; this._Position < textLength; this._Position++)
                    {
                        c = bodyText[this._Position];
                        if (c == '\'')
                        {
                            // 遇到单引号
                            isInGroup = !isInGroup;
                        }
                        if (isInGroup)
                        {
                            // 在单引号组内，无条件的添加
                            currentItem_EndPosition = this._Position;
                            currentItem_Length++;
                            //this._CurrentItem[this._CurrentItem_Length++] = c;
                        }
                        else
                        {
                            int charType = CharType_None;
                            if (c < 127)
                            {
                                charType = _CharTypes[c];
                            }
                            switch( charType )
                            {
                                case CharType_None:
                                    {
                                        // 遇到常规字符
                                        if (c == '/' && this._Position < textLength - 1 && bodyText[this._Position + 1] == '/')
                                        {
                                            // 遇到注释
                                            this.MoveNextLine();
                                            if (currentItem_Length > 0)
                                            {
                                                return GetCurrentItemString(currentItem_Length, currentItem_EndPosition);
                                            }
                                            else
                                            {
                                                goto Retry;
                                            }
                                        }
                                        currentItem_EndPosition = this._Position;
                                        currentItem_Length++;
                                    }
                                    break;
                                case CharType_Spliter:
                                    // 遇到分隔字符
                                    if (currentItem_Length == 0)
                                    {
                                        this._Position++;
                                        return _SplitChars[c];
                                    }
                                    else
                                    {
                                        return GetCurrentItemString(currentItem_Length, currentItem_EndPosition);
                                    }
                                    break;
                                case CharType_Whitespace:
                                    // 遇到空白字符
                                    if (currentItem_Length > 0)
                                    {
                                        return GetCurrentItemString(currentItem_Length, currentItem_EndPosition);
                                    }
                                    break;
                            }
                        }
                    }
                    if (currentItem_Length > 0)
                    {
                        return GetCurrentItemString(currentItem_Length, currentItem_EndPosition);
                    }
                }
            }
            return null;
        }

        public int ReadArrayIndex()
        {
            if (this.SkipWhitespace())
            {
                if (this._Text[this._Position] == '[')
                {
                    int index = this._Text.IndexOf(']', this._Position + 1);
                    if (index > 0)
                    {
                        int result = 0;
                        if (int.TryParse(
                            this._Text.Substring(this._Position + 1, index - this._Position - 1),
                            out result))
                        {
                            this._Position = index;
                            return result;
                        }
                    }
                }
            }
            return int.MinValue;
        }

        /// <summary>
        /// 跳过所有的空格
        /// </summary>
        /// <returns>是否还有内容可供读取</returns>
        public bool SkipWhitespace()
        {
            for (; this._Position < this._Length; this._Position++)
            {
                if (DCUtils.IsWhitespace(this._Text[this._Position]) == false)
                {
                    break;
                }
            }
            return this._Position < this._Length;
        }
        /// <summary>
        /// 跳过当前行中所有的空格
        /// </summary>
        /// <returns>是否还有内容可供读取</returns>
        public bool SkipLineWhitespace()
        {
            for (; this._Position < this._Length; this._Position++)
            {
                var c = this._Text[this._Position];
                if (c != ' ' && c != '\t')
                {
                    break;
                }
            }
            return this._Position < this._Length;
        }
        /// <summary>
        /// 当前行是否还具有非空白的内容
        /// </summary>
        /// <returns></returns>
        public bool HasContentLeftCurrentLine()
        {
            for (int iCount = this._Position; iCount < this._Length; iCount++)
            {
                var c = this._Text[iCount];
                if (c == '\r' || c == '\n')
                {
                    return false;
                }
                if (c != ' ' && c != '\t')
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasContentLeft()
        {
            return this._Position < this._Length;
        }
        public string ReadAfterChar(char c)
        {
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index = this._Text.IndexOf(c, this._Position);
            if (index >= 0)
            {
                var result = this._Text.Substring(this._Position, index - this._Position + 1);
                this._Position = index + 1;
                return result;
            }
            else
            {
                var result = this._Text.Substring(this._Position);
                this._Position = this._Length;
                return result;
            }
        }
        public void MoveToLineEnd()
        {
            for (; this._Position < this._Length; this._Position++)
            {
                var c = this._Text[this._Position];
                if (c == '\r' || c == '\n')
                {
                    break;
                }
            }
        }
        public string ReadInstructionContent()
        {
            var strData = new System.Text.StringBuilder();
            //strData.Append(this.ReadLine());
            int level = 0;
            bool isNewLineFlag = false;
            for (; this._Position < this._Length; this._Position++)
            {
                var c = this._Text[this._Position];
                if (c == '/'
                    && this._Position < this._Length - 1
                    && this._Text[this._Position + 1] == '/')
                {
                    // 遇到注释
                    this.MoveToLineEnd();
                    // 撤销注释之前的空白
                    for (int iCount = strData.Length - 1; iCount >= 0; iCount--)
                    {
                        var c2 = strData[iCount];
                        if (c2 != ' ' && c2 != '\t')
                        {
                            if (iCount < strData.Length - 2)
                            {
                                strData.Remove(iCount + 1, strData.Length - iCount - 1);
                            }
                            break;
                        }
                    }
                    continue;
                }
                else if (c == '\r' || c == '\n')
                {
                    isNewLineFlag = true;
                }
                else if (c == '.' && isNewLineFlag)
                {
                    // 新的指令
                    for (int iCount = strData.Length - 1; iCount >= 0; iCount--)
                    {
                        var c2 = strData[iCount];
                        if (c2 == '\r' || c2 == '\n')
                        {
                            strData.Remove(iCount, strData.Length - iCount);
                            break;
                        }
                    }
                    break;
                }
                else if (c == '{')
                {
                    level++;
                }
                else if (c == '}')
                {
                    level--;
                    if (level <= 0)
                    {
                        // 出组
                        if (level == 0)
                        {
                            strData.Append(c);
                            this._Position++;
                        }
                        else
                        {
                            //this._Position--;
                        }
                        break;
                    }
                }
                if (isNewLineFlag && IsWhiteSpace(c) == false)
                {
                    isNewLineFlag = false;
                }
                strData.Append(c);
            }
            if (strData.Length > 0 && strData[strData.Length - 1] == '\r')
            {
                strData.Remove(strData.Length - 1, 1);
            }
            return strData.ToString();
        }
        public bool MatchText(string text)
        {
            if (text != null && text.Length > 0 && this._Position < this._Length - text.Length)
            {
                if (string.Compare(this._Text, this._Position, text, 0, text.Length, StringComparison.Ordinal) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        
        private System.Collections.Generic.Dictionary<long, string> _StringTable = null;
        public string GetSubStringUseTable(int pos, int len, bool trim = false)
        {
            var thisText = this._Text;
            if (trim)
            {
                int endIndex = pos + len - 1;
                while (pos <= endIndex)
                {
                    char c = thisText[pos];
                    if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                    {
                        break;
                    }
                    pos++;
                }
                while (endIndex >= pos)
                {
                    char c = thisText[endIndex];
                    if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                    {
                        break;
                    }
                    endIndex--;
                }
                len = endIndex - pos + 1;
            }
            if (len == 0)
            {
                return string.Empty;
            }
            if (len == 1)
            {
                return DCUtils.GetSingleCharString(thisText[pos]);
            }
            string result = null;
            long key = ((long)len << 32) + pos;
            if (this._StringTable.TryGetValue(key, out result) == false)
            {
                result = DCUtils.GetStringUseTable(thisText.Substring(pos, len));
                this._StringTable[key] = result;
            }
            //if(result.IndexOf("\r") > 0 )
            //{

            //}
            return result;
        }

        private class KeyStringComparere : System.Collections.Generic.IEqualityComparer<long>
        {
            public KeyStringComparere(string txt)
            {
                this._Text = txt;
            }
            private string _Text = null;

            private int _EqualsCount = 0;
            private int _GetHashCodeCount = 0;

            public bool Equals(long x, long y)
            {
                //this._EqualsCount++;
                int len1 = (int)(x >> 32);
                int pos1 = (int)x & 0xfffffff;
                int len2 = (int)(y >> 32);
                int pos2 = (int)y & 0xfffffff;
                if (len1 != len2)
                {
                    return false;
                }
                return string.CompareOrdinal(this._Text, pos1, this._Text, pos2, len1) == 0;
            }

            public unsafe int GetHashCode(long obj)
            {
                //this._GetHashCodeCount++;
                int len = (int)(obj >> 32);
                int pos = (int)obj & 0xfffffff;
                fixed (char* pStart = this._Text)
                {
                    char* pend = pStart + pos + len;
                    char* p = pend - len;
                    int num = 5381;
                    while (p < pend)
                    {
                        num = ((num << 5) + num) ^ *p;
                        p++;
                    }
                    return num;

                    //int num2 = num;
                    //if ((((int)p) & 2) == 0)
                    //{
                    //    while (p < pend)
                    //    {
                    //        if ((((int)p) & 2) == 0)
                    //        {
                    //            num = ((num << 5) + num) ^ *p;
                    //        }
                    //        else
                    //        {
                    //            num2 = ((num2 << 5) + num2) ^ *p;
                    //        }
                    //        p++;
                    //    }
                    //}
                    //else
                    //{
                    //    while (p < pend)
                    //    {
                    //        if ((((int)p) & 2) != 0)
                    //        {
                    //            num = ((num << 5) + num) ^ *p;
                    //        }
                    //        else
                    //        {
                    //            num2 = ((num2 << 5) + num2) ^ *p;
                    //        }
                    //        p++;
                    //    }
                    //}
                    //int result = num + num2 * 1566083941 + len;
                    //return result;


                    //int num = 5381;
                    //int num2 = num;
                    //char* ptr2 = ptr;
                    //int num3;
                    //while ((num3 = *ptr2) != 0)
                    //{
                    //    num = ((num << 5) + num) ^ num3;
                    //    num3 = ptr2[1];
                    //    if (num3 == 0)
                    //    {
                    //        break;
                    //    }
                    //    num2 = ((num2 << 5) + num2) ^ num3;
                    //    ptr2 += 2;
                    //}
                    //return num + num2 * 1566083941;
                }
            }
        }

        private string GetCurrentItemString(int _CurrentItem_Length , int _CurrentItem_EndPosition )
        {
            if (_CurrentItem_Length == 0)
            {
                return string.Empty;
            }
            else
            {
                int pos = _CurrentItem_EndPosition - _CurrentItem_Length + 1;
                string result = GetSubStringUseTable(pos, _CurrentItem_Length );
                return result;
            }
        }

        

        public string ReadAfterCharsExcludeLastChar(string chrs, out char resultChar)
        {
            if (chrs == null)
            {
                throw new ArgumentNullException("chrs");
            }
            resultChar = char.MinValue;
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index = this._Position;
            for (; this._Position < this._Length; this._Position++)
            {
                int index2 = chrs.IndexOf(this._Text[this._Position]);
                if (index2 >= 0)
                {
                    resultChar = chrs[index2];
                    var result = this._Text.Substring(index, this._Position - index);
                    this._Position++;
                    return result;
                }
            }
            return this._Text.Substring(index);
        } 
        public string ReadStyleExtValue()
        {
            var c = this.ReadContentChar();
            if (c == '(')
            {
                if (this._Position == this._Length)
                {
                    return string.Empty;
                }
                int level = 0;
                for (int iCount = this._Position; iCount < this._Length; iCount++)
                {
                    var c2 = this._Text[iCount];
                    if (c2 == '(')
                    {
                        level++;
                    }
                    else if (c2 == ')')
                    {
                        level--;
                        if (level < 0)
                        {
                            string result = GetSubStringUseTable(this._Position, iCount - this._Position);// this._Text.Substring(this._Position, iCount - this._Position);
                            this._Position = iCount + 1;
                            return result;
                        }
                    }
                }
                var result2 = GetSubStringUseTable(this._Position, this._Length - this._Position);// this._Text.Substring(this._Position, this._Length - this._Position);
                this._Position = this._Length;
                return result2;
            }
            return null;
        }

        //public string Trim( string txt )
        //{
        //    if (txt != null && txt.Length > 0)
        //    {

        //    }
        //    else
        //    {
        //        return txt;
        //    }
        //}

        public void MoveAfterChar( char c )
        {
            if(this._Position < this._Length )
            {
                int index = this._Text.IndexOf(c, this._Position);
                if(index > 0)
                {
                    this._Position = index + 1;
                }
                else
                {
                    this._Position = this._Length;
                }
            }
        }

        public string ReadAfterCharExcludeLastChar(char c , bool trim = false )
        {
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index2 = this._Text.IndexOf(c, this._Position);
            if (index2 > 0)
            {
                var result = GetSubStringUseTable( this._Position, index2 - this._Position , trim );
                this._Position = index2 + 1;
                return result;
            }
            else
            {
                var result = this._Text.Substring(this._Position);
                this._Position = this._Length;
                return result;
            }
        }

        private static readonly byte[] EmptyBytes = new byte[0];
        private byte[] _byteBuffer = new byte[1024];

        public byte[] ReadBinaryFromHex()
        {
            //var list = new List<byte>();
            int curValue = -1;
            int bufferSize = _byteBuffer.Length - 10;
            int bufferPosition = 0;
            var textLength = this._Text.Length;
            var thisText = this._Text;
            for (; this._Position < textLength; this._Position++)
            {

                char c = thisText[this._Position];
                if (c == '/' && this._Position < textLength - 1 && thisText[this._Position + 1] == '/')
                {
                    // 遇到注释,跳过当前行剩余内容
                    this.MoveNextLine();
                }
                else if (c == ')')
                {
                    // 遇到圆括号，退出
                    this.MoveNextLine();
                    break;
                }
                else
                {
                    var index = DCUtils.IndexOfHexChar(c);// DCILDocument.IndexOfHexChar(c);
                    if (index >= 0)
                    {
                        if (curValue >= 0)
                        {
                            if (bufferSize < bufferPosition)
                            {
                                var temp = new byte[(int)(bufferPosition * 1.5) + 10];
                                Array.Copy(_byteBuffer, temp, _byteBuffer.Length);
                                _byteBuffer = temp;
                                bufferSize = _byteBuffer.Length - 10;
                            }
                            _byteBuffer[bufferPosition++] = (byte)((curValue << 4) + index);
                            //list.Add((byte)((curValue << 4) + index));
                            curValue = -1;
                        }
                        else
                        {
                            curValue = index;
                        }
                    }
                }
            }
            if (curValue >= 0)
            {
                _byteBuffer[bufferPosition++] = (byte)curValue;
                //list.Add((byte)curValue);
            }
            if (bufferPosition > 0)
            {
                var result = new byte[bufferPosition];
                Array.Copy(this._byteBuffer, result, bufferPosition);
                return result;
            }
            return null;
            // return list.ToArray();
        }
        public int Read()
        {
            if (this._Position < this._Length)
            {
                return this._Text[this._Position++];
            }
            else
            {
                return -1;
            }
        }
        public char Peek()
        {
            if (this._Position < this._Length)
            {
                return this._Text[this._Position];
            }
            else
            {
                return char.MinValue;
            }
        }
        public string ReadLineTrim()
        {
            return this.ReadLine()?.Trim();

        }
        public string ReadLineTrimRemoveComment()
        {
            var line = this.ReadLine();
            if (line != null && line.Length > 0)
            {
                int index = line.IndexOf("//");
                if (index > 0)
                {
                    int index2 = line.IndexOf('"');
                    if (index2 < 0 || index2 > index)
                    {
                        return line.Substring(0, index).Trim();
                    }
                }
            }
            return line.Trim();
        }

        public static string ToRawILText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length == 0)
            {
                return "\"\"";
            }
            bool isBinary = false;
            foreach (var c in text)
            {
                if (c > 128)
                {
                    isBinary = true;
                    break;
                }
            }
            if (isBinary)
            {
                var bs = Encoding.Unicode.GetBytes(text);
                var w = new DCILWriter(new StringBuilder());
                w.Write("bytearray(");
                w.WriteHexs(bs);
                w.Write(")");
                return w.ToString();
            }
            else
            {
                var result = new System.Text.StringBuilder();
                result.Append('"');
                foreach (var c in text)
                {
                    switch (c)
                    {
                        case '\r': result.Append(@"\r"); break;
                        case '\n': result.Append(@"\n"); break;
                        case '\'': result.Append(@"\'"); break;
                        case '\\': result.Append(@"\\"); break;
                        case '"': result.Append("\\\""); break;
                        case '\b': result.Append(@"\b"); break;
                        case '\f': result.Append(@"\f"); break;
                        case '\t': result.Append(@"\t"); break;
                        default: result.Append(c); break;
                    }
                }
                result.Append('"');
                return result.ToString();
            }
        }
        //public static int _SimpleString = 0;
        //private static int _AllStrings = 0;

        public class StringValueInfo
        {
            public string Value = null;
            public bool IsBinary = false;
            public string ILRawText = null;
            public byte[] BianryData = null;
        }

        public StringValueInfo ReadStringValue()
        {
            //_AllStrings++;
            StringValueInfo info = new StringValueInfo();
            this.SkipWhitespace();
            int startPosition = this._Position;
            var thisText = this._Text;
            var thisLength = this._Length;
            if (thisText[this._Position] == '"')
            {
                // 明文格式的字符串
                this._Position++;
                for (; this._Position < thisLength; this._Position++)
                {
                    var c = thisText[this._Position];
                    if (c == '\\')
                    {
                        // 遇到转义字符
                        break;
                    }
                    else if (c == '"')
                    {
                        // 结束字符串定义
                        int posBack = this._Position;
                        this.MoveNextLine();
                        var nextChar = this.PeekContentChar();
                        if (nextChar != '+')
                        {
                            // 不是多行文本，是单行文本。在大多数情况下都是无转义的单行文本
                            //_SimpleString++;
                            info.ILRawText = this.GetSubStringUseTable(startPosition, posBack - startPosition + 1);
                            if ( info.ILRawText.Length == 2)
                            {
                                info.Value = string.Empty;
                            }
                            else
                            {
                                info.Value = this.GetSubStringUseTable(startPosition + 1, posBack - startPosition + 1 - 2);// rawILText.Substring(1, rawILText.Length - 2);
                            }
                            return info;
                        }
                        break;
                    }
                }
                // 恢复原始位置
                this._Position = startPosition;
                var strFinallyValue = new StringBuilder();
                bool isInSting = false;
                int endIndex = this._Position;
                int thisLength_1 = thisLength - 1;
                for (; this._Position < thisLength; this._Position++)
                {
                    var c = thisText[this._Position];
                    if (c == '"')
                    {
                        // 切换字符串定义模式
                        isInSting = !isInSting;
                        if (isInSting == false)
                        {
                            endIndex = this._Position + 1;
                            // 如果不是定义字符串，则跳到下一行。
                            this.MoveNextLine();
                            //this.SkipWhitespace();
                        }
                    }
                    else if (isInSting)
                    {
                        // 正在定义一个字符串
                        if (c == '\\' && this._Position < thisLength_1)
                        {
                            // 遇到转义字符
                            this._Position++;
                            var nc = thisText[this._Position];
                            switch (nc)
                            {
                                case 'r': strFinallyValue.Append('\r'); break;
                                case 'n': strFinallyValue.Append('\n'); break;
                                case '\'': strFinallyValue.Append('\''); break;
                                case '"': strFinallyValue.Append('"'); break;
                                case '\\': strFinallyValue.Append('\\'); break;
                                case 'b': strFinallyValue.Append('\b'); break;
                                case 'f': strFinallyValue.Append('\f'); break;
                                case 't': strFinallyValue.Append('\t'); break;
                                default: strFinallyValue.Append(nc); break;
                            }
                        }
                        else
                        {
                            strFinallyValue.Append(c);
                        }
                    }
                    else if (c == '+')
                    {
                        // 定义多行字符串
                    }
                    else if (IsWhiteSpace(c) == false)
                    {
                        // 不是定义多行字符串
                        this._Position--;
                        break;
                    }
                }//for
                info.ILRawText = thisText.Substring(startPosition, endIndex - startPosition);
                info.Value = strFinallyValue.ToString();
                return info;
            }
            else if (string.Compare(thisText, this._Position, _bytearray, 0, _bytearray.Length, StringComparison.Ordinal) == 0)
            {
                // 二进制格式定义的字符串
                this._Position += _bytearray.Length;
                this.MoveAfterChar('(');
                var bs = this.ReadBinaryFromHex();
                //info.ILRawText = thisText.Substring(startPosition, this._Position - startPosition);
                info.BianryData = bs;
                info.IsBinary = true;
                if (bs != null && bs.Length > 0)
                {
                    string result = Encoding.Unicode.GetString(bs);
                    if (result.Length < 10)
                    {
                        result = DCUtils.GetStringUseTable(result);
                    }
                    info.Value = result;
                    return info;
                }
            }
            throw new NotSupportedException("no string value at line " + this.CurrentLineIndex());
        }

        internal static readonly string _bytearray = "bytearray";

        public string ReadLine()
        {
            int i;
            for (i = this._Position; i < this._Length; i++)
            {
                char c = this._Text[i];
                if (c == '\r' || c == '\n')
                {
                    string result = this._Text.Substring(this._Position, i - this._Position);
                    this._Position = i + 1;
                    if (c == '\r' && this._Position < this._Length && this._Text[this._Position] == '\n')
                    {
                        this._Position++;
                    }
                    return result;
                }
            }
            if (i > this._Position)
            {
                string result2 = this._Text.Substring(this._Position, i - this._Position);
                this._Position = i;
                return result2;
            }
            return null;
        }

        public void MoveNextLine()
        {
            int i;
            for (i = this._Position; i < this._Length; i++)
            {
                char c = this._Text[i];
                if (c == '\r' || c == '\n')
                {
                    this._Position = i + 1;
                    if (c == '\r' && this._Position < this._Length && this._Text[this._Position] == '\n')
                    {
                        this._Position++;
                    }
                    return;
                }
            }
            if (i > this._Position)
            {
                this._Position = i;
            }
        }

        public static int ParseArrayIndex(string text)
        {
            if (text != null && text.Length > 0)
            {
                int len = text.Length;
                for (int iCount = 0; iCount < len; iCount++)
                {
                    if (text[iCount] == '[')
                    {
                        int result = 0;
                        for (int iCount2 = iCount + 1; iCount2 < len; iCount2++)
                        {
                            var c = text[iCount2];
                            if (c >= '0' && c <= '9')
                            {
                                result = result * 10 + c - '0';
                            }
                            else// if( c == ']')
                            {
                                break;
                            }
                        }
                        return result;
                    }
                }
            }
            return int.MinValue;
        }

        public static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
        private string _Text = null;

        private void SetSourceText(string txt)
        {
            if (txt == null)
            {
                throw new ArgumentNullException("txt");
            }
            this._Text = txt;
            this._StringTable = new Dictionary<long, string>(new KeyStringComparere(this._Text));
            this._OperCodeTable = new Dictionary<long, DCILOperCodeDefine>(new KeyStringComparere(this._Text));
            this._Length = txt.Length;
        }

        private int _Position = 0;
        public int Position
        {
            get
            {
                return this._Position;
            }
            set
            {
                this._Position = value;
            }
        }
        private int _Length = 0;
        public int Length
        {
            get
            {
                return this._Length;
            }
        }
    }
     
    internal class DCILDocument : DCILObject
    {

        /// <summary>
        /// 字符串 .resources
        /// </summary>
        public static readonly string EXT_resources = ".resources";

        public DCILDocument()
        {

        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Document;
            }
        }
        /// <summary>
        /// .NET Core版本号
        /// </summary>
        public string NetCoreVersion = null;
        /// <summary>
        /// 文档注释XML文档
        /// </summary>
        public System.Xml.XmlDocument CommentXmlDoc = null;
        /// <summary>
        /// 程序集文件配套的deps.json文件的内容
        /// </summary>
        public byte[] Content_DepsJson = null;
        
        public void LoadByReader(string fileName, System.Text.Encoding encoding)
        {
            this.LibraryNames = new Dictionary<string, string>();
            var reader = new DCILReader(fileName, encoding, this);
            this.RootPath = Path.GetDirectoryName(fileName);
            this.FileName = fileName;
            this._Name = Path.GetFileNameWithoutExtension(fileName);
            this.Load(reader);
            reader.Close();
        }

        public Dictionary<string, int> GetNodeIndexs<T>() where T : DCILObject
        {
            var result = new Dictionary<string, int>();
            var len = this.ChildNodes.Count;
            for (int iCount = 0; iCount < len; iCount++)
            {
                if (this.ChildNodes[iCount] is T)
                {
                    result[this.ChildNodes[iCount].Name] = iCount;
                }
            }
            return result;
        }
         
        private Dictionary<DCILTypeReference, DCILTypeReference> _CachedTypes = null;

        public Dictionary<string, string> LibraryNames = new Dictionary<string, string>();
        public string GetLibraryName(string typeName)
        {
            if (typeName == null || typeName.Length == 0)
            {
                throw new ArgumentNullException("typeName");
            }
            string result = null;
            if (this.LibraryNames.TryGetValue(typeName, out result))
            {
                return result;
            }
            return null;
        }
       
        private static Dictionary<string, string> _Standard_Type_LibName = null;
        private static Dictionary<string, string> GetStandardTypeLibName()
        {
            if (_Standard_Type_LibName == null)
            {
                _Standard_Type_LibName = new Dictionary<string, string>();
#if DOTNETCORE
                _Standard_Type_LibName[typeof(string).FullName] = "System.Runtime";
                _Standard_Type_LibName[typeof(System.Diagnostics.DebuggerBrowsableAttribute).FullName] = "System.Diagnostics.Debug";
                _Standard_Type_LibName[typeof(System.Threading.Thread).FullName] = "System.Threading.Thread";
                _Standard_Type_LibName[typeof(System.IO.MemoryStream).FullName] = "System.Runtime.Extensions";
                _Standard_Type_LibName[typeof(System.Drawing.Bitmap).FullName] = "System.Drawing.Common";
                var asms = System.Runtime.Loader.AssemblyLoadContext.Default.Assemblies;
                foreach (var asm in asms)
                {
                    var asmName = asm.GetName().Name;
                    if ( DCUtils.IsSystemAsseblyName( asmName ))
                    {
                        var ts = asm.GetForwardedTypes();
                        if (ts != null && ts.Length > 0)
                        {
                            foreach (var t in ts)
                            {
                                var fn2 = DCUtils.GetFullName(t);
                                if (_Standard_Type_LibName.ContainsKey(fn2) == false)
                                {
                                    _Standard_Type_LibName[fn2] = asmName;
                                }
                            }
                        }
                    }
                }
#else
                var asms = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach(var asm in asms )
                {
                    var asmName = asm.GetName().Name;
                    if( DCUtils.IsSystemAsseblyName( asmName ))
                    {
                        var ts = asm.GetExportedTypes();
                        if( ts != null &&ts.Length > 0 )
                        {
                            foreach(var t in ts)
                            {
                                var fn2 = DCUtils.GetFullName(t);
                                if( _Standard_Type_LibName.ContainsKey( fn2 )== false )
                                {
                                    _Standard_Type_LibName[fn2] = asmName;
                                }
                            }
                        }
                    }
                }
#endif
            }
            return _Standard_Type_LibName;
        }

        private Dictionary<string, string> _Cache_TypeNameWithLibraryName = new Dictionary<string, string>();
        public string GetTypeNameWithLibraryName(string typeName, string defaultLibName = null, Type nativeType = null)
        {
            if (typeName == null || typeName.Length == 0)
            {
                throw new ArgumentNullException("typeName");
            }
            var dic = GetStandardTypeLibName();
            string result = null;
            if (this._Cache_TypeNameWithLibraryName.TryGetValue(typeName, out result))
            {
                return result;
            }
            else if (this.LibraryNames.TryGetValue(typeName, out result))
            {
                result = "[" + result + "]" + typeName;
            }
            else if (dic.TryGetValue(typeName, out result))
            {
                result = "[" + result + "]" + typeName;
            }
            else if (defaultLibName != null && defaultLibName.Length > 0)
            {
                result = "[" + defaultLibName + "]" + typeName;
            }
            else
            {
                var t = typeof(string).Assembly.GetType(typeName, false, true);
                if (t == null)
                {

#if DOTNETCORE
                    t = typeof(string).Assembly.GetType(typeName, false, true);
                    if( t == null )
                    {
                        t = typeof(System.Math).Assembly.GetType(typeName, false, true);
                    }
                    if (t == null)
                    {
                        foreach (var asm in System.Runtime.Loader.AssemblyLoadContext.Default.Assemblies)
                        {
                            t = asm.GetType(typeName, false, true);
                            if (t != null)
                            {
                                break;
                            }
                        }
                    }
#else

#endif
                }
                if (t == null)
                {
                    t = nativeType;
                }
                if (t == null)
                {
                    result = "[" + this.LibName_mscorlib + "]" + typeName;
                }
                else
                {
                    result = "[" + t.Module.Name + "]" + t.Namespace + "." + t.Name;
                }
            }
            _Cache_TypeNameWithLibraryName[typeName] = result;
            return result;
        }

        public void FixDomState()
        {
            this._CachedTypes = new Dictionary<DCILTypeReference, DCILTypeReference>();

            this.LibName_mscorlib = null;
            foreach (var item in this.Assemblies)
            {
                if (this.LibName_mscorlib == null && item.IsExtern)
                {
                    this.LibName_mscorlib = item.Name;
                    break;
                }
            }

            if (this.LibName_mscorlib == null)
            {
                this.LibName_mscorlib = typeof(string).Assembly.GetName().Name;
            }

            this.UpdateLocalInfo();

            this.LibraryNames = new Dictionary<string, string>();
            foreach (var item in this._CachedTypes)
            {
                var lib = item.Value.LibraryName;
                var tn = item.Value.Name;
                if (lib != null && lib.Length > 0 && tn != null && tn.Length > 0)
                {
                    this.LibraryNames[tn] = lib;
                }
            }
        }
         
        public void UpdateLocalInfo()
        {
            this._CachedInvokeMethods = new Dictionary<DCILInvokeMethodInfo, DCILInvokeMethodInfo>();
            var clses = this.GetAllClassesUseCache(); //new Dictionary<string, DCILClass>();
            //GetAllClasses(null, this.Classes, clses);
            foreach (var cls in clses.Values)
            {
                cls.CacheInfo(this, clses);
            }
            foreach (var asm in this.Assemblies)
            {
                asm.CusotmAttributesCacheTypeReference(this);
            }
            this.CusotmAttributesCacheTypeReference(this);
            foreach (var item in this._CachedTypes.Values)
            {
                if (item.HasLibraryName == false)
                {
                    if (clses.TryGetValue(item.Name, out item.LocalClass))
                    {

                    }
                    else
                    {

                    }
                }
            }
            foreach (var item in this._CachedInvokeMethods.Values)
            {
                item.UpdateLocalInfo(this, clses);
            }
        }

        private void GetAllClasses(string baseName, List<DCILClass> list, Dictionary<string, DCILClass> clses)
        {
            foreach (var cls in list)
            {
                var name = cls.Name;
                if (baseName != null)
                {
                    name = baseName + cls.Name;
                }
                clses[name] = cls;
                if (cls.NestedClasses != null && cls.NestedClasses.Count > 0)
                {
                    GetAllClasses(name + "/", cls.NestedClasses, clses);
                }
            }
        }

        private Dictionary<DCILFieldReference, DCILFieldReference> _Cache_FieldReference 
            = new Dictionary<DCILFieldReference, DCILFieldReference>();
        internal DCILFieldReference CacheFieldReference(DCILFieldReference field)
        {
            if (field == null)
            {
                return null;
            }
            DCILFieldReference result = null;
            if (_Cache_FieldReference.TryGetValue(field, out result))
            {
                return result;
            }
            this._Cache_FieldReference[field] = field;
            return field;
        }

        internal DCILTypeReference CacheTypeReference(DCILTypeReference tr)
        {
            if (tr == null)
            {
                return null;
            }
            if (tr.IsPrimitive
                || tr.Mode == DCILTypeMode.GenericTypeInMethodDefine
                || tr.Mode == DCILTypeMode.GenericTypeInTypeDefine)
            {
                return tr;
            }
            DCILTypeReference result = null;
            if (this._CachedTypes.TryGetValue(tr, out result))
            {
                return result;
            }
            else
            {
                this._CachedTypes[tr] = tr;
                if (tr.IsGenericType)
                {
                    for (int iCount = 0; iCount < tr.GenericParamters.Count; iCount++)
                    {
                        tr.GenericParamters[iCount] = CacheTypeReference(tr.GenericParamters[iCount]);
                    }
                }
                return tr;
            }
        }

        internal void EnumAllOperCodes(EnumOperCodeHandler handler)
        {
            for (int iCount = this.Classes.Count - 1; iCount >= 0; iCount--)
            {
                ClassEnumOperCode(this.Classes[iCount], handler);
            }
        }

        private void ClassEnumOperCode( DCILClass cls , EnumOperCodeHandler handler )
        {
            foreach( var item in cls.ChildNodes)
            {
                if(item is DCILMethod )
                {
                    var method = (DCILMethod)item;
                    method.OperCodes.EnumDeeply(method, handler);
                }
            }
            if( cls.NestedClasses != null &&  cls.NestedClasses.Count > 0 )
            {
                foreach( var cls2 in cls.NestedClasses)
                {
                    ClassEnumOperCode(cls2, handler);
                }
            }
        }
        internal void DisplayMethodRefCount()
        {
            var dic = new SortedDictionary<string, int>();
            this.EnumAllOperCodes(delegate ( EnumOperCodeArgs args )
            {
                var code = args.Current;
                if (code.OperCodeValue == DCILOpCodeValue.Call
                    || code.OperCodeValue == DCILOpCodeValue.Callvirt)
                {
                    var cm = code as DCILOperCode_HandleMethod;
                    if (cm != null)
                    {
                        string name = code.OperCode + " " + cm.InvokeInfo.ToString();
                        if (dic.ContainsKey(name))
                        {
                            dic[name]++;
                        }
                        else
                        {
                            dic[name] = 1;
                        }
                    }
                }
            });
                    
            var list = new List<Tuple<string, int>>();
            foreach (var item in dic)
            {
                if (item.Value > 10)
                {
                    list.Add(new Tuple<string, int>(item.Key, item.Value));
                }
            }
            list.Sort(delegate (Tuple<string, int> x, Tuple<string, int> y) {
                return y.Item2 - x.Item2;
            });
            foreach( var item in list )
            {
                System.Diagnostics.Debug.WriteLine(item.Item2 + " = " + item.Item1);
            }
        }

        internal Dictionary<DCILInvokeMethodInfo, DCILInvokeMethodInfo> _CachedInvokeMethods = null;

        internal DCILInvokeMethodInfo CacheDCILInvokeMethodInfo(DCILInvokeMethodInfo info)
        {
            if (info == null)
            {
                return null;
            }
            DCILInvokeMethodInfo result = null;
            if (_CachedInvokeMethods.TryGetValue(info, out result))
            {
                return result;
            }
            _CachedInvokeMethods[info] = info;
            info.CacheTypeReference(this);
            return info;
        }
        public void UpdateCustomAttributeValues()
        {
            var cusAttributes = new List<DCILCustomAttribute>();
            this.AddCustomAttributes(cusAttributes);
            var clses = this.GetAllClassesUseCache();
            foreach (var cls in clses.Values)
            {
                cls.AddCustomAttributes(cusAttributes);
                foreach (var node in cls.ChildNodes)
                {
                    node.AddCustomAttributes(cusAttributes);
                }
            }
            foreach (var asm in this.Assemblies)
            {
                asm.AddCustomAttributes(cusAttributes);
            }
            foreach (var item in cusAttributes)
            {
                item.ParseValues(new ReadCustomAttributeValueArgs(null, this, Path.GetDirectoryName(this.AssemblyFileName)));
            }
        }
        /// <summary>
        /// 合并IL文档，并将第一个文档作为主文档进行输出
        /// </summary>
        /// <param name="documents">要合并的文档对象列表</param>
        /// <returns>合并后的文档</returns>
        public static DCILDocument MergeDocuments(List<DCILDocument> documents)
        {
            if (documents == null || documents.Count == 0)
            {
                return null;
            }
            if (documents.Count == 1)
            {
                return documents[0];
            }
            int tick = Environment.TickCount;
            var globalAllClasses = new Dictionary<string, Dictionary<string, DCILClass>>();
            foreach (var doc in documents)
            {
                var libName = doc.Name;
                var cusAttributes = doc.GetAllCustomAttributesUseCache();
                foreach (var item in cusAttributes)
                {
                    item.ParseValues(new ReadCustomAttributeValueArgs(
                        documents,
                        doc,
                        Path.GetDirectoryName(doc.AssemblyFileName)));
                }
                var clses = doc.GetAllClassesUseCache();
                DCILClass cls2 = null;
                if (clses.TryGetValue("'<PrivateImplementationDetails>'", out cls2))
                {
                    cls2._Name = "___InnerTempData" + doc.GetHashCode();
                }
                globalAllClasses[doc.Name] = clses;
                foreach (var item in doc._CachedTypes.Values)
                {
                    if (item.LocalClass != null)
                    {
                        if (item.LocalClass == cls2)
                        {
                            item.Name = cls2.Name;
                        }
                        else if (item.LocalClass.Parent == cls2)
                        {
                            item.Name = item.LocalClass.NameWithNested;
                        }
                    }
                }
            }//foreach

            var mainDoc = documents[0];
            foreach (var doc in documents)
            {
                foreach (var item in doc._CachedTypes.Values)
                {
                    if (item.LibraryName != null && item.LibraryName.Length > 0)
                    {
                        Dictionary<string, DCILClass> clses = null;
                        if (globalAllClasses.TryGetValue(item.LibraryName, out clses))
                        {
                            DCILClass cls = null;
                            if (clses.TryGetValue(item.Name, out cls))
                            {
                                item.LocalClass = cls;
                                item.LibraryName = null;
                            }
                        }
                    }
                }
            }//foreach
            int clsesCount = 0;
            bool hasAddILDataItem = false;
            int nameMaxLength = 0;
            foreach( var doc in documents )
            {
                var n = Path.GetFileName(doc.AssemblyFileName);
                nameMaxLength = Math.Max(nameMaxLength, System.Text.Encoding.UTF8.GetByteCount(n));
            }
            nameMaxLength += 2;
            for (int iCount = 0 ; iCount < documents.Count; iCount++)
            {
                var document = documents[iCount];
                string msg = "";
                if( document.FileSize > 0 )
                {
                    msg = msg + DCUtils.FormatByteSize(document.FileSize);
                }
                if(document.CommentXmlDoc != null )
                {
                    msg = msg + " with '" + document.Name + ".XML'";
                }
                if(msg.Length > 0 )
                {
                    msg = "(" + msg +")";
                    int len2 = Encoding.UTF8.GetByteCount(Path.GetFileName(document.AssemblyFileName));
                    msg = new string(' ', nameMaxLength - len2) + msg ;
                }
                msg = "       + " + Path.GetFileName(document.AssemblyFileName) + msg ;
                MyConsole.Instance.WriteLine( msg );
                if( iCount == 0 )
                {
                    continue;
                }
                if (document.Resouces != null)
                {
                    foreach (var item in document.Resouces)
                    {
                        if (mainDoc.Resouces.ContainsKey(item.Key))
                        {
                            ConsoleReportError("       [Warring],Duplicate resource name : " + item.Key);
                            return null;
                        }
                        else
                        {
                            mainDoc.Resouces[item.Key] = item.Value;
                            item.Value.Modified = true;
                        }
                    }
                }
                foreach (var asm in mainDoc.Assemblies)
                {
                    if (string.Compare(asm.Name, document.Name, true) == 0)
                    {
                        mainDoc.Assemblies.Remove(asm);
                        break;
                    }
                }
                var itemNames = new HashSet<string>();
                foreach (var asm in mainDoc.Assemblies)
                {
                    itemNames.Add(asm.Name);
                }
                foreach (var asm in document.Assemblies)
                {
                    if (asm.IsExtern && itemNames.Contains(asm.Name) == false)
                    {
                        mainDoc.Assemblies.Add(asm);
                    }
                }
                itemNames.Clear();
                foreach (var item in mainDoc.Modules)
                {
                    if (item.IsExtern)
                    {
                        itemNames.Add(item.Name);
                    }
                }
                foreach (var item in document.Modules)
                {
                    if (item.IsExtern == true
                        && itemNames.Contains(item.Name) == false)
                    {
                        mainDoc.Modules.Insert(0, item);
                    }
                }
                // 合并类型
                itemNames.Clear();
                foreach (var cls in mainDoc.Classes)
                {
                    itemNames.Add(cls.Name);
                }
                foreach (var cls in document.Classes)
                {
                    if (itemNames.Contains(cls.Name))
                    {
                        ConsoleReportError("       [Warring],Duplicate class name : " + cls.Name);
                        return null;
                    }
                    mainDoc.Classes.Add(cls);
                }
                if( document.CommentXmlDoc != null )
                {
                    // 合并文档注释XML节点
                    var srcRoot = document.CommentXmlDoc.SelectSingleNode("doc/members");
                    if( srcRoot != null )
                    {
                        var descRoot = mainDoc.CommentXmlDoc?.SelectSingleNode("doc/members");
                        if(descRoot == null )
                        {
                            mainDoc.CommentXmlDoc = new System.Xml.XmlDocument();
                            mainDoc.CommentXmlDoc.LoadXml("<doc><assembly><name>" + mainDoc.Name + "</name></assembly><members/></doc>");
                            descRoot = mainDoc.CommentXmlDoc?.SelectSingleNode("doc/members");
                        }
                        var doc4 = mainDoc.CommentXmlDoc;
                        foreach( System.Xml.XmlNode node in srcRoot.ChildNodes )
                        {
                            descRoot.AppendChild(doc4.ImportNode( node , true ));
                        }
                    }
                }
                clsesCount += document.Classes.Count;
                if (mainDoc._CachedTypes != null && mainDoc._CachedTypes.Count > 0)
                {
                    if (mainDoc._Cache_FieldReference != null)
                    {
                        foreach (var item in mainDoc._Cache_FieldReference.Values)
                        {
                            if (item.OwnerType.LocalClass != null && item.LocalField == null)
                            {
                                item.UpdateLocalField(item.OwnerType.LocalClass);
                            }
                        }
                    }
                    if (mainDoc._CachedInvokeMethods != null)
                    {
                        foreach (var item in mainDoc._CachedInvokeMethods.Values)
                        {
                            if (item.OwnerType.LocalClass != null && item.LocalMethod == null)
                            {
                                item.UpdateLocalInfo(item.OwnerType.LocalClass);
                            }
                        }
                    }
                }
                // 清除一些无效的缓存数据
                mainDoc._AllClasses = null;
                mainDoc._AllCustomAttributes = null;
                // 合并缓存的信息
                mainDoc._HasMergeDocuments = true;
                mainDoc._CachedInvokeMethods = MergeDictionary<DCILInvokeMethodInfo, DCILInvokeMethodInfo>(
                    mainDoc._CachedInvokeMethods, document._CachedInvokeMethods);
                mainDoc._CachedTypes = MergeDictionary<DCILTypeReference, DCILTypeReference>(
                    mainDoc._CachedTypes, document._CachedTypes);
                mainDoc._Cache_FieldReference = MergeDictionary<DCILFieldReference, DCILFieldReference>(
                    mainDoc._Cache_FieldReference, document._Cache_FieldReference);
                mainDoc._Cache_TypeNameWithLibraryName = MergeDictionary<string, string>(
                    mainDoc._Cache_TypeNameWithLibraryName, document._Cache_TypeNameWithLibraryName);
                mainDoc.LibraryNames = MergeDictionary<string, string>(mainDoc.LibraryNames, document.LibraryNames);
                if (document.ILDatas.Count > 0)
                {
                    mainDoc.ILDatas.AddRange(document.ILDatas);
                    hasAddILDataItem = true;
                    
                }
                document.CleanFieldValue();
            }//for
            if(hasAddILDataItem )
            {
                int ilDataIDCounter = Math.Abs(Environment.TickCount)/2;
                foreach (var item in mainDoc.ILDatas)
                {
                    item._Name = "I_" + Convert.ToString(ilDataIDCounter++);
                }
            }
            for (int iCount = mainDoc.Assemblies.Count - 1; iCount >= 0; iCount--)
            {
                if (mainDoc.Assemblies[iCount].IsExtern)
                {
                    var asmName = mainDoc.Assemblies[iCount].Name;
                    foreach (var doc in documents)
                    {
                        if (string.Compare(asmName, doc.Name, true) == 0)
                        {
                            mainDoc.Assemblies.RemoveAt(iCount);
                            break;
                        }
                    }
                }
            }
            MyConsole.Instance.WriteLine("       Merge " + Convert.ToString(documents.Count - 1)
                + " assembly files , add " + clsesCount + " classes, span "
                + Math.Abs(Environment.TickCount - tick) + " milliseconds.");
            return mainDoc;
        }

        public bool _HasMergeDocuments = false;
         
        private static Dictionary<K, V> MergeDictionary<K, V>(Dictionary<K, V> dic1, Dictionary<K, V> dic2)
        {
            if (dic2 == null || dic2.Count == 0)
            {
                return dic1;
            }
            if (dic1 == null)
            {
                return dic2;
            }
            Dictionary<K, V> result = dic1;
            foreach (var item2 in dic2)
            {
                dic1[item2.Key] = item2.Value;
            }
            return result;
        }

        private static void ConsoleReportError(string msg)
        {
            MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
            MyConsole.Instance.BackgroundColor = ConsoleColor.White;
            MyConsole.Instance.WriteLine(msg);
            MyConsole.Instance.ResetColor();
        }
        public List<DCILModule> Modules = null;
        public List<DCILAssembly> Assemblies = null;

        private List<DCILCustomAttribute> _AllCustomAttributes = null;
        public List<DCILCustomAttribute> GetAllCustomAttributesUseCache()
        {
            if (this._AllCustomAttributes == null)
            {
                this._AllCustomAttributes = new List<DCILCustomAttribute>();
                this.AddCustomAttributes(_AllCustomAttributes);
                foreach (var asm in this.Assemblies)
                {
                    if (asm.IsExtern == false)
                    {
                        asm.AddCustomAttributes(_AllCustomAttributes);
                    }
                }
                var clses = GetAllClassesUseCache();
                foreach (var cls in clses.Values)
                {
                    cls.AddCustomAttributes(_AllCustomAttributes);
                    foreach (var node in cls.ChildNodes)
                    {
                        node.AddCustomAttributes(_AllCustomAttributes);
                    }
                }
            }
            return this._AllCustomAttributes;
        }
        /// <summary>
        /// 根据名称获得类型对象
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="nestedSplitChar">套嵌类型名称分隔字符</param>
        /// <returns>获得的类型对象</returns>
        public DCILClass GetClassByName(string typeName, char nestedSplitChar = '/')
        {
            if (typeName == null || typeName.Length == 0)
            {
                return null;
            }
            if ( nestedSplitChar == '/' || typeName.IndexOf(nestedSplitChar) < 0)
            {
                DCILClass result = null;
                if (this.GetAllClassesUseCache().TryGetValue(typeName, out result))
                {
                    return result;
                }
            }
            else
            {
                var tns = typeName.Split(nestedSplitChar);
                DCILClass rootClass = null;
                if (this.GetAllClassesUseCache().TryGetValue(tns[0], out rootClass))
                {
                    DCILClass curCls = rootClass;
                    for (int iCount = 1; iCount < tns.Length; iCount++)
                    {
                        if (curCls.NestedClasses == null)
                        {
                            break;
                        }
                        foreach (var cls2 in curCls.NestedClasses)
                        {
                            if (cls2.Name == tns[iCount])
                            {
                                if (iCount == tns.Length - 1)
                                {
                                    return cls2;
                                }
                                curCls = cls2;
                                break;
                            }
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获得所有包含运行指令的方法对象
        /// </summary>
        /// <returns></returns>
        public List<DCILMethod> GetAllMethodHasOperCodes()
        {
            var list = new List<DCILMethod>();
            foreach( var cls in this.Classes )
            {
                InnerGetAllMethodHasOperCodes(cls, list);
            }
            return list;
        }
        private void InnerGetAllMethodHasOperCodes(DCILClass cls, List<DCILMethod> list)
        {
            if (cls.IsImport == false && cls.IsInterface == false)
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        var m = (DCILMethod)item;
                        if (m.OperCodes != null && m.OperCodes.Count > 0)
                        {
                            list.Add(m);
                        }
                    }
                }
            }
        }
        private Dictionary<string, DCILClass> _AllClasses = null;
        public Dictionary<string, DCILClass> GetAllClassesUseCache()
        {
            if (this._AllClasses == null)
            {
                this._AllClasses = new Dictionary<string, DCILClass>();
                this.GetAllClasses(null, this.Classes, _AllClasses);
            }
            return this._AllClasses;
        }
        public void ClearCacheForAllClasses()
        {
            this._AllClasses = null;
        }
        public List<DCILClass> Classes = null;
        public List<DCILUnknowObject> UnknowObjects = null;
        public string RuntimeVersion = null;
        
        public override void Load(DCILReader reader)
        {
            this.Modules = new List<DCILModule>();
            this.Assemblies = new List<DCILAssembly>();
            this.UnknowObjects = new List<DCILUnknowObject>();
            this.Classes = new List<DCILClass>();
            if (this.ChildNodes == null)
            {
                this.ChildNodes = new DCILObjectList();
            }
            var classMap = new Dictionary<string, DCILClass>();
            for( int iCount  = 0;iCount <100; iCount ++)
            {
                var line = reader.ReadLine();
                if(line == null )
                {
                    break;
                }
                if(line.StartsWith("//") && line.IndexOf("Metadata version") > 0 )
                {
                    int index = line.IndexOf(':');
                    if(index > 0 )
                    {
                        this.RuntimeVersion = line.Substring(index + 1).Trim();
                    }
                    break;
                }
            }
            reader.Position = 0;
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == null)
                {
                    break;
                }
                switch (strWord)
                {
                    case DCILModule.TagName_Module:
                        {
                            var module = new DCILModule(reader);
                            if (module.IsExtern == false)
                            {
                                if (module.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                                    || module.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    this._Name = module.Name.Substring(0, module.Name.Length - 4);
                                }
                            }
                            this.Modules.Add(module);
                        }
                        break;
                    case DCILMResource.TagName_mresource:
                        {
                            var item = new DCILMResource(this, reader);
                            this.Resouces[item.Name] = item;
                            //this.ChildNodes.Add(item);
                        }
                        break;
                    case DCILAssembly.TagName:
                        var asm = new DCILAssembly();
                        asm.LoadHeader(reader);
                        bool match = false;
                        foreach (var item in this.Assemblies)
                        {
                            if (item.Name == asm.Name)
                            {
                                item.LoadContent(reader);
                                match = true;
                            }
                        }
                        if (match == false)
                        {
                            asm.LoadContent(reader);
                            this.Assemblies.Add(asm);
                        }
                        break;
                    case DCILCustomAttribute.TagName_custom:
                        {
                            base.ReadCustomAttribute(reader);
                        }
                        break;
                    case DCILData.TagName_Data:
                        {
                            this.ILDatas.Add(new DCILData(reader));
                        }
                        break;
                    case DCILClass.TagName:
                        {
                            var cls = new DCILClass();
                            cls.LoadHeader(reader);
                            if (cls.Name != null && cls.Name.Length > 0)
                            {
                                DCILClass oldCls = null;
                                if (classMap.TryGetValue(cls.Name, out oldCls))
                                {
                                    oldCls.LoadContent(reader);
                                }
                                else
                                {
                                    cls.LoadContent(reader);
                                    this.Classes.Add(cls);
                                    classMap[cls.Name] = cls;
                                    reader.NumOfClass++;
                                }
                            }
                        }
                        break;
                    //case "corflags":
                    //    {
                    //        string str = reader.ReadInstructionContent();
                    //        int v2 = 0;
                    //        if(Int32.TryParse( str , out v2 ))
                    //        {
                    //            this.CorFlags = (DCILRuntimeFlags) v2;
                    //        }
                    //    }
                    //    break;
                    default:
                        {
                            var obj = new DCILUnknowObject(strWord, reader);
                            SetCorFlags(obj.Name, obj.Data);
                            this.UnknowObjects.Add(obj);
                        }
                        break;
                }
            }
            DCILMResource.LoadData(this.Resouces.Values, Path.GetDirectoryName(reader.FileName));
            var resFileName = Path.ChangeExtension(reader.FileName, ".res");
            if(File.Exists( resFileName ))
            {
                this.Win32ResData = File.ReadAllBytes(resFileName);
            }
            reader.UpdateFieldReferenceData(this);
            //if (reader.FieldsReferenceData != null
            //    && reader.FieldsReferenceData.Count > 0
            //    && this.ILDatas != null && this.ILDatas.Count > 0)
            //{
            //    var datas = new Dictionary<string, DCILData>();
            //    foreach (var item in this.ILDatas)
            //    {
            //        datas[item.Name] = item;
            //    }
            //    foreach (var field in reader.FieldsReferenceData)
            //    {
            //        datas.TryGetValue(field.DataLabel, out field.ReferenceData);
            //    }
            //}
            FixDomState();
        }
        /// <summary>
        /// 必须运行在32位操作系统上。
        /// </summary>
        public bool IsRequired32Bit = false;

        public int Value_CorFlags = -1;
        private void SetCorFlags(string name, string v)
        {
            if (name == ".corflags")
            {
                try
                {
                    this.Value_CorFlags = DCUtils.ConvertToInt32(v);
                    this.IsRequired32Bit = (this.Value_CorFlags & 2) == 2;
                }
                catch (System.Exception ext)
                {

                }
            }
        }
        public List<DCILData> ILDatas = new List<DCILData>();
        public void WriteResourceFile(string basePath)
        {
            if (this.Resouces != null)
            {
                foreach (var item in this.Resouces)
                {
                    if (item.Value.Modified)
                    {
                        item.Value.WriteDataFile(basePath);
                    }
                }
            }
        }

        public Dictionary<string, string> CustomInstructions = null;

        public override void WriteTo(DCILWriter writer)
        {
            writer.WriteObjects2(this.Modules);
            writer.WriteObjects2(this.Assemblies);
            writer.WriteObjects2(this.Resouces.Values);
            if (this.CustomInstructions != null && this.CustomInstructions.Count > 0)
            {
                foreach (var item in this.CustomInstructions)
                {
                    writer.WriteLine(item.Key + " " + item.Value);
                    SetCorFlags(item.Key, item.Value);
                }
                foreach (var item in this.UnknowObjects)
                {
                    if (this.CustomInstructions.ContainsKey(item.Name) == false)
                    {
                        item.WriteTo(writer);
                        SetCorFlags(item.Name, item.Data);
                    }
                }
            }
            else
            {
                foreach (var item in this.UnknowObjects )
                {
                    SetCorFlags(item.Name, item.Data);
                    item.WriteTo(writer);
                }
                //writer.WriteObjects2(this.UnknowObjects);
            }
            writer.WriteObjects2(this.CustomAttributes);
            foreach( var cls in this.Classes)
            {
                cls.InnerWriteTo(writer, true);
            }
            writer.WriteObjects2(this.Classes);
            writer.WriteObjects2(this.ILDatas);
        }

        public void WriteToFile(string fileName, Encoding encod)
        {
            using (var writer = new System.IO.StreamWriter(fileName, false, encod))
            {
                var w2 = new DCILWriter(writer);
                this.WriteTo(w2);
            }
            this.WriteResourceFile(Path.GetDirectoryName(fileName));
        }

        /// <summary>
        /// 获得所有支持的语言信息对象
        /// </summary>
        /// <param name="rootDir">根目录</param>
        /// <returns>语言信息对象</returns>
        public System.Globalization.CultureInfo[] GetSupportCultures()
        {
            var names = new List<string>();
            foreach (var res in this.Resouces.Values)
            {
                if (res.LocalDatas != null)
                {
                    foreach (var locName in res.LocalDatas.Keys)
                    {
                        if (names.Contains(locName) == false)
                        {
                            names.Add(locName);
                        }
                    }
                }
            }
            if (names.Count > 0)
            {
                names.Sort();
                var list = new List<System.Globalization.CultureInfo>();
                foreach (var name in names)
                {
                    try
                    {
                        var cul = System.Globalization.CultureInfo.GetCultureInfo(name);
                        if (cul != null)
                        {
                            list.Add(cul);
                        }
                    }
                    catch (System.Exception ext)
                    {

                    }
                }
                return list.ToArray();
            }
            return null;
        }


        public override string ToString()
        {
            return "ILFile " + this.FileName;
        }
        public int FileSize = 0;
        public string FileName = null;

        //public string[] SourceLines = null;
        public string RootPath = null;
        public string AssemblyFileName = null;
        public string LibName_mscorlib = "mscorlib";
        public int ModifiedCount = 0;
        public List<string> ReferenceAssemblies = new List<string>();
        public byte[] Win32ResData = null;
        public SortedDictionary<string, DCILMResource> Resouces = new SortedDictionary<string, DCILMResource>();

        public void CleanFieldValue()
        {
            this.Resouces = null;
            this.Classes = null;
            this.ILDatas = null;
            this.Assemblies = null;
            this.Modules = null;
        }
        public override void Dispose()
        {
            base.Dispose();
            if (this.Classes != null)
            {
                foreach (var cls in this.Classes)
                {
                    cls.Dispose();
                }
                this.Classes.Clear();
                this.Classes = null;
            }
            if(this._AllClasses != null )
            {
                this._AllClasses.Clear();
                this._AllClasses = null;
            }
            if( this._AllCustomAttributes != null )
            {
                this._AllCustomAttributes.Clear();
                this._AllCustomAttributes = null;
            }
            if( this._CachedInvokeMethods != null )
            {
                foreach( var item in this._CachedInvokeMethods)
                {
                    item.Value.Dispose();
                }
                this._CachedInvokeMethods.Clear();
                this._CachedInvokeMethods = null;
            }
            if( this._CachedTypes != null )
            {
                foreach( var item in this._CachedTypes)
                {
                    item.Value.Dispose();
                }
                this._CachedTypes.Clear();
                this._CachedTypes = null;
            }
            if( this._Cache_FieldReference != null )
            {
                foreach( var item in this._Cache_FieldReference)
                {
                    item.Value.Dispose();
                }
                this._Cache_FieldReference.Clear();
                this._Cache_FieldReference = null;
            }
            if( this._Cache_TypeNameWithLibraryName != null )
            {
                this._Cache_TypeNameWithLibraryName.Clear();
                this._Cache_TypeNameWithLibraryName = null;
            }
            this.FileName = null;
            this.RootPath = null;
            if(this.Assemblies != null )
            {
                foreach( var item in this.Assemblies )
                {
                    item.Dispose();
                }
                this.Assemblies.Clear();
                this.Assemblies = null;
            }
            if(this.Resouces != null )
            {
                foreach( var item in this.Resouces )
                {
                    item.Value.Dispose();
                }
                this.Resouces.Clear();
                this.Resouces = null;
            }
            this.RuntimeVersion = null;
            this.Win32ResData = null;
            if(this.ReferenceAssemblies != null )
            {
                this.ReferenceAssemblies.Clear();
                this.ReferenceAssemblies = null;
            }
            this.Content_DepsJson = null;
            this.CommentXmlDoc = null;
            this.CustomInstructions = null;
            this.LibName_mscorlib = null;
            if(this.LibraryNames != null )
            {
                this.LibraryNames.Clear();
                this.LibraryNames = null;
            }
            if( this.Modules != null )
            {
                foreach( var item in this.Modules )
                {
                    item.Dispose();
                }
                this.Modules.Clear();
                this.Modules = null;
            }
            if( this.UnknowObjects != null )
            {
                foreach( var item in this.UnknowObjects )
                {
                    item.Dispose();
                }
                this.UnknowObjects.Clear();
                this.UnknowObjects = null;
            }
            if (this.ILDatas != null)
            {
                foreach( var item in this.ILDatas )
                {
                    item.Dispose();
                }
                this.ILDatas.Clear();
                this.ILDatas = null;
            }
            
        }

        public bool _IsDotNetCoreAssembly = false;

    }
    internal class DCILOperCodeList : List<DCILOperCode>,IDisposable
    {
        static DCILOperCodeList()
        {
           
        }
        public string GetDebugTextForStackOffset()
        {
            var str = new DCILWriter(new StringBuilder());
            int stackCount = 0;
            foreach( var item in this )
            {
                int so = item.StackOffset;
                str.Write(stackCount.ToString("00") + "  " + so + "\t | ");
                str._IsNewLine = true;
                item.WriteTo(str);
                str.WriteLine();
                stackCount += so;
            }
            return str.ToString();
        }
        //public int GroupIndex = 0;

        public void Dispose()
        {
            foreach( var item in this )
            {
                item.Dispose();
            }
            this.Clear();
        }
        public bool ItemBitSizeChanged = false;
        public DCILOperCodeDefine SafeGetNativeDefine( int index )
        {
            if (index >= 0 && index < this.Count )
            {
                return this[index].NativeDefine;
            }
            else
            {
                return null;
            }
        }
        public DCILOperCode SafeGet( int index )
        {
            if (index >= 0 && index < this.Count)
            {
                return this[index];
            }
            else
            {
                return null;
            }
        }
        public DCILOperCode GetNextCode( DCILOperCode item )
        {
            if(item != null )
            {
                int index = this.IndexOf(item);
                if(index >= 0 && index < this.Count -1 )
                {
                    return this[index + 1];
                }
            }
            return null;
        }
        public string FirstLabelID
        {
            get
            {
                if( this.Count > 0 )
                {
                    return this[0].LabelID;
                }
                else
                {
                    return null;
                }
            }
        }
        public DCILOperCode LastCode
        {
            get
            {
                if(this.Count > 0 )
                {
                    return this[this.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public DCILOperCodeList NextGroup = null;

        private bool ChangeShortInstruction_Flag = false;
        public void OnModified()
        {
            //if (this.ChangeShortInstruction_Flag)
            //{
            //    this.ChangeShortInstruction_Flag = false;
            //}
        }
        private static Dictionary<string, string> _ShortJumpOperCodeMaps = null;

        /// <summary>
        /// 将短指令转换为长指令
        /// </summary>
        /// <param name="codes"></param>
        public void ChangeShortInstruction()
        {
            if( this.ChangeShortInstruction_Flag == true || this.Count == 0 )
            {
                return;
            }
            if( _ShortJumpOperCodeMaps == null )
            {
                _ShortJumpOperCodeMaps = new Dictionary<string, string>();
                _ShortJumpOperCodeMaps["beq.s"] = "beq";
                _ShortJumpOperCodeMaps["bge.s"] = "bge";
                _ShortJumpOperCodeMaps["bge.un.s"] = "bge.un";
                _ShortJumpOperCodeMaps["bgt.s"] = "bgt";
                _ShortJumpOperCodeMaps["bgt.un.s"] = "bgt.un";
                _ShortJumpOperCodeMaps["ble.s"] = "ble";
                _ShortJumpOperCodeMaps["ble.un.s"] = "ble.un";
                _ShortJumpOperCodeMaps["blt.s"] = "blt";
                _ShortJumpOperCodeMaps["blt.un.s"] = "blt.un";
                _ShortJumpOperCodeMaps["bne.un.s"] = "bne.un";
                _ShortJumpOperCodeMaps["br.s"] = "br";
                _ShortJumpOperCodeMaps["brfalse.s"] = "brfalse";
                _ShortJumpOperCodeMaps["brtrue.s"] = "brtrue";
                _ShortJumpOperCodeMaps["leave.s"] = "leave";
            }
            this.ChangeShortInstruction_Flag = true;
            foreach (var item in this)
            {
                var code = item.OperCode;
                if (code != null
                    && code.Length > 3
                    && code[code.Length - 2] == '.'
                    && code[code.Length - 1] == 's')
                {
                    string newCode = null;
                    if( _ShortJumpOperCodeMaps.TryGetValue( code , out newCode))
                    {
                        item.SetOperCode( newCode);
                    }
                    //if(code == "brtrue.s")
                    //{

                    //}
                    //if(DCILOperCode.GetOperCodeType( code  ) == ILOperCodeType.JumpShort)
                    //{
                    //    item.OperCode = code.Substring(0, code.Length - 2);
                    //}
                }
                else if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var tg = (DCILOperCode_Try_Catch_Finally)item;
                    tg._Try?.OperCodes?.ChangeShortInstruction();
                    if (tg._Catchs != null)
                    {
                        foreach (var citem in tg._Catchs)
                        {
                            citem.OperCodes?.ChangeShortInstruction();
                        }
                    }
                    tg._Finally?.OperCodes?.ChangeShortInstruction();
                    tg._fault?.OperCodes?.ChangeShortInstruction();
                }
            }
        }

        //private void ChangeShortInstruction_Leave()
        //{
        //    if(this.Count > 0 && this.ChangeShortInstruction_Flag == false )
        //    {
        //        foreach( var item in this )
        //        {
        //            if(item.OperCode == "leave.s")
        //            {
        //                item.OperCode = "leave";
        //            }
        //        }
        //    }
        //}

        public DCILOperCode AddItem(string labelID, string operCode, string operData = null)
        {
            var item = new DCILOperCode(labelID, operCode, operData);
            this.Add(item);
            return item;
        }

        public DCILOperCode AddItem(string labelID, DCILOperCodeDefine myDefine , string operData = null)
        {
            //if(myDefine == DCILOperCodeDefine._br &&( operData == null || operData.Length == 0 ))
            //{

            //}
            var item = new DCILOperCode(labelID, myDefine, operData);
            this.Add(item);
            return item;
        }

        public int StartLineIndex = 0;

        public void WriteTo(DCILWriter writer)
        {
            foreach (var item in this)
            {
                item.WriteTo(writer);
            }
        }

        public void EnumDeeply(DCILMethod method, EnumOperCodeHandler handler)
        {
            var args = new EnumOperCodeArgs();
            args.OwnerList = this;
            args.Method = method;
            for (int iCount = 0; iCount < this.Count; iCount++)
            {
                args.Current = this[iCount];
                args.CurrentCodeIndex = iCount;
                handler(args);
                if (args.Current is DCILOperCode_Try_Catch_Finally)
                {
                    var group = (DCILOperCode_Try_Catch_Finally)args.Current;
                    group._Try?.OperCodes?.EnumDeeply(method, handler);
                    if (group.HasCatchs())
                    {
                        foreach (var item2 in group._Catchs)
                        {
                            item2.OperCodes?.EnumDeeply(method, handler);
                        }
                    }
                    group._Finally?.OperCodes?.EnumDeeply(method, handler);
                    group._fault?.OperCodes?.EnumDeeply(method, handler);
                }
            }
        }
    }

    internal class EnumOperCodeArgs
    {
        public DCILMethod Method = null;
        public DCILOperCodeList OwnerList = null;
        public int CurrentCodeIndex = 0;
        public DCILOperCode Current = null;
        public DCILOperCode GetPreCode()
        {
            return this.OwnerList.SafeGet(this.CurrentCodeIndex - 1);
        }
    }

    internal delegate void EnumOperCodeHandler(EnumOperCodeArgs args );

    /// <summary>
    /// 处理类型的指令
    /// </summary>
    internal class DCILOperCode_HandleClass : DCILOperCode
    {
        public DCILOperCode_HandleClass(string lableID, DCILOperCodeDefine vdef , DCILReader reader)
        {
            this.LabelID = lableID;
            this._Define = vdef;
            this.ClassType = DCILTypeReference.Load(reader);
        }
        public DCILOperCode_HandleClass(string lableID, string operCode, DCILReader reader)
        {
            this.LabelID = lableID;
            this.SetOperCode( operCode);
            this.ClassType = DCILTypeReference.Load(reader);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.ClassType = null;
            this.LocalClass = null;
        }
        public override void WriteOperData(DCILWriter writer)
        {
            if (this.ClassType != null)
            {
                writer.Write(" ");
                this.ClassType.WriteTo(writer);
            }
        }
        internal void UpdateDomState(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            this.ClassType = document.CacheTypeReference(this.ClassType);
            if (this.ClassType != null)
            {
                this.ClassType.UpdateLocalClass(clses);
            }
        }

        public DCILTypeReference ClassType = null;

        public DCILClass LocalClass = null;

    }

    internal class DCILFieldReference :IDisposable
    {
        public DCILFieldReference()
        {

        }
        public DCILFieldReference( DCILField field )
        {
            if(field == null )
            {
                throw new ArgumentNullException("field");
            }
            this.ValueType = field.ValueType;
            if (field.Parent is DCILClass)
            {
                this.OwnerType = ((DCILClass)field.Parent).GetLocalTypeReference();
            }
            this.FieldName = field.Name;
            this.LocalField = field;
        }

        public DCILFieldReference(DCILReader reader)
        {
            this.ValueType = DCILTypeReference.Load(reader);
            if (reader.PeekWord() == "modreq")
            {
                reader.ReadWord();
                this.modreq = reader.ReadStyleExtValue();
            }
            this.OwnerType = DCILTypeReference.Load(reader);
            if (reader.MatchText("::"))
            {
                reader.Position += 2;
                this.FieldName = reader.ReadWord();
            }
        }
        public void Dispose()
        {
            this.FieldName = null;
            this.LocalField = null;
            this.modreq = null;
            this.OwnerType = null;
            this.ValueType = null;
        }
        public override string ToString()
        {
            return this.ValueType?.ToString() + " " + this.OwnerType?.Name + "::" + this.FieldName;
        }
        public DCILTypeReference ValueType = null;
        public DCILTypeReference OwnerType = null;
        public DCILField LocalField = null;
        public string FieldName = null;
        public string modreq = null;

        public void UpdateLocalField(DCILClass cls)
        {
            if (this.OwnerType != null)
            {
                this.OwnerType.LocalClass = cls;
            }
            foreach (var item in cls.ChildNodes)
            {
                if (item is DCILField && item.Name == this.FieldName)
                {
                    this.LocalField = (DCILField)item;
                    break;
                }
            }
        }

        public void UpdateLocalInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            //if (this.OwnerType.Name == "__DC20210205._336")
            //{

            //}
            this.ValueType = document.CacheTypeReference(this.ValueType);
            this.OwnerType = document.CacheTypeReference(this.OwnerType);
            if (this.ValueType != null)
            {
                //var cls2 = this.ValueType.LocalClass;
                this.ValueType.UpdateLocalClass(clses);
                //if (this.ValueType.Name.StartsWith("'<PrivateImplementationDetails>'") 
                //    && this.ValueType.LocalClass == null)
                //{

                //}
            }
            if (this.OwnerType != null )
            {
                this.OwnerType.UpdateLocalClass(clses);
                var cls2 = this.OwnerType.LocalClass;
                if (cls2 != null && this.LocalField == null )
                {
                    foreach (var item in cls2.ChildNodes)
                    {
                        if (item is DCILField && item.Name == this.FieldName)
                        {
                            this.LocalField = (DCILField)item;
                            break;
                        }
                    }
                }
            }
        }
        public void WriteTo(DCILWriter writer, bool forLdtoken = false)
        {
            writer.Write("  ");

            this.ValueType.WriteTo(writer, true);
            if (this.modreq != null && this.modreq.Length > 0)
            {
                writer.Write(" modreq(");
                writer.Write(this.modreq);
                writer.Write(") ");
            }
            writer.Write(" ");
            if( this.OwnerType == null )
            {
                this.OwnerType = ((DCILClass)this.LocalField.Parent).GetLocalTypeReference();
            }
            if (this.OwnerType.IsGenericType)
            {
                this.OwnerType.WriteTo(writer, true );
            }
            else
            {
                this.OwnerType.WriteTo(writer, false);
            }
            writer.Write("::");
            if (this.LocalField == null)
            {
                writer.Write(this.FieldName);
            }
            else
            {
                writer.Write(this.LocalField.Name);
            }
        }

        public override bool Equals(object obj)
        {
            return EqualsValue(obj as DCILFieldReference);
        }
        public bool EqualsValue(DCILFieldReference info2)
        {
            if (info2 == null)
            {
                return false;
            }
            if (info2 == this)
            {
                return true;
            }
            if (this.FieldName != info2.FieldName)
            {
                return false;
            }
            if (DCUtils.EqualsStringExt(this.modreq, info2.modreq) == false)
            {
                return false;
            }
            if (DCILTypeReference.StaticEquals(this.ValueType, info2.ValueType) == false)
            {
                return false;
            }
            if (DCILTypeReference.StaticEquals(this.OwnerType, info2.OwnerType) == false)
            {
                return false;
            }
            return true;
        }
        private int _HashCode = 0;
        public override int GetHashCode()
        {
            if (this._HashCode == 0)
            {
                this._HashCode = this.FieldName.GetHashCode();
                if (this.modreq != null)
                {
                    this._HashCode += this.modreq.GetHashCode();
                }
                if (this.OwnerType != null)
                {
                    this._HashCode += this.OwnerType.GetHashCode();
                }
                if (this.ValueType != null)
                {
                    this._HashCode += this.ValueType.GetHashCode();
                }
            }
            return this._HashCode;
        }

    }

    /// <summary>
    /// 处理类型字段的指令
    /// </summary>
    internal class DCILOperCode_HandleField : DCILOperCode
    {
        public DCILOperCode_HandleField(string labelID, string operCode, DCILReader reader)
        {
            this.LabelID = labelID;
            this.SetOperCode( operCode);
            this._Value = new DCILFieldReference(reader);
        }
        public DCILOperCode_HandleField(string labelID, DCILOperCodeDefine vdef , DCILReader reader)
        {
            this.LabelID = labelID;
            this._Define = vdef;
            this._Value = new DCILFieldReference(reader);
        }
        public DCILOperCode_HandleField(string labelID, string operCode, DCILFieldReference field )
        {
            this.LabelID = labelID;
            this.SetOperCode( operCode);
            this._Value = field;
            //this.LocalField = field.LocalField;
        }
        public DCILOperCode_HandleField(string labelID, DCILOperCodeDefine vdef, DCILFieldReference field)
        {
            this.LabelID = labelID;
            this._Define = vdef;
            this._Value = field;
            //this.LocalField = field.LocalField;
        }
        public override void Dispose()
        {
            base.Dispose();
            this._Value = null;
            //this.LocalField = null;
        }
        public override string ToString()
        {
            return this.StackOffset + "#" + this.LabelID + " : " + this.OperCode + " " + this._Value.ToString();
        }
        private DCILFieldReference _Value = null;
        public DCILFieldReference Value
        {
            get
            {
                return this._Value;
            }
        }
        //public DCILField LocalField = null;
        //{
        //    get
        //    {
        //        return this._Value.LocalField;
        //    }
        //}
        public void CacheInfo(DCILDocument document)
        {
            this._Value = document.CacheFieldReference(this._Value);
        }

        public override void WriteOperData(DCILWriter writer)
        {
            //var lf = this.LocalField;
            //if (lf != null)
            //{
            //    writer.Write("  ");
            //    if (this._Value.ValueType.Mode == DCILTypeMode.GenericTypeInMethodDefine ||
            //        this._Value.ValueType.Mode == DCILTypeMode.GenericTypeInTypeDefine)
            //    {
            //        this._Value.ValueType.WriteTo(writer);
            //    }
            //    else
            //    {
            //        lf.ValueType.WriteTo(writer);
            //    }
            //    if (this._Value != null && this._Value.modreq != null && this._Value.modreq.Length > 0)
            //    {
            //        writer.Write(" modreq(");
            //        writer.Write(this._Value.modreq);
            //        writer.Write(") ");
            //    }
            //    writer.Write(' ');
            //    writer.Write(((DCILClass)lf.Parent).NameWithNested);
            //    writer.Write("::");
            //    writer.Write(lf.Name);
            //}
            //else if (this._Value != null)
            {
                this._Value.WriteTo(writer);
            }
        }
    }
    /// <summary>
    /// 处理成员方法的指令
    /// </summary>
    internal class DCILOperCode_HandleMethod : DCILOperCode
    {
        //private static readonly DCILOperCodeDefine _Define_Call = DCILOperCodeDefine.GetDefine("call");
        //private static readonly DCILOperCodeDefine _Define_Callvirt = DCILOperCodeDefine.GetDefine("callvirt");
        //private static readonly DCILOperCodeDefine _Define_NewObj = DCILOperCodeDefine.GetDefine("newobj");
        //private static readonly DCILOperCodeDefine _Define_Ldftn = DCILOperCodeDefine.GetDefine("ldftn");
        //private static readonly DCILOperCodeDefine _Define_ldvirtftn = DCILOperCodeDefine.GetDefine("ldvirtftn");
        public DCILOperCode_HandleMethod()
        {

        }
        public override void SetOperCode(string code)
        {
            if( code == "call")
            {
                base._Define = DCILOperCodeDefine._call ;
            }
            else if( code == "callvirt")
            {
                base._Define = DCILOperCodeDefine._callvirt;
            }
            else if(code == "newobj")
            {
                base._Define = DCILOperCodeDefine._newobj;
            }
            else if(code == "ldftn")
            {
                base._Define = DCILOperCodeDefine._ldftn;
            }
            else if( code == "ldvirtftn")
            {
                base._Define = DCILOperCodeDefine._ldvirtftn;
            }
            else
            {
                base._Define = DCILOperCodeDefine.GetDefine(code);
            }
        }
        public DCILInvokeMethodInfo InvokeInfo = null;
        /// <summary>
        /// 是否匹配类型名称和函数名称
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="methodName">函数名称</param>
        /// <returns>是否匹配</returns>
        public bool MatchTypeAndMethod( string typeName , string methodName , int parameterCount )
        {
            return this.InvokeInfo.OwnerType.Name == typeName 
                && this.InvokeInfo.MethodName == methodName
                && this.InvokeInfo.ParametersCount == parameterCount ;
        }

        public void ChangeTarget(DCILTypeReference type, string methodName, List<DCILMethodParamter> ps = null )
        {
            var info = this.InvokeInfo.Clone();
            info.OwnerType = type;
            info.MethodName = methodName;
            this.LocalMethod = null;
            foreach (var item in type.LocalClass.ChildNodes)
            {
                if (item is DCILMethod && item.Name == methodName)
                {
                    var method = (DCILMethod)item;
                    if (ps != null && ps.Count > 0)
                    {
                        if (ps.Count == method.ParametersCount 
                            || DCILMethodParamter.EqualsList(ps, method.Parameters, false, false) == false)
                        {
                            continue;
                        }
                    }
                    info.LocalMethod = method;
                    info.Paramters = method.Parameters;
                    this.LocalMethod = method;
                    info.IsInstance = method.IsInstance;
                    break;
                }
            }
            if( this.LocalMethod == null )
            {
                throw new NotSupportedException(type.Name + "::" + methodName);
            }
            //this.InvokeInfo.LocalMethod = this.LocalMethod;
            this.InvokeInfo = info;
        }

        public DCILMethod LocalMethod = null;
        public DCILOperCode_HandleMethod(string labelID , string code , DCILMethod method )
        {
            this.LabelID = labelID;
            this.SetOperCode(code);
            this.InvokeInfo = new DCILInvokeMethodInfo( method );
        }
        public DCILOperCode_HandleMethod(string labelID, string code, DCILInvokeMethodInfo method)
        {
            this.LabelID = labelID;
            this.SetOperCode(code);
            this.InvokeInfo = method;
        }
        //public DCILOperCode_HandleMethod(string code, DCILReader reader)
        //{
        //    this.SetOperCode(code);
        //    this.InvokeInfo = new DCILInvokeMethodInfo(reader);
        //}

        public DCILOperCode_HandleMethod(string labelID , DCILOperCodeDefine vdef , DCILReader reader)
        {
            this.LabelID = labelID;
            this._Define = vdef;
            this.InvokeInfo = new DCILInvokeMethodInfo(reader);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.LocalMethod = null;
            this.InvokeInfo = null;
        }
        public DCILOperCode_HandleMethod CacheInfo(DCILDocument document)
        {
            this.InvokeInfo = document.CacheDCILInvokeMethodInfo(this.InvokeInfo);
            return null;
        }
        public override int StackOffset
        {
            get
            {
                if( this._Define == DCILOperCodeDefine._ldftn || this._Define == DCILOperCodeDefine._ldvirtftn)
                {
                    return 1;
                }
                if(this.InvokeInfo != null )
                {
                    var lm = this.InvokeInfo.LocalMethod;
                    if( lm != null )
                    {
                        int result = 0;
                        if (this._Define == DCILOperCodeDefine._newobj)
                        {
                            result = 1;
                        }
                        else
                        {
                            if (lm.ReturnTypeInfo != DCILTypeReference.Type_Void)
                            {
                                result = 1;
                            }
                            if (lm.IsInstance)
                            {
                                result--;
                            }
                        }
                        if(lm.Parameters != null )
                        {
                            result -= lm.Parameters.Count;
                        }
                        return result;
                    }
                    else
                    {
                        int result = 0;
                        
                        if( this._Define == DCILOperCodeDefine._newobj)
                        {
                            result = 1;
                        }
                        else
                        {
                            if (this.InvokeInfo.ReturnType != DCILTypeReference.Type_Void)
                            {
                                result = 1;
                            }
                            if (this.InvokeInfo.IsInstance)
                            {
                                result--;
                            }
                        }
                        if( this.InvokeInfo.Paramters != null )
                        {
                            result -= this.InvokeInfo.Paramters.Count;
                        }
                        return result;
                    }
                }
                return 0;
            }
        }

        public override void WriteTo(DCILWriter writer)
        {
            //if (this.InvokeInfo.MethodName == "MyDispose" && this.OperCode == "callvirt")
            //{

            //}
            writer.EnsureNewLine();
            writer.Write(this.LabelID);
            writer.Write(": ");
            if (this.LabelID.Length < 10)
            {
                writer.WriteWhitespace(10 - this.LabelID.Length);
            }
            //if( this.InvokeInfo.MethodName == "Object_ToString")
            //{

            //}
            writer.Write(this.OperCode);
            if (this.InvokeInfo != null)
            {
                writer.WriteWhitespace(Math.Max(1, 10 - this.OperCode.Length));
                this.InvokeInfo.WriteTo(writer);
            }

        }
        public override void WriteOperData(DCILWriter writer)
        {
            if (this.InvokeInfo != null)
            {
                writer.Write(" ");
                this.InvokeInfo.WriteTo(writer);
            }
        }
        public override string ToString()
        {
            var str = new StringBuilder( this.StackOffset +"#" + this.LabelID + ":" + this.OperCode + " " + this.InvokeInfo.OwnerType.Name + "::" + this.InvokeInfo.MethodName);
            if(this.InvokeInfo.ParametersCount > 0 )
            {
                str.Append("(");
                for(int iCount = 0; iCount < this.InvokeInfo.ParametersCount; iCount ++)
                {
                    if(iCount > 0 )
                    {
                        str.Append(',');
                    }
                    str.Append(this.InvokeInfo.Paramters[iCount].ValueType.Name);
                }
                str.Append(")");
            }
            return str.ToString();
        }
    }
    internal class DCILOperCode_Try_Catch_Finally : DCILOperCode
    {
        private static readonly DCILOperCodeDefine _SrcDefine = new DCILOperCodeDefine("try_cath_finally");

        public DCILOperCode_Try_Catch_Finally()
        {
            base._Define = _SrcDefine;
        }

        public string SingleLineContent = null;

        public DCILObject _Try = null;
        public List<DCILCatchBlock> _Catchs = null;
        public DCILObject _Finally = null;
        public DCILObject _fault = null;
        public override DCILOperCode Clone(string newLabelID)
        {
            return base.Clone(newLabelID);
        }
        public bool HasTryOperCodes()
        {
            return this._Try != null && this._Try.OperCodes != null && this._Try.OperCodes.Count > 0;
        }
        public bool HasCatchs()
        {
            return this._Catchs != null && this._Catchs.Count > 0;
        }
        public bool HasFinallyOperCodes()
        {
            return this._Finally != null && this._Finally.OperCodes != null && this._Finally.OperCodes.Count > 0;
        }
        public bool HasFaultOperCodes()
        {
            return this._fault != null && this._fault.OperCodes != null && this._fault.OperCodes.Count > 0;
        }
        public override void Dispose()
        {
            base.Dispose();
            if( this._Try != null )
            {
                this._Try.Dispose();
                this._Try = null;
            }
            if(this._Catchs != null )
            {
                foreach( var item in this._Catchs)
                {
                    item.Dispose();
                }
                this._Catchs.Clear();
                this._Catchs = null;
            }
            if(this._Finally != null )
            {
                this._Finally.Dispose();
                this._Finally = null;
            }
            if( this._fault != null )
            {
                this._fault.Dispose();
                this._fault = null;
            }
        }
        public int TotalOperCodesCount
        {
            get
            {
                int result = 0;
                if (this._Try != null)
                {
                    result = this._Try.TotalOperCodesCount;
                }
                if (this._Catchs != null)
                {
                    foreach (var item in this._Catchs)
                    {
                        result += item.TotalOperCodesCount;
                    }
                }
                if (this._Finally != null)
                {
                    result += this._Finally.TotalOperCodesCount;
                }
                if (this._fault != null)
                {
                    result += this._fault.TotalOperCodesCount;
                }
                return result;
            }
        }
        public override string ToString()
        {
            var str = new StringBuilder();

            WriteText(str, "try", this._Try.OperCodes);
            if (this._Catchs != null && this._Catchs.Count > 0)
            {
                foreach (var item in this._Catchs)
                {
                    WriteText(str, "catch " + item.ExcpetionType?.ToString(), item.OperCodes);
                }
            }
            if (this._Finally != null)
            {
                WriteText(str, "finally", this._Finally.OperCodes);
            }
            if (this._fault != null)
            {
                WriteText(str, "fault", this._fault.OperCodes);
            }
            return str.ToString();
        }

        private void WriteText(StringBuilder str, string name, DCILOperCodeList list)
        {
            str.Append(name);
            str.Append("{");
            if (list != null && list.Count > 0)
            {
                str.Append(list.Count.ToString());
            }
            str.Append("}");
        }

        public override void WriteTo(DCILWriter writer)
        {
            writer.EnsureNewLine();
            if( this.SingleLineContent != null &&  this.SingleLineContent.Length > 0 )
            {
                writer.WriteLine(".try " + this.SingleLineContent);
                return;
            }
            writer.WriteLine(".try");
            writer.WriteStartGroup();
            foreach (var item in this._Try.OperCodes)
            {
                item.WriteTo(writer);
            }
            writer.WriteEndGroup();

            if (this._Catchs != null && this._Catchs.Count > 0)
            {
                foreach (var item in this._Catchs)
                {
                    writer.Write("catch ");
                    item.ExcpetionType.WriteTo(writer, false);
                    writer.WriteLine();
                    writer.WriteStartGroup();
                    foreach (var item2 in item.OperCodes)
                    {
                        item2.WriteTo(writer);
                    }
                    writer.WriteEndGroup();
                }
            }
            if (this._fault != null && this._fault.OperCodes != null && this._fault.OperCodes.Count > 0)
            {
                writer.WriteLine("fault");
                writer.WriteStartGroup();
                foreach (var itemi in this._fault.OperCodes)
                {
                    itemi.WriteTo(writer);
                }
                writer.WriteEndGroup();
            }
            if (this._Finally != null)
            {
                writer.WriteLine("finally");
                writer.WriteStartGroup();
                foreach (var itemi in this._Finally.OperCodes)
                {
                    itemi.WriteTo(writer);
                }
                writer.WriteEndGroup();
            }
        }
    }

    internal class DCILCatchBlock : DCILObject
    {
        public DCILCatchBlock()
        {
            this._Name = "catch";
            this.OperCodes = new DCILOperCodeList();
        }
        public DCILTypeReference ExcpetionType = null;
        public string ExcpetionTypeName = null;
        public override string ToString()
        {
            return "catch " + this.ExcpetionType?.ToString();
        }
        public override void Dispose()
        {
            base.Dispose();
            this.ExcpetionType = null;
            this.ExcpetionTypeName = null;
        }
    }
    internal class DCILOperCodeComment : DCILOperCode
    {
        public DCILOperCodeComment()
        {

        }
        public DCILOperCodeComment(string txt)
        {
            this.Text = txt;
        }
        public override void WriteTo(DCILWriter writer)
        {
            if (this.Text != null && this.Text.Length > 0)
            {
                writer.WriteLine(Environment.NewLine + "//" + this.Text);
            }
        }
        public override string ToString()
        {
            return "//" + this.Text;
        }
        public string Text = null;
        public override void Dispose()
        {
            base.Dispose();
            this.Text = null;
        }
    }
    internal enum ILOperCodeType
    {
        Nop,
        Normal,
        Field,
        Method,
        Class,
        Prefix,
        ldstr,
        switch_,
        ldtoken,
        JumpShort,
        Jump,
        ArgsOrLocalsByName,
        LoadNumberByOperData
    }

    internal enum DCILOpCodeValue
    {
        Nop = 0,
        Break = 1,
        Ldarg_0 = 2,
        Ldarg_1 = 3,
        Ldarg_2 = 4,
        Ldarg_3 = 5,
        Ldloc_0 = 6,
        Ldloc_1 = 7,
        Ldloc_2 = 8,
        Ldloc_3 = 9,
        Stloc_0 = 10,
        Stloc_1 = 11,
        Stloc_2 = 12,
        Stloc_3 = 13,
        Ldarg_S = 14,
        Ldarga_S = 0xF,
        Starg_S = 0x10,
        Ldloc_S = 17,
        Ldloca_S = 18,
        Stloc_S = 19,
        Ldnull = 20,
        Ldc_I4_M1 = 21,
        Ldc_I4_0 = 22,
        Ldc_I4_1 = 23,
        Ldc_I4_2 = 24,
        Ldc_I4_3 = 25,
        Ldc_I4_4 = 26,
        Ldc_I4_5 = 27,
        Ldc_I4_6 = 28,
        Ldc_I4_7 = 29,
        Ldc_I4_8 = 30,
        Ldc_I4_S = 0x1F,
        Ldc_I4 = 0x20,
        Ldc_I8 = 33,
        Ldc_R4 = 34,
        Ldc_R8 = 35,
        Dup = 37,
        Pop = 38,
        Jmp = 39,
        Call = 40,
        Calli = 41,
        Ret = 42,
        Br_S = 43,
        Brfalse_S = 44,
        Brtrue_S = 45,
        Beq_S = 46,
        Bge_S = 47,
        Bgt_S = 48,
        Ble_S = 49,
        Blt_S = 50,
        Bne_Un_S = 51,
        Bge_Un_S = 52,
        Bgt_Un_S = 53,
        Ble_Un_S = 54,
        Blt_Un_S = 55,
        Br = 56,
        Brfalse = 57,
        Brtrue = 58,
        Beq = 59,
        Bge = 60,
        Bgt = 61,
        Ble = 62,
        Blt = 0x3F,
        Bne_Un = 0x40,
        Bge_Un = 65,
        Bgt_Un = 66,
        Ble_Un = 67,
        Blt_Un = 68,
        Switch = 69,
        Ldind_I1 = 70,
        Ldind_U1 = 71,
        Ldind_I2 = 72,
        Ldind_U2 = 73,
        Ldind_I4 = 74,
        Ldind_U4 = 75,
        Ldind_I8 = 76,
        Ldind_I = 77,
        Ldind_R4 = 78,
        Ldind_R8 = 79,
        Ldind_Ref = 80,
        Stind_Ref = 81,
        Stind_I1 = 82,
        Stind_I2 = 83,
        Stind_I4 = 84,
        Stind_I8 = 85,
        Stind_R4 = 86,
        Stind_R8 = 87,
        Add = 88,
        Sub = 89,
        Mul = 90,
        Div = 91,
        Div_Un = 92,
        Rem = 93,
        Rem_Un = 94,
        And = 95,
        Or = 96,
        Xor = 97,
        Shl = 98,
        Shr = 99,
        Shr_Un = 100,
        Neg = 101,
        Not = 102,
        Conv_I1 = 103,
        Conv_I2 = 104,
        Conv_I4 = 105,
        Conv_I8 = 106,
        Conv_R4 = 107,
        Conv_R8 = 108,
        Conv_U4 = 109,
        Conv_U8 = 110,
        Callvirt = 111,
        Cpobj = 112,
        Ldobj = 113,
        Ldstr = 114,
        Newobj = 115,
        Castclass = 116,
        Isinst = 117,
        Conv_R_Un = 118,
        Unbox = 121,
        Throw = 122,
        Ldfld = 123,
        Ldflda = 124,
        Stfld = 125,
        Ldsfld = 126,
        Ldsflda = 0x7F,
        Stsfld = 0x80,
        Stobj = 129,
        Conv_Ovf_I1_Un = 130,
        Conv_Ovf_I2_Un = 131,
        Conv_Ovf_I4_Un = 132,
        Conv_Ovf_I8_Un = 133,
        Conv_Ovf_U1_Un = 134,
        Conv_Ovf_U2_Un = 135,
        Conv_Ovf_U4_Un = 136,
        Conv_Ovf_U8_Un = 137,
        Conv_Ovf_I_Un = 138,
        Conv_Ovf_U_Un = 139,
        Box = 140,
        Newarr = 141,
        Ldlen = 142,
        Ldelema = 143,
        Ldelem_I1 = 144,
        Ldelem_U1 = 145,
        Ldelem_I2 = 146,
        Ldelem_U2 = 147,
        Ldelem_I4 = 148,
        Ldelem_U4 = 149,
        Ldelem_I8 = 150,
        Ldelem_I = 151,
        Ldelem_R4 = 152,
        Ldelem_R8 = 153,
        Ldelem_Ref = 154,
        Stelem_I = 155,
        Stelem_I1 = 156,
        Stelem_I2 = 157,
        Stelem_I4 = 158,
        Stelem_I8 = 159,
        Stelem_R4 = 160,
        Stelem_R8 = 161,
        Stelem_Ref = 162,
        Ldelem = 163,
        Stelem = 164,
        Unbox_Any = 165,
        Conv_Ovf_I1 = 179,
        Conv_Ovf_U1 = 180,
        Conv_Ovf_I2 = 181,
        Conv_Ovf_U2 = 182,
        Conv_Ovf_I4 = 183,
        Conv_Ovf_U4 = 184,
        Conv_Ovf_I8 = 185,
        Conv_Ovf_U8 = 186,
        Refanyval = 194,
        Ckfinite = 195,
        Mkrefany = 198,
        Ldtoken = 208,
        Conv_U2 = 209,
        Conv_U1 = 210,
        Conv_I = 211,
        Conv_Ovf_I = 212,
        Conv_Ovf_U = 213,
        Add_Ovf = 214,
        Add_Ovf_Un = 215,
        Mul_Ovf = 216,
        Mul_Ovf_Un = 217,
        Sub_Ovf = 218,
        Sub_Ovf_Un = 219,
        Endfinally = 220,
        Leave = 221,
        Leave_S = 222,
        Stind_I = 223,
        Conv_U = 224,
        Prefix7 = 248,
        Prefix6 = 249,
        Prefix5 = 250,
        Prefix4 = 251,
        Prefix3 = 252,
        Prefix2 = 253,
        Prefix1 = 254,
        Prefixref = 0xFF,
        Arglist = 65024,
        Ceq = 65025,
        Cgt = 65026,
        Cgt_Un = 65027,
        Clt = 65028,
        Clt_Un = 65029,
        Ldftn = 65030,
        Ldvirtftn = 65031,
        Ldarg = 65033,
        Ldarga = 65034,
        Starg = 65035,
        Ldloc = 65036,
        Ldloca = 65037,
        Stloc = 65038,
        Localloc = 65039,
        Endfilter = 65041,
        Unaligned_ = 65042,
        Volatile_ = 65043,
        Tailcal = 65044,
        Initobj = 65045,
        Constrained_ = 65046,
        Cpblk = 65047,
        Initblk = 65048,
        Rethrow = 65050,
        Sizeof = 65052,
        Refanytype = 65053,
        Readonly_ = 65054
    }
    /// <summary>
    /// IL指令信息对象
    /// </summary>
    internal class DCILOperCodeDefine
    {
        static DCILOperCodeDefine()
        {
            var extCodeTypes = new Dictionary<string, ILOperCodeType>();
            extCodeTypes["nop"] = ILOperCodeType.Nop;
            extCodeTypes["ldstr"] = ILOperCodeType.ldstr;
            extCodeTypes["switch"] = ILOperCodeType.switch_;
            extCodeTypes["box"] = ILOperCodeType.Class;
            extCodeTypes["call"] = ILOperCodeType.Method;
            extCodeTypes["callvirt"] = ILOperCodeType.Method;
            extCodeTypes["castclass"] = ILOperCodeType.Class;
            extCodeTypes["constrained."] = ILOperCodeType.Class;
            extCodeTypes["cpobj"] = ILOperCodeType.Class;
            extCodeTypes["initobj"] = ILOperCodeType.Class;
            extCodeTypes["isinst"] = ILOperCodeType.Class;
            extCodeTypes["ldfld"] = ILOperCodeType.Field;
            extCodeTypes["ldflda"] = ILOperCodeType.Field;
            extCodeTypes["ldftn"] = ILOperCodeType.Method;
            extCodeTypes["ldelem"] = ILOperCodeType.Class;
            extCodeTypes["ldelema"] = ILOperCodeType.Class;
            extCodeTypes["ldobj"] = ILOperCodeType.Class;
            extCodeTypes["ldsfld"] = ILOperCodeType.Field;
            extCodeTypes["ldsflda"] = ILOperCodeType.Field;
            extCodeTypes["ldtoken"] = ILOperCodeType.ldtoken;
            extCodeTypes["ldvirtftn"] = ILOperCodeType.Method;
            extCodeTypes["mkrefany"] = ILOperCodeType.Class;
            extCodeTypes["newarr"] = ILOperCodeType.Class;
            extCodeTypes["newobj"] = ILOperCodeType.Method;
            extCodeTypes["refanyval"] = ILOperCodeType.Class;
            extCodeTypes["sizeof"] = ILOperCodeType.Class;
            extCodeTypes["stelem"] = ILOperCodeType.Class;
            extCodeTypes["stfld"] = ILOperCodeType.Field;
            extCodeTypes["stobj"] = ILOperCodeType.Class;
            extCodeTypes["stsfld"] = ILOperCodeType.Field;
            extCodeTypes["unbox"] = ILOperCodeType.Class;
            extCodeTypes["unbox.any"] = ILOperCodeType.Class;

            extCodeTypes["beq.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bge.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bge.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bgt.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bgt.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["ble.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["ble.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["blt.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["blt.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bne.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["br.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["brfalse.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["brtrue.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["leave.s"] = ILOperCodeType.JumpShort;

            extCodeTypes["beq"] = ILOperCodeType.Jump;
            extCodeTypes["bge"] = ILOperCodeType.Jump;
            extCodeTypes["bge.un"] = ILOperCodeType.Jump;
            extCodeTypes["bgt"] = ILOperCodeType.Jump;
            extCodeTypes["bgt.un"] = ILOperCodeType.Jump;
            extCodeTypes["ble"] = ILOperCodeType.Jump;
            extCodeTypes["ble.un"] = ILOperCodeType.Jump;
            extCodeTypes["blt"] = ILOperCodeType.Jump;
            extCodeTypes["blt.un"] = ILOperCodeType.Jump;
            extCodeTypes["bne.un"] = ILOperCodeType.Jump;
            extCodeTypes["br"] = ILOperCodeType.Jump;
            extCodeTypes["brfalse"] = ILOperCodeType.Jump;
            extCodeTypes["brtrue"] = ILOperCodeType.Jump;
            extCodeTypes["leave"] = ILOperCodeType.Jump;

            extCodeTypes["ldarg"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldarg.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldarga"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldarga.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldloc"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldloc.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldloca"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldloca.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["starg"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["starg.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["stloc"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["stloc.s"] = ILOperCodeType.ArgsOrLocalsByName;

            extCodeTypes["ldc.i4"] = ILOperCodeType.LoadNumberByOperData;
            extCodeTypes["ldc.i4.s"] = ILOperCodeType.LoadNumberByOperData;
            extCodeTypes["ldc.i8"] = ILOperCodeType.LoadNumberByOperData;
            extCodeTypes["ldc.r4"] = ILOperCodeType.LoadNumberByOperData;
            extCodeTypes["ldc.r8"] = ILOperCodeType.LoadNumberByOperData;
             
            var fields = typeof(System.Reflection.Emit.OpCodes).GetFields(
                BindingFlags.Public | BindingFlags.Static);
            foreach( var f in fields )
            {
                var code = (System.Reflection.Emit.OpCode)f.GetValue(null);
                _Defines[code.Name.ToLower()] = new DCILOperCodeDefine(code, extCodeTypes);
            }
            var values = _Defines.Values;

            _br = _Defines["br"];
            _br_s = _Defines["br.s"];
            _pop = _Defines["pop"];
            _call = _Defines["call"];
            _callvirt = _Defines["callvirt"];
            _nop = _Defines["nop"];
            _dup = _Defines["dup"];
            _brtrue = _Defines["brtrue"];
            _brtrue_s = _Defines["brtrue.s"];
            _newobj = _Defines["newobj"];
            _ldftn = _Defines["ldftn"];
            _ldvirtftn = _Defines["ldvirtftn"];
            _ldtoken = _Defines["ldtoken"];
            _switch = _Defines["switch"];
            _ldstr = _Defines["ldstr"];
            _ldc_i4 = _Defines["ldc.i4"];
            _ldsfld = _Defines["ldsfld"];
        }

        public static readonly DCILOperCodeDefine _br;
        public static readonly DCILOperCodeDefine _br_s;
        public static readonly DCILOperCodeDefine _nop;
        public static readonly DCILOperCodeDefine _pop;
        public static readonly DCILOperCodeDefine _call;
        public static readonly DCILOperCodeDefine _callvirt;
        public static readonly DCILOperCodeDefine _dup;
        public static readonly DCILOperCodeDefine _brtrue;
        public static readonly DCILOperCodeDefine _brtrue_s;
        public static readonly DCILOperCodeDefine _newobj;
        public static readonly DCILOperCodeDefine _ldftn;
        public static readonly DCILOperCodeDefine _ldvirtftn;
        public static readonly DCILOperCodeDefine _ldtoken;
        public static readonly DCILOperCodeDefine _switch;
        public static readonly DCILOperCodeDefine _ldstr;
        public static readonly DCILOperCodeDefine _ldc_i4;
        public static readonly DCILOperCodeDefine _ldsfld;

        private static readonly SortedDictionary<string, DCILOperCodeDefine> _Defines
               = new SortedDictionary<string, DCILOperCodeDefine>();

        public static DCILOperCodeDefine GetDefine( string codeName)
        {
            if( codeName == null || codeName.Length == 0 )
            {
                throw new ArgumentNullException("codeName");
            }
            DCILOperCodeDefine info = null;
            if(_Defines.TryGetValue(codeName.ToLower() , out info ))
            {
                info.RefCount++;
                return info;
            }
            else
            {
                throw new NotSupportedException(codeName);
            }
        }

        public DCILOperCodeDefine(string name  )
        {
            this.Name = name;
        }

        private DCILOperCodeDefine(
            System.Reflection.Emit.OpCode code,
            Dictionary<string, ILOperCodeType> extCodeTypes )
        {
            this.FlowControl = code.FlowControl;
            this.Name = code.Name;
            this.OpCodeType = code.OpCodeType;
            this.OperandType = code.OperandType;
            this.Size = code.Size;
            this.StackBehaviourPop = code.StackBehaviourPop;
            this.StackBehaviourPush = code.StackBehaviourPush;
            this.Value = ( DCILOpCodeValue)code.Value;
            if (extCodeTypes.TryGetValue(this.Name, out this.ExtCodeType) == false)
            {
                this.ExtCodeType = ILOperCodeType.Normal;
            }
            this.StackOffset = GetStackAdd(this.StackBehaviourPush) + GetStackAdd(this.StackBehaviourPop);

            //if( stdStackOffset.TryGetValue( this.Name , out this.StackOffset ) == false )
            //{
            //    this.StackOffset = 0;
            //}
            this.IsPrefix = code.OpCodeType == System.Reflection.Emit.OpCodeType.Prefix;
            this.WithoutOperData = this.OperandType == System.Reflection.Emit.OperandType.InlineNone;
        }
        private static int GetStackAdd( System.Reflection.Emit.StackBehaviour sb )
        {
            switch( sb )
            {
                case System.Reflection.Emit.StackBehaviour.Pop0: return 0;
                case System.Reflection.Emit.StackBehaviour.Pop1:return -1;
                case System.Reflection.Emit.StackBehaviour.Pop1_pop1: return -2;
                case System.Reflection.Emit.StackBehaviour.Popi:return -1;
                case System.Reflection.Emit.StackBehaviour.Popi_pop1:return -2;
                case System.Reflection.Emit.StackBehaviour.Popi_popi:return -2;
                case System.Reflection.Emit.StackBehaviour.Popi_popi8:return -2;
                case System.Reflection.Emit.StackBehaviour.Popi_popi_popi: return -3;
                case System.Reflection.Emit.StackBehaviour.Popi_popr4: return -2;
                case System.Reflection.Emit.StackBehaviour.Popi_popr8:return -2;
                case System.Reflection.Emit.StackBehaviour.Popref: return -1;
                case System.Reflection.Emit.StackBehaviour.Popref_pop1:return -2;
                case System.Reflection.Emit.StackBehaviour.Popref_popi:return -2;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_pop1:return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popi:return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popi8: return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popr4:return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popr8:return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popref:return -3;
                case System.Reflection.Emit.StackBehaviour.Push0:return 0;
                case System.Reflection.Emit.StackBehaviour.Push1: return 1;
                case System.Reflection.Emit.StackBehaviour.Push1_push1:return 2;
                case System.Reflection.Emit.StackBehaviour.Pushi:return 1;
                case System.Reflection.Emit.StackBehaviour.Pushi8:return 1;
                case System.Reflection.Emit.StackBehaviour.Pushr4: return 1;
                case System.Reflection.Emit.StackBehaviour.Pushr8:return 1;
                case System.Reflection.Emit.StackBehaviour.Pushref: return 1;
            }
            return 0;
        }
        public int RefCount = 0;
        public readonly System.Reflection.Emit.FlowControl FlowControl;
        public readonly string Name;
        public readonly System.Reflection.Emit.OpCodeType OpCodeType;
        public readonly System.Reflection.Emit.OperandType OperandType;
        public readonly int Size;
        public readonly System.Reflection.Emit.StackBehaviour StackBehaviourPop;
        public readonly System.Reflection.Emit.StackBehaviour StackBehaviourPush;
        public readonly DCILOpCodeValue Value;
        public readonly bool IsPrefix;
        public readonly ILOperCodeType ExtCodeType;
        public readonly int StackOffset = 0;

        public readonly bool WithoutOperData;
        
        public override string ToString()
        {
            var str = this.Name + " ExtCodeType=" + this.ExtCodeType + " WOD=" + this.WithoutOperData + " ST=" + this.StackOffset;
            if( this.IsPrefix )
            {
                str = str + " Prefix";
            }
            return str;
        }
    }
    internal class DCILOperCode : IDisposable
    {
        public static DCILOperCode Gen_ldci4_Code(string labelID, int v)
        {
            switch (v)
            {
                case 0: return new DCILOperCode(labelID, "ldc.i4.0");
                case 1: return new DCILOperCode(labelID, "ldc.i4.1");
                case 2: return new DCILOperCode(labelID, "ldc.i4.2");
                case 3: return new DCILOperCode(labelID, "ldc.i4.3");
                case 4: return new DCILOperCode(labelID, "ldc.i4.4");
                case 5: return new DCILOperCode(labelID, "ldc.i4.5");
                case 6: return new DCILOperCode(labelID, "ldc.i4.6");
                case 7: return new DCILOperCode(labelID, "ldc.i4.7");
                case 8: return new DCILOperCode(labelID, "ldc.i4.8");
                case -1: return new DCILOperCode(labelID, "ldc.i4.m1");
                default:
                    if (v > -127 && v < 128)
                    {
                        return new DCILOperCode(labelID, "ldc.i4.s", DCUtils.GetInt32ValueString(v));
                    }
                    else
                    {
                        return new DCILOperCode(labelID, "ldc.i4", DCUtils.GetInt32ValueString(v));
                    }
            }
        }

        ///// <summary>
        ///// 判断是否存在指令数据
        ///// </summary>
        ///// <param name="strOperCode"></param>
        ///// <returns></returns>
        //public static bool WithoutOperData(string strOperCode)
        //{
        //    return DCILOperCodeDefine.GetDefine(strOperCode).WithoutOperData;
        //}
           
        /// <summary>
        /// 获得指令类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        //public static ILOperCodeType GetOperCodeType(string code)
        //{
        //    return DCILOperCodeDefine.GetDefine(code).ExtCodeType;
             
        //}
        public DCILOperCode()
        {

        }
        public DCILOperCode(string vlabelID, string voperCode, string voperData = null )
        {
            this.LabelID = vlabelID;
            this.SetOperCode( voperCode);
            this.OperData = voperData;
        }

        public DCILOperCode(string vlabelID, DCILOperCodeDefine myDefine, string voperData = null )
        {
            if( myDefine == null )
            {
                throw new ArgumentNullException("myDefine");
            }
            this.LabelID = vlabelID;
            this._Define = myDefine;
            this.OperData = voperData;
        }

        protected DCILOperCodeDefine _Define = null;
        public DCILOperCodeDefine NativeDefine
        {
            get
            {
                return this._Define;
            }
        }
        public virtual void Dispose()
        {
            this._Define = null;
            this.OperData = null;
        } 

        //public bool BitSizeChanged = false;

        public virtual int StackOffset
        {
            get
            {
                return this._Define.StackOffset;
            }
        }

        //public bool IsJumpOperCode()
        //{
        //    return this._Define.ExtCodeType == ILOperCodeType.Jump 
        //        || this._Define.ExtCodeType == ILOperCodeType.JumpShort;
        //}

        /// <summary>
        /// 判断是否为修饰性指令，必须紧跟在后面的指令之前
        /// </summary>
        /// <returns></returns>
        public bool IsPrefixOperCode()
        {
            return this._Define.IsPrefix;
            //return this.OperCodeValue == DCILOpCodeValue.Volatile_
            //    || this.OperCodeValue == DCILOpCodeValue.Constrained_
            //    //|| operCode == "cpblk"
            //    || this.OperCodeValue == DCILOpCodeValue.Unaligned_
            //    || this.OperCodeValue == DCILOpCodeValue.Tailcal;// == "tailcall";
        }
        public virtual void WriteTo(DCILWriter writer)
        {
            writer.EnsureNewLine();
            writer.Write(this.LabelID);
            writer.Write(": ");
            if (this.LabelID.Length < 10)
            {
                writer.WriteWhitespace(10 - this.LabelID.Length);
            }
            writer.Write(this.OperCode);
            WriteOperData(writer);

        }
        public virtual void WriteOperData(DCILWriter writer)
        {
            if (this.OperData != null && this.OperData.Length > 0)
            {
                writer.WriteWhitespace(Math.Max(1, 10 - this.OperCode.Length));
                writer.Write(this.OperData);
            }
        }
        
        //private static int _InstanceIndexCounter = 0;
        //public int InstanceIndex = _InstanceIndexCounter++;

        //public bool Enabled = true;
        //public DCILOperCodeList OwnerList = null;
        //public DCILMethod OwnerMethod = null;
        //public string NativeSource = null;
        public string LabelID = null;
        public bool HasLabelID()
        {
            return this.LabelID != null && this.LabelID.Length > 0;
        }
        //private DCILOpCodeIntegerValue _OperCodeIntegerValue = DCILOpCodeIntegerValue.Nop;
        //public DCILOpCodeIntegerValue  OperCodeIntegerValue
        //{
        //    get
        //    {
        //        return this._OperCodeIntegerValue;
        //    }
        //}

        /// <summary>
        /// 字节长度
        /// </summary>
        public virtual int ByteSize
        {
            get
            {
                return this._Define.Size;
            }
        }

        public string OperCode
        {
            get
            {
                return this._Define.Name;
            }
        }

        public DCILOpCodeValue OperCodeValue
        {
            get
            {
                return (DCILOpCodeValue)this._Define.Value;
            }
        }

        public virtual void SetOperCode( string code )
        {
            this._Define = DCILOperCodeDefine.GetDefine(code);
        }

        public string OperData = null;
        //public int LineIndex = 0;
        //public int EndLineIndex = 0;
        public virtual DCILOperCode Clone(string newLabelID)
        {
            var result = (DCILOperCode)this.MemberwiseClone();
            result.LabelID = newLabelID;
            return result;
        }
        public override string ToString()
        {
            if (this.OperData == null || this.OperData.Length == 0)
            {
                return this.StackOffset + "#" + this.LabelID + " : " + this.OperCode;
            }
            else
            {
                return this.StackOffset + "#" + this.LabelID + " : " + this.OperCode + "     " + this.OperData;
            }
        }
        ///// <summary>
        ///// get IL opercode from a IL code line
        ///// </summary>
        ///// <param name="line">IL code line</param>
        ///// <param name="labelID">label id</param>
        ///// <param name="operData">opertion data</param>
        ///// <returns></returns>
        //internal static string GetILCode(string line, ref string labelID, ref string operData)
        //{
        //    int len = line.Length;
        //    for (int iCount = 0; iCount < len; iCount++)
        //    {
        //        char c = line[iCount];
        //        if (c == ':')
        //        {
        //            labelID = line.Substring(0, iCount).Trim();
        //            for (iCount++; iCount < len; iCount++)
        //            {
        //                var c2 = line[iCount];
        //                if (c2 != ' ' && c2 != '\t')
        //                {
        //                    string operCode = null;
        //                    for (int iCount2 = iCount + 1; iCount2 < len; iCount2++)
        //                    {
        //                        var c3 = line[iCount2];
        //                        if (c3 == ' ' || c3 == '\t')
        //                        {
        //                            operCode = line.Substring(iCount, iCount2 - iCount);
        //                            if (iCount2 < len - 1)
        //                            {
        //                                operData = line.Substring(iCount2).Trim();
        //                            }
        //                            break;
        //                        }
        //                    }
        //                    if (operCode == null)
        //                    {
        //                        operCode = line.Substring(iCount);
        //                    }
        //                    return operCode;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
    }

    internal class DCILOperCode_Switch : DCILOperCode
    {
        //private static readonly DCILOperCodeDefine _SrcInfo = DCILOperCodeDefine.GetDefine("switch");

        public DCILOperCode_Switch(string labelID )
        {
            this.LabelID = labelID;
            this._Define = DCILOperCodeDefine._switch;
        }

        public DCILOperCode_Switch(string labelID, DCILReader reader)
        {
            this.LabelID = labelID;
            this._Define = DCILOperCodeDefine._switch;
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == ")")
                {
                    break;
                }
                if (strWord.StartsWith("IL_", StringComparison.OrdinalIgnoreCase))
                {
                    this.TargetLabels.Add(strWord);
                }
            }
        }
        public override int ByteSize
        {
            get
            {
                return base.ByteSize + this.TargetLabels.Count * 4 + 4;
            }
        }

        public List<string> TargetLabels = new List<string>();
        public override void WriteOperData(DCILWriter writer)
        {
            writer.Write("(");
            var len = this.TargetLabels.Count;
            for (int iCount = 0; iCount < len; iCount++)
            {
                if (iCount > 0)
                {
                    writer.Write(',');
                }
                writer.Write(this.TargetLabels[iCount]);
            }
            writer.WriteLine(")");
        }
    }

    internal class DCILOperCode_LdToken : DCILOperCode
    {
        //private static readonly DCILOperCodeDefine _SrcInfo = DCILOperCodeDefine.GetDefine("ldtoken");

        public DCILOperCode_LdToken(string labelID, DCILFieldReference field)
        {
            this._Define = DCILOperCodeDefine._ldtoken;
            this.LabelID = labelID;
            this.OperType = "field";
            this.FieldReference = field;
        }
        public DCILOperCode_LdToken(string labelID, DCILTypeReference type)
        {
            this._Define = DCILOperCodeDefine._ldtoken;
            this.LabelID = labelID;
            this.ClassType = type;
        }
        //public const string CodeName_LdToken = "ldtoken";
        public DCILOperCode_LdToken(string labelID, DCILReader reader)
        {
            this._Define = DCILOperCodeDefine._ldtoken;
            this.LabelID = labelID;
            int pos = reader.Position;
            string strWord = reader.ReadWord();
            if (strWord == "field")
            {
                this.OperType = strWord;
                this.FieldReference = new DCILFieldReference(reader);
            }
            else if (strWord == "method")
            {
                this.OperType = "method";
                this.Method = new DCILInvokeMethodInfo(reader);
            }
            else
            {
                reader.Position = pos;
                this.ClassType = DCILTypeReference.Load(reader);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.OperType = null;
            this.FieldReference = null;
            this.Method = null;
            this.ClassType = null;
        }
        public string OperType = null;
        public DCILFieldReference FieldReference = null;
        public DCILInvokeMethodInfo Method = null;
        public DCILTypeReference ClassType = null;

        public DCILMemberInfo LocalMemberInfo = null;
        public void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            if (this.FieldReference != null)
            {
                this.FieldReference.UpdateLocalInfo(document, clses);
            }
            else if (this.Method != null)
            {
                this.Method = document.CacheDCILInvokeMethodInfo(this.Method);
            }
            else if (this.ClassType != null)
            {
                this.ClassType = document.CacheTypeReference(this.ClassType);
            }
        }

        public override void WriteOperData(DCILWriter writer)
        {
            writer.Write(" ");
            if (this.OperType != null)
            {
                writer.Write(" " + this.OperType + " ");
            }
            if (this.LocalMemberInfo is DCILClass)
            {
                writer.Write(" ");
                writer.Write(((DCILClass)this.LocalMemberInfo).NameWithNested);
            }
            else
            {
                if (this.FieldReference != null)
                {
                    this.FieldReference.WriteTo(writer);
                }
                else if (this.Method != null)
                {
                    this.Method.WriteTo(writer);
                }
                else if (this.ClassType != null)
                {
                    this.ClassType.WriteTo(writer,
                        this.ClassType.IsLocalType
                        || this.ClassType.IsGenericType
                        || this.ClassType.IsGenericType
                        || this.ClassType.IsArray);
                }
            }
        }

    }

    //internal class DCILStringValue
    //{
    //    public DCILStringValue( string text )
    //    {
    //        if(text == null )
    //        {
    //            throw new ArgumentNullException("text");
    //        }
    //        this.Value = text;
    //        this.IsBinary = false;
    //        foreach( var c in text )
    //        {
    //            if( c > 128)
    //            {
    //                this.IsBinary = true;
    //                break;
    //            }
    //        }
    //        if(this.IsBinary )
    //        {
    //            var bs = Encoding.Unicode.GetBytes(text);
    //            var w = new DCILWriter(new StringBuilder());
    //            w.Write("bytearray(");
    //            w.WriteHexs(bs);
    //            w.Write(")");
    //            this.RawILText = w.ToString();
    //        }
    //        else
    //        {
    //            this.RawILText = ToRawILText(text);
    //        }
    //    }
    //    //public DCILStringValue(DCILReader reader)
    //    //{
    //    //    int posBack = reader.Position;
    //    //    var strOperData = reader.ReadLine()?.Trim();
    //    //    if (strOperData[0] == '"')
    //    //    {
    //    //        int startPos = posBack;
    //    //        this.IsBinary = false;
    //    //        if (strOperData.Length == 2 && strOperData[1] == '"')
    //    //        {
    //    //            this.Value = string.Empty;
    //    //        }
    //    //        var strFinalValue = new StringBuilder();
    //    //        GetFinalValue(strOperData, strFinalValue);
    //    //        while (reader.HasContentLeft())
    //    //        {
    //    //            posBack = reader.Position;
    //    //            var line2 = reader.ReadLine().Trim();
    //    //            if (line2.Length > 0 && line2[0] == '+')
    //    //            {
    //    //                //line2 = RemoveComment(line2).Trim();
    //    //                //strOperData = strOperData + Environment.NewLine + line2;
    //    //                GetFinalValue(line2, strFinalValue);
    //    //            }
    //    //            else
    //    //            {
    //    //                reader.Position = posBack;
    //    //                break;
    //    //            }
    //    //        }
    //    //        this.RawILText = reader.GetSubStringUseTable(startPos, reader.Position - startPos, true);
    //    //        this.Value = strFinalValue.ToString();
    //    //        if(this.Value.Length < 10 )
    //    //        {
    //    //            this.Value = DCUtils.GetStringUseTable(this.Value);
    //    //        }
    //    //    }
    //    //    else if (strOperData.StartsWith("bytearray", StringComparison.Ordinal))
    //    //    {
    //    //        this.IsBinary = true;
    //    //        reader.Position = posBack;
    //    //        reader.ReadToChar('(');
    //    //        var bs = reader.ReadBinaryFromHex();
    //    //        this.RawILText = reader.GetSubStringUseTable(posBack, reader.Position - posBack, true);
    //    //        if (bs != null && bs.Length > 0)
    //    //        {
    //    //            this.Value = Encoding.Unicode.GetString(bs);
    //    //            if (this.Value.Length < 10)
    //    //            {
    //    //                this.Value = DCUtils.GetStringUseTable(this.Value);
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            this.Value = string.Empty;
    //    //        }

    //    //    }
    //    //}
    //    public static string ToRawILText(string text)
    //    {
    //        if (text == null)
    //        {
    //            throw new ArgumentNullException("text");
    //        }
    //        if (text.Length == 0)
    //        {
    //            return "\"\"";
    //        }
    //        var result = new System.Text.StringBuilder();
    //        result.Append('"');
    //        foreach (var c in text)
    //        {
    //            switch (c)
    //            {
    //                case '\r': result.Append(@"\r"); break;
    //                case '\n': result.Append(@"\n"); break;
    //                case '\'': result.Append(@"\'"); break;
    //                case '\\': result.Append(@"\\"); break;
    //                case '"': result.Append("\\\""); break;
    //                case '\b': result.Append(@"\b"); break;
    //                case '\f': result.Append(@"\f"); break;
    //                case '\t': result.Append(@"\t"); break;
    //                default: result.Append(c); break;
    //            }
    //        }
    //        result.Append('"');
    //        return result.ToString();
    //    }


    //    //private void GetFinalValue(string line, StringBuilder result)
    //    //{
    //    //    int index = line.IndexOf('"');
    //    //    int len = line.Length;
    //    //    for (int iCount = line.IndexOf('"') + 1; iCount < len; iCount++)
    //    //    {
    //    //        var c = line[iCount];
    //    //        if (c == '\\' && iCount < len - 1)
    //    //        {
    //    //            var nc = line[iCount + 1];
    //    //            iCount++;
    //    //            switch (nc)
    //    //            {
    //    //                case 'r': result.Append('\r'); break;
    //    //                case 'n': result.Append('\n'); break;
    //    //                case '\'': result.Append('\''); break;
    //    //                case '"': result.Append('"'); break;
    //    //                case '\\': result.Append('\\'); break;
    //    //                case 'b': result.Append('\b'); break;
    //    //                case 'f': result.Append('\f'); break;
    //    //                case 't': result.Append('\t'); break;
    //    //                default: result.Append(nc); break;
    //    //            }
    //    //        }
    //    //        else if (c == '"')
    //    //        {
    //    //            break;
    //    //        }
    //    //        else
    //    //        {
    //    //            result.Append(c);
    //    //        }
    //    //    }
    //    //}
    //    public readonly string Value = null;
    //    public readonly bool IsBinary = false;
    //    public readonly string RawILText = null;
    //}
    internal class DCILOperCode_LoadString : DCILOperCode
    {
        public DCILOperCode_LoadString(string labelID, DCILReader reader)
        {
            this.LabelID = labelID;
            this._Define = DCILOperCodeDefine._ldstr;
            var info = reader.ReadStringValue();
            this.Value = info.Value;
            this.IsBinary = info.IsBinary;
            this.OperData = info.ILRawText;
            this.BianryData = info.BianryData;
        }

        public DCILOperCode_LoadString( string labelID , string text )
        {
            this.LabelID = labelID;
            this._Define = DCILOperCodeDefine._ldstr;
            this.OperData = DCILReader.ToRawILText(text);
            this.IsBinary = this.OperData != null && this.OperData.StartsWith(DCILReader._bytearray, StringComparison.Ordinal);
            this.Value = text;

            //var v = new DCILStringValue(text);
            //this.IsBinary = v.IsBinary;
            //this.Value = v.Value;
            //this.OperData = v.RawILText;
        }

        //public DCILOperCode_LoadString(DCILOperCode code)
        //{
        //    this._Define = DCILOperCodeDefine._ldstr;
        //    this.LabelID = code.LabelID;
        //    this.OperData = code.OperData;
        //    //this.LineIndex = code.LineIndex;
        //    //this.EndLineIndex = code.EndLineIndex;
        //    //this.NativeSource = code.NativeSource;
        //    //this.OwnerList = code.OwnerList;
        //    //this.OwnerMethod = code.OwnerMethod;
        //}
        public override void Dispose()
        {
            base.Dispose();
            this.Value = null;
            this.ReplaceCode = null;
            this.BianryData = null;
        }
        /// <summary>
        /// 字符串值
        /// </summary>
        public string Value = null;
        /// <summary>
        /// 是否采用二进制格式来定义
        /// </summary>
        public bool IsBinary = false;
        /// <summary>
        /// 二进制数据
        /// </summary>
        public byte[] BianryData = null;
        /// <summary>
        /// 是否为了设置静态字段
        /// </summary>
        public bool IsSetStaticField = false;
        /// <summary>
        /// 替换的指令
        /// </summary>
        public DCILOperCode ReplaceCode = null;
        public override void WriteTo(DCILWriter writer)
        {
            if (this.ReplaceCode != null)
            {
                this.ReplaceCode.LabelID = this.LabelID;
                this.ReplaceCode.WriteTo(writer);
            }
            else
            {
                base.WriteTo(writer);
            }
        }
        public override void WriteOperData(DCILWriter writer)
        {
            writer.Write(' ');
            if( this.IsBinary )
            {
                writer.Write(DCILReader._bytearray);
                writer.Write('(');
                writer.WriteHexs(this.BianryData);
                writer.WriteLine(")");
            }
            else
            {
                writer.Write(this.OperData);
            }
        }
    }
    internal class DCILField : DCILMemberInfo
    {
        private static readonly HashSet<string> _FieldAttributes = null;
        static DCILField()
        {
            _FieldAttributes = new HashSet<string>();
            _FieldAttributes.Add("assembly");
            _FieldAttributes.Add("famandassem");
            _FieldAttributes.Add("family");
            _FieldAttributes.Add("famorassem");
            _FieldAttributes.Add("initonly");
            _FieldAttributes.Add("literal");
            //_FieldAttributes.Add("marshal ‘(’ NativeType ‘)’");
            _FieldAttributes.Add("notserialized");
            _FieldAttributes.Add("private");
            _FieldAttributes.Add("compilercontrolled");
            _FieldAttributes.Add("public");
            _FieldAttributes.Add("rtspecialname");
            _FieldAttributes.Add("specialname");
            _FieldAttributes.Add("static");
            _FieldAttributes.Add("instance");
        }

        public const string TagName = ".field";
        public DCILField()
        {

        }
        public DCILField(DCILClass parent, DCILReader reader)
        {
            this.Load(reader);
            this.Parent = parent;
        }
        public override void Dispose()
        {
            base.Dispose();
            this.MarshalAs = null;
            this.modopt = null;
            this.modreq = null;
            //this.OldSignature = null;
            this.ReferenceData = null;
            this.ValueType = null;
            this.ConstValue = null;
            //this.DataLabel = null;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Field;
            }
        }
        public DCILTypeReference ValueType = null;

        public int InnerTag = 0;

        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            this.ValueType = document.CacheTypeReference(this.ValueType);
            base.CusotmAttributesCacheTypeReference(document);
        }
        public bool IsConst
        {
            get
            {
                return base.HasStyle("literal");
            }
        }
        public string ConstValue = null;
        //public string DataLabel = null;
        public DCILData ReferenceData = null;

        public string MarshalAs = null;
        public string modreq = null;
        public string modopt = null;
        //public List<System.Tuple<string,string>> ExtStyles = null;
        public override void Load(DCILReader reader)
        {
            if (reader.PeekContentChar() == '[')
            {
                this.SpecifyIndex = reader.ReadArrayIndex();
            }
            while (reader.HasContentLeft())
            {
                int pos = reader.Position;
                var strWord = reader.ReadWord();
                if (_FieldAttributes.Contains(strWord))
                {
                    this.AddStyle(strWord);
                }
                else if (strWord == "marshal")
                {
                    this.MarshalAs = reader.ReadStyleExtValue();
                    continue;
                }
                else if (DCILTypeReference.IsStartWord(strWord))
                {
                    this.ValueType = DCILTypeReference.Load(strWord, reader);
                    strWord = reader.ReadWord();
                    if (strWord == "modreq")
                    {
                        this.modreq = reader.ReadStyleExtValue();
                        strWord = reader.ReadWord();
                    }
                    else if (strWord == "modopt")
                    {
                        this.modopt = reader.ReadStyleExtValue();
                        strWord = reader.ReadWord();
                    }
                    this._Name = strWord;
                    if (reader.HasContentLeftCurrentLine() == false)
                    {
                        break;
                    }
                }
                else if (strWord == "at")
                {
                    var strDataLabel = reader.ReadWord();
                    reader.AddReferenceDataLabel(this, strDataLabel);
                    break;
                }
                else if (strWord == "=")
                {
                    if (this.ValueType == DCILTypeReference.Type_String)
                    {
                        var info = reader.ReadStringValue();
                        this.ConstValue = info.ILRawText;
                        //string text2 = reader.ReadStringValue(ref this.ConstValue);

                        //this.ConstValue = reader.ReadStringValue(ref rawILText);

                        //var v = new DCILStringValue(reader);
                        //this.ConstValue = DCUtils.GetStringUseTable( v.RawILText );
                    }
                    else
                    {
                        this.ConstValue = DCUtils.GetStringUseTable( reader.ReadLineTrimRemoveComment() );
                    }
                    break;
                }
                else if (strWord[0] == '.')
                {
                    reader.Position = pos;
                    break;
                }
            }

            while (reader.HasContentLeft())
            {
                int pos = reader.Position;
                var word = reader.ReadWord();
                if (word == DCILCustomAttribute.TagName_custom)
                {
                    base.ReadCustomAttribute(reader);
                }
                else
                {
                    reader.Position = pos;
                    break;
                }
            }
        }
        /// <summary>
        /// 旧的签名信息
        /// </summary>
        //public string OldSignature = null;
        //public void UpdateOldSignature()
        //{
        //    var writer = new DCILWriter(new StringBuilder());
        //    this.ValueType.WriteToForSignString(writer);
        //    this.OldSignature = writer.ToString();
        //}

        public override void WriteTo(DCILWriter writer)
        {
            if(this.RenameState == DCILRenameState.Renamed )
            {
                writer.WriteLine("// " + ((DCILClass)this.Parent).GetOldName() + "::" + this.GetOldName());
            }
            writer.Write(".field ");
            if(this.SpecifyIndex != int.MinValue)
            {
                writer.Write(" [" + this.SpecifyIndex + "] ");
            }
            base.WriteStyles(writer);
            if (this.MarshalAs != null && this.MarshalAs.Length > 0)
            {
                writer.Write(" marshal(" + this.MarshalAs + ")");
            }
            this.ValueType.WriteTo(writer);
            if (this.modopt != null && this.modopt.Length > 0)
            {
                writer.Write(" modopt(" + this.modopt + ")");
            }
            if (this.modreq != null && this.modreq.Length > 0)
            {
                writer.Write(" modreq(" + this.modreq + ")");
            }
            writer.Write(" " + this._Name);
            if (this.ConstValue != null && this.ConstValue.Length > 0)
            {
                writer.Write(" = ");
                writer.Write(this.ConstValue);
            }
            else if (this.ReferenceData != null)
            {
                writer.Write(" at ");
                writer.Write(this.ReferenceData.Name);
            }
            writer.WriteLine();
            base.WriteCustomAttributes(writer);
        }

        public int SpecifyIndex = int.MinValue;

        public override string ToString()
        {
            return "field " + this.ValueType + " " + this._Name;
        }
    }
    internal class DCILGenericParamterList : List<DCILGenericParamter> , IDisposable
    {
        public static DCILGenericParamterList Merge(DCILGenericParamterList list1, DCILGenericParamterList list2)
        {
            if (list1 == null)
            {
                if (list2 == null)
                {
                    return null;
                }
                else
                {
                    return list2;
                }
            }
            else
            {
                if (list2 == null)
                {
                    return list1;
                }
                else
                {
                    var result = new DCILGenericParamterList(list1.Count + list2.Count);
                    result.AddRange(list1);
                    result.AddRange(list2);
                    return result;
                }
            }
        }

        public static DCILGenericParamterList CreateByNativeType(Type t)
        {
            if (t == null || t.IsGenericType == false)
            {
                return null;
            }
            var gs = t.GetGenericArguments();
            if (gs == null || gs.Length == 0)
            {
                return null;
            }
            var list = new DCILGenericParamterList(gs.Length);
            foreach (var item in gs)
            {
                list.Add(new DCILGenericParamter(item.Name, true));
            }
            list.ResetIndex();
            return list;
        }

        public DCILGenericParamterList(int len) : base(len)
        {

        }

        public DCILGenericParamterList(DCILReader reader, bool defineInClass)
        {
            DCILGenericParamter cgp = new DCILGenericParamter();
            this.Add(cgp);
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == null || strWord == ">")
                {
                    break;
                }
                else if (strWord == "valuetype"
                    || strWord == "class"
                    || strWord == ".ctor"
                    || strWord == "'+'" || strWord == "'-'")
                {
                    if (cgp.Attributes == null)
                    {
                        cgp.Attributes = new List<string>();
                    }
                    cgp.Attributes.Add(strWord);
                    continue;
                }
                else if (strWord == "(")
                {
                    // 约束
                    var list9 = new List<DCILTypeReference>();
                    while (reader.SkipWhitespace())
                    {
                        if (reader.Peek() == ',')
                        {
                            cgp = new DCILGenericParamter();
                            this.Add(cgp);
                            reader.Read();
                        }
                        else if (reader.Peek() == ')')
                        {
                            reader.Read();
                            break;
                        }
                        else
                        {
                            var item9 = DCILTypeReference.Load(reader);
                            if (item9 != null)
                            {
                                list9.Add(item9);
                            }
                        }
                    }//while
                    if (list9.Count > 0)
                    {
                        cgp.Constraints = list9.ToArray();
                    }
                    continue;
                }
                else if (strWord == ",")
                {
                    cgp = new DCILGenericParamter();
                    this.Add(cgp);
                }
                else
                {
                    cgp.Name = strWord;
                }
            }
            for (int iCount = this.Count - 1; iCount >= 0; iCount--)
            {
                this[iCount].DefineInClass = defineInClass;
                this[iCount].Index = iCount;
            }
        }
        public void Dispose()
        {
            foreach( var item in this )
            {
                item.Dispose();
            }
            this.Clear();
        }
        public void ClearRuntimeType()
        {
            foreach (var item in this)
            {
                item.RuntimeType = null;
            }
        }
        public void ResetIndex()
        {
            for (int iCount = this.Count - 1; iCount >= 0; iCount--)
            {
                this[iCount].Index = iCount;
            }
        }
        public void SetRuntimeType(List<DCILTypeReference> ts)
        {
            if (this.Count > 0 && ts != null && ts.Count != this.Count)
            {

            }
            if (ts != null && ts.Count == this.Count)
            {
                for (int iCount = 0; iCount < this.Count; iCount++)
                {
                    this[iCount].RuntimeType = ts[iCount];
                }
            }
        }
        public DCILGenericParamter GetItem(string name, bool defineInClass)
        {
            if (name == null || name.Length == 0)
            {
                return null;
            }
            if (char.IsNumber(name[0]))
            {
                int index = int.Parse(name);
                if (index >= 0)
                {
                    foreach (var item in this)
                    {
                        if (item.DefineInClass == defineInClass && item.Index == index)
                        {
                            return item;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in this)
                {
                    if (item.DefineInClass == defineInClass && item.Name == name)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public void WriteTo(DCILWriter writer)
        {
            if (this.Count > 0)
            {
                //writer.WriteLine();
                writer.ChangeIndentLevel(2);
                writer.Write('<');
                for (int iCount = 0; iCount < this.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(',');
                    }
                    var item = this[iCount];
                    if (item.Attributes != null && item.Attributes.Count > 0)
                    {
                        foreach (var attr in item.Attributes)
                        {
                            writer.Write(attr + " ");
                        }
                    }
                    if (item.Constraints != null && item.Constraints.Length > 0)
                    {
                        writer.Write('(');
                        for (int iCount2 = 0; iCount2 < item.Constraints.Length; iCount2++)
                        {
                            var item2 = item.Constraints[iCount2];
                            if (iCount2 > 0)
                            {
                                writer.Write(',');
                            }
                            item2.WriteTo(writer);
                        }
                        writer.Write(')');
                    }
                    writer.Write(item.Name);
                }
                writer.Write('>');
                writer.ChangeIndentLevel(-2);
            }
        }
    }

    /// <summary>
    /// see "Partition II Metadata.doc",topic 10.1.7	Generic parameters (GenPars).
    /// </summary>
    internal class DCILGenericParamter : IEqualsValue<DCILGenericParamter> , IDisposable
    {
        public DCILGenericParamter()
        {

        }
        public DCILGenericParamter(string name, bool defineInClass)
        {
            this.Name = name;
            this.DefineInClass = defineInClass;
        }
        public void Dispose()
        {
            this.Name = null;
            if( this.Attributes != null )
            {
                this.Attributes.Clear();
                this.Attributes = null;
            }
            this.RuntimeType = null;
            this.Constraints = null;
            this.RuntimeType = null;
        }
        private static int _InstanceIndexCount = 0;
        public int InstanceIndex = _InstanceIndexCount++;
        public int Index = -1;
        public bool DefineInClass = false;
        public DCILTypeReference RuntimeType = null;

        public string Name = null;
        public List<string> Attributes = null;
        public DCILTypeReference[] Constraints = null;
        public static bool MatchList(List<DCILGenericParamter> ps1, List<DCILTypeReference> ps2)
        {
            int len1 = ps1 == null ? 0 : ps1.Count;
            int len2 = ps2 == null ? 0 : ps2.Count;
            if (len1 != len2)
            {
                return false;
            }
            return true;
        }
        public bool EqualsValue(DCILGenericParamter p)
        {
            if (p == null)
            {
                return false;
            }

            if (this == p)
            {
                return true;
            }
            int len1 = this.Constraints == null ? 0 : this.Constraints.Length;
            int len2 = p.Constraints == null ? 0 : p.Constraints.Length;
            if (len1 != len2)
            {
                return false;
            }
            if (len1 > 0)
            {
                for (int iCount = 0; iCount < len1; iCount++)
                {
                    if (this.Constraints[iCount].EqualsValue(p.Constraints[iCount]) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void CacheTypeReference(DCILDocument document, List<DCILGenericParamter> ps)
        {
            if (ps != null)
            {
                foreach (var item in ps)
                {
                    if (item.Constraints != null)
                    {
                        for (int iCount = 0; iCount < item.Constraints.Length; iCount++)
                        {
                            item.Constraints[iCount] = document.CacheTypeReference(item.Constraints[iCount]);
                        }
                    }
                }
            }
        }


    }
    /// <summary>
    /// See "Partition II Metadata.doc",topic "10.Defining types"
    /// </summary>
    internal class DCILClass : DCILMemberInfo
    {

        private static readonly HashSet<string> _ClassAttributeNames = null;
        static DCILClass()
        {
            _ClassAttributeNames = new HashSet<string>();
            _ClassAttributeNames.Add("abstract"); //Type is abstract.
            _ClassAttributeNames.Add("ansi"); //Marshal strings to platform as ANSI.
            _ClassAttributeNames.Add("auto"); //Layout of fields is provided automatically.
            _ClassAttributeNames.Add("autochar"); //Marshal strings to platform as ANSI or Unicode (platform-specific).
            _ClassAttributeNames.Add("beforefieldinit"); //Need not initialize the type before a static method is called.
            _ClassAttributeNames.Add("explicit"); //Layout of fields is provided explicitly.
            _ClassAttributeNames.Add("interface"); //Declares an interface.
            //_ClassAttributeNames.Add("nested assembly"); //Assembly accessibility for nested type.
            //_ClassAttributeNames.Add("nested famandassem"); //Family and assembly accessibility for nested type.
            //_ClassAttributeNames.Add("nested family"); //Family accessibility for nested type.
            //_ClassAttributeNames.Add("nested famorassem"); //Family or assembly accessibility for nested type.
            //_ClassAttributeNames.Add("nested private"); //Private accessibility for nested type.
            //_ClassAttributeNames.Add("nested public"); //Public accessibility for nested type.
            _ClassAttributeNames.Add("private"); //Private visibility of top-level type.
            _ClassAttributeNames.Add("public"); //Public visibility of top-level type.
            _ClassAttributeNames.Add("rtspecialname"); //Special treatment by runtime.
            _ClassAttributeNames.Add("sealed"); //The type cannot be derived from.
            _ClassAttributeNames.Add("sequential"); //Layout of fields is sequential.
            _ClassAttributeNames.Add("serializable"); //Reserved (to indicate this type can be serialized).
            _ClassAttributeNames.Add("specialname"); //Might get special treatment by tools.
            _ClassAttributeNames.Add("unicode"); //Marshal strings to platform as Unicode.
            _ClassAttributeNames.Add("import");
        }
        public const string TagName = ".class";

        public static readonly DCILClass Empty = new DCILClass();

        public DCILClass()
        {
            base.ChildNodes = new DCILObjectList();
        }

        private DCILTypeReference _LocalTypeRef = null;
        public DCILTypeReference GetLocalTypeReference()
        {
            if(this._LocalTypeRef == null )
            {
                this._LocalTypeRef = new DCILTypeReference(this);
            }
            return this._LocalTypeRef;
        }
        /// <summary>
        /// 运行时使用的开关
        /// </summary>
        public JieJieSwitchs RuntimeSwitchs = null;
        public override void Dispose()
        {
            base.Dispose();
            if( this.NestedClasses != null )
            {
                foreach( var item in this.NestedClasses)
                {
                    item.Dispose();
                }
                this.NestedClasses.Clear();
                this.NestedClasses = null;
            }
            if(this.GenericParamters != null )
            {
                foreach( var item in this.GenericParamters )
                {
                    item.Dispose(); 
                }
            }
            this.Method_Cctor = null;
            this.NativeType = null;
            this.RuntimeSwitchs = null;
            if(this.ImplementsInterfaces != null )
            {
                this.ImplementsInterfaces.Clear();
                this.ImplementsInterfaces = null;
            }
            if( this.FieldLineIndexs != null )
            {
                this.FieldLineIndexs.Clear();
                this.FieldLineIndexs = null;
            }
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Class;
            }
        }

        /// <summary>
        /// 系统内部自动产生的
        /// </summary>
        public bool InnerGenerate = false;
        /// <summary>
        /// 存在系统内部自动生成的方法
        /// </summary>
        public bool HasInnerGenerateMethod = false;

        private static readonly Dictionary<Type, DCILClass> _NativeClasses = new Dictionary<Type, DCILClass>();

        //public DCILClass(Type nativeType, DCILDocument document) : base(nativeType)
        //{
        //    this.IsEnum = nativeType.IsEnum;
        //    this.IsValueType = nativeType.IsValueType;
        //    this.IsMulticastDelegate = typeof(System.MulticastDelegate).IsAssignableFrom(nativeType);
        //    this.IsInterface = nativeType.IsInterface;
        //    this.IsImport = nativeType.IsImport;
        //    this.ChildNodes = new DCILObjectList();
        //    this.NativeType = nativeType;
        //    this._Name = DCUtils.GetFullName(nativeType);
        //    if (nativeType.IsGenericType)
        //    {
        //        var gps = nativeType.GetGenericArguments();
        //        this.GenericParamters = new DCILGenericParamterList(gps.Length);
        //        foreach (var item in gps)
        //        {
        //            var p = new DCILGenericParamter();
        //            p.Name = DCUtils.GetFullName(item);
        //            this.GenericParamters.Add(p);
        //        }
        //    }
        //    if (nativeType.BaseType != null)
        //    {
        //        this.BaseType = DCILTypeReference.CreateByNativeType(nativeType.BaseType, document);
        //    }
        //    var its = nativeType.GetInterfaces();
        //    if (its != null && its.Length > 0)
        //    {
        //        this.ImplementsInterfaces = new List<DCILTypeReference>(its.Length);
        //        foreach (var item in its)
        //        {
        //            this.ImplementsInterfaces.Add(DCILTypeReference.CreateByNativeType(item, document));
        //        }
        //    }

        //}
        public Type NativeType = null;

        public DCILClass(string ilText, DCILDocument document)
        {
            if (ilText == null || ilText.Length == 0)
            {
                throw new ArgumentNullException("ilText");
            }
            //this.Parent = document;
            var reader = new DCILReader(ilText, document);
            if (reader.PeekWord() == ".class")
            {
                reader.ReadWord();
            }
            this.Load(reader);
            reader.UpdateFieldReferenceData(document);
            this.InnerGenerate = true;
        }

        /// <summary>
        /// 对象的静态构造函数 
        /// </summary>
        public DCILMethod Method_Cctor = null;

        public override void Load(DCILReader reader)
        {
            LoadHeader(reader);
            LoadContent(reader);
        }
        private static readonly DCILProperty[] _EmptyArray = new DCILProperty[0];

        private static readonly DCILMethod[] _EmptyArray2 = new DCILMethod[0];


        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            base.CacheInfo(document, clses);
            foreach (var item in this.ChildNodes)
            {
                if (item is DCILMemberInfo)
                {
                    ((DCILMemberInfo)item).CacheInfo(document, clses);
                }
            }
            if (this.BaseType != null)
            {
                this.BaseType = document.CacheTypeReference(this.BaseType);
            }
            if (this.ImplementsInterfaces != null)
            {
                for (int iCount = 0; iCount < this.ImplementsInterfaces.Count; iCount++)
                {
                    this.ImplementsInterfaces[iCount] = document.CacheTypeReference(this.ImplementsInterfaces[iCount]);
                }
            }
            DCILGenericParamter.CacheTypeReference(document, this.GenericParamters);

        }

        public void LoadHeader(DCILReader reader)
        {
            this.ChildNodes = new DCILObjectList();
            string strWord = null;
            while (reader.HasContentLeft())
            {
                strWord = reader.ReadWord();
                if (strWord == null)
                {
                    break;
                }
                if (strWord == "nested")
                {
                    this.AddStyle(strWord);
                    strWord = reader.ReadWord();
                    this.AddStyle(strWord);
                    continue;
                }
                else if (_ClassAttributeNames.Contains(strWord))
                {
                    this.AddStyle(strWord);
                    continue;
                }
                else if (strWord == "<")
                {
                    // 读取泛型参数
                    this.GenericParamters = new DCILGenericParamterList(reader, true);
                }
                else if (strWord == "extends")
                {
                    this.BaseType = DCILTypeReference.Load(reader);
                }
                else if (strWord == "implements")
                {
                    this.ImplementsInterfaces = new List<DCILTypeReference>();
                    while (reader.HasContentLeft())
                    {
                        var nc = reader.PeekContentChar();
                        if (nc == ',')
                        {
                            reader.ReadContentChar();
                            continue;
                        }
                        if (nc == '{' || nc == char.MinValue)
                        {
                            reader.ReadContentChar();
                            break;
                        }
                        this.ImplementsInterfaces.Add(DCILTypeReference.Load(reader));
                    }
                    break;
                }
                else
                {
                    if (this._Name == null)
                    {
                        this._Name = strWord;
                    }
                }
                if (strWord == null || strWord == "{")
                {
                    break;
                }
            }
            this.IsImport = base.HasStyle("import");
            this.IsInterface = base.HasStyle("interface");
            if (this.BaseType != null)
            {
                this.IsEnum = this.BaseType.Name == "System.Enum";
                this.IsValueType = this.BaseType.Name == "System.ValueType";
                this.IsMulticastDelegate = this.BaseType.Name == "System.MulticastDelegate";
            }
        }

        // public IDGenerator idGenForMember = null;
        public int StructLayoutPack = int.MinValue;
        public int StructLayoutSize = int.MinValue;

        public bool IsImport = false;
        public bool IsInterface = false;

        public bool HasGenericParamters
        {
            get
            {
                return this.GenericParamters != null && this.GenericParamters.Count > 0;
            }
        }

        public DCILGenericParamterList GenericParamters = null;
        public void LoadContent(DCILReader reader)
        {
            //reader.ReadAfterCharExcludeLastChar('{');
            DCInterfaceimpl itimpl = null;
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == "}")
                {
                    break;
                }
                switch (strWord)
                {
                    case DCInterfaceimpl.TagName:// ".interfaceimpl":
                        {
                            itimpl = new DCInterfaceimpl( reader );
                        }
                        break;
                    case DCILCustomAttribute.TagName_custom:
                        {
                            var item = base.ReadCustomAttribute(reader);
                            if(itimpl != null )
                            {
                                item.PrefixObject = itimpl;
                            }
                            itimpl = null;
                        }
                        break;
                    case DCILField.TagName:
                        {
                            var field = new DCILField(this, reader);
                            this.ChildNodes.Add(field);
                            reader.NumOfField++;
                        }
                        break;
                    case DCILMethod.TagName:
                        {
                            var m = new DCILMethod(this, reader);
                            this.ChildNodes.Add(m);
                            reader.NumOfMethod++;
                            if(m.Name == ".cctor")
                            {
                                this.Method_Cctor = m;
                            }
                        }
                        break;
                    case DCILProperty.TagName_property:
                        {
                            var p = new DCILProperty(this, reader);
                            this.ChildNodes.Add(p);
                            reader.NumOfProperty++;
                        }
                        break;
                    case DCILClass.TagName:
                        {
                            if (this.NestedClasses == null)
                            {
                                this.NestedClasses = new List<DCILClass>();
                            }
                            var cls = new DCILClass();
                            cls.LoadHeader(reader);
                            bool match = false;
                            foreach (var item in this.NestedClasses)
                            {
                                if (cls.Name == item.Name)
                                {
                                    item.LoadContent(reader);
                                    match = true;
                                    break;
                                }
                            }
                            if (match == false)
                            {
                                cls.Parent = this;
                                cls.LoadContent(reader);
                                this.NestedClasses.Add(cls);
                                reader.NumOfClass++;
                            }
                        }
                        break;
                    case DCILEvent.TagName:
                        var e2 = new DCILEvent(this, reader);
                        this.ChildNodes.Add(e2);
                        break;
                    case DCILData.TagName_Data:
                        var d3 = new DCILData(reader);
                        if (reader.Document != null)
                        {
                            reader.Document.ILDatas.Add(d3);
                        }
                        else
                        {
                            this.ChildNodes.Add(d3);
                        }
                        break;
                    case ".pack":
                        {
                            var strValue = reader.ReadWord();
                            reader.MoveNextLine();
                            if(Int32.TryParse(strValue, out this.StructLayoutPack)== false )
                            {
                                this.StructLayoutPack = int.MinValue;
                            }
                        }
                        break;
                    case ".size":
                        {
                            var strValue = reader.ReadWord();
                            reader.MoveNextLine();
                            if(Int32.TryParse(strValue, out this.StructLayoutSize) == false )
                            {
                                this.StructLayoutSize = int.MinValue;
                            }
                        }
                        break;
                    default:
                        this.ChildNodes.Add(new DCILUnknowObject(strWord, reader));
                        break;
                }
            }
            if (this.IsEnum)
            {
                foreach (var item in this.ChildNodes)
                {
                    if (item.Name == "value__")
                    {
                        var field = (DCILField)item;
                        this.EnumByteSize = GetIntegerByteSize(field.ValueType?.NativeType);
                        break;
                    }
                }
            }
        }
        internal static int GetIntegerByteSize(Type t)
        {
            if (t == typeof(byte) || t == typeof(sbyte))
            {
                return 1;
            }
            if (t == typeof(short) || t == typeof(ushort))
            {
                return 2;
            }
            if (t == typeof(int) || t == typeof(uint))
            {
                return 4;
            }
            if (t == typeof(long) || t == typeof(ulong))
            {
                return 8;
            }
            return 4;
        }

        public string GetNameWithNestedPlus(bool oldMode)
        {

            if (this.Parent == null)
            {
                if (oldMode && this.OldName != null)
                {
                    return this.OldName;
                }
                else
                {
                    return this._Name;
                }
            }
            else
            {
                var str = new StringBuilder();
                if (oldMode && this.OldName != null)
                {
                    str.Append(this.OldName);
                }
                else
                {
                    str.Append(this._Name);
                }
                var p = this.Parent as DCILClass;
                while (p != null)
                {
                    str.Insert(0, '+');
                    if (p.Name == null || p.Name.Length == 0)
                    {

                    }
                    if (oldMode && p.OldName != null)
                    {
                        str.Insert(0, p.OldName);
                    }
                    else
                    {
                        str.Insert(0, p.Name);
                    }
                    p = p.Parent as DCILClass;
                }
                return str.ToString();
            }
        }
        public string NameWithNested
        {
            get
            {
                return GetNameWithNested('/');
            }
        }
        
        private string GetRuntimeName(bool useOldName )
        {
            if (useOldName && this.OldName != null && this.OldName.Length > 0)
            {
                return this.OldName;
            }
            else
            {
                return this._Name;
            }
        }

        public string GetNameWithNested(char nestedSplitChar , bool useOldName = false )
        {
            if (this.Parent == null)
            {
                return this.GetRuntimeName(useOldName);
            }
            else
            {
                var str = new StringBuilder();
                str.Append(this.GetRuntimeName(useOldName));
                var p = this.Parent;
                while (p != null)
                {
                    str.Insert(0, nestedSplitChar);
                    string pName = ((DCILClass)p).GetRuntimeName(useOldName);
                    //if (pName == null || pName.Length == 0)
                    //{

                    //}
                    str.Insert(0, pName);
                    p = p.Parent;
                }
                return str.ToString();
            }

        }
        public override void WriteTo(DCILWriter writer)
        {
            InnerWriteTo(writer, false);
        }

        public void InnerWriteTo( DCILWriter writer , bool declearationOnly )
        { 
            if(this.RenameState == DCILRenameState.Renamed )
            {
                writer.WriteLine("// " + this.GetOldName());
            }
            writer.Write(".class ");
            base.WriteStyles(writer);
            //if (this._Name == "__DC20210205.__jiejienet_sm")
            //{

            //}
            writer.Write(" " + this._Name);
            this.GenericParamters?.WriteTo(writer);

            if (this.BaseType != null)
            {
                writer.Write(" extends ");
                this.BaseType.WriteTo(writer, true);
                //if (this.BaseType.HasLibraryName)
                //{
                //    this.BaseType.WriteTo(writer, true );
                //}
                //else
                //{
                //    this.BaseType.WriteTo(writer, false);
                //}
            }
            if (this.ImplementsInterfaces != null
                && this.ImplementsInterfaces.Count > 0)
            {
                writer.WriteLine();
                writer.Write("             implements ");
                for (int iCount = 0; iCount < this.ImplementsInterfaces.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(',');
                    }
                    this.ImplementsInterfaces[iCount].WriteTo(writer);
                }
            }
            writer.WriteStartGroup();
            if (declearationOnly == false)
            {
                base.WriteCustomAttributes(writer);
            }
            if (this.NestedClasses != null && this.NestedClasses.Count > 0)
            {
                foreach (var item in this.NestedClasses)
                {
                    writer.WriteLine();
                    item.InnerWriteTo(writer , declearationOnly);
                }
            }
            if (declearationOnly == false)
            {
                if( this.StructLayoutPack >= 0 )
                {
                    writer.WriteLine(Environment.NewLine + "   .pack " + this.StructLayoutPack);
                }
                if(this.StructLayoutSize >= 0 )
                {
                    writer.WriteLine(Environment.NewLine + "   .size " + this.StructLayoutSize);
                }
                writer.WriteObjects(this.ChildNodes);
            }
            writer.WriteEndGroup();
        }

        public List<DCILClass> NestedClasses = null;
        /// <summary>
        /// 获得指定名称的内嵌类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DCILClass GetNestedClass(string name )
        {
            if(this.NestedClasses != null )
            {
                foreach( var item in this.NestedClasses )
                {
                    if(item.Name == name )
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public List<DCILTypeReference> ImplementsInterfaces = null;
        public List<int> FieldLineIndexs = new List<int>();
        public bool Modified = false;
        public DCILTypeReference BaseType = null;
        public bool IsMulticastDelegate = false;
        public bool IsEnum = false;
        public bool IsValueType = false;

        /// <summary>
        /// 获得枚举值数据类型
        /// </summary>
        /// <returns></returns>
        public DCILTypeReference GetEnumItemValueType()
        {
            if (this.IsEnum)
            {
                foreach (var f in this.ChildNodes)
                {
                    if (f.Name == "value__")
                    {
                        return ((DCILField)f).ValueType;
                    }
                }
            }
            return null;
        }

        public int EnumByteSize = 4;
        public List<DCILTypeReference> GetBaseTypeAndImplementsInterfaces()
        {
            var list = new List<DCILTypeReference>();
            if (this.BaseType != null)
            {
                list.Add(this.BaseType);
            }
            if (this.ImplementsInterfaces != null && this.ImplementsInterfaces.Count > 0)
            {
                list.AddRange(this.ImplementsInterfaces);
            }
            return list;
        }
        private string _BaseTypeName = null;
        public string BaseTypeName
        {
            get
            {
                if (this._BaseTypeName == null && this.BaseType != null)
                {
                    this._BaseTypeName = this.BaseType.ToString();
                }
                return this._BaseTypeName;
            }
        }
        /// <summary>
        /// 是否为访问程序集资源的包装类型
        /// </summary>
        /// <returns></returns>
        public bool IsResoucePackage()
        {
            if (this.CustomAttributes != null
                && this.BaseType?.Name == "System.Object")
            {
                int flagCount = 0;
                foreach (var item in this.CustomAttributes)
                {
                    var tn = item.TypeName;
                    if (tn != null && (tn.Contains("GeneratedCodeAttribute")
                        || tn.Contains("DebuggerNonUserCodeAttribute")
                        || tn.Contains("CompilerGeneratedAttribute")))
                    {
                        flagCount++;
                    }
                }
                foreach (var item in this.ChildNodes)
                {
                    if (item is DCILProperty && item.Name == "ResourceManager")
                    {
                        flagCount++;
                    }
                }
                return flagCount == 4;
            }
            return false;
        }

        public override string ToString()
        {
            if (this.IsInterface)
            {
                return "Interface " + this.NameWithNested;
            }
            else if (this.BaseTypeName == null || this.BaseTypeName.Contains("System.Object"))
            {
                return "Class " + this.NameWithNested;
            }
            else
            {
                return "Class " + this.NameWithNested + " : " + this.BaseTypeName;
            }
        }
    }

    internal class DCILEditorAttribute : DCILCustomAttribute
    {
        public const string ConstAttributeTypeName = "System.ComponentModel.EditorAttribute";
        public DCILEditorAttribute()
        {
        }
        public override void ParseValues(ReadCustomAttributeValueArgs args)
        {
            base.ParseValues(args);
            DCILTypeConverterAttribute.FixTypeName(this, args);
        }
        public override void WriteTo(DCILWriter writer)
        {
            base.WriteTo(writer);
        }
        public override bool UpdateBinaryValueForLocalClassRename()
        {
            if(this.Parent.Name == "SQLTextForHeaderLabel")
            {

            }
            return base.UpdateBinaryValueForLocalClassRename();
        }
    }
    internal class DCILTypeConverterAttribute : DCILCustomAttribute
    {
        public const string ConstAttributeTypeName = "System.ComponentModel.TypeConverterAttribute";

        public DCILTypeConverterAttribute()
        {
        }
        public override void ParseValues(ReadCustomAttributeValueArgs args)
        {
            base.ParseValues(args);
            FixTypeName(this, args);
        }

        internal static void FixTypeName(DCILCustomAttribute attr, ReadCustomAttributeValueArgs args)
        {
            if (attr.Values != null)
            {
                foreach (var item in attr.Values)
                {
                    if (item.Value is string)
                    {
                        var str = (string)item.Value;
                        item.Value = new DCILCustomAttributeValue.TypeRefInfo(str, args);
                    }
                }
            }
        }

        //public string ConvertTypeName
        //{
        //    get
        //    {
        //        if (this.Values != null && this.Values.Length == 1)
        //        {
        //            return this.Values[0].Value?.ToString();
        //        }
        //        return null;
        //    }
        //}

    }
    internal class DCILObfuscationAttribute : DCILCustomAttribute
    {
        public const string ConstAttributeTypeName = "System.Reflection.ObfuscationAttribute";
        public DCILObfuscationAttribute(DCILInvokeMethodInfo myInvokeInfo, byte[] bsValue)
        {
            this.InvokeInfo = myInvokeInfo;
            this.BinaryValue = bsValue;
        }

        public override void ParseValues(ReadCustomAttributeValueArgs args)
        {
            base.ParseValues(args);
            if (this.Values != null)
            {
                foreach (var item in this.Values)
                {
                    switch (item.Name)
                    {
                        case "StripAfterObfuscation": this.StripAfterObfuscation = (bool)item.Value; break;
                        case "Exclude": this.Exclude = (bool)item.Value; break;
                        case "ApplyToMembers": this.ApplyToMembers = (bool)item.Value; break;
                        case "Feature": this.Feature = (string)item.Value; break;
                    }
                }
            }
        }
        public bool StripAfterObfuscation = true;
        public bool Exclude = true;
        public bool ApplyToMembers = true;
        public string Feature = "all";
        public override void Dispose()
        {
            base.Dispose();
            this.Feature = null;
        }
        /// <summary>
        /// 不执行任何操作
        /// </summary>
        /// <returns></returns>
        public override bool UpdateBinaryValueForLocalClassRename()
        {
            return false;
        }
    }

    internal class DCILCustomAttributeValue : IDisposable
    {
        public DCILCustomAttributeValue()
        {
        }
        public void Dispose()
        {
            if(this.EnumType != null )
            {
                this.EnumType.Dispose();
                this.EnumType = null;
            }
            this.Name = null;
            if(this.Value is IDisposable)
            {
                ((IDisposable)this.Value).Dispose();
            }
        }
        public bool IsBoxed = false;
        public TypeRefInfo EnumType = null;
        /// <summary>
        /// 是否为构造函数使用的参数
        /// </summary>
        public bool IsCtorParamter = false;
        public byte Flag = 0;
        public DCILElementType ElementType = DCILElementType.None;
        public string Name = null;
        public object Value = null;
        public override string ToString()
        {
            return this.Name + "=" + this.Value;
        }

        //public List<AttributeValue> AttriubteValues = null;
        public static byte[] GetBinaryValue(DCILCustomAttributeValue[] values, DCILInvokeMethodInfo invokeInfo)
        {
            var ms = new System.IO.MemoryStream();
            var writer = new System.IO.BinaryWriter(ms);
            writer.Write((short)1);
            int psCount = invokeInfo.Paramters == null ? 0 : invokeInfo.Paramters.Count;
            if (psCount > 0)
            {
                for (int iCount = 0; iCount < psCount; iCount++)
                {
                    var item = values[iCount];
                    if (item.ElementType == DCILElementType.System_Type)
                    {
                        WriteUTF8String(writer, item.Value.ToString());
                    }
                    else if (item.ElementType == DCILElementType.Enum)
                    {
                        WriteUTF8String(writer, item.EnumType.ToString());
                        WriteAttributeValue(writer, item.ElementType, item.Value);
                    }
                    else
                    {
                        WriteAttributeValue(writer, item.ElementType, item.Value);
                    }
                }
            }
            if (psCount < values.Length)
            {
                writer.Write((short)(values.Length - psCount));
                for (int iCount = psCount; iCount < values.Length; iCount++)
                {
                    var item = values[iCount];
                    writer.Write(item.Flag);
                    writer.Write((byte)item.ElementType);
                    if (item.EnumType != null)
                    {
                        WriteUTF8String(writer, item.EnumType.ToString());
                    }
                    WriteUTF8String(writer, item.Name);
                    if (item.IsBoxed)
                    {
                        writer.Write((byte)DCILElementType.Boxed);
                    }
                    WriteAttributeValue(writer, item.ElementType, item.Value);
                }
            }
            else
            {
                writer.Write((short)0);
            }
            var result = ms.ToArray();
            ms.Close();
            return result;
        }

        public static List<DCILCustomAttributeValue> ParseValues(
            byte[] bsValue,
            DCILInvokeMethodInfo invokeInfo,
            ReadCustomAttributeValueArgs args)
        {
            if (bsValue == null || bsValue.Length == 0)
            {
                return null;
            }
            List<DCILCustomAttributeValue> result = new List<DCILCustomAttributeValue>();
            var reader = new System.IO.BinaryReader(new System.IO.MemoryStream(bsValue));
            if (reader.ReadInt16() != 1)
            {
                throw new InvalidOperationException();
            }
            args.Reader = reader;
            if (invokeInfo.Paramters != null && invokeInfo.Paramters.Count > 0)
            {
                // 有参数
                if (_TypeMaps == null)
                {
                    _TypeMaps = new Dictionary<Type, DCILElementType>();
                    _TypeMaps[typeof(string)] = DCILElementType.String;
                    _TypeMaps[typeof(bool)] = DCILElementType.Boolean;
                    _TypeMaps[typeof(sbyte)] = DCILElementType.I1;
                    _TypeMaps[typeof(byte)] = DCILElementType.U1;
                    _TypeMaps[typeof(short)] = DCILElementType.I2;
                    _TypeMaps[typeof(ushort)] = DCILElementType.U2;
                    _TypeMaps[typeof(int)] = DCILElementType.I4;
                    _TypeMaps[typeof(uint)] = DCILElementType.U4;
                    _TypeMaps[typeof(long)] = DCILElementType.I8;
                    _TypeMaps[typeof(ulong)] = DCILElementType.U8;
                    _TypeMaps[typeof(float)] = DCILElementType.R4;
                    _TypeMaps[typeof(double)] = DCILElementType.R8;
                    _TypeMaps[typeof(char)] = DCILElementType.Char;
                    _TypeMaps[typeof(object)] = DCILElementType.Object;
                }
                for (int iCount = 0; iCount < invokeInfo.Paramters.Count; iCount++)
                {
                    var av = new DCILCustomAttributeValue();
                    av.IsCtorParamter = true;
                    var p = invokeInfo.Paramters[iCount];
                    av.Name = p.ValueType.Name;
                    if (p.ValueType.Name == "System.Type")
                    {
                        av.Value = new TypeRefInfo(args);// ReadUTF8String(reader);
                        av.ElementType = DCILElementType.System_Type;
                    }
                    else
                    {
                        var vt = p.ValueType.NativeType;
                        DCILElementType et = DCILElementType.None;
                        if (vt != null)
                        {
                            _TypeMaps.TryGetValue(vt, out et);
                        }
                        if (et == DCILElementType.None
                            && p.ValueType.Mode == DCILTypeMode.ValueType)
                        {
                            et = DCILElementType.I4;
                        }
                        av.Value = ReadAttributeValue(args, et);
                        av.ElementType = et;
                    }
                    result.Add(av);
                }//for
            }
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // 无参数
                int num = reader.ReadUInt16();
                for (int iCount = 0; iCount < num; iCount++)
                {
                    var av = new DCILCustomAttributeValue();
                    av.IsCtorParamter = false;
                    av.Flag = reader.ReadByte();
                    av.ElementType = (DCILElementType)reader.ReadByte();
                    if (av.ElementType == DCILElementType.Enum)
                    {
                        av.EnumType = new TypeRefInfo(args);
                    }
                    av.Name = DCUtils.GetStringUseTable( ReadUTF8String(reader) );
                    if (av.Flag == 83)
                    {

                    }
                    else if (av.Flag == 84)
                    {

                    }
                    else
                    {

                    }
                    if (av.ElementType == DCILElementType.Boxed)
                    {
                        av.IsBoxed = true;
                        av.ElementType = (DCILElementType)reader.ReadByte();
                    }
                    av.Value = ReadAttributeValue(args, av.ElementType);
                    result.Add(av);
                }
            }
            reader.Close();
            args.Reader = null;
            return result;
        }

        private static Dictionary<Type, DCILElementType> _TypeMaps = null;
        protected static void WriteAttributeValue(BinaryWriter writer, DCILElementType type, object Value)
        {
            if (Value is TypeRefInfo)
            {
                ((TypeRefInfo)Value).WriteTo(writer);
                return;
            }
            switch (type)
            {
                case DCILElementType.Boolean:
                    {
                        if ((bool)Value)
                        {
                            writer.Write((byte)1);
                        }
                        else
                        {
                            writer.Write((byte)0);
                        }
                    }
                    break;
                case DCILElementType.I1: writer.Write((sbyte)Value); break;
                case DCILElementType.U1: writer.Write((byte)Value); break;
                case DCILElementType.Char: writer.Write((short)Value); break;
                case DCILElementType.I2: writer.Write((short)Value); break;
                case DCILElementType.U2: writer.Write((ushort)Value); break;
                case DCILElementType.I4: writer.Write((int)Value); break;
                case DCILElementType.U4: writer.Write((uint)Value); break;
                case DCILElementType.I8: writer.Write((long)Value); break;
                case DCILElementType.U8: writer.Write((ulong)Value); break;
                case DCILElementType.R4: writer.Write((float)Value); break;
                case DCILElementType.R8: writer.Write((double)Value); break;
                case DCILElementType.String: WriteUTF8String(writer, (string)Value); break;
                case DCILElementType.Type: WriteUTF8String(writer, (string)Value); break;
                case DCILElementType.Enum: writer.Write((int)Value); break;
                default:
                    throw new NotSupportedException(type.ToString());
            }
        }
        internal static bool IsPrimateType(DCILElementType type)
        {
            return type == DCILElementType.Boolean
                || type == DCILElementType.I1
                || type == DCILElementType.U1
                || type == DCILElementType.Char
                || type == DCILElementType.I2
                || type == DCILElementType.U2
                || type == DCILElementType.I4
                || type == DCILElementType.U4
                || type == DCILElementType.I8
                || type == DCILElementType.U8
                || type == DCILElementType.R4
                || type == DCILElementType.R8
                || type == DCILElementType.String
                || type == DCILElementType.Enum
                || type == DCILElementType.Type;
        }
        //internal static object ReadPrimativTypeValue( BinaryReader reader , DCILElementType type , out bool isPrimativeType )
        //{
        //    switch (type)
        //    {
        //        case DCILElementType.Boolean: isPrimativeType = true; return reader.ReadByte() == 1;
        //        case DCILElementType.I1: isPrimativeType = true; return (sbyte)reader.ReadByte();
        //        case DCILElementType.U1: isPrimativeType = true; return reader.ReadByte();
        //        case DCILElementType.Char: isPrimativeType = true; return (char)reader.ReadInt16();
        //        case DCILElementType.I2: isPrimativeType = true; return reader.ReadInt16();
        //        case DCILElementType.U2: isPrimativeType = true; return reader.ReadUInt16();
        //        case DCILElementType.I4: isPrimativeType = true; return reader.ReadInt32();
        //        case DCILElementType.U4: isPrimativeType = true; return reader.ReadUInt32();
        //        case DCILElementType.I8: isPrimativeType = true; return reader.ReadInt64();
        //        case DCILElementType.U8: isPrimativeType = true; return reader.ReadUInt64();
        //        case DCILElementType.R4: isPrimativeType = true; return reader.ReadSingle();
        //        case DCILElementType.R8: isPrimativeType = true; return reader.ReadDouble();
        //        case DCILElementType.String: isPrimativeType = true; return ReadUTF8String(reader);
        //        case DCILElementType.Type: isPrimativeType = true; return ReadUTF8String(reader);
        //        default:
        //            isPrimativeType = false;
        //            return null;
        //    }
        //}
        protected static object ReadAttributeValue(ReadCustomAttributeValueArgs args, DCILElementType type)
        {
            switch (type)
            {
                case DCILElementType.Boolean: return args.Reader.ReadByte() == 1;
                case DCILElementType.I1: return (sbyte)args.Reader.ReadByte();
                case DCILElementType.U1: return args.Reader.ReadByte();
                case DCILElementType.Char: return (char)args.Reader.ReadInt16();
                case DCILElementType.I2: return args.Reader.ReadInt16();
                case DCILElementType.U2: return args.Reader.ReadUInt16();
                case DCILElementType.I4: return args.Reader.ReadInt32();
                case DCILElementType.U4: return args.Reader.ReadUInt32();
                case DCILElementType.I8: return args.Reader.ReadInt64();
                case DCILElementType.U8: return args.Reader.ReadUInt64();
                case DCILElementType.R4: return args.Reader.ReadSingle();
                case DCILElementType.R8: return args.Reader.ReadDouble();
                case DCILElementType.String: return DCUtils.GetStringUseTable( ReadUTF8String(args.Reader));
                case DCILElementType.Enum:
                    return args.Reader.ReadInt32();
                case DCILElementType.Type:
                    return new TypeRefInfo(args);
                case DCILElementType.Object:
                    return new PackageValueInfo(args);
                default:
                    throw new NotSupportedException(type.ToString());
            }
            return null;
        }

        internal class PackageValueInfo : TypeRefInfo
        {
            public PackageValueInfo(ReadCustomAttributeValueArgs args)
            {
                this.ValueType = (DCILElementType)args.Reader.ReadByte();
                if (this.ValueType == DCILElementType.Enum)
                {
                    base.Parse(DCUtils.GetStringUseTable( ReadUTF8String(args.Reader)));
                    base.UpdateLocalInfo(args);
                    if (this.LocalClass != null)
                    {
                        this.EnumByteSize = this.LocalClass.EnumByteSize;
                    }
                    else
                    {
                        var bt = this.NativeType;
                        this.EnumByteSize = DCILClass.GetIntegerByteSize(bt);
                        if (bt == null)
                        {

                        }
                    }
                    this.BinValue = args.Reader.ReadBytes(this.EnumByteSize);
                    switch (this.EnumByteSize)
                    {
                        case 1: this.Value = this.BinValue[0]; break;
                        case 2: this.Value = BitConverter.ToInt16(this.BinValue, 0); break;
                        case 4: this.Value = BitConverter.ToInt32(this.BinValue, 0); break;
                        case 8: this.Value = BitConverter.ToInt64(this.BinValue, 0); break;
                    }
                }
                else if (IsPrimateType(this.ValueType))
                {
                    this.Value = ReadAttributeValue(args, this.ValueType);
                }
                else
                {
                    throw new NotSupportedException(this.ValueType.ToString());
                }
            }
            public int EnumByteSize = 4;
            public DCILElementType ValueType = DCILElementType.None;
            public object Value = null;
            public byte[] BinValue = null;
            public override void WriteTo(BinaryWriter writer)
            {
                if (this.TypeName != null && this.TypeName.Length > 0)
                {
                    writer.Write((byte)this.ValueType);
                    if (this.LocalClass != null)
                    {
                        WriteUTF8String(writer, this.LocalClass.GetNameWithNested('+'));
                    }
                    else
                    {
                        WriteUTF8String(writer, this.ToTypeString());
                    }
                    if (this.ValueType == DCILElementType.Enum)
                    {
                        writer.Write(this.BinValue, 0, this.BinValue.Length);
                    }
                }
                else if (IsPrimateType(this.ValueType))
                {
                    WriteAttributeValue(writer, this.ValueType, this.Value);
                }
                else
                {
                    throw new NotSupportedException(this.ValueType.ToString());
                }
            }
        }



        internal class TypeRefInfo : DCILTypeNameInfo
        {
            public TypeRefInfo()
            {

            }
            public TypeRefInfo(string typeName, ReadCustomAttributeValueArgs args)
            {
                base.Parse(typeName);
                UpdateLocalInfo(args);
            }

            public TypeRefInfo(ReadCustomAttributeValueArgs args)
            {
                var str = DCUtils.GetStringUseTable( ReadUTF8String(args.Reader));
                base.Parse(str);
                UpdateLocalInfo(args);
            }
            public override void Dispose()
            {
                base.Dispose();
                this.LocalClass = null;
                this.NativeType = null;
            }
            public virtual void WriteTo(BinaryWriter writer)
            {
                if (this.LocalClass != null)
                {
                    WriteUTF8String(writer, this.LocalClass.NameWithNested);
                }
                else
                {
                    WriteUTF8String(writer, this.ToTypeString());
                }
            }
            private bool SetLocalClass( DCILDocument doc )
            {
                this.LocalClass = doc.GetClassByName(this.TypeName, '+');
                return this.LocalClass != null;
                //if( this.TypeName.IndexOf('+') < 0 )
                //{
                //    return doc.GetAllClassesUseCache().TryGetValue(this.TypeName, out this.LocalClass);
                //}
                //else
                //{
                //    var tns = this.TypeName.Split('+');
                //    foreach( var cls in doc.Classes )
                //    {
                //        if( cls.Name == tns[0])
                //        {
                //            DCILClass curCls = cls;
                //            for(int iCount = 1; iCount < tns.Length; iCount ++)
                //            {
                //                if(curCls.NestedClasses == null )
                //                {
                //                    break;
                //                }
                //                foreach( var cls2 in curCls.NestedClasses)
                //                {
                //                    if( cls2.Name ==  tns[iCount])
                //                    {
                //                        if(iCount == tns.Length -1 )
                //                        {
                //                            this.LocalClass = cls2;
                //                            return true;
                //                        }
                //                        curCls = cls2;
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                //return false;
            }
            protected void UpdateLocalInfo(ReadCustomAttributeValueArgs args)
            {
                var doc = args.GetDocument(this.AssemblyName);
                if (doc != null)
                {
                    SetLocalClass(doc);
                    return;
                }
                //if (this.HasAssemblyName && args.Documents != null)
                //{
                //    foreach (var doc in args.Documents)
                //    {
                //        if (doc.Name == this.AssemblyName)
                //        {
                //            if(SetLocalClass( doc ))
                //            {
                //                return;
                //            }
                //            break;
                //        }
                //    }
                //}
                //else if (args.MainDocument != null)
                //{
                //    if(SetLocalClass(args.MainDocument ))
                //    {
                //        return;
                //    }
                //}
                var t2 = new DCILTypeReference(this.TypeName, DCILTypeMode.Unsigned);
                t2.LibraryName = this.AssemblyName;
                this.NativeType = t2.SearchNativeType(args.AssemblySeachPath);
            }

            public bool UpdateForLocalClassNameChanged()
            {
                bool result = false;
                if (this.HasAssemblyName)
                {
                    //if (this.AssemblyName.StartsWith("System."))
                    //{

                    //}
                    this.AssemblyName = null;
                    this.AssemblyCulture = null;
                    this.AssemblyPublicKeyToken = null;
                    this.AssemblyVersion = null;
                    result = true;
                }
                if (this.LocalClass.RenameState == DCILRenameState.Renamed)
                {
                    result = true;
                }
                return result;
            }

            public DCILClass LocalClass = null;
            public Type NativeType = null;
            public override string ToString()
            {
                if (this.LocalClass == null)
                {
                    return base.ToTypeString();
                }
                else
                {
                    return this.LocalClass.GetNameWithNested('+');
                }
            }
        }



        protected static void WriteUTF8String(System.IO.BinaryWriter writer, string text)
        {
            if (text == null)
            {
                writer.Write((byte)0xff);
                return;
            }
            if (text.IndexOf("DCSoft.Common,") >= 0)
            {

            }
            if (text.Length == 0)
            {
                writer.Write((byte)0);
                return;
            }
            var bsData = System.Text.Encoding.UTF8.GetBytes(text);
            var len = bsData.Length;
            if (len < 128)
            {
                writer.Write((byte)len);
            }
            else if (len < 16384)
            {
                writer.Write((byte)(0x80 | (len >> 8)));
                writer.Write((byte)(len & 0xFF));
            }
            else
            {
                writer.Write((byte)((len >> 24) | 0xC0));
                writer.Write((byte)((len >> 16) & 0xFF));
                writer.Write((byte)((len >> 8) & 0xFF));
                writer.Write((byte)(len & 0xFF));
            }
            writer.Write(bsData);
        }
        protected static string ReadUTF8String(System.IO.BinaryReader reader)
        {
            uint bsLength = 0;
            byte b = reader.ReadByte();
            if (b == 0xff)
            {
                return null;
            }
            if (b == 0)
            {
                return string.Empty;
            }
            if ((b & 0x80) == 0)
            {
                bsLength = b;
            }
            else if ((b & 0x40) == 0)
            {
                bsLength = (uint)(((b & -129) << 8) | reader.ReadByte());
            }
            else
            {
                bsLength = (uint)(((b & -193) << 24) | (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte());
            }
            if (bsLength == 0)
            {
                return null;
            }
            var bsTemp = reader.ReadBytes((int)bsLength);
            var str = System.Text.Encoding.UTF8.GetString(bsTemp);
            return str;
        }
        public enum DCILElementType : byte
        {
            None = 0,
            Void = 1,
            Boolean = 2,
            Char = 3,
            I1 = 4,
            U1 = 5,
            I2 = 6,
            U2 = 7,
            I4 = 8,
            U4 = 9,
            I8 = 10,
            U8 = 11,
            R4 = 12,
            R8 = 13,
            String = 14,
            Ptr = 0xF,
            ByRef = 0x10,
            ValueType = 17,
            Class = 18,
            Var = 19,
            Array = 20,
            GenericInst = 21,
            TypedByRef = 22,
            I = 24,
            U = 25,
            FnPtr = 27,
            Object = 28,
            SzArray = 29,
            MVar = 30,
            CModReqD = 0x1F,
            CModOpt = 0x20,
            Internal = 33,
            Modifier = 0x40,
            Sentinel = 65,
            Pinned = 69,
            Type = 80,
            Boxed = 81,
            Enum = 85,
            System_Type = 200
        }

    }

    internal class ReadCustomAttributeValueArgs
    {
        public ReadCustomAttributeValueArgs(List<DCILDocument> documents, DCILDocument mainDoc, string seachPath)
        {
            this.Documents = documents;
            this.MainDocument = mainDoc;
            this.AssemblySeachPath = seachPath;
        }
        public BinaryReader Reader = null;
        public string AssemblySeachPath = null;
        public List<DCILDocument> Documents = null;
        public DCILDocument MainDocument = null;

        public DCILDocument GetDocument(string assemblyName)
        {
            if (this.Documents != null
                && assemblyName != null
                && assemblyName.Length > 0 )
            {
                foreach (var doc in this.Documents)
                {
                    if (string.Compare(doc.Name, assemblyName, true) == 0)
                    {
                        return doc;
                    }
                    if (doc.AssemblyFileName != null
                        && string.Compare(Path.GetFileNameWithoutExtension(doc.AssemblyFileName), assemblyName, true) == 0)
                    {
                        return doc;
                    }
                }
            }
            return this.MainDocument;
        }
       

    }

    internal class DCInterfaceimpl : DCILObject
    {
        public const string TagName = ".interfaceimpl";

        public DCInterfaceimpl(DCILReader reader)
        {
            this.InterfaceType = reader.ReadWord();
            //string tn = reader.ReadWord();
            this.RefType = DCILTypeReference.Load(reader);
            if( this.RefType.Mode == DCILTypeMode.Class)
            {
                this.HasHeaderType = true;
            }
        }

        public void UpdateLocalInfo(ReadCustomAttributeValueArgs args)
        {
            //var doc = args.GetDocument(this.RefType.LibraryName);
            //if (doc != null)
            //{
            //    this.RefType.UpdateLocalClass(doc.GetAllClassesUseCache());
            //}
        }
        private bool HasHeaderType = false;

        public string InterfaceType = null;

        public DCILTypeReference RefType = null;
        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(TagName);
            writer.Write(" " + this.InterfaceType + " ");
            this.RefType.WriteTo(writer, this.HasHeaderType);
            writer.WriteLine();
        }
    }


    internal class DCILCustomAttribute : DCILObject
    {
        public static DCILCustomAttribute Create(DCILObject parent, DCILReader reader)
        {
            string preFix = null;
            if (reader.PeekContentChar() == '(')
            {
                preFix = reader.ReadAfterChar(')');
            }
            var invokeInfo = new DCILInvokeMethodInfo(reader);
            reader.MoveAfterChar('=');// reader.ReadAfterChar('=');
            var bsValue = reader.ReadBinaryFromHex();
            DCILCustomAttribute result = null;
            string typeName = invokeInfo?.OwnerType?.Name;
            switch (typeName)
            {
                case DCILObfuscationAttribute.ConstAttributeTypeName:
                    result = new DCILObfuscationAttribute(invokeInfo, bsValue);
                    break;
                case DCILTypeConverterAttribute.ConstAttributeTypeName:
                    result = new DCILTypeConverterAttribute();
                    break;
                case DCILEditorAttribute.ConstAttributeTypeName:
                    result = new DCILEditorAttribute();
                    break;
                default:
                    result = new DCILCustomAttribute();
                    break;
            }
            result.Prefix = preFix;
            result.Parent = parent;
            result.InvokeInfo = invokeInfo;
            result.BinaryValue = bsValue;
            result.AttributeTypeName = typeName;
            return result;
        }

        public const string TagName_custom = ".custom";

        public DCILCustomAttribute()
        {

        }

        public DCInterfaceimpl PrefixObject = null;

        public string Prefix = null;

        public override void Dispose()
        {
            this.AttributeTypeName = null;
            this.BinaryValue = null;
            if(this._Values != null )
            {
                foreach( var item in this._Values)
                {
                    item.Dispose();
                }
                this._Values = null;
            }
            this.HexValue = null;
            this.InvokeInfo = null;

            base.Dispose();
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.CustomAttribute;
            }
        }

        public string AttributeTypeName = null;

        public DCILInvokeMethodInfo InvokeInfo = null;

        public byte[] BinaryValue = null;

        private DCILCustomAttributeValue[] _Values = null;
        public virtual void ParseValues(ReadCustomAttributeValueArgs args)
        {
            var list = DCILCustomAttributeValue.ParseValues(this.BinaryValue, this.InvokeInfo, args);
            this.PrefixObject?.UpdateLocalInfo(args);
            if (list == null || list.Count == 0)
            {
                this._Values = null;
            }
            else
            {
                this._Values = list.ToArray();
            }
        }
        public virtual bool UpdateBinaryValueForLocalClassRename()
        {
            bool result = false;
            if (this._Values != null)
            {
                foreach (var item in this._Values)
                {
                    if (item.EnumType != null
                        && item.EnumType.LocalClass != null
                        && item.EnumType.UpdateForLocalClassNameChanged())
                    {
                        result = true;
                    }
                    if (item.Value is DCILCustomAttributeValue.TypeRefInfo)
                    {
                        var info = (DCILCustomAttributeValue.TypeRefInfo)item.Value;
                        if (info.LocalClass != null && info.UpdateForLocalClassNameChanged())
                        {
                            result = true;
                        }
                    }
                }
            }
            if (result)
            {
                var bs = this.BinaryValue;
                this.BinaryValue = DCILCustomAttributeValue.GetBinaryValue(this._Values, this.InvokeInfo);
            }
            return result;
        }

        public DCILCustomAttributeValue[] Values
        {
            get
            {
                return this._Values;
            }
        }
        ///// <summary>
        ///// 为重命名而更新属性值
        ///// </summary>
        ///// <param name="map"></param>
        //public virtual bool UpdateValuesForRename(RenameMapInfo map)
        //{
        //    if (this.AttributeTypeName == "System.ComponentModel.DefaultValueAttribute")
        //    {
        //        return false;
        //    }
        //    if (this.Values == null)
        //    {
        //        return false;
        //    }
        //    bool changed = false;
        //    foreach (var item in this.Values)
        //    {
        //        if (item.ElementType == DCILCustomAttributeValue.DCILElementType.System_Type)
        //        {
        //            var newName = map.GetNewClassName((string)item.Value);
        //            if (newName != null)
        //            {
        //                changed = true;
        //                item.Value = newName;
        //            }
        //        }
        //    }
        //    if (changed)
        //    {
        //        this.BinaryValue = DCILCustomAttributeValue.GetBinaryValue(this.Values, this.InvokeInfo);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public string TypeName
        {
            get
            {
                return this.InvokeInfo?.OwnerType?.Name;
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            if( this.PrefixObject != null )
            {
                this.PrefixObject.WriteTo(writer); 
            }
            writer.Write(".custom ");
            if( this.Prefix != null && this.Prefix.Length > 0 )
            {
                writer.Write(this.Prefix + " ");
            }
            this.InvokeInfo.WriteTo(writer);
            writer.Write(" = (");
            writer.WriteHexs(this.BinaryValue);
            writer.WriteLine(")");
        }
        public string HexValue = null;
        public override string ToString()
        {
            return ".custom " + this.AttributeTypeName;
        }



    }

    internal class DCILAssembly : DCILMemberInfo
    {
        public const string TagName = ".assembly";
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Assembly;
            }
        }
        public bool IsExtern = false;

        public List<string> MResourceNames = null;

        public override void Load(DCILReader reader)
        {
            LoadHeader(reader);
            LoadContent(reader);
        }

        public void LoadHeader(DCILReader reader)
        {
            var strWord = reader.ReadWord();
            if (strWord == "extern")
            {
                this.IsExtern = true;
                this._Name = reader.ReadWord();
            }
            else
            {
                this.IsExtern = false;
                this._Name = strWord;
            }
        }
        public void LoadContent(DCILReader reader)
        {
            if (this.ChildNodes == null)
            {
                this.ChildNodes = new DCILObjectList();
            }
            reader.MoveAfterChar('{');// reader.ReadAfterChar('{');
            while (reader.HasContentLeft())
            {
                var strWord = reader.ReadWord();
                if (strWord == "}")
                {
                    // 退出代码组
                    break;
                }
                switch (strWord)
                {
                    case DCILCustomAttribute.TagName_custom:
                        base.ReadCustomAttribute(reader);
                        break;
                    case ".mresouce":
                        {
                            if (this.MResourceNames == null)
                            {
                                this.MResourceNames = new List<string>();

                                var strWord2 = reader.ReadWord();
                                if (strWord2 == "public")
                                {
                                    var name2 = reader.ReadLine()?.Trim();
                                    if (name2 != null && name2.Length > 0)
                                    {
                                        this.MResourceNames.Add(name2);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        {
                            this.ChildNodes.Add(new DCILUnknowObject(strWord, reader));
                        }
                        break;
                }

            }
        }
        //private Dictionary<string, DCILClass> _ClassMap = new Dictionary<string, DCILClass>();

        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".assembly ");
            if (this.IsExtern)
            {
                writer.Write(" extern ");
                writer.WriteLine(this._Name);
            }
            else
            {
                writer.WriteLine(this._Name);
            }
            writer.WriteStartGroup();
            base.WriteCustomAttributes(writer);
            writer.WriteObjects(this.ChildNodes);
            writer.WriteEndGroup();
        }

        public override string ToString()
        {
            if (this.IsExtern)
            {
                return ".assembly extern " + this._Name;
            }
            else
            {
                return ".assembly " + this._Name;
            }
        }
    }
    internal class DCILModule : DCILObject
    {
        //public DCILModule()
        //{

        //}
        public DCILModule(DCILReader reader)
        {
            this.Load(reader);
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Module;
            }
        }
        public const string TagName_Module = ".module";
        public bool IsExtern = false;
        public override void Load(DCILReader reader)
        {
            string v = reader.ReadWord();
            if (v == "extern")
            {
                this.IsExtern = true;
                this._Name = reader.ReadWord();
            }
            else
            {
                this.IsExtern = false;
                this._Name = v;
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.WriteLine(this.ToString());
        }
        public override string ToString()
        {
            if (this.IsExtern)
            {
                return ".module extern " + this._Name;
            }
            else
            {
                return ".module " + this._Name;
            }
        }
    }
    
    internal class DCILUnknowObject : DCILObject
    {
        //public DCILUnknowObject()
        //{
        //}
        public DCILUnknowObject(string name, DCILReader reader)
        {
            //if( name == "Syste")
            //{

            //}
            this._Name = name;
            this.Data = reader.ReadInstructionContent();
            if (this.Data != null)
            {
                this.Data = this.Data.Trim();
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.EnsureNewLine();
            writer.Write(this._Name);
            writer.Write(' ');
            writer.WriteLine(this.Data);
        }
        public string Data = null;
        public override string ToString()
        {
            return this._Name + " " + this.Data;
        }
        public override void Dispose()
        {
            base.Dispose();
            this.Data = null;
        }
    }
    /// <summary>
    /// 重命名状态
    /// </summary>
    internal enum DCILRenameState
    {
        /// <summary>
        /// 未处理
        /// </summary>
        NotHandled,
        /// <summary>
        /// 已经重命名了
        /// </summary>
        Renamed,
        /// <summary>
        /// 需要保留名称，不重命名
        /// </summary>
        Preserve
    }
    internal class DCILMemberInfo : DCILObject
    {
        public DCILMemberInfo()
        {

        }
        protected DCILMemberInfo(System.Reflection.MemberInfo member)
        {
            this._Name = member.Name;
            this.IsNative = true;
        }
        public override void Dispose()
        {
            base.Dispose();
            this.OldName = null;
            this.OldSignatureForMap = null;
            if(this.Styles != null )
            {
                this.Styles.Clear();
                this.Styles = null;
            }
        }
        /// <summary>
        /// 为重命名而添加EditorBrowsableAttribute特性
        /// </summary>
        /// <param name="ebattr"></param>
        public bool AddEditorBrowsableAttributeForRename(DCILCustomAttribute ebattr)
        {
            //return false;
            if (this.RenameState == DCILRenameState.Renamed
                && this.Styles != null
                && this.Styles.Contains("public"))
            {
                if (this.CustomAttributes == null)
                {
                    this.CustomAttributes = new List<DCILCustomAttribute>();
                }
                else
                {
                    foreach( var attr in this.CustomAttributes )
                    {
                        if(attr.AttributeTypeName == ebattr.AttributeTypeName )
                        {
                            this.CustomAttributes.Remove(attr);
                            break;
                        }
                    }
                }
                this.CustomAttributes.Add(ebattr);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 为建立映射文件而保存的旧签名
        /// </summary>
        public string OldSignatureForMap = null;

        public virtual string GetSignatureForMap()
        {
            return ((DCILClass)this.Parent).GetNameWithNested('.') + "." + this.Name;
        }

        public virtual void ChangeName(string newName)
        {
            if (this.RenameState == DCILRenameState.Renamed)
            {

            }
            this._Name = newName;
            this.RenameState = DCILRenameState.Renamed;
            //this.RemoveObfuscationAttribute();
        }
        public readonly bool IsNative = false;

        /// <summary>
        /// 重命名操作状态
        /// </summary>
        public DCILRenameState RenameState = DCILRenameState.NotHandled;

        public string OldName = null;
        public string GetOldName()
        {
            if( this.OldName == null || this.OldName.Length == 0 )
            {
                return this.Name;
            }
            else
            {
                return this.OldName;
            }
        }
        protected void WriteCustomAttributes(DCILWriter writer)
        {
            if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
            {
                foreach (var item in this.CustomAttributes)
                {
                    //writer.WriteLine();
                    item.WriteTo(writer);
                }
            }
        }
        //public void CollectAttributes(List<DCILCustomAttribute> attributes)
        //{
        //    if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
        //    {
        //        attributes.AddRange(this.CustomAttributes);
        //    }
        //}
        protected void WriteStyles(DCILWriter writer)
        {
            if (this.Styles != null && this.Styles.Count > 0)
            {
                foreach (var item in this.Styles)
                {
                    writer.Write(" " + item);
                }
                writer.Write(' ');
            }
        }

        public List<string> Styles = null;
        /// <summary>
        /// 删除样式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveStyle(string name)
        {
            if (name != null && name.Length > 0 && this.Styles != null)
            {
                int index = this.Styles.IndexOf(name);
                if (index >= 0)
                {
                    this.Styles.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }
        public bool RemoveStyle_specialname()
        {
            return RemoveStyle("specialname");
        }

        public void AddStyles(params string[] names)
        {
            if (names != null && names.Length > 0)
            {
                if (this.Styles == null)
                {
                    this.Styles = new List<string>();
                }
                this.Styles.AddRange(names);
            }
        }
        public bool AddStyle(string name)
        {
            if (name != null && name.Length > 0)
            {
                if (name[0] >= 'a' && name[0] <= 'z')
                {
                    if (this.Styles == null)
                    {
                        this.Styles = new List<string>();
                    }
                    this.Styles.Add(name);
                    return true;
                }
            }
            return false;
        }

        public bool HasStyle(string name)
        {
            return this.Styles != null && this.Styles.Contains(name);
        }

        public virtual void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            if (this.CustomAttributes != null)
            {
                foreach (var item in this.CustomAttributes)
                {
                    item.InvokeInfo = document.CacheDCILInvokeMethodInfo(item.InvokeInfo);
                    if(item.PrefixObject?.RefType != null )
                    {
                        item.PrefixObject.RefType = document.CacheTypeReference(item.PrefixObject.RefType);
                    }
                }
            }
        }
    }
    internal class DCILObjectList : List<DCILObject> , IDisposable
    {
        public DCILObjectList()
        {

        }
        public void Dispose()
        {
            foreach( var item in this )
            {
                item.Dispose();
            }
            this.Clear();
        }
        //public DCILObjectList Clone()
        //{
        //    var list = new DCILObjectList();
        //    list.AddRange(this);
        //    return list;
        //}
        public bool RemoveByName(string name)
        {
            for (int iCount = this.Count - 1; iCount >= 0; iCount--)
            {
                if (this[iCount].Name == name)
                {
                    this.RemoveAt(iCount);
                    return true;
                }
            }
            return false;
        }

        public DCILObject GetByName( string name )
        {
            foreach( var item in this )
            {
                if(item.Name == name )
                {
                    return item;
                }
            }
            return null;
        }
        //public int IndexOf<T>(string name) where T : DCILObject
        //{
        //    int len = this.Count;
        //    for (int iCount = 0; iCount < len; iCount++)
        //    {
        //        if (this[iCount] is T && this[iCount].Name == name)
        //        {
        //            return iCount;
        //        }
        //    }
        //    return -1;
        //}
        //public T[] GetSubArray<T>() where T : DCILObject
        //{
        //    var list = new List<T>();
        //    foreach (var item in this)
        //    {
        //        if (item is T)
        //        {
        //            list.Add((T)item);
        //        }
        //    }
        //    if (list.Count > 0)
        //    {
        //        return list.ToArray();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
    internal enum DCILObjectType
    {
        None,
        Field,
        Method,
        Property,
        Event,
        Class,
        Resource,
        Data,
        Assembly,
        CustomAttribute,
        Document,
        Module
    }

    internal class DCILObject : IDisposable
    {
        private static int _InstanceIndexCounter = 0;
        public int InstanceIndex = _InstanceIndexCounter++;
        public DCILDocument OwnerDocument = null;
        /// <summary>
        /// 对象类型
        /// </summary>
        public virtual DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.None;
            }
        }

        internal string _Name = null;
        public string Name
        {
            get
            {
                return this._Name;
            }
        }
        public string Type = null;
        // public string Header = null;
        public virtual void Load(DCILReader reader)
        {

        }
        public virtual void WriteTo(DCILWriter writer)
        {

        }
        public virtual void Dispose()
        {
            this._Name = null;
            if(this.ChildNodes != null )
            {
                this.ChildNodes.Dispose();
                this.ChildNodes = null;
            }
            if(this.CustomAttributes != null )
            {
                foreach( var item in this.CustomAttributes )
                {
                    item.Dispose();
                }
                this.CustomAttributes = null;
            }
            if(this.ObfuscationSettings != null )
            {
                this.ObfuscationSettings.Dispose();
                this.ObfuscationSettings = null;
            }
            if(this.OperCodes != null )
            {
                this.OperCodes.Dispose();
                this.OperCodes = null;
            }
            this.OwnerDocument = null;
            this.Parent = null;
            this.Type = null;
        }
        public int StartLineIndex = 0;
        public DCILOperCodeList OperCodes = null;
        public List<T> GetAllOperCodes<T>() where T : DCILOperCode
        {
            var list = new List<T>();
            this.CollectOperCodes<T>(list);
            return list;
        }
        public int CollectOperCodes<T>(List<T> outList) where T : DCILOperCode
        {
            if (this.OperCodes != null && this.OperCodes.Count > 0)
            {
                int result = outList.Count;
                InnerCollectOperCodes<T>(this.OperCodes, outList);
                result = outList.Count - result;
                return result;
            }
            return 0;
        }

        private void InnerCollectOperCodes<T>(DCILOperCodeList list, List<T> outList) where T : DCILOperCode
        {
            foreach (var item in list)
            {
                if (item is T)
                {
                    outList.Add((T)item);
                }
                if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var block = (DCILOperCode_Try_Catch_Finally)item;
                    if (block.HasTryOperCodes())
                    {
                        InnerCollectOperCodes<T>(block._Try.OperCodes, outList);
                    }
                    if (block.HasCatchs())
                    {
                        foreach (var item2 in block._Catchs)
                        {
                            if (item2.OperCodes != null && item2.OperCodes.Count > 0)
                            {
                                InnerCollectOperCodes<T>(item2.OperCodes, outList);
                            }
                        }
                    }
                    if (block.HasFinallyOperCodes())
                    {
                        InnerCollectOperCodes<T>(block._Finally.OperCodes, outList);
                    }
                    if (block.HasFaultOperCodes())
                    {
                        InnerCollectOperCodes<T>(block._fault.OperCodes, outList);
                    }
                }
            }
        }
        /// <summary>
        /// 子节点列表
        /// </summary>
        public DCILObjectList ChildNodes = null;
        /// <summary>
        /// 父节点
        /// </summary>
        public DCILObject Parent = null;
        /// <summary>
        /// 指定的混淆设置
        /// </summary>
        public DCILObfuscationAttribute ObfuscationSettings = null;
        /// <summary>
        /// 是否具有自定义特性
        /// </summary>
        public bool HasCustomAttributes
        {
            get
            {
                return this.CustomAttributes != null && this.CustomAttributes.Count > 0;
            }
        }
        public List<DCILCustomAttribute> CustomAttributes = null;
        public void CusotmAttributesCacheTypeReference(DCILDocument document)
        {
            if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
            {
                foreach (var item in this.CustomAttributes)
                {
                    item.InvokeInfo?.CacheTypeReference(document);
                }
            }
        }
        internal DCILCustomAttribute ReadCustomAttribute(DCILReader reader)
        {
            if (this.CustomAttributes == null)
            {
                this.CustomAttributes = new List<DCILCustomAttribute>();
            }
            var item = DCILCustomAttribute.Create(this, reader);
            this.CustomAttributes.Add(item);
            if (item is DCILObfuscationAttribute)
            {
                this.ObfuscationSettings = (DCILObfuscationAttribute)item;
            }
            return item;
        }
        internal void AddCustomAttributes(List<DCILCustomAttribute> attrs)
        {
            if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
            {
                attrs.AddRange(this.CustomAttributes);
            }
        }
        internal void RemoveObfuscationAttribute()
        {
            //return;
            if (this.ObfuscationSettings != null)
            {
                this.ObfuscationSettings = null;
                if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
                {
                    for (int iCount = this.CustomAttributes.Count - 1; iCount >= 0; iCount--)
                    {
                        if (this.CustomAttributes[iCount] is DCILObfuscationAttribute)
                        {
                            this.CustomAttributes.RemoveAt(iCount);
                            break;
                        }
                    }
                }
            }
        }

        public int Level = 0;
        public int TotalOperCodesCount
        {
            get
            {
                int result = 0;
                if (this.OperCodes != null && this.OperCodes.Count > 0)
                {
                    result = this.OperCodes.Count;
                    foreach (var item in this.OperCodes)
                    {
                        if (item is DCILOperCode_Try_Catch_Finally)
                        {
                            result += ((DCILOperCode_Try_Catch_Finally)item).TotalOperCodesCount;
                        }
                    }
                }
                return result;
            }
        }
        public override string ToString()
        {
            if (this.ChildNodes != null && this.ChildNodes.Count > 0)
            {
                return this.Type + "#" + this._Name + " " + this.ChildNodes.Count + "个子节点";
            }
            else
            {
                return this.Type + "#" + this._Name;
            }
        }
        public virtual DCILMethod GetOwnerMethod()
        {
            var p = this;
            while (p != null)
            {
                if (p is DCILMethod)
                {
                    return (DCILMethod)p;
                }
                p = p.Parent;
            }
            return null;
        }
    }
    internal class DCILData : DCILObject
    {
        public const string TagName_Data = ".data";
        public DCILData()
        {

        }
        public DCILData(DCILReader reader)
        {
            var strHeader = reader.ReadAfterCharExcludeLastChar('=');
            var words = DCUtils.SplitByWhitespace(strHeader);
            this._Name = words[words.Count - 1];
            this.IsCil = words.Contains("cil");
            this.DataType = reader.ReadWord();
            int lineindex = reader.CurrentLineIndex();
            if (this.DataType == "bytearray")
            {
                this.Value = reader.ReadBinaryFromHex();
            }
            else
            {
                this.Value = reader.ReadLineTrimRemoveComment();
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            this.DataType = null;
            this.Value = null;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Data;
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".data cil " + this._Name + " = " + this.DataType);
            if (this.DataType == "bytearray")
            {
                var bs = (byte[])this.Value;
                if (bs.Length > 16)
                {
                    writer.WriteLine("(");
                }
                else
                {
                    writer.Write("(");
                }
                if( this.XORKey > 0 )
                {
                    var bsNative = (byte[])this.Value;
                    bs = (byte[]) bs.Clone();
                    XORDatas(bs, this.XORKey);
                    var bs2 = (byte[])bs.Clone();
                    XORDatas(bs2, this.XORKey);
                    for(int iCount = bs.Length -1; iCount >=0; iCount --)
                    {
                        if(bsNative[ iCount ] != bs2[iCount])
                        {

                        }
                    }
                }
                writer.WriteHexs(bs);
                writer.WriteLine(")");
            }
            else
            {
                writer.WriteLine(Convert.ToString(this.Value));
            }
        }

        private unsafe void XORDatas(byte[] v, int encKey)
        {
            if (v.Length >= 4 && encKey != 0)
            {
                fixed (byte* pByte = v)
                {
                    int* pStart = (int*)pByte;
                    int* pEnd = pStart + (int)Math.Floor(v.Length / 4.0) - 1;
                    while (pEnd >= pStart)
                    {
                        *pEnd = *pEnd ^ encKey;
                        encKey += 13;
                        pEnd--;
                    }
                }
            }
        }

        public int XORKey = 0;

        public string DataType = null;
        public object Value = null;
        public bool IsCil = true;
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(".data");
            if (this.IsCil)
            {
                str.Append(" cil");
            }
            str.Append(this._Name);
            str.Append(" = " + this.DataType);
            if (this.Value is byte[])
            {
                str.Append(" " + DCUtils.FormatByteSize(((byte[])this.Value).Length));
            }
            else
            {
                str.Append(" " + Convert.ToString(this.Value));
            }
            return str.ToString();
        }
    }
    internal enum DCILTypeMode
    {
        Primitive,
        ValueType,
        Class,
        Native,
        Unsigned,
        GenericTypeInTypeDefine,
        GenericTypeInMethodDefine,
        NotSpecify
    }
    /// <summary>
    /// 类型名称信息
    /// </summary>
    internal class DCILTypeNameInfo :IDisposable
    {
        public DCILTypeNameInfo()
        {

        }
        public DCILTypeNameInfo(string name)
        {
            Parse(name);
        }
        public virtual void Dispose()
        {
            this.AssemblyName = null;
            this.AssemblyCulture = null;
            this.AssemblyPublicKeyToken = null;
            this.AssemblyVersion = null;
        }
        public void Parse(string name)
        {
            if (name == null || name.Length == 0)
            {
                return;
            }
            if (name[0] == '[')
            {
                int index = name.IndexOf(']');
                if (index > 0)
                {
                    // 符合 [LibrayName]TypeName,例如 [System]System.Object
                    this.AsmNamePrefix = true;
                    this.AssemblyName = name.Substring(1, index - 2);
                    this.TypeName = name.Substring(index + 1);
                    return;
                }
            }
            int index2 = name.IndexOf(',');
            if (index2 > 0)
            {
                this.AsmNamePrefix = false;
                // 例如 System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                this.TypeName = name.Substring(0, index2);
                var items = name.Substring(index2 + 1).Split(',');
                foreach (var item in items)
                {
                    var item2 = item.Trim();
                    int index3 = item2.IndexOf('=');
                    if (index3 < 0 && this.AssemblyName == null)
                    {
                        this.AssemblyName = item2;
                    }
                    else if (index3 > 0)
                    {
                        var name2 = item2.Substring(0, index3).Trim();
                        var v2 = item2.Substring(index3 + 1);
                        switch (name2.ToLower())
                        {
                            case "version": this.AssemblyVersion = v2; break;
                            case "culture": this.AssemblyCulture = v2; break;
                            case "publickeytoken": this.AssemblyPublicKeyToken = v2; break;
                        }
                    }
                }
            }
            if (this.TypeName == null)
            {
                this.TypeName = name;
            }
            this.TypeName = DCUtils.GetStringUseTable(this.TypeName);
            this.AssemblyCulture = DCUtils.GetStringUseTable(this.AssemblyCulture);
            this.AssemblyName = DCUtils.GetStringUseTable(this.AssemblyName);
            this.AssemblyPublicKeyToken = DCUtils.GetStringUseTable(this.AssemblyPublicKeyToken);
            this.AssemblyVersion = DCUtils.GetStringUseTable(this.AssemblyVersion);

        }
        public string TypeName = null;
        public string AssemblyName = null;
        public bool HasAssemblyName
        {
            get
            {
                return this.AssemblyName != null && this.AssemblyName.Length > 0;
            }
        }
        public string AssemblyVersion = null;
        public string AssemblyCulture = null;
        public string AssemblyPublicKeyToken = null;
        public bool AsmNamePrefix = false;
        public override string ToString()
        {
            return ToTypeString();
        }
        protected string ToTypeString()
        {
            var str = new StringBuilder();
            if (this.AsmNamePrefix)
            {
                str.Append("[" + this.AssemblyName + "]");
                str.Append(this.TypeName);
            }
            else
            {
                str.Append(this.TypeName);
                bool hasItem = false;
                if (this.HasAssemblyName)
                {
                    str.Append(',');
                    str.Append(this.AssemblyName);
                    if (this.AssemblyName == "DCSoft.Common")
                    {

                    }
                    hasItem = true;
                }
                if (this.AssemblyVersion != null && this.AssemblyVersion.Length > 0)
                {
                    if (hasItem)
                    {
                        str.Append(',');
                    }
                    hasItem = true;
                    str.Append("Version=" + this.AssemblyVersion);
                }
                if (this.AssemblyCulture != null && this.AssemblyCulture.Length > 0)
                {
                    if (hasItem)
                    {
                        str.Append(',');
                    }
                    str.Append("Culture=" + this.AssemblyCulture);
                    hasItem = true;
                }
                if (this.AssemblyPublicKeyToken != null && this.AssemblyPublicKeyToken.Length > 0)
                {
                    if (hasItem)
                    {
                        str.Append(',');
                    }
                    str.Append("PublicKeyToken=" + this.AssemblyPublicKeyToken);
                }
            }
            return str.ToString();
        }
    }

    /// <summary>
    /// see"Partition II Metadata.doc",topic 7.1
    /// </summary>
    internal class DCILTypeReference : IEqualsValue<DCILTypeReference> , IDisposable
    {
        internal static void Test()
        {
            var ttt = DCILTypeReference.LoadText("class DCSoft.Writer.Dom.XTextTableCellElement[0...,0...]");
            var t = DCILTypeReference.LoadText("int8");
            var t56 = DCILTypeReference.LoadText("class [mscorlib]System.Collections.Generic.List`1<class OpenSource.ICSharpCode.TextEditor.Document.TextMarker>&");
            var t99 = DCILTypeReference.LoadText("int8* [ ][ ]");
            var t23 = DCILTypeReference.LoadText("valuetype [mscorlib]System.Decimal[][]");
            var t2 = DCILTypeReference.LoadText("valuetype [mscorlib]System.Decimal::ToString()");
            var t3 = DCILTypeReference.LoadText(@"class [mscorlib]System.Collections.Generic.Dictionary`2<string, 
                    class [mscorlib]System.Collections.Generic.List`1<class ConsoleApp4.aaaa>>::ToString()");
            var t4 = DCILTypeReference.LoadText("class [mscorlib]System.Collections.Generic.Dictionary`2<string, class ConsoleApp4.aaaa>");
            var t46 = DCILTypeReference.LoadText("class bbbb[][]");
            var t5 = DCILTypeReference.LoadText("zzz");
            var t52 = DCILTypeReference.LoadText("zzz[]");
            var t53 = DCILTypeReference.LoadText("zzz*");
            var t6 = DCILTypeReference.LoadText("!T");
            var t9 = DCILTypeReference.LoadText("!!TT");
            var t999 = DCILTypeReference.LoadText("class [mscorlib]System.Collections.Generic.IComparer`1<valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,float64>>");
        }
        private static readonly Dictionary<string, DCILTypeReference> _PrimitiveTypes = null;
        private static readonly HashSet<string> PrimitiveTypeNames = null;
        public static readonly DCILTypeReference Type_Void = null;
        public static readonly DCILTypeReference Type_String = null;
        public static readonly DCILTypeReference Type_Object = null;
        public static readonly DCILTypeReference Type_Int32 = null;
        public static readonly DCILTypeReference Type_Boolean = null;
        public static readonly DCILTypeReference Type_Char = null;
        public static readonly DCILTypeReference Type_Byte = null;
        static DCILTypeReference()
        {
            PrimitiveTypeNames = new HashSet<string>();
            PrimitiveTypeNames.Add("uint8");
            PrimitiveTypeNames.Add("int8");
            PrimitiveTypeNames.Add("char");
            PrimitiveTypeNames.Add("bool");
            PrimitiveTypeNames.Add("int16");
            PrimitiveTypeNames.Add("uint16");
            PrimitiveTypeNames.Add("int32");
            PrimitiveTypeNames.Add("uint32");
            PrimitiveTypeNames.Add("int64");
            PrimitiveTypeNames.Add("uint64");
            PrimitiveTypeNames.Add("float32");
            PrimitiveTypeNames.Add("float64");
            PrimitiveTypeNames.Add("valuetype");
            PrimitiveTypeNames.Add("string");
            PrimitiveTypeNames.Add("native");
            PrimitiveTypeNames.Add("class");
            PrimitiveTypeNames.Add("object");
            PrimitiveTypeNames.Add("void");
            PrimitiveTypeNames.Add("unsigned");
            PrimitiveTypeNames.Add("lpstr");
            PrimitiveTypeNames.Add("lpwstr");
            PrimitiveTypeNames.Add("typedref");

            _PrimitiveTypes = new Dictionary<string, DCILTypeReference>();
            AddPrimitiveType("uint8", typeof(byte))._NameInCSharp = "byte";
            AddPrimitiveType("int8", typeof(sbyte))._NameInCSharp ="sbyte";
            AddPrimitiveType("char", typeof(char))._NameInCSharp = "char";
            AddPrimitiveType("bool", typeof(bool))._NameInCSharp = "bool";
            AddPrimitiveType("int16", typeof(short))._NameInCSharp = "short";
            AddPrimitiveType("uint16", typeof(ushort))._NameInCSharp = "ushort";
            AddPrimitiveType("int32", typeof(int))._NameInCSharp = "int";
            AddPrimitiveType("uint32", typeof(uint))._NameInCSharp = "uint";
            AddPrimitiveType("int64", typeof(long))._NameInCSharp = "long";
            AddPrimitiveType("uint64", typeof(ulong))._NameInCSharp = "ulong";
            AddPrimitiveType("float32", typeof(float))._NameInCSharp = "float";
            AddPrimitiveType("float64", typeof(double))._NameInCSharp = "double";
            AddPrimitiveType("string", typeof(string))._NameInCSharp = "string";
            AddPrimitiveType("object", typeof(object))._NameInCSharp = "object";
            AddPrimitiveType("void", typeof(void))._NameInCSharp = "void";
            AddPrimitiveType("lpwstr", typeof(string))._NameInCSharp = "string";
            AddPrimitiveType("typedref", typeof(System.TypedReference))._NameInCSharp = "System.TypedReference";

            Type_Void = _PrimitiveTypes["void"];
            Type_String = _PrimitiveTypes["string"];
            Type_Object = _PrimitiveTypes["object"];
            Type_Int32 = _PrimitiveTypes["int32"];
            Type_Boolean = _PrimitiveTypes["bool"];
            Type_Char = _PrimitiveTypes["char"];
            Type_Byte = _PrimitiveTypes["uint8"];
            _Cache_CreateByNativeType[typeof(string)] = Type_String;

        }
        public void Dispose()
        {
            if(this.Mode == DCILTypeMode.Primitive )
            {
                return;
            }
            if(this.GenericParamters != null )
            {
                this.GenericParamters.Clear();
                this.GenericParamters = null;
            }
            this.LibraryName = null;
            this.LocalClass = null;
            this.Name = null;
            this._ArrayAndPointerSettings = null;
            this._NameInCSharp = null;
            this._NativeType = null;
        }
        public bool EqualsValue(DCILTypeReference type, DCILGenericParamterList gps)
        {
            if (type == null)
            {
                return false;
            }
            if (type == this)
            {
                return true;
            }
            if (type.Mode == DCILTypeMode.Primitive || this.Mode == DCILTypeMode.Primitive)
            {
                return this.EqualsValue(type);
            }
            if (gps == null || gps.Count == 0)
            {
                return this.EqualsValue(type);
            }
            else
            {
                var type1 = this.Transform(gps);
                var type2 = type.Transform(gps);
                return type1.EqualsValue(type2);
            }
        }
        public DCILTypeReference Transform(DCILGenericParamterList gps)
        {
            if (gps == null || gps.Count == 0)
            {
                return this;
            }
            if (this.Mode == DCILTypeMode.GenericTypeInMethodDefine)
            {
                var item = gps.GetItem(this.Name, false);
                if (item != null && item.RuntimeType != null)
                {
                    if (this.ArrayAndPointerSettings != null && this.ArrayAndPointerSettings.Length > 0)
                    {
                        var result2 = item.RuntimeType.Clone();
                        result2.ArrayAndPointerSettings = this.ArrayAndPointerSettings;
                        return result2;
                    }
                    else
                    {
                        return item.RuntimeType;
                    }
                }
                return this;
            }
            if (this.Mode == DCILTypeMode.GenericTypeInTypeDefine)
            {
                var item = gps.GetItem(this.Name, true);
                if (item != null && item.RuntimeType != null)
                {
                    if (this.ArrayAndPointerSettings != null && this.ArrayAndPointerSettings.Length > 0)
                    {
                        var result2 = item.RuntimeType.Clone();
                        result2.ArrayAndPointerSettings = this.ArrayAndPointerSettings;
                        return result2;
                    }
                    else
                    {
                        return item.RuntimeType;
                    }
                }
                return this;
            }
            if (this.GenericParamters == null || this.GenericParamters.Count == 0)
            {
                return this;
            }
            else
            {
                DCILTypeReference result = (DCILTypeReference)this.MemberwiseClone();
                result.GenericParamters = new List<DCILTypeReference>();
                foreach (var item in this.GenericParamters)
                {
                    result.GenericParamters.Add(item.Transform(gps));
                }
                return result;
            }
        }

        private static DCILTypeReference AddPrimitiveType(string name, Type nativeType)
        {
            var t = new DCILTypeReference(name, DCILTypeMode.Primitive);
            t._NativeType = nativeType;
            _PrimitiveTypes[name] = t;
            _Cache_CreateByNativeType[nativeType] = t;
            return t;
        }
        private string _NameInCSharp = null;
        public string NameInCSharp
        {
            get
            {
                return this._NameInCSharp;
            }
        }
        public static DCILTypeReference GetPrimitiveType(string name)
        {
            DCILTypeReference result = null;
            if (_PrimitiveTypes.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }
        public static DCILTypeReference LoadText(string text)
        {
            var reader = new DCILReader(text, null);
            return Load(reader.ReadWord(), reader);
        }
        private static readonly Dictionary<Type, DCILTypeReference> _Cache_CreateByNativeType
            = new Dictionary<Type, DCILTypeReference>();
        public static DCILTypeReference CreateByNativeType(Type t, DCILDocument document)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            DCILTypeReference result = null;
            if (_Cache_CreateByNativeType.TryGetValue(t, out result) == false)
            {
                result = new DCILTypeReference(t, document);
                _Cache_CreateByNativeType[t] = result;
            }
            return result;
        }

        public DCILTypeReference ( DCILClass cls )
        {
            if( cls == null )
            {
                throw new ArgumentNullException("cls");
            }
            this.Name = cls.Name;
            this.Mode = DCILTypeMode.Class;
            this.LocalClass = cls;
        }

        public DCILTypeReference(Type nativeType, DCILDocument document)
        {
            if (nativeType == null)
            {
                throw new ArgumentNullException("nativeType");
            }
            this._NativeType = nativeType;
            if (nativeType == typeof(System.IntPtr))
            {
                this.Mode = DCILTypeMode.Native;
                this.Name = "int";
                return;
            }

            if (nativeType.IsPointer)
            {
                this.ArrayAndPointerSettings = "*";
            }
            if (nativeType.IsArray)
            {
                int rank = nativeType.GetArrayRank();
                for (int iCount = 0; iCount < rank; iCount++)
                {
                    this.ArrayAndPointerSettings = this.ArrayAndPointerSettings + "[]";
                }
                nativeType = nativeType.GetElementType();
            }
            else if (nativeType.HasElementType)
            {
                nativeType = nativeType.GetElementType();
                this.ArrayAndPointerSettings = this.ArrayAndPointerSettings + "&";
            }


            if (nativeType.IsValueType)
            {
                this.Mode = DCILTypeMode.ValueType;
            }
            else
            {
                this.Mode = DCILTypeMode.Class;
            }
            if (nativeType.IsGenericParameter)
            {
                this.Name = nativeType.Name;
                this.Mode = DCILTypeMode.GenericTypeInMethodDefine;
            }
            else if (nativeType.IsGenericType)
            {
                this.Name = DCUtils.GetFullName(nativeType);
                //this.Mode = DCILTypeMode.GenericTypeInTypeDefine;
            }
            else
            {
                this.Name = nativeType.FullName.Replace('+', '/');
            }

            this.LibraryName = document.GetLibraryName(this.Name);
            if (this.LibraryName == null)
            {
                this.LibraryName = nativeType.Assembly.GetName().Name;
            }

            if (nativeType.IsGenericType)
            {
                var gps = nativeType.GetGenericArguments();
                this.GenericParamters = new List<DCILTypeReference>(gps.Length);
                foreach (var gp in gps)
                {
                    var p = new DCILTypeReference(gp, document);
                    p.Mode = DCILTypeMode.GenericTypeInTypeDefine;
                    this.GenericParamters.Add(p);
                }
            }
        }

        public static DCILTypeReference Load(DCILReader reader)
        {
            int pos = reader.Position;
            var word = reader.ReadWord();
            if (IsStartWord(word))
            {
                return Load(word, reader);
            }
            else
            {
                reader.Position = pos;
                return Load(ClassMode_NotSpecify, reader);
            }
        }

        public static DCILTypeReference Load(string firstWord, DCILReader reader)
        {
            if (firstWord == "valuetype"
                || firstWord == "class"
                || firstWord == "native"
                || firstWord == ClassMode_NotSpecify
                || firstWord[0] == '!'
                || reader == null)
            {
                return new DCILTypeReference(firstWord, reader);
            }
            DCILTypeReference result = null;
            if (_PrimitiveTypes.TryGetValue(firstWord, out result))
            {
                if (reader != null && "*&[]".IndexOf(reader.PeekContentChar()) >= 0)
                {
                    // 可能为数组或者指针类型
                    var result2 = (DCILTypeReference)result.MemberwiseClone();
                    if (result2.ReadArrayAndPointerSettings(reader))
                    {
                        return result2;
                    }
                    else
                    {
                        return result;
                    }
                }
                return result;
            }
            result = new DCILTypeReference(firstWord);
            result.ReadArrayAndPointerSettings(reader);
            return result;
        }

        public static bool IsStartWord(string word)
        {
            if (PrimitiveTypeNames.Contains(word))
            {
                return true;
            }
            if (word[0] == '!')
            {
                return true;
            }
            return false;
        }
        public static void ClearGlobalBuffer()
        {
            foreach (var item in _Cache_CreateByNativeType)
            {
                if (item.Value.IsPrimitive == false)
                {
                    item.Value.Dispose();
                }
            }
            _Cache_CreateByNativeType.Clear();
        }

        private static readonly Dictionary<string, Type> _NativeTypes = new Dictionary<string, Type>();
        private static System.Reflection.Assembly[] _LocalAssemblies = null;
        public DCILTypeReference Clone()
        {
            return (DCILTypeReference)this.MemberwiseClone();
        }
#if DOTNETCORE
        
        internal static System.Runtime.Loader.AssemblyLoadContext _AsmLoader = null;

#endif
        private static HashSet<string> _MissLibNames = new HashSet<string>();
        public static void ClearCacheNativeTypes()
        {
            _NativeTypes.Clear();
        }
        public Type SearchNativeType(string searchPath = null)
        {
            if (this._NativeType != null)
            {
                return this._NativeType;
            }
            Type result = null;
            if (_NativeTypes.TryGetValue(this.NameWithLibraryName, out result) == false)
            {
                if (this.LibraryName != null
                    && this.LibraryName.Length > 0
                    && _MissLibNames.Contains(this.LibraryName) == false)
                {
                    bool findAsm = false;
                    if (_LocalAssemblies == null)
                    {
                        _LocalAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    }
                    foreach (var asm in _LocalAssemblies)
                    {
                        if (asm.GetName().Name == this.LibraryName)
                        {
                            findAsm = true;
                            result = asm.GetType(this.Name, false, false);
                            break;
                        }
                    }
                    if (findAsm == false)
                    {
                        var path = Path.GetDirectoryName(typeof(string).Assembly.Location);
                        var asmFileName = Path.Combine(path, this.LibraryName + ".dll");
                        if (File.Exists(asmFileName) == false)
                        {
                            if (searchPath != null && searchPath.Length > 0)
                            {
                                asmFileName = Path.Combine(searchPath, this.LibraryName + ".dll");
                            }
                        }
                        if (File.Exists(asmFileName) == false)
                        {
                            asmFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.LibraryName + ".dll");
                        }
                        if (File.Exists(asmFileName))
                        {
                            try
                            {
                                System.Reflection.Assembly asm = null;
#if DOTNETCORE
                                if (_AsmLoader != null)
                                {
                                    asm = _AsmLoader.LoadFromAssemblyPath(asmFileName);
                                }
                                else
                                {
                                    asm = System.Reflection.Assembly.LoadFile(asmFileName);
                                }
#else
                                asm = System.Reflection.Assembly.LoadFile(asmFileName);
#endif
                                _LocalAssemblies = null;
                                findAsm = true;
                                if (asm != null)
                                {
                                    result = asm.GetType(this.Name, false, false);
                                }
                            }
                            catch (System.Exception ext)
                            {
                                if (_MissLibNames.Contains(this.LibraryName) == false)
                                {
                                    _MissLibNames.Add(this.LibraryName);
                                    MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                                    MyConsole.Instance.Write(Environment.NewLine + "    [Warring]Error load referenced assembly file : " + asmFileName + " , MSG:" + ext.Message);
                                    MyConsole.Instance.ResetColor();
                                }
                            }
                        }
                        else
                        {
                            if (_MissLibNames.Contains(this.LibraryName) == false)
                            {
                                _MissLibNames.Add(this.LibraryName);
                                MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                                MyConsole.Instance.Write(Environment.NewLine + "    [Warring]Can not find referenced assembly : " + this.LibraryName + ".dll");
                                MyConsole.Instance.ResetColor();
                            }
                        }
                    }
                    if (findAsm == false)
                    {
                        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if ((asm is System.Reflection.Emit.AssemblyBuilder) == false)
                            {
                                result = asm.GetType(this.Name, false, false);
                                if (result != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                _NativeTypes[this.NameWithLibraryName] = result;
            }
            return result;
        }

        private Type _NativeType = null;
        public Type NativeType
        {
            get
            {
                return this._NativeType;
            }
        }
        public void UpdateLocalClass(Dictionary<string, DCILClass> clses)
        {
            var bc = this.LocalClass;
            clses.TryGetValue(this.Name, out this.LocalClass);
            if (bc != null && this.LocalClass == null)
            {

            }
        }

        public DCILClass LocalClass = null;
        public bool Handled = false;
        private int _HashCode = 0;

        public override int GetHashCode()
        {
            if (this._HashCode == 0)
            {
                if (this.LibraryName != null)
                {
                    this._HashCode = this.LibraryName.GetHashCode();
                }
                if (this.Name != null)
                {
                    this._HashCode += this.Name.GetHashCode();
                }
                this._HashCode += (int)this.Mode;
                if (this.ArrayAndPointerSettings != null)
                {
                    this._HashCode += this.ArrayAndPointerSettings.GetHashCode();
                }
                if (this.GenericParamters != null && this.GenericParamters.Count > 0)
                {
                    foreach (var item in this.GenericParamters)
                    {
                        this._HashCode += item.GetHashCode();
                    }
                }
            }
            return this._HashCode;
        }
        public override bool Equals(object obj)
        {
            if (obj is DCILTypeReference)
            {
                return this.EqualsValue((DCILTypeReference)obj);
            }
            return false;
        }

        public static bool StaticEquals(DCILTypeReference t1, DCILTypeReference t2)
        {
            if (t1 == t2)
            {
                return true;
            }
            if (t1 != null)
            {
                return t1.EqualsValue(t2);
            }
            else
            {
                return false;
            }
        }
        public bool EqualsValue(DCILTypeReference t)
        {
            if (t == null)
            {
                return false;
            }
            if (t == this)
            {
                return true;
            }
            if (this.Name != t.Name)
            {
                return false;
            }
            if (this.LibraryName != t.LibraryName)
            {
                return false;
            }
            if (this.Mode != t.Mode)
            {
                return false;
            }
            if (DCUtils.EqualsStringExt(this.ArrayAndPointerSettings, t.ArrayAndPointerSettings) == false)
            {
                return false;
            }
            int num1 = this.GenericParamters == null ? 0 : this.GenericParamters.Count;
            int num2 = t.GenericParamters == null ? 0 : t.GenericParamters.Count;
            if (num1 != num2)
            {
                return false;
            }
            if (num1 > 0)
            {
                for (int iCount = 0; iCount < num1; iCount++)
                {
                    if (this.GenericParamters[iCount].EqualsValue(t.GenericParamters[iCount]) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public DCILTypeMode Mode = DCILTypeMode.Primitive;
        /// <summary>
        /// 是否为本地类型
        /// </summary>
        public bool IsLocalType
        {
            get
            {
                if (this.LibraryName == null || this.LibraryName.Length == 0)
                {
                    if (this.Mode == DCILTypeMode.Class
                        || this.Mode == DCILTypeMode.ValueType
                        || this.Mode == DCILTypeMode.NotSpecify)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsPrimitive
        {
            get
            {
                return this.Mode == DCILTypeMode.Primitive;
            }
        }

        public DCILTypeReference(string name)
        {
            this.Name = name;
            this.Mode = DCILTypeMode.NotSpecify;
        }
        public const string ClassMode_NotSpecify = "NotSpecify";

        private DCILTypeReference(string firstWord, DCILReader reader)
        {
            if (firstWord == "class" || firstWord == "valuetype" || firstWord == ClassMode_NotSpecify)
            {
                if (firstWord == "class")
                {
                    this.Mode = DCILTypeMode.Class;
                }
                else if (firstWord == "valuetype")
                {
                    this.Mode = DCILTypeMode.ValueType;
                }
                else
                {
                    this.Mode = DCILTypeMode.NotSpecify;
                }
                ReadName(reader);
                int pos = reader.Position;
                string word = reader.ReadWord();
                if (word == "<")
                {
                    this.GenericParamters = new List<DCILTypeReference>();
                    while (reader.HasContentLeft())
                    {
                        word = reader.ReadWord();
                        if (word == ">")
                        {
                            break;
                        }
                        else if (IsStartWord(word))
                        {
                            var subType = Load(word, reader);
                            this.GenericParamters.Add(subType);
                        }
                    }
                }
                else
                {
                    reader.Position = pos;
                }
            }
            else if (firstWord == "native")
            {
                this.Name = reader.ReadWord();
                this.Mode = DCILTypeMode.Native;
            }
            else if (firstWord == "unsigned")
            {
                this.Name = reader.ReadWord();
                this.Mode = DCILTypeMode.Unsigned;
            }
            else if (firstWord[0] == '!')
            {
                if (firstWord.Length > 2 && firstWord[1] == '!')
                {
                    this.Mode = DCILTypeMode.GenericTypeInMethodDefine;
                    this.Name = DCUtils.GetStringUseTable( firstWord.Substring(2));
                }
                else
                {
                    this.Mode = DCILTypeMode.GenericTypeInTypeDefine;
                    this.Name = DCUtils.GetStringUseTable(firstWord.Substring(1));
                }
            }
            this.ReadArrayAndPointerSettings(reader);

            //reader?.Cache(this);
        }

        public DCILTypeReference(string type, DCILTypeMode m = DCILTypeMode.Primitive)
        {
            this.Name = type;
            this.Mode = m;
        }

        public string NameWithLibraryName
        {
            get
            {
                if (this.LibraryName != null && this.LibraryName.Length > 0)
                {
                    return "[" + this.LibraryName + "]" + this.Name;
                }
                else
                {
                    return this.Name;
                }
            }
        }
        private void ReadName(DCILReader reader)
        {
            var strWord = reader.ReadWord();
            if (strWord == "[")
            {
                this.LibraryName = reader.ReadAfterCharExcludeLastChar(']');
                this.Name = reader.ReadWord();
            }
            else
            {
                this.Name = strWord;
            }
        }
        private string _ArrayAndPointerSettings = null;
        public string ArrayAndPointerSettings
        {
            get
            {
                return this._ArrayAndPointerSettings;
            }
            set
            {
                this._ArrayAndPointerSettings = value;
            }
        }

        public DCILTypeReference ChangeArrayAndPointerSettings(string newValue)
        {
            if (DCUtils.EqualsStringExt(this.ArrayAndPointerSettings, newValue) == false)
            {
                var item = (DCILTypeReference)this.MemberwiseClone();
                item._ArrayAndPointerSettings = newValue;
                return item;
            }
            else
            {
                return this;
            }
        }
        private bool ReadArrayAndPointerSettings(DCILReader reader)
        {
            if (reader != null)
            {
                var result = new List<char>();
                bool inArray = false;
                int len = 0;
                for (int iCount = reader.Position; iCount < reader.Length; iCount++)
                {
                    var c = reader.GetChar(iCount);
                    if (c == '*' && result.IndexOf('*') < 0)
                    {
                        result.Add(c);
                        len++;
                    }
                    else if (c == '&' && result.IndexOf('&') < 0)
                    {
                        result.Add(c);
                        len++;
                    }
                    else if (c == '[' && inArray == false)
                    {
                        result.Add(c);
                        inArray = true;
                        len++;
                    }
                    else if (c == ']' && inArray == true)
                    {
                        result.Add(c);
                        inArray = false;
                        len++;
                    }
                    else if ((c == '.' || c == ',' || char.IsNumber(c)) && inArray)
                    {
                        result.Add(c);
                        len++;
                    }
                    else if (c == ' ' && c == '\t')
                    {
                        len++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (result.Count > 0)
                {
                    reader.Position += len;
                    if (result.Count == 2 && result[0] == '[' && result[1] == ']')
                    {
                        this.ArrayAndPointerSettings = "[]";
                    }
                    else if (result.Count == 1)
                    {
                        this.ArrayAndPointerSettings = DCUtils.GetSingleCharString(result[0]);
                    }
                    else if(result.Count == len )
                    {
                        this.ArrayAndPointerSettings = reader.GetSubStringUseTable(reader.Position - len, len);
                    }
                    else
                    {
                        this.ArrayAndPointerSettings = DCUtils.GetStringUseTable(new string(result.ToArray()));
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name = null;

        public List<DCILTypeReference> GenericParamters = null;
        public string LibraryName = null;
        public bool HasLibraryName
        {
            get
            {
                return this.LibraryName != null && this.LibraryName.Length > 0;
            }
        }
        public bool IsArray
        {
            get
            {
                return this.ArrayAndPointerSettings != null
                    && this.ArrayAndPointerSettings.Length > 0
                    && this.ArrayAndPointerSettings.IndexOf('[') >= 0;
            }
        }
        public bool IsGenericType
        {
            get
            {
                return this.GenericParamters != null && this.GenericParamters.Count > 0;
            }
        }
        private string FixTypeName(string name, bool useShortName)
        {
            if (useShortName && name != null && name.Length > 0 )
            {
                return DCUtils.GetShortName(name);
            }
            return name;
        }
        public void WriteToForSignString(DCILWriter writer, DCILGenericParamterList gps = null, int stackLevel = 0 , bool useShortName = false )
        {
            if (stackLevel > 20)
            {

            }

            switch (this.Mode)
            {
                case DCILTypeMode.Primitive:
                    writer.Write(this._NativeType.Name);
                    //writer.Write( FixTypeName( this.Name , useShortName));
                    break;
                case DCILTypeMode.GenericTypeInMethodDefine:
                    {
                        bool find = false;
                        if (gps != null && gps.Count > 0)
                        {
                            foreach (var gp in gps)
                            {
                                if (gp.DefineInClass == false && gp.Name == this.Name)
                                {
                                    if (gp.RuntimeType != null && gp.RuntimeType.IsGenericType == false)
                                    {
                                        gp.RuntimeType.WriteToForSignString(writer, gps ,stackLevel +1, useShortName);
                                    }
                                    else
                                    {
                                        writer.Write("!!" + gp.Index);
                                    }
                                    find = true;
                                    break;
                                }
                            }
                        }
                        if (find == false)
                        {
                            writer.Write("!!" + this.Name);
                        }
                    }
                    break;
                case DCILTypeMode.GenericTypeInTypeDefine:
                    {
                        bool find = false;
                        if (gps != null && gps.Count > 0)
                        {
                            foreach (var gp in gps)
                            {
                                if (gp.DefineInClass == true && gp.Name == this.Name)
                                {
                                    if (gp.RuntimeType != null
                                        && gp.RuntimeType.Mode != DCILTypeMode.GenericTypeInMethodDefine
                                        && gp.RuntimeType.Mode != DCILTypeMode.GenericTypeInTypeDefine)
                                    {
                                        gp.RuntimeType.WriteToForSignString(writer, gps, stackLevel + 1 , useShortName);
                                    }
                                    else
                                    {
                                        writer.Write("!" + gp.Index);
                                    }
                                    find = true;
                                    break;
                                }
                            }
                        }
                        if (find == false)
                        {
                            writer.Write("!" + this.Name);
                        }
                    }
                    break;
                case DCILTypeMode.Class:
                    if (this.LocalClass != null)
                    {
                        if (useShortName)
                        {
                            writer.Write(FixTypeName(this.LocalClass.Name, true ));
                        }
                        else
                        {
                            writer.Write(this.LocalClass.GetNameWithNested('.'));
                        }
                    }
                    else
                    {
                        writer.Write(FixTypeName(this.Name, useShortName));
                    }
                    break;
                case DCILTypeMode.ValueType:
                    writer.Write( FixTypeName( this.Name , useShortName ));
                    break;
                case DCILTypeMode.NotSpecify:
                    writer.Write( FixTypeName( this.Name , useShortName ));
                    break;
                case DCILTypeMode.Native:
                    writer.Write("native " + this.Name);
                    break;
                case DCILTypeMode.Unsigned:
                    writer.Write("unsigned " + this.Name);
                    break;
            }
            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
            {

                writer.Write("<");
                for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    this.GenericParamters[iCount].WriteToForSignString(writer, gps , stackLevel , useShortName );
                }
                writer.Write(">");
            }
            if (this.ArrayAndPointerSettings != null
                && this.ArrayAndPointerSettings.Length > 0)
            {
                writer.Write(this.ArrayAndPointerSettings);
            }
        }

        public void WriteTo(DCILWriter writer, bool writeHeader = true, bool writeLibraryName = true)
        {
            if (this.Mode == DCILTypeMode.Primitive)
            {
                WriteName(writer, writeLibraryName);
            }
            else if (this.Mode == DCILTypeMode.Class
                || this.Mode == DCILTypeMode.ValueType
                || this.Mode == DCILTypeMode.NotSpecify)
            {
                if (writeHeader)
                {
                    if (this.Mode == DCILTypeMode.Class)
                    {
                        writer.Write("class ");
                    }
                    else if (this.Mode == DCILTypeMode.ValueType)
                    {
                        writer.Write("valuetype ");
                    }
                }
                WriteName(writer, writeLibraryName);
                if (this.GenericParamters != null && this.GenericParamters.Count > 0)
                {
                    writer.Write('<');
                    for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                    {
                        if (iCount > 0)
                        {
                            writer.Write(',');
                        }
                        this.GenericParamters[iCount].WriteTo(writer);
                        //writer.Write(this.GenericParamters[iCount].ToString());
                    }
                    writer.Write('>');
                }
            }
            else if (this.Mode == DCILTypeMode.Native)
            {
                writer.Write("native " + this.Name);
            }
            else if (this.Mode == DCILTypeMode.Unsigned)
            {
                writer.Write("unsigned " + this.Name);
            }
            else if (this.Mode == DCILTypeMode.GenericTypeInMethodDefine)
            {
                writer.Write("!!" + this.Name);
            }
            else if (this.Mode == DCILTypeMode.GenericTypeInTypeDefine)
            {
                //if( this.Name == "TKey2")
                //{

                //}
                writer.Write("!" + this.Name);
            }
            else
            {
                throw new Exception("未知类型" + this.Mode);
            }
            if (this.ArrayAndPointerSettings != null && this.ArrayAndPointerSettings.Length > 0)
            {
                writer.Write(this.ArrayAndPointerSettings);
            }
        }

        private void WriteName(DCILWriter writer, bool writeLibraryName)
        {
            if (this.LocalClass != null)
            {
                var nwn = this.LocalClass.NameWithNested;
                //if(nwn.StartsWith("'<PrivateImplementationDetails>'"))
                //{

                //}
                writer.Write(nwn);
            }
            else
            {
                if (writeLibraryName
                    && this.LibraryName != null
                    && this.LibraryName.Length > 0)
                {
                    writer.Write('[');
                    writer.Write(this.LibraryName);
                    writer.Write(']');
                }
                //if(this.Name.StartsWith("'<PrivateImplementationDetails>'"))
                //{

                //}
                writer.Write(this.Name);
            }
        }
        public override string ToString()
        {
            var str = new StringBuilder();
            var w = new DCILWriter(str);
            this.WriteTo(w);
            var resut = str.ToString();
            return resut;
        }
    }

    internal class DCILMethodParamter : IDisposable
    {
        public static bool EqualsList(
            List<DCILMethodParamter> ps1,
            List<DCILMethodParamter> ps2,
            bool includeName,
            bool checkDefaultValue)
        {
            int len1 = ps1 == null ? 0 : ps1.Count;
            int len2 = ps2 == null ? 0 : ps2.Count;
            if (len1 != len2)
            {
                return false;
            }
            if (len1 > 0)
            {
                for (int iCount = 0; iCount < len1; iCount++)
                {
                    if (ps1[iCount].EqualsValue(ps2[iCount], includeName, checkDefaultValue) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //public List<DCILMethodParamter> CloneList( List<DCILMethodParamter > ps)
        //{
        //    if( ps != null )
        //    {
        //        var ps2 = new List<DCILMethodParamter>();
        //        foreach( var p in ps )
        //        {
        //            ps2.Add(p.Clone());
        //        }
        //        return ps2;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        public void Dispose()
        {
            this.DefaultValue = null;
            this.Marshal = null;
            this.Name = null;
            this.ValueType = null;
        }
        public DCILMethodParamter Clone()
        {
            return (DCILMethodParamter)this.MemberwiseClone();
        }
        public bool IsOut = false;
        public bool IsIn = false;

        public string Name = null;
        public DCILTypeReference ValueType = null;
        public string DefaultValue = null;
        public string Marshal = null;
        public static void CacheTypeReference(DCILDocument document, List<DCILMethodParamter> ps)
        {
            if (ps != null)
            {
                foreach (var p in ps)
                {
                    p.ValueType = document.CacheTypeReference(p.ValueType);
                }
            }
        }
        internal int ComputeHashCode(bool includeName)
        {
            int result = 0;
            if (this.IsOut)
            {
                result = 1;
            }
            if (this.IsIn)
            {
                result = result + 2;
            }
            if (includeName && this.Name != null && this.Name.Length > 0)
            {
                result += this.Name.GetHashCode();
            }
            if (this.ValueType != null)
            {
                result += this.ValueType.GetHashCode();
            }
            if (this.DefaultValue != null && this.DefaultValue.Length > 0)
            {
                result += this.DefaultValue.GetHashCode();
            }
            if (this.Marshal != null && this.Marshal.Length > 0)
            {
                result += this.Marshal.GetHashCode();
            }
            return result;
        }
        internal bool EqualsValue(DCILMethodParamter p, bool includeName, bool checkDefaultValue)
        {
            if (p == null)
            {
                return false;
            }
            if (p == this)
            {
                return true;
            }
            if (checkDefaultValue && DCUtils.EqualsStringExt(this.DefaultValue, p.DefaultValue) == false)
            {
                return false;
            }
            if (this.IsIn != p.IsIn
                || this.IsOut != p.IsOut
                || DCUtils.EqualsStringExt(this.Marshal, p.Marshal) == false)
            {
                return false;
            }
            if (DCILTypeReference.StaticEquals(this.ValueType, p.ValueType) == false)
            {
                return false;
            }
            if (includeName && DCUtils.EqualsStringExt(this.Name, p.Name) == false)
            {
                return false;
            }
            return true;
        }
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(this.ValueType.ToString());
            if (this.Name != null && this.Name.Length > 0)
            {
                str.Append(' ');
                str.Append(this.Name);
            }
            return str.ToString();
        }
        public static List<DCILMethodParamter> ReadParameters(DCILReader reader, bool readName = true)
        {
            var paramters = new List<DCILMethodParamter>();
            DCILMethodParamter mp = null;
            while (reader.HasContentLeft())
            {
                var word = reader.ReadWord();
                if (word == ")")
                {
                    break;
                }
                if (word == "(")
                {
                    continue;
                }
                //if (readName == false)
                //{

                //}
                if (mp == null)
                {
                    mp = new DCILMethodParamter();
                    paramters.Add(mp);
                }
                if (word == "[")
                {
                    word = reader.ReadAfterCharExcludeLastChar(']', true);
                    if (word == "in")
                    {
                        mp.IsIn = true;
                    }
                    else if (word == "out")
                    {
                        mp.IsOut = true;
                    }
                    continue;
                }
                if (DCILTypeReference.IsStartWord(word))
                {
                    mp.ValueType = DCILTypeReference.Load(word, reader);
                    word = reader.ReadWord();
                    if (word == ")")
                    {
                        break;
                    }
                }
                if (word == ",")
                {
                    mp = new DCILMethodParamter();
                    paramters.Add(mp);
                    continue;
                }
                else if (word == "marshal")
                {
                    mp.Marshal = reader.ReadStyleExtValue();
                    mp.Name = reader.ReadWord();
                }
                else if (readName)
                {
                    if (mp.Name == null)
                    {
                        mp.Name = word;
                    }
                }
            }
            if (paramters.Count > 0)
            {
                //foreach (var item in paramters)
                //{
                //    if (item.ValueType == null)
                //    {

                //    }
                //}
                return paramters;
            }
            else
            {
                return null;
            }
        }
        public static void WriteTo(List<DCILMethodParamter> parameters, DCILWriter writer, bool forSignString)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (forSignString)
                {
                    writer.Write("(");
                }
                else
                {
                    writer.WriteLine("(");
                    writer.ChangeIndentLevel(1);
                }
                for (int iCount = 0; iCount < parameters.Count; iCount++)
                {
                    var p = parameters[iCount];
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    if (p.IsIn)
                    {
                        writer.Write("[in] ");
                    }
                    if (p.IsOut)
                    {
                        writer.Write("[out] ");
                    }
                    if (p.DefaultValue != null && p.DefaultValue.Length > 0)
                    {
                        writer.Write("[opt] ");
                    }
                    if (forSignString)
                    {
                        p.ValueType.WriteTo(writer, true, false);
                    }
                    else
                    {
                        p.ValueType.WriteTo(writer);
                    }
                    if (p.Marshal != null && p.Marshal.Length > 0)
                    {
                        writer.Write(" marshal( " + p.Marshal + ") ");
                    }
                    if (forSignString == false)
                    {
                        writer.Write("  " + p.Name);
                    }
                }
                writer.Write(")");
                if (forSignString == false)
                {
                    writer.ChangeIndentLevel(-1);
                }
            }
            else
            {
                writer.Write("()");
            }
        }
    }
    internal class DCILMethod : DCILMemberInfo
    {
        public bool ObfuscateControlFlowFlag = false;
        /// <summary>
        /// 系统内部自动产生的
        /// </summary>
        public bool InnerGenerate = false;

        public static readonly IComparer<DCILMethod> ComparerByName = new NameCompaer(false);

        internal class NameCompaer : IComparer<DCILMethod>
        {
            public NameCompaer(bool comareClassName)
            {
                this._CompareClassName = comareClassName;
            }
            private bool _CompareClassName = false;

            public int Compare(DCILMethod x, DCILMethod y)
            {
                if (this._CompareClassName)
                {
                    int v = string.Compare(x.OwnerClass?.Name, y.OwnerClass?.Name);
                    if (v != 0)
                    {
                        return v;
                    }
                }
                return string.Compare(x.Name, y.Name);
            }
        }
        public static readonly HashSet<string> PredefinedAttributes = null;
        static DCILMethod()
        {
            PredefinedAttributes = new HashSet<string>();
            PredefinedAttributes.Add("abstract");
            PredefinedAttributes.Add("assembly");
            PredefinedAttributes.Add("compilercontrolled");
            PredefinedAttributes.Add("famandassem");
            PredefinedAttributes.Add("family");
            PredefinedAttributes.Add("famorassem");
            PredefinedAttributes.Add("final");
            PredefinedAttributes.Add("hidebysig");
            PredefinedAttributes.Add("newslot");
            PredefinedAttributes.Add("private");
            PredefinedAttributes.Add("public");
            PredefinedAttributes.Add("rtspecialname");
            PredefinedAttributes.Add("specialname");
            PredefinedAttributes.Add("static");
            PredefinedAttributes.Add("virtual");
            PredefinedAttributes.Add("strict");
            PredefinedAttributes.Add("pinvokeimpl");
            PredefinedAttributes.Add("instance");
        }

        public static bool IsCtorOrCctor(string methodName)
        {
            return methodName == ".ctor" || methodName == ".cctor";
        }
        public const string TagName = ".method";

        public static readonly DCILMethod Empty = new DCILMethod();

        public DCILMethod(DCILClass parent, DCILReader reader)
        {
            this.Parent = parent;
            this.Load(reader);
            this.HasGenericStyle = GetHasGenericStyle();

        }

        private static readonly List<string> _NewLabelIDList = new List<string>();
        public static void ClearNewLabelIDCache()
        {
            _NewLabelIDList.Clear();
        }
        private int _NewLabelIndex = 0;
        /// <summary>
        /// 创建新的指令行标记
        /// </summary>
        /// <returns>新的标记</returns>
        public string GenNewLabelID()
        {
            this._NewLabelIndex++;
            if (this._NewLabelIndex >= _NewLabelIDList.Count)
            {
                for (int iCount = _NewLabelIDList.Count; iCount <= this._NewLabelIndex; iCount++)
                {
                    _NewLabelIDList.Add("IL_N" + iCount.ToString("0000"));
                    //if (iCount < 100)
                    //{
                    //    _NewLabelIDList.Add("IL_N" + iCount.ToString("000"));
                    //}
                    //else
                    //{
                    //    _NewLabelIDList.Add("IL_N" + iCount);
                    //}
                }
            }
            return _NewLabelIDList[this._NewLabelIndex];
        }

        public void EnumOperCodes(EnumOperCodeHandler handler )
        {
            if( this.OperCodes != null &&this.OperCodes.Count > 0 )
            {
                this.OperCodes.EnumDeeply(this, handler);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.RuntimeSwitchs = null;
            if(this.GenericParamters != null )
            {
                this.GenericParamters.Dispose();
                this.GenericParamters = null;
            }
            if(this.Locals != null )
            {
                this.Locals.Dispose();
                this.Locals = null;
            }
            if(this.Parameters != null )
            {
                foreach( var item in this.Parameters )
                {
                    item.Dispose();
                }
                this.Parameters.Clear();
                this.Parameters = null;
            }
            this.ParentMember = null;
            this.permission = null;
            this.permissionset = null;
            this.Pinvokeimpl = null;
            this.ReturnMarshal = null;
            this.ReturnType = null;
            this.ReturnTypeInfo = null;
            this.RuntimeSwitchs = null;
            this.Type = null;
            this._NativeDecilaryTypeName = null;
            this._NativeMethod = null;
            if(this._Override != null )
            {
                this._Override.Dispose();
                this._Override = null;
            }
            this._SignString = null;
            
        }
        /// <summary>
        /// 运行时使用的开关
        /// </summary>
        public JieJieSwitchs RuntimeSwitchs = null;

        public DCILTypeReference GetResultValueTypeForLoad(DCILOperCode code)
        {
            if (code == null || code.OperCode == null || code.OperCode.Length == 0)
            {
                return null;
            }
            if (code.OperCodeValue == DCILOpCodeValue.Ldfld
                || code.OperCodeValue == DCILOpCodeValue.Ldsfld)
            {
                if (code is DCILOperCode_HandleField)
                {
                    var rf = ((DCILOperCode_HandleField)code).Value?.ValueType;
                    return rf;
                }
                return null;
            }
            if ( code.OperCode.StartsWith("ldarg" , StringComparison.Ordinal)
                || code.OperCode.StartsWith("ldloc" , StringComparison.Ordinal)
                )
            {
                int pIndex = -1;
                int locIndex = -1;
                if (code.OperCodeValue == DCILOpCodeValue.Ldarg 
                    || code.OperCodeValue == DCILOpCodeValue.Ldarg_S)
                {
                    try
                    {
                        if (this.Parameters != null)
                        {
                            foreach (var item in this.Parameters)
                            {
                                if (item.Name == code.OperData)
                                {
                                    return item.ValueType;
                                }
                            }
                        }
                        pIndex = DCUtils.ConvertToInt32(code.OperData);
                    }
                    catch
                    {
                        
                        return null;
                    }
                }
                else if (code.OperCodeValue == DCILOpCodeValue.Ldloc 
                    || code.OperCodeValue == DCILOpCodeValue.Ldloc_S)
                {
                    try
                    {
                        if (this.Locals != null)
                        {
                            foreach (var item in this.Locals)
                            {
                                if (item.Name == code.OperData)
                                {
                                    return item.ValueType;
                                }
                            }
                        }
                        locIndex = DCUtils.ConvertToInt32(code.OperData);
                    }
                    catch
                    {
                        return null;
                    }
                }
                
                else
                {
                    switch (code.OperCodeValue)
                    {
                        case DCILOpCodeValue.Ldarg_0: pIndex = 0; break;
                        case DCILOpCodeValue.Ldarg_1: pIndex = 1; break;
                        case DCILOpCodeValue.Ldarg_2: pIndex = 2; break;
                        case DCILOpCodeValue.Ldarg_3: pIndex = 3; break;

                        case DCILOpCodeValue.Ldloc_0: locIndex = 0; break;
                        case DCILOpCodeValue.Ldloc_1: locIndex = 1; break;
                        case DCILOpCodeValue.Ldloc_2: locIndex = 2; break;
                        case DCILOpCodeValue.Ldloc_3: locIndex = 3; break;
                    }
                }
                if (pIndex >= 0 && this.Parameters != null && pIndex < this.Parameters.Count)
                {
                    return this.Parameters[pIndex].ValueType;
                }
                if (locIndex >= 0 && this.Locals != null && locIndex < this.Locals.Count)
                {
                    return this.Locals[locIndex].ValueType;
                }
            }
            return null;
        }

        public DCILTypeReference GetTargetValueTypeForSet(DCILOperCode code)
        {
            if (code == null || code.OperCode == null || code.OperCode.Length == 0)
            {
                return null;
            }
            if (code.OperCodeValue == DCILOpCodeValue.Stfld
                || code.OperCodeValue == DCILOpCodeValue.Stsfld)
            {
                if (code is DCILOperCode_HandleField)
                {
                    var rf = ((DCILOperCode_HandleField)code).Value?.ValueType;
                    return rf;
                }
                return null;
            }
            if(code.OperCodeValue == DCILOpCodeValue.Starg 
                || code.OperCodeValue == DCILOpCodeValue.Starg_S)
            {
                if (this.Parameters != null && this.Parameters.Count > 0)
                {
                    foreach (var item in this.Parameters)
                    {
                        if (item.Name == code.OperData)
                        {
                            return item.ValueType;
                        }
                    }
                    try
                    {
                        int index = DCUtils.ConvertToInt32(code.OperData);
                        if (index >= 0 && index < this.Parameters.Count)
                        {
                            return this.Parameters[index].ValueType;
                        }
                    }
                    catch
                    {
                    }
                }
                return null;
            }
            if (code.OperCode.StartsWith("stloc", StringComparison.Ordinal)
                && this.Locals != null 
                && this.Locals.Count > 0 )
            {
                if (code.OperCodeValue == DCILOpCodeValue.Stloc 
                    || code.OperCodeValue == DCILOpCodeValue.Stloc_S)
                {
                    foreach (var item in this.Locals)
                    {
                        if (item.Name == code.OperData)
                        {
                            return item.ValueType;
                        }
                    }
                    try
                    {
                        int index = DCUtils.ConvertToInt32(code.OperData);
                        if (index >= 0 && index < this.Locals.Count)
                        {
                            return this.Locals[index].ValueType;
                        }
                    }
                    catch
                    {

                    }
                    return null;
                }
                else
                {
                    int index = -1;
                    if (code.OperCodeValue == DCILOpCodeValue.Stloc_0) index = 0;
                    else if (code.OperCodeValue == DCILOpCodeValue.Stloc_1) index = 1;
                    else if (code.OperCodeValue == DCILOpCodeValue.Stloc_2) index = 2;
                    else if (code.OperCodeValue == DCILOpCodeValue.Stloc_3) index = 3;
                    if(index >= 0 && index < this.Locals.Count )
                    {
                        return this.Locals[index].ValueType;
                    }
                }
            }
            
            return null;
        }

        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Method;
            }
        }

        public DCILMethod SimpleClone()
        {
            var method = (DCILMethod)this.MemberwiseClone();
            if (this.Styles != null)
            {
                method.Styles = new List<string>(this.Styles);
            }
            return method;
        }

        public bool FlagsForPrivate = true;

        public int IndexOfExtLocals = int.MinValue;
        /// <summary>
        /// 是否处于某个重载链条中
        /// </summary>
        public bool IsInOverrideList = false;

        public  System.Reflection.MethodInfo _NativeMethod = null;
        public string _NativeDecilaryTypeName = null;
        public DCILMethod(
            System.Reflection.MethodInfo method,
            DCILDocument document,
            DCILGenericParamterList typeGps) : base(method)
        {
            this.IsFromInterface = method.DeclaringType.IsInterface;
            this._NativeMethod = method;
            var declaringType = method.DeclaringType;
            this._NativeDecilaryTypeName = DCUtils.GetFullName(method.DeclaringType);
            this.ReturnTypeInfo = CreateFromNative(declaringType, method, method.ReturnType, document);
            if (this.ReturnTypeInfo != null && typeGps != null)
            {
                this.ReturnTypeInfo = this.ReturnTypeInfo.Transform(typeGps);
            }
            this.Styles = new List<string>();
            this.IsPublic = method.IsPublic;
            this.IsStatic = method.IsStatic;
            this.IsVirtual = method.IsVirtual;
            this.IsSpecialname = method.IsSpecialName;
            this.IsAbstract = method.IsAbstract;
            this.IsFinal = method.IsFinal;

            if (method.IsGenericMethod)
            {
                var gps = method.GetGenericArguments();
                this.GenericParamters = new DCILGenericParamterList(gps.Length);
                foreach (var gp in gps)
                {
                    var dcgp = new DCILGenericParamter(gp.Name, false);
                    this.GenericParamters.Add(dcgp);
                }
            }
            var ps = method.GetParameters();
            if (ps != null && ps.Length > 0)
            {
                this.Parameters = new List<DCILMethodParamter>(ps.Length);
                foreach (var p in ps)
                {
                    var dcp = new DCILMethodParamter();
                    dcp.Name = p.Name;
                    dcp.IsIn = p.IsIn;
                    dcp.IsOut = p.IsOut;
                    var pt2 = p.ParameterType;

                    dcp.ValueType = CreateFromNative(declaringType, method, pt2, document);
                    if (dcp.ValueType != null && typeGps != null)
                    {
                        dcp.ValueType = dcp.ValueType.Transform(typeGps);
                    }

                    if (p.DefaultValue != null && DBNull.Value.Equals(p.DefaultValue) == false)
                    {
                        dcp.DefaultValue = p.DefaultValue.ToString();
                    }
                    this.Parameters.Add(dcp);
                }
            }
            this.HasGenericStyle = GetHasGenericStyle();
        }

        public bool IsFromInterface = false;

        public bool IsStatic = false;


        public bool IsPublic = false;

        public bool IsVirtual = false;

        public bool IsInstance = false;

        public bool IsSpecialname = false;

        public bool IsNewslot = false;

        public bool IsFinal = false;

        public bool IsAbstract = false;
        

        public int AddExtLocalsIndex()
        {
            if (this.Locals == null)
            {
                this.Locals = new DCILMethodLocalVariableList();
            }
            var loc = new DCILMethodLocalVariable();
            loc.ValueType = DCILTypeReference.Type_Boolean;
            this.Locals.Add(loc);
            return this.Locals.Count - 1;
        }
     
        private static readonly System.Random _Random = new Random();
        /// <summary>
        /// 是否具有泛型样式
        /// </summary>
        public readonly bool HasGenericStyle = false;

        private bool GetHasGenericStyle()
        {
            if (this.GenericParamters != null
                && this.GenericParamters.Count > 0)
            {
                return true;
            }
            else if (this.ReturnTypeInfo != null
                && (this.ReturnTypeInfo.Mode == DCILTypeMode.GenericTypeInMethodDefine
                || this.ReturnTypeInfo.Mode == DCILTypeMode.GenericTypeInTypeDefine))
            {
                return true;
            }
            else if (this.Parameters != null && this.Parameters.Count > 0)
            {
                foreach (var item in this.Parameters)
                {
                    if (item.ValueType.Mode == DCILTypeMode.GenericTypeInMethodDefine
                        || item.ValueType.Mode == DCILTypeMode.GenericTypeInTypeDefine
                        || item.ValueType.IsGenericType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void ChangeName(string newName)
        {
            base.ChangeName(newName);
            this._SignString = null;
        }
        private static DCILTypeReference CreateFromNative(
            Type declaringType,
            System.Reflection.MethodInfo method,
            Type inputType,
            DCILDocument document)
        {
            string endFix = null;
            if (inputType.IsArray)
            {
                for (int iCount = 0; iCount < inputType.GetArrayRank(); iCount++)
                {
                    endFix = endFix + "[]";
                }
                inputType = inputType.GetElementType();
            }
            if (inputType.IsPointer)
            {
                endFix = "*" + endFix;
                inputType = inputType.GetElementType();
            }
            if (inputType.IsByRef)
            {
                endFix = "&" + endFix;
                inputType = inputType.GetElementType();
            }
            if (inputType.IsGenericParameter)
            {
                if (declaringType.IsGenericType)
                {
                    var gps = declaringType.GetGenericArguments();
                    if (gps != null && gps.Length > 0)
                    {
                        foreach (var item in gps)
                        {
                            if (inputType == item)
                            {
                                var result = new DCILTypeReference(item.Name, DCILTypeMode.GenericTypeInTypeDefine);
                                result = result.ChangeArrayAndPointerSettings(endFix);
                                return result;
                            }
                        }
                    }
                }
                if (method.IsGenericMethod)
                {
                    var gps = method.GetGenericArguments();
                    if (gps != null && gps.Length > 0)
                    {
                        foreach (var item in gps)
                        {
                            if (inputType == item)
                            {
                                var result = new DCILTypeReference(item.Name, DCILTypeMode.GenericTypeInMethodDefine);
                                result = result.ChangeArrayAndPointerSettings(endFix);
                                return result;
                            }
                        }
                    }
                }
            }
            var result2 = DCILTypeReference.CreateByNativeType(inputType, document);
            result2 = result2.ChangeArrayAndPointerSettings(endFix);
            return result2;
        }

        public DCILMemberInfo ParentMember = null;

        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            base.CacheInfo(document, clses);
            this.ReturnTypeInfo = document.CacheTypeReference(this.ReturnTypeInfo);
            DCILGenericParamter.CacheTypeReference(document, this.GenericParamters);
            DCILMethodParamter.CacheTypeReference(document, this.Parameters);
            if (this.Locals != null)
            {
                foreach (var item in this.Locals)
                {
                    item.ValueType = document.CacheTypeReference(item.ValueType);
                }
            }
            if (this._Override != null)
            {
                this._Override = document.CacheDCILInvokeMethodInfo(this._Override);
            }
            if (this.OperCodes != null && this.OperCodes.Count > 0)
            {
                this.OperCodesCacheInfo(this.OperCodes, document, clses);
            }
            base.CusotmAttributesCacheTypeReference(document);
        }
        private void OperCodesCacheInfo(DCILOperCodeList codes, DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            foreach (var item in codes)
            {
                if (item is DCILOperCode_HandleClass)
                {
                    ((DCILOperCode_HandleClass)item).UpdateDomState(document, clses);
                }
                else if (item is DCILOperCode_HandleField)
                {
                    var code = (DCILOperCode_HandleField)item;
                    if (code.Value != null)
                    {
                        code.Value.UpdateLocalInfo(document, clses);
                        code.CacheInfo(document);
                    }
                }
                else if (item is DCILOperCode_HandleMethod)
                {
                    ((DCILOperCode_HandleMethod)item).CacheInfo(document);
                }
                else if (item is DCILOperCode_LdToken)
                {
                    ((DCILOperCode_LdToken)item).CacheInfo(document, clses);
                }
                else if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var group = (DCILOperCode_Try_Catch_Finally)item;
                    if (group.HasTryOperCodes())
                    {
                        OperCodesCacheInfo(group._Try.OperCodes, document, clses);
                    }
                    if (group.HasCatchs())
                    {
                        foreach (var item2 in group._Catchs)
                        {
                            item2.ExcpetionType = document.CacheTypeReference(item2.ExcpetionType);
                            if (item2.OperCodes != null)
                            {
                                OperCodesCacheInfo(item2.OperCodes, document, clses);
                            }
                        }
                    }
                    if (group.HasFinallyOperCodes())
                    {
                        OperCodesCacheInfo(group._Finally.OperCodes, document, clses);
                    }
                    if (group.HasFaultOperCodes())
                    {
                        OperCodesCacheInfo(group._fault.OperCodes, document, clses);
                    }
                }
            }
        }

        /// <summary>
        /// 检查函数签名
        /// </summary>
        /// <param name="returnType">返回值类型</param>
        /// <param name="genericParamters">泛型参数</param>
        /// <param name="parameters">参数</param>
        /// <returns>是否匹配</returns>
        public bool MatchSign(
            DCILTypeReference returnType,
            List<DCILTypeReference> genericParamters,
            List<DCILMethodParamter> parameters)
        {
            if ((returnType == null) != (this.ReturnTypeInfo == null))
            {
                return false;
            }
            if (this.ReturnTypeInfo != null)
            {
                if (this.ReturnTypeInfo.EqualsValue(returnType) == false)
                {
                    return false;
                }
            }
            if (DCILGenericParamter.MatchList(this.GenericParamters, genericParamters) == false)
            {
                return false;
            }
            if (DCILMethodParamter.EqualsList(this.Parameters, parameters, false, false) == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查函数签名
        /// </summary>
        /// <param name="returnType">返回值类型</param>
        /// <param name="genericParamters">泛型参数</param>
        /// <param name="parameters">参数</param>
        /// <returns>是否匹配</returns>
        public bool MatchSign(DCILMethod method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (method == this)
            {
                return true;
            }
            if ((method.ReturnTypeInfo == null) != (this.ReturnTypeInfo == null))
            {
                return false;
            }
            if (this.ReturnTypeInfo != null)
            {
                if (this.ReturnTypeInfo.Mode == DCILTypeMode.GenericTypeInMethodDefine)
                {

                }
            }
            if (this.ReturnTypeInfo != null
                && this.ReturnTypeInfo.EqualsValue(method.ReturnTypeInfo) == false)
            {
                return false;
            }
            if (DCUtils.EqualsList<DCILGenericParamter>(this.GenericParamters, method.GenericParamters) == false)
            {
                return false;
            }
            if (DCILMethodParamter.EqualsList(this.Parameters, method.Parameters, false, false) == false)
            {
                return false;
            }
            return true;
        }
        public DCILInvokeMethodInfo _Override = null;

        public bool EntryPoint = false;
        public DCILGenericParamterList GenericParamters = null;
        public string Pinvokeimpl = null;
        public override void Load(DCILReader reader)
        {
            this.StartLineIndex = reader.CurrentLineIndex();
            if (this.StartLineIndex > 116380)
            {

            }
            this.OperCodes = new DCILOperCodeList();
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == "pinvokeimpl")
                {
                    this.Pinvokeimpl = reader.ReadStyleExtValue();
                }
                else if (DCILTypeReference.IsStartWord(strWord))
                {
                    this.ReturnTypeInfo = DCILTypeReference.Load(strWord, reader);
                    if (reader.PeekWord() == "marshal")
                    {
                        reader.ReadWord();
                        this.ReturnMarshal = reader.ReadStyleExtValue();
                    }
                    break;
                }
                else
                {
                    this.AddStyle(strWord);
                }
                if (strWord == "<" || strWord == "(")
                {
                    break;
                }
            }
            this._Name = reader.ReadWord();
            var starChar = reader.ReadContentChar();
            if (starChar == '<')
            {
                // 泛型方法,获得参数类型
                this.GenericParamters = new DCILGenericParamterList(reader, false);
                reader.MoveAfterChar('(');
                //reader.ReadAfterCharExcludeLastChar('(');
            }
            this.Parameters = DCILMethodParamter.ReadParameters(reader);
            this.DeclearEndFix = reader.ReadAfterCharExcludeLastChar('{', true);
            if (this.DeclearEndFix == "cil managed")
            {
                this.DeclearEndFix = "cil managed";
            }
            else if (this.DeclearEndFix == "cil managed preservesig")
            {
                this.DeclearEndFix = "cil managed preservesig";
            }
            InnerLoadILOperCode(this, reader);

            this.IsStatic = base.HasStyle("instance") == false;
            this.IsPublic = base.HasStyle("public");
            this.IsVirtual = base.HasStyle("virtual");
            this.IsInstance = base.HasStyle("instance");
            this.IsSpecialname = base.HasStyle("specialname");
            this.IsNewslot = base.HasStyle("newslot");
            this.IsAbstract = base.HasStyle("abstract");
            this.IsFinal = base.HasStyle("final");
        }
        
        public DCILClass OwnerClass
        {
            get
            {
                return (DCILClass)this.Parent;
            }
        }
        /// <summary>
        /// 存在特别结构的指令，无法混淆流程。
        /// </summary>
        public bool OperCodeSpecifyStructure = false;

        public string ReturnMarshal = null;
        public string permissionset = null;
        public string permission = null;
        private void InnerLoadILOperCode(DCILObject rootObject, DCILReader reader)
        {
            if (rootObject.OperCodes == null)
            {
                rootObject.OperCodes = null;
            }
            DCILOperCodeList operInfoList = rootObject.OperCodes;
            while (reader.HasContentLeft())
            {
                int pos = reader.Position;
                var strWord = reader.ReadWord();
                if (strWord.StartsWith("IL_", StringComparison.Ordinal))
                {
                    // 开始读取IL指令
                    string labelID = strWord;
                    strWord = reader.ReadWord();
                    var myDefine = reader.ReadOperCode();
                    switch (myDefine.ExtCodeType)
                    {
                        case ILOperCodeType.ldstr:
                            {
                                operInfoList.Add(new DCILOperCode_LoadString(labelID, reader));
                            }
                            break;
                        case ILOperCodeType.Method:
                            {
                                operInfoList.Add(new DCILOperCode_HandleMethod(labelID, myDefine, reader));
                            }
                            break;
                        case ILOperCodeType.Field:
                            {
                                operInfoList.Add(new DCILOperCode_HandleField(labelID, myDefine, reader));
                            }
                            break;
                        case ILOperCodeType.ldtoken:
                            {
                                operInfoList.Add(new DCILOperCode_LdToken(labelID, reader));
                            }
                            break;
                        case ILOperCodeType.Class:
                            {
                                operInfoList.Add(new DCILOperCode_HandleClass(labelID, myDefine, reader));
                            }
                            break;
                        case ILOperCodeType.switch_:
                            {
                                operInfoList.Add(new DCILOperCode_Switch(labelID, reader));
                            }
                            break;
                        case ILOperCodeType.Jump:
                            {
                                var strOperData = reader.ReadWord();
                                operInfoList.AddItem(labelID, myDefine, strOperData);
                                reader.MoveNextLine();
                            }
                            break;
                        case ILOperCodeType.JumpShort:
                            {
                                var strOperData = reader.ReadWord();
                                operInfoList.AddItem(labelID, myDefine, strOperData);
                                reader.MoveNextLine();
                            }
                            break;
                        case ILOperCodeType.ArgsOrLocalsByName:
                            {
                                var strOperData = reader.ReadWord();
                                operInfoList.AddItem(labelID, myDefine, strOperData);
                                reader.MoveNextLine();
                            }
                            break;
                        case ILOperCodeType.Nop:
                            {
                                operInfoList.AddItem(labelID, myDefine);
                                reader.MoveNextLine();
                            }
                            break;
                        default:
                            {
                                if( myDefine.WithoutOperData)// DCILOperCode.WithoutOperData( strOperCode))
                                {
                                    operInfoList.AddItem(labelID, myDefine);
                                    reader.MoveNextLine();
                                    break;
                                }
                                if(myDefine.ExtCodeType == ILOperCodeType.LoadNumberByOperData )
                                {
                                    reader.SkipLineWhitespace();
                                    var nc = reader.Peek();
                                    if( nc >='0' && nc <='9')
                                    {
                                        var strOperData2 = reader.ReadWord();
                                        operInfoList.AddItem(labelID, myDefine, strOperData2);
                                        reader.MoveNextLine();
                                        break;
                                    }
                                    else
                                    {

                                    }
                                }
                                var strOperData = reader.ReadLineTrim();
                                if( myDefine.Value == DCILOpCodeValue.Ldc_R8)// strOperCode == "ldc.r8")
                                {
                                    //var sss = DCUtils.ToHexString(BitConverter.GetBytes(float.NaN));
                                    if ( strOperData == "-nan(ind)")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(double.NaN));// "(00 00 00 00 00 00 F8 FF)";
                                    }
                                    else if( strOperData == "-inf")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(double.NegativeInfinity));// "(00 00 00 00 00 00 F0 FF)";
                                    }
                                    else if (strOperData == "inf")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(double.PositiveInfinity)); //"(00 00 00 00 00 00 F0 7F)";
                                    }
                                }
                                else if(myDefine.Value == DCILOpCodeValue.Ldc_R4)//strOperCode == "ldc.r4")
                                {
                                    if (strOperData == "-nan(ind)")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(float.NaN));// "(00 00 00 00 00 00 F8 FF)";
                                    }
                                    else if (strOperData == "-inf")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(float.NegativeInfinity));// "(00 00 00 00 00 00 F0 FF)";
                                    }
                                    else if (strOperData == "inf")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(float.PositiveInfinity)); //"(00 00 00 00 00 00 F0 7F)";
                                    }
                                }
                                operInfoList.AddItem(labelID, myDefine , strOperData);
                            }
                            break;
                    }

                    reader.NumOfOperCode++;
                }
                else if (strWord == ".try"
                    || strWord == "catch"
                    || strWord == "finally"
                    || strWord == "fault")
                {

                    DCILOperCode_Try_Catch_Finally tryBlock = null;
                    if (strWord == ".try")
                    {
                        tryBlock = new DCILOperCode_Try_Catch_Finally();
                        operInfoList.Add(tryBlock);
                        string word4 = reader.PeekWord();
                        if (word4.StartsWith("IL"))
                        {
                            tryBlock.SingleLineContent = reader.ReadLine();
                            this.OperCodeSpecifyStructure = true;
                        }
                        else
                        {
                            tryBlock._Try = new DCILObject();
                            tryBlock._Try._Name = ".try";
                            tryBlock._Try.OperCodes = new DCILOperCodeList();
                            tryBlock._Try.Parent = rootObject;
                            InnerLoadILOperCode(tryBlock._Try, reader);
                        }
                    }
                    else if (strWord == "catch")
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList[operInfoList.Count - 1];
                        var catch2 = new DCILCatchBlock();
                        catch2.ExcpetionType = DCILTypeReference.Load("class", reader);
                        catch2.Parent = rootObject;
                        if (tryBlock._Catchs == null)
                        {
                            tryBlock._Catchs = new List<DCILCatchBlock>();
                        }
                        tryBlock._Catchs.Add(catch2);
                        InnerLoadILOperCode(catch2, reader);
                    }
                    else if (strWord == "fault")
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList[operInfoList.Count - 1];
                        tryBlock._fault = new DCILObject();
                        tryBlock._fault._Name = "fault";
                        tryBlock._fault.OperCodes = new DCILOperCodeList();
                        tryBlock._fault.Parent = rootObject;
                        InnerLoadILOperCode(tryBlock._fault, reader);
                    }
                    else // finally
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList[operInfoList.Count - 1];
                        tryBlock._Finally = new DCILObject();
                        tryBlock._Finally._Name = "finally";
                        tryBlock._Finally.OperCodes = new DCILOperCodeList();
                        tryBlock._Finally.Parent = rootObject;
                        InnerLoadILOperCode(tryBlock._Finally, reader);
                    }
                    reader.NumOfOperCode++;
                }
                else if (strWord == ".maxstack")
                {
                    this.Maxstack = Convert.ToInt32(reader.ReadLine());
                }
                else if (strWord == ".override")
                {
                    strWord = reader.ReadWord();
                    if (strWord == "method")
                    {
                        this._Override = new DCILInvokeMethodInfo(reader);
                    }
                    else
                    {
                        reader.Position -= strWord.Length;

                        this._Override = new DCILInvokeMethodInfo(reader, true);
                        this._Override.ReturnType = this.ReturnTypeInfo;
                        if (this.Parameters != null && this.Parameters.Count > 0)
                        {
                            this._Override.Paramters = new List<DCILMethodParamter>(this.Parameters);

                        }
                    }
                }
                else if (strWord == DCILCustomAttribute.TagName_custom)
                {
                    this.ReadCustomAttribute(reader);
                }
                else if (strWord == ".param")
                {
                    var line = reader.ReadLine();
                    int index = line.IndexOf('=');
                    if (index > 0)
                    {
                        var pIndex = DCILReader.ParseArrayIndex(line.Substring(0, index));
                        if (pIndex >= 0 && pIndex <= this.Parameters.Count)
                        {
                            this.Parameters[pIndex - 1].DefaultValue = line.Substring(index + 1).Trim();
                        }
                    }
                }
                else if (strWord == ".locals")
                {
                    // see topic # 15.4.1.3	The .locals directive
                    this.Locals = new DCILMethodLocalVariableList();
                    strWord = reader.ReadWord();
                    if (strWord == "init")
                    {
                        this.Locals.HasInit = true;
                        strWord = reader.ReadWord();
                    }
                    else
                    {
                        this.Locals.HasInit = false;
                    }
                    if (strWord == "(")
                    {
                        while (reader.HasContentLeft())
                        {
                            var pinfo = new DCILMethodLocalVariable();
                            int pindex = int.MinValue;
                            char startChar = reader.PeekContentChar();
                            if (startChar == '[')
                            {
                                reader.ReadContentChar();
                                var strIndex = reader.ReadAfterChar(']').Trim();
                                if (strIndex.Length > 0 && int.TryParse(strIndex, out pindex))
                                {
                                    pinfo.Index = pindex;
                                }
                            }
                            strWord = reader.ReadWord();
                            if (DCILTypeReference.IsStartWord(strWord))
                            {
                                pinfo.ValueType = DCILTypeReference.Load(strWord, reader);
                                this.Locals.Add(pinfo);
                            }
                            char endChar = char.MinValue;
                            pinfo.Name = DCUtils.GetStringUseTable( reader.ReadAfterCharsExcludeLastChar(",)", out endChar).Trim() );

                            if (endChar == ')')
                            {
                                break;
                            }
                        }
                    }
                }
                else if (strWord == ".permissionset")
                {
                    this.permissionset = reader.ReadInstructionContent();
                }
                else if (strWord == ".permission")
                {
                    this.permission = reader.ReadInstructionContent();
                }
                else if (strWord == ".entrypoint")
                {
                    this.EntryPoint = true;
                }
                else if (strWord == "}")
                {
                    // 结束代码组
                    break;
                }
            }
        }
        public override string GetSignatureForMap()
        {
            string mn = this.OwnerClass.GetNameWithNested('.') + "." + this.Name;
            mn = mn + GetParamterListString(false );
            return mn;
        }

        private string _SignString = null;
        public virtual string GetSignString()
        {
            if (this.HasGenericStyle || this._SignString == null)
            {
                this._SignString = InnerGetSignString(this.ReturnTypeInfo, this._Name, this.GenericParamters, ((DCILClass)this.Parent)?.GenericParamters, this.Parameters);
                this._SignString = DCUtils.GetStringUseTable(this._SignString);
            }
            return this._SignString;
        }

        /// <summary>
        /// 获得方法签名字符串
        /// </summary>
        /// <param name="returnType">返回值</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="methodGps">方法使用的泛型参数值</param>
        /// <param name="classGps">方法所属的类型的泛型参数</param>
        /// <param name="ps">方法参数</param>
        /// <returns>字符串</returns>
        public static string InnerGetSignString(
            DCILTypeReference returnType,
            string methodName,
            List<DCILTypeReference> methodGps,
            DCILGenericParamterList classGps,
            List<DCILMethodParamter> ps)
        {
            var str = new StringBuilder();
            var writer = new DCILWriter(str);
            returnType.WriteToForSignString(writer, classGps);
            writer.Write(" ");
            if (methodName != null)
            {
                writer.Write(methodName);
            }
            if (methodGps != null && methodGps.Count > 0)
            {
                writer.Write("<");
                for (int iCount = 0; iCount < methodGps.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    methodGps[iCount].WriteToForSignString(writer, classGps);
                }
                writer.Write(">");
            }
            writer.Write("(");
            if (ps != null && ps.Count > 0)
            {
                for (int iCount = 0; iCount < ps.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    ps[iCount].ValueType.WriteToForSignString(writer, classGps);
                }
            }
            writer.Write(")");
            return str.ToString();
        }

        public string GetParamterListString( bool userShortName , bool outputParameterName = true )
        {
            var writer = new DCILWriter(new StringBuilder());
            if(this.GenericParamters != null && this.GenericParamters.Count > 0 )
            {
                writer.Write("<");
                for(int iCount = 0; iCount < this.GenericParamters.Count; iCount ++)
                {
                    if(iCount > 0 )
                    {
                        writer.Write(",");
                    }
                    var item = this.GenericParamters[iCount];
                    if (item.RuntimeType != null)
                    {
                        item.RuntimeType.WriteToForSignString(writer,null , 0 , userShortName);
                    }
                    else
                    {
                        writer.Write("MT" + item.Index);
                    }
                }
                writer.Write(">");
            }
            writer.Write("(");
            if( this.Parameters != null && this.Parameters.Count > 0 )
            {
                for(int iCount = 0; iCount < this.Parameters.Count; iCount ++)
                {
                    if(iCount > 0 )
                    {
                        writer.Write(",");
                    }
                    var p = this.Parameters[iCount];
                    p.ValueType.WriteToForSignString(writer, null, 0, userShortName);
                    if( outputParameterName
                        && this.RenameState != DCILRenameState.Renamed 
                        && p.Name != null 
                        && p.Name.Length > 0 )
                    {
                        writer.Write(" ");
                        writer.Write(p.Name);
                    }
                }
            }
            writer.Write(")");
            return writer.ToString();
        }

        public static string InnerGetSignString(
            DCILTypeReference returnType,
            string methodName,
            DCILGenericParamterList methodGps,
            DCILGenericParamterList classGps,
            List<DCILMethodParamter> ps)
        {
            var str = new StringBuilder();
            var writer = new DCILWriter(str);
            var allGps = DCILGenericParamterList.Merge(methodGps, classGps);
            returnType?.WriteToForSignString(writer, allGps);
            if (methodName != null)
            {
                if (str.Length > 0)
                {
                    writer.Write(" ");
                }
                writer.Write(methodName);
            }
            if (methodGps != null && methodGps.Count > 0)
            {
                writer.Write("<");
                for (int iCount = 0; iCount < methodGps.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    var item = methodGps[iCount];
                    if (item.RuntimeType != null)
                    {
                        item.RuntimeType.WriteToForSignString(writer, allGps);
                    }
                    else
                    {
                        writer.Write("MT" + item.Index);
                    }
                }
                writer.Write(">");
            }
            writer.Write("(");
            if (ps != null && ps.Count > 0)
            {
                for (int iCount = 0; iCount < ps.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    ps[iCount].ValueType.WriteToForSignString(writer, allGps);
                }
            }
            writer.Write(")");
            return str.ToString();
        }


        public override void WriteTo(DCILWriter writer)
        {
            if (this.RenameState == DCILRenameState.Renamed)
            {
                writer.WriteLine("// " + this.OwnerClass.GetOldName() + "::" + this.GetOldName());
            }
            writer.Write(".method ");
            base.WriteStyles(writer);
            if (this.Pinvokeimpl != null && this.Pinvokeimpl.Length > 0)
            {
                writer.Write(" pinvokeimpl (" + this.Pinvokeimpl + ") ");
            }
            this.ReturnTypeInfo.WriteTo(writer);
            if (this.ReturnMarshal != null && this.ReturnMarshal.Length > 0)
            {
                writer.Write(" marshal( " + this.ReturnMarshal + " ) ");
            }
            writer.Write(" " + this._Name);
            this.GenericParamters?.WriteTo(writer);

            DCILMethodParamter.WriteTo(this.Parameters, writer, false);

            writer.Write(this.DeclearEndFix);
            writer.WriteStartGroup();
            if (this.EntryPoint)
            {
                writer.Write(".entrypoint");
            }
            base.WriteCustomAttributes(writer);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {
                for (int iCount = 0; iCount < this.Parameters.Count; iCount++)
                {
                    var p = this.Parameters[iCount];
                    if (p.DefaultValue != null && p.DefaultValue.Length > 0)
                    {
                        writer.WriteLine(".param [" + Convert.ToString(iCount + 1) + "] = " + p.DefaultValue);
                    }
                }
            }
            if (this.permission != null && this.permission.Length > 0)
            {
                writer.WriteLine(".permission " + this.permission);
            }
            if (this.permissionset != null && this.permissionset.Length > 0)
            {
                writer.WriteLine(".permissionset " + this.permissionset);
            }
            if (this._Override != null)
            {
                if (this._Override.SimpleMode)
                {
                    writer.Write(".override ");
                }
                else
                {
                    writer.Write(".override method ");
                    if (this.Parent.Name == "'<CastYield>d__2`1'")
                    {

                    }
                }
                this._Override.WriteTo(writer);
                writer.WriteLine();
            }
            if (this.Maxstack >= 0)
            {
                writer.WriteLine(".maxstack " + Convert.ToString( this.Maxstack  + this.MaxstackFix ));
            }
            if (this.Locals != null && this.Locals.Count > 0)
            {
                writer.Write(".locals ");
                if (this.Locals.HasInit)
                {
                    writer.Write(" init (");
                }
                else
                {
                    writer.Write("(");
                }
                writer.ChangeIndentLevel(1);
                for (int iCount = 0; iCount < this.Locals.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.WriteLine(",");
                    }
                    var item = this.Locals[iCount];
                    if (item.Index >= 0)
                    {
                        writer.Write("[" + item.Index + "] ");
                    }
                    item.ValueType.WriteTo(writer);
                    if (item.Name != null && item.Name.Length > 0)
                    {
                        writer.Write(" " + item.Name);
                    }
                }
                writer.WriteLine(")");
                writer.ChangeIndentLevel(-1);
            }
            if (this.OperCodes != null && this.OperCodes.Count > 0)
            {
                this.OperCodes.WriteTo(writer);
            }
            writer.WriteEndGroup();
        }
        public DCILMethod()
        {
            base.OperCodes = new DCILOperCodeList();
        }
        public DCILTypeReference ReturnTypeInfo = null;

        public DCILMethodLocalVariableList Locals = null;

        public int Maxstack = -1;
        public int MaxstackFix = 0;
        public List<DCILMethodParamter> Parameters = null;
        public int ParametersCount
        {
            get
            {
                return this.Parameters == null ? 0 : this.Parameters.Count;
            }
        }
        public string DeclearEndFix = null;

        public int ILCodeStartLineIndex = 0;
        public bool ILCodesModified = false;
         public string ReturnType = null;
        public override DCILMethod GetOwnerMethod()
        {
            return this;
        }
        //public int ComponentResourceManagerLineIndex = -1;
        public override string ToString()
        {
            this._SignString = null;
            var resut = this.Name + " ## " + this.GetSignString();// this.ReturnType + " " + this.Name;
            if (this.Parent is DCILClass)
            {
                resut = resut + " # " + ((DCILClass)this.Parent).Name;
            }
            else if (this._NativeMethod != null)
            {
                resut = resut + " #$ " + this._NativeMethod.DeclaringType.Name;
            }
            return resut;
        }
    }
    internal class DCILMethodLocalVariableList : List<DCILMethodLocalVariable> , IDisposable
    {
        public bool HasInit = true;
        public void Dispose()
        {
            foreach( var item in this )
            {
                item.Name = null;
                item.ValueType = null;
            }
            this.Clear();
        }
    }
    internal class DCILMethodLocalVariable
    {
        public int Index = int.MinValue;
        public string Name = null;
        public DCILTypeReference ValueType = null;
        public override string ToString()
        {
            var str = new StringBuilder();
            if( this.Index >= 0 )
            {
                str.Append("[" + this.Index + "]");
            }
            str.Append(this.ValueType.ToString());
            if(this.Name != null && this.Name.Length > 0 )
            {
                str.Append(" " + this.Name);
            }
            return str.ToString();
        }
    }

    internal class DCILMResource : DCILObject
    {
        public const string EXT_Resources = ".resources";
       
        public static void LoadData(System.Collections.Generic.IEnumerable<DCILMResource> reses, string basePath)
        {
            if (basePath == null || basePath.Length == 0)
            {
                throw new ArgumentNullException("basePath");
            }
            if (System.IO.Directory.Exists(basePath) == false)
            {
                throw new System.IO.DirectoryNotFoundException(basePath);
            }
            var subDirs = System.IO.Directory.GetDirectories(basePath);
            foreach (var res in reses)
            {
                
                //if (res.Name.IndexOf("LocalWebServer")>=0)
                //{

                //}
                //if( res.IsResources == false )
                //{
                //    continue;
                //}
                var rfn = res.RuntimeFileName;
                var fn = Path.Combine(basePath, rfn);
                if (File.Exists(fn))
                {
                    res.Data = File.ReadAllBytes(fn);
                    if (res.IsResources)
                    {
                        foreach (var subDir in subDirs)
                        {
                            var dir2 = Path.GetFileName(subDir);
                            var fn2 = Path.Combine(
                                subDir, 
                                Path.GetFileNameWithoutExtension(rfn) + "." + dir2 + EXT_Resources);
                            if (System.IO.File.Exists(fn2))
                            {
                                if (res.LocalDatas == null)
                                {
                                    res.LocalDatas = new SortedDictionary<string, byte[]>();
                                }
                                res.LocalDatas[dir2] = System.IO.File.ReadAllBytes(fn2);
                            }
                        }
                    }
                }
            }
        }

        public const string TagName_mresource = ".mresource";
        public DCILMResource()
        {

        }

        public DCILMResource(DCILDocument doc, DCILReader reader)
        {
            this.OwnerDocument = doc;
            var word = reader.ReadWord();
            if (word == "public")
            {
                this.IsPublic = true;
                this._Name = reader.ReadWord();
            }
            else if (word == "private")
            {
                this.IsPublic = false;
                this._Name = reader.ReadWord();
            }
            reader.MoveAfterChar('}');
            //reader.ReadAfterCharExcludeLastChar('}');
            this.IsResources = this._Name.EndsWith(EXT_Resources);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.Data = null;
            if (this.LocalDatas != null)
            {
                this.LocalDatas.Clear();
                this.LocalDatas = null;
            }
            if (this._ResourceValues != null)
            {
                foreach (var item in this._ResourceValues)
                {
                    item.Value.Name = null;
                    item.Value.Value = null;
                }
                this._ResourceValues.Clear();
                this._ResourceValues = null;
            }
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Resource;
            }
        }
        private string RuntimeFileName
        {
            get
            {
                var rfn = this.Name;
                if (rfn == null || rfn.Length == 0)
                {
                    return null;
                }
                if (rfn[0] == '\'' && rfn.Length > 2 && rfn[rfn.Length - 1] == '\'')
                {
                    rfn = rfn.Substring(1, rfn.Length - 2);
                }
                return rfn;
            }
        }
        public bool Modified = false;
        public bool IsResources = false;

        public byte[] Data = null;
        public SortedDictionary<string, byte[]> LocalDatas = null;
        public bool HasBmpValue
        {
            get
            {
                foreach (var item in this.ResourceValues)
                {
                    if (item.Value.Value is System.Drawing.Bitmap)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private Dictionary<string, MResourceItem> _ResourceValues = null;
        public Dictionary<string, MResourceItem> ResourceValues
        {
            get
            {
                if (this._ResourceValues == null && this.Data != null && this.Data.Length > 0)
                {
                    this._ResourceValues = new Dictionary<string, MResourceItem>();
                    FillValues2(new System.IO.MemoryStream(this.Data), this._ResourceValues);
                }
                return this._ResourceValues;
            }
        }

        public bool ChangeLanguage(string language)
        {
            if (language == null || language.Length == 0)
            {
                return false;
            }
            if (this.Data == null || this.Data.Length == 0)
            {
                return false;
            }
            byte[] localData = null;
            if (this.LocalDatas == null || this.LocalDatas.TryGetValue(language, out localData) == false)
            {
                return false;
            }
            if (localData == null || localData.Length == 0)
            {
                return false;
            }
            this._ResourceValues = new Dictionary<string, MResourceItem>();
            FillValues2(new System.IO.MemoryStream(this.Data), this._ResourceValues);
            if (FillValues2(new System.IO.MemoryStream(localData), this._ResourceValues) > 0)
            {
                MyConsole.Instance.WriteLine("    Change resource language : " + this.Name);

                var ms = new System.IO.MemoryStream();
                using (var writer = new System.Resources.ResourceWriter(ms))
                {
                    var names = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in this._ResourceValues.Values)
                    {
                        if (names.ContainsKey(item.Name) == false)
                        {
                            if (item.Value is string)
                            {
                                writer.AddResource(item.Name, (string)item.Value);
                            }
                            else
                            {
                                writer.AddResourceData(item.Name, item.Type, item.Data);
                            }
                            names[item.Name] = null;
                        }
                        else
                        {
                            MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                            MyConsole.Instance.BackgroundColor = ConsoleColor.White;
                            MyConsole.Instance.WriteLine("       [Warring],Duplicate resource item name : " + this.Name + " # " + item.Key);
                            MyConsole.Instance.ResetColor();
                        }
                    }
                }
                this.Data = ms.ToArray();
                ms.Close();
                this.Modified = true;
                return true;
            }
            return false;
        }
#if DOTNETCORE
        private static int FillValues2(System.IO.Stream stream, Dictionary<string, MResourceItem> values)
        {
            int result = 0;
            using (var reader = new System.Resources.Extensions.DeserializingResourceReader(stream))
            {
                var enumer = reader.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string name = Convert.ToString(enumer.Key);
                    string resType = null;
                    byte[] itemData = null;
                    object enumValue = enumer.Value;
                    if( enumValue is string )
                    {
                        resType = null;
                        itemData = System.Text.Encoding.UTF8.GetBytes((string)enumValue);
                    }
                    else if( enumValue is System.Drawing.Bitmap )
                    {
                        resType = "System.Drawing.Bitmap, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";// typeof(System.Drawing.Bitmap).AssemblyQualifiedName;
                        var ms = new System.IO.MemoryStream();
                        _bf.Serialize(ms, enumValue);
                        itemData = ms.ToArray();
                        ms.Close();
                    }
                    if (itemData != null && itemData.Length > 0)
                    {
                        var item = new MResourceItem();
                        item.Name = name;
                        item.Type = resType;
                        item.Data = itemData;
                        item.Value = enumer.Value;
                        values[name] = item;
                        //values[name] = new Tuple<string, byte[]>(resType, itemData);
                        result++;
                    }
                }
            }
            return result;
        }
        private static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
#else
        private static int FillValues2(System.IO.Stream stream, Dictionary<string, MResourceItem> values)
        {
            int result = 0;
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                var enumer = reader.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string name = Convert.ToString(enumer.Key);
                    string resType = null;
                    byte[] itemData = null;
                    reader.GetResourceData(name, out resType, out itemData);
                    if (resType != null && resType.Length > 0 && itemData != null && itemData.Length > 0)
                    {
                        var item = new MResourceItem();
                        item.Name = name;
                        item.Type = resType;
                        item.Data = itemData;
                        item.Value = enumer.Value;
                        values[name] = item;
                        //values[name] = new Tuple<string, byte[]>(resType, itemData);
                        result++;
                    }
                }
            }
            return result;
        }
#endif

        public bool WriteDataFile(string basePath)
        {
            if (this.Data != null && this.Data.Length > 0)
            {
                var fn = Path.Combine(basePath, this.RuntimeFileName);
                File.WriteAllBytes(fn, this.Data);
                return true;
            }
            else
            {
                return false;
            }
        }
        public class MResourceItem
        {
            public string Name = null;
            public string Type = null;
            public object Value = null;
            public bool IsBmp
            {
                get
                {
                    return this.Value is System.Drawing.Bitmap;
                }
            }
            public byte[] Data = null;
            public int StartIndex = 0;
            public int BSLength = 0;
            public int Key = 0;
            public override string ToString()
            {
                return this.Name + " # " + this.Type;
            }
        }
        public byte[] EncryptData()
        {
            var items = this.ResourceValues.Values;
            var lstData = new List<byte>();
            var rnd = new System.Random();
            foreach (var item in items)
            {
                item.StartIndex = lstData.Count;
                item.Key = rnd.Next(1000, int.MaxValue - 1000);
                if (item.Value is string)
                {
                    string str = (string)item.Value;
                    int key = item.Key;
                    for (int iCount = 0; iCount < str.Length; iCount++, key++)
                    {
                        var v = str[iCount] ^ key;
                        lstData.Add((byte)(v >> 8));
                        lstData.Add((byte)(v & 0xff));
                    }
                }
                else if (item.Value is System.Drawing.Bitmap)
                {
                    var ms2 = new System.IO.MemoryStream();
                    ((System.Drawing.Bitmap)item.Value).Save(ms2, System.Drawing.Imaging.ImageFormat.Bmp);
                    var bs = ms2.ToArray();
                    ms2.Close();
                    var key = (byte)item.Key;
                    for (int iCount = 0; iCount < bs.Length; iCount++, key++)
                    {
                        lstData.Add((byte)(bs[iCount] ^ key));
                    }
                }
                else
                {
                    MyConsole.Instance.EnsureNewLine();
                    MyConsole.Instance.WriteError("    Not support resource item:" + this.Name + "|" + item.Name + "=" + item.Value.GetType().FullName);
                }
                item.BSLength = lstData.Count - item.StartIndex;
            }
            var result = lstData.ToArray();
            return result;
        }

        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".mresource ");
            if (this.IsPublic)
            {
                writer.Write(" public ");
            }
            else
            {
                writer.Write(" private ");
            }
            writer.Write(this._Name);
            writer.WriteStartGroup();
            writer.WriteEndGroup();
        }
        public override string ToString()
        {
            return "Resource " + this._Name;
        }

        public bool IsPublic = true;

    }
    internal class DCILEvent : DCILMemberInfo
    {
        public const string TagName = ".event";
        public DCILEvent()
        {

        }
        public DCILEvent(DCILClass cls, DCILReader reader)
        {
            this.Parent = cls;
            this.Load(reader);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.EventHandlerType = null;
            this.EventHandlerTypeName = null;
            this.Method_Addon = null;
            this.Method_Removeon = null;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Event;
            }
        }
        public override string ToString()
        {
            return "Event " + this.EventHandlerType.ToString() + " " + this._Name;
        }
        public override void Load(DCILReader reader)
        {
            while (reader.HasContentLeft())
            {
                int pos = reader.Position;
                string strWord = reader.ReadWord();
                if (strWord == "specialname" || strWord == "rtspecialname")
                {
                    this.AddStyle(strWord);
                }
                else
                {
                    if (DCILTypeReference.IsStartWord(strWord))
                    {
                        this.EventHandlerType = DCILTypeReference.Load(strWord, reader);
                    }
                    else
                    {
                        reader.Position = pos;
                        this.EventHandlerType = DCILTypeReference.Load("class", reader);
                    }
                    this._Name = reader.ReadWord();
                    break;
                }
            }
            reader.MoveAfterChar('{');// reader.ReadAfterChar('{');
            while (reader.HasContentLeft())
            {
                var word = reader.ReadWord();
                if (word == DCILCustomAttribute.TagName_custom)
                {
                    base.ReadCustomAttribute(reader);
                }
                else if (word == ".addon")
                {
                    this.Method_Addon = new DCILInvokeMethodInfo(reader);
                }
                else if (word == ".removeon")
                {
                    this.Method_Removeon = new DCILInvokeMethodInfo(reader);
                }
                else if (word == "}")
                {
                    break;
                }
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".event ");
            this.EventHandlerType.WriteTo(writer);
            writer.Write(" ");
            writer.WriteLine(this._Name);
            writer.WriteStartGroup();
            base.WriteCustomAttributes(writer);
            if (this.Method_Addon != null)
            {
                writer.Write(".addon ");
                this.Method_Addon.WriteTo(writer);
                writer.WriteLine();
            }
            if (this.Method_Removeon != null)
            {
                writer.Write(".removeon ");
                this.Method_Removeon.WriteTo(writer);
                writer.WriteLine();
            }
            writer.WriteEndGroup();
        }
        public DCILInvokeMethodInfo Method_Addon = null;
        public DCILInvokeMethodInfo Method_Removeon = null;
        public DCILTypeReference EventHandlerType = null;

        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            this.Method_Addon = document.CacheDCILInvokeMethodInfo(this.Method_Addon);
            this.Method_Removeon = document.CacheDCILInvokeMethodInfo(this.Method_Removeon);
            this.EventHandlerType = document.CacheTypeReference(this.EventHandlerType);
            this.Method_Addon?.UpdateLocalInfo(this.Parent as DCILClass);
            this.Method_Removeon?.UpdateLocalInfo(this.Parent as DCILClass);
            if (this.Method_Addon.LocalMethod != null)
            {
                this.Method_Addon.LocalMethod.ParentMember = this;
            }
            if (this.Method_Removeon.LocalMethod != null)
            {
                this.Method_Removeon.LocalMethod.ParentMember = this;
            }
            base.CusotmAttributesCacheTypeReference(document);
        }

        public string EventHandlerTypeName = null;
    }
    internal class DCILProperty : DCILMemberInfo
    {
        public const string TagName_property = ".property";
        public DCILProperty(DCILClass cls, DCILReader reader)
        {
            this.Parent = cls;
            this.Load(reader);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.Method_Get = null;
            this.Method_Set = null;
            if( this.Parameters != null )
            {
                foreach( var item in this.Parameters )
                {
                    item.Dispose();
                }
                this.Parameters = null;
                this.Parameters = null;
            }
            this.ValueType = null;
            this.ValueTypeName = null;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Property;
            }
        }
        public override void Load(DCILReader reader)
        {
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (DCILTypeReference.IsStartWord(strWord))
                {
                    this.ValueType = DCILTypeReference.Load(strWord, reader);
                    this._Name = reader.ReadWord();
                    break;
                }
                else
                {
                    this.AddStyle(strWord);
                }
            }
            reader.MoveAfterChar('('); //reader.ReadAfterCharExcludeLastChar('(');
            this.Parameters = DCILMethodParamter.ReadParameters(reader, false);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {

            }
            reader.MoveAfterChar('{');// reader.ReadAfterChar('{');
            while (reader.HasContentLeft())
            {
                var word = reader.ReadWord();
                if (word == DCILCustomAttribute.TagName_custom)
                {
                    base.ReadCustomAttribute(reader);
                }
                else if (word == ".get")
                {
                    this.Method_Get = new DCILInvokeMethodInfo(reader);
                }
                else if (word == ".set")
                {
                    this.Method_Set = new DCILInvokeMethodInfo(reader);
                }
                else if (word == "}")
                {
                    break;
                }
            }
        }

        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".property ");
            base.WriteStyles(writer);
            this.ValueType.WriteTo(writer);
            writer.Write(" " + this._Name);
            DCILMethodParamter.WriteTo(this.Parameters, writer, false);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {

            }
            writer.WriteStartGroup();
            base.WriteCustomAttributes(writer);
            if (this.Method_Get != null)
            {
                writer.Write(".get ");
                this.Method_Get.WriteTo(writer);
                writer.WriteLine();
            }
            if (this.Method_Set != null)
            {
                writer.Write(".set ");
                this.Method_Set.WriteTo(writer);
                writer.WriteLine();
            }
            writer.WriteEndGroup();
        }
        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            DCILMethodParamter.CacheTypeReference(document, this.Parameters);
            this.Method_Get = document.CacheDCILInvokeMethodInfo(this.Method_Get);
            this.Method_Set = document.CacheDCILInvokeMethodInfo(this.Method_Set);
            this.ValueType = document.CacheTypeReference(this.ValueType);
            if (this.Method_Get != null)
            {
                this.Method_Get.UpdateLocalInfo(this.Parent as DCILClass);
                if (this.Method_Get.LocalMethod != null)
                {
                    this.Method_Get.LocalMethod.ParentMember = this;
                }
            }
            if (this.Method_Set != null)
            {
                this.Method_Set.UpdateLocalInfo(this.Parent as DCILClass);
                if (this.Method_Set.LocalMethod != null)
                {
                    this.Method_Set.LocalMethod.ParentMember = this;
                }
            }
            base.CusotmAttributesCacheTypeReference(document);
        }
        public List<DCILMethodParamter> Parameters = null;

        public DCILInvokeMethodInfo Method_Get = null;
        public DCILInvokeMethodInfo Method_Set = null;
        public DCILTypeReference ValueType = null;

        public bool ValueTypeIsClass = false;

        public string ValueTypeName = null;

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Property " + this._Name);
            if (this.Method_Get != null)
            {
                str.Append(" get; ");
            }
            if (this.Method_Set != null)
            {
                str.Append(" set;");
            }
            return str.ToString();
        }
    }


    internal class IDGenerator
    {
        public IDGenerator(string strPreFix, string memberPrefix , int inputGenCount = int.MinValue)
        {
            if (strPreFix == null || strPreFix.Trim().Length == 0)
            {
                this._ClassNamePrefx = "_jiejie";
            }
            else
            {
                this._ClassNamePrefx = strPreFix.Trim();
            }
            if (memberPrefix == null || memberPrefix.Trim().Length == 0)
            {
                this._MemberNamePrefix = "_jj";
            }
            else
            {
                this._MemberNamePrefix = memberPrefix.Trim();
            }
            if (inputGenCount == int.MinValue)
            {
                this.GenCount = GenCountBase + _rnd.Next(10, 100);
            }
            else
            {
                this.GenCount = inputGenCount;
            }
        }

        private static readonly System.Random _rnd = new Random();
        private static readonly string _Chars = "lkjhgfdsaqwertyuiopmnbvcxz";//"mn0O1l";
        private static readonly int _CharsLength = _Chars.Length;
        private readonly string _ClassNamePrefx = null;
        private readonly string _MemberNamePrefix = null;
        
        public static int GenCountBase = 0;

        public int GenCount = 0;
        public bool DebugMode = false;

        public string GenerateIDForClass(string oldName, DCILObject obj)
        {
            if (this.DebugMode)
            {
                return DebugModeGenerateID(oldName, obj);
            }
            string id = this.InnerGenerateID();
            if (this._ClassNamePrefx != null)
            {
                return this._ClassNamePrefx + id;
            }
            else
            {
                return id;
            }
        }

        [ThreadStatic]
        private static string _CreateTime = null;
       
        public string GenerateIDForMember(string oldName, DCILObject obj)
        {
            if (this.DebugMode)
            {
                return DebugModeGenerateID(oldName, obj);
            }
            var id = this.InnerGenerateID();
            if (this._MemberNamePrefix != null)
            {
                id = this._MemberNamePrefix + id;
            }
            if (_rnd.Next(0, 100) < 2)
            {
                // 有较小的概率添加当前时间
                if (_CreateTime == null)
                {
                    _CreateTime = "_jiejie" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                id = id + _CreateTime;
            }
            return DCUtils.GetStringUseTable( id );
        }

        private string DebugModeGenerateID(string oldName, DCILObject obj)
        {
            //this.GenCount++;
            int idFix = obj.InstanceIndex;
            string result = null;
            if (oldName[0] == '\'')
            {
                result = "'__" + oldName.Substring(1);
            }
            else
            {
                result = "__" + oldName;
            }
            if (result[result.Length - 1] == '\'')
            {
                result = result.Substring(0, result.Length - 1) + "_" + idFix.ToString() + "'";
            }
            else
            {
                result = result + "_" + idFix.ToString();
            }
            return result;
        }
         
        private char[] _ResultBuffer = new char[20];
        private string InnerGenerateID()
        {
            int gc = this.GenCount++;
            for (int iCount = 0; iCount < 20; iCount++)
            {
                int index = gc % _CharsLength;
                _ResultBuffer[iCount] = _Chars[index];
                gc = (gc - index) / _CharsLength;
                if (gc == 0)
                {
                    return new string(this._ResultBuffer, 0, iCount + 1);
                }
            }
            return null;
        }
    }


    public interface IEqualsValue<T>
    {
        bool EqualsValue(T otherValue);
    }

    internal static class DCUtils
    {
        /// <summary>
        /// 获得程序集支持的所有的用户界面语言的数组
        /// </summary>
        /// <param name="asmFileName">程序集文件名</param>
        /// <returns>数组</returns>
        public static SortedDictionary<string,string> GetSupporttedCultures( string asmFileName )
        {
            if( asmFileName == null || asmFileName.Length == 0 )
            {
                return null;
            }
            if( File.Exists( asmFileName ) == false )
            {
                return null;
            }
            var resFileName = Path.GetFileNameWithoutExtension(asmFileName) + ".resources.dll";

            var allCuls = System.Globalization.CultureInfo.GetCultures(
                System.Globalization.CultureTypes.AllCultures);
            var rootDir = Path.GetDirectoryName(asmFileName);
            var result = new SortedDictionary<string, string>();
            foreach( var subDir in Directory.GetDirectories( rootDir ))
            {
                var dn = Path.GetFileName(subDir);
                foreach( var cul in allCuls )
                {
                    if( string.Compare( cul.Name , dn , true ) == 0)
                    {
                        var fn2 = Path.Combine(subDir, resFileName);
                        if( File.Exists( fn2 ))
                        {
                            result[cul.Name] = cul.DisplayName;
                        }
                        break;
                    }
                }
            }
            return result;
        }

        private static readonly Dictionary<int, string> _Int32ValueStrings = new Dictionary<int, string>();
        /// <summary>
        /// 获得整数值的字符串值
        /// </summary>
        /// <param name="v">整数</param>
        /// <returns>字符串值</returns>
        public static string GetInt32ValueString( int v )
        {
            string result = null;
            if(_Int32ValueStrings.TryGetValue( v , out result ) == false )
            {
                result = v.ToString();
                _Int32ValueStrings[v] = result;
            }
            return result;
        }

        private static string[] _SingleANSICharStrings = null;
        private static Dictionary<char, string> _SinglCharStrings = null;
        /// <summary>
        /// 获得单个字符组成的字符串
        /// </summary>
        /// <param name="c">字符值</param>
        /// <returns>字符串</returns>
        public static string GetSingleCharString(char c)
        {
            if (c < 127)
            {
                if (_SingleANSICharStrings == null)
                {
                    _SingleANSICharStrings = new string[127];
                    for (int iCount = 0; iCount < _SingleANSICharStrings.Length; iCount++)
                    {
                        _SingleANSICharStrings[iCount] = new string((char)iCount, 1);
                    }
                }
                return _SingleANSICharStrings[c];
            }
            else
            {
                if (_SinglCharStrings == null)
                {
                    _SinglCharStrings = new Dictionary<char, string>();
                }
                string v = null;
                if (_SinglCharStrings.TryGetValue(c, out v) == false)
                {
                    v = c.ToString();
                    _SinglCharStrings[c] = v;
                }
                return v;
            }
        }

        private static readonly Dictionary<string, string> _StringTable = new Dictionary<string, string>();
        public static void ClearStringTable()
        {
            _StringTable.Clear();
            _Type_FullName.Clear();
        }
        public static string GetStringUseTable(string v)
        {
            if (v == null)
            {
                return null;
            }
            if (v.Length == 0)
            {
                return string.Empty;
            }
            string result = null;
            if (_StringTable.TryGetValue(v, out result) == false)
            {
                _StringTable[v] = v;
                result = v;
            }
            return result;
        }

        public static string GetShortName(string fullName )
        {
            string prefix = null;
            return GetShortName(fullName, out prefix);
        }

        public static string GetShortName( string fullName , out string preFix )
        {
            preFix = string.Empty;
            if(fullName == null || fullName .Length == 0 )
            {
                return string.Empty;
            }
            if( fullName.EndsWith("..cctor"))
            {
                preFix = fullName.Substring(0, fullName.Length - 7);
                return ".cctor";
            }
            if( fullName.EndsWith("..ctor"))
            {
                preFix = fullName.Substring(0, fullName.Length - 6);
                return "..ctor";
            }
            int index = fullName.LastIndexOf('.');
            if(index > 0 )
            {
                string sn = fullName.Substring(index + 1);
                preFix = fullName.Substring(0, index);
                return sn;
            }
            return fullName;
        }
        public static void RunExe(string exeFileName, string argument)
        {
            if (exeFileName == null || exeFileName.Length == 0)
            {
                throw new ArgumentNullException("exeFileName");
            }
            if (File.Exists(exeFileName) == false)
            {
                throw new FileNotFoundException(exeFileName);
            }
            var pstart = new System.Diagnostics.ProcessStartInfo();
            pstart.FileName = exeFileName;
            pstart.Arguments = argument;
            MyConsole.Instance.EnsureNewLine();
            MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
            MyConsole.Instance.Write("   >>RUN:");
            MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
            MyConsole.Instance.Write("\"" + pstart.FileName + "\" ");
            if (argument != null && argument.Length > 0)
            {
                MyConsole.Instance.Write(" ");
                MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                MyConsole.Instance.Write(argument);
            }
            MyConsole.Instance.ResetColor();
            MyConsole.Instance.WriteLine();
            System.Diagnostics.Debug.WriteLine(">>RUN: \"" + pstart.FileName + "\" " + pstart.Arguments);
            pstart.UseShellExecute = false;
            if( MyConsole.Instance.IsNativeConsole == false )
            {
                pstart.CreateNoWindow = true;
            }
            if(MyConsole.Instance.IsNativeConsole == false )
            {
                pstart.RedirectStandardError = true;
                pstart.RedirectStandardInput = true;
                pstart.RedirectStandardOutput = true ;
            }
            //pstart.CreateNoWindow = false;
            var p = System.Diagnostics.Process.Start(pstart);
            if(MyConsole.Instance.IsNativeConsole == false )
            {
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                p.ErrorDataReceived += P_ErrorDataReceived;
                p.OutputDataReceived += P_OutputDataReceived;
            }
            p.WaitForExit();
        }

        private static void P_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.Length > 0)
            {
                MyConsole.Instance.Write(Environment.NewLine + e.Data);
            }
        }

        private static void P_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.Length > 0)
            {
                MyConsole.Instance.WriteError(Environment.NewLine + e.Data);
            }
        }

        public static void EatAllConsoleKey()
        {
            if (MyConsole.Instance.SupportKeyboardInput)
            {
                while (MyConsole.Instance.KeyAvailable)
                {
                    MyConsole.Instance.ReadKey();
                }
            }
        }
        private static readonly string _hexs = "0123456789ABCDEF";

        public static string ToHexString( byte[] bs )
        {
            var str = new StringBuilder();
            int len = bs.Length;
            str.Append('(');
            for( int iCount = 0; iCount < len; iCount ++ )
            {
                if(iCount > 0 )
                {
                    str.Append(' ');
                }
                var b = bs[iCount];
                str.Append(_hexs[b >> 4]);
                str.Append(_hexs[b & 15]);
            }
            str.Append(')');
            return str.ToString();
        }

        public static int ConvertToInt32( string v )
        {
            if(v == null || v.Length == 0 )
            {
                throw new ArgumentNullException("v");
            }
            if (v.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                int result = int.Parse(v.Substring(2), System.Globalization.NumberStyles.HexNumber);
                return result;
            }
            else
            {
                return Convert.ToInt32(v);
            }
        }

        public static bool IsSystemAsseblyName(string asmName)
        {
            if( asmName == null || asmName .Length == 0 )
            {
                throw new ArgumentNullException("asmName");
            }
            return asmName.StartsWith("mscor", StringComparison.OrdinalIgnoreCase)
                || asmName.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
                || string.Compare(asmName, "System", true) == 0
                || asmName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase);
        }
        internal static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
        internal static List<string> SplitByWhitespace(string text)
        {
            var list = new List<string>();
            int len = text.Length;
            int lastIndex = 0;
            bool isInGroup = false;
            for (int iCount = 0; iCount < len; iCount++)
            {
                var c = text[iCount];
                if (c == '\'')
                {
                    isInGroup = !isInGroup;
                }
                if (IsWhitespace(c) && isInGroup == false)
                {
                    if (iCount > lastIndex)
                    {
                        list.Add(text.Substring(lastIndex, iCount - lastIndex));
                    }
                    lastIndex = iCount + 1;
                }
            }
            if (lastIndex < len)
            {
                list.Add(text.Substring(lastIndex));
            }
            return list;
        }

        /// <summary>
        /// 获得.NET SDK安装目录
        /// </summary>
        /// <returns></returns>
        public static string GetSDKDir()
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows", false);
            if (key != null)
            {
                string v = Convert.ToString(key.GetValue("CurrentInstallFolder"));
                key.Close();
                if (v != null && System.IO.Directory.Exists(v))
                {
                    v = System.IO.Path.Combine(v, "Bin");
                    if (System.IO.Directory.Exists(v))
                    {
                        var result = DCUtils.SearchFileDeeply(v, "ildasm.exe");
                        if (result != null)
                        {
                            return Path.GetDirectoryName(result);
                        }
                    }
                }
            }
            key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows", false);
            if (key != null)
            {
                string v = Convert.ToString(key.GetValue("CurrentInstallFolder"));
                key.Close();
                if (v != null && System.IO.Directory.Exists(v))
                {
                    v = System.IO.Path.Combine(v, "Bin");
                    if (System.IO.Directory.Exists(v))
                    {
                        var result = DCUtils.SearchFileDeeply(v, "ildasm.exe");
                        if (result != null)
                        {
                            return Path.GetDirectoryName(result);
                        }
                    }
                }
            }
            foreach (var dir in new string[] {
                @"C:\Program Files (x86)\Microsoft SDKs\Windows"
             })
            {
                if (Directory.Exists(dir))
                {
                    var result = DCUtils.SearchFileDeeply(dir, "ildasm.exe");
                    if (result != null)
                    {
                        return Path.GetDirectoryName(result);
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///  删除列表中的重复项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void RemoveSameItem<T>(List<T> list)
        {
            if (list.Count > 1)
            {
                for (int iCount = list.Count - 1; iCount > 0; iCount--)
                {
                    if (list.IndexOf(list[iCount]) != iCount)
                    {
                        list.RemoveAt(iCount);
                    }
                }
            }
        }
        private static readonly Dictionary<Type, string> _Type_FullName = new Dictionary<Type, string>();
        public static string GetFullName(Type t)
        {
            string result = null;
            if( _Type_FullName.TryGetValue( t , out result ) == false )
            {
                result = GetFullName(t.Namespace, t.Name);
                _Type_FullName[t] = result;
            }
            return result;
        }

        public static string GetFullName(string nameSpace, string name)
        {
            if (nameSpace != null && nameSpace.Length > 0)
            {
                return nameSpace + "." + name;
            }
            else
            {
                return name;
            }
        }
        public static bool EqualsListCount(System.Collections.IList ls1, System.Collections.IList ls2)
        {
            int len = ls1 == null ? 0 : ls1.Count;
            int len2 = ls2 == null ? 0 : ls2.Count;
            return len == len2;
        }
        public static bool EqualsList<T>(List<T> list1, List<T> list2) where T : IEqualsValue<T>
        {
            int len1 = list1 == null ? 0 : list1.Count;
            int len2 = list2 == null ? 0 : list2.Count;
            if (len1 != len2)
            {
                return false;
            }
            if (len1 > 0)
            {
                for (int iCount = 0; iCount < len1; iCount++)
                {
                    if (list1[iCount].EqualsValue(list2[iCount]) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool EqualsStringExt(string s1, string s2)
        {
            if (s1 == s2)
            {
                return true;
            }
            return (s1 == null || s1.Length == 0) && (s2 == null || s2.Length == 0);
        }
 
        private static readonly string[] _WhitespaceString = null;
        static DCUtils()
        {
            _WhitespaceString = new string[50];
            _WhitespaceString[0] = string.Empty;
            for (int iCount = 1; iCount < _WhitespaceString.Length; iCount++)
            {
                _WhitespaceString[iCount] = new string(' ', iCount);
            }

            _HexsIndexs = new int[127];
            for (int iCount = 0; iCount < _HexsIndexs.Length; iCount++)
            {
                char c = (char)iCount;
                var index = -1;
                if (c >= '0' && c <= '9')
                {
                    index = c - '0';
                }
                else if (c >= 'A' && c <= 'F')
                {
                    index = c - 'A' + 10;
                }
                else if (c >= 'a' && c <= 'f')
                {
                    index = c - 'a' + 10;
                }
                _HexsIndexs[iCount] = index;
            }
        }

        private static readonly int[] _HexsIndexs = null;

        internal static int IndexOfHexChar(char c)
        {
            if (c < 127)
            {
                return _HexsIndexs[c];
            }
            else
            {
                return -1;
            }
        }

        public static string SearchFileDeeply(string rootDir, string fileName)
        {
            if (File.Exists(Path.Combine(rootDir, fileName)))
            {
                return Path.Combine(rootDir, fileName);
            }
            foreach (var dir in Directory.GetDirectories(rootDir))
            {
                var result = SearchFileDeeply(dir, fileName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        public static void DeleteDirecotry(string rootDir)
        {
            if (rootDir != null && rootDir.Length > 0 && Directory.Exists(rootDir))
            {
                CleanDirectory(rootDir);
                System.IO.Directory.Delete(rootDir, true);
            }
        }
        public static int CleanDirectory(string rootDir)
        {
            if (rootDir == null || rootDir.Length == 0)
            {
                return 0;
            }
            int result = 0;
            if (Directory.Exists(rootDir))
            {
                foreach (var fn in Directory.GetFiles(rootDir))
                {
                    File.SetAttributes(fn, FileAttributes.Normal);
                    File.Delete(fn);
                    result++;
                }
                foreach (var dir in Directory.GetDirectories(rootDir))
                {
                    result += CleanDirectory(dir);
                    Directory.Delete(dir);
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// 格式化输出字节大小数据
        /// </summary>
        /// <param name="ByteSize">字节数</param>
        /// <returns>输出的字符串</returns>
        public static string FormatByteSize(long byteSize)
        {
            const long _PBSIZE = (long)1024 * 1024 * 1024 * 1024;
            const long _GBSIZE = 1024 * 1024 * 1024;
            const long _MBSIZE = 1024 * 1024;
            const long _KBSIZE = 1024;

            long v = byteSize;
            string unit = null;
            if (byteSize > _PBSIZE)
            {
                v = v * 100 / _PBSIZE;
                unit = "PB";
            }
            else if (byteSize > _GBSIZE)
            {
                v = v * 100 / _GBSIZE;
                unit = "GB";
            }
            else if (byteSize > _MBSIZE)
            {
                v = v * 100 / _MBSIZE;
                unit = "MB";
            }
            else if (byteSize > _KBSIZE)
            {
                v = v * 100 / _KBSIZE;
                unit = "KB";
            }
            else
            {
                return byteSize.ToString() + "B";
            }
            int mod = (int)(v % 100);
            v = v / 100;
            if (v > 10)
            {
                mod = mod / 10;
            }
            if (mod == 0)
            {
                return v.ToString() + unit;
            }
            else
            {
                return v.ToString() + '.' + mod.ToString() + unit;
            }
        }

        public static void ObfuseListOrder(System.Collections.IList list)
        {
            ObfuseListOrder(list, 0, list.Count);
        }
        public static void ObfuseListOrder(System.Collections.IList list, int startIndex, int length)
        {
            if (length <= 1)
            {
                return;
            }
            var rnd = new System.Random();
            var strListCount = list.Count;
            var endIndex = startIndex + length;
            for (int iCount = startIndex; iCount < endIndex; iCount++)
            {
                int index = startIndex + rnd.Next(0, length - 1);
                var temp = list[iCount];
                list[iCount] = list[index];
                list[index] = temp;
            }
        }

    }
 

    [System.Runtime.InteropServices.ComVisible(false)]
    internal static class InnerAssemblyHelper20211018
    {
        public static bool EqualString(string s1, string s2)
        {
            return string.Equals(s1, s2);
        }

        private static volatile System.Threading.Thread _CloneStringCrossThead_Thread = null;
        private static readonly System.Threading.AutoResetEvent _CloneStringCrossThead_Event
            = new System.Threading.AutoResetEvent(false);
        private static readonly System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner
            = new System.Threading.AutoResetEvent(false);
        private static volatile string _CloneStringCrossThead_CurrentValue = null;
        /// <summary>
        /// 跨线程的复制字符串值，用于改变创建字符串的调用堆栈
        /// </summary>
        /// <param name="txt">字符串值</param>
        /// <returns>复制品</returns>
        public static string CloneStringCrossThead(string txt)
        {
            if (txt == null || txt.Length == 0)
            {
                return txt;
            }
            lock (_CloneStringCrossThead_Event)
            {
                _CloneStringCrossThead_CurrentValue = txt;
                _CloneStringCrossThead_Event_Inner.Set();
                _CloneStringCrossThead_Event.Reset();
                if (_CloneStringCrossThead_Thread == null)
                {
                    _CloneStringCrossThead_Thread = new System.Threading.Thread(CloneStringCrossThead_Thread);
                    _CloneStringCrossThead_Thread.Start();
                }
                _CloneStringCrossThead_Event.WaitOne(100);
                return _CloneStringCrossThead_CurrentValue;
            }
        }
        private static void CloneStringCrossThead_Thread()
        {
            try
            {
                while (true)
                {
                    if (_CloneStringCrossThead_Event_Inner.WaitOne(1000) == false)
                    {
                        // 等了1秒没任务了，退出线程。
                        break;
                    }

                    if (_CloneStringCrossThead_CurrentValue != null)
                    {
                        _CloneStringCrossThead_CurrentValue = new string(_CloneStringCrossThead_CurrentValue.ToCharArray());
                    }
                    _CloneStringCrossThead_Event.Set();
                    _CloneStringCrossThead_Event_Inner.Reset();
                }
            }
            finally
            {
                _CloneStringCrossThead_Thread = null;
                _CloneStringCrossThead_Event.Reset();
                _CloneStringCrossThead_Event_Inner.Reset();
            }
        }

        public static string GetString(byte[] bsData, int startIndex, int bsLength, int key)
        {
            int chrsLength = bsLength / 2;
            char[] chrs = new char[chrsLength];
            for (int iCount = 0; iCount < chrsLength; iCount++, key++)
            {
                int bi = startIndex + iCount * 2;
                int v = ((bsData[bi] << 8) + bsData[bi + 1]);
                v = v ^ key;
                chrs[iCount] = (char)v;
            }
            return new string(chrs);
        }
        public static System.Drawing.Bitmap GetBitmap(byte[] bsData, int startIndex, int bsLength, int key)
        {
            var bs = new byte[bsLength];
            for (int iCount = 0; iCount < bsLength; iCount++, key++)
            {
                bs[iCount] = (byte)(bsData[startIndex + iCount] ^ key);
            }
            var ms = new System.IO.MemoryStream(bs);
            var bmp = new System.Drawing.Bitmap(ms);
            return bmp;
        }

        /// <summary>
        /// 从字节数组中加载资源数据
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="key"></param>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static System.Resources.ResourceSet LoadResourceSet(byte[] bs, byte key, bool gzip)
        {
            var stream = GetStream(bs, key, gzip);
            var result = new System.Resources.ResourceSet(stream);
            stream.Close();
            return result;
        }
        private static System.IO.Stream GetStream(byte[] bs, byte key, bool gzip)
        {
            int len = bs.Length;
            for (int iCount = 0; iCount < len; iCount++, key++)
            {
                bs[iCount] = ((byte)(bs[iCount] ^ key));
            }
            System.IO.MemoryStream ms = null;
            if (gzip)
            {
                var stream = new System.IO.Compression.GZipStream(
                    new System.IO.MemoryStream(bs),
                    System.IO.Compression.CompressionMode.Decompress);
                ms = new System.IO.MemoryStream();
                var bsTemp = new byte[1024];
                while (true)
                {
                    len = stream.Read(bsTemp, 0, bsTemp.Length);
                    if (len > 0)
                    {
                        ms.Write(bsTemp, 0, len);
                    }
                    else
                    {
                        break;
                    }
                }
                stream.Close();
                ms.Position = 0;
            }
            else
            {
                ms = new System.IO.MemoryStream(bs);
            }
            return ms;
        }

    }

#if DOTNETCORE
    internal class CrossGenHelper
    {
        static CrossGenHelper()
        {
            _nugetPackagesBasePath = "C:\\Users\\" + Environment.UserName + "\\.nuget\\packages";
            if(Directory.Exists( _nugetPackagesBasePath ) == false )
            {
                _nugetPackagesBasePath = null;
                foreach( var dir in Directory.GetDirectories( "c:\\Users"))
                {
                    try
                    {
                        var path2 = Path.Combine(dir, ".nuget\\packages");
                        if(Directory.Exists(path2 ))
                        {
                            _nugetPackagesBasePath = path2;
                            break;
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private static readonly string _nugetPackagesBasePath;

        public bool TestByCrossGen(DCILDocument doc, string inputAsmFileName)
        {
            var refAsms = GetRefAsseblyFileNames(doc);
            if (GetCrossGenExeFileName(doc))
            {
                var strArgs = new StringBuilder();
                strArgs.Append("/ReadyToRun /MissingDependenciesOK");
                if (File.Exists(this._ClrJitFileName))
                {
                    strArgs.Append(" /JITPath \"" + this._ClrJitFileName + "\"");
                }
                if (refAsms != null && refAsms.Count > 0)
                {
                    var refpaths = new HashSet<string>();
                    foreach (var item in refAsms)
                    {
                        //var path2 = Path.GetDirectoryName(item);
                        //if(refpaths.Contains(path2 )== false )
                        //{
                        //    refpaths.Add(path2);
                        //    strArgs.Append(" -p \"" + path2 + "\"");
                        //}
                        strArgs.Append(" -r \"" + item + "\"");
                    }
                }
                string outputPath = Path.Combine(Path.GetDirectoryName(inputAsmFileName), "CrossGen_Temp");
                if (Directory.Exists(outputPath) == false)
                {
                    Directory.CreateDirectory(outputPath);
                }
                string outFileName = Path.Combine(outputPath, Path.GetFileName(inputAsmFileName));
                strArgs.Append(" /out \"" + outFileName +"\"") ;
                strArgs.Append(" \"" + inputAsmFileName + "\"");
                DCUtils.RunExe(this._CrossGenExeFileName, strArgs.ToString());
                if(File.Exists( outFileName ))
                {
                    File.Delete(outFileName);
                }
                Directory.Delete(outputPath);
                return true;
            }
            else
            {
                MyConsole.Instance.WriteLine(" Can not search file 'crossgen.exe' , you must install CORSSGEN.EXE at first!! You can visit 'https://github.com/dotnet/coreclr/blob/master/Documentation/building/crossgen.md'.");
            }
            return false;
        }

        private string _CrossGenExeFileName = null;
        private string _ClrJitFileName = null;

        public bool GetCrossGenExeFileName(DCILDocument doc)
        {
            string resultFileName = null;
            string basePath = null;
            if (doc.IsRequired32Bit)
            {
                basePath = Path.Combine(_nugetPackagesBasePath, "microsoft.netcore.app.runtime.win-x86");
            }
            else
            {
                basePath = Path.Combine(_nugetPackagesBasePath, "microsoft.netcore.app.runtime.win-x64");
            }
            string rootPath = null;
            if (Directory.Exists(basePath))
            {
                if (doc.NetCoreVersion != null && doc.NetCoreVersion.Length > 0)
                {
                    rootPath = SearchVersionPath(basePath, doc.NetCoreVersion);
                    if (rootPath != null)
                    {
                        resultFileName = Path.Combine(rootPath, "tools\\crossgen.exe");
                    }
                }
                if (resultFileName == null || File.Exists(resultFileName) == false)
                {
                    foreach (var dir2 in Directory.GetDirectories(basePath))
                    {
                        var path2 = Path.Combine(dir2, "tools\\crossgen.exe");
                        if (File.Exists(path2))
                        {
                            rootPath = dir2;
                            resultFileName = path2;
                            break;
                        }
                    }
                }
            }
            this._CrossGenExeFileName = resultFileName;
            if (File.Exists(this._CrossGenExeFileName))
            {
                this._ClrJitFileName = DCUtils.SearchFileDeeply(rootPath, "clrjit.dll");
                return true;
            }
            return false;
        }

        private string SearchVersionPath(string basePath, string strVersion,bool deeply = false )
        {
            foreach (var dir in Directory.GetDirectories(basePath))
            {
                var sn = Path.GetFileName(dir);
                if (string.Compare(sn, strVersion, false) == 0 || sn.StartsWith(strVersion, StringComparison.OrdinalIgnoreCase))
                {
                    return dir;
                }
            }
            if(deeply )
            {
                foreach (var dir in Directory.GetDirectories(basePath))
                {
                    var result = SearchVersionPath(dir, strVersion, true);
                    if(result != null )
                    {
                        return result;
                    }
                }
            }
            return null ;
        }

        public List<string> GetRefAsseblyFileNames(DCILDocument doc)
        {
            var jsonFile = Path.ChangeExtension(doc.AssemblyFileName, "deps.json");
            if (File.Exists(jsonFile) == false)
            {
                return null;
            }
            var jsonText = File.ReadAllText(jsonFile, Encoding.UTF8);
            var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonText);
            System.Text.Json.JsonElement rootElement;
            if (jsonDoc.RootElement.TryGetProperty("targets", out rootElement) == false)
            {
                return null;
            }
            var basePaths = new List<string>();
            var resultList = new List<string>();
            var libInfos = new SortedDictionary<string, string>();
            string mainRootPath = null;
            foreach (var p in rootElement.EnumerateObject())
            {
                string tname = RemoveWhitespace(p.Name);
                int index = tname.IndexOf(',');
                if (index > 0)
                {
                    string fwName = tname.Substring(0, index);
                    string strVersion = tname.Substring(index + 1);
                    if (strVersion.StartsWith("Version=", StringComparison.OrdinalIgnoreCase))
                    {
                        strVersion = strVersion.Substring(8);
                        if (strVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                        {
                            strVersion = strVersion.Substring(1);
                        }
                        doc.NetCoreVersion = strVersion;
                        string dotnetRoot = @"C:\Program Files\dotnet\shared";
                        if (Directory.Exists( dotnetRoot))
                        {
                            if( SearchVersionPath( dotnetRoot , strVersion , true ) == null && strVersion.Length > 2 )
                            {
                                // 未找到对应的.NET基础类库路径， 则尝试采用更宽松的查找条件
                                strVersion = strVersion.Substring(0, 2);
                            }
                            foreach (var dir in Directory.GetDirectories( dotnetRoot))
                            {
                                var dir2 = SearchVersionPath(dir, strVersion);
                                if (dir2 != null)
                                {
                                    if (dir2.IndexOf("Microsoft.NETCore.App", StringComparison.OrdinalIgnoreCase) > 0)
                                    {
                                        mainRootPath = dir2;
                                        basePaths.Insert(0, dir2);
                                    }
                                    else
                                    {
                                        basePaths.Add(dir2);
                                    }
                                }
                            }
                        }
                        foreach (var p2 in p.Value.EnumerateObject())
                        {
                            var libName = p2.Name.Replace('/', Path.DirectorySeparatorChar);
                            foreach (var p3 in p2.Value.EnumerateObject())
                            {
                                if (p3.Name == "runtime")
                                {
                                    foreach (var p4 in p3.Value.EnumerateObject())
                                    {
                                        var fileName = p4.Name;
                                        if (fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                                            || fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                                        {
                                            fileName = fileName.Replace('/', Path.DirectorySeparatorChar);
                                            libInfos[libName] = fileName;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (_nugetPackagesBasePath != null)
            {
                basePaths.Add(_nugetPackagesBasePath);
            }
            var existNames = new HashSet<string>();
            foreach (var item in doc.Assemblies)
            {
                if (item.IsExtern && existNames.Contains(item.Name.ToLower()) == false)
                {
                    foreach (var basePath in basePaths)
                    {
                        var fileName = Path.Combine(basePath, item.Name + ".dll");
                        if (File.Exists(fileName))
                        {
                            existNames.Add(item.Name.ToLower());
                            resultList.Add(fileName);
                            break;
                        }
                    }
                }
            }
            foreach (var item in libInfos)
            {
                var sfn = Path.GetFileNameWithoutExtension(item.Value).ToLower();
                if (existNames.Contains(sfn))
                {
                    continue;
                }
                string itemFileName = null;
                foreach (var basePath in basePaths)
                {
                    var fileName = Path.Combine(basePath, item.Value);
                    if (File.Exists(fileName))
                    {
                        itemFileName = fileName;
                        break;
                    }
                    fileName = Path.Combine(Path.Combine(basePath, item.Key), item.Value);
                    if (File.Exists(fileName))
                    {
                        itemFileName = fileName;
                        break;
                    }
                    fileName = Path.Combine(basePath, Path.GetFileName(item.Value));
                    if (File.Exists(fileName))
                    {
                        itemFileName = fileName;
                        break;
                    }
                }
                if (itemFileName != null && resultList.Contains(itemFileName) == false)
                {
                    existNames.Add(sfn);
                    resultList.Add(itemFileName);
                }
            }
            if (mainRootPath != null)
            {
                foreach( var name in new string[] {
                    "System.Private.CoreLib.dll",
                    "netstandard.dll" ,
                    "System.Runtime.dll",
                    "System.Private.Xml.dll",
                    "System.Private.Xml.Linq.dll",
                    "System.Private.Uri.dll",
                    "System.ComponentModel.EventBasedAsync.dll"})
                {
                    var sn = Path.GetFileNameWithoutExtension(name).ToLower();
                    if(existNames.Contains(sn )== false )
                    {
                        var fileName4 = Path.Combine(mainRootPath, name);
                        if(File.Exists( fileName4))
                        {
                            existNames.Add(sn);
                            resultList.Insert(0, fileName4);
                        }
                    }
                }
            }
            resultList.Sort();
            return resultList;
        }



        /// <summary>
        /// 删除字符串中的空白字符
        /// </summary>
        /// <param name="txt">文本</param>
        /// <returns>处理后的文本</returns>
        private static string RemoveWhitespace(string txt)
        {
            if (txt == null || txt.Length == 0)
            {
                return txt;
            }
            int len = txt.Length;
            for (int iCount = 0; iCount < len; iCount++)
            {
                var c = txt[iCount];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    var str = new System.Text.StringBuilder(txt.Length);
                    if (iCount > 0)
                    {
                        str.Append(txt.Substring(0, iCount - 1));
                    }
                    for (int iCount2 = iCount + 1; iCount2 < len; iCount2++)
                    {
                        c = txt[iCount2];
                        if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                        {
                            str.Append(c);
                        }
                    }
                    return str.ToString();
                }
            }
            return txt;
        }
    }
#endif



}

namespace __DC20211119
{
    /// <summary>
    /// 内嵌的帮助类型，只有在.NET2.0和Release模式下编译后的IL代码才能使用。
    /// </summary>
    internal static class JIEJIEHelper
    {
        private sealed class SMF_ResStream : System.IO.Stream
        {
            public SMF_ResStream(byte[] bs)
            {
                int gzipLen = BitConverter.ToInt32(bs, 0);
                if(gzipLen == 0 )
                {
                    this._Content = new byte[bs.Length - 4];
                    Array.Copy(bs, 4, this._Content, 0, this._Content.Length);
                }
                else
                {
                    // 使用GZIP压缩了。
                    var msSource = new System.IO.MemoryStream(bs);
                    msSource.Position = 4;
                    var gm = new System.IO.Compression.GZipStream(msSource, System.IO.Compression.CompressionMode.Decompress);
                    this._Content = new byte[gzipLen];
                    gm.Read(this._Content, 0, gzipLen);
                    gm.Close();
                    msSource.Close();
                }
            }

            private byte[] _Content = null;

            public override bool CanRead
            {
                get
                {
                    return true;
                }
            }
            public override bool CanSeek
            {
                get
                {
                    return true;
                }
            }
            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }

            public override long Length
            {
                get
                {
                    return this._Content.Length;
                }
            }
            private long _Position = 0;
            public override long Position
            {
                get
                {
                    return this._Position;
                }
                set
                {
                    this._Position = value;
                }
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int len = (int)(this._Content.Length - this._Position);
                if( len > count )
                {
                    len = count;
                }
                if (len > 0)
                {
                    Array.Copy(this._Content, this._Position, buffer, offset, len);
                    int endIndex = offset + len;
                    for(int iCount = offset; iCount < endIndex; iCount ++)
                    {
                        buffer[iCount] = (byte)(buffer[iCount] ^ 123);
                    }
                    this._Position += len;
                }
                return len;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        this._Position = offset;
                        break;
                    case SeekOrigin.Current:
                        this._Position += offset;
                        break;
                    case SeekOrigin.End:
                        this._Position = this._Content.Length - offset;
                        break;
                }
                return this._Position;
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
        }
 

        private static Dictionary<string, byte[]> __SMF_Contents = null;
        private static readonly System.Reflection.Assembly ThisAssembly = typeof(JIEJIEHelper).Assembly;
        
        private static Dictionary<string, byte[]> SMF_GetContents()
        {
            var result = __SMF_Contents;
            if( result == null )
            {
                result = new Dictionary<string, byte[]>();
                SMF_InitContent(result);
                __SMF_Contents = result;
            }
            return result;
        }

        private static void SMF_InitContent(Dictionary<string,byte[]> contents )
        {
            //contents["aaa"] = BitConverter.GetBytes(20222);
        }

        public static System.Reflection.ManifestResourceInfo SMF_GetManifestResourceInfo( System.Reflection.Assembly asm , string resourceName )
        {
            if (ThisAssembly.Equals(asm))
            {
                var dic = SMF_GetContents();
                if (dic.ContainsKey( resourceName ))
                {
                    return new ManifestResourceInfo(asm, resourceName, ResourceLocation.Embedded);
                }
            }
            return asm.GetManifestResourceInfo(resourceName);
        }
        public static string[] SMF_GetManifestResourceNames( System.Reflection.Assembly asm )
        {
            if (ThisAssembly.Equals(asm))
            {
                var dic = SMF_GetContents();
                return new List<string>(dic.Keys).ToArray();
            }
            return asm.GetManifestResourceNames();
        }
        public static System.IO.Stream SMF_GetManifestResourceStream(System.Reflection.Assembly asm, string resourceName)
        {
            if ( ThisAssembly.Equals( asm ))
            {
                var dic = SMF_GetContents();
                byte[] bsContent = null;
                if (dic.TryGetValue(resourceName, out bsContent))
                {
                    return new SMF_ResStream(bsContent);
                }
            }
            return asm.GetManifestResourceStream(resourceName);
        }

        public static System.IO.Stream SMF_GetManifestResourceStream2(System.Reflection.Assembly asm, Type t , string resourceName)
        {
            if( resourceName == null || resourceName.Length == 0 )
            {
                throw new ArgumentNullException('r'.ToString());
            }
            if (object.ReferenceEquals(t, null))
            {
                throw new ArgumentNullException('t'.ToString());
            }
            if (ThisAssembly.Equals(asm))
            {
                return SMF_GetManifestResourceStream(asm, t.FullName + '.'.ToString() + resourceName);
            }
            return asm.GetManifestResourceStream(t , resourceName);
        }

        public static string String_Trim( string v )
        {
            return v.Trim();
        }

        public static Type Object_GetType(object a)
        {
            //int v = 99;
            //string v2 = Int32_ToString(ref v);
            return a.GetType();
        }

        public unsafe static void MyInitializeArray(Array v, RuntimeFieldHandle fldHandle, int encKey)
        {
            RuntimeHelpers.InitializeArray(v, fldHandle);
            if (v.Length < 4 || encKey == 0)
            {
                return;
            }
            fixed (byte* ptr = (byte[])v)
            {
                int* ptr2 = (int*)ptr;
                for (int* ptr3 = ptr2 + (int)Math.Floor((double)v.Length / 4.0) - 1; ptr3 >= ptr2; ptr3--)
                {
                    *ptr3 ^= encKey;
                    encKey += 13;
                }
            }
        }

        public static string Int32_ToString( ref int v)
        {
            return v.ToString();
        }
        public static bool String_Equality(string a, string b)
        {
            return string.Equals(a, b);
        }

        public static string String_ConcatObject(object a, object b)
        {
            return string.Concat(a, b);
        }
        public static string String_Concat3Object(object a, object b, object c)
        {
            return string.Concat(a, b, c);
        }
        public static string String_Concat3String(string a, string b, string c)
        {
            return string.Concat(a, b, c);
        }
        public static string Object_ToString(object v)
        {
            return v.ToString();
        }

        public static bool String_IsNullOrEmpty(string v)
        {
            if (v != null)
            {
                return v.Length == 0;
            }
            return true;
        }
        public static void Monitor_Enter(object a)
        {
            System.Threading.Monitor.Enter(a);
        }

        public static void Monitor_Enter2(object a , ref bool lockTaken)
        {
            System.Threading.Monitor.Enter(a ,ref lockTaken);
        }

        public static void Monitor_Exit(object a)
        {
            System.Threading.Monitor.Exit(a);
        }
        public static string String_Concat(string a, string b)
        {
            return a + b;
        }
        public static void MyDispose(IDisposable obj)
        {
            obj.Dispose();
        }

        private static volatile System.Threading.Thread _CloneStringCrossThead_Thread = null;
        private static readonly System.Threading.AutoResetEvent _CloneStringCrossThead_Event
            = new System.Threading.AutoResetEvent(false);
        private static readonly System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner
            = new System.Threading.AutoResetEvent(false);
        private static volatile string _CloneStringCrossThead_CurrentValue = null;
        /// <summary>
        /// 跨线程的复制字符串值，用于改变创建字符串的调用堆栈
        /// </summary>
        /// <param name="txt">字符串值</param>
        /// <returns>复制品</returns>
        public static string CloneStringCrossThead(string txt)
        {
            if (txt == null || txt.Length == 0)
            {
                return txt;
            }
            try
            {
                System.Threading.Monitor.Enter(_CloneStringCrossThead_Event);
                _CloneStringCrossThead_CurrentValue = txt;
                _CloneStringCrossThead_Event_Inner.Set();
                _CloneStringCrossThead_Event.Reset();
                if (_CloneStringCrossThead_Thread == null)
                {
                    _CloneStringCrossThead_Thread = new System.Threading.Thread(CloneStringCrossThead_Thread);
                    _CloneStringCrossThead_Thread.Start();
                }
                _CloneStringCrossThead_Event.WaitOne(100);
                return _CloneStringCrossThead_CurrentValue;
            }
            finally
            {
                System.Threading.Monitor.Exit(_CloneStringCrossThead_Event);
            }
        }

        private static void CloneStringCrossThead_Thread()
        {
            try
            {
                while (true)
                {
                    if (_CloneStringCrossThead_Event_Inner.WaitOne(1000) == false)
                    {
                        // 等了1秒没任务了，退出线程。
                        break;
                    }

                    if (_CloneStringCrossThead_CurrentValue != null)
                    {
                        _CloneStringCrossThead_CurrentValue = new string(_CloneStringCrossThead_CurrentValue.ToCharArray());
                    }
                    _CloneStringCrossThead_Event.Set();
                    _CloneStringCrossThead_Event_Inner.Reset();
                }
            }
            finally
            {
                _CloneStringCrossThead_Thread = null;
                _CloneStringCrossThead_Event.Reset();
                _CloneStringCrossThead_Event_Inner.Reset();
            }
        }

        public static string GetString(byte[] bsData, int startIndex, int bsLength, int key)
        {
            int chrsLength = bsLength / 2;
            char[] chrs = new char[chrsLength];
            for (int iCount = 0; iCount < chrsLength; iCount++, key++)
            {
                int bi = startIndex + iCount * 2;
                int v = ((bsData[bi] << 8) + bsData[bi + 1]);
                v = v ^ key;
                chrs[iCount] = (char)v;
            }
            return new string(chrs);
        }
        public static System.Drawing.Bitmap GetBitmap(byte[] bsData, int startIndex, int bsLength, int key)
        {
            var bs = new byte[bsLength];
            for (int iCount = 0; iCount < bsLength; iCount++, key++)
            {
                bs[iCount] = (byte)(bsData[startIndex + iCount] ^ key);
            }
            var ms = new System.IO.MemoryStream(bs);
            var bmp = new System.Drawing.Bitmap(ms);
            return bmp;
        }

        /// <summary>
        /// 从字节数组中加载资源数据
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="key"></param>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static System.Resources.ResourceSet LoadResourceSet(byte[] bs, byte key, bool gzip)
        {
            var stream = GetStream(bs, key, gzip);
            var result = new System.Resources.ResourceSet(stream);
            stream.Close();
            return result;
        }
        private static System.IO.Stream GetStream(byte[] bs, byte key, bool gzip)
        {
            int len = bs.Length;
            for (int iCount = 0; iCount < len; iCount++, key++)
            {
                bs[iCount] = ((byte)(bs[iCount] ^ key));
            }
            System.IO.MemoryStream ms = null;
            if (gzip)
            {
                var stream = new System.IO.Compression.GZipStream(
                    new System.IO.MemoryStream(bs),
                    System.IO.Compression.CompressionMode.Decompress);
                ms = new System.IO.MemoryStream();
                var bsTemp = new byte[1024];
                while (true)
                {
                    len = stream.Read(bsTemp, 0, bsTemp.Length);
                    if (len > 0)
                    {
                        ms.Write(bsTemp, 0, len);
                    }
                    else
                    {
                        break;
                    }
                }
                stream.Close();
                ms.Position = 0;
            }
            else
            {
                ms = new System.IO.MemoryStream(bs);
            }
            return ms;
        }
    }

}