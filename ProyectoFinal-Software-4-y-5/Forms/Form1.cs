using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinalDesarrollo4_5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        namespace ProyectoFinal_Software_4_y_5.Forms
    {
        public partial class FormLogin : Form
        {
            private ServicioBanco servicioBanco;

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
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(txtUsuario.Text))
                {
                    MessageBox.Show("Por favor ingrese su usuario", "Campo Requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsuario.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Por favor ingrese su contraseña", "Campo Requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                // Deshabilitar botón durante la validación
                btnLogin.Enabled = false;
                btnLogin.Text = "Validando...";
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    // Intentar login usando el servicio
                    Usuario usuario = servicioBanco.ValidarLogin(
                        txtUsuario.Text.Trim(),
                        txtPassword.Text
                    );

                    if (usuario != null)
                    {
                        // Login exitoso
                        ControladorSesion.UsuarioActual = usuario;

                        // Verificar si es administrador
                        if (usuario.Admin)
                        {
                            // Abrir Panel Admin
                            FormPanelAdmin panelAdmin = new FormPanelAdmin();
                            this.Hide();
                            panelAdmin.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Acceso solo para administradores", "Acceso Denegado",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            LimpiarCampos();
                        }
                    }
                    else
                    {
                        // Login fallido
                        MessageBox.Show("Usuario o contraseña incorrectos", "Error de Autenticación",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al iniciar sesión: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Restaurar botón
                    btnLogin.Enabled = true;
                    btnLogin.Text = "Iniciar Sesión";
                    this.Cursor = Cursors.Default;
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