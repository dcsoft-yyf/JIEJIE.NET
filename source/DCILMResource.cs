using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCNETProtector
{
    internal class DCILMResource : DCILGroup
    {
        public override string ToString()
        {
            return "Resource " + this.Name;
        }
        public string FileName
        {
            get
            {
                return System.IO.Path.Combine(this.OwnerDocument.RootPath, this.Name + DCILDocument.EXT_resources);
            }
        }
    }
}
