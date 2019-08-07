using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PCPOS.Sifarnik
{
    public partial class frmDucani : Form
    {
        public frmDucani()
        {
            InitializeComponent();
        }

        private DataSet DSducani = new DataSet();

        private void frmDucani_Load(object sender, EventArgs e)
        {
            SetDucani();     
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void SetDucani()
        {
            if (dgv.Rows.Count > 0)
            {
                dgv.Rows.Clear();
            }

            DataTable DT = classSQL.select("SELECT id_ducan, ime_ducana, aktivnost FROM ducan;", "stolovi").Tables[0];

            for (int i = 0; i < DT.Rows.Count; i++)
            {
                bool b = false;
                if (DT.Rows[i]["aktivnost"].ToString() == "DA")
                {
                    b = true;
                }
                dgv.Rows.Add(DT.Rows[i]["ime_ducana"].ToString(), b, DT.Rows[i]["id_ducan"].ToString());
            }
        }

        private void btnNoviUnos_Click(object sender, EventArgs e)
        {
            if (txtnazivStola.Text == "")
            {
                MessageBox.Show("Greška niste upisali naziv poslovnice."); return;
            }

            string sql = string.Format(@"INSERT INTO ducan
(
    ime_ducana, aktivnost
)
VALUES
(
    '{0}', 'DA'
);", txtnazivStola.Text);

            classSQL.insert(sql);

            SetDucani();
            MessageBox.Show("Spremljno.");
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv.CurrentCell.ColumnIndex == 0)
            {
                try
                {
                    string sql = string.Format(@"UPDATE ducan
SET
    ime_ducana = '{0}'
WHERE id_ducan = '{1}';",
dgv.Rows[e.RowIndex].Cells["poslovnica"].FormattedValue.ToString(),
dgv.Rows[e.RowIndex].Cells["id_poslovnica"].FormattedValue.ToString());
                    classSQL.update(sql);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else if (dgv.CurrentCell.ColumnIndex == 1)
            {
                try
                {
                    string aa = "NE";
                    if (dgv.Rows[e.RowIndex].Cells["aktivnost"].FormattedValue.ToString() == "True")
                    {
                        aa = "DA";
                    }

                    string sql = string.Format(@"UPDATE ducan
SET
    aktivnost = '{0}'
WHERE id_ducan = '{1}';",
aa,
dgv.Rows[e.RowIndex].Cells["id_poslovnica"].FormattedValue.ToString());
                    classSQL.update(sql);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}