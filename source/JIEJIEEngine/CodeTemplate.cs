using System;
using System.Collections.Generic;
using System.Text;

namespace JIEJIE
{
    /// <summary>
    /// 代码模板
    /// </summary>
    public static class CodeTemplate
    {
		public static readonly string _ClassName_JIEJIEPerformanceCounter = "__DC20211119.JIEJIEPerformanceCounter";

		public static readonly string _Code_Template_JIEJIEPerformanceCounter = @".class public auto ansi abstract sealed beforefieldinit __DC20211119.JIEJIEPerformanceCounter
	extends [System.Runtime]System.Object
{
	// Nested Types
	.class nested private auto ansi beforefieldinit PerformanceMethodComparer
		extends [System.Runtime]System.Object
		implements class [System.Runtime]System.Collections.Generic.IComparer`1<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>
	{
		// Fields
		.field private int32 _Type

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor (
				int32 'type'
			) cil managed 
		{
			// Method begins at RVA 0x23e2f
			// Header size: 1
			// Code size: 14 (0xe)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [System.Runtime]System.Object::.ctor()
			IL_0006: ldarg.0
			IL_0007: ldarg.1
			IL_0008: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethodComparer::_Type
			IL_000d: ret
		} // end of method PerformanceMethodComparer::.ctor

		.method public final hidebysig newslot virtual 
			instance int32 Compare (
				class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod p1,
				class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod p2
			) cil managed 
		{
			// Method begins at RVA 0x23e40
			// Header size: 12
			// Code size: 96 (0x60)
			.maxstack 3

			IL_0000: ldarg.0
			IL_0001: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethodComparer::_Type
			IL_0006: brtrue.s IL_0016

			IL_0008: ldarg.2
			IL_0009: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_000e: ldarg.1
			IL_000f: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_0014: sub
			IL_0015: ret

			IL_0016: ldarg.0
			IL_0017: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethodComparer::_Type
			IL_001c: ldc.i4.1
			IL_001d: bne.un.s IL_003b

			IL_001f: ldarg.2
			IL_0020: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_0025: ldarg.2
			IL_0026: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
			IL_002b: sub
			IL_002c: ldarg.1
			IL_002d: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_0032: ldarg.1
			IL_0033: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
			IL_0038: sub
			IL_0039: sub
			IL_003a: ret

			IL_003b: ldarg.0
			IL_003c: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethodComparer::_Type
			IL_0041: ldc.i4.2
			IL_0042: bne.un.s IL_0052

			IL_0044: ldarg.2
			IL_0045: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
			IL_004a: ldarg.1
			IL_004b: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
			IL_0050: sub
			IL_0051: ret

			IL_0052: ldarg.2
			IL_0053: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::CallCount
			IL_0058: ldarg.1
			IL_0059: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::CallCount
			IL_005e: sub
			IL_005f: ret
		} // end of method PerformanceMethodComparer::Compare

	} // end of class PerformanceMethodComparer

	.class nested private auto ansi beforefieldinit PerformanceMethod
		extends [System.Runtime]System.Object
	{
		// Fields
		.field public initonly int32 MethodIndex
		.field public initonly string MethodName
		.field public int32 TickCount
		.field public int32 CallCount
		.field public int32 ChildTickCount
		.field public class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32> ChildTicks

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor (
				int32 intMethodIndex,
				string name
			) cil managed 
		{
			// Method begins at RVA 0x23eac
			// Header size: 1
			// Code size: 54 (0x36)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: newobj instance void class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32>::.ctor()
			IL_0006: stfld class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32> __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTicks
			IL_000b: ldarg.0
			IL_000c: call instance void [System.Runtime]System.Object::.ctor()
			IL_0011: ldarg.2
			IL_0012: brfalse.s IL_001c

			IL_0014: ldarg.2
			IL_0015: callvirt instance int32 [System.Runtime]System.String::get_Length()
			IL_001a: brtrue.s IL_0027

			IL_001c: ldstr #name#
			IL_0021: newobj instance void [System.Runtime]System.ArgumentNullException::.ctor(string)
			IL_0026: throw

			IL_0027: ldarg.0
			IL_0028: ldarg.2
			IL_0029: stfld string __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::MethodName
			IL_002e: ldarg.0
			IL_002f: ldarg.1
			IL_0030: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::MethodIndex
			IL_0035: ret
		} // end of method PerformanceMethod::.ctor

		.method public hidebysig 
			instance void AddChildTick (
				int32 intMethodIndex,
				int32 tick
			) cil managed 
		{
			// Method begins at RVA 0x23ee4
			// Header size: 12
			// Code size: 93 (0x5d)
			.maxstack 4
			.locals init (
				[0] int32 v
			)

			IL_0000: ldarg.0
			IL_0001: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::MethodIndex
			IL_0006: ldarg.1
			IL_0007: bne.un.s IL_0018

			IL_0009: ldarg.0
			IL_000a: ldarg.0
			IL_000b: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_0010: ldarg.2
			IL_0011: add
			IL_0012: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_0017: ret

			IL_0018: ldarg.0
			IL_0019: ldarg.0
			IL_001a: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
			IL_001f: ldarg.2
			IL_0020: add
			IL_0021: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
			IL_0026: ldsfld bool __DC20211119.JIEJIEPerformanceCounter::_FastMode
			IL_002b: brtrue.s IL_005c

			IL_002d: ldc.i4.0
			IL_002e: stloc.0
			IL_002f: ldarg.0
			IL_0030: ldfld class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32> __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTicks
			IL_0035: ldarg.1
			IL_0036: ldloca.s 0
			IL_0038: callvirt instance bool class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32>::TryGetValue(!0, !1&)
			IL_003d: brfalse.s IL_004f

			IL_003f: ldarg.0
			IL_0040: ldfld class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32> __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTicks
			IL_0045: ldarg.1
			IL_0046: ldloc.0
			IL_0047: ldarg.2
			IL_0048: add
			IL_0049: callvirt instance void class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32>::set_Item(!0, !1)
			IL_004e: ret

			IL_004f: ldarg.0
			IL_0050: ldfld class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32> __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTicks
			IL_0055: ldarg.1
			IL_0056: ldarg.2
			IL_0057: callvirt instance void class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32>::set_Item(!0, !1)

			IL_005c: ret
		} // end of method PerformanceMethod::AddChildTick

	} // end of class PerformanceMethod


	// Fields
	.field private static int64 _StartTimeTick
	.field private static int32 _GlobalStartTick
	.field private static int32 _GlobalEndTick
	.field private static int32 _GlobalTickSpanFix
	.field private static bool _Enabled
	.field private static bool _FastMode
	.field private static class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] _Methods
	.field private static int32[] _CallStack
	.custom instance void [System.Runtime]System.ThreadStaticAttribute::.ctor() = (
		01 00 00 00
	)
	.field private static int32 _CallStackLength
	.custom instance void [System.Runtime]System.ThreadStaticAttribute::.ctor() = (
		01 00 00 00
	)
	.field private static int32 _CallStackPosition
	.custom instance void [System.Runtime]System.ThreadStaticAttribute::.ctor() = (
		01 00 00 00
	)

	// Methods
	.method public hidebysig static 
		void PublicStart () cil managed 
	{
		// Method begins at RVA 0x25d5
		// Header size: 1
		// Code size: 34 (0x22)
		.maxstack 8

		IL_0000: ldc.i4.2
		IL_0001: newarr [System.Runtime]System.String
		IL_0016: call void __DC20211119.JIEJIEPerformanceCounter::SetMethodNames(string[])
		IL_001b: ldc.i4.1
		IL_001c: call void __DC20211119.JIEJIEPerformanceCounter::Start(bool)
		IL_0021: ret
	} // end of method JIEJIEPerformanceCounter::PublicStart

	.method public hidebysig static 
		void SetMethod (
			int32 index,
			string methodName
		) cil managed 
	{
		// Method begins at RVA 0x25f8
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
		IL_0005: ldarg.0
		IL_0006: ldarg.0
		IL_0007: ldarg.1
		IL_0008: newobj instance void __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::.ctor(int32, string)
		IL_000d: stelem.ref
		IL_000e: ret
	} // end of method JIEJIEPerformanceCounter::SetMethod

	.method public hidebysig static 
		void SetMethodNames (
			string[] methodNames
		) cil managed 
	{
		// Method begins at RVA 0x2608
		// Header size: 12
		// Code size: 44 (0x2c)
		.maxstack 5
		.locals init (
			[0] int32 iCount
		)

		IL_0000: ldarg.0
		IL_0001: ldlen
		IL_0002: conv.i4
		IL_0003: newarr __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod
		IL_0008: stsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
		IL_000d: ldc.i4.0
		IL_000e: stloc.0
		IL_000f: br.s IL_0025
		// loop start (head: IL_0025)
			IL_0011: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
			IL_0016: ldloc.0
			IL_0017: ldloc.0
			IL_0018: ldarg.0
			IL_0019: ldloc.0
			IL_001a: ldelem.ref
			IL_001b: newobj instance void __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::.ctor(int32, string)
			IL_0020: stelem.ref
			IL_0021: ldloc.0
			IL_0022: ldc.i4.1
			IL_0023: add
			IL_0024: stloc.0

			IL_0025: ldloc.0
			IL_0026: ldarg.0
			IL_0027: ldlen
			IL_0028: conv.i4
			IL_0029: blt.s IL_0011
		// end loop

		IL_002b: ret
	} // end of method JIEJIEPerformanceCounter::SetMethodNames

	.method public hidebysig static 
		void Start (
			bool fastMode
		) cil managed 
	{
		// Method begins at RVA 0x2640
		// Header size: 12
		// Code size: 78 (0x4e)
		.maxstack 2
		.locals init (
			[0] class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[],
			[1] int32,
			[2] class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod item
		)

		IL_0000: ldarg.0
		IL_0001: stsfld bool __DC20211119.JIEJIEPerformanceCounter::_FastMode
		IL_0006: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
		IL_000b: stloc.0
		IL_000c: ldc.i4.0
		IL_000d: stloc.1
		IL_000e: br.s IL_0031
		// loop start (head: IL_0031)
			IL_0010: ldloc.0
			IL_0011: ldloc.1
			IL_0012: ldelem.ref
			IL_0013: stloc.2
			IL_0014: ldloc.2
			IL_0015: ldc.i4.0
			IL_0016: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
			IL_001b: ldloc.2
			IL_001c: ldfld class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32> __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTicks
			IL_0021: callvirt instance void class [System.Collections]System.Collections.Generic.Dictionary`2<int32, int32>::Clear()
			IL_0026: ldloc.2
			IL_0027: ldc.i4.0
			IL_0028: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_002d: ldloc.1
			IL_002e: ldc.i4.1
			IL_002f: add
			IL_0030: stloc.1

			IL_0031: ldloc.1
			IL_0032: ldloc.0
			IL_0033: ldlen
			IL_0034: conv.i4
			IL_0035: blt.s IL_0010
		// end loop

		IL_0037: ldc.i4.0
		IL_0038: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackPosition
		IL_003d: ldc.i4.1
		IL_003e: stsfld bool __DC20211119.JIEJIEPerformanceCounter::_Enabled
		IL_0043: call int32 __DC20211119.JIEJIEPerformanceCounter::GetCurrentTick()
		IL_0048: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalStartTick
		IL_004d: ret
	} // end of method JIEJIEPerformanceCounter::Start

