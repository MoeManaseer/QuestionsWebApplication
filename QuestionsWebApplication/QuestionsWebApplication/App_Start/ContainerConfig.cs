using Autofac;
using Autofac.Integration.Mvc;
using LoggerUtils;
using QuestionsController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuestionsWebApplication
{
    public static class ContainerConfig
    {
        internal static void RegisterContainer()
        {
            try
            {
                var tBuilder = new ContainerBuilder();

                tBuilder.RegisterControllers(typeof(MvcApplication).Assembly);
                tBuilder.RegisterType<QuestionsHandler>()
                    .As<QuestionsHandler>()
                    .InstancePerRequest();

                var tContainer = tBuilder.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(tContainer));
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }
    }
}