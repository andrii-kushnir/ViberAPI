
namespace Arsenium
{
    partial class FindFromPhoneWin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindFromPhoneWin));
            this.lbFindPhone = new System.Windows.Forms.Label();
            this.tbFind = new System.Windows.Forms.TextBox();
            this.bFind = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbFindPhone
            // 
            this.lbFindPhone.AutoSize = true;
            this.lbFindPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbFindPhone.Location = new System.Drawing.Point(21, 16);
            this.lbFindPhone.MaximumSize = new System.Drawing.Size(260, 0);
            this.lbFindPhone.Name = "lbFindPhone";
            this.lbFindPhone.Size = new System.Drawing.Size(218, 16);
            this.lbFindPhone.TabIndex = 0;
            this.lbFindPhone.Text = "Введіть номер телефону клієта:";
            // 
            // tbFind
            // 
            this.tbFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbFind.Location = new System.Drawing.Point(24, 37);
            this.tbFind.Name = "tbFind";
            this.tbFind.Size = new System.Drawing.Size(242, 22);
            this.tbFind.TabIndex = 1;
            this.tbFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFind_KeyPress);
            // 
            // bFind
            // 
            this.bFind.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bFind.BackgroundImage")));
            this.bFind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bFind.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bFind.Location = new System.Drawing.Point(108, 71);
            this.bFind.Name = "bFind";
            this.bFind.Size = new System.Drawing.Size(75, 23);
            this.bFind.TabIndex = 2;
            this.bFind.Text = "Find";
            this.bFind.UseVisualStyleBackColor = true;
            this.bFind.Click += new System.EventHandler(this.bFind_Click);
            // 
            // FindFromPhoneWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 105);
            this.Controls.Add(this.bFind);
            this.Controls.Add(this.tbFind);
            this.Controls.Add(this.lbFindPhone);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FindFromPhoneWin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Пошук по номеру телефону";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbFindPhone;
        private System.Windows.Forms.TextBox tbFind;
        private System.Windows.Forms.Button bFind;
    }
}