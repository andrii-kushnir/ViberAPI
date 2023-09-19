using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class NewConversationRequest : Request
    {
        public UserViber UserViber { get; set; }
        public NewConversationRequest()
        {
            MessageType = MessageTypes.NewConversationRequest;
        }

        public NewConversationRequest(UserViber userViber) : this()
        {
            UserViber = userViber;
        }
    }
}
