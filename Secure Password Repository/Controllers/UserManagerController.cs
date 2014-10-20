using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using Secure_Password_Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Caching;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Secure_Password_Repository.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserManagerController : Controller
    {

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public UserManagerController()
        {
        }

        public UserManagerController(ApplicationUserManager userManager)
        {
            UserMgr = userManager;
        }

        public UserManagerController(ApplicationRoleManager roleManager)
        {
            RoleMgr = roleManager;
        }

        public ApplicationUserManager UserMgr
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
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

        // GET: UserManager
        public async Task<ActionResult> Index()
        {
            //success messages upon return to index page
            if (Request.QueryString.Get("successaction") == "Account Updated")
            {
                ViewBag.Message = "Account updated successfully.";
            }
            else if (Request.QueryString.Get("successaction") == "Account Deleted")
            {
                ViewBag.Message = "Account delete successfully.";
            }

            return View(new UserList() { 
                Users = await UserMgr.Users.Include("Roles").ToListAsync()
            });
        }

        // GET: UserManager/Edit/5
        public async Task<ActionResult> Edit(int UserId)
        {
            UpdateAccountViewModel model = new UpdateAccountViewModel();

            //grab the selected account
            var selectedAccount = await UserMgr.FindByIdAsync(UserId);
            if(selectedAccount!=null)
            {
  
                IEnumerable<ApplicationRole> availableRoles = await RoleMgr.Roles.ToListAsync();
                //put the attributes from this account into the model, so the existing values are displayed
                model.Email = selectedAccount.Email;
                model.Username = selectedAccount.UserName;
                model.FullName = selectedAccount.userFullName;
                model.Role = selectedAccount.GetRole();
                model.RolesList = new SelectList(availableRoles, "Id", "Name", model.Role.Id);
            }
            else
                ModelState.AddModelError("", "User account does not exsit");

            return View(model);
        }

        // POST: UserManager/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int UserId, UpdateAccountViewModel model)
        {
            try
            {

                //grab the account selected
                var selectedAccount = await UserMgr.FindByIdAsync(UserId);
                if (selectedAccount != null)
                {

                    //update the account's attributes
                    selectedAccount.userFullName = model.FullName;
                    selectedAccount.UserName = model.Username;
                    selectedAccount.Email = model.Email;
                    selectedAccount.Roles.Clear();
                    selectedAccount.Roles.Add(new CustomUserRole() { RoleId = model.Role.Id, UserId = UserId });

                    //attempt to update the account
                    var result = await UserMgr.UpdateAsync(selectedAccount);
                    if (result.Succeeded)
                    {
                        //redirect back to the account list, with a success message 
                        return RedirectToAction("Index", "UserManager", new { successaction = "Account Updated" });
                    }
                    else
                    {
                        //list any errors (e.g. email/username already exists)
                        string errorlist = string.Empty;
                        foreach (string error in result.Errors.ToList())
                            errorlist += error + " and ";

                        if (errorlist != string.Empty)
                            errorlist = errorlist.Substring(0, errorlist.Length - 5);

                        ModelState.AddModelError("", errorlist);
                    }

                }
                else
                {

                    ModelState.AddModelError("", "User account does not exist");

                }

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }


            //if we got this far, something baaaaad happened
            return View(model);
        }

        //
        // POST: /Account/AuthoriseAccount/4
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthoriseAccount(int UserId)
        {
            int intLoggedInUserId = User.Identity.GetUserId().ToInt();

            var loggedInUser = await UserMgr.FindByIdAsync(intLoggedInUserId);
            var user = await UserMgr.FindByIdAsync(UserId);

            if (user != null)
            {

                #region Decrypt_Administrators_Copy_of_Encryption_Key

                    //grab the 3 encryption keys that are required to do encryption
                    byte[] bytePrivateKey = loggedInUser.userPrivateKey.FromBase64().ToBytes();
                    byte[] byteEncryptionKey = loggedInUser.userEncryptionKey.FromBase64().ToBytes();
                    byte[] bytePasswordBasedKey = MemoryCache.Default.Get(loggedInUser.UserName).ToString().ToBytes();

                    //decrypt the user's private
                    EncryptionAndHashing.DecryptPrivateKey(ref bytePrivateKey, bytePasswordBasedKey);

                    //decrypt the database encryption key
                    EncryptionAndHashing.DecryptDatabaseKey(ref byteEncryptionKey, bytePrivateKey);

                #endregion

                #region Encrypt_And_Store_Authorised_Users_Copy_Of_Encryption_Key

                    //encrypt the database key
                    EncryptionAndHashing.EncryptDatabaseKey(ref byteEncryptionKey, user.userPublicKey);

                    //convert key to string and store
                    user.userEncryptionKey = byteEncryptionKey.ToBase64String();

                #endregion

                user.isAuthorised = true;

                //attempt to update the account
                var result = await UserMgr.UpdateAsync(user);
                if (result.Succeeded)
                {

                    var callbackUrl = Url.Action("Login", "Account", new {}, protocol: Request.Url.Scheme);
                    await UserMgr.SendEmailAsync(user.Id, "Account has been authorised", RenderViewContent.RenderViewToString("UserManager", "AccountAuthorisedEmail", new AccountAuthorisedConfirmation { CallBackURL = callbackUrl, UserName = user.UserName }));

                    return Json(new
                    {
                        AccountId = UserId
                    });
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

        }

        public ActionResult ResetPassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("ResetPassword", "UserManager");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetMyPassword(int UserId, ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("ResetPassword", "UserManager");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserMgr.ChangePasswordAsync(UserId, model.OldPassword, model.NewPassword);

                    //decrypt admin copy of key
                    //encrypt users copy of key

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPassword", "UserManager", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserMgr.AddPasswordAsync(int.Parse(User.Identity.GetUserId()), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPassword", "UserManager", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private bool HasPassword()
        {
            var user = UserMgr.FindById(int.Parse(User.Identity.GetUserId()));
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}
