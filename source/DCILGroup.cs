using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCNETProtector
{
    internal class DCILGroup
    {
        public DCILDocument OwnerDocument = null;
        public string Name = null;
        public string Type = null;
        public string Header = null;
        internal virtual void SetHeader(string strHeader)
        {
            this.Header = strHeader;
        }
        public int StartLineIndex = 0;
        public int BodyLineIndex = 0;
        public int EndLineIndex = 0;
        public List<DCILGroup> ChildNodes = null;
        public List<DCILCustomAttribute> CustomAttributes = null;
        public int Level = 0;
        public override string ToString()
        {
            if (this.ChildNodes != null && this.ChildNodes.Count > 0)
            {
                return this.Type + "#" + this.Name + " " + this.ChildNodes.Count + "个子节点";
            }
            else
            {
                return this.Type + "#" + this.Name;
            }
        }

    }
}
