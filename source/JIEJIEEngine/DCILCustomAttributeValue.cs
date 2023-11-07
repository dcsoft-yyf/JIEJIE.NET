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
using System.IO;

namespace JIEJIE
{
    internal class DCILCustomAttributeValue : IDisposable
    {
        public DCILCustomAttributeValue()
        {
        }
        public void Dispose()
        {
            if(this.EnumType != null )
            {
                this.EnumType.Dispose();
                this.EnumType = null;
            }
            this.Name = null;
            if(this.Value is IDisposable)
            {
                ((IDisposable)this.Value).Dispose();
            }
        }
        public bool IsBoxed = false;
        public TypeRefInfo EnumType = null;
        /// <summary>
        /// 是否为构造函数使用的参数
        /// </summary>
        public bool IsCtorParamter = false;
        public byte Flag = 0;
        public DCILElementType ElementType = DCILElementType.None;
        public string Name = null;
        public object Value = null;
        public override string ToString()
        {
            return this.Name + "=" + this.Value;
        }

        //public List<AttributeValue> AttriubteValues = null;
        public static byte[] GetBinaryValue(DCILCustomAttributeValue[] values, DCILInvokeMethodInfo invokeInfo)
        {
            var ms = new System.IO.MemoryStream();
            var writer = new System.IO.BinaryWriter(ms);
            writer.Write((short)1);
            int psCount = invokeInfo.Paramters == null ? 0 : invokeInfo.Paramters.Count;
            if (psCount > 0)
            {
                for (int iCount = 0; iCount < psCount; iCount++)
                {
                    var item = values[iCount];
                    if (item.ElementType == DCILElementType.System_Type)
                    {
                        WriteUTF8String(writer, item.Value.ToString());
                    }
                    else if (item.ElementType == DCILElementType.Enum)
                    {
                        WriteUTF8String(writer, item.EnumType.ToString());
                        WriteAttributeValue(writer, item.ElementType, item.Value);
                    }
                    else
                    {
                        WriteAttributeValue(writer, item.ElementType, item.Value);
                    }
                }
            }
            if (psCount < values.Length)
            {
                writer.Write((short)(values.Length - psCount));
                for (int iCount = psCount; iCount < values.Length; iCount++)
                {
                    var item = values[iCount];
                    writer.Write(item.Flag);
                    writer.Write((byte)item.ElementType);
                    if (item.EnumType != null)
                    {
                        WriteUTF8String(writer, item.EnumType.ToString());
                    }
                    WriteUTF8String(writer, item.Name);
                    if (item.IsBoxed)
                    {
                        writer.Write((byte)DCILElementType.Boxed);
                    }
                    WriteAttributeValue(writer, item.ElementType, item.Value);
                }
            }
            else
            {
                writer.Write((short)0);
            }
            var result = ms.ToArray();
            ms.Close();
            return result;
        }

