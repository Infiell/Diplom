using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Diplom
{
    public partial class AuthForm : Form
    {
        public string loginSave;
        public AuthForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegForm RegForm = new RegForm();
            RegForm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPassword.Text;
            string query = "SELECT * FROM users WHERE login = @login AND password = @password";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@login", login),
                new MySqlParameter("@password", password)
            };

            DatabaseManager dbManager = new DatabaseManager("77.246.99.119", "Diploma_project", "root", "Hesoyam-123");
            int loginCountAndPass = Convert.ToInt32(dbManager.ExecuteScalar(query, parameters));
            if (loginCountAndPass > 0)
            {
                string loginSave = login;
                MainForm MainForm = new MainForm(loginSave);
                MainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Не правильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AuthForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }

    public class DatabaseManager
    {
        private readonly string connectionString;

        public DatabaseManager(string server, string database, string username, string password)
        {
            connectionString = $"server={server};database={database};user={username};password={password};";
        }
        public object ExecuteScalar(string query, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }

    }

}
