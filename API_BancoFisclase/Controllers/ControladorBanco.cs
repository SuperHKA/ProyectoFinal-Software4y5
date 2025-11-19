using API_BancoFisclase.Servicios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API_BancoFisclase.Controllers
{
    [ApiController]
    [Route("api/banco")]
    public class ControladorBanco : ControllerBase
    {
        private readonly IServicioBanco _banco;

        public ControladorBanco(IServicioBanco banco)
        {
            _banco = banco;
        }

        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrEmpty(req.Password))
                return BadRequest("username and password required");

            var res = await _banco.AuthenticateAdminAsync(req.Username.Trim(), req.Password).ConfigureAwait(false);
            switch (res.Status)
            {
                case EstatusLogin.Success: return Ok(res);
                case EstatusLogin.NotFound: return NotFound(res);
                case EstatusLogin.Locked: return StatusCode(423, res);
                case EstatusLogin.BadCredentials: return Unauthorized(res);
                case EstatusLogin.Forbidden: return Forbid();
                default: return StatusCode(500, res);
            }
        }

        [HttpGet("estadisticas")]
        public async Task<IActionResult> Estadisticas()
            => Ok(await _banco.ObtenerEstadisticasGeneralesAsync().ConfigureAwait(false));

        [HttpGet("movimientos-del-dia")]
        public async Task<IActionResult> MovimientosDelDia()
            => Ok(await _banco.ObtenerMovimientosDelDiaAsync().ConfigureAwait(false));

        [HttpGet("clientes")]
        public async Task<IActionResult> ObtenerClientes()
            => Ok(await _banco.ObtenerClientesAsync().ConfigureAwait(false));

        [HttpGet("transacciones-recientes")]
        public async Task<IActionResult> TransaccionesRecientes([FromQuery] int limit = 20)
            => Ok(await _banco.ObtenerTransaccionesRecientesAsync(limit).ConfigureAwait(false));

        [HttpGet("analisis")]
        public async Task<IActionResult> Analisis()
            => Ok(await _banco.ObtenerAnalisisTransaccionesAsync().ConfigureAwait(false));

        [HttpGet("resumen-clientes")]
        public async Task<IActionResult> ResumenClientes()
            => Ok(await _banco.ObtenerResumenClientesAsync().ConfigureAwait(false));

        [HttpGet("usuarios-bloqueados")]
        public async Task<IActionResult> UsuariosBloqueados()
            => Ok(await _banco.ObtenerUsuariosBloqueadosAsync().ConfigureAwait(false));

        [HttpGet("notificaciones-sistema")]
        public async Task<IActionResult> NotificacionesSistema()
            => Ok(await _banco.ObtenerNotificacionesSistemaAsync().ConfigureAwait(false));

        // Health endpoint - returns 200 if ProbarConsultaBasicaAsync returns "OK", otherwise 503
        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            try
            {
                var resultado = await _banco.ProbarConsultaBasicaAsync().ConfigureAwait(false);
                if (string.Equals(resultado, "OK", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok(new { status = "healthy" });
                }
                return StatusCode(503, new { status = "unhealthy", detail = resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { status = "unhealthy", detail = ex.Message });
            }
        }

        // Typed DTOs for creates/updates — controller accepts typed DTOs and passes them to IServicioBanco.
        [HttpPost("clientes")]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteCreateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.CrearClienteCompletoAsync(payload, payload.Password).ConfigureAwait(false);
            return Ok(ok);
        }

        [HttpPut("clientes/{id:int}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ClienteUpdateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.ActualizarClienteAsync(id, payload).ConfigureAwait(false);
            return Ok(ok);
        }

        [HttpPost("usuarios/{id:int}/cambiar-contrasena")]
        public async Task<IActionResult> CambiarContrasena(int id, [FromBody] ChangePasswordDto body)
        {
            if (body == null || string.IsNullOrEmpty(body.Password)) return BadRequest("Password required");
            var ok = await _banco.CambiarContrasenaAsync(id, body.Password).ConfigureAwait(false);
            return Ok(ok);
        }

        [HttpPost("cuentas")]
        public async Task<IActionResult> CrearCuenta([FromBody] CuentaCreateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.CrearCuentaBancariaAsync(payload).ConfigureAwait(false);
            return Ok(ok);
        }

        [HttpPost("usuarios/{id:int}/bloquear")]
        public async Task<IActionResult> BloquearUsuario(int id, [FromBody] BloquearDto body)
        {
            if (body == null) return BadRequest("payload required");
            var ok = await _banco.BloquearDesbloquearUsuarioAsync(id, body.Bloquear).ConfigureAwait(false);
            return Ok(ok);
        }

        [HttpPost("usuarios/admin")]
        public async Task<IActionResult> CrearAdmin([FromBody] AdminCreateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.CrearUsuarioAdministradorAsync(payload).ConfigureAwait(false);
            return Ok(ok);
        }

        [HttpGet("reporte")]
        public async Task<IActionResult> Reporte([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => Ok(await _banco.ObtenerReporteTransaccionesPorFechaAsync(desde, hasta).ConfigureAwait(false));

        [HttpPost("ejecutar-resumenes")]
        public async Task<IActionResult> EjecutarResumenes()
        {
            var ok = await _banco.EjecutarResumenesDiariosAsync().ConfigureAwait(false);
            return ok ? NoContent() : StatusCode(500, "Error ejecutando resúmenes");
        }
    }

    // Request DTOs (duplicate here for now; move to shared DTO project later)
    public class LoginRequestDto { public string Username { get; set; } public string Password { get; set; } }

    public class ClienteCreateDto
    {
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Password { get; set; }
    }

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

    public class ChangePasswordDto { public string Password { get; set; } }

    public class CuentaCreateDto
    {
        public int ClienteId { get; set; }
        public string Tipo { get; set; }
        public string NumeroCuenta { get; set; }
        public double SaldoInicial { get; set; }
    }

    public class BloquearDto { public bool Bloquear { get; set; } }

    public class AdminCreateDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}
