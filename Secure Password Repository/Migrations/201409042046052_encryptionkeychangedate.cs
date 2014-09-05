namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class encryptionkeychangedate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "userLastEncryptionKeyUpdate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "userLastEncryptionKeyUpdate");
        }
    }
}
