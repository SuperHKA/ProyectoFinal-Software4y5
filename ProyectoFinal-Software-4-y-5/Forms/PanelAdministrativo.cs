using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoFinal_Software_4_y_5.Entidades;
using ProyectoFinal_Software_4_y_5.Servicios;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public partial class PanelAdministrativo : Form
    {
        // Controles del formulario de cliente (se crearán dinámicamente)
        private TextBox txtId, txtUsuario, txtNombre, txtEmail, txtTelefono;
        private TextBox txtCalle, txtNumero, txtCiudad, txtEstado, txtCodigoPostal, txtPais;
        private TextBox txtContrasena;
        private Button btnNuevo, btnGuardar, btnEliminar, btnActualizar;
        private Button btnCambiarContrasena, btnCrearCuenta, btnActualizarSaldo;
        private Button btnVerCuentas; // Nuevo botón para ver cuentas del cliente

        // Use the API adapter exclusively
        private readonly ServicioBancoApi _servicioBancoApi;
        private readonly ApiAdmin _apiCliente;

        // Flag para evitar loops de cierre y reentradas
        private bool _isShuttingDown;

        public PanelAdministrativo()
        {
            try
            {
                InitializeComponent();

                // Eliminar mapeo a botones del Designer; usaremos creación dinámica
                // btnNuevo = this.btnNuevoCliente;
                // btnGuardar = this.btnGuardarCliente;
                // btnEliminar = this.btnEliminarCliente;
                // btnActualizar = this.btnActualizarClientes;
                // btnCambiarContrasena = this.btnCambiarContrasenaCliente;
                // btnCrearCuenta = this.btnCrearCuentaCliente;
                // btnActualizarSaldo = this.btnActualizarSaldoCliente;
                // btnVerCuentas = this.btnVerCuentasCliente;

                _apiCliente = new ApiAdmin("http://localhost:5202", allowInvalidSsl: false);
                _servicioBancoApi = new ServicioBancoApi("http://localhost:5202", allowInvalidSsl: false);

                AplicarEstilosModernos();

                // Crear formulario de cliente dinámicamente (incluiremos botones dinámicos)
                CrearFormularioCliente();

                ConectarEventos();

                this.Load += PanelAdministrativo_Load;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico al inicializar el panel: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarEstilosModernos()
        {
            // Estilos para el formulario
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            // Estilos para paneles de estadísticas
            var paneles = new[] { panelTotalClientes, panelTotalCuentas, panelSaldoTotal, panelTransaccionesHoy };
            foreach (var panel in paneles)
            {
                panel.BackColor = Color.White;
            }

            // Estilos para DataGridView de transacciones recientes
            AplicarEstiloDataGridView(gridTransaccionesRecientes);
            AplicarEstiloDataGridView(tablaClientes);
            AplicarEstiloDataGridView(tablaMovimientos);
            AplicarEstiloDataGridView(tablaAnalisis);
            AplicarEstiloDataGridView(tablaResumen);

            // Panel dashboard
            panelDashboard.BackColor = Color.FromArgb(248, 249, 250);
        }

        private void AplicarEstiloDataGridView(DataGridView grid)
        {
            // Estilos generales
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(230, 230, 230);
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.EnableHeadersVisualStyles = false;
            grid.AllowUserToResizeRows = false;

            // Estilos de encabezado
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.SlateBlue;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.SlateBlue;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 8, 10, 8);
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Estilos de celdas
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 90, 168);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            grid.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            // Filas alternas
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            grid.RowTemplate.Height = 32;
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            CerrarSesion();
        }

        private void CerrarSesion()
        {
            var resultado = MessageBox.Show(
                "¿Está seguro que desea cerrar la sesión?",
                "Confirmar Cierre de Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    _isShuttingDown = true; // Evitar OnFormClosing loops y reabrir login automáticamente

                    // Clear any sensitive data or perform cleanup
                    LimpiarDatosSesion();

                    // Cerrar de forma segura usando ProcessManager
                    System.Diagnostics.Debug.WriteLine("INFO: Cerrando sesión administrativa");

                    // Limpiar recursos de API
                    _apiCliente?.Dispose();

                    // Close current form
                    this.Close();

                    // Mostrar login form si la app sigue abierta
                    var loginForm = new FormLogin();
                    loginForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cerrar sesión: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Forzar cierre seguro de la app
                    Program.ExitApplication(1);
                }
            }
        }

        // Override form closing to ensure proper cleanup
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Si el cierre fue iniciado por la aplicación o estamos en proceso de cierre, no pedir confirmación ni reabrir login
            if (_isShuttingDown || e.CloseReason == CloseReason.ApplicationExitCall)
            {
                base.OnFormClosing(e);
                return;
            }

            if (e.CloseReason == CloseReason.UserClosing)
            {
                var resultado = MessageBox.Show(
                    "¿Está seguro que desea cerrar la aplicación?",
                    "Confirmar Cierre",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                try
                {
                    _isShuttingDown = true; // marcamos para evitar reentradas
                    LimpiarDatosSesion();
                    _apiCliente?.Dispose();
                    System.Diagnostics.Debug.WriteLine("INFO: PanelAdministrativo cerrándose correctamente");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Error en OnFormClosing: {ex.Message}");
                }

                // Determinar si es el último formulario
                var openForms = Application.OpenForms;
                bool isLastForm = true;
                foreach (Form form in openForms)
                {
                    if (form != this && !(form is FormLogin))
                    {
                        isLastForm = false;
                        break;
                    }
                }

                if (isLastForm)
                {
                    System.Diagnostics.Debug.WriteLine("INFO: Es el último formulario, cerrando aplicación");
                    e.Cancel = true; // Cancelar cierre normal y usar salida segura
                    Program.ExitApplication(0);
                    return;
                }

                // No reabrir login aquí para evitar loops; si se necesita, se gestionará externamente
            }

            base.OnFormClosing(e);
        }

        private async void PanelAdministrativo_Load(object sender, EventArgs e)
        {
            // Run initial data load asynchronously to avoid blocking UI thread
            try
            {
                await InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inicializando datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task InitializeAsync()
        {
            // API health check
            try
            {
                var healthy = await _apiCliente.HealthAsync().ConfigureAwait(false);
                if (!healthy)
                {
                    this.BeginInvoke((Action)(() => MessageBox.Show("Warning: API health check failed. UI will run in degraded mode and features requiring the API may fail.",
                        "API Unreachable", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                }
            }
            catch (Exception hx)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Warning: could not reach API: {hx.Message}\nThe application will continue but some features may not work.",
                    "API Unreachable", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
            }

            // Load data in background and marshal to UI
            try
            {
                var clientesTask = _apiCliente.ObtenerClientesAsync();
                var statsTask = _servicioBancoApi.ObtenerEstadisticasGeneralesAsync();
                var transTask = _servicioBancoApi.ObtenerTransaccionesRecientesAsync(20);
                var movimientosTask = _servicioBancoApi.ObtenerMovimientosDelDiaAsync();
                var usuariosBloqueadosTask = _servicioBancoApi.ObtenerUsuariosBloqueadosAsync();
                var notifsTask = _servicioBancoApi.ObtenerNotificacionesSistemaAsync();

                await Task.WhenAll(statsTask, transTask, movimientosTask, usuariosBloqueadosTask, notifsTask, clientesTask).ConfigureAwait(false);

                var clientes = await clientesTask.ConfigureAwait(false);
                var stats = await statsTask.ConfigureAwait(false);
                var trans = await transTask.ConfigureAwait(false);
                var movimientos = await movimientosTask.ConfigureAwait(false);
                var usuariosBloqueados = await usuariosBloqueadosTask.ConfigureAwait(false);
                var notifs = await notifsTask.ConfigureAwait(false);

                // marshal to UI
                this.BeginInvoke((Action)(() =>
                {
                    try
                    {
                        // Convert clientes to our DTO format before populating
                        var clientesDto = clientes.Select(c => new ProyectoFinal_Software_4_y_5.Entidades.DTOs.ClienteDto
                        {
                            usuario_id = c.usuario_id,
                            cliente_id = c.cliente_id,
                            username = c.username,
                            nombre_completo = c.nombre_completo,
                            email = c.email,
                            telefono = c.telefono,
                            es_admin = c.es_admin,
                            bloqueado = c.bloqueado
                        }).ToList();

                        // populate UI with results
                        PopulateClientesGridFromDto(clientesDto);
                        PopulateDashboard(stats, trans);
                        
                        // Asignar datos directamente - el servicio ya mapea los nombres
                        tablaMovimientos.DataSource = movimientos;
                        gridUsuariosBloqueados.DataSource = usuariosBloqueados;
                        gridNotificacionesSistema.DataSource = notifs;
                        
                        // Use async results for small views
                        var analisis = _servicioBancoApi.ObtenerAnalisisTransaccionesAsync().GetAwaiter().GetResult();
                        tablaAnalisis.DataSource = analisis;
                        
                        var resumen = _servicioBancoApi.ObtenerResumenClientesAsync().GetAwaiter().GetResult();
                        tablaResumen.DataSource = resumen;
                    }
                    catch (Exception uiEx)
                    {
                        MessageBox.Show($"Error actualizando UI: {uiEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }));
            }
            catch (Exception ex)
            {
                // non-fatal; let user know
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error cargando datos iniciales: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
            }
        }

        private void PopulateClientesGrid(System.Collections.Generic.List<Usuario> clientes)
        {
            try
            {
                if (clientes == null)
                {
                    tablaClientes.DataSource = null;
                    return;
                }

                var dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("Usuario");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Email");
                dt.Columns.Add("Telefono");
                dt.Columns.Add("Estado");

                foreach (var cliente in clientes)
                {
                    dt.Rows.Add(
                        cliente.IdUsuario,
                        cliente.NombreUsuario ?? "N/A",
                        cliente.NombreCompleto ?? "N/A",
                        cliente.Email ?? "N/A",
                        cliente.Telefono ?? "N/A",
                        cliente.EstadoCuenta ? "Bloqueado" : "Activo"
                    );
                }

                tablaClientes.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al poblar lista de clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateClientesGridFromDto(System.Collections.Generic.IEnumerable<ProyectoFinal_Software_4_y_5.Entidades.DTOs.ClienteDto> clientes)
        {
            try
            {
                if (clientes == null)
                {
                    tablaClientes.DataSource = null;
                    return;
                }

                var dt = new DataTable();
                // ID shown = cliente_id when available (matches Cliente table), fall back to usuario_id
                dt.Columns.Add("ID");
                dt.Columns.Add("UsuarioId", typeof(int)); // hidden helper column
                dt.Columns.Add("ClienteId", typeof(int)); // hidden helper column
                dt.Columns.Add("Usuario");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Email");
                dt.Columns.Add("Telefono");
                dt.Columns.Add("Estado");

                foreach (var cliente in clientes)
                {
                    // Handle potential null values safely
                    var usuarioIdVal = cliente.usuario_id;
                    var clienteIdVal = cliente.cliente_id ?? 0; // Use 0 if null
                    var displayId = clienteIdVal > 0 ? clienteIdVal.ToString() : usuarioIdVal.ToString();

                    var row = dt.NewRow();
                    row["ID"] = displayId;
                    row["UsuarioId"] = usuarioIdVal;
                    row["ClienteId"] = clienteIdVal;
                    row["Usuario"] = cliente.username ?? "N/A";  // Nombre de usuario (username)
                    row["Nombre"] = cliente.nombre_completo ?? "N/A";  // Nombre completo
                    row["Email"] = cliente.email ?? "N/A";
                    row["Telefono"] = cliente.telefono ?? "N/A";
                    row["Estado"] = cliente.bloqueado ? "Bloqueado" : "Activo";
                    dt.Rows.Add(row);
                }

                tablaClientes.DataSource = dt;

                // hide helper columns from view but ensure they exist
                if (tablaClientes.Columns.Contains("UsuarioId"))
                {
                    tablaClientes.Columns["UsuarioId"].Visible = false;
                }
                if (tablaClientes.Columns.Contains("ClienteId"))
                {
                    tablaClientes.Columns["ClienteId"].Visible = false;
                }

                // Debug: Print column names
                System.Diagnostics.Debug.WriteLine("DEBUG: Columns in DataTable:");
                foreach (DataColumn col in dt.Columns)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {col.ColumnName} ({col.DataType})");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al poblar lista de clientes (DTO): {ex.Message}\nDetalles: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDashboard(DataTable stats, DataTable trans)
        {
            try
            {
                if (stats != null && stats.Rows.Count > 0)
                {
                    var row = stats.Rows[0];
                    lblTotalClientes.Text = row["Total Clientes"].ToString();
                    lblTotalCuentas.Text = row["Total Cuentas"].ToString();
                    lblSaldoTotal.Text = $"${Convert.ToDecimal(row["Saldo Total Sistema"]):N2}";
                    lblTransaccionesHoy.Text = row["Transacciones Hoy"].ToString();
                }

                // El servicio ya devuelve los nombres en español
                gridTransaccionesRecientes.DataSource = trans;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al poblar dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Los nombres de columnas ahora se manejan en ServicioBancoApi.ToDataTable()

        private async Task CargarDatosClientes()
        {
            try
            {
                // Use the DTO method instead of the old one to maintain consistency
                var clientes = await _apiCliente.ObtenerClientesAsync().ConfigureAwait(false);
                
                // Convert to our DTO format
                var clientesDto = clientes.Select(c => new ProyectoFinal_Software_4_y_5.Entidades.DTOs.ClienteDto
                {
                    usuario_id = c.usuario_id,
                    cliente_id = c.cliente_id,
                    username = c.username,
                    nombre_completo = c.nombre_completo,
                    email = c.email,
                    telefono = c.telefono,
                    es_admin = c.es_admin,
                    bloqueado = c.bloqueado
                }).ToList();
                
                this.BeginInvoke((Action)(() => PopulateClientesGridFromDto(clientesDto)));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private void CrearFormularioCliente()
        {
            var panel = panelFormularioCliente;
            panel.Controls.Clear();

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 15,
                Padding = new Padding(10)
            };

            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            int row = 0;
            AgregarControl(tableLayout, "ID:", txtId = new TextBox { ReadOnly = true, Name = "txtClienteId" }, row++);
            AgregarControl(tableLayout, "Usuario:", txtUsuario = new TextBox { Name = "txtClienteUsuario" }, row++);
            AgregarControl(tableLayout, "Nombre:", txtNombre = new TextBox { Name = "txtClienteNombre" }, row++);
            AgregarControl(tableLayout, "Email:", txtEmail = new TextBox { Name = "txtClienteEmail" }, row++);
            AgregarControl(tableLayout, "Telefono:", txtTelefono = new TextBox { Name = "txtClienteTelefono" }, row++);

            tableLayout.Controls.Add(new Label { Text = "Dirección", Font = new Font("Arial", 9, FontStyle.Bold), Name = "lblDireccionTitulo" }, 0, row++);
            tableLayout.SetColumnSpan(tableLayout.Controls[tableLayout.Controls.Count - 1], 2);

            AgregarControl(tableLayout, "Calle:", txtCalle = new TextBox { Name = "txtClienteCalle" }, row++);
            AgregarControl(tableLayout, "Numero:", txtNumero = new TextBox { Name = "txtClienteNumero" }, row++);
            AgregarControl(tableLayout, "Ciudad:", txtCiudad = new TextBox { Name = "txtClienteCiudad" }, row++);
            AgregarControl(tableLayout, "Estado:", txtEstado = new TextBox { Name = "txtClienteEstado" }, row++);
            AgregarControl(tableLayout, "Codigo Postal:", txtCodigoPostal = new TextBox { Name = "txtClienteCodigoPostal" }, row++);
            AgregarControl(tableLayout, "País:", txtPais = new TextBox { Name = "txtClientePais" }, row++);

            AgregarControl(tableLayout, "Contraseña:", txtContrasena = new TextBox { UseSystemPasswordChar = true, Name = "txtClienteContrasena" }, row++);

            // Agregar panel de botones creado dinámicamente (arriba para replicar el comportamiento original)
            var panelBotones = CrearPanelBotones();
            panelBotones.Dock = DockStyle.Bottom; // o DockStyle.Bottom si lo prefieres abajo
            panel.Controls.Add(panelBotones);

            // Luego el layout de formulario ocupa el resto
            panel.Controls.Add(tableLayout);
        }

        // Crea los botones y asigna a los campos existentes para que la lógica de eventos funcione
        private Panel CrearPanelBotones()
        {
            var panel = new Panel { Height = 150, Dock = DockStyle.Top };
            var flowLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };

            btnNuevo = new Button { Text = "Nuevo", Size = new Size(100, 30), BackColor = Color.SlateBlue, ForeColor = Color.White };
            btnGuardar = new Button { Text = "Guardar", Size = new Size(100, 30), BackColor = Color.SlateBlue, ForeColor = Color.White };
            btnEliminar = new Button { Text = "Eliminar", Size = new Size(100, 30), BackColor = Color.SlateBlue, ForeColor = Color.White };
            btnActualizar = new Button { Text = "Actualizar", Size = new Size(100, 30), BackColor = Color.SlateBlue, ForeColor = Color.White };
            btnCambiarContrasena = new Button { Text = "Cambiar Contraseña", Size = new Size(180, 30), BackColor = Color.SlateBlue, ForeColor = Color.White  };
            btnCrearCuenta = new Button { Text = "Crear Cuenta Bancaria", Size = new Size(200, 30), BackColor = Color.SlateBlue, ForeColor = Color.White };
            btnActualizarSaldo = new Button { Text = "Actualizar Saldo", Size = new Size(140, 30), BackColor = Color.SlateBlue, ForeColor = Color.White };
            btnVerCuentas = new Button { Text = "Ver Cuentas", Size = new Size(110, 30), BackColor = Color.SlateBlue, ForeColor = Color.White };

            flowLayout.Controls.AddRange(new Control[]
            {
                btnNuevo, btnGuardar, btnEliminar, btnActualizar,
                btnCambiarContrasena, btnCrearCuenta, btnActualizarSaldo, btnVerCuentas
            });

            panel.Controls.Add(flowLayout);
            return panel;
        }

        private void AgregarControl(TableLayoutPanel tableLayout, string labelText, Control control, int row)
        {
            tableLayout.Controls.Add(new Label { Text = labelText, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            tableLayout.Controls.Add(control, 1, row);
        }

        private void ConectarEventos()
        {
            tablaClientes.SelectionChanged += (s, e) => CargarClienteSeleccionado();

            if (btnNuevo != null) btnNuevo.Click += (s, e) => LimpiarFormulario();
            if (btnGuardar != null) btnGuardar.Click += async (s, e) => await GuardarClienteAsync();
            if (btnEliminar != null) btnEliminar.Click += (s, e) => EliminarCliente();
            if (btnActualizar != null) btnActualizar.Click += async (s, e) => await CargarDatosClientes();
            if (btnCambiarContrasena != null) btnCambiarContrasena.Click += async (s, e) => await CambiarContrasenaAsync();
            if (btnCrearCuenta != null) btnCrearCuenta.Click += async (s, e) => await CrearCuentaBancariaAsync();
            if (btnActualizarSaldo != null) btnActualizarSaldo.Click += (s, e) => ActualizarSaldoCuenta();
            if (btnVerCuentas != null) btnVerCuentas.Click += (s, e) => AbrirCuentasCliente();

            if (btnDesbloquearUsuario != null) btnDesbloquearUsuario.Click += async (s, e) => await DesbloquearUsuarioAsync();
            if (btnCrearAdministrador != null) btnCrearAdministrador.Click += async (s, e) => await CrearAdministradorAsync();
            if (btnGenerarReporte != null) btnGenerarReporte.Click += async (s, e) => await GenerarReporteTransaccionesAsync();
            if (btnEjecutarResumenes != null) btnEjecutarResumenes.Click += async (s, e) => await EjecutarResumenesDiariosAsync();
            if (btnActualizarTransRecientes != null) btnActualizarTransRecientes.Click += async (s, e) => await ActualizarTransaccionesRecientesAsync();

            if (tablaResumen != null) tablaResumen.CellDoubleClick += TablaResumen_CellDoubleClick;
        }

        // NUEVO: Método para manejar doble clic en resumen de clientes
        private async void TablaResumen_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verificar que no se haga clic en el encabezado
            {
                try
                {
                    var row = tablaResumen.Rows[e.RowIndex];
                    int? clienteId = null;
                    string nombreCliente = null;

                    // Buscar el cliente_id en las columnas (puede ser "cliente_id" o "ID Cliente")
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        var columnName = tablaResumen.Columns[cell.ColumnIndex].Name;
                        var cellValue = cell.Value?.ToString();

                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            // Buscar cliente_id
                            if (columnName.Equals("cliente_id", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("ID Cliente", StringComparison.OrdinalIgnoreCase))
                            {
                                if (int.TryParse(cellValue, out int id))
                                {
                                    clienteId = id;
                                }
                            }

                            // Buscar nombre del cliente
                            if (columnName.Equals("nombre", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("Nombre del Cliente", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                            {
                                nombreCliente = cellValue;
                            }
                        }
                    }

                    if (!clienteId.HasValue)
                    {
                        MessageBox.Show("No se pudo obtener el ID del cliente.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Obtener las cuentas del cliente
                    var cuentas = await _apiCliente.ObtenerCuentasPorClienteAsync(clienteId.Value);
                    var listaCuentas = cuentas.ToList();

                    if (!listaCuentas.Any())
                    {
                        MessageBox.Show($"El cliente {nombreCliente ?? clienteId.Value.ToString()} no tiene cuentas activas.",
                            "Sin Cuentas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string numeroCuenta;

                    if (listaCuentas.Count == 1)
                    {
                        // Si solo hay una cuenta, abrirla directamente
                        var cuenta = listaCuentas.First();
                        numeroCuenta = cuenta.ContainsKey("Numero de Cuenta") 
                            ? cuenta["Numero de Cuenta"]?.ToString() 
                            : null;

                        if (string.IsNullOrEmpty(numeroCuenta))
                        {
                            MessageBox.Show("No se pudo obtener el número de cuenta.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        // Si hay múltiples cuentas, mostrar diálogo para seleccionar
                        using (var dlg = new Form
                        {
                            Text = $"Seleccionar Cuenta - {nombreCliente ?? clienteId.Value.ToString()}",
                            Size = new Size(500, 200),
                            StartPosition = FormStartPosition.CenterParent,
                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            MaximizeBox = false,
                            MinimizeBox = false
                        })
                        {
                            var cmbCuentas = new ComboBox
                            {
                                Location = new Point(20, 50),
                                Size = new Size(440, 25),
                                DropDownStyle = ComboBoxStyle.DropDownList
                            };

                            foreach (var cuenta in listaCuentas)
                            {
                                var numCta = cuenta.ContainsKey("Numero de Cuenta") 
                                    ? cuenta["Numero de Cuenta"]?.ToString() 
                                    : "N/A";
                                var tipoCta = cuenta.ContainsKey("Tipo de Cuenta") 
                                    ? cuenta["Tipo de Cuenta"]?.ToString() 
                                    : "N/A";
                                var saldo = cuenta.ContainsKey("Saldo") 
                                    ? cuenta["Saldo"]?.ToString() 
                                    : "0";

                                cmbCuentas.Items.Add(new CuentaComboItem
                                {
                                    NumeroCuenta = numCta,
                                    Display = $"{numCta} - {tipoCta} - Saldo: ${saldo}"
                                });
                            }

                            cmbCuentas.SelectedIndex = 0;

                            var btnOk = new Button
                            {
                                Text = "Aceptar",
                                DialogResult = DialogResult.OK,
                                Location = new Point(280, 110),
                                Size = new Size(90, 30)
                            };

                            var btnCancel = new Button
                            {
                                Text = "Cancelar",
                                DialogResult = DialogResult.Cancel,
                                Location = new Point(380, 110),
                                Size = new Size(90, 30)
                            };

                            dlg.Controls.AddRange(new Control[]
                            {
                                new Label { Text = "Seleccione una cuenta:", Location = new Point(20, 20), Size = new Size(400, 20) },
                                cmbCuentas,
                                btnOk,
                                btnCancel
                            });

                            dlg.AcceptButton = btnOk;
                            dlg.CancelButton = btnCancel;

                            if (dlg.ShowDialog(this) != DialogResult.OK)
                                return;

                            var selectedItem = cmbCuentas.SelectedItem as CuentaComboItem;
                            numeroCuenta = selectedItem?.NumeroCuenta;

                            if (string.IsNullOrEmpty(numeroCuenta) || numeroCuenta == "N/A")
                            {
                                MessageBox.Show("No se pudo obtener el número de cuenta.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    // Abrir el formulario de detalle de cuenta
                    var detalleForm = new DetalleDeCuenta(clienteId.Value, numeroCuenta);
                    detalleForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al procesar doble clic: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task CargarDashboardAsync()
        {
            try
            {
                var stats = await _servicioBancoApi.ObtenerEstadisticasGeneralesAsync().ConfigureAwait(false);
                if (stats != null && stats.Rows.Count > 0)
                {
                    var row = stats.Rows[0];
                    this.BeginInvoke((Action)(() =>
                    {
                        lblTotalClientes.Text = row["Total Clientes"].ToString();
                        lblTotalCuentas.Text = row["Total Cuentas"].ToString();
                        lblSaldoTotal.Text = $"${Convert.ToDecimal(row["Saldo Total Sistema"]):N2}";
                        lblTransaccionesHoy.Text = row["Transacciones Hoy"].ToString();
                    }));
                }

                var trans = await _servicioBancoApi.ObtenerTransaccionesRecientesAsync(20).ConfigureAwait(false);
                this.BeginInvoke((Action)(() => gridTransaccionesRecientes.DataSource = trans));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private async Task CargarVistasAsync()
        {
            try
            {
                var movimientos = await _servicioBancoApi.ObtenerMovimientosDelDiaAsync().ConfigureAwait(false);
                var analisis = await _servicioBancoApi.ObtenerAnalisisTransaccionesAsync().ConfigureAwait(false);
                var resumen = await _servicioBancoApi.ObtenerResumenClientesAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() =>
                {
                    tablaMovimientos.DataSource = movimientos;
                    tablaAnalisis.DataSource = analisis;
                    tablaResumen.DataSource = resumen;
                }));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar vistas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private async Task ActualizarTransaccionesRecientesAsync()
        {
            if (btnActualizarTransRecientes != null)
            {
                btnActualizarTransRecientes.Enabled = false;
                btnActualizarTransRecientes.Text = "⏳ Actualizando...";
            }
            
            try
            {
                var trans = await _servicioBancoApi.ObtenerTransaccionesRecientesAsync(20).ConfigureAwait(false);
                this.BeginInvoke((Action)(() =>
                {
                    gridTransaccionesRecientes.DataSource = trans;
                    MessageBox.Show("Transacciones recientes actualizadas correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al actualizar transacciones recientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
            finally
            {
                if (btnActualizarTransRecientes != null)
                {
                    this.BeginInvoke((Action)(() =>
                    {
                        btnActualizarTransRecientes.Enabled = true;
                        btnActualizarTransRecientes.Text = "🔄 Actualizar";
                    }));
                }
            }
        }

        private async void BtnActualizarBloqueados_Click(object sender, EventArgs e)
        {
            btnActualizarBloqueados.Enabled = false;
            btnActualizarBloqueados.Text = "⏳ Actualizando...";
            
            try
            {
                await CargarUsuariosBloqueadosAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() =>
                {
                    btnActualizarBloqueados.Text = "✓ Actualizado";
                    System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ =>
                    {
                        this.BeginInvoke((Action)(() =>
                        {
                            btnActualizarBloqueados.Text = "🔄 Actualizar";
                            btnActualizarBloqueados.Enabled = true;
                        }));
                    });
                }));
            }
            catch
            {
                this.BeginInvoke((Action)(() =>
                {
                    btnActualizarBloqueados.Text = "🔄 Actualizar";
                    btnActualizarBloqueados.Enabled = true;
                }));
            }
        }

        private async Task CargarUsuariosBloqueadosAsync()
        {
            try
            {
                var users = await _servicioBancoApi.ObtenerUsuariosBloqueadosAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() => gridUsuariosBloqueados.DataSource = users));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar usuarios bloqueados: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private async Task CargarNotificacionesSistemaAsync()
        {
            try
            {
                var notifs = await _servicioBancoApi.ObtenerNotificacionesSistemaAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() => gridNotificacionesSistema.DataSource = notifs));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar notificaciones: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private async void BtnActualizarMovimientos_Click(object sender, EventArgs e)
        {
            btnActualizarMovimientos.Enabled = false;
            btnActualizarMovimientos.Text = "⏳ Actualizando...";

            try
            {
                await CargarMovimientosAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() =>
                {
                    btnActualizarMovimientos.Text = "✓ Actualizado";
                    System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ =>
                    {
                        this.BeginInvoke((Action)(() =>
                        {
                            btnActualizarMovimientos.Text = "🔄 Actualizar";
                            btnActualizarMovimientos.Enabled = true;
                        }));
                    });
                }));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() =>
                {
                    MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnActualizarMovimientos.Text = "🔄 Actualizar";
                    btnActualizarMovimientos.Enabled = true;
                }));
            }
        }

        private async Task CargarMovimientosAsync()
        {
            try
            {
                var movimientos = await _servicioBancoApi.ObtenerMovimientosDelDiaAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() => tablaMovimientos.DataSource = movimientos));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar movimientos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private async void BtnActualizarAnalisis_Click(object sender, EventArgs e)
        {
            btnActualizarAnalisis.Enabled = false;
            btnActualizarAnalisis.Text = "⏳ Actualizando...";

            try
            {
                await CargarAnalisisAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() =>
                {
                    btnActualizarAnalisis.Text = "✓ Actualizado";
                    System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ =>
                    {
                        this.BeginInvoke((Action)(() =>
                        {
                            btnActualizarAnalisis.Text = "🔄 Actualizar";
                            btnActualizarAnalisis.Enabled = true;
                        }));
                    });
                }));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() =>
                {
                    MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnActualizarAnalisis.Text = "🔄 Actualizar";
                    btnActualizarAnalisis.Enabled = true;
                }));
            }
        }

        private async Task CargarAnalisisAsync()
        {
            try
            {
                var analisis = await _servicioBancoApi.ObtenerAnalisisTransaccionesAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() => tablaAnalisis.DataSource = analisis));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar análisis: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private async void BtnActualizarResumen_Click(object sender, EventArgs e)
        {
            btnActualizarResumen.Enabled = false;
            btnActualizarResumen.Text = "⏳ Actualizando...";

            try
            {
                await CargarResumenAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() =>
                {
                    btnActualizarResumen.Text = "✓ Actualizado";
                    System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ =>
                    {
                        this.BeginInvoke((Action)(() =>
                        {
                            btnActualizarResumen.Text = "🔄 Actualizar";
                            btnActualizarResumen.Enabled = true;
                        }));
                    });
                }));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() =>
                {
                    MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnActualizarResumen.Text = "🔄 Actualizar";
                    btnActualizarResumen.Enabled = true;
                }));
            }
        }

        private async Task CargarResumenAsync()
        {
            try
            {
                var resumen = await _servicioBancoApi.ObtenerResumenClientesAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() => tablaResumen.DataSource = resumen));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar resumen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private void CargarClienteSeleccionado()
        {
            if (tablaClientes.SelectedRows.Count > 0)
            {
                var row = tablaClientes.SelectedRows[0];

                try
                {
                    // Debug: print available columns
                    System.Diagnostics.Debug.WriteLine("DEBUG: Available columns in selected row:");
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - {tablaClientes.Columns[cell.ColumnIndex].Name}: {cell.Value}");
                    }

                    // Safely get IDs with fallback
                    int clienteIdVal = 0;
                    int usuarioIdVal = 0;

                    // Try to get ClienteId
                    if (tablaClientes.Columns.Contains("ClienteId"))
                    {
                        var clienteIdObj = row.Cells["ClienteId"].Value;
                        if (clienteIdObj != null && clienteIdObj != DBNull.Value)
                        {
                            int.TryParse(clienteIdObj.ToString(), out clienteIdVal);
                        }
                    }

                    // Try to get UsuarioId
                    if (tablaClientes.Columns.Contains("UsuarioId"))
                    {
                        var usuarioIdObj = row.Cells["UsuarioId"].Value;
                        if (usuarioIdObj != null && usuarioIdObj != DBNull.Value)
                        {
                            int.TryParse(usuarioIdObj.ToString(), out usuarioIdVal);
                        }
                    }

                    // Use ClienteId if available and > 0, otherwise use UsuarioId
                    if (clienteIdVal > 0)
                    {
                        txtId.Text = clienteIdVal.ToString();
                    }
                    else if (usuarioIdVal > 0)
                    {
                        txtId.Text = usuarioIdVal.ToString();
                    }
                    else
                    {
                        txtId.Text = "";
                    }

                    // Safely get other field values
                    txtUsuario.Text = tablaClientes.Columns.Contains("Usuario") && row.Cells["Usuario"].Value != null
                        ? row.Cells["Usuario"].Value.ToString() : "";
                    txtNombre.Text = tablaClientes.Columns.Contains("Nombre") && row.Cells["Nombre"].Value != null
                        ? row.Cells["Nombre"].Value.ToString() : "";
                    txtEmail.Text = tablaClientes.Columns.Contains("Email") && row.Cells["Email"].Value != null
                        ? row.Cells["Email"].Value.ToString() : "";
                    txtTelefono.Text = tablaClientes.Columns.Contains("Telefono") && row.Cells["Telefono"].Value != null
                        ? row.Cells["Telefono"].Value.ToString() : "";

                    // Cargar campos de dirección desde la API
                    // Necesitamos hacer una llamada adicional a la API para obtener los detalles completos
                    var clienteId = clienteIdVal > 0 ? clienteIdVal : usuarioIdVal;
                    if (clienteId > 0)
                    {
                        _ = CargarDireccionClienteAsync(clienteId);
                    }
                    else
                    {
                        // clear address fields if no valid ID
                        txtCalle.Text = "";
                        txtNumero.Text = "";
                        txtCiudad.Text = "";
                        txtEstado.Text = "";
                        txtCodigoPostal.Text = "";
                        txtPais.Text = "";
                    }

                    System.Diagnostics.Debug.WriteLine($"DEBUG: Loaded client - ID: {txtId.Text}, Usuario: {txtUsuario.Text}, Nombre: {txtNombre.Text}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar cliente seleccionado: {ex.Message}\nDetalles: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Clear all fields on error
                    LimpiarFormulario();
                }
            }
        }

        private async Task CargarDireccionClienteAsync(int clienteId)
        {
            try
            {
                // Obtener detalles completos del cliente desde la API
                var clientes = await _apiCliente.ObtenerClientesAsync().ConfigureAwait(false);
                var clienteDetalle = clientes.FirstOrDefault(c => c.cliente_id == clienteId);

                // Address fields removed - no longer needed
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar dirección del cliente: {ex.Message}");
            }
        }

        private void LimpiarFormulario()
        {
            foreach (var control in new TextBox[]
                     {
                         txtId, txtUsuario, txtNombre, txtEmail, txtTelefono,
                         txtCalle, txtNumero, txtCiudad, txtEstado, txtCodigoPostal, txtPais, txtContrasena
                     })
            {
                if (control != null) control.Text = "";
            }
        }

        private async Task GuardarClienteAsync()
        {
            try
            {
                this.BeginInvoke((Action)(() =>
                {
                    if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtEmail.Text))
                    {
                        MessageBox.Show("Usuario, Nombre y Email son campos obligatorios", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }));

                var usuario = new Usuario(
                    idUsuario: string.IsNullOrEmpty(txtId.Text) ? 0 : int.Parse(txtId.Text),
                    nombreUsuario: txtUsuario.Text.Trim(),
                    contraHash: null,
                    salt: null,
                    nombreCompleto: txtNombre.Text.Trim(),
                    email: txtEmail.Text.Trim()
                )
                {
                    Telefono = string.IsNullOrEmpty(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                    Calle = string.IsNullOrEmpty(txtCalle.Text) ? null : txtCalle.Text.Trim(),
                    Numero = string.IsNullOrEmpty(txtNumero.Text) ? null : txtNumero.Text.Trim(),
                    Ciudad = string.IsNullOrEmpty(txtCiudad.Text) ? null : txtCiudad.Text.Trim(),
                    EstadoProvincia = string.IsNullOrEmpty(txtEstado.Text) ? null : txtEstado.Text.Trim(),
                    CodigoPostal = string.IsNullOrEmpty(txtCodigoPostal.Text) ? null : txtCodigoPostal.Text.Trim(),
                    Pais = string.IsNullOrEmpty(txtPais.Text) ? null : txtPais.Text.Trim()
                };

                bool resultado = false;
                if (string.IsNullOrEmpty(txtId.Text))
                {
                    if (string.IsNullOrEmpty(txtContrasena.Text))
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Para crear un nuevo cliente debe ingresar una contraseña", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                        return;
                    }

                    if (await UsuarioExisteAsync(txtUsuario.Text.Trim()).ConfigureAwait(false))
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("El nombre de usuario ya existe. Por favor elija otro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                        return;
                    }

                    if (await EmailExisteAsync(txtEmail.Text.Trim()).ConfigureAwait(false))
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("El email ya está registrado. Por favor use otro email.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                        return;
                    }

                    resultado = await _servicioBancoApi.CrearClienteCompletoAsync(usuario, txtContrasena.Text).ConfigureAwait(false);
                    if (resultado)
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Cliente creado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                        this.BeginInvoke((Action)(() => LimpiarFormulario()));
                    }
                    else
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Error al crear el cliente. Verifique los datos e intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                }
                else
                {
                    usuario.IdUsuario = int.Parse(txtId.Text);
                    resultado = await _servicioBancoApi.ActualizarClienteAsync(usuario).ConfigureAwait(false);

                    if (resultado)
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Cliente actualizado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));

                        if (!string.IsNullOrEmpty(txtContrasena.Text))
                        {
                            if (await _servicioBancoApi.CambiarContrasenaAsync(usuario.IdUsuario, txtContrasena.Text).ConfigureAwait(false))
                            {
                                this.BeginInvoke((Action)(() => MessageBox.Show("Contraseña actualizada exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                                this.BeginInvoke((Action)(() => txtContrasena.Text = ""));
                            }
                            else
                            {
                                this.BeginInvoke((Action)(() => MessageBox.Show("Cliente actualizado pero error al cambiar la contraseña", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                            }
                        }
                    }
                    else
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Error al actualizar el cliente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                }

                if (resultado)
                {
                    await CargarDatosClientes().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al guardar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private async Task<bool> UsuarioExisteAsync(string username)
        {
            try
            {
                var clientes = await _apiCliente.ObtenerClientesAsync().ConfigureAwait(false);
                return clientes.Any(c => c.username != null && c.username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> EmailExisteAsync(string email)
        {
            try
            {
                var clientes = await _apiCliente.ObtenerClientesAsync().ConfigureAwait(false);
                return clientes.Any(c => c.email != null && c.email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private async void EliminarCliente()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int clienteId = int.Parse(txtId.Text);
                string nombreCliente = txtNombre.Text;

                // Confirmación con información detallada
                var resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar al cliente?\n\n" +
                    $"Cliente: {nombreCliente}\n" +
                    $"ID: {clienteId}\n\n" +
                    $"NOTA: Esta acción desactivará el cliente y todas sus cuentas.\n" +
                    $"No se puede eliminar si tiene cuentas con saldo.\n\n" +
                    $"¿Desea continuar?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2
                );

                if (resultado != DialogResult.Yes)
                {
                    return;
                }

                // Mostrar indicador de carga
                this.Cursor = Cursors.WaitCursor;
                btnEliminar.Enabled = false;
                btnEliminar.Text = "Eliminando...";

                await Task.Run(async () =>
                {
                    try
                    {
                        // Usar DELETE con query parameter
                        using (var httpClient = new System.Net.Http.HttpClient())
                        {
                            httpClient.BaseAddress = new Uri("http://localhost:5202");
                            var deleteResponse = await httpClient.DeleteAsync(
                                $"/api/banco/clientes/{clienteId}?usuarioIdAdmin=1"
                            ).ConfigureAwait(false);

                            var responseBody = await deleteResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);

                            this.BeginInvoke((Action)(() =>
                            {
                                this.Cursor = Cursors.Default;
                                btnEliminar.Enabled = true;
                                btnEliminar.Text = "Eliminar";

                                if (deleteResponse.IsSuccessStatusCode && result.success == true)
                                {
                                    MessageBox.Show(
                                        result.message?.ToString() ?? "Cliente eliminado correctamente",
                                        "Éxito",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information
                                    );

                                    // Limpiar formulario
                                    LimpiarFormulario();

                                    // Recargar lista de clientes
                                    Task.Run(async () => await CargarDatosClientes().ConfigureAwait(false));
                                }
                                else
                                {
                                    string errorMsg = result.message?.ToString() ?? "Error desconocido al eliminar cliente";
                                    MessageBox.Show(
                                        errorMsg,
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error
                                    );
                                }
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke((Action)(() =>
                        {
                            this.Cursor = Cursors.Default;
                            btnEliminar.Enabled = true;
                            btnEliminar.Text = "Eliminar";
                            MessageBox.Show(
                                $"Error al eliminar cliente: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                if (btnEliminar != null)
                {
                    btnEliminar.Enabled = true;
                    btnEliminar.Text = "Eliminar";
                }
                MessageBox.Show(
                    $"Error: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async Task CambiarContrasenaAsync()
        {
            if (string.IsNullOrEmpty(txtId.Text) || string.IsNullOrEmpty(txtContrasena.Text))
            {
                MessageBox.Show("Seleccione un cliente e ingrese una nueva contraseña", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (await _servicioBancoApi.CambiarContrasenaAsync(int.Parse(txtId.Text), txtContrasena.Text).ConfigureAwait(false))
                {
                    this.BeginInvoke((Action)(() => MessageBox.Show("Contraseña cambiada exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    this.BeginInvoke((Action)(() => txtContrasena.Text = ""));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar contraseña: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CrearCuentaBancariaAsync()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dlg = new CreateAccountDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    var numeroCuenta = GenerarNumeroCuenta();
                    var clienteId = int.Parse(txtId.Text);
                    var ok = await _servicioBancoApi.CrearCuentaBancariaAsync(clienteId, dlg.SelectedTipo, numeroCuenta, dlg.SaldoInicial).ConfigureAwait(false);
                    if (ok)
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show($"Cuenta creada exitosamente\nNúmero: {numeroCuenta}\nTipo: {dlg.SelectedTipo}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                        await CargarDatosClientes().ConfigureAwait(false);
                    }
                    else
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Error al crear cuenta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke((Action)(() => MessageBox.Show($"Error al crear cuenta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }

        private async void ActualizarSaldoCuenta()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Obtener cuentas del cliente
                int clienteId = int.Parse(txtId.Text);
                var cuentas = await _apiCliente.ObtenerCuentasPorClienteIdAsync(clienteId).ConfigureAwait(false);
                
                if (cuentas == null || cuentas.Count == 0)
                {
                    this.BeginInvoke((Action)(() => MessageBox.Show("Este cliente no tiene cuentas", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    return;
                }

                // Crear formulario de diálogo para actualizar saldo
                this.BeginInvoke((Action)(() =>
                {
                    using (var dialogForm = new Form())
                    {
                        dialogForm.Text = "Actualizar Saldo";
                        dialogForm.Size = new Size(450, 280);
                        dialogForm.StartPosition = FormStartPosition.CenterParent;
                        dialogForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                        dialogForm.MaximizeBox = false;
                        dialogForm.MinimizeBox = false;

                        var tableLayout = new TableLayoutPanel
                        {
                            Dock = DockStyle.Fill,
                            ColumnCount = 2,
                            RowCount = 5,
                            Padding = new Padding(20)
                        };

                        // Cuenta
                        var lblCuenta = new Label { Text = "Cuenta:", AutoSize = true, Anchor = AnchorStyles.Left };
                        var cmbCuentas = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right };
                        foreach (var cuenta in cuentas)
                        {
                            string numeroCuenta = cuenta.ContainsKey("Numero de Cuenta") ? cuenta["Numero de Cuenta"]?.ToString() : cuenta["numero_cuenta"]?.ToString();
                            string tipoCuenta = cuenta.ContainsKey("Tipo de Cuenta") ? cuenta["Tipo de Cuenta"]?.ToString() : cuenta["tipo_cuenta"]?.ToString();
                            decimal saldo = cuenta.ContainsKey("Saldo") ? Convert.ToDecimal(cuenta["Saldo"]) : Convert.ToDecimal(cuenta["saldo"]);
                            cmbCuentas.Items.Add($"{numeroCuenta} - {tipoCuenta} (Saldo actual: ${saldo:N2})");
                        }
                        if (cmbCuentas.Items.Count > 0) cmbCuentas.SelectedIndex = 0;

                        // Nuevo Saldo
                        var lblNuevoSaldo = new Label { Text = "Nuevo Saldo:", AutoSize = true, Anchor = AnchorStyles.Left };
                        var txtNuevoSaldo = new TextBox { Width = 250, Anchor = AnchorStyles.Left | AnchorStyles.Right };
                        txtNuevoSaldo.KeyPress += (s, e) =>
                        {
                            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                                e.Handled = true;
                            if (e.KeyChar == '.' && ((TextBox)s).Text.Contains("."))
                                e.Handled = true;
                        };

                        // Motivo
                        var lblMotivo = new Label { Text = "Motivo:", AutoSize = true, Anchor = AnchorStyles.Left };
                        var txtMotivo = new TextBox { Width = 250, Multiline = true, Height = 60, Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top };

                        // Botones
                        var btnConfirmar = new Button { Text = "Confirmar", DialogResult = DialogResult.OK, Width = 100, Height = 30, BackColor = Color.SlateBlue, ForeColor = Color.White };
                        var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Width = 100, Height = 30 };
                        var panelBotones = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, WrapContents = false };
                        panelBotones.Controls.Add(btnCancelar);
                        panelBotones.Controls.Add(btnConfirmar);

                        // Agregar controles
                        tableLayout.Controls.Add(lblCuenta, 0, 0);
                        tableLayout.Controls.Add(cmbCuentas, 1, 0);
                        tableLayout.Controls.Add(lblNuevoSaldo, 0, 1);
                        tableLayout.Controls.Add(txtNuevoSaldo, 1, 1);
                        tableLayout.Controls.Add(lblMotivo, 0, 2);
                        tableLayout.Controls.Add(txtMotivo, 1, 2);
                        tableLayout.SetRowSpan(lblMotivo, 1);
                        tableLayout.SetRowSpan(txtMotivo, 1);
                        tableLayout.Controls.Add(panelBotones, 1, 4);

                        dialogForm.Controls.Add(tableLayout);
                        dialogForm.AcceptButton = btnConfirmar;
                        dialogForm.CancelButton = btnCancelar;

                        if (dialogForm.ShowDialog() == DialogResult.OK)
                        {
                            if (string.IsNullOrWhiteSpace(txtNuevoSaldo.Text) || string.IsNullOrWhiteSpace(txtMotivo.Text))
                            {
                                MessageBox.Show("Por favor complete todos los campos", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            if (!decimal.TryParse(txtNuevoSaldo.Text, out decimal nuevoSaldo) || nuevoSaldo < 0)
                            {
                                MessageBox.Show("Ingrese un saldo válido (mayor o igual a 0)", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // Extraer número de cuenta de la selección
                            string cuentaSeleccionada = cmbCuentas.SelectedItem.ToString();
                            string numeroCuentaSeleccionada = cuentaSeleccionada.Split(new[] { " - " }, StringSplitOptions.None)[0];

                            // Llamar al método async
                            Task.Run(async () =>
                            {
                                try
                                {
                                    var resultado = await _apiCliente.PostJsonAsync<object, object>(
                                        "/api/banco/cuentas/actualizar-saldo",
                                        new
                                        {
                                            NumeroCuenta = numeroCuentaSeleccionada,
                                            NuevoSaldo = nuevoSaldo,
                                            Motivo = txtMotivo.Text,
                                            UsuarioId = 1 // ID del admin
                                        }
                                    ).ConfigureAwait(false);

                                    this.BeginInvoke((Action)(() =>
                                    {
                                        MessageBox.Show("Saldo actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        // Refrescar datos de clientes
                                        Task.Run(async () => await CargarDatosClientes().ConfigureAwait(false));
                                    }));
                                }
                                catch (Exception ex)
                                {
                                    this.BeginInvoke((Action)(() => MessageBox.Show($"Error al actualizar saldo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                                }
                            });
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tableLayoutAdmin_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AbrirCuentasCliente()
        {
            try
            {
                // Verificar que hay un cliente seleccionado
                if (tablaClientes.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Por favor, seleccione un cliente de la lista primero.",
                        "Selección requerida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var row = tablaClientes.SelectedRows[0];

                // Obtener el ClienteId de las columnas ocultas
                int clienteId = 0;
                if (row.DataBoundItem is DataRowView dataRowView)
                {
                    var dataRow = dataRowView.Row;

                    // Intentar obtener cliente_id primero
                    if (dataRow.Table.Columns.Contains("ClienteId") &&
                        dataRow["ClienteId"] != DBNull.Value)
                    {
                        clienteId = Convert.ToInt32(dataRow["ClienteId"]);
                    }

                    // Si no hay cliente_id válido, usar usuario_id
                    if (clienteId == 0 && dataRow.Table.Columns.Contains("UsuarioId") &&
                        dataRow["UsuarioId"] != DBNull.Value)
                    {
                        clienteId = Convert.ToInt32(dataRow["UsuarioId"]);
                    }
                }

                // Obtener el nombre completo
                var nombreCompleto = row.Cells["Nombre"].Value?.ToString() ?? "Cliente";

                if (clienteId == 0)
                {
                    MessageBox.Show("No se pudo obtener el ID del cliente seleccionado.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Abrir el formulario de cuentas del cliente
                var formCuentas = new CuentasCliente(clienteId, nombreCompleto);
                formCuentas.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir las cuentas del cliente: {ex.Message}\n\nDetalles: {ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DesbloquearUsuarioAsync()
        {
            if (gridUsuariosBloqueados.SelectedRows.Count > 0)
            {
                var usuarioIdCell = gridUsuariosBloqueados.SelectedRows[0].Cells["ID Usuario"];
                if (usuarioIdCell?.Value == null || usuarioIdCell.Value == DBNull.Value)
                {
                    MessageBox.Show("No se puede obtener el ID del usuario seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(usuarioIdCell.Value.ToString(), out var usuarioId))
                {
                    MessageBox.Show("ID de usuario inválido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("¿Está seguro de desbloquear este usuario?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (await _servicioBancoApi.BloquearDesbloquearUsuarioAsync(usuarioId, false).ConfigureAwait(false))
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Usuario desbloqueado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                        await CargarUsuariosBloqueadosAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task CrearAdministradorAsync()
        {
            if (string.IsNullOrEmpty(txtAdminUsername.Text) || string.IsNullOrEmpty(txtAdminPassword.Text))
            {
                MessageBox.Show("Username y password son obligatorios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (await _servicioBancoApi.CrearUsuarioAdministradorAsync(txtAdminUsername.Text, txtAdminPassword.Text, txtAdminNombre.Text, txtAdminEmail.Text).ConfigureAwait(false))
                {
                    this.BeginInvoke((Action)(() => MessageBox.Show("Administrador creado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    this.BeginInvoke((Action)(() => { txtAdminUsername.Text = ""; txtAdminPassword.Text = ""; txtAdminNombre.Text = ""; txtAdminEmail.Text = ""; }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear administrador: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task GenerarReporteTransaccionesAsync()
        {
            try
            {
                var reporte = await _servicioBancoApi.ObtenerReporteTransaccionesPorFechaAsync(dtpFechaInicio.Value, dtpFechaFin.Value).ConfigureAwait(false);
                // El servicio ya devuelve los nombres en español
                this.BeginInvoke((Action)(() => gridReporteTransacciones.DataSource = reporte));
                this.BeginInvoke((Action)(() => MessageBox.Show($"Reporte generado con {reporte.Rows.Count} registros", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() => MessageBox.Show($"Error al generar reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private async Task EjecutarResumenesDiariosAsync()
        {
            if (MessageBox.Show("¿Ejecutar envío de resúmenes diarios manualmente?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (await _servicioBancoApi.EjecutarResumenesDiariosAsync().ConfigureAwait(false))
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Resúmenes diarios ejecutados exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                        await CargarNotificacionesSistemaAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("Error al ejecutar resúmenes diarios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke((Action)(() => MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }

        private string GenerarNumeroCuenta()
        {
            var random = new Random();
            return random.Next(1000000000, int.MaxValue).ToString("D10");
        }

        // New helper: ask API for accounts of selected client and show a ComboBox dialog to pick origin/destination
        private async Task<(bool Success, string CuentaOrigen, string CuentaDestino)> PreguntarTransferenciaEntreCuentasClienteAsync()
        {
            string cuentaOrigen = null;
            string cuentaDestino = null;

            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (false, null, null);
            }

            var clienteId = int.Parse(txtId.Text);
            try
            {
                var cuentas = await _apiCliente.ObtenerCuentasPorClienteAsync(clienteId).ConfigureAwait(false);
                if (cuentas == null || cuentas.Count() == 0)
                {
                    MessageBox.Show("Este cliente no tiene cuentas activas", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return (false, null, null);
                }

                // Build display array
                var items = cuentas.Select(c => new ComboItem
                {
                    Id = c.ContainsKey("ID Cuenta") && c["ID Cuenta"] != null && c["ID Cuenta"] != DBNull.Value ?
                        Convert.ToInt32(c["ID Cuenta"]) : 0,
                    Display = c.ContainsKey("Numero de Cuenta") && c["Numero de Cuenta"] != null ?
                        c["Numero de Cuenta"].ToString() :
                        (c.ContainsKey("ID Cuenta") && c["ID Cuenta"] != null ? c["ID Cuenta"].ToString() : "Sin numero")
                }).ToArray();

                // Ask source using UI thread
                ComboItem src = null;
                ComboItem dest = null;
                this.Invoke((Action)(() =>
                {
                    src = (ComboItem)ComboBoxDialog.Show("Seleccione cuenta origen", "Cuenta Origen", items);
                }));
                if (src == null) return (false, null, null);
                this.Invoke((Action)(() =>
                {
                    dest = (ComboItem)ComboBoxDialog.Show("Seleccione cuenta destino", "Cuenta Destino", items.Where(i => i.Id != src.Id).ToArray());
                }));
                if (dest == null) return (false, null, null);

                cuentaOrigen = src.Display;
                cuentaDestino = dest.Display;
                return (true, cuentaOrigen, cuentaDestino);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener cuentas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false, null, null);
            }
        }

        // Simple combo dialog
        private class ComboItem
        {
            public int Id { get; set; }
            public string Display { get; set; }
            public override string ToString() => Display;
        }

        // Helper para ComboBox de cuentas
        private class CuentaComboItem
        {
            public string NumeroCuenta { get; set; }
            public string Display { get; set; }
            public override string ToString() => Display;
        }

        private static class ComboBoxDialog
        {
            public static object Show(string prompt, string title, ComboItem[] items)
            {
                var form = new Form
                {
                    Text = title,
                    Size = new Size(350, 160),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent
                };

                var label = new Label { Text = prompt, Left = 10, Top = 10, Width = 320 };
                var comboBox = new ComboBox { Left = 10, Top = 40, Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
                comboBox.Items.AddRange(items.Cast<object>().ToArray());
                if (items.Length > 0) comboBox.SelectedIndex = 0;

                var btnOk = new Button { Text = "OK", Left = 170, Top = 80, Width = 75, DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "Cancel", Left = 250, Top = 80, Width = 75, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, comboBox, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                return form.ShowDialog() == DialogResult.OK ? comboBox.SelectedItem : null;
            }
        }

        // Dialog for creating account
        private class CreateAccountDialog : Form
        {
            public Cuenta.TipoCuenta SelectedTipo { get; private set; }
            public double SaldoInicial { get; private set; }

            private ComboBox cmbTipo;
            private TextBox txtSaldo;
            private Button btnOk;
            private Button btnCancel;

            public CreateAccountDialog()
            {
                Text = "Crear Cuenta";
                Size = new Size(400, 180);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                StartPosition = FormStartPosition.CenterParent;

                var lblTipo = new Label { Text = "Tipo de cuenta:", Left = 10, Top = 20, Width = 100 };
                cmbTipo = new ComboBox { Left = 120, Top = 16, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbTipo.Items.AddRange(Enum.GetNames(typeof(Cuenta.TipoCuenta)));
                cmbTipo.SelectedIndex = 0;

                var lblSaldo = new Label { Text = "Saldo inicial:", Left = 10, Top = 60, Width = 100 };
                txtSaldo = new TextBox { Left = 120, Top = 56, Width = 240, Text = "0" };

                btnOk = new Button { Text = "Crear", Left = 200, Top = 100, Width = 75, DialogResult = DialogResult.OK };
                btnCancel = new Button { Text = "Cancelar", Left = 285, Top = 100, Width = 75, DialogResult = DialogResult.Cancel };

                Controls.AddRange(new Control[] { lblTipo, cmbTipo, lblSaldo, txtSaldo, btnOk, btnCancel });

                btnOk.Click += (s, e) =>
                {
                    if (!double.TryParse(txtSaldo.Text, out var sdo) || sdo < 0)
                    {
                        MessageBox.Show("Saldo inválido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.DialogResult = DialogResult.None;
                        return;
                    }

                    SelectedTipo = (Cuenta.TipoCuenta)Enum.Parse(typeof(Cuenta.TipoCuenta), cmbTipo.SelectedItem.ToString());
                    SaldoInicial = sdo;
                };
            }
        }

        public static class InputBox
        {
            public static object Show(string prompt, string title, object[] items)
            {
                var form = new Form
                {
                    Text = title,
                    Size = new Size(300, 150),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent
                };

                var label = new Label { Text = prompt, Left = 10, Top = 20, Width = 280 };
                var comboBox = new ComboBox
                { Left = 10, Top = 50, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
                comboBox.Items.AddRange(items);
                if (items.Length > 0) comboBox.SelectedIndex = 0;

                var btnOk = new Button { Text = "OK", Left = 130, Top = 80, Width = 75, DialogResult = DialogResult.OK };
                var btnCancel = new Button
                { Text = "Cancel", Left = 210, Top = 80, Width = 75, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, comboBox, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                return form.ShowDialog() == DialogResult.OK ? comboBox.SelectedItem : null;
            }
        }

        private void LimpiarDatosSesion()
        {
            try
            {
                // Limpiar cualquier dato sensible en memoria
                if (tablaClientes?.DataSource != null)
                {
                    tablaClientes.DataSource = null;
                }

                // Limpiar otras estructuras de datos si existen
                System.Diagnostics.Debug.WriteLine("INFO: Datos de sesión limpiados correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Error limpiando datos de sesión: {ex.Message}");
            }
        }

        // Fix for the conversion issue - convert the ApiAdmin.ClienteDto collection to our DTO collection
        private async void CargarClientes()
        {
            try
            {
                var clientes = await _apiCliente.ObtenerClientesAsync();
                if (clientes != null)
                {
                    // Convert ApiAdmin.ClienteDto to our DTOs.ClienteDto
                    var clientesDto = clientes.Select(c => new ProyectoFinal_Software_4_y_5.Entidades.DTOs.ClienteDto
                    {
                        usuario_id = c.usuario_id,
                        cliente_id = c.cliente_id,
                        username = c.username,
                        nombre_completo = c.nombre_completo,
                        email = c.email,
                        telefono = c.telefono,
                        es_admin = c.es_admin,
                        bloqueado = c.bloqueado
                    }).ToList();

                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke((Action)(() => PopulateClientesGridFromDto(clientesDto)));
                    }
                    else
                    {
                        PopulateClientesGridFromDto(clientesDto);
                    }
                }
                else
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke((Action)(() => MessageBox.Show("No se pudieron cargar los clientes.", "Información")));
                    }
                    else
                    {
                        MessageBox.Show("No se pudieron cargar los clientes.", "Información");
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((Action)(() => MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
                else
                {
                    MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Los nombres de columnas ahora se manejan centralizadamente en ServicioBancoApi
    }
}