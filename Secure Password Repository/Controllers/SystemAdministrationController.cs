using Secure_Password_Repository.Settings;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Secure_Password_Repository.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SystemAdministrationController : Controller
    {
        // GET: SystemSetting
        public ActionResult Index()
        {
            return View();
        }

        // GET: SystemAdministration/SystemSettings
        public ActionResult SystemSettings()
        {
            SystemSettingViewModel viewModel = new SystemSettingViewModel()
            {
                AdminsHaveAccessToAllPasswords = ApplicationSettings.Default.AdminsHaveAccessToAllPasswords,
                LogoImage = ApplicationSettings.Default.LogoImage,
                PBKDF2IterationCount = ApplicationSettings.Default.PBKDF2IterationCount,
                RoleAllowAddCategories = ApplicationSettings.Default.RoleAllowAddCategories,
                RoleAllowAddPasswords = ApplicationSettings.Default.RoleAllowAddPasswords,
                RoleAllowDeleteCategories = ApplicationSettings.Default.RoleAllowDeleteCategories,
                RoleAllowEditCategories = ApplicationSettings.Default.RoleAllowEditCategories,
                SCryptHashCost = ApplicationSettings.Default.SCryptHashCost,
                SMTPEmailAddress = ApplicationSettings.Default.SMTPEmailAddress,
                SMTPServerAddress = ApplicationSettings.Default.SMTPServerAddress,
                SMTPServerPassword = ApplicationSettings.Default.SMTPServerPassword,
                SMTPServerUsername = ApplicationSettings.Default.SMTPServerUsername,
                BroadcastCategoryPositionChange = ApplicationSettings.Default.BroadastCategoryPositionChange,
                BroadcastPasswordPositionChange = ApplicationSettings.Default.BroadastPasswordPositionChange
            };

            return View(viewModel);
        }

        // POST: SystemSetting/Edit
        [HttpPost]
        public ActionResult EditSystemSettings(SystemSettingViewModel model)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
