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
    internal class DCILObfuscationAttribute : DCILCustomAttribute
    {
        public const string ConstAttributeTypeName = "System.Reflection.ObfuscationAttribute";
        public DCILObfuscationAttribute(DCILInvokeMethodInfo myInvokeInfo, byte[] bsValue)
        {
            this.InvokeInfo = myInvokeInfo;
            this.BinaryValue = bsValue;
        }

        public override void ParseValues(ReadCustomAttributeValueArgs args)
        {
            base.ParseValues(args);
            if (this.Values != null)
            {
                foreach (var item in this.Values)
                {
                    switch (item.Name)
                    {
                        case "StripAfterObfuscation": this.StripAfterObfuscation = (bool)item.Value; break;
                        case "Exclude": this.Exclude = (bool)item.Value; break;
                        case "ApplyToMembers": this.ApplyToMembers = (bool)item.Value; break;
                        case "Feature": this.Feature = (string)item.Value; break;
                    }
                }
            }
        }
        public bool StripAfterObfuscation = true;
        public bool Exclude = true;
        public bool ApplyToMembers = true;
        public string Feature = "all";
        public override void Dispose()
        {
            base.Dispose();
            this.Feature = null;
        }
        /// <summary>
        /// 不执行任何操作
        /// </summary>
        /// <returns></returns>
        public override bool UpdateBinaryValueForLocalClassRename()
        {
            return false;
        }
    }
}
