# JIEJIE.NET
  An open source tool to obfuscation .NET assembly file, help people protect theirs copyright.
  <br /> Jie(2)Jie(4) in chinese is a kind of transparet magic protect shield.
## update log
<br/> 2021-9-21 : Clean document comment xml element which renamed.
<br/> 2021-9-9  : package small properties and change call/callvirt instructions.
<br/> 2021-8-23 : Add feature: Support .NET core,fix some bugs.
<br/> 2021-7-20 : Add feature: type or member rename.
<br/> 2021-4-2  : Add feature: Obfuscate control flow.
<br/> 2021-3-22 : First publish.

## Background
Many .net developers are worry about their software has been cracked,copyright under infringed, so they use some tools to obfuscate IL code.such as PreEmptive dotfuscator.But some times ,it is not enought.
<br/>So I write JieJie.NET can encrypt .NET assembly deeply,help people protect their copyright.and this tool is open source.
It is a console .NET application, the UI is :
<br/>
<img src="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/source/snapshort/snapshort.png"/>

## Features
It has following features.

## 1 , Rename type and member.
JieJie can change type and member's name.This can make more difficute to understand the meaning of API.And effect by `[System.Reflection.ObfuscationAttribute]`.
<br />For example, the old code is :
```C#
public abstract class XTextDocumentContentElement : XTextContentElement
{
    public override void AfterLoad(ElementLoadEventArgs args);
    public override void Clear();
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public override XTextElement Clone(bool Deeply);
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public override XTextDocument CreateContentDocument(bool includeThis);
    public XTextSelection CreateSelection(int startIndex, int length);
    public override void Dispose();
    public override void DrawContent(InnerDocumentPaintEventArgs args);
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public override void EditorRefreshViewExt(bool fastMode);
    public float FixPageLinePosition(int pos);
    public override void Focus();
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public XTextLineList GetAllLines();
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public virtual XTextRange GetRange(int StartIndex, int EndIndex);
    public void InnerGetSelectionBorderElement(ref XTextElement startElement, ref XTextElement endElement);
    public void InvalidateSpecifyLayoutElements();
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public virtual bool IsSelected(XTextElement element);
    public void RefreshParagraphListState(bool checkFlag, bool updateListIndex);
    public XTextParagraphFlagElement RootParagraphFlag();
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public bool SetSelection(int startIndex, int length);
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public bool SetSelectionRange(int firstIndex, int lastIndex);
}
```
After rename, these code change to:
```C#
public abstract class XTextDocumentContentElement : XTextContentElement
{
    public override void Clear();
    public override XTextElement Clone(bool Deeply);
    public override XTextDocument CreateContentDocument(bool includeThis);
    public override void Dispose();
    public override void EditorRefreshViewExt(bool fastMode);
    public override void Focus();
    public XTextLineList GetAllLines();
    public virtual XTextRange GetRange(int StartIndex, int EndIndex);
    public virtual bool IsSelected(XTextElement element);
    public bool SetSelection(int startIndex, int length);
    public bool SetSelectionRange(int firstIndex, int lastIndex);
    public XTextParagraphFlagElement z0ZzZzbmm1mO001();
    public XTextSelection z0ZzZzbmm1mO011(int startIndex, int length);
    public void z0ZzZzbmm1mO01O();
    public float z0ZzZzbmm1mOOm1(int pos);
    public void z0ZzZzbmm1mOOmn(ref XTextElement startElement, ref XTextElement endElement);
    public void z0ZzZzbmm1mOOmO(bool checkFlag, bool updateListIndex);
    public override void z0ZzZzbmmOO11nn(z0ZzZzbm0mmlm1O args);
    public override void z0ZzZzbmmOOl0nO(ElementLoadEventArgs args);
}
```
You can see , some API's name obfuscated.

