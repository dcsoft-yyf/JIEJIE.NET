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
using System.Text;

namespace JIEJIE
{
    internal class DCILMethodParamter : IDisposable
    {
        public static bool EqualsList(
            List<DCILMethodParamter> ps1,
            List<DCILMethodParamter> ps2,
            bool includeName,
            bool checkDefaultValue)
        {
            int len1 = ps1 == null ? 0 : ps1.Count;
            int len2 = ps2 == null ? 0 : ps2.Count;
            if (len1 != len2)
            {
                return false;
            }
            if (len1 > 0)
            {
                for (int iCount = 0; iCount < len1; iCount++)
                {
                    if (ps1[iCount].EqualsValue(ps2[iCount], includeName, checkDefaultValue) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //public List<DCILMethodParamter> CloneList( List<DCILMethodParamter > ps)
        //{
        //    if( ps != null )
        //    {
        //        var ps2 = new List<DCILMethodParamter>();
        //        foreach( var p in ps )
        //        {
        //            ps2.Add(p.Clone());
        //        }
        //        return ps2;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        public void Dispose()
        {
            this.DefaultValue = null;
            this.Marshal = null;
            this.Name = null;
            this.ValueType = null;
        }
        public DCILMethodParamter Clone()
        {
            return (DCILMethodParamter)this.MemberwiseClone();
        }
        public bool IsOut = false;
        public bool IsIn = false;

        public string Name = null;
        public DCILTypeReference ValueType = null;
        public string DefaultValue = null;
        public string Marshal = null;
        public static void CacheTypeReference(DCILDocument document, List<DCILMethodParamter> ps)
        {
            if (ps != null)
            {
                foreach (var p in ps)
                {
                    p.ValueType = document.CacheTypeReference(p.ValueType);
                }
            }
        }
        internal int ComputeHashCode(bool includeName)
        {
            int result = 0;
            if (this.IsOut)
            {
                result = 1;
            }
            if (this.IsIn)
            {
                result = result + 2;
            }
            if (includeName && this.Name != null && this.Name.Length > 0)
            {
                result += this.Name.GetHashCode();
            }
            if (this.ValueType != null)
            {
                result += this.ValueType.GetHashCode();
            }
            if (this.DefaultValue != null && this.DefaultValue.Length > 0)
            {
                result += this.DefaultValue.GetHashCode();
            }
            if (this.Marshal != null && this.Marshal.Length > 0)
            {
                result += this.Marshal.GetHashCode();
            }
            return result;
        }
        internal bool EqualsValue(DCILMethodParamter p, bool includeName, bool checkDefaultValue)
        {
            if (p == null)
            {
                return false;
            }
            if (p == this)
            {
                return true;
            }
            if (checkDefaultValue && DCUtils.EqualsStringExt(this.DefaultValue, p.DefaultValue) == false)
            {
                return false;
            }
            if (this.IsIn != p.IsIn
                || this.IsOut != p.IsOut
                || DCUtils.EqualsStringExt(this.Marshal, p.Marshal) == false)
            {
                return false;
            }
            if (DCILTypeReference.StaticEquals(this.ValueType, p.ValueType) == false)
            {
                return false;
            }
            if (includeName && DCUtils.EqualsStringExt(this.Name, p.Name) == false)
            {
                return false;
            }
            return true;
        }
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(this.ValueType.ToString());
            if (this.Name != null && this.Name.Length > 0)
            {
                str.Append(' ');
                str.Append(this.Name);
            }
            return str.ToString();
        }
        public static List<DCILMethodParamter> ReadParameters(DCILReader reader, bool readName = true)
        {
            var paramters = new List<DCILMethodParamter>();
            DCILMethodParamter mp = null;
            while (reader.HasContentLeft())
            {
                var word = reader.ReadWord();
                if (word == ")")
                {
                    break;
                }
                if (word == "(")
                {
                    continue;
                }
                //if (readName == false)
                //{

                //}
                if (mp == null)
                {
                    mp = new DCILMethodParamter();
                    paramters.Add(mp);
                }
                if (word == "[")
                {
                    word = reader.ReadAfterCharExcludeLastChar(']', true);
                    if (word == "in")
                    {
                        mp.IsIn = true;
                    }
                    else if (word == "out")
                    {
                        mp.IsOut = true;
                    }
                    continue;
                }
                if (DCILTypeReference.IsStartWord(word))
                {
                    mp.ValueType = DCILTypeReference.Load(word, reader);
                    word = reader.ReadWord();
                    if (word == ")")
                    {
                        break;
                    }
                }
                if (word == ",")
                {
                    mp = new DCILMethodParamter();
                    paramters.Add(mp);
                    continue;
                }
                else if (word == "marshal")
                {
                    mp.Marshal = reader.ReadStyleExtValue();
                    mp.Name = reader.ReadWord();
                }
                else if (readName)
                {
                    if (mp.Name == null)
                    {
                        mp.Name = word;
                    }
                }
            }
            if (paramters.Count > 0)
            {
                //foreach (var item in paramters)
                //{
                //    if (item.ValueType == null)
                //    {

                //    }
                //}
                return paramters;
            }
            else
            {
                return null;
            }
        }
        public static void WriteTo(List<DCILMethodParamter> parameters, DCILWriter writer, bool forSignString)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (forSignString)
                {
                    writer.Write("(");
                }
                else
                {
                    writer.WriteLine("(");
                    writer.ChangeIndentLevel(1);
                }
                for (int iCount = 0; iCount < parameters.Count; iCount++)
                {
                    var p = parameters[iCount];
                    if (iCount > 0)
                    {
                        writer.Write(",");
                    }
                    if (p.IsIn)
                    {
                        writer.Write("[in] ");
                    }
                    if (p.IsOut)
                    {
                        writer.Write("[out] ");
                    }
                    if (p.DefaultValue != null && p.DefaultValue.Length > 0)
                    {
                        writer.Write("[opt] ");
                    }
                    if (forSignString)
                    {
                        p.ValueType.WriteTo(writer, true, false);
                    }
                    else
                    {
                        p.ValueType.WriteTo(writer);
                    }
                    if (p.Marshal != null && p.Marshal.Length > 0)
                    {
                        writer.Write(" marshal( " + p.Marshal + ") ");
                    }
                    if (forSignString == false)
                    {
                        writer.Write("  " + p.Name);
                    }
                }
                writer.Write(")");
                if (forSignString == false)
                {
                    writer.ChangeIndentLevel(-1);
                }
            }
            else
            {
                writer.Write("()");
            }
        }
    }
}
