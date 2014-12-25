using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Services
{
    public interface IPermissionService
    {
        bool CanAddCategories();
        bool CanEditCategories();
        bool CanDeleteCategories();
        bool CanAddPassword();
        bool CanDeletePassword();
        bool CanEditPassword();
        bool CanViewPassword(Password passworditem);
        bool CanEditPasswordPermissions();
    }
}
