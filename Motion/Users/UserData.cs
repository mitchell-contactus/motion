using System;
using System.Collections.Generic;
using Motion.AD;
using Motion.Contacts;
using Motion.Database;
using Motion.Sessions;

namespace Motion.Users
{
    public class UserData : DataBase
    {
        readonly ContactData contactData = new ContactData();

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
        {2}
        GROUP BY
        id
        ORDER BY 
        name REGEXP '^[a-z]' DESC, name";
        public User GetUser(int accountId, UserQuery query)
        {
            var queryString = String.Join(" AND ", query.GenerateQueries());
            using (var select = Select(GetUserQuery, Config.Get("mysql_db"), accountId, queryString))
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

        const string CreateUserQuery = 
        @"INSERT INTO 
        {0}.auth_local 
        (
            username,
            email_address,
            name
        ) 
        VALUES
        ('{1}','{1}','{2}')";
        const string CreateUser2Query = 
        @"INSERT INTO 
        {0}.auth_customer 
        (
            auth_local_id,
            account_id
        ) 
        VALUES
        ({1},{2})";
        public User CreateUser(string emailAddress, string firstName, string lastName, int accountId)
        {
            int? authLocalId = InsertReturnId(CreateUserQuery,
                                              Config.Get("mysql_db"),
                                              E(emailAddress),
                                              E(String.Format("{0} {1}",
                                                              firstName ?? "",
                                                              lastName ?? "")));
            if (authLocalId != null)
            {
                Insert(CreateUser2Query , Config.Get("mysql_db"), authLocalId, accountId);
                MigrateContactToUser(accountId, emailAddress, authLocalId);
                return GetUser(accountId, new UserQuery { ID = authLocalId });
            }
            return null;
        }

        const string MigrateContactToUserQuery = 
        @"UPDATE {0}.tt_tickets 
        SET opened_by = {1} 
        WHERE opened_by = {2};
        UPDATE {0}.tt_tickets 
        SET closed_by = {1} 
        WHERE closed_by = {2};
        UPDATE {0}.tt_tickets 
        SET assigned_to = {1} 
        WHERE assigned_to = {2};
        UPDATE {0}.tt_tickets 
        SET contact_id = {1} 
        WHERE contact_id = {2}
        UPDATE {0}.tt_ticket_comments 
        SET created_by = {1} 
        WHERE created_by = {2};
        DELETE FROM {0}.tt_contacts 
        WHERE id= {2};";
        public void MigrateContactToUser(int accountId, string emailAddress, int? userId)
        {
            var contacts = contactData.getContacts(accountId, new ContactQuery { Email = emailAddress });
            foreach (var contact in contacts)
            {
                Update(MigrateContactToUserQuery, Config.Get("mysql_db"), userId, contact.ID);
            }
        }

        const string UpdateADGroupsForUserQuery = 
        @"DELETE FROM 
        {0}.auth_user_ad_groups_cache 
        WHERE 
        user_id={1}";
        const string UpdateADGroupsForUser2Query =
        @"INSERT INTO 
        {0}.auth_user_ad_groups_cache 
        (
            user_id,
            name
        ) 
        VALUES
        ({1},'{2}');";
        public void UpdateADGroupsForUser(int userId, ADInfo info)
        {
            //TODO: optimize this insert
            Update(UpdateADGroupsForUserQuery, Config.Get("mysql_db"), userId);
            foreach(var group in info.Groups)
            {
                Insert(UpdateADGroupsForUser2Query, Config.Get("mysql_db"), userId, E(group));
            }
        }
    }
}
