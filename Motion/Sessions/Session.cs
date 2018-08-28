using System;
namespace Motion.Sessions
{
    public class Session
    {
        public string SessionId { get; }
        public int UserId { get; }
        public int AccountId { get; }

        public Session(string SessionId, int UserId, int AccountId) {
            this.SessionId = SessionId;
            this.UserId = UserId;
            this.AccountId = AccountId;
        }
    }
}
