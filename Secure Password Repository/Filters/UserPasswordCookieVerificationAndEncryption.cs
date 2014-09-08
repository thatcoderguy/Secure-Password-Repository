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

namespace Secure_Password_Repository.Filters
{
    public class UserPasswordCookieVerificationAndEncryption : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            IPrincipal user = filterContext.HttpContext.User;

            if (user.Identity.IsAuthenticated)
                if (HttpContext.Current.Cache[user.Identity.Name] == null)
                {

                    AuthenticationManager.SignOut();
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    //decrypt the OTP
                    //decrypt password
                    //generate new OTP
                    //reencrypt the password
                    //encrypt OTP
                    //store in cache
                    //store in cookie
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