        public static List<DCILCustomAttributeValue> ParseValues(
            byte[] bsValue,
            DCILInvokeMethodInfo invokeInfo,
            ReadCustomAttributeValueArgs args)
        {
            if (bsValue == null || bsValue.Length == 0)
            {
                return null;
            }
            List<DCILCustomAttributeValue> result = new List<DCILCustomAttributeValue>();
            var reader = new System.IO.BinaryReader(new System.IO.MemoryStream(bsValue));
            if (reader.ReadInt16() != 1)
            {
                throw new InvalidOperationException();
            }
            args.Reader = reader;
            if (invokeInfo.Paramters != null && invokeInfo.Paramters.Count > 0)
            {
                // 有参数
                if (_TypeMaps == null)
                {
                    _TypeMaps = new Dictionary<Type, DCILElementType>();
                    _TypeMaps[typeof(string)] = DCILElementType.String;
                    _TypeMaps[typeof(bool)] = DCILElementType.Boolean;
                    _TypeMaps[typeof(sbyte)] = DCILElementType.I1;
                    _TypeMaps[typeof(byte)] = DCILElementType.U1;
                    _TypeMaps[typeof(short)] = DCILElementType.I2;
                    _TypeMaps[typeof(ushort)] = DCILElementType.U2;
                    _TypeMaps[typeof(int)] = DCILElementType.I4;
                    _TypeMaps[typeof(uint)] = DCILElementType.U4;
                    _TypeMaps[typeof(long)] = DCILElementType.I8;
                    _TypeMaps[typeof(ulong)] = DCILElementType.U8;
                    _TypeMaps[typeof(float)] = DCILElementType.R4;
                    _TypeMaps[typeof(double)] = DCILElementType.R8;
                    _TypeMaps[typeof(char)] = DCILElementType.Char;
                    _TypeMaps[typeof(object)] = DCILElementType.Object;
                }
                for (int iCount = 0; iCount < invokeInfo.Paramters.Count; iCount++)
                {
                    var av = new DCILCustomAttributeValue();
                    av.IsCtorParamter = true;
                    var p = invokeInfo.Paramters[iCount];
                    av.Name = p.ValueType.Name;
                    if (p.ValueType.Name == "System.Type")
                    {
                        av.Value = new TypeRefInfo(args);// ReadUTF8String(reader);
                        av.ElementType = DCILElementType.System_Type;
                    }
                    else
                    {
                        var vt = p.ValueType.NativeType;
                        DCILElementType et = DCILElementType.None;
                        if (vt != null)
                        {
                            _TypeMaps.TryGetValue(vt, out et);
                        }
                        if (et == DCILElementType.None
                            && p.ValueType.Mode == DCILTypeMode.ValueType)
                        {
                            et = DCILElementType.I4;
                        }
                        av.Value = ReadAttributeValue(args, et);
                        av.ElementType = et;
                    }
                    result.Add(av);
                }//for
            }
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // 无参数
                int num = reader.ReadUInt16();
                for (int iCount = 0; iCount < num; iCount++)
                {
                    var av = new DCILCustomAttributeValue();
                    av.IsCtorParamter = false;
                    av.Flag = reader.ReadByte();
                    av.ElementType = (DCILElementType)reader.ReadByte();
                    if (av.ElementType == DCILElementType.Enum)
                    {
                        av.EnumType = new TypeRefInfo(args);
                    }
                    av.Name = DCUtils.GetStringUseTable( ReadUTF8String(reader) );
                    if (av.Flag == 83)
                    {

                    }
                    else if (av.Flag == 84)
                    {

                    }
                    else
                    {

                    }
                    if (av.ElementType == DCILElementType.Boxed)
                    {
                        av.IsBoxed = true;
                        av.ElementType = (DCILElementType)reader.ReadByte();
                    }
                    av.Value = ReadAttributeValue(args, av.ElementType);
                    result.Add(av);
                }
            }
            reader.Close();
            args.Reader = null;
            return result;
        }

