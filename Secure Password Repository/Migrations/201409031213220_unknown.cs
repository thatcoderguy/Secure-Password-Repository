namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unknown : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Password", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Password", "EncryptedUserName", c => c.String(nullable: false));
            AlterColumn("dbo.Password", "EncryptedPassword", c => c.String(nullable: false));
            AlterColumn("dbo.Password", "Location", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Password", "Location", c => c.String());
            AlterColumn("dbo.Password", "EncryptedPassword", c => c.String());
            AlterColumn("dbo.Password", "EncryptedUserName", c => c.String());
            AlterColumn("dbo.Password", "Description", c => c.String());
        }
    }
}
