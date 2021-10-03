using LoggerUtils;
using QuestionsWebApplication.Extensions;
using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;

namespace QuestionsWebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ContainerConfig.RegisterContainer(GlobalConfiguration.Configuration);
            ModelBinders.Binders.DefaultBinder = new QuestionsModelBinder();
            ViewEngines.Engines.Add(new QuestionsViewEngine());
        }

        protected void Application_BeginRequest(object pSender, EventArgs pEventArgs)
        {
            try
            {
                HttpCookie tCurrentLanguageCookie = Request.Cookies["Language"];

                if (tCurrentLanguageCookie != null && !string.IsNullOrEmpty(tCurrentLanguageCookie.Value))
                {
                    Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(tCurrentLanguageCookie.Value.Substring(0, 2));
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(tCurrentLanguageCookie.Value.Substring(0, 2));
                }
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }
    }
}
