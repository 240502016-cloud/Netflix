using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using NetflixCloneProject.DataAccess;

namespace NetflixCloneProject
{
    public partial class AnaSayfaForm : Form
    {
        int aktifKullaniciID; int aktifProgramID = 0; int programToplamBolum = 1; bool suAnFavoriMi = false;
        ProgramDAL programDal = new ProgramDAL(); KullaniciDAL kullaniciDal = new KullaniciDAL();

        Panel pnlSidebar = new Panel(), pnlIcerik = new Panel(), pnlAnaSayfa = new Panel(), pnlDetay = new Panel(), pnlIzleme = new Panel(), pnlFavoriler = new Panel(), pnlGecmis = new Panel(), pnlProfil = new Panel();
        DataGridView dgvAnaSayfa = new DataGridView(), dgvFavoriler = new DataGridView(), dgvGecmis = new DataGridView();

        TextBox txtAra = new TextBox(); ComboBox cmbTur = new ComboBox(), cmbTip = new ComboBox(); string aktifSiralama = "";

        Label lblDetayBaslik = new Label(), lblDetayBilgi = new Label(), lblKullaniciGecmisi = new Label();
        Button btnIzleEkrani = new Button(), btnFavDetay = new Button(), btnDetayKapat = new Button(); ComboBox cmbBolumler = new ComboBox();

        Label lblIzlemeIcerikAdi = new Label(), lblIzlemeBolumBilgisi = new Label();
        TextBox txtIzlemeBolum = new TextBox(), txtIzlemeSure = new TextBox(), txtPuan = new TextBox(); CheckBox chkTamamlandi = new CheckBox();

        TextBox txtPAd = new TextBox(), txtPSoyad = new TextBox(), txtPEmail = new TextBox(), txtPDogum = new TextBox(), txtPSifre = new TextBox();
        ComboBox cmbPUlke = new ComboBox(); Label lblPFavoriTurler = new Label(), lblPIstatistik = new Label();
        ComboBox cmbFavFiltre = new ComboBox();

        public AnaSayfaForm(int userId) { aktifKullaniciID = userId; InitializeComponent(); ArayuzuKur(); FiltreleriDoldur(); IcerikleriListele(); }

        private void ArayuzuKur()
        {
            this.Size = new Size(1300, 850); this.BackColor = Color.FromArgb(20, 20, 20); this.StartPosition = FormStartPosition.CenterScreen; this.Text = "Netflix";

            pnlSidebar.Size = new Size(250, 850); pnlSidebar.Dock = DockStyle.Left; pnlSidebar.BackColor = Color.FromArgb(0, 0, 0);
            pnlSidebar.Controls.Add(new Label() { Text = "NETFLIX", ForeColor = Color.FromArgb(229, 9, 20), Font = new Font("Segoe UI Black", 28, FontStyle.Bold), Location = new Point(25, 30), AutoSize = true });

            Button b1 = MenuButon("🏠 Ana Sayfa", 130); b1.Click += (s, e) => { PanelGoster(pnlAnaSayfa); IcerikleriListele(); };
            Button b2 = MenuButon("❤ Favorilerim", 190); b2.Click += (s, e) => { PanelGoster(pnlFavoriler); FavoriListele(); };
            Button b3 = MenuButon("🕒 İzleme Geçmişi", 250); b3.Click += (s, e) => { PanelGoster(pnlGecmis); GecmisListele(); };
            Button b4 = MenuButon("👤 Profilim", 310); b4.Click += (s, e) => { PanelGoster(pnlProfil); ProfilDoldur(); };
            Button bCikis = MenuButon("🚪 Çıkış Yap", 700); bCikis.ForeColor = Color.DarkGray;
            bCikis.Click += (s, e) => { new Form1().Show(); this.Hide(); };

            pnlSidebar.Controls.AddRange(new Control[] { b1, b2, b3, b4, bCikis });

            pnlIcerik.Dock = DockStyle.Fill; TumPanelleriAyarla();
            this.Controls.Add(pnlIcerik); this.Controls.Add(pnlSidebar); PanelGoster(pnlAnaSayfa);
        }