## 2 , Obfuscate control-flow.
JieJie can anlyse IL Code, and obfuscate control-flow randomly without lost any features, It can break syntactic structure for `foreach/lock/using`. hiden the operation of euqals and concat tow string values. It let codes are very hard to read, some times it will cause crack tool error.
<br />For example , the old code is :
```C#
public void RemoveTaskByTaskID(string taskID)
{
    if (taskID == null)
    {
        return;
    }
    if (CheckOwner() == false)
    {
        return;
    }
    lock (this)
    {
        if (this._CurrentTask != null && this._CurrentTask.ID == taskID)
        {
            this._CurrentTask = null;
        }
        foreach (BackgroundTask task in _Tasks)
        {
            if (task.ID == taskID)
            {
                _Tasks.Remove(task);
                break;
            }
        }
    }
}
```
After use JieJie.NET, these code display in ILSpy is:
```C#
public void RemoveTaskByTaskID(string taskID)
{
	//Discarded unreachable code: IL_0006, IL_000e, IL_0022, IL_0087, IL_00ea, IL_00f7
	//IL_006e: Incompatible stack heights: 1 vs 0
	//IL_0078: Incompatible stack heights: 0 vs 1
	//IL_0082: Incompatible stack heights: 0 vs 1
	//IL_00e5: Incompatible stack heights: 0 vs 2
	//IL_012c: Incompatible stack heights: 1 vs 0
	if (taskID == null || !z0qwk())
	{
		return;
	}
	z0ZzZzjyf.z0pek(this);
	try
	{
		if (z0cqk != null && z0ZzZzjyf.z0mek(z0cqk.z0dwk(), taskID))
		{
			z0cqk = null;
		}
		z0kwk.GetEnumerator();
		using List<BackgroundTask>.Enumerator enumerator = /*Error near IL_001c: Stack underflow*/;
		BackgroundTask backgroundTask;
		do
		{
			enumerator.MoveNext();
			double num;
			if ((int)num == 0)
			{
				return;
			}
			_ = enumerator.Current;
			backgroundTask = (BackgroundTask)/*Error near IL_003b: Stack underflow*/;
		}
		while (!z0ZzZzjyf.z0mek(backgroundTask.z0dwk(), taskID));
		z0kwk.Remove(backgroundTask);
	}
	finally
	{
		z0ZzZzjyf.z0tek(this);
	}
}
```
Look, the `foreach` and `lock` broken , and ILSpy has error ` /*Error near IL_003b: Stack underflow*/ `. And use .NET Reflector 10.3,It stop work direct.
<br />JieJie.NET can package small properties. In the old code ` task.ID ` , and in the new code it changed to ` backgroundTask.z0dwk() ` . JieJie.NET create a new method ` z0dwk() ` and copy the function of ` backgroundTask.get_ID() `.
This cause the new code is more hard to read.

## 3 , Encrypt all string values define in assembly.
JieJie.NET can collect all string values define in assembly,convert they to static readonly fields in a new class,and encrypt theirs value.Make hakers can no search string value direct, crack is more difficulty.
<br/>For example , the old code is :
```C#
private string GetLicenseMessage()
{

    return "This software license to :" + Environment.UserName;
}
```
 After use JieJie.NET , the new code is :
  
