using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Secure_Password_Repository
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            /*
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            */

            //front page content
            routes.MapRoute("Home", "", new { controller = "Home", action = "Index" });

            //Content Management

            //passwords
            routes.MapRoute("Password", "Password", new { controller = "Password", action = "Index" });
            routes.MapRoute("PasswordIndex", "Password/Index", new { controller = "Password", action = "Index" });
            routes.MapRoute("NewCategory", "Password/AddCategory/{ParentId}", new { controller = "Password", action = "AddCategory", ParentId = UrlParameter.Optional, CategoryName = UrlParameter.Optional });
            routes.MapRoute("UpdateCategoryPosition", "Password/UpdateCategoryPosition/{categoryid}", new { controller = "Password", action = "UpdateCategoryPosition", CategoryId = UrlParameter.Optional, NewPosition = UrlParameter.Optional });

            //account handling
            routes.MapRoute("Login", "Login/{returnUrl}", new { controller = "Account", action = "Login", returnUrl = "Password" });
            routes.MapRoute("AccountLogin", "Account/Login/{returnUrl}", new { controller = "Account", action = "Login", returnUrl = "Password" });
            routes.MapRoute("Register", "Register", new { controller = "Account", action = "Register" });
            routes.MapRoute("LogOff", "LogOff", new { controller = "Account", action = "LogOff" });
            routes.MapRoute("RegistrationConfirmation", "RegistrationConfirmation", new { controller = "Account", action = "RegistrationConfirmation" });

            //account managment
            routes.MapRoute("Manage", "Manage", new { controller = "Account", action = "Manage" });
            routes.MapRoute("UserManagerIndex", "UserManager/Index", new { controller = "UserManager", action = "Index" });
            routes.MapRoute("UserManager", "UserManager", new { controller = "UserManager", action = "Index" });
            routes.MapRoute("UserManagerEdit", "UserManager/Edit/{id}", new { controller = "UserManager", action = "Edit", Id = UrlParameter.Optional });
            routes.MapRoute("UserManagerResetPassword", "UserManager/ResetPassword/{id}", new { controller = "UserManager", action = "ResetPassword", Id = UrlParameter.Optional });
            routes.MapRoute("UserManagerDelete", "UserManager/Delete/{id}", new { controller = "UserManager", action = "Delete", Id = UrlParameter.Optional });

        }
    }
}
