using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    public interface IRequest
    {
        Guid Id { get; set; }
    }

    public class Request : Message, IRequest
    {
        public Guid Id { get; set; }

        protected Request()
        {
            Id = Guid.NewGuid();
        }
    }
}
