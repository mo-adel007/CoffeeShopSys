using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Corner_Application
{
    public partial class Client_Bill : Form

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


        public Client_Bill()
        {
            InitializeComponent();


            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select * from `bill` ; ";
                
                MySqlDataAdapter mySqlDataAdapterBill = new MySqlDataAdapter { SelectCommand = cmd };
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
                cmd.CommandText = "DELETE  FROM `corner`.`bill` ; ";
                MySqlDataReader reader = cmd.ExecuteReader();
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
                cmd.CommandText = "select * FROM `corner`.`bill` ; ";
                MySqlDataAdapter mySqlDataAdapterbill = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableBill = new DataTable();
                mySqlDataAdapterbill.Fill(dTableBill);

                _connect.Close();


                foreach (DataRow rowdel in dTableBill.Rows)
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText =
                        "INSERT INTO `corner`.`close_shift`(`processT`, `price`,`proName`,`quantity`, `UserN`,`Dtime`,`Userid`,`ShiftNumber`) VALUES ('sell', '" +
                        Convert.ToDouble(rowdel["tot_price"]) + "' , '" + Convert.ToString(rowdel["pro_name"]) +
                        "' , '" + Convert.ToInt16(rowdel["quantity"]) + "' , '" +
                        label1.Text + "' , '" + now + "' , '" + Convert.ToInt16(UNid.Text) + "' , '" +
                        Convert.ToInt16(shiftNum.Text) + "') ; ";
                    MySqlDataReader reader3 = cmd.ExecuteReader();
                    _connect.Close();


                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText =
                        "INSERT INTO `corner`.`close_day`(`processT`, `price`,`proName`,`quantity`, `UserN`,`Dtime`,`Userid`,`ShiftNumber`) VALUES ('sell', '" +
                        Convert.ToDouble(rowdel["tot_price"]) + "' , '" + Convert.ToString(rowdel["pro_name"]) +
                        "' , '" + Convert.ToInt16(rowdel["quantity"]) + "' , '" +
                        label1.Text + "' , '" + now + "' , '" + Convert.ToInt16(UNid.Text) + "' , '" +
                        Convert.ToInt16(shiftNum.Text) + "') ; ";
                    MySqlDataReader reader2 = cmd.ExecuteReader();
                    _connect.Close();

                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText =
                        "INSERT INTO `corner`.`product_process`(`User_Name`,`Process_type`,`Product_name`, `quantity`,`price`,`DateTime`,`IdProduct`,`UserId`) VALUES ('" + label1.Text +"', 'sell' , '" +
                        Convert.ToString(rowdel["pro_name"]) + "' , '" + Convert.ToInt16(rowdel["quantity"]) + "' , '" + Convert.ToDouble(rowdel["tot_price"]) + "' ,'"+
                        now + "' , '" + Convert.ToInt16(rowdel["IdProduct"])  + "' , '" + Convert.ToInt16(UNid.Text) +  "') ; ";
                    MySqlDataReader reader1 = cmd.ExecuteReader();
                    _connect.Close();

                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText =
                        "UPDATE `corner`.`product` SET `Store` = '"+ Convert.ToInt16(rowdel ["Store"]) +"' WHERE Product_id = '" + Convert.ToInt16(rowdel["IDproduct"]) + "' ;";

                    MySqlDataReader reader4 = cmd.ExecuteReader();
                    _connect.Close();

                }


                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "DELETE  FROM `corner`.`bill` ; ";
                MySqlDataReader reader = cmd.ExecuteReader();
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
