using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetflixCloneProject.DataAccess
{
    public class ProgramDAL
    {
        private string baglantiMetni = "Server=localhost;Database=netflix_db;Uid=root;Pwd=1234;";

        // Türleri çoğalmayı önleyerek (GROUP BY ile) tekil şekilde getirir
        public DataTable TurleriGetir()
        {
            DataTable tablo = new DataTable();
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try { baglanti.Open(); new MySqlDataAdapter("SELECT MIN(id) AS id, tur_adi FROM Tur GROUP BY tur_adi ORDER BY tur_adi ASC", baglanti).Fill(tablo); }
                catch (Exception ex) { MessageBox.Show("Tür Getirme Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            return tablo;
        }

        // Yıl Sütunu Geri Eklendi (Yönetici Paneli Çökmesin Diye)
        public DataTable IcerikleriGetir(string ara = "", string tur = "Tüm Türler", string tip = "Tüm Tipler", string siralama = "")
        {
            DataTable dt = new DataTable();
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    string sorgu = @"SELECT p.id AS 'ID', p.ad AS 'İçerik Adı', p.tip AS 'Tip', IFNULL(GROUP_CONCAT(DISTINCT t.tur_adi SEPARATOR ', '), 'Belirtilmemiş') AS 'Türleri', p.bolum_sayisi AS 'Bölüm', IFNULL(p.uzunluk, 0) AS 'Süre', p.yayin_yili AS 'Yıl', p.ortalama_puan AS 'Ort. Puan', p.izlenme_sayisi AS 'İzlenme' FROM Program p LEFT JOIN ProgramTur pt ON p.id = pt.program_id LEFT JOIN Tur t ON pt.tur_id = t.id WHERE 1=1 ";
                    if (!string.IsNullOrEmpty(ara)) sorgu += " AND p.ad LIKE @ara ";
                    if (tip != "Tüm Tipler") sorgu += " AND p.tip = @tip ";
                    sorgu += " GROUP BY p.id ";
                    if (tur != "Tüm Türler") sorgu += " HAVING Türleri LIKE @tur ";

                    if (siralama == "Puan") sorgu += " ORDER BY p.ortalama_puan DESC ";
                    else if (siralama == "İzlenme") sorgu += " ORDER BY p.izlenme_sayisi DESC ";

                    MySqlCommand cmd = new MySqlCommand(sorgu, baglanti);
                    cmd.Parameters.AddWithValue("@ara", "%" + ara + "%");
                    cmd.Parameters.AddWithValue("@tip", tip);
                    cmd.Parameters.AddWithValue("@tur", "%" + tur + "%");
                    new MySqlDataAdapter(cmd).Fill(dt);
                }
                catch (Exception ex) { MessageBox.Show("İçerik Listeleme Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            return dt;
        }

        public DataRow IcerikDetayGetir(int programId)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    string sql = @"SELECT p.*, IFNULL(GROUP_CONCAT(DISTINCT t.tur_adi SEPARATOR ', '), 'Belirtilmemiş') AS Turler FROM Program p LEFT JOIN ProgramTur pt ON p.id = pt.program_id LEFT JOIN Tur t ON pt.tur_id = t.id WHERE p.id = @id GROUP BY p.id";
                    MySqlCommand cmd = new MySqlCommand(sql, baglanti); cmd.Parameters.AddWithValue("@id", programId);
                    new MySqlDataAdapter(cmd).Fill(dt);
                }
                catch (Exception ex) { MessageBox.Show("Detay Çekme Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public bool FavoriMi(int userId, int programId)
        {
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM KullaniciFavori WHERE kullanici_id=@u AND program_id=@p", baglanti);
                    cmd.Parameters.AddWithValue("@u", userId); cmd.Parameters.AddWithValue("@p", programId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
                catch { return false; }
            }
        }

        public DataTable FavorileriGetir(int userId, string turFiltre = "Tüm Türler")
        {
            DataTable dt = new DataTable();
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    string sql = @"SELECT p.id AS 'ID', p.ad AS 'İçerik Adı', p.tip AS 'Tip', IFNULL(GROUP_CONCAT(DISTINCT t.tur_adi SEPARATOR ', '), 'Belirtilmemiş') AS 'Türleri' 
                                   FROM Program p INNER JOIN KullaniciFavori f ON p.id = f.program_id LEFT JOIN ProgramTur pt ON p.id = pt.program_id LEFT JOIN Tur t ON pt.tur_id = t.id 
                                   WHERE f.kullanici_id = @uid GROUP BY p.id ";
                    if (turFiltre != "Tüm Türler") sql += " HAVING Türleri LIKE @tur ";

                    MySqlCommand cmd = new MySqlCommand(sql, baglanti);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@tur", "%" + turFiltre + "%");
                    new MySqlDataAdapter(cmd).Fill(dt);
                }
                catch { }
            }
            return dt;
        }

        public void FavoriIslemi(int userId, int programId, bool ekle)
        {
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    string sql = ekle ? "INSERT IGNORE INTO KullaniciFavori (kullanici_id, program_id) VALUES (@u, @p)" : "DELETE FROM KullaniciFavori WHERE kullanici_id=@u AND program_id=@p";
                    MySqlCommand cmd = new MySqlCommand(sql, baglanti); cmd.Parameters.AddWithValue("@u", userId); cmd.Parameters.AddWithValue("@p", programId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { MessageBox.Show("Favori İşlem Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        public DataTable GecmisGetir(int userId)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    string sql = @"SELECT p.ad AS 'İçerik Adı', k.izleme_tarihi AS 'İzleme Tarihi', k.izlenen_bolum AS 'İzlenen Bölüm', k.izleme_suresi AS 'İzleme Süresi(Dk)', k.verilen_puan AS 'Verilen Puan', IF(k.tamamlandi_mi=1, 'Evet', 'Hayır') AS 'Tamamlandı Mı' FROM KullaniciProgram k INNER JOIN Program p ON k.program_id = p.id WHERE k.kullanici_id = @uid ORDER BY k.izleme_tarihi DESC";
                    MySqlCommand cmd = new MySqlCommand(sql, baglanti); cmd.Parameters.AddWithValue("@uid", userId);
                    new MySqlDataAdapter(cmd).Fill(dt);
                }
                catch (Exception ex) { MessageBox.Show("Geçmiş Listeleme Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            return dt;
        }

        public DataRow KalinanYeriGetir(int userId, int programId)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT izlenen_bolum, izleme_suresi, tamamlandi_mi, verilen_puan FROM KullaniciProgram WHERE kullanici_id=@u AND program_id=@p ORDER BY izleme_tarihi DESC LIMIT 1", baglanti);
                    cmd.Parameters.AddWithValue("@u", userId); cmd.Parameters.AddWithValue("@p", programId);
                    new MySqlDataAdapter(cmd).Fill(dt);
                }
                catch (Exception ex) { MessageBox.Show("Kayıt Yeri Okuma Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public void IzlemeKaydet(int userId, int progId, int bolum, int sure, int puan, bool tamam)
        {
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    string checkSql = "SELECT COUNT(*) FROM KullaniciProgram WHERE kullanici_id=@u AND program_id=@p";
                    MySqlCommand checkCmd = new MySqlCommand(checkSql, baglanti);
                    checkCmd.Parameters.AddWithValue("@u", userId);
                    checkCmd.Parameters.AddWithValue("@p", progId);
                    int kayitSayisi = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (kayitSayisi > 0)
                    {
                        string updateSql = "UPDATE KullaniciProgram SET izlenen_bolum=@b, izleme_suresi=@s, verilen_puan=IF(@pu>0, @pu, verilen_puan), tamamlandi_mi=@t, izleme_tarihi=NOW() WHERE kullanici_id=@u AND program_id=@p";
                        MySqlCommand cmd = new MySqlCommand(updateSql, baglanti);
                        cmd.Parameters.AddWithValue("@b", bolum); cmd.Parameters.AddWithValue("@s", sure); cmd.Parameters.AddWithValue("@pu", puan); cmd.Parameters.AddWithValue("@t", tamam ? 1 : 0); cmd.Parameters.AddWithValue("@u", userId); cmd.Parameters.AddWithValue("@p", progId);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string sql = "INSERT INTO KullaniciProgram (kullanici_id, program_id, izleme_tarihi, izlenen_bolum, izleme_suresi, verilen_puan, tamamlandi_mi) VALUES (@u,@p,NOW(),@b,@s,@pu,@t)";
                        MySqlCommand cmd = new MySqlCommand(sql, baglanti);
                        cmd.Parameters.AddWithValue("@u", userId); cmd.Parameters.AddWithValue("@p", progId); cmd.Parameters.AddWithValue("@b", bolum); cmd.Parameters.AddWithValue("@s", sure); cmd.Parameters.AddWithValue("@pu", puan); cmd.Parameters.AddWithValue("@t", tamam ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }

                    if (tamam)
                    {
                        MySqlCommand upProg = new MySqlCommand("UPDATE Program SET izlenme_sayisi = izlenme_sayisi + 1 WHERE id=@p", baglanti);
                        upProg.Parameters.AddWithValue("@p", progId);
                        upProg.ExecuteNonQuery();
                    }

                    if (puan > 0)
                    {
                        string avgSql = "UPDATE Program SET ortalama_puan = IFNULL((SELECT AVG(verilen_puan) FROM KullaniciProgram WHERE program_id=@p AND verilen_puan > 0), 0) WHERE id=@p";
                        MySqlCommand avgCmd = new MySqlCommand(avgSql, baglanti);
                        avgCmd.Parameters.AddWithValue("@p", progId);
                        avgCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { MessageBox.Show("İzleme Kaydetme Hatası: " + ex.Message, "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        public void VerileriDuzenleVeDagit()
        {
            using (MySqlConnection baglanti = new MySqlConnection(baglantiMetni))
            {
                try
                {
                    baglanti.Open();
                    MySqlCommand temizle = new MySqlCommand("SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE ProgramTur; TRUNCATE TABLE KullaniciProgram; TRUNCATE TABLE KullaniciFavori; TRUNCATE TABLE Program; SET FOREIGN_KEY_CHECKS = 1;", baglanti);
                    temizle.ExecuteNonQuery();

                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM ExcelHamVeri", baglanti);
                    var hamListe = new List<dynamic>();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            hamListe.Add(new
                            {
                                Ad = r[0].ToString(),
                                TurMetni = r[1].ToString(),
                                Tip = r.FieldCount > 2 ? r[2].ToString() : "",
                                Ek = r.FieldCount > 3 ? r[3].ToString() : ""
                            });
                        }
                    }

                    Random rnd = new Random();

                    foreach (var item in hamListe)
                    {
                        if (string.IsNullOrEmpty(item.Ad)) continue;

                        string hamTip = item.Tip.ToString().Trim().ToLower();
                        string hamEk = item.Ek.ToString().Trim().ToLower();

                        string fTip = "Film";
                        if (hamTip.Contains("dizi") || hamTip.Contains("reality") || hamEk.Contains("show") || string.IsNullOrWhiteSpace(hamTip))
                        {
                            fTip = "Dizi";
                        }

                        int bSayisi = fTip == "Film" ? 1 : rnd.Next(8, 55);
                        int sureDk = fTip == "Film" ? rnd.Next(90, 160) : rnd.Next(20, 60);

                        MySqlCommand ins = new MySqlCommand("INSERT INTO Program (ad, tip, bolum_sayisi, uzunluk, yayin_yili, aciklama) VALUES (@a,@t,@b,@u,@y,'Bu içerik hakkında henüz detaylı bir açıklama girilmemiştir.'); SELECT LAST_INSERT_ID();", baglanti);
                        ins.Parameters.AddWithValue("@a", item.Ad);
                        ins.Parameters.AddWithValue("@t", fTip);
                        ins.Parameters.AddWithValue("@b", bSayisi);
                        ins.Parameters.AddWithValue("@u", sureDk);
                        ins.Parameters.AddWithValue("@y", rnd.Next(2005, 2025));
                        int pId = Convert.ToInt32(ins.ExecuteScalar());

                        string[] ayiricilar = { ",", "ve", "Yapımlar", "Doğa" };
                        string[] parcalar = item.TurMetni.Split(ayiricilar, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string p in parcalar)
                        {
                            string t = p.Trim(); if (t.Length < 3) continue;
                            MySqlCommand tCmd = new MySqlCommand("SELECT id FROM Tur WHERE tur_adi LIKE @tadi LIMIT 1", baglanti);
                            tCmd.Parameters.AddWithValue("@tadi", "%" + t + "%");
                            object res = tCmd.ExecuteScalar();
                            if (res != null)
                            {
                                MySqlCommand b = new MySqlCommand("INSERT IGNORE INTO ProgramTur (program_id, tur_id) VALUES (@pid,@tid)", baglanti);
                                b.Parameters.AddWithValue("@pid", pId); b.Parameters.AddWithValue("@tid", res);
                                b.ExecuteNonQuery();
                            }
                        }
                    }
                    MessageBox.Show("Mükemmel! Excel verileri tüm eksikleri (Süre, Bölüm, Tip) giderilerek veritabanına işlendi.", "Sistem Güncellemesi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("Dağıtım Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}