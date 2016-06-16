using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;

namespace Foyerry.Server.Web.Api
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //   5339.46  416.6  35
    //        GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings =
    //new JsonSerializerSettings
    //{
    //    DateFormatHandling = DateFormatHandling.IsoDateFormat,
    //    DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
    //    Culture = CultureInfo.GetCultureInfo("fr-FR")
    //};

            //JsonMediaTypeFormatter jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //JsonSerializerSettings jSettings = new Newtonsoft.Json.JsonSerializerSettings()
            //{
            //    Formatting = Formatting.Indented,
            //    DateTimeZoneHandling = DateTimeZoneHandling.Utc
            //};
            //jSettings.Converters.Add(new MyDateTimeConvertor());
            //jsonFormatter.SerializerSettings = jSettings;

        }
    }
}