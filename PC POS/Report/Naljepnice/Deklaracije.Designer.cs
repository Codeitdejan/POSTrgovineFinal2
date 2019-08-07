namespace PCPOS.Report.Naljepnice
{
    partial class Deklaracije
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Deklaracije));
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.DTRobaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dSDeklaracije = new PCPOS.Report.Naljepnice.DSDeklaracije();
            this.txtOstalo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProizvodac = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtZapocniOdBroja = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btnKreiraj = new System.Windows.Forms.Button();
            this.txtBrojNaljepnica = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtZemlja = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtUvoznik = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtIzvoznik = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSifra = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNaslov = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPartner = new System.Windows.Forms.PictureBox();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DTRobaBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dSDeklaracije)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPartner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // DTRobaBindingSource
            // 
            this.DTRobaBindingSource.DataMember = "DTRoba";
            this.DTRobaBindingSource.DataSource = this.dSDeklaracije;
            // 
            // dSDeklaracije
            // 
            this.dSDeklaracije.DataSetName = "DSDeklaracije";
            this.dSDeklaracije.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // txtOstalo
            // 
            this.txtOstalo.Location = new System.Drawing.Point(61, 265);
            this.txtOstalo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtOstalo.MaxLength = 60;
            this.txtOstalo.Multiline = true;
            this.txtOstalo.Name = "txtOstalo";
            this.txtOstalo.Size = new System.Drawing.Size(164, 72);
            this.txtOstalo.TabIndex = 604;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 267);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 603;
            this.label2.Text = "Ostalo:";
            // 
            // txtProizvodac
            // 
            this.txtProizvodac.Location = new System.Drawing.Point(80, 151);
            this.txtProizvodac.MaxLength = 30;
            this.txtProizvodac.Name = "txtProizvodac";
            this.txtProizvodac.Size = new System.Drawing.Size(146, 20);
            this.txtProizvodac.TabIndex = 601;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 154);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(64, 13);
            this.label14.TabIndex = 602;
            this.label14.Text = "Proizvođač:";
            // 
            // txtZapocniOdBroja
            // 
            this.txtZapocniOdBroja.Location = new System.Drawing.Point(111, 375);
            this.txtZapocniOdBroja.Name = "txtZapocniOdBroja";
            this.txtZapocniOdBroja.Size = new System.Drawing.Size(114, 20);
            this.txtZapocniOdBroja.TabIndex = 591;
            this.txtZapocniOdBroja.Text = "1";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 375);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(90, 13);
            this.label13.TabIndex = 599;
            this.label13.Text = "Započni od broja:";
            // 
            // btnKreiraj
            // 
            this.btnKreiraj.Location = new System.Drawing.Point(17, 404);
            this.btnKreiraj.Name = "btnKreiraj";
            this.btnKreiraj.Size = new System.Drawing.Size(207, 30);
            this.btnKreiraj.TabIndex = 592;
            this.btnKreiraj.Text = "Kreiraj";
            this.btnKreiraj.UseVisualStyleBackColor = true;
            this.btnKreiraj.Click += new System.EventHandler(this.btnKreiraj_Click);
            // 
            // txtBrojNaljepnica
            // 
            this.txtBrojNaljepnica.Location = new System.Drawing.Point(111, 347);
            this.txtBrojNaljepnica.Name = "txtBrojNaljepnica";
            this.txtBrojNaljepnica.Size = new System.Drawing.Size(114, 20);
            this.txtBrojNaljepnica.TabIndex = 590;
            this.txtBrojNaljepnica.Text = "10";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 351);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 13);
            this.label12.TabIndex = 598;
            this.label12.Text = "Broj naljepnica:";
            // 
            // txtZemlja
            // 
            this.txtZemlja.Location = new System.Drawing.Point(103, 236);
            this.txtZemlja.MaxLength = 26;
            this.txtZemlja.Name = "txtZemlja";
            this.txtZemlja.Size = new System.Drawing.Size(122, 20);
            this.txtZemlja.TabIndex = 589;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 239);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 13);
            this.label10.TabIndex = 597;
            this.label10.Text = "Zemlja podrijetla:";
            // 
            // txtUvoznik
            // 
            this.txtUvoznik.Location = new System.Drawing.Point(64, 208);
            this.txtUvoznik.MaxLength = 30;
            this.txtUvoznik.Name = "txtUvoznik";
            this.txtUvoznik.Size = new System.Drawing.Size(161, 20);
            this.txtUvoznik.TabIndex = 588;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 210);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 596;
            this.label8.Text = "Uvoznik:";
            // 
            // txtIzvoznik
            // 
            this.txtIzvoznik.Location = new System.Drawing.Point(64, 180);
            this.txtIzvoznik.MaxLength = 30;
            this.txtIzvoznik.Name = "txtIzvoznik";
            this.txtIzvoznik.Size = new System.Drawing.Size(161, 20);
            this.txtIzvoznik.TabIndex = 587;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 595;
            this.label6.Text = "Izvoznik:";
            // 
            // txtSifra
            // 
            this.txtSifra.Location = new System.Drawing.Point(61, 94);
            this.txtSifra.MaxLength = 25;
            this.txtSifra.Name = "txtSifra";
            this.txtSifra.Size = new System.Drawing.Size(132, 20);
            this.txtSifra.TabIndex = 585;
            this.txtSifra.Text = "2070600461";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 593;
            this.label4.Text = "Šifra:";
            // 
            // txtNaslov
            // 
            this.txtNaslov.Location = new System.Drawing.Point(61, 123);
            this.txtNaslov.MaxLength = 30;
            this.txtNaslov.Name = "txtNaslov";
            this.txtNaslov.Size = new System.Drawing.Size(164, 20);
            this.txtNaslov.TabIndex = 586;
            this.txtNaslov.Text = "Dječja majca";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 594;
            this.label1.Text = "Naslov:";
            // 
            // btnPartner
            // 
            this.btnPartner.BackColor = System.Drawing.Color.Transparent;
            this.btnPartner.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPartner.Image = ((System.Drawing.Image)(resources.GetObject("btnPartner.Image")));
            this.btnPartner.Location = new System.Drawing.Point(194, 89);
            this.btnPartner.Name = "btnPartner";
            this.btnPartner.Size = new System.Drawing.Size(31, 28);
            this.btnPartner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPartner.TabIndex = 600;
            this.btnPartner.TabStop = false;
            this.btnPartner.Click += new System.EventHandler(this.btnPartner_Click);
            // 
            // reportViewer1
            // 
            reportDataSource1.Name = "DSDeklaracije";
            reportDataSource1.Value = this.DTRobaBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "PCPOS.Report.Naljepnice.Deklaracije.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(237, 94);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(787, 631);
            this.reportViewer1.TabIndex = 605;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataMember = "DTRoba";
            this.bindingSource1.DataSource = this.dSDeklaracije;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(527, 78);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 13);
            this.label3.TabIndex = 606;
            this.label3.Text = "Print na etikete 70x37mm (T-46)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(436, 20);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(172, 36);
            this.label5.TabIndex = 607;
            this.label5.Text = "Deklaracije";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.button1.Image = global::PCPOS.Properties.Resources.Actions_application_exit_icon;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(913, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 40);
            this.button1.TabIndex = 608;
            this.button1.Text = "Izlaz      ";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Deklaracije
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(1055, 648);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.reportViewer1);
            this.Controls.Add(this.txtOstalo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtProizvodac);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtZapocniOdBroja);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.btnPartner);
            this.Controls.Add(this.btnKreiraj);
            this.Controls.Add(this.txtBrojNaljepnica);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtZemlja);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtUvoznik);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtIzvoznik);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSifra);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtNaslov);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Deklaracije";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Deklaracije";
            this.Load += new System.EventHandler(this.Deklaracije_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DTRobaBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dSDeklaracije)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPartner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOstalo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtProizvodac;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtZapocniOdBroja;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.PictureBox btnPartner;
        private System.Windows.Forms.Button btnKreiraj;
        private System.Windows.Forms.TextBox txtBrojNaljepnica;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtZemlja;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtUvoznik;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtIzvoznik;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSifra;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNaslov;
        private System.Windows.Forms.Label label1;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private DSDeklaracije dSDeklaracije;
        private System.Windows.Forms.BindingSource DTRobaBindingSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
    }
}