#DC.NET Protector
protect your .NET software copyright powerfull.

##Background
Many .net developers are worry about their software has been cracked , so they use some tools to obfuscate IL code.such as PreEmptive dotfuscator.But some times ,it is not enought.So I write DC.NET Protector to provide more powerful .net IL code protection and it is open source.
It is a console .NET application, the UI is :
<br/>
<img src="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/source/snapshort/snapshort.png"/>

## Features
It has following features.

## First.Avoid performance problem.
Some times application obfuscate can cause bad performance problem.For example, to the following C# code.
```C#
public static byte[] ParseUpperHexString(string hexs)
{
    var list = new List<byte>();
    int Value = -1;
    foreach (char c in hexs)
    {
        int index = "0123456789ABCDEF".IndexOf(c);
        if (index >= 0)
        {
            if (Value < 0)
            {
                Value = index;
            }
            else
            {
                Value = Value * 16 + index;
                list.Add((byte)Value);
                Value = -1;
            }
        }
    }
    return list.ToArray();
}
```
After dotfuscate the code change to
```C#
public static byte[] z0ZzZzbn(string A_0)
{
  int a_ = 15;
  List<byte> list = new List<byte>();
  int num = -1;
  foreach (char value in A_0)
  {
    int num2 = z0ZzZzbbz.b("\uf0bf\uf3c1\uf6c3\uf5c5\uffc9留\ue8cf\uebd1闓铕鯗黙駛飝", a_).IndexOf(value);
    if (num2 >= 0)
    {
      if (num < 0)
      {
        num = num2;
        continue;
      }
      num = num * 16 + num2;
      list.Add((byte)num);
      num = -1;
    }
  }
  return list.ToArray();
}

internal unsafe static string z0ZzZzbbz.b(string A_0, int A_1)
{
  char[] array = A_0.ToCharArray();
  int num = (int)((long)(IntPtr)(void*)((long)(IntPtr)(void*)((long)(1169192937 + A_1) + 80L) + 78L) + 41L);
  int num2 = 0;
  if (num2 >= 1)
  {
    goto IL_0029;
  }
  goto IL_005c;
  IL_005c:
  if (num2 >= array.Length)
  {
    return string.Intern(new string(array));
  }
  goto IL_0029;
  IL_0029:
  int num3 = num2;
  char num4 = array[num3];
  byte b = (byte)((num4 & 0xFFu) ^ (uint)num++);
  byte b2 = (byte)(((int)num4 >> 8) ^ num++);
  byte num5 = b2;
  b2 = b;
  b = num5;
  array[num3] = (char)((b2 << 8) | b);
  num2++;
  goto IL_005c;
}
```

This cause a serious performance problem.To solve the problem, I can use the follow code:
```C#
private static readonly string _HexChars = "0123456789ABCDEF";
public static byte[] ParseUpperHexString(string hexs)
{
  var list = new List<byte>();
  int Value = -1;
  foreach (char c in hexs)
  {
    int index = _HexChars.IndexOf(c);
    if (index >= 0)
    {
      if (Value < 0)
      {
        Value = index;
      }
      else
      {
        Value = Value * 16 + index;
        list.Add((byte)Value);
        Value = -1;
      }
    }
  }
  return list.ToArray();
}
```
the result code after dotfuscate is
```C#
private static readonly string z0ZzZzbg = z0ZzZzbbz.b("蚵覷袹辻誽\uf5bf\uf4c1\uf3c3\uf1c7诉軋跍铏韑鋓", 5);
public static byte[] z0ZzZzbu(string A_0)
{
  List<byte> list = new List<byte>();
  int num = -1;
  foreach (char value in A_0)
  {
    int num2 = DCFormCheckBoxElement.z0ZzZzbg.IndexOf(value);
    if (num2 >= 0)
    {
      if (num < 0)
      {
        num = num2;
        continue;
      }
      num = num * 16 + num2;
      list.Add((byte)num);
      num = -1;
    }
  }
  return list.ToArray();
}
```
So my new tools can analyse IL code ,select all string value define in methods , and bring out and redefine as a static readonly field,  This can solve this performance problem.
Additional , New tool can hidden all string value defines, hakers can not find any string value define in ildasm result.

## Second,Encrypt *.resources file.
 Haker can use ildasm.exe get *.resouces file emit in .NET assembly file , change it , mark their name or logo image, and use ilasm.exe to rebuild a .NET assembly file.Change your copyright UI as haker's copyright UI.
