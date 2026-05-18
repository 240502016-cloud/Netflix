using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace NetflixCloneProject.DataAccess
{
    public class KullaniciDAL
    {
        private string baglantiMetni = "Server=localhost;Database=netflix_db;Uid=root;Pwd=1234;";

        public bool KullaniciKayitEt(string ad, string soyad, string email, string sifre, DateTime dogumTarihi, string cinsiyet, string ulke, List<string> secilenTurler)
        {
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    using (MySqlCommand emailCmd = new MySqlCommand("SELECT COUNT(*) FROM Kullanici WHERE email = @email", baglanti))
                    {
                        emailCmd.Parameters.AddWithValue("@email", email);
                        if (Convert.ToInt32(emailCmd.ExecuteScalar()) > 0) return false;
                    }

                    string kayitSorgu = "INSERT INTO Kullanici (ad, soyad, email, sifre, dogum_tarihi, cinsiyet, ulke, rol_id) VALUES (@ad, @soyad, @email, @sifre, @dt, @cins, @ulke, 1); SELECT LAST_INSERT_ID();";
                    int yeniId = 0;
                    using (MySqlCommand cmd = new MySqlCommand(kayitSorgu, baglanti))
                    {
                        cmd.Parameters.AddWithValue("@ad", ad); cmd.Parameters.AddWithValue("@soyad", soyad); cmd.Parameters.AddWithValue("@email", email); cmd.Parameters.AddWithValue("@sifre", sifre);
                        cmd.Parameters.AddWithValue("@dt", dogumTarihi.ToString("yyyy-MM-dd")); cmd.Parameters.AddWithValue("@cins", cinsiyet); cmd.Parameters.AddWithValue("@ulke", ulke);
                        yeniId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    foreach (string tur in secilenTurler)
                    {
                        using (MySqlCommand tCmd = new MySqlCommand("SELECT id FROM Tur WHERE tur_adi = @tadi", baglanti))
                        {
                            tCmd.Parameters.AddWithValue("@tadi", tur); object res = tCmd.ExecuteScalar();
                            if (res != null)
                            {
                                using (MySqlCommand bCmd = new MySqlCommand("INSERT INTO KullaniciTur (kullanici_id, tur_id) VALUES (@k, @t)", baglanti))
                                {
                                    bCmd.Parameters.AddWithValue("@k", yeniId); bCmd.Parameters.AddWithValue("@t", Convert.ToInt32(res)); bCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    return true;
                }
                catch { return false; }
            }
        }

        public int[] KullaniciGirisYapDetayli(string email, string sifre)
        {
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT id, rol_id, IFNULL(pasif_mi, 0) FROM Kullanici WHERE email = @e AND sifre = @s", baglanti))
                    {
                        cmd.Parameters.AddWithValue("@e", email); cmd.Parameters.AddWithValue("@s", sifre);
                        using (var r = cmd.ExecuteReader()) { if (r.Read()) return new int[] { Convert.ToInt32(r[0]), Convert.ToInt32(r[1]), Convert.ToInt32(r[2]) }; }
                    }
                }
                catch { }
                return new int[] { -1, -1, -1 };
            }
        }

        public DataRow ProfilGetir(int userId)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    // Kullanıcı bilgilerini ve favori türlerini birleşik çeker
                    string sql = @"SELECT k.ad, k.soyad, k.email, k.dogum_tarihi, k.ulke, 
                                   IFNULL((SELECT GROUP_CONCAT(t.tur_adi SEPARATOR ', ') FROM KullaniciTur kt JOIN Tur t ON kt.tur_id = t.id WHERE kt.kullanici_id = k.id), 'Seçilmemiş') AS FavoriTurler 
                                   FROM Kullanici k WHERE k.id = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, baglanti); cmd.Parameters.AddWithValue("@id", userId);
                    new MySqlDataAdapter(cmd).Fill(dt);
                }
                catch { }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public void ProfilGuncelle(int userId, string ad, string soyad, string ulke, string sifre)
        {
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open(); string sql = "UPDATE Kullanici SET ad=@a, soyad=@s, ulke=@u " + (!string.IsNullOrEmpty(sifre) ? ", sifre=@p " : "") + " WHERE id=@id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, baglanti))
                    {
                        cmd.Parameters.AddWithValue("@a", ad); cmd.Parameters.AddWithValue("@s", soyad); cmd.Parameters.AddWithValue("@u", ulke); cmd.Parameters.AddWithValue("@id", userId);
                        if (!string.IsNullOrEmpty(sifre)) cmd.Parameters.AddWithValue("@p", sifre); cmd.ExecuteNonQuery();
                    }
                }
                catch { }
            }
        }

        public string IstatistikGetir(int userId)
        {
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT SUM(izleme_suresi), COUNT(DISTINCT program_id), AVG(verilen_puan) FROM KullaniciProgram WHERE kullanici_id=" + userId, baglanti);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            string sure = r[0] != DBNull.Value ? r[0].ToString() : "0"; string adet = r[1] != DBNull.Value ? r[1].ToString() : "0"; string puan = r[2] != DBNull.Value ? Convert.ToDouble(r[2]).ToString("0.0") : "0";
                            return $"Toplam İzleme Süresi:\t{sure} Dk\nİzlenen İçerik Sayısı:\t{adet}\nVerilen Ort. Puan:\t{puan} / 10";
                        }
                    }
                }
                catch { }
            }
            return "Henüz istatistik bulunmuyor.";
        }
    }
}