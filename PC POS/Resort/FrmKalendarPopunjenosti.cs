using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PCPOS.Resort
{
    public partial class FrmKalendarPopunjenosti : Form
    {
        public frmMenu MainMenu;

        private int SelectedMonth { get; set; }
        public bool otvoritiNovuFormicu = true;

        public FrmKalendarPopunjenosti()
        {
            InitializeComponent();
        }

        private void FrmKalendarPopunjenosti_Load(object sender, EventArgs e)
        {
            Paint += new PaintEventHandler(FrmKalendarPopunjenosti_Paint); // Sets background gradient
            SelectedMonth = DateTime.Now.Month;
            NumericYearOnLoad();
            SetMonthCB();
            LoadMonthData(SelectedMonth, Convert.ToInt32(numYear.Value));
        }

        private void FrmKalendarPopunjenosti_Paint(object sender, PaintEventArgs e)
        {
            Graphics c = e.Graphics;
            Brush bG = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.AliceBlue, Color.LightSlateGray, 250);
            c.FillRectangle(bG, 0, 0, Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="month"></param>
        public void LoadMonthData(int month, int year)
        {
            ClearGrid();

            // Set columns (days) for selected months
            int days = DateTime.DaysInMonth(year, month);
            if (days > 0)
            {
                dataGridView.Columns.Add("naziv_sobe", "Naziv sobe");
                dataGridView.Columns[0].Width = 150;
                for (int i = 1; i <= days; i++)
                {
                    dataGridView.Columns.Add(i.ToString(), i.ToString());
                    dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }

            // Add rooms
            DataTable DTsobe = Global.Database.GetSobe();
            if (DTsobe?.Rows.Count > 0)
            {
                foreach (DataRow row in DTsobe.Rows)
                {
                    int index = dataGridView.Rows.Add();
                    dataGridView.Rows[index].Cells["naziv_sobe"].Value = row["naziv_sobe"];

                    // Guest reservations
                    DataTable DTrezervacije = Global.Database.GetRezervacije(null, row["broj_sobe"].ToString());
                    if (DTrezervacije.Rows.Count > 0)
                    {
                        foreach (DataRow reservationRow in DTrezervacije.Rows)
                        {
                            DateTime datumDolaska = DateTime.Parse(reservationRow["datum_dolaska"].ToString());
                            DateTime datumOdlaska = DateTime.Parse(reservationRow["datum_odlaska"].ToString());

                            if (datumDolaska.Month == SelectedMonth || datumOdlaska.Month == SelectedMonth)
                            {
                                int daysDifference = (datumOdlaska - datumDolaska).Days;
                                int startingDay = 1;

                                if (SelectedMonth == datumDolaska.Month)
                                    startingDay = datumDolaska.Day;

                                for (int i = 0; i <= daysDifference; i++)
                                {
                                    if ((startingDay + i > days) || (startingDay == 1 && i == datumOdlaska.Day))
                                        break;
                                    

                                    DataGridViewCell cell = dataGridView.Rows[index].Cells[startingDay + i];

                                    if (string.IsNullOrEmpty(cell.FormattedValue.ToString()))
                                    {
                                        // Set cell color
                                        cell.Style.SelectionForeColor = SystemColors.Highlight;

                                        if (reservationRow["naplaceno"].ToString() == "1")
                                        {
                                            cell.Style.BackColor = Color.LightGreen;
                                            cell.Style.ForeColor = Color.LightGreen;

                                        }
                                        else if (reservationRow["naplaceno"].ToString() == "0" && datumOdlaska >= DateTime.Now)
                                        {
                                            cell.Style.BackColor = Color.CornflowerBlue;
                                            cell.Style.ForeColor = Color.CornflowerBlue;
                                        }
                                        else if (reservationRow["naplaceno"].ToString() == "0" && datumOdlaska < DateTime.Now)
                                        {
                                            cell.Style.BackColor = Color.Salmon;
                                            cell.Style.ForeColor = Color.Salmon;
                                        }

                                        cell.Value = "R" + reservationRow["broj"].ToString();

                                        SetHoverText(index, startingDay, i, reservationRow, cell, cell.Value.ToString());
                                        
                                    }
                                    else
                                    {
                                        cell.Style.BackColor = Color.DarkSlateBlue;
                                        cell.Style.ForeColor = Color.DarkSlateBlue;
                                        cell.Style.SelectionForeColor = SystemColors.Highlight;
                                        cell.Value = cell.Value + "R" + reservationRow["broj"].ToString();

                                        SetHoverText(index, startingDay, i, reservationRow, cell, cell.Value.ToString());
                                        continue;
                                    }
                                    

                                }
                            }
                        }
                    }
                }
            }
        }

        //Vraca broj rezervacija koje se nalaze u odabranoj celiji
        //Npr R1R4, vraca 2
        //Npr R1, vraca 1
        private int BrojRezervacijaUOdredjenojCeliji(string cellValue)
        {
            cellValue = cellValue.Remove(0, 1);
            string[] array = cellValue.Split('R');
            return array.Length;
        }

        private void SetHoverText(int index, int startingDay, int i, DataRow reservationRow, DataGridViewCell cell, string cellValue)
        {
            int duljina = BrojRezervacijaUOdredjenojCeliji(dataGridView.Rows[index].Cells[startingDay + i].FormattedValue.ToString());

            //Uzimamo vrijednost koja piše u čeliji
            if (duljina <= 1)
            {
                DataTable dt = Global.Database.GetPartners(reservationRow["id_partner"].ToString());
                if (dt?.Rows.Count > 0)
                {
                    DataRow dataRow = dt.Rows[0];
                    cell.ToolTipText = dataRow["ime_tvrtke"].ToString();
                }
                else
                {
                    cell.ToolTipText = "Nema partnera.";
                }
            }
            else
            {
                //Potreban za pronalazak partnera na odlasku i partnera za dolazak
                cellValue = cellValue.Remove(0, 1);
                string[] array = cellValue.Split('R');

                //Partner 1 ID
                DataTable dt = Global.Database.GetIdPartnera(array[0]);
                string idPartneraNaOdlasku = dt.Rows[0]["id_partner"].ToString();
                //Lik koji odlazi
                string naOdlasku = "";
                dt = Global.Database.GetPartners(idPartneraNaOdlasku);
                if (dt?.Rows.Count > 0)
                {
                    DataRow dataRow = dt.Rows[0];
                    naOdlasku = dataRow["ime_tvrtke"].ToString();
                }

                //Partner 2 ID
                dt = Global.Database.GetIdPartnera(array[1]);
                string idPartneraNaDolasku = dt.Rows[0]["id_partner"].ToString();
                //Lik koji dolazi
                string naDolasku = "";
                dt = Global.Database.GetPartners(idPartneraNaDolasku);
                if (dt?.Rows.Count > 0)
                {
                    DataRow dataRow = dt.Rows[0];
                    naDolasku = dataRow["ime_tvrtke"].ToString();
                }

                //Usluge ID
                //Imam u array[0] ID
                //U unos rezervacije pronađem id_vrsta_usluge
                //Iz tablice vrsta_usluge uzimam ime usluge i to zapisujem također u ToolTipText
                dt = Global.Database.GetIdVrstaUsluge(array[0]);
                string idVrsteUsluge = dt.Rows[0]["id_vrsta_usluge"].ToString();
                //Ime usluge
                //MessageBox.Show(idVrsteUsluge);
                dt = Global.Database.GetVrstaUsluge(idVrsteUsluge);
                string vrstaUsluge = dt.Rows[0]["naziv_usluge"].ToString();
                
                cell.ToolTipText = $"Odlazi: {naOdlasku}\nDolazi: {naDolasku}\nUsluga: {vrstaUsluge}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetMonthCB()
        {
            DataTable DTmonths = new DataTable();
            DTmonths.Columns.Add("number", typeof(int));
            DTmonths.Columns.Add("name", typeof(string));

            DTmonths.Rows.Add(1, "Siječanj");
            DTmonths.Rows.Add(2, "Veljača");
            DTmonths.Rows.Add(3, "Ožujak");
            DTmonths.Rows.Add(4, "Travanj");
            DTmonths.Rows.Add(5, "Svibanj");
            DTmonths.Rows.Add(6, "Lipanj");
            DTmonths.Rows.Add(7, "Srpanj");
            DTmonths.Rows.Add(8, "Kolovoz");
            DTmonths.Rows.Add(9, "Rujan");
            DTmonths.Rows.Add(10, "Listopad");
            DTmonths.Rows.Add(11, "Studeni");
            DTmonths.Rows.Add(12, "Prosinac");

            cbMonth.DataSource = DTmonths;
            cbMonth.ValueMember = "number";
            cbMonth.DisplayMember = "name";

            cbMonth.SelectedValue = DateTime.Now.Month;
        }

        /// <summary>
        ///  
        /// </summary>
        private void NumericYearOnLoad()
        {
            numYear.Minimum = DateTime.Now.Year - 30;
            numYear.Maximum = DateTime.Now.Year + 30;
            numYear.Value = DateTime.Now.Year;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearGrid()
        {
            dataGridView.Columns.Clear();
            dataGridView.Rows.Clear();
            dataGridView.Refresh();
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex <= 0 || e.RowIndex < 0)
                return;

            string cellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            if (string.IsNullOrWhiteSpace(cellValue))
                return;

            cellValue = cellValue.Remove(0, 1);
            string[] array = cellValue.Split('R');

            FrmOpcijeRezervacije form = new FrmOpcijeRezervacije
            {
                BrojRezervacije = Convert.ToInt32(array[0])
            };
            form.ShowDialog();

            otvoritiNovuFormicu = false;
        }

        private void BtnPreviousMonth_Click(object sender, EventArgs e)
        {
            if (SelectedMonth > 1)
            {
                SelectedMonth--;
                cbMonth.SelectedValue = SelectedMonth;
                LoadMonthData(SelectedMonth, Convert.ToInt32(numYear.Value));
            }
        }

        private void BtnNextMonth_Click(object sender, EventArgs e)
        {
            if (SelectedMonth < 12)
            {
                SelectedMonth++;
                cbMonth.SelectedValue = SelectedMonth;
                LoadMonthData(SelectedMonth, Convert.ToInt32(numYear.Value));
            }
        }

        private void CbMonth_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SelectedMonth = (int)cbMonth.SelectedValue;
            LoadMonthData(SelectedMonth, Convert.ToInt32(numYear.Value));
        }

        private void BtnReservation_Click(object sender, EventArgs e)
        {
            FrmRezervacija form = new FrmRezervacija();
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();
            if (form.ReservationCreated)
                LoadMonthData(SelectedMonth, Convert.ToInt32(numYear.Value));
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void dataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            if (otvoritiNovuFormicu)
            {
                //Broj označenih čelija
                int selectedCellCount = dataGridView.GetCellCount(DataGridViewElementStates.Selected);

                if (UserSelectedMultipleRows(ref dataGridView, selectedCellCount))
                    return;

                if (UserSelectedRowHeader(ref dataGridView, selectedCellCount))
                    return;

                int[] days = Days(ref dataGridView, selectedCellCount);
                int firstDay = days[0];
                int lastDay = days[1];

                //OTVORITI FORMU UNOS REZERVACIJE S PARAMETRIMA O KOJOJ SOBI SE RADI, PRVI DAN, POSLJEDNJI DAN, MJESEC, GODINA, TRENUTNO VRIJEME hh:mm:ss
                int rowNumberToInsertIntoComboBox = dataGridView.SelectedCells[0].RowIndex;
                string fullFirstDay = firstDay.ToString() + "." + (cbMonth.SelectedIndex + 1).ToString() + "." + numYear.Value.ToString() + " " + DateTime.Now.ToString("hh:mm:ss");
                string fullLastDay = lastDay.ToString() + "." + (cbMonth.SelectedIndex + 1).ToString() + "." + numYear.Value.ToString() + " " + DateTime.Now.ToString("hh:mm:ss");
                FrmRezervacija frmRezervacija = new FrmRezervacija(rowNumberToInsertIntoComboBox, fullFirstDay, fullLastDay);
                frmRezervacija.ShowDialog();
            }
            otvoritiNovuFormicu = true;
        }

        //Provjera ako je korisnik označio više od 1 reda
        //To se ne smije dopustiti, zbog toga što je moguće napraviti jednu rezervaciju za jednu sobu u jednom trenutku
        private bool UserSelectedMultipleRows(ref DataGridView dataGridView, int selectedCellCount)
        {
            
            int firstCellRowNumber = dataGridView.SelectedCells[0].RowIndex;
            bool oneRowSelected = true;
            for (int i = 1; oneRowSelected && i < selectedCellCount; i++)
            {
                if (dataGridView.SelectedCells[i].RowIndex != firstCellRowNumber)
                    oneRowSelected = false;
            }
            if (!oneRowSelected)
            {
                MessageBox.Show("Možete odabrati samo 1 sobu (red).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return !oneRowSelected;
        }

        //Provjera ako je korisnik označio ROW header
        private bool UserSelectedRowHeader(ref DataGridView dataGridView, int selectedCellCount)
        {
            for (int i = 0; i < selectedCellCount; i++)
            {
                if (dataGridView.SelectedCells[i].ColumnIndex == 0)
                {
                    MessageBox.Show("Krivi odabir. Ne možete odabrati zaglavlje redka / stupca.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }
            return false;
        }

        //Odabiremo prvi i posljednji označeni dan
        //Ovisno je li korisnik označavao s lijeva na desno ili s desna na lijevo, potrebno je posložiti first day i last day
        private int[] Days(ref DataGridView dataGridView, int selectedCellCount)
        {
            int[] days = new int[2];
            int firstDay = dataGridView.SelectedCells[0].ColumnIndex;
            int lastDay = dataGridView.SelectedCells[selectedCellCount - 1].ColumnIndex;
            if (firstDay > lastDay)
            {
                int switchVariable = firstDay;
                firstDay = lastDay;
                lastDay = switchVariable;
            }
            days[0] = firstDay;
            days[1] = lastDay;

            return days;
        }
    }
}
