using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class MessageToViberRequest : Request
    {
        public ChatMessage Message { get; set; }
        public User User { get; set; }
        public MessageToViberRequest()
        {
            MessageType = MessageTypes.MessageToViberRequest;
        }

        public MessageToViberRequest(ChatMessage message, User user) : this()
        {
            Message = message;
            User = user;
        }
    }
}
