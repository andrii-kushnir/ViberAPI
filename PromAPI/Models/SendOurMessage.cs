using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromAPI.Models
{
    public class SendOurMessage
    {
        public string room_ident { get; set; }
        public string user_id { get; set; }
        public string body { get; set; }
    }
}
