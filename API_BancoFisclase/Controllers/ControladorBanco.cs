using API_BancoFisclase.Servicios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using LoginResultDto = API_BancoFisclase.Servicios.LoginResultDto;
using EstatusLogin = API_BancoFisclase.Servicios.EstatusLogin;
using API_BancoFisclase.DTOs;

namespace API_BancoFisclase.Controllers
{
    // Controlador API para operaciones del sistema bancario Fisclase.
    // Proporciona endpoints para autenticación, gestión de clientes, cuentas y transacciones.
    [ApiController]
    [Route("api/banco")]
    public class ControladorBanco : ControllerBase
    {
        private readonly IServicioBanco _banco;

        public ControladorBanco(IServicioBanco banco)
        {
            _banco = banco;
        }

        // Autenticación de administradores del sistema
        // req: Credenciales de login (username y password)
        // Retorna: Resultado del proceso de autenticación con información del usuario
        // 200: Login exitoso
        // 401: Credenciales incorrectas  
        // 404: Usuario no encontrado
        // 423: Cuenta bloqueada
        [HttpPost("login-admin")]
        [ProducesResponseType(typeof(LoginResultDto), 200)]
        [ProducesResponseType(typeof(LoginResultDto), 401)]
        [ProducesResponseType(typeof(LoginResultDto), 404)]
        [ProducesResponseType(typeof(LoginResultDto), 423)]
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

