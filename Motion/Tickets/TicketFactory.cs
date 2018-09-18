using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Motion.Forms;
using Motion.Rest;
using Motion.Sessions;

namespace Motion.Tickets
{
    public class TicketFactory
    {
        readonly int formId;
        readonly string subject;
        readonly Guid guid;
        readonly string source;

        public TicketFactory(NameValueCollection data, Session session, FormData formData)
        {
            if (!data.AllKeys.Contains("form_id"))
            {
                throw new InputException("Missing the form id");
            }
            formId = Convert.ToInt32(data["form_id"]);
            var permissions = formData.GetPermissionsForForm(session, formId);
            if (!permissions.CanView)
            {
                throw new RequestException("Not authorized to create ticket");
            }

            if (!data.AllKeys.Contains("subject") || data["subject"].Length == 0)
            {
                throw new InputException("Missing the subject");
            }
            subject = data["subject"];

            if (!data.AllKeys.Contains("GUID"))
            {
                throw new InputException("Missing the GUID");
            }
            guid = Guid.Parse(data["GUID"]);

            if (data.AllKeys.Contains("source"))
            {
                source = data["source"];
            }
            else
            {
                source = "Web";
            }
        }

        public string BuildInsertString()
        {
            return "form_id, subject, client_guid, source_type";
        }

        public string BuildValuesString()
        {
            List<string> values = new List<string>
            {
                formId.ToString(),
                subject,
                guid.ToString(),
                source
            };
            return String.Join(",", values);
        }
    }
}
