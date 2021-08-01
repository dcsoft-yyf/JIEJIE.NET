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


[assembly: AssemblyTitle("JieJie.NET console application")]
[assembly: AssemblyDescription("Protect your .NET software copyright powerfull.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DCSoft")]
[assembly: AssemblyProduct("JieJie.NET")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(JIEJIE.DCJieJieNetEngine.ProductVersion)]

namespace JIEJIE
{
#if !DCSoftInner
    static class ConsoleProgram
    {
        static void Main(string[] args)
        {
            //Test();return;
            args = new string[] {
                @"d:\temp2\DCSoft.Writer.ForWinForm.dll",
                @"output=E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\DCSoft.DCWriterSimpleDemo\Lib\DCSoft.Writer.ForWinForm.dll",
                @"snk=E:\Source\DCSoft\08代码\DCSoft\DCWriter专用版\DCSoft.Writer.ForASPNETCore_All\yyf.snk",
                "pause"
            };
            string inputAssmblyFileName = null;
            string outputAssemblyFileName = null;
            bool pause = false;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
*******************************************************************************
             __   __   _______        __   __   _______   .__   __.  _______ .___________.
            |  | |  | |   ____|      |  | |  | |   ____|  |  \ |  | |   ____||           |
            |  | |  | |  |__         |  | |  | |  |__     |   \|  | |  |__   `---|  |----`
      .--.  |  | |  | |   __|  .--.  |  | |  | |   __|    |  . `  | |   __|      |  |     
      |  `--'  | |  | |  |____ |  `--'  | |  | |  |____ __|  |\   | |  |____     |  |     
       \______/  |__| |_______| \______/  |__| |_______(__)__| \__| |_______|    |__|     

     JieJie.NET v" + DCJieJieNetEngine.ProductVersion + @" ,encrypt .NET assembly file, help you protect copyright.
     Jie(2)Jie(4) in chinese is a kind of transparet magic protect shield.
     Author:yuan yong fu . mail: 28348092@qq.com
     Site :https://github.com/dcsoft-yyf/JIEJIE.NET
     Last update 2021-7-10
     You can use this software unlimited,but CAN NOT modify source code anytime.
     Any good idears you can write to 28348092@qq.com.
     Support command line argument :
        input =[required,default argument,Full path of input .NET assembly file , can be .exe or .dll, currenttly only support .NET framework 2.0 or later]
        output=[optional,Full path of output .NET assmebly file , if it is empty , then use input argument value]
        snk   =[optional,Full path of snk file. It use to add strong name to output assembly file.]
        switch=[optional,multi-switch split by ',',also can be define in [System.Reflection.ObfuscationAttribute.Feature]. It support :
                +/-contorlfow  = enable/disable obfuscate control flow in method body.
                +/-strings     = enable/disable encrypt string value.
                +/-resources   = enable/disable encrypt resources data.
                +/-memberorder = enable/disable member list order in type.
                +/-rename      = enable/disable rename type or member's name.
                +/-allocationcallstack  = enable/disable encrypt string value allocation callstack.
                +/-removemember= enable/disable remove unused const field.
            ]
        pause =[optional,pause the console after finish process.]
        debugmode=[optional,Allow show some debug info text.]
        sdkpath=[optional,set the direcotry full name of ildasm.exe.]

     Example 1, protect d:\a.dll ,this will modify dll file.
        >JIEJIE.NET.exe d:\a.dll  
     Exmaple 2, anlyse d:\a.dll , and write result to another dll file with strong name. enable obfuscate control flow and not encript resources.
        >JIEJIE.NET.exe input=d:\a.dll output=d:\publish\a.dll snk=d:\source\company.snk options=+contorlfow,-resources
　　　　　　　　　　　　　　　　　　　　
******************** MADE IN CHINA **********************************************");
            Console.ResetColor();
            var eng = new DCJieJieNetEngine();

            if (args != null)
            {
                foreach (var arg in args)
                {
                    int index = arg.IndexOf('=');
                    if (index > 0)
                    {
                        string argName = arg.Substring(0, index).Trim().ToLower();
                        string argValue = arg.Substring(index + 1).Trim();
                        switch (argName)
                        {
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
                                if( argValue != null
                                    && argValue.Length > 0 
                                    && File.Exists( argValue ) == false )
                                {
                                    ConsoleWriteError("Can not find file : " + argValue);
                                    return;
                                }
                                eng.SnkFileName = argValue;
                                break;
                            case "pause":
                                pause = true;
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
                            case "debugmode":
                                eng.DebugMode = true;
                                break;
                        }
                    }
                    else if (string.Compare(arg, "pause", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        pause = true;
                    }
                    else if (arg != null && arg.Length > 0 && Path.IsPathRooted(arg) && File.Exists(arg))
                    {
                        inputAssmblyFileName = arg;
                    }
                }
            }
            try
            {
                if (inputAssmblyFileName != null && inputAssmblyFileName.Length > 0)
                {
                    if (File.Exists(inputAssmblyFileName))
                    {
                        Console.Title = "JieJie.NET - " + inputAssmblyFileName;
                        eng = eng.CreateEngineCrossAppdomain(Path.GetDirectoryName(inputAssmblyFileName));
                        eng.LoadAssemblyFile(inputAssmblyFileName);
                        eng.HandleDocument();
                        if( outputAssemblyFileName == null || outputAssemblyFileName.Length == 0 )
                        {
                            var dir = Path.Combine(Path.GetDirectoryName(inputAssmblyFileName), "jiejie.net_result");
                            if( Directory.Exists( dir ) == false )
                            {
                                Directory.CreateDirectory(dir);
                            }
                            outputAssemblyFileName = Path.Combine(dir, Path.GetFileName(inputAssmblyFileName));
                        }
                        else if ( System.IO.Directory.Exists( outputAssemblyFileName ) )
                        {
                            outputAssemblyFileName = Path.Combine(outputAssemblyFileName, Path.GetFileName(inputAssmblyFileName));
                        }
                        eng.SaveAssemblyFile(outputAssemblyFileName, true);
                        eng.Close();
                    }
                    else
                    {
                        ConsoleWriteError("Can not find file : " + inputAssmblyFileName);
                    }
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
                    Console.WriteLine("[JieJie.NET] All finished, press any key to continue...");
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

        public static void Test()
        {
            //var gen = new IDGenerator();
            //for( int iCount = 0; iCount < 1000; iCount ++ )
            //{
            //    System.Console.WriteLine(gen.GenerateID());
            //}

            //var ccc = new DCILInvokeMethodInfo(new DCILReader("instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult>::GetEnumerator()" , null ));
            //var bbb = new DCILInvokeMethodInfo(new DCILReader("class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> Open_Newtonsoft_Json.Utilities.LinqBridge.JsonEnumerable::Select<valuetype [mscorlib]System.Collections.DictionaryEntry,  valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<!TKey, !TValue>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class Open_Newtonsoft_Json.Serialization.JsonFunc`2<!!0, !!1>)", null));
            //var doc = new DCILDocument(@"D:\temp2\ilcompress\DCILTemp\DCSoft.Writer.ForWinForm.dll.il", Encoding.UTF8);
            //var doc = new DCILDocument();
            //var exeFileName = @"C:\Users\Administrator\source\repos\ConsoleApp4\ConsoleApp4\bin\Debug\ConsoleApp4.exe";
            //var exeFileName = @"d:\temp2\DCSoft.Common.dll";
            var inputAsmFileName = @"d:\temp2\DCSoft.Writer.ForWinForm.dll";
            //doc.LoadByAssembly(exeFileName, Encoding.UTF8, @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\ildasm.exe");
            //doc.LoadByReader(@"D:\temp2\DCSoft.Writer.ForWinForm.dll.il", Encoding.UTF8);
            //return;
            var eng = new DCJieJieNetEngine();
            eng.SnkFileName = @"E:\Source\DCSoft\08代码\DCSoft\DCWriterForWinForm\yyf.snk";
            eng.DebugMode = false;
            eng.LoadAssemblyFile(inputAsmFileName);
            eng.HandleDocument();

            //eng.SelectUILanguage();
            //eng.HandleCollectStringValue();
            //eng.AddClassInnerAssemblyHelper20210315();
            //eng.ApplyResouceContainerClass();
            //var nodes = eng.Document.ChildNodes.Clone();
            //foreach (var item in nodes)
            //{
            //    if (item is DCILClass)
            //    {
            //        eng.ChangeComponentResourceManager((DCILClass)item);
            //        foreach (var item2 in item.ChildNodes)
            //        {
            //            if (item2 is DCILMethod)
            //            {
            //                eng.ObfuscateOperCodeList((DCILMethod)item2);
            //            }
            //        }
            //    }
            //}
            //eng.AddDatasClass();
            //eng.Document.FixDomState();
            //eng.ObfuseClassOrder();
            //eng.RenameClasses();
            var outputAsmFileName = @"E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\DCSoft.DCWriterSimpleDemo\Lib\DCSoft.Writer.ForWinForm.dll";
            eng.SaveAssemblyFile(outputAsmFileName, true);

            //var ilFileName = Path.ChangeExtension(exeFileName, ".loaded.il");
            //doc.WriteToFile(ilFileName, Encoding.Default);
            //var path2 = Path.GetDirectoryName(typeof(string).Assembly.Location);
            //var exe2 = @"E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\DCSoft.DCWriterSimpleDemo\Lib\DCSoft.Writer.ForWinForm.dll";// Path.ChangeExtension(exeFileName, "new.exe");

            //ResourceFileHelper.RunExe(Path.Combine(path2, "ilasm.exe"), ilFileName + " /dll  /output:" + exe2);
            //ResourceFileHelper.RunExe(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\sn.exe", "-Ra " + exe2 + " " + snkFile);
            //ResourceFileHelper.RunExe(Path.Combine(path2, "ngen.exe"), "install " + exe2);
            //ResourceFileHelper.RunExe(Path.Combine(path2, "ngen.exe"), "uninstall " + exe2);
            Console.ReadLine();
            //doc.LoadByReader(@"D:\temp2\temp5.il", Encoding.UTF8);

        }

        //public static void Test2()
        //{
        //    //var gen = new IDGenerator();
        //    //for( int iCount = 0; iCount < 1000; iCount ++ )
        //    //{
        //    //    System.Console.WriteLine(gen.GenerateID());
        //    //}

        //    //var ccc = new DCILInvokeMethodInfo(new DCILReader("instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!TResult>::GetEnumerator()" , null ));
        //    //var bbb = new DCILInvokeMethodInfo(new DCILReader("class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> Open_Newtonsoft_Json.Utilities.LinqBridge.JsonEnumerable::Select<valuetype [mscorlib]System.Collections.DictionaryEntry,  valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<!TKey, !TValue>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class Open_Newtonsoft_Json.Serialization.JsonFunc`2<!!0, !!1>)", null));
        //    //var doc = new DCILDocument(@"D:\temp2\ilcompress\DCILTemp\DCSoft.Writer.ForWinForm.dll.il", Encoding.UTF8);
        //    var doc = new DCILDocument();
        //    var snkFile = @"E:\Source\DCSoft\08代码\DCSoft\DCWriterForWinForm\yyf.snk";
        //    //var exeFileName = @"C:\Users\Administrator\source\repos\ConsoleApp4\ConsoleApp4\bin\Debug\ConsoleApp4.exe";
        //    //var exeFileName = @"d:\temp2\DCSoft.Common.dll";
        //    var exeFileName = @"d:\temp2\DCSoft.Common.dll";
        //    //var exeFileName = @"E:\Source\DCSoft\08代码\DCSoft\DCWriterForWinForm\bin\debug\DCSoft.Writer.Core.dll";
        //    doc.LoadByAssembly(exeFileName, Encoding.UTF8, @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\ildasm.exe");
        //    //doc.LoadByReader(@"D:\temp2\DCSoft.Writer.ForWinForm.dll.il", Encoding.UTF8);
        //    //return;
        //    var eng = new DCJieJieNetEngine(doc);
        //    eng.DebugMode = false;
        //    eng.HandleCollectStringValue();
        //    eng.AddClassInnerAssemblyHelper20210315();
        //    eng.ApplyResouceContainerClass();
        //    var nodes = doc.ChildNodes.Clone();
        //    foreach (var item in nodes)
        //    {
        //        if (item is DCILClass)
        //        {
        //            eng.ChangeComponentResourceManager((DCILClass)item);
        //            foreach (var item2 in item.ChildNodes)
        //            {
        //                if (item2 is DCILMethod)
        //                {
        //                    eng.ObfuscateOperCodeList((DCILMethod)item2);
        //                }
        //            }
        //        }
        //    }
        //    eng.AddDatasClass();
        //    eng.Document.FixDomState();
        //    //eng.RenameClasses();

        //    var ilFileName = Path.ChangeExtension(exeFileName, ".loaded.il");
        //    doc.WriteToFile(ilFileName, Encoding.Default);
        //    var path2 = Path.GetDirectoryName(typeof(string).Assembly.Location);
        //    var exe2 = exeFileName + ".new.dll";// Path.GetDirectoryName( exeFileName )  + Path.GetFileName(exeFileName);// DCSoft.Common.dll";// Path.ChangeExtension(exeFileName, "new.exe");

        //    ResourceFileHelper.RunExe(Path.Combine(path2, "ilasm.exe"), ilFileName + " /dll  /output:" + exe2);
        //    ResourceFileHelper.RunExe(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\sn.exe", "-Ra " + exe2 + " " + snkFile);
        //    ResourceFileHelper.RunExe(Path.Combine(path2, "ngen.exe"), "install " + exe2);
        //    ResourceFileHelper.RunExe(Path.Combine(path2, "ngen.exe"), "uninstall " + exe2);
        //    Console.ReadLine();
        //    //doc.LoadByReader(@"D:\temp2\temp5.il", Encoding.UTF8);

        //}

    }

#endif
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
        public bool RemoveMember = true ;
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
        public const string ProductVersion = "1.2.0.0";


        public DCJieJieNetEngine(DCILDocument doc)
        {
            this.Document = doc;
        }
        public DCJieJieNetEngine()
        {
        }

        public DCJieJieNetEngine CreateEngineCrossAppdomain( string basePath )
        {
            if( basePath == null || basePath.Length == 0 )
            {
                return this;
            }
            if( string.Compare( AppDomain.CurrentDomain.SetupInformation.ApplicationBase , basePath , true ) == 0 )
            {
                return this;
            }
            var appSetup = new System.AppDomainSetup();
            appSetup.PrivateBinPath = basePath;
            appSetup.ApplicationName = "jiejie.net temp";
            appSetup.ApplicationBase = basePath;
            if (AppDomain.CurrentDomain.ApplicationTrust != null)
            {
                appSetup.ApplicationTrust = AppDomain.CurrentDomain.ApplicationTrust;
            }
            var domain = System.AppDomain.CreateDomain("jiejie.net temp" , AppDomain.CurrentDomain.Evidence , appSetup);
            var eng = (DCJieJieNetEngine) domain.CreateInstanceFromAndUnwrap(this.GetType().Assembly.Location, this.GetType().FullName);

            eng.ContentEncoding = this.ContentEncoding;
            eng.DebugMode = this.DebugMode;
            eng.RenamePrefix = this.RenamePrefix;
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
            eng.SetCrossAppdomain();
            return eng;
        }
        private void SetCrossAppdomain()
        {
            this._Appdomain = System.AppDomain.CurrentDomain;
            this._CreateCrossAppdomain = true;
        }

        //private void CopySettingsFrom(object obj)
        //{
        //    DCJieJieNetEngine eng = (DCJieJieNetEngine)obj;
        //    this.ContentEncoding = eng.ContentEncoding;
        //    this.DebugMode = eng.DebugMode;
        //    this.RenamePrefix = eng.RenamePrefix;
        //    this.SDKDirectory = eng.SDKDirectory;
        //    this.SnkFileName = eng.SnkFileName;
        //    if (eng.Switchs != null)
        //    {
        //        this.Switchs = new JieJieSwitchs();
        //        this.Switchs.AllocationCallStack = eng.Switchs.AllocationCallStack;
        //        this.Switchs.ControlFlow = eng.Switchs.ControlFlow;
        //        this.Switchs.MemberOrder = eng.Switchs.MemberOrder;
        //        this.Switchs.Rename = eng.Switchs.Rename;
        //        this.Switchs.Resources = eng.Switchs.Resources;
        //        this.Switchs.Strings = eng.Switchs.Strings;
        //    }
        //    this.TempDirectory = eng.TempDirectory;
        //    this._UILanguageDisplayName = eng._UILanguageDisplayName;
        //    this._UILanguageName = eng._UILanguageName;
        //}

        public void Close()
        {
            if (this.Document != null)
            {
                this.Document.Dispose();
                this.Document = null;
            }
            if( this._Appdomain != null && this._Appdomain != AppDomain.CurrentDomain)
            {
                AppDomain.Unload(this._Appdomain);
                this._Appdomain = null;
            }
            this._AllClasses?.Clear();
            this._AllBaseTypes.Clear();
            this._ByteDataContainer = null;
            this._IDGenForClass = null;
            _NativeTypeMethods.Clear();
            _Native_BaseMethods.Clear();
        }

        private bool _CreateCrossAppdomain = false;
        private System.AppDomain _Appdomain = null;

        /// <summary>
        /// 保存程序集文件
        /// </summary>
        /// <param name="asmFileName">程序集文件</param>
        /// <param name="checkUseNgen">是否使用 Ngen.exe 进行验证</param>
        /// <returns>操作是否成功</returns>
        public bool SaveAssemblyFile(string asmFileName, bool checkUseNgen)
        {
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
            var ilFileName = Path.Combine(this.TempDirectory, "result_" + Path.GetFileName(asmFileName) + ".il");
            this.Document.WriteToFile(ilFileName, this.ContentEncoding);
            var path2 = Path.GetDirectoryName(typeof(string).Assembly.Location);
            var asmTempFileName = Path.Combine(this.TempDirectory, "Temp_" + Path.GetFileName(asmFileName));
            if (File.Exists(asmTempFileName))
            {
                File.Delete(asmTempFileName);
            }
            // 生成临时程序集文件
            if (this.DebugMode)
            {
                ResourceFileHelper.RunExe(Path.Combine(path2, "ilasm.exe"), "\"" + ilFileName + "\" /dll  \"/output:" + asmTempFileName + "\"");
            }
            else
            {
                ResourceFileHelper.RunExe(Path.Combine(path2, "ilasm.exe"), "\"" + ilFileName + "\" /dll  \"/output:" + asmTempFileName + "\" /quiet");
            }
            if (File.Exists(asmTempFileName))
            {
                if (this.SnkFileName != null && this.SnkFileName.Length > 0 && File.Exists(this.SnkFileName))
                {
                    ResourceFileHelper.RunExe(Path.Combine(this.SDKDirectory, "sn.exe"), "-Ra \"" + asmTempFileName + "\" " + this.SnkFileName);
                }
                File.Copy(asmTempFileName, asmFileName, true);
                File.Delete(asmTempFileName);
                if (checkUseNgen)
                {
                    ConsoleWriteTask();
                    Console.WriteLine("Testing by ngen.exe...");
                    ResourceFileHelper.RunExe(Path.Combine(path2, "ngen.exe"), "install \"" + asmFileName + "\"");
                    ResourceFileHelper.RunExe(Path.Combine(path2, "ngen.exe"), "uninstall \"" + asmFileName + "\"");
                }
                ConsoleWriteTask();
                Console.WriteLine("Job finished.");
                return true;
            }
            else
            {
                ConsoleWriteTask();
                Console.WriteLine("Job failed.");
                return false;
            }
        }
        /// <summary>
        /// 加载程序集文件
        /// </summary>
        /// <param name="asmFileName">程序集文件名</param>
        /// <returns>操作是否成功</returns>
        public bool LoadAssemblyFile(string asmFileName)
        {
            if (asmFileName == null || asmFileName.Length == 0)
            {
                throw new ArgumentNullException("asmFileName");
            }
            if (File.Exists(asmFileName) == false)
            {
                throw new FileNotFoundException(asmFileName);
            }
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
            ConsoleWriteTask();
            string ilFileName = null;
            if (asmFileName.EndsWith(".il", StringComparison.OrdinalIgnoreCase))
            {
                // 已经存在 il 文件。
                ilFileName = asmFileName;
            }
            else
            {
                // 调用 ildasm.exe 将程序集文件反编译为 il 文件。
                Console.WriteLine("Loading assembly file " + asmFileName);
                ilFileName = Path.Combine(this.TempDirectory, Path.GetFileName(asmFileName) + ".il");
                var ildasmExeFileName = Path.Combine(this.SDKDirectory, "ildasm.exe");
                ResourceFileHelper.RunExe(
                    ildasmExeFileName,
                    "\"" + asmFileName + "\" /forward /UTF8 \"/output=" + ilFileName + "\"");
                string rootDir = Path.GetDirectoryName(asmFileName);
                foreach (var dir in Directory.GetDirectories(rootDir))
                {
                    // 反编译资源DLL文件
                    var resDllFileName = Path.Combine(
                            dir,
                            Path.GetFileNameWithoutExtension(asmFileName) + DCILDocument.EXT_resources + ".dll");
                    if (File.Exists(resDllFileName))
                    {
                        var tempFileName = Path.Combine(this.TempDirectory, Path.GetFileName(dir));
                        if (Directory.Exists(tempFileName) == false)
                        {
                            Directory.CreateDirectory(tempFileName);
                        }
                        tempFileName = Path.Combine(tempFileName, Path.GetFileNameWithoutExtension(resDllFileName) + ".il");
                        ResourceFileHelper.RunExe(
                            ildasmExeFileName,
                            "\"" + resDllFileName + "\" /forward /UTF8 \"/output=" + tempFileName + "\"");
                    }
                }
                Console.WriteLine(" span " + Math.Abs(Environment.TickCount - tick) + " milliseconds.");
            }

            tick = Environment.TickCount;
            ConsoleWriteTask();
            Console.Write("Anlysing IL file...");
            var doc = new DCILDocument();
            doc.LoadByReader(ilFileName, this.ContentEncoding);
            doc.AssemblyFileName = asmFileName;
            doc.RootPath = Path.GetDirectoryName(ilFileName);
            this.Document = doc;
            Console.WriteLine(" span " + Math.Abs(Environment.TickCount - tick) + " milliseconds. get " + doc.ChildNodes.Count + " IL entries.");
            return true;
        }

        public void HandleDocument()
        {
            this.SelectUILanguage();
            if (this.Switchs.Strings)
            {
                this.HandleCollectStringValue();
            }
            this.AddClassInnerAssemblyHelper20210315();
            if (this.Switchs.Resources)
            {
                this.ApplyResouceContainerClass();
            }
            var nodes = this.Document.ChildNodes.Clone();
            foreach (var item in nodes)
            {
                if (item is DCILClass)
                {
                    HandleClass((DCILClass)item, this.Switchs);
                }
            }
            this.AddDatasClass();
            this.Document.FixDomState();
            if (this.Switchs.MemberOrder)
            {
                this.ObfuseClassOrder();
            }
            if (this.Switchs.Rename)
            {
                this.RenameClasses();
            }
            if( this.Switchs.RemoveMember )
            {
                RemoveMember();
            }
        }
        private void RemoveMember()
        {
            ConsoleWriteTask();
            Console.Write("Remove Type members ...");
            SortedDictionary<string, int> counters = null;
            if (this.DebugMode)
            {
                counters = new SortedDictionary<string, int>();
            }
            int tick = Environment.TickCount;
            int removeCount = 0;
            foreach (var item in this.Document.ChildNodes)
            {
                if (item is DCILClass)
                {
                    var cls = (DCILClass)item;
                    var localCount = 0;
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
                            for (int iCount = cls.ChildNodes.Count - 1; iCount >= 0; iCount--)
                            {
                                var field = cls.ChildNodes[iCount] as DCILField;
                                if (field != null && field.IsConst && field.RenameState == DCILRenameState.Renamed)
                                {
                                    // 删除被混淆的常量
                                    cls.ChildNodes.RemoveAt(iCount);
                                    removeCount++;
                                    localCount++;
                                }
                            }
                        }
                    }
                    //if( this.DebugMode && localCount > 0 )
                    if (localCount > 0 && counters != null)
                    {
                        if (cls.OldName == null)
                        {
                            counters[cls.Name] = localCount;
                        }
                        else
                        {
                            counters[cls.OldName] = localCount;
                        }
                    }
                }
            }
            if (counters != null)
            {
                foreach (var item in counters)
                {
                    Console.WriteLine("     " + item.Key + " \t remove " + item.Value + " const fields.");
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
            Console.WriteLine("    Total remove " + removeCount + " const fields , span " + tick + " milliseconds.");
        }

        private void RunTask(string title, VoidHandler handler)
        {
            Console.ResetColor();
            Console.Write(title);
            int tick = Environment.TickCount;
            handler();
            tick = Math.Abs(Environment.TickCount - tick);
            Console.WriteLine(" Span " + tick + " milliseconds.");
        }
        private delegate void VoidHandler();

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
        public string TempDirectory = Path.Combine(Path.GetTempPath(), "JieJie.NET_ProtecterTemp");

        public string RenamePrefix = "Zz.z0ZzZzb";

        private int _IndexCounter = 0;
        public int AllocIndex()
        {
            return this._IndexCounter++;
        }
        private static readonly string _ClassNamePrefix = "__DC20210205._";

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
                InnerGetAllClasses(this.Document.ChildNodes, this._AllClasses);
            }
            return this._AllClasses;
        }
        private void InnerGetAllClasses(System.Collections.IEnumerable nodes, List<DCILClass> list)
        {
            foreach (var item in nodes)
            {
                if (item is DCILClass)
                {
                    var cls = (DCILClass)item;
                    list.Add(cls);
                    if (cls.NestedClasses != null && cls.NestedClasses.Count > 0)
                    {
                        InnerGetAllClasses(cls.NestedClasses, list);
                    }
                }
            }
        }


        public string _UILanguageName = null;
        public string _UILanguageDisplayName = null;

        public void SelectUILanguage()
        {
            this._UILanguageName = null;
            var allResFiles = this.Document.ResouceFiles;
            if (allResFiles.Count > 0)
            {
                var culs = this.Document.GetSupportCultures();
                if (culs != null && culs.Length > 0)
                {
                    ConsoleWriteTask();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please select UI language you want:");
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (int iCount = 0; iCount < culs.Length; iCount++)
                    {
                        Console.WriteLine(iCount + ":" + culs[iCount].Name + " " + culs[iCount].DisplayName);
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Please input index,press enter to use default,");
                    Console.ResetColor();

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
                                _UILanguageName = culs[index].Name;
                                _UILanguageDisplayName = culs[index].DisplayName;
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("[Task]");
            Console.ResetColor();
            Console.Write(" ");
        }
        public void HandleCollectStringValue()
        {
            int tick = Environment.TickCount;
            ConsoleWriteTask();
            Console.Write("Encrypting string values ...");
            var allClasses = this.GetAllClasses();
            var strNativeCodes = new List<DCILOperCode_LoadString>();
            foreach (var cls in allClasses)
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        var method = (DCILMethod)item;
                        method.CollectOperCodes<DCILOperCode_LoadString>(strNativeCodes);
                    }
                }
            }
            if (strNativeCodes.Count == 0)
            {
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
            const int maxGroupSize = 50;

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
                this.Document.ChildNodes.Add(newCls);
                foreach (var item in newILCodes)
                {
                    item.Value.UpdateLocalField(newCls);
                }
            }
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

        private static readonly Dictionary<Type, List<DCILMethod>> _Native_BaseMethods = new Dictionary<Type, List<DCILMethod>>();
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
                    var nt = baseType.SearchNativeType(this.Document.RootPath);
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
                            if (method.Name == "ToString")
                            {

                            }
                            var key = method.GetSignString();
                            List<DCILMethod> baseMethods = null;
                            if (baseTypeMethodMap.TryGetValue(key, out baseMethods))
                            {
                                this.AddInfos(baseMethods, method);
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

            public void AddInfos(List<DCILMethod> baseMethods, DCILMethod method)
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
        public int RenameClasses()
        {
            ConsoleWriteTask();
            Console.Write("Renaming.....");
            int tick = Environment.TickCount;
            var idGen = new IDGenerator(this.RenamePrefix);
            idGen.DebugMode = this.DebugMode;

            var allClasses = GetAllClasses();
            var attributes = new List<DCILCustomAttribute>();
            foreach (var cls in allClasses)
            {
                cls.CollectAttributes(attributes);
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        ((DCILMemberInfo)item).CollectAttributes(attributes);
                    }
                }
            }
            var clsNameMaps = new RenameMapInfo();
            int result = RenameByOverrideList(allClasses, idGen, clsNameMaps);
            // 执行函数的重命名
            var idGenForClass = new IDGenerator(this.RenamePrefix);
            foreach (var cls in allClasses)
            {
                if (cls.IsImport)
                {
                    // 外界导入的接口，则不改名
                    continue;
                }
                var clsOs = cls.ObfuscationSettings;
                //cls.RemoveObfuscationAttribute();
                if (clsOs == null || clsOs.Exclude == false)
                {
                    cls.OldName = cls.Name;
                    clsNameMaps.Register(cls);
                    cls.ChangeName(idGenForClass.GenerateIDForClass(cls.OldName, cls));
                    result++;
                }
                else
                {
                    cls.RenameState = DCILRenameState.Preserve;
                }
                //continue;
                if (clsOs == null || clsOs.Exclude == false || clsOs.ApplyToMembers == false)
                {
                    if (cls.IsMulticastDelegate)// .BaseType?.Name == "System.MulticastDelegate")
                    {
                        continue;
                    }
                    // 重命名成员
                    foreach (var item in cls.ChildNodes)
                    {
                        if (item is DCILMethod)
                        {
                            var method = (DCILMethod)item;
                            if (method.RenameState == DCILRenameState.NotHandled)
                            {
                                if (method.Pinvokeimpl != null && method.Pinvokeimpl.Length > 0)
                                {
                                    method.RenameState = DCILRenameState.Preserve;
                                }
                                else if (method.ObfuscationSettings == null
                                    || method.ObfuscationSettings.Exclude == false)
                                {
                                    method.OldName = method.Name;
                                    method.ChangeName(idGen.GenerateIDForMember(method.OldName, method));
                                    result++;
                                }
                            }
                        }
                        else if (item is DCILField)
                        {
                            var field = (DCILField)item;
                            if (field.ObfuscationSettings == null || field.ObfuscationSettings.Exclude == false)
                            {
                                if (cls.IsEnum && field.Name == "value__" && field.HasStyle("specialname"))
                                {
                                    continue;
                                }
                                field.OldName = field.Name;
                                field.ChangeName(idGen.GenerateIDForMember(field.OldName, field));
                                result++;
                            }
                        }
                    }
                }
            }//foreach
            if (result > 0)
            {
                foreach (var attr in attributes)
                {
                    if (attr.UpdateValuesForRename(clsNameMaps))
                    {
                        result++;
                    }
                }
            }
            int totalCount_cls = 0;
            int handleCount_cls = 0;
            int totalCount_field = 0;
            int handleCount_field = 0;
            int totalCount_Method = 0;
            int handleCount_Method = 0;
            foreach (var cls in allClasses)
            {
                if (cls.NewGenerate == false)
                {
                    totalCount_cls++;
                    if (cls.RenameState == DCILRenameState.Renamed)
                    {
                        handleCount_cls++;
                    }
                }
                cls.RemoveObfuscationAttribute();
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        ((DCILMemberInfo)item).RemoveObfuscationAttribute();
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
                            }
                            else if (cls.RenameState == DCILRenameState.Renamed && DCILMethod.IsCtorOrCctor(method.Name))
                            {
                                RenameMethodParameter(method);
                            }
                        }
                    }
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
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
            if (strResult.Length > 0)
            {
                Console.WriteLine(strResult.ToString());
            }
            return result;
        }
        /// <summary>
        /// 重命名函数的参数名
        /// </summary>
        /// <param name="method"></param>
        private void RenameMethodParameter(DCILMethod method )
        {
            if (method.Parameters != null && method.Parameters.Count > 0)
            {
                var maps = new Dictionary<string, string>();
                int pCount = 0;
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
            if (title.Length < 20)
            {
                str.Append('.', 20 - title.Length);
            }
            string strCount = handleCount.ToString() + "/" + totalCount.ToString();
            str.Append(strCount);
            if (strCount.Length < 20)
            {
                str.Append(' ', 20 - strCount.Length);
            }
            double rate = handleCount * 100.0 / totalCount;
            str.AppendLine(" (" + rate.ToString("0.00") + "%)");
        }

        internal class IDGenerator
        {
            public IDGenerator(string strPreFix, int len = 8)
            {
                this.Length = len;
                this._Indexs = new int[len];
                this._CharsLength = _Chars.Length;
                if (strPreFix != null && strPreFix.Length > 0)
                {
                    this._ClassNamePrefx = strPreFix;
                    int index = strPreFix.LastIndexOf('.');
                    if (index > 0)
                    {
                        this._MemberNamePrefix = strPreFix.Substring(index + 1);
                    }
                    else
                    {
                        this._MemberNamePrefix = strPreFix;
                    }
                }
                Reset();
            }
            public void Reset()
            {
                for (int iCount = 0; iCount < this._Indexs.Length; iCount++)
                {
                    this._Indexs[iCount] = _rnd.Next(0, _Chars.Length - 1);
                }
                this._Indexs[0] = 0;
                this._Indexs[1] = 0;
                this.GenCount = _rnd.Next(10, 100);
            }
            private System.Random _rnd = new Random();
            private static readonly string _Chars = "lkjhgfdsaqwertyuiopmnbvcxz";//"mn0O1l";
            private readonly int _CharsLength = 0;
            private readonly string _ClassNamePrefx = null;
            private readonly string _MemberNamePrefix = null;

            private int[] _Indexs = null;
            public readonly int Length = 0;
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
                string id = this.InnerGenerateID();
                if (this._MemberNamePrefix != null)
                {
                    return this._MemberNamePrefix + id;
                }
                else
                {
                    return id;
                }
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
                for( int iCount = 0; iCount < 20; iCount ++  )
                {
                    int index = gc % this._CharsLength;
                    _ResultBuffer[iCount] = _Chars[index];
                    gc = (gc - index) / this._CharsLength;
                    if( gc == 0 )
                    {
                        return new string(this._ResultBuffer, 0, iCount+1);
                    }
                }
                return null;

                //var chrs = new char[this._Indexs.Length];
                //for (int iCount = this._Indexs.Length - 1; iCount >= 0; iCount--)
                //{
                //    int v = this._Indexs[iCount] + 1;
                //    if (v >= this._CharsLength)
                //    {
                //        //v = 0;
                //        this._Indexs[iCount] = 0;
                //    }
                //    else
                //    {
                //        this._Indexs[iCount] = v;
                //        break;
                //    }
                //}
                //for (int iCount = this._Indexs.Length - 1; iCount >= 0; iCount--)
                //{
                //    chrs[iCount] = _Chars[this._Indexs[iCount]];
                //}
                //this.GenCount++;
                //return new string(chrs);
            }
        }

