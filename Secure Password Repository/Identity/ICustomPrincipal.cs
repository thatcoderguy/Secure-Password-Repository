using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Identity
{
    interface ICustomPrincipal : IPrincipal
    {
        bool CanEditCategories();
        bool CanDeleteCategories();
        bool CanAddCategories();
        bool CanAddPasswords();
        bool isAdministrator();
    }
}
