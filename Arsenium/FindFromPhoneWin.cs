using Models.Messages.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arsenium
{
    public partial class FindFromPhoneWin : Form
    {
        public FindFromPhoneWin(string phone = null)
        {
            InitializeComponent();
            if (phone != null)
                tbFind.Text = phone;
        }

        private void bFind_Click(object sender, EventArgs e)
        {
            SendFindRequest();
        }

        private void tbFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                SendFindRequest();
            }
        }

        private void SendFindRequest()
        {
            var findText = Regex.Replace(tbFind.Text, @"[ A-Za-z(),.+-]+", "");
            if (findText.Length >= 10)
                if (findText.Length >= 12)
                {
                    var phone = findText.Substring(findText.Length - 12, 12);
                    Program.Session.Send(new FindUserRequest(phone));
                    Close();
                }
                else
                {
                    var phone = "38" + findText.Substring(findText.Length - 10, 10);
                    Program.Session.Send(new FindUserRequest(phone));
                    Close();
                }
        }
    }
}
