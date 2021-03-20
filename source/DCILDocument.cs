using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Resources;

namespace DCNETProtector
{
    internal class DCILDocument : DCILGroup,IDisposable
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

        public DCILDocument( string ilFileName , Encoding encod)
        {
            if( ilFileName == null )
            {
                throw new ArgumentNullException("ilFileName");
            }
            if( File.Exists( ilFileName ) == false )
            {
                throw new FileNotFoundException(ilFileName);
            }
            this.FileSize = (int)new FileInfo(ilFileName).Length;
            if( encod == null )
            {
                encod = Encoding.Default;
            }
            using (var reader = new System.IO.StreamReader(ilFileName, Encoding.Default, true))
            {
                var line = reader.ReadLine();
                var list = new List<string>();
                while( line != null )
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
        public DCILDocument( string rootPath , string[] srcLines )
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
            if( this.StringDefines != null)
            {
                this.StringDefines.Clear();
                this.StringDefines = null;
            }
            if( this.AllGroups != null )
            {
                this.AllGroups.Clear();
                this.AllGroups = null;
            }
            if( this.ResouceFiles != null )
            {
                this.ResouceFiles.Clear();
                this.ResouceFiles = null;
            }
        }

        public bool _IsDotNetCoreAssembly = false;

        public  List<DCILClass> GetClasses( string name )
        {
            var list = new List<DCILClass>();
            foreach( var item in this.ChildNodes )
            {
                if( item is DCILClass && item.Name == name )
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
            if( this.LibName_mscorlib == null )
            {
                this.LibName_mscorlib = "mscorlib";
            }
            tick = Math.Abs( Environment.TickCount - tick);
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
        private bool IsEmptyLine( string line )
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
        private void GetFinalValue(string line , StringBuilder result )
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
                else if( c == '"')
                {
                    break;
                }
                else
                {
                    result.Append(c);
                }
            }
        }

        private string RemoveComment( string line )
        {
            int index = line.IndexOf("//", StringComparison.Ordinal);
            if (index == 0)
            {
                return string.Empty;
            }
            else if( index > 0 )
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
        internal static int IndexOfHexChar( char c )
        {
            if( c >= '0' && c <='9')
            {
                return c - '0';
            }
            else if( c >='a' && c <='f')
            {
                return c - 'a' + 10;
            }
            else if( c >='A' && c <='F')
            {
                return c - 'A' + 10;
            }
            return -1;
        }
        internal static byte[] HexToBinary( string hexs )
        {
            if( hexs == null || hexs.Length == 0 )
            {
                return null;
            }
            var list = new List<byte>( hexs.Length / 2 );
            byte bytCurrentValue = 0;
            bool addFlag = false;
            foreach( var c in hexs)
            {
                int index = IndexOfHexChar(c);
                if (index >= 0 )
                {
                    if( addFlag )
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
        internal static string GetHexString( string srcText )
        {
            var str = new StringBuilder();
            foreach( var c in srcText)
            {
                if( IndexOfHexChar( c ) >= 0)
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
            return list ;
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
   
   
    
    
    
    
    
    
    
}
