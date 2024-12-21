using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace Diplom
{
    public partial class MainForm : Form
    {
        private string recData;
        private DatabaseManager dbManager;
        public MainForm(string loginSave)
        {
            InitializeComponent();
            dbManager = new DatabaseManager("77.246.99.119", "Diploma_project", "root", "Hesoyam-123");
            recData = loginSave;
        }

        private void LoadImageIntoPictureBox(string userLogin)
        {
            byte[] imageData = dbManager.GetImageFromDatabase(userLogin);
            if (imageData != null)
            {
                Image image = ConvertByteArrayToImageIm(imageData);
                if (image != null)
                {
                    pictureBox1.Image = image;
                }
            }
        }

        private Image ConvertByteArrayToImageIm(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            using (var ms = new MemoryStream(imageData))
            {
                return Image.FromStream(ms);
            }
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            timerElapsedTime.Start();
            timerCurrentTime.Start();
            string userLogin = recData; 
            string fullName = dbManager.GetFullNameByLogin(userLogin);
            label1.Text = fullName;
            LoadImageIntoPictureBox(userLogin);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string loginSave = recData;
            ChangeInfoForm ChangeInfoForm = new ChangeInfoForm(loginSave);
            ChangeInfoForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BooksTableForm BooksTableForm = new BooksTableForm();
            BooksTableForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShortsStatForm ShortsStatForm = new ShortsStatForm();
            ShortsStatForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private int secondsElapsed = 0;
        private void timerElapsedTime_Tick(object sender, EventArgs e)
        {
            secondsElapsed++;

            int hours = secondsElapsed / 3600;
            int minutes = (secondsElapsed % 3600) / 60;
            int seconds = secondsElapsed % 60;

            labelElapsedTime.Text = $"Время сессии: {hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        private void timerCurrentTime_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;

            labelCurrentTime.Text = "Текущее время: " + currentTime.ToString("HH:mm:ss");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AudenceForm AudenceForm = new AudenceForm();
            AudenceForm.Show();
        }

        public class DatabaseManager
        {
            private readonly string connectionString;

            public byte[] GetImageFromDatabase(string login)
            {
                byte[] imageData = null;
                string query = "SELECT image FROM users WHERE login = @login";

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read() && reader["image"] != DBNull.Value)
                            {
                                imageData = (byte[])reader["image"];
                            }
                        }
                    }
                }

                return imageData;
            }
            public DatabaseManager(string server, string database, string username, string password)
            {
                connectionString = $"server={server};database={database};user={username};password={password};";
            }

            public void ExecuteQuery(string query, Action<MySqlDataReader> readAction, MySqlParameter[] parameters = null)
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

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                readAction(reader);
                            }
                        }
                    }
                }
            }

            public string GetFullNameByLogin(string login)
            {
                string fullName = "";
                string query = "SELECT first_name, second_name, third_name FROM users WHERE login = @login";
                MySqlParameter[] parameters = new MySqlParameter[]
                {
            new MySqlParameter("@login", login)
                };

                ExecuteQuery(query, (reader) =>
                {
                    fullName = $"Добро пожаловать, {reader["first_name"]} {reader["second_name"]} {reader["third_name"]}.";
                }, parameters);

                return fullName;
            }

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
