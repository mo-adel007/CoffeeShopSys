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
    public partial class Control_Panel : Form
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

        public Control_Panel()
        {
            InitializeComponent();
            usersData();
            DataCBoxUsers();
            dateTimePicker1.CustomFormat = "MM/dd/yyyy hh:mm tt";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "MM/dd/yyyy hh:mm tt";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Admin frm = new Admin();
            frm.DTime.Text = label1.Text;
            frm.UserName.Text = label2.Text;
            frm.UserID.Text = label3.Text;
            frm.shiftNu.Text = "0";
            frm.Show();
            this.Hide();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch ((sender as TabControl).SelectedIndex)
            {

                case 1:
                    usersData();
                    break;
                case 2:
                    productTypeData();
                    break;
                case 3:
                    productData();
                    break;
                case 4:
                    CMonth();
                    break;


                default:
                    break;
            }
        }


        private void CMonth()
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText =
                    "SELECT  `day_details`.`DayNum`,`day_details`.`TotalSell`,`day_details`.`TotalBuy`,`day_details`.`ProfitDay` FROM `corner`.`day_details`;";
                MySqlDataAdapter mySqlDataAdapterUserShify = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableUserShift = new DataTable();
                mySqlDataAdapterUserShify.Fill(dTableUserShift);
                _connect.Close();


                dataGridView4.DataSource = dTableUserShift;
                dataGridView4.Columns[0].Width = 50;
                dataGridView4.Columns[1].Width = 150;
                dataGridView4.Columns[2].Width = 150;
                dataGridView4.Columns[3].Width = 150;
                double TIncom, TExpenses, TProfit;
                TIncom = TExpenses = TProfit = 0;

                foreach (DataRow rowdel in dTableUserShift.Rows)
                {
                    double DIncom = Convert.ToDouble(rowdel["TotalSell"]);
                    double DExpenses = Convert.ToDouble(rowdel["TotalBuy"]);
                    double DProfit = Convert.ToDouble(rowdel["ProfitDay"]);
                    TIncom = TIncom + DIncom;
                    TExpenses = TExpenses + DExpenses;
                    TProfit = TProfit + DProfit;

                }

                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `expens_name`,`Price` from `monthly_expenses` ; ";
                MySqlDataAdapter mySqlDataAdapterExpences = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableUserExpences = new DataTable();
                mySqlDataAdapterExpences.Fill(dTableUserExpences);
                _connect.Close();

                dataGridView5.DataSource = dTableUserExpences;
                dataGridView5.Columns[0].Width = 200;
                dataGridView5.Columns[1].Width = 200;

                double TMonthlyExp = 0;

                foreach (DataRow rowdel in dTableUserExpences.Rows)
                {
                    double MExpenses = Convert.ToDouble(rowdel["Price"]);

                    TMonthlyExp = TMonthlyExp + MExpenses;

                }

                TProfit = TProfit - TMonthlyExp;

                label27.Text = Convert.ToString(TMonthlyExp);
                label17.Text = Convert.ToString(TIncom);
                label18.Text = Convert.ToString(TExpenses);
                label19.Text = Convert.ToString(TProfit);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void productData()
        {
            RefreshProductData();
            ResetProductTab();

        }

        private void ResetProductTab()
        {
            comboBox5.Text = "";
            comboBox6.Text = "";
            comboBox7.Text = "";
            comboBox6.Visible = false;
            comboBox7.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            textBox4.Text = "";
            textBox4.Visible = false;
            textBox5.Text = "";
            textBox5.Visible = false;
            textBox6.Text = "";
            textBox6.Visible = false;
            button4.Text = "OK";
            button4.Enabled = false;
        }

        private void RefreshProductData()
        {
            try
            {
                dataGridView3.Hide();
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `Product_id`,`Product_name`,`Price`,`Store` ,`ProductType_id` from `product` ; ";

                MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableLogin = new DataTable();
                mySqlDataAdapterlogin.Fill(dTableLogin);

                _connect.Close();
                
                dataGridView3.DataSource = dTableLogin;
                //dataGridView3.Columns[0].Name = "ID";
                //dataGridView3.Columns[4].Name = "Type_ID";
                dataGridView3.Columns[0].ReadOnly = true;
                dataGridView3.Columns[3].Width = dataGridView3.Columns[2].Width = dataGridView3.Columns[0].Width = 50;
                dataGridView3.Columns[1].Width = 250;
                dataGridView3.Columns[4].Width = 50;dataGridView3.Show();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void productTypeData()
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `type_name` from `product_type` ; ";

                MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableLogin = new DataTable();
                mySqlDataAdapterlogin.Fill(dTableLogin);

                _connect.Close();


                dataGridView2.DataSource = dTableLogin;
                dataGridView2.Columns[0].Width = 300;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void usersData()
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `username`,`Pass` from `login` ; ";

                MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableLogin = new DataTable();
                mySqlDataAdapterlogin.Fill(dTableLogin);

                _connect.Close();

                dataGridView1.DataSource = dTableLogin;
                dataGridView1.Columns[0].Width = 250;
                dataGridView1.Columns[1].Width = 250;


                DataCBoxUsers();

                //dbConnection();
                //cmd.Connection = _connect;
                //cmd.CommandText = "Select `type_name` from `product_type` ; ";

                //MySqlDataAdapter mySqlDataAdapterPType = new MySqlDataAdapter {SelectCommand = cmd};
                //DataTable dTablePType = new DataTable();
                //mySqlDataAdapterPType.Fill(dTablePType);

                //_connect.Close();


                //dataGridView2.DataSource = dTablePType;
                //dataGridView2.Columns[0].Width = 300;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Edit")
            {
                label5.Visible = true;
                comboBox2.Visible = true;
                button2.Text = "Update";


            }
            else if (comboBox1.Text == "Add")
            {
                label5.Visible = false;
                comboBox2.Visible = false;
                label4.Visible = true;
                label6.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                button2.Enabled = true;
                button2.Text = "Add User";

            }
            //else delete
            else if (comboBox1.Text == "Delete")
            {
                label5.Visible = true;
                comboBox2.Visible = true;
                label4.Visible = false;
                label6.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
                button2.Enabled = true;
                button2.Text = "Delete User";



            }
            DataCBoxUsers();

        }

        private void DataCBoxUsers()
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `username` from `login` ; ";

                MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableLogin = new DataTable();
                mySqlDataAdapterlogin.Fill(dTableLogin);

                _connect.Close();

                comboBox2.Items.Clear();
                foreach (DataRow rowdel in dTableLogin.Rows)
                {
                    comboBox2.Items.Add(Convert.ToString(rowdel["username"]));
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string username = comboBox2.Text;
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `username`,`Pass` from `login` where 	username='" + username + "'; ";

                MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableLogin = new DataTable();
                mySqlDataAdapterlogin.Fill(dTableLogin);

                _connect.Close();


                if (dTableLogin.Rows.Count == 1)
                {
                    if (comboBox1.Text == "Delete")
                    {
                        label4.Visible = false;
                        label6.Visible = false;
                    }

                    else
                    {
                        label4.Visible = true;
                        label6.Visible = true;
                        foreach (DataRow rowdel in dTableLogin.Rows)
                        {
                            textBox1.Text = Convert.ToString(rowdel["username"]);
                            textBox2.Text = Convert.ToString(rowdel["Pass"]);
                            textBox1.Visible = true;
                            textBox2.Visible = true;
                        }
                    }


                    button2.Enabled = true;
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Update")
            {
                try
                {
                    
                    dbConnection();
                    cmd.Connection = _connect;

                    cmd.CommandText = "UPDATE `corner`.`login` SET `username`= '"+ textBox1.Text +"' , `Pass` = '" +textBox2.Text + "' where 	username='" + comboBox2.Text + "'; ";

                    MySqlDataReader reader = cmd.ExecuteReader();
                    _connect.Close();

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else if (button2.Text == "Add User")
            {
                try
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "select * from `login` where 	username='" + textBox1.Text + "'; ";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    bool b = reader.Read();
                    
                    _connect.Close();


                    if (b)
                    {
                        MessageBox.Show("there is the same user name ,please enter another name", "Message",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else 
                    {
                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText =
                            "INSERT INTO `corner`.`login` (`username`,`Pass`) VALUES ('" +
                            textBox1.Text + "' , '" + textBox2.Text + "') ; ";
                        MySqlDataReader reader2 = cmd.ExecuteReader();
                        _connect.Close();

                    }

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (button2.Text == "Delete User")
            {
                try
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "DELETE  FROM `corner`.`login` where username='" + comboBox2.Text + "' ; ";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    _connect.Close();

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            usersData();

        }

        private void setToStart()
        {
            comboBox1.Text = "";
            button2.Enabled = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox1.Visible = false;
            textBox2.Visible = false;
            comboBox2.Text = "";
            comboBox2.Visible = false;

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox3.Text == "Add")
            {
                AddType();

            }
            else if (comboBox3.Text == "Update")
            {
                UpdateType();
            }
            else if (comboBox3.Text == "Delete")
            {
                DeleteType();
            }


        }

        private void DeleteType()
        {
            button3.Text = "Delete";
            //button3.Enabled = true;
            label8.Visible = false;
            textBox3.Text = "";
            textBox3.Visible = false;
            comboBox4.Visible = true;
            label7.Visible = true;
            ReloadComboBox4();
        }

        private void UpdateType()
        {
            button3.Text = "Update";
            //button3.Enabled = true;
            label8.Visible = false;
            textBox3.Text = "";
            textBox3.Visible = false;
            comboBox4.Visible = true;
            label7.Visible = true;
            ReloadComboBox4();
        }

        private void ReloadComboBox4()
        {
            comboBox4.Text = "";
            comboBox4.Items.Clear();
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `type_name` from `product_type` ;";
                MySqlDataAdapter mySqlDataAdaptersell = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableSell = new DataTable();
                mySqlDataAdaptersell.Fill(dTableSell);
                _connect.Close();


                foreach (DataRow dR in dTableSell.Rows)
                {
                    comboBox4.Items.Add(Convert.ToString(dR["type_name"]));
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void AddType()
        {
            button3.Text = "Add Type";
            button3.Enabled = true;
            label8.Visible = true;
            textBox3.Text = "";
            textBox3.Visible = true;
            comboBox4.Visible = false;
            label7.Visible = false;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            string typeName = comboBox4.Text;

            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `type_name` from `product_type` where type_name='" + typeName + "' ;";
                MySqlDataAdapter mySqlDataAdaptersell = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableSell = new DataTable();
                mySqlDataAdaptersell.Fill(dTableSell);
                _connect.Close();

                if (dTableSell.Rows.Count == 1)
                {

                    if (comboBox3.Text == "Update")
                    {
                        label8.Visible = true;
                        textBox3.Visible = true;
                        textBox3.Text = typeName;

                    }

                    else if (comboBox3.Text == "Delete")
                    {
                        label8.Visible = false;
                        textBox3.Visible = false;

                    }


                    button3.Enabled = true;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "Add")
            {
                if (string.IsNullOrEmpty(textBox3.Text))
                    MessageBox.Show("Enter New Type", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    try
                    {
                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText = "Select * from `product_type` where type_name='" + textBox3.Text + "' ;";
                        //MySqlDataAdapter mySqlDataAdaptersell = new MySqlDataAdapter {SelectCommand = cmd};
                        //DataTable dTableSell = new DataTable();
                        //mySqlDataAdaptersell.Fill(dTableSell);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        bool b = reader.Read();
                        _connect.Close();


                        if (b)
                        {
                            MessageBox.Show("there is the same Type name ,please enter another name", "Message",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else 
                        {

                            dbConnection();
                            cmd.Connection = _connect;
                            cmd.CommandText = "INSERT INTO `corner`.`product_type`(`type_name`) VALUES('"+textBox3.Text+"'); ";
                            MySqlDataReader reader2 = cmd.ExecuteReader();
                           
                            _connect.Close();



                        }

                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } // end If Add product Type

            else if (comboBox3.Text == "Update")
            {
                if (string.IsNullOrEmpty(textBox3.Text))
                    MessageBox.Show("Enter New Type", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    try
                    {
                        //dbConnection();
                        //cmd.Connection = _connect;
                        //cmd.CommandText = "Select * from `product_type` where type_name='" + comboBox4.Text + "' ;";
                        ////MySqlDataAdapter mySqlDataAdaptersell = new MySqlDataAdapter {SelectCommand = cmd};
                        ////DataTable dTableSell = new DataTable();
                        ////mySqlDataAdaptersell.Fill(dTableSell);
                        ////_connect.Close();
                        //MySqlDataReader reader = cmd.ExecuteReader();
                        //bool b = reader.Read();
                        //_connect.Close();

                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText = "Select * from `product_type` where type_name='" + textBox3.Text + "' ;";
                        //MySqlDataAdapter mySqlDataAdapterThere = new MySqlDataAdapter {SelectCommand = cmd};
                        //DataTable dTableThere = new DataTable();
                        //mySqlDataAdapterThere.Fill(dTableThere);

                        //_connect.Close();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        bool a = reader.Read();
                        _connect.Close();


                        if (a)
                        {
                            MessageBox.Show("Enter another Name", "Message", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        else
                        {
                            
                                dbConnection();
                            cmd.Connection = _connect;
                            cmd.CommandText =
                                    "UPDATE `corner`.`product_type` SET `type_name` = '" + textBox3.Text + "' WHERE `type_name` = '" + comboBox4.Text +"' ; ";

                                MySqlDataReader reader4 = cmd.ExecuteReader();
                                _connect.Close();

                        }

                    } //end try

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            } // end if for update product type
            else if (comboBox3.Text == "Delete")
            {
                try
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "Delete  from `product_type` where type_name ='" + comboBox4.Text + "' ;";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    _connect.Close();


                } //end try

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

            productTypeData();
        }

        private void setToStartproTypeTab()
        {
            comboBox3.Text = "";
            label7.Visible = false;
            label8.Visible = false;
            ReloadComboBox4();
            comboBox4.Visible = false;
            textBox3.Text = "";
            textBox3.Visible = false;
            usersData();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "Add")
            {
                label9.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label13.Visible = true;
                comboBox7.Visible = true;
                RefreshProductType();
                textBox4.Text = "";
                textBox4.Visible = true;
                textBox5.Text = "";
                textBox5.Visible = true;
                textBox6.Text = "";
                textBox6.Visible = true;
                button4.Text = "Add";
                button4.Enabled = true;
                comboBox6.Visible = false;
                label12.Visible = false;
            } //end Add product

            else if (comboBox5.Text == "Update")
            {
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label13.Visible = false;
                comboBox7.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                textBox6.Visible = false;
                button4.Text = "Update";
                comboBox6.Visible = true;
                RefreshCBoxProduct();
                label12.Visible = true;

            } //end update product
            else if (comboBox5.Text == "Delete")
            {
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label13.Visible = false;
                comboBox7.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                textBox6.Visible = false;
                button4.Text = "Delete";
                comboBox6.Visible = true;
                RefreshCBoxProduct();
                label12.Visible = true;
                button4.Enabled = true;
            } //end delete product
        }

        private void RefreshCBoxProduct()
        {
            comboBox6.Items.Clear();
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `Product_name` from `product`  ; ";

                MySqlDataAdapter mySqlDataAdapterProduct = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableProduct = new DataTable();
                mySqlDataAdapterProduct.Fill(dTableProduct);

                _connect.Close();

                foreach (DataRow rowdel in dTableProduct.Rows)
                {
                    comboBox6.Items.Add(Convert.ToString(rowdel["Product_name"]));
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshProductType()
        {
            comboBox7.Items.Clear();
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select `type_name` from `product_type`  ; ";

                MySqlDataAdapter mySqlDataAdapterProduct = new MySqlDataAdapter {SelectCommand = cmd};
                DataTable dTableProduct = new DataTable();
                mySqlDataAdapterProduct.Fill(dTableProduct);

                _connect.Close();

                foreach (DataRow rowdel in dTableProduct.Rows)
                {
                    comboBox7.Items.Add(Convert.ToString(rowdel["type_name"]));
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "Update")
            {
                label9.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label13.Visible = true;
                comboBox7.Visible = true;
                RefreshProductType();
                textBox4.Visible = true;
                textBox5.Visible = true;
                textBox6.Visible = true;

                

                string Product_name = comboBox6.Text;

                try
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "Select * from `product` where  Product_name = '"+ Product_name + "' ; ";

                    MySqlDataAdapter mySqlDataAdapterProduct = new MySqlDataAdapter {SelectCommand = cmd};
                    DataTable dTableProduct = new DataTable();
                    mySqlDataAdapterProduct.Fill(dTableProduct);

                    _connect.Close();

                    if (dTableProduct.Rows.Count == 1)
                    {

                        foreach (DataRow rowdel in dTableProduct.Rows)
                        {
                            int PTID = Convert.ToInt16(rowdel["ProductType_id"]);
                            dbConnection();
                            cmd.Connection = _connect;
                            cmd.CommandText = "Select * from `product_type`  where type_id=" +
                                             PTID + "; ";

                            MySqlDataAdapter mySqlDataAdapterType = new MySqlDataAdapter {SelectCommand = cmd};
                            DataTable dTableType = new DataTable();
                            mySqlDataAdapterType.Fill(dTableType);

                            _connect.Close();

                            if (dTableType.Rows.Count == 1)
                            {
                                string storeP = Convert.ToString(rowdel["Store"]);
                                string priceP = Convert.ToString(rowdel["Price"]);
                                foreach (DataRow rowpro in dTableType.Rows)
                                {
                                    textBox4.Text = Convert.ToString(rowdel["Product_name"]);
                                    textBox5.Text = priceP;
                                    textBox6.Text = storeP;
                                    comboBox7.Text = Convert.ToString(rowpro["type_name"]);
                                }
                            }


                            //comboBox7.Text = x.type;
                        }
                        button4.Enabled = true;
                    }

                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

        }

        private string[] a =
        {
            "قهوة تركى", "قهوة تركى دوبل", "قهوة اسبريسو", "قهوة اسبريسو دوبل", "قهوة بندق", "قهوة بالكريم الايرلندى",
            "قهوة فرنساوى", "قهوة فرنساوى دوبل", "قهوة كراميل", "قهوة لوز", "قهوة فستق", "قهوة شيكولاته",
            "قهوة فانيليا", "قهوة ميكس", "قهوة نكهات دوبل", "قهوة قرفة", "نسكافيه", "نسكافيه بلاك", "نسكافيه كراميل",
            "نسكافيه جولد", "كوفى ميكس", "كابتشينو ماكينة", "كابتشينو كلاسيك", "كابتشينو بندق", "كابتشينو فانيليا",
            "كابتشينو موكا", "كابتشينو شيكولاته", "نسكويك", "لاتيه", "موكا", "هوت شوكلت", "شوكو ميلك", "ماكياتو",
            "فرابينو بالفانيليا", "فرابينو بالكراميل", "شاى", "شاى لاتيه شيكولاته", "شاى تفاح", "شاى مانجو", "شاى توت",
            "شاى فانيليا", "شاى مشمش", "شاى مانجو و خوخ", "شاى توت برى", "شاى توت و فراولة", "شاى خوخ",
            "شاى خوخ و فواكه استوائية", "شاى فراولة", "شاى فراولة و كيوى", "شاى فراولة و رمان", "شاى توت و رمان",
            "شاى عدنى", "شاى ياسمين", "شاى اخضر", "شاى اخضر بالياسمين", "شاى إيرل جراى", "شاى بالليمون", "ينسون",
            "كركديه", "نعناع", "نعناع بالكاموميل", "جنزبيل", "كراويه", "ليمون بالجنزبيل", "قرفة", "تليو",
            "جنزبيل بالقرفة", "waffle basic", "شاى فواكة برية", "شاى خوخ وورد", "سادة فاتح", "سادة وسط", "سادة غامق",
            "سادة محروق", "محوج فاتح", "محوج وسط", "محوج غامق", "محوج محروق", "كولومبى", "حبشى", "نسكافية فانيليا",
            "نسكافية بندق", "شاى بالنعناع", "شاى بالقرنفل", "لاتية كراميل", "شاى مغربى", "قهوة كولومبى", "قهوة حبشى",
            "مياة كبير", "مياة صغير", "كانز كبير", "فروتز", "لبن", "اضافة حليب", "نسكافية موكا", "قهوة تركى محوج",
            "قهوة تركى محوج دوبل", "قهوة بالحليب"
        };

        private int[] a2 =
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 5, 2, 2, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 2, 2, 3, 2, 1, 1, 6, 6, 6, 6, 6, 2, 3, 1, 1, 1
        };
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox5.Text == "Add")
            {
                try
                {
                    if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text) ||
                        string.IsNullOrEmpty(textBox6.Text) || string.IsNullOrEmpty(comboBox7.Text))
                    {
                        MessageBox.Show("Complete your data", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    else
                    {
                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText = "Select * from `product`  where 	Product_name='" + textBox4.Text + "'; ";

                        //MySqlDataAdapter mySqlDataAdapterProduct = new MySqlDataAdapter {SelectCommand = cmd};
                        //DataTable dTableProduct = new DataTable();
                        //mySqlDataAdapterProduct.Fill(dTableProduct);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        bool b = reader.Read();
                        _connect.Close();

                        if (b)
                        {
                            MessageBox.Show("there is the same product name ,please enter another name", "Message",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else 
                        {
                            int pricePro = Convert.ToInt32(textBox5.Text);
                            int AddStore = Convert.ToInt32(textBox6.Text);

                            dbConnection();
                            cmd.Connection = _connect;
                            cmd.CommandText = "Select `type_id` from `product_type`  where 	type_name='" +
                                              comboBox7.Text + "'; ";

                            MySqlDataAdapter mySqlDataAdapterType = new MySqlDataAdapter {SelectCommand = cmd};
                            DataTable dTableType = new DataTable();
                            mySqlDataAdapterType.Fill(dTableType);

                            _connect.Close();


                            foreach (DataRow rowdel in dTableType.Rows)
                            {
                                dbConnection();
                                cmd.Connection = _connect;
                                cmd.CommandText =
                                    "INSERT INTO `corner`.`product` (`Product_name`,`Price`,`ProductType_id`,`Store`) VALUES ('" +
                                    textBox4.Text + "' , '" + pricePro + "' , '" + Convert.ToInt16(rowdel["type_id"]) +
                                    "' , '" + AddStore + "') ; ";
                                MySqlDataReader reader2 = cmd.ExecuteReader();
                                _connect.Close();


                            }


                        }

                    }
                }


                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } //end add
            else if (comboBox5.Text == "Update")
            {
                try
                {
                    if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text) ||
                        string.IsNullOrEmpty(textBox6.Text) || string.IsNullOrEmpty(comboBox7.Text))
                    {
                        MessageBox.Show("Complete your data", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    else
                    {
                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText = "Select *  from `product`  where 	Product_name='" + comboBox6.Text + "'; ";

                        //MySqlDataAdapter mySqlDataAdapterProduct = new MySqlDataAdapter {SelectCommand = cmd};
                        //DataTable dTableProduct = new DataTable();
                        //mySqlDataAdapterProduct.Fill(dTableProduct);

                        //_connect.Close();
                        MySqlDataReader reader = cmd.ExecuteReader();
                        int Product_id = 0;
                        if (reader.Read()){
                            
                            Product_id = Convert.ToInt16(reader["Product_id"]);
                        }
                        _connect.Close();

                        if (Product_id!=0)
                        {

                            Double pricePro = Convert.ToDouble(textBox5.Text);
                        int AddStore = Convert.ToInt32(textBox6.Text);

                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText = "Select `type_id` from `product_type` where 	type_name='" +
                                          comboBox7.Text + "'; ";

                        MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter {SelectCommand = cmd};
                        DataTable dTableLogin = new DataTable();
                        mySqlDataAdapterlogin.Fill(dTableLogin);

                        _connect.Close();
                          //textBox9.Text=  Convert.ToString(dataGridView3.Rows[1].Cells[0].Value);
                        foreach (DataRow rowType in dTableLogin.Rows)
                        {
                              //  `Product_id`,`Product_name`,`Price`,`Store` ,`ProductType_id`
                            dbConnection();
                            cmd.Connection = _connect;
                            cmd.CommandText =
                                "UPDATE `corner`.`product` SET `Product_name` = '" + textBox4.Text +
                                "' , `Price`= '" + pricePro + "' , `Store`= '" + AddStore +
                                "' , `ProductType_id`= '" + Convert.ToInt16(rowType["type_id"]) +
                                "' where Product_id ='" + Product_id + "'; ";

                            MySqlDataReader reader4 = cmd.ExecuteReader();
                            reader4.Read();
                            _connect.Close();


                        }
                    }

                    productData();

                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } //end update
            else if (comboBox5.Text == "Delete")
            {
                try
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "Delete  from `product` where Product_name='" + comboBox6.Text + "'; ";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    _connect.Close();


                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                productData();

            } //end delete
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Select * from `month_details` ; ";

                //MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter {SelectCommand = cmd};
                //DataTable dTableLogin = new DataTable();
                //mySqlDataAdapterlogin.Fill(dTableLogin);
                MySqlDataReader readerMEx = cmd.ExecuteReader();
                bool b = readerMEx.Read();
                _connect.Close();

                int MNumber;
                if (b)
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "Select * from `month_details` ; ";

                    MySqlDataAdapter mySqlDataAdapterlogin = new MySqlDataAdapter { SelectCommand = cmd };
                    DataTable dTableLogin = new DataTable();
                    mySqlDataAdapterlogin.Fill(dTableLogin);
                    _connect.Close();
                    MNumber = dTableLogin.Rows.Count + 1;

                }
                else
                    MNumber = 1;


                decimal TSell = Convert.ToDecimal(label17.Text);
                decimal TBuy = Convert.ToDecimal(label18.Text);
                decimal MonthlyExp = Convert.ToDecimal(label27.Text);
                decimal profitMonth = Convert.ToDecimal(label19.Text);

                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText =
                    "INSERT INTO `corner`.`month_details` (`MonthNumber`,`TotalSell`,`TotalBuy`, `TotalMonthlyExpenses`,`ProfitDay`) VALUES ('" +
                    MNumber + "' , '" + TSell + "' , '" + TBuy + "' , '" + MonthlyExp + "' , '" + profitMonth + "') ; ";
                MySqlDataReader reader2 = cmd.ExecuteReader();
                _connect.Close();


                //remove from Monthly Expenses 
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText = "Delete  from `day_details` ; Delete  from `monthly_expenses` ;";
                MySqlDataReader reader = cmd.ExecuteReader();
                _connect.Close();

                button5.Enabled = false;

            } //end try

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text))
            {

                MessageBox.Show("Enter Data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

                try
                {

                    string ExpName = textBox7.Text;
                    decimal Exp_Price = Convert.ToDecimal(textBox8.Text);

                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText =
                        "INSERT INTO `corner`.`monthly_expenses` (`expens_name`,`Price`) VALUES ('" +
                        ExpName + "' , '" + Exp_Price + "' ) ;" +
                        "INSERT INTO `corner`.`all_monthlyexpenses` (`expens_name`,`Price`,`Date_time`) VALUES('" +
                        ExpName + "' , '" + Exp_Price + "' , '" + DateTime.Now.ToString("s") + "' ); ";
                    MySqlDataReader reader2 = cmd.ExecuteReader();
                    _connect.Close();





                    textBox7.Text = "";
                    textBox8.Text = "";



                } //end try

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

            SearchName.Items.Clear();
            SearchName.Text = "";
            try
            {


                if (SearchType.Text == "Product Type")
                {
                    SearchName.Visible = true;
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "Select * from `product_type` ;";
                    MySqlDataAdapter mySqlDataAdaptersell = new MySqlDataAdapter {SelectCommand = cmd};
                    DataTable dTableProType = new DataTable();
                    mySqlDataAdaptersell.Fill(dTableProType);
                    _connect.Close();



                    foreach (DataRow row in dTableProType.Rows)
                    {
                        SearchName.Items.Add(row["type_name"]);
                    }
                  
                }
                else if (SearchType.Text == "Product")
                {
                    SearchName.Visible = true;
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "Select * from `product` ;";
                    MySqlDataAdapter mySqlDataAdaptersell = new MySqlDataAdapter {SelectCommand = cmd};
                    DataTable dTablePro = new DataTable();
                    mySqlDataAdaptersell.Fill(dTablePro);
                    _connect.Close();



                    foreach (DataRow row in dTablePro.Rows)
                    {
                        SearchName.Items.Add(row["Product_name"]);
                    }

                }
                else if (SearchType.Text == "Details")
                {
                    SearchName.Visible = TDeposite.Visible = false;
                    

                }

                else if (SearchType.Text == "Monthly Expenses")
                {
                    SearchName.Visible = false;

                }


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void button7_Click(object sender, EventArgs e)
        {

            DateTime a = dateTimePicker1.Value;
            DateTime b = dateTimePicker2.Value;
            SearchResult.DataSource = null;
            GroupSearchResult.DataSource = null;
            GroupSearchResult.Rows.Clear();
            GroupSearchResult.Refresh();
            SearchResult.Refresh();



            try
            {

                SearchResult.Visible = true;

                if (SearchType.Text == "Product Type")
                {
                    GroupSearchResult.Visible = true;
                    Withdraw.Visible = false;
                    Deposite.Visible = false;
                    Twithdraw.Visible = false;
                    TDeposite.Visible = false;
                    Total.Visible = false;
                    

                //    dbConnection();
                //    cmd.Connection = _connect;
                //    cmd.CommandText = " SELECT type_id FROM `product_type`  WHERE type_name = '" +
                //                      SearchName.SelectedText + "'; ";
                //    //cmd.CommandText =
                //    //    "Select `product_process`.`Product_name` , `product_process`.`quantity` , `product_process`.`price`, `product_process`.`DateTime` from `product_process`" +
                //    //    " INNER JOIN product ON product.ProductType_id=product_type.type_id  INNER JOIN product_process ON product_process.Product_name=product.Product_name where product_type.type_name= '" +
                //    //    SearchName.Text + "' and " + a.ToString("s") + "<= product_process.DateTime and product_process.DateTime <=" +
                //    //    b + " ; ";
                //    MySqlDataAdapter mysqlcat = new MySqlDataAdapter {SelectCommand = cmd};
                //    DataTable dtcat = new DataTable();
                //    mysqlcat.Fill(dtcat);
                //    string catid = null;
                //    foreach (DataRow row in dtcat.Rows)
                //    {
                //        catid = Convert.ToString(row["type_id"]);
                //}
                    //MySqlCommand cmd2 = new MySqlCommand();
                    int catid = SearchName.SelectedIndex + 1;
                    cmd.Connection = _connect; 
                    cmd.CommandText = "SELECT * FROM `product_process` where catid='"+catid.ToString()+ "' and DateTime Between '" + a.ToString("s") +
                                      "' and '" + b.ToString("s") + " ' ;";
                    MySqlDataAdapter mySqlDataAdapterUserShify = new MySqlDataAdapter { SelectCommand = cmd };

                    DataTable dTableUserShift = new DataTable();
                    mySqlDataAdapterUserShify.Fill(dTableUserShift);
                    _connect.Close();



                    SearchResult.DataSource = dTableUserShift;
                    SearchResult.Columns[0].Width = 200;
                    SearchResult.Columns[3].Width = 200;

                    List<string> termsList = new List<string>();

                    double total = 0;
                    int q = 0;

                    foreach (DataRow rowdel in dTableUserShift.Rows)
                    {
                        total = total + Convert.ToDouble(rowdel["price"]);
                        q = q + Convert.ToInt32(rowdel["quantity"]);

                        int f = 0;
                        foreach (string aPart in termsList)
                        {
                            if (aPart == Convert.ToString(rowdel["Product_name"]))
                            {
                                f = 1;
                            }

                        }

                        if (f == 0)
                        {
                            termsList.Add(Convert.ToString(rowdel["Product_name"]));

                            int quantities = 0;
                            double price = 0.0;

                            DataRow[] dRows;
                            dRows = dTableUserShift.Select("Product_name =" + Convert.ToString(rowdel["Product_name"]));
                            //var queryProN = from pN in queryProT
                            //                where pN.productName == x.productName
                            //                select new { Quantities = pN.Quantities, Price = pN.Price };



                            for (int p = 0; p < dRows.Length; p++)
                            {
                                quantities = quantities + Convert.ToInt32(dRows[p]["quantity"]);
                                price = price + Convert.ToDouble(dRows[p]["Price)"]);

                            }

                            this.GroupSearchResult.Rows.Add(rowdel["Product_name"].ToString(), quantities, price);
                        }




                    }

                    this.GroupSearchResult.Rows.Add("Total", q, total);

                }
                else if (SearchType.Text == "Product")
                {
                    GroupSearchResult.Visible = true;
                    Withdraw.Visible = false;
                    Deposite.Visible = false;
                    Twithdraw.Visible = false;
                    TDeposite.Visible = false;
                    Total.Visible = false;

                    if (SearchName.Text == "All")
                    {
                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText = "Select * from `product_process` WHERE DateTime Between '" + a.ToString("s") +
                                          "' and '" + b.ToString("s") + "' ;";
                        MySqlDataAdapter mySqlDataAdapterBuy = new MySqlDataAdapter {SelectCommand = cmd};
                        DataTable dTableBuy = new DataTable();
                        mySqlDataAdapterBuy.Fill(dTableBuy);
                        _connect.Close();

                        //var queryProT = from o in cornerE.product_process
                        //                where a <= o.date_time && o.date_time <= b
                        //                select new { productName = o.product_name, Quantities = o.quantities, Price = o.price, Date_Time = o.date_time };

                        SearchResult.DataSource = dTableBuy;
                        SearchResult.Columns[0].Width = 200;
                        SearchResult.Columns[3].Width = 200;

                        List<string> termsList = new List<string>();
                        double total = 0;
                        int q = 0;

                        foreach (DataRow x in dTableBuy.Rows)
                        {
                            total = total + Convert.ToDouble(x["Price"]);
                            q = q + Convert.ToInt32(x["quantity"]);

                            int f = 0;
                            foreach (string aPart in termsList)
                            {
                                if (aPart == x["product_name"].ToString())
                                {
                                    f = 1;
                                }

                            }

                            if (f == 0)
                            {
                                termsList.Add(x["product_name"].ToString());

                                int quantities = 0;
                                double price = 0.0;


                                DataRow[] dRows;
                                dRows = dTableBuy.Select("Product_name =" + Convert.ToString(x["Product_name"]));


                                //var queryProN = from pN in queryProT
                                //                   where pN.productName == x.productName
                                //                   select new { Quantities = pN.Quantities, Price = pN.Price };


                                for (int p = 0; p < dRows.Length; p++)
                                {
                                    quantities = quantities + Convert.ToInt32(dRows[p]["quantity"]);
                                    price = price + Convert.ToDouble(dRows[p]["Price)"]);

                                }
                                //foreach (var p in queryProN)
                                //   {
                                //       quantities = quantities + Convert.ToInt32(p.Quantities);
                                //       price = price + Convert.ToDouble(p.Price);

                                //   }

                                this.GroupSearchResult.Rows.Add(x["Product_name"].ToString(), quantities, price);
                            }




                        }

                        this.GroupSearchResult.Rows.Add("Total", q, total);
                    }

                    else
                    {
                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText = "Select * from `product_process` WHERE product_name = '" + SearchName.Text +
                                          "' and DateTime Between '" + a.ToString("s") +
                                          "' and '" + b.ToString("s") + "' ;";
                        MySqlDataAdapter mySqlDataAdapterBuy = new MySqlDataAdapter {SelectCommand = cmd};
                        DataTable dTableBuy = new DataTable();
                        mySqlDataAdapterBuy.Fill(dTableBuy);
                        _connect.Close();

                        //var queryProT = from o in cornerE.product_process
                        //                where o.product_name == SearchName.Text && a <= o.date_time && o.date_time <= b
                        //                select new { productName = o.product_name, Quantities = o.quantities, Price = o.price, Date_Time = o.date_time };

                        SearchResult.DataSource = dTableBuy;
                        SearchResult.Columns[0].Width = 200;
                        SearchResult.Columns[3].Width = 200;

                        List<string> termsList = new List<string>();
                        double total = 0;
                        int q = 0;

                        foreach (DataRow x in dTableBuy.Rows)
                        {
                            total = total + Convert.ToDouble(x["Price"]);
                            q = q + Convert.ToInt32(x["quantity"]);


                            int f = 0;
                            foreach (string aPart in termsList)
                            {
                                if (aPart == x["product_name"].ToString())
                                {

                                    f = 1;
                                }

                            }

                            if (f == 0)
                            {
                                termsList.Add(x["product_name"].ToString());

                                //termsList.Add(x.productName);

                                int quantities = 0;
                                double price = 0.0;
                                DataRow[] dRows;
                                dRows = dTableBuy.Select("Product_name =" + Convert.ToString(x["Product_name"]));

                                //var queryProN = from pN in queryProT
                                //                   where pN.productName == x.productName
                                //                   select new { Quantities = pN.Quantities, Price = pN.Price };


                                for (int p = 0; p < dRows.Length; p++)
                                {
                                    quantities = quantities + Convert.ToInt32(dRows[p]["quantity"]);
                                    price = price + Convert.ToDouble(dRows[p]["Price)"]);

                                }
                                //foreach (var p in queryProN)
                                //   {
                                //       quantities = quantities + Convert.ToInt32(p.Quantities);
                                //       price = price + Convert.ToDouble(p.Price);

                                //   }

                                this.GroupSearchResult.Rows.Add(x["Product_name"].ToString(), quantities, price);





                                //foreach (var p in queryProN)
                                //   {
                                //       quantities = quantities + Convert.ToInt32(p.Quantities);
                                //       price = price + Convert.ToDouble(p.Price);

                                //   }

                                //   this.GroupSearchResult.Rows.Add(x.productName, quantities, price);
                            }




                        }



                    }


                }

                else if (SearchType.Text == "Details")
                {
                    GroupSearchResult.Visible = true;
                    Withdraw.Visible = true;
                    Deposite.Visible = true;

                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.CommandText = "Select * from `product_process` WHERE  DateTime Between '" + a.ToString("s") +
                                      "' and '" + b.ToString("s") + "' ;";
                    MySqlDataAdapter mySqlDataAdapterBuy = new MySqlDataAdapter {SelectCommand = cmd};
                    DataTable dTableBuy = new DataTable();
                    mySqlDataAdapterBuy.Fill(dTableBuy);
                    _connect.Close();

                    //var queryProT = from o in cornerE.product_process
                    //                    where a <= o.date_time && o.date_time <= b
                    //                    select new { productName = o.product_name, Quantities = o.quantities, Price = o.price, Date_Time = o.date_time };

                    List<string> termsList = new List<string>();
                    double Dtotal = 0;
                    int q = 0;

                    foreach (DataRow x in dTableBuy.Rows)
                    {
                        Dtotal = Dtotal + Convert.ToDouble(x["Price"]);
                        q = q + Convert.ToInt32(x["quantity"]);

                        int f = 0;
                        foreach (string aPart in termsList)
                        {
                            if (aPart == x["product_name"].ToString())
                            {
                                {
                                    f = 1;
                                }

                            }

                            if (f == 0)
                            {
                                termsList.Add(x["product_name"].ToString());

                                int quantities = 0;
                                double price = 0.0;



                                DataRow[] dRows;
                                dRows = dTableBuy.Select("Product_name =" + Convert.ToString(x["Product_name"]));

                                //var queryProN = from pN in queryProT
                                //                   where pN.productName == x.productName
                                //                   select new { Quantities = pN.Quantities, Price = pN.Price };


                                for (int p = 0; p < dRows.Length; p++)
                                {
                                    quantities = quantities + Convert.ToInt32(dRows[p]["quantity"]);
                                    price = price + Convert.ToDouble(dRows[p]["Price)"]);

                                }
                                //foreach (var p in queryProN)
                                //   {
                                //       quantities = quantities + Convert.ToInt32(p.Quantities);
                                //       price = price + Convert.ToDouble(p.Price);

                                //   }

                                this.GroupSearchResult.Rows.Add(x["Product_name"].ToString(), quantities, price);






                                //    var queryProN = from pN in queryProT
                                //                    where pN.productName == x.productName
                                //                    select new { Quantities = pN.Quantities, Price = pN.Price };

                                //    foreach (var p in queryProN)
                                //    {
                                //        quantities = quantities + Convert.ToInt32(p.Quantities);
                                //        price = price + Convert.ToDouble(p.Price);

                                //    }

                                //    this.GroupSearchResult.Rows.Add(x["product_name"].ToString(), quantities, price);
                                //
                            }

                        }

                        this.GroupSearchResult.Rows.Add("Total Selling","", q, Dtotal);
                        //end add to group result search

                        // start add to groupsearchresult إضافة مبلغ
                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.CommandText = "Select * from `safe` WHERE  `DTime` Between '" + a.ToString("s") +
                                          "' and '" + b.ToString("s") +
                                          "' and type= 'إضافة مبلغ' ;";
                        MySqlDataAdapter mySqlDataAdapterSafe = new MySqlDataAdapter {SelectCommand = cmd};
                        DataTable dTableSafe = new DataTable();
                        mySqlDataAdapterBuy.Fill(dTableSafe);
                        _connect.Close();
                        //var queryDeposite = from o in cornerE.safes
                        //                    where a <= o.date_time && o.date_time <= b && o.type == "إضافة مبلغ"
                        //                    select o ;
                        //select new { productName = o.product_name, Quantities = o.who_PersonAdd, Price = o.price, Date_Time = o.date_time };


                        foreach (DataRow xy in dTableSafe.Rows)
                        {
                            Dtotal = Dtotal + Convert.ToDouble(x["price"]);

                            this.GroupSearchResult.Rows.Add(xy["type"] + "  ( " + xy["Dtime"] + "   ) ",
                                xy["Who_personAdd"],0, xy["price"]);

                        }



                        //this.GroupSearchResult.Rows.Add("Total Deposiite", q, total);
                        TDeposite.Text = "Total Deposite : " + Dtotal + " LE";
                        TDeposite.Visible = true;


                        //start show withdraw
                        double totalWithdraw = 0;


                        dbConnection();
                        cmd.CommandText =
                            "Select  `Type` ,`price` , `reason`,`proName`, `quantity`, `ReasonOfT`, `PersonTake` , `Dtime` from `safe` where  Dtime Between '" + a.ToString("s") +
                            "' and '" + b.ToString("s") + "' AND not Type = 'إضافة مبلغ' ; ";

                        MySqlDataAdapter mySqlDataAdapterViewProduct = new MySqlDataAdapter {SelectCommand = cmd};
                        DataTable dTableViewProduct = new DataTable();
                        mySqlDataAdapterViewProduct.Fill(dTableViewProduct);
                        _connect.Close();

                        SearchResult.DataSource = dTableViewProduct;
                        //SearchResult.Columns[0].Width = 200;
                        //SearchResult.Columns[2].Width = 200;
                        //SearchResult.Columns[3].Width = 200;
                        //SearchResult.Columns[5].Width = 200;
                        //SearchResult.Columns[6].Width = 200;
                        //SearchResult.Columns[7].Width = 200;

                        foreach (DataRow rowdel in dTableViewProduct.Rows)
                        {
                            totalWithdraw = totalWithdraw + Convert.ToDouble(rowdel["price"]);

                        }

                        Twithdraw.Text = "Total Withdraw : " + totalWithdraw + " LE";
                        Twithdraw.Visible = true;


                        Total.Text = "Profit : " + (Dtotal - totalWithdraw) + " LE";
                        Total.Visible = true;

                        //this.SearchResult.Rows.Add("Total Withdraw", totalWithdraw );

                    }
                } //end else details 

                //start Monthly Expenses
                else if (SearchType.Text == "Monthly Expenses")
                {
                    GroupSearchResult.Visible = false;
                    Withdraw.Visible = false;
                    Deposite.Visible = false;
                    Twithdraw.Visible = false;
                    TDeposite.Visible = false;
                    Total.Visible = false;

                    dbConnection();
                    cmd.CommandText =
                        "Select  `expens_name` ,`Price` , `Date_time` from `all_monthlyexpenses` where Date_time Between '" + a.ToString("s") +
                        "' and '" + b.ToString("s") + "' ; ";

                    MySqlDataAdapter mySqlDataAdapterViewProduct = new MySqlDataAdapter {SelectCommand = cmd};
                    DataTable dTableViewProduct = new DataTable();
                    mySqlDataAdapterViewProduct.Fill(dTableViewProduct);
                    _connect.Close();


                    double MExpenses = 0;
                    SearchResult.DataSource = dTableViewProduct;
                    SearchResult.Columns[0].Width = 200;
                    SearchResult.Columns[2].Width = 200;

                    foreach (DataRow rowdel in dTableViewProduct.Rows)
                    {
                        MExpenses = MExpenses + Convert.ToDouble(rowdel["Price"]);
                    }


                    TDeposite.Text = "Total Monthly Expenses : " + MExpenses + " LE";
                    TDeposite.Visible = true;


                } //end else Monthly details

            }



            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Add22_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < a.Length; i++)
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText =
                    "INSERT INTO `corner`.`product` (`Product_name`,`ProductType_id`,`store`,`price`) VALUES ('" +
                    a[i] +  "','"+a2[i]+ "','0','0') ; ";
                MySqlDataReader reader2 = cmd.ExecuteReader();
                _connect.Close();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
           // textBox9.Text = Convert.ToString(dataGridView3.Rows[1].Cells[0].Value);
            for (int i =0;i< dataGridView3.Rows.Count;i++)
            {
                //  `Product_id`,`Product_name`,`Price`,`Store` ,`ProductType_id`
                dbConnection();
                cmd.Connection = _connect;
                cmd.CommandText =
                    "UPDATE `corner`.`product` SET `Product_name` = '" + Convert.ToString(dataGridView3.Rows[i].Cells[1].Value) +
                    "' , `Price`= '" + Convert.ToDouble(dataGridView3.Rows[i].Cells[2].Value) + "' , `Store`= '" + Convert.ToInt16(dataGridView3.Rows[i].Cells[3].Value) +
                    "' , `ProductType_id`= '" + Convert.ToInt16(dataGridView3.Rows[i].Cells[4].Value) +
                    "' where Product_id ='" + Convert.ToInt16(dataGridView3.Rows[i].Cells[0].Value) + "'; ";

                MySqlDataReader reader4 = cmd.ExecuteReader();
                reader4.Read();
                _connect.Close();


            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //dbConnection();
            //cmd.Connection = _connect;
            //cmd.CommandText =
            //    "ALTER TABLE product_process  ADD CatID int; ";
            //MySqlDataReader reader = cmd.ExecuteReader();
            //reader.Read();
            //_connect.Close();
            dbConnection();
            MySqlCommand cmd2 = new MySqlCommand();
            cmd2.Connection = _connect;
            cmd2.CommandText = "SELECT * FROM `product`  ;";
            MySqlDataAdapter mySqlDataAdapterBuy = new MySqlDataAdapter {SelectCommand = cmd2};
            DataTable dTableBuy = new DataTable();
            mySqlDataAdapterBuy.Fill(dTableBuy);
            _connect.Close();
            foreach (DataRow x in dTableBuy.Rows)
            {
                //dbConnection();
                //cmd.Connection = _connect;
                //cmd.CommandText =
                //    "SELECT * FROM `product_process`  WHERE Product_name = '" +Convert.ToString( x["Product_name"]) + "'; ";
                //MySqlDataReader reader = cmd.ExecuteReader();
                //_connect.Close();
                //if (reader.Read())
                //{

                


                dbConnection();
                MySqlCommand cmd3 = new MySqlCommand();
                cmd3.Connection = _connect;
                cmd3.CommandText =
                    "UPDATE product_process SET CatID = '" + Convert.ToString(x["ProductType_id"]) +
                    "' WHERE Product_name = '" + Convert.ToString(x["Product_name"]) + "'; ";
                MySqlDataReader reader3 = cmd3.ExecuteReader();
                reader3.Read();
                _connect.Close();
            //}

        }
    }

        private void SearchName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //label31.Text = SearchName.SelectedIndex.ToString();
        }

        private void SearchResult_FilterStringChanged(object sender, EventArgs e)
        {

        }

        private void GroupSearchResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }
    }
        }
    

