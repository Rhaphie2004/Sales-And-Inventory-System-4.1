using MySql.Data.MySqlClient;
using sims.Admin_Side;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sims.Messages_Boxes
{
    public partial class Failed_Attempts : Form
    {
<<<<<<< HEAD:Staff Side/Items/Manage_Items_Staff.cs
        private Dashboard_Inventory_Staff _inventoryDashboard;

        public Manage_Items_Staff(Dashboard_Inventory_Staff inventoryDashboard)
=======
        public Failed_Attempts()
>>>>>>> 76ff571b76b30d47259db34b2a5e8d7493ef0d44:Messages Boxes/Failed Attempts.cs
        {
            InitializeComponent();
            _inventoryDashboard = inventoryDashboard;
        }

        public DataGridView ItemsDgv
        {
            get { return itemsDgv; }
        }

        public Label CountItem
        {
            get { return itemCountTxt; }
        }

        private void Manage_Items_Staff_Load(object sender, EventArgs e)
        {
            Populate();
            ItemsCount();
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
                    string query = "SELECT * FROM items";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    adapter.SelectCommand = command;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    itemsDgv.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void previewStock()
        {
            if (_inventoryDashboard != null)
            {
                _inventoryDashboard.ItemsCount();
            }
            else
            {
                MessageBox.Show("Inventory Dashboard is not available.");
            }
        }

        private void searchComboBox()
        {
            searchCategoryCmb.Items.Clear();

            string query = "SELECT Category_Name FROM categories";
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

        private void ItemsCount()
        {
            dbModule db = new dbModule();
            string query = "SELECT COUNT(*) FROM items";

            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        int itemCount = Convert.ToInt32(cmd.ExecuteScalar());
                        itemCountTxt.Text = itemCount.ToString();
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

        private DataTable SearchInDatabase(string searchTerm)
        {
            DataTable dataTable = new DataTable();
            dbModule db = new dbModule();

            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string query = "SELECT Item_ID, Item_Name, Category, Item_Description, Item_Image " +
                               "FROM items WHERE Item_Name LIKE @SearchTerm";

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
            string searchText = searchItemTxt.Text.Trim();
            string selectedCategory = searchCategoryCmb.SelectedItem?.ToString();

            // If no filters are applied, show all data
            if (string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(selectedCategory))
            {
                itemsDgv.DataSource = SearchInDatabase(""); // Pass an empty string to retrieve all records
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
                filters.Add($"Item_Name LIKE '%{searchText.Replace("'", "''")}%'"); // Escape single quotes
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
            itemsDgv.DataSource = filteredData;
        }

        private void ResetFilters()
        {
            searchCategoryCmb.SelectedIndex = -1; // Reset category filter
            searchItemTxt.Clear();               // Clear text filter
            ApplyFilters();                      // Reapply filters to reset DataView
        }

        private void searchCategoryCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void searchItemTxt_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            ResetFilters();
        }

        private void continueBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
