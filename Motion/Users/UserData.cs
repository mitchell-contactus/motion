using System;
using System.Collections.Generic;
using Motion.Database;
using Motion.Sessions;

namespace Motion.Users
{
    public class UserData : DataBase
    {
        const string AuthUserQuery =
        @"SELECT
        id,
        username,
        name,
        email_address,
        is_global_admin,
        active
        FROM
        {0}.auth_local
        WHERE
        UPPER(username)=UPPER('{1}')
        AND
        UPPER(password)=UPPER('{2}')";
        public User AuthUser(string username, string password) {
            var hash = GetSha1(String.Format("{0}-{1}-{2}", username.ToLower(), password, "CONTACTUS"));

            using (var select = Select(AuthUserQuery, Config.Get("mysql_db"), E(username), hash))
            {
                if (select.Read())
                {
                    return new User(select.GetInt32(0), this)
                    {
                        Username = select.IsDBNull(1) ? "" : select.GetString(1),
                        Name = select.IsDBNull(2) ? null : select.GetString(2),
                        Email = select.IsDBNull(3) ? "" : select.GetString(3),
                        IsGlobalAdmin = select.IsDBNull(4) ? false : select.GetInt16(4) == 1,
                        IsActive = select.IsDBNull(5) ? false : select.GetInt16(5) == 1
                    };
                }
            }
            return null;
        }

        const string GetAccountsForUserQuery =
        @"SELECT 
        auth_customer.account_id, 
        accounts.name 
        FROM 
        {0}.auth_customer 
        INNER JOIN 
        {0}.accounts 
        ON 
        accounts.id=auth_customer.account_id 
        WHERE 
        auth_customer.auth_local_id={1} 
        ORDER BY 
        name ASC";
        public Dictionary<int, string> GetAccountsForUser(User user) {
            if (user.IsGlobalAdmin)
            {
                return GetAccounts();
            }

            Dictionary<int, string> list = new Dictionary<int, string>();
            using (var select = Select(GetAccountsForUserQuery, Config.Get("mysql_db"), user.ID))
                while (select.Read())
            {
                list.Add(select.GetInt32(0), select.IsDBNull(1) ? null : select.GetString(1));
            }
            return list;
        }

        const string GetAccountsQuery =
        @"SELECT 
        id,
        name 
        FROM 
        {0}.accounts 
        ORDER BY 
        name ASC";
        public Dictionary<int, string> GetAccounts() {
            Dictionary<int, string> list = new Dictionary<int, string>();
            using (var select = Select(GetAccountsQuery, Config.Get("mysql_db")))
                while (select.Read())
            {
                list.Add(select.GetInt32(0), select.IsDBNull(1) ? null : select.GetString(1));
            }
            return list;
        }

        const string GetUsersQuery = 
        @"SELECT
        id,
        username,
        name,
        email_address,
        is_global_admin,
        active
        FROM 
        {0}.auth_local
        LEFT JOIN 
        {0}.auth_customer
        ON
        auth_customer.auth_local_id=auth_local.id
        WHERE
        active=true
        AND
        (
            account_id={1}
            OR 
            is_global_admin=1
        )
        GROUP BY
        id
        ORDER BY 
        name REGEXP '^[a-z]' DESC, name";
        public List<User> GetUsers(Session session)
        {
            List<User> list = new List<User>();
            using (var select = Select(GetUsersQuery, Config.Get("mysql_db"), session.AccountId))
                while (select.Read())
                {
                    list.Add(new User(select.GetInt32(0), this)
                    {
                        Username = select.IsDBNull(1) ? "" : select.GetString(1),
                        Name = select.IsDBNull(2) ? null : select.GetString(2),
                        Email = select.IsDBNull(3) ? "" : select.GetString(3),
                        IsGlobalAdmin = select.IsDBNull(4) ? false : select.GetInt16(4) == 1,
                        IsActive = select.IsDBNull(5) ? false : select.GetInt16(5) == 1
                    });
                }
            return list;
        }

        const string GetUserQuery =
        @"SELECT
        id,
        username,
        name,
        email_address,
        is_global_admin,
        active
        FROM 
        {0}.auth_local
        LEFT JOIN 
        {0}.auth_customer
        ON
        auth_customer.auth_local_id=auth_local.id
        WHERE
        active=true
        AND
        (
            account_id={1}
            OR 
            is_global_admin=1
        )
        AND
        id = {2}
        GROUP BY
        id
        ORDER BY 
        name REGEXP '^[a-z]' DESC, name";
        public User GetUser(Session session, int userId)
        {
            using (var select = Select(GetUserQuery, Config.Get("mysql_db"), session.AccountId, userId))
            {
                if (select.Read())
                {
                    return new User(select.GetInt32(0), this)
                    {
                        Username = select.IsDBNull(1) ? "" : select.GetString(1),
                        Name = select.IsDBNull(2) ? null : select.GetString(2),
                        Email = select.IsDBNull(3) ? "" : select.GetString(3),
                        IsGlobalAdmin = select.IsDBNull(4) ? false : select.GetInt16(4) == 1,
                        IsActive = select.IsDBNull(5) ? false : select.GetInt16(5) == 1
                    };
                }
            }
            return null;
        }
    }
}