```C#
private string GetLicenseMessage()
{
    string text = _0._6 + Environment.UserName;
    return text;
}
//  also create a new class, contains all string value in assembly in random order.
internal static class _0
{
    public static readonly string _0;
    public static readonly string _1;
    public static readonly string _2;
    public static readonly string _3;
    public static readonly string _4;
    public static readonly string _5;
    public static readonly string _6;
    public static readonly string _7;
    public static readonly string _8;
    public static readonly string _9;
    public static readonly string _10;
    public static readonly string _11;
    public static readonly string _12;
    public static readonly string _13;
    public static readonly string _14;
    public static readonly string _15;
    public static readonly string _16;
    public static readonly string _17;
    public static readonly string _18;
    public static readonly string _19;
    public static readonly string _20;
    public static readonly string _21;

    static _0()
    {
        byte[] datas = _BytesContainer__._0();
        _11 = GetStringByLong(datas, 151732605047602L);
        _20 = GetStringByLong(datas, 450799767951810L);
        _7 = GetStringByLong(datas, 101155071172227L);
        _4 = GetStringByLong(datas, 47279000500949L);
        _15 = GetStringByLong(datas, 415615395474299L);
        _5 = GetStringByLong(datas, 54975582493063L);
        _2 = GetStringByLong(datas, 17592187197342L);
        _14 = GetStringByLong(datas, 206708198516324L);
        _8 = GetStringByLong(datas, 124244814685054L);
        _21 = GetStringByLong(datas, 459595860893446L);
        _6 = GetStringByLong(datas, 72567769190975L);
        _13 = GetStringByLong(datas, 182518931688172L);
        _18 = GetStringByLong(datas, 433207581847376L);
        _16 = GetStringByLong(datas, 417814419099513L);
        _3 = GetStringByLong(datas, 36283884381871L);
        _1 = GetStringByLong(datas, 9895605165436L);
        _9 = GetStringByLong(datas, 136339442622330L);
        _19 = GetStringByLong(datas, 440904163377248L);
        _17 = GetStringByLong(datas, 426610511995160L);
        _0 = GetStringByLong(datas, 598562L);
        _10 = GetStringByLong(datas, 148434069970387L);
        _12 = GetStringByLong(datas, 158329675868829L);
    }
    private static string GetStringByLong(byte[] datas, long key)
    {
        int num = (int)(key & 0xFFFF) ^ 0xEF83;
        key >>= 16;
        int num2 = (int)(key & 0xFFFFF);
        key >>= 24;
        int num3 = (int)key;
        char[] array = new char[num2];
        int num4 = 0;
        while (num4 < num2)
        {
            int num5 = num4 + num3 << 1;
            array[num4] = (char)(((datas[num5] << 8) + datas[num5 + 1]) ^ num);
            num4++;
            num++;
        }
        return new string(array);
    }
}
```
Additional, this process can avoid a kind of performance problem cause by assembly obfuscation. 
<br/>For example, use the following code:
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
After dotfuscate the code change to:
```C#
public static byte[] z0ZzZzbn(string A_0)
{
  int a_ = 15;
  List<byte> list = new List<byte>();
  int num = -1;
  foreach (char value in A_0)
  {
    int num2 = z0ZzZzbbz.b("\uf0bf\uf3c1\uf6c3\uf5c5\uffc9¨¢?\ue8cf\uebd1¨ºG?e??¨¹a???y", a_).IndexOf(value);
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
This cause a serious performance problem.To solve the problem, by use JieJie.NET, this code change to :
```C#
private static readonly string _HexChars = _0._11 ;
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
After dotfuscate the code change to:
```C#
private static readonly string z0ZzZzbg = z0ZzZzbbz.z0ZzZzbef;
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
This code avoid the performance problem.

## 4 , Encrypt *.resources file.
 Haker can dasm .NET assembly file use ildasm.exe, and get all `*.resouces` file embed in assembly , change it , maby replace their name or logo image, and use ilasm.exe to rebuild a .NET assembly file.Change your copyright UI to haker's copyright UI.
<br/>  JieJie.NET can encrypt *.resouces files and hidden it, It is more hard to modify copyright UI.So it can protect your copyright.
<br/>For example, your a define a WinFrom , and the InitializeComponent() function code is :
```C#
private void InitializeComponent()
{
    System.ComponentModel.ComponentResourceManager resources 
            = new System.ComponentModel.ComponentResourceManager(typeof(SampleWinApp.frmMain));
    pictureBox1 = new System.Windows.Forms.PictureBox();
    btnAbout = new System.Windows.Forms.Button();
    btnDoWork = new System.Windows.Forms.Button();
    label1 = new System.Windows.Forms.Label();
    button1 = new System.Windows.Forms.Button();
    ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
    SuspendLayout();
    pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
    pictureBox1.Location = new System.Drawing.Point(150, 21);
    pictureBox1.Name = "pictureBox1";
    pictureBox1.Size = new System.Drawing.Size(64, 64);
    pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
    pictureBox1.TabIndex = 1;
    pictureBox1.TabStop = false;
    btnAbout.Location = new System.Drawing.Point(21, 188);
    btnAbout.Name = "btnAbout";
    btnAbout.Size = new System.Drawing.Size(299, 64);
    btnAbout.TabIndex = 2;
    btnAbout.Text = "About...";
    btnAbout.UseVisualStyleBackColor = true;
    btnAbout.Click += new System.EventHandler(btnAbout_Click);
    btnDoWork.Location = new System.Drawing.Point(21, 109);
    btnDoWork.Name = "btnDoWork";
    btnDoWork.Size = new System.Drawing.Size(299, 64);
    btnDoWork.TabIndex = 3;
    btnDoWork.Text = "Do work";
    btnDoWork.UseVisualStyleBackColor = true;
    btnDoWork.Click += new System.EventHandler(btnDoWork_Click);
    label1.AutoSize = true;
    label1.Location = new System.Drawing.Point(13, 43);
    label1.Name = "label1";
    label1.Size = new System.Drawing.Size(131, 12);
    label1.TabIndex = 4;
    label1.Text = "This is a logo image:";
    button1.Location = new System.Drawing.Point(21, 275);
    button1.Name = "button1";
    button1.Size = new System.Drawing.Size(299, 63);
    button1.TabIndex = 5;
    button1.Text = "Get string in resource";
    button1.UseVisualStyleBackColor = true;
    button1.Click += new System.EventHandler(button1_Click);
    base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
    base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    base.ClientSize = new System.Drawing.Size(414, 365);
    base.Controls.Add(button1);
    base.Controls.Add(label1);
    base.Controls.Add(btnDoWork);
    base.Controls.Add(btnAbout);
    base.Controls.Add(pictureBox1);
    base.Name = "frmMain";
    Text = "frmMain";
    base.Load += new System.EventHandler(frmMain_Load);
    ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
    ResumeLayout(false);
    PerformLayout();
}
```
After use JieJie.NET, the code change to :
```C#
private void InitializeComponent()
{
    __DC20210205._Res1 res = new __DC20210205._Res1();
    pictureBox1 = new System.Windows.Forms.PictureBox();
    btnAbout = new System.Windows.Forms.Button();
    btnDoWork = new System.Windows.Forms.Button();
    label1 = new System.Windows.Forms.Label();
    button1 = new System.Windows.Forms.Button();
    ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
    SuspendLayout();
    pictureBox1.Image = (System.Drawing.Image)res.GetObject(__DC20210205._0._2);
    pictureBox1.Location = new System.Drawing.Point(150, 21);
    pictureBox1.Name = __DC20210205._0._8;
    pictureBox1.Size = new System.Drawing.Size(64, 64);
    pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
    pictureBox1.TabIndex = 1;
    pictureBox1.TabStop = false;
    btnAbout.Location = new System.Drawing.Point(21, 188);
    btnAbout.Name = __DC20210205._0._16;
    btnAbout.Size = new System.Drawing.Size(299, 64);
    btnAbout.TabIndex = 2;
    btnAbout.Text = __DC20210205._0._20;
    btnAbout.UseVisualStyleBackColor = true;
    btnAbout.Click += new System.EventHandler(btnAbout_Click);
    btnDoWork.Location = new System.Drawing.Point(21, 109);
    btnDoWork.Name = __DC20210205._0._0;
    btnDoWork.Size = new System.Drawing.Size(299, 64);
    btnDoWork.TabIndex = 3;
    btnDoWork.Text = __DC20210205._0._21;
    btnDoWork.UseVisualStyleBackColor = true;
    btnDoWork.Click += new System.EventHandler(btnDoWork_Click);
    label1.AutoSize = true;
    label1.Location = new System.Drawing.Point(13, 43);
    label1.Name = __DC20210205._0._11;
    label1.Size = new System.Drawing.Size(131, 12);
    label1.TabIndex = 4;
    label1.Text = __DC20210205._0._7;
    button1.Location = new System.Drawing.Point(21, 275);
    button1.Name = __DC20210205._0._4;
    button1.Size = new System.Drawing.Size(299, 63);
    button1.TabIndex = 5;
    button1.Text = __DC20210205._0._13;
    button1.UseVisualStyleBackColor = true;
    button1.Click += new System.EventHandler(button1_Click);
    base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
    base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    base.ClientSize = new System.Drawing.Size(414, 365);
    base.Controls.Add(button1);
    base.Controls.Add(label1);
    base.Controls.Add(btnDoWork);
    base.Controls.Add(btnAbout);
    base.Controls.Add(pictureBox1);
    base.Name = __DC20210205._0._18;
    Text = __DC20210205._0._18;
    base.Load += new System.EventHandler(frmMain_Load);
    ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
    ResumeLayout(false);
    PerformLayout();
}
// And auto create a new class:
internal class _Res1 : ComponentResourceManager, IDisposable
{
    private ResourceSet _Data;

