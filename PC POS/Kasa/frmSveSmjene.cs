using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PCPOS.Kasa
{
    public partial class frmSveSmjene : Form
    {
        public frmSveSmjene()
        {
            InitializeComponent();
        }

        private void frmSveSmjene_Load(object sender, EventArgs e)
        {
            LoadSmjene();
       
        }

        private void LoadSmjene()
        {
            string sql = "SELECT pocetno_stanje AS [Blag.Minimum],pocetak AS [Početak],zavrsetak AS [Završetak smjene],zavrsno_stanje AS [Završno stanje],napomena AS [Napomena],id FROM smjene ORDER BY id DESC";
            DataTable DT = classSQL.select(sql, "smjene").Tables[0];
            //dgv.DataSource = DT;

            for (int i = 0; i < DT.Rows.Count; i++)
            {
                dgv.Rows.Add(DT.Rows[i]["Blag.Minimum"].ToString(),
                    DT.Rows[i]["Početak"].ToString(),
                    DT.Rows[i]["Završetak smjene"].ToString(),
                    DT.Rows[i]["Završno stanje"].ToString(),
                    DT.Rows[i]["Napomena"].ToString(),
                    DT.Rows[i]["id"].ToString()
                    );

                dgv.Columns["id"].Visible = false;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Kasa.frmPregledSmjene ps = new Kasa.frmPregledSmjene();
            ps.id = dgv.Rows[e.RowIndex].Cells["id"].FormattedValue.ToString();
            ps.datumOD = dgv.Rows[e.RowIndex].Cells["pocetak"].FormattedValue.ToString();
            ps.datumDO = dgv.Rows[e.RowIndex].Cells["zavrsetak"].FormattedValue.ToString();
            ps.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}