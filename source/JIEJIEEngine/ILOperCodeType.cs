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
    internal enum ILOperCodeType
    {
        Nop,
        Normal,
        Field,
        Method,
        Class,
        Prefix,
        ldstr,
        switch_,
        ldtoken,
        JumpShort,
        Jump,
        ArgsOrLocalsByName,
        LoadNumberByOperData
    }

}
