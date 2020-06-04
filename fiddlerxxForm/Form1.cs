using ElmahFiddler;
using Fiddler;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fiddlerxxForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] files = e.Argument as string[];
            foreach(var f in files)
            {
                log("开始处理:" + f);
                try
                {
                    doProc(f, int.Parse(textBox1.Text), textBox2.Text.Trim());
                }catch(Exception ex)
                {
                    log(ex.Message);
                }
                log("结束处理:" + f);

            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("处理完成");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Multiselect = true;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] files = this.openFileDialog1.FileNames;
                this.backgroundWorker1.RunWorkerAsync(files);
            }

            
        }
     
        private void log(string info)
        {
            this.Invoke(new Action(()=> { this.richTextBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + info + "\r\n"); }));
        }
        private static void doProc(string file, int timeout, string keyword = "")
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("文件不存在");
                return;
            }
            var results = test(file, "");
            StringBuilder output = new StringBuilder();
            foreach (var r in results)
            {
                System.Diagnostics.Trace.WriteLine(r.oResponse.MIMEType);
                if (r.oResponse.MIMEType.Contains("image/png") || r.oResponse.MIMEType.Contains("image/jpeg")
                    || r.oResponse.MIMEType.Contains("image/gif") || r.oResponse.MIMEType.Contains("application/x-javascript")
                    || r.oResponse.MIMEType.Contains("text/css"))
                {
                    continue;
                }
                string msg = ("\r\n时间:") + (r.Timers.ServerConnected.ToString("yyyy-MM-dd HH:mm:ss")) + "\r\nID:" + r.id + ("\r\n请求URL:\r\n") + r.fullUrl + ("\r\n请求内容:\r\n") + (r.GetRequestBodyAsString()) + ("\r\n响应时间:") + (r.oResponse.iTTLB) + ("毫秒\r\n");

                if (r.oResponse.iTTLB > timeout)
                {
                    if (keyword.Length > 0 && r.fullUrl.ToLower().IndexOf(keyword.ToLower()) == -1) continue;
                    output.Append(msg);
                    Console.WriteLine(msg);
                    //sql.Append("insert into sfxfiddler values(");
                    //sql.Append("'");
                    //sql.Append(r.Timers.ServerConnected.ToString("yyyy-MM-dd HH:mm:ss"));
                    //sql.Append("',");

                    //sql.Append("'");
                    //sql.Append(r.fullUrl);
                    //sql.Append("',");

                    //sql.Append("'");
                    //sql.Append(r.GetRequestBodyAsString());
                    //sql.Append("',");

                    //sql.Append("'");
                    //sql.Append(r.GetResponseBodyAsString().Replace("'", ""));
                    //sql.Append("',");

                    //sql.Append("'");
                    //sql.Append(r.oResponse.iTTLB);
                    //sql.Append("'");

                    //sql.Append(");\r\n");
                }


            }
            File.AppendAllText(file+".txt", output.ToString(), Encoding.UTF8);
            //File.AppendAllText("fiddler.sql", sql.ToString(), Encoding.UTF8);
        }

        private static IEnumerable<Session> test(string sazFile, string password)
        {
            List<Session> result = new List<Session>();
            using (ZipFile zip = ZipFile.Read(sazFile))
            {
                //result = (from z in zip.Entries
                //          where z.FileName.EndsWith("_c.txt")
                //          select z.ExtractWithPasswordToBytes(password) into request
                //          select new Session(request, new byte[0])).ToList<Session>();

                //foreach (var z in zip.Entries)
                //{
                //    if (z.FileName.EndsWith("_c.txt")) {
                //        var req = z.ExtractWithPasswordToBytes(password);
                //    }

                //}
                for (int i = 1; i < zip.Entries.Count; i += 3)
                {
                    if (zip[i].FileName.EndsWith(".txt"))
                    {
                        Session s = new Session(zip[i].ExtractWithPasswordToBytes(password), zip[i + 2].ExtractWithPasswordToBytes(password));
                        s.LoadMetadata(zip[i + 1].ExtractWithPassword2(password));
                        result.Add(s);
                    }

                }

            }
            return result;
        }
    }
}