        private static Dictionary<Type, DCILElementType> _TypeMaps = null;
        protected static void WriteAttributeValue(BinaryWriter writer, DCILElementType type, object Value)
        {
            if (Value is TypeRefInfo)
            {
                ((TypeRefInfo)Value).WriteTo(writer);
                return;
            }
            switch (type)
            {
                case DCILElementType.Boolean:
                    {
                        if ((bool)Value)
                        {
                            writer.Write((byte)1);
                        }
                        else
                        {
                            writer.Write((byte)0);
                        }
                    }
                    break;
                case DCILElementType.I1: writer.Write((sbyte)Value); break;
                case DCILElementType.U1: writer.Write((byte)Value); break;
                case DCILElementType.Char: writer.Write((short)Value); break;
                case DCILElementType.I2: writer.Write((short)Value); break;
                case DCILElementType.U2: writer.Write((ushort)Value); break;
                case DCILElementType.I4: writer.Write((int)Value); break;
                case DCILElementType.U4: writer.Write((uint)Value); break;
                case DCILElementType.I8: writer.Write((long)Value); break;
                case DCILElementType.U8: writer.Write((ulong)Value); break;
                case DCILElementType.R4: writer.Write((float)Value); break;
                case DCILElementType.R8: writer.Write((double)Value); break;
                case DCILElementType.String: WriteUTF8String(writer, (string)Value); break;
                case DCILElementType.Type: WriteUTF8String(writer, (string)Value); break;
                case DCILElementType.Enum: writer.Write((int)Value); break;
                default:
                    throw new NotSupportedException(type.ToString());
            }
        }
        internal static bool IsPrimateType(DCILElementType type)
        {
            return type == DCILElementType.Boolean
                || type == DCILElementType.I1
                || type == DCILElementType.U1
                || type == DCILElementType.Char
                || type == DCILElementType.I2
                || type == DCILElementType.U2
                || type == DCILElementType.I4
                || type == DCILElementType.U4
                || type == DCILElementType.I8
                || type == DCILElementType.U8
                || type == DCILElementType.R4
                || type == DCILElementType.R8
                || type == DCILElementType.String
                || type == DCILElementType.Enum
                || type == DCILElementType.Type;
        }
        //internal static object ReadPrimativTypeValue( BinaryReader reader , DCILElementType type , out bool isPrimativeType )
        //{
        //    switch (type)
        //    {
        //        case DCILElementType.Boolean: isPrimativeType = true; return reader.ReadByte() == 1;
        //        case DCILElementType.I1: isPrimativeType = true; return (sbyte)reader.ReadByte();
        //        case DCILElementType.U1: isPrimativeType = true; return reader.ReadByte();
        //        case DCILElementType.Char: isPrimativeType = true; return (char)reader.ReadInt16();
        //        case DCILElementType.I2: isPrimativeType = true; return reader.ReadInt16();
        //        case DCILElementType.U2: isPrimativeType = true; return reader.ReadUInt16();
        //        case DCILElementType.I4: isPrimativeType = true; return reader.ReadInt32();
        //        case DCILElementType.U4: isPrimativeType = true; return reader.ReadUInt32();
        //        case DCILElementType.I8: isPrimativeType = true; return reader.ReadInt64();
        //        case DCILElementType.U8: isPrimativeType = true; return reader.ReadUInt64();
        //        case DCILElementType.R4: isPrimativeType = true; return reader.ReadSingle();
        //        case DCILElementType.R8: isPrimativeType = true; return reader.ReadDouble();
        //        case DCILElementType.String: isPrimativeType = true; return ReadUTF8String(reader);
        //        case DCILElementType.Type: isPrimativeType = true; return ReadUTF8String(reader);
        //        default:
        //            isPrimativeType = false;
        //            return null;
        //    }
        //}
        protected static object ReadAttributeValue(ReadCustomAttributeValueArgs args, DCILElementType type)
        {
            switch (type)
            {
                case DCILElementType.Boolean: return args.Reader.ReadByte() == 1;
                case DCILElementType.I1: return (sbyte)args.Reader.ReadByte();
                case DCILElementType.U1: return args.Reader.ReadByte();
                case DCILElementType.Char: return (char)args.Reader.ReadInt16();
                case DCILElementType.I2: return args.Reader.ReadInt16();
                case DCILElementType.U2: return args.Reader.ReadUInt16();
                case DCILElementType.I4: return args.Reader.ReadInt32();
                case DCILElementType.U4: return args.Reader.ReadUInt32();
                case DCILElementType.I8: return args.Reader.ReadInt64();
                case DCILElementType.U8: return args.Reader.ReadUInt64();
                case DCILElementType.R4: return args.Reader.ReadSingle();
                case DCILElementType.R8: return args.Reader.ReadDouble();
                case DCILElementType.String: return DCUtils.GetStringUseTable( ReadUTF8String(args.Reader));
                case DCILElementType.Enum:
                    return args.Reader.ReadInt32();
                case DCILElementType.Type:
                    return new TypeRefInfo(args);
                case DCILElementType.Object:
                    return new PackageValueInfo(args);
                default:
                    throw new NotSupportedException(type.ToString());
            }
            return null;
        }

