using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages.Requests
{
    public class LoginRequest : Request
    {
        public Guid ClientId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public LoginRequest(Guid clientId, string login, string password)
        {
            MessageType = MessageTypes.LoginRequest;
            ClientId = clientId;
            Login = login;
            Password = password;
        }
    }
}
