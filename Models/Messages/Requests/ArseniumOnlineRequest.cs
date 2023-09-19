using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ArseniumOnlineRequest : Message
    {
        public User User { get; set; }
        public ArseniumOnlineRequest()
        {
            MessageType = MessageTypes.ArseniumOnlineRequest;
        }

        public ArseniumOnlineRequest(User user) : this()
        {
            User = user;
        }
    }
}