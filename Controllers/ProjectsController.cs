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
            //Determine current user role(s) to determine which projects they see

            //Get current user id
            var currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            //Create helper objects to allow access to helper functions
            UserRolesHelper userRolesHelper = new UserRolesHelper(db);
            ProjectUsersHelper projectUsersHelper = new ProjectUsersHelper();

            //Get the list of current user roles
            var currentUserRoles = userRolesHelper.ListUserRoles(currentUserId);

            if (currentUserRoles == null)
            {
                return View();
            }

            if (currentUserRoles != null)
                foreach (var item in currentUserRoles)
                    if (item == "Admin")
                    {
                        //var projects = db.Projects.ToList();
                        var projects = db.Projects.ToList();
                        return View(projects); //return all projects if user is Admin
                    }

            if (currentUserRoles != null)
            {
                bool isPMOrDeveloper = false; //bool for whether the current user is a PM or developer
                                                           //Default is false

                foreach (var item in currentUserRoles) //test all roles for PM or dev.
                    if (item == "Project Manager" || item == "Developer")
                        isPMOrDeveloper = true; //if we hit one, set to true

                    if(isPMOrDeveloper)
                    {
                        //var userProjects = projectUsersHelper.ListUserProjects(currentUserId); //Get user project list
                        //var projects = userProjects.ToList();
                    
                    var projects = currentUser.Projects.ToList();
                        //foreach (var item2 in projects) //add them
                        //    projects.Add(item2);
                        
                        return View(projects); //return them
                    }
            }





            
            return View();
        }

        public ActionResult ListProjects()
        {

            return View();
        }

        //GET: Projects/CreateProject
        [Authorize(Roles="Admin")]
        public ActionResult CreateProject()
        {
            ProjectUsersHelper helper = new ProjectUsersHelper();
            var model = new ProjectViewModel();

            List<ApplicationUser> allUsers = db.Users.ToList();

            //var allUsers = helper.ListAllUserDisplayNames().ToArray();

            List<SelectListItem> users = new List<SelectListItem>();

            foreach (var item in allUsers)
                users.Add(new SelectListItem { Text = item.DisplayName, Value = item.Id });

            model.users = users;
            model.selected = db.Users.First().DisplayName.ToString();

            return View(model);
        }

        //POST: Projects/CreateProject
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateProject([Bind(Include = "Id, Name, selected")] ProjectViewModel project)
        {
            if (String.IsNullOrWhiteSpace(project.Name))
            {
                ModelState.AddModelError("Name", "A project name is required.");
                return View(project);
            }

            if (ModelState.IsValid)
            {
                //ApplicationUser projectManager = db.Users.FirstOrDefault(n => n.DisplayName == project.selected);
                //project.ProjectManagerUserId = projectManager.Id;

                var projectToAdd = new Projects();

                projectToAdd.Id = project.ProjectId;
                projectToAdd.Name = project.Name;
                //projectToAdd.ProjectManagerUserId = project.ProjectManagerUserId;
                


                db.Projects.Add(projectToAdd);
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

            var post2 = db.Projects.FirstOrDefault(p => p.Id == project.Id);
            return RedirectToAction("EditProject", new { id = post2.Id });
        }

        // GET:  Edit Project Users!
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditProjectUsers(int id)
        {
            //if (id == null)
            //{
            //    return RedirectToAction("Index");
            //}

            Projects project = db.Projects.FirstOrDefault(p => p.Id == id);

            if ( project == null)
            {
                return RedirectToAction("Index");
            }

            var helper = new ProjectUsersHelper();
            var model = new ProjectUserViewModel();

            //model.ProjectId = project.Id.ToString();
            model.selected = helper.ListProjectUsers(id).ToArray();
            model.users = new MultiSelectList(db.Users, "DisplayName", "DisplayName", model.selected);
            model.Name = project.Name;
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectUsers([Bind(Include = "selected, Id, Name, roles")] ProjectUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Declare variable of the application context
                //var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
                var context = this.db;

                //Create new helper to allow access to helper functions
                var helper = new ProjectUsersHelper();

                //List of users to be added
                //List<string> usersToAdd = new List<string>();
                List<ApplicationUser> usersToAdd = new List<ApplicationUser>();

                //List of users to be removed
                //List<string> usersToRemove = new List<string>();
                List<ApplicationUser> usersToRemove = new List<ApplicationUser>();

                //Check for selected to be null

                if (model.selected != null) //only addusers if selected is not null
                {
                    //Add selected users to be added to usersToAdd list
                    foreach (var item in model.selected)
                    {

                        usersToAdd.Add(db.Users.First(n => n.DisplayName == item));

                    }
                } 
                    var project = db.Projects.Find(model.Id); //new line

                    //Add all users to usersToRemove
                    // foreach (var item in project.Users)  //used to be db.Projects instead of project.Users
                    //    usersToRemove.Add(item.DisplayName);
                    usersToRemove = db.Users.ToList();


                //remove users to be added from usersToRemove list

                if (usersToAdd != null)
                {
                    foreach (var item in usersToAdd)
                    {
                        usersToRemove.Remove(db.Users.First(n => n.DisplayName == item.DisplayName));
                        //usersToRemove.Remove(item);
                    }

                    IEnumerable<ApplicationUser> userList = db.Users; //Create queryable list of all users

                    foreach (var item in usersToAdd)
                    {
                        //Find the user by display name
                        //ApplicationUser userToAdd = userList.FirstOrDefault(n => item == n.DisplayName);


                        if (!helper.IsUserOnProject(model.Id, item.Id)) //Check whether they're on the project
                            helper.AddUserToProject(model.Id, item.Id); //Add them if above condition is false
                    }
                }
                    //Remove user from project if the user was not selected but they are in the project
                    foreach (var item in usersToRemove)
                    {
                        //Find the user by display name
                        //ApplicationUser userToRemove = userList.FirstOrDefault(n => item == n.DisplayName);

                        if (helper.IsUserOnProject(model.Id, item.Id)) //Check whether they're on the project
                            helper.RemoveUserFromProject(model.Id, item.Id); //Remove them if above is true
                    }
                
                {

                }
                
                db.SaveChanges();

            }
            //return RedirectToAction("EditProjectUsers", new { model.Id });
            return RedirectToAction("Index");
        }



    }

}