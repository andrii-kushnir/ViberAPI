using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ImageToViberRequest : Request
    {
        public ChatMessage Message { get; set; }
        public User User { get; set; }
        public ImageToViberRequest()
        {
            MessageType = MessageTypes.ImageToViberRequest;
        }

        public ImageToViberRequest(ChatMessage message, User user) : this()
        {
            Message = message;
            User = user;
        }
    }
}
