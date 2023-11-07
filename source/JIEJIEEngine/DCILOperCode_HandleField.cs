/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */

namespace JIEJIE
{
    /// <summary>
    /// 处理类型字段的指令
    /// </summary>
    internal class DCILOperCode_HandleField : DCILOperCode
    {
        public DCILOperCode_HandleField(string labelID, string operCode, DCILReader reader)
        {
            this.LabelID = labelID;
            this.SetOperCode( operCode);
            this._Value = new DCILFieldReference(reader);
        }
        public DCILOperCode_HandleField(string labelID, DCILOperCodeDefine vdef , DCILReader reader)
        {
            this.LabelID = labelID;
            this._Define = vdef;
            this._Value = new DCILFieldReference(reader);
        }
        public DCILOperCode_HandleField(string labelID, string operCode, DCILFieldReference field )
        {
            this.LabelID = labelID;
            this.SetOperCode( operCode);
            this._Value = field;
            //this.LocalField = field.LocalField;
        }
        public DCILOperCode_HandleField(string labelID, DCILOperCodeDefine vdef, DCILFieldReference field)
        {
            this.LabelID = labelID;
            this._Define = vdef;
            this._Value = field;
            //this.LocalField = field.LocalField;
        }
        public override void Dispose()
        {
            base.Dispose();
            this._Value = null;
            //this.LocalField = null;
        }
        public override string ToString()
        {
            return this.StackOffset + "#" + this.LabelID + " : " + this.OperCode + " " + this._Value.ToString();
        }
        private DCILFieldReference _Value = null;
        public DCILFieldReference Value
        {
            get
            {
                return this._Value;
            }
        }
        //public DCILField LocalField = null;
        //{
        //    get
        //    {
        //        return this._Value.LocalField;
        //    }
        //}
        public void CacheInfo(DCILDocument document)
        {
            this._Value = document.CacheFieldReference(this._Value);
        }

        public override void WriteOperData(DCILWriter writer)
        {
            //var lf = this.LocalField;
            //if (lf != null)
            //{
            //    writer.Write("  ");
            //    if (this._Value.ValueType.Mode == DCILTypeMode.GenericTypeInMethodDefine ||
            //        this._Value.ValueType.Mode == DCILTypeMode.GenericTypeInTypeDefine)
            //    {
            //        this._Value.ValueType.WriteTo(writer);
            //    }
            //    else
            //    {
            //        lf.ValueType.WriteTo(writer);
            //    }
            //    if (this._Value != null && this._Value.modreq != null && this._Value.modreq.Length > 0)
            //    {
            //        writer.Write(" modreq(");
            //        writer.Write(this._Value.modreq);
            //        writer.Write(") ");
            //    }
            //    writer.Write(' ');
            //    writer.Write(((DCILClass)lf.Parent).NameWithNested);
            //    writer.Write("::");
            //    writer.Write(lf.Name);
            //}
            //else if (this._Value != null)
            {
                this._Value.WriteTo(writer);
            }
        }
    }
}