        internal class PackageValueInfo : TypeRefInfo
        {
            public PackageValueInfo(ReadCustomAttributeValueArgs args)
            {
                this.ValueType = (DCILElementType)args.Reader.ReadByte();
                if (this.ValueType == DCILElementType.Enum)
                {
                    base.Parse(DCUtils.GetStringUseTable( ReadUTF8String(args.Reader)));
                    base.UpdateLocalInfo(args);
                    if (this.LocalClass != null)
                    {
                        this.EnumByteSize = this.LocalClass.EnumByteSize;
                    }
                    else
                    {
                        var bt = this.NativeType;
                        this.EnumByteSize = DCILClass.GetIntegerByteSize(bt);
                        if (bt == null)
                        {

                        }
                    }
                    this.BinValue = args.Reader.ReadBytes(this.EnumByteSize);
                    switch (this.EnumByteSize)
                    {
                        case 1: this.Value = this.BinValue[0]; break;
                        case 2: this.Value = BitConverter.ToInt16(this.BinValue, 0); break;
                        case 4: this.Value = BitConverter.ToInt32(this.BinValue, 0); break;
                        case 8: this.Value = BitConverter.ToInt64(this.BinValue, 0); break;
                    }
                }
                else if (IsPrimateType(this.ValueType))
                {
                    this.Value = ReadAttributeValue(args, this.ValueType);
                }
                else
                {
                    throw new NotSupportedException(this.ValueType.ToString());
                }
            }
            public int EnumByteSize = 4;
            public DCILElementType ValueType = DCILElementType.None;
            public object Value = null;
            public byte[] BinValue = null;
            public override void WriteTo(BinaryWriter writer)
            {
                if (this.TypeName != null && this.TypeName.Length > 0)
                {
                    writer.Write((byte)this.ValueType);
                    if (this.LocalClass != null)
                    {
                        WriteUTF8String(writer, this.LocalClass.GetNameWithNested('+'));
                    }
                    else
                    {
                        WriteUTF8String(writer, this.ToTypeString());
                    }
                    if (this.ValueType == DCILElementType.Enum)
                    {
                        writer.Write(this.BinValue, 0, this.BinValue.Length);
                    }
                }
                else if (IsPrimateType(this.ValueType))
                {
                    WriteAttributeValue(writer, this.ValueType, this.Value);
                }
                else
                {
                    throw new NotSupportedException(this.ValueType.ToString());
                }
            }
        }



        internal class TypeRefInfo : DCILTypeNameInfo
        {
            public TypeRefInfo()
            {

            }
            public TypeRefInfo(string typeName, ReadCustomAttributeValueArgs args)
            {
                base.Parse(typeName);
                UpdateLocalInfo(args);
            }

            public TypeRefInfo(ReadCustomAttributeValueArgs args)
            {
                var str = DCUtils.GetStringUseTable( ReadUTF8String(args.Reader));
                base.Parse(str);
                UpdateLocalInfo(args);
            }
            public override void Dispose()
            {
                base.Dispose();
                this.LocalClass = null;
                this.NativeType = null;
            }
            public virtual void WriteTo(BinaryWriter writer)
            {
                if (this.LocalClass != null)
                {
                    WriteUTF8String(writer, this.LocalClass.NameWithNested);
                }
                else
                {
                    WriteUTF8String(writer, this.ToTypeString());
                }
            }
            private bool SetLocalClass( DCILDocument doc )
            {
                this.LocalClass = doc.GetClassByName(this.TypeName, '+');
                if(this.LocalClass != null )
                {
                    this.LocalClass.Used = true;
                }
                return this.LocalClass != null;
                //if( this.TypeName.IndexOf('+') < 0 )
                //{
                //    return doc.GetAllClassesUseCache().TryGetValue(this.TypeName, out this.LocalClass);
                //}
                //else
                //{
                //    var tns = this.TypeName.Split('+');
                //    foreach( var cls in doc.Classes )
                //    {
                //        if( cls.Name == tns[0])
                //        {
                //            DCILClass curCls = cls;
                //            for(int iCount = 1; iCount < tns.Length; iCount ++)
                //            {
                //                if(curCls.NestedClasses == null )
                //                {
                //                    break;
                //                }
                //                foreach( var cls2 in curCls.NestedClasses)
                //                {
                //                    if( cls2.Name ==  tns[iCount])
                //                    {
                //                        if(iCount == tns.Length -1 )
                //                        {
                //                            this.LocalClass = cls2;
                //                            return true;
                //                        }
                //                        curCls = cls2;
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                //return false;
            }
            protected void UpdateLocalInfo(ReadCustomAttributeValueArgs args)
            {
                var doc = args.GetDocument(this.AssemblyName);
                if (doc != null)
                {
                    SetLocalClass(doc);
                    return;
                }
                //if (this.HasAssemblyName && args.Documents != null)
                //{
                //    foreach (var doc in args.Documents)
                //    {
                //        if (doc.Name == this.AssemblyName)
                //        {
                //            if(SetLocalClass( doc ))
                //            {
                //                return;
                //            }
                //            break;
                //        }
                //    }
                //}
                //else if (args.MainDocument != null)
                //{
                //    if(SetLocalClass(args.MainDocument ))
                //    {
                //        return;
                //    }
                //}
                var t2 = new DCILTypeReference(this.TypeName, DCILTypeMode.Unsigned);
                t2.LibraryName = this.AssemblyName;
                this.NativeType = t2.SearchNativeType(args.AssemblySeachPath);
            }

