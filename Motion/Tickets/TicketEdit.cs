using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Motion.Database;
using Motion.Forms;
using Motion.Rest;
using Motion.Sessions;
using Motion.Users;

namespace Motion.Tickets
{
    public class TicketEdit
    {
        readonly FormData formData = new FormData();
        readonly UserData userData = new UserData();

        public List<string> UpdateQueries { get; }
        public List<KeyValuePair<TICKET_EVENT, string>> Events { get; }

        public TicketEdit(Session session, NameValueCollection data, Ticket ticket)
        {
            UpdateQueries = new List<string>();
            Events = new List<KeyValuePair<TICKET_EVENT, string>>();

            if (data.AllKeys.Contains("Subject"))
            {
                UpdateQueries.Add("subject = \"" + DataBase.E(data["Subject"]) + "\"");
                AddEvent(TICKET_EVENT.EDITED, 
                         String.Format("Changed Subject From\"{0}\" to \"{1}\"", 
                                       ticket.Subject, DataBase.E(data["Subject"])));
            }

            if (data.AllKeys.Contains("FormId"))
            {
                int formId = ParseInt(data["FormId"]);
                var permissions = formData.GetPermissionsForForm(session, formId);
                if (permissions == null || !permissions.CanView)
                {
                    throw new RequestException("Form permissions incorrect");
                }
                UpdateQueries.Add("form_id = " + formId);
                AddEvent(TICKET_EVENT.CHANGED_QUEUE, ticket.FormName + "=>" + formData.GetForm(session, formId).Name);
                // TODO: Send email to watchers
            }

            if (data.AllKeys.Contains("Priority"))
            {
                UpdateQueries.Add("priority = " + ParseInt(data["Priority"]));
                AddEvent(TICKET_EVENT.EDITED, 
                         "Priority From " + (ticket.Priority > -1 ? ticket.Priority.ToString() : " Not Set") + " => " + ParseInt(data["Priority"]));
            }

            if (data.AllKeys.Contains("Status"))
            {
                TICKET_STATUS status = (TICKET_STATUS)ParseInt(data["Status"]);
                UpdateQueries.Add("status = " + ParseInt(data["Status"]));

                TICKET_EVENT eventType;
                switch (status)
                {
                    case TICKET_STATUS.OPEN:
                        eventType = TICKET_EVENT.REOPENED;
                        // TODO: Send email to watchers
                        break;
                    case TICKET_STATUS.CLOSED:
                        eventType = TICKET_EVENT.CLOSED;
                        UpdateQueries.Add("closed_by=" + session.UserId + ", closed_at=now()");
                        if (ticket.AssignedId == null)
                        {
                            UpdateQueries.Add("assigned_to=" + session.UserId + ",");
                        }
                        // TODO: Send email to watchers
                        break;
                    default:
                        eventType = TICKET_EVENT.STATUS_CHANGE;
                        // TODO: Send email to watchers
                        break;
                }
                AddEvent(eventType,
                                 "From " + Enum.GetName(typeof(TICKET_STATUS), ticket.Status) +
                                 " => " + Enum.GetName(typeof(TICKET_STATUS), status));
            }

            if (data.AllKeys.Contains("AssignedId"))
            {
                UpdateQueries.Add("assigned_to = " + ParseInt(data["AssignedId"]));
                AddEvent(TICKET_EVENT.ASSIGNED,
                         "From " + (ticket.AssignedId != null ? ticket.AssignedName : "Not Previously Assigned") + 
                         " => " + userData.GetUser(session, ParseInt(data["AssignedId"])));
                // TODO: Send email to watchers
            }
        }

        private void AddEvent(TICKET_EVENT e, string detail)
        {
            Events.Add(new KeyValuePair<TICKET_EVENT, string>(e, detail));
        }

        private int ParseInt(string i)
        {
            try
            {
                return Convert.ToInt32(i);
            }
            catch (Exception)
            {
                throw new RequestException("Error parsing int");
            }
        }
    }
}
