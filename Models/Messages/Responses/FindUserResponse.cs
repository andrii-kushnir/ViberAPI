using Models.Messages.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Responses
{
    public class FindUserResponse : Response
    {
        public UserViber UserViber { get; set; }
        public FindUserResponse()
        {
            MessageType = MessageTypes.FindUserResponse;
        }

        public FindUserResponse(FindUserRequest request, UserViber userViber) : this()
        {
            RequestId = request.Id;
            UserViber = userViber;
        }
    }
}
