using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models.Helpers
{
    public class TicketUsersHelper
    {

        ApplicationDbContext db = new ApplicationDbContext();

        public void AddUserToTicket(int ticketId, string userId)
        {

            ApplicationUser user = db.Users.Find(userId);
            Tickets ticket = db.Tickets.First(p => p.Id == ticketId);
            IEnumerable<ApplicationUser> ticketUsers = ticket.TicketProject.Users.ToList();
            bool userIsOnTicket = ticketUsers.Any(n => n.Id == user.Id);
            TicketUsersHelper ticketUsersHelper = new TicketUsersHelper();

            if (!ticketUsersHelper.IsUserOnTicket(ticket.Id, user.Id)) //only add user if they are not already on the ticket
            {
                ticket.TicketProject.Users.Add(user);
                db.SaveChanges();
            }
            
        }

        public void RemoveUserToTicket(int ticketId, string userId)
        {

            ApplicationUser user = db.Users.Find(userId);
            Tickets ticket = db.Tickets.First(p => p.Id == ticketId);
            IEnumerable<ApplicationUser> ticketUsers = ticket.TicketProject.Users.ToList();
            bool userIsOnTicket = ticketUsers.Any(n => n.Id == user.Id);
            TicketUsersHelper ticketUsersHelper = new TicketUsersHelper();

            if (ticketUsersHelper.IsUserOnTicket(ticket.Id, user.Id)) //only remove user if they are not already on the ticket
            {
                ticket.TicketProject.Users.Remove(user);
                db.SaveChanges();
            }

        }

        public IList<string> ListTicketUsers(int ticketId)
        {
            Tickets ticket = db.Tickets.First(p => p.Id == ticketId);
            IList<string> ticketUserList = new List<string>();

            //ticketUserList = ticket.Users.Where(x => x.Id == )

            foreach (var item in ticket.TicketProject.Users)
                ticketUserList.Add(item.DisplayName);
            
            return ticketUserList;
        }

        public IList<string> ListNonTicketUsers(int ticketId)
        {
            Tickets ticket = db.Tickets.FirstOrDefault(p => p.Id == ticketId);
            List<ApplicationUser> userList = db.Users.ToList(); //list of all users
            IList<string> nonUserDisplayNames = new List<string>();

            foreach (var item in ticket.TicketProject.Users) //remove ticket users from all users to get non-ticket users
                userList.Remove(item);

            foreach (var item in userList) //add non-ticket user display names to nonUserDisplayNames
                nonUserDisplayNames.Add(item.DisplayName);

            return nonUserDisplayNames;
        }

        public List<Tickets> ListUserTickets(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            IEnumerable<Tickets> tickets = db.Tickets.Where(x => x.TicketProject.Users == user);
            List<Tickets> ticketsList = new List<Tickets>();

            if (tickets != null)
                ticketsList = tickets.ToList();
            return ticketsList;
        }

        public bool IsUserOnTicket(int ticketId, string userId)
        {
            var ticket = db.Tickets.FirstOrDefault(p => p.Id == ticketId);
            var flag = ticket.TicketProject.Users.Any(u => u.Id == userId.ToString());
            return (flag);
        }


    }


}