        private Dictionary<DCILClass, List<DCILTypeReference>> _AllBaseTypes = new Dictionary<DCILClass, List<DCILTypeReference>>();
        private List<DCILTypeReference> GetAllBaseType(DCILClass start)
        {
            List<DCILTypeReference> list = null;
            if (_AllBaseTypes.TryGetValue(start, out list))
            {
                return list;
            }
            list = new List<DCILTypeReference>();
            InnerGetAllBaseType(start, list);
            _AllBaseTypes[start] = list;
            return list;
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
        private IDGenerator _IDGenForClass = null;

        private void PreserveMethodName(DCILClass cls, DCILMethod method)
        {
            // 重载了外部类型的成员方法，方法不重命名。
            method.RenameState = DCILRenameState.Preserve;
            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
            {
                foreach (var item2 in cls.ChildNodes)
                {
                    if (item2 is DCILProperty)
                    {
                        var p2 = (DCILProperty)item2;
                        if (p2.Method_Get?.LocalMethod == method || p2.Method_Set?.LocalMethod == method)
                        {
                            p2.RenameState = DCILRenameState.Preserve;
                            break;
                        }
                    }
                }
            }
            else if (method.Name.StartsWith("add_") || method.Name.StartsWith("remove_"))
            {
                foreach (var item2 in cls.ChildNodes)
                {
                    if (item2 is DCILEvent)
                    {
                        var p2 = (DCILEvent)item2;
                        if (p2.Method_Addon?.LocalMethod == method || p2.Method_Removeon?.LocalMethod == method)
                        {
                            p2.RenameState = DCILRenameState.Preserve;
                            break;
                        }
                    }
                }
            }
        }
        private int _ModifiedCount = 0;
        private static readonly string _OptionPrefix = "JIEJIE.NET:";
        /// <summary>
        /// 是否处于调试模式
        /// </summary>
        public bool DebugMode = false;
        public JieJieSwitchs Switchs = new JieJieSwitchs();

        private void HandleMethod(DCILMethod method, JieJieSwitchs parentOptions)
        {
            var opts = GetRuntimeSwitchs(method, parentOptions);
            if (opts.AllocationCallStack && method.ReturnTypeInfo == DCILTypeReference.Type_String)
            {
                for (int ilIndex = method.OperCodes.Count - 1; ilIndex >= 0; ilIndex--)
                {
                    var ilcode = method.OperCodes[ilIndex];
                    if (ilcode.OperCode == "ret")
                    {
                        method.ILCodesModified = true;
                        method.OperCodes.Insert(ilIndex, new DCILOperCode("IL_zzzzz", "call", "string DCSoft.Common.InnerAssemblyHelper20210315::CloneStringCrossThead(string)"));
                        _ModifiedCount++;
                        break;
                    }
                }
            }
            if (opts.ControlFlow)
            {
                ObfuscateOperCodeList(method);
            }
        }

        private void HandleClass(DCILClass cls, JieJieSwitchs parentOptions)
        {
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
                if (index == 398)
                {

                }
                return FullClassName + "::_" + index;
            }
            public bool HasData()
            {
                return _Datas.Count > 0;
            }
            private List<byte[]> _Datas = new List<byte[]>();
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

            public void WriteTo(DCILDocument document)
            {
                var str = new StringBuilder();
                var LibName_mscorlib = document.LibName_mscorlib;
                str.AppendLine(".class private auto ansi abstract sealed beforefieldinit " + FullClassName + " extends[" + LibName_mscorlib + "]System.Object");
                str.AppendLine("{");
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    str.AppendLine(@".class nested private explicit ansi sealed _DATA" + iCount + @" extends[mscorlib]System.ValueType
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
                    int labelIndex = 10;
                    foreach (var index in _FieldIndexs)
                    {
                        labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": ldc.i4 " + _Datas[index].Length);
                        labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": newarr [" + LibName_mscorlib + "]System.Byte");
                        labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": dup");
                        labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": ldtoken field valuetype " + FullClassName + @"/_DATA" + index + " " + FullClassName + @"::_Field" + index);
                        labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)");
                        labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": stsfld uint8[] " + FullClassName + "::_" + index);
                    }
                    labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": ret");
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
	// Method begins at RVA 0x2b64
	// Code size 23 (0x17)
	.maxstack 3
	.locals init (
		[0] uint8[]
	)
	IL_0000: nop
	IL_0001: ldc.i4 " + _Datas[iCount].Length + @"
	IL_0002: newarr [" + LibName_mscorlib + @"]System.Byte
	IL_0007: dup
	IL_0008: ldtoken field valuetype " + FullClassName + @"/_DATA" + iCount + " " + FullClassName + @"::_Field" + iCount + @"
    IL_000d: call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)
	IL_0012: stloc.0
	IL_0013: br.s IL_0015
	IL_0015: ldloc.0
	IL_0016: ret
}
");
                }
                str.AppendLine("}");
                var cls = new DCILClass(str.ToString(), document);
                document.ChildNodes.Add(cls);
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    var item = new DCILData();
                    item._Name = "I_BDC" + iCount;
                    item.DataType = "bytearray";
                    item.Value = _Datas[iCount];
                    document.ChildNodes.Add(item);
                }
            }
        }

        private MyResourceDataFileList GetResourceDataFiles()
        {
            var resDataNames = new List<string>();
            foreach (var item in this.Document.ChildNodes)
            {
                if (item is DCILMResource
                    && item.Name.EndsWith(DCILDocument.EXT_resources, StringComparison.Ordinal))
                {
                    resDataNames.Add(item.Name);
                }
            }
            MyResourceDataFileList dataFiles = null;
            if (this.Document._IsDotNetCoreAssembly && typeof(string).Assembly.FullName.Contains("mscorlib"))
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "DCSoft.ResourceFileHelper.NetCore");
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }
                if (DCUtils.ExpandResourcesToPath(
                    typeof(DCProtectEngine).Assembly,
                    "DCSoft.AssemblyPublish.DCSoft.ResourceFileHelper.NetCore.",
                    tempPath,
                    true) == 0)
                {
                    DCUtils.ExpandResourcesToPath(
                    typeof(DCProtectEngine).Assembly,
                    "DCNETProtector.DCSoft.ResourceFileHelper.NetCore.",
                    tempPath,
                    true);
                }
                dataFiles = ResourceFileHelper.ExecuteByExe(
                    Path.Combine(tempPath, "DCSoft.ResourceFileHelper.NetCore.exe"),
                    this.Document.RootPath,
                    this._UILanguageName,
                    resDataNames);
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
            }
            else
            {
                dataFiles = ResourceFileHelper.Execute(this.Document.RootPath, _UILanguageName, resDataNames);// new MyResourceDataFileList(resFileNames);
            }
            return dataFiles;
        }
        public void ApplyResouceContainerClass()
        {
            int tick = Environment.TickCount;
            var cls_resIndex = new Dictionary<DCILClass, int>();
            var allRes = this.Document.GetNodeIndexs<DCILMResource>();
            var allResNames = new List<string>();
            foreach (var node in this.Document.ChildNodes)
            {
                if (node is DCILClass)
                {
                    var cls = (DCILClass)node;
                    if (cls.IsResoucePackage())
                    {
                        var resName = cls.Name + DCILDocument.EXT_resources;
                        if (allRes.ContainsKey(resName))
                        {
                            cls_resIndex[cls] = allRes[resName];
                            allResNames.Add(cls.Name);
                        }
                    }
                }
            }
            if (allResNames.Count == 0)
            {
                return;
            }
            MyResourceDataFileList dataFiles = null;
            if (this.Document._IsDotNetCoreAssembly && typeof(string).Assembly.FullName.Contains("mscorlib"))
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "DCSoft.ResourceFileHelper.NetCore");
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }
                if (DCUtils.ExpandResourcesToPath(
                    typeof(DCProtectEngine).Assembly,
                    "DCSoft.AssemblyPublish.DCSoft.ResourceFileHelper.NetCore.",
                    tempPath,
                    true) == 0)
                {
                    DCUtils.ExpandResourcesToPath(
                    typeof(DCProtectEngine).Assembly,
                    "DCNETProtector.DCSoft.ResourceFileHelper.NetCore.",
                    tempPath,
                    true);
                }
                dataFiles = ResourceFileHelper.ExecuteByExe(
                    Path.Combine(tempPath, "DCSoft.ResourceFileHelper.NetCore.exe"),
                    this.Document.RootPath,
                    this._UILanguageName,
                    allResNames);
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
            }
            else
            {
                dataFiles = ResourceFileHelper.Execute(this.Document.RootPath, this._UILanguageName, allResNames);// new MyResourceDataFileList(resFileNames);
            }
            if (dataFiles == null || dataFiles.Count == 0)
            {
                return;
            }
            var fileTable = dataFiles.GetFileTable();
            var bmpTypeName = this.Document.GetTypeNameWithLibraryName(
                "System.Drawing.Bitmap",
                typeof(System.Drawing.Bitmap).Assembly.GetName().Name);
            var removeResIndexs = new List<int>();
            foreach (var clsItem in cls_resIndex)
            {
                if (fileTable.ContainsKey(clsItem.Key.Name))
                {
                    var cls = clsItem.Key;
                    var dataFile = fileTable[cls.Name];
                    DCUtils.ObfuseListOrder(dataFile.Items);
                    var hasBmpValue = dataFile.HasBmp;
                    var strNewClassCode = new StringBuilder();
                    var clsName = cls.Name;
                    strNewClassCode.AppendLine(".class " + clsName + " extends System.Object");
                    strNewClassCode.AppendLine("{");
                    //var strDataID = AllocID();
                    strNewClassCode.AppendLine("");
                    strNewClassCode.AppendLine(".field private static initonly uint8[] _Datas");
                    if (hasBmpValue)
                    {
                        foreach (var item in dataFile.Items)
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
	// _Datas = Class3.GetData2();
	IL_0001: call uint8[] " + this._ByteDataContainer.GetMethodName(dataFile.Datas) + @"()
	IL_0006: stsfld uint8[] " + clsName + "::_Datas");

                    int labelCount = 100;
                    labelCount += 5; strNewClassCode.AppendLine("IL_" + labelCount.ToString("X4") + ": ret");
                    strNewClassCode.AppendLine("}");
                    if (hasBmpValue)
                    {
                        foreach (var item in dataFile.Items)
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
	IL_0018: ldc.i4 " + item.BsLength + @"
	IL_001d: ldc.i4 " + item.Key + @"
	IL_0022: call class " + bmpTypeName + @" DCSoft.Common.InnerAssemblyHelper20210315::GetBitmap(uint8[], int32, int32, int32)
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
                    foreach (var item in dataFile.Items)
                    {
                        if (item.IsBmp == false)
                        {
                            strNewClassCode.AppendLine(@"  .method assembly hidebysig static  string get_" + item.Name + @"() cil managed 
  {
	.maxstack 4
	.locals init (
		[0] string
	)
	IL_0000: nop
	IL_0001: ldsfld uint8[] " + clsName + @"::_Datas
	IL_0006: ldc.i4 " + item.StartIndex + @"
	IL_000b: ldc.i4 " + item.BsLength + @"
	IL_0010: ldc.i4 " + item.Key + @"
	IL_0015: call string DCSoft.Common.InnerAssemblyHelper20210315::GetString(uint8[], int32, int32, int32)
	IL_001a: stloc.0
	IL_001b: br.s IL_001d
	IL_001d: ldloc.0
	IL_001e: ret
   }");
                        }
                    }
                    strNewClassCode.AppendLine("}");
                    var strCodeText = strNewClassCode.ToString();
                    var newCls = new DCILClass(strCodeText, this.Document);
                    newCls.NewGenerate = false;
                    cls.CustomAttributes = null;
                    cls.ChildNodes = newCls.ChildNodes;
                    this._ModifiedCount++;
                    removeResIndexs.Add(clsItem.Value);
                }
            }
            if (removeResIndexs.Count > 0)
            {
                removeResIndexs.Sort();
                for (int iCount = removeResIndexs.Count - 1; iCount > 0; iCount--)
                {
                    var item2 = this.Document.ChildNodes[removeResIndexs[iCount]];
                    var fn = Path.Combine(this.Document.RootPath, item2.Name);
                    if (File.Exists(fn))
                    {
                        File.Delete(fn);
                    }
                    this.Document.ChildNodes.RemoveAt(removeResIndexs[iCount]);
                }
            }
        }


        public bool ChangeComponentResourceManager(DCILClass cls)
        {
            if (cls.BaseType != null && cls.IsInterface == false)
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod && item.Name == "InitializeComponent")
                    {
                        var method = (DCILMethod)item;
                        if (method.ReturnTypeInfo == DCILTypeReference.Type_Void)
                        {
                            if (method.Locals != null
                                && method.Locals.Count > 0
                                && method.Locals[0].ValueType.Name == "System.ComponentModel.ComponentResourceManager"
                                && method.OperCodes != null
                                && method.OperCodes.Count > 10)
                            {
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
                                            && code2.InvokeInfo.OwnerType.Name == "System.ComponentModel.ComponentResourceManager")
                                        {
                                            string resFileName = Path.Combine(this.Document.RootPath, cls.Name + DCILDocument.EXT_resources);
                                            if (File.Exists(resFileName))
                                            {
                                                var bs = File.ReadAllBytes(resFileName);
                                                var bsWrite = GetBytesForWrite(bs);// GetGZipCompressedContentIfNeed(bs);
                                                string clsName = _ClassNamePrefix + "Res" + AllocIndex();
                                                string strNewClassCode = _Code_Template_ComponentResourceManager;
                                                strNewClassCode = strNewClassCode.Replace("mscorlib", this.Document.LibName_mscorlib);
                                                strNewClassCode = strNewClassCode.Replace("#CLASSNAME#", clsName);
                                                strNewClassCode = strNewClassCode.Replace(
                                                    "[mscorlib]System.Resources.ResourceSet",
                                                    this.Document.GetTypeNameWithLibraryName("System.Resources.ResourceSet"));
                                                strNewClassCode = strNewClassCode.Replace(
                                                    "[System]System.ComponentModel.ComponentResourceManager",
                                                    code2.InvokeInfo.OwnerType.NameWithLibraryName);
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
                                                this.Document.ChildNodes.Add(resCls);
                                                _ModifiedCount++;
                                                code2.InvokeInfo = new DCILInvokeMethodInfo();
                                                code2.InvokeInfo.ReturnType = DCILTypeReference.Type_Void;
                                                code2.InvokeInfo.OwnerType = new DCILTypeReference(resCls.Name, DCILTypeMode.Class);
                                                code2.InvokeInfo.MethodName = ".ctor";
                                                code2.InvokeInfo.IsInstance = true;
                                                codes.RemoveAt(iCount + 1);
                                                codes.RemoveAt(iCount);
                                                method.Locals[0].ValueType = new DCILTypeReference(resCls.Name, DCILTypeMode.Class);
                                                int index4 = this.Document.ChildNodes.IndexOf<DCILMResource>(cls.Name + DCILDocument.EXT_resources);
                                                if (index4 >= 0)
                                                {
                                                    this.Document.ChildNodes.RemoveAt(index4);
                                                }
                                                else
                                                {

                                                }
                                                return true;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            int index5 = this.Document.ChildNodes.IndexOf<DCILMResource>(cls.Name + DCILDocument.EXT_resources);
                            if (index5 >= 0)
                            {
                                this.Document.ChildNodes.RemoveAt(index5);
                            }
                            else
                            {

                            }
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
                this._ByteDataContainer.WriteTo(this.Document);
            }
        }

        public void AddClassInnerAssemblyHelper20210315()
        {
            var code = _Code_Template_InnerAssemblyHelper20210315;
            var cls = new DCILClass(code, this.Document);
            this.Document.ChildNodes.Add(cls);
        }
        private static readonly string _Code_Template_InnerAssemblyHelper20210315 = @"
.class private auto ansi abstract sealed beforefieldinit DCSoft.Common.InnerAssemblyHelper20210315
	extends [mscorlib]System.Object
{
	.custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = (
		01 00 00 00 00
	)
	// Fields
	.field private static class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_Thread
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner
	.field private static string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_CurrentValue

	// Methods
	.method public hidebysig static 
		string CloneStringCrossThead (
			string txt
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 167 (0xa7)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] string,
			[2] class [mscorlib]System.Threading.AutoResetEvent,
			[3] bool,
			[4] bool
		)

		// (no C# code)
		IL_0000: nop
		// if (txt == null || txt.Length == 0)
		IL_0001: ldarg.0
		IL_0002: brfalse.s IL_000f

		IL_0004: ldarg.0
		IL_0005: callvirt instance int32 [mscorlib]System.String::get_Length()
		// (no C# code)
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: br.s IL_0010

		IL_000f: ldc.i4.1

		IL_0010: stloc.0
		IL_0011: ldloc.0
		IL_0012: brfalse.s IL_001c

		IL_0014: nop
		// return txt;
		IL_0015: ldarg.0
		IL_0016: stloc.1
		// (no C# code)
		IL_0017: br IL_00a5

		// lock (_CloneStringCrossThead_Event)
		IL_001c: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
		// (no C# code)
		IL_0021: stloc.2
		IL_0022: ldc.i4.0
		IL_0023: stloc.3
		.try
		{
			IL_0024: ldloc.2
			IL_0025: ldloca.s 3
			IL_0027: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
			IL_002c: nop
			IL_002d: nop
			// _CloneStringCrossThead_CurrentValue = txt;
			IL_002e: ldarg.0
			IL_002f: volatile.
			IL_0031: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
			// _CloneStringCrossThead_Event_Inner.Set();
			IL_0036: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
			IL_003b: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
			IL_0040: pop
			// _CloneStringCrossThead_Event.Reset();
			IL_0041: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
			IL_0046: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_004b: pop
			// if (_CloneStringCrossThead_Thread == null)
			IL_004c: volatile.
			IL_004e: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
			IL_0053: ldnull
			IL_0054: ceq
			IL_0056: stloc.s 4
			IL_0058: ldloc.s 4
			// (no C# code)
			IL_005a: brfalse.s IL_0083

			IL_005c: nop
			// _CloneStringCrossThead_Thread = new Thread(CloneStringCrossThead_Thread);
			IL_005d: ldnull
			IL_005e: ldftn void DCSoft.Common.InnerAssemblyHelper20210315::CloneStringCrossThead_Thread()
			IL_0064: newobj instance void [mscorlib]System.Threading.ThreadStart::.ctor(object, native int)
			IL_0069: newobj instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
			IL_006e: volatile.
			IL_0070: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
			// _CloneStringCrossThead_Thread.Start();
			IL_0075: volatile.
			IL_0077: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
			IL_007c: callvirt instance void [mscorlib]System.Threading.Thread::Start()
			// (no C# code)
			IL_0081: nop
			IL_0082: nop

			// _CloneStringCrossThead_Event.WaitOne(100);
			IL_0083: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
			IL_0088: ldc.i4.s 100
			IL_008a: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
			IL_008f: pop
			// return _CloneStringCrossThead_CurrentValue;
			IL_0090: volatile.
			IL_0092: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
			IL_0097: stloc.1
			// (no C# code)
			IL_0098: leave IL_00a5
		} // end .try
		finally
		{
			IL_009a: ldloc.3
			IL_009b: brfalse.s IL_00a4

			IL_009d: ldloc.2
			IL_009e: call void [mscorlib]System.Threading.Monitor::Exit(object)
			IL_00a3: nop

			IL_00a4: endfinally
		} // end handler

		IL_00a5: ldloc.1
		IL_00a6: ret
	} // end of method InnerAssemblyHelper20210315::CloneStringCrossThead

	.method private hidebysig static 
		void CloneStringCrossThead_Thread () cil managed 
	{
		// Method begins at RVA 0x2114
		// Code size 134 (0x86)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool,
			[2] bool
		)

		// (no C# code)
		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: br.s IL_005d
			// loop start (head: IL_005d)
				IL_0004: nop
				// while (_CloneStringCrossThead_Event_Inner.WaitOne(1000))
				IL_0005: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
				IL_000a: ldc.i4 1000
				IL_000f: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
				// (no C# code)
				IL_0014: ldc.i4.0
				IL_0015: ceq
				IL_0017: stloc.0
				IL_0018: ldloc.0
				IL_0019: brfalse.s IL_001e

				IL_001b: nop
				// }
				IL_001c: br.s IL_0061

				// if (_CloneStringCrossThead_CurrentValue != null)
				IL_001e: volatile.
				IL_0020: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
				IL_0025: ldnull
				IL_0026: cgt.un
				IL_0028: stloc.1
				IL_0029: ldloc.1
				// (no C# code)
				IL_002a: brfalse.s IL_0046

				IL_002c: nop
				// _CloneStringCrossThead_CurrentValue = new string(_CloneStringCrossThead_CurrentValue.ToCharArray());
				IL_002d: volatile.
				IL_002f: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
				IL_0034: callvirt instance char[] [mscorlib]System.String::ToCharArray()
				IL_0039: newobj instance void [mscorlib]System.String::.ctor(char[])
				IL_003e: volatile.
				IL_0040: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
				// (no C# code)
				IL_0045: nop

				// _CloneStringCrossThead_Event.Set();
				IL_0046: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
				IL_004b: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
				IL_0050: pop
				// _CloneStringCrossThead_Event_Inner.Reset();
				IL_0051: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
				IL_0056: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
				IL_005b: pop
				// (no C# code)
				IL_005c: nop

				IL_005d: ldc.i4.1
				IL_005e: stloc.2
				IL_005f: br.s IL_0004
			// end loop

			IL_0061: nop
			IL_0062: leave IL_0085
		} // end .try
		finally
		{
			IL_0064: nop
			// _CloneStringCrossThead_Thread = null;
			IL_0065: ldnull
			IL_0066: volatile.
			IL_0068: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
			// _CloneStringCrossThead_Event.Reset();
			IL_006d: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
			IL_0072: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0077: pop
			// _CloneStringCrossThead_Event_Inner.Reset();
			IL_0078: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
			IL_007d: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0082: pop
			// (no C# code)
			IL_0083: nop
			// }
			IL_0084: endfinally
		} // end handler

		// (no C# code)
		IL_0085: ret
	} // end of method InnerAssemblyHelper20210315::CloneStringCrossThead_Thread

	.method public hidebysig static 
		string GetString (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x21b8
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

		// (no C# code)
		IL_0000: nop
		// int num = bsLength / 2;
		IL_0001: ldarg.2
		IL_0002: ldc.i4.2
		IL_0003: div
		IL_0004: stloc.0
		// char[] array = new char[num];
		IL_0005: ldloc.0
		IL_0006: newarr [mscorlib]System.Char
		IL_000b: stloc.1
		// int num2 = 0;
		IL_000c: ldc.i4.0
		IL_000d: stloc.2
		// (no C# code)
		IL_000e: br.s IL_003a
		// loop start (head: IL_003a)
			IL_0010: nop
			// int num3 = startIndex + num2 * 2;
			IL_0011: ldarg.1
			IL_0012: ldloc.2
			IL_0013: ldc.i4.2
			IL_0014: mul
			IL_0015: add
			IL_0016: stloc.3
			// int num4 = (bsData[num3] << 8) + bsData[num3 + 1];
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
			// num4 ^= key;
			IL_0024: ldloc.s 4
			IL_0026: ldarg.3
			IL_0027: xor
			IL_0028: stloc.s 4
			// array[num2] = (char)num4;
			IL_002a: ldloc.1
			IL_002b: ldloc.2
			IL_002c: ldloc.s 4
			IL_002e: conv.u2
			IL_002f: stelem.i2
			// (no C# code)
			IL_0030: nop
			// num2++;
			IL_0031: ldloc.2
			IL_0032: ldc.i4.1
			IL_0033: add
			IL_0034: stloc.2
			// key++;
			IL_0035: ldarg.3
			IL_0036: ldc.i4.1
			IL_0037: add
			IL_0038: starg.s key

			// while (num2 < num)
			IL_003a: ldloc.2
			IL_003b: ldloc.0
			IL_003c: clt
			IL_003e: stloc.s 5
			IL_0040: ldloc.s 5
			IL_0042: brtrue.s IL_0010
		// end loop

		// return new string(array);
		IL_0044: ldloc.1
		IL_0045: newobj instance void [mscorlib]System.String::.ctor(char[])
		IL_004a: stloc.s 6
		// (no C# code)
		IL_004c: br.s IL_004e

		IL_004e: ldloc.s 6
		IL_0050: ret
	} // end of method InnerAssemblyHelper20210315::GetString

	.method public hidebysig static 
		class [System.Drawing]System.Drawing.Bitmap GetBitmap (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x2218
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

		// (no C# code)
		IL_0000: nop
		// byte[] array = new byte[bsLength];
		IL_0001: ldarg.2
		IL_0002: newarr [mscorlib]System.Byte
		IL_0007: stloc.0
		// int num = 0;
		IL_0008: ldc.i4.0
		IL_0009: stloc.3
		// (no C# code)
		IL_000a: br.s IL_0022
		// loop start (head: IL_0022)
			IL_000c: nop
			// array[num] = (byte)(bsData[startIndex + num] ^ key);
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
			// (no C# code)
			IL_0018: nop
			// num++;
			IL_0019: ldloc.3
			IL_001a: ldc.i4.1
			IL_001b: add
			IL_001c: stloc.3
			// key++;
			IL_001d: ldarg.3
			IL_001e: ldc.i4.1
			IL_001f: add
			IL_0020: starg.s key

			// while (num < bsLength)
			IL_0022: ldloc.3
			IL_0023: ldarg.2
			IL_0024: clt
			IL_0026: stloc.s 4
			IL_0028: ldloc.s 4
			IL_002a: brtrue.s IL_000c
		// end loop

		// MemoryStream stream = new MemoryStream(array);
		IL_002c: ldloc.0
		IL_002d: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0032: stloc.1
		// return new Bitmap(stream);
		IL_0033: ldloc.1
		IL_0034: newobj instance void [System.Drawing]System.Drawing.Bitmap::.ctor(class [mscorlib]System.IO.Stream)
		IL_0039: stloc.2
		IL_003a: ldloc.2
		IL_003b: stloc.s 5
		// (no C# code)
		IL_003d: br.s IL_003f

		IL_003f: ldloc.s 5
		IL_0041: ret
	} // end of method InnerAssemblyHelper20210315::GetBitmap

	.method public hidebysig static 
		class [mscorlib]System.Resources.ResourceSet LoadResourceSet (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x2268
		// Code size 30 (0x1e)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.IO.Stream 'stream',
			[1] class [mscorlib]System.Resources.ResourceSet result,
			[2] class [mscorlib]System.Resources.ResourceSet
		)

		// (no C# code)
		IL_0000: nop
		// Stream stream = GetStream(bs, key, gzip);
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call class [mscorlib]System.IO.Stream DCSoft.Common.InnerAssemblyHelper20210315::GetStream(uint8[], uint8, bool)
		IL_0009: stloc.0
		// ResourceSet result = new ResourceSet(stream);
		IL_000a: ldloc.0
		IL_000b: newobj instance void [mscorlib]System.Resources.ResourceSet::.ctor(class [mscorlib]System.IO.Stream)
		IL_0010: stloc.1
		// stream.Close();
		IL_0011: ldloc.0
		IL_0012: callvirt instance void [mscorlib]System.IO.Stream::Close()
		// (no C# code)
		IL_0017: nop
		// return result;
		IL_0018: ldloc.1
		IL_0019: stloc.2
		// (no C# code)
		IL_001a: br.s IL_001c

		IL_001c: ldloc.2
		IL_001d: ret
	} // end of method InnerAssemblyHelper20210315::LoadResourceSet

	.method private hidebysig static 
		class [mscorlib]System.IO.Stream GetStream (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x2294
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

		// (no C# code)
		IL_0000: nop
		// int num = bs.Length;
		IL_0001: ldarg.0
		IL_0002: ldlen
		IL_0003: conv.i4
		IL_0004: stloc.0
		// int num2 = 0;
		IL_0005: ldc.i4.0
		IL_0006: stloc.2
		// (no C# code)
		IL_0007: br.s IL_001e
		// loop start (head: IL_001e)
			IL_0009: nop
			// bs[num2] = (byte)(bs[num2] ^ key);
			IL_000a: ldarg.0
			IL_000b: ldloc.2
			IL_000c: ldarg.0
			IL_000d: ldloc.2
			IL_000e: ldelem.u1
			IL_000f: ldarg.1
			IL_0010: xor
			IL_0011: conv.u1
			IL_0012: stelem.i1
			// (no C# code)
			IL_0013: nop
			// num2++;
			IL_0014: ldloc.2
			IL_0015: ldc.i4.1
			IL_0016: add
			IL_0017: stloc.2
			// key = (byte)(key + 1);
			IL_0018: ldarg.1
			IL_0019: ldc.i4.1
			IL_001a: add
			IL_001b: conv.u1
			IL_001c: starg.s key

			// while (num2 < num)
			IL_001e: ldloc.2
			IL_001f: ldloc.0
			IL_0020: clt
			IL_0022: stloc.3
			IL_0023: ldloc.3
			IL_0024: brtrue.s IL_0009
		// end loop

		// MemoryStream memoryStream = null;
		IL_0026: ldnull
		IL_0027: stloc.1
		// if (gzip)
		IL_0028: ldarg.2
		IL_0029: stloc.s 4
		IL_002b: ldloc.s 4
		// (no C# code)
		IL_002d: brfalse.s IL_0098

		IL_002f: nop
		// GZipStream gZipStream = new GZipStream(new MemoryStream(bs), CompressionMode.Decompress);
		IL_0030: ldarg.0
		IL_0031: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0036: ldc.i4.0
		IL_0037: newobj instance void [System]System.IO.Compression.GZipStream::.ctor(class [mscorlib]System.IO.Stream, valuetype [System]System.IO.Compression.CompressionMode)
		IL_003c: stloc.s 5
		// memoryStream = new MemoryStream();
		IL_003e: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor()
		IL_0043: stloc.1
		// byte[] array = new byte[1024];
		IL_0044: ldc.i4 1024
		IL_0049: newarr [mscorlib]System.Byte
		IL_004e: stloc.s 6
		// (no C# code)
		IL_0050: br.s IL_007f
		// loop start (head: IL_007f)
			IL_0052: nop
			// num = gZipStream.Read(array, 0, array.Length);
			IL_0053: ldloc.s 5
			IL_0055: ldloc.s 6
			IL_0057: ldc.i4.0
			IL_0058: ldloc.s 6
			IL_005a: ldlen
			IL_005b: conv.i4
			IL_005c: callvirt instance int32 [mscorlib]System.IO.Stream::Read(uint8[], int32, int32)
			IL_0061: stloc.0
			// if (num > 0)
			IL_0062: ldloc.0
			IL_0063: ldc.i4.0
			IL_0064: cgt
			IL_0066: stloc.s 7
			IL_0068: ldloc.s 7
			// while (true)
			IL_006a: brfalse.s IL_007b

			// (no C# code)
			IL_006c: nop
			// memoryStream.Write(array, 0, num);
			IL_006d: ldloc.1
			IL_006e: ldloc.s 6
			IL_0070: ldc.i4.0
			IL_0071: ldloc.0
			IL_0072: callvirt instance void [mscorlib]System.IO.Stream::Write(uint8[], int32, int32)
			// (no C# code)
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

		// gZipStream.Close();
		IL_0084: ldloc.s 5
		IL_0086: callvirt instance void [mscorlib]System.IO.Stream::Close()
		// (no C# code)
		IL_008b: nop
		// memoryStream.Position = 0L;
		IL_008c: ldloc.1
		IL_008d: ldc.i4.0
		IL_008e: conv.i8
		IL_008f: callvirt instance void [mscorlib]System.IO.Stream::set_Position(int64)
		// (no C# code)
		IL_0094: nop
		IL_0095: nop
		IL_0096: br.s IL_00a1

		IL_0098: nop
		// memoryStream = new MemoryStream(bs);
		IL_0099: ldarg.0
		IL_009a: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_009f: stloc.1
		// (no C# code)
		IL_00a0: nop

		// return memoryStream;
		IL_00a1: ldloc.1
		IL_00a2: stloc.s 9
		// (no C# code)
		IL_00a4: br.s IL_00a6

		IL_00a6: ldloc.s 9
		IL_00a8: ret
	} // end of method InnerAssemblyHelper20210315::GetStream

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x2349
		// Code size 39 (0x27)
		.maxstack 8

		// _CloneStringCrossThead_Thread = null;
		IL_0000: ldnull
		IL_0001: volatile.
		IL_0003: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
		// _CloneStringCrossThead_Event = new AutoResetEvent(initialState: false);
		IL_0008: ldc.i4.0
		IL_0009: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_000e: stsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
		// _CloneStringCrossThead_Event_Inner = new AutoResetEvent(initialState: false);
		IL_0013: ldc.i4.0
		IL_0014: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_0019: stsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
		// _CloneStringCrossThead_CurrentValue = null;
		IL_001e: ldnull
		IL_001f: volatile.
		IL_0021: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
		// }
		IL_0026: ret
	} // end of method InnerAssemblyHelper20210315::.cctor

} // end of class DCSoft.Common.InnerAssemblyHelper20210315

";

        private static readonly string _Code_Template_ComponentResourceManager = @"
