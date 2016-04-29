using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker2.Models;
using Microsoft.AspNet.Identity;
using BugTracker2.Models.Helpers;

namespace BugTracker2.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            
            var userRolesHelper = new UserRolesHelper(db);
            var currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //var user = db.Users.Find(currentUserId);
            IList<string> currentUserRoles = userRolesHelper.ListUserRoles(currentUserId);

            bool isAdmin = false;
            //bool isPMorDeveloper = false;
            //bool isSubmitter = false;

            foreach(var item in currentUserRoles)
            {
                if (item == "Admin")
                    isAdmin = true;

                //if (item == "Project Manager" || item == "Developer")
                //    isPMorDeveloper = true;

                //if (item == "Submitter")
                //    isSubmitter = true;
            }

            List<Tickets> tickets = new List<Tickets>();
            //List<Projects> allProjects = db.Projects.ToList();
            //List<Tickets> allTickets = db.Tickets.ToList();



            if(isAdmin)
            {
                tickets = db.Tickets.ToList(); //If admin, list all tickets
            }
            else
            {
                var helper = new TicketUsersHelper();
                var ticketsToAdd = new List<Tickets>();
                IEnumerable<Tickets> allTickets = db.Tickets.ToList();

                foreach(var item in allTickets) //If not admin, cycle through all tickets and test the
                {                               //corresponding project's users against the current user
                    if (item.TicketProject != null)
                    {
                        foreach (var item2 in item.TicketProject.Users)
                            if (item2.Id == currentUserId)
                                ticketsToAdd.Add(item); tickets = db.Tickets.ToList();
                    }
                }
            }


            return View(tickets);
            
        }

        [Authorize]
        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {

            //CHECK FOR CURRENT USER'S PERMISSION ON THE TICKET?

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // GET: Tickets/Create
        [Authorize]
        public ActionResult CreateTicket()
        {
            var currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            
            ViewBag.ProjectId = new SelectList(currentUser.Projects, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTicket([Bind(Include = "Id, Title, Description, ProjectId, selected, TicketTypeId, TicketPriorityId, TicketStatusId")] TicketsViewModel ticket)
        {

            //CREATE VIEW NEEDS A LIST OF PROJECTS TO SELECT FROM
            if (ModelState.IsValid)
            {
                var currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

                var ticketToAdd = new Tickets();

                ticketToAdd.ProjectId = ticket.ProjectId.ToString();
                ticketToAdd.Created = DateTimeOffset.Now;
                ticketToAdd.Updated = DateTimeOffset.Now;
                ticketToAdd.OwnerUserId = currentUserId;
                ticketToAdd.Title = ticket.Title;
                ticketToAdd.Description = ticket.Description;
                ticketToAdd.TicketPriorityId = ticket.TicketPriorityId;
                ticketToAdd.TicketStatusId = ticket.TicketStatusId;
                ticketToAdd.TicketTypeId = ticket.TicketTypeId;

                ticketToAdd.TicketOwner = db.Users.Find(currentUserId);

                var projectIdInt = 0;
                Int32.TryParse(ticketToAdd.ProjectId, out projectIdInt); 
                    
                var ticketProject = db.Projects.Find(projectIdInt);
                ticketToAdd.TicketProject = ticketProject;

                db.Tickets.Add(ticketToAdd);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles ="Admin, Project Manager, Submitter")]
        public ActionResult EditTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var id2 = id.GetValueOrDefault();

            var helper = new TicketUsersHelper();
            var currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //If user is a submitter
            if (this.User.IsInRole("Submitter") && helper.IsUserOnTicket(id2, currentUserId));
            {

            }

            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTicket([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tickets).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tickets);
        }

        // GET: Tickets/Delete/5
        public ActionResult DeleteTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets tickets = db.Tickets.Find(id);
            db.Tickets.Remove(tickets);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
