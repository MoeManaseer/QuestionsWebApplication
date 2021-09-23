using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(QuestionsWebApplication.Startup))]
namespace QuestionsWebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
