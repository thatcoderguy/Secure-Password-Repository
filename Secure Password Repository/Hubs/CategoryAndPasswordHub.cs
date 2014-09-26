using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Secure_Password_Repository.Controllers;
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
    [HubName("applicationHub")]
    public class CategoryAndPasswordHub : Hub
    {

    }

}