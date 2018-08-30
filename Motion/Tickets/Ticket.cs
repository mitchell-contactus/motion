using System;
using System.Collections.Generic;
using Motion.Comments;

namespace Motion.Tickets
{
    public class Ticket
    {
        public int ID { get; }
        public string CreatedDate { get; internal set; }
        public string ClosedDate { get; internal set; }
        public string DueDate { get; internal set; }
        public string FirstTouchDate { get; internal set; }
        public int Status { get; internal set; }
        public int? OpenedById { get; internal set; }
        public string OpenedByUsername { get; internal set; }
        public string OpenedByName { get; internal set; }
        public int? ClosedById { get; internal set; }
        public string ClosedByUsername { get; internal set; }
        public string ClosedByName { get; internal set; }
        public string Subject { get; internal set; }
        public int AccountId { get; internal set; }
        public int FormId { get; internal set; }
        public string FormName { get; internal set; }
        public string UpdatedDate { get; internal set; }
        public int? AssignedId { get; internal set; }
        public string AssignedUsername { get; internal set; }
        public string AssignedName { get; internal set; }
        public string ClientGuid { get; internal set; }
        public int? ContactId { get; internal set; }
        public string ContactUsername { get; internal set; }
        public string ContactName { get; internal set; }
        public int? ReasonId { get; internal set; }
        public int? ParentId { get; internal set; }
        public int? Priority { get; internal set; }
        public int? LockedById { get; internal set; }
        public string SourceType { get; internal set; }

        public List<TicketEvent> History { get; set; }
        public TicketPermissions Permissions { get; set; }
        public List<Comment> Comments { get; set; }

        public Ticket(int ID)
        {
            this.ID = ID;
        }
    }
}
