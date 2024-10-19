using System;
using System.Collections.Generic;
using System.Text;

namespace JIEJIE
{
     
    /// <summary>
    /// 简单的字符串通配符，只支持星号和问号
    /// </summary>
    public class StringPattern
    {
        public static StringPattern[] CreatePatterns(string strSettings)
        {
            if (strSettings != null && strSettings.Length > 0)
            {
                List<StringPattern> specifyRenameTypes = null;
                specifyRenameTypes = new List<StringPattern>();
                foreach (var item in strSettings.Split(new char[] { ',', '|' }))
                {
                    var item2 = item.Trim();
                    if (item2.Length > 0)
                    {
                        if (item2[0] == '+')
                        {
                            if (item2.Length > 1)
                            {
                                specifyRenameTypes.Add(new StringPattern(item2.Substring(1), true, true));
                            }
                        }
                        else if (item2[0] == '-')
                        {
                            if (item2.Length > 0)
                            {
                                specifyRenameTypes.Add(new StringPattern(item2.Substring(1), true, false));
                            }
                        }
                        else
                        {
                            specifyRenameTypes.Add(new StringPattern(item2, true, true));
                        }
                    }
                }
                if (specifyRenameTypes.Count > 0)
                {
                    return specifyRenameTypes.ToArray();
                }
            }
            return null;
        }

        public StringPattern(string pattern, bool ignoreCase, bool isInclude)
        {
            this.IsInclude = isInclude;
            this._Pattern = pattern;
            if (this._Pattern == null)
            {
                this._Pattern = string.Empty;
            }
            this._CompareMode = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (this._Pattern == "*")
            {
                this._Both = true;
            }
            else
            {
                if (this._Pattern != null
                    && this._Pattern.Length > 0
                    && (this._Pattern.IndexOf('*') >= 0 || this._Pattern.IndexOf('?') >= 0))
                {
                    var items = new List<string>();
                    var lastIndex = 0;
                    for (var iCount = 0; iCount < this._Pattern.Length; iCount++)
                    {
                        var c = this._Pattern[iCount];
                        if (c == '*' || c == '?')
                        {
                            if (iCount > lastIndex)
                            {
                                items.Add(this._Pattern.Substring(lastIndex, iCount - lastIndex));
                            }
                            items.Add(c.ToString());
                            lastIndex = iCount + 1;
                        }
                    }
                    if (lastIndex < this._Pattern.Length - 1)
                    {
                        items.Add(this._Pattern.Substring(lastIndex));
                    }
                    this._Items = items.ToArray();
                }
            }
        }
        private readonly bool _Both = false;
        public readonly bool IsInclude;
        private  string _Pattern;
        private readonly StringComparison _CompareMode;
        private  string[] _Items = null;
        public bool Match( string txt )
        {
            if(this._Both )
            {
                // 无条件全部匹配
                return true;
            }
            if(txt == null || txt.Length == 0 )
            {
                return false;
            }
            if( this._Items == null )
            {
                return string.Equals(txt, this._Pattern, this._CompareMode);
            }
            else
            {
                int lastIndex = 0;
                var itemsLength = this._Items.Length;
                for (var iCount = 0; iCount < itemsLength; iCount++)
                {
                    var item = this._Items[iCount];
                    if (item == "*")
                    {
                        // 匹配多个字符
                        iCount++;
                        while( iCount < itemsLength )
                        {
                            var nextItem = this._Items[iCount];
                            if(nextItem !="*" && nextItem != "?")
                            {
                                var index9 = txt.IndexOf(nextItem, lastIndex, this._CompareMode);
                                if(index9 >= 0)
                                {
                                    lastIndex = index9 + nextItem.Length;
                                    break;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else if (item == "?")
                    {
                        // 匹配单个字符
                        if((++lastIndex)>= txt.Length )
                        {
                            // 长度不够
                            return false;
                        }
                    }
                    else
                    {
                        if(string.Compare(  item , 0 , txt , lastIndex , item.Length , this._CompareMode) != 0)
                        {
                            // 不匹配
                            return false;
                        }
                        lastIndex += item.Length;
                    }
                }
            }
            return true;
        }
        public void Clear()
        {
            this._Pattern = null;
            this._Items = null;
        }
    }
}
