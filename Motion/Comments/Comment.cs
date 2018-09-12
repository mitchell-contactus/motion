using System;
namespace Motion.Comments
{
    public class Comment
    {
        public string CreatedDate { get; internal set; }
        public int UserId { get; internal set; }
        public string UserName { get; internal set; }
        public string Entry { get; internal set; }
        public string TextEntry { get; internal set; }
        public int Source { get; internal set; }
        public COMMENT_TYPE Type { get; internal set; }
    }

    public enum COMMENT_TYPE
    {
        INTERNAL = 0,
        PUBLIC = 1
    }

    public enum COMMENT_SOURCE
    {
        Unknown = -1,
        Email = 1,
        Web = 2,
        Slack = 3,
        Sms = 4,
        Phone = 5
    };
}
