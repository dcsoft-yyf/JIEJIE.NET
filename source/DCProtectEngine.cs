using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DCNETProtector
{

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

        private static string TransformForDotNetCore( string code )
        {
            if( _Document._IsDotNetCoreAssembly)
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

                if( Execute(ilFileName, ilFileName, Encoding.UTF8)== false )
                {
                    return false;
                }

                DCUtils.CheckFileWriteable(asmFileName);
                if (outputFileName == null || outputFileName.Length == 0)
                {
                    outputFileName = asmFileName;
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
            if(_Document.StringDefines.Count==0)
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
	IL_0001: call uint8[] " + ByteArrayDataContainer.GetMethodName( lstItems.Datas) + @"()
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
                    lableIDCount += 10; strNewClassCode.AppendLine("        IL_" + Convert.ToString(lableIDCount) + ": call string " + clsName + "::GetStringByLong(uint8[], int64)");
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
            if( _Document.SecurityMethods != null && _Document.SecurityMethods.Count > 0 )
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
                //if (resKey.EndsWith( DCILDocument.EXT_resources, StringComparison.Ordinal) == false)
                //{
                //    continue;
                //}
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
                ExpandResourcesToPath(
                    typeof(DCProtectEngine).Assembly,
                    "DCSoft.AssemblyPublish.DCSoft.ResourceFileHelper.NetCore.",
                    tempPath,
                    true);
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

   
}
