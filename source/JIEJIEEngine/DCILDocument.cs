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
    internal class DCILDocument : DCILObject
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
        /// .NET Core版本号
        /// </summary>
        public string NetCoreVersion = null;
        /// <summary>
        /// 文档注释XML文档
        /// </summary>
        public System.Xml.XmlDocument CommentXmlDoc = null;
        /// <summary>
        /// 程序集文件配套的deps.json文件的内容
        /// </summary>
        public byte[] Content_DepsJson = null;
        
        public void LoadByReader(string fileName, System.Text.Encoding encoding)
        {
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.LoadByReader);
            this.LibraryNames = new Dictionary<string, string>();
            var reader = new DCILReader(fileName, encoding, this);
            this.RootPath = Path.GetDirectoryName(fileName);
            this.FileName = fileName;
            this._Name = Path.GetFileNameWithoutExtension(fileName);
            this.Load(reader);
            reader.Close();
            SelfPerformanceCounterForTest.Leave(h4);
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
         
        internal Dictionary<DCILTypeReference, DCILTypeReference> _CachedTypes = null;

        public Dictionary<string, string> LibraryNames = new Dictionary<string, string>();
        public string GetLibraryName(string typeName, bool useDefault = false)
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
            if(useDefault )
            {
                return this.LibName_mscorlib;
            }
            return null;
        }
       
        private Dictionary<string, string> _Standard_Type_LibName = null;
        private Dictionary<string, string> GetStandardTypeLibName()
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
                    if (DCUtils.IsSystemAsseblyName(asmName))
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
                foreach (var cls in this.GetAllClassesUseCache().Values)
                {
                    if (_Standard_Type_LibName.ContainsKey(cls.Name))
                    {
                        _Standard_Type_LibName[cls.Name] = string.Empty;
                    }
                }
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
                if (result != null && result.Length > 0)
                {
                    result = "[" + result + "]" + typeName;
                }
                else
                {
                    result = typeName;
                }
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
            var h3 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.DocumentFixDomState);
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
            SelfPerformanceCounterForTest.Leave(h3);
        }
         
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

        private Dictionary<DCILFieldReference, DCILFieldReference> _Cache_FieldReference 
            = new Dictionary<DCILFieldReference, DCILFieldReference>();
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
                if (code.OperCodeValue == DCILOpCodeValue.Call
                    || code.OperCodeValue == DCILOpCodeValue.Callvirt)
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

        internal Dictionary<DCILInvokeMethodInfo, DCILInvokeMethodInfo> _CachedInvokeMethods = null;

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
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.MergeDocuments);
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
            int nameMaxLength = 0;
            foreach( var doc in documents )
            {
                var n = Path.GetFileName(doc.AssemblyFileName);
                nameMaxLength = Math.Max(nameMaxLength, System.Text.Encoding.UTF8.GetByteCount(n));
            }
            var sysClassNames = new HashSet<string>(new string[] {
                "Microsoft.CodeAnalysis.EmbeddedAttribute",
                "System.Runtime.CompilerServices.NullableAttribute",
                "System.Runtime.CompilerServices.NullableContextAttribute",
                "System.Runtime.CompilerServices.RefSafetyRulesAttribute"
            });
            nameMaxLength += 2;
            int renameCount = 1;
            for (int iCount = 0 ; iCount < documents.Count; iCount++)
            {
                var document = documents[iCount];
                string msg = "";
                if( document.FileSize > 0 )
                {
                    msg = msg + DCUtils.FormatByteSize(document.FileSize);
                }
                if(document.CommentXmlDoc != null )
                {
                    msg = msg + " with '" + document.Name + ".XML'";
                }
                if(msg.Length > 0 )
                {
                    msg = "(" + msg +")";
                    int len2 = Encoding.UTF8.GetByteCount(Path.GetFileName(document.AssemblyFileName));
                    msg = new string(' ', nameMaxLength - len2) + msg ;
                }
                msg = "       + " + Path.GetFileName(document.AssemblyFileName) + msg ;
                if( mainDoc._MergedAssemblyNames == null )
                {
                    mainDoc._MergedAssemblyNames = new List<string>();
                }
                mainDoc._MergedAssemblyNames.Add(Path.GetFileName(document.AssemblyFileName));

                MyConsole.Instance.WriteLine( msg );
                if( iCount == 0 )
                {
                    continue;
                }
                mainDoc.AddPreserviceTypeNames(document.PrerviceTypeNames);
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
                        //if (cls.CustomAttributes != null && cls.CustomAttributes.Count > 0)
                        //{
                        //    bool isImportType = false;
                        //    foreach (var item in cls.CustomAttributes)
                        //    {
                        //        if (item.AttributeTypeName == "System.Runtime.CompilerServices.NativeCppClassAttribute")
                        //        {
                        //            isImportType = true;
                        //            break;
                        //        }
                        //    }
                        //    if (isImportType)
                        //    {
                        //        continue;
                        //    }
                        //}
                        bool hasContent = (cls.ChildNodes != null && cls.ChildNodes.Count > 0)
                            || (cls.NestedClasses != null && cls.NestedClasses.Count > 0);
                        if (hasContent)
                        {
                            foreach (var oldCls in mainDoc.Classes)
                            {
                                if (oldCls.Name == cls.Name)
                                {
                                    bool oldHasContent = (oldCls.ChildNodes != null && oldCls.ChildNodes.Count > 0) 
                                        || (oldCls.NestedClasses != null && oldCls.NestedClasses.Count > 0);
                                    if (oldHasContent)
                                    {
                                        // 两个类型都有内容，则改名添加
                                        mainDoc.Classes.Add(cls);
                                        if (sysClassNames.Contains(cls.Name) == false)
                                        {
                                            ConsoleReportError("       [Warring]Rename and add duplicate class name : "
                                                + document.Name + " # " + cls.Name);
                                        }
                                        if (cls._Name[cls._Name.Length - 1] == '\'')
                                        {
                                            cls._Name = cls._Name.Substring(0, cls._Name.Length - 1) + "_" + Convert.ToString(renameCount++) + "'";
                                        }
                                        else
                                        {
                                            cls._Name = cls.Name + "_" + Convert.ToString(renameCount++);
                                        }
                                    }
                                    else
                                    {
                                        // 旧类型没有内容，则覆盖
                                        mainDoc.Classes.Remove(oldCls);
                                        mainDoc.Classes.Add(cls);
                                        if (sysClassNames.Contains(cls.Name) == false)
                                        {
                                            ConsoleReportError("       [Warring]Overwrite duplicate class name : "
                                                + document.Name + " # " + cls.Name);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // 新类型无任何内容，则不添加
                            ConsoleReportError("       [Warring]Ignore duplicate class name : "
                                + document.Name + " # " + cls.Name);
                        }
                        continue;
                    }
                    mainDoc.Classes.Add(cls);
                    if( mainDoc.LibraryNames.ContainsKey( cls.Name ))
                    {
                        mainDoc.LibraryNames.Remove(cls.Name);
                    }
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
            MyConsole.Instance.WriteLine("       Merge " + Convert.ToString(documents.Count - 1)
                + " assembly files , add " + clsesCount + " classes, span "
                + Math.Abs(Environment.TickCount - tick) + " milliseconds.");
            SelfPerformanceCounterForTest.Leave(h4);
            return mainDoc;
        }
        /// <summary>
        /// 合并的其他程序集的名称
        /// </summary>
        public List<string> _MergedAssemblyNames = null;
        /// <summary>
        /// 是否合并了其他程序集文档
        /// </summary>
        public bool HasMergeDocuments
        {
            get
            {
                return this._MergedAssemblyNames != null && this._MergedAssemblyNames.Count > 0;
            }
        }
         
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
            MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
            MyConsole.Instance.BackgroundColor = ConsoleColor.White;
            MyConsole.Instance.WriteLine(msg);
            MyConsole.Instance.ResetColor();
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
        /// <summary>
        /// 根据名称获得类型对象
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="nestedSplitChar">套嵌类型名称分隔字符</param>
        /// <returns>获得的类型对象</returns>
        public DCILClass GetClassByName(string typeName, char nestedSplitChar = '/')
        {
            if (typeName == null || typeName.Length == 0)
            {
                return null;
            }
            if ( nestedSplitChar == '/' || typeName.IndexOf(nestedSplitChar) < 0)
            {
                DCILClass result = null;
                if (this.GetAllClassesUseCache().TryGetValue(typeName, out result))
                {
                    return result;
                }
            }
            else
            {
                var tns = typeName.Split(nestedSplitChar);
                DCILClass rootClass = null;
                if (this.GetAllClassesUseCache().TryGetValue(tns[0], out rootClass))
                {
                    DCILClass curCls = rootClass;
                    for (int iCount = 1; iCount < tns.Length; iCount++)
                    {
                        if (curCls.NestedClasses == null)
                        {
                            break;
                        }
                        foreach (var cls2 in curCls.NestedClasses)
                        {
                            if (cls2.Name == tns[iCount])
                            {
                                if (iCount == tns.Length - 1)
                                {
                                    return cls2;
                                }
                                curCls = cls2;
                                break;
                            }
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获得所有包含运行指令的方法对象
        /// </summary>
        /// <returns></returns>
        public List<DCILMethod> GetAllMethodHasOperCodes()
        {
            var list = new List<DCILMethod>();
            foreach( var cls in this.Classes )
            {
                InnerGetAllMethodHasOperCodes(cls, list);
            }
            return list;
        }
        private void InnerGetAllMethodHasOperCodes(DCILClass cls, List<DCILMethod> list)
        {
            if (cls.IsImport == false && cls.IsInterface == false)
            {
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod)
                    {
                        var m = (DCILMethod)item;
                        if (m.OperCodes != null && m.OperCodes.Count > 0)
                        {
                            list.Add(m);
                        }
                    }
                }
                if( cls.NestedClasses != null && cls.NestedClasses.Count > 0 )
                {
                    foreach ( var cls2 in cls.NestedClasses)
                    {
                        InnerGetAllMethodHasOperCodes(cls2, list);
                    }
                }
            }
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
        public HashSet<string> PrerviceTypeNames = null;
        public void AddPreserviceTypeNames(HashSet<string> names )
        {
            if(names != null && names.Count > 0 )
            {
                if( this.PrerviceTypeNames == null )
                {
                    this.PrerviceTypeNames = new HashSet<string>();
                }
                foreach(var item in names )
                {
                    this.PrerviceTypeNames.Add(item);
                }
            }
        }
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
            reader.UpdateFieldReferenceData(this);
            this.AddPreserviceTypeNames(reader.PreserveTypeNames);
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
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.WriteIL);
            writer.WriteObjects2(this.Assemblies);
            writer.WriteObjects2(this.Resouces.Values);
            writer.WriteObjects2(this.Modules);
            writer.WriteObjects2(this.CustomAttributes);

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

            //System.Diagnostics.Debugger.Break();
            foreach ( var cls in this.Classes)
            {
                cls.InnerWriteTo(writer, true);
            }
            writer.WriteObjects2(this.Classes);
            writer.WriteObjects2(this.ILDatas);
            SelfPerformanceCounterForTest.Leave(h4);
        }

        public void WriteToFile(string fileName, Encoding encod)
        {
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.WriteILFile);
            using (var writer = new System.IO.StreamWriter(fileName, false, encod))
            {
                var w2 = new DCILWriter(writer);
                this.WriteTo(w2);
            }
            this.WriteResourceFile(Path.GetDirectoryName(fileName));
            SelfPerformanceCounterForTest.Leave(h4);
        }

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

        //public string[] SourceLines = null;
        public string RootPath = null;
        public string AssemblyFileName = null;
        public string LibName_mscorlib = "mscorlib";
        public int ModifiedCount = 0;
        public List<string> ReferenceAssemblies = new List<string>();
        public byte[] Win32ResData = null;
        public SortedDictionary<string, DCILMResource> Resouces = new SortedDictionary<string, DCILMResource>();

        public void CleanFieldValue()
        {
            this.Resouces = null;
            this.Classes = null;
            this.ILDatas = null;
            this.Assemblies = null;
            this.Modules = null;
        }
        public override void Dispose()
        {
            base.Dispose();
            if (this.Classes != null)
            {
                foreach (var cls in this.Classes)
                {
                    cls.Dispose();
                }
                this.Classes.Clear();
                this.Classes = null;
            }
            if(this._AllClasses != null )
            {
                this._AllClasses.Clear();
                this._AllClasses = null;
            }
            if( this._AllCustomAttributes != null )
            {
                this._AllCustomAttributes.Clear();
                this._AllCustomAttributes = null;
            }
            if( this._CachedInvokeMethods != null )
            {
                foreach( var item in this._CachedInvokeMethods)
                {
                    item.Value.Dispose();
                }
                this._CachedInvokeMethods.Clear();
                this._CachedInvokeMethods = null;
            }
            if( this._CachedTypes != null )
            {
                foreach( var item in this._CachedTypes)
                {
                    item.Value.Dispose();
                }
                this._CachedTypes.Clear();
                this._CachedTypes = null;
            }
            if( this._Cache_FieldReference != null )
            {
                foreach( var item in this._Cache_FieldReference)
                {
                    item.Value.Dispose();
                }
                this._Cache_FieldReference.Clear();
                this._Cache_FieldReference = null;
            }
            if( this._Cache_TypeNameWithLibraryName != null )
            {
                this._Cache_TypeNameWithLibraryName.Clear();
                this._Cache_TypeNameWithLibraryName = null;
            }
            this.FileName = null;
            this.RootPath = null;
            if(this.Assemblies != null )
            {
                foreach( var item in this.Assemblies )
                {
                    item.Dispose();
                }
                this.Assemblies.Clear();
                this.Assemblies = null;
            }
            if(this.Resouces != null )
            {
                foreach( var item in this.Resouces )
                {
                    item.Value.Dispose();
                }
                this.Resouces.Clear();
                this.Resouces = null;
            }
            this.RuntimeVersion = null;
            this.Win32ResData = null;
            if(this.ReferenceAssemblies != null )
            {
                this.ReferenceAssemblies.Clear();
                this.ReferenceAssemblies = null;
            }
            this.Content_DepsJson = null;
            this.CommentXmlDoc = null;
            this.CustomInstructions = null;
            this.LibName_mscorlib = null;
            if(this.LibraryNames != null )
            {
                this.LibraryNames.Clear();
                this.LibraryNames = null;
            }
            if( this.Modules != null )
            {
                foreach( var item in this.Modules )
                {
                    item.Dispose();
                }
                this.Modules.Clear();
                this.Modules = null;
            }
            if( this.UnknowObjects != null )
            {
                foreach( var item in this.UnknowObjects )
                {
                    item.Dispose();
                }
                this.UnknowObjects.Clear();
                this.UnknowObjects = null;
            }
            if (this.ILDatas != null)
            {
                foreach( var item in this.ILDatas )
                {
                    item.Dispose();
                }
                this.ILDatas.Clear();
                this.ILDatas = null;
            }
            
        }

        public bool _IsDotNetCoreAssembly = false;

    }

}
