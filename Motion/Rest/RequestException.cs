using System;
namespace Motion.Rest
{
    public class RequestException : Exception
    {
        public string Reason { get; }

        public RequestException(string Reason) {
            this.Reason = Reason;
        }
    }
}
