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
            CargarDatosEjemplo();
            ConectarEventos();
        }

        // Constructor para cargar datos específicos de una cuenta
        public DetalleDeCuenta(int clienteId, string numeroCuenta) : this()
        {
            _clienteId = clienteId;
            _numeroCuenta = numeroCuenta;
            _ = CargarDatosCuentaAsync(clienteId, numeroCuenta);
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
                // Obtener información del cliente usando métodos disponibles
                var clientesData = await _apiCliente.ObtenerClientesAsync().ConfigureAwait(false);
                var clienteInfo = clientesData.FirstOrDefault(c => c.cliente_id == clienteId);
                
                if (clienteInfo == null)
                {
                    MessageBox.Show("No se pudo obtener la información del cliente.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtener cuentas del cliente
                var cuentas = await _apiCliente.ObtenerCuentasPorClienteAsync(clienteId).ConfigureAwait(false);
                var cuentaData = cuentas.FirstOrDefault(c => 
                    c.ContainsKey("numero_cuenta") && 
                    c["numero_cuenta"]?.ToString() == numeroCuenta);

                if (cuentaData == null)
                {
                    MessageBox.Show("No se encontró la cuenta especificada.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Crear información simulada de transacciones (ya que el método no está disponible)
                var transaccionesCuenta = new List<Dictionary<string, object>>();

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
                    MessageBox.Show($"Error al cargar datos de la cuenta: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CargarDatosEjemplo(); // Fallback a datos de ejemplo
                }));
            }
        }

        private void CrearCuentaBancariaConDatos(
            ApiAdmin.ClienteDto clienteInfo, 
            IDictionary<string, object> cuentaData,
            IEnumerable<IDictionary<string, object>> transacciones)
        {
            var saldo = Convert.ToDecimal(cuentaData["saldo"] ?? 0);
            var fechaApertura = DateTime.Now.AddMonths(-6); // Fecha de apertura por defecto

            // Determinar tipo de cuenta
            var tipoCuenta = "Cuenta";
            if (cuentaData.ContainsKey("tipo") || cuentaData.ContainsKey("tipoCuenta"))
            {
                tipoCuenta = cuentaData["tipo"]?.ToString() ?? cuentaData["tipoCuenta"]?.ToString() ?? "Cuenta";
            }
            
            // Calcular estadísticas de transacciones del mes actual (datos simulados)
            var depositos = 0m;
            var retiros = 0m;

            // Si hay transacciones reales, usarlas
            if (transacciones != null)
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

                depositos = transaccionesDelMes.Where(t => 
                {
                    var cuentaDestino = t.ContainsKey("cuenta_destino") ? t["cuenta_destino"]?.ToString() : "";
                    var tipo = t.ContainsKey("tipo_transaccion") ? t["tipo_transaccion"]?.ToString() : "";
                    return cuentaDestino == _numeroCuenta || tipo?.ToLower().Contains("depósito") == true;
                }).Sum(t => Convert.ToDecimal(t["monto"] ?? 0));

                retiros = transaccionesDelMes.Where(t => 
                {
                    var cuentaOrigen = t.ContainsKey("cuenta_origen") ? t["cuenta_origen"]?.ToString() : "";
                    var tipo = t.ContainsKey("tipo_transaccion") ? t["tipo_transaccion"]?.ToString() : "";
                    return cuentaOrigen == _numeroCuenta || tipo?.ToLower().Contains("retiro") == true;
                }).Sum(t => Convert.ToDecimal(t["monto"] ?? 0));
            }
            else
            {
                // Datos simulados para demostración
                depositos = saldo * 0.3m; // 30% del saldo como depósitos del mes
                retiros = saldo * 0.1m;   // 10% del saldo como retiros del mes
            }

            DateTime? fechaUltimaTransaccion = DateTime.Now.AddDays(-3); // Última transacción hace 3 días

            cuentaActual = new CuentaBancaria
            {
                NumeroCuenta = cuentaData["numero_cuenta"]?.ToString() ?? "",
                Titular = clienteInfo?.nombre_completo ?? "Cliente",
                TipoCuenta = tipoCuenta,
                SaldoActual = saldo,
                Estado = Convert.ToBoolean(cuentaData["activa"] ?? true) ? "Activa" : "Inactiva",
                FechaApertura = fechaApertura,
                UltimoMovimiento = fechaUltimaTransaccion,
                DepositosMes = depositos,
                RetirosMes = retiros,
                SaldoNeto = depositos - retiros
            };
        }

        // Cargar datos de ejemplo (fallback)
        private void CargarDatosEjemplo()
        {
            cuentaActual = new CuentaBancaria
            {
                NumeroCuenta = _numeroCuenta ?? "ACC-2024-001234",
                Titular = "Cliente Ejemplo",
                TipoCuenta = "Cuenta de Ahorros",
                SaldoActual = 15750.50m,
                Estado = "Activa",
                FechaApertura = new DateTime(2023, 3, 15),
                UltimoMovimiento = DateTime.Now.AddDays(-2),
                DepositosMes = 5500.00m,
                RetirosMes = 1700.00m,
                SaldoNeto = 3800.00m
            };

            MostrarDatos();
        }

        // Mostrar datos en la interfaz
        private void MostrarDatos()
        {
            try
            {
                if (cuentaActual == null) return;

                // Título
                label1.Text = $"Detalle de Cuenta #{cuentaActual.NumeroCuenta}";

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