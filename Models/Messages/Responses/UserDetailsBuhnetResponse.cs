using Models.Messages.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Responses
{
    public class UserDetailsBuhnetResponse : Response
    {
        public UserBuhnet UserBuhnet { get; set; }
        public UserViber UserViber { get; set; }
        public UserDetailsBuhnetResponse()
        {
            MessageType = MessageTypes.UserDetailsResponse;
        }

        public UserDetailsBuhnetResponse(UserDetailsBuhnetRequest request, UserBuhnet userBuhnet, UserViber userViber) : this()
        {
            RequestId = request.Id;
            UserBuhnet = userBuhnet;
            UserViber = userViber;
        }
    }
}
