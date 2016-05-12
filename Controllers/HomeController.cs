﻿using BugTracker2.Models;
using BugTracker2.Models.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            var currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);
            ProjectUsersHelper projectUsersHelper = new ProjectUsersHelper();

            dashboardViewModel.UserId = currentUser.Id;
            dashboardViewModel.UserDisplayName = currentUser.DisplayName;
            dashboardViewModel.UserAssignments = db.Tickets.Where(n => n.AssignedToUserId == currentUserId).ToList();
            dashboardViewModel.UserProjects = new List<Project>();
            dashboardViewModel.UserTickets = new List<Ticket>();

            List<Project> allProjects = db.Projects.ToList();

            foreach(var item in allProjects)
                foreach (var item2 in item.ProjectUsers)
                    if (item2.Id == currentUserId)
                        dashboardViewModel.UserProjects.Add(item);

            List<Ticket> allTickets = db.Tickets.ToList();

            foreach (var item in allTickets)
                if (item.OwnerUserId == currentUserId)
                    dashboardViewModel.UserTickets.Add(item);
            
            return View(dashboardViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}