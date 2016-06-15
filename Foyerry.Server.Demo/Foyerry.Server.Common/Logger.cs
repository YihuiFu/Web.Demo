using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Web;
using Common.Logging;

namespace Foyerry.Server.Common
{
    public class Logger : IDisposable
    {
        private static ILog _fileLog = LogManager.GetLogger("日志记录");

        private static Logger _log = null;
        private static object _objLock = new object();
        //Log Message queue
        private Queue<LogMessage> _logMessages;
        //log write file state
        private bool _state;

        private Semaphore _semaphore;
        public static Logger Instance
        {
            get
            {
                lock (_objLock)
                {
                    return _log ?? (_log = new Logger());
                }
            }
        }

        private Logger()
        {
            lock (_objLock)
            {
                try
                {
                    _state = true;
                    _semaphore = new Semaphore(0, int.MaxValue);
                    _logMessages = new Queue<LogMessage>();
                    var thread = new Thread(Work) { IsBackground = true };
                    thread.Start();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Write Log file  work method
        /// </summary>
        private void Work()
        {
            while (true)
            {
                if (_logMessages.Count <= 0)
                {
                    if (WaitLogMessage())
                        break;
                }
                else
                {
                    WriteFileMessage();
                }
            }
        }

        /// <summary>
        /// Write message to log file
        /// </summary>
        private void WriteFileMessage()
        {
            LogMessage logMessage = null;
            lock (_objLock)
            {
                if (_logMessages.Count > 0)
                    logMessage = _logMessages.Dequeue();
                if (logMessage != null)
                {
                    switch (logMessage.Type)
                    {
                        case MessageType.Debug:
                            _fileLog.Debug(logMessage.ToString());
                            break;
                        case MessageType.Info:
                            _fileLog.Info(logMessage.ToString());
                            break;
                        case MessageType.Error:
                            _fileLog.Error(logMessage.ToString());
                            break;
                        case MessageType.Warn:
                            _fileLog.Warn(logMessage.ToString());
                            break;
                        case MessageType.Fatal:
                            _fileLog.Fatal(logMessage.ToString());
                            break;
                    }
                    //-------------------------------------
                    //---- Write messge into file or database
                    //-------------------------------------
                    //  FileWrite(logMessage);
                }

            }
            //
            //var logMessage = new List<LogMessage>();
            //lock (_objLock)
            //{
            //    while (_logMessages.Count > 0)
            //    {
            //        logMessage.Add(_logMessages.Dequeue());
            //    }
            //}
            //try
            //{

            //    switch (logMessage)
            //    {

            //    }
            //    _fileLog.Debug("");

            //}
            //catch
            //{
            //    foreach (var item in logMessage)
            //    {
            //        _logMessages.Enqueue(item);
            //    }
            //}
        }


        /// <summary>
        /// The thread wait a log message
        /// </summary>
        /// <returns>is close or not</returns>
        private bool WaitLogMessage()
        {
            //determine log life time is true or false
            if (!_state) return true;
            WaitHandle.WaitAny(new WaitHandle[] { _semaphore }, -1, false);
            return false;
        }

        /// <summary>
        /// Enqueue a new log message and release a semaphore
        /// </summary>
        /// <param name="msg">Log message</param>
        public void Write(LogMessage msg)
        {
            if (msg == null) return;
            lock (_objLock)
            {
                var traceFrame = new StackTrace().GetFrames();
                if (traceFrame != null)
                {
                    foreach (var t in traceFrame)
                    {
                        if (t.GetMethod().DeclaringType == null || t.GetMethod().DeclaringType == typeof(AOP) ||
                            t.GetMethod().DeclaringType == Logger.Instance.GetType()) continue;
                        var declaringType = t.GetMethod().DeclaringType;
                        if (declaringType != null)
                            msg.NameSpace = declaringType.FullName;
                        msg.MethodName = t.GetMethod().Name;
                        break;
                    }
                }
                msg.LogTime = DateTime.Now;
                msg.ClientIP = "";// IP.GetIP();
                msg.AppName = "";
                while (_logMessages.Count > 1000)
                {
                    _logMessages.Dequeue();
                }
                _logMessages.Enqueue(msg);
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Write message by message content and type
        /// </summary>
        /// <param name="text">log message</param>
        /// <param name="type">message type</param>
        public void Write(string text, MessageType type)
        {
            Write(new LogMessage(text, type));
        }

        /// <summary>
        /// Write Message by datetime and message content and type
        /// </summary>
        /// <param name="dateTime">datetime</param>
        /// <param name="text">message content</param>
        /// <param name="type">message type</param>
        public void Write(DateTime dateTime, string text, MessageType type)
        {
            Write(new LogMessage(dateTime, text, type));
        }

        /// <summary>
        /// Write message ty exception and message type 
        /// </summary>
        /// <param name="e">exception</param>
        /// <param name="type">message type</param>
        public void Write(Exception e, MessageType type)
        {
            var msg = new LogMessage(e.Message, type);
            if (e.InnerException != null)
            {
                msg.BriefInfo = e.InnerException.Message;
                msg.DetailInfo = e.StackTrace;
            }
            else
            {
                msg.BriefInfo = e.Message;
                msg.DetailInfo = e.StackTrace;
            }

            Write(msg);
        }
        public void LogWebErrors(HttpContext context)
        {
            //获得所有未处理的Exception集
            var errors = context.AllErrors;
            if (errors == null) return;
            foreach (var item in errors)
            {
                var msg = new LogMessage();
                var sb = new StringBuilder();
                sb.Append("地址:" + context.Request.Url.ToString() + "\r\n");
                if (context.Request.UrlReferrer != null)
                {
                    sb.Append("引用:" + context.Request.UrlReferrer.ToString() + "\r\n");
                }
                foreach (var item1 in context.Request.Form.AllKeys)
                {
                    sb.Append(string.Format("{0}:{1}\r\n", item1, context.Request.Form[item1]));
                }
                if (item.InnerException != null)
                {
                    msg.BriefInfo = item.InnerException.Message;
                    sb.AppendLine("错误信息:" + item.InnerException.Message + "\r\n" + item.InnerException.StackTrace);
                }
                else
                {
                    msg.BriefInfo = item.Message;
                    sb.AppendLine("错误信息:" + item.Message + "\r\n" + item.StackTrace);
                }
                // Could record session or cookie 

                msg.Crawler = HttpContext.Current.Request.Browser.Crawler;
                msg.ClientIP = "";//Common.IP.GetWanIp();
                msg.DetailInfo = sb.ToString();
                msg.Type = MessageType.Error;
                Logger.Instance.Write(msg);
            }
        }

        #region --IDisposable
        /// <summary>
        /// Dispose log
        /// </summary>
        public void Dispose()
        {
            _state = false;
        }
        #endregion
    }

    public class LogMessage
    {
        public LogMessage()
            : this("", MessageType.Unknown)
        {
        }
        public LogMessage(string text, MessageType messageType)
            : this(DateTime.Now, text, messageType)
        {
        }

        public LogMessage(DateTime dateTime, string text, MessageType messageType)
        {
            LogTime = dateTime;
            Type = messageType;
            BriefInfo = text;
        }
        public string AppName { get; set; }
        public string NameSpace { get; set; }
        public string MethodName { get; set; }
        public DateTime LogTime { get; set; }
        public string BriefInfo { get; set; }
        public string DetailInfo { get; set; }
        public MessageType Type { get; set; }
        public string ClientIP { get; set; }
        public bool Crawler { get; set; }

        public new string ToString()
        {
            return "Time:" + LogTime.ToString(CultureInfo.InvariantCulture) + "\r\nType:" + Type + "\r\nAppInfo:" + AppName + "\t" + NameSpace + "\t" + MethodName + "\r\nMsg:"
                + BriefInfo + "\r\nDetail:" + DetailInfo + "\r\nIP:" + ClientIP;
        }
    }


    /// <summary>
    /// Log Message Type enum
    /// </summary>
    public enum MessageType
    {
        Information = 0,
        HowLong,
        Success,
        Static,
        Warning,

        Debug,
        Info,
        Warn,
        Error,
        Fatal,
        Unknown,

    }
}
