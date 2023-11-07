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
    /// 重命名状态
    /// </summary>
    internal enum DCILRenameState
    {
        /// <summary>
        /// 未处理
        /// </summary>
        NotHandled,
        /// <summary>
        /// 需要重命名，但还未执行
        /// </summary>
        NeedRename ,
        /// <summary>
        /// 已经重命名了
        /// </summary>
        Renamed,
        /// <summary>
        /// 需要保留名称，不重命名
        /// </summary>
        Preserve
    }
}
