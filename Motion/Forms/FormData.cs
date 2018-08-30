using System;
using System.Collections.Generic;
using Motion.Database;
using Motion.Sessions;

namespace Motion.Forms
{
    public class FormData : DataBase
    {
        const string ListTicketsQuery =
        @"SELECT
        tt_tickets.id,
        (CONVERT_TZ((tt_tickets.created_at), 'UTC','{4}')) as created_at,
        status,
        subject,
        opened_by,
        closed_by,
        (CONVERT_TZ( (due_at), 'UTC','{4}')) as due_at,
        UNIX_TIMESTAMP(due_at) * 1000 as due_at_utc,
        UNIX_TIMESTAMP(now()) * 1000 as now_utc,
        ala.name,
        aca.first_name as contact_first_name,
        aca.last_name as contact_last_name,
        alb.name as assigned_name,
        acb.first_name as assigned_first_name,
        acb.last_name as assigned_last_name,
        tt_ticket_forms.name as form_name,
        tt_tickets.form_id as form_id,
        tt_tickets.priority as priority,
        (CONVERT_TZ( (tt_tickets.updated_at), 'UTC','{4}')) as updated_at,
        tt_tickets.locked_by as locked_by,
        (CONVERT_TZ( (tt_tickets.locked_until), 'UTC','{4}')) as locked_until
        FROM
        {0}.tt_tickets
        LEFT JOIN 
        {0}.auth_local ala 
        ON 
        ala.id=tt_tickets.opened_by
        LEFT JOIN 
        {0}.tt_contacts aca 
        ON 
        aca.id=tt_tickets.opened_by
        LEFT JOIN 
        {0}.auth_local alb 
        ON 
        alb.id=tt_tickets.assigned_to
        LEFT JOIN 
        {0}.tt_contacts acb 
        ON
        acb.id=tt_tickets.assigned_to
        LEFT JOIN 
        {0}.tt_ticket_forms 
        ON 
        tt_ticket_forms.id=tt_tickets.form_id
        WHERE
        tt_tickets.account_id = {1}
        AND
        (
            tt_tickets.form_id IN ({3})
            OR 
            assigned_to = {2}
        )";
        public List<FormTicket> ListTickets(Session session, TicketFilter filter)
        {

            string query = ListTicketsQuery;
            foreach (var f in filter.GetFilters())
            {
                query += " AND " + f;
            }
            query += "ORDER BY tt_tickets.id desc";

            var permissions = GetFormPermissions(session);
            List<string> formIds = new List<string>();
            foreach (var formId in permissions.Keys)
            {
                if (permissions[formId].CanView)
                {
                    formIds.Add(formId.ToString());
                }
            }

            List<FormTicket> tickets = new List<FormTicket>();
            using (var select = Select(query,
                                       Config.Get("mysql_db"),
                                       session.AccountId,
                                       session.UserId,
                                       String.Join(",", formIds),
                                       "EST"))
                while (select.Read())
                {
                    tickets.Add(new FormTicket(select.GetInt32(0))
                    {
                        CreatedDate = select.IsDBNull(1) ? null : select.GetString(1),
                        Status = select.IsDBNull(2) ? 0 : select.GetInt32(2),
                        Subject = select.IsDBNull(3) ? null : select.GetString(3),
                        OpenedById = select.IsDBNull(4) ? null : new int?(select.GetInt32(4)),
                        ClosedById = select.IsDBNull(5) ? null : new int?(select.GetInt32(5)),
                        DueDate = select.IsDBNull(6) ? null : select.GetString(6),
                        OpenedByName = select.IsDBNull(9) ? null : select.GetString(9),
                        ContactFirstName = select.IsDBNull(10) ? null : select.GetString(10),
                        ContactLastName = select.IsDBNull(11) ? null : select.GetString(11),
                        AssignedName = select.IsDBNull(12) ? null : select.GetString(12),
                        AssignedFirstName = select.IsDBNull(13) ? null : select.GetString(13),
                        AssignedLastName = select.IsDBNull(14) ? null : select.GetString(14),
                        FormName = select.IsDBNull(15) ? null : select.GetString(15),
                        FormId = select.IsDBNull(16) ? 0 : select.GetInt32(16),
                        Priority = select.IsDBNull(17) ? null : new int?(select.GetInt32(17)),
                        UpdatedDate = select.IsDBNull(18) ? null : select.GetString(18)
                    });
                }
            return tickets;
        }

