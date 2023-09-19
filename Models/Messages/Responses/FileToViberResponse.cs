using Models.Messages.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Responses
{
    public class FileToViberResponse : Response
    {
        public UserViber UserViber { get; set; }
        public ChatMessage Message { get; set; }
        public FileToViberResponse()
        {
            MessageType = MessageTypes.FileToViberResponse;
        }

        public FileToViberResponse(FileToViberRequest request, UserViber userViber, ChatMessage message) : this()
        {
            RequestId = request.Id;
            UserViber = userViber;
            Message = message;
        }
    }
}
