﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class TicketsViewModel
    {

        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public IEnumerable<SelectListItem> Projects { get; set; }
        public string selected { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TicketTypeId { get; set; }
        public string TicketPriorityId { get; set; }
        public string TicketStatusId { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; }
        public string ProjectName { get; set; }

    }
}