using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class PingRequest : Request
    {
        public DateTime Time { get; set; }
        public PingRequest()
        {
            MessageType = MessageTypes.PingRequest;
            Time = DateTime.Now;
        }
    }
}
