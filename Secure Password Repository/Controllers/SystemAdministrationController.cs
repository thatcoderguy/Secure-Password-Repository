using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Secure_Password_Repository.Controllers;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Utilities;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Settings;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Secure_Password_Repository.Controllers
{
    [Authorize(Roles = "Administrator")]
#if !DEBUG
    [RequireHttps] //apply to all actions in controller
#endif

    public class SystemAdministrationController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        private ApplicationDbContext DatabaseContext = new ApplicationDbContext();

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

        public ApplicationUserManager UserMgr
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
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

            if (ModelState.IsValid)
            {
                ApplicationSettings.Default.UpdateSettings(model);
            }

            return View(model);
        }

        public ActionResult ExportPasswords()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportPasswords(ExportPasswordsViewModel model)
        {
            if (!model.ConfirmPasswordExport)
                return View();

            var fileName = "Exported Passwords.txt";
            var contentType = "text/plain";

            int UserId = User.Identity.GetUserId().ToInt();
            bool userIsAdmin = User.IsInRole("Administrator");
            var user = await UserMgr.FindByIdAsync(UserId);

            string FileContent = "Description\tUsername\tSecond Credential\tPassword\tLocation\tNotes\tCreator\tCategory\r\n";

            //Retrive all of the unpasswords
            var AllPasswords = DatabaseContext.Passwords.Include(p => p.Creator).Include(p => p.Parent_Category).Where(p => !p.Deleted).ToList();

            //descrypt all and build up file contents
            foreach (Password passworditem in AllPasswords)
            {
                string Password = passworditem.EncryptedPassword;
                string Username = passworditem.EncryptedUserName;
                string SecondCredential = passworditem.EncryptedSecondCredential;

                EncryptionAndHashing.DecryptUsernameFields(user, ref Username, ref SecondCredential);
                EncryptionAndHashing.DecryptPasswordField(user, ref Password);

                FileContent += passworditem.Description + "\t" + Username + "\t" + SecondCredential + "\t" + Password + "\t" + passworditem.Location + "\t" + passworditem.Notes + "\t" + passworditem.Creator.UserName + "\t" + passworditem.Parent_Category.CategoryName + "\r\n";

            }

            //send email
            #region send_email_to_all_admins

            //send an email to all administrators letting them know a new account needs authorising
            var roleId = RoleMgr.FindByName("Administrator").Id;

            //generate list of userIDs of the admins
            List<int> adminUserIdList = UserMgr.Users.Include("Roles").Where(u => u.Roles.Any(r => r.RoleId == roleId && r.UserId == u.Id)).Select(u => u.Id).ToList();

            foreach (int adminUserId in adminUserIdList)
            {

                //build the email body from a view
                string bodyText = RenderViewContent.RenderViewToString("SystemAdministration", "ExportPasswordsEmail",
                                                                                                                new ExportPasswordsEmail()
                                                                                                                {
                                                                                                                    FullUserName = user.userFullName,
                                                                                                                    UserName = user.UserName
                                                                                                                });

                await UserMgr.SendEmailAsync(adminUserId, "Passwords have been exported from the system", bodyText);
            }

            #endregion

            return File(Encoding.ASCII.GetBytes(FileContent), contentType, fileName);
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
