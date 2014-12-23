using Microsoft.AspNet.Identity.Owin;
using Secure_Password_Repository.Exceptions;
using Secure_Password_Repository.Models;
using System;
using System.Security.Principal;
using System.Web;

namespace Secure_Password_Repository.Services
{
    public class AccountService: IAccountService
    {
        private ApplicationUserManager UserManager;
        private IPrincipal SecurityPrincipal;
        private IApplicationSettingsService ApplicationSettings;

        public AccountService(HttpContextBase httpcontext, IPrincipal securityprincipal, IApplicationSettingsService applicationsettings)
        {
            this.UserManager = httpcontext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            this.SecurityPrincipal = securityprincipal;
            this.ApplicationSettings = applicationsettings;
        }

        public AccountService(ApplicationUserManager usermanager, IPrincipal securityprincipal, IApplicationSettingsService applicationsettings)
        {
            this.UserManager = usermanager;
            this.SecurityPrincipal = securityprincipal;
            this.ApplicationSettings = applicationsettings;
        }

        public bool UserIsAnAdministrator()
        {
            return SecurityPrincipal.IsInRole("Administrator");
        }

        public ApplicationUser GetUserAccount()
        {
            var UserAccount = UserManager.FindByNameAsync(SecurityPrincipal.Identity.Name).Result;

            if (UserAccount == null)
                throw new UserAccountNotFoundException("An account with the username {0} could not be found.");

            return UserAccount;
        }

        public int GetUserId()
        {
            var UserAccount = UserManager.FindByNameAsync(SecurityPrincipal.Identity.Name).Result;

            if (UserAccount == null)
                throw new UserAccountNotFoundException("An account with the username {0} could not be found.");

            return UserAccount.Id;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}