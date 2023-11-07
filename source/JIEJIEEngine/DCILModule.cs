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
    internal class DCILModule : DCILObject
    {
        //public DCILModule()
        //{

        //}
        public DCILModule(DCILReader reader)
        {
            this.Load(reader);
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Module;
            }
        }
        public const string TagName_Module = ".module";
        public bool IsExtern = false;
        public override void Load(DCILReader reader)
        {
            string v = reader.ReadWord();
            if (v == "extern")
            {
                this.IsExtern = true;
                this._Name = reader.ReadWord();
            }
            else
            {
                this.IsExtern = false;
                this._Name = v;
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.WriteLine(this.ToString());
        }
        public override string ToString()
        {
            if (this.IsExtern)
            {
                return ".module extern " + this._Name;
            }
            else
            {
                return ".module " + this._Name;
            }
        }
    }
}
