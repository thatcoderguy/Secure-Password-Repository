using System;
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
            return View(UserMgr);
        }

        // GET: UserManager/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserManager/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserManager/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        // GET: UserManager/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserManager/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
