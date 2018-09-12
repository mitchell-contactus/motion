using System;
using System.Collections.Generic;
using Motion.Database;

namespace Motion.Contacts
{
    public class ContactData : DataBase
    {
        const string GetContactsQuery = 
        @"SELECT 
        id 
        FROM 
        {0}.tt_contacts 
        WHERE 
        account_id = {1}
        AND
        {2}";
        public List<Contact> getContacts(int accountId, ContactQuery query)
        {
            var queryString = String.Join(" AND ", query.GenerateQueries());
            List<Contact> list = new List<Contact>();
            using (var select = Select(GetContactsQuery, Config.Get("mysql_db"), accountId, queryString))
                while (select.Read())
                {
                    list.Add(new Contact(select.GetInt32(0))
                    {
                    });
                }
            return list;
        }
    }
}
