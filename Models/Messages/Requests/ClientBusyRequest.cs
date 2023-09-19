using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ClientBusyRequest : Message
    {
        public UserViber UserViber { get; set; }
        public ClientBusyRequest()
        {
            MessageType = MessageTypes.ClientBusyRequest;
        }

        public ClientBusyRequest(UserViber userViber) : this()
        {
            UserViber = userViber;
        }
    }
}
