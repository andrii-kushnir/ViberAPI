using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;
using Chat = RozetkaAPI.ModelsRozetka.Chat;

namespace Arsenium
{
    public class Client : IClient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public UserTypes Type { get; set; }
        public TreeNode Node { get; set; }
        public string NameShow { get; set; }
        public Image AvatarImage
        {
            get
            {
                if (Avatar == null)
                    return null;
                if (_avatar == null)
                    switch (Type)
                    {
                        case UserTypes.Viber:
                            _avatar = Avatar.GetIconFromWebImage();
                            break;
                        case UserTypes.Asterium:
                            _avatar = ("https://viber.ars.ua/" + Avatar).GetIconFromWebImage();
                            break;
                    }
                return _avatar;
            }
        }
        private Image _avatar;
        public BlinkType Blinking { get; set; }
        public ClientSessionWin ClientWindow { get; set; }
        public ClientRozetkaWin RozetkaWindow { get; set; }
        public ClientPromWin PromWindow { get; set; }
        public PopUp PopUpWindow { get; set; }
        public PopUp PopUpRozetkaWindow { get; set; }
        public PopUp PopUpPromWindow { get; set; }
        public bool WaitOperator { get; set; } = false;
        public DetailsInfo DetailsInfo { get; set; }
        public Chat ChatRozetka { get; set; }
        public List<PromAPI.ModelsProm.Message> MessagesProm { get; set; }

        public Client()
        {
        }

        public Client(Guid id, string name, string avatar, UserTypes clientType)
        {
            Id = id;
            NameShow = name; // Можливо використаю в майбутньому
            Name = name;
            Avatar = avatar;
            Type = clientType;
            Blinking = BlinkType.Normal;
            DetailsInfo = null;
        }

        //public static Client TestViberClient1()
        //{
        //    var testclient = new Client(Guid.NewGuid(), "Andrii Kushnir", "https://media-direct.cdn.viber.com/download_photo?dlid=Lb7YahbdZ4c0c81NcZkS2ZAS3ztXoykkfFnDDAHYz6V522g80Us1BIZo67RUpFWaJqYuYrBpvYy6w3f5SZBJUarqPKN0VlMyJuzVkO_ypCqYcmoAwOdZCJg7U6ep4dDNCWXp2A&fltp=jpg&imsz=0000", UserTypes.Viber);
        //    //"A1uI5qBer7dkTSZX1h4aKg=="
        //    return testclient;
        //}

        //public static Client TestClient()
        //{
        //    var testclient = new Client(Guid.NewGuid(), "Інший Asteriun", "https://viber.ars.ua/1_2.png", UserTypes.Asterium);
        //    return testclient;
        //}
    }

    public enum BlinkType
    {
        Normal = 0,
        Hot = 1,
        NotHot = 2,
    }

    public class DetailsInfo
    {
        public string Phone { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Primary_device_os { get; set; }
        public string Device_type { get; set; }
        public List<ChatMessage> MessageList { get; set; }
        public bool Subscribed { get; set; }
        public InviteType InviteType { get; set; }
        public string OperatorName { get; set; }
        public string BuhnetName { get; set; }

        public DetailsInfo(UserViber userViber)
        {
            Phone = userViber.phone;
            Language = userViber.language;
            Country = userViber.country;
            Primary_device_os = userViber.primary_device_os;
            Device_type = userViber.device_type;
            MessageList = userViber.messageList;
            Subscribed = userViber.subscribed;
            InviteType = userViber.inviteType;
            OperatorName = userViber.operatoName;
            BuhnetName = userViber.buhnetName;
        }
    }
}