using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Common.Logging;
using Foyerry.Server.Common;
using Foyerry.Server.Demo.Filters;
using Foyerry.Server.Demo.Models;

namespace Foyerry.Server.Demo.Controllers
{
    public class HomeController : Controller
    {
        protected ILog _log = LogManager.GetLogger(typeof(HomeController));
        public ActionResult Index()
        {
            //_log.Info("测试");
            ////Test.Do();
            //ViewBag.Message = "Modify ：" + DateTime.Now.ToLongTimeString();
            //for (int i = 0; i < 100; i++)
            //{
            //    Logger.Instance.Write("测试日志队列：" + i + "", MessageType.Info);
            //}



            //var arr = new[] { 1, 2, 3 };
            //for (int i = 0; i < 4; i++)
            //{
            //    var r = arr[i] * 5;
            //}

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


        [GlobalExceptionLog(Order = 1, ExceptionType = typeof(NullReferenceException))]
        public ActionResult Test()
        {
            ViewBag.Message = "测试";
            return View();
        }

        [HttpGet]
        public ActionResult GetDate()
        {
            ViewBag.date = DateTime.Now.ToLongTimeString();
            return View();
        }
        public ActionResult PostDate(string UserName)
        {
            return Content(UserName + ":" + DateTime.Now.ToString());
        }

    }
}
