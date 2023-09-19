using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arsenium
{
    public partial class ClientBuhnetInfo : Form
    {
        public ClientBuhnetInfo()
        {
            var poplata = new string[5];
            poplata[1] = "Наложений платіж";
            poplata[2] = "Готівка(наш магазин)";
            poplata[3] = "Рахунок(Банк) ";
            poplata[4] = "Liqpay платіж";
            poplata[5] = "WayForPay платіж";

            var popldod = new string[5];
            popldod[1] = "Немає";
            popldod[2] = "Готівка(наш магазин)";
            popldod[3] = "Рахунок(Банк) ";
            popldod[4] = "Liqpay платіж";
            popldod[5] = "WayForPay платіж";



            InitializeComponent();
        }
    }
}
