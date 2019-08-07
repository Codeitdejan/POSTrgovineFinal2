using System;
using GenCode128;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace PCPOS.Report.Naljepnice
{
    public partial class Deklaracije : Form
    {
        public Deklaracije()
        {
            InitializeComponent();
        }

        private void btnPartner_Click(object sender, EventArgs e)
        {
            frmRobaTrazi roba_trazi = new frmRobaTrazi();
            roba_trazi.ShowDialog();
            string propertis_sifra = Properties.Settings.Default.id_roba.Trim();
            if (propertis_sifra != "")
            {
                string sql = "SELECT * FROM roba WHERE sifra='" + propertis_sifra + "'";

                DataTable DTRoba = classSQL.select(sql, "roba").Tables[0];
                if (DTRoba.Rows.Count > 0)
                {
                    txtSifra.Text = DTRoba.Rows[0]["sifra"].ToString();
                    txtNaslov.Text = DTRoba.Rows[0]["naziv"].ToString();
                }
                else
                {
                    MessageBox.Show("Za ovu šifru ne postoji artikl ili usluga.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Deklaracije_Load(object sender, EventArgs e)
        {
            try
            {
                
                if (File.Exists("Deklaracije"))
                {
                    string[] podaci = File.ReadAllLines("Deklaracije");
                    txtSifra.Text = podaci[0];
                    txtNaslov.Text = podaci[1];
                    txtProizvodac.Text = podaci[2];
                    txtIzvoznik.Text = podaci[3];
                    txtUvoznik.Text = podaci[4];
                    txtZemlja.Text = podaci[5];
                    txtOstalo.Text = podaci[6];
                    txtBrojNaljepnica.Text = podaci[7];
                    txtZapocniOdBroja.Text = podaci[8];
                }

            }
            catch (Exception ex)
            {
            }
            this.reportViewer1.RefreshReport();
        }

        private void btnKreiraj_Click(object sender, EventArgs e)
        {
            try
            {
                int brojZadanihNaljepnica;
                int.TryParse(txtBrojNaljepnica.Text, out brojZadanihNaljepnica);

                int zapocniOdBroja = 0;
                int.TryParse(txtZapocniOdBroja.Text, out zapocniOdBroja);

                int brojGeneriranih = 0;

                DataTable DTlista = dSDeklaracije.DTRoba;

                DTlista.Clear();

                for (int i = 0; i < (brojZadanihNaljepnica + zapocniOdBroja); i = i + 3)
                {
                    bool _1 = false, _2 = false, _3 = false;

                    if ((i + 1) >= zapocniOdBroja && brojGeneriranih < brojZadanihNaljepnica)
                    {
                        _1 = true;
                        brojGeneriranih++;
                    }
                    if ((i + 2) >= zapocniOdBroja && brojGeneriranih < brojZadanihNaljepnica)
                    {
                        _2 = true;
                        brojGeneriranih++;
                    }
                    if ((i + 3) >= zapocniOdBroja && brojGeneriranih < brojZadanihNaljepnica)
                    {
                        _3 = true;
                        brojGeneriranih++;
                    }


                    DataRow r = DTlista.NewRow();
                    r["sifra"] = _1 ? "ŠIFRA: " + txtSifra.Text : "";
                    r["naziv_robe"] = _1 ? "NAZIV ROBE: " + txtNaslov.Text : "";
                    r["proizvodac"] = _1 ? "PROIZVOĐAČ: " + txtProizvodac.Text : "";
                    r["izvoznik"] = _1 ? "IZVOZNIK: " + txtIzvoznik.Text : "";
                    r["uvoznik"] = _1 ? "UVOZNIK: " + txtUvoznik.Text : "";
                    r["zemlja_podrijetla"] = _1 ? "ZEMLJA PODRIJETLA: " + txtZemlja.Text : "";
                    r["ostalo"] = _1 ? "OSTALO: " + txtOstalo.Text : "";
                    
                    r["sifra1"] = _2 ? "ŠIFRA: " + txtSifra.Text : "";
                    r["naziv_robe1"] = _2 ? "NAZIV ROBE: " + txtNaslov.Text : "";
                    r["proizvodac1"] = _2 ? "PROIZVOĐAČ: " + txtProizvodac.Text : "";
                    r["izvoznik1"] = _2 ? "IZVOZNIK: " + txtIzvoznik.Text : "";
                    r["uvoznik1"] = _2 ? "UVOZNIK: " + txtUvoznik.Text : "";
                    r["zamlja_podrijetla1"] = _2 ? "ZEMLJA PODRIJETLA: " + txtZemlja.Text : "";
                    r["ostalo1"] = _2 ? "OSTALO: " + txtOstalo.Text : "";

                    r["sifra2"] = _3 ? "ŠIFRA: " + txtSifra.Text : "";
                    r["naziv_robe2"] = _3 ? "NAZIV ROBE: " + txtNaslov.Text : "";
                    r["proizvodac2"] = _3 ? "PROIZVOĐAČ: " + txtProizvodac.Text : "";
                    r["izvoznik2"] = _3 ? "IZVOZNIK: " + txtIzvoznik.Text : "";
                    r["uvoznik2"] = _3 ? "UVOZNIK: " + txtUvoznik.Text : "";
                    r["zamlja_podrijetla2"] = _3 ? "ZEMLJA PODRIJETLA: " + txtZemlja.Text : "";
                    r["ostalo2"] = _3 ? "OSTALO: " + txtOstalo.Text : "";

                    DTlista.Rows.Add(r);
                }

                this.reportViewer1.RefreshReport();

                File.WriteAllText("Deklaracije",
                    txtSifra.Text + "\r\n" +
                    txtNaslov.Text + "\r\n" +
                    txtProizvodac.Text + "\r\n" +
                    txtIzvoznik.Text + "\r\n" +
                    txtUvoznik.Text + "\r\n" +
                    txtZemlja.Text + "\r\n" +
                    txtOstalo.Text + "\r\n" +
                    txtBrojNaljepnica.Text + "\r\n" +
                    txtZapocniOdBroja.Text +
                    "");

            }
            catch (Exception ex)
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
