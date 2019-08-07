namespace PCPOS.Resort
{
    partial class FrmAgencijeUredi
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
            this.labelBroj = new System.Windows.Forms.Label();
            this.labelAgencije = new System.Windows.Forms.Label();
            this.labelImeAgencije = new System.Windows.Forms.Label();
            this.labelAktivnost = new System.Windows.Forms.Label();
            this.labelNapomena = new System.Windows.Forms.Label();
            this.textBoxBroj = new System.Windows.Forms.TextBox();
            this.textBoxImeAgencije = new System.Windows.Forms.TextBox();
            this.comboBoxAktivnost = new System.Windows.Forms.ComboBox();
            this.richTextBoxNapomena = new System.Windows.Forms.RichTextBox();
            this.buttonUredi = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelBroj
            // 
            this.labelBroj.AutoSize = true;
            this.labelBroj.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBroj.Location = new System.Drawing.Point(73, 70);
            this.labelBroj.Name = "labelBroj";
            this.labelBroj.Size = new System.Drawing.Size(40, 16);
            this.labelBroj.TabIndex = 11;
            this.labelBroj.Text = "Broj:";
            // 
            // labelAgencije
            // 
            this.labelAgencije.AutoSize = true;
            this.labelAgencije.BackColor = System.Drawing.Color.Transparent;
            this.labelAgencije.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelAgencije.Location = new System.Drawing.Point(57, 19);
            this.labelAgencije.Name = "labelAgencije";
            this.labelAgencije.Size = new System.Drawing.Size(186, 29);
            this.labelAgencije.TabIndex = 12;
            this.labelAgencije.Text = "Uredi agenciju";
            // 
            // labelImeAgencije
            // 
            this.labelImeAgencije.AutoSize = true;
            this.labelImeAgencije.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelImeAgencije.Location = new System.Drawing.Point(12, 106);
            this.labelImeAgencije.Name = "labelImeAgencije";
            this.labelImeAgencije.Size = new System.Drawing.Size(101, 16);
            this.labelImeAgencije.TabIndex = 13;
            this.labelImeAgencije.Text = "Ime agencije:";
            // 
            // labelAktivnost
            // 
            this.labelAktivnost.AutoSize = true;
            this.labelAktivnost.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAktivnost.Location = new System.Drawing.Point(37, 142);
            this.labelAktivnost.Name = "labelAktivnost";
            this.labelAktivnost.Size = new System.Drawing.Size(75, 16);
            this.labelAktivnost.TabIndex = 14;
            this.labelAktivnost.Text = "Aktivnost:";
            // 
            // labelNapomena
            // 
            this.labelNapomena.AutoSize = true;
            this.labelNapomena.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNapomena.Location = new System.Drawing.Point(24, 179);
            this.labelNapomena.Name = "labelNapomena";
            this.labelNapomena.Size = new System.Drawing.Size(88, 16);
            this.labelNapomena.TabIndex = 15;
            this.labelNapomena.Text = "Napomena:";
            // 
            // textBoxBroj
            // 
            this.textBoxBroj.Location = new System.Drawing.Point(119, 69);
            this.textBoxBroj.Name = "textBoxBroj";
            this.textBoxBroj.ReadOnly = true;
            this.textBoxBroj.Size = new System.Drawing.Size(154, 20);
            this.textBoxBroj.TabIndex = 16;
            // 
            // textBoxImeAgencije
            // 
            this.textBoxImeAgencije.Location = new System.Drawing.Point(119, 105);
            this.textBoxImeAgencije.Name = "textBoxImeAgencije";
            this.textBoxImeAgencije.Size = new System.Drawing.Size(154, 20);
            this.textBoxImeAgencije.TabIndex = 17;
            // 
            // comboBoxAktivnost
            // 
            this.comboBoxAktivnost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAktivnost.FormattingEnabled = true;
            this.comboBoxAktivnost.Location = new System.Drawing.Point(118, 141);
            this.comboBoxAktivnost.Name = "comboBoxAktivnost";
            this.comboBoxAktivnost.Size = new System.Drawing.Size(155, 21);
            this.comboBoxAktivnost.TabIndex = 18;
            // 
            // richTextBoxNapomena
            // 
            this.richTextBoxNapomena.Location = new System.Drawing.Point(117, 178);
            this.richTextBoxNapomena.Name = "richTextBoxNapomena";
            this.richTextBoxNapomena.Size = new System.Drawing.Size(156, 68);
            this.richTextBoxNapomena.TabIndex = 19;
            this.richTextBoxNapomena.Text = "";
            // 
            // buttonUredi
            // 
            this.buttonUredi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUredi.Location = new System.Drawing.Point(87, 261);
            this.buttonUredi.Name = "buttonUredi";
            this.buttonUredi.Size = new System.Drawing.Size(134, 36);
            this.buttonUredi.TabIndex = 20;
            this.buttonUredi.Text = "Uredi";
            this.buttonUredi.UseVisualStyleBackColor = true;
            this.buttonUredi.Click += new System.EventHandler(this.buttonUredi_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.button1.Image = global::PCPOS.Properties.Resources.Actions_application_exit_icon;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(285, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 40);
            this.button1.TabIndex = 21;
            this.button1.Text = "Izlaz      ";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FrmAgencijeUredi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SlateGray;
            this.ClientSize = new System.Drawing.Size(417, 313);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonUredi);
            this.Controls.Add(this.richTextBoxNapomena);
            this.Controls.Add(this.comboBoxAktivnost);
            this.Controls.Add(this.textBoxImeAgencije);
            this.Controls.Add(this.textBoxBroj);
            this.Controls.Add(this.labelNapomena);
            this.Controls.Add(this.labelAktivnost);
            this.Controls.Add(this.labelImeAgencije);
            this.Controls.Add(this.labelAgencije);
            this.Controls.Add(this.labelBroj);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAgencijeUredi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Uredi agenciju";
            this.Load += new System.EventHandler(this.FrmAgencijeUredi_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelBroj;
        private System.Windows.Forms.Label labelAgencije;
        private System.Windows.Forms.Label labelImeAgencije;
        private System.Windows.Forms.Label labelAktivnost;
        private System.Windows.Forms.Label labelNapomena;
        private System.Windows.Forms.TextBox textBoxBroj;
        private System.Windows.Forms.TextBox textBoxImeAgencije;
        private System.Windows.Forms.ComboBox comboBoxAktivnost;
        private System.Windows.Forms.RichTextBox richTextBoxNapomena;
        private System.Windows.Forms.Button buttonUredi;
        private System.Windows.Forms.Button button1;
    }
}