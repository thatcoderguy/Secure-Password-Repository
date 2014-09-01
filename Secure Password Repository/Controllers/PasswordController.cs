using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Utilities;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                var categoryList = DatabaseContext.Categories.Include("SubCategories").Single(c => c.CategoryId == newCategory.Category_ParentID);

                //set the order of the category by getting the number of subcategories
                newCategory.CategoryOrder = (Int16)(categoryList.SubCategories.Count + 1);

                //save the new category
                DatabaseContext.Categories.Add(newCategory);
                await DatabaseContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


        // POST: Password/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategory(Category editedCategory)
        {
            try
            {
                if(ModelState.IsValid) {
                
                    DatabaseContext.Entry(editedCategory).State = EntityState.Modified;
                    DatabaseContext.Entry(editedCategory).Property("CategoryOrder").IsModified=false;
                    await DatabaseContext.SaveChangesAsync();

                    return Json(editedCategory);
                }
                else
                {
                    return Json("failed");
                }
            }
            catch(Exception ex)
            {
                return Json("failed");
            }
        }

        // POST: Password/DeleteCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int id)
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
        public async Task<string> UpdateCategoryPosition(Int32 CategoryId, Int16 NewPosition, Int16 OldPosition)
        {

            try
            {
                //moved down
                if (NewPosition > OldPosition)
                {
                    //disable auto detect changes (as this slows everything down)
                    DatabaseContext.Configuration.AutoDetectChangesEnabled = false;

                    //get the category that has been moved
                    Category currentCategory = DatabaseContext.Categories.Single(c => c.CategoryId == CategoryId);

                    //loop through the parent's sub categories that are below the moved category, but dont grab the ones above where the category is being moved to
                    foreach (Category childCategory in currentCategory.Parent_Category.SubCategories.Where(c => c.CategoryOrder > OldPosition).Where(c => c.CategoryOrder < NewPosition + 1))
                    {
                        //move the category up
                        childCategory.CategoryOrder--;

                        //tell EF that the category has been altered
                        DatabaseContext.Entry(childCategory).State = EntityState.Modified;
                    }

                    //set its new position
                    currentCategory.CategoryOrder = NewPosition;
                    DatabaseContext.Entry(currentCategory).State = EntityState.Modified;

                    DatabaseContext.Configuration.AutoDetectChangesEnabled = true;

                    //save changes to database
                    await DatabaseContext.SaveChangesAsync();

                }
                //moved up
                else
                {
                    //disable auto detect changes (as this slows everything down)
                    DatabaseContext.Configuration.AutoDetectChangesEnabled = false;

                    //get the category that has been moved
                    Category currentCategory = DatabaseContext.Categories.Single(c => c.CategoryId == CategoryId);

                    //loop through the parent's sub categories that are above the moved category, but dont grab the ones below where the category is being moved to
                    foreach (Category childCategory in currentCategory.Parent_Category.SubCategories.Where(c => c.CategoryOrder < OldPosition).Where(c => c.CategoryOrder > NewPosition - 1))
                    {
                        //move the category up
                        childCategory.CategoryOrder++;

                        //tell EF that the category has been altered
                        DatabaseContext.Entry(childCategory).State = EntityState.Modified;
                    }

                    //set its new position
                    currentCategory.CategoryOrder = NewPosition;
                    DatabaseContext.Entry(currentCategory).State = EntityState.Modified;

                    DatabaseContext.Configuration.AutoDetectChangesEnabled = true;

                    //save changes to database
                    await DatabaseContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return "failed";
            }

            return "complete";
        }
    }
}
