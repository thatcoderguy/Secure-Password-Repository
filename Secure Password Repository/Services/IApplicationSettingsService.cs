using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Services
{
    public interface IApplicationSettingsService
    {
        public string GetLogoImage();
        public string GetSMTPServerAddress();
        public string GetSMTPEmailAddress();
        public string GetSMTPServerUsername();
        public string GetSMTPServerPassword();
        public string GetSystemSalt();
        public string GetSystemInitilisationVector();
        public string GetSCryptHashCost();
        public string GetPBKDF2IterationCount();
        public string GetRoleAllowEditCategories();
        public string GetRoleAllowDeleteCategories();
        public string GetRoleAllowAddCategories();
        public string GetRoleAllowAddPasswords();
        public bool GetAdminsHaveAccessToAllPasswords();
        public bool GetBroadcastCategoryPositionChange();
        public bool GetBroadcastPasswordPositionChange();


    }
}
