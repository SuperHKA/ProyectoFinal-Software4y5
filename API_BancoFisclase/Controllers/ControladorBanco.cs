using API_BancoFisclase.Servicios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API_BancoFisclase.Controllers
{
    /*
     Controlador API para operaciones del banco.

     Breve descripción:
     - Expone endpoints para autenticación, consultas y acciones sobre clientes, cuentas y transacciones.
     - Devuelve DTOs y códigos HTTP apropiados según el resultado.
    */
    [ApiController]
    [Route("api/banco")]
    public class ControladorBanco : ControllerBase
    {
        private readonly IServicioBanco _banco;

        public ControladorBanco(IServicioBanco banco)
        {
            _banco = banco;
        }

        // Endpoint para login de administradores
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

        // Endpoint para login general de usuarios (clientes) or admins (auto-detect)
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrEmpty(req.Password))
                return BadRequest("username and password required");

            // Check DB flag to decide whether to perform admin authentication first
            var isAdminFlag = await _banco.IsUsernameAdminAsync(req.Username.Trim()).ConfigureAwait(false);
            if (isAdminFlag)
            {
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

            // Not an admin -> normal user authentication
            var userRes = await _banco.AuthenticateUserAsync(req.Username.Trim(), req.Password).ConfigureAwait(false);
            switch (userRes.Status)
            {
                case EstatusLogin.Success: return Ok(userRes);
                case EstatusLogin.NotFound: return NotFound(userRes);
                case EstatusLogin.Locked: return StatusCode(423, userRes);
                case EstatusLogin.BadCredentials: return Unauthorized(userRes);
                default: return StatusCode(500, userRes);
            }
        }

        // Devuelve estadísticas generales
        [HttpGet("estadisticas")]
        public async Task<IActionResult> Estadisticas()
            => Ok(await _banco.ObtenerEstadisticasGeneralesAsync().ConfigureAwait(false));

        // Devuelve los movimientos del día
        [HttpGet("movimientos-del-dia")]
        public async Task<IActionResult> MovimientosDelDia()
            => Ok(await _banco.ObtenerMovimientosDelDiaAsync().ConfigureAwait(false));

        // Devuelve la lista de clientes (supports pagination)
        [HttpGet("clientes")]
        public async Task<IActionResult> ObtenerClientes([FromQuery] int page = 1, [FromQuery] int pageSize = 100, [FromQuery] string search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 1000) pageSize = 100;
            var res = await _banco.ObtenerClientesPaginadoAsync(page, pageSize, search).ConfigureAwait(false);
            return Ok(new { items = res.Items, total = res.Total, page, pageSize });
        }

        // Devuelve cuentas de un cliente por id
        [HttpGet("clientes/{clienteId:int}/cuentas")]
        public async Task<IActionResult> ObtenerCuentasPorCliente([FromRoute] int clienteId)
        {
            if (clienteId <= 0) return BadRequest("clienteId required");
            var cuentas = await _banco.ObtenerCuentasPorClienteAsync(clienteId).ConfigureAwait(false);
            return Ok(cuentas);
        }

        // Devuelve cuentas por nombre de usuario (helper for clients when clienteId not present client-side)
        [HttpGet("clientes/por-usuario")]
        public async Task<IActionResult> ObtenerCuentasPorUsuario([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("username required");
            var clienteId = await _banco.GetClienteIdByUsernameAsync(username.Trim()).ConfigureAwait(false);
            if (!clienteId.HasValue) return NotFound(new { message = "Cliente not found for username" });
            var cuentas = await _banco.ObtenerCuentasPorClienteAsync(clienteId.Value).ConfigureAwait(false);
            return Ok(cuentas);
        }

        // Transacciones recientes (paginated/filterable)
        [HttpGet("transacciones-recientes")]
        public async Task<IActionResult> TransaccionesRecientes([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null, [FromQuery] string q = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 1000) pageSize = 20;
            var res = await _banco.ObtenerTransaccionesPaginadasAsync(page, pageSize, desde, hasta, q).ConfigureAwait(false);
            return Ok(new { items = res.Items, total = res.Total, page, pageSize });
        }

        // Análisis de transacciones
        [HttpGet("analisis")]
        public async Task<IActionResult> Analisis()
            => Ok(await _banco.ObtenerAnalisisTransaccionesAsync().ConfigureAwait(false));

        // Resumen por clientes
        [HttpGet("resumen-clientes")]
        public async Task<IActionResult> ResumenClientes()
            => Ok(await _banco.ObtenerResumenClientesAsync().ConfigureAwait(false));

        // Usuarios bloqueados
        [HttpGet("usuarios-bloqueados")]
        public async Task<IActionResult> UsuariosBloqueados()
            => Ok(await _banco.ObtenerUsuariosBloqueadosAsync().ConfigureAwait(false));

        // Notificaciones del sistema
        [HttpGet("notificaciones-sistema")]
        public async Task<IActionResult> NotificacionesSistema()
            => Ok(await _banco.ObtenerNotificacionesSistemaAsync().ConfigureAwait(false));

        // Endpoint de salud (health) — devuelve 200 si la consulta básica responde OK
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

        // Crear cliente
        [HttpPost("clientes")]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteCreateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.CrearClienteCompletoAsync(payload, payload.Password).ConfigureAwait(false);
            return Ok(ok);
        }

        // Actualizar cliente
        [HttpPut("clientes/{id:int}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ClienteUpdateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.ActualizarClienteAsync(id, payload).ConfigureAwait(false);
            return Ok(ok);
        }

        // Cambiar contraseña de usuario
        [HttpPost("usuarios/{id:int}/cambiar-contrasena")]
        public async Task<IActionResult> CambiarContrasena(int id, [FromBody] ChangePasswordDto body)
        {
            if (body == null || string.IsNullOrEmpty(body.Password)) return BadRequest("Password required");
            var ok = await _banco.CambiarContrasenaAsync(id, body.Password).ConfigureAwait(false);
            return Ok(ok);
        }

        // Crear cuenta bancaria
        [HttpPost("cuentas")]
        public async Task<IActionResult> CrearCuenta([FromBody] CuentaCreateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.CrearCuentaBancariaAsync(payload).ConfigureAwait(false);
            return Ok(ok);
        }

        // Bloquear/desbloquear usuario
        [HttpPost("usuarios/{id:int}/bloquear")]
        public async Task<IActionResult> BloquearUsuario(int id, [FromBody] BloquearDto body)
        {
            if (body == null) return BadRequest("payload required");
            var ok = await _banco.BloquearDesbloquearUsuarioAsync(id, body.Bloquear).ConfigureAwait(false);
            return Ok(ok);
        }

        // Crear administrador
        [HttpPost("usuarios/admin")]
        public async Task<IActionResult> CrearAdmin([FromBody] AdminCreateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.CrearUsuarioAdministradorAsync(payload).ConfigureAwait(false);
            return Ok(ok);
        }

        // Reporte de transacciones por fecha
        [HttpGet("reporte")]
        public async Task<IActionResult> Reporte([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => Ok(await _banco.ObtenerReporteTransaccionesPorFechaAsync(desde, hasta).ConfigureAwait(false));

        // Ejecutar resúmenes diarios
        [HttpPost("ejecutar-resumenes")]
        public async Task<IActionResult> EjecutarResumenes()
        {
            var ok = await _banco.EjecutarResumenesDiariosAsync().ConfigureAwait(false);
            return ok ? NoContent() : StatusCode(500, "Error ejecutando resúmenes");
        }

        // Transferencia interna
        [HttpPost("transfer/internal")]
        public async Task<IActionResult> TransferInternal([FromBody] TransferRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.CuentaOrigen) || string.IsNullOrWhiteSpace(req.CuentaDestino) || req.Monto <= 0)
                return BadRequest("invalid request");

            try
            {
                var (success, message, transaccionId) = await _banco.TransferenciaInternaAsync(req.CuentaOrigen, req.CuentaDestino, req.Monto, req.Descripcion, req.UsuarioId ?? 0).ConfigureAwait(false);
                if (success) return Ok(new { success = true, transaccionId, message });
                return BadRequest(new { success = false, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Transferencia externa
        [HttpPost("transfer/external")]
        public async Task<IActionResult> TransferExternal([FromBody] TransferRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.CuentaOrigen) || string.IsNullOrWhiteSpace(req.CuentaDestino) || req.Monto <= 0)
                return BadRequest("invalid request");

            try
            {
                var (success, message, transaccionId) = await _banco.TransferenciaExternaAsync(req.CuentaOrigen, req.CuentaDestino, req.Monto, req.Descripcion, req.UsuarioId ?? 0).ConfigureAwait(false);
                if (success) return Ok(new { success = true, transaccionId, message });
                return BadRequest(new { success = false, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Endpoint to check if a username is flagged as admin (used by frontend to decide auth path)
        [HttpGet("is-admin")]
        public async Task<IActionResult> IsAdmin([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("username required");
            var isAdmin = await _banco.IsUsernameAdminAsync(username.Trim()).ConfigureAwait(false);
            return Ok(new { username = username.Trim(), isAdmin });
        }

        // DTO para solicitudes de transferencia
        public class TransferRequestDto
        {
            public string CuentaOrigen { get; set; }
            public string CuentaDestino { get; set; }
            public decimal Monto { get; set; }
            public string Descripcion { get; set; }
            public int? UsuarioId { get; set; }
        }

    }

    // DTOs de solicitud (duplicados aquí por ahora; mover a proyecto compartido más adelante)
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
