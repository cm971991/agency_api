using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace agency
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.EnableCors();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Log4net.config")));
        }

        /// <summary>
        /// 捕获全局错误并记录日志
        /// </summary>
        protected void Application_Error()
        {
            #region LOG4NET错误日志记录
            LogManager.GetLogger(Request.Url.PathAndQuery).Error("error", Server.GetLastError().GetBaseException());
            #endregion
            Server.ClearError();
            // Response.Redirect("~/");
        }
    }
}
