using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  DCNETProtector
{
    internal class DCILMethod : DCILGroup
    {
        public string ReturnType = null;
        internal override void SetHeader(string strHeader)
        {
            this.Header = strHeader;
            var items = DCILDocument.SplitByWhitespace(strHeader);
            for (int itemCount = 0; itemCount < items.Count; itemCount++)
            {
                var item = items[itemCount];
                int index10 = item.IndexOf('(');
                if (index10 > 0)
                {
                    this.Name = item.Substring(0, index10);
                    this.ReturnType = items[itemCount - 1];
                    //currentMethodName = group.Name;
                    break;
                }
            }
        }
        //public int ComponentResourceManagerLineIndex = -1;
        public override string ToString()
        {
            return "Method " + this.ReturnType + " " + this.Name;
        }
    }
}
