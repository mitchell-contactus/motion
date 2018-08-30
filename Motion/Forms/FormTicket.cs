using System.Collections.Generic;

namespace Motion.Forms
{
    public class FormTicket
    {
        public int ID { get; }
        public string CreatedDate { get; set; }
        public int Status { get; set; }
        public string Subject { get; set; }
        public int? OpenedById { get; set; }
        public int? ClosedById { get; set; }
        public string DueDate { get; set; }
        public string OpenedByName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string AssignedName { get; set; }
        public string AssignedFirstName { get; set; }
        public string AssignedLastName { get; set; }
        public string FormName { get; set; }
        public int FormId { get; set; }
        public int? Priority { get; set; }
        public string UpdatedDate { get; set; }

        public FormTicket(int ID) {
            this.ID = ID;
        }
    }
}
