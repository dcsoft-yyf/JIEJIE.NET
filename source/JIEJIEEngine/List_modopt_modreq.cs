/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */

namespace JIEJIE
{
    internal class List_modopt_modreq : System.Collections.Generic.List<string>
    {
        public static bool IsStartWord(string strWord )
        {
            return strWord == "modopt" || strWord == "modreq" || strWord == "*";
        }
        public bool Read( string strWord , DCILReader reader )
        {
            if( strWord == "modopt" || strWord == "modreq" || strWord == "*")
            {
                this.Add(strWord);
                string strValue = reader.ReadStyleExtValue().Trim();
                if ( strValue != null && strValue.Length > 0)
                {
                    if (strWord == "*")
                    {

                    }
                    string strTypeName44 = strValue;
                    if (strValue.StartsWith("valuetype "))
                    {
                        strTypeName44 = strValue.Substring(9).Trim();
                    }
                    strTypeName44 = strTypeName44.Replace("*", "");
                    reader.AddPreserverTypeName(strTypeName44);
                }
                if(reader.Peek() == '*')
                {
                    strValue = "(" + strValue + ")";
                    while( reader.Peek() == '*')
                    {
                        strValue = strValue + "*";
                        reader.Position++;
                    }
                    this.Add(strValue);
                }
                else
                {
                    this.Add("(" + strValue + ")");
                }
                return true;
            }
            return false;
        }
        public void Write(DCILWriter writer )
        {
            if(this.Count > 0 )
            {
                for( int iCount = 0;iCount < this.Count;iCount += 2 )
                {
                    if( this[iCount] == string.Empty )
                    {

                    }
                    writer.Write(" " + this[iCount] + " " + this[iCount+1] + " ");
                }
            }
        }
    }
}
