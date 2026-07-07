namespace Corner_Application
{
    partial class Admin
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
            this.AsUser = new System.Windows.Forms.Button();
            this.AsAdmin = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.DTime = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.Label();
            this.UserID = new System.Windows.Forms.Label();
            this.shiftNu = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AsUser
            // 
            this.AsUser.Font = new System.Drawing.Font("Tahoma", 40F);
            this.AsUser.Location = new System.Drawing.Point(549, 228);
            this.AsUser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AsUser.Name = "AsUser";
            this.AsUser.Size = new System.Drawing.Size(482, 174);
            this.AsUser.TabIndex = 0;
            this.AsUser.Text = "As User";
            this.AsUser.UseVisualStyleBackColor = true;
            this.AsUser.Click += new System.EventHandler(this.AsUser_Click);
            // 
            // AsAdmin
            // 
            this.AsAdmin.Font = new System.Drawing.Font("Tahoma", 40F);
            this.AsAdmin.Location = new System.Drawing.Point(549, 527);
            this.AsAdmin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AsAdmin.Name = "AsAdmin";
            this.AsAdmin.Size = new System.Drawing.Size(482, 174);
            this.AsAdmin.TabIndex = 1;
            this.AsAdmin.Text = "As Admin";
            this.AsAdmin.UseVisualStyleBackColor = true;
            this.AsAdmin.Click += new System.EventHandler(this.AsAdmin_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.button1.Location = new System.Drawing.Point(28, 793);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 43);
            this.button1.TabIndex = 2;
            this.button1.Text = "Logout";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DTime
            // 
            this.DTime.AutoSize = true;
            this.DTime.Location = new System.Drawing.Point(838, 793);
            this.DTime.Name = "DTime";
            this.DTime.Size = new System.Drawing.Size(67, 17);
            this.DTime.TabIndex = 3;
            this.DTime.Text = "Date time";
            this.DTime.Visible = false;
            // 
            // UserName
            // 
            this.UserName.AutoSize = true;
            this.UserName.Location = new System.Drawing.Point(939, 791);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(72, 17);
            this.UserName.TabIndex = 4;
            this.UserName.Text = "user name";
            this.UserName.Visible = false;
            // 
            // UserID
            // 
            this.UserID.AutoSize = true;
            this.UserID.Location = new System.Drawing.Point(1037, 790);
            this.UserID.Name = "UserID";
            this.UserID.Size = new System.Drawing.Size(49, 17);
            this.UserID.TabIndex = 5;
            this.UserID.Text = "User id";
            this.UserID.Visible = false;
            // 
            // shiftNu
            // 
            this.shiftNu.AutoSize = true;
            this.shiftNu.Location = new System.Drawing.Point(773, 793);
            this.shiftNu.Name = "shiftNu";
            this.shiftNu.Size = new System.Drawing.Size(42, 17);
            this.shiftNu.TabIndex = 10;
            this.shiftNu.Text = "shiftN";
            this.shiftNu.Visible = false;
            // 
            // Admin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1580, 863);
            this.Controls.Add(this.shiftNu);
            this.Controls.Add(this.UserID);
            this.Controls.Add(this.UserName);
            this.Controls.Add(this.DTime);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.AsAdmin);
            this.Controls.Add(this.AsUser);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Admin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Admin";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AsUser;
        private System.Windows.Forms.Button AsAdmin;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Label DTime;
        public System.Windows.Forms.Label UserName;
        public System.Windows.Forms.Label UserID;
        public System.Windows.Forms.Label shiftNu;
    }
}