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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelAdministrativo));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabDashboard = new System.Windows.Forms.TabPage();
            this.panelDashboard = new System.Windows.Forms.Panel();
            this.panelTransaccionesRecientes = new System.Windows.Forms.Panel();
            this.gridTransaccionesRecientes = new System.Windows.Forms.DataGridView();
            this.label11 = new System.Windows.Forms.Label();
            this.panelEstadisticas = new System.Windows.Forms.Panel();
            this.flowLayoutEstadisticas = new System.Windows.Forms.FlowLayoutPanel();
            this.panelTotalClientes = new System.Windows.Forms.Panel();
            this.lblTotalClientes = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelTotalCuentas = new System.Windows.Forms.Panel();
            this.lblTotalCuentas = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelSaldoTotal = new System.Windows.Forms.Panel();
            this.lblSaldoTotal = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panelTransaccionesHoy = new System.Windows.Forms.Panel();
            this.lblTransaccionesHoy = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tabClientes = new System.Windows.Forms.TabPage();
            this.splitContainerClientes = new System.Windows.Forms.SplitContainer();
            this.tablaClientes = new System.Windows.Forms.DataGridView();
            this.panelFormularioCliente = new System.Windows.Forms.Panel();
            this.tabSeguridad = new System.Windows.Forms.TabPage();
            this.splitContainerSeguridad = new System.Windows.Forms.SplitContainer();
            this.panelUsuariosBloqueados = new System.Windows.Forms.Panel();
            this.btnDesbloquearUsuario = new System.Windows.Forms.Button();
            this.gridUsuariosBloqueados = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.panelCrearAdmin = new System.Windows.Forms.Panel();
            this.panelFormAdmin = new System.Windows.Forms.Panel();
            this.btnCrearAdministrador = new System.Windows.Forms.Button();
            this.tableLayoutAdmin = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.txtAdminUsername = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAdminPassword = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtAdminNombre = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtAdminEmail = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tabReportes = new System.Windows.Forms.TabPage();
            this.panelReportes = new System.Windows.Forms.Panel();
            this.gridReporteTransacciones = new System.Windows.Forms.DataGridView();
            this.panelFiltrosReporte = new System.Windows.Forms.Panel();
            this.btnGenerarReporte = new System.Windows.Forms.Button();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabNotificaciones = new System.Windows.Forms.TabPage();
            this.panelNotificaciones = new System.Windows.Forms.Panel();
            this.gridNotificacionesSistema = new System.Windows.Forms.DataGridView();
            this.panelControlesNotificaciones = new System.Windows.Forms.Panel();
            this.btnEjecutarResumenes = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.tabMovimientos = new System.Windows.Forms.TabPage();
            this.tablaMovimientos = new System.Windows.Forms.DataGridView();
            this.tabAnalisis = new System.Windows.Forms.TabPage();
            this.tablaAnalisis = new System.Windows.Forms.DataGridView();
            this.tabResumen = new System.Windows.Forms.TabPage();
            this.tablaResumen = new System.Windows.Forms.DataGridView();
            this.tabControl.SuspendLayout();
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
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabDashboard);
            this.tabControl.Controls.Add(this.tabClientes);
            this.tabControl.Controls.Add(this.tabSeguridad);
            this.tabControl.Controls.Add(this.tabReportes);
            this.tabControl.Controls.Add(this.tabNotificaciones);
            this.tabControl.Controls.Add(this.tabMovimientos);
            this.tabControl.Controls.Add(this.tabAnalisis);
            this.tabControl.Controls.Add(this.tabResumen);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1400, 800);
            this.tabControl.TabIndex = 0;
            // 
            // tabDashboard
            // 
            this.tabDashboard.Controls.Add(this.panelDashboard);
            this.tabDashboard.Location = new System.Drawing.Point(4, 29);
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabDashboard.Size = new System.Drawing.Size(1392, 767);
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
            this.panelDashboard.Size = new System.Drawing.Size(1386, 761);
            this.panelDashboard.TabIndex = 0;
            // 
            // panelTransaccionesRecientes
            // 
            this.panelTransaccionesRecientes.Controls.Add(this.gridTransaccionesRecientes);
            this.panelTransaccionesRecientes.Controls.Add(this.label11);
            this.panelTransaccionesRecientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTransaccionesRecientes.Location = new System.Drawing.Point(0, 150);
            this.panelTransaccionesRecientes.Name = "panelTransaccionesRecientes";
            this.panelTransaccionesRecientes.Size = new System.Drawing.Size(1386, 611);
            this.panelTransaccionesRecientes.TabIndex = 1;
            // 
            // gridTransaccionesRecientes
            // 
            this.gridTransaccionesRecientes.AllowUserToAddRows = false;
            this.gridTransaccionesRecientes.AllowUserToDeleteRows = false;
            this.gridTransaccionesRecientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridTransaccionesRecientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTransaccionesRecientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTransaccionesRecientes.Location = new System.Drawing.Point(0, 30);
            this.gridTransaccionesRecientes.Name = "gridTransaccionesRecientes";
            this.gridTransaccionesRecientes.ReadOnly = true;
            this.gridTransaccionesRecientes.RowHeadersWidth = 51;
            this.gridTransaccionesRecientes.RowTemplate.Height = 24;
            this.gridTransaccionesRecientes.Size = new System.Drawing.Size(1386, 581);
            this.gridTransaccionesRecientes.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Top;
            this.label11.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(0, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(1386, 30);
            this.label11.TabIndex = 0;
            this.label11.Text = "Transacciones Recientes";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelEstadisticas
            // 
            this.panelEstadisticas.Controls.Add(this.flowLayoutEstadisticas);
            this.panelEstadisticas.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEstadisticas.Location = new System.Drawing.Point(0, 0);
            this.panelEstadisticas.Name = "panelEstadisticas";
            this.panelEstadisticas.Size = new System.Drawing.Size(1386, 150);
            this.panelEstadisticas.TabIndex = 0;
            // 
            // flowLayoutEstadisticas
            // 
            this.flowLayoutEstadisticas.Controls.Add(this.panelTotalClientes);
            this.flowLayoutEstadisticas.Controls.Add(this.panelTotalCuentas);
            this.flowLayoutEstadisticas.Controls.Add(this.panelSaldoTotal);
            this.flowLayoutEstadisticas.Controls.Add(this.panelTransaccionesHoy);
            this.flowLayoutEstadisticas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutEstadisticas.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutEstadisticas.Name = "flowLayoutEstadisticas";
            this.flowLayoutEstadisticas.Size = new System.Drawing.Size(1386, 150);
            this.flowLayoutEstadisticas.TabIndex = 0;
            // 
            // panelTotalClientes
            // 
            this.panelTotalClientes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTotalClientes.Controls.Add(this.lblTotalClientes);
            this.panelTotalClientes.Controls.Add(this.label2);
            this.panelTotalClientes.Location = new System.Drawing.Point(20, 20);
            this.panelTotalClientes.Margin = new System.Windows.Forms.Padding(20);
            this.panelTotalClientes.Name = "panelTotalClientes";
            this.panelTotalClientes.Size = new System.Drawing.Size(300, 110);
            this.panelTotalClientes.TabIndex = 0;
            // 
            // lblTotalClientes
            // 
            this.lblTotalClientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalClientes.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalClientes.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalClientes.Location = new System.Drawing.Point(0, 30);
            this.lblTotalClientes.Name = "lblTotalClientes";
            this.lblTotalClientes.Size = new System.Drawing.Size(298, 78);
            this.lblTotalClientes.TabIndex = 1;
            this.lblTotalClientes.Text = "0";
            this.lblTotalClientes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(298, 30);
            this.label2.TabIndex = 0;
            this.label2.Text = "Total Clientes";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelTotalCuentas
            // 
            this.panelTotalCuentas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTotalCuentas.Controls.Add(this.lblTotalCuentas);
            this.panelTotalCuentas.Controls.Add(this.label4);
            this.panelTotalCuentas.Location = new System.Drawing.Point(360, 20);
            this.panelTotalCuentas.Margin = new System.Windows.Forms.Padding(20);
            this.panelTotalCuentas.Name = "panelTotalCuentas";
            this.panelTotalCuentas.Size = new System.Drawing.Size(300, 110);
            this.panelTotalCuentas.TabIndex = 1;
            // 
            // lblTotalCuentas
            // 
            this.lblTotalCuentas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalCuentas.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCuentas.ForeColor = System.Drawing.Color.Green;
            this.lblTotalCuentas.Location = new System.Drawing.Point(0, 30);
            this.lblTotalCuentas.Name = "lblTotalCuentas";
            this.lblTotalCuentas.Size = new System.Drawing.Size(298, 78);
            this.lblTotalCuentas.TabIndex = 1;
            this.lblTotalCuentas.Text = "0";
            this.lblTotalCuentas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(298, 30);
            this.label4.TabIndex = 0;
            this.label4.Text = "Total Cuentas";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelSaldoTotal
            // 
            this.panelSaldoTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSaldoTotal.Controls.Add(this.lblSaldoTotal);
            this.panelSaldoTotal.Controls.Add(this.label6);
            this.panelSaldoTotal.Location = new System.Drawing.Point(700, 20);
            this.panelSaldoTotal.Margin = new System.Windows.Forms.Padding(20);
            this.panelSaldoTotal.Name = "panelSaldoTotal";
            this.panelSaldoTotal.Size = new System.Drawing.Size(300, 110);
            this.panelSaldoTotal.TabIndex = 2;
            // 
            // lblSaldoTotal
            // 
            this.lblSaldoTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSaldoTotal.Font = new System.Drawing.Font("Arial", 14.2F, System.Drawing.FontStyle.Bold);
            this.lblSaldoTotal.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblSaldoTotal.Location = new System.Drawing.Point(0, 30);
            this.lblSaldoTotal.Name = "lblSaldoTotal";
            this.lblSaldoTotal.Size = new System.Drawing.Size(298, 78);
            this.lblSaldoTotal.TabIndex = 1;
            this.lblSaldoTotal.Text = "$0";
            this.lblSaldoTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(298, 30);
            this.label6.TabIndex = 0;
            this.label6.Text = "Saldo Total Sistema";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelTransaccionesHoy
            // 
            this.panelTransaccionesHoy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTransaccionesHoy.Controls.Add(this.lblTransaccionesHoy);
            this.panelTransaccionesHoy.Controls.Add(this.label8);
            this.panelTransaccionesHoy.Location = new System.Drawing.Point(1040, 20);
            this.panelTransaccionesHoy.Margin = new System.Windows.Forms.Padding(20);
            this.panelTransaccionesHoy.Name = "panelTransaccionesHoy";
            this.panelTransaccionesHoy.Size = new System.Drawing.Size(300, 110);
            this.panelTransaccionesHoy.TabIndex = 3;
            // 
            // lblTransaccionesHoy
            // 
            this.lblTransaccionesHoy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTransaccionesHoy.Font = new System.Drawing.Font("Arial", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransaccionesHoy.ForeColor = System.Drawing.Color.Purple;
            this.lblTransaccionesHoy.Location = new System.Drawing.Point(0, 30);
            this.lblTransaccionesHoy.Name = "lblTransaccionesHoy";
            this.lblTransaccionesHoy.Size = new System.Drawing.Size(298, 78);
            this.lblTransaccionesHoy.TabIndex = 1;
            this.lblTransaccionesHoy.Text = "0";
            this.lblTransaccionesHoy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Top;
            this.label8.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(298, 30);
            this.label8.TabIndex = 0;
            this.label8.Text = "Transacciones Hoy";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabClientes
            // 
            this.tabClientes.Controls.Add(this.splitContainerClientes);
            this.tabClientes.Location = new System.Drawing.Point(4, 29);
            this.tabClientes.Name = "tabClientes";
            this.tabClientes.Padding = new System.Windows.Forms.Padding(3);
            this.tabClientes.Size = new System.Drawing.Size(1392, 767);
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
            this.splitContainerClientes.Size = new System.Drawing.Size(1386, 761);
            this.splitContainerClientes.SplitterDistance = 800;
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
            this.tablaClientes.Size = new System.Drawing.Size(800, 761);
            this.tablaClientes.TabIndex = 0;
            // 
            // panelFormularioCliente
            // 
            this.panelFormularioCliente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFormularioCliente.Location = new System.Drawing.Point(0, 0);
            this.panelFormularioCliente.Name = "panelFormularioCliente";
            this.panelFormularioCliente.Size = new System.Drawing.Size(582, 761);
            this.panelFormularioCliente.TabIndex = 0;
            // 
            // tabSeguridad
            // 
            this.tabSeguridad.Controls.Add(this.splitContainerSeguridad);
            this.tabSeguridad.Location = new System.Drawing.Point(4, 29);
            this.tabSeguridad.Name = "tabSeguridad";
            this.tabSeguridad.Size = new System.Drawing.Size(1392, 767);
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
            this.splitContainerSeguridad.Size = new System.Drawing.Size(1392, 767);
            this.splitContainerSeguridad.SplitterDistance = 383;
            this.splitContainerSeguridad.TabIndex = 0;
            // 
            // panelUsuariosBloqueados
            // 
            this.panelUsuariosBloqueados.Controls.Add(this.btnDesbloquearUsuario);
            this.panelUsuariosBloqueados.Controls.Add(this.gridUsuariosBloqueados);
            this.panelUsuariosBloqueados.Controls.Add(this.label9);
            this.panelUsuariosBloqueados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUsuariosBloqueados.Location = new System.Drawing.Point(0, 0);
            this.panelUsuariosBloqueados.Name = "panelUsuariosBloqueados";
            this.panelUsuariosBloqueados.Size = new System.Drawing.Size(1392, 383);
            this.panelUsuariosBloqueados.TabIndex = 0;
            // 
            // btnDesbloquearUsuario
            // 
            this.btnDesbloquearUsuario.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnDesbloquearUsuario.Location = new System.Drawing.Point(0, 350);
            this.btnDesbloquearUsuario.Name = "btnDesbloquearUsuario";
            this.btnDesbloquearUsuario.Size = new System.Drawing.Size(1392, 33);
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
            this.gridUsuariosBloqueados.Size = new System.Drawing.Size(1392, 353);
            this.gridUsuariosBloqueados.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Top;
            this.label9.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(1392, 30);
            this.label9.TabIndex = 0;
            this.label9.Text = "Usuarios Bloqueados";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelCrearAdmin
            // 
            this.panelCrearAdmin.Controls.Add(this.panelFormAdmin);
            this.panelCrearAdmin.Controls.Add(this.label15);
            this.panelCrearAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCrearAdmin.Location = new System.Drawing.Point(0, 0);
            this.panelCrearAdmin.Name = "panelCrearAdmin";
            this.panelCrearAdmin.Size = new System.Drawing.Size(1392, 380);
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
            this.panelFormAdmin.Size = new System.Drawing.Size(1392, 350);
            this.panelFormAdmin.TabIndex = 1;
            // 
            // btnCrearAdministrador
            // 
            this.btnCrearAdministrador.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCrearAdministrador.Location = new System.Drawing.Point(10, 307);
            this.btnCrearAdministrador.Name = "btnCrearAdministrador";
            this.btnCrearAdministrador.Size = new System.Drawing.Size(1372, 33);
            this.btnCrearAdministrador.TabIndex = 1;
            this.btnCrearAdministrador.Text = "Crear Administrador";
            this.btnCrearAdministrador.UseVisualStyleBackColor = true;
            // 
            // tableLayoutAdmin
            // 
            this.tableLayoutAdmin.ColumnCount = 2;
            this.tableLayoutAdmin.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutAdmin.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutAdmin.Controls.Add(this.label10, 0, 0);
            this.tableLayoutAdmin.Controls.Add(this.txtAdminUsername, 1, 0);
            this.tableLayoutAdmin.Controls.Add(this.label12, 0, 1);
            this.tableLayoutAdmin.Controls.Add(this.txtAdminPassword, 1, 1);
            this.tableLayoutAdmin.Controls.Add(this.label13, 0, 2);
            this.tableLayoutAdmin.Controls.Add(this.txtAdminNombre, 1, 2);
            this.tableLayoutAdmin.Controls.Add(this.label14, 0, 3);
            this.tableLayoutAdmin.Controls.Add(this.txtAdminEmail, 1, 3);
            this.tableLayoutAdmin.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutAdmin.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutAdmin.Name = "tableLayoutAdmin";
            this.tableLayoutAdmin.RowCount = 4;
            this.tableLayoutAdmin.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAdmin.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAdmin.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAdmin.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAdmin.Size = new System.Drawing.Size(1372, 200);
            this.tableLayoutAdmin.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 20);
            this.label10.TabIndex = 0;
            this.label10.Text = "Username:";
            // 
            // txtAdminUsername
            // 
            this.txtAdminUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdminUsername.Location = new System.Drawing.Point(414, 12);
            this.txtAdminUsername.Name = "txtAdminUsername";
            this.txtAdminUsername.Size = new System.Drawing.Size(955, 27);
            this.txtAdminUsername.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 65);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 20);
            this.label12.TabIndex = 2;
            this.label12.Text = "Password:";
            // 
            // txtAdminPassword
            // 
            this.txtAdminPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdminPassword.Location = new System.Drawing.Point(414, 62);
            this.txtAdminPassword.Name = "txtAdminPassword";
            this.txtAdminPassword.PasswordChar = '*';
            this.txtAdminPassword.Size = new System.Drawing.Size(955, 27);
            this.txtAdminPassword.TabIndex = 3;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 115);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 20);
            this.label13.TabIndex = 4;
            this.label13.Text = "Nombre:";
            // 
            // txtAdminNombre
            // 
            this.txtAdminNombre.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdminNombre.Location = new System.Drawing.Point(414, 112);
            this.txtAdminNombre.Name = "txtAdminNombre";
            this.txtAdminNombre.Size = new System.Drawing.Size(955, 27);
            this.txtAdminNombre.TabIndex = 5;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 165);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 20);
            this.label14.TabIndex = 6;
            this.label14.Text = "Email:";
            // 
            // txtAdminEmail
            // 
            this.txtAdminEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdminEmail.Location = new System.Drawing.Point(414, 162);
            this.txtAdminEmail.Name = "txtAdminEmail";
            this.txtAdminEmail.Size = new System.Drawing.Size(955, 27);
            this.txtAdminEmail.TabIndex = 7;
            // 
            // label15
            // 
            this.label15.Dock = System.Windows.Forms.DockStyle.Top;
            this.label15.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(0, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(1392, 30);
            this.label15.TabIndex = 0;
            this.label15.Text = "Crear Nuevo Administrador";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabReportes
            // 
            this.tabReportes.Controls.Add(this.panelReportes);
            this.tabReportes.Location = new System.Drawing.Point(4, 29);
            this.tabReportes.Name = "tabReportes";
            this.tabReportes.Size = new System.Drawing.Size(1392, 767);
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
            this.panelReportes.Size = new System.Drawing.Size(1392, 767);
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
            this.gridReporteTransacciones.Size = new System.Drawing.Size(1392, 687);
            this.gridReporteTransacciones.TabIndex = 1;
            // 
            // panelFiltrosReporte
            // 
            this.panelFiltrosReporte.Controls.Add(this.btnGenerarReporte);
            this.panelFiltrosReporte.Controls.Add(this.dtpFechaFin);
            this.panelFiltrosReporte.Controls.Add(this.label7);
            this.panelFiltrosReporte.Controls.Add(this.dtpFechaInicio);
            this.panelFiltrosReporte.Controls.Add(this.label5);
            this.panelFiltrosReporte.Controls.Add(this.label3);
            this.panelFiltrosReporte.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltrosReporte.Location = new System.Drawing.Point(0, 0);
            this.panelFiltrosReporte.Name = "panelFiltrosReporte";
            this.panelFiltrosReporte.Size = new System.Drawing.Size(1392, 80);
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
            this.dtpFechaFin.Size = new System.Drawing.Size(200, 27);
            this.dtpFechaFin.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(470, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "Fecha Fin:";
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.Location = new System.Drawing.Point(220, 28);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(200, 27);
            this.dtpFechaInicio.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(120, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 20);
            this.label5.TabIndex = 1;
            this.label5.Text = "Fecha Inicio:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(20, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "Filtrar por:";
            // 
            // tabNotificaciones
            // 
            this.tabNotificaciones.Controls.Add(this.panelNotificaciones);
            this.tabNotificaciones.Location = new System.Drawing.Point(4, 29);
            this.tabNotificaciones.Name = "tabNotificaciones";
            this.tabNotificaciones.Size = new System.Drawing.Size(1392, 767);
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
            this.panelNotificaciones.Size = new System.Drawing.Size(1392, 767);
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
            this.gridNotificacionesSistema.Size = new System.Drawing.Size(1392, 687);
            this.gridNotificacionesSistema.TabIndex = 1;
            // 
            // panelControlesNotificaciones
            // 
            this.panelControlesNotificaciones.Controls.Add(this.btnEjecutarResumenes);
            this.panelControlesNotificaciones.Controls.Add(this.label16);
            this.panelControlesNotificaciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControlesNotificaciones.Location = new System.Drawing.Point(0, 0);
            this.panelControlesNotificaciones.Name = "panelControlesNotificaciones";
            this.panelControlesNotificaciones.Size = new System.Drawing.Size(1392, 80);
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
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(400, 32);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(187, 19);
            this.label16.TabIndex = 0;
            this.label16.Text = "Notificaciones del Sistema";
            // 
            // tabMovimientos
            // 
            this.tabMovimientos.Controls.Add(this.tablaMovimientos);
            this.tabMovimientos.Location = new System.Drawing.Point(4, 29);
            this.tabMovimientos.Name = "tabMovimientos";
            this.tabMovimientos.Size = new System.Drawing.Size(1392, 767);
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
            this.tablaMovimientos.Size = new System.Drawing.Size(1392, 767);
            this.tablaMovimientos.TabIndex = 0;
            // 
            // tabAnalisis
            // 
            this.tabAnalisis.Controls.Add(this.tablaAnalisis);
            this.tabAnalisis.Location = new System.Drawing.Point(4, 29);
            this.tabAnalisis.Name = "tabAnalisis";
            this.tabAnalisis.Size = new System.Drawing.Size(1392, 767);
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
            this.tablaAnalisis.Size = new System.Drawing.Size(1392, 767);
            this.tablaAnalisis.TabIndex = 0;
            // 
            // tabResumen
            // 
            this.tabResumen.Controls.Add(this.tablaResumen);
            this.tabResumen.Location = new System.Drawing.Point(4, 29);
            this.tabResumen.Name = "tabResumen";
            this.tabResumen.Size = new System.Drawing.Size(1392, 767);
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
            this.tablaResumen.Size = new System.Drawing.Size(1392, 767);
            this.tablaResumen.TabIndex = 0;
            // 
            // PanelAdministrativo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PanelAdministrativo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Banco Fisclase 24/7 - Panel de Administración Avanzado";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabControl.ResumeLayout(false);
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerClientes)).EndInit();
            this.splitContainerClientes.Panel1.ResumeLayout(false);
            this.splitContainerClientes.Panel2.ResumeLayout(false);
            this.splitContainerClientes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tablaClientes)).EndInit();
            this.tabSeguridad.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSeguridad)).EndInit();
            this.splitContainerSeguridad.Panel1.ResumeLayout(false);
            this.splitContainerSeguridad.Panel2.ResumeLayout(false);
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

        private TabControl tabControl;
        private TabPage tabDashboard;
        private Panel panelDashboard;
        private Panel panelTransaccionesRecientes;
        private DataGridView gridTransaccionesRecientes;
        private Label label11;
        private Panel panelEstadisticas;
        private FlowLayoutPanel flowLayoutEstadisticas;
        private Panel panelTotalClientes;
        private Label lblTotalClientes;
        private Label label2;
        private Panel panelTotalCuentas;
        private Label lblTotalCuentas;
        private Label label4;
        private Panel panelSaldoTotal;
        private Label lblSaldoTotal;
        private Label label6;
        private Panel panelTransaccionesHoy;
        private Label lblTransaccionesHoy;
        private Label label8;
        private TabPage tabClientes;
        private SplitContainer splitContainerClientes;
        private DataGridView tablaClientes;
        private Panel panelFormularioCliente;
        private TabPage tabSeguridad;
        private SplitContainer splitContainerSeguridad;
        private Panel panelUsuariosBloqueados;
        private Button btnDesbloquearUsuario;
        private DataGridView gridUsuariosBloqueados;
        private Label label9;
        private Panel panelCrearAdmin;
        private Panel panelFormAdmin;
        private Button btnCrearAdministrador;
        private TableLayoutPanel tableLayoutAdmin;
        private Label label10;
        private TextBox txtAdminUsername;
        private Label label12;
        private TextBox txtAdminPassword;
        private Label label13;
        private TextBox txtAdminNombre;
        private Label label14;
        private TextBox txtAdminEmail;
        private Label label15;
        private TabPage tabReportes;
        private Panel panelReportes;
        private DataGridView gridReporteTransacciones;
        private Panel panelFiltrosReporte;
        private Button btnGenerarReporte;
        private DateTimePicker dtpFechaFin;
        private Label label7;
        private DateTimePicker dtpFechaInicio;
        private Label label5;
        private Label label3;
        private TabPage tabNotificaciones;
        private Panel panelNotificaciones;
        private DataGridView gridNotificacionesSistema;
        private Panel panelControlesNotificaciones;
        private Button btnEjecutarResumenes;
        private Label label16;
        private TabPage tabMovimientos;
        private DataGridView tablaMovimientos;
        private TabPage tabAnalisis;
        private DataGridView tablaAnalisis;
        private TabPage tabResumen;
        private DataGridView tablaResumen;
    }
}