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
            IEnumerable<ApplicationRole> availableRoles = await RoleMgr.Roles.ToListAsync();

            SystemSettingViewModel viewModel = new SystemSettingViewModel()
            {
                AdminsHaveAccessToAllPasswords = ApplicationSettings.Default.AdminsHaveAccessToAllPasswords,
                LogoImage = ApplicationSettings.Default.LogoImage,
                PBKDF2IterationCount = ApplicationSettings.Default.PBKDF2IterationCount,
                RoleAllowAddCategories = new SelectList(availableRoles, "Name", "Name", new SelectListItem() { Text = ApplicationSettings.Default.RoleAllowAddCategories, Value = ApplicationSettings.Default.RoleAllowAddCategories, Selected = true }),
                RoleAllowAddPasswords = new SelectList(availableRoles, "Name", "Name", new SelectListItem() { Text = ApplicationSettings.Default.RoleAllowAddPasswords, Value = ApplicationSettings.Default.RoleAllowAddPasswords, Selected = true }),
                RoleAllowDeleteCategories = new SelectList(availableRoles, "Name", "Name", new SelectListItem() { Text = ApplicationSettings.Default.RoleAllowDeleteCategories, Value = ApplicationSettings.Default.RoleAllowDeleteCategories, Selected = true }),
                RoleAllowEditCategories = new SelectList(availableRoles, "Name", "Name", new SelectListItem() { Text = ApplicationSettings.Default.RoleAllowEditCategories, Value = ApplicationSettings.Default.RoleAllowEditCategories, Selected = true }),
                SCryptHashCost = ApplicationSettings.Default.SCryptHashCost,
                SMTPEmailAddress = ApplicationSettings.Default.SMTPEmailAddress,
                SMTPServerAddress = ApplicationSettings.Default.SMTPServerAddress,
                SMTPServerPassword = ApplicationSettings.Default.SMTPServerPassword,
                SMTPServerUsername = ApplicationSettings.Default.SMTPServerUsername,
                BroadcastCategoryPositionChange = ApplicationSettings.Default.BroadcastCategoryPositionChange,
                BroadcastPasswordPositionChange = ApplicationSettings.Default.BroadcastPasswordPositionChange,

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
