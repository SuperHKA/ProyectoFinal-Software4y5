namespace ProyectoFinal_Software_4_y_5.Forms
{
    partial class VerHistorial
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
            this.lblTituloHistorial = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.dgvHistorialTransacciones = new System.Windows.Forms.DataGridView();
            this.Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Monto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialTransacciones)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.SlateBlue;
            this.pnlHeader.Controls.Add(this.lblTituloHistorial);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(742, 70);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTituloHistorial
            // 
            this.lblTituloHistorial.AutoSize = true;
            this.lblTituloHistorial.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloHistorial.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblTituloHistorial.Location = new System.Drawing.Point(25, 18);
            this.lblTituloHistorial.Name = "lblTituloHistorial";
            this.lblTituloHistorial.Size = new System.Drawing.Size(180, 32);
            this.lblTituloHistorial.TabIndex = 1;
            this.lblTituloHistorial.Text = "Ver Historial";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.Location = new System.Drawing.Point(277, 406);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(171, 49);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            // 
            // dgvHistorialTransacciones
            // 
            this.dgvHistorialTransacciones.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvHistorialTransacciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorialTransacciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Fecha,
            this.Monto,
            this.Descripcion});
            this.dgvHistorialTransacciones.Location = new System.Drawing.Point(94, 108);
            this.dgvHistorialTransacciones.Name = "dgvHistorialTransacciones";
            this.dgvHistorialTransacciones.RowHeadersWidth = 51;
            this.dgvHistorialTransacciones.RowTemplate.Height = 24;
            this.dgvHistorialTransacciones.Size = new System.Drawing.Size(551, 272);
            this.dgvHistorialTransacciones.TabIndex = 3;
            this.dgvHistorialTransacciones.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Fecha
            // 
            this.Fecha.HeaderText = "Fecha";
            this.Fecha.MinimumWidth = 6;
            this.Fecha.Name = "Fecha";
            this.Fecha.Width = 125;
            // 
            // Monto
            // 
            this.Monto.HeaderText = "Monto";
            this.Monto.MinimumWidth = 6;
            this.Monto.Name = "Monto";
            this.Monto.Width = 125;
            // 
            // Descripcion
            // 
            this.Descripcion.HeaderText = "Descripcion";
            this.Descripcion.MinimumWidth = 6;
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.Width = 125;
            // 
            // VerHistorial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(742, 482);
            this.Controls.Add(this.dgvHistorialTransacciones);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.pnlHeader);
            this.Name = "VerHistorial";
            this.Text = "VerHistorial";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialTransacciones)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTituloHistorial;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.DataGridView dgvHistorialTransacciones;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Monto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
    }
}