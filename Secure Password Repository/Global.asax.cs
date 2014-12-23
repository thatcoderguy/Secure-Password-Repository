using LightInject;
using Microsoft.AspNet.Identity.EntityFramework;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Repositories;
using Secure_Password_Repository.Services;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Secure_Password_Repository
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new ServiceContainer();
            container.RegisterControllers();        

            container.Register<IAccountService, AccountService>(new PerScopeLifetime());
            container.Register<ICategoryRepository, CategoryRepository>(new PerScopeLifetime());
            container.Register<IPasswordRepository, PasswordRepository>(new PerScopeLifetime());
            container.Register<IUserPasswordRepository, UserPasswordRepository>(new PerScopeLifetime());

            container.Register<IViewModelValidatorService, ViewModelValidatorService>(new PerScopeLifetime());
            container.Register<IPermissionService, PermissionService>(new PerScopeLifetime());
            container.Register<IViewModelService, ViewModelService>(new PerScopeLifetime());

            container.Register<ApplicationDbContext, ApplicationDbContext>(new PerScopeLifetime());
            container.Register<ApplicationUserManager, ApplicationUserManager>(new PerScopeLifetime());

            container.EnableMvc();
        }

        //make sure the session ID persists between user requests
        protected void Session_Start()
        {
            Session["PersistSessionId"] = true;
        }

    }
}
