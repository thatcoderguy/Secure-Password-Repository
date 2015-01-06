using LightInject;
using Microsoft.AspNet.Identity.EntityFramework;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Identity;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Repositories;
using Secure_Password_Repository.Services;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;

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
            container.Register<IPasswordPermissionService, PasswordPermissionService>(new PerScopeLifetime());
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

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                CustomPrincipal currentUser = new CustomPrincipal(authTicket.Name);

                HttpContext.Current.User = currentUser;
            }
        }

    }
}
