using System;
using Motion.Accounts;
using Motion.Rest;
using Motion.Sessions;
using Motion.Users;
using Novell.Directory.Ldap;

namespace Motion.AD
{
    public class ADConnection
    {
        const int LdapVersion = LdapConnection.Ldap_V3;
        const string SearchBase = "DC=contactusinc,DC=com";

        readonly UserData userData = new UserData();

        public string Username { get; }
        public string Domain { get; }
        public Account Account { get; }

        public ADConnection(string username, string domain, Account account)
        {
            Username = username;
            Domain = domain;
            Account = account;
        }

        public User Authenticate(string password)
        {
            LdapConnection lc = new LdapConnection();
            ADInfo info = null;
            try
            {
                lc.Connect(Account.ADHost, 389);
                lc.Bind(LdapVersion, Account.ADLoginDomainPrepend + Username, password);
                info = Search(lc);
                lc.Disconnect();
            }
            catch (Exception)
            {
                throw new RequestException("Error LDAP connection");
            }
            if (info == null)
            {
                throw new RequestException("Error getting AD Info");
            }

            string email = Username + "@" + Domain;
            User user = userData.GetUser(Account.ID, new UserQuery { Username = email });
            if (user == null)
            {
                user = userData.CreateUser(email, info.FirstName, info.LastName, Account.ID);
                if (user == null)
                {
                    throw new RequestException("Error creating user");
                }
            }
            else
            {
                //TODO: Update name of user?
            }
            userData.UpdateADGroupsForUser(user.ID, info);
            return user;
        }

        ADInfo Search(LdapConnection ldapConn)
        {
            LdapSearchConstraints constraints = new LdapSearchConstraints
            {
                TimeLimit = 10000
            };

            LdapSearchResults lsc = ldapConn.Search(
                SearchBase,
                LdapConnection.SCOPE_SUB,
                "SAMAccountName=" + Username.ToLower(),
                null, // no specified attributes
                false, // return attr and value
                constraints);

            while (lsc.hasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.next();
                    return new ADInfo(Username, nextEntry);
                }
                catch (LdapException e)
                {
                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                    // Exception is thrown, go for next entry
                    continue;
                }
            }
            return null;
        }
    }
}
