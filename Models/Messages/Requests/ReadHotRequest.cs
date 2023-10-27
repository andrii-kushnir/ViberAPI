using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ReadHotRequest : Request
    {
        public User User { get; set; }
        public ReadHotRequest()
        {
            MessageType = MessageTypes.ReadHotRequest;
        }

        public ReadHotRequest(User user) : this()
        {
            User = user;
        }
    }
}
