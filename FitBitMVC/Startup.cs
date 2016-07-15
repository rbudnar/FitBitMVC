using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FitBitMVC.Startup))]
namespace FitBitMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
