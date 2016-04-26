using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class Projects
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public MultiSelectList users { get; set; }
        //public string[] selected { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}