	.method private hidebysig static 
		int32 GetCurrentTick () cil managed 
	{
		// Method begins at RVA 0x269c
		// Header size: 12
		// Code size: 48 (0x30)
		.maxstack 2
		.locals init (
			[0] valuetype [System.Runtime]System.DateTime
		)

		IL_0000: ldsfld int64 __DC20211119.JIEJIEPerformanceCounter::_StartTimeTick
		IL_0005: brtrue.s IL_001b

		IL_0007: call valuetype [System.Runtime]System.DateTime [System.Runtime]System.DateTime::get_Now()
		IL_000c: stloc.0
		IL_000d: ldloca.s 0
		IL_000f: call instance int64 [System.Runtime]System.DateTime::get_Ticks()
		IL_0014: stsfld int64 __DC20211119.JIEJIEPerformanceCounter::_StartTimeTick
		IL_0019: ldc.i4.0
		IL_001a: ret

		IL_001b: call valuetype [System.Runtime]System.DateTime [System.Runtime]System.DateTime::get_Now()
		IL_0020: stloc.0
		IL_0021: ldloca.s 0
		IL_0023: call instance int64 [System.Runtime]System.DateTime::get_Ticks()
		IL_0028: ldsfld int64 __DC20211119.JIEJIEPerformanceCounter::_StartTimeTick
		IL_002d: sub
		IL_002e: conv.i4
		IL_002f: ret
	} // end of method JIEJIEPerformanceCounter::GetCurrentTick

	.method private hidebysig static 
		int32 GlobalTickSpan () cil managed 
	{
		// Method begins at RVA 0x26d8
		// Header size: 1
		// Code size: 18 (0x12)
		.maxstack 8

		IL_0000: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalTickSpanFix
		IL_0005: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalStartTick
		IL_000a: add
		IL_000b: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalEndTick
		IL_0010: sub
		IL_0011: ret
	} // end of method JIEJIEPerformanceCounter::GlobalTickSpan

