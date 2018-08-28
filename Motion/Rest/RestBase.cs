using System;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using Grapevine.Server;

namespace Motion.Rest
{
    public abstract class RestBase : RESTResource
    {
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
