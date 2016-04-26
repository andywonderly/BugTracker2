using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models.Helpers
{
    public class ProjectUsersHelper
    {

        private ApplicationDbContext db;

        public void AddUserToProject(string projectId, string userId)
        {

            ApplicationUser user = db.Users.Find(userId);
            Projects project = db.Projects.Find(projectId);
            project.Users.Add(user);
        }

        public void RemoveUserFromProject(string projectId, string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            Projects project = db.Projects.Find(projectId);
            project.Users.Remove(user);
        }

        public List<ApplicationUser> ListProjectUsers(string projectId)
        {
            Projects project = db.Projects.Find(projectId);
            var users = project.Users.ToList();
            return users;
        }

        public List<Projects> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            IEnumerable<Projects> projects = db.Projects.Where(x => x.Users == user);
            List<Projects> projectsList = projects.ToList();
            return projectsList;
        }

        
    }


}