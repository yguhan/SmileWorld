namespace SimpleLoginClient
{
    partial class Form2
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.UserData = new System.Windows.Forms.TextBox();
            this.UserStatus = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.chatLog = new System.Windows.Forms.TextBox();
            this.msgInput = new System.Windows.Forms.TextBox();
            this.sendMsg = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.USER = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(493, 278);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // UserData
            // 
            this.UserData.Location = new System.Drawing.Point(511, 12);
            this.UserData.Name = "UserData";
            this.UserData.Size = new System.Drawing.Size(203, 21);
            this.UserData.TabIndex = 1;
            // 
            // UserStatus
            // 
            this.UserStatus.Location = new System.Drawing.Point(511, 39);
            this.UserStatus.Name = "UserStatus";
            this.UserStatus.Size = new System.Drawing.Size(203, 21);
            this.UserStatus.TabIndex = 2;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(511, 66);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(203, 21);
            this.textBox3.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(511, 296);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(203, 42);
            this.button1.TabIndex = 5;
            this.button1.Text = "선택 게임방 입장";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(511, 392);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(203, 42);
            this.button2.TabIndex = 6;
            this.button2.Text = "게임 종료";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chatLog
            // 
            this.chatLog.AccessibleDescription = "chatLog";
            this.chatLog.AccessibleName = "chagLog";
            this.chatLog.Location = new System.Drawing.Point(12, 296);
            this.chatLog.Multiline = true;
            this.chatLog.Name = "chatLog";
            this.chatLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatLog.Size = new System.Drawing.Size(493, 115);
            this.chatLog.TabIndex = 7;
            // 
            // msgInput
            // 
            this.msgInput.Location = new System.Drawing.Point(12, 415);
            this.msgInput.Name = "msgInput";
            this.msgInput.Size = new System.Drawing.Size(412, 21);
            this.msgInput.TabIndex = 8;
            // 
            // sendMsg
            // 
            this.sendMsg.AccessibleDescription = "sendMsg";
            this.sendMsg.AccessibleName = "sendMsg";
            this.sendMsg.Location = new System.Drawing.Point(430, 415);
            this.sendMsg.Name = "sendMsg";
            this.sendMsg.Size = new System.Drawing.Size(75, 23);
            this.sendMsg.TabIndex = 10;
            this.sendMsg.Text = "전송";
            this.sendMsg.UseVisualStyleBackColor = true;
            this.sendMsg.Click += new System.EventHandler(this.sendMsg_Click);
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.USER});
            this.listView2.Location = new System.Drawing.Point(511, 93);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(203, 197);
            this.listView2.TabIndex = 0;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged);
            // 
            // USER
            // 
            this.USER.Text = "USER";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(511, 344);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(203, 42);
            this.button3.TabIndex = 11;
            this.button3.Text = "새로운 방 개설";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 450);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.sendMsg);
            this.Controls.Add(this.msgInput);
            this.Controls.Add(this.chatLog);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.UserStatus);
            this.Controls.Add(this.UserData);
            this.Controls.Add(this.listView1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TextBox UserData;
        private System.Windows.Forms.TextBox UserStatus;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button sendMsg;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader USER;
        public System.Windows.Forms.TextBox chatLog;
        public System.Windows.Forms.TextBox msgInput;
        private System.Windows.Forms.Button button3;
    }
}