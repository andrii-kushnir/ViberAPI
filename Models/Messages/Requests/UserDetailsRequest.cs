using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class UserDetailsRequest : Request
    {
        public User User { get; set; }
        public UserDetailsRequest()
        {
            MessageType = MessageTypes.UserDetailsRequest;
        }

        public UserDetailsRequest(User user) : this()
        {
            User = user;
        }
    }
}
