using System.Net;
using Grapevine;
using Grapevine.Server;
using Motion.Rest;

namespace Motion.Tickets
{
    public sealed class TicketRestHandler : RestBase
    {

        readonly TicketData ticketData = new TicketData();

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/tickets/list")]
        public void ListTickets(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);
                var tickets = ticketData.ListTickets(session, new TicketFilter(data));
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
    }
}
