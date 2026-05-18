using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace NetflixCloneProject.DataAccess
{
    public class DbConnection
    {
        // Kendi MySQL şifreni "1234" yazan yere girmelisin.
        private string connectionString = "Server=localhost;Database=netflix_db;Uid=root;Pwd=1234;";

        public MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}