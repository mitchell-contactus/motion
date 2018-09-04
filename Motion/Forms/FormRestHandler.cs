using System;
using System.Net;
using Grapevine;
using Grapevine.Server;
using Motion.Rest;

namespace Motion.Forms
{
    public sealed class FormRestHandler : RestBase
    {
        readonly FormData formData = new FormData();

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/forms/list")]
        public void GetFormList(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);
                var forms = formData.GetForms(session);
                SendJsonResponse(context, forms);
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
    }
}
