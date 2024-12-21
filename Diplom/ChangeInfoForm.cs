using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Diplom
{
    public partial class ChangeInfoForm : Form
    {
        private DatabaseManager dbManager;
        private readonly string recData;
        public ChangeInfoForm(string loginSave)
        {
            InitializeComponent();
            recData = loginSave;
            dbManager = new DatabaseManager("77.246.99.119", "Diploma_project", "root", "Hesoyam-123");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;
            string newpassword = textBox3.Text;
            string secondName = textBox4.Text;
            string firstName = textBox5.Text;
            string thirdName = textBox6.Text;

            if (password == newpassword)
            {
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(newpassword) || string.IsNullOrWhiteSpace(secondName) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(thirdName))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }

                UpdateUserData(login, password, secondName, firstName, thirdName);
            }
            else
            {
                MessageBox.Show("Пароли не совпадают");
            }
        }

        private void UpdateUserData(string login, string password, string secondName, string firstName, string thirdName)
        {
            string query = "UPDATE users SET login = @login, password = @password, second_name = @secondName, first_name = @firstName, third_name = @thirdName WHERE login = @recLogin";

            using (var connection = new MySqlConnection(dbManager.connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@recLogin", recData);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@secondName", secondName);
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@thirdName", thirdName);
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Данные обновлены.");
        }

        public class DatabaseManager
        {
            public string connectionString { get; private set; }

            public DatabaseManager(string server, string database, string username, string password)
            {
                connectionString = $"server={server};database={database};user={username};password={password};";
            }
        }
    }
}
