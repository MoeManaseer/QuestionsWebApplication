using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using LoggerUtils;
using QuestionsController;
using System;
using System.Web.Http;
using System.Web.Mvc;

namespace QuestionsWebApplication
{
    internal class ContainerConfig
    {
        internal static void RegisterContainer(HttpConfiguration pHttpConfiguration)
        {
            try
            {
                var tBuilder = new ContainerBuilder();

                tBuilder.RegisterControllers(typeof(MvcApplication).Assembly);
                tBuilder.RegisterApiControllers(typeof(MvcApplication).Assembly);
                tBuilder.RegisterType<QuestionsHandler>()
                    .As<QuestionsHandler>()
                    .SingleInstance();

                var tContainer = tBuilder.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(tContainer));
                pHttpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(tContainer);
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }
    }
}