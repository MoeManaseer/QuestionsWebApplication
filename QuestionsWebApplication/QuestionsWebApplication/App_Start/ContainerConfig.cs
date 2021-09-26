using Autofac;
using Autofac.Integration.Mvc;
using LoggerUtils;
using QuestionsController;
using System;
using System.Web.Mvc;

namespace QuestionsWebApplication
{
    internal class ContainerConfig
    {
        internal static void RegisterContainer()
        {
            try
            {
                var tBuilder = new ContainerBuilder();

                tBuilder.RegisterControllers(typeof(MvcApplication).Assembly);
                tBuilder.RegisterType<QuestionsHandler>()
                    .As<QuestionsHandler>()
                    .SingleInstance();

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