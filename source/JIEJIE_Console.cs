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
[assembly: AssemblyTitle("JieJie.NETConsoleApplication")]

#endif

namespace JIEJIE
{
    internal static class JIEJIE_Console
    {
        static string Test()
        {
            try
            {
                var s = new StringBuilder();
                s.Append("aaaaaaaaa");
                s.Append("bbbbb");
                if (s.Length > 1)
                {
                    return s.ToString();
                }
            }
            catch( System.Exception ext )
            {
                return ext.ToString();
            }
            return "bbb";
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Argument command line:" + Environment.CommandLine);
            //var fn = @"E:\Source\DCSoft\08代码\DCSoft\DCSoft.WASM\bin\Debug\net6.0\wwwroot\_framework\DCSoft.Writer.ForWASM.dll";
            //var sh = System.Security.Cryptography.SHA256.Create();
            //var bs2 = sh.ComputeHash(System.IO.File.ReadAllBytes(fn));
            //Console.WriteLine(Convert.ToBase64String(bs2));
            //return;
            //var bs = System.IO.File.ReadAllBytes(fn);
            //var gz = new System.IO.Compression.GZipStream(
            //    new System.IO.FileStream(fn + ".gz", FileMode.Open, FileAccess.Read),
            //    System.IO.Compression.CompressionMode.Decompress);
            //var ms = new System.IO.MemoryStream();
            //var bsTemp = new byte[1024];
            //while( true )
            //{
            //    int len = gz.Read(bsTemp, 0, bsTemp.Length);
            //    if(len > 0 )
            //    {
            //        ms.Write(bsTemp, 0, len);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            //var bs2 = ms.ToArray();
            //if(bs.Length == bs2.Length )
            //{
            //    for(var iCount = 0; iCount <bs.Length;iCount ++ )
            //    {
            //        if( bs[iCount] != bs2[iCount])
            //        {

            //        }
            //    }
            //}
            ///* test data **************************/
            if (args == null || args.Length == 0)
            {
                args = new string[] {
                 //   @"d:\temp3\RFLeaderboards.dll",
                //@"translatestack=E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\DCSoft.DCWriterSimpleDemo\Lib\DCSoft.Writer.ForWinForm.dll.map.xml",
                //@"E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\DCSoft.ASPNETDemo\bin\DCSoft.Writer.ForASPNET.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCWriterForASPNET\bin\debug\DCSoft.Writer.ForASPNET.dll",
                //@"C:\Users\yfyuan\source\repos\WindowsFormsApp13\bin\Debug\WindowsFormsApp13.exe",
                //@"E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\Bin\DCSoft.WinFormDemo.exe",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCSoft.Common\bin\Debug\DCSoft.Common.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCWriterForWinForm\bin\debug\DCSoft.Writer.ForWinForm.dll",
                //@"E:\Source\DCSoft.WASM.Publish\bin\Release\net7.0\DCSoft.WASM.dll",
                @"D:\DCSoft.WASM.Publish\bin\Debug\net7.0\DCSoft.WASM.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCSoft.WASM\bin\Debug\net7.0\DCSoft.WASM.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\bin\Debug\net7.0\DCSoft.Writer.ForWASM.dll",
                //@"C:\Users\yfyuan\source\repos\ClassLibrary3\bin\Debug\net7.0\ClassLibrary3.dll",
                //@"C:\Users\yfyuan\.nuget\packages\dcsoft.writer.forwasm\1.2023.3.10\lib\net7.0\DCSoft.Writer.ForWASM.dll",
                //@"C:\Users\yfyuan\source\repos\ClassLibrary2\bin\Debug\net7.0\ClassLibrary2.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCSoft.WinForms\Source\bin\Debug\DCSoft.WinForms.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCSoft.Data\Source\bin\Debug\DCSoft.Data.dll",
                    //@"D:\temp3\DCSoft.Writer.ForWinForm.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCSoft.Drawing\Source\bin\Debug\DCSoft.Drawing.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCSoft.Writer.Core\bin\debug\DCSoft.Writer.Core.dll",
                //@"E:\Source\DCSoft\08代码\DCSoft\DCWriter专用版\DCSoft.Writer.ForASPNETCore_All\bin_netcore\debug\netcoreapp3.0\DCSoft.Writer.ForASPNETCore.dll",
               // @"d:\temp\DecryptVSEncrypt.exe",
                    //@"inputtemppath=D:\temp3\JieJie.NET_Temp",
                "outputmapxml",
                @"output=E:\Source\DCSoft.WASM.Publish\bin\test",
                //@"output=E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\DCSoft.ASPNETDemo\DCLib",
                //@"output=E:\Source\DCSoft\08代码\DCSoft\DCWriter专用版\MyWriterMvcCore\wwwroot\wasm\_framework",
                //@"output=E:\Source\DCSoft\08代码\DCSoft\DCSoft.WASM\bin\Debug\net7.0\wwwroot\_framework",
                //@"output=E:\Source\DCSoft\08代码\ConsoleApp1\Lib",
                //@"output=C:\Users\yfyuan\.nuget\packages\dcsoft.writer.forwasm\1.2023.3.10\lib\net7.0",
                //@"output=E:\Source\DCSoft\08代码\DCSoft\DCSoft.WASMTest\Lib",
                //@"output=C:\Users\yfyuan\.nuget\packages\dcsoft.writer.forwasm\1.2023.3.10\lib\net7.0\",
                //@"output=E:\Source\DCSoftDemoCenter\08代码\旧版演示程序\DCSoft.DCWriterSimpleDemo\Lib",
                //@"output=D:\temp\DCWriterCoreMVCDemo30\DCWriterCoreMVCDemo",
                @"snk=E:\Source\DCSoft\08代码\DCSoft\yyf.snk",
                @"merge=*",
                //".subsystem=3",
                //".corflags=1",
                "switch=-rename,-controlflow,+strings,+resources,+memberorder,+removemember,-allocationcallstack",
                "StringsSelector=+DCSoft.DCSR|+ShowAboutDialog|+GetNotSupportModules|+DCSoft.MyLicense*|-*",
                "SpecifyRename=DCSoft.Writer.Controls.IWASMParent,DCSoft.Common.FileHeaderHelper,DCSoft.Common.TickSpanTable,DCSoft.Common.TypeConverterSupportProperties,DCSoft.Common.ListDebugView,DCSoft.Writer.Controls.WASMEnvironment,DCSoft.Writer.Dom.Undo.XTextDocumentUndoList,DCSoft.Writer.Undo.*,DCSoft.Writer.Serialization.ArrayXmlReader,DCSoft.Drawing.DefaultFontNameValueAttribute",
                "RemoveTypes=DCSystem_Resources.*,DCSoft.Common.SafeCreateResourceManager",
                //"switch=+hightstrings,-rename" ,
                //"switch=-rename" ,
                //"prefixfortyperename=_jiejie._0",
                //"prefixformemberrename=_jj",
                "prefixfortyperename=zzz.z0ZzZz",
                "prefixformemberrename=z0",
                //"debugmode",
                @"ResourceNameNeedEncrypt=DCSoft\.Chart.Design\.Images\.ChartViewStyle\s*",
                "RemoveCustomAttributeTypeFullnames=System.Runtime.InteropServices.ComVisibleAttribute,DCSoft.Common.DCDescriptionAttribute,System.Runtime.InteropServices.GuidAttribute,DCSoft.Common.DCDisplayNameAttribute,System.ComponentModel.CategoryAttribute,DCSoft.Common.DCPublishAPIAttribute,System.Xml.Serialization.XmlTypeAttribute,System.Xml.Serialization.XmlIncludeAttribute,System.SerializableAttribute,System.ComponentModel.BrowsableAttribute,System.Xml.Serialization.XmlElementAttribute,System.Xml.Serialization.XmlAttributeAttribute,System.Xml.Serialization.XmlIgnoreAttribute,System.ObsoleteAttribute,System.ComponentModel.EditorBrowsableAttribute,DCSoft.Common.DCPublishAPIAttribute,DCSoft.Common.DCXSDAttribute,System.Drawing.ToolboxBitmapAttribute",
                "BlazorWebAssembly",
                //"PerformanceCounter",
                "DeadCode=All",
                "RemoveDeadCodeTypes=DCSoft.Writer.NewSerialization.NewSerializer20220801,DCSoft.Writer.NewSerialization.JsonSerializer20220818",
                "pause",
                "MeasureSizeOfMethod"
                //"notnativeconsole",
                //"OnlyForReleaseAssembly"
                };
            }
            //var p = System.Diagnostics.Process.GetCurrentProcess();
            //Console.WriteLine("Tittttttttt:" + p.. .StartInfo.UseShellExecute);
            //Console.WriteLine("aaa:" + Environment.UserInteractive);
            //return;
            ///***************************************/
            var prj = new JieJieProject();
            //SelfPerformanceCounterForTest.Start();
            ParseCommandLines(args , prj );
            prj.Run();
            //SelfPerformanceCounterForTest.Stop();


        }
        /// <summary>
        /// 解释命令行文本，设置参数
        /// </summary>
        /// <param name="args"></param>
        private static void ParseCommandLines(string[] args , JieJieProject prj )
        {
            var handle9 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.ParseCommandLines);
            prj.PauseAtLast = false;
            prj.CustomInstructions = new Dictionary<string, string>(System.StringComparer.CurrentCultureIgnoreCase);
            if (args != null)
            {
                foreach (var arg in args)
                {
                    Console.WriteLine("Get arg : " + arg);
                    int index = arg.IndexOf('=');
                    if (index > 0)
                    {
                        string argName = arg.Substring(0, index).Trim().ToLower();
                        string argValue = arg.Substring(index + 1).Trim();
                        if (argName[0] == '.')
                        {
                            prj.CustomInstructions[argName] = argValue;
                            continue;
                        }
                        switch (argName)
                        {
                            case "translate":
                                {
                                    prj.TranslateStackTraceUseMapXml = argValue;
                                    return;
                                }
                                break;
                            case "translatestack":
                                {
                                    prj.TranslateStackTraceUseMapXml = argValue;
                                    return;
                                }
                                break;
                            case "inputtemppath":
                                prj.InputTempPath = argValue;
                                break;
                            case "input":
                                prj.InputAssemblyFileName = argValue;
                                if (File.Exists(prj.InputAssemblyFileName) == false)
                                {
                                    return;
                                }
                                break;
                            case "output":
                                prj.OutputAssemblyFileName = argValue;
                                break;
                            case "snk":
                                if (argValue != null
                                    && argValue.Length > 0
                                    && File.Exists(argValue) == false)
                                {
                                    MyConsole.Instance.WriteError("Can not find file : " + argValue);
                                    return;
                                }
                                prj.SnkFileName = argValue;
                                break;
                            case "switch":
                                prj.Switchs = new JieJieSwitchs(argValue, null , null );
                                break;
                            case "stringsselector":
                                prj.StringsSelector = argValue;
                                break;
                            case "sdkpath":
                                if (argValue != null
                                    && argValue.Length > 0
                                    && Directory.Exists(argValue) == false)
                                {
                                    MyConsole.Instance.WriteError("Can not find directory : " + argValue);
                                    return;
                                }
                                prj.SDKDirectory = argValue;
                                break;
                            case "prefixfortyperename":
                                if (argValue != null && argValue.Length > 0)
                                {
                                    prj.PrefixForTypeRename = argValue;
                                }
                                break;
                            case "prefixformemberrename":
                                if (argValue != null && argValue.Length > 0)
                                {
                                    prj.PrefixForMemberRename = argValue;
                                }
                                break;
                            case "merge":
                                prj.MergeFileNames = argValue;
                                break;
                            case "resourcenameneedencrypt":
                                prj.ResourceNameNeedEncrypt = argValue;
                                break;
                            case "uilanguage":
                                prj.UILanguageName = argValue;
                                break;
                            case "removecustomattributetypefullnames":
                                prj.RemoveCustomAttributeTypeFullNames = argValue;
                                break;
                            case "specifyrename":
                                prj.SpecifyRename = argValue;
                                break;
                            case "removetypes":
                                prj.RemoveTypes = argValue;
                                break;
                            case "removedeadcodetypes":
                                prj.RemoveDeadCodeTypes = argValue;
                                break;
                            case "deadcode":
                                if( string.Equals( argValue , "normal" , StringComparison.OrdinalIgnoreCase ))
                                {
                                    prj.DetectDeadCode = DetectDeadCodeMode.Normal;
                                }
                                else if( string.Equals( argValue , "disabled" , StringComparison.OrdinalIgnoreCase))
                                {
                                    prj.DetectDeadCode = DetectDeadCodeMode.Disabled;
                                }
                                else if( string.Equals( argValue , "all" , StringComparison.OrdinalIgnoreCase))
                                {
                                    prj.DetectDeadCode = DetectDeadCodeMode.All;
                                }
                                else
                                {
                                    prj.DetectDeadCode = DetectDeadCodeMode.Normal;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (arg.Trim().ToLower())
                        {
                            case "outputmapxml": prj.OutpuptMapXml = true; break;
                            case "deletetempfile": prj.DeleteTempFile = true; break;
                            case "pause": prj.PauseAtLast = true; break;
                            case "debugmode": prj.DebugMode = true; break;
                            case "blazorwebassembly": prj.ForBlazorWebAssembly = true;break;
                            case "deadcode":prj.DetectDeadCode = DetectDeadCodeMode.Normal; break;
                            case "performancecounter":prj.AddPerformanceCounter = true;break;
                            case "notnativeconsole": MyConsole.Instance.IsNativeConsole = false;break;
                            case "onlyforreleaseassembly": prj.OnlyForReleaseAssembly = true;break;
                            case "measuresizeofmethod":prj.MeasureSizeOfMethod = true;break;
                            default:
                                if (arg != null
                                    && arg.Length > 0
                                    && Path.IsPathRooted(arg))
                                {
                                    if (File.Exists(arg))
                                    {
                                        // 默认为输入的程序集的文件全路径名
                                        prj.InputAssemblyFileName = arg;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error:File not exist:" + arg);
                                    }
                                }
                                break;
                        }
                    }
                }
            }//if
            SelfPerformanceCounterForTest.Leave(handle9);
        }
    }
}
