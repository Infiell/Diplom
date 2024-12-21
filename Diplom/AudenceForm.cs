using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System;

namespace Diplom
{
    public partial class AudenceForm : Form
    {
        private bool isInitializingData = true;
        private void LoadData()
        {
            isInitializingData = true;
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123";
            string query = "SELECT * FROM readers";

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
        public AudenceForm()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void AudenceForm_Load(object sender, System.EventArgs e)
        {
            LoadData();
            dataGridView1.Columns["idreaders"].Visible = false;
            dataGridView1.Columns["number_reader"].HeaderText = "Номер читателя";
            dataGridView1.Columns["first_name"].HeaderText = "Имя";
            dataGridView1.Columns["second_name"].HeaderText = "Фамилия";
            dataGridView1.Columns["third_name"].HeaderText = "Отчество";
            dataGridView1.Columns["birthday"].HeaderText = "Дата рождения";
            dataGridView1.Columns["address"].HeaderText = "Адрес";
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var updatedValue = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
            var columnName = dataGridView1.Columns[e.ColumnIndex].Name;
            int rowId;
            if (dataGridView1["idreaders", e.RowIndex].Value == DBNull.Value)
            {
                rowId = 0;
            }
            else
            {
                rowId = Convert.ToInt32(dataGridView1["idreaders", e.RowIndex].Value);
            }

            UpdateDatabase("readers", columnName, updatedValue, rowId);
        }
        private void UpdateDatabase(string tableName, string columnName, object updatedValue, int rowId)
        {
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123";

            string query = $"UPDATE {tableName} SET {columnName} = @updatedValue WHERE idreaders = @rowId";

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
        private void AddDataToDatabase(int number_reader, string first_name, string second_name, string third_name, string birthday, string address)
        {
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123";

            string query = "INSERT INTO readers (number_reader, first_name, second_name, third_name, birthday, address) VALUES (@number_reader ,@first_name, @second_name, @third_name, @birthday, @address)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@number_reader", number_reader);
                    cmd.Parameters.AddWithValue("@first_name", first_name);
                    cmd.Parameters.AddWithValue("@second_name", second_name);
                    cmd.Parameters.AddWithValue("@third_name", third_name);
                    cmd.Parameters.AddWithValue("@birthday", birthday);
                    cmd.Parameters.AddWithValue("@address", address);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            int rowIndex = e.Row.Index - 1;

            if (rowIndex >= 0 && !isInitializingData)
            {
                var row = dataGridView1.Rows[rowIndex];
                var cellValue = row.Cells["number_reader"].Value;
                var number_reader = cellValue != DBNull.Value ? Convert.ToInt32(cellValue) : 0;
                var first_name = row.Cells["first_name"].Value?.ToString() ?? "";
                var second_name = row.Cells["second_name"].Value?.ToString() ?? "";
                var third_name = row.Cells["third_name"].Value?.ToString() ?? "";
                var birthday = row.Cells["birthday"].Value?.ToString() ?? "";
                var address = row.Cells["address"].Value?.ToString() ?? "";

                AddDataToDatabase(number_reader, first_name, second_name, third_name, birthday, address);
            }
        }

        private void DeleteRowFromDatabase(int rowId)
        {
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123";
            string query = "DELETE FROM readers WHERE idreaders = @rowId";

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
                int rowId = Convert.ToInt32(e.Row.Cells["idreaders"].Value);

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

                label8.Text = selectedRow.Cells["number_reader"].Value.ToString();
                label2.Text = $"{selectedRow.Cells["second_name"].Value.ToString()} {selectedRow.Cells["first_name"].Value.ToString()} {selectedRow.Cells["third_name"].Value.ToString()}";
                label6.Text = selectedRow.Cells["birthday"].Value.ToString();
                label7.Text = selectedRow.Cells["address"].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddEmptyRowToDatabase();
            LoadData(); // Перезагрузка данных для обновления DataGridView
        }

        private void AddEmptyRowToDatabase()
        {
            string connectionString = "server=77.246.99.119;database=Diploma_project;user=root;password=Hesoyam-123";
            string query = "INSERT INTO readers (number_reader, first_name, second_name, third_name, birthday, address) VALUES (NULL, NULL, NULL, NULL, NULL, NULL)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
