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
            var project = db.ProjectUsers.Find(id);
            UserRolesHelper helper = new UserRolesHelper(db);
            var model = new ProjectsViewModel();

            List<string> users = new List<string>();

            foreach( var u in db.Users)
            {
                users.Add(u.DisplayName);
            }

            model.UserId = project.UserId;
            model.Id = project.Id;
            model.selected = users;
            model.roles = new MultiSelectList(db.Roles, "Name", "Name", model.selected);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectUsers([Bind(Include = "selected, Id, Name, roles")] AdminUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Declare variable of the application context
                //var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var context = this.db;

                //Create new roleManager to allow access to helper functions
                var roleManager = new UserRolesHelper(context);

                //List of roles to be added
                List<string> rolesToAdd = new List<string>();

                //List of roles to be removed.  Starts out as all roles
                //FUTURE UPDATE - get roles from role list rather than typing the all out.
                List<string> rolesToRemove = new List<string> { "Admin", "Developer", "Project Manager", "Submitter" };

                //The following commented-out code was an attempt to get the existing roles as a list
                //var roleStore = new RoleStore<IdentityRole>(context);
                //var roleMngr = new RoleManager<IdentityRole>(roleStore);
                //var roles = roleMngr.Roles.ToList();


                //Add the roles to be added to rolesToAdd list
                foreach (var item in model.selected)
                    rolesToAdd.Add(item);

                //remove roles to be added from rolesToRemove list
                foreach (var item in rolesToAdd)
                {
                    rolesToRemove.Remove(item);
                }

                //Add user to role if user is not in the role already
                foreach (var item in rolesToAdd)
                {
                    if (!roleManager.IsUserInRole(model.Id, item))
                        roleManager.AddUserToRole(model.Id, item);
                }

                //Remove user from role if the role was not selected but they are in the role
                foreach (var item in rolesToRemove)
                {
                    if (roleManager.IsUserInRole(model.Id, item))
                        roleManager.RemoveUserFromRole(model.Id, item);
                }


                db.SaveChanges();

            }

            return RedirectToAction("Index");
        }

    }

}