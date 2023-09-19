using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Messages.Requests;

namespace Models.Messages.Responses
{
    public class UserListResponse : Response
    {
        public List<User> Users { get; set; }
        public UserListResponse()
        {
            MessageType = MessageTypes.UserListResponse;
        }

        public UserListResponse(UserListRequest request, List<User> users) : this()
        {
            RequestId = request.Id;
            Users = users;
        }
    }
}
