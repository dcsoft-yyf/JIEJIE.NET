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
    internal class DCILObject : IDisposable
    {
        private static int _InstanceIndexCounter = 0;
        public int InstanceIndex = _InstanceIndexCounter++;
        public DCILDocument OwnerDocument = null;
        /// <summary>
        /// 对象类型
        /// </summary>
        public virtual DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.None;
            }
        }

        internal string _Name = null;
        public string Name
        {
            get
            {
                return this._Name;
            }
        }
        public string Type = null;
        // public string Header = null;
        public virtual void Load(DCILReader reader)
        {

        }
        public virtual void WriteTo(DCILWriter writer)
        {

        }
        public virtual void Dispose()
        {
            this._Name = null;
            if(this.ChildNodes != null )
            {
                this.ChildNodes.Dispose();
                this.ChildNodes = null;
            }
            if(this.CustomAttributes != null )
            {
                foreach( var item in this.CustomAttributes )
                {
                    item.Dispose();
                }
                this.CustomAttributes = null;
            }
            if(this.ObfuscationSettings != null )
            {
                this.ObfuscationSettings.Dispose();
                this.ObfuscationSettings = null;
            }
            if(this.OperCodes != null )
            {
                this.OperCodes.Dispose();
                this.OperCodes = null;
            }
            this.OwnerDocument = null;
            this.Parent = null;
            this.Type = null;
        }
        public int StartLineIndex = 0;
        public DCILOperCodeList OperCodes = null;
        public List<T> GetAllOperCodes<T>() where T : DCILOperCode
        {
            var list = new List<T>();
            this.CollectOperCodes<T>(list);
            return list;
        }
        public int CollectOperCodes<T>(List<T> outList) where T : DCILOperCode
        {
            if (this.OperCodes != null && this.OperCodes.Count > 0)
            {
                int result = outList.Count;
                InnerCollectOperCodes<T>(this.OperCodes, outList);
                result = outList.Count - result;
                return result;
            }
            return 0;
        }

        private void InnerCollectOperCodes<T>(DCILOperCodeList list, List<T> outList) where T : DCILOperCode
        {
            foreach (var item in list)
            {
                if (item is T)
                {
                    outList.Add((T)item);
                }
                if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var block = (DCILOperCode_Try_Catch_Finally)item;
                    if (block.HasTryOperCodes())
                    {
                        InnerCollectOperCodes<T>(block._Try.OperCodes, outList);
                    }
                    if (block.HasCatchs())
                    {
                        foreach (var item2 in block._Catchs)
                        {
                            if (item2.OperCodes != null && item2.OperCodes.Count > 0)
                            {
                                InnerCollectOperCodes<T>(item2.OperCodes, outList);
                            }
                        }
                    }
                    if (block.HasFinallyOperCodes())
                    {
                        InnerCollectOperCodes<T>(block._Finally.OperCodes, outList);
                    }
                    if (block.HasFaultOperCodes())
                    {
                        InnerCollectOperCodes<T>(block._fault.OperCodes, outList);
                    }
                }
            }
        }
        /// <summary>
        /// 子节点列表
        /// </summary>
        public DCILObjectList ChildNodes = null;
        /// <summary>
        /// 父节点
        /// </summary>
        public DCILObject Parent = null;
        /// <summary>
        /// 指定的混淆设置
        /// </summary>
        public DCILObfuscationAttribute ObfuscationSettings = null;
        /// <summary>
        /// 是否具有自定义特性
        /// </summary>
        public bool HasCustomAttributes
        {
            get
            {
                return this.CustomAttributes != null && this.CustomAttributes.Count > 0;
            }
        }
        public List<DCILCustomAttribute> CustomAttributes = null;
        public void CusotmAttributesCacheTypeReference(DCILDocument document)
        {
            if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
            {
                foreach (var item in this.CustomAttributes)
                {
                    item.InvokeInfo?.CacheTypeReference(document);
                }
            }
        }
        internal DCILCustomAttribute ReadCustomAttribute(DCILReader reader)
        {
            if (this.CustomAttributes == null)
            {
                this.CustomAttributes = new List<DCILCustomAttribute>();
            }
            var item = DCILCustomAttribute.Create(this, reader);
            if (item.Prefix == "(UNKNOWN_OWNER)")
            {
                return null;
            }
            this.CustomAttributes.Add(item);
            if (item is DCILObfuscationAttribute)
            {
                this.ObfuscationSettings = (DCILObfuscationAttribute)item;
            }
            return item;
        }
        internal void AddCustomAttributes(List<DCILCustomAttribute> attrs)
        {
            if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
            {
                attrs.AddRange(this.CustomAttributes);
            }
        }
        internal void RemoveObfuscationAttribute()
        {
            //return;
            if (this.ObfuscationSettings != null)
            {
                this.ObfuscationSettings = null;
                if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
                {
                    for (int iCount = this.CustomAttributes.Count - 1; iCount >= 0; iCount--)
                    {
                        if (this.CustomAttributes[iCount] is DCILObfuscationAttribute)
                        {
                            this.CustomAttributes.RemoveAt(iCount);
                            break;
                        }
                    }
                }
            }
        }

        public int Level = 0;
        public int TotalOperCodesCount
        {
            get
            {
                int result = 0;
                if (this.OperCodes != null && this.OperCodes.Count > 0)
                {
                    result = this.OperCodes.Count;
                    foreach (var item in this.OperCodes)
                    {
                        if (item is DCILOperCode_Try_Catch_Finally)
                        {
                            result += ((DCILOperCode_Try_Catch_Finally)item).TotalOperCodesCount;
                        }
                    }
                }
                return result;
            }
        }
        public override string ToString()
        {
            if (this.ChildNodes != null && this.ChildNodes.Count > 0)
            {
                return this.Type + "#" + this._Name + " " + this.ChildNodes.Count + "个子节点";
            }
            else
            {
                return this.Type + "#" + this._Name;
            }
        }
        public virtual DCILMethod GetOwnerMethod()
        {
            var p = this;
            while (p != null)
            {
                if (p is DCILMethod)
                {
                    return (DCILMethod)p;
                }
                p = p.Parent;
            }
            return null;
        }
    }
}
