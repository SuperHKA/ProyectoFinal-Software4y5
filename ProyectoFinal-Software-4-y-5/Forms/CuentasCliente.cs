using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public partial class CuentasCliente : Form
    {
        private readonly ApiAdmin _apiCliente;
        private readonly int _clienteId;
        private readonly string _nombreCliente;

        public CuentasCliente(int clienteId, string nombreCliente)
        {
            _clienteId = clienteId;
            _nombreCliente = nombreCliente;
            _apiCliente = new ApiAdmin("http://localhost:5202", allowInvalidSsl: false);
            
            InitializeComponent();
            
            // Configurar el título dinámicamente
            lblTitulo.Text = $"Cuentas de: {_nombreCliente}";
            
            _ = CargarCuentasAsync();
        }

        private async Task CargarCuentasAsync()
        {
            try
            {
                var cuentas = await _apiCliente.ObtenerCuentasPorClienteAsync(_clienteId);

                if (cuentas == null || !cuentas.Any())
                {
                    MessageBox.Show("Este cliente no tiene cuentas registradas.",
                        "Sin cuentas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var dt = new DataTable();
                dt.Columns.Add("Número de Cuenta", typeof(string));
                dt.Columns.Add("Tipo de Cuenta", typeof(string));
                dt.Columns.Add("Saldo", typeof(string));
                dt.Columns.Add("Estado", typeof(string));

                foreach (var cuenta in cuentas)
                {
                    var numeroCuenta = cuenta.ContainsKey("numeroCuenta") 
                        ? cuenta["numeroCuenta"]?.ToString() 
                        : cuenta.ContainsKey("numero_cuenta") 
                            ? cuenta["numero_cuenta"]?.ToString() 
                            : "N/A";

                    var tipoCuenta = cuenta.ContainsKey("tipoCuenta") 
                        ? cuenta["tipoCuenta"]?.ToString() 
                        : cuenta.ContainsKey("tipo_cuenta") 
                            ? cuenta["tipo_cuenta"]?.ToString() 
                            : cuenta.ContainsKey("tipo")
                                ? cuenta["tipo"]?.ToString()
                                : "N/A";

                    var saldo = cuenta.ContainsKey("saldo") 
                        ? Convert.ToDecimal(cuenta["saldo"]) 
                        : 0m;

                    var estado = cuenta.ContainsKey("activa") 
                        ? (Convert.ToBoolean(cuenta["activa"]) ? "Activa" : "Inactiva")
                        : "Activa";

                    dt.Rows.Add(
                        numeroCuenta,
                        tipoCuenta,
                        $"${saldo:N2}",
                        estado
                    );
                }

                dgvCuentas.DataSource = dt;

                // Ajustar ancho de columnas
                if (dgvCuentas.Columns.Count >= 4)
                {
                    dgvCuentas.Columns[0].Width = 200; // Número de Cuenta
                    dgvCuentas.Columns[1].Width = 200; // Tipo
                    dgvCuentas.Columns[2].Width = 180; // Saldo
                    dgvCuentas.Columns[3].Width = 160; // Estado
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las cuentas: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvCuentas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignorar clic en encabezado

            try
            {
                var row = dgvCuentas.Rows[e.RowIndex];
                var numeroCuenta = row.Cells[0].Value?.ToString();

                if (!string.IsNullOrEmpty(numeroCuenta) && numeroCuenta != "N/A")
                {
                    // Abrir el formulario de detalles de cuenta
                    var detalleForm = new DetalleDeCuenta(_clienteId, numeroCuenta);
                    detalleForm.Show();
                }
                else
                {
                    MessageBox.Show("No se pudo obtener el número de cuenta.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir detalles de cuenta: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