        private void TumPanelleriAyarla()
        {
            // --- ANA SAYFA ---
            pnlAnaSayfa.Dock = DockStyle.Fill;
            Panel f = new Panel() { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(20, 20, 20) };
            txtAra.PlaceholderText = "İçerik Ara..."; txtAra.Location = new Point(30, 30); txtAra.Size = new Size(220, 30); txtAra.Font = new Font("Segoe UI", 12); txtAra.BackColor = Color.FromArgb(51, 51, 51); txtAra.ForeColor = Color.White; txtAra.BorderStyle = BorderStyle.None;
            cmbTur.Location = new Point(270, 30); cmbTur.Size = new Size(150, 30); cmbTur.DropDownStyle = ComboBoxStyle.DropDownList; cmbTur.Font = new Font("Segoe UI", 12); cmbTur.BackColor = Color.FromArgb(51, 51, 51); cmbTur.ForeColor = Color.White; cmbTur.Cursor = Cursors.Hand;
            cmbTip.Location = new Point(440, 30); cmbTip.Size = new Size(130, 30); cmbTip.DropDownStyle = ComboBoxStyle.DropDownList; cmbTip.Font = new Font("Segoe UI", 12); cmbTip.BackColor = Color.FromArgb(51, 51, 51); cmbTip.ForeColor = Color.White; cmbTip.Cursor = Cursors.Hand;

            Button btnF = new Button() { Text = "Filtrele", Location = new Point(590, 28), BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, Size = new Size(100, 35), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand }; btnF.FlatAppearance.BorderSize = 0; btnF.Click += (s, e) => IcerikleriListele();
            ButonEfektiEkle(btnF, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50));

            Button btnY = new Button() { Text = "★ Puana Göre", Location = new Point(710, 28), BackColor = Color.DarkOrange, ForeColor = Color.White, Size = new Size(120, 35), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand }; btnY.FlatAppearance.BorderSize = 0; btnY.Click += (s, e) => { aktifSiralama = "Puan"; IcerikleriListele(); };
            ButonEfektiEkle(btnY, Color.DarkOrange, Color.Orange);

            Button btnP = new Button() { Text = "🔥 Popüler", Location = new Point(850, 28), BackColor = Color.DodgerBlue, ForeColor = Color.White, Size = new Size(110, 35), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand }; btnP.FlatAppearance.BorderSize = 0; btnP.Click += (s, e) => { aktifSiralama = "İzlenme"; IcerikleriListele(); };
            ButonEfektiEkle(btnP, Color.DodgerBlue, Color.DeepSkyBlue);

            f.Controls.AddRange(new Control[] { txtAra, cmbTur, cmbTip, btnF, btnY, btnP });

            dgvAnaSayfa.Dock = DockStyle.Fill; GridAyarla(dgvAnaSayfa); dgvAnaSayfa.Cursor = Cursors.Hand;
            dgvAnaSayfa.CellContentClick += (s, e) => {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvAnaSayfa.Columns[e.ColumnIndex].Name == "btnDetayCol")
                {
                    aktifProgramID = Convert.ToInt32(dgvAnaSayfa.Rows[e.RowIndex].Cells["ID"].Value); DetaySayfasiniAc(aktifProgramID);
                }
            };
            pnlAnaSayfa.Controls.Add(dgvAnaSayfa); pnlAnaSayfa.Controls.Add(f);

            // --- DETAY PANELİ ---
            pnlDetay.Dock = DockStyle.Fill; pnlDetay.BackColor = Color.FromArgb(20, 20, 20);

            lblDetayBaslik.Font = new Font("Segoe UI Black", 42, FontStyle.Bold); lblDetayBaslik.ForeColor = Color.White; lblDetayBaslik.Location = new Point(50, 40); lblDetayBaslik.AutoSize = true; lblDetayBaslik.MaximumSize = new Size(1000, 0);
            lblDetayBilgi.Font = new Font("Segoe UI", 14); lblDetayBilgi.ForeColor = Color.LightGray; lblDetayBilgi.Location = new Point(55, 130); lblDetayBilgi.AutoSize = true;
            lblKullaniciGecmisi.Font = new Font("Segoe UI", 12, FontStyle.Italic); lblKullaniciGecmisi.ForeColor = Color.DarkOrange; lblKullaniciGecmisi.Location = new Point(55, 290); lblKullaniciGecmisi.AutoSize = true;

