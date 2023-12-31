﻿
namespace Arsenium
{
    partial class ClientRozetkaWin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientRozetkaWin));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._lItem = new System.Windows.Forms.TextBox();
            this._lOrder = new System.Windows.Forms.TextBox();
            this._lClient = new System.Windows.Forms.TextBox();
            this._lChatType = new System.Windows.Forms.TextBox();
            this._bSend = new System.Windows.Forms.Button();
            this._tbSendMessage = new System.Windows.Forms.TextBox();
            this._pTalk = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._lItem);
            this.splitContainer1.Panel1.Controls.Add(this._lOrder);
            this.splitContainer1.Panel1.Controls.Add(this._lClient);
            this.splitContainer1.Panel1.Controls.Add(this._lChatType);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._bSend);
            this.splitContainer1.Panel2.Controls.Add(this._tbSendMessage);
            this.splitContainer1.Panel2.Controls.Add(this._pTalk);
            this.splitContainer1.Size = new System.Drawing.Size(984, 761);
            this.splitContainer1.SplitterDistance = 166;
            this.splitContainer1.TabIndex = 0;
            // 
            // _lItem
            // 
            this._lItem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._lItem.Location = new System.Drawing.Point(13, 136);
            this._lItem.Name = "_lItem";
            this._lItem.ReadOnly = true;
            this._lItem.Size = new System.Drawing.Size(633, 19);
            this._lItem.TabIndex = 10;
            this._lItem.TabStop = false;
            this._lItem.Text = "Товар";
            // 
            // _lOrder
            // 
            this._lOrder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._lOrder.Location = new System.Drawing.Point(13, 102);
            this._lOrder.Name = "_lOrder";
            this._lOrder.ReadOnly = true;
            this._lOrder.Size = new System.Drawing.Size(633, 19);
            this._lOrder.TabIndex = 9;
            this._lOrder.TabStop = false;
            this._lOrder.Text = "Замовлення";
            // 
            // _lClient
            // 
            this._lClient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._lClient.Location = new System.Drawing.Point(13, 68);
            this._lClient.Name = "_lClient";
            this._lClient.ReadOnly = true;
            this._lClient.Size = new System.Drawing.Size(633, 19);
            this._lClient.TabIndex = 8;
            this._lClient.TabStop = false;
            this._lClient.Text = "Клієнт";
            // 
            // _lChatType
            // 
            this._lChatType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lChatType.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._lChatType.Location = new System.Drawing.Point(41, 7);
            this._lChatType.Multiline = true;
            this._lChatType.Name = "_lChatType";
            this._lChatType.ReadOnly = true;
            this._lChatType.Size = new System.Drawing.Size(936, 55);
            this._lChatType.TabIndex = 7;
            this._lChatType.TabStop = false;
            this._lChatType.Text = "Запитання про ...";
            // 
            // _bSend
            // 
            this._bSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bSend.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_bSend.BackgroundImage")));
            this._bSend.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._bSend.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this._bSend.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._bSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._bSend.Location = new System.Drawing.Point(827, 535);
            this._bSend.Name = "_bSend";
            this._bSend.Size = new System.Drawing.Size(145, 44);
            this._bSend.TabIndex = 2;
            this._bSend.Text = "Відіслати (Enter)";
            this._bSend.UseVisualStyleBackColor = true;
            this._bSend.Click += new System.EventHandler(this._bSend_Click);
            // 
            // _tbSendMessage
            // 
            this._tbSendMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._tbSendMessage.Location = new System.Drawing.Point(12, 445);
            this._tbSendMessage.Multiline = true;
            this._tbSendMessage.Name = "_tbSendMessage";
            this._tbSendMessage.Size = new System.Drawing.Size(809, 134);
            this._tbSendMessage.TabIndex = 1;
            this._tbSendMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._tbSendMessage_KeyPress);
            // 
            // _pTalk
            // 
            this._pTalk.AutoScroll = true;
            this._pTalk.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._pTalk.Location = new System.Drawing.Point(12, 8);
            this._pTalk.Name = "_pTalk";
            this._pTalk.Size = new System.Drawing.Size(960, 431);
            this._pTalk.TabIndex = 3;
            // 
            // ClientRozetkaWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 761);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClientRozetkaWin";
            this.Text = "ClientRozetka";
            this.Activated += new System.EventHandler(this.ClientRozetkaWin_Activated);
            this.Load += new System.EventHandler(this.ClientRozetkaWin_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel _pTalk;
        private System.Windows.Forms.TextBox _tbSendMessage;
        private System.Windows.Forms.Button _bSend;
        private System.Windows.Forms.TextBox _lClient;
        private System.Windows.Forms.TextBox _lChatType;
        private System.Windows.Forms.TextBox _lOrder;
        private System.Windows.Forms.TextBox _lItem;
    }
}