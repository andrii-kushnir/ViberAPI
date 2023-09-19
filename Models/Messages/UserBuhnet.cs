using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    public class UserBuhnet
    {
        public int status { get; set; }
        public string status_message { get; set; }
        public string chat_hostname { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public static string[] Oplata = { "Невідомо", "Наложений платіж", "Готівка(наш магазин)", "Рахунок(Банк)", "Liqpay платіж", "WayForPay платіж" };
        public static string[] Dodopl = { "Невідомо", "Немає", "Готівка(наш магазин)", "Рахунок(Банк)", "Liqpay платіж", "WayForPay платіж" };

        public int proforma { get; set; }
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public string name { get; set; }
        public string ov { get; set; }
        public decimal kol { get; set; }
        public decimal cena_r { get; set; }
    }
}
