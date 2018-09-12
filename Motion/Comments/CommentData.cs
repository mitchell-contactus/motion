using System;
using System.Collections.Generic;
using Motion.Database;
using Motion.Sessions;
using Motion.Tickets;

namespace Motion.Comments
{
    public class CommentData : DataBase
    {
        const string GetCommentsForTicketQuery = 
        @"SELECT
        (CONVERT_TZ( (created_at), 'UTC','{2}')) as created_at,
        created_by,
        name,
        entry,
        text_entry,
        source,
        type 
        FROM 
        {0}.tt_ticket_comments comments
        INNER JOIN
        {0}.auth_local auth
        ON
        created_by = auth.id
        WHERE 
        ticket_id={1}
        {3}
        ORDER BY 
        comments.id desc";
        public List<Comment> GetCommentsForTicket(Session session, int ticketId, TicketPermissions permissions)
        {
            string filter = permissions.CanViewInternalComments ? "" : "AND type = " + COMMENT_TYPE.PUBLIC;
            List<Comment> comments = new List<Comment>();
            using (var select = Select(GetCommentsForTicketQuery, Config.Get("mysql_db"), ticketId, "EST", filter))
                while (select.Read())
                {
                    comments.Add(new Comment()
                    {
                        CreatedDate = select.IsDBNull(0) ? null : select.GetString(0),
                        UserId = select.GetInt32(1),
                        UserName = select.IsDBNull(2) ? null : select.GetString(2),
                        Entry = select.IsDBNull(3) ? null : select.GetString(3),
                        TextEntry = select.IsDBNull(4) ? null : select.GetString(4),
                        Source = select.GetInt32(5),
                        Type = (COMMENT_TYPE) select.GetInt32(6)
                    });
                }
            return comments;
        }

        const string AddCommentQuery = 
        @"INSERT INTO 
        {0}.tt_ticket_comments (
            ticket_id,
            account_id,
            created_at,
            created_by,
            entry,
            text_entry,
            source,
            type
        ) VALUES (
            {1},
            {2},
            now(),
            {3},
            '{4}',
            '{5}',
            {6},
            {7}
        );
        UPDATE
        {0}.tt_tickets 
        SET
        updated_at=now(),
        last_action_activity_by={3},
        last_action_activity_at=now()
        WHERE 
        id={1};";
        public void AddComment(Session session, int ticketId, TicketData ticketData,
                               string sourceHTML, string sourceText, COMMENT_TYPE type, COMMENT_SOURCE source)
        {
            Insert(AddCommentQuery, Config.Get("mysql_db"), ticketId, session.AccountId, session.UserId,
                   E(sourceHTML), E(sourceText), (int)source, (int)type);

            switch (type)
            {
                case COMMENT_TYPE.INTERNAL:
                    ticketData.LogEvent(session, ticketId, TICKET_EVENT.COMMENTED_INTERNAL, "");
                    break;
                case COMMENT_TYPE.PUBLIC:
                    ticketData.LogEvent(session, ticketId, TICKET_EVENT.COMMENTED_EXTERNAL, "");
                    break;
            }
        }
    }
}
