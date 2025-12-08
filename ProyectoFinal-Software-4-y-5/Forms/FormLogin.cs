using ProyectoFinal_Software_4_y_5.Servicios;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoFinal_Software_4_y_5.Entidades.DTOs;


namespace ProyectoFinal_Software_4_y_5.Forms
{

    public partial class FormLogin : Form
    {
        private ServicioBancoApi servicioBanco;
        // Use the API scheme+port the server actually exposes (HTTP on 5202 per your logs)
        private readonly ApiAdmin _apiCliente = new ApiAdmin("http://localhost:5202"); // use your API URL

        public FormLogin()
        {
            InitializeComponent();
            // Use matching base URL for the adapter as well
            servicioBanco = new ServicioBancoApi("http://localhost:5202", allowInvalidSsl: false);

            // Configuraciones adicionales
            this.AcceptButton = btnLogin; // Enter ejecuta login
            txtPassword.PasswordChar = '•';
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            try
            {
                var username = txtUsuario.Text?.Trim();
                var password = txtPassword.Text ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Usuario y contraseña son obligatorios", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // Strict admin authentication
                    var result = await _apiCliente.AutenticarAdministradorAsync(username, password).ConfigureAwait(false);

                    // Switch back to UI thread
                    this.BeginInvoke((Action)(() =>
                    {
                        switch (result.Status)
                        {
                            case EstatusLogin.Success:
                                // Only allow admins
                                var panel = new PanelAdministrativo();
                                panel.Show();
                                this.Hide();
                                break;
                            case EstatusLogin.NotFound:
                                MessageBox.Show("Usuario no encontrado", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            case EstatusLogin.Locked:
                                MessageBox.Show("Cuenta bloqueada", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case EstatusLogin.BadCredentials:
                                MessageBox.Show("Credenciales incorrectas", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            case EstatusLogin.Forbidden:
                                // Explicit message for non-admins
                                MessageBox.Show("No es administrador. Acceso denegado.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                break;
                            default:
                                MessageBox.Show("Error de autenticación: " + result.Message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error comunicándose con la API: {ex.Message}", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                this.BeginInvoke((Action)(() => btnLogin.Enabled = true));
            }
        }

        private void LimpiarCampos()
        {
            txtUsuario.Clear();
            txtPassword.Clear();
            txtUsuario.Focus();
        }

        private void lnkRecuperar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Contacte al administrador del sistema para recuperar su contraseña.",
                "Recuperar Contraseña", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}