using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static sims.Admin_Side.Sales.Add_Product;

namespace sims.Admin_Side.Sales
{
    public partial class Product_Saless : Form
    {
        public Product_Saless()
        {
            InitializeComponent();
        }

        public FlowLayoutPanel CoffeeLayoutPanel
        {
            get { return coffeeLayoutPanel; }
        }

        private void Product_Saless_Load(object sender, EventArgs e)
        {
            LoadProductButtons();
        }

        private void AddProductButton(string productID, string productName, string productPrice)
        {
            Button productButton = new Button
            {
                Width = 170,
                Height = 110,
                Text = $"{productName}\nPrice: ₱ {productPrice}",
                Tag = new ProductDetails
                {
                    ProductID = productID,
                    ProductName = productName,
                    ProductPrice = productPrice
                },
                BackColor = Color.FromArgb(222, 196, 125),
                Font = new Font("Poppins", 12),
                TextAlign = ContentAlignment.MiddleCenter
            };
            productButton.Click += ProductButton_Click;
            CoffeeLayoutPanel.Controls.Add(productButton);
        }

        private void ProductButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is ProductDetails productDetails)
            {
                string productID = productDetails.ProductID;
                Sales_Form detailsForm = new Sales_Form(productID);
                detailsForm.Show();
            }
        }

        private void LoadProductButtons()
        {
            dbModule db = new dbModule();
            MySqlConnection conn = db.GetConnection();
            MySqlCommand cmd = db.GetCommand();

            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT Product_ID, Product_Name, Product_Price FROM products";

                MySqlDataReader reader = cmd.ExecuteReader();
                CoffeeLayoutPanel.Controls.Clear();

                while (reader.Read())
                {
                    string productID = reader.GetInt32("Product_ID").ToString();
                    string productName = reader.GetString("Product_Name");
                    string productPrice = reader.GetDecimal("Product_Price").ToString("F2");

                    AddProductButton(productID, productName, productPrice);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
        }
    }
}
