using System;
using Motion.Database;
using Motion.Users;

namespace Motion.Sessions
{
    public class SessionData : DataBase
    {
        const string CreateSessionQuery =
        @"INSERT INTO 
        auth_sessions 
        (client_id,
        session,
        created_at,
        user_id,
        account_id) 
        VALUES (
        '{0}',
        '{1}',
        now(),
        '{2}',
        '{3}')";
        public string CreateSession(User user, int accountId)
        {
            string sessionId = String.Format("{0}", Guid.NewGuid().ToString("N"));
            try
            {
                Insert(CreateSessionQuery, E(user.Username), sessionId, user.ID, accountId);
                return sessionId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        const string GetSessionQuery =
        @"SELECT
        session,
        user_id,
        account_id
        FROM
        {0}.auth_sessions
        WHERE
        session = '{1}'";
        public Session GetSession(string sessionId) {
            using (var select = Select(GetSessionQuery, Config.Get("mysql_db"), E(sessionId)))
            {
                if (select.Read())
                {
                    return new Session(select.GetString(0), select.GetInt32(1), select.GetInt32(2));
                }
            }
            return null;
        }
    }
}
