using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class AttachOperatorRequest : Message
    {
        public UserViber UserViber { get; set; }
        public AttachOperatorRequest()
        {
            MessageType = MessageTypes.AttachOperatorRequest;
        }

        public AttachOperatorRequest(UserViber userViber) : this()
        {
            UserViber = userViber;
        }
    }
}
