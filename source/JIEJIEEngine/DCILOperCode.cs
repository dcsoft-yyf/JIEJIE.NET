/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;

namespace JIEJIE
{
    internal class DCILOperCode : IDisposable
    {
        public static DCILOperCode Gen_ldci4_Code(string labelID, int v)
        {
            switch (v)
            {
                case 0: return new DCILOperCode(labelID, "ldc.i4.0");
                case 1: return new DCILOperCode(labelID, "ldc.i4.1");
                case 2: return new DCILOperCode(labelID, "ldc.i4.2");
                case 3: return new DCILOperCode(labelID, "ldc.i4.3");
                case 4: return new DCILOperCode(labelID, "ldc.i4.4");
                case 5: return new DCILOperCode(labelID, "ldc.i4.5");
                case 6: return new DCILOperCode(labelID, "ldc.i4.6");
                case 7: return new DCILOperCode(labelID, "ldc.i4.7");
                case 8: return new DCILOperCode(labelID, "ldc.i4.8");
                case -1: return new DCILOperCode(labelID, "ldc.i4.m1");
                default:
                    if (v > -127 && v < 128)
                    {
                        return new DCILOperCode(labelID, "ldc.i4.s", DCUtils.GetInt32ValueString(v));
                    }
                    else
                    {
                        return new DCILOperCode(labelID, "ldc.i4", DCUtils.GetInt32ValueString(v));
                    }
            }
        }

        ///// <summary>
        ///// 判断是否存在指令数据
        ///// </summary>
        ///// <param name="strOperCode"></param>
        ///// <returns></returns>
        //public static bool WithoutOperData(string strOperCode)
        //{
        //    return DCILOperCodeDefine.GetDefine(strOperCode).WithoutOperData;
        //}
           
        /// <summary>
        /// 获得指令类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        //public static ILOperCodeType GetOperCodeType(string code)
        //{
        //    return DCILOperCodeDefine.GetDefine(code).ExtCodeType;
             
        //}
        public DCILOperCode()
        {

        }
        public DCILOperCode(string vlabelID, string voperCode, string voperData = null )
        {
            this.LabelID = vlabelID;
            this.SetOperCode( voperCode);
            this.OperData = voperData;
        }

        public DCILOperCode(string vlabelID, DCILOperCodeDefine myDefine, string voperData = null )
        {
            if( myDefine == null )
            {
                throw new ArgumentNullException("myDefine");
            }
            this.LabelID = vlabelID;
            this._Define = myDefine;
            this.OperData = voperData;
        }

        protected DCILOperCodeDefine _Define = null;
        public DCILOperCodeDefine NativeDefine
        {
            get
            {
                return this._Define;
            }
        }
        public virtual void Dispose()
        {
            this._Define = null;
            this.OperData = null;
        } 

        //public bool BitSizeChanged = false;

        public virtual int StackOffset
        {
            get
            {
                return this._Define.StackOffset;
            }
        }

        //public bool IsJumpOperCode()
        //{
        //    return this._Define.ExtCodeType == ILOperCodeType.Jump 
        //        || this._Define.ExtCodeType == ILOperCodeType.JumpShort;
        //}

        /// <summary>
        /// 判断是否为修饰性指令，必须紧跟在后面的指令之前
        /// </summary>
        /// <returns></returns>
        public bool IsPrefixOperCode()
        {
            return this._Define.IsPrefix;
            //return this.OperCodeValue == DCILOpCodeValue.Volatile_
            //    || this.OperCodeValue == DCILOpCodeValue.Constrained_
            //    //|| operCode == "cpblk"
            //    || this.OperCodeValue == DCILOpCodeValue.Unaligned_
            //    || this.OperCodeValue == DCILOpCodeValue.Tailcal;// == "tailcall";
        }
        public virtual void WriteTo(DCILWriter writer)
        {
            writer.EnsureNewLine();
            writer.Write(this.LabelID);
            writer.Write(": ");
            if (this.LabelID.Length < 10)
            {
                writer.WriteWhitespace(10 - this.LabelID.Length);
            }
            writer.Write(this.OperCode);
            WriteOperData(writer);

        }
        public virtual void WriteOperData(DCILWriter writer)
        {
            if (this.OperData != null && this.OperData.Length > 0)
            {
                writer.WriteWhitespace(Math.Max(1, 10 - this.OperCode.Length));
                writer.Write(this.OperData);
            }
        }
        
        //private static int _InstanceIndexCounter = 0;
        //public int InstanceIndex = _InstanceIndexCounter++;

        //public bool Enabled = true;
        //public DCILOperCodeList OwnerList = null;
        //public DCILMethod OwnerMethod = null;
        //public string NativeSource = null;
        public string LabelID = null;
        public bool HasLabelID()
        {
            return this.LabelID != null && this.LabelID.Length > 0;
        }
        //private DCILOpCodeIntegerValue _OperCodeIntegerValue = DCILOpCodeIntegerValue.Nop;
        //public DCILOpCodeIntegerValue  OperCodeIntegerValue
        //{
        //    get
        //    {
        //        return this._OperCodeIntegerValue;
        //    }
        //}

        /// <summary>
        /// 字节长度
        /// </summary>
        public virtual int ByteSize
        {
            get
            {
                return this._Define.Size;
            }
        }

        public string OperCode
        {
            get
            {
                return this._Define.Name;
            }
        }

        public DCILOpCodeValue OperCodeValue
        {
            get
            {
                return (DCILOpCodeValue)this._Define.Value;
            }
        }

        public virtual void SetOperCode( string code )
        {
            this._Define = DCILOperCodeDefine.GetDefine(code);
        }

        public string OperData = null;
        //public int LineIndex = 0;
        //public int EndLineIndex = 0;
        public virtual DCILOperCode Clone(string newLabelID)
        {
            var result = (DCILOperCode)this.MemberwiseClone();
            result.LabelID = newLabelID;
            return result;
        }
        public override string ToString()
        {
            if (this.OperData == null || this.OperData.Length == 0)
            {
                return this.StackOffset + "#" + this.LabelID + " : " + this.OperCode;
            }
            else
            {
                return this.StackOffset + "#" + this.LabelID + " : " + this.OperCode + "     " + this.OperData;
            }
        }
        ///// <summary>
        ///// get IL opercode from a IL code line
        ///// </summary>
        ///// <param name="line">IL code line</param>
        ///// <param name="labelID">label id</param>
        ///// <param name="operData">opertion data</param>
        ///// <returns></returns>
        //internal static string GetILCode(string line, ref string labelID, ref string operData)
        //{
        //    int len = line.Length;
        //    for (int iCount = 0; iCount < len; iCount++)
        //    {
        //        char c = line[iCount];
        //        if (c == ':')
        //        {
        //            labelID = line.Substring(0, iCount).Trim();
        //            for (iCount++; iCount < len; iCount++)
        //            {
        //                var c2 = line[iCount];
        //                if (c2 != ' ' && c2 != '\t')
        //                {
        //                    string operCode = null;
        //                    for (int iCount2 = iCount + 1; iCount2 < len; iCount2++)
        //                    {
        //                        var c3 = line[iCount2];
        //                        if (c3 == ' ' || c3 == '\t')
        //                        {
        //                            operCode = line.Substring(iCount, iCount2 - iCount);
        //                            if (iCount2 < len - 1)
        //                            {
        //                                operData = line.Substring(iCount2).Trim();
        //                            }
        //                            break;
        //                        }
        //                    }
        //                    if (operCode == null)
        //                    {
        //                        operCode = line.Substring(iCount);
        //                    }
        //                    return operCode;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}
