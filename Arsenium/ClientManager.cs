using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arsenium
{
    public class ClientManager
    {
        private static readonly List<Client> _clients = new List<Client>();
        public static UserArsenium Myself;

        public static List<Client> GetAllClients()
        {
            return _clients;
        }

        public static void Add(Client client)
        {
            _clients.Add(client);
        }

        public static void Remove(Client client)
        {
            _clients.Remove(client);
        }

        public static void SetNodeBlink(Client client)
        {
            client.Blinking = BlinkType.Hot;
        }

        public static void SetNodeUnblink(Client client)
        {
            client.Blinking = BlinkType.Normal;
            client.Node.BackColor = Color.White;
        }
    }
}
