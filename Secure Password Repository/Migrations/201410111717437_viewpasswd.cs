namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class viewpasswd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserPassword", "CanViewPassword", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserPassword", "CanChangePermissions", c => c.Boolean(nullable: false));

        }
        
        public override void Down()
        {
            DropColumn("dbo.UserPassword", "CanViewPassword");
            DropColumn("dbo.UserPassword", "CanChangePermissions");
        }
    }
}
