using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Diplom
{
    public partial class ShortsStatForm : Form
    {
        public ShortsStatForm()
        {
            InitializeComponent();
        }

        public class DatabaseManager
        {
            private readonly string connectionString;

            public DatabaseManager(string server, string database, string username, string password)
            {
                connectionString = $"server={server};database={database};user={username};password={password};";
            }

            public int GetCount(string tableName)
            {
                string query = $"SELECT COUNT(*) FROM {tableName}";
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }

            public int GetCountByYear(string tableName, string yearColumn, int year)
            {
                string query = $"SELECT COUNT(*) FROM {tableName} WHERE {yearColumn} = @year";

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@year", year);
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }


        }

        private void ShortsStatForm_Load(object sender, EventArgs e)
        {
            DatabaseManager dbManager = new DatabaseManager("77.246.99.119", "Diploma_project", "root", "Hesoyam-123");

            int booksCount = dbManager.GetCount("books");
            int readersCount = dbManager.GetCount("readers");

            label8.Text = $"{booksCount}";
            label9.Text = $"{readersCount}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int year))
            {
                string selectedOption = comboBox1.SelectedItem.ToString();
                string tableName, yearColumn;

                switch (selectedOption)
                {
                    case "Читателям":
                        tableName = "readers";
                        yearColumn = "birthday";
                        break;
                    case "Книгам":
                        tableName = "books";
                        yearColumn = "book_release";
                        break;
                    default:
                        label7.Text = "Выберите валидную опцию.";
                        return;
                }

                DatabaseManager dbManager = new DatabaseManager("77.246.99.119", "Diploma_project", "root", "Hesoyam-123");
                int count = dbManager.GetCountByYear(tableName, yearColumn, year);

                label7.Text = $"Количество записей за {year} год: {count}";
            }
            else
            {
                label7.Text = "Ошибка: введите корректный год.";
            }
        }
    }
}
