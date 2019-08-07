using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PCPOS.Sifarnik
{
    public partial class frmGrupeProizvoda : Form
    {
        public frmGrupeProizvoda()
        {
            InitializeComponent();
        }

        private DataSet DS = new DataSet();

        private void frmGrupeProizvoda_Load(object sender, EventArgs e)
        {
            SetGrupe();
            
        }
        private void GasenjeForme_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void bgSinkronizacija_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SetGrupe()
        {
            if (dgv.Rows.Count > 0)
            {
                dgv.Rows.Clear();
            }

            DataTable DT = classSQL.select("SELECT * FROM grupa;", "grupa").Tables[0];

            for (int i = 0; i < DT.Rows.Count; i++)
            {
                dgv.Rows.Add(DT.Rows[i]["grupa"].ToString(), DT.Rows[i]["id_grupa"].ToString());
            }
        }

        private void btnNoviUnos_Click(object sender, EventArgs e)
        {
            if (txtnazivGrupe.Text == "")
            {
                MessageBox.Show("Greška niste upisali naziv grupe."); return;
            }

            string s = "SELECT setval('grupa_id_grupa_seq', (SELECT MAX(id_grupa) FROM grupa) + 1);";
            classSQL.insert(s);

            string sql = string.Format(@"INSERT INTO grupa
(
    grupa, novo
)
VALUES
(
    '{0}', '1'
);", txtnazivGrupe.Text);
            classSQL.insert(sql);

            SetGrupe();
            MessageBox.Show("Spremljno.");
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv.CurrentCell.ColumnIndex == 0)
            {
                try
                {
                    string sql = string.Format(@"UPDATE grupa
SET
    grupa = '{0}',
    editirano = '1'
WHERE id_grupa = '{1}';",
dgv.Rows[e.RowIndex].Cells["grupa"].FormattedValue.ToString(),
dgv.Rows[e.RowIndex].Cells["id_grupa"].FormattedValue.ToString());
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