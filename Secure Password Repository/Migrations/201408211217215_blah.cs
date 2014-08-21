namespace Secure_Password_Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blah : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Category", "Category_CategoryId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Category", "Category_CategoryId", c => c.Int(nullable: false));
        }
    }
}
