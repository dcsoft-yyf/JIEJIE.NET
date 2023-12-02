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
    internal class DCILField : DCILMemberInfo
    {
        private static readonly HashSet<string> _FieldAttributes = null;
        static DCILField()
        {
            _FieldAttributes = new HashSet<string>();
            _FieldAttributes.Add("assembly");
            _FieldAttributes.Add("famandassem");
            _FieldAttributes.Add("family");
            _FieldAttributes.Add("famorassem");
            _FieldAttributes.Add("initonly");
            _FieldAttributes.Add("literal");
            //_FieldAttributes.Add("marshal ‘(’ NativeType ‘)’");
            _FieldAttributes.Add("notserialized");
            _FieldAttributes.Add("private");
            _FieldAttributes.Add("compilercontrolled");
            _FieldAttributes.Add("public");
            _FieldAttributes.Add("rtspecialname");
            _FieldAttributes.Add("specialname");
            _FieldAttributes.Add("static");
            _FieldAttributes.Add("instance");
            _FieldAttributes.Add("method");
            _FieldAttributes.Add("unmanaged");
            _FieldAttributes.Add("cdecl");
        }

        public const string TagName = ".field";
        public DCILField()
        {

        }
        public DCILField(DCILClass parent, DCILReader reader)
        {
            this.Load(reader);
            this.Parent = parent;
        }
        public override void Dispose()
        {
            base.Dispose();
            //this.OldSignature = null;
            this.ReferenceData = null;
            this.ValueType = null;
            this.ConstValue = null;
            //this.DataLabel = null;
            this.MarshalAs = null;
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Field;
            }
        }
        public DCILTypeReference ValueType = null;

        public int InnerTag = 0;

        public override void CacheInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            this.ValueType = document.CacheTypeReference(this.ValueType);
            base.CusotmAttributesCacheTypeReference(document);
        }
        public bool IsConst
        {
            get
            {
                return base.HasStyle("literal");
            }
        }
        public string ConstValue = null;
        //public string DataLabel = null;
        public DCILData ReferenceData = null;

        public string MarshalAs = null; 
        //public List<System.Tuple<string,string>> ExtStyles = null;
        public override void Load(DCILReader reader)
        {
            this.StartLineIndex = reader.CurrentLineIndex();
            if (reader.PeekContentChar() == '[')
            {
                this.SpecifyIndex = reader.ReadArrayIndex();
            }
            while (reader.HasContentLeft())
            {
                int pos = reader.Position;
                var strWord = reader.ReadWord();
                if (_FieldAttributes.Contains(strWord))
                {
                    this.AddStyle(strWord , reader );
                }
                else if( strWord == "marshal")
                {
                    this.MarshalAs = reader.ReadStyleExtValue();
                }
                else if (DCILTypeReference.IsStartWord(strWord))
                {
                    this.ValueType = DCILTypeReference.Load(strWord, reader);
                    strWord = reader.ReadWord();
                    this._Name = strWord;
                    if (reader.HasContentLeftCurrentLine() == false)
                    {
                        break;
                    }
                }
                else if (strWord == "at")
                {
                    var strDataLabel = reader.ReadWord();
                    reader.AddReferenceDataLabel(this, strDataLabel);
                    break;
                }
                else if (strWord == "=")
                {
                    if (this.ValueType == DCILTypeReference.Type_String)
                    {
                        var info = reader.ReadStringValue();
                        this.ConstValue = info.ILRawText;
                        //string text2 = reader.ReadStringValue(ref this.ConstValue);

                        //this.ConstValue = reader.ReadStringValue(ref rawILText);

                        //var v = new DCILStringValue(reader);
                        //this.ConstValue = DCUtils.GetStringUseTable( v.RawILText );
                    }
                    else
                    {
                        this.ConstValue = DCUtils.GetStringUseTable( reader.ReadLineTrimRemoveComment() );
                    }
                    break;
                }
                else if (strWord[0] == '.')
                {
                    reader.Position = pos;
                    break;
                }
            }

            while (reader.HasContentLeft())
            {
                int pos = reader.Position;
                var word = reader.ReadWord();
                if (word == DCILCustomAttribute.TagName_custom)
                {
                    base.ReadCustomAttribute(reader);
                }
                else
                {
                    reader.Position = pos;
                    break;
                }
            }
        }
        /// <summary>
        /// 旧的签名信息
        /// </summary>
        //public string OldSignature = null;
        //public void UpdateOldSignature()
        //{
        //    var writer = new DCILWriter(new StringBuilder());
        //    this.ValueType.WriteToForSignString(writer);
        //    this.OldSignature = writer.ToString();
        //}

        public override void WriteTo(DCILWriter writer)
        {
            if(this.RenameState == DCILRenameState.Renamed )
            {
                writer.WriteLine("// " + ((DCILClass)this.Parent).GetOldName() + "::" + this.GetOldName());
            }
            writer.Write(".field ");
            if(this.SpecifyIndex != int.MinValue)
            {
                writer.Write(" [" + this.SpecifyIndex + "] ");
            }
            base.WriteStyles(writer);
            if( this.MarshalAs != null && this.MarshalAs.Length > 0 )
            {
                writer.Write(" marshal(" + this.MarshalAs + ") ");
            }
            this.ValueType.WriteTo(writer);
            writer.Write(" " + this._Name);
            if (this.ConstValue != null && this.ConstValue.Length > 0)
            {
                writer.Write(" = ");
                writer.Write(this.ConstValue);
            }
            else if (this.ReferenceData != null)
            {
                writer.Write(" at ");
                writer.Write(this.ReferenceData.Name);
            }
            writer.WriteLine();
            base.WriteCustomAttributes(writer);
        }

        public int SpecifyIndex = int.MinValue;

        public override string ToString()
        {
            return "field " + this.ValueType + " " + this._Name;
        }
    }
}
