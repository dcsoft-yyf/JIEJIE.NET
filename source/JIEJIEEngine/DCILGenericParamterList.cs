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
    internal class DCILGenericParamterList : List<DCILGenericParamter> , IDisposable
    {
        public static DCILGenericParamterList Merge(DCILGenericParamterList list1, DCILGenericParamterList list2)
        {
            if (list1 == null)
            {
                if (list2 == null)
                {
                    return null;
                }
                else
                {
                    return list2;
                }
            }
            else
            {
                if (list2 == null)
                {
                    return list1;
                }
                else
                {
                    var result = new DCILGenericParamterList(list1.Count + list2.Count);
                    result.AddRange(list1);
                    result.AddRange(list2);
                    return result;
                }
            }
        }

        public static DCILGenericParamterList CreateByNativeType(Type t)
        {
            if (t == null || t.IsGenericType == false)
            {
                return null;
            }
            var gs = t.GetGenericArguments();
            if (gs == null || gs.Length == 0)
            {
                return null;
            }
            var list = new DCILGenericParamterList(gs.Length);
            foreach (var item in gs)
            {
                list.Add(new DCILGenericParamter(item.Name, true));
            }
            list.ResetIndex();
            return list;
        }

        public DCILGenericParamterList(int len) : base(len)
        {

        }

        public DCILGenericParamterList(DCILReader reader, bool defineInClass)
        {
            DCILGenericParamter cgp = new DCILGenericParamter();
            this.Add(cgp);
            while (reader.HasContentLeft())
            {
                string strWord = reader.ReadWord();
                if (strWord == null || strWord == ">")
                {
                    break;
                }
                else if (strWord == "valuetype"
                    || strWord == "class"
                    || strWord == ".ctor"
                    || strWord == "'+'" || strWord == "'-'")
                {
                    if (cgp.Attributes == null)
                    {
                        cgp.Attributes = new List<string>();
                    }
                    cgp.Attributes.Add(strWord);
                    continue;
                }
                else if (strWord == "(")
                {
                    // 约束
                    var list9 = new List<DCILTypeReference>();
                    while (reader.SkipWhitespace())
                    {
                        if (reader.Peek() == ',')
                        {
                            cgp = new DCILGenericParamter();
                            this.Add(cgp);
                            reader.Read();
                        }
                        else if (reader.Peek() == ')')
                        {
                            reader.Read();
                            break;
                        }
                        else
                        {
                            var item9 = DCILTypeReference.Load(reader);
                            if (item9 != null)
                            {
                                list9.Add(item9);
                            }
                        }
                    }//while
                    if (list9.Count > 0)
                    {
                        cgp.Constraints = list9.ToArray();
                    }
                    continue;
                }
                else if (strWord == ",")
                {
                    cgp = new DCILGenericParamter();
                    this.Add(cgp);
                }
                else
                {
                    cgp.Name = strWord;
                }
            }
            for (int iCount = this.Count - 1; iCount >= 0; iCount--)
            {
                this[iCount].DefineInClass = defineInClass;
                this[iCount].Index = iCount;
            }
        }
        public void Dispose()
        {
            foreach( var item in this )
            {
                item.Dispose();
            }
            this.Clear();
        }
        public void ClearRuntimeType()
        {
            foreach (var item in this)
            {
                item.RuntimeType = null;
            }
        }
        public void ResetIndex()
        {
            for (int iCount = this.Count - 1; iCount >= 0; iCount--)
            {
                this[iCount].Index = iCount;
            }
        }
        public void SetRuntimeType(List<DCILTypeReference> ts)
        {
            if (this.Count > 0 && ts != null && ts.Count != this.Count)
            {

            }
            if (ts != null && ts.Count == this.Count)
            {
                for (int iCount = 0; iCount < this.Count; iCount++)
                {
                    this[iCount].RuntimeType = ts[iCount];
                }
            }
        }
        public DCILGenericParamter GetItem(string name, bool defineInClass)
        {
            if (name == null || name.Length == 0)
            {
                return null;
            }
            if (char.IsNumber(name[0]))
            {
                int index = int.Parse(name);
                if (index >= 0)
                {
                    foreach (var item in this)
                    {
                        if (item.DefineInClass == defineInClass && item.Index == index)
                        {
                            return item;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in this)
                {
                    if (item.DefineInClass == defineInClass && item.Name == name)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public void WriteTo(DCILWriter writer)
        {
            if (this.Count > 0)
            {
                //writer.WriteLine();
                writer.ChangeIndentLevel(2);
                writer.Write('<');
                for (int iCount = 0; iCount < this.Count; iCount++)
                {
                    if (iCount > 0)
                    {
                        writer.Write(',');
                    }
                    var item = this[iCount];
                    if (item.Attributes != null && item.Attributes.Count > 0)
                    {
                        foreach (var attr in item.Attributes)
                        {
                            writer.Write(attr + " ");
                        }
                    }
                    if (item.Constraints != null && item.Constraints.Length > 0)
                    {
                        writer.Write('(');
                        for (int iCount2 = 0; iCount2 < item.Constraints.Length; iCount2++)
                        {
                            var item2 = item.Constraints[iCount2];
                            if (iCount2 > 0)
                            {
                                writer.Write(',');
                            }
                            item2.WriteTo(writer);
                        }
                        writer.Write(')');
                    }
                    writer.Write(item.Name);
                }
                writer.Write('>');
                writer.ChangeIndentLevel(-2);
            }
        }
    }
}
