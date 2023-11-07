/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;
using System.Text;

namespace JIEJIE
{
    internal class DCILData : DCILObject
    {
        public const string TagName_Data = ".data";
        public DCILData()
        {

        }
        public DCILData(DCILReader reader)
        {
            var h3 = SelfPerformanceCounterForTest.Enter(SelfPerformanceCounterForTest.MethodIndexs.LoadILData);
            var strHeader = reader.ReadAfterCharExcludeLastChar('=');
            var words = DCUtils.SplitByWhitespace(strHeader);
            this._Name = words[words.Count - 1];
            this.IsCil = words.Contains("cil");
            this.DataType = reader.ReadWord();
            int lineindex = reader.CurrentLineIndex();
            if (this.DataType == "bytearray")
            {
                this.Value = reader.ReadBinaryFromHex();
            }
            else
            {
                this.Value = reader.ReadLineTrimRemoveComment();
            }
            SelfPerformanceCounterForTest.Leave(h3);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.DataType = null;
            this.Value = null;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Data;
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".data cil " + this._Name + " = " + this.DataType);
            if (this.DataType == "bytearray")
            {
                var bs = (byte[])this.Value;
                if (bs.Length > 16)
                {
                    writer.WriteLine("(");
                }
                else
                {
                    writer.Write("(");
                }
                if( this.XORKey > 0 )
                {
                    var bsNative = (byte[])this.Value;
                    bs = (byte[]) bs.Clone();
                    XORDatas(bs, this.XORKey);
                    var bs2 = (byte[])bs.Clone();
                    XORDatas(bs2, this.XORKey);
                    for(int iCount = bs.Length -1; iCount >=0; iCount --)
                    {
                        if(bsNative[ iCount ] != bs2[iCount])
                        {

                        }
                    }
                }
                writer.WriteHexs(bs);
                writer.WriteLine(")");
            }
            else
            {
                writer.WriteLine(Convert.ToString(this.Value));
            }
        }

        private unsafe void XORDatas(byte[] v, int encKey)
        {
            if (v.Length >= 4 && encKey != 0)
            {
                fixed (byte* pByte = v)
                {
                    int* pStart = (int*)pByte;
                    int* pEnd = pStart + (int)Math.Floor(v.Length / 4.0) - 1;
                    while (pEnd >= pStart)
                    {
                        *pEnd = *pEnd ^ encKey;
                        encKey += 13;
                        pEnd--;
                    }
                }
            }
        }

        public int XORKey = 0;

        public string DataType = null;
        public object Value = null;
        public bool IsCil = true;
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(".data");
            if (this.IsCil)
            {
                str.Append(" cil");
            }
            str.Append(this._Name);
            str.Append(" = " + this.DataType);
            if (this.Value is byte[])
            {
                str.Append(" " + DCUtils.FormatByteSize(((byte[])this.Value).Length));
            }
            else
            {
                str.Append(" " + Convert.ToString(this.Value));
            }
            return str.ToString();
        }
    }

}
