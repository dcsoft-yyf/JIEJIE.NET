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
    internal class DCILOperCode_Try_Catch_Finally : DCILOperCode
    {
        private static readonly DCILOperCodeDefine _SrcDefine = new DCILOperCodeDefine("try_cath_finally");

        public DCILOperCode_Try_Catch_Finally()
        {
            base._Define = _SrcDefine;
        }

        public string SingleLineContent = null;

        public DCILObject _Try = null;
        public List<DCILCatchBlock> _Catchs = null;
        public DCILObject _Finally = null;
        public DCILObject _fault = null;
        public DCILObject _Filter = null;

        public override DCILOperCode Clone(string newLabelID)
        {
            return base.Clone(newLabelID);
        }
        public bool HasTryOperCodes()
        {
            return this._Try != null && this._Try.OperCodes != null && this._Try.OperCodes.Count > 0;
        }
        public bool HasCatchs()
        {
            return this._Catchs != null && this._Catchs.Count > 0;
        }
        public bool HasFinallyOperCodes()
        {
            return this._Finally != null && this._Finally.OperCodes != null && this._Finally.OperCodes.Count > 0;
        }
        public bool HasFaultOperCodes()
        {
            return this._fault != null && this._fault.OperCodes != null && this._fault.OperCodes.Count > 0;
        }
        public override void Dispose()
        {
            base.Dispose();
            if( this._Try != null )
            {
                this._Try.Dispose();
                this._Try = null;
            }
            if(this._Catchs != null )
            {
                foreach( var item in this._Catchs)
                {
                    item.Dispose();
                }
                this._Catchs.Clear();
                this._Catchs = null;
            }
            if(this._Finally != null )
            {
                this._Finally.Dispose();
                this._Finally = null;
            }
            if( this._fault != null )
            {
                this._fault.Dispose();
                this._fault = null;
            }
            if(this._Filter != null )
            {
                this._Filter.Dispose();
                this._Filter = null;
            }
        }
        public int TotalOperCodesCount
        {
            get
            {
                int result = 0;
                if (this._Try != null)
                {
                    result = this._Try.TotalOperCodesCount;
                }
                if (this._Catchs != null)
                {
                    foreach (var item in this._Catchs)
                    {
                        result += item.TotalOperCodesCount;
                    }
                }
                if (this._Finally != null)
                {
                    result += this._Finally.TotalOperCodesCount;
                }
                if (this._fault != null)
                {
                    result += this._fault.TotalOperCodesCount;
                }
                if( this._Filter != null )
                {
                    result += this._Filter.TotalOperCodesCount;
                }
                return result;
            }
        }
        public override string ToString()
        {
            var str = new StringBuilder();

            WriteText(str, "try", this._Try.OperCodes);
            if( this._Filter != null )
            {
                WriteText(str, "filter" , this._Filter.OperCodes);
            }
            if (this._Catchs != null && this._Catchs.Count > 0)
            {
                foreach (var item in this._Catchs)
                {
                    WriteText(str, "catch " + item.ExcpetionType?.ToString(), item.OperCodes);
                }
            }
            if (this._Finally != null)
            {
                WriteText(str, "finally", this._Finally.OperCodes);
            }
            if (this._fault != null)
            {
                WriteText(str, "fault", this._fault.OperCodes);
            }
            return str.ToString();
        }

        private void WriteText(StringBuilder str, string name, DCILOperCodeList list)
        {
            str.Append(name);
            str.Append("{");
            if (list != null && list.Count > 0)
            {
                str.Append(list.Count.ToString());
            }
            str.Append("}");
        }

        public override void WriteTo(DCILWriter writer)
        {
            writer.EnsureNewLine();
            if( this.SingleLineContent != null &&  this.SingleLineContent.Length > 0 )
            {
                writer.WriteLine(".try " + this.SingleLineContent);
                return;
            }
            writer.WriteLine(".try");
            writer.WriteStartGroup();
            foreach (var item in this._Try.OperCodes)
            {
                item.WriteTo(writer);
            }
            writer.WriteEndGroup();
            if( this._Filter != null )
            {
                writer.WriteLine("filter");
                writer.WriteStartGroup();
                foreach( var item in this._Filter.OperCodes )
                {
                    item.WriteTo(writer);
                }
                writer.WriteEndGroup();
            }
            if (this._Catchs != null && this._Catchs.Count > 0)
            {
                foreach (var item in this._Catchs)
                {
                    if (this._Filter == null)
                    {
                        writer.Write("catch ");
                        if (item.ExcpetionType != null)
                        {
                            item.ExcpetionType.WriteTo(writer, false);
                        }
                    }
                    writer.WriteLine();
                    writer.WriteStartGroup();
                    foreach (var item2 in item.OperCodes)
                    {
                        item2.WriteTo(writer);
                    }
                    writer.WriteEndGroup();
                }
            }
            if (this._fault != null && this._fault.OperCodes != null && this._fault.OperCodes.Count > 0)
            {
                writer.WriteLine("fault");
                writer.WriteStartGroup();
                foreach (var itemi in this._fault.OperCodes)
                {
                    itemi.WriteTo(writer);
                }
                writer.WriteEndGroup();
            }
            if (this._Finally != null)
            {
                writer.WriteLine("finally");
                writer.WriteStartGroup();
                foreach (var itemi in this._Finally.OperCodes)
                {
                    itemi.WriteTo(writer);
                }
                writer.WriteEndGroup();
            }
        }
    }


}
