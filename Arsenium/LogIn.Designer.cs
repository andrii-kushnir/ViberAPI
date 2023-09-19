
namespace Arsenium
{
    partial class LogIn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogIn));
            this._btExit = new System.Windows.Forms.Button();
            this._btLogIn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._tbPassword = new System.Windows.Forms.TextBox();
            this._tbLogin = new System.Windows.Forms.TextBox();
            this.lVersion = new System.Windows.Forms.Label();
            this._cbAutoload = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _btExit
            // 
            this._btExit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_btExit.BackgroundImage")));
            this._btExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._btExit.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this._btExit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._btExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
            this._btExit.Location = new System.Drawing.Point(162, 141);
            this._btExit.Name = "_btExit";
            this._btExit.Size = new System.Drawing.Size(92, 46);
            this._btExit.TabIndex = 13;
            this._btExit.Text = "Вихід";
            this._btExit.UseVisualStyleBackColor = true;
            this._btExit.Click += new System.EventHandler(this._btExit_Click);
            // 
            // _btLogIn
            // 
            this._btLogIn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_btLogIn.BackgroundImage")));
            this._btLogIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._btLogIn.FlatAppearance.BorderSize = 0;
            this._btLogIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._btLogIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
            this._btLogIn.Location = new System.Drawing.Point(44, 141);
            this._btLogIn.Name = "_btLogIn";
            this._btLogIn.Size = new System.Drawing.Size(92, 46);
            this._btLogIn.TabIndex = 11;
            this._btLogIn.Text = "Вхід";
            this._btLogIn.UseVisualStyleBackColor = true;
            this._btLogIn.Click += new System.EventHandler(this._btLogIn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.label2.Location = new System.Drawing.Point(22, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 24);
            this.label2.TabIndex = 10;
            this.label2.Text = "Пароль:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.label1.Location = new System.Drawing.Point(41, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 24);
            this.label1.TabIndex = 9;
            this.label1.Text = "Логін:";
            // 
            // _tbPassword
            // 
            this._tbPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this._tbPassword.Location = new System.Drawing.Point(109, 84);
            this._tbPassword.Name = "_tbPassword";
            this._tbPassword.Size = new System.Drawing.Size(167, 29);
            this._tbPassword.TabIndex = 8;
            this._tbPassword.UseSystemPasswordChar = true;
            this._tbPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this._tbPassword_KeyDown);
            // 
            // _tbLogin
            // 
            this._tbLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this._tbLogin.Location = new System.Drawing.Point(109, 35);
            this._tbLogin.Name = "_tbLogin";
            this._tbLogin.Size = new System.Drawing.Size(167, 29);
            this._tbLogin.TabIndex = 7;
            this._tbLogin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._tbLogin_KeyPress);
            // 
            // lVersion
            // 
            this.lVersion.AutoSize = true;
            this.lVersion.Location = new System.Drawing.Point(166, 9);
            this.lVersion.Name = "lVersion";
            this.lVersion.Size = new System.Drawing.Size(0, 13);
            this.lVersion.TabIndex = 14;
            // 
            // _cbAutoload
            // 
            this._cbAutoload.AutoSize = true;
            this._cbAutoload.Checked = true;
            this._cbAutoload.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbAutoload.Location = new System.Drawing.Point(12, 203);
            this._cbAutoload.Name = "_cbAutoload";
            this._cbAutoload.Size = new System.Drawing.Size(173, 17);
            this._cbAutoload.TabIndex = 15;
            this._cbAutoload.Text = "Додати в Автозавантаження";
            this._cbAutoload.UseVisualStyleBackColor = true;
            // 
            // LogIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 232);
            this.Controls.Add(this._cbAutoload);
            this.Controls.Add(this.lVersion);
            this.Controls.Add(this._btExit);
            this.Controls.Add(this._btLogIn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._tbPassword);
            this.Controls.Add(this._tbLogin);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogIn";
            this.Text = "LogIn";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogIn_FormClosed);
            this.Load += new System.EventHandler(this.LogIn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btExit;
        private System.Windows.Forms.Button _btLogIn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _tbPassword;
        private System.Windows.Forms.TextBox _tbLogin;
        private System.Windows.Forms.Label lVersion;
        private System.Windows.Forms.CheckBox _cbAutoload;
    }
}