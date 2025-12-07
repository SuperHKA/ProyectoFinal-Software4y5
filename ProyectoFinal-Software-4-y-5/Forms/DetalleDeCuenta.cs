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
    public partial class DetalleDeCuenta : Form
    {
        private CuentaBancaria cuentaActual;
        private CultureInfo culturaMoneda = new CultureInfo("es-PA");

        public DetalleDeCuenta()
        {
            InitializeComponent();
            CargarDatosEjemplo();

            // CONFIGURAR EVENTOS DE LOS BOTONES
            btnNuevaTransaccion.Click += BtnNuevaTransaccion_Click;
            btnVerHistorial.Click += BtnVerHistorial_Click;
        }


        // EVENTO: Abrir ventana de Nueva Transacción
        private void BtnNuevaTransaccion_Click(object sender, EventArgs e)
        {
            NuevaTransaccion formNuevaTransaccion = new NuevaTransaccion();
            formNuevaTransaccion.ShowDialog();
        }


        // EVENTO: Abrir ventana de Ver Historial

        private void BtnVerHistorial_Click(object sender, EventArgs e)
        {
            VerHistorial formVerHistorial = new VerHistorial();
            formVerHistorial.ShowDialog();
        }

        // Cargar datos de ejemplo
        private void CargarDatosEjemplo()
        {
            cuentaActual = new CuentaBancaria
            {
                NumeroCuenta = "ACC-2024-001234",
                Titular = "Juan Pérez González",
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
    }
}