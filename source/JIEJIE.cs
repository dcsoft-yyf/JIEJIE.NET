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

#if ! DOTNETCORE
[assembly: AssemblyTitle("JieJie.NETConsoleApplication")]
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
    internal static class ConsoleProgram
    {
#if DCSoftInner
        public static void AppMain(string[] args)
        { 
#else
        [STAThreadAttribute]
        static void Main(string[] args)
        {
           
            
#endif

            string inputAssmblyFileName = null;
            string outputAssemblyFileName = null;
            bool pause = false;
            //const string _SwapCallModeFlag = "4325faf20210820";
            var eng = new DCJieJieNetEngine();
            string mergeFileNames = null;
            var customInstructions = new Dictionary<string, string>(System.StringComparer.CurrentCultureIgnoreCase);
            if (args != null)
            {
                foreach (var arg in args)
                {
                    int index = arg.IndexOf('=');
                    if (index > 0)
                    {
                        string argName = arg.Substring(0, index).Trim().ToLower();
                        string argValue = arg.Substring(index + 1).Trim();
                        if( argName[0]=='.')
                        {
                            customInstructions[argName] = argValue;
                            continue;
                        }
                        switch (argName)
                        {
                            case "translatestack":
                                {
                                    DCJieJieNetEngine.ConsoleTranslateStackTraceUseMapXml(argValue);
                                    return;
                                }
                                break;
                            case "input":
                                inputAssmblyFileName = argValue;
                                if (File.Exists(inputAssmblyFileName) == false)
                                {
                                    return;
                                }
                                break;
                            case "output":
                                outputAssemblyFileName = argValue;
                                break;
                            case "snk":
                                if (argValue != null
                                    && argValue.Length > 0
                                    && File.Exists(argValue) == false)
                                {
                                    ConsoleWriteError("Can not find file : " + argValue);
                                    return;
                                }
                                eng.SnkFileName = argValue;
                                break;
                            case "switch":
                                eng.Switchs = new JieJieSwitchs(argValue, null);
                                break;
                            case "sdkpath":
                                if (argValue != null
                                    && argValue.Length > 0
                                    && Directory.Exists(argValue) == false)
                                {
                                    ConsoleWriteError("Can not find directory : " + argValue);
                                    return;
                                }
                                eng.SDKDirectory = argValue;
                                break;
                            case "prefixfortyperename":
                                if (argValue != null && argValue.Length > 0)
                                {
                                    eng.PrefixForTypeRename = argValue;
                                }
                                break;
                            case "prefixformemberrename":
                                if (argValue != null && argValue.Length > 0)
                                {
                                    eng.PrefixForMemberRename = argValue;
                                }
                                break;
                            case "merge":
                                mergeFileNames = argValue;
                                break;
                        }
                    }
                    else
                    {
                        switch (arg.Trim().ToLower())
                        {
                            case "outputmapxml":eng.OutpuptMapXml = true;break;
                            case "deletetempfile": eng.DeleteTempFile = true; break;
                            case "pause": pause = true; break;
                            case "debugmode": eng.DebugMode = true; break;
                            default:
                                if (arg != null
                                    && arg.Length > 0
                                    && Path.IsPathRooted(arg)
                                    && File.Exists(arg))
                                {
                                    // 默认为输入的程序集的文件全路径名
                                    inputAssmblyFileName = arg;
                                }
                                break;
                        }
                    }
                }
            }//if
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
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
        mapxml   =[optional, a file/directory name to save map infomation for class/member's old name and new name in xml format.]
        pause    =[optional,pause the console after finish process.]
        debugmode=[optional,Allow show some debug info text.]
        sdkpath  =[optional,set the direcotry full name of ildasm.exe.]
        merge    =[optional,some .net assembly file to merge to the result file. '*' for all referenced assembly files.]
        .corflags=[optional, it is a integer flag,'3' for 32-bit process without strong name signature, '1' for 64-bit wihout strong name, '9' for 32-bit with strong name ,'10' for 64-bit with strong name.]
        .subsystem=[optional, it a integer value, '2' for application in GUI mode.'3' for application in console mode.]
        prefixfortyperename=[optional, the prefix use to rename type name.]
        prefixformemberrename=[optional,the prefix use to rename type's member name.]
        deletetempfile=[optional,delete template file after job finshed.default is false.]
     Example 1, protect d:\a.dll ,this will modify dll file.
        >JIEJIE.NET.exe d:\a.dll  
     Exmaple 2, anlyse d:\a.dll , and write result to another dll file with strong name. enable obfuscate control flow and not encript resources.
        >JIEJIE.NET.exe input=d:\a.dll output=d:\publish\a.dll snk=d:\source\company.snk switch=+contorlfow,-resources
**************************** MADE IN CHINA **********************************************";
                if (inputAssmblyFileName != null && inputAssmblyFileName.Length > 0)
                {
                    if (File.Exists(inputAssmblyFileName))
                    {
                        Console.Title = "JieJie.NET - " + inputAssmblyFileName;
                        Console.WriteLine("**************************** MADE IN CHINA **********************************************");
                        Console.ResetColor();
                        var inputDir = Path.GetDirectoryName(inputAssmblyFileName);
                        var startDir = Path.GetDirectoryName(typeof(DCJieJieNetEngine).Assembly.Location);
                        System.AppDomain taskDomain = null;
                        if (string.Compare(startDir, inputDir, true) != 0)
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
                            var taskEng = (DCJieJieNetEngine)taskDomain.CreateInstanceFromAndUnwrap(eng.GetType().Assembly.Location, eng.GetType().FullName);
                            eng.CopytSettingsTo(taskEng);
                            eng = taskEng;
#endif
                        }
                        if (eng.LoadAssemblyFile(inputAssmblyFileName, mergeFileNames) == false)
                        {
                            return;
                        }
                        eng.SetDocumentCustomInstructions( customInstructions );
                        eng.HandleDocument();
                        if (outputAssemblyFileName == null || outputAssemblyFileName.Length == 0)
                        {
                            var dir = Path.Combine(Path.GetDirectoryName(inputAssmblyFileName), "jiejie.net_result");
                            if (Directory.Exists(dir) == false)
                            {
                                Directory.CreateDirectory(dir);
                            }
                            outputAssemblyFileName = Path.Combine(
                                dir, 
                                Path.GetFileName(inputAssmblyFileName));
                        }
                        else if (System.IO.Directory.Exists(outputAssemblyFileName))
                        {
                            outputAssemblyFileName = Path.Combine(
                                outputAssemblyFileName,
                                Path.GetFileName(inputAssmblyFileName));
                        }
                        eng.SaveAssemblyFile(outputAssemblyFileName, true);
                        if (eng.DeleteTempFile)
                        {
                            // 删除临时文件
                            eng.DeleteTemplateDirecotry();
                        }
                        eng.Close();
#if ! DOTNETCORE
                        if (taskDomain != null)
                        {
                            System.AppDomain.Unload(taskDomain);
                        }
#endif
                    }
                    else
                    {
                        ConsoleWriteError("Can not find file : " + inputAssmblyFileName);
                    }
                }
                else
                {
                    Console.WriteLine(strInfoMore);
                }
            }
            catch (System.Exception ext)
            {
                ConsoleWriteError(ext.ToString());
            }
            finally
            {
                if (pause)
                {
                    DCUtils.EatAllConsoleKey();
                    Console.WriteLine("##########  All finished, press any key to continue ############");
                    Console.ReadKey();
                }
            }
        }

        private static void ConsoleWriteError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
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

    internal class RenameMapInfo : SortedDictionary<string, DCILClass>
    {
        public void Register(DCILClass cls)
        {
            this[cls.GetNameWithNestedPlus(true)] = cls;
        }
        public string GetNewClassName(string oldClassName)
        {
            if (oldClassName == null || oldClassName.Length == 0)
            {
                return null;
            }
            DCILClass cls = null;
            if (this.TryGetValue(oldClassName, out cls))
            {
                var result = cls.GetNameWithNestedPlus(false);
                return result;
            }
            return null;
        }
    }
    [Serializable]
    internal class DCJieJieNetEngine : System.MarshalByRefObject
    {
        public const string ProductVersion = "1.5.0.1";


        public DCJieJieNetEngine(DCILDocument doc)
        {
            this.Document = doc;
        }
        public DCJieJieNetEngine()
        {
        }

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
        }
        /// <summary>
        /// 关闭对象
        /// </summary>
        public void Close()
        {
            if (this.Document != null)
            {
                this.Document.Dispose();
                this.Document = null;
            }
            this._CallOperCodes?.Clear();
            this._CallOperCodes = null;
            this._AllClasses?.Clear();
            //this._AllBaseTypes.Clear();
            this._ByteDataContainer = null;
            //this._IDGenForClass = null;
            _NativeTypeMethods.Clear();
            _Native_BaseMethods.Clear();
            this._RuntimeSwitchs.Clear();
            this._Int32ValueContainer = null;
            DCILTypeReference.ClearCacheNativeTypes();

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
                Console.Write("Deleting template directory : " + this.TempDirectory);
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
                Console.WriteLine("Write comment XML file \"" + descFileName + "\" ,remove " + removeCount + "(" + percent + "%)members,span " + tick + " milliseconds.");
                return true;
            }
            catch (System.Exception ext)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ext.ToString());
                Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Can not find file " + mapXmlFileName);
                Console.ResetColor();
                return;
            }

            Console.WriteLine("JIEJIE.NET translate stack trace use MAP.XML ");
            Console.WriteLine("MAP xml file : " + mapXmlFileName);
            Console.Write("Please paste or input stack trace text and press ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("ESC");
            Console.ResetColor();
            Console.WriteLine(" to finish:");
            var strBuffer = new System.Text.StringBuilder();
            while (true)
            {
                var info = Console.ReadKey();
                if (info.Key == ConsoleKey.Escape)
                {
                    break;
                }
                if (info.KeyChar > 7)
                {
                    strBuffer.Append(info.KeyChar.ToString());
                    if (info.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                    }
                }
            }
            var strResult = DCJieJieNetEngine.TranslateStackTraceUseMapXml(mapXmlFileName, strBuffer.ToString());
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" ########## Translate result ##########");
            Console.ResetColor();
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
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(strMethodName.Substring(0, index2));
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(strMethodName.Substring(index2));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(strLine.Substring(0, index));
                    }
                    Console.ResetColor();
                    Console.Write(strLine.Substring(index));
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(strLine);
                }
                strLine = reader.ReadLine();
            }
            reader.Close();
            Console.WriteLine("### Press Enter to exit. ###");
            Console.ReadLine();
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
                        strResult.Append(infos[0].OldSign + "[ !! Maby !!]");
                    }
                    strResult.Append(reader.ReadLineTrim());
                    strResult.AppendLine();
                }
                else if (strWord.IndexOf('.') > 0 && reader.PeekContentChar() == '(')
                {
                    string clsName = null;
                    string methodName = DCUtils.GetShortName(strWord, out clsName);

                    if (clsName != null && clsName.Length > 0)
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
                    }
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
        //private DCILTypeReference[] ParseMethodParameters(DCILReader reader )
        //{
        //    var list = new List<DCILTypeReference>();
        //    bool hasName = false;
        //    DCILTypeReference type = null;
        //    while(reader.HasContentLeft())
        //    {
        //        string strWord = reader.ReadWord();
        //        if( strWord == ",")
        //        {
        //            type = null;
        //        }
        //        else if(type == null )
        //        {
        //            type = DCILTypeReference.Load( strWord, reader);
        //            list.Add(type);
        //        }
        //    }
        //    return list.ToArray();
        //}

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
            methods.Sort(delegate (DCILMethod a, DCILMethod b) {
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
        //public void WriteMapXml(System.Xml.XmlWriter writer)
        //{
        //    if (writer == null)
        //    {
        //        throw new ArgumentNullException("writer");
        //    }
        //    writer.WriteStartDocument();
        //    writer.WriteDocType("dotfuscatorMap", null, "http://www.preemptive.com/dotfuscator/dtd/dotfuscatorMap_v1.2.dtd", null);
        //    writer.WriteStartElement("dotfuscatorMap");
        //    writer.WriteAttributeString("version", "1.1");
        //    writer.WriteStartElement("header");
        //    writer.WriteElementString("timestamp", DateTime.Now.ToString());
        //    writer.WriteStartElement("product");
        //    writer.WriteAttributeString("version", DCJieJieNetEngine.ProductVersion);
        //    writer.WriteAttributeString("user", Environment.UserName);
        //    writer.WriteString("JieJie.NET");
        //    writer.WriteEndElement();//</product>
        //    writer.WriteFullEndElement();//</header>
        //    writer.WriteStartElement("mapping");
        //    writer.WriteStartElement("module");
        //    writer.WriteElementString("name", Path.GetFileName(this.Document.AssemblyFileName));
        //    foreach (var cls in this.Document.Classes)
        //    {
        //        if (cls.InnerGenerate)
        //        {
        //            continue;
        //        }
        //        bool hasRenamedMethod = false;
        //        bool hasRenamedField = false;
        //        foreach (var item2 in cls.ChildNodes)
        //        {
        //            if (item2 is DCILMethod && ((DCILMethod)item2).RenameState == DCILRenameState.Renamed)
        //            {
        //                hasRenamedMethod = true;
        //                break;
        //            }
        //            else if (item2 is DCILField && ((DCILField)item2).RenameState == DCILRenameState.Renamed)
        //            {
        //                hasRenamedField = true;
        //                break;
        //            }
        //        }//foreach

        //        if (cls.RenameState == DCILRenameState.Renamed
        //            || hasRenamedMethod
        //            || hasRenamedField)
        //        {
        //            writer.WriteStartElement("type");
        //            if( cls.RenameState == DCILRenameState.Preserve )
        //            {
        //                writer.WriteElementString("name", cls.Name);
        //            }
        //            else
        //            {
        //                writer.WriteElementString("name", cls.OldName);
        //            }
        //            writer.WriteElementString("newname", cls.Name);
        //            if (hasRenamedMethod)
        //            {
        //                writer.WriteStartElement("methodlist");

        //                foreach (var item2 in cls.ChildNodes)
        //                {
        //                    if (item2 is DCILMethod)
        //                    {
        //                        var method = (DCILMethod)item2;
        //                        if (method.InnerGenerate == false
        //                            && method.RenameState == DCILRenameState.Renamed)
        //                        {
        //                            writer.WriteStartElement("method");
        //                            writer.WriteElementString("signature", method.OldSignature);
        //                            writer.WriteElementString("name", method.OldName);
        //                            writer.WriteElementString("newname", method.Name);
        //                            writer.WriteEndElement();//</method>
        //                        }
        //                    }
        //                }
        //                writer.WriteEndElement();//</methodlist>
        //            }
        //            if (hasRenamedField)
        //            {
        //                writer.WriteStartElement("fieldlist");
        //                foreach (var item2 in cls.ChildNodes)
        //                {
        //                    if (item2 is DCILField)
        //                    {
        //                        var field = (DCILField)item2;
        //                        if (field.RenameState == DCILRenameState.Renamed)
        //                        {
        //                            writer.WriteStartElement("field");
        //                            writer.WriteElementString("signature", field.OldSignature);
        //                            writer.WriteElementString("name", field.OldName);
        //                            writer.WriteElementString("newname", field.Name);
        //                            writer.WriteEndElement();
        //                        }
        //                    }
        //                }
        //                writer.WriteEndElement();// </fieldlist>
        //            }
        //            writer.WriteEndElement();//</type>
        //        }
        //    }
        //    writer.WriteEndElement();//</module>
        //    writer.WriteEndElement();//</mapping>
        //    writer.WriteStartElement("statistics");
        //    writer.WriteEndElement();
        //    writer.WriteFullEndElement();//</dotfuscatorMap>
        //    writer.WriteEndDocument();
        //}

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
            Console.WriteLine("Saving assembly to " + asmFileName);
            var ilFileName = Path.Combine(this.Document.RootPath, "result_" + Path.GetFileName(asmFileName) + ".il");
            Console.WriteLine("    Writing IL codes to " + ilFileName);
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
                    Console.WriteLine("Write rename map xml to\"" + fn2 + "\".");
                }
                if (this.Document.Content_DepsJson != null && this.Document.Content_DepsJson.Length > 0)
                {
                    var fn2 = Path.ChangeExtension(asmFileName, ".deps.json");
                    Console.WriteLine("    Write " + fn2);
                    File.WriteAllBytes(fn2, this.Document.Content_DepsJson);
                }
                if (this.Document.CommentXmlDoc != null && this.Document.CommentXmlDoc.DocumentElement?.Name == "doc")
                {
                    // clean document comment xml file.
                    var comXmlFileName = Path.ChangeExtension(asmFileName, ".xml");
                    WriteDocumentCommentXml(comXmlFileName);
                }
                if (checkUseNgen)
                {
                    ConsoleWriteTask();
#if !DOTNETCORE // .NET Core not support ngen.exe
                    Console.WriteLine("Testing by ngen.exe...");
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
                        DCUtils.RunExe(pathNgen, "install \"" + asmFileName + "\"  /NoDependencies");
                        DCUtils.RunExe(pathNgen, "uninstall \"" + asmFileName + "\"");
                    }
                    else
                    {
                        Console.WriteLine("can not find file : " + pathNgen);
                    }
#else
                    Console.WriteLine(".NET Core not support ngen.exe");
#endif
                }
                Console.WriteLine();
                ConsoleWriteTask();
                Console.Write("Job finished, final save to :");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(asmFileName);
                Console.ResetColor();
                if (this._SourceAssemblyFileSize > 0)
                {
                    Console.Write(" Source file size : " + DCUtils.FormatByteSize(this._SourceAssemblyFileSize) + " ,");
                }
                var newFileLength = new FileInfo(asmFileName).Length;
                Console.WriteLine(" Result file size : " + DCUtils.FormatByteSize(newFileLength));

                return true;
            }
            else
            {
                ConsoleWriteTask();
                Console.WriteLine("Job failed.");
                return false;
            }
        }
        private string _InputAssemblyFileName = null;
        private string _InputAssemblyDirectory = null;
        internal string _UseAnotherExeName = null;
        private int _SourceAssemblyFileSize = 0;

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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine();
#if DOTNETCORE
                    Console.WriteLine("   Warring!!! This program not support .NET Framework,please use '" + this._UseAnotherExeName + "'.");
#else
                    Console.WriteLine("   Warring!!! This program not support .NET Core,please use '" + this._UseAnotherExeName + "'.");
