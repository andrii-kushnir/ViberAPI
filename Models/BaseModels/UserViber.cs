using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserViber : User
    {
        public string idViber { get; set; }
        public string language { get; set; }
        public string country { get; set; }
        public string primary_device_os { get; set; }
        public string device_type { get; set; }
        public string phone { get; set; }
        public List<ChatMessage> messageList { get; set; }
        public bool subscribed { get; set; }
        public InviteType inviteType { get; set; }
        public DateTime dateCreate { get; set; }
        public string buhnetName { get; set; }
        public int codep { get; set; }

        public UserViber() : base() { }

        public UserViber(Guid id, string name, string avatar, UserTypes userType) : base(id, name, avatar, userType) { }
    }

    public enum InviteType
    {
        Unknown = 0,
        DirectLink = 1,
        Pool = 2,
        Site = 3,
        Buhnet = 4,
        Worker = 5,
        WorkerDriver = 6
    }
}
