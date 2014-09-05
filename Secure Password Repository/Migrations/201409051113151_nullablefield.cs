namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullablefield : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "userLastEncryptionKeyUpdate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "userLastEncryptionKeyUpdate", c => c.DateTime(nullable: false));
        }
    }
}
