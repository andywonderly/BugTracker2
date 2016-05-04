using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class Ticket
    {
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public string ProjectId { get; set; }
        public string TicketTypeId { get; set; }
        public string TicketPriorityId { get; set; }
        public string TicketStatusId { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; }
        public string TicketAssigneeId { get; set; }
        //public virtual ICollection<ApplicationUser> Users { get; set; }
        //This was reduntant.  List of ticket users not needed.  Just use project users
        //of the project that the ticket is for.
        public virtual ApplicationUser TicketOwner { get; set; }
        //public virtual ApplicationUser TicketAssignee { get; set; }
        public virtual Projects TicketProject { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        //[ForeignKey("Tickets_Id")]
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual ICollection<TicketPriority> TicketPriorities { get; set; }
        public virtual ICollection<TicketStatus> TicketStatuses { get; set; }
        public virtual ICollection<TicketType> TicketTypes { get; set; }
        public virtual ICollection<TicketAssignee> TicketAssignees { get; set; }
        public string TicketOwnerId { get; set; }
    }
}