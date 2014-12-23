using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Services
{
    public class PermissionService : IPermissionService
    {

        private IAccountService UserAccountService;
        private IApplicationSettingsService ApplicationSettings;

        public PermissionService(IAccountService useraccountservice, IApplicationSettingsService applicationsettings)
        {
            this.UserAccountService = useraccountservice;
            this.ApplicationSettings = applicationsettings;
        }

        public bool CanAddCategory()
        {
            throw new NotImplementedException();
        }

        public bool CanEditCategory()
        {
            throw new NotImplementedException();
        }

        public bool CanDeleteCategory()
        {
            throw new NotImplementedException();
        }

        public bool CanAddPassword()
        {
            throw new NotImplementedException();
        }

        public bool CanDeletePassword()
        {
            throw new NotImplementedException();
        }

        public bool CanEditPassword()
        {
            throw new NotImplementedException();
        }

        public bool CanEditPasswordPermissions()
        {
            throw new NotImplementedException();
        }
    }
}