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
    public partial class worker : Form
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
        int count=0;
        int chooseType;
        public object pro_id;
        string reasonT, proN, personT, typeT , who_add;
        int q;

        public Button b , button ;


        public worker()
        {
            InitializeComponent();

            createButtons();
            CDay();
            button2.Visible = true;
        }

        private void createButtons()
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from product ORDER BY ProductType_id ASC ; ";

                SQLiteDataAdapter mySqlDataAdapterProduct = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableProduct = new DataTable();
                mySqlDataAdapterProduct.Fill(dTableProduct);

                _connect.Close();

                    int i, j, rows;
                    i = 1148;
                    j = 6;
                    rows = 0;

                    if (dTableProduct.Rows.Count > 0)
                    {
                    foreach (DataRow rowdel in dTableProduct.Rows)
                    {
                            b = new Button();
                            b.Location = new Point(i, j);
                            b.Text = Convert.ToString(rowdel["Product_name"]);
                            b.Tag = Convert.ToInt16(rowdel["Product_id"]);
                            b.Name = Convert.ToString(rowdel["Product_id"]);
                            b.Size = new Size(150, 35);
                            b.Click += new EventHandler(button_Click);
                            tabPage1.Controls.Add(b);
                            if (rows <= 13)
                            {
                                rows = rows + 1;
                                j = j + 40;
                            }
                            else
                            {
                                rows = 0;

                                j = 6;
                                i = i - 155;
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            button = sender as Button;
            label1.Text = Convert.ToString(button.Name);

            if (button.BackColor == Color.SkyBlue)
            {
                button.BackColor = Color.Silver;

                try
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "Delete  from bill where pro_name=@proName; ";
                    cmd.Parameters.AddWithValue("@proName", button.Text);
                    cmd.ExecuteNonQuery();
                    _connect.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else
            {
                order frm = new order();
                frm.bClick = button as Button;


                try
                {
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "Select * from product where Product_name=@proName; ";
                    cmd.Parameters.AddWithValue("@proName", button.Text);

                    SQLiteDataAdapter mySqlDataAdapterlogin = new SQLiteDataAdapter { SelectCommand = cmd };
                    DataTable dTableLogin = new DataTable();
                    mySqlDataAdapterlogin.Fill(dTableLogin);

                    _connect.Close();

                        if (dTableLogin.Rows.Count == 1)
                        {
                        foreach (DataRow rowdel in dTableLogin.Rows)
                        {

                                frm.label1.Text = Convert.ToString(rowdel["Price"] );
                                frm.label4.Text = Convert.ToString(button.Tag);
                                frm.label5.Text = button.Text;

                                frm.btn_name.Text = button.Name;
                                frm.label2.Text = Convert.ToString(rowdel["Price"]);
                                frm.storeLab.Text = Convert.ToString(rowdel["Store"] );
                                frm.Show();
                                break;
                            }
                        }

                        else
                            label1.Text = "Error " + button.Text;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Delete  from bill; ";
                cmd.ExecuteNonQuery();
                _connect.Close();

                    string now = DTime();

                dbConnection();
                cmd.Connection = _connect;

                cmd.Parameters.Clear();
                cmd.CommandText =
                    "INSERT INTO timing (Login_Date_time,User_Name,Logout_date_time,User_id) VALUES (@loginDateTime, @userName, @logoutDateTime, @userId) ; ";
                cmd.Parameters.AddWithValue("@loginDateTime", Convert.ToDateTime(label2.Text).ToString("s"));
                cmd.Parameters.AddWithValue("@userName", label3.Text);
                cmd.Parameters.AddWithValue("@logoutDateTime", now);
                cmd.Parameters.AddWithValue("@userId", Convert.ToInt16(UNameID.Text));
                cmd.ExecuteNonQuery();
                _connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            LoginForm frm = new LoginForm();
            frm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Client_Bill Bill_frm = new Client_Bill();
            Bill_frm.UNid.Text = UNameID.Text;
            Bill_frm.label1.Text = label3.Text;
            Bill_frm.DTimeLogin.Text = label2.Text;
            Bill_frm.shiftNum.Text = shiftNu.Text;

            Bill_frm.Show();

            tabPage1.Controls.Clear();
            createButtons();
            //this.Hide();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string type_process = comboBox1.Text;
            //textBox1.Text = type_process;

            if(comboBox1.Text == "شراء سلعة")
            {
                proName.Text = "اسم السلعة";
                proName.Visible = true;
                productName.Visible = true;
                label5.Visible = true;
                numericUpDown2.Visible = true;
                label8.Visible = false;
                textBox1.Visible = false;
                chooseType = 2;
            }
            else if(comboBox1.Text == "مسحوبات")
            {
                proName.Text = "اسم الساحب";
                proName.Visible = true;
                productName.Visible = true;
                label5.Visible = false;
                numericUpDown2.Visible = false;
                label8.Visible = true ;
                textBox1.Visible = true ;
                chooseType = 1;
            }
            else if (comboBox1.Text == "إضافة مبلغ")
            {
                proName.Text = "اسم المودع ";
                proName.Visible = true;
                productName.Visible = true;
                productName.Text = "";
                label5.Visible = false;
                numericUpDown2.Visible = false;
                label8.Visible = true;
                textBox1.Visible = true;
                chooseType = 3;
            }
            else if (comboBox1.Text == "اخرى")
            {
                proName.Text = "اسم/نوع المصروف";
                proName.Visible = true;
                productName.Visible = true;
                label5.Visible = false;
                numericUpDown2.Visible = false;
                label8.Visible = false ;
                textBox1.Visible = false ;
                chooseType = 4;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ( string.IsNullOrEmpty(priceTB.Text) || string.IsNullOrEmpty(comboBox1.Text) )
            {

                MessageBox.Show("برجاء ادخال البيانات", "خطـــأ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                switch (chooseType)
                {
                    case 1:
                        ChTake();
                        AddSafe();
                        break;

                    case 2:
                        ChBuyPro();
                        AddSafe();
                        break;
                    case 3:
                        AddMoney();
                        AddSafe();
                        break;
                    case 4:
                        ChOther();
                        AddSafe();
                        break;
                    default:
                        break;
                }

            }
        }

        private void AddMoney()
        {
            proN = reasonT = personT = typeT = "";
            reasonT = textBox1.Text;
            who_add = productName.Text;
            //typeT = productName.Text;
            q = 0;
        }

        private void returnAsStart()
        {
            comboBox1.Text = "";
            priceTB.Text = "";
            proName.Visible = false;
            productName.Visible = false;
            productName.Text = "";
            label5.Visible = false;
            numericUpDown2.Value = 0;
            numericUpDown2.Visible = false;
            textBox1.Visible =false;
            textBox1.Text = "";
            label8.Visible = false;

        }

        private void AddSafe()
        {
            try
            {
                   string now = DTime();

                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText =
                    "INSERT INTO safe(Type,price,reason,proName,quantity,ReasonOfT,PersonTake,UserN,Dtime,Userid,Who_personAdd) VALUES (@type, @price, @reason, @proName, @quantity, @reasonOfT, @personTake, @userN, @dtime, @userid, @whoPersonAdd) ; ";
                cmd.Parameters.AddWithValue("@type", comboBox1.Text);
                cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(priceTB.Text));
                cmd.Parameters.AddWithValue("@reason", reasonT);
                cmd.Parameters.AddWithValue("@proName", proN);
                cmd.Parameters.AddWithValue("@quantity", q);
                cmd.Parameters.AddWithValue("@reasonOfT", typeT);
                cmd.Parameters.AddWithValue("@personTake", personT);
                cmd.Parameters.AddWithValue("@userN", label3.Text);
                cmd.Parameters.AddWithValue("@dtime", now);
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt32(UNameID.Text));
                cmd.Parameters.AddWithValue("@whoPersonAdd", who_add);
                cmd.ExecuteNonQuery();
                _connect.Close();

                       int shift_N = Convert.ToInt32(shiftNu.Text);
                       string processType;
                       if (comboBox1.Text == "إضافة مبلغ")
                       {
                           processType = "sell";
                       }
                       else
                           processType = "buy";

                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText =
                    "INSERT INTO close_shift (processT,price,reason,proName,quantity,ReasonOfT,PersonTake,UserN,Dtime,Userid,ShiftNumber,WhoPersonAdd)VALUES (@processT, @price, @reason, @proName, @quantity, @reasonOfT, @personTake, @userN, @dtime, @userid, @shiftNumber, @whoPersonAdd) ; ";
                cmd.Parameters.AddWithValue("@processT", processType);
                cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(priceTB.Text));
                cmd.Parameters.AddWithValue("@reason", reasonT);
                cmd.Parameters.AddWithValue("@proName", proN);
                cmd.Parameters.AddWithValue("@quantity", q);
                cmd.Parameters.AddWithValue("@reasonOfT", typeT);
                cmd.Parameters.AddWithValue("@personTake", personT);
                cmd.Parameters.AddWithValue("@userN", label3.Text);
                cmd.Parameters.AddWithValue("@dtime", now);
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt32(UNameID.Text));
                cmd.Parameters.AddWithValue("@shiftNumber", shift_N);
                cmd.Parameters.AddWithValue("@whoPersonAdd", who_add);
                cmd.ExecuteNonQuery();
                _connect.Close();

                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText =
                    "INSERT INTO close_day(processT,price,reason,proName, quantity,ReasonOfT , PersonT,UserN,Dtime,Userid,ShiftNumber,whoPerson_add)VALUES (@processT, @price, @reason, @proName, @quantity, @reasonOfT, @personT, @userN, @dtime, @userid, @shiftNumber, @whoPersonAdd) ; ";
                cmd.Parameters.AddWithValue("@processT", processType);
                cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(priceTB.Text));
                cmd.Parameters.AddWithValue("@reason", reasonT);
                cmd.Parameters.AddWithValue("@proName", proN);
                cmd.Parameters.AddWithValue("@quantity", q);
                cmd.Parameters.AddWithValue("@reasonOfT", typeT);
                cmd.Parameters.AddWithValue("@personT", personT);
                cmd.Parameters.AddWithValue("@userN", label3.Text);
                cmd.Parameters.AddWithValue("@dtime", now);
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt16(UNameID.Text));
                cmd.Parameters.AddWithValue("@shiftNumber", shift_N);
                cmd.Parameters.AddWithValue("@whoPersonAdd", who_add);
                cmd.ExecuteNonQuery();
                _connect.Close();

                       MessageBox.Show("تمت اضافة العملية بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }//end try

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            returnAsStart();
        }

        private void ChOther()
        {

            proN = reasonT = personT = who_add ="";
            typeT = productName.Text;
            q = 0;

        }

        private string DTime()
        {
            //try
            //{
            //    return  TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time"));
            //}
            //catch
            //{
            //    return  DateTime.Now;
            //}
            return DateTime.Now.ToString("s");
        }

        private void ChBuyPro()
        {
            reasonT = typeT = personT = who_add = "";
            proN = productName.Text;
            q = Convert.ToInt32 (numericUpDown2.Value);
        }

        private void ChTake()
        {
            reasonT = textBox1.Text;
            proN = typeT = who_add = "";
            personT = productName.Text;
            q = 0;

        }



        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {

                case 0:
                    button2.Visible = true;
                    break;
                case 2:
                    CloseShiftTab();
                    break;
                case 3:
                    CDay();
                    break;
                case 4:
                    productComboBox();
                    break;
                default:
                    button2.Visible = false;
                    break;
            }

        }

        private void CDay()
        {
            button2.Visible = false;
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from close_day WHERE processT = 'sell' ;";
                SQLiteDataAdapter mySqlDataAdaptersell = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableSell = new DataTable();
                mySqlDataAdaptersell.Fill(dTableSell);
                _connect.Close();
                dbConnection();

                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from close_day WHERE  processT = 'buy' ;";
                SQLiteDataAdapter mySqlDataAdapterBuy = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableBuy = new DataTable();
                mySqlDataAdapterBuy.Fill(dTableBuy);

                _connect.Close();


                    dataGridView1.DataSource = dTableSell;
                    dataGridView2.DataSource = dTableBuy;
                    decimal sumSell, sumBuy;
                    sumSell = sumBuy =0 ;
                foreach (DataRow rowdel in dTableSell.Rows)
                {
                       sumSell = sumSell + Convert.ToDecimal(rowdel["price"]) ;
                    }
                foreach (DataRow rowdel in dTableBuy.Rows)
                {
                        sumBuy = sumBuy + Convert.ToDecimal(rowdel["price"]);
                    }
                    decimal profit = sumSell - sumBuy ;
                    label16.Text = Convert.ToString(sumSell);
                    label6.Text = Convert.ToString(sumBuy);
                    label7.Text = Convert.ToString(profit);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseShiftTab()
        {
            button2.Visible = false;
            int shNumber = Convert.ToInt32(shiftNu.Text);
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from close_shift WHERE processT = 'buy' and ShiftNumber = @shiftNumber ;";
                cmd.Parameters.AddWithValue("@shiftNumber", shNumber);
                SQLiteDataAdapter mySqlDataAdapterBuy = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableBuy = new DataTable();
                mySqlDataAdapterBuy.Fill(dTableBuy);
                _connect.Close();

                    decimal totalS = 0; decimal totalB = 0;
                foreach (DataRow rowdel in dTableBuy.Rows)
                {
                        totalB = totalB + Convert.ToDecimal(rowdel["price"]);
                    }

                    label13.Text = Convert.ToString(totalB) ;

                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from close_shift WHERE processT = 'sell' and ShiftNumber = @shiftNumber ;";
                cmd.Parameters.AddWithValue("@shiftNumber", shNumber);
                SQLiteDataAdapter mySqlDataAdaptersell = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableSell = new DataTable();
                mySqlDataAdaptersell.Fill(dTableSell);
                _connect.Close();

                foreach (DataRow rowdel in dTableSell.Rows)
                {
                        totalS = totalS + Convert.ToDecimal(rowdel["price"] );
                    }
                    dataGridView5.DataSource = dTableSell;
                    dataGridView4.DataSource = dTableBuy;

                    label12.Text = Convert.ToString(totalS) ;

                    label14.Text = Convert.ToString(totalS - totalB) ;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void productComboBox()
        {
            button2.Visible = false;
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select Product_name ,Store  from product ; ";

                SQLiteDataAdapter mySqlDataAdapterProduct = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableProduct = new DataTable();
                mySqlDataAdapterProduct.Fill(dTableProduct);
                _connect.Close();

                dataGridView3.DataSource = dTableProduct;
                dataGridView3.Columns[0].Width = 250;
                dataGridView3.Columns[1].Width = 200;

                foreach (DataRow rowdel in dTableProduct.Rows)
                {
                        productsCB.Items.Add(Convert.ToString(rowdel["Product_name"]));
                    }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //اضافة للمخزن
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select *  from product where Product_name =@proName; ";
                cmd.Parameters.AddWithValue("@proName", productsCB.Text);

                SQLiteDataAdapter mySqlDataAdapterProduct = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableProduct = new DataTable();
                mySqlDataAdapterProduct.Fill(dTableProduct);
                _connect.Close();


                    int plusStoreN =  Convert.ToInt16 (numericUpDown3.Value);
                        if (dTableProduct.Rows.Count == 1)
                        {

                    foreach (DataRow rowdel in dTableProduct.Rows)
                    {
                                int store =Convert.ToInt16(rowdel["Store"])  + plusStoreN;
                        dbConnection();
                        cmd.Connection = _connect;
                        cmd.Parameters.Clear();
                        cmd.CommandText =
                            "UPDATE product SET Store = @store where Product_name =@proName; ";
                        cmd.Parameters.AddWithValue("@store", store);
                        cmd.Parameters.AddWithValue("@proName", productsCB.Text);

                        cmd.ExecuteNonQuery();
                        _connect.Close();
                    }
                        }

                        MessageBox.Show("تمت الاضافة بنجاح", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        productsCB.Text = "";
                        numericUpDown3.Value = 0;

                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select  Product_name ,Store from product ; ";

                SQLiteDataAdapter mySqlDataAdapterViewProduct = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableViewProduct = new DataTable();
                mySqlDataAdapterViewProduct.Fill(dTableViewProduct);
                _connect.Close();

                        dataGridView3.DataSource = dTableViewProduct;
                        dataGridView3.Columns[0].Width = 250;
                        dataGridView3.Columns[1].Width = 200;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {


            Admin frm = new Admin();
            frm.DTime.Text = label2.Text ;
            frm.UserName.Text = label3.Text;
            frm.UserID.Text =UNameID.Text ;
            frm.shiftNu.Text = "0";
            frm.Show();
            this.Hide();


        }

        private void CloseShift_Click(object sender, EventArgs e)
        {
            try
            {
                    int shN = Convert.ToInt16(shiftNu.Text);
                     decimal   TBuy = Convert.ToDecimal(label13.Text);
                     decimal   TSell = Convert.ToDecimal(label12.Text);
                      decimal  profitShift = Convert.ToDecimal(label14.Text);


                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO shift_details(ShiftNum,TotalSell, TotalBuy, ProfitShift)  VALUES (@shiftNum, @totalSell, @totalBuy, @profitShift);";
                cmd.Parameters.AddWithValue("@shiftNum", shN);
                cmd.Parameters.AddWithValue("@totalSell", TSell);
                cmd.Parameters.AddWithValue("@totalBuy", TBuy);
                cmd.Parameters.AddWithValue("@profitShift", profitShift);
                cmd.ExecuteNonQuery();
                _connect.Close();

                    CloseShift.Enabled = false;
                    tabControl1.TabPages.Remove(tabPage1);
                    tabControl1.TabPages.Remove(tabPage2);
                    CloseDay.Enabled = true;

                //take from closeShift and set in closeDay
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "DELETE FROM close_shift; ";
                cmd.ExecuteNonQuery();
                _connect.Close();


                button1.Enabled = true;

            }//end try

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseDay_Click(object sender, EventArgs e)
        {
            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from day_details ; ";
                SQLiteDataAdapter mySqlDataAdapterUserShify = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableUserShift = new DataTable();
                mySqlDataAdapterUserShify.Fill(dTableUserShift);
                _connect.Close();

                    int DNumber;
                    if ( dTableUserShift.Rows.Count > 0)
                    {
                        DNumber = dTableUserShift.Rows.Count + 1;

                    }
                    else
                        DNumber = 1;

                    int DNum = DNumber;
                    decimal TSell = Convert.ToDecimal(label16.Text);
                    decimal TBuy = Convert.ToDecimal(label6.Text);
                    decimal profitDay = Convert.ToDecimal(label7.Text);

                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO day_details ( DayNum,TotalSell,TotalBuy, ProfitDay) VALUES (@dayNum, @totalSell, @totalBuy, @profitDay);";
                cmd.Parameters.AddWithValue("@dayNum", DNumber);
                cmd.Parameters.AddWithValue("@totalSell", TSell);
                cmd.Parameters.AddWithValue("@totalBuy", TBuy);
                cmd.Parameters.AddWithValue("@profitDay", profitDay);
                cmd.ExecuteNonQuery();
                _connect.Close();

                    CloseDay.Enabled = false;
                    tabControl1.TabPages.Remove(tabPage3);

                //take from closeShift and set in closeDay
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                cmd.CommandText = "Delete  from close_day ; Delete  from shift_details ;Delete  from user_shift ; ";
                cmd.ExecuteNonQuery();
                _connect.Close();

                button1.Enabled = true;

            }//end try

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CDay();
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            productComboBox();
        }


    }
}
