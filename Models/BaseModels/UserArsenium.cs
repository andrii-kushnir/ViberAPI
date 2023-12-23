using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static ViberAPI.Permissions;

namespace Models
{
    public class UserArsenium : User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public uint Permission { get { return _permission; } }
        public bool Active { get; set; }
        public bool Online { get; set; }

        [JsonIgnore]
        private uint _permission = 0;
        [JsonIgnore]
        public int Codep = 0;

        public void SetPermission(string permission)
        {
            _permission = permission.ToUint();
        }
    }
}
