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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;

namespace __DC20211119
{

    /// <summary>
    /// 内嵌的帮助类型，只有在.NET2.0和Release模式下编译后的IL代码才能使用。
    /// </summary>
    internal static class JIEJIEHelper
    {
        private sealed class SMF_ResStream : System.IO.Stream
        {
            public SMF_ResStream(byte[] bs)
            {
                int gzipLen = BitConverter.ToInt32(bs, 0);
                if(gzipLen == 0 )
                {
                    this._Content = new byte[bs.Length - 4];
                    Array.Copy(bs, 4, this._Content, 0, this._Content.Length);
                }
                else
                {
                    // 使用GZIP压缩了。
                    var msSource = new System.IO.MemoryStream(bs);
                    msSource.Position = 4;
                    var gm = new System.IO.Compression.GZipStream(msSource, System.IO.Compression.CompressionMode.Decompress);
                    this._Content = new byte[gzipLen];
                    gm.Read(this._Content, 0, gzipLen);
                    gm.Close();
                    msSource.Close();
                }
            }

            private byte[] _Content = null;

            public override bool CanRead
            {
                get
                {
                    return true;
                }
            }
            public override bool CanSeek
            {
                get
                {
                    return true;
                }
            }
            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }

            public override long Length
            {
                get
                {
                    return this._Content.Length;
                }
            }
            private long _Position = 0;
            public override long Position
            {
                get
                {
                    return this._Position;
                }
                set
                {
                    this._Position = value;
                }
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int len = (int)(this._Content.Length - this._Position);
                if( len > count )
                {
                    len = count;
                }
                if (len > 0)
                {
                    Array.Copy(this._Content, this._Position, buffer, offset, len);
                    int endIndex = offset + len;
                    for(int iCount = offset; iCount < endIndex; iCount ++)
                    {
                        buffer[iCount] = (byte)(buffer[iCount] ^ 123);
                    }
                    this._Position += len;
                }
                return len;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        this._Position = offset;
                        break;
                    case SeekOrigin.Current:
                        this._Position += offset;
                        break;
                    case SeekOrigin.End:
                        this._Position = this._Content.Length - offset;
                        break;
                }
                return this._Position;
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
        }


        private static readonly Dictionary<string, byte[]> __SMF_Contents = SMF_CreateEmptyTable();
        private static Dictionary<string, byte[]> SMF_CreateEmptyTable()
        {
            var table = new Dictionary<string, byte[]>();
            table["aaaaaaa"] = null;
            table["bbbbbbbbbbb"] = null;
            return table;
        }
        private static readonly System.Reflection.Assembly ThisAssembly = typeof(JIEJIEHelper).Assembly;
        
        //private static Dictionary<string, byte[]> SMF_GetContents()
        //{
        //    var result = __SMF_Contents;
        //    if( result == null )
        //    {
        //        result = new Dictionary<string, byte[]>();
        //        SMF_InitContent(result);
        //        __SMF_Contents = result;
        //    }
        //    return result;
        //}

        //private static void SMF_InitContent(Dictionary<string,byte[]> contents )
        //{
        //    //contents["aaa"] = BitConverter.GetBytes(20222);
        //}

        public static System.Reflection.ManifestResourceInfo SMF_GetManifestResourceInfo( System.Reflection.Assembly asm , string resourceName )
        {
            if (ThisAssembly.Equals(asm) && __SMF_Contents != null)
            {
                if (__SMF_Contents.ContainsKey( resourceName ))
                {
                    return new ManifestResourceInfo(asm, resourceName, ResourceLocation.Embedded);
                }
            }
            return asm.GetManifestResourceInfo(resourceName);
        }
        private static byte[] aaaa()
        {
            return null;
        }
        private static byte [] bbbbbbb()
        {
            return null;
        }
        private static byte[] SMF_GetContent( string name )
        {
            if(name == "1111111111")
            {
                return aaaa();
            }
            if(name == "22222222")
            {
                return bbbbbbb();
            }
            return null;
        }
        public static string[] SMF_GetManifestResourceNames(System.Reflection.Assembly asm)
        {
            if (ThisAssembly.Equals(asm) && __SMF_Contents != null)
            {
                var list = new List<string>(__SMF_Contents.Keys);
                var names2 = asm.GetManifestResourceNames();
                if (names2 != null)
                {
                    list.AddRange(names2);
                }
                return list.ToArray();
            }
            return asm.GetManifestResourceNames();
        }

