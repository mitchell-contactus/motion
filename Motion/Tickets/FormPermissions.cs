using System;
namespace Motion.Tickets
{
    public class FormPermissions
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanViewInternalComments { get; set; }
        public bool CanDelete { get; set; }
    }
}
