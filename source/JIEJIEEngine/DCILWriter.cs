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
using System.IO;

namespace JIEJIE
{
    internal class DCILWriter
    {
        private static readonly string[] _WhitespaceString = null;
        static DCILWriter()
        {
            _WhitespaceString = new string[50];
            _WhitespaceString[0] = string.Empty;
            for (int iCount = 0; iCount < _WhitespaceString.Length; iCount++)
            {
                _WhitespaceString[iCount] = new string(' ', iCount);
            }
        }
        public DCILWriter(TextWriter w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }
            this._BaseWriter = w;
        }
        public DCILWriter(StringBuilder w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }
            this._BaseWriter = new StringWriter(w);
            this._StringBuilder = w;
        }
        public override string ToString()
        {
            if (this._StringBuilder != null)
            {
                return this._StringBuilder.ToString();
            }
            else
            {
                return base.ToString();
            }
        }

        private readonly TextWriter _BaseWriter = null;
        private readonly StringBuilder _StringBuilder = null;

        private static readonly string _hexs = "0123456789ABCDEF";
        private char[] _HexsBuffer = null;

        public void WriteHexs(byte[] bs, int lineHeadWhitespaceNum = 16)
        {
            if (bs != null && bs.Length > 0)
            {
                var len = bs.Length;
                int bufferSize = (int)(bs.Length * 6);
                if (_HexsBuffer == null || bufferSize > _HexsBuffer.Length)
                {
                    _HexsBuffer = new char[bufferSize];
                }
                var position = 0;
                for (int iCount = 0; iCount < len; iCount++)
                {
                    var b = bs[iCount];
                    _HexsBuffer[position++] = _hexs[(b >> 4)];
                    _HexsBuffer[position++] = _hexs[b & 0xf];
                    _HexsBuffer[position++] = ' ';
                    if (iCount > 0 && iCount < len - 1 && (iCount % 16) == 0)
                    {
                        _HexsBuffer[position++] = '\r';
                        _HexsBuffer[position++] = '\n';
                        if (lineHeadWhitespaceNum > 0)
                        {
                            int endIndex = position + lineHeadWhitespaceNum;
                            for (; position < endIndex; position++)
                            {
                                _HexsBuffer[position] = ' ';
                            }
                        }
                    }
                }
                if (this._StringBuilder != null)
                {
                    this._StringBuilder.Append(_HexsBuffer, 0, position);
                }
                else
                {
                    this._BaseWriter.Write(_HexsBuffer, 0, position);
                }
            }
        }

        public void WriteObjects2(System.Collections.IEnumerable objs)
        {
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    if (obj is DCILObject)
                    {
                        ((DCILObject)obj).WriteTo(this);
                    }
                }
            }
        }

        public void WriteObjects(List<DCILObject> objs)
        {
            if (objs != null && objs.Count > 0)
            {
                int len = objs.Count;
                for(int iCount = 0;iCount < len; iCount ++)
                {
                    objs[iCount].WriteTo(this);
                }
            }
        }
        public void Write(string txt)
        {
            EnsureIndent();
            this._IsNewLine = false;
            if (this._StringBuilder != null)
            {
                this._StringBuilder.Append(txt);
            }
            else
            {
                _BaseWriter.Write(txt);
            }
        }
        public void Write(char c)
        {
            EnsureIndent();
            this._IsNewLine = false;
            if (_StringBuilder != null)
            {
                this._StringBuilder.Append(c);
            }
            else
            {
                _BaseWriter.Write(c.ToString());
            }
        }
        public void WriteWhitespace(int num)
        {
            if (this._StringBuilder != null)
            {
                this._StringBuilder.Append(' ', num);
            }
            else
            {
                if (num >= 50)
                {
                    _BaseWriter.Write(new string(' ', num));
                }
                else
                {
                    _BaseWriter.Write(_WhitespaceString[num]);
                }
            }
        }
        private void EnsureIndent()
        {
            if (this._IsNewLine && this._IndentLevel > 0)
            {
                if (this._StringBuilder != null)
                {
                    _StringBuilder.Append(_WhitespaceString[_IndentLevel * 3]);
                }
                else
                {
                    _BaseWriter.Write(_WhitespaceString[_IndentLevel * 3]);
                }
                this._IsNewLine = false;
            }
        }
        public void EnsureNewLine()
        {
            if (this._IsNewLine == false)
            {
                this.WriteLine();
            }
        }

        public void WriteLine(string txt)
        {

            this.EnsureIndent();
            if (this._StringBuilder != null)
            {
                _StringBuilder.AppendLine(txt);
                this._IsNewLine = true;
            }
            else
            {
                _BaseWriter.WriteLine(txt);
                this._IsNewLine = true;
            }
        }
        internal bool _IsNewLine = true;

        public void WriteLine()
        {
            if (this._StringBuilder != null)
            {
                this._StringBuilder.AppendLine();
                this._IsNewLine = true;
            }
            else
            {
                _BaseWriter.WriteLine();
                this._IsNewLine = true;
            }
        }

        public void WriteStartGroup()
        {
            this.EnsureNewLine();
            this.WriteLine("{");
            this._IndentLevel++;
        }
        public void WriteEndGroup()
        {
            this.EnsureNewLine();
            this._IndentLevel--;
            this.WriteLine("}");
        }
        private int _IndentLevel = 0;
        public void ChangeIndentLevel(int step)
        {
            this._IndentLevel += step;
        }
    }
}
