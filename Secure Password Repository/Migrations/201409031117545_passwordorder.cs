namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class passwordorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Password", "PasswordOrder", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Password", "PasswordOrder");
        }
    }
}
