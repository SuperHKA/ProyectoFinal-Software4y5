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
using datellees_de_cuentas;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public partial class DetalleDeCuenta : Form
    {
        private CuentaBancaria cuentaActual;
        private CultureInfo culturaMoneda = new CultureInfo("es-PA");
        private readonly ApiAdmin _apiCliente;
        private int _clienteId;
        private string _numeroCuenta;

        public DetalleDeCuenta()
        {
            InitializeComponent();
            _apiCliente = new ApiAdmin("http://localhost:5202", allowInvalidSsl: false);
            ConectarEventos();
        }

        // Constructor para cargar datos específicos de una cuenta
        public DetalleDeCuenta(int clienteId, string numeroCuenta) : this()
        {
            _clienteId = clienteId;
            _numeroCuenta = numeroCuenta;
            this.Load += async (s, e) => await CargarDatosCuentaAsync(clienteId, numeroCuenta);
        }

        private void ConectarEventos()
        {
            // CONFIGURAR EVENTOS DE LOS BOTONES
            btnNuevaTransaccion.Click += BtnNuevaTransaccion_Click;
            btnVerHistorial.Click += BtnVerHistorial_Click;
        }

        // EVENTO: Abrir ventana de Nueva Transacción
        private void BtnNuevaTransaccion_Click(object sender, EventArgs e)
        {
            try
            {
                if (cuentaActual != null && !string.IsNullOrEmpty(cuentaActual.NumeroCuenta))
                {
                    // Usar constructor sin parámetros y configurar después si es necesario
                    var formNuevaTransaccion = new NuevaTransaccion();
                    if (formNuevaTransaccion.ShowDialog() == DialogResult.OK)
                    {
                        // Recargar datos de la cuenta después de una transacción exitosa
                        if (_clienteId > 0 && !string.IsNullOrEmpty(_numeroCuenta))
                        {
                            _ = CargarDatosCuentaAsync(_clienteId, _numeroCuenta);
                        }
                    }
                }
                else
                {
                    var formNuevaTransaccion = new NuevaTransaccion();
                    formNuevaTransaccion.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir Nueva Transacción: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // EVENTO: Abrir ventana de Ver Historial
        private void BtnVerHistorial_Click(object sender, EventArgs e)
        {
            try
            {
                if (cuentaActual != null && !string.IsNullOrEmpty(cuentaActual.NumeroCuenta))
                {
                    // Usar constructor sin parámetros
                    var formVerHistorial = new VerHistorial();
                    formVerHistorial.ShowDialog();
                }
                else
                {
                    var formVerHistorial = new VerHistorial();
                    formVerHistorial.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir Ver Historial: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cargar datos reales de la cuenta desde la API
        private async Task CargarDatosCuentaAsync(int clienteId, string numeroCuenta)
        {
            try
            {
                // DEBUG: Mostrar qué se está buscando
                System.Diagnostics.Debug.WriteLine($"DEBUG DetalleDeCuenta: Buscando cuenta '{numeroCuenta}' para cliente {clienteId}");
                
                // Obtener información del cliente usando métodos disponibles
                var clientesData = await _apiCliente.ObtenerClientesAsync().ConfigureAwait(false);
                var clienteInfo = clientesData.FirstOrDefault(c => c.cliente_id == clienteId);
                
                if (clienteInfo == null)
                {
                    this.BeginInvoke((Action)(() =>
                    {
                        MessageBox.Show("No se pudo obtener la información del cliente.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }));
                    return;
                }

                // Obtener cuentas del cliente
                var cuentas = await _apiCliente.ObtenerCuentasPorClienteAsync(clienteId).ConfigureAwait(false);
                
                // DEBUG: Mostrar qué cuentas se recibieron
                System.Diagnostics.Debug.WriteLine($"DEBUG DetalleDeCuenta: Se recibieron {cuentas.Count()} cuentas");
                foreach (var cuenta in cuentas)
                {
                    var numCuenta = cuenta.ContainsKey("numero_cuenta") ? cuenta["numero_cuenta"]?.ToString() : 
                                   cuenta.ContainsKey("numeroCuenta") ? cuenta["numeroCuenta"]?.ToString() : "N/A";
                    System.Diagnostics.Debug.WriteLine($"  - Cuenta: {numCuenta}");
                }
                
                // Buscar la cuenta con ambas variantes del nombre de campo
                var cuentaData = cuentas.FirstOrDefault(c => 
                    (c.ContainsKey("numero_cuenta") && c["numero_cuenta"]?.ToString() == numeroCuenta) ||
                    (c.ContainsKey("numeroCuenta") && c["numeroCuenta"]?.ToString() == numeroCuenta));

                if (cuentaData == null)
                {
                    this.BeginInvoke((Action)(() =>
                    {
                        MessageBox.Show($"No se encontró la cuenta especificada.\n\nCliente ID: {clienteId}\nNúmero de cuenta buscado: {numeroCuenta}\nCuentas encontradas: {cuentas.Count()}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }));
                    return;
                }

                // Obtener transacciones de la cuenta
                var todasTransacciones = await _apiCliente.ObtenerTransaccionesAsync().ConfigureAwait(false);
                var transaccionesCuenta = todasTransacciones.Where(t =>
                {
                    var cuentaOrigen = t.ContainsKey("cuenta_origen") ? t["cuenta_origen"]?.ToString() : "";
                    var cuentaDestino = t.ContainsKey("cuenta_destino") ? t["cuenta_destino"]?.ToString() : "";
                    return cuentaOrigen == numeroCuenta || cuentaDestino == numeroCuenta;
                }).ToList();

                // Crear objeto CuentaBancaria con datos reales
                this.BeginInvoke((Action)(() =>
                {
                    CrearCuentaBancariaConDatos(clienteInfo, cuentaData, transaccionesCuenta);
                    MostrarDatos();
                }));
            }
            catch (Exception ex)
            {
                this.BeginInvoke((Action)(() =>
                {
                    MessageBox.Show($"Error al cargar datos de la cuenta: {ex.Message}\n\nDetalles: {ex.StackTrace}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }));
            }
        }

        private void CrearCuentaBancariaConDatos(
            ApiAdmin.ClienteDto clienteInfo, 
            IDictionary<string, object> cuentaData,
            IEnumerable<IDictionary<string, object>> transacciones)
        {
            // Obtener saldo con verificación
            var saldo = 0m;
            if (cuentaData.ContainsKey("saldo"))
            {
                saldo = Convert.ToDecimal(cuentaData["saldo"] ?? 0);
            }
            
            // Obtener número de cuenta con ambas variantes
            var numeroCuenta = cuentaData.ContainsKey("numero_cuenta") ? cuentaData["numero_cuenta"]?.ToString() :
                              cuentaData.ContainsKey("numeroCuenta") ? cuentaData["numeroCuenta"]?.ToString() : "";
            
            // Obtener fecha de apertura si existe
            DateTime fechaApertura = DateTime.Now.AddMonths(-6);
            if (cuentaData.ContainsKey("fecha_apertura") && cuentaData["fecha_apertura"] != null)
            {
                DateTime.TryParse(cuentaData["fecha_apertura"].ToString(), out fechaApertura);
            }

            // Determinar tipo de cuenta with múltiples variantes
            var tipoCuenta = "Cuenta";
            if (cuentaData.ContainsKey("tipo"))
            {
                tipoCuenta = cuentaData["tipo"]?.ToString() ?? "Cuenta";
            }
            else if (cuentaData.ContainsKey("tipoCuenta"))
            {
                tipoCuenta = cuentaData["tipoCuenta"]?.ToString() ?? "Cuenta";
            }
            else if (cuentaData.ContainsKey("tipo_cuenta"))
            {
                tipoCuenta = cuentaData["tipo_cuenta"]?.ToString() ?? "Cuenta";
            }
            
            // Obtener estado con verificación
            var activa = true;
            if (cuentaData.ContainsKey("activa"))
            {
                activa = Convert.ToBoolean(cuentaData["activa"] ?? true);
            }
            
            // Calcular estadísticas de transacciones del mes actual con datos reales
            var depositos = 0m;
            var retiros = 0m;
            DateTime? fechaUltimaTransaccion = null;

            if (transacciones != null && transacciones.Any())
            {
                var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var transaccionesDelMes = transacciones.Where(t =>
                {
                    if (t.TryGetValue("fecha", out var fechaObj) && DateTime.TryParse(fechaObj?.ToString(), out var fecha))
                    {
                        return fecha >= inicioMes;
                    }
                    return false;
                }).ToList();

                // Calcular depósitos (transacciones donde esta cuenta es destino)
                depositos = transaccionesDelMes.Where(t => 
                {
                    var cuentaDestino = t.ContainsKey("cuenta_destino") ? t["cuenta_destino"]?.ToString() : "";
                    var tipo = t.ContainsKey("tipo_transaccion") ? t["tipo_transaccion"]?.ToString()?.ToLower() : "";
                    return cuentaDestino == _numeroCuenta || tipo.Contains("depósito") || tipo.Contains("deposito");
                }).Sum(t => 
                {
                    if (t.TryGetValue("monto", out var montoObj))
                    {
                        return Convert.ToDecimal(montoObj);
                    }
                    return 0m;
                });

                // Calcular retiros (transacciones donde esta cuenta es origen)
                retiros = transaccionesDelMes.Where(t => 
                {
                    var cuentaOrigen = t.ContainsKey("cuenta_origen") ? t["cuenta_origen"]?.ToString() : "";
                    var tipo = t.ContainsKey("tipo_transaccion") ? t["tipo_transaccion"]?.ToString()?.ToLower() : "";
                    return cuentaOrigen == _numeroCuenta || tipo.Contains("retiro");
                }).Sum(t => 
                {
                    if (t.TryGetValue("monto", out var montoObj))
                    {
                        return Convert.ToDecimal(montoObj);
                    }
                    return 0m;
                });

                // Obtener fecha de última transacción
                var ultimaTransaccion = transacciones
                    .Where(t => t.ContainsKey("fecha"))
                    .OrderByDescending(t => DateTime.TryParse(t["fecha"]?.ToString(), out var f) ? f : DateTime.MinValue)
                    .FirstOrDefault();

                if (ultimaTransaccion != null && ultimaTransaccion.TryGetValue("fecha", out var fechaUltima))
                {
                    DateTime.TryParse(fechaUltima?.ToString(), out var fechaTemp);
                    fechaUltimaTransaccion = fechaTemp;
                }
            }

            cuentaActual = new CuentaBancaria
            {
                NumeroCuenta = numeroCuenta,
                Titular = clienteInfo?.nombre_completo ?? "Cliente",
                TipoCuenta = tipoCuenta,
                SaldoActual = saldo,
                Estado = activa ? "Activa" : "Inactiva",
                FechaApertura = fechaApertura,
                UltimoMovimiento = fechaUltimaTransaccion,
                DepositosMes = depositos,
                RetirosMes = retiros,
                SaldoNeto = depositos - retiros
            };
        }

        // Mostrar datos en la interfaz
        private void MostrarDatos()
        {
            try
            {
                if (cuentaActual == null) return;

                // Título
                lblTituloPrincipal.Text = $"Detalle de Cuenta #{cuentaActual.NumeroCuenta}";

                // Información de la cuenta
                txtNumeroCuentaValor.Text = cuentaActual.NumeroCuenta;
                txtTitularValor.Text = cuentaActual.Titular;
                txtTipoCuentaValor.Text = cuentaActual.TipoCuenta;
                txtSaldoActualValor.Text = cuentaActual.SaldoActual.ToString("C2", culturaMoneda);
                txtEstadoValor.Text = cuentaActual.Estado;
                txtFechaAperturaValor.Text = cuentaActual.FechaApertura.ToString("dd/MM/yyyy");
                txtUltimoMovimientoValor.Text = cuentaActual.UltimoMovimiento?.ToString("dd/MM/yyyy HH:mm") ?? "Sin movimientos";

                // Resumen mensual
                txtDepositosValor.Text = cuentaActual.DepositosMes.ToString("C2", culturaMoneda);
                txtRetirosValor.Text = cuentaActual.RetirosMes.ToString("C2", culturaMoneda);
                txtSaldoNetoValor.Text = cuentaActual.SaldoNeto.ToString("C2", culturaMoneda);

                // Colores
                txtSaldoActualValor.ForeColor = cuentaActual.SaldoActual >= 0 ? Color.Green : Color.Red;
                txtEstadoValor.ForeColor = cuentaActual.Estado == "Activa" ? Color.Green : Color.Red;
                txtDepositosValor.ForeColor = Color.Green;
                txtRetirosValor.ForeColor = Color.Red;
                txtSaldoNetoValor.ForeColor = cuentaActual.SaldoNeto >= 0 ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar datos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método público para recargar datos
        public async Task RecargarDatosAsync()
        {
            if (_clienteId > 0 && !string.IsNullOrEmpty(_numeroCuenta))
            {
                await CargarDatosCuentaAsync(_clienteId, _numeroCuenta);
            }
        }
    }
}