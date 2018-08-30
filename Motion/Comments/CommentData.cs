using System;
using System.Collections.Generic;
using Motion.Database;
using Motion.Sessions;

namespace Motion.Comments
{
    public class CommentData : DataBase
    {
        const string GetCommentsForTicketQuery = 
        @"SELECT
        (CONVERT_TZ( (created_at), 'UTC','{2}')) as created_at,
        created_by,
        entry,
        text_entry,
        source,
        type 
        FROM 
        {0}.tt_ticket_comments 
        WHERE 
        ticket_id={1} 
        ORDER BY 
        id desc";
        public List<Comment> GetCommentsForTicket(Session session, int ticketId)
        {
            List<Comment> comments = new List<Comment>();
            using (var select = Select(GetCommentsForTicketQuery, Config.Get("mysql_db"), ticketId, "EST"))
                while (select.Read())
                {
                    comments.Add(new Comment()
                    {
                        CreatedDate = select.IsDBNull(0) ? null : select.GetString(0),
                        UserId = select.GetInt32(1),
                        Entry = select.IsDBNull(2) ? null : select.GetString(2),
                        TextEntry = select.IsDBNull(3) ? null : select.GetString(3),
                        Source = select.GetInt32(4),
                        Type = select.GetInt32(5)
                    });
                }
            return comments;
        }
    }
}
