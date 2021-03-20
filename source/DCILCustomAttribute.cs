using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCNETProtector
{
    internal class DCILCustomAttribute : DCILGroup
    {
        internal override void SetHeader(string text)
        {
            this.Header = text;
            int indexEqual = text.IndexOf('=');
            if (indexEqual > 0)
            {
                this.HexValue = DCILDocument.GetHexString(text.Substring(indexEqual + 1));
            }
            var index = text.IndexOf("::.ctor", StringComparison.Ordinal);
            for (int iCount = index; iCount >= 0; iCount--)
            {
                if (DCILDocument.IsWhitespace(text[iCount]))// char.IsWhiteSpace( text[iCount]))
                {
                    this.Name = text.Substring(iCount, index - iCount);
                    break;
                }
            }
        }

        public string HexValue = null;
        public override string ToString()
        {
            return "Attribute " + this.Name;
        }
    }
}
