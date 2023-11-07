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
using System.Text;

namespace JIEJIE
{
    internal class DCILOperCodeList : List<DCILOperCode>,IDisposable
    {
        static DCILOperCodeList()
        {
           
        }
        public string GetDebugTextForStackOffset()
        {
            var str = new DCILWriter(new StringBuilder());
            int stackCount = 0;
            foreach( var item in this )
            {
                int so = item.StackOffset;
                str.Write(stackCount.ToString("00") + "  " + so + "\t | ");
                str._IsNewLine = true;
                item.WriteTo(str);
                str.WriteLine();
                stackCount += so;
            }
            return str.ToString();
        }
        //public int GroupIndex = 0;

        public void Dispose()
        {
            foreach( var item in this )
            {
                item.Dispose();
            }
            this.Clear();
        }
        public bool ItemBitSizeChanged = false;
        public DCILOperCodeDefine SafeGetNativeDefine( int index )
        {
            if (index >= 0 && index < this.Count )
            {
                return this[index].NativeDefine;
            }
            else
            {
                return null;
            }
        }
        public DCILOperCode SafeGet( int index )
        {
            if (index >= 0 && index < this.Count)
            {
                return this[index];
            }
            else
            {
                return null;
            }
        }
        public DCILOperCode GetNextCode( DCILOperCode item )
        {
            if(item != null )
            {
                int index = this.IndexOf(item);
                if(index >= 0 && index < this.Count -1 )
                {
                    return this[index + 1];
                }
            }
            return null;
        }
        public string FirstLabelID
        {
            get
            {
                if( this.Count > 0 )
                {
                    return this[0].LabelID;
                }
                else
                {
                    return null;
                }
            }
        }
        public DCILOperCode LastCode
        {
            get
            {
                if(this.Count > 0 )
                {
                    return this[this.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public DCILOperCodeList NextGroup = null;

        private bool ChangeShortInstruction_Flag = false;
        public void OnModified()
        {
            //if (this.ChangeShortInstruction_Flag)
            //{
            //    this.ChangeShortInstruction_Flag = false;
            //}
        }
        private static Dictionary<string, string> _ShortJumpOperCodeMaps = null;

        /// <summary>
        /// 将短指令转换为长指令
        /// </summary>
        /// <param name="codes"></param>
        public void ChangeShortInstruction()
        {
            if( this.ChangeShortInstruction_Flag == true || this.Count == 0 )
            {
                return;
            }
            if( _ShortJumpOperCodeMaps == null )
            {
                _ShortJumpOperCodeMaps = new Dictionary<string, string>();
                _ShortJumpOperCodeMaps["beq.s"] = "beq";
                _ShortJumpOperCodeMaps["bge.s"] = "bge";
                _ShortJumpOperCodeMaps["bge.un.s"] = "bge.un";
                _ShortJumpOperCodeMaps["bgt.s"] = "bgt";
                _ShortJumpOperCodeMaps["bgt.un.s"] = "bgt.un";
                _ShortJumpOperCodeMaps["ble.s"] = "ble";
                _ShortJumpOperCodeMaps["ble.un.s"] = "ble.un";
                _ShortJumpOperCodeMaps["blt.s"] = "blt";
                _ShortJumpOperCodeMaps["blt.un.s"] = "blt.un";
                _ShortJumpOperCodeMaps["bne.un.s"] = "bne.un";
                _ShortJumpOperCodeMaps["br.s"] = "br";
                _ShortJumpOperCodeMaps["brfalse.s"] = "brfalse";
                _ShortJumpOperCodeMaps["brtrue.s"] = "brtrue";
                _ShortJumpOperCodeMaps["leave.s"] = "leave";
            }
            this.ChangeShortInstruction_Flag = true;
            foreach (var item in this)
            {
                var code = item.OperCode;
                if (code != null
                    && code.Length > 3
                    && code[code.Length - 2] == '.'
                    && code[code.Length - 1] == 's')
                {
                    string newCode = null;
                    if( _ShortJumpOperCodeMaps.TryGetValue( code , out newCode))
                    {
                        item.SetOperCode( newCode);
                    }
                    //if(code == "brtrue.s")
                    //{

                    //}
                    //if(DCILOperCode.GetOperCodeType( code  ) == ILOperCodeType.JumpShort)
                    //{
                    //    item.OperCode = code.Substring(0, code.Length - 2);
                    //}
                }
                else if (item is DCILOperCode_Try_Catch_Finally)
                {
                    var tg = (DCILOperCode_Try_Catch_Finally)item;
                    tg._Try?.OperCodes?.ChangeShortInstruction();
                    if (tg._Catchs != null)
                    {
                        foreach (var citem in tg._Catchs)
                        {
                            citem.OperCodes?.ChangeShortInstruction();
                        }
                    }
                    tg._Finally?.OperCodes?.ChangeShortInstruction();
                    tg._fault?.OperCodes?.ChangeShortInstruction();
                }
            }
        }

        //private void ChangeShortInstruction_Leave()
        //{
        //    if(this.Count > 0 && this.ChangeShortInstruction_Flag == false )
        //    {
        //        foreach( var item in this )
        //        {
        //            if(item.OperCode == "leave.s")
        //            {
        //                item.OperCode = "leave";
        //            }
        //        }
        //    }
        //}

        public DCILOperCode AddItem(string labelID, string operCode, string operData = null)
        {
            var item = new DCILOperCode(labelID, operCode, operData);
            this.Add(item);
            return item;
        }

        public DCILOperCode AddItem(string labelID, DCILOperCodeDefine myDefine , string operData = null)
        {
            //if(myDefine == DCILOperCodeDefine._br &&( operData == null || operData.Length == 0 ))
            //{

            //}
            var item = new DCILOperCode(labelID, myDefine, operData);
            this.Add(item);
            return item;
        }

        public int StartLineIndex = 0;

        public void WriteTo(DCILWriter writer)
        {
            foreach (var item in this)
            {
                item.WriteTo(writer);
            }
        }

        public void EnumDeeply(DCILMethod method, EnumOperCodeHandler handler)
        {
            var args = new EnumOperCodeArgs();
            args.OwnerList = this;
            args.Method = method;
            for (int iCount = 0; iCount < this.Count; iCount++)
            {
                args.Current = this[iCount];
                args.CurrentCodeIndex = iCount;
                handler(args);
                if (args.Current is DCILOperCode_Try_Catch_Finally)
                {
                    var group = (DCILOperCode_Try_Catch_Finally)args.Current;
                    group._Try?.OperCodes?.EnumDeeply(method, handler);
                    if (group.HasCatchs())
                    {
                        foreach (var item2 in group._Catchs)
                        {
                            item2.OperCodes?.EnumDeeply(method, handler);
                        }
                    }
                    group._Finally?.OperCodes?.EnumDeeply(method, handler);
                    group._fault?.OperCodes?.EnumDeeply(method, handler);
                }
            }
        }
    }
}
