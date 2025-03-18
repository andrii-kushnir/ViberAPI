using Newtonsoft.Json;
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
        public OperTypes OperType { get; set; }

        public void SetPermission(string permission)
        {
            _permission = permission.ToUint();
        }

        public enum OperTypes
        {
            Usual = 1,
            Workflow = 2
        }
    }
}
