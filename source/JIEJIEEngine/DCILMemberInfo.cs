/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System.Collections.Generic;

namespace JIEJIE
{
    internal class DCILMemberInfo : DCILObject
    {
        public DCILMemberInfo()
        {

        }
        protected DCILMemberInfo(System.Reflection.MemberInfo member)
        {
            this._Name = member.Name;
            this.IsNative = true;
        }
        public override void Dispose()
        {
            base.Dispose();
            this.OldName = null;
            this.OldSignatureForMap = null;
            if(this.Styles != null )
            {
                this.Styles.Clear();
                this.Styles = null;
            }
        }
        /// <summary>
        /// 是否被其他模块调用
        /// </summary>
        public bool Used = false ;
        /// <summary>
        /// 为重命名而添加EditorBrowsableAttribute特性
        /// </summary>
        /// <param name="ebattr"></param>
        public bool AddEditorBrowsableAttributeForRename(DCILCustomAttribute ebattr)
        {
            //return false;
            if (this.RenameState == DCILRenameState.Renamed
                && this.Styles != null
                && this.Styles.Contains("public"))
            {
                if (this.CustomAttributes == null)
                {
                    this.CustomAttributes = new List<DCILCustomAttribute>();
                }
                else
                {
                    foreach( var attr in this.CustomAttributes )
                    {
                        if(attr.AttributeTypeName == ebattr.AttributeTypeName )
                        {
                            this.CustomAttributes.Remove(attr);
                            break;
                        }
                    }
                }
                this.CustomAttributes.Add(ebattr);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 为建立映射文件而保存的旧签名
        /// </summary>
        public string OldSignatureForMap = null;

        public virtual string GetSignatureForMap()
        {
            return ((DCILClass)this.Parent).GetNameWithNested('.') + "." + this.Name;
        }

        public virtual void ChangeName(string newName)
        {
            if (this.RenameState == DCILRenameState.Renamed)
            {

            }
            this._Name = newName;
            this.RenameState = DCILRenameState.Renamed;
            //this.RemoveObfuscationAttribute();
        }
        public readonly bool IsNative = false;

        private DCILRenameState _RenameState = DCILRenameState.NotHandled;

        /// <summary>
        /// 重命名操作状态
        /// </summary>
        public DCILRenameState RenameState
        {
            get
            {
                return this._RenameState;
            }
            set
            {
                if (this._RenameState != value)
                {
                    if (this._RenameState != DCILRenameState.Preserve)
                    {
                        this._RenameState = value;
                    }
                    else
                    {

                    }
                }
            }
        }

        public string OldName = null;
        public string GetOldName()
        {
            if( this.OldName == null || this.OldName.Length == 0 )
            {
                return this.Name;
            }
            else
            {
                return this.OldName;
            }
        }
        protected void WriteCustomAttributes(DCILWriter writer)
        {
            if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
            {
                foreach (var item in this.CustomAttributes)
                {
                    //writer.WriteLine();
                    item.WriteTo(writer);
                }
            }
        }
        //public void CollectAttributes(List<DCILCustomAttribute> attributes)
        //{
        //    if (this.CustomAttributes != null && this.CustomAttributes.Count > 0)
        //    {
        //        attributes.AddRange(this.CustomAttributes);
        //    }
        //}
        protected void WriteStyles(DCILWriter writer)
        {
            if (this.Styles != null && this.Styles.Count > 0)
            {
                foreach (var item in this.Styles)
                {
                    writer.Write(" " + item);
                }
                writer.Write(' ');
            }
        }
        

        public List<string> Styles = null;
        /// <summary>
        /// 删除样式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveStyle(string name)
        {
            if (name != null && name.Length > 0 && this.Styles != null)
            {
                int index = this.Styles.IndexOf(name);
                if (index >= 0)
                {
                    this.Styles.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }
        public bool RemoveStyle_specialname()
        {
            return RemoveStyle("specialname");
        }

        public void AddStyles(params string[] names)
        {
            if (names != null && names.Length > 0)
            {
                if (this.Styles == null)
                {
                    this.Styles = new List<string>();
                }
                this.Styles.AddRange(names);
            }
        }
        public bool AddStyle(string name , DCILReader reader )
        {
            if (name != null && name.Length > 0)
            {
                if (name[0] >= 'a' && name[0] <= 'z')
                {
                    if (this.Styles == null)
                    {
                        this.Styles = new List<string>();
                    }
                    this.Styles.Add(name);
                    return true;
                }
            }
            return false;
        }

        public bool HasStyle(string name)
        {
            return this.Styles != null && this.Styles.Contains(name);
        }

        public virtual void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            if (this.CustomAttributes != null)
            {
                foreach (var item in this.CustomAttributes)
                {
                    item.InvokeInfo = document.CacheDCILInvokeMethodInfo(item.InvokeInfo);
                    if(item.PrefixObject?.RefType != null )
                    {
                        item.PrefixObject.RefType = document.CacheTypeReference(item.PrefixObject.RefType);
                    }
                }
            }
        }
    }
}
