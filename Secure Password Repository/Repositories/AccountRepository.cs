using Microsoft.AspNet.Identity.Owin;
using Secure_Password_Repository.Models;
using System;
using System.Web;

namespace Secure_Password_Repository.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private ApplicationUserManager UserManager;
        private HttpContextBase HttpContext;

        public AccountRepository(HttpContextBase httpcontext)
        {
            this.HttpContext = httpcontext;
            this.UserManager = httpcontext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public AccountRepository(HttpContextBase httpcontext, ApplicationUserManager usermanager)
        {
            this.HttpContext = httpcontext;
            this.UserManager = usermanager;
        }

        public ApplicationUser GetUserAccount()
        {
            return null;
            //return UserManager.FindById(User.Identity.GetUserId().ToInt());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}