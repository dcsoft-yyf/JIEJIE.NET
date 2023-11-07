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
    /// <summary>
    /// options of protect
    /// </summary>
    [Serializable]
    internal class JieJieSwitchs : System.MarshalByRefObject
    {
        public JieJieSwitchs()
        {

        }
        public JieJieSwitchs(string args, JieJieSwitchs baseOptions , object parentObject )
        {
            if (baseOptions != null)
            {
                this.ControlFlow = baseOptions.ControlFlow;
                this.Resources = baseOptions.Resources;
                this.Strings = baseOptions.Strings;
                this.HightStrings = baseOptions.HightStrings;
                if (parentObject is DCILClass)
                {
                    this.AllocationCallStack = baseOptions.AllocationCallStack;
                }
            }
            if (args != null)
            {
                var items = args.Split(',');
                foreach (var item in items)
                {
                    var item2 = item.Trim().ToLower();
                    switch (item2)
                    {
                        case "+contorlflow": this.ControlFlow = true; break;
                        case "-controlflow": this.ControlFlow = false; break;
                        case "+strings": this.Strings = true; break;
                        case "-strings": this.Strings = false; break;
                        case "+resources": this.Resources = true; break;
                        case "-resources": this.Resources = false; break;
                        case "+allocationcallstack": this.AllocationCallStack = true; break;
                        case "-allocationcallstack": this.AllocationCallStack = false; break;
                        case "+memberorder": this.MemberOrder = true; break;
                        case "-memberorder": this.MemberOrder = false; break;
                        case "+rename": this.Rename = true; break;
                        case "-rename": this.Rename = false; break;
                        case "+removemember": this.RemoveMember = true; break;
                        case "-removemember": this.RemoveMember = false; break;
                        case "+hightstrings": this.HightStrings = true;break;
                        case "-hightstrings": this.HightStrings = false;break;
                    }
                }
            }
        }
        /// <summary>
        /// 混淆代码执行流程
        /// </summary>
        public bool ControlFlow = true;
        /// <summary>
        /// 加密字符串
        /// </summary>
        public bool Strings = true;
        /// <summary>
        /// 高度加密字符串,但会拖累性能。
        /// </summary>
        public bool HightStrings = false;
        /// <summary>
        /// 加密资源文件
        /// </summary>
        public bool Resources = true;
        /// <summary>
        /// 隐藏字符串创建调用堆栈
        /// </summary>
        public bool AllocationCallStack = false;
        /// <summary>
        /// 混淆类型成员顺序
        /// </summary>
        public bool MemberOrder = true;
        /// <summary>
        /// 重命名
        /// </summary>
        public bool Rename = true;
        /// <summary>
        /// 删除无作用的类型成员
        /// </summary>
        public bool RemoveMember = true;
    }
}
