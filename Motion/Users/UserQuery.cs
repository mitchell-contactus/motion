using System;
using System.Collections.Generic;
using Motion.Database;

namespace Motion.Users
{
    public class UserQuery
    {
        public int? ID { get; internal set; }
        public string Username { get; internal set; }

        public List<string> GenerateQueries()
        {
            List<string> queries = new List<string>();
            if (ID != null)
            {
                queries.Add(String.Format("id = {0}", ID));
            }
            if (Username != null)
            {
                queries.Add(String.Format("UPPER(username)=UPPER('{0}')", DataBase.E(Username)));
            }
            return queries;
        }
    }
}
