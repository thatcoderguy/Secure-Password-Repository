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

            //front page content
            routes.MapRoute("Home", "", new { controller = "Home", action = "Index" });
            routes.MapRoute("HomeIndex", "Home/Index", new { controller = "Home", action = "Index" });

            //passwords & categories
            routes.MapRoute("Password", "Password", new { controller = "Password", action = "Index" });
            routes.MapRoute("PasswordIndex", "Password/Index", new { controller = "Password", action = "Index" });

            //categories
            routes.MapRoute("NewCategory", "Password/AddCategory", new { controller = "Password", action = "AddCategory" });
            routes.MapRoute("EditCategory", "Password/EditCategory", new { controller = "Password", action = "EditCategory" });
            routes.MapRoute("DeleteCategory", "Password/DeleteCategory/{categoryid}", new { controller = "Password", action = "DeleteCategory", CategoryId = UrlParameter.Optional });
            routes.MapRoute("GetCategoryChildren", "Password/GetCategoryChildren/{parentcategoryid}", new { controller = "Password", action = "GetCategoryChildren", ParentCategoryId = UrlParameter.Optional });
            routes.MapRoute("UpdatePosition", "Password/UpdatePosition/{itemid}", new { controller = "Password", action = "UpdatePosition", ItemId = UrlParameter.Optional, NewPosition = UrlParameter.Optional, isCategoryItem = UrlParameter.Optional });

            //passwords
            routes.MapRoute("AddPassword", "Password/AddPassword/{parentcategoryid}", new { controller = "Password", action = "AddPassword", ParentCategoryId = UrlParameter.Optional });
            routes.MapRoute("DeletePassword", "Password/DeletePassword/{passwordid}", new { controller = "Password", action = "DeletePassword", PasswordId = UrlParameter.Optional });
            routes.MapRoute("EditPassword", "Password/EditPassword/{passwordid}", new { controller = "Password", action = "EditPassword", PasswordId = UrlParameter.Optional });
            routes.MapRoute("ViewPassword", "Password/ViewPassword/{passwordid}", new { controller = "Password", action = "ViewPassword", PasswordId = UrlParameter.Optional });
            routes.MapRoute("GetEncryptedPassword", "Password/GetEncryptedPassword/{passwordid}", new { controller = "Password", action = "GetEncryptedPassword", PasswordId = UrlParameter.Optional });
            routes.MapRoute("EditUserPermissions", "Password/EditUserPermissions/{passwordid}", new { controller = "Password", action = "EditUserPermissions", PasswordId = UrlParameter.Optional });
            
            //account handling
            routes.MapRoute("Login", "Login", new { controller = "Account", action = "Login", returnUrl = UrlParameter.Optional });
            routes.MapRoute("AccountLogin", "Account/Login", new { controller = "Account", action = "Login", returnUrl = UrlParameter.Optional });
            routes.MapRoute("Register", "Register", new { controller = "Account", action = "Register" });
            routes.MapRoute("LogOff", "LogOff", new { controller = "Account", action = "LogOff" });
            routes.MapRoute("RegistrationConfirmation", "RegistrationConfirmation", new { controller = "Account", action = "RegistrationConfirmation" });
            routes.MapRoute("ForgotPassword", "ForgotPassword", new { controller = "Account", action = "ForgotPassword" });
            routes.MapRoute("ForgotPasswordConfirmation", "ForgotPasswordConfirmation", new { controller = "Account", action = "ForgotPasswordConfirmation" });
            routes.MapRoute("ResetPassword", "ResetPassword/{userid}", new { controller = "Account", action = "ResetPassword", UserId = UrlParameter.Optional });
            routes.MapRoute("ResetPasswordConfirmation", "ResetPasswordConfirmation", new { controller = "Account", action = "ResetPasswordConfirmation" });
            routes.MapRoute("AccountResetPasswordConfirmation", "Account/ResetPasswordConfirmation", new { controller = "Account", action = "ResetPasswordConfirmation" });

            //account managment
            routes.MapRoute("Manage", "Manage", new { controller = "Account", action = "Manage" });
            routes.MapRoute("UserManagerIndex", "UserManager/Index", new { controller = "UserManager", action = "Index" });
            routes.MapRoute("UserManager", "UserManager", new { controller = "UserManager", action = "Index" });
            routes.MapRoute("AuthoriseAccount", "UserManager/AuthoriseAccount/{userid}", new { controller = "UserManager", action = "AuthoriseAccount", UserId = UrlParameter.Optional });
            routes.MapRoute("UserManagerEdit", "UserManager/Edit/{userid}", new { controller = "UserManager", action = "Edit", UserId = UrlParameter.Optional });
            routes.MapRoute("UserManagerDelete", "UserManager/Delete/{userid}", new { controller = "UserManager", action = "Delete", UserId = UrlParameter.Optional });
            
            //system setting
            routes.MapRoute("SystemAdministration", "SystemAdministration/Index", new { controller = "SystemAdministration", action = "Index" });
            routes.MapRoute("SystemSettings", "SystemAdministration/SystemSettings", new { controller = "SystemAdministration", action = "SystemSettings" });
        }
    }
}
