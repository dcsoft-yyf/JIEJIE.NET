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
    internal class DCILOperCodeComment : DCILOperCode
    {
        public DCILOperCodeComment()
        {

        }
        public DCILOperCodeComment(string txt)
        {
            this.Text = txt;
        }
        public override void WriteTo(DCILWriter writer)
        {
            if (this.Text != null && this.Text.Length > 0)
            {
                writer.WriteLine(Environment.NewLine + "//" + this.Text);
            }
        }
        public override string ToString()
        {
            return "//" + this.Text;
        }
        public string Text = null;
        public override void Dispose()
        {
            base.Dispose();
            this.Text = null;
        }
    }
}
