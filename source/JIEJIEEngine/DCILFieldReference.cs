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
    internal class DCILFieldReference :IDisposable
    {
        public DCILFieldReference()
        {

        }
        public DCILFieldReference( DCILField field )
        {
            if(field == null )
            {
                throw new ArgumentNullException("field");
            }
            this.ValueType = field.ValueType;
            if (field.Parent is DCILClass)
            {
                this.OwnerType = ((DCILClass)field.Parent).GetLocalTypeReference();
            }
            this.FieldName = field.Name;
            this.LocalField = field;
        }

        public DCILFieldReference(DCILReader reader)
        {
            this.ValueType = DCILTypeReference.Load(reader);
            this.OwnerType = DCILTypeReference.Load(reader);
            if (reader.MatchText("::"))
            {
                reader.Position += 2;
                this.FieldName = reader.ReadWord();
            }
        }
        public void Dispose()
        {
            this.FieldName = null;
            this.LocalField = null;
            this.OwnerType = null;
            this.ValueType = null;
        }
        public override string ToString()
        {
            return this.ValueType?.ToString() + " " + this.OwnerType?.Name + "::" + this.FieldName;
        }
        public DCILTypeReference ValueType = null;
        public DCILTypeReference OwnerType = null;
        public DCILField LocalField = null;
        public string FieldName = null;

        public void UpdateLocalField(DCILClass cls)
        {
            if (this.OwnerType != null)
            {
                this.OwnerType.LocalClass = cls;
            }
            foreach (var item in cls.ChildNodes)
            {
                if (item is DCILField && item.Name == this.FieldName)
                {
                    this.LocalField = (DCILField)item;
                    break;
                }
            }
        }

        public void UpdateLocalInfo(DCILDocument document, Dictionary<string, DCILClass> clses)
        {
            //if (this.OwnerType.Name == "__DC20210205._336")
            //{

            //}
            this.ValueType = document.CacheTypeReference(this.ValueType);
            this.OwnerType = document.CacheTypeReference(this.OwnerType);
            if (this.ValueType != null)
            {
                //var cls2 = this.ValueType.LocalClass;
                this.ValueType.UpdateLocalClass(clses);
                //if (this.ValueType.Name.StartsWith("'<PrivateImplementationDetails>'") 
                //    && this.ValueType.LocalClass == null)
                //{

                //}
            }
            if (this.OwnerType != null )
            {
                this.OwnerType.UpdateLocalClass(clses);
                var cls2 = this.OwnerType.LocalClass;
                if (cls2 != null && this.LocalField == null )
                {
                    foreach (var item in cls2.ChildNodes)
                    {
                        if (item is DCILField && item.Name == this.FieldName)
                        {
                            this.LocalField = (DCILField)item;
                            break;
                        }
                    }
                }
            }
        }
        public void WriteTo(DCILWriter writer, bool forLdtoken = false)
        {
            writer.Write("  ");

            this.ValueType.WriteTo(writer, true);
            writer.Write(" ");
            if( this.OwnerType == null )
            {
                this.OwnerType = ((DCILClass)this.LocalField.Parent).GetLocalTypeReference();
            }
            if (this.OwnerType.IsGenericType)
            {
                this.OwnerType.WriteTo(writer, true );
            }
            else
            {
                this.OwnerType.WriteTo(writer, false);
            }
            writer.Write("::");
            if (this.LocalField == null)
            {
                writer.Write(this.FieldName);
            }
            else
            {
                writer.Write(this.LocalField.Name);
            }
        }

        public override bool Equals(object obj)
        {
            return EqualsValue(obj as DCILFieldReference);
        }
        public bool EqualsValue(DCILFieldReference info2)
        {
            if (info2 == null)
            {
                return false;
            }
            if (info2 == this)
            {
                return true;
            }
            if (this.FieldName != info2.FieldName)
            {
                return false;
            }
            if (DCILTypeReference.StaticEquals(this.ValueType, info2.ValueType) == false)
            {
                return false;
            }
            if (DCILTypeReference.StaticEquals(this.OwnerType, info2.OwnerType) == false)
            {
                return false;
            }
            return true;
        }
        private int _HashCode = 0;
        public override int GetHashCode()
        {
            if (this._HashCode == 0)
            {
                this._HashCode = this.FieldName.GetHashCode();
                if (this.OwnerType != null)
                {
                    this._HashCode += this.OwnerType.GetHashCode();
                }
                if (this.ValueType != null)
                {
                    this._HashCode += this.ValueType.GetHashCode();
                }
            }
            return this._HashCode;
        }

    }
}
