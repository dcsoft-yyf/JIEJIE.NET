using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.IO;

namespace DCNETProtector
{
    internal static class ResourceFileHelper
    {

#if DOTNETCORE
        static void Main(string[] args)
        {
            ___Main( args );
        }
#endif

        public static string[] ParseCommandLineArgs(string line)
        {
            if(line == null || line.Length == 0 )
            {
                return new string[0];
            }
            var list = new List<string>();
            StringBuilder args = new StringBuilder ();
            bool hasStartFlag = false;
            var len = line.Length;
            for (int iCount = 0; iCount < len; iCount++)
            {
                char c = line[iCount];
                bool completeItem = false;
                if( char.IsWhiteSpace( c ))
                {
                    if( hasStartFlag )
                    {
                        args.Append(c);
                    }
                    else
                    {
                        completeItem = true;
                    }
                }
                else
                {
                    if ( c == '"')
                    {
                        if( args.Length == 0 )
                        {
                            hasStartFlag = true;
                        }
                        else if( hasStartFlag )
                        {
                            hasStartFlag = false;
                            completeItem = true;
                        }
                    }
                    args.Append(c);
                    if(completeItem == false && iCount == len -1 )
                    {
                        completeItem = true;
                    }
                }
                if (completeItem)
                {
                    // 结束一个命令
                    if (args != null && args.Length > 0 )
                    {
                        string txt = args.ToString();
                        if (txt[0] == '"')
                        {
                            if (txt[txt.Length - 1] == '"')
                            {
                                txt = txt.Substring(1, txt.Length - 2);
                            }
                            else
                            {
                                txt = txt.Substring(1);
                            }
                        }
                        list.Add(txt);
                        args = new StringBuilder ();
                    }
                }
            }
            //if( args != null && args.Length > 0 )
            //{
            //    string txt = args.ToString();
            //    if (txt[0] == '"')
            //    {
            //        if (txt[txt.Length - 1] == '"')
            //        {
            //            txt = txt.Substring(1, txt.Length - 2);
            //        }
            //        else
            //        {
            //            txt = txt.Substring(1);
            //        }
            //    }
            //    list.Add(txt);
            //}
            return list.ToArray();
        }

        private static void ___Main(string[] args)
        {
            string rootPath = null;
            string languageName = null;
            string[] containerClassNames = null;
            foreach (var arg in args)
            {
                int index = arg.IndexOf('=');
                if (index > 0)
                {
                    var argName = arg.Substring(0, index).ToLower();
                    var argValue = arg.Substring(index + 1);
                    switch (argName)
                    {
                        case "rootpath": rootPath = argValue; break;
                        case "language": languageName = argValue; break;
                        case "containerclassnames": containerClassNames = argValue.Split(','); break;
                    }
                }
            }
            var result = Execute(rootPath, languageName, containerClassNames);
            if (result != null)
            {
                result.SaveToStdFileName(rootPath);
            }
        }