            cmbBolumler.Location = new Point(55, 340); cmbBolumler.Size = new Size(150, 30); cmbBolumler.Font = new Font("Segoe UI", 14); cmbBolumler.DropDownStyle = ComboBoxStyle.DropDownList; cmbBolumler.BackColor = Color.FromArgb(51, 51, 51); cmbBolumler.ForeColor = Color.White; cmbBolumler.Cursor = Cursors.Hand;

            btnIzleEkrani.Font = new Font("Segoe UI", 14, FontStyle.Bold); btnIzleEkrani.Location = new Point(220, 335); btnIzleEkrani.Size = new Size(240, 40); btnIzleEkrani.BackColor = Color.White; btnIzleEkrani.ForeColor = Color.Black; btnIzleEkrani.FlatStyle = FlatStyle.Flat; btnIzleEkrani.FlatAppearance.BorderSize = 0; btnIzleEkrani.Cursor = Cursors.Hand; btnIzleEkrani.Click += (s, e) => IzlemeEkraniniAc();
            ButonEfektiEkle(btnIzleEkrani, Color.White, Color.LightGray);

            btnFavDetay.Font = new Font("Segoe UI", 12, FontStyle.Bold); btnFavDetay.Location = new Point(480, 335); btnFavDetay.Size = new Size(200, 40); btnFavDetay.FlatStyle = FlatStyle.Flat; btnFavDetay.FlatAppearance.BorderSize = 0; btnFavDetay.Cursor = Cursors.Hand;
            btnFavDetay.Click += (s, e) => { programDal.FavoriIslemi(aktifKullaniciID, aktifProgramID, !suAnFavoriMi); DetaySayfasiniAc(aktifProgramID); };

            btnDetayKapat.Text = "← Geri Dön"; btnDetayKapat.Font = new Font("Segoe UI", 12, FontStyle.Bold); btnDetayKapat.Location = new Point(700, 335); btnDetayKapat.Size = new Size(120, 40); btnDetayKapat.BackColor = Color.FromArgb(51, 51, 51); btnDetayKapat.ForeColor = Color.White; btnDetayKapat.FlatStyle = FlatStyle.Flat; btnDetayKapat.FlatAppearance.BorderSize = 0; btnDetayKapat.Cursor = Cursors.Hand; btnDetayKapat.Click += (s, e) => PanelGoster(pnlAnaSayfa);
            ButonEfektiEkle(btnDetayKapat, Color.FromArgb(51, 51, 51), Color.Gray);

            pnlDetay.Controls.AddRange(new Control[] { lblDetayBaslik, lblDetayBilgi, lblKullaniciGecmisi, cmbBolumler, btnIzleEkrani, btnFavDetay, btnDetayKapat });

            // --- İZLEME EKRANI ---
            pnlIzleme.Dock = DockStyle.Fill; pnlIzleme.BackColor = Color.FromArgb(10, 10, 10);
            lblIzlemeIcerikAdi.Font = new Font("Segoe UI Black", 28, FontStyle.Bold); lblIzlemeIcerikAdi.ForeColor = Color.White; lblIzlemeIcerikAdi.Location = new Point(50, 20); lblIzlemeIcerikAdi.AutoSize = true;
            lblIzlemeBolumBilgisi.Font = new Font("Segoe UI", 12); lblIzlemeBolumBilgisi.ForeColor = Color.LightGray; lblIzlemeBolumBilgisi.Location = new Point(55, 75); lblIzlemeBolumBilgisi.AutoSize = true;

            Panel pnlPlayer = new Panel() { Size = new Size(920, 420), Location = new Point(50, 110), BackColor = Color.Black, BorderStyle = BorderStyle.None };
            pnlPlayer.Controls.Add(new Label() { Text = "▶", ForeColor = Color.FromArgb(50, 50, 50), Font = new Font("Segoe UI", 60, FontStyle.Bold), AutoSize = true, Location = new Point(410, 150) });

