using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ReturnAwayRequest : Request
    {
        public ReturnAwayRequest()
        {
            MessageType = MessageTypes.ReturnAwayRequest;
        }
    }
}