        public static System.IO.Stream SMF_GetManifestResourceStream(System.Reflection.Assembly asm, string resourceName)
        {
            if (ThisAssembly.Equals(asm) && __SMF_Contents != null)
            {
                byte[] bsContent = null;
                if (__SMF_Contents.TryGetValue(resourceName, out bsContent))
                {
                    if (bsContent == null)
                    {
                        bsContent = SMF_GetContent(resourceName);
                        __SMF_Contents[resourceName] = bsContent;
                    }
                    return new SMF_ResStream(bsContent);
                }
            }
            return asm.GetManifestResourceStream(resourceName);
        }

        public static System.IO.Stream SMF_GetManifestResourceStream2(System.Reflection.Assembly asm, Type t , string resourceName)
        {
            if( resourceName == null || resourceName.Length == 0 )
            {
                throw new ArgumentNullException('r'.ToString());
            }
            if (object.ReferenceEquals(t, null))
            {
                throw new ArgumentNullException('t'.ToString());
            }
            if (ThisAssembly.Equals(asm))
            {
                return SMF_GetManifestResourceStream(asm, t.FullName + '.'.ToString() + resourceName);
            }
            return asm.GetManifestResourceStream(t , resourceName);
        }

        public static string String_Trim( string v )
        {
            return v.Trim();
        }

        public static Type Object_GetType(object a)
        {
            //int v = 99;
            //string v2 = Int32_ToString(ref v);
            return a.GetType();
        }

        public unsafe static void MyInitializeArray(Array v, RuntimeFieldHandle fldHandle, int encKey)
        {
            RuntimeHelpers.InitializeArray(v, fldHandle);
            if (v.Length < 4 || encKey == 0)
            {
                return;
            }
            fixed (byte* ptr = (byte[])v)
            {
                int* ptr2 = (int*)ptr;
                for (int* ptr3 = ptr2 + (int)Math.Floor((double)v.Length / 4.0) - 1; ptr3 >= ptr2; ptr3--)
                {
                    *ptr3 ^= encKey;
                    encKey += 13;
                }
            }
        }

        public static string Int32_ToString( ref int v)
        {
            return v.ToString();
        }
        public static bool String_Equality(string a, string b)
        {
            return string.Equals(a, b);
        }

        public static string String_ConcatObject(object a, object b)
        {
            return string.Concat(a, b);
        }
        public static string String_Concat3Object(object a, object b, object c)
        {
            return string.Concat(a, b, c);
        }
        public static string String_Concat3String(string a, string b, string c)
        {
            return string.Concat(a, b, c);
        }
        public static string Object_ToString(object v)
        {
            return v.ToString();
        }

        public static bool String_IsNullOrEmpty(string v)
        {
            if (v != null)
            {
                return v.Length == 0;
            }
            return true;
        }
        public static void Monitor_Enter(object a)
        {
            System.Threading.Monitor.Enter(a);
        }

        public static void Monitor_Enter2(object a , ref bool lockTaken)
        {
            System.Threading.Monitor.Enter(a ,ref lockTaken);
        }

        public static void Monitor_Exit(object a)
        {
            System.Threading.Monitor.Exit(a);
        }
        public static string String_Concat(string a, string b)
        {
            return a + b;
        }
        public static void MyDispose(IDisposable obj)
        {
            obj.Dispose();
        }

        private static volatile System.Threading.Thread _CloneStringCrossThead_Thread ;
        private static System.Threading.AutoResetEvent _CloneStringCrossThead_Event;
        private static System.Threading.AutoResetEvent _CloneStringCrossThead_Event_Inner;
        private static volatile string _CloneStringCrossThead_CurrentValue;
        private static readonly object _LockObject = null;
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
            try
            {
                System.Threading.Monitor.Enter(_LockObject);
                if( _CloneStringCrossThead_Event_Inner == null )
                {
                    _CloneStringCrossThead_Event_Inner = new System.Threading.AutoResetEvent(false);
                }
                if(_CloneStringCrossThead_Event == null )
                {
                    _CloneStringCrossThead_Event = new System.Threading.AutoResetEvent(false);
                }
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
            finally
            {
                System.Threading.Monitor.Exit(_LockObject);
            }
        }

        private static void CloneStringCrossThead_Thread()
        {
            try
            {
                while (true)
                {
                    if (_CloneStringCrossThead_Event_Inner != null 
                        && _CloneStringCrossThead_Event_Inner.WaitOne(1000) == false)
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
#if DOTNETCORE
        /// <summary>
        /// 从字节数组中加载资源数据
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="key"></param>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static System.Resources.ResourceSet LoadResourceSet2(byte[] bs, byte key, bool gzip)
        {
            var stream = GetStream(bs, key, gzip);
            var result = new System.Resources.ResourceSet(
                new System.Resources.Extensions.DeserializingResourceReader(stream));
            stream.Close();
            return result;
        }
#endif
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
    }

}