using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using Grapevine.Server;
using Motion.Sessions;

namespace Motion.Rest
{
    public abstract class RestBase : RESTResource
    {

        readonly SessionData sessionData = new SessionData();

        protected NameValueCollection GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                throw new RequestException("Post has no data");
            }

            using (System.IO.Stream body = request.InputStream)
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    string data = reader.ReadToEnd();
                    try
                    {
                        return HttpUtility.ParseQueryString(data);
                    }
                    catch (Exception)
                    {
                        throw new RequestException("Post data parse error");
                    }
                }
            }
        }

        protected Session ValidateSession(NameValueCollection postData) {
            if (!postData.AllKeys.Contains("session"))
            {
                throw new InputException("session");
            }

            var session = sessionData.GetSession(postData["session"]);
            if (session == null)
            {
                throw new RequestException("Invalid Session");
            }
            return session;
        }

        protected void SendUnexpectedError(HttpListenerContext context, string errorDetail = "")
        {
            SendError(context, new ServerError() { ErrorCode = 500, Error = "Unexpected Error", ErrorDetail = errorDetail });
        }

        protected void SendMissingParameter(HttpListenerContext context, string missingParameter = "")
        {
            SendError(context, new ServerError() { ErrorCode = 400, Error = "The request is missing a required parameter : " + missingParameter });
        }

        protected void SendError(HttpListenerContext context, ServerError error)
        {
            context.Response.StatusCode = error.ErrorCode;
            SendJsonResponse(context, error);
        }
    }
}