#endif
                    Console.ResetColor();
                    return false;
                }
                this.Document = documents[0];
                if (documents.Count > 1)
                {
                    ConsoleWriteTask();
                    Console.WriteLine("Merge other assembly files to " + Path.GetFileName(this.Document.AssemblyFileName));
                    // 进行文档合并
                    this.Document = DCILDocument.MergeDocuments(documents);
                }
                else
                {
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
            Console.WriteLine("Loading assembly file " + asmFileName);
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
            Console.Write("    Anlysing IL file...");
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
                    Console.WriteLine("      Fail to load " + xmlFileName + " . " + ext.Message);
                }
            }
            Console.WriteLine(" span " + Math.Abs(Environment.TickCount - tick) + " milliseconds. get " + doc.Classes.Count + " classes.");
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
            this._CallOperCodes = new List<DCILOperCode_HandleMethod>();
            this.SelectUILanguage();

            this.AddClassJIEJIEHelper();
            if (this.Switchs.Resources)
            {
                this.ApplyResouceContainerClass();
            }
            if (this.Switchs.ControlFlow)
            {
                this.EncryptCharValue();
            }
            var clses = new List<DCILClass>(this.Document.Classes);
            foreach (var cls in clses)
            {
                HandleClass(cls, this.Switchs);
            }
            if (this.Switchs.ControlFlow)
            {
                this.EncryptTypeHandle();
                this.EncryptMethodParamterEnumValue();
            }
            if (this.Switchs.Strings)
            {
                this.HandleCollectStringValue();
            }
            this.AddDatasClass();
            this._RFHContainer?.Commit();
            this.Document.FixDomState();
            if (this.Switchs.ControlFlow)
            {

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
                if (this.Switchs.Rename)
                {
                    //ChangeEnumParamterToInteger();
                }
                this.CollectStatcMethod();
                if (this._Int32ValueContainer != null)
                {
                    this._Int32ValueContainer.Commit();
                    ObfuscateOperCodeList(
                        this._Int32ValueContainer._Class.Method_Cctor,
                        this._Int32ValueContainer._Class.Method_Cctor.OperCodes,
                        false,
                        null);
                    DCUtils.ObfuseListOrder(this._Int32ValueContainer._Class.ChildNodes);
                }
                if (this._RFHContainer != null)
                {
                    ObfuscateOperCodeList(
                        this._RFHContainer._Class.Method_Cctor,
                        this._RFHContainer._Class.Method_Cctor.OperCodes,
                        false,
                        null);
                }
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
                        if(eit2 != null )
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

        //private void ChangeEnumParamterToInteger()
        //{
        //    int methodCount = 0;
        //    foreach( var cls in this.Document.GetAllClassesUseCache().Values )
        //    {
        //        foreach( var item in cls.ChildNodes )
        //        {
        //            if(item is DCILMethod )
        //            {
        //                var method = (DCILMethod)item;
        //                if( method.HasGenericStyle == false && method.RenameState == DCILRenameState.Renamed )
        //                {
        //                    if (method.OperCodes != null && method.OperCodes.Count < 20)
        //                    {
        //                        bool changed = true;
        //                        foreach (var item2 in method.OperCodes)
        //                        {
        //                            if (item2 is DCILOperCode_HandleMethod || item2 is DCILOperCode_Try_Catch_Finally)
        //                            {
        //                                changed = false;
        //                                break;
        //                            }
        //                        }
        //                        if (changed && method.Parameters != null && method.Parameters.Count > 0)
        //                        {
        //                            changed = false;
        //                            foreach (var p in method.Parameters)
        //                            {
        //                                var pcls = p.ValueType?.LocalClass;
        //                                if (pcls != null && pcls.IsEnum)
        //                                {
        //                                    foreach (var f in pcls.ChildNodes)
        //                                    {
        //                                        if (f.Name == "value__")
        //                                        {
        //                                            p.ValueType = ((DCILField)f).ValueType;
        //                                            changed = true;
        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            if( changed )
        //                            {
        //                                method._Name = method.Name + "_";
        //                            }
        //                        }
        //                    }
        //                    //if (method.Locals != null && method.Locals.Count > 0)
        //                    //{
        //                    //    bool changed = false;
        //                    //    foreach (var p in method.Locals)
        //                    //    {
        //                    //        if (p.ValueType.LocalClass != null && p.ValueType.LocalClass.IsEnum)
        //                    //        {
        //                    //            var vf = (DCILField)p.ValueType.LocalClass.ChildNodes.GetByName("value__");
        //                    //            if (vf != null)
        //                    //            {
        //                    //                p.ValueType = vf.ValueType;
        //                    //                changed = true;
        //                    //            }
        //                    //        }
        //                    //    }
        //                    //    if(changed )
        //                    //    {
        //                    //        //method._Name = method.Name + "___" + Convert.ToString(methodCount++);
        //                    //    }
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //}
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
                        Console.WriteLine("            Move static methods : " + oldClsName + "::" + oldMethodName);
                    }
                }//for
                DCUtils.ObfuseListOrder(newClass.ChildNodes);
                this.Document.Classes.Add(newClass);
                ConsoleWriteTask();
                Console.WriteLine("Move " + methods.Count + " static methods.");
            }
        }

        private void ChangeCallOperCodes(List<DCILOperCode_HandleMethod> callCodes)
        {
            ConsoleWriteTask();
            Console.Write("Start package member property...");
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
                if (item.Value > 5)
                {
                    var method = item.Key;
                    //if (method.Name == "get_ID" && method.Parent.Name == "DCSoft.Common.BackgroundTask")
                    //{

                    //}
                    if (method.IsFinal == false)
                    {
                        if (method.IsVirtual || method.IsNewslot || method.IsAbstract)
                        {
                            continue;
                        }
                    }
                    //if (method.Parent.Name == "DCSoft.Writer.DocumentBehaviorOptions")
                    //{

                    //}
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
            Console.WriteLine(" create " + newMethodCount + " methods , change " + changeCodeCount + " call/callvirt instructions. span " + tick + " milliseconds.");
        }

        private void RemoveMember()
        {
            ConsoleWriteTask();
            Console.Write("Remove Type members ...");
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
                if (GetRuntimeSwitchs(cls, this.Switchs).RemoveMember)
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
                        Console.WriteLine("     " + item.Key + " \t remove " + item.Value.Item1 + " const fields.");
                    }
                    else
                    {
                        Console.WriteLine("     " + item.Key + " \t remove " + item.Value.Item1 + " const fields, " + item.Value.Item2 + " properties.");
                    }
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
            Console.WriteLine("    Total remove " + removeCount + " members , span " + tick + " milliseconds.");
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
            this._UILanguageName = null;
            var allResFiles = this.Document.Resouces;
            if (allResFiles.Count > 0)
            {
                var culs = this.Document.GetSupportCultures();
                if (culs != null && culs.Length > 0)
                {
                    ConsoleWriteTask();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please select UI language you want:");
                    Console.ForegroundColor = ConsoleColor.Green;
                    for (int iCount = 0; iCount < culs.Length; iCount++)
                    {
                        Console.WriteLine("    " + iCount + ":" + culs[iCount].Name + " " + culs[iCount].DisplayName);
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Please input index,press enter to use default,");
                    Console.ResetColor();
                    DCUtils.EatAllConsoleKey();
                    int countDown = 15;
                    int left = Console.CursorLeft;
                    int top = Console.CursorTop;
                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey();
                            int index = key.KeyChar - '0';
                            if (index >= 0 && index < culs.Length)
                            {
                                Console.WriteLine();
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
                            Console.CursorLeft = left;
                            Console.CursorTop = top;
                            countDown--;
                            if (countDown < 0)
                            {
                                break;
                            }
                            Console.Write("(" + countDown.ToString("00") + "):");
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        private void ConsoleWriteTask()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("Task");
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("]");
            Console.ResetColor();
            Console.Write(" ");
        }
        public void HandleCollectStringValue()
        {
            int tick = Environment.TickCount;
            ConsoleWriteTask();
            Console.Write("Encrypting string values ...");
            var strNativeCodes = new List<DCILOperCode_LoadString>();
            this.Document.EnumAllOperCodes(delegate (EnumOperCodeArgs args) {
                if (args.Current is DCILOperCode_LoadString)
                {
                    strNativeCodes.Add((DCILOperCode_LoadString)args.Current);
                }
            });
            //var allClasses = this.GetAllClasses();
            //foreach (var cls in allClasses)
            //{
            //    if( cls.IsImport )
            //    {
            //        continue;
            //    }
            //    foreach (var item in cls.ChildNodes)
            //    {
            //        if (item is DCILMethod)
            //        {
            //            var method = (DCILMethod)item;
            //            method.CollectOperCodes<DCILOperCode_LoadString>(strNativeCodes);
            //        }
            //    }
            //}
            if (strNativeCodes.Count == 0)
            {
                Console.WriteLine(" Can not find any string values in assembly.");
                return;
            }
            var strLdstrOperCodeGroups = new Dictionary<string, List<DCILOperCode_LoadString>>();
            var emptyILCode = new DCILOperCode(
                null,
                "ldsfld",
                "string [" + this.Document.LibName_mscorlib + "]System.String::Empty");
            foreach (var item in strNativeCodes)
            {
                if (item.FinalValue.Length == 0)
                {
                    item.ReplaceCode = emptyILCode;
                }
                else
                {
                    List<DCILOperCode_LoadString> list = null;
                    if (strLdstrOperCodeGroups.TryGetValue(item.FinalValue, out list) == false)
                    {
                        list = new List<DCILOperCode_LoadString>();
                        strLdstrOperCodeGroups[item.FinalValue] = list;
                    }
                    list.Add(item);
                }
            }
            var allStrings = new List<string>(strLdstrOperCodeGroups.Count);
            allStrings.AddRange(strLdstrOperCodeGroups.Keys);
            DCUtils.ObfuseListOrder(allStrings);

            var strList = new List<string>(strLdstrOperCodeGroups.Keys);

            // 混淆字符串次序
            DCUtils.ObfuseListOrder(strList);
            // 字符串分组最大长度
            const int maxGroupSize = 100;

            int strListCount = strList.Count;
            var lstItems = new StringValueContainer();
            lstItems.LibName_mscorlib = this.Document.LibName_mscorlib;
            for (int pos = 0; pos < strListCount; pos += maxGroupSize)
            {
                int groupIndex = pos / maxGroupSize;
                int groupLength = Math.Min(strListCount - pos, maxGroupSize);
                if (groupLength == 0)
                {
                    break;
                }
                var clsName = _ClassNamePrefix + groupIndex;

                var strNewClassCode = new StringBuilder();
                strNewClassCode.AppendLine(".class private auto ansi abstract sealed beforefieldinit " + clsName + " extends [" + this.Document.LibName_mscorlib + "]System.Object");
                strNewClassCode.AppendLine("{");
                lstItems.LibName_mscorlib = this.Document.LibName_mscorlib;
                lstItems.Clear();
                var newILCodes = new List<DCILOperCode_HandleField>();
                for (int iCount = 0; iCount < groupLength; iCount++)
                {
                    string strValue = strList[pos + iCount];
                    lstItems.AddItem(iCount, strValue);
                    var newILCode = new DCILOperCode_HandleField(
                        null,
                        "ldsfld",
                        new DCILReader("string " + clsName + "::_" + iCount.ToString(), this.Document));
                    newILCodes.Add(newILCode);
                    foreach (var item in strLdstrOperCodeGroups[strValue])
                    {
                        item.ReplaceCode = newILCode;
                        if (item.OwnerMethod != null)
                        {
                            item.OwnerMethod.ILCodesModified = true;
                        }
                    }
                    strNewClassCode.AppendLine("    .field public static initonly string _" + iCount.ToString());
                }
                lstItems.RefreshState();
                strNewClassCode.AppendLine(@"
.method private hidebysig specialname rtspecialname static void .cctor () cil managed 
{
	.maxstack 3
	.locals init (
		[0] uint8[] datas
	)
	// (no C# code)
	IL_0000: nop
	IL_0001: call uint8[] " + this._ByteDataContainer.GetMethodName(lstItems.Datas) + @"()
	IL_0003: stloc.0");
                int lableIDCount = 0x30;
                foreach (var item in lstItems.Items)
                {
                    /*
    // _1 = GetStringByLong(data, 9223372036854775806L);
    IL_0007: ldloc.0
    IL_0008: ldc.i8 9223372036854775806
    IL_0011: call string WindowsFormsApp1.Class3::GetStringByLong(uint8[], int64)
    IL_0016: stsfld string WindowsFormsApp1.Class3::_1
                     */
                    //var valueIndex = item.Item1;
                    //strNewClassCode.AppendLine();
                    //long key = item.Item2;
                    //key = (key << 24) + item.Item3;
                    //key = (key << 16) + (ushort)(item.Item4 ^ keyOffset);

                    //var v2 = GetStringByLong(lstDatas.ToArray(), key);
                    //int key2 = (int)(key & 0xffff);
                    //key >>= 16;
                    //int length = (int)(key & 0xfffff);
                    //key >>= 24;
                    //int startIndex = (int)key;

                    lableIDCount += 10; strNewClassCode.AppendLine("        IL_" + Convert.ToString(lableIDCount) + ": ldloc.0");
                    lableIDCount += 10; strNewClassCode.AppendLine("        IL_" + Convert.ToString(lableIDCount) + ": ldc.i8 " + item.LongParamter);
                    lableIDCount += 10; strNewClassCode.AppendLine("        IL_" + Convert.ToString(lableIDCount) + ": call string " + clsName + "::dcsoft(uint8[], int64)");
                    lableIDCount += 10; strNewClassCode.AppendLine("        IL_" + Convert.ToString(lableIDCount) + ": stsfld string " + clsName + "::_" + item.ValueIndex);

                }//foreach
                strNewClassCode.AppendLine(@"
    IL_FFF03: ret
}// end of method");
                strNewClassCode.AppendLine(lstItems.IL_GetStringLong);
                strNewClassCode.AppendLine("}// end of class");
                strNewClassCode.Replace("[mscorlib]", '[' + this.Document.LibName_mscorlib + ']');
                //strNewClassCode.AppendLine("}// end of class " + clsName);
                var txtNewClassCodeText = strNewClassCode.ToString();

                var newCls = new DCILClass(txtNewClassCodeText, this.Document);
                //newCls.CacheInfo(this.Document, null);
                this.Document.Classes.Add(newCls);
                if (this.Switchs.ControlFlow)
                {
                    foreach (var item in newCls.ChildNodes)
                    {
                        if (item is DCILMethod)
                        {
                            ObfuscateOperCodeList((DCILMethod)item);
                        }
                    }
                }
                foreach (var item in newILCodes)
                {
                    item.Value.UpdateLocalField(newCls);
                }
            }
            this.Document.ClearCacheForAllClasses();
            this._AllClasses = null;
            tick = Math.Abs(Environment.TickCount - tick);
            Console.WriteLine(" Handle " + strNativeCodes.Count + " string defines , "
                + strList.Count + " string values, span " + tick + " milliseconds.");
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
        /// <param name="clsNameMaps"></param>
        /// <returns></returns>
        private int RenameByOverrideList(List<DCILClass> allClasses, IDGenerator idGen, RenameMapInfo clsNameMaps)
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

        private static readonly Dictionary<DCILClass, List<DCILMethod>> _Cache_BaseMethods
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
                        localClass = this.Document.GetClassNode(baseType.Name);
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
            public bool ContainsInstanceIndex(int index)
            {
                foreach (var item in this)
                {
                    if (item.InstanceIndex == index)
                    {
                        return true;
                    }
                }
                return false;
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
            Console.Write("Renaming.....");

            int curPos = Console.CursorLeft * Console.CursorTop;

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
                        m.OldSignatureForMap = m.GetSignatureForMap();
                    }
                }//foreach
            }//foreach

            var clsNameMaps = new RenameMapInfo();
            // 根据函数重载关系来重命名。
            int result = RenameByOverrideList(allClasses, idGenForMember, clsNameMaps);

            IDGenerator.GenCountBase = idGenForMember.GenCount + 1;
            var countBaseGenMember = idGenForMember.GenCount + 1;
            // 执行函数的重命名
            this._IDGenForClass = new IDGenerator(this.PrefixForTypeRename, this.PrefixForMemberRename);
            this._IDGenForClass.DebugMode = this.DebugMode;
            int genCountStart = _Random.Next(10, 100);
            foreach (var cls in allClasses)
            {
                if (cls.IsImport)
                {
                    // 外界导入的COM接口，则不改名
                    continue;
                }
                var clsOs = cls.ObfuscationSettings;
                //cls.RemoveObfuscationAttribute();
                if (clsOs == null || clsOs.Exclude == false)
                {
                    cls.OldName = cls.Name;
                    clsNameMaps.Register(cls);
                    var newName = this._IDGenForClass.GenerateIDForClass(cls.OldName, cls);
                    if (cls.Parent is DCILClass)
                    {
                        // 内嵌的类型，则去除命名空间
                        int index4 = newName.LastIndexOf('.');
                        if (index4 > 0)
                        {
                            newName = newName.Substring(index4 + 1);
                        }
                    }
                    cls.ChangeName(newName);
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
            eba.AttributeTypeName = "System.ComponentModel.EditorBrowsableAttribute";
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
                var clsAddEBA = cls.AddEditorBrowsableAttributeForRename(eba);
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        var itemMember = (DCILMemberInfo)item;
                        itemMember.RemoveObfuscationAttribute();
                        if (clsAddEBA == false && (itemMember.HasStyle("family") || itemMember.HasStyle("public")))
                        {
                            itemMember.AddEditorBrowsableAttributeForRename(eba);
                        }
                        if (item is DCILField)
                        {
                            //if(item.Name == "_BackImage12")
                            //{

                            //}
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
            if( Console.CursorLeft * Console.CursorTop != curPos)
            {
                Console.WriteLine();
            }
            Console.WriteLine(" span " + tick + " milliseconds.");
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
                Console.WriteLine(strResult.ToString());
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
                    var newPName = "p" + Convert.ToString(pCount++);
                    maps[p.Name] = newPName;
                    p.Name = newPName;
                }
                var codes = method.GetAllOperCodes<DCILOperCode>();
                if (codes != null && codes.Count > 0)
                {
                    foreach (var code in codes)
                    {
                        var operCode = code.OperCode;
                        if (operCode == "ldarg.s" || operCode == "ldarga.s" || operCode == "starg.s")
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

        private void HandleMethod(DCILMethod method, JieJieSwitchs parentOptions)
        {
            //if( method.Name == "TryConvertTo" || method.OldName == "TryConvertTo")
            //{

            //}
            var opts = GetRuntimeSwitchs(method, parentOptions);
            if (opts.AllocationCallStack && method.ReturnTypeInfo == DCILTypeReference.Type_String)
            {
                // 加密关键字符串对象创建调用堆栈
                for (int ilIndex = method.OperCodes.Count - 1; ilIndex >= 0; ilIndex--)
                {
                    var ilcode = method.OperCodes[ilIndex];
                    if (ilcode.OperCode == "ret")
                    {
                        method.ILCodesModified = true;
                        method.OperCodes.Insert(
                            ilIndex,
                            new DCILOperCode(
                                "IL_zzzzz",
                                "call",
                                "string " + _ClassName_JIEJIEHelper + "::CloneStringCrossThead(string)"));
                        _ModifiedCount++;
                        break;
                    }
                }
            }
            if (opts.ControlFlow && method.OperCodeSpecifyStructure == false)
            {
                if (method.OperCodes != null && method.OperCodes.Count > 0)
                {
                    ObfuscateOperCodeList(method);
                }
            }
        }

        private void HandleClass(DCILClass cls, JieJieSwitchs parentOptions)
        {
            if (cls.IsImport)
            {
                //COM导入的类型不进行任何处理
                return;
            }
            var opts = GetRuntimeSwitchs(cls, parentOptions);
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
                    HandleMethod((DCILMethod)item, opts);
                }
                else if (item is DCILClass)
                {
                    HandleClass((DCILClass)item, opts);
                }
            }
        }

        private ByteArrayDataContainer _ByteDataContainer = new ByteArrayDataContainer();


        private class ByteArrayDataContainer
        {
            public string FullClassName = DCJieJieNetEngine._ClassNamePrefix + "ByteArrayDataContainer";

            private List<int> _FieldIndexs = new List<int>();
            public string GetFieldName(byte[] data)
            {
                var index = IndexOf(data);
                if (_FieldIndexs.Contains(index) == false)
                {
                    _FieldIndexs.Add(index);
                }
                return FullClassName + "::_" + index;
            }
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
                    var lableGen = new ILLabelIDGen();
                    foreach (var index in _FieldIndexs)
                    {
                        var field = eng.Int32ValueContainer.GetField(_Datas[index].Length);
                        if (field == null)
                        {
                            str.AppendLine(lableGen.Gen() + ": ldc.i4 " + _Datas[index].Length);
                        }
                        else
                        {
                            str.AppendLine(lableGen.Gen() + ": ldsfld int32 " + field.Parent.Name + "::" + field.Name);
                        }
                        str.AppendLine(lableGen.Gen() + ": newarr [" + LibName_mscorlib + "]System.Byte");
                        str.AppendLine(lableGen.Gen() + ": dup");
                        str.AppendLine(lableGen.Gen() + ": ldtoken field valuetype " + FullClassName + @"/_DATA" + index + " " + FullClassName + @"::_Field" + index);
                        str.AppendLine(lableGen.Gen() + ": call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)");
                        str.AppendLine(lableGen.Gen() + ": stsfld uint8[] " + FullClassName + "::_" + index);
                    }
                    str.AppendLine(lableGen.Gen() + ": ret");
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
    IL_000d: call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)
	//IL_0012: stloc.0
	//IL_0013: br.s IL_0015
	//IL_0015: ldloc.0
	IL_0016: ret
}
");
                }
                str.AppendLine("}");
                var cls = new DCILClass(str.ToString(), document);
                document.Classes.Add(cls);
                document.ClearCacheForAllClasses();
                var datas = new Dictionary<string, DCILData>();
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    var item = new DCILData();
                    item._Name = "I_BDC" + iCount;
                    item.DataType = "bytearray";
                    item.Value = _Datas[iCount];
                    document.ILDatas.Add(item);
                    datas[item.Name] = item;
                }
                this.LocalClass = cls;
                var clses = document.GetAllClassesUseCache();
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        ((DCILMemberInfo)item).CacheInfo(document, clses);
                        if (item is DCILField)
                        {
                            var field = (DCILField)item;
                            if (field.DataLabel != null && field.DataLabel.Length > 0)
                            {
                                datas.TryGetValue(field.DataLabel, out field.ReferenceData);
                            }
                        }
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
                    if (res.ResourceValues.Count == 0)
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
                    newCls.InnerGenerate = false;
                    cls.CustomAttributes = null;
                    cls.ChildNodes = newCls.ChildNodes;
                    foreach (DCILObject item in cls.ChildNodes)
                    {
                        item.Parent = cls;
                    }
                    this._ModifiedCount++;
                    this.Document.Resouces.Remove(resName);
                    var fn = Path.Combine(this.Document.RootPath, resName);
                    if (File.Exists(fn))
                    {
                        File.Delete(fn);
                    }
                }
            }//for
        }

        private static readonly string LibNameForComponentResourceManager = typeof(System.ComponentModel.ComponentResourceManager).Assembly.GetName().Name;
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
                                    && codes[iCount + 1].OperCode == "call"
                                    && codes[iCount + 2].OperCode == "newobj")
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
                                            if (code.OperCode == "callvirt")
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
                    HandleClass(cls, this.Switchs);
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
        //private string ReplaceTypeLibName( string code , string typeName , Type nativeType )
        //{
        //    if( typeName == null && nativeType != null )
        //    {
        //        typeName = DCUtils.GetFullName(nativeType);
        //    }
        //    string name2 = this.Document.GetTypeNameWithLibraryName(typeName, null, nativeType);
        //    code = code.Replace("[mscorlib]" + typeName, name2);
        //    return code;
        //}
        private DCILTypeReference _Type_JIEJIEHelper = null;
        private static readonly string _ClassName_JIEJIEHelper = "__DC20211119.JIEJIEHelper";
        private static readonly string _Code_Template_JIEJIEHelper = @"
