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
    internal class DCInterfaceimpl : DCILObject
    {
        public const string TagName = ".interfaceimpl";

        public DCInterfaceimpl(DCILReader reader)
        {
            this.InterfaceType = reader.ReadWord();
            //string tn = reader.ReadWord();
            this.RefType = DCILTypeReference.Load(reader);
            if( this.RefType.Mode == DCILTypeMode.Class)
            {
                this.HasHeaderType = true;
            }
        }

        public void UpdateLocalInfo(ReadCustomAttributeValueArgs args)
        {
            //var doc = args.GetDocument(this.RefType.LibraryName);
            //if (doc != null)
            //{
            //    this.RefType.UpdateLocalClass(doc.GetAllClassesUseCache());
            //}
        }
        private bool HasHeaderType = false;

        public string InterfaceType = null;

        public DCILTypeReference RefType = null;
        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(TagName);
            writer.Write(" " + this.InterfaceType + " ");
            this.RefType.WriteTo(writer, this.HasHeaderType);
            writer.WriteLine();
        }
    }

}
