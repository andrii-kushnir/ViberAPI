using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Messages.Requests;

namespace Models.Messages.Responses
{
    public class UserDetailsResponse : Response
    {
        public User User { get; set; }
        public UserViber UserViber { get; set; }
        public UserDetailsResponse()
        {
            MessageType = MessageTypes.UserDetailsResponse;
        }

        public UserDetailsResponse(UserDetailsRequest request, User user, UserViber userViber) : this()
        {
            RequestId = request.Id;
            User = user;
            UserViber = userViber;
        }
    }
}
