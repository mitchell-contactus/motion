using System;
using System.Linq;
using System.Net;
using Grapevine;
using Grapevine.Server;
using Motion.Comments;
using Motion.Forms;
using Motion.Rest;

namespace Motion.Tickets
{
    public sealed class TicketRestHandler : RestBase
    {

        readonly FormData formData = new FormData();
        readonly TicketData ticketData = new TicketData();
        readonly CommentData commentData = new CommentData();

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/tickets/list")]
        public void ListTickets(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);
                var tickets = formData.ListTickets(session, new TicketFilter(data));
                SendJsonResponse(context, tickets);
            }
            catch (RequestException e)
            {
                SendUnexpectedError(context, e.Reason);
            }
            catch (InputException e)
            {
                SendMissingParameter(context, e.Reason);
            }
        }

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/tickets/view")]
        public void ViewTicket(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);

                if (!data.AllKeys.Contains("ticketId"))
                {
                    throw new InputException("ticketId");
                }
                int ticketId = Convert.ToInt32(data["ticketId"]);

                var ticket = ticketData.GetTicket(session, ticketId);
                if (ticket == null || !ticket.Permissions.CanView)
                {
                    throw new RequestException("Not authorized to view ticket");
                }
                SendJsonResponse(context, ticket);
            }
            catch (RequestException e)
            {
                SendUnexpectedError(context, e.Reason);
            }
            catch (InputException e)
            {
                SendMissingParameter(context, e.Reason);
            }
        }

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/tickets/edit")]
        public void EditTicket(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);

                if (!data.AllKeys.Contains("ticketId"))
                {
                    throw new InputException("ticketId");
                }
                int ticketId = Convert.ToInt32(data["ticketId"]);

                var ticket = ticketData.GetTicket(session, ticketId);
                if (ticket == null || !ticket.Permissions.CanEdit)
                {
                    ticketData.LogEvent(session, ticketId, TICKET_EVENT.SECURITY_PREVENTED, null);
                    throw new RequestException("Not authorized to edit ticket");
                }

                TicketEdit edit = new TicketEdit(session, data, ticket);
                ticketData.ApplyEdit(session, ticketId, edit);
                SendTextResponse(context, "1");
            }
            catch (RequestException e)
            {
                SendUnexpectedError(context, e.Reason);
            }
            catch (InputException e)
            {
                SendMissingParameter(context, e.Reason);
            }
        }

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/tickets/comment")]
        public void AddComment(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);

                if (!data.AllKeys.Contains("ticketId"))
                {
                    throw new InputException("ticketId");
                }
                int ticketId = Convert.ToInt32(data["ticketId"]);

                var ticket = ticketData.GetTicket(session, ticketId);
                if (ticket == null || !ticket.Permissions.CanComment)
                {
                    ticketData.LogEvent(session, ticketId, TICKET_EVENT.SECURITY_PREVENTED, null);
                    throw new RequestException("Not authorized to comment on ticket");
                }

                if (!data.AllKeys.Contains("comment"))
                {
                    throw new InputException("comment");
                }
                string comment = data["comment"];

                if (!data.AllKeys.Contains("type"))
                {
                    throw new InputException("type");
                }
                COMMENT_TYPE type = (COMMENT_TYPE)Convert.ToInt32(data["type"]);

                if (type == COMMENT_TYPE.INTERNAL && !ticket.Permissions.CanViewInternalComments)
                {
                    ticketData.LogEvent(session, ticketId, TICKET_EVENT.SECURITY_PREVENTED, null);
                    throw new RequestException("Not authorized to comment internally on ticket");
                }

                commentData.AddComment(session, ticketId, ticketData, comment, comment, type, COMMENT_SOURCE.Web);
                //TODO: Send email
                //TODO: If created by SMS AND public, send SMS

                if (data.AllKeys.Contains("subtask_assigned") && data["subtask_assigned"] != "nulL")
                {
                    int subtaskUserId = Convert.ToInt32(data["subtask_assigned"]);
                    //TODO: Create new subtask
                }

                SendTextResponse(context, "1");
            }
            catch (RequestException e)
            {
                SendUnexpectedError(context, e.Reason);
            }
            catch (InputException e)
            {
                SendMissingParameter(context, e.Reason);
            }
        }
    }
}
