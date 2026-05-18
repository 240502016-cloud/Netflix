using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using NetflixCloneProject.DataAccess;

namespace NetflixCloneProject
{
    public partial class YoneticiForm : Form
    {
        YoneticiDAL yoneticiDal = new YoneticiDAL(); ProgramDAL programDal = new ProgramDAL();
        Panel pnlIcerik = new Panel(), pnlTur = new Panel(), pnlKullanici = new Panel(), pnlRapor = new Panel();
        DataGridView dgvListe = new DataGridView(); Panel pnlSag = new Panel(), pnlAltMenu = new Panel();

        TextBox txtAd = new TextBox(), txtTip = new TextBox(), txtSure = new TextBox(), txtYil = new TextBox(), txtBolum = new TextBox(), txtAciklama = new TextBox();
        CheckedListBox clbIcerikTur = new CheckedListBox(); int seciliID = 0; ComboBox cmbRapor = new ComboBox();

        public YoneticiForm() { InitializeComponent(); ArayuzuKur(); PanelGoster("Icerik"); }

        private void ArayuzuKur()
        {
            this.Size = new Size(1300, 850); this.StartPosition = FormStartPosition.CenterScreen; this.Text = "Netflix - Yönetici Paneli"; this.BackColor = Color.FromArgb(20, 20, 20);

            Panel pnlSol = new Panel() { Dock = DockStyle.Left, Width = 250, BackColor = Color.FromArgb(10, 10, 10) };
            pnlSol.Controls.Add(new Label() { Text = "ADMIN", ForeColor = Color.FromArgb(229, 9, 20), Font = new Font("Segoe UI Black", 28), Location = new Point(30, 30), AutoSize = true });

            Button b2 = BtnOlustur("🎬 İçerik Yönetimi", 130); b2.Click += (s, e) => PanelGoster("Icerik");
            Button b3 = BtnOlustur("🏷 Tür Yönetimi", 190); b3.Click += (s, e) => PanelGoster("Tur");
            Button b4 = BtnOlustur("👥 Kullanıcılar", 250); b4.Click += (s, e) => PanelGoster("Kullanici");
            Button b1 = BtnOlustur("📊 Raporlar", 310); b1.Click += (s, e) => PanelGoster("Rapor");
            Button b5 = BtnOlustur("🚪 Güvenli Çıkış", 700); b5.ForeColor = Color.DarkGray; b5.Click += (s, e) => { new Form1().Show(); this.Hide(); };
            pnlSol.Controls.AddRange(new Control[] { b1, b2, b3, b4, b5 });

            pnlSag.Dock = DockStyle.Fill;
            dgvListe.Dock = DockStyle.Top; dgvListe.Height = 400; GridAyarla(dgvListe); dgvListe.Cursor = Cursors.Hand; dgvListe.CellClick += DgvListe_CellClick;
            pnlAltMenu.Dock = DockStyle.Fill; pnlAltMenu.BackColor = Color.FromArgb(20, 20, 20);

            // --- İÇERİK YÖNETİMİ ---
            pnlIcerik.Dock = DockStyle.Fill;
            pnlIcerik.Controls.Add(new Label() { Text = "Program Adı:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 11), Location = new Point(30, 20) }); txtAd.Location = new Point(30, 45); txtAd.Size = new Size(200, 30); txtAd.Font = new Font("Segoe UI", 12); txtAd.BackColor = Color.FromArgb(51, 51, 51); txtAd.ForeColor = Color.White; txtAd.BorderStyle = BorderStyle.None;
            pnlIcerik.Controls.Add(new Label() { Text = "Tipi (Film/Dizi):", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 11), Location = new Point(30, 90) }); txtTip.Location = new Point(30, 115); txtTip.Size = new Size(200, 30); txtTip.Font = new Font("Segoe UI", 12); txtTip.BackColor = Color.FromArgb(51, 51, 51); txtTip.ForeColor = Color.White; txtTip.BorderStyle = BorderStyle.None;
            pnlIcerik.Controls.Add(new Label() { Text = "Süre (Dk):", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 11), Location = new Point(30, 160) }); txtSure.Location = new Point(30, 185); txtSure.Size = new Size(200, 30); txtSure.Font = new Font("Segoe UI", 12); txtSure.BackColor = Color.FromArgb(51, 51, 51); txtSure.ForeColor = Color.White; txtSure.BorderStyle = BorderStyle.None;
            pnlIcerik.Controls.Add(new Label() { Text = "Yayın Yılı:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 11), Location = new Point(270, 20) }); txtYil.Location = new Point(270, 45); txtYil.Size = new Size(150, 30); txtYil.Font = new Font("Segoe UI", 12); txtYil.BackColor = Color.FromArgb(51, 51, 51); txtYil.ForeColor = Color.White; txtYil.BorderStyle = BorderStyle.None;
            pnlIcerik.Controls.Add(new Label() { Text = "Bölüm:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 11), Location = new Point(270, 90) }); txtBolum.Location = new Point(270, 115); txtBolum.Size = new Size(150, 30); txtBolum.Font = new Font("Segoe UI", 12); txtBolum.BackColor = Color.FromArgb(51, 51, 51); txtBolum.ForeColor = Color.White; txtBolum.BorderStyle = BorderStyle.None;
            pnlIcerik.Controls.Add(new Label() { Text = "Açıklama:", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 11), Location = new Point(460, 20) }); txtAciklama.Location = new Point(460, 45); txtAciklama.Size = new Size(300, 100); txtAciklama.Multiline = true; txtAciklama.Font = new Font("Segoe UI", 12); txtAciklama.BackColor = Color.FromArgb(51, 51, 51); txtAciklama.ForeColor = Color.White; txtAciklama.BorderStyle = BorderStyle.None;

            pnlIcerik.Controls.Add(new Label() { Text = "Bu İçeriğin Türleri (Çoklu Seçim):", ForeColor = Color.DarkGray, Font = new Font("Segoe UI", 11), Location = new Point(30, 230) });
            clbIcerikTur.Location = new Point(30, 255); clbIcerikTur.Size = new Size(420, 100); clbIcerikTur.BackColor = Color.FromArgb(51, 51, 51); clbIcerikTur.ForeColor = Color.White; clbIcerikTur.BorderStyle = BorderStyle.None; clbIcerikTur.CheckOnClick = true; clbIcerikTur.MultiColumn = true; clbIcerikTur.Cursor = Cursors.Hand;

            Button btnIE = new Button() { Text = "✚ Yeni Ekle", Location = new Point(800, 30), Size = new Size(200, 40), BackColor = Color.ForestGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnIE.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnIE, Color.ForestGreen, Color.LimeGreen);
            btnIE.Click += (s, e) => {
                List<int> secilenTurler = new List<int>(); foreach (DataRowView item in clbIcerikTur.CheckedItems) secilenTurler.Add(Convert.ToInt32(item["id"]));
                int b = int.TryParse(txtBolum.Text, out int bOut) ? bOut : 1; int s2 = int.TryParse(txtSure.Text, out int sOut) ? sOut : 0; int y = int.TryParse(txtYil.Text, out int yOut) ? yOut : 2024;
                yoneticiDal.ProgramIslemi("Ekle", 0, txtAd.Text, txtTip.Text, b, s2, y, txtAciklama.Text, secilenTurler); PanelGoster("Icerik");
            };
            Button btnIG = new Button() { Text = "✎ Güncelle", Location = new Point(800, 80), Size = new Size(200, 40), BackColor = Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnIG.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnIG, Color.DarkOrange, Color.Orange);
            btnIG.Click += (s, e) => {
                List<int> secilenTurler = new List<int>(); foreach (DataRowView item in clbIcerikTur.CheckedItems) secilenTurler.Add(Convert.ToInt32(item["id"]));
                int b = int.TryParse(txtBolum.Text, out int bOut) ? bOut : 1; int s2 = int.TryParse(txtSure.Text, out int sOut) ? sOut : 0; int y = int.TryParse(txtYil.Text, out int yOut) ? yOut : 2024;
                yoneticiDal.ProgramIslemi("Guncelle", seciliID, txtAd.Text, txtTip.Text, b, s2, y, txtAciklama.Text, secilenTurler); PanelGoster("Icerik");
            };
            Button btnIS = new Button() { Text = "✖ Sil", Location = new Point(800, 130), Size = new Size(200, 40), BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnIS.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnIS, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50));
            btnIS.Click += (s, e) => { yoneticiDal.ProgramIslemi("Sil", seciliID, "", "", 0, 0, 0, "", new List<int>()); PanelGoster("Icerik"); };

            Button btnOnar = new Button() { Text = "🛠 Sistemi Excel'den Onar", Location = new Point(800, 210), Size = new Size(200, 50), BackColor = Color.Purple, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Cursor = Cursors.Hand }; btnOnar.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnOnar, Color.Purple, Color.MediumOrchid);
            btnOnar.Click += (s, e) => {
                DialogResult dialogResult = MessageBox.Show("DİKKAT: Bu işlem mevcut tüm içerikleri silecek ve 'ExcelHamVeri' tablosundaki ham verileri alıp baştan dağıtacaktır. Onaylıyor musunuz?", "Veritabanı Onarımı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes) { programDal.VerileriDuzenleVeDagit(); PanelGoster("Icerik"); }
            };

            pnlIcerik.Controls.AddRange(new Control[] { txtAd, txtTip, txtSure, txtYil, txtBolum, txtAciklama, clbIcerikTur, btnIE, btnIG, btnIS, btnOnar });

            // --- TÜR YÖNETİMİ ---
            pnlTur.Dock = DockStyle.Fill;
            pnlTur.Controls.Add(new Label() { Text = "Tür Adı:", ForeColor = Color.White, Font = new Font("Segoe UI", 14), Location = new Point(30, 30) });
            TextBox txtTurAd = new TextBox() { Location = new Point(130, 28), Size = new Size(200, 30), Font = new Font("Segoe UI", 14), BackColor = Color.FromArgb(51, 51, 51), ForeColor = Color.White, BorderStyle = BorderStyle.None }; pnlTur.Controls.Add(txtTurAd);
            Button btnTE = new Button() { Text = "Ekle", Location = new Point(350, 25), Size = new Size(120, 40), BackColor = Color.ForestGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnTE.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnTE, Color.ForestGreen, Color.LimeGreen); btnTE.Click += (s, e) => { yoneticiDal.TurIslemi("Ekle", 0, txtTurAd.Text); PanelGoster("Tur"); };
            Button btnTG = new Button() { Text = "Güncelle", Location = new Point(480, 25), Size = new Size(120, 40), BackColor = Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnTG.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnTG, Color.DarkOrange, Color.Orange); btnTG.Click += (s, e) => { yoneticiDal.TurIslemi("Guncelle", seciliID, txtTurAd.Text); PanelGoster("Tur"); };
            Button btnTS = new Button() { Text = "Sil", Location = new Point(610, 25), Size = new Size(120, 40), BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnTS.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnTS, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50)); btnTS.Click += (s, e) => { yoneticiDal.TurIslemi("Sil", seciliID, ""); PanelGoster("Tur"); };
            pnlTur.Controls.AddRange(new Control[] { btnTE, btnTG, btnTS });

            // --- KULLANICI YÖNETİMİ ---
            pnlKullanici.Dock = DockStyle.Fill;
            Button btnPasif = new Button() { Text = "✖ Kullanıcıyı Banla (Pasif)", Location = new Point(30, 30), Size = new Size(300, 50), BackColor = Color.FromArgb(229, 9, 20), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnPasif.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnPasif, Color.FromArgb(229, 9, 20), Color.FromArgb(255, 50, 50)); btnPasif.Click += (s, e) => { yoneticiDal.KullaniciPasifYap(seciliID, true); PanelGoster("Kullanici"); };
            Button btnAktif = new Button() { Text = "✔ Kullanıcı Engelini Kaldır", Location = new Point(350, 30), Size = new Size(300, 50), BackColor = Color.ForestGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnAktif.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnAktif, Color.ForestGreen, Color.LimeGreen); btnAktif.Click += (s, e) => { yoneticiDal.KullaniciPasifYap(seciliID, false); PanelGoster("Kullanici"); };
            pnlKullanici.Controls.AddRange(new Control[] { btnPasif, btnAktif });

            // --- RAPOR YÖNETİMİ ---
            pnlRapor.Dock = DockStyle.Fill;
            cmbRapor.Items.AddRange(new string[] { "En Çok İzlenen 10 İçerik", "En Yüksek Puanlı 10 İçerik", "En Çok İzlenen Türler", "En Aktif Kullanıcılar", "Son 7 Günde İzlenenler", "Toplam Kullanıcı Sayısı", "Toplam İçerik İzlenme", "Toplam Verilen Puan Sayısı" });
            cmbRapor.Location = new Point(30, 30); cmbRapor.Size = new Size(350, 30); cmbRapor.DropDownStyle = ComboBoxStyle.DropDownList; cmbRapor.Font = new Font("Segoe UI", 14); cmbRapor.BackColor = Color.FromArgb(51, 51, 51); cmbRapor.ForeColor = Color.White; cmbRapor.Cursor = Cursors.Hand;
            Button btnRapor = new Button() { Text = "Rapor Oluştur", Location = new Point(400, 28), BackColor = Color.DodgerBlue, ForeColor = Color.White, Size = new Size(180, 40), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand }; btnRapor.FlatAppearance.BorderSize = 0; ButonEfektiEkle(btnRapor, Color.DodgerBlue, Color.DeepSkyBlue); btnRapor.Click += (s, e) => { dgvListe.DataSource = yoneticiDal.RaporCek(cmbRapor.SelectedIndex); };
            pnlRapor.Controls.AddRange(new Control[] { cmbRapor, btnRapor });

            pnlAltMenu.Controls.AddRange(new Control[] { pnlIcerik, pnlTur, pnlKullanici, pnlRapor });
            pnlSag.Controls.Add(pnlAltMenu); pnlSag.Controls.Add(dgvListe);
            this.Controls.Add(pnlSag); this.Controls.Add(pnlSol);
        }

        private void DgvListe_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (pnlIcerik.Visible)
            {
                seciliID = Convert.ToInt32(dgvListe.Rows[e.RowIndex].Cells["ID"].Value); txtAd.Text = dgvListe.Rows[e.RowIndex].Cells["İçerik Adı"].Value.ToString(); txtTip.Text = dgvListe.Rows[e.RowIndex].Cells["Tip"].Value.ToString(); txtBolum.Text = dgvListe.Rows[e.RowIndex].Cells["Bölüm"].Value.ToString(); txtSure.Text = dgvListe.Rows[e.RowIndex].Cells["Süre"].Value.ToString(); txtYil.Text = dgvListe.Rows[e.RowIndex].Cells["Yıl"].Value.ToString();
                string turlerStr = dgvListe.Rows[e.RowIndex].Cells["Türleri"].Value.ToString();
                for (int i = 0; i < clbIcerikTur.Items.Count; i++) { DataRowView rv = (DataRowView)clbIcerikTur.Items[i]; clbIcerikTur.SetItemChecked(i, turlerStr.Contains(rv["tur_adi"].ToString())); }
            }
            else if (pnlTur.Visible) { seciliID = Convert.ToInt32(dgvListe.Rows[e.RowIndex].Cells[0].Value); pnlTur.Controls[1].Text = dgvListe.Rows[e.RowIndex].Cells[1].Value.ToString(); }
            else if (pnlKullanici.Visible) { seciliID = Convert.ToInt32(dgvListe.Rows[e.RowIndex].Cells["ID"].Value); }
        }

