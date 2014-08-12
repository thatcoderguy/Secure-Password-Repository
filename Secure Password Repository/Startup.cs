using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Secure_Password_Repository.Startup))]
namespace Secure_Password_Repository
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
