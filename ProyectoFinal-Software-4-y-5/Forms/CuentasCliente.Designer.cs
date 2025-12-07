namespace ProyectoFinal_Software_4_y_5.Forms
{
    partial class CuentasCliente
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.dgvCuentas = new System.Windows.Forms.DataGridView();
            this.lblInstrucciones = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentas)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.SlateBlue;
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Padding = new System.Windows.Forms.Padding(20);
            this.panelHeader.Size = new System.Drawing.Size(800, 80);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(20, 22);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(230, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Cuentas del Cliente";
            // 
            // dgvCuentas
            // 
            this.dgvCuentas.AllowUserToAddRows = false;
            this.dgvCuentas.AllowUserToDeleteRows = false;
            this.dgvCuentas.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.dgvCuentas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCuentas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCuentas.BackgroundColor = System.Drawing.Color.White;
            this.dgvCuentas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCuentas.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvCuentas.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.SlateBlue;
            this.dgvCuentas.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.dgvCuentas.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvCuentas.ColumnHeadersHeight = 40;
            this.dgvCuentas.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(90)))), ((int)(((byte)(168)))));
            this.dgvCuentas.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvCuentas.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dgvCuentas.Location = new System.Drawing.Point(20, 100);
            this.dgvCuentas.MultiSelect = false;
            this.dgvCuentas.Name = "dgvCuentas";
            this.dgvCuentas.ReadOnly = true;
            this.dgvCuentas.RowHeadersVisible = false;
            this.dgvCuentas.RowHeadersWidth = 51;
            this.dgvCuentas.RowTemplate.Height = 24;
            this.dgvCuentas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCuentas.Size = new System.Drawing.Size(760, 420);
            this.dgvCuentas.TabIndex = 1;
            this.dgvCuentas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvCuentas_CellDoubleClick);
            // 
            // lblInstrucciones
            // 
            this.lblInstrucciones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInstrucciones.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblInstrucciones.ForeColor = System.Drawing.Color.Gray;
            this.lblInstrucciones.Location = new System.Drawing.Point(20, 530);
            this.lblInstrucciones.Name = "lblInstrucciones";
            this.lblInstrucciones.Size = new System.Drawing.Size(760, 20);
            this.lblInstrucciones.TabIndex = 2;
            this.lblInstrucciones.Text = "Haga doble clic en una cuenta para ver sus detalles";
            this.lblInstrucciones.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CuentasCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(800, 570);
            this.Controls.Add(this.lblInstrucciones);
            this.Controls.Add(this.dgvCuentas);
            this.Controls.Add(this.panelHeader);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "CuentasCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cuentas del Cliente";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCuentas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvCuentas;
        private System.Windows.Forms.Label lblInstrucciones;
    }
}
