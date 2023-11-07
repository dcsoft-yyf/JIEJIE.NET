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
    internal class DCILReader
    {
        public DCILReader(string fileName, System.Text.Encoding encoding, DCILDocument doc)
        {
            this.FileName = fileName;
            using (var reader = new System.IO.StreamReader(fileName, encoding, true))
            {
                this.SetSourceText(reader.ReadToEnd());
                this.Document = doc;
            }
        }
        private DCILReader()
        {

        }
        public DCILReader(string txt, DCILDocument doc)
        {
            if (txt == null || txt.Length == 0)
            {
                throw new ArgumentNullException("txt");
            }
            this.SetSourceText(txt);
            this.Document = doc;
        }

        public void AddPreserverTypeName( string name )
        {
            if( name != null && name.Length > 0 )
            {
                if(this.PreserveTypeNames == null )
                {
                    this.PreserveTypeNames = new HashSet<string>();
                }
                if(name[0]=='[')
                {
                    int index = name.IndexOf(']');
                    if(index > 0 )
                    {
                        name = name.Substring(index + 1).Trim();
                    }
                    else
                    {
                        return;
                    }
                }
                this.PreserveTypeNames.Add(name);
            }
        }

        public HashSet<string> PreserveTypeNames = null;

        private Dictionary<DCILField, string> _FieldReferenceDataLabels = null;
        /// <summary>
        /// 添加字段对象引用的数据块信息
        /// </summary>
        /// <param name="field">字段对象</param>
        /// <param name="dataLabel">数据块编号</param>
        public void AddReferenceDataLabel(DCILField field , string dataLabel )
        {
            if(this._FieldReferenceDataLabels == null )
            {
                this._FieldReferenceDataLabels = new Dictionary<DCILField, string>();
            }
            this._FieldReferenceDataLabels[field] = dataLabel;
        }

        /// <summary>
        /// 更新字段对象引用的数据块对象
        /// </summary>
        /// <param name="document"></param>
        public void UpdateFieldReferenceData(DCILDocument document )
        {
            if( this._FieldReferenceDataLabels != null 
                && document != null 
                && document.ILDatas != null 
                && document.ILDatas.Count > 0 )
            {
                var dataMaps = new Dictionary<string, DCILData>();
                foreach( var item in document.ILDatas )
                {
                    dataMaps[item.Name] = item;
                }
                foreach( var item in this._FieldReferenceDataLabels )
                {
                    dataMaps.TryGetValue(item.Value, out item.Key.ReferenceData);
                }
            }
        }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName = null;
        public List<DCILField> FieldsReferenceData = null;
        public void Close()
        {
            this._Text = null;
            if (this._StringTable != null)
            {
                this._StringTable.Clear();
                this._StringTable = null;
            }
            this._Position = 0;
            if( this._FieldReferenceDataLabels != null )
            {
                this._FieldReferenceDataLabels.Clear();
                this._FieldReferenceDataLabels = null;
            }
        }
        public int NumOfOperCode = 0;
        public int NumOfMethod = 0;
        public int NumOfField = 0;
        public int NumOfClass = 0;
        public int NumOfProperty = 0;

        public readonly DCILDocument Document = null;

        private const string _Str_SplitChars = "{}(),=<>&*[]:";

        private static readonly string[] _SplitChars = GetSplitWords();
     
        private static string[] GetSplitWords()
        {
            var result = new string[127];
            foreach (var c in _Str_SplitChars)
            {
                result[c] = new string(c, 1);
            }
            return result;
        }
        private static readonly DCILReader _ReaderForSplit = new DCILReader();

        public char GetChar(int index)
        {
            if (index >= 0 && index < this._Length)
            {
                return this._Text[index];
            }
            else
            {
                return char.MinValue;
            }
        }

        public char PeekContentChar()
        {
            for (int iCount = this._Position; iCount < this._Length; iCount++)
            {
                if (IsWhiteSpace(this._Text[iCount]) == false)
                {
                    return this._Text[iCount];
                }
            }
            return char.MinValue;
        }

        public char ReadContentChar()
        {
            var list = new List<char>();
            for (; this._Position < this._Length; this._Position++)
            {
                if (IsWhiteSpace(this._Text[this._Position]) == false)
                {
                    return this._Text[this._Position++];
                }
            }
            return char.MinValue;
        }

        private int _LastLineIndex = 0;
        private int _LastLineIndex_Position = 0;
        public unsafe int CurrentLineIndex()
        {
            if (this._Position < this._LastLineIndex_Position)
            {
                this._LastLineIndex = 0;
                this._LastLineIndex_Position = 0;
            }
            int result = this._LastLineIndex ;
            fixed ( char * pFirst = this._Text)
            {
                char* pStart = pFirst + this._LastLineIndex_Position ;
                char* pEnd = pFirst + this._Position;
                while( pStart < pEnd )
                {
                    if( *pStart == '\r')
                    {
                        result++;
                    }
                    pStart++;
                }
                this._LastLineIndex = result;
                this._LastLineIndex_Position = (int)(pStart - pFirst);
            }
            return result;
        }
        public string PeekString(int length)
        {
            if (length > 0 && this._Position + length < this._Text.Length)
            {
                return this._Text.Substring(this._Position, length);
            }
            else
            {
                return string.Empty;
            }
        }
        public string PeekWord()
        {
            int pos = this._Position;
            var result = this.ReadWord();
            this._Position = pos;
            return result;
        }

        private const int CharType_None = 0;
        private const int CharType_Spliter = 1;
        private const int CharType_Whitespace = 2;

        private static readonly byte[] _CharTypes = InitCharTypes();
        private static byte[] InitCharTypes()
        {
            var types = new byte[127];
            for (int iCount = 0; iCount < types.Length; iCount++)
            {
                if (_Str_SplitChars.IndexOf((char)iCount) >= 0)
                {
                    types[iCount] = CharType_Spliter;
                }
                else if (iCount == ' ' || iCount == '\r' || iCount == '\n' || iCount == '\t')
                {
                    types[iCount] = CharType_Whitespace;
                }
                else
                {
                    types[iCount] = CharType_None;
                }
            }
            return types;
        }
       
        private static readonly bool[] _IsOperCodeChars = BuildOperCodeChars();
        private static bool[] BuildOperCodeChars()
        {
            var result = new bool[127];
            for(int c = 0; c < result.Length; c ++ )
            {
                result[c] = c >= 'a' && c <= 'z' || c >= '0' && c <= '9' || c == '.';
            }
            return result;
        }

        private System.Collections.Generic.Dictionary<long, DCILOperCodeDefine> _OperCodeTable = null;

        public DCILOperCodeDefine ReadOperCode()
        {
            var thisText = this._Text;
            var thisLength = this._Length;
            for (; this._Position < thisLength; this._Position++)
            {
                var c = thisText[this._Position];
                if (c < 127 && _IsOperCodeChars[c])
                {
                    for (int startPos = this._Position; this._Position < thisLength; this._Position++)
                    {
                        c = thisText[this._Position];
                        if (c >= 127 || _IsOperCodeChars[c] == false)
                        {
                            long key = ((long)(this._Position - startPos) << 32) + startPos;
                            DCILOperCodeDefine result = null;
                            if (this._OperCodeTable.TryGetValue(key, out result) == false)
                            {
                                string strCode = thisText.Substring(startPos, this._Position - startPos);
                                result = DCILOperCodeDefine.GetDefine(strCode);
                                this._OperCodeTable[key] = result;
                            }
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        public string ReadWord()
        {
            var currentItem_EndPosition = -1;
            var currentItem_Length = 0;
            int textLength = this._Length;
            var bodyText = this._Text;
            Retry:;
            for (; this._Position < textLength; this._Position++)
            {
                char c = bodyText[this._Position];
                if ( c !=' ' && c !='\t' && c !='\r' && c !='\n')// IsWhiteSpace(this._Text[this._Position]) == false)
                {
                    bool isInGroup = false;
                    for (; this._Position < textLength; this._Position++)
                    {
                        c = bodyText[this._Position];
                        if (c == '\'')
                        {
                            // 遇到单引号
                            isInGroup = !isInGroup;
                        }
                        if (isInGroup)
                        {
                            // 在单引号组内，无条件的添加
                            currentItem_EndPosition = this._Position;
                            currentItem_Length++;
                            //this._CurrentItem[this._CurrentItem_Length++] = c;
                        }
                        else
                        {
                            int charType = CharType_None;
                            if (c < 127)
                            {
                                charType = _CharTypes[c];
                            }
                            switch( charType )
                            {
                                case CharType_None:
                                    {
                                        // 遇到常规字符
                                        if (c == '/' && this._Position < textLength - 1 && bodyText[this._Position + 1] == '/')
                                        {
                                            // 遇到注释
                                            this.MoveNextLine();
                                            if (currentItem_Length > 0)
                                            {
                                                return GetCurrentItemString(currentItem_Length, currentItem_EndPosition);
                                            }
                                            else
                                            {
                                                goto Retry;
                                            }
                                        }
                                        currentItem_EndPosition = this._Position;
                                        currentItem_Length++;
                                    }
                                    break;
                                case CharType_Spliter:
                                    // 遇到分隔字符
                                    if (currentItem_Length == 0)
                                    {
                                        this._Position++;
                                        return _SplitChars[c];
                                    }
                                    else
                                    {
                                        return GetCurrentItemString(currentItem_Length, currentItem_EndPosition);
                                    }
                                    break;
                                case CharType_Whitespace:
                                    // 遇到空白字符
                                    if (currentItem_Length > 0)
                                    {
                                        return GetCurrentItemString(currentItem_Length, currentItem_EndPosition);
                                    }
                                    break;
                            }
                        }
                    }
                    if (currentItem_Length > 0)
                    {
                        return GetCurrentItemString(currentItem_Length, currentItem_EndPosition);
                    }
                }
            }
            return null;
        }

        public int ReadArrayIndex()
        {
            if (this.SkipWhitespace())
            {
                if (this._Text[this._Position] == '[')
                {
                    int index = this._Text.IndexOf(']', this._Position + 1);
                    if (index > 0)
                    {
                        int result = 0;
                        if (int.TryParse(
                            this._Text.Substring(this._Position + 1, index - this._Position - 1),
                            out result))
                        {
                            this._Position = index;
                            return result;
                        }
                    }
                }
            }
            return int.MinValue;
        }

        /// <summary>
        /// 跳过所有的空格
        /// </summary>
        /// <returns>是否还有内容可供读取</returns>
        public bool SkipWhitespace()
        {
            for (; this._Position < this._Length; this._Position++)
            {
                if (DCUtils.IsWhitespace(this._Text[this._Position]) == false)
                {
                    break;
                }
            }
            return this._Position < this._Length;
        }
        /// <summary>
        /// 跳过当前行中所有的空格
        /// </summary>
        /// <returns>是否还有内容可供读取</returns>
        public bool SkipLineWhitespace()
        {
            for (; this._Position < this._Length; this._Position++)
            {
                var c = this._Text[this._Position];
                if (c != ' ' && c != '\t')
                {
                    break;
                }
            }
            return this._Position < this._Length;
        }
        /// <summary>
        /// 当前行是否还具有非空白的内容
        /// </summary>
        /// <returns></returns>
        public bool HasContentLeftCurrentLine()
        {
            for (int iCount = this._Position; iCount < this._Length; iCount++)
            {
                var c = this._Text[iCount];
                if (c == '\r' || c == '\n')
                {
                    return false;
                }
                if (c != ' ' && c != '\t')
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasContentLeft()
        {
            return this._Position < this._Length;
        }
        public string ReadAfterChar(char c)
        {
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index = this._Text.IndexOf(c, this._Position);
            if (index >= 0)
            {
                var result = this._Text.Substring(this._Position, index - this._Position + 1);
                this._Position = index + 1;
                return result;
            }
            else
            {
                var result = this._Text.Substring(this._Position);
                this._Position = this._Length;
                return result;
            }
        }
        public void MoveToLineEnd()
        {
            for (; this._Position < this._Length; this._Position++)
            {
                var c = this._Text[this._Position];
                if (c == '\r' || c == '\n')
                {
                    break;
                }
            }
        }
        public string ReadInstructionContent()
        {
            var strData = new System.Text.StringBuilder();
            //strData.Append(this.ReadLine());
            int level = 0;
            bool isNewLineFlag = false;
            for (; this._Position < this._Length; this._Position++)
            {
                var c = this._Text[this._Position];
                if (c == '/'
                    && this._Position < this._Length - 1
                    && this._Text[this._Position + 1] == '/')
                {
                    // 遇到注释
                    this.MoveToLineEnd();
                    // 撤销注释之前的空白
                    for (int iCount = strData.Length - 1; iCount >= 0; iCount--)
                    {
                        var c2 = strData[iCount];
                        if (c2 != ' ' && c2 != '\t')
                        {
                            if (iCount < strData.Length - 2)
                            {
                                strData.Remove(iCount + 1, strData.Length - iCount - 1);
                            }
                            break;
                        }
                    }
                    continue;
                }
                else if (c == '\r' || c == '\n')
                {
                    isNewLineFlag = true;
                }
                else if (c == '.' && isNewLineFlag)
                {
                    // 新的指令
                    for (int iCount = strData.Length - 1; iCount >= 0; iCount--)
                    {
                        var c2 = strData[iCount];
                        if (c2 == '\r' || c2 == '\n')
                        {
                            strData.Remove(iCount, strData.Length - iCount);
                            break;
                        }
                    }
                    break;
                }
                else if (c == '{')
                {
                    level++;
                }
                else if (c == '}')
                {
                    level--;
                    if (level <= 0)
                    {
                        // 出组
                        if (level == 0)
                        {
                            strData.Append(c);
                            this._Position++;
                        }
                        else
                        {
                            //this._Position--;
                        }
                        break;
                    }
                }
                if (isNewLineFlag && IsWhiteSpace(c) == false)
                {
                    isNewLineFlag = false;
                }
                strData.Append(c);
            }
            if (strData.Length > 0 && strData[strData.Length - 1] == '\r')
            {
                strData.Remove(strData.Length - 1, 1);
            }
            return strData.ToString();
        }
        public bool MatchText(string text)
        {
            if (text != null && text.Length > 0 && this._Position < this._Length - text.Length)
            {
                if (string.Compare(this._Text, this._Position, text, 0, text.Length, StringComparison.Ordinal) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        
        private System.Collections.Generic.Dictionary<long, string> _StringTable = null;
        public string GetSubStringUseTable(int pos, int len, bool trim = false)
        {
            var thisText = this._Text;
            if (trim)
            {
                int endIndex = pos + len - 1;
                while (pos <= endIndex)
                {
                    char c = thisText[pos];
                    if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                    {
                        break;
                    }
                    pos++;
                }
                while (endIndex >= pos)
                {
                    char c = thisText[endIndex];
                    if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                    {
                        break;
                    }
                    endIndex--;
                }
                len = endIndex - pos + 1;
            }
            if (len == 0)
            {
                return string.Empty;
            }
            if (len == 1)
            {
                return DCUtils.GetSingleCharString(thisText[pos]);
            }
            string result = null;
            long key = ((long)len << 32) + pos;
            if (this._StringTable.TryGetValue(key, out result) == false)
            {
                result = DCUtils.GetStringUseTable(thisText.Substring(pos, len));
                this._StringTable[key] = result;
            }
            //if(result.IndexOf("\r") > 0 )
            //{

            //}
            return result;
        }

        private class KeyStringComparere : System.Collections.Generic.IEqualityComparer<long>
        {
            public KeyStringComparere(string txt)
            {
                this._Text = txt;
            }
            private string _Text = null;

            private int _EqualsCount = 0;
            private int _GetHashCodeCount = 0;

            public bool Equals(long x, long y)
            {
                //this._EqualsCount++;
                int len1 = (int)(x >> 32);
                int pos1 = (int)x & 0xfffffff;
                int len2 = (int)(y >> 32);
                int pos2 = (int)y & 0xfffffff;
                if (len1 != len2)
                {
                    return false;
                }
                return string.CompareOrdinal(this._Text, pos1, this._Text, pos2, len1) == 0;
            }

            public unsafe int GetHashCode(long obj)
            {
                //this._GetHashCodeCount++;
                int len = (int)(obj >> 32);
                int pos = (int)obj & 0xfffffff;
                fixed (char* pStart = this._Text)
                {
                    char* pend = pStart + pos + len;
                    char* p = pend - len;
                    int num = 5381;
                    while (p < pend)
                    {
                        num = ((num << 5) + num) ^ *p;
                        p++;
                    }
                    return num;

                    //int num2 = num;
                    //if ((((int)p) & 2) == 0)
                    //{
                    //    while (p < pend)
                    //    {
                    //        if ((((int)p) & 2) == 0)
                    //        {
                    //            num = ((num << 5) + num) ^ *p;
                    //        }
                    //        else
                    //        {
                    //            num2 = ((num2 << 5) + num2) ^ *p;
                    //        }
                    //        p++;
                    //    }
                    //}
                    //else
                    //{
                    //    while (p < pend)
                    //    {
                    //        if ((((int)p) & 2) != 0)
                    //        {
                    //            num = ((num << 5) + num) ^ *p;
                    //        }
                    //        else
                    //        {
                    //            num2 = ((num2 << 5) + num2) ^ *p;
                    //        }
                    //        p++;
                    //    }
                    //}
                    //int result = num + num2 * 1566083941 + len;
                    //return result;


                    //int num = 5381;
                    //int num2 = num;
                    //char* ptr2 = ptr;
                    //int num3;
                    //while ((num3 = *ptr2) != 0)
                    //{
                    //    num = ((num << 5) + num) ^ num3;
                    //    num3 = ptr2[1];
                    //    if (num3 == 0)
                    //    {
                    //        break;
                    //    }
                    //    num2 = ((num2 << 5) + num2) ^ num3;
                    //    ptr2 += 2;
                    //}
                    //return num + num2 * 1566083941;
                }
            }
        }

        private string GetCurrentItemString(int _CurrentItem_Length , int _CurrentItem_EndPosition )
        {
            if (_CurrentItem_Length == 0)
            {
                return string.Empty;
            }
            else
            {
                int pos = _CurrentItem_EndPosition - _CurrentItem_Length + 1;
                string result = GetSubStringUseTable(pos, _CurrentItem_Length );
                return result;
            }
        }

        

        public string ReadAfterCharsExcludeLastChar(string chrs, out char resultChar)
        {
            if (chrs == null)
            {
                throw new ArgumentNullException("chrs");
            }
            resultChar = char.MinValue;
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index = this._Position;
            for (; this._Position < this._Length; this._Position++)
            {
                int index2 = chrs.IndexOf(this._Text[this._Position]);
                if (index2 >= 0)
                {
                    resultChar = chrs[index2];
                    var result = this._Text.Substring(index, this._Position - index);
                    this._Position++;
                    return result;
                }
            }
            return this._Text.Substring(index);
        } 
        public string ReadStyleExtValue()
        {
            var c = this.ReadContentChar();
            if (c == '(')
            {
                if (this._Position == this._Length)
                {
                    return string.Empty;
                }
                int level = 0;
                for (int iCount = this._Position; iCount < this._Length; iCount++)
                {
                    var c2 = this._Text[iCount];
                    if (c2 == '(')
                    {
                        level++;
                    }
                    else if (c2 == ')')
                    {
                        level--;
                        if (level < 0)
                        {
                            string result = GetSubStringUseTable(this._Position, iCount - this._Position);// this._Text.Substring(this._Position, iCount - this._Position);
                            this._Position = iCount + 1;
                            return result;
                        }
                    }
                }
                var result2 = GetSubStringUseTable(this._Position, this._Length - this._Position);// this._Text.Substring(this._Position, this._Length - this._Position);
                this._Position = this._Length;
                return result2;
            }
            return null;
        }

        //public string Trim( string txt )
        //{
        //    if (txt != null && txt.Length > 0)
        //    {

        //    }
        //    else
        //    {
        //        return txt;
        //    }
        //}

        public void MoveAfterChar( char c )
        {
            if(this._Position < this._Length )
            {
                int index = this._Text.IndexOf(c, this._Position);
                if(index > 0)
                {
                    this._Position = index + 1;
                }
                else
                {
                    this._Position = this._Length;
                }
            }
        }

        public string ReadAfterCharExcludeLastChar(char c , bool trim = false )
        {
            if (this._Position == this._Length)
            {
                return string.Empty;
            }
            int index2 = this._Text.IndexOf(c, this._Position);
            if (index2 > 0)
            {
                var result = GetSubStringUseTable( this._Position, index2 - this._Position , trim );
                this._Position = index2 + 1;
                return result;
            }
            else
            {
                var result = this._Text.Substring(this._Position);
                this._Position = this._Length;
                return result;
            }
        }

        private static readonly byte[] EmptyBytes = new byte[0];
        private byte[] _byteBuffer = new byte[1024];

        public byte[] ReadBinaryFromHex()
        {
            //var list = new List<byte>();
            int curValue = -1;
            int bufferSize = _byteBuffer.Length - 10;
            int bufferPosition = 0;
            var textLength = this._Text.Length;
            var thisText = this._Text;
            for (; this._Position < textLength; this._Position++)
            {

                char c = thisText[this._Position];
                if (c == '/' && this._Position < textLength - 1 && thisText[this._Position + 1] == '/')
                {
                    // 遇到注释,跳过当前行剩余内容
                    this.MoveNextLine();
                }
                else if (c == ')')
                {
                    // 遇到圆括号，退出
                    this.MoveNextLine();
                    break;
                }
                else
                {
                    var index = DCUtils.IndexOfHexChar(c);// DCILDocument.IndexOfHexChar(c);
                    if (index >= 0)
                    {
                        if (curValue >= 0)
                        {
                            if (bufferSize < bufferPosition)
                            {
                                var temp = new byte[(int)(bufferPosition * 1.5) + 10];
                                Array.Copy(_byteBuffer, temp, _byteBuffer.Length);
                                _byteBuffer = temp;
                                bufferSize = _byteBuffer.Length - 10;
                            }
                            _byteBuffer[bufferPosition++] = (byte)((curValue << 4) + index);
                            //list.Add((byte)((curValue << 4) + index));
                            curValue = -1;
                        }
                        else
                        {
                            curValue = index;
                        }
                    }
                }
            }
            if (curValue >= 0)
            {
                _byteBuffer[bufferPosition++] = (byte)curValue;
                //list.Add((byte)curValue);
            }
            if (bufferPosition > 0)
            {
                var result = new byte[bufferPosition];
                Array.Copy(this._byteBuffer, result, bufferPosition);
                return result;
            }
            return null;
            // return list.ToArray();
        }
        public int Read()
        {
            if (this._Position < this._Length)
            {
                return this._Text[this._Position++];
            }
            else
            {
                return -1;
            }
        }
        public char Peek()
        {
            if (this._Position < this._Length)
            {
                return this._Text[this._Position];
            }
            else
            {
                return char.MinValue;
            }
        }
        public string ReadLineTrim()
        {
            return this.ReadLine()?.Trim();

        }
        public string ReadLineTrimRemoveComment()
        {
            var line = this.ReadLine();
            if (line != null && line.Length > 0)
            {
                int index = line.IndexOf("//");
                if (index > 0)
                {
                    int index2 = line.IndexOf('"');
                    if (index2 < 0 || index2 > index)
                    {
                        return line.Substring(0, index).Trim();
                    }
                }
            }
            return line.Trim();
        }

        public static string ToRawILText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length == 0)
            {
                return "\"\"";
            }
            bool isBinary = false;
            foreach (var c in text)
            {
                if (c > 128)
                {
                    isBinary = true;
                    break;
                }
            }
            if (isBinary)
            {
                var bs = Encoding.Unicode.GetBytes(text);
                var w = new DCILWriter(new StringBuilder());
                w.Write("bytearray(");
                w.WriteHexs(bs);
                w.Write(")");
                return w.ToString();
            }
            else
            {
                var result = new System.Text.StringBuilder();
                result.Append('"');
                foreach (var c in text)
                {
                    switch (c)
                    {
                        case '\r': result.Append(@"\r"); break;
                        case '\n': result.Append(@"\n"); break;
                        case '\'': result.Append(@"\'"); break;
                        case '\\': result.Append(@"\\"); break;
                        case '"': result.Append("\\\""); break;
                        case '\b': result.Append(@"\b"); break;
                        case '\f': result.Append(@"\f"); break;
                        case '\t': result.Append(@"\t"); break;
                        default: result.Append(c); break;
                    }
                }
                result.Append('"');
                return result.ToString();
            }
        }
        //public static int _SimpleString = 0;
        //private static int _AllStrings = 0;

        public class StringValueInfo
        {
            public string Value = null;
            public bool IsBinary = false;
            public string ILRawText = null;
            public byte[] BianryData = null;
        }

        public StringValueInfo ReadStringValue()
        {
            //_AllStrings++;
            StringValueInfo info = new StringValueInfo();
            this.SkipWhitespace();
            int startPosition = this._Position;
            var thisText = this._Text;
            var thisLength = this._Length;
            if (thisText[this._Position] == '"')
            {
                // 明文格式的字符串
                this._Position++;
                for (; this._Position < thisLength; this._Position++)
                {
                    var c = thisText[this._Position];
                    if (c == '\\')
                    {
                        // 遇到转义字符
                        break;
                    }
                    else if (c == '"')
                    {
                        // 结束字符串定义
                        int posBack = this._Position;
                        this.MoveNextLine();
                        var nextChar = this.PeekContentChar();
                        if (nextChar != '+')
                        {
                            // 不是多行文本，是单行文本。在大多数情况下都是无转义的单行文本
                            //_SimpleString++;
                            info.ILRawText = this.GetSubStringUseTable(startPosition, posBack - startPosition + 1);
                            if ( info.ILRawText.Length == 2)
                            {
                                info.Value = string.Empty;
                            }
                            else
                            {
                                info.Value = this.GetSubStringUseTable(startPosition + 1, posBack - startPosition + 1 - 2);// rawILText.Substring(1, rawILText.Length - 2);
                            }
                            return info;
                        }
                        break;
                    }
                }
                // 恢复原始位置
                this._Position = startPosition;
                var strFinallyValue = new StringBuilder();
                bool isInSting = false;
                int endIndex = this._Position;
                int thisLength_1 = thisLength - 1;
                for (; this._Position < thisLength; this._Position++)
                {
                    var c = thisText[this._Position];
                    if (c == '"')
                    {
                        // 切换字符串定义模式
                        isInSting = !isInSting;
                        if (isInSting == false)
                        {
                            endIndex = this._Position + 1;
                            // 如果不是定义字符串，则跳到下一行。
                            this.MoveNextLine();
                            //this.SkipWhitespace();
                        }
                    }
                    else if (isInSting)
                    {
                        // 正在定义一个字符串
                        if (c == '\\' && this._Position < thisLength_1)
                        {
                            // 遇到转义字符
                            this._Position++;
                            var nc = thisText[this._Position];
                            switch (nc)
                            {
                                case 'r': strFinallyValue.Append('\r'); break;
                                case 'n': strFinallyValue.Append('\n'); break;
                                case '\'': strFinallyValue.Append('\''); break;
                                case '"': strFinallyValue.Append('"'); break;
                                case '\\': strFinallyValue.Append('\\'); break;
                                case 'b': strFinallyValue.Append('\b'); break;
                                case 'f': strFinallyValue.Append('\f'); break;
                                case 't': strFinallyValue.Append('\t'); break;
                                default: strFinallyValue.Append(nc); break;
                            }
                        }
                        else
                        {
                            strFinallyValue.Append(c);
                        }
                    }
                    else if (c == '+')
                    {
                        // 定义多行字符串
                    }
                    else if (IsWhiteSpace(c) == false)
                    {
                        // 不是定义多行字符串
                        this._Position--;
                        break;
                    }
                }//for
                info.ILRawText = thisText.Substring(startPosition, endIndex - startPosition);
                info.Value = strFinallyValue.ToString();
                return info;
            }
            else if (string.Compare(thisText, this._Position, _bytearray, 0, _bytearray.Length, StringComparison.Ordinal) == 0)
            {
                // 二进制格式定义的字符串
                this._Position += _bytearray.Length;
                this.MoveAfterChar('(');
                var bs = this.ReadBinaryFromHex();
                //info.ILRawText = thisText.Substring(startPosition, this._Position - startPosition);
                info.BianryData = bs;
                info.IsBinary = true;
                if (bs != null && bs.Length > 0)
                {
                    string result = Encoding.Unicode.GetString(bs);
                    if (result.Length < 10)
                    {
                        result = DCUtils.GetStringUseTable(result);
                    }
                    info.Value = result;
                    return info;
                }
            }
            throw new NotSupportedException("no string value at line " + this.CurrentLineIndex());
        }

        internal static readonly string _bytearray = "bytearray";

        public string ReadLine()
        {
            int i;
            for (i = this._Position; i < this._Length; i++)
            {
                char c = this._Text[i];
                if (c == '\r' || c == '\n')
                {
                    string result = this._Text.Substring(this._Position, i - this._Position);
                    this._Position = i + 1;
                    if (c == '\r' && this._Position < this._Length && this._Text[this._Position] == '\n')
                    {
                        this._Position++;
                    }
                    return result;
                }
            }
            if (i > this._Position)
            {
                string result2 = this._Text.Substring(this._Position, i - this._Position);
                this._Position = i;
                return result2;
            }
            return null;
        }

        public void MoveNextLine()
        {
            int i;
            for (i = this._Position; i < this._Length; i++)
            {
                char c = this._Text[i];
                if (c == '\r' || c == '\n')
                {
                    this._Position = i + 1;
                    if (c == '\r' && this._Position < this._Length && this._Text[this._Position] == '\n')
                    {
                        this._Position++;
                    }
                    return;
                }
            }
            if (i > this._Position)
            {
                this._Position = i;
            }
        }

        public static int ParseArrayIndex(string text)
        {
            if (text != null && text.Length > 0)
            {
                int len = text.Length;
                for (int iCount = 0; iCount < len; iCount++)
                {
                    if (text[iCount] == '[')
                    {
                        int result = 0;
                        for (int iCount2 = iCount + 1; iCount2 < len; iCount2++)
                        {
                            var c = text[iCount2];
                            if (c >= '0' && c <= '9')
                            {
                                result = result * 10 + c - '0';
                            }
                            else// if( c == ']')
                            {
                                break;
                            }
                        }
                        return result;
                    }
                }
            }
            return int.MinValue;
        }

        public static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
        private string _Text = null;

        private void SetSourceText(string txt)
        {
            if (txt == null)
            {
                throw new ArgumentNullException("txt");
            }
            this._Text = txt;
            this._StringTable = new Dictionary<long, string>(new KeyStringComparere(this._Text));
            this._OperCodeTable = new Dictionary<long, DCILOperCodeDefine>(new KeyStringComparere(this._Text));
            this._Length = txt.Length;
        }

        private int _Position = 0;
        public int Position
        {
            get
            {
                return this._Position;
            }
            set
            {
                this._Position = value;
            }
        }
        private int _Length = 0;
        public int Length
        {
            get
            {
                return this._Length;
            }
        }
    }
}
