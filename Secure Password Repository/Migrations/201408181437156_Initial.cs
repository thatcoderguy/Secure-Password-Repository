namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {

            CreateTable(
                 "dbo.AspNetRoles",
                 c => new
                 {
                     Id = c.Int(nullable: false, identity: true),
                     Name = c.String(nullable: false, maxLength: 256),
                 })
                 .PrimaryKey(t => t.Id)
                 .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.Int(nullable: false),
                    RoleId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

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
                    UserName = c.String(nullable: false, maxLength: 256),
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
                    ClaimValue = c.String(),
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
                    UserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Category",
                c => new
                {
                    CategoryId = c.Int(nullable: false, identity: true),
                    CategoryName = c.String(nullable: false),
                    Category_CategoryId = c.Int(nullable: true),
                    CategoryOrder = c.Short(nullable: false),
                    Deleted = c.Boolean(nullable: false),
                    SubCategory = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.Category", t => t.Category_CategoryId, false)
                .Index(t => t.Category_CategoryId);

            CreateTable(
                "dbo.Password",
                c => new
                {
                    PasswordId = c.Int(nullable: false, identity: true),
                    Description = c.String(),
                    EncryptedUserName = c.String(),
                    EncryptedSecondCredential = c.String(),
                    EncryptedPassword = c.String(),
                    Location = c.String(),
                    Notes = c.String(),
                    CategoryId = c.Int(nullable: false),
                    Deleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.PasswordId)
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: false)
                .Index(t => t.CategoryId);

            CreateTable(
                "dbo.UserPassword",
                c => new
                {
                    Id = c.Int(nullable: false),
                    PasswordId = c.Int(nullable: false)
                })
                .PrimaryKey(t => new { t.Id, t.PasswordId })
                .ForeignKey("dbo.AspNetUsers", t => t.Id, cascadeDelete: false)
                .ForeignKey("dbo.Password", t => t.PasswordId, cascadeDelete: false)
                .Index(t => t.Id)
                .Index(t => t.PasswordId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.UserPassword", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPassword", "PasswordId", "dbo.Password");
            DropForeignKey("dbo.Password", "CategoryId", "dbo.Category");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.UserPassword");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Password");
            DropTable("dbo.Category");
        }
    }
}
