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
    internal class DCILTypeConverterAttribute : DCILCustomAttribute
    {
        public const string ConstAttributeTypeName = "System.ComponentModel.TypeConverterAttribute";

        public DCILTypeConverterAttribute()
        {
        }
        public override void ParseValues(ReadCustomAttributeValueArgs args)
        {
            base.ParseValues(args);
            FixTypeName(this, args);
        }

        internal static void FixTypeName(DCILCustomAttribute attr, ReadCustomAttributeValueArgs args)
        {
            if (attr.Values != null)
            {
                foreach (var item in attr.Values)
                {
                    if (item.Value is string)
                    {
                        var str = (string)item.Value;
                        item.Value = new DCILCustomAttributeValue.TypeRefInfo(str, args);
                    }
                }
            }
        }

        //public string ConvertTypeName
        //{
        //    get
        //    {
        //        if (this.Values != null && this.Values.Length == 1)
        //        {
        //            return this.Values[0].Value?.ToString();
        //        }
        //        return null;
        //    }
        //}

    }
}