.class private auto ansi abstract sealed beforefieldinit __DC20211119.JIEJIEHelper
	extends [mscorlib]System.Object
{
	// Fields
	.field private static class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_Thread
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner
	.field private static string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_CurrentValue

	// Methods
    .method public hidebysig static 
	    string String_Trim (
		    string v
	    ) cil managed 
    {
	    .maxstack 2
	    IL_0001: ldarg.0
	    IL_0002: callvirt instance string [mscorlib]System.String::Trim()
	    IL_000b: ret
    } 

	.method public hidebysig static 
		class [mscorlib]System.Type Object_GetType (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: callvirt instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
		IL_0006: ret
	} // end of method JIEJIEHelper::Object_GetType

	.method public hidebysig static 
		void MyInitializeArray (
			class [mscorlib]System.Array v,
			valuetype [mscorlib]System.RuntimeFieldHandle fldHandle,
			int32 encKey
		) cil managed 
	{
		// Method begins at RVA 0x2058
		// Code size 107 (0x6b)
		.maxstack 3
		.locals init (
			[0] uint8* ptr,
			[1] uint8[] pinned,
			[2] int32* ptr2,
			[3] int32* ptr3
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array, valuetype [mscorlib]System.RuntimeFieldHandle)
		IL_0007: ldarg.0
		IL_0008: callvirt instance int32 [mscorlib]System.Array::get_Length()
		IL_000d: ldc.i4.4
		IL_000e: blt.s IL_0013

		IL_0010: ldarg.2
		IL_0011: brtrue.s IL_0014

		IL_0013: ret

		IL_0014: ldarg.0
		IL_0015: castclass uint8[]
		IL_001a: dup
		IL_001b: stloc.1
		IL_001c: brfalse.s IL_0023

		IL_001e: ldloc.1
		IL_001f: ldlen
		IL_0020: conv.i4
		IL_0021: brtrue.s IL_0028

		IL_0023: ldc.i4.0
		IL_0024: conv.u
		IL_0025: stloc.0
		IL_0026: br.s IL_0031

		IL_0028: ldloc.1
		IL_0029: ldc.i4.0
		IL_002a: ldelema [mscorlib]System.Byte
		IL_002f: conv.u
		IL_0030: stloc.0

		IL_0031: ldloc.0
		IL_0032: stloc.2
		IL_0033: ldloc.2
		IL_0034: ldarg.0
		IL_0035: callvirt instance int32 [mscorlib]System.Array::get_Length()
		IL_003a: conv.r8
		IL_003b: ldc.r8 4
		IL_0044: div
		IL_0045: call float64 [mscorlib]System.Math::Floor(float64)
		IL_004a: conv.i4
		IL_004b: conv.i
		IL_004c: ldc.i4.4
		IL_004d: mul
		IL_004e: add
		IL_004f: ldc.i4.4
		IL_0050: sub
		IL_0051: stloc.3
		IL_0052: br.s IL_0064
		// loop start (head: IL_0064)
			IL_0054: ldloc.3
			IL_0055: dup
			IL_0056: ldind.i4
			IL_0057: ldarg.2
			IL_0058: xor
			IL_0059: stind.i4
			IL_005a: ldarg.2
			IL_005b: ldc.i4.s 13
			IL_005d: add
			IL_005e: starg.s encKey
			IL_0060: ldloc.3
			IL_0061: ldc.i4.4
			IL_0062: sub
			IL_0063: stloc.3

			IL_0064: ldloc.3
			IL_0065: ldloc.2
			IL_0066: bge.un.s IL_0054
		// end loop

		IL_0068: ldnull
		IL_0069: stloc.1
		IL_006a: ret
	} // end of method JIEJIEHelper::MyInitializeArray

	.method public hidebysig static 
		string Int32_ToString (
			int32& v
		) cil managed 
	{
		// Method begins at RVA 0x20cf
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.s v
		IL_0002: call instance string [mscorlib]System.Int32::ToString()
		IL_0007: ret
	} // end of method JIEJIEHelper::Int32_ToString

	.method public hidebysig static 
		bool String_Equality (
			string a,
			string b
		) cil managed 
	{
		// Method begins at RVA 0x20d8
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call bool [mscorlib]System.String::Equals(string, string)
		IL_0007: ret
	} // end of method JIEJIEHelper::String_Equality

	.method public hidebysig static 
		string String_ConcatObject (
			object a,
			object b
		) cil managed 
	{
		// Method begins at RVA 0x20e1
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call string [mscorlib]System.String::Concat(object, object)
		IL_0007: ret
	} // end of method JIEJIEHelper::String_ConcatObject

	.method public hidebysig static 
		string String_Concat3Object (
			object a,
			object b,
			object c
		) cil managed 
	{
		// Method begins at RVA 0x20ea
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldarg.2
		IL_0003: call string [mscorlib]System.String::Concat(object, object, object)
		IL_0008: ret
	} // end of method JIEJIEHelper::String_Concat3Object

	.method public hidebysig static 
		string String_Concat3String (
			string a,
			string b,
			string c
		) cil managed 
	{
		// Method begins at RVA 0x20f4
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldarg.2
		IL_0003: call string [mscorlib]System.String::Concat(string, string, string)
		IL_0008: ret
	} // end of method JIEJIEHelper::String_Concat3String

	.method public hidebysig static 
		string Object_ToString (
			object v
		) cil managed 
	{
		// Method begins at RVA 0x20fe
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: callvirt instance string [mscorlib]System.Object::ToString()
		IL_0006: ret
	} // end of method JIEJIEHelper::Object_ToString

	.method public hidebysig static 
		bool String_IsNullOrEmpty (
			string v
		) cil managed 
	{
		// Method begins at RVA 0x2106
		// Code size 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: brfalse.s IL_000d

		IL_0003: ldarg.0
		IL_0004: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_0009: ldc.i4.0
		IL_000a: ceq
		IL_000c: ret

		IL_000d: ldc.i4.1
		IL_000e: ret
	} // end of method JIEJIEHelper::String_IsNullOrEmpty

	.method public hidebysig static 
		void Monitor_Enter (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x2116
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call void [mscorlib]System.Threading.Monitor::Enter(object)
		IL_0006: ret
	} // end of method JIEJIEHelper::Monitor_Enter

	.method public hidebysig static 
		void Monitor_Exit (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x211e
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call void [mscorlib]System.Threading.Monitor::Exit(object)
		IL_0006: ret
	} // end of method JIEJIEHelper::Monitor_Exit

	.method public hidebysig static 
		string String_Concat (
			string a,
			string b
		) cil managed 
	{
		// Method begins at RVA 0x2126
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: call string [mscorlib]System.String::Concat(string, string)
		IL_0007: ret
	} // end of method JIEJIEHelper::String_Concat

	.method public hidebysig static 
		void MyDispose (
			class [mscorlib]System.IDisposable obj
		) cil managed 
	{
		// Method begins at RVA 0x212f
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: callvirt instance void [mscorlib]System.IDisposable::Dispose()
		IL_0006: ret
	} // end of method JIEJIEHelper::MyDispose

	.method public hidebysig static 
		string CloneStringCrossThead (
			string txt
		) cil managed 
	{
		// Method begins at RVA 0x2138
		// Code size 139 (0x8b)
		.maxstack 2
		.locals init (
			[0] class [mscorlib]System.Threading.AutoResetEvent,
			[1] bool,
			[2] string
		)

		IL_0000: ldarg.0
		IL_0001: brfalse.s IL_000b

		IL_0003: ldarg.0
		IL_0004: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_0009: brtrue.s IL_000d

		IL_000b: ldarg.0
		IL_000c: ret

		IL_000d: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
		IL_0012: stloc.0
		IL_0013: ldc.i4.0
		IL_0014: stloc.1
		.try
		{
			IL_0015: ldloc.0
			IL_0016: ldloca.s 1
			IL_0018: call void [mscorlib]System.Threading.Monitor::Enter(object)
			IL_001d: ldarg.0
			IL_001e: volatile.
			IL_0020: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
			IL_0025: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
			IL_002a: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
			IL_002f: pop
			IL_0030: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0035: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_003a: pop
			IL_003b: volatile.
			IL_003d: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_0042: brtrue.s IL_0068

			IL_0044: ldnull
			IL_0045: ldftn void __DC20211119.JIEJIEHelper::CloneStringCrossThead_Thread()
			IL_004b: newobj instance void [mscorlib]System.Threading.ThreadStart::.ctor(object, native int)
			IL_0050: newobj instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
			IL_0055: volatile.
			IL_0057: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_005c: volatile.
			IL_005e: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_0063: callvirt instance void [mscorlib]System.Threading.Thread::Start()

			IL_0068: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_006d: ldc.i4.s 100
			IL_006f: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
			IL_0074: pop
			IL_0075: volatile.
			IL_0077: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
			IL_007c: stloc.2
			IL_007d: leave.s IL_0089
		} // end .try
		finally
		{
			IL_007f: ldloc.1
			IL_0080: brfalse.s IL_0088

			IL_0082: ldloc.0
			IL_0083: call void [mscorlib]System.Threading.Monitor::Exit(object)

			IL_0088: endfinally
		} // end handler

		IL_0089: ldloc.2
		IL_008a: ret
	} // end of method JIEJIEHelper::CloneStringCrossThead

	.method private hidebysig static 
		void CloneStringCrossThead_Thread () cil managed 
	{
		// Method begins at RVA 0x21e0
		// Code size 108 (0x6c)
		.maxstack 2

		.try
		{
			// loop start
				IL_0000: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
				IL_0005: ldc.i4 1000
				IL_000a: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
				IL_000f: brtrue.s IL_0013

				IL_0011: leave.s IL_006b

				IL_0013: volatile.
				IL_0015: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
				IL_001a: brfalse.s IL_0034

				IL_001c: volatile.
				IL_001e: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
				IL_0023: callvirt instance char[] [mscorlib]System.String::ToCharArray()
				IL_0028: newobj instance void [mscorlib]System.String::.ctor(char[])
				IL_002d: volatile.
				IL_002f: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue

				IL_0034: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
				IL_0039: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
				IL_003e: pop
				IL_003f: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
				IL_0044: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
				IL_0049: pop
				IL_004a: br.s IL_0000
			// end loop
		} // end .try
		finally
		{
			IL_004c: ldnull
			IL_004d: volatile.
			IL_004f: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_0054: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0059: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_005e: pop
			IL_005f: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
			IL_0064: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0069: pop
			IL_006a: endfinally
		} // end handler

		IL_006b: ret
	} // end of method JIEJIEHelper::CloneStringCrossThead_Thread

	.method public hidebysig static 
		string GetString (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x2268
		// Code size 66 (0x42)
		.maxstack 4
		.locals init (
			[0] int32 chrsLength,
			[1] char[] chrs,
			[2] int32 iCount,
			[3] int32 bi,
			[4] int32 v
		)

		IL_0000: ldarg.2
		IL_0001: ldc.i4.2
		IL_0002: div
		IL_0003: stloc.0
		IL_0004: ldloc.0
		IL_0005: newarr [mscorlib]System.Char
		IL_000a: stloc.1
		IL_000b: ldc.i4.0
		IL_000c: stloc.2
		IL_000d: br.s IL_0037
		// loop start (head: IL_0037)
			IL_000f: ldarg.1
			IL_0010: ldloc.2
			IL_0011: ldc.i4.2
			IL_0012: mul
			IL_0013: add
			IL_0014: stloc.3
			IL_0015: ldarg.0
			IL_0016: ldloc.3
			IL_0017: ldelem.u1
			IL_0018: ldc.i4.8
			IL_0019: shl
			IL_001a: ldarg.0
			IL_001b: ldloc.3
			IL_001c: ldc.i4.1
			IL_001d: add
			IL_001e: ldelem.u1
			IL_001f: add
			IL_0020: stloc.s 4
			IL_0022: ldloc.s 4
			IL_0024: ldarg.3
			IL_0025: xor
			IL_0026: stloc.s 4
			IL_0028: ldloc.1
			IL_0029: ldloc.2
			IL_002a: ldloc.s 4
			IL_002c: conv.u2
			IL_002d: stelem.i2
			IL_002e: ldloc.2
			IL_002f: ldc.i4.1
			IL_0030: add
			IL_0031: stloc.2
			IL_0032: ldarg.3
			IL_0033: ldc.i4.1
			IL_0034: add
			IL_0035: starg.s key

			IL_0037: ldloc.2
			IL_0038: ldloc.0
			IL_0039: blt.s IL_000f
		// end loop

		IL_003b: ldloc.1
		IL_003c: newobj instance void [mscorlib]System.String::.ctor(char[])
		IL_0041: ret
	} // end of method JIEJIEHelper::GetString

	.method public hidebysig static 
		class [System.Drawing]System.Drawing.Bitmap GetBitmap (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x22b8
		// Code size 47 (0x2f)
		.maxstack 5
		.locals init (
			[0] uint8[] bs,
			[1] int32 iCount
		)

		IL_0000: ldarg.2
		IL_0001: newarr [mscorlib]System.Byte
		IL_0006: stloc.0
		IL_0007: ldc.i4.0
		IL_0008: stloc.1
		IL_0009: br.s IL_001f
		// loop start (head: IL_001f)
			IL_000b: ldloc.0
			IL_000c: ldloc.1
			IL_000d: ldarg.0
			IL_000e: ldarg.1
			IL_000f: ldloc.1
			IL_0010: add
			IL_0011: ldelem.u1
			IL_0012: ldarg.3
			IL_0013: xor
			IL_0014: conv.u1
			IL_0015: stelem.i1
			IL_0016: ldloc.1
			IL_0017: ldc.i4.1
			IL_0018: add
			IL_0019: stloc.1
			IL_001a: ldarg.3
			IL_001b: ldc.i4.1
			IL_001c: add
			IL_001d: starg.s key

			IL_001f: ldloc.1
			IL_0020: ldarg.2
			IL_0021: blt.s IL_000b
		// end loop

		IL_0023: ldloc.0
		IL_0024: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0029: newobj instance void [System.Drawing]System.Drawing.Bitmap::.ctor(class [mscorlib]System.IO.Stream)
		IL_002e: ret
	} // end of method JIEJIEHelper::GetBitmap

	.method public hidebysig static 
		class [mscorlib]System.Resources.ResourceSet LoadResourceSet (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x22f4
		// Code size 22 (0x16)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.Resources.ResourceSet result
		)

		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldarg.2
		IL_0003: call class [mscorlib]System.IO.Stream __DC20211119.JIEJIEHelper::GetStream(uint8[], uint8, bool)
		IL_0008: dup
		IL_0009: newobj instance void [mscorlib]System.Resources.ResourceSet::.ctor(class [mscorlib]System.IO.Stream)
		IL_000e: stloc.0
		IL_000f: callvirt instance void [mscorlib]System.IO.Stream::Close()
		IL_0014: ldloc.0
		IL_0015: ret
	} // end of method JIEJIEHelper::LoadResourceSet

	.method private hidebysig static 
		class [mscorlib]System.IO.Stream GetStream (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x2318
		// Code size 122 (0x7a)
		.maxstack 4
		.locals init (
			[0] int32 len,
			[1] class [mscorlib]System.IO.MemoryStream ms,
			[2] int32 iCount,
			[3] class [System]System.IO.Compression.GZipStream 'stream',
			[4] uint8[] bsTemp
		)

		IL_0000: ldarg.0
		IL_0001: ldlen
		IL_0002: conv.i4
		IL_0003: stloc.0
		IL_0004: ldc.i4.0
		IL_0005: stloc.2
		IL_0006: br.s IL_001b
		// loop start (head: IL_001b)
			IL_0008: ldarg.0
			IL_0009: ldloc.2
			IL_000a: ldarg.0
			IL_000b: ldloc.2
			IL_000c: ldelem.u1
			IL_000d: ldarg.1
			IL_000e: xor
			IL_000f: conv.u1
			IL_0010: stelem.i1
			IL_0011: ldloc.2
			IL_0012: ldc.i4.1
			IL_0013: add
			IL_0014: stloc.2
			IL_0015: ldarg.1
			IL_0016: ldc.i4.1
			IL_0017: add
			IL_0018: conv.u1
			IL_0019: starg.s key

			IL_001b: ldloc.2
			IL_001c: ldloc.0
			IL_001d: blt.s IL_0008
		// end loop

		IL_001f: ldnull
		IL_0020: stloc.1
		IL_0021: ldarg.2
		IL_0022: brfalse.s IL_0071

		IL_0024: ldarg.0
		IL_0025: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_002a: ldc.i4.0
		IL_002b: newobj instance void [System]System.IO.Compression.GZipStream::.ctor(class [mscorlib]System.IO.Stream, valuetype [System]System.IO.Compression.CompressionMode)
		IL_0030: stloc.3
		IL_0031: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor()
		IL_0036: stloc.1
		IL_0037: ldc.i4 1024
		IL_003c: newarr [mscorlib]System.Byte
		IL_0041: stloc.s 4
		// loop start (head: IL_0043)
			IL_0043: ldloc.3
			IL_0044: ldloc.s 4
			IL_0046: ldc.i4.0
			IL_0047: ldloc.s 4
			IL_0049: ldlen
			IL_004a: conv.i4
			IL_004b: callvirt instance int32 [mscorlib]System.IO.Stream::Read(uint8[], int32, int32)
			IL_0050: stloc.0
			IL_0051: ldloc.0
			IL_0052: ldc.i4.0
			IL_0053: ble.s IL_0061

			IL_0055: ldloc.1
			IL_0056: ldloc.s 4
			IL_0058: ldc.i4.0
			IL_0059: ldloc.0
			IL_005a: callvirt instance void [mscorlib]System.IO.Stream::Write(uint8[], int32, int32)
			IL_005f: br.s IL_0043
		// end loop

		IL_0061: ldloc.3
		IL_0062: callvirt instance void [mscorlib]System.IO.Stream::Close()
		IL_0067: ldloc.1
		IL_0068: ldc.i4.0
		IL_0069: conv.i8
		IL_006a: callvirt instance void [mscorlib]System.IO.Stream::set_Position(int64)
		IL_006f: br.s IL_0078

		IL_0071: ldarg.0
		IL_0072: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0077: stloc.1

		IL_0078: ldloc.1
		IL_0079: ret
	} // end of method JIEJIEHelper::GetStream

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x239e
		// Code size 39 (0x27)
		.maxstack 8

		IL_0000: ldnull
		IL_0001: volatile.
		IL_0003: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
		IL_0008: ldc.i4.0
		IL_0009: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_000e: stsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
		IL_0013: ldc.i4.0
		IL_0014: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_0019: stsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
		IL_001e: ldnull
		IL_001f: volatile.
		IL_0021: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
		IL_0026: ret
	} // end of method JIEJIEHelper::.cctor

} // end of class __DC20211119.JIEJIEHelper
"; 
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
        //public void ObfuseClassOrder()
        //{
        //    var list = this.Document.ChildNodes;
        //    int len = list.Count;
        //    for (int iCount = 0; iCount < len; iCount++)
        //    {
        //        var item = list[iCount];
        //        if (item is DCILClass)
        //        {
        //            DCUtils.ObfuseListOrder(list, iCount, list.Count - iCount);
        //            break;
        //        }
        //    }//for
        //}

        private static readonly string _SwitchPrefix = "JIEJIE.NET.SWITCH:";

        private Dictionary<object, JieJieSwitchs> _RuntimeSwitchs = new Dictionary<object, JieJieSwitchs>();

        private JieJieSwitchs GetRuntimeSwitchs(DCILClass cls, JieJieSwitchs parent)
        {
            JieJieSwitchs result = null;
            if (_RuntimeSwitchs.TryGetValue(cls, out result))
            {
                return result;
            }
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
            _RuntimeSwitchs[cls] = result;
            return result;
        }

        private JieJieSwitchs GetRuntimeSwitchs(DCILMethod method, JieJieSwitchs parent)
        {
            JieJieSwitchs result = null;
            if (_RuntimeSwitchs.TryGetValue(method, out result))
            {
                return result;
            }
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
                        var strCode = code.FinalValue;
                        if (strCode != null && strCode.StartsWith(_SwitchPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            //code.OperCode = "\"\"";
                            code.ReplaceCode = new DCILOperCode(
                                null,
                                "ldsfld",
                                "string [" + this.Document.LibName_mscorlib + "]System.String::Empty");
                            code.FinalValue = string.Empty;
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
            _RuntimeSwitchs[method] = result;
            return result;
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
        internal void EncryptCharValue()
        {
            ConsoleWriteTask();
            Console.Write("Encrypting char values ...");
            int codeCount = 0;
            int tick = Environment.TickCount;
            var strCodes = new HashSet<string>(new string[] {
                "add","and","beq","beq.s","bgt","bgt.s","ble","ble.s","blt","blt.s","bne","bne.s","ceq","cgt","clt" });
            this.Document.EnumAllOperCodes(delegate (EnumOperCodeArgs args)
            {
                if (args.Current.OperCode == "ldc.i4" || args.Current.OperCode == "ldc.i4.s")
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
                                args.OwnerList[args.CurrentCodeIndex] = this.Int32ValueContainer.GetOperCode(args.Current.LabelID, intv);
                                codeCount++;
                            }
                        }
                        else if (nextCode != null && nextCode.OperCode != null)
                        {
                            vt = args.Method.GetTargetValueTypeForSet(nextCode);
                            if (vt == DCILTypeReference.Type_Char)
                            {
                                args.OwnerList[args.CurrentCodeIndex] = this.Int32ValueContainer.GetOperCode(args.Current.LabelID, intv);
                                codeCount++;
                            }
                        }
                    }
                }
            });
            tick = Math.Abs(Environment.TickCount - tick);
            Console.WriteLine(" change " + codeCount + " ldc.i4 instructions, span " + tick + " milliseconds.");
        }

        internal void EncryptMethodParamterEnumValue()
        {
            ConsoleWriteTask();
            Console.Write("Encrypting enum paramter values ...");
            int codeCount = 0;
            int tick = Environment.TickCount;
            this.Document.EnumAllOperCodes(delegate (EnumOperCodeArgs args)
            {
                if (args.Current is DCILOperCode_HandleMethod && args.CurrentCodeIndex > 0)
                {
                    var preCode = args.OwnerList[args.CurrentCodeIndex - 1];
                    if (preCode.OperCode == "ldc.i4" || preCode.OperCode == "ldc.i4.s")
                    {
                        var code = (DCILOperCode_HandleMethod)args.Current;
                        if (code.InvokeInfo != null && code.InvokeInfo.ParametersCount > 0)
                        {
                            var vt = code.InvokeInfo.Paramters[code.InvokeInfo.ParametersCount - 1].ValueType;
                            if (vt.Mode == DCILTypeMode.ValueType)
                            {
                                var intValue = DCUtils.ConvertToInt32(preCode.OperData);
                                args.OwnerList[args.CurrentCodeIndex - 1] = this.Int32ValueContainer.GetOperCode(preCode.LabelID, intValue);
                                codeCount++;
                            }
                        }
                    }
                }
            });
            tick = Math.Abs(Environment.TickCount - tick);
            Console.WriteLine(" change " + codeCount + " call/callvirt instructions, span " + tick + " milliseconds.");
        }

        internal class OperCodeReference
        {
            public OperCodeReference( DCILOperCode code , DCILOperCodeList list , int index )
            {
                this.Code = code;
                this.OwnerList = list;
                this.Index = index;
            }
            public DCILOperCode Code = null;
            public DCILOperCodeList OwnerList = null;
            public int Index = 0;
        }
        public void EncryptTypeHandle()
        {
            ConsoleWriteTask();
            Console.Write("Encrypting typeof() instructions ...");
            int tick = Environment.TickCount;

            var strNativeCodes = new Dictionary<DCILTypeReference, List<OperCodeReference>>();
            this.Document.EnumAllOperCodes(delegate (EnumOperCodeArgs args)
            {
                var code = args.Current;
                if (code is DCILOperCode_LdToken && args.CurrentCodeIndex < args.OwnerList.Count - 1)
                {
                    var nextCode = args.OwnerList[args.CurrentCodeIndex + 1];
                    if (nextCode.OperCode == "call")
                    {
                        var mInfo = ((DCILOperCode_HandleMethod)nextCode).InvokeInfo;
                        if (mInfo.OwnerType.Name == "System.Type" && mInfo.MethodName == "GetTypeFromHandle")
                        {
                            var ldtoken = (DCILOperCode_LdToken)code;
                            if (ldtoken.ClassType != null)
                            {
                                if(ldtoken.ClassType.IsGenericType 
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
                                list4.Add(new OperCodeReference(code, args.OwnerList, args.CurrentCodeIndex));
                            }
                        }
                    }
                }
            });
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
                var strListTypeName = this.Document.GetTypeNameWithLibraryName("System.Collections.Generic.List`1", null, typeof(System.Collections.Generic.List<>));
                var strAddCode = "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::Add(!0)";
                var addOperCode = new DCILOperCode(null, "callvirt", "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::Add(!0)");
                var operCodes = clsContainer.Method_Cctor.OperCodes;
                var methodGetHandle = (DCILMethod)clsContainer.ChildNodes.GetByName("GetTypeInstance");
                var labelGen = new ILLabelIDGen();
                methodGetHandle.CacheInfo(this.Document, this.Document.GetAllClassesUseCache());
                operCodes.Clear();
                operCodes.Add(new DCILOperCode(labelGen.Gen(), "newobj", "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::.ctor()"));
                var array = new DCILField[types.Count];
                int fieldIndex = -1;
                int totalOperCodes = 0;
                foreach (var type in types)
                {
                    fieldIndex++;
                    operCodes.Add(new DCILOperCode(labelGen.Gen(), "dup", null));
                    operCodes.Add(new DCILOperCode_LdToken(labelGen.Gen(), type));
                    operCodes.Add(new DCILOperCode(labelGen.Gen(), "callvirt", strAddCode));
                    var list = strNativeCodes[type];
                    totalOperCodes += list.Count;
                    foreach (var codeInfo in list)
                    {
                        codeInfo.OwnerList[codeInfo.Index] = this.Int32ValueContainer.GetOperCode(codeInfo.Code.LabelID, fieldIndex);
                        codeInfo.OwnerList[codeInfo.Index + 1] = new DCILOperCode_HandleMethod(labelGen.Gen(), "call", methodGetHandle);
                    }
                }
                operCodes.Add(new DCILOperCode(labelGen.Gen(), "callvirt", "instance !0[] class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::ToArray()"));
                operCodes.Add(new DCILOperCode_HandleField(labelGen.Gen(), "stsfld", new DCILFieldReference((DCILField)clsContainer.ChildNodes.GetByName("_Handles"))));
                operCodes.Add(new DCILOperCode(labelGen.Gen(), "ret", null));
                ObfuscateOperCodeList(clsContainer.Method_Cctor);
                this.Document.ClearCacheForAllClasses();
                Console.WriteLine(" Handle " + types.Count + " types , " + totalOperCodes + " instructions , span " + (Environment.TickCount - tick) + " milliseconds.");
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
        internal class DCRuntimeFieldHandleContainer
        {
            public DCRuntimeFieldHandleContainer( DCJieJieNetEngine eng , DCILDocument document)
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
                this._Class = new DCILClass( strILCode , document);
                var fields = new List<DCILField>();
                foreach( var cls in document.GetAllClassesUseCache().Values)
                {
                    foreach( var item in cls.ChildNodes)
                    {
                        if(item is DCILField )
                        {
                            var field = (DCILField)item;
                            if(field.DataLabel != null && field.DataLabel.Length > 0 )
                            {
                                fields.Add(field);
                            }
                        }
                    }
                }
                DCUtils.ObfuseListOrder(fields);
                for(int iCount = fields.Count -1; iCount >=0; iCount --)
                {
                    _FieldIndexs[fields[iCount]] = iCount;
                }
                document.Classes.Add(_Class);
                this._Class.OwnerDocument = document;
                document.ClearCacheForAllClasses();
                this._Method_GetHandle =(DCILMethod) this._Class.ChildNodes.GetByName("GetHandle");
                this._RFH = new DCILTypeReference(typeof(System.RuntimeFieldHandle), document);
            }

            public void Commit()
            {
                var doc = this._Class.OwnerDocument;
                var strListTypeName = doc.GetTypeNameWithLibraryName("System.Collections.Generic.List`1" , null , typeof( System.Collections.Generic.List<>));
                var strAddCode = "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::Add(!0)";
                var addOperCode = new DCILOperCode(null, "callvirt", "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::Add(!0)");
                var operCodes = this._Class.Method_Cctor.OperCodes;
                operCodes.Clear();
                var labelGen = new ILLabelIDGen();
                operCodes.Add(new DCILOperCode(labelGen.Gen(), "newobj", "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::.ctor()"));
                var array = new DCILField[_FieldIndexs.Count];
                foreach (var item in _FieldIndexs)
                {
                    array[item.Value] = item.Key;
                }
                foreach (var item in array)
                {
                    operCodes.Add(new DCILOperCode(labelGen.Gen(), "dup", null));
                    operCodes.Add(new DCILOperCode_LdToken(labelGen.Gen(), new DCILFieldReference(item)));
                    operCodes.Add(new DCILOperCode(labelGen.Gen(), "callvirt", strAddCode));
                }
                operCodes.Add(new DCILOperCode(labelGen.Gen(), "callvirt", "instance !0[] class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::ToArray()"));
                operCodes.Add(new DCILOperCode_HandleField(labelGen.Gen(), "stsfld", new DCILFieldReference((DCILField)this._Class.ChildNodes.GetByName("_Handles"))));
                operCodes.Add(new DCILOperCode(labelGen.Gen(), "ret", null));
                //this._Class.Method_Cctor.CacheInfo(doc, doc.GetAllClassesUseCache());
            }

            public DCILMethod _Method_GetHandle = null;
            private Dictionary<DCILField, int> _FieldIndexs = new Dictionary<DCILField, int>();

            private DCILTypeReference _RFH = null;
            public DCILClass _Class = null;
            public int GetFieldIndex( DCILField field )
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

            //private Dictionary<string, DCILField> _Fields = new Dictionary<string, DCILField>();

            //public DCILOperCode_HandleField GetCode(string labelID, DCILFieldReference refField)
            //{
            //    DCILField field = null;
            //    var strName = refField.ToString();
            //    if (this._Fields.TryGetValue(strName, out field) == false)
            //    {
            //        if (this._Fields.Count > 10000)
            //        {
            //            // 超出处理范围
            //            return null;
            //        }
            //        field = new DCILField();
            //        field.Parent = this._Class;
            //        field.AddStyles("public", "static", "initonly");
            //        field.ValueType = this._RFH;
            //        field._Name = "_" + this._Class.ChildNodes.Count;
            //        this._Class.ChildNodes.Add(field);
            //        this._Fields[strName] = field;
            //        int labelIndex = this._Fields.Count * 8;
            //        var operCodes = this._Class.Method_Cctor.OperCodes;
            //        var retCode = operCodes[operCodes.Count - 1];
            //        operCodes.RemoveAt(operCodes.Count - 1);
            //        operCodes.Add(new DCILOperCode_LdToken("IL_" + Convert.ToString(labelIndex++), refField));
            //        operCodes.Add(new DCILOperCode_HandleField("IL_" + Convert.ToString(labelIndex++), "stsfld", new DCILFieldReference( field )));
            //        operCodes.Add(retCode);
            //    }
            //    var result = new DCILOperCode_HandleField(labelID, "ldsfld", new DCILFieldReference(field));
            //    return result;
            //}
        }

        private DCInt32ValueContainer _Int32ValueContainer = null;
        public DCInt32ValueContainer Int32ValueContainer
        {
            get
            {
                if( this._Int32ValueContainer == null )
                {
                    this._Int32ValueContainer = new DCInt32ValueContainer(this.Document);
                }
                return this._Int32ValueContainer;
            }
        }
        internal class DCInt32ValueContainer
        {
            public DCInt32ValueContainer(DCILDocument document )
            {
                if( document == null )
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
                document.Classes.Add(_Class);
                document.ClearCacheForAllClasses();
                this.GetField(Environment.TickCount);
            }

            public DCILClass _Class = null;
            private Dictionary<int, DCILField> _Fields = new Dictionary<int, DCILField>();
            //private DCILField _LastField = null;
            //private ILLabelIDGen _LabelGen = new ILLabelIDGen();
            public void Commit()
            {
                var labelGen = new ILLabelIDGen();
                var operCodes = this._Class.Method_Cctor.OperCodes;
                operCodes.Clear();
                var list = new List<DCILField>(this._Fields.Values);
                DCUtils.ObfuseListOrder(list);
                int currentValue = Environment.TickCount;
                operCodes.Add(new DCILOperCode(labelGen.Gen(), "ldc.i8", currentValue.ToString()));
                int listCount = list.Count - 1;
                for (int iCount = 0;iCount <= listCount; iCount ++)
                {
                    var field = list[iCount];
                    int v = field.InnerTag - currentValue;
                    currentValue = field.InnerTag ;
                    operCodes.Add(new DCILOperCode(labelGen.Gen(), "ldc.i8", v.ToString()));
                    operCodes.Add(new DCILOperCode(labelGen.Gen(), "add", null));
                    if (iCount < listCount)
                    {
                        operCodes.Add(new DCILOperCode(labelGen.Gen(), "dup", null));
                    }
                    operCodes.Add(new DCILOperCode(labelGen.Gen(), "conv.i4", null));
                    operCodes.Add(new DCILOperCode_HandleField(labelGen.Gen(), "stsfld", new DCILFieldReference(field)));
                }
                operCodes.Add(new DCILOperCode(labelGen.Gen(), "ret", null));
            }

            public DCILField GetField( int v )
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
                    field._Name = "_" + this._Class.ChildNodes.Count ;
                    if(  v >= 0 )
                    {
                        field._Name = field._Name + "_" + v;
                    }
                    else
                    {
                        field._Name = field._Name + "_N_" + Convert.ToString(-v);
                    }
                    field.InnerTag = v;
                    this._Class.ChildNodes.Add(field);
                    this._Fields[v] = field;
                    //var operCodes = this._Class.Method_Cctor.OperCodes;
                    //var retCode = operCodes[operCodes.Count - 1];
                    //operCodes.RemoveAt(operCodes.Count - 1);
                    //if (this._LastField == null)
                    //{
                    //    operCodes.Add(new DCILOperCode(_LabelGen.Gen(), "ldc.i4", v.ToString()));
                    //}
                    //else
                    //{
                    //    int v2 = v - this._LastField.InnerTag;
                    //    operCodes.Add(new DCILOperCode_HandleField(_LabelGen.Gen(), "ldsfld", new DCILFieldReference( this._LastField)));
                    //    operCodes.Add(new DCILOperCode(_LabelGen.Gen(), "conv.i8", null));
                    //    operCodes.Add(new DCILOperCode(_LabelGen.Gen(), "ldc.i8", v2.ToString()));
                    //    operCodes.Add(new DCILOperCode(_LabelGen.Gen(), "add", null));
                    //    operCodes.Add(new DCILOperCode(_LabelGen.Gen(), "conv.i4", null));
                    //}
                    //operCodes.Add(new DCILOperCode_HandleField(_LabelGen.Gen(), "stsfld",new DCILFieldReference( field)));
                    //operCodes.Add(retCode);
                    //this._LastField = field;
                }
                return field;
            }

            public DCILOperCode GetOperCode(string labelID, int v)
            {
                var field = GetField(v);
                if (field == null)
                {
                    return new DCILOperCode(
                                labelID,
                                "ldc.i4",
                                v.ToString());
                }
                else
                {
                    var result = new DCILOperCode_HandleField(labelID, "ldsfld", new DCILFieldReference(field));
                    return result;
                }
            }
        }

