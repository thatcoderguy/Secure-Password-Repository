using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace Secure_Password_Repository.Services
{
    public class PermissionService : IPermissionService
    {
        IAccountService AccountRespository;
        IUserPasswordRepository UserPasswordRepository;
        IApplicationSettingsService ApplicationSettingsService;

        public PermissionService(IAccountService accountrespository, IUserPasswordRepository userpasswordrepository, IApplicationSettingsService applicationsettingsservice)
        {
            this.UserPasswordRepository = userpasswordrepository;
            this.ApplicationSettingsService = applicationsettingsservice;
            this.AccountRespository = accountrespository;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passworditem"></param>
        /// <returns></returns>
        public bool CanViewPassword(Password passworditem)
        {
            int UserId = AccountRespository.GetUserId();

            if(ApplicationSettingsService.GetAdminsHaveAccessToAllPasswords() && AccountRespository.UserIsAnAdministrator())
                return true;

            if (passworditem.Creator_Id == UserId)
                return true;

            return UserPasswordRepository.GetUserPassword(passworditem.PasswordId, UserId) != null;
        }

        public bool CanEditPasswordPermissions()
        {
            throw new NotImplementedException();
        }
    }
}