        public static MyResourceDataFileList ExecuteByExe(string exeFileName, string rootPath, string languageName, System.Collections.IList containerClassNames)
        {
            var args = new StringBuilder();
            args.Append("\"rootpath="  + rootPath + "\"" );
            if( languageName != null && languageName.Length > 0 )
            {
                args.Append(" language=" + languageName);
            }
            if( containerClassNames != null && containerClassNames.Count > 0 )
            {
                args.Append(" \"containerclassnames=");
                for( int iCount = 0; iCount < containerClassNames.Count; iCount ++ )
                {
                    args.Append(containerClassNames[iCount] );
                    if( iCount != containerClassNames.Count -1)
                    {
                        args.Append(',');
                    }
                }
                args.Append('"');
            }
            //___Main(ParseCommandLineArgs(args.ToString()));
            RunExe(exeFileName, args.ToString());
            var result = new MyResourceDataFileList();
            if( result.LoadFromStdFileName(rootPath))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static void RunExe(string exeFileName, string argument)
        {
            var pstart = new System.Diagnostics.ProcessStartInfo();
            pstart.FileName = exeFileName;
            pstart.Arguments = argument;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(">>Execute command line:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\"" + pstart.FileName + "\" " + pstart.Arguments);
            System.Diagnostics.Debug.WriteLine(">>Execute command line: \"" + pstart.FileName + "\" " + pstart.Arguments);
            Console.ResetColor();
            pstart.UseShellExecute = false;
            //pstart.CreateNoWindow = false;
            var p = System.Diagnostics.Process.Start(pstart);
            p.WaitForExit();
        }

       

        public static bool ExpendResouceToFile(string resName, string fileName, bool overWrite)
        {
            if( overWrite == false && File.Exists( fileName ))
            {
                return true ;
            }
            var ms = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream(resName);
            if (ms != null)
            {
                using (var stream = new System.IO.FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    var bsTemp = new byte[1024];
                    while( true )
                    {
                        int len = ms.Read(bsTemp, 0, bsTemp.Length);
                        if( len > 0 )
                        {
                            stream.Write(bsTemp, 0, len);
                        }
                        else
                        {
                            break;
                        }
                    }//while
                }//using
                ms.Close();
                return true;
            }
            return false;
        }

        public static MyResourceDataFileList Execute( string rootPath , string language , System.Collections.IList containerClassNames )
        {  
            if (rootPath == null || rootPath.Length == 0 || Directory.Exists(rootPath) == false)
            {
                return null ;
            }
            if (language != null && language.Length > 0)
            {
                var fns = Directory.GetFiles(rootPath, "*.resources");
                foreach (var fn in fns)
                {
                    Console.WriteLine("Merge resource file:" + fn);
                    CombineResourceFile(fn, language, fn);
                }
            }
            if (containerClassNames != null && containerClassNames.Count > 0)
            {
                var values = new MyResourceDataFileList();
                foreach (var name in containerClassNames)
                {
                    var fn = Path.Combine(rootPath, name + EXT_resources);
                    if (File.Exists(fn))
                    {
                        values.Add(new MyResourceDataFile(fn));
                    }
                }
                //values.SaveToStdFileName(rootPath);
                return values;
            }
            return null;
        }

        public static readonly string EXT_resources = ".resources";


       

        public static bool CombineResourceFile(string fileName, string language, string outputFileName )
        {
            if (language == null || language.Length == 0)
            {
                return false;
            }
            if (fileName == null || fileName.Length == 0)
            {
                throw new ArgumentNullException("fileName");
            }
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException(fileName);
            }
            var values = new Dictionary<string, System.Tuple<string, byte[]>>();
            FillValues2(fileName, values);
            var rootPath = Path.GetDirectoryName(fileName);
            var fn2 = Path.Combine(
                Path.Combine(rootPath, language),
                Path.GetFileNameWithoutExtension(fileName) + "." + language + EXT_resources);
            if (File.Exists(fn2))
            {
                if (FillValues2(fn2, values) > 0)
                {
                    if (outputFileName == null || outputFileName.Length == 0)
                    {
                        outputFileName = fileName;
                    }

                    using (var writer = new System.Resources.ResourceWriter(outputFileName))
                    {
                        var names = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        foreach (var item in values)
                        {
                            if (names.ContainsKey(item.Key) == false)
                            {
                                writer.AddResourceData(item.Key, item.Value.Item1, item.Value.Item2);
                                names[item.Key] = null;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine("Same name resource item:" + fileName + " # " + item.Key);
                                Console.ResetColor();
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
#if DOTNETCORE
        public static int FillValues2(string fileName, Dictionary<string, System.Tuple<string, byte[]>> values)
        {
            int result = 0;
            using (var reader = new System.Resources.Extensions.DeserializingResourceReader(fileName))
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
                    if (resType != null && resType.Length > 0 && itemData != null && itemData.Length > 0)
                    {
                        values[name] = new Tuple<string, byte[]>(resType, itemData);
                        result++;
                    }
                }
            }
            return result;
        }
        private static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
#else
        public static int FillValues2(string fileName, Dictionary<string, System.Tuple<string, byte[]>> values)
        {
            int result = 0;
            using (var reader = new System.Resources.ResourceReader(fileName))
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
                        values[name] = new Tuple<string, byte[]>(resType, itemData);
                        result++;
                    }
                }
            }
            return result;
        }
#endif
    }

