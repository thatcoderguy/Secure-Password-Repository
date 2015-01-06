using Secure_Password_Repository.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Secure_Password_Repository.Identity
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }

        private IApplicationSettingsService applicationSettingService;

        public CustomPrincipal(string username, IApplicationSettingsService applicationsettingservice)
        {
            this.Identity = new GenericIdentity(username);
            this.applicationSettingService = applicationsettingservice;
        }

        public bool IsInRole(string role)
        {
            return Identity != null && Identity.IsAuthenticated &&
               !string.IsNullOrWhiteSpace(role) && Roles.IsUserInRole(Identity.Name, role);
        }

        public bool isAdministrator()
        {
            return IsInRole("Administrator");
        }

        public bool CanEditCategories()
        {
            return isAdministrator() || IsInRole(applicationSettingService.GetRoleAllowEditCategories());
        }

        public bool CanDeleteCategories()
        {
            return isAdministrator() || IsInRole(applicationSettingService.GetRoleAllowDeleteCategories());
        }

        public bool CanAddCategories()
        {
            return isAdministrator() || IsInRole(applicationSettingService.GetRoleAllowAddCategories());
        }

        public bool CanAddPasswords()
        {
            return isAdministrator() || IsInRole(applicationSettingService.GetRoleAllowAddPasswords());
        }
    }
}