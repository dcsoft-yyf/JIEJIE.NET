/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */

namespace JIEJIE
{
    internal class DCILCustomAttribute : DCILObject
    {
        public static DCILCustomAttribute Create(DCILObject parent, DCILReader reader)
        {
            if( reader.CurrentLineIndex() == 3375961)
            {

            }
            string preFix = null;
            if (reader.PeekContentChar() == '(')
            {
                preFix = reader.ReadAfterChar(')').Trim();
            }
            var invokeInfo = new DCILInvokeMethodInfo(reader);
            reader.MoveAfterChar('=');// reader.ReadAfterChar('=');
            var bsValue = reader.ReadBinaryFromHex();
            DCILCustomAttribute result = null;
            string typeName = invokeInfo?.OwnerType?.Name;
            switch (typeName)
            {
                case DCILObfuscationAttribute.ConstAttributeTypeName:
                    result = new DCILObfuscationAttribute(invokeInfo, bsValue);
                    break;
                case DCILTypeConverterAttribute.ConstAttributeTypeName:
                    result = new DCILTypeConverterAttribute();
                    break;
                case DCILEditorAttribute.ConstAttributeTypeName:
                    result = new DCILEditorAttribute();
                    break;
                default:
                    result = new DCILCustomAttribute();
                    break;
            }
            result.Prefix = preFix;
            result.Parent = parent;
            result.InvokeInfo = invokeInfo;
            result.BinaryValue = bsValue;
            result.AttributeTypeName = typeName;
            return result;
        }

        public const string TagName_custom = ".custom";

        public DCILCustomAttribute()
        {

        }

        public DCInterfaceimpl PrefixObject = null;

        public string Prefix = null;

        public override void Dispose()
        {
            this.AttributeTypeName = null;
            this.BinaryValue = null;
            if(this._Values != null )
            {
                foreach( var item in this._Values)
                {
                    item.Dispose();
                }
                this._Values = null;
            }
            this.HexValue = null;
            this.InvokeInfo = null;

            base.Dispose();
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.CustomAttribute;
            }
        }

        public string AttributeTypeName = null;

        public DCILInvokeMethodInfo InvokeInfo = null;

        public byte[] BinaryValue = null;

        private DCILCustomAttributeValue[] _Values = null;
        public virtual void ParseValues(ReadCustomAttributeValueArgs args)
        {
            try
            {
                var list = DCILCustomAttributeValue.ParseValues(this.BinaryValue, this.InvokeInfo, args);
                this.PrefixObject?.UpdateLocalInfo(args);
                if (list == null || list.Count == 0)
                {
                    this._Values = null;
                }
                else
                {
                    this._Values = list.ToArray();
                }
            }
            catch( System.Exception ext )
            {
                this._Values = null;
            }
        }
        public virtual bool UpdateBinaryValueForLocalClassRename()
        {
            bool result = false;
            if (this._Values != null)
            {
                foreach (var item in this._Values)
                {
                    if (item.EnumType != null
                        && item.EnumType.LocalClass != null
                        && item.EnumType.UpdateForLocalClassNameChanged())
                    {
                        result = true;
                    }
                    if (item.Value is DCILCustomAttributeValue.TypeRefInfo)
                    {
                        var info = (DCILCustomAttributeValue.TypeRefInfo)item.Value;
                        if (info.LocalClass != null && info.UpdateForLocalClassNameChanged())
                        {
                            result = true;
                        }
                    }
                }
            }
            if (result)
            {
                var bs = this.BinaryValue;
                this.BinaryValue = DCILCustomAttributeValue.GetBinaryValue(this._Values, this.InvokeInfo);
            }
            return result;
        }

        public DCILCustomAttributeValue[] Values
        {
            get
            {
                return this._Values;
            }
        }
        ///// <summary>
        ///// 为重命名而更新属性值
        ///// </summary>
        ///// <param name="map"></param>
        //public virtual bool UpdateValuesForRename(RenameMapInfo map)
        //{
        //    if (this.AttributeTypeName == "System.ComponentModel.DefaultValueAttribute")
        //    {
        //        return false;
        //    }
        //    if (this.Values == null)
        //    {
        //        return false;
        //    }
        //    bool changed = false;
        //    foreach (var item in this.Values)
        //    {
        //        if (item.ElementType == DCILCustomAttributeValue.DCILElementType.System_Type)
        //        {
        //            var newName = map.GetNewClassName((string)item.Value);
        //            if (newName != null)
        //            {
        //                changed = true;
        //                item.Value = newName;
        //            }
        //        }
        //    }
        //    if (changed)
        //    {
        //        this.BinaryValue = DCILCustomAttributeValue.GetBinaryValue(this.Values, this.InvokeInfo);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public string TypeName
        {
            get
            {
                return this.InvokeInfo?.OwnerType?.Name;
            }
        }
        public override void WriteTo(DCILWriter writer)
        {
            if( this.PrefixObject != null )
            {
                this.PrefixObject.WriteTo(writer); 
            }
            writer.Write(".custom ");
            if( this.Prefix != null && this.Prefix.Length > 0 )
            {
                writer.Write(this.Prefix + " ");
            }
            this.InvokeInfo.WriteTo(writer);
            writer.Write(" = (");
            writer.WriteHexs(this.BinaryValue);
            writer.WriteLine(")");
        }
        public string HexValue = null;
        public override string ToString()
        {
            return ".custom " + this.AttributeTypeName;
        }



    }
}
