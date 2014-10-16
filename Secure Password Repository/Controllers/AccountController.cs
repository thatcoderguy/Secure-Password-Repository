using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using Secure_Password_Repository.ViewModels;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.Settings;
using System.Runtime.Caching;

namespace Secure_Password_Repository.Controllers
{
    [Authorize]
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
            if (ModelState.IsValid)
            {
                
                var user = await UserMgr.FindAsync(model.Username, model.Password);

                if (user != null)
                {
                    //only allow the user to sign in if their account is authorised
                    //this is because an admin needs to authorise new accounts, so that the encryption key can be 
                    //encrypted with the user's public key
                    if (user.isAuthorised)
                    {

                        if (MemoryCache.Default.Get(model.Username) != null)
                        {
                            MemoryCache.Default.Remove(model.Username);
                            MemoryCache.Default.Remove(model.Username + "-connectionId");
                        }

                        //hash and encrypt the user's password - so this can be used to decrypt the user's private key
                        byte[] hashedPassword = EncryptionAndHashing.Hash_SHA1_ToBytes(model.Password);
                        hashedPassword = EncryptionAndHashing.Hash_PBKDF2_ToBytes(hashedPassword, ApplicationSettings.Default.SystemSalt);
                        
                        //in-memory encryption of the hash
                        EncryptionAndHashing.Encrypt_DPAPI(ref hashedPassword);

                        CacheEntryRemovedCallback onRemove = new CacheEntryRemovedCallback(this.RemovedCallback);

                        //store the encrypted memory in cache
                        MemoryCache.Default.Set(model.Username,
                                                hashedPassword.ConvertToString(), 
                                                new CacheItemPolicy() { 
                                                                        AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration, 
                                                                        SlidingExpiration=TimeSpan.FromHours(1),    //1 hour - incase user logs out
                                                                        Priority=CacheItemPriority.Default, 
                                                                        RemovedCallback = onRemove });              //add item back into cache, if user logged in


                        await SignInAsync(user, false);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your account needs to be authorised by an Administrator.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
            if (ModelState.IsValid)
            {
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

                    //Encrypt private key with DPAPI
                    EncryptionAndHashing.Encrypt_DPAPI(ref userPrivateKeyBytes);

                    //hash the user's password,and then use 
                    byte[] hashedPassword = EncryptionAndHashing.Hash_SHA1_ToBytes(model.Password);
                    hashedPassword = EncryptionAndHashing.Hash_PBKDF2_ToBytes(hashedPassword, ApplicationSettings.Default.SystemSalt);

                    //Encrypt privateKey with the user's encryptionkey (based on their password)
                    user.userPrivateKey = EncryptionAndHashing.Encrypt_AES256_ToBytes(userPrivateKeyBytes.ToBase64String(), hashedPassword).ToBase64String();

                    //clear the PrivateKey data - for security
                    Array.Clear(userPrivateKeyBytes, 0, userPrivateKeyBytes.Length);
                    Array.Clear(hashedPassword, 0, hashedPassword.Length);

                    //Keys has been encrypted, the plaintext ones can now be destroyed
                    EncryptionAndHashing.Destroy_RSAKeys();

                #endregion

                if (UserMgr.Users.ToList().Count == 0)
                {

                    #region generate_new_database_symetric_encryption_key_and_encrypt

                        //generate 32 random bytes - this will be the encryption key
                        byte[] DatabaseEncryptionKeyBytes = EncryptionAndHashing.Generate_RandomBytes(32);

                        //first level of encryption - using DPAPI
                        EncryptionAndHashing.Encrypt_DPAPI(ref DatabaseEncryptionKeyBytes);

                        //second level of encryption - using RSA
                        user.userEncryptionKey = EncryptionAndHashing.Encrypt_RSA_ToBytes(DatabaseEncryptionKeyBytes.ToBase64String(), user.userPublicKey).ToBase64String();

                        //clear the original data
                        Array.Clear(DatabaseEncryptionKeyBytes, 0, DatabaseEncryptionKeyBytes.Length);

                    #endregion

                    user.isAuthorised = true;
                    FirstUserAccount = "Yes";
                    UserDefaultRole = "Administrator";

                }
                else
                {

                    //user needs to be authorised by an admin, so the encryption key can be copied to the user's account
                    user.isAuthorised = false;
                    FirstUserAccount = "No";
                    UserDefaultRole = "User";

                }

                if (RoleMgr.RoleExists(UserDefaultRole))
                {
                    IdentityResult result = await UserMgr.CreateAsync(user, model.Password);
                    string blah = user.PasswordHash;
                    if (result.Succeeded)
                    {
                        //await SignInAsync(user, isPersistent: false);

                        result = await UserMgr.AddToRoleAsync(user.Id, UserDefaultRole);

                        return RedirectToAction("RegistrationConfirmation", new { ThisIsTheFirstAccount = FirstUserAccount });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                } 
                else
                {
                    AddErrors(IdentityResult.Failed(new string []{ "The role: " + UserDefaultRole + " does not exist" }));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserMgr.FindByNameAsync(model.Email);
                if (user == null || !(await UserMgr.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                    return View();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserMgr.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserMgr.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
        public ActionResult ResetPassword(string code)
        {
            if (code == null) 
            {
                return View("Error");
            }
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserMgr.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found.");
                    return View();
                }
                IdentityResult result = await UserMgr.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    AddErrors(result);
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserMgr.ChangePasswordAsync(int.Parse(User.Identity.GetUserId()), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
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
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
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
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
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

        private void SendEmail(string email, string callbackUrl, string subject, string message)
        {
            // For information on sending mail, please visit http://go.microsoft.com/fwlink/?LinkID=320771



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