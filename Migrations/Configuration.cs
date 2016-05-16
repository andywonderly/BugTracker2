namespace BugTracker2.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker2.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker2.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));

            if (!context.Users.Any(r => r.DisplayName == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Users.Any(r => r.DisplayName == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Users.Any(r => r.DisplayName == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Users.Any(r => r.DisplayName == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "andywonderly@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "andywonderly@gmail.com",
                    Email = "andywonderly@gmail.com",
                    FirstName = "Andrew",
                    LastName = "Wonderly",
                    DisplayName = "Andy"
                }, "clickboom");
            }

            var userId = userManager.FindByEmail("andywonderly@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            context.TicketPriorities.AddOrUpdate(
                n => n.Name,
                new TicketPriority { Name = "Low" },
                new TicketPriority { Name = "Normal" },
                new TicketPriority { Name = "High" }

                );

            context.TicketStatuses.AddOrUpdate(
                n => n.Name,
                new TicketStatus { Name = "Submitted" },
                new TicketStatus { Name = "Open" },
                new TicketStatus { Name = "Resolved" }
                );

            context.TicketTypes.AddOrUpdate(
                n => n.Name,
                new TicketType { Name = "Troubleshooting" },
                new TicketType { Name = "Operation Assistance" },
                new TicketType { Name = "Other" }
                );
        }
    }
}
