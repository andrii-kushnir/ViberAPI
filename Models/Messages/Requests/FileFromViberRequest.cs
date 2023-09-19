using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class FileFromViberRequest : Request
    {
        public ChatMessage Message { get; set; }
        public DateTime Date { get; set; }
        public UserViber UserViber { get; set; }
        public FileFromViberRequest()
        {
            MessageType = MessageTypes.FileFromViberRequest;
        }

        public FileFromViberRequest(ChatMessage message, DateTime date, UserViber userViber) : this()
        {
            Message = message;
            Date = date;
            UserViber = userViber;
        }
    }
}
