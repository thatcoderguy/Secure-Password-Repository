using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.Identity;
using Secure_Password_Repository.Controllers;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Secure_Password_Repository.Settings;

namespace Secure_Password_Repository.Hubs
{

    /// <summary>
    /// This class is used to push notifications of user actions
    /// to clients, so that the UI can be updated
    /// </summary>
    public class BroadcastHub : Hub
    {

        private ApplicationDbContext DatabaseContext = new ApplicationDbContext();

        public override Task OnConnected()
        {
            System.Runtime.Caching.MemoryCache.Default.Set(
                                                            HttpContext.Current.User.Identity.Name + "-connectionId", 
                                                            Context.ConnectionId, 
                                                            new CacheItemPolicy() { 
                                                                                    Priority = CacheItemPriority.Default, 
                                                                                    SlidingExpiration = TimeSpan.FromHours(1), 
                                                                                    AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration });

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            System.Runtime.Caching.MemoryCache.Default.Set(
                                                            HttpContext.Current.User.Identity.Name + "-connectionId", 
                                                            Context.ConnectionId, 
                                                            new CacheItemPolicy() { 
                                                                                    Priority = CacheItemPriority.Default, 
                                                                                    SlidingExpiration = TimeSpan.FromHours(1), 
                                                                                    AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration });

            return base.OnReconnected();
        }

        public void getNewCategoryDetails(Int32 newCategoryId)
        {
            //retreive the new category that was just created
            Category newCategory = DatabaseContext.Categories.Single(c => c.CategoryId == newCategoryId);
            
            //map new category to display view model
            AutoMapper.Mapper.CreateMap<Category, CategoryItem>();
            CategoryItem returnCategoryViewItem = AutoMapper.Mapper.Map<CategoryItem>(newCategory);

            //generate a string based view of the new category
            string categoryPartialView = RenderViewContent.RenderPartialToString("Password", "_CategoryItem", returnCategoryViewItem);

            //broadcast the new category details
            PushNotifications.sendAddedCategoryDetails(categoryPartialView, newCategory.Category_ParentID);
        }

        public void getNewPasswordDetails(Int32 newPasswordId)
        {
            int UserId = HttpContext.Current.User.Identity.GetUserId().ToInt();
            bool canAccessAllPassword = HttpContext.Current.User.IsInRole("Administrator")
                                                && ApplicationSettings.Default.AdminsHaveAccessToAllPasswords;

            //retreive the new password that was just created
            Password newPassword = DatabaseContext
                                        .Passwords
                                        .Where(pass => !pass.Deleted
                                        && (DatabaseContext.UserPasswords
                                                    .Any(up => up.PasswordId == pass.PasswordId && up.Id == UserId))
                                        || (
                                                canAccessAllPassword
                                           ))
                                        .Select(p => new Password()
                                        {
                                            PasswordId = p.PasswordId,
                                            Creator = p.Creator,
                                            Creator_Id = p.Creator_Id,
                                            Description = p.Description,
                                            Parent_UserPasswords = p.Parent_UserPasswords.Where(up => up.Id == UserId).ToList(),
                                            CreatedDate = p.CreatedDate,
                                             Parent_CategoryId = p.Parent_CategoryId,
                                              Parent_Category = p.Parent_Category,
                                               Deleted = p.Deleted,
                                                EncryptedPassword = p.EncryptedPassword,
                                                 EncryptedSecondCredential = p.EncryptedSecondCredential,
                                                  EncryptedUserName =p.EncryptedUserName,
                                                   ModifiedDate= p.ModifiedDate,
                                                    Location = p.Location,
                                                     Notes = p.Notes,
                                                      PasswordOrder = p.PasswordOrder
                                        })
                                        .SingleOrDefault(p => p.PasswordId == newPasswordId);                                        

            //map new password to display view model
            AutoMapper.Mapper.CreateMap<Password, PasswordItem>();
            PasswordItem returnPasswordViewItem = AutoMapper.Mapper.Map<PasswordItem>(newPassword);

            //generate a string based view of the new category
            string passwordPartialView = RenderViewContent.RenderPartialToString("Password", "_PasswordItem", returnPasswordViewItem);

            //broadcast the new password details
            PushNotifications.sendAddedPasswordDetails(passwordPartialView, returnPasswordViewItem.Parent_CategoryId);

        }

    }

}