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
using System.Runtime.InteropServices;
using System.Reflection;


#if !DOTNETCORE
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
        /// 只针对Release模式的编译结果而执行本程序
        /// </summary>
        /// <remarks>
        /// 当本程序作为VS.NET的编译后事件而运行时，一般而言调试模式无需执行混淆加密操作。Release模式才需要混淆加密。
        /// </remarks>
        public bool OnlyForReleaseAssembly = false;
        /// <summary>
        /// 是否为正常的命令行模式。可以读取键盘输入，可以移动光标。
        /// </summary>
        /// <remarks>当本程序作为VS.NET的编译后事件而运行时，不支持键盘和光标的操作。此时设置本属性为false.</remarks>
        public bool IsNativeConsole = true;

        /// <summary>
        /// 要删除的自定义属性类型全名
        /// </summary>
        public string RemoveCustomAttributeTypeFullNames = null;
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
        /// 针对Blazor WebAssembly进行处理
        /// </summary>
        public bool ForBlazorWebAssembly = false;
        /// <summary>
        /// 在重命名后检测死代码
        /// </summary>
        public DetectDeadCodeMode DetectDeadCode = DetectDeadCodeMode.Disabled;
        /// <summary>
        /// 添加性能累计器
        /// </summary>
        public bool AddPerformanceCounter = false;
        /// <summary>
        /// 指定的重命名类型
        /// </summary>
        public string SpecifyRename = null;
        /// <summary>
        /// 要删除的类型名称
        /// </summary>
        public string RemoveTypes = null;
        /// <summary>
        /// 要删除死代码的类型名称
        /// </summary>
        public string RemoveDeadCodeTypes = null;
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
                        case "BlazorWebAssembly": this.ForBlazorWebAssembly = StringToBoolean(node.InnerText, false);break;
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
                        case "ResourceNameNeedEncrypt": this.ResourceNameNeedEncrypt = node.InnerText;break;
                        case "OutpuptMapXml": this.OutpuptMapXml = StringToBoolean(node.InnerText, true); break;
                        case "DeleteTempFile": this.DeleteTempFile = StringToBoolean(node.InnerText, false); break;
                        case "DebugMode": this.DebugMode = StringToBoolean(node.InnerText, false); break;
                        case "TestUseNGen": this.TestUseNGen = StringToBoolean(node.InnerText, true);break;
                        case "RemoveCustomAttributeTypeFullNames": this.RemoveCustomAttributeTypeFullNames = node.InnerText;break;
                        case "DeadCode":
                            Enum.TryParse<DetectDeadCodeMode>(node.InnerText, out this.DetectDeadCode);
                            break;
                        case "PerformanceCounter": this.AddPerformanceCounter = StringToBoolean(node.InnerText, false);break;
                        case "SpecifyRename": this.SpecifyRename = node.InnerText;break;
                        case "RemoveDeadCodeTypes": this.RemoveDeadCodeTypes = node.InnerText;break;
                        case "OnlyForReleaseAssembly": this.OnlyForReleaseAssembly = StringToBoolean(node.InnerText, false);break;
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
                if (this.ResourceNameNeedEncrypt != null && this.ResourceNameNeedEncrypt.Length > 0 )
                {
                    writer.WriteElementString("ResourceNameNeedEncrypt" , this.ResourceNameNeedEncrypt);
                }
                writer.WriteElementString("OutpuptMapXml", this.OutpuptMapXml.ToString());
                writer.WriteElementString("DeleteTempFile", this.DeleteTempFile.ToString());
                writer.WriteElementString("DebugMode", this.DebugMode.ToString());
                writer.WriteElementString("TestUseNGen", this.TestUseNGen.ToString());
                writer.WriteElementString("BlazorWebAssembly", this.ForBlazorWebAssembly.ToString());
                if (this.RemoveCustomAttributeTypeFullNames != null && this.RemoveCustomAttributeTypeFullNames.Length > 0)
                {
                    writer.WriteElementString("RemoveCustomAttributeTypeFullNames", this.RemoveCustomAttributeTypeFullNames);
                }
                writer.WriteElementString("DeadCode", this.DetectDeadCode.ToString());
                writer.WriteElementString("PerformanceCounter", this.AddPerformanceCounter.ToString());
                if (this.SpecifyRename != null && this.SpecifyRename.Length > 0)
                {
                    writer.WriteElementString("SpecifyRename", this.SpecifyRename);
                }
                if( this.RemoveDeadCodeTypes != null && this.RemoveDeadCodeTypes.Length > 0 )
                {
                    writer.WriteElementString("RemoveDeadCodeTypes", this.RemoveDeadCodeTypes);
                }
                writer.WriteElementString("OnlyForReleaseAssembly", this.OnlyForReleaseAssembly.ToString());
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
        RemoveCustomAttributeTypeFullNames=[optional,specify custom attribute type full names need removed , items split by ',']
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
                var handle33 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.ProjectRun);
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
                eng.RemoveCustomAttributeTypeFullNames = this.RemoveCustomAttributeTypeFullNames;
                eng.ForBlazorWebAssembly = this.ForBlazorWebAssembly;
                eng.DetectDeadCode = this.DetectDeadCode;
                eng.AddPerformanceCounter = this.AddPerformanceCounter;
                eng.SpeicfyRename = this.SpecifyRename;
                eng.RemoveTypes = this.RemoveTypes;
                eng.RemoveDeadCodeTypes = this.RemoveDeadCodeTypes;
                eng.IsNativeConsole = this.IsNativeConsole;
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
                if( this.OnlyForReleaseAssembly && this.InputAssemblyFileName != null )
                {
                    // 检查是否针对Release版本
                    var items = this.InputAssemblyFileName.Split(Path.DirectorySeparatorChar);
                    var bolFlag = false;
                    foreach( var item in items )
                    {
                        if( string.Equals( item , "Release" , StringComparison.OrdinalIgnoreCase) )
                        {
                            bolFlag = true;
                            break;
                        }
                    }
                    if( bolFlag == false )
                    {
                        // 不是Release版本，不处理
                        MyConsole.Instance.WriteLine("   Not release version, cancel process.");
                        return;
                    }
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
                SelfPerformanceCounterForTest.Leave(handle33);
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
}
