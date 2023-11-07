using System;
using System.Collections.Generic;
using System.Text;

namespace JIEJIE
{
    public enum DetectDeadCodeMode
    {
        /// <summary>
        /// 未启用
        /// </summary>
        Disabled,
        /// <summary>
        /// 列出未被使用，没有附加特性的，而且被混淆重命名的函数
        /// </summary>
        Normal,
        /// <summary>
        /// 列出未被使用而且被混淆重命名的函数
        /// </summary>
        All
    }
}
