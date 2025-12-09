using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public partial class NuevaTransaccion : Form
    {
        private readonly ApiAdmin _apiCliente;
        private int? _clienteId;
        private string _cuentaOrigenPredefinida;

        public NuevaTransaccion()
        {
            InitializeComponent();
            _apiCliente = new ApiAdmin("http://localhost:5202", allowInvalidSsl: false);
            ConfigurarEventos();
            CargarTiposTransaccion();
        }

        // Constructor con cuenta de origen predefinida
        public NuevaTransaccion(int clienteId, string numeroCuenta) : this()
        {
            _clienteId = clienteId;
            _cuentaOrigenPredefinida = numeroCuenta;
            txtNumeroCuenta.Text = numeroCuenta;
            txtNumeroCuenta.ReadOnly = true;
        }

        private void ConfigurarEventos()
        {
            btnCancelar.Click += btnCancelar_Click;
            btnConfirmar.Click += btnConfirmar_ClickAsync;
            cmbTipoTransaccion.SelectedIndexChanged += CmbTipoTransaccion_SelectedIndexChanged;
            txtMonto.KeyPress += TxtMonto_KeyPress;
        }

        private void CargarTiposTransaccion()
        {
            cmbTipoTransaccion.Items.Clear();
            cmbTipoTransaccion.Items.AddRange(new object[]
            {
                "Depósito",
                "Retiro",
                "Transferencia Interna",
                "Transferencia Externa"
            });
            cmbTipoTransaccion.SelectedIndex = 0;
        }

        private void CmbTipoTransaccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ajustar la interfaz según el tipo de transacción
            string tipoSeleccionado = cmbTipoTransaccion.SelectedItem?.ToString();
            
            if (tipoSeleccionado == "Transferencia Interna" || tipoSeleccionado == "Transferencia Externa")
            {
                lblNumeroCuenta.Text = "Cuenta Destino:";
                if (!txtNumeroCuenta.ReadOnly)
                {
                    txtNumeroCuenta.Text = "";
                }
            }
            else
            {
                lblNumeroCuenta.Text = "Cuenta:";
                if (!string.IsNullOrEmpty(_cuentaOrigenPredefinida))
                {
                    txtNumeroCuenta.Text = _cuentaOrigenPredefinida;
                }
                else
                {
                    txtNumeroCuenta.Text = "";
                }
            }
        }

        private void TxtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números, punto decimal y teclas de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Permitir solo un punto decimal
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private async void btnConfirmar_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                // Validar campos
                if (!ValidarCampos())
                    return;

                // Deshabilitar botón durante procesamiento
                btnConfirmar.Enabled = false;
                btnConfirmar.Text = "Procesando...";

                string tipoTransaccion = cmbTipoTransaccion.SelectedItem.ToString();
                string numeroCuenta = txtNumeroCuenta.Text.Trim();
                decimal monto = decimal.Parse(txtMonto.Text.Trim());
                string descripcion = txtDescripcion.Text.Trim();

                bool resultado = false;
                string mensaje = "";

                switch (tipoTransaccion)
                {
                    case "Depósito":
                        resultado = await RealizarDepositoAsync(numeroCuenta, monto, descripcion);
                        mensaje = resultado ? "Depósito realizado exitosamente" : "Error al realizar el depósito";
                        break;

                    case "Retiro":
                        resultado = await RealizarRetiroAsync(numeroCuenta, monto, descripcion);
                        mensaje = resultado ? "Retiro realizado exitosamente" : "Error al realizar el retiro";
                        break;

                    case "Transferencia Interna":
                        if (string.IsNullOrEmpty(_cuentaOrigenPredefinida))
                        {
                            MessageBox.Show("No se ha especificado una cuenta de origen.", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        resultado = await RealizarTransferenciaInternaAsync(_cuentaOrigenPredefinida, numeroCuenta, monto, descripcion);
                        mensaje = resultado ? "Transferencia interna realizada exitosamente" : "Error al realizar la transferencia interna";
                        break;

                    case "Transferencia Externa":
                        if (string.IsNullOrEmpty(_cuentaOrigenPredefinida))
                        {
                            MessageBox.Show("No se ha especificado una cuenta de origen.", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        resultado = await RealizarTransferenciaExternaAsync(_cuentaOrigenPredefinida, numeroCuenta, monto, descripcion);
                        mensaje = resultado ? "Transferencia externa realizada exitosamente" : "Error al realizar la transferencia externa";
                        break;
                }

                if (resultado)
                {
                    MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar la transacción: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConfirmar.Enabled = true;
                btnConfirmar.Text = "Confirmar";
            }
        }

        private bool ValidarCampos()
        {
            if (cmbTipoTransaccion.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un tipo de transacción.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTipoTransaccion.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNumeroCuenta.Text))
            {
                MessageBox.Show("Ingrese el número de cuenta.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNumeroCuenta.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMonto.Text) || !decimal.TryParse(txtMonto.Text, out decimal monto) || monto <= 0)
            {
                MessageBox.Show("Ingrese un monto válido mayor a 0.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMonto.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Ingrese una descripción para la transacción.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
                return false;
            }

            return true;
        }

        private async Task<bool> RealizarDepositoAsync(string numeroCuenta, decimal monto, string descripcion)
        {
            try
            {
                // Nota: Necesitarás implementar este endpoint en el API
                var payload = new
                {
                    NumeroCuenta = numeroCuenta,
                    Monto = monto,
                    Descripcion = descripcion,
                    TipoTransaccion = "DEPOSITO"
                };

                var response = await _apiCliente.PostJsonAsync<object, Dictionary<string, object>>(
                    "/api/banco/transacciones/deposito", payload);

                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RealizarDepositoAsync: {ex.Message}");
                MessageBox.Show($"Error al realizar depósito: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> RealizarRetiroAsync(string numeroCuenta, decimal monto, string descripcion)
        {
            try
            {
                var payload = new
                {
                    NumeroCuenta = numeroCuenta,
                    Monto = monto,
                    Descripcion = descripcion,
                    TipoTransaccion = "RETIRO"
                };

                var response = await _apiCliente.PostJsonAsync<object, Dictionary<string, object>>(
                    "/api/banco/transacciones/retiro", payload);

                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RealizarRetiroAsync: {ex.Message}");
                MessageBox.Show($"Error al realizar retiro: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> RealizarTransferenciaInternaAsync(string cuentaOrigen, string cuentaDestino, decimal monto, string descripcion)
        {
            try
            {
                var payload = new
                {
                    CuentaOrigen = cuentaOrigen,
                    CuentaDestino = cuentaDestino,
                    Monto = monto,
                    Descripcion = descripcion
                };

                var response = await _apiCliente.PostJsonAsync<object, Dictionary<string, object>>(
                    "/api/banco/transacciones/transferencia-interna", payload);

                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RealizarTransferenciaInternaAsync: {ex.Message}");
                MessageBox.Show($"Error al realizar transferencia interna: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async Task<bool> RealizarTransferenciaExternaAsync(string cuentaOrigen, string cuentaDestino, decimal monto, string descripcion)
        {
            try
            {
                var payload = new
                {
                    CuentaOrigen = cuentaOrigen,
                    CuentaDestino = cuentaDestino,
                    Monto = monto,
                    Descripcion = descripcion
                };

                var response = await _apiCliente.PostJsonAsync<object, Dictionary<string, object>>(
                    "/api/banco/transacciones/transferencia-externa", payload);

                return response != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RealizarTransferenciaExternaAsync: {ex.Message}");
                MessageBox.Show($"Error al realizar transferencia externa: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
