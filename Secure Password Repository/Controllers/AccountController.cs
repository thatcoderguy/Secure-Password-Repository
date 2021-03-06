﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Settings;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Secure_Password_Repository.Controllers
{
    [Authorize]
    #if !DEBUG
    [RequireHttps] //apply to all actions in controller
    #endif
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private ApplicationDbContext DatabaseContext = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserMgr = userManager;
        }

        public AccountController(ApplicationRoleManager roleManager)
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //model state invalid
            if (!ModelState.IsValid)
                return View(model);

            //find user account so that it's status can
            var user = await UserMgr.FindAsync(model.Username, model.Password);

            //could not find user
            if (user == null)
            {
                ModelState.AddModelError("", "Username or Password is invalid.");
                return View(model);
            }

            //user account has been locked out due to too many invalid attempts
            if (await UserMgr.GetLockoutEndDateAsync(user.Id) > DateTimeOffset.Now.UtcDateTime)
            {
                ModelState.AddModelError("", "Your account has been locked out.");
                return View(model);
            }

            //only allow the user to sign in if their account is authorised
            //this is because an admin needs to authorise new accounts, so that the encryption key can be 
            //encrypted with the user's public key
            if (!user.isAuthorised)
            {
                ModelState.AddModelError("", "Your account needs to be authorised by an Administrator.");
                return View(model);
            }
            
            //user account has been deactived
            if (!user.isActive)
            {
                ModelState.AddModelError("", "Your account has been disabled.");
                return View(model);
            }

            //user account is ok, so sign the user in
            await SignInAsync(user, false);

            //reset invalid access count
            await UserMgr.ResetAccessFailedCountAsync(user.Id);

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "Password";

            #region store_password_hash_into_cache

                CacheEntryRemovedCallback onRemove = new CacheEntryRemovedCallback(this.RemovedCallback);

                //store the session ID in cache - this is to stop the same user logging in twice
                MemoryCache.Default.Set(model.Username + "SessionID",
                                        Session.SessionID,
                                        new CacheItemPolicy()
                                        {
                                            AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration,
                                            SlidingExpiration = TimeSpan.FromHours(1),    //1 hour - incase user logs out
                                            Priority = CacheItemPriority.Default,
                                            RemovedCallback = onRemove
                                        });              //add item back into cache, if user logged in

                //hash and encrypt the user's password - so this can be used to decrypt the user's private key
                byte[] hashedPassword = EncryptionAndHashing.Hash_SHA1_ToBytes(model.Password);
                hashedPassword = EncryptionAndHashing.Hash_PBKDF2_ToBytes(hashedPassword, ApplicationSettings.Default.SystemSalt).ToBase64();

                //in-memory encryption of the hash
                EncryptionAndHashing.Encrypt_Memory_DPAPI(ref hashedPassword);

                //store the encrypted password hash in cache
                MemoryCache.Default.Set(model.Username,
                                        hashedPassword.ConvertToString(),
                                        new CacheItemPolicy()
                                        {
                                            AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration,
                                            SlidingExpiration = TimeSpan.FromHours(1),    //1 hour - incase user logs out
                                            Priority = CacheItemPriority.Default,
                                            RemovedCallback = onRemove
                                        });              //add item back into cache, if user logged in

            #endregion

            //redirect to the return URL (usually /passwords) now that the account has been logged in
            return RedirectToLocal(returnUrl);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            //we need to know if there are any existing accounts
            //because if not, the first account needs to be an admin account, and contain a new encryption key
            if (UserMgr.Users.ToList().Count == 0)
                return View("RegisterFirstAccount");
            else
                return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //model state is invalid
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser() { UserName = model.Username, Email = model.Email, userFullName = model.FullName };

            //store whether this was the first account created in the system (gets returned in the querystring)
            string FirstUserAccount = string.Empty;
            string UserDefaultRole = string.Empty;

            //generate a set of RSA keys - this set of keys are persistant until Destroy_RSAKeys() is called
            //so we'll want to call Destroy_RSAKeys ASAP, for security purposes!
            EncryptionAndHashing.Generate_NewRSAKeys();

            //retrieve the generated RSA public key used for new user
            //this can be stored as plaintext - we want people to use this key!
            user.userPublicKey = await EncryptionAndHashing.Retrieve_PublicKey();

            #region retrieve_and_encrypt_private_key

                //convert the private key to bytes, then clear the original string
                byte[] userPrivateKeyBytes = (await EncryptionAndHashing.Retrieve_PrivateKey()).ToBytes();

                //encrypt the user's private key
                EncryptionAndHashing.EncryptPrivateKey(ref userPrivateKeyBytes, model.Password);

                //convert to string and store
                user.userPrivateKey = userPrivateKeyBytes.ToBase64String();

                //clear the raw privatekey out of memory
                Array.Clear(userPrivateKeyBytes, 0, userPrivateKeyBytes.Length);

                //Keys has been encrypted, the plaintext ones can now be destroyed
                EncryptionAndHashing.Destroy_RSAKeys();

            #endregion

            //if this is the first account being registered, then make it the admin account and create & store an encryption key
            if (UserMgr.Users.ToList().Count == 0)
            {
                #region generate_new_database_symetric_encryption_key_and_encrypt

                    //generate 32 random bytes - this will be the encryption key
                    byte[] DatabaseEncryptionKeyBytes = EncryptionAndHashing.Generate_Random_ReadableBytes(32);

                    //encrypt the database key
                    EncryptionAndHashing.EncryptDatabaseKey(ref DatabaseEncryptionKeyBytes, user.userPublicKey);

                    //convert key to string and store
                    user.userEncryptionKey = DatabaseEncryptionKeyBytes.ToBase64String();

                    //clear the original data
                    Array.Clear(DatabaseEncryptionKeyBytes, 0, DatabaseEncryptionKeyBytes.Length);

                #endregion

                user.isAuthorised = true;
                user.isActive = true;
                FirstUserAccount = "Yes";
                UserDefaultRole = "Administrator";
            }
            //user needs to be authorised by an admin, so the encryption key can be copied to the user's account
            else
            {
                user.isAuthorised = false;
                user.isActive = true;
                FirstUserAccount = "No";
                UserDefaultRole = "User";
            }

            //check that the roles exists
            if (!RoleMgr.RoleExists(UserDefaultRole))
            {
                AddErrors(IdentityResult.Failed(new string[] { "The role: " + UserDefaultRole + " does not exist" }));
                return View(model);
            }

            //create user account
            IdentityResult result = await UserMgr.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }

            //add default role to user
            result = await UserMgr.AddToRoleAsync(user.Id, UserDefaultRole);
            if(!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }

            //if this is not the first account being registred, then send an email to all admins
            //letting them know an account needs to be authorisd
            if (FirstUserAccount == "No")
            {
                #region send_email_to_all_admins

                    //send an email to all administrators letting them know a new account needs authorising
                    var roleId = RoleMgr.FindByName("Administrator").Id;
                    
                    //generate list of userIDs of the admins
                    List<int> adminUserIdList = UserMgr.Users.Include("Roles").Where(u => u.Roles.Any(r => r.RoleId == roleId && r.UserId == u.Id)).Select(u => u.Id).ToList();
                    
                    //generate the url to the usermanager index action
                    string callbackurl = Url.RouteUrl("UserManager", new { }, protocol: Request.Url.Scheme);

                    foreach (int adminUserId in adminUserIdList)
                    {

                        //build the email body from a view
                        string bodyText = RenderViewContent.RenderViewToString("Account", "AuthorisationRequiredEmail",
                                                                                                                        new AccountAuthorisationRequest()
                                                                                                                        {
                                                                                                                            callbackurl = callbackurl,
                                                                                                                            userEmail = user.Email,
                                                                                                                            userFullName = user.userFullName,
                                                                                                                            userName = user.UserName
                                                                                                                        });

                        await UserMgr.SendEmailAsync(adminUserId, "New account needs authorisation", bodyText);
                    }

                #endregion
            }

            return RedirectToAction("RegistrationConfirmation", new { ThisIsTheFirstAccount = FirstUserAccount });
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserMgr.FindByEmailAsync(model.Email);
            if (user == null)  // || !(await UserMgr.IsEmailConfirmedAsync(user.Id))   -- maybe implement this later
            {
                ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                return View();
            }

            //setup a token provider to generate the reset code
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("Secure Password Repository");
            UserMgr.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("EmailConfirmation"));

            // Send an email with this link
            string code = await UserMgr.GeneratePasswordResetTokenAsync(user.Id);

            //generate the url for the reset password action
            var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = user.Id, Code = code, Email = user.Email }, protocol: Request.Url.Scheme);
                
            //build the email body from a view
            string bodyText = RenderViewContent.RenderViewToString("Account", "ResetPasswordEmail", 
                                                                                                    new PasswordForgetConfirmation { 
                                                                                                        CallBackURL = callbackUrl 
                                                                                                    });

            await UserMgr.SendEmailAsync(user.Id, "Password Reset Request", bodyText);
            return RedirectToAction("ForgotPasswordConfirmation", "Account");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(int UserId, string Code, string email)
        {
            if (Code == null)
            {
                return View("Error");
            }
            return View(new ResetPasswordViewModel {  Email = email });
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(int UserId, ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ResetPassword", model);

            var user = await UserMgr.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found.");
                return View();
            }

            //setup a token provider to verify the reset code
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("Secure Password Repository");
            UserMgr.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, int>(provider.Create("EmailConfirmation"));

            IdentityResult result = await UserMgr.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View("ResetPassword", model);
            }

            //generate a set of RSA keys - this set of keys are persistant until Destroy_RSAKeys() is called
            //so we'll want to call Destroy_RSAKeys ASAP, for security purposes!
            EncryptionAndHashing.Generate_NewRSAKeys();

            //retrieve the generated RSA public key used for new user
            //this can be stored as plaintext - we want people to use this key!
            user.userPublicKey = await EncryptionAndHashing.Retrieve_PublicKey();

            #region retrieve_and_encrypt_private_key

                //convert the private key to bytes, then clear the original string
                byte[] userPrivateKeyBytes = (await EncryptionAndHashing.Retrieve_PrivateKey()).ToBytes();

                //encrypt the user's private key
                EncryptionAndHashing.EncryptPrivateKey(ref userPrivateKeyBytes, model.Password);

                //convert to string and store
                user.userPrivateKey = userPrivateKeyBytes.ToBase64String();

                //clear the raw privatekey out of memory
                Array.Clear(userPrivateKeyBytes, 0, userPrivateKeyBytes.Length);

                //Keys has been encrypted, the plaintext ones can now be destroyed
                EncryptionAndHashing.Destroy_RSAKeys();

            #endregion

            user.isAuthorised = false;
            user.userEncryptionKey = null;

            //attempt to update the account
            result = await UserMgr.UpdateAsync(user);
            if (!result.Succeeded)
            {
                //list any errors (e.g. email/username already exists)
                string errorlist = string.Empty;

                errorlist = string.Join(" and ", result.Errors);

                ModelState.AddModelError("", errorlist);
                return View("ResetPassword", model);
            }

            //send an email to all administrators letting them know a new account needs authorising
            var roleId = RoleMgr.FindByName("Administrator").Id;

            //generate list of userIDs of the admins
            List<int> adminUserIdList = UserMgr.Users.Include("Roles").Where(u => u.Roles.Any(r => r.RoleId == roleId && r.UserId == u.Id)).Select(u => u.Id).ToList();

            //generate the url to the usermanager index action
            string callbackurl = Url.RouteUrl("UserManager", new { }, protocol: Request.Url.Scheme);

            foreach (int adminUserId in adminUserIdList)
            {
                //generate email body from a view
                string bodyText =  RenderViewContent.RenderViewToString("Account", "AuthorisationRequiredEmail", 
                                                                                                                new AccountAuthorisationRequest()
                                                                                                                {
                                                                                                                    callbackurl = callbackurl,
                                                                                                                    userEmail = user.Email,
                                                                                                                    userFullName = user.userFullName,
                                                                                                                    userName = user.UserName
                                                                                                                });

                await UserMgr.SendEmailAsync(adminUserId, "New account needs authorisation", bodyText);
            }

            //redirect back to the account list, with a success message 
            return RedirectToAction("ResetPasswordConfirmation", "Account");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserMgr.RemoveLoginAsync(int.Parse(User.Identity.GetUserId()), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserMgr.FindByIdAsync(int.Parse(User.Identity.GetUserId()));
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            //attempt to change password
            IdentityResult result = await UserMgr.ChangePasswordAsync(User.Identity.GetUserId().ToInt(), model.OldPassword, model.NewPassword);
            
            //could not change password
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }
            
            var user = await UserMgr.FindByIdAsync(User.Identity.GetUserId().ToInt());

            #region re-encrypt_private_key_and_store_hash_in_cache

                byte[] bytePrivateKey = user.userPrivateKey.FromBase64().ToBytes();
                byte[] bytePasswordBasedKey = MemoryCache.Default.Get(user.UserName).ToString().ToBytes();

                //decrypt the users copy of the private key
                EncryptionAndHashing.DecryptPrivateKey(ref bytePrivateKey, bytePasswordBasedKey);

                //clear the old password key
                Array.Clear(bytePasswordBasedKey, 0, bytePasswordBasedKey.Length);

                CacheEntryRemovedCallback onRemove = new CacheEntryRemovedCallback(this.RemovedCallback);

                //hash and encrypt the user's password - so this can be used to decrypt the user's private key
                byte[] hashedPassword = EncryptionAndHashing.Hash_SHA1_ToBytes(model.NewPassword);
                hashedPassword = EncryptionAndHashing.Hash_PBKDF2_ToBytes(hashedPassword, ApplicationSettings.Default.SystemSalt).ToBase64();

                //encrypt the user's private key
                EncryptionAndHashing.EncryptPrivateKey(ref bytePrivateKey, hashedPassword.ConvertToString());

                //convert to string and store
                user.userPrivateKey = bytePrivateKey.ToBase64String();

                //clear the raw privatekey out of memory
                Array.Clear(bytePrivateKey, 0, bytePrivateKey.Length);

                //in-memory encryption of the hash
                EncryptionAndHashing.Encrypt_Memory_DPAPI(ref hashedPassword);

                //store the encrypted password hash in cache
                MemoryCache.Default.Set(User.Identity.GetUserName(),
                                            hashedPassword.ConvertToString(),
                                            new CacheItemPolicy()
                                            {
                                                AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration,
                                                SlidingExpiration = TimeSpan.FromHours(1),    //1 hour - incase user logs out
                                                Priority = CacheItemPriority.Default,
                                                RemovedCallback = onRemove
                                            });              //add item back into cache, if user logged in

            #endregion

            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /RegistrationComplete
        [AllowAnonymous]
        public ActionResult RegistrationConfirmation(string ThisIsTheFirstAccount)
        {
            ViewBag.Title = "Account Registration Complete";
            ViewBag.IsFirstAccount = ThisIsTheFirstAccount.ToLower();
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserMgr.GetLogins(int.Parse(User.Identity.GetUserId()));
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserMgr != null)
            {
                UserMgr.Dispose();
                UserMgr = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserMgr));
        }

        private void AddErrors(IdentityResult result)
        {
            ModelState.AddModelError("", string.Join(" and ", result.Errors));
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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            //addtional step - without a / at the begining, IsLocalUrl will return false
            if (returnUrl.Substring(0, 1) != "/")
                returnUrl = "/" + returnUrl;

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        /// <summary>
        /// Item removed callback, so that the cached data can be reloaded into memory
        /// </summary>
        /// <param name="arguments"></param>
        public void RemovedCallback(CacheEntryRemovedArguments arguments)
        {

            CacheEntryRemovedCallback onRemove = new CacheEntryRemovedCallback(this.RemovedCallback);

            //only if the cache hasnt been manually removed do we re-add it
            if (arguments.RemovedReason != CacheEntryRemovedReason.Removed)
                //generate the user encryption key (used to decrypt the user's private key) from the password and store in cache
                MemoryCache.Default.Set(arguments.CacheItem.Key,
                                        arguments.CacheItem.Value, 
                                        new CacheItemPolicy() { 
                                                                AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration, 
                                                                SlidingExpiration=TimeSpan.FromHours(1), 
                                                                Priority=CacheItemPriority.Default,
                                                                RemovedCallback = onRemove });

        }

        #endregion
    }
}