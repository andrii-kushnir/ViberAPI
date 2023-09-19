using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class FixMessageRequest : Request
    {
        public string Message { get; set; }
        public User User { get; set; }
        public FixMessageRequest()
        {
            MessageType = MessageTypes.FixMessageRequest;
        }

        public FixMessageRequest(string message, User user) : this()
        {
            Message = message;
            User = user;
        }
    }
}
