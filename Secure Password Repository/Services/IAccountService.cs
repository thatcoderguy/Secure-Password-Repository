using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Secure_Password_Repository.Models;

namespace Secure_Password_Repository.Services
{
    interface IAccountService: IDisposable
    {
        ApplicationUser GetUserAccount();
        int GetUserId();
        bool UserIsAnAdministrator();
    }
}
