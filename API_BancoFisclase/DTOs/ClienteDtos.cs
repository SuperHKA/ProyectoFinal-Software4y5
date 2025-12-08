namespace API_BancoFisclase.DTOs
{
    /// <summary>
    /// Data Transfer Object para crear un nuevo cliente
    /// </summary>
    public class ClienteCreateDto
    {
        /// <summary>
        /// Nombre de usuario único
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Nombre completo del cliente
        /// </summary>
        public string Nombre { get; set; }
        
        /// <summary>
        /// Dirección de email del cliente
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Número de teléfono del cliente
        /// </summary>
        public string Telefono { get; set; }
        
        /// <summary>
        /// Contraseña inicial del usuario
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Data Transfer Object para actualizar información de cliente
    /// </summary>
    public class ClienteUpdateDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Ciudad { get; set; }
        public string EstadoProvincia { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }
    }

    /// <summary>
    /// Data Transfer Object para cambiar contraseña
    /// </summary>
    public class ChangePasswordDto 
    { 
        public string Password { get; set; } 
    }

    /// <summary>
    /// Data Transfer Object para crear cuenta bancaria
    /// </summary>
    public class CuentaCreateDto
    {
        public int ClienteId { get; set; }
        public string Tipo { get; set; }
        public string NumeroCuenta { get; set; }
        public double SaldoInicial { get; set; }
    }

    /// <summary>
    /// Data Transfer Object para bloquear/desbloquear usuario
    /// </summary>
    public class BloquearDto 
    { 
        public bool Bloquear { get; set; } 
    }

    /// <summary>
    /// Data Transfer Object para crear administrador
    /// </summary>
    public class AdminCreateDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}