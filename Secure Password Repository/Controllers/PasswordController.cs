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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Secure_Password_Repository.Settings;
using Secure_Password_Repository.Hubs;

namespace Secure_Password_Repository.Controllers
{
    [Authorize(Roles = "Administrator, Super User, User")]
    public class PasswordController : Controller
    {

        private ApplicationDbContext DatabaseContext = new ApplicationDbContext();

        private ApplicationUserManager _userManager;

        public PasswordController()
        {
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

        // GET: Password
        public ActionResult Index()
        {

            //get the root node, and include it's subcategories
            var rootCategoryItem = DatabaseContext.Categories
                .Include("SubCategories")
                .ToList()
                .Select(c => new Category()
                {
                    SubCategories = c.SubCategories
                    .Where(sub => !sub.Deleted)
                    .OrderBy(sub => sub.CategoryOrder)
                    .Select(s => new Category()                 //remove subcategories and passwords - as these cant be mapped
                    {
                        Category_ParentID = s.Category_ParentID,
                        CategoryId = s.CategoryId,
                        CategoryName = s.CategoryName,
                        CategoryOrder = s.CategoryOrder
                    })
                    .ToList(),                                  //make sure only undeleted subcategories are returned

                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Category_ParentID = c.Category_ParentID,
                    CategoryOrder = c.CategoryOrder,
                    Parent_Category = c.Parent_Category,
                    Deleted = c.Deleted
                })
                .Single(c => c.CategoryId == 1);

            //create the model view from the model
            AutoMapper.Mapper.CreateMap<Category, CategoryItem>();
            CategoryItem rootCategoryViewItem = AutoMapper.Mapper.Map<CategoryItem>(rootCategoryItem);

            //return wrapper class
            return View(new CategoryDisplayItem()
            {
                categoryListItem = rootCategoryViewItem,
                categoryAddItem = new CategoryAdd()
                {
                    Category_ParentID = rootCategoryViewItem.CategoryId
                }
            });
        }

        #region CategoryActions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCategoryChildren(Int32 ParentCategoryId)
        {
            try
            {

                var userId = int.Parse(User.Identity.GetUserId());

                //return the selected item - with its children
                var selectedCategoryItem = DatabaseContext.Categories
                        .Include("SubCategories")
                        .Include("Passwords")
                        .ToList().Select(c => new Category()
                        {
                            SubCategories = c.SubCategories
                            .Where(sub => !sub.Deleted)
                            .OrderBy(sub => sub.CategoryOrder)
                            .Select(s => new Category()         //remove subcategories and passwords - as these cant be mapped
                            {
                                Category_ParentID = s.Category_ParentID,
                                CategoryId = s.CategoryId,
                                CategoryName = s.CategoryName,
                                CategoryOrder = s.CategoryOrder
                            })
                            .ToList(),                          //make sure only undeleted subcategories are returned

                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            Category_ParentID = c.Category_ParentID,
                            CategoryOrder = c.CategoryOrder,
                            Parent_Category = c.Parent_Category,
                            Deleted = c.Deleted,

                            Passwords = c.Passwords
                            .Where(pass => !pass.Deleted
                                        && (DatabaseContext.UserPasswords
                                                    .Any(up => up.PasswordId == pass.PasswordId && up.Id == userId))
                                        || (
                                                User.IsInRole("Administrator")
                                                && ApplicationSettings.Default.AdminsHaveAccessToAllPasswords
                                           ))
                            .OrderBy(pass => pass.PasswordOrder)
                            .Select(pass => new Password()
                            {
                                CreatedDate = pass.CreatedDate,
                                Creator_Id = pass.Creator_Id,
                                Creator = pass.Creator,
                                Deleted = pass.Deleted,
                                Description = pass.Description,
                                EncryptedPassword = pass.EncryptedPassword,
                                EncryptedSecondCredential = pass.EncryptedSecondCredential,
                                EncryptedUserName = pass.EncryptedUserName,
                                Location = pass.Location,
                                Notes = pass.Notes,
                                ModifiedDate = pass.ModifiedDate,
                                PasswordId = pass.PasswordId,
                                PasswordOrder = pass.PasswordOrder,
                                Parent_Category  = pass.Parent_Category,
                                Parent_CategoryId =pass.Parent_CategoryId,
                                Parent_UserPasswords = pass.Parent_UserPasswords.Where(p => p.Id == userId).ToList()
                            })
                            .ToList()                           //make sure only undeleted passwords - that the current user has acccess to - are returned
                        })
                        .Single(c => c.CategoryId == ParentCategoryId);

                //create view model from model
                AutoMapper.Mapper.CreateMap<Category, CategoryItem>();
                AutoMapper.Mapper.CreateMap<Password, PasswordItem>();
                AutoMapper.Mapper.CreateMap<UserPassword, PasswordUserPermission>();
                CategoryItem selectedCategoryViewItem = AutoMapper.Mapper.Map<CategoryItem>(selectedCategoryItem);

                //return wrapper class
                return PartialView("_ReturnCategoryChildren", new CategoryDisplayItem()
                {
                    categoryListItem = selectedCategoryViewItem,
                    categoryAddItem = new CategoryAdd()
                    {
                        Category_ParentID = selectedCategoryViewItem.CategoryId
                    },
                    passwordAddItem = new PasswordAdd()
                    {
                        Parent_CategoryId = (Int32)selectedCategoryViewItem.CategoryId
                    }
                });

            }
            catch { }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(CategoryAdd model)
        {
            if (User.IsInRole(ApplicationSettings.Default.RoleAllowAddCategories) || User.IsInRole("Administrator"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //create model from view model
                        AutoMapper.Mapper.CreateMap<CategoryAdd, Category>();
                        Category newCategory = AutoMapper.Mapper.Map<Category>(model);

                        //get the parent node, and include it's subcategories
                        var categoryList = DatabaseContext.Categories.Include("SubCategories").Single(c => c.CategoryId == model.Category_ParentID);

                        //set the order of the category by getting the number of subcategories
                        if (categoryList.SubCategories.Count > 0)
                            newCategory.CategoryOrder = (Int16)(categoryList.SubCategories.Where(c => !c.Deleted).Max(c => c.CategoryOrder) + 1);
                        else
                            newCategory.CategoryOrder = 1;

                        //save the new category
                        DatabaseContext.Categories.Add(newCategory);
                        await DatabaseContext.SaveChangesAsync();

                        //map new category to display view model
                        AutoMapper.Mapper.CreateMap<Category, CategoryItem>();
                        CategoryItem returnCategoryViewItem = AutoMapper.Mapper.Map<CategoryItem>(newCategory);

                        string categoryPartialView = RenderViewContent.RenderPartialToString(this, "_CategoryItem", returnCategoryViewItem);

                        PushNotifications.sendAddedCategoryDetails(categoryPartialView, newCategory.Category_ParentID);

                        return PartialView("_CategoryItem", returnCategoryViewItem);

                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    }

                }
                catch { }
            }


            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

        }

        // POST: Password/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategory(CategoryEdit model)
        {
            if (User.IsInRole(ApplicationSettings.Default.RoleAllowEditCategories) || User.IsInRole("Administrator"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        AutoMapper.Mapper.CreateMap<CategoryEdit, Category>();
                        Category editedCategory = AutoMapper.Mapper.Map<Category>(model);

                        //update the category
                        DatabaseContext.Entry(editedCategory).State = EntityState.Modified;

                        //dont update the categoryorder value
                        DatabaseContext.Entry(editedCategory).Property("CategoryOrder").IsModified = false;
                        DatabaseContext.Entry(editedCategory).Property("Category_ParentID").IsModified = false;

                        //save changes
                        await DatabaseContext.SaveChangesAsync();

                        PushNotifications.sendUpdatedCategoryDetails(model);

                        //return the object, so that the UI can be updated
                        return Json(model);
                    }
                }
                catch { }
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

        }

        // POST: Password/DeleteCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCategory(int CategoryId)
        {
            if (User.IsInRole(ApplicationSettings.Default.RoleAllowDeleteCategories) || User.IsInRole("Administrator"))
            {
                try
                {

                    //get the category item to delete
                    var deletedCategory = DatabaseContext.Categories
                                                            .Include("Parent_Category")
                                                            .Single(c => c.CategoryId == CategoryId);

                    //load in the parent's subcategories
                    DatabaseContext.Entry(deletedCategory.Parent_Category)
                                            .Collection(c => c.SubCategories)
                                            .Query()
                                            .Where(c => !c.Deleted)
                                            .Load();

                    //loop through and adjust category order
                    foreach (Category siblingCategory in deletedCategory.Parent_Category.SubCategories
                                                                                            .Where(c => c.CategoryOrder > deletedCategory.CategoryOrder)
                                                                                            .Where(c => !c.Deleted))
                    {
                        siblingCategory.CategoryOrder--;
                        DatabaseContext.Entry(siblingCategory).State = EntityState.Modified;
                    }

                    //set the item to deleted
                    deletedCategory.Deleted = true;

                    //move it to the very end
                    deletedCategory.CategoryOrder = 9999;

                    //set the item to be deleted
                    DatabaseContext.Entry(deletedCategory).State = EntityState.Modified;

                    //save changes
                    await DatabaseContext.SaveChangesAsync();

                    //proxies are no longer needed, so remove to avoid the "cicular reference" issue
                    if (deletedCategory.SubCategories != null)
                    {
                        deletedCategory.SubCategories.Clear();
                        deletedCategory.SubCategories = null;
                    }
                    if (deletedCategory.Passwords != null)
                    {
                        deletedCategory.Passwords.Clear();
                        deletedCategory.Passwords = null;
                    }

                    deletedCategory.Parent_Category = null;

                    PushNotifications.sendDeletedCategoryDetails(deletedCategory);

                    //return the item, so that it can be removed from the UI
                    return Json(deletedCategory);
                }
                catch { }
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        #endregion

        #region PasswordActions

        public ActionResult AddPassword(int ParentCategoryId)
        {
            return View("AddPassword", new PasswordAdd { Parent_CategoryId = ParentCategoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPassword(PasswordAdd model)
        {

            if (ModelState.IsValid)
            {
                AutoMapper.Mapper.CreateMap<PasswordAdd, Password>();
                Password newPasswordItem = AutoMapper.Mapper.Map<Password>(model);

                var userId = int.Parse(User.Identity.GetUserId());
                var user = await UserMgr.FindByIdAsync(userId);

                //get the parent category node, and include it's passwords
                var passwordList = DatabaseContext.Categories.Include("Passwords").Single(c => c.CategoryId == model.Parent_CategoryId);

                //set the order of the category by getting the number of subcategories
                if (passwordList.Passwords.Count > 0)
                    newPasswordItem.PasswordOrder = (Int16)(passwordList.Passwords.Where(p => !p.Deleted).Max(p => p.PasswordOrder) + 1);
                else
                    newPasswordItem.PasswordOrder = 1;

                newPasswordItem.Parent_CategoryId = model.Parent_CategoryId;
                newPasswordItem.CreatedDate = DateTime.Now;
                newPasswordItem.Creator_Id = user.Id;

                //save the new category
                DatabaseContext.Passwords.Add(newPasswordItem);
                await DatabaseContext.SaveChangesAsync();

                //also create the UserPassword record
                UserPassword newUserPassword = new UserPassword()
                {
                    CanDeletePassword = true,
                    CanEditPassword = true,
                    PasswordId = newPasswordItem.PasswordId,
                    Id = user.Id
                };

                DatabaseContext.UserPasswords.Add(newUserPassword);
                await DatabaseContext.SaveChangesAsync();

                return View("thanks");
            }

            return View(model);
        }

        #endregion

        #region CategoryAndPasswordActions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePosition(Int32 ItemId, Int16 NewPosition, Int16 OldPosition, bool isCategoryItem)
        {
            if ((User.IsInRole(ApplicationSettings.Default.RoleAllowEditCategories) && isCategoryItem) || !isCategoryItem || User.IsInRole("Administrator"))
            {
                try
                {
                    //disable auto detect changes (as this slows everything down)
                    DatabaseContext.Configuration.AutoDetectChangesEnabled = false;

                    //moved down
                    if (NewPosition > OldPosition)
                    {


                        if (isCategoryItem)
                        {

                            #region movecategorydown

                            //get the category item to move
                            var currentCategory = DatabaseContext.Categories
                                                                    .Include("Parent_Category")
                                                                    .Single(c => c.CategoryId == ItemId);

                            //load in the parent's subcategories
                            DatabaseContext.Entry(currentCategory.Parent_Category)
                                                    .Collection(c => c.SubCategories)
                                                    .Query()
                                                    .Where(c => !c.Deleted)
                                                    .Load();

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

                            //save changes to database
                            await DatabaseContext.SaveChangesAsync();

                            PushNotifications.sendUpdatedItemPosition(ItemId.ToString(), NewPosition);

                            #endregion

                        }
                        else
                        {

                            #region movepasswordown

                            //get the password item to move
                            var currentPassword = DatabaseContext.Passwords
                                                                    .Include("Parent_Category")
                                                                    .Single(p => p.PasswordId == ItemId);

                            //load in the parent's subcategories
                            DatabaseContext.Entry(currentPassword.Parent_Category)
                                                    .Collection(c => c.Passwords)
                                                    .Query()
                                                    .Where(p => !p.Deleted)
                                                    .Load();

                            //loop through the parent's passwords that are below the moved password, but dont grab the ones above where the password is being moved to
                            foreach (Password childPassword in currentPassword.Parent_Category.Passwords.Where(p => p.PasswordOrder > OldPosition).Where(p => p.PasswordOrder < NewPosition + 1))
                            {
                                //move the password up
                                childPassword.PasswordOrder--;

                                //tell EF that the password has been altered
                                DatabaseContext.Entry(childPassword).State = EntityState.Modified;
                            }

                            //set its new position
                            currentPassword.PasswordOrder = NewPosition;
                            DatabaseContext.Entry(currentPassword).State = EntityState.Modified;

                            //save changes to database
                            await DatabaseContext.SaveChangesAsync();

                            PushNotifications.sendUpdatedItemPosition("Password-" + ItemId.ToString(), NewPosition);

                            #endregion

                        }

                    }
                    //moved up
                    else
                    {

                        if (isCategoryItem)
                        {

                            #region movecategoryup

                            //get the category item to move
                            var currentCategory = DatabaseContext.Categories
                                                                    .Include("Parent_Category")
                                                                    .Single(c => c.CategoryId == ItemId);

                            //load in the parent's subcategories
                            DatabaseContext.Entry(currentCategory.Parent_Category)
                                                    .Collection(c => c.SubCategories)
                                                    .Query()
                                                    .Where(c => !c.Deleted)
                                                    .Load();

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

                            //save changes to database
                            await DatabaseContext.SaveChangesAsync();

                            PushNotifications.sendUpdatedItemPosition(ItemId.ToString(), NewPosition);

                            #endregion

                        }
                        else
                        {

                            #region movepasswordup

                            //get the password item to move
                            var currentPassword = DatabaseContext.Passwords
                                                                    .Include("Parent_Category")
                                                                    .Single(p => p.PasswordId == ItemId);

                            //load in the parent's subcategories
                            DatabaseContext.Entry(currentPassword.Parent_Category)
                                                    .Collection(c => c.Passwords)
                                                    .Query()
                                                    .Where(p => !p.Deleted)
                                                    .Load();

                            //loop through the parent's passwords that are below the moved password, but dont grab the ones above where the password is being moved to
                            foreach (Password childPassword in currentPassword.Parent_Category.Passwords.Where(p => p.PasswordOrder < OldPosition).Where(p => p.PasswordOrder > NewPosition - 1))
                            {
                                //move the password up
                                childPassword.PasswordOrder++;

                                //tell EF that the password has been altered
                                DatabaseContext.Entry(childPassword).State = EntityState.Modified;
                            }

                            //set its new position
                            currentPassword.PasswordOrder = NewPosition;
                            DatabaseContext.Entry(currentPassword).State = EntityState.Modified;

                            //save changes to database
                            await DatabaseContext.SaveChangesAsync();

                            PushNotifications.sendUpdatedItemPosition("Password-" + ItemId.ToString(), NewPosition);

                            #endregion

                        }

                    }

                    DatabaseContext.Configuration.AutoDetectChangesEnabled = true;

                    return Json(new
                    {
                        Status = "Completed"
                    });

                }
                catch { }

            }

            return Json(new
            {
                Status = "Failed"
            });

        }

        #endregion

    }
}
