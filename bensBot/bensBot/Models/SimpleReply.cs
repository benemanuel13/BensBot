using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bensBot.Models
{
    public class SimpleReply
    {
        public string Type { get; set; } = "Reply";
        public string Message { get; set; }
        public string FromMessage { get; set; }
        public string Watermark { get; set; }

        public SimpleReply()
        { }

        //public SimpleReply(string message, string fromMessage)
        //{
        //    Type = "SimpleReply";
        //    Message = message;
        //    FromMessage = fromMessage;
        //}
    }
}