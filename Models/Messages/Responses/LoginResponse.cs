using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Messages.Requests;

namespace Models.Messages.Responses
{
    public class LoginResponse : Response
    {
        public bool Access { get; set; }
        public UserArsenium Myself { get; set; }
        public List<UserArsenium> UserList { get; set; }
        public List<UserViber> NightClient { get; set; }
        public List<UserViber> LastClient { get; set; }
        public LoginResponse()
        {
            MessageType = MessageTypes.LoginResponse;
        }

        public LoginResponse(LoginRequest request, bool access, UserArsenium myself, List<UserArsenium> userList, List<UserViber> nightClient, List<UserViber> lastClient) : this()
        {
            RequestId = request.Id;
            Access = access;
            Myself = myself;
            UserList = userList;
            NightClient = nightClient;
            LastClient = lastClient;
        }
    }
}
