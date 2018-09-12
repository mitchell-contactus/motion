using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;

namespace Motion.AD
{
    public class ADInfo
    {
        public string AccountName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public List<string> Groups = new List<string>();

        public ADInfo(string accountName, LdapEntry entry)
        {
            AccountName = accountName;
            try
            {
                FirstName = entry.getAttribute("givenName").StringValue;
            }
            catch
            {
                FirstName = "?";
            }

            try
            {
                LastName = entry.getAttribute("SN").StringValue;
            }
            catch
            {
                LastName = "?";
            }

            try
            {
                Email = entry.getAttribute("MAIL").StringValue;
            }
            catch
            {
                Email = "?";
            }

            try
            {
                var groups = entry.getAttribute("MEMBEROF");
                foreach (var value in groups.StringValueArray)
                {
                    string this_value = value;
                    if (this_value.Contains(","))
                    {
                        this_value = value.Substring(0, this_value.IndexOf(",", StringComparison.Ordinal));
                    }
                        
                    if (this_value.ToUpper().StartsWith("CN=", StringComparison.Ordinal))
                    {
                        this_value = this_value.Substring(3);
                    }
                    Groups.Add(this_value);
                }
            }
            catch
            {
            }
        }
    }
}
