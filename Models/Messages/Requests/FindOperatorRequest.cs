using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class FindOperatorRequest : Request
    {
        public UserViber UserViber { get; set; }
        public FindOperatorRequest()
        {
            MessageType = MessageTypes.FindOperatorRequest;
        }

        public FindOperatorRequest(UserViber userViber) : this()
        {
            UserViber = userViber;
        }
    }
}
