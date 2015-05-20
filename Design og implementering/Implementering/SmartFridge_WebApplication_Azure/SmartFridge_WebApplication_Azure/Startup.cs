using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmartFridge_WebApplication_Azure.Startup))]
namespace SmartFridge_WebApplication_Azure
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
