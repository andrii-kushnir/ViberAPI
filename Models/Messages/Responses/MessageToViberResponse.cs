using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Messages.Requests;

namespace Models.Messages.Responses
{
    public class MessageToViberResponse : Response
    {
        public UserViber UserViber { get; set; }
        public ChatMessage  Message { get; set; }
        public MessageToViberResponse()
        {
            MessageType = MessageTypes.MessageToViberResponse;
        }

        public MessageToViberResponse(MessageToViberRequest request, UserViber userViber, ChatMessage message) : this()
        {
            RequestId = request.Id;
            UserViber = userViber;
            Message = message;
        }
    }
}
