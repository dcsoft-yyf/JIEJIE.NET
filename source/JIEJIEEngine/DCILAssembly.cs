/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System.Collections.Generic;

namespace JIEJIE
{
    internal class DCILAssembly : DCILMemberInfo
    {
        public const string TagName = ".assembly";
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Assembly;
            }
        }
        public bool IsExtern = false;

        public List<string> MResourceNames = null;

        public override void Load(DCILReader reader)
        {
            LoadHeader(reader);
            LoadContent(reader);
        }

        public void LoadHeader(DCILReader reader)
        {
            var strWord = reader.ReadWord();
            if (strWord == "extern")
            {
                this.IsExtern = true;
                this._Name = reader.ReadWord();
            }
            else
            {
                this.IsExtern = false;
                this._Name = strWord;
            }
        }
        public void LoadContent(DCILReader reader)
        {
            if (this.ChildNodes == null)
            {
                this.ChildNodes = new DCILObjectList();
            }
            reader.MoveAfterChar('{');// reader.ReadAfterChar('{');
            while (reader.HasContentLeft())
            {
                var strWord = reader.ReadWord();
                if (strWord == "}")
                {
                    // 退出代码组
                    break;
                }
                switch (strWord)
                {
                    case DCILCustomAttribute.TagName_custom:
                        base.ReadCustomAttribute(reader);
                        break;
                    case ".mresouce":
                        {
                            if (this.MResourceNames == null)
                            {
                                this.MResourceNames = new List<string>();

                                var strWord2 = reader.ReadWord();
                                if (strWord2 == "public")
                                {
                                    var name2 = reader.ReadLine()?.Trim();
                                    if (name2 != null && name2.Length > 0)
                                    {
                                        this.MResourceNames.Add(name2);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        {
                            this.ChildNodes.Add(new DCILUnknowObject(strWord, reader));
                        }
                        break;
                }

            }
        }
        //private Dictionary<string, DCILClass> _ClassMap = new Dictionary<string, DCILClass>();

        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".assembly ");
            if (this.IsExtern)
            {
                writer.Write(" extern ");
                writer.WriteLine(this._Name);
            }
            else
            {
                writer.WriteLine(this._Name);
            }
            writer.WriteStartGroup();
            base.WriteCustomAttributes(writer);
            writer.WriteObjects(this.ChildNodes);
            writer.WriteEndGroup();
        }

        public override string ToString()
        {
            if (this.IsExtern)
            {
                return ".assembly extern " + this._Name;
            }
            else
            {
                return ".assembly " + this._Name;
            }
        }
    }


}
