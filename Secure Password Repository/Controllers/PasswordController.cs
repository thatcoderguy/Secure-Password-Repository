using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Utilities;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
            //get the root node, and include it's subcategories
            var rootCategoryItem = DatabaseContext.Categories.Include("SubCategories").OrderBy(c => c.CategoryOrder).Single(c => c.CategoryId == 1);

            return View(rootCategoryItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCategoryChildren(Int32 ParentCategoryId)
        {
            var selectedCategoryItem = DatabaseContext.Categories.Include("SubCategories").Include("Passwords").OrderBy(c => c.CategoryOrder).Single(c => c.CategoryId == ParentCategoryId);

            return PartialView("_ReturnCategoryChildren", selectedCategoryItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(Category newCategory)
        {

            if(ModelState.IsValid)
            { 
                //get the root node, and include it's subcategories
                var categoryList = DatabaseContext.Categories.Include("SubCategories").OrderBy(c => c.CategoryOrder).Single(c => c.CategoryId == 1);

                //set the order of the category by getting the number of subcategories
                newCategory.CategoryOrder = (Int16)(categoryList.SubCategories.Count + 1);

                //save the new category
                DatabaseContext.Categories.Add(newCategory);
                await DatabaseContext.SaveChangesAsync();
            }

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
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCategoryPosition(Int32 CategoryId, Int16 NewPosition)
        {
            return Json(null);
        }
    }
}
