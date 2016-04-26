using BugTracker2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Collections.Generic;
using System.Web.Security;
using System;
using System.Net;
using BugTracker2.Models.Helpers;
using Newtonsoft.Json.Linq;

namespace BugTracker2.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Index / projects list
        [Authorize(Roles = "Admin, Project Manager, Developer")]
        public ActionResult Index()
        {
            var projects = db.Projects.ToList();
            return View(projects);
        }

        public ActionResult ListProjects()
        {

            return View();
        }

        //GET: Projects/CreateProject
        [Authorize(Roles="Admin")]
        public ActionResult CreateProject()
        {
            return View();
        }

        //POST: Projects/CreateProject
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateProject([Bind(Include = "Id, Name")] Projects project)
        {
            if (String.IsNullOrWhiteSpace(project.Name))
            {
                ModelState.AddModelError("Name", "A project name is required.");
                return View(project);
            }

            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View();
        }

        // GET: Projects/DeleteProject
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteProject(int id)
        {

            Projects project = db.Projects.FirstOrDefault(p => p.Id == id);




            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: BlogPosts/DeleteProject
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("DeleteProject")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects project = db.Projects.FirstOrDefault(p => p.Id == id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        // GET: Projects/EditProject
        public ActionResult EditProject(int id)
        {

            Projects project = db.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return HttpNotFound();
            }

            return View(project);

        }

        // POST: BlogPosts/EditComment/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProject([Bind(Include = "Id,Name")] Projects project)
        {
           

            if (ModelState.IsValid)
            {


                if (String.IsNullOrWhiteSpace(project.Name))
                {
                    ModelState.AddModelError("Name", "A project name is required.");
                    return View(project);
                }

                if (project.Name.Length > 250)
                {
                    ModelState.AddModelError("Name", "Name length cannot exceed 250 characters.");
                    return View(project);
                }

                db.Projects.Attach(project);
                db.Entry(project).Property("Name").IsModified = true;
                db.SaveChanges();

                return RedirectToAction("EditProject", new { id = project.Id });
            }

            var post2 = db.Projects.Find(project.Id);
            return RedirectToAction("EditProject", new { id = post2.Id });
        }

        // GET:  Edit Project Users!
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditProjectUsers(string id)
        {
            if (id == null)
            {
                RedirectToAction("Index");
            }

            var project = db.Projects.Find(id);
            var helper = new ProjectUsersHelper();
            var model = new ProjectsViewModel();

            model.ProjectId = project.Id.ToString();
            model.selected = helper.ListProjectUsers(id).ToArray();
            model.Users = new MultiSelectList(db.Users, "DisplayName", "DisplayName", model.selected);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectUsers([Bind(Include = "selected, Id, Name, roles")] ProjectsViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Declare variable of the application context
                //var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var context = this.db;

                //Create new helper to allow access to helper functions
                var helper = new ProjectUsersHelper();

                //List of users to be added
                List<string> usersToAdd = new List<string>();

                //List of roles to be removed.  Starts out as all roles
                //FUTURE UPDATE - get users from user list rather than typing them all out.
                List<string> usersToRemove = new List<string>();

                foreach (var item in db.Users)
                    usersToRemove.Add(item.DisplayName);

                //Add the users to be added to users list
                foreach (var item in model.selected)
                    usersToAdd.Add(item);

                //remove users to be added from usersToRemove list
                foreach (var item in usersToAdd)
                {
                    usersToRemove.Remove(item);
                }

                //Add user to project if user is not on the project already************

                IEnumerable<ApplicationUser> userList = db.Users;


                foreach (var item in usersToAdd)
                {
                    ApplicationUser userToAdd = userList.FirstOrDefault(n => item == n.DisplayName);
                    
                    if (!helper.IsUserOnProject(userToAdd.Id, model.Id))  //UserID, projectID
                        helper.AddUserToProject(model.UserId, model.Id.ToString());
                }

                //Remove user from project if the user was not selected but they are in the project
                foreach (var item in usersToRemove)
                {
                    ApplicationUser userToAdd = userList.FirstOrDefault(n => item == n.DisplayName);

                    if (helper.IsUserOnProject(userToAdd.Id, model.Id))  //UserID, projectID
                        helper.RemoveUserFromProject(model.UserId, model.Id.ToString());
                }


                db.SaveChanges();

            }

            return RedirectToAction("Index");
        }

    }

}