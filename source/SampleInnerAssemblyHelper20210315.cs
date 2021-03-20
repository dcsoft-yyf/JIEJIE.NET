using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DCSoft.Common
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal static class InnerAssemblyHelper20210315
    {
        private static volatile System.Threading.Thread _CloneStringCrossThead_Thread = null;
        private static readonly System.Threading.AutoResetEvent _CloneStringCrossThead_Event = new System.Threading.AutoResetEvent(false);
        private static readonly System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner = new System.Threading.AutoResetEvent(false);
        private static volatile string _CloneStringCrossThead_CurrentValue = null;
        /// <summary>
        /// 跨线程的复制字符串值，用于改变创建字符串的调用堆栈
        /// </summary>
        /// <param name="txt">字符串值</param>
        /// <returns>复制品</returns>
        public static string CloneStringCrossThead(string txt)
        {
            if (txt == null || txt.Length == 0)
            {
                return txt;
            }
            lock (_CloneStringCrossThead_Event)
            {
                _CloneStringCrossThead_CurrentValue = txt;
                _CloneStringCrossThead_Event_Inner.Set();
                _CloneStringCrossThead_Event.Reset();
                if (_CloneStringCrossThead_Thread == null)
                {
                    _CloneStringCrossThead_Thread = new System.Threading.Thread(CloneStringCrossThead_Thread);
                    _CloneStringCrossThead_Thread.Start();
                }
                _CloneStringCrossThead_Event.WaitOne(100);
                return _CloneStringCrossThead_CurrentValue;
            }
        }
        private static void CloneStringCrossThead_Thread()
        {
            try
            {
                while (true)
                {
                    if (_CloneStringCrossThead_Event_Inner.WaitOne(1000) == false)
                    {
                        // 等了1秒没任务了，退出线程。
                        break;
                    }

                    if (_CloneStringCrossThead_CurrentValue != null)
                    {
                        _CloneStringCrossThead_CurrentValue = new string(_CloneStringCrossThead_CurrentValue.ToCharArray());
                    }
                    _CloneStringCrossThead_Event.Set();
                    _CloneStringCrossThead_Event_Inner.Reset();
                }
            }
            finally
            {
                _CloneStringCrossThead_Thread = null;
                _CloneStringCrossThead_Event.Reset();
                _CloneStringCrossThead_Event_Inner.Reset();
            }
        }

        public static string GetString(byte[] bsData, int startIndex, int bsLength, int key)
        {
            int chrsLength = bsLength / 2;
            char[] chrs = new char[chrsLength];
            for (int iCount = 0; iCount < chrsLength; iCount++, key++)
            {
                int bi = startIndex + iCount * 2;
                int v = ((bsData[bi] << 8) + bsData[bi + 1]);
                v = v ^ key;
                chrs[iCount] = (char)v;
            }
            return new string(chrs);
        }
        public static System.Drawing.Bitmap GetBitmap(byte[] bsData, int startIndex, int bsLength, int key)
        {
            var bs = new byte[bsLength];
            for (int iCount = 0; iCount < bsLength; iCount++, key++)
            {
                bs[iCount] = (byte)(bsData[startIndex + iCount] ^ key);
            }
            var ms = new System.IO.MemoryStream(bs);
            var bmp = new System.Drawing.Bitmap(ms);
            return bmp;
        }

        //public static System.Drawing.Bitmap LoadBmp(byte[] bs, byte key, bool gzip)
        //{
        //    var stream = GetStream(bs, key, gzip);
        //    var bmp = new System.Drawing.Bitmap(stream);
        //    return bmp;
        //}

        /// <summary>
        /// 从字节数组中加载资源数据
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="key"></param>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static System.Resources.ResourceSet LoadResourceSet(byte[] bs, byte key, bool gzip)
        {
            var stream = GetStream(bs, key, gzip);
            var result = new System.Resources.ResourceSet(stream);
            stream.Close();
            return result;
        }
        private static System.IO.Stream GetStream(byte[] bs, byte key, bool gzip)
        {
            int len = bs.Length;
            for (int iCount = 0; iCount < len; iCount++, key++)
            {
                bs[iCount] = ((byte)(bs[iCount] ^ key));
            }
            System.IO.MemoryStream ms = null;
            if (gzip)
            {
                var stream = new System.IO.Compression.GZipStream(
                    new System.IO.MemoryStream(bs),
                    System.IO.Compression.CompressionMode.Decompress);
                ms = new System.IO.MemoryStream();
                var bsTemp = new byte[1024];
                while (true)
                {
                    len = stream.Read(bsTemp, 0, bsTemp.Length);
                    if (len > 0)
                    {
                        ms.Write(bsTemp, 0, len);
                    }
                    else
                    {
                        break;
                    }
                }
                stream.Close();
                ms.Position = 0;
            }
            else
            {
                ms = new System.IO.MemoryStream(bs);
            }
            return ms;
        }

        //private static readonly Random _Random = new Random();
        //public static byte[] GetBytes(byte[] bs, ref byte key, ref bool gzip)
        //{
        //    if (bs == null || bs.Length < 1000)
        //    {
        //        return bs;
        //    }
        //    var ms = new System.IO.MemoryStream();
        //    var stream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress);
        //    stream.Write(bs, 0, bs.Length);
        //    stream.Close();
        //    var bsResult = ms.ToArray();
        //    var rate = (double)bs.Length / (double)bsResult.Length;
        //    if (bs.Length - bsResult.Length > 512 && rate > 2)
        //    {
        //        // 压缩造成的效益足够大
        //        gzip = true;
        //    }
        //    else
        //    {
        //        // 压缩造成的效益不够大
        //        bsResult = bs;
        //        gzip = false;
        //    }
        //    key = (byte)(_Random.Next(100, 234));
        //    byte key2 = key;
        //    for (int iCount = 0; iCount < bs.Length; iCount++, key2++)
        //    {
        //        bs[iCount] = (byte)(bs[iCount] ^ key2);
        //    }
        //    return bsResult;
        //}
    }
}
