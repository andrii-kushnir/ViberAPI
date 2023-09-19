using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ReConnectRequest : Request
    {
        public Guid ClientId { get; set; }
        public ReConnectRequest(Guid clientId)
        {
            MessageType = MessageTypes.ReConnectRequest;
            ClientId = clientId;
        }
    }
}
