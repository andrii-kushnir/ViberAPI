
namespace Arsenium
{
    partial class MainWin
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
            this.treeMain = new System.Windows.Forms.TreeView();
            this.btAway = new System.Windows.Forms.Button();
            this.tmLastInput = new System.Windows.Forms.Timer(this.components);
            this.mMainMenu = new System.Windows.Forms.MenuStrip();
            this.miDo = new System.Windows.Forms.ToolStripMenuItem();
            this.miFind = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miReConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miAdmin = new System.Windows.Forms.ToolStripMenuItem();
            this.miPools = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mMainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeMain
            // 
            this.treeMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeMain.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeMain.Location = new System.Drawing.Point(2, 27);
            this.treeMain.Name = "treeMain";
            this.treeMain.Size = new System.Drawing.Size(290, 567);
            this.treeMain.TabIndex = 0;
            this.treeMain.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeMain_NodeMouseDoubleClick);
            // 
            // btAway
            // 
            this.btAway.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btAway.BackgroundImage")));
            this.btAway.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btAway.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btAway.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAway.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btAway.Location = new System.Drawing.Point(88, 600);
            this.btAway.Name = "btAway";
            this.btAway.Size = new System.Drawing.Size(119, 28);
            this.btAway.TabIndex = 6;
            this.btAway.Text = "Відійти";
            this.btAway.UseVisualStyleBackColor = true;
            this.btAway.Click += new System.EventHandler(this.btAway_Click);
            // 
            // tmLastInput
            // 
            this.tmLastInput.Interval = 1000;
            this.tmLastInput.Tick += new System.EventHandler(this.tmLastInput_Tick);
            // 
            // mMainMenu
            // 
            this.mMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDo,
            this.miAbout,
            this.miExit});
            this.mMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mMainMenu.Name = "mMainMenu";
            this.mMainMenu.Size = new System.Drawing.Size(295, 24);
            this.mMainMenu.TabIndex = 9;
            this.mMainMenu.Text = "menuStrip1";
            // 
            // miDo
            // 
            this.miDo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFind,
            this.toolStripSeparator2,
            this.miReConnect,
            this.toolStripSeparator3,
            this.miAdmin,
            this.toolStripSeparator1,
            this.miUpdate});
            this.miDo.Name = "miDo";
            this.miDo.Size = new System.Drawing.Size(33, 20);
            this.miDo.Text = "Дії";
            // 
            // miFind
            // 
            this.miFind.Name = "miFind";
            this.miFind.Size = new System.Drawing.Size(233, 22);
            this.miFind.Text = "Пошук клієнта";
            this.miFind.Click += new System.EventHandler(this.miFind_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(230, 6);
            // 
            // miReConnect
            // 
            this.miReConnect.Name = "miReConnect";
            this.miReConnect.Size = new System.Drawing.Size(233, 22);
            this.miReConnect.Text = "Переконектитись до сервера";
            this.miReConnect.Click += new System.EventHandler(this.miReConnect_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(230, 6);
            // 
            // miAdmin
            // 
            this.miAdmin.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPools});
            this.miAdmin.Name = "miAdmin";
            this.miAdmin.Size = new System.Drawing.Size(233, 22);
            this.miAdmin.Text = "Адміністрування";
            // 
            // miPools
            // 
            this.miPools.Name = "miPools";
            this.miPools.Size = new System.Drawing.Size(160, 22);
            this.miPools.Text = "Відгуки клієнтів";
            this.miPools.Click += new System.EventHandler(this.miPools_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(230, 6);
            // 
            // miUpdate
            // 
            this.miUpdate.Name = "miUpdate";
            this.miUpdate.Size = new System.Drawing.Size(233, 22);
            this.miUpdate.Text = "Оновити програму";
            this.miUpdate.Click += new System.EventHandler(this.miUpdate_Click);
            // 
            // miAbout
            // 
            this.miAbout.Name = "miAbout";
            this.miAbout.Size = new System.Drawing.Size(99, 20);
            this.miAbout.Text = "Про програму";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(48, 20);
            this.miExit.Text = "Вихід";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // MainWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 635);
            this.Controls.Add(this.btAway);
            this.Controls.Add(this.treeMain);
            this.Controls.Add(this.mMainMenu);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mMainMenu;
            this.MaximizeBox = false;
            this.Name = "MainWin";
            this.Text = "ARSenium";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWin_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.mMainMenu.ResumeLayout(false);
            this.mMainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeMain;
        private System.Windows.Forms.Button btAway;
        private System.Windows.Forms.Timer tmLastInput;
        private System.Windows.Forms.MenuStrip mMainMenu;
        private System.Windows.Forms.ToolStripMenuItem miDo;
        private System.Windows.Forms.ToolStripMenuItem miAbout;
        private System.Windows.Forms.ToolStripMenuItem miReConnect;
        private System.Windows.Forms.ToolStripMenuItem miUpdate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miFind;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem miAdmin;
        private System.Windows.Forms.ToolStripMenuItem miPools;
        private System.Windows.Forms.ToolStripMenuItem miExit;
    }
}

