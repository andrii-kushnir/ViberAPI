using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class UserDetailsBuhnetRequest : Request
    {
        public User User { get; set; }
        public UserDetailsBuhnetRequest()
        {
            MessageType = MessageTypes.UserDetailsBuhnetRequest;
        }

        public UserDetailsBuhnetRequest(User user) : this()
        {
            User = user;
        }
    }
}
