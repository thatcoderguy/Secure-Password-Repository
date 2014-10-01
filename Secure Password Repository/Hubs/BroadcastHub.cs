using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
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

namespace Secure_Password_Repository.Hubs
{

    /// <summary>
    /// This class is used to push notifications of user actions
    /// to clients, so that the UI can be updated
    /// </summary>
    public class BroadcastHub : Hub
    {

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

    }

}