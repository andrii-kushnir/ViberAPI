
namespace Arsenium
{
    partial class PoolsWin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PoolsWin));
            this._dgvPools = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPools)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgvPools
            // 
            this._dgvPools.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvPools.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvPools.Location = new System.Drawing.Point(0, 0);
            this._dgvPools.Name = "_dgvPools";
            this._dgvPools.ReadOnly = true;
            this._dgvPools.Size = new System.Drawing.Size(1241, 591);
            this._dgvPools.TabIndex = 0;
            this._dgvPools.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgvPools_CellContentClick);
            this._dgvPools.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgvPools_CellDoubleClick);
            this._dgvPools.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this._dgvPools_CellFormatting);
            // 
            // PoolsWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 591);
            this.Controls.Add(this._dgvPools);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PoolsWin";
            this.Text = "Відгуки і оцінки клієнтів";
            this.Load += new System.EventHandler(this.Pools_Load);
            ((System.ComponentModel.ISupportInitialize)(this._dgvPools)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgvPools;
    }
}