using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromAPI.Models
{
    public class SendMessage
    {
        public string status { get; set; }
        public DataSendMessage data { get; set; }
    }

    public class DataSendMessage
    {
        public int message_id { get; set; }
    }

}
