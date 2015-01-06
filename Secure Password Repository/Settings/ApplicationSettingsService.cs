using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Settings
{
    public class ApplicationSettingsService: IApplicationSettingsService
    {
        public ApplicationSettings ApplicationSettingsInstance;

        public ApplicationSettingsService()
        {
            this.ApplicationSettingsInstance = ApplicationSettings.Default;
        }

        public ApplicationSettingsService(ApplicationSettings applicationsettingsinstance)
        {
            this.ApplicationSettingsInstance = applicationsettingsinstance;
        }

        public string GetLogoImage()
        {
            return ApplicationSettingsInstance.LogoImage;
        }

        public string GetSMTPServerAddress()
        {
            return ApplicationSettingsInstance.SMTPServerAddress;
        }

        public string GetSMTPEmailAddress()
        {
            return ApplicationSettingsInstance.SMTPEmailAddress;
        }

        public string GetSMTPServerUsername()
        {
            return ApplicationSettingsInstance.SMTPServerUsername;
        }

        public string GetSMTPServerPassword()
        {
            return ApplicationSettingsInstance.SMTPServerPassword;
        }

        public string GetSystemSalt()
        {
            return ApplicationSettingsInstance.SystemSalt;
        }

        public string GetSystemInitilisationVector()
        {
            return ApplicationSettingsInstance.SystemInitilisationVector;
        }

        public string GetSCryptHashCost()
        {
            return ApplicationSettingsInstance.SCryptHashCost;
        }

        public string GetPBKDF2IterationCount()
        {
            return ApplicationSettingsInstance.PBKDF2IterationCount;
        }

        public string GetRoleAllowEditCategories()
        {
            return ApplicationSettingsInstance.RoleAllowEditCategories;
        }

        public string GetRoleAllowDeleteCategories()
        {
            return ApplicationSettingsInstance.RoleAllowDeleteCategories;
        }

        public string GetRoleAllowAddCategories()
        {
            return ApplicationSettingsInstance.RoleAllowAddCategories;
        }

        public string GetRoleAllowAddPasswords()
        {
            return ApplicationSettingsInstance.RoleAllowAddPasswords;
        }

        public bool GetAdminsHaveAccessToAllPasswords()
        {
            return ApplicationSettingsInstance.AdminsHaveAccessToAllPasswords;
        }

        public bool GetBroadcastCategoryPositionChange()
        {
            return ApplicationSettingsInstance.BroadcastCategoryPositionChange;
        }

        public bool GetBroadcastPasswordPositionChange()
        {
            return ApplicationSettingsInstance.BroadcastPasswordPositionChange;
        }
    }
}