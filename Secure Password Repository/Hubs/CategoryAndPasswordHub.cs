using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Secure_Password_Repository.Controllers;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Secure_Password_Repository.Hubs
{

    /// <summary>
    /// This class is used to push notifications of user actions
    /// to clients, so that the UI can be updated
    /// </summary>
    /// 
    public class CategoryAndPasswordHub : Hub
    {

        private readonly PushNotifications _categoryandpasswordHub;

        public CategoryAndPasswordHub() :
            this(PushNotifications.Instance)
        {

        }

        public CategoryAndPasswordHub(PushNotifications PushNotification)
        {
            _categoryandpasswordHub = PushNotification;
        }


        public void sendUpdatedCategoryDetails(CategoryEdit updatedCategory)
        {
            _categoryandpasswordHub.sendUpdatedCategoryDetails(updatedCategory);
        }

        public void sendUpdatedPasswordDetails(PasswordEdit updatedPassword)
        {
            _categoryandpasswordHub.sendUpdatedPasswordDetails(updatedPassword);
        }

        public void sendDeletedCategoryDetails(Category deletedCategory)
        {
            _categoryandpasswordHub.sendDeletedCategoryDetails(deletedCategory);
        }

        public void sendDeletedPasswordDetails(Password deletedPassword)
        {
            _categoryandpasswordHub.sendDeletedPasswordDetails(deletedPassword);
        }

        public void sendAddedCategoryDetails(string addedCategory)
        {
            _categoryandpasswordHub.sendAddedCategoryDetails(addedCategory);
        }

        public void sendAddedPasswordDetails(string addedPassword)
        {
            _categoryandpasswordHub.sendAddedPasswordDetails(addedPassword);
        }

    }

}