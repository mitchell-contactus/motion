using System;
using Motion.Forms;

namespace Motion.Tickets
{
    public class TicketPermissions : FormPermissions
    {
        public bool CanComment { get; set; }

        public TicketPermissions(FormPermissions formPerm)
        {
            CanView = formPerm.CanView;
            CanComment = formPerm.CanEdit;
            CanEdit = formPerm.CanEdit;
            CanViewInternalComments = formPerm.CanViewInternalComments;
            CanDelete = formPerm.CanDelete;
        }
    }
}
