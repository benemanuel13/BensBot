using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Net.Http.Headers;
using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web;
using System.Web.SessionState;

using Microsoft.Bot.Connector.DirectLine;
using System.Threading.Tasks;

using bensBot.Models;

namespace bensBot.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class BotController : ApiController
    {
        private static string DirectlineUrl
                = @"https://directline.botframework.com";
        private static string directLineSecret =
            "DirectLine Secret Goes Here...";
        private static string botId =
            "BensBot02";

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Bot/StartConversation")]
        public async Task<ClientInfo> StartConversation()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["UserId"];

            string id;

            if (cookie == null)
            {
                id = Guid.NewGuid().ToString();

                HttpCookie newCookie = new HttpCookie("UserId", id);
                newCookie.Expires = DateTime.MaxValue;
                //HttpContext.Current.Response.Cookies.Add(newCookie);
            }
            else
                id = cookie.Value;

            DirectLineClient client = new DirectLineClient(directLineSecret);

            Conversation conversation = await client.Conversations.StartConversationAsync();
            HttpContext.Current.Session["conversation"] = conversation;

            id = "BenEmanuel";
            ClientInfo info = new ClientInfo(id, conversation.ConversationId, conversation.StreamUrl, conversation.Token);

            return info;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Bot/SendBotMessage")]
        public async Task<SimpleReply> SendBotMessage([FromBody]string message)
        {
            SimpleReply reply = await SendAndRecieveMessageAsync(message);

            return reply;
        }

        private async Task<SimpleReply> SendAndRecieveMessageAsync(string message)
        {
            //string userId = HttpContext.Current.Request.Cookies["UserId"].Value;
            string userId = "BenEmanuel";

            // Connect to the DirectLine service
            DirectLineClient client = new DirectLineClient(directLineSecret);

            // Try to get the existing Conversation
            Conversation conversation;

            // Try to get an existing watermark 
            // the watermark marks the last message we received
            string watermark;

            if (System.Web.HttpContext.Current.Session["watermark"] == null)
                watermark = "";
            else 
                watermark = System.Web.HttpContext.Current.Session["watermark"] as string;

            if (System.Web.HttpContext.Current.Session["conversation"] == null)
            {
                // There is no existing conversation
                // start a new one
                conversation = await client.Conversations.StartConversationAsync();
            }
            else
                conversation = System.Web.HttpContext.Current.Session["conversation"] as Conversation;

            // Use the text passed to the method (by the user)
            // to create a new message
            Activity userMessage = new Activity
            {
                From = new ChannelAccount(userId),
                Text = message,
                Type = ActivityTypes.Message
            };

            // Post the message to the Bot
            await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);

            // Get the response as a Chat object
            //SimpleReply objChat =
            //    await ReadBotMessagesAsync(client, conversation.ConversationId, watermark);

            SimpleReply objChat = new SimpleReply();
            objChat.Message = "Hello From Bot";
            objChat.FromMessage = message;
            objChat.Watermark = "12345";

            // Save values
            System.Web.HttpContext.Current.Session["conversation"] = conversation;
            System.Web.HttpContext.Current.Session["watermark"] = objChat.Watermark;

            // Return the response as a Chat object
            return objChat;
        }

        private async Task<SimpleReply> ReadBotMessagesAsync(
            DirectLineClient client, string conversationId, string watermark)
        {
            // Create an Instance of the Chat object
            SimpleReply objChat = new SimpleReply();

            // We want to keep waiting until a message is received
            bool messageReceived = false;
            while (!messageReceived)
            {
                // Retrieve the activity set from the bot.
                var activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);

                // Set the watermark to the message received
                watermark = activitySet?.Watermark;

                // Extract the activies sent from our bot.
                var activities = (from Activity in activitySet.Activities
                                  where Activity.From.Id == botId
                                  select Activity).ToList();

                // Analyze each activity in the activity set.
                foreach (Activity activity in activities)
                {
                    // Set the text response
                    // to the message text
                    objChat.Message
                        += " "
                        + activity.Text.Replace("\n\n", "<br />");

                    // Are there any attachments?
                    if (activity.Attachments != null)
                    {
                        // Extract each attachment from the activity.
                        foreach (Attachment attachment in activity.Attachments)
                        {
                            switch (attachment.ContentType)
                            {
                                case "image/png":
                                    // Set the text response as an HTML link
                                    // to the image
                                    objChat.Message
                                        += " "
                                        + attachment.ContentUrl;
                                    break;
                            }
                        }
                    }
                }

                // Mark messageReceived so we can break 
                // out of the loop
                messageReceived = true;
            }

            // Set watermark on the Chat object that will be 
            // returned
            objChat.Watermark = watermark;

            // Return a response as a Chat object
            return objChat;
        }
    }
}
