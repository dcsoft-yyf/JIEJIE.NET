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
using System.Reflection;

namespace JIEJIE
{
    /// <summary>
    /// IL指令信息对象
    /// </summary>
    internal class DCILOperCodeDefine
    {
        static DCILOperCodeDefine()
        {
            var extCodeTypes = new Dictionary<string, ILOperCodeType>();
            extCodeTypes["nop"] = ILOperCodeType.Nop;
            extCodeTypes["ldstr"] = ILOperCodeType.ldstr;
            extCodeTypes["switch"] = ILOperCodeType.switch_;
            extCodeTypes["box"] = ILOperCodeType.Class;
            extCodeTypes["call"] = ILOperCodeType.Method;
            extCodeTypes["callvirt"] = ILOperCodeType.Method;
            extCodeTypes["castclass"] = ILOperCodeType.Class;
            extCodeTypes["constrained."] = ILOperCodeType.Class;
            extCodeTypes["cpobj"] = ILOperCodeType.Class;
            extCodeTypes["initobj"] = ILOperCodeType.Class;
            extCodeTypes["isinst"] = ILOperCodeType.Class;
            extCodeTypes["ldfld"] = ILOperCodeType.Field;
            extCodeTypes["ldflda"] = ILOperCodeType.Field;
            extCodeTypes["ldftn"] = ILOperCodeType.Method;
            extCodeTypes["ldelem"] = ILOperCodeType.Class;
            extCodeTypes["ldelema"] = ILOperCodeType.Class;
            extCodeTypes["ldobj"] = ILOperCodeType.Class;
            extCodeTypes["ldsfld"] = ILOperCodeType.Field;
            extCodeTypes["ldsflda"] = ILOperCodeType.Field;
            extCodeTypes["ldtoken"] = ILOperCodeType.ldtoken;
            extCodeTypes["ldvirtftn"] = ILOperCodeType.Method;
            extCodeTypes["mkrefany"] = ILOperCodeType.Class;
            extCodeTypes["newarr"] = ILOperCodeType.Class;
            extCodeTypes["newobj"] = ILOperCodeType.Method;
            extCodeTypes["refanyval"] = ILOperCodeType.Class;
            extCodeTypes["sizeof"] = ILOperCodeType.Class;
            extCodeTypes["stelem"] = ILOperCodeType.Class;
            extCodeTypes["stfld"] = ILOperCodeType.Field;
            extCodeTypes["stobj"] = ILOperCodeType.Class;
            extCodeTypes["stsfld"] = ILOperCodeType.Field;
            extCodeTypes["unbox"] = ILOperCodeType.Class;
            extCodeTypes["unbox.any"] = ILOperCodeType.Class;

            extCodeTypes["beq.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bge.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bge.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bgt.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bgt.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["ble.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["ble.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["blt.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["blt.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["bne.un.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["br.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["brfalse.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["brtrue.s"] = ILOperCodeType.JumpShort;
            extCodeTypes["leave.s"] = ILOperCodeType.JumpShort;

            extCodeTypes["beq"] = ILOperCodeType.Jump;
            extCodeTypes["bge"] = ILOperCodeType.Jump;
            extCodeTypes["bge.un"] = ILOperCodeType.Jump;
            extCodeTypes["bgt"] = ILOperCodeType.Jump;
            extCodeTypes["bgt.un"] = ILOperCodeType.Jump;
            extCodeTypes["ble"] = ILOperCodeType.Jump;
            extCodeTypes["ble.un"] = ILOperCodeType.Jump;
            extCodeTypes["blt"] = ILOperCodeType.Jump;
            extCodeTypes["blt.un"] = ILOperCodeType.Jump;
            extCodeTypes["bne.un"] = ILOperCodeType.Jump;
            extCodeTypes["br"] = ILOperCodeType.Jump;
            extCodeTypes["brfalse"] = ILOperCodeType.Jump;
            extCodeTypes["brtrue"] = ILOperCodeType.Jump;
            extCodeTypes["leave"] = ILOperCodeType.Jump;

            extCodeTypes["ldarg"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldarg.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldarga"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldarga.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldloc"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldloc.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldloca"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["ldloca.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["starg"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["starg.s"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["stloc"] = ILOperCodeType.ArgsOrLocalsByName;
            extCodeTypes["stloc.s"] = ILOperCodeType.ArgsOrLocalsByName;

            extCodeTypes["ldc.i4"] = ILOperCodeType.LoadNumberByOperData;
            extCodeTypes["ldc.i4.s"] = ILOperCodeType.LoadNumberByOperData;
            extCodeTypes["ldc.i8"] = ILOperCodeType.LoadNumberByOperData;
            extCodeTypes["ldc.r4"] = ILOperCodeType.LoadNumberByOperData;
            extCodeTypes["ldc.r8"] = ILOperCodeType.LoadNumberByOperData;
             
            var fields = typeof(System.Reflection.Emit.OpCodes).GetFields(
                BindingFlags.Public | BindingFlags.Static);
            foreach( var f in fields )
            {
                var code = (System.Reflection.Emit.OpCode)f.GetValue(null);
                _Defines[code.Name.ToLower()] = new DCILOperCodeDefine(code, extCodeTypes);
            }
            var values = _Defines.Values;

            _br = _Defines["br"];
            _br_s = _Defines["br.s"];
            _pop = _Defines["pop"];
            _call = _Defines["call"];
            _callvirt = _Defines["callvirt"];
            _nop = _Defines["nop"];
            _dup = _Defines["dup"];
            _brtrue = _Defines["brtrue"];
            _brtrue_s = _Defines["brtrue.s"];
            _newobj = _Defines["newobj"];
            _ldftn = _Defines["ldftn"];
            _ldvirtftn = _Defines["ldvirtftn"];
            _ldtoken = _Defines["ldtoken"];
            _switch = _Defines["switch"];
            _ldstr = _Defines["ldstr"];
            _ldc_i4 = _Defines["ldc.i4"];
            _ldsfld = _Defines["ldsfld"];
        }

        public static readonly DCILOperCodeDefine _br;
        public static readonly DCILOperCodeDefine _br_s;
        public static readonly DCILOperCodeDefine _nop;
        public static readonly DCILOperCodeDefine _pop;
        public static readonly DCILOperCodeDefine _call;
        public static readonly DCILOperCodeDefine _callvirt;
        public static readonly DCILOperCodeDefine _dup;
        public static readonly DCILOperCodeDefine _brtrue;
        public static readonly DCILOperCodeDefine _brtrue_s;
        public static readonly DCILOperCodeDefine _newobj;
        public static readonly DCILOperCodeDefine _ldftn;
        public static readonly DCILOperCodeDefine _ldvirtftn;
        public static readonly DCILOperCodeDefine _ldtoken;
        public static readonly DCILOperCodeDefine _switch;
        public static readonly DCILOperCodeDefine _ldstr;
        public static readonly DCILOperCodeDefine _ldc_i4;
        public static readonly DCILOperCodeDefine _ldsfld;

        private static readonly SortedDictionary<string, DCILOperCodeDefine> _Defines
               = new SortedDictionary<string, DCILOperCodeDefine>();

        public static DCILOperCodeDefine GetDefine( string codeName)
        {
            if( codeName == null || codeName.Length == 0 )
            {
                throw new ArgumentNullException("codeName");
            }
            DCILOperCodeDefine info = null;
            if(_Defines.TryGetValue(codeName.ToLower() , out info ))
            {
                info.RefCount++;
                return info;
            }
            else
            {
                throw new NotSupportedException(codeName);
            }
        }

        public DCILOperCodeDefine(string name  )
        {
            this.Name = name;
        }

        private DCILOperCodeDefine(
            System.Reflection.Emit.OpCode code,
            Dictionary<string, ILOperCodeType> extCodeTypes )
        {
            this.FlowControl = code.FlowControl;
            this.Name = code.Name;
            this.OpCodeType = code.OpCodeType;
            this.OperandType = code.OperandType;
            this.Size = code.Size;
            this.StackBehaviourPop = code.StackBehaviourPop;
            this.StackBehaviourPush = code.StackBehaviourPush;
            this.Value = ( DCILOpCodeValue)code.Value;
            if (extCodeTypes.TryGetValue(this.Name, out this.ExtCodeType) == false)
            {
                this.ExtCodeType = ILOperCodeType.Normal;
            }
            this.StackOffset = GetStackAdd(this.StackBehaviourPush) + GetStackAdd(this.StackBehaviourPop);

            //if( stdStackOffset.TryGetValue( this.Name , out this.StackOffset ) == false )
            //{
            //    this.StackOffset = 0;
            //}
            this.IsPrefix = code.OpCodeType == System.Reflection.Emit.OpCodeType.Prefix;
            this.WithoutOperData = this.OperandType == System.Reflection.Emit.OperandType.InlineNone;
        }
        private static int GetStackAdd( System.Reflection.Emit.StackBehaviour sb )
        {
            switch( sb )
            {
                case System.Reflection.Emit.StackBehaviour.Pop0: return 0;
                case System.Reflection.Emit.StackBehaviour.Pop1:return -1;
                case System.Reflection.Emit.StackBehaviour.Pop1_pop1: return -2;
                case System.Reflection.Emit.StackBehaviour.Popi:return -1;
                case System.Reflection.Emit.StackBehaviour.Popi_pop1:return -2;
                case System.Reflection.Emit.StackBehaviour.Popi_popi:return -2;
                case System.Reflection.Emit.StackBehaviour.Popi_popi8:return -2;
                case System.Reflection.Emit.StackBehaviour.Popi_popi_popi: return -3;
                case System.Reflection.Emit.StackBehaviour.Popi_popr4: return -2;
                case System.Reflection.Emit.StackBehaviour.Popi_popr8:return -2;
                case System.Reflection.Emit.StackBehaviour.Popref: return -1;
                case System.Reflection.Emit.StackBehaviour.Popref_pop1:return -2;
                case System.Reflection.Emit.StackBehaviour.Popref_popi:return -2;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_pop1:return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popi:return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popi8: return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popr4:return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popr8:return -3;
                case System.Reflection.Emit.StackBehaviour.Popref_popi_popref:return -3;
                case System.Reflection.Emit.StackBehaviour.Push0:return 0;
                case System.Reflection.Emit.StackBehaviour.Push1: return 1;
                case System.Reflection.Emit.StackBehaviour.Push1_push1:return 2;
                case System.Reflection.Emit.StackBehaviour.Pushi:return 1;
                case System.Reflection.Emit.StackBehaviour.Pushi8:return 1;
                case System.Reflection.Emit.StackBehaviour.Pushr4: return 1;
                case System.Reflection.Emit.StackBehaviour.Pushr8:return 1;
                case System.Reflection.Emit.StackBehaviour.Pushref: return 1;
            }
            return 0;
        }
        public int RefCount = 0;
        public readonly System.Reflection.Emit.FlowControl FlowControl;
        public readonly string Name;
        public readonly System.Reflection.Emit.OpCodeType OpCodeType;
        public readonly System.Reflection.Emit.OperandType OperandType;
        public readonly int Size;
        public readonly System.Reflection.Emit.StackBehaviour StackBehaviourPop;
        public readonly System.Reflection.Emit.StackBehaviour StackBehaviourPush;
        public readonly DCILOpCodeValue Value;
        public readonly bool IsPrefix;
        public readonly ILOperCodeType ExtCodeType;
        public readonly int StackOffset = 0;

        public readonly bool WithoutOperData;
        
        public override string ToString()
        {
            var str = this.Name + " ExtCodeType=" + this.ExtCodeType + " WOD=" + this.WithoutOperData + " ST=" + this.StackOffset;
            if( this.IsPrefix )
            {
                str = str + " Prefix";
            }
            return str;
        }
    }
}
