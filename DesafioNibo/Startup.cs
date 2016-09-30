using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DesafioNibo.Startup))]
namespace DesafioNibo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
