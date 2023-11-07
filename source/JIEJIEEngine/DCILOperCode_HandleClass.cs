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
    /// <summary>
    /// 处理类型的指令
    /// </summary>
    internal class DCILOperCode_HandleClass : DCILOperCode
    {
        public DCILOperCode_HandleClass(string lableID, DCILOperCodeDefine vdef , DCILReader reader)
        {
            this.LabelID = lableID;
            this._Define = vdef;
            this.ClassType = DCILTypeReference.Load(reader);
        }
        public DCILOperCode_HandleClass(string lableID, string operCode, DCILReader reader)
        {
            this.LabelID = lableID;
            this.SetOperCode( operCode);
            this.ClassType = DCILTypeReference.Load(reader);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.ClassType = null;
            this.LocalClass = null;
        }
        public override void WriteOperData(DCILWriter writer)
        {
            if (this.ClassType != null)
            {
                writer.Write(" ");
                this.ClassType.WriteTo(writer);
            }
        }
        internal void UpdateDomState(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            this.ClassType = document.CacheTypeReference(this.ClassType);
            if (this.ClassType != null)
            {
                this.ClassType.UpdateLocalClass(clses);
            }
        }

        public DCILTypeReference ClassType = null;

        public DCILClass LocalClass = null;

    }
}
