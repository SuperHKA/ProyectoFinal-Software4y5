using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProyectoFinal_Software_4_y_5.Entidades;
using ProyectoFinal_Software_4_y_5.Servicios;
using ProyectoFinal_Software_4_y_5.Forms; // ensure ApiCliente visible (namespace may vary)

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

        public PanelAdministrativo()
        {
            try
            {
                InitializeComponent();

                // API health check (replace local DB diagnostic)
                try
                {
                    var apiClient = new ApiCliente("http://localhost:5202", allowInvalidSsl: false);
                    var healthy = apiClient.HealthAsync().GetAwaiter().GetResult();
                    if (!healthy)
                    {
                        MessageBox.Show("Warning: API health check failed. UI will run in degraded mode and features requiring the API may fail.",
                            "API Unreachable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception hx)
                {
                    // Non-fatal: show a warning, continue into UI (calls will fail until API is up)
                    MessageBox.Show($"Warning: could not reach API: {hx.Message}\nThe application will continue but some features may not work.",
                        "API Unreachable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Use API adapter (single instance)
                _servicioBancoApi = new ServicioBancoApi("http://localhost:5202", allowInvalidSsl: false);

                // Ejecutar diagnóstico completo (now uses API)
                EjecutarDiagnosticoCompleto();

                // Crear formulario de cliente dinámicamente
                CrearFormularioCliente();

                // Conectar eventos
                ConectarEventos();

                // CARGAR DATOS INICIALES EN UI
                try
                {
                    CargarDatosClientes();
                    CargarDashboard();
                    CargarVistas();
                    CargarUsuariosBloqueados();
                    CargarNotificacionesSistema();
                }
                catch (Exception loadEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[PanelAdministrativo] Error cargando datos iniciales: {loadEx.Message}");
                    MessageBox.Show("Hubo un problema cargando datos iniciales. Revisa el Output de Debug para más detalles.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico al inicializar el panel: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EjecutarDiagnosticoCompleto()
        {
            try
            {
                // 1. Probar consulta básica (not available on API adapter) — skip or call API health if present.
                string resultadoConsulta = "API health not checked here.";

                // 2. Diagnosticar tablas (not available in API adapter signature) — skip local DB checks if removed.
                string mensajeTablas = "Using API service; local DB diagnostics omitted.";

                // 3. Diagnosticar vistas
                string mensajeVistas = "";

                // 4. Contar clientes via API
                var clientes = _servicioBancoApi.ObtenerTodosLosClientes();
                string mensajeRegistros = $"\nREGISTROS:\n• Clientes: {clientes?.Count ?? 0}";

                MessageBox.Show($"{resultadoConsulta}\n\n{mensajeTablas}{mensajeVistas}{mensajeRegistros}",
                    "Diagnóstico Completo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en diagnóstico: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatosClientes()
        {
            try
            {
                var clientes = _servicioBancoApi.ObtenerTodosLosClientes();

                if (clientes == null)
                {
                    MessageBox.Show("Error: La lista de clientes es nula", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (clientes.Count == 0)
                {
                    MessageBox.Show("No se encontraron clientes en la base de datos", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                var dt = new DataTable();
                dt.Columns.Add("Error");
                dt.Rows.Add($"Error: {ex.Message}");
                tablaClientes.DataSource = dt;
            }
        }

        private void CrearFormularioCliente()
        {
            var panel = panelFormularioCliente;
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
            btnGuardar.Click += (s, e) => GuardarCliente();
            btnEliminar.Click += (s, e) => EliminarCliente();
            btnActualizar.Click += (s, e) => CargarDatosClientes();
            btnCambiarContrasena.Click += (s, e) => CambiarContrasena();
            btnCrearCuenta.Click += (s, e) => CrearCuentaBancaria();
            btnActualizarSaldo.Click += (s, e) => ActualizarSaldoCuenta();

            btnDesbloquearUsuario.Click += (s, e) => DesbloquearUsuario();
            btnCrearAdministrador.Click += (s, e) => CrearAdministrador();

            btnGenerarReporte.Click += (s, e) => GenerarReporteTransacciones();

            btnEjecutarResumenes.Click += (s, e) => EjecutarResumenesDiarios();
        }

        private void CargarDashboard()
        {
            try
            {
                var stats = _servicioBancoApi.ObtenerEstadisticasGenerales();
                if (stats.Rows.Count > 0)
                {
                    var row = stats.Rows[0];
                    lblTotalClientes.Text = row["total_clientes"].ToString();
                    lblTotalCuentas.Text = row["total_cuentas"].ToString();
                    lblSaldoTotal.Text = $"${Convert.ToDecimal(row["saldo_total_sistema"]):N2}";
                    lblTransaccionesHoy.Text = row["transacciones_hoy"].ToString();
                }

                gridTransaccionesRecientes.DataSource = _servicioBancoApi.ObtenerTransaccionesRecientes(20);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar dashboard: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarVistas()
        {
            try
            {
                tablaMovimientos.DataSource = _servicioBancoApi.ObtenerMovimientosDelDia();
                tablaAnalisis.DataSource = _servicioBancoApi.ObtenerAnalisisTransacciones();
                tablaResumen.DataSource = _servicioBancoApi.ObtenerResumenClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar vistas: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarUsuariosBloqueados()
        {
            try
            {
                gridUsuariosBloqueados.DataSource = _servicioBancoApi.ObtenerUsuariosBloqueados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios bloqueados: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarNotificacionesSistema()
        {
            try
            {
                gridNotificacionesSistema.DataSource = _servicioBancoApi.ObtenerNotificacionesSistema();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar notificaciones: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarClienteSeleccionado()
        {
            if (tablaClientes.SelectedRows.Count > 0)
            {
                var row = tablaClientes.SelectedRows[0];
                txtId.Text = row.Cells["ID"].Value?.ToString() ?? "";
                txtUsuario.Text = row.Cells["Usuario"].Value?.ToString() ?? "";
                txtNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                txtTelefono.Text = row.Cells["Teléfono"].Value?.ToString() ?? "";
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
                control.Text = "";
            }
        }

        private void GuardarCliente()
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtNombre.Text) ||
                string.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Usuario, Nombre y Email son campos obligatorios", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!EsEmailValido(txtEmail.Text))
            {
                MessageBox.Show("El formato del email no es válido", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
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

                bool resultado;

                if (string.IsNullOrEmpty(txtId.Text))
                {
                    if (string.IsNullOrEmpty(txtContrasena.Text))
                    {
                        MessageBox.Show("Para crear un nuevo cliente debe ingresar una contraseña", "Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (UsuarioExiste(txtUsuario.Text.Trim()))
                    {
                        MessageBox.Show("El nombre de usuario ya existe. Por favor elija otro.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (EmailExiste(txtEmail.Text.Trim()))
                    {
                        MessageBox.Show("El email ya está registrado. Por favor use otro email.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    resultado = _servicioBancoApi.CrearClienteCompleto(usuario, txtContrasena.Text);

                    if (resultado)
                    {
                        MessageBox.Show("Cliente creado exitosamente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarFormulario();
                    }
                    else
                    {
                        MessageBox.Show("Error al crear el cliente. Verifique los datos e intente nuevamente.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    usuario.IdUsuario = int.Parse(txtId.Text);

                    resultado = _servicioBancoApi.ActualizarCliente(usuario);

                    if (resultado)
                    {
                        MessageBox.Show("Cliente actualizado exitosamente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (!string.IsNullOrEmpty(txtContrasena.Text))
                        {
                            if (_servicioBancoApi.CambiarContrasena(usuario.IdUsuario, txtContrasena.Text))
                            {
                                MessageBox.Show("Contraseña actualizada exitosamente", "Éxito",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtContrasena.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("Cliente actualizado pero error al cambiar la contraseña",
                                    "Advertencia",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar el cliente", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (resultado)
                {
                    CargarDatosClientes();
                }

            }
            catch (FormatException)
            {
                MessageBox.Show(
                    "Error en el formato de los datos ingresados. Verifique que todos los campos tengan el formato correcto.",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error de base de datos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cliente: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool EsEmailValido(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool UsuarioExiste(string username)
        {
            try
            {
                var clientes = _servicioBancoApi.ObtenerTodosLosClientes();
                return clientes.Any(c =>
                    c.NombreUsuario != null &&
                    c.NombreUsuario.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private bool EmailExiste(string email)
        {
            try
            {
                var clientes = _servicioBancoApi.ObtenerTodosLosClientes();
                return clientes.Any(c =>
                    c.Email != null &&
                    c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
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
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este cliente?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MessageBox.Show("Funcionalidad de eliminación en desarrollo", "Info", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void CambiarContrasena()
        {
            if (string.IsNullOrEmpty(txtId.Text) || string.IsNullOrEmpty(txtContrasena.Text))
            {
                MessageBox.Show("Seleccione un cliente e ingrese una nueva contraseña", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_servicioBancoApi.CambiarContrasena(int.Parse(txtId.Text), txtContrasena.Text))
                {
                    MessageBox.Show("Contraseña cambiada exitosamente", "Éxito", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    txtContrasena.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar contraseña: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CrearCuentaBancaria()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var tipos = Enum.GetValues(typeof(Cuenta.TipoCuenta)).Cast<object>().ToArray();
            var tipoSeleccionadoObj = InputBox.Show("Seleccione tipo de cuenta", "Crear Cuenta", tipos);
            if (tipoSeleccionadoObj != null)
            {
                var tipoSeleccionado = (Cuenta.TipoCuenta)tipoSeleccionadoObj;
                try
                {
                    var numeroCuenta = GenerarNumeroCuenta();
                    if (_servicioBancoApi.CrearCuentaBancaria(int.Parse(txtId.Text), tipoSeleccionado, numeroCuenta, 0))
                    {
                        MessageBox.Show($"Cuenta creada exitosamente\nNúmero: {numeroCuenta}\nTipo: {tipoSeleccionado}",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al crear cuenta: {ex.Message}", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void ActualizarSaldoCuenta()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Funcionalidad de actualizar saldo en desarrollo", "Info", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void DesbloquearUsuario()
        {
            if (gridUsuariosBloqueados.SelectedRows.Count > 0)
            {
                var usuarioId = Convert.ToInt32(gridUsuariosBloqueados.SelectedRows[0].Cells["usuario_id"].Value);

                if (MessageBox.Show("¿Está seguro de desbloquear este usuario?", "Confirmar",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_servicioBancoApi.BloquearDesbloquearUsuario(usuarioId, false))
                    {
                        MessageBox.Show("Usuario desbloqueado exitosamente", "Éxito", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        CargarUsuariosBloqueados();
                    }
                }
            }
        }

        private void CrearAdministrador()
        {
            if (string.IsNullOrEmpty(txtAdminUsername.Text) || string.IsNullOrEmpty(txtAdminPassword.Text))
            {
                MessageBox.Show("Username y password son obligatorios", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_servicioBancoApi.CrearUsuarioAdministrador(
                        txtAdminUsername.Text,
                        txtAdminPassword.Text,
                        txtAdminNombre.Text,
                        txtAdminEmail.Text))
                {
                    MessageBox.Show("Administrador creado exitosamente", "Éxito", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    txtAdminUsername.Text = "";
                    txtAdminPassword.Text = "";
                    txtAdminNombre.Text = "";
                    txtAdminEmail.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear administrador: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void GenerarReporteTransacciones()
        {
            try
            {
                var reporte = _servicioBancoApi.ObtenerReporteTransaccionesPorFecha(
                    dtpFechaInicio.Value,
                    dtpFechaFin.Value);

                gridReporteTransacciones.DataSource = reporte;

                MessageBox.Show($"Reporte generado con {reporte.Rows.Count} registros", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar reporte: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void EjecutarResumenesDiarios()
        {
            if (MessageBox.Show("¿Ejecutar envío de resúmenes diarios manualmente?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_servicioBancoApi.EjecutarResumenesDiarios())
                    {
                        MessageBox.Show("Resúmenes diarios ejecutados exitosamente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarNotificacionesSistema();
                    }
                    else
                    {
                        MessageBox.Show("Error al ejecutar resúmenes diarios", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GenerarNumeroCuenta()
        {
            var random = new Random();
            return random.Next(1000000000, int.MaxValue).ToString("D10");
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