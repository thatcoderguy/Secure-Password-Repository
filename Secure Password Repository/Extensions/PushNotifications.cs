using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Secure_Password_Repository.Hubs;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Extensions
{
    public sealed class PushNotifications
    {

        private static volatile PushNotifications _instance;
        private static object syncRoot = new Object();

        private PushNotifications() { }

        public static PushNotifications Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new PushNotifications(GlobalHost.ConnectionManager.GetHubContext<CategoryAndPasswordHub>().Clients);
                    }
                }

                return _instance;
            }
        }

        private PushNotifications(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public void sendUpdatedCategoryDetails(CategoryEdit updatedCategory)
        {
            _instance.Clients.All.sendCategoryDetails(updatedCategory);
        }

        public void sendUpdatedPasswordDetails(PasswordEdit updatedPassword)
        {
            _instance.Clients.All.sendCategoryDetails(updatedPassword);
        }

        public void sendDeletedCategoryDetails(Category deletedCategory)
        {
            _instance.Clients.All.sendDeletedCategoryDetails(deletedCategory);
        }

        public void sendDeletedPasswordDetails(Password deletedPassword)
        {
            _instance.Clients.All.sendDeletedPasswordDetails(deletedPassword);
        }

        public void sendAddedCategoryDetails(string addedCategory)
        {
            _instance.Clients.All.sendAddedCategoryDetails(addedCategory);
        }

        public void sendAddedPasswordDetails(string addedPassword)
        {
            _instance.Clients.All.sendAddedPasswordDetail(addedPassword);
        }

    }
}