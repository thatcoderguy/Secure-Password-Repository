using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Secure_Password_Repository.Controllers;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Settings;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(PasswordController), "AutoMapperStart")]
namespace Secure_Password_Repository.Controllers
{
    [Authorize(Roles = "Administrator, Super User, User")]
    #if !DEBUG
    [RequireHttps] //apply to all actions in controller
    #endif
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

        /// <summary>
        /// Setup all of the automapper maps that will be used throughout this controller - this is called by WebActivatorEx
        /// </summary>
        public static void AutoMapperStart()
        {
            AutoMapper.Mapper.CreateMap<Category, CategoryItem>();
            AutoMapper.Mapper.CreateMap<Category, CategoryDelete>();
            AutoMapper.Mapper.CreateMap<CategoryAdd, Category>();
            AutoMapper.Mapper.CreateMap<CategoryEdit, Category>();

            AutoMapper.Mapper.CreateMap<Category[], IList<CategoryItem>>();
            AutoMapper.Mapper.CreateMap<Password[], IList<PasswordItem>>();
            AutoMapper.Mapper.CreateMap<UserPassword[], IList<PasswordUserPermission>>().ReverseMap();

            AutoMapper.Mapper.CreateMap<UserPassword, PasswordUserPermission>().ReverseMap();

            AutoMapper.Mapper.CreateMap<Password, PasswordItem>();
            AutoMapper.Mapper.CreateMap<Password, PasswordEdit>();
            AutoMapper.Mapper.CreateMap<Password, PasswordDisplay>();
            AutoMapper.Mapper.CreateMap<Password, PasswordDelete>();
            AutoMapper.Mapper.CreateMap<PasswordAdd, Password>();
            AutoMapper.Mapper.CreateMap<PasswordEdit, PasswordDisplay>();
        }

        // GET: Password
        public ActionResult Index()
        {

            //get the root node, and include it's subcategories
            var rootCategoryItem = DatabaseContext.Categories
                                                        .Where(c => c.CategoryId == 1)
                                                        .Include(c => c.SubCategories)
                                                        .ToList()
                                                        .Select(c => new Category()
                                                        {
                                                            SubCategories = c.SubCategories
                                                                                .Where(sub => !sub.Deleted)
                                                                                .OrderBy(sub => sub.CategoryOrder)
                                                                                .ToList(),                         //make sure only undeleted subcategories are returned

                                                            CategoryId = c.CategoryId,
                                                            CategoryName = c.CategoryName,
                                                            Category_ParentID = c.Category_ParentID,
                                                            CategoryOrder = c.CategoryOrder,
                                                            Parent_Category = c.Parent_Category,
                                                            Deleted = c.Deleted
                                                        })
                                                        .SingleOrDefault();

            //root item not found
            if (rootCategoryItem == null)
                return View(new CategoryDisplayItem() {
                                                            categoryListItem = new CategoryItem(),
                                                            categoryAddItem = new CategoryAdd() { Category_ParentID = null }
                                                      });

            //create the model view from the model
            CategoryItem rootCategoryViewItem = AutoMapper.Mapper.Map<CategoryItem>(rootCategoryItem);

            //return wrapper class
            return View(new CategoryDisplayItem()
            {
                categoryListItem = rootCategoryViewItem,
                categoryAddItem = new CategoryAdd() { Category_ParentID = rootCategoryViewItem.CategoryId }
            });
        }

        #region CategoryActions

