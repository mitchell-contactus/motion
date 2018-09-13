using System;
using System.Collections.Generic;
using Motion.Database;
using Motion.Sessions;

namespace Motion.Views
{
    public class ViewData : DataBase
    {
        const string GetViewsQuery = 
        @"SELECT
        v.id,
        name,
        form_id
        FROM
        {0}.tt_views v
        LEFT JOIN
        {0}.tt_view_filters vf
        ON
        v.id = vf.view_id
        WHERE
        account_id = {1}";
        public List<View> GetViews(int accountId)
        {
            Dictionary<int, View> viewMap = new Dictionary<int, View>();
            using (var select = Select(GetViewsQuery, Config.Get("mysql_db"), accountId))
                while (select.Read())
            {
                int vId = select.GetInt32(0);
                if (viewMap.ContainsKey(vId))
                {
                    viewMap[vId].Forms.Add(select.GetInt32(2));
                }
                else
                {
                    View v = new View(vId)
                    {
                        Name = select.IsDBNull(1) ? null : select.GetString(1)
                    };
                    v.Forms.Add(select.GetInt32(2));
                    viewMap.Add(vId, v);
                }
            }
            return new List<View>(viewMap.Values);
        }

        const string GetViewQuery = 
        @"SELECT
        v.id,
        name,
        form_id
        FROM
        {0}.tt_views v
        LEFT JOIN
        {0}.tt_view_filters vf
        ON
        v.id = vf.view_id
        WHERE
        v.id = {1}";
        public View GetView(int viewId)
        {
            View view = null;
            using (var select = Select(GetViewQuery, Config.Get("mysql_db"), viewId))
                while (select.Read())
            {
                if (view == null)
                {
                    view = new View(select.GetInt32(0))
                    {
                        Name = select.IsDBNull(1) ? null : select.GetString(1)
                    };
                    view.Forms.Add(select.GetInt32(2));
                }
                else
                {
                    view.Forms.Add(select.GetInt32(2));
                }
            }
            return view;
        }
    }
}
