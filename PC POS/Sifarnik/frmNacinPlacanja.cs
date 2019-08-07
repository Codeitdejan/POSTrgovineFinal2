using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PCPOS.Sifarnik
{
    public partial class frmNacinPlacanja : Form
    {
        public frmNacinPlacanja()
        {
            InitializeComponent();
        }

        private DataSet DSsk = new DataSet();

        private void frmSkladista_Load(object sender, EventArgs e)
        {
            SetDgv();
           
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SetDgv()
        {
            if (dgvSk.Rows.Count != 0)
            {
                dgvSk.Rows.Clear();
            }

            DataTable DT = classSQL.select("SELECT * FROM nacin_placanja;", "nacin_placanja").Tables[0];

            for (int i = 0; i < DT.Rows.Count; i++)
            {
                dgvSk.Rows.Add(DT.Rows[i]["naziv_placanja"].ToString(), DT.Rows[i]["ostalo"].ToString(), DT.Rows[i]["id_placanje"].ToString());
            }
        }

        private void provjera_sql(string str)
        {
            if (str != "")
            {
                MessageBox.Show(str);
            }
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            if (txtNaziv.Text == "")
            {
                MessageBox.Show("Niste pravilno upisali način plaćanja."); return;
            }

            provjera_sql(classSQL.insert(string.Format(@"INSERT INTO nacin_placanja
(
    naziv_placanja, ostalo
)
VALUES
(
    '{0}', '{1}'
);",
    txtNaziv.Text,
    txtOstalo.Text)));

            SetDgv();
        }

        private void dgvSk_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSk.CurrentCell.ColumnIndex == 0)
            {
                try
                {
                    string sql = string.Format(@"UPDATE nacin_placanja
SET
    naziv_placanja = '{0}'
WHERE id_placanje = '{1}';",
dgvSk.Rows[e.RowIndex].Cells["Naziv"].FormattedValue.ToString(),
dgvSk.Rows[e.RowIndex].Cells["id_placanja"].FormattedValue.ToString());

                    classSQL.update(sql);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else if (dgvSk.CurrentCell.ColumnIndex == 1)
            {
                try
                {
                    string sql = string.Format(@"UPDATE nacin_placanja
SET
    ostalo = '{0}'
WHERE id_placanje = '{1}';",
dgvSk.Rows[e.RowIndex].Cells["ostalo"].FormattedValue.ToString(),
dgvSk.Rows[e.RowIndex].Cells["id_placanja"].FormattedValue.ToString());
                    classSQL.update(sql);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void frmStopePoreza_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnNovo.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}