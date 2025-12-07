namespace ProyectoFinal_Software_4_y_5.Forms
{
    partial class NuevaTransaccion
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
            this.lblTituloNuevaTransaccion = new System.Windows.Forms.Label();
            this.lblTipoTransaccion = new System.Windows.Forms.Label();
            this.lblNumeroCuenta = new System.Windows.Forms.Label();
            this.lblMonto = new System.Windows.Forms.Label();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.grpDatosTransaccion = new System.Windows.Forms.GroupBox();
            this.cmbTipoTransaccion = new System.Windows.Forms.ComboBox();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.txtMonto = new System.Windows.Forms.TextBox();
            this.txtNumeroCuenta = new System.Windows.Forms.TextBox();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.grpDatosTransaccion.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.SlateBlue;
            this.pnlHeader.Controls.Add(this.lblTituloNuevaTransaccion);
            this.pnlHeader.Location = new System.Drawing.Point(-1, -2);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(672, 80);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTituloNuevaTransaccion
            // 
            this.lblTituloNuevaTransaccion.AutoSize = true;
            this.lblTituloNuevaTransaccion.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloNuevaTransaccion.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblTituloNuevaTransaccion.Location = new System.Drawing.Point(185, 18);
            this.lblTituloNuevaTransaccion.Name = "lblTituloNuevaTransaccion";
            this.lblTituloNuevaTransaccion.Size = new System.Drawing.Size(270, 32);
            this.lblTituloNuevaTransaccion.TabIndex = 1;
            this.lblTituloNuevaTransaccion.Text = "Nueva Transacción";
            // 
            // lblTipoTransaccion
            // 
            this.lblTipoTransaccion.AutoSize = true;
            this.lblTipoTransaccion.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipoTransaccion.Location = new System.Drawing.Point(18, 37);
            this.lblTipoTransaccion.Name = "lblTipoTransaccion";
            this.lblTipoTransaccion.Size = new System.Drawing.Size(83, 38);
            this.lblTipoTransaccion.TabIndex = 1;
            this.lblTipoTransaccion.Text = "Tipo:";
            // 
            // lblNumeroCuenta
            // 
            this.lblNumeroCuenta.AutoSize = true;
            this.lblNumeroCuenta.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeroCuenta.Location = new System.Drawing.Point(18, 116);
            this.lblNumeroCuenta.Name = "lblNumeroCuenta";
            this.lblNumeroCuenta.Size = new System.Drawing.Size(117, 38);
            this.lblNumeroCuenta.TabIndex = 2;
            this.lblNumeroCuenta.Text = "Cuenta:";
            // 
            // lblMonto
            // 
            this.lblMonto.AutoSize = true;
            this.lblMonto.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonto.Location = new System.Drawing.Point(18, 192);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(114, 38);
            this.lblMonto.TabIndex = 3;
            this.lblMonto.Text = "Monto:";
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescripcion.Location = new System.Drawing.Point(18, 267);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(177, 38);
            this.lblDescripcion.TabIndex = 4;
            this.lblDescripcion.Text = "Descripcion:";
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmar.Location = new System.Drawing.Point(88, 517);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(197, 51);
            this.btnConfirmar.TabIndex = 5;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.UseVisualStyleBackColor = true;
            // 
            // grpDatosTransaccion
            // 
            this.grpDatosTransaccion.Controls.Add(this.cmbTipoTransaccion);
            this.grpDatosTransaccion.Controls.Add(this.txtDescripcion);
            this.grpDatosTransaccion.Controls.Add(this.txtMonto);
            this.grpDatosTransaccion.Controls.Add(this.txtNumeroCuenta);
            this.grpDatosTransaccion.Controls.Add(this.lblMonto);
            this.grpDatosTransaccion.Controls.Add(this.lblTipoTransaccion);
            this.grpDatosTransaccion.Controls.Add(this.lblDescripcion);
            this.grpDatosTransaccion.Controls.Add(this.lblNumeroCuenta);
            this.grpDatosTransaccion.Location = new System.Drawing.Point(54, 113);
            this.grpDatosTransaccion.Name = "grpDatosTransaccion";
            this.grpDatosTransaccion.Size = new System.Drawing.Size(559, 389);
            this.grpDatosTransaccion.TabIndex = 6;
            this.grpDatosTransaccion.TabStop = false;
            // 
            // cmbTipoTransaccion
            // 
            this.cmbTipoTransaccion.FormattingEnabled = true;
            this.cmbTipoTransaccion.Location = new System.Drawing.Point(269, 51);
            this.cmbTipoTransaccion.Name = "cmbTipoTransaccion";
            this.cmbTipoTransaccion.Size = new System.Drawing.Size(254, 24);
            this.cmbTipoTransaccion.TabIndex = 8;
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(269, 275);
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(254, 72);
            this.txtDescripcion.TabIndex = 7;
            // 
            // txtMonto
            // 
            this.txtMonto.Location = new System.Drawing.Point(269, 200);
            this.txtMonto.Multiline = true;
            this.txtMonto.Name = "txtMonto";
            this.txtMonto.Size = new System.Drawing.Size(254, 30);
            this.txtMonto.TabIndex = 6;
            // 
            // txtNumeroCuenta
            // 
            this.txtNumeroCuenta.Location = new System.Drawing.Point(269, 124);
            this.txtNumeroCuenta.Multiline = true;
            this.txtNumeroCuenta.Name = "txtNumeroCuenta";
            this.txtNumeroCuenta.Size = new System.Drawing.Size(254, 30);
            this.txtNumeroCuenta.TabIndex = 5;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(370, 517);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(197, 51);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // NuevaTransaccion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 593);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.grpDatosTransaccion);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.pnlHeader);
            this.Name = "NuevaTransaccion";
            this.Text = "Nueva Transacción";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.grpDatosTransaccion.ResumeLayout(false);
            this.grpDatosTransaccion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTituloNuevaTransaccion;
        private System.Windows.Forms.Label lblTipoTransaccion;
        private System.Windows.Forms.Label lblNumeroCuenta;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.GroupBox grpDatosTransaccion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.TextBox txtMonto;
        private System.Windows.Forms.TextBox txtNumeroCuenta;
        private System.Windows.Forms.ComboBox cmbTipoTransaccion;
        private System.Windows.Forms.Button btnCancelar;
    }
}