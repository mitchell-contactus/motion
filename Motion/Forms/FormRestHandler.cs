using System;
using System.Linq;
using System.Net;
using Grapevine;
using Grapevine.Server;
using Motion.Rest;
using Motion.Views;

namespace Motion.Forms
{
    public sealed class FormRestHandler : RestBase
    {
        readonly FormData formData = new FormData();
        readonly ViewData viewData = new ViewData();

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

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/forms/getFields")]
        public void GetFieldsForForm(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);
                if (!data.AllKeys.Contains("formId"))
                {
                    throw new InputException("formId");
                }
                var fields = formData.GetFieldsForForm(Convert.ToInt32(data["formId"]));
                SendJsonResponse(context, fields);
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

        [RESTRoute(Method = HttpMethod.POST, PathInfo = @"^/api/views/list")]
        public void GetViewList(HttpListenerContext context)
        {
            try
            {
                var data = GetRequestPostData(context.Request);
                var session = ValidateSession(data);
                var views = viewData.GetViews(session.AccountId);
                SendJsonResponse(context, views);
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
