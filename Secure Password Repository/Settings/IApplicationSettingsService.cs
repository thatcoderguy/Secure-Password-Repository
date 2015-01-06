using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Settings
{
    public interface IApplicationSettingsService
    {
        string GetLogoImage();
        string GetSMTPServerAddress();
        string GetSMTPEmailAddress();
        string GetSMTPServerUsername();
        string GetSMTPServerPassword();
        string GetSystemSalt();
        string GetSystemInitilisationVector();
        string GetSCryptHashCost();
        string GetPBKDF2IterationCount();
        string GetRoleAllowEditCategories();
        string GetRoleAllowDeleteCategories();
        string GetRoleAllowAddCategories();
        string GetRoleAllowAddPasswords();
        bool GetAdminsHaveAccessToAllPasswords();
        bool GetBroadcastCategoryPositionChange();
        bool GetBroadcastPasswordPositionChange();
    }
}
