using ProyectoFinal_Software_4_y_5.Servicios;
using ProyectoFinal_Software_4_y_5.Entidades;
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

    public partial class FormLogin : Form
    {
        private ServicioBanco servicioBanco;
        private ControladorSession controladorSesion;
        private readonly ApiCliente _apiCliente = new ApiCliente("https://localhost:5001"); // use your API URL

        public FormLogin()
        {
            InitializeComponent();
            servicioBanco = new ServicioBanco();

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

                var result = await _apiCliente.AutenticarAdministradorAsync(username, password).ConfigureAwait(false);

                // back to UI thread
                this.Invoke((Action)(() =>
                {
                    switch (result.Status)
                    {
                        case EstatusLogin.Success:
                            // set local session if you want, open admin panel
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
                            MessageBox.Show("No es administrador", "Login", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            break;
                        default:
                            MessageBox.Show("Error de autenticación: " + result.Message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }));
            }
            finally
            {
                this.Invoke((Action)(() => btnLogin.Enabled = true));
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