namespace ProyectoFinal_Software_4_y_5.Forms
{
    partial class GenerarEstados
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTituloGenerarEstados = new System.Windows.Forms.Label();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.btnGenerarEstado = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.SlateBlue;
            this.pnlHeader.Controls.Add(this.lblTituloGenerarEstados);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(800, 80);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTituloGenerarEstados
            // 
            this.lblTituloGenerarEstados.AutoSize = true;
            this.lblTituloGenerarEstados.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloGenerarEstados.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblTituloGenerarEstados.Location = new System.Drawing.Point(25, 20);
            this.lblTituloGenerarEstados.Name = "lblTituloGenerarEstados";
            this.lblTituloGenerarEstados.Size = new System.Drawing.Size(230, 32);
            this.lblTituloGenerarEstados.TabIndex = 1;
            this.lblTituloGenerarEstados.Text = "Generar Estados";
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.AutoSize = true;
            this.lblFechaDesde.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaDesde.Location = new System.Drawing.Point(213, 171);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(85, 31);
            this.lblFechaDesde.TabIndex = 1;
            this.lblFechaDesde.Text = "Desde:";
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaHasta.Location = new System.Drawing.Point(213, 252);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(81, 31);
            this.lblFechaHasta.TabIndex = 2;
            this.lblFechaHasta.Text = "Hasta:";
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Location = new System.Drawing.Point(325, 178);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(219, 22);
            this.dtpFechaDesde.TabIndex = 3;
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Location = new System.Drawing.Point(325, 259);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(219, 22);
            this.dtpFechaHasta.TabIndex = 4;
            // 
            // btnGenerarEstado
            // 
            this.btnGenerarEstado.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerarEstado.Location = new System.Drawing.Point(305, 347);
            this.btnGenerarEstado.Name = "btnGenerarEstado";
            this.btnGenerarEstado.Size = new System.Drawing.Size(158, 45);
            this.btnGenerarEstado.TabIndex = 5;
            this.btnGenerarEstado.Text = "Aceptar";
            this.btnGenerarEstado.UseVisualStyleBackColor = true;
            // 
            // GenerarEstados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnGenerarEstado);
            this.Controls.Add(this.dtpFechaHasta);
            this.Controls.Add(this.dtpFechaDesde);
            this.Controls.Add(this.lblFechaHasta);
            this.Controls.Add(this.lblFechaDesde);
            this.Controls.Add(this.pnlHeader);
            this.Name = "GenerarEstados";
            this.Text = "GenerarEstados";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTituloGenerarEstados;
        private System.Windows.Forms.Label lblFechaDesde;
        private System.Windows.Forms.Label lblFechaHasta;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.Button btnGenerarEstado;
    }
}