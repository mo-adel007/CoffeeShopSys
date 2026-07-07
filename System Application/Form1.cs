using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace Corner_Application
{
    public partial class LoginForm : Form
    {
        private MySqlConnection _connect;
        MySqlCommand cmd = new MySqlCommand();

        void dbConnection()
        {
            try
            {
                _connect = new MySqlConnection(Program.Constring);
                _connect.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void txtUserName_keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                textBox2.Focus();
        }

        private void txtPass_keypass(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string dt;

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("please enter your username.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("please enter your password.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            try
            {
                dbConnection();                
                cmd.Connection = _connect;
                cmd.CommandText = "Select * from `login` where 	username='"+textBox1.Text +"' and Pass ='"+ textBox2.Text+"'; ";

                //MySqlDataReader reader = cmd.ExecuteReader();
                MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter { SelectCommand = cmd };
                DataTable dTableLogin = new DataTable();
                mySqlDataAdapterlogin.Fill(dTableLogin);
                
                _connect.Close();
                if (dTableLogin.Rows.Count == 1)
                {
                    string now;
                    now = DateTime.Now.ToString("s");
                    dt = Convert.ToString(now);
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "Select * from `user_shift` ; " ;
                    MySqlDataAdapter mySqlDataAdapterUserShify = new MySqlDataAdapter { SelectCommand = cmd };
                    DataTable dTableUserShift = new DataTable();
                    mySqlDataAdapterUserShify.Fill(dTableUserShift);
                    _connect.Close();
                    int maxShiftN = 0;
                    string LastUser = "Em";
                    int sh_nu;
                    if (dTableUserShift.Rows.Count >= 1)
                    {
                        foreach (DataRow rowdel in dTableUserShift.Rows)
                        {
                            sh_nu =  Convert.ToInt16( rowdel["ShiftNumber"]);
                            if (sh_nu > maxShiftN)
                            {
                                maxShiftN = sh_nu;
                                LastUser = Convert.ToString( rowdel["UserName"]);
                            }
                        }
                    }

                    foreach (DataRow rowdel in dTableLogin.Rows)
                    {
                        int sh;
                        if (Convert.ToString(rowdel["time_work"]) == "admin")
                        {
                            Admin frm = new Admin();
                            frm.DTime.Text = dt;
                            frm.UserName.Text = Convert.ToString(rowdel["username"]);
                            frm.UserID.Text = Convert.ToString(rowdel["id"]);
                            frm.shiftNu.Text = "0";
                            frm.Show();
                        }
                        else
                        {
                            if (Convert.ToString(rowdel["userName"]) == LastUser)
                            { sh = maxShiftN; }
                            else
                            {
                                sh = maxShiftN + 1;
                                dbConnection();
                                cmd.Connection = _connect;
                                cmd.CommandText = "INSERT INTO `corner`.`user_shift`( `UserName`, `ShiftNumber`) VALUES ('" +
                                    Convert.ToString(rowdel["userName"]) +  "' , '"+ sh +"');";
                                MySqlDataReader reader3 = cmd.ExecuteReader();
                                _connect.Close(); 
                                
                            }


                            worker frm = new worker();
                            frm.label2.Text = dt;
                            frm.label3.Text = Convert.ToString(rowdel["username"]);
                            frm.UNameID.Text = Convert.ToString(rowdel["id"]);
                            frm.shiftNu.Text = Convert.ToString(sh);
                            frm.button1.Enabled = false;
                            frm.Show();
                        }


                        //MessageBox.Show(dt, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();

                    }

                }
                else
                {
                    MessageBox.Show("user name or password is incorrect", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
             
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


      

    }
}
