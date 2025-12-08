namespace API_BancoFisclase.DTOs
{
    /// <summary>
    /// Enumeration for login status results
    /// </summary>
    public enum EstatusLogin 
    { 
        Success, 
        NotFound, 
        Locked, 
        BadCredentials, 
        Forbidden, 
        Error 
    }

    /// <summary>
    /// Data Transfer Object for login results
    /// </summary>
    public class LoginResultDto
    {
        public EstatusLogin Status { get; set; }
        public string Message { get; set; }
        public UserDto User { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for user information
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        /// <summary>
        /// ClienteId: the customer id associated to this user (if any)
        /// </summary>
        public int? ClienteId { get; set; }
        /// <summary>
        /// IsAdmin flag reflected from DB
        /// </summary>
        public bool IsAdmin { get; set; }
    }

    /// <summary>
    /// Data Transfer Object para solicitudes de login
    /// </summary>
    public class LoginRequestDto 
    { 
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string Username { get; set; } 
        
        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        public string Password { get; set; } 
    }
}