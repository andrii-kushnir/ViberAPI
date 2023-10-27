using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class NewMessagePromRequest : Request
    {
        public UserProm UserProm { get; set; }
        public NewMessagePromRequest()
        {
            MessageType = MessageTypes.NewMessagePromRequest;
        }

        public NewMessagePromRequest(UserProm userProm) : this()
        {
            UserProm = userProm;
        }
    }
}