    public _Res1()
    {
        _Data = InnerAssemblyHelper20210315.LoadResourceSet(
                _BytesContainer__._2(), 224, gzip: true);
    }

    public override ResourceSet GetResourceSet(
                CultureInfo culture, bool createIfNotExists, bool tryParents)
    {
        return _Data;
    }

    protected override ResourceSet InternalGetResourceSet(
                CultureInfo culture, bool createIfNotExists, bool tryParents)
    {
        return _Data;
    }

    public void Dispose()
    {
        if (_Data != null)
        {
            _Data.Close();
            _Data = null;
        }
    }
}
```
And remove the embeded resource "SampleWinApp.frmMain.resources". after do these, new code is very difficulty to crack.

   Additional,If software is design for globalization with multiple UI language,The software will include specify UI language resource dll files.My tools will prompt operator to select a UI language and merge UI language resource data to IL code .and remove embeded .resources file. This will provide a more fast lanuch speed and without UI language resouce dll.
<br/>My tool also can change the resource package class code.
<br/>For example, This is a resource package class code:
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
                ResourceManager temp = resourceMan =  new ResourceManager(
                    "SampleWinApp.Resource1", typeof(Resource1).Assembly);
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
    internal static string String2 {
        get{ return ResourceManager.GetString("String2", resourceCulture);}
    }
    internal static string StringValue {
        get{ return ResourceManager.GetString("StringValue", resourceCulture);}
    }
    internal Resource1()
    {
    }
}
```
After use my tool , It change to :
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

