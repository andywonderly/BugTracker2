using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models.Helpers
{
    public class ProjectsHelper
    {

        ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.FirstOrDefault(p=> p.Id == projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }

        public ICollection<Projects> ListUserProjects(string userId)
        {
            //ApplicationUser user = db.Users.Find(userId);
            IEnumerable<Projects> project = new List<Projects>().Where(n => n.Id.ToString() == userId);
            ICollection<Projects> projects = project.ToList();
            projects = project.ToList();
            return (projects);
        }
    }
}