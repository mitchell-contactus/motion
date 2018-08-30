using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Motion.Database;

namespace Motion.Forms
{
    public class TicketFilter
    {
        public int? FormId { get; }

        public TicketFilter(NameValueCollection data)
        {
            FormId = null;
            if (data.AllKeys.Contains("formId"))
            {
                FormId = Convert.ToInt32(data["formId"]);
            }
        }

        public List<string> GetFilters() {
            List<string> filters = new List<string>();
            if (FormId != null) {
                filters.Add("tt_tickets.form_id = " + FormId);
            }

            return filters;
        }

        private string E(string s) {
            return DataBase.E(s);
        }
    }
}
