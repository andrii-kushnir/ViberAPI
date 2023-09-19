using Models;
using Models.Messages.Requests;
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
    public partial class PoolsWin : Form
    {
        private List<Pool> _pools;

        public PoolsWin()
        {
            InitializeComponent();
        }

        public PoolsWin(List<Pool> pools) : this()
        {
            _pools = pools;
        }

        private void Pools_Load(object sender, EventArgs e)
        {
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ViberName", Name = "ViberName", HeaderText = "Клієнт" });
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Phone", Name = "Phone", HeaderText = "Телефон" });
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Operator", Name = "Operator", HeaderText = "Оператор" });
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DateCreate", Name = "DateCreate", HeaderText = "Дата відгуку" });
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StringType", Name = "StringType", HeaderText = "Тип відгуку" });
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Text", Name = "Text", HeaderText = "Текст відгуку", Width = 500 });
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", Visible = false });
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Avatar", Visible = false });
            _dgvPools.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", Visible = false });
            _dgvPools.Columns.Add(new DataGridViewButtonColumn { Name = "Do", HeaderText = "Відповідь", Text = "Відповідь", UseColumnTextForButtonValue = true });
            _dgvPools.DataSource = _pools;

            foreach (DataGridViewRow row in _dgvPools.Rows)
            {
                if (row.Cells["StringType"].Value.ToString() == Pool.complaint)
                    row.Cells["StringType"].Style.BackColor = Color.Red;
                if (row.Cells["StringType"].Value.ToString() == Pool.offer)
                    row.Cells["StringType"].Style.BackColor = Color.Green;
                if ((row.Cells["StringType"].Value.ToString() == Pool.markOperator && row.Cells["Text"].Value.ToString() == "Погано"))
                    row.Cells["Text"].Style.BackColor = Color.Red;
                if ((row.Cells["StringType"].Value.ToString() == Pool.markOperator && row.Cells["Text"].Value.ToString() == "Відмінно"))
                    row.Cells["Text"].Style.BackColor = Color.Green;
            }
            //_dgvPools.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void _dgvPools_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var pool = _pools[e.RowIndex];
            Program.Session.Send(new FindUserRequest(pool.Id));
        }

        private void _dgvPools_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell;
            try
            {
                cell = _dgvPools[e.ColumnIndex, e.RowIndex];
            }
            catch { return; }

            if (cell is DataGridViewButtonCell)
            {
                MessageBox.Show("Це ще не реалізовано");
            }
        }

        private void _dgvPools_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == _dgvPools.Columns["Text"].Index)
            {
                _dgvPools[e.ColumnIndex, e.RowIndex].ToolTipText = _pools[e.RowIndex].Text;
            }
        }
    }
}
