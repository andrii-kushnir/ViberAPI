using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class MessageToPromRequest : Request
    {
        public User User { get; set; }
        public string Text { get; set; }
        public MessageToPromRequest()
        {
            MessageType = MessageTypes.MessageToPromRequest;
        }

        public MessageToPromRequest(User user, string text) : this()
        {
            User = user;
            Text = text;
        }
    }
}
