/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System.Text;

namespace JIEJIE
{
    internal class DCILMethodLocalVariable
    {
        public int Index = int.MinValue;
        public string Name = null;
        public DCILTypeReference ValueType = null;
        public override string ToString()
        {
            var str = new StringBuilder();
            if( this.Index >= 0 )
            {
                str.Append("[" + this.Index + "]");
            }
            str.Append(this.ValueType.ToString());
            if(this.Name != null && this.Name.Length > 0 )
            {
                str.Append(" " + this.Name);
            }
            return str.ToString();
        }
    }
}
