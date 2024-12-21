using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Diplom
{
    public partial class BooksTableForm : Form
    {
        private bool isInitializingData = true;
        private void LoadData()
        {
            isInitializingData = true;
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123"; 
            string query = "SELECT * FROM books";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }
            isInitializingData = false;

        }
        public BooksTableForm()
        {
            InitializeComponent();
        }

        private void BooksTableForm_Load(object sender, EventArgs e)
        {
            LoadData();
            dataGridView1.Columns["idbooks"].Visible = false;
            dataGridView1.Columns["number"].HeaderText = "Номер книги";
            dataGridView1.Columns["book_name"].HeaderText = "Наиминование книги";
            dataGridView1.Columns["book_publisher"].HeaderText = "Издательство";
            dataGridView1.Columns["book_release"].HeaderText = "Год выхода";
            dataGridView1.Columns["book_author"].HeaderText = "Автор книги";
            dataGridView1.Columns["book_type"].HeaderText = "Жанр";
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var updatedValue = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
            var columnName = dataGridView1.Columns[e.ColumnIndex].Name;
            int rowId;
            if (dataGridView1["idbooks", e.RowIndex].Value == DBNull.Value)
            {
                rowId = 0;
            }
            else
            {
                rowId = Convert.ToInt32(dataGridView1["idbooks", e.RowIndex].Value);
            }

            UpdateDatabase("books", columnName, updatedValue, rowId);
        }

        private void UpdateDatabase(string tableName, string columnName, object updatedValue, int rowId)
        {
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123";

            string query = $"UPDATE {tableName} SET {columnName} = @updatedValue WHERE idbooks = @rowId";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@updatedValue", updatedValue);
                    cmd.Parameters.AddWithValue("@rowId", rowId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void DeleteRowFromDatabase(int rowId)
        {
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123";
            string query = "DELETE FROM books WHERE idbooks = @rowId";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@rowId", rowId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить эту строку?", "Удаление", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                int rowId = Convert.ToInt32(e.Row.Cells["idbooks"].Value);

                DeleteRowFromDatabase(rowId);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateLabelsFromSelectedRow();
        }

        private void UpdateLabelsFromSelectedRow()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];

                label2.Text = selectedRow.Cells["book_name"].Value.ToString();
                label10.Text = selectedRow.Cells["book_author"].Value.ToString();
                label8.Text = selectedRow.Cells["book_publisher"].Value.ToString();
                label9.Text = selectedRow.Cells["book_release"].Value.ToString();
                label12.Text = selectedRow.Cells["book_type"].Value.ToString();
            }
        }

        private void AddEmptyRowToDatabase()
        {
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123";
            string query = "INSERT INTO books (number, book_name, book_publisher, book_release, book_author, book_type) VALUES (NULL, NULL, NULL, NULL, NULL, NULL)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddEmptyRowToDatabase();
            LoadData(); // Перезагрузка данных для обновления DataGridView
        }
    }
}
