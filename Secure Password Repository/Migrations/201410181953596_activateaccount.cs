namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activateaccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "isActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "isActive");
        }
    }
}
