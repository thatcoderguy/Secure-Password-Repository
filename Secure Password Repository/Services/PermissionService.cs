using Secure_Password_Repository.Models;
using Secure_Password_Repository.Repositories;
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
        private IUserPasswordRepository UserPasswordRepository;
        private IPasswordRepository PasswordRepository;

        public PermissionService(IAccountService useraccountservice, IApplicationSettingsService applicationsettings, IUserPasswordRepository userpasswordrepository, IPasswordRepository passwordrepository)
        {
            this.UserAccountService = useraccountservice;
            this.ApplicationSettings = applicationsettings;
            this.UserPasswordRepository = userpasswordrepository;
            this.PasswordRepository = passwordrepository;
        }

        public bool CanAddCategories()
        {
            return UserAccountService.UserIsAnAdministrator() || UserAccountService.UserInRole(ApplicationSettings.GetRoleAllowAddCategories());
        }

        public bool CanEditCategories()
        {
            throw new NotImplementedException();
        }

        public bool CanDeleteCategories()
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


        public bool CanViewPassword(int passwordid)
        {
            return ApplicationSettings.GetAdminsHaveAccessToAllPasswords() || PasswordRepository.GetPasswordCreatorId(passwordid) == UserAccountService.GetUserId() || UserPasswordRepository.UserHasViewAccessToPassword(passwordid);
        }


        public bool CanViewPassword(Password passworditem)
        {
            throw new NotImplementedException();
        }
    }
}