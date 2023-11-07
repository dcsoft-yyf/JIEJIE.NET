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
    internal class DCILUnknowObject : DCILObject
    {
        //public DCILUnknowObject()
        //{
        //}
        public DCILUnknowObject(string name, DCILReader reader)
        {
            //if( name == "Syste")
            //{

            //}
            this._Name = name;
            this.Data = reader.ReadInstructionContent();
            if (this.Data != null)
            {
                this.Data = this.Data.Trim();
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.EnsureNewLine();
            writer.Write(this._Name);
            writer.Write(' ');
            writer.WriteLine(this.Data);
        }
        public string Data = null;
        public override string ToString()
        {
            return this._Name + " " + this.Data;
        }
        public override void Dispose()
        {
            base.Dispose();
            this.Data = null;
        }
    }
}
