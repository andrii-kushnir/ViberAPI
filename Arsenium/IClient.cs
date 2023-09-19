using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;

namespace Arsenium
{
    public interface IClient
    {
        Guid Id { get; set; }
        TreeNode Node { get; set; }
        string NameShow { get; set; }
        UserTypes Type { get; set; }
    }
}
