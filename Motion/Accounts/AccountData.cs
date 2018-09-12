using System;
using System.Collections.Generic;
using Motion.Database;
using Motion.Users;

namespace Motion.Accounts
{
    public class AccountData : DataBase
    {
        const string GetAccountByADQuery = 
        @"SELECT
        id,
        name,
        guid,
        ad_host,
        ad_domain,
        ad_login_dn_prepend,
        ticket_mail_from_name,
        ticket_mail_from_domain,
        ticket_host_prefix 
        FROM 
        {0}.accounts
        WHERE
        id in ( 
            SELECT
            account_id 
            FROM 
            {0}.account_domains 
            WHERE
            domain='{1}'
        )";
        public Account GetAccountByAD(string domain)
        {
            using (var select = Select(GetAccountByADQuery, Config.Get("mysql_db"), E(domain)))
            {
                if (select.Read())
                {
                    return new Account(select.GetInt32(0))
                    {
                        Name = select.IsDBNull(1) ? null : select.GetString(1),
                        Guid = select.IsDBNull(2) ? null : (Guid?) Guid.Parse(select.GetString(2)),
                        ADHost = select.IsDBNull(3) ? null : select.GetString(3),
                        ADDomain = select.IsDBNull(4) ? null : select.GetString(4),
                        ADLoginDomainPrepend = select.IsDBNull(5) ? null : select.GetString(5),
                        TicketMailFromName = select.IsDBNull(6) ? null : select.GetString(6),
                        TicketMailFromDomain = select.IsDBNull(7) ? null : select.GetString(7),
                        TicketHostPrepend = select.IsDBNull(8) ? null : select.GetString(8)
                    };
                }
            }
            return null;
        }

        const string GetAccountsForUserQuery =
        @"SELECT 
        auth_customer.account_id, 
        accounts.name,
        guid,
        ad_host,
        ad_domain,
        ad_login_dn_prepend,
        ticket_mail_from_name,
        ticket_mail_from_domain,
        ticket_host_prefix
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
        public List<Account> GetAccountsForUser(User user)
        {
            if (user.IsGlobalAdmin)
            {
                return GetAccounts();
            }

            List<Account> list = new List<Account>();
            using (var select = Select(GetAccountsForUserQuery, Config.Get("mysql_db"), user.ID))
                while (select.Read())
                {
                    list.Add(new Account(select.GetInt32(0))
                    {
                        Name = select.IsDBNull(1) ? null : select.GetString(1),
                        Guid = select.IsDBNull(2) ? null : (Guid?)Guid.Parse(select.GetString(2)),
                        ADHost = select.IsDBNull(3) ? null : select.GetString(3),
                        ADDomain = select.IsDBNull(4) ? null : select.GetString(4),
                        ADLoginDomainPrepend = select.IsDBNull(5) ? null : select.GetString(5),
                        TicketMailFromName = select.IsDBNull(6) ? null : select.GetString(6),
                        TicketMailFromDomain = select.IsDBNull(7) ? null : select.GetString(7),
                        TicketHostPrepend = select.IsDBNull(8) ? null : select.GetString(8)
                    });
                }
            return list;
        }

        const string GetAccountsQuery =
        @"SELECT 
        id,
        name,
        guid,
        ad_host,
        ad_domain,
        ad_login_dn_prepend,
        ticket_mail_from_name,
        ticket_mail_from_domain,
        ticket_host_prefix 
        FROM 
        {0}.accounts 
        ORDER BY 
        name ASC";
        public List<Account> GetAccounts()
        {
            List<Account> list = new List<Account>();
            using (var select = Select(GetAccountsQuery, Config.Get("mysql_db")))
                while (select.Read())
                {
                    list.Add(new Account(select.GetInt32(0))
                    {
                        Name = select.IsDBNull(1) ? null : select.GetString(1),
                        Guid = select.IsDBNull(2) ? null : (Guid?)Guid.Parse(select.GetString(2)),
                        ADHost = select.IsDBNull(3) ? null : select.GetString(3),
                        ADDomain = select.IsDBNull(4) ? null : select.GetString(4),
                        ADLoginDomainPrepend = select.IsDBNull(5) ? null : select.GetString(5),
                        TicketMailFromName = select.IsDBNull(6) ? null : select.GetString(6),
                        TicketMailFromDomain = select.IsDBNull(7) ? null : select.GetString(7),
                        TicketHostPrepend = select.IsDBNull(8) ? null : select.GetString(8)
                    });
                }
            return list;
        }
    }
}
