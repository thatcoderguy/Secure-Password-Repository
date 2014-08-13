using Microsoft.AspNet.Identity.EntityFramework;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Database
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Password> Passwords { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}