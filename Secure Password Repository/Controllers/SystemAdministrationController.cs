using Secure_Password_Repository.Models;
using Secure_Password_Repository.Settings;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace Secure_Password_Repository.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SystemAdministrationController : Controller
    {
        private ApplicationRoleManager _roleManager;

        public SystemAdministrationController()
        {
        }

        public SystemAdministrationController(ApplicationRoleManager roleManager)
        {
            RoleMgr = roleManager;
        }

        public ApplicationRoleManager RoleMgr
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        
        // GET: SystemSetting
        public ActionResult Index()
        {
            return View();
        }

        // GET: SystemAdministration/SystemSettings
        public async Task<ActionResult> SystemSettings()
        {

            SystemSettingViewModel viewModel = new SystemSettingViewModel()
            {
                AdminsHaveAccessToAllPasswords = ApplicationSettings.Default.AdminsHaveAccessToAllPasswords,
                LogoImage = ApplicationSettings.Default.LogoImage,
                PBKDF2IterationCount = ApplicationSettings.Default.PBKDF2IterationCount,
                RoleAllowAddCategories = GetRoleFromName(ApplicationSettings.Default.RoleAllowAddCategories),
                RoleAllowAddPasswords = GetRoleFromName(ApplicationSettings.Default.RoleAllowAddPasswords),
                RoleAllowDeleteCategories = GetRoleFromName(ApplicationSettings.Default.RoleAllowDeleteCategories),
                RoleAllowEditCategories = GetRoleFromName(ApplicationSettings.Default.RoleAllowEditCategories),
                SCryptHashCost = ApplicationSettings.Default.SCryptHashCost,
                SMTPEmailAddress = ApplicationSettings.Default.SMTPEmailAddress,
                SMTPServerAddress = ApplicationSettings.Default.SMTPServerAddress,
                SMTPServerPassword = ApplicationSettings.Default.SMTPServerPassword,
                SMTPServerUsername = ApplicationSettings.Default.SMTPServerUsername,
                BroadcastCategoryPositionChange = ApplicationSettings.Default.BroadcastCategoryPositionChange,
                BroadcastPasswordPositionChange = ApplicationSettings.Default.BroadcastPasswordPositionChange,
                AvailableRoles = await RoleMgr.Roles.ToListAsync()
            };

            return View(viewModel);
        }

        // POST: SystemAdministration/SystemSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SystemSettings(SystemSettingViewModel model)
        {

            model.AvailableRoles = await RoleMgr.Roles.ToListAsync();

            //Store the full role object - required for updating the settings object
            model.RoleAllowAddCategories = model.AvailableRoles.Single(r => r.Id == model.RoleAllowAddCategories.Id);
            model.RoleAllowAddPasswords = model.AvailableRoles.Single(r => r.Id == model.RoleAllowAddPasswords.Id);
            model.RoleAllowEditCategories = model.AvailableRoles.Single(r => r.Id == model.RoleAllowEditCategories.Id);
            model.RoleAllowDeleteCategories = model.AvailableRoles.Single(r => r.Id == model.RoleAllowDeleteCategories.Id);

            model.SMTPServerPassword = model.SMTPServerPassword ?? string.Empty;
            model.SMTPServerUsername = model.SMTPServerUsername ?? string.Empty;

            if(ModelState.IsValid)
            {
                ApplicationSettings.Default.UpdateSettings(model);
            }
            
            return View(model);
        }

        #region helpers

            /// <summary>
            ///get a role object from it's name
            /// </summary>
            private ApplicationRole GetRoleFromName(string RoleName)
            {
                var role = RoleMgr.Roles.Where(r => r.Name == RoleName).First();
                return role;
            }

        #endregion

    }
}
