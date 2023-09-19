using Models.Messages.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Responses
{
    public class PoolsListResponse : Response
    {
        public List<Pool> Pools { get; set; }
        public string Result { get; set; }
        public PoolsListResponse()
        {
            MessageType = MessageTypes.PoolsListResponse;
        }

        public PoolsListResponse(PoolsListRequest request, string result, List<Pool> pools = null) : this()
        {
            RequestId = request.Id;
            Result = result;
            Pools = pools;
        }
    }
}
