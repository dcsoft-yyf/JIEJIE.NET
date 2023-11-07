/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;
using System.Text;

namespace JIEJIE
{
    /// <summary>
    /// 类型名称信息
    /// </summary>
    internal class DCILTypeNameInfo :IDisposable
    {
        public DCILTypeNameInfo()
        {

        }
        public DCILTypeNameInfo(string name)
        {
            Parse(name);
        }
        public virtual void Dispose()
        {
            this.AssemblyName = null;
            this.AssemblyCulture = null;
            this.AssemblyPublicKeyToken = null;
            this.AssemblyVersion = null;
        }
        public void Parse(string name)
        {
            if (name == null || name.Length == 0)
            {
                return;
            }
            if (name[0] == '[')
            {
                int index = name.IndexOf(']');
                if (index > 0)
                {
                    // 符合 [LibrayName]TypeName,例如 [System]System.Object
                    this.AsmNamePrefix = true;
                    this.AssemblyName = name.Substring(1, index - 2);
                    this.TypeName = name.Substring(index + 1);
                    return;
                }
            }
            int index2 = name.IndexOf(',');
            if (index2 > 0)
            {
                this.AsmNamePrefix = false;
                // 例如 System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                this.TypeName = name.Substring(0, index2);
                var items = name.Substring(index2 + 1).Split(',');
                foreach (var item in items)
                {
                    var item2 = item.Trim();
                    int index3 = item2.IndexOf('=');
                    if (index3 < 0 && this.AssemblyName == null)
                    {
                        this.AssemblyName = item2;
                    }
                    else if (index3 > 0)
                    {
                        var name2 = item2.Substring(0, index3).Trim();
                        var v2 = item2.Substring(index3 + 1);
                        switch (name2.ToLower())
                        {
                            case "version": this.AssemblyVersion = v2; break;
                            case "culture": this.AssemblyCulture = v2; break;
                            case "publickeytoken": this.AssemblyPublicKeyToken = v2; break;
                        }
                    }
                }
            }
            if (this.TypeName == null)
            {
                this.TypeName = name;
            }
            this.TypeName = DCUtils.GetStringUseTable(this.TypeName);
            this.AssemblyCulture = DCUtils.GetStringUseTable(this.AssemblyCulture);
            this.AssemblyName = DCUtils.GetStringUseTable(this.AssemblyName);
            this.AssemblyPublicKeyToken = DCUtils.GetStringUseTable(this.AssemblyPublicKeyToken);
            this.AssemblyVersion = DCUtils.GetStringUseTable(this.AssemblyVersion);

        }
        public string TypeName = null;
        public string AssemblyName = null;
        public bool HasAssemblyName
        {
            get
            {
                return this.AssemblyName != null && this.AssemblyName.Length > 0;
            }
        }
        public string AssemblyVersion = null;
        public string AssemblyCulture = null;
        public string AssemblyPublicKeyToken = null;
        public bool AsmNamePrefix = false;
        public override string ToString()
        {
            return ToTypeString();
        }
        protected string ToTypeString()
        {
            var str = new StringBuilder();
            if (this.AsmNamePrefix)
            {
                str.Append("[" + this.AssemblyName + "]");
                str.Append(this.TypeName);
            }
            else
            {
                str.Append(this.TypeName);
                bool hasItem = false;
                if (this.HasAssemblyName)
                {
                    str.Append(',');
                    str.Append(this.AssemblyName);
                    if (this.AssemblyName == "DCSoft.Common")
                    {

                    }
                    hasItem = true;
                }
                if (this.AssemblyVersion != null && this.AssemblyVersion.Length > 0)
                {
                    if (hasItem)
                    {
                        str.Append(',');
                    }
                    hasItem = true;
                    str.Append("Version=" + this.AssemblyVersion);
                }
                if (this.AssemblyCulture != null && this.AssemblyCulture.Length > 0)
                {
                    if (hasItem)
                    {
                        str.Append(',');
                    }
                    str.Append("Culture=" + this.AssemblyCulture);
                    hasItem = true;
                }
                if (this.AssemblyPublicKeyToken != null && this.AssemblyPublicKeyToken.Length > 0)
                {
                    if (hasItem)
                    {
                        str.Append(',');
                    }
                    str.Append("PublicKeyToken=" + this.AssemblyPublicKeyToken);
                }
            }
            return str.ToString();
        }
    }
}
