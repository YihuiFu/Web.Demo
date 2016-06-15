using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Common.Logging;
using Foyerry.Server.Common;

namespace Foyerry.Server.Demo.Controllers
{
    public class HomeController : Controller
    {
        protected ILog _log = LogManager.GetLogger(typeof(HomeController));
        public ActionResult Index()
        {
            _log.Info("测试");
            Test.Do();
            ViewBag.Message = "Modify ：" + DateTime.Now.ToLongTimeString();
            for (int i = 0; i < 100; i++)
            {
                Logger.Instance.Write("测试日志队列：" + i + "", MessageType.Info);
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