        // Autenticación de usuarios (clientes y administradores)
        // req: Credenciales de login
        // Retorna: Resultado del proceso de autenticación
        // 200: Login exitoso
        // 401: Credenciales incorrectas
        // 404: Usuario no encontrado
        // 423: Cuenta bloqueada
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResultDto), 200)]
        [ProducesResponseType(typeof(LoginResultDto), 401)]
        [ProducesResponseType(typeof(LoginResultDto), 404)]
        [ProducesResponseType(typeof(LoginResultDto), 423)]
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

        // Obtiene estadísticas generales del sistema bancario
        // Retorna: Estadísticas del sistema (clientes, cuentas, transacciones, etc.)
        // 200: Estadísticas obtenidas exitosamente
        [HttpGet("estadisticas")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> Estadisticas()
            => Ok(await _banco.ObtenerEstadisticasGeneralesAsync().ConfigureAwait(false));

        // Obtiene los movimientos realizados en el día actual
        // Retorna: Lista de movimientos del día
        // 200: Movimientos obtenidos exitosamente
        [HttpGet("movimientos-del-dia")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public async Task<IActionResult> MovimientosDelDia()
            => Ok(await _banco.ObtenerMovimientosDelDiaAsync().ConfigureAwait(false));

        // Obtiene la lista de clientes del sistema
        // page: Número de página (default: 1)
        // pageSize: Tamaño de página (default: 100, max: 1000)
        // search: Término de búsqueda opcional
        // Retorna: Lista paginada de clientes
        // 200: Clientes obtenidos exitosamente
        [HttpGet("clientes")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ObtenerClientes([FromQuery] int page = 1, [FromQuery] int pageSize = 100, [FromQuery] string search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 1000) pageSize = 100;
            
            // Si no se especifican parámetros de paginación, usar el método simple sin paginación
            if (page == 1 && pageSize == 100 && string.IsNullOrWhiteSpace(search))
            {
                // Usar el método que ya funciona correctamente con el panel administrativo
                var simpleResult = await _banco.ObtenerClientesAsync().ConfigureAwait(false);
                return Ok(simpleResult);
            }
            
            // Si se especifican parámetros, usar paginación
            var res = await _banco.ObtenerClientesPaginadoAsync(page, pageSize, search).ConfigureAwait(false);
            return Ok(new { items = res.Items, total = res.Total, page, pageSize });
        }

        // Obtiene las cuentas de un cliente específico
        // clienteId: ID del cliente
        // Retorna: Lista de cuentas del cliente
        // 200: Cuentas obtenidas exitosamente
        // 400: ID de cliente inválido
        [HttpGet("clientes/{clienteId:int}/cuentas")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObtenerCuentasPorCliente([FromRoute] int clienteId)
        {
            if (clienteId <= 0) return BadRequest("clienteId required");
            var cuentas = await _banco.ObtenerCuentasPorClienteAsync(clienteId).ConfigureAwait(false);
            return Ok(cuentas);
        }

        // Devuelve información completa del cliente y sus cuentas por nombre de usuario
        [HttpGet("clientes/por-usuario")]
        public async Task<IActionResult> ObtenerClienteInfoPorUsuario([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("username required");
            var clienteId = await _banco.GetClienteIdByUsernameAsync(username.Trim()).ConfigureAwait(false);
            if (!clienteId.HasValue) return NotFound(new { message = "Cliente not found for username" });
            
            // Obtener información del cliente Y las cuentas
            var cliente = await _banco.ObtenerClientePorIdAsync(clienteId.Value).ConfigureAwait(false);
            var cuentas = await _banco.ObtenerCuentasPorClienteAsync(clienteId.Value).ConfigureAwait(false);
            
            return Ok(new { 
                cliente = cliente,
                cuentas = cuentas
            });
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

        // Notificaciones de un cliente específico
        [HttpGet("clientes/{clienteId:int}/notificaciones")]
        public async Task<IActionResult> ObtenerNotificacionesCliente(int clienteId, [FromQuery] int dias = 30)
        {
            try
            {
                var notificaciones = await _banco.ObtenerNotificacionesClienteAsync(clienteId, dias).ConfigureAwait(false);
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

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
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] object payload)
        {
            if (payload == null) return BadRequest("payload required");
            var ok = await _banco.ActualizarClienteAsync(id, payload).ConfigureAwait(false);
            return Ok(ok);
        }

        // Eliminar cliente (soft delete)
        [HttpDelete("clientes/{id:int}")]
        public async Task<IActionResult> EliminarCliente(int id, [FromQuery] int usuarioIdAdmin = 1)
        {
            try
            {
                var resultado = await _banco.EliminarClienteAsync(id, usuarioIdAdmin).ConfigureAwait(false);
                
                if (resultado.Success)
                {
                    return Ok(new { success = true, message = resultado.Message });
                }
                else
                {
                    return BadRequest(new { success = false, message = resultado.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error al eliminar cliente: {ex.Message}" });
            }
        }

        // Cambiar contraseña de usuario
        [HttpPost("usuarios/{id:int}/cambiar-contrasena")]
        public async Task<IActionResult> CambiarContrasena(int id, [FromBody] ChangePasswordDto body)
        {
            if (body == null || string.IsNullOrEmpty(body.Password)) return BadRequest("Password required");
            var ok = await _banco.CambiarContrasenaAsync(id, body.Password).ConfigureAwait(false);
            return Ok(ok);
        }

        // Actualizar saldo de cuenta (solo administradores)
        [HttpPost("cuentas/actualizar-saldo")]
        public async Task<IActionResult> ActualizarSaldo([FromBody] ActualizarSaldoDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.NumeroCuenta))
                return BadRequest(new { success = false, message = "Datos inválidos" });

            if (dto.NuevoSaldo < 0)
                return BadRequest(new { success = false, message = "El saldo no puede ser negativo" });

            if (string.IsNullOrEmpty(dto.Motivo))
                return BadRequest(new { success = false, message = "Debe especificar un motivo para el ajuste" });

            try
            {
                var resultado = await _banco.ActualizarSaldoCuentaAsync(dto.NumeroCuenta, dto.NuevoSaldo, dto.Motivo, dto.UsuarioId).ConfigureAwait(false);
                
                if (resultado.Success)
                {
                    return Ok(new { success = true, message = resultado.Message });
                }
                else
                {
                    return BadRequest(new { success = false, message = resultado.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error al actualizar saldo: {ex.Message}" });
            }
        }

        // Crear cuenta bancaria
        [HttpPost("cuentas")]
        public async Task<IActionResult> CrearCuenta([FromBody] CuentaCreateDto payload)
        {
            if (payload == null) return BadRequest("payload required");
            
            // If ClienteId is not provided, try to get it from a username in the payload or context
            if (payload.ClienteId <= 0)
            {
                return BadRequest("ClienteId is required");
            }
            
            try
            {
                var ok = await _banco.CrearCuentaBancariaAsync(payload).ConfigureAwait(false);
                return Ok(new { success = ok, message = ok ? "Cuenta creada exitosamente" : "Error al crear cuenta" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
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

        // Realizar transferencia interna entre cuentas del mismo banco
        // req: Datos de la transferencia (cuentas origen/destino, monto, descripción)
        // Retorna: Resultado de la transferencia con ID de transacción
        // 200: Transferencia realizada exitosamente
        // 400: Datos de transferencia inválidos o transferencia rechazada
        // 500: Error interno del servidor
        [HttpPost("transfer/internal")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> TransferInternal([FromBody] TransferRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.CuentaOrigen) || string.IsNullOrWhiteSpace(req.CuentaDestino) || req.Monto <= 0)
                return BadRequest("invalid request");

            try
            {
                // Obtener usuarioId desde username si no viene en el request
                int usuarioId = req.UsuarioId ?? 0;
                if (usuarioId == 0 && !string.IsNullOrWhiteSpace(req.Username))
                {
                    var uid = await _banco.GetUsuarioIdByUsernameAsync(req.Username).ConfigureAwait(false);
                    usuarioId = uid ?? 0;
                }
                
                var (success, message, transaccionId) = await _banco.TransferenciaInternaAsync(req.CuentaOrigen, req.CuentaDestino, req.Monto, req.Descripcion, usuarioId).ConfigureAwait(false);
                if (success) return Ok(new { success = true, transaccionId, message });
                return BadRequest(new { success = false, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Realizar transferencia externa hacia cuentas de otros bancos o clientes externos
        // req: Datos de la transferencia externa
        // Retorna: Resultado de la transferencia con ID de transacción
        // 200: Transferencia realizada exitosamente
        // 400: Datos de transferencia inválidos o transferencia rechazada
        // 500: Error interno del servidor
        [HttpPost("transfer/external")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> TransferExternal([FromBody] TransferRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.CuentaOrigen) || string.IsNullOrWhiteSpace(req.CuentaDestino) || req.Monto <= 0)
                return BadRequest("invalid request");

            try
            {
                // Obtener usuarioId desde username si no viene en el request
                int usuarioId = req.UsuarioId ?? 0;
                if (usuarioId == 0 && !string.IsNullOrWhiteSpace(req.Username))
                {
                    var uid = await _banco.GetUsuarioIdByUsernameAsync(req.Username).ConfigureAwait(false);
                    usuarioId = uid ?? 0;
                }
                
                var (success, message, transaccionId) = await _banco.TransferenciaExternaAsync(req.CuentaOrigen, req.CuentaDestino, req.Monto, req.Descripcion, usuarioId).ConfigureAwait(false);
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

        // NEW: Debug endpoint to check user configuration
        [HttpGet("debug/user")]
        public async Task<IActionResult> DebugUser([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("username required");
            
            try
            {
                var isAdmin = await _banco.IsUsernameAdminAsync(username.Trim()).ConfigureAwait(false);
                var clienteId = await _banco.GetClienteIdByUsernameAsync(username.Trim()).ConfigureAwait(false);
                
                var debugInfo = new
                {
                    username = username.Trim(),
                    isAdmin = isAdmin,
                    clienteId = clienteId,
                    hasClienteId = clienteId.HasValue,
                    timestamp = DateTime.Now
                };

                // If we have a clienteId, also get the accounts and transactions count
                if (clienteId.HasValue)
                {
                    try
                    {
                        var cuentas = await _banco.ObtenerCuentasPorClienteAsync(clienteId.Value).ConfigureAwait(false);
                        var transacciones = await _banco.ObtenerTransaccionesClienteAsync(clienteId.Value, 5).ConfigureAwait(false);
                        
                        return Ok(new
                        {
                            debugInfo.username,
                            debugInfo.isAdmin,
                            debugInfo.clienteId,
                            debugInfo.hasClienteId,
                            debugInfo.timestamp,
                            accountsCount = cuentas.Count(),
                            transactionsCount = transacciones.Count(),
                            sampleAccounts = cuentas.Take(3).ToList(),
                            sampleTransactions = transacciones.Take(3).ToList()
                        });
                    }
                    catch (Exception ex)
                    {
                        return Ok(new
                        {
                            debugInfo.username,
                            debugInfo.isAdmin,
                            debugInfo.clienteId,
                            debugInfo.hasClienteId,
                            debugInfo.timestamp,
                            dataError = ex.Message
                        });
                    }
                }

                return Ok(debugInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, username });
            }
        }

        // NEW: Datos específicos para un cliente (Dashboard personal)
        [HttpGet("clientes/{clienteId:int}/dashboard")]
        public async Task<IActionResult> DashboardCliente([FromRoute] int clienteId)
        {
            if (clienteId <= 0) return BadRequest("clienteId required");
            
            try
            {
                Console.WriteLine($"DEBUG: Getting dashboard for clienteId: {clienteId}");
                
                // Obtener datos específicos del cliente
                var cuentas = await _banco.ObtenerCuentasPorClienteAsync(clienteId).ConfigureAwait(false);
                var cuentasList = cuentas.ToList();
                
                Console.WriteLine($"DEBUG: Found {cuentasList.Count} accounts for clienteId {clienteId}");
                
                // Calcular estadísticas del cliente
                var totalSaldo = cuentasList.Where(c => c.ContainsKey("saldo"))
                    .Sum(c => Convert.ToDecimal(c["saldo"] ?? 0));
                var totalCuentas = cuentasList.Count;
                
                Console.WriteLine($"DEBUG: Total saldo: {totalSaldo}, Total cuentas: {totalCuentas}");
                
                // Para las transacciones del cliente, necesitamos un método específico
                var transaccionesCliente = await _banco.ObtenerTransaccionesClienteAsync(clienteId).ConfigureAwait(false);
                var transaccionesClienteList = transaccionesCliente.ToList();
                
                Console.WriteLine($"DEBUG: Found {transaccionesClienteList.Count} transactions for clienteId {clienteId}");
                
                var transaccionesHoy = transaccionesClienteList.Where(t => 
                {
                    if (t.TryGetValue("fecha", out var fechaObj) && fechaObj != null)
                    {
                        if (DateTime.TryParse(fechaObj.ToString(), out var fecha))
                        {
                            return fecha.Date == DateTime.Today;
                        }
                    }
                    return false;
                }).Count();
                
                Console.WriteLine($"DEBUG: Found {transaccionesHoy} transactions today for clienteId {clienteId}");
                
                var dashboard = new
                {
                    saldo_total = totalSaldo,
                    total_cuentas = totalCuentas,
                    transacciones_hoy = transaccionesHoy,
                    cliente_id = clienteId
                };
                
                Console.WriteLine($"DEBUG: Returning dashboard data: {System.Text.Json.JsonSerializer.Serialize(dashboard)}");
                
                return Ok(new[] { dashboard });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Error in DashboardCliente: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, clienteId });
            }
        }

        // NEW: Transacciones específicas de un cliente
        [HttpGet("clientes/{clienteId:int}/transacciones")]
        public async Task<IActionResult> TransaccionesCliente([FromRoute] int clienteId, [FromQuery] int limit = 20)
        {
            if (clienteId <= 0) return BadRequest("clienteId required");
            
            try
            {
                var transacciones = await _banco.ObtenerTransaccionesClienteAsync(clienteId, limit).ConfigureAwait(false);
                return Ok(transacciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // NEW: Dashboard por username (para facilitar el acceso desde el frontend)
        [HttpGet("dashboard/usuario")]
        public async Task<IActionResult> DashboardPorUsuario([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("username required");
            
            try
            {
                var clienteId = await _banco.GetClienteIdByUsernameAsync(username.Trim()).ConfigureAwait(false);
                if (!clienteId.HasValue) 
                {
                    // Debug logging
                    Console.WriteLine($"DEBUG: No clienteId found for username: {username}");
                    return NotFound(new { message = "Cliente not found for username", username });
                }
                
                Console.WriteLine($"DEBUG: Found clienteId {clienteId.Value} for username: {username}");
                return await DashboardCliente(clienteId.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Error in DashboardPorUsuario: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, username });
            }
        }

        // NEW: Transacciones por username
        [HttpGet("transacciones/usuario")]
        public async Task<IActionResult> TransaccionesPorUsuario([FromQuery] string username, [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("username required");
            
            try
            {
                var clienteId = await _banco.GetClienteIdByUsernameAsync(username.Trim()).ConfigureAwait(false);
                if (!clienteId.HasValue) 
                {
                    Console.WriteLine($"DEBUG: No clienteId found for transactions username: {username}");
                    return NotFound(new { message = "Cliente not found for username", username });
                }
                
                Console.WriteLine($"DEBUG: Found clienteId {clienteId.Value} for transactions username: {username}");
                return await TransaccionesCliente(clienteId.Value, limit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Error in TransaccionesPorUsuario: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, username });
            }
        }

        // NEW: Endpoints para dropdowns de transferencias
        // Obtiene todas las cuentas activas del sistema para dropdowns de transferencia
        // Retorna: Lista de todas las cuentas con información básica
        // 200: Cuentas obtenidas exitosamente
        // 500: Error interno del servidor
        [HttpGet("cuentas")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ObtenerTodasLasCuentas()
        {
            try
            {
                var cuentas = await _banco.ObtenerTodasLasCuentasAsync().ConfigureAwait(false);
                return Ok(cuentas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Obtiene todos los clientes activos del sistema para dropdowns de transferencia
        // Retorna: Lista de todos los clientes con cuentas activas
        // 200: Clientes obtenidos exitosamente
        // 500: Error interno del servidor
        [HttpGet("clientes-transferencia")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ObtenerClientesParaTransferencia()
        {
            try
            {
                var clientes = await _banco.ObtenerTodosLosClientesParaTransferenciaAsync().ConfigureAwait(false);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DTO para solicitudes de transferencia
        // Datos de solicitud de transferencia
        public class TransferRequestDto
        {
            // Número de cuenta o ID de cuenta origen
            public string CuentaOrigen { get; set; }
            
            // Número de cuenta destino (para transferencias internas) o ID de cliente (para transferencias externas)
            public string CuentaDestino { get; set; }
            
            // Monto a transferir (debe ser mayor a 0)
            public decimal Monto { get; set; }
            
            // Descripción opcional de la transferencia
            public string Descripcion { get; set; }
            
            // ID del usuario que realiza la transferencia
            public int? UsuarioId { get; set; }
            
            // Nombre de usuario que realiza la transferencia (alternativa a UsuarioId)
            public string Username { get; set; }
        }

        // DTO para depósitos y retiros
        public class TransaccionSimpleDto
        {
            public string NumeroCuenta { get; set; }
            public decimal Monto { get; set; }
            public string Descripcion { get; set; }
            public string TipoTransaccion { get; set; }
        }

        // Endpoint para realizar depósito
        [HttpPost("transacciones/deposito")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> RealizarDeposito([FromBody] TransaccionSimpleDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.NumeroCuenta))
                    return BadRequest(new { error = "Datos inválidos", message = "El número de cuenta es requerido" });

                if (request.Monto <= 0)
                    return BadRequest(new { error = "Monto inválido", message = "El monto debe ser mayor a 0" });

                // Por ahora, usar usuario ID 1 como predeterminado (administrador)
                // En producción, esto debería venir del token de autenticación
                var resultado = await _banco.TransferenciaInternaAsync(
                    request.NumeroCuenta, 
                    request.NumeroCuenta, 
                    request.Monto, 
                    request.Descripcion ?? "Depósito", 
                    1
                ).ConfigureAwait(false);

                if (resultado.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = resultado.Message,
                        transaccionId = resultado.TransaccionId
                    });
                }

                return BadRequest(new { error = "Depósito fallido", message = resultado.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno", message = ex.Message });
            }
        }

        // Endpoint para realizar retiro
        [HttpPost("transacciones/retiro")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> RealizarRetiro([FromBody] TransaccionSimpleDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.NumeroCuenta))
                    return BadRequest(new { error = "Datos inválidos", message = "El número de cuenta es requerido" });

                if (request.Monto <= 0)
                    return BadRequest(new { error = "Monto inválido", message = "El monto debe ser mayor a 0" });

                // Implementar lógica de retiro (debitar la cuenta)
                // Por ahora usar transferencia interna con monto negativo
                var resultado = await _banco.TransferenciaInternaAsync(
                    request.NumeroCuenta, 
                    request.NumeroCuenta, 
                    -Math.Abs(request.Monto), 
                    request.Descripcion ?? "Retiro", 
                    1
                ).ConfigureAwait(false);

                if (resultado.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = resultado.Message,
                        transaccionId = resultado.TransaccionId
                    });
                }

                return BadRequest(new { error = "Retiro fallido", message = resultado.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno", message = ex.Message });
            }
        }

        // Endpoint para transferencia interna
        [HttpPost("transacciones/transferencia-interna")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> RealizarTransferenciaInterna([FromBody] TransferRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.CuentaOrigen) || string.IsNullOrWhiteSpace(request.CuentaDestino))
                    return BadRequest(new { error = "Datos inválidos", message = "Las cuentas de origen y destino son requeridas" });

                if (request.Monto <= 0)
                    return BadRequest(new { error = "Monto inválido", message = "El monto debe ser mayor a 0" });

                if (request.CuentaOrigen == request.CuentaDestino)
                    return BadRequest(new { error = "Cuentas inválidas", message = "La cuenta origen y destino no pueden ser iguales" });

                var resultado = await _banco.TransferenciaInternaAsync(
                    request.CuentaOrigen,
                    request.CuentaDestino,
                    request.Monto,
                    request.Descripcion ?? "Transferencia interna",
                    request.UsuarioId ?? 1
                ).ConfigureAwait(false);

                if (resultado.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = resultado.Message,
                        transaccionId = resultado.TransaccionId
                    });
                }

                return BadRequest(new { error = "Transferencia fallida", message = resultado.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno", message = ex.Message });
            }
        }

        // Endpoint para transferencia externa
        [HttpPost("transacciones/transferencia-externa")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> RealizarTransferenciaExterna([FromBody] TransferRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.CuentaOrigen) || string.IsNullOrWhiteSpace(request.CuentaDestino))
                    return BadRequest(new { error = "Datos inválidos", message = "Las cuentas de origen y destino son requeridas" });

                if (request.Monto <= 0)
                    return BadRequest(new { error = "Monto inválido", message = "El monto debe ser mayor a 0" });

                var resultado = await _banco.TransferenciaExternaAsync(
                    request.CuentaOrigen,
                    request.CuentaDestino,
                    request.Monto,
                    request.Descripcion ?? "Transferencia externa",
                    request.UsuarioId ?? 1
                ).ConfigureAwait(false);

                if (resultado.Success)
                {
                    return Ok(new { 
                        success = true, 
                        message = resultado.Message,
                        transaccionId = resultado.TransaccionId
                    });
                }

                return BadRequest(new { error = "Transferencia fallida", message = resultado.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno", message = ex.Message });
            }
        }

    }

    // DTOs de solicitud (duplicados aquí por ahora; mover a proyecto compartido más adelante)
    // Datos de solicitud para login
    public class LoginRequestDto 
    { 
        // Nombre de usuario
        public string Username { get; set; } 
        
        // Contraseña del usuario
        public string Password { get; set; } 
    }

    // Datos para crear un nuevo cliente
    public class ClienteCreateDto
    {
        // Nombre de usuario único
        public string Username { get; set; }
        
        // Nombre completo del cliente
        public string Nombre { get; set; }
        
        // Dirección de email del cliente
        public string Email { get; set; }
        
        // Número de teléfono del cliente
        public string Telefono { get; set; }
        
        // Contraseña inicial del usuario
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

    public class ActualizarSaldoDto 
    { 
        public string NumeroCuenta { get; set; }
        public decimal NuevoSaldo { get; set; }
        public string Motivo { get; set; }
        public int UsuarioId { get; set; }
    }

    public class EliminarClienteDto
    {
        public int UsuarioIdAdmin { get; set; }
    }

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
