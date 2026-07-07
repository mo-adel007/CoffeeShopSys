using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Corner_Application
{
    public partial class Client_Bill : Form

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


        public Client_Bill()
        {
            InitializeComponent();


            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from bill;";

                SQLiteDataAdapter mySqlDataAdapterBill = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableBill = new DataTable();
                mySqlDataAdapterBill.Fill(dTableBill);

                _connect.Close();
                dataGridView1.DataSource = dTableBill;
                double total=0;
                foreach (DataRow rowdel in dTableBill.Rows)
                {
                    total = total + Convert.ToDouble( rowdel["tot_price"] );
                }
                totPrice.Text = Convert.ToString(total);
                
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "DELETE FROM bill;";
                cmd.ExecuteNonQuery();
                _connect.Close();
                this.Hide();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //اضافة الفاتورة 
        private void button5_Click(object sender, EventArgs e)
        {

            string now;
            now = DateTime.Now.ToString("s");
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "select * from bill;";
                SQLiteDataAdapter mySqlDataAdapterbill = new SQLiteDataAdapter {SelectCommand = cmd};
                DataTable dTableBill = new DataTable();
                mySqlDataAdapterbill.Fill(dTableBill);

                _connect.Close();


                foreach (DataRow rowdel in dTableBill.Rows)
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.Parameters.Clear();
                    cmd.CommandText =
                        "INSERT INTO close_shift (processT, price, proName, quantity, UserN, Dtime, Userid, ShiftNumber) VALUES ('sell', @price, @proName, @quantity, @userN, @dtime, @userid, @shiftNumber);";
                    cmd.Parameters.AddWithValue("@price", Convert.ToDouble(rowdel["tot_price"]));
                    cmd.Parameters.AddWithValue("@proName", Convert.ToString(rowdel["pro_name"]));
                    cmd.Parameters.AddWithValue("@quantity", Convert.ToInt16(rowdel["quantity"]));
                    cmd.Parameters.AddWithValue("@userN", label1.Text);
                    cmd.Parameters.AddWithValue("@dtime", now);
                    cmd.Parameters.AddWithValue("@userid", Convert.ToInt16(UNid.Text));
                    cmd.Parameters.AddWithValue("@shiftNumber", Convert.ToInt16(shiftNum.Text));
                    cmd.ExecuteNonQuery();
                    _connect.Close();


                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.Parameters.Clear();
                    cmd.CommandText =
                        "INSERT INTO close_day (processT, price, proName, quantity, UserN, Dtime, Userid, ShiftNumber) VALUES ('sell', @price, @proName, @quantity, @userN, @dtime, @userid, @shiftNumber);";
                    cmd.Parameters.AddWithValue("@price", Convert.ToDouble(rowdel["tot_price"]));
                    cmd.Parameters.AddWithValue("@proName", Convert.ToString(rowdel["pro_name"]));
                    cmd.Parameters.AddWithValue("@quantity", Convert.ToInt16(rowdel["quantity"]));
                    cmd.Parameters.AddWithValue("@userN", label1.Text);
                    cmd.Parameters.AddWithValue("@dtime", now);
                    cmd.Parameters.AddWithValue("@userid", Convert.ToInt16(UNid.Text));
                    cmd.Parameters.AddWithValue("@shiftNumber", Convert.ToInt16(shiftNum.Text));
                    cmd.ExecuteNonQuery();
                    _connect.Close();

                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.Parameters.Clear();
                    cmd.CommandText =
                        "INSERT INTO product_process (User_Name, Process_type, Product_name, quantity, price, \"DateTime\", IdProduct, UserId) VALUES (@userName, 'sell', @productName, @quantity, @price, @dateTime, @idProduct, @userId);";
                    cmd.Parameters.AddWithValue("@userName", label1.Text);
                    cmd.Parameters.AddWithValue("@productName", Convert.ToString(rowdel["pro_name"]));
                    cmd.Parameters.AddWithValue("@quantity", Convert.ToInt16(rowdel["quantity"]));
                    cmd.Parameters.AddWithValue("@price", Convert.ToDouble(rowdel["tot_price"]));
                    cmd.Parameters.AddWithValue("@dateTime", now);
                    cmd.Parameters.AddWithValue("@idProduct", Convert.ToInt16(rowdel["IdProduct"]));
                    cmd.Parameters.AddWithValue("@userId", Convert.ToInt16(UNid.Text));
                    cmd.ExecuteNonQuery();
                    _connect.Close();

                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.Parameters.Clear();
                    cmd.CommandText =
                        "UPDATE product SET Store = @store WHERE Product_id = @productId;";
                    cmd.Parameters.AddWithValue("@store", Convert.ToInt16(rowdel["Store"]));
                    cmd.Parameters.AddWithValue("@productId", Convert.ToInt16(rowdel["IDproduct"]));
                    cmd.ExecuteNonQuery();
                    _connect.Close();

                }


                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "DELETE FROM bill;";
                cmd.ExecuteNonQuery();
                _connect.Close();
                
            this.Hide();
        }

        catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
    }
}
