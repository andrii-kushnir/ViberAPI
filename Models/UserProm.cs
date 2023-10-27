using PromAPI.ModelsProm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserProm : User
    {
        public string room_ident { get; set; } // це поле є головним індентифікаторм в Промі, тому що один клієнт може мати декілька чатів(румів)
        public string room_id { get; set; }
        public string user_ident { get; set; }
        public string user_phone { get; set; }
        public List<Message> messages { get; set; }
    }
}
