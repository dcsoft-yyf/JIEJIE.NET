/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace JIEJIE
{
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

    /*

    "C:\Users\yfyuan\.nuget\packages\microsoft.netcore.app.crossgen2.win-x64\6.0.0\tools\crossgen2.exe"  --jitpath "C:\Users\yfyuan\.nuget\packages\microsoft.netcore.app.runtime.win-x64\6.0.8\runtimes\win-x64\native\clrjit.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\netstandard.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Collections.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.ComponentModel.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.ComponentModel.EventBasedAsync.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.ComponentModel.Primitives.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.ComponentModel.TypeConverter.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Console.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Drawing.Primitives.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.IO.Compression.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Memory.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.ObjectModel.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Private.CoreLib.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Private.Uri.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Private.Xml.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Private.Xml.Linq.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Runtime.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Runtime.InteropServices.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Runtime.Serialization.Formatters.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Text.Encoding.Extensions.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Text.Json.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Text.RegularExpressions.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Threading.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Web.HttpUtility.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Xml.ReaderWriter.dll" -r "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\6.0.8\System.Xml.XmlSerializer.dll" -r "C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\6.0.8\System.Resources.Extensions.dll"  "E:\Source\DCSoft\08代码\DCSoft\DCSoft.WASM\bin\Debug\net6.0\wwwroot\_framework\DCSoft.Writer.ForWASM.dll" --out "d:\temp.dll" 
    
    */

}
