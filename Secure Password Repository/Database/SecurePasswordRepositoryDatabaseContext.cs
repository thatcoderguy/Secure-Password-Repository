﻿using Microsoft.AspNet.Identity.EntityFramework;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        public virtual DbSet<Password> tblPassword { get; set; }

        public virtual DbSet<Category> tblCategory { get; set; }

        public virtual DbSet<UserPassword> tblUserPassword { get; set; }

    }
}