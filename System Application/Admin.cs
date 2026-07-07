using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Corner_Application
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void AsUser_Click(object sender, EventArgs e)
        {
            worker frm = new worker();
            frm.label2.Text = DTime.Text;
            frm.label3.Text = UserName.Text;
            frm.UNameID.Text = UserID.Text;
            frm.button4.Visible = true;
            frm.shiftNu.Text = shiftNu.Text;
            frm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginForm frm = new LoginForm();
            frm.Show();
            this.Hide();
        }

        private void AsAdmin_Click(object sender, EventArgs e)
        {
            
            
            Control_Panel frm = new Control_Panel();
            frm.label1.Text = DTime.Text;
            frm.label2.Text = UserName.Text;
            frm.label3.Text = UserID.Text;
            
            frm.Show();
            this.Hide();
        }
    }
}
