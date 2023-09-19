using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class ChangeTypeRequest : Request
    {
        public User User { get; set; }
        public InviteType InviteType { get; set; }
        public ChangeTypeRequest()
        {
            MessageType = MessageTypes.ChangeTypeRequest;
        }

        public ChangeTypeRequest(User user, InviteType inviteType) : this()
        {
            User = user;
            InviteType = inviteType;
        }
    }
}
