using System;
using System.Linq;
using System.Net;
using Grapevine;
using Grapevine.Server;
using Motion.Forms;
using Motion.Rest;

namespace Motion.Tickets
{
    public sealed class TicketRestHandler : RestBase
    {

        readonly FormData formData = new FormData();
        readonly TicketData ticketData = new TicketData();

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
    }
}
