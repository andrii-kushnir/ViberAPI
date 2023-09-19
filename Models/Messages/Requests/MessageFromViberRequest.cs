using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class MessageFromViberRequest : Message
    {
        public ChatMessage Message { get; set; }
        public DateTime Date { get; set; }
        public UserViber UserViber { get; set; }
        public MessageFromViberRequest()
        {
            MessageType = MessageTypes.MessageFromViberRequest;
        }

        public MessageFromViberRequest(ChatMessage message, DateTime date, UserViber userViber) : this()
        {
            Message = message;
            Date = date;
            UserViber = userViber;
        }
    }
}
