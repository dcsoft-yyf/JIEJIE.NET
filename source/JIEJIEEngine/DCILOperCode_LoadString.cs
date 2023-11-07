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
    internal class DCILOperCode_LoadString : DCILOperCode
    {
        public DCILOperCode_LoadString(string labelID, DCILReader reader)
        {
            this.LabelID = labelID;
            this._Define = DCILOperCodeDefine._ldstr;
            var info = reader.ReadStringValue();
            this.Value = info.Value;
            this.IsBinary = info.IsBinary;
            this.OperData = info.ILRawText;
            this.BianryData = info.BianryData;
        }

        public DCILOperCode_LoadString( string labelID , string text )
        {
            this.LabelID = labelID;
            this._Define = DCILOperCodeDefine._ldstr;
            this.OperData = DCILReader.ToRawILText(text);
            this.IsBinary = this.OperData != null && this.OperData.StartsWith(DCILReader._bytearray, StringComparison.Ordinal);
            this.Value = text;

            //var v = new DCILStringValue(text);
            //this.IsBinary = v.IsBinary;
            //this.Value = v.Value;
            //this.OperData = v.RawILText;
        }

        //public DCILOperCode_LoadString(DCILOperCode code)
        //{
        //    this._Define = DCILOperCodeDefine._ldstr;
        //    this.LabelID = code.LabelID;
        //    this.OperData = code.OperData;
        //    //this.LineIndex = code.LineIndex;
        //    //this.EndLineIndex = code.EndLineIndex;
        //    //this.NativeSource = code.NativeSource;
        //    //this.OwnerList = code.OwnerList;
        //    //this.OwnerMethod = code.OwnerMethod;
        //}
        public override void Dispose()
        {
            base.Dispose();
            this.Value = null;
            this.ReplaceCode = null;
            this.BianryData = null;
        }
        /// <summary>
        /// 字符串值
        /// </summary>
        public string Value = null;
        /// <summary>
        /// 是否采用二进制格式来定义
        /// </summary>
        public bool IsBinary = false;
        /// <summary>
        /// 二进制数据
        /// </summary>
        public byte[] BianryData = null;
        /// <summary>
        /// 是否为了设置静态字段
        /// </summary>
        public bool IsSetStaticField = false;
        /// <summary>
        /// 替换的指令
        /// </summary>
        public DCILOperCode ReplaceCode = null;
        public override void WriteTo(DCILWriter writer)
        {
            if (this.ReplaceCode != null)
            {
                this.ReplaceCode.LabelID = this.LabelID;
                this.ReplaceCode.WriteTo(writer);
            }
            else
            {
                base.WriteTo(writer);
            }
        }
        public override void WriteOperData(DCILWriter writer)
        {
            writer.Write(' ');
            if( this.IsBinary )
            {
                writer.Write(DCILReader._bytearray);
                writer.Write('(');
                writer.WriteHexs(this.BianryData);
                writer.WriteLine(")");
            }
            else
            {
                writer.Write(this.OperData);
            }
        }
    }

}
