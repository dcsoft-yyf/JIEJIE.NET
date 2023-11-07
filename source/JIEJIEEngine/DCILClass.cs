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

namespace JIEJIE
{
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

        /// <summary>
        /// 未找到原生态的基础类型或者接口
        /// </summary>
        public bool MissNativeBaseTypeOrInterface = false;
        /// <summary>
        /// 运行时的是否存在未找到原生态基础类型或者接口
        /// </summary>
        /// <returns></returns>
        public bool RuntimeMissNativeBaseTypeOrInterface()
        {
            if( this.MissNativeBaseTypeOrInterface )
            {
                return true;
            }
            if( this.BaseType?.LocalClass != null 
                && this.BaseType.LocalClass.RuntimeMissNativeBaseTypeOrInterface())
            {
                return true;
            }
            if( this.ImplementsInterfaces != null )
            {
                foreach( var item in this.ImplementsInterfaces)
                {
                    if(item.LocalClass != null && item.LocalClass.RuntimeMissNativeBaseTypeOrInterface())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private DCILTypeReference _LocalTypeRef = null;
        public DCILTypeReference GetLocalTypeReference()
        {
            if(this._LocalTypeRef == null )
            {
                this._LocalTypeRef = new DCILTypeReference(this);
            }
            return this._LocalTypeRef;
        }
        /// <summary>
        /// 运行时使用的开关
        /// </summary>
        public JieJieSwitchs RuntimeSwitchs = null;
        public override void Dispose()
        {
            base.Dispose();
            if( this.NestedClasses != null )
            {
                foreach( var item in this.NestedClasses)
                {
                    item.Dispose();
                }
                this.NestedClasses.Clear();
                this.NestedClasses = null;
            }
            if(this.GenericParamters != null )
            {
                foreach( var item in this.GenericParamters )
                {
                    item.Dispose(); 
                }
            }
            this.Method_Cctor = null;
            this.NativeType = null;
            this.RuntimeSwitchs = null;
            if(this.ImplementsInterfaces != null )
            {
                this.ImplementsInterfaces.Clear();
                this.ImplementsInterfaces = null;
            }
            if( this.FieldLineIndexs != null )
            {
                this.FieldLineIndexs.Clear();
                this.FieldLineIndexs = null;
            }
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Class;
            }
        }

        /// <summary>
        /// 系统内部自动产生的
        /// </summary>
        public bool InnerGenerate = false;
        /// <summary>
        /// 存在系统内部自动生成的方法
        /// </summary>
        public bool HasInnerGenerateMethod = false;

        private static readonly Dictionary<Type, DCILClass> _NativeClasses = new Dictionary<Type, DCILClass>();

        //public DCILClass(Type nativeType, DCILDocument document) : base(nativeType)
        //{
        //    this.IsEnum = nativeType.IsEnum;
        //    this.IsValueType = nativeType.IsValueType;
        //    this.IsMulticastDelegate = typeof(System.MulticastDelegate).IsAssignableFrom(nativeType);
        //    this.IsInterface = nativeType.IsInterface;
        //    this.IsImport = nativeType.IsImport;
        //    this.ChildNodes = new DCILObjectList();
        //    this.NativeType = nativeType;
        //    this._Name = DCUtils.GetFullName(nativeType);
        //    if (nativeType.IsGenericType)
        //    {
        //        var gps = nativeType.GetGenericArguments();
        //        this.GenericParamters = new DCILGenericParamterList(gps.Length);
        //        foreach (var item in gps)
        //        {
        //            var p = new DCILGenericParamter();
        //            p.Name = DCUtils.GetFullName(item);
        //            this.GenericParamters.Add(p);
        //        }
        //    }
        //    if (nativeType.BaseType != null)
        //    {
        //        this.BaseType = DCILTypeReference.CreateByNativeType(nativeType.BaseType, document);
        //    }
        //    var its = nativeType.GetInterfaces();
        //    if (its != null && its.Length > 0)
        //    {
        //        this.ImplementsInterfaces = new List<DCILTypeReference>(its.Length);
        //        foreach (var item in its)
        //        {
        //            this.ImplementsInterfaces.Add(DCILTypeReference.CreateByNativeType(item, document));
        //        }
        //    }

        //}
        public Type NativeType = null;

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
            reader.UpdateFieldReferenceData(document);
            this.InnerGenerate = true;
        }

        /// <summary>
        /// 对象的静态构造函数 
        /// </summary>
        public DCILMethod Method_Cctor = null;

        public override void Load(DCILReader reader)
        {
            LoadHeader(reader);
            LoadContent(reader);
        }
        private static readonly DCILProperty[] _EmptyArray = new DCILProperty[0];

        private static readonly DCILMethod[] _EmptyArray2 = new DCILMethod[0];


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

        }

