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


namespace JIEJIE
{
    /// <summary>
    /// 测试JIEJIEPerformanceCounter使用的代码
    /// </summary>
    public static class SelfPerformanceCounterForTest
    {
        public enum MethodIndexs
        {
            ParseCommandLines = 0,
            ProjectRun,
            BlazorWebAssembly,
            SaveAssemblyFile,
            WriteILFile,
            WriteIL,
            RunExe,
            WriteMapXml,
            LoadAssemblyFiles,
            LoadOneAssemblyFile,
            LoadByReader,
            LoadClassHeader,
            LoadClassContent,
            LoadILData,
            DocumentFixDomState,
            MergeDocuments,
            HandleDocument,
            Rename,
            EncryptStringValues,
            AddDatasClass,
            ObfuseListOrder,
            RemoveMember
        }
        public static void Start()
        {
            var fs = typeof(MethodIndexs).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var methods = new string[fs.Length];
            foreach(var f in fs)
            {
                methods[(int)f.GetValue(null)] = f.Name;
            }
            __DC20211119.JIEJIEPerformanceCounter.SetMethodNames(methods);
            __DC20211119.JIEJIEPerformanceCounter.Start(true);
        }
        public static void Stop()
        {
            __DC20211119.JIEJIEPerformanceCounter.Stop();
            string txt = __DC20211119.JIEJIEPerformanceCounter.AnalyseSimple(2,0);
            MyConsole.Instance.WriteLine("--------- Performace analyse result: -------");
            MyConsole.Instance.WriteLine(txt);
            MyConsole.Instance.WriteLine("Press Enter to continue.");
            MyConsole.Instance.ReadLine();
        }
        public static int Enter(MethodIndexs mi )
        {
            return __DC20211119.JIEJIEPerformanceCounter.Enter((int)mi);
        }
        public static void Leave(int handle )
        {
            __DC20211119.JIEJIEPerformanceCounter.Leave(handle);
        }
    }
}

namespace __DC20211119
{
    public static class JIEJIEPerformanceCounter
    {
        //static JIEJIEPerformanceCounter()
        //{
        //    SetMethodNames(new string[] { "aaa.bb()","aaa.zz()"});
        //    //_Methods = new PerformanceMethod[10];
        //    //var index = 0;
        //    //SetMethod(index++, "aaa.bb()");
        //    //SetMethod(index++, "aaa.zz()");
        //}
        public static void PublicStart()
        {
            SetMethodNames(new string[] { "%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%","bbb"});
            Start(true);
        }

        public static void SetMethod( int index , string methodName )
        {
            _Methods[index] = new PerformanceMethod(index, methodName);
        }
        public static void SetMethodNames( string[] methodNames )
        {
            _Methods = new PerformanceMethod[methodNames.Length];
            for(var iCount = 0;iCount < methodNames.Length; iCount ++)
            {
                _Methods[iCount] = new PerformanceMethod(iCount, methodNames[iCount]);
            }
        }
        public static void Start( bool fastMode )
        {
            _FastMode = fastMode;
            foreach(var item in _Methods )
            {
                item.ChildTickCount = 0;
                item.ChildTicks.Clear();
                item.TickCount = 0;
            }
            _CallStackPosition = 0;
            _Enabled = true;
            _GlobalStartTick = GetCurrentTick();
        }

        private static long _StartTimeTick = 0;
        private static int GetCurrentTick()
        {
            if(_StartTimeTick == 0)
            {
                _StartTimeTick = DateTime.Now.Ticks;
                return 0;
            }
            return (int)(DateTime.Now.Ticks - _StartTimeTick);
        }

