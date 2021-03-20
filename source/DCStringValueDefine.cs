using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCNETProtector
{
    internal class DCStringValueDefine
    {
        public string NativeSourcde = null;
        public bool IsSetStaticField = false;
        public string MethodName = null;
        public int LineIndex = -1;
        public int EndLineIndex = -1;
        public string Value = null;
        public string FinalValue = null;
        public bool IsBinary = false;
        public string LabelID = null;
        public override string ToString()
        {
            return this.LineIndex + " : " + this.Value + "  #" + this.NativeSourcde;
        }

    }
}
