using Secure_Password_Repository.Models;
using Secure_Password_Repository.Repositories;
using Secure_Password_Repository.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Services
{
    public class PasswordPermissionService : IPasswordPermissionService
    {
        private IAccountService UserAccountService;
        private IUserPasswordRepository UserPasswordRepository;
        private IPasswordRepository PasswordRepository;
        private IApplicationSettingsService applicationSettings;

        public PasswordPermissionService(IAccountService useraccountservice, IUserPasswordRepository userpasswordrepository, IPasswordRepository passwordrepository, IApplicationSettingsService applicationsettings)
        {
            this.UserAccountService = useraccountservice;
            this.UserPasswordRepository = userpasswordrepository;
            this.PasswordRepository = passwordrepository;
            this.applicationSettings = applicationsettings;
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
            return applicationSettings.GetAdminsHaveAccessToAllPasswords() || PasswordRepository.GetPasswordCreatorId(passwordid) == UserAccountService.GetUserId() || UserPasswordRepository.UserHasViewAccessToPassword(passwordid);
        }


        public bool CanViewPassword(Password passworditem)
        {
            throw new NotImplementedException();
        }
    }
}