    internal class MyResourceDataFileList : List<MyResourceDataFile>
    {
        public static readonly string StdFileName = "dcsoft20210306.resouces.dat";
        public MyResourceDataFileList( )
        {

        }
        public MyResourceDataFileList(List<string> fileNames)
        {
            foreach (var fn in fileNames)
            {
                this.Add(new MyResourceDataFile(fn));
            }
        }
        public bool LoadFromStdFileName( string rootPath )
        {
            var fn = Path.Combine(rootPath, StdFileName);
            if( File.Exists( fn ))
            {
                Load(fn);
                return true;
            }
            return false;
        }
        public void SaveToStdFileName( string rootPath )
        {
            var fn = Path.Combine(rootPath, StdFileName);
            Save(fn);
        }
        public void Load(string fileName)
        {
            using (var reader = new System.IO.BinaryReader(
                new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read)))
            {
                int fileCount = reader.ReadInt32();
                for( int fileIndex = 0; fileIndex < fileCount; fileIndex ++ )
                {
                    var dataFile = new MyResourceDataFile();
                    dataFile.FileName = reader.ReadString();
                    dataFile.Name = reader.ReadString();
                    var bsLength = reader.ReadInt32();
                    if( bsLength > 0 )
                    {
                        dataFile.Datas = reader.ReadBytes(bsLength);
                    }
                    var itemCount = reader.ReadInt32();
                    for( int iCount = 0; iCount < itemCount; iCount ++ )
                    {
                        var newItem = new MyResourceDataFile.MyResourceDataItem();
                        newItem.Name = reader.ReadString();
                        newItem.StartIndex = reader.ReadInt32();
                        newItem.BsLength = reader.ReadInt32();
                        newItem.Key = reader.ReadInt32();
                        newItem.IsBmp = reader.ReadBoolean();
                        dataFile.Items.Add(newItem);
                    }
                    this.Add(dataFile);
                }
            }
        }
        public void Save(string fileName)
        {
            using (var writer = new System.IO.BinaryWriter(
                new System.IO.FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)))
            {
                writer.Write(this.Count);
                foreach (var dataFile in this)
                {
                    writer.Write(dataFile.FileName);
                    writer.Write(dataFile.Name);
                    if (dataFile.Datas == null || dataFile.Datas.Length == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(dataFile.Datas.Length);
                        writer.Write(dataFile.Datas, 0, dataFile.Datas.Length);
                    }
                    if (dataFile.Items == null || dataFile.Items.Count == 0)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(dataFile.Items.Count);
                        foreach (var item in dataFile.Items)
                        {
                            writer.Write(item.Name);
                            writer.Write(item.StartIndex);
                            writer.Write(item.BsLength);
                            writer.Write(item.Key);
                            writer.Write(item.IsBmp);
                        }
                    }
                }
            }
        }
    }

    internal class MyResourceDataFile
    {
        internal class MyResourceDataItem
        {
            public string Name = null;
            public int StartIndex = 0;
            public int BsLength = 0;
            public int Key = 0;
            public bool IsBmp = false;
            public object Value = null;
            public override string ToString()
            {
                return this.Name + ":" + this.Value;
            }
        }
        public override string ToString()
        {
            return this.Name;
        }
        public string Name = null;
        public string FileName = null;
        public List<MyResourceDataItem> Items = new List<MyResourceDataItem>();
        public byte[] Datas = null;
        public bool HasBmp
        {
            get
            {
                foreach( var item in this.Items )
                {
                    if( item.IsBmp )
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public MyResourceDataFile()
        {

        }
        private static readonly Random _Random = new Random();
        public MyResourceDataFile(string fileName)
        {
#if DOTNETCORE
            var reader = new System.Resources.Extensions.DeserializingResourceReader(fileName);
#else
            var reader = new System.Resources.ResourceReader(fileName);
#endif

            this.FileName = fileName;
            this.Name = Path.GetFileNameWithoutExtension(fileName);
            var objValues = new Dictionary<string, object>();
            var strValues = new Dictionary<string, string>();
            var resouceSet = new System.Resources.ResourceSet( reader );
            var enumer = resouceSet.GetEnumerator();
            var lstData = new List<byte>();
            bool hasBmpValue = false;
            while (enumer.MoveNext())
            {
                string itemName = (string)enumer.Key;
                var item = new MyResourceDataItem();
                item.Name = itemName;
                item.StartIndex = lstData.Count;
                item.Key = _Random.Next(1000, int.MaxValue - 1000);
                item.Value = enumer.Value;
                if (enumer.Value is string)
                {
                    item.IsBmp = false;
                    string str = (string)item.Value;
                    int key = item.Key;
                    for (int iCount = 0; iCount < str.Length; iCount++, key++)
                    {
                        var v = str[iCount] ^ key;
                        lstData.Add((byte)(v >> 8));
                        lstData.Add((byte)(v & 0xff));
                    }
                }
                else if (enumer.Value is System.Drawing.Bitmap)
                {
                    item.IsBmp = true;
                    hasBmpValue = true;
                    var ms = new System.IO.MemoryStream();
                    ((System.Drawing.Bitmap)item.Value).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    var bs2 = ms.ToArray();
                    byte key = (byte)item.Key;
                    for (int iCount = 0; iCount < bs2.Length; iCount++, key++)
                    {
                        bs2[iCount] = (byte)(bs2[iCount] ^ key);
                    }
                    lstData.AddRange(bs2);
                }
                else
                {
                    throw new NotSupportedException(item.Value.GetType().FullName);
                }
                item.BsLength = lstData.Count - item.StartIndex;
                this.Items.Add(item);
            }
            resouceSet.Close();
            this.Datas = lstData.ToArray();
        }
    }

}