        private static int _GlobalStartTick = GetCurrentTick();
        private static int _GlobalEndTick = 0;
        private static int _GlobalTickSpanFix = 0;
        private static int GlobalTickSpan()
        {
            return _GlobalTickSpanFix + _GlobalStartTick - _GlobalEndTick;
        }
        //private const int _FileHeaderFlag = 534324532;
        //public static void ReadRecords( System.IO.Stream stream )
        //{
        //    if( stream == null )
        //    {
        //        throw new ArgumentNullException("stream");
        //    }
        //    var reader = new System.IO.BinaryReader(stream, System.Text.Encoding.UTF8);
        //    if(reader.ReadInt32() != _FileHeaderFlag )
        //    {
        //        throw new InvalidOperationException("文件头不对");
        //    }
        //    _FastMode = reader.ReadBoolean();
        //    var globalTickSpan = reader.ReadInt32();
        //    var recordLength = reader.ReadInt32();
        //    _Methods = new PerformanceMethod[recordLength];
        //    for(var iCount = 0;iCount < recordLength;iCount ++ )
        //    {
        //        var record = new PerformanceMethod(iCount,reader.ReadString());
        //        record.TickCount = reader.ReadInt32();
        //        if(_FastMode)
        //        {
        //            record.ChildTickCount = reader.ReadInt32();
        //        }
        //        else
        //        {
        //            if( record.ChildTicks == null )
        //            {
        //                record.ChildTicks = new Dictionary<int, int>();
        //            }
        //            var childNum = reader.ReadInt16();
        //            for(var cc = 0; cc <childNum; cc ++)
        //            {
        //                var i2 = reader.ReadInt16();
        //                var t2 = reader.ReadInt32();
        //                record.ChildTicks[i2] = t2;
        //            }
        //        }
        //    }
        //}
        //public static void WriteTo( System.IO.Stream stream )
        //{
        //    if( stream == null )
        //    {
        //        throw new ArgumentNullException("stream");
        //    }
        //    var writer = new System.IO.BinaryWriter(stream, System.Text.Encoding.UTF8);
        //    writer.Write(_FileHeaderFlag);
        //    writer.Write(_FastMode);
        //    writer.Write((int)(GetCurrentTick() - _GlobalStartTick));
        //    writer.Write(_Methods.Length);
        //    foreach( var record in _Methods)
        //    {
        //        writer.Write(record.MethodName);
        //        writer.Write(record.TickCount);
        //        if( _FastMode )
        //        {
        //            writer.Write(record.ChildTickCount);
        //        }
        //        else if( record.ChildTicks == null || record.ChildTicks.Count == 0 )
        //        {
        //            writer.Write((short)0);
        //        }
        //        else
        //        {
        //            writer.Write((short)record.ChildTicks.Count);
        //            foreach( var item in record.ChildTicks)
        //            {
        //                writer.Write((short)item.Key);
        //                writer.Write(item.Value);
        //            }
        //        }
        //    }
        //}

        public static string AnalyseSimple(int sortType ,int minMilliseconds )
        {
            if(_Enabled)
            {
                Stop();
            }
            if( _FastMode == false )
            {
                return "Only support fast mode now.";
            }
            var strResult = new System.Text.StringBuilder();
            var globalTick = GlobalTickSpan() / 10000 ;
            strResult.Append("Program total span " + globalTick + " milliseconds,");
            var list = new List<PerformanceMethod>();
            minMilliseconds *= 10000;
            foreach(var item in _Methods)
            {
                if(item.TickCount > minMilliseconds )
                {
                    list.Add(item);
                }
            }
            list.Sort(new PerformanceMethodComparer(sortType));
            if (_FastMode)
            {
                strResult.AppendLine(" list order by column " + Math.Min( sortType + 1 , 4 ) + ":");
                strResult.AppendLine("     Total  |  Private |  Child  |   Call  |  Name");
                foreach (var item in list)
                {
                    AppendTick(strResult, item.TickCount, true);
                    AppendTick(strResult, item.TickCount - item.ChildTickCount, true);
                    AppendTick(strResult, item.ChildTickCount, true);
                    AppendTick(strResult, item.CallCount, false);
                    strResult.Append("      ");
                    strResult.AppendLine(item.MethodName);
                }
            }
            return strResult.ToString();
        }
        private static void AppendTick( System.Text.StringBuilder str , int num ,bool isTick )
        {
            var txt = isTick ? ((int)(num / 10000)).ToString() : num.ToString();
            str.Append(' ', 10 - txt.Length);
            str.Append(txt);
        }
        private static bool _Enabled = false ;
        private static bool _FastMode = true;
        private class PerformanceMethodComparer :IComparer<PerformanceMethod>
        {
            public PerformanceMethodComparer(int type )
            {
                this._Type = type;
            }
            private int _Type = 0;
            public int Compare( PerformanceMethod p1 , PerformanceMethod p2 )
            {
                if (this._Type == 0)
                {
                    return p2.TickCount - p1.TickCount;
                }
                else if (this._Type == 1)
                {
                    return (p2.TickCount - p2.ChildTickCount) - (p1.TickCount - p1.ChildTickCount);
                }
                else if (this._Type == 2)
                {
                    return p2.ChildTickCount - p1.ChildTickCount;
                }
                else
                {
                    return p2.CallCount - p1.CallCount;
                }
            }
        }
        private class PerformanceMethod
        {
            public PerformanceMethod(int intMethodIndex, string name )
            {
                if( name == null || name.Length == 0 )
                {
                    throw new ArgumentNullException("name");
                }
                this.MethodName = name;
                this.MethodIndex = intMethodIndex;
            }
            public readonly int MethodIndex;
            public readonly string MethodName ;
            public int TickCount ;
            public int CallCount;
            public int ChildTickCount;
            public Dictionary<int, int> ChildTicks = new Dictionary<int, int>();
            /// <summary>
            /// 添加被调用的函数的耗时
            /// </summary>
            /// <param name="intMethodIndex"></param>
            /// <param name="tick"></param>
            public void AddChildTick( int intMethodIndex , int tick )
            {
                if (this.MethodIndex == intMethodIndex)
                {
                    // 递归调用了自己
                    this.TickCount += tick;
                }
                else
                {
                    this.ChildTickCount += tick;
                    if (_FastMode == false)
                    {
                        int v = 0;
                        if (this.ChildTicks.TryGetValue(intMethodIndex, out v))
                        {
                            this.ChildTicks[intMethodIndex] = v + tick;
                        }
                        else
                        {
                            this.ChildTicks[intMethodIndex] = tick;
                        }
                    }
                }
            }
        }

