namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userpasswordextrafields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserPassword", "CanEditPassword", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserPassword", "CanDeletePassword", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserPassword", "CanDeletePassword");
            DropColumn("dbo.UserPassword", "CanEditPassword");
        }
    }
}
