using System;
using System.Collections.Generic;
using Motion.Comments;
using Motion.Database;
using Motion.Forms;
using Motion.Sessions;

namespace Motion.Tickets
{
    public class TicketData : DataBase
    {
        readonly FormData formData = new FormData();
        readonly CommentData commentData = new CommentData();

        const string GetTicketQuery = 
        @"SELECT
        tickets.id,
        (CONVERT_TZ( (created_at), 'UTC','{3}')) as created_at,
        (CONVERT_TZ( (closed_at), 'UTC','{3}')) as closed_at,
        (CONVERT_TZ( (due_at), 'UTC','{3}')) as due_at,
        due_at as due_at_utc, 
        first_touch_at, 
        status, 
        opened_by,
        opened.username,
        opened.name,
        closed_by,
        closed.username,
        closed.name,
        subject, 
        tickets.account_id, 
        form_id,
        forms.name,
        (CONVERT_TZ( (updated_at), 'UTC','{3}')) as updated_at,
        assigned_to, 
        client_guid, 
        contact_id,
        contact.username,
        contact.name,
        reason_id, 
        parent_id, 
        priority, 
        locked_by, 
        source_type
        FROM
        {0}.tt_tickets tickets
        LEFT JOIN
        {0}.auth_local opened
        ON
        opened_by = opened.id
        LEFT JOIN
        {0}.auth_local closed
        ON
        closed_by = closed.id
        LEFT JOIN
        {0}.auth_local contact
        ON
        contact_id = contact.id
        LEFT JOIN
        {0}.tt_ticket_forms forms
        ON
        form_id = forms.id
        WHERE
        tickets.id = {2}
        AND
        tickets.account_id = {1}";
        public Ticket GetTicket(Session session, int ticketId)
        {
            Ticket ticket = null;
            using (var select = Select(GetTicketQuery, Config.Get("mysql_db"), session.AccountId, ticketId, "EST"))
            {
                if (select.Read())
                {
                    ticket = new Ticket(select.GetInt32(0))
                    {
                        CreatedDate = select.IsDBNull(1) ? null : select.GetString(1),
                        ClosedDate = select.IsDBNull(2) ? null : select.GetString(2),
                        DueDate = select.IsDBNull(3) ? null : select.GetString(3),

                        FirstTouchDate = select.IsDBNull(5) ? null : select.GetString(5),
                        Status = select.GetInt32(6),
                        OpenedById = select.IsDBNull(7) ? null : new int?(select.GetInt32(7)),
                        OpenedByUsername = select.IsDBNull(8) ? null : select.GetString(8),
                        OpenedByName = select.IsDBNull(9) ? null : select.GetString(9),
                        ClosedById = select.IsDBNull(10) ? null : new int?(select.GetInt32(10)),
                        ClosedByUsername = select.IsDBNull(11) ? null : select.GetString(11),
                        ClosedByName = select.IsDBNull(12) ? null : select.GetString(12),
                        Subject = select.IsDBNull(13) ? null : select.GetString(13),
                        AccountId = select.GetInt32(14),
                        FormId = select.GetInt32(15),
                        FormName = select.IsDBNull(16) ? null : select.GetString(16),
                        UpdatedDate = select.IsDBNull(17) ? null : select.GetString(17),
                        AssignedId = select.IsDBNull(18) ? null : new int?(select.GetInt32(18)),
                        ClientGuid = select.IsDBNull(19) ? null : select.GetString(19),
                        ContactId = select.IsDBNull(20) ? null : new int?(select.GetInt32(20)),
                        ContactUsername = select.IsDBNull(21) ? null : select.GetString(21),
                        ContactName = select.IsDBNull(22) ? null : select.GetString(22),
                        ReasonId = select.IsDBNull(23) ? null : new int?(select.GetInt32(23)),
                        ParentId = select.IsDBNull(24) ? null : new int?(select.GetInt32(24)),
                        Priority = select.IsDBNull(25) ? null : new int?(select.GetInt32(25)),
                        LockedById = select.IsDBNull(26) ? null : new int?(select.GetInt32(26)),
                        SourceType = select.IsDBNull(27) ? null : select.GetString(27)
                    };
                }
            }
            if (ticket == null)
            {
                return null;
            }

            ticket.History = GetTicketHistory(session, ticketId);
            ticket.Comments = commentData.GetCommentsForTicket(session, ticketId);

            ticket.Permissions = new TicketPermissions(formData.GetPermissionsForForm(session, ticket.FormId));
            if (ticket.AssignedId == session.UserId)
            {
                ticket.Permissions.CanView = true;
                ticket.Permissions.CanComment = true;
            }
            foreach (var e in ticket.History) {
                if (e.UserId == session.UserId)
                {
                    ticket.Permissions.CanView = true;
                    ticket.Permissions.CanComment = true;
                    break;
                }
            }

            return ticket;
        }

        const string GetTicketHistoryQuery =
        @"SELECT
        event_by,
        name,
        username,
        event_type,
        (CONVERT_TZ( (event_at), 'UTC','EST')) as event_at
        FROM
        {0}.tt_ticket_history history
        LEFT JOIN
        {0}.auth_local auth
        ON
        history.event_by = auth.id
        WHERE
        ticket_id = {2}
        AND
        account_id = {1}
        ORDER BY
        history.id";
        /// <summary>
        ///  Does not check if the session has view priviledges on the ticket
        /// </summary>
        public List<TicketEvent> GetTicketHistory(Session session, int ticketId)
        {
            List<TicketEvent> history = new List<TicketEvent>();
            using (var select = Select(GetTicketHistoryQuery, Config.Get("mysql_db"), session.AccountId, ticketId))
                while(select.Read())
            {
                history.Add(new TicketEvent()
                {
                    UserId = select.GetInt32(0),
                    Name = select.IsDBNull(1) ? null : select.GetString(1),
                    Username = select.GetString(2),
                    EventType = select.GetInt32(3),
                    Timestamp = select.GetString(4)
                });
            }
            return history;
        }
    } 
}
