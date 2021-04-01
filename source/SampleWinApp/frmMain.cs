using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
 
using System.Text;
using System.Windows.Forms;

namespace SampleWinApp
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Text = "Start at:" + DateTime.Now.ToLongTimeString();
        }

private void btnAbout_Click(object sender, EventArgs e)
{
    MessageBox.Show(this, GetLicenseMessage());
}
         
private string GetLicenseMessage()
{
    var str = "DC.NET Protector Options:HiddenAllocationCallStack";
    string msg = "This software license to :" + Environment.UserName;
    return msg;
}


        private void btnDoWork_Click(object sender, EventArgs e)
        {
            int tick = Environment.TickCount;
            for( int iCount = 0; iCount < 10000; iCount ++ )
            {
                var bs = ParseUpperHexString("123213214abdef979879873243243232432432123213214abdef979879873243243232432432123213214abdef979879873243243232432432123213214abdef979879873243243232432432123213214abdef979879873243243232432432");
            }
            tick = Environment.TickCount - tick;
            MessageBox.Show("span tick:" + tick);
        }

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
        private string aaa()
        {
            int v = Environment.TickCount;
            if(v==1)
            {
                return "aaa";
            }
            else
            {
                return v.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, Resource1.StringValue);
        }
    }
}
