﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using System.Threading.Tasks;

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
        public ActionResult Index()
        {
            if (Request.QueryString.Get("successaction") == "Account Updated")
            {
                ViewBag.Message = "Account updated successfully.";
            }
            else if (Request.QueryString.Get("successaction") == "Account Deleted")
            {
                ViewBag.Message = "Account delete successfully.";
            }

            return View(UserMgr);
        }

        // GET: UserManager/Edit/5
        public async Task<ActionResult> Edit(int Id)
        {
            UpdateAccountViewModel model = new UpdateAccountViewModel();

            //grab the selected account
            var selectedAccount = await UserMgr.FindByIdAsync(Id);
            if(selectedAccount!=null)
            {
                //put the attributes from this account into the model, so the existing values are displayed
                model.Email = selectedAccount.Email;
                model.Username = selectedAccount.UserName;
                model.FullName = selectedAccount.userFullName;
            }
            else
                ModelState.AddModelError("", "User account does not exsit");

            return View(model);
        }

        // POST: UserManager/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int Id, UpdateAccountViewModel model)
        {
            try
            {

                //grab the account selected
                var selectedAccount = await UserMgr.FindByIdAsync(Id);
                if (selectedAccount != null)
                {

                    //update the account's attributes
                    selectedAccount.userFullName = model.FullName;
                    selectedAccount.UserName = model.Username;
                    selectedAccount.Email = model.Email;

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
                        string errorlist = "";
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

        // GET: UserManager/Delete/5
        public async Task<ActionResult> Delete(int Id)
        {
            UpdateAccountViewModel model = new UpdateAccountViewModel();

            //grab the selected account
            var selectedAccount = await UserMgr.FindByIdAsync(Id);
            if (selectedAccount != null)
            {
                //put the attributes from this account into the model, so the existing values are displayed
                model.Email = selectedAccount.Email;
                model.Username = selectedAccount.UserName;
                model.FullName = selectedAccount.userFullName;
            }
            else
                ModelState.AddModelError("", "User account does not exsit");

            return View(model);
        }

        // POST: UserManager/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int Id, FormCollection collection)
        {
            try
            {

                //grab the account selected
                var selectedAccount = await UserMgr.FindByIdAsync(Id);
                if (selectedAccount != null)
                {

                    //attempt to delete the account
                    var result = await UserMgr.DeleteAsync(selectedAccount);
                    if (result.Succeeded)
                    {
                        //redirect back to the account list, with a success message 
                        return RedirectToAction("Index", "UserManager", new { successaction = "Account Deleted" });
                    }
                    else
                    {
                        //list any errors (e.g. email/username already exists)
                        string errorlist = "";
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }


            //if we got this far, something baaaaad happened
            return View();
        }

        [HttpPost]
        public ActionResult AuthoriseAccount(int id)
        {
            //authorise account
            return RedirectToAction("Index", "UserManager");
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
        public async Task<ActionResult> ResetPassword(int Id, ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("ResetPassword", "UserManager");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserMgr.ChangePasswordAsync(Id, model.OldPassword, model.NewPassword);

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
