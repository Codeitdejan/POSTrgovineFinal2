using System;
using GenCode128;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;


namespace PCPOS.Report.KPR
{
    public partial class kpr : Form
    {
        public kpr()
        {
            InitializeComponent();
        }

        private void kpr_Load(object sender, EventArgs e)
        {
            SetValue();
            PlaceniRacuni();
        }

        private void SetValue()
        {          
            DataTable DTpodaci = classSQL.select_settings("SELECT * FROM podaci_tvrtka WHERE id='1'", "podaci_tvrtka").Tables[0];
            DataTable data = podaciTvrtke.Podaci;
            data.Clear();
            DataRow r = data.NewRow();
            foreach (DataRow item in DTpodaci.Rows)
            {
                string sql1 = "SELECT grad, posta FROM grad WHERE id_grad='" + DTpodaci.Rows[0]["id_grad"].ToString() + "'";
                DataTable poslovnica = classSQL.select(sql1, "grad").Tables[0];

                r["naziv"] = item["naziv_djelatnosti"].ToString();                
                r["sifra"] = item["sifra_djelatnosti"].ToString();                
                r["ime_vl"] = item["vl"].ToString();                
                r["adresa_vl"] = item["adresa"].ToString() +", "+poslovnica.Rows[0]["grad"].ToString()+ poslovnica.Rows[0]["posta"].ToString();              
                r["oib"] = item["oib"].ToString();                
                r["ime_tvrtke"] = item["ime_tvrtke"].ToString();                
                r["adresa_tvrtke"] = item["poslovnica_adresa"].ToString() +", "+ item["poslovnica_grad"].ToString();                
            }
            data.Rows.Add(r);
            ReportDataSource dataSource = new ReportDataSource("Podaci", data);
            this.reportViewer1.LocalReport.DataSources.Add(dataSource);
            this.reportViewer1.RefreshReport();
        }

        private void PlaceniRacuni()
        {
            int god, mj, br;
            decimal uplaceno, ukupno, uplacenoGotovina, ukupnoGotovina;
            DateTime datum;
            god = dateTimePicker1.Value.Year;
            mj = dateTimePicker1.Value.Month;

            int trenutniMj = DateTime.Now.Month;
            int trenutnaGod = DateTime.Now.Year;
            int trenutniDan = DateTime.Now.Day;

            if (trenutnaGod == god && trenutniMj == mj) {
                br = trenutniDan;
            }
            else if((trenutnaGod == god && trenutniMj < mj)||(trenutnaGod < god))
            {
                br = 0;
            }
            else
            {
                br = DateTime.DaysInMonth(god, mj);
            }

            DataTable DTracuni = classSQL.select("SELECT datum_racuna, ukupno FROM racuni", "racuni").Tables[0];
            DataTable DTfakture = classSQL.select("SELECT datum, uplaceno FROM salda_konti", "salda_konti").Tables[0];
            DataTable data = podaciTvrtke.placeniRacuni;
            data.Clear();
            
            for(int i = 1;i<=br;i++)
            {
                uplaceno = 0;
                ukupno = 0;
                uplacenoGotovina = 0;
                ukupnoGotovina = 0;
                datum = new DateTime(god, mj , i);

                foreach (DataRow item in DTfakture.Rows)
                {
                    DateTime date2 = System.Convert.ToDateTime(item["datum"]);
                    if(date2 == datum)
                    {
                        uplaceno = System.Convert.ToDecimal(item["uplaceno"]);
                        ukupno += uplaceno;
                    }                    
                }

                foreach (DataRow item in DTracuni.Rows)
                {
                    DateTime date2 = System.Convert.ToDateTime(item["datum_racuna"]);
                    date2 = new DateTime(date2.Year, date2.Month, date2.Day, 0, 0, 0);
                    if (date2 == datum)
                    {
                        uplacenoGotovina = System.Convert.ToDecimal(item["ukupno"]);
                        ukupnoGotovina += uplacenoGotovina;
                    }
                }

                DataRow r = data.NewRow();
                r["broj"] = i;
                r["datum"] = i + "." + mj + "." + god + ".";
                r["negotovina"] = ukupno;
                r["gotovina"] = ukupnoGotovina;
                r["ukupno"] = ukupno + ukupnoGotovina;
                data.Rows.Add(r);
            }
            
            ReportDataSource dataSource = new ReportDataSource("placeniRacuni", data);
            this.reportViewer1.LocalReport.DataSources.Add(dataSource);
            this.reportViewer1.RefreshReport();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();         
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            SetValue();
            PlaceniRacuni();
        }
    }
}
