using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Grapevine;
using Grapevine.Server;
using Motion.Rest;
using Motion.Sessions;

namespace Motion.Users
{
    public sealed class UserRestHandler : RestBase
    {
        readonly UserData userData = new UserData();
        readonly SessionData sessionData = new SessionData();

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/auth/listAccountsForUser")]
        public void ListAccountsForUser(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var user = AuthenticateUser(data);

                if (user != null)
                {
                    var accountList = userData.GetAccountsForUser(user);
                    SendJsonResponse(context, accountList);
                }
                else
                {
                    SendInvalidAuth(context);
                }
            }
            catch (RequestException e)
            {
                SendUnexpectedError(context, e.Reason);
            }
            catch (InputException e)
            {
                SendMissingParameter(context, e.Reason);
            }
        }

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/users/list")]
        public void ListUsers(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);
                var users = userData.GetUsers(session);
                SendJsonResponse(context, users);
            }
            catch (RequestException e)
            {
                SendUnexpectedError(context, e.Reason);
            }
            catch (InputException e)
            {
                SendMissingParameter(context, e.Reason);
            }
        }

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/auth/createSession")]
        public void CreateSession(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var user = AuthenticateUser(data);

                if (!data.AllKeys.Contains("account"))
                {
                    throw new InputException("account");
                }

                int accountId;
                try
                {
                    accountId = Convert.ToInt32(data["account"]);
                }
                catch
                {
                    throw new RequestException("Account Id Malformed");
                }

                if (user != null)
                {
                    var accountList = userData.GetAccountsForUser(user);
                    if (accountList.ContainsKey(accountId))
                    {
                        var sessionId = sessionData.CreateSession(user, accountId);
                        if (sessionId == null) {
                            throw new RequestException("Failed To Create Session");
                        }
                        SendTextResponse(context, sessionId);
                    }
                }
                else
                {
                    SendInvalidAuth(context);
                }
            }
            catch (RequestException e)
            {
                SendUnexpectedError(context, e.Reason);
            }
            catch (InputException e)
            {
                SendMissingParameter(context, e.Reason);
            }
        }

        User AuthenticateUser(NameValueCollection data)
        {
            if (!data.AllKeys.Contains("username"))
            {
                throw new InputException("username");
            }

            if (!data.AllKeys.Contains("password"))
            {
                throw new InputException("password");
            }

            var user = userData.AuthUser(data["username"], data["password"]);
            if (user == null)
            {
                // TODO: Add AD authentication
            }
            return user;
        }

        void SendInvalidAuth(HttpListenerContext context)
        {
            SendError(context, new ServerError() { ErrorCode = 401, Error = "Invalid Authorization Code" });
        }
    }
}
