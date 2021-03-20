using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCNETProtector
{
    internal static class ByteArrayDataContainer
    {
        public static string FullClassName = "DCSoft20210314.ByteArrayDataContainer";
        public static string LibName_mscorlib = "mscorlib";
        public static string GetMethodName(byte[] data)
        {
            return GetMethodName(IndexOf(data));
        }
        private static string GetMethodName(int index)
        {
            if (index == 398)
            {

            }
            return FullClassName + "::_" + index;
        }
        public static bool HasData()
        {
            return _Datas.Count > 0;
        }
        private static List<byte[]> _Datas = new List<byte[]>();
        private static int IndexOf(byte[] bsData)
        {
            if (bsData == null || bsData.Length == 0)
            {
                throw new ArgumentNullException("bsData");
            }
            for (int iCount = 0; iCount < _Datas.Count; iCount++)
            {
                var item = _Datas[iCount];
                if (item == bsData)
                {
                    return iCount;
                }
                if (item.Length == bsData.Length)
                {
                    continue;
                }
                int len = item.Length;
                bool equals = true;
                for (int iCount2 = 0; iCount2 < len; iCount2++)
                {
                    if (item[iCount2] != bsData[iCount2])
                    {
                        equals = false;
                        break;
                    }
                }
                if (equals)
                {
                    return iCount;
                }
            }
            _Datas.Add(bsData);
            return _Datas.Count - 1;
        }

        private static readonly string _hexs = "0123456789abcdef";

        public static void WriteDataCode(System.IO.TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            for (int index = 0; index < _Datas.Count; index++)
            {
                writer.WriteLine();
                writer.Write(".data cil I_BDC" + index + "= bytearray(");
                var bs = _Datas[index];
                for (int iCount = 0; iCount < bs.Length; iCount++)
                {
                    if ((iCount % 32) == 0)
                    {
                        writer.WriteLine();
                        writer.Write("      ");
                    }
                    var bv = bs[iCount];
                    writer.Write(_hexs[bv >> 4]);
                    writer.Write(_hexs[bv & 0xf]);

                    //writer.Write(bs[iCount].ToString("X2"));
                    writer.Write(' ');
                }
                writer.WriteLine(")");
            }
        }

        public static void WriteClassSourceCode(StringBuilder str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            str.AppendLine(".class private auto ansi abstract sealed beforefieldinit " + FullClassName + " extends[" + LibName_mscorlib + "]System.Object");
            str.AppendLine("{");
            for (int iCount = 0; iCount < _Datas.Count; iCount++)
            {
                str.AppendLine(@".class nested private explicit ansi sealed _DATA" + iCount + @" extends[mscorlib]System.ValueType
{
	.pack 1
    .size " + _Datas[iCount].Length + @"
}");
            }
            for (int iCount = 0; iCount < _Datas.Count; iCount++)
            {
                str.AppendLine(".field assembly static initonly valuetype " + FullClassName + "/_DATA" + iCount + " _Field" + iCount + " at I_BDC" + iCount);
            }
            for (int iCount = 0; iCount < _Datas.Count; iCount++)
            {
                str.AppendLine(@".method public hidebysig static uint8[] _" + iCount + @"() cil managed 
{
	// Method begins at RVA 0x2b64
	// Code size 23 (0x17)
	.maxstack 3
	.locals init (
		[0] uint8[]
	)
	IL_0000: nop
	IL_0001: ldc.i4 " + _Datas[iCount].Length + @"
	IL_0002: newarr [" + LibName_mscorlib + @"]System.Byte
	IL_0007: dup
	IL_0008: ldtoken field valuetype " + FullClassName + @"/_DATA" + iCount + " " + FullClassName + @"::_Field" + iCount + @"
    IL_000d: call void [" + LibName_mscorlib + @"]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [" + LibName_mscorlib + @"]System.Array, valuetype [" + LibName_mscorlib + @"]System.RuntimeFieldHandle)
	IL_0012: stloc.0
	IL_0013: br.s IL_0015
	IL_0015: ldloc.0
	IL_0016: ret
}
");
            }
            str.AppendLine("}");
        }
    }
}
