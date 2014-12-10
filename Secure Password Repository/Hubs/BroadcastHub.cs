using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Utilities;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Settings;
using Secure_Password_Repository.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;

namespace Secure_Password_Repository.Hubs
{

    /// <summary>
    /// This class is used to push notifications of user actions
    /// to clients, so that the UI can be updated
    /// </summary>
    public class BroadcastHub : Hub
    {

        private ApplicationDbContext DatabaseContext = new ApplicationDbContext();

        /// <summary>
        /// When client connects, store the connection id - used later to ignore certain clients when broadcasting
        /// </summary>
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

        /// <summary>
        /// When client reconnects, store the connection id - used later to ignore certain clients when broadcasting
        /// </summary>
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

        /// <summary>
        /// When a new category is added, all clients request a copy via this method - this is so the view can be rendered with correct permissions
        /// </summary>
        /// <param name="newcategoryid">ID of the new category</param>
        public void getNewCategoryDetails(Int32 newcategoryid)
        {
            ApplicationUserManager UserMgr = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            int UserId = HttpContext.Current.User.Identity.GetUserId().ToInt();
            ApplicationUser CurrentUser = UserMgr.FindById(UserId);

            //retreive the new category that was just created
            Category newCategoryDetails = DatabaseContext.Categories.SingleOrDefault(c => c.CategoryId == newcategoryid);

            //category not found
            if (newCategoryDetails == null)
                return;
            
            //map new category to display view model
            AutoMapper.Mapper.CreateMap<Category, CategoryItem>();
            CategoryItem returnCategoryViewItem = AutoMapper.Mapper.Map<CategoryItem>(newCategoryDetails);

            //generate a string based view of the new category
            string categoryPartialView = RenderViewContent.RenderViewToString("Password", "_CategoryItem", returnCategoryViewItem, CurrentUser);

            //broadcast the new category details
            PushNotifications.sendAddedCategoryDetails(categoryPartialView, newCategoryDetails.Category_ParentID);
        }

        /// <summary>
        /// When a new password is added, all clients request a copy via this method - this is so the view can be rendered with correct permissions
        /// </summary>
        /// <param name="newPasswordId">ID of the new password</param>
        public void getNewPasswordDetails(Int32 newPasswordId)
        {
            ApplicationUserManager UserMgr = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            int UserId = HttpContext.Current.User.Identity.GetUserId().ToInt();
            bool userIsAdmin = HttpContext.Current.User.IsInRole("Administrator");
            ApplicationUser CurrentUser = UserMgr.FindById(UserId);

            //get a list of userIds that have UserPassword records for this password
            var UserIDList = DatabaseContext.UserPasswords.Where(up => up.PasswordId == newPasswordId).Select(up => up.Id).ToList();

            //Retrive the password -if the user has access
            Password newPassword = DatabaseContext.Passwords
                                                            .Include("Creator")
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
                string passwordPartialView = RenderViewContent.RenderViewToString("Password", "_PasswordItem", returnPasswordViewItem, CurrentUser);

                //broadcast the new password details
                PushNotifications.sendAddedPasswordDetails(passwordPartialView, returnPasswordViewItem.Parent_CategoryId, returnPasswordViewItem.PasswordId);
            }
            else
            {
                //we dont have access any more, so tell UI to remove the password
                PushNotifications.sendRemovePasswordAccess(new PasswordDelete()
                                                                    { 
                                                                         PasswordId = newPasswordId
                                                                    });
            }

        }


    }

}