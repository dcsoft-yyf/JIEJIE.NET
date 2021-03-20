using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DCNETProtector
{
    internal static class DCUtils
    {
        //public static void ObfuseListOrder(System.Collections.IList strList)
        //{
        //    var rnd = new System.Random();
        //    var strListCount = strList.Count;
        //    for (int iCount = 0; iCount < strListCount; iCount++)
        //    {
        //        int index = rnd.Next(0, strListCount - 1);
        //        var temp = strList[iCount];
        //        strList[iCount] = strList[index];
        //        strList[index] = temp;
        //    }
        //}

        public static string SearchFileDeeply(string rootDir, string fileName)
        {
            if (File.Exists(Path.Combine(rootDir, fileName)))
            {
                return Path.Combine(rootDir, fileName);
            }
            foreach (var dir in Directory.GetDirectories(rootDir))
            {
                var result = SearchFileDeeply(dir, fileName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        public static int CleanDirectory(string rootDir)
        {
            int result = 0;
            if (Directory.Exists(rootDir))
            {
                foreach (var fn in Directory.GetFiles(rootDir))
                {
                    File.SetAttributes(fn, FileAttributes.Normal);
                    File.Delete(fn);
                    result++;
                }
                foreach (var dir in Directory.GetDirectories(rootDir))
                {
                    result += CleanDirectory(dir);
                    Directory.Delete(dir);
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// 格式化输出字节大小数据
        /// </summary>
        /// <param name="ByteSize">字节数</param>
        /// <returns>输出的字符串</returns>
        public static string FormatByteSize(long byteSize)
        {
            const long _PBSIZE = (long)1024 * 1024 * 1024 * 1024;
            const long _GBSIZE = 1024 * 1024 * 1024;
            const long _MBSIZE = 1024 * 1024;
            const long _KBSIZE = 1024;

            long v = byteSize;
            string unit = null;
            if (byteSize > _PBSIZE)
            {
                v = v * 100 / _PBSIZE;
                unit = "PB";
            }
            else if (byteSize > _GBSIZE)
            {
                v = v * 100 / _GBSIZE;
                unit = "GB";
            }
            else if (byteSize > _MBSIZE)
            {
                v = v * 100 / _MBSIZE;
                unit = "MB";
            }
            else if (byteSize > _KBSIZE)
            {
                v = v * 100 / _KBSIZE;
                unit = "KB";
            }
            else
            {
                return byteSize.ToString() + "B";
            }
            int mod = (int)(v % 100);
            v = v / 100;
            if (v > 10)
            {
                mod = mod / 10;
            }
            if (mod == 0)
            {
                return v.ToString() + unit;
            }
            else
            {
                return v.ToString() + '.' + mod.ToString() + unit;
            }
        }

        internal static bool CheckFileWriteable(string fileName, bool throwException = true)
        {
            if (fileName == null || fileName.Length == 0)
            {
                throw new ArgumentNullException("fileName");
            }
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException(fileName);
            }
            if (throwException)
            {
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    var bs = new byte[1024];
                    int len = stream.Read(bs, 0, bs.Length);
                    if (len > 0)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.Write(bs, 0, len);
                    }
                }
                return true;
            }
            else
            {
                try
                {
                    using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        var bs = new byte[1024];
                        int len = stream.Read(bs, 0, bs.Length);
                        if (len > 0)
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.Write(bs, 0, len);
                        }
                    }
                    return true;
                }
                catch (System.Exception ext)
                {
                    System.Diagnostics.Debug.WriteLine(fileName + ":" + ext.Message);
                }
            }
            return false;
        }

        public static void ObfuseListOrder(System.Collections.IList list)
        {
            var rnd = new System.Random();
            var strListCount = list.Count;
            for (int iCount = 0; iCount < strListCount; iCount++)
            {
                int index = rnd.Next(0, strListCount - 1);
                var temp = list[iCount];
                list[iCount] = list[index];
                list[index] = temp;
            }
        }

        private static readonly Dictionary<string, string> _AllocID_Values = new Dictionary<string, string>();
        private static readonly Random _Random = new Random();
        private static readonly string _IDChars = "0123456789abcdefghijklmnopqrstuvwxyz";
        public static string AllocID(int length = 6)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length=" + length);
            }
            var chrs = new char[length];
            while (true)
            {
                for (int iCount = 0; iCount < length; iCount++)
                {
                    chrs[iCount] = _IDChars[_Random.Next(0, _IDChars.Length - 1)];
                }
                string id = new string(chrs);
                if (_AllocID_Values.ContainsKey(id) == false)
                {
                    _AllocID_Values[id] = id;
                    return id;
                }
            }
        }
    }
}
