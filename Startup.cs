using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BugTracker2.Startup))]
namespace BugTracker2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
