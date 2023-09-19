
namespace Arsenium
{
    partial class PopUp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopUp));
            this.lbMessage = new System.Windows.Forms.Label();
            this.btClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbMessage
            // 
            this.lbMessage.BackColor = System.Drawing.Color.AliceBlue;
            this.lbMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbMessage.Location = new System.Drawing.Point(6, 6);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(348, 88);
            this.lbMessage.TabIndex = 0;
            this.lbMessage.UseCompatibleTextRendering = true;
            this.lbMessage.Paint += new System.Windows.Forms.PaintEventHandler(this.lbMessage_Paint);
            this.lbMessage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopUp_MouseDown);
            // 
            // btClose
            // 
            this.btClose.BackColor = System.Drawing.Color.Transparent;
            this.btClose.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btClose.Image = ((System.Drawing.Image)(resources.GetObject("btClose.Image")));
            this.btClose.Location = new System.Drawing.Point(336, 0);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(24, 24);
            this.btClose.TabIndex = 1;
            this.btClose.UseVisualStyleBackColor = false;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // PopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(216)))), ((int)(((byte)(0)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(360, 100);
            this.ControlBox = false;
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.lbMessage);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopUp";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PopUp_FormClosing);
            this.Load += new System.EventHandler(this.PopUp_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PopUp_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopUp_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.Button btClose;
    }
}