.class private auto ansi #CLASSNAME# extends [System]System.ComponentModel.ComponentResourceManager implements [mscorlib]System.IDisposable
{
  .field private class [mscorlib]System.Resources.ResourceSet _Data

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
	IL_0014: call class [mscorlib]System.Resources.ResourceSet DCSoft.Common.InnerAssemblyHelper20210315::LoadResourceSet(uint8[], uint8, bool)
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


        private void ObfuseClassMembers(DCILClass cls)
        {
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
        public void ObfuseClassOrder()
        {
            var list = this.Document.ChildNodes;
            int len = list.Count;
            for (int iCount = 0; iCount < len; iCount++)
            {
                var item = list[iCount];
                if (item is DCILClass)
                {
                    DCUtils.ObfuseListOrder(list, iCount, list.Count - iCount);
                    break;
                }
            }//for
        }
        private Dictionary<object, JieJieSwitchs> _RuntimeSwitchs = new Dictionary<object, JieJieSwitchs>();

        protected JieJieSwitchs GetRuntimeSwitchs(DCILClass cls, JieJieSwitchs parent)
        {
            JieJieSwitchs result = null;
            if (_RuntimeSwitchs.TryGetValue(cls, out result))
            {
                return result;
            }
            if (cls.ObfuscationSettings != null
                && cls.ObfuscationSettings.Feature != null
                && cls.ObfuscationSettings.Feature.StartsWith(_OptionPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string strSettings = cls.ObfuscationSettings.Feature.Substring(_OptionPrefix.Length);
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
                                if (cv.StartsWith(_OptionPrefix, StringComparison.OrdinalIgnoreCase))
                                {
                                    string strSettings = cv.Substring(_OptionPrefix.Length);
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

        protected JieJieSwitchs GetRuntimeSwitchs(DCILMethod method, JieJieSwitchs parent)
        {
            JieJieSwitchs result = null;
            if( _RuntimeSwitchs.TryGetValue( method , out result ))
            {
                return result;
            }
            if (method.ObfuscationSettings != null
                && method.ObfuscationSettings.Feature != null
                && method.ObfuscationSettings.Feature.StartsWith(_OptionPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string strSettings = method.ObfuscationSettings.Feature.Substring(_OptionPrefix.Length);
                result = new JieJieSwitchs(strSettings, parent);
            }
            else if (method.OperCodes != null && method.OperCodes.Count > 0)
            {
                int len = Math.Min(100, method.OperCodes.Count);
                for (int iCount = 0; iCount < len; iCount++)
                {
                    if (method.OperCodes[iCount] is DCILOperCode_LoadString)
                    {
                        var code = ((DCILOperCode_LoadString)method.OperCodes[iCount]);
                        var strCode = code.FinalValue;
                        code.FinalValue = string.Empty;
                        code.OperCode = "\"\"";
                        if (strCode != null && strCode.StartsWith(_OptionPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            string strSettings = strCode.Substring(_OptionPrefix.Length);
                            result = new JieJieSwitchs(strSettings, parent);
                            break;
                        }
                    }
                }
            }
            if( result == null )
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

        private static readonly Random _Random = new Random();

        private int _LableIDCounter = 0;
        private string CreataLableID()
        {
            _LableIDCounter++;
            return "IL_Z" + _LableIDCounter.ToString("X4");// Convert.ToString(_LableIDCounter++);
        }

        private void ChangeShortInstruction(DCILOperCodeList codes)
        {
            foreach (var item in codes)
            {
                var code = item.OperCode;
                if (code != null
                    && code.Length > 3
                    && code[code.Length - 2] == '.'
                    && code[code.Length - 1] == 's')
                {
                    if (code == "beq.s"
                        || code == "bge.s"
                        || code == "bge.un.s"
                        || code == "bgt.s"
                        || code == "bgt.un.s"
                        || code == "ble.s"
                        || code == "ble.un.s"
                        || code == "blt.s"
                        || code == "blt.un.s"
                        || code == "bne.un.s"
                        || code == "br.s"
                        || code == "brfalse.s"
                        || code == "brtrue.s"
                        || code == "leave.s")
                    {
                        item.OperCode = code.Substring(0, code.Length - 2);
                    }
                }
                else if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var tg = (DCILOperCode_Try_Catch_Finally)item;
                    if (tg.HasTryOperCodes())
                    {
                        ChangeShortInstruction(tg._Try.OperCodes);
                    }
                    if (tg._Catchs != null)
                    {
                        foreach (var citem in tg._Catchs)
                        {
                            if (citem.OperCodes != null && citem.OperCodes.Count > 0)
                            {
                                ChangeShortInstruction(citem.OperCodes);
                            }
                        }
                    }
                    if (tg.HasFinallyOperCodes())
                    {
                        ChangeShortInstruction(tg._Finally.OperCodes);
                    }
                    if (tg.HasFaultOperCodes())
                    {
                        ChangeShortInstruction(tg._fault.OperCodes);
                    }
                }
            }
        }
        public bool ObfuscateOperCodeList(DCILMethod method)
        {
            if (ObfuscateOperCodeList(method.OperCodes))
            {
                ChangeShortInstruction(method.OperCodes);
                return true;
            }
            return false;
        }
        public bool ObfuscateOperCodeList(DCILOperCodeList items, bool isInTryBlock = false)
        {
            if (items == null || items.Count == 0)
            {
                return false;
            }
            bool result = false;
            foreach (var item in items)
            {
                if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var tcf = (DCILOperCode_Try_Catch_Finally)item;
                    if (tcf._Try != null && ObfuscateOperCodeList(tcf._Try.OperCodes, true))
                    {
                        result = true;
                    }
                }
            }
            var groupMaxLen = _Random.Next(20, 50);
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
                items.RemoveAt(items.Count - 1);
            }
            var groups = new List<List<DCILOperCode>>();
            var group = new List<DCILOperCode>();
            var firstGroup = group;
            groups.Add(group);
            foreach (var item in items)
            {
                group.Add(item);
                if (item.IsPrefixOperCode() == false)// item.OperCode != "volatile." && item.OperCode != "constrained.")
                {
                    if (group.Count > groupMaxLen)
                    {
                        group = new List<DCILOperCode>();
                        groups.Add(group);
                        groupMaxLen = _Random.Next(20, 50);
                    }
                }
                //if (_Random.Next(0, 100) < 2)
                //{
                //    // 小概率插入无效跳转指令
                //    string rndLableID = items[_Random.Next(0, items.Count - 1)].LabelID;
                //    if (rndLableID != null && rndLableID.Length > 0)
                //    {
                //        //if( rndLableID == "IL_0001")
                //        //{

                //        //}
                //        group.Add(new DCILOperCodeComment("no used"));
                //        group.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
                //        group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
                //        group.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                //        group.Add(new DCILOperCode(CreataLableID(), "brtrue", rndLableID));
                //        addCount += 4;
                //    }
                //}
            }
            if (groups[groups.Count - 1].Count == 0)
            {
                groups.RemoveAt(groups.Count - 1);
            }
            for (int iCount = 0; iCount < groups.Count - 1; iCount++)
            {
                // 每条指令组后面添加跳到下一个指令组的指令
                group = groups[iCount];
                var nextGroup = groups[iCount + 1];
                group.Add(new DCILOperCodeComment("jump random"));
                var nextGroupLableID = nextGroup[0].LabelID;
                if (nextGroupLableID == null || nextGroupLableID.Length == 0)
                {
                    nextGroup.Insert(0, new DCILOperCode(CreataLableID(), "nop", null));
                    nextGroupLableID = nextGroup[0].LabelID;
                }
                group.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));
                //// 输出无效的垃圾代码
                //group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", "1"));
                ////group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", _Random.Next().ToString()));
                //group.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));

                ////group.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));

                //group.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
                //if (_Random.Next(0, 1) == 0)
                //{
                //    group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
                //    group.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                //    group.Add(new DCILOperCode(CreataLableID(), "brfalse", nextGroupLableID));
                //}
                //else
                //{
                //    group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetUnZeroIndex().ToString()));
                //    group.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                //    group.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
                //}
            }

            if (retCode != null)
            {
                groups[groups.Count - 1].Add(new DCILOperCode(CreataLableID(), "br", retCode.LabelID));
            }
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

            DCUtils.ObfuseListOrder(groups);
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
            items.Add(new DCILOperCode(CreataLableID(), "nop", null));
            //if (startGroupNone.Count > 0)
            //{
            //    var labelIDStart = CreataLableID();
            //    items.Add(new DCILOperCode(CreataLableID(), "br", labelIDStart));
            //    items.AddRange(startGroupNone);
            //    items.Add(new DCILOperCode(labelIDStart, "nop", null));
            //}
            if (firstGroup != groups[0])
            {
                var nextGroupLableID = firstGroup[0].LabelID;
                if (nextGroupLableID == null || nextGroupLableID.Length == 0)
                {

                }
                items.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));

                //items.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
                //if (_Random.Next(0, 1) == 0)
                //{
                //    items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
                //    items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                //    items.Add(new DCILOperCode(CreataLableID(), "brfalse", nextGroupLableID));
                //}
                //else
                //{
                //    items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetUnZeroIndex().ToString()));
                //    items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                //    items.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
                //}
            }
            var lastGroup = groups[groups.Count - 1];
            foreach (var group2 in groups)
            {
                foreach (var item in group2)
                {
                    items.Add(item);
                    //if ( items.Count > 10 && _Random.Next(0, 100) < 2)
                    //{
                    //    // 小概率插入无效跳转指令
                    //    string rndLableID = items[_Random.Next(0, items.Count - 1)].LabelID;
                    //    if (rndLableID != null && rndLableID.Length > 0)
                    //    {
                    //        //if( rndLableID == "IL_0001")
                    //        //{

                    //        //}
                    //        items.Add(new DCILOperCodeComment("no used"));
                    //        items.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
                    //        items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
                    //        items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                    //        items.Add(new DCILOperCode(CreataLableID(), "brtrue", rndLableID));
                    //    }
                    //}
                }
                //if( group2 == lastGroup)
                //{
                //    break;
                //}
                if (group2 == lastGroup)
                {
                    break;
                }
                if (groups.Count > 4 && _Random.Next(0, 100) < 70)
                {
                    // 指令组和指令组之间小概率插入随机指令(类似花指令)
                    items.Add(new DCILOperCode(CreataLableID(), "nop", null)); // "\"111111111222222222\""));
                    int num = _Random.Next(5, 10);
                    for (int iCount = 0; iCount < num; iCount++)
                    {
                        var g3 = groups[_Random.Next(0, groups.Count - 1)];
                        var code3 = g3[_Random.Next(0, g3.Count - 1)];
                        if (code3.IsPrefixOperCode() == false && ((code3 is DCILOperCode_Try_Catch_Finally) == false))
                        {
                            items.Add(code3.Clone(CreataLableID()));
                        }
                        //items.Add(followCodes[_Random.Next(0, followCodes.Count - 1)].Clone(CreataLableID()));
                    }
                    items.Add(new DCILOperCode(CreataLableID(), "nop", null));// "\"4444444444444455555555555555555\""));
                }
                if (isInTryBlock && group2 == lastGroup)
                {
                    items.Add(leaveCode.Clone(CreataLableID()));
                }
            }

            if (retCode != null)
            {
                var newRetlabelID = CreataLableID();
                items.Add(new DCILOperCode(retCode.LabelID, "br.s", newRetlabelID));
                items.Add(new DCILOperCode(CreataLableID(), "nop", null));
                retCode.LabelID = newRetlabelID;
                items.Add(retCode);
            }
            //ChangeJumpCode(items);

            int addCount = items.Count - oldItemsCount;
            return true;
        }
    }

    internal static class DCProtectEngine
    {
        internal static void Test()
        {
            //ExecuteAssemblyFile(
            //   @"E:\Source\DCSoft\08代码\DCSoft\DCSoft.AssemblyPublish\DCILDotfuscation\SampleWinApp\bin\Debug\SampleWinApp.exe",
            //   @"",
            //   @"E:\Source\DCSoft\08代码\DCSoft\DCSoft.AssemblyPublish\DCILDotfuscation\SampleWinApp\bin\Debug\SampleWinApp2.exe");

            //ExecuteAssemblyFile(
            //    @"D:\temp2\ilcompress\DCSoft.Writer.ForASPNETCore.dll",
            //    @"E:\Source\DCSoft\08代码\DCSoft\DCWriterForWinForm\yyf.snk",
            //    @"D:\temp2\ilcompress\DCSoft.Writer.ForASPNETCore_2.dll");
            ExecuteAssemblyFile(
                @"D:\temp2\ilcompress\DCSoft.Writer.ForWinForm.dll",
                @"E:\Source\DCSoft\08代码\DCSoft\DCWriterForWinForm\yyf.snk",
                @"E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\DCSoft.DCWriterSimpleDemo\Lib\DCSoft.Writer.ForWinForm.dll");


            //ExecuteAssemblyFile(
            //    @"D:\temp2\ilcompress\DCSoft.FW.Core.dll",
            //    @"E:\Source\DCSoft\08代码\DCSoft\DCWriterForWinForm\yyf.snk",
            //    @"D:\temp2\ilcompress\DCSoft.FW.Core_New.dll");
            Console.ReadLine();
        }


        static DCProtectEngine()
        {
            _TypeNameConvertForNetCore = new System.Collections.Generic.Dictionary<string, string>();
            _TypeNameConvertForNetCore["[mscorlib]System.IO.MemoryStream"] = "[System.Runtime.Extensions]System.IO.MemoryStream";
            _TypeNameConvertForNetCore["[System]System.IO.Compression.GZipStream"] = "[System.IO.Compression]System.IO.Compression.GZipStream";
            _TypeNameConvertForNetCore["[mscorlib]System.IO.Stream"] = "[System.Runtime]System.IO.Stream";
            _TypeNameConvertForNetCore["[System]System.IO.Compression.CompressionMode"] = "[System.IO.Compression]System.IO.Compression.CompressionMode";
            _TypeNameConvertForNetCore["[mscorlib]System.Object"] = "[System.Runtime]System.Object";
            _TypeNameConvertForNetCore["[mscorlib]System.Runtime.InteropServices.ComVisibleAttribute"] = "[System.Runtime]System.Runtime.InteropServices.ComVisibleAttribute";
            _TypeNameConvertForNetCore["[mscorlib]System.Random"] = "[System.Runtime.Extensions]System.Random";
            //_TypeNameConvertForNetCore["[mscorlib]"] = "[System.Runtime]";
            _TypeNameConvertForNetCore["[System.Drawing]System.Drawing.Bitmap"] = "[System.Drawing.Common]System.Drawing.Bitmap";
            _TypeNameConvertForNetCore["[mscorlib]System.Resources.ResourceSet"] = "[System.Resources.ResourceManager]System.Resources.ResourceSet";
            _TypeNameConvertForNetCore["[mscorlib]System.Threading.ThreadStart"] = "[mscorlib]System.Threading.ThreadStart";
            _TypeNameConvertForNetCore["[mscorlib]System.Threading.Thread"] = "[mscorlib]System.Threading.Thread";
            _TypeNameConvertForNetCore["[mscorlib]System.Runtime.CompilerServices.IsVolatile"] = "[mscorlib]System.Runtime.CompilerServices.IsVolatile";
            _TypeNameConvertForNetCore["[mscorlib]System.Threading.AutoResetEvent"] = "[mscorlib]System.Threading.AutoResetEvent";
            _TypeNameConvertForNetCore["[mscorlib]System.Threading.WaitHandle"] = "[mscorlib]System.Threading.WaitHandle";
            _TypeNameConvertForNetCore["[mscorlib]System.Threading.Monitor"] = "[mscorlib]System.Threading.Monitor";
            _TypeNameConvertForNetCore["[mscorlib]System.Threading.EventWaitHandle"] = "[mscorlib]System.Threading.EventWaitHandle";
            _TypeNameConvertForNetCore["[mscorlib]System.Threading.ThreadStart"] = "[mscorlib]System.Threading.ThreadStart";
            //_TypeNameConvertForNetCore[""] = "";
            //_TypeNameConvertForNetCore[""] = "";
            //_TypeNameConvertForNetCore[""] = "";
            //_TypeNameConvertForNetCore[""] = "";
            //_TypeNameConvertForNetCore[""] = "";
            //_TypeNameConvertForNetCore[""] = "";
            //_TypeNameConvertForNetCore[""] = "";
        }



        private static readonly Dictionary<string, string> _TypeNameConvertForNetCore = null;

        private static string TransformForDotNetCore(string code)
        {
            if (_Document._IsDotNetCoreAssembly)
            {
                var maps = _TypeNameConvertForNetCore;
                foreach (var item in maps)
                {
                    if (item.Key != item.Value)
                    {
                        code = code.Replace(item.Key, item.Value);
                    }
                }
            }
            return code;
        }
        private static readonly string _SDKDir = GetSDKDir();
        /// <summary>
        /// 获得.NET SDK安装目录
        /// </summary>
        /// <returns></returns>
        private static string GetSDKDir()
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
                        return v;
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
                        return v;
                    }
                }
            }
            foreach (var dir in new string[] {
                @"C:\Program Files (x86)\Microsoft SDKs\Windows"
             })
            {
                if (Directory.Exists(dir))
                {
                    var result = DCUtils.SearchFileDeeply(dir, "sn.exe");
                    if (result != null)
                    {
                        return Path.GetDirectoryName(result);
                    }
                }
            }
            return null;
        }

        public static JieJieSwitchs GlobalOptions = new JieJieSwitchs();

        public static bool ExecuteAssemblyFile(string asmFileName, string snkFileName, string outputFileName = null, Version fwVersion = null)
        {
            if (asmFileName == null || asmFileName.Length == 0)
            {
                throw new ArgumentNullException("asmFileName");
            }
            if (File.Exists(asmFileName) == false)
            {
                throw new FileNotFoundException(asmFileName);
            }
            var sdkDir = GetSDKDir();
            if (sdkDir == null)
            {
                throw new DirectoryNotFoundException("SDK DIR");
            }
            var ildasmExeFileName = Path.Combine(sdkDir, "ildasm.exe");
            if (File.Exists(ildasmExeFileName) == false)
            {
                throw new FileNotFoundException(ildasmExeFileName);
            }
            var ilasmExeFileName = Path.Combine(Path.GetDirectoryName(typeof(string).Assembly.Location), "ilasm.exe");
            if (File.Exists(ilasmExeFileName) == false)
            {
                throw new FileNotFoundException(ilasmExeFileName);
            }
            var tempPath = Path.Combine(Path.GetDirectoryName(asmFileName), "DCILTemp");
            if (Directory.Exists(tempPath))
            {
                DCUtils.CleanDirectory(tempPath);
            }
            else
            {
                Directory.CreateDirectory(tempPath);
            }
            try
            {
                DCUtils.CheckFileWriteable(asmFileName);

                var ilFileName = Path.Combine(tempPath, Path.GetFileName(asmFileName) + ".il");
                ResourceFileHelper.RunExe(
                    ildasmExeFileName,
                    "\"" + asmFileName + "\" /forward /UTF8 \"/output=" + ilFileName + "\"");

                string rootDir = Path.GetDirectoryName(asmFileName);
                foreach (var dir in Directory.GetDirectories(rootDir))
                {
                    // 反编译资源DLL文件

                    var resDllFileName = Path.Combine(
                            dir,
                            Path.GetFileNameWithoutExtension(asmFileName) + DCILDocument.EXT_resources + ".dll");
                    if (File.Exists(resDllFileName))
                    {
                        var tempFileName = Path.Combine(tempPath, Path.GetFileName(dir));
                        if (Directory.Exists(tempFileName) == false)
                        {
                            Directory.CreateDirectory(tempFileName);
                        }
                        tempFileName = Path.Combine(tempFileName, Path.GetFileNameWithoutExtension(resDllFileName) + ".il");
                        ResourceFileHelper.RunExe(
                            ildasmExeFileName,
                            "\"" + resDllFileName + "\" /forward /UTF8 \"/output=" + tempFileName + "\"");
                    }
                }

                if (Execute(ilFileName, ilFileName, Encoding.UTF8) == false)
                {
                    return false;
                }

                DCUtils.CheckFileWriteable(asmFileName);
                if (outputFileName == null || outputFileName.Length == 0)
                {
                    outputFileName = asmFileName;
                }
                else
                {
                    // Ensure the output directory existed.
                    var dir3 = Path.GetDirectoryName(outputFileName);
                    if (Directory.Exists(dir3) == false)
                    {
                        Directory.CreateDirectory(dir3);
                    }
                }
                string tempFileName3 = outputFileName;
                bool useTempFileName = false;
                if (File.Exists(outputFileName))
                {
                    useTempFileName = true;
                    tempFileName3 = outputFileName + ".new.dat";
                }
                else
                {
                    useTempFileName = false;
                }
                if (File.Exists(tempFileName3))
                {
                    File.Delete(tempFileName3);
                }
                var args = "\"" + ilFileName + "\" \"/output=" + tempFileName3 + "\" ";
                if (System.Diagnostics.Debugger.IsAttached == false)
                {
                    args = args + " /quiet ";
                }
                if (fwVersion != null)
                {
                    args = args + " /mdv=v" + fwVersion.ToString() + " /msv=2.0";
                }
                if (asmFileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    args = args + " /dll";
                }
                else
                {
                    args = args + " /exe";
                }
                var resFileName = Path.Combine(tempPath, Path.GetFileName(asmFileName) + ".res");
                if (File.Exists(resFileName))
                {
                    args = args + " \"/resource:" + resFileName + "\"";
                }
                ResourceFileHelper.RunExe(ilasmExeFileName, args);
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.Delete(tempPath, true);
                }
                if (useTempFileName)
                {
                    // 使用临时文件
                    if (File.Exists(tempFileName3) == false)
                    {
                        return false;
                    }
                    File.Copy(tempFileName3, outputFileName, true);
                    File.Delete(tempFileName3);
                }
                else
                {
                    // 没有临时文件
                    if (File.Exists(outputFileName) == false)
                    {
                        return false;
                    }
                }
                if (snkFileName != null && File.Exists(snkFileName))
                {
                    var snExe = Path.Combine(sdkDir, "sn.exe");
                    if (File.Exists(snExe) == false)
                    {
                        throw new FileNotFoundException(snExe);
                    }
                    ResourceFileHelper.RunExe(snExe, "-Ra \"" + outputFileName + "\" \"" + snkFileName + "\"");
                }
                return true;
            }
            finally
            {
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.Delete(tempPath, true);
                }
            }
            return false;
        }

        //private static readonly string _Namespcae = "__DC20210210";
        //private static string _ObjectTypeName = "[mscorlib]System.Object";
        private static string GetClassDefine(string clsName)
        {
            if (_Document._IsDotNetCoreAssembly)
            {
                return ".class private auto ansi " + clsName + " extends [System.Runtime]System.Object";
            }
            else
            {
                return ".class private abstract auto ansi sealed beforefieldinit " + clsName + " extends [mscorlib]System.Object";
            }
        }
        private static List<string> _NewClassCodes = new List<string>();



        private static readonly string _ClassNamePrefix = "__DC20210205._";
        private static DCILDocument _Document = null;

        /// <summary>
        /// compress string define in IL file.
        /// </summary>
        /// <param name="inputILFileName">Input IL file name</param>
        /// <param name="outputILFileName">Output IL file name</param>
        /// <returns></returns>
        public static bool Execute(string inputILFileName, string outputILFileName, Encoding txtEncoding)
        {
            if (inputILFileName == null || inputILFileName.Length == 0)
            {
                throw new ArgumentNullException("fileName");
            }
            if (File.Exists(inputILFileName) == false)
            {
                throw new FileNotFoundException(inputILFileName);
            }
            if (outputILFileName == null || outputILFileName.Length == 0)
            {
                outputILFileName = inputILFileName;
            }
            _ModifiedCount = 0;
            int tickStart = Environment.TickCount;
            if (txtEncoding == null)
            {
                txtEncoding = Encoding.Default;
            }
            ByteArrayDataContainer.FullClassName = _ClassNamePrefix + "BytesContainer__";

            Console.WriteLine("Analysing " + inputILFileName + " ...");
            _Document = new DCILDocument(inputILFileName, txtEncoding, GlobalOptions);
            if (_Document.StringDefines.Count == 0)
            {
                Console.WriteLine("To this assembly, I can not do anything.");
                return false;
            }
            //_Document.StringDefines.Clear();

            //var sourceLines = new List<string>(File.ReadAllLines(inputILFileName, Encoding.Default));
            var inputFileSize = new FileInfo(inputILFileName).Length;
            _NewClassCodes = new List<string>();
            // 设置字符串资源
            SelectUILanguage();
            RemoveInnerProperty();
            var nativeStringDefine = _Document.StringDefines.Count;

            // 进行项目合并
            var strValues = new Dictionary<string, List<DCILOperCode_LoadString>>();
            foreach (var item in _Document.StringDefines)
            {
                List<DCILOperCode_LoadString> list = null;
                if (strValues.TryGetValue(item.FinalValue, out list) == false)
                {
                    list = new List<DCILOperCode_LoadString>();
                    strValues[item.FinalValue] = list;
                }
                list.Add(item);
            }
            // 删除一些项目
            var strList = new List<string>(strValues.Keys);
            int stringGroupSize = 50;

            var rnd = new System.Random();

            // 建立字符串值索引号
            strList = new List<string>(strValues.Keys);
            // 混淆字符串次序
            DCUtils.ObfuseListOrder(strList);
            var valueIndexs = new Dictionary<string, int>();
            for (int iCount = 0; iCount < strList.Count; iCount++)
            {
                valueIndexs[strList[iCount]] = iCount;
            }
            var sourceLines = _Document.SourceLines;
            foreach (var items in strValues.Values)
            {
                foreach (var item in items)
                {
                    int valueIndex = valueIndexs[item.FinalValue];// strList.BinarySearch(item.Value);// .IndexOf(item.Value);
                    var newILCode = new DCILOperCode(
                        item.LabelID,
                        "ldsfld",
                        "string " + _ClassNamePrefix + Convert.ToString(valueIndex / stringGroupSize)
                            + "::_" + valueIndex.ToString());
                    item.ReplaceCode = newILCode;
                    item.OwnerMethod.ILCodesModified = true;
                    //var strNewLine = new StringBuilder();
                    //strNewLine.Append("    " + item.LabelID + ": ldsfld string ");
                    //strNewLine.Append(_ClassNamePrefix + Convert.ToString(valueIndex / stringGroupSize)
                    //    + "::_" + valueIndex.ToString());
                    //sourceLines[item.LineIndex] = strNewLine.ToString();
                    if (item.EndLineIndex > item.LineIndex)
                    {
                        for (int iCount2 = item.LineIndex; iCount2 <= item.EndLineIndex; iCount2++)
                        {
                            sourceLines[iCount2] = string.Empty;
                        }
                    }
                    _ModifiedCount++;
                }

                foreach (var item in items)
                {
                    int valueIndex = valueIndexs[item.FinalValue];// strList.BinarySearch(item.Value);// .IndexOf(item.Value);
                    var strNewLine = new StringBuilder();
                    strNewLine.Append("    " + item.LabelID + ": ldsfld string ");
                    strNewLine.Append(_ClassNamePrefix + Convert.ToString(valueIndex / stringGroupSize)
                        + "::_" + valueIndex.ToString());
                    sourceLines[item.LineIndex] = strNewLine.ToString();
                    if (item.EndLineIndex > item.LineIndex)
                    {
                        for (int iCount = item.LineIndex + 1; iCount <= item.EndLineIndex; iCount++)
                        {
                            sourceLines[iCount] = string.Empty;
                        }
                    }
                    _ModifiedCount++;
                }
            }


            // 创建新类型
            int clsNum = strList.Count / stringGroupSize;
            for (int iCount = 0; iCount <= clsNum; iCount++)
            {
                var strNewClassCode = new StringBuilder();
                string clsName = _ClassNamePrefix + Convert.ToString(iCount);
                strNewClassCode.AppendLine(GetClassDefine(clsName));
                //if (_IsDotNetCoreAssembly)
                //{
                //    strNewClassCode.AppendLine(".class private auto ansi " + clsName + " extends [System.Runtime]System.Object");
                //}
                //else
                //{
                //    strNewClassCode.AppendLine(".class private abstract auto ansi sealed beforefieldinit " + clsName + " extends [mscorlib]System.Object");
                //}
                strNewClassCode.AppendLine("{");
                int startPostion = strNewClassCode.Length;
                var lstItems = new StringValueContainer();
                lstItems.LibName_mscorlib = _Document.LibName_mscorlib;
                for (int iCount2 = 0; iCount2 < stringGroupSize; iCount2++)
                {
                    var valueIndex = iCount * stringGroupSize + iCount2;
                    if (valueIndex < strList.Count)
                    {
                        string strText = strList[valueIndex];
                        lstItems.AddItem(valueIndex, strText);

                        strNewClassCode.AppendLine("    .field public static initonly string _" + valueIndex.ToString());
                    }
                }
                if (lstItems.Items.Count == 0)
                {
                    continue;
                }
                lstItems.RefreshState();
                //var dataID = AllocID();

                strNewClassCode.AppendLine(@"
.method private hidebysig specialname rtspecialname static void .cctor () cil managed 
{
	.maxstack 3
	.locals init (
		[0] uint8[] datas
	)
	// (no C# code)
	IL_0000: nop
	IL_0001: call uint8[] " + ByteArrayDataContainer.GetMethodName(lstItems.Datas) + @"()
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
                strNewClassCode.Replace("[mscorlib]", '[' + _Document.LibName_mscorlib + ']');
                //strNewClassCode.AppendLine("}// end of class " + clsName);
                var txtNewClassCodeText = strNewClassCode.ToString();
                _NewClassCodes.Add(txtNewClassCodeText);
            }//for

            var strCodeInnerAssemblyHelper20210315 = _Code_Template_InnerAssemblyHelper20210315;
            strCodeInnerAssemblyHelper20210315 = strCodeInnerAssemblyHelper20210315.Replace("mscorlib", _Document.LibName_mscorlib);
            _NewClassCodes.Add(strCodeInnerAssemblyHelper20210315);
            if (_Document.SecurityMethods != null && _Document.SecurityMethods.Count > 0)
            {
                foreach (var methodInfo in _Document.SecurityMethods)
                {
                    var method = methodInfo.Item1;
                    for (int ilIndex = method.OperCodes.Count - 1; ilIndex >= 0; ilIndex--)
                    {
                        var ilcode = method.OperCodes[ilIndex];
                        if (ilcode.OperCode == "ret")
                        {
                            method.ILCodesModified = true;
                            method.OperCodes.Insert(ilIndex, new DCILOperCode("IL_zzzzz", "call", "string DCSoft.Common.InnerAssemblyHelper20210315::CloneStringCrossThead(string)"));
                            _ModifiedCount++;
                            break;
                        }
                    }
                    //for (int iCount9 = method.EndLineIndex; iCount9 >= method.StartLineIndex; iCount9--)
                    //{
                    //    var line9 = _Document.SourceLines[iCount9];
                    //    if (line9.Contains("ret"))
                    //    {
                    //        string labelID = null;
                    //        string operData = null;
                    //        var operCode = DCILOperCode.GetILCode(line9, ref labelID, ref operData);
                    //        if (operCode == "ret")
                    //        {
                    //            line9 = "IL_zzzzz: call string DCSoft.Common.InnerAssemblyHelper20210315::CloneStringCrossThead(string)\r\n" + line9;
                    //            _Document.SourceLines[iCount9] = line9;
                    //            _ModifiedCount++;
                    //            break;
                    //        }
                    //    }
                    //}
                }
            }
            if (_Document.ResouceFiles.Count > 0)
            {
                ApplyResouceContainerClass();
                ApplyComponentResourceManagers();

            }
            ObfuscateFlowEngine.ObfuscateAndUpdateMethodILCode(_Document);
            if (ByteArrayDataContainer.HasData())
            {
                var strDatas = new StringBuilder();
                ByteArrayDataContainer.WriteClassSourceCode(strDatas);
                var txt9 = strDatas.ToString();
                _NewClassCodes.Add(txt9);
            }

            // 混淆类型中的字段顺序
            foreach (var cls in _Document.AllClasses)
            {
                if (cls.FieldLineIndexs != null && cls.FieldLineIndexs.Count > 2)
                {
                    var list2 = new List<string>();
                    foreach (var index in cls.FieldLineIndexs)
                    {
                        list2.Add(_Document.SourceLines[index]);
                    }
                    DCUtils.ObfuseListOrder(list2);
                    for (int iCount = 0; iCount < list2.Count; iCount++)
                    {
                        _Document.SourceLines[cls.FieldLineIndexs[iCount]] = list2[iCount];
                    }
                    _ModifiedCount += list2.Count;
                }
                var methods = new List<DCILMethod>();
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        methods.Add((DCILMethod)item);
                    }
                }
                if (methods.Count > 2)
                {
                    // 混淆类型成员方法的顺序
                    DCUtils.ObfuseListOrder(methods);
                    var strCode = new StringBuilder();
                    foreach (var method in methods)
                    {
                        for (int iCount = method.StartLineIndex; iCount <= method.EndLineIndex; iCount++)
                        {
                            strCode.AppendLine(_Document.SourceLines[iCount]);
                            _Document.SourceLines[iCount] = string.Empty;
                        }
                    }
                    _Document.SourceLines[methods[0].StartLineIndex] = strCode.ToString();
                    _ModifiedCount += methods.Count;
                }
            }
            if (_ModifiedCount > 0)
            {
                int classIndex = 0;
                using (var writer = new System.IO.StreamWriter(outputILFileName, false, txtEncoding))
                {
                    var lineNum = _Document.SourceLines.Length;
                    for (int iCount = 0; iCount < lineNum; iCount++)
                    {
                        string line = _Document.SourceLines[iCount];
                        if (line == null || line.Length == 0)
                        {
                            continue;
                        }
                        if (_NewClassCodes.Count > 0 && line.StartsWith(".class", StringComparison.Ordinal))
                        {
                            classIndex++;
                            if ((classIndex % 27) == 0)
                            {
                                writer.WriteLine(_NewClassCodes[_NewClassCodes.Count - 1]);
                                _NewClassCodes.RemoveAt(_NewClassCodes.Count - 1);
                            }
                        }
                        writer.WriteLine(line);
                    }
                    if (_NewClassCodes.Count > 0)
                    {
                        foreach (var item in _NewClassCodes)
                        {
                            writer.WriteLine(item);
                        }
                    }

                    ByteArrayDataContainer.WriteDataCode(writer);

                }
                //File.WriteAllLines(outputILFileName, sourceLines.ToArray(), Encoding.Default);
                tickStart = Math.Abs(Environment.TickCount - tickStart);
                Console.WriteLine("Input assembly file:" + inputILFileName + " ("
                    + DCUtils.FormatByteSize(inputFileSize) + ")," + _Document.SourceLines.Length + " lines IL code.");
                Console.WriteLine("Ouput assembly file:" + outputILFileName
                    + " (" + DCUtils.FormatByteSize(new FileInfo(outputILFileName).Length) + ")");
                var totalStrLength = 0;
                var utf8 = Encoding.UTF8;
                var byteTotalLength = 0;
                foreach (var item in _Document.StringDefines)
                {
                    totalStrLength += item.FinalValue.Length;
                    byteTotalLength += utf8.GetByteCount(item.FinalValue);
                }
                Console.WriteLine("Define :" + _Document.StringDefines.Count
                    + " string values ,total length:" + totalStrLength + "(" + DCUtils.FormatByteSize(byteTotalLength) + ")");
                totalStrLength = 0;
                byteTotalLength = 0;
                foreach (var item in strValues.Keys)
                {
                    totalStrLength += item.Length;
                    byteTotalLength += utf8.GetByteCount(item);
                }
                Console.WriteLine("Compress to :" + strValues.Count
                    + " string values , total length:" + totalStrLength + "(" + DCUtils.FormatByteSize(byteTotalLength) + ")");
                Console.WriteLine("IL modified  :" + _ModifiedCount);
                Console.WriteLine("Time span :" + tickStart + " milliseconds.");
                return true;
            }
            else
            {
                Console.WriteLine("No change " + inputILFileName);
                return false;
            }
        }

        private static class ObfuscateFlowEngine
        {
            private static int _LableIDCounter = 0;
            private static string CreataLableID()
            {
                _LableIDCounter++;
                return "IL_Z" + _LableIDCounter.ToString("X4");// Convert.ToString(_LableIDCounter++);
            }
            public static void ObfuscateAndUpdateMethodILCode(DCILDocument document)
            {
                if (document.ProtectedOptions.ControlFlow == false)
                {
                    return;
                }
                foreach (var cls in document.AllClasses)
                {
                    foreach (var cld in cls.ChildNodes)
                    {
                        if (cld is DCILMethod)
                        {
                            DCILMethod method = (DCILMethod)cld;

                            if (method.ProtectedOptions != null && method.ProtectedOptions.ControlFlow == false)
                            {
                                continue;
                            }
                            //if(method.InstanceIndex ==8809)
                            //{

                            //}
                            //if (method.Name == "GetFontDataUseWin32API")
                            //{

                            //}
                            _LableIDCounter = 0;
                            if (Obfuscate(method.OperCodes))
                            {
                                method.ILCodesModified = true;
                            }
                            if (method.ILCodesModified)
                            {
                                for (int iCount = method.OperCodes.StartLineIndex; iCount < method.EndLineIndex; iCount++)
                                {
                                    document.SourceLines[iCount] = string.Empty;
                                }
                                var strCode = new System.Text.StringBuilder();
                                method.OperCodes.WriteTo(new DCILWriter(strCode));
                                document.SourceLines[method.OperCodes.StartLineIndex] = strCode.ToString();
                                for (int iCount = 0; iCount < 10; iCount++)
                                {
                                    var line = document.SourceLines[method.StartLineIndex + iCount].Trim();
                                    if (line.StartsWith(".maxstack"))
                                    {
                                        var v = Convert.ToInt32(line.Substring(9).Trim());
                                        document.SourceLines[method.StartLineIndex + iCount] = "   .maxstack " + Convert.ToString(v + 4);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private static bool Obfuscate(DCILOperCodeList items)
            {
                //return false;

                if (items == null || items.Count == 0)
                {
                    return false;
                }
                bool result = false;
                foreach (var item in items)
                {
                    if (item is DCILOperCode_Try_Catch_Finally)
                    {
                        var tcf = (DCILOperCode_Try_Catch_Finally)item;
                        if (tcf._Try != null && Obfuscate(tcf._Try.OperCodes))
                        {
                            result = true;
                        }
                    }
                }
                var groupMaxLen = _Random.Next(20, 50);
                if (groupMaxLen >= items.Count)
                {
                    // 指令数量太少，不管了。
                    if (result)
                    {
                        ChangeJumpCode(items);
                    }
                    return result;
                }
                int oldItemsCount = items.Count;
                DCILOperCode retCode = null;
                if (items[items.Count - 1].OperCode == "ret")
                {
                    retCode = items[items.Count - 1];
                    items.RemoveAt(items.Count - 1);
                }
                var groups = new List<List<DCILOperCode>>();
                var group = new List<DCILOperCode>();
                var firstGroup = group;
                groups.Add(group);
                foreach (var item in items)
                {
                    group.Add(item);
                    if (item.OperCode != "volatile." && item.OperCode != "constrained.")
                    {
                        if (group.Count > groupMaxLen)
                        {
                            group = new List<DCILOperCode>();
                            groups.Add(group);
                            groupMaxLen = _Random.Next(20, 50);
                            //if (item is DCILOperCode_Try_Catch_Finally)
                            //{
                            //    group.Add(new DCILOperCode(CreataLableID(), "nop", null));
                            //}
                        }
                    }
                    //if (_Random.Next(0, 100) < 2)
                    //{
                    //    // 小概率插入无效跳转指令
                    //    string rndLableID = items[_Random.Next(0, items.Count - 1)].LabelID;
                    //    if (rndLableID != null && rndLableID.Length > 0)
                    //    {
                    //        //if( rndLableID == "IL_0001")
                    //        //{

                    //        //}
                    //        group.Add(new DCILOperCodeComment("no used"));
                    //        group.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
                    //        group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
                    //        group.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                    //        group.Add(new DCILOperCode(CreataLableID(), "brtrue", rndLableID));
                    //        addCount += 4;
                    //    }
                    //}
                }
                if (groups[groups.Count - 1].Count == 0)
                {
                    groups.RemoveAt(groups.Count - 1);
                }
                for (int iCount = 0; iCount < groups.Count - 1; iCount++)
                {
                    // 每条指令组后面添加跳到下一个指令组的指令
                    group = groups[iCount];
                    var nextGroup = groups[iCount + 1];
                    group.Add(new DCILOperCodeComment("jump random"));
                    var nextGroupLableID = nextGroup[0].LabelID;
                    if (nextGroupLableID == null || nextGroupLableID.Length == 0)
                    {
                        nextGroup.Insert(0, new DCILOperCode(CreataLableID(), "nop", null));
                        nextGroupLableID = nextGroup[0].LabelID;
                    }
                    group.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));
                    //// 输出无效的垃圾代码
                    //group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", "1"));
                    ////group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", _Random.Next().ToString()));
                    //group.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));

                    ////group.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));

                    //group.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
                    //if (_Random.Next(0, 1) == 0)
                    //{
                    //    group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
                    //    group.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                    //    group.Add(new DCILOperCode(CreataLableID(), "brfalse", nextGroupLableID));
                    //}
                    //else
                    //{
                    //    group.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetUnZeroIndex().ToString()));
                    //    group.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                    //    group.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
                    //}
                }

                if (retCode != null)
                {
                    groups[groups.Count - 1].Add(new DCILOperCode(CreataLableID(), "br", retCode.LabelID));
                }
                //var lastCode = new DCILOperCode(CreataLableID(), "nop", null);
                //var lastGroup = groups[groups.Count - 1];
                //lastGroup.Add(new DCILOperCode(CreataLableID(), "br", lastCode.LabelID));
                DCUtils.ObfuseListOrder(groups);
                items.Clear();
                items.Add(new DCILOperCode(CreataLableID(), "nop", null));
                if (firstGroup != groups[0])
                {
                    var nextGroupLableID = firstGroup[0].LabelID;
                    if (nextGroupLableID == null || nextGroupLableID.Length == 0)
                    {

                    }
                    items.Add(new DCILOperCode(CreataLableID(), "br", nextGroupLableID));

                    //items.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
                    //if (_Random.Next(0, 1) == 0)
                    //{
                    //    items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
                    //    items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                    //    items.Add(new DCILOperCode(CreataLableID(), "brfalse", nextGroupLableID));
                    //}
                    //else
                    //{
                    //    items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetUnZeroIndex().ToString()));
                    //    items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                    //    items.Add(new DCILOperCode(CreataLableID(), "brtrue", nextGroupLableID));
                    //}
                }
                foreach (var group2 in groups)
                {
                    foreach (var item in group2)
                    {
                        items.Add(item);
                        //if ( items.Count > 10 && _Random.Next(0, 100) < 2)
                        //{
                        //    // 小概率插入无效跳转指令
                        //    string rndLableID = items[_Random.Next(0, items.Count - 1)].LabelID;
                        //    if (rndLableID != null && rndLableID.Length > 0)
                        //    {
                        //        //if( rndLableID == "IL_0001")
                        //        //{

                        //        //}
                        //        items.Add(new DCILOperCodeComment("no used"));
                        //        items.Add(new DCILOperCode(CreataLableID(), "ldsfld", "uint8[] " + FieldName));
                        //        items.Add(new DCILOperCode(CreataLableID(), "ldc.i4", GetZeroIndex().ToString()));
                        //        items.Add(new DCILOperCode(CreataLableID(), "ldelem.u1", null));
                        //        items.Add(new DCILOperCode(CreataLableID(), "brtrue", rndLableID));
                        //    }
                        //}
                    }
                }

                if (retCode != null)
                {
                    var newRetlabelID = CreataLableID();
                    items.Add(new DCILOperCode(retCode.LabelID, "br.s", newRetlabelID));
                    items.Add(new DCILOperCode(CreataLableID(), "nop", null));
                    retCode.LabelID = newRetlabelID;
                    items.Add(retCode);
                }
                ChangeJumpCode(items);
                int addCount = items.Count - oldItemsCount;
                return true;
            }

            private static void ChangeJumpCode(DCILOperCodeList list)
            {
                foreach (var item in list)
                {
                    var code = item.OperCode;
                    if (code != null
                        &&
                        code.Length > 3
                        && code[code.Length - 2] == '.'
                        && code[code.Length - 1] == 's')
                    {
                        if (code == "beq.s"
                            || code == "bge.s"
                            || code == "bge.un.s"
                            || code == "bgt.s"
                            || code == "bgt.un.s"
                            || code == "ble.s"
                            || code == "ble.un.s"
                            || code == "blt.s"
                            || code == "blt.un.s"
                            || code == "bne.un.s"
                            || code == "br.s"
                            || code == "brfalse.s"
                            || code == "brtrue.s")
                        {
                            item.OperCode = code.Substring(0, code.Length - 2);
                        }
                    }
                }
            }

            public static readonly string FieldName = null;
            private static readonly byte[] _Values = null;
            private static readonly int[] ZeroIndexs = null;
            private static readonly int[] UnZeroIndexs = null;

            private static readonly Random _Random = new Random();
            static ObfuscateFlowEngine()
            {
                _Values = new byte[_Random.Next(512, 1024)];
                _Random.NextBytes(_Values);
                var indexs2 = new List<int>();
                var indexs4 = new List<int>();
                for (int iCount = 0; iCount < _Values.Length; iCount++)
                {
                    if (_Random.Next(1, 100) > 60)
                    {
                        _Values[iCount] = 0;
                    }
                    if (_Values[iCount] == 0)
                    {
                        indexs2.Add(iCount);
                    }
                    else
                    {
                        indexs4.Add(iCount);
                    }
                }
                ZeroIndexs = indexs2.ToArray();
                UnZeroIndexs = indexs4.ToArray();
                FieldName = ByteArrayDataContainer.GetFieldName(_Values);
            }
            public static int GetZeroIndex()
            {
                return ZeroIndexs[_Random.Next(0, ZeroIndexs.Length - 1)];
            }
            public static int GetUnZeroIndex()
            {
                return UnZeroIndexs[_Random.Next(0, UnZeroIndexs.Length - 1)];
            }
        }

        public static string _UILanguage = null;
        public static string _UILanguageDisplayName = null;

        private static void SelectUILanguage()
        {
            _UILanguage = null;
            var allResFiles = _Document.ResouceFiles;
            if (allResFiles.Count > 0)
            {
                var culs = _Document.GetSupportCultures();
                if (culs != null && culs.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please select UI language you want:");
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (int iCount = 0; iCount < culs.Length; iCount++)
                    {
                        Console.WriteLine(iCount + ":" + culs[iCount].Name + " " + culs[iCount].DisplayName);
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Please input index,press enter to use default,");
                    Console.ResetColor();

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
                                _UILanguage = culs[index].Name;
                                _UILanguageDisplayName = culs[index].DisplayName;
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
        private static readonly string _Code_Template_InnerAssemblyHelper20210315 = @"
.class private auto ansi abstract sealed beforefieldinit DCSoft.Common.InnerAssemblyHelper20210315
	extends [mscorlib]System.Object
{
	.custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = (
		01 00 00 00 00
	)
	// Fields
	.field private static class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_Thread
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner
	.field private static string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_CurrentValue

	// Methods
	.method public hidebysig static 
		string CloneStringCrossThead (
			string txt
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 167 (0xa7)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] string,
			[2] class [mscorlib]System.Threading.AutoResetEvent,
			[3] bool,
			[4] bool
		)

		// (no C# code)
		IL_0000: nop
		// if (txt == null || txt.Length == 0)
		IL_0001: ldarg.0
		IL_0002: brfalse.s IL_000f

		IL_0004: ldarg.0
		IL_0005: callvirt instance int32 [mscorlib]System.String::get_Length()
		// (no C# code)
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: br.s IL_0010

		IL_000f: ldc.i4.1

		IL_0010: stloc.0
		IL_0011: ldloc.0
		IL_0012: brfalse.s IL_001c

		IL_0014: nop
		// return txt;
		IL_0015: ldarg.0
		IL_0016: stloc.1
		// (no C# code)
		IL_0017: br IL_00a5

		// lock (_CloneStringCrossThead_Event)
		IL_001c: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
		// (no C# code)
		IL_0021: stloc.2
		IL_0022: ldc.i4.0
		IL_0023: stloc.3
		.try
		{
			IL_0024: ldloc.2
			IL_0025: ldloca.s 3
			IL_0027: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
			IL_002c: nop
			IL_002d: nop
			// _CloneStringCrossThead_CurrentValue = txt;
			IL_002e: ldarg.0
			IL_002f: volatile.
			IL_0031: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
			// _CloneStringCrossThead_Event_Inner.Set();
			IL_0036: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
			IL_003b: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
			IL_0040: pop
			// _CloneStringCrossThead_Event.Reset();
			IL_0041: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
			IL_0046: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_004b: pop
			// if (_CloneStringCrossThead_Thread == null)
			IL_004c: volatile.
			IL_004e: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
			IL_0053: ldnull
			IL_0054: ceq
			IL_0056: stloc.s 4
			IL_0058: ldloc.s 4
			// (no C# code)
			IL_005a: brfalse.s IL_0083

			IL_005c: nop
			// _CloneStringCrossThead_Thread = new Thread(CloneStringCrossThead_Thread);
			IL_005d: ldnull
			IL_005e: ldftn void DCSoft.Common.InnerAssemblyHelper20210315::CloneStringCrossThead_Thread()
			IL_0064: newobj instance void [mscorlib]System.Threading.ThreadStart::.ctor(object, native int)
			IL_0069: newobj instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
			IL_006e: volatile.
			IL_0070: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
			// _CloneStringCrossThead_Thread.Start();
			IL_0075: volatile.
			IL_0077: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
			IL_007c: callvirt instance void [mscorlib]System.Threading.Thread::Start()
			// (no C# code)
			IL_0081: nop
			IL_0082: nop

			// _CloneStringCrossThead_Event.WaitOne(100);
			IL_0083: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
			IL_0088: ldc.i4.s 100
			IL_008a: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
			IL_008f: pop
			// return _CloneStringCrossThead_CurrentValue;
			IL_0090: volatile.
			IL_0092: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
			IL_0097: stloc.1
			// (no C# code)
			IL_0098: leave IL_00a5
		} // end .try
		finally
		{
			IL_009a: ldloc.3
			IL_009b: brfalse.s IL_00a4

			IL_009d: ldloc.2
			IL_009e: call void [mscorlib]System.Threading.Monitor::Exit(object)
			IL_00a3: nop

			IL_00a4: endfinally
		} // end handler

		IL_00a5: ldloc.1
		IL_00a6: ret
	} // end of method InnerAssemblyHelper20210315::CloneStringCrossThead

	.method private hidebysig static 
		void CloneStringCrossThead_Thread () cil managed 
	{
		// Method begins at RVA 0x2114
		// Code size 134 (0x86)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool,
			[2] bool
		)

		// (no C# code)
		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: br.s IL_005d
			// loop start (head: IL_005d)
				IL_0004: nop
				// while (_CloneStringCrossThead_Event_Inner.WaitOne(1000))
				IL_0005: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
				IL_000a: ldc.i4 1000
				IL_000f: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
				// (no C# code)
				IL_0014: ldc.i4.0
				IL_0015: ceq
				IL_0017: stloc.0
				IL_0018: ldloc.0
				IL_0019: brfalse.s IL_001e

				IL_001b: nop
				// }
				IL_001c: br.s IL_0061

				// if (_CloneStringCrossThead_CurrentValue != null)
				IL_001e: volatile.
				IL_0020: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
				IL_0025: ldnull
				IL_0026: cgt.un
				IL_0028: stloc.1
				IL_0029: ldloc.1
				// (no C# code)
				IL_002a: brfalse.s IL_0046

				IL_002c: nop
				// _CloneStringCrossThead_CurrentValue = new string(_CloneStringCrossThead_CurrentValue.ToCharArray());
				IL_002d: volatile.
				IL_002f: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
				IL_0034: callvirt instance char[] [mscorlib]System.String::ToCharArray()
				IL_0039: newobj instance void [mscorlib]System.String::.ctor(char[])
				IL_003e: volatile.
				IL_0040: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
				// (no C# code)
				IL_0045: nop

				// _CloneStringCrossThead_Event.Set();
				IL_0046: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
				IL_004b: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
				IL_0050: pop
				// _CloneStringCrossThead_Event_Inner.Reset();
				IL_0051: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
				IL_0056: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
				IL_005b: pop
				// (no C# code)
				IL_005c: nop

				IL_005d: ldc.i4.1
				IL_005e: stloc.2
				IL_005f: br.s IL_0004
			// end loop

			IL_0061: nop
			IL_0062: leave IL_0085
		} // end .try
		finally
		{
			IL_0064: nop
			// _CloneStringCrossThead_Thread = null;
			IL_0065: ldnull
			IL_0066: volatile.
			IL_0068: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
			// _CloneStringCrossThead_Event.Reset();
			IL_006d: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
			IL_0072: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0077: pop
			// _CloneStringCrossThead_Event_Inner.Reset();
			IL_0078: ldsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
			IL_007d: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0082: pop
			// (no C# code)
			IL_0083: nop
			// }
			IL_0084: endfinally
		} // end handler

		// (no C# code)
		IL_0085: ret
	} // end of method InnerAssemblyHelper20210315::CloneStringCrossThead_Thread

	.method public hidebysig static 
		string GetString (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x21b8
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

		// (no C# code)
		IL_0000: nop
		// int num = bsLength / 2;
		IL_0001: ldarg.2
		IL_0002: ldc.i4.2
		IL_0003: div
		IL_0004: stloc.0
		// char[] array = new char[num];
		IL_0005: ldloc.0
		IL_0006: newarr [mscorlib]System.Char
		IL_000b: stloc.1
		// int num2 = 0;
		IL_000c: ldc.i4.0
		IL_000d: stloc.2
		// (no C# code)
		IL_000e: br.s IL_003a
		// loop start (head: IL_003a)
			IL_0010: nop
			// int num3 = startIndex + num2 * 2;
			IL_0011: ldarg.1
			IL_0012: ldloc.2
			IL_0013: ldc.i4.2
			IL_0014: mul
			IL_0015: add
			IL_0016: stloc.3
			// int num4 = (bsData[num3] << 8) + bsData[num3 + 1];
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
			// num4 ^= key;
			IL_0024: ldloc.s 4
			IL_0026: ldarg.3
			IL_0027: xor
			IL_0028: stloc.s 4
			// array[num2] = (char)num4;
			IL_002a: ldloc.1
			IL_002b: ldloc.2
			IL_002c: ldloc.s 4
			IL_002e: conv.u2
			IL_002f: stelem.i2
			// (no C# code)
			IL_0030: nop
			// num2++;
			IL_0031: ldloc.2
			IL_0032: ldc.i4.1
			IL_0033: add
			IL_0034: stloc.2
			// key++;
			IL_0035: ldarg.3
			IL_0036: ldc.i4.1
			IL_0037: add
			IL_0038: starg.s key

			// while (num2 < num)
			IL_003a: ldloc.2
			IL_003b: ldloc.0
			IL_003c: clt
			IL_003e: stloc.s 5
			IL_0040: ldloc.s 5
			IL_0042: brtrue.s IL_0010
		// end loop

		// return new string(array);
		IL_0044: ldloc.1
		IL_0045: newobj instance void [mscorlib]System.String::.ctor(char[])
		IL_004a: stloc.s 6
		// (no C# code)
		IL_004c: br.s IL_004e

		IL_004e: ldloc.s 6
		IL_0050: ret
	} // end of method InnerAssemblyHelper20210315::GetString

	.method public hidebysig static 
		class [System.Drawing]System.Drawing.Bitmap GetBitmap (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x2218
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

		// (no C# code)
		IL_0000: nop
		// byte[] array = new byte[bsLength];
		IL_0001: ldarg.2
		IL_0002: newarr [mscorlib]System.Byte
		IL_0007: stloc.0
		// int num = 0;
		IL_0008: ldc.i4.0
		IL_0009: stloc.3
		// (no C# code)
		IL_000a: br.s IL_0022
		// loop start (head: IL_0022)
			IL_000c: nop
			// array[num] = (byte)(bsData[startIndex + num] ^ key);
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
			// (no C# code)
			IL_0018: nop
			// num++;
			IL_0019: ldloc.3
			IL_001a: ldc.i4.1
			IL_001b: add
			IL_001c: stloc.3
			// key++;
			IL_001d: ldarg.3
			IL_001e: ldc.i4.1
			IL_001f: add
			IL_0020: starg.s key

			// while (num < bsLength)
			IL_0022: ldloc.3
			IL_0023: ldarg.2
			IL_0024: clt
			IL_0026: stloc.s 4
			IL_0028: ldloc.s 4
			IL_002a: brtrue.s IL_000c
		// end loop

		// MemoryStream stream = new MemoryStream(array);
		IL_002c: ldloc.0
		IL_002d: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0032: stloc.1
		// return new Bitmap(stream);
		IL_0033: ldloc.1
		IL_0034: newobj instance void [System.Drawing]System.Drawing.Bitmap::.ctor(class [mscorlib]System.IO.Stream)
		IL_0039: stloc.2
		IL_003a: ldloc.2
		IL_003b: stloc.s 5
		// (no C# code)
		IL_003d: br.s IL_003f

		IL_003f: ldloc.s 5
		IL_0041: ret
	} // end of method InnerAssemblyHelper20210315::GetBitmap

	.method public hidebysig static 
		class [mscorlib]System.Resources.ResourceSet LoadResourceSet (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x2268
		// Code size 30 (0x1e)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.IO.Stream 'stream',
			[1] class [mscorlib]System.Resources.ResourceSet result,
			[2] class [mscorlib]System.Resources.ResourceSet
		)

		// (no C# code)
		IL_0000: nop
		// Stream stream = GetStream(bs, key, gzip);
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call class [mscorlib]System.IO.Stream DCSoft.Common.InnerAssemblyHelper20210315::GetStream(uint8[], uint8, bool)
		IL_0009: stloc.0
		// ResourceSet result = new ResourceSet(stream);
		IL_000a: ldloc.0
		IL_000b: newobj instance void [mscorlib]System.Resources.ResourceSet::.ctor(class [mscorlib]System.IO.Stream)
		IL_0010: stloc.1
		// stream.Close();
		IL_0011: ldloc.0
		IL_0012: callvirt instance void [mscorlib]System.IO.Stream::Close()
		// (no C# code)
		IL_0017: nop
		// return result;
		IL_0018: ldloc.1
		IL_0019: stloc.2
		// (no C# code)
		IL_001a: br.s IL_001c

		IL_001c: ldloc.2
		IL_001d: ret
	} // end of method InnerAssemblyHelper20210315::LoadResourceSet

	.method private hidebysig static 
		class [mscorlib]System.IO.Stream GetStream (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x2294
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

		// (no C# code)
		IL_0000: nop
		// int num = bs.Length;
		IL_0001: ldarg.0
		IL_0002: ldlen
		IL_0003: conv.i4
		IL_0004: stloc.0
		// int num2 = 0;
		IL_0005: ldc.i4.0
		IL_0006: stloc.2
		// (no C# code)
		IL_0007: br.s IL_001e
		// loop start (head: IL_001e)
			IL_0009: nop
			// bs[num2] = (byte)(bs[num2] ^ key);
			IL_000a: ldarg.0
			IL_000b: ldloc.2
			IL_000c: ldarg.0
			IL_000d: ldloc.2
			IL_000e: ldelem.u1
			IL_000f: ldarg.1
			IL_0010: xor
			IL_0011: conv.u1
			IL_0012: stelem.i1
			// (no C# code)
			IL_0013: nop
			// num2++;
			IL_0014: ldloc.2
			IL_0015: ldc.i4.1
			IL_0016: add
			IL_0017: stloc.2
			// key = (byte)(key + 1);
			IL_0018: ldarg.1
			IL_0019: ldc.i4.1
			IL_001a: add
			IL_001b: conv.u1
			IL_001c: starg.s key

			// while (num2 < num)
			IL_001e: ldloc.2
			IL_001f: ldloc.0
			IL_0020: clt
			IL_0022: stloc.3
			IL_0023: ldloc.3
			IL_0024: brtrue.s IL_0009
		// end loop

		// MemoryStream memoryStream = null;
		IL_0026: ldnull
		IL_0027: stloc.1
		// if (gzip)
		IL_0028: ldarg.2
		IL_0029: stloc.s 4
		IL_002b: ldloc.s 4
		// (no C# code)
		IL_002d: brfalse.s IL_0098

		IL_002f: nop
		// GZipStream gZipStream = new GZipStream(new MemoryStream(bs), CompressionMode.Decompress);
		IL_0030: ldarg.0
		IL_0031: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0036: ldc.i4.0
		IL_0037: newobj instance void [System]System.IO.Compression.GZipStream::.ctor(class [mscorlib]System.IO.Stream, valuetype [System]System.IO.Compression.CompressionMode)
		IL_003c: stloc.s 5
		// memoryStream = new MemoryStream();
		IL_003e: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor()
		IL_0043: stloc.1
		// byte[] array = new byte[1024];
		IL_0044: ldc.i4 1024
		IL_0049: newarr [mscorlib]System.Byte
		IL_004e: stloc.s 6
		// (no C# code)
		IL_0050: br.s IL_007f
		// loop start (head: IL_007f)
			IL_0052: nop
			// num = gZipStream.Read(array, 0, array.Length);
			IL_0053: ldloc.s 5
			IL_0055: ldloc.s 6
			IL_0057: ldc.i4.0
			IL_0058: ldloc.s 6
			IL_005a: ldlen
			IL_005b: conv.i4
			IL_005c: callvirt instance int32 [mscorlib]System.IO.Stream::Read(uint8[], int32, int32)
			IL_0061: stloc.0
			// if (num > 0)
			IL_0062: ldloc.0
			IL_0063: ldc.i4.0
			IL_0064: cgt
			IL_0066: stloc.s 7
			IL_0068: ldloc.s 7
			// while (true)
			IL_006a: brfalse.s IL_007b

			// (no C# code)
			IL_006c: nop
			// memoryStream.Write(array, 0, num);
			IL_006d: ldloc.1
			IL_006e: ldloc.s 6
			IL_0070: ldc.i4.0
			IL_0071: ldloc.0
			IL_0072: callvirt instance void [mscorlib]System.IO.Stream::Write(uint8[], int32, int32)
			// (no C# code)
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

		// gZipStream.Close();
		IL_0084: ldloc.s 5
		IL_0086: callvirt instance void [mscorlib]System.IO.Stream::Close()
		// (no C# code)
		IL_008b: nop
		// memoryStream.Position = 0L;
		IL_008c: ldloc.1
		IL_008d: ldc.i4.0
		IL_008e: conv.i8
		IL_008f: callvirt instance void [mscorlib]System.IO.Stream::set_Position(int64)
		// (no C# code)
		IL_0094: nop
		IL_0095: nop
		IL_0096: br.s IL_00a1

		IL_0098: nop
		// memoryStream = new MemoryStream(bs);
		IL_0099: ldarg.0
		IL_009a: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_009f: stloc.1
		// (no C# code)
		IL_00a0: nop

		// return memoryStream;
		IL_00a1: ldloc.1
		IL_00a2: stloc.s 9
		// (no C# code)
		IL_00a4: br.s IL_00a6

		IL_00a6: ldloc.s 9
		IL_00a8: ret
	} // end of method InnerAssemblyHelper20210315::GetStream

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x2349
		// Code size 39 (0x27)
		.maxstack 8

		// _CloneStringCrossThead_Thread = null;
		IL_0000: ldnull
		IL_0001: volatile.
		IL_0003: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Thread
		// _CloneStringCrossThead_Event = new AutoResetEvent(initialState: false);
		IL_0008: ldc.i4.0
		IL_0009: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_000e: stsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event
		// _CloneStringCrossThead_Event_Inner = new AutoResetEvent(initialState: false);
		IL_0013: ldc.i4.0
		IL_0014: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_0019: stsfld class [mscorlib]System.Threading.AutoResetEvent DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_Event_Inner
		// _CloneStringCrossThead_CurrentValue = null;
		IL_001e: ldnull
		IL_001f: volatile.
		IL_0021: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) DCSoft.Common.InnerAssemblyHelper20210315::_CloneStringCrossThead_CurrentValue
		// }
		IL_0026: ret
	} // end of method InnerAssemblyHelper20210315::.cctor

} // end of class DCSoft.Common.InnerAssemblyHelper20210315

";

        private static readonly string _Code_Template_ComponentResourceManager = @"
.class private auto ansi #CLASSNAME# extends [System]System.ComponentModel.ComponentResourceManager implements [mscorlib]System.IDisposable
{
  .field private class [mscorlib]System.Resources.ResourceSet _Data

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
	IL_0014: call class [mscorlib]System.Resources.ResourceSet DCSoft.Common.InnerAssemblyHelper20210315::LoadResourceSet(uint8[], uint8, bool)
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

        private static void ApplyComponentResourceManagers()
        {
            //return;
            var managers = _Document.ComponentResourceManagers;
            if (managers == null || managers.Count == 0)
            {
                return;
            }
            string typename_ComponentResourceManager = null;
            int itemIndex = 0;
            foreach (var item in managers)
            {
                itemIndex++;
                string resFileName = Path.Combine(_Document.RootPath, item.ClassName + DCILDocument.EXT_resources);
                if (File.Exists(resFileName))
                {
                    if (typename_ComponentResourceManager == null)
                    {
                        var line7 = _Document.SourceLines[item.LineIndex];
                        int index7 = line7.IndexOf("ComponentResourceManager");
                        for (int iCount7 = index7; iCount7 >= 0; iCount7--)
                        {
                            if (char.IsWhiteSpace(line7[iCount7]))
                            {
                                typename_ComponentResourceManager = line7.Substring(iCount7 + 1, index7 - iCount7 + "ComponentResourceManager".Length - 1);
                                break;
                            }
                        }
                    }
                    var bs = File.ReadAllBytes(resFileName);
                    var bsWrite = GetBytesForWrite(bs);// GetGZipCompressedContentIfNeed(bs);
                    string clsName = _ClassNamePrefix + "Res" + itemIndex;
                    string strNewClassCode = _Code_Template_ComponentResourceManager;
                    strNewClassCode = strNewClassCode.Replace("mscorlib", _Document.LibName_mscorlib);
                    strNewClassCode = strNewClassCode.Replace("#CLASSNAME#", clsName);
                    strNewClassCode = strNewClassCode.Replace("[System]System.ComponentModel.ComponentResourceManager", typename_ComponentResourceManager);
                    strNewClassCode = strNewClassCode.Replace("#ENCRYKEY#", bsWrite.Item2.ToString());
                    strNewClassCode = strNewClassCode.Replace("#GETDATA#", ByteArrayDataContainer.GetMethodName(bsWrite.Item1));
                    if (bsWrite.Item3 == false)
                    {
                        strNewClassCode = strNewClassCode.Replace("#GZIPED#", "0");
                    }
                    else
                    {
                        strNewClassCode = strNewClassCode.Replace("#GZIPED#", "1");
                    }
                    _NewClassCodes.Add(strNewClassCode);
                    _ModifiedCount++;

                    item.KeyOperCode.OperCode = "newobj";
                    item.KeyOperCode.OperData = "instance void " + clsName + "::.ctor()";
                    item.KeyOperCode.OwnerMethod.ILCodesModified = true;
                    //_Document.ClearContent(item.KeyOperCode.OwnerMethod);
                    var list3 = item.KeyOperCode.OwnerList;

                    var index = list3.IndexOf(item.KeyOperCode);
                    list3.RemoveAt(index - 1);
                    list3.RemoveAt(index - 2);
                    //list3[index - 1].Enabled = false;
                    //list3[index - 2].Enabled = false;

                    //var line = _Document.SourceLines[item.LineIndex];
                    //string labelID = null;
                    //string operData = null;
                    //string operCode = DCILOperCode.GetILCode(line, ref labelID, ref operData);
                    //line = labelID + " : newobj     instance void " + clsName + "::.ctor()";
                    _Document.SourceLines[item.LineIndex - 2] = string.Empty;
                    _Document.SourceLines[item.LineIndex - 1] = string.Empty;
                    _Document.SourceLines[item.LineIndex] = item.KeyOperCode.ToString();
                    _ModifiedCount += 3;
                    for (int iCount = 0; iCount < 100; iCount++)
                    {
                        string line3 = _Document.SourceLines[item.LineIndex - iCount];

                        if (line3.Contains(".locals")
                            && line3.Contains("init")
                            && line3.Contains("[0]")
                            && line3.Contains("System.ComponentModel.ComponentResourceManager")
                            && line3.Contains("resources"))
                        {
                            string newLine = line3.Replace(typename_ComponentResourceManager, clsName);
                            _Document.SourceLines[item.LineIndex - iCount] = newLine;
                            _ModifiedCount++;
                            break;
                        }

                    }
                    DCILMResource resDefine = null;
                    if (_Document.ResouceFiles.TryGetValue(item.ClassName, out resDefine))
                    {
                        _ModifiedCount += _Document.ClearContent(resDefine);
                        _Document.ResouceFiles.Remove(item.ClassName);
                    }
                }
            }
            foreach (var resFile in _Document.ResouceFiles)
            {
                var classes = _Document.GetClasses(resFile.Key);
                foreach (var myClass in classes)
                {
                    bool deleted = false;
                    foreach (var member in myClass.ChildNodes)
                    {
                        if (member is DCILMethod
                            && member.Name == "InitializeComponent")
                        {
                            // 删除无用的资源
                            _Document.ClearContent(resFile.Value);
                            deleted = true;
                            break;
                        }
                    }
                    if (deleted)
                    {
                        break;
                    }
                }
            }
        }

        private static void ApplyResouceContainerClass()
        {
            //return;
            string bmpTypeName = null;
            var resKeys = new List<string>(_Document.ResouceFiles.Keys);
            var resContainerClassNames = new List<string>();
            var clsPackages = new Dictionary<string, DCILClass>();
            foreach (var clsName in resKeys)
            {
                string fileName = Path.Combine(_Document.RootPath, clsName + DCILDocument.EXT_resources);
                if (File.Exists(fileName) == false)
                {
                    continue;
                }
                var classes = _Document.GetClasses(clsName);
                if (classes == null || classes.Count == 0)
                {
                    continue;
                }
                DCILClass clsPackage = null;
                foreach (var cls in classes)
                {
                    if (cls.IsResoucePackage())
                    {
                        clsPackage = cls;
                        break;
                    }
                }
                if (clsPackage == null)
                {
                    continue;
                }
                resContainerClassNames.Add(clsName);
                clsPackages[clsName] = clsPackage;
            }
            MyResourceDataFileList dataFiles = null;
            if (_Document._IsDotNetCoreAssembly && typeof(string).Assembly.FullName.Contains("mscorlib"))
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "DCSoft.ResourceFileHelper.NetCore");
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }
                if (DCUtils.ExpandResourcesToPath(
                    typeof(DCProtectEngine).Assembly,
                    "DCSoft.AssemblyPublish.DCSoft.ResourceFileHelper.NetCore.",
                    tempPath,
                    true) == 0)
                {
                    DCUtils.ExpandResourcesToPath(
                    typeof(DCProtectEngine).Assembly,
                    "DCNETProtector.DCSoft.ResourceFileHelper.NetCore.",
                    tempPath,
                    true);
                }
                dataFiles = ResourceFileHelper.ExecuteByExe(
                    Path.Combine(tempPath, "DCSoft.ResourceFileHelper.NetCore.exe"),
                    _Document.RootPath,
                    _UILanguage,
                    resContainerClassNames);
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
            }
            else
            {
                dataFiles = ResourceFileHelper.Execute(_Document.RootPath, _UILanguage, resContainerClassNames);// new MyResourceDataFileList(resFileNames);
            }
            if (dataFiles == null || dataFiles.Count == 0)
            {
                return;
            }
            foreach (var dataFile in dataFiles)
            {
                var clsName = dataFile.Name;// Path.GetFileNameWithoutExtension( dataFile.FileName);
                var classes = _Document.GetClasses(clsName);
                DCUtils.ObfuseListOrder(dataFile.Items);
                var clsPackage = clsPackages[clsName];
                var hasBmpValue = dataFile.HasBmp;
                var members = new List<DCILMethod>();
                if (hasBmpValue && bmpTypeName == null)
                {
                    foreach (var item in clsPackage.ChildNodes)
                    {
                        if (item is DCILProperty)
                        {
                            var p = (DCILProperty)item;
                            if (p.ValueTypeName.Contains("System.Drawing.Bitmap"))
                            {
                                bmpTypeName = p.ValueTypeName;
                                break;
                            }
                        }
                    }
                }
                var strNewClassCode = new StringBuilder();
                strNewClassCode.AppendLine(clsPackage.Header);
                strNewClassCode.AppendLine("{");
                //var strDataID = AllocID();
                strNewClassCode.AppendLine("");
                strNewClassCode.AppendLine(".field private static initonly uint8[] _Datas");
                if (hasBmpValue)
                {
                    foreach (var item in dataFile.Items)
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
	// _Datas = Class3.GetData2();
	IL_0001: call uint8[] " + ByteArrayDataContainer.GetMethodName(dataFile.Datas) + @"()
	IL_0006: stsfld uint8[] " + clsName + "::_Datas");

                int labelCount = 100;
                labelCount += 5; strNewClassCode.AppendLine("IL_" + labelCount.ToString("X4") + ": ret");
                strNewClassCode.AppendLine("}");
                if (hasBmpValue)
                {
                    foreach (var item in dataFile.Items)
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
	IL_0018: ldc.i4 " + item.BsLength + @"
	IL_001d: ldc.i4 " + item.Key + @"
	IL_0022: call class " + bmpTypeName + @" DCSoft.Common.InnerAssemblyHelper20210315::GetBitmap(uint8[], int32, int32, int32)
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
                foreach (var item in dataFile.Items)
                {
                    if (item.IsBmp == false)
                    {
                        strNewClassCode.AppendLine(@"  .method assembly hidebysig static  string get_" + item.Name + @"() cil managed 
  {
	.maxstack 4
	.locals init (
		[0] string
	)
	IL_0000: nop
	IL_0001: ldsfld uint8[] " + clsName + @"::_Datas
	IL_0006: ldc.i4 " + item.StartIndex + @"
	IL_000b: ldc.i4 " + item.BsLength + @"
	IL_0010: ldc.i4 " + item.Key + @"
	IL_0015: call string DCSoft.Common.InnerAssemblyHelper20210315::GetString(uint8[], int32, int32, int32)
	IL_001a: stloc.0
	IL_001b: br.s IL_001d
	IL_001d: ldloc.0
	IL_001e: ret
   }");
                    }
                }
                strNewClassCode.AppendLine("}");
                var strCodeText = strNewClassCode.ToString();
                _NewClassCodes.Add(strCodeText);
                _ModifiedCount++;
                var resDefine = _Document.ResouceFiles[clsName];
                //删除对资源文件的引用
                _ModifiedCount += _Document.ClearContent(resDefine);
                // 删除旧的字符串资源类型的定义
                foreach (var group in classes)
                {
                    _ModifiedCount += _Document.ClearContent(group);
                    _Document.AllClasses.Remove(group);
                    _Document.ChildNodes.Remove(group);
                }
            }//foreach
        }



        private class ResourceItem
        {
            public string Name = null;
            public int StartIndex = 0;
            public int BsLength = 0;
            public int Key = 0;
            public object Value = null;
            public override string ToString()
            {
                return this.Name + ":" + this.Value;
            }
        }

        private static int _ModifiedCount = 0;


        public static void RemoveInnerProperty()
        {
            //foreach( var item in _Document.AllClasses)
            //{
            //    if( item.IsPublic == false && item.IsInterface == false && item.ImplementsInterfaces == null )
            //    {
            //        //if( item.Name.Contains("DCSoft.Writer.Controls.WriterControl"))
            //        //{

            //        //}
            //        bool hasRemove = false;
            //        foreach( var m in item.ChildNodes)
            //        {
            //            if( m is DCILProperty)
            //            {
            //                _ModifiedCount += _Document.ClearContent(m);
            //                hasRemove = true;
            //            }
            //        }
            //        if( hasRemove )
            //        {
            //            Console.WriteLine("删除属性 " + item.Name);
            //        }
            //    }
            //}
        }


        private static readonly Random _Random = new Random();

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
    }

    internal static class ByteArrayDataContainer
    {
        public static string FullClassName = "DCSoft20210314.ByteArrayDataContainer";
        public static string LibName_mscorlib = "mscorlib";
        private static List<int> _FieldIndexs = new List<int>();
        public static string GetFieldName(byte[] data)
        {
            var index = IndexOf(data);
            if (_FieldIndexs.Contains(index) == false)
            {
                _FieldIndexs.Add(index);
            }
            return FullClassName + "::_" + index;
        }
        public static string GetMethodName(byte[] data)
        {
            return GetMethodName(IndexOf(data));
        }
        private static string GetMethodName(int index)
        {
            if (index == 398)
            {

            }
            return FullClassName + "::_" + index;
        }
        public static bool HasData()
        {
            return _Datas.Count > 0;
        }
        private static List<byte[]> _Datas = new List<byte[]>();
        private static int IndexOf(byte[] bsData)
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

        private static readonly string _hexs = "0123456789abcdef";

        public static void WriteDataCode(System.IO.TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            for (int index = 0; index < _Datas.Count; index++)
            {
                writer.WriteLine();
                writer.Write(".data cil I_BDC" + index + "= bytearray(");
                var bs = _Datas[index];
                for (int iCount = 0; iCount < bs.Length; iCount++)
                {
                    if ((iCount % 32) == 0)
                    {
                        writer.WriteLine();
                        writer.Write("      ");
                    }
                    var bv = bs[iCount];
                    writer.Write(_hexs[bv >> 4]);
                    writer.Write(_hexs[bv & 0xf]);

                    //writer.Write(bs[iCount].ToString("X2"));
                    writer.Write(' ');
                }
                writer.WriteLine(")");
            }
        }

        public static void WriteClassSourceCode(StringBuilder str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            str.AppendLine(".class private auto ansi abstract sealed beforefieldinit " + FullClassName + " extends[" + LibName_mscorlib + "]System.Object");
            str.AppendLine("{");
            for (int iCount = 0; iCount < _Datas.Count; iCount++)
            {
                str.AppendLine(@".class nested private explicit ansi sealed _DATA" + iCount + @" extends[mscorlib]System.ValueType
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
                int labelIndex = 10;
                foreach (var index in _FieldIndexs)
                {
                    labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": ldc.i4 " + _Datas[index].Length);
                    labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": newarr [" + LibName_mscorlib + "]System.Byte");
                    labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": dup");
                    labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": ldtoken field valuetype " + FullClassName + @"/_DATA" + index + " " + FullClassName + @"::_Field" + index);
                    labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)");
                    labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": stsfld uint8[] " + FullClassName + "::_" + index);
                }
                labelIndex += 10; str.AppendLine("IL_" + labelIndex + ": ret");
                str.AppendLine("}// end of method .cctor ");
                //	// (no C# code)
                //	IL_0000: ldc.i4.s 11
                //	IL_0002: newarr [mscorlib]System.Byte
                //	IL_0007: dup
                //	IL_0008: ldtoken field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=11' '<PrivateImplementationDetails>'::'5BCAE92B79178C81E4F5D04B52475E0D9CABE3C2'
                //	IL_000d: call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array, valuetype [mscorlib]System.RuntimeFieldHandle)

                //	IL_0012: stsfld uint8[] SampleWinApp.frmMain::_Switchs
                //	// }
                //	IL_0017: ret
                //} // end of method frmMain::.cctor");

            }
            for (int iCount = 0; iCount < _Datas.Count; iCount++)
            {
                if (_FieldIndexs.Contains(iCount))
                {
                    continue;
                }
                str.AppendLine(@".method public hidebysig static uint8[] _" + iCount + @"() cil managed 
{
	// Method begins at RVA 0x2b64
	// Code size 23 (0x17)
	.maxstack 3
	.locals init (
		[0] uint8[]
	)
	IL_0000: nop
	IL_0001: ldc.i4 " + _Datas[iCount].Length + @"
	IL_0002: newarr [" + LibName_mscorlib + @"]System.Byte
	IL_0007: dup
	IL_0008: ldtoken field valuetype " + FullClassName + @"/_DATA" + iCount + " " + FullClassName + @"::_Field" + iCount + @"
    IL_000d: call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)
	IL_0012: stloc.0
	IL_0013: br.s IL_0015
	IL_0015: ldloc.0
	IL_0016: ret
}
");
            }
            str.AppendLine("}");
        }
    }

    internal class ComponentResourceManagerInfo
    {
        public DCILOperCode KeyOperCode = null;
        public int LineIndex = 0;
        public DCILMethod Method = null;
        public DCILClass MyClass = null;
        public string ClassName = null;
    }
    internal class DCILInvokeMethodInfo : IEqualsValue<DCILInvokeMethodInfo>
    {
        public static DCILInvokeMethodInfo ReadInfoInSimpleMode(DCILReader reader)
        {
            DCILInvokeMethodInfo result = new DCILInvokeMethodInfo();
            return result;
        }
        public DCILInvokeMethodInfo()
        {

        }
        public int LineIndex = 0;
        public readonly bool SimpleMode = false;
        public DCILInvokeMethodInfo(DCILReader reader, bool simpleMode = false)
        {
            this.LineIndex = reader.CurrentLineIndex();
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

            //var cls = this.OwnerType?.LocalClass;
            //if( cls != null )
            //{
            //    foreach( var item in cls.ChildNodes)
            //    {
            //        if( item is DCILMethod && item.Name == this.MethodName)
            //        {
            //            var method = (DCILMethod)item;
            //            if( method.MatchSign( this.ReturnType , this.GenericParamters , this.Paramters ) )
            //            {
            //                this.LocalMethod = method;
            //                break;
            //            }
            //        }
            //    }
            //}
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
                //var strSign = DCILMethod.InnerGetSignString(
                //    this.ReturnType, 
                //    this.MethodName, 
                //    this.GenericParamters ,
                //    gps,
                //    this.Paramters);
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
        public bool HasParameters
        {
            get
            {
                return this.Paramters != null && this.Paramters.Count > 0;
            }
        }
        public List<DCILMethodParamter> Paramters = null;
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
            this.ReturnType.WriteTo(writer);
            writer.Write(' ');
            this.OwnerType.WriteTo(writer);
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
            if (this.Paramters != null && this.Paramters.Count > 0)
            {
                bool hasAdd = false;
                foreach (var item in this.Paramters)
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
                    if (item.Name != null && item.Name.Length > 0)
                    {
                        writer.Write(item.Name);
                    }
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

        public void WriteItem(string item)
        {
            if (item != null && item.Length > 0)
            {
                this.EnsureIndent();
                this.InnerWrite(' ');
                this.InnerWrite(item);
            }
        }

        public void WriteItems(IList<string> items, char splitChar = ',')
        {
            this.EnsureIndent();
            if (items != null && items.Count > 0)
            {
                int len = items.Count;
                for (int iCount = 0; iCount < len; iCount++)
                {
                    if (iCount > 0)
                    {
                        this.InnerWrite(splitChar);
                    }
                    this.InnerWrite(items[iCount]);
                }
            }
        }
        public void WriteObjects(List<DCILObject> objs)
        {
            if (objs != null && objs.Count > 0)
            {
                foreach (var obj in objs)
                {
                    //this.EnsureNewLine();
                    obj.WriteTo(this);
                }
            }
        }
        public void Write(string txt)
        {
            //if (txt == "Syste" || txt == "Syste ")
            //{

            //}
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
        private void InnerWrite(string txt)
        {
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
        private void InnerWrite(char c)
        {
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
            //if( txt == "Syste" || txt == "Syste ")
            //{

            //}
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
        /// <summary>
        /// 压缩空格字符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CompressWhitespace(string text)
        {
            if (text == null || text.Length == 0)
            {
                return text;
            }
            var str = new StringBuilder();
            var lastIsWhitespace = false;
            foreach (char c in text)
            {
                if (DCILDocument.IsWhitespace(c))
                {
                    if (lastIsWhitespace == false)
                    {
                        str.Append(c);
                        lastIsWhitespace = true;
                    }
                }
                else
                {
                    lastIsWhitespace = false;
                    str.Append(c);
                }
            }
            return str.ToString();
        }

        public DCILReader(string fileName, System.Text.Encoding encoding, DCILDocument doc)
        {
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
        public string GetSubString(int pos, int len)
        {
            return this._Text.Substring(pos, len);
        }
        //public DCILTypeReference Cache( DCILTypeReference type )
        //{
        //    if( type.HasLibraryName
        //        && this.Document != null 
        //        && type.Name != null )
        //    {
        //        this.Document.LibraryNames[type.Name] = type.LibraryName;
        //    }
        //    return type;
        //}
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
        //public readonly string Word_StartGroup = "{";
        //public readonly string Word_EndGroup = "}";

        public List<string> GetStyles(List<string> words, int skipCount = 2)
        {
            var list = new List<string>();
            for (int iCount = 0; iCount < words.Count - skipCount; iCount++)
            {
                list.Add(words[iCount]);
            }
            return list;
        }
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
                if (IsWhiteSpace(c) && isInGroup == false)
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

        private static readonly DCILReader _ReaderForSplit = new DCILReader();

        internal static List<string> SplitToWords(string text)
        {
            if (text == null || text.Length == 0)
            {
                return new List<string>();
            }
            _ReaderForSplit._Text = text;
            _ReaderForSplit._Length = text.Length;
            _ReaderForSplit._Position = 0;
            var list = new List<string>();
            while (_ReaderForSplit.HasContentLeft())
            {
                var word = _ReaderForSplit.ReadWord();
                if (word != null)
                {
                    list.Add(word);
                }
                else
                {
                    break;
                }
            }
            _ReaderForSplit._Text = null;
            return list;
        }
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
        //public string ReadContentChars(int maxLength)
        //{
        //    var list = new List<char>();
        //    for(; this._Position < this._Length; this._Position ++)
        //    {
        //        if( IsWhiteSpace( this._Text[ this._Position ]) == false )
        //        {
        //            return this._Text[this._Position];
        //        }
        //    }
        //    return char.MinValue;
        //}

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

        //public string ReadWordForTypeName( )
        //{
        //    this._CurrentItemLength = 0;
        //Retry:;
        //    for (; this._Position < this._Length; this._Position++)
        //    {
        //        if (IsWhiteSpace(this._Text[this._Position]) == false)
        //        {
        //            bool isInGroup = false;
        //            for (; this._Position < this._Length; this._Position++)
        //            {
        //                char c = this._Text[this._Position];
        //                if (c == '\'')
        //                {
        //                    isInGroup = !isInGroup;
        //                }
        //                if (isInGroup)
        //                {
        //                    // 在分号组内，无条件的添加
        //                    this._CurrentItem[this._CurrentItemLength++] = c;
        //                }
        //                else
        //                {
        //                    if (c == '/' && this._Position < this._Length - 1 && this._Text[this._Position + 1] == '/')
        //                    {
        //                        // 遇到注释
        //                        this.MoveNextLine();
        //                        if (this._CurrentItemLength > 0)
        //                        {
        //                            return GetCurrentItemString();
        //                        }
        //                        else
        //                        {
        //                            goto Retry;
        //                        }
        //                    }
        //                    if (IsWhiteSpace(c))
        //                    {
        //                        // 遇到空格
        //                        if (this._CurrentItemLength > 0)
        //                        {
        //                            return GetCurrentItemString();
        //                        }
        //                    }
        //                    else if (IsSplitChar(c))
        //                    {
        //                        if (this._CurrentItemLength == 0)
        //                        {
        //                            this._Position++;
        //                            return _SplitChars[c];
        //                        }
        //                        else
        //                        {
        //                            return GetCurrentItemString();
        //                        }
        //                    }
        //                    else if ( c =='[' || c ==']')
        //                    {
        //                        if (this._CurrentItemLength == 0)
        //                        {
        //                            this._Position++;
        //                            return c.ToString();
        //                        }
        //                        else
        //                        {
        //                            return GetCurrentItemString();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        this._CurrentItem[this._CurrentItemLength++] = c;
        //                    }
        //                }
        //            }
        //            if (this._CurrentItemLength > 0)
        //            {
        //                return GetCurrentItemString();
        //            }
        //        }
        //    }
        //    return null;
        //}

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
                if (DCILDocument.IsWhitespace(this._Text[this._Position]) == false)
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
        public bool MatchNextWord(string word, bool firstSkipSpace = true)
        {
            if (word == null || word.Length == 0)
            {
                throw new ArgumentNullException("word");
            }
            if (firstSkipSpace)
            {
                this.SkipWhitespace();
            }
            if (this.HasContentLeft())
            {
                if (string.Compare(this._Text, this._Position, word, 0, word.Length, StringComparison.Ordinal) == 0)
                {
                    this._Position += word.Length;
                    return true;
                }
            }
            return false;
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
        public string ReadAfterChars(string chrs, out char endChar)
        {
            endChar = char.MinValue;
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            for (int iCount = this._Position; iCount < this._Length; iCount++)
            {
                if (chrs.IndexOf(this._Text[iCount]) >= 0)
                {
                    var result = this._Text.Substring(this._Position, iCount - this._Position + 1);
                    this._Position = iCount + 1;
                    endChar = this._Text[iCount];
                    return result;
                }
            }
            var result2 = this._Text.Substring(this._Position);
            this._Position = this._Length;
            return result2;
        }

        private char GetFirstNotWhitespaceChar(string text)
        {
            int len = text.Length;
            for (int iCount = 0; iCount < len; iCount++)
            {
                if (IsWhiteSpace(text[iCount]) == false)
                {
                    return text[iCount];
                }
            }
            return char.MinValue;
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
        public string ReadToString(string text)
        {
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index = this._Text.IndexOf(text, this._Position);
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
            //return this._Text.Substring( index )
            //int index = this._Position;
            //for(; this._Position < this._Length; this._Position ++ )
            //{
            //    if( this._Text[ this._Position ] == c )
            //    {
            //        return this._Text.Substring(index, this._Position - index);
            //    }
            //}
            //return this._Text.Substring(index);
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
        private void AddCurrentItem(List<string> list)
        {
            if (this._CurrentItemLength > 0)
            {
                list.Add(new string(this._CurrentItem, 0, this._CurrentItemLength));
                this._CurrentItemLength = 0;
            }
        }
        public List<string> ReadWordsAfterChars(string chrs, out char lastChar)
        {
            if (chrs == null || chrs.Length == 0)
            {
                throw new ArgumentNullException("chrs");
            }
            lastChar = char.MinValue;
            var list = new List<string>();
            while (this._Position < this._Length)
            {
                var word = ReadWord();
                if (word == null)
                {
                    break;
                }
                if (word.Length == 1 && chrs.IndexOf(word[0]) >= 0)
                {
                    lastChar = word[0];
                    break;
                }
                list.Add(word);
            }
            return list;
            //this._CurrentItemLength = 0;
            //var list = new List<string>();
            //bool inGroup = false;
            //for(; this._Position < this._Length; this._Position ++)
            //{
            //    var c = this._Text[this._Position];
            //    if( c == '\'')
            //    {
            //        inGroup = !inGroup;
            //    }
            //    if( inGroup == false )
            //    {
            //        if (IsWhiteSpace(c))
            //        {
            //            AddCurrentItem(list);
            //        }
            //        else if( c == '/' && this._Position < this._Length - 1 && this._Text[ this._Position + 1 ] == '/')
            //        {
            //            // 遇到注释
            //            this.MoveNextLine();
            //            AddCurrentItem(list);
            //        }
            //        else
            //        {
            //            int index = chrs.IndexOf(c);
            //            if( index > 0 )
            //            {
            //                lastChar = chrs[index];
            //                break;
            //            }
            //            else 
            //            {
            //                if( IsSplitChar( c ))
            //                {
            //                    AddCurrentItem(list);
            //                }
            //                this._CurrentItem[this._CurrentItemLength++] = c;
            //            }
            //        }
            //    }
            //}
            //AddCurrentItem(list);
            //return list;
        }

        public List<string> ReadWordsBeforeChar(char chr)
        {
            this._CurrentItemLength = 0;
            var list = new List<string>();
            bool inGroup = false;
            for (; this._Position < this._Length; this._Position++)
            {
                var c = this._Text[this._Position];
                if (c == '\'')
                {
                    inGroup = !inGroup;
                }
                if (inGroup == false)
                {
                    if (IsWhiteSpace(c))
                    {
                        AddCurrentItem(list);
                    }
                    else if (c == '/' && this._Position < this._Length - 1 && this._Text[this._Position + 1] == '/')
                    {
                        // 遇到注释
                        this.MoveNextLine();
                        AddCurrentItem(list);
                    }
                    else if (c == chr)
                    {
                        break;
                    }
                    else
                    {
                        if (IsSplitChar(c))
                        {
                            AddCurrentItem(list);
                        }
                        this._CurrentItem[this._CurrentItemLength++] = c;
                    }
                }
            }
            AddCurrentItem(list);
            return list;
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
        public string ReadToCharsExcludeLastChar(string chrs, out char resultChar)
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
                    return this._Text.Substring(index, this._Position - index);
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
                var result = this.ReadAfterCharExcludeLastChar(')');
                return result;
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
                                Array.Copy(_byteBuffer, temp, bufferSize);
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
        //public string ParseHexsWithoutComment( string text)
        //{
        //    var strHexs = new StringBuilder();

        //}

        public char CurrentChar
        {
            get
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
        public List<string> ReadLineInWords()
        {
            if (this.HasContentLeft())
            {
                var line = ReadLine();
                return DCILReader.SplitToWords(line);
            }
            return null;
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
        public void Offset(int step)
        {
            this._Position += step;
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
    internal class DCILDocument : DCILObject, IDisposable
    {

        public const string Name_class = ".class";
        public const string Name_method = ".method";
        public const string Name_data = ".data";
        public const string Name_mresource = ".mresource";
        public const string Name_custom = ".custom";
        /// <summary>
        /// 字符串 .resources
        /// </summary>
        public static readonly string EXT_resources = ".resources";
        public static readonly string Name_property = ".property";
        public static readonly string Name_get = ".get";
        public static readonly string Name_set = ".set";

        public DCILDocument()
        {

        }

        public void LoadByAssembly(string asmFileName, System.Text.Encoding encoding, string ildasmExeFileName)
        {
            var ilFileName = asmFileName + ".il";
            ResourceFileHelper.RunExe(ildasmExeFileName, "\"" + asmFileName + "\" /forward /UTF8 \"/output=" + ilFileName + "\"");
            LoadByReader(ilFileName, encoding);
            this.RootPath = Path.GetDirectoryName(asmFileName);
            this.AssemblyFileName = asmFileName;
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
        private Dictionary<string, int> _DocumentResources = null;
        public int GetResourceNodeIndex(string resName)
        {
            if (_DocumentResources == null)
            {
                _DocumentResources = GetNodeIndexs<DCILMResource>();
            }
            int result = 0;
            if (this._DocumentResources.TryGetValue(resName, out result))
            {
                return result;
            }
            return -1;
        }

        public bool RemoveResourceNode(string name)
        {
            int index = GetResourceNodeIndex(name);
            if (index >= 0)
            {
                this.ChildNodes.RemoveAt(index);
                this._DocumentResources = null;
                return true;
            }
            return false;
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
        public string GetTypeNameWithLibraryName(string typeName, string defaultLibName = null)
        {
            if (typeName == null || typeName.Length == 0)
            {
                throw new ArgumentNullException("typeName");
            }
            string result = null;
            if (this.LibraryNames.TryGetValue(typeName, out result))
            {
                return "[" + result + "]" + typeName;
            }
            if (defaultLibName != null && defaultLibName.Length > 0)
            {
                return "[" + defaultLibName + "]" + typeName;
            }
            else
            {
                return "[" + this.LibName_mscorlib + "]" + typeName;
            }
        }
        //public string GetLibraryName(string typeName)
        //{
        //    if (typeName == null || typeName.Length == 0)
        //    {
        //        throw new ArgumentNullException("typeName");
        //    }
        //    string result = null;
        //    if( this.LibraryNames.TryGetValue( typeName , out result ) )
        //    {
        //        return result;
        //    }
        //    return this.LibName_mscorlib;
        //}
        public void FixDomState()
        {
            this._CachedTypes = new Dictionary<DCILTypeReference, DCILTypeReference>();
            //foreach( var item in this.ChildNodes )
            //{
            //    if( item is DCILAssembly)
            //    {
            //        ((DCILAssembly)item).CusotmAttributesCacheTypeReference(this);
            //    }
            //}
            this.LibName_mscorlib = null;
            foreach (var item in this.ChildNodes)
            {
                if (this.LibName_mscorlib == null && item is DCILAssembly)
                {
                    var asm = (DCILAssembly)item;
                    if (asm.IsExtern)
                    {
                        this.LibName_mscorlib = asm.Name;
                        break;
                    }
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
            var clses = new Dictionary<string, DCILClass>();
            GetAllClasses(null, this.ChildNodes, clses);
            foreach (var cls in clses.Values)
            {
                cls.CacheInfo(this, clses);
            }
            int len = Math.Min(500, this.ChildNodes.Count);
            for (int iCount = 0; iCount < len; iCount++)
            {
                if (this.ChildNodes[iCount] is DCILAssembly)
                {
                    var asm = (DCILAssembly)this.ChildNodes[iCount];
                    asm.CusotmAttributesCacheTypeReference(this);
                }
            }
            foreach (var item in this._CachedTypes.Values)
            {
                if (item.HasLibraryName == false)
                {
                    if (item.Name == "__DC20210205._264")
                    {

                    }
                    if (clses.TryGetValue(item.Name, out item.LocalClass))
                    {

                    }
                    else
                    {

                    }
                }
            }
            //foreach (var item in this._CachedTypes.Values)
            //{
            //    item.UpdateLocalClass(clses);
            //}
            foreach (var item in this._CachedInvokeMethods.Values)
            {
                item.UpdateLocalInfo(this, clses);
            }

        }
        private void GetAllClasses(string baseName, System.Collections.IList list, Dictionary<string, DCILClass> clses)
        {
            foreach (var item in list)
            {
                if (item is DCILClass)
                {
                    var cls = (DCILClass)item;
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

        private void CompressTypeReferences(DCILClass cls)
        {

        }
        //private void UpdateDomState(DCILClass cls)
        //{
        //    //SetTypeLibraryName(cls.BaseType);
        //    if( cls.ImplementsInterfaces != null && cls.ImplementsInterfaces.Count > 0 )
        //    {
        //        foreach( var item in cls.ImplementsInterfaces)
        //        {
        //            //SetTypeLibraryName(item);
        //        }
        //    }
        //}
        //private void SetTypeLibraryName( DCILTypeReference t )
        //{
        //    if( t != null 
        //        && t.LibraryName != null 
        //        && t.LibraryName.Length > 0 
        //        && t.Name != null 
        //        && this.Name.Length > 0 )
        //    {
        //        this.LibraryNames[t.Name] = t.LibraryName;
        //    }
        //    if( t.GenericParamters != null && t.GenericParamters.Count > 0 )
        //    {
        //        foreach ( var item in t.GenericParamters )
        //        {
        //            SetTypeLibraryName(item);
        //        }
        //    }
        //}
        //private void SetTypeLibraryName( string typeName , string libName )
        //{
        //    if(libName != null && libName.Length > 0  && typeName != null && typeName.Length > 0 )
        //    {
        //        this.LibraryNames[typeName] = libName;
        //    }
        //}
        //public List<DCILModule> Modules = new List<DCILModule>();
        //public List<DCILMResource> Resources = new List<DCILMResource>();
        //public List<DCILAssembly> Assemblies = new List<DCILAssembly>();
        //public List<DCILCustomAttribute> CustomAttributes = new List<DCILCustomAttribute>();
        //public List<DCILData> Datas = new List<DCILData>();
        //public List<DCILClass> Classes = new List<DCILClass>();

        public override void Load(DCILReader reader)
        {
            if (this.ChildNodes == null)
            {
                this.ChildNodes = new DCILObjectList();
            }
            var classMap = new Dictionary<string, DCILClass>();

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
                        this.ChildNodes.Add(new DCILModule(reader));
                        break;
                    case DCILMResource.TagName_mresource:
                        {
                            var item = new DCILMResource(this, reader);
                            this.ResouceFiles[item.Name] = item;
                            this.ChildNodes.Add(item);
                        }
                        break;
                    case DCILAssembly.TagName:
                        var asm = new DCILAssembly();
                        asm.LoadHeader(reader);
                        bool match = false;
                        foreach (var item in this.ChildNodes)
                        {
                            if (item is DCILAssembly && item.Name == asm.Name)
                            {
                                ((DCILAssembly)item).LoadContent(reader);
                                match = true;
                            }
                        }
                        if (match == false)
                        {
                            asm.LoadContent(reader);
                            this.ChildNodes.Add(asm);
                        }
                        break;
                    case DCILCustomAttribute.TagName_custom:
                        {
                            this.ChildNodes.Add((DCILObject)DCILCustomAttribute.Create(this, reader));
                            //this.ReadCustomAttribute(reader);
                        }
                        break;
                    case DCILData.TagName_Data:
                        {
                            this.ChildNodes.Add(new DCILData(reader));
                        }
                        break;
                    case DCILClass.TagName:
                        {
                            //if( reader.CurrentLineIndex() >34735)
                            //{

                            //}
                            var cls = new DCILClass();
                            cls.LoadHeader(reader);
                            if (cls.Name != null && cls.Name.Length > 0)
                            {
                                //if( cls.Name == "DCSoft.Common.DictionaryDoubleCounter")
                                //{

                                //}
                                DCILClass oldCls = null;
                                if (classMap.TryGetValue(cls.Name, out oldCls))
                                {
                                    oldCls.LoadContent(reader);
                                }
                                else
                                {
                                    cls.LoadContent(reader);
                                    this.ChildNodes.Add(cls);
                                    classMap[cls.Name] = cls;
                                    reader.NumOfClass++;
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
            FixDomState();
        }

        public override void WriteTo(DCILWriter writer)
        {
            writer.WriteObjects(this.ChildNodes);
        }

        public void WriteToFile(string fileName, Encoding encod)
        {
            using (var writer = new System.IO.StreamWriter(fileName, false, encod))
            {
                var w2 = new DCILWriter(writer);
                this.WriteTo(w2);
            }
        }

        //public List<DCILModuleExtern> Modules = null;
        //public List<DCILAssembly> Assemblies = null;
        //public List<DCILMResource> Resources = null;

        public DCILDocument(string ilFileName, Encoding encod, JieJieSwitchs options = null)
        {
            if (ilFileName == null)
            {
                throw new ArgumentNullException("ilFileName");
            }
            if (File.Exists(ilFileName) == false)
            {
                throw new FileNotFoundException(ilFileName);
            }
            this.ProtectedOptions = options;
            this.FileSize = (int)new FileInfo(ilFileName).Length;
            if (encod == null)
            {
                encod = Encoding.Default;
            }
            using (var reader = new System.IO.StreamReader(ilFileName, Encoding.Default, true))
            {
                var line = reader.ReadLine();
                var list = new List<string>();
                while (line != null)
                {
                    list.Add(line);
                    line = reader.ReadLine();
                }
                this.SourceLines = list.ToArray();
            }
            this.FileName = ilFileName;
            this._Name = Path.GetFileName(ilFileName);
            this.RootPath = Path.GetDirectoryName(ilFileName);
            this.ChildNodes = new DCILObjectList();
            this.Parse();
            this.OwnerDocument = this;
        }
        public DCILDocument(string rootPath, string[] srcLines)
        {
            this.SourceLines = srcLines;
            this.RootPath = rootPath;
            this.ChildNodes = new DCILObjectList();
            this.Parse();
            this.OwnerDocument = this;
        }

        public readonly JieJieSwitchs ProtectedOptions = null;

        /// <summary>
        /// 获得所有支持的语言信息对象
        /// </summary>
        /// <param name="rootDir">根目录</param>
        /// <returns>语言信息对象</returns>
        public System.Globalization.CultureInfo[] GetSupportCultures()
        {
            var list = new List<System.Globalization.CultureInfo>();
            //string resFileName  = null;
            //if( this.AssemblyFileName != null )
            //{
            //    resFileName = Path.GetFileNameWithoutExtension(this.AssemblyFileName);
            //    resFileName = resFileName + ".resouces.dll";
            //}
            foreach (var dir in Directory.GetDirectories(this.RootPath))
            {
                var fns = Directory.GetFiles(dir, "*" + EXT_resources);
                if (fns != null && fns.Length > 0)
                {
                    var name = Path.GetFileName(dir);
                    //if (resFileName != null)
                    //{
                    //    if (File.Exists(Path.Combine(dir, resFileName)))
                    //    {
                    //        try
                    //        {
                    //            var cul = System.Globalization.CultureInfo.GetCultureInfo(name);
                    //            if (cul != null)
                    //            {
                    //                list.Add(cul);
                    //            }
                    //        }
                    //        catch
                    //        {

                    //        }
                    //    }
                    //}
                    //else
                    {
                        try
                        {
                            var cul = System.Globalization.CultureInfo.GetCultureInfo(name);
                            if (cul != null)
                            {
                                list.Add(cul);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return list.ToArray();
        }

        public int ClearContent(DCILObject group)
        {
            for (int iCount = group.StartLineIndex; iCount <= group.EndLineIndex; iCount++)
            {
                this.SourceLines[iCount] = string.Empty;
            }
            return group.EndLineIndex - group.StartLineIndex;
        }

        public override string ToString()
        {
            return "ILFile " + this.FileName;
        }
        public readonly int FileSize = 0;
        public readonly string FileName = null;
        public string FileDirectory
        {
            get
            {
                if (this.FileName != null && this.FileName.Length > 0)
                {
                    return Path.GetDirectoryName(this.FileName);
                }
                else
                {
                    return null;
                }
            }
        }
        public string[] SourceLines = null;
        public string RootPath = null;
        public string AssemblyFileName = null;
        public string LibName_mscorlib = "mscorlib";
        public int ModifiedCount = 0;
        public List<ComponentResourceManagerInfo> ComponentResourceManagers = new List<ComponentResourceManagerInfo>();
        public List<DCILOperCode_LoadString> StringDefines = new List<DCILOperCode_LoadString>();
        public List<DCILObject> AllGroups = new List<DCILObject>();
        public List<DCILClass> AllClasses = new List<DCILClass>();
        public List<string> ReferenceAssemblies = new List<string>();

        public SortedDictionary<string, DCILMResource> ResouceFiles = new SortedDictionary<string, DCILMResource>();

        public List<System.Tuple<DCILMethod, int>> SecurityMethods = new List<Tuple<DCILMethod, int>>();
        public static readonly string SecurityMethodFlags = "Yuan_yong_fu_dao_ci_yi_you";

        public void Dispose()
        {
            this.SourceLines = null;
            if (this.ComponentResourceManagers != null)
            {
                this.ComponentResourceManagers.Clear();
                this.ComponentResourceManagers = null;
            }
            if (this.StringDefines != null)
            {
                this.StringDefines.Clear();
                this.StringDefines = null;
            }
            if (this.AllGroups != null)
            {
                this.AllGroups.Clear();
                this.AllGroups = null;
            }
            if (this.ResouceFiles != null)
            {
                this.ResouceFiles.Clear();
                this.ResouceFiles = null;
            }
        }

        public bool _IsDotNetCoreAssembly = false;

        public List<DCILClass> GetClasses(string name)
        {
            var list = new List<DCILClass>();
            foreach (var item in this.ChildNodes)
            {
                if (item is DCILClass && item.Name == name)
                {
                    list.Add((DCILClass)item);
                }
            }
            return list;
        }

        private void Parse()
        {
            int tick = Environment.TickCount;
            this._IsDotNetCoreAssembly = false;
            this.LibName_mscorlib = null;
            int lineNum1000 = Math.Min(1000, this.SourceLines.Length);
            for (int iCount = 0; iCount < lineNum1000; iCount++)
            {
                string line = this.SourceLines[iCount].Trim();
                if (line == ".assembly extern System.Runtime")
                {
                    this._IsDotNetCoreAssembly = true;
                    this.LibName_mscorlib = "System.Runtime";
                    break;
                }
            }
            ParseILGroup(this, 0);
            if (this.LibName_mscorlib == null)
            {
                this.LibName_mscorlib = "mscorlib";
            }
            tick = Math.Abs(Environment.TickCount - tick);
        }

        private void ParseILGroup(DCILObject rootGroup, int startLineIndex)
        {
            int lineNum = this.SourceLines.Length;
            //List<StringDefine> strDefines = new List<StringDefine>();
            string currentMethodName = null;
            if (rootGroup.Type == Name_method)
            {
                currentMethodName = rootGroup.Name;
            }
            int currentLevel = 0;
            DCILMethod currentMethod = rootGroup.GetOwnerMethod();
            for (int iCount = startLineIndex; iCount < lineNum; iCount++)
            {
                if (iCount == 30238)
                {

                }
                var line = this.SourceLines[iCount];
                if (line.Length == 0)
                {
                    continue;
                }
                //if( line.IndexOf(".class") >= 0 )
                //{

                //}
                string firstWord = null;
                int firstCharIndex = line.Length;
                for (int iCount2 = 0; iCount2 < firstCharIndex; iCount2++)
                {
                    var c = line[iCount2];
                    if (c != ' ' && c != '\t')
                    {
                        for (int iCount3 = iCount2 + 1; iCount3 < firstCharIndex; iCount3++)
                        {
                            var c2 = line[iCount3];
                            if (c2 == ' ' || c2 == '\t')
                            {
                                firstWord = line.Substring(iCount2, iCount3 - iCount2);
                                break;
                            }
                        }
                        if (firstWord == null)
                        {
                            firstWord = line.Substring(iCount2);
                        }
                        firstCharIndex = iCount2;
                        break;
                    }
                }
                if (firstWord == null || firstWord.Length == 0)
                {
                    // 完全的空白行
                    continue;
                }
                if (firstWord.StartsWith("IL_", StringComparison.Ordinal))
                {
                    // 执行代码行
                    var operInfo = new DCILOperCode(line, iCount);
                    DCILOperCodeList operInfoList = rootGroup.OperCodes;
                    if (operInfoList.StartLineIndex == 0)
                    {
                        operInfoList.StartLineIndex = iCount;
                    }
                    operInfo.OwnerMethod = currentMethod;
                    operInfo.OwnerList = operInfoList;
                    operInfoList.Add(operInfo);

                    if (operInfo.OperData == null || operInfo.OperData.Length == 0)
                    {
                        continue;
                    }
                    if (operInfo.OperCode == "call"
                        || operInfo.OperCode == "callvirt"
                        || operInfo.OperCode == "newobj"
                        || operInfo.OperCode == "ldftn"
                        || operInfo.OperCode == "ldvirtftn")
                    {
                        if (DCUtils.StringEndWithChar(operInfo.OperData, ')') == false)
                        {
                            var strData = new StringBuilder();
                            strData.Append(operInfo.OperData);
                            for (iCount++; iCount < lineNum; iCount++)
                            {
                                var line2 = this.SourceLines[iCount].Trim();
                                if (line2.Length > 0)
                                {
                                    strData.Append(line2);
                                    if (line2[line2.Length - 1] == ')')
                                    {
                                        operInfo.OperData = strData.ToString();
                                        operInfo.EndLineIndex = iCount;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (operInfo.OperCode == "call")
                    {

                        //if (strOperData.StartsWith("string", StringComparison.Ordinal))
                        //{
                        //    string methodName = strOperData.Substring(7).Trim();
                        //    if (methodName[0] != '[')
                        //    {
                        //        int index9 = methodName.IndexOf("::");
                        //        if (index9 > 0)
                        //        {
                        //            string typeName = methodName.Substring(0, index9);
                        //            string mn2 = methodName.Substring(index9 + 2);
                        //            int index10 = mn2.IndexOf('(');
                        //            mn2 = mn2.Substring(0, index10);
                        //            if (mn2.StartsWith("get_", StringComparison.Ordinal))
                        //            {
                        //                StringResourceFileDefine file = null;
                        //                if (_StrResourceFiles.TryGetValue(typeName, out file))
                        //                {
                        //                    file.References[iCount] = mn2.Substring(4);
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    else if (operInfo.OperCode == "newobj")
                    {
                        if (operInfo.OperData.Contains("ComponentResourceManager::.ctor")
                            && rootGroup.Name == "InitializeComponent")
                        {
                            var line2 = this.SourceLines[iCount - 2];
                            var code3 = new DCILOperCode(line2, 0);
                            //string labelID2 = null;
                            //string operData2 = null;
                            //string operCode2 = DCILOperCode.GetILCode(line2, ref labelID2, ref operData2);
                            if (code3.OperCode == "ldtoken")
                            {
                                ComponentResourceManagerInfo info = new ComponentResourceManagerInfo();
                                info.LineIndex = iCount;
                                info.KeyOperCode = operInfo;
                                info.Method = currentMethod;
                                info.ClassName = code3.OperData;
                                this.ComponentResourceManagers.Add(info);
                            }
                        }
                    }
                    else if (operInfo.OperCode == "ldstr")
                    {
                        var strOperData = operInfo.OperData;

                        var ldstrOper = new DCILOperCode_LoadString(operInfo);

                        operInfoList.RemoveAt(operInfoList.Count - 1);

                        //var strDif = new DCStringValueDefine();
                        //strDif.NativeSourcde = this.SourceLines[iCount];
                        //strDif.LabelID = operInfo.LabelID;
                        //strDif.MethodName = currentMethodName;
                        //strDif.LineIndex = iCount;
                        //strDif.EndLineIndex = iCount;
                        if (strOperData[0] == '"')
                        {
                            ldstrOper.IsBinary = false;
                            if (strOperData.Length == 2 && strOperData[1] == '"')
                            {
                                // use string.Empty
                                //this.SourceLines[iCount] = operInfo.LabelID + ":ldsfld     string [" + this.LibName_mscorlib + "]System.String::Empty";
                                operInfo.OperCode = "ldsfld";
                                operInfo.OperData = "string [" + this.LibName_mscorlib + "]System.String::Empty";
                                this.SourceLines[iCount] = operInfo.ToString();
                                operInfoList.Add(operInfo);
                                this.ModifiedCount++;
                                continue;
                            }
                            var strFinalValue = new StringBuilder();
                            GetFinalValue(strOperData, strFinalValue);
                            for (int iCount2 = iCount + 1; iCount2 < lineNum; iCount2++)
                            {
                                string line2 = this.SourceLines[iCount2].Trim();
                                if (line2.Length > 0 && line2[0] == '+')
                                {
                                    //line2 = RemoveComment(line2).Trim();
                                    strOperData = strOperData + Environment.NewLine + line2;
                                    GetFinalValue(line2, strFinalValue);
                                    ldstrOper.EndLineIndex = iCount2;
                                    iCount++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            ldstrOper.OperData = strOperData;
                            ldstrOper.FinalValue = strFinalValue.ToString();
                        }
                        else if (strOperData.StartsWith("bytearray", StringComparison.Ordinal))
                        {
                            _HexCharNum = 0;
                            ldstrOper.IsBinary = true;
                            bool hasFinish = false;
                            var strOperDataLength = strOperData.Length;
                            for (int iCount2 = 9; iCount2 < strOperDataLength; iCount2++)
                            {
                                char c = strOperData[iCount2];
                                if (AddHexChar(c) == false)
                                {
                                    if (c == ')')
                                    {
                                        hasFinish = true;
                                        break;
                                    }
                                    else if (c == '/')
                                    {
                                        break;
                                    }
                                }
                            }
                            if (hasFinish == false)
                            {
                                // 有多行定义
                                for (iCount++; iCount < lineNum; iCount++)
                                {
                                    string line2 = this.SourceLines[iCount];
                                    strOperData = strOperData + Environment.NewLine + line2;
                                    int len2 = line2.Length;
                                    for (int iCount4 = 0; iCount4 < len2; iCount4++)
                                    {
                                        var c = line2[iCount4];
                                        if (AddHexChar(c) == false)
                                        {
                                            if (c == '/')
                                            {
                                                // 开始注释
                                                break;
                                            }
                                            else if (c == ')')
                                            {
                                                // 结束定义字符串
                                                ldstrOper.EndLineIndex = iCount;
                                                goto EndReadStringDefine;
                                            }
                                        }
                                    }
                                }
                            EndReadStringDefine:;
                                ldstrOper.OperData = strOperData;
                            }
                            ldstrOper.OperData = strOperData;
                            var bsText = HexToBinary(GetHexString());
                            if (bsText != null && bsText.Length > 0)
                            {
                                ldstrOper.FinalValue = Encoding.Unicode.GetString(bsText);
                            }
                            else
                            {
                                ldstrOper.FinalValue = string.Empty;
                            }
                        }

                        if (ldstrOper.FinalValue == null)
                        {
                        }
                        if (currentMethod != null &&
                            ldstrOper.FinalValue.StartsWith("DC.NET Protector Options:", StringComparison.OrdinalIgnoreCase))
                        {
                            var strOptions = ldstrOper.FinalValue.Substring("DC.NET Protector Options:".Length);
                            currentMethod.ProtectedOptions = new JieJieSwitchs(strOptions, this.ProtectedOptions);
                            if (currentMethod.ProtectedOptions.AllocationCallStack && currentMethod.ReturnType == "string")
                            {
                                this.SecurityMethods.Add(new Tuple<DCILMethod, int>(currentMethod, iCount));
                                ldstrOper.FinalValue = DateTime.Now.Second.ToString();
                                ldstrOper.OperData = '"' + ldstrOper.FinalValue + '"';
                            }
                        }
                        operInfoList.Add(ldstrOper);
                        this.StringDefines.Add(ldstrOper);
                        if (iCount < lineNum && currentMethod?.Name == ".cctor")
                        {
                            var nextCode = new DCILOperCode(this.SourceLines[iCount + 1], 0);
                            if (nextCode.OperCode == "stsfld")
                            {
                                ldstrOper.IsSetStaticField = true;
                            }
                        }
                    }
                    else if (operInfo.OperCode == "switch")
                    {
                        if (DCUtils.StringEndWithChar(operInfo.OperData, ')') == false)
                        {
                            var strData = new StringBuilder();
                            strData.Append(operInfo.OperData);
                            for (iCount++; iCount < lineNum; iCount++)
                            {
                                var line2 = this.SourceLines[iCount];
                                strData.AppendLine(line2);
                                if (DCUtils.StringEndWithChar(line2, ')'))
                                {
                                    operInfo.EndLineIndex = iCount;
                                    operInfo.OperData = strData.ToString();
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (firstWord == ".try"
                    || firstWord == "catch"
                    || firstWord == "finally"
                    || firstWord == "fault")
                {

                    DCILOperCodeList operInfoList = rootGroup.OperCodes;
                    if (operInfoList.StartLineIndex == 0)
                    {
                        operInfoList.StartLineIndex = iCount;
                    }
                    int groupStartLineIndex = iCount + 1;
                    for (int iCount2 = iCount + 1; iCount2 < lineNum; iCount2++)
                    {
                        if (DCUtils.StringStartWithChar(this.SourceLines[iCount2], '{'))
                        {
                            groupStartLineIndex = iCount2 + 1;
                            break;
                        }
                    }
                    DCILOperCode_Try_Catch_Finally tryBlock = null;
                    if (firstWord == ".try")
                    {
                        tryBlock = new DCILOperCode_Try_Catch_Finally();
                        operInfoList.Add(tryBlock);
                        tryBlock._Try = new DCILObject();
                        tryBlock._Try._Name = ".try";
                        tryBlock._Try.OperCodes = new DCILOperCodeList();
                        tryBlock._Try.Parent = rootGroup;
                        tryBlock._Try.StartLineIndex = iCount;
                        ParseILGroup(tryBlock._Try, groupStartLineIndex);
                        iCount = tryBlock._Try.EndLineIndex;
                    }
                    else if (firstWord == "catch")
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList[operInfoList.Count - 1];
                        var catch2 = new DCILCatchBlock();
                        catch2.ExcpetionTypeName = line.Trim().Substring(6).Trim();
                        catch2.Parent = rootGroup;
                        catch2.StartLineIndex = iCount;
                        if (tryBlock._Catchs == null)
                        {
                            tryBlock._Catchs = new List<DCILCatchBlock>();
                        }
                        tryBlock._Catchs.Add(catch2);
                        ParseILGroup(catch2, groupStartLineIndex);
                        iCount = catch2.EndLineIndex;
                    }
                    else if (firstWord == "fault")
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList[operInfoList.Count - 1];
                        tryBlock._fault = new DCILObject();
                        tryBlock._fault._Name = "fault";
                        tryBlock._fault.OperCodes = new DCILOperCodeList();
                        tryBlock._fault.Parent = rootGroup;
                        tryBlock._fault.StartLineIndex = iCount;
                        ParseILGroup(tryBlock._fault, groupStartLineIndex);
                        iCount = tryBlock._fault.EndLineIndex;
                    }
                    else // finally
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList[operInfoList.Count - 1];
                        tryBlock._Finally = new DCILObject();
                        tryBlock._Finally._Name = "finally";
                        tryBlock._Finally.OperCodes = new DCILOperCodeList();
                        tryBlock._Finally.Parent = rootGroup;
                        tryBlock._Finally.StartLineIndex = iCount;
                        ParseILGroup(tryBlock._Finally, groupStartLineIndex);
                        iCount = tryBlock._Finally.EndLineIndex;
                    }
                }
                else if (firstWord == ".field")
                {
                    for (int iCount9 = iCount + 1; iCount9 < lineNum; iCount9++)
                    {
                        var line9 = this.SourceLines[iCount9];
                        if (IsEmptyLine(line9) == false)
                        {
                            if (line9.Contains(".field"))
                            {
                                ((DCILClass)rootGroup).FieldLineIndexs.Add(iCount);
                            }
                            break;
                        }
                    }
                }
                else if (firstWord == Name_method
                    || firstWord == Name_class
                    || firstWord == Name_property)
                {
                    currentMethodName = null;
                    DCILObject group = null;
                    if (firstWord == Name_method)
                    {
                        group = new DCILMethod();
                        ((DCILMethod)group).ProtectedOptions = this.ProtectedOptions;
                    }
                    else if (firstWord == Name_property)
                    {
                        group = new DCILProperty();
                    }
                    else
                    {
                        group = new DCILClass();
                        this.AllClasses.Add((DCILClass)group);
                    }
                    group.Parent = rootGroup;
                    group.OwnerDocument = this;
                    group.Type = firstWord;
                    this.AllGroups.Add(group);
                    group.StartLineIndex = iCount;
                    group.Level = rootGroup.Level + 1;
                    var strHeader = new StringBuilder();
                    strHeader.Append(line);
                    for (int iCount2 = iCount + 1; iCount2 < lineNum; iCount2++)
                    {
                        var line2 = this.SourceLines[iCount2].Trim();
                        if (line2.Length > 0 && line2[0] == '{')
                        {
                            group.BodyLineIndex = iCount2;
                            break;
                        }
                        strHeader.Append(' ');
                        strHeader.Append(line2);
                    }
                    group.SetHeader(strHeader.ToString());
                    rootGroup.ChildNodes.Add(group);
                    int back = this.ComponentResourceManagers.Count;
                    ParseILGroup(group, group.BodyLineIndex + 1);
                    //group.GetILCodeString();
                    iCount = group.EndLineIndex;
                }
                //else if (firstWord == Name_get)
                //{
                //    if (rootGroup is DCILProperty)
                //    {
                //        ((DCILProperty)rootGroup).HasGetMethod = true;
                //    }
                //}
                //else if (firstWord == Name_set)
                //{
                //    if (rootGroup is DCILProperty)
                //    {
                //        ((DCILProperty)rootGroup).HasSetMethod = true;
                //    }
                //}
                else if (firstWord == Name_mresource)
                {
                    var items = SplitByWhitespace(line);
                    string name = items[items.Count - 1];
                    if (name.EndsWith(EXT_resources))
                    {
                        name = name.Substring(0, name.Length - EXT_resources.Length);
                        var file = new DCILMResource();
                        file._Name = name;
                        file.OwnerDocument = this;
                        file.StartLineIndex = iCount;
                        for (iCount++; iCount < lineNum; iCount++)
                        {
                            var line2 = this.SourceLines[iCount].Trim();
                            if (line2.StartsWith("}"))
                            {
                                file.EndLineIndex = iCount;
                                break;
                            }
                        }
                        //_StrResourceFiles[name] = file;
                        this.ResouceFiles[name] = file;
                    }
                }
                else if (firstWord[0] == '{')
                {
                    // 进入一个代码组
                    currentLevel++;
                }
                else if (firstWord[0] == '}')
                {
                    // 退出一个代码组
                    currentLevel--;
                    if (currentLevel < 0)
                    {
                        rootGroup.EndLineIndex = iCount;
                        return;
                    }
                }
                else if (firstWord == Name_custom)
                {
                    var strHeader = new StringBuilder();
                    bool hasEqualOper = false;
                    int startLineIndex2 = iCount;
                    for (; iCount < lineNum; iCount++)
                    {
                        var line2 = RemoveComment(this.SourceLines[iCount]).Trim();
                        strHeader.Append(line2);
                        if (hasEqualOper == false && line2.IndexOf('=') > 0)
                        {
                            hasEqualOper = true;
                        }
                        if (hasEqualOper)
                        {
                            if (line2.EndsWith(")"))
                            {
                                break;
                            }
                        }
                    }
                    var attr = new DCILCustomAttribute();
                    attr.OwnerDocument = this;
                    attr.SetHeader(strHeader.ToString());
                    attr.StartLineIndex = startLineIndex2;
                    attr.EndLineIndex = iCount;
                    if (rootGroup.CustomAttributes == null)
                    {
                        rootGroup.CustomAttributes = new List<DCILCustomAttribute>();
                    }
                    rootGroup.CustomAttributes.Add(attr);
                }
                else if (firstWord == ".assembly")
                {
                    var words = SplitByWhitespace(line);
                    if (words.Count == 3)
                    {
                        var asmName = words[2];
                        this.ReferenceAssemblies.Add(asmName);
                        if (this.LibName_mscorlib == null)
                        {
                            if (asmName == "System.Runtime" || asmName == "mscorlib")
                            {
                                this.LibName_mscorlib = asmName;
                            }
                        }
                    }
                }
            }
        }

        private int GetGroupStartLineIndex(int startIndex)
        {
            for (int iCount = startIndex; iCount < this.SourceLines.Length; iCount++)
            {
                var line = this.SourceLines[iCount].Trim();
                if (line.Length > 0 && line[0] == '{')
                {
                    return iCount;
                }
            }
            return -1;
        }

        private bool IsEmptyLine(string line)
        {
            if (line.Length > 0)
            {
                int len = line.Length;
                for (int iCount = 0; iCount < len; iCount++)
                {
                    var c = line[iCount];
                    if (c != ' ' && c != '\t')
                    {
                        return false;
                    }
                }
            }
            return true;
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

        private string RemoveComment(string line)
        {
            int index = line.IndexOf("//", StringComparison.Ordinal);
            if (index == 0)
            {
                return string.Empty;
            }
            else if (index > 0)
            {
                return line.Substring(0, index);
            }
            else
            {
                return line;
            }
        }
        private static char[] _HexBuffer = new char[1024];
        private static int _HexCharNum = 0;
        private static bool AddHexChar(char c)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'))
            {
                if (_HexCharNum >= _HexBuffer.Length)
                {
                    var temp = new char[(int)(_HexCharNum * 1.5)];
                    Array.Copy(_HexBuffer, temp, _HexBuffer.Length);
                    _HexBuffer = temp;
                }
                _HexBuffer[_HexCharNum++] = c;
                return true;
            }
            return false;
        }
        private static string GetHexString()
        {
            if (_HexCharNum > 0)
            {
                var result = new string(_HexBuffer, 0, _HexCharNum);
                _HexCharNum = 0;
                return result;
            }
            return null;
        }
        internal static int IndexOfHexChar(char c)
        {
            if (c == ' ')
            {
                return -1;
            }
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }
            else if (c >= 'A' && c <= 'F')
            {
                return c - 'A' + 10;
            }
            else if (c >= 'a' && c <= 'f')
            {
                return c - 'a' + 10;
            }
            return -1;
        }
        internal static byte[] HexToBinary(string hexs)
        {
            if (hexs == null || hexs.Length == 0)
            {
                return null;
            }
            var list = new List<byte>(hexs.Length / 2);
            byte bytCurrentValue = 0;
            bool addFlag = false;
            foreach (var c in hexs)
            {
                int index = IndexOfHexChar(c);
                if (index >= 0)
                {
                    if (addFlag)
                    {
                        bytCurrentValue = (byte)((bytCurrentValue << 4) + index);
                        addFlag = false;
                        list.Add(bytCurrentValue);
                        bytCurrentValue = 0;
                    }
                    else
                    {
                        bytCurrentValue = (byte)index;
                        addFlag = true;
                    }
                }
            }
            return list.ToArray();
        }
        internal static string GetHexString(string srcText)
        {
            var str = new StringBuilder();
            foreach (var c in srcText)
            {
                if (IndexOfHexChar(c) >= 0)
                {
                    str.Append(c);
                }
            }
            return str.ToString();
        }
        internal static string RemoveChars(string text, string chrs)
        {
            var str = new StringBuilder();
            foreach (var c in text)
            {
                if (chrs.IndexOf(c) < 0)
                {
                    str.Append(c);
                }
            }
            return str.ToString();
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
        internal static int IndexOfWhitespace(string text, int startIndex = 0)
        {
            if (text == null || text.Length == 0)
            {
                return -1;
            }
            int len = text.Length;
            for (int iCount = startIndex; iCount < len; iCount++)
            {
                if (IsWhitespace(text[iCount]))
                {
                    return iCount;
                }
            }
            return -1;
        }

        private static string GetFirstWord(string text)
        {
            int len = text.Length;
            for (int iCount = 0; iCount < len; iCount++)
            {
                if (IsWhitespace(text[iCount]) == false)
                {
                    for (int iCount2 = iCount + 1; iCount2 < len; iCount2++)
                    {
                        if (IsWhitespace(text[iCount2]))
                        {
                            return text.Substring(iCount, iCount2 - iCount);
                        }
                    }
                    return text.Substring(iCount);
                }
            }
            return null;
        }
        internal static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
        private static string GetLastWord(string text)
        {
            int len = text.Length;
            for (int iCount = text.Length - 1; iCount >= 0; iCount--)
            {
                var c = text[iCount];
                if (c != ' ' && c != '\t')
                {
                    for (int iCount2 = iCount - 1; iCount2 >= 0; iCount2--)
                    {
                        var c2 = text[iCount2];
                        if (c2 == ' ' || c2 == '\t')
                        {
                            return text.Substring(iCount2 + 1, iCount - iCount2);
                        }
                    }
                    return text.Substring(0, iCount);
                }
            }
            return null;
        }



    }
    internal class DCILOperCodeList : List<DCILOperCode>
    {
        public DCILOperCode AddItem(string labelID, string operCode, string operData = null)
        {
            var item = new DCILOperCode(labelID, operCode, operData);
            this.Add(item);
            return item;
        }
        public DCILOperCode GetByLabelID(string labelID)
        {
            foreach (var item in this)
            {
                if (item.LabelID == labelID)
                {
                    return item;
                }
            }
            return null;
        }
        public int StartLineIndex = 0;

        public void WriteTo(DCILWriter writer)
        {
            foreach (var item in this)
            {
                item.WriteTo(writer);
            }
        }
    }

    /// <summary>
    /// 处理类型的指令
    /// </summary>
    internal class DCILOperCode_HandleClass : DCILOperCode
    {
        //public static bool IsHandleClassOperCode(string code)
        //{
        //    return code == "castclass"
        //        || code == "box" 
        //        || code == "constrained." 
        //        || code == "initobj" 
        //        || code == "isinst" 
        //        || code == "ldelema" 
        //        || code == "ldobj" 
        //        || code == "ldtoken" 
        //        || code == "newarr"
        //        || code == "sizeof";
        //}
        //public DCILOperCode_HandleClass(string lableID, string operCode, string operData)
        //{
        //    this.LabelID = lableID;
        //    this.OperCode = operCode;
        //    this.OperData = operData;
        //}
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
        //public void CacheTypeReference( DCILDocument document )
        //{
        //    this.ClassType = document.CacheTypeReference(this.ClassType);
        //}
        //public string ClassName = null;
        public DCILTypeReference ClassType = null;

        public DCILClass LocalClass = null;
        //public void UpdateLocalClass(Dictionary<string,DCILClass> cls )
        //{
        //    if( this.ClassType != null )
        //    {
        //        this.ClassType.UpdateLocalClass(cls);
        //    }
        //}
    }

    internal class DCILFieldReference
    {
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
            if (this.OwnerType.Name == "__DC20210205._336")
            {

            }
            this.ValueType = document.CacheTypeReference(this.ValueType);
            this.OwnerType = document.CacheTypeReference(this.OwnerType);
            this.ValueType?.UpdateLocalClass(clses);
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
            //if (forLdtoken)
            //{
            //    if (this.ValueType.IsPrimitive == false && this.ValueType.HasLibraryName == false)
            //    {
            //        writer.Write(" class ");
            //    }
            //}
            this.ValueType.WriteTo(writer, true);
            if (this.modreq != null && this.modreq.Length > 0)
            {
                writer.Write(" modreq(");
                writer.Write(this.modreq);
                writer.Write(") ");
            }
            writer.Write(" ");
            this.OwnerType.WriteTo(writer);
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

    }

    /// <summary>
    /// 处理类型字段的指令
    /// </summary>
    internal class DCILOperCode_HandleField : DCILOperCode
    {
        //public static bool IsHandleFieldOperCode( string code )
        //{
        //    return code == "ldfld" || code == "ldflda" || code == "ldsfld" || code == "ldsflda" || code == "stfld" || code == "stsfld";
        //}
        //public DCILOperCode_HandleField( string lableID , string operCode , string fieldName )
        //{
        //    this.LabelID = lableID;
        //    this.OperCode = operCode;
        //    this.OperData = fieldName;
        //}
        public DCILOperCode_HandleField(string labelID, string operCode, DCILReader reader)
        {
            this.LabelID = labelID;
            this.OperCode = operCode;
            this.Value = new DCILFieldReference(reader);
        }
        //public void UpdateLocalInfo( DCILDocument document , Dictionary<string,DCILClass> classes )
        //{
        //    this.Value?.UpdateLocalInfo(document, classes);
        //}
        public DCILFieldReference Value = null;

        public DCILField LocalField = null;
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
        //public static bool IsHandleMethodOperCode( string code )
        //{
        //    return code == "newobj" || code == "call" || code == "callvirt" || code == "ldftn" || code == "ldvirtftn";
        //}
        public DCILOperCode_HandleMethod()
        {

        }
        public DCILInvokeMethodInfo InvokeInfo = null;

        public DCILMethod LocalMethod = null;

        public DCILOperCode_HandleMethod(string code, DCILReader reader)
        {
            this.OperCode = code;
            this.InvokeInfo = new DCILInvokeMethodInfo(reader);
            //this.OperData = this.InvokeInfo.ToString();
        }
        public void CacheInfo(DCILDocument document)
        {
            this.InvokeInfo = document.CacheDCILInvokeMethodInfo(this.InvokeInfo);
        }
        //public void CacheTypeReference( DCILDocument document )
        //{
        //    this.InvokeInfo?.CacheTypeReference(document);
        //}
        public override void WriteTo(DCILWriter writer)
        {
            writer.EnsureNewLine();
            writer.Write(this.LabelID);
            writer.Write(": ");
            if (this.LabelID.Length < 10)
            {
                writer.WriteWhitespace(10 - this.LabelID.Length);
            }
            writer.Write(this.OperCode);
            if (this.InvokeInfo != null)
            {
                writer.WriteWhitespace(Math.Max(1, 10 - this.OperCode.Length));
                this.InvokeInfo.WriteTo(writer);
            }
            //var data = this.GetOperData();
            //if (data != null && data.Length > 0)
            //{
            //    writer.WriteWhitespace(Math.Max(1, 10 - this.OperCode.Length));
            //    writer.Write(data);
            //}
            ////writer.WriteLine();
        }
        public override void WriteOperData(DCILWriter writer)
        {
            if (this.InvokeInfo != null)
            {
                writer.Write(" ");
                this.InvokeInfo.WriteTo(writer);
            }
        }
    }
    internal class DCILOperCode_Try_Catch_Finally : DCILOperCode
    {
        public DCILObject _Try = null;
        public List<DCILCatchBlock> _Catchs = null;
        public DCILObject _Finally = null;
        public DCILObject _fault = null;
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
                writer.WriteLine("//" + this.Text);
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
        ldtoken
    }
    internal class DCILOperCode
    {
        private readonly static Dictionary<string, ILOperCodeType> CodeTypes = null;
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
        }
        /// <summary>
        /// 获得指令类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ILOperCodeType GetOperCodeType(string code)
        {
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
        ///// <summary>
        ///// 判断是否为修饰性指令，必须紧跟在后面的指令之前
        ///// </summary>
        ///// <param name="operCode"></param>
        ///// <returns></returns>
        //public static bool IsPrefixOperCode( string operCode )
        //{
        //    return operCode == "volatile." 
        //        || operCode == "constrained." 
        //        //|| operCode == "cpblk"
        //        || operCode == "unaligned"
        //        || operCode == "tailcall";
        //}

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

            //var data = this.GetOperData();
            //if (data != null && data.Length > 0)
            //{
            //    writer.WriteWhitespace(Math.Max(1, 10 - this.OperCode.Length));
            //    writer.Write(data);
            //}
            //writer.WriteLine();
        }
        public virtual void WriteOperData(DCILWriter writer)
        {
            if (this.OperData != null && this.OperData.Length > 0)
            {
                writer.WriteWhitespace(Math.Max(1, 10 - this.OperCode.Length));
                writer.Write(this.OperData);
            }
        }
        //public virtual string GetOperData()
        //{
        //    return this.OperData;
        //}
        private static int _InstanceIndexCounter = 0;
        public int InstanceIndex = _InstanceIndexCounter++;
        //public bool Enabled = true;
        public DCILOperCodeList OwnerList = null;
        public DCILMethod OwnerMethod = null;
        public string NativeSource = null;
        public string LabelID = null;
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
            else
            {
                reader.Position = pos;
                this.ClassType = DCILTypeReference.Load(reader);
            }
        }
        public string OperType = null;
        public DCILFieldReference FieldReference = null;
        public DCILTypeReference ClassType = null;

        public DCILMemberInfo LocalMemberInfo = null;
        public void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            if (this.FieldReference != null)
            {
                this.FieldReference.UpdateLocalInfo(document, clses);
            }
            else if (this.ClassType != null)
            {
                this.ClassType = document.CacheTypeReference(this.ClassType);
            }
        }
        //public void CacheTypeReference( DCILDocument document )
        //{
        //    if (this.FieldReference != null )
        //    {
        //        this.FieldReference.CacheTypreReference(document);
        //    }
        //    else if( this.ClassType != null )
        //    {
        //        this.ClassType = document.CacheTypeReference(this.ClassType);
        //    }
        //}
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
        //public DCTokenType TokenType = DCTokenType.Type;

        //public DCILTypeReference Field_ValueType = null;

        //public DCILTypeReference ReferencedType = null;
        //public DCILField ReferencedField = null;

        //internal enum DCTokenType
        //{
        //    Type,
        //    Method,
        //    Field
        //}

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
        private static void GetFinalValue(string line, StringBuilder result)
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

            //int posBack = reader.Position;
            //var strOperData = reader.ReadLine()?.Trim();
            //if (strOperData[0] == '"')
            //{
            //    int startPos = posBack;
            //    this.IsBinary = false;
            //    if (strOperData.Length == 2 && strOperData[1] == '"')
            //    {
            //        this.FinalValue = string.Empty;
            //    }
            //    var strFinalValue = new StringBuilder();
            //    GetFinalValue(strOperData, strFinalValue);
            //    while( reader.HasContentLeft())
            //    {
            //        posBack = reader.Position;
            //        var line2 = reader.ReadLine().Trim();
            //        if (line2.Length > 0 && line2[0] == '+')
            //        {
            //            //line2 = RemoveComment(line2).Trim();
            //            //strOperData = strOperData + Environment.NewLine + line2;
            //            GetFinalValue(line2, strFinalValue);
            //        }
            //        else
            //        {
            //            reader.Position = posBack;
            //            break;
            //        }
            //    }
            //    this.OperData = reader.GetSubString(startPos, reader.Position - startPos).Trim();
            //    this.FinalValue = strFinalValue.ToString();
            //}
            //else if (strOperData.StartsWith("bytearray", StringComparison.Ordinal))
            //{
            //    this.IsBinary = true;
            //    reader.Position = posBack;
            //    reader.ReadToChar('(');
            //    var bs = reader.ReadBinaryFromHex();
            //    this.OperData = reader.GetSubString(posBack, reader.Position - posBack).Trim();
            //    if (bs != null && bs.Length > 0)
            //    {
            //        this.FinalValue = Encoding.Unicode.GetString(bs);
            //    }
            //    else
            //    {
            //        this.FinalValue = string.Empty;
            //    }

            //}

        }

        //private static void GetFinalValue(string line, StringBuilder result)
        //{
        //    int index = line.IndexOf('"');
        //    int len = line.Length;
        //    for (int iCount = line.IndexOf('"') + 1; iCount < len; iCount++)
        //    {
        //        var c = line[iCount];
        //        if (c == '\\' && iCount < len - 1)
        //        {
        //            var nc = line[iCount + 1];
        //            iCount++;
        //            switch (nc)
        //            {
        //                case 'r': result.Append('\r'); break;
        //                case 'n': result.Append('\n'); break;
        //                case '\'': result.Append('\''); break;
        //                case '"': result.Append('"'); break;
        //                case '\\': result.Append('\\'); break;
        //                case 'b': result.Append('\b'); break;
        //                case 'f': result.Append('\f'); break;
        //                case 't': result.Append('\t'); break;
        //                default: result.Append(nc); break;
        //            }
        //        }
        //        else if (c == '"')
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            result.Append(c);
        //        }
        //    }
        //}

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
        public DCILTypeReference ValueType = null;

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
            //if( this.ValueType == null )
            //{

            //}
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

        public static DCILGenericParamterList CreateByNativeMethod(System.Reflection.MethodInfo method)
        {
            if (method == null || method.IsGenericMethod == false)
            {
                return null;
            }
            var gs = method.GetGenericArguments();
            var list = new DCILGenericParamterList(gs.Length);
            foreach (var item in gs)
            {
                list.Add(new DCILGenericParamter(item.Name, false));
            }
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

        public int IndexOfName(string name)
        {
            foreach (var item in this)
            {
                if (item.Name == name)
                {
                    return item.Index;
                }
            }
            return -1;
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
        public static bool MatchList(List<DCILGenericParamter> ps1, List<DCILMethodParamter> ps2)
        {
            int len1 = ps1 == null ? 0 : ps1.Count;
            int len2 = ps2 == null ? 0 : ps2.Count;
            if (len1 != len2)
            {
                return false;
            }
            return true;
        }
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
        public static bool MatchList(List<DCILTypeReference> ps1, List<DCILMethodParamter> ps2)
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
        public bool CheckConstraints(DCILTypeReference type)
        {
            if (this.Constraints != null && this.Constraints.Length > 0)
            {
                foreach (var item in this.Constraints)
                {

                }
            }
            return false;
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
        public bool NewGenerate = false;

        private static readonly Dictionary<Type, DCILClass> _NativeClasses = new Dictionary<Type, DCILClass>();
        public static DCILClass CreateByNativeType(Type nativeType, DCILDocument document)
        {
            if (nativeType == null)
            {
                return null;
            }
            DCILClass result = null;
            if (_NativeClasses.TryGetValue(nativeType, out result) == false)
            {
                result = new DCILClass(nativeType, document);
                _NativeClasses[nativeType] = result;
            }
            return result;
        }

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
            //var nativeMethods = nativeType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //if( nativeMethods != null && nativeMethods.Length > 0 )
            //{
            //    this.ChildNodes = new DCILObjectList();
            //    foreach( var item in nativeMethods)
            //    {
            //        if( item.IsAssembly || item.IsPrivate)
            //        {
            //            continue;
            //        }
            //        if( item.IsVirtual ||item.IsAbstract )
            //        {

            //            this.ChildNodes.Add(new DCILMethod(item, document));
            //        }
            //    }
            //}
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
            this.NewGenerate = true;
        }

        public DCILClass(DCILObject parent, DCILReader reader)
        {
            this.Parent = parent;
            this.Load(reader);
        }
        public override void Load(DCILReader reader)
        {
            LoadHeader(reader);
            LoadContent(reader);

        }
        //public bool IsImport
        //{
        //    get
        //    {
        //        return base.HasStyle("import");
        //    }
        //}
        //public bool IsInterface
        //{
        //    get
        //    {
        //        return base.HasStyle("interface");
        //    }
        //}



        //private static readonly Dictionary<Type, List<DCILMethod>> _NativeMethods
        //    = new Dictionary<Type, List<DCILMethod>>();
        //private void UpdateMethodOverrideSourceFromNativeType( Type t , DCILDocument document , Dictionary<string, DCILMethod> list)
        //{
        //    if( t == null )
        //    {
        //        return;
        //    }
        //    List<DCILMethod> nativeMethods = null;
        //    if( _NativeMethods.TryGetValue( t , out nativeMethods) == false )
        //    {
        //        //var cls = DCILClass.CreateByNativeType(t, document);
        //        nativeMethods = new List<DCILMethod>();
        //        _NativeMethods[t] = nativeMethods;
        //        var nms = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
        //        foreach (System.Reflection.MethodInfo nm in nms)
        //        {
        //            if( nm.IsFamily || nm.IsPrivate)
        //            {
        //                continue;
        //            }
        //            if (nm.IsVirtual || nm.IsAbstract)
        //            {
        //                if (nm.Name != null && nm.Name.IndexOf('.') < 0)
        //                {
        //                    var m = new DCILMethod(nm, document);
        //                    nativeMethods.Add(m);
        //                }
        //            }
        //        }
        //    }
        //    if( nativeMethods != null && nativeMethods.Count >  0 )
        //    {
        //        foreach( var m in nativeMethods)
        //        {
        //            list[m.GetSignString()] = m;
        //        }
        //    }
        //}
        //public Dictionary<string, DCILMethod> _UpdateMethodOverrideSource_Map = null;

        //public void UpdateMethodOverrideSource(
        //    Dictionary<string,DCILMethod> list ,
        //    DCILDocument document ,
        //    Dictionary<string,DCILTypeReference> classGenericeValues = null )
        //{
        //    if( this._UpdateMethodOverrideSource_Map == null )
        //    {
        //        this._UpdateMethodOverrideSource_Map = new Dictionary<string, DCILMethod>();
        //        var baseTypes = new List<DCILTypeReference>();
        //        if (this.ImplementsInterfaces != null && this.ImplementsInterfaces.Count > 0)
        //        {
        //            baseTypes.AddRange(this.ImplementsInterfaces);
        //        }
        //        if( this.BaseType != null )
        //        {
        //            baseTypes.Add(this.BaseType);
        //        }
        //        foreach (var item in baseTypes)
        //        {
        //            if (item.LocalClass != null)
        //            {
        //                item.LocalClass.UpdateMethodOverrideSource(
        //                    this._UpdateMethodOverrideSource_Map,
        //                    document,
        //                    item.CreateGenericeRuntimeValues());
        //            }
        //            else
        //            {
        //                UpdateMethodOverrideSourceFromNativeType(
        //                    item.SearchNativeType(),
        //                    document,
        //                    this._UpdateMethodOverrideSource_Map);
        //            }
        //        }
        //        if (this._UpdateMethodOverrideSource_Map.Count > 0)
        //        {
        //            foreach (var item in this.ChildNodes)
        //            {
        //                if (item is DCILMethod)
        //                {
        //                    var method = (DCILMethod)item;
        //                    if (method.IsVirtual && method.IsInstance)
        //                    {
        //                        this._UpdateMethodOverrideSource_Map.TryGetValue(
        //                            method.GetSignString( classGenericeValues ),
        //                            out method.OverrideSource);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (list != null)
        //    {
        //        if (this._UpdateMethodOverrideSource_Map.Count > 0)
        //        {
        //            foreach (var item in this._UpdateMethodOverrideSource_Map)
        //            {
        //                list[item.Key] = item.Value;
        //            }
        //        }
        //        foreach (var item in this.ChildNodes)
        //        {
        //            if (item is DCILMethod)
        //            {
        //                var m = (DCILMethod)item;
        //                if (m.IsInstance)
        //                {
        //                    if (m.IsVirtual || m.IsNewslot || m.IsAbstract)
        //                    {
        //                        list[m.GetSignString( classGenericeValues)] = m;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        private static readonly DCILProperty[] _EmptyArray = new DCILProperty[0];
        private DCILProperty[] _Properties = null;
        public DCILProperty[] GetProperties()
        {
            if (this._Properties == null)
            {
                this._Properties = this.ChildNodes.GetSubArray<DCILProperty>();
                if (this._Properties == null)
                {
                    this._Properties = _EmptyArray;
                }
            }
            return this._Properties;
        }

        private static readonly DCILMethod[] _EmptyArray2 = new DCILMethod[0];
        private DCILMethod[] _Methods = null;
        public DCILMethod[] GetMethods()
        {
            if (this._Methods == null)
            {
                this._Methods = this.ChildNodes.GetSubArray<DCILMethod>();
                if (this._Methods == null)
                {
                    this._Methods = _EmptyArray2;
                }
            }
            return this._Methods;
        }

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

            //foreach ( var item in this.ChildNodes )
            //{
            //    item.CusotmAttributesCacheTypeReference(document);
            //    if( item is DCILMethod )
            //    {
            //        var method = (DCILMethod)item;
            //        document.CacheTypeReference(method.ReturnTypeInfo);
            //        DCILGenericParamter.CacheTypeReference(document, method.GenericParamters);

            //        //if( method.GenericParamters != null && method.GenericParamters.Count > 0 )
            //        //{
            //        //    foreach (var item2 in method.GenericParamters )
            //        //    {
            //        //        if (item2.Constraints != null)
            //        //        {
            //        //            for (int iCount = 0; iCount < item2.Constraints.Length; iCount++)
            //        //            {
            //        //                item2.Constraints[iCount] = document.CacheTypeReference(item2.Constraints[iCount]);
            //        //            }
            //        //        }
            //        //    }
            //        //}
            //        DCILMethodParamter.CacheTypeReference(document, method.Parameters);

            //        //if( method.Parameters != null && method.Parameters.Count > 0 )
            //        //{
            //        //    foreach( var item2 in method.Parameters )
            //        //    {
            //        //        item2.ValueType = document.CacheTypeReference(item2.ValueType);
            //        //    }
            //        //}
            //        if( method.OperCodes != null )
            //        {
            //            CachedTypeReference(method.OperCodes, document);
            //        }
            //    }
            //    else if( item is DCILProperty )
            //    {
            //        var p = (DCILProperty)item;
            //        p.ValueType = document.CacheTypeReference(p.ValueType);
            //        p.Method_Get?.CacheTypeReference(document);
            //        p.Method_Set?.CacheTypeReference(document);
            //        DCILMethodParamter.CacheTypeReference(document, p.Parameters);
            //        //if( p.Parameters != null )
            //        //{
            //        //    foreach( var p2 in p.Parameters )
            //        //    {
            //        //        p2.ValueType = document.CacheTypeReference(p2.ValueType);
            //        //    }
            //        //}
            //    }
            //    else if( item is DCILField )
            //    {
            //        var f = (DCILField)item;
            //        f.ValueType = document.CacheTypeReference(f.ValueType);
            //    }
            //    else if( item is DCILEvent )
            //    {
            //        var eve = (DCILEvent)item;
            //        eve.EventHandlerType = document.CacheTypeReference(eve.EventHandlerType);
            //        eve.Method_Addon?.CacheTypeReference(document);
            //        eve.Method_Removeon?.CacheTypeReference(document);
            //    }
            //}

        }
        //private void CachedTypeReference( DCILOperCodeList codes , DCILDocument document )
        //{
        //    foreach(var item in codes )
        //    {
        //        if( item is DCILOperCode_HandleMethod)
        //        {
        //            var hm = (DCILOperCode_HandleMethod)item;
        //            hm.CacheTypeReference(document);
        //        }
        //        else if( item is DCILOperCode_HandleClass)
        //        {
        //            var hc = (DCILOperCode_HandleClass)item;
        //            hc.CacheTypeReference(document);
        //        }
        //        else if( item is DCILOperCode_HandleField )
        //        {
        //            ((DCILOperCode_HandleField)item).CacheTypreReference(document);
        //        }
        //        else if( item is DCILOperCode_LdToken)
        //        {
        //            ((DCILOperCode_LdToken)item).CacheTypeReference(document);
        //        }
        //        else if( item is DCILOperCode_Try_Catch_Finally)
        //        {
        //            var tcf = (DCILOperCode_Try_Catch_Finally)item;
        //            if( tcf._Try != null && tcf._Try.OperCodes != null )
        //            {
        //                CachedTypeReference(tcf._Try.OperCodes , document);
        //            }
        //            if( tcf._Catchs != null )
        //            {
        //                foreach( var item2 in tcf._Catchs)
        //                {
        //                    if( item2.OperCodes != null )
        //                    {
        //                        CachedTypeReference(item2.OperCodes, document);
        //                    }
        //                }
        //            }
        //            if( tcf._Finally != null && tcf._Finally.OperCodes != null )
        //            {
        //                CachedTypeReference(tcf._Finally.OperCodes, document);
        //            }
        //            if (tcf._fault != null && tcf._fault.OperCodes != null)
        //            {
        //                CachedTypeReference(tcf._fault.OperCodes, document);
        //            }
        //        }
        //    }
        //}
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
                    default:
                        this.ChildNodes.Add(new DCILUnknowObject(strWord, reader));
                        break;
                }
            }
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
                if (this.Parent == null)
                {
                    return this._Name;
                }
                else
                {
                    var str = new StringBuilder();
                    str.Append(this._Name);
                    var p = this.Parent;
                    while (p != null)
                    {
                        str.Insert(0, '/');
                        if (p.Name == null || p.Name.Length == 0)
                        {

                        }
                        str.Insert(0, p.Name);
                        p = p.Parent;
                    }
                    return str.ToString();
                }
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".class ");
            base.WriteStyles(writer);
            writer.Write(" " + this._Name);
            this.GenericParamters?.WriteTo(writer);

            if (this.BaseType != null)
            {
                writer.Write(" extends ");
                this.BaseType.WriteTo(writer);
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
        internal override void SetHeader(string strHeader)
        {
            this.Header = strHeader;
            var items = DCILDocument.SplitByWhitespace(strHeader);
            for (int itemCount = 0; itemCount < items.Count; itemCount++)
            {
                if (items[itemCount] == "extends" && itemCount > 0)
                {
                    this._Name = items[itemCount - 1];
                    //this.BaseTypeName = items[itemCount + 1];
                    return;
                }
            }
            this._Name = items[items.Count - 1];


            //int index2 = items.IndexOf("implements");
            //if (index2 > 0)
            //{
            //    this.ImplementsInterfaces = new List<string>();
            //    for (int iCount = index2; iCount < items.Count; iCount++)
            //    {
            //        this.ImplementsInterfaces.Add(items[iCount]);
            //    }
            //}
            //if (strHeader.Contains("WriterControl") && this.IsPublic == false )
            //{

            //}
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
                    if (tn != null && tn.Contains("GeneratedCodeAttribute")
                        || tn.Contains("DebuggerNonUserCodeAttribute")
                        || tn.Contains("CompilerGeneratedAttribute"))
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
        private void GetFullName(StringBuilder str)
        {
            str.Append(this._Name);
            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
            {
                str.Append('<');
                for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                {

                    var p = this.GenericParamters[iCount];
                }
                str.Append('>');
            }
        }
        public bool IsValueType
        {
            get
            {
                return this.BaseType?.Name == "System.ValueType";
            }
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
    //public class ObfuscationAttributeValue
    //{
    //    public bool StripAfterObfuscation = true;
    //    public bool Exclude = true;
    //    public bool ApplyToMembers = true;
    //    public string Feature = "all";
    //}
    internal class DCILEditorAttribute : DCILCustomAttribute
    {
        public const string ConstAttributeTypeName = "System.ComponentModel.EditorAttribute";
        public DCILEditorAttribute()
        {
        }
        public override bool UpdateValuesForRename(RenameMapInfo map)
        {
            bool changed = false;
            if (this.Values != null)
            {
                foreach (var item in this.Values)
                {
                    if (item.Value is string)
                    {
                        var newName = map.GetNewClassName((string)item.Value);
                        if (newName != null)
                        {
                            item.Value = newName;
                            changed = true;
                        }
                    }
                }
            }
            if (changed)
            {
                base.BinaryValue = DCILCustomAttributeValue.GetBinaryValue(this.Values, this.InvokeInfo);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    internal class DCILTypeConverterAttribute : DCILCustomAttribute
    {
        public const string ConstAttributeTypeName = "System.ComponentModel.TypeConverterAttribute";

        public DCILTypeConverterAttribute()
        {
        }
        public override bool UpdateValuesForRename(RenameMapInfo map)
        {
            if (this.Values != null && this.Values.Length == 1)
            {
                var newName = map.GetNewClassName((string)this.Values[0].Value);
                if (newName != null)
                {
                    this.Values[0].Value = newName;
                    this.BinaryValue = DCILCustomAttributeValue.GetBinaryValue(this.Values, this.InvokeInfo);
                    return true;
                }
            }
            return false;
        }
        public string ConvertTypeName
        {
            get
            {
                if (this.Values != null && this.Values.Length == 1)
                {
                    return (string)this.Values[0].Value;
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
            foreach (var item in base.Values)
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

        public bool StripAfterObfuscation = true;
        public bool Exclude = true;
        public bool ApplyToMembers = true;
        public string Feature = "all";
        public override bool UpdateValuesForRename(RenameMapInfo map)
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
        public string EnumTypeName = null;
        public bool MatchCtorParamter = false;
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
                        WriteUTF8String(writer, (string)item.Value);
                    }
                    else if (item.ElementType == DCILElementType.Enum)
                    {
                        WriteUTF8String(writer, item.EnumTypeName);
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
                    if (item.IsBoxed)
                    {
                        writer.Write((byte)DCILElementType.Boxed);
                    }
                    writer.Write((byte)item.ElementType);

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

        public static List<DCILCustomAttributeValue> ParseValues(byte[] bsValue, DCILInvokeMethodInfo invokeInfo)
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
                }
                for (int iCount = 0; iCount < invokeInfo.Paramters.Count; iCount++)
                {
                    var av = new DCILCustomAttributeValue();
                    av.MatchCtorParamter = true;
                    var p = invokeInfo.Paramters[iCount];
                    av.Name = p.ValueType.Name;
                    if (p.ValueType.Name == "System.Type")
                    {
                        av.Value = ReadUTF8String(reader);
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
                        av.Value = ReadAttributeValue(reader, et);
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
                    av.MatchCtorParamter = false;
                    av.Flag = reader.ReadByte();
                    av.ElementType = (DCILElementType)reader.ReadByte();
                    if (av.ElementType == DCILElementType.Enum)
                    {
                        av.EnumTypeName = ReadUTF8String(reader);
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
                    av.Value = ReadAttributeValue(reader, av.ElementType);
                    result.Add(av);
                }
            }
            reader.Close();
            return result;
        }

        private static Dictionary<Type, DCILElementType> _TypeMaps = null;
        protected static void WriteAttributeValue(BinaryWriter writer, DCILElementType type, object Value)
        {
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
        protected static object ReadAttributeValue(BinaryReader reader, DCILElementType type)
        {
            switch (type)
            {
                case DCILElementType.Boolean: return reader.ReadByte() == 1;
                case DCILElementType.I1: return (sbyte)reader.ReadByte();
                case DCILElementType.U1: return reader.ReadByte();
                case DCILElementType.Char: return (char)reader.ReadInt16();
                case DCILElementType.I2: return reader.ReadInt16();
                case DCILElementType.U2: return reader.ReadUInt16();
                case DCILElementType.I4: return reader.ReadInt32();
                case DCILElementType.U4: return reader.ReadUInt32();
                case DCILElementType.I8: return reader.ReadInt64();
                case DCILElementType.U8: return reader.ReadUInt64();
                case DCILElementType.R4: return reader.ReadSingle();
                case DCILElementType.R8: return reader.ReadDouble();
                case DCILElementType.String: return ReadUTF8String(reader);
                case DCILElementType.Enum: return reader.ReadInt32();
                case DCILElementType.Type: return ReadUTF8String(reader);
                default:
                    throw new NotSupportedException(type.ToString());
            }
            return null;
        }

        protected static void WriteUTF8String(System.IO.BinaryWriter writer, string text)
        {
            if (text == null)
            {
                writer.Write((byte)0xff);
            }
            if (text.Length == 0)
            {
                writer.Write((byte)0);
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
        public string AttributeTypeName = null;

        public DCILInvokeMethodInfo InvokeInfo = null;

        public byte[] BinaryValue = null;

        private static readonly DCILCustomAttributeValue[] _EmptyValues = new DCILCustomAttributeValue[0];
        private DCILCustomAttributeValue[] _Values = null;

        public DCILCustomAttributeValue[] Values
        {
            get
            {
                if (_Values == null)
                {
                    var list = DCILCustomAttributeValue.ParseValues(this.BinaryValue, this.InvokeInfo);
                    if (list == null || list.Count == 0)
                    {
                        this._Values = _EmptyValues;
                    }
                    else
                    {
                        this._Values = list.ToArray();
                    }
                }
                if (this._Values == _EmptyValues)
                {
                    return null;
                }
                else
                {
                    return this._Values;
                }
            }
        }
        /// <summary>
        /// 为重命名而更新属性值
        /// </summary>
        /// <param name="map"></param>
        public virtual bool UpdateValuesForRename(RenameMapInfo map)
        {
            if (this.AttributeTypeName == "System.ComponentModel.DefaultValueAttribute")
            {
                return false;
            }
            if (this.Values == null)
            {
                return false;
            }
            bool changed = false;
            foreach (var item in this.Values)
            {
                if (item.ElementType == DCILCustomAttributeValue.DCILElementType.System_Type)
                {
                    var newName = map.GetNewClassName((string)item.Value);
                    if (newName != null)
                    {
                        changed = true;
                        item.Value = newName;
                    }
                }
            }
            if (changed)
            {
                this.BinaryValue = DCILCustomAttributeValue.GetBinaryValue(this.Values, this.InvokeInfo);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void UpdateLocalInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {

        }
        //public DCILCustomAttribute(DCILObject parent, DCILReader reader)
        //{
        //    this.Parent = parent;
        //    this.InvokeInfo = new DCILInvokeMethodInfo(reader);
        //    reader.ReadAfterChar('=');
        //    this.Value = reader.ReadBinaryFromHex();
        //    if( this.InvokeInfo?.OwnerType?.Name == "System.Reflection.ObfuscationAttribute")
        //    {
        //        var values = this.ParseValues(reader.Document);
        //        this.ObfuscationSettings = new ObfuscationAttributeValue();
        //        foreach( var item in values )
        //        {
        //            switch (item.Name)
        //            {
        //                case "StripAfterObfuscation": this.ObfuscationSettings.StripAfterObfuscation = (bool)item.Value;break;
        //                case "Exclude": this.ObfuscationSettings.Exclude = (bool)item.Value;break;
        //                case "ApplyToMembers": this.ObfuscationSettings.ApplyToMembers = (bool)item.Value;break;
        //                case "Feature": this.ObfuscationSettings.Feature = (string)item.Value;break;
        //            }
        //        }
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
        internal override void SetHeader(string text)
        {
            this.Header = text;
            int indexEqual = text.IndexOf('=');
            if (indexEqual > 0)
            {
                this.HexValue = DCILDocument.GetHexString(text.Substring(indexEqual + 1));
            }
            var index = text.IndexOf("::.ctor", StringComparison.Ordinal);
            for (int iCount = index; iCount >= 0; iCount--)
            {
                if (DCILDocument.IsWhitespace(text[iCount]))// char.IsWhiteSpace( text[iCount]))
                {
                    this._Name = text.Substring(iCount, index - iCount);
                    break;
                }
            }
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
                    //case DCILClass.TagName:
                    //    if( this.ChildNodes == null )
                    //    {
                    //        this.ChildNodes = new List<DCILObject>();
                    //    }
                    //    var cls = new DCILClass();
                    //    cls.LoadHeader(reader);
                    //    if( cls.Name != null  && cls.Name.Length > 0 )
                    //    {
                    //        DCILClass oldCls = null;
                    //        if( this._ClassMap.TryGetValue( cls.Name , out oldCls))
                    //        {
                    //            oldCls.LoadContent(reader);
                    //        }
                    //        else
                    //        {
                    //            cls.LoadContent(reader);
                    //            this.ChildNodes.Add(cls);
                    //            this._ClassMap[cls.Name] = cls;
                    //        }

                    //    }
                    //    break;
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
            return ".assembly extern " + this._Name;
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
        //public DCILUnknowObject( string name , string data )
        //{
        //    this.Name = name;
        //    this.Data = data;
        //}
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

        //private static readonly HashSet<string> _AllowStyles = null;
        //static DCILMemberInfo()
        //{
        //    _AllowStyles = new HashSet<string>();
        //    _AllowStyles.Add("abstract");
        //    _AllowStyles.Add("assembly");
        //    _AllowStyles.Add("compilercontrolled");
        //    _AllowStyles.Add("famandassem");
        //    _AllowStyles.Add("family");
        //    _AllowStyles.Add("famorassem");
        //    _AllowStyles.Add("final");
        //    _AllowStyles.Add("hidebysig");
        //    _AllowStyles.Add("newslot");
        //    _AllowStyles.Add("private");
        //    _AllowStyles.Add("public");
        //    _AllowStyles.Add("rtspecialname");
        //    _AllowStyles.Add("specialname");
        //    _AllowStyles.Add("static");
        //    _AllowStyles.Add("virtual");
        //    _AllowStyles.Add("strict");
        //    _AllowStyles.Add("pinvokeimpl");
        //}


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
        public void AddStyles(string styleSpilitByWhitespace)
        {
            if (styleSpilitByWhitespace != null && styleSpilitByWhitespace.Length > 0)
            {
                if (this.Styles == null)
                {
                    this.Styles = new List<string>();
                }
                foreach (var item in styleSpilitByWhitespace.Split(' '))
                {
                    if (item.Length > 0)
                    {
                        this.Styles.Add(item);
                    }
                }
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

        protected void SetStyles(List<string> words, int itemCount)
        {
            if (words != null)
            {
                if (this.Styles == null)
                {
                    this.Styles = new List<string>();
                }
                for (int iCount = 0; iCount < itemCount && iCount < words.Count; iCount++)
                {
                    this.Styles.Add(words[iCount]);
                }
            }
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
        public int IndexOf<T>(string name) where T : DCILObject
        {
            int len = this.Count;
            for (int iCount = 0; iCount < len; iCount++)
            {
                if (this[iCount] is T && this[iCount].Name == name)
                {
                    return iCount;
                }
            }
            return -1;
        }
        public T[] GetSubArray<T>() where T : DCILObject
        {
            var list = new List<T>();
            foreach (var item in this)
            {
                if (item is T)
                {
                    list.Add((T)item);
                }
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            else
            {
                return null;
            }
        }
    }

    internal class DCILObject
    {
        private static int _InstanceIndexCounter = 0;
        public int InstanceIndex = _InstanceIndexCounter++;
        public DCILDocument OwnerDocument = null;
        internal string _Name = null;
        public string Name
        {
            get
            {
                return this._Name;
            }
        }
        public string Type = null;
        public string Header = null;
        public virtual void Load(DCILReader reader)
        {

        }
        public virtual void WriteTo(DCILWriter writer)
        {

        }

        internal virtual void SetHeader(string strHeader)
        {
            this.Header = strHeader;
        }
        public int StartLineIndex = 0;
        public int BodyLineIndex = 0;
        public int EndLineIndex = 0;
        public int LineSpan
        {
            get
            {
                return this.EndLineIndex - this.StartLineIndex;
            }
        }
        public DCILOperCodeList OperCodes = null;
        public List<T> GetAllOperCodes<T> () where T : DCILOperCode
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

        public string GetILCodeString()
        {
            var str = new StringBuilder();
            if (this.OperCodes != null)
            {
                this.OperCodes.WriteTo(new DCILWriter(str));
            }
            string txt = str.ToString();
            System.Diagnostics.Debug.WriteLine(txt);
            return txt;
        }
        public DCILObjectList ChildNodes = null;
        public DCILObject Parent = null;
        public DCILObfuscationAttribute ObfuscationSettings = null;


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
    internal class DCILOperCodeBlock : DCILObject
    {
        public DCILOperCodeBlock()
        {
            base.OperCodes = new DCILOperCodeList();
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
            var words = DCILDocument.SplitByWhitespace(strHeader);
            this._Name = words[words.Count - 1];
            this.IsCil = words.Contains("cil");
            this.DataType = reader.ReadWord();
            if (this.DataType == "bytearray")
            {
                this.Value = reader.ReadBinaryFromHex();
            }
            else
            {
                this.Value = reader.ReadLineTrimRemoveComment();
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".data cil " + this._Name + " = " + this.DataType);
            if (this.DataType == "bytearray")
            {
                writer.Write("(");
                writer.WriteHexs((byte[])this.Value);
                writer.WriteLine(")");
            }
            else
            {
                writer.WriteLine(Convert.ToString(this.Value));
            }
        }
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
    //internal enum DCILPointerMode
    //{
    //    None,
    //    ManagedPointer,
    //    UnmanagedPointer
    //}
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
            AddPrimitiveType("uint8", typeof(byte));
            AddPrimitiveType("int8", typeof(sbyte));
            AddPrimitiveType("char", typeof(char));
            AddPrimitiveType("bool", typeof(bool));
            AddPrimitiveType("int16", typeof(short));
            AddPrimitiveType("uint16", typeof(ushort));
            AddPrimitiveType("int32", typeof(int));
            AddPrimitiveType("uint32", typeof(uint));
            AddPrimitiveType("int64", typeof(long));
            AddPrimitiveType("uint64", typeof(ulong));
            AddPrimitiveType("float32", typeof(float));
            AddPrimitiveType("float64", typeof(double));
            AddPrimitiveType("string", typeof(string));
            AddPrimitiveType("object", typeof(object));
            AddPrimitiveType("void", typeof(void));
            AddPrimitiveType("lpwstr", typeof(string));

            Type_Void = _PrimitiveTypes["void"];
            Type_String = _PrimitiveTypes["string"];
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

        private static void AddPrimitiveType(string name, Type nativeType)
        {
            var t = new DCILTypeReference(name, DCILTypeMode.Primitive);
            t._NativeType = nativeType;
            _PrimitiveTypes[name] = t;
            _Cache_CreateByNativeType[nativeType] = t;
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
        //public static bool LoadMarshalType(string firstWord, DCILReader reader , out string result )
        //{
        //    result = null;
        //    if (firstWord == "marshal")
        //    {
        //        reader.ReadAfterChar('(');
        //        result = reader.ReadAfterChar(')').Trim();
        //        return true ;
        //    }
        //    return false ;
        //}

        public static DCILTypeReference LoadText(string text)
        {
            var reader = new DCILReader(text, null);
            return Load(reader.ReadWord(), reader);
        }
        private static readonly Dictionary<Type, DCILTypeReference> _Cache_CreateByNativeType = new Dictionary<Type, DCILTypeReference>();
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
            //int pos = 0;
            //if( reader != null )
            //{
            //    pos = reader.Position;
            //}
            //var fixName = SplitEndFix(firstWord, reader);

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
            //var item = SplitEndFix(word, null);
            //if(PrimitiveTypeNames.Contains(item.Item1))
            //{
            //    return true;
            //}
            return false;
        }
        private static readonly Dictionary<string, Type> _NativeTypes = new Dictionary<string, Type>();
        private static System.Reflection.Assembly[] _LocalAssemblies = null;
        public DCILTypeReference Clone()
        {
            return (DCILTypeReference)this.MemberwiseClone();
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
                if (this.LibraryName != null && this.LibraryName.Length > 0)
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
                        if( File.Exists( asmFileName ) == false )
                        {
                            asmFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.LibraryName + ".dll");
                        }
                        if (File.Exists(asmFileName))
                        {
                            try
                            {
                                var asm = System.Reflection.Assembly.LoadFile(asmFileName);
                                _LocalAssemblies = null;
                                findAsm = true;
                                if (asm != null)
                                {
                                    result = asm.GetType(this.Name, false, false);
                                }
                            }
                            catch (System.Exception ext)
                            {

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
            clses.TryGetValue(this.Name, out this.LocalClass);
        }

        public DCILClass LocalClass = null;
        public Dictionary<string, string> CreateGenericeRuntimeValues()
        {
            if (this.GenericParamters != null && this.GenericParamters.Count > 0
                && this.LocalClass != null && this.LocalClass.GenericParamters != null
                && this.LocalClass.GenericParamters.Count == this.GenericParamters.Count)
            {
                var dic = new Dictionary<string, string>(this.GenericParamters.Count);
                for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                {
                    dic[this.LocalClass.GenericParamters[iCount].Name] = this.GenericParamters[iCount].Name;
                }
                return dic;
            }
            else
            {
                return null;
            }
        }
        private DCILClass NativeClass = null;

        public DCILClass GetClassInfo(DCILDocument document)
        {
            if (this.LocalClass != null)
            {
                return this.LocalClass;
            }
            if (this.NativeClass == DCILClass.Empty)
            {
                return null;
            }
            var t = SearchNativeType();
            if (t != null)
            {
                this.NativeClass = new DCILClass(t, document);
                return this.NativeClass;
            }
            else
            {
                this.NativeClass = DCILClass.Empty;
                return null;
            }
        }

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
        public bool CanConvertTo(DCILTypeReference type)
        {
            if (this.EqualsValue(type))
            {
                return true;
            }

            var cls = this.LocalClass;
            return true;
        }
        //public DCILPointerMode PointerMode = DCILPointerMode.None;
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

        //public DCILGenericParamter ReferenceGenericParamter = null;

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
        //public DCILTypeReference( List<string> words , int length )
        //{

        //}
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
            else;
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
        //private bool ReadArrayAndPointer( DCILReader reader )
        //{
        //    bool result = false;
        //    if (reader != null)
        //    {
        //        while (reader.HasContentLeft())
        //        {
        //            var c = reader.PeekContentChar();
        //            if (c == '*' && this.PointerMode == DCILPointerMode.None && this.NumOfArrayDimensions == 0)
        //            {
        //                this.PointerMode = DCILPointerMode.UnmanagedPointer;
        //                reader.ReadContentChar();
        //                result = true;
        //            }
        //            else if (c == '&' && this.PointerMode == DCILPointerMode.None && this.NumOfArrayDimensions == 0)
        //            {
        //                this.PointerMode = DCILPointerMode.ManagedPointer;
        //                reader.ReadContentChar();
        //                result = true;
        //            }
        //            else if (c == '[')
        //            {
        //                int pos = reader.Position;
        //                reader.ReadContentChar();
        //                if (reader.PeekContentChar() == ']')
        //                {
        //                    this.NumOfArrayDimensions++;
        //                    reader.ReadContentChar();
        //                    result = true;
        //                }
        //                else
        //                {
        //                    // 反悔了
        //                    reader.Position = pos;
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// 数组维度
        ///// </summary>
        //public int NumOfArrayDimensions = 0;

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
        public void WriteToForSignString(DCILWriter writer, DCILGenericParamterList gps = null, int stackLevel = 0)
        {
            if (stackLevel > 20)
            {

            }
            //if(this.ReferenceGenericParamter!=null
            //    && this.ReferenceGenericParamter.RuntimeTypeName !=null
            //    && this.ReferenceGenericParamter.RuntimeTypeName.Length>0)
            //{
            //    writer.Write(this.ReferenceGenericParamter.RuntimeTypeName);
            //    return;
            //}
            switch (this.Mode)
            {
                case DCILTypeMode.Primitive:
                    writer.Write(this.Name);
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
                                        gp.RuntimeType.WriteToForSignString(writer, gps);
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
                                        gp.RuntimeType.WriteToForSignString(writer, gps, stackLevel + 1);
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
                    writer.Write(this.Name);
                    break;
                case DCILTypeMode.ValueType:
                    writer.Write(this.Name);
                    break;
                case DCILTypeMode.NotSpecify:
                    writer.Write(this.Name);
                    break;
                case DCILTypeMode.Native:
                    writer.Write("native " + this.Name);
                    break;
                case DCILTypeMode.Unsigned:
                    writer.Write("unsigned " + this.Name);
                    break;
            }

            //if (this.Mode == DCILTypeMode.Primitive)
            //{
            //    writer.Write(this.Name);
            //}
            //else if (this.Mode == DCILTypeMode.GenericTypeInMethodDefine 
            //    || this.Mode == DCILTypeMode.GenericTypeInTypeDefine)
            //{
            //    DCILTypeReference rn = null;
            //    if (classGenericeTypeValues != null
            //        && classGenericeTypeValues.TryGetValue(this.Name, out rn))
            //    {
            //        rn.WriteToForSignString(writer, null);
            //    }
            //    else
            //    {
            //        writer.Write(this.Name);
            //    }
            //}
            //else if (this.Mode == DCILTypeMode.Class 
            //    || this.Mode == DCILTypeMode.ValueType 
            //    || this.Mode == DCILTypeMode.NotSpecify)
            //{
            //    writer.Write(this.Name);
            //}
            //else if (this.Mode == DCILTypeMode.Native)
            //{
            //    writer.Write("native " + this.Name);
            //}
            //else if (this.Mode == DCILTypeMode.Unsigned)
            //{
            //    writer.Write("unsigned " + this.Name);
            //}
            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
            {
                //Dictionary<string, DCILTypeReference> gps = new Dictionary<string, DCILTypeReference>();
                //if( this.LocalClass != null )
                //{
                //    if( this.LocalClass.GenericParamters != null 
                //        && this.LocalClass.GenericParamters.Count == this.GenericParamters.Count )
                //    {
                //        for( int iCount = 0; iCount < this.GenericParamters.Count; iCount ++ )
                //        {

                //        }
                //    }
                //}
                writer.Write("<");
                for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    this.GenericParamters[iCount].WriteToForSignString(writer, gps);
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
                //if (this.LocalClass == null)
                //{
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
                //}
                //else
                //{
                //    DCILGenericParamter.WriteTo(this.LocalClass.GenericParamters, writer);
                //}
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
                writer.Write(this.LocalClass.NameWithNested);
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
        public static bool EqualsList(List<DCILMethodParamter> ps1, List<DCILMethodParamter> ps2, bool includeName, bool checkDefaultValue)
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
    //internal class DCILGenericParamter
    //{
    //    public string ParamterName = null;
    //    public string TypeName = null;
    //    public DCILTypeReference BaseType = null;
    //    //public static List<DCILGenericParamter > ReadParamters( DCILReader reader , bool includeName )
    //    //{
    //    //    var list = new List<DCILGenericParamter>();
    //    //    while( reader.HasContentLeft())
    //    //    {
    //    //        char endChar = char.MinValue;
    //    //        var words = reader.ReadWordsAfterChars(",>", out endChar);
    //    //        var item = new DCILGenericParamter();
    //    //        int index = words.IndexOf("(");
    //    //        if( index > 0 )
    //    //        {
    //    //            item.BaseTypeName = words[index + 1];
    //    //            item.TypeName = words[index + 3];
    //    //        }
    //    //        list.Add(item);
    //    //        if( endChar == '>')
    //    //        {
    //    //            break;
    //    //        }
    //    //    }
    //    //    return list;
    //    //}
    //}
    //internal class DCILMethodOutside : DCILMethod
    //{
    //    private static readonly Dictionary<Type, string> _PrimitiveTypeName = null;
    //    static DCILMethodOutside()
    //    {
    //        _PrimitiveTypeName = new Dictionary<Type, string>();
    //        _PrimitiveTypeName[typeof(void)] = "void";
    //        _PrimitiveTypeName[typeof(byte)] = "uint8";
    //        _PrimitiveTypeName[typeof(sbyte)] = "int8";
    //        _PrimitiveTypeName[typeof(char)] = "char";
    //        _PrimitiveTypeName[typeof(bool)] = "bool";
    //        _PrimitiveTypeName[typeof(short)] = "int16";
    //        _PrimitiveTypeName[typeof(ushort)] = "uint16";
    //        _PrimitiveTypeName[typeof(int)] = "int32";
    //        _PrimitiveTypeName[typeof(uint)] = "uint32";
    //        _PrimitiveTypeName[typeof(long)] = "int64";
    //        _PrimitiveTypeName[typeof(ulong)] = "uint64";
    //        _PrimitiveTypeName[typeof(float)] = "float32";
    //        _PrimitiveTypeName[typeof(double)] = "float64";
    //        _PrimitiveTypeName[typeof(object)] = "objecct";
    //        _PrimitiveTypeName[typeof(string)] = "string";
    //    }
    //    public DCILMethodOutside( System.Reflection.MethodInfo method )
    //    {

    //    }

    //    private readonly string _SignString = null;
    //    public override string GetSignString()
    //    {
    //        return this._SignString;
    //    }
    //    private readonly string _SignStringWithoutName = null;
    //    public override string GetSignStringWithoutName()
    //    {
    //        return this._SignStringWithoutName;
    //    }
    //}

    internal class DCILMethod : DCILMemberInfo
    {
        public static readonly IComparer<DCILMethod> ComparerByName = new NameCompaer();

        private class NameCompaer : IComparer<DCILMethod>
        {
            public int Compare(DCILMethod x, DCILMethod y)
            {
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

        //private static readonly Dictionary<System.Reflection.MethodInfo, DCILMethod> _NativeMethods = new Dictionary<MethodInfo, DCILMethod>();
        //public static DCILMethod CreateByNativeMethodInfo( System.Reflection.MethodInfo info , DCILDocument document )
        //{
        //    if( info == null )
        //    {
        //        return null;
        //    }
        //    DCILMethod result = null;
        //    if( _NativeMethods.TryGetValue( info , out result ) == false )
        //    {
        //        result = new DCILMethod(info, document);
        //        _NativeMethods[info] = result;
        //    }
        //    return result;
        //}
        public readonly System.Reflection.MethodInfo _NativeMethod = null;
        public string _NativeDecilaryTypeName = null;
        public DCILMethod(System.Reflection.MethodInfo method, DCILDocument document, DCILGenericParamterList typeGps) : base(method)
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

            //if (method.IsPublic)
            //{
            //    this.Styles.Add("public");
            //}
            //if (method.IsStatic == false)
            //{
            //    this.Styles.Add("instance");
            //}
            //if (method.IsVirtual)
            //{
            //    this.Styles.Add("virtual");
            //}
            //if (method.IsSpecialName)
            //{
            //    this.Styles.Add("specialname");
            //}
            //if (method.IsAbstract)
            //{
            //    this.Styles.Add("abstract");
            //}
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
                    //string arrayEndFix = null;
                    //if (p.IsOut)
                    //{
                    //    pt2 = p.ParameterType.GetElementType();
                    //    arrayEndFix = "&";
                    //}
                    dcp.ValueType = CreateFromNative(declaringType, method, pt2, document);
                    if (dcp.ValueType != null && typeGps != null)
                    {
                        dcp.ValueType = dcp.ValueType.Transform(typeGps);
                    }
                    //if( arrayEndFix != null )
                    //{
                    //    dcp.ValueType = dcp.ValueType.ChangeArrayAndPointerSettings( dcp.ValueType.ArrayAndPointerSettings + arrayEndFix);
                    //}
                    //dcp.ValueType.ArrayAndPointerSettings += arrayEndFix;
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

        //{
        //    get
        //    {
        //        return this.Styles != null && this.Styles.Contains("instance") == false;
        //    }
        //}
        public bool IsPublic = false;
        //{
        //    get
        //    {
        //        return this.Styles != null && this.Styles.Contains("public");
        //    }
        //}
        public bool IsVirtual = false;
        //{
        //    get
        //    {
        //        return this.Styles != null && this.Styles.Contains("virtual");
        //    }
        //}
        public bool IsInstance = false;
        //{
        //    get
        //    {
        //        return this.Styles != null && this.Styles.Contains("instance");
        //    }
        //}
        public bool IsSpecialname = false;
        //{
        //    get
        //    {
        //        return this.Styles != null && this.Styles.Contains("specialname");
        //    }
        //}
        public bool IsNewslot = false;
        //{
        //    get
        //    {
        //        return this.Styles != null && this.Styles.Contains("newslot");
        //    }
        //}
        public bool IsFinal = false;

        public bool IsAbstract = false;
        //{
        //    get
        //    {
        //        return this.Styles != null && this.Styles.Contains("abstract");
        //    }
        //}

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
                        || item.ValueType.Mode == DCILTypeMode.GenericTypeInTypeDefine)
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
            this._SignStringWithoutName = null;
        }
        private DCILTypeReference[] _SignValueTypes = null;
        private DCILTypeReference[] SignValueTypes
        {
            get
            {
                if (_SignValueTypes == null)
                {
                    var cls = this.Parent as DCILClass;
                    var list = new List<DCILTypeReference>();
                    list.Add(this.ReturnTypeInfo);
                    if (this.Parameters != null && this.Parameters.Count > 0)
                    {
                        foreach (var p in this.Parameters)
                        {
                            list.Add(p.ValueType);
                        }
                    }
                    var typeGPS = cls.BaseType?.GenericParamters;
                    for (int iCount = 0; iCount < list.Count; iCount++)
                    {
                        var item = list[iCount];
                        if (item.Mode == DCILTypeMode.GenericTypeInMethodDefine
                            || item.Mode == DCILTypeMode.GenericTypeInTypeDefine)
                        {
                            if (typeGPS != null && typeGPS.Count > 0)
                            {
                                foreach (var gp in typeGPS)
                                {
                                    if (item.Name == gp.Name)
                                    {

                                    }
                                }
                            }
                        }
                    }
                    this._SignValueTypes = list.ToArray();
                }
                return this._SignValueTypes;
            }
        }
        //private bool? _HasGenericParamter = null;
        ///// <summary>
        ///// 判断是否包含泛型参数
        ///// </summary>
        //public bool HasGenericParamter
        //{
        //    get
        //    {
        //        if (this._HasGenericParamter.HasValue == false)
        //        {
        //            this._HasGenericParamter = false;
        //            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
        //            {
        //                this._HasGenericParamter = true;
        //            }
        //            else if (this.ReturnTypeInfo != null
        //                && (this.ReturnTypeInfo.Mode == DCILTypeMode.GenericTypeInMethodDefine
        //                || this.ReturnTypeInfo.Mode == DCILTypeMode.GenericTypeInTypeDefine))
        //            {
        //                this._HasGenericParamter = true;
        //            }
        //            else if (this.Parameters != null && this.Parameters.Count > 0)
        //            {
        //                foreach (var item in this.Parameters)
        //                {
        //                    if (item.ValueType.Mode == DCILTypeMode.GenericTypeInMethodDefine
        //                        || item.ValueType.Mode == DCILTypeMode.GenericTypeInTypeDefine)
        //                    {
        //                        this._HasGenericParamter = true;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        return this._HasGenericParamter.Value;
        //    }
        //}

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

        //public bool HasGenericInfoFromNativeMethod = false;

        //public bool HasOverrideFromNative = false;

        public DCILMethod OverrideSource = null;
        public DCILMethod GetOverrideSource()
        {
            if (this.OverrideSource != null)
            {
                return this.OverrideSource;
            }
            return this._Override?.LocalMethod;
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
                    code.Value?.UpdateLocalInfo(document, clses);
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
        //private bool MatchParamterType( DCILMethod method , DCILClass cls , DCILTypeReference thisValueType , DCILTypeReference targetValueType )
        //{
        //    if( thisValueType.EqualsValue( targetValueType ))
        //    {
        //        return true;
        //    }
        //    if( thisValueType.Mode == DCILTypeMode.GenericTypeInTypeDefine)
        //    {
        //        foreach( var item in cls.GenericParamters)
        //        {

        //        }
        //    }
        //}
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
            if (this.StartLineIndex == 104424)
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
                reader.ReadAfterCharExcludeLastChar('(');
            }
            this.Parameters = DCILMethodParamter.ReadParameters(reader);
            this.DeclearEndFix = reader.ReadAfterCharExcludeLastChar('{').Trim();

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

        public void UpdateGenericParamtersReference()
        {
            var cls = (DCILClass)this.Parent;
            DCILGenericParamterList gps = cls.GenericParamters;
            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
            {
                if (gps == null)
                {
                    gps = this.GenericParamters;
                }
                else
                {
                    gps = new DCILGenericParamterList(cls.GenericParamters.Count + this.GenericParamters.Count);
                    gps.AddRange(cls.GenericParamters);
                    gps.AddRange(this.GenericParamters);
                }
            }
            if (gps != null && gps.Count > 0)
            {
                var types = new List<DCILTypeReference>();
                types.Add(this.ReturnTypeInfo);
                if (this.Parameters != null && this.Parameters.Count > 0)
                {
                    foreach (var item in this.Parameters)
                    {
                        types.Add(item.ValueType);
                    }
                }
                //foreach (var item in types)
                //{
                //    item.ReferenceGenericParamter = null;
                //    if (item.Mode == DCILTypeMode.GenericTypeInMethodDefine
                //        || item.Mode == DCILTypeMode.GenericTypeInTypeDefine)
                //    {
                //        foreach (var p in gps)
                //        {
                //            if (p.Name == item.Name)
                //            {
                //                item.ReferenceGenericParamter = p;
                //                break;
                //            }
                //        }
                //    }
                //}
            }
        }

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
                                operInfoList.AddItem(labelID, strOperCode, strOperData);
                            }
                            break;
                    }

                    //if (strOperCode == DCILOperCode_LoadString.CodeName_ldstr)
                    //{
                    //    var code = new DCILOperCode_LoadString(reader);
                    //    code.LabelID = labelID;
                    //    operInfoList.Add(code);
                    //}
                    //else if (operCodeType == ILOperCodeType.Method)
                    //{
                    //    var code = new DCILOperCode_HandleMethod(strOperCode, reader);
                    //    code.LabelID = labelID;
                    //    operInfoList.Add(code);
                    //}
                    //else if (operCodeType == ILOperCodeType.Field)
                    //{
                    //    operInfoList.Add(new DCILOperCode_HandleField(labelID, strOperCode, reader.ReadLineTrim()));
                    //}
                    //else if (operCodeType == ILOperCodeType.Class)
                    //{
                    //    operInfoList.Add(new DCILOperCode_HandleClass(labelID, strOperCode, reader.ReadLineTrim()));
                    //}
                    //else if (strOperCode == "switch")
                    //{
                    //    reader.Position--;
                    //    reader.ReadToChar('(');
                    //    string strOperData = reader.ReadToCharExcludeLastChar(')') + ")";
                    //    operInfoList.AddItem(labelID, strOperCode, strOperData);
                    //}
                    //else
                    //{
                    //    var strOperData = reader.ReadLineTrim();
                    //    operInfoList.AddItem(labelID, strOperCode, strOperData);
                    //}
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
                        tryBlock._Try = new DCILObject();
                        tryBlock._Try._Name = ".try";
                        tryBlock._Try.OperCodes = new DCILOperCodeList();
                        tryBlock._Try.Parent = rootObject;
                        InnerLoadILOperCode(tryBlock._Try, reader);
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
        private string _StdSignString = null;
        public virtual string GetStdSignString(bool useCache)
        {
            if (useCache == false || this._StdSignString == null)
            {
                var str = new StringBuilder();
                var writer = new DCILWriter(str);
                var gps = DCILGenericParamterList.Merge(((DCILClass)this.Parent).GenericParamters, this.GenericParamters);
                this.ReturnTypeInfo.WriteToForSignString(writer, gps);
                writer.Write(" ");
                writer.Write(this.Name);

            }
            return this._StdSignString;
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

        private string _SignStringWithoutName = null;
        public virtual string GetSignStringWithoutName()
        {
            if (this.HasGenericStyle || this._SignStringWithoutName == null)
            {
                this._SignStringWithoutName = InnerGetSignString(this.ReturnTypeInfo, null, this.GenericParamters, ((DCILClass)this.Parent)?.GenericParamters, this.Parameters);
            }
            return this._SignStringWithoutName;
        }

        //private string InnerGetSignString( bool addName , Dictionary<string, DCILTypeReference> classGenericeTypeValues  )
        //{
        //    var str = new StringBuilder();
        //    var writer = new DCILWriter(str);
        //    this.ReturnTypeInfo.WriteToForSignString(writer , classGenericeTypeValues);
        //    writer.Write(" ");
        //    if( addName )
        //    {
        //        writer.Write(this.Name);
        //    }
        //    if( this.GenericParamters != null && this.GenericParamters.Count > 0 )
        //    {
        //        writer.Write("<");
        //        for( int iCount = 0; iCount < this.GenericParamters.Count; iCount ++ )
        //        {
        //            if( iCount > 0 )
        //            {
        //                writer.Write(",");
        //            }
        //            writer.Write(this.GenericParamters[iCount].Name);
        //        }
        //        writer.Write(">");
        //    }
        //    writer.Write("(");
        //    if( this.Parameters != null && this.Parameters.Count > 0 )
        //    {
        //        for( int iCount = 0;iCount < this.Parameters.Count;iCount ++ )
        //        {
        //            if( iCount > 0 )
        //            {
        //                writer.Write(",");
        //            }
        //            this.Parameters[iCount].ValueType.WriteToForSignString(writer , classGenericeTypeValues);
        //        }
        //    }
        //    writer.Write(")");
        //    return str.ToString();
        //}

        //public static string InnerGetSignString(
        //    DCILTypeReference returnType,
        //    string methodName,
        //    List<DCILTypeReference> gps,
        //    List<DCILMethodParamter> ps,
        //    Dictionary<string, DCILTypeReference> classGenericeTypeValues)
        //{
        //    var str = new StringBuilder();
        //    var writer = new DCILWriter(str);
        //    returnType.WriteToForSignString(writer, classGenericeTypeValues);
        //    writer.Write(" ");
        //    if (methodName != null)
        //    {
        //        writer.Write(methodName);
        //    }
        //    if (gps != null && gps.Count > 0)
        //    {
        //        writer.Write("<");
        //        for (int iCount = 0; iCount < gps.Count; iCount++)
        //        {
        //            if (iCount > 0)
        //            {
        //                writer.Write(",");
        //            }
        //            gps[iCount].WriteToForSignString(writer, classGenericeTypeValues);
        //        }
        //        writer.Write(">");
        //    }
        //    writer.Write("(");
        //    if (ps != null && ps.Count > 0)
        //    {
        //        for (int iCount = 0; iCount < ps.Count; iCount++)
        //        {
        //            if (iCount > 0)
        //            {
        //                writer.Write(",");
        //            }
        //            ps[iCount].ValueType.WriteToForSignString(writer, classGenericeTypeValues);
        //        }
        //    }
        //    writer.Write(")");
        //    return str.ToString();
        //}
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
            returnType.WriteToForSignString(writer, allGps);
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
                writer.WriteLine(".maxstack " + this.Maxstack);
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
                    writer.Write(" " + item.Name);
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
        public List<DCILMethodParamter> Parameters = null;
        public string DeclearEndFix = null;

        public int ILCodeStartLineIndex = 0;
        public bool ILCodesModified = false;

        public JieJieSwitchs ProtectedOptions = null;

        public string ReturnType = null;
        internal override void SetHeader(string strHeader)
        {
            this.Header = strHeader;
            var items = DCILDocument.SplitByWhitespace(strHeader);
            for (int itemCount = 0; itemCount < items.Count; itemCount++)
            {
                var item = items[itemCount];
                int index10 = item.IndexOf('(');
                if (index10 > 0)
                {
                    this._Name = item.Substring(0, index10);
                    this.ReturnType = items[itemCount - 1];
                    if (this._Name == "GetFontDataUseWin32API")
                    {

                    }
                    //currentMethodName = group.Name;
                    break;
                }
            }
        }
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
    }
    internal class DCILMResource : DCILObject
    {
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
        public string FileName
        {
            get
            {
                if (this.OwnerDocument == null)
                {
                    return this._Name;
                }
                else
                {
                    return System.IO.Path.Combine(this.OwnerDocument.RootPath, this._Name + DCILDocument.EXT_resources);
                }
            }
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
        internal override void SetHeader(string strHeader)
        {
            this.Header = strHeader;

            var words = DCILDocument.SplitByWhitespace(DCILDocument.RemoveChars(strHeader, "()"));
            this._Name = words[words.Count - 1];
            this.ValueTypeName = words[words.Count - 2];
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

    internal class HexCharBuffer
    {
        private char[] _HexBuffer = new char[1024];
        private int _HexCharNum = 0;
        public bool AddHexChar(char c)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'))
            {
                if (_HexCharNum >= _HexBuffer.Length)
                {
                    var temp = new char[(int)(_HexCharNum * 1.5)];
                    Array.Copy(_HexBuffer, temp, _HexBuffer.Length);
                    _HexBuffer = temp;
                }
                _HexBuffer[_HexCharNum++] = c;
                return true;
            }
            return false;
        }
        public int Length
        {
            get
            {
                return this._HexCharNum;
            }
        }
        public string GetHexString()
        {
            if (_HexCharNum > 0)
            {
                var result = new string(_HexBuffer, 0, _HexCharNum);
                _HexCharNum = 0;
                return result;
            }
            return null;
        }
        public byte[] ToByteArray()
        {
            return DCILDocument.HexToBinary(GetHexString());
        }
    }
    public interface IEqualsValue<T>
    {
        bool EqualsValue(T otherValue);
    }

    internal static class DCUtils
    {
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
        public static int ExpandResourcesToPath(System.Reflection.Assembly asm, string resBaseName, string rootPath, bool overWrite)
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

        public static string GetWhitespaceString(int len)
        {
            return _WhitespaceString[len];
        }

        private static readonly string _hexs = "0123456789ABCDEF";

        public static void WriteBytes(System.IO.TextWriter writer, byte[] bs)
        {
            for (int iCount = 0; iCount < bs.Length; iCount++)
            {
                if ((iCount % 32) == 0)
                {
                    writer.WriteLine();
                    writer.Write("      ");
                }
                var bv = bs[iCount];
                writer.Write(_hexs[bv >> 4]);
                writer.Write(_hexs[bv & 0xf]);

                //writer.Write(bs[iCount].ToString("X2"));
                writer.Write(' ');
            }
            writer.WriteLine(")");
        }

        public static bool StringStartWithChar(string text, char c)
        {
            int len = text.Length;
            for (int iCount = 0; iCount < len; iCount++)
            {
                var c2 = text[iCount];
                if (c2 != ' ' && c2 != '\t')
                {
                    if (c2 == c)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return false;
        }
        public static bool StringEndWithChar(string text, char c)
        {
            for (int iCount = text.Length - 1; iCount >= 0; iCount--)
            {
                var c2 = text[iCount];
                if (c2 != ' ' && c2 != '\t')
                {
                    if (c2 == c)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return false;
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
        public static int CleanDirectory(string rootDir)
        {
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

        internal static bool CheckFileWriteable(string fileName, bool throwException = true)
        {
            if (fileName == null || fileName.Length == 0)
            {
                throw new ArgumentNullException("fileName");
            }
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException(fileName);
            }
            if (throwException)
            {
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    var bs = new byte[1024];
                    int len = stream.Read(bs, 0, bs.Length);
                    if (len > 0)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.Write(bs, 0, len);
                    }
                }
                return true;
            }
            else
            {
                try
                {
                    using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        var bs = new byte[1024];
                        int len = stream.Read(bs, 0, bs.Length);
                        if (len > 0)
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.Write(bs, 0, len);
                        }
                    }
                    return true;
                }
                catch (System.Exception ext)
                {
                    System.Diagnostics.Debug.WriteLine(fileName + ":" + ext.Message);
                }
            }
            return false;
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

        private static readonly Dictionary<string, string> _AllocID_Values = new Dictionary<string, string>();
        private static readonly Random _Random = new Random();
        private static readonly string _IDChars = "0123456789abcdefghijklmnopqrstuvwxyz";
        private static readonly string _IDFirstChars = "abcdefghijklmnopqrstuvwxyz";
        public static string AllocID(int length = 6)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length=" + length);
            }
            var chrs = new char[length];
            while (true)
            {
                chrs[0] = _IDFirstChars[_Random.Next(0, _IDFirstChars.Length - 1)];
                for (int iCount = 1; iCount < length; iCount++)
                {
                    chrs[iCount] = _IDChars[_Random.Next(0, _IDChars.Length - 1)];
                }
                string id = new string(chrs);
                if (_AllocID_Values.ContainsKey(id) == false)
                {
                    _AllocID_Values[id] = id;
                    return id;
                }
            }
        }
    }

    internal static class ResourceFileHelper
    {

#if DOTNETCORE
        static void Main(string[] args)
        {
            ___Main( args );
        }
#endif

        public static string[] ParseCommandLineArgs(string line)
        {
            if (line == null || line.Length == 0)
            {
                return new string[0];
            }
            var list = new List<string>();
            StringBuilder args = new StringBuilder();
            bool hasStartFlag = false;
            var len = line.Length;
            for (int iCount = 0; iCount < len; iCount++)
            {
                char c = line[iCount];
                bool completeItem = false;
                if (char.IsWhiteSpace(c))
                {
                    if (hasStartFlag)
                    {
                        args.Append(c);
                    }
                    else
                    {
                        completeItem = true;
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        if (args.Length == 0)
                        {
                            hasStartFlag = true;
                        }
                        else if (hasStartFlag)
                        {
                            hasStartFlag = false;
                            completeItem = true;
                        }
                    }
                    args.Append(c);
                    if (completeItem == false && iCount == len - 1)
                    {
                        completeItem = true;
                    }
                }
                if (completeItem)
                {
                    // 结束一个命令
                    if (args != null && args.Length > 0)
                    {
                        string txt = args.ToString();
                        if (txt[0] == '"')
                        {
                            if (txt[txt.Length - 1] == '"')
                            {
                                txt = txt.Substring(1, txt.Length - 2);
                            }
                            else
                            {
                                txt = txt.Substring(1);
                            }
                        }
                        list.Add(txt);
                        args = new StringBuilder();
                    }
                }
            }
            //if( args != null && args.Length > 0 )
            //{
            //    string txt = args.ToString();
            //    if (txt[0] == '"')
            //    {
            //        if (txt[txt.Length - 1] == '"')
            //        {
            //            txt = txt.Substring(1, txt.Length - 2);
            //        }
            //        else
            //        {
            //            txt = txt.Substring(1);
            //        }
            //    }
            //    list.Add(txt);
            //}
            return list.ToArray();
        }

        private static void ___Main(string[] args)
        {
            string rootPath = null;
            string languageName = null;
            string[] containerClassNames = null;
            foreach (var arg in args)
            {
                int index = arg.IndexOf('=');
                if (index > 0)
                {
                    var argName = arg.Substring(0, index).ToLower();
                    var argValue = arg.Substring(index + 1);
                    switch (argName)
                    {
                        case "rootpath": rootPath = argValue; break;
                        case "language": languageName = argValue; break;
                        case "containerclassnames": containerClassNames = argValue.Split(','); break;
                    }
                }
            }
            var result = Execute(rootPath, languageName, containerClassNames);
            if (result != null)
            {
                result.SaveToStdFileName(rootPath);
            }
        }

        public static MyResourceDataFileList ExecuteByExe(string exeFileName, string rootPath, string languageName, System.Collections.IList containerClassNames)
        {
            var args = new StringBuilder();
            args.Append("\"rootpath=" + rootPath + "\"");
            if (languageName != null && languageName.Length > 0)
            {
                args.Append(" language=" + languageName);
            }
            if (containerClassNames != null && containerClassNames.Count > 0)
            {
                args.Append(" \"containerclassnames=");
                for (int iCount = 0; iCount < containerClassNames.Count; iCount++)
                {
                    args.Append(containerClassNames[iCount]);
                    if (iCount != containerClassNames.Count - 1)
                    {
                        args.Append(',');
                    }
                }
                args.Append('"');
            }
            //___Main(ParseCommandLineArgs(args.ToString()));
            RunExe(exeFileName, args.ToString());
            var result = new MyResourceDataFileList();
            if (result.LoadFromStdFileName(rootPath))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static void RunExe(string exeFileName, string argument)
        {
            var pstart = new System.Diagnostics.ProcessStartInfo();
            pstart.FileName = exeFileName;
            pstart.Arguments = argument;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   >>Execute command line:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\"" + pstart.FileName + "\" " + pstart.Arguments);
            System.Diagnostics.Debug.WriteLine(">>Execute command line: \"" + pstart.FileName + "\" " + pstart.Arguments);
            Console.ResetColor();
            pstart.UseShellExecute = false;
            //pstart.CreateNoWindow = false;
            var p = System.Diagnostics.Process.Start(pstart);
            p.WaitForExit();
        }



        public static bool ExpendResouceToFile(string resName, string fileName, bool overWrite)
        {
            if (overWrite == false && File.Exists(fileName))
            {
                return true;
            }
            var ms = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream(resName);
            if (ms != null)
            {
                using (var stream = new System.IO.FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    var bsTemp = new byte[1024];
                    while (true)
                    {
                        int len = ms.Read(bsTemp, 0, bsTemp.Length);
                        if (len > 0)
                        {
                            stream.Write(bsTemp, 0, len);
                        }
                        else
                        {
                            break;
                        }
                    }//while
                }//using
                ms.Close();
                return true;
            }
            return false;
        }

        public static MyResourceDataFileList Execute(string rootPath, string language, System.Collections.IList containerClassNames)
        {
            if (rootPath == null || rootPath.Length == 0 || Directory.Exists(rootPath) == false)
            {
                return null;
            }
            if (language != null && language.Length > 0)
            {
                var fns = Directory.GetFiles(rootPath, "*.resources");
                foreach (var fn in fns)
                {
                    Console.WriteLine("Merge resource file:" + fn);
                    CombineResourceFile(fn, language, fn);
                }
            }
            if (containerClassNames != null && containerClassNames.Count > 0)
            {
                var values = new MyResourceDataFileList();
                foreach (var name in containerClassNames)
                {
                    var fn = Path.Combine(rootPath, (string)name);
                    if (fn.EndsWith(DCILDocument.EXT_resources, StringComparison.Ordinal) == false)
                    {
                        fn = fn + DCILDocument.EXT_resources;
                    }
                    if (File.Exists(fn))
                    {
                        values.Add(new MyResourceDataFile(fn));
                    }
                }
                //values.SaveToStdFileName(rootPath);
                return values;
            }
            return null;
        }

        public static readonly string EXT_resources = ".resources";




        public static bool CombineResourceFile(string fileName, string language, string outputFileName)
        {
            if (language == null || language.Length == 0)
            {
                return false;
            }
            if (fileName == null || fileName.Length == 0)
            {
                throw new ArgumentNullException("fileName");
            }
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException(fileName);
            }
            var values = new Dictionary<string, System.Tuple<string, byte[]>>();
            FillValues2(fileName, values);
            var rootPath = Path.GetDirectoryName(fileName);
            var fn2 = Path.Combine(
                Path.Combine(rootPath, language),
                Path.GetFileNameWithoutExtension(fileName) + "." + language + EXT_resources);
            if (File.Exists(fn2))
            {
                if (FillValues2(fn2, values) > 0)
                {
                    if (outputFileName == null || outputFileName.Length == 0)
                    {
                        outputFileName = fileName;
                    }

                    using (var writer = new System.Resources.ResourceWriter(outputFileName))
                    {
                        var names = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        foreach (var item in values)
                        {
                            if (names.ContainsKey(item.Key) == false)
                            {
                                writer.AddResourceData(item.Key, item.Value.Item1, item.Value.Item2);
                                names[item.Key] = null;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine("Same name resource item:" + fileName + " # " + item.Key);
                                Console.ResetColor();
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
#if DOTNETCORE
        public static int FillValues2(string fileName, Dictionary<string, System.Tuple<string, byte[]>> values)
        {
            int result = 0;
            using (var reader = new System.Resources.Extensions.DeserializingResourceReader(fileName))
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
                    if (resType != null && resType.Length > 0 && itemData != null && itemData.Length > 0)
                    {
                        values[name] = new Tuple<string, byte[]>(resType, itemData);
                        result++;
                    }
                }
            }
            return result;
        }
        private static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
#else
        public static int FillValues2(string fileName, Dictionary<string, System.Tuple<string, byte[]>> values)
        {
            int result = 0;
            using (var reader = new System.Resources.ResourceReader(fileName))
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
                        values[name] = new Tuple<string, byte[]>(resType, itemData);
                        result++;
                    }
                }
            }
            return result;
        }
#endif
    }

    internal class MyResourceDataFileList : List<MyResourceDataFile>
    {
        public static readonly string StdFileName = "dcsoft20210306.resouces.dat";
        public MyResourceDataFileList()
        {

        }
        public MyResourceDataFileList(List<string> fileNames)
        {
            foreach (var fn in fileNames)
            {
                this.Add(new MyResourceDataFile(fn));
            }
        }

        public Dictionary<string, MyResourceDataFile> GetFileTable()
        {
            var result = new Dictionary<string, MyResourceDataFile>();
            foreach (var item in this)
            {
                result[item.Name] = item;
            }
            return result;
        }
        public bool LoadFromStdFileName(string rootPath)
        {
            var fn = Path.Combine(rootPath, StdFileName);
            if (File.Exists(fn))
            {
                Load(fn);
                return true;
            }
            return false;
        }
        public void SaveToStdFileName(string rootPath)
        {
            var fn = Path.Combine(rootPath, StdFileName);
            Save(fn);
        }
        public void Load(string fileName)
        {
            using (var reader = new System.IO.BinaryReader(
                new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read)))
            {
                int fileCount = reader.ReadInt32();
                for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
                {
                    var dataFile = new MyResourceDataFile();
                    dataFile.FileName = reader.ReadString();
                    dataFile.Name = reader.ReadString();
                    var bsLength = reader.ReadInt32();
                    if (bsLength > 0)
                    {
                        dataFile.Datas = reader.ReadBytes(bsLength);
                    }
                    var itemCount = reader.ReadInt32();
                    for (int iCount = 0; iCount < itemCount; iCount++)
                    {
                        var newItem = new MyResourceDataFile.MyResourceDataItem();
                        newItem.Name = reader.ReadString();
                        newItem.StartIndex = reader.ReadInt32();
                        newItem.BsLength = reader.ReadInt32();
                        newItem.Key = reader.ReadInt32();
                        newItem.IsBmp = reader.ReadBoolean();
                        dataFile.Items.Add(newItem);
                    }
                    this.Add(dataFile);
                }
            }
        }
        public void Save(string fileName)
        {
            using (var writer = new System.IO.BinaryWriter(
                new System.IO.FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)))
            {
                writer.Write(this.Count);
                foreach (var dataFile in this)
                {
                    writer.Write(dataFile.FileName);
                    writer.Write(dataFile.Name);
                    if (dataFile.Datas == null || dataFile.Datas.Length == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(dataFile.Datas.Length);
                        writer.Write(dataFile.Datas, 0, dataFile.Datas.Length);
                    }
                    if (dataFile.Items == null || dataFile.Items.Count == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(dataFile.Items.Count);
                        foreach (var item in dataFile.Items)
                        {
                            writer.Write(item.Name);
                            writer.Write(item.StartIndex);
                            writer.Write(item.BsLength);
                            writer.Write(item.Key);
                            writer.Write(item.IsBmp);
                        }
                    }
                }
            }
        }
    }

    internal class MyResourceDataFile
    {
        internal class MyResourceDataItem
        {
            public string Name = null;
            public int StartIndex = 0;
            public int BsLength = 0;
            public int Key = 0;
            public bool IsBmp = false;
            public object Value = null;
            public override string ToString()
            {
                return this.Name + ":" + this.Value;
            }
        }
        public override string ToString()
        {
            return this.Name;
        }
        public string Name = null;
        public string FileName = null;
        public List<MyResourceDataItem> Items = new List<MyResourceDataItem>();
        public byte[] Datas = null;
        public bool HasBmp
        {
            get
            {
                foreach (var item in this.Items)
                {
                    if (item.IsBmp)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public MyResourceDataFile()
        {

        }
        private static readonly Random _Random = new Random();
        public MyResourceDataFile(string fileName)
        {
#if DOTNETCORE
            var reader = new System.Resources.Extensions.DeserializingResourceReader(fileName);
#else
            var reader = new System.Resources.ResourceReader(fileName);
#endif

            this.FileName = fileName;
            this.Name = Path.GetFileNameWithoutExtension(fileName);
            var objValues = new Dictionary<string, object>();
            var strValues = new Dictionary<string, string>();
            var resouceSet = new System.Resources.ResourceSet(reader);
            var enumer = resouceSet.GetEnumerator();
            var lstData = new List<byte>();
            bool hasBmpValue = false;
            while (enumer.MoveNext())
            {
                string itemName = (string)enumer.Key;
                var item = new MyResourceDataItem();
                item.Name = itemName;
                item.StartIndex = lstData.Count;
                item.Key = _Random.Next(1000, int.MaxValue - 1000);
                item.Value = enumer.Value;
                if (enumer.Value is string)
                {
                    item.IsBmp = false;
                    string str = (string)item.Value;
                    int key = item.Key;
                    for (int iCount = 0; iCount < str.Length; iCount++, key++)
                    {
                        var v = str[iCount] ^ key;
                        lstData.Add((byte)(v >> 8));
                        lstData.Add((byte)(v & 0xff));
                    }
                }
                else if (enumer.Value is System.Drawing.Bitmap)
                {
                    item.IsBmp = true;
                    hasBmpValue = true;
                    var ms = new System.IO.MemoryStream();
                    ((System.Drawing.Bitmap)item.Value).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    var bs2 = ms.ToArray();
                    byte key = (byte)item.Key;
                    for (int iCount = 0; iCount < bs2.Length; iCount++, key++)
                    {
                        bs2[iCount] = (byte)(bs2[iCount] ^ key);
                    }
                    lstData.AddRange(bs2);
                }
                else
                {
                    throw new NotSupportedException(item.Value.GetType().FullName);
                }
                item.BsLength = lstData.Count - item.StartIndex;
                this.Items.Add(item);
            }
            resouceSet.Close();
            this.Datas = lstData.ToArray();
        }
    }

    [System.Runtime.InteropServices.ComVisible(false)]
    internal static class InnerAssemblyHelper20210315
    {
        private static volatile System.Threading.Thread _CloneStringCrossThead_Thread = null;
        private static readonly System.Threading.AutoResetEvent _CloneStringCrossThead_Event = new System.Threading.AutoResetEvent(false);
        private static readonly System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner = new System.Threading.AutoResetEvent(false);
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

        //private static readonly Random _Random = new Random();
        //public static byte[] GetBytes(byte[] bs, ref byte key, ref bool gzip)
        //{
        //    if (bs == null || bs.Length < 1000)
        //    {
        //        return bs;
        //    }
        //    var ms = new System.IO.MemoryStream();
        //    var stream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress);
        //    stream.Write(bs, 0, bs.Length);
        //    stream.Close();
        //    var bsResult = ms.ToArray();
        //    var rate = (double)bs.Length / (double)bsResult.Length;
        //    if (bs.Length - bsResult.Length > 512 && rate > 2)
        //    {
        //        // 压缩造成的效益足够大
        //        gzip = true;
        //    }
        //    else
        //    {
        //        // 压缩造成的效益不够大
        //        bsResult = bs;
        //        gzip = false;
        //    }
        //    key = (byte)(_Random.Next(100, 234));
        //    byte key2 = key;
        //    for (int iCount = 0; iCount < bs.Length; iCount++, key2++)
        //    {
        //        bs[iCount] = (byte)(bs[iCount] ^ key2);
        //    }
        //    return bsResult;
        //}
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

}