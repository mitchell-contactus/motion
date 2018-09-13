using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Motion.Database;
using Motion.Views;

namespace Motion.Forms
{
    public class TicketFilter
    {
        readonly ViewData viewData = new ViewData();

        public int[] FormIds { get; }
        public int? ViewId { get; }

        public TicketFilter(NameValueCollection data)
        {
            FormIds = null;
            if (data.AllKeys.Contains("formIds"))
            {
                FormIds = data["formIds"].Split(',').Select(s => Convert.ToInt32(s)).ToArray();
            }

            ViewId = null;
            if (data.AllKeys.Contains("viewId"))
            {
                ViewId = Convert.ToInt32(data["viewId"]);
            }
        }

        public List<string> GetFilters() {
            List<string> filters = new List<string>();
            if (FormIds != null) {
                filters.Add(String.Join(" OR ", FormIds.Select(id => "tt_tickets.form_id = " + id)));
            }
            if (ViewId != null) {
                var view = viewData.GetView((int) ViewId);
                filters.Add(String.Join(" OR ", view.Forms.Select(id => "tt_tickets.form_id = " + id)));
            }
            return filters;
        }

        private string E(string s) {
            return DataBase.E(s);
        }
    }
}
