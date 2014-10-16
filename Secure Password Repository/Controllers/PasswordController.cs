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
using Secure_Password_Repository.Controllers;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Runtime.Caching;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(PasswordController), "AutoMapperStart")]
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
                var UserPasswordList = DatabaseContext.UserPasswords.Where(up => up.Id == UserId && DatabaseContext.Passwords.Where(p => p.Parent_CategoryId == ParentCategoryId).Select(p => p.PasswordId).Contains(up.PasswordId)).ToList();

                //return the selected item - with its children
                var selectedCategoryItem = DatabaseContext.Categories
                                                            .Where(c => c.CategoryId == ParentCategoryId)
                                                            .Include("SubCategories")
                                                            .Include("Passwords")
                                                            .ToList()
                                                            .Select(c => new Category()
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
                                                                                    && (
                                                                                        (UserPasswordList.Any(up => up.PasswordId == pass.PasswordId && up.Id == UserId))
                                                                                        || (userIsAdmin && ApplicationSettings.Default.AdminsHaveAccessToAllPasswords)
                                                                                        || pass.Creator_Id == UserId
                                                                                        )
                                                                                        )   //make sure only undeleted passwords - that the current user has acccess to - are returned
                                                                .OrderBy(pass => pass.PasswordOrder)
                                                                .ToList()
                                                            })
                                                            .Single(c => c.CategoryId == ParentCategoryId);

                //create view model from model
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


        // POST: Password/AddCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(CategoryAdd model)
        {
            //check user can add categories
            if (User.CanAddCategories() || User.IsInRole("Administrator"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //create model from view model
                        Category newCategory = AutoMapper.Mapper.Map<Category>(model);

                        //get the parent node, and include it's subcategories
                        var categoryList = DatabaseContext.Categories
                                                                .Where(c => c.CategoryId == model.Category_ParentID)
                                                                .Include("SubCategories")
                                                                .Single(c => c.CategoryId == model.Category_ParentID);

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

                        return PartialView("_CategoryItem", returnCategoryViewItem);

                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    }

                }
                catch {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            //user does not have access to add passwords
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        // POST: Password/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategory(CategoryEdit model)
        {
            //check user has access to edit categories
            if (User.CanEditCategories() || User.IsInRole("Administrator"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Category editedCategory = AutoMapper.Mapper.Map<Category>(model);

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
                        return Json(model);
                    }
                }
                catch {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            //user does not have access to edit category
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }


        // POST: Password/DeleteCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCategory(int CategoryId)
        {
            //check the user is allowed to delete categories
            if (User.CanDeleteCategories() || User.IsInRole("Administrator"))
            {
                try
                {

                    //get the category item to delete
                    var selectedCategory = DatabaseContext.Categories
                                                            .Where(c => c.CategoryId == CategoryId)
                                                            .Include("Parent_Category")
                                                            .Single(c => c.CategoryId == CategoryId);

                    //load in the parent's subcategories
                    DatabaseContext.Entry(selectedCategory.Parent_Category)
                                            .Collection(c => c.SubCategories)
                                            .Query()
                                            .Where(c => !c.Deleted)
                                            .Load();

                    //loop through and adjust category order
                    foreach (Category siblingCategory in selectedCategory.Parent_Category.SubCategories
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
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            //user does not have access to delete categories
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
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
                                                             || pass.Creator_Id == UserId)
                                                                )
                                                                .Include(p => p.Parent_UserPasswords.Select(up => up.UserPasswordUser))
                                                                .SingleOrDefault(p => p.PasswordId == PasswordId);

           
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

            ///////////// decryption process //////////////

            //grab the 3 encryption keys that are required to do encryption
            byte[] bytePrivateKey = user.userPrivateKey.FromBase64().ToBytes();
            byte[] byteEncryptionKey = user.userEncryptionKey.FromBase64().ToBytes();
            byte[] bytePrivateKeyKey = MemoryCache.Default.Get(user.UserName).ToString().ToBytes();

            //decrypt the key that is used to decrypt the user's private key
            EncryptionAndHashing.Decrypt_DPAPI(ref bytePrivateKeyKey);

            //decrypt the user private key
            bytePrivateKey = EncryptionAndHashing.Decrypt_AES256_ToBytes(bytePrivateKey, bytePrivateKeyKey).FromBase64();
            //decrypt again
            EncryptionAndHashing.Decrypt_DPAPI(ref bytePrivateKey);

            //we dont need this anymote
            Array.Clear(bytePrivateKeyKey, 0, bytePrivateKeyKey.Length);

            //decrypt the user's copy of the password encryption key
            byteEncryptionKey = EncryptionAndHashing.Decrypt_RSA_ToBytes(byteEncryptionKey, bytePrivateKey).FromBase64();
            //decyrpt again
            EncryptionAndHashing.Decrypt_DPAPI(ref byteEncryptionKey);

            //we dont need this anymore
            Array.Clear(bytePrivateKey, 0, bytePrivateKey.Length);

            //convert the key to base64
            byteEncryptionKey = byteEncryptionKey.ToBase64();

            //get the encrypted details and un-base64 them
            byte[] byteEncryptedSecondCredential = selectedPassword.EncryptedSecondCredential.FromBase64().ToBytes();
            byte[] byteEncryptedUserName = selectedPassword.EncryptedUserName.FromBase64().ToBytes();

            //decryption first pass
            EncryptionAndHashing.Decrypt_DPAPI(ref byteEncryptedSecondCredential);
            EncryptionAndHashing.Decrypt_DPAPI(ref byteEncryptedUserName);

            //decryption second pass
            byteEncryptedSecondCredential = EncryptionAndHashing.Decrypt_AES256_ToBytes(byteEncryptedSecondCredential.RemoveNullBytes().FromBase64(), byteEncryptionKey);
            byteEncryptedUserName = EncryptionAndHashing.Decrypt_AES256_ToBytes(byteEncryptedUserName.RemoveNullBytes().FromBase64(), byteEncryptionKey);

            //we dont need this anymore
            Array.Clear(byteEncryptionKey, 0, byteEncryptionKey.Length);

            //convert encrypted data to base64 and store in the model
            selectedPassword.EncryptedSecondCredential = byteEncryptedSecondCredential.ConvertToString();
            selectedPassword.EncryptedUserName = byteEncryptedUserName.ConvertToString();

            //we dont need these arrays anymore
            Array.Clear(byteEncryptedSecondCredential, 0, byteEncryptedSecondCredential.Length);
            Array.Clear(byteEncryptedUserName, 0, byteEncryptedUserName.Length);

            //////////////////////////////////////////////

            //user has access
            if (selectedPassword != null)
            {
                passwordDisplayDetails = new PasswordDetails
                {
                    UserPermissions = AutoMapper.Mapper.Map<IList<PasswordUserPermission>>(selectedPassword.Parent_UserPasswords.OrderBy(up => up.UserPasswordUser.userFullName).ToList()),
                    ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword),
                    EditPassword = AutoMapper.Mapper.Map<PasswordEdit>(selectedPassword)
                };
            }
            //user does not have access
            else
            {
                passwordDisplayDetails = new PasswordDetails
                {
                    UserPermissions = new System.Collections.ObjectModel.Collection<PasswordUserPermission>(),
                    ViewPassword = new PasswordDisplay(),
                    EditPassword = new PasswordEdit(),
                    OpenTab = DefaultTab.ViewPassword
                };

                ModelState.AddModelError("", "You do not have permission to view this password");
            }

            passwordDisplayDetails.OpenTab = DefaultTab.ViewPassword;
            return View(passwordDisplayDetails);
        }

        
        // GET: Password/AddPassword/24
        public ActionResult AddPassword(int ParentCategoryId)
        {
            return View("AddPassword", new PasswordAdd { Parent_CategoryId = ParentCategoryId });
        }


        // POST: Password/AddPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPassword(PasswordAdd model)
        {
            //check if usercan add passwords
            if (User.CanAddPasswords() || User.IsInRole("Administrator"))
            {

                if (ModelState.IsValid)
                {
                    Password newPasswordItem = AutoMapper.Mapper.Map<Password>(model);

                    var userId = int.Parse(User.Identity.GetUserId());
                    var user = await UserMgr.FindByIdAsync(userId);

                    //get the parent category node, and include it's passwords
                    var passwordList = DatabaseContext.Categories
                                                            .Where(c => c.CategoryId == model.Parent_CategoryId)
                                                            .Include("Passwords")
                                                            .Single(c => c.CategoryId == model.Parent_CategoryId);

                    //set the order of the category by getting the number of subcategories
                    if (passwordList.Passwords.Where(p => !p.Deleted).ToList().Count > 0)
                        newPasswordItem.PasswordOrder = (Int16)(passwordList.Passwords.Where(p => !p.Deleted).Max(p => p.PasswordOrder) + 1);
                    else
                        newPasswordItem.PasswordOrder = 1;

                    newPasswordItem.Parent_CategoryId = model.Parent_CategoryId;
                    newPasswordItem.CreatedDate = DateTime.Now;
                    newPasswordItem.Creator_Id = user.Id;
                    newPasswordItem.Location = model.Location.Replace("http://", "");

                    /////// encryption process //////////

                    //grab the 3 encryption keys that are required to do encryption
                    byte[] bytePrivateKey = user.userPrivateKey.FromBase64().ToBytes();
                    byte[] byteEncryptionKey = user.userEncryptionKey.FromBase64().ToBytes();
                    byte[] bytePrivateKeyKey = MemoryCache.Default.Get(user.UserName).ToString().ToBytes();

                    //decrypt the key that is used to decrypt the user's private key
                    EncryptionAndHashing.Decrypt_DPAPI(ref bytePrivateKeyKey);

                    //decrypt the user private key
                    bytePrivateKey = EncryptionAndHashing.Decrypt_AES256_ToBytes(bytePrivateKey, bytePrivateKeyKey).FromBase64();
                    //decrypt again
                    EncryptionAndHashing.Decrypt_DPAPI(ref bytePrivateKey);

                    //we dont need this anymote
                    Array.Clear(bytePrivateKeyKey, 0, bytePrivateKeyKey.Length);

                    //decrypt the user's copy of the password encryption key
                    byteEncryptionKey = EncryptionAndHashing.Decrypt_RSA_ToBytes(byteEncryptionKey, bytePrivateKey).FromBase64();
                    //decyrpt again
                    EncryptionAndHashing.Decrypt_DPAPI(ref byteEncryptionKey);

                    //we dont need this anymore
                    Array.Clear(bytePrivateKey, 0, bytePrivateKey.Length);

                    //convert the key to base64
                    byteEncryptionKey = byteEncryptionKey.ToBase64();
              
                    //encrypt the details of the new password using AES
                    byte[] byteEncryptedPassword = EncryptionAndHashing.Encrypt_AES256_ToBytes(newPasswordItem.EncryptedPassword, byteEncryptionKey).ToBase64();
                    byte[] byteEncryptedSecondCredential = EncryptionAndHashing.Encrypt_AES256_ToBytes(newPasswordItem.EncryptedSecondCredential, byteEncryptionKey).ToBase64();
                    byte[] byteEncryptedUserName = EncryptionAndHashing.Encrypt_AES256_ToBytes(newPasswordItem.EncryptedUserName, byteEncryptionKey).ToBase64();

                    //we dont need this anymore
                    Array.Clear(byteEncryptionKey, 0, byteEncryptionKey.Length);

                    //another layer of encryption
                    EncryptionAndHashing.Encrypt_DPAPI(ref byteEncryptedPassword);
                    EncryptionAndHashing.Encrypt_DPAPI(ref byteEncryptedSecondCredential);
                    EncryptionAndHashing.Encrypt_DPAPI(ref byteEncryptedUserName);

                    //convert encrypted data to base64 and store in the model
                    newPasswordItem.EncryptedPassword = byteEncryptedPassword.ToBase64String();
                    newPasswordItem.EncryptedSecondCredential = byteEncryptedSecondCredential.ToBase64String();
                    newPasswordItem.EncryptedUserName = byteEncryptedUserName.ToBase64String();

                    //we dont need these arrays anymore
                    Array.Clear(byteEncryptedPassword, 0, byteEncryptedPassword.Length);
                    Array.Clear(byteEncryptedSecondCredential, 0, byteEncryptedSecondCredential.Length);
                    Array.Clear(byteEncryptedUserName, 0, byteEncryptedUserName.Length);

                    ////////////////////////////

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
            }
            else
            {
                ModelState.AddModelError("", "You do not have permission to create passwords");
            }

            return View(model);
        }


        // POST: Password/EditPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPassword(PasswordDetails model)
        {

            if (ModelState.IsValid && model.EditPassword != null)
            {
                int UserId = User.Identity.GetUserId().ToInt();

                //get the UserPassword records for the selected password - so we dont have multiple hits on the DB later
                var UserPasswordList = DatabaseContext.UserPasswords.Where(up => up.PasswordId == model.EditPassword.PasswordId).Select(up => up.Id).ToList();

                //Retrive the password - if the user has access to view the password
                Password selectedPassword = DatabaseContext.Passwords
                                            .Where(pass => !pass.Deleted
                                                    && (
                                                        (UserPasswordList.Contains(UserId))
                                                     || pass.Creator_Id == UserId
                                                        )
                                                   )
                                                   .Include(p => p.Parent_UserPasswords.Select(up => up.UserPasswordUser))
                                                   .SingleOrDefault(p => p.PasswordId == model.EditPassword.PasswordId);



                //user has access to aleast view the password
                if (selectedPassword != null)
                {

                    
                    //if user can change permission, then load up the additional users
                    if (selectedPassword.Parent_UserPasswords.Any(up => up.Id == UserId && up.CanEditPassword) || selectedPassword.Creator_Id == UserId)
                    {
                        //obtain a list of users that cont have a record in the UserPassword table
                        var UserList = DatabaseContext.Users.Where(u => !UserPasswordList.Contains(u.Id)).ToList();

                        //add a new UserPassword record in to the list, so they every user is displayed on the "permissions" page.
                        foreach (var userItem in UserList)
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
                    }
                    else
                    {
                        //if user doesnt have access to change permissions, then clear the list
                        selectedPassword.Parent_UserPasswords.Clear();
                    }



                    //does the user have access to edit the password?
                    if(selectedPassword.Parent_UserPasswords.Any(up => up.Id == UserId && up.CanEditPassword))
                    {

                        var user = await UserMgr.FindByIdAsync(UserId);

                        /////// encryption process //////////

                        //grab the 3 encryption keys that are required to do encryption
                        byte[] bytePrivateKey = user.userPrivateKey.FromBase64().ToBytes();
                        byte[] byteEncryptionKey = user.userEncryptionKey.FromBase64().ToBytes();
                        byte[] bytePrivateKeyKey = MemoryCache.Default.Get(user.UserName).ToString().ToBytes();

                        //decrypt the key that is used to decrypt the user's private key
                        EncryptionAndHashing.Decrypt_DPAPI(ref bytePrivateKeyKey);

                        //decrypt the user private key
                        bytePrivateKey = EncryptionAndHashing.Decrypt_AES256_ToBytes(bytePrivateKey, bytePrivateKeyKey).FromBase64();
                        //decrypt again
                        EncryptionAndHashing.Decrypt_DPAPI(ref bytePrivateKey);

                        //we dont need this anymote
                        Array.Clear(bytePrivateKeyKey, 0, bytePrivateKeyKey.Length);

                        //decrypt the user's copy of the password encryption key
                        byteEncryptionKey = EncryptionAndHashing.Decrypt_RSA_ToBytes(byteEncryptionKey, bytePrivateKey).FromBase64();
                        //decyrpt again
                        EncryptionAndHashing.Decrypt_DPAPI(ref byteEncryptionKey);

                        //we dont need this anymore
                        Array.Clear(bytePrivateKey, 0, bytePrivateKey.Length);

                        //convert the key to base64
                        byteEncryptionKey = byteEncryptionKey.ToBase64();

                        //encrypt the details of the new password using AES
                        byte[] byteEncryptedPassword = EncryptionAndHashing.Encrypt_AES256_ToBytes(model.EditPassword.EncryptedPassword, byteEncryptionKey).ToBase64();
                        byte[] byteEncryptedSecondCredential = EncryptionAndHashing.Encrypt_AES256_ToBytes(model.EditPassword.EncryptedSecondCredential, byteEncryptionKey).ToBase64();
                        byte[] byteEncryptedUserName = EncryptionAndHashing.Encrypt_AES256_ToBytes(model.EditPassword.EncryptedUserName, byteEncryptionKey).ToBase64();

                        //we dont need this anymore
                        Array.Clear(byteEncryptionKey, 0, byteEncryptionKey.Length);

                        //another layer of encryption
                        EncryptionAndHashing.Encrypt_DPAPI(ref byteEncryptedPassword);
                        EncryptionAndHashing.Encrypt_DPAPI(ref byteEncryptedSecondCredential);
                        EncryptionAndHashing.Encrypt_DPAPI(ref byteEncryptedUserName);

                        //convert encrypted data to base64 and store in the model
                        model.EditPassword.EncryptedPassword = byteEncryptedPassword.ToBase64String();
                        model.EditPassword.EncryptedSecondCredential = byteEncryptedSecondCredential.ToBase64String();
                        model.EditPassword.EncryptedUserName = byteEncryptedUserName.ToBase64String();

                        //we dont need these arrays anymore
                        Array.Clear(byteEncryptedPassword, 0, byteEncryptedPassword.Length);
                        Array.Clear(byteEncryptedSecondCredential, 0, byteEncryptedSecondCredential.Length);
                        Array.Clear(byteEncryptedUserName, 0, byteEncryptedUserName.Length);

                        ////////////////////////////


                        //save changes to database
                        DatabaseContext.Entry(selectedPassword).CurrentValues.SetValues(model.EditPassword);
                        await DatabaseContext.SaveChangesAsync();

                        //supply the missing data, so the model can be returned
                        model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
                        model.UserPermissions = AutoMapper.Mapper.Map<IList<PasswordUserPermission>>(selectedPassword.Parent_UserPasswords);

                        PushNotifications.sendUpdatedPasswordDetails(model.EditPassword);
                    }
                    //user does not have access to edit
                    else
                    {
                        //return just a readonly model
                        model.EditPassword = new PasswordEdit();
                        model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
                        model.UserPermissions = AutoMapper.Mapper.Map<IList<PasswordUserPermission>>(selectedPassword.Parent_UserPasswords);

                        ModelState.AddModelError("", "You do not have permission to edit this password");
                    }


                }
                //user does not have access to view
                else
                {
                    //return empty model
                    model.ViewPassword = new PasswordDisplay();
                    model.EditPassword = new PasswordEdit();
                    model.UserPermissions = null;
                    
                    ModelState.AddModelError("", "You do not have permission to view this password");
                }
            }
            //model is invalid
            else
            {
                //supply the missing data, so the model can be returned
                model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(model.EditPassword);
            }

            model.OpenTab = DefaultTab.EditPassword;
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


            if (selectedPassword!=null)
            {
                //user has access to delete the password
                //if(selectedPassword.Parent_UserPasswords.Any(up => up.Id == UserId && up.CanDeletePassword))
                //{

                    //set the password as deleted and save to database
                    selectedPassword.Deleted = true;
                    DatabaseContext.Entry(selectedPassword).State = EntityState.Modified;
                    await DatabaseContext.SaveChangesAsync();

                    PasswordDelete deletedPassword = AutoMapper.Mapper.Map<PasswordDelete>(selectedPassword);

                    PushNotifications.sendDeletedPasswordDetails(deletedPassword);

                    //return item so it can be removed from the UI
                    return Json(deletedPassword);

                //}
            }

            //user does not have access to delete the password
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }


        //POST: /Password/EditUserPermissions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserPermissions(PasswordDetails model)
        {

            if (ModelState.IsValid && model.UserPermissions != null && model.UserPermissions.Count>0)
            {

                int UserId = User.Identity.GetUserId().ToInt();

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
                                                    )
                                                    .Include(p => p.Creator)
                                                    .Include(p => p.Parent_UserPasswords.Select(up => up.UserPasswordUser))
                                                    .SingleOrDefault(p => p.PasswordId == PasswordId);



                //user has access to aleast view the password
                if (selectedPassword != null)
                {

                    //does the user have access to edit the password permissions?
                    if (selectedPassword.Parent_UserPasswords.Any(up => up.Id == UserId && up.CanChangePermissions) || selectedPassword.Creator_Id == UserId)
                    {

                        //get all of the users
                        var UserList = DatabaseContext.Users.AsNoTracking().ToList();

                        //get all of the UserPassword records for the selected password
                        var UserPasswordList = DatabaseContext.UserPasswords.AsNoTracking()
                                                                                .Where(up => up.PasswordId == PasswordId)
                                                                                .Include(up => up.UserPasswordUser)
                                                                                .ToList();


                        //load in the UserPassword records into the related User records (normally EF does this view Lazy Loading, but we can't filter on Include())
                        UserList = UserList.Select(u => new ApplicationUser()
                        {
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
                                bool PermissionsSelected = userPermissionItem.CanViewPassword || userPermissionItem.CanEditPassword || userPermissionItem.CanChangePermissions || userPermissionItem.CanDeletePassword;

                                ApplicationUser CurrentPermissionRecordUser = UserList.Single(u => u.Id == userPermissionItem.Id);

                                //the user doesnt have a record, but a permission has been selected
                                if (!CurrentPermissionRecordUser.UserPasswords.Any(up => up.PasswordId == userPermissionItem.PasswordId) && PermissionsSelected)
                                {
                                    var MissingUser = DatabaseContext.Users.Where(u => u.Id == userPermissionItem.Id).Single();
                                    userPermissionItem.CanViewPassword = true;                                                          //default
                                    model.UserPermissions[intPermissionIndex].CanViewPassword = true;                                   //for the UI
                                    model.UserPermissions[intPermissionIndex].UserPasswordUser = CurrentPermissionRecordUser;           //for the UI
                                    DatabaseContext.Entry(userPermissionItem).State = EntityState.Added;                                //add new record
                                }
                                //the user DOES have a record, and has a selected permission
                                else if (CurrentPermissionRecordUser.UserPasswords.Any(up => up.PasswordId == userPermissionItem.PasswordId) && PermissionsSelected)
                                {
                                    userPermissionItem.CanViewPassword = true;                                                          //default
                                    model.UserPermissions[intPermissionIndex].CanViewPassword = true;                                   //for the UI
                                    model.UserPermissions[intPermissionIndex].UserPasswordUser = CurrentPermissionRecordUser;           //for the UI
                                    DatabaseContext.Entry(userPermissionItem).State = EntityState.Modified;                             //update record
                                }
                                //user DOES have a record, and no permissions have been selected
                                //the view permission here gets a double check - because if that is unticked, everything else has to be
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



                            }
                        }

                        //supply the missing data, so the model can be returned
                        model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
                        model.EditPassword = AutoMapper.Mapper.Map<PasswordEdit>(selectedPassword);

                        //if there were no errors
                        if (ModelState.IsValid)
                        {
                            //save changes to DB
                            await DatabaseContext.SaveChangesAsync();

                            //send push notification (to update the UI)
                            //PushNotifications.sendUpdatedPasswordDetails(model.EditPassword);
                        }

                    }
                    //user does not have access to edit user permissions
                    else
                    {
                        //return just a readonly model
                        model.ViewPassword = AutoMapper.Mapper.Map<PasswordDisplay>(selectedPassword);
                        model.EditPassword = AutoMapper.Mapper.Map<PasswordEdit>(selectedPassword);

                        ModelState.AddModelError("", "You do not have permission to edit permissions for this password");
                    }
                }
                //user does not have access to view
                else
                {
                    //return empty model
                    model.ViewPassword = new PasswordDisplay();
                    model.EditPassword = new PasswordEdit();
                    model.UserPermissions = null;

                    ModelState.AddModelError("", "You do not have permission to view this password");
                }
            }
            //model is invalid
            else
            {
                //supply the missing data, so the model can be returned
                //return empty model
                model.ViewPassword = new PasswordDisplay();
                model.EditPassword = new PasswordEdit();
            }

            model.OpenTab = DefaultTab.EditPermissions;
            return View("ViewPassword", model);
        }


        //POST: /Password/GetEncryptedPassword/24
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetEncryptedPassword(int PasswordId)
        {
            int UserId = User.Identity.GetUserId().ToInt();
            bool userIsAdmin = User.IsInRole("Administrator");

            //Retrive the password -if the user has access
            Password selectedPassword = DatabaseContext.Passwords.Include("Parent_UserPasswords").Where(pass => !pass.Deleted
                                                            && (DatabaseContext.UserPasswords
                                                                        .Any(up => up.PasswordId == pass.PasswordId && up.Id == UserId))
                                                            || (
                                                                    userIsAdmin && ApplicationSettings.Default.AdminsHaveAccessToAllPasswords
                                                               )).SingleOrDefault(p => p.PasswordId == PasswordId);


            //return the password if it exists
            if (selectedPassword != null)
            {
                PasswordPassword passwordText = new PasswordPassword {PlainTextPassword = selectedPassword.EncryptedPassword };
                return Json(passwordText);
            }

            //password does not exist... or user does not have access
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        #endregion

        #region CategoryAndPasswordActions

        //POST: /Password/UpdatePosition/123?NewPosition=2&OldPosition=4&isCategory=true
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePosition(Int32 ItemId, Int16 NewPosition, Int16 OldPosition, bool isCategoryItem)
        {
            //check if user can edit categories
            if (User.CanEditCategories() || User.IsInRole("Administrator"))
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
                catch { }

            }

            return Json(new
            {
                Status = "Failed"
            });

        }

        #endregion

        #region helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion

    }
}
