using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EventsPlanning.Startup))]
namespace EventsPlanning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
