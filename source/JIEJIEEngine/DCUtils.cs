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

namespace JIEJIE
{
    internal static class DCUtils
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool DeleteFile(string fileName )
        {
            if(fileName != null && fileName.Length > 0 && File.Exists( fileName ))
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
                MyConsole.Instance.WriteLine(" Delete file " + fileName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 去除文本两边的分号
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>处理后的文本</returns>
        public static string CleanSurroundingSemicolon(string text)
        {
            if (text != null && text.Length > 0)
            {
                if (text[0] == '\'' && text[text.Length - 1] == '\'')
                {
                    return text.Substring(1, text.Length - 2);
                }
                else if (text[0] == '"' && text[text.Length - 1] == '"')
                {
                    return text.Substring(1, text.Length - 2);
                }
            }
            return text;
        }

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
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.RunExe);
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
            SelfPerformanceCounterForTest.Leave(h4);
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

        public static bool DeleteFileDeeeply( string rootDir , string simpleFileName )
        {
            var result = false;
            var fn = Path.Combine(rootDir, simpleFileName);
            if( File.Exists( fn ))
            {
                File.SetAttributes(fn, FileAttributes.Normal);
                File.Delete(fn);
                MyConsole.Instance.WriteLine("   Delete file " + fn);
                result = true;
            }
            foreach (var dir in Directory.GetDirectories(rootDir))
            {
                if( DeleteFileDeeeply(dir, simpleFileName))
                {
                    result = true;
                }
            }
            return result;
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
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.ObfuseListOrder);
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
            SelfPerformanceCounterForTest.Leave(h4);
        }

    }


}
