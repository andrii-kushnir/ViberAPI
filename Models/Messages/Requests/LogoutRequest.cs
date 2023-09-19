using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class LogoutRequest : Request
    {
        public LogoutRequest()
        {
            MessageType = MessageTypes.LogoutRequest;
        }
    }
}
