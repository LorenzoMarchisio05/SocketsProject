namespace Client.Presentation_WinForm
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.nudClientNumber = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.listBoxClients = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudClientNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // nudClientNumber
            // 
            this.nudClientNumber.Location = new System.Drawing.Point(189, 37);
            this.nudClientNumber.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudClientNumber.Name = "nudClientNumber";
            this.nudClientNumber.Size = new System.Drawing.Size(120, 31);
            this.nudClientNumber.TabIndex = 0;
            this.nudClientNumber.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(557, 33);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(160, 36);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // listBoxClients
            // 
            this.listBoxClients.FormattingEnabled = true;
            this.listBoxClients.ItemHeight = 25;
            this.listBoxClients.Location = new System.Drawing.Point(30, 101);
            this.listBoxClients.Name = "listBoxClients";
            this.listBoxClients.Size = new System.Drawing.Size(687, 529);
            this.listBoxClients.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Numero Client";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 644);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxClients);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.nudClientNumber);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.nudClientNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudClientNumber;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ListBox listBoxClients;
        private System.Windows.Forms.Label label1;
    }
}