//        private DCILClass _Int32ValueContainerClass = null;
//        private Dictionary<int, DCILField> _Int32ValueFields = null;
//        private DCILField _LastInt32ValueField = null;
//        private DCILOperCode_HandleField GetOperCodeForInt32Value(string labelID, int v)
//        {
//            if (this._Int32ValueContainerClass == null)
//            {
//                this._Int32ValueContainerClass = new DCILClass(@"
//.class private auto ansi abstract sealed beforefieldinit __DC20210205._Int32ValueContainer extends[" + this.Document.LibName_mscorlib + @"]System.Object
//{
//    .method private hidebysig specialname rtspecialname static 
//		void .cctor () cil managed 
//	{
//		.maxstack 5
//        IL_999999: ret
//    }
//}", this.Document);
//                this.Document.Classes.Add(_Int32ValueContainerClass);
//                this.Document.ClearCacheForAllClasses();
//                this._Int32ValueFields = new Dictionary<int, DCILField>();
//            }
//            DCILField field = null;
//            if (this._Int32ValueFields.TryGetValue(v, out field) == false)
//            {
//                if (this._Int32ValueFields.Count > 10000)
//                {
//                    // 超出处理范围
//                    return null;
//                }
//                field = new DCILField();
//                field.Parent = this._Int32ValueContainerClass;
//                field.AddStyles("public", "static", "initonly");
//                field.ValueType = DCILTypeReference.Type_Int32;
//                field._Name = "_" + this._Int32ValueContainerClass.ChildNodes.Count;
//                field.InnerTag = v;
//                this._Int32ValueContainerClass.ChildNodes.Add(field);
//                this._Int32ValueFields[v] = field;
//                int labelIndex = this._Int32ValueFields.Count * 8;
//                var operCodes = this._Int32ValueContainerClass.Method_Cctor.OperCodes;
//                var retCode = operCodes[operCodes.Count - 1];
//                operCodes.RemoveAt(operCodes.Count - 1);
//                if (this._LastInt32ValueField == null)
//                {
//                    operCodes.Add(new DCILOperCode("IL_" + Convert.ToString(labelIndex++), "ldc.i4", v.ToString()));
//                }
//                else
//                {
//                    int v2 = v - this._LastInt32ValueField.InnerTag;
//                    operCodes.Add(new DCILOperCode_HandleField("IL_" + Convert.ToString(labelIndex++), "ldsfld", this._LastInt32ValueField));
//                    operCodes.Add(new DCILOperCode("IL_" + Convert.ToString(labelIndex++), "conv.i8", null));
//                    operCodes.Add(new DCILOperCode("IL_" + Convert.ToString(labelIndex++), "ldc.i8", v2.ToString()));
//                    operCodes.Add(new DCILOperCode("IL_" + Convert.ToString(labelIndex++), "add", null));
//                    operCodes.Add(new DCILOperCode("IL_" + Convert.ToString(labelIndex++), "conv.i4", null));
//                }
//                operCodes.Add(new DCILOperCode_HandleField("IL_" + Convert.ToString(labelIndex++), "stsfld", field));
//                operCodes.Add(retCode);
//                this._LastInt32ValueField = field;
//            }
//            var result = new DCILOperCode_HandleField(labelID, "ldsfld", field);
//            return result;
//        }

        private static readonly Random _Random = new Random();

        //private int _LableIDCounter = 0;
        //private string CreataLableID()
        //{
        //    _LableIDCounter++;
        //    return "IL_Z" + _LableIDCounter.ToString("X4");// Convert.ToString(_LableIDCounter++);
        //}
        ///// <summary>
        ///// 将短指令转换为长指令
        ///// </summary>
        ///// <param name="codes"></param>
        //private void ChangeShortInstruction(DCILOperCodeList codes)
        //{
        //    foreach (var item in codes)
        //    {
        //        var code = item.OperCode;
        //        if (code != null
        //            && code.Length > 3
        //            && code[code.Length - 2] == '.'
        //            && code[code.Length - 1] == 's')
        //        {
        //            if (code == "beq.s"
        //                || code == "bge.s"
        //                || code == "bge.un.s"
        //                || code == "bgt.s"
        //                || code == "bgt.un.s"
        //                || code == "ble.s"
        //                || code == "ble.un.s"
        //                || code == "blt.s"
        //                || code == "blt.un.s"
        //                || code == "bne.un.s"
        //                || code == "br.s"
        //                || code == "brfalse.s"
        //                || code == "brtrue.s"
        //                || code == "leave.s")
        //            {
        //                item.OperCode = code.Substring(0, code.Length - 2);
        //            }
        //        }
        //        else if (item is DCILOperCode_Try_Catch_Finally)
        //        {
        //            var tg = (DCILOperCode_Try_Catch_Finally)item;
        //            if (tg.HasTryOperCodes())
        //            {
        //                ChangeShortInstruction(tg._Try.OperCodes);
        //            }
        //            if (tg._Catchs != null)
        //            {
        //                foreach (var citem in tg._Catchs)
        //                {
        //                    if (citem.OperCodes != null && citem.OperCodes.Count > 0)
        //                    {
        //                        ChangeShortInstruction(citem.OperCodes);
        //                    }
        //                }
        //            }
        //            if (tg.HasFinallyOperCodes())
        //            {
        //                ChangeShortInstruction(tg._Finally.OperCodes);
        //            }
        //            if (tg.HasFaultOperCodes())
        //            {
        //                ChangeShortInstruction(tg._fault.OperCodes);
        //            }
        //        }
        //    }
        //}
        public bool ObfuscateOperCodeList(DCILMethod method)
        {
            if (ObfuscateOperCodeList(method, method.OperCodes, false, null))
            {
                method.Maxstack += 2;
                //method.OperCodes.ChangeShortInstruction();
                return true;
            }
            return false;
        }

        private ILLabelIDGen _LabelGen_MyInitializeArray = new ILLabelIDGen ("IL_MIA");
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
            // 进行特定方法调用信息的替换
            DCILOperCode preCode = null;
            for (int codeIndex = 0; codeIndex < items.Count; codeIndex++)
            {
                var code = items[codeIndex];
                if (code.OperCode == "callvirt")
                {
                    if (preCode != null && preCode.IsPrefixOperCode())
                    {
                        continue;
                    }
                    var callCode = code as DCILOperCode_HandleMethod;
                    if (callCode != null)
                    {
                        if (callCode.MatchTypeAndMethod("System.IDisposable", "Dispose", 0))
                        {
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "MyDispose");
                            callCode.InvokeInfo.IsInstance = false;
                            callCode.OperCode = "call";
                        }
                        else if (callCode.MatchTypeAndMethod("System.Object", "ToString", 0))
                        {
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "Object_ToString");
                            callCode.InvokeInfo.IsInstance = false;
                            callCode.OperCode = "call";
                        }
                        else if (callCode.MatchTypeAndMethod("System.String", "Trim", 0))
                        {
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "String_Trim");
                            callCode.InvokeInfo.IsInstance = false;
                            callCode.OperCode = "call";
                        }
                    }
                }
                else if (code.OperCode == "call")
                {
                    if (preCode != null && preCode.IsPrefixOperCode())
                    {
                        continue;
                    }
                    var callCode = (DCILOperCode_HandleMethod)code;
                    if( callCode.InvokeInfo.MethodName == ".ctor")
                    {
                        continue;
                    }
                    if (callCode.MatchTypeAndMethod("System.String", "op_Equality", 2))
                    {
                        callCode.ChangeTarget(this._Type_JIEJIEHelper, "String_Equality");
                    }
                    else if (callCode.MatchTypeAndMethod("System.String", "Concat", 2))
                    {
                        if (callCode.InvokeInfo.Paramters[0].ValueType.Name == "object")
                        {
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "String_ConcatObject");
                        }
                        else
                        {
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "String_Concat");
                        }
                    }
                    else if (callCode.MatchTypeAndMethod("System.String", "Concat", 3))
                    {
                        if (callCode.InvokeInfo.Paramters[0].ValueType.Name == "object")
                        {
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "String_Concat3Object");
                        }
                        else
                        {
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "String_Concat3String");
                        }
                    }
                    else if( callCode.MatchTypeAndMethod("System.Int32","ToString",0))
                    {
                        callCode.ChangeTarget(this._Type_JIEJIEHelper, "Int32_ToString");
                        callCode.InvokeInfo.IsInstance = false;
                    }
                    else if (callCode.MatchTypeAndMethod("System.Object", "GetType", 0))
                    {
                        callCode.ChangeTarget(this._Type_JIEJIEHelper, "Object_GetType");
                        callCode.InvokeInfo.IsInstance = false;
                    }
                    else if (callCode.MatchTypeAndMethod("System.String", "IsNullOrEmpty", 1))
                    {
                        callCode.ChangeTarget(this._Type_JIEJIEHelper, "String_IsNullOrEmpty");
                    }
                    //else if (callCode.MatchTypeAndMethod("System.Type", "GetTypeFromHandle", 1))
                    //{
                    //    callCode.ChangeTarget(_Type_InnerAssemblyHelper20211018, "Type_GetTypeFromHandle");
                    //}
                    else if (callCode.MatchTypeAndMethod("System.Runtime.CompilerServices.RuntimeHelpers", "InitializeArray", 2))
                    {
                        method.MaxstackFix = 1;
                        var ldTokenCode = items[codeIndex - 1] as DCILOperCode_LdToken;
                        if (ldTokenCode != null && ldTokenCode.FieldReference.LocalField != null)
                        {
                            var fieldIndex = this.RFHContainer.GetFieldIndex(ldTokenCode.FieldReference.LocalField);
                            if (fieldIndex >= 0)
                            {
                                //var code10 = this.Int32ValueContainer.GetOperCode(ldTokenCode.LabelID, fieldIndex);
                                items[codeIndex - 1] = this.Int32ValueContainer.GetOperCode(ldTokenCode.LabelID, fieldIndex); ;
                                items.Insert(
                                    codeIndex,
                                    new DCILOperCode_HandleMethod(
                                        _LabelGen_MyInitializeArray.Gen(),
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
                                if (code10.OperCode == "newarr")
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
                                else if ((code10.OperCode == "ldc.i4" || code10.OperCode == "ldc.i4.s"))
                                {
                                    var intValue = DCUtils.ConvertToInt32(code10.OperData);
                                    items[index2] = this.Int32ValueContainer.GetOperCode(code10.LabelID, intValue);
                                    break;
                                }
                            }
                        }//for
                        items.Insert(codeIndex, this.Int32ValueContainer.GetOperCode(_LabelGen_MyInitializeArray.Gen(), encKey));
                        callCode.ChangeTarget(this._Type_JIEJIEHelper, "MyInitializeArray");
                        codeIndex++;
                    }
                    else if (callCode.MatchTypeAndMethod("System.Threading.Monitor", "Enter", 1))
                    {
                        callCode.ChangeTarget(this._Type_JIEJIEHelper, "Monitor_Enter");
                    }
                    else if (callCode.MatchTypeAndMethod("System.Threading.Monitor", "Exit", 1))
                    {
                        callCode.ChangeTarget(this._Type_JIEJIEHelper, "Monitor_Exit");
                    }
                }
                preCode = code;
            }//foreach
            if (method.Name == ".cctor")
            {
                // 遇到静态构造函数，则加密整数数值。
                for (int iCount = items.Count - 1; iCount >= 0; iCount--)
                {
                    var code = items[iCount];
                    if (code.OperCode == "ldc.i4")
                    {
                        var intValue = DCUtils.ConvertToInt32(code.OperData);
                        items[iCount] = this.Int32ValueContainer.GetOperCode(code.LabelID, intValue);
                    }
                }
            }
        }

        private List<DCILOperCode_HandleMethod> _CallOperCodes = new List<DCILOperCode_HandleMethod>();

        private readonly ILLabelIDGen _labelGenGlobal = new ILLabelIDGen("IL_G");

        public bool ObfuscateOperCodeList(DCILMethod method, DCILOperCodeList items, bool isInTryBlock, DCILOperCodeList parentList)
        {
            return ObfuscateOperCodeListNew(method, items, isInTryBlock, parentList , 1);

            if (items == null || items.Count == 0)
            {
                return false;
            }
            bool result = false;
            if (method.Parent != this._Int32ValueContainer?._Class)
            {
                foreach (var item in items)
                {
                    if (item is DCILOperCode_Try_Catch_Finally)
                    {
                        var tcf = (DCILOperCode_Try_Catch_Finally)item;
                        if (tcf._Try != null && ObfuscateOperCodeList(method, tcf._Try.OperCodes, true, items))
                        {
                            result = true;
                        }
                        if (method.OwnerClass?.Name != _ClassName_JIEJIEHelper)
                        {
                            ChangeSpecifyCallTarget(tcf._Finally?.OperCodes, method);
                            items.ChangeShortInstruction();
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
                if (item.OperCode == "call" || item.OperCode == "callvirt")
                {
                    var callCode = item as DCILOperCode_HandleMethod;
                    if ( callCode != null && callCode.InvokeInfo != null)
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
            DCILOperCode retCode = null;
            if (items[items.Count - 1].OperCode == "ret")
            {
                retCode = items[items.Count - 1];
                items[items.Count - 1] = new DCILOperCode( this._labelGenGlobal.Gen(), "br", retCode.LabelID);
                //items.RemoveAt(items.Count - 1);
            }

            // 进行指令分组
            var groups = new List<List<DCILOperCode>>();
            var group = new List<DCILOperCode>();
            var firstGroup = group;
            groups.Add(group);
            List<DCILOperCode> preGroup = null;
            foreach (var item in items)
            {
                if (group.Count == 0 && item.HasLabelID() == false)
                {
                    group.Add(new DCILOperCode(this._labelGenGlobal.Gen(), "nop", null));
                }
                group.Add(item);
                //if(item.OperCode == "call" || item.OperCode == "callvirt")
                //{
                //    group.Add(this.Int32ValueContainer.GetOperCode(CreataLableID(), _Random.Next(100, 110)));
                //    group.Add(new DCILOperCode(CreataLableID(), "ldc.i4.0", null));
                //    group.Add( new DCILOperCode(CreataLableID(), "ble", group[0].LabelID));
                //}
                if (preGroup != null)
                {
                    preGroup.Add(new DCILOperCode(this._labelGenGlobal.Gen(), "br", group[0].LabelID));
                    preGroup.Add(new DCILOperCodeComment("goto next group"));
                    preGroup = null;
                    group.Insert(0, new DCILOperCode(this._labelGenGlobal.Gen(), "conv.r8", null));
                    //group.Insert(0, new DCILOperCode(CreataLableID(), "ble", group[0].LabelID));
                    //group.Insert(0, new DCILOperCode(CreataLableID(), "ldc.i4.0", null));
                    //group.Insert(0, this.Int32ValueContainer.GetOperCode(CreataLableID(), _Random.Next( 100,110)));

                    //if (_Random.Next(0, 100) < 30 && items.Count > 40)
                    //{
                    //    // 小概率插入花指令
                    //    for (int iCount2 = _Random.Next(10, items.Count - 10); iCount2 < items.Count; iCount2++)
                    //    {
                    //        if (items[iCount2].HasLabelID() && items[iCount2].IsPrefixOperCode() == false)
                    //        {
                    //            group.Insert(0, new DCILOperCode(CreataLableID(), "br",items[iCount2].LabelID));
                    //            break;
                    //        }
                    //    }
                    //}
                }
                if (item.IsPrefixOperCode() == false)// item.OperCode != "volatile." && item.OperCode != "constrained.")
                {
                    if (group.Count > groupMaxLen)
                    {
                        preGroup = group;
                        group = new List<DCILOperCode>();
                        groups.Add(group);
                        groupMaxLen = _Random.Next(20, 30);
                    }
                }
            }

            var lastGroup = groups[groups.Count - 1];
            if (lastGroup.Count == 0)
            {
                // 删除最后一个内容为空的指令组
                groups.RemoveAt(groups.Count - 1);
                lastGroup = groups[groups.Count - 1];
            }


            //for (int iCount = 0; iCount < groups.Count - 1; iCount++)
            //{
            //    // 每条指令组后面添加跳到下一个指令组的指令
            //    group = groups[iCount];
            //    var nextGroup = groups[iCount + 1];
            //    var nextGroupLableID = nextGroup[0].LabelID;
            //    //if (nextGroupLableID == null || nextGroupLableID.Length == 0)
            //    //{
            //    //    nextGroup.Insert(0, new DCILOperCode(CreataLableID(), "nop", null));
            //    //    nextGroupLableID = nextGroup[0].LabelID;
            //    //}
            //    //foreach (var item3 in group)
            //    //{
            //    //    if (item3.LabelID != null && item3.LabelID.Length > 0)
            //    //    {
            //    //        nextGroup.Insert(0, new DCILOperCode(CreataLableID(), "ble", item3.LabelID));
            //    //        break;
            //    //    }
            //    //}
            //    //if (method != null && method.Locals != null && method.Locals.Count > 0)
            //    //{
            //    //    nextGroup.Insert(0, new DCILOperCode(CreataLableID(), "ldloc.0", null));
            //    //}
            //    //else
            //    //{
            //    //    nextGroup.Insert(0, new DCILOperCode(CreataLableID(), "ldc.i4", _Random.Next().ToString()));
            //    //}
            //    //if (parentList != null && isInTryBlock)
            //    //{
            //    //    foreach (var item2 in parentList)
            //    //    {
            //    //        if (_Random.Next(0, 100) > 90
            //    //            && item2.LabelID != null
            //    //            && item2.LabelID.Length > 0)
            //    //        {
            //    //            nextGroup.Insert(0, new DCILOperCode(CreataLableID(), "leave", item2.LabelID));
            //    //            break;
            //    //        }
            //    //    }
            //    //}

            //        group.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));
            //    group.Add(new DCILOperCodeComment("jump next group"));

            //    //group.Add(new DCILOperCode(CreataLableID(), "br", group[0].LabelID));
            //    //group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", _Random.Next(0, 10000).ToString()));
            //    //group.Add(new DCILOperCode(CreataLableID(), "pop",null));

            //    ////// 输出无效的垃圾代码
            //    //group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", "8888"));
            //    //group.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));

            //    ////group.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));

            //    //group.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
            //    //if (_Random.Next(0, 1) == 0)
            //    //{
            //    //    group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
            //    //    group.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
            //    //    group.Add(new DCILOperCode(CreataLableID(), "brfalse", nextGroupLableID));
            //    //}
            //    //else
            //    //{
            //    //    group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetUnZeroIndex().ToString()));
            //    //    group.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
            //    //    group.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
            //    //}
            //}

            //if (retCode != null)
            //{
            //    groups[groups.Count - 1].Add(new DCILOperCode(CreataLableID(), "br", retCode.LabelID));
            //}
            //var lastCode = new DCILOperCode(CreataLableID(), "nop", null);
            //var lastGroup = groups[groups.Count - 1];
            //lastGroup.Add(new DCILOperCode(CreataLableID(), "br", lastCode.LabelID));
            //var startGroupNone = new List<DCILOperCode>();
            //int maxNoneCodeNum = Math.Max(30, items.Count / 15);
            //for (int iCount = 0; iCount < maxNoneCodeNum; iCount++)
            //{
            //    // 随机挑一些指令放在开头混淆视听
            //    var code = items[_Random.Next(0, items.Count - 1)];
            //    if ( code.IsPrefixOperCode() == false
            //        && (code is DCILOperCode_Try_Catch_Finally) == false)
            //    {
            //        startGroupNone.Add(code.Clone( CreataLableID()));
            //    }
            //}


            DCILOperCode leaveCode = null;
            if (isInTryBlock)
            {
                for (int iCount = items.Count - 1; iCount >= 0; iCount--)
                {
                    if (items[iCount].OperCode == "leave"
                        || items[iCount].OperCode == "leave.s")
                    {
                        leaveCode = items[iCount];
                        break;
                    }
                }
            }
            items.Clear();
            items.Add(new DCILOperCode(this._labelGenGlobal.Gen(), "nop", null));
            //////if (groups.Count > 1)
            //////{
            //////    //items.Add(new DCILOperCode(
            //////    //    CreataLableID(),
            //////    //    "ldsfld",
            //////    //    "object " + _ClassName_InnerAssemblyHelper20211018 + "::_NullObject"));
            //////    items.Add(new DCILOperCode(
            //////        CreataLableID(),
            //////        "ldc.i4",
            //////        _Random.Next(1, 10).ToString()));
            //////    items.Add(new DCILOperCode(CreataLableID(), "brtrue", groups[0][0].LabelID));
            //////    items.Add(new DCILOperCode(CreataLableID(), "br", groups[_Random.Next(1, groups.Count - 1)][0].LabelID));
            //////}
            //////else
            {
                items.Add(new DCILOperCode(this._labelGenGlobal.Gen(), "br", groups[0][0].LabelID));
            }
            DCUtils.ObfuseListOrder(groups);
            foreach (var group3 in groups)
            {
                items.AddRange(group3);
            }
            //if (startGroupNone.Count > 0)
            //{
            //    var labelIDStart = CreataLableID();
            //    items.Add(new DCILOperCode(CreataLableID(), "br", labelIDStart));
            //    items.AddRange(startGroupNone);
            //    items.Add(new DCILOperCode(labelIDStart, "nop", null));
            //}
            //if (firstGroup != groups[0])
            //{
            //    var nextGroupLableID = firstGroup[0].LabelID;
            //    if (nextGroupLableID == null || nextGroupLableID.Length == 0)
            //    {

            //    }

            //    if (method.IndexOfExtLocals == int.MinValue)
            //    {
            //        if (method.Locals == null)
            //        {
            //            method.Locals = new DCILMethodLocalVariableList();
            //        }
            //        method.IndexOfExtLocals = method.Locals.Count;
            //        var loc = new DCILMethodLocalVariable();
            //        loc.ValueType = DCILTypeReference.GetPrimitiveType("int32");
            //        loc.Name = "jijietemp_" + Environment.TickCount.ToString();
            //        method.Locals.Add(loc);
            //    }

            //    items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", _Random.Next(1, 1000).ToString()));
            //    items.Add(new DCILOperCode(CreataLableID(), "stloc", method.IndexOfExtLocals.ToString()));
            //    items.Add(new DCILOperCode(CreataLableID(), "ldloc", method.IndexOfExtLocals.ToString()));
            //    items.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
            //    //items.Add(new DCILOperCode(CreataLableID(), "ldloc", method.IndexOfExtLocals.ToString()));
            //    //items.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
            //    //items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", _Random.Next(0,100).ToString()));
            //    //items.Add(new DCILOperCode(CreataLableID(), "pop", null));
            //    //items.Add(new DCILOperCode(CreataLableID(), "pop", null));
            //    items.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));
            //    //items.Add(new DCILOperCode(CreataLableID(), "ldc.i4.0", null));
            //    //items.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
            //    //var id3 = nextGroupLableID;
            //    //if( groups.Count > 1 )
            //    //{
            //    //    id3 = groups[1][0].LabelID;
            //    //}
            //    //if(id3 == null || id3.Length == 0 )
            //    //{
            //    //    id3 = nextGroupLableID;
            //    //}
            //    ////foreach (var item3 in items)
            //    ////{
            //    ////    if (item3.LabelID != null && item3.LabelID.Length > 0 && _Random.Next(0, 100) > 90)
            //    ////    {
            //    ////        id3 = item3.LabelID;
            //    ////        break;
            //    ////    }
            //    ////}
            //    ////items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", _Random.Next(1, 1000).ToString()));
            //    //items.Add(new DCILOperCode(CreataLableID(), "br", id3));

            //    //items.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));

            //    //items.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
            //    //if (_Random.Next(0, 1) == 0)
            //    //{
            //    //    items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
            //    //    items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
            //    //    items.Add(new DCILOperCode(CreataLableID(), "brfalse", nextGroupLableID));
            //    //}
            //    //else
            //    //{
            //    //    items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetUnZeroIndex().ToString()));
            //    //    items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
            //    //    items.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
            //    //}
            //}
            ////var lastGroup = groups[groups.Count - 1];
            //foreach (var group2 in groups)
            //{
            //    foreach (var item in group2)
            //    {
            //        items.Add(item);
            //        //if ( items.Count > 10 && _Random.Next(0, 100) < 2)
            //        //{
            //        //    // 小概率插入无效跳转指令
            //        //    string rndLableID = items[_Random.Next(0, items.Count - 1)].LabelID;
            //        //    if (rndLableID != null && rndLableID.Length > 0)
            //        //    {
            //        //        //if( rndLableID == "IL_0001")
            //        //        //{

            //        //        //}
            //        //        items.Add(new DCILOperCodeComment("no used"));
            //        //        items.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
            //        //        items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
            //        //        items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
            //        //        items.Add(new DCILOperCode(CreataLableID(), "brtrue", rndLableID));
            //        //    }
            //        //}
            //    }
            //    if (group2 == lastGroup)
            //    {
            //        break;
            //    }
            //    //if (group2 == lastGroup)
            //    //{
            //    //    if(isInTryBlock )
            //    //    {
            //    //        items.Add(leaveCode.Clone(CreataLableID()));
            //    //    }
            //    //    break;
            //    //}
            //    //if (groups.Count > 4 && _Random.Next(0, 100) < 70)
            //    //{
            //    //    // 指令组和指令组之间小概率插入随机指令(类似花指令)
            //    //    items.Add(new DCILOperCode(CreataLableID(), "nop", null)); // "\"111111111222222222\""));
            //    //    int num = _Random.Next(5, 10);
            //    //    for (int iCount = 0; iCount < num; iCount++)
            //    //    {
            //    //        var g3 = groups[_Random.Next(0, groups.Count - 1)];
            //    //        var code3 = g3[_Random.Next(0, g3.Count - 1)];
            //    //        if (code3.IsPrefixOperCode() == false && ((code3 is DCILOperCode_Try_Catch_Finally) == false))
            //    //        {
            //    //            items.Add(code3.Clone(CreataLableID()));
            //    //        }
            //    //        //items.Add(followCodes[_Random.Next(0, followCodes.Count - 1)].Clone(CreataLableID()));
            //    //    }
            //    //    items.Add(new DCILOperCode(CreataLableID(), "nop", null));// "\"4444444444444455555555555555555\""));
            //    //}
            //    //if (isInTryBlock && group2 == lastGroup)
            //    //{
            //    //    items.Add(leaveCode.Clone(CreataLableID()));
            //    //}
            //}
            // 进行某些特征性代码移动位置的跳转，用于增加using/lock/foreach的恢复难度。
            var tempList = new DCILOperCodeList();
            var itemsCount_1 = items.Count - 1;
            for (int iCount = 1; iCount < itemsCount_1; iCount++)
            {
                bool changePos = false;
                var code = items[iCount];
                if (code.OperCode == "throw")
                {
                    changePos = true;
                }
                else if (code is DCILOperCode_HandleMethod)
                {
                    var info = ((DCILOperCode_HandleMethod)code).InvokeInfo;
                    var methodName = info.MethodName;
                    if (methodName == "ToString")
                    {
                        if (info.ParametersCount == 0 && info.OwnerType?.Name == "System.Object")
                        {
                            changePos = true;
                        }
                    }
                    //else if( methodName == "Concat")
                    //{
                    //    if( info.ParametersCount > 0 && info.OwnerType?.Name == "System.String")
                    //    {
                    //        changePos = true;
                    //    }
                    //}
                    else if (methodName == "GetEnumerator"
                        || methodName == "Dispose"
                        || methodName == "MoveNext"
                        || methodName == "get_Current"
                        //|| methodName == "get_Count"
                        )
                    {
                        if (info.Paramters == null || info.Paramters.Count == 0)
                        {
                            changePos = true;
                        }
                    }
                }
                if (changePos)
                {
                    var preCode = items[iCount - 1];
                    if (preCode.IsPrefixOperCode())
                    {
                        continue;
                    }
                    var nextCode = items[iCount + 1];
                    if (nextCode.HasLabelID() == false)
                    {
                        continue;
                    }
                    var newID = this._labelGenGlobal.Gen();
                    items[iCount] = new DCILOperCode(code.LabelID, "br", newID);// 替换成跳转代码
                    code.LabelID = newID;
                    tempList.Add(code);
                    tempList.Add(new DCILOperCode(this._labelGenGlobal.Gen(), "br", nextCode.LabelID));
                }
            }//for

            if (tempList.Count > 0)
            {
                if (groups.Count > 2)
                {
                    items.Add(new DCILOperCode(this._labelGenGlobal.Gen(), "br", groups[_Random.Next(1, groups.Count - 1)][0].LabelID));
                }
                items.AddRange(tempList);
            }
            if (leaveCode != null)
            {
                items.Add(leaveCode.Clone(this._labelGenGlobal.Gen()));
            }
            if (retCode != null)
            {
                items.Add(new DCILOperCode(retCode.LabelID, "nop", null));
                items.Add(new DCILOperCode(this._labelGenGlobal.Gen(), "ret", null));
                //items.Add(retCode);
                //items.Add(new DCILOperCode(CreataLableID(), "ret", null));

                //var newRetlabelID = CreataLableID();
                //items.Add(new DCILOperCode(retCode.LabelID, "br.s", newRetlabelID));
                //items.Add(new DCILOperCode(CreataLableID(), "nop", null));
                //retCode.LabelID = newRetlabelID;
                //items.Add(retCode);
            }
            int addCount = items.Count - oldItemsCount;
            items.ChangeShortInstruction();
            return true;
        }

        private static List<string> codes_br_2 = new List<string>(new string[] { "ble", "bgt", "blt", "bne" });
        private static List<string> codes_brs_2 = new List<string>(new string[] { "ble.s", "bgt.s", "blt.s", "bne.s" });
        private static List<string> codes_max_2 = new List<string>(new string[] { "add", "sub", "div", "mul", "xor", "and" });


        public bool ObfuscateOperCodeListNew(DCILMethod method, DCILOperCodeList items, bool isInTryBlock, DCILOperCodeList parentList , int level )
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

            if (isMethodOfInt32ValueContainer == false )
            {
                foreach (var item in items)
                {
                    if (item is DCILOperCode_Try_Catch_Finally)
                    {
                        var tcf = (DCILOperCode_Try_Catch_Finally)item;
                        if (tcf._Try != null && ObfuscateOperCodeListNew(method, tcf._Try.OperCodes, true, items , level + 1 ))
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
                if (item.OperCode == "call" || item.OperCode == "callvirt")
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
                    newGroup.Add(new DCILOperCode(this._labelGenGlobal.Gen(), "nop", null));
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


            //DCILOperCode leaveCode = null;
            //if (isInTryBlock)
            //{
            //    for (int iCount = items.Count - 1; iCount >= 0; iCount--)
            //    {
            //        if (items[iCount].OperCode == "leave"
            //            || items[iCount].OperCode == "leave.s")
            //        {
            //            leaveCode = items[iCount];
            //            break;
            //        }
            //    }
            //}

            for (int iCount = 0; iCount < groups.Count - 1; iCount++)
            {
                groups[iCount].NextGroup = groups[iCount + 1];
            }
            var firstGroup = groups[0];
            string strSwitchLabelID = this._labelGenGlobal.Gen();
            var strSwitch = new StringBuilder();
            foreach (var g in groups)
            {
                if (strSwitch.Length > 0)
                {
                    strSwitch.Append(',');
                }
                strSwitch.Append(g[0].LabelID);
            }
            if(method.MaxstackFix < level * 3)
            {
                method.MaxstackFix = level * 3;
            }
            var extLocalIndex = int.MinValue;
            var extLocalValue = int.MinValue;
            DCILOperCodeList firstGroupShadow = null;
            if (isMethodOfInt32ValueContainer == false )
            {
                firstGroupShadow = new DCILOperCodeList();
                foreach (var item in firstGroup)
                {
                    if ((item is DCILOperCode_Try_Catch_Finally) == false)
                    {
                        var item2 = item.Clone(this._labelGenGlobal.Gen());
                        if (item2.OperCode == "ldc.i4" || item2.OperCode == "ldc.i4.s")
                        {
                            var v2 = DCUtils.ConvertToInt32(item2.OperData);
                            item2.OperCode = "ldc.i4";
                            item2.OperData = Convert.ToString(v2 + _Random.Next(-100, 100));
                        }
                        if (codes_br_2.Contains(item2.OperCode))
                        {
                            item2.OperCode = codes_br_2[_Random.Next(0, codes_br_2.Count - 1)];
                        }
                        else if (codes_brs_2.Contains(item2.OperCode))
                        {
                            item2.OperCode = codes_brs_2[_Random.Next(0, codes_brs_2.Count - 1)];
                        }
                        else if (codes_max_2.Contains(item2.OperCode))
                        {
                            item2.OperCode = codes_max_2[_Random.Next(0, codes_max_2.Count - 1)];
                        }
                        //else if(item2.OperCode == "ldc.i4.0")
                        //{
                        //    item2.OperCode = "ldc.i4.s";
                        //    item2.OperData = _Random.Next(-200, 200).ToString();
                        //}
                        else if (item2.OperCode == "brtrue" || item2.OperCode == "brtrue.s")
                        {
                            item2.OperCode = "brfalse";
                        }
                        else if (item2.OperCode == "brfalse" || item2.OperCode == "brfalse.s")
                        {
                            item2.OperCode = "brtrue";
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
                firstGroup.Insert(0, this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), extLocalValue));
                firstGroup.Insert(1, new DCILOperCode(this._labelGenGlobal.Gen(), "stloc", extLocalIndex.ToString()));

                firstGroupShadow.Insert(0, this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), extLocalValue == 0 ? 1 : 0));
                firstGroupShadow.Insert(1, new DCILOperCode(this._labelGenGlobal.Gen(), "stloc", extLocalIndex.ToString()));

                firstGroupShadow.NextGroup = firstGroup.NextGroup;

                //if( firstGroup.NextGroup.NextGroup != null )
                //{
                //    firstGroupShadow.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.NextGroup.NextGroup.FirstLabelID);
                //}
                //firstGroupShadow.AddItem(this._labelGenGlobal.Gen(), "br", groups[_Random.Next(0, groups.Count - 1)].FirstLabelID);
                //    var flag = _Random.Next(20, 100);
                //    firstGroup.Add(this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), flag));
                //if (_Random.Next(0, 100) > 50)
                //{
                //    firstGroup.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", Convert.ToString(flag - 10));
                //    firstGroup.AddItem(this._labelGenGlobal.Gen(), "ble", firstGroupShadow.FirstLabelID);
                //}
                //else
                //{
                //    firstGroup.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", Convert.ToString(flag + 10));
                //    firstGroup.AddItem(this._labelGenGlobal.Gen(), "bge", firstGroupShadow.FirstLabelID);
                //}
                groups.Add(firstGroupShadow);
            }
            else
            {
            }

            //firstGroup.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.NextGroup.FirstLabelID);

            //if( method.ReturnTypeInfo != DCILTypeReference.Type_Void )
            //{
            //    firstGroupShadow.AddItem(this._labelGenGlobal.Gen(), "ldnull");
            //}
            //firstGroupShadow.AddItem(this._labelGenGlobal.Gen(), "ret");
            //firstGroupShadow.NextGroup = groups[1];
            var resultList = new DCILOperCodeList();
            DCUtils.ObfuseListOrder(groups);
            string idForLastCode = lastCode.LabelID;
            if (idForLastCode == null || idForLastCode.Length == 0)
            {
                idForLastCode = this._labelGenGlobal.Gen();
            }

            //resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup[0].LabelID);

            //firstGroup.Insert(0, new DCILOperCode(this._labelGenGlobal.Gen(), "ldc.i4", "1"));
            //firstGroup.Insert(1, new DCILOperCode(this._labelGenGlobal.Gen(), "switch", "(" + groups[1].FirstLabelID  + ")"));

            //resultList.AddItem(this._labelGenGlobal.Gen(), "nop");
            //resultList.Add(this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), groups.Count + 100 ));
            //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "0");
            //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + firstGroup.FirstLabelID + "," + groups[1].FirstLabelID + ")");
            // resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.FirstLabelID);
            //resultList.AddItem(this._labelGenGlobal.Gen(), "br", idForLastCode);


            //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "0");
            // resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + strSwitch + ")");


            //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + strSwitch + ")");
            //resultList.Add(lastCode.Clone(this._labelGenGlobal.Gen()));
            //resultList.AddItem(this._labelGenGlobal.Gen(), "br", idForLastCode);

            //var rnd2 = new System.Random();
            if (firstGroupShadow != null)
            {
                var flag = _Random.Next(30, 100);
                var retLabelID = this._labelGenGlobal.Gen();
                resultList.Add(this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), flag));
                if (_Random.Next(0, 100) > 50)
                {
                    // 直接命中
                    resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", Convert.ToString(flag - 10));
                    resultList.AddItem(this._labelGenGlobal.Gen(), "bgt", firstGroup.FirstLabelID);
                    resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroupShadow.FirstLabelID);
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "1");
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + firstGroup.FirstLabelID + "," + firstGroupShadow.FirstLabelID + ")");
                }
                else
                {
                    // 间接命中
                    resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", Convert.ToString(flag + 10));
                    resultList.AddItem(this._labelGenGlobal.Gen(), "bgt", firstGroupShadow.FirstLabelID);
                    resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.FirstLabelID);
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "1");
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + firstGroupShadow.FirstLabelID + "," + firstGroup.FirstLabelID + ")");
                }
                
                //resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.FirstLabelID);
                //if (method.Locals != null && method.Locals.Count > 0 )
                //{
                //    resultList.AddItem(this._labelGenGlobal.Gen(), "ldarga.s","0");
                //}
                //else
                //{
                //    resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "1");
                //}
                //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + firstGroup.FirstLabelID + ")");
                //resultList.AddItem(retLabelID, "nop");
                //if ( lastCode.OperCode == "ret")
                //{
                //    if( method.ReturnTypeInfo != DCILTypeReference.Type_Void)
                //    {
                //        resultList.AddItem(this._labelGenGlobal.Gen(), "ldnull");
                //    }
                //}
                ////resultList.AddItem(this._labelGenGlobal.Gen(), "br", lastCode.LabelID);
                //resultList.Add( lastCode.Clone( this._labelGenGlobal.Gen()));
            }
            else
            {
                resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.FirstLabelID);

            }
            ////if (firstGroup == groups[0])
            ////{
            ////    resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", Convert.ToString(flag - 10));// this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), flag - 10));
            ////    resultList.AddItem(this._labelGenGlobal.Gen(), "ble", groups[0].FirstLabelID);
            ////}
            ////else
            //{
            //    resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", Convert.ToString(flag + 10));// this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), flag - 10));
            //    resultList.AddItem(this._labelGenGlobal.Gen(), "ble", firstGroup.FirstLabelID);
            //}


            ////resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.FirstLabelID);
            //if (method.ReturnTypeInfo != DCILTypeReference.Type_Void)
            //{
            //    resultList.AddItem(this._labelGenGlobal.Gen(), "ldnull");
            //}
            //resultList.Add(lastCode.Clone(this._labelGenGlobal.Gen()));
            //if (firstGroup != groups[0])
            //{
            //    resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.FirstLabelID);
            //}
            //else
            //{
            //    resultList.AddItem(this._labelGenGlobal.Gen(), "br", firstGroup.FirstLabelID);
            //}

            //resultList.AddItem(this._labelGenGlobal.Gen(), "br", idForLastCode );
            int codeCount = 0;
            string lastLabelID = null;
            foreach (var group2 in groups)
            {
                foreach( var item2 in group2 )
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
                            || item2.OperCode == "brtrue"
                            || item2.OperCode == "brtrue.s"
                            || item2.OperCode == "brfalse"
                            || item2.OperCode == "brfalse.s")
                        {
                            addCode = true;
                        }
                        if( addCode )
                        { 
                            //if( lastLabelID == null )
                            {
                                lastLabelID = item2.OperData;
                            }
                            resultList.AddItem(this._labelGenGlobal.Gen(), "ldloc", extLocalIndex.ToString());
                            if (extLocalValue == 0)
                            {
                                resultList.AddItem(this._labelGenGlobal.Gen(), "brtrue", lastLabelID);// firstGroupShadow.FirstLabelID);
                            }
                            else
                            {
                                resultList.AddItem(this._labelGenGlobal.Gen(), "brfalse", lastLabelID);// firstGroupShadow.FirstLabelID);
                            }
                            codeCount = 0;
                            lastLabelID = item2.OperData;
                        }
                    }
              
                }
                //resultList.AddRange(group2);
                //if( groups.IndexOf( group2 ) == 1 )
                //{
                //    resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "1");
                //    resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + strSwitch + ")");
                //}
                //if(group2.LastCode is DCILOperCode_HandleMethod)
                //{
                //    var flag = rnd2.Next(0, 100);
                //    resultList.Add(this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), flag));
                //    resultList.Add(this.Int32ValueContainer.GetOperCode(this._labelGenGlobal.Gen(), flag + 10));
                //    resultList.AddItem(this._labelGenGlobal.Gen(), "ble", idForLastCode);
                //}
                if ( group2 == firstGroupShadow )
                {
                    //continue;
                }
                if (group2.NextGroup == null )
                {
                    resultList.AddItem(this._labelGenGlobal.Gen(), "br", idForLastCode);
                }
                else
                {
                    resultList.AddItem(this._labelGenGlobal.Gen(), "br", group2.NextGroup[0].LabelID);
                }
            }

            //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", groups.IndexOf( firstGroup ).ToString());
            //resultList.AddItem(strSwitchLabelID, "switch", "(" + strSwitch.ToString() + ")");
            //foreach (var group2 in groups)
            //{
            //    resultList.AddRange(group2);
            //    if (group2.NextGroup == null)
            //    {
            //        if (group2 != groups[groups.Count - 1])
            //        {
            //            resultList.AddItem(this._labelGenGlobal.Gen(), "br", idForLastCode);
            //        }
            //    }
            //    else
            //    {
            //        resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", groups.IndexOf(group2.NextGroup).ToString());
            //        resultList.AddItem(this._labelGenGlobal.Gen(), "br", strSwitchLabelID);
            //    }
            //}
            if(lastCode.HasLabelID() == false )
            {
                resultList.AddItem(idForLastCode, "nop");
            }
            resultList.Add(lastCode);
            items.Clear();
            items.AddRange(resultList);
            items.ChangeShortInstruction();
            int addCount = items.Count - oldItemsCount;
            return true;
        }

    }

    internal class DCILInvokeMethodInfo : IEqualsValue<DCILInvokeMethodInfo>
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
            this.LineIndex = reader.CurrentLineIndex();
            //if (this.LineIndex == 917962)
            //{

            //}
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
                for(int iCount = objs.Count -1; iCount >=0; iCount -- )
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
        private bool _IsNewLine = true;

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
                this._Text = reader.ReadToEnd();
                this._Length = this._Text.Length;
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
            this._Text = txt;
            this._Length = txt.Length;
            this.Document = doc;
        }
        public string FileName = null;
        public List<DCILField> FieldsReferenceData = null;
        public string GetSubString(int pos, int len)
        {
            return this._Text.Substring(pos, len);
        }
        public void Close()
        {
            this._Text = null;
            this._Position = 0;
        }
        public int NumOfOperCode = 0;
        public int NumOfMethod = 0;
        public int NumOfField = 0;
        public int NumOfClass = 0;
        public int NumOfProperty = 0;

        public readonly DCILDocument Document = null;
        private static readonly string[] _SplitChars = GetSplitWords();
        private static string[] GetSplitWords()
        {
            var result = new string[127];
            foreach (var c in "{}(),=<>&*[]:")
            {
                result[c] = new string(c, 1);
            }
            return result;
        }
        private static bool IsSplitChar(char c)
        {
            return c < 127 && _SplitChars[c] != null;
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
        public int CurrentLineIndex()
        {
            if (this._Position < this._LastLineIndex_Position)
            {
                this._LastLineIndex = 0;
                this._LastLineIndex_Position = 0;
            }
            for (; this._LastLineIndex_Position < this._Position; this._LastLineIndex_Position++)
            {
                if (this._Text[this._LastLineIndex_Position] == '\r')
                {
                    this._LastLineIndex++;
                }
            }

            return this._LastLineIndex;
        }

        public string PeekWord()
        {
            int pos = this._Position;
            var result = this.ReadWord();
            this._Position = pos;
            return result;
        }

        public string ReadWord()
        {
            this._CurrentItemLength = 0;
            Retry:;
            for (; this._Position < this._Length; this._Position++)
            {
                if (IsWhiteSpace(this._Text[this._Position]) == false)
                {
                    bool isInGroup = false;
                    for (; this._Position < this._Length; this._Position++)
                    {
                        char c = this._Text[this._Position];
                        if (c == '\'')
                        {
                            isInGroup = !isInGroup;
                        }
                        if (isInGroup)
                        {
                            // 在分号组内，无条件的添加
                            this._CurrentItem[this._CurrentItemLength++] = c;
                        }
                        else
                        {
                            if (c == '/' && this._Position < this._Length - 1 && this._Text[this._Position + 1] == '/')
                            {
                                // 遇到注释
                                this.MoveNextLine();
                                if (this._CurrentItemLength > 0)
                                {
                                    return GetCurrentItemString();
                                }
                                else
                                {
                                    goto Retry;
                                }
                            }
                            //if (c == ':' && this._Position < this._Length - 1 && this._Text[this._Position + 1] == ':')
                            //{

                            //}
                            if (IsWhiteSpace(c))
                            {
                                // 遇到空格
                                if (this._CurrentItemLength > 0)
                                {
                                    return GetCurrentItemString();
                                }
                            }
                            else if (IsSplitChar(c))
                            {
                                if (this._CurrentItemLength == 0)
                                {
                                    this._Position++;
                                    return _SplitChars[c];
                                }
                                else
                                {
                                    return GetCurrentItemString();
                                }
                            }
                            else
                            {
                                this._CurrentItem[this._CurrentItemLength++] = c;
                            }
                        }
                    }
                    if (this._CurrentItemLength > 0)
                    {
                        return GetCurrentItemString();
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
        public string ReadToChar(char c)
        {
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index = this._Text.IndexOf(c, this._Position);
            if (index >= 0)
            {
                var result = this._Text.Substring(this._Position, index - this._Position);
                this._Position = index;
                return result;
            }
            else
            {
                var result = this._Text.Substring(this._Position);
                this._Position = this._Length;
                return result;
            }

        }
        private readonly char[] _CurrentItem = new char[1024];
        private int _CurrentItemLength = 0;
        private string GetCurrentItemString()
        {
            if (this._CurrentItemLength == 0)
            {
                return string.Empty;
            }
            else
            {
                var result = new string(this._CurrentItem, 0, this._CurrentItemLength);
                this._CurrentItemLength = 0;
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
        public string ReadToCharExcludeLastChar(char c)
        {
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index = this._Position;
            for (; this._Position < this._Length; this._Position++)
            {
                if (this._Text[this._Position] == c)
                {
                    return this._Text.Substring(index, this._Position - index);
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
                            string result = this._Text.Substring(this._Position, iCount - this._Position);
                            this._Position = iCount + 1;
                            return result;
                        }
                    }
                }
                var result2 = this._Text.Substring(this._Position, this._Length - this._Position);
                this._Position = this._Length;
                return result2;
            }
            return null;
        }
        public string ReadAfterCharExcludeLastChar(char c)
        {
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index2 = this._Text.IndexOf(c, this._Position);
            if (index2 > 0)
            {
                var result = this._Text.Substring(this._Position, index2 - this._Position);
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
            for (; this._Position < this._Length; this._Position++)
            {

                char c = this._Text[this._Position];
                if (c == '/' && this._Position < this._Length - 1 && this._Text[this._Position + 1] == '/')
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
    //[Flags]
    //internal enum DCILRuntimeFlags
    //{
    //    COMIMAGE_FLAGS_ILONLY=1,
    //    /// <summary>
    //    /// Image can only be loaded into a 32-bit process,
    //    ///for instance if there are 32-bit vtablefixups, or
    //    ///casts from native integers to int32. CLI
    //    ///implementations that have 64-bit native
    //    ///integers shall refuse loading binaries with this
    //    ///flag set.
    //    /// </summary>
    //    COMIMAGE_FLAGS_32BITREQUIRED =2,
    //    /// <summary>
    //    /// Image has a strong name signature.
    //    /// </summary>
    //    COMIMAGE_FLAGS_STRONGNAMESIGNED = 8,
    //}
    internal class DCILDocument : DCILObject, IDisposable
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
        /// 文档注释XML文档
        /// </summary>
        public System.Xml.XmlDocument CommentXmlDoc = null;
        /// <summary>
        /// 程序集文件配套的deps.json文件的内容
        /// </summary>
        public byte[] Content_DepsJson = null;

        //public DCILRuntimeFlags CorFlags = DCILRuntimeFlags.COMIMAGE_FLAGS_ILONLY;

        public void LoadByAssembly(string asmFileName, System.Text.Encoding encoding, string ildasmExeFileName)
        {
            var ilFileName = asmFileName + ".il";
            DCUtils.RunExe(ildasmExeFileName, "\"" + asmFileName + "\" /forward /UTF8 \"/output=" + ilFileName + "\"");
            this.AssemblyFileName = asmFileName;
            this.RootPath = Path.GetDirectoryName(asmFileName);
            LoadByReader(ilFileName, encoding);
            var resFileName = Path.GetFileNameWithoutExtension(asmFileName) + ".resouces.dll";
            var dirs = Directory.GetDirectories(Path.GetDirectoryName(asmFileName));
            foreach (var dir in dirs)
            {
                if (File.Exists(Path.Combine(dir, resFileName)))
                {

                }

            }
        }

        public void LoadByReader(string fileName, System.Text.Encoding encoding)
        {
            this.LibraryNames = new Dictionary<string, string>();
            var reader = new DCILReader(fileName, encoding, this);
            this.RootPath = Path.GetDirectoryName(fileName);
            this.FileName = fileName;
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

        private Dictionary<string, int> _ClassNodesIndexs = null;
        public int GetClassNodeIndex(string name)
        {
            if (_ClassNodesIndexs == null)
            {
                this._ClassNodesIndexs = GetNodeIndexs<DCILClass>();
            }
            int result = 0;
            if (this._ClassNodesIndexs.TryGetValue(name, out result))
            {
                return result;
            }
            return -1;
        }
        public DCILClass GetClassNode(string name)
        {
            int index = GetClassNodeIndex(name);
            if (index >= 0)
            {
                return (DCILClass)this.ChildNodes[index];
            }
            else
            {
                return null;
            }
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

        //public void CollectCustomAttributes( List<DCILCustomAttribute> attrs )
        //{
        //    this.AddCustomAttributes(attrs);
        //    foreach( var asm in this.Assemblies)
        //    {
        //        asm.AddCustomAttributes(attrs);
        //    }
        //    var clses = GetAllClassesUseCache();
        //    foreach( var cls in clses.Values)
        //    {
        //        cls.AddCustomAttributes(attrs);
        //        foreach( var node in cls.ChildNodes )
        //        {
        //            node.AddCustomAttributes(attrs);
        //        }
        //    }
        //}

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
            //int len = Math.Min(500, this.ChildNodes.Count);
            //for (int iCount = 0; iCount < len; iCount++)
            //{
            //    if (this.ChildNodes[iCount] is DCILAssembly)
            //    {
            //        var asm = (DCILAssembly)this.ChildNodes[iCount];
            //        asm.CusotmAttributesCacheTypeReference(this);
            //    }
            //}
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

        private Dictionary<DCILFieldReference, DCILFieldReference> _Cache_FieldReference = new Dictionary<DCILFieldReference, DCILFieldReference>();
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
                if (code.OperCode == "call" || code.OperCode == "callvirt")
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
        private Dictionary<DCILInvokeMethodInfo, DCILInvokeMethodInfo> _CachedInvokeMethods = null;

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
            for (int iCount = 1; iCount < documents.Count; iCount++)
            {
                var document = documents[iCount];
                string msg = "       + " + Path.GetFileName( document.AssemblyFileName ) + "  (";
                if( document.FileSize > 0 )
                {
                    msg = msg + DCUtils.FormatByteSize(document.FileSize);
                }
                if(document.CommentXmlDoc != null )
                {
                    msg = msg + " with '" + document.Name + ".XML'";
                }
                msg = msg + ")";
                Console.WriteLine( msg );
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
            Console.WriteLine("       Merge " + Convert.ToString(documents.Count - 1)
                + " assembly files , add " + clsesCount + " classes, span "
                + Math.Abs(Environment.TickCount - tick) + " milliseconds.");
            return mainDoc;
        }

        public bool _HasMergeDocuments = false;

        ///// <summary>
        ///// 合并IL文档
        ///// </summary>
        ///// <param name="document">要合并的对象</param>
        ///// <returns>操作是否成功</returns>
        //public bool Merge( DCILDocument document )
        //{
        //    if( document == this || document == null )
        //    {
        //        return true ;
        //    }
        //    if (document.Resouces != null)
        //    {
        //        foreach (var item in document.Resouces)
        //        {
        //            if (this.Resouces.ContainsKey(item.Key))
        //            {
        //                ConsoleReportError("       [Warring],Duplicate resource name : " + item.Key);
        //                return false;
        //            }
        //            else
        //            {
        //                this.Resouces[item.Key] = item.Value;
        //                item.Value.Modified = true;
        //            }
        //        }
        //    }
        //    foreach (var asm in this.Assemblies)
        //    {
        //        if (string.Compare(asm.Name, document.Name, true) == 0)
        //        {
        //            this.Assemblies.Remove(asm);
        //            break;
        //        }
        //    }
        //    var itemNames = new HashSet<string>();
        //    foreach( var asm in this.Assemblies)
        //    {
        //        itemNames.Add(asm.Name);
        //    }
        //    foreach( var asm in document.Assemblies)
        //    {
        //        if( asm.IsExtern && itemNames.Contains( asm.Name )== false )
        //        {
        //            this.Assemblies.Add(asm);
        //        }
        //    }
        //    itemNames.Clear();
        //    foreach (var item in this.Modules)
        //    {
        //        if (item.IsExtern)
        //        {
        //            itemNames.Add(item.Name);
        //        }
        //    }
        //    foreach( var item in document.Modules )
        //    {
        //        if( item.IsExtern == true
        //            && itemNames.Contains( item.Name ) == false )
        //        {
        //            this.Modules.Insert(0,item);
        //        }
        //    }
        //    // 合并类型
        //    itemNames.Clear();
        //    foreach (var cls in this.Classes)
        //    {
        //        itemNames.Add(cls.Name);
        //    }
        //    foreach (var cls in document.Classes)
        //    {
        //        if (itemNames.Contains(cls.Name))
        //        {
        //            if (cls.Name == "'<PrivateImplementationDetails>'")
        //            {
        //                cls._Name = "'<PrivateImplementationDetails_" + document.Name + ">'";
        //                this.Classes.Add(cls);
        //            }
        //            else
        //            {
        //                ConsoleReportError("       [Warring],Duplicate class name : " + cls.Name);
        //                return false;
        //            }
        //        }
        //        this.Classes.Add(cls);
        //    }
        //    if (this._CachedTypes != null && this._CachedTypes.Count > 0 )
        //    {
        //        var clses = new Dictionary<string, DCILClass>();
        //        document.GetAllClasses(null, document.Classes , clses);
        //        foreach( var item in this._CachedTypes.Values )
        //        {
        //            if(item.LibraryName == document.Name )
        //            {
        //                DCILClass newCls = null;
        //                if( clses.TryGetValue( item.Name , out newCls ))
        //                {
        //                    item.LocalClass = newCls;
        //                    item.LibraryName = null;
        //                }
        //            }
        //        }
        //        if (this._Cache_FieldReference != null)
        //        {
        //            foreach (var item in this._Cache_FieldReference.Values)
        //            {
        //                if (item.OwnerType.LocalClass != null &&  item.LocalField == null )
        //                {
        //                    item.UpdateLocalField(item.OwnerType.LocalClass);
        //                }
        //            }
        //        }
        //        if( this._CachedInvokeMethods != null )
        //        {
        //            foreach( var item in this._CachedInvokeMethods.Values)
        //            {
        //                if( item.OwnerType.LocalClass != null && item.LocalMethod == null )
        //                {
        //                    item.UpdateLocalInfo(item.OwnerType.LocalClass);
        //                }
        //            }
        //        }
        //    }
        //    // 合并缓存的信息
        //    this._CachedInvokeMethods = MergeDictionary<DCILInvokeMethodInfo, DCILInvokeMethodInfo>(this._CachedInvokeMethods, document._CachedInvokeMethods);
        //    this._CachedTypes = MergeDictionary<DCILTypeReference, DCILTypeReference>(this._CachedTypes, document._CachedTypes);
        //    this._Cache_FieldReference = MergeDictionary<DCILFieldReference, DCILFieldReference>(this._Cache_FieldReference, document._Cache_FieldReference);
        //    this._Cache_TypeNameWithLibraryName = MergeDictionary<string, string>(this._Cache_TypeNameWithLibraryName, document._Cache_TypeNameWithLibraryName);
        //    if (document.ILDatas.Count > 0)
        //    {
        //        this.ILDatas.AddRange(document.ILDatas);
        //        int tick = Environment.TickCount;
        //        foreach (var item in this.ILDatas)
        //        {
        //            item._Name = "I_" + Convert.ToString(tick++);
        //        }
        //    }
        //    document.CleanFieldValue();
        //    return true ;
        //}

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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(msg);
            Console.ResetColor();
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
            if (reader.FieldsReferenceData != null
                && reader.FieldsReferenceData.Count > 0
                && this.ILDatas != null && this.ILDatas.Count > 0)
            {
                var datas = new Dictionary<string, DCILData>();
                foreach (var item in this.ILDatas)
                {
                    datas[item.Name] = item;
                }
                foreach (var field in reader.FieldsReferenceData)
                {
                    datas.TryGetValue(field.DataLabel, out field.ReferenceData);
                }
            }
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


        public readonly JieJieSwitchs ProtectedOptions = null;

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

        public string[] SourceLines = null;
        public string RootPath = null;
        public string AssemblyFileName = null;
        public string LibName_mscorlib = "mscorlib";
        public int ModifiedCount = 0;
        public List<DCILOperCode_LoadString> StringDefines = new List<DCILOperCode_LoadString>();
        //public List<DCILObject> AllGroups = new List<DCILObject>();
        //public List<DCILClass> AllClasses = new List<DCILClass>();
        public List<string> ReferenceAssemblies = new List<string>();
        public byte[] Win32ResData = null;
        public SortedDictionary<string, DCILMResource> Resouces = new SortedDictionary<string, DCILMResource>();

        public List<System.Tuple<DCILMethod, int>> SecurityMethods = new List<Tuple<DCILMethod, int>>();
        public void CleanFieldValue()
        {
            this.SourceLines = null;
            this.StringDefines = null;
            this.Resouces = null;
            this.Classes = null;
            this.ILDatas = null;
            this.Assemblies = null;
            this.Modules = null;
        }
        public void Dispose()
        {
            this.SourceLines = null;
            if (this.StringDefines != null)
            {
                this.StringDefines.Clear();
                this.StringDefines = null;
            }
            //if (this.AllGroups != null)
            //{
            //    this.AllGroups.Clear();
            //    this.AllGroups = null;
            //}
            if (this.Resouces != null)
            {
                this.Resouces.Clear();
                this.Resouces = null;
            }
            if (this.ILDatas != null)
            {
                this.ILDatas.Clear();
                this.ILDatas = null;
            }
            this.Modules = null;
            this.Classes = null;
            this.Resouces = null;
        }

        public bool _IsDotNetCoreAssembly = false;

    }
    internal class DCILOperCodeList : List<DCILOperCode>
    {
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
            this.ChangeShortInstruction_Flag = true;
            foreach (var item in this)
            {
                var code = item.OperCode;
                if (code != null
                    && code.Length > 3
                    && code[code.Length - 2] == '.'
                    && code[code.Length - 1] == 's')
                {
                    if(DCILOperCode.GetOperCodeType( code  ) == ILOperCodeType.JumpShort)
                    {
                        item.OperCode = code.Substring(0, code.Length - 2);
                    }
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
    }

    internal delegate void EnumOperCodeHandler(EnumOperCodeArgs args );

    /// <summary>
    /// 处理类型的指令
    /// </summary>
    internal class DCILOperCode_HandleClass : DCILOperCode
    {

        public DCILOperCode_HandleClass(string lableID, string operCode, DCILReader reader)
        {
            this.LabelID = lableID;
            this.OperCode = operCode;
            this.ClassType = DCILTypeReference.Load(reader);
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

    internal class DCILFieldReference
    {
        public DCILFieldReference( DCILField field )
        {
            if(field == null )
            {
                throw new ArgumentNullException("field");
            }
            this.ValueType = field.ValueType;
            this.OwnerType = new DCILTypeReference((DCILClass)field.Parent);
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
            if (this.OwnerType != null)
            {
                this.OwnerType.UpdateLocalClass(clses);
                var cls2 = this.OwnerType.LocalClass;
                if (cls2 != null)
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
            this.OperCode = operCode;
            this.Value = new DCILFieldReference(reader);
        }
        public DCILOperCode_HandleField(string labelID, string operCode, DCILFieldReference field )
        {
            this.LabelID = labelID;
            this.OperCode = operCode;
            this.Value = field;
        }

        public override string ToString()
        {
            return this.LabelID + " : " + this.OperCode + " " + this.Value.ToString();
        }

        public DCILFieldReference Value = null;

        public DCILField LocalField = null;
        public void CacheInfo(DCILDocument document)
        {
            this.Value = document.CacheFieldReference(this.Value);
        }

        public override void WriteOperData(DCILWriter writer)
        {
            if (this.LocalField != null)
            {
                writer.Write("  ");
                this.LocalField.ValueType.WriteTo(writer);
                if (this.Value != null && this.Value.modreq != null && this.Value.modreq.Length > 0)
                {
                    writer.Write(" modreq(");
                    writer.Write(this.Value.modreq);
                    writer.Write(") ");
                }
                writer.Write(' ');
                writer.Write(((DCILClass)this.LocalField.Parent).NameWithNested);
                writer.Write("::");
                writer.Write(this.LocalField.Name);
            }
            else if (this.Value != null)
            {
                this.Value.WriteTo(writer);
            }
        }

    }
    /// <summary>
    /// 处理成员方法的指令
    /// </summary>
    internal class DCILOperCode_HandleMethod : DCILOperCode
    {

        public DCILOperCode_HandleMethod()
        {

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
                    break;
                }
            }
            if( this.LocalMethod == null )
            {
                throw new NotSupportedException(type.Name + "::" + methodName);
            }
            this.InvokeInfo = info;
        }

        public DCILMethod LocalMethod = null;
        public DCILOperCode_HandleMethod(string labelID , string code , DCILMethod method )
        {
            this.LabelID = labelID;
            this.OperCode = code;
            this.InvokeInfo = new DCILInvokeMethodInfo( method );
        }
        public DCILOperCode_HandleMethod(string code, DCILReader reader)
        {
            this.OperCode = code;
            this.InvokeInfo = new DCILInvokeMethodInfo(reader);
        }
        public void CacheInfo(DCILDocument document)
        {
            this.InvokeInfo = document.CacheDCILInvokeMethodInfo(this.InvokeInfo);
        }
        public override int StackOffset
        {
            get
            {
                if(this.InvokeInfo != null )
                {
                    var lm = this.InvokeInfo.LocalMethod;
                    if( lm != null )
                    {
                        int result = 0;
                        if( lm.ReturnTypeInfo != DCILTypeReference.Type_Void )
                        {
                            result = 1;
                        }
                        if( lm.IsInstance )
                        {
                            result--;
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
                        if(this.InvokeInfo.ReturnType != DCILTypeReference.Type_Void )
                        {
                            result = 1;
                        }
                        if(this.InvokeInfo.IsInstance )
                        {
                            result--;
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
            //if(this.InvokeInfo.MethodName == "MyDispose")
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
            return this.LabelID + ":" + this.OperCode + " " + this.InvokeInfo.OwnerType.Name + "::" + this.InvokeInfo.MethodName;
        }
    }
    internal class DCILOperCode_Try_Catch_Finally : DCILOperCode
    {
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
    }
    internal enum ILOperCodeType
    {
        Normal,
        Field,
        Method,
        Class,
        Prefix,
        ldstr,
        switch_,
        ldtoken,
        JumpShort,
        Jump
    }
    internal class DCILOperCode
    {
        private readonly static Dictionary<string, ILOperCodeType> CodeTypes = null;
        private readonly static Dictionary<string, int> _StdStackOffset = null;
        static DCILOperCode()
        {
            CodeTypes = new Dictionary<string, ILOperCodeType>();
            CodeTypes["ldstr"] = ILOperCodeType.ldstr;
            CodeTypes["switch"] = ILOperCodeType.switch_;
            CodeTypes["box"] = ILOperCodeType.Class;
            CodeTypes["call"] = ILOperCodeType.Method;
            CodeTypes["callvirt"] = ILOperCodeType.Method;
            CodeTypes["castclass"] = ILOperCodeType.Class;
            CodeTypes["constrained."] = ILOperCodeType.Class;
            CodeTypes["cpobj"] = ILOperCodeType.Class;
            CodeTypes["initobj"] = ILOperCodeType.Class;
            CodeTypes["isinst"] = ILOperCodeType.Class;
            CodeTypes["ldfld"] = ILOperCodeType.Field;
            CodeTypes["ldflda"] = ILOperCodeType.Field;
            CodeTypes["ldftn"] = ILOperCodeType.Method;
            CodeTypes["ldelem"] = ILOperCodeType.Class;
            CodeTypes["ldelema"] = ILOperCodeType.Class;
            CodeTypes["ldobj"] = ILOperCodeType.Class;
            CodeTypes["ldsfld"] = ILOperCodeType.Field;
            CodeTypes["ldsflda"] = ILOperCodeType.Field;
            CodeTypes["ldtoken"] = ILOperCodeType.ldtoken;
            CodeTypes["ldvirtftn"] = ILOperCodeType.Method;
            CodeTypes["mkrefany"] = ILOperCodeType.Class;
            CodeTypes["newarr"] = ILOperCodeType.Class;
            CodeTypes["newobj"] = ILOperCodeType.Method;
            CodeTypes["refanyval"] = ILOperCodeType.Class;
            CodeTypes["sizeof"] = ILOperCodeType.Class;
            CodeTypes["stelem"] = ILOperCodeType.Class;
            CodeTypes["stfld"] = ILOperCodeType.Field;
            CodeTypes["stobj"] = ILOperCodeType.Class;
            CodeTypes["stsfld"] = ILOperCodeType.Field;
            CodeTypes["unbox"] = ILOperCodeType.Class;
            CodeTypes["unbox.any"] = ILOperCodeType.Class;

            CodeTypes["beq.s"] = ILOperCodeType.JumpShort;
            CodeTypes["bge.s"] = ILOperCodeType.JumpShort;
            CodeTypes["bge.un.s"] = ILOperCodeType.JumpShort;
            CodeTypes["bgt.s"] = ILOperCodeType.JumpShort;
            CodeTypes["bgt.un.s"] = ILOperCodeType.JumpShort;
            CodeTypes["ble.s"] = ILOperCodeType.JumpShort;
            CodeTypes["ble.un.s"] = ILOperCodeType.JumpShort;
            CodeTypes["blt.s"] = ILOperCodeType.JumpShort;
            CodeTypes["blt.un.s"] = ILOperCodeType.JumpShort;
            CodeTypes["bne.un.s"] = ILOperCodeType.JumpShort;
            CodeTypes["br.s"] = ILOperCodeType.JumpShort;
            CodeTypes["brfalse.s"] = ILOperCodeType.JumpShort;
            CodeTypes["brtrue.s"] = ILOperCodeType.JumpShort;
            CodeTypes["leave.s"] = ILOperCodeType.JumpShort;

            CodeTypes["beq"] = ILOperCodeType.Jump;
            CodeTypes["bge"] = ILOperCodeType.Jump;
            CodeTypes["bge.un"] = ILOperCodeType.Jump;
            CodeTypes["bgt"] = ILOperCodeType.Jump;
            CodeTypes["bgt.un"] = ILOperCodeType.Jump;
            CodeTypes["ble"] = ILOperCodeType.Jump;
            CodeTypes["ble.un"] = ILOperCodeType.Jump;
            CodeTypes["blt"] = ILOperCodeType.Jump;
            CodeTypes["blt.un"] = ILOperCodeType.Jump;
            CodeTypes["bne.un"] = ILOperCodeType.Jump;
            CodeTypes["br"] = ILOperCodeType.Jump;
            CodeTypes["brfalse"] = ILOperCodeType.Jump;
            CodeTypes["brtrue"] = ILOperCodeType.Jump;
            CodeTypes["leave"] = ILOperCodeType.Jump;


            _StdStackOffset = new Dictionary<string, int>();
            _StdStackOffset["add"] = -1;
            _StdStackOffset["add.ovf"] = -1;
            _StdStackOffset["add.ovf.un"] = -1;
            _StdStackOffset["and"] = -1;
            _StdStackOffset["beq"] = -2;
            _StdStackOffset["beq.s"] = -2;
            _StdStackOffset["bge"] = -2;
            _StdStackOffset["bge.s"] = -2;
            _StdStackOffset["bge.un"] = -2;
            _StdStackOffset["bge.un.s"] = -2;
            _StdStackOffset["bgt"] = -2;
            _StdStackOffset["bgt.s"] = -2;
            _StdStackOffset["bgt.un"] = -2;
            _StdStackOffset["bgt.un.s"] = -2;
            _StdStackOffset["ble"] = -2;
            _StdStackOffset["ble.s"] = -2;
            _StdStackOffset["ble.un"] = -2;
            _StdStackOffset["ble.un.s"] = -2;
            _StdStackOffset["blt"] = -2;
            _StdStackOffset["blt.s"] = -2;
            _StdStackOffset["blt.un"] = -2;
            _StdStackOffset["blt.un.s"] = -2;
            _StdStackOffset["bne.un"] = -2;
            _StdStackOffset["bne.us.s"] = -2;
            _StdStackOffset["brfalse"] = -1;
            _StdStackOffset["brfalse.s"] = -1;
            _StdStackOffset["brtrue"] = -1;
            _StdStackOffset["brtrue.s"] = -1;
            _StdStackOffset["ceq"] = -1;
            _StdStackOffset["cgt"] = -1;
            _StdStackOffset["cgt.un"] = -1;
            _StdStackOffset["clt"] = -1;
            _StdStackOffset["clt.un"] = -1;
            _StdStackOffset["cpblk"] = -3;
            _StdStackOffset["cpobj"] = -2;
            _StdStackOffset["div"] = -1;
            _StdStackOffset["div.un"] = -1;
            _StdStackOffset["dup"] = 1;
            _StdStackOffset["endfilter"] = -1;
            _StdStackOffset["initblk"] = -3;
            _StdStackOffset["initobj"] = -1;
            _StdStackOffset["ldarg"] = 1;
            _StdStackOffset["ldarg.0"] = 1;
            _StdStackOffset["ldarg.1"] = 1;
            _StdStackOffset["ldarg.2"] = 1;
            _StdStackOffset["ldarg.3"] = 1;
            _StdStackOffset["ldarg.s"] = 1;
            _StdStackOffset["ldarga"] = 1;
            _StdStackOffset["ldargs.s"] = 1;
            _StdStackOffset["ldc.i4"] = 1;
            _StdStackOffset["ldc.i4.0"] = 1;
            _StdStackOffset["ldc.i4.1"] = 1;
            _StdStackOffset["ldc.i4.2"] = 1;
            _StdStackOffset["ldc.i4.3"] = 1;
            _StdStackOffset["ldc.i4.4"] = 1;
            _StdStackOffset["ldc.i4.5"] = 1;
            _StdStackOffset["ldc.i4.6"] = 1;
            _StdStackOffset["ldc.i4.7"] = 1;
            _StdStackOffset["ldc.i4.8"] = 1;
            _StdStackOffset["ldc.i4.m1"] = 1;
            _StdStackOffset["ldc.i4.s"] = 1;
            _StdStackOffset["ldc.i8"] = 1;
            _StdStackOffset["ldc.r4"] = 1;
            _StdStackOffset["ldc.r8"] = 1;
            _StdStackOffset["ldelem"] = -1;
            _StdStackOffset["ldelem.i1"] = -1;
            _StdStackOffset["ldelem.i2"] = -1;
            _StdStackOffset["ldelem.i3"] = -1;
            _StdStackOffset["ldelem.i8"] = -1;
            _StdStackOffset["ldelem.r4"] = -1;
            _StdStackOffset["ldelem.r8"] = -1;
            _StdStackOffset["ldelem.ref"] = -1;
            _StdStackOffset["ldelem.u1"] = -1;
            _StdStackOffset["ldelem.u2"] = -1;
            _StdStackOffset["ldelem.u4"] = -1;
            _StdStackOffset["ldelema"] = -1;
            _StdStackOffset["ldloc"] = 1;
            _StdStackOffset["ldloc.0"] = 1;
            _StdStackOffset["ldloc.1"] = 1;
            _StdStackOffset["ldloc.2"] = 1;
            _StdStackOffset["ldloc.3"] = 1;
            _StdStackOffset["ldloc.s"] = 1;
            _StdStackOffset["ldloc"] = 1;
            _StdStackOffset["ldloc"] = 1;
            _StdStackOffset["ldloca"] = 1;
            _StdStackOffset["ldloca.s"] = 1;
            _StdStackOffset["ldnull"] = 1;
            _StdStackOffset["ldsfld"] = 1;
            _StdStackOffset["ldsflda"] = 1;
            _StdStackOffset["ldstr"] = 1;
            _StdStackOffset["ldtoken"] = 1;
            _StdStackOffset["mul"] = -1;
            _StdStackOffset["mul.ovf"] = 1;
            _StdStackOffset["mul.ovf.un"] = 1;
            _StdStackOffset["or"] = -1;
            _StdStackOffset["pop"] = -1;
            _StdStackOffset["rem"] = -1;
            _StdStackOffset["rem.un"] = 1;
            _StdStackOffset["shl"] = -1;
            _StdStackOffset["shr"] = -1;
            _StdStackOffset["shr.un"] = -1;
            _StdStackOffset["starg"] = -1;
            _StdStackOffset["starg.s"] = 1;
            _StdStackOffset["stelem"] = -3;
            _StdStackOffset["stelem.i"] = -3;
            _StdStackOffset["stelem.i1"] = -3;
            _StdStackOffset["stelem.i2"] = -3;
            _StdStackOffset["stelem.i4"] = -3;
            _StdStackOffset["stelem.i8"] = -3;
            _StdStackOffset["stelem.r4"] = -3;
            _StdStackOffset["stelem.r8"] = -3;
            _StdStackOffset["stelem.ref"] = -3;
            _StdStackOffset["stfld"] = -2;
            _StdStackOffset["stlnd.i"] = -2;
            _StdStackOffset["stlnd.i1"] = -2;
            _StdStackOffset["stlnd.i2"] = -2;
            _StdStackOffset["stlnd.i4"] = -2;
            _StdStackOffset["stlnd.i8"] = -2;
            _StdStackOffset["stlnd.r4"] = -2;
            _StdStackOffset["stlnd.r8"] = -2;
            _StdStackOffset["stlnd.ref"] = -2;
            _StdStackOffset["stloc"] = -1;
            _StdStackOffset["stloc.0"] = -1;
            _StdStackOffset["stloc.1"] = -1;
            _StdStackOffset["stloc.2"] = -1;
            _StdStackOffset["stloc.3"] = -1;
            _StdStackOffset["stloc.s"] = -1;
            _StdStackOffset["stobj"] = -2;
            _StdStackOffset["stsfld"] = -1;
            _StdStackOffset["sub"] = -1;
            _StdStackOffset["sub.ovf"] = -1;
            _StdStackOffset["sub.ovf.un"] = -1;
            _StdStackOffset["switch"] = -1;
            _StdStackOffset["thorw"] = -1;
            _StdStackOffset["unaligned"] = -1;
            _StdStackOffset["volatile"] = -1;
            _StdStackOffset["xor"] = -1;
        }

        public static int GetValue_ldc_i4(DCILOperCode code)
        {
            if (code != null && code.OperCode != null && code.OperCode.Length > 0)
            {
                switch (code.OperCode)
                {
                    case "ldc.i4": return DCUtils.ConvertToInt32(code.OperData);
                    case "ldc.i4.0": return 0;
                    case "ldc.i4.1": return 1;
                    case "ldc.i4.2": return 2;
                    case "ldc.i4.3": return 3;
                    case "ldc.i4.4": return 4;
                    case "ldc.i4.5": return 5;
                    case "ldc.i4.6": return 6;
                    case "ldc.i4.7": return 7;
                    case "ldc.i4.8": return 8;
                    case "ldc.i4.s": return DCUtils.ConvertToInt32(code.OperData);
                    case "ldc.i4.m1": return -1;
                }
            }
            return int.MinValue;
        }

        /// <summary>
        /// 获得指令类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ILOperCodeType GetOperCodeType(string code)
        {
            if( code == null )
            {
                return ILOperCodeType.Normal;
            }
            ILOperCodeType result = ILOperCodeType.Normal;
            if (CodeTypes.TryGetValue(code, out result))
            {
                return result;
            }
            else
            {
                return ILOperCodeType.Normal;
            }
        }
        public DCILOperCode()
        {

        }
        public DCILOperCode(string vlabelID, string voperCode, string voperData)
        {
            this.LabelID = vlabelID;
            this.OperCode = voperCode;
            this.OperData = voperData;
        }
        public DCILOperCode(string line, int vLineIndex)
        {
            if (line == null || line.Length == 0)
            {
                throw new ArgumentNullException("line");
            }
            this.OperCode = GetILCode(line, ref this.LabelID, ref this.OperData);
            this.LineIndex = vLineIndex;
            this.EndLineIndex = vLineIndex;
            this.NativeSource = line;
        }

        internal int _StackOffset = int.MinValue;
        public virtual int StackOffset
        {
            get
            {
                if( this._StackOffset == int.MinValue )
                {
                    if (this.OperCode == null || this.OperCode.Length == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        int v = 0;
                        if (_StdStackOffset.TryGetValue(this.OperCode, out v))
                        {
                            return v;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                return this._StackOffset;
            }
        }

        public bool IsJumpOperCode()
        {
            var type = GetOperCodeType(this.OperCode);
            return type == ILOperCodeType.Jump || type == ILOperCodeType.JumpShort;
        }

        /// <summary>
        /// 判断是否为修饰性指令，必须紧跟在后面的指令之前
        /// </summary>
        /// <returns></returns>
        public bool IsPrefixOperCode()
        {
            return this.OperCode == "volatile."
                || this.OperCode == "constrained."
                //|| operCode == "cpblk"
                || this.OperCode == "unaligned"
                || this.OperCode == "tailcall";
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
        private static int _InstanceIndexCounter = 0;
        public int InstanceIndex = _InstanceIndexCounter++;
        //public bool Enabled = true;
        public DCILOperCodeList OwnerList = null;
        public DCILMethod OwnerMethod = null;
        public string NativeSource = null;
        public string LabelID = null;
        public bool HasLabelID()
        {
            return this.LabelID != null && this.LabelID.Length > 0;
        }
        public string OperCode = null;
        public string OperData = null;
        public int LineIndex = 0;
        public int EndLineIndex = 0;
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
                return this.LabelID + " : " + this.OperCode;
            }
            else
            {
                return this.LabelID + " : " + this.OperCode + "     " + this.OperData;
            }
        }
        /// <summary>
        /// get IL opercode from a IL code line
        /// </summary>
        /// <param name="line">IL code line</param>
        /// <param name="labelID">label id</param>
        /// <param name="operData">opertion data</param>
        /// <returns></returns>
        internal static string GetILCode(string line, ref string labelID, ref string operData)
        {
            int len = line.Length;
            for (int iCount = 0; iCount < len; iCount++)
            {
                char c = line[iCount];
                if (c == ':')
                {
                    labelID = line.Substring(0, iCount).Trim();
                    for (iCount++; iCount < len; iCount++)
                    {
                        var c2 = line[iCount];
                        if (c2 != ' ' && c2 != '\t')
                        {
                            string operCode = null;
                            for (int iCount2 = iCount + 1; iCount2 < len; iCount2++)
                            {
                                var c3 = line[iCount2];
                                if (c3 == ' ' || c3 == '\t')
                                {
                                    operCode = line.Substring(iCount, iCount2 - iCount);
                                    if (iCount2 < len - 1)
                                    {
                                        operData = line.Substring(iCount2).Trim();
                                    }
                                    break;
                                }
                            }
                            if (operCode == null)
                            {
                                operCode = line.Substring(iCount);
                            }
                            return operCode;
                        }
                    }
                }
            }
            return null;
        }

    }

    internal class DCILOperCode_LdToken : DCILOperCode
    {
        public DCILOperCode_LdToken(string labelID, DCILFieldReference field)
        {
            this.OperCode = "ldtoken";
            this.LabelID = labelID;
            this.OperType = "field";
            this.FieldReference = field;
        }
        public DCILOperCode_LdToken(string labelID, DCILTypeReference type)
        {
            this.OperCode = "ldtoken";
            this.LabelID = labelID;
            this.ClassType = type;
        }
        //public const string CodeName_LdToken = "ldtoken";
        public DCILOperCode_LdToken(string labelID, DCILReader reader)
        {
            this.OperCode = "ldtoken";

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

    internal class DCILStringValue
    {
        public DCILStringValue(DCILReader reader)
        {
            int posBack = reader.Position;
            var strOperData = reader.ReadLine()?.Trim();
            if (strOperData[0] == '"')
            {
                int startPos = posBack;
                this.IsBinary = false;
                if (strOperData.Length == 2 && strOperData[1] == '"')
                {
                    this.Value = string.Empty;
                }
                var strFinalValue = new StringBuilder();
                GetFinalValue(strOperData, strFinalValue);
                while (reader.HasContentLeft())
                {
                    posBack = reader.Position;
                    var line2 = reader.ReadLine().Trim();
                    if (line2.Length > 0 && line2[0] == '+')
                    {
                        //line2 = RemoveComment(line2).Trim();
                        //strOperData = strOperData + Environment.NewLine + line2;
                        GetFinalValue(line2, strFinalValue);
                    }
                    else
                    {
                        reader.Position = posBack;
                        break;
                    }
                }
                this.RawILText = reader.GetSubString(startPos, reader.Position - startPos).Trim();
                this.Value = strFinalValue.ToString();
            }
            else if (strOperData.StartsWith("bytearray", StringComparison.Ordinal))
            {
                this.IsBinary = true;
                reader.Position = posBack;
                reader.ReadToChar('(');
                var bs = reader.ReadBinaryFromHex();
                this.RawILText = reader.GetSubString(posBack, reader.Position - posBack).Trim();
                if (bs != null && bs.Length > 0)
                {
                    this.Value = Encoding.Unicode.GetString(bs);
                }
                else
                {
                    this.Value = string.Empty;
                }

            }
        }
        private void GetFinalValue(string line, StringBuilder result)
        {
            int index = line.IndexOf('"');
            int len = line.Length;
            for (int iCount = line.IndexOf('"') + 1; iCount < len; iCount++)
            {
                var c = line[iCount];
                if (c == '\\' && iCount < len - 1)
                {
                    var nc = line[iCount + 1];
                    iCount++;
                    switch (nc)
                    {
                        case 'r': result.Append('\r'); break;
                        case 'n': result.Append('\n'); break;
                        case '\'': result.Append('\''); break;
                        case '"': result.Append('"'); break;
                        case '\\': result.Append('\\'); break;
                        case 'b': result.Append('\b'); break;
                        case 'f': result.Append('\f'); break;
                        case 't': result.Append('\t'); break;
                        default: result.Append(nc); break;
                    }
                }
                else if (c == '"')
                {
                    break;
                }
                else
                {
                    result.Append(c);
                }
            }
        }
        public readonly string Value = null;
        public readonly bool IsBinary = false;
        public readonly string RawILText = null;
    }
    internal class DCILOperCode_LoadString : DCILOperCode
    {
        public const string CodeName_ldstr = "ldstr";

        public DCILOperCode_LoadString(DCILReader reader)
        {
            this.OperCode = CodeName_ldstr;
            var v = new DCILStringValue(reader);
            this.IsBinary = v.IsBinary;
            this.FinalValue = v.Value;
            this.OperData = v.RawILText;

        }

        public DCILOperCode_LoadString(DCILOperCode code)
        {
            this.OperCode = "ldstr";
            this.LabelID = code.LabelID;
            this.OperData = code.OperData;
            this.LineIndex = code.LineIndex;
            this.EndLineIndex = code.EndLineIndex;
            this.NativeSource = code.NativeSource;
            this.OwnerList = code.OwnerList;
            this.OwnerMethod = code.OwnerMethod;
        }
        public string FinalValue = null;
        /// <summary>
        /// 是否采用二进制格式来定义
        /// </summary>
        public bool IsBinary = false;
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
        public string DataLabel = null;
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
                    this.DataLabel = reader.ReadWord();
                    if (this.DataLabel != null && this.DataLabel.Length > 0)
                    {
                        if (reader.FieldsReferenceData == null)
                        {
                            reader.FieldsReferenceData = new List<DCILField>();
                        }
                        reader.FieldsReferenceData.Add(this);
                    }
                    break;
                }
                else if (strWord == "=")
                {
                    if (this.ValueType == DCILTypeReference.Type_String)
                    {
                        var v = new DCILStringValue(reader);
                        this.ConstValue = v.RawILText;
                    }
                    else
                    {
                        this.ConstValue = reader.ReadLineTrimRemoveComment();
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
        public string OldSignature = null;
        public void UpdateOldSignature()
        {
            var writer = new DCILWriter(new StringBuilder());
            this.ValueType.WriteToForSignString(writer);
            this.OldSignature = writer.ToString();
        }

        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".field ");
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
            else if (this.DataLabel != null && this.DataLabel.Length > 0)
            {
                writer.Write(" at ");
                writer.Write(this.DataLabel);
            }
            writer.WriteLine();
            base.WriteCustomAttributes(writer);
        }

        public int SpecifyIndex = int.MaxValue;

        public override string ToString()
        {
            return "field " + this.ValueType + " " + this._Name;
        }
    }
    internal class DCILGenericParamterList : List<DCILGenericParamter>
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
    internal class DCILGenericParamter : IEqualsValue<DCILGenericParamter>
    {
        public DCILGenericParamter()
        {

        }
        public DCILGenericParamter(string name, bool defineInClass)
        {
            this.Name = name;
            this.DefineInClass = defineInClass;
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

        public DCILClass(Type nativeType, DCILDocument document) : base(nativeType)
        {
            this.IsEnum = nativeType.IsEnum;
            this.IsMulticastDelegate = typeof(System.MulticastDelegate).IsAssignableFrom(nativeType);
            this.IsInterface = nativeType.IsInterface;
            this.IsImport = nativeType.IsImport;
            this.ChildNodes = new DCILObjectList();
            this.NativeType = nativeType;
            this._Name = DCUtils.GetFullName(nativeType);
            if (nativeType.IsGenericType)
            {
                var gps = nativeType.GetGenericArguments();
                this.GenericParamters = new DCILGenericParamterList(gps.Length);
                foreach (var item in gps)
                {
                    var p = new DCILGenericParamter();
                    p.Name = DCUtils.GetFullName(item);
                    this.GenericParamters.Add(p);
                }
            }
            if (nativeType.BaseType != null)
            {
                this.BaseType = DCILTypeReference.CreateByNativeType(nativeType.BaseType, document);
            }
            var its = nativeType.GetInterfaces();
            if (its != null && its.Length > 0)
            {
                this.ImplementsInterfaces = new List<DCILTypeReference>(its.Length);
                foreach (var item in its)
                {
                    this.ImplementsInterfaces.Add(DCILTypeReference.CreateByNativeType(item, document));
                }
            }

        }
        public readonly Type NativeType = null;

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
            this.InnerGenerate = true;
        }

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
                this.IsMulticastDelegate = this.BaseType.Name == "System.MulticastDelegate";
            }
        }

       // public IDGenerator idGenForMember = null;

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
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == "}")
                {
                    break;
                }
                switch (strWord)
                {
                    case DCILCustomAttribute.TagName_custom:
                        base.ReadCustomAttribute(reader);
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
            writer.Write(".class ");
            base.WriteStyles(writer);
            if (this._Name == "__DC20210205.__jiejienet_sm")
            {

            }
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
            base.WriteCustomAttributes(writer);
            if (this.NestedClasses != null && this.NestedClasses.Count > 0)
            {
                foreach (var item in this.NestedClasses)
                {
                    writer.WriteLine();
                    item.WriteTo(writer);
                }
            }
            writer.WriteObjects(this.ChildNodes);
            writer.WriteEndGroup();
        }

        public List<DCILClass> NestedClasses = null;

        public List<DCILTypeReference> ImplementsInterfaces = null;
        public List<int> FieldLineIndexs = new List<int>();
        public bool Modified = false;
        public DCILTypeReference BaseType = null;
        public bool IsMulticastDelegate = false;
        public bool IsEnum = false;

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

        public string ConvertTypeName
        {
            get
            {
                if (this.Values != null && this.Values.Length == 1)
                {
                    return this.Values[0].Value?.ToString();
                }
                return null;
            }
        }

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
        /// <summary>
        /// 不执行任何操作
        /// </summary>
        /// <returns></returns>
        public override bool UpdateBinaryValueForLocalClassRename()
        {
            return false;
        }
    }

    internal class DCILCustomAttributeValue
    {
        public DCILCustomAttributeValue()
        {
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
                    av.Name = ReadUTF8String(reader);
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
                case DCILElementType.String: return ReadUTF8String(args.Reader);
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
                    base.Parse(ReadUTF8String(args.Reader));
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
                var str = ReadUTF8String(args.Reader);
                base.Parse(str);
                UpdateLocalInfo(args);
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
                if( this.TypeName.IndexOf('+') < 0 )
                {
                    return doc.GetAllClassesUseCache().TryGetValue(this.TypeName, out this.LocalClass);
                }
                else
                {
                    var tns = this.TypeName.Split('+');
                    foreach( var cls in doc.Classes )
                    {
                        if( cls.Name == tns[0])
                        {
                            DCILClass curCls = cls;
                            for(int iCount = 1; iCount < tns.Length; iCount ++)
                            {
                                if(curCls.NestedClasses == null )
                                {
                                    break;
                                }
                                foreach( var cls2 in curCls.NestedClasses)
                                {
                                    if( cls2.Name ==  tns[iCount])
                                    {
                                        if(iCount == tns.Length -1 )
                                        {
                                            this.LocalClass = cls2;
                                            return true;
                                        }
                                        curCls = cls2;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            protected void UpdateLocalInfo(ReadCustomAttributeValueArgs args)
            {
                if (this.HasAssemblyName && args.Documents != null)
                {
                    foreach (var doc in args.Documents)
                    {
                        if (doc.Name == this.AssemblyName)
                        {
                            if(SetLocalClass( doc ))
                            {
                                return;
                            }
                            break;
                        }
                    }
                }
                else if (args.MainDocument != null)
                {
                    if(SetLocalClass(args.MainDocument ))
                    {
                        return;
                    }
                }
                var t2 = new DCILTypeReference(this.TypeName, DCILTypeMode.Unsigned);
                t2.LibraryName = this.AssemblyName;
                this.NativeType = t2.SearchNativeType(args.AssemblySeachPath);
            }

            public bool UpdateForLocalClassNameChanged()
            {
                bool result = false;
                if (this.HasAssemblyName)
                {
                    if (this.AssemblyName.StartsWith("System."))
                    {

                    }
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
    }

    internal class DCILCustomAttribute : DCILObject
    {
        public static DCILCustomAttribute Create(DCILObject parent, DCILReader reader)
        {
            var invokeInfo = new DCILInvokeMethodInfo(reader);
            reader.ReadAfterChar('=');
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
            if (this.TypeName == "System.Diagnostics.DebuggerTypeProxyAttribute")
            {

            }
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
            writer.Write(".custom ");
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
            reader.ReadAfterChar('{');
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
        private Dictionary<string, DCILClass> _ClassMap = new Dictionary<string, DCILClass>();

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
        public DCILModule()
        {

        }
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
        public DCILUnknowObject()
        {

        }
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
        /// <summary>
        /// 为重命名而添加EditorBrowsableAttribute特性
        /// </summary>
        /// <param name="ebattr"></param>
        public bool AddEditorBrowsableAttributeForRename(DCILCustomAttribute ebattr)
        {
            if (this.RenameState == DCILRenameState.Renamed)
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
        public void CollectAttributes(List<DCILCustomAttribute> attributes)
        {
            if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
            {
                attributes.AddRange(this.CustomAttributes);
            }
        }
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
                }
            }
        }
    }
    internal class DCILObjectList : List<DCILObject>
    {
        public DCILObjectList()
        {

        }
        public DCILObjectList Clone()
        {
            var list = new DCILObjectList();
            list.AddRange(this);
            return list;
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

    internal class DCILObject
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
        internal void ReadCustomAttribute(DCILReader reader)
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
    internal class DCILTypeNameInfo
    {
        public DCILTypeNameInfo()
        {

        }
        public DCILTypeNameInfo(string name)
        {
            Parse(name);
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
    internal class DCILTypeReference : IEqualsValue<DCILTypeReference>
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

            Type_Void = _PrimitiveTypes["void"];
            Type_String = _PrimitiveTypes["string"];
            Type_Object = _PrimitiveTypes["object"];
            Type_Int32 = _PrimitiveTypes["int32"];
            Type_Boolean = _PrimitiveTypes["bool"];
            Type_Char = _PrimitiveTypes["char"];

            _Cache_CreateByNativeType[typeof(string)] = Type_String;

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
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(Environment.NewLine + "    [Warring]Error load referenced assembly file : " + asmFileName + " , MSG:" + ext.Message);
                                    Console.ResetColor();
                                }
                            }
                        }
                        else
                        {
                            if (_MissLibNames.Contains(this.LibraryName) == false)
                            {
                                _MissLibNames.Add(this.LibraryName);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(Environment.NewLine + "    [Warring]Can not find referenced assembly : " + this.LibraryName + ".dll");
                                Console.ResetColor();
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
                    this.Name = firstWord.Substring(2);
                }
                else
                {
                    this.Mode = DCILTypeMode.GenericTypeInTypeDefine;
                    this.Name = firstWord.Substring(1);
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
                    this.ArrayAndPointerSettings = new string(result.ToArray());
                    return true;
                }
            }
            return false;
        }
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

    internal class DCILMethodParamter
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
        public List<DCILMethodParamter> CloneList( List<DCILMethodParamter > ps)
        {
            if( ps != null )
            {
                var ps2 = new List<DCILMethodParamter>();
                foreach( var p in ps )
                {
                    ps2.Add(p.Clone());
                }
                return ps2;
            }
            else
            {
                return null;
            }
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
                if (readName == false)
                {

                }
                if (mp == null)
                {
                    mp = new DCILMethodParamter();
                    paramters.Add(mp);
                }
                if (word == "[")
                {
                    word = reader.ReadAfterCharExcludeLastChar(']').Trim();
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
                foreach (var item in paramters)
                {
                    if (item.ValueType == null)
                    {

                    }
                }
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

        public DCILTypeReference GetResultValueTypeForLoad(DCILOperCode code)
        {
            if (code == null || code.OperCode == null || code.OperCode.Length == 0)
            {
                return null;
            }
            if (code.OperCode == "ldfld" || code.OperCode == "ldsfld")
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
                if (code.OperCode == "ldarg" || code.OperCode == "ldarg.s")
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
                else if (code.OperCode == "ldloc" || code.OperCode == "ldloc.s")
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
                    switch (code.OperCode)
                    {
                        case "ldarg.0": pIndex = 0; break;
                        case "ldarg.1": pIndex = 1; break;
                        case "ldarg.2": pIndex = 2; break;
                        case "ldarg.3": pIndex = 3; break;

                        case "ldloc.0": locIndex = 0; break;
                        case "ldloc.1": locIndex = 1; break;
                        case "ldloc.2": locIndex = 2; break;
                        case "ldloc.3": locIndex = 3; break;
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
            if (code.OperCode == "stfld" || code.OperCode == "stsfld")
            {
                if (code is DCILOperCode_HandleField)
                {
                    var rf = ((DCILOperCode_HandleField)code).Value?.ValueType;
                    return rf;
                }
                return null;
            }
            if(code.OperCode == "starg" || code.OperCode == "starg.s")
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
                if (code.OperData == "stloc" || code.OperData == "stloc.s")
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
                    if (code.OperCode == "stloc.0") index = 0;
                    else if (code.OperCode == "stloc.1") index = 1;
                    else if (code.OperCode == "stloc.2") index = 2;
                    else if (code.OperCode == "stloc.3") index = 3;
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

        public readonly System.Reflection.MethodInfo _NativeMethod = null;
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
            //if(this.Name == "GetAllChildControlsDeeplyInner")
            //{

            //}
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
            //this._SignStringWithoutName = null;
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
            //if (reader.CurrentLineIndex() > 116140)
            //{

            //}
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
                    if (this.Pinvokeimpl != null && this.Pinvokeimpl.Contains("udev_monitor_new_from_netlink"))
                    {

                    }
                }
                else if (DCILTypeReference.IsStartWord(strWord))
                {
                    this.ReturnTypeInfo = DCILTypeReference.Load(strWord, reader);
                    if (reader.PeekWord() == "marshal")
                    {
                        reader.ReadWord();
                        this.ReturnMarshal = reader.ReadStyleExtValue();
                        if( this.ReturnMarshal.Contains("custom"))
                        {

                        }
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
                reader.ReadAfterCharExcludeLastChar('(');
            }
            this.Parameters = DCILMethodParamter.ReadParameters(reader);
            this.DeclearEndFix = reader.ReadAfterCharExcludeLastChar('{').Trim();
            //if(this._Name == "SetValueCore")
            //{
            //    int v3 = reader.CurrentLineIndex();
            //}
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
                    //reader.Position = pos;
                    //string labelID = reader.ReadAfterCharExcludeLastChar(':').Trim();
                    string strOperCode = reader.ReadWord();
                    var operCodeType = DCILOperCode.GetOperCodeType(strOperCode);
                    switch (operCodeType)
                    {
                        case ILOperCodeType.ldstr:
                            {
                                var code = new DCILOperCode_LoadString(reader);
                                code.LabelID = labelID;
                                operInfoList.Add(code);
                            }
                            break;
                        case ILOperCodeType.Method:
                            {
                                var code = new DCILOperCode_HandleMethod(strOperCode, reader);
                                code.LabelID = labelID;
                                operInfoList.Add(code);
                            }
                            break;
                        case ILOperCodeType.Field:
                            {
                                operInfoList.Add(new DCILOperCode_HandleField(labelID, strOperCode, reader));
                            }
                            break;
                        case ILOperCodeType.ldtoken:
                            {
                                operInfoList.Add(new DCILOperCode_LdToken(labelID, reader));
                            }
                            break;
                        case ILOperCodeType.Class:
                            {
                                operInfoList.Add(new DCILOperCode_HandleClass(labelID, strOperCode, reader));
                            }
                            break;
                        case ILOperCodeType.switch_:
                            {
                                reader.Position--;
                                reader.ReadToChar('(');
                                string strOperData = reader.ReadToCharExcludeLastChar(')') + ")";
                                operInfoList.AddItem(labelID, strOperCode, strOperData);
                            }
                            break;
                        default:
                            {
                                var strOperData = reader.ReadLineTrim();
                                if( strOperCode == "ldc.r8")
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
                                else if(strOperCode == "ldc.r4")
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
                                operInfoList.AddItem(labelID, strOperCode, strOperData);
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
                        //var line = reader.ReadLine();
                        //catch2.ExcpetionTypeName = line.Trim().Substring(6).Trim();
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
                            pinfo.Name = reader.ReadAfterCharsExcludeLastChar(",)", out endChar).Trim();

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
          
        ///// <summary>
        ///// 旧的签名信息
        ///// </summary>
        //public string OldSignature = null;
        //public void UpdateOldSignature()
        //{
        //    //var this.OwnerClass.NameWithNested
        //    this.OldSignature = InnerGetSignString(this.ReturnTypeInfo, null, this.GenericParamters, ((DCILClass)this.Parent)?.GenericParamters, this.Parameters);
        //}


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

        public JieJieSwitchs ProtectedOptions = null;

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
    internal class DCILMethodLocalVariableList : List<DCILMethodLocalVariable>
    {
        public bool HasInit = true;
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
                
                if (res.Name.IndexOf("LocalWebServer")>=0)
                {

                }
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
            reader.ReadAfterCharExcludeLastChar('}');
            this.IsResources = this._Name.EndsWith(EXT_Resources);
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
                Console.WriteLine("    Change resource language : " + this.Name);

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
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.WriteLine("       [Warring],Duplicate resource item name : " + this.Name + " # " + item.Key);
                            Console.ResetColor();
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
                    throw new NotSupportedException(item.Value.GetType().FullName);
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
            reader.ReadAfterChar('{');
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
        public DCILProperty()
        {

        }
        public DCILProperty(DCILClass cls, DCILReader reader)
        {
            this.Parent = cls;
            this.Load(reader);
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
            reader.ReadAfterCharExcludeLastChar('(');
            this.Parameters = DCILMethodParamter.ReadParameters(reader, false);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {

            }
            reader.ReadAfterChar('{');
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
            //this.Length = len;
            //this._Indexs = new int[len];
            //this._CharsLength = _Chars.Length;
            this._ClassNamePrefx = strPreFix;
            this._MemberNamePrefix = memberPrefix;
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

        //private int[] _Indexs = null;
        //public readonly int Length = 0;
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
            return id;
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

        //private HashSet<string> _ExistedID = null;
        //public bool CheckExisedID
        //{
        //    get
        //    {
        //        return this._ExistedID != null;
        //    }
        //    set
        //    {
        //        if( (this._ExistedID != null ) != value )
        //        {
        //            if(value )
        //            {
        //                this._ExistedID = new HashSet<string>();
        //            }
        //            else
        //            {
        //                this._ExistedID = null;
        //            }
        //        }
        //    }
        //}
        //public void AddExisedID(string id )
        //{
        //    if(this._ExistedID == null )
        //    {
        //        this._ExistedID = new HashSet<string>();
        //    }
        //    if(this._ExistedID.Contains(id )== false )
        //    {
        //        this._ExistedID.Add(id);
        //    }
        //}

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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   >>RUN:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\"" + pstart.FileName + "\" ");
            if (argument != null && argument.Length > 0)
            {
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(argument);
            }
            Console.ResetColor();
            Console.WriteLine();
            System.Diagnostics.Debug.WriteLine(">>RUN: \"" + pstart.FileName + "\" " + pstart.Arguments);
            pstart.UseShellExecute = false;
            //pstart.CreateNoWindow = false;
            var p = System.Diagnostics.Process.Start(pstart);
            p.WaitForExit();
        }

        public static void EatAllConsoleKey()
        {
            while( Console.KeyAvailable )
            {
                Console.ReadKey();
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
        public static string GetFullName(Type t)
        {
            return GetFullName(t.Namespace, t.Name);
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

            //int len1 = s1 == null ? 0 : s1.Length;
            //int len2 = s2 == null ? 0 : s2.Length;
            //if( len1 != len2 )
            //{
            //    return false;
            //}
            //return s1 == s2;
        }
        public static int ExpandResourcesToPath(
            System.Reflection.Assembly asm,
            string resBaseName,
            string rootPath,
            bool overWrite)
        {
            int result = 0;
            var names = asm.GetManifestResourceNames();
            foreach (var name in names)
            {
                if (name.StartsWith(resBaseName))
                {
                    result++;
                    var fn = Path.Combine(rootPath, name.Substring(resBaseName.Length));
                    if (overWrite == true || File.Exists(fn) == false)
                    {
                        var stream = asm.GetManifestResourceStream(name);
                        var data = new System.IO.MemoryStream();
                        var bsTemp = new byte[1024];
                        while (true)
                        {
                            int len = stream.Read(bsTemp, 0, bsTemp.Length);
                            if (len > 0)
                            {
                                data.Write(bsTemp, 0, len);
                            }
                            else
                            {
                                break;
                            }
                        }//while
                        File.WriteAllBytes(fn, data.ToArray());
                        stream.Close();
                        data.Close();
                    }
                }
            }
            return result;
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


    internal class StringValueContainer
    {
        public string LibName_mscorlib = "mscorlib";

        private static readonly Random _Random = new Random();

        private int KeyOffset = _Random.Next(10000, 99999);
        public byte[] Datas = null;

        public readonly List<StringValueItem> Items = new List<StringValueItem>();
        public void Clear()
        {
            this.Items.Clear();
            this.KeyOffset = _Random.Next(10000, 99999);
            this.Datas = null;
        }
        internal class StringValueItem
        {
            public int ValueIndex = 0;
            public int StartIndex = 0;
            public int Length = 0;
            public int EncryptKey = 0;
            public long LongParamter = 0;
            public string Value = null;
        }

        public void AddItem(int valueIndex, string v)
        {
            if (v == null || v.Length == 0)
            {
                throw new ArgumentNullException("v");
            }
            var item = new StringValueItem();
            item.ValueIndex = valueIndex;
            item.Value = v;
            this.Items.Add(item);
        }

        public void RefreshState()
        {
            var lstDatas = new List<byte>();
            int startIndexCount = 0;
            foreach (var item in this.Items)
            {
                item.StartIndex = startIndexCount;
                item.Length = item.Value.Length;
                startIndexCount += item.Length;
                item.EncryptKey = _Random.Next(10000, ushort.MaxValue - 10000);
                long longKey = item.StartIndex;
                longKey = (longKey << 24) + item.Length;
                longKey = (longKey << 16) + (ushort)(item.EncryptKey ^ this.KeyOffset);
                item.LongParamter = longKey;

                var key = item.EncryptKey;
                foreach (var c in item.Value)
                {
                    ushort v3 = (ushort)(c ^ key);
                    //lstData2.Add(v3);
                    lstDatas.Add((byte)(v3 >> 8));
                    lstDatas.Add((byte)(v3 & 0xff));
                    //ushort v9 = BitConverter.ToUInt16(lstDatas.ToArray(), lstDatas.Count-2);
                    key++;
                }
            }
            this.Datas = lstDatas.ToArray();
            DCUtils.ObfuseListOrder(this.Items);
            /*
       private static string GetStringByLong(byte[] datas, long key)
       {
           int key2 = (int)(key & 0xffff) ^ 9999;
           key >>= 16;
           int length = (int)(key & 0xfffff);
           key >>= 24;
           int startIndex = (int)key;
           char[] array = new char[length];
           for (int i = 0; i < length; i++, key2++)
           {
               int index2 = (i + startIndex) << 2;
               array[i] = (char)(((datas[index2] << 8) + datas[index2 + 1]) ^ key2);
           }
           return new string(array);
       }

                */
            this.IL_GetStringLong = @"
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
	IL_000a: ldc.i4 " + this.KeyOffset + @"
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
} // end of method Class3::GetStringByLong";
            this.IL_GetStringLong = this.IL_GetStringLong.Replace("mscorlib", this.LibName_mscorlib);
        }

        public string IL_GetStringLong = null;

        private static string GetStringByLong(byte[] datas, long key)
        {
            int key2 = (int)(key & 0xffff) ^ 9999;
            key >>= 16;
            int length = (int)(key & 0xfffff);
            key >>= 24;
            int startIndex = (int)key;
            char[] array = new char[length];
            for (int i = 0; i < length; i++, key2++)
            {
                int index2 = (i + startIndex) << 2;
                array[i] = (char)(((datas[index2] << 8) + datas[index2 + 1]) ^ key2);
            }
            return new string(array);
        }

    }

    internal class ILLabelIDGen
    {
        public ILLabelIDGen( string strPreFix = null , int startCount = 10 )
        {
            if (strPreFix != null && strPreFix.Length > 0)
            {
                this.Prefix = strPreFix;
            }
            this.Counter = startCount;
        }

        public string Prefix = "IL_New";
        public int Counter = 10;
        public string Gen()
        {
            string v = this.Prefix + this.Counter.ToString();
            this.Counter+=10;
            return v;
        }
    }
}

namespace __DC20211119
{
    /// <summary>
    /// 内嵌的帮助类型，只有在.NET2.0和Release模式下编译后的IL代码才能使用。
    /// </summary>
    internal static class JIEJIEHelper
    {
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
    internal static class ProjectAssetsJsonHelper
    {
        public static List<string> GetRefAsseblyFiles(string jsonText )
        {
            var resultList = new List<string>();
            var doc = System.Text.Json.JsonDocument.Parse(jsonText);
            System.Text.Json.JsonElement e1 ;
            System.Text.Json.JsonElement e2  ;
            if( doc.RootElement.TryGetProperty("targets" , out e1 ))
            {

            }
            
            return resultList;
        }
    }
#endif
}