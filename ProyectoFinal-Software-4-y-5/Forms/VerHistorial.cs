using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public partial class VerHistorial : Form
    {
        private CultureInfo culturaMoneda = new CultureInfo("es-PA");
        private readonly ApiAdmin _apiCliente;
        private int _clienteId;
        private string _numeroCuenta;
        private List<TransaccionHistorial> _transacciones;

        public VerHistorial()
        {
            InitializeComponent();
            _apiCliente = new ApiAdmin("http://localhost:5202", allowInvalidSsl: false);
            _transacciones = new List<TransaccionHistorial>();

            // CONFIGURAR EVENTO DEL BOTÓN CERRAR
            btnCerrar.Click += BtnCerrar_Click;

            // CARGAR DATOS AL ABRIR EL FORMULARIO
            this.Load += async (s, e) => await CargarHistorialAsync();
        }

        // Constructor que recibe información específica de una cuenta
        public VerHistorial(int clienteId, string numeroCuenta) : this()
        {
            _clienteId = clienteId;
            _numeroCuenta = numeroCuenta;
            
            // Actualizar título con información de la cuenta
            if (!string.IsNullOrEmpty(numeroCuenta))
            {
                lblTituloHistorial.Text = $"Historial - Cuenta {numeroCuenta}";
            }
        }

        // MÉTODO: Cargar historial de transacciones desde la API
        private async Task CargarHistorialAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Iniciando carga de historial");
                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: _clienteId={_clienteId}, _numeroCuenta={_numeroCuenta}");
                
                // Mostrar indicador de carga
                dgvHistorialTransacciones.Enabled = false;
                lblTituloHistorial.Text = "Cargando historial...";

                _transacciones.Clear();

                if (_clienteId > 0 && !string.IsNullOrEmpty(_numeroCuenta))
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Cargando transacciones específicas de cuenta");
                    // Cargar transacciones específicas de la cuenta
                    await CargarTransaccionesPorCuentaAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Cargando transacciones generales");
                    // Cargar transacciones generales (para vista administrativa o cuando no hay cuenta específica)
                    await CargarTransaccionesGeneralesAsync();
                }

                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Mostrando {_transacciones.Count} transacciones en el grid");
                
                // Mostrar las transacciones en el grid
                MostrarTransaccionesEnGrid();
                
                // Configurar apariencia del DataGridView
                ConfigurarDataGridView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR VerHistorial: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ERROR VerHistorial StackTrace: {ex.StackTrace}");
                
                MessageBox.Show($"Error al cargar el historial: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Cargar datos de ejemplo en caso de error
                await CargarDatosEjemploAsync();
            }
            finally
            {
                dgvHistorialTransacciones.Enabled = true;
                if (_clienteId > 0 && !string.IsNullOrEmpty(_numeroCuenta))
                {
                    lblTituloHistorial.Text = $"Historial - Cuenta {_numeroCuenta}";
                }
                else
                {
                    lblTituloHistorial.Text = "Ver Historial";
                }
            }
        }

        private async Task CargarTransaccionesPorCuentaAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Cargando transacciones para clienteId={_clienteId}, cuenta={_numeroCuenta}");
                
                // Preferir endpoint específico del cliente para obtener columnas cuenta_origen/cuenta_destino
                var transCliente = await _apiCliente.ObtenerTransaccionesClienteAsync(_clienteId, 200).ConfigureAwait(false);
                
                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Recibidas {transCliente.Count()} transacciones del API");

                // Filtrar las transacciones que involucran esta cuenta específica
                var transaccionesCuenta = transCliente.Where(t =>
                {
                    var cuentaOrigen = GetStringValue(t, "Cuenta Origen");
                    var cuentaDestino = GetStringValue(t, "Cuenta Destino");
                    var numeroTransferencia = GetStringValue(t, "numero_cuenta");

                    System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Transacción - Origen={cuentaOrigen}, Destino={cuentaDestino}, NumCuenta={numeroTransferencia}");

                    return cuentaOrigen == _numeroCuenta ||
                           cuentaDestino == _numeroCuenta ||
                           numeroTransferencia == _numeroCuenta;
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Filtradas {transaccionesCuenta.Count} transacciones para cuenta {_numeroCuenta}");

                // Convertir a nuestro modelo local
                foreach (var transaccion in transaccionesCuenta)
                {
                    var historial = ConvertirATransaccionHistorial(transaccion);
                    if (historial != null)
                    {
                        _transacciones.Add(historial);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial: Total {_transacciones.Count} transacciones añadidas a la lista");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR VerHistorial cargando transacciones por cuenta (endpoint cliente): {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ERROR VerHistorial StackTrace: {ex.StackTrace}");
                // Fallback a método anterior si falla el endpoint específico
                try
                {
                    var todasTransacciones = await _apiCliente.ObtenerTransaccionesAsync().ConfigureAwait(false);
                    var transaccionesCuenta = todasTransacciones.Where(t =>
                    {
                        var cuentaOrigen = GetStringValue(t, "Cuenta Origen");
                        var cuentaDestino = GetStringValue(t, "Cuenta Destino");
                        var numeroTransferencia = GetStringValue(t, "numero_cuenta");

                        return cuentaOrigen == _numeroCuenta ||
                               cuentaDestino == _numeroCuenta ||
                               numeroTransferencia == _numeroCuenta;
                    }).ToList();

                    foreach (var transaccion in transaccionesCuenta)
                    {
                        var historial = ConvertirATransaccionHistorial(transaccion);
                        if (historial != null)
                        {
                            _transacciones.Add(historial);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR VerHistorial en fallback transacciones por cuenta: {ex2.Message}");
                    throw;
                }
            }
        }

        private async Task CargarTransaccionesGeneralesAsync()
        {
            try
            {
                // Obtener transacciones recientes del sistema
                var transaccionesRecientes = await _apiCliente.ObtenerTransaccionesRecientesAsync(50);
                
                foreach (var transaccion in transaccionesRecientes)
                {
                    var historial = new TransaccionHistorial
                    {
                        Fecha = transaccion.fecha,
                        Tipo = transaccion.tipo_transaccion ?? "Transacción",
                        Monto = transaccion.monto,
                        Descripcion = transaccion.descripcion ?? "Sin descripción",
                        Estado = transaccion.estado ?? "Completada",
                        TransaccionId = transaccion.transaccion_id
                    };
                    
                    _transacciones.Add(historial);
                }

                System.Diagnostics.Debug.WriteLine($"DEBUG: Cargadas {_transacciones.Count} transacciones recientes generales");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando transacciones generales: {ex.Message}");
                
                // Fallback: intentar obtener todas las transacciones
                try
                {
                    var todasTransacciones = await _apiCliente.ObtenerTransaccionesAsync();
                    var ultimas50 = todasTransacciones.Take(50);
                    
                    foreach (var transaccion in ultimas50)
                    {
                        var historial = ConvertirATransaccionHistorial(transaccion);
                        if (historial != null)
                        {
                            _transacciones.Add(historial);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private async Task CargarDatosEjemploAsync()
        {
            // Datos de ejemplo en caso de que falle la conexión a la API
            _transacciones = new List<TransaccionHistorial>
            {
                new TransaccionHistorial
                {
                    Fecha = DateTime.Now.AddDays(-1),
                    Tipo = "Depósito",
                    Monto = 2000.00m,
                    Descripcion = "Depósito en efectivo",
                    Estado = "Completada",
                    TransaccionId = 1
                },
                new TransaccionHistorial
                {
                    Fecha = DateTime.Now.AddDays(-3),
                    Tipo = "Retiro",
                    Monto = -500.00m,
                    Descripcion = "Retiro ATM",
                    Estado = "Completada",
                    TransaccionId = 2
                },
                new TransaccionHistorial
                {
                    Fecha = DateTime.Now.AddDays(-5),
                    Tipo = "Transferencia",
                    Monto = -1200.00m,
                    Descripcion = "Pago de servicios",
                    Estado = "Completada",
                    TransaccionId = 3
                },
                new TransaccionHistorial
                {
                    Fecha = DateTime.Now.AddDays(-7),
                    Tipo = "Depósito",
                    Monto = 3500.00m,
                    Descripcion = "Transferencia recibida",
                    Estado = "Completada",
                    TransaccionId = 4
                }
            };
        }

        private TransaccionHistorial ConvertirATransaccionHistorial(Dictionary<string, object> transaccion)
        {
            try
            {
                var fecha = DateTime.TryParse(GetStringValue(transaccion, "Fecha") ?? GetStringValue(transaccion, "fecha"), out var fechaParsed) 
                    ? fechaParsed : DateTime.Now;

                var monto = decimal.TryParse(GetStringValue(transaccion, "Monto") ?? GetStringValue(transaccion, "monto"), out var montoParsed) 
                    ? montoParsed : 0m;

                var tipo = GetStringValue(transaccion, "Tipo Transacción") ?? GetStringValue(transaccion, "tipo_transaccion") ?? "Transacción";
                var descripcion = GetStringValue(transaccion, "Descripción") ?? GetStringValue(transaccion, "descripcion") ?? "";
                var estado = GetStringValue(transaccion, "Estado") ?? GetStringValue(transaccion, "estado") ?? "Completada";
                
                var transaccionId = int.TryParse(GetStringValue(transaccion, "ID Transacción") ?? GetStringValue(transaccion, "transaccion_id"), out var idParsed) 
                    ? idParsed : 0;

                // Determinar si es entrada o salida basándose en las cuentas involucradas
                var cuentaOrigen = GetStringValue(transaccion, "Cuenta Origen") ?? GetStringValue(transaccion, "cuenta_origen");
                var cuentaDestino = GetStringValue(transaccion, "Cuenta Destino") ?? GetStringValue(transaccion, "cuenta_destino");
                
                if (!string.IsNullOrEmpty(_numeroCuenta))
                {
                    // Si esta cuenta es el destino, es una entrada (monto positivo)
                    // Si esta cuenta es el origen, es una salida (monto negativo)
                    if (cuentaDestino == _numeroCuenta && cuentaOrigen != _numeroCuenta)
                    {
                        monto = Math.Abs(monto); // Asegurar que sea positivo
                        if (string.IsNullOrEmpty(descripcion))
                        {
                            descripcion = $"Recibido desde {cuentaOrigen}";
                        }
                    }
                    else if (cuentaOrigen == _numeroCuenta && cuentaDestino != _numeroCuenta)
                    {
                        monto = -Math.Abs(monto); // Asegurar que sea negativo
                        if (string.IsNullOrEmpty(descripcion))
                        {
                            descripcion = $"Enviado a {cuentaDestino}";
                        }
                    }
                }

                return new TransaccionHistorial
                {
                    Fecha = fecha,
                    Tipo = tipo,
                    Monto = monto,
                    Descripcion = descripcion,
                    Estado = estado,
                    TransaccionId = transaccionId
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error convirtiendo transacción: {ex.Message}");
                return null;
            }
        }

        private void MostrarTransaccionesEnGrid()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial MostrarTransaccionesEnGrid: Mostrando {_transacciones.Count} transacciones");
                
                // Limpiar filas existentes
                dgvHistorialTransacciones.Rows.Clear();

                if (_transacciones.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial MostrarTransaccionesEnGrid: No hay transacciones para mostrar");
                    MessageBox.Show("No hay transacciones para mostrar en esta cuenta.", 
                        "Sin transacciones", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Ordenar por fecha descendente (más recientes primero)
                var transaccionesOrdenadas = _transacciones.OrderByDescending(t => t.Fecha).ToList();

                foreach (var trans in transaccionesOrdenadas)
                {
                    int index = dgvHistorialTransacciones.Rows.Add();
                    DataGridViewRow fila = dgvHistorialTransacciones.Rows[index];

                    // Llenar las columnas
                    fila.Cells["Fecha"].Value = trans.Fecha.ToString("dd/MM/yyyy HH:mm");
                    fila.Cells["Monto"].Value = trans.Monto.ToString("C2", culturaMoneda);
                    fila.Cells["Descripcion"].Value = $"{trans.Tipo} - {trans.Descripcion}";

                    // Guardar información adicional en la fila para el evento de clic
                    fila.Tag = trans;

                    // Aplicar color según si es depósito (positivo) o retiro (negativo)
                    if (trans.Monto >= 0)
                    {
                        fila.Cells["Monto"].Style.ForeColor = Color.Green;
                        fila.Cells["Monto"].Style.Font = new Font(dgvHistorialTransacciones.Font, FontStyle.Bold);
                    }
                    else
                    {
                        fila.Cells["Monto"].Style.ForeColor = Color.Red;
                        fila.Cells["Monto"].Style.Font = new Font(dgvHistorialTransacciones.Font, FontStyle.Bold);
                    }

                    // Color de estado
                    if (trans.Estado?.ToLower() == "pendiente")
                    {
                        fila.DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                    else if (trans.Estado?.ToLower() == "fallida")
                    {
                        fila.DefaultCellStyle.BackColor = Color.LightPink;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"DEBUG VerHistorial MostrarTransaccionesEnGrid: Se agregaron {dgvHistorialTransacciones.Rows.Count} filas al grid");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR VerHistorial MostrarTransaccionesEnGrid: {ex.Message}");
                MessageBox.Show($"Error al mostrar transacciones: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método helper para obtener valores string de manera segura
        private static string GetStringValue(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value) && value != null && value != DBNull.Value)
            {
                return value.ToString();
            }
            return null;
        }

        // MÉTODO: Configurar apariencia del DataGridView
        private void ConfigurarDataGridView()
        {
            try
            {
                // Ajustar columnas
                dgvHistorialTransacciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Configurar anchos específicos de columnas
                if (dgvHistorialTransacciones.Columns.Count >= 3)
                {
                    dgvHistorialTransacciones.Columns["Fecha"].FillWeight = 25; // 25%
                    dgvHistorialTransacciones.Columns["Monto"].FillWeight = 20; // 20%
                    dgvHistorialTransacciones.Columns["Descripcion"].FillWeight = 55; // 55%
                }

                // Deshabilitar edición
                dgvHistorialTransacciones.ReadOnly = true;

                // Selección de fila completa
                dgvHistorialTransacciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Estilo alternado de filas
                dgvHistorialTransacciones.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 250);

                // Estilo del encabezado
                dgvHistorialTransacciones.ColumnHeadersDefaultCellStyle.BackColor = Color.SlateBlue;
                dgvHistorialTransacciones.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvHistorialTransacciones.ColumnHeadersDefaultCellStyle.Font = new Font(dgvHistorialTransacciones.Font, FontStyle.Bold);

                // Permitir ordenamiento por columnas
                dgvHistorialTransacciones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                foreach (DataGridViewColumn col in dgvHistorialTransacciones.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.Automatic;
                }

                // NUEVO: Agregar botón de filtro si no existe
                AgregarBotonFiltro();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error configurando DataGridView: {ex.Message}");
            }
        }

        private void AgregarBotonFiltro()
        {
            try
            {
                // Verificar si ya existe el botón
                if (this.Controls.OfType<Button>().Any(b => b.Name == "btnFiltrarFechas"))
                    return;

                // Crear botón para filtrar por fechas
                var btnFiltrarFechas = new Button
                {
                    Name = "btnFiltrarFechas",
                    Text = "Filtrar por Fechas",
                    Size = new Size(120, 30),
                    Location = new Point(this.Width - 140, 85), // Posición en la parte superior derecha
                    BackColor = Color.SlateBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                btnFiltrarFechas.Click += (s, e) => AbrirFiltroFechas();
                this.Controls.Add(btnFiltrarFechas);

                // Crear botón para limpiar filtros
                var btnLimpiarFiltros = new Button
                {
                    Name = "btnLimpiarFiltros",
                    Text = "Limpiar Filtros",
                    Size = new Size(100, 30),
                    Location = new Point(this.Width - 260, 85),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                btnLimpiarFiltros.Click += async (s, e) =>
                {
                    lblTituloHistorial.Text = _clienteId > 0 && !string.IsNullOrEmpty(_numeroCuenta) 
                        ? $"Historial - Cuenta {_numeroCuenta}" 
                        : "Ver Historial";
                    await CargarHistorialAsync();
                };

                this.Controls.Add(btnLimpiarFiltros);

                // Crear etiqueta para mostrar información del filtro actual
                var lblFiltroInfo = new Label
                {
                    Name = "lblFiltroInfo",
                    Text = "",
                    Size = new Size(400, 20),
                    Location = new Point(20, 85),
                    ForeColor = Color.DarkBlue,
                    Font = new Font(this.Font, FontStyle.Italic),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };

                this.Controls.Add(lblFiltroInfo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error agregando botón de filtro: {ex.Message}");
            }
        }

        private void AbrirFiltroFechas()
        {
            try
            {
                using (var filtroForm = new FiltroFechasDialog())
                {
                    if (filtroForm.ShowDialog(this) == DialogResult.OK)
                    {
                        FiltrarPorFecha(filtroForm.FechaDesde, filtroForm.FechaHasta);
                        
                        // Actualizar etiqueta de información del filtro
                        var lblFiltroInfo = this.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "lblFiltroInfo");
                        if (lblFiltroInfo != null)
                        {
                            lblFiltroInfo.Text = $"Filtro aplicado: {filtroForm.FechaDesde:dd/MM/yyyy} - {filtroForm.FechaHasta:dd/MM/yyyy}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir filtro de fechas: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clase para el diálogo de filtro por fechas
        private class FiltroFechasDialog : Form
        {
            public DateTime FechaDesde { get; private set; }
            public DateTime FechaHasta { get; private set; }

            private DateTimePicker dtpFechaDesde;
            private DateTimePicker dtpFechaHasta;

            public FiltroFechasDialog()
            {
                InitializeComponent();
                FechaDesde = DateTime.Now.AddDays(-30); // Por defecto últimos 30 días
                FechaHasta = DateTime.Now;
                dtpFechaDesde.Value = FechaDesde;
                dtpFechaHasta.Value = FechaHasta;
            }

            private void InitializeComponent()
            {
                this.Text = "Filtrar Historial por Fechas";
                this.Size = new Size(450, 280); // Aumentar tamaño significativamente
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                // Fecha desde
                var lblFechaDesde = new Label
                {
                    Text = "Fecha desde:",
                    Location = new Point(30, 40), // Más espacio desde el borde
                    Size = new Size(100, 25), // Más ancho para el texto
                    Font = new Font(this.Font.FontFamily, 10)
                };

                dtpFechaDesde = new DateTimePicker
                {
                    Location = new Point(140, 38),
                    Size = new Size(250, 25), // Más ancho
                    Format = DateTimePickerFormat.Short,
                    Font = new Font(this.Font.FontFamily, 10)
                };

                // Fecha hasta
                var lblFechaHasta = new Label
                {
                    Text = "Fecha hasta:",
                    Location = new Point(30, 80), // Más separación vertical
                    Size = new Size(100, 25),
                    Font = new Font(this.Font.FontFamily, 10)
                };

                dtpFechaHasta = new DateTimePicker
                {
                    Location = new Point(140, 78),
                    Size = new Size(250, 25), // Más ancho
                    Format = DateTimePickerFormat.Short,
                    Font = new Font(this.Font.FontFamily, 10)
                };

                // Separador visual
                var lblOpciones = new Label
                {
                    Text = "Opciones rápidas:",
                    Location = new Point(30, 120),
                    Size = new Size(150, 20),
                    Font = new Font(this.Font.FontFamily, 9, FontStyle.Bold),
                    ForeColor = Color.DarkBlue
                };

                // Botones predefinidos con mejor espacio
                var btnUltimos7Dias = new Button
                {
                    Text = "Últimos 7 días",
                    Location = new Point(30, 150),
                    Size = new Size(110, 30), // Botones más grandes
                    BackColor = Color.LightBlue,
                    FlatStyle = FlatStyle.Flat
                };
                btnUltimos7Dias.Click += (s, e) =>
                {
                    dtpFechaDesde.Value = DateTime.Now.AddDays(-7);
                    dtpFechaHasta.Value = DateTime.Now;
                };

                var btnUltimos30Dias = new Button
                {
                    Text = "Últimos 30 días",
                    Location = new Point(155, 150),
                    Size = new Size(110, 30),
                    BackColor = Color.LightBlue,
                    FlatStyle = FlatStyle.Flat
                };
                btnUltimos30Dias.Click += (s, e) =>
                {
                    dtpFechaDesde.Value = DateTime.Now.AddDays(-30);
                    dtpFechaHasta.Value = DateTime.Now;
                };

                var btnEsteAno = new Button
                {
                    Text = "Este año",
                    Location = new Point(280, 150),
                    Size = new Size(110, 30),
                    BackColor = Color.LightBlue,
                    FlatStyle = FlatStyle.Flat
                };
                btnEsteAno.Click += (s, e) =>
                {
                    dtpFechaDesde.Value = new DateTime(DateTime.Now.Year, 1, 1);
                    dtpFechaHasta.Value = DateTime.Now;
                };

                // Botones de acción con mejor posicionamiento
                var btnOk = new Button
                {
                    Text = "Aplicar Filtro",
                    Location = new Point(200, 200), // Más abajo
                    Size = new Size(120, 35),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.SlateBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold)
                };

                var btnCancelar = new Button
                {
                    Text = "Cancelar",
                    Location = new Point(330, 200),
                    Size = new Size(90, 35),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font(this.Font.FontFamily, 10)
                };

                btnOk.Click += (s, e) =>
                {
                    FechaDesde = dtpFechaDesde.Value;
                    FechaHasta = dtpFechaHasta.Value;

                    if (FechaDesde > FechaHasta)
                    {
                        MessageBox.Show("La fecha 'desde' debe ser menor o igual a la fecha 'hasta'.",
                            "Fechas inválidas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                };

                this.Controls.AddRange(new Control[]
                {
                    lblFechaDesde, dtpFechaDesde,
                    lblFechaHasta, dtpFechaHasta,
                    lblOpciones,
                    btnUltimos7Dias, btnUltimos30Dias, btnEsteAno,
                    btnOk, btnCancelar
                });

                this.AcceptButton = btnOk;
                this.CancelButton = btnCancelar;
            }
        }

        // EVENTO: Click en celda del DataGridView
        private void DgvHistorialTransacciones_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar que no se hizo clic en el encabezado
            if (e.RowIndex >= 0 && e.RowIndex < dgvHistorialTransacciones.Rows.Count)
            {
                try
                {
                    DataGridViewRow fila = dgvHistorialTransacciones.Rows[e.RowIndex];
                    
                    // Obtener la información de la transacción desde el Tag de la fila
                    if (fila.Tag is TransaccionHistorial transaccion)
                    {
                        MostrarDetalleTransaccion(transaccion);
                    }
                    else
                    {
                        // Fallback: obtener datos directamente de las celdas
                        string fecha = fila.Cells["Fecha"].Value?.ToString() ?? "";
                        string monto = fila.Cells["Monto"].Value?.ToString() ?? "";
                        string descripcion = fila.Cells["Descripcion"].Value?.ToString() ?? "";

                        MessageBox.Show(
                            $"Detalles de la transacción:\n\n" +
                            $"Fecha: {fecha}\n" +
                            $"Monto: {monto}\n" +
                            $"Descripción: {descripcion}",
                            "Detalle de Transacción",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al mostrar detalles: {ex.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // EVENTO: Cerrar ventana y volver a DetalleDeCuenta
        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MostrarDetalleTransaccion(TransaccionHistorial transaccion)
        {
            var detalles = new StringBuilder();
            detalles.AppendLine("=== DETALLE DE TRANSACCIÓN ===");
            detalles.AppendLine();
            detalles.AppendLine($"ID: {transaccion.TransaccionId}");
            detalles.AppendLine($"Fecha: {transaccion.Fecha:dd/MM/yyyy HH:mm:ss}");
            detalles.AppendLine($"Tipo: {transaccion.Tipo}");
            detalles.AppendLine($"Monto: {transaccion.Monto:C2}");
            detalles.AppendLine($"Estado: {transaccion.Estado}");
            detalles.AppendLine($"Descripción: {transaccion.Descripcion}");
            
            if (!string.IsNullOrEmpty(_numeroCuenta))
            {
                detalles.AppendLine();
                detalles.AppendLine($"Cuenta: {_numeroCuenta}");
                
                if (transaccion.Monto >= 0)
                {
                    detalles.AppendLine("Tipo de movimiento: ENTRADA (Crédito)");
                }
                else
                {
                    detalles.AppendLine("Tipo de movimiento: SALIDA (Débito)");
                }
            }

            MessageBox.Show(
                detalles.ToString(),
                "Detalle Completo de Transacción",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        // Método para filtrar transacciones por fecha
        public void FiltrarPorFecha(DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                var transaccionesFiltradas = _transacciones
                    .Where(t => t.Fecha.Date >= fechaDesde.Date && t.Fecha.Date <= fechaHasta.Date)
                    .ToList();

                // Limpiar grid
                dgvHistorialTransacciones.Rows.Clear();

                // Mostrar transacciones filtradas
                foreach (var trans in transaccionesFiltradas.OrderByDescending(t => t.Fecha))
                {
                    int index = dgvHistorialTransacciones.Rows.Add();
                    DataGridViewRow fila = dgvHistorialTransacciones.Rows[index];

                    fila.Cells["Fecha"].Value = trans.Fecha.ToString("dd/MM/yyyy HH:mm");
                    fila.Cells["Monto"].Value = trans.Monto.ToString("C2", culturaMoneda);
                    fila.Cells["Descripcion"].Value = $"{trans.Tipo} - {trans.Descripcion}";
                    fila.Tag = trans;

                    // Aplicar colores
                    if (trans.Monto >= 0)
                    {
                        fila.Cells["Monto"].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        fila.Cells["Monto"].Style.ForeColor = Color.Red;
                    }
                }

                lblTituloHistorial.Text = $"Historial Filtrado ({transaccionesFiltradas.Count} transacciones)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar transacciones: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método público para recargar el historial
        public async Task RecargarHistorialAsync()
        {
            await CargarHistorialAsync();
        }
    }

    // CLASE AUXILIAR: Modelo de transacción para el historial (más completa que la original)
    public class TransaccionHistorial
    {
        public int TransaccionId { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
    }
}