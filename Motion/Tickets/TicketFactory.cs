using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Motion.Database;
using Motion.Forms;
using Motion.Rest;
using Motion.Sessions;

namespace Motion.Tickets
{
    public class TicketFactory
    {
        readonly Session session;
        readonly int formId;
        readonly string subject;
        readonly Guid guid;

        readonly string source;
        readonly string callId;
        readonly int? contactId;
        readonly int? parentId;

        public readonly Dictionary<int, string> customFields = new Dictionary<int, string>();

        public TicketFactory(NameValueCollection data, Session session, FormData formData)
        {
            this.session = session;
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

            if (data.AllKeys.Contains("call_id"))
            {
                callId = data["call_id"];
            }

            if (data.AllKeys.Contains("contact_id"))
            {
                contactId = Convert.ToInt32(data["contact_id"]);
            }

            if (data.AllKeys.Contains("parent_id"))
            {
                parentId = Convert.ToInt32(data["parent_id"]);
            }

            var fields = formData.GetFieldsForForm(formId);
            foreach (var field in fields)
            {
                if (data.AllKeys.Contains(field.Name))
                {
                    customFields.Add(field.ID, data[field.Name]);
                }
                else if (field.Required)
                {
                    throw new InputException(field.Name);
                }
            }
        }

        public string BuildInsertString()
        {
            string insertString = "created_at, updated_at, opened_by, account_id, form_id, subject, client_guid";

            if (source != null)
            {
                insertString += ", source_type";
            }
            if (callId != null)
            {
                insertString += ", call_id";
            }
            if (contactId != null)
            {
                insertString += ", contact_id";
            }
            if (parentId != null)
            {
                insertString += ", parent_id";
            }

            return insertString;
        }

        public string BuildValuesString()
        {
            List<string> values = new List<string>
            {
                "now()",
                "now()",
                session.UserId.ToString(),
                session.AccountId.ToString(),
                formId.ToString(),
                '"' + DataBase.E(subject) + '"',
                '"' + guid.ToString() + '"'
            };
            if (source != null)
            {
                values.Add('"' + DataBase.E(source) + '"');
            }
            if (callId != null)
            {
                values.Add('"' + DataBase.E(callId) + '"');
            }
            if (contactId != null)
            {
                values.Add(contactId.ToString());
            }
            if (parentId != null)
            {
                values.Add(parentId.ToString());
            }
            return String.Join(",", values);
        }
    }
}