        private void PanelGoster(string tip)
        {
            foreach (Control c in pnlAltMenu.Controls) c.Visible = false;
            if (tip == "Icerik") { pnlIcerik.Visible = true; pnlIcerik.BringToFront(); dgvListe.DataSource = programDal.IcerikleriGetir(); dgvListe.Columns["ID"].Visible = false; ((ListBox)clbIcerikTur).DataSource = programDal.TurleriGetir(); ((ListBox)clbIcerikTur).DisplayMember = "tur_adi"; ((ListBox)clbIcerikTur).ValueMember = "id"; }
            else if (tip == "Tur") { pnlTur.Visible = true; pnlTur.BringToFront(); dgvListe.DataSource = programDal.TurleriGetir(); }
            else if (tip == "Kullanici") { pnlKullanici.Visible = true; pnlKullanici.BringToFront(); dgvListe.DataSource = yoneticiDal.KullanicilariGetir(); dgvListe.Columns["ID"].Visible = false; }
            else if (tip == "Rapor") { pnlRapor.Visible = true; pnlRapor.BringToFront(); dgvListe.DataSource = null; }
        }

        private Button BtnOlustur(string t, int y)
        {
            Button btn = new Button() { Text = t, Location = new Point(0, y), Size = new Size(250, 60), FlatStyle = FlatStyle.Flat, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(20, 0, 0, 0), Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            ButonEfektiEkle(btn, Color.FromArgb(10, 10, 10), Color.FromArgb(40, 40, 40));
            return btn;
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
            d.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; d.AllowUserToAddRows = false; d.ReadOnly = true; d.SelectionMode = DataGridViewSelectionMode.FullRowSelect; d.RowTemplate.Height = 40; d.RowHeadersVisible = false;
        }
    }
}