        private static PerformanceMethod[] _Methods = null;
        //private struct CallRecord
        //{
        //    public int MethodIndex ;
        //    public long StartTick ;
        //}
        [ThreadStatic]
        private static int[] _CallStack = null;
        [ThreadStatic]
        private static int _CallStackLength = 0;
        [ThreadStatic]
        private static int _CallStackPosition = -1; 
        /// <summary>
        /// 进入一个函数
        /// </summary>
        /// <param name="intMethodIndex">函数编号</param>
        /// <returns>句柄,该返回值用于调用Leave()</returns>
        public static int Enter( int intMethodIndex )
        {
            if( _Enabled == false )
            {
                return -1;
            }
            var index = _CallStackPosition = _CallStackPosition + 2;
            if(_CallStack == null )
            {
                _CallStack = new int[20];
                _CallStackPosition = 0;
                index = 0;
                _CallStackLength = _CallStack.Length;
            }
            else if( index == _CallStackLength)
            {
                // 扩大缓存区
                var temp = new int[index << 1];
                Array.Copy(_CallStack, temp, _CallStackLength);
                _CallStack = temp;
                _CallStackLength = _CallStack.Length;
            }
            _CallStack[index] = intMethodIndex;
            _CallStack[index+1] = GetCurrentTick();
            _Methods[intMethodIndex].CallCount++;
            return index;
        }
        /// <summary>
        /// 离开一个函数
        /// </summary>
        /// <param name="intHandle">句柄</param>
        public static void Leave(int intHandle)
        {
            if (_Enabled == false || intHandle < 0)
            {
                return;
            }
            var tick = GetCurrentTick();
            if (_CallStackPosition == intHandle)
            {
                // 在绝大多数情况下都是这样,无需执行循环
                var tickSpan = tick - _CallStack[intHandle + 1];
                if (tickSpan < 0)
                {
                    throw new Exception("tickspan=" + tickSpan);
                }
                var mIndex = _CallStack[intHandle];
                _Methods[mIndex].TickCount += tickSpan;
                if (intHandle > 0)
                {
                    _Methods[_CallStack[intHandle - 2]].AddChildTick(mIndex, tickSpan);
                }
            }
            else
            {
                for (var iCount = _CallStackPosition; iCount >= intHandle; iCount -= 2)
                {
                    var tickSpan = tick - _CallStack[iCount + 1];
                    if (tickSpan < 0)
                    {
                        //for(var iCount2 = iCount;iCount2 >=0;iCount2 --)
                        //{
                        //    var item2 = _CallStack[iCount2];
                        //    if( item2.MethodIndex >= 0 && item2.MethodIndex < _Methods.Length )
                        //    {
                        //        Console.WriteLine(_Methods[item2.MethodIndex] + " " + item2.MethodIndex + " Start Tick:" + item2.StartTick);
                        //    }
                        //}
                        throw new Exception("tickspan=" + tickSpan);// + " " + new DateTime( tick ).ToLongTimeString() + " vs " + new DateTime( _CallStack[iCount].StartTick).ToLongTimeString() );
                    }
                    var mIndex = _CallStack[iCount];
                    _Methods[mIndex].TickCount += tickSpan;
                    if (iCount > 0)
                    {
                        _Methods[_CallStack[iCount - 2]].AddChildTick(mIndex, tickSpan);
                    }
                }
            }
            if( intHandle == 0 && _StartTimeTick > 0 )
            {
                // 离开检测区域，不参与累计时间
                _GlobalTickSpanFix += GetCurrentTick();
                _StartTimeTick = 0;
            }
            _CallStackPosition = intHandle - 2;
        }

        public static void Stop()
        {
            if (_Enabled)
            {
                Leave(0);
                _GlobalEndTick = GetCurrentTick();
                _Enabled = false;
            }
        }
    }

}