My new tool can encrypt *.resouces files and hidden it, It is more hard to modify copyright UI.So my new tool can protect your copyright.
Additional,If software is design for globalization with multiple UI language,The software will include specify UI language resource dll files.My new tools will prompt operator to select a UI language and compress UI language resource data to IL code . This will provide a more fast lanuch speed and without UI language resouce dll.
My new tool also can change the resource package class code.For example, This is a resource package class code:
```C#
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resource1
{
	private static ResourceManager resourceMan;
	private static CultureInfo resourceCulture;
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				ResourceManager temp = resourceMan = new ResourceManager("SampleWinApp.Resource1", typeof(Resource1).Assembly);
			}
			return resourceMan;
		}
	}
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}
	internal static Bitmap blue96
	{
		get
		{
			object obj = ResourceManager.GetObject("blue96", resourceCulture);
			return (Bitmap)obj;
		}
	}
	internal static string String2 => ResourceManager.GetString("String2", resourceCulture);
	internal static string StringValue => ResourceManager.GetString("StringValue", resourceCulture);
	internal Resource1()
	{
	}
}
```
After use my new tool , It change to :
```C#
internal class Resource1
{
	private static readonly byte[] _Datas = _BytesContainer__._1();
	private static Bitmap _blue96;
	public static Bitmap get_blue96()
	{
		if (_blue96 == null)
		{
			_blue96 = InnerAssemblyHelper20210315.GetBitmap(_Datas, 0, 36918, 1807292644);
		}
		return _blue96;
	}
	internal static string get_String2()
	{
		return InnerAssemblyHelper20210315.GetString(_Datas, 37002, 98, 614997590);
	}
	internal static string get_StringValue()
	{
		return InnerAssemblyHelper20210315.GetString(_Datas, 36918, 84, 57466195);
	}
}
```
The resouce data aleady has been encrypted, and hard to crack.

## Thirth,Hidden allocation call stack.
Haker can get and seach key code by using memory profiler tools , etc. Scitech .NET memory Profiler.
For example, I use the follow code to display software license info.
```C#
private void btnAbout_Click(object sender, EventArgs e)
{
    MessageBox.Show(this, GetLicenseMessage());
}
private string GetLicenseMessage()
{
    string msg = "This software license to :" + Environment.UserName;
    return msg;
}
```
When you start application in Scitech .NET memory Profiler, and show the about dialog. the screensnapshort like this.
<br/><img src="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/source/snapshort/winformdemo.png"/>
In .NET Memory profiler.the UI is 
<br/><img src="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/source/snapshort/winformdemo2.png"/>
seach the string "This software license to:Administrator" and double click , then you can see the follow UI:
<br/><img src="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/source/snapshort/winformdemo3.png"/>
There are list the key string value allocation call stack :
```C#
mscorlib!System.String.Concat( string,string )
SampleWinApp!SampleWinApp.frmMain.GetLicenseMessage() frmMain.cs
SampleWinApp!SampleWinApp.frmMain.btnAbout_Click( object,EventArgs ) frmMain.cs
System.Windows.Forms!System.Windows.Forms.Control.OnClick( EventArgs )
System.Windows.Forms!System.Windows.Forms.Button.OnMouseUp( MouseEventArgs )
System.Windows.Forms!System.Windows.Forms.Control.WmMouseUp( ref Message,MouseButtons,int )
System.Windows.Forms!System.Windows.Forms.Control.WndProc( ref Message )
System.Windows.Forms!System.Windows.Forms.ButtonBase.WndProc( ref Message )
System.Windows.Forms!System.Windows.Forms.Control.ControlNativeWindow.WndProc( ref Message )
System.Windows.Forms!System.Windows.Forms.NativeWindow.Callback( IntPtr,int,IntPtr,IntPtr )
[Native to managed transition]
[Managed to native transition]
System.Windows.Forms!System.Windows.Forms.UnsafeNativeMethods.DispatchMessageW( ref MSG )
System.Windows.Forms!System.Windows.Forms.Application.ComponentManager.System.Windows.Forms.UnsafeNativeMethods.IMsoComponentManager.FPushMessageLoop( int,int,int )
System.Windows.Forms!System.Windows.Forms.Application.ThreadContext.RunMessageLoopInner( int,ApplicationContext )
System.Windows.Forms!System.Windows.Forms.Application.ThreadContext.RunMessageLoop( int,ApplicationContext )
SampleWinApp!SampleWinApp.Program.Main() Program.cs
```
 This call stack maby point out how to crack the software. 
My new tool can change this call stack to this:
```C#
mscorlib!System.String.CtorCharArray( char[] )
SampleWinApp2!DCSoft.Common.InnerAssemblyHelper20210315.CloneStringCrossThead_Thread()
mscorlib!System.Threading.ExecutionContext.RunInternal( ExecutionContext,ContextCallback,object,bool )
mscorlib!System.Threading.ExecutionContext.Run( ExecutionContext,ContextCallback,object,bool )
mscorlib!System.Threading.ExecutionContext.Run( ExecutionContext,ContextCallback,object )
mscorlib!System.Threading.ThreadHelper.ThreadStart()
```
It is more difficuted to find out the key call stack.This feature help you protect your software copyright.

## Fifth. Easy to use.
My new tool is a .NET framework console  application.     It support following command line argument :
        input =[required,Full path of input .NET assembly file , can be .exe or .dll, currenttly only support .NET framework 2.0 or later]
        output=[optional,Full path of output .NET assmebly file , if it is empty , then use input argument value]
        snk   =[optional,Full path of snk file. It use to add strong name to output assembly file.]
        pause =[optional,pause the console after finish process.]
     Example 1, protect d:\a.dll ,this will modify dll file.
        DCNETProtector.exe input=d:\a.dll  
     Exmaple 2, anlyse d:\a.dll , and write result to another dll file with strong name.
        DCNETProtector.exe input=d:\a.dll output=d:\publish\a.dll snk=d:\source\company.snk

## My target
This new tool's finall target is protect all .NET software copyright.Of cause,it does not do all things, it does not support renaming,flow-obfuscate, For those function you can seach other tools.


