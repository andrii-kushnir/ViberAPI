using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class FindUserRequest : Request
    {
        public string Phone { get; set; }
        public Guid Guid { get; set; }
        public FindUserRequest()
        {
            MessageType = MessageTypes.FindUserRequest;
        }

        public FindUserRequest(string phone) : this()
        {
            Phone = phone;
        }

        public FindUserRequest(Guid guid) : this()
        {
            Guid = guid;
            Phone = null;
        }
    }
}
