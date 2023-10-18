using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat = RozetkaAPI.ModelsRozetka.Chat;

namespace Models.Messages.Requests
{
    public class NewMessageRozetkaRequest : Request
    {
        public UserRozetka UserRozetka { get; set; }
        public Chat Chat { get; set; }
        public NewMessageRozetkaRequest()
        {
            MessageType = MessageTypes.NewMessageRozetkaRequest;
        }

        public NewMessageRozetkaRequest(UserRozetka userRozetka, Chat chat) : this()
        {
            UserRozetka = userRozetka;
            Chat = chat;
        }
    }
}
