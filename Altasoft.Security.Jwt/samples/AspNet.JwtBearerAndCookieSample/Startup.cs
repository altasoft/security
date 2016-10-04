using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AspNet.JwtBearerAndCookieSample.Startup))]

namespace AspNet.JwtBearerAndCookieSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
