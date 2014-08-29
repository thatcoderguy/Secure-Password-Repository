﻿using Secure_Password_Repository.Database;
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
                var categoryList = DatabaseContext.Categories.Include("SubCategories").OrderBy(c => c.CategoryOrder).Single(c => c.CategoryId == newCategory.Category_ParentID);

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
        public bool UpdateCategoryPosition(Int32 CategoryId, Int16 NewPosition, Int16 OldPosition)
        {

            //moved down
            if (NewPosition > OldPosition)
            {
                //disable auto detect changes (as this slows everything down)
                DatabaseContext.Configuration.AutoDetectChangesEnabled = false;

                //get the parent category of the category that has been moved
                Category parentCategory = DatabaseContext.Categories.Single(c => c.CategoryId == CategoryId).Parent_Category;

                //loop through the parent's sub categories that are below the moved category, but dont grab the ones above where the category is being moved to
                foreach (Category childCategory in parentCategory.SubCategories.Where(c => c.CategoryOrder > OldPosition).Where(c => c.CategoryOrder < NewPosition+1))
                {
                    //move the category up
                    childCategory.CategoryOrder--;

                    //tell EF that the category has been altered
                    DatabaseContext.Entry(childCategory).State = EntityState.Modified;
                }

                //get the category that has been moved
                Category currentCategory = parentCategory.SubCategories.Single(c => c.CategoryOrder == OldPosition);

                //set its new position
                currentCategory.CategoryOrder = NewPosition;
                DatabaseContext.Entry(currentCategory).State = EntityState.Modified;

                DatabaseContext.Configuration.AutoDetectChangesEnabled = true;

                //save changes to database
                DatabaseContext.SaveChanges();

            }
            //moved up
            else
            {
                //disable auto detect changes (as this slows everything down)
                DatabaseContext.Configuration.AutoDetectChangesEnabled = false;

                //get the parent category of the category that has been moved
                Category parentCategory = DatabaseContext.Categories.Single(c => c.CategoryId == CategoryId).Parent_Category;

                //loop through the parent's sub categories that are above the moved category, but dont grab the ones below where the category is being moved to
                foreach (Category childCategory in parentCategory.SubCategories.Where(c => c.CategoryOrder < OldPosition).Where(c => c.CategoryOrder > NewPosition-1))
                {
                    //move the category up
                    childCategory.CategoryOrder++;

                    //tell EF that the category has been altered
                    DatabaseContext.Entry(childCategory).State = EntityState.Modified;
                }

                //get the category that has been moved
                Category currentCategory = parentCategory.SubCategories.Single(c => c.CategoryId == CategoryId);

                //set its new position
                currentCategory.CategoryOrder = NewPosition;
                DatabaseContext.Entry(currentCategory).State = EntityState.Modified;

                DatabaseContext.Configuration.AutoDetectChangesEnabled = true;

                //save changes to database
                DatabaseContext.SaveChanges();
            }

            return true;
        }
    }
}
