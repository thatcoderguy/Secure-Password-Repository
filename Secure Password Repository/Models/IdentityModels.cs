using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.Database;
using System;

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

        public ICollection<UserPassword> UserPasswords { get; set; }

        public string GetRoleName()
        {

            ApplicationDbContext DatabaseContext = new ApplicationDbContext();
            string MyRoles = string.Empty;
            
            DatabaseContext.Users.Load();

            //grab this user account from the database
            var thisUserAccount = DatabaseContext.Users.FirstOrDefaultAsync(u => u.Id == Id).Result;
            if (thisUserAccount != null)
                foreach (var role in thisUserAccount.Roles)
                {
                    //grab first role that the user has (there should be only 1)
                    var myRole = DatabaseContext.Roles.FirstOrDefaultAsync(r => r.Id == role.RoleId).Result;

                    if (myRole != null)
                        MyRoles += myRole.Name;
                    else
                        //throw new PasswordRepositoryException("User does not have any roles");
                        MyRoles = string.Empty;
                }
                //user does not exist... some how
            else
                //throw new PasswordRepositoryException("User does not exist");
                MyRoles = string.Empty;

            return MyRoles;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
    }

}