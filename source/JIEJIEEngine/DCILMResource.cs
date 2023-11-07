/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace JIEJIE
{
    internal class DCILMResource : DCILObject
    {
        public const string EXT_Resources = ".resources";
       
        public static void LoadData(System.Collections.Generic.IEnumerable<DCILMResource> reses, string basePath)
        {
            if (basePath == null || basePath.Length == 0)
            {
                throw new ArgumentNullException("basePath");
            }
            if (System.IO.Directory.Exists(basePath) == false)
            {
                throw new System.IO.DirectoryNotFoundException(basePath);
            }
            var subDirs = System.IO.Directory.GetDirectories(basePath);
            foreach (var res in reses)
            {
                
                //if (res.Name.IndexOf("LocalWebServer")>=0)
                //{

                //}
                //if( res.IsResources == false )
                //{
                //    continue;
                //}
                var rfn = res.RuntimeFileName;
                var fn = Path.Combine(basePath, rfn);
                if (File.Exists(fn))
                {
                    res.Data = File.ReadAllBytes(fn);
                    if (res.IsResources)
                    {
                        foreach (var subDir in subDirs)
                        {
                            var dir2 = Path.GetFileName(subDir);
                            var fn2 = Path.Combine(
                                subDir, 
                                Path.GetFileNameWithoutExtension(rfn) + "." + dir2 + EXT_Resources);
                            if (System.IO.File.Exists(fn2))
                            {
                                if (res.LocalDatas == null)
                                {
                                    res.LocalDatas = new SortedDictionary<string, byte[]>();
                                }
                                res.LocalDatas[dir2] = System.IO.File.ReadAllBytes(fn2);
                            }
                        }
                    }
                }
            }
        }

        public const string TagName_mresource = ".mresource";
        public DCILMResource()
        {

        }

        public DCILMResource(DCILDocument doc, DCILReader reader)
        {
            this.OwnerDocument = doc;
            var word = reader.ReadWord();
            if (word == "public")
            {
                this.IsPublic = true;
                this._Name = reader.ReadWord();
            }
            else if (word == "private")
            {
                this.IsPublic = false;
                this._Name = reader.ReadWord();
            }
            reader.MoveAfterChar('}');
            //reader.ReadAfterCharExcludeLastChar('}');
            this.IsResources = this._Name.EndsWith(EXT_Resources);
        }
        public override void Dispose()
        {
            base.Dispose();
            this.Data = null;
            if (this.LocalDatas != null)
            {
                this.LocalDatas.Clear();
                this.LocalDatas = null;
            }
            if (this._ResourceValues != null)
            {
                foreach (var item in this._ResourceValues)
                {
                    item.Value.Name = null;
                    item.Value.Value = null;
                }
                this._ResourceValues.Clear();
                this._ResourceValues = null;
            }
        }
        /// <summary>
        /// 对象类型
        /// </summary>
        public override DCILObjectType ObjectType
        {
            get
            {
                return DCILObjectType.Resource;
            }
        }
        private string RuntimeFileName
        {
            get
            {
                var rfn = this.Name;
                if (rfn == null || rfn.Length == 0)
                {
                    return null;
                }
                if (rfn[0] == '\'' && rfn.Length > 2 && rfn[rfn.Length - 1] == '\'')
                {
                    rfn = rfn.Substring(1, rfn.Length - 2);
                }
                return rfn;
            }
        }
        public bool Modified = false;
        public bool IsResources = false;

        public byte[] Data = null;
        public SortedDictionary<string, byte[]> LocalDatas = null;
        public bool HasBmpValue
        {
            get
            {
                foreach (var item in this.ResourceValues)
                {
                    if (item.Value.Value is System.Drawing.Bitmap)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private Dictionary<string, MResourceItem> _ResourceValues = null;
        public Dictionary<string, MResourceItem> ResourceValues
        {
            get
            {
                if (this._ResourceValues == null && this.Data != null && this.Data.Length > 0)
                {
                    this._ResourceValues = new Dictionary<string, MResourceItem>();
                    FillValues2(new System.IO.MemoryStream(this.Data), this._ResourceValues);
                }
                return this._ResourceValues;
            }
        }

        public bool ChangeLanguage(string language)
        {
            if (language == null || language.Length == 0)
            {
                return false;
            }
            if (this.Data == null || this.Data.Length == 0)
            {
                return false;
            }
            byte[] localData = null;
            if (this.LocalDatas == null || this.LocalDatas.TryGetValue(language, out localData) == false)
            {
                return false;
            }
            if (localData == null || localData.Length == 0)
            {
                return false;
            }
            this._ResourceValues = new Dictionary<string, MResourceItem>();
            FillValues2(new System.IO.MemoryStream(this.Data), this._ResourceValues);
            if (FillValues2(new System.IO.MemoryStream(localData), this._ResourceValues) > 0)
            {
                MyConsole.Instance.WriteLine("    Change resource language : " + this.Name);

                var ms = new System.IO.MemoryStream();
                using (var writer = new System.Resources.ResourceWriter(ms))
                {
                    var names = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in this._ResourceValues.Values)
                    {
                        if (names.ContainsKey(item.Name) == false)
                        {
                            if (item.Value is string)
                            {
                                writer.AddResource(item.Name, (string)item.Value);
                            }
                            else
                            {
                                writer.AddResourceData(item.Name, item.Type, item.Data);
                            }
                            names[item.Name] = null;
                        }
                        else
                        {
                            MyConsole.Instance.ForegroundColor = ConsoleColor.Red;
                            MyConsole.Instance.BackgroundColor = ConsoleColor.White;
                            MyConsole.Instance.WriteLine("       [Warring],Duplicate resource item name : " + this.Name + " # " + item.Key);
                            MyConsole.Instance.ResetColor();
                        }
                    }
                }
                this.Data = ms.ToArray();
                ms.Close();
                this.Modified = true;
                return true;
            }
            return false;
        }
#if DOTNETCORE
        private static int FillValues2(System.IO.Stream stream, Dictionary<string, MResourceItem> values)
        {
            int result = 0;
            using (var reader = new System.Resources.Extensions.DeserializingResourceReader(stream))
            {
                var enumer = reader.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string name = Convert.ToString(enumer.Key);
                    string resType = null;
                    byte[] itemData = null;
                    object enumValue = enumer.Value;
                    if( enumValue is string )
                    {
                        resType = null;
                        itemData = System.Text.Encoding.UTF8.GetBytes((string)enumValue);
                    }
                    else if( enumValue is System.Drawing.Bitmap )
                    {
                        resType = "System.Drawing.Bitmap, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";// typeof(System.Drawing.Bitmap).AssemblyQualifiedName;
                        var ms = new System.IO.MemoryStream();
                        _bf.Serialize(ms, enumValue);
                        itemData = ms.ToArray();
                        ms.Close();
                    }
                    if (itemData != null && itemData.Length > 0)
                    {
                        var item = new MResourceItem();
                        item.Name = name;
                        item.Type = resType;
                        item.Data = itemData;
                        item.Value = enumer.Value;
                        values[name] = item;
                        //values[name] = new Tuple<string, byte[]>(resType, itemData);
                        result++;
                    }
                }
            }
            return result;
        }
        private static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
#else
        private static int FillValues2(System.IO.Stream stream, Dictionary<string, MResourceItem> values)
        {
            int result = 0;
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                var enumer = reader.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string name = Convert.ToString(enumer.Key);
                    string resType = null;
                    byte[] itemData = null;
                    reader.GetResourceData(name, out resType, out itemData);
                    if (resType != null && resType.Length > 0 && itemData != null && itemData.Length > 0)
                    {
                        var item = new MResourceItem();
                        item.Name = name;
                        item.Type = resType;
                        item.Data = itemData;
                        item.Value = enumer.Value;
                        values[name] = item;
                        //values[name] = new Tuple<string, byte[]>(resType, itemData);
                        result++;
                    }
                }
            }
            return result;
        }
#endif

        public bool WriteDataFile(string basePath)
        {
            if (this.Data != null && this.Data.Length > 0)
            {
                var fn = Path.Combine(basePath, this.RuntimeFileName);
                File.WriteAllBytes(fn, this.Data);
                return true;
            }
            else
            {
                return false;
            }
        }
        public class MResourceItem
        {
            public string Name = null;
            public string Type = null;
            public object Value = null;
            public bool IsBmp
            {
                get
                {
                    return this.Value is System.Drawing.Bitmap;
                }
            }
            public byte[] Data = null;
            public int StartIndex = 0;
            public int BSLength = 0;
            public int Key = 0;
            public override string ToString()
            {
                return this.Name + " # " + this.Type;
            }
        }
        public byte[] EncryptData()
        {
            var items = this.ResourceValues.Values;
            var lstData = new List<byte>();
            var rnd = new System.Random();
            foreach (var item in items)
            {
                item.StartIndex = lstData.Count;
                item.Key = rnd.Next(1000, int.MaxValue - 1000);
                if (item.Value is string)
                {
                    string str = (string)item.Value;
                    int key = item.Key;
                    for (int iCount = 0; iCount < str.Length; iCount++, key++)
                    {
                        var v = str[iCount] ^ key;
                        lstData.Add((byte)(v >> 8));
                        lstData.Add((byte)(v & 0xff));
                    }
                }
                else if (item.Value is System.Drawing.Bitmap)
                {
                    var ms2 = new System.IO.MemoryStream();
                    ((System.Drawing.Bitmap)item.Value).Save(ms2, System.Drawing.Imaging.ImageFormat.Bmp);
                    var bs = ms2.ToArray();
                    ms2.Close();
                    var key = (byte)item.Key;
                    for (int iCount = 0; iCount < bs.Length; iCount++, key++)
                    {
                        lstData.Add((byte)(bs[iCount] ^ key));
                    }
                }
                else
                {
                    MyConsole.Instance.EnsureNewLine();
                    MyConsole.Instance.WriteError("    Not support resource item:" + this.Name + "|" + item.Name + "=" + item.Value.GetType().FullName);
                }
                item.BSLength = lstData.Count - item.StartIndex;
            }
            var result = lstData.ToArray();
            return result;
        }

        public override void WriteTo(DCILWriter writer)
        {
            writer.Write(".mresource ");
            if (this.IsPublic)
            {
                writer.Write(" public ");
            }
            else
            {
                writer.Write(" private ");
            }
            writer.Write(this._Name);
            writer.WriteStartGroup();
            writer.WriteEndGroup();
        }
        public override string ToString()
        {
            return "Resource " + this._Name;
        }

        public bool IsPublic = true;

    }
}
