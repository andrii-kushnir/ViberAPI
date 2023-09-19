using Models.Messages.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Responses
{
    public class FindOperatorResponse : Response
    {
        public User User { get; set; }
        public FindOperatorResponse()
        {
            MessageType = MessageTypes.FindOperatorResponse;
        }

        public FindOperatorResponse(FindOperatorRequest request, User user) : this()
        {
            RequestId = request.Id;
            User = user;
        }

        public FindOperatorResponse(User user) : this()
        {
            RequestId = Guid.Empty;
            User = user;
        }
    }
}
