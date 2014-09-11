using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Microsoft.Owin.Security;
using System.Web.Routing;

namespace Secure_Password_Repository.Filters
{
    /// <summary>
    /// Verify that the encryption key still exists in cache - otherwise return an error
    /// </summary>

    public class UserEncryptionKeyCacheVerification : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //get the current user
            IPrincipal user = filterContext.HttpContext.User;

            //make sure they are autenticated
            if (user.Identity.IsAuthenticated)

                //if the encryption key doesnt exist in cache
                if (HttpContext.Current.Cache[user.Identity.Name] == null)
                {

                    //log out the user
                    AuthenticationManager.SignOut();
                    filterContext.HttpContext.Response.Clear();
                    //return a 401 error
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

                    //if the request was AJAX based, then just return the error
                    if(filterContext.HttpContext.Request.IsAjaxRequest())
                        filterContext.HttpContext.Response.End();
                    //otherwise return back to the login screen
                    else
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Account" }, { "Action", "Login" }, { "ReturnUrl", "Password" } });

                }

        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

    }

}