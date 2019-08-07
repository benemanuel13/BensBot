using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bensBot.Models
{
    public class Reply
    {
        public string UserId { get; set; }
        public string Type { get; set; } = "Reply";
        public string FromMessage { get; set; }

        public Reply() { }

        public Reply (string fromMessage)
        {
            FromMessage = fromMessage;
        }
    }
}