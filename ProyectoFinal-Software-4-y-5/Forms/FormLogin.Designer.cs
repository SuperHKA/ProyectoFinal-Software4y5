namespace ProyectoFinal_Software_4_y_5.Forms
{
    partial class FormLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlLeftSide = new System.Windows.Forms.Panel();
            this.picBankLogo = new System.Windows.Forms.PictureBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle2 = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lnkRecuperar = new System.Windows.Forms.LinkLabel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.pnlLeftSide.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBankLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlLeftSide
            // 
            this.pnlLeftSide.BackColor = System.Drawing.Color.SlateBlue;
            this.pnlLeftSide.Controls.Add(this.picBankLogo);
            this.pnlLeftSide.Controls.Add(this.lblDescription);
            this.pnlLeftSide.Controls.Add(this.lblSubtitle);
            this.pnlLeftSide.Controls.Add(this.lblAppName);
            this.pnlLeftSide.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftSide.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlLeftSide.Name = "pnlLeftSide";
            this.pnlLeftSide.Size = new System.Drawing.Size(528, 814);
            this.pnlLeftSide.TabIndex = 0;
            // 
            // picBankLogo
            // 
            this.picBankLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBankLogo.Image = global::ProyectoFinal_Software_4_y_5.Properties.Resources.Screenshot_2025_11_10_184112;
            this.picBankLogo.Location = new System.Drawing.Point(16, 154);
            this.picBankLogo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picBankLogo.Name = "picBankLogo";
            this.picBankLogo.Size = new System.Drawing.Size(63, 100);
            this.picBankLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBankLogo.TabIndex = 6;
            this.picBankLogo.TabStop = false;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDescription.Location = new System.Drawing.Point(85, 288);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(380, 69);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Gestiona todas las operaciones de tu institución \r\nfinanciera desde un solo lugar" +
    " , seguro \r\nrapido y  confiable.\r\n";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblSubtitle.Location = new System.Drawing.Point(84, 242);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(313, 25);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Sistema de Administración Bancaria";
            // 
            // lblAppName
            // 
            this.lblAppName.AutoSize = true;
            this.lblAppName.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAppName.ForeColor = System.Drawing.Color.White;
            this.lblAppName.Location = new System.Drawing.Point(117, 172);
            this.lblAppName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(196, 54);
            this.lblAppName.TabIndex = 0;
            this.lblAppName.Text = "FISCLASE";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(676, 95);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(233, 46);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Iniciar Sesión";
            // 
            // lblSubtitle2
            // 
            this.lblSubtitle2.AutoSize = true;
            this.lblSubtitle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle2.ForeColor = System.Drawing.Color.Gray;
            this.lblSubtitle2.Location = new System.Drawing.Point(652, 140);
            this.lblSubtitle2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSubtitle2.Name = "lblSubtitle2";
            this.lblSubtitle2.Size = new System.Drawing.Size(295, 23);
            this.lblSubtitle2.TabIndex = 3;
            this.lblSubtitle2.Text = "Ingresa tus credenciales para acceder";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsuario.Location = new System.Drawing.Point(685, 229);
            this.txtUsuario.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(232, 34);
            this.txtUsuario.TabIndex = 5;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Location = new System.Drawing.Point(680, 298);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(110, 28);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Contraseña";
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(685, 327);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(232, 34);
            this.txtPassword.TabIndex = 7;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(99)))), ((int)(((byte)(255)))));
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(712, 417);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(173, 38);
            this.btnLogin.TabIndex = 8;
            this.btnLogin.Text = "Iniciar Sesión";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lnkRecuperar
            // 
            this.lnkRecuperar.AutoSize = true;
            this.lnkRecuperar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkRecuperar.Location = new System.Drawing.Point(665, 484);
            this.lnkRecuperar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkRecuperar.Name = "lnkRecuperar";
            this.lnkRecuperar.Size = new System.Drawing.Size(252, 18);
            this.lnkRecuperar.TabIndex = 9;
            this.lnkRecuperar.TabStop = true;
            this.lnkRecuperar.Text = "¿Olvidaste tu contraseña? Recuperar";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.Location = new System.Drawing.Point(681, 204);
            this.lblUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(68, 23);
            this.lblUsername.TabIndex = 10;
            this.lblUsername.Text = "Usuario";
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1081, 593);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lnkRecuperar);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblSubtitle2);
            this.Controls.Add(this.pnlLeftSide);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BankAdmin Pro - Login";
            this.pnlLeftSide.ResumeLayout(false);
            this.pnlLeftSide.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBankLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlLeftSide;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle2;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.LinkLabel lnkRecuperar;
        private System.Windows.Forms.PictureBox picBankLogo;
        private System.Windows.Forms.Label lblUsername;
    }
}

