/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System.Collections.Generic;

namespace JIEJIE
{
    internal class DCILOperCode_LdToken : DCILOperCode
    {
        //private static readonly DCILOperCodeDefine _SrcInfo = DCILOperCodeDefine.GetDefine("ldtoken");

        public DCILOperCode_LdToken(string labelID, DCILFieldReference field)
        {
            this._Define = DCILOperCodeDefine._ldtoken;
            this.LabelID = labelID;
            this.OperType = "field";
            this.FieldReference = field;
        }
        public DCILOperCode_LdToken(string labelID, DCILTypeReference type)
        {
            this._Define = DCILOperCodeDefine._ldtoken;
            this.LabelID = labelID;
            this.ClassType = type;
        }
        //public const string CodeName_LdToken = "ldtoken";
        public DCILOperCode_LdToken(string labelID, DCILReader reader)
        {
            this._Define = DCILOperCodeDefine._ldtoken;
            this.LabelID = labelID;
            int pos = reader.Position;
            string strWord = reader.ReadWord();
            if (strWord == "field")
            {
                this.OperType = strWord;
                this.FieldReference = new DCILFieldReference(reader);
            }
            else if (strWord == "method")
            {
                this.OperType = "method";
                this.Method = new DCILInvokeMethodInfo(reader);
            }
            else
            {
                reader.Position = pos;
                this.ClassType = DCILTypeReference.Load(reader);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.OperType = null;
            this.FieldReference = null;
            this.Method = null;
            this.ClassType = null;
        }
        public string OperType = null;
        public DCILFieldReference FieldReference = null;
        public DCILInvokeMethodInfo Method = null;
        public DCILTypeReference ClassType = null;

        public DCILMemberInfo LocalMemberInfo = null;
        public void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            if (this.FieldReference != null)
            {
                this.FieldReference.UpdateLocalInfo(document, clses);
            }
            else if (this.Method != null)
            {
                this.Method = document.CacheDCILInvokeMethodInfo(this.Method);
            }
            else if (this.ClassType != null)
            {
                this.ClassType = document.CacheTypeReference(this.ClassType);
            }
        }

        public override void WriteOperData(DCILWriter writer)
        {
            writer.Write(" ");
            if (this.OperType != null)
            {
                writer.Write(" " + this.OperType + " ");
            }
            if (this.LocalMemberInfo is DCILClass)
            {
                writer.Write(" ");
                writer.Write(((DCILClass)this.LocalMemberInfo).NameWithNested);
            }
            else
            {
                if (this.FieldReference != null)
                {
                    this.FieldReference.WriteTo(writer);
                }
                else if (this.Method != null)
                {
                    this.Method.WriteTo(writer);
                }
                else if (this.ClassType != null)
                {
                    this.ClassType.WriteTo(writer,
                        this.ClassType.IsLocalType
                        || this.ClassType.IsGenericType
                        || this.ClassType.IsGenericType
                        || this.ClassType.IsArray);
                }
            }
        }

    }
}
