/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;
using System.Collections.Generic;

namespace JIEJIE
{
    internal class DCILMethodLocalVariableList : List<DCILMethodLocalVariable> , IDisposable
    {
        public bool HasInit = true;
        public void Dispose()
        {
            foreach( var item in this )
            {
                item.Name = null;
                item.ValueType = null;
            }
            this.Clear();
        }
    }
}
