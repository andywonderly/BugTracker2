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
            IEnumerable<ApplicationUser> projectUsers = project.Users.ToList();
            bool userIsOnProject = projectUsers.Any(n => n.Id == user.Id);
            ProjectUsersHelper projectUsersHelper = new ProjectUsersHelper();

            if (!projectUsersHelper.IsUserOnProject(user.Id, project.Id)) //only add user if they are not already on the project
            {
                project.Users.Add(user);
                db.SaveChanges();
            }
            
        }

        public void RemoveUserFromProject(string projectId, string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            Projects project = db.Projects.Find(projectId);
            IEnumerable<ApplicationUser> projectUsers = project.Users.ToList();
            bool userIsOnProject = projectUsers.Any(n => n.Id == user.Id);
            ProjectUsersHelper projectUsersHelper = new ProjectUsersHelper();

            if (projectUsersHelper.IsUserOnProject(user.Id, project.Id))
            {
                project.Users.Remove(user);
                db.SaveChanges();
            }
        }

        public IList<string> ListProjectUsers(string projectId)
        {
            Projects project = db.Projects.Find(projectId);
            IList<string> projectUserList = new List<string>();

            //projectUserList = project.Users.Where(x => x.Id == )

            foreach (var item in project.Users)
                projectUserList.Add(item.DisplayName);
            
            return projectUserList;
        }

        public IList<string> ListNonProjectUsers(string projectId)
        {
            Projects project = db.Projects.Find(projectId);
            List<ApplicationUser> userList = db.Users.ToList(); //list of all users
            IList<string> nonUserDisplayNames = new List<string>();

            foreach (var item in project.Users) //remove project users from all users to get non-project users
                userList.Remove(item);

            foreach (var item in userList) //add non-project user display names to nonUserDisplayNames
                nonUserDisplayNames.Add(item.DisplayName);

            return nonUserDisplayNames;
        }

        public List<Projects> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            IEnumerable<Projects> projects = db.Projects.Where(x => x.Users == user);
            List<Projects> projectsList = projects.ToList();
            return projectsList;
        }

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }


    }


}