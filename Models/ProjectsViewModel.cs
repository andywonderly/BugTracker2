using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class ProjectsViewModel
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string UserId { get; set; }
        public MultiSelectList Users { get; set; }
        public string[] selected { get; set; }
    }


}