            public bool UpdateForLocalClassNameChanged()
            {
                bool result = false;
                if (this.HasAssemblyName)
                {
                    //if (this.AssemblyName.StartsWith("System."))
                    //{

                    //}
                    this.AssemblyName = null;
                    this.AssemblyCulture = null;
                    this.AssemblyPublicKeyToken = null;
                    this.AssemblyVersion = null;
                    result = true;
                }
                if (this.LocalClass.RenameState == DCILRenameState.Renamed)
                {
                    result = true;
                }
                return result;
            }

            public DCILClass LocalClass = null;
            public Type NativeType = null;
            public override string ToString()
            {
                if (this.LocalClass == null)
                {
                    return base.ToTypeString();
                }
                else
                {
                    return this.LocalClass.GetNameWithNested('+');
                }
            }
        }



        protected static void WriteUTF8String(System.IO.BinaryWriter writer, string text)
        {
            if (text == null)
            {
                writer.Write((byte)0xff);
                return;
            }
            if (text.IndexOf("DCSoft.Common,") >= 0)
            {

            }
            if (text.Length == 0)
            {
                writer.Write((byte)0);
                return;
            }
            var bsData = System.Text.Encoding.UTF8.GetBytes(text);
            var len = bsData.Length;
            if (len < 128)
            {
                writer.Write((byte)len);
            }
            else if (len < 16384)
            {
                writer.Write((byte)(0x80 | (len >> 8)));
                writer.Write((byte)(len & 0xFF));
            }
            else
            {
                writer.Write((byte)((len >> 24) | 0xC0));
                writer.Write((byte)((len >> 16) & 0xFF));
                writer.Write((byte)((len >> 8) & 0xFF));
                writer.Write((byte)(len & 0xFF));
            }
            writer.Write(bsData);
        }
        protected static string ReadUTF8String(System.IO.BinaryReader reader)
        {
            uint bsLength = 0;
            byte b = reader.ReadByte();
            if (b == 0xff)
            {
                return null;
            }
            if (b == 0)
            {
                return string.Empty;
            }
            if ((b & 0x80) == 0)
            {
                bsLength = b;
            }
            else if ((b & 0x40) == 0)
            {
                bsLength = (uint)(((b & -129) << 8) | reader.ReadByte());
            }
            else
            {
                bsLength = (uint)(((b & -193) << 24) | (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte());
            }
            if (bsLength == 0)
            {
                return null;
            }
            var bsTemp = reader.ReadBytes((int)bsLength);
            var str = System.Text.Encoding.UTF8.GetString(bsTemp);
            return str;
        }
        public enum DCILElementType : byte
        {
            None = 0,
            Void = 1,
            Boolean = 2,
            Char = 3,
            I1 = 4,
            U1 = 5,
            I2 = 6,
            U2 = 7,
            I4 = 8,
            U4 = 9,
            I8 = 10,
            U8 = 11,
            R4 = 12,
            R8 = 13,
            String = 14,
            Ptr = 0xF,
            ByRef = 0x10,
            ValueType = 17,
            Class = 18,
            Var = 19,
            Array = 20,
            GenericInst = 21,
            TypedByRef = 22,
            I = 24,
            U = 25,
            FnPtr = 27,
            Object = 28,
            SzArray = 29,
            MVar = 30,
            CModReqD = 0x1F,
            CModOpt = 0x20,
            Internal = 33,
            Modifier = 0x40,
            Sentinel = 65,
            Pinned = 69,
            Type = 80,
            Boxed = 81,
            Enum = 85,
            System_Type = 200
        }

    }

}