	.method public hidebysig static 
		string AnalyseSimple (
			int32 sortType,
			int32 minMilliseconds
		) cil managed 
	{
		// Method begins at RVA 0x26ec
		// Header size: 12
		// Code size: 375 (0x177)
		.maxstack 4
		.locals init (
			[0] class [System.Runtime]System.Text.StringBuilder strResult,
			[1] int32 globalTick,
			[2] class [System.Collections]System.Collections.Generic.List`1<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod> list,
			[3] class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[],
			[4] int32,
			[5] class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod item,
			[6] valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>,
			[7] class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod item
		)

		IL_0000: ldsfld bool __DC20211119.JIEJIEPerformanceCounter::_Enabled
		IL_0005: brfalse.s IL_000c

		IL_0007: call void __DC20211119.JIEJIEPerformanceCounter::Stop()

		IL_000c: ldsfld bool __DC20211119.JIEJIEPerformanceCounter::_FastMode
		IL_0011: brtrue.s IL_0019

		IL_0013: ldstr #Only support fast mode.#
		IL_0018: ret

		IL_0019: newobj instance void [System.Runtime]System.Text.StringBuilder::.ctor()
		IL_001e: stloc.0
		IL_001f: call int32 __DC20211119.JIEJIEPerformanceCounter::GlobalTickSpan()
		IL_0024: ldc.i4 10000
		IL_0029: div
		IL_002a: stloc.1
		IL_002b: ldloc.0
		IL_002c: ldstr #Program total span #
		IL_0031: ldloca.s 1
		IL_0033: call instance string [System.Runtime]System.Int32::ToString()
		IL_0038: ldstr # milliseconds,#
		IL_003d: call string [System.Runtime]System.String::Concat(string, string, string)
		IL_0042: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
		IL_0047: pop
		IL_0048: newobj instance void class [System.Collections]System.Collections.Generic.List`1<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>::.ctor()
		IL_004d: stloc.2
		IL_004e: ldarg.1
		IL_004f: ldc.i4 10000
		IL_0054: mul
		IL_0055: starg.s minMilliseconds
		IL_0057: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
		IL_005c: stloc.3
		IL_005d: ldc.i4.0
		IL_005e: stloc.s 4
		IL_0060: br.s IL_0080
		// loop start (head: IL_0080)
			IL_0062: ldloc.3
			IL_0063: ldloc.s 4
			IL_0065: ldelem.ref
			IL_0066: stloc.s 5
			IL_0068: ldloc.s 5
			IL_006a: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_006f: ldarg.1
			IL_0070: ble.s IL_007a

			IL_0072: ldloc.2
			IL_0073: ldloc.s 5
			IL_0075: callvirt instance void class [System.Collections]System.Collections.Generic.List`1<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>::Add(!0)

			IL_007a: ldloc.s 4
			IL_007c: ldc.i4.1
			IL_007d: add
			IL_007e: stloc.s 4

			IL_0080: ldloc.s 4
			IL_0082: ldloc.3
			IL_0083: ldlen
			IL_0084: conv.i4
			IL_0085: blt.s IL_0062
		// end loop

		IL_0087: ldloc.2
		IL_0088: ldarg.0
		IL_0089: newobj instance void __DC20211119.JIEJIEPerformanceCounter/PerformanceMethodComparer::.ctor(int32)
		IL_008e: callvirt instance void class [System.Collections]System.Collections.Generic.List`1<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>::Sort(class [System.Runtime]System.Collections.Generic.IComparer`1<!0>)
		IL_0093: ldsfld bool __DC20211119.JIEJIEPerformanceCounter::_FastMode
		IL_0098: brfalse IL_0170

		IL_009d: ldarg.0
		IL_009e: brtrue.s IL_00ae

		IL_00a0: ldloc.0
		IL_00a1: ldstr # list order by Total is:#
		IL_00a6: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::AppendLine(string)
		IL_00ab: pop
		IL_00ac: br.s IL_00de

		IL_00ae: ldarg.0
		IL_00af: ldc.i4.1
		IL_00b0: bne.un.s IL_00c0

		IL_00b2: ldloc.0
		IL_00b3: ldstr # list order by Private is:#
		IL_00b8: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::AppendLine(string)
		IL_00bd: pop
		IL_00be: br.s IL_00de

		IL_00c0: ldarg.0
		IL_00c1: ldc.i4.2
		IL_00c2: bne.un.s IL_00d2

		IL_00c4: ldloc.0
		IL_00c5: ldstr # list order by Child is:#
		IL_00ca: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::AppendLine(string)
		IL_00cf: pop
		IL_00d0: br.s IL_00de

		IL_00d2: ldloc.0
		IL_00d3: ldstr # list order by Call is:#
		IL_00d8: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::AppendLine(string)
		IL_00dd: pop

		IL_00de: ldloc.0
		IL_00df: ldstr #     Total  |  Private |  Child  |   Call  |  Name#
		IL_00e4: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::AppendLine(string)
		IL_00e9: pop
		IL_00ea: ldloc.2
		IL_00eb: callvirt instance valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<!0> class [System.Collections]System.Collections.Generic.List`1<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>::GetEnumerator()
		IL_00f0: stloc.s 6
		.try
		{
			IL_00f2: br.s IL_0157
			// loop start (head: IL_0157)
				IL_00f4: ldloca.s 6
				IL_00f6: call instance !0 valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>::get_Current()
				IL_00fb: stloc.s 7
				IL_00fd: ldloc.0
				IL_00fe: ldloc.s 7
				IL_0100: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
				IL_0105: ldc.i4.1
				IL_0106: call void __DC20211119.JIEJIEPerformanceCounter::AppendTick(class [System.Runtime]System.Text.StringBuilder, int32, bool)
				IL_010b: ldloc.0
				IL_010c: ldloc.s 7
				IL_010e: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
				IL_0113: ldloc.s 7
				IL_0115: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
				IL_011a: sub
				IL_011b: ldc.i4.1
				IL_011c: call void __DC20211119.JIEJIEPerformanceCounter::AppendTick(class [System.Runtime]System.Text.StringBuilder, int32, bool)
				IL_0121: ldloc.0
				IL_0122: ldloc.s 7
				IL_0124: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::ChildTickCount
				IL_0129: ldc.i4.1
				IL_012a: call void __DC20211119.JIEJIEPerformanceCounter::AppendTick(class [System.Runtime]System.Text.StringBuilder, int32, bool)
				IL_012f: ldloc.0
				IL_0130: ldloc.s 7
				IL_0132: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::CallCount
				IL_0137: ldc.i4.0
				IL_0138: call void __DC20211119.JIEJIEPerformanceCounter::AppendTick(class [System.Runtime]System.Text.StringBuilder, int32, bool)
				IL_013d: ldloc.0
				IL_013e: ldstr #      #
				IL_0143: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
				IL_0148: pop
				IL_0149: ldloc.0
				IL_014a: ldloc.s 7
				IL_014c: ldfld string __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::MethodName
				IL_0151: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::AppendLine(string)
				IL_0156: pop

				IL_0157: ldloca.s 6
				IL_0159: call instance bool valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>::MoveNext()
				IL_015e: brtrue.s IL_00f4
			// end loop

			IL_0160: leave.s IL_0170
		} // end .try
		finally
		{
			IL_0162: ldloca.s 6
			IL_0164: constrained. valuetype [System.Collections]System.Collections.Generic.List`1/Enumerator<class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod>
			IL_016a: callvirt instance void [System.Runtime]System.IDisposable::Dispose()
			IL_016f: endfinally
		} // end handler

		IL_0170: ldloc.0
		IL_0171: callvirt instance string [System.Runtime]System.Object::ToString()
		IL_0176: ret
	} // end of method JIEJIEPerformanceCounter::AnalyseSimple

	.method private hidebysig static 
		void AppendTick (
			class [System.Runtime]System.Text.StringBuilder str,
			int32 num,
			bool isTick
		) cil managed 
	{
		// Method begins at RVA 0x2880
		// Header size: 12
		// Code size: 55 (0x37)
		.maxstack 4
		.locals init (
			[0] string txt,
			[1] int32
		)

		IL_0000: ldarg.2
		IL_0001: brtrue.s IL_000c

		IL_0003: ldarga.s num
		IL_0005: call instance string [System.Runtime]System.Int32::ToString()
		IL_000a: br.s IL_001b

		IL_000c: ldarg.1
		IL_000d: ldc.i4 10000
		IL_0012: div
		IL_0013: stloc.1
		IL_0014: ldloca.s 1
		IL_0016: call instance string [System.Runtime]System.Int32::ToString()

		IL_001b: stloc.0
		IL_001c: ldarg.0
		IL_001d: ldc.i4.s 32
		IL_001f: ldc.i4.s 10
		IL_0021: ldloc.0
		IL_0022: callvirt instance int32 [System.Runtime]System.String::get_Length()
		IL_0027: sub
		IL_0028: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(char, int32)
		IL_002d: pop
		IL_002e: ldarg.0
		IL_002f: ldloc.0
		IL_0030: callvirt instance class [System.Runtime]System.Text.StringBuilder [System.Runtime]System.Text.StringBuilder::Append(string)
		IL_0035: pop
		IL_0036: ret
	} // end of method JIEJIEPerformanceCounter::AppendTick

	.method public hidebysig static 
		int32 Enter (
			int32 intMethodIndex
		) cil managed 
	{
		// Method begins at RVA 0x28c4
		// Header size: 12
		// Code size: 159 (0x9f)
		.maxstack 3
		.locals init (
			[0] int32 index,
			[1] int32[] temp
		)

		IL_0000: ldsfld bool __DC20211119.JIEJIEPerformanceCounter::_Enabled
		IL_0005: brtrue.s IL_0009

		IL_0007: ldc.i4.m1
		IL_0008: ret

		IL_0009: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackPosition
		IL_000e: ldc.i4.2
		IL_000f: add
		IL_0010: dup
		IL_0011: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackPosition
		IL_0016: stloc.0
		IL_0017: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_001c: brtrue.s IL_0040

		IL_001e: ldc.i4.s 20
		IL_0020: newarr [System.Runtime]System.Int32
		IL_0025: stsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_002a: ldc.i4.0
		IL_002b: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackPosition
		IL_0030: ldc.i4.0
		IL_0031: stloc.0
		IL_0032: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_0037: ldlen
		IL_0038: conv.i4
		IL_0039: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackLength
		IL_003e: br.s IL_0073

		IL_0040: ldloc.0
		IL_0041: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackLength
		IL_0046: bne.un.s IL_0073

		IL_0048: ldloc.0
		IL_0049: ldc.i4.1
		IL_004a: shl
		IL_004b: newarr [System.Runtime]System.Int32
		IL_0050: stloc.1
		IL_0051: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_0056: ldloc.1
		IL_0057: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackLength
		IL_005c: call void [System.Runtime]System.Array::Copy(class [System.Runtime]System.Array, class [System.Runtime]System.Array, int32)
		IL_0061: ldloc.1
		IL_0062: stsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_0067: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_006c: ldlen
		IL_006d: conv.i4
		IL_006e: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackLength

		IL_0073: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_0078: ldloc.0
		IL_0079: ldarg.0
		IL_007a: stelem.i4
		IL_007b: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_0080: ldloc.0
		IL_0081: ldc.i4.1
		IL_0082: add
		IL_0083: call int32 __DC20211119.JIEJIEPerformanceCounter::GetCurrentTick()
		IL_0088: stelem.i4
		IL_0089: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
		IL_008e: ldarg.0
		IL_008f: ldelem.ref
		IL_0090: dup
		IL_0091: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::CallCount
		IL_0096: ldc.i4.1
		IL_0097: add
		IL_0098: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::CallCount
		IL_009d: ldloc.0
		IL_009e: ret
	} // end of method JIEJIEPerformanceCounter::Enter

	.method public hidebysig static 
		void Leave (
			int32 intHandle
		) cil managed 
	{
		// Method begins at RVA 0x2970
		// Header size: 12
		// Code size: 284 (0x11c)
		.maxstack 4
		.locals init (
			[0] int32 tick,
			[1] int32 tickSpan,
			[2] int32 mIndex,
			[3] int32 iCount,
			[4] int32 tickSpan,
			[5] int32 mIndex
		)

		IL_0000: ldsfld bool __DC20211119.JIEJIEPerformanceCounter::_Enabled
		IL_0005: brfalse.s IL_000b

		IL_0007: ldarg.0
		IL_0008: ldc.i4.0
		IL_0009: bge.s IL_000c

		IL_000b: ret

		IL_000c: call int32 __DC20211119.JIEJIEPerformanceCounter::GetCurrentTick()
		IL_0011: stloc.0
		IL_0012: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackPosition
		IL_0017: ldarg.0
		IL_0018: bne.un.s IL_007c

		IL_001a: ldloc.0
		IL_001b: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_0020: ldarg.0
		IL_0021: ldc.i4.1
		IL_0022: add
		IL_0023: ldelem.i4
		IL_0024: sub
		IL_0025: stloc.1
		IL_0026: ldloc.1
		IL_0027: ldc.i4.0
		IL_0028: bge.s IL_0041

		IL_002a: ldstr #tickspan=#
		IL_002f: ldloca.s 1
		IL_0031: call instance string [System.Runtime]System.Int32::ToString()
		IL_0036: call string [System.Runtime]System.String::Concat(string, string)
		IL_003b: newobj instance void [System.Runtime]System.Exception::.ctor(string)
		IL_0040: throw

		IL_0041: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_0046: ldarg.0
		IL_0047: ldelem.i4
		IL_0048: stloc.2
		IL_0049: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
		IL_004e: ldloc.2
		IL_004f: ldelem.ref
		IL_0050: dup
		IL_0051: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
		IL_0056: ldloc.1
		IL_0057: add
		IL_0058: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
		IL_005d: ldarg.0
		IL_005e: ldc.i4.0
		IL_005f: ble IL_00f0

		IL_0064: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
		IL_0069: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_006e: ldarg.0
		IL_006f: ldc.i4.2
		IL_0070: sub
		IL_0071: ldelem.i4
		IL_0072: ldelem.ref
		IL_0073: ldloc.2
		IL_0074: ldloc.1
		IL_0075: callvirt instance void __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::AddChildTick(int32, int32)
		IL_007a: br.s IL_00f0

		IL_007c: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackPosition
		IL_0081: stloc.3
		IL_0082: br.s IL_00ec
		// loop start (head: IL_00ec)
			IL_0084: ldloc.0
			IL_0085: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
			IL_008a: ldloc.3
			IL_008b: ldc.i4.1
			IL_008c: add
			IL_008d: ldelem.i4
			IL_008e: sub
			IL_008f: stloc.s 4
			IL_0091: ldloc.s 4
			IL_0093: ldc.i4.0
			IL_0094: bge.s IL_00ad

			IL_0096: ldstr #tickspan=#
			IL_009b: ldloca.s 4
			IL_009d: call instance string [System.Runtime]System.Int32::ToString()
			IL_00a2: call string [System.Runtime]System.String::Concat(string, string)
			IL_00a7: newobj instance void [System.Runtime]System.Exception::.ctor(string)
			IL_00ac: throw

			IL_00ad: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
			IL_00b2: ldloc.3
			IL_00b3: ldelem.i4
			IL_00b4: stloc.s 5
			IL_00b6: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
			IL_00bb: ldloc.s 5
			IL_00bd: ldelem.ref
			IL_00be: dup
			IL_00bf: ldfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_00c4: ldloc.s 4
			IL_00c6: add
			IL_00c7: stfld int32 __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::TickCount
			IL_00cc: ldloc.3
			IL_00cd: ldc.i4.0
			IL_00ce: ble.s IL_00e8

			IL_00d0: ldsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
			IL_00d5: ldsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
			IL_00da: ldloc.3
			IL_00db: ldc.i4.2
			IL_00dc: sub
			IL_00dd: ldelem.i4
			IL_00de: ldelem.ref
			IL_00df: ldloc.s 5
			IL_00e1: ldloc.s 4
			IL_00e3: callvirt instance void __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod::AddChildTick(int32, int32)

			IL_00e8: ldloc.3
			IL_00e9: ldc.i4.2
			IL_00ea: sub
			IL_00eb: stloc.3

			IL_00ec: ldloc.3
			IL_00ed: ldarg.0
			IL_00ee: bge.s IL_0084
		// end loop

		IL_00f0: ldarg.0
		IL_00f1: brtrue.s IL_0113

		IL_00f3: ldsfld int64 __DC20211119.JIEJIEPerformanceCounter::_StartTimeTick
		IL_00f8: ldc.i4.0
		IL_00f9: conv.i8
		IL_00fa: ble.s IL_0113

		IL_00fc: ldsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalTickSpanFix
		IL_0101: call int32 __DC20211119.JIEJIEPerformanceCounter::GetCurrentTick()
		IL_0106: add
		IL_0107: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalTickSpanFix
		IL_010c: ldc.i4.0
		IL_010d: conv.i8
		IL_010e: stsfld int64 __DC20211119.JIEJIEPerformanceCounter::_StartTimeTick

		IL_0113: ldarg.0
		IL_0114: ldc.i4.2
		IL_0115: sub
		IL_0116: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackPosition
		IL_011b: ret
	} // end of method JIEJIEPerformanceCounter::Leave

	.method public hidebysig static 
		void Stop () cil managed 
	{
		// Method begins at RVA 0x2a98
		// Header size: 1
		// Code size: 30 (0x1e)
		.maxstack 8

		IL_0000: ldsfld bool __DC20211119.JIEJIEPerformanceCounter::_Enabled
		IL_0005: brfalse.s IL_001d

		IL_0007: ldc.i4.0
		IL_0008: call void __DC20211119.JIEJIEPerformanceCounter::Leave(int32)
		IL_000d: call int32 __DC20211119.JIEJIEPerformanceCounter::GetCurrentTick()
		IL_0012: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalEndTick
		IL_0017: ldc.i4.0
		IL_0018: stsfld bool __DC20211119.JIEJIEPerformanceCounter::_Enabled

		IL_001d: ret
	} // end of method JIEJIEPerformanceCounter::Stop

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x2ab8
		// Header size: 12
		// Code size: 66 (0x42)
		.maxstack 1

		IL_0000: ldc.i4.0
		IL_0001: conv.i8
		IL_0002: stsfld int64 __DC20211119.JIEJIEPerformanceCounter::_StartTimeTick
		IL_0007: call int32 __DC20211119.JIEJIEPerformanceCounter::GetCurrentTick()
		IL_000c: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalStartTick
		IL_0011: ldc.i4.0
		IL_0012: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalEndTick
		IL_0017: ldc.i4.0
		IL_0018: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_GlobalTickSpanFix
		IL_001d: ldc.i4.0
		IL_001e: stsfld bool __DC20211119.JIEJIEPerformanceCounter::_Enabled
		IL_0023: ldc.i4.1
		IL_0024: stsfld bool __DC20211119.JIEJIEPerformanceCounter::_FastMode
		IL_0029: ldnull
		IL_002a: stsfld class __DC20211119.JIEJIEPerformanceCounter/PerformanceMethod[] __DC20211119.JIEJIEPerformanceCounter::_Methods
		IL_002f: ldnull
		IL_0030: stsfld int32[] __DC20211119.JIEJIEPerformanceCounter::_CallStack
		IL_0035: ldc.i4.0
		IL_0036: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackLength
		IL_003b: ldc.i4.m1
		IL_003c: stsfld int32 __DC20211119.JIEJIEPerformanceCounter::_CallStackPosition
		IL_0041: ret
	} // end of method JIEJIEPerformanceCounter::.cctor

} // end of class __DC20211119.JIEJIEPerformanceCounter
".Replace('#','"');

		public static readonly string _ClassName_JIEJIEHelper = "__DC20211119.JIEJIEHelper";


		public static readonly string _Code_Template_JIEJIEHelper = @"
.class private auto ansi abstract sealed beforefieldinit __DC20211119.JIEJIEHelper
	extends [mscorlib]System.Object
{
	// Nested Types
	.class nested private auto ansi sealed beforefieldinit SMF_ResStream
		extends [mscorlib]System.IO.Stream
	{
		// Fields
		.field private uint8[] _Content
		.field private int64 _Position

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor (
				uint8[] bs
			) cil managed 
		{
			// Method begins at RVA 0x26234
			// Code size 150 (0x96)
			.maxstack 5
			.locals init (
				[0] int32 gzipLen,
				[1] bool,
				[2] class [mscorlib]System.IO.MemoryStream msSource,
				[3] class [System]System.IO.Compression.GZipStream gm
			)

			IL_0000: ldarg.0
			IL_0001: ldnull
			IL_0002: stfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0007: ldarg.0
			IL_0008: ldc.i4.0
			IL_0009: conv.i8
			IL_000a: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_000f: ldarg.0
			IL_0010: call instance void [mscorlib]System.IO.Stream::.ctor()
			IL_0015: nop
			IL_0016: nop
			IL_0017: ldarg.1
			IL_0018: ldc.i4.0
			IL_0019: call int32 [mscorlib]System.BitConverter::ToInt32(uint8[], int32)
			IL_001e: stloc.0
			IL_001f: ldloc.0
			IL_0020: ldc.i4.0
			IL_0021: ceq
			IL_0023: stloc.1
			IL_0024: ldloc.1
			IL_0025: brfalse.s IL_0052

			IL_0027: nop
			IL_0028: ldarg.0
			IL_0029: ldarg.1
			IL_002a: ldlen
			IL_002b: conv.i4
			IL_002c: ldc.i4.4
			IL_002d: sub
			IL_002e: newarr [mscorlib]System.Byte
			IL_0033: stfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0038: ldarg.1
			IL_0039: ldc.i4.4
			IL_003a: ldarg.0
			IL_003b: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0040: ldc.i4.0
			IL_0041: ldarg.0
			IL_0042: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0047: ldlen
			IL_0048: conv.i4
			IL_0049: call void [mscorlib]System.Array::Copy(class [mscorlib]System.Array, int32, class [mscorlib]System.Array, int32, int32)
			IL_004e: nop
			IL_004f: nop
			IL_0050: br.s IL_0095

			IL_0052: nop
			IL_0053: ldarg.1
			IL_0054: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
			IL_0059: stloc.2
			IL_005a: ldloc.2
			IL_005b: ldc.i4.4
			IL_005c: conv.i8
			IL_005d: callvirt instance void [mscorlib]System.IO.Stream::set_Position(int64)
			IL_0062: nop
			IL_0063: ldloc.2
			IL_0064: ldc.i4.0
			IL_0065: newobj instance void [System]System.IO.Compression.GZipStream::.ctor(class [mscorlib]System.IO.Stream, valuetype [System]System.IO.Compression.CompressionMode)
			IL_006a: stloc.3
			IL_006b: ldarg.0
			IL_006c: ldloc.0
			IL_006d: newarr [mscorlib]System.Byte
			IL_0072: stfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0077: ldloc.3
			IL_0078: ldarg.0
			IL_0079: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_007e: ldc.i4.0
			IL_007f: ldloc.0
			IL_0080: callvirt instance int32 [mscorlib]System.IO.Stream::Read(uint8[], int32, int32)
			IL_0085: pop
			IL_0086: ldloc.3
			IL_0087: callvirt instance void [mscorlib]System.IO.Stream::Close()
			IL_008c: nop
			IL_008d: ldloc.2
			IL_008e: callvirt instance void [mscorlib]System.IO.Stream::Close()
			IL_0093: nop
			IL_0094: nop

			IL_0095: ret
		} // end of method SMF_ResStream::.ctor

		.method public hidebysig specialname virtual 
			instance bool get_CanRead () cil managed 
		{
			// Method begins at RVA 0x262d8
			// Code size 7 (0x7)
			.maxstack 1
			.locals init (
				[0] bool
			)

			IL_0000: nop
			IL_0001: ldc.i4.1
			IL_0002: stloc.0
			IL_0003: br.s IL_0005

			IL_0005: ldloc.0
			IL_0006: ret
		} // end of method SMF_ResStream::get_CanRead

		.method public hidebysig specialname virtual 
			instance bool get_CanSeek () cil managed 
		{
			// Method begins at RVA 0x262ec
			// Code size 7 (0x7)
			.maxstack 1
			.locals init (
				[0] bool
			)

			IL_0000: nop
			IL_0001: ldc.i4.1
			IL_0002: stloc.0
			IL_0003: br.s IL_0005

			IL_0005: ldloc.0
			IL_0006: ret
		} // end of method SMF_ResStream::get_CanSeek

		.method public hidebysig specialname virtual 
			instance bool get_CanWrite () cil managed 
		{
			// Method begins at RVA 0x26300
			// Code size 7 (0x7)
			.maxstack 1
			.locals init (
				[0] bool
			)

			IL_0000: nop
			IL_0001: ldc.i4.0
			IL_0002: stloc.0
			IL_0003: br.s IL_0005

			IL_0005: ldloc.0
			IL_0006: ret
		} // end of method SMF_ResStream::get_CanWrite

		.method public hidebysig specialname virtual 
			instance int64 get_Length () cil managed 
		{
			// Method begins at RVA 0x26314
			// Code size 15 (0xf)
			.maxstack 1
			.locals init (
				[0] int64
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0007: ldlen
			IL_0008: conv.i4
			IL_0009: conv.i8
			IL_000a: stloc.0
			IL_000b: br.s IL_000d

			IL_000d: ldloc.0
			IL_000e: ret
		} // end of method SMF_ResStream::get_Length

		.method public hidebysig specialname virtual 
			instance int64 get_Position () cil managed 
		{
			// Method begins at RVA 0x26330
			// Code size 12 (0xc)
			.maxstack 1
			.locals init (
				[0] int64
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0007: stloc.0
			IL_0008: br.s IL_000a

			IL_000a: ldloc.0
			IL_000b: ret
		} // end of method SMF_ResStream::get_Position

		.method public hidebysig specialname virtual 
			instance void set_Position (
				int64 'value'
			) cil managed 
		{
			// Method begins at RVA 0x26348
			// Code size 9 (0x9)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldarg.1
			IL_0003: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0008: ret
		} // end of method SMF_ResStream::set_Position

		.method public hidebysig virtual 
			instance void Flush () cil managed 
		{
			// Method begins at RVA 0x26352
			// Code size 2 (0x2)
			.maxstack 8

			IL_0000: nop
			IL_0001: ret
		} // end of method SMF_ResStream::Flush

		.method public hidebysig virtual 
			instance int32 Read (
				uint8[] buffer,
				int32 offset,
				int32 count
			) cil managed 
		{
			// Method begins at RVA 0x26358
			// Code size 127 (0x7f)
			.maxstack 5
			.locals init (
				[0] int32 len,
				[1] bool,
				[2] bool,
				[3] int32 endIndex,
				[4] int32 iCount,
				[5] bool,
				[6] int32
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0007: ldlen
			IL_0008: conv.i4
			IL_0009: conv.i8
			IL_000a: ldarg.0
			IL_000b: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0010: sub
			IL_0011: conv.i4
			IL_0012: stloc.0
			IL_0013: ldloc.0
			IL_0014: ldarg.3
			IL_0015: cgt
			IL_0017: stloc.1
			IL_0018: ldloc.1
			IL_0019: brfalse.s IL_001f

			IL_001b: nop
			IL_001c: ldarg.3
			IL_001d: stloc.0
			IL_001e: nop

			IL_001f: ldloc.0
			IL_0020: ldc.i4.0
			IL_0021: cgt
			IL_0023: stloc.2
			IL_0024: ldloc.2
			IL_0025: brfalse.s IL_0077

			IL_0027: nop
			IL_0028: ldarg.0
			IL_0029: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_002e: ldarg.0
			IL_002f: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0034: ldarg.1
			IL_0035: ldarg.2
			IL_0036: conv.i8
			IL_0037: ldloc.0
			IL_0038: conv.i8
			IL_0039: call void [mscorlib]System.Array::Copy(class [mscorlib]System.Array, int64, class [mscorlib]System.Array, int64, int64)
			IL_003e: nop
			IL_003f: ldarg.2
			IL_0040: ldloc.0
			IL_0041: add
			IL_0042: stloc.3
			IL_0043: ldarg.2
			IL_0044: stloc.s 4
			IL_0046: br.s IL_005c
			// loop start (head: IL_005c)
				IL_0048: nop
				IL_0049: ldarg.1
				IL_004a: ldloc.s 4
				IL_004c: ldarg.1
				IL_004d: ldloc.s 4
				IL_004f: ldelem.u1
				IL_0050: ldc.i4.s 123
				IL_0052: xor
				IL_0053: conv.u1
				IL_0054: stelem.i1
				IL_0055: nop
				IL_0056: ldloc.s 4
				IL_0058: ldc.i4.1
				IL_0059: add
				IL_005a: stloc.s 4

				IL_005c: ldloc.s 4
				IL_005e: ldloc.3
				IL_005f: clt
				IL_0061: stloc.s 5
				IL_0063: ldloc.s 5
				IL_0065: brtrue.s IL_0048
			// end loop

			IL_0067: ldarg.0
			IL_0068: ldarg.0
			IL_0069: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_006e: ldloc.0
			IL_006f: conv.i8
			IL_0070: add
			IL_0071: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0076: nop

			IL_0077: ldloc.0
			IL_0078: stloc.s 6
			IL_007a: br.s IL_007c

			IL_007c: ldloc.s 6
			IL_007e: ret
		} // end of method SMF_ResStream::Read

		.method public hidebysig virtual 
			instance int64 Seek (
				int64 offset,
				valuetype [mscorlib]System.IO.SeekOrigin origin
			) cil managed 
		{
			// Method begins at RVA 0x263e4
			// Code size 78 (0x4e)
			.maxstack 3
			.locals init (
				[0] valuetype [mscorlib]System.IO.SeekOrigin,
				[1] int64
			)

			IL_0000: nop
			IL_0001: ldarg.2
			IL_0002: stloc.0
			IL_0003: ldloc.0
			IL_0004: switch (IL_0017, IL_0020, IL_0030)

			IL_0015: br.s IL_0043

			IL_0017: ldarg.0
			IL_0018: ldarg.1
			IL_0019: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_001e: br.s IL_0043

			IL_0020: ldarg.0
			IL_0021: ldarg.0
			IL_0022: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0027: ldarg.1
			IL_0028: add
			IL_0029: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_002e: br.s IL_0043

			IL_0030: ldarg.0
			IL_0031: ldarg.0
			IL_0032: ldfld uint8[] __DC20211119.JIEJIEHelper/SMF_ResStream::_Content
			IL_0037: ldlen
			IL_0038: conv.i4
			IL_0039: conv.i8
			IL_003a: ldarg.1
			IL_003b: sub
			IL_003c: stfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0041: br.s IL_0043

			IL_0043: ldarg.0
			IL_0044: ldfld int64 __DC20211119.JIEJIEHelper/SMF_ResStream::_Position
			IL_0049: stloc.1
			IL_004a: br.s IL_004c

			IL_004c: ldloc.1
			IL_004d: ret
		} // end of method SMF_ResStream::Seek

		.method public hidebysig virtual 
			instance void SetLength (
				int64 'value'
			) cil managed 
		{
			// Method begins at RVA 0x2643e
			// Code size 7 (0x7)
			.maxstack 8

			IL_0000: nop
			IL_0001: newobj instance void [mscorlib]System.NotImplementedException::.ctor()
			IL_0006: throw
		} // end of method SMF_ResStream::SetLength

		.method public hidebysig virtual 
			instance void Write (
				uint8[] buffer,
				int32 offset,
				int32 count
			) cil managed 
		{
			// Method begins at RVA 0x26446
			// Code size 7 (0x7)
			.maxstack 8

			IL_0000: nop
			IL_0001: newobj instance void [mscorlib]System.NotImplementedException::.ctor()
			IL_0006: throw
		} // end of method SMF_ResStream::Write

		// Properties
		.property instance bool CanRead()
		{
			.get instance bool __DC20211119.JIEJIEHelper/SMF_ResStream::get_CanRead()
		}
		.property instance bool CanSeek()
		{
			.get instance bool __DC20211119.JIEJIEHelper/SMF_ResStream::get_CanSeek()
		}
		.property instance bool CanWrite()
		{
			.get instance bool __DC20211119.JIEJIEHelper/SMF_ResStream::get_CanWrite()
		}
		.property instance int64 Length()
		{
			.get instance int64 __DC20211119.JIEJIEHelper/SMF_ResStream::get_Length()
		}
		.property instance int64 Position()
		{
			.get instance int64 __DC20211119.JIEJIEHelper/SMF_ResStream::get_Position()
			.set instance void __DC20211119.JIEJIEHelper/SMF_ResStream::set_Position(int64)
		}

	} // end of class SMF_ResStream


	// Fields
	.field private static initonly class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __SMF_Contents
	.field private static initonly class [mscorlib]System.Reflection.Assembly ThisAssembly
	.field private static class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_Thread
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event
	.field private static initonly class [mscorlib]System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner
	.field private static string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) _CloneStringCrossThead_CurrentValue

.method private hidebysig static 
	class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> SMF_CreateEmptyTable () cil managed 
{
	// Method begins at RVA 0x2050
	// Code size 30 (0x1e)
	.maxstack 8
    IL_0001: newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>::.ctor()
    IL_0002: ret
} // end of method JIEJIEHelper::SMF_CreateEmptyTable


.method private hidebysig static 
	uint8[] SMF_GetContent (
		string name
	) cil managed 
{
	// Method begins at RVA 0x20a4
	// Code size 7 (0x7)
	.maxstack 1
	.locals init (
		[0] uint8[]
	)

	// {
	IL_0000: nop
	// return null;
	IL_0001: ldnull
	IL_0002: stloc.0
	// (no C# code)
	IL_0003: br.s IL_0005

	IL_0005: ldloc.0
	IL_0006: ret
} // end of method JIEJIEHelper::SMF_GetContent

 .method public hidebysig static 
	class [mscorlib]System.Reflection.ManifestResourceInfo SMF_GetManifestResourceInfo (
		class [mscorlib]System.Reflection.Assembly asm,
		string resourceName
	) cil managed 
{
	// Method begins at RVA 0x2050
	// Code size 70 (0x46)
	.maxstack 3
	.locals init (
		[0] bool,
		[1] bool,
		[2] class [mscorlib]System.Reflection.ManifestResourceInfo
	)

	// {
	IL_0000: nop
	// if (ThisAssembly.Equals(asm) && __SMF_Contents != null && __SMF_Contents.ContainsKey(resourceName))
	IL_0001: ldsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
	IL_0006: ldarg.0
	IL_0007: callvirt instance bool [mscorlib]System.Object::Equals(object)
	IL_000c: brfalse.s IL_0018

	IL_000e: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
	IL_0013: ldnull
	IL_0014: cgt.un
	IL_0016: br.s IL_0019

	// (no C# code)
	IL_0018: ldc.i4.0

	IL_0019: stloc.0
	IL_001a: ldloc.0
	IL_001b: brfalse.s IL_003a

	IL_001d: nop
	IL_001e: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
	IL_0023: ldarg.1
	IL_0024: callvirt instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>::ContainsKey(!0)
	IL_0029: stloc.1
	IL_002a: ldloc.1
	IL_002b: brfalse.s IL_0039

	// return new ManifestResourceInfo(asm, resourceName, ResourceLocation.Embedded);
	IL_002d: nop
	IL_002e: ldarg.0
	IL_002f: ldarg.1
	IL_0030: ldc.i4.1
	IL_0031: newobj instance void [mscorlib]System.Reflection.ManifestResourceInfo::.ctor(class [mscorlib]System.Reflection.Assembly, string, valuetype [mscorlib]System.Reflection.ResourceLocation)
	IL_0036: stloc.2
	// (no C# code)
	IL_0037: br.s IL_0044

	// return asm.GetManifestResourceInfo(resourceName);
	IL_0039: nop

	IL_003a: ldarg.0
	IL_003b: ldarg.1
	IL_003c: callvirt instance class [mscorlib]System.Reflection.ManifestResourceInfo [mscorlib]System.Reflection.Assembly::GetManifestResourceInfo(string)
	IL_0041: stloc.2
	// (no C# code)
	IL_0042: br.s IL_0044

	IL_0044: ldloc.2
	IL_0045: ret
} // end of method JIEJIEHelper::SMF_GetManifestResourceInfo

.method public hidebysig static 
	string[] SMF_GetManifestResourceNames (
		class [mscorlib]System.Reflection.Assembly asm
	) cil managed 
{
	// Method begins at RVA 0x20d4
	// Code size 67 (0x43)
	.maxstack 2
	.locals init (
		[0] class [mscorlib]System.Collections.Generic.List`1<string> list,
		[1] string[] names2
	)

	// if (ThisAssembly.Equals(asm) && __SMF_Contents != null)
	IL_0000: ldsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
	IL_0005: ldarg.0
	IL_0006: callvirt instance bool [mscorlib]System.Object::Equals(object)
	IL_000b: brfalse.s IL_003c

	IL_000d: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
	IL_0012: brfalse.s IL_003c

	// List<string> list = new List<string>(__SMF_Contents.Keys);
	IL_0014: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
	IL_0019: callvirt instance class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<!0, !1> class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]>::get_Keys()
	IL_001e: newobj instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
	IL_0023: stloc.0
	// string[] manifestResourceNames = asm.GetManifestResourceNames();
	IL_0024: ldarg.0
	IL_0025: callvirt instance string[] [mscorlib]System.Reflection.Assembly::GetManifestResourceNames()
	IL_002a: stloc.1
	// if (manifestResourceNames != null)
	IL_002b: ldloc.1
	IL_002c: brfalse.s IL_0035

	// list.AddRange(manifestResourceNames);
	IL_002e: ldloc.0
	IL_002f: ldloc.1
	IL_0030: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::AddRange(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)

	// return list.ToArray();
	IL_0035: ldloc.0
	IL_0036: callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<string>::ToArray()
	IL_003b: ret

	// return asm.GetManifestResourceNames();
	IL_003c: ldarg.0
	IL_003d: callvirt instance string[] [mscorlib]System.Reflection.Assembly::GetManifestResourceNames()
	IL_0042: ret
} // end of method JIEJIEHelper::SMF_GetManifestResourceNames

.method public hidebysig static 
	class [System.Runtime]System.IO.Stream SMF_GetManifestResourceStream (
		class [System.Runtime]System.Reflection.Assembly asm,
		string resourceName
	) cil managed 
{
	// Method begins at RVA 0x2130
	// Header size: 12
	// Code size: 74 (0x4a)
	.maxstack 3
	.locals init (
		[0] uint8[] bsContent
	)

	IL_0000: ldsfld class [System.Runtime]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
	IL_0005: ldarg.0
	IL_0006: callvirt instance bool [System.Runtime]System.Object::Equals(object)
	IL_000b: brfalse.s IL_0042

	IL_000d: ldsfld class [System.Collections]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
	IL_0012: brfalse.s IL_0042

	IL_0014: ldnull
	IL_0015: stloc.0
	IL_0016: ldsfld class [System.Collections]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
	IL_001b: ldarg.1
	IL_001c: ldloca.s 0
	IL_001e: callvirt instance bool class [System.Collections]System.Collections.Generic.Dictionary`2<string, uint8[]>::TryGetValue(!0, !1&)
	IL_0023: brfalse.s IL_0042

	IL_0025: ldloc.0
	IL_0026: brtrue.s IL_003b

	IL_0028: ldarg.1
	IL_0029: call uint8[] __DC20211119.JIEJIEHelper::SMF_GetContent(string)
	IL_002e: stloc.0
	IL_002f: ldsfld class [System.Collections]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
	IL_0034: ldarg.1
	IL_0035: ldloc.0
	IL_0036: callvirt instance void class [System.Collections]System.Collections.Generic.Dictionary`2<string, uint8[]>::set_Item(!0, !1)

	IL_003b: ldloc.0
	IL_003c: newobj instance void __DC20211119.JIEJIEHelper/SMF_ResStream::.ctor(uint8[])
	IL_0041: ret

	IL_0042: ldarg.0
	IL_0043: ldarg.1
	IL_0044: callvirt instance class [System.Runtime]System.IO.Stream [System.Runtime]System.Reflection.Assembly::GetManifestResourceStream(string)
	IL_0049: ret
} // end of method JIEJIEHelper::SMF_GetManifestResourceStream

.method public hidebysig static 
	class [mscorlib]System.IO.Stream SMF_GetManifestResourceStream2 (
		class [mscorlib]System.Reflection.Assembly asm,
		class [mscorlib]System.Type t,
		string resourceName
	) cil managed 
{
	// Method begins at RVA 0x217c
	// Code size 125 (0x7d)
	.maxstack 4
	.locals init (
		[0] bool,
		[1] char,
		[2] bool,
		[3] bool,
		[4] class [mscorlib]System.IO.Stream
	)

	// {
	IL_0000: nop
	// if (resourceName == null || resourceName.Length == 0)
	IL_0001: ldarg.2
	IL_0002: brfalse.s IL_000f

	IL_0004: ldarg.2
	IL_0005: callvirt instance int32 [mscorlib]System.String::get_Length()
	IL_000a: ldc.i4.0
	IL_000b: ceq
	IL_000d: br.s IL_0010

	// (no C# code)
	IL_000f: ldc.i4.1

	IL_0010: stloc.0
	IL_0011: ldloc.0
	IL_0012: brfalse.s IL_0025

	// throw new ArgumentNullException('r'.ToString());
	IL_0014: nop
	IL_0015: ldc.i4.s 114
	IL_0017: stloc.1
	// if ((object)t == null)
	IL_0018: ldloca.s 1
	IL_001a: call instance string [mscorlib]System.Char::ToString()
	IL_001f: newobj instance void [mscorlib]System.ArgumentNullException::.ctor(string)
	IL_0024: throw

	IL_0025: ldarg.1
	IL_0026: ldnull
	IL_0027: ceq
	IL_0029: stloc.2
	IL_002a: ldloc.2
	IL_002b: brfalse.s IL_003e

	// throw new ArgumentNullException('t'.ToString());
	IL_002d: nop
	IL_002e: ldc.i4.s 116
	IL_0030: stloc.1
	// if (ThisAssembly.Equals(asm))
	IL_0031: ldloca.s 1
	IL_0033: call instance string [mscorlib]System.Char::ToString()
	IL_0038: newobj instance void [mscorlib]System.ArgumentNullException::.ctor(string)
	IL_003d: throw

	IL_003e: ldsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
	IL_0043: ldarg.0
	IL_0044: callvirt instance bool [mscorlib]System.Object::Equals(object)
	IL_0049: stloc.3
	IL_004a: ldloc.3
	IL_004b: brfalse.s IL_006e

	// return SMF_GetManifestResourceStream(asm, t.FullName + '.' + resourceName);
	IL_004d: nop
	IL_004e: ldarg.0
	IL_004f: ldarg.1
	IL_0050: callvirt instance string [mscorlib]System.Type::get_FullName()
	IL_0055: ldc.i4.s 46
	IL_0057: stloc.1
	IL_0058: ldloca.s 1
	IL_005a: call instance string [mscorlib]System.Char::ToString()
	IL_005f: ldarg.2
	IL_0060: call string [mscorlib]System.String::Concat(string, string, string)
	IL_0065: call class [mscorlib]System.IO.Stream __DC20211119.JIEJIEHelper::SMF_GetManifestResourceStream(class [mscorlib]System.Reflection.Assembly, string)
	IL_006a: stloc.s 4
	// return asm.GetManifestResourceStream(t, resourceName);
	IL_006c: br.s IL_007a

	IL_006e: ldarg.0
	IL_006f: ldarg.1
	IL_0070: ldarg.2
	IL_0071: callvirt instance class [mscorlib]System.IO.Stream [mscorlib]System.Reflection.Assembly::GetManifestResourceStream(class [mscorlib]System.Type, string)
	IL_0076: stloc.s 4
	// (no C# code)
	IL_0078: br.s IL_007a

	IL_007a: ldloc.s 4
	IL_007c: ret
} // end of method JIEJIEHelper::SMF_GetManifestResourceStream2




	.method public hidebysig static 
		string String_Trim (
			string v
		) cil managed 
	{
		// Method begins at RVA 0x21f0
		// Code size 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: callvirt instance string [mscorlib]System.String::Trim()
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method JIEJIEHelper::String_Trim

	.method public hidebysig static 
		class [mscorlib]System.Type Object_GetType (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x2208
		// Code size 12 (0xc)
		.maxstack 1
		.locals init (
			[0] class [mscorlib]System.Type
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: callvirt instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method JIEJIEHelper::Object_GetType

	.method public hidebysig static 
		void MyInitializeArray (
			class [mscorlib]System.Array v,
			valuetype [mscorlib]System.RuntimeFieldHandle fldHandle,
			int32 encKey
		) cil managed 
	{
		// Method begins at RVA 0x2220
		// Code size 137 (0x89)
		.maxstack 3
		.locals init (
			[0] bool,
			[1] uint8* ptr,
			[2] uint8[] pinned,
			[3] int32* ptr2,
			[4] int32* ptr3,
			[5] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array, valuetype [mscorlib]System.RuntimeFieldHandle)
		IL_0008: nop
		IL_0009: ldarg.0
		IL_000a: callvirt instance int32 [mscorlib]System.Array::get_Length()
		IL_000f: ldc.i4.4
		IL_0010: blt.s IL_0018

		IL_0012: ldarg.2
		IL_0013: ldc.i4.0
		IL_0014: ceq
		IL_0016: br.s IL_0019

		IL_0018: ldc.i4.1

		IL_0019: stloc.0
		IL_001a: ldloc.0
		IL_001b: brfalse.s IL_0020

		IL_001d: nop
		IL_001e: br.s IL_0088

		IL_0020: ldarg.0
		IL_0021: castclass uint8[]
		IL_0026: dup
		IL_0027: stloc.2
		IL_0028: brfalse.s IL_002f

		IL_002a: ldloc.2
		IL_002b: ldlen
		IL_002c: conv.i4
		IL_002d: brtrue.s IL_0034

		IL_002f: ldc.i4.0
		IL_0030: conv.u
		IL_0031: stloc.1
		IL_0032: br.s IL_003d

		IL_0034: ldloc.2
		IL_0035: ldc.i4.0
		IL_0036: ldelema [mscorlib]System.Byte
		IL_003b: conv.u
		IL_003c: stloc.1

		IL_003d: nop
		IL_003e: ldloc.1
		IL_003f: stloc.3
		IL_0040: ldloc.3
		IL_0041: ldarg.0
		IL_0042: callvirt instance int32 [mscorlib]System.Array::get_Length()
		IL_0047: conv.r8
		IL_0048: ldc.r8 4
		IL_0051: div
		IL_0052: call float64 [mscorlib]System.Math::Floor(float64)
		IL_0057: conv.i4
		IL_0058: conv.i
		IL_0059: ldc.i4.4
		IL_005a: mul
		IL_005b: add
		IL_005c: ldc.i4.4
		IL_005d: sub
		IL_005e: stloc.s 4
		IL_0060: br.s IL_0077
		// loop start (head: IL_0077)
			IL_0062: nop
			IL_0063: ldloc.s 4
			IL_0065: dup
			IL_0066: ldind.i4
			IL_0067: ldarg.2
			IL_0068: xor
			IL_0069: stind.i4
			IL_006a: ldarg.2
			IL_006b: ldc.i4.s 13
			IL_006d: add
			IL_006e: starg.s encKey
			IL_0070: nop
			IL_0071: ldloc.s 4
			IL_0073: ldc.i4.4
			IL_0074: sub
			IL_0075: stloc.s 4

			IL_0077: ldloc.s 4
			IL_0079: ldloc.3
			IL_007a: clt.un
			IL_007c: ldc.i4.0
			IL_007d: ceq
			IL_007f: stloc.s 5
			IL_0081: ldloc.s 5
			IL_0083: brtrue.s IL_0062
		// end loop

		IL_0085: nop
		IL_0086: ldnull
		IL_0087: stloc.2

		IL_0088: ret
	} // end of method JIEJIEHelper::MyInitializeArray

	.method public hidebysig static 
		string Int32_ToString (
			int32& v
		) cil managed 
	{
		// Method begins at RVA 0x22b8
		// Code size 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance string [mscorlib]System.Int32::ToString()
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method JIEJIEHelper::Int32_ToString

	.method public hidebysig static 
		bool String_Equality (
			string a,
			string b
		) cil managed 
	{
		// Method begins at RVA 0x22d0
		// Code size 13 (0xd)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call bool [mscorlib]System.String::Equals(string, string)
		IL_0008: stloc.0
		IL_0009: br.s IL_000b

		IL_000b: ldloc.0
		IL_000c: ret
	} // end of method JIEJIEHelper::String_Equality

	.method public hidebysig static 
		string String_ConcatObject (
			object a,
			object b
		) cil managed 
	{
		// Method begins at RVA 0x22ec
		// Code size 13 (0xd)
		.maxstack 2
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call string [mscorlib]System.String::Concat(object, object)
		IL_0008: stloc.0
		IL_0009: br.s IL_000b

		IL_000b: ldloc.0
		IL_000c: ret
	} // end of method JIEJIEHelper::String_ConcatObject

	.method public hidebysig static 
		string String_Concat3Object (
			object a,
			object b,
			object c
		) cil managed 
	{
		// Method begins at RVA 0x2308
		// Code size 14 (0xe)
		.maxstack 3
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call string [mscorlib]System.String::Concat(object, object, object)
		IL_0009: stloc.0
		IL_000a: br.s IL_000c

		IL_000c: ldloc.0
		IL_000d: ret
	} // end of method JIEJIEHelper::String_Concat3Object

	.method public hidebysig static 
		string String_Concat3String (
			string a,
			string b,
			string c
		) cil managed 
	{
		// Method begins at RVA 0x2324
		// Code size 14 (0xe)
		.maxstack 3
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call string [mscorlib]System.String::Concat(string, string, string)
		IL_0009: stloc.0
		IL_000a: br.s IL_000c

		IL_000c: ldloc.0
		IL_000d: ret
	} // end of method JIEJIEHelper::String_Concat3String

	.method public hidebysig static 
		string Object_ToString (
			object v
		) cil managed 
	{
		// Method begins at RVA 0x2340
		// Code size 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: callvirt instance string [mscorlib]System.Object::ToString()
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method JIEJIEHelper::Object_ToString

	.method public hidebysig static 
		bool String_IsNullOrEmpty (
			string v
		) cil managed 
	{
		// Method begins at RVA 0x2358
		// Code size 28 (0x1c)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldnull
		IL_0003: cgt.un
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: brfalse.s IL_0016

		IL_0009: nop
		IL_000a: ldarg.0
		IL_000b: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_0010: ldc.i4.0
		IL_0011: ceq
		IL_0013: stloc.1
		IL_0014: br.s IL_001a

		IL_0016: ldc.i4.1
		IL_0017: stloc.1
		IL_0018: br.s IL_001a

		IL_001a: ldloc.1
		IL_001b: ret
	} // end of method JIEJIEHelper::String_IsNullOrEmpty

	.method public hidebysig static 
		void Monitor_Enter (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x2380
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call void [mscorlib]System.Threading.Monitor::Enter(object)
		IL_0007: nop
		IL_0008: ret
	} // end of method JIEJIEHelper::Monitor_Enter

	.method public hidebysig static 
		void Monitor_Enter2 (
			object a,
			bool& lockTaken
		) cil managed 
	{
		// Method begins at RVA 0x238a
		// Code size 10 (0xa)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
		IL_0008: nop
		IL_0009: ret
	} // end of method JIEJIEHelper::Monitor_Enter2

	.method public hidebysig static 
		void Monitor_Exit (
			object a
		) cil managed 
	{
		// Method begins at RVA 0x2395
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call void [mscorlib]System.Threading.Monitor::Exit(object)
		IL_0007: nop
		IL_0008: ret
	} // end of method JIEJIEHelper::Monitor_Exit

	.method public hidebysig static 
		string String_Concat (
			string a,
			string b
		) cil managed 
	{
		// Method begins at RVA 0x23a0
		// Code size 13 (0xd)
		.maxstack 2
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call string [mscorlib]System.String::Concat(string, string)
		IL_0008: stloc.0
		IL_0009: br.s IL_000b

		IL_000b: ldloc.0
		IL_000c: ret
	} // end of method JIEJIEHelper::String_Concat

	.method public hidebysig static 
		void MyDispose (
			class [mscorlib]System.IDisposable obj
		) cil managed 
	{
		// Method begins at RVA 0x23b9
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: callvirt instance void [mscorlib]System.IDisposable::Dispose()
		IL_0007: nop
		IL_0008: ret
	} // end of method JIEJIEHelper::MyDispose

	.method public hidebysig static 
		string CloneStringCrossThead (
			string txt
		) cil managed 
	{
		// Method begins at RVA 0x23c4
		// Code size 163 (0xa3)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] string,
			[2] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: brfalse.s IL_000f

		IL_0004: ldarg.0
		IL_0005: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: br.s IL_0010

		IL_000f: ldc.i4.1

		IL_0010: stloc.0
		IL_0011: ldloc.0
		IL_0012: brfalse.s IL_001c

		IL_0014: nop
		IL_0015: ldarg.0
		IL_0016: stloc.1
		IL_0017: br IL_00a1

		IL_001c: nop
		.try
		{
			IL_001d: nop
			IL_001e: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0023: call void [mscorlib]System.Threading.Monitor::Enter(object)
			IL_0028: nop
			IL_0029: ldarg.0
			IL_002a: volatile.
			IL_002c: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
			IL_0031: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
			IL_0036: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
			IL_003b: pop
			IL_003c: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0041: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0046: pop
			IL_0047: volatile.
			IL_0049: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_004e: ldnull
			IL_004f: ceq
			IL_0051: stloc.2
			IL_0052: ldloc.2
			IL_0053: brfalse.s IL_007c

			IL_0055: nop
			IL_0056: ldnull
			IL_0057: ldftn void __DC20211119.JIEJIEHelper::CloneStringCrossThead_Thread()
			IL_005d: newobj instance void [mscorlib]System.Threading.ThreadStart::.ctor(object, native int)
			IL_0062: newobj instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
			IL_0067: volatile.
			IL_0069: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_006e: volatile.
			IL_0070: ldsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_0075: callvirt instance void [mscorlib]System.Threading.Thread::Start()
			IL_007a: nop
			IL_007b: nop

			IL_007c: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0081: ldc.i4.s 100
			IL_0083: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
			IL_0088: pop
			IL_0089: volatile.
			IL_008b: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
			IL_0090: stloc.1
			IL_0091: leave.s IL_00a1
		} // end .try
		finally
		{
			IL_0093: nop
			IL_0094: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0099: call void [mscorlib]System.Threading.Monitor::Exit(object)
			IL_009e: nop
			IL_009f: nop
			IL_00a0: endfinally
		} // end handler

		IL_00a1: ldloc.1
		IL_00a2: ret
	} // end of method JIEJIEHelper::CloneStringCrossThead

	.method private hidebysig static 
		void CloneStringCrossThead_Thread () cil managed 
	{
		// Method begins at RVA 0x2484
		// Code size 134 (0x86)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool,
			[2] bool
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: br.s IL_005d
			// loop start (head: IL_005d)
				IL_0004: nop
				IL_0005: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
				IL_000a: ldc.i4 1000
				IL_000f: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32)
				IL_0014: ldc.i4.0
				IL_0015: ceq
				IL_0017: stloc.0
				IL_0018: ldloc.0
				IL_0019: brfalse.s IL_001e

				IL_001b: nop
				IL_001c: br.s IL_0061

				IL_001e: volatile.
				IL_0020: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
				IL_0025: ldnull
				IL_0026: cgt.un
				IL_0028: stloc.1
				IL_0029: ldloc.1
				IL_002a: brfalse.s IL_0046

				IL_002c: nop
				IL_002d: volatile.
				IL_002f: ldsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
				IL_0034: callvirt instance char[] [mscorlib]System.String::ToCharArray()
				IL_0039: newobj instance void [mscorlib]System.String::.ctor(char[])
				IL_003e: volatile.
				IL_0040: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
				IL_0045: nop

				IL_0046: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
				IL_004b: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Set()
				IL_0050: pop
				IL_0051: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
				IL_0056: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
				IL_005b: pop
				IL_005c: nop

				IL_005d: ldc.i4.1
				IL_005e: stloc.2
				IL_005f: br.s IL_0004
			// end loop

			IL_0061: nop
			IL_0062: leave.s IL_0085
		} // end .try
		finally
		{
			IL_0064: nop
			IL_0065: ldnull
			IL_0066: volatile.
			IL_0068: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
			IL_006d: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
			IL_0072: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0077: pop
			IL_0078: ldsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
			IL_007d: callvirt instance bool [mscorlib]System.Threading.EventWaitHandle::Reset()
			IL_0082: pop
			IL_0083: nop
			IL_0084: endfinally
		} // end handler

		IL_0085: ret
	} // end of method JIEJIEHelper::CloneStringCrossThead_Thread

	.method public hidebysig static 
		string GetString (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x2528
		// Code size 81 (0x51)
		.maxstack 4
		.locals init (
			[0] int32 chrsLength,
			[1] char[] chrs,
			[2] int32 iCount,
			[3] int32 bi,
			[4] int32 v,
			[5] bool,
			[6] string
		)

		IL_0000: nop
		IL_0001: ldarg.2
		IL_0002: ldc.i4.2
		IL_0003: div
		IL_0004: stloc.0
		IL_0005: ldloc.0
		IL_0006: newarr [mscorlib]System.Char
		IL_000b: stloc.1
		IL_000c: ldc.i4.0
		IL_000d: stloc.2
		IL_000e: br.s IL_003a
		// loop start (head: IL_003a)
			IL_0010: nop
			IL_0011: ldarg.1
			IL_0012: ldloc.2
			IL_0013: ldc.i4.2
			IL_0014: mul
			IL_0015: add
			IL_0016: stloc.3
			IL_0017: ldarg.0
			IL_0018: ldloc.3
			IL_0019: ldelem.u1
			IL_001a: ldc.i4.8
			IL_001b: shl
			IL_001c: ldarg.0
			IL_001d: ldloc.3
			IL_001e: ldc.i4.1
			IL_001f: add
			IL_0020: ldelem.u1
			IL_0021: add
			IL_0022: stloc.s 4
			IL_0024: ldloc.s 4
			IL_0026: ldarg.3
			IL_0027: xor
			IL_0028: stloc.s 4
			IL_002a: ldloc.1
			IL_002b: ldloc.2
			IL_002c: ldloc.s 4
			IL_002e: conv.u2
			IL_002f: stelem.i2
			IL_0030: nop
			IL_0031: ldloc.2
			IL_0032: ldc.i4.1
			IL_0033: add
			IL_0034: stloc.2
			IL_0035: ldarg.3
			IL_0036: ldc.i4.1
			IL_0037: add
			IL_0038: starg.s key

			IL_003a: ldloc.2
			IL_003b: ldloc.0
			IL_003c: clt
			IL_003e: stloc.s 5
			IL_0040: ldloc.s 5
			IL_0042: brtrue.s IL_0010
		// end loop

		IL_0044: ldloc.1
		IL_0045: newobj instance void [mscorlib]System.String::.ctor(char[])
		IL_004a: stloc.s 6
		IL_004c: br.s IL_004e

		IL_004e: ldloc.s 6
		IL_0050: ret
	} // end of method JIEJIEHelper::GetString

	.method public hidebysig static 
		class [System.Drawing]System.Drawing.Bitmap GetBitmap (
			uint8[] bsData,
			int32 startIndex,
			int32 bsLength,
			int32 key
		) cil managed 
	{
		// Method begins at RVA 0x2588
		// Code size 66 (0x42)
		.maxstack 5
		.locals init (
			[0] uint8[] bs,
			[1] class [mscorlib]System.IO.MemoryStream ms,
			[2] class [System.Drawing]System.Drawing.Bitmap bmp,
			[3] int32 iCount,
			[4] bool,
			[5] class [System.Drawing]System.Drawing.Bitmap
		)

		IL_0000: nop
		IL_0001: ldarg.2
		IL_0002: newarr [mscorlib]System.Byte
		IL_0007: stloc.0
		IL_0008: ldc.i4.0
		IL_0009: stloc.3
		IL_000a: br.s IL_0022
		// loop start (head: IL_0022)
			IL_000c: nop
			IL_000d: ldloc.0
			IL_000e: ldloc.3
			IL_000f: ldarg.0
			IL_0010: ldarg.1
			IL_0011: ldloc.3
			IL_0012: add
			IL_0013: ldelem.u1
			IL_0014: ldarg.3
			IL_0015: xor
			IL_0016: conv.u1
			IL_0017: stelem.i1
			IL_0018: nop
			IL_0019: ldloc.3
			IL_001a: ldc.i4.1
			IL_001b: add
			IL_001c: stloc.3
			IL_001d: ldarg.3
			IL_001e: ldc.i4.1
			IL_001f: add
			IL_0020: starg.s key

			IL_0022: ldloc.3
			IL_0023: ldarg.2
			IL_0024: clt
			IL_0026: stloc.s 4
			IL_0028: ldloc.s 4
			IL_002a: brtrue.s IL_000c
		// end loop

		IL_002c: ldloc.0
		IL_002d: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0032: stloc.1
		IL_0033: ldloc.1
		IL_0034: newobj instance void [System.Drawing]System.Drawing.Bitmap::.ctor(class [mscorlib]System.IO.Stream)
		IL_0039: stloc.2
		IL_003a: ldloc.2
		IL_003b: stloc.s 5
		IL_003d: br.s IL_003f

		IL_003f: ldloc.s 5
		IL_0041: ret
	} // end of method JIEJIEHelper::GetBitmap

	.method public hidebysig static 
		class [mscorlib]System.Resources.ResourceSet LoadResourceSet (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{" +
#if DOTNETCORE
						@"
		// Method begins at RVA 0x25d8
		// Code size 30 (0x1e)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.IO.Stream 'stream',
			[1] class [mscorlib]System.Resources.ResourceSet result,
			[2] class [mscorlib]System.Resources.ResourceSet
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call class [mscorlib]System.IO.Stream __DC20211119.JIEJIEHelper::GetStream(uint8[], uint8, bool)
		IL_0009: stloc.0
		IL_000a: ldloc.0
		IL_000b  : newobj instance void [System.Resources.Extensions]System.Resources.Extensions.DeserializingResourceReader::.ctor(class [System.Runtime]System.IO.Stream)
	    IL_001AAA: newobj instance void [System.Resources.ResourceManager]System.Resources.ResourceSet::.ctor(class [System.Resources.ResourceManager]System.Resources.IResourceReader)
	    IL_001BBB: stloc.1
		IL_0011: ldloc.0
		IL_0012: callvirt instance void [mscorlib]System.IO.Stream::Close()
		IL_0017: nop
		IL_0018: ldloc.1
		IL_0019: stloc.2
		IL_001a: br.s IL_001c

		IL_001c: ldloc.2
		IL_001d: ret
	} // end of method JIEJIEHelper::LoadResourceSet
"
#else
            @"
		// Method begins at RVA 0x25d8
		// Code size 30 (0x1e)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.IO.Stream 'stream',
			[1] class [mscorlib]System.Resources.ResourceSet result,
			[2] class [mscorlib]System.Resources.ResourceSet
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: ldarg.2
		IL_0004: call class [mscorlib]System.IO.Stream __DC20211119.JIEJIEHelper::GetStream(uint8[], uint8, bool)
		IL_0009: stloc.0
		IL_000a: ldloc.0
		IL_000b: newobj instance void [mscorlib]System.Resources.ResourceSet::.ctor(class [mscorlib]System.IO.Stream)
		IL_0010: stloc.1
		IL_0011: ldloc.0
		IL_0012: callvirt instance void [mscorlib]System.IO.Stream::Close()
		IL_0017: nop
		IL_0018: ldloc.1
		IL_0019: stloc.2
		IL_001a: br.s IL_001c

		IL_001c: ldloc.2
		IL_001d: ret
	} // end of method JIEJIEHelper::LoadResourceSet
" 
#endif


+ @"
	.method private hidebysig static 
		class [mscorlib]System.IO.Stream GetStream (
			uint8[] bs,
			uint8 key,
			bool gzip
		) cil managed 
	{
		// Method begins at RVA 0x2604
		// Code size 169 (0xa9)
		.maxstack 4
		.locals init (
			[0] int32 len,
			[1] class [mscorlib]System.IO.MemoryStream ms,
			[2] int32 iCount,
			[3] bool,
			[4] bool,
			[5] class [System]System.IO.Compression.GZipStream 'stream',
			[6] uint8[] bsTemp,
			[7] bool,
			[8] bool,
			[9] class [mscorlib]System.IO.Stream
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldlen
		IL_0003: conv.i4
		IL_0004: stloc.0
		IL_0005: ldc.i4.0
		IL_0006: stloc.2
		IL_0007: br.s IL_001e
		// loop start (head: IL_001e)
			IL_0009: nop
			IL_000a: ldarg.0
			IL_000b: ldloc.2
			IL_000c: ldarg.0
			IL_000d: ldloc.2
			IL_000e: ldelem.u1
			IL_000f: ldarg.1
			IL_0010: xor
			IL_0011: conv.u1
			IL_0012: stelem.i1
			IL_0013: nop
			IL_0014: ldloc.2
			IL_0015: ldc.i4.1
			IL_0016: add
			IL_0017: stloc.2
			IL_0018: ldarg.1
			IL_0019: ldc.i4.1
			IL_001a: add
			IL_001b: conv.u1
			IL_001c: starg.s key

			IL_001e: ldloc.2
			IL_001f: ldloc.0
			IL_0020: clt
			IL_0022: stloc.3
			IL_0023: ldloc.3
			IL_0024: brtrue.s IL_0009
		// end loop

		IL_0026: ldnull
		IL_0027: stloc.1
		IL_0028: ldarg.2
		IL_0029: stloc.s 4
		IL_002b: ldloc.s 4
		IL_002d: brfalse.s IL_0098

		IL_002f: nop
		IL_0030: ldarg.0
		IL_0031: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_0036: ldc.i4.0
		IL_0037: newobj instance void [System]System.IO.Compression.GZipStream::.ctor(class [mscorlib]System.IO.Stream, valuetype [System]System.IO.Compression.CompressionMode)
		IL_003c: stloc.s 5
		IL_003e: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor()
		IL_0043: stloc.1
		IL_0044: ldc.i4 1024
		IL_0049: newarr [mscorlib]System.Byte
		IL_004e: stloc.s 6
		IL_0050: br.s IL_007f
		// loop start (head: IL_007f)
			IL_0052: nop
			IL_0053: ldloc.s 5
			IL_0055: ldloc.s 6
			IL_0057: ldc.i4.0
			IL_0058: ldloc.s 6
			IL_005a: ldlen
			IL_005b: conv.i4
			IL_005c: callvirt instance int32 [mscorlib]System.IO.Stream::Read(uint8[], int32, int32)
			IL_0061: stloc.0
			IL_0062: ldloc.0
			IL_0063: ldc.i4.0
			IL_0064: cgt
			IL_0066: stloc.s 7
			IL_0068: ldloc.s 7
			IL_006a: brfalse.s IL_007b

			IL_006c: nop
			IL_006d: ldloc.1
			IL_006e: ldloc.s 6
			IL_0070: ldc.i4.0
			IL_0071: ldloc.0
			IL_0072: callvirt instance void [mscorlib]System.IO.Stream::Write(uint8[], int32, int32)
			IL_0077: nop
			IL_0078: nop
			IL_0079: br.s IL_007e

			IL_007b: nop
			IL_007c: br.s IL_0084

			IL_007e: nop

			IL_007f: ldc.i4.1
			IL_0080: stloc.s 8
			IL_0082: br.s IL_0052
		// end loop

		IL_0084: ldloc.s 5
		IL_0086: callvirt instance void [mscorlib]System.IO.Stream::Close()
		IL_008b: nop
		IL_008c: ldloc.1
		IL_008d: ldc.i4.0
		IL_008e: conv.i8
		IL_008f: callvirt instance void [mscorlib]System.IO.Stream::set_Position(int64)
		IL_0094: nop
		IL_0095: nop
		IL_0096: br.s IL_00a1

		IL_0098: nop
		IL_0099: ldarg.0
		IL_009a: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
		IL_009f: stloc.1
		IL_00a0: nop

		IL_00a1: ldloc.1
		IL_00a2: stloc.s 9
		IL_00a4: br.s IL_00a6

		IL_00a6: ldloc.s 9
		IL_00a8: ret
	} // end of method JIEJIEHelper::GetStream

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x26bc
		// Code size 65 (0x41)
		.maxstack 1

		IL_0000: call class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::SMF_CreateEmptyTable()
	    IL_0005: stsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, uint8[]> __DC20211119.JIEJIEHelper::__SMF_Contents
		IL_0006: ldtoken __DC20211119.JIEJIEHelper
		IL_000b: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
		IL_0010: callvirt instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()
		IL_0015: stsfld class [mscorlib]System.Reflection.Assembly __DC20211119.JIEJIEHelper::ThisAssembly
		IL_001a: ldnull
		IL_001b: volatile.
		IL_001d: stsfld class [mscorlib]System.Threading.Thread modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Thread
		IL_0022: ldc.i4.0
		IL_0023: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_0028: stsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event
		IL_002d: ldc.i4.0
		IL_002e: newobj instance void [mscorlib]System.Threading.AutoResetEvent::.ctor(bool)
		IL_0033: stsfld class [mscorlib]System.Threading.AutoResetEvent __DC20211119.JIEJIEHelper::_CloneStringCrossThead_Event_Inner
		IL_0038: ldnull
		IL_0039: volatile.
		IL_003b: stsfld string modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) __DC20211119.JIEJIEHelper::_CloneStringCrossThead_CurrentValue
		IL_0040: ret
	} // end of method JIEJIEHelper::.cctor

} // end of class __DC20211119.JIEJIEHelper

";

		//******************************************************************************************
		//******************************************************************************************
		//******************************************************************************************
		//******************************************************************************************

		public static readonly string _Code_Template_ComponentResourceManager = @"
.class private auto ansi #CLASSNAME# extends [System]System.ComponentModel.ComponentResourceManager implements [mscorlib]System.IDisposable
{
  .field private class [mscorlib]System.Resources.ResourceSet _Data

.method assembly hidebysig 
	instance void MyApplyResources (
		object v2,
		string objectName
	) cil managed 
{
	.maxstack 8

	IL_0000: ldarg.0
	IL_0001: ldarg.1
	IL_0002: ldarg.2
	IL_0003: ldnull
	IL_0004: callvirt instance void [System]System.ComponentModel.ComponentResourceManager::ApplyResources(object, string, class [mscorlib]System.Globalization.CultureInfo)
	IL_0009: ret
}

.method assembly hidebysig 
	instance string MyGetString (
		string objectName
	) cil managed 
{
	.maxstack 8

	IL_0000: ldarg.0
	IL_0001: ldarg.1
	IL_0004: callvirt instance string [mscorlib]System.Resources.ResourceManager::GetString(string)
	IL_0009: ret
}


.method public hidebysig specialname rtspecialname 
	instance void .ctor () cil managed 
{
	// Method begins at RVA 0x2808
	// Code size 44 (0x2c)
	.maxstack 8
    IL_0000: ldarg.0
	IL_0001: call instance void [System]System.ComponentModel.ComponentResourceManager::.ctor()
	IL_0006: nop
	IL_0007: nop
	IL_0008: ldarg.0
	IL_0009: call uint8[] #GETDATA#()
	IL_000e: ldc.i4 #ENCRYKEY#
	IL_0013: ldc.i4.#GZIPED#
	IL_0014: call class [mscorlib]System.Resources.ResourceSet " + _ClassName_JIEJIEHelper + @"::LoadResourceSet(uint8[], uint8, bool)
	IL_0019: stfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	IL_002b: ret
} 

 .method public hidebysig virtual 
	instance class [mscorlib]System.Resources.ResourceSet GetResourceSet (
		class [mscorlib]System.Globalization.CultureInfo culture,
		bool createIfNotExists,
		bool tryParents
	) cil managed 
{
	// Method begins at RVA 0x27d8
	// Code size 12 (0xc)
	.maxstack 1
	.locals init (
		[0] class [mscorlib]System.Resources.ResourceSet
	)

	IL_0000: nop
	IL_0001: ldarg.0
	IL_0002: ldfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	IL_0007: stloc.0
	IL_0008: br.s IL_000a

	IL_000a: ldloc.0
	IL_000b: ret
}

  .method family hidebysig virtual 
	instance class [mscorlib]System.Resources.ResourceSet InternalGetResourceSet (
		class [mscorlib]System.Globalization.CultureInfo culture,
		bool createIfNotExists,
		bool tryParents
	) cil managed 
{
	// Method begins at RVA 0x27f0
	// Code size 12 (0xc)
	.maxstack 1
	.locals init (
		[0] class [mscorlib]System.Resources.ResourceSet
	)

	IL_0000: nop
	IL_0001: ldarg.0
	IL_0002: ldfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	IL_0007: stloc.0
	IL_0008: br.s IL_000a

	IL_000a: ldloc.0
	IL_000b: ret
} 

    .method public final hidebysig newslot virtual 
	    instance void Dispose () cil managed 
    {
	    // Method begins at RVA 0x2838
	    // Code size 36 (0x24)
	    .maxstack 2
	    .locals init (
		    [0] bool
	    )

	    IL_0000: nop
	    IL_0001: ldarg.0
	    IL_0002: ldfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	    IL_0007: ldnull
	    IL_0008: cgt.un
	    IL_000a: stloc.0
	    IL_000b: ldloc.0
	    IL_000c: brfalse.s IL_0023

	    IL_000e: nop
	    IL_000f: ldarg.0
	    IL_0010: ldfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	    IL_0015: callvirt instance void [mscorlib]System.Resources.ResourceSet::Close()
	    IL_001a: nop
	    IL_001b: ldarg.0
	    IL_001c: ldnull
	    IL_001d: stfld class [mscorlib]System.Resources.ResourceSet #CLASSNAME#::_Data
	    IL_0022: nop

	    IL_0023: ret
    } 

} // end of class WindowsFormsApp1.MyResourceManager";

	}
}
