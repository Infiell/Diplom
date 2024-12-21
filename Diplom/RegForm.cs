using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace Diplom
{
    public partial class RegForm : Form
    {
        public RegForm()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            AuthForm AuthForm = new AuthForm();
            AuthForm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPassword.Text;
            string againPass = textBoxPassAgain.Text;
            string first_name = textBoxFName.Text;
            string second_name = textBoxSName.Text;
            string third_name = textBoxTName.Text;

            if (password == againPass)
            {
                if (checkBoxConf.Checked == true)
                {
                    if (string.IsNullOrWhiteSpace(textBoxLogin.Text) || string.IsNullOrWhiteSpace(textBoxFName.Text) || string.IsNullOrWhiteSpace(textBoxSName.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
                    {
                        MessageBox.Show("Не все данные заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        try
                        {
                            string queryUser = "SELECT COUNT(*) FROM users WHERE login = @login";
                            string query = "INSERT INTO users (login, password, first_name, second_name, third_name, image) VALUES (@login, @password, @first_name, @second_Name, @third_name, @image)";
                            MySqlParameter[] parameters = new MySqlParameter[]
                            {
                            new MySqlParameter("@login",login),
                            new MySqlParameter("@password",password),
                            new MySqlParameter("@first_name",first_name),
                            new MySqlParameter("@second_name",second_name),
                            new MySqlParameter("@third_name",third_name),
                            new MySqlParameter("@image", imageBlop)
                            };
                            DatabaseManager dbManager = new DatabaseManager("77.246.99.119", "Diploma_project", "root", "Hesoyam-123");
                            int loginCount = Convert.ToInt32(dbManager.ExecuteScalar(queryUser, parameters));
                            if (loginCount > 0)
                            {
                                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                dbManager.ExecuteNonQuery(query, parameters);
                                MessageBox.Show("Регистрация прошла успешно", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}. Проверьте соедниение с интернетом или обратитесь к администратору.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                    MessageBox.Show("Подтвердите конфедициальность данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public class DatabaseManager
        {
            private readonly string connectionString;

            public DatabaseManager(string server, string database, string username, string password)
            {
                connectionString = $"server={server};database={database};user={username};password={password};";
            }
            public void ExecuteNonQuery(string query, MySqlParameter[] parameters = null)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        command.ExecuteNonQuery();
                    }
                }
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

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.consultant.ru/document/cons_doc_LAW_61801/a15bab6050028753706f22c32fe60627a7be79f9/");
        }

        private byte[] imageBlop;
        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файлы изображений (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    long fileSizeInBytes = new FileInfo(filePath).Length;
                    long maxSizeInBytes = 1024 * 1024;

                    if (fileSizeInBytes <= maxSizeInBytes)
                    {
                        Image newImage = Image.FromFile(filePath);
                        pictureBox1.Image = newImage;
                        imageBlop = ConvertImageToBlob(filePath);
                    }
                    else
                    {
                        MessageBox.Show("Размер файла превышает лимит в 1 мб", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }
        private byte[] ConvertImageToBlob(string filePath)
        {
            byte[] imageBytes;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    imageBytes = br.ReadBytes((int)fs.Length);
                }
            }
            return imageBytes;
        }

        private void RegForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}