using PromAPI.ModelsProm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromAPI.Models
{
    public class MessagesHistory
    {
        public string status { get; set; }
        public DataMessagesHistory data { get; set; }
}

    public class DataMessagesHistory
    {
        public List<Message> messages { get; set; }
    }
}
