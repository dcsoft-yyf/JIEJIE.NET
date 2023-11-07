/*
   JieJie.NET
  
   An open source tool to encrypt .NET assembly file, help you protect your copyright.

   JieJie in chinese word is 结界, is a kind of transparet magic protect shield.

   You can use this software unlimited,but CAN NOT modify source code anytime.

   The only target of open source is let you know this software is safety.

   Any good idears you can write to 28348092@qq.com .
 
 */
using System;
using System.Runtime.InteropServices;
using System.Reflection;
 

namespace JIEJIE
{
    /// <summary>
    /// 命令行输出界面
    /// </summary>
    [Serializable]
    internal class MyConsole : System.MarshalByRefObject
    {
        private static MyConsole _Instance = new MyConsole();
        /// <summary>
        /// 对象静态实例
        /// </summary>
        public static MyConsole Instance
        {
            get
            {
                return _Instance;
            }
        }
        /// <summary>
        /// 设置对象静态实例
        /// </summary>
        /// <param name="instance"></param>
        public static void SetInstance(MyConsole instance)
        {
            if (instance == null)
            {
                _Instance = new MyConsole();
            }
            else
            {
                _Instance = instance;
            }
        }
        public MyConsole()
        {

        }
        /// <summary>
        /// 确保在新的一行开始
        /// </summary>
        /// <returns>操作是否产生换行行为</returns>
        public virtual bool EnsureNewLine()
        {
            return false;
        }
        protected bool _IsNativeConsole = true ;
        public bool IsNativeConsole
        {
            get
            {
                return this._IsNativeConsole;
            }
            set
            {
                this._IsNativeConsole = value;
            }
        }
        /// <summary>
        /// 是否支持键盘输入
        /// </summary>
        public virtual bool SupportKeyboardInput
        {
            get
            {
                return this._IsNativeConsole && System.Environment.UserInteractive;
            }
        }
        /*
        
                //
    // 摘要:
    //     指定定义控制台前景色和背景色的常数。
    public enum ConsoleColor
    {
        //
        // 摘要:
        //     黑色。
        Black = 0,
        //
        // 摘要:
        //     藏蓝色。
        DarkBlue = 1,
        //
        // 摘要:
        //     深绿色。
        DarkGreen = 2,
        //
        // 摘要:
        //     深紫色（深蓝绿色）。
        DarkCyan = 3,
        //
        // 摘要:
        //     深红色。
        DarkRed = 4,
        //
        // 摘要:
        //     深紫红色。
        DarkMagenta = 5,
        //
        // 摘要:
        //     深黄色（赭色）。
        DarkYellow = 6,
        //
        // 摘要:
        //     灰色。
        Gray = 7,
        //
        // 摘要:
        //     深灰色。
        DarkGray = 8,
        //
        // 摘要:
        //     蓝色。
        Blue = 9,
        //
        // 摘要:
        //     绿色。
        Green = 10,
        //
        // 摘要:
        //     青色（蓝绿色）。
        Cyan = 11,
        //
        // 摘要:
        //     红色。
        Red = 12,
        //
        // 摘要:
        //     紫红色。
        Magenta = 13,
        //
        // 摘要:
        //     黄色。
        Yellow = 14,
        //
        // 摘要:
        //     白色。
        White = 15
    }

            */
        private static System.Drawing.Color[] _ConsoleColors = null;
        /// <summary>
        /// 将命令行颜色值转换为标准颜色值
        /// </summary>
        /// <param name="c">命令行颜色值</param>
        /// <returns>标准颜色值</returns>
        protected System.Drawing.Color ToColor(System.ConsoleColor c)
        {
            if (_ConsoleColors == null)
            {
                lock (typeof(MyConsole))
                {
                    _ConsoleColors = new System.Drawing.Color[] {
                            System.Drawing.Color.FromArgb( 12,12,12) ,
                            System.Drawing.Color.FromArgb( 0 , 55, 218) ,
                            System.Drawing.Color.FromArgb( 19 , 161 , 14 ) ,
                            System.Drawing.Color.FromArgb( 58 , 150 , 221) ,
                            System.Drawing.Color.FromArgb( 197, 15 , 31 ) ,
                            System.Drawing.Color.FromArgb( 136 , 23 , 152 )  ,
                            System.Drawing.Color.FromArgb( 193,156,0),
                            System.Drawing.Color.FromArgb( 204,204,204) ,
                            System.Drawing.Color.FromArgb( 118,118,118) ,
                            System.Drawing.Color.FromArgb( 59,120,255) ,
                            System.Drawing.Color.FromArgb( 22,198,12),
                            System.Drawing.Color.FromArgb(97,214,214) ,
                            System.Drawing.Color.FromArgb(231,72,86),
                            System.Drawing.Color.FromArgb(180,0,158) ,
                            System.Drawing.Color.FromArgb(249,241,165) ,
                            System.Drawing.Color.FromArgb(242,242,242)
                        };

                    //var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Console", false);
                    //if (reg != null)
                    //{
                    //    var indexMap = new ConsoleColor[16];
                    //    indexMap[0] = ConsoleColor.Black;
                    //    indexMap[4] = ConsoleColor.DarkBlue;
                    //    indexMap[2] = ConsoleColor.DarkGreen;
                    //    indexMap[6] = ConsoleColor.DarkCyan;
                    //    indexMap[1] = ConsoleColor.DarkRed;
                    //    indexMap[5] = ConsoleColor.DarkMagenta;
                    //    indexMap[3] = ConsoleColor.DarkYellow;
                    //    indexMap[7] = ConsoleColor.Gray;
                    //    indexMap[8] = ConsoleColor.DarkGray;
                    //    indexMap[12] = ConsoleColor.Blue;
                    //    indexMap[10] = ConsoleColor.Green;
                    //    indexMap[14] = ConsoleColor.Cyan;
                    //    indexMap[9] = ConsoleColor.Red;
                    //    indexMap[13] = ConsoleColor.Magenta;
                    //    indexMap[11] = ConsoleColor.Yellow;
                    //    indexMap[15] = ConsoleColor.White;
                    //    for (int iCount = 0; iCount < _ConsoleColors.Length; iCount++)
                    //    {
                    //        var cv = reg.GetValue("ColorTable" + iCount.ToString("00"));
                    //        if (cv is int)
                    //        {
                    //            _ConsoleColors[(int)indexMap[iCount]] = System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb((int)cv));
                    //        }
                    //    }
                    //    reg.Close();
                    //}
                    ////foreach( var item in Enum.GetValues( typeof(System.ConsoleColor)))
                    ////{
                    ////    var v =  _ConsoleColors[(int)item].ToArgb().ToString("X6").Substring( 2 );
                    ////    System.Diagnostics.Debug.WriteLine("<br />" + Convert.ToInt32( item ) +" " + item.ToString() + "<span style='width:100px;background-color:#" + v + "'>11111111</span>");
                    ////}
                }

            }
            if (c >= ConsoleColor.Black && c <= ConsoleColor.White)
            {
                return _ConsoleColors[(int)c];
            }
            else
            {
                return System.Drawing.Color.Black;
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title
        {
            get
            {
                return Console.Title;
            }
            set
            {
                Console.Title = value;
            }
        }
        
        public virtual bool KeyAvailable
        {
            get
            {
                return Console.KeyAvailable;
            }
        }
        public virtual string ReadLine()
        {
            return Console.ReadLine();
        }
        public virtual ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
        public virtual int CursorLeft
        {
            get
            {
                return Console.CursorLeft;
            }
            set
            {
                Console.CursorLeft = value;
            }
        }

        public virtual int CursorTop
        {
            get
            {
                return Console.CursorTop;
            }
            set
            {
                Console.CursorTop = value;
            }
        }

        public virtual ConsoleColor BackgroundColor
        {
            get
            {
                return Console.BackgroundColor;
            }
            set
            {
                Console.BackgroundColor = value;
            }
        }
        public virtual ConsoleColor ForegroundColor
        {
            get
            {
                return Console.ForegroundColor;
            }
            set
            {
                Console.ForegroundColor = value;
            }
        }
        public virtual void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
        public virtual void WriteLine()
        {
            Console.WriteLine();
        }
        public virtual void Write(string value)
        {
            Console.Write(value);
        }
        public virtual void ResetColor()
        {
            Console.ResetColor();
        }
        public virtual void WriteError(string msg)
        {
            this.ForegroundColor = ConsoleColor.Red;
            this.WriteLine(msg);
            this.ResetColor();
        }
    }
}
