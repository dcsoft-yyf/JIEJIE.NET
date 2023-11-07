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
    /// 处理成员方法的指令
    /// </summary>
    internal class DCILOperCode_HandleMethod : DCILOperCode
    {
        //private static readonly DCILOperCodeDefine _Define_Call = DCILOperCodeDefine.GetDefine("call");
        //private static readonly DCILOperCodeDefine _Define_Callvirt = DCILOperCodeDefine.GetDefine("callvirt");
        //private static readonly DCILOperCodeDefine _Define_NewObj = DCILOperCodeDefine.GetDefine("newobj");
        //private static readonly DCILOperCodeDefine _Define_Ldftn = DCILOperCodeDefine.GetDefine("ldftn");
        //private static readonly DCILOperCodeDefine _Define_ldvirtftn = DCILOperCodeDefine.GetDefine("ldvirtftn");
        public DCILOperCode_HandleMethod()
        {

        }
        public override void SetOperCode(string code)
        {
            if( code == "call")
            {
                base._Define = DCILOperCodeDefine._call ;
            }
            else if( code == "callvirt")
            {
                base._Define = DCILOperCodeDefine._callvirt;
            }
            else if(code == "newobj")
            {
                base._Define = DCILOperCodeDefine._newobj;
            }
            else if(code == "ldftn")
            {
                base._Define = DCILOperCodeDefine._ldftn;
            }
            else if( code == "ldvirtftn")
            {
                base._Define = DCILOperCodeDefine._ldvirtftn;
            }
            else
            {
                base._Define = DCILOperCodeDefine.GetDefine(code);
            }
        }
        public DCILInvokeMethodInfo InvokeInfo = null;
        /// <summary>
        /// 是否匹配类型名称和函数名称
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="methodName">函数名称</param>
        /// <returns>是否匹配</returns>
        public bool MatchTypeAndMethod( string typeName , string methodName , int parameterCount )
        {
            return this.InvokeInfo.OwnerType.Name == typeName 
                && this.InvokeInfo.MethodName == methodName
                && this.InvokeInfo.ParametersCount == parameterCount ;
        }

        public void ChangeTarget(DCILTypeReference type, string methodName, List<DCILMethodParamter> ps = null )
        {
            var info = this.InvokeInfo.Clone();
            info.OwnerType = type;
            info.MethodName = methodName;
            this.LocalMethod = null;
            foreach (var item in type.LocalClass.ChildNodes)
            {
                if (item is DCILMethod && item.Name == methodName)
                {
                    var method = (DCILMethod)item;
                    if (ps != null && ps.Count > 0)
                    {
                        if (ps.Count == method.ParametersCount 
                            || DCILMethodParamter.EqualsList(ps, method.Parameters, false, false) == false)
                        {
                            continue;
                        }
                    }
                    info.LocalMethod = method;
                    info.Paramters = method.Parameters;
                    this.LocalMethod = method;
                    info.IsInstance = method.IsInstance;
                    break;
                }
            }
            if( this.LocalMethod == null )
            {
                throw new NotSupportedException(type.Name + "::" + methodName);
            }
            //this.InvokeInfo.LocalMethod = this.LocalMethod;
            this.InvokeInfo = info;
        }

        public DCILMethod LocalMethod = null;
        public DCILOperCode_HandleMethod(string labelID , string code , DCILMethod method )
        {
            this.LabelID = labelID;
            this.SetOperCode(code);
            this.InvokeInfo = new DCILInvokeMethodInfo( method );
            this.LocalMethod = method;
        }
        public DCILOperCode_HandleMethod(string labelID, string code, DCILInvokeMethodInfo method)
        {
            this.LabelID = labelID;
            this.SetOperCode(code);
            this.InvokeInfo = method;
            this.LocalMethod = this.InvokeInfo.LocalMethod;
        }
        //public DCILOperCode_HandleMethod(string code, DCILReader reader)
        //{
        //    this.SetOperCode(code);
        //    this.InvokeInfo = new DCILInvokeMethodInfo(reader);
        //}

        public DCILOperCode_HandleMethod(string labelID , DCILOperCodeDefine vdef , DCILReader reader)
        {
            this.LabelID = labelID;
            this._Define = vdef;
            this.InvokeInfo = new DCILInvokeMethodInfo(reader);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.LocalMethod = null;
            this.InvokeInfo = null;
        }
        public DCILOperCode_HandleMethod CacheInfo(DCILDocument document)
        {
            this.InvokeInfo = document.CacheDCILInvokeMethodInfo(this.InvokeInfo);
            return null;
        }
        public override int StackOffset
        {
            get
            {
                if( this._Define == DCILOperCodeDefine._ldftn || this._Define == DCILOperCodeDefine._ldvirtftn)
                {
                    return 1;
                }
                if(this.InvokeInfo != null )
                {
                    var lm = this.InvokeInfo.LocalMethod;
                    if( lm != null )
                    {
                        int result = 0;
                        if (this._Define == DCILOperCodeDefine._newobj)
                        {
                            result = 1;
                        }
                        else
                        {
                            if (lm.ReturnTypeInfo != DCILTypeReference.Type_Void)
                            {
                                result = 1;
                            }
                            if (lm.IsInstance)
                            {
                                result--;
                            }
                        }
                        if(lm.Parameters != null )
                        {
                            result -= lm.Parameters.Count;
                        }
                        return result;
                    }
                    else
                    {
                        int result = 0;
                        
                        if( this._Define == DCILOperCodeDefine._newobj)
                        {
                            result = 1;
                        }
                        else
                        {
                            if (this.InvokeInfo.ReturnType != DCILTypeReference.Type_Void)
                            {
                                result = 1;
                            }
                            if (this.InvokeInfo.IsInstance)
                            {
                                result--;
                            }
                        }
                        if( this.InvokeInfo.Paramters != null )
                        {
                            result -= this.InvokeInfo.Paramters.Count;
                        }
                        return result;
                    }
                }
                return 0;
            }
        }

        public override void WriteTo(DCILWriter writer)
        {
            //if (this.InvokeInfo.MethodName == "MyDispose" && this.OperCode == "callvirt")
            //{

            //}
            writer.EnsureNewLine();
            writer.Write(this.LabelID);
            writer.Write(": ");
            if (this.LabelID.Length < 10)
            {
                writer.WriteWhitespace(10 - this.LabelID.Length);
            }
            //if( this.InvokeInfo.MethodName == "Object_ToString")
            //{

            //}
            writer.Write(this.OperCode);
            if (this.InvokeInfo != null)
            {
                writer.WriteWhitespace(Math.Max(1, 10 - this.OperCode.Length));
                this.InvokeInfo.WriteTo(writer);
            }

        }
        public override void WriteOperData(DCILWriter writer)
        {
            if (this.InvokeInfo != null)
            {
                writer.Write(" ");
                this.InvokeInfo.WriteTo(writer);
            }
        }
        public override string ToString()
        {
            var str = new StringBuilder( this.StackOffset +"#" + this.LabelID + ":" + this.OperCode + " " + this.InvokeInfo.OwnerType.Name + "::" + this.InvokeInfo.MethodName);
            if(this.InvokeInfo.ParametersCount > 0 )
            {
                str.Append("(");
                for(int iCount = 0; iCount < this.InvokeInfo.ParametersCount; iCount ++)
                {
                    if(iCount > 0 )
                    {
                        str.Append(',');
                    }
                    str.Append(this.InvokeInfo.Paramters[iCount].ValueType.Name);
                }
                str.Append(")");
            }
            return str.ToString();
        }
    }
}
