using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Models
{
    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }

    public class CustomUserStore : UserStore<ApplicationUser, ApplicationRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(DbContext context)
            : base(context)
        {
        }
    }
    public class CustomRoleStore : RoleStore<ApplicationRole, int, CustomUserRole>
    {
        public CustomRoleStore(DbContext context)
            : base(context)
        {
        }
    }

    public class ApplicationRole : IdentityRole<int, CustomUserRole>
    {
        public ApplicationRole()
        {
        }
        public ApplicationRole(string name) : this() { Name = name; }
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public string userPrivateKey { get; set; }
        public string userPublicKey { get; set; }
        public string userEncryptionKey { get; set; }
        public string userFullName { get; set; }
        [Display(Name = "Account authorised by Admin")]
        public bool isAuthorised { get; set; }
        public DateTime? userLastEncryptionKeyUpdate { get; set; }
        public virtual ICollection<UserPassword> UserPasswords { get; set; }
        public virtual ICollection<Password> Passwords { get; set; }
        public bool isActive { get; set; }

        public ApplicationRole GetRole()
        {
            ApplicationDbContext DatabaseContext = new ApplicationDbContext();

            foreach (var role in this.Roles)
            {
                return DatabaseContext.Roles.FirstOrDefaultAsync(r => r.Id == role.RoleId).Result;
            }

            return null;

        }

        public string GetRoleName()
        {

            ApplicationDbContext DatabaseContext = new ApplicationDbContext();

            foreach (var role in this.Roles)
            {
                //grab first role that the user has (there should be only 1)
                ApplicationRole myRole = DatabaseContext.Roles.FirstOrDefaultAsync(r => r.Id == role.RoleId).Result;
                return myRole.Name;
            }

            return "";
        }

        public bool CanOverridePasswordPermissions() 
        {
            return ApplicationSettings.Default.AdminsHaveAccessToAllPasswords && this.GetRoleName() == "Administrator";
        }

        public bool CanEditCategories()
        {
            string RoleName = this.GetRoleName();
            return ApplicationSettings.Default.RoleAllowEditCategories != "None" && ((RoleName == ApplicationSettings.Default.RoleAllowEditCategories) || RoleName == "Administrator");
        }

        public bool CanDeleteCategories()
        {
            string RoleName = this.GetRoleName();
            return ApplicationSettings.Default.RoleAllowDeleteCategories != "None" && ((RoleName == ApplicationSettings.Default.RoleAllowDeleteCategories) || RoleName == "Administrator");
        }

        public bool CanAddCategories()
        {
            string RoleName = this.GetRoleName();
            return ApplicationSettings.Default.RoleAllowAddCategories != "None" && ((RoleName == ApplicationSettings.Default.RoleAllowAddCategories) || RoleName == "Administrator");
        }

        public bool CanAddPasswords()
        {
            string RoleName = this.GetRoleName();
            return ApplicationSettings.Default.RoleAllowAddPasswords != "None" && ((RoleName == ApplicationSettings.Default.RoleAllowAddPasswords) || RoleName == "Administrator");
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
    }

}