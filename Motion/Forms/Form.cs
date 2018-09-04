using System;
namespace Motion.Forms
{
    public class Form
    {
        public int ID { get; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public FormPermissions Permissions { get; internal set; }

        public Form(int ID)
        {
            this.ID = ID;
        }
    }
}
