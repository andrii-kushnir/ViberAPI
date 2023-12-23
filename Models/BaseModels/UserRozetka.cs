using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserRozetka : User
    {
        public int chat_id { get; set; } // це поле є головним індентифікаторм в Розетці, тому що один клієнт може мати декілька чатів
        public int user_id { get; set; }
    }
}
