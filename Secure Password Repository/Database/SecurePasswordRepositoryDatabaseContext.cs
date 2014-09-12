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

        public virtual DbSet<Password> Passwords { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<UserPassword> UserPasswords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasRequired(a => a.Parent_Category)
                .WithMany(b => b.SubCategories)
                .HasForeignKey(c => c.Category_ParentID) // FK_Category_ParentID
                .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Password>()
                .HasRequired(a => a.Parent)
                .WithMany(b => b.Passwords)
                .HasForeignKey(c => c.Parent_CategoryId) // FK_Parent_CategoryId
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserPassword>()
                .HasRequired(a => a.)
                .WithMany(b => b.)
                .HasForeignKey(c => c.PasswordId)
                .WillCascadeOnDelete(false);

            


        }

    }

}