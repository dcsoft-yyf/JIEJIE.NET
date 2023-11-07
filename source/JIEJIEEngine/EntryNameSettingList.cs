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
    /// <summary>
    /// 实体名称设置列表
    /// </summary>
    internal class EntryNameSettingList : List<EntryNameSettingList.EntryNameSettingItem>
    {
        public EntryNameSettingList(string text)
        {
            if (text != null && text.Length > 0)
            {
                foreach (var strItem in text.Split('|'))
                {
                    this.AddItem(strItem);
                }
            }
        }
        public override string ToString()
        {
            var str = new StringBuilder();
            foreach( var item in this )
            {
                if(str.Length > 0 )
                {
                    str.Append('|');
                }
                str.Append(item.ToString());
            }
            return str.ToString();
        }
        /// <summary>
        /// 是否包含指定名称
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>是否包含</returns>
        public bool IsInclude( string name , bool defaultValue )
        {
            foreach( var item in this )
            {
                if(item.IsMatch( name ))
                {
                    return item.IsInclude;
                }
            }
            return defaultValue;
        }

        public EntryNameSettingItem GetItem( string name )
        {
            foreach (var item in this)
            {
                if (item.IsMatch(name))
                {
                    return item;
                }
            }
            return null;
        }


        public EntryNameSettingItem AddItem(string name)
        {
            if (name != null && name.Length > 0)
            {
                var item = new EntryNameSettingItem(name);
                if (item.Name != null && item.Name.Length > 0)
                {
                    this.Add(item);
                    return item;
                }
            }
            return null;
        }

        internal class EntryNameSettingItem
        {
            public EntryNameSettingItem( string strName )
            {
                if(strName == null || strName.Length == 0 )
                {
                    return;
                }
                if (strName[0] == '-')
                {
                    this.IsInclude = false;
                    strName = strName.Substring(1).Trim();
                }
                else if (strName[0] == '+')
                {
                    this.IsInclude = true;
                    strName = strName.Substring(1).Trim();
                }
                else
                {
                    this.IsInclude = true;
                }
                if (strName.Length == 0)
                {
                    return;
                }
                this.Name = strName;
                this.IsRegex = false;
                foreach (var c in strName)
                {
                    if (_RegexChars.IndexOf(c) >= 0)
                    {
                        // 遇到特别字符，认为是正则表达式
                        this.IsRegex = true;
                        break;
                    }
                }
            }

            private static readonly string _RegexChars = @"$^{[(|)*+?\";
            /// <summary>
            /// 名称
            /// </summary>
            public readonly string Name = null;
            /// <summary>
            /// 是否为正则表达式
            /// </summary>
            public readonly bool IsRegex = false;
            /// <summary>
            /// 是否包含在输出结果中
            /// </summary>
            public readonly bool IsInclude = true;

            public override string ToString()
            {
                if( this.IsInclude )
                {
                    return "+" + this.Name;
                }
                else
                {
                    return "-" + this.Name;
                }
            }
            private System.Text.RegularExpressions.Regex _Regex = null;
            /// <summary>
            /// 判断是否命中本设置
            /// </summary>
            /// <param name="resName"></param>
            /// <returns></returns>
            public bool IsMatch( string resName )
            {
                if( this.Name == null || this.Name.Length == 0 )
                {
                    return false;
                }
                if( this.IsRegex )
                {
                    if( this._Regex == null  )
                    {
                        this._Regex = new System.Text.RegularExpressions.Regex(this.Name);
                    }
                    return this._Regex.IsMatch(resName);
                }
                else
                {
                    return string.Compare(this.Name, resName, StringComparison.OrdinalIgnoreCase) == 0;
                }
            }
             
        }
    }
}
