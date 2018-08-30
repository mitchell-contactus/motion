using System;
namespace Motion.Tickets
{
    public class TicketEvent
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public int EventType { get; set; }
        public string Timestamp { get; set; }

    }
}
