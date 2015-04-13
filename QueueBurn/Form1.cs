using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace QueueBurn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int b;
            if (IsNumeric(textBox1.Text) && IsNumeric(textBox2.Text) && IsNumeric(textBox4.Text))
            {
                //複製幾張
                int copy_num = Convert.ToInt16(textBox4.Text);
                //光碟機代號
                string DVD_Device = textBox5.Text;
                //燒錄起始號碼
                int burnStart = Convert.ToInt32(textBox1.Text);
                int burnsum = Convert.ToInt32(textBox2.Text) + 1;
                string burnsplit = String.Format("{0:00000000}", burnStart).Substring(6);
                int burnnumber = Convert.ToInt32(burnsplit);
                string burnnum = String.Format("{0:00000000}", burnStart).Substring(7);
                if (burnStart % 10 == 0)
                {
                    b = 10;
                }
                else
                {
                    b = Convert.ToInt32(burnnum);
                }
                
                if ((b + burnsum - 1) > 10)
                {
                    textBox3.Text += "錯誤!燒錄序號大於10片，您輸入的序號有誤，共" + (b + burnsum - 1) + "片";
                }
                else
                {
                    if (MessageBox.Show("您即將將要燒錄的DVD序號為:" + String.Format("{0:00000000}", burnStart) + " 至 " + String.Format("{0:00000000}", (burnStart + burnsum - 1)) + "是否確定?", "燒錄確認!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        setCmdtext(burnStart, burnsum, burnnumber,b,DVD_Device,copy_num);
                    }
                    else
                    {
                        textBox3.Text = "使用者取消";
                    }
                }

            }
            else
            {
                textBox3.Text = "非法字元!!請輸入數字!!";
            }

        }
        private void setCmdtext(int burnStart, int burnsum,int startnum,int b,string dvd,int copynum)
        {
            int nextnum = 0;
            string swfilename= "QueueBurn" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".cmd";
            string swpath = "C:\\Program Files\\ImgBurn\\" + swfilename;
            using (StreamWriter sw = new StreamWriter(swpath))
                {
                    sw.WriteLine("@ECHO OFF\r\necho Burning\r\n");
                    for (int i = startnum; i <= (startnum + burnsum - 1); i++)
                    {
                        sw.WriteLine("echo DVD" + String.Format("{0:00}", b + nextnum) + " Burning...");
                        string strData = "ImgBurn /MODE BUILD /BUILDMODE DEVICE /SPEED MAX /SRC \"D:\\DVD" + String.Format("{0:00}", b + nextnum) + "\" /DEST \""+ dvd + "\\\" /FILESYSTEM \"ISO9660 + Joliet + UDF\" /VOLUMELABEL \"" + String.Format("{0:00000000}", (burnStart + nextnum)) + "\" /ROOTFOLDER YES /COPIES "+ copynum +" /VERIFY YES /EJECT YES /INCLUDEHIDDENFILES YES /NOSAVESETTINGS /WAITFORMEDIA /START /NOIMAGEDETAILS /CLOSESUCCESS ";
                        sw.WriteLine(strData);
                        nextnum++;
                    }
                    sw.WriteLine("echo Brun Success!!");
                    sw.WriteLine("echo pasue");
                }
            if (Startcmd(swfilename))
            {
                    textBox3.Text = "開始燒錄...";
            }
        }


        private bool Startcmd(string filenamestr)
        {
            try
            {
                ProcessStartInfo Info2 = new ProcessStartInfo();

                Info2.FileName = filenamestr;//執行的檔案名稱
                textBox3.Text += filenamestr;
                Info2.WorkingDirectory = @"C:\Program Files\ImgBurn\";//檔案所在的目錄

                Process.Start(Info2);
                return true;
            }
            catch (Exception e)
                {
                    textBox3.Text += e.ToString();
                    return false;
                }
        }

        static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1
                = new System.Text.RegularExpressions.Regex(@"^[-]?\d+[.]?\d*$");
            return reg1.IsMatch(str);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Label [] labAry = new Label [] {label5,label6,label7,label8,label9,label10,label11,label12,label13,label14};
            for (int i = 1; i <= 10; i++)
            {
                string DVDnumber = "DVD" + String.Format("{0:00}", i);
                DirectoryInfo dir  = new DirectoryInfo("D:\\" + DVDnumber);
                if (getallfilesize(dir) > 4.36)
                {
                    labAry[i - 1].ForeColor = Color.Red;
                }
                else
                {
                    labAry[i - 1].ForeColor = Color.Black;
                }
                labAry[i -1].Text = getallfilesize(dir).ToString("0.00") + "GB";
            }

            MessageBox.Show("讀取DVD容量完成!!,若有[紅字]請台腦處理");
            
        }
        private double getallfilesize(DirectoryInfo dir)
        {
            long size = 0;
            foreach (FileInfo fileInfo in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                size += fileInfo.Length;
            }
            double sizeG = size / (1024.0 * 1024.0 * 1024.0);
            return sizeG;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
