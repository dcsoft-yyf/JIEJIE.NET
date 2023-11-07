/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System.Collections.Generic;
using System.Text;

namespace JIEJIE
{
    internal class DCILProperty : DCILMemberInfo
    {
        public const string TagName_property = ".property";
        public DCILProperty(DCILClass cls, DCILReader reader)
        {
            this.Parent = cls;
            this.Load(reader);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.Method_Get = null;
            this.Method_Set = null;
            if( this.Parameters != null )
            {
                foreach( var item in this.Parameters )
                {
                    item.Dispose();
                }
                this.Parameters = null;
                this.Parameters = null;
            }
            this.ValueType = null;
            this.ValueTypeName = null;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Property;
            }
        }
        public override void Load(DCILReader reader)
        {
            this.StartLineIndex = reader.CurrentLineIndex();
            if( this.StartLineIndex == 30462)
            {

            }
            while (reader.HasContentLeft())
            {
                int posBack = reader.Position;
                string strWord = reader.ReadWord();
                if( this.ValueType != null )
                {
                    reader.Position = posBack;
                    break;
                }
                else if (DCILTypeReference.IsStartWord(strWord))
                {
                    this.ValueType = DCILTypeReference.Load(strWord, reader);
                }
                else
                {
                    this.AddStyle(strWord , reader );
                }
            }
            this._Name = reader.ReadWord();
            reader.MoveAfterChar('('); //reader.ReadAfterCharExcludeLastChar('(');
            this.Parameters = DCILMethodParamter.ReadParameters(reader, false);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {

            }
            reader.MoveAfterChar('{');// reader.ReadAfterChar('{');
            while (reader.HasContentLeft())
            {
                var word = reader.ReadWord();
                if (word == DCILCustomAttribute.TagName_custom)
                {
                    base.ReadCustomAttribute(reader);
                }
                else if (word == ".get")
                {
                    this.Method_Get = new DCILInvokeMethodInfo(reader);
                }
                else if (word == ".set")
                {
                    this.Method_Set = new DCILInvokeMethodInfo(reader);
                }
                else if (word == "}")
                {
                    break;
                }
            }
        }

        public override void WriteTo(DCILWriter writer)
        {
            if( this.StartLineIndex == 30462)
            {

            }
            if( this.Name == "IsQuiescent")
            {

            }
            writer.Write(".property ");
            base.WriteStyles(writer);
            this.ValueType.WriteTo(writer);
            writer.Write(" " + this._Name);
            DCILMethodParamter.WriteTo(this.Parameters, writer, false);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {

            }
            writer.WriteStartGroup();
            base.WriteCustomAttributes(writer);
            if (this.Method_Get != null)
            {
                writer.Write(".get ");
                this.Method_Get.WriteTo(writer);
                writer.WriteLine();
            }
            if (this.Method_Set != null)
            {
                writer.Write(".set ");
                this.Method_Set.WriteTo(writer);
                writer.WriteLine();
            }
            writer.WriteEndGroup();
        }
        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            DCILMethodParamter.CacheTypeReference(document, this.Parameters);
            this.Method_Get = document.CacheDCILInvokeMethodInfo(this.Method_Get);
            this.Method_Set = document.CacheDCILInvokeMethodInfo(this.Method_Set);
            this.ValueType = document.CacheTypeReference(this.ValueType);
            if (this.Method_Get != null)
            {
                this.Method_Get.UpdateLocalInfo(this.Parent as DCILClass);
                if (this.Method_Get.LocalMethod != null)
                {
                    this.Method_Get.LocalMethod.ParentMember = this;
                }
            }
            if (this.Method_Set != null)
            {
                this.Method_Set.UpdateLocalInfo(this.Parent as DCILClass);
                if (this.Method_Set.LocalMethod != null)
                {
                    this.Method_Set.LocalMethod.ParentMember = this;
                }
            }
            base.CusotmAttributesCacheTypeReference(document);
        }
        public List<DCILMethodParamter> Parameters = null;

        public DCILInvokeMethodInfo Method_Get = null;
        public DCILInvokeMethodInfo Method_Set = null;
        public DCILTypeReference ValueType = null;

        public bool ValueTypeIsClass = false;

        public string ValueTypeName = null;

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Property " + this._Name);
            if (this.Method_Get != null)
            {
                str.Append(" get; ");
            }
            if (this.Method_Set != null)
            {
                str.Append(" set;");
            }
            return str.ToString();
        }
    }
}