            Panel pnlI = new Panel() { Size = new Size(920, 220), Location = new Point(50, 550), BackColor = Color.FromArgb(20, 20, 20) };
            pnlI.Controls.Add(new Label() { Text = "İzlenen Bölüm:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(30, 20), AutoSize = true }); txtIzlemeBolum.Location = new Point(30, 50); txtIzlemeBolum.Size = new Size(180, 29); txtIzlemeBolum.Font = new Font("Segoe UI", 14); txtIzlemeBolum.BackColor = Color.FromArgb(51, 51, 51); txtIzlemeBolum.ForeColor = Color.White; txtIzlemeBolum.BorderStyle = BorderStyle.None;
            pnlI.Controls.Add(new Label() { Text = "Süre (Dakika):", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(240, 20), AutoSize = true }); txtIzlemeSure.Location = new Point(240, 50); txtIzlemeSure.Size = new Size(180, 29); txtIzlemeSure.Font = new Font("Segoe UI", 14); txtIzlemeSure.BackColor = Color.FromArgb(51, 51, 51); txtIzlemeSure.ForeColor = Color.White; txtIzlemeSure.BorderStyle = BorderStyle.None;
            pnlI.Controls.Add(new Label() { Text = "Puan (1-10):", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(450, 20), AutoSize = true }); txtPuan.Location = new Point(450, 50); txtPuan.Size = new Size(180, 29); txtPuan.Font = new Font("Segoe UI", 14); txtPuan.BackColor = Color.FromArgb(51, 51, 51); txtPuan.ForeColor = Color.White; txtPuan.BorderStyle = BorderStyle.None;
            chkTamamlandi.Text = "Tamamlandı"; chkTamamlandi.ForeColor = Color.White; chkTamamlandi.Font = new Font("Segoe UI", 14); chkTamamlandi.Location = new Point(700, 48); chkTamamlandi.AutoSize = true; chkTamamlandi.Cursor = Cursors.Hand;

            Button btnKaydetCik = new Button() { Text = "⏸ Kaldığım Yere Kaydet", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(30, 110), Size = new Size(420, 60), BackColor = Color.FromArgb(51, 51, 51), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; btnKaydetCik.FlatAppearance.BorderSize = 0; btnKaydetCik.Click += (s, e) => IzlemeIslemi(false);
            ButonEfektiEkle(btnKaydetCik, Color.FromArgb(51, 51, 51), Color.Gray);

            Button btnTamamla = new Button() { Text = "✔ İzlemeyi Tamamla", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(470, 110), Size = new Size(420, 60), BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; btnTamamla.FlatAppearance.BorderSize = 0; btnTamamla.Click += (s, e) => IzlemeIslemi(true);
            ButonEfektiEkle(btnTamamla, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50));

            pnlI.Controls.AddRange(new Control[] { txtIzlemeBolum, txtIzlemeSure, txtPuan, chkTamamlandi, btnKaydetCik, btnTamamla });
            pnlIzleme.Controls.AddRange(new Control[] { lblIzlemeIcerikAdi, lblIzlemeBolumBilgisi, pnlPlayer, pnlI });

            // --- FAVORİ VE GEÇMİŞ ---
            pnlFavoriler.Dock = pnlGecmis.Dock = DockStyle.Fill;
            Panel pnlFavTop = new Panel() { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(20, 20, 20) };
            pnlFavTop.Controls.Add(new Label() { Text = "Favorilerim", Font = new Font("Segoe UI Black", 24, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true });
            cmbFavFiltre.Location = new Point(300, 30); cmbFavFiltre.Size = new Size(200, 30); cmbFavFiltre.Font = new Font("Segoe UI", 12); cmbFavFiltre.DropDownStyle = ComboBoxStyle.DropDownList; cmbFavFiltre.BackColor = Color.FromArgb(51, 51, 51); cmbFavFiltre.ForeColor = Color.White; cmbFavFiltre.Cursor = Cursors.Hand;
            Button btnFavFiltrele = new Button() { Text = "Filtrele", Location = new Point(520, 28), BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, Size = new Size(120, 35), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand }; btnFavFiltrele.FlatAppearance.BorderSize = 0; btnFavFiltrele.Click += (s, e) => FavoriListele();
            ButonEfektiEkle(btnFavFiltrele, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50));
            pnlFavTop.Controls.AddRange(new Control[] { cmbFavFiltre, btnFavFiltrele });

            pnlGecmis.Controls.Add(new Label() { Text = "İzleme Geçmişim", Font = new Font("Segoe UI Black", 24, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true });

            dgvFavoriler.Dock = DockStyle.Fill; dgvGecmis.Location = new Point(20, 80); dgvGecmis.Size = new Size(1000, 650);
            GridAyarla(dgvFavoriler); GridAyarla(dgvGecmis); dgvFavoriler.Cursor = Cursors.Hand;

            dgvFavoriler.CellContentClick += (s, e) => {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvFavoriler.Columns[e.ColumnIndex].Name == "btnFavCikarCol")
                {
                    int pId = Convert.ToInt32(dgvFavoriler.Rows[e.RowIndex].Cells["ID"].Value);
                    programDal.FavoriIslemi(aktifKullaniciID, pId, false); FavoriListele();
                }
            };

            pnlFavoriler.Controls.Add(dgvFavoriler); pnlFavoriler.Controls.Add(pnlFavTop); pnlGecmis.Controls.Add(dgvGecmis);

            // --- PROFİL ---
            pnlProfil.Dock = DockStyle.Fill;
            pnlProfil.Controls.Add(new Label() { Text = "Profil Ayarları", Font = new Font("Segoe UI Black", 24, FontStyle.Bold), ForeColor = Color.White, Location = new Point(50, 40), AutoSize = true });

            pnlProfil.Controls.Add(new Label() { Text = "Ad:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(50, 110) }); txtPAd.Location = new Point(50, 135); txtPAd.Size = new Size(180, 30); txtPAd.Font = new Font("Segoe UI", 12); txtPAd.BackColor = Color.FromArgb(51, 51, 51); txtPAd.ForeColor = Color.White; txtPAd.BorderStyle = BorderStyle.None;
            pnlProfil.Controls.Add(new Label() { Text = "Soyad:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(250, 110) }); txtPSoyad.Location = new Point(250, 135); txtPSoyad.Size = new Size(180, 30); txtPSoyad.Font = new Font("Segoe UI", 12); txtPSoyad.BackColor = Color.FromArgb(51, 51, 51); txtPSoyad.ForeColor = Color.White; txtPSoyad.BorderStyle = BorderStyle.None;
            pnlProfil.Controls.Add(new Label() { Text = "E-mail (Salt Okunur):", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(50, 180) }); txtPEmail.Location = new Point(50, 205); txtPEmail.Size = new Size(380, 30); txtPEmail.Font = new Font("Segoe UI", 12); txtPEmail.BackColor = Color.FromArgb(30, 30, 30); txtPEmail.ForeColor = Color.Gray; txtPEmail.BorderStyle = BorderStyle.None; txtPEmail.ReadOnly = true;
            pnlProfil.Controls.Add(new Label() { Text = "Doğum Tarihi:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(50, 250) }); txtPDogum.Location = new Point(50, 275); txtPDogum.Size = new Size(180, 30); txtPDogum.Font = new Font("Segoe UI", 12); txtPDogum.BackColor = Color.FromArgb(30, 30, 30); txtPDogum.ForeColor = Color.Gray; txtPDogum.BorderStyle = BorderStyle.None; txtPDogum.ReadOnly = true;
            pnlProfil.Controls.Add(new Label() { Text = "Ülke:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(250, 250) }); cmbPUlke.Location = new Point(250, 275); cmbPUlke.Size = new Size(180, 30); cmbPUlke.Font = new Font("Segoe UI", 12); cmbPUlke.BackColor = Color.FromArgb(51, 51, 51); cmbPUlke.ForeColor = Color.White; cmbPUlke.Items.AddRange(new string[] { "Türkiye", "ABD", "Almanya", "İngiltere", "Fransa" }); cmbPUlke.Cursor = Cursors.Hand;
            pnlProfil.Controls.Add(new Label() { Text = "Yeni Şifre (Boş Bırakılabilir):", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 12), Location = new Point(50, 320) }); txtPSifre.Location = new Point(50, 345); txtPSifre.Size = new Size(380, 30); txtPSifre.Font = new Font("Segoe UI", 12); txtPSifre.PasswordChar = '*'; txtPSifre.BackColor = Color.FromArgb(51, 51, 51); txtPSifre.ForeColor = Color.White; txtPSifre.BorderStyle = BorderStyle.None;

            lblPFavoriTurler.Font = new Font("Segoe UI", 11, FontStyle.Italic); lblPFavoriTurler.ForeColor = Color.DarkOrange; lblPFavoriTurler.Location = new Point(50, 400); lblPFavoriTurler.AutoSize = true; pnlProfil.Controls.Add(lblPFavoriTurler);

            Button btnPGuncelle = new Button() { Text = "Bilgileri Güncelle", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(50, 450), Size = new Size(380, 45), BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand }; btnPGuncelle.FlatAppearance.BorderSize = 0;
            ButonEfektiEkle(btnPGuncelle, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50));
            btnPGuncelle.Click += (s, e) => { kullaniciDal.ProfilGuncelle(aktifKullaniciID, txtPAd.Text, txtPSoyad.Text, cmbPUlke.Text, txtPSifre.Text); MessageBox.Show("Güncellendi!", "Başarılı"); txtPSifre.Clear(); ProfilDoldur(); };

            Panel pnlIstatistik = new Panel() { Location = new Point(500, 110), Size = new Size(450, 200), BackColor = Color.FromArgb(30, 30, 30) };
            pnlIstatistik.Controls.Add(new Label() { Text = "📊 İzleme İstatistiklerim", ForeColor = Color.White, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true });
            lblPIstatistik.ForeColor = Color.LightGray; lblPIstatistik.Font = new Font("Segoe UI", 13); lblPIstatistik.Location = new Point(20, 70); lblPIstatistik.Size = new Size(400, 120); pnlIstatistik.Controls.Add(lblPIstatistik);
            pnlProfil.Controls.AddRange(new Control[] { txtPAd, txtPSoyad, txtPEmail, txtPDogum, cmbPUlke, txtPSifre, btnPGuncelle, pnlIstatistik });

            pnlIcerik.Controls.AddRange(new Control[] { pnlAnaSayfa, pnlDetay, pnlIzleme, pnlFavoriler, pnlGecmis, pnlProfil });
        }

        private void DetaySayfasiniAc(int progId)
        {
            DataRow dr = programDal.IcerikDetayGetir(progId); if (dr == null) return;
            string tip = dr["tip"].ToString(); programToplamBolum = Convert.ToInt32(dr["bolum_sayisi"]);

            lblDetayBaslik.Text = dr["ad"].ToString(); lblIzlemeIcerikAdi.Text = dr["ad"].ToString();
            lblDetayBilgi.Text = $"🎬 Tip: {tip}   |   📅 Yayın Yılı: {dr["yayin_yili"]}   |   ⏱ Uzunluk: {dr["uzunluk"]} Dk\n\n🏷 Türler: {dr["Turler"]}\n\n⭐ Ortalama Puan: {dr["ortalama_puan"]} / 10   |   👁 Toplam İzlenme: {dr["izlenme_sayisi"]}";

            lblDetayBilgi.Top = lblDetayBaslik.Bottom + 30;
            lblKullaniciGecmisi.Top = lblDetayBilgi.Bottom + 30;
            cmbBolumler.Top = lblKullaniciGecmisi.Bottom + 20;
            btnIzleEkrani.Top = lblKullaniciGecmisi.Bottom + 15;
            btnFavDetay.Top = lblKullaniciGecmisi.Bottom + 15;
            btnDetayKapat.Top = lblKullaniciGecmisi.Bottom + 15;

            DataRow kalinanYer = programDal.KalinanYeriGetir(aktifKullaniciID, progId);

            if (kalinanYer != null)
            {
                int userPuan = Convert.ToInt32(kalinanYer["verilen_puan"]);
                string durum = Convert.ToInt32(kalinanYer["tamamlandi_mi"]) == 1 ? "Bölüm Tamamlandı." : $"{kalinanYer["izlenen_bolum"]}. Bölüm, {kalinanYer["izleme_suresi"]}. Dakikada Kaldınız.";
                lblKullaniciGecmisi.Text = $"Daha Önce İzlediniz ✔  |  Durum: {durum}  |  Verdiğiniz Puan: {(userPuan > 0 ? userPuan.ToString() : "Yok")}";

                txtIzlemeBolum.Text = kalinanYer["izlenen_bolum"].ToString(); txtIzlemeSure.Text = kalinanYer["izleme_suresi"].ToString(); txtPuan.Text = userPuan > 0 ? userPuan.ToString() : "";
            }
            else
            {
                lblKullaniciGecmisi.Text = "Bu içeriği henüz izlemediniz.";
                txtIzlemeBolum.Text = tip == "Film" ? "1" : ""; txtIzlemeSure.Text = ""; txtPuan.Text = "";
            }

            if (tip == "Dizi")
            {
                cmbBolumler.Visible = true; cmbBolumler.Items.Clear();
                for (int i = 1; i <= programToplamBolum; i++) cmbBolumler.Items.Add($"{i}. Bölüm");
                cmbBolumler.SelectedIndex = 0;
                if (kalinanYer != null)
                {
                    int kBolum = Convert.ToInt32(kalinanYer["izlenen_bolum"]);
                    if (kBolum <= programToplamBolum && kBolum > 0) cmbBolumler.SelectedIndex = kBolum - 1;
                    btnIzleEkrani.Text = "▶ DEVAM ET";
                }
                else btnIzleEkrani.Text = "▶ BÖLÜMÜ İZLE";
            }
            else
            {
                cmbBolumler.Visible = false; btnIzleEkrani.Text = "▶ FİLMİ İZLE";
                btnIzleEkrani.Left = 55; btnFavDetay.Left = 310; btnDetayKapat.Left = 535;
            }

            suAnFavoriMi = programDal.FavoriMi(aktifKullaniciID, progId);
            if (suAnFavoriMi) { btnFavDetay.Text = "❤ Favorilerden Çıkar"; btnFavDetay.BackColor = Color.FromArgb(51, 51, 51); btnFavDetay.ForeColor = Color.White; ButonEfektiEkle(btnFavDetay, Color.FromArgb(51, 51, 51), Color.Gray); }
            else { btnFavDetay.Text = "🤍 Favoriye Ekle"; btnFavDetay.BackColor = Color.DarkRed; btnFavDetay.ForeColor = Color.White; ButonEfektiEkle(btnFavDetay, Color.DarkRed, Color.Red); }

            PanelGoster(pnlDetay);
        }

        private void IzlemeEkraniniAc()
        {
            if (cmbBolumler.Visible && cmbBolumler.SelectedIndex >= 0) txtIzlemeBolum.Text = (cmbBolumler.SelectedIndex + 1).ToString();
            int bol = int.TryParse(txtIzlemeBolum.Text, out int b) ? b : 1; int sur = int.TryParse(txtIzlemeSure.Text, out int s) ? s : 0;
            lblIzlemeBolumBilgisi.Text = $"Seçili Bölüm: {bol} / Toplam Bölüm: {programToplamBolum}";
            programDal.IzlemeKaydet(aktifKullaniciID, aktifProgramID, bol, sur, 0, false);
            PanelGoster(pnlIzleme);
        }

        private void IzlemeIslemi(bool tamamlandi)
        {
            int bol = int.TryParse(txtIzlemeBolum.Text, out int b) ? b : 1; int sur = int.TryParse(txtIzlemeSure.Text, out int s) ? s : 0; int pua = 0;
            if (!string.IsNullOrWhiteSpace(txtPuan.Text)) { if (!int.TryParse(txtPuan.Text, out pua) || pua < 1 || pua > 10) { MessageBox.Show("Lütfen 1 ile 10 arasında geçerli bir puan giriniz!", "Hatalı Puan", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } }
            if (bol > programToplamBolum) { MessageBox.Show($"Girdiğiniz bölüm sayısı toplam bölümden ({programToplamBolum}) büyük olamaz!"); return; }
            programDal.IzlemeKaydet(aktifKullaniciID, aktifProgramID, bol, sur, pua, tamamlandi);
            MessageBox.Show(tamamlandi ? "Tebrikler, içeriği bitirdiniz! Veriler kaydedildi." : "Kaldığınız yer başarıyla güncellendi.", "Sistem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IcerikleriListele(); PanelGoster(pnlAnaSayfa);
        }

        private void FiltreleriDoldur()
        {
            cmbTur.Items.Clear(); cmbTur.Items.Add("Tüm Türler"); cmbFavFiltre.Items.Clear(); cmbFavFiltre.Items.Add("Tüm Türler");
            foreach (DataRow r in programDal.TurleriGetir().Rows) { cmbTur.Items.Add(r["tur_adi"].ToString()); cmbFavFiltre.Items.Add(r["tur_adi"].ToString()); }
            cmbTur.SelectedIndex = 0; cmbFavFiltre.SelectedIndex = 0; cmbTip.Items.Clear(); cmbTip.Items.AddRange(new[] { "Tüm Tipler", "Film", "Dizi" }); cmbTip.SelectedIndex = 0;
        }

        private void IcerikleriListele()
        {
            dgvAnaSayfa.DataSource = null; dgvAnaSayfa.Columns.Clear();
            DataTable dt = programDal.IcerikleriGetir(txtAra.Text, cmbTur.Text, cmbTip.Text, aktifSiralama); dgvAnaSayfa.DataSource = dt;
            if (dgvAnaSayfa.Columns["ID"] != null) dgvAnaSayfa.Columns["ID"].Visible = false; if (dgvAnaSayfa.Columns["Yıl"] != null) dgvAnaSayfa.Columns["Yıl"].Visible = false;
            DataGridViewButtonColumn bd = new DataGridViewButtonColumn() { Name = "btnDetayCol", HeaderText = "İşlem", Text = "İncele", UseColumnTextForButtonValue = true, FlatStyle = FlatStyle.Flat }; dgvAnaSayfa.Columns.Add(bd);
        }

        private void FavoriListele()
        {
            dgvFavoriler.DataSource = null; dgvFavoriler.Columns.Clear(); dgvFavoriler.DataSource = programDal.FavorileriGetir(aktifKullaniciID, cmbFavFiltre.Text); if (dgvFavoriler.Columns["ID"] != null) dgvFavoriler.Columns["ID"].Visible = false;
            DataGridViewButtonColumn bc = new DataGridViewButtonColumn() { Name = "btnFavCikarCol", HeaderText = "İşlem", Text = "✖ Çıkar", UseColumnTextForButtonValue = true, FlatStyle = FlatStyle.Flat }; dgvFavoriler.Columns.Add(bc);
        }

        private void GecmisListele() { dgvGecmis.DataSource = programDal.GecmisGetir(aktifKullaniciID); }
        private void ProfilDoldur()
        {
            DataRow dr = kullaniciDal.ProfilGetir(aktifKullaniciID);
            if (dr != null) { txtPAd.Text = dr["ad"].ToString(); txtPSoyad.Text = dr["soyad"].ToString(); txtPEmail.Text = dr["email"].ToString(); txtPDogum.Text = Convert.ToDateTime(dr["dogum_tarihi"]).ToShortDateString(); cmbPUlke.Text = dr["ulke"].ToString(); lblPFavoriTurler.Text = "❤ Favori Türleriniz: " + dr["FavoriTurler"].ToString(); }
            lblPIstatistik.Text = kullaniciDal.IstatistikGetir(aktifKullaniciID);
        }

        private Button MenuButon(string t, int y)
        {
            Button b = new Button() { Text = t, Location = new Point(0, y), Size = new Size(250, 60), FlatStyle = FlatStyle.Flat, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(20, 0, 0, 0), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            ButonEfektiEkle(b, Color.FromArgb(0, 0, 0), Color.FromArgb(30, 30, 30));
            return b;
        }

        private void ButonEfektiEkle(Button btn, Color normal, Color hover)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = hover;
            btn.MouseLeave += (s, e) => btn.BackColor = normal;
        }

        private void GridAyarla(DataGridView d)
        {
            d.BackgroundColor = Color.FromArgb(20, 20, 20); d.BorderStyle = BorderStyle.None; d.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            d.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30); d.DefaultCellStyle.ForeColor = Color.White; d.DefaultCellStyle.SelectionBackColor = Color.FromArgb(229, 9, 20); d.DefaultCellStyle.SelectionForeColor = Color.White;
            d.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None; d.ColumnHeadersDefaultCellStyle.BackColor = Color.Black; d.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; d.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold); d.EnableHeadersVisualStyles = false;
            d.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; d.AllowUserToAddRows = false; d.ReadOnly = true; d.SelectionMode = DataGridViewSelectionMode.FullRowSelect; d.RowTemplate.Height = 45; d.RowHeadersVisible = false;
        }
        private void PanelGoster(Panel p) { foreach (Control c in pnlIcerik.Controls) c.Visible = false; p.Visible = true; }
    }
}