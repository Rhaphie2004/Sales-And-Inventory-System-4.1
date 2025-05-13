using Bunifu.UI.WinForms;
using Guna.UI.WinForms;
using MySql.Data.MySqlClient;
using sims.Admin_Side.Stocks;
using sims.Messages_Boxes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace sims.Admin_Side.Sales
{
    public partial class Add_Product : Form
    {
        private Manage_Saless count;
        private Manage_Saless dashboard;
        private Manage_Stockk _stock;
        private Inventory_Dashboard _inventoryDashboard;
        private Product_Saless _productSales;

        public Add_Product(Manage_Saless count, Manage_Saless dashboard, Manage_Stockk stock, Inventory_Dashboard inventoryDashboard, Product_Saless productSales)
        {
            InitializeComponent();
            this.count = count;
            this.dashboard = dashboard;

            _stock = stock;
            _inventoryDashboard = inventoryDashboard;
            _productSales = productSales;
        }

        public class ProductDetails
        {
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public string ProductPrice { get; set; }
            public string category { get; set; }
        }

        private void Add_Product_Load(object sender, EventArgs e)
        {
            stocks();
            Populate();
            ProductsCount();
            GenerateRandomItemID();
            LoadCategoriesProducts();
            label15.Text = DateTime.Now.ToString("ddd, d MMMM yyyy");
        }

        private void previewStock()
        {
            if (_stock != null)
            {
                _stock.ViewStock();
            }
            else
            {
                MessageBox.Show("Inventory Dashboard is not available.");
            }
        }

        private void previewStockDashboard()
        {
            if (_inventoryDashboard != null)
            {
                _inventoryDashboard.StockPreview();
            }
            else
            {
                MessageBox.Show("Inventory Dashboard is not available.");
            }
        }

        private void LoadCategoriesProducts()
        {
            string query = "SELECT Category_Name FROM categoriesproducts";
            dbModule db = new dbModule();

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categoryCmb.Items.Add(reader["Category_Name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private List<string> allStockItems = new List<string>();
        private bool isUpdatingComboBoxes = false;

        private void stocks()
        {
            string query = "SELECT Item_Name FROM stocks";
            dbModule db = new dbModule();

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            allStockItems.Clear();

                            while (reader.Read())
                            {
                                string itemName = reader["Item_Name"].ToString();
                                allStockItems.Add(itemName);
                            }
                        }
                    }
                }

                // Populate combo boxes
                PopulateComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading stocks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateComboBoxes()
        {
            stockCmb.Items.Clear();
            stock2Cmb.Items.Clear();
            stock3Cmb.Items.Clear();
            stock4Cmb.Items.Clear();
            stock5Cmb.Items.Clear();
            stock6Cmb.Items.Clear();

            foreach (var item in allStockItems)
            {
                stockCmb.Items.Add(item);
                stock2Cmb.Items.Add(item);
                stock3Cmb.Items.Add(item);
                stock4Cmb.Items.Add(item);
                stock5Cmb.Items.Add(item);
                stock6Cmb.Items.Add(item);
            }
        }
        
        private void UpdateComboBoxItems()
        {
            // Prevent recursive updates
            if (isUpdatingComboBoxes) return;

            isUpdatingComboBoxes = true;

            try
            {
                // Get selected items from all combo boxes
                var selectedItems = new HashSet<string>
        {
            stockCmb.SelectedItem?.ToString(),
            stock2Cmb.SelectedItem?.ToString(),
            stock3Cmb.SelectedItem?.ToString(),
            stock4Cmb.SelectedItem?.ToString(),
            stock5Cmb.SelectedItem?.ToString(),
            stock6Cmb.SelectedItem?.ToString()
        };

                // Update each combo box's items
                UpdateComboBox(stockCmb, selectedItems);
                UpdateComboBox(stock2Cmb, selectedItems);
                UpdateComboBox(stock3Cmb, selectedItems);
                UpdateComboBox(stock4Cmb, selectedItems);
                UpdateComboBox(stock5Cmb, selectedItems);
                UpdateComboBox(stock6Cmb, selectedItems);
            }
            finally
            {
                // Re-enable updates
                isUpdatingComboBoxes = false;
            }
        }

        private void UpdateComboBox(GunaComboBox comboBox, HashSet<string> selectedItems)
        {
            // Get the currently selected item
            var currentSelection = comboBox.SelectedItem?.ToString();

            // Clear and repopulate items
            comboBox.Items.Clear();
            foreach (var item in allStockItems)
            {
                if (!selectedItems.Contains(item) || item == currentSelection)
                {
                    comboBox.Items.Add(item);
                }
            }

            // Restore the current selection if still valid
            if (!string.IsNullOrEmpty(currentSelection))
            {
                comboBox.SelectedItem = currentSelection;
            }
        }

        private void GenerateRandomItemID()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000000, 99999999);
            productIDTxt.Text = randomNumber.ToString();
        }

        private void Populate()
        {
            dbModule db = new dbModule();
            MySqlDataAdapter adapter = db.GetAdapter();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM products";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    adapter.SelectCommand = command;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dashboard.ProductsDgv.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ProductsCount()
        {
            dbModule db = new dbModule();
            string query = "SELECT COUNT(*) FROM products";

            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        int itemCount = Convert.ToInt32(cmd.ExecuteScalar());
                        count.productCountTxt.Text = itemCount.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void addProductBtn_Click(object sender, EventArgs e)
        {
            addProduct();
        }

        private void addProduct()
        {
            dbModule db = new dbModule();
            MySqlConnection conn = db.GetConnection();
            MySqlCommand cmd = db.GetCommand();

            string productID = productIDTxt.Text.Trim();
            string productName = productNameTxt.Text.Trim();
            string category = categoryCmb.SelectedItem?.ToString() ?? string.Empty;
            string productPrice = productPriceTxt.Text.Trim();
            string stockQuantity = quantityStockTxt.Text.Trim();
            string quantitySold = quantitySoldTxt.Text.Trim();

            List<string> stockItems = new List<string>();
            if (!string.IsNullOrEmpty(stockCmb.SelectedItem?.ToString())) stockItems.Add(stockCmb.SelectedItem.ToString());
            if (!string.IsNullOrEmpty(stock2Cmb.SelectedItem?.ToString())) stockItems.Add(stock2Cmb.SelectedItem.ToString());
            if (!string.IsNullOrEmpty(stock3Cmb.SelectedItem?.ToString())) stockItems.Add(stock3Cmb.SelectedItem.ToString());
            if (!string.IsNullOrEmpty(stock4Cmb.SelectedItem?.ToString())) stockItems.Add(stock4Cmb.SelectedItem.ToString());
            if (!string.IsNullOrEmpty(stock5Cmb.SelectedItem?.ToString())) stockItems.Add(stock5Cmb.SelectedItem.ToString());
            if (!string.IsNullOrEmpty(stock6Cmb.SelectedItem?.ToString())) stockItems.Add(stock6Cmb.SelectedItem.ToString());
            // Add more stock items if necessary...

            string stockNeeded = string.Join(", ", stockItems);

            if (string.IsNullOrEmpty(productID) || string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(category) ||
                string.IsNullOrEmpty(stockQuantity) || string.IsNullOrEmpty(quantitySold))
            {
                new Field_Required().Show();
                return;
            }

            if (!int.TryParse(stockQuantity, out var parsedStockQuantity))
            {
                MessageBox.Show("Invalid stock quantity. Please enter a valid integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(quantitySold, out var parsedQuantitySold))
            {
                MessageBox.Show("Invalid quantity sold. Please enter a valid integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                conn.Open();
                cmd.Connection = conn;

                // Insert the product
                cmd.CommandText = "INSERT INTO products(Product_ID, Product_Name, Category, Product_Price, Stock_Quantity, Quantity_Sold, Stock_Needed)" +
                                  "VALUES (@Product_ID, @Product_Name, @Category, @Product_Price, @Stock_Quantity, @Quantity_Sold, @Stock_Needed)";
                cmd.Parameters.AddWithValue("@Product_ID", productID);
                cmd.Parameters.AddWithValue("@Product_Name", productName);
                cmd.Parameters.AddWithValue("@Category", category);
                cmd.Parameters.AddWithValue("@Product_Price", parsedPrice); // Use the parsed decimal price
                cmd.Parameters.AddWithValue("@Stock_Quantity", parsedStockQuantity); // Use parsed integer
                cmd.Parameters.AddWithValue("@Quantity_Sold", parsedQuantitySold); // Use parsed integer
                cmd.Parameters.AddWithValue("@Stock_Needed", stockNeeded);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Reduce stock quantities
                    foreach (var stockItem in stockItems)
                    {
                        cmd.CommandText = "UPDATE stocks SET Stock_In = Stock_In - @Stock_In WHERE Item_Name = @ItemName";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Stock_In", parsedStockQuantity);  // Use parsed stock quantity
                        cmd.Parameters.AddWithValue("@ItemName", stockItem);
                        cmd.ExecuteNonQuery();
                        previewStock();
                        previewStockDashboard();
                    }

                    MessageBox.Show("Product added successfully, and stock quantities updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset the form
                    productNameTxt.Clear();
                    categoryCmb.SelectedIndex = -1;
                    productPriceTxt.Clear();
                    quantitySoldTxt.Clear();
                    stockCmb.SelectedIndex = -1;
                    stock2Cmb.SelectedIndex = -1;
                    stock3Cmb.SelectedIndex = -1;
                    stock4Cmb.SelectedIndex = -1;
                    stock5Cmb.SelectedIndex = -1;
                    stock6Cmb.SelectedIndex = -1;
                    Populate();
                    this.Close();
                    ProductsCount();
                    GenerateRandomItemID();

                }
                else
                {
                    MessageBox.Show("Failed to add product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd.Dispose();
                conn.Dispose();
            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ValidateTextBoxForNumbersOnly(BunifuTextBox textBox)
        {
            string newText = textBox.Text;

            if (System.Text.RegularExpressions.Regex.IsMatch(newText, @"[a-zA-Z]"))
            {
                MessageBox.Show("Letters are not allowed!", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox.Text = System.Text.RegularExpressions.Regex.Replace(newText, @"[a-zA-Z]", "");
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void productNameTxt_TextChanged(object sender, EventArgs e)
        {
            string newText = productNameTxt.Text;

            if (System.Text.RegularExpressions.Regex.IsMatch(newText, @"\d"))
            {
                MessageBox.Show("Numbers are not allowed!", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                productNameTxt.Text = System.Text.RegularExpressions.Regex.Replace(newText, @"\d", "");
                productNameTxt.SelectionStart = productNameTxt.Text.Length;
            }
        }

        private void productPriceTxt_TextChanged(object sender, EventArgs e)
        {
            ValidateTextBoxForNumbersOnly(productPriceTxt);
        }

        private decimal parsedPrice;
        private void productPriceTxt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(productPriceTxt.Text))
            {
                // Remove peso sign and other formatting for processing
                string rawInput = productPriceTxt.Text.Replace("₱", "").Trim();

                if (decimal.TryParse(rawInput, out parsedPrice))
                {
                    // Format the input with the peso sign and two decimal places for display
                    productPriceTxt.Text = $"₱{parsedPrice:0.00}";
                }
                else
                {
                    MessageBox.Show("Invalid input. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    productPriceTxt.Clear();
                    parsedPrice = 0; // Reset the parsed value to ensure no invalid data is stored
                }
            }
        }

        private void quantityStockTxt_TextChanged(object sender, EventArgs e)
        {
            // Ensure the input is a valid number
            ValidateTextBoxForNumbersOnly(quantityStockTxt);

            // Update the quantitySoldTxt with the same value
            quantitySoldTxt.Text = quantityStockTxt.Text;
        }

        private void quantitySoldTxt_TextChanged(object sender, EventArgs e)
        {
            ValidateTextBoxForNumbersOnly(quantitySoldTxt);
        }

        private void stockCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboBoxItems();
        }

        private void stock2Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboBoxItems();
        }

        private void stock3Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboBoxItems();
        }

        private void stock4Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboBoxItems();
        }

        private void stock5Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboBoxItems();
        }

        private void stock6Cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboBoxItems();
        }
    }
}