using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Logging;

namespace Foyerry.Server.Demo.Filters
{
    public class GlobalExceptionLogAttribute : HandleErrorAttribute
    {
        protected ILog _log = LogManager.GetLogger(typeof(GlobalExceptionLogAttribute));
        public override void OnException(ExceptionContext filterContext)
        {
            var message = string.Format("\r\n消息类型：{0}\r\n消息内容：{1}\r\n引发异常的方法：{2}\r\n引发异常的对象：{3}\r\n异常目录：{4}\r\n异常文件：{5}\r\nStackTrace:{6}"
                                        , filterContext.Exception.GetType().Name
                                        , filterContext.Exception.Message
                                        , filterContext.Exception.TargetSite
                                        , filterContext.Exception.Source
                                        , filterContext.RouteData.GetRequiredString("controller")
                                        , filterContext.RouteData.GetRequiredString("action")
                                        , filterContext.Exception.StackTrace
                                        );
            _log.Warn(message); //记录日志到文件
            //filterContext.ExceptionHandled = true;
            //filterContext.Result = new RedirectResult("http://www.baidu.com");
            base.OnException(filterContext);
        }
    }
}