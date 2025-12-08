using System.Drawing;
using System.Windows.Forms;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    partial class PanelAdministrativo
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
            this.panelSuperior = new System.Windows.Forms.Panel();
            this.lblTituloSistema = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.tabPanelAdmin = new System.Windows.Forms.TabControl();
            this.tabDashboard = new System.Windows.Forms.TabPage();
            this.panelDashboard = new System.Windows.Forms.Panel();
            this.panelTransaccionesRecientes = new System.Windows.Forms.Panel();
            this.gridTransaccionesRecientes = new System.Windows.Forms.DataGridView();
            this.lblTransRecientes = new System.Windows.Forms.Label();
            this.panelEstadisticas = new System.Windows.Forms.Panel();
            this.flowLayoutEstadisticas = new System.Windows.Forms.FlowLayoutPanel();
            this.panelTotalClientes = new System.Windows.Forms.Panel();
            this.lblTotalClientes = new System.Windows.Forms.Label();
            this.lblTotalCliente = new System.Windows.Forms.Label();
            this.panelTotalCuentas = new System.Windows.Forms.Panel();
            this.lblTotalCuentas = new System.Windows.Forms.Label();
            this.lblTotalCuenta = new System.Windows.Forms.Label();
            this.panelSaldoTotal = new System.Windows.Forms.Panel();
            this.lblSaldoTotal = new System.Windows.Forms.Label();
            this.lblSaldoSis = new System.Windows.Forms.Label();
            this.panelTransaccionesHoy = new System.Windows.Forms.Panel();
            this.lblTransaccionesHoy = new System.Windows.Forms.Label();
            this.lblTransHoy = new System.Windows.Forms.Label();
            this.tabClientes = new System.Windows.Forms.TabPage();
            this.splitContainerClientes = new System.Windows.Forms.SplitContainer();
            this.tablaClientes = new System.Windows.Forms.DataGridView();
            this.panelFormularioCliente = new System.Windows.Forms.Panel();
            this.flowClienteBotones = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNuevoCliente = new System.Windows.Forms.Button();
            this.btnGuardarCliente = new System.Windows.Forms.Button();
            this.btnEliminarCliente = new System.Windows.Forms.Button();
            this.btnActualizarClientes = new System.Windows.Forms.Button();
            this.btnCambiarContrasenaCliente = new System.Windows.Forms.Button();
            this.btnCrearCuentaCliente = new System.Windows.Forms.Button();
            this.btnActualizarSaldoCliente = new System.Windows.Forms.Button();
            this.btnVerCuentasCliente = new System.Windows.Forms.Button();
            this.tabSeguridad = new System.Windows.Forms.TabPage();
            this.splitContainerSeguridad = new System.Windows.Forms.SplitContainer();
            this.panelUsuariosBloqueados = new System.Windows.Forms.Panel();
            this.btnDesbloquearUsuario = new System.Windows.Forms.Button();
            this.gridUsuariosBloqueados = new System.Windows.Forms.DataGridView();
            this.lblUsuariosBloqueadosTitle = new System.Windows.Forms.Label();
            this.panelCrearAdmin = new System.Windows.Forms.Panel();
            this.panelFormAdmin = new System.Windows.Forms.Panel();
            this.btnCrearAdministrador = new System.Windows.Forms.Button();
            this.tableLayoutAdmin = new System.Windows.Forms.TableLayoutPanel();
            this.lblAdminUsername = new System.Windows.Forms.Label();
            this.txtAdminUsername = new System.Windows.Forms.TextBox();
            this.lblAdminPassword = new System.Windows.Forms.Label();
            this.txtAdminPassword = new System.Windows.Forms.TextBox();
            this.lblAdminNombre = new System.Windows.Forms.Label();
            this.txtAdminNombre = new System.Windows.Forms.TextBox();
            this.lblAdminEmail = new System.Windows.Forms.Label();
            this.txtAdminEmail = new System.Windows.Forms.TextBox();
            this.lblCrearAdminTitle = new System.Windows.Forms.Label();
            this.tabReportes = new System.Windows.Forms.TabPage();
            this.panelReportes = new System.Windows.Forms.Panel();
            this.gridReporteTransacciones = new System.Windows.Forms.DataGridView();
            this.panelFiltrosReporte = new System.Windows.Forms.Panel();
            this.btnGenerarReporte = new System.Windows.Forms.Button();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.lblFechaInicio = new System.Windows.Forms.Label();
            this.lblFiltrarPor = new System.Windows.Forms.Label();
            this.tabNotificaciones = new System.Windows.Forms.TabPage();
            this.panelNotificaciones = new System.Windows.Forms.Panel();
            this.gridNotificacionesSistema = new System.Windows.Forms.DataGridView();
            this.panelControlesNotificaciones = new System.Windows.Forms.Panel();
            this.btnEjecutarResumenes = new System.Windows.Forms.Button();
            this.lblNotificacionesTitulo = new System.Windows.Forms.Label();
            this.tabMovimientos = new System.Windows.Forms.TabPage();
            this.tablaMovimientos = new System.Windows.Forms.DataGridView();
            this.tabAnalisis = new System.Windows.Forms.TabPage();
            this.tablaAnalisis = new System.Windows.Forms.DataGridView();
            this.tabResumen = new System.Windows.Forms.TabPage();
            this.tablaResumen = new System.Windows.Forms.DataGridView();
            this.panelSuperior.SuspendLayout();
            this.tabPanelAdmin.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.panelDashboard.SuspendLayout();
            this.panelTransaccionesRecientes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTransaccionesRecientes)).BeginInit();
            this.panelEstadisticas.SuspendLayout();
            this.flowLayoutEstadisticas.SuspendLayout();
            this.panelTotalClientes.SuspendLayout();
            this.panelTotalCuentas.SuspendLayout();
            this.panelSaldoTotal.SuspendLayout();
            this.panelTransaccionesHoy.SuspendLayout();
            this.tabClientes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerClientes)).BeginInit();
            this.splitContainerClientes.Panel1.SuspendLayout();
            this.splitContainerClientes.Panel2.SuspendLayout();
            this.splitContainerClientes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablaClientes)).BeginInit();
            this.tabSeguridad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSeguridad)).BeginInit();
            this.splitContainerSeguridad.Panel1.SuspendLayout();
            this.splitContainerSeguridad.Panel2.SuspendLayout();
            this.splitContainerSeguridad.SuspendLayout();
            this.panelUsuariosBloqueados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUsuariosBloqueados)).BeginInit();
            this.panelCrearAdmin.SuspendLayout();
            this.panelFormAdmin.SuspendLayout();
            this.tableLayoutAdmin.SuspendLayout();
            this.tabReportes.SuspendLayout();
            this.panelReportes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReporteTransacciones)).BeginInit();
            this.panelFiltrosReporte.SuspendLayout();
            this.tabNotificaciones.SuspendLayout();
            this.panelNotificaciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridNotificacionesSistema)).BeginInit();
            this.panelControlesNotificaciones.SuspendLayout();
            this.tabMovimientos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablaMovimientos)).BeginInit();
            this.tabAnalisis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablaAnalisis)).BeginInit();
            this.tabResumen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablaResumen)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSuperior
            // 
            this.panelSuperior.BackColor = System.Drawing.Color.SlateBlue;
            this.panelSuperior.Controls.Add(this.lblTituloSistema);
            this.panelSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperior.Location = new System.Drawing.Point(0, 0);
            this.panelSuperior.Name = "panelSuperior";
            this.panelSuperior.Size = new System.Drawing.Size(1397, 70);
            this.panelSuperior.TabIndex = 2;
            // 
            // lblTituloSistema
            // 
            this.lblTituloSistema.AutoSize = true;
            this.lblTituloSistema.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloSistema.ForeColor = System.Drawing.Color.White;
            this.lblTituloSistema.Location = new System.Drawing.Point(25, 18);
            this.lblTituloSistema.Name = "lblTituloSistema";
            this.lblTituloSistema.Size = new System.Drawing.Size(381, 32);
            this.lblTituloSistema.TabIndex = 0;
            this.lblTituloSistema.Text = "FISCLASE - Panel Administrativo";
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.BackColor = System.Drawing.Color.SlateBlue;
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(90)))), ((int)(((byte)(168)))));
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(1260, 80);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(120, 35);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "Cerrar Sesión";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.BtnLogout_Click);
            // 
            // tabPanelAdmin
            // 
            this.tabPanelAdmin.Controls.Add(this.tabDashboard);
            this.tabPanelAdmin.Controls.Add(this.tabClientes);
            this.tabPanelAdmin.Controls.Add(this.tabSeguridad);
            this.tabPanelAdmin.Controls.Add(this.tabReportes);
            this.tabPanelAdmin.Controls.Add(this.tabNotificaciones);
            this.tabPanelAdmin.Controls.Add(this.tabMovimientos);
            this.tabPanelAdmin.Controls.Add(this.tabAnalisis);
            this.tabPanelAdmin.Controls.Add(this.tabResumen);
            this.tabPanelAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPanelAdmin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tabPanelAdmin.Location = new System.Drawing.Point(0, 0);
            this.tabPanelAdmin.Name = "tabPanelAdmin";
            this.tabPanelAdmin.SelectedIndex = 0;
            this.tabPanelAdmin.Size = new System.Drawing.Size(1397, 575);
            this.tabPanelAdmin.TabIndex = 0;
            // 
            // tabDashboard
            // 
            this.tabDashboard.Controls.Add(this.panelDashboard);
            this.tabDashboard.Location = new System.Drawing.Point(4, 26);
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabDashboard.Size = new System.Drawing.Size(1389, 545);
            this.tabDashboard.TabIndex = 0;
            this.tabDashboard.Text = "Dashboard";
            this.tabDashboard.UseVisualStyleBackColor = true;
            // 
            // panelDashboard
            // 
            this.panelDashboard.Controls.Add(this.panelTransaccionesRecientes);
            this.panelDashboard.Controls.Add(this.panelEstadisticas);
            this.panelDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDashboard.Location = new System.Drawing.Point(3, 3);
            this.panelDashboard.Name = "panelDashboard";
            this.panelDashboard.Size = new System.Drawing.Size(1383, 539);
            this.panelDashboard.TabIndex = 0;
            // 
            // panelTransaccionesRecientes
            // 
            this.panelTransaccionesRecientes.Controls.Add(this.gridTransaccionesRecientes);
            this.panelTransaccionesRecientes.Controls.Add(this.lblTransRecientes);
            this.panelTransaccionesRecientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTransaccionesRecientes.Location = new System.Drawing.Point(0, 150);
            this.panelTransaccionesRecientes.Name = "panelTransaccionesRecientes";
            this.panelTransaccionesRecientes.Size = new System.Drawing.Size(1383, 389);
            this.panelTransaccionesRecientes.TabIndex = 1;
            // 
            // gridTransaccionesRecientes
            // 
            this.gridTransaccionesRecientes.AllowUserToAddRows = false;
            this.gridTransaccionesRecientes.AllowUserToDeleteRows = false;
            this.gridTransaccionesRecientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridTransaccionesRecientes.BackgroundColor = System.Drawing.Color.White;
            this.gridTransaccionesRecientes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridTransaccionesRecientes.ColumnHeadersHeight = 40;
            this.gridTransaccionesRecientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridTransaccionesRecientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTransaccionesRecientes.EnableHeadersVisualStyles = false;
            this.gridTransaccionesRecientes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.gridTransaccionesRecientes.Location = new System.Drawing.Point(0, 45);
            this.gridTransaccionesRecientes.Name = "gridTransaccionesRecientes";
            this.gridTransaccionesRecientes.ReadOnly = true;
            this.gridTransaccionesRecientes.RowHeadersVisible = false;
            this.gridTransaccionesRecientes.RowHeadersWidth = 51;
            this.gridTransaccionesRecientes.RowTemplate.Height = 32;
            this.gridTransaccionesRecientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridTransaccionesRecientes.Size = new System.Drawing.Size(1383, 344);
            this.gridTransaccionesRecientes.TabIndex = 1;
            // 
            // lblTransRecientes
            // 
            this.lblTransRecientes.BackColor = System.Drawing.Color.SlateBlue;
            this.lblTransRecientes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTransRecientes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransRecientes.ForeColor = System.Drawing.Color.White;
            this.lblTransRecientes.Location = new System.Drawing.Point(0, 0);
            this.lblTransRecientes.Name = "lblTransRecientes";
            this.lblTransRecientes.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.lblTransRecientes.Size = new System.Drawing.Size(1383, 45);
            this.lblTransRecientes.TabIndex = 0;
            this.lblTransRecientes.Text = "📊 Transacciones Recientes";
            this.lblTransRecientes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelEstadisticas
            // 
            this.panelEstadisticas.Controls.Add(this.flowLayoutEstadisticas);
            this.panelEstadisticas.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEstadisticas.Location = new System.Drawing.Point(0, 0);
            this.panelEstadisticas.Name = "panelEstadisticas";
            this.panelEstadisticas.Size = new System.Drawing.Size(1383, 150);
            this.panelEstadisticas.TabIndex = 0;
            // 
            // flowLayoutEstadisticas
            // 
            this.flowLayoutEstadisticas.BackColor = System.Drawing.Color.White;
            this.flowLayoutEstadisticas.Controls.Add(this.panelTotalClientes);
            this.flowLayoutEstadisticas.Controls.Add(this.panelTotalCuentas);
            this.flowLayoutEstadisticas.Controls.Add(this.panelSaldoTotal);
            this.flowLayoutEstadisticas.Controls.Add(this.panelTransaccionesHoy);
            this.flowLayoutEstadisticas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutEstadisticas.ForeColor = System.Drawing.SystemColors.ControlText;
            this.flowLayoutEstadisticas.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutEstadisticas.Name = "flowLayoutEstadisticas";
            this.flowLayoutEstadisticas.Size = new System.Drawing.Size(1383, 150);
            this.flowLayoutEstadisticas.TabIndex = 0;
            // 
            // panelTotalClientes
            // 
            this.panelTotalClientes.BackColor = System.Drawing.Color.White;
            this.panelTotalClientes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTotalClientes.Controls.Add(this.lblTotalClientes);
            this.panelTotalClientes.Controls.Add(this.lblTotalCliente);
            this.panelTotalClientes.Location = new System.Drawing.Point(20, 20);
            this.panelTotalClientes.Margin = new System.Windows.Forms.Padding(20, 20, 10, 20);
            this.panelTotalClientes.Name = "panelTotalClientes";
            this.panelTotalClientes.Padding = new System.Windows.Forms.Padding(15);
            this.panelTotalClientes.Size = new System.Drawing.Size(310, 110);
            this.panelTotalClientes.TabIndex = 0;
            // 
            // lblTotalClientes
            // 
            this.lblTotalClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalClientes.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalClientes.ForeColor = System.Drawing.Color.SlateBlue;
            this.lblTotalClientes.Location = new System.Drawing.Point(15, 40);
            this.lblTotalClientes.Name = "lblTotalClientes";
            this.lblTotalClientes.Size = new System.Drawing.Size(278, 53);
            this.lblTotalClientes.TabIndex = 1;
            this.lblTotalClientes.Text = "0";
            this.lblTotalClientes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalCliente
            // 
            this.lblTotalCliente.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTotalCliente.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCliente.ForeColor = System.Drawing.Color.Black;
            this.lblTotalCliente.Location = new System.Drawing.Point(15, 15);
            this.lblTotalCliente.Name = "lblTotalCliente";
            this.lblTotalCliente.Size = new System.Drawing.Size(278, 25);
            this.lblTotalCliente.TabIndex = 0;
            this.lblTotalCliente.Text = "Total Clientes";
            this.lblTotalCliente.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelTotalCuentas
            // 
            this.panelTotalCuentas.BackColor = System.Drawing.Color.White;
            this.panelTotalCuentas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTotalCuentas.Controls.Add(this.lblTotalCuentas);
            this.panelTotalCuentas.Controls.Add(this.lblTotalCuenta);
            this.panelTotalCuentas.Location = new System.Drawing.Point(350, 20);
            this.panelTotalCuentas.Margin = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.panelTotalCuentas.Name = "panelTotalCuentas";
            this.panelTotalCuentas.Padding = new System.Windows.Forms.Padding(15);
            this.panelTotalCuentas.Size = new System.Drawing.Size(310, 110);
            this.panelTotalCuentas.TabIndex = 1;
            // 
            // lblTotalCuentas
            // 
            this.lblTotalCuentas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalCuentas.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCuentas.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.lblTotalCuentas.Location = new System.Drawing.Point(15, 40);
            this.lblTotalCuentas.Name = "lblTotalCuentas";
            this.lblTotalCuentas.Size = new System.Drawing.Size(278, 53);
            this.lblTotalCuentas.TabIndex = 1;
            this.lblTotalCuentas.Text = "0";
            this.lblTotalCuentas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalCuenta
            // 
            this.lblTotalCuenta.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTotalCuenta.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCuenta.ForeColor = System.Drawing.Color.Black;
            this.lblTotalCuenta.Location = new System.Drawing.Point(15, 15);
            this.lblTotalCuenta.Name = "lblTotalCuenta";
            this.lblTotalCuenta.Size = new System.Drawing.Size(278, 25);
            this.lblTotalCuenta.TabIndex = 0;
            this.lblTotalCuenta.Text = "Total Cuentas";
            this.lblTotalCuenta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelSaldoTotal
            // 
            this.panelSaldoTotal.BackColor = System.Drawing.Color.White;
            this.panelSaldoTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSaldoTotal.Controls.Add(this.lblSaldoTotal);
            this.panelSaldoTotal.Controls.Add(this.lblSaldoSis);
            this.panelSaldoTotal.Location = new System.Drawing.Point(680, 20);
            this.panelSaldoTotal.Margin = new System.Windows.Forms.Padding(10, 20, 10, 20);
            this.panelSaldoTotal.Name = "panelSaldoTotal";
            this.panelSaldoTotal.Padding = new System.Windows.Forms.Padding(15);
            this.panelSaldoTotal.Size = new System.Drawing.Size(310, 110);
            this.panelSaldoTotal.TabIndex = 2;
            // 
            // lblSaldoTotal
            // 
            this.lblSaldoTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSaldoTotal.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblSaldoTotal.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblSaldoTotal.Location = new System.Drawing.Point(15, 40);
            this.lblSaldoTotal.Name = "lblSaldoTotal";
            this.lblSaldoTotal.Size = new System.Drawing.Size(278, 53);
            this.lblSaldoTotal.TabIndex = 1;
            this.lblSaldoTotal.Text = "$0";
            this.lblSaldoTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSaldoSis
            // 
            this.lblSaldoSis.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSaldoSis.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaldoSis.ForeColor = System.Drawing.Color.Black;
            this.lblSaldoSis.Location = new System.Drawing.Point(15, 15);
            this.lblSaldoSis.Name = "lblSaldoSis";
            this.lblSaldoSis.Size = new System.Drawing.Size(278, 25);
            this.lblSaldoSis.TabIndex = 0;
            this.lblSaldoSis.Text = "Saldo Total Sistema";
            this.lblSaldoSis.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelTransaccionesHoy
            // 
            this.panelTransaccionesHoy.BackColor = System.Drawing.Color.White;
            this.panelTransaccionesHoy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTransaccionesHoy.Controls.Add(this.lblTransaccionesHoy);
            this.panelTransaccionesHoy.Controls.Add(this.lblTransHoy);
            this.panelTransaccionesHoy.Location = new System.Drawing.Point(1010, 20);
            this.panelTransaccionesHoy.Margin = new System.Windows.Forms.Padding(10, 20, 20, 20);
            this.panelTransaccionesHoy.Name = "panelTransaccionesHoy";
            this.panelTransaccionesHoy.Padding = new System.Windows.Forms.Padding(15);
            this.panelTransaccionesHoy.Size = new System.Drawing.Size(310, 110);
            this.panelTransaccionesHoy.TabIndex = 3;
            // 
            // lblTransaccionesHoy
            // 
            this.lblTransaccionesHoy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTransaccionesHoy.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransaccionesHoy.ForeColor = System.Drawing.Color.Purple;
            this.lblTransaccionesHoy.Location = new System.Drawing.Point(15, 40);
            this.lblTransaccionesHoy.Name = "lblTransaccionesHoy";
            this.lblTransaccionesHoy.Size = new System.Drawing.Size(278, 53);
            this.lblTransaccionesHoy.TabIndex = 1;
            this.lblTransaccionesHoy.Text = "0";
            this.lblTransaccionesHoy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTransHoy
            // 
            this.lblTransHoy.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTransHoy.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransHoy.ForeColor = System.Drawing.Color.Black;
            this.lblTransHoy.Location = new System.Drawing.Point(15, 15);
            this.lblTransHoy.Name = "lblTransHoy";
            this.lblTransHoy.Size = new System.Drawing.Size(278, 25);
            this.lblTransHoy.TabIndex = 0;
            this.lblTransHoy.Text = "Transacciones Hoy";
            this.lblTransHoy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabClientes
            // 
            this.tabClientes.Controls.Add(this.splitContainerClientes);
            this.tabClientes.Location = new System.Drawing.Point(4, 26);
            this.tabClientes.Name = "tabClientes";
            this.tabClientes.Padding = new System.Windows.Forms.Padding(3);
            this.tabClientes.Size = new System.Drawing.Size(1389, 545);
            this.tabClientes.TabIndex = 1;
            this.tabClientes.Text = "Gestión de Clientes";
            this.tabClientes.UseVisualStyleBackColor = true;
            // 
            // splitContainerClientes
            // 
            this.splitContainerClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerClientes.Location = new System.Drawing.Point(3, 3);
            this.splitContainerClientes.Name = "splitContainerClientes";
            // 
            // splitContainerClientes.Panel1
            // 
            this.splitContainerClientes.Panel1.Controls.Add(this.tablaClientes);
            // 
            // splitContainerClientes.Panel2
            // 
            this.splitContainerClientes.Panel2.Controls.Add(this.panelFormularioCliente);
            this.splitContainerClientes.Size = new System.Drawing.Size(1383, 539);
            this.splitContainerClientes.SplitterDistance = 798;
            this.splitContainerClientes.TabIndex = 0;
            // 
            // tablaClientes
            // 
            this.tablaClientes.AllowUserToAddRows = false;
            this.tablaClientes.AllowUserToDeleteRows = false;
            this.tablaClientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tablaClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tablaClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablaClientes.Location = new System.Drawing.Point(0, 0);
            this.tablaClientes.Name = "tablaClientes";
            this.tablaClientes.ReadOnly = true;
            this.tablaClientes.RowHeadersWidth = 51;
            this.tablaClientes.RowTemplate.Height = 24;
            this.tablaClientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tablaClientes.Size = new System.Drawing.Size(798, 539);
            this.tablaClientes.TabIndex = 0;
            // 
            // panelFormularioCliente
            // 
            this.panelFormularioCliente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFormularioCliente.Location = new System.Drawing.Point(0, 0);
            this.panelFormularioCliente.Name = "panelFormularioCliente";
            this.panelFormularioCliente.Size = new System.Drawing.Size(581, 539);
            this.panelFormularioCliente.TabIndex = 0;
            // 
            // flowClienteBotones
            // 
            this.flowClienteBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowClienteBotones.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flowClienteBotones.Location = new System.Drawing.Point(0, 479);
            this.flowClienteBotones.Name = "flowClienteBotones";
            this.flowClienteBotones.Padding = new System.Windows.Forms.Padding(10);
            this.flowClienteBotones.Size = new System.Drawing.Size(581, 60);
            this.flowClienteBotones.TabIndex = 0;
            // 
            // tabSeguridad
            // 
            this.tabSeguridad.Controls.Add(this.splitContainerSeguridad);
            this.tabSeguridad.Location = new System.Drawing.Point(4, 26);
            this.tabSeguridad.Name = "tabSeguridad";
            this.tabSeguridad.Size = new System.Drawing.Size(1389, 545);
            this.tabSeguridad.TabIndex = 2;
            this.tabSeguridad.Text = "Seguridad";
            this.tabSeguridad.UseVisualStyleBackColor = true;
            // 
            // splitContainerSeguridad
            // 
            this.splitContainerSeguridad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerSeguridad.Location = new System.Drawing.Point(0, 0);
            this.splitContainerSeguridad.Name = "splitContainerSeguridad";
            this.splitContainerSeguridad.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerSeguridad.Panel1
            // 
            this.splitContainerSeguridad.Panel1.Controls.Add(this.panelUsuariosBloqueados);
            // 
            // splitContainerSeguridad.Panel2
            // 
            this.splitContainerSeguridad.Panel2.Controls.Add(this.panelCrearAdmin);
            this.splitContainerSeguridad.Size = new System.Drawing.Size(1389, 545);
            this.splitContainerSeguridad.SplitterDistance = 271;
            this.splitContainerSeguridad.TabIndex = 0;
            // 
            // panelUsuariosBloqueados
            // 
            this.panelUsuariosBloqueados.Controls.Add(this.btnDesbloquearUsuario);
            this.panelUsuariosBloqueados.Controls.Add(this.gridUsuariosBloqueados);
            this.panelUsuariosBloqueados.Controls.Add(this.lblUsuariosBloqueadosTitle);
            this.panelUsuariosBloqueados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUsuariosBloqueados.Location = new System.Drawing.Point(0, 0);
            this.panelUsuariosBloqueados.Name = "panelUsuariosBloqueados";
            this.panelUsuariosBloqueados.Size = new System.Drawing.Size(1389, 271);
            this.panelUsuariosBloqueados.TabIndex = 0;
            // 
            // btnDesbloquearUsuario
            // 
            this.btnDesbloquearUsuario.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnDesbloquearUsuario.Location = new System.Drawing.Point(0, 238);
            this.btnDesbloquearUsuario.Name = "btnDesbloquearUsuario";
            this.btnDesbloquearUsuario.Size = new System.Drawing.Size(1389, 33);
            this.btnDesbloquearUsuario.TabIndex = 2;
            this.btnDesbloquearUsuario.Text = "Desbloquear Usuario Seleccionado";
            this.btnDesbloquearUsuario.UseVisualStyleBackColor = true;
            // 
            // gridUsuariosBloqueados
            // 
            this.gridUsuariosBloqueados.AllowUserToAddRows = false;
            this.gridUsuariosBloqueados.AllowUserToDeleteRows = false;
            this.gridUsuariosBloqueados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridUsuariosBloqueados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUsuariosBloqueados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridUsuariosBloqueados.Location = new System.Drawing.Point(0, 30);
            this.gridUsuariosBloqueados.Name = "gridUsuariosBloqueados";
            this.gridUsuariosBloqueados.ReadOnly = true;
            this.gridUsuariosBloqueados.RowHeadersWidth = 51;
            this.gridUsuariosBloqueados.RowTemplate.Height = 24;
            this.gridUsuariosBloqueados.Size = new System.Drawing.Size(1389, 241);
            this.gridUsuariosBloqueados.TabIndex = 1;
            // 
            // lblUsuariosBloqueadosTitle
            // 
            this.lblUsuariosBloqueadosTitle.BackColor = System.Drawing.Color.SlateBlue;
            this.lblUsuariosBloqueadosTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblUsuariosBloqueadosTitle.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuariosBloqueadosTitle.ForeColor = System.Drawing.Color.White;
            this.lblUsuariosBloqueadosTitle.Location = new System.Drawing.Point(0, 0);
            this.lblUsuariosBloqueadosTitle.Name = "lblUsuariosBloqueadosTitle";
            this.lblUsuariosBloqueadosTitle.Size = new System.Drawing.Size(1389, 30);
            this.lblUsuariosBloqueadosTitle.TabIndex = 0;
            this.lblUsuariosBloqueadosTitle.Text = "Usuarios Bloqueados";
            this.lblUsuariosBloqueadosTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelCrearAdmin
            // 
            this.panelCrearAdmin.Controls.Add(this.panelFormAdmin);
            this.panelCrearAdmin.Controls.Add(this.lblCrearAdminTitle);
            this.panelCrearAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCrearAdmin.Location = new System.Drawing.Point(0, 0);
            this.panelCrearAdmin.Name = "panelCrearAdmin";
            this.panelCrearAdmin.Size = new System.Drawing.Size(1389, 270);
            this.panelCrearAdmin.TabIndex = 0;
            // 
            // panelFormAdmin
            // 
            this.panelFormAdmin.Controls.Add(this.btnCrearAdministrador);
            this.panelFormAdmin.Controls.Add(this.tableLayoutAdmin);
            this.panelFormAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFormAdmin.Location = new System.Drawing.Point(0, 30);
            this.panelFormAdmin.Name = "panelFormAdmin";
            this.panelFormAdmin.Padding = new System.Windows.Forms.Padding(10);
            this.panelFormAdmin.Size = new System.Drawing.Size(1389, 240);
            this.panelFormAdmin.TabIndex = 1;
            // 
            // btnCrearAdministrador
            // 
            this.btnCrearAdministrador.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCrearAdministrador.Location = new System.Drawing.Point(10, 197);
            this.btnCrearAdministrador.Name = "btnCrearAdministrador";
            this.btnCrearAdministrador.Size = new System.Drawing.Size(1369, 33);
            this.btnCrearAdministrador.TabIndex = 1;
            this.btnCrearAdministrador.Text = "Crear Administrador";
            this.btnCrearAdministrador.UseVisualStyleBackColor = true;
            // 
            // tableLayoutAdmin
            // 
            this.tableLayoutAdmin.ColumnCount = 2;
            this.tableLayoutAdmin.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutAdmin.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutAdmin.Controls.Add(this.lblAdminUsername, 0, 0);
            this.tableLayoutAdmin.Controls.Add(this.txtAdminUsername, 1, 0);
            this.tableLayoutAdmin.Controls.Add(this.lblAdminPassword, 0, 1);
            this.tableLayoutAdmin.Controls.Add(this.txtAdminPassword, 1, 1);
            this.tableLayoutAdmin.Controls.Add(this.lblAdminNombre, 0, 2);
            this.tableLayoutAdmin.Controls.Add(this.txtAdminNombre, 1, 2);
            this.tableLayoutAdmin.Controls.Add(this.lblAdminEmail, 0, 3);
            this.tableLayoutAdmin.Controls.Add(this.txtAdminEmail, 1, 3);
            this.tableLayoutAdmin.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutAdmin.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutAdmin.Name = "tableLayoutAdmin";
            this.tableLayoutAdmin.RowCount = 4;
            this.tableLayoutAdmin.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAdmin.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAdmin.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAdmin.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAdmin.Size = new System.Drawing.Size(1369, 200);
            this.tableLayoutAdmin.TabIndex = 0;
            // 
            // lblAdminUsername
            // 
            this.lblAdminUsername.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAdminUsername.AutoSize = true;
            this.lblAdminUsername.Location = new System.Drawing.Point(3, 15);
            this.lblAdminUsername.Name = "lblAdminUsername";
            this.lblAdminUsername.Size = new System.Drawing.Size(74, 19);
            this.lblAdminUsername.TabIndex = 0;
            this.lblAdminUsername.Text = "Username:";
            // 
            // txtAdminUsername
            // 
            this.txtAdminUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdminUsername.Location = new System.Drawing.Point(413, 12);
            this.txtAdminUsername.Name = "txtAdminUsername";
            this.txtAdminUsername.Size = new System.Drawing.Size(953, 25);
            this.txtAdminUsername.TabIndex = 1;
            // 
            // lblAdminPassword
            // 
            this.lblAdminPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAdminPassword.AutoSize = true;
            this.lblAdminPassword.Location = new System.Drawing.Point(3, 65);
            this.lblAdminPassword.Name = "lblAdminPassword";
            this.lblAdminPassword.Size = new System.Drawing.Size(70, 19);
            this.lblAdminPassword.TabIndex = 2;
            this.lblAdminPassword.Text = "Password:";
            // 
            // txtAdminPassword
            // 
            this.txtAdminPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdminPassword.Location = new System.Drawing.Point(413, 62);
            this.txtAdminPassword.Name = "txtAdminPassword";
            this.txtAdminPassword.PasswordChar = '*';
            this.txtAdminPassword.Size = new System.Drawing.Size(953, 25);
            this.txtAdminPassword.TabIndex = 3;
            // 
            // lblAdminNombre
            // 
            this.lblAdminNombre.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAdminNombre.AutoSize = true;
            this.lblAdminNombre.Location = new System.Drawing.Point(3, 115);
            this.lblAdminNombre.Name = "lblAdminNombre";
            this.lblAdminNombre.Size = new System.Drawing.Size(62, 19);
            this.lblAdminNombre.TabIndex = 4;
            this.lblAdminNombre.Text = "Nombre:";
            // 
            // txtAdminNombre
            // 
            this.txtAdminNombre.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdminNombre.Location = new System.Drawing.Point(413, 112);
            this.txtAdminNombre.Name = "txtAdminNombre";
            this.txtAdminNombre.Size = new System.Drawing.Size(953, 25);
            this.txtAdminNombre.TabIndex = 5;
            // 
            // lblAdminEmail
            // 
            this.lblAdminEmail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAdminEmail.AutoSize = true;
            this.lblAdminEmail.Location = new System.Drawing.Point(3, 165);
            this.lblAdminEmail.Name = "lblAdminEmail";
            this.lblAdminEmail.Size = new System.Drawing.Size(44, 19);
            this.lblAdminEmail.TabIndex = 6;
            this.lblAdminEmail.Text = "Email:";
            // 
            // txtAdminEmail
            // 
            this.txtAdminEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdminEmail.Location = new System.Drawing.Point(413, 162);
            this.txtAdminEmail.Name = "txtAdminEmail";
            this.txtAdminEmail.Size = new System.Drawing.Size(953, 25);
            this.txtAdminEmail.TabIndex = 7;
            // 
            // lblCrearAdminTitle
            // 
            this.lblCrearAdminTitle.BackColor = System.Drawing.Color.SlateBlue;
            this.lblCrearAdminTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCrearAdminTitle.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCrearAdminTitle.ForeColor = System.Drawing.Color.White;
            this.lblCrearAdminTitle.Location = new System.Drawing.Point(0, 0);
            this.lblCrearAdminTitle.Name = "lblCrearAdminTitle";
            this.lblCrearAdminTitle.Size = new System.Drawing.Size(1389, 30);
            this.lblCrearAdminTitle.TabIndex = 0;
            this.lblCrearAdminTitle.Text = "Crear Nuevo Administrador";
            this.lblCrearAdminTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabReportes
            // 
            this.tabReportes.Controls.Add(this.panelReportes);
            this.tabReportes.Location = new System.Drawing.Point(4, 26);
            this.tabReportes.Name = "tabReportes";
            this.tabReportes.Size = new System.Drawing.Size(1389, 545);
            this.tabReportes.TabIndex = 3;
            this.tabReportes.Text = "Reportes Avanzados";
            this.tabReportes.UseVisualStyleBackColor = true;
            // 
            // panelReportes
            // 
            this.panelReportes.Controls.Add(this.gridReporteTransacciones);
            this.panelReportes.Controls.Add(this.panelFiltrosReporte);
            this.panelReportes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelReportes.Location = new System.Drawing.Point(0, 0);
            this.panelReportes.Name = "panelReportes";
            this.panelReportes.Size = new System.Drawing.Size(1389, 545);
            this.panelReportes.TabIndex = 0;
            // 
            // gridReporteTransacciones
            // 
            this.gridReporteTransacciones.AllowUserToAddRows = false;
            this.gridReporteTransacciones.AllowUserToDeleteRows = false;
            this.gridReporteTransacciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridReporteTransacciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridReporteTransacciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridReporteTransacciones.Location = new System.Drawing.Point(0, 80);
            this.gridReporteTransacciones.Name = "gridReporteTransacciones";
            this.gridReporteTransacciones.ReadOnly = true;
            this.gridReporteTransacciones.RowHeadersWidth = 51;
            this.gridReporteTransacciones.RowTemplate.Height = 24;
            this.gridReporteTransacciones.Size = new System.Drawing.Size(1389, 465);
            this.gridReporteTransacciones.TabIndex = 1;
            // 
            // panelFiltrosReporte
            // 
            this.panelFiltrosReporte.BackColor = System.Drawing.Color.SlateBlue;
            this.panelFiltrosReporte.Controls.Add(this.btnGenerarReporte);
            this.panelFiltrosReporte.Controls.Add(this.dtpFechaFin);
            this.panelFiltrosReporte.Controls.Add(this.lblFechaFin);
            this.panelFiltrosReporte.Controls.Add(this.dtpFechaInicio);
            this.panelFiltrosReporte.Controls.Add(this.lblFechaInicio);
            this.panelFiltrosReporte.Controls.Add(this.lblFiltrarPor);
            this.panelFiltrosReporte.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltrosReporte.Location = new System.Drawing.Point(0, 0);
            this.panelFiltrosReporte.Name = "panelFiltrosReporte";
            this.panelFiltrosReporte.Size = new System.Drawing.Size(1389, 80);
            this.panelFiltrosReporte.TabIndex = 0;
            // 
            // btnGenerarReporte
            // 
            this.btnGenerarReporte.Location = new System.Drawing.Point(800, 25);
            this.btnGenerarReporte.Name = "btnGenerarReporte";
            this.btnGenerarReporte.Size = new System.Drawing.Size(150, 35);
            this.btnGenerarReporte.TabIndex = 5;
            this.btnGenerarReporte.Text = "Generar Reporte";
            this.btnGenerarReporte.UseVisualStyleBackColor = true;
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.Location = new System.Drawing.Point(550, 28);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(200, 25);
            this.dtpFechaFin.TabIndex = 4;
            // 
            // lblFechaFin
            // 
            this.lblFechaFin.AutoSize = true;
            this.lblFechaFin.ForeColor = System.Drawing.Color.White;
            this.lblFechaFin.Location = new System.Drawing.Point(470, 32);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(69, 19);
            this.lblFechaFin.TabIndex = 3;
            this.lblFechaFin.Text = "Fecha Fin:";
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.Location = new System.Drawing.Point(220, 28);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(200, 25);
            this.dtpFechaInicio.TabIndex = 2;
            // 
            // lblFechaInicio
            // 
            this.lblFechaInicio.AutoSize = true;
            this.lblFechaInicio.ForeColor = System.Drawing.Color.White;
            this.lblFechaInicio.Location = new System.Drawing.Point(120, 32);
            this.lblFechaInicio.Name = "lblFechaInicio";
            this.lblFechaInicio.Size = new System.Drawing.Size(83, 19);
            this.lblFechaInicio.TabIndex = 1;
            this.lblFechaInicio.Text = "Fecha Inicio:";
            // 
            // lblFiltrarPor
            // 
            this.lblFiltrarPor.AutoSize = true;
            this.lblFiltrarPor.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFiltrarPor.ForeColor = System.Drawing.Color.White;
            this.lblFiltrarPor.Location = new System.Drawing.Point(20, 30);
            this.lblFiltrarPor.Name = "lblFiltrarPor";
            this.lblFiltrarPor.Size = new System.Drawing.Size(81, 16);
            this.lblFiltrarPor.TabIndex = 0;
            this.lblFiltrarPor.Text = "Filtrar por:";
            // 
            // tabNotificaciones
            // 
            this.tabNotificaciones.Controls.Add(this.panelNotificaciones);
            this.tabNotificaciones.Location = new System.Drawing.Point(4, 26);
            this.tabNotificaciones.Name = "tabNotificaciones";
            this.tabNotificaciones.Size = new System.Drawing.Size(1389, 545);
            this.tabNotificaciones.TabIndex = 4;
            this.tabNotificaciones.Text = "Notificaciones Sistema";
            this.tabNotificaciones.UseVisualStyleBackColor = true;
            // 
            // panelNotificaciones
            // 
            this.panelNotificaciones.Controls.Add(this.gridNotificacionesSistema);
            this.panelNotificaciones.Controls.Add(this.panelControlesNotificaciones);
            this.panelNotificaciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelNotificaciones.Location = new System.Drawing.Point(0, 0);
            this.panelNotificaciones.Name = "panelNotificaciones";
            this.panelNotificaciones.Size = new System.Drawing.Size(1389, 545);
            this.panelNotificaciones.TabIndex = 0;
            // 
            // gridNotificacionesSistema
            // 
            this.gridNotificacionesSistema.AllowUserToAddRows = false;
            this.gridNotificacionesSistema.AllowUserToDeleteRows = false;
            this.gridNotificacionesSistema.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridNotificacionesSistema.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridNotificacionesSistema.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridNotificacionesSistema.Location = new System.Drawing.Point(0, 80);
            this.gridNotificacionesSistema.Name = "gridNotificacionesSistema";
            this.gridNotificacionesSistema.ReadOnly = true;
            this.gridNotificacionesSistema.RowHeadersWidth = 51;
            this.gridNotificacionesSistema.RowTemplate.Height = 24;
            this.gridNotificacionesSistema.Size = new System.Drawing.Size(1389, 465);
            this.gridNotificacionesSistema.TabIndex = 1;
            // 
            // panelControlesNotificaciones
            // 
            this.panelControlesNotificaciones.BackColor = System.Drawing.Color.SlateBlue;
            this.panelControlesNotificaciones.Controls.Add(this.btnEjecutarResumenes);
            this.panelControlesNotificaciones.Controls.Add(this.lblNotificacionesTitulo);
            this.panelControlesNotificaciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControlesNotificaciones.Location = new System.Drawing.Point(0, 0);
            this.panelControlesNotificaciones.Name = "panelControlesNotificaciones";
            this.panelControlesNotificaciones.Size = new System.Drawing.Size(1389, 80);
            this.panelControlesNotificaciones.TabIndex = 0;
            // 
            // btnEjecutarResumenes
            // 
            this.btnEjecutarResumenes.Location = new System.Drawing.Point(20, 25);
            this.btnEjecutarResumenes.Name = "btnEjecutarResumenes";
            this.btnEjecutarResumenes.Size = new System.Drawing.Size(300, 35);
            this.btnEjecutarResumenes.TabIndex = 1;
            this.btnEjecutarResumenes.Text = "Ejecutar Resúmenes Diarios Manualmente";
            this.btnEjecutarResumenes.UseVisualStyleBackColor = true;
            // 
            // lblNotificacionesTitulo
            // 
            this.lblNotificacionesTitulo.AutoSize = true;
            this.lblNotificacionesTitulo.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotificacionesTitulo.ForeColor = System.Drawing.Color.White;
            this.lblNotificacionesTitulo.Location = new System.Drawing.Point(400, 32);
            this.lblNotificacionesTitulo.Name = "lblNotificacionesTitulo";
            this.lblNotificacionesTitulo.Size = new System.Drawing.Size(192, 16);
            this.lblNotificacionesTitulo.TabIndex = 0;
            this.lblNotificacionesTitulo.Text = "Notificaciones del Sistema";
            // 
            // tabMovimientos
            // 
            this.tabMovimientos.Controls.Add(this.tablaMovimientos);
            this.tabMovimientos.Location = new System.Drawing.Point(4, 26);
            this.tabMovimientos.Name = "tabMovimientos";
            this.tabMovimientos.Size = new System.Drawing.Size(1389, 545);
            this.tabMovimientos.TabIndex = 5;
            this.tabMovimientos.Text = "Movimientos del Día";
            this.tabMovimientos.UseVisualStyleBackColor = true;
            // 
            // tablaMovimientos
            // 
            this.tablaMovimientos.AllowUserToAddRows = false;
            this.tablaMovimientos.AllowUserToDeleteRows = false;
            this.tablaMovimientos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tablaMovimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tablaMovimientos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablaMovimientos.Location = new System.Drawing.Point(0, 0);
            this.tablaMovimientos.Name = "tablaMovimientos";
            this.tablaMovimientos.ReadOnly = true;
            this.tablaMovimientos.RowHeadersWidth = 51;
            this.tablaMovimientos.RowTemplate.Height = 24;
            this.tablaMovimientos.Size = new System.Drawing.Size(1389, 545);
            this.tablaMovimientos.TabIndex = 0;
            // 
            // tabAnalisis
            // 
            this.tabAnalisis.Controls.Add(this.tablaAnalisis);
            this.tabAnalisis.Location = new System.Drawing.Point(4, 26);
            this.tabAnalisis.Name = "tabAnalisis";
            this.tabAnalisis.Size = new System.Drawing.Size(1389, 545);
            this.tabAnalisis.TabIndex = 6;
            this.tabAnalisis.Text = "Análisis de Transacciones";
            this.tabAnalisis.UseVisualStyleBackColor = true;
            // 
            // tablaAnalisis
            // 
            this.tablaAnalisis.AllowUserToAddRows = false;
            this.tablaAnalisis.AllowUserToDeleteRows = false;
            this.tablaAnalisis.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tablaAnalisis.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tablaAnalisis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablaAnalisis.Location = new System.Drawing.Point(0, 0);
            this.tablaAnalisis.Name = "tablaAnalisis";
            this.tablaAnalisis.ReadOnly = true;
            this.tablaAnalisis.RowHeadersWidth = 51;
            this.tablaAnalisis.RowTemplate.Height = 24;
            this.tablaAnalisis.Size = new System.Drawing.Size(1389, 545);
            this.tablaAnalisis.TabIndex = 0;
            // 
            // tabResumen
            // 
            this.tabResumen.Controls.Add(this.tablaResumen);
            this.tabResumen.Location = new System.Drawing.Point(4, 26);
            this.tabResumen.Name = "tabResumen";
            this.tabResumen.Size = new System.Drawing.Size(1389, 545);
            this.tabResumen.TabIndex = 7;
            this.tabResumen.Text = "Resumen de Clientes";
            this.tabResumen.UseVisualStyleBackColor = true;
            // 
            // tablaResumen
            // 
            this.tablaResumen.AllowUserToAddRows = false;
            this.tablaResumen.AllowUserToDeleteRows = false;
            this.tablaResumen.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tablaResumen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tablaResumen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablaResumen.Location = new System.Drawing.Point(0, 0);
            this.tablaResumen.Name = "tablaResumen";
            this.tablaResumen.ReadOnly = true;
            this.tablaResumen.RowHeadersWidth = 51;
            this.tablaResumen.RowTemplate.Height = 24;
            this.tablaResumen.Size = new System.Drawing.Size(1389, 545);
            this.tablaResumen.TabIndex = 0;
            // 
            // PanelAdministrativo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1397, 575);
            this.Controls.Add(this.tabPanelAdmin);
            this.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PanelAdministrativo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Banco Fisclase 24/7 - Panel de Administración Avanzado";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelSuperior.ResumeLayout(false);
            this.panelSuperior.PerformLayout();
            this.tabPanelAdmin.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.panelDashboard.ResumeLayout(false);
            this.panelTransaccionesRecientes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTransaccionesRecientes)).EndInit();
            this.panelEstadisticas.ResumeLayout(false);
            this.flowLayoutEstadisticas.ResumeLayout(false);
            this.panelTotalClientes.ResumeLayout(false);
            this.panelTotalCuentas.ResumeLayout(false);
            this.panelSaldoTotal.ResumeLayout(false);
            this.panelTransaccionesHoy.ResumeLayout(false);
            this.tabClientes.ResumeLayout(false);
            this.splitContainerClientes.Panel1.ResumeLayout(false);
            this.splitContainerClientes.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerClientes)).EndInit();
            this.splitContainerClientes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tablaClientes)).EndInit();
            this.tabSeguridad.ResumeLayout(false);
            this.splitContainerSeguridad.Panel1.ResumeLayout(false);
            this.splitContainerSeguridad.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSeguridad)).EndInit();
            this.splitContainerSeguridad.ResumeLayout(false);
            this.panelUsuariosBloqueados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridUsuariosBloqueados)).EndInit();
            this.panelCrearAdmin.ResumeLayout(false);
            this.panelFormAdmin.ResumeLayout(false);
            this.tableLayoutAdmin.ResumeLayout(false);
            this.tableLayoutAdmin.PerformLayout();
            this.tabReportes.ResumeLayout(false);
            this.panelReportes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridReporteTransacciones)).EndInit();
            this.panelFiltrosReporte.ResumeLayout(false);
            this.panelFiltrosReporte.PerformLayout();
            this.tabNotificaciones.ResumeLayout(false);
            this.panelNotificaciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridNotificacionesSistema)).EndInit();
            this.panelControlesNotificaciones.ResumeLayout(false);
            this.panelControlesNotificaciones.PerformLayout();
            this.tabMovimientos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tablaMovimientos)).EndInit();
            this.tabAnalisis.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tablaAnalisis)).EndInit();
            this.tabResumen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tablaResumen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelSuperior;
        private Label lblTituloSistema;

        private TabControl tabPanelAdmin;
        private TabPage tabDashboard;
        private Panel panelDashboard;
        private Panel panelTransaccionesRecientes;
        private DataGridView gridTransaccionesRecientes;
        private Label lblTransRecientes;
        private Panel panelEstadisticas;
        private FlowLayoutPanel flowLayoutEstadisticas;
        private Panel panelTotalClientes;
        private Label lblTotalClientes;
        private Label lblTotalCliente;
        private Panel panelTotalCuentas;
        private Label lblTotalCuentas;
        private Label lblTotalCuenta;
        private Panel panelSaldoTotal;
        private Label lblSaldoTotal;
        private Label lblSaldoSis;
        private Panel panelTransaccionesHoy;
        private Label lblTransaccionesHoy;
        private Label lblTransHoy;
        private TabPage tabClientes;
        private SplitContainer splitContainerClientes;
        private DataGridView tablaClientes;
        private Panel panelFormularioCliente;
        private FlowLayoutPanel flowClienteBotones;
        private Button btnNuevoCliente;
        private Button btnGuardarCliente;
        private Button btnEliminarCliente;
        private Button btnActualizarClientes;
        private Button btnCambiarContrasenaCliente;
        private Button btnCrearCuentaCliente;
        private Button btnActualizarSaldoCliente;
        private Button btnVerCuentasCliente;
        private TabPage tabSeguridad;
        private SplitContainer splitContainerSeguridad;
        private Panel panelUsuariosBloqueados;
        private Button btnDesbloquearUsuario;
        private DataGridView gridUsuariosBloqueados;
        private Label lblUsuariosBloqueadosTitle;
        private Panel panelCrearAdmin;
        private Panel panelFormAdmin;
        private Button btnCrearAdministrador;
        private TableLayoutPanel tableLayoutAdmin;
        private Label lblAdminUsername;
        private TextBox txtAdminUsername;
        private Label lblAdminPassword;
        private TextBox txtAdminPassword;
        private Label lblAdminNombre;
        private TextBox txtAdminNombre;
        private Label lblAdminEmail;
        private TextBox txtAdminEmail;
        private Label lblCrearAdminTitle;
        private TabPage tabReportes;
        private Panel panelReportes;
        private DataGridView gridReporteTransacciones;
        private Panel panelFiltrosReporte;
        private Button btnGenerarReporte;
        private DateTimePicker dtpFechaFin;
        private Label lblFechaFin;
        private DateTimePicker dtpFechaInicio;
        private Label lblFechaInicio;
        private Label lblFiltrarPor;
        private TabPage tabNotificaciones;
        private Panel panelNotificaciones;
        private DataGridView gridNotificacionesSistema;
        private Panel panelControlesNotificaciones;
        private Button btnEjecutarResumenes;
        private Label lblNotificacionesTitulo;
        private TabPage tabMovimientos;
        private DataGridView tablaMovimientos;
        private TabPage tabAnalisis;
        private DataGridView tablaAnalisis;
        private TabPage tabResumen;
        private DataGridView tablaResumen;
        private Button btnLogout;
    }
}