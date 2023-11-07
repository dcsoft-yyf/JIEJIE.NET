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
    /// <summary>
    /// see"Partition II Metadata.doc",topic 7.1
    /// </summary>
    internal class DCILTypeReference : IEqualsValue<DCILTypeReference> , IDisposable
    {
        internal static void Test()
        {
            var ttt = DCILTypeReference.LoadText("class DCSoft.Writer.Dom.XTextTableCellElement[0...,0...]");
            var t = DCILTypeReference.LoadText("int8");
            var t56 = DCILTypeReference.LoadText("class [mscorlib]System.Collections.Generic.List`1<class OpenSource.ICSharpCode.TextEditor.Document.TextMarker>&");
            var t99 = DCILTypeReference.LoadText("int8* [ ][ ]");
            var t23 = DCILTypeReference.LoadText("valuetype [mscorlib]System.Decimal[][]");
            var t2 = DCILTypeReference.LoadText("valuetype [mscorlib]System.Decimal::ToString()");
            var t3 = DCILTypeReference.LoadText(@"class [mscorlib]System.Collections.Generic.Dictionary`2<string, 
                    class [mscorlib]System.Collections.Generic.List`1<class ConsoleApp4.aaaa>>::ToString()");
            var t4 = DCILTypeReference.LoadText("class [mscorlib]System.Collections.Generic.Dictionary`2<string, class ConsoleApp4.aaaa>");
            var t46 = DCILTypeReference.LoadText("class bbbb[][]");
            var t5 = DCILTypeReference.LoadText("zzz");
            var t52 = DCILTypeReference.LoadText("zzz[]");
            var t53 = DCILTypeReference.LoadText("zzz*");
            var t6 = DCILTypeReference.LoadText("!T");
            var t9 = DCILTypeReference.LoadText("!!TT");
            var t999 = DCILTypeReference.LoadText("class [mscorlib]System.Collections.Generic.IComparer`1<valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,float64>>");
        }
        private static readonly Dictionary<string, DCILTypeReference> _PrimitiveTypes = null;
        private static readonly HashSet<string> PrimitiveTypeNames = null;
        public static readonly DCILTypeReference Type_Void = null;
        public static readonly DCILTypeReference Type_String = null;
        public static readonly DCILTypeReference Type_Object = null;
        public static readonly DCILTypeReference Type_Int32 = null;
        public static readonly DCILTypeReference Type_Boolean = null;
        public static readonly DCILTypeReference Type_Char = null;
        public static readonly DCILTypeReference Type_Byte = null;
        static DCILTypeReference()
        {
            PrimitiveTypeNames = new HashSet<string>();
            PrimitiveTypeNames.Add("uint8");
            PrimitiveTypeNames.Add("int8");
            PrimitiveTypeNames.Add("char");
            PrimitiveTypeNames.Add("bool");
            PrimitiveTypeNames.Add("int16");
            PrimitiveTypeNames.Add("uint16");
            PrimitiveTypeNames.Add("int32");
            PrimitiveTypeNames.Add("uint32");
            PrimitiveTypeNames.Add("int64");
            PrimitiveTypeNames.Add("uint64");
            PrimitiveTypeNames.Add("float32");
            PrimitiveTypeNames.Add("float64");
            PrimitiveTypeNames.Add("valuetype");
            PrimitiveTypeNames.Add("string");
            PrimitiveTypeNames.Add("native");
            PrimitiveTypeNames.Add("class");
            PrimitiveTypeNames.Add("object");
            PrimitiveTypeNames.Add("void");
            PrimitiveTypeNames.Add("unsigned");
            PrimitiveTypeNames.Add("lpstr");
            PrimitiveTypeNames.Add("lpwstr");
            PrimitiveTypeNames.Add("typedref");

            _PrimitiveTypes = new Dictionary<string, DCILTypeReference>();
            AddPrimitiveType("uint8", typeof(byte))._NameInCSharp = "byte";
            AddPrimitiveType("int8", typeof(sbyte))._NameInCSharp ="sbyte";
            AddPrimitiveType("char", typeof(char))._NameInCSharp = "char";
            AddPrimitiveType("bool", typeof(bool))._NameInCSharp = "bool";
            AddPrimitiveType("int16", typeof(short))._NameInCSharp = "short";
            AddPrimitiveType("uint16", typeof(ushort))._NameInCSharp = "ushort";
            AddPrimitiveType("int32", typeof(int))._NameInCSharp = "int";
            AddPrimitiveType("uint32", typeof(uint))._NameInCSharp = "uint";
            AddPrimitiveType("int64", typeof(long))._NameInCSharp = "long";
            AddPrimitiveType("uint64", typeof(ulong))._NameInCSharp = "ulong";
            AddPrimitiveType("float32", typeof(float))._NameInCSharp = "float";
            AddPrimitiveType("float64", typeof(double))._NameInCSharp = "double";
            AddPrimitiveType("string", typeof(string))._NameInCSharp = "string";
            AddPrimitiveType("object", typeof(object))._NameInCSharp = "object";
            AddPrimitiveType("void", typeof(void))._NameInCSharp = "void";
            AddPrimitiveType("lpwstr", typeof(string))._NameInCSharp = "string";
            AddPrimitiveType("typedref", typeof(System.TypedReference))._NameInCSharp = "System.TypedReference";

            Type_Void = _PrimitiveTypes["void"];
            Type_String = _PrimitiveTypes["string"];
            Type_Object = _PrimitiveTypes["object"];
            Type_Int32 = _PrimitiveTypes["int32"];
            Type_Boolean = _PrimitiveTypes["bool"];
            Type_Char = _PrimitiveTypes["char"];
            Type_Byte = _PrimitiveTypes["uint8"];
            _Cache_CreateByNativeType[typeof(string)] = Type_String;

        }
        
        public List_modopt_modreq _modopt_modreqs = null;
        public string MarshalAs = null;
        public void Dispose()
        {
            if(this.Mode == DCILTypeMode.Primitive )
            {
                return;
            }
            if(this.GenericParamters != null )
            {
                this.GenericParamters.Clear();
                this.GenericParamters = null;
            }
            this.LibraryName = null;
            this.LocalClass = null;
            this.Name = null;
            this._ArrayAndPointerSettings = null;
            this._NameInCSharp = null;
            this._NativeType = null;
        }
        public bool EqualsValue(DCILTypeReference type, DCILGenericParamterList gps , bool checkMarshalAndMod = true )
        {
            if (type == null)
            {
                return false;
            }
            if (type == this)
            {
                return true;
            }
            if (type.Mode == DCILTypeMode.Primitive || this.Mode == DCILTypeMode.Primitive)
            {
                return this.EqualsValue(type , checkMarshalAndMod);
            }
            if (gps == null || gps.Count == 0)
            {
                return this.EqualsValue(type);
            }
            else
            {
                var type1 = this.Transform(gps);
                var type2 = type.Transform(gps);
                return type1.EqualsValue(type2);
            }
        }
        public DCILTypeReference Transform(DCILGenericParamterList gps)
        {
            if (gps == null || gps.Count == 0)
            {
                return this;
            }
            if (this.Mode == DCILTypeMode.GenericTypeInMethodDefine)
            {
                var item = gps.GetItem(this.Name, false);
                if (item != null && item.RuntimeType != null)
                {
                    if (this.ArrayAndPointerSettings != null && this.ArrayAndPointerSettings.Length > 0)
                    {
                        var result2 = item.RuntimeType.Clone();
                        result2.ArrayAndPointerSettings = this.ArrayAndPointerSettings;
                        return result2;
                    }
                    else
                    {
                        return item.RuntimeType;
                    }
                }
                return this;
            }
            if (this.Mode == DCILTypeMode.GenericTypeInTypeDefine)
            {
                var item = gps.GetItem(this.Name, true);
                if (item != null && item.RuntimeType != null)
                {
                    if (this.ArrayAndPointerSettings != null && this.ArrayAndPointerSettings.Length > 0)
                    {
                        var result2 = item.RuntimeType.Clone();
                        result2.ArrayAndPointerSettings = this.ArrayAndPointerSettings;
                        return result2;
                    }
                    else
                    {
                        return item.RuntimeType;
                    }
                }
                return this;
            }
            if (this.GenericParamters == null || this.GenericParamters.Count == 0)
            {
                return this;
            }
            else
            {
                DCILTypeReference result = (DCILTypeReference)this.MemberwiseClone();
                result.GenericParamters = new List<DCILTypeReference>();
                foreach (var item in this.GenericParamters)
                {
                    result.GenericParamters.Add(item.Transform(gps));
                }
                return result;
            }
        }

        private static DCILTypeReference AddPrimitiveType(string name, Type nativeType)
        {
            var t = new DCILTypeReference(name, DCILTypeMode.Primitive);
            t._NativeType = nativeType;
            _PrimitiveTypes[name] = t;
            _Cache_CreateByNativeType[nativeType] = t;
            return t;
        }
        private string _NameInCSharp = null;
        public string NameInCSharp
        {
            get
            {
                return this._NameInCSharp;
            }
        }
        public static DCILTypeReference GetPrimitiveType(string name)
        {
            DCILTypeReference result = null;
            if (_PrimitiveTypes.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }
        public static DCILTypeReference LoadText(string text)
        {
            var reader = new DCILReader(text, null);
            return Load(reader.ReadWord(), reader);
        }
        private static readonly Dictionary<Type, DCILTypeReference> _Cache_CreateByNativeType
            = new Dictionary<Type, DCILTypeReference>();
        public static DCILTypeReference CreateByNativeType(Type t, DCILDocument document)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            DCILTypeReference result = null;
            if (_Cache_CreateByNativeType.TryGetValue(t, out result) == false)
            {
                result = new DCILTypeReference(t, document);
                _Cache_CreateByNativeType[t] = result;
            }
            return result;
        }

        public DCILTypeReference ( DCILClass cls )
        {
            if( cls == null )
            {
                throw new ArgumentNullException("cls");
            }
            this.Name = cls.Name;
            this.Mode = DCILTypeMode.Class;
            this.LocalClass = cls;
        }

        public DCILTypeReference(Type nativeType, DCILDocument document)
        {
            if (nativeType == null)
            {
                throw new ArgumentNullException("nativeType");
            }
            this._NativeType = nativeType;
            if (nativeType == typeof(System.IntPtr))
            {
                this.Mode = DCILTypeMode.Native;
                this.Name = "int";
                return;
            }

            if (nativeType.IsPointer)
            {
                this.ArrayAndPointerSettings = "*";
            }
            if (nativeType.IsArray)
            {
                int rank = nativeType.GetArrayRank();
                for (int iCount = 0; iCount < rank; iCount++)
                {
                    this.ArrayAndPointerSettings = this.ArrayAndPointerSettings + "[]";
                }
                nativeType = nativeType.GetElementType();
            }
            else if (nativeType.HasElementType)
            {
                nativeType = nativeType.GetElementType();
                this.ArrayAndPointerSettings = this.ArrayAndPointerSettings + "&";
            }


            if (nativeType.IsValueType)
            {
                this.Mode = DCILTypeMode.ValueType;
            }
            else
            {
                this.Mode = DCILTypeMode.Class;
            }
            if (nativeType.IsGenericParameter)
            {
                this.Name = nativeType.Name;
                this.Mode = DCILTypeMode.GenericTypeInMethodDefine;
            }
            else if (nativeType.IsGenericType)
            {
                this.Name = DCUtils.GetFullName(nativeType);
                //this.Mode = DCILTypeMode.GenericTypeInTypeDefine;
            }
            else
            {
                this.Name = nativeType.FullName.Replace('+', '/');
            }

            this.LibraryName = document.GetLibraryName(this.Name);
            if (this.LibraryName == null)
            {
                this.LibraryName = nativeType.Assembly.GetName().Name;
            }

            if (nativeType.IsGenericType)
            {
                var gps = nativeType.GetGenericArguments();
                this.GenericParamters = new List<DCILTypeReference>(gps.Length);
                foreach (var gp in gps)
                {
                    var p = new DCILTypeReference(gp, document);
                    p.Mode = DCILTypeMode.GenericTypeInTypeDefine;
                    this.GenericParamters.Add(p);
                }
            }
        }

        public static DCILTypeReference Load(DCILReader reader)
        {
            int pos = reader.Position;
            var word = reader.ReadWord();
            if (IsStartWord(word))
            {
                return Load(word, reader);
            }
            else
            {
                reader.Position = pos;
                return Load(ClassMode_NotSpecify, reader);
            }
        }

        public static DCILTypeReference Load(string firstWord, DCILReader reader)
        {
            if (firstWord == "valuetype"
                || firstWord == "class"
                || firstWord == "native"
                || firstWord == ClassMode_NotSpecify
                || firstWord[0] == '!'
                || reader == null)
            {
                return new DCILTypeReference(firstWord, reader);
            }
            DCILTypeReference result = null;
            if (_PrimitiveTypes.TryGetValue(firstWord, out result))
            {
                if (reader != null && DCILTypeReference.HasExtInfo( reader ))
                {
                    // 可能为数组或者指针类型
                    var result2 = (DCILTypeReference)result.MemberwiseClone();
                    if(result2.ReadExtInfo( reader ))
                    {
                        return result2;
                    }
                    else
                    {
                        return result;
                    }
                }
                return result;
            }
            result = new DCILTypeReference(firstWord);
            result.ReadExtInfo(reader);
            return result;
        }
        public static bool HasExtInfo( DCILReader reader )
        {
            var c = reader.PeekContentChar();
            if ("*&[]".IndexOf(c) >= 0)
            {
                return true;
            }
            string strWord = reader.PeekWord();
            if( strWord == "marshal" || List_modopt_modreq.IsStartWord( strWord ))
            {
                return true;
            }
            return false;
        }
        public bool ReadExtInfo( DCILReader reader )
        {
            if( reader == null )
            {
                return false;
            }
            bool result = false;
            if("*&[]".IndexOf( reader.PeekContentChar()) >= 0 )
            {
                result = this.ReadArrayAndPointerSettings(reader);
            }
            while (true)
            {
                string strWord = reader.PeekWord();
                if (strWord == "marshal")
                {
                    reader.ReadWord();
                    this.MarshalAs = reader.ReadStyleExtValue();
                    result = true;
                }
                else if( List_modopt_modreq.IsStartWord( strWord ))
                {
                    if( this._modopt_modreqs == null )
                    {
                        this._modopt_modreqs = new List_modopt_modreq();
                    }
                    this._modopt_modreqs.Read(reader.ReadWord(), reader);
                    result = true;
                }
                else
                {
                    break;
                }
            }
            return result;
        }
        public static bool IsStartWord(string word)
        {
            if (PrimitiveTypeNames.Contains(word))
            {
                return true;
            }
            if (word[0] == '!')
            {
                return true;
            }
            return false;
        }
        public static void ClearGlobalBuffer()
        {
            foreach (var item in _Cache_CreateByNativeType)
            {
                if (item.Value.IsPrimitive == false)
                {
                    item.Value.Dispose();
                }
            }
            _Cache_CreateByNativeType.Clear();
        }

        private static readonly Dictionary<string, Type> _NativeTypes = new Dictionary<string, Type>();
        private static System.Reflection.Assembly[] _LocalAssemblies = null;
        public DCILTypeReference Clone()
        {
            return (DCILTypeReference)this.MemberwiseClone();
        }
#if DOTNETCORE
        
        internal static System.Runtime.Loader.AssemblyLoadContext _AsmLoader = null;

#endif
        private static HashSet<string> _MissLibNames = new HashSet<string>();
        public static void ClearCacheNativeTypes()
        {
            _NativeTypes.Clear();
        }
        public Type SearchNativeType(string searchPath = null)
        {
            if (this._NativeType != null)
            {
                return this._NativeType;
            }
            Type result = null;
            if (_NativeTypes.TryGetValue(this.NameWithLibraryName, out result) == false)
            {
                if (this.LibraryName == "System.Windows.Forms")
                {

                }
                if (this.LibraryName != null
                    && this.LibraryName.Length > 0
                    && _MissLibNames.Contains(this.LibraryName) == false)
                {
                    bool findAsm = false;
                    if (_LocalAssemblies == null)
                    {
                        _LocalAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    }
                    foreach (var asm in _LocalAssemblies)
                    {
                        if (asm.GetName().Name == this.LibraryName)
                        {
                            findAsm = true;
                            result = asm.GetType(this.Name, false, false);
#if DOTNETCORE
                            if( result == null )
                            {
                                var fts = asm.GetForwardedTypes();
                                if(fts != null && fts.Length > 0 )
                                {
                                    foreach( var ft in fts )
                                    {
                                        if( ft.FullName == this.Name )
                                        {
                                            result = ft;
                                            break;
                                        }
                                    }
                                }
                            }
#endif
                            break;
                        }
                    }
                    if (findAsm == false)
                    {
                        var path = Path.GetDirectoryName(typeof(string).Assembly.Location);
                        var asmFileName = Path.Combine(path, this.LibraryName + ".dll");
                        if (File.Exists(asmFileName) == false)
                        {
                            asmFileName = DCUtils.SearchFileDeeply(path, this.LibraryName + ".dll");
                        }
                        if( asmFileName == null )
                        { 
                            if (searchPath != null && searchPath.Length > 0)
                            {
                                asmFileName = Path.Combine(searchPath, this.LibraryName + ".dll");
                            }
                        }
                        if (File.Exists(asmFileName) == false)
                        {
                            asmFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.LibraryName + ".dll");
                        }
#if DOTNETCORE
                        if( File.Exists( asmFileName ) == false )
                        {
                            var rootPath = Path.GetDirectoryName( Path.GetDirectoryName(Path.GetDirectoryName(typeof(string).Assembly.Location)));
                            asmFileName = DCUtils.SearchFileDeeply(rootPath, this.LibraryName + ".dll");
                        }
#endif
                        if (File.Exists(asmFileName))
                        {
                            try
                            {
                                System.Reflection.Assembly asm = null;
#if DOTNETCORE
                                if (_AsmLoader != null)
                                {
                                    asm = _AsmLoader.LoadFromAssemblyPath(asmFileName);
                                }
                                else
                                {
                                    asm = System.Reflection.Assembly.LoadFile(asmFileName);
                                }
#else
                                asm = System.Reflection.Assembly.LoadFile(asmFileName);
#endif
                                _LocalAssemblies = null;
                                findAsm = true;
                                if (asm != null)
                                {
                                    result = asm.GetType(this.Name, false, false);
                                }
                            }
                            catch (System.Exception ext)
                            {
                                if (_MissLibNames.Contains(this.LibraryName) == false)
                                {
                                    _MissLibNames.Add(this.LibraryName);
                                    MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                                    MyConsole.Instance.Write(Environment.NewLine + "    [Warring]Error load referenced assembly file : " + asmFileName + " , MSG:" + ext.Message);
                                    MyConsole.Instance.ResetColor();
                                }
                            }
                        }
                        else
                        {
                            if (_MissLibNames.Contains(this.LibraryName) == false)
                            {
                                _MissLibNames.Add(this.LibraryName);
                                MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                                MyConsole.Instance.Write(Environment.NewLine + "    [Warring]Can not find referenced assembly : " + this.LibraryName + ".dll");
                                MyConsole.Instance.ResetColor();
                            }
                        }
                    }
                    if (findAsm == false)
                    {
                        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if ((asm is System.Reflection.Emit.AssemblyBuilder) == false)
                            {
                                result = asm.GetType(this.Name, false, false);
                                if (result != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (result == null)
                    {
                        // 最后抢救一下
                        var asm4 = typeof(string).Assembly;
                        result = asm4.GetType(this.Name, false, false);
                    }
                }
                if(result == null )
                {
                    MyConsole.Instance.Write( Environment.NewLine + "    [Warring]Can not find native type :[" + this.LibraryName + "]" + this.Name);
                }
                _NativeTypes[this.NameWithLibraryName] = result;
            }
            return result;
        }

        private Type _NativeType = null;
        public Type NativeType
        {
            get
            {
                return this._NativeType;
            }
        }
        public void UpdateLocalClass(Dictionary<string, DCILClass> clses)
        {
            var bc = this.LocalClass;
            clses.TryGetValue(this.Name, out this.LocalClass);
            if (bc != null && this.LocalClass == null)
            {

            }
        }

        public DCILClass LocalClass = null;
        public bool Handled = false;
        private int _HashCode = 0;

        public override int GetHashCode()
        {
            if (this._HashCode == 0)
            {
                if (this.LibraryName != null)
                {
                    this._HashCode = this.LibraryName.GetHashCode();
                }
                if (this.Name != null)
                {
                    this._HashCode += this.Name.GetHashCode();
                }
                this._HashCode += (int)this.Mode;
                if (this.ArrayAndPointerSettings != null)
                {
                    this._HashCode += this.ArrayAndPointerSettings.GetHashCode();
                }
                if (this.GenericParamters != null && this.GenericParamters.Count > 0)
                {
                    foreach (var item in this.GenericParamters)
                    {
                        this._HashCode += item.GetHashCode();
                    }
                }
            }
            return this._HashCode;
        }
        public override bool Equals(object obj)
        {
            if (obj is DCILTypeReference)
            {
                return this.EqualsValue((DCILTypeReference)obj);
            }
            return false;
        }

        public static bool StaticEquals(DCILTypeReference t1, DCILTypeReference t2)
        {
            if (t1 == t2)
            {
                return true;
            }
            if (t1 != null)
            {
                return t1.EqualsValue(t2);
            }
            else
            {
                return false;
            }
        }
        public bool EqualsValue(DCILTypeReference t )
        {
            return EqualsValue(t, true);
        }
        public bool EqualsValue(DCILTypeReference t , bool checkMarshalAndMod  )
        {
            if (t == null)
            {
                return false;
            }
            if (t == this)
            {
                return true;
            }
            if (this.Name != t.Name)
            {
                return false;
            }
            if (this.LibraryName != t.LibraryName)
            {
                return false;
            }
            if (this.Mode != t.Mode)
            {
                return false;
            }
            if (DCUtils.EqualsStringExt(this.ArrayAndPointerSettings, t.ArrayAndPointerSettings) == false)
            {
                return false;
            }
            int num1 = this.GenericParamters == null ? 0 : this.GenericParamters.Count;
            int num2 = t.GenericParamters == null ? 0 : t.GenericParamters.Count;
            if (num1 != num2)
            {
                return false;
            }
            if (num1 > 0)
            {
                for (int iCount = 0; iCount < num1; iCount++)
                {
                    if (this.GenericParamters[iCount].EqualsValue(t.GenericParamters[iCount]) == false)
                    {
                        return false;
                    }
                }
            }
            if (checkMarshalAndMod)
            {
                if (DCUtils.EqualsStringExt(this.MarshalAs, t.MarshalAs) == false)
                {
                    return false;
                }
                num1 = this._modopt_modreqs == null ? 0 : this._modopt_modreqs.Count;
                num2 = t._modopt_modreqs == null ? 0 : t._modopt_modreqs.Count;
                if (num1 != num2)
                {
                    return false;
                }
                else if (num1 > 0)
                {
                    for (var iCount = 0; iCount < num1; iCount++)
                    {
                        if (this._modopt_modreqs[iCount] != t._modopt_modreqs[iCount])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public DCILTypeMode Mode = DCILTypeMode.Primitive;
        /// <summary>
        /// 是否为本地类型
        /// </summary>
        public bool IsLocalType
        {
            get
            {
                if (this.LibraryName == null || this.LibraryName.Length == 0)
                {
                    if (this.Mode == DCILTypeMode.Class
                        || this.Mode == DCILTypeMode.ValueType
                        || this.Mode == DCILTypeMode.NotSpecify)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsPrimitive
        {
            get
            {
                return this.Mode == DCILTypeMode.Primitive;
            }
        }

        public DCILTypeReference(string name)
        {
            this.Name = name;
            this.Mode = DCILTypeMode.NotSpecify;
        }
        public const string ClassMode_NotSpecify = "NotSpecify";

        private DCILTypeReference(string firstWord, DCILReader reader)
        {
            if (firstWord == "class" || firstWord == "valuetype" || firstWord == ClassMode_NotSpecify)
            {
                if (firstWord == "class")
                {
                    this.Mode = DCILTypeMode.Class;
                }
                else if (firstWord == "valuetype")
                {
                    this.Mode = DCILTypeMode.ValueType;
                }
                else
                {
                    this.Mode = DCILTypeMode.NotSpecify;
                }
                ReadName(reader);
                int pos = reader.Position;
                string word = reader.ReadWord();
                if (word == "<")
                {
                    this.GenericParamters = new List<DCILTypeReference>();
                    while (reader.HasContentLeft())
                    {
                        word = reader.ReadWord();
                        if (word == ">")
                        {
                            break;
                        }
                        else if (IsStartWord(word))
                        {
                            var subType = Load(word, reader);
                            this.GenericParamters.Add(subType);
                        }
                    }
                }
                else
                {
                    reader.Position = pos;
                }
            }
            else if (firstWord == "native")
            {
                this.Name = reader.ReadWord();
                this.Mode = DCILTypeMode.Native;
            }
            else if (firstWord == "unsigned")
            {
                this.Name = reader.ReadWord();
                this.Mode = DCILTypeMode.Unsigned;
            }
            else if (firstWord[0] == '!')
            {
                if (firstWord.Length > 2 && firstWord[1] == '!')
                {
                    this.Mode = DCILTypeMode.GenericTypeInMethodDefine;
                    this.Name = DCUtils.GetStringUseTable( firstWord.Substring(2));
                }
                else
                {
                    this.Mode = DCILTypeMode.GenericTypeInTypeDefine;
                    this.Name = DCUtils.GetStringUseTable(firstWord.Substring(1));
                }
            }
            this.ReadExtInfo( reader );
        }

        public DCILTypeReference(string type, DCILTypeMode m = DCILTypeMode.Primitive)
        {
            this.Name = type;
            this.Mode = m;
        }

        public string NameWithLibraryName
        {
            get
            {
                if (this.LibraryName != null && this.LibraryName.Length > 0)
                {
                    return "[" + this.LibraryName + "]" + this.Name;
                }
                else
                {
                    return this.Name;
                }
            }
        }
        private void ReadName(DCILReader reader)
        {
            var strWord = reader.ReadWord();
            if (strWord == "[")
            {
                this.LibraryName = reader.ReadAfterCharExcludeLastChar(']');
                this.Name = reader.ReadWord();
            }
            else
            {
                this.Name = strWord;
            }
        }
        private string _ArrayAndPointerSettings = null;
        public string ArrayAndPointerSettings
        {
            get
            {
                return this._ArrayAndPointerSettings;
            }
            set
            {
                this._ArrayAndPointerSettings = value;
            }
        }

        public DCILTypeReference ChangeArrayAndPointerSettings(string newValue)
        {
            if (DCUtils.EqualsStringExt(this.ArrayAndPointerSettings, newValue) == false)
            {
                var item = (DCILTypeReference)this.MemberwiseClone();
                item._ArrayAndPointerSettings = newValue;
                return item;
            }
            else
            {
                return this;
            }
        }
        private bool Read_modopt_modreq(DCILReader reader)
        {
            bool result = false;
            while(List_modopt_modreq.IsStartWord(reader.PeekWord()))
            {
                if (this._modopt_modreqs == null)
                {
                    this._modopt_modreqs = new List_modopt_modreq();
                }
                this._modopt_modreqs.Read(reader.ReadWord(), reader);
                result = true;
            }
            return result;
        }
    private bool ReadArrayAndPointerSettings(DCILReader reader)
        {
            if (reader != null)
            {
                var result = new List<char>();
                bool inArray = false;
                int len = 0;
                for (int iCount = reader.Position; iCount < reader.Length; iCount++)
                {
                    var c = reader.GetChar(iCount);
                    if (c == '*' )
                    {
                        result.Add(c);
                        len++;
                    }
                    else if (c == '&' && result.IndexOf('&') < 0)
                    {
                        result.Add(c);
                        len++;
                    }
                    else if (c == '[' && inArray == false)
                    {
                        result.Add(c);
                        inArray = true;
                        len++;
                    }
                    else if (c == ']' && inArray == true)
                    {
                        result.Add(c);
                        inArray = false;
                        len++;
                    }
                    else if ((c == '.' || c == ',' || char.IsNumber(c)) && inArray)
                    {
                        result.Add(c);
                        len++;
                    }
                    else if (c == ' ' && c == '\t')
                    {
                        len++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (result.Count > 0)
                {
                    reader.Position += len;
                    if (result.Count == 2 && result[0] == '[' && result[1] == ']')
                    {
                        this.ArrayAndPointerSettings = "[]";
                    }
                    else if (result.Count == 1)
                    {
                        this.ArrayAndPointerSettings = DCUtils.GetSingleCharString(result[0]);
                    }
                    else if(result.Count == len )
                    {
                        this.ArrayAndPointerSettings = reader.GetSubStringUseTable(reader.Position - len, len);
                    }
                    else
                    {
                        this.ArrayAndPointerSettings = DCUtils.GetStringUseTable(new string(result.ToArray()));
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name = null;

        public List<DCILTypeReference> GenericParamters = null;
        public string LibraryName = null;
        public bool HasLibraryName
        {
            get
            {
                return this.LibraryName != null && this.LibraryName.Length > 0;
            }
        }
        public bool IsArray
        {
            get
            {
                return this.ArrayAndPointerSettings != null
                    && this.ArrayAndPointerSettings.Length > 0
                    && this.ArrayAndPointerSettings.IndexOf('[') >= 0;
            }
        }
        public bool IsGenericType
        {
            get
            {
                return this.GenericParamters != null && this.GenericParamters.Count > 0;
            }
        }
        private string FixTypeName(string name, bool useShortName)
        {
            if (useShortName && name != null && name.Length > 0 )
            {
                return DCUtils.GetShortName(name);
            }
            return name;
        }
        public void WriteToForSignString(DCILWriter writer, DCILGenericParamterList gps = null, int stackLevel = 0 , bool useShortName = false )
        {
            if (stackLevel > 20)
            {

            }

            switch (this.Mode)
            {
                case DCILTypeMode.Primitive:
                    writer.Write(this._NativeType.Name);
                    //writer.Write( FixTypeName( this.Name , useShortName));
                    break;
                case DCILTypeMode.GenericTypeInMethodDefine:
                    {
                        bool find = false;
                        if (gps != null && gps.Count > 0)
                        {
                            foreach (var gp in gps)
                            {
                                if (gp.DefineInClass == false && gp.Name == this.Name)
                                {
                                    if (gp.RuntimeType != null && gp.RuntimeType.IsGenericType == false)
                                    {
                                        gp.RuntimeType.WriteToForSignString(writer, gps ,stackLevel +1, useShortName);
                                    }
                                    else
                                    {
                                        writer.Write("!!" + gp.Index);
                                    }
                                    find = true;
                                    break;
                                }
                            }
                        }
                        if (find == false)
                        {
                            writer.Write("!!" + this.Name);
                        }
                    }
                    break;
                case DCILTypeMode.GenericTypeInTypeDefine:
                    {
                        bool find = false;
                        if (gps != null && gps.Count > 0)
                        {
                            foreach (var gp in gps)
                            {
                                if (gp.DefineInClass == true && gp.Name == this.Name)
                                {
                                    if (gp.RuntimeType != null
                                        && gp.RuntimeType.Mode != DCILTypeMode.GenericTypeInMethodDefine
                                        && gp.RuntimeType.Mode != DCILTypeMode.GenericTypeInTypeDefine)
                                    {
                                        gp.RuntimeType.WriteToForSignString(writer, gps, stackLevel + 1 , useShortName);
                                    }
                                    else
                                    {
                                        writer.Write("!" + gp.Index);
                                    }
                                    find = true;
                                    break;
                                }
                            }
                        }
                        if (find == false)
                        {
                            writer.Write("!" + this.Name);
                        }
                    }
                    break;
                case DCILTypeMode.Class:
                    if (this.LocalClass != null)
                    {
                        if (useShortName)
                        {
                            writer.Write(FixTypeName(this.LocalClass.Name, true ));
                        }
                        else
                        {
                            writer.Write(this.LocalClass.GetNameWithNested('.'));
                        }
                    }
                    else
                    {
                        writer.Write(FixTypeName(this.Name, useShortName));
                    }
                    break;
                case DCILTypeMode.ValueType:
                    writer.Write( FixTypeName( this.Name , useShortName ));
                    break;
                case DCILTypeMode.NotSpecify:
                    writer.Write( FixTypeName( this.Name , useShortName ));
                    break;
                case DCILTypeMode.Native:
                    writer.Write("native " + this.Name);
                    break;
                case DCILTypeMode.Unsigned:
                    writer.Write("unsigned " + this.Name);
                    break;
            }
            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
            {

                writer.Write("<");
                for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    this.GenericParamters[iCount].WriteToForSignString(writer, gps , stackLevel , useShortName );
                }
                writer.Write(">");
            }
            if (this.ArrayAndPointerSettings != null
                && this.ArrayAndPointerSettings.Length > 0)
            {
                writer.Write(this.ArrayAndPointerSettings);
            }
        }

        public void WriteTo(DCILWriter writer, bool writeHeader = true, bool writeLibraryName = true)
        {
            if (this.Mode == DCILTypeMode.Primitive)
            {
                WriteName(writer, writeLibraryName);
            }
            else if (this.Mode == DCILTypeMode.Class
                || this.Mode == DCILTypeMode.ValueType
                || this.Mode == DCILTypeMode.NotSpecify)
            {
                if (writeHeader)
                {
                    if (this.Mode == DCILTypeMode.Class)
                    {
                        writer.Write("class ");
                    }
                    else if (this.Mode == DCILTypeMode.ValueType)
                    {
                        writer.Write("valuetype ");
                    }
                }
                WriteName(writer, writeLibraryName);
                if (this.GenericParamters != null && this.GenericParamters.Count > 0)
                {
                    writer.Write('<');
                    for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                    {
                        if (iCount > 0)
                        {
                            writer.Write(',');
                        }
                        this.GenericParamters[iCount].WriteTo(writer);
                        //writer.Write(this.GenericParamters[iCount].ToString());
                    }
                    writer.Write('>');
                }
            }
            else if (this.Mode == DCILTypeMode.Native)
            {
                writer.Write("native " + this.Name);
            }
            else if (this.Mode == DCILTypeMode.Unsigned)
            {
                writer.Write("unsigned " + this.Name);
            }
            else if (this.Mode == DCILTypeMode.GenericTypeInMethodDefine)
            {
                writer.Write("!!" + this.Name);
            }
            else if (this.Mode == DCILTypeMode.GenericTypeInTypeDefine)
            {
                //if( this.Name == "TKey2")
                //{

                //}
                writer.Write("!" + this.Name);
            }
            else
            {
                throw new Exception("未知类型" + this.Mode);
            }
            if (this.ArrayAndPointerSettings != null && this.ArrayAndPointerSettings.Length > 0)
            {
                writer.Write(this.ArrayAndPointerSettings);
            }
            if( this.MarshalAs != null && this.MarshalAs.Length > 0 )
            {
                writer.Write(" marshal(" + this.MarshalAs + ") ");
            }
            if( this._modopt_modreqs != null && this._modopt_modreqs.Count > 0 )
            {
                this._modopt_modreqs.Write(writer);
            }
        }

        private void WriteName(DCILWriter writer, bool writeLibraryName)
        {
            if (this.LocalClass != null)
            {
                var nwn = this.LocalClass.NameWithNested;
                //if(nwn.StartsWith("'<PrivateImplementationDetails>'"))
                //{

                //}
                writer.Write(nwn);
            }
            else
            {
                if (writeLibraryName
                    && this.LibraryName != null
                    && this.LibraryName.Length > 0)
                {
                    writer.Write('[');
                    writer.Write(this.LibraryName);
                    writer.Write(']');
                }
                //if(this.Name.StartsWith("'<PrivateImplementationDetails>'"))
                //{

                //}
                writer.Write(this.Name);
            }
        }
        public override string ToString()
        {
            var str = new StringBuilder();
            var w = new DCILWriter(str);
            this.WriteTo(w);
            var resut = str.ToString();
            return resut;
        }
    }

}
