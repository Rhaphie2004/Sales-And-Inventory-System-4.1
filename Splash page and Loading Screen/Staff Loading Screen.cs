using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sims.Splash_page_and_Loading_Screen
{
    public partial class Staff_Loading_Screen : Form
    {
<<<<<<< HEAD:Admin Side/Sales/Sales Form.cs
        private string _productID;
        public Sales_Form(string productID)
=======
        public Staff_Loading_Screen()
>>>>>>> 76ff571b76b30d47259db34b2a5e8d7493ef0d44:Splash page and Loading Screen/Staff Loading Screen.cs
        {
            InitializeComponent();
            _productID = productID;
        }

        private void Sales_Form_Load(object sender, EventArgs e)
        {
            LoadProductDetails(_productID);
        }

        private void LoadProductDetails(string productID)
        {
            dbModule db = new dbModule();
            string query = "SELECT Product_ID, Product_Name, Category, Product_Price, Stock_Quantity, Quantity_Sold, Stock_Needed " +
                           "FROM products WHERE Product_ID = @Product_ID";

            using (MySqlConnection conn = db.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Product_ID", _productID);
                    try
                    {
                        conn.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                productIDTxt.Text = reader["Product_ID"].ToString();
                                productNameTxt.Text = reader["Product_Name"].ToString();
                                productPriceTxt.Text = reader["Product_Price"].ToString();
                                quantityStockTxt.Text = reader["Stock_Quantity"].ToString();
                                quantitySoldTxt.Text = reader["Quantity_Sold"].ToString();
                            }
                            else
                            {
                               MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while fetching product details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

    }
}
