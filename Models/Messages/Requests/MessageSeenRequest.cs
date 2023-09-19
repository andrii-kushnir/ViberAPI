using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class MessageSeenRequest : Message
    {
        public UserViber UserViber { get; set; }
        public DateTime DateSeen { get; set; }
        public long Token { get; set; }
        public MessageSeenRequest()
        {
            MessageType = MessageTypes.MessageSeenRequest;
        }

        public MessageSeenRequest(UserViber userViber, DateTime dateSeen, long token) : this()
        {
            UserViber = userViber;
            DateSeen = dateSeen;
            Token = token;
        }
    }
}
