using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class PoolsListRequest : Request
    {
        public PoolsListRequest()
        {
            MessageType = MessageTypes.PoolsListRequest;
        }
    }
}
