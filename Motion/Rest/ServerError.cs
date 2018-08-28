using System;
namespace Motion.Rest
{
    public class ServerError
    {
        public int ErrorCode { get; set; }
        public string Error { get; set; }
        public string ErrorDetail { get; set; }
    }
}
