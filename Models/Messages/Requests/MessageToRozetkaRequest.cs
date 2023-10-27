using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class MessageToRozetkaRequest : Request
    {
        public User User { get; set; }
        public string Text { get; set; }
        public MessageToRozetkaRequest()
        {
            MessageType = MessageTypes.MessageToRozetkaRequest;
        }

        public MessageToRozetkaRequest(User user, string text) : this()
        {
            User = user;
            Text = text;
        }
    }
}
