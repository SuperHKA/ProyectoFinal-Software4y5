using ProyectoFinal_Software_4_y_5.Entidades;
using ProyectoFinal_Software_4_y_5.Servicios;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public class PanelAdmin : Form
    {
        private TabControl tabControl;
        private DataGridView tablaClientes;
        private DataGridView tablaMovimientos;
        private DataGridView tablaAnalisis;
        private DataGridView tablaResumen;

        private TextBox txtId, txtUsuario, txtNombre, txtEmail, txtTelefono;
        private TextBox txtCalle, txtNumero, txtCiudad, txtEstado, txtCodigoPostal, txtPais;
        private TextBox txtContrasena;

        private Button btnNuevo, btnGuardar, btnEliminar, btnActualizar;
        private Button btnCambiarContrasena, btnCrearCuenta, btnActualizarSaldo;

        private readonly ServicioBanco _servicioBanco;

        public PanelAdmin()
        {
            _servicioBanco = new ServicioBanco();
            InitializeComponent();
            CargarDatosClientes();
            CargarVistas();
        }

        private void InitializeComponent()
        {
            // Configuración básica del formulario
            Text = "Banco Dinamo 24/7 - Panel de Administración";
            Size = new Size(1200, 700);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;

            // TabControl principal
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Size = new Size(1200, 700)
            };

            // Crear pestañas
            tabControl.TabPages.Add(CrearTabClientes());
            tabControl.TabPages.Add(CrearTabMovimientos());
            tabControl.TabPages.Add(CrearTabAnalisis());
            tabControl.TabPages.Add(CrearTabResumen());

            Controls.Add(tabControl);
        }

        private TabPage CrearTabClientes()
        {
            var tabPage = new TabPage("Gestión de Clientes");
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 600
            };

            // Panel izquierdo - Tabla de clientes
            tablaClientes = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tablaClientes.SelectionChanged += (s, e) => CargarClienteSeleccionado();

            // Panel derecho - Formulario de cliente
            var panelFormulario = CrearPanelFormularioCliente();

            splitContainer.Panel1.Controls.Add(tablaClientes);
            splitContainer.Panel2.Controls.Add(panelFormulario);
            tabPage.Controls.Add(splitContainer);

            return tabPage;
        }

        private Panel CrearPanelFormularioCliente()
        {
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 15,
                Padding = new Padding(10)
            };

            // Configurar columnas
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            // Crear controles
            int row = 0;
            AgregarControl(tableLayout, "ID:", txtId = new TextBox { ReadOnly = true }, row++);
            AgregarControl(tableLayout, "Usuario:", txtUsuario = new TextBox(), row++);
            AgregarControl(tableLayout, "Nombre:", txtNombre = new TextBox(), row++);
            AgregarControl(tableLayout, "Email:", txtEmail = new TextBox(), row++);
            AgregarControl(tableLayout, "Teléfono:", txtTelefono = new TextBox(), row++);

            // Separador dirección
            tableLayout.Controls.Add(new Label { Text = "Dirección", Font = new Font("Arial", 9, FontStyle.Bold) }, 0, row++);
            tableLayout.SetColumnSpan(tableLayout.Controls[tableLayout.Controls.Count - 1], 2);

            AgregarControl(tableLayout, "Calle:", txtCalle = new TextBox(), row++);
            AgregarControl(tableLayout, "Número:", txtNumero = new TextBox(), row++);
            AgregarControl(tableLayout, "Ciudad:", txtCiudad = new TextBox(), row++);
            AgregarControl(tableLayout, "Estado:", txtEstado = new TextBox(), row++);
            AgregarControl(tableLayout, "Código Postal:", txtCodigoPostal = new TextBox(), row++);
            AgregarControl(tableLayout, "País:", txtPais = new TextBox(), row++);

            AgregarControl(tableLayout, "Contraseña:", txtContrasena = new TextBox { UseSystemPasswordChar = true }, row++);

            // Panel de botones
            var panelBotones = CrearPanelBotones();
            tableLayout.Controls.Add(panelBotones, 0, row++);
            tableLayout.SetColumnSpan(panelBotones, 2);

            panel.Controls.Add(tableLayout);
            return panel;
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

            // Asignar eventos
            btnNuevo.Click += (s, e) => LimpiarFormulario();
            btnGuardar.Click += (s, e) => GuardarCliente();
            btnEliminar.Click += (s, e) => EliminarCliente();
            btnActualizar.Click += (s, e) => CargarDatosClientes();
            btnCambiarContrasena.Click += (s, e) => CambiarContrasena();
            btnCrearCuenta.Click += (s, e) => CrearCuentaBancaria();
            btnActualizarSaldo.Click += (s, e) => ActualizarSaldoCuenta();

            flowLayout.Controls.AddRange(new Control[] {
                btnNuevo, btnGuardar, btnEliminar, btnActualizar,
                btnCambiarContrasena, btnCrearCuenta, btnActualizarSaldo
            });

            panel.Controls.Add(flowLayout);
            return panel;
        }

        private TabPage CrearTabMovimientos()
        {
            var tabPage = new TabPage("Movimientos del Día");
            tablaMovimientos = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true };
            tabPage.Controls.Add(tablaMovimientos);
            return tabPage;
        }

        private TabPage CrearTabAnalisis()
        {
            var tabPage = new TabPage("Análisis de Transacciones");
            tablaAnalisis = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true };
            tabPage.Controls.Add(tablaAnalisis);
            return tabPage;
        }

        private TabPage CrearTabResumen()
        {
            var tabPage = new TabPage("Resumen de Clientes");
            tablaResumen = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true };
            tabPage.Controls.Add(tablaResumen);
            return tabPage;
        }

        // Métodos de carga de datos
        private void CargarDatosClientes()
        {
            try
            {
                var clientes = _servicioBanco.ObtenerTodosLosClientes();
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
                        cliente.NombreUsuario,
                        cliente.NombreCompleto,
                        cliente.Email,
                        cliente.Telefono,
                        cliente.EstadoCuenta ? "Bloqueado" : "Activo"
                    );
                }

                tablaClientes.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarVistas()
        {
            try
            {
                tablaMovimientos.DataSource = _servicioBanco.ObtenerMovimientosDelDia();
                tablaAnalisis.DataSource = _servicioBanco.ObtenerAnalisisTransacciones();
                tablaResumen.DataSource = _servicioBanco.ObtenerResumenClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar vistas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarClienteSeleccionado()
        {
            if (tablaClientes.SelectedRows.Count > 0)
            {
                var row = tablaClientes.SelectedRows[0];
                txtId.Text = row.Cells["ID"].Value.ToString();
                txtUsuario.Text = row.Cells["Usuario"].Value.ToString();
                txtNombre.Text = row.Cells["Nombre"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                txtTelefono.Text = row.Cells["Teléfono"].Value.ToString();
            }
        }

        private void LimpiarFormulario()
        {
            foreach (var control in new TextBox[] { txtId, txtUsuario, txtNombre, txtEmail, txtTelefono,
                txtCalle, txtNumero, txtCiudad, txtEstado, txtCodigoPostal, txtPais, txtContrasena })
            {
                control.Text = "";
            }
        }

        private void GuardarCliente()
        {
            // Implementar lógica de guardado
            MessageBox.Show("Funcionalidad de guardado en desarrollo", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EliminarCliente()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este cliente?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Implementar eliminación
                MessageBox.Show("Funcionalidad de eliminación en desarrollo", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CambiarContrasena()
        {
            if (string.IsNullOrEmpty(txtId.Text) || string.IsNullOrEmpty(txtContrasena.Text))
            {
                MessageBox.Show("Seleccione un cliente e ingrese una nueva contraseña", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_servicioBanco.CambiarContrasena(int.Parse(txtId.Text), txtContrasena.Text))
                {
                    MessageBox.Show("Contraseña cambiada exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtContrasena.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar contraseña: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearCuentaBancaria()
        {
            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Seleccione un cliente primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Diálogo para seleccionar tipo de cuenta
            var tipos = Enum.GetValues(typeof(Cuenta.TipoCuenta)).Cast<object>().ToArray();
            var tipoSeleccionadoObj = InputBox.Show("Seleccione tipo de cuenta", "Crear Cuenta", tipos);
            if (tipoSeleccionadoObj != null)
            {
                var tipoSeleccionado = (Cuenta.TipoCuenta)tipoSeleccionadoObj;
                try
                {
                    var numeroCuenta = GenerarNumeroCuenta();
                    if (_servicioBanco.CrearCuentaBancaria(int.Parse(txtId.Text), tipoSeleccionado, numeroCuenta, 0))
                    {
                        MessageBox.Show($"Cuenta creada exitosamente\nNúmero: {numeroCuenta}\nTipo: {tipoSeleccionado}",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al crear cuenta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Implementar diálogo para actualizar saldo
            MessageBox.Show("Funcionalidad de actualizar saldo en desarrollo", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerarNumeroCuenta()
        {
            var random = new Random();
            return random.Next(1000000000, int.MaxValue).ToString("D10");
        }
    }

    // Clase auxiliar para diálogos de entrada
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
            var comboBox = new ComboBox { Left = 10, Top = 50, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            comboBox.Items.AddRange(items);
            if (items.Length > 0) comboBox.SelectedIndex = 0;

            var btnOk = new Button { Text = "OK", Left = 130, Top = 80, Width = 75, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 210, Top = 80, Width = 75, DialogResult = DialogResult.Cancel };

            form.Controls.AddRange(new Control[] { label, comboBox, btnOk, btnCancel });
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK ? comboBox.SelectedItem : null;
        }
    }
}