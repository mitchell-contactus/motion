using System;
namespace Motion.Tickets
{
    public class TicketEvent
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public TICKET_EVENT EventType { get; set; }
        public string Timestamp { get; set; }
        public string Detail { get; set; }

    }

    public enum TICKET_EVENT
    {
        ERROR_CREATING_TICKET = -1,
        CREATED = 1,
        VIEWED = 2,
        COMMENTED_INTERNAL = 3,
        CLOSED = 4,
        CHANGED_QUEUE = 5,
        EDITED = 6,
        PREVIEW = 7,
        SECURITY_PREVENTED = 8,
        ASSIGNED = 9,
        COMMENTED_EXTERNAL = 10,
        REOPENED = 11,
        STATUS_CHANGE = 12, // General status hange other than Open, reopened, or closed
        ADDED_PARTICIPANT = 13,
        REMOVED_PARTICIPANT = 14,
        CREATED_SUBTASK = 15

    };
}
