using System;
namespace Motion.Comments
{
    public class Comment
    {
        public string CreatedDate { get; internal set; }
        public int UserId { get; internal set; }
        public string Entry { get; internal set; }
        public string TextEntry { get; internal set; }
        public int Source { get; internal set; }
        public int Type { get; internal set; }
    }
}
