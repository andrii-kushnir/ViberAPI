using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ArseniumOfflineRequest : Message
    {
        public User User { get; set; }
        public ArseniumOfflineRequest()
        {
            MessageType = MessageTypes.ArseniumOfflineRequest;
        }

        public ArseniumOfflineRequest(User user) : this()
        {
            User = user;
        }
    }
}
