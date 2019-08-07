using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bensBot.Models
{
    public class ClientInfo
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }
        public string StreamUrl { get; set; }
        public string Token { get; set; }
        

        public ClientInfo()
        { }

        public ClientInfo(string userId, string conversationId, string streamUrl, string token)
        {
            UserId = userId;
            ConversationId = conversationId;
            StreamUrl = streamUrl;
            Token = token;
        }
    }
}