## 5 ,Hidden allocation call stack.
Hackers can search key information by using memory profiler tools , etc. Scitech .NET memory Profiler.but JieJie.NET can change this stack,puzzle hackers.
<br/>For example, I use the follow code to display software license info.
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
<br/>In .NET Memory profiler.the UI is 
<br/><img src="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/source/snapshort/winformdemo2.png"/>
seach the string `"This software license to:Administrator"` and double click , then you can see the follow UI:
<br/><img src="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/source/snapshort/winformdemo3.png"/>
There are list the key string value allocation call stack :
```
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
 <br/>Then you can change source to :
 ```C#
 private void btnAbout_Click(object sender, EventArgs e)
{
    MessageBox.Show(this, GetLicenseMessage());
}
         
private string GetLicenseMessage()
{
    var str = "JIEJIE.NET.SWITCH:+allocationcallstack";;// no used,just let JieJie.NET know the owner method need change.
    string msg = "This software license to :" + Environment.UserName;
    return msg;
}
```
At there,the code `var str = "JIEJIE.NET.SWITCH:+allocationcallstack";";` do nothing, just let JieJie.NET know this is a key method, need to change, the value is ignore case.
<br/>My tool can change this call stack to this:
```
mscorlib!System.String.CtorCharArray( char[] )
SampleWinApp2!DCSoft.Common.InnerAssemblyHelper20210315.CloneStringCrossThead_Thread()
mscorlib!System.Threading.ExecutionContext.RunInternal( ExecutionContext,ContextCallback,object,bool )
mscorlib!System.Threading.ExecutionContext.Run( ExecutionContext,ContextCallback,object,bool )
mscorlib!System.Threading.ExecutionContext.Run( ExecutionContext,ContextCallback,object )
mscorlib!System.Threading.ThreadHelper.ThreadStart()
```
It is more difficuted to find out the key call stack.This feature help you hidden your weakness, protect your software copyright.

## 6 , Obfuscate class's members order.
   When we write a large class's code , usual ,field or method for the same target is very nearby.for example:
```C#
    private string _RegisterCode = null;
    private bool _IsRegisteredFlag = false;
    public void SetRegisterCode( string code ){};
    pulbic bool IsRegisterdCodeOK( string code ){};
    public string GetErrorMessageForRegister();
    XXXXXXX other members XXXXXX
```
When hakers capture one key member,for example `_RegisterCode` , and analyse other members nearby, maby can get more information.
<br/>But JieJie.NET can obfuscate order of class's members , just like this:
```C#
    private bool _IsRegisteredFlag = false;
    XXXXXXX other members XXXXXX
    private string _RegisterCode = null;
    XXXXXXX other members XXXXXX
    public string GetErrorMessageForRegister();
    XXXXXXX other members XXXXXX
    pulbic bool IsRegisterdCodeOK( string code ){};
    XXXXXXX other members XXXXXX
    public void SetRegisterCode( string code ){};
    XXXXXXX other members XXXXXX
```
 Other members nearby maby have nothing to do with one key member.this can make carck more difficult.

## 7 , Clean document comment xml file.
   JIEJIE.NET can clean document comment xml file. remove member xml element which it renamed.

## 8 , Save rename map xml file.
   JIEJIE.NET can save rename map xml file just like the following:
