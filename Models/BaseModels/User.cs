using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public UserTypes UserType { get; set; }
        public Guid operatoId { get; set; } = Guid.Empty;
        public string operatoName { get; set; }

        public User() { }

        public User(Guid id) : this(id, UserTypes.Unknown) { }

        public User(Guid id, UserTypes userTypes) : this(id, null, null, userTypes) { }

        public User(Guid id, string name, string avatar, UserTypes userType)
        {
            Id = id;
            Name = name;
            Avatar = avatar;
            UserType = userType;
        }
    }

    public enum UserTypes
    {
        Unknown = 0,
        Asterium = 1,
        Viber = 2,
        Telegram = 3,
        Rozetka = 4,
        Prom = 5
    }
}
