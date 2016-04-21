using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketNotifications
    {
        public int Id { get; set; }
        public int TicketID { get; set; }
        public int UserId { get; set; }
    }
}