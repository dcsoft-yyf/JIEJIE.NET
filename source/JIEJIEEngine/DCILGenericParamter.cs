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
    /// <summary>
    /// see "Partition II Metadata.doc",topic 10.1.7	Generic parameters (GenPars).
    /// </summary>
    internal class DCILGenericParamter : IEqualsValue<DCILGenericParamter> , IDisposable
    {
        public DCILGenericParamter()
        {

        }
        public DCILGenericParamter(string name, bool defineInClass)
        {
            this.Name = name;
            this.DefineInClass = defineInClass;
        }
        public void Dispose()
        {
            this.Name = null;
            if( this.Attributes != null )
            {
                this.Attributes.Clear();
                this.Attributes = null;
            }
            this.RuntimeType = null;
            this.Constraints = null;
            this.RuntimeType = null;
        }
        private static int _InstanceIndexCount = 0;
        public int InstanceIndex = _InstanceIndexCount++;
        public int Index = -1;
        public bool DefineInClass = false;
        public DCILTypeReference RuntimeType = null;

        public string Name = null;
        public List<string> Attributes = null;
        public DCILTypeReference[] Constraints = null;
        public static bool MatchList(List<DCILGenericParamter> ps1, List<DCILTypeReference> ps2)
        {
            int len1 = ps1 == null ? 0 : ps1.Count;
            int len2 = ps2 == null ? 0 : ps2.Count;
            if (len1 != len2)
            {
                return false;
            }
            return true;
        }
        public bool EqualsValue(DCILGenericParamter p)
        {
            if (p == null)
            {
                return false;
            }

            if (this == p)
            {
                return true;
            }
            int len1 = this.Constraints == null ? 0 : this.Constraints.Length;
            int len2 = p.Constraints == null ? 0 : p.Constraints.Length;
            if (len1 != len2)
            {
                return false;
            }
            if (len1 > 0)
            {
                for (int iCount = 0; iCount < len1; iCount++)
                {
                    if (this.Constraints[iCount].EqualsValue(p.Constraints[iCount]) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void CacheTypeReference(DCILDocument document, List<DCILGenericParamter> ps)
        {
            if (ps != null)
            {
                foreach (var item in ps)
                {
                    if (item.Constraints != null)
                    {
                        for (int iCount = 0; iCount < item.Constraints.Length; iCount++)
                        {
                            item.Constraints[iCount] = document.CacheTypeReference(item.Constraints[iCount]);
                        }
                    }
                }
            }
        }


    }
}
