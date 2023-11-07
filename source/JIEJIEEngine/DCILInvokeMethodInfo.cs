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
    internal class DCILInvokeMethodInfo : IEqualsValue<DCILInvokeMethodInfo> , IDisposable
    {
        public DCILInvokeMethodInfo()
        {

        }
        public int LineIndex = 0;
        public readonly bool SimpleMode = false;
        public DCILInvokeMethodInfo(DCILMethod method , bool simpleMode = false)
        {
            if(method == null )
            {
                throw new ArgumentNullException("method");
            }
            if (method.Parent != null)
            {
                this.OwnerType = new DCILTypeReference((DCILClass)method.Parent);
            }
            this.LocalMethod = method;
            this.ReturnType = method.ReturnTypeInfo;
            this.Paramters = method.Parameters;
            this.MethodName = method.Name;
            this.SimpleMode = simpleMode;
            if (method.Parent != null)
            {
                this.OwnerType = new DCILTypeReference((DCILClass)method.Parent);
            }
            this.IsInstance = method.IsInstance;
        }

        

        public DCILInvokeMethodInfo(DCILReader reader, bool simpleMode = false)
        {
            //this.LineIndex = reader.CurrentLineIndex();
            if (simpleMode)
            {
                this.SimpleMode = simpleMode;
                this.ReturnType = DCILTypeReference.GetPrimitiveType("void");
                this.OwnerType = DCILTypeReference.Load(reader);
                if (reader.MatchText("::"))
                {
                    reader.Position += 2;
                    this.MethodName = reader.ReadWord();
                }
                return;
            }
            var strWord = reader.ReadWord();
            if (strWord == "instance")
            {
                this.IsInstance = true;
                strWord = reader.ReadWord();
            }
            else
            {
                this.IsInstance = false;
            }
            if (DCILTypeReference.IsStartWord(strWord))
            {
                this.ReturnType = DCILTypeReference.Load(strWord, reader);
                this.OwnerType = DCILTypeReference.Load(reader);
                if (reader.MatchText("::"))
                {
                    reader.Position += 2;
                    this.MethodName = reader.ReadWord();
                }
                if (reader.PeekContentChar() == '<')
                {
                    this.GenericParamters = new List<DCILTypeReference>();
                    reader.ReadContentChar();
                    while (reader.HasContentLeft())
                    {
                        var chr = reader.PeekContentChar();
                        if (chr == '>')
                        {
                            reader.ReadContentChar();
                            break;
                        }
                        else if (chr == ',')
                        {
                            reader.ReadContentChar();
                            continue;
                        }
                        var pt = DCILTypeReference.Load(reader);
                        if (pt != null)
                        {
                            this.GenericParamters.Add(pt);
                        }
                    }
                }
                if (reader.PeekContentChar() == '(')
                {
                    reader.ReadContentChar();
                    this.Paramters = DCILMethodParamter.ReadParameters(reader, false);
                }

                return;

            }

            reader.ReadLine();
        }
        public void Dispose()
        {
            if( this.GenericParamters != null )
            {
                this.GenericParamters.Clear();
                this.GenericParamters = null;
            }
            this.LocalMethod = null;
            this.MethodName = null;
            this.OwnerType = null;
            if(this.Paramters != null )
            {
                foreach( var item in this.Paramters )
                {
                    item.Dispose();
                }
                this.Paramters.Clear();
                this.Paramters = null;
            }
            this.ReturnType = null;
            if(this.Styles != null )
            {
                this.Styles.Clear();
                this.Styles = null;
            }
        }
        public DCILInvokeMethodInfo SimpleClone()
        {
            return (DCILInvokeMethodInfo)this.MemberwiseClone();
        }
        public DCILInvokeMethodInfo Clone()
        {
            var result = (DCILInvokeMethodInfo)this.MemberwiseClone();
            if (this.Paramters != null)
            {
                result.Paramters = new List<DCILMethodParamter>();
                foreach (var p in this.Paramters)
                {
                    result.Paramters.Add(p.Clone());
                }
            }
            if (this.GenericParamters != null)
            {
                result.GenericParamters = new List<DCILTypeReference>();
                foreach (var p in this.GenericParamters)
                {
                    result.GenericParamters.Add(p);
                }
            }
            return result;
        }
        public void CacheTypeReference(DCILDocument document)
        {
            if (this.ReturnType != null)
            {
                this.ReturnType = document.CacheTypeReference(this.ReturnType);
            }
            if (this.OwnerType != null)
            {
                this.OwnerType = document.CacheTypeReference(this.OwnerType);
            }
            if (this.GenericParamters != null && this.GenericParamters.Count > 0)
            {
                for (int iCount = 0; iCount < this.GenericParamters.Count; iCount++)
                {
                    this.GenericParamters[iCount] = document.CacheTypeReference(this.GenericParamters[iCount]);
                }
            }
            if (this.Paramters != null && this.Paramters.Count > 0)
            {
                foreach (var item in this.Paramters)
                {
                    item.ValueType = document.CacheTypeReference(item.ValueType);
                }
            }
        }
        public void UpdateLocalInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            UpdateLocalInfo(this.OwnerType?.LocalClass);

        }
        public void UpdateLocalInfo(DCILClass cls)
        {
            if (DCILMethod.IsCtorOrCctor(this.MethodName))
            {
                return;
            }
            if (this.OwnerType.IsArray)
            {
                return;
            }
            if( this.MethodName == "get_IsQuiescent")
            {

            }
            if (cls != null)
            {
                //Dictionary<string, DCILTypeReference> gpValues = null;
                DCILGenericParamterList gps = cls.GenericParamters;
                if (this.OwnerType.IsGenericType)
                {
                    //if( gpValues == null )
                    //{
                    //    gpValues = new Dictionary<string, DCILTypeReference>();
                    //}
                    if (this.OwnerType.LocalClass != null)
                    {
                        gps = this.OwnerType.LocalClass.GenericParamters;
                    }
                    else
                    {
                        gps = DCILGenericParamterList.CreateByNativeType(this.OwnerType.SearchNativeType());
                    }
                    if (gps != null)
                    {
                        gps.SetRuntimeType(this.OwnerType.GenericParamters);
                    }
                }
                foreach (var item in cls.ChildNodes)
                {
                    if (item is DCILMethod && item.Name == this.MethodName)
                    {
                        var method = (DCILMethod)item;
                        if (DCUtils.EqualsListCount(method.Parameters, this.Paramters)
                            && DCUtils.EqualsListCount(method.GenericParamters, this.GenericParamters))
                        {
                            if (method.HasGenericStyle)
                            {
                                var mgps = method.GenericParamters;
                                if (mgps != null && mgps.Count > 0)
                                {
                                    mgps.SetRuntimeType(this.GenericParamters);
                                    gps = DCILGenericParamterList.Merge(gps, mgps);
                                }
                            }
                            if (this.SimpleMode == false && method.ReturnTypeInfo.EqualsValue(this.ReturnType, gps , false) == false)
                            {
                                continue;
                            }
                            if (this.Paramters != null && this.Paramters.Count > 0)
                            {
                                bool flag = true;
                                for (int iCount = this.Paramters.Count - 1; iCount >= 0; iCount--)
                                {
                                    if (method.Parameters[iCount].ValueType.EqualsValue(this.Paramters[iCount].ValueType, gps , false) == false)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    this.LocalMethod = method;
                                    break;
                                }
                            }
                            else
                            {
                                this.LocalMethod = method;
                                break;
                            }
                        }
                    }
                }
            }
            if (this.LocalMethod == null && this.OwnerType.HasLibraryName == false)
            {
                // 未找到本地IL中定义的方法。
            }
        }

        public override bool Equals(object obj)
        {
            return EqualsValue(obj as DCILInvokeMethodInfo);
        }
        public bool EqualsValue(DCILInvokeMethodInfo info2)
        {
            if (info2 == null)
            {
                return false;
            }
            if (info2 == this)
            {
                return true;
            }
            if (this.MethodName != info2.MethodName)
            {
                return false;
            }
            int len1 = this.Styles == null ? 0 : this.Styles.Count;
            int len2 = info2.Styles == null ? 0 : info2.Styles.Count;
            if (len1 != len2)
            {
                return false;
            }
            for (int iCount = 0; iCount < len1; iCount++)
            {
                if (this.Styles[iCount] != info2.Styles[iCount])
                {
                    return false;
                }
            }
            if (DCILTypeReference.StaticEquals(this.ReturnType, info2.ReturnType) == false)
            {
                return false;
            }
            if (DCILTypeReference.StaticEquals(this.OwnerType, info2.OwnerType) == false)
            {
                return false;
            }
            if (DCUtils.EqualsList<DCILTypeReference>(this.GenericParamters, info2.GenericParamters) == false)
            {
                return false;
            }
            if (DCILMethodParamter.EqualsList(this.Paramters, info2.Paramters, false, false) == false)
            {
                return false;
            }
            return true;
        }
        private int _HashCode = 0;
        public override int GetHashCode()
        {
            if (this._HashCode == 0)
            {
                if (this.Styles != null && this.Styles.Count > 0)
                {
                    foreach (var item in this.Styles)
                    {
                        this._HashCode += item.GetHashCode();
                    }
                }
                this._HashCode += this.IsInstance.GetHashCode();
                if (this.ReturnType != null)
                {
                    this._HashCode += this.ReturnType.GetHashCode();
                }
                if (this.OwnerType != null)
                {
                    this._HashCode += this.OwnerType.GetHashCode();
                }
                if (this.MethodName != null && this.MethodName.Length > 0)
                {
                    this._HashCode += this.MethodName.GetHashCode();
                }
                if (this.GenericParamters != null)
                {
                    foreach (var item in this.GenericParamters)
                    {
                        this._HashCode += item.GetHashCode();
                    }
                }
                if (this.Paramters != null)
                {
                    foreach (var item in this.Paramters)
                    {
                        this._HashCode += item.ComputeHashCode(true);
                    }
                }
            }
            return this._HashCode;
        }

        public string GetSignString(bool addMethodName)
        {
            return DCILMethod.InnerGetSignString(
                this.ReturnType,
                addMethodName ? this.MethodName : null,
                this.GenericParamters,
                this.OwnerType.LocalClass?.GenericParamters,
                this.Paramters);
        }

        public List<string> Styles = null;
        public bool IsInstance = false;
        public DCILTypeReference ReturnType = DCILTypeReference.Type_Void;
        public DCILTypeReference OwnerType = null;
        public string MethodName = null;
        private DCILMethod _LocalMethod = null;
        public DCILMethod LocalMethod
        {
            get
            {
                return this._LocalMethod;
            }
            set
            {
                this._LocalMethod = value;
            }
        }
        public List<DCILTypeReference> GenericParamters = null;
        public List<DCILMethodParamter> Paramters = null;
        public int ParametersCount
        {
            get
            {
                if (this.Paramters == null)
                {
                    return 0;
                }
                else
                {
                    return this.Paramters.Count;
                }
            }
        }

        public void WriteTo(DCILWriter writer)
        {
            var strMethodName = this.MethodName;
            if (this.LocalMethod != null)
            {
                strMethodName = this.LocalMethod.Name;
            }
            if (this.SimpleMode)
            {
                this.OwnerType.WriteTo(writer);
                writer.Write("::");
                writer.Write(strMethodName);
                return;
            }
            if (this.IsInstance)
            {
                writer.Write("instance ");
            }
            if (this.ReturnType != null && this.ReturnType.IsPrimitive)
            {
                this.ReturnType.WriteTo(writer);
            }
            else
            {
                var lmt = this.LocalMethod?.ReturnTypeInfo;
                if (lmt != null
                    && lmt.Mode != DCILTypeMode.GenericTypeInMethodDefine
                    && lmt.Mode != DCILTypeMode.GenericTypeInTypeDefine
                    && lmt.IsGenericType == false)
                {

                    lmt.WriteTo(writer);
                }
                else
                {
                    this.ReturnType.WriteTo(writer);
                }
            }
            writer.Write(' ');
            var cls = this.LocalMethod?.OwnerClass;
            if (cls != null && cls.HasGenericParamters == false)
            {
                writer.Write(cls.NameWithNested);
            }
            else
            {
                this.OwnerType.WriteTo(writer);
            }
            writer.Write("::");
            writer.Write(strMethodName);
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
                }
                writer.Write('>');
            }
            writer.Write('(');
            var ps = (this.LocalMethod == null || this.LocalMethod.HasGenericStyle )? this.Paramters : this.LocalMethod.Parameters;
            if (ps != null && ps.Count > 0)
            {
                bool hasAdd = false;
                foreach (var item in ps)
                {
                    if (hasAdd)
                    {
                        writer.Write(',');
                    }
                    hasAdd = true;
                    //if (item.IsClassValueType)
                    //{
                    //    str.Append("class ");
                    //}
                    item.ValueType.WriteTo(writer);
                    //if (item.Name != null && item.Name.Length > 0)
                    //{
                    //    writer.Write(' ');
                    //    writer.Write(item.Name);
                    //}
                }
            }
            writer.Write(')');
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            WriteTo(new DCILWriter(str));
            return str.ToString();
        }
    }
}
