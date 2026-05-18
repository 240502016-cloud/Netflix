using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace NetflixCloneProject.DataAccess
{
    public class YoneticiDAL
    {
        private string baglantiMetni = "Server=localhost;Database=netflix_db;Uid=root;Pwd=1234;";

        public DataTable RaporCek(int raporTipi)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection bag = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    bag.Open(); string sql = "";
                    switch (raporTipi)
                    {
                        case 0: sql = "SELECT ad AS 'İçerik Adı', izlenme_sayisi AS 'İzlenme' FROM Program ORDER BY izlenme_sayisi DESC LIMIT 10"; break;
                        case 1: sql = "SELECT ad AS 'İçerik Adı', ortalama_puan AS 'Puan' FROM Program ORDER BY ortalama_puan DESC LIMIT 10"; break;
                        // case 2 içindeki COUNT(kp.id) hatası COUNT(kp.program_id) olarak düzeltildi
                        case 2: sql = "SELECT t.tur_adi AS 'Tür', COUNT(kp.program_id) AS 'İzlenme Sayısı' FROM Tur t JOIN ProgramTur pt ON t.id=pt.tur_id JOIN KullaniciProgram kp ON pt.program_id=kp.program_id GROUP BY t.tur_adi ORDER BY `İzlenme Sayısı` DESC"; break;
                        case 3: sql = "SELECT u.ad AS 'Kullanıcı Adı', u.email AS 'E-mail', SUM(kp.izleme_suresi) AS 'Toplam Süre(Dk)' FROM Kullanici u JOIN KullaniciProgram kp ON u.id=kp.kullanici_id GROUP BY u.id ORDER BY `Toplam Süre(Dk)` DESC LIMIT 10"; break;
                        case 4: sql = "SELECT p.ad AS 'İçerik Adı', kp.izleme_tarihi AS 'İzlenme Tarihi', u.email AS 'İzleyen Kullanıcı' FROM KullaniciProgram kp JOIN Program p ON kp.program_id=p.id JOIN Kullanici u ON kp.kullanici_id=u.id WHERE kp.izleme_tarihi >= DATE_SUB(NOW(), INTERVAL 7 DAY)"; break;
                        case 5: sql = "SELECT COUNT(*) AS 'Toplam Kullanıcı Sayısı' FROM Kullanici WHERE rol_id=1"; break;
                        case 6: sql = "SELECT SUM(izlenme_sayisi) AS 'Toplam İçerik İzlenme' FROM Program"; break;
                        case 7: sql = "SELECT COUNT(*) AS 'Toplam Verilen Puan Sayısı' FROM KullaniciProgram WHERE verilen_puan > 0"; break;
                    }
                    new MySqlDataAdapter(sql, bag).Fill(dt);
                }
                catch (Exception ex) { MessageBox.Show("Rapor Çekme Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            return dt;
        }

        public DataTable KullanicilariGetir()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection bag = new MySqlConnection(baglantiMetni))
            {
                try { bag.Open(); new MySqlDataAdapter("SELECT id AS 'ID', ad AS 'Ad', soyad AS 'Soyad', email AS 'E-mail', IF(pasif_mi=1,'Evet','Hayır') AS 'Pasif Mi' FROM Kullanici WHERE rol_id=1", bag).Fill(dt); }
                catch (Exception ex) { MessageBox.Show("Kullanıcı Listeleme Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            return dt;
        }

        public void KullaniciPasifYap(int id, bool pasifMi)
        {
            using (MySqlConnection bag = new MySqlConnection(baglantiMetni))
            {
                try { bag.Open(); new MySqlCommand($"UPDATE Kullanici SET pasif_mi={(pasifMi ? 1 : 0)} WHERE id={id}", bag).ExecuteNonQuery(); }
                catch (Exception ex) { MessageBox.Show("Kullanıcı Durum Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        public void TurIslemi(string islem, int id, string ad)
        {
            using (MySqlConnection bag = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    bag.Open();
                    if (islem == "Ekle") new MySqlCommand($"INSERT INTO Tur (tur_adi) VALUES ('{ad}')", bag).ExecuteNonQuery();
                    else if (islem == "Guncelle") new MySqlCommand($"UPDATE Tur SET tur_adi='{ad}' WHERE id={id}", bag).ExecuteNonQuery();
                    else if (islem == "Sil")
                    {
                        if (Convert.ToInt32(new MySqlCommand($"SELECT COUNT(*) FROM ProgramTur WHERE tur_id={id}", bag).ExecuteScalar()) > 0) MessageBox.Show("Bağlı içerikler var, silinemez!");
                        else new MySqlCommand($"DELETE FROM Tur WHERE id={id}", bag).ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Tür İşlem Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        public void ProgramIslemi(string islem, int id, string ad, string tip, int bolum, int sure, int yil, string aciklama, List<int> turIds)
        {
            using (MySqlConnection bag = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    bag.Open();
                    if (islem == "Ekle")
                    {
                        MySqlCommand cmd = new MySqlCommand($"INSERT INTO Program (ad, tip, bolum_sayisi, uzunluk, yayin_yili, aciklama) VALUES ('{ad}','{tip}',{bolum},{sure},{yil},'{aciklama}'); SELECT LAST_INSERT_ID();", bag);
                        int yeniId = Convert.ToInt32(cmd.ExecuteScalar());
                        foreach (int tId in turIds) new MySqlCommand($"INSERT INTO ProgramTur (program_id, tur_id) VALUES ({yeniId}, {tId})", bag).ExecuteNonQuery();
                    }
                    else if (islem == "Sil")
                    {
                        new MySqlCommand($"DELETE FROM ProgramTur WHERE program_id={id}", bag).ExecuteNonQuery();
                        new MySqlCommand($"DELETE FROM KullaniciProgram WHERE program_id={id}", bag).ExecuteNonQuery();
                        new MySqlCommand($"DELETE FROM Program WHERE id={id}", bag).ExecuteNonQuery();
                    }
                    else if (islem == "Guncelle")
                    {
                        new MySqlCommand($"UPDATE Program SET ad='{ad}', tip='{tip}', bolum_sayisi={bolum}, uzunluk={sure}, yayin_yili={yil}, aciklama='{aciklama}' WHERE id={id}", bag).ExecuteNonQuery();
                        new MySqlCommand($"DELETE FROM ProgramTur WHERE program_id={id}", bag).ExecuteNonQuery();
                        foreach (int tId in turIds) new MySqlCommand($"INSERT INTO ProgramTur (program_id, tur_id) VALUES ({id}, {tId})", bag).ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { MessageBox.Show("İçerik İşlem Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}