        public void LoadHeader(DCILReader reader)
        {
            var h4 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.LoadClassHeader);
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
                    this.AddStyle(strWord , reader );
                    strWord = reader.ReadWord();
                    this.AddStyle(strWord , reader );
                    continue;
                }
                else if (_ClassAttributeNames.Contains(strWord))
                {
                    this.AddStyle(strWord , reader );
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
                this.IsValueType = this.BaseType.Name == "System.ValueType";
                this.IsMulticastDelegate = this.BaseType.Name == "System.MulticastDelegate";
            }
            SelfPerformanceCounterForTest.Leave(h4);
        }

        // public IDGenerator idGenForMember = null;
        public int StructLayoutPack = int.MinValue;
        public int StructLayoutSize = int.MinValue;

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
            var h3 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.LoadClassContent);
            DCInterfaceimpl itimpl = null;
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == "}")
                {
                    break;
                }
                switch (strWord)
                {
                    case DCInterfaceimpl.TagName:// ".interfaceimpl":
                        {
                            itimpl = new DCInterfaceimpl( reader );
                        }
                        break;
                    case DCILCustomAttribute.TagName_custom:
                        {
                            var item = base.ReadCustomAttribute(reader);
                            if(itimpl != null )
                            {
                                item.PrefixObject = itimpl;
                            }
                            itimpl = null;
                        }
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
                            if(m.Name == ".cctor")
                            {
                                this.Method_Cctor = m;
                            }
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
                    case DCILData.TagName_Data:
                        var d3 = new DCILData(reader);
                        if (reader.Document != null)
                        {
                            reader.Document.ILDatas.Add(d3);
                        }
                        else
                        {
                            this.ChildNodes.Add(d3);
                        }
                        break;
                    case ".pack":
                        {
                            var strValue = reader.ReadWord();
                            reader.MoveNextLine();
                            if(Int32.TryParse(strValue, out this.StructLayoutPack)== false )
                            {
                                this.StructLayoutPack = int.MinValue;
                            }
                        }
                        break;
                    case ".size":
                        {
                            var strValue = reader.ReadWord();
                            reader.MoveNextLine();
                            if(Int32.TryParse(strValue, out this.StructLayoutSize) == false )
                            {
                                this.StructLayoutSize = int.MinValue;
                            }
                        }
                        break;
                    default:
                        this.ChildNodes.Add(new DCILUnknowObject(strWord, reader));
                        break;
                }
            }
            if (this.IsEnum)
            {
                foreach (var item in this.ChildNodes)
                {
                    if (item.Name == "value__")
                    {
                        var field = (DCILField)item;
                        this.EnumByteSize = GetIntegerByteSize(field.ValueType?.NativeType);
                        break;
                    }
                }
            }
            SelfPerformanceCounterForTest.Leave(h3);
        }
        internal static int GetIntegerByteSize(Type t)
        {
            if (t == typeof(byte) || t == typeof(sbyte))
            {
                return 1;
            }
            if (t == typeof(short) || t == typeof(ushort))
            {
                return 2;
            }
            if (t == typeof(int) || t == typeof(uint))
            {
                return 4;
            }
            if (t == typeof(long) || t == typeof(ulong))
            {
                return 8;
            }
            return 4;
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
                return GetNameWithNested('/');
            }
        }
        
        private string GetRuntimeName(bool useOldName )
        {
            if (useOldName && this.OldName != null && this.OldName.Length > 0)
            {
                return this.OldName;
            }
            else
            {
                return this._Name;
            }
        }

        public string GetNameWithNested(char nestedSplitChar , bool useOldName = false )
        {
            if (this.Parent == null)
            {
                return this.GetRuntimeName(useOldName);
            }
            else
            {
                var str = new StringBuilder();
                str.Append(this.GetRuntimeName(useOldName));
                var p = this.Parent;
                while (p != null)
                {
                    str.Insert(0, nestedSplitChar);
                    string pName = ((DCILClass)p).GetRuntimeName(useOldName);
                    //if (pName == null || pName.Length == 0)
                    //{

                    //}
                    str.Insert(0, pName);
                    p = p.Parent;
                }
                return str.ToString();
            }

        }
        public override void WriteTo(DCILWriter writer)
        {
            InnerWriteTo(writer, false);
        }

        public void InnerWriteTo( DCILWriter writer , bool declearationOnly )
        { 
            if(this.RenameState == DCILRenameState.Renamed )
            {
                writer.WriteLine("// " + this.GetOldName());
            }
            writer.Write(".class ");
            base.WriteStyles(writer);
            //if (this._Name == "__DC20210205.__jiejienet_sm")
            //{

            //}
            writer.Write(" " + this._Name);
            this.GenericParamters?.WriteTo(writer);

            if (this.BaseType != null)
            {
                writer.Write(" extends ");
                this.BaseType.WriteTo(writer, true);
                //if (this.BaseType.HasLibraryName)
                //{
                //    this.BaseType.WriteTo(writer, true );
                //}
                //else
                //{
                //    this.BaseType.WriteTo(writer, false);
                //}
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
            if (declearationOnly == false)
            {
                base.WriteCustomAttributes(writer);
            }
            if (this.NestedClasses != null && this.NestedClasses.Count > 0)
            {
                foreach (var item in this.NestedClasses)
                {
                    writer.WriteLine();
                    item.InnerWriteTo(writer , declearationOnly);
                }
            }
            if (declearationOnly == false)
            {
                if( this.StructLayoutPack >= 0 )
                {
                    writer.WriteLine(Environment.NewLine + "   .pack " + this.StructLayoutPack);
                }
                if(this.StructLayoutSize >= 0 )
                {
                    writer.WriteLine(Environment.NewLine + "   .size " + this.StructLayoutSize);
                }
                writer.WriteObjects(this.ChildNodes);
            }
            writer.WriteEndGroup();
        }

        public List<DCILClass> NestedClasses = null;
        /// <summary>
        /// 获得指定名称的内嵌类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DCILClass GetNestedClass(string name )
        {
            if(this.NestedClasses != null )
            {
                foreach( var item in this.NestedClasses )
                {
                    if(item.Name == name )
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        public DCILObject GetChildNodeByName(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentNullException("name");
            }
            if( this.ChildNodes != null )
            {
                foreach( var item in this.ChildNodes )
                {
                    if( item.Name == name )
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        public List<DCILTypeReference> ImplementsInterfaces = null;
        public List<int> FieldLineIndexs = new List<int>();
        public bool Modified = false;
        public DCILTypeReference BaseType = null;
        public bool IsMulticastDelegate = false;
        public bool IsEnum = false;
        public bool IsValueType = false;

        /// <summary>
        /// 获得枚举值数据类型
        /// </summary>
        /// <returns></returns>
        public DCILTypeReference GetEnumItemValueType()
        {
            if (this.IsEnum)
            {
                foreach (var f in this.ChildNodes)
                {
                    if (f.Name == "value__")
                    {
                        return ((DCILField)f).ValueType;
                    }
                }
            }
            return null;
        }

        public int EnumByteSize = 4;
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
        /// <summary>
        /// 是否为访问程序集资源的包装类型
        /// </summary>
        /// <returns></returns>
        public bool IsResoucePackage()
        {
            if( this.Name == "DCSoft.DCFunctionNameStrings")
            {

            }
            if (this.BaseType?.Name == "System.Object")
            {
                bool bolResult = true;
                foreach (var item in this.ChildNodes)
                {
                    if (item.Name == "resourceMan")
                    {
                        var f = item as DCILField;
                        if (f == null || f.ValueType?.Name != "System.Resources.ResourceManager")
                        {
                            bolResult = false;
                            break;
                        }
                    }
                    else if (item.Name == "resourceCulture")
                    {
                        var f = item as DCILField;
                        if (f == null || f.ValueType?.Name != "System.Globalization.CultureInfo")
                        {
                            bolResult = false;
                            break;
                        }
                    }
                    else if (item.Name == "ResourceManager")
                    {
                        var p = item as DCILProperty;
                        if (p == null || p.ValueType?.Name != "System.Resources.ResourceManager")
                        {
                            bolResult = false;
                            break;
                        }
                    }
                    else if (item is DCILMethod && item.Name != null)
                    {
                        if (DCILMethod.IsCtorOrCctor(item.Name) || item.Name == "set_Culture")
                        {
                            continue;
                        }
                        if (item.Name.StartsWith("get_", StringComparison.Ordinal) == false)
                        {
                            bolResult = false;
                            break;
                        }
                    }
                }
                return bolResult;
            }
            return false;
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


}
