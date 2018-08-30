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
        ORDER BY 
        comments.id desc";
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
                        UserName = select.IsDBNull(2) ? null : select.GetString(2),
                        Entry = select.IsDBNull(3) ? null : select.GetString(3),
                        TextEntry = select.IsDBNull(4) ? null : select.GetString(4),
                        Source = select.GetInt32(5),
                        Type = select.GetInt32(6)
                    });
                }
            return comments;
        }
    }
}