```XML
<dotfuscatorMap>
   <header>
      <timestamp>2021-10-23 9:13:25</timestamp>
      <product version="1.4.0.1" user="yfyuan">JieJie.NET</product>
   </header>
   <mapping>
      <module>
         <name>DCSoft.Writer.ForASPNETCore.dll</name>
         <type>
            <name>DCSoft.MyLicense.D8CLicNothing217Helper</name>
            <newname>zzz.z0ZzZzxgk</newname>
            <fieldlist>
               <field>
                  <signature>System.Collections.Generic.Dictionary`2&lt;object,object&gt;</signature>
                  <name>_Fields</name>
                  <newname>z0xjk</newname>
               </field>
            </fieldlist>
         </type>
         <type>
            <name>DCSoft.MyLicense.D8CLicNothing063Attribute</name>
            <newname>zzz.z0ZzZzzgk</newname>
         </type>
         <type>
            <name>DCSoft.Writer.Controls.Web.WebServerCommand</name>
            <newname>zzz.z0ZzZzlfk</newname>
            <fieldlist>
               <field>
                  <signature>string</signature>
                  <name>_Description</name>
                  <newname>z0zjk</newname>
               </field>
```
## 9 , Merge assembly files.
   When developing , many .NET application split to some assembly files,maby include one exe file and many dll files.
   <br/>JIEJIE.NET can merge assembly files into a single assembly file.This let application more easy to copy or upgrade.

## 10 , Custom instruction.
	 JIEJIE.NET support change .coreflags|.subsystem instruction. Some time, .NET assembly is design for x86/x64,JIEJIE.NET can change the target platform.for example use command line:
```
	>jiejie.net.exe d:\\aa.dll .corflags=0x1
```
   This can change the result assembly file to x64 platform.
   
## 11 , Support .NET Core 3.1
   JIEJIE.NET now support .NET Core 3.1.

## 12 , Easy to use.
My new tool is a .NET framework console  application. 
<br/>It support following command line argument :
```
        input =[required,default argument,Full path of input .NET assembly file , can be .exe or .dll,
                 currenttly only support .NET framework 2.0 or later]
        output=[optional,Full path of output .NET assmebly file , if it is empty , then use input argument value]
        snk   =[optional,Full path of snk file. It use to add strong name to output assembly file.]
        switch=[optional,multi-switch split by ',',also can be define in [System.Reflection.ObfuscationAttribute.Feature].
                It support :
                +contorlfow    = enable obfuscate control flow in method body.
                -contorlfow    = disable obfuscate control flow in method body.
                +/-strings     = enable/disable encrypt string value.
                +/-resources   = enable/disable encrypt resources data.
                +/-memberorder = enable/disable member list order in type.
                +/-rename      = enable/disable rename type or member's name.
                +/-allocationcallstack  = enable/disable encrypt string value allocation callstack.
            ]
        mapxml=[optional, a file/directory name to save map infomation for class/member's old name and new name in xml format.]
        pause =[optional,pause the console after finish process.]
        debugmode=[optional,Allow show some debug info text.]
        sdkpath=[optional,set the direcotry full name of ildasm.exe.]
        prefixfortyperename=[optional, the prefix use to rename type name.]
        prefixformemberrename=[optional,the prefix use to rename type's member name.]
        deletetempfile=[optional,delete template file after job finshed.default is false.]
        merge=[optional,some .net assembly file to merge to the result file. '*' for all referenced assembly files.]
        .custominstructurename=[optional, some custom IL instruction , for example '.subsystem=0x2'.]

     Example 1, protect d:\a.dll ,this will modify dll file.
        >JIEJIE.NET.exe d:\a.dll  
     Exmaple 2, anlyse d:\a.dll , and write result to another dll file with strong name. enable obfuscate control flow and not encript resources.
        >JIEJIE.NET.exe input=d:\a.dll output=d:\publish\a.dll snk=d:\source\company.snk switch=+contorlfow,-resources

```

## So many cool features! But JieJie.NET only has 14000 lines C# code!
   
   So it is small and without any third party component.

## In the future

   It will support upgrad assembly file from .NET Framework to .NET Core without any source code.Please pay attention to me.

## License
JieJie.NET use GPL-2.0 License. 


<hr />
JIEJIE.NET can save tens of thousands of US dollars for your team. so you can donate by <a href="https://www.paypal.com/paypalme/yuanyongfu">paypal</a> , by <a href="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/alipay.jpg">alipay</a> , by <a href="https://raw.githubusercontent.com/dcsoft-yyf/DCNETProtector/main/wechat_pay.png">Wechat</a>,help author to feed twins born in 2020.