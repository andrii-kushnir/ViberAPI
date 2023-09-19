using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    public interface IResponse
    {
        Guid RequestId { get; set; }
    }

    public class Response : Message, IResponse
    {
        public Guid RequestId { get; set; }
        public Response()
        {

        }

        public Response(Guid requestId)
        {
            RequestId = requestId;
        }
    }
}
