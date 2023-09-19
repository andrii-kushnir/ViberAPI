using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ChangeOperatorRequest : Request
    {
        public User User { get; set; }
        public ChangeOperatorRequest()
        {
            MessageType = MessageTypes.ChangeOperatorRequest;
        }

        public ChangeOperatorRequest(User user) : this()
        {
            User = user;
        }
    }
}
