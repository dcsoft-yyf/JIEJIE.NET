/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System.Collections.Generic;
using System.IO;

namespace JIEJIE
{
    internal class ReadCustomAttributeValueArgs
    {
        public ReadCustomAttributeValueArgs(List<DCILDocument> documents, DCILDocument mainDoc, string seachPath)
        {
            this.Documents = documents;
            this.MainDocument = mainDoc;
            this.AssemblySeachPath = seachPath;
        }
        public BinaryReader Reader = null;
        public string AssemblySeachPath = null;
        public List<DCILDocument> Documents = null;
        public DCILDocument MainDocument = null;

        public DCILDocument GetDocument(string assemblyName)
        {
            if (this.Documents != null
                && assemblyName != null
                && assemblyName.Length > 0 )
            {
                foreach (var doc in this.Documents)
                {
                    if (string.Compare(doc.Name, assemblyName, true) == 0)
                    {
                        return doc;
                    }
                    if (doc.AssemblyFileName != null
                        && string.Compare(Path.GetFileNameWithoutExtension(doc.AssemblyFileName), assemblyName, true) == 0)
                    {
                        return doc;
                    }
                }
            }
            return this.MainDocument;
        }
       

    }
}
