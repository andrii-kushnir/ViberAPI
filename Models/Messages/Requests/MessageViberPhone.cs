using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class MessageViberPhone : Message
    {
        public UserViber UserViber { get; set; }
        public MessageViberPhone()
        {
            MessageType = MessageTypes.MessageViberPhone;
        }

        public MessageViberPhone(UserViber userViber) : this()
        {
            UserViber = userViber;
        }
    }
}
