using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Transports;
using Secure_Password_Repository.Hubs;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace Secure_Password_Repository.Extensions
{
    /// <summary>
    /// Class to handle broadcasting of messages from the SignalR Hub
    /// </summary>
    public static class PushNotifications
    {

        /// <summary>
        /// Broadcasts the details of an updated category, so that the clients can update their UI
        /// </summary>
        /// <param name="updatedCategory">Instance of the updated category view model</param>
        public static void sendUpdatedCategoryDetails(CategoryEdit updatedCategory)
        {
            //broadcast details to all clients except the one requesting the broadcast
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.AllExcept(MemoryCache.Default.Get(HttpContext.Current.User.Identity.Name + HttpContext.Current.Session.SessionID + "-connectionId").ToString()).sendUpdatedCategoryDetails(updatedCategory);
        }

        /// <summary>
        /// Broadcasts the details of an updated password, so that the clients can update their UI
        /// </summary>
        /// <param name="updatedPassword">Instance of the updated password view model</param>
        /// <param name="clientConnectionId">Connection ID of the client requesting the broadcast</param>
        public static void sendUpdatedPasswordDetails(PasswordEdit updatedPassword)
        {
            //broadcast details to all clients except the one requesting the broadcast
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.AllExcept(MemoryCache.Default.Get(HttpContext.Current.User.Identity.Name + HttpContext.Current.Session.SessionID + "-connectionId").ToString()).sendUpdatedPasswordDetails(updatedPassword);
        }

        /// <summary>
        /// Broadcasts the details of a deleted category, so that the clients can update their UI
        /// </summary>
        /// <param name="deletedCategory">Instance of the deleted category model</param>
        /// <param name="clientConnectionId">Connection ID of the client requesting the broadcast</param>
        public static void sendDeletedCategoryDetails(Category deletedCategory)
        {
            //broadcast details to all clients except the one requesting the broadcast
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.AllExcept(MemoryCache.Default.Get(HttpContext.Current.User.Identity.Name + HttpContext.Current.Session.SessionID + "-connectionId").ToString()).sendDeletedCategoryDetails(deletedCategory);
        }

        /// <summary>
        /// Broadcasts the details of a deleted password, so that the clients can update their UI
        /// </summary>
        /// <param name="deletedPassword">Instance of the deleted password model</param>
        /// <param name="clientConnectionId">Connection ID of the client requesting the broadcast</param>
        public static void sendDeletedPasswordDetails(Password deletedPassword)
        {
            //broadcast details to all clients except the one requesting the broadcast
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.AllExcept(MemoryCache.Default.Get(HttpContext.Current.User.Identity.Name + HttpContext.Current.Session.SessionID + "-connectionId").ToString()).sendDeletedPasswordDetails(deletedPassword);
        }

        /// <summary>
        /// Broadcasts the details of a newly created category, so that the clients can update their UI
        /// </summary>
        /// <param name="addedCategory">View rendered to a string of the new category</param>
        /// <param name="clientConnectionId">Connection ID of the client requesting the broadcast</param>
        public static void sendAddedCategoryDetails(string addedCategory, int? categoryParentId)
        {
            //broadcast details to only this client - the one requesting the broadcast
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.Client(MemoryCache.Default.Get(HttpContext.Current.User.Identity.Name + HttpContext.Current.Session.SessionID + "-connectionId").ToString()).sendAddedCategoryDetails(addedCategory, categoryParentId);
        }

        /// <summary>
        /// Broadcasts the details of a newly created password, so that the clients can update their UI
        /// </summary>
        /// <param name="addedPassword">View rendered to a string of the new password</param>
        /// <param name="clientConnectionId">Connection ID of the client requesting the broadcast</param>
        public static void sendAddedPasswordDetails(string addedPassword, int? passwordParentId)
        {
            //broadcast details to ALL clients
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.Client(MemoryCache.Default.Get(HttpContext.Current.User.Identity.Name + HttpContext.Current.Session.SessionID + "-connectionId").ToString()).sendAddedPasswordDetails(addedPassword, passwordParentId);
        }

        /// <summary>
        /// Broadcasts the position details of an item that has been reordered
        /// </summary>
        /// <param name="ItemID">The HTML id of the item moved</param>
        /// <param name="OldPosition">The item's old position</param>
        /// <param name="NewPosition">The item's new position</param>
        public static void sendUpdatedItemPosition(string ItemID, Int16 NewPosition, Int16 OldPosition)
        {
            //broadcast details to all clients except the one requesting the broadcast
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.AllExcept(MemoryCache.Default.Get(HttpContext.Current.User.Identity.Name + HttpContext.Current.Session.SessionID + "-connectionId").ToString()).sendUpdatedItemPosition(ItemID, NewPosition, OldPosition);
        }

        public static void newCategoryAdded(Int32 newCategoryId)
        {
            //broadcast details to all clients except the one requesting the broadcast
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.AllExcept(MemoryCache.Default.Get(HttpContext.Current.User.Identity.Name + HttpContext.Current.Session.SessionID + "-connectionId").ToString()).newCategoryAdded(newCategoryId);
        }

        public static void newPasswordAdded(Int32 newPasswordId)
        {
            //broadcast details to all clients except the one requesting the broadcast
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
            hubContext.Clients.All.newPasswordAdded(newPasswordId);
        }

    }
} 