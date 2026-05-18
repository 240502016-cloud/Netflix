using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NetflixCloneProject.DataAccess;

namespace NetflixCloneProject
{
    public partial class Form1 : Form
    {
        KullaniciDAL kullaniciDal = new KullaniciDAL(); ProgramDAL programDal = new ProgramDAL();
        Panel pnlGiris = new Panel(), pnlKayit = new Panel();

        public Form1() { InitializeComponent(); ArayuzuKur(); }

        private void ArayuzuKur()
        {
            this.BackColor = Color.FromArgb(15, 15, 15); this.Size = new Size(1000, 800); this.StartPosition = FormStartPosition.CenterScreen; this.Text = "Netflix - Hoş Geldiniz";
            this.Resize += (s, e) => {
                pnlGiris.Location = new Point((this.Width - pnlGiris.Width) / 2, (this.Height - pnlGiris.Height) / 2);
                pnlKayit.Location = new Point((this.Width - pnlKayit.Width) / 2, (this.Height - pnlKayit.Height) / 2);
            };

            // --- GİRİŞ PANELİ ---
            pnlGiris.Size = new Size(400, 500); pnlGiris.BackColor = Color.FromArgb(10, 10, 10);
            pnlGiris.Controls.Add(new Label() { Text = "NETFLIX", ForeColor = Color.FromArgb(229, 9, 20), Font = new Font("Segoe UI Black", 32, FontStyle.Bold), Location = new Point(80, 40), AutoSize = true });

            TextBox txtGEmail = new TextBox() { PlaceholderText = "E-posta", Location = new Point(50, 150), Size = new Size(300, 40), Font = new Font("Segoe UI", 14), BackColor = Color.FromArgb(51, 51, 51), ForeColor = Color.White, BorderStyle = BorderStyle.None };
            TextBox txtGSifre = new TextBox() { PlaceholderText = "Parola", Location = new Point(50, 210), Size = new Size(300, 40), PasswordChar = '*', Font = new Font("Segoe UI", 14), BackColor = Color.FromArgb(51, 51, 51), ForeColor = Color.White, BorderStyle = BorderStyle.None };

            Button btnGiris = new Button() { Text = "Oturum Aç", BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, Location = new Point(50, 280), Size = new Size(300, 50), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 14, FontStyle.Bold), Cursor = Cursors.Hand };
            btnGiris.FlatAppearance.BorderSize = 0;
            ButonEfektiEkle(btnGiris, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50)); // Parlama efekti

            Label lblKayitYonlendir = new Label() { Text = "Netflix'e katılmak ister misiniz? Şimdi kaydolun.", ForeColor = Color.LightGray, Location = new Point(45, 360), AutoSize = true, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 10) };
            lblKayitYonlendir.MouseEnter += (s, e) => lblKayitYonlendir.ForeColor = Color.White;
            lblKayitYonlendir.MouseLeave += (s, e) => lblKayitYonlendir.ForeColor = Color.LightGray;
            lblKayitYonlendir.Click += (s, e) => { pnlGiris.Visible = false; pnlKayit.Visible = true; };

            btnGiris.Click += (s, e) => {
                int[] g = kullaniciDal.KullaniciGirisYapDetayli(txtGEmail.Text, txtGSifre.Text);
                if (g[0] > 0)
                {
                    if (g[2] == 1) { MessageBox.Show("Hesabınız pasife alınmıştır.", "Erişim Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (g[1] == 2) { new YoneticiForm().Show(); } else { new AnaSayfaForm(g[0]).Show(); }
                    this.Hide();
                }
                else MessageBox.Show("Hatalı e-posta veya şifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            pnlGiris.Controls.AddRange(new Control[] { txtGEmail, txtGSifre, btnGiris, lblKayitYonlendir });

            // --- KAYIT PANELİ ---
            pnlKayit.Size = new Size(450, 700); pnlKayit.BackColor = Color.FromArgb(10, 10, 10); pnlKayit.Visible = false;
            pnlKayit.Controls.Add(new Label() { Text = "ÜYE OL", ForeColor = Color.White, Font = new Font("Segoe UI Black", 24, FontStyle.Bold), Location = new Point(150, 20), AutoSize = true });

            TextBox txtKAd = new TextBox() { PlaceholderText = "Ad", Location = new Point(50, 90), Size = new Size(165, 30), Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(51, 51, 51), ForeColor = Color.White, BorderStyle = BorderStyle.None };
            TextBox txtKSoyad = new TextBox() { PlaceholderText = "Soyad", Location = new Point(235, 90), Size = new Size(165, 30), Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(51, 51, 51), ForeColor = Color.White, BorderStyle = BorderStyle.None };
            TextBox txtKEmail = new TextBox() { PlaceholderText = "E-mail", Location = new Point(50, 140), Size = new Size(350, 30), Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(51, 51, 51), ForeColor = Color.White, BorderStyle = BorderStyle.None };
            TextBox txtKSifre = new TextBox() { PlaceholderText = "Şifre", Location = new Point(50, 190), Size = new Size(350, 30), PasswordChar = '*', Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(51, 51, 51), ForeColor = Color.White, BorderStyle = BorderStyle.None };

            pnlKayit.Controls.Add(new Label() { Text = "Doğum Tarihi:", ForeColor = Color.White, Font = new Font("Segoe UI", 10), Location = new Point(47, 240), AutoSize = true });
            DateTimePicker dtpDogum = new DateTimePicker() { Location = new Point(160, 237), Size = new Size(240, 30), Font = new Font("Segoe UI", 10), Cursor = Cursors.Hand };
            ComboBox cmbCinsiyet = new ComboBox() { Text = "Cinsiyet Seçiniz", Location = new Point(50, 290), Size = new Size(165, 30), Font = new Font("Segoe UI", 11), Cursor = Cursors.Hand }; cmbCinsiyet.Items.AddRange(new string[] { "Erkek", "Kadın", "Diğer" });
            ComboBox cmbUlke = new ComboBox() { Text = "Ülke Seçiniz", Location = new Point(235, 290), Size = new Size(165, 30), Font = new Font("Segoe UI", 11), Cursor = Cursors.Hand }; cmbUlke.Items.AddRange(new string[] { "Türkiye", "ABD", "Almanya", "İngiltere", "Fransa" });

            pnlKayit.Controls.Add(new Label() { Text = "Favori 3 Türünüzü Seçiniz:", ForeColor = Color.White, Font = new Font("Segoe UI", 10), Location = new Point(47, 340), AutoSize = true });
            CheckedListBox clbTurler = new CheckedListBox() { Location = new Point(50, 365), Size = new Size(350, 120), BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.None, Cursor = Cursors.Hand };
            foreach (DataRow r in programDal.TurleriGetir().Rows) clbTurler.Items.Add(r["tur_adi"].ToString());

            Button btnKayit = new Button() { Text = "Hemen Katıl", BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, Location = new Point(50, 520), Size = new Size(350, 50), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 14, FontStyle.Bold), Cursor = Cursors.Hand };
            btnKayit.FlatAppearance.BorderSize = 0;
            ButonEfektiEkle(btnKayit, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50));

            Label lblGirisYonlendir = new Label() { Text = "Zaten üye misiniz? Oturum Açın.", ForeColor = Color.LightGray, Location = new Point(120, 590), AutoSize = true, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 10) };
            lblGirisYonlendir.MouseEnter += (s, e) => lblGirisYonlendir.ForeColor = Color.White;
            lblGirisYonlendir.MouseLeave += (s, e) => lblGirisYonlendir.ForeColor = Color.LightGray;
            lblGirisYonlendir.Click += (s, e) => { pnlKayit.Visible = false; pnlGiris.Visible = true; };

            btnKayit.Click += (s, e) => {
                List<string> secTurler = new List<string>(); foreach (var item in clbTurler.CheckedItems) secTurler.Add(item.ToString());
                if (secTurler.Count != 3) { MessageBox.Show("Tam 3 adet tür seçmelisiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (kullaniciDal.KullaniciKayitEt(txtKAd.Text, txtKSoyad.Text, txtKEmail.Text, txtKSifre.Text, dtpDogum.Value, cmbCinsiyet.Text, cmbUlke.Text, secTurler)) { MessageBox.Show("Aramıza Hoş Geldiniz!", "Başarılı"); pnlKayit.Visible = false; pnlGiris.Visible = true; }
            };

            pnlKayit.Controls.AddRange(new Control[] { txtKAd, txtKSoyad, txtKEmail, txtKSifre, dtpDogum, cmbCinsiyet, cmbUlke, clbTurler, btnKayit, lblGirisYonlendir });
            this.Controls.Add(pnlGiris); this.Controls.Add(pnlKayit);
        }

        // Modern Hover Efekti Metodu
        private void ButonEfektiEkle(Button btn, Color normal, Color hover)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = hover;
            btn.MouseLeave += (s, e) => btn.BackColor = normal;
        }
    }
}