using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Secure_Password_Repository.Controllers
{
    [Authorize(Roles = "Administrator, Super User, User")]
    public class PasswordController : Controller
    {

        ApplicationDbContext DatabaseContext = new ApplicationDbContext();

        // GET: Password
        public ActionResult Index()
        {
            var CategoryList = DatabaseContext.tblCategory.Include("SubCategories").OrderBy(c => c.CategoryOrder).Single(c => c.CategoryId == 1);
            return View(CategoryList);
        }

        [HttpGet]
        public ActionResult GetChildren(Int32 ParentCategoryId)
        {
            return View();
            //are the children catgories or passwords?
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(int ParentId, string CategoryName)
        {
            var CategoryList = DatabaseContext.tblCategory.Include("SubCategories").OrderBy(c => c.CategoryOrder).Single(c => c.CategoryId == 1);
            //CategoryList.SubCategories.Add();
            //C
            //Category newCategory = new Category { }

            return RedirectToAction("Index");
        }

        // GET: Password/EditCategory/5
        public ActionResult EditCategory(int id)
        {
            return View();
        }

        // POST: Password/EditCategory/5
        [HttpPost]
        public ActionResult EditCategory(int id, FormCollection collection)
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

        

        // GET: Password/DeleteCategory/5
        public ActionResult DeleteCategory(int id)
        {
            return View();
        }

        // POST: Password/DeleteCategory/5
        [HttpPost]
        public ActionResult DeleteCategory(int id, FormCollection collection)
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

        [HttpPost]
        public ActionResult UpdateCategoryPosition(Int32 CategoryId, Int16 NewPosition)
        {
            HttpRequestMessage httpRequestMessage = System.Web.HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
            SecurityUtilities.ValidateRequestHeader(httpRequestMessage);
            return Json(null);
        }
    }
}