        const string GetFormPermissionsQuery =
        @"SELECT
        forms.id,
        perm.can_view,
        perm.can_edit,
        perm.can_view_internal_comments,
        perm.can_delete
        FROM
        {0}.tt_ticket_forms forms
        LEFT JOIN
        (
            SELECT 
            form_id,
            max(can_view) as can_view,
            max(can_edit) as can_edit,
            max(can_view_internal_comments) as can_view_internal_comments,
            max(can_delete) as can_delete, 
            max(can_see_others_todo_items) as can_see_others_todo_items 
            FROM 
            {0}.tt_ticket_role_permissions 
            WHERE 
            tt_ticket_role_permissions.account_id = {1}
            AND 
            (
                tt_ticket_role_permissions.role_id IN
                (
                    SELECT 
                    account_role_id 
                    FROM 
                    {0}.account_roles_ad_group 
                    WHERE 
                    account_id = {1} 
                    AND 
                    ad_group_name IN
                    (
                        SELECT 
                        name 
                        FROM 
                        {0}.auth_user_ad_groups_cache 
                        WHERE 
                        user_id = {2}
                    )
                    UNION 
                    SELECT 
                    role_id 
                    FROM 
                    {0}.account_role_users 
                    WHERE 
                    account_id = {1}
                    AND 
                    user_id = {2}
                )
                OR
                tt_ticket_role_permissions.role_id = -1
            )
            GROUP BY 
            form_id
        ) perm
        ON
        forms.id = perm.form_id
        ORDER BY
        forms.id";
        public Dictionary<int, FormPermissions> GetFormPermissions(Session session)
        {
            var defaultPermissions = GetPermissionsForForm(session, -1);

            Dictionary<int, FormPermissions> permissions = new Dictionary<int, FormPermissions>();
            using (var select = Select(GetFormPermissionsQuery, Config.Get("mysql_db"), session.AccountId, session.UserId))
                while (select.Read())
                {
                    permissions.Add(select.GetInt32(0), new FormPermissions()
                    {
                        CanView = select.IsDBNull(1) ? defaultPermissions.CanView : select.GetBoolean(1),
                        CanEdit = select.IsDBNull(2) ? defaultPermissions.CanEdit : select.GetBoolean(2),
                        CanViewInternalComments = select.IsDBNull(3) ? defaultPermissions.CanViewInternalComments : select.GetBoolean(3),
                        CanDelete = select.IsDBNull(4) ? defaultPermissions.CanDelete : select.GetBoolean(4)
                    });
                }
            return permissions;
        }

        const string GetPermissionsForFormQuery =
        @"SELECT 
        max(can_view) as can_view,
        max(can_edit) as can_edit,
        max(can_view_internal_comments) as can_view_internal_comments,
        max(can_delete) as can_delete, 
        max(can_see_others_todo_items) as can_see_others_todo_items 
        FROM 
        {0}.tt_ticket_role_permissions 
        WHERE 
        tt_ticket_role_permissions.account_id = {1}
        AND
        tt_ticket_role_permissions.form_id = {3}
        AND 
        (
            tt_ticket_role_permissions.role_id IN
            (
                SELECT 
                account_role_id 
                FROM 
                {0}.account_roles_ad_group 
                WHERE 
                account_id = {1} 
                AND 
                ad_group_name IN
                (
                    SELECT 
                    name 
                    FROM 
                    {0}.auth_user_ad_groups_cache 
                    WHERE 
                    user_id = {2}
                )
                UNION 
                SELECT 
                role_id 
                FROM 
                {0}.account_role_users 
                WHERE 
                account_id = {1}
                AND 
                user_id = {2}
            )
            OR
            tt_ticket_role_permissions.role_id = -1
        )";
        public FormPermissions GetPermissionsForForm(Session session, int formId)
        {
            using (var select = Select(GetPermissionsForFormQuery,
                                       Config.Get("mysql_db"), session.AccountId, session.UserId, formId))
            {
                if (select.Read() && !select.IsDBNull(0))
                {
                    return new FormPermissions()
                    {
                        CanView = select.GetBoolean(0),
                        CanEdit = select.GetBoolean(1),
                        CanViewInternalComments = select.GetBoolean(2),
                        CanDelete = select.GetBoolean(3)
                    };
                }
            }
            // If the form is not the default form
            if (formId != -1)
            {
                // Return default permissions
                return GetPermissionsForForm(session, -1);
            }
            // Super default permissions are all false
            return new FormPermissions()
            {
                CanView = false,
                CanEdit = false,
                CanViewInternalComments = false,
                CanDelete = false
            };
        }
    }
}
