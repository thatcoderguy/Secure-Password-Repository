using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Secure_Password_Repository.Controllers
{
    [Authorize(Roles = "Administrator, Super User, User")]
    public class PasswordController : Controller
    {

        ApplicationDbContext DatabaseContext = new ApplicationDbContext();

        // GET: Password
        public ActionResult Index()
        {

            DatabaseContext.Configuration.LazyLoadingEnabled = false;

            //get the root node, and include it's subcategories
            var rootCategoryItem = DatabaseContext.Categories
                .Include("SubCategories")
                .ToList()
                .Select(c => new Category()
                {
                    SubCategories = c.SubCategories.Where(sub => !sub.Deleted).ToList(),        //make sure only undeleted subcategories are returned
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Category_ParentID = c.Category_ParentID,
                    CategoryOrder = c.CategoryOrder,
                    Parent_Category = c.Parent_Category,
                    Deleted = c.Deleted
                })
                .Single(c => c.CategoryId == 1);

            DatabaseContext.Configuration.LazyLoadingEnabled = true;
             
            return View(rootCategoryItem);
        }

        #region CategoryActions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCategoryChildren(Int32 ParentCategoryId)
        {

            try
            {

                //return the selected item - with its children
                var selectedCategoryItem = DatabaseContext.Categories
                        .Include("SubCategories")
                        .Include("Passwords")
                        .ToList().Select(c => new Category()
                        {
                            SubCategories = c.SubCategories.Where(sub => !sub.Deleted).ToList(),    //make sure only undeleted subcategories are returned
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            Category_ParentID = c.Category_ParentID,
                            CategoryOrder = c.CategoryOrder,
                            Parent_Category = c.Parent_Category,
                            Passwords = c.Passwords.Where(pass => !pass.Deleted).ToList(),          //make sure only undeleted passwords are returned
                            Deleted = c.Deleted
                        })
                        .Single(c => c.CategoryId == ParentCategoryId);
                    
                return PartialView("_ReturnCategoryChildren", selectedCategoryItem);

            } catch { }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(Category newCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //get the root node, and include it's subcategories
                    var categoryList = DatabaseContext.Categories.Include("SubCategories").Single(c => c.CategoryId == newCategory.Category_ParentID);

                    //set the order of the category by getting the number of subcategories
                    if (categoryList.SubCategories.Count > 0)
                        newCategory.CategoryOrder = (Int16)(categoryList.SubCategories.Where(c => !c.Deleted).Max(c => c.CategoryOrder) + 1);
                    else
                        newCategory.CategoryOrder = 1;

                    //save the new category
                    DatabaseContext.Categories.Add(newCategory);
                    await DatabaseContext.SaveChangesAsync();

                    return PartialView("_CategoryModelItem", newCategory);

                } else {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }

            } catch { }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

        }

        // POST: Password/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategory(Category editedCategory)
        {
            try
            {
                if(ModelState.IsValid) {
                
                    //update the category
                    DatabaseContext.Entry(editedCategory).State = EntityState.Modified;

                    //dont update the categoryorder value
                    DatabaseContext.Entry(editedCategory).Property("CategoryOrder").IsModified=false;

                    //save changes
                    await DatabaseContext.SaveChangesAsync();

                    //return the object, so that the UI can be updated
                    return Json(editedCategory);
                }
            } catch {}

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

        }

        // POST: Password/DeleteCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCategory(int CategoryId)
        {
            try
            {

                //get the category item to delete
                var deleteCategory = DatabaseContext.Categories
                                                        .Include("Parent_Category")
                                                        .Single(c => c.CategoryId == CategoryId);

                //load in the parent's subcategories
                DatabaseContext.Entry(deleteCategory.Parent_Category)
                                        .Collection(c => c.SubCategories)
                                        .Query()
                                        .Where(c => !c.Deleted)
                                        .Load();

                //loop through and adjust category order
                foreach(Category siblingCategory in deleteCategory.Parent_Category.SubCategories
                                                                                        .Where(c => c.CategoryOrder > deleteCategory.CategoryOrder)
                                                                                        .Where(c => !c.Deleted))
                {
                    siblingCategory.CategoryOrder--;
                    DatabaseContext.Entry(siblingCategory).State = EntityState.Modified;
                }

                //set the item to deleted
                deleteCategory.Deleted = true;

                //move it to the very end
                deleteCategory.CategoryOrder = 9999;

                //set the item to be deleted
                DatabaseContext.Entry(deleteCategory).State = EntityState.Modified;

                //save changes
                await DatabaseContext.SaveChangesAsync();

                //proxies are no longer needed, so remove to avoid the "cicular reference" issue
                if (deleteCategory.SubCategories != null) {
                    deleteCategory.SubCategories.Clear();
                    deleteCategory.SubCategories = null;
                }
                if (deleteCategory.Passwords != null){
                    deleteCategory.Passwords.Clear();
                    deleteCategory.Passwords = null;
                }
                deleteCategory.Parent_Category = null;

                //return the item, so that it can be removed from the UI
                return Json(deleteCategory);
            }
            catch {}

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        #endregion

        #region PasswordActions

        public ActionResult CreatePassword(int ParentCategoryId)
        {
            Password n = new Password();
            n.Parent_CategoryId = ParentCategoryId;
            return View(n);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePassword(int ParentCategoryId, Password model)
        {
            //Password n = new Password();
            model.Parent_CategoryId = ParentCategoryId;
            return View(model);
        }

        #endregion

        #region CategoryAndPasswordActions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePosition(Int32 ItemId, Int16 NewPosition, Int16 OldPosition, bool isCategoryItem)
        {

            try
            {
                //moved down
                if (NewPosition > OldPosition)
                {
                    //disable auto detect changes (as this slows everything down)
                    DatabaseContext.Configuration.AutoDetectChangesEnabled = false;

                    //get the category that has been moved
                    Category currentCategory = DatabaseContext.Categories.Single(c => c.CategoryId == ItemId);

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
                    Category currentCategory = DatabaseContext.Categories.Single(c => c.CategoryId == ItemId);

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
            catch
            {
                return Json(new
                {
                    Status = "Failed"
                });
            }

            return Json(new
            {
                Status = "Completed"
            });
        }

        #endregion

    }
}
