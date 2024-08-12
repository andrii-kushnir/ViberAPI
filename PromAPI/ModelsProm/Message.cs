using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromAPI.ModelsProm
{
    public class Message
    {
        public int id { get; set; }
        public string room_id { get; set; }
        public string room_ident { get; set; }
        public string body { get; set; }
        public DateTime date_sent { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public long? context_item_id { get; set; }
        public string context_item_image_url { get; set; }
        public string context_item_type { get; set; }
        public string user_name { get; set; }
        public string user_ident { get; set; }
        public string user_phone { get; set; }
        public int buyer_client_id { get; set; }
        public bool is_sender { get; set; }
    }

}
