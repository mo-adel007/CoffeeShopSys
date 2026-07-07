namespace Corner_Application
{
    partial class Client_Bill
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.UNid = new System.Windows.Forms.Label();
            this.DTimeLogin = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.totPrice = new System.Windows.Forms.Label();
            this.billBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.shiftNum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.billBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(14, 16);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dataGridView1.Size = new System.Drawing.Size(918, 622);
            this.dataGridView1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(213, 672);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 48);
            this.button2.TabIndex = 2;
            this.button2.Text = "الغاء";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(33, 673);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(152, 48);
            this.button5.TabIndex = 5;
            this.button5.Text = "اضافة";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(866, 690);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "UName";
            this.label1.Visible = false;
            // 
            // UNid
            // 
            this.UNid.AutoSize = true;
            this.UNid.Location = new System.Drawing.Point(776, 689);
            this.UNid.Name = "UNid";
            this.UNid.Size = new System.Drawing.Size(66, 17);
            this.UNid.TabIndex = 7;
            this.UNid.Text = "UNameID";
            this.UNid.Visible = false;
            // 
            // DTimeLogin
            // 
            this.DTimeLogin.AutoSize = true;
            this.DTimeLogin.Location = new System.Drawing.Point(675, 663);
            this.DTimeLogin.Name = "DTimeLogin";
            this.DTimeLogin.Size = new System.Drawing.Size(59, 17);
            this.DTimeLogin.TabIndex = 8;
            this.DTimeLogin.Text = "DTLogin";
            this.DTimeLogin.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(405, 689);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "جنيهاً";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(569, 690);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "اجمالى المبلغ";
            // 
            // totPrice
            // 
            this.totPrice.AutoSize = true;
            this.totPrice.Location = new System.Drawing.Point(485, 690);
            this.totPrice.Name = "totPrice";
            this.totPrice.Size = new System.Drawing.Size(16, 17);
            this.totPrice.TabIndex = 11;
            this.totPrice.Text = "0";
            // 
            // billBindingSource
            // 
            this.billBindingSource.DataSource = typeof(Corner_Application.Client_Bill);
            // 
            // shiftNum
            // 
            this.shiftNum.AutoSize = true;
            this.shiftNum.Location = new System.Drawing.Point(692, 703);
            this.shiftNum.Name = "shiftNum";
            this.shiftNum.Size = new System.Drawing.Size(42, 17);
            this.shiftNum.TabIndex = 13;
            this.shiftNum.Text = "shiftN";
            this.shiftNum.Visible = false;
            // 
            // Client_Bill
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 737);
            this.ControlBox = false;
            this.Controls.Add(this.shiftNum);
            this.Controls.Add(this.totPrice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DTimeLogin);
            this.Controls.Add(this.UNid);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Client_Bill";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bill";
            //this.Load += new System.EventHandler(this.Client_Bill_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.billBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource billBindingSource;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button5;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label UNid;
        public System.Windows.Forms.Label DTimeLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label totPrice;
        public System.Windows.Forms.Label shiftNum;

    }
}