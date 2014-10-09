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

            //passwords & categories
            routes.MapRoute("Password", "Password", new { controller = "Password", action = "Index" });
            routes.MapRoute("PasswordIndex", "Password/Index", new { controller = "Password", action = "Index" });

            //categories
            routes.MapRoute("NewCategory", "Password/AddCategory", new { controller = "Password", action = "AddCategory" });
            routes.MapRoute("EditCategory", "Password/EditCategory", new { controller = "Password", action = "EditCategory" });
            routes.MapRoute("DeleteCategory", "Password/DeleteCategory/{categoryid}", new { controller = "Password", action = "DeleteCategory", CategoryId = UrlParameter.Optional });
            routes.MapRoute("GetCategoryChildren", "Password/GetCategoryChildren/{parentcategoryid}", new { controller = "Password", action = "GetCategoryChildren", ParentCategoryId = UrlParameter.Optional });
            routes.MapRoute("UpdatePosition", "Password/UpdatePosition/{categoryid}", new { controller = "Password", action = "UpdatePosition", CategoryId = UrlParameter.Optional, NewPosition = UrlParameter.Optional, isCategoryItem = UrlParameter.Optional });

            //passwords
            routes.MapRoute("AddPassword", "Password/AddPassword/{parentcategoryid}", new { controller = "Password", action = "AddPassword", ParentCategoryId = UrlParameter.Optional });
            routes.MapRoute("DeletePassword", "Password/DeletePassword/{passwordid}", new { controller = "Password", action = "DeletePassword", PasswordId = UrlParameter.Optional });
            routes.MapRoute("EditPassword", "Password/EditPassword/{passwordid}", new { controller = "Password", action = "EditPassword", PasswordId = UrlParameter.Optional });
            routes.MapRoute("ViewPassword", "Password/ViewPassword/{passwordid}", new { controller = "Password", action = "ViewPassword", PasswordId = UrlParameter.Optional });
            routes.MapRoute("GetEncryptedPassword", "Password/GetEncryptedPassword/{passwordid}", new { controller = "Password", action = "GetEncryptedPassword", PasswordId = UrlParameter.Optional });
            routes.MapRoute("EditUserPermissions", "Password/EditUserPermissions/{passwordid}", new { controller = "Password", action = "EditUserPermissions", PasswordId = UrlParameter.Optional });
            
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

            //system setting
            routes.MapRoute("SystemSetting", "SystemSetting", new { controller = "SystemSetting", action = "Index" });

        }
    }
}
