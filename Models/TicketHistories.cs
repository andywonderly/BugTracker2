﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketHistories
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Changed { get; set; }
        public int UserId { get; set; }
    }
}