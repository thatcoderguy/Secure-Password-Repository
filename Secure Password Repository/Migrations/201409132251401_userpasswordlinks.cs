namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userpasswordlinks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserPassword", "PasswordId", "dbo.Password");
            AddColumn("dbo.Password", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Password", "ModifiedDate", c => c.DateTime());
            AddColumn("dbo.Password", "Creator_Id", c => c.Int());
            CreateIndex("dbo.Password", "Creator_Id");
            AddForeignKey("dbo.Password", "Creator_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.UserPassword", "PasswordId", "dbo.Password", "PasswordId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPassword", "PasswordId", "dbo.Password");
            DropForeignKey("dbo.Password", "Creator_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Password", new[] { "Creator_Id" });
            DropColumn("dbo.Password", "Creator_Id");
            DropColumn("dbo.Password", "ModifiedDate");
            DropColumn("dbo.Password", "CreatedDate");
            AddForeignKey("dbo.UserPassword", "PasswordId", "dbo.Password", "PasswordId", cascadeDelete: true);
        }
    }
}
