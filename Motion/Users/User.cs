namespace Motion.Users
{
    public class User
    {
        public int ID { get; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsGlobalAdmin { get; set; }
        public bool IsActive { get; set; }

        readonly UserData connection;

        public User(int ID, UserData connection) {
            this.ID = ID;
            this.connection = connection;
        }
    }
}
