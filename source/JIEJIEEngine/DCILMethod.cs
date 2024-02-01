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
    internal class DCILMethod : DCILMemberInfo
    {
        ///// <summary>
        ///// 所属的重载列表
        ///// </summary>
        //public DCJieJieNetEngine.RefMethodList OwnerOverriedMethods = null;

        public bool ObfuscateControlFlowFlag = false;
        /// <summary>
        /// 系统内部自动产生的
        /// </summary>
        public bool InnerGenerate = false;

        public static readonly IComparer<DCILMethod> ComparerByName = new NameCompaer(false);

        internal class NameCompaer : IComparer<DCILMethod>
        {
            public NameCompaer(bool comareClassName)
            {
                this._CompareClassName = comareClassName;
            }
            private bool _CompareClassName = false;

            public int Compare(DCILMethod x, DCILMethod y)
            {
                if (this._CompareClassName)
                {
                    int v = string.Compare(x.OwnerClass?.Name, y.OwnerClass?.Name);
                    if (v != 0)
                    {
                        return v;
                    }
                }
                return string.Compare(x.Name, y.Name);
            }
        }
        public static readonly HashSet<string> PredefinedAttributes = null;
        static DCILMethod()
        {
            PredefinedAttributes = new HashSet<string>();
            PredefinedAttributes.Add("abstract");
            PredefinedAttributes.Add("assembly");
            PredefinedAttributes.Add("compilercontrolled");
            PredefinedAttributes.Add("famandassem");
            PredefinedAttributes.Add("family");
            PredefinedAttributes.Add("famorassem");
            PredefinedAttributes.Add("final");
            PredefinedAttributes.Add("hidebysig");
            PredefinedAttributes.Add("newslot");
            PredefinedAttributes.Add("private");
            PredefinedAttributes.Add("public");
            PredefinedAttributes.Add("rtspecialname");
            PredefinedAttributes.Add("specialname");
            PredefinedAttributes.Add("static");
            PredefinedAttributes.Add("virtual");
            PredefinedAttributes.Add("strict");
            PredefinedAttributes.Add("pinvokeimpl");
            PredefinedAttributes.Add("instance");
        }

        public static bool IsCtorOrCctor(string methodName)
        {
            return methodName == ".ctor" || methodName == ".cctor";
        }
        public const string TagName = ".method";

        public static readonly DCILMethod Empty = new DCILMethod();

        public DCILMethod(DCILClass parent, DCILReader reader)
        {
            this.Parent = parent;
            this.Load(reader);
            this.HasGenericStyle = GetHasGenericStyle();
            if (this.Name == "SMF_CreateEmptyTable")
            {

            }
            //if ( parent.IsInterface == false
            //    && this.IsAbstract == false
            //    && ( this.OperCodes == null || this.OperCodes.Count == 0 ) 
            //    && parent.BaseType.Name != "System.MulticastDelegate"
            //    && this.Pinvokeimpl == null )
            //{

            //}
        }
        public string[] GetStringValues()
        {
            var list = new System.Collections.Generic.List<string>();
            this.EnumOperCodes(delegate (EnumOperCodeArgs args)
            {
                if (args.Current is DCILOperCode_LoadString)
                {
                    var lds = (DCILOperCode_LoadString)args.Current;
                    if ( lds.Value != null 
                        &&  lds.Value.Length > 0 
                        && list.Contains(lds.Value) == false)
                    {
                        list.Add(lds.Value);
                    }
                }
            });
            list.Sort();
            return list.ToArray();
        }
    

        /// <summary>
        /// 获得方法中快速初始化数组的字节数大小
        /// </summary>
        /// <param name="eng"></param>
        /// <returns></returns>
        public int GetByteArraySize(DCJieJieNetEngine eng)
        {
            var bytesArraySize = 0;
            this.EnumOperCodes(delegate (EnumOperCodeArgs args)
            {
                var callCode = args.Current as DCILOperCode_HandleMethod;
                if (callCode != null
                    && callCode.MatchTypeAndMethod(
                        "System.Runtime.CompilerServices.RuntimeHelpers",
                        "InitializeArray",
                        2))
                {
                    var items = args.OwnerList;
                    var codeIndex = args.CurrentCodeIndex;
                    var ldTokenCode = items[codeIndex - 1] as DCILOperCode_LdToken;
                    var data = ldTokenCode.FieldReference?.LocalField?.ReferenceData?.Value;
                    if (data is byte[])
                    {
                        bytesArraySize += ((byte[])data).Length;
                    }
                    else if (data is string)
                    {
                        bytesArraySize += ((string)data).Length;
                    }
                }
            });

            return bytesArraySize;
        }

        private static readonly List<string> _NewLabelIDList = new List<string>();
        public static void ClearNewLabelIDCache()
        {
            _NewLabelIDList.Clear();
        }
        private int _NewLabelIndex = 0;
        /// <summary>
        /// 创建新的指令行标记
        /// </summary>
        /// <returns>新的标记</returns>
        public string GenNewLabelID()
        {
            this._NewLabelIndex++;
            if (this._NewLabelIndex >= _NewLabelIDList.Count)
            {
                for (int iCount = _NewLabelIDList.Count; iCount <= this._NewLabelIndex; iCount++)
                {
                    _NewLabelIDList.Add("IL_N" + iCount.ToString("0000"));
                    //if (iCount < 100)
                    //{
                    //    _NewLabelIDList.Add("IL_N" + iCount.ToString("000"));
                    //}
                    //else
                    //{
                    //    _NewLabelIDList.Add("IL_N" + iCount);
                    //}
                }
            }
            return _NewLabelIDList[this._NewLabelIndex];
        }

        public void EnumOperCodes(EnumOperCodeHandler handler )
        {
            if( this.OperCodes != null &&this.OperCodes.Count > 0 )
            {
                this.OperCodes.EnumDeeply(this, handler);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.RuntimeSwitchs = null;
            if(this.GenericParamters != null )
            {
                this.GenericParamters.Dispose();
                this.GenericParamters = null;
            }
            if(this.Locals != null )
            {
                this.Locals.Dispose();
                this.Locals = null;
            }
            if(this.Parameters != null )
            {
                foreach( var item in this.Parameters )
                {
                    item.Dispose();
                }
                this.Parameters.Clear();
                this.Parameters = null;
            }
            this.ParentMember = null;
            this.permission = null;
            this.permissionset = null;
            this.Pinvokeimpl = null;
            this.ReturnType = null;
            this.ReturnTypeInfo = null;
            this.RuntimeSwitchs = null;
            this.Type = null;
            this._NativeDecilaryTypeName = null;
            this._NativeMethod = null;
            if(this._Override != null )
            {
                this._Override.Dispose();
                this._Override = null;
            }
            this._SignString = null;
            
        }
        /// <summary>
        /// 运行时使用的开关
        /// </summary>
        public JieJieSwitchs RuntimeSwitchs = null;

        public DCILTypeReference GetResultValueTypeForLoad(DCILOperCode code)
        {
            if (code == null || code.OperCode == null || code.OperCode.Length == 0)
            {
                return null;
            }
            if (code.OperCodeValue == DCILOpCodeValue.Ldfld
                || code.OperCodeValue == DCILOpCodeValue.Ldsfld)
            {
                if (code is DCILOperCode_HandleField)
                {
                    var rf = ((DCILOperCode_HandleField)code).Value?.ValueType;
                    return rf;
                }
                return null;
            }
            if ( code.OperCode.StartsWith("ldarg" , StringComparison.Ordinal)
                || code.OperCode.StartsWith("ldloc" , StringComparison.Ordinal)
                )
            {
                int pIndex = -1;
                int locIndex = -1;
                if (code.OperCodeValue == DCILOpCodeValue.Ldarg 
                    || code.OperCodeValue == DCILOpCodeValue.Ldarg_S)
                {
                    try
                    {
                        if (this.Parameters != null)
                        {
                            foreach (var item in this.Parameters)
                            {
                                if (item.Name == code.OperData)
                                {
                                    return item.ValueType;
                                }
                            }
                        }
                        pIndex = DCUtils.ConvertToInt32(code.OperData);
                    }
                    catch
                    {
                        
                        return null;
                    }
                }
                else if (code.OperCodeValue == DCILOpCodeValue.Ldloc 
                    || code.OperCodeValue == DCILOpCodeValue.Ldloc_S)
                {
                    try
                    {
                        if (this.Locals != null)
                        {
                            foreach (var item in this.Locals)
                            {
                                if (item.Name == code.OperData)
                                {
                                    return item.ValueType;
                                }
                            }
                        }
                        locIndex = DCUtils.ConvertToInt32(code.OperData);
                    }
                    catch
                    {
                        return null;
                    }
                }
                
                else
                {
                    switch (code.OperCodeValue)
                    {
                        case DCILOpCodeValue.Ldarg_0: pIndex = 0; break;
                        case DCILOpCodeValue.Ldarg_1: pIndex = 1; break;
                        case DCILOpCodeValue.Ldarg_2: pIndex = 2; break;
                        case DCILOpCodeValue.Ldarg_3: pIndex = 3; break;

                        case DCILOpCodeValue.Ldloc_0: locIndex = 0; break;
                        case DCILOpCodeValue.Ldloc_1: locIndex = 1; break;
                        case DCILOpCodeValue.Ldloc_2: locIndex = 2; break;
                        case DCILOpCodeValue.Ldloc_3: locIndex = 3; break;
                    }
                }
                if (pIndex >= 0 && this.Parameters != null && pIndex < this.Parameters.Count)
                {
                    return this.Parameters[pIndex].ValueType;
                }
                if (locIndex >= 0 && this.Locals != null && locIndex < this.Locals.Count)
                {
                    return this.Locals[locIndex].ValueType;
                }
            }
            return null;
        }

        public DCILTypeReference GetTargetValueTypeForSet(DCILOperCode code)
        {
            if (code == null || code.OperCode == null || code.OperCode.Length == 0)
            {
                return null;
            }
            if (code.OperCodeValue == DCILOpCodeValue.Stfld
                || code.OperCodeValue == DCILOpCodeValue.Stsfld)
            {
                if (code is DCILOperCode_HandleField)
                {
                    var rf = ((DCILOperCode_HandleField)code).Value?.ValueType;
                    return rf;
                }
                return null;
            }
            if(code.OperCodeValue == DCILOpCodeValue.Starg 
                || code.OperCodeValue == DCILOpCodeValue.Starg_S)
            {
                if (this.Parameters != null && this.Parameters.Count > 0)
                {
                    foreach (var item in this.Parameters)
                    {
                        if (item.Name == code.OperData)
                        {
                            return item.ValueType;
                        }
                    }
                    try
                    {
                        int index = DCUtils.ConvertToInt32(code.OperData);
                        if (index >= 0 && index < this.Parameters.Count)
                        {
                            return this.Parameters[index].ValueType;
                        }
                    }
                    catch
                    {
                    }
                }
                return null;
            }
            if (code.OperCode.StartsWith("stloc", StringComparison.Ordinal)
                && this.Locals != null 
                && this.Locals.Count > 0 )
            {
                if (code.OperCodeValue == DCILOpCodeValue.Stloc 
                    || code.OperCodeValue == DCILOpCodeValue.Stloc_S)
                {
                    foreach (var item in this.Locals)
                    {
                        if (item.Name == code.OperData)
                        {
                            return item.ValueType;
                        }
                    }
                    try
                    {
                        int index = DCUtils.ConvertToInt32(code.OperData);
                        if (index >= 0 && index < this.Locals.Count)
                        {
                            return this.Locals[index].ValueType;
                        }
                    }
                    catch
                    {

                    }
                    return null;
                }
                else
                {
                    int index = -1;
                    if (code.OperCodeValue == DCILOpCodeValue.Stloc_0) index = 0;
                    else if (code.OperCodeValue == DCILOpCodeValue.Stloc_1) index = 1;
                    else if (code.OperCodeValue == DCILOpCodeValue.Stloc_2) index = 2;
                    else if (code.OperCodeValue == DCILOpCodeValue.Stloc_3) index = 3;
                    if(index >= 0 && index < this.Locals.Count )
                    {
                        return this.Locals[index].ValueType;
                    }
                }
            }
            
            return null;
        }

        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Method;
            }
        }

        public DCILMethod SimpleClone()
        {
            var method = (DCILMethod)this.MemberwiseClone();
            if (this.Styles != null)
            {
                method.Styles = new List<string>(this.Styles);
            }
            return method;
        }

        public bool FlagsForPrivate = true;

        public int IndexOfExtLocals = int.MinValue;
        /// <summary>
        /// 是否处于某个重载链条中
        /// </summary>
        public bool IsInOverrideList = false;

        public  System.Reflection.MethodInfo _NativeMethod = null;
        public string _NativeDecilaryTypeName = null;
        public DCILMethod(
            System.Reflection.MethodInfo method,
            DCILDocument document,
            DCILGenericParamterList typeGps) : base(method)
        {
            this.IsFromInterface = method.DeclaringType.IsInterface;
            this._NativeMethod = method;
            var declaringType = method.DeclaringType;
            this._NativeDecilaryTypeName = DCUtils.GetFullName(method.DeclaringType);
            this.ReturnTypeInfo = CreateFromNative(declaringType, method, method.ReturnType, document);
            if (this.ReturnTypeInfo != null && typeGps != null)
            {
                this.ReturnTypeInfo = this.ReturnTypeInfo.Transform(typeGps);
            }
            this.Styles = new List<string>();
            this.IsPublic = method.IsPublic;
            this.IsStatic = method.IsStatic;
            this.IsVirtual = method.IsVirtual;
            this.IsSpecialname = method.IsSpecialName;
            this.IsAbstract = method.IsAbstract;
            this.IsFinal = method.IsFinal;

            if (method.IsGenericMethod)
            {
                var gps = method.GetGenericArguments();
                this.GenericParamters = new DCILGenericParamterList(gps.Length);
                foreach (var gp in gps)
                {
                    var dcgp = new DCILGenericParamter(gp.Name, false);
                    this.GenericParamters.Add(dcgp);
                }
            }
            var ps = method.GetParameters();
            if (ps != null && ps.Length > 0)
            {
                this.Parameters = new List<DCILMethodParamter>(ps.Length);
                foreach (var p in ps)
                {
                    var dcp = new DCILMethodParamter();
                    dcp.Name = p.Name;
                    dcp.IsIn = p.IsIn;
                    dcp.IsOut = p.IsOut;
                    var pt2 = p.ParameterType;

                    dcp.ValueType = CreateFromNative(declaringType, method, pt2, document);
                    if (dcp.ValueType != null && typeGps != null)
                    {
                        dcp.ValueType = dcp.ValueType.Transform(typeGps);
                    }

                    if (p.DefaultValue != null && DBNull.Value.Equals(p.DefaultValue) == false)
                    {
                        dcp.DefaultValue = p.DefaultValue.ToString();
                    }
                    this.Parameters.Add(dcp);
                }
            }
            this.HasGenericStyle = GetHasGenericStyle();
        }

        public bool IsFromInterface = false;

        public bool IsStatic = false;


        public bool IsPublic = false;

        public bool IsVirtual = false;

        public bool IsInstance = false;

        public bool IsSpecialname = false;

        public bool IsNewslot = false;

        public bool IsFinal = false;

        public bool IsAbstract = false;
        

        public int AddExtLocalsIndex()
        {
            if (this.Locals == null)
            {
                this.Locals = new DCILMethodLocalVariableList();
            }
            var loc = new DCILMethodLocalVariable();
            loc.ValueType = DCILTypeReference.Type_Boolean;
            this.Locals.Add(loc);
            return this.Locals.Count - 1;
        }
     
        private static readonly System.Random _Random = new Random();
        /// <summary>
        /// 是否具有泛型样式
        /// </summary>
        public readonly bool HasGenericStyle = false;

        private bool GetHasGenericStyle()
        {
            if (this.GenericParamters != null
                && this.GenericParamters.Count > 0)
            {
                return true;
            }
            else if (this.ReturnTypeInfo != null
                && (this.ReturnTypeInfo.Mode == DCILTypeMode.GenericTypeInMethodDefine
                || this.ReturnTypeInfo.Mode == DCILTypeMode.GenericTypeInTypeDefine))
            {
                return true;
            }
            else if (this.Parameters != null && this.Parameters.Count > 0)
            {
                foreach (var item in this.Parameters)
                {
                    if (item.ValueType.Mode == DCILTypeMode.GenericTypeInMethodDefine
                        || item.ValueType.Mode == DCILTypeMode.GenericTypeInTypeDefine
                        || item.ValueType.IsGenericType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void ChangeName(string newName)
        {
            base.ChangeName(newName);
            this._SignString = null;
        }
        private static DCILTypeReference CreateFromNative(
            Type declaringType,
            System.Reflection.MethodInfo method,
            Type inputType,
            DCILDocument document)
        {
            string endFix = null;
            if (inputType.IsArray)
            {
                for (int iCount = 0; iCount < inputType.GetArrayRank(); iCount++)
                {
                    endFix = endFix + "[]";
                }
                inputType = inputType.GetElementType();
            }
            if (inputType.IsPointer)
            {
                endFix = "*" + endFix;
                inputType = inputType.GetElementType();
            }
            if (inputType.IsByRef)
            {
                endFix = "&" + endFix;
                inputType = inputType.GetElementType();
            }
            if (inputType.IsGenericParameter)
            {
                if (declaringType.IsGenericType)
                {
                    var gps = declaringType.GetGenericArguments();
                    if (gps != null && gps.Length > 0)
                    {
                        foreach (var item in gps)
                        {
                            if (inputType == item)
                            {
                                var result = new DCILTypeReference(item.Name, DCILTypeMode.GenericTypeInTypeDefine);
                                result = result.ChangeArrayAndPointerSettings(endFix);
                                return result;
                            }
                        }
                    }
                }
                if (method.IsGenericMethod)
                {
                    var gps = method.GetGenericArguments();
                    if (gps != null && gps.Length > 0)
                    {
                        foreach (var item in gps)
                        {
                            if (inputType == item)
                            {
                                var result = new DCILTypeReference(item.Name, DCILTypeMode.GenericTypeInMethodDefine);
                                result = result.ChangeArrayAndPointerSettings(endFix);
                                return result;
                            }
                        }
                    }
                }
            }
            var result2 = DCILTypeReference.CreateByNativeType(inputType, document);
            result2 = result2.ChangeArrayAndPointerSettings(endFix);
            return result2;
        }

        public DCILMemberInfo ParentMember = null;

        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            base.CacheInfo(document, clses);
            this.ReturnTypeInfo = document.CacheTypeReference(this.ReturnTypeInfo);
            DCILGenericParamter.CacheTypeReference(document, this.GenericParamters);
            DCILMethodParamter.CacheTypeReference(document, this.Parameters);
            if (this.Locals != null)
            {
                foreach (var item in this.Locals)
                {
                    item.ValueType = document.CacheTypeReference(item.ValueType);
                }
            }
            if (this._Override != null)
            {
                this._Override = document.CacheDCILInvokeMethodInfo(this._Override);
            }
            if (this.OperCodes != null && this.OperCodes.Count > 0)
            {
                this.OperCodesCacheInfo(this.OperCodes, document, clses);
            }
            base.CusotmAttributesCacheTypeReference(document);
        }
        private void OperCodesCacheInfo(DCILOperCodeList codes, DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            foreach (var item in codes)
            {
                if (item is DCILOperCode_HandleClass)
                {
                    ((DCILOperCode_HandleClass)item).UpdateDomState(document, clses);
                }
                else if (item is DCILOperCode_HandleField)
                {
                    var code = (DCILOperCode_HandleField)item;
                    if (code.Value != null)
                    {
                        code.Value.UpdateLocalInfo(document, clses);
                        code.CacheInfo(document);
                    }
                }
                else if (item is DCILOperCode_HandleMethod)
                {
                    ((DCILOperCode_HandleMethod)item).CacheInfo(document);
                }
                else if (item is DCILOperCode_LdToken)
                {
                    ((DCILOperCode_LdToken)item).CacheInfo(document, clses);
                }
                else if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var group = (DCILOperCode_Try_Catch_Finally)item;
                    if (group.HasTryOperCodes())
                    {
                        OperCodesCacheInfo(group._Try.OperCodes, document, clses);
                    }
                    if (group.HasCatchs())
                    {
                        foreach (var item2 in group._Catchs)
                        {
                            item2.ExcpetionType = document.CacheTypeReference(item2.ExcpetionType);
                            if (item2.OperCodes != null)
                            {
                                OperCodesCacheInfo(item2.OperCodes, document, clses);
                            }
                        }
                    }
                    if (group.HasFinallyOperCodes())
                    {
                        OperCodesCacheInfo(group._Finally.OperCodes, document, clses);
                    }
                    if (group.HasFaultOperCodes())
                    {
                        OperCodesCacheInfo(group._fault.OperCodes, document, clses);
                    }
                }
            }
        }

        /// <summary>
        /// 检查函数签名
        /// </summary>
        /// <param name="returnType">返回值类型</param>
        /// <param name="genericParamters">泛型参数</param>
        /// <param name="parameters">参数</param>
        /// <returns>是否匹配</returns>
        public bool MatchSign(
            DCILTypeReference returnType,
            List<DCILTypeReference> genericParamters,
            List<DCILMethodParamter> parameters)
        {
            if ((returnType == null) != (this.ReturnTypeInfo == null))
            {
                return false;
            }
            if (this.ReturnTypeInfo != null)
            {
                if (this.ReturnTypeInfo.EqualsValue(returnType) == false)
                {
                    return false;
                }
            }
            if (DCILGenericParamter.MatchList(this.GenericParamters, genericParamters) == false)
            {
                return false;
            }
            if (DCILMethodParamter.EqualsList(this.Parameters, parameters, false, false) == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查函数签名
        /// </summary>
        /// <param name="returnType">返回值类型</param>
        /// <param name="genericParamters">泛型参数</param>
        /// <param name="parameters">参数</param>
        /// <returns>是否匹配</returns>
        public bool MatchSign(DCILMethod method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (method == this)
            {
                return true;
            }
            if ((method.ReturnTypeInfo == null) != (this.ReturnTypeInfo == null))
            {
                return false;
            }
            if (this.ReturnTypeInfo != null)
            {
                if (this.ReturnTypeInfo.Mode == DCILTypeMode.GenericTypeInMethodDefine)
                {

                }
            }
            if (this.ReturnTypeInfo != null
                && this.ReturnTypeInfo.EqualsValue(method.ReturnTypeInfo) == false)
            {
                return false;
            }
            if (DCUtils.EqualsList<DCILGenericParamter>(this.GenericParamters, method.GenericParamters) == false)
            {
                return false;
            }
            if (DCILMethodParamter.EqualsList(this.Parameters, method.Parameters, false, false) == false)
            {
                return false;
            }
            return true;
        }
        public DCILInvokeMethodInfo _Override = null;

        public bool EntryPoint = false;
        public DCILGenericParamterList GenericParamters = null;
        public string Pinvokeimpl = null;
        public override void Load(DCILReader reader)
        {
            this.StartLineIndex = reader.CurrentLineIndex();
            if (this.StartLineIndex == 88720)
            {

            }
            this.OperCodes = new DCILOperCodeList();
            while (reader.HasContentLeft())
            {
                int posBack = reader.Position;
                string strWord = reader.ReadWord();
                if (strWord == "pinvokeimpl")
                {
                    this.Pinvokeimpl = reader.ReadStyleExtValue();
                }
                else if (DCILTypeReference.IsStartWord(strWord))
                {
                    this.ReturnTypeInfo = DCILTypeReference.Load(strWord, reader);
                    break;
                }
                else if (strWord == "<" || strWord == "(")
                {
                    break;
                }
                else
                {
                    this.AddStyle(strWord, reader);
                }
            }
            this._Name = reader.ReadWord();
            var starChar = reader.ReadContentChar();
            if (starChar == '<')
            {
                // 泛型方法,获得参数类型
                this.GenericParamters = new DCILGenericParamterList(reader, false);
                reader.MoveAfterChar('(');
                //reader.ReadAfterCharExcludeLastChar('(');
            }
            this.Parameters = DCILMethodParamter.ReadParameters(reader);
            this.DeclearEndFix = reader.ReadAfterCharExcludeLastChar('{', true);
            if (this.DeclearEndFix == "cil managed")
            {
                this.DeclearEndFix = "cil managed";
            }
            else if (this.DeclearEndFix == "cil managed preservesig")
            {
                this.DeclearEndFix = "cil managed preservesig";
            }
            InnerLoadILOperCode(this, reader);

            this.IsStatic = base.HasStyle("instance") == false;
            this.IsPublic = base.HasStyle("public");
            this.IsVirtual = base.HasStyle("virtual");
            this.IsInstance = base.HasStyle("instance");
            this.IsSpecialname = base.HasStyle("specialname");
            this.IsNewslot = base.HasStyle("newslot");
            this.IsAbstract = base.HasStyle("abstract");
            this.IsFinal = base.HasStyle("final");
        }
        
        public DCILClass OwnerClass
        {
            get
            {
                return (DCILClass)this.Parent;
            }
        }
        /// <summary>
        /// 存在特别结构的指令，无法混淆流程。
        /// </summary>
        public bool OperCodeSpecifyStructure = false;

        public string permissionset = null;
        public string permission = null;
        private void InnerLoadILOperCode(DCILObject rootObject, DCILReader reader)
        {
            if (rootObject.OperCodes == null)
            {
                rootObject.OperCodes = null;
            }
            DCILOperCodeList operInfoList = rootObject.OperCodes;
            bool lastIsFilter = false;
            while (reader.HasContentLeft())
            {
                int pos = reader.Position;
                var strWord = reader.ReadWord();

                if (lastIsFilter)
                {
                    lastIsFilter = false;
                    if (strWord == "{")
                    {
                        var tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList.LastCode;
                        if (tryBlock != null && tryBlock._Filter != null)
                        {
                            var catch2 = new DCILCatchBlock();
                            catch2.Parent = rootObject;
                            if (tryBlock._Catchs == null)
                            {
                                tryBlock._Catchs = new List<DCILCatchBlock>();
                            }
                            tryBlock._Catchs.Add(catch2);
                            InnerLoadILOperCode(catch2, reader);
                            reader.NumOfOperCode++;
                            continue;
                        }
                    }
                }
                if (strWord.StartsWith("IL_", StringComparison.Ordinal))
                {
                    // 开始读取IL指令
                    string labelID = strWord;
                    strWord = reader.ReadWord();
                    var myDefine = reader.ReadOperCode();
                    switch (myDefine.ExtCodeType)
                    {
                        case ILOperCodeType.ldstr:
                            {
                                operInfoList.Add(new DCILOperCode_LoadString(labelID, reader));
                            }
                            break;
                        case ILOperCodeType.Method:
                            {
                                operInfoList.Add(new DCILOperCode_HandleMethod(labelID, myDefine, reader));
                            }
                            break;
                        case ILOperCodeType.Field:
                            {
                                operInfoList.Add(new DCILOperCode_HandleField(labelID, myDefine, reader));
                            }
                            break;
                        case ILOperCodeType.ldtoken:
                            {
                                operInfoList.Add(new DCILOperCode_LdToken(labelID, reader));
                            }
                            break;
                        case ILOperCodeType.Class:
                            {
                                operInfoList.Add(new DCILOperCode_HandleClass(labelID, myDefine, reader));
                            }
                            break;
                        case ILOperCodeType.switch_:
                            {
                                operInfoList.Add(new DCILOperCode_Switch(labelID, reader));
                            }
                            break;
                        case ILOperCodeType.Jump:
                            {
                                var strOperData = reader.ReadWord();
                                operInfoList.AddItem(labelID, myDefine, strOperData);
                                reader.MoveNextLine();
                            }
                            break;
                        case ILOperCodeType.JumpShort:
                            {
                                var strOperData = reader.ReadWord();
                                operInfoList.AddItem(labelID, myDefine, strOperData);
                                reader.MoveNextLine();
                            }
                            break;
                        case ILOperCodeType.ArgsOrLocalsByName:
                            {
                                var strOperData = reader.ReadWord();
                                operInfoList.AddItem(labelID, myDefine, strOperData);
                                reader.MoveNextLine();
                            }
                            break;
                        case ILOperCodeType.Nop:
                            {
                                operInfoList.AddItem(labelID, myDefine);
                                reader.MoveNextLine();
                            }
                            break;
                        default:
                            {
                                if (myDefine.WithoutOperData)// DCILOperCode.WithoutOperData( strOperCode))
                                {
                                    operInfoList.AddItem(labelID, myDefine);
                                    reader.MoveNextLine();
                                    break;
                                }
                                if (myDefine.ExtCodeType == ILOperCodeType.LoadNumberByOperData)
                                {
                                    reader.SkipLineWhitespace();
                                    var nc = reader.Peek();
                                    if (nc >= '0' && nc <= '9')
                                    {
                                        var strOperData2 = reader.ReadWord();
                                        operInfoList.AddItem(labelID, myDefine, strOperData2);
                                        reader.MoveNextLine();
                                        break;
                                    }
                                    else
                                    {

                                    }
                                }
                                var strOperData = reader.ReadLineTrim();
                                if (myDefine.Value == DCILOpCodeValue.Ldc_R8)// strOperCode == "ldc.r8")
                                {
                                    //var sss = DCUtils.ToHexString(BitConverter.GetBytes(float.NaN));
                                    if (strOperData == "-nan(ind)")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(double.NaN));// "(00 00 00 00 00 00 F8 FF)";
                                    }
                                    else if (strOperData == "-inf")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(double.NegativeInfinity));// "(00 00 00 00 00 00 F0 FF)";
                                    }
                                    else if (strOperData == "inf")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(double.PositiveInfinity)); //"(00 00 00 00 00 00 F0 7F)";
                                    }
                                }
                                else if (myDefine.Value == DCILOpCodeValue.Ldc_R4)//strOperCode == "ldc.r4")
                                {
                                    if (strOperData == "-nan(ind)")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(float.NaN));// "(00 00 00 00 00 00 F8 FF)";
                                    }
                                    else if (strOperData == "-inf")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(float.NegativeInfinity));// "(00 00 00 00 00 00 F0 FF)";
                                    }
                                    else if (strOperData == "inf")
                                    {
                                        strOperData = DCUtils.ToHexString(BitConverter.GetBytes(float.PositiveInfinity)); //"(00 00 00 00 00 00 F0 7F)";
                                    }
                                }
                                operInfoList.AddItem(labelID, myDefine, strOperData);
                            }
                            break;
                    }

                    reader.NumOfOperCode++;
                }
                
                else if (strWord == ".try"
                    || strWord == "catch"
                    || strWord == "finally"
                    || strWord == "fault"
                    || strWord == "filter")
                {

                    DCILOperCode_Try_Catch_Finally tryBlock = null;
                    if (strWord == ".try")
                    {
                        tryBlock = new DCILOperCode_Try_Catch_Finally();
                        operInfoList.Add(tryBlock);
                        string word4 = reader.PeekWord();
                        if (word4.StartsWith("IL"))
                        {
                            tryBlock.SingleLineContent = reader.ReadLine();
                            this.OperCodeSpecifyStructure = true;
                        }
                        else
                        {
                            tryBlock._Try = new DCILObject();
                            tryBlock._Try._Name = ".try";
                            tryBlock._Try.OperCodes = new DCILOperCodeList();
                            tryBlock._Try.Parent = rootObject;
                            InnerLoadILOperCode(tryBlock._Try, reader);
                        }
                    }
                    else if (strWord == "catch")
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList.LastCode;
                        var catch2 = new DCILCatchBlock();
                        catch2.ExcpetionType = DCILTypeReference.Load("class", reader);
                        catch2.Parent = rootObject;
                        if (tryBlock._Catchs == null)
                        {
                            tryBlock._Catchs = new List<DCILCatchBlock>();
                        }
                        tryBlock._Catchs.Add(catch2);
                        InnerLoadILOperCode(catch2, reader);
                    }
                    else if (strWord == "filter")
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList.LastCode;
                        var filter = new DCILObject();
                        filter._Name = "fiter";
                        filter.OperCodes = new DCILOperCodeList();
                        filter.Parent = rootObject;
                        tryBlock._Filter = filter;
                        InnerLoadILOperCode(filter, reader);
                        lastIsFilter = true;
                    }
                    else if (strWord == "fault")
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList.LastCode;
                        tryBlock._fault = new DCILObject();
                        tryBlock._fault._Name = "fault";
                        tryBlock._fault.OperCodes = new DCILOperCodeList();
                        tryBlock._fault.Parent = rootObject;
                        InnerLoadILOperCode(tryBlock._fault, reader);
                    }
                    else // finally
                    {
                        tryBlock = (DCILOperCode_Try_Catch_Finally)operInfoList.LastCode;
                        tryBlock._Finally = new DCILObject();
                        tryBlock._Finally._Name = "finally";
                        tryBlock._Finally.OperCodes = new DCILOperCodeList();
                        tryBlock._Finally.Parent = rootObject;
                        InnerLoadILOperCode(tryBlock._Finally, reader);
                    }
                    reader.NumOfOperCode++;
                }
                else if (strWord == ".maxstack")
                {
                    this.Maxstack = Convert.ToInt32(reader.ReadLine());
                }
                else if (strWord == ".override")
                {
                    strWord = reader.ReadWord();
                    if (strWord == "method")
                    {
                        this._Override = new DCILInvokeMethodInfo(reader);
                    }
                    else
                    {
                        reader.Position -= strWord.Length;

                        this._Override = new DCILInvokeMethodInfo(reader, true);
                        this._Override.ReturnType = this.ReturnTypeInfo;
                        if (this.Parameters != null && this.Parameters.Count > 0)
                        {
                            this._Override.Paramters = new List<DCILMethodParamter>(this.Parameters);

                        }
                    }
                }
                else if (strWord == DCILCustomAttribute.TagName_custom)
                {
                    this.ReadCustomAttribute(reader);
                }
                else if (strWord == ".param")
                {
                    var line = reader.ReadLine();
                    int index = line.IndexOf('=');
                    if (index > 0)
                    {
                        var pIndex = DCILReader.ParseArrayIndex(line.Substring(0, index));
                        if (pIndex >= 0 && pIndex <= this.Parameters.Count)
                        {
                            this.Parameters[pIndex - 1].DefaultValue = line.Substring(index + 1).Trim();
                        }
                    }
                }
                else if (strWord == ".locals")
                {
                    // see topic # 15.4.1.3	The .locals directive
                    this.Locals = new DCILMethodLocalVariableList();
                    strWord = reader.ReadWord();
                    if (strWord == "init")
                    {
                        this.Locals.HasInit = true;
                        strWord = reader.ReadWord();
                    }
                    else
                    {
                        this.Locals.HasInit = false;
                    }
                    if (strWord == "(")
                    {
                        while (reader.HasContentLeft())
                        {
                            var pinfo = new DCILMethodLocalVariable();
                            int pindex = int.MinValue;
                            char startChar = reader.PeekContentChar();
                            if (startChar == '[')
                            {
                                reader.ReadContentChar();
                                var strIndex = reader.ReadAfterChar(']').Trim();
                                if (strIndex.Length > 0 && int.TryParse(strIndex, out pindex))
                                {
                                    pinfo.Index = pindex;
                                }
                            }
                            strWord = reader.ReadWord();
                            if (DCILTypeReference.IsStartWord(strWord))
                            {
                                pinfo.ValueType = DCILTypeReference.Load(strWord, reader);
                                this.Locals.Add(pinfo);
                            }
                            char endChar = char.MinValue;
                            pinfo.Name = DCUtils.GetStringUseTable(reader.ReadAfterCharsExcludeLastChar(",)", out endChar).Trim());

                            if (endChar == ')')
                            {
                                break;
                            }
                        }
                    }
                }
                else if (strWord == ".permissionset")
                {
                    this.permissionset = reader.ReadInstructionContent();
                }
                else if (strWord == ".permission")
                {
                    this.permission = reader.ReadInstructionContent();
                }
                else if (strWord == ".entrypoint")
                {
                    this.EntryPoint = true;
                }
                else if (strWord == "}")
                {
                    // 结束代码组
                    break;
                }
            }
        }
        public override string GetSignatureForMap()
        {
            string mn = this.OwnerClass.GetNameWithNested('.') + "." + this.Name;
            mn = mn + GetParamterListString(false );
            return mn;
        }

        private string _SignString = null;
        public virtual string GetSignString()
        {
            if (this.HasGenericStyle || this._SignString == null)
            {
                this._SignString = InnerGetSignString(this.ReturnTypeInfo, this._Name, this.GenericParamters, ((DCILClass)this.Parent)?.GenericParamters, this.Parameters);
                this._SignString = DCUtils.GetStringUseTable(this._SignString);
            }
            return this._SignString;
        }

        /// <summary>
        /// 获得方法签名字符串
        /// </summary>
        /// <param name="returnType">返回值</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="methodGps">方法使用的泛型参数值</param>
        /// <param name="classGps">方法所属的类型的泛型参数</param>
        /// <param name="ps">方法参数</param>
        /// <returns>字符串</returns>
        public static string InnerGetSignString(
            DCILTypeReference returnType,
            string methodName,
            List<DCILTypeReference> methodGps,
            DCILGenericParamterList classGps,
            List<DCILMethodParamter> ps)
        {
            var str = new StringBuilder();
            var writer = new DCILWriter(str);
            returnType.WriteToForSignString(writer, classGps);
            writer.Write(" ");
            if (methodName != null)
            {
                writer.Write(methodName);
            }
            if (methodGps != null && methodGps.Count > 0)
            {
                writer.Write("<");
                for (int iCount = 0; iCount < methodGps.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    methodGps[iCount].WriteToForSignString(writer, classGps);
                }
                writer.Write(">");
            }
            writer.Write("(");
            if (ps != null && ps.Count > 0)
            {
                for (int iCount = 0; iCount < ps.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    ps[iCount].ValueType.WriteToForSignString(writer, classGps);
                }
            }
            writer.Write(")");
            return str.ToString();
        }

        public string GetParamterListString( bool userShortName , bool outputParameterName = true )
        {
            var writer = new DCILWriter(new StringBuilder());
            if(this.GenericParamters != null && this.GenericParamters.Count > 0 )
            {
                writer.Write("<");
                for(int iCount = 0; iCount < this.GenericParamters.Count; iCount ++)
                {
                    if(iCount > 0 )
                    {
                        writer.Write(",");
                    }
                    var item = this.GenericParamters[iCount];
                    if (item.RuntimeType != null)
                    {
                        item.RuntimeType.WriteToForSignString(writer,null , 0 , userShortName);
                    }
                    else
                    {
                        writer.Write("MT" + item.Index);
                    }
                }
                writer.Write(">");
            }
            writer.Write("(");
            if( this.Parameters != null && this.Parameters.Count > 0 )
            {
                for(int iCount = 0; iCount < this.Parameters.Count; iCount ++)
                {
                    if(iCount > 0 )
                    {
                        writer.Write(",");
                    }
                    var p = this.Parameters[iCount];
                    p.ValueType.WriteToForSignString(writer, null, 0, userShortName);
                    if( outputParameterName
                        && this.RenameState != DCILRenameState.Renamed 
                        && p.Name != null 
                        && p.Name.Length > 0 )
                    {
                        writer.Write(" ");
                        writer.Write(p.Name);
                    }
                }
            }
            writer.Write(")");
            return writer.ToString();
        }

        public static string InnerGetSignString(
            DCILTypeReference returnType,
            string methodName,
            DCILGenericParamterList methodGps,
            DCILGenericParamterList classGps,
            List<DCILMethodParamter> ps)
        {
            var str = new StringBuilder();
            var writer = new DCILWriter(str);
            var allGps = DCILGenericParamterList.Merge(methodGps, classGps);
            returnType?.WriteToForSignString(writer, allGps);
            if (methodName != null)
            {
                if (str.Length > 0)
                {
                    writer.Write(" ");
                }
                writer.Write(methodName);
            }
            if (methodGps != null && methodGps.Count > 0)
            {
                writer.Write("<");
                for (int iCount = 0; iCount < methodGps.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    var item = methodGps[iCount];
                    if (item.RuntimeType != null)
                    {
                        item.RuntimeType.WriteToForSignString(writer, allGps);
                    }
                    else
                    {
                        writer.Write("MT" + item.Index);
                    }
                }
                writer.Write(">");
            }
            writer.Write("(");
            if (ps != null && ps.Count > 0)
            {
                for (int iCount = 0; iCount < ps.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    ps[iCount].ValueType.WriteToForSignString(writer, allGps);
                }
            }
            writer.Write(")");
            return str.ToString();
        }


        public override void WriteTo(DCILWriter writer)
        {
            if( this.StartLineIndex == 88720)
            {

            }
            if (this.RenameState == DCILRenameState.Renamed)
            {
                writer.WriteLine("// " + this.OwnerClass.GetOldName() + "::" + this.GetOldName() + " at line " + this.StartLineIndex);
            }
            else
            {
                writer.WriteLine("// line " + this.StartLineIndex);
            }
            writer.Write(".method ");
            base.WriteStyles(writer);
            if (this.Pinvokeimpl != null && this.Pinvokeimpl.Length > 0)
            {
                writer.Write(" pinvokeimpl (" + this.Pinvokeimpl + ") ");
            }
            this.ReturnTypeInfo.WriteTo(writer);
            writer.Write(" " + this._Name);
            this.GenericParamters?.WriteTo(writer);

            DCILMethodParamter.WriteTo(this.Parameters, writer, false);

            writer.Write(this.DeclearEndFix);
            writer.WriteStartGroup();
            if (this.EntryPoint)
            {
                writer.Write(".entrypoint");
            }
            base.WriteCustomAttributes(writer);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {
                for (int iCount = 0; iCount < this.Parameters.Count; iCount++)
                {
                    var p = this.Parameters[iCount];
                    if (p.DefaultValue != null && p.DefaultValue.Length > 0)
                    {
                        writer.WriteLine(".param [" + Convert.ToString(iCount + 1) + "] = " + p.DefaultValue);
                    }
                }
            }
            if (this.permission != null && this.permission.Length > 0)
            {
                writer.WriteLine(".permission " + this.permission);
            }
            if (this.permissionset != null && this.permissionset.Length > 0)
            {
                writer.WriteLine(".permissionset " + this.permissionset);
            }
            if (this._Override != null)
            {
                if (this._Override.SimpleMode)
                {
                    writer.Write(".override ");
                }
                else
                {
                    writer.Write(".override method ");
                    if (this.Parent.Name == "'<CastYield>d__2`1'")
                    {

                    }
                }
                this._Override.WriteTo(writer);
                writer.WriteLine();
            }
            if (this.Maxstack >= 0)
            {
                writer.WriteLine(".maxstack " + Convert.ToString( this.Maxstack  + this.MaxstackFix ));
            }
            if (this.Locals != null && this.Locals.Count > 0)
            {
                writer.Write(".locals ");
                if (this.Locals.HasInit)
                {
                    writer.Write(" init (");
                }
                else
                {
                    writer.Write("(");
                }
                writer.ChangeIndentLevel(1);
                for (int iCount = 0; iCount < this.Locals.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.WriteLine(",");
                    }
                    var item = this.Locals[iCount];
                    if (item.Index >= 0)
                    {
                        writer.Write("[" + item.Index + "] ");
                    }
                    item.ValueType.WriteTo(writer);
                    if (item.Name != null && item.Name.Length > 0)
                    {
                        writer.Write(" " + item.Name);
                    }
                }
                writer.WriteLine(")");
                writer.ChangeIndentLevel(-1);
            }
            if (this.OperCodes != null && this.OperCodes.Count > 0)
            {
                this.OperCodes.WriteTo(writer);
            }
            writer.WriteEndGroup();
        }
        public DCILMethod()
        {
            base.OperCodes = new DCILOperCodeList();
        }
        public DCILTypeReference ReturnTypeInfo = null;

        public DCILMethodLocalVariableList Locals = null;

        public int Maxstack = -1;
        public int MaxstackFix = 0;
        public List<DCILMethodParamter> Parameters = null;
        public int ParametersCount
        {
            get
            {
                return this.Parameters == null ? 0 : this.Parameters.Count;
            }
        }
        public string DeclearEndFix = null;

        public int ILCodeStartLineIndex = 0;
        public bool ILCodesModified = false;
         public string ReturnType = null;
        public override DCILMethod GetOwnerMethod()
        {
            return this;
        }
        //public int ComponentResourceManagerLineIndex = -1;
        public override string ToString()
        {
            this._SignString = null;
            var resut = this.Name + " ## " + this.GetSignString();// this.ReturnType + " " + this.Name;
            if (this.Parent is DCILClass)
            {
                resut = resut + " # " + ((DCILClass)this.Parent).Name;
            }
            else if (this._NativeMethod != null)
            {
                resut = resut + " #$ " + this._NativeMethod.DeclaringType.Name;
            }
            return resut;
        }
    }


}
