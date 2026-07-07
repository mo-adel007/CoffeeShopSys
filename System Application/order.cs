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
    public partial class order : Form
    {
        //public object product_id;

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

        int old_Value = 0;
        public Button bClick;
        NumericUpDown o;
        int thisValue;
       

        public order()
        {
            InitializeComponent();
            Theme.Apply(this);
 
        }

        private void change_price(object sender, EventArgs e)
        {
            o = (NumericUpDown)sender;
            thisValue = (int)o.Value;
            //old_Value = thisValue;

            try
            {
                dbConnection();
                cmd.Connection = _connect;
                cmd.Parameters.Clear();
                int idU = Convert.ToInt32(label4.Text);
                cmd.CommandText = "Select * from product where Product_id = @productId;";
                cmd.Parameters.AddWithValue("@productId", idU);
                SQLiteDataAdapter mySqlDataAdapterProduct = new SQLiteDataAdapter { SelectCommand = cmd };
                DataTable dTableProduct = new DataTable();
                mySqlDataAdapterProduct.Fill(dTableProduct);
                _connect.Close();

          
                    if (dTableProduct.Rows.Count == 1)
                    {

                    foreach (DataRow rowdel in dTableProduct.Rows)
                    {
                            int store = Convert.ToInt16(storeLab.Text);
                            if (Convert.ToInt16(rowdel["Store"]) >= thisValue)
                            {
                                if (thisValue > old_Value )
                                {
                                    decimal price = Convert.ToDecimal(label1.Text);
                                    decimal tot_price = price * thisValue;
                                    label2.Text = Convert.ToString(tot_price);

                                    store = store - 1;
                                    storeLab.Text = Convert.ToString(store);
                                }
        
                                else if (thisValue < old_Value && thisValue >= 1)
                                {
                                    decimal price = Convert.ToDecimal(label1.Text);
                                    decimal tot_price = price * thisValue;
                                    label2.Text = Convert.ToString(tot_price);

                                    store = store + 1;
                                    storeLab.Text = Convert.ToString(store);
                                    //old_Value = thisValue;
                                }

                                else if (thisValue < old_Value && thisValue == 0)
                                {
                                    //MessageBox.Show("ادخل رقم اكبر من 0", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    label2.Text = "0";

                                    store = store + 1;
                                    storeLab.Text = Convert.ToString(store);

                                }

                                else if (thisValue == 0 && store==0)
                                {
                                    label2.Text = "0.0";
                                }
                                
                                old_Value = thisValue;
                            }
                            else
                            {
                                MessageBox.Show("غير متوفر هذه الكمية من المنتج", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                o.Value = Convert.ToInt16(rowdel["Store"]);
                            }
                               

                        } // end foreach
                    } // end if

                    //this.Hide();



            }//end try

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            

    

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(numericUpDown1.Value == 0)
            {
                MessageBox.Show("ادخل رقم اكبر من 0", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(numericUpDown1.Value >0)
            {
                bClick.BackColor = Color.SkyBlue;

                try
                {
                   
                        decimal price = Convert.ToDecimal(label1.Text);
                        decimal total = Convert.ToDecimal(label2.Text);
                        int idpro = Convert.ToInt32(label4.Text);

                    
                    dbConnection();
                    cmd.Connection = _connect;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "INSERT INTO bill (pro_name, pro_price, quantity, tot_price, store, IDproduct) VALUES (@proName, @proPrice, @quantity, @totPrice, @store, @idProduct);";
                    cmd.Parameters.AddWithValue("@proName", label5.Text);
                    cmd.Parameters.AddWithValue("@proPrice", price);
                    cmd.Parameters.AddWithValue("@quantity", thisValue);
                    cmd.Parameters.AddWithValue("@totPrice", total);
                    cmd.Parameters.AddWithValue("@store", Convert.ToInt32(storeLab.Text));
                    cmd.Parameters.AddWithValue("@idProduct", idpro);
                    cmd.ExecuteNonQuery();
                    _connect.Close();
                      
                }//end try

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                this.Hide();
           
            }

            
        }
    }
}
