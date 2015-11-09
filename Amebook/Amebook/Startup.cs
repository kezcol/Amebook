using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Amebook.Startup))]
namespace Amebook
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
