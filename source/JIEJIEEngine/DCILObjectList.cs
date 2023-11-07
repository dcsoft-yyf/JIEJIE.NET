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
    internal class DCILObjectList : List<DCILObject> , IDisposable
    {
        public DCILObjectList()
        {

        }
        public void Dispose()
        {
            foreach( var item in this )
            {
                item.Dispose();
            }
            this.Clear();
        }
        //public DCILObjectList Clone()
        //{
        //    var list = new DCILObjectList();
        //    list.AddRange(this);
        //    return list;
        //}
        public bool RemoveByName(string name)
        {
            for (int iCount = this.Count - 1; iCount >= 0; iCount--)
            {
                if (this[iCount].Name == name)
                {
                    this.RemoveAt(iCount);
                    return true;
                }
            }
            return false;
        }

        //public DCILObject GetByName( string name )
        //{
        //    foreach( var item in this )
        //    {
        //        if(item.Name == name )
        //        {
        //            return item;
        //        }
        //    }
        //    return null;
        //}
        //public int IndexOf<T>(string name) where T : DCILObject
        //{
        //    int len = this.Count;
        //    for (int iCount = 0; iCount < len; iCount++)
        //    {
        //        if (this[iCount] is T && this[iCount].Name == name)
        //        {
        //            return iCount;
        //        }
        //    }
        //    return -1;
        //}
        //public T[] GetSubArray<T>() where T : DCILObject
        //{
        //    var list = new List<T>();
        //    foreach (var item in this)
        //    {
        //        if (item is T)
        //        {
        //            list.Add((T)item);
        //        }
        //    }
        //    if (list.Count > 0)
        //    {
        //        return list.ToArray();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}
