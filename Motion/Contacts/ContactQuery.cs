using System;
using System.Collections.Generic;
using Motion.Database;

namespace Motion.Contacts
{
    public class ContactQuery
    {
        public string Email { get; internal set; }

        public List<string> GenerateQueries()
        {
            List<string> queries = new List<string>();
            if (Email != null)
            {
                queries.Add(String.Format("UPPER(primary_email)=UPPER('{0}')", DataBase.E(Email)));
            }
            return queries;
        }
    }
}
