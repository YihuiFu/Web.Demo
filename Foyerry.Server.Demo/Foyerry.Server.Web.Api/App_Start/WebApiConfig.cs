using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Foyerry.Server.Web.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 取消注释下面的代码行可对具有 IQueryable 或 IQueryable<T> 返回类型的操作启用查询支持。
            // 若要避免处理意外查询或恶意查询，请使用 QueryableAttribute 上的验证设置来验证传入查询。
            // 有关详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=279712。
            config.EnableQuerySupport();

            // 若要在应用程序中禁用跟踪，请注释掉或删除以下代码行
            // 有关详细信息，请参阅: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //时间去掉UTC
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

            // ConfigureApi(config);

            // routes config
            config.Routes.MapHttpRoute(
            name: "特殊处理",
            routeTemplate: "haha/hehe/{id}",
            defaults: new
            {
                controller = "User",
                action = "Detail"
            },
            constraints: new { id = @"[3-9]{1,200}" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


        }

        public static void ConfigureApi(HttpConfiguration config)
        {
            var jsonFormatter = new JsonMediaTypeFormatter();
            var settings = jsonFormatter.SerializerSettings;
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss" };
            //这里使用自定义日期格式
            settings.Converters.Add(timeConverter);
            //settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));
        }
        public class JsonContentNegotiator : IContentNegotiator
        {
            private readonly JsonMediaTypeFormatter _jsonFormatter;
            public JsonContentNegotiator(JsonMediaTypeFormatter formatter)
            {
                _jsonFormatter = formatter;
            }
            public ContentNegotiationResult Negotiate(Type type, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
            {
                var result = new ContentNegotiationResult(_jsonFormatter, new MediaTypeHeaderValue("application/json"));
                return result;
            }
        }
    }
}
