/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;

namespace JIEJIE
{
    internal class IDGenerator
    {
        public IDGenerator(string strPreFix, string memberPrefix , int inputGenCount = int.MinValue)
        {
            if (strPreFix == null || strPreFix.Trim().Length == 0)
            {
                this._ClassNamePrefx = "_jiejie";
            }
            else
            {
                this._ClassNamePrefx = strPreFix.Trim();
            }
            if (memberPrefix == null || memberPrefix.Trim().Length == 0)
            {
                this._MemberNamePrefix = "_jj";
            }
            else
            {
                this._MemberNamePrefix = memberPrefix.Trim();
            }
            if (inputGenCount == int.MinValue)
            {
                this.GenCount = GenCountBase + _rnd.Next(10, 100);
            }
            else
            {
                this.GenCount = inputGenCount;
            }
        }

        private static readonly System.Random _rnd = new Random();
        private static readonly string _Chars = "lkjhgfdsaqwertyuiopmnbvcxz";//"mn0O1l";
        private static readonly int _CharsLength = _Chars.Length;
        private readonly string _ClassNamePrefx = null;
        private readonly string _MemberNamePrefix = null;
        
        public static int GenCountBase = 0;

        public int GenCount = 0;
        public bool DebugMode = false;

        public string GenerateIDForClass(string oldName, DCILObject obj)
        {
            if (this.DebugMode)
            {
                return DebugModeGenerateID(oldName, obj);
            }
            string id = this.InnerGenerateID();
            if (this._ClassNamePrefx != null)
            {
                return this._ClassNamePrefx + id;
            }
            else
            {
                return id;
            }
        }

        [ThreadStatic]
        private static string _CreateTime = null;
       
        public string GenerateIDForMember(string oldName, DCILObject obj)
        {
            if (this.DebugMode)
            {
                return DebugModeGenerateID(oldName, obj);
            }
            var id = this.InnerGenerateID();
            if (this._MemberNamePrefix != null)
            {
                id = this._MemberNamePrefix + id;
            }
            if (_rnd.Next(0, 100) < 2)
            {
                // 有较小的概率添加当前时间
                if (_CreateTime == null)
                {
                    _CreateTime = "_jiejie" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                id = id + _CreateTime;
            }
            return DCUtils.GetStringUseTable( id );
        }

        private string DebugModeGenerateID(string oldName, DCILObject obj)
        {
            if (oldName.StartsWith("std.'map<ATL::CStringT<wchar_"))
            {

            }
            //this.GenCount++;
            int idFix = obj.InstanceIndex;
            string result = null;
            if (oldName[0] == '\'')
            {
                result = "'__" + oldName.Substring(1);
            }
            else
            {
                result = "__" + oldName;
            }
            if (result[result.Length - 1] == '\'')
            {
                result = result.Substring(0, result.Length - 1) + "_" + idFix.ToString() + "'";
            }
            else
            {
                result = result + "_" + idFix.ToString();
            }
            return result;
        }
         
        private char[] _ResultBuffer = new char[20];
        private string InnerGenerateID()
        {
            int gc = this.GenCount++;
            for (int iCount = 0; iCount < 20; iCount++)
            {
                int index = gc % _CharsLength;
                _ResultBuffer[iCount] = _Chars[index];
                gc = (gc - index) / _CharsLength;
                if (gc == 0)
                {
                    return new string(this._ResultBuffer, 0, iCount + 1);
                }
            }
            return null;
        }
    }

}
