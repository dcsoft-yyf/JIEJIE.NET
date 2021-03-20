using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCNETProtector
{
    internal class DCILProperty : DCILGroup
    {
        internal override void SetHeader(string strHeader)
        {
            this.Header = strHeader;

            var words = DCILDocument.SplitByWhitespace(DCILDocument.RemoveChars(strHeader, "()"));
            this.Name = words[words.Count - 1];
            this.ValueTypeName = words[words.Count - 2];
        }
        public string ValueTypeName = null;
        public bool HasGetMethod = false;
        public bool HasSetMethod = false;
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Property " + this.Name);
            if (this.HasGetMethod)
            {
                str.Append(" get; ");
            }
            if (this.HasSetMethod)
            {
                str.Append(" set;");
            }
            return str.ToString();
        }
    }
}
