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
using System.Data.SQLite;


namespace Corner_Application
{
    public partial class LoginForm : Form
    {
        private SQLiteConnection _connect;
        SQLiteCommand cmd = new SQLiteCommand();

        void dbConnection()
        {
            try
            {
                _connect = new SQLiteConnection(Program.Constring);
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
            Theme.Apply(this);
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
                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from login where username = @username;";
                cmd.Parameters.AddWithValue("@username", textBox1.Text);

                SQLiteDataAdapter mySqlDataAdapterlogin = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableLogin = new DataTable();
                mySqlDataAdapterlogin.Fill(dTableLogin);

                _connect.Close();

                // Verify the typed password against the stored PBKDF2 hash (never plaintext).
                bool passwordOk = false;
                if (dTableLogin.Rows.Count == 1)
                {
                    string storedPass = Convert.ToString(dTableLogin.Rows[0]["Pass"]);
                    passwordOk = Security.Verify(textBox2.Text, storedPass);
                    // Transparently upgrade a legacy plaintext row to a hash on success.
                    if (passwordOk && Security.IsLegacyPlaintext(storedPass))
                    {
                        UpgradePasswordHash(Convert.ToString(dTableLogin.Rows[0]["id"]), textBox2.Text);
                    }
                }

                if (dTableLogin.Rows.Count == 1 && passwordOk)
                {
                    string now;
                    now = DateTime.Now.ToString("s");
                    dt = Convert.ToString(now);
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "Select * from user_shift;";
                    SQLiteDataAdapter mySqlDataAdapterUserShify = new SQLiteDataAdapter { SelectCommand = cmd };
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
                                cmd.Parameters.Clear();
                                cmd.CommandText = "INSERT INTO user_shift (UserName, ShiftNumber) VALUES (@userName, @shiftNumber);";
                                cmd.Parameters.AddWithValue("@userName", Convert.ToString(rowdel["userName"]));
                                cmd.Parameters.AddWithValue("@shiftNumber", sh);
                                cmd.ExecuteNonQuery();
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

        /// <summary>
        /// One-time migration: replace a legacy plaintext password with its PBKDF2 hash
        /// after the user authenticates successfully with it. Non-fatal on failure.
        /// </summary>
        private void UpgradePasswordHash(string userId, string plaintext)
        {
            try
            {
                using (var conn = new SQLiteConnection(Program.Constring))
                {
                    conn.Open();
                    using (var upd = new SQLiteCommand(
                        "UPDATE login SET Pass = @pass WHERE id = @id;", conn))
                    {
                        upd.Parameters.AddWithValue("@pass", Security.Hash(plaintext));
                        upd.Parameters.AddWithValue("@id", userId);
                        upd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Login still succeeds even if the re-hash can't be persisted.
            }
        }

    }
}
