using ElmahFiddler;
using Fiddler;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fiddlerxx
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var results = test(@" E:\360data\重要数据\桌面\临时\20171010.saz", "");
            StringBuilder output = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            sql.Append("--create table sfxfiddler(时间 datetime,请求url varchar(2000),请求内容 varchar(max),响应内容 varchar(max),耗时毫秒 int); \r\n");
            foreach (var r in results)
            {
                System.Diagnostics.Trace.WriteLine(r.oResponse.MIMEType);
                if (r.oResponse.MIMEType.Contains("image/png") ||r.oResponse.MIMEType.Contains("image/jpeg") 
                    || r.oResponse.MIMEType.Contains("image/gif") || r.oResponse.MIMEType.Contains("application/x-javascript")
                    || r.oResponse.MIMEType.Contains("text/css"))
                {
                    continue;
                }
                string msg = ("\r\n时间:") + (r.Timers.ServerConnected.ToString("yyyy-MM-dd HH:mm:ss")) + ("\r\n请求内容\r\n") + (r.GetRequestBodyAsString()) + ("\r\n响应时间") + (r.oResponse.iTTLB) + ("毫秒\r\n");
                Console.WriteLine(msg);
                output.Append(msg);
                sql.Append("insert into sfxfiddler values(");
                sql.Append("'");
                sql.Append(r.Timers.ServerConnected.ToString("yyyy-MM-dd HH:mm:ss"));
                sql.Append("',");

                sql.Append("'");
                sql.Append(r.fullUrl);
                sql.Append("',");

                sql.Append("'");
                sql.Append(r.GetRequestBodyAsString());
                sql.Append("',");

                sql.Append("'");
                sql.Append(r.GetResponseBodyAsString().Replace("'",""));
                sql.Append("',");

                sql.Append("'");
                sql.Append(r.oResponse.iTTLB);
                sql.Append("'");

                sql.Append(");\r\n");

            }
            File.AppendAllText("fiddler.txt", output.ToString(), Encoding.UTF8);
            File.AppendAllText("fiddler.sql", sql.ToString(), Encoding.UTF8);
            Console.ReadLine();
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
