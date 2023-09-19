using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class FileToViberRequest : Request
    {
        public ChatMessage Message { get; set; }
        public User User { get; set; }
        public FileToViberRequest()
        {
            MessageType = MessageTypes.FileToViberRequest;
        }

        public FileToViberRequest(ChatMessage message, User user) : this()
        {
            Message = message;
            User = user;
        }
    }
}
