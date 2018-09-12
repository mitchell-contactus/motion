using System;
using Motion.Accounts;
using Motion.Rest;

namespace Motion.AD
{
    public class ADConnectionFactory
    {

        readonly AccountData data = new AccountData();

        public ADConnection BuildConnection(string username)
        {
            if (!username.Contains("@"))
            {
                throw new RequestException("AD username invalid");
            }

            String[] parts = username.Split(new[] { '@' });
            if ((parts == null) || (parts.Length != 2))
            {
                throw new RequestException("AD username invalid");
            }

            Account account = data.GetAccountByAD(parts[1].ToLower());
            if (account == null)
            {
                throw new RequestException("No Accounts set up with AD that can process user");
            }

            return new ADConnection(parts[0].ToLower(), parts[1].ToLower(), account);
        }
    }
}
