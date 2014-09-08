using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

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

            if(!filterContext.HttpContext.Request.IsAjaxRequest())
                if (user.Identity.IsAuthenticated)
                    if (HttpContext.Current.Cache[user.Identity.Name] == null)
                    {
                        //log out user
                        //return to index
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
    }
}