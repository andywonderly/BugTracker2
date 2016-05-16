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

            if (!context.Users.Any(r => r.DisplayName == "Admin Demo"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin Demo" });
            }
            if (!context.Users.Any(r => r.DisplayName == "Project Manager Demo"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager Demo" });
            }
            if (!context.Users.Any(r => r.DisplayName == "Developer Demo"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer Demo" });
            }
            if (!context.Users.Any(r => r.DisplayName == "Submitter Demo"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter Demo" });
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

            //Create demo logins

            if (!context.Users.Any(u => u.Email == "AdminDemo@awonderly-bugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Admin Demo",
                    Email = "AdminDemo@awonderly-bugtracker.com",
                    FirstName = "Admin",
                    LastName = "Demo",
                    DisplayName = "Admin Demo"
                }, "clickboom");
            }

            //var adminDemoId = userManager.FindByEmail("AdminDemo@awonderly-bugtracker.com").Id;
            //userManager.AddToRole(adminDemoId, "Admin Demo");

            if (!context.Users.Any(u => u.Email == "ProjectManagerDemo@awonderly-bugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Project Manager Demo",
                    Email = "ProjectManagerDemo@awonderly-bugtracker.com",
                    FirstName = "Project Manager",
                    LastName = "Demo",
                    DisplayName = "Project Manager Demo"
                }, "clickboom");
            }

            //var PMDemoId = userManager.FindByEmail("ProjectManagerDemo@awonderly-bugtracker.com").Id;
            //userManager.AddToRole(PMDemoId, "Project Manager Demo");

            if (!context.Users.Any(u => u.Email == "DeveloperDemo@awonderly-bugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Developer Demo",
                    Email = "DeveloperDemo@awonderly-bugtracker.com",
                    FirstName = "Developer",
                    LastName = "Demo",
                    DisplayName = "Devloper Demo"
                }, "clickboom");
            }

            //var developerDemoId = userManager.FindByEmail("DeveloperDemo@awonderly-bugtracker.com").Id;
            //userManager.AddToRole(developerDemoId, "Developer Demo");

            if (!context.Users.Any(u => u.Email == "SubmitterDemo@awonderly-bugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Submitter Demo",
                    Email = "SubmitterDemo@awonderly-bugtracker.com",
                    FirstName = "Submitter",
                    LastName = "Demo",
                    DisplayName = "Submitter Demo"
                }, "clickboom");
            }

            //var submitterDemoId = userManager.FindByEmail("SubmitterDemo@awonderly-bugtracker.com").Id;
            //userManager.AddToRole(developerDemoId, "Submitter Demo");

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
