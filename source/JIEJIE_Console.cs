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
        static void Main(string[] args)
        {
             
            ///***************************************/
            var prj = new JieJieProject();
            ParseCommandLines(args , prj );
            prj.Run();
        }
        /// <summary>
        /// 解释命令行文本，设置参数
        /// </summary>
        /// <param name="args"></param>
        private static void ParseCommandLines(string[] args , JieJieProject prj )
        {
            prj.PauseAtLast = false;
            prj.CustomInstructions = new Dictionary<string, string>(System.StringComparer.CurrentCultureIgnoreCase);
            if (args != null)
            {
                foreach (var arg in args)
                {
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
                                prj.Switchs = new JieJieSwitchs(argValue, null,null);
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
                            default:
                                if (arg != null
                                    && arg.Length > 0
                                    && Path.IsPathRooted(arg)
                                    && File.Exists(arg))
                                {
                                    // 默认为输入的程序集的文件全路径名
                                    prj.InputAssemblyFileName = arg;
                                }
                                break;
                        }
                    }
                }
            }//if
        }
    }
}
