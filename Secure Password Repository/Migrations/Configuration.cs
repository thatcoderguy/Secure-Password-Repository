namespace Secure_Password_Repository.Migrations
{
    using Secure_Password_Repository.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Secure_Password_Repository.Database.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Secure_Password_Repository.Database.ApplicationDbContext context)
        {

            //Add in the roles required for the Secure Password Repository
            context.Roles.AddOrUpdate(
                r => r.Name,
                new ApplicationRole { Name = "Administrator" },
                new ApplicationRole { Name = "User" },
                new ApplicationRole { Name = "Super User" }
            );

        }
    }
}
