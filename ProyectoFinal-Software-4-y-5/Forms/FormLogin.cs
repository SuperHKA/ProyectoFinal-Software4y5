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
        public FormLogin()
        {
            InitializeComponent();
            servicioBanco = new ServicioBanco();

            // Configuraciones adicionales
            this.AcceptButton = btnLogin; // Enter ejecuta login
            txtPassword.PasswordChar = '•';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsuario.Text.Trim();
            var password = txtPassword.Text ?? string.Empty;

            var result = servicioBanco.AutenticarAdministrador(username, password, out var usuario);

            switch (result)
            {
                case ServicioBanco.LoginResult.Success:
                    // Open admin panel and hide login; restore login when admin panel closes
                    var panel = new PanelAdministrativo();
                    panel.FormClosed += (s, args) => this.Show();
                    panel.Show();
                    this.Hide();
                    break;

                case ServicioBanco.LoginResult.UsuarioNoExiste:
                    MessageBox.Show("Usuario no existe.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case ServicioBanco.LoginResult.Bloqueado:
                    MessageBox.Show("Cuenta bloqueada por seguridad. Contacte con soporte.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case ServicioBanco.LoginResult.ContrasenaIncorrecta:
                    MessageBox.Show("Contraseña incorrecta. Intentos incrementados.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case ServicioBanco.LoginResult.NoAdmin:
                    MessageBox.Show("No tiene permisos de administrador.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;

                default:
                    MessageBox.Show("Error desconocido al iniciar sesión.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
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