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

namespace JIEJIE
{
    internal class DCILOperCode_Switch : DCILOperCode
    {
        //private static readonly DCILOperCodeDefine _SrcInfo = DCILOperCodeDefine.GetDefine("switch");

        public DCILOperCode_Switch(string labelID )
        {
            this.LabelID = labelID;
            this._Define = DCILOperCodeDefine._switch;
        }

        public DCILOperCode_Switch(string labelID, DCILReader reader)
        {
            this.LabelID = labelID;
            this._Define = DCILOperCodeDefine._switch;
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == ")")
                {
                    break;
                }
                if (strWord.StartsWith("IL_", StringComparison.OrdinalIgnoreCase))
                {
                    this.TargetLabels.Add(strWord);
                }
            }
        }
        public override int ByteSize
        {
            get
            {
                return base.ByteSize + this.TargetLabels.Count * 4 + 4;
            }
        }

        public List<string> TargetLabels = new List<string>();
        public override void WriteOperData(DCILWriter writer)
        {
            writer.Write("(");
            var len = this.TargetLabels.Count;
            for (int iCount = 0; iCount < len; iCount++)
            {
                if (iCount > 0)
                {
                    writer.Write(',');
                }
                writer.Write(this.TargetLabels[iCount]);
            }
            writer.WriteLine(")");
        }
    }
}
