using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Foyerry.Server.Common
{
    public class LoggerManager : ILogger
    {
        public static readonly TextWriter Writer = Console.Out;

        public void WriteFileLog(Exception ex)
        {
            WriteFileLog("Message:" + ex.Message + "\r\n\r\nStackTrace:\r\n" + ex.StackTrace, "Log", "Exception_", false);
        }

        public void WriteFileLog(string msg, string prefix)
        {
            WriteFileLog(msg, "Log", prefix, false);
        }

        public void WriteFileLog(string msg, string dir, string logfiePrefix, bool isThrowException)
        {
            string text = string.Empty;
            if (string.IsNullOrEmpty(dir))
            {
                dir = "Log";
            }
            if (dir.IndexOf(":") > -1)
            {
                text = dir;
            }
            else if (HttpContext.Current == null)
            {
                text = AppDomain.CurrentDomain.BaseDirectory + dir;
            }
            else
            {
                text = HttpContext.Current.Request.PhysicalApplicationPath + dir;
            }
            text += DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            try
            {
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                logfiePrefix = (string.IsNullOrEmpty(logfiePrefix) ? DateTime.Now.ToString("yyyyMMdd") : logfiePrefix);
                using (StreamWriter streamWriter = new StreamWriter(text + "\\" + logfiePrefix + ".txt", true, Encoding.UTF8))
                {
                    streamWriter.WriteLine(string.Concat(new string[]
					{
						"========== Time:",
						DateTime.Now.ToString(),
						":",
						DateTime.Now.Millisecond.ToString(),
						"=========="
					}));
                    streamWriter.WriteLine(msg);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                if (isThrowException)
                {
                    throw ex;
                }
            }
        }

        public void Log(string message)
        {
            WriteFileLog(message, "Info_");
        }

        public void Log(string[] categories, string message)
        {
            foreach (var category in categories)
            {
                WriteFileLog(category, "Cate_");
            }
        }

        public void LogException(Exception x)
        {
            WriteFileLog(x);
        }
    }
}
