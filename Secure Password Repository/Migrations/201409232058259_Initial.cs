namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        Category_ParentID = c.Int(nullable: true),
                        CategoryOrder = c.Short(nullable: false),
                        Deleted = c.Boolean(nullable: false)
                    })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.Category", t => t.Category_ParentID)
                .Index(t => t.Category_ParentID);
            
            CreateTable(
                "dbo.Password",
                c => new
                    {
                        PasswordId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        EncryptedUserName = c.String(nullable: false),
                        EncryptedSecondCredential = c.String(),
                        EncryptedPassword = c.String(nullable: false),
                        Location = c.String(nullable: false),
                        Notes = c.String(),
                        Parent_CategoryId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        PasswordOrder = c.Short(nullable: false),
                        Creator_Id = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime()
                    })
                .PrimaryKey(t => t.PasswordId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.Category", t => t.Parent_CategoryId)
                .Index(t => t.Parent_CategoryId)
                .Index(t => t.Creator_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        userPrivateKey = c.String(),
                        userPublicKey = c.String(),
                        userEncryptionKey = c.String(),
                        userFullName = c.String(),
                        isAuthorised = c.Boolean(nullable: false),
                        userLastEncryptionKeyUpdate = c.DateTime(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256)
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String()
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false)
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false)
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserPassword",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PasswordId = c.Int(nullable: false),
                        CanEditPassword = c.Boolean(nullable: false),
                        CanDeletePassword = c.Boolean(nullable: false)
                    })
                .PrimaryKey(t => new { t.Id, t.PasswordId })
                .ForeignKey("dbo.AspNetUsers", t => t.Id, cascadeDelete: true)
                .ForeignKey("dbo.Password", t => t.PasswordId)
                .Index(t => t.Id)
                .Index(t => t.PasswordId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256)
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Password", "Parent_CategoryId", "dbo.Category");
            DropForeignKey("dbo.Password", "Creator_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPassword", "PasswordId", "dbo.Password");
            DropForeignKey("dbo.UserPassword", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Category", "Category_ParentID", "dbo.Category");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.UserPassword", new[] { "PasswordId" });
            DropIndex("dbo.UserPassword", new[] { "Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Password", new[] { "Creator_Id" });
            DropIndex("dbo.Password", new[] { "Parent_CategoryId" });
            DropIndex("dbo.Category", new[] { "Category_ParentID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.UserPassword");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Password");
            DropTable("dbo.Category");
        }
    }
}
