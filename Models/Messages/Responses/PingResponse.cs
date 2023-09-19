using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Messages.Requests;

namespace Models.Messages.Responses
{
    public class PingResponse : Response
    {
        public DateTime Time { get; set; }
        public PingResponse()
        {
            MessageType = MessageTypes.PingResponse;
        }

        public PingResponse(PingRequest request) : this()
        {
            Time = request.Time;
            RequestId = request.Id;
        }
    }
}
