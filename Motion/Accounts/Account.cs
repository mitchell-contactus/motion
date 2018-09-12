using System;

namespace Motion.Accounts
{
    public class Account
    {
        public int ID { get; }
        public string Name { get; internal set; }
        public Guid? Guid { get; internal set; }

        public string ADHost { get; internal set; }
        public string ADDomain { get; internal set; }
        public string ADLoginDomainPrepend { get; internal set; }
        public string TicketMailFromName { get; internal set; }
        public string TicketMailFromDomain { get; internal set; }
        public string TicketHostPrepend { get; internal set; }

        public Account(int id)
        {
            ID = id;
        }
    }
}
