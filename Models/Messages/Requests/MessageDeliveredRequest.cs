using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class MessageDeliveredRequest : Message
    {
        public UserViber UserViber { get; set; }
        public DateTime DateDelivered { get; set; }
        public long Token { get; set; }
        public MessageDeliveredRequest()
        {
            MessageType = MessageTypes.MessageDeliveredRequest;
        }

        public MessageDeliveredRequest(UserViber userViber, DateTime dateDelivered, long token) : this()
        {
            UserViber = userViber;
            DateDelivered = dateDelivered;
            Token = token;
        }
    }
}
