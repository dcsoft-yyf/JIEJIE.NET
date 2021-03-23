/*
   DC.NET Protector
  
   An open source tool to encrypt .NET assembly file, help people protect theirs copyright.

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

[assembly: AssemblyTitle("DC.NET Protector console application")]
[assembly: AssemblyDescription("Protect your .NET software copyright powerfull.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DCSoft")]
[assembly: AssemblyProduct("DC.NET Protector")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.1.0.0")]

namespace DCNETProtector
{
#if !DCSoftInner
    static class ConsoleProgram
    {
        static void Main(string[] args)
        {
            //args = new string[] {
            //    @"input=E:\Source\DCSoft\08代码\DCSoft\DCWriter专用版\DCSoft.Writer.ForASPNETCore_All\bin_netcore\debug\netcoreapp3.0\DCSoft.Writer.ForASPNETCore.dll",
            //    @"output=E:\Source\DCSoft\08代码\DCSoft\DCWriter专用版\DCSoft.Writer.ForASPNETCore_All\bin_netcore\debug\netcoreapp3.0\DCSoft.Writer.ForASPNETCore_2.dll",
            //    @"snk=E:\Source\DCSoft\08代码\DCSoft\DCWriter专用版\DCSoft.Writer.ForASPNETCore_All\yyf.snk",
            //    "pause"
            //};
            string inputAssmblyFileName = null;
            string outputAssemblyFileName = null;
            string snkFileName = null;
            bool pause = false;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
*******************************************************************************
  _____   _____   _   _ ______ _______   _____           _            _             
 |  __ \ / ____| | \ | |  ____|__   __| |  __ \         | |          | |            
 | |  | | |      |  \| | |__     | |    | |__) | __ ___ | |_ ___  ___| |_ ___  _ __ 
 | |  | | |      | . ` |  __|    | |    |  ___/ '__/ _ \| __/ _ \/ __| __/ _ \| '__|
 | |__| | |____ _| |\  | |____   | |    | |   | | | (_) | ||  __/ (__| || (_) | |   
 |_____/ \_____(_)_| \_|______|  |_|    |_|   |_|  \___/ \__\___|\___|\__\___/|_|   
                                                                                    

     DC.NET Protector v1.1 ,encrypt .NET assembly file, help you protect copyright.
     Author:yuan yong fu from CHINA . mail: 28348092@qq.com
     Site :https://github.com/dcsoft-yyf/DCNETProtector
     Last update 2021-3-22
     You can use this software unlimited,but CAN NOT modify source code anytime.
     If you have good idears you can write to 28348092@qq.com.
     Support command line argument :
        input =[required,Full path of input .NET assembly file , can be .exe or .dll, currenttly only support .NET framework 2.0 or later]
        output=[optional,Full path of output .NET assmebly file , if it is empty , then use input argument value]
        snk   =[optional,Full path of snk file. It use to add strong name to output assembly file.]
        pause =[optional,pause the console after finish process.]

     Example 1, protect d:\a.dll ,this will modify dll file.
        >DCNETProtector.exe input=d:\a.dll  
     Exmaple 2, anlyse d:\a.dll , and write result to another dll file with strong name.
        >DCNETProtector.exe input=d:\a.dll output=d:\publish\a.dll snk=d:\source\company.snk
　　　　　　　　　　　　　　　　　　　　
*******************************************************************************");
            Console.ResetColor();
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
                                snkFileName = argValue;
                                break;
                            case "pause":
                                pause = true;
                                break;
                        }
                    }
                    else if (string.Compare(arg, "pause", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        pause = true;
                    }
                }
            }
            try
            {
                if (snkFileName != null
                   && snkFileName.Length > 0
                   && File.Exists(snkFileName) == false)
                {
                    ConsoleWriteError("Can not find file : " + snkFileName);
                    return;
                }
                if (inputAssmblyFileName != null && inputAssmblyFileName.Length > 0)
                {
                    if (File.Exists(inputAssmblyFileName))
                    {
                        Console.Title = "DC.NET Protector - " + inputAssmblyFileName;
                        DCProtectEngine.ExecuteAssemblyFile(inputAssmblyFileName, snkFileName, outputAssemblyFileName);
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
                    Console.WriteLine("Press any key to continue...");
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

#endif

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
                var args = "\"" + ilFileName + "\" \"/output=" + outputFileName + "\" ";
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
            _Document = new DCILDocument(inputILFileName, txtEncoding);
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
            var strValues = new Dictionary<string, List<DCStringValueDefine>>();
            foreach (var item in _Document.StringDefines)
            {
                List<DCStringValueDefine> list = null;
                if (strValues.TryGetValue(item.FinalValue, out list) == false)
                {
                    list = new List<DCStringValueDefine>();
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
                    for (int iCount9 = method.EndLineIndex; iCount9 >= method.StartLineIndex; iCount9--)
                    {
                        var line9 = _Document.SourceLines[iCount9];
                        if (line9.Contains("ret"))
                        {
                            string labelID = null;
                            string operData = null;
                            var operCode = DCILDocument.GetILCode(line9, ref labelID, ref operData);
                            if (operCode == "ret")
                            {
                                line9 = "IL_zzzzz: call string DCSoft.Common.InnerAssemblyHelper20210315::CloneStringCrossThead(string)\r\n" + line9;
                                _Document.SourceLines[iCount9] = line9;
                                _ModifiedCount++;
                                break;
                            }
                        }
                    }
                }
            }
            if (_Document.ResouceFiles.Count > 0)
            {
                ApplyResouceContainerClass();
                ApplyComponentResourceManagers();

            }
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
                    totalStrLength += item.Value.Length;
                    byteTotalLength += utf8.GetByteCount(item.Value);
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
			IL_0098: leave.s IL_00a5
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
			IL_0062: leave.s IL_0085
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
                    var line = _Document.SourceLines[item.LineIndex];
                    string labelID = null;
                    string operData = null;
                    string operCode = DCILDocument.GetILCode(line, ref labelID, ref operData);
                    line = labelID + " : newobj     instance void " + clsName + "::.ctor()";
                    _Document.SourceLines[item.LineIndex - 2] = string.Empty;
                    _Document.SourceLines[item.LineIndex - 1] = string.Empty;
                    _Document.SourceLines[item.LineIndex] = line;
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
                if (ExpandResourcesToPath(
                    typeof(DCProtectEngine).Assembly,
                    "DCSoft.AssemblyPublish.DCSoft.ResourceFileHelper.NetCore.",
                    tempPath,
                    true) == 0)
                {
                    ExpandResourcesToPath(
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
                }
            }//foreach
        }

        private static int ExpandResourcesToPath(System.Reflection.Assembly asm, string resBaseName, string rootPath, bool overWrite)
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
            for (int iCount = 0; iCount < _Datas.Count; iCount++)
            {
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
        public int LineIndex = 0;
        public DCILMethod Method = null;
        public DCILClass MyClass = null;
        public string ClassName = null;
    }

    internal class DCILDocument : DCILGroup, IDisposable
    {
        public static void Test()
        {
            var doc = new DCILDocument(@"D:\temp2\ilcompress\DCILTemp\DCSoft.Writer.ForWinForm.dll.il", Encoding.UTF8);

        }
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

        public DCILDocument(string ilFileName, Encoding encod)
        {
            if (ilFileName == null)
            {
                throw new ArgumentNullException("ilFileName");
            }
            if (File.Exists(ilFileName) == false)
            {
                throw new FileNotFoundException(ilFileName);
            }
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
            this.Name = Path.GetFileName(ilFileName);
            this.RootPath = Path.GetDirectoryName(ilFileName);
            this.ChildNodes = new List<DCILGroup>();
            this.Parse();
            this.OwnerDocument = this;
        }
        public DCILDocument(string rootPath, string[] srcLines)
        {
            this.SourceLines = srcLines;
            this.RootPath = rootPath;
            this.ChildNodes = new List<DCILGroup>();
            this.Parse();
            this.OwnerDocument = this;
        }

        /// <summary>
        /// 获得所有支持的语言信息对象
        /// </summary>
        /// <param name="rootDir">根目录</param>
        /// <returns>语言信息对象</returns>
        public System.Globalization.CultureInfo[] GetSupportCultures()
        {
            var list = new List<System.Globalization.CultureInfo>();
            foreach (var dir in Directory.GetDirectories(this.RootPath))
            {
                var fns = Directory.GetFiles(dir, "*" + EXT_resources);
                if (fns != null && fns.Length > 0)
                {
                    var name = Path.GetFileName(dir);
                    var cul = System.Globalization.CultureInfo.GetCultureInfo(name);
                    if (cul != null)
                    {
                        list.Add(cul);
                    }
                }
            }
            return list.ToArray();
        }

        public int ClearContent(DCILGroup group)
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
        public string[] SourceLines = null;
        public readonly string RootPath = null;
        public string LibName_mscorlib = "mscorlib";
        public int ModifiedCount = 0;
        public List<ComponentResourceManagerInfo> ComponentResourceManagers = new List<ComponentResourceManagerInfo>();
        public List<DCStringValueDefine> StringDefines = new List<DCStringValueDefine>();
        public List<DCILGroup> AllGroups = new List<DCILGroup>();
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
            ReadAllDefines(this, 0);
            if (this.LibName_mscorlib == null)
            {
                this.LibName_mscorlib = "mscorlib";
            }
            tick = Math.Abs(Environment.TickCount - tick);
        }

        private void ReadAllDefines(DCILGroup rootGroup, int startLineIndex)
        {
            int lineNum = this.SourceLines.Length;
            //List<StringDefine> strDefines = new List<StringDefine>();
            string currentMethodName = null;
            if (rootGroup.Type == Name_method)
            {
                currentMethodName = rootGroup.Name;
            }
            int currentLevel = 0;
            for (int iCount = startLineIndex; iCount < lineNum; iCount++)
            {
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
                    string strOperData = null;
                    string strLabelID = null;
                    string strILCode = GetILCode(line, ref strLabelID, ref strOperData);
                    if (strOperData == null || strOperData.Length == 0)
                    {
                        continue;
                    }
                    if (strILCode == "call")
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
                    else if (strILCode == "newobj")
                    {
                        if (strOperData.Contains("ComponentResourceManager::.ctor")
                            && rootGroup.Name == "InitializeComponent")
                        {
                            var line2 = this.SourceLines[iCount - 2];

                            string labelID2 = null;
                            string operData2 = null;
                            string operCode2 = GetILCode(line2, ref labelID2, ref operData2);
                            if (operCode2 == "ldtoken")
                            {
                                ComponentResourceManagerInfo info = new ComponentResourceManagerInfo();
                                info.LineIndex = iCount;
                                info.Method = (DCILMethod)rootGroup;
                                info.ClassName = operData2;
                                this.ComponentResourceManagers.Add(info);
                            }
                        }
                    }
                    else if (strILCode == "ldstr")
                    {
                        var strDif = new DCStringValueDefine();
                        strDif.NativeSourcde = this.SourceLines[iCount];
                        strDif.LabelID = strLabelID;
                        strDif.MethodName = currentMethodName;
                        strDif.LineIndex = iCount;
                        strDif.EndLineIndex = iCount;
                        if (strOperData[0] == '"')
                        {
                            strDif.IsBinary = false;
                            if (strOperData.Length == 2 && strOperData[1] == '"')
                            {
                                // use string.Empty
                                this.SourceLines[iCount] = strLabelID + ":ldsfld     string [" + this.LibName_mscorlib + "]System.String::Empty";
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
                                    strDif.EndLineIndex = iCount2;
                                    iCount++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            strDif.Value = strOperData;
                            strDif.FinalValue = strFinalValue.ToString();
                        }
                        else if (strOperData.StartsWith("bytearray", StringComparison.Ordinal))
                        {
                            _HexCharNum = 0;
                            strDif.IsBinary = true;
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
                            if (hasFinish)
                            {
                                // 光靠一行就结束了
                                strDif.Value = GetHexString();
                            }
                            else
                            {
                                for (iCount++; iCount < lineNum; iCount++)
                                {
                                    string line2 = this.SourceLines[iCount];
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
                                                strDif.EndLineIndex = iCount;
                                                goto EndReadStringDefine;
                                            }
                                        }
                                    }
                                }
                            EndReadStringDefine:;
                                strDif.Value = GetHexString();
                            }
                            var bsText = HexToBinary(strDif.Value);
                            if (bsText != null && bsText.Length > 0)
                            {
                                strDif.FinalValue = Encoding.Unicode.GetString(bsText);
                            }
                            else
                            {
                                strDif.FinalValue = string.Empty;
                            }
                        }

                        if (strDif.FinalValue == null)
                        {
                        }
                        if (strDif.FinalValue == SecurityMethodFlags && rootGroup is DCILMethod)
                        {
                            // is Securty method
                            var method9 = (DCILMethod)rootGroup;
                            if (method9.ReturnType == "string")
                            {
                                this.SecurityMethods.Add(new Tuple<DCILMethod, int>((DCILMethod)rootGroup, iCount));
                                strDif.FinalValue = DateTime.Now.Second.ToString();
                                strDif.Value = '"' + strDif.FinalValue + '"';
                            }
                        }
                        this.StringDefines.Add(strDif);
                        if (iCount < lineNum && currentMethodName == ".cctor")
                        {
                            string nextLine = this.SourceLines[iCount + 1];
                            strILCode = GetILCode(nextLine, ref strLabelID, ref strOperData);
                            if (strILCode == "stsfld")
                            {
                                strDif.IsSetStaticField = true;
                            }
                        }
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
                    DCILGroup group = null;
                    if (firstWord == Name_method)
                    {
                        group = new DCILMethod();
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
                    ReadAllDefines(group, group.BodyLineIndex + 1);
                    iCount = group.EndLineIndex;
                }
                else if (firstWord == Name_get)
                {
                    if (rootGroup is DCILProperty)
                    {
                        ((DCILProperty)rootGroup).HasGetMethod = true;
                    }
                }
                else if (firstWord == Name_set)
                {
                    if (rootGroup is DCILProperty)
                    {
                        ((DCILProperty)rootGroup).HasSetMethod = true;
                    }
                }
                else if (firstWord == Name_mresource)
                {
                    var items = SplitByWhitespace(line);
                    string name = items[items.Count - 1];
                    if (name.EndsWith(EXT_resources))
                    {
                        name = name.Substring(0, name.Length - EXT_resources.Length);
                        var file = new DCILMResource();
                        file.Name = name;
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
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }
            else if (c >= 'a' && c <= 'f')
            {
                return c - 'a' + 10;
            }
            else if (c >= 'A' && c <= 'F')
            {
                return c - 'A' + 10;
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
            for (int iCount = 0; iCount < len; iCount++)
            {
                if (IsWhitespace(text[iCount]))
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

    internal class DCILClass : DCILGroup
    {
        public DCILClass()
        {
            base.ChildNodes = new List<DCILGroup>();
        }
        public List<string> ImplementsInterfaces = null;
        public List<int> FieldLineIndexs = new List<int>();
        public bool Modified = false;
        public string BaseTypeName = null;
        internal override void SetHeader(string strHeader)
        {
            this.Header = strHeader;
            var items = DCILDocument.SplitByWhitespace(strHeader);
            for (int itemCount = 0; itemCount < items.Count; itemCount++)
            {
                if (items[itemCount] == "extends" && itemCount > 0)
                {
                    this.Name = items[itemCount - 1];
                    this.BaseTypeName = items[itemCount + 1];
                    return;
                }
            }
            this.Name = items[items.Count - 1];

            this.IsInterface = items.Contains("interface");
            this.IsPublic = items.Contains("public");
            int index2 = items.IndexOf("implements");
            if (index2 > 0)
            {
                this.ImplementsInterfaces = new List<string>();
                for (int iCount = index2; iCount < items.Count; iCount++)
                {
                    this.ImplementsInterfaces.Add(items[iCount]);
                }
            }
            //if (strHeader.Contains("WriterControl") && this.IsPublic == false )
            //{

            //}
        }

        public bool IsInterface = false;
        public bool IsPublic = false;
        /// <summary>
        /// 是否为访问程序集资源的包装类型
        /// </summary>
        /// <returns></returns>
        public bool IsResoucePackage()
        {
            if (this.CustomAttributes != null
                && this.BaseTypeName != null
                && this.BaseTypeName.Contains("System.Object"))
            {
                int flagCount = 0;
                foreach (var item in this.CustomAttributes)
                {
                    if (item.Name.Contains("GeneratedCodeAttribute")
                        || item.Name.Contains("DebuggerNonUserCodeAttribute")
                        || item.Name.Contains("CompilerGeneratedAttribute"))
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
                return "Interface " + this.Name;
            }
            else if (this.BaseTypeName == null || this.BaseTypeName.Contains("System.Object"))
            {
                return "Class " + this.Name;
            }
            else
            {
                return "Class " + this.Name + " : " + this.BaseTypeName;
            }
        }
    }
    internal class DCILCustomAttribute : DCILGroup
    {
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
                    this.Name = text.Substring(iCount, index - iCount);
                    break;
                }
            }
        }

        public string HexValue = null;
        public override string ToString()
        {
            return "Attribute " + this.Name;
        }
    }

    internal class DCILGroup
    {
        public DCILDocument OwnerDocument = null;
        public string Name = null;
        public string Type = null;
        public string Header = null;
        internal virtual void SetHeader(string strHeader)
        {
            this.Header = strHeader;
        }
        public int StartLineIndex = 0;
        public int BodyLineIndex = 0;
        public int EndLineIndex = 0;
        public List<DCILGroup> ChildNodes = null;
        public List<DCILCustomAttribute> CustomAttributes = null;
        public int Level = 0;
        public override string ToString()
        {
            if (this.ChildNodes != null && this.ChildNodes.Count > 0)
            {
                return this.Type + "#" + this.Name + " " + this.ChildNodes.Count + "个子节点";
            }
            else
            {
                return this.Type + "#" + this.Name;
            }
        }

    }

    internal class DCILMethod : DCILGroup
    {
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
                    this.Name = item.Substring(0, index10);
                    this.ReturnType = items[itemCount - 1];
                    //currentMethodName = group.Name;
                    break;
                }
            }
        }
        //public int ComponentResourceManagerLineIndex = -1;
        public override string ToString()
        {
            return "Method " + this.ReturnType + " " + this.Name;
        }
    }

    internal class DCILMResource : DCILGroup
    {
        public override string ToString()
        {
            return "Resource " + this.Name;
        }
        public string FileName
        {
            get
            {
                return System.IO.Path.Combine(this.OwnerDocument.RootPath, this.Name + DCILDocument.EXT_resources);
            }
        }
    }

    internal class DCILProperty : DCILGroup
    {
        internal override void SetHeader(string strHeader)
        {
            this.Header = strHeader;

            var words = DCILDocument.SplitByWhitespace(DCILDocument.RemoveChars(strHeader, "()"));
            this.Name = words[words.Count - 1];
            this.ValueTypeName = words[words.Count - 2];
        }
        public string ValueTypeName = null;
        public bool HasGetMethod = false;
        public bool HasSetMethod = false;
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Property " + this.Name);
            if (this.HasGetMethod)
            {
                str.Append(" get; ");
            }
            if (this.HasSetMethod)
            {
                str.Append(" set;");
            }
            return str.ToString();
        }
    }

    internal class DCStringValueDefine
    {
        public string NativeSourcde = null;
        public bool IsSetStaticField = false;
        public string MethodName = null;
        public int LineIndex = -1;
        public int EndLineIndex = -1;
        public string Value = null;
        public string FinalValue = null;
        public bool IsBinary = false;
        public string LabelID = null;
        public override string ToString()
        {
            return this.LineIndex + " : " + this.Value + "  #" + this.NativeSourcde;
        }

    }

    internal static class DCUtils
    {
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
            var rnd = new System.Random();
            var strListCount = list.Count;
            for (int iCount = 0; iCount < strListCount; iCount++)
            {
                int index = rnd.Next(0, strListCount - 1);
                var temp = list[iCount];
                list[iCount] = list[index];
                list[index] = temp;
            }
        }

        private static readonly Dictionary<string, string> _AllocID_Values = new Dictionary<string, string>();
        private static readonly Random _Random = new Random();
        private static readonly string _IDChars = "0123456789abcdefghijklmnopqrstuvwxyz";
        public static string AllocID(int length = 6)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length=" + length);
            }
            var chrs = new char[length];
            while (true)
            {
                for (int iCount = 0; iCount < length; iCount++)
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
            Console.Write(">>Execute command line:");
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
                    var fn = Path.Combine(rootPath, name + EXT_resources);
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
