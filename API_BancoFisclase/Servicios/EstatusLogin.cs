namespace API_BancoFisclase.Servicios
{
    public enum EstatusLogin
    {
        Success,
        NotFound,
        Locked,
        BadCredentials,
        Forbidden,
        Error
    }

    public class LoginResultDto
    {
        public EstatusLogin Status { get; set; }
        public string Message { get; set; }
        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        // ClienteId: the customer id associated to this user (if any)
        public int? ClienteId { get; set; }
        // IsAdmin flag reflected from DB
        public bool IsAdmin { get; set; }
    }

    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}