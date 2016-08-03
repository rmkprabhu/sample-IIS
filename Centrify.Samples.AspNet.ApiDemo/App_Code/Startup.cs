using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(API_Demo.Startup))]
namespace API_Demo
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