        // POST: Password/GetCategoryChildren/23
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCategoryChildren(Int32 ParentCategoryId)
        {
            try
            {
                int UserId = User.Identity.GetUserId().ToInt();
                bool userIsAdmin = User.IsInRole("Administrator");

                //load all of the userpassword item that are related to the passwords being return - this stops multiple hits on the DB when calling Any below
                var UserPasswordList = DatabaseContext.UserPasswords.Where(up => up.Id == UserId && 
                                                                                DatabaseContext.Passwords
                                                                                                    .Where(p => p.Parent_CategoryId == ParentCategoryId)
                                                                                                    .Select(p => p.PasswordId)
                                                                                                    .Contains(up.PasswordId))
                                                                                                    .ToList();

                //return the selected item - with its children
                var selectedCategoryItem = DatabaseContext.Categories
                                                            .Where(c => c.CategoryId == ParentCategoryId)
                                                            .Include(c => c.SubCategories)
                                                            .Include(c => c.Passwords)
                                                            .Include(c => c.Passwords.Select(p => p.Creator))
                                                            .ToList()
                                                            .Select(c => new Category()
                                                            {

                                                                SubCategories = c.SubCategories
                                                                                    .Where(sub => !sub.Deleted)
                                                                                    .OrderBy(sub => sub.CategoryOrder)
                                                                                    .ToList(),                          //make sure only undeleted subcategories are returned

                                                                Passwords = c.Passwords
                                                                                    .Where(pass => !pass.Deleted
                                                                                    && (
                                                                                        (UserPasswordList.Any(up => up.PasswordId == pass.PasswordId && up.Id == UserId))
                                                                                        || (userIsAdmin && ApplicationSettings.Default.AdminsHaveAccessToAllPasswords)
                                                                                        || pass.Creator_Id == UserId
                                                                                        )
                                                                                        )   //make sure only undeleted passwords - that the current user has acccess to - are returned
                                                                                    .OrderBy(pass => pass.PasswordOrder)
                                                                                    .ToList(),

                                                                CategoryId = c.CategoryId,
                                                                CategoryName = c.CategoryName,
                                                                Category_ParentID = c.Category_ParentID,
                                                                CategoryOrder = c.CategoryOrder,
                                                                Parent_Category = c.Parent_Category,
                                                                Deleted = c.Deleted
                                                            })
                                                            .SingleOrDefault();

                //selected item not found
                if(selectedCategoryItem == null)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

                //create view model from model
                CategoryItem selectedCategoryViewItem = AutoMapper.Mapper.Map<CategoryItem>(selectedCategoryItem);

                //return wrapper class
                return PartialView("_ReturnCategoryChildren", 
                                            new CategoryDisplayItem() {
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
            catch { 
                //add logging here
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }


        // POST: Password/AddCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(CategoryAdd model)
        {
            if (ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

            //user does not have access to add passwords
            if (!User.CanAddCategories() && !User.IsInRole("Administrator"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            try
            {
                    
                //create model from view model
                Category newCategory = AutoMapper.Mapper.Map<Category>(model);

                //make sure there are no spaces at the end or start of the name
                newCategory.CategoryName = newCategory.CategoryName.Trim();

                //if another category with the same parent and the same name is found
                var foundDuplicate = DatabaseContext.Categories.Any(c => c.Category_ParentID == newCategory.Category_ParentID
                                                                                                        && !c.Deleted && c.CategoryName == newCategory.CategoryName);

                if (foundDuplicate)
                    return Json(new { Status = "Error", Data = "Duplicate Category Name" });

                //get the parent node, and include it's subcategories
                var categoryList = DatabaseContext.Categories
                                                        .Where(c => c.CategoryId == model.Category_ParentID)
                                                        .Include(c => c.SubCategories)
                                                        .SingleOrDefault();
                //parent not not found
                if (categoryList == null)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

                //set the order of the category by getting the number of subcategories
                if (categoryList.SubCategories.Where(c => !c.Deleted).ToList().Count > 0)
                    newCategory.CategoryOrder = (Int16)(categoryList.SubCategories.Where(c => !c.Deleted).Max(c => c.CategoryOrder) + 1);
                else
                    newCategory.CategoryOrder = 1;

                //save the new category
                DatabaseContext.Categories.Add(newCategory);
                await DatabaseContext.SaveChangesAsync();

                //map new category to display view model
                CategoryItem returnCategoryViewItem = AutoMapper.Mapper.Map<CategoryItem>(newCategory);

                //notify clients that a new category has been added
                PushNotifications.newCategoryAdded(newCategory.CategoryId.Value);

                return Json(new { Status = "OK", Data = RenderViewContent.RenderViewToString("Password", "_CategoryItem", returnCategoryViewItem) });

            }
            catch {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Password/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategory(CategoryEdit model)
        {
            if (ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

            //user does not have access to edit category
            if (!User.CanEditCategories() && !User.IsInRole("Administrator"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
       
            try
            {
  
                //map the view model to a category mode
                Category editedCategory = AutoMapper.Mapper.Map<Category>(model);

                //remove any spaces from the end of the name
                editedCategory.CategoryName = editedCategory.CategoryName.Trim();

                int intParentId = DatabaseContext.Categories.AsNoTracking().SingleOrDefault(c => c.CategoryId == model.CategoryId).Category_ParentID ?? -1;

                //if another category with the same parent and the same name is found
                var foundDuplicate = DatabaseContext.Categories.AsNoTracking().Any(c => c.Category_ParentID == intParentId
                                                                                                                        && !c.Deleted && c.CategoryId != model.CategoryId
                                                                                                                        && c.CategoryName == model.CategoryName);

                if (foundDuplicate)
                    return Json(new { 
                                        Status = "Error", 
                                        Data = new { 
                                                        ErrorMessage = "Duplicate Category Name", 
                                                        CategoryId = editedCategory.CategoryId 
                                                    } 
                                });

                //update the category
                DatabaseContext.Entry(editedCategory).State = EntityState.Modified;

                //dont update the categoryorder value
                DatabaseContext.Entry(editedCategory).Property("CategoryOrder").IsModified = false;
                DatabaseContext.Entry(editedCategory).Property("Category_ParentID").IsModified = false;

                //save changes
                await DatabaseContext.SaveChangesAsync();

                //notify clients that a category has been updated
                PushNotifications.sendUpdatedCategoryDetails(model);

                //return the object, so that the UI can be updated
                return Json(new { Status = "OK", Data = editedCategory });

            }
            catch {
                //add logging here
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }


        // POST: Password/DeleteCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCategory(int CategoryId)
        {
            //user does not have access to delete categories
            if (!User.CanDeleteCategories() && !User.IsInRole("Administrator"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            try
            {

                //get the category item to delete
                var selectedCategory = DatabaseContext.Categories
                                                        .Where(c => c.CategoryId == CategoryId)
                                                        .Include("Parent_Category")
                                                        .SingleOrDefault();

                //category not found
                if(selectedCategory == null)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

                //load in the parent's subcategories
                DatabaseContext.Entry(selectedCategory.Parent_Category)
                                        .Collection(c => c.SubCategories)
                                        .Query()
                                        .Where(c => !c.Deleted)
                                        .Load();

                //loop through and adjust category order
                foreach (Category siblingCategory in selectedCategory.Parent_Category
                                                                                .SubCategories
                                                                                .Where(c => !c.Deleted && c.CategoryOrder > selectedCategory.CategoryOrder))
                {
                    siblingCategory.CategoryOrder--;
                    DatabaseContext.Entry(siblingCategory).State = EntityState.Modified;
                }

                //set the item to deleted
                selectedCategory.Deleted = true;

                //move it to the very end
                selectedCategory.CategoryOrder = 9999;

                //set the item to be deleted
                DatabaseContext.Entry(selectedCategory).State = EntityState.Modified;

                //save changes
                await DatabaseContext.SaveChangesAsync();

                //create the view model to return relevant details
                CategoryDelete deletedCategory = AutoMapper.Mapper.Map<CategoryDelete>(selectedCategory);

                //notify clients that a category has been deleted
                PushNotifications.sendDeletedCategoryDetails(deletedCategory);

                //return the item, so that it can be removed from the UI
                return Json(deletedCategory);
            }
            catch {
                //add logging here   
            }

            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }

        #endregion

        
        #region PasswordActions


        // GET:  ViewPassword/23
        public async Task<ActionResult> ViewPassword(int PasswordId)
        {

            PasswordDetails passwordDisplayDetails;
            int UserId = User.Identity.GetUserId().ToInt();
            bool userIsAdmin = User.IsInRole("Administrator");
            var user = await UserMgr.FindByIdAsync(UserId);

            //get a list of userIds that have UserPassword records for this password
            var UserIDList = DatabaseContext.UserPasswords.Where(up => up.PasswordId == PasswordId).Select(up => up.Id).ToList();

            //Retrive the password -if the user has access
            Password selectedPassword = DatabaseContext.Passwords
                                                            .Where(pass => !pass.Deleted
                                                            && (
                                                                (UserIDList.Contains(UserId))
                                                             || (userIsAdmin && ApplicationSettings.Default.AdminsHaveAccessToAllPasswords)
                                                             || pass.Creator_Id == UserId) && pass.PasswordId == PasswordId
                                                                )
                                                                .Include(p => p.Parent_UserPasswords.Select(up => up.UserPasswordUser))
                                                                .SingleOrDefault();


            //user does not have access to view password
            if(selectedPassword == null)
            {
                passwordDisplayDetails = new PasswordDetails
                {
                    UserPermissions = new System.Collections.ObjectModel.Collection<PasswordUserPermission>(),
                    ViewPassword = new PasswordDisplay(),
                    EditPassword = new PasswordEdit(),
                    OpenTab = DefaultTab.ViewPassword
                };

                ModelState.AddModelError("", "You do not have permission to view this password");

                return View(passwordDisplayDetails);
            }
           
            //obtain a list of users that dont have a record in the UserPassword table
            var UserList = DatabaseContext.Users.Where(u => !UserIDList.Contains(u.Id)).ToList();

            //add a new UserPassword record in to the list, so they every user is displayed on the "permissions" page.
            foreach(var userItem in UserList)
            {
                selectedPassword.Parent_UserPasswords.Add(new UserPassword()
                {
                    Id = userItem.Id,
                    PasswordId = selectedPassword.PasswordId,
                    CanChangePermissions = false,
                    CanViewPassword = false,
                    CanEditPassword = false,
                    CanDeletePassword = false,
                    UserPasswordUser = userItem,
                    UsersPassword = selectedPassword
                });
            }

            //wipe this out, as we dont want to decrypted and on show - plus we want to know if we should update the password when EditPassword is called
            selectedPassword.EncryptedPassword = "";

            #region Decrypt_Password_Fields

                string DecryptedUsername = selectedPassword.EncryptedUserName;
                string DecryptedSecondCredential = selectedPassword.EncryptedSecondCredential;

                EncryptionAndHashing.DecryptUsernameFields(user, ref DecryptedUsername, ref DecryptedSecondCredential);

                selectedPassword.EncryptedUserName = DecryptedUsername;
                selectedPassword.EncryptedSecondCredential = DecryptedSecondCredential;

            #endregion

            passwordDisplayDetails = new PasswordDetails
            {
                UserPermissions = AutoMapper.Mapper.Map<IList<PasswordUserPermission>>(selectedPassword.Parent_UserPasswords.OrderBy(up => up.UserPasswordUser.userFullName).ToList()),
                ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword),
                EditPassword = AutoMapper.Mapper.Map<PasswordEdit>(selectedPassword)
            };

            passwordDisplayDetails.OpenTab = DefaultTab.ViewPassword;
            return View(passwordDisplayDetails);
        }

        
        // GET: Password/AddPassword/24
        public ActionResult AddPassword(int ParentCategoryId)
        {
            //return the view - this will open in Magnificent window
            return View("AddPassword", new PasswordAdd { Parent_CategoryId = ParentCategoryId });
        }


        // POST: Password/AddPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPassword(PasswordAdd model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //check if user can add passwords
            if (!User.CanAddPasswords() && !User.IsInRole("Administrator"))
            {
                ModelState.AddModelError("", "You do not have permission to create passwords");
                return View(model);
            }

            try
            {

                Password newPasswordItem = AutoMapper.Mapper.Map<Password>(model);

                var userId = int.Parse(User.Identity.GetUserId());
                var user = await UserMgr.FindByIdAsync(userId);

                //get the parent category node, and include it's passwords
                var passwordList = DatabaseContext.Categories
                                                        .Where(c => c.CategoryId == model.Parent_CategoryId)
                                                        .Include("Passwords")
                                                        .SingleOrDefault();

                //could not find item
                if (passwordList == null)
                {
                    ModelState.AddModelError("", "Could not find parent category");
                    return View(model);
                }

                //set the order of the category by getting the number of subcategories
                if (passwordList.Passwords.Where(p => !p.Deleted).ToList().Count > 0)
                    newPasswordItem.PasswordOrder = (Int16)(passwordList.Passwords.Where(p => !p.Deleted).Max(p => p.PasswordOrder) + 1);
                else
                    newPasswordItem.PasswordOrder = 1;

                newPasswordItem.Parent_CategoryId = model.Parent_CategoryId;
                newPasswordItem.CreatedDate = DateTime.Now;
                newPasswordItem.Creator_Id = user.Id;
                newPasswordItem.Location = model.Location;

                #region Encrypt_Password_Fields

                    string EncryptedUsername = newPasswordItem.EncryptedUserName;
                    string EncryptedSecondCredential = newPasswordItem.EncryptedSecondCredential;
                    string EncryptedPassword = newPasswordItem.EncryptedPassword;

                    EncryptionAndHashing.EncryptUsernameAndPasswordFields(user, ref EncryptedUsername, ref EncryptedSecondCredential, ref EncryptedPassword);

                    newPasswordItem.EncryptedUserName = EncryptedUsername;
                    newPasswordItem.EncryptedSecondCredential = EncryptedSecondCredential;
                    newPasswordItem.EncryptedPassword = EncryptedPassword;

                #endregion

                //add the new password item
                DatabaseContext.Passwords.Add(newPasswordItem);

                //also create the UserPassword record
                UserPassword newUserPassword = new UserPassword()
                {
                    CanDeletePassword = true,
                    CanEditPassword = true,
                    CanViewPassword = true,
                    CanChangePermissions = true,
                    PasswordId = newPasswordItem.PasswordId,
                    Id = user.Id
                };

                //add the new userpassword item
                DatabaseContext.UserPasswords.Add(newUserPassword);

                //save both items to database
                await DatabaseContext.SaveChangesAsync();

                //notify clients that a new password has been added
                PushNotifications.newPasswordAdded(newPasswordItem.PasswordId);

                return RedirectToAction("ViewPassword", new System.Web.Routing.RouteValueDictionary { { "PasswordId", newPasswordItem.PasswordId } });

            }
            catch (Exception Ex)
            {
                ModelState.AddModelError("", "An error occured: " + Ex.Message);
            }

            return View(model);
        }


        // POST: Password/EditPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPassword(PasswordDetails model)
        {

            int UserId = User.Identity.GetUserId().ToInt();
            var user = await UserMgr.FindByIdAsync(UserId);

            //get the UserPassword records for the selected password - so we dont have multiple hits on the DB later
            var UserPasswordList = DatabaseContext.UserPasswords.Where(up => up.PasswordId == model.EditPassword.PasswordId).ToList();
            var UserIDList = UserPasswordList.Select(up => up.Id).ToList();

            //Retrive the password - if the user has access to view the password
            Password selectedPassword = DatabaseContext.Passwords
                                                            .Where(pass => !pass.Deleted
                                                                    && (
                                                                        (UserIDList.Contains(UserId))
                                                                     || pass.Creator_Id == UserId
                                                                        ) 
                                                                    && pass.PasswordId == model.EditPassword.PasswordId
                                                                   )
                                                                   .Include(p => p.Parent_UserPasswords.Select(up => up.UserPasswordUser))
                                                                   .Include(p => p.Creator)
                                                                   .SingleOrDefault();

            //user does not have access to view the password
            if (selectedPassword == null)
            {
                //return empty model
                model.ViewPassword = new PasswordDisplay();
                model.EditPassword = new PasswordEdit();
                model.UserPermissions = null;
                model.OpenTab = DefaultTab.ViewPassword;

                ModelState.AddModelError("", "You do not have permission to view this password");

                return View("ViewPassword", model);
            }

            //if user can change permission, then load up the additional users
            if (selectedPassword.Parent_UserPasswords.Any(up => up.Id == UserId && up.CanChangePermissions) || selectedPassword.Creator_Id == UserId)
            {
                //obtain a list of users that cont have a record in the UserPassword table
                var UserList = DatabaseContext.Users.Where(u => !UserIDList.Contains(u.Id)).ToList();

                //add a new UserPassword record in to the list, so that every user is displayed on the "permissions" page.
                UserPasswordList.AddRange(UserList.Select(userpass => new UserPassword()
                {
                    Id = userpass.Id,
                    PasswordId = selectedPassword.PasswordId,
                    CanChangePermissions = false,
                    CanViewPassword = false,
                    CanEditPassword = false,
                    CanDeletePassword = false,
                    UserPasswordUser = userpass,
                    UsersPassword = selectedPassword
                }));

            }

            //model state is invalid, to the editpassword model not submitted
            if (!ModelState.IsValid || model.EditPassword == null)
            {
                #region Decrypt_Password_Fields_To_Display

                    string DecryptedUserName = selectedPassword.EncryptedUserName;
                    string DecryptedSecondCredential = selectedPassword.EncryptedSecondCredential;

                    EncryptionAndHashing.DecryptUsernameFields(user,
                                                                            ref DecryptedUserName,
                                                                            ref DecryptedSecondCredential);

                    selectedPassword.EncryptedUserName = DecryptedUserName;
                    selectedPassword.EncryptedSecondCredential = DecryptedSecondCredential;
                    selectedPassword.EncryptedPassword = "";

                #endregion

                //supply the missing data, so the model can be returned
                model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
                model.UserPermissions = AutoMapper.Mapper.Map<IList<PasswordUserPermission>>(UserPasswordList.OrderBy(up => up.UserPasswordUser.userFullName));
                model.OpenTab = DefaultTab.EditPassword;

                return View("ViewPassword", model);
            }

            //user does not have access to edit the password
            if (!selectedPassword.Parent_UserPasswords.Any(up => up.Id == UserId && up.CanEditPassword) && selectedPassword.Creator_Id != UserId)
            {
                #region Decrypt_Password_Fields_To_Display

                    string DecryptedUserName = selectedPassword.EncryptedUserName;
                    string DecryptedSecondCredential = selectedPassword.EncryptedSecondCredential;

                    EncryptionAndHashing.DecryptUsernameFields(user,
                                                                            ref DecryptedUserName,
                                                                            ref DecryptedSecondCredential);

                    selectedPassword.EncryptedUserName = DecryptedUserName;
                    selectedPassword.EncryptedSecondCredential = DecryptedSecondCredential;
                    selectedPassword.EncryptedPassword = "";

                #endregion

                //return just a readonly model
                model.EditPassword = new PasswordEdit();
                model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
                model.UserPermissions = AutoMapper.Mapper.Map<IList<PasswordUserPermission>>(UserPasswordList.OrderBy(up => up.UserPasswordUser.userFullName));
                model.OpenTab = DefaultTab.ViewPassword;

                ModelState.AddModelError("", "You do not have permission to edit this password");

                return View("ViewPassword", model);
            }

            #region Encrypt_Updated_Password_Fields

                string EncryptedUserName = model.EditPassword.EncryptedUserName;
                string EncryptedSecondCredential = model.EditPassword.EncryptedSecondCredential;
                string EncryptedPassword = model.EditPassword.EncryptedPassword;

                EncryptionAndHashing.EncryptUsernameAndPasswordFields(user, 
                                                                        ref EncryptedUserName, 
                                                                        ref EncryptedSecondCredential,
                                                                        ref EncryptedPassword);

            #endregion

            //map changes to existing entry
            DatabaseContext.Entry(selectedPassword).CurrentValues.SetValues(model.EditPassword);
            DatabaseContext.Entry(selectedPassword).Property("EncryptedUserName").CurrentValue = EncryptedUserName;
            DatabaseContext.Entry(selectedPassword).Property("EncryptedSecondCredential").CurrentValue = EncryptedSecondCredential;
            DatabaseContext.Entry(selectedPassword).Property("EncryptedPassword").CurrentValue = EncryptedPassword;

            //if a new password has not been entered
            if (string.IsNullOrEmpty(EncryptedPassword))
                DatabaseContext.Entry(selectedPassword).Property("EncryptedPassword").IsModified = false;

            await DatabaseContext.SaveChangesAsync();

            //do not display on edit form
            model.EditPassword.EncryptedPassword = "";
            selectedPassword.EncryptedPassword = "";

            //copy over new values to view model
            selectedPassword.EncryptedUserName = model.EditPassword.EncryptedUserName;
            selectedPassword.EncryptedSecondCredential = model.EditPassword.EncryptedSecondCredential;

            //supply the missing data, so the model can be returned
            model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
            model.UserPermissions = AutoMapper.Mapper.Map<IList<PasswordUserPermission>>(UserPasswordList.OrderBy(up => up.UserPasswordUser.userFullName));
            model.OpenTab = DefaultTab.EditPassword;

            PushNotifications.sendUpdatedPasswordDetails(model.EditPassword);

            return View("ViewPassword", model);
        }


        // POST: Password/DeletePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePassword(int PasswordId)
        {
            int UserId = User.Identity.GetUserId().ToInt();

            //get the UserPassword records for the selected password - so we dont have multiple hits on the DB later
            var UserPasswordList = DatabaseContext.UserPasswords.Where(up => up.PasswordId == PasswordId).ToList();

            //Retrive the password - if the user has access to delete the password
            Password selectedPassword = DatabaseContext.Passwords
                                                            .Include(pass => pass.Creator)
                                                            .Where(pass => !pass.Deleted && pass.PasswordId == PasswordId)
                                                            .Include(p => p.Parent_UserPasswords.Select(up => up.UserPasswordUser))
                                                            .Where(pass => pass.Creator_Id == UserId || pass.Parent_UserPasswords.Any(up => up.CanDeletePassword && up.Id == UserId))
                                                            .SingleOrDefault(p => p.PasswordId == PasswordId);

            //user does not have access to delete the password
            if (selectedPassword==null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden); 

            //set the password as deleted and save to database
            selectedPassword.Deleted = true;
            DatabaseContext.Entry(selectedPassword).State = EntityState.Modified;
            await DatabaseContext.SaveChangesAsync();

            PasswordDelete deletedPassword = AutoMapper.Mapper.Map<PasswordDelete>(selectedPassword);

            PushNotifications.sendDeletedPasswordDetails(deletedPassword);

            //return item so it can be removed from the UI
            return Json(deletedPassword);
        }


        //POST: /Password/EditUserPermissions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserPermissions(PasswordDetails model)
        {

            if (!ModelState.IsValid || model.UserPermissions == null || model.UserPermissions.Count==0)
            {
                //supply the missing data, so the model can be returned
                //return empty model
                model.ViewPassword = new PasswordDisplay();
                model.EditPassword = new PasswordEdit();

                return View("ViewPassword", model);
            }

            int UserId = User.Identity.GetUserId().ToInt();
            var user = await UserMgr.FindByIdAsync(UserId);

            //important we store the password id - this is so that an attacker cannot change the passwordId field
            //in one of the userpermission items to a password Id he doesnt have access to.
            //e.g. if   model.UserPermissions[0].PasswordId  is "1", which the attacker has access to
            //he can quite easily change model.UserPermissions[1].PasswordId to "2" which he doesnt have access to
            //so this variable will be used for validation later.
            int PasswordId = model.UserPermissions[0].PasswordId;

            //get a list of userIds that have UserPassword records for this password
            var UserIDList = DatabaseContext.UserPasswords.Where(up => up.PasswordId == PasswordId).Select(up => up.Id).ToList();

            //Retrive the password - if the user has access to view the password
            Password selectedPassword = DatabaseContext.Passwords.AsNoTracking()
                                                                        .Where(pass => !pass.Deleted
                                                                                && (
                                                                                    (UserIDList.Contains(UserId))
                                                                                    || pass.Creator_Id == UserId
                                                                                    )
                                                                                 && pass.PasswordId == PasswordId
                                                                                )
                                                                                .Include(p => p.Creator)
                                                                                .Include(p => p.Parent_UserPasswords.Select(up => up.UserPasswordUser))
                                                                                .SingleOrDefault();

            //user does not have access to view
            if (selectedPassword == null)
            {
                //return empty model
                model.ViewPassword = new PasswordDisplay();
                model.EditPassword = new PasswordEdit();
                model.UserPermissions = new List<PasswordUserPermission>();
                model.OpenTab = DefaultTab.ViewPassword;

                ModelState.AddModelError("", "You do not have permission to view this password");

                return View("ViewPassword", model);
            }

            #region Decrypt_Password_Fields

                string DecryptedUserName = selectedPassword.EncryptedUserName;
                string DecryptedSecondCredential = selectedPassword.EncryptedSecondCredential;
                string DecryptedPassword = selectedPassword.EncryptedPassword;

                EncryptionAndHashing.DecryptUsernameFields(user, ref DecryptedUserName, ref DecryptedSecondCredential);

                selectedPassword.EncryptedUserName = DecryptedUserName;
                selectedPassword.EncryptedSecondCredential = DecryptedSecondCredential;

            #endregion

            //user does not have access to edit user permissions
            if (!selectedPassword.Parent_UserPasswords.Any(up => up.Id == UserId && up.CanChangePermissions) && selectedPassword.Creator_Id != UserId)
            {
                            
                //get all of the UserPassword records for the selected password
                var CurrentUserUserPassword = DatabaseContext.UserPasswords.AsNoTracking()
                                                                        .Where(up => up.PasswordId == PasswordId && up.Id == user.Id)
                                                                        .Include(up => up.UserPasswordUser)
                                                                        .Select(up => new PasswordUserPermission()
                                                                        {
                                                                            Id = up.Id,
                                                                            PasswordId = up.PasswordId,
                                                                            UserPasswordUser = up.UserPasswordUser,
                                                                            CanChangePermissions = up.CanChangePermissions,
                                                                            CanDeletePassword = up.CanDeletePassword,
                                                                            CanEditPassword = up.CanEditPassword,
                                                                            CanViewPassword = up.CanViewPassword
                                                                        })
                                                                        .ToList();

                //prepare the model to return back to the view
                model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
                model.OpenTab = DefaultTab.ViewPassword;

                //make sure the user can edit the password before returning an edit password form
                if (CurrentUserUserPassword.Any(up => up.CanEditPassword) || selectedPassword.Creator_Id == UserId)
                    model.EditPassword = AutoMapper.Mapper.Map<PasswordEdit>(selectedPassword);
                else
                    model.EditPassword = new PasswordEdit();

                model.UserPermissions = null;

                ModelState.AddModelError("", "You do not have permission to edit permissions for this password");

                return View("ViewPassword", model);

            }

            //get all of the users
            var UserList = DatabaseContext.Users.AsNoTracking().ToList();

            //get all of the UserPassword records for the selected password
            var UserPasswordList = DatabaseContext.UserPasswords.AsNoTracking()
                                                                    .Where(up => up.PasswordId == PasswordId)
                                                                    .Include(up => up.UserPasswordUser)
                                                                    .ToList();


            //load in the UserPassword records into the related User records (normally EF does this view Lazy Loading, but we can't filter on Include())
            UserList = UserList.Select(u => new ApplicationUser() {
                                                                        Id = u.Id,
                                                                        UserName = u.UserName,
                                                                        userFullName = u.userFullName,
                                                                        UserPasswords = UserPasswordList.Where(up => up.Id == u.Id).ToList()
                                                                  }).ToList();


            //loop through each permission item submitted
            for (int intPermissionIndex = 0; intPermissionIndex < model.UserPermissions.Count; intPermissionIndex++)
            {
                //pssword id does not match the one submitted - basically the user has been naughty and modifed the form (tut! tut!)
                if (model.UserPermissions[intPermissionIndex].PasswordId != PasswordId)
                {
                    ModelState.AddModelError("", "An illegal change has been made to the permission form");
                    break;
                }
                else
                {

                    //convert the current permission item into a UserPassword entity
                    UserPassword userPermissionItem = AutoMapper.Mapper.Map<UserPassword>(model.UserPermissions[intPermissionIndex]);

                    //has one of the 4 permissions been selected
                    bool PermissionsSelected = (userPermissionItem.CanViewPassword 
                                                                                    || userPermissionItem.CanEditPassword 
                                                                                    || userPermissionItem.CanChangePermissions 
                                                                                    || userPermissionItem.CanDeletePassword);

                    ApplicationUser CurrentPermissionRecordUser = UserList.Where(u => u.Id == userPermissionItem.Id).SingleOrDefault();

                    #region Decide_UserPassword_Record_Action

                    //the user doesnt have a user password record for this password, so we need to add one
                    if (!CurrentPermissionRecordUser.UserPasswords.Any(up => up.PasswordId == userPermissionItem.PasswordId) && PermissionsSelected)
                    {
                        //get the user from the userid in the permission item submitted
                        var MissingUser = DatabaseContext.Users.Where(u => u.Id == userPermissionItem.Id).SingleOrDefault();
                        userPermissionItem.CanViewPassword = true;                                                          //default
                        model.UserPermissions[intPermissionIndex].CanViewPassword = true;                                   //for the UI
                        model.UserPermissions[intPermissionIndex].UserPasswordUser = CurrentPermissionRecordUser;           //for the UI
                        DatabaseContext.Entry(userPermissionItem).State = EntityState.Added;                                //add new record
                    }
                    //the user DOES have a record, so we need to update
                    else if (CurrentPermissionRecordUser.UserPasswords.Any(up => up.PasswordId == userPermissionItem.PasswordId) && PermissionsSelected)
                    {
                        userPermissionItem.CanViewPassword = true;                                                          //default
                        model.UserPermissions[intPermissionIndex].CanViewPassword = true;                                   //for the UI
                        model.UserPermissions[intPermissionIndex].UserPasswordUser = CurrentPermissionRecordUser;           //for the UI
                        DatabaseContext.Entry(userPermissionItem).State = EntityState.Modified;                             //update record
                    }
                    //user DOES have a record, and no permissions have been selected (or View permission has been unticked)
                    //if the view permission is unticked, then all other permission (del/edit etc..) are revoked
                    else if (CurrentPermissionRecordUser.UserPasswords.Any(up => up.PasswordId == userPermissionItem.PasswordId) && 
                                                                                                        (!PermissionsSelected || !userPermissionItem.CanViewPassword))
                    {
                        model.UserPermissions[intPermissionIndex].CanEditPassword = false;
                        model.UserPermissions[intPermissionIndex].CanDeletePassword = false;
                        model.UserPermissions[intPermissionIndex].CanChangePermissions = false;
                        model.UserPermissions[intPermissionIndex].UserPasswordUser = CurrentPermissionRecordUser;           //for the UI
                        DatabaseContext.Entry(userPermissionItem).State = EntityState.Deleted;                              //delete record
                    }
                    //user doesnt have a record, and nothing has been selected
                    else
                    {
                        model.UserPermissions[intPermissionIndex].CanEditPassword = false;
                        model.UserPermissions[intPermissionIndex].CanDeletePassword = false;
                        model.UserPermissions[intPermissionIndex].CanChangePermissions = false;
                        model.UserPermissions[intPermissionIndex].UserPasswordUser = CurrentPermissionRecordUser;           //for the UI
                    }

                    #endregion

                }
            }

            //save changes to DB
            await DatabaseContext.SaveChangesAsync();

            //grab the current user's UserPassword record again - incase they changed their own permissions
            UserPasswordList = DatabaseContext.UserPasswords.AsNoTracking()
                                                                    .Where(up => up.PasswordId == PasswordId && up.Id == UserId)
                                                                    .ToList();

            //supply the missing data, so the model can be returned
            model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
                        
            //make sure the user can edit the password before returning an edit password form
            //the use the model to see if the user has added the edit permission to their account (they can do this as they have "edit permission")
            if (model.UserPermissions.Any(up => up.Id == UserId && up.CanEditPassword) || UserPasswordList.Any(up => up.CanEditPassword) || selectedPassword.Creator_Id == UserId)
                model.EditPassword = AutoMapper.Mapper.Map<PasswordEdit>(selectedPassword);
            else
                model.EditPassword = new PasswordEdit();
                        
            //check if the user disabled change permission for themself (also check thier existing records)
            if (model.UserPermissions.Any(up => up.Id == UserId && up.CanChangePermissions) || UserPasswordList.Any(up => up.CanChangePermissions) || selectedPassword.Creator_Id == UserId)
                model.OpenTab = DefaultTab.EditPermissions;
            else
            {
                model.UserPermissions = new List<PasswordUserPermission>();
                model.OpenTab = DefaultTab.ViewPassword;
            }

            //It's easier to just tell the client to pull down a whole new copy of the password
            PushNotifications.newPasswordAdded(model.EditPassword.PasswordId);

            return View("ViewPassword", model);
        }


        //POST: /Password/GetEncryptedPassword/24
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetEncryptedPassword(int PasswordId)
        {
            int UserId = User.Identity.GetUserId().ToInt();
            bool userIsAdmin = User.IsInRole("Administrator");
            var user = await UserMgr.FindByIdAsync(UserId);

            //Retrive the password -if the user has access
            Password selectedPassword = DatabaseContext.Passwords.Include("Parent_UserPasswords").Where(pass => !pass.Deleted
                                                            && (DatabaseContext.UserPasswords
                                                                        .Any(up => up.PasswordId == pass.PasswordId && up.Id == UserId))
                                                            || (
                                                                    userIsAdmin && ApplicationSettings.Default.AdminsHaveAccessToAllPasswords
                                                               )).SingleOrDefault(p => p.PasswordId == PasswordId);


            //password does not exist... or user does not have access
            if (selectedPassword == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            #region Decrypt_Password

                string DecryptedPassword = selectedPassword.EncryptedPassword;
                EncryptionAndHashing.DecryptPasswordField(user, ref DecryptedPassword);
                selectedPassword.EncryptedPassword = DecryptedPassword;

            #endregion

            PasswordPassword passwordText = new PasswordPassword {PlainTextPassword = selectedPassword.EncryptedPassword };
            return Json(passwordText);
        }

        #endregion

        #region CategoryAndPasswordActions

        //POST: /Password/UpdatePosition/123?NewPosition=2&OldPosition=4&isCategory=true
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePosition(Int32 ItemId, Int16 NewPosition, Int16 OldPosition, bool isCategoryItem)
        {
            //check if user can edit categories
            if (!User.CanEditCategories() && !User.IsInRole(ApplicationSettings.Default.RoleAllowEditCategories) && !User.IsInRole("Administrator"))
                return Json(new
                {
                    Status = "Failed"
                });

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
                        foreach (Category childCategory in currentCategory.Parent_Category
                                                                                .SubCategories
                                                                                .Where(c => c.CategoryOrder > OldPosition && c.CategoryOrder < NewPosition + 1))
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

                        if(ApplicationSettings.Default.BroadcastCategoryPositionChange)
                            //notify clients that a category or password has changed position
                            PushNotifications.sendUpdatedItemPosition(ItemId.ToString(), NewPosition, OldPosition);

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
                        foreach (Password childPassword in currentPassword.Parent_Category
                                                                                    .Passwords
                                                                                    .Where(p => p.PasswordOrder > OldPosition && p.PasswordOrder < NewPosition + 1))
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

                        if(ApplicationSettings.Default.BroadcastPasswordPositionChange)
                            //notify clients that a category or password has changed position
                            PushNotifications.sendUpdatedItemPosition("Password-" + ItemId.ToString(), NewPosition, OldPosition);

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
                        foreach (Category childCategory in currentCategory.Parent_Category
                                                                                .SubCategories
                                                                                .Where(c => c.CategoryOrder < OldPosition && c.CategoryOrder > NewPosition - 1))
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

                        if(ApplicationSettings.Default.BroadcastCategoryPositionChange)
                            //notify clients that a category or password has changed position
                            PushNotifications.sendUpdatedItemPosition(ItemId.ToString(), NewPosition, OldPosition);

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
                        foreach (Password childPassword in currentPassword.Parent_Category
                                                                                .Passwords
                                                                                .Where(p => p.PasswordOrder < OldPosition && p.PasswordOrder > NewPosition - 1))
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

                        if(ApplicationSettings.Default.BroadcastPasswordPositionChange)
                            //notify clients that a category or password has changed position
                            PushNotifications.sendUpdatedItemPosition("Password-" + ItemId.ToString(), NewPosition, OldPosition);

                        #endregion

                    }

                }

                DatabaseContext.Configuration.AutoDetectChangesEnabled = true;

                return Json(new
                {
                    Status = "Completed"
                });

            }
            catch { 
                //add logging here
            }
            
            return Json(new
           {
                 Status = "Completed"
           });
        }

        #endregion

        #region helpers

        private void AddErrors(IdentityResult result)
        {
            ModelState.AddModelError("", string.Join(" and ", result.Errors));
        }

        #endregion

    }
}
