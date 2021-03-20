using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCNETProtector
{
    internal class DCILClass : DCILGroup
    {
        public DCILClass()
        {
            base.ChildNodes = new List<DCILGroup>();
        }
        public List<string> ImplementsInterfaces = null;
        public List<int> FieldLineIndexs = new List<int>();
        public bool Modified = false;
        public string BaseTypeName = null;
        internal override void SetHeader(string strHeader)
        {
            this.Header = strHeader;
            var items = DCILDocument.SplitByWhitespace(strHeader);
            for (int itemCount = 0; itemCount < items.Count; itemCount++)
            {
                if (items[itemCount] == "extends" && itemCount > 0)
                {
                    this.Name = items[itemCount - 1];
                    this.BaseTypeName = items[itemCount + 1];
                    return;
                }
            }
            this.Name = items[items.Count - 1];

            this.IsInterface = items.Contains("interface");
            this.IsPublic = items.Contains("public");
            int index2 = items.IndexOf("implements");
            if (index2 > 0)
            {
                this.ImplementsInterfaces = new List<string>();
                for (int iCount = index2; iCount < items.Count; iCount++)
                {
                    this.ImplementsInterfaces.Add(items[iCount]);
                }
            }
            //if (strHeader.Contains("WriterControl") && this.IsPublic == false )
            //{

            //}
        }

        public bool IsInterface = false;
        public bool IsPublic = false;
        /// <summary>
        /// 是否为访问程序集资源的包装类型
        /// </summary>
        /// <returns></returns>
        public bool IsResoucePackage()
        {
            if (this.CustomAttributes != null
                && this.BaseTypeName != null
                && this.BaseTypeName.Contains("System.Object"))
            {
                int flagCount = 0;
                foreach (var item in this.CustomAttributes)
                {
                    if (item.Name.Contains("GeneratedCodeAttribute")
                        || item.Name.Contains("DebuggerNonUserCodeAttribute")
                        || item.Name.Contains("CompilerGeneratedAttribute"))
                    {
                        flagCount++;
                    }
                }
                foreach (var item in this.ChildNodes)
                {
                    if (item is DCILProperty && item.Name == "ResourceManager")
                    {
                        flagCount++;
                    }
                }
                return flagCount == 4;
            }
            return false;
        }

        public override string ToString()
        {
            if (this.IsInterface)
            {
                return "Interface " + this.Name;
            }
            else if (this.BaseTypeName == null || this.BaseTypeName.Contains("System.Object"))
            {
                return "Class " + this.Name;
            }
            else
            {
                return "Class " + this.Name + " : " + this.BaseTypeName;
            }
        }
    }
}
