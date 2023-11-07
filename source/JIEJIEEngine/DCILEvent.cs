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
    internal class DCILEvent : DCILMemberInfo
    {
        public const string TagName = ".event";
        public DCILEvent()
        {

        }
        public DCILEvent(DCILClass cls, DCILReader reader)
        {
            this.Parent = cls;
            this.Load(reader);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.EventHandlerType = null;
            this.EventHandlerTypeName = null;
            this.Method_Addon = null;
            this.Method_Removeon = null;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Event;
            }
        }
        public override string ToString()
        {
            return "Event " + this.EventHandlerType.ToString() + " " + this._Name;
        }
        public override void Load(DCILReader reader)
        {
            while (reader.HasContentLeft())
            {
                int pos = reader.Position;
                string strWord = reader.ReadWord();
                if (strWord == "specialname" || strWord == "rtspecialname")
                {
                    this.AddStyle(strWord , reader );
                }
                else
                {
                    if (DCILTypeReference.IsStartWord(strWord))
                    {
                        this.EventHandlerType = DCILTypeReference.Load(strWord, reader);
                    }
                    else
                    {
                        reader.Position = pos;
                        this.EventHandlerType = DCILTypeReference.Load("class", reader);
                    }
                    this._Name = reader.ReadWord();
                    break;
                }
            }
            reader.MoveAfterChar('{');// reader.ReadAfterChar('{');
            while (reader.HasContentLeft())
            {
                var word = reader.ReadWord();
                if (word == DCILCustomAttribute.TagName_custom)
                {
                    base.ReadCustomAttribute(reader);
                }
                else if (word == ".addon")
                {
                    this.Method_Addon = new DCILInvokeMethodInfo(reader);
                }
                else if (word == ".removeon")
                {
                    this.Method_Removeon = new DCILInvokeMethodInfo(reader);
                }
                else if (word == "}")
                {
                    break;
                }
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".event ");
            this.EventHandlerType.WriteTo(writer);
            writer.Write(" ");
            writer.WriteLine(this._Name);
            writer.WriteStartGroup();
            base.WriteCustomAttributes(writer);
            if (this.Method_Addon != null)
            {
                writer.Write(".addon ");
                this.Method_Addon.WriteTo(writer);
                writer.WriteLine();
            }
            if (this.Method_Removeon != null)
            {
                writer.Write(".removeon ");
                this.Method_Removeon.WriteTo(writer);
                writer.WriteLine();
            }
            writer.WriteEndGroup();
        }
        public DCILInvokeMethodInfo Method_Addon = null;
        public DCILInvokeMethodInfo Method_Removeon = null;
        public DCILTypeReference EventHandlerType = null;

        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            this.Method_Addon = document.CacheDCILInvokeMethodInfo(this.Method_Addon);
            this.Method_Removeon = document.CacheDCILInvokeMethodInfo(this.Method_Removeon);
            this.EventHandlerType = document.CacheTypeReference(this.EventHandlerType);
            this.Method_Addon?.UpdateLocalInfo(this.Parent as DCILClass);
            this.Method_Removeon?.UpdateLocalInfo(this.Parent as DCILClass);
            if (this.Method_Addon.LocalMethod != null)
            {
                this.Method_Addon.LocalMethod.ParentMember = this;
            }
            if (this.Method_Removeon.LocalMethod != null)
            {
                this.Method_Removeon.LocalMethod.ParentMember = this;
            }
            base.CusotmAttributesCacheTypeReference(document);
        }

        public string EventHandlerTypeName = null;
    }
}
