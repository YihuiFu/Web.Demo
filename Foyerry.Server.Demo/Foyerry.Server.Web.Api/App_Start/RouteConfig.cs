using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Foyerry.Server.Web.Api
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //添加自定义路由
           // routes.Add(new CustomRoute());

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }

    public class CustomRoute : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath + httpContext.Request.PathInfo;
            var cateName = virtualPath.Substring(2).Trim('/');//此时URL会是～/ca-categoryname，截取后面的ca-categoryname

            if (string.IsNullOrEmpty(cateName))
            {
                return null;
            }
            var routeData = new RouteData(this, new MvcRouteHandler());
            routeData.Values.Add("controller","");
            routeData.Values.Add("action","");
            routeData.Values.Add("id",0);
            return routeData;
        }

        /// <summary>
        ///  用于匹配类似 url.action("","") 这种生成的链接
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            //判断请求是否来源于CategoryController.Showcategory(string id),不是则返回null,让匹配继续
            var categoryId = values["id"] as string;

            if (categoryId == null)//路由信息中缺少参数id，不是我们要处理的请求，返回null
                return null;

            //请求不是CategoryController发起的，不是我们要处理的请求，返回null
            if (!values.ContainsKey("controller") || !values["controller"].ToString().Equals("category", StringComparison.OrdinalIgnoreCase))
                return null;
            //请求不是CategoryController.Showcategory(string id)发起的，不是我们要处理的请求，返回null
            if (!values.ContainsKey("action") || !values["action"].ToString().Equals("showcategory", StringComparison.OrdinalIgnoreCase))
                return null;

            //至此，我们可以确定请求是CategoryController.Showcategory(string id)发起的，生成相应的URL并返回
            //var category = CategoryManager.AllCategories.Find(c => c.CategoeyID == categoryId);

            //if (category == null)
            //    throw new ArgumentNullException("category");//找不到分类抛出异常

            //var path = "ca-" + category.CategoeyName.Trim();//生成URL
            var path = "123";

            return new VirtualPathData(this, path.ToLowerInvariant());
        }
    }

    public class RewriteModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}