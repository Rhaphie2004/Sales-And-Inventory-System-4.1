using Guna.UI.WinForms;
using MySql.Data.MySqlClient;
using sims.Admin_Side.Stocks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sims.Admin_Side.Sales
{
    public partial class Manage_Saless : Form
    {
        private Inventory_Dashboard _inventoryDashboard;
        private Manage_Stockk _stock;
        private Add_Product _addProductForm;
        private Product_Saless _productSales;

        public Manage_Saless(Inventory_Dashboard inventoryDashboard, Manage_Stockk stock, Product_Saless productSales)
        {
            InitializeComponent();
            _inventoryDashboard = inventoryDashboard;
            _stock = stock;
            _productSales = productSales;
        }
        public GunaDataGridView ProductsDgv
        {
            get { return productsDgv; }
        }

        public Label CountProduct
        {
            get { return productCountTxt; }
        }

        private void Manage_Saless_Load(object sender, EventArgs e)
        {
            Populate();
            ProductsCount();
            searchComboBox();
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
                    productsDgv.DataSource = dt;
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
                        productCountTxt.Text = itemCount.ToString();
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
        private void searchComboBox()
        {
            searchCategoryCmb.Items.Clear();

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
                                searchCategoryCmb.Items.Add(reader["Category_Name"].ToString());
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

        private DataTable SearchInDatabase(string searchTerm)
        {
            DataTable dataTable = new DataTable();
            dbModule db = new dbModule();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string query = "SELECT Product_ID, Product_Name, Category, Product_Price, Quantity_Sold, Stock_Needed " +
                               "FROM products WHERE Product_Name LIKE @SearchTerm";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }

        private void ApplyFilters()
        {
            string searchText = searchProductTxt.Text.Trim();
            string selectedCategory = searchCategoryCmb.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(selectedCategory))
            {
                productsDgv.DataSource = SearchInDatabase(""); // Pass an empty string to retrieve all records
                return;
            }

            // Build the search term for database filtering
            List<string> filters = new List<string>();

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                filters.Add($"Category = '{selectedCategory.Replace("'", "''")}'"); // Escape single quotes
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                filters.Add($"Product_Name LIKE '%{searchText.Replace("'", "''")}%'"); // Escape single quotes
            }

            string searchTerm = string.Join(" AND ", filters);

            // Retrieve filtered data from the database
            DataTable filteredData = SearchInDatabase(searchText); // Use `searchText` to fetch from the database

            // If a category is selected, filter the returned data further
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                DataView dv = filteredData.DefaultView;
                dv.RowFilter = $"Category = '{selectedCategory.Replace("'", "''")}'";
                filteredData = dv.ToTable();
            }

            // Bind the filtered data to the DataGridView
            productsDgv.DataSource = filteredData;
        }

        private void ResetFilters()
        {
            searchCategoryCmb.SelectedIndex = -1;
            searchProductTxt.Clear();
            ApplyFilters();
        }

        private void NewProductBtn_Click(object sender, EventArgs e)
        {
            // Check if the Add_Product form is already open
            if (_addProductForm == null || _addProductForm.IsDisposed)
            {
                // Pass the valid _productSalesForm to Add_Product form
                var manage_Stock = new Manage_Stockk(_inventoryDashboard);
                _addProductForm = new Add_Product(this, this, _stock, _inventoryDashboard, _productSales);
                _addProductForm.Show();
            }
            else
            {
                // If the Add_Product form is already open, bring it to the front
                _addProductForm.BringToFront();
            }
        }

        private void searchCategoryCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void searchProductTxt_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            ResetFilters();
        }
    }
}
