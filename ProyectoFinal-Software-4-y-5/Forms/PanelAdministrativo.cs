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

        // Use the API adapter exclusively
        private readonly ServicioBancoApi _servicioBancoApi;
        private readonly ApiAdmin _apiCliente;

        public PanelAdministrativo()
        {
            try
            {
                InitializeComponent();

                // Use API adapter (single instance)
                _apiCliente = new ApiAdmin("http://localhost:5202", allowInvalidSsl: false);
                _servicioBancoApi = new ServicioBancoApi("http://localhost:5202", allowInvalidSsl: false);

                // Crear formulario de cliente dinámicamente
                CrearFormularioCliente();

                // Conectar eventos
                ConectarEventos();

                // Defer heavy I/O to form load to keep constructor fast
                this.Load += PanelAdministrativo_Load;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico al inicializar el panel: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        // populate UI with results
                        PopulateClientesGridFromDto(clientes);
                        PopulateDashboard(stats, trans);
                        tablaMovimientos.DataSource = movimientos;
                        // Use async results for small views
                        tablaAnalisis.DataSource = _servicioBancoApi.ObtenerAnalisisTransaccionesAsync().GetAwaiter().GetResult();
                        tablaResumen.DataSource = _servicioBancoApi.ObtenerResumenClientesAsync().GetAwaiter().GetResult();
                        gridUsuariosBloqueados.DataSource = usuariosBloqueados;
                        gridNotificacionesSistema.DataSource = notifs;
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
                dt.Columns.Add("Teléfono");
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

        private void PopulateClientesGridFromDto(System.Collections.Generic.IEnumerable<ApiAdmin.ClienteDto> clientes)
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
                dt.Columns.Add("Teléfono");
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
                    row["Teléfono"] = cliente.telefono ?? "N/A";
                    row["Estado"] = cliente.bloqueado ? "Bloqueado" : "Activo";
                    dt.Rows.Add(row);
                }

                tablaClientes.DataSource = dt;

                // hide helper columns from view
                if (tablaClientes.Columns.Contains("UsuarioId")) tablaClientes.Columns["UsuarioId"].Visible = false;
                if (tablaClientes.Columns.Contains("ClienteId")) tablaClientes.Columns["ClienteId"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al poblar lista de clientes (DTO): {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDashboard(DataTable stats, DataTable trans)
        {
            try
            {
                if (stats != null && stats.Rows.Count > 0)
                {
                    var row = stats.Rows[0];
                    lblTotalClientes.Text = row["total_clientes"].ToString();
                    lblTotalCuentas.Text = row["total_cuentas"].ToString();
                    lblSaldoTotal.Text = $"${Convert.ToDecimal(row["saldo_total_sistema"]):N2}";
                    lblTransaccionesHoy.Text = row["transacciones_hoy"].ToString();
                }

                gridTransaccionesRecientes.DataSource = trans;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al poblar dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarDatosClientes()
        {
            try
            {
                var clientes = await _servicioBancoApi.ObtenerTodosLosClientesAsync().ConfigureAwait(false);
                this.BeginInvoke((Action)(() => PopulateClientesGrid(clientes)));
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
            AgregarControl(tableLayout, "ID:", txtId = new TextBox { ReadOnly = true }, row++);
            AgregarControl(tableLayout, "Usuario:", txtUsuario = new TextBox(), row++);
            AgregarControl(tableLayout, "Nombre:", txtNombre = new TextBox(), row++);
            AgregarControl(tableLayout, "Email:", txtEmail = new TextBox(), row++);
            AgregarControl(tableLayout, "Teléfono:", txtTelefono = new TextBox(), row++);

            tableLayout.Controls.Add(new Label { Text = "Dirección", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, row++);
            tableLayout.SetColumnSpan(tableLayout.Controls[tableLayout.Controls.Count - 1], 2);

            AgregarControl(tableLayout, "Calle:", txtCalle = new TextBox(), row++);
            AgregarControl(tableLayout, "Número:", txtNumero = new TextBox(), row++);
            AgregarControl(tableLayout, "Ciudad:", txtCiudad = new TextBox(), row++);
            AgregarControl(tableLayout, "Estado:", txtEstado = new TextBox(), row++);
            AgregarControl(tableLayout, "Código Postal:", txtCodigoPostal = new TextBox(), row++);
            AgregarControl(tableLayout, "País:", txtPais = new TextBox(), row++);

            AgregarControl(tableLayout, "Contraseña:", txtContrasena = new TextBox { UseSystemPasswordChar = true }, row++);

            var panelBotones = CrearPanelBotones();
            tableLayout.Controls.Add(panelBotones, 0, row++);
            tableLayout.SetColumnSpan(panelBotones, 2);

            panel.Controls.Add(tableLayout);
        }

        private void AgregarControl(TableLayoutPanel tableLayout, string labelText, Control control, int row)
        {
            tableLayout.Controls.Add(new Label { Text = labelText, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            tableLayout.Controls.Add(control, 1, row);
        }

        private Panel CrearPanelBotones()
        {
            var panel = new Panel { Height = 150, Dock = DockStyle.Top };
            var flowLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };

            btnNuevo = new Button { Text = "Nuevo", Size = new Size(100, 30) };
            btnGuardar = new Button { Text = "Guardar", Size = new Size(100, 30) };
            btnEliminar = new Button { Text = "Eliminar", Size = new Size(100, 30) };
            btnActualizar = new Button { Text = "Actualizar", Size = new Size(100, 30) };
            btnCambiarContrasena = new Button { Text = "Cambiar Contraseña", Size = new Size(120, 30) };
            btnCrearCuenta = new Button { Text = "Crear Cuenta", Size = new Size(100, 30) };
            btnActualizarSaldo = new Button { Text = "Actualizar Saldo", Size = new Size(120, 30) };

            flowLayout.Controls.AddRange(new Control[]
            {
                btnNuevo, btnGuardar, btnEliminar, btnActualizar,
                btnCambiarContrasena, btnCrearCuenta, btnActualizarSaldo
            });

            panel.Controls.Add(flowLayout);
            return panel;
        }

        private void ConectarEventos()
        {
            tablaClientes.SelectionChanged += (s, e) => CargarClienteSeleccionado();
            btnNuevo.Click += (s, e) => LimpiarFormulario();
            btnGuardar.Click += async (s, e) => await GuardarClienteAsync();
            btnEliminar.Click += (s, e) => EliminarCliente();
            btnActualizar.Click += async (s, e) => await CargarDatosClientes();
            btnCambiarContrasena.Click += async (s, e) => await CambiarContrasenaAsync();
            btnCrearCuenta.Click += async (s, e) => await CrearCuentaBancariaAsync();
            btnActualizarSaldo.Click += (s, e) => ActualizarSaldoCuenta();

            btnDesbloquearUsuario.Click += async (s, e) => await DesbloquearUsuarioAsync();
            btnCrearAdministrador.Click += async (s, e) => await CrearAdministradorAsync();

            btnGenerarReporte.Click += async (s, e) => await GenerarReporteTransaccionesAsync();

            btnEjecutarResumenes.Click += async (s, e) => await EjecutarResumenesDiariosAsync();
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
                        lblTotalClientes.Text = row["total_clientes"].ToString();
                        lblTotalCuentas.Text = row["total_cuentas"].ToString();
                        lblSaldoTotal.Text = $"${Convert.ToDecimal(row["saldo_total_sistema"]):N2}";
                        lblTransaccionesHoy.Text = row["transacciones_hoy"].ToString();
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

        private void CargarClienteSeleccionado()
        {
            if (tablaClientes.SelectedRows.Count > 0)
            {
                var row = tablaClientes.SelectedRows[0];
                
                try
                {
                    // txtId should hold cliente_id (for account creation) - read from hidden ClienteId column
                    var clienteIdObj = row.Cells["ClienteId"].Value;
                    var usuarioIdObj = row.Cells["UsuarioId"].Value;
                    
                    // Use ClienteId if available and > 0, otherwise use UsuarioId
                    if (clienteIdObj != null && clienteIdObj != DBNull.Value && 
                        int.TryParse(clienteIdObj.ToString(), out var cid) && cid > 0)
                    {
                        txtId.Text = cid.ToString();
                    }
                    else if (usuarioIdObj != null && usuarioIdObj != DBNull.Value)
                    {
                        txtId.Text = usuarioIdObj.ToString();
                    }
                    else
                    {
                        txtId.Text = "";
                    }

                    txtUsuario.Text = row.Cells["Usuario"].Value?.ToString() ?? "";
                    txtNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
                    txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                    txtTelefono.Text = row.Cells["Teléfono"].Value?.ToString() ?? "";

                    // clear address fields -- API DTO does not include address in current contract
                    txtCalle.Text = "";
                    txtNumero.Text = "";
                    txtCiudad.Text = "";
                    txtEstado.Text = "";
                    txtCodigoPostal.Text = "";
                    txtPais.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar cliente seleccionado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Clear all fields on error
                    LimpiarFormulario();
                }
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
                var clientes = await _servicioBancoApi.ObtenerTodosLosClientesAsync().ConfigureAwait(false);
                return clientes.Any(c => c.NombreUsuario != null && c.NombreUsuario.Equals(username, StringComparison.OrdinalIgnoreCase));
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
                var clientes = await _servicioBancoApi.ObtenerTodosLosClientesAsync().ConfigureAwait(false);
                return clientes.Any(c => c.Email != null && c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private void EliminarCliente()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este cliente?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MessageBox.Show("Funcionalidad de eliminación en desarrollo", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void ActualizarSaldoCuenta()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Funcionalidad de actualizar saldo en desarrollo", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task DesbloquearUsuarioAsync()
        {
            if (gridUsuariosBloqueados.SelectedRows.Count > 0)
            {
                var usuarioIdCell = gridUsuariosBloqueados.SelectedRows[0].Cells["usuario_id"];
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
                    Id = c.ContainsKey("cuenta_id") && c["cuenta_id"] != null && c["cuenta_id"] != DBNull.Value ? 
                        Convert.ToInt32(c["cuenta_id"]) : 0,
                    Display = c.ContainsKey("numero_cuenta") && c["numero_cuenta"] != null ? 
                        c["numero_cuenta"].ToString() : 
                        (c.ContainsKey("cuenta_id") && c["cuenta_id"] != null ? c["cuenta_id"].ToString() : "Sin número")
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
    }
}