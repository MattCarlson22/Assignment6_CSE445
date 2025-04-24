using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace TextToPdfTryIt
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }
        protected void Application_BeginRequest()
        {
            var ctx = HttpContext.Current;
            var path = ctx.Request.AppRelativeCurrentExecutionFilePath;

            // Only guard our specific action
            if (!path.StartsWith("~/api/pdf/convert-file",
                                 StringComparison.OrdinalIgnoreCase))
                return;

            var ctype = (ctx.Request.ContentType ?? "").ToLowerInvariant();

            // Allow the normal browser upload envelope:
            if (ctype.StartsWith("multipart/form-data"))
                return;   // let the controller validate inside the multipart parts

            // Reject *direct* raw uploads of PDFs or anything else
            ctx.Response.StatusCode = 415;           // Unsupported Media Type
            ctx.Response.End();                      // stop pipeline right here
        }

    }
}