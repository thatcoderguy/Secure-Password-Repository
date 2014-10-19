using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.Identity;
using Secure_Password_Repository.Controllers;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using Secure_Password_Repository.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Data.Entity;

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
            string categoryPartialView = RenderViewContent.RenderViewToString("Password", "_CategoryItem", returnCategoryViewItem);

            //broadcast the new category details
            PushNotifications.sendAddedCategoryDetails(categoryPartialView, newCategory.Category_ParentID);
        }

        public void getNewPasswordDetails(Int32 newPasswordId)
        {

            int UserId = HttpContext.Current.User.Identity.GetUserId().ToInt();
            bool userIsAdmin = HttpContext.Current.User.IsInRole("Administrator");

            //get a list of userIds that have UserPassword records for this password
            var UserIDList = DatabaseContext.UserPasswords.Where(up => up.PasswordId == newPasswordId).Select(up => up.Id).ToList();

            //Retrive the password -if the user has access
            Password newPassword = DatabaseContext.Passwords
                                                            .Where(pass => !pass.Deleted
                                                            && (
                                                                (UserIDList.Contains(UserId))
                                                             || (userIsAdmin && ApplicationSettings.Default.AdminsHaveAccessToAllPasswords)
                                                             || pass.Creator_Id == UserId)
                                                                )
                                                                .Include(p => p.Parent_UserPasswords.Select(up => up.UserPasswordUser))
                                                                .SingleOrDefault(p => p.PasswordId == newPasswordId);

            

            if (newPassword != null)
            {
                //map new password to display view model
                AutoMapper.Mapper.CreateMap<Password, PasswordItem>();
                PasswordItem returnPasswordViewItem = AutoMapper.Mapper.Map<PasswordItem>(newPassword);

                //generate a string based view of the new category
                string passwordPartialView = RenderViewContent.RenderViewToString("Password", "_PasswordItem", returnPasswordViewItem);

                //broadcast the new password details
                PushNotifications.sendAddedPasswordDetails(passwordPartialView, returnPasswordViewItem.Parent_CategoryId);
            }

        }

    }

}