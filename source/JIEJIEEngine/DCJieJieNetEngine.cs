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
using System.Reflection;

namespace JIEJIE
{
    [Serializable]
    internal class DCJieJieNetEngine : System.MarshalByRefObject, IDisposable
    {
        public const string ProductVersion = "1.2022.11.7";


        public DCJieJieNetEngine(DCILDocument doc)
        {
            this.Document = doc;
        }
        public DCJieJieNetEngine()
        {
        }
        /// <summary>
        /// 设置命令行界面操作接口
        /// </summary>
        /// <param name="instance">对象实例</param>
        public void SetConsoleInstance(MyConsole instance)
        {
            MyConsole.SetInstance(instance);
        }

#if !DOTNETCORE
        public void BindCurrentDomain_AssemblyResolve()
        {
            System.AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmName = typeof(JieJieProject).Assembly.GetName().Name;
            if (args.Name.StartsWith(asmName))
            {
                return typeof(JieJieProject).Assembly;
            }
            return null;
        }

#endif
        ///// <summary>
        ///// 只针对Release模式的编译结果而执行本程序
        ///// </summary>
        ///// <remarks>
        ///// 当本程序作为VS.NET的编译后事件而运行时，一般而言调试模式无需执行混淆加密操作。Release模式才需要混淆加密。
        ///// </remarks>
        //public bool OnlyForReleaseAssembly = false;
        /// <summary>
        /// 是否为正常的命令行模式。可以读取键盘输入，可以移动光标。
        /// </summary>
        /// <remarks>当本程序作为VS.NET的编译后事件而运行时，不支持键盘和光标的操作。此时设置本属性为false.</remarks>
        public bool IsNativeConsole = true;
        /// <summary>
        /// 要删除的类型的名称
        /// </summary>
        public string RemoveTypes = null;
        /// <summary>
        /// 指定的重命名设置
        /// </summary>
        public string SpeicfyRename = null;
        /// <summary>
        /// 添加性能累计器
        /// </summary>
        public bool AddPerformanceCounter = false;
        /// <summary>
        /// 针对Blazor WebAssembly进行处理
        /// </summary>
        public bool ForBlazorWebAssembly = false;
        /// <summary>
        /// 在重命名后检测死代码
        /// </summary>
        public DetectDeadCodeMode DetectDeadCode = DetectDeadCodeMode.Disabled;
        public string RemoveDeadCodeTypes = null;
        /// <summary>
        ///  复制系统配置
        /// </summary>
        /// <param name="eng">复制输出对象</param>
        public void CopytSettingsTo(DCJieJieNetEngine eng)
        {
            eng.ForBlazorWebAssembly = this.ForBlazorWebAssembly;
            eng.RemoveCustomAttributeTypeFullNames = this.RemoveCustomAttributeTypeFullNames;
            eng.ContentEncoding = this.ContentEncoding;
            eng.DebugMode = this.DebugMode;
            eng.PrefixForTypeRename = this.PrefixForTypeRename;
            eng.PrefixForMemberRename = this.PrefixForMemberRename;
            eng.SDKDirectory = this.SDKDirectory;
            eng.SnkFileName = this.SnkFileName;
            eng.DetectDeadCode = this.DetectDeadCode;
            eng.IsNativeConsole = this.IsNativeConsole;
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
            eng.ResourceNameNeedEncrypt = this.ResourceNameNeedEncrypt;
            eng.SpeicfyRename = this.SpeicfyRename;
            eng.RemoveTypes = this.RemoveTypes;
            eng.RemoveDeadCodeTypes = this.RemoveDeadCodeTypes;
            eng.StringsSelector = this.StringsSelector;
        }
        /// <summary>
        /// 关闭对象
        /// </summary>
        public void Dispose()
        {
            DCILTypeReference.ClearGlobalBuffer();

            DCUtils.ClearStringTable();
            if (this.Document != null)
            {
                this.Document.Dispose();
                this.Document = null;
            }
            if (this._AllClasses != null)
            {
                this._AllClasses.Clear();
                this._AllClasses = null;
            }
            if (this._Type_JIEJIEHelper != null)
            {
                this._Type_JIEJIEHelper.LocalClass?.Dispose();
                this._Type_JIEJIEHelper.Dispose();
                this._Type_JIEJIEHelper = null;
            }
            this.Switchs = null;
            this._CallOperCodes?.Clear();
            this._CallOperCodes = null;
            this._AllClasses?.Clear();
            //this._AllBaseTypes.Clear();
            //this._IDGenForClass = null;
            _NativeTypeMethods.Clear();
            _Native_BaseMethods.Clear();
            //this._RuntimeSwitchs.Clear();
            if (this._Int32ValueContainer != null)
            {
                this._Int32ValueContainer.Dispose();
                this._Int32ValueContainer = null;
            }
            if (this._RFHContainer != null)
            {
                this._RFHContainer.Dispose();
                this._RFHContainer = null;
            }
            if (this._ByteDataContainer != null)
            {
                this._ByteDataContainer.Dispose();
                this._ByteDataContainer = null;
            }
            if (this._Int32ValueString != null)
            {
                this._Int32ValueString.Dispose();
                this._Int32ValueString = null;
            }

            DCILTypeReference.ClearCacheNativeTypes();
            if (this._Cache_BaseMethods != null)
            {
                foreach (var item in this._Cache_BaseMethods)
                {
                    item.Key.Dispose();
                    foreach (var m in item.Value)
                    {
                        m.Dispose();
                    }
                    item.Value.Clear();
                }
                this._Cache_BaseMethods.Clear();
                this._Cache_BaseMethods = null;
            }
            if (this._CallOperCodes != null)
            {
                this._CallOperCodes.Clear();
                this._CallOperCodes = null;
            }
            this._NewPNameGen = null;
            this.ContentEncoding = null;
            this.SnkFileName = null;
            this._InputAssemblyDirectory = null;
            this._InputAssemblyFileName = null;
            this.TempDirectory = null;
            DCILMethod.ClearNewLabelIDCache();
            GC.Collect();
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
                MyConsole.Instance.Write("Deleting template directory : " + this.TempDirectory);
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
                            if (m2.HasStyle("private") || m2.HasStyle("assembly"))
                            {
                                continue;
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
                            if (f.HasStyle("private") || f.HasStyle("assembly"))
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
                MyConsole.Instance.WriteLine("Write comment XML file \"" + descFileName + "\" ,remove " + removeCount + "(" + percent + "%)members,span " + tick + " milliseconds.");
                return true;
            }
            catch (System.Exception ext)
            {
                MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                MyConsole.Instance.WriteLine(ext.ToString());
                MyConsole.Instance.ResetColor();
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
                MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                MyConsole.Instance.WriteLine("Can not find file " + mapXmlFileName);
                MyConsole.Instance.ResetColor();
                return;
            }

            MyConsole.Instance.WriteLine("JIEJIE.NET translate stack trace use MAP.XML ");
            MyConsole.Instance.WriteLine("MAP xml file : " + mapXmlFileName);
            MyConsole.Instance.Write("Please paste or input stack trace text and press ");
            MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
            MyConsole.Instance.BackgroundColor = ConsoleColor.Black;
            MyConsole.Instance.Write("ESC");
            MyConsole.Instance.ResetColor();
            MyConsole.Instance.WriteLine(" to finish:");
            var strBuffer = new System.Text.StringBuilder();
            while (true)
            {
                var info = MyConsole.Instance.ReadKey();
                if (info.Key == ConsoleKey.Escape)
                {
                    break;
                }
                if (info.KeyChar > 7)
                {
                    strBuffer.Append(info.KeyChar.ToString());
                    if (info.Key == ConsoleKey.Enter)
                    {
                        MyConsole.Instance.WriteLine();
                    }
                }
            }
            var strResult = DCJieJieNetEngine.TranslateStackTraceUseMapXml(mapXmlFileName, strBuffer.ToString());
            MyConsole.Instance.WriteLine();
            MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
            MyConsole.Instance.WriteLine(" ########## Translate result ##########");
            ConoleWriteStackTrace(strResult);
            MyConsole.Instance.WriteLine("### Press Enter to exit. ###");
            MyConsole.Instance.ReadLine();
        }

        public static void ConoleWriteStackTrace( string strResult )
        {
            if(strResult == null || strResult.Length == 0 )
            {
                return;
            }
            MyConsole.Instance.ResetColor();
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
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                        MyConsole.Instance.Write(strMethodName.Substring(0, index2));
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                        MyConsole.Instance.Write(strMethodName.Substring(index2));
                    }
                    else
                    {
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                        MyConsole.Instance.Write(strLine.Substring(0, index));
                    }
                    MyConsole.Instance.ResetColor();
                    MyConsole.Instance.Write(strLine.Substring(index));
                    MyConsole.Instance.WriteLine();
                }
                else
                {
                    MyConsole.Instance.WriteLine(strLine);
                }
                strLine = reader.ReadLine();
            }
            reader.Close();
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
                if (maps.ContainsKey(strWord) == false)
                {
                    string clsName = null;
                    string methodName = null;
                    if (strWord.IndexOf('.') > 0 && reader.PeekContentChar() == '(')
                    {
                        methodName = DCUtils.GetShortName(strWord, out clsName);
                    }
                    else if (reader.PeekContentChar() == ':')
                    {
                        if (reader.ReadContentChar() == ':' && reader.Read() == ':')
                        {
                            clsName = strWord;
                            methodName = reader.ReadWord();
                        }
                    }
                    if (clsName != null && methodName != null)
                    {
                        if (maps.ContainsKey(clsName + "." + methodName))
                        {
                            strWord = clsName + "." + methodName;
                        }
                        else
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
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
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
                        strResult.Append(infos[0].OldSign + "[!! Maby !!]");
                    }
                    strResult.Append(reader.ReadLineTrim());
                    strResult.AppendLine();
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
            methods.Sort(delegate (DCILMethod a, DCILMethod b)
            {
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
            var hs = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.SaveAssemblyFile);
            if (InnerSaveAssemblyFile(asmFileName, checkUseNgen))
            {
                if (this.ForBlazorWebAssembly)
                {
                    // 为 Blazor WebAssembly 而更新文件
                    UpdateForBlazorWebAssembly(asmFileName);
                    //foreach (var cls in this.Document.GetAllClassesUseCache().Values)
                    //{
                    //    if (cls.Name.StartsWith("DCSoft.Chart"))
                    //    {
                    //        foreach (var item in cls.ChildNodes)
                    //        {
                    //            if (item is DCILMethod)
                    //            {
                    //                var m = (DCILMethod)item;
                    //                if (m.OperCodes != null && m.OperCodes.Count > 10)
                    //                {
                    //                    m.OperCodes.Clear();
                    //                    m.OperCodes.AddItem("il00333", "newobj", "instance void [System.Runtime]System.NotSupportedException::.ctor()");
                    //                    m.OperCodes.AddItem("il00044", "throw");
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //this.OutpuptMapXml = false;
                    //var fn2 = asmFileName + "_empty.dll";
                    //if (InnerSaveAssemblyFile(fn2, false))
                    //{
                    //    var sha256 = System.Security.Cryptography.SHA256.Create();
                    //    var bs256 = sha256.ComputeHash(File.ReadAllBytes(fn2));
                    //    var strSHA256_Base64 = Convert.ToBase64String(bs256);
                    //    File.WriteAllText(fn2 + ".sha256.txt", strSHA256_Base64);
                    //}
                }
                return true;
            }
            else
            {
                return false;
            }
//            ConsoleWriteTask();
//            MyConsole.Instance.WriteLine("Saving assembly to " + asmFileName);
//            var ilFileName = Path.Combine(this.Document.RootPath, "result_" + Path.GetFileName(asmFileName) + ".il");
//            MyConsole.Instance.WriteLine("    Writing IL codes to " + ilFileName);
//            this.Document.WriteToFile(ilFileName, this.ContentEncoding);
//            if (_PathOfildasm == null)
//            {
//                _PathOfildasm = Path.GetDirectoryName(typeof(string).Assembly.Location);
//#if DOTNETCORE
//                if ( File.Exists( Path.Combine( _PathOfildasm , "ilasm.exe")) == false )
//                {
//                    _PathOfildasm = null;
//                    if (Directory.Exists(@"C:\Windows\Microsoft.NET\Framework64"))
//                    {
//                        _PathOfildasm = @"C:\Windows\Microsoft.NET\Framework64";
//                    }
//                    else if( Directory.Exists(@"C:\Windows\Microsoft.NET\Framework"))
//                    {
//                        _PathOfildasm = @"C:\Windows\Microsoft.NET\Framework";
//                    }
//                    if( _PathOfildasm != null )
//                    {
//                        var list = new List<string>(Directory.GetDirectories(_PathOfildasm));
//                        _PathOfildasm = null;
//                        list.Sort();
//                        for( int iCount = list.Count -1; iCount >=0;iCount -- )
//                        {
//                            var item = Path.GetFileName( list[iCount]);
//                            if( item.Length > 4 && item[0]=='v' && char.IsDigit( item[1]))
//                            {
//                                _PathOfildasm = list[iCount];
//                                break;
//                            }
//                        }
//                    }
//                    if( _PathOfildasm == null )
//                    {
//                        throw new Exception("Can not find ilasm.exe path.");
//                    }
//                }
//#endif
//            }

//            var asmTempFileName = Path.Combine(this.TempDirectory, "Temp_" + Path.GetFileName(asmFileName));
//            if (File.Exists(asmTempFileName))
//            {
//                File.Delete(asmTempFileName);
//            }
//            // 生成临时程序集文件
//            var ilAsmArgs = "\"" + ilFileName + "\"  \"/output:" + asmTempFileName + "\"";
//            if (asmTempFileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
//            {
//                ilAsmArgs = ilAsmArgs + " /exe";
//            }
//            else
//            {
//                ilAsmArgs = ilAsmArgs + " /dll";
//            }
//            if (this.Document.Win32ResData != null && this.Document.Win32ResData.Length > 0)
//            {
//                var resFileName = Path.ChangeExtension(ilFileName, ".res");
//                if (File.Exists(resFileName) == false)
//                {
//                    File.WriteAllBytes(resFileName, this.Document.Win32ResData);
//                }
//                ilAsmArgs = ilAsmArgs + " \"/resource=" + resFileName + "\"";
//            }
//            if (this.Document.RuntimeVersion != null && this.Document.RuntimeVersion.Length > 0)
//            {
//                ilAsmArgs = ilAsmArgs + " /mdv=" + this.Document.RuntimeVersion + " /msv:" + this.Document.RuntimeVersion;
//            }
//            if (this.DebugMode == false)
//            {
//                ilAsmArgs = ilAsmArgs + " /quiet";
//            }
//            DCUtils.RunExe(Path.Combine(_PathOfildasm, "ilasm.exe"), ilAsmArgs);
//            //if (this.DebugMode)
//            //{
//            //    ResourceFileHelper.RunExe(Path.Combine(_PathOfildasm, "ilasm.exe"), "\"" + ilFileName + "\" /dll  \"/output:" + asmTempFileName + "\"");
//            //}
//            //else
//            //{
//            //    ResourceFileHelper.RunExe(Path.Combine(_PathOfildasm, "ilasm.exe"), "\"" + ilFileName + "\" /dll  \"/output:" + asmTempFileName + "\" /quiet");
//            //}
//            if (File.Exists(asmTempFileName))
//            {
//                if (this.SnkFileName != null && this.SnkFileName.Length > 0 && File.Exists(this.SnkFileName))
//                {
//                    DCUtils.RunExe(Path.Combine(this.SDKDirectory, "sn.exe"), "-Ra \"" + asmTempFileName + "\" " + this.SnkFileName);
//                }
//                if (checkUseNgen)
//                {
//                    ConsoleWriteTask();
//#if !DOTNETCORE // .NET Core not support ngen.exe
//                    MyConsole.Instance.WriteLine("Testing by ngen.exe...");
//                    string pathNgen = Path.Combine(_PathOfildasm, "ngen.exe");
//                    if (this.Document.Value_CorFlags > 0 && ((this.Document.Value_CorFlags & 2) == 2))
//                    {
//                        // 32位PE文件
//                        if (_PathOfildasm.Contains("Framework64"))
//                        {
//                            pathNgen = _PathOfildasm.Replace("Framework64", "Framework");
//                            pathNgen = Path.Combine(pathNgen, "ngen.exe");
//                        }
//                    }
//                    if (File.Exists(pathNgen))
//                    {
//                        string appBase = null;
//                        if( this.Document.AssemblyFileName != null 
//                            && this.Document.AssemblyFileName.Length > 0
//                            && File.Exists( this.Document.AssemblyFileName))
//                        {
//                            appBase = " \"/AppBase:" + Path.GetDirectoryName( this.Document.AssemblyFileName ) + "\"";
//                        }
//                        DCUtils.RunExe(pathNgen, "install \"" + asmTempFileName + "\"  /NoDependencies" + appBase);
//                        DCUtils.RunExe(pathNgen, "uninstall \"" + asmTempFileName + "\"");
//                    }
//                    else
//                    {
//                        MyConsole.Instance.WriteLine("can not find file : " + pathNgen);
//                    }
//#else
//                    MyConsole.Instance.WriteLine("Testing by crossgen.exe...");
//                    CrossGenHelper hp = new CrossGenHelper();
//                    hp.TestByCrossGen(this.Document, asmTempFileName);
//                    //Console.WriteLine(".NET Core not support ngen.exe");
//#endif
//                }
//                File.Copy(asmTempFileName, asmFileName, true);
//                File.Delete(asmTempFileName);
//                if (this.Switchs.Rename && this.OutpuptMapXml)
//                {
//                    ConsoleWriteTask();
//                    var hwm = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.WriteMapXml);
//                    string strMapFileName = this._SpecifyOutputMapXmlFileName;
//                    if (string.IsNullOrEmpty(strMapFileName))
//                    {
//                        strMapFileName = asmFileName + ".map.xml";
//                    }
//                    using (var writer = new System.Xml.XmlTextWriter(strMapFileName, Encoding.UTF8))
//                    {
//                        writer.Formatting = System.Xml.Formatting.Indented;
//                        writer.IndentChar = ' ';
//                        writer.Indentation = 3;
//                        this.WriteMapXml2(writer);
//                    }
//                    MyConsole.Instance.WriteLine("Write rename map xml to\"" + strMapFileName + "\".");
//                    SelfPerformanceCounterForTest.Leave(hwm);
//                }
//                if (this.Document.Content_DepsJson != null && this.Document.Content_DepsJson.Length > 0)
//                {
//                    var fn2 = Path.ChangeExtension(asmFileName, ".deps.json");
//                    MyConsole.Instance.WriteLine("    Write " + fn2);
//                    File.WriteAllBytes(fn2, this.Document.Content_DepsJson);
//                }
//                if (this.Document.CommentXmlDoc != null && this.Document.CommentXmlDoc.DocumentElement?.Name == "doc")
//                {
//                    // clean document comment xml file.
//                    var comXmlFileName = Path.ChangeExtension(asmFileName, ".xml");
//                    WriteDocumentCommentXml(comXmlFileName);
//                }

//                MyConsole.Instance.WriteLine();
//                ConsoleWriteTask();
//                MyConsole.Instance.Write("Job finished, final save to :");
//                MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
//                MyConsole.Instance.WriteLine(asmFileName);
//                MyConsole.Instance.ResetColor();
//                if (this._SourceAssemblyFileSize > 0)
//                {
//                    MyConsole.Instance.Write(" Source file size : " + DCUtils.FormatByteSize(this._SourceAssemblyFileSize) + " ,");
//                }
//                var newFileLength = new FileInfo(asmFileName).Length;
//                MyConsole.Instance.WriteLine(" Result file size : " + DCUtils.FormatByteSize(newFileLength));
//                if( this.ForBlazorWebAssembly)
//                {
//                    // 为 Blazor WebAssembly 而更新文件
//                    UpdateForBlazorWebAssembly(asmFileName);
//                }
//                SelfPerformanceCounterForTest.Leave(hs);
//                return true;
//            }
//            else
//            {
//                ConsoleWriteTask();
//                MyConsole.Instance.WriteLine("Job failed.");
//                return false;
//            }
        }

        /// <summary>
        /// 保存程序集文件
        /// </summary>
        /// <param name="asmFileName">程序集文件</param>
        /// <param name="checkUseNgen">是否使用 Ngen.exe 进行验证</param>
        /// <returns>操作是否成功</returns>
        public bool InnerSaveAssemblyFile(string asmFileName, bool checkUseNgen)
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
            MyConsole.Instance.WriteLine("Saving assembly to " + asmFileName);
            var ilFileName = Path.Combine(this.Document.RootPath, "result_" + Path.GetFileName(asmFileName) + ".il");
            MyConsole.Instance.WriteLine("    Writing IL codes to " + ilFileName);
            this.Document.WriteToFile(ilFileName, this.ContentEncoding);
            if (_PathOfildasm == null)
            {
                _PathOfildasm = Path.GetDirectoryName(typeof(string).Assembly.Location);
#if DOTNETCORE
                if (File.Exists(Path.Combine(_PathOfildasm, "ilasm.exe")) == false)
                {
                    _PathOfildasm = null;
                    if (Directory.Exists(@"C:\Windows\Microsoft.NET\Framework64"))
                    {
                        _PathOfildasm = @"C:\Windows\Microsoft.NET\Framework64";
                    }
                    else if (Directory.Exists(@"C:\Windows\Microsoft.NET\Framework"))
                    {
                        _PathOfildasm = @"C:\Windows\Microsoft.NET\Framework";
                    }
                    if (_PathOfildasm != null)
                    {
                        var list = new List<string>(Directory.GetDirectories(_PathOfildasm));
                        _PathOfildasm = null;
                        list.Sort();
                        for (int iCount = list.Count - 1; iCount >= 0; iCount--)
                        {
                            var item = Path.GetFileName(list[iCount]);
                            if (item.Length > 4 && item[0] == 'v' && char.IsDigit(item[1]))
                            {
                                _PathOfildasm = list[iCount];
                                break;
                            }
                        }
                    }
                    if (_PathOfildasm == null)
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
                if (checkUseNgen)
                {
                    ConsoleWriteTask();
#if !DOTNETCORE // .NET Core not support ngen.exe
                    MyConsole.Instance.WriteLine("Testing by ngen.exe...");
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
                        string appBase = null;
                        if( this.Document.AssemblyFileName != null 
                            && this.Document.AssemblyFileName.Length > 0
                            && File.Exists( this.Document.AssemblyFileName))
                        {
                            appBase = " \"/AppBase:" + Path.GetDirectoryName( this.Document.AssemblyFileName ) + "\"";
                        }
                        DCUtils.RunExe(pathNgen, "install \"" + asmTempFileName + "\"  /NoDependencies" + appBase);
                        DCUtils.RunExe(pathNgen, "uninstall \"" + asmTempFileName + "\"");
                    }
                    else
                    {
                        MyConsole.Instance.WriteLine("can not find file : " + pathNgen);
                    }
#else
                    MyConsole.Instance.WriteLine("Testing by crossgen.exe...");
                    CrossGenHelper hp = new CrossGenHelper();
                    hp.TestByCrossGen(this.Document, asmTempFileName);
                    //Console.WriteLine(".NET Core not support ngen.exe");
#endif
                }
                File.Copy(asmTempFileName, asmFileName, true);
                File.Delete(asmTempFileName);
                if (this.Switchs.Rename && this.OutpuptMapXml)
                {
                    ConsoleWriteTask();
                    var hwm = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.WriteMapXml);
                    string strMapFileName = this._SpecifyOutputMapXmlFileName;
                    if (string.IsNullOrEmpty(strMapFileName))
                    {
                        strMapFileName = asmFileName + ".map.xml";
                    }
                    using (var writer = new System.Xml.XmlTextWriter(strMapFileName, Encoding.UTF8))
                    {
                        writer.Formatting = System.Xml.Formatting.Indented;
                        writer.IndentChar = ' ';
                        writer.Indentation = 3;
                        this.WriteMapXml2(writer);
                    }
                    MyConsole.Instance.WriteLine("Write rename map xml to\"" + strMapFileName + "\".");
                    SelfPerformanceCounterForTest.Leave(hwm);
                }
                if (this.Document.Content_DepsJson != null && this.Document.Content_DepsJson.Length > 0)
                {
                    var fn2 = Path.ChangeExtension(asmFileName, ".deps.json");
                    MyConsole.Instance.WriteLine("    Write " + fn2);
                    File.WriteAllBytes(fn2, this.Document.Content_DepsJson);
                }
                if (this.Document.CommentXmlDoc != null && this.Document.CommentXmlDoc.DocumentElement?.Name == "doc")
                {
                    // clean document comment xml file.
                    var comXmlFileName = Path.ChangeExtension(asmFileName, ".xml");
                    WriteDocumentCommentXml(comXmlFileName);
                }
                if( this.MeasureSizeOfMethod && this._SizeOfMethod != null )
                {
                    var fn6 = asmFileName + ".SizeOfMethod.xml";
                    this._SizeOfMethod.Save( fn6);
                    MyConsole.Instance.WriteLine("    Write " + fn6);
                }
                MyConsole.Instance.WriteLine();
                ConsoleWriteTask();
                MyConsole.Instance.Write("Job finished, final save to :");
                MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                MyConsole.Instance.WriteLine(asmFileName);
                MyConsole.Instance.ResetColor();
                if (this._SourceAssemblyFileSize > 0)
                {
                    MyConsole.Instance.Write(" Source file size : " + DCUtils.FormatByteSize(this._SourceAssemblyFileSize) + " ,");
                }
                var newFileLength = new FileInfo(asmFileName).Length;
                MyConsole.Instance.WriteLine(" Result file size : " + DCUtils.FormatByteSize(newFileLength));
                return true;
            }
            else
            {
                ConsoleWriteTask();
                MyConsole.Instance.WriteLine("Job failed.");
                return false;
            }
        }

        /// <summary>
        /// 为了BlazorWebAssembly而更新信息
        /// </summary>
        private void UpdateForBlazorWebAssembly( string outputAssemblyFileName )
        {
            // 为 Blazor WebAssembly 进行输出
            var handle34 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.BlazorWebAssembly);
            this.ConsoleWriteTask();
            MyConsole.Instance.WriteLine("Run for Blazor WebAssembly...");
            var workDir = Path.GetDirectoryName(outputAssemblyFileName);
            var pathItems = outputAssemblyFileName.Split(Path.DirectorySeparatorChar);
            // 搜索obj目录下的同名程序集文件,若找到需要替换，因为AOT会使用这个文件
            string objOutputDir = null;
            if( pathItems != null 
                && pathItems.Length > 4 
                && pathItems[ pathItems.Length - 4 ].IndexOf("bin",StringComparison.OrdinalIgnoreCase) >=0)
            {
                pathItems[pathItems.Length - 4] = pathItems[pathItems.Length - 4].Replace("bin", "obj");
                objOutputDir = pathItems[0];
                for (var iCount = 1; iCount < pathItems.Length -1; iCount++)
                {
                    objOutputDir = objOutputDir + Path.DirectorySeparatorChar + pathItems[iCount];
                }
                if( Directory.Exists(objOutputDir) == false )
                {
                    objOutputDir = null;
                }
            }
            if( objOutputDir != null )
            {
                var fn2 = Path.Combine(objOutputDir, Path.GetFileName(outputAssemblyFileName));
                if( File.Exists( fn2 ))
                {
                    File.SetAttributes(fn2, FileAttributes.Normal);
                    File.Copy(outputAssemblyFileName, fn2, true);
                    MyConsole.Instance.WriteLine("   Overwrite " + fn2);
                }
            }
            string wwwRootDir = Path.Combine(Path.GetDirectoryName(outputAssemblyFileName), "wwwroot");
            if( Directory.Exists( wwwRootDir))
            {
                var fn3 = DCUtils.SearchFileDeeply(wwwRootDir, Path.GetFileName(outputAssemblyFileName));
                if( File.Exists( fn3 ))
                {
                    wwwRootDir = Path.GetDirectoryName(fn3);
                    File.SetAttributes(fn3, FileAttributes.Normal);
                    File.Copy(outputAssemblyFileName, fn3, true);
                    MyConsole.Instance.WriteLine("   Overwrite " + fn3);
                }
                else
                {
                    wwwRootDir = null;
                }
            }
            else
            {
                wwwRootDir = null;
            }
            // 同步在其他目录下的目标程序集文件
            if( objOutputDir != null )
            {
                ReplaceFileDeeply(outputAssemblyFileName, objOutputDir);
            }
            if(wwwRootDir != null )
            {
                ReplaceFileDeeply(outputAssemblyFileName, wwwRootDir);
            }
            ReplaceFileDeeply(outputAssemblyFileName, Path.GetDirectoryName(outputAssemblyFileName));

            var bsNative = System.IO.File.ReadAllBytes(outputAssemblyFileName);
            var ms = new System.IO.MemoryStream();
            var gzStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress);
            gzStream.Write(bsNative, 0, bsNative.Length);
            gzStream.Close();
            var bsWrite = ms.ToArray();
            ms.Close();
            var gzFileName = outputAssemblyFileName + ".gz";
            if (File.Exists(gzFileName))
            {
                System.IO.File.WriteAllBytes(gzFileName, bsWrite);
                MyConsole.Instance.WriteLine("   Overwrite file " + gzFileName);
            }
            DeleteFileForBlazorWebAssembly(Path.ChangeExtension(outputAssemblyFileName, ".pdb"), objOutputDir, wwwRootDir);
            DeleteFileForBlazorWebAssembly(Path.ChangeExtension(outputAssemblyFileName, ".pdb.gz"), objOutputDir, wwwRootDir);
            if ( wwwRootDir != null )
            {
                var gz3 = Path.Combine(wwwRootDir, Path.GetFileName(outputAssemblyFileName) + ".gz");
                if( File.Exists( gz3 ))
                {
                    System.IO.File.WriteAllBytes(gz3, bsWrite);
                    MyConsole.Instance.WriteLine("   Overwrite file " + gz3);
                }
            }
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var bs256 = sha256.ComputeHash(bsNative);
            var strSHA256_Base64 = Convert.ToBase64String(bs256);
            MyConsole.Instance.WriteLine("   New SHA256:" + Convert.ToBase64String(bs256));
            var rootDir2 = System.IO.Path.GetDirectoryName(outputAssemblyFileName);
            var jsonFileName = System.IO.Path.Combine(rootDir2, "blazor.boot.json");
            if( File.Exists( jsonFileName ) == false && wwwRootDir != null )
            {
                jsonFileName = Path.Combine(wwwRootDir, "blazor.boot.json");
            }
            if (File.Exists(jsonFileName))
            {
                var strLines = System.IO.File.ReadAllLines(jsonFileName, System.Text.Encoding.UTF8);
                var mainFileName = System.IO.Path.GetFileName(outputAssemblyFileName);
                bool bolModfied = false;
                var meredFileNames = this.Document._MergedAssemblyNames;
                for (var lineIndex = 0; lineIndex < strLines.Length; lineIndex++)
                {
                    var line = strLines[lineIndex].Trim();

                    if (line.StartsWith("\"" + mainFileName + "\"") && line.IndexOf("sha256-", mainFileName.Length + 2) > 0)
                    {
                        if (line.Trim().EndsWith(","))
                        {
                            strLines[lineIndex] = "\"" + mainFileName + "\":\"sha256-" + strSHA256_Base64 + "\",";
                        }
                        else
                        {
                            strLines[lineIndex] = "\"" + mainFileName + "\":\"sha256-" + strSHA256_Base64 + "\"";
                        }
                        bolModfied = true;
                        break;
                    }
                }
                var deletedLineIndexs = new List<int>();
                var pdbFile = Path.ChangeExtension(outputAssemblyFileName, ".pdb");
                if (DeleteFileForBlazorWebAssembly(pdbFile, objOutputDir, wwwRootDir))
                {
                    DeleteFileForBlazorWebAssembly(pdbFile + ".gz", objOutputDir, wwwRootDir);
                    var sfn4 = "\"" + Path.GetFileName(pdbFile) + "\"";
                    for (var lineIndex = 0; lineIndex < strLines.Length; lineIndex++)
                    {
                        var line = strLines[lineIndex].Trim();
                        if (line.StartsWith(sfn4, StringComparison.OrdinalIgnoreCase))
                        {
                            deletedLineIndexs.Add(lineIndex);
                        }
                    }
                }
                if (meredFileNames != null && meredFileNames.Count > 0)
                {
                    // 合并了其他程序集
                   
                    var lstFileNameNeedDelete = new List<string>();
                    for (var lineIndex = 0; lineIndex < strLines.Length; lineIndex++)
                    {
                        var line = strLines[lineIndex].Trim();
                        // 删除被合并的程序集的信息
                        foreach (var fn2 in meredFileNames)
                        {
                            if (string.Compare(fn2, mainFileName, true) == 0)
                            {
                                continue;
                            }
                            var sfn2 = Path.GetFileNameWithoutExtension(fn2);
                            if (line.StartsWith("\"" + sfn2 + ".", StringComparison.OrdinalIgnoreCase))
                            {
                                deletedLineIndexs.Add(lineIndex);
                                lstFileNameNeedDelete.Add(sfn2 + ".dll");
                                lstFileNameNeedDelete.Add(sfn2 + ".dll.gz");
                                lstFileNameNeedDelete.Add(sfn2 + ".resources.dll");
                                lstFileNameNeedDelete.Add(sfn2 + ".resources.dll.gz");
                                lstFileNameNeedDelete.Add(sfn2 + ".pdb");
                                lstFileNameNeedDelete.Add(sfn2 + ".pdb.gz");
                                break;
                            }
                        }//foreach
                    }//for
                    foreach( var sfn in lstFileNameNeedDelete )
                    {
                        // 删除被合并的程序集文件以及关联文件
                        DeleteFileForBlazorWebAssembly(Path.Combine(rootDir2, sfn), objOutputDir, wwwRootDir);
                    }
                }//if
                if (deletedLineIndexs.Count > 0)
                {
                    bolModfied = true;
                    foreach (var index in deletedLineIndexs)
                    {
                        var line = strLines[index].Trim();
                        strLines[index] = string.Empty;
                        if (line.EndsWith(",") == false && index > 1)
                        {
                            var preLine = strLines[index - 1].TrimEnd();
                            if (preLine.EndsWith(","))
                            {
                                strLines[index - 1] = preLine.Substring(0, preLine.Length - 1);
                            }
                        }
                    }
                }
                if (bolModfied)
                {
                    using (var writer = new System.IO.StreamWriter(jsonFileName, false, System.Text.Encoding.UTF8))
                    {
                        foreach (var line in strLines)
                        {
                            writer.WriteLine(line);
                        }
                    }
                    MyConsole.Instance.WriteLine("   Modifiy " + jsonFileName);
                    if (objOutputDir != null)
                    {
                        var fn9 = Path.Combine(objOutputDir, "blazor.boot.json");
                        if (File.Exists(fn9))
                        {
                            using (var writer = new System.IO.StreamWriter(fn9, false, System.Text.Encoding.UTF8))
                            {
                                foreach (var line in strLines)
                                {
                                    writer.WriteLine(line);
                                }
                            }
                            MyConsole.Instance.WriteLine("   Modifiy " + fn9);
                        }
                    }
                }
            }
            SelfPerformanceCounterForTest.Leave(handle34);
        }

        private void ReplaceFileDeeply( string srcFileName ,string rootDir )
        {
            var fn2 = Path.Combine(rootDir, Path.GetFileName(srcFileName));
            if( File.Exists( fn2 ) && string.Equals(srcFileName , fn2 , StringComparison.OrdinalIgnoreCase) == false )
            {
                File.SetAttributes(fn2, FileAttributes.Normal);
                File.Copy(srcFileName, fn2, true);
                MyConsole.Instance.WriteLine("    Overwrite " + fn2);
            }
            foreach( var dir in Directory.GetDirectories( rootDir ))
            {
                ReplaceFileDeeply(srcFileName, dir);
            }
        }
        private bool DeleteFileForBlazorWebAssembly(string fileName, string objOutputDir, string wwwRootDir)
        {
            if (fileName != null && fileName.Length > 0 )
            {
                if (File.Exists(fileName))
                {
                    File.SetAttributes(fileName, FileAttributes.Normal);
                    File.Delete(fileName);
                    MyConsole.Instance.WriteLine("    Delete file " + fileName);
                }
                if (objOutputDir != null)
                {
                    DCUtils.DeleteFileDeeeply(objOutputDir, Path.GetFileName(fileName));
                }
                if (wwwRootDir != null)
                {
                    DCUtils.DeleteFileDeeeply(wwwRootDir, Path.GetFileName(fileName));
                }
                return true;
            }
            return false;
        }

        internal string _SpecifyOutputMapXmlFileName = null;
        private string _InputAssemblyFileName = null;
        private string _InputAssemblyDirectory = null;
        internal string _UseAnotherExeName = null;
        private int _SourceAssemblyFileSize = 0;

        /// <summary>
        /// 从一个临时目录中加载IL文件
        /// </summary>
        /// <param name="rootPath">临时目录</param>
        /// <returns>操作是否成功</returns>
        public bool LoadILFromTempPath(string rootPath)
        {
            if (rootPath == null || rootPath.Length == 0)
            {
                throw new ArgumentNullException("rootPath");
            }
            if (Directory.Exists(rootPath) == false)
            {
                throw new DirectoryNotFoundException(rootPath);
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
            ConsoleWriteTask();
            MyConsole.Instance.Write(" Loading document in IL format from '" + rootPath + "'.");
            var documents = new List<DCILDocument>();
            foreach (var subDir in Directory.GetDirectories(rootPath))
            {
                var sn = Path.GetFileName(subDir);
                var ilFileName = Path.Combine(subDir, sn + ".dll.il");
                string asmFileName = sn + ".dll";
                if (File.Exists(ilFileName) == false)
                {
                    ilFileName = Path.Combine(subDir, sn + ".exe.il");
                    asmFileName = sn + ".exe";
                }
                if (File.Exists(ilFileName))
                {
                    MyConsole.Instance.Write(Environment.NewLine + "      Loading '" + ilFileName + "' ...");
                    int tick = Environment.TickCount;
                    var doc = new DCILDocument();
                    doc.LoadByReader(ilFileName, this.ContentEncoding);
                    doc.AssemblyFileName = asmFileName;
                    documents.Add(doc);
                    tick = Math.Abs(tick - Environment.TickCount);
                    MyConsole.Instance.Write(" span " + tick + " millisenconds , get " + doc.Classes.Count + " classes.");
                }
            }
            if (documents.Count > 1)
            {
                // 根据程序集之间相互引用关系来获得主程序集对象
                var mainDocs = new List<DCILDocument>(documents);
                for (int iCount = mainDocs.Count - 1; iCount >= 0; iCount--)
                {
                    var mainDoc = mainDocs[iCount];
                    bool beRef = false;
                    foreach (var doc in documents)
                    {
                        if (doc != mainDoc)
                        {
                            foreach (var asm in doc.Assemblies)
                            {
                                if (asm.IsExtern && string.Compare(asm.Name, mainDoc.Name, true) == 0)
                                {
                                    beRef = true;
                                    break;
                                }
                            }
                        }
                        if (beRef)
                        {
                            break;
                        }
                    }
                    if (beRef)
                    {
                        mainDocs.RemoveAt(iCount);
                    }
                }
                if (mainDocs.Count > 0)
                {
                    documents.Remove(mainDocs[0]);
                    documents.Insert(0, mainDocs[0]);
                }
                MyConsole.Instance.WriteLine();
                ConsoleWriteTask();
                MyConsole.Instance.WriteLine("Merging assembly files ...");
                // 进行文档合并
                this.Document = DCILDocument.MergeDocuments(documents);
            }
            else
            {
                this.Document.UpdateCustomAttributeValues();
            }
            return true;
        }
        /// <summary>
        /// 获得文档程序集文件名
        /// </summary>
        /// <returns>文件名</returns>
        public string GetDocumentAssemblyFileName()
        {
            return this.Document?.AssemblyFileName;
        }
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
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                    MyConsole.Instance.BackgroundColor = ConsoleColor.Black;
                    MyConsole.Instance.WriteLine();
#if DOTNETCORE
                    MyConsole.Instance.WriteLine("   Warring!!! This program not support .NET Framework,please use '" + this._UseAnotherExeName + "'.");
#else
                    MyConsole.Instance.WriteLine("   Warring!!! This program not support .NET Core,please use '" + this._UseAnotherExeName + "'.");
#endif
                    MyConsole.Instance.ResetColor();
                    return false;
                }
                if (documents.Count > 1)
                {
                    ConsoleWriteTask();
                    MyConsole.Instance.WriteLine("Merging assembly files ...");
                    // 进行文档合并
                    this.Document = DCILDocument.MergeDocuments(documents);
                }
                else
                {
                    this.Document = documents[0];
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
            MyConsole.Instance.WriteLine("Loading assembly file " + asmFileName);
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
            MyConsole.Instance.Write("    Anlysing IL file...");
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
                    MyConsole.Instance.WriteLine("      Fail to load " + xmlFileName + " . " + ext.Message);
                }
            }
            MyConsole.Instance.WriteLine(" span " + Math.Abs(Environment.TickCount - tick) + " milliseconds. get " + doc.Classes.Count + " classes.");
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
        private void RemoveSpecifyTypes()
        {
            var patterns = StringPattern.CreatePatterns(this.RemoveTypes);
            if(patterns != null && patterns.Length > 0 )
            {
                ConsoleWriteTask();
                MyConsole.Instance.WriteLine("Removing class use " + this.RemoveTypes);
                var clses = this.Document.Classes;
                for(var iCount = clses.Count -1;iCount >=0;iCount --)
                {
                    var cls = clses[iCount];
                    foreach( var item in patterns)
                    {
                        if( item.Match( cls.Name ) && item.IsInclude)
                        {
                            clses.RemoveAt(iCount);
                            MyConsole.Instance.WriteLine(" Remove class " + cls.Name);
                        }
                    }
                }
            }
        }
        public void HandleDocument()
        {
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.HandleDocument);
            this.UpdateRuntimeSwitchs();
            this.RemoveSpecifyTypes();
            this._CallOperCodes = new List<DCILOperCode_HandleMethod>();
            this.SelectUILanguage();
            var allMethods = this.Document.GetAllMethodHasOperCodes();
            this.BuildSizeOfMethod();
            this.AddClassJIEJIEHelper();
            // 删除自定义特性
            this.RemoveCustomAttributes();
            if (this.Switchs.Resources)
            {
                // 加密资源包装类型
                this.ApplyResouceContainerClass();
            }
            // 加密内嵌程序集资源
            this.EncryptEmbeddedResource(allMethods);
            
            if (this.Switchs.ControlFlow)
            {
                // 加密部分字符数值
                this.EncryptCharValue(allMethods);
            }
            // 加密数组定义
            this.Encrypt_ArrayDefine(allMethods);
            // 加密 lock()/using() 结构。
            this.Encrypt_Lock_Using_Structure(allMethods);
            var clses = new List<DCILClass>(this.Document.Classes);
            foreach (var cls in clses)
            {
                HandleClass(cls);
            }
            if (this.Switchs.ControlFlow)
            {
                // 加密typeof()结构
                this.EncryptTypeHandle(allMethods);
                // 加密枚举类型的方法参数数据类型
                this.EncryptMethodParamterEnumValue();
            }
            if (this.Switchs.Strings)
            {
                // 加密字符串
                this.EncryptStringValues(allMethods);
                //this.HandleCollectStringValue();
            }
            this.AddDatasClass();
            this._RFHContainer?.Commit( this );
            this.Document.FixDomState();
            if (this.Switchs.ControlFlow)
            {
                // 加密流程
                this.ObfuscateControlFlow();
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
            if (hasRenamed || this.Document.HasMergeDocuments)
            {
                var attrs = this.Document.GetAllCustomAttributesUseCache();
                foreach (var attr in attrs)
                {
                    attr.UpdateBinaryValueForLocalClassRename();
                }
            }
            if (this.Switchs.ControlFlow)
            {
                this.CollectStatcMethod();
                //this.ObfuscateControlFlow();
                //if (this._CallOperCodes != null && this._CallOperCodes.Count > 0)
                //{
                //    ChangeCallOperCodes(this._CallOperCodes);
                //    this._CallOperCodes.Clear();
                //    this._CallOperCodes = null;
                //}
                if (this._Int32ValueContainer != null)
                {
                    this._Int32ValueContainer.Commit( this );
                    DCUtils.ObfuseListOrder(this._Int32ValueContainer._Class.ChildNodes);
                }
                //if (this._RFHContainer != null)
                //{
                //    ObfuscateMethodOperCodes(this._RFHContainer._Class.Method_Cctor);

                //    //ObfuscateOperCodeList(
                //    //    this._RFHContainer._Class.Method_Cctor,
                //    //    this._RFHContainer._Class.Method_Cctor.OperCodes,
                //    //    false,
                //    //    null);
                //}
            }
            
            if (this.Switchs.RemoveMember)
            {
                RemoveMember();
            }
            if (this._JIEJIEHelper_LoadResourceSet_Used == false)
            {
                var nodes5 = this._Type_JIEJIEHelper?.LocalClass?.ChildNodes;
                if (nodes5 != null)
                {
                    foreach (var item in nodes5)
                    {
                        if (item.Name == "LoadResourceSet")
                        {
                            nodes5.Remove(item);
                            break;
                        }
                    }
                }

            }
            this.InjectPerformanceCounter();
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
            SelfPerformanceCounterForTest.Leave(h4);
        }

        /// <summary>
        /// 计算成员方法的大小（包括指令数量，字符串，引用的字节数组大小）
        /// </summary>
        public bool MeasureSizeOfMethod = false;

        private System.Xml.XmlDocument _SizeOfMethod = null;
        /// <summary>
        /// 计算成员方法的“大小”
        /// </summary>
        private void BuildSizeOfMethod()
        {
            if(this.MeasureSizeOfMethod == false )
            {
                return;
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write("Measure sizeof method...");
            var tick = Environment.TickCount;
            var doc = new System.Xml.XmlDocument();
            doc.AppendChild(doc.CreateElement("SizeOfMethod"));
            doc.DocumentElement.SetAttribute("AssemblyName", this.Document.AssemblyFileName);
            doc.DocumentElement.SetAttribute("Creationtime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            var strTable = new System.Collections.Generic.SortedDictionary<string, List<DCILMethod>>();
            var types = new List<DCILClass>(this.Document.GetAllClassesUseCache().Values);
            types.Sort(delegate (DCILClass c1, DCILClass c2) { return string.Compare(c1.Name, c2.Name); });
            foreach (var type in types)
            {
                if (type.ChildNodes == null || type.ChildNodes.Count == 0)
                {
                    continue;
                }
                var typeElement = doc.CreateElement("Type");
                typeElement.SetAttribute("Name", type.NameWithNested);
                foreach (var item in type.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        var method = (DCILMethod)item;
                        var opcCount = method.TotalOperCodesCount;
                        if (opcCount == 0)
                        {
                            continue;
                        }
                        var methodElement = doc.CreateElement("Method");
                        methodElement.SetAttribute("Name", method.Name);
                        methodElement.SetAttribute("OperCodeCount", opcCount.ToString());
                        var size9 = method.GetByteArraySize(this);
                        if (size9 > 0)
                        {
                            methodElement.SetAttribute("ArraySize", method.GetByteArraySize(this).ToString());
                        }
                        methodElement.SetAttribute("Sign", method.GetSignString());
                        var strs = method.GetStringValues();
                        if (strs != null && strs.Length > 0)
                        {
                            foreach (var str in strs)
                            {
                                var strElement = doc.CreateElement("String");
                                strElement.SetAttribute("Length", str.Length.ToString());
                                strElement.InnerText = str;
                                methodElement.AppendChild(strElement);
                                List<DCILMethod> methods = null;
                                if( strTable.TryGetValue( str , out methods ) == false )
                                {
                                    methods = new List<DCILMethod>();
                                    strTable[str] = methods;
                                }
                                methods.Add(method);
                            }
                        }
                        if (DCILMethod.IsCtorOrCctor(method.Name))
                        {
                            typeElement.PrependChild(methodElement);
                        }
                        else
                        {
                            typeElement.AppendChild(methodElement);
                        }
                    }
                }//foreach
                if (typeElement.FirstChild != null)
                {
                    doc.DocumentElement.AppendChild(typeElement);
                }
            }//foreach
            if( strTable.Count > 0 )
            {
                var tableElements = doc.CreateElement("Strings");
                doc.DocumentElement.AppendChild(tableElements);
                var strList = new List<string>(strTable.Keys);
                strList.Sort(delegate (string s1, string s2) {
                    var result = s1.Length.CompareTo(s2.Length);
                    if( result == 0 )
                    {
                        result = s1.CompareTo(s2);
                    }
                    return result;
                });
                tableElements.SetAttribute("Count", strList.Count.ToString());
                var totalLength = 0;
                foreach( var strItem in strList)
                {
                    totalLength += strItem.Length;
                    var strElement = doc.CreateElement("String");
                    var methods = strTable[strItem];
                    strElement.SetAttribute("Length", strItem.Length.ToString());
                    strElement.SetAttribute("Value", strItem);
                    DCILClass lastClass = null;
                    foreach( var m in methods)
                    {
                        if( lastClass == null || m.OwnerClass != lastClass)
                        {
                            
                        }
                        var me2 = doc.CreateElement("Method");
                        me2.SetAttribute("Name", m.OwnerClass.NameWithNested +"::" +m.Name);
                        //me2.SetAttribute("Sign", m.GetSignatureForMap());
                        strElement.AppendChild(me2);
                    }
                    tableElements.AppendChild(strElement);
                }
                tableElements.SetAttribute("TotalLength", totalLength.ToString());
            }
            if (doc.DocumentElement.FirstChild != null)
            {
                this._SizeOfMethod = doc;
            }
            tick = Environment.TickCount - tick;
            MyConsole.Instance.WriteLine(" span " + tick + " milliseconds.");
        }

        private void InjectPerformanceCounter()
        {
            if(this.AddPerformanceCounter == false )
            {
                return;
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write("Inject performace counter...");
            int tick = Environment.TickCount;
            var methodNames = new List<string>();
            //var _enter = new DCILInvokeMethodInfo(
            //    (DCILMethod)this._Type_JIEJIEPerformanceCounter.LocalClass.GetChildNodeByName("Enter"), true);
            //var _leave = new DCILInvokeMethodInfo(
            //    (DCILMethod)this._Type_JIEJIEPerformanceCounter.LocalClass.GetChildNodeByName("Leave"), true);
            foreach ( var cls in this.Document.GetAllClassesUseCache().Values)
            {
                if(cls == this._Type_JIEJIEHelper.LocalClass 
                    || cls == this._Type_JIEJIEPerformanceCounter.LocalClass)
                {
                    continue;
                }
                if( cls.Parent != null )
                {
                    if( cls.Parent == this._Type_JIEJIEHelper.LocalClass 
                        || cls.Parent == this._Type_JIEJIEPerformanceCounter.LocalClass)
                    {
                        continue;
                    }
                }
                if( cls.IsInterface )
                {
                    continue;
                }
                if( cls.BaseType?.Name == "System.Attribute")
                {
                    continue;
                }
                if( cls.BaseTypeName == "System.Attribute")
                {
                    continue;
                }
                foreach(var item in cls.ChildNodes )
                {
                    if(item is DCILMethod)
                    {
                        var method = (DCILMethod)item;
                        if( method.TotalOperCodesCount <= 2 )
                        {
                            continue;
                        }
                        method.Maxstack++;
                        if(method.Locals == null )
                        {
                            method.Locals = new DCILMethodLocalVariableList();
                        }
                        var locHandle = new DCILMethodLocalVariable();
                        locHandle.Index = method.Locals.Count;
                        locHandle.Name = "__pc2023_Handle";
                        locHandle.ValueType = DCILTypeReference.Type_Int32;
                        method.Locals.Add(locHandle);
                        var codes = method.OperCodes;
                        for( var iCount = codes.Count -1;iCount >=0;iCount --)
                        {
                            if(codes[iCount].OperCode == "ret" || codes[iCount].OperCode == "throw")
                            {
                                var strLableID = codes[iCount].LabelID;
                                codes[iCount].LabelID = "IL_Ret_Throw_" + iCount;
                                codes.Insert(
                                    iCount,
                                    new DCILOperCode(
                                        strLableID,
                                        "ldloc",
                                        (method.Locals.Count-1).ToString()));
                                codes.Insert(
                                    iCount+1, 
                                    new DCILOperCode(
                                        "IL_PCLeave_" + iCount,
                                        "call","void __DC20211119.JIEJIEPerformanceCounter::Leave(int32)"));
                            }
                        }
                        codes.Insert(
                            0,
                            new DCILOperCode("IL_PCEnter3", "stloc",( method.Locals.Count-1).ToString()));
                        codes.Insert(
                            0,
                            new DCILOperCode("IL_PCEnter2", "call" , "int32 __DC20211119.JIEJIEPerformanceCounter::Enter(int32)"));
                        codes.Insert(
                            0, 
                            new DCILOperCode(
                                "IL_PCEnter1", 
                                DCILOperCodeDefine._ldc_i4, 
                                methodNames.Count.ToString()));
                        if (method.OldSignatureForMap != null)
                        {
                            methodNames.Add(method.OldSignatureForMap);
                        }
                        else
                        {
                            methodNames.Add(method.GetSignatureForMap());
                        }
                        codes.ChangeShortInstruction();
                        
                    }//if
                }//foreach
            }//foreach
            if( methodNames.Count > 0 )
            {
                var method = (DCILMethod)this._Type_JIEJIEPerformanceCounter.LocalClass.GetChildNodeByName("PublicStart");
                var codes = method.OperCodes;
                codes[0] = new DCILOperCode("IL_LoadNum", "ldc.i4", methodNames.Count.ToString());
                var newCodes = new DCILOperCodeList();
                for (var iCount = 0; iCount < methodNames.Count; iCount++)
                {
                    newCodes.AddItem("IL_Dup" + iCount, DCILOperCodeDefine._dup);
                    newCodes.AddItem("IL_Index" + iCount, DCILOperCodeDefine._ldc_i4, iCount.ToString());
                    var strName = methodNames[iCount];
                    strName = strName.Replace('"', '#');
                    newCodes.AddItem("IL_Ldstr" + iCount, DCILOperCodeDefine._ldstr, '"' + strName + '"');
                    newCodes.AddItem("IL_Set" + iCount, "stelem.ref");
                }
                codes.InsertRange(2, newCodes);
                this._Type_JIEJIEPerformanceCounter.LocalClass.Modified = true;
                method.ILCodesModified = true;
            }
            MyConsole.Instance.WriteLine(" Handle " + methodNames.Count + " methods , span "
                + Math.Abs(Environment.TickCount - tick) + " milliseconds.");
        }

        /// <summary>
        /// 删除自定义特性
        /// </summary>
        private void RemoveCustomAttributes()
        {
            if( this.RemoveCustomAttributeTypeFullNames == null 
                || this.RemoveCustomAttributeTypeFullNames.Length == 0 )
            {
                return;
            }
            Dictionary<string, int> typeNames = new Dictionary<string, int>();
            var srcItems = this.RemoveCustomAttributeTypeFullNames.Split(',');
            foreach( var item in srcItems)
            {
                var item2 = item.Trim().ToLower() ;
                if(item2.Length > 0 )
                {
                    typeNames[item2] = 0;
                }
            }
            if(typeNames.Count == 0 )
            {
                return;
            }
            var dtm = DateTime.Now;
            int removeCount = 0;
            foreach (var asm in this.Document.Assemblies)
            {
                removeCount += RemoveCustomAttributeList(asm.CustomAttributes, typeNames);
            }
            foreach( var cls in this.Document.GetAllClassesUseCache().Values)
            {
                removeCount += RemoveCustomAttributeList(cls.CustomAttributes, typeNames);
                foreach( var item in cls.ChildNodes)
                {
                    removeCount += RemoveCustomAttributeList(item.CustomAttributes, typeNames);
                }
            }
            var tick = (int)((DateTime.Now - dtm).TotalMilliseconds);
            ConsoleWriteTask();
            MyConsole.Instance.WriteLine("Remove " +  removeCount +" custom attributes , span " + tick + "millisenconds.");
            foreach(var item in typeNames)
            {
                foreach( var item2 in srcItems)
                {
                    if(string.Equals( item.Key , item2 , StringComparison.OrdinalIgnoreCase))
                    {
                        MyConsole.Instance.WriteLine("    " + item2 + " , removed " + item.Value + " times.");
                        break;
                    }
                }
            }
        }

        private static int RemoveCustomAttributeList(List<DCILCustomAttribute> attrs, Dictionary<string, int> typeNames)
        {
            int result = 0;
            if (attrs != null && attrs.Count > 0)
            {
                for (var iCount = attrs.Count - 1; iCount >= 0; iCount--)
                {
                    var item = attrs[iCount];
                    if (item.AttributeTypeName != null
                        && typeNames.ContainsKey(item.AttributeTypeName.ToLower()))
                    {
                        attrs.RemoveAt(iCount);
                        typeNames[item.AttributeTypeName.ToLower()]++;
                        result++;
                    }
                }
            }
            return result;
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
                        if (eit2 != null)
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
                        MyConsole.Instance.WriteLine("            Move static methods : " + oldClsName + "::" + oldMethodName);
                    }
                }//for
                DCUtils.ObfuseListOrder(newClass.ChildNodes);
                this.Document.Classes.Add(newClass);
                ConsoleWriteTask();
                MyConsole.Instance.WriteLine("Move " + methods.Count + " static methods.");
            }
        }

        private void ChangeCallOperCodes(List<DCILOperCode_HandleMethod> callCodes)
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Packaging member property...");
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
                //if( item.Key.Name == "get_Document"
                //    && item.Key.Parent.Name == "DCSoft.Writer.Controls.WriterControl")
                //{

                //}
                if (item.Value > 5)
                {
                    var method = item.Key;
                    if (method.IsFinal == false)
                    {
                        if (method.IsVirtual || method.IsNewslot || method.IsAbstract)
                        {
                            continue;
                        }
                    }
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
            MyConsole.Instance.WriteLine(" create " + newMethodCount + " methods , change " + changeCodeCount + " call/callvirt instructions. span " + tick + " milliseconds.");
            MyConsole.Instance.WriteLine();
        }

        private void RemoveMember()
        {
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.RemoveMember);
            ConsoleWriteTask();
            MyConsole.Instance.Write("Removing Type members ...");
            SortedDictionary<string, System.Tuple<int, int>> counters = null;
            if (this.DebugMode)
            {
                counters = new SortedDictionary<string, System.Tuple<int, int>>();
            }
            int tick = Environment.TickCount;
            int removeCount = 0;
            foreach (var cls in this.Document.Classes)
            {
                if (cls.OldName == "DCSoft.ShapeEditor.Controls.ShapeEditorControl")
                {

                }
                var localCount = 0;
                var localPCount = 0;
                if (cls.RuntimeSwitchs == null)
                {
                    cls.RuntimeSwitchs = this.Switchs;
                }
                if (cls.RuntimeSwitchs.RemoveMember)
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
                        MyConsole.Instance.WriteLine("     " + item.Key + " \t remove " + item.Value.Item1 + " const fields.");
                    }
                    else
                    {
                        MyConsole.Instance.WriteLine("     " + item.Key + " \t remove " + item.Value.Item1 + " const fields, " + item.Value.Item2 + " properties.");
                    }
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
            MyConsole.Instance.WriteLine(" remove " + removeCount + " members , span " + tick + " milliseconds.");
            SelfPerformanceCounterForTest.Leave(h4);
        }
        /// <summary>
        /// 要删除的自定义属性类型全名
        /// </summary>
        public string RemoveCustomAttributeTypeFullNames = null;
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
            //this._UILanguageName = null;
            var allResFiles = this.Document.Resouces;
            if (allResFiles.Count > 0)
            {
                var culs = this.Document.GetSupportCultures();
                if (culs != null && culs.Length > 0)
                {
                    ConsoleWriteTask();
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                    MyConsole.Instance.WriteLine("Please select UI language you want:");
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                    for (int iCount = 0; iCount < culs.Length; iCount++)
                    {
                        MyConsole.Instance.WriteLine("    " + iCount + ":" + culs[iCount].Name + " " + culs[iCount].DisplayName);
                    }
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                    MyConsole.Instance.Write("Please input index,press enter to use default,");
                    MyConsole.Instance.ResetColor();
                    if( this._UILanguageName != null && this._UILanguageName.Length > 0 )
                    {
                        bool find = false;
                        foreach(var cul in culs )
                        {
                            if( cul.Name == this._UILanguageName )
                            {
                                find = true;
                                this._UILanguageDisplayName = cul.DisplayName;
                                break;
                            }
                        }
                        if(find == false )
                        {
                            this._UILanguageDisplayName = null;
                            this._UILanguageName = null;
                        }
                    }
                    if (MyConsole.Instance.SupportKeyboardInput == false 
                        || MyConsole.Instance.IsNativeConsole == false 
                        || string.IsNullOrEmpty(this._UILanguageName) == false)
                    {
                        MyConsole.Instance.WriteLine();
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                        if (this._UILanguageName == null)
                        {
                            MyConsole.Instance.WriteLine("OK,you already select default language.");
                            MyConsole.Instance.ResetColor();
                        }
                        else
                        {
                            MyConsole.Instance.WriteLine("OK,you already select [" + this._UILanguageName + "-" + this._UILanguageDisplayName + "].");
                            MyConsole.Instance.ResetColor();
                            foreach (var item in this.Document.Resouces.Values)
                            {
                                item.ChangeLanguage(this._UILanguageName);
                            }
                        }
                        return;
                    }
                    try
                    {
                        DCUtils.EatAllConsoleKey();
                        int countDown = 15;
                        int left = MyConsole.Instance.CursorLeft;
                        int top = MyConsole.Instance.CursorTop;
                        while (true)
                        {
                            if (MyConsole.Instance.KeyAvailable)
                            {
                                var key = MyConsole.Instance.ReadKey();
                                int index = key.KeyChar - '0';
                                if (index >= 0 && index < culs.Length)
                                {
                                    MyConsole.Instance.WriteLine();
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
                                //MyConsole.Instance.CursorLeft = left;
                                //MyConsole.Instance.CursorTop = top;
                                countDown--;
                                if (countDown < 0)
                                {
                                    break;
                                }
                                MyConsole.Instance.Write("(" + countDown.ToString() + ")");
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                        MyConsole.Instance.WriteLine();
                    }
                    catch( System.InvalidOperationException ext)
                    {
                        // 在某些情况下，不支持键盘输入，则使用默认值
                        MyConsole.Instance.IsNativeConsole = false;
                        MyConsole.Instance.WriteLine("Error:" + ext.Message);
                        MyConsole.Instance.WriteLine("OK,you already select default language.");
                        MyConsole.Instance.ResetColor();
                    }
                }
            }
        }
        public void ConsoleWriteTask()
        {
            MyConsole.Instance.EnsureNewLine();
            MyConsole.Instance.BackgroundColor = ConsoleColor.Green;
            MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
            MyConsole.Instance.Write("[");
            MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
            MyConsole.Instance.BackgroundColor = ConsoleColor.Black;
            MyConsole.Instance.Write("Task");
            MyConsole.Instance.BackgroundColor = ConsoleColor.Green;
            MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
            MyConsole.Instance.Write("]");
            MyConsole.Instance.ResetColor();
            MyConsole.Instance.Write(" ");
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
        /// <returns></returns>
        private int RenameByOverrideList(List<DCILClass> allClasses, IDGenerator idGen )
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
                if (cls.Name == "DCSoft.ShapeEditor.Controls.ShapeEditorControl")
                {

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
                if (cls.RuntimeMissNativeBaseTypeOrInterface())
                {
                    // 这个类型存在未找到的原生态的基础类型，则不重命名重载的成员方法。
                    foreach (var item in cls.ChildNodes)
                    {
                        if (item is DCILMethod)
                        {
                            var method = (DCILMethod)item;
                            if (method.RenameState == DCILRenameState.NotHandled)
                            {
                                if (method.IsInstance && method.IsVirtual)
                                {
                                    // 处理疑似实现外部接口的方法
                                    method.RenameState = DCILRenameState.Preserve;
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
                foreach( var item in methods)
                {
                    //if( item.Name == "get_Handle")
                    //    //&& item.OwnerClass?.Name == "DCSoft.ShapeEditor.Controls.ShapeEditorControl")
                    //{

                    //}
                }
                if( this.DetectDeadCode == DetectDeadCodeMode.Normal 
                    || this.DetectDeadCode == DetectDeadCodeMode.All)
                {
                    foreach( var item2 in methods )
                    {
                        if(item2.Used )
                        {
                            // 如果重载列表中只要有一个被调用过，则所有的同组方法都是被调用了
                            foreach( var item3 in methods )
                            {
                                item3.Used = true;
                            }
                            break;
                        }
                    }
                }
                //foreach( var item2 in methods)
                //{
                //    item2.OwnerOverriedMethods = methods;
                //}
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
            resultMap.ClearData();
            methodGrounds.Clear();
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
                try
                {
                    resultList.Add(new DCILMethod(m, this.Document, gps));
                }
                catch( System.Exception ext )
                {

                }
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

        private Dictionary<DCILClass, List<DCILMethod>> _Cache_BaseMethods
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
                    else
                    {
                        rootClass.MissNativeBaseTypeOrInterface = true;
                    }
                }
                else
                {
                    var localClass = baseType.LocalClass;
                    if (localClass == null)
                    {
                        localClass = this.Document.GetClassByName(baseType.Name);
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
            resultList.Sort( delegate(DCILMethod m1, DCILMethod m2) { return string.Compare(m1.Name, m2.Name); });
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
            public void ClearData()
            {
                foreach (var item in this)
                {
                    item.Value.Clear();
                }
                base.Clear();
            }
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
                if(baseTypeMethodMap != null && baseTypeMethodMap.Count > 0 )
                {
                    foreach( var list2 in baseTypeMethodMap.Values )
                    {
                        if(list2.Count > 1 )
                        {
                            foreach( var m in list2 )
                            {
                                this.AddInfos(list2, m);
                            }
                        }
                    }
                }
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
                    refMethods.AddRange(baseMethods);
                }
                else
                {
                    foreach( var item in baseMethods)
                    {
                        if( refMethods.Contains(item )== false )
                        {
                            refMethods.Add(item);
                        }
                    }
                }
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
        internal class RefMethodList : List<DCILMethod>
        {
            public bool Used = false;
            /// <summary>
            /// 删除重复的项目
            /// </summary>
            public void RemoveSameItem()
            {
                DCUtils.RemoveSameItem<DCILMethod>(this);
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
        //private void CheckSomeState()
        //{
        //    var nodes = GetClassByNameOrOldName("DCSoft.ShapeEditor.Controls.ShapeEditorControl")?.ChildNodes;
        //    bool hasProperty = false;
        //    if (nodes != null )
        //    {
        //        foreach( var item in nodes )
        //        {
        //            if(item is DCILProperty)
        //            {
        //                hasProperty = true;
        //                break;
        //            }
        //        }
        //    }
        //    if (hasProperty == false)
        //    {
        //        Console.WriteLine("ssssssssssss");
        //    }
            
        //}
        //private DCILClass GetClassByNameOrOldName( string name )
        //{
        //    foreach(var cls in GetAllClasses())
        //    {
        //        if(cls.Name == name || cls.OldName == name )
        //        {
        //            return cls;
        //        }
        //    }
        //    return null;
        //}
        public int RenameClasses()
        {
            IDGenerator.GenCountBase = 0;
            ConsoleWriteTask();
            MyConsole.Instance.Write("Renaming.....");
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.Rename);
            var specifyRenameTypes = StringPattern.CreatePatterns(this.SpeicfyRename);
            int curPos = 0;
            if (MyConsole.Instance.IsNativeConsole)
            {
                curPos = MyConsole.Instance.CursorLeft * MyConsole.Instance.CursorTop;
            }

            int tick = Environment.TickCount;
            var idGenForMember = new IDGenerator(this.PrefixForTypeRename, this.PrefixForMemberRename);
            idGenForMember.DebugMode = this.DebugMode;
            if (this.DetectDeadCode == DetectDeadCodeMode.Normal 
                || this.DetectDeadCode == DetectDeadCodeMode.All)
            {
                // 设置基本的方法被调用状态
                foreach (var item in this.Document._CachedInvokeMethods.Keys)
                {
                    if (item.LocalMethod != null)
                    {
                        item.LocalMethod.Used = true;
                    }
                }
            }

            var allClasses = GetAllClasses();
            allClasses.Sort(delegate (DCILClass cls1, DCILClass cls2) 
            { return string.Compare(cls1.Name, cls2.Name); });
            if (this.ForBlazorWebAssembly)
            {
                // 处理 Microsoft.JSInterop.JSInvokableAttribute 特性
                foreach (var cls in allClasses)
                {
                    // 删除二进制序列化标记
                    cls.RemoveStyle("serializable");
                    foreach (var item in cls.ChildNodes)
                    {
                        if (item is DCILMethod)
                        {
                            var method = (DCILMethod)item;
                            if(method.EntryPoint)
                            {
                                // 对于入口点所在的类型，所有成员都不重命名。
                                cls.RenameState = DCILRenameState.Preserve;
                                foreach( var item2 in cls.ChildNodes)
                                {
                                    if(item2 is DCILMemberInfo)
                                    {
                                        ((DCILMemberInfo)item2).RenameState = DCILRenameState.Preserve;
                                    }
                                }
                                break;
                            }
                            var hasJSInvokable = false;
                            var attrs = method.CustomAttributes;
                            if (attrs != null && attrs.Count > 0)
                            {
                                foreach (var attr in attrs)
                                {
                                    if (attr.AttributeTypeName == "Microsoft.JSInterop.JSInvokableAttribute")
                                    {
                                        // 方法附加了 JSInvokableAttribute 特性，不改名
                                        hasJSInvokable = true;
                                        break;
                                    }
                                }
                            }
                            if (hasJSInvokable)
                            {
                                method.RenameState = DCILRenameState.Preserve;
                                cls.RenameState = DCILRenameState.Preserve;
                                if (method.Parent is DCILProperty)
                                {
                                    ((DCILProperty)method.Parent).RenameState = DCILRenameState.Preserve;
                                }
                            }
                        }
                    }
                }
            }

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
                if (this.Document.PrerviceTypeNames != null 
                    && this.Document.PrerviceTypeNames.Contains( cls.Name ))
                {
                    // 收到保留的类型名称
                    cls.RenameState = DCILRenameState.Preserve;
                    foreach( var item in cls.ChildNodes )
                    {
                        if( item is DCILMemberInfo )
                        {
                            ((DCILMemberInfo)item).RenameState = DCILRenameState.Preserve;
                        }
                    }
                    continue;
                }
                if(specifyRenameTypes != null )
                {
                    foreach( var item in specifyRenameTypes )
                    {
                        if(item.Match( cls.Name ))
                        {
                            if (item.IsInclude)
                            {
                                cls.RenameState = DCILRenameState.NeedRename;
                                cls.ObfuscationSettings = null;
                            }
                            else
                            {
                                cls.RenameState = DCILRenameState.Preserve;
                            }
                            break;
                        }
                    }
                }
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        var m = (DCILMemberInfo)item;
                        if (item is DCILField && cls.InnerGenerate)
                        {
                            // 内部产生的类型，不保留旧名称
                            continue;
                        }
                        m.OldSignatureForMap = m.GetSignatureForMap();
                    }
                }//foreach
            }//foreach
            // 根据函数重载关系来重命名。
            int result = RenameByOverrideList(allClasses, idGenForMember);

            IDGenerator.GenCountBase = idGenForMember.GenCount + 1;
            var countBaseGenMember = idGenForMember.GenCount + 1;
            // 执行函数的重命名
            this._IDGenForClass = new IDGenerator(this.PrefixForTypeRename, this.PrefixForMemberRename);
            this._IDGenForClass.DebugMode = this.DebugMode;
            int genCountStart = _Random.Next(10, 100);
            int nestedClassCounter = 0;
            foreach (var cls in allClasses)
            {
                if (cls.IsImport)
                {
                    // 外界导入的COM接口，则不改名
                    continue;
                }
                if( cls.Name == CodeTemplate._ClassName_JIEJIEPerformanceCounter)
                {
                    cls.RenameState = DCILRenameState.Preserve;
                    continue;
                }
                if (this.Document.PrerviceTypeNames != null
                    && this.Document.PrerviceTypeNames.Contains(cls.Name))
                {
                    // 收到保留的类型名称
                    cls.RenameState = DCILRenameState.Preserve;
                    continue;
                }
                //if( cls.Name == "DCSoft.WASM.Prograss")
                //{

                //}
                bool ignoreClass = false;
                if( cls.ImplementsInterfaces != null && cls.ImplementsInterfaces.Count > 0 )
                {
                    foreach( var item in cls.ImplementsInterfaces )
                    {
                        if(item.Name == "System.Runtime.CompilerServices.IAsyncStateMachine")
                        {
                            // 针对 await 语句自动生成的类型，则例外处理。
                            ignoreClass = true;
                            break;
                        }
                    }
                }
                if(ignoreClass )
                {
                    // 遇到忽略的类型
                    continue;
                }
                var needRename = false;
                var clsOs = cls.ObfuscationSettings;
                if (cls.RenameState == DCILRenameState.NotHandled)
                {
                    //cls.RemoveObfuscationAttribute();
                    idGenForMember.GenCount = countBaseGenMember;
                    if (clsOs == null || clsOs.Exclude == false)
                    {
                        needRename = true;
                    }
                    else
                    {
                        cls.RenameState = DCILRenameState.Preserve;
                    }
                }
                else if( cls.RenameState == DCILRenameState.NeedRename)
                {
                    needRename = true;
                }
                if(needRename )
                {
                    cls.OldName = cls.Name;
                    if (cls.Parent is DCILClass)
                    {
                        nestedClassCounter++;
                        idGenForMember.GenCount = countBaseGenMember + nestedClassCounter;
                        // 内嵌类型
                        var newName = idGenForMember.GenerateIDForMember(cls.Name, cls);
                        cls.ChangeName(newName);
                    }
                    else
                    {
                        var newName = this._IDGenForClass.GenerateIDForClass(cls.OldName, cls);
                        //if (cls.Parent is DCILClass)
                        //{
                        //    // 内嵌的类型，则去除命名空间
                        //    int index4 = newName.LastIndexOf('.');
                        //    if (index4 > 0)
                        //    {
                        //        newName = newName.Substring(index4 + 1);
                        //    }
                        //}
                        cls.ChangeName(newName);
                    }
                    result++;
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
            eba.AttributeTypeName = typeof(System.ComponentModel.EditorBrowsableAttribute).FullName;// "System.ComponentModel.EditorBrowsableAttribute";
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
            foreach (var cls in allClasses)
            {
                var childNeedRemove = new List<DCILObject>();
                if (cls.InnerGenerate == false)
                {
                    totalCount_cls++;
                    if (cls.RenameState == DCILRenameState.Renamed)
                    {
                        handleCount_cls++;
                    }
                }
                cls.RemoveObfuscationAttribute();
                bool memberAddEBA = true;
                if( this.ForBlazorWebAssembly )
                {
                    memberAddEBA = false;
                }
                else if ( cls.AddEditorBrowsableAttributeForRename(eba))
                {
                    memberAddEBA = false;
                }
                else if (cls.HasStyle("public") == false)
                {
                    memberAddEBA = false;
                }
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        var itemMember = (DCILMemberInfo)item;
                        itemMember.RemoveObfuscationAttribute();
                        if (memberAddEBA && (itemMember.HasStyle("family") || itemMember.HasStyle("public")))
                        {
                            itemMember.AddEditorBrowsableAttributeForRename(eba);
                        }
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

                                if (this.Switchs.RemoveMember)
                                {
                                    if (method.IsSpecialname)
                                    {
                                        method.RemoveStyle("specialname");
                                        method.IsSpecialname = false;
                                    }
                                    if (method.ParentMember != null && childNeedRemove.Contains( method.ParentMember ) == false )
                                    {
                                        childNeedRemove.Add(method.ParentMember);
                                    }
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
                        if (item is DCILProperty)
                        {
                            var p = (DCILProperty)item;
                            if (p.Method_Get?.LocalMethod != null)
                            {
                                p.Method_Get.LocalMethod.IsSpecialname = false;
                            }
                            if (p.Method_Set?.LocalMethod != null)
                            {
                                p.Method_Set.LocalMethod.IsSpecialname = false;
                            }
                        }
                        cls.ChildNodes.Remove(item);
                    }
                    childNeedRemove.Clear();
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
            if (MyConsole.Instance.IsNativeConsole)
            {
                if (MyConsole.Instance.CursorLeft * MyConsole.Instance.CursorTop != curPos)
                {
                    MyConsole.Instance.WriteLine();
                }
            }
            MyConsole.Instance.WriteLine(" span " + tick + " milliseconds.");
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
                MyConsole.Instance.WriteLine(strResult.ToString());
            }
            if( this.DetectDeadCode == DetectDeadCodeMode.Normal || this.DetectDeadCode == DetectDeadCodeMode.All )
            {
                RunDetectDeadCodeTask(allClasses);
            }
            SelfPerformanceCounterForTest.Leave(h4);
            return result;
        }
        /// <summary>
        /// 执行检测死亡代码的任务
        /// </summary>
        /// <param name="allClasses"></param>
        private void RunDetectDeadCodeTask(List<DCILClass> allClasses)
        {
            ConsoleWriteTask();
            MyConsole.Instance.WriteLine("Detect death code...");
            var clsTable = new Dictionary<DCILClass, DCILClass>();
            var methodTable = new Dictionary<DCILMethod, DCILMethod>();
            foreach (var item in allClasses)
            {
                clsTable[item] = item;
            }
            foreach (var item in this.Document._CachedTypes.Values)
            {
                if (item.LocalClass != null && clsTable.ContainsKey(item.LocalClass))
                {
                    clsTable.Remove(item.LocalClass);
                }
            }
            var deadClass = new List<DCILClass>();
            if (clsTable.Count > 0)
            {
                // 剩下的就是从未被使用过的类型
                foreach (var cls in clsTable.Keys)
                {
                    if (cls.Used)
                    {
                        // 类型标记为被使用了。
                        continue;
                    }
                    if (this.DetectDeadCode == DetectDeadCodeMode.Normal)
                    {
                        if (cls.RenameState == DCILRenameState.Renamed
                            && cls.HasCustomAttributes == false)
                        {
                            // 已经被重名了，而且没有附加任何特性，则认为是死亡代码
                            deadClass.Add(cls);
                        }
                    }
                    else if ( this.DetectDeadCode == DetectDeadCodeMode.All )
                    {
                        if( cls.RenameState == DCILRenameState.Renamed )
                        {
                            deadClass.Add(cls);
                        }
                    }
                }
                if (deadClass.Count > 0)
                {
                    deadClass.Sort(delegate (DCILClass cls1, DCILClass cls2) { return string.Compare(cls1.OldName, cls2.OldName); });
                    if (this.DetectDeadCode == DetectDeadCodeMode.Normal)
                    {
                        MyConsole.Instance.WriteLine("The following types never used and renamed and has no custom attributes.");
                    }
                    else
                    {
                        MyConsole.Instance.WriteLine("The following types never used and renamed.");
                    }
                    MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                    foreach (var cls in deadClass)
                    {
                        // 存在 const 类型字段，则不一定是死亡代码
                        bool hasConstField = false;
                        foreach (var member in cls.ChildNodes)
                        {
                            if (member is DCILField)
                            {
                                var field = (DCILField)member;
                                if (field.IsConst)
                                {
                                    hasConstField = true;
                                    break;
                                }
                            }
                        }
                        if (hasConstField)
                        {
                            MyConsole.Instance.WriteLine("     " + cls.OldName + " [Maby for const field.]");
                        }
                        else
                        {
                            MyConsole.Instance.WriteLine("     " + cls.OldName);
                        }
                    }
                    MyConsole.Instance.ResetColor();
                }
            }
            // 检测死亡的方法
            var deadMethods = new HashSet<DCILMethod>();

            foreach (var cls in allClasses)
            {
                if (deadClass.Contains(cls))
                {
                    // 死亡的类型就不检测了
                    continue;
                }
                if (cls.OldName == CodeTemplate._ClassName_JIEJIEHelper)
                {
                    continue;
                }
                foreach (var member in cls.ChildNodes)
                {
                    if (member is DCILMethod)
                    {
                        var method = (DCILMethod)member;
                        if (method == cls.Method_Cctor
                            || method.RenameState != DCILRenameState.Renamed)
                        {
                            // 静态构造函数|未重命名|有自定义特性的方法不处理
                            continue;
                        }
                        if (this.DetectDeadCode == DetectDeadCodeMode.Normal && member.HasCustomAttributes )
                        {
                            continue;
                        }
                        if (method.Used)
                        {
                            continue;
                        }
                        if( method._Override != null )
                        {
                            continue;
                        }
                        deadMethods.Add(method);
                    }
                }//foreach
            }//foreach
            if (deadMethods.Count > 0)
            {
                var remdeadTypes = StringPattern.CreatePatterns(this.RemoveDeadCodeTypes);
                var lstMethods = new List<DCILMethod>(deadMethods);
                lstMethods.Sort(delegate (DCILMethod m1, DCILMethod m2)
                {
                    return string.Compare(m1.OldSignatureForMap, m2.OldSignatureForMap, true);
                });
                if (this.DetectDeadCode == DetectDeadCodeMode.Normal)
                {
                    MyConsole.Instance.WriteLine("The " + lstMethods.Count + " following methods never used and renamed and has no custom attributes.");
                }
                else
                {
                    MyConsole.Instance.WriteLine("The " + lstMethods.Count + " following methods never used and renamed.");
                }
                MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                string lastClassName = null;
                foreach (var item in lstMethods)
                {
                    var clsName = item.OwnerClass.OldName == null ? item.OwnerClass.Name : item.OwnerClass.OldName;
                    if (item.OwnerClass.Parent is DCILClass)
                    {
                        var c2 = (DCILClass)item.OwnerClass.Parent;
                        if (c2.OldName == null)
                        {
                            clsName = c2.Name + "." + clsName;
                        }
                        else
                        {
                            clsName = c2.OldName + "." + clsName;
                        }
                    }
                    if (lastClassName == null || lastClassName != clsName)
                    {
                        lastClassName = clsName;
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
                        MyConsole.Instance.WriteLine("  " + clsName);
                    }
                    var strCodeCount = item.TotalOperCodesCount.ToString();
                    if (strCodeCount.Length < 5)
                    {
                        strCodeCount = strCodeCount + new string(' ', 5 - strCodeCount.Length);
                    }
                    bool handed = false;
                    if (remdeadTypes != null)
                    {
                        foreach (var item2 in remdeadTypes)
                        {
                            if (item2.Match(clsName))
                            {
                                handed = true;
                                MyConsole.Instance.ForegroundColor = ConsoleColor.Blue;
                                item.OwnerClass.ChildNodes.Remove(item);
                                MyConsole.Instance.WriteLine("    " + strCodeCount + item.OldSignatureForMap.Replace(clsName + '.', "") + " and removed.");
                                break;
                            }
                        }
                    }
                    if (handed == false)
                    {
                        MyConsole.Instance.ForegroundColor = ConsoleColor.Green;
                        MyConsole.Instance.WriteLine("    " + strCodeCount + item.OldSignatureForMap.Replace(clsName + '.', ""));
                    }
                }
                MyConsole.Instance.ResetColor();
            }
            else if (deadClass.Count == 0)
            {
                MyConsole.Instance.WriteLine("      [Detect nothing.]");
            }
        }
            


        internal class NumberNameGen : IDisposable
        {
            public NumberNameGen(string prefix, int num = 0)
            {
                this._Prefix = prefix;
                if (num > 0)
                {
                    this._Values = new List<string>(num);
                }
            }
            public string GetName(int index)
            {
                if (index >= this._Values.Count)
                {
                    for (int iCount = this._Values.Count; iCount <= index; iCount++)
                    {
                        if (this._Prefix == null || this._Prefix.Length == 0)
                        {
                            this._Values.Add(iCount.ToString());
                        }
                        else
                        {
                            this._Values.Add(this._Prefix + iCount.ToString());
                        }
                    }
                }
                return this._Values[index];
            }
            private string _Prefix = null;
            private List<string> _Values = new List<string>();

            public void Dispose()
            {
                if (this._Values != null)
                {
                    this._Values.Clear();
                    this._Values = null;
                }
            }
        }

        private NumberNameGen _NewPNameGen = new NumberNameGen("p");
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
                    var newPName = _NewPNameGen.GetName(pCount++);// "p" + Convert.ToString(pCount++);
                    maps[p.Name] = newPName;
                    p.Name = newPName;
                }
                var codes = method.GetAllOperCodes<DCILOperCode>();
                if (codes != null && codes.Count > 0)
                {
                    foreach (var code in codes)
                    {
                        var operCodeValue = code.OperCodeValue;
                        if (operCodeValue == DCILOpCodeValue.Ldarg_S
                            || operCodeValue == DCILOpCodeValue.Ldarga_S
                            || operCodeValue == DCILOpCodeValue.Starg_S)
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

        private void HandleMethod(DCILMethod method)
        {
            var opts = method.RuntimeSwitchs;
            if (opts == null)
            {
                opts = this.Switchs;
            }
            if (opts.AllocationCallStack
                && this.ForBlazorWebAssembly == false // 在Blazor WebAssembly 模式下，该功能无效。
                && method.ReturnTypeInfo == DCILTypeReference.Type_String
                && ((DCILClass)method.Parent ).InnerGenerate == false )
            {
                var targetMethod =(DCILMethod) this._Type_JIEJIEHelper.LocalClass.GetChildNodeByName("CloneStringCrossThead");
                // 加密关键字符串对象创建调用堆栈
                for (int ilIndex = method.OperCodes.Count - 1; ilIndex >= 0; ilIndex--)
                {
                    var ilcode = method.OperCodes[ilIndex];
                    if (ilcode.OperCodeValue == DCILOpCodeValue.Ret)
                    {
                        method.ILCodesModified = true;
                        method.OperCodes.Insert(
                            ilIndex,
                            new DCILOperCode_HandleMethod(method.GenNewLabelID(), "call", targetMethod));
                            //new DCILOperCode(
                            //    "IL_zzzzz",
                            //    "call",
                            //    "string " + _ClassName_JIEJIEHelper + "::CloneStringCrossThead(string)"));
                        _ModifiedCount++;
                        break;
                    }
                }
            }
            //if (opts.ControlFlow && method.OperCodeSpecifyStructure == false)
            //{
            //    if (method.OperCodes != null && method.OperCodes.Count > 0)
            //    {
            //        ObfuscateOperCodeList(method);
            //    }
            //}
        }

        private void HandleClass(DCILClass cls)
        {
            if (cls.IsImport)
            {
                //COM导入的类型不进行任何处理
                return;
            }
            var opts = cls.RuntimeSwitchs;
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
                    HandleMethod((DCILMethod)item);
                }
                else if (item is DCILClass)
                {
                    HandleClass((DCILClass)item);
                }
            }
            if (cls.NestedClasses != null && cls.NestedClasses.Count > 0)
            {
                foreach (var cls2 in cls.NestedClasses)
                {
                    HandleClass(cls2);
                }
            }
        }

        private static readonly Dictionary<int, string> _IndexedName = new Dictionary<int, string>();
        public static string GetIndexName(int index)
        {
            string v = null;
            if (_IndexedName.TryGetValue(index, out v) == false)
            {
                v = "_" + index;
                _IndexedName[index] = v;
            }
            return v;
        }

        private ByteArrayDataContainer _ByteDataContainer = new ByteArrayDataContainer();


        private class ByteArrayDataContainer : IDisposable
        {
            public string FullClassName = DCJieJieNetEngine._ClassNamePrefix + "ByteArrayDataContainer";

            private List<int> _FieldIndexs = new List<int>();
            //public string GetFieldName(byte[] data)
            //{
            //    var index = IndexOf(data);
            //    if (_FieldIndexs.Contains(index) == false)
            //    {
            //        _FieldIndexs.Add(index);
            //    }
            //    return FullClassName + "::_" + index;
            //}
            public DCILOperCode_HandleMethod GetOperCode(string labelID, byte[] data)
            {
                int index = IndexOf(data);
                var code = new DCILOperCode_HandleMethod();
                code.LabelID = labelID;
                code.SetOperCode("call");
                code.InvokeInfo = new DCILInvokeMethodInfo();
                code.InvokeInfo.IsInstance = false;
                code.InvokeInfo.ReturnType = DCILTypeReference.Type_Byte.Clone();
                code.InvokeInfo.ReturnType.ArrayAndPointerSettings = "[]";
                code.InvokeInfo.OwnerType = new DCILTypeReference(FullClassName, DCILTypeMode.Class);
                code.InvokeInfo.MethodName = GetIndexName(index);// "_" + index;
                _MyCodes.Add(code);
                return code;
            }
            private List<DCILOperCode_HandleMethod> _MyCodes = new List<DCILOperCode_HandleMethod>();
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
            public void Dispose()
            {
                if (this._Datas != null)
                {
                    this._Datas.Clear();
                    this._Datas = null;
                }
                if (this.LocalClass != null)
                {
                    this.LocalClass.Dispose();
                    this.LocalClass = null;
                }
                this.FullClassName = null;
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
                    var tempMethod = new DCILMethod();
                    //var lableGen = new ILLabelIDGen();
                    foreach (var index in _FieldIndexs)
                    {
                        var field = eng.Int32ValueContainer.GetField(_Datas[index].Length);
                        if (field == null)
                        {
                            str.AppendLine(tempMethod.GenNewLabelID() + ": ldc.i4 " + _Datas[index].Length);
                        }
                        else
                        {
                            str.AppendLine(tempMethod.GenNewLabelID() + ": ldsfld int32 " + field.Parent.Name + "::" + field.Name);
                        }
                        str.AppendLine(tempMethod.GenNewLabelID() + ": newarr [" + LibName_mscorlib + "]System.Byte");
                        str.AppendLine(tempMethod.GenNewLabelID() + ": dup");
                        str.AppendLine(tempMethod.GenNewLabelID() + ": ldtoken field valuetype " + FullClassName + @"/_DATA" + index + " " + FullClassName + @"::_Field" + index);
                        //str.AppendLine(tempMethod.GenNewLabelID() + ": ldc.i4.0");
                        //str.AppendLine(tempMethod.GenNewLabelID() + ": call void " + _ClassName_JIEJIEHelper + @"::MyInitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle,int32)");
                        str.AppendLine(tempMethod.GenNewLabelID() + ": call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)");
                        str.AppendLine(tempMethod.GenNewLabelID() + ": stsfld uint8[] " + FullClassName + "::_" + index);
                    }
                    str.AppendLine(tempMethod.GenNewLabelID() + ": ret");
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
	//IL_0009: ldc.i4 0
    //IL_000d: call void " + CodeTemplate._ClassName_JIEJIEHelper + "::MyInitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle, int32)
	IL_000d: call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)
	//IL_0012: stloc.0
	//IL_0013: br.s IL_0015
	//IL_0015: ldloc.0
	IL_0016: ret
}
");
                }
                str.AppendLine("}");
                for (int iCount = 0; iCount < _Datas.Count; iCount++)
                {
                    var item = new DCILData();
                    item._Name = "I_BDC" + iCount;
                    item.DataType = "bytearray";
                    item.Value = _Datas[iCount];
                    document.ILDatas.Add(item);
                    //datas[item.Name] = item;
                }
                var cls = new DCILClass(str.ToString(), document);
                document.Classes.Add(cls);
                document.ClearCacheForAllClasses();
                //var datas = new Dictionary<string, DCILData>();
                this.LocalClass = cls;
                cls.RuntimeSwitchs = eng.Switchs;
                var clses = document.GetAllClassesUseCache();
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMemberInfo)
                    {
                        ((DCILMemberInfo)item).CacheInfo(document, clses);
                    }
                }
                if (_MyCodes.Count > 0)
                {
                    var t2 = new DCILTypeReference(cls);
                    foreach (var code in this._MyCodes)
                    {
                        code.InvokeInfo.LocalMethod = (DCILMethod)cls.GetChildNodeByName(code.InvokeInfo.MethodName);
                        code.InvokeInfo.OwnerType = t2;
                    }
                }
                return cls;
            }
        }
        private string _BitmapTypeName = null;
        /// <summary>
        /// 获得位图对象类型名称
        /// </summary>
        /// <returns></returns>
        private string GetBitmapTypeName()
        {
            if (_BitmapTypeName == null)
            {
                foreach (var cls in this.Document.Classes)
                {
                    if (cls.Name == "DCSystem_Drawing.Bitmap")
                    {
                        // 这是一个神秘的判断，嘿嘿。
                        _BitmapTypeName = cls.Name;
                        break;
                    }
                }
                if (_BitmapTypeName == null)
                {
                    _BitmapTypeName = this.Document.GetTypeNameWithLibraryName(
                       "System.Drawing.Bitmap",
                       typeof(System.Drawing.Bitmap).Assembly.GetName().Name);
                }
            }
            return _BitmapTypeName;
        }
        public void ApplyResouceContainerClass()
        {
            ConsoleWriteTask();
            MyConsole.Instance.WriteLine("Encrypting resources containers ...");

            int tick = Environment.TickCount;
            var calledMethods = new HashSet<DCILMethod>();
            // 获得系统中所有被调用过的 get_ 方法
            foreach( var method in this.Document.GetAllMethodHasOperCodes())
            {
                method.EnumOperCodes(delegate (EnumOperCodeArgs args) { 
                    if( args.Current is DCILOperCode_HandleMethod)
                    {
                        var m2 = ((DCILOperCode_HandleMethod)args.Current).InvokeInfo?.LocalMethod;
                        if(m2 != null
                            && m2.Name != null 
                            && m2.Name.StartsWith("get_", StringComparison.Ordinal))
                        {
                            calledMethods.Add(m2);
                        }
                    }
                });
            }
            var cls_resIndex = new Dictionary<DCILClass, int>();
            var allRes = this.Document.GetNodeIndexs<DCILMResource>();
            var allResNames = new List<string>();
            var bmpTypeName = this.GetBitmapTypeName();
            
            foreach (var cls in this.Document.Classes)
            {
                if (cls.IsResoucePackage() == false)
                {
                    continue;
                }
                var resName = cls.Name + DCILMResource.EXT_Resources;// DCILDocument.EXT_resources;
                DCILMResource res = null;
                if( this.Document.Resouces.ContainsKey(resName ))
                {

                }
                var runtimeResourceName = resName;
                if (this.Document.Resouces.TryGetValue(resName, out res) == false )
                {
                    // 没有直接命中资源名称，则按照子名称进行查找
                    var fieldCount = 0;
                    var flag_ResourceManager = false;
                    var flag_CultureInfo = false;
                    foreach ( var item in cls.ChildNodes)
                    {
                        if(item is DCILField)
                        {
                            var field = (DCILField)item;
                            if( field.ValueType.Name == "System.Resources.ResourceManager")
                            {
                                flag_ResourceManager = true;
                            }
                            else if( field.ValueType.Name == "System.Globalization.CultureInfo")
                            {
                                flag_CultureInfo = true;
                            }
                            fieldCount++;
                            if( fieldCount > 2 )
                            {
                                break;
                            }
                        }
                        else if( item is DCILMethod )
                        {
                            if(item.Name.StartsWith("get_") == false
                                && item.Name.StartsWith("set_") == false
                                && DCILMethod.IsCtorOrCctor( item.Name ) == false )
                            {
                                // 资源容器类型只能有属性，不可能有独立的方法。
                                flag_ResourceManager = false;
                                break;
                            }
                        }
                        else if(item is DCILEvent)
                        {
                            //  资源容器类型不可能有事件
                            flag_ResourceManager = false;
                            break;
                        }
                    }
                    if(fieldCount == 2 && flag_ResourceManager && flag_CultureInfo)
                    {
                        var nameItems = cls.Name.Split('.');
                        foreach( var item in this.Document.Resouces)
                        {
                            if (item.Key.EndsWith(DCILMResource.EXT_Resources))
                            {
                                var keyItems = item.Key.Split('.');
                                int currentIndex = nameItems.Length - 1;
                                for(int iCount = keyItems.Length - 2;iCount >= 0;iCount --)
                                {
                                    if( keyItems[iCount] == nameItems[currentIndex])
                                    {
                                        currentIndex--;
                                        if( currentIndex == 0 )
                                        {
                                            break;
                                        }
                                    }
                                }
                                if( currentIndex == 0)
                                {
                                    res = item.Value;
                                    runtimeResourceName = item.Key;
                                    break;
                                }
                            }
                        }
                    }
                }
                if ( res != null )
                {
                    if (res.ResourceValues == null || res.ResourceValues.Count == 0)
                    {
                        continue;
                    }
                    var resValues = new List<DCILMResource.MResourceItem>(res.ResourceValues.Values);
                    if (this.Switchs.Rename && this.Switchs.RemoveMember)
                    {
                        int removeItemCount = 0;
                        for (var iCount = resValues.Count - 1; iCount >= 0; iCount--)
                        {
                            //if( resValues[iCount].Name == "CADisabledTip")
                            //{

                            //}
                            var method = cls.GetChildNodeByName("get_" + resValues[iCount].Name) as DCILMethod;
                            if (method != null)
                            {
                                if (calledMethods.Contains(method) == false)
                                {
                                    removeItemCount++;
                                    resValues.RemoveAt(iCount);
                                }
                            }
                        }//for
                        if (removeItemCount > 0)
                        {
                            MyConsole.Instance.WriteLine("   Remove " + removeItemCount + " items from resources " + cls.Name);
                        }
                    }
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
	IL_0022: call class " + bmpTypeName + @" " + CodeTemplate._ClassName_JIEJIEHelper + @"::GetBitmap(uint8[], int32, int32, int32)
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
	IL_0015: call string " + CodeTemplate._ClassName_JIEJIEHelper + @"::GetString(uint8[], int32, int32, int32)
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
                    this.UpdateRuntimeSwitchs_Class(newCls, this.Switchs);
                    newCls.InnerGenerate = false;
                    cls.CustomAttributes = null;
                    cls.ChildNodes = newCls.ChildNodes;
                    cls.ObfuscationSettings = null;
                    foreach (DCILObject item in cls.ChildNodes)
                    {
                        item.Parent = cls;
                    }
                    this._ModifiedCount++;
                    this.Document.Resouces.Remove(runtimeResourceName);
                    var fn = Path.Combine(this.Document.RootPath, resName);
                    //if (File.Exists(fn))
                    //{
                    //    File.Delete(fn);
                    //}
                }
            }//for
        }

        private bool _JIEJIEHelper_LoadResourceSet_Used = false;

        private static readonly string LibNameForComponentResourceManager 
            = typeof(System.ComponentModel.ComponentResourceManager).Assembly.GetName().Name;
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
                                    && codes[iCount + 1].OperCodeValue == DCILOpCodeValue.Call
                                    && codes[iCount + 2].OperCodeValue == DCILOpCodeValue.Newobj)
                                {
                                    var code2 = codes[iCount + 2] as DCILOperCode_HandleMethod;
                                    if (code2.InvokeInfo.MethodName == ".ctor"
                                        && (code2.InvokeInfo.OwnerType.Name == "System.ComponentModel.ComponentResourceManager"
                                        || code2.InvokeInfo.OwnerType.Name == "System.Resources.ResourceManager"))
                                    {
                                        var bsWrite = GetBytesForWrite(res.Data);// GetGZipCompressedContentIfNeed(bs);
                                        this._JIEJIEHelper_LoadResourceSet_Used = true;
                                        string clsName = _ClassNamePrefix + "Res" + AllocIndex();
                                        string strNewClassCode = FixTypeLibNameForNetCore(CodeTemplate._Code_Template_ComponentResourceManager);
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
                                        this.UpdateRuntimeSwitchs_Class(resCls, this.Switchs);
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
                                            if (code.OperCodeValue == DCILOpCodeValue.Callvirt)
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
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.AddDatasClass);
            if (this._ByteDataContainer != null && this._ByteDataContainer.HasData())
            {
                var cls = this._ByteDataContainer.WriteTo(this.Document, this);
                if (cls != null)
                {
                    UpdateRuntimeSwitchs_Class(cls, this.Switchs);
                    HandleClass(cls);
                }
            }
            SelfPerformanceCounterForTest.Leave(h4);
        }

        private void AddClassJIEJIEHelper()
        {
            var code = FixTypeLibNameForNetCore(CodeTemplate._Code_Template_JIEJIEHelper.Replace("[System.Drawing]System.Drawing.Bitmap", this.GetBitmapTypeName()));
            var cls = new DCILClass(code, this.Document);
            this.Document.Classes.Add(cls);
            this.Document.ClearCacheForAllClasses();
            if( this.Switchs.AllocationCallStack || this.ForBlazorWebAssembly)
            {
                // 删除加密字符串调用堆栈的代码
                for( var iCount = cls.ChildNodes.Count -1;iCount >= 0;iCount --)
                {
                    var name = cls.ChildNodes[iCount].Name;
                    if(name.Contains("CloneStringCrossThead")
                        || name.Contains("Monitor_Enter") 
                        || name.Contains("Monitor_Exit"))
                    {
                        cls.ChildNodes.RemoveAt(iCount);
                    }
                    
                }
            }
            this._Type_JIEJIEHelper = new DCILTypeReference(CodeTemplate._ClassName_JIEJIEHelper, DCILTypeMode.Class);
            this._Type_JIEJIEHelper.LocalClass = cls;
            UpdateRuntimeSwitchs_Class(cls, this.Switchs);

            if(this.AddPerformanceCounter)
            {
                code = FixTypeLibNameForNetCore(CodeTemplate._Code_Template_JIEJIEPerformanceCounter);
                cls = new DCILClass(code, this.Document);
                this.Document.Classes.Add(cls);
                this.Document.ClearCacheForAllClasses();
                this._Type_JIEJIEPerformanceCounter = new DCILTypeReference(CodeTemplate._ClassName_JIEJIEPerformanceCounter, DCILTypeMode.Class);
                this._Type_JIEJIEPerformanceCounter.LocalClass = cls;
                UpdateRuntimeSwitchs_Class(cls, this.Switchs);
            }
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
                _NetCore_Type_LibName[typeof(System.Resources.Extensions.DeserializingResourceReader).FullName] = typeof(System.Resources.Extensions.DeserializingResourceReader).Assembly.GetName().Name;
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
                foreach( var cls in this.Document.GetAllClassesUseCache().Values )
                {
                    if( _NetCore_Type_LibName.ContainsKey( cls.Name ))
                    {
                        _NetCore_Type_LibName[cls.Name] = string.Empty;
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

                                        if (newLibName.Length > 0)
                                        {
                                            strResult.Append("[" + newLibName + "]");
                                        }
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

        private DCILTypeReference _Type_JIEJIEHelper = null;

        private DCILTypeReference _Type_JIEJIEPerformanceCounter = null;

        //******************************************************************************************
        //******************************************************************************************
        //******************************************************************************************

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
            if (cls.IsValueType || cls.IsEnum )
            {
                // 对于结构体和枚举类型，不混淆成员次序
                return;
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
        private static readonly string _SwitchPrefix = "JIEJIE.NET.SWITCH:";
         
        private void UpdateRuntimeSwitchs()
        {
            if (this.Switchs == null)
            {
                this.Switchs = new JieJieSwitchs();
            }
            foreach (var cls in this.Document.Classes)
            {
                UpdateRuntimeSwitchs_Class(cls, this.Switchs);
            }
        }

        private void UpdateRuntimeSwitchs_Class(DCILClass cls, JieJieSwitchs parent)
        {
            JieJieSwitchs result = null;
            string strFeature = cls.ObfuscationSettings?.Feature;
            if (strFeature != null
                && strFeature.StartsWith(_SwitchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string strSettings = strFeature.Substring(_SwitchPrefix.Length);
                result = new JieJieSwitchs(strSettings, parent , cls );
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
                                    result = new JieJieSwitchs(strSettings, parent , cls );
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
            cls.RuntimeSwitchs = result;
            if (cls.NestedClasses != null && cls.NestedClasses.Count > 0)
            {
                foreach (var cls2 in cls.NestedClasses)
                {
                    UpdateRuntimeSwitchs_Class(cls2, cls.RuntimeSwitchs);
                }
            }
            if (cls.ChildNodes != null && cls.ChildNodes.Count > 0)
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        UpdateRuntimeSwitchs_Method((DCILMethod)item, cls.RuntimeSwitchs);
                    }
                }
            }
        }

        private void UpdateRuntimeSwitchs_Method(DCILMethod method, JieJieSwitchs parent)
        {
            JieJieSwitchs result = null;
            var strFeature = method.ObfuscationSettings?.Feature;
            if (strFeature != null
                && strFeature.StartsWith(_SwitchPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string strSettings = strFeature.Substring(_SwitchPrefix.Length);
                result = new JieJieSwitchs(strSettings, parent , method.Parent);
            }
            else if (method.OperCodes != null && method.OperCodes.Count > 0)
            {
                int len = method.OperCodes.Count;// Math.Min(1000, method.OperCodes.Count);
                for (int iCount = 0; iCount < len; iCount++)
                {
                    if (method.OperCodes[iCount] is DCILOperCode_LoadString)
                    {
                        var code = ((DCILOperCode_LoadString)method.OperCodes[iCount]);
                        var strCode = code.Value;
                        if (strCode != null && strCode.StartsWith(_SwitchPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            //code.OperCode = "\"\"";
                            code.ReplaceCode = new DCILOperCode(
                                null,
                                "ldsfld",
                                "string [" + this.Document.LibName_mscorlib + "]System.String::Empty");
                            code.Value = string.Empty;
                            string strSettings = strCode.Substring(_SwitchPrefix.Length);
                            result = new JieJieSwitchs(strSettings, parent , method.Parent);
                            break;
                        }
                    }
                }
            }
            if (result == null)
            {
                result = parent;
            }
            method.RuntimeSwitchs = result;
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

        private int Encrypt_ArrayDefine_Items(DCILMethod method, DCILOperCodeList items)
        {
            int result = 0;
            int itemsCount = items.Count;
            for (int codeIndex = 0; codeIndex < itemsCount; codeIndex++)
            {
                if (items[codeIndex] is DCILOperCode_Try_Catch_Finally)
                {
                    var tcf = (DCILOperCode_Try_Catch_Finally)items[codeIndex];
                    if (tcf._Try != null && tcf._Try.OperCodes != null)
                    {
                        result += Encrypt_ArrayDefine_Items(method, tcf._Try.OperCodes);
                    }
                    if (tcf._Catchs != null)
                    {
                        foreach (var item2 in tcf._Catchs)
                        {
                            if (item2.OperCodes != null)
                            {
                                result += Encrypt_ArrayDefine_Items(method, item2.OperCodes);
                            }
                        }
                    }
                    if (tcf._Finally != null && tcf._Finally.OperCodes != null)
                    {
                        result += Encrypt_ArrayDefine_Items(method, tcf._Finally.OperCodes);
                    }
                    if (tcf._fault != null && tcf._fault.OperCodes != null)
                    {
                        result += Encrypt_ArrayDefine_Items(method, tcf._fault.OperCodes);
                    }
                    continue;
                }
                var callCode = items[codeIndex] as DCILOperCode_HandleMethod;
                if (callCode != null
                    && callCode.MatchTypeAndMethod(
                        "System.Runtime.CompilerServices.RuntimeHelpers",
                        "InitializeArray",
                        2))
                {
                    method.MaxstackFix = 1;
                    var ldTokenCode = items[codeIndex - 1] as DCILOperCode_LdToken;
                    if (ldTokenCode != null && ldTokenCode.FieldReference.LocalField != null)
                    {
                        var fieldIndex = this.RFHContainer.GetFieldIndex(ldTokenCode.FieldReference.LocalField);
                        if (fieldIndex >= 0)
                        {
                            items[codeIndex - 1] = this.Int32ValueContainer.GetOperCode(ldTokenCode.LabelID, fieldIndex); ;
                            items.Insert(
                                codeIndex,
                                new DCILOperCode_HandleMethod(
                                    method.GenNewLabelID(),
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
                            if (code10.OperCodeValue == DCILOpCodeValue.Newarr)
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
                            else if ((code10.OperCodeValue == DCILOpCodeValue.Ldc_I4
                                || code10.OperCodeValue == DCILOpCodeValue.Ldc_I4_S))
                            {
                                var intValue = DCUtils.ConvertToInt32(code10.OperData);
                                this.Int32ValueContainer.ChangeOperCode(items, index2, intValue);
                                break;
                            }
                        }
                    }//for
                    items.Insert(codeIndex, this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), encKey));
                    callCode.ChangeTarget(this._Type_JIEJIEHelper, "MyInitializeArray");
                    codeIndex++;
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// 加密数组定义
        /// </summary>
        /// <param name="allMethods"></param>
        private void Encrypt_ArrayDefine(List<DCILMethod> allMethods)
        {
            if (this.Switchs.ControlFlow == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("MyInitializeArray");
                return;
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting array defines ...");
            int startTick = Environment.TickCount;
            int totalCount = 0;
            foreach (var method in allMethods)
            {
                if (method.RuntimeSwitchs.ControlFlow && method.OperCodes != null)
                {
                    totalCount += Encrypt_ArrayDefine_Items(method, method.OperCodes);
                }
            }
            if (totalCount == 0)
            {
                MyConsole.Instance.WriteLine(" do noting.");
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("MyInitializeArray");
            }
            else
            {
                MyConsole.Instance.WriteLine(" change " + totalCount
                    + " array defines,span " 
                    + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
        }
        /// <summary>
        /// 加密 lock/using 语法结构。
        /// </summary>
        /// <param name="allMethods"></param>
        private void Encrypt_Lock_Using_Structure(List<DCILMethod> allMethods)
        {
            if (this.Switchs.ControlFlow == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("Monitor_Enter");
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("Monitor_Enter2");
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("MyDispose");
                return;
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting lock()/using() structure...");
            int startTick = Environment.TickCount;
            bool hasOneP = false;
            bool hasTwoP = false;
            bool hasMyDispose = false;
            int totalCount = 0;
            foreach (var method in allMethods)
            {
                if (method.RuntimeSwitchs.ControlFlow == false)
                {
                    continue;
                }
                //if(method.Name == "RemoveTaskByTaskID")
                //{

                //}
                method.EnumOperCodes(delegate (EnumOperCodeArgs args)
                {
                    if (args.Current is DCILOperCode_HandleMethod)
                    {
                        var callCode = (DCILOperCode_HandleMethod)args.Current;
                        var targetMethod = callCode.InvokeInfo;
                        if (callCode.OperCodeValue == DCILOpCodeValue.Callvirt
                            && callCode.MatchTypeAndMethod("System.IDisposable", "Dispose", 0))
                        {
                            var preCode = args.OwnerList.SafeGet(args.CurrentCodeIndex - 1);
                            if (preCode != null && preCode.IsPrefixOperCode())
                            {
                                return;
                            }
                            callCode.ChangeTarget(this._Type_JIEJIEHelper, "MyDispose");
                            callCode.InvokeInfo.IsInstance = false;
                            callCode.SetOperCode("call");
                            hasMyDispose = true;
                            totalCount++;
                        }
                        else if (callCode.OperCodeValue == DCILOpCodeValue.Call
                            && targetMethod.OwnerType.Name == "System.Threading.Monitor"
                            && targetMethod.MethodName == "Enter")
                        {
                            if (targetMethod.ParametersCount == 1)
                            {
                                hasOneP = true;
                                callCode.ChangeTarget(this._Type_JIEJIEHelper, "Monitor_Enter");
                                totalCount++;
                            }
                            else if (targetMethod.ParametersCount == 2)
                            {
                                hasTwoP = true;
                                callCode.ChangeTarget(this._Type_JIEJIEHelper, "Monitor_Enter2");
                                totalCount++;
                            }
                        }
                    }
                });
            }
            if (hasOneP == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("Monitor_Enter");
            }
            if (hasTwoP == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("Monitor_Enter2");
            }
            if (hasMyDispose == false)
            {
                this._Type_JIEJIEHelper.LocalClass.ChildNodes.RemoveByName("MyDispose");
            }
            if (totalCount == 0)
            {
                MyConsole.Instance.WriteLine(" do nothings.");
            }
            else
            {
                MyConsole.Instance.WriteLine(" change " + totalCount
                    + " call/callvirt instructions.span " 
                    + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
        }
        /// <summary>
        /// 执行加密字符串的类型或者方法名称的选择器
        /// </summary>
        public string StringsSelector = null;
        

        /// <summary>
        /// 加密字符串操作
        /// </summary>
        /// <param name="allMethods"></param>
        internal void EncryptStringValues(List<DCILMethod> allMethods)
        {
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.EncryptStringValues);
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting strings ...");
            var emptyILCode = new DCILOperCode(
                null,
                "ldsfld",
                "string [" + this.Document.LibName_mscorlib + "]System.String::Empty");
            var codeFields = new Dictionary<string, DCILOperCode_HandleField>();
            var codeMethods = new Dictionary<string, DCILOperCode_HandleMethod>();
            int totalCount = 0;
            int startTick = Environment.TickCount;

            var listTemp = new List<DCILMethod>(allMethods);
            foreach (var item in this._Type_JIEJIEHelper.LocalClass.ChildNodes)
            {
                if (item is DCILMethod)
                {
                    listTemp.Add((DCILMethod)item);
                }
            }
            allMethods = listTemp;
            StringPattern[] patters = null;
            if (this.StringsSelector != null && this.StringsSelector.Length > 0)
            {
                // 指明要处理的方法名称
                patters = StringPattern.CreatePatterns(this.StringsSelector);
            }
            foreach (var method in allMethods)
            {
                if (method.RuntimeSwitchs.Strings == false)
                {
                    continue;
                }
                if (patters != null)
                {
                    var bolInclude = true;
                    foreach (var item in patters)
                    {
                        if (item.Match(method.OwnerClass?.Name) || item.Match(method.Name))
                        {
                            bolInclude = item.IsInclude;
                            break;
                        }
                    }
                    if (bolInclude == false)
                    {
                        // 不处理这个方法
                        continue;
                    }
                }
                method.OperCodes.EnumDeeply(method, delegate (EnumOperCodeArgs args)
                {
                    if (args.Current is DCILOperCode_LoadString)
                    {
                        totalCount++;
                        var ldCode = (DCILOperCode_LoadString)args.Current;
                        if (ldCode.Value.Length == 0)
                        {
                            ldCode.ReplaceCode = emptyILCode;
                        }
                        else if (method.RuntimeSwitchs.HightStrings)
                        {
                            // 高强度加密
                            DCILOperCode_HandleMethod codeMethod = null;
                            if (codeMethods.TryGetValue(ldCode.Value, out codeMethod) == false)
                            {
                                var method2 = new DCILMethod();
                                method2.AddStyles("public", "static");
                                method2.IsInstance = false;
                                method2._Name = null;
                                method2.RuntimeSwitchs = this.Switchs;
                                codeMethod = new DCILOperCode_HandleMethod(null, "call", method2);
                                codeMethods[ldCode.Value] = codeMethod;
                            }
                            ldCode.ReplaceCode = codeMethod;
                            ldCode.Value = null;
                            ldCode.OperData = null;
                        }
                        else
                        {
                            // 普通加密字符串
                            DCILOperCode_HandleField codeField = null;
                            if (codeFields.TryGetValue(ldCode.Value, out codeField) == false)
                            {
                                var field = new DCILField();
                                field.AddStyles("public", "static", "initonly");
                                field.ValueType = DCILTypeReference.Type_String;
                                field._Name = null;
                                codeField = new DCILOperCode_HandleField(null, "ldsfld", new DCILFieldReference(field));
                                codeFields[ldCode.Value] = codeField;
                            }
                            ldCode.ReplaceCode = codeField;
                            ldCode.Value = null;
                            ldCode.OperData = null;
                        }
                    }
                });
            }
            if (totalCount == 0)
            {
                MyConsole.Instance.WriteLine("Do nothings.");
                return;
            }
            else if (codeFields.Count == 0 && codeMethods.Count == 0)
            {
                MyConsole.Instance.WriteLine(" Handle " + totalCount + " empty string values.");
                return;
            }
            if (codeMethods.Count > 0)
            {
                // 高强度加密字符串
                var methods = new List<DCILMethod>();
                var strValues = new Dictionary<DCILMethod, string>();
                foreach (var item in codeMethods)
                {
                    methods.Add(item.Value.LocalMethod);
                    strValues[item.Value.LocalMethod] = item.Key;
                }
                DCUtils.ObfuseListOrder(methods);
                int classCount = 0;
                var rnd = new System.Random();
                //string[] fieldNames = null;
                while (methods.Count > 0)
                {
                    var clsName = _ClassNamePrefix + "Strings" + Convert.ToString(classCount++);
                    var keyOffset = rnd.Next(10000, 99999);
                    var strNewClassILCode = @"
.class private auto ansi abstract sealed beforefieldinit MyClass extends [mscorlib]System.Object
{
.method private hidebysig specialname rtspecialname static void .cctor () cil managed 
{
	.maxstack 3
	.locals init (
		[0] uint8[] datas
	)
}// end of method

.field private static initonly uint8[] _Data 

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
	IL_000a: ldc.i4 " + keyOffset + @"
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
}
}// end of class";
                    strNewClassILCode = strNewClassILCode.Replace("[mscorlib]", "[" + this.Document.LibName_mscorlib + "]");
                    var newClass = new DCILClass(strNewClassILCode, this.Document);
                    newClass._Name = _ClassNamePrefix + "HightStrings" + Convert.ToString(methods.Count);
                    var methodDecrypt = this.Document.CacheDCILInvokeMethodInfo(
                        new DCILInvokeMethodInfo((DCILMethod)newClass.GetChildNodeByName("dcsoft")));
                    this.Document.Classes.Add(newClass);
                    this.Document.ClearCacheForAllClasses();
                    this.UpdateRuntimeSwitchs_Class(newClass, this.Switchs);

                    //if (fieldNames == null)
                    //{
                    //    fieldNames = new string[110];
                    //    for (int iCount = 0; iCount < fieldNames.Length; iCount++)
                    //    {
                    //        fieldNames[iCount] = GetIndexName(iCount);// "_" + iCount.ToString();
                    //    }
                    //}
                    int itemNum = Math.Min(rnd.Next(50, 100), methods.Count);
                    var lstDatas = new List<byte>();
                    //var lbGen = new ILLabelIDGen();
                    var fieldRef_Data = new DCILFieldReference((DCILField)newClass.GetChildNodeByName("_Data"));
                    for (int iCount = 0; iCount < itemNum; iCount++)
                    {
                        var method = methods[iCount];
                        method._Name = GetIndexName(iCount);// fieldNames[iCount];// "_" + iCount.ToString();
                        method.Parent = newClass;
                        method.ReturnTypeInfo = DCILTypeReference.Type_String;
                        method.Maxstack = 4;
                        method.IsInstance = false;
                        newClass.ChildNodes.Add(method);
                        long longKey = EncryptStringValues_AddString(lstDatas, strValues[method], keyOffset);
                        //var strValue = strValues[method];
                        //var bsContent = System.Text.Encoding.UTF8.GetBytes(strValue);
                        //var itemEncryptKey = rnd.Next(10000, ushort.MaxValue - 10000);
                        //long longKey = lstDatas.Count / 2;
                        //longKey = (longKey << 24) + strValue.Length;
                        //longKey = (longKey << 16) + (ushort)(itemEncryptKey ^ keyOffset);
                        //var key = itemEncryptKey;
                        //foreach (var c in strValue)
                        //{
                        //    ushort v3 = (ushort)(c ^ key);
                        //    lstDatas.Add((byte)(v3 >> 8));
                        //    lstDatas.Add((byte)(v3 & 0xff));
                        //    key++;
                        //}
                        method.Maxstack = 3;
                        method.OperCodes = new DCILOperCodeList();
                        method.OperCodes.Add(new DCILOperCode_HandleField(method.GenNewLabelID(), "ldsfld", fieldRef_Data));
                        method.OperCodes.AddItem(method.GenNewLabelID(), "ldc.i8", longKey.ToString());
                        method.OperCodes.Add(new DCILOperCode_HandleMethod(method.GenNewLabelID(), "call", methodDecrypt));
                        method.OperCodes.AddItem(method.GenNewLabelID(), "ret");
                    }
                    var methodCctor = newClass.Method_Cctor;
                    methodCctor.OperCodes = new DCILOperCodeList();
                    methodCctor.OperCodes.AddItem(methodCctor.GenNewLabelID(), "nop");
                    methodCctor.OperCodes.Add(this._ByteDataContainer.GetOperCode(methodCctor.GenNewLabelID(), lstDatas.ToArray()));
                    methodCctor.OperCodes.Add(new DCILOperCode_HandleField(methodCctor.GenNewLabelID(), "stsfld", fieldRef_Data));
                    methodCctor.OperCodes.AddItem(methodCctor.GenNewLabelID(), "ret");
                    methods.RemoveRange(0, itemNum);
                }//while
                codeMethods.Clear();
                MyConsole.Instance.WriteLine(" Handle " + totalCount + " ldstr instructions, "
                    + strValues.Count + " height string values , span "
                    + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
            if (codeFields.Count > 0)
            {
                // 普通加密字符串
                var fields = new List<DCILField>();
                var strValues = new Dictionary<DCILField, string>();
                foreach (var item in codeFields)
                {
                    fields.Add(item.Value.Value.LocalField);
                    strValues[item.Value.Value.LocalField] = item.Key;
                }
                DCUtils.ObfuseListOrder(fields);
                int classCount = 0;
                var rnd = new System.Random();
                //string[] fieldNames = null;
                while (fields.Count > 0)
                {
                    var clsName = _ClassNamePrefix + "Strings" + Convert.ToString(classCount++);
                    var keyOffset = rnd.Next(10000, 99999);
                    var strNewClassILCode = @"
.class private auto ansi abstract sealed beforefieldinit MyClass extends [mscorlib]System.Object
{
.method private hidebysig specialname rtspecialname static void .cctor () cil managed 
{
	.maxstack 3
	.locals init (
		[0] uint8[] datas
	)
}// end of method

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
	IL_000a: ldc.i4 " + keyOffset + @"
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
}
}// end of class";
                    strNewClassILCode = strNewClassILCode.Replace("[mscorlib]", "[" + this.Document.LibName_mscorlib + "]");
                    var newClass = new DCILClass(strNewClassILCode, this.Document);
                    newClass._Name = _ClassNamePrefix + "Strings" + Convert.ToString(fields.Count);
                    var operCodes = new DCILOperCodeList();
                    newClass.Method_Cctor.OperCodes = operCodes;
                    var methodCctor = newClass.Method_Cctor;
                    var methodDecrypt = this.Document.CacheDCILInvokeMethodInfo(
                        new DCILInvokeMethodInfo((DCILMethod)newClass.GetChildNodeByName("dcsoft")));

                    this.Document.Classes.Add(newClass);
                    this.Document.ClearCacheForAllClasses();
                    this.UpdateRuntimeSwitchs_Class(newClass, this.Switchs);

                    //if (fieldNames == null)
                    //{
                    //    fieldNames = new string[110];
                    //    for (int iCount = 0; iCount < fieldNames.Length; iCount++)
                    //    {
                    //        fieldNames[iCount] = GetIndexName(iCount);// "_" + iCount.ToString();
                    //    }
                    //}
                    int itemNum = Math.Min(rnd.Next(50, 100), fields.Count);
                    var lstDatas = new List<byte>();
                    //var lbGen = new ILLabelIDGen();
                    for (int iCount = 0; iCount < itemNum; iCount++)
                    {
                        var field = fields[iCount];
                        field._Name = GetIndexName(iCount);// fieldNames[iCount];// "_" + iCount.ToString();
                        field.Parent = newClass;
                        newClass.ChildNodes.Add(field);
                        var longKey = EncryptStringValues_AddString(lstDatas, strValues[field], keyOffset);
                        //var strValue = strValues[field];
                        //var bsContent = System.Text.Encoding.UTF8.GetBytes(strValue);
                        //var itemEncryptKey = rnd.Next(10000, ushort.MaxValue - 10000);
                        //long longKey = lstDatas.Count / 2;
                        //longKey = (longKey << 24) + strValue.Length;
                        //longKey = (longKey << 16) + (ushort)(itemEncryptKey ^ keyOffset);
                        //var key = itemEncryptKey;
                        //foreach (var c in strValue)
                        //{
                        //    ushort v3 = (ushort)(c ^ key);
                        //    lstDatas.Add((byte)(v3 >> 8));
                        //    lstDatas.Add((byte)(v3 & 0xff));
                        //    key++;
                        //}
                        operCodes.AddItem(methodCctor.GenNewLabelID(), "ldloc.0");
                        operCodes.AddItem(methodCctor.GenNewLabelID(), "ldc.i8", longKey.ToString());
                        operCodes.Add(new DCILOperCode_HandleMethod(methodCctor.GenNewLabelID(), "call", methodDecrypt));
                        operCodes.Add(new DCILOperCode_HandleField(methodCctor.GenNewLabelID(), "stsfld", new DCILFieldReference(field)));
                    }
                    operCodes.Insert(0, new DCILOperCode(methodCctor.GenNewLabelID(), "nop"));
                    operCodes.Insert(1, this._ByteDataContainer.GetOperCode(methodCctor.GenNewLabelID(), lstDatas.ToArray()));
                    operCodes.Insert(2, new DCILOperCode(methodCctor.GenNewLabelID(), "stloc.0"));
                    operCodes.AddItem(methodCctor.GenNewLabelID(), "ret");
                    fields.RemoveRange(0, itemNum);
                    if (this.Switchs.ControlFlow)
                    {
                        ObfuscateMethodOperCodes(methodCctor);
                    }
                }//while
                codeFields.Clear();
                MyConsole.Instance.WriteLine(" Handle " + totalCount + " ldstr instructions, "
                    + strValues.Count + " string values , span "
                    + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
            SelfPerformanceCounterForTest.Leave(h4);
        }

        private long EncryptStringValues_AddString( List<byte> lstDatas, string strValue , int keyOffset )
        {
            var bsContent = System.Text.Encoding.UTF8.GetBytes(strValue);
            var itemEncryptKey = _Random.Next(10000, ushort.MaxValue - 10000);
            long longKey = lstDatas.Count / 2;
            longKey = (longKey << 24) + strValue.Length;
            longKey = (longKey << 16) + (ushort)(itemEncryptKey ^ keyOffset);
            var key = itemEncryptKey;
            foreach (var c in strValue)
            {
                ushort v3 = (ushort)(c ^ key);
                lstDatas.Add((byte)(v3 >> 8));
                lstDatas.Add((byte)(v3 & 0xff));
                key++;
            }
            return longKey;
        }

        /// <summary>
        /// 需要进行加密的资源名称，支持正则表达式
        /// </summary>
        public string ResourceNameNeedEncrypt = null;

        internal void EncryptEmbeddedResource(List<DCILMethod> allMethod)
        {
            var rootCls = this._Type_JIEJIEHelper.LocalClass;
            bool isCancel = false;
            if (this.Document.RuntimeVersion != null)
            {
                if (this.Document.RuntimeVersion.StartsWith("v2.", StringComparison.OrdinalIgnoreCase)
                    || this.Document.RuntimeVersion.StartsWith("2."))
                {
                    // 遇到.NET2.0,则考虑是否取消加密内嵌资源，因为对于.NET2.0,类型System.Reflection.ManifestResourceInfo的构造函数是内部的，无法外部调用。
                    isCancel = true;
                    if (this.Document._CachedInvokeMethods != null)
                    {
                        bool hasMethod = false;
                        foreach (var item in this.Document._CachedInvokeMethods.Values)
                        {
                            if (item.MethodName == "GetManifestResourceInfo")
                            {
                                hasMethod = true;
                                break;
                            }
                        }
                        if (hasMethod == false)
                        {
                            // 所有的代码都没有调用GetManifestResourceInfo(),可以认为不会创建ManifestResourceInfo，可以加密内嵌资源。
                            isCancel = false;
                            rootCls.ChildNodes.RemoveByName("SMF_GetManifestResourceInfo");
                        }
                    }
                }
            }
            if (isCancel || this.Switchs.Resources == false )
            {
                // 不需要处理任何数据,删除无用的功能模块
                RemoveSMF_Function(rootCls);
                //for (int iCount = rootCls.ChildNodes.Count - 1; iCount >= 0; iCount--)
                //{
                //    if (rootCls.ChildNodes[iCount].Name.StartsWith("SMF_"))
                //    {
                //        rootCls.ChildNodes.RemoveAt(iCount);
                //    }
                //}
                //var cls2 = rootCls.GetNestedClass("SMF_ResStream");
                //if (cls2 != null)
                //{
                //    rootCls.NestedClasses.Remove(cls2);
                //}
                return;
            }
            // 收集要处理的数据
            var datas = new SortedDictionary<string, DCILMResource>();
            EntryNameSettingList resNameEncrypt = null;
            if(this.ResourceNameNeedEncrypt != null && this.ResourceNameNeedEncrypt.Length > 0 )
            {
                resNameEncrypt = new EntryNameSettingList(this.ResourceNameNeedEncrypt);
            }
            foreach (var item in this.Document.Resouces)
            {
                var resName = DCUtils.CleanSurroundingSemicolon(item.Key);
                //if(resName.Contains(".exe"))
                //{

                //}
                if (resNameEncrypt != null)
                {
                    var item2 = resNameEncrypt.GetItem(resName);
                    if (item2 != null)
                    {
                        if (item2.IsInclude)
                        {
                            datas[item.Key] = item.Value;
                        }
                        continue;
                    }
                }
                if (resName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase) == false
                    && resName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) == false
                    && resName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) == false
                    && item.Value.Data != null
                    && item.Value.Data.Length > 0)
                {
                    datas[item.Key] = item.Value;
                }
            }//foreach
            if (datas.Count == 0)
            {
                // 没有任何要处理的数据
                return;
            }
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting embedded resoruces ...");
            int startTick = Environment.TickCount;

            int changeCount = 0;
            foreach (var method in allMethod)
            {
                method.EnumOperCodes(delegate (EnumOperCodeArgs args)
                {
                    if (args.Current.OperCodeValue == DCILOpCodeValue.Callvirt
                        && args.Current is DCILOperCode_HandleMethod)
                    {
                        var hmcode = (DCILOperCode_HandleMethod)args.Current;
                        var targetMethod = hmcode.InvokeInfo;
                        string newMethodName = null;
                        if (targetMethod.OwnerType.Name == "System.Reflection.Assembly")
                        {
                            if (targetMethod.MethodName == "GetManifestResourceStream")
                            {
                                if (targetMethod.ParametersCount == 1)
                                {
                                    newMethodName = "SMF_GetManifestResourceStream";
                                }
                                else if (targetMethod.ParametersCount == 2)
                                {
                                    newMethodName = "SMF_GetManifestResourceStream2";
                                }
                            }
                            else if (targetMethod.MethodName == "GetManifestResourceNames")
                            {
                                newMethodName = "SMF_GetManifestResourceNames";
                            }
                            else if (targetMethod.MethodName == "GetManifestResourceInfo")
                            {
                                newMethodName = "SMF_GetManifestResourceInfo";
                            }
                        }
                        if (newMethodName != null)
                        {
                            targetMethod = targetMethod.Clone();
                            targetMethod.LocalMethod = (DCILMethod)this._Type_JIEJIEHelper.LocalClass.GetChildNodeByName(newMethodName);
                            targetMethod.OwnerType = this._Type_JIEJIEHelper;
                            targetMethod.IsInstance = false;
                            hmcode.SetOperCode("call");
                            hmcode.InvokeInfo = targetMethod;
                            changeCount++;
                        }
                    }
                });
            }
            if (changeCount == 0)
            {
                // 不需要处理任何数据,删除无用的功能模块
                RemoveSMF_Function(rootCls);

                //for (int iCount = rootCls.ChildNodes.Count - 1; iCount >= 0; iCount--)
                //{
                //    var item99 = rootCls.ChildNodes[iCount];
                //    if (item99.Name.StartsWith("SMF_") || item99.Name == "__SMF_Contents")
                //    {
                //        rootCls.ChildNodes.RemoveAt(iCount);
                //    }
                //    else if( item99.Name == ".cctor")
                //    {

                //    }
                //}
                //var cls2 = rootCls.GetNestedClass("SMF_ResStream");
                //if (cls2 != null)
                //{
                //    rootCls.NestedClasses.Remove(cls2);
                //}
                //var operCodes = rootCls.Method_Cctor.OperCodes;
                //operCodes.RemoveAt(0);
                //operCodes.RemoveAt(0);
                MyConsole.Instance.WriteLine(" Do nothing.");
            }
            else
            {
                byte xorKey = (byte)(new System.Random().Next(100, 233));
                var methodRead = rootCls.GetNestedClass("SMF_ResStream")?.GetChildNodeByName("Read") as DCILMethod;
                if (methodRead != null)
                {
                    foreach (var item in methodRead.OperCodes)
                    {
                        if (item.OperCode.StartsWith("ldc.i4") && item.OperData == "123")
                        {
                            item.OperData = xorKey.ToString();
                            break;
                        }
                    }
                }
                var methodCreateEmtpy = (DCILMethod)rootCls.GetChildNodeByName("SMF_CreateEmptyTable");
                methodCreateEmtpy.OperCodes.Clear();
                methodCreateEmtpy.OperCodes.AddItem( methodCreateEmtpy.GenNewLabelID() , "newobj" , "instance void class [" +this.Document.GetLibraryName("System.Collections.Generic.Dictionary`2" , true ) + "]System.Collections.Generic.Dictionary`2<string, uint8[]>::.ctor()");
              
                var methodGetContent = (DCILMethod)rootCls.GetChildNodeByName("SMF_GetContent");
                var codesGetContent = methodGetContent.OperCodes;
                codesGetContent.Clear();
                codesGetContent.AddItem(methodGetContent.GenNewLabelID(), "nop");
                string firstLabelID = methodGetContent.GenNewLabelID();
                var initTableCodes = new DCILOperCodeList();
                MyConsole.Instance.WriteLine();
                foreach (var item in datas)
                {
                    codesGetContent.AddItem(firstLabelID, "ldarg.0");
                    var resName = DCUtils.CleanSurroundingSemicolon( item.Key );

                    MyConsole.Instance.WriteLine("    Processing " + resName);

                    methodCreateEmtpy.OperCodes.AddItem(methodCreateEmtpy.GenNewLabelID(), "dup");
                    methodCreateEmtpy.OperCodes.Add(new DCILOperCode_LoadString(methodCreateEmtpy.GenNewLabelID(), resName));
                    methodCreateEmtpy.OperCodes.AddItem(methodCreateEmtpy.GenNewLabelID(), "ldnull");
                    methodCreateEmtpy.OperCodes.AddItem(methodCreateEmtpy.GenNewLabelID(), "callvirt", "instance void class [" + this.Document.GetLibraryName("System.Collections.Generic.Dictionary`2", true) + "]System.Collections.Generic.Dictionary`2<string, uint8[]>::Add(!0, !1)");

                    codesGetContent.Add(new DCILOperCode_LoadString(methodGetContent.GenNewLabelID(), resName));
                    codesGetContent.Add(new DCILOperCode(methodGetContent.GenNewLabelID(), "call", "bool [" + this.Document.GetLibraryName("System.String", true) + "]System.String::Equals(string, string)"));
                    firstLabelID = methodGetContent.GenNewLabelID();
                    codesGetContent.Add(new DCILOperCode(methodGetContent.GenNewLabelID(), "brfalse", firstLabelID));

                    var data = item.Value.Data;
                    for (int iCount = 0; iCount < data.Length; iCount++)
                    {
                        // 首先是加密
                        data[iCount] = (byte)(data[iCount] ^ xorKey);
                    }
                    int gzipLen = 0;
                    if (data.Length > 50 * 1024)
                    {
                        var ms2 = new System.IO.MemoryStream();
                        var gm2 = new System.IO.Compression.GZipStream(ms2, System.IO.Compression.CompressionMode.Compress);
                        gm2.Write(data, 0, data.Length);
                        gm2.Flush();
                        gm2.Close();
                        var data2 = ms2.ToArray();
                        ms2.Close();
                        float rate = (float)data2.Length / (float)data.Length;
                        if (rate < 0.6)
                        {
                            // 压缩比率比较大，则采用GZIP压缩
                            gzipLen = data.Length;
                            data = data2;
                        }
                    }
                    var dataWrite = new byte[data.Length + 4];
                    var bsLen = BitConverter.GetBytes(gzipLen);
                    Array.Copy(bsLen, dataWrite, 4);
                    Array.Copy(data, 0, dataWrite, 4, data.Length);
                    codesGetContent.Add(this._ByteDataContainer.GetOperCode(methodGetContent.GenNewLabelID(), dataWrite));
                    //codesGetContent.AddItem(methodGetContent.GenNewLabelID(), "callvirt", "instance void class [" + this.Document.LibName_mscorlib + "]System.Collections.Generic.Dictionary`2<string, uint8[]>::set_Item(!0, !1)");
                    codesGetContent.AddItem(methodGetContent.GenNewLabelID(), "ret");
                    item.Value.Dispose();
                    this.Document.Resouces.Remove(item.Key);
                }

                methodCreateEmtpy.OperCodes.AddItem(methodCreateEmtpy.GenNewLabelID(), "ret");

                codesGetContent.AddItem(firstLabelID, "ldnull");
                codesGetContent.AddItem(methodGetContent.GenNewLabelID(), "ret");
                MyConsole.Instance.WriteLine(" encrypt " + datas.Count + " resources ,span " + Math.Abs(Environment.TickCount - startTick) + " milliseconds.");
            }
        }

        private void RemoveSMF_Function( DCILClass rootCls )
        {
            for (int iCount = rootCls.ChildNodes.Count - 1; iCount >= 0; iCount--)
            {
                var item99 = rootCls.ChildNodes[iCount];
                if (item99.Name.StartsWith("SMF_") || item99.Name == "__SMF_Contents")
                {
                    rootCls.ChildNodes.RemoveAt(iCount);
                }
            }
            var cls2 = rootCls.GetNestedClass("SMF_ResStream");
            if (cls2 != null)
            {
                rootCls.NestedClasses.Remove(cls2);
            }
            var operCodes = rootCls.Method_Cctor.OperCodes;
            operCodes.RemoveAt(0);
            operCodes.RemoveAt(0);
        }
        internal void EncryptCharValue(List<DCILMethod> methods)
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting char values ...");
            int codeCount = 0;
            int tick = Environment.TickCount;
            var strCodes = new HashSet<string>(new string[] {
                "add","and","beq","beq.s","bgt","bgt.s","ble","ble.s","blt","blt.s","bne","bne.s","ceq","cgt","clt" });
            foreach (var method in methods)
            {
                if (method.RuntimeSwitchs.Strings == false)
                {
                    continue;
                }
                method.EnumOperCodes(delegate (EnumOperCodeArgs args)
                {
                    if (args.Current.OperCodeValue == DCILOpCodeValue.Ldc_I4
                    || args.Current.OperCodeValue == DCILOpCodeValue.Ldc_I4_S)
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
                                    //args.OwnerList[args.CurrentCodeIndex] = this.Int32ValueContainer.GetOperCode(args.Current.LabelID, intv);
                                    if (this.Int32ValueContainer.ChangeOperCode(args.OwnerList, args.CurrentCodeIndex, intv))
                                    {
                                        codeCount++;
                                    }
                                }
                            }
                            else if (nextCode != null && nextCode.OperCode != null)
                            {
                                vt = args.Method.GetTargetValueTypeForSet(nextCode);
                                if (vt == DCILTypeReference.Type_Char)
                                {
                                    //args.OwnerList[args.CurrentCodeIndex] = this.Int32ValueContainer.GetOperCode(args.Current.LabelID, intv);
                                    if (this.Int32ValueContainer.ChangeOperCode(args.OwnerList, args.CurrentCodeIndex, intv))
                                    {
                                        codeCount++;
                                    }
                                }
                            }
                        }
                    }
                });
            }
            tick = Math.Abs(Environment.TickCount - tick);
            MyConsole.Instance.WriteLine(" change " + codeCount + " ldc.i4 instructions, span " + tick + " milliseconds.");
        }
        /// <summary>
        /// 混淆成员方法代码控制流程
        /// </summary>
        internal void ObfuscateControlFlow()
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Obfuscate control flow ...");
            int methodCount = 0;
            int tick = Environment.TickCount;
            var methods = this.Document.GetAllMethodHasOperCodes();
            if(methods != null && methods.Count > 0 )
            {
                foreach(var method in methods )
                {
                    if(method.RuntimeSwitchs.ControlFlow && method.OperCodeSpecifyStructure == false )
                    {
                        if(ObfuscateMethodOperCodes( method ))
                        {
                            methodCount++;
                        }
                    }
                }
            }
            tick = Math.Abs(Environment.TickCount - tick);
            MyConsole.Instance.WriteLine(" handle " + methodCount + " methods, span " + tick + " milliseconds.");
        }

        /// <summary>
        /// 加密方法参数中的枚举类型数值
        /// </summary>
        internal void EncryptMethodParamterEnumValue()
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting enum paramter values ...");
            int codeCount = 0;
            int tick = Environment.TickCount;
            this.Document.EnumAllOperCodes(delegate (EnumOperCodeArgs args)
            {
                if (args.Current is DCILOperCode_HandleMethod && args.CurrentCodeIndex > 0)
                {
                    var preCode = args.OwnerList[args.CurrentCodeIndex - 1];
                    if (preCode.OperCodeValue == DCILOpCodeValue.Ldc_I4
                        || preCode.OperCodeValue == DCILOpCodeValue.Ldc_I4_S)
                    {
                        var code = (DCILOperCode_HandleMethod)args.Current;
                        if (code.InvokeInfo != null && code.InvokeInfo.ParametersCount > 0)
                        {
                            var vt = code.InvokeInfo.Paramters[code.InvokeInfo.ParametersCount - 1].ValueType;
                            if (vt.Mode == DCILTypeMode.ValueType)
                            {
                                var intValue = DCUtils.ConvertToInt32(preCode.OperData);
                                //args.OwnerList[args.CurrentCodeIndex - 1] = this.Int32ValueContainer.GetOperCode(preCode.LabelID, intValue);
                                if (this.Int32ValueContainer.ChangeOperCode(args.OwnerList, args.CurrentCodeIndex - 1, intValue))
                                {
                                    codeCount++;
                                }
                            }
                        }
                    }
                }
            });
            tick = Math.Abs(Environment.TickCount - tick);
            MyConsole.Instance.WriteLine(" change " + codeCount + " call/callvirt instructions, span " + tick + " milliseconds.");
        }

        internal class OperCodeReference
        {
            public OperCodeReference(DCILMethod method, DCILOperCode code, DCILOperCodeList list, int index)
            {
                this.Method = method;
                this.Code = code;
                this.OwnerList = list;
                this.Index = index;
            }
            public DCILMethod Method = null;
            public DCILOperCode Code = null;
            public DCILOperCodeList OwnerList = null;
            public int Index = 0;
        }
        public void EncryptTypeHandle(List<DCILMethod> allMethods)
        {
            ConsoleWriteTask();
            MyConsole.Instance.Write("Encrypting typeof() instructions ...");
            int tick = Environment.TickCount;

            var strNativeCodes = new Dictionary<DCILTypeReference, List<OperCodeReference>>();
            foreach (var method in allMethods)
            {
                if (method.RuntimeSwitchs.ControlFlow == false)
                {
                    continue;
                }
                method.EnumOperCodes(delegate (EnumOperCodeArgs args)
                {
                    var code = args.Current;
                    if (code is DCILOperCode_LdToken && args.CurrentCodeIndex < args.OwnerList.Count - 1)
                    {
                        var nextCode = args.OwnerList[args.CurrentCodeIndex + 1];
                        if (nextCode.OperCodeValue == DCILOpCodeValue.Call && nextCode is DCILOperCode_HandleMethod)
                        {
                            var mInfo = ((DCILOperCode_HandleMethod)nextCode).InvokeInfo;
                            if (mInfo.OwnerType.Name == "System.Type" && mInfo.MethodName == "GetTypeFromHandle")
                            {
                                var ldtoken = (DCILOperCode_LdToken)code;
                                if (ldtoken.ClassType != null)
                                {
                                    if (ldtoken.ClassType.IsGenericType
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
                                    list4.Add(new OperCodeReference(method, code, args.OwnerList, args.CurrentCodeIndex));
                                }
                            }
                        }
                    }
                });
            }
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
                this.UpdateRuntimeSwitchs_Class(clsContainer, this.Switchs);
                var strListTypeName = this.Document.GetTypeNameWithLibraryName("System.Collections.Generic.List`1", null, typeof(System.Collections.Generic.List<>));
                var strAddCode = "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::Add(!0)";
                var addOperCode = new DCILOperCode(null, "callvirt", "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::Add(!0)");
                var method_Cctor = clsContainer.Method_Cctor;
                var operCodes = method_Cctor.OperCodes;
                var methodGetHandle = (DCILMethod)clsContainer.GetChildNodeByName("GetTypeInstance");
                //var labelGen = new ILLabelIDGen();
                methodGetHandle.CacheInfo(this.Document, this.Document.GetAllClassesUseCache());
                operCodes.Clear();
                operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "newobj", "instance void class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::.ctor()"));
                var array = new DCILField[types.Count];
                int fieldIndex = -1;
                int totalOperCodes = 0;
                foreach (var type in types)
                {
                    fieldIndex++;
                    operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "dup", null));
                    operCodes.Add(new DCILOperCode_LdToken(method_Cctor.GenNewLabelID(), type));
                    operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "callvirt", strAddCode));
                    var list = strNativeCodes[type];
                    totalOperCodes += list.Count;
                    foreach (var codeInfo in list)
                    {
                        codeInfo.OwnerList[codeInfo.Index] = this.Int32ValueContainer.GetOperCode(codeInfo.Code.LabelID, fieldIndex);
                        codeInfo.OwnerList[codeInfo.Index + 1] = new DCILOperCode_HandleMethod(codeInfo.Method.GenNewLabelID(), "call", methodGetHandle);
                    }
                }
                operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "callvirt", "instance !0[] class " + strListTypeName + "<valuetype [" + this.Document.LibName_mscorlib + "]System.RuntimeTypeHandle>::ToArray()"));
                operCodes.Add(new DCILOperCode_HandleField(method_Cctor.GenNewLabelID(), "stsfld", new DCILFieldReference((DCILField)clsContainer.GetChildNodeByName("_Handles"))));
                operCodes.Add(new DCILOperCode(method_Cctor.GenNewLabelID(), "ret", null));
                //ObfuscateMethodOperCodes(clsContainer.Method_Cctor);
                this.Document.ClearCacheForAllClasses();
                if(this.Switchs.ControlFlow )
                {
                    ObfuscateMethodOperCodes(method_Cctor);
                }
                MyConsole.Instance.WriteLine(" handle " + types.Count + " types , " + totalOperCodes + " instructions , span " + (Environment.TickCount - tick) + " milliseconds.");
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
        internal class DCRuntimeFieldHandleContainer : IDisposable
        {
            public DCRuntimeFieldHandleContainer(DCJieJieNetEngine eng, DCILDocument document)
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
                this._Class = new DCILClass(strILCode, document);
                var fields = new List<DCILField>();
                foreach (var cls in document.GetAllClassesUseCache().Values)
                {
                    foreach (var item in cls.ChildNodes)
                    {
                        if (item is DCILField)
                        {
                            var field = (DCILField)item;
                            if (field.ReferenceData != null)//.DataLabel != null && field.DataLabel.Length > 0 )
                            {
                                fields.Add(field);
                            }
                        }
                    }
                }
                DCUtils.ObfuseListOrder(fields);
                for (int iCount = fields.Count - 1; iCount >= 0; iCount--)
                {
                    _FieldIndexs[fields[iCount]] = iCount;
                }
                document.Classes.Add(_Class);
                eng.UpdateRuntimeSwitchs_Class(this._Class, eng.Switchs);
                this._Class.OwnerDocument = document;
                this._Class.RuntimeSwitchs = eng.Switchs;
                document.ClearCacheForAllClasses();
                this._Method_GetHandle = (DCILMethod)this._Class.GetChildNodeByName("GetHandle");
                this._RFH = new DCILTypeReference(typeof(System.RuntimeFieldHandle), document);
            }
            public void Dispose()
            {
                if (this._Class != null)
                {
                    this._Class.Dispose();
                    this._Class = null;
                }
                this._Method_GetHandle = null;
                this._RFH = null;
                if (this._FieldIndexs != null)
                {
                    this._FieldIndexs.Clear();
                    this._FieldIndexs = null;
                }
            }
            public void Commit( DCJieJieNetEngine eng )
            {
                var doc = this._Class.OwnerDocument;
                var strListTypeName = doc.GetTypeNameWithLibraryName("System.Collections.Generic.List`1", null, typeof(System.Collections.Generic.List<>));
                var strAddCode = "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::Add(!0)";
                var addOperCode = new DCILOperCode(null, "callvirt", "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::Add(!0)");
                var method = this._Class.Method_Cctor;
                method.ObfuscateControlFlowFlag = false;
                var operCodes = method.OperCodes;
                operCodes.Clear();
                //var labelGen = new ILLabelIDGen();
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "newobj", "instance void class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::.ctor()"));
                var array = new DCILField[_FieldIndexs.Count];
                foreach (var item in _FieldIndexs)
                {
                    array[item.Value] = item.Key;
                }
                foreach (var item in array)
                {
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "dup", null));
                    operCodes.Add(new DCILOperCode_LdToken(method.GenNewLabelID(), new DCILFieldReference(item)));
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "callvirt", strAddCode));
                }
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "callvirt", "instance !0[] class " + strListTypeName + "<valuetype [" + doc.LibName_mscorlib + "]System.RuntimeFieldHandle>::ToArray()"));
                operCodes.Add(new DCILOperCode_HandleField(method.GenNewLabelID(), "stsfld", new DCILFieldReference((DCILField)this._Class.GetChildNodeByName("_Handles"))));
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "ret", null));
                if(eng.Switchs.ControlFlow)
                {
                    eng.ObfuscateMethodOperCodes(method);
                }
                //this._Class.Method_Cctor.CacheInfo(doc, doc.GetAllClassesUseCache());
            }

            public DCILMethod _Method_GetHandle = null;
            private Dictionary<DCILField, int> _FieldIndexs = new Dictionary<DCILField, int>();

            private DCILTypeReference _RFH = null;
            public DCILClass _Class = null;
            public int GetFieldIndex(DCILField field)
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
        }

        private DCInt32ValueContainer _Int32ValueContainer = null;
        public DCInt32ValueContainer Int32ValueContainer
        {
            get
            {
                if (this._Int32ValueContainer == null)
                {
                    this._Int32ValueContainer = new DCInt32ValueContainer(this.Document, this);
                }
                return this._Int32ValueContainer;
            }
        }
        internal class DCInt32ValueContainer : IDisposable
        {
            public DCInt32ValueContainer(DCILDocument document, DCJieJieNetEngine eng)
            {
                if (document == null)
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
                document.Classes.Add(this._Class);
                document.ClearCacheForAllClasses();
                eng.UpdateRuntimeSwitchs_Class(this._Class , eng.Switchs);
                this.GetField(Environment.TickCount);
            }
            public void Dispose()
            {
                if (this._Class != null)
                {
                    this._Class.Dispose();
                    this._Class = null;
                }
                if (this._Fields != null)
                {
                    this._Fields.Clear();
                    this._Fields = null;
                }
            }
            public DCILClass _Class = null;
            private Dictionary<int, DCILField> _Fields = new Dictionary<int, DCILField>();
            public void Commit(DCJieJieNetEngine eng )
            {
                var method = this._Class.Method_Cctor;
                method.ObfuscateControlFlowFlag = false;
                var operCodes = method.OperCodes;
                operCodes.Clear();
                var list = new List<DCILField>(this._Fields.Values);
                DCUtils.ObfuseListOrder(list);
                int currentValue = Environment.TickCount;
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "ldc.i8", DCUtils.GetInt32ValueString(currentValue)));
                int listCount = list.Count - 1;
                for (int iCount = 0; iCount <= listCount; iCount++)
                {
                    var field = list[iCount];
                    int v = field.InnerTag - currentValue;
                    currentValue = field.InnerTag;
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "ldc.i8", DCUtils.GetInt32ValueString(v)));
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "add", null));
                    if (iCount < listCount)
                    {
                        operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "dup", null));
                    }
                    operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "conv.i4", null));
                    operCodes.Add(new DCILOperCode_HandleField(method.GenNewLabelID(), "stsfld", new DCILFieldReference(field)));
                }
                operCodes.Add(new DCILOperCode(method.GenNewLabelID(), "ret", null));
                if(eng.Switchs.ControlFlow )
                {
                    eng.ObfuscateMethodOperCodes(method);
                }
            }

            public DCILField GetField(int v)
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
                    //field._Name = GetIndexName(  this._Class.ChildNodes.Count );
                    if (v == int.MaxValue)
                    {
                        field._Name = "_IntMaxValue";
                    }
                    else if (v == int.MinValue)
                    {
                        field._Name = "_IntMinValue";
                    }
                    else if (v >= 0)
                    {
                        field._Name = "_" + this._Class.ChildNodes.Count + "_" + v;
                    }
                    else
                    {
                        field._Name = "_" + this._Class.ChildNodes.Count + "_N_" + Convert.ToString(-v);
                    }
                    field.InnerTag = v;
                    this._Class.ChildNodes.Add(field);
                    this._Fields[v] = field;
                }
                return field;
            }
            /// <summary>
            /// 修改指令
            /// </summary>
            /// <param name="list">指令列表</param>
            /// <param name="index">要修改的序号</param>
            /// <param name="intValue">整数数值</param>
            /// <returns>是否修改了指令列表</returns>
            public bool ChangeOperCode(DCILOperCodeList list, int index, int intValue)
            {
                var field = GetField(intValue);
                if (field != null)
                {
                    var oldCode = list[index];
                    var newCode = new DCILOperCode_HandleField(oldCode.LabelID, "ldsfld", new DCILFieldReference(field));
                    list[index] = newCode;
                    if (oldCode.OperCodeValue != DCILOpCodeValue.Ldc_I4)
                    {
                        //newCode.BitSizeChanged = true;
                        list.ItemBitSizeChanged = true;
                        list.ChangeShortInstruction();
                    }
                    return true;
                }
                return false;
            }

            public DCILOperCode GetOperCode(string labelID, int v)
            {
                var field = GetField(v);
                if (field == null)
                {
                    var code = new DCILOperCode(
                                labelID,
                                DCILOperCodeDefine._ldc_i4,
                                DCUtils.GetInt32ValueString(v));
                    //code.BitSizeChanged = true;
                    return code;
                }
                else
                {
                    var result = new DCILOperCode_HandleField(
                        labelID, 
                        DCILOperCodeDefine._ldsfld ,
                        new DCILFieldReference(field));
                    //result.BitSizeChanged = true;
                    return result;
                }
            }
        }
         
        private static readonly Random _Random = new Random();
         
        public bool ObfuscateMethodOperCodes(DCILMethod method)
        {
            if(method == null || method.ObfuscateControlFlowFlag == true )
            {
                return false;
            }
            method.ObfuscateControlFlowFlag = true;
            if( ObfuscateOperCodeListNew2( method , method.OperCodes , false , null , 0 ))
            //if (ObfuscateOperCodeList(method, method.OperCodes, false, null))
            {
                method.Maxstack += 3;
                //method.OperCodes.ChangeShortInstruction();
                return true;
            }
            return false;
        }

        //private ILLabelIDGen _LabelGen_MyInitializeArray = new ILLabelIDGen ("IL_MIA");
        private void ChangeSpecifyCallTarget(DCILOperCodeList items, DCILMethod method)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }
            if (this._Type_JIEJIEHelper == null)
            {
                throw new NotSupportedException(CodeTemplate._ClassName_JIEJIEHelper);
            }
            if (method.Name == ".cctor")
            {
                // 遇到静态构造函数，则加密整数数值。
                for (int iCount = items.Count - 1; iCount >= 0; iCount--)
                {
                    var code = items[iCount];
                    if (code.OperCodeValue == DCILOpCodeValue.Ldc_I4)
                    {
                        var intValue = DCUtils.ConvertToInt32(code.OperData);
                        this.Int32ValueContainer.ChangeOperCode(items, iCount, intValue);
                        //items[iCount] = this.Int32ValueContainer.GetOperCode(code.LabelID, intValue);
                    }
                }
            }
        }

        private List<DCILOperCode_HandleMethod> _CallOperCodes = new List<DCILOperCode_HandleMethod>();
         
        private NumberNameGen _Int32ValueString = new NumberNameGen(null);
        private static List<string> codes_br_2 = new List<string>(new string[] { "ble", "bgt", "blt", "bne" });
        private static List<string> codes_brs_2 = new List<string>(new string[] { "ble.s", "bgt.s", "blt.s", "bne.s" });
        private static List<string> codes_max_2 = new List<string>(new string[] { "add", "sub", "div", "mul", "xor", "and" });


        public bool ObfuscateOperCodeListNew2(DCILMethod method, DCILOperCodeList items, bool isInTryBlock, DCILOperCodeList parentList, int level)
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

            if (isMethodOfInt32ValueContainer == false)
            {
                foreach (var item in items)
                {
                    if (item is DCILOperCode_Try_Catch_Finally)
                    {
                        var tcf = (DCILOperCode_Try_Catch_Finally)item;
                        if (tcf._Try != null && ObfuscateOperCodeListNew2(method, tcf._Try.OperCodes, true, items, level + 1))
                        {
                            items.ChangeShortInstruction();
                            result = true;
                        }
                        if (method.OwnerClass?.Name != CodeTemplate._ClassName_JIEJIEHelper)
                        {
                            ChangeSpecifyCallTarget(tcf._Finally?.OperCodes, method);
                        }
                    }
                }
                if (method.OwnerClass?.Name != CodeTemplate._ClassName_JIEJIEHelper)
                {
                    ChangeSpecifyCallTarget(items, method);
                }
            }
            foreach (var item in items)
            {
                if (item.OperCodeValue == DCILOpCodeValue.Call
                    || item.OperCodeValue == DCILOpCodeValue.Callvirt)
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
            //var groupMaxLen = 20;// _Random.Next(20, 30);
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
            //DCILOperCode retCode = null;
            //if (items[items.Count - 1].OperCode == "ret")
            //{
            //    retCode = items[items.Count - 1];
            //    items[items.Count - 1] = new DCILOperCode(this._labelGenGlobal.Gen(), "br", retCode.LabelID);
            //    //items.RemoveAt(items.Count - 1);
            //}
            var lastCode = items[items.Count - 1];
            if (lastCode.HasLabelID())
            {
                items.RemoveAt(items.Count - 1);
            }
            else
            {
                lastCode = null;
            }
            // 进行指令分组
            var groups = new List<DCILOperCodeList>();
            var newGroup = new DCILOperCodeList();
            groups.Add(newGroup);
            int stackLevel = 0;
            int byteSize = 0;
            int itemsCount = items.Count;
            //DCILOperCode preCode = null;
            //if (method.Name == "SplitQuoted" && method.Parent.Name == "Open_ICSharpCode_SharpZipLib_Core.NameFilter")
            //{

            //}
            //var def_br = DCILOperCodeDefine.GetDefine("br");
            //var def_br_s = DCILOperCodeDefine.GetDefine("br.s");
            //var def_pop = DCILOperCodeDefine.GetDefine("pop");
            //var def_dup = DCILOperCodeDefine.GetDefine("dup");
            //var def_brtrue = DCILOperCodeDefine.GetDefine("brtrue");
            //var def_brtrue_s = DCILOperCodeDefine.GetDefine("brtrue.s");

            var middleLabels = new List<string>();
            for (int iCount = 0; iCount < itemsCount; iCount++)
            {
                var item = items[iCount];
                if (newGroup.Count == 0 && item.HasLabelID() == false)
                {
                    newGroup.Add(new DCILOperCode(method.GenNewLabelID(), "nop", null));
                }
                newGroup.Add(item);

                stackLevel += item.StackOffset;
                byteSize += item.ByteSize;
                //if(newGroup.Count > groupMaxLen)
                //{
                //    if(item.NativeDefine == def_br || item.NativeDefine == def_br_s)
                //    {
                //        newGroup = new DCILOperCodeList();
                //        groups.Add(newGroup);
                //        preCode = item;
                //        continue;
                //    }
                //}
                if (stackLevel < 0)
                {

                }
                if ((item.NativeDefine == DCILOperCodeDefine._br_s || item.NativeDefine == DCILOperCodeDefine._br)
                    && iCount < itemsCount - 2
                    && items[iCount + 2].LabelID == item.OperData)
                {
                    if (items[iCount + 1].StackOffset == 1)// .OperCode == "ldc.i4.1" || items[iCount + 1].OperCode == "ldc.i4.0")
                    {
                        // 可能遇到一种特殊的结构
                        var lbl2 = items[iCount + 1].LabelID;
                        bool matchJump = false;
                        for (int iCount2 = iCount - 1; iCount2 >= 0; iCount2--)
                        {
                            var def3 = items[iCount2].NativeDefine;
                            if (def3.WithoutOperData == false
                                && (def3.ExtCodeType == ILOperCodeType.Jump || def3.ExtCodeType == ILOperCodeType.JumpShort)
                                && items[iCount2].OperData == lbl2)
                            {
                                matchJump = true;
                                break;
                            }
                        }
                        if (matchJump)
                        {
                            // 确定为特殊结构
                            newGroup.Add(items[iCount + 1]);
                            //newGroup.Add(items[iCount + 2]);
                            //stackLevel -= items[iCount + 2].StackOffset;
                            iCount += 1;
                            continue;
                        }
                    }
                }
                if (item.NativeDefine == DCILOperCodeDefine._dup)
                {
                    if (items.SafeGetNativeDefine(iCount + 1) == DCILOperCodeDefine._brtrue
                        || items.SafeGetNativeDefine(iCount + 1) == DCILOperCodeDefine._brtrue_s)
                    {
                        if (items.SafeGetNativeDefine(iCount + 2) == DCILOperCodeDefine._pop)
                        {
                            if (items.SafeGetNativeDefine(iCount + 3) == DCILOperCodeDefine._br
                                || items.SafeGetNativeDefine(iCount + 3) == DCILOperCodeDefine._br_s)
                            {
                                // 识别出一种特殊语法结构
                                var brItem = items[iCount + 3];
                                var labelID = brItem.OperData;
                                for (iCount++; iCount < itemsCount; iCount++)
                                {
                                    if (items[iCount].LabelID == labelID)
                                    {
                                        iCount--;
                                        //preCode = items[iCount - 1];
                                        newGroup = new DCILOperCodeList();
                                        groups.Add(newGroup);
                                        stackLevel = 0;
                                        byteSize = 0;
                                        break;
                                    }
                                    else
                                    {
                                        newGroup.Add(items[iCount]);
                                    }
                                }
                                continue;
                            }
                        }
                    }
                }
                if (stackLevel == 0)
                {

                }
                if (newGroup.Count > groupMaxLen
                    && stackLevel == 0
                    && item.IsPrefixOperCode() == false)
                {
                    newGroup = new DCILOperCodeList();
                    groups.Add(newGroup);
                }
            }

            if (groups[groups.Count - 1].Count == 0)
            {
                // 删除最后一个空白组
                groups.RemoveAt(groups.Count - 1);
            }
            if (groups.Count == 1)
            {
                // 只分得一组，无需混淆
                if (lastCode != null)
                {
                    items.Add(lastCode);
                }
                if (items.Count > 100)
                {
                    //var dt = items.GetDebugTextForStackOffset();
                    return ObfuscateOperCodeList_Rude(method, items, isInTryBlock, parentList, level);
                }
                return false;
            }
            var lastGroup = groups[groups.Count - 1];
            if (lastCode != null)
            {
                lastGroup.AddItem(method.GenNewLabelID(), DCILOperCodeDefine._br , lastCode.LabelID);
            }
            var resultGroups = new List<DCILOperCodeList>(groups);
            var nullCount = (int)_Random.Next(groups.Count, (int)(groups.Count * 1.5));
            for (int iCount = 0; iCount < nullCount; iCount++)
            {
                resultGroups.Add(null);
            }
            DCUtils.ObfuseListOrder(resultGroups);

            var resultList = new DCILOperCodeList();
            resultList.AddItem(method.GenNewLabelID(), "nop");
            var firstGIndex = resultGroups.IndexOf(groups[0]);
            if (isMethodOfInt32ValueContainer)
            {
                resultList.AddItem(
                    method.GenNewLabelID(),
                    DCILOperCodeDefine._ldc_i4,
                    DCUtils.GetInt32ValueString(firstGIndex));
            }
            else
            {
                resultList.Add(this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), firstGIndex));
            }
            method.MaxstackFix = 1;
            //var startLabels = new List<string>();
            //for(int iCount = 0;iCount < 4;iCount ++)
            //{
            //    //startLabels.Add(resultList.AddItem(method.GenNewLabelID(), "nop").LabelID);
            //    startLabels.Add(
            //        resultList.AddItem(
            //            method.GenNewLabelID(),
            //            "ldc.i4.s" , 
            //            DCUtils.GetInt32ValueString( _Random.Next( 0 , 100))).LabelID);
            //    resultList.AddItem(method.GenNewLabelID(), "pop" );
            //}
            string switchLabel = method.GenNewLabelID();
            //resultList.AddItem(method.GenNewLabelID(), "ldc.i4.1");
            //resultList.AddItem(method.GenNewLabelID(), "brtrue", switchLabel);
            resultList.AddItem(method.GenNewLabelID(), "br", switchLabel);
            //resultList.AddItem(method.GenNewLabelID(), "dup");
            string strPopLabel = resultList.AddItem(method.GenNewLabelID(), "pop").LabelID;
            var startSwitch = new DCILOperCode_Switch(switchLabel);// method.GenNewLabelID());
            resultList.Add(startSwitch);
            //startLabels.Add( startSwitch.LabelID );
            foreach (var group in resultGroups)
            {
                if (group == null)
                {
                    var g2 = groups[_Random.Next(0, groups.Count)];
                    startSwitch.TargetLabels.Add(g2.FirstLabelID);
                    continue;
                }
                if (group[0] is DCILOperCode_Try_Catch_Finally)
                {
                    group.Insert(0, new DCILOperCode(method.GenNewLabelID(), DCILOperCodeDefine._nop));
                }
                startSwitch.TargetLabels.Add(group.FirstLabelID);
                if (group != lastGroup)
                {
                    int gIndex = groups.IndexOf(group);
                    var nextGroup = groups[gIndex + 1];
                    gIndex = resultGroups.IndexOf(nextGroup);

                    if (isMethodOfInt32ValueContainer)
                    {
                        group.Add(DCILOperCode.Gen_ldci4_Code(method.GenNewLabelID(), gIndex));
                    }
                    else
                    {
                        group.Add(this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), gIndex));
                    }
                    if (_Random.Next(0, 100) > 50)
                    {
                        group.AddItem(method.GenNewLabelID(), DCILOperCodeDefine._br, startSwitch.LabelID);
                    }
                    else
                    {
                        if (isMethodOfInt32ValueContainer)
                        {
                            group.Add(DCILOperCode.Gen_ldci4_Code(method.GenNewLabelID(), _Random.Next(0, groups.Count)));
                        }
                        else
                        {
                            group.Add(this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), _Random.Next(0, groups.Count)));
                        }
                        group.AddItem(method.GenNewLabelID(), DCILOperCodeDefine._br, strPopLabel);
                    }
                    //group.AddItem(method.GenNewLabelID(), DCILOperCodeDefine._br, startLabels[_Random.Next(0, startLabels.Count)]);// startSwitch.LabelID);
                }
                resultList.AddRange(group);
            }
            if (lastCode != null)
            {
                resultList.Add(lastCode);
            }
            items.Clear();
            items.AddRange(resultList);
            items.OnModified();
            items.ChangeShortInstruction();
            int addCount = items.Count - oldItemsCount;
            return true;
        }
         
        private bool ObfuscateOperCodeList_Rude(DCILMethod method, DCILOperCodeList items, bool isInTryBlock, DCILOperCodeList parentList, int level)
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
                    newGroup.Add(new DCILOperCode(method.GenNewLabelID(), "nop", null));
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

            for (int iCount = 0; iCount < groups.Count - 1; iCount++)
            {
                groups[iCount].NextGroup = groups[iCount + 1];
            }
            var firstGroup = groups[0];
            string strSwitchLabelID = method.GenNewLabelID();
            var strSwitch = new StringBuilder();
            foreach (var g in groups)
            {
                if (strSwitch.Length > 0)
                {
                    strSwitch.Append(',');
                }
                strSwitch.Append(g[0].LabelID);
            }
            if (method.MaxstackFix < level * 3)
            {
                method.MaxstackFix = level * 3;
            }
            var extLocalIndex = int.MinValue;
            var extLocalValue = int.MinValue;
            DCILOperCodeList firstGroupShadow = null;
            if (isMethodOfInt32ValueContainer == false)
            {
                firstGroupShadow = new DCILOperCodeList();
                foreach (var item in firstGroup)
                {
                    if ((item is DCILOperCode_Try_Catch_Finally) == false)
                    {
                        var item2 = item.Clone(method.GenNewLabelID());// this._labelGenGlobal.Gen());
                        if (item2.OperCodeValue == DCILOpCodeValue.Ldc_I4
                            || item2.OperCodeValue == DCILOpCodeValue.Ldc_I4_S)
                        {
                            var v2 = DCUtils.ConvertToInt32(item2.OperData);
                            item2.SetOperCode("ldc.i4");
                            item2.OperData = DCUtils.GetInt32ValueString(v2 + _Random.Next(-100, 100));
                        }
                        if (codes_br_2.Contains(item2.OperCode))
                        {
                            item2.SetOperCode(codes_br_2[_Random.Next(0, codes_br_2.Count - 1)]);
                        }
                        else if (codes_brs_2.Contains(item2.OperCode))
                        {
                            item2.SetOperCode(codes_brs_2[_Random.Next(0, codes_brs_2.Count - 1)]);
                        }
                        else if (codes_max_2.Contains(item2.OperCode))
                        {
                            item2.SetOperCode(codes_max_2[_Random.Next(0, codes_max_2.Count - 1)]);
                        }
                        //else if(item2.OperCode == "ldc.i4.0")
                        //{
                        //    item2.OperCode = "ldc.i4.s";
                        //    item2.OperData = _Random.Next(-200, 200).ToString();
                        //}
                        else if (item2.OperCodeValue == DCILOpCodeValue.Brtrue
                            || item2.OperCodeValue == DCILOpCodeValue.Brtrue_S)
                        {
                            item2.SetOperCode("brfalse");
                        }
                        else if (item2.OperCodeValue == DCILOpCodeValue.Brfalse
                            || item2.OperCodeValue == DCILOpCodeValue.Brfalse_S)
                        {
                            item2.SetOperCode("brtrue");
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
                firstGroup.Insert(0, this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), extLocalValue));
                firstGroup.Insert(1, new DCILOperCode(method.GenNewLabelID(), "stloc", DCUtils.GetInt32ValueString(extLocalIndex)));

                firstGroupShadow.Insert(0, this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), extLocalValue == 0 ? 1 : 0));
                firstGroupShadow.Insert(1, new DCILOperCode(method.GenNewLabelID(), "stloc", DCUtils.GetInt32ValueString(extLocalIndex)));

                firstGroupShadow.NextGroup = firstGroup.NextGroup;

                groups.Add(firstGroupShadow);
            }
            else
            {
            }

            var resultList = new DCILOperCodeList();
            DCUtils.ObfuseListOrder(groups);
            string idForLastCode = lastCode.LabelID;
            if (idForLastCode == null || idForLastCode.Length == 0)
            {
                idForLastCode = method.GenNewLabelID();
            }

            if (firstGroupShadow != null)
            {
                var flag = _Random.Next(30, 100);
                var retLabelID = method.GenNewLabelID();
                resultList.Add(this.Int32ValueContainer.GetOperCode(method.GenNewLabelID(), flag));
                if (_Random.Next(0, 100) > 50)
                {
                    // 直接命中
                    resultList.AddItem(method.GenNewLabelID(), "ldc.i4", DCUtils.GetInt32ValueString(flag - 10));
                    resultList.AddItem(method.GenNewLabelID(), "bgt", firstGroup.FirstLabelID);
                    resultList.AddItem(method.GenNewLabelID(), "br", firstGroupShadow.FirstLabelID);
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "1");
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + firstGroup.FirstLabelID + "," + firstGroupShadow.FirstLabelID + ")");
                }
                else
                {
                    // 间接命中
                    resultList.AddItem(method.GenNewLabelID(), "ldc.i4", DCUtils.GetInt32ValueString(flag + 10));
                    resultList.AddItem(method.GenNewLabelID(), "bgt", firstGroupShadow.FirstLabelID);
                    resultList.AddItem(method.GenNewLabelID(), "br", firstGroup.FirstLabelID);
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "ldc.i4", "1");
                    //resultList.AddItem(this._labelGenGlobal.Gen(), "switch", "(" + firstGroupShadow.FirstLabelID + "," + firstGroup.FirstLabelID + ")");
                }

            }
            else
            {
                resultList.AddItem(method.GenNewLabelID(), "br", firstGroup.FirstLabelID);

            }

            int codeCount = 0;
            string lastLabelID = null;
            foreach (var group2 in groups)
            {
                foreach (var item2 in group2)
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
                            || item2.OperCodeValue == DCILOpCodeValue.Brtrue
                            || item2.OperCodeValue == DCILOpCodeValue.Brtrue_S
                            || item2.OperCodeValue == DCILOpCodeValue.Brfalse
                            || item2.OperCodeValue == DCILOpCodeValue.Brfalse_S)
                        {
                            addCode = true;
                        }
                        if (addCode)
                        {
                            //if( lastLabelID == null )
                            {
                                lastLabelID = item2.OperData;
                            }
                            resultList.AddItem(method.GenNewLabelID(), "ldloc", DCUtils.GetInt32ValueString(extLocalIndex));
                            if (extLocalValue == 0)
                            {
                                resultList.AddItem(method.GenNewLabelID(), "brtrue", lastLabelID);// firstGroupShadow.FirstLabelID);
                            }
                            else
                            {
                                resultList.AddItem(method.GenNewLabelID(), "brfalse", lastLabelID);// firstGroupShadow.FirstLabelID);
                            }
                            codeCount = 0;
                            lastLabelID = item2.OperData;
                        }
                    }

                }

                if (group2 == firstGroupShadow)
                {
                    //continue;
                }
                if (group2.NextGroup == null)
                {
                    resultList.AddItem(method.GenNewLabelID(), "br", idForLastCode);
                }
                else
                {
                    resultList.AddItem(method.GenNewLabelID(), "br", group2.NextGroup[0].LabelID);
                }
            }

            if (lastCode.HasLabelID() == false)
            {
                resultList.AddItem(idForLastCode, "nop");
            }
            resultList.Add(lastCode);
            items.Clear();
            items.AddRange(resultList);
            items.OnModified();
            items.ChangeShortInstruction();
            int addCount = items.Count - oldItemsCount;
            return true;
        }
    }


}
