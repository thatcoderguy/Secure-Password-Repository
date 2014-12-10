using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Services
{
    interface IPermissionService
    {
        bool CanAddCategory();
        bool CanEditCategory();
        bool CanDeleteCategory();
        bool CanAddPassword();
        bool CanDeletePassword();
        bool CanEditPassword();
        bool CanEditPasswordPermissions();
    }
}
