using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API_BancoFisclase.Servicios
{
    public class APIServicioBanco : IServicioBanco
    {
        private readonly string _cs;

        public APIServicioBanco(IConfiguration config)
        {
            _cs = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(_cs)) throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        public async Task<LoginResultDto> AuthenticateAdminAsync(string username, string password)
        {
            const string sql = @"SELECT usuario_id, username, password_hash, salt, intentos_fallidos, bloqueado, es_admin, cliente_id FROM Usuario WHERE username = @username";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                await conn.OpenAsync().ConfigureAwait(false);
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (!await reader.ReadAsync().ConfigureAwait(false)) return new LoginResultDto { Status = EstatusLogin.NotFound, Message = "Usuario no encontrado" };
                    var id = reader.GetInt32(reader.GetOrdinal("usuario_id"));
                    var dbHash = reader.IsDBNull(reader.GetOrdinal("password_hash")) ? null : reader.GetString(reader.GetOrdinal("password_hash"));
                    var salt = reader.IsDBNull(reader.GetOrdinal("salt")) ? string.Empty : reader.GetString(reader.GetOrdinal("salt"));
                    var bloqueado = !reader.IsDBNull(reader.GetOrdinal("bloqueado")) && reader.GetBoolean(reader.GetOrdinal("bloqueado"));
                    var esAdmin = !reader.IsDBNull(reader.GetOrdinal("es_admin")) && reader.GetBoolean(reader.GetOrdinal("es_admin"));
                    int? clienteId = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("cliente_id"))) clienteId = reader.GetInt32(reader.GetOrdinal("cliente_id"));

                    if (bloqueado) return new LoginResultDto { Status = EstatusLogin.Locked, Message = "Cuenta bloqueada" };

                    var computedHex = ComputeSha256Hex(password + salt);
                    var computedBase64 = ComputeSha256Base64(password + salt);
                    bool passwordMatches = false;
                    if (!string.IsNullOrEmpty(dbHash))
                    {
                        if (dbHash.Length == 64 && Regex.IsMatch(dbHash, @"\A[0-9a-fA-F]{64}\z")) passwordMatches = string.Equals(computedHex, dbHash, StringComparison.OrdinalIgnoreCase);
                        else passwordMatches = string.Equals(computedBase64, dbHash, StringComparison.Ordinal);
                    }

                    if (!passwordMatches)
                    {
                        reader.Close();
                        const string sqlUpdateAttempts = @"UPDATE Usuario SET intentos_fallidos = intentos_fallidos + 1, bloqueado = CASE WHEN intentos_fallidos + 1 >= 3 THEN 1 ELSE 0 END WHERE usuario_id = @id";
                        using (var upd = new SqlCommand(sqlUpdateAttempts, conn)) { upd.Parameters.AddWithValue("@id", id); await upd.ExecuteNonQueryAsync().ConfigureAwait(false); }
                        return new LoginResultDto { Status = EstatusLogin.BadCredentials, Message = "Credenciales incorrectas" };
                    }

                    if (!esAdmin) return new LoginResultDto { Status = EstatusLogin.Forbidden, Message = "Usuario no es administrador" };

                    reader.Close();
                    const string sqlReset = @"UPDATE Usuario SET intentos_fallidos = 0, fecha_ultimo_login = @fecha WHERE usuario_id = @id";
                    using (var reset = new SqlCommand(sqlReset, conn)) { reset.Parameters.AddWithValue("@fecha", DateTime.Now); reset.Parameters.AddWithValue("@id", id); await reset.ExecuteNonQueryAsync().ConfigureAwait(false); }

                    return new LoginResultDto { Status = EstatusLogin.Success, Message = "Login exitoso", User = new UserDto { Id = id, Username = username, ClienteId = clienteId, IsAdmin = esAdmin } };
                }
            }
        }

        public async Task<LoginResultDto> AuthenticateUserAsync(string username, string password)
        {
            const string sql = @"SELECT usuario_id, username, password_hash, salt, intentos_fallidos, bloqueado, es_admin, cliente_id FROM Usuario WHERE username = @username";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                await conn.OpenAsync().ConfigureAwait(false);
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (!await reader.ReadAsync().ConfigureAwait(false)) return new LoginResultDto { Status = EstatusLogin.NotFound, Message = "Usuario no encontrado" };
                    var id = reader.GetInt32(reader.GetOrdinal("usuario_id"));
                    var dbHash = reader.IsDBNull(reader.GetOrdinal("password_hash")) ? null : reader.GetString(reader.GetOrdinal("password_hash"));
                    var salt = reader.IsDBNull(reader.GetOrdinal("salt")) ? string.Empty : reader.GetString(reader.GetOrdinal("salt"));
                    var bloqueado = !reader.IsDBNull(reader.GetOrdinal("bloqueado")) && reader.GetBoolean(reader.GetOrdinal("bloqueado"));
                    var esAdmin = !reader.IsDBNull(reader.GetOrdinal("es_admin")) && reader.GetBoolean(reader.GetOrdinal("es_admin"));
                    int? clienteId = null; if (!reader.IsDBNull(reader.GetOrdinal("cliente_id"))) clienteId = reader.GetInt32(reader.GetOrdinal("cliente_id"));

                    if (bloqueado) return new LoginResultDto { Status = EstatusLogin.Locked, Message = "Cuenta bloqueada" };

                    var computedHex = ComputeSha256Hex(password + salt);
                    var computedBase64 = ComputeSha256Base64(password + salt);
                    bool passwordMatches = false;
                    if (!string.IsNullOrEmpty(dbHash))
                    {
                        if (dbHash.Length == 64 && Regex.IsMatch(dbHash, @"\A[0-9a-fA-F]{64}\z")) passwordMatches = string.Equals(computedHex, dbHash, StringComparison.OrdinalIgnoreCase);
                        else passwordMatches = string.Equals(computedBase64, dbHash, StringComparison.Ordinal);
                    }

                    if (!passwordMatches)
                    {
                        reader.Close();
                        const string sqlUpdateAttempts = @"UPDATE Usuario SET intentos_fallidos = intentos_fallidos + 1, bloqueado = CASE WHEN intentos_fallidos + 1 >= 3 THEN 1 ELSE 0 END WHERE usuario_id = @id";
                        using (var upd = new SqlCommand(sqlUpdateAttempts, conn)) { upd.Parameters.AddWithValue("@id", id); await upd.ExecuteNonQueryAsync().ConfigureAwait(false); }
                        return new LoginResultDto { Status = EstatusLogin.BadCredentials, Message = "Credenciales incorrectas" };
                    }

                    reader.Close();
                    const string sqlReset = @"UPDATE Usuario SET intentos_fallidos = 0, fecha_ultimo_login = @fecha WHERE usuario_id = @id";
                    using (var reset = new SqlCommand(sqlReset, conn)) { reset.Parameters.AddWithValue("@fecha", DateTime.Now); reset.Parameters.AddWithValue("@id", id); await reset.ExecuteNonQueryAsync().ConfigureAwait(false); }

                    return new LoginResultDto { Status = EstatusLogin.Success, Message = "Login exitoso", User = new UserDto { Id = id, Username = username, ClienteId = clienteId, IsAdmin = esAdmin } };
                }
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerEstadisticasGeneralesAsync()
        {
            const string sql = @"SELECT (SELECT COUNT(*) FROM Cliente WHERE activo = 1) AS 'Total Clientes', (SELECT COUNT(*) FROM Cuenta WHERE activa = 1) AS 'Total Cuentas', (SELECT COUNT(*) FROM Usuario WHERE bloqueado = 0) AS 'Usuarios Activos', (SELECT COUNT(*) FROM Usuario WHERE bloqueado = 1) AS 'Usuarios Bloqueados', (SELECT ISNULL(SUM(saldo), 0) FROM Cuenta WHERE activa = 1) AS 'Saldo Total Sistema', (SELECT COUNT(*) FROM Transaccion WHERE CAST(fecha AS DATE) = CAST(GETDATE() AS DATE)) AS 'Transacciones Hoy', (SELECT ISNULL(SUM(monto), 0) FROM Transaccion WHERE CAST(fecha AS DATE) = CAST(GETDATE() AS DATE)) AS 'Monto Total Hoy'";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerMovimientosDelDiaAsync()
        {
            // SELECT * desde la vista y dejar que devuelva los nombres originales
            // La vista debería tener: cliente_id, cliente_nombre, cliente_email, transaccion_id, fecha,
            // tipo_transaccion, monto, descripcion, cuenta_origen_id, cuenta_destino_id, monto_debito, monto_credito,
            // estado_transaccion, usuario_transaccion
            const string sql = "SELECT * FROM vw_MovimientosDelDia";
            var results = await ExecuteQueryAsync(sql).ConfigureAwait(false);
            
            // Mapear a aliases descriptivos en el código
            return results.Select(row => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["ID Cliente"] = row.ContainsKey("cliente_id") ? row["cliente_id"] : null,
                ["Nombre"] = row.ContainsKey("cliente_nombre") ? row["cliente_nombre"] : null,
                ["Email"] = row.ContainsKey("cliente_email") ? row["cliente_email"] : null,
                ["ID Transacción"] = row.ContainsKey("transaccion_id") ? row["transaccion_id"] : null,
                ["Fecha"] = row.ContainsKey("fecha") ? row["fecha"] : null,
                ["Tipo de Transacción"] = row.ContainsKey("tipo_transaccion") ? MapTipoTransaccion(row["tipo_transaccion"]?.ToString()) : null,
                ["Monto"] = row.ContainsKey("monto") ? row["monto"] : null,
                ["Descripción"] = row.ContainsKey("descripcion") ? row["descripcion"] : null,
                ["Cuenta de Origen"] = row.ContainsKey("cuenta_origen_id") ? row["cuenta_origen_id"] : null,
                ["Cuenta de Destino"] = row.ContainsKey("cuenta_destino_id") ? row["cuenta_destino_id"] : null,
                ["Monto Débito"] = row.ContainsKey("monto_debito") ? row["monto_debito"] : null,
                ["Monto Crédito"] = row.ContainsKey("monto_credito") ? row["monto_credito"] : null,
                ["Estado de Transacción"] = row.ContainsKey("estado_transaccion") ? MapEstadoTransaccion(row["estado_transaccion"]?.ToString()) : null,
                ["Usuario"] = row.ContainsKey("usuario_transaccion") ? row["usuario_transaccion"] : null
            });
        }
        
        private static string MapTipoTransaccion(string tipo)
        {
            return tipo switch
            {
                "TRANSFER_INTERNO" => "Transferencia entre cuentas del mismo cliente",
                "TRANSFER_EXTERNO" => "Transferencia a cuentas de otros clientes",
                "DEPOSITO" => "Depósito de efectivo",
                "RETIRO" => "Retiro de efectivo",
                "TRANSFERENCIA" => "Transferencia",
                "PAGO" => "Pago",
                _ => tipo
            };
        }
        
        private static string MapEstadoTransaccion(string estado)
        {
            return estado switch
            {
                "COMPLETADO" => "Transacción completada exitosamente",
                "RECHAZADO" => "Transacción rechazada",
                "PENDIENTE" => "Transacción pendiente de procesamiento",
                _ => estado
            };
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerClientesAsync()
        {
            const string sql = @"
                SELECT 
                    u.usuario_id AS 'ID Usuario', 
                    ISNULL(u.cliente_id, 0) AS 'ID Cliente', 
                    u.username AS 'Usuario', 
                    c.nombre AS 'Nombre Completo', 
                    c.email AS 'Email', 
                    c.telefono AS 'Teléfono', 
                    u.es_admin AS 'Es Admin', 
                    u.bloqueado AS 'Bloqueado',
                    c.calle AS 'Calle',
                    c.numero AS 'Número',
                    c.ciudad AS 'Ciudad',
                    c.estado_provincia AS 'Estado/Provincia',
                    c.codigo_postal AS 'Código Postal',
                    c.pais AS 'País'
                FROM Usuario u 
                LEFT JOIN Cliente c ON u.cliente_id = c.cliente_id 
                ORDER BY u.usuario_id";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerTransaccionesRecientesAsync(int limit = 20)
        {
            const string sqlTemplate = @"
                SELECT TOP(@limit) 
                    t.transaccion_id AS 'ID Transacción',
                    t.fecha AS 'Fecha',
                    CASE tt.nombre
                        WHEN 'TRANSFER_INTERNO' THEN 'Transferencia entre cuentas del mismo cliente'
                        WHEN 'TRANSFER_EXTERNO' THEN 'Transferencia a cuentas de otros clientes'
                        WHEN 'DEPOSITO' THEN 'Depósito de efectivo'
                        WHEN 'RETIRO' THEN 'Retiro de efectivo'
                        WHEN 'TRANSFERENCIA' THEN 'Transferencia'
                        WHEN 'PAGO' THEN 'Pago'
                        ELSE tt.nombre
                    END AS 'Tipo Transacción',
                    CASE et.nombre
                        WHEN 'COMPLETADO' THEN 'Transacción completada exitosamente'
                        WHEN 'RECHAZADO' THEN 'Transacción rechazada'
                        WHEN 'PENDIENTE' THEN 'Transacción pendiente de procesamiento'
                        ELSE et.nombre
                    END AS 'Estado',
                    t.monto AS 'Monto',
                    t.descripcion AS 'Descripción'
                FROM Transaccion t 
                LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id 
                LEFT JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id 
                ORDER BY t.fecha DESC";
            return await ExecuteQueryAsync(sqlTemplate, ("@limit", limit)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerAnalisisTransaccionesAsync()
        {
            const string sql = "SELECT * FROM vw_AnalisisTransacciones ORDER BY cantidad_transacciones DESC";
            var results = await ExecuteQueryAsync(sql).ConfigureAwait(false);
            
            return results.Select(row => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["ID Cliente"] = row.ContainsKey("cliente_id") ? row["cliente_id"] : null,
                ["Nombre del Cliente"] = row.ContainsKey("cliente_nombre") ? row["cliente_nombre"] : null,
                ["Tipo de Transacción"] = row.ContainsKey("tipo_transaccion") ? MapTipoTransaccion(row["tipo_transaccion"]?.ToString()) : null,
                ["Cantidad de Transacciones"] = row.ContainsKey("cantidad_transacciones") ? row["cantidad_transacciones"] : null,
                ["Monto Total"] = row.ContainsKey("monto_total") ? row["monto_total"] : null,
                ["Monto Promedio"] = row.ContainsKey("monto_promedio") ? row["monto_promedio"] : null,
                ["Primera Transacción"] = row.ContainsKey("primera_transaccion") ? row["primera_transaccion"] : null,
                ["Última Transacción"] = row.ContainsKey("ultima_transaccion") ? row["ultima_transaccion"] : null,
                ["Estado Más Común"] = row.ContainsKey("estado_mas_comun") ? MapEstadoTransaccion(row["estado_mas_comun"]?.ToString()) : null
            });
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerResumenClientesAsync()
        {
            const string sql = "SELECT * FROM vw_ResumenCliente ORDER BY cliente_id";
            var results = await ExecuteQueryAsync(sql).ConfigureAwait(false);
            
            return results.Select(row => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["ID Cliente"] = row.ContainsKey("cliente_id") ? row["cliente_id"] : null,
                ["Nombre del Cliente"] = row.ContainsKey("nombre") ? row["nombre"] : null,
                ["Email"] = row.ContainsKey("email") ? row["email"] : null,
                ["Teléfono"] = row.ContainsKey("telefono") ? row["telefono"] : null,
                ["Total de Cuentas"] = row.ContainsKey("total_cuentas") ? row["total_cuentas"] : null,
                ["Cuentas Activas"] = row.ContainsKey("cuentas_activas") ? row["cuentas_activas"] : null,
                ["Cuentas Inactivas"] = row.ContainsKey("cuentas_inactivas") ? row["cuentas_inactivas"] : null,
                ["Saldo Total"] = row.ContainsKey("saldo_total") ? row["saldo_total"] : null,
                ["Última Transacción"] = row.ContainsKey("ultima_transaccion") ? row["ultima_transaccion"] : null,
                ["Usuarios Asociados"] = row.ContainsKey("usuarios_asociados") ? row["usuarios_asociados"] : null
            });
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerUsuariosBloqueadosAsync()
        {
            const string sql = @"SELECT u.usuario_id AS 'ID Usuario', u.username AS 'Usuario', u.intentos_fallidos AS 'Intentos Fallidos', u.bloqueado AS 'Bloqueado', c.nombre AS 'Nombre del Cliente' FROM Usuario u LEFT JOIN Cliente c ON u.cliente_id = c.cliente_id WHERE u.bloqueado = 1";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerNotificacionesSistemaAsync()
        {
            const string sql = @"
                SELECT 
                    notificacion_id AS 'ID Notificación',
                    cliente_id AS 'ID Cliente',
                    tipo AS 'Tipo',
                    asunto AS 'Asunto',
                    fecha_envio AS 'Fecha Envío',
                    estado AS 'Estado'
                FROM Notificacion 
                ORDER BY fecha_envio DESC";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerReporteTransaccionesPorFechaAsync(DateTime desde, DateTime hasta)
        {
            const string sql = @"
                SELECT 
                    t.transaccion_id AS 'ID Transacción',
                    CASE tt.nombre
                        WHEN 'TRANSFER_INTERNO' THEN 'Transferencia entre cuentas del mismo cliente'
                        WHEN 'TRANSFER_EXTERNO' THEN 'Transferencia a cuentas de otros clientes'
                        WHEN 'DEPOSITO' THEN 'Depósito de efectivo'
                        WHEN 'RETIRO' THEN 'Retiro de efectivo'
                        WHEN 'TRANSFERENCIA' THEN 'Transferencia'
                        WHEN 'PAGO' THEN 'Pago'
                        ELSE tt.nombre
                    END AS 'Tipo Transacción',
                    t.monto AS 'Monto',
                    CASE et.nombre
                        WHEN 'COMPLETADO' THEN 'Transacción completada exitosamente'
                        WHEN 'RECHAZADO' THEN 'Transacción rechazada'
                        WHEN 'PENDIENTE' THEN 'Transacción pendiente de procesamiento'
                        ELSE et.nombre
                    END AS 'Estado',
                    t.descripcion AS 'Descripción',
                    t.fecha AS 'Fecha',
                    t.usuario_id AS 'ID Usuario'
                FROM Transaccion t 
                LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id 
                LEFT JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id 
                WHERE t.fecha BETWEEN @desde AND @hasta 
                ORDER BY t.fecha DESC";
            return await ExecuteQueryAsync(sql, ("@desde", desde), ("@hasta", hasta)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerCuentasPorClienteAsync(int clienteId)
        {
            const string sql = @"
                SELECT 
                    c.cuenta_id AS 'ID Cuenta',
                    c.numero_cuenta AS 'Número de Cuenta',
                    CASE tc.nombre
                        WHEN 'AHORRO' THEN 'Cuenta de ahorros personal'
                        WHEN 'CORRIENTE' THEN 'Cuenta corriente personal'
                        WHEN 'CREDITO' THEN 'Tarjeta de crédito'
                        WHEN 'DEBITO' THEN 'Tarjeta de débito'
                        ELSE tc.nombre
                    END AS 'Tipo de Cuenta',
                    c.saldo AS 'Saldo',
                    c.activa AS 'Activa'
                FROM Cuenta c 
                LEFT JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id 
                WHERE c.cliente_id = @clienteId AND c.activa = 1 
                ORDER BY c.cuenta_id";
            return await ExecuteQueryAsync(sql, ("@clienteId", clienteId)).ConfigureAwait(false);
        }

        public async Task<IDictionary<string, object>> ObtenerClientePorIdAsync(int clienteId)
        {
            const string sql = @"
                SELECT 
                    c.cliente_id AS 'ID Cliente',
                    c.nombre AS 'Nombre',
                    c.email AS 'Email',
                    c.telefono AS 'Teléfono',
                    c.calle AS 'Calle',
                    c.numero AS 'Número',
                    c.ciudad AS 'Ciudad',
                    c.estado_provincia AS 'Estado/Provincia',
                    c.codigo_postal AS 'Código Postal',
                    c.pais AS 'País',
                    c.fecha_registro AS 'Fecha de Registro',
                    u.username AS 'Nombre de Usuario'
                FROM Cliente c
                LEFT JOIN Usuario u ON c.cliente_id = u.cliente_id
                WHERE c.cliente_id = @clienteId";
            
            var result = await ExecuteQueryAsync(sql, ("@clienteId", clienteId)).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerNotificacionesClienteAsync(int clienteId, int dias = 30)
        {
            const string sql = @"
                SELECT 
                    notificacion_id AS 'ID Notificación',
                    tipo AS 'Tipo',
                    asunto AS 'Asunto',
                    contenido AS 'Contenido',
                    fecha_envio AS 'Fecha de Envío',
                    estado AS 'Estado'
                FROM Notificacion
                WHERE cliente_id = @clienteId
                AND fecha_envio >= DATEADD(DAY, -@dias, GETDATE())
                ORDER BY fecha_envio DESC";
            
            return await ExecuteQueryAsync(sql, ("@clienteId", clienteId), ("@dias", dias)).ConfigureAwait(false);
        }

        // NEW: Obtener transacciones específicas de un cliente
        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerTransaccionesClienteAsync(int clienteId, int limit = 20)
        {
            const string sql = @"
                SELECT TOP(@limit) 
                    t.transaccion_id AS 'ID Transacción', 
                    t.fecha AS 'Fecha', 
                    CASE tt.nombre
                        WHEN 'TRANSFER_INTERNO' THEN 'Transferencia entre cuentas del mismo cliente'
                        WHEN 'TRANSFER_EXTERNO' THEN 'Transferencia a cuentas de otros clientes'
                        WHEN 'DEPOSITO' THEN 'Depósito de efectivo'
                        WHEN 'RETIRO' THEN 'Retiro de efectivo'
                        WHEN 'TRANSFERENCIA' THEN 'Transferencia'
                        WHEN 'PAGO' THEN 'Pago'
                        ELSE tt.nombre
                    END AS 'Tipo Transacción', 
                    CASE et.nombre
                        WHEN 'COMPLETADO' THEN 'Transacción completada exitosamente'
                        WHEN 'RECHAZADO' THEN 'Transacción rechazada'
                        WHEN 'PENDIENTE' THEN 'Transacción pendiente de procesamiento'
                        ELSE et.nombre
                    END AS 'Estado', 
                    t.monto AS 'Monto', 
                    t.descripcion AS 'Descripción',
                    co.numero_cuenta AS 'Cuenta Origen',
                    cd.numero_cuenta AS 'Cuenta Destino',
                    clienteDestino.nombre_completo AS 'Cliente Destino'
                FROM Transaccion t 
                LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id
                LEFT JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id 
                LEFT JOIN Cuenta co ON t.cuenta_origen_id = co.cuenta_id
                LEFT JOIN Cuenta cd ON t.cuenta_destino_id = cd.cuenta_id
                LEFT JOIN Cliente clienteDestino ON cd.cliente_id = clienteDestino.cliente_id
                WHERE (co.cliente_id = @clienteId OR cd.cliente_id = @clienteId)
                ORDER BY t.fecha DESC";
            
            Console.WriteLine($"DEBUG: ObtenerTransaccionesClienteAsync - clienteId: {clienteId}, limit: {limit}");
            
            var result = await ExecuteQueryAsync(sql, ("@limit", limit), ("@clienteId", clienteId)).ConfigureAwait(false);
            var resultList = result.ToList();
            
            Console.WriteLine($"DEBUG: ObtenerTransaccionesClienteAsync - Found {resultList.Count} transactions for clienteId {clienteId}");
            
            return resultList;
        }

        public async Task<bool> EjecutarResumenesDiariosAsync()
        {
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("sp_EnviarResumenesDiarios", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync().ConfigureAwait(false);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                return true;
            }
        }

        private async Task<int?> GetCuentaIdByNumeroAsync(string numeroCuenta)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta)) return null;
            const string sql = "SELECT TOP(1) cuenta_id FROM Cuenta WHERE numero_cuenta = @numero";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@numero", numeroCuenta);
                await conn.OpenAsync().ConfigureAwait(false);
                var val = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                if (val == null || val == DBNull.Value) return null;
                return Convert.ToInt32(val);
            }
        }

        public async Task<(bool Success, string Message, int TransaccionId)> TransferenciaInternaAsync(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto, string descripcion, int usuarioId)
        {
            int origenId;
            int destinoId;
            if (!int.TryParse(numeroCuentaOrigen, out origenId))
            {
                var maybe = await GetCuentaIdByNumeroAsync(numeroCuentaOrigen).ConfigureAwait(false);
                if (!maybe.HasValue) return (false, "Cuenta de origen no encontrada", 0);
                origenId = maybe.Value;
            }
            if (!int.TryParse(numeroCuentaDestino, out destinoId))
            {
                var maybe = await GetCuentaIdByNumeroAsync(numeroCuentaDestino).ConfigureAwait(false);
                if (!maybe.HasValue) return (false, "Cuenta de destino no encontrada", 0);
                destinoId = maybe.Value;
            }

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("sp_TransferenciaInterna", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cuenta_origen_id", origenId);
                cmd.Parameters.AddWithValue("@cuenta_destino_id", destinoId);
                cmd.Parameters.AddWithValue("@monto", monto);
                cmd.Parameters.AddWithValue("@descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@usuario_id", usuarioId);

                var outTrans = new SqlParameter("@transaccion_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outMsg = new SqlParameter("@message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outTrans);
                cmd.Parameters.Add(outMsg);

                await conn.OpenAsync().ConfigureAwait(false);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                var tId = outTrans.Value == DBNull.Value ? 0 : Convert.ToInt32(outTrans.Value);
                var msg = outMsg.Value == DBNull.Value ? string.Empty : (string)outMsg.Value;
                var success = tId > 0;

                try { await LogAuditAsync("TransferenciaInterna", usuarioId, $"origen={origenId},destino={destinoId},monto={monto},transaccionId={tId},mensaje={msg}").ConfigureAwait(false); } catch { }
                return (success, msg, tId);
            }
        }

        public async Task<(bool Success, string Message, int TransaccionId)> TransferenciaExternaAsync(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto, string descripcion, int usuarioId)
        {
            int origenId;
            if (!int.TryParse(numeroCuentaOrigen, out origenId))
            {
                var maybe = await GetCuentaIdByNumeroAsync(numeroCuentaOrigen).ConfigureAwait(false);
                if (!maybe.HasValue) return (false, "Cuenta de origen no encontrada", 0);
                origenId = maybe.Value;
            }

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("sp_TransferenciaExterna", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cuenta_origen_id", origenId);
                cmd.Parameters.AddWithValue("@numero_cuenta_destino", numeroCuentaDestino ?? string.Empty);
                cmd.Parameters.AddWithValue("@monto", monto);
                cmd.Parameters.AddWithValue("@descripcion", (object)descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@usuario_id", usuarioId);

                var outTrans = new SqlParameter("@transaccion_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var outMsg = new SqlParameter("@message", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outTrans);
                cmd.Parameters.Add(outMsg);

                await conn.OpenAsync().ConfigureAwait(false);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                var tId = outTrans.Value == DBNull.Value ? 0 : Convert.ToInt32(outTrans.Value);
                var msg = outMsg.Value == DBNull.Value ? string.Empty : (string)outMsg.Value;
                var success = tId > 0;

                try { await LogAuditAsync("TransferenciaExterna", usuarioId, $"origen={origenId},numero_destino={numeroCuentaDestino},monto={monto},transaccionId={tId},mensaje={msg}").ConfigureAwait(false); } catch { }
                return (success, msg, tId);
            }
        }

        public async Task<bool> CrearClienteCompletoAsync(object clienteDto, string password)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));
            using (var conn = new SqlConnection(_cs))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var tx = conn.BeginTransaction())
                {
                    cmd.Transaction = tx;
                    try
                    {
                        var doc = JsonDocument.Parse(JsonSerializer.Serialize(clienteDto));
                        var root = doc.RootElement;
                        var nombre = GetString(root, "Nombre") ?? GetString(root, "nombre") ?? throw new ArgumentException("Nombre is required");
                        var email = GetString(root, "Email") ?? GetString(root, "email") ?? throw new ArgumentException("Email is required");
                        var telefono = GetString(root, "Telefono") ?? GetString(root, "telefono");
                        var username = GetString(root, "Username") ?? GetString(root, "username") ?? throw new ArgumentException("Username is required");
                        var pwd = string.IsNullOrEmpty(password) ? (GetString(root, "Password") ?? GetString(root, "password")) : password;
                        if (string.IsNullOrEmpty(pwd)) throw new ArgumentException("Password is required");

                        cmd.CommandText = @"INSERT INTO Cliente (nombre, email, telefono, activo) VALUES (@nombre, @email, @telefono, 1); SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@telefono", (object)telefono ?? DBNull.Value);
                        var clienteIdObj = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                        var clienteId = Convert.ToInt32(clienteIdObj);

                        var salt = GenerateSalt();
                        var hash = ComputeSha256Base64(pwd + salt);

                        cmd.CommandText = @"INSERT INTO Usuario (cliente_id, username, password_hash, salt, es_admin) VALUES (@clienteId, @username, @passwordHash, @salt, 0)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@clienteId", clienteId);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@passwordHash", hash);
                        cmd.Parameters.AddWithValue("@salt", salt);
                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        tx.Commit();
                        try { await LogAuditAsync("CrearCliente", null, $"clienteId={clienteId},username={username},email={email}").ConfigureAwait(false); } catch { }
                        return true;
                    }
                    catch
                    {
                        try { tx.Rollback(); } catch { }
                        throw;
                    }
                }
            }
        }

        public async Task<bool> ActualizarClienteAsync(int id, object clienteDto)
        {
            if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));
            var json = JsonSerializer.Serialize(clienteDto);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(@"UPDATE Cliente SET nombre = @nombre, email = @email, telefono = @telefono, calle = @calle, numero = @numero, ciudad = @ciudad, estado_provincia = @estado_provincia, codigo_postal = @codigo_postal, pais = @pais WHERE cliente_id = (SELECT cliente_id FROM Usuario WHERE usuario_id = @usuarioId)", conn))
            {
                cmd.Parameters.AddWithValue("@usuarioId", id);
                cmd.Parameters.AddWithValue("@nombre", (object)GetString(root, "Nombre") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object)GetString(root, "Email") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@telefono", (object)GetString(root, "Telefono") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@calle", (object)GetString(root, "Calle") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@numero", (object)GetString(root, "Numero") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ciudad", (object)GetString(root, "Ciudad") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estado_provincia", (object)GetString(root, "EstadoProvincia") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@codigo_postal", (object)GetString(root, "CodigoPostal") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@pais", (object)GetString(root, "Pais") ?? DBNull.Value);
                await conn.OpenAsync().ConfigureAwait(false);
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                return rows > 0;
            }
        }

        public async Task<bool> CambiarContrasenaAsync(int usuarioId, string nuevaContrasena)
        {
            if (string.IsNullOrEmpty(nuevaContrasena)) throw new ArgumentNullException(nameof(nuevaContrasena));
            var salt = GenerateSalt();
            var hash = ComputeSha256Base64(nuevaContrasena + salt);
            const string sql = @"UPDATE Usuario SET password_hash = @hash, salt = @salt WHERE usuario_id = @id";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Parameters.AddWithValue("@id", usuarioId);
                await conn.OpenAsync().ConfigureAwait(false);
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                var ok = rows > 0;
                if (ok) { try { await LogAuditAsync("CambiarContrasena", usuarioId, $"usuarioId={usuarioId}").ConfigureAwait(false); } catch { } }
                return ok;
            }
        }

        public async Task<bool> CrearCuentaBancariaAsync(object cuentaDto)
        {
            if (cuentaDto == null) throw new ArgumentNullException(nameof(cuentaDto));
            var doc = JsonDocument.Parse(JsonSerializer.Serialize(cuentaDto));
            var root = doc.RootElement;
            if (!TryGetInt(root, "ClienteId", out var clienteId) && !TryGetInt(root, "clienteId", out clienteId)) throw new ArgumentException("ClienteId is required");
            var tipo = GetString(root, "Tipo") ?? GetString(root, "tipo") ?? throw new ArgumentException("Tipo is required");
            var numero = GetString(root, "NumeroCuenta") ?? GetString(root, "numeroCuenta") ?? GetString(root, "Numero") ?? GetString(root, "numero") ?? throw new ArgumentException("NumeroCuenta is required");
            var saldoInicial = TryGetDouble(root, "SaldoInicial", out var temp) || TryGetDouble(root, "saldoInicial", out temp) ? temp : 0.0;
            const string sql = @"INSERT INTO Cuenta (cliente_id, tipo_cuenta_id, numero_cuenta, saldo, activa) VALUES (@clienteId, (SELECT tipo_cuenta_id FROM TipoCuenta WHERE nombre = @tipo), @numero, @saldo, 1)";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@clienteId", clienteId);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@numero", numero);
                cmd.Parameters.AddWithValue("@saldo", Convert.ToDecimal(saldoInicial));
                await conn.OpenAsync().ConfigureAwait(false);
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                var ok = rows > 0;
                if (ok) { try { await LogAuditAsync("CrearCuenta", null, $"clienteId={clienteId},tipo={tipo},numero={numero},saldoInicial={saldoInicial}").ConfigureAwait(false); } catch { } }
                return ok;
            }
        }

        public async Task<(bool Success, string Message)> ActualizarSaldoCuentaAsync(string numeroCuenta, decimal nuevoSaldo, string motivo, int usuarioId)
        {
            if (string.IsNullOrEmpty(numeroCuenta)) throw new ArgumentNullException(nameof(numeroCuenta));
            if (nuevoSaldo < 0) return (false, "El saldo no puede ser negativo");
            if (string.IsNullOrEmpty(motivo)) throw new ArgumentNullException(nameof(motivo));

            using (var conn = new SqlConnection(_cs))
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Verificar que la cuenta existe
                        const string sqlVerificar = @"SELECT cuenta_id, saldo FROM Cuenta WHERE numero_cuenta = @numeroCuenta";
                        using (var cmdVerificar = new SqlCommand(sqlVerificar, conn, tx))
                        {
                            cmdVerificar.Parameters.AddWithValue("@numeroCuenta", numeroCuenta);
                            using (var reader = await cmdVerificar.ExecuteReaderAsync().ConfigureAwait(false))
                            {
                                if (!await reader.ReadAsync().ConfigureAwait(false))
                                {
                                    return (false, $"La cuenta {numeroCuenta} no existe");
                                }
                                var cuentaId = reader.GetInt32(0);
                                var saldoAnterior = reader.GetDecimal(1);
                                await reader.CloseAsync().ConfigureAwait(false);

                                // Actualizar el saldo
                                const string sqlActualizar = @"UPDATE Cuenta SET saldo = @nuevoSaldo WHERE cuenta_id = @cuentaId";
                                using (var cmdActualizar = new SqlCommand(sqlActualizar, conn, tx))
                                {
                                    cmdActualizar.Parameters.AddWithValue("@nuevoSaldo", nuevoSaldo);
                                    cmdActualizar.Parameters.AddWithValue("@cuentaId", cuentaId);
                                    await cmdActualizar.ExecuteNonQueryAsync().ConfigureAwait(false);
                                }

                                // Registrar transacción de ajuste
                                decimal diferencia = nuevoSaldo - saldoAnterior;
                                string tipoTransaccion = diferencia > 0 ? "Ajuste Positivo" : "Ajuste Negativo";
                                string descripcion = $"{motivo} (Saldo anterior: {saldoAnterior:C}, Nuevo saldo: {nuevoSaldo:C})";

                                const string sqlTransaccion = @"INSERT INTO Transaccion (cuenta_origen_id, monto, descripcion, tipo_transaccion_id, estado_transaccion_id, fecha, usuario_id) 
                                    VALUES (@cuentaId, @monto, @descripcion, (SELECT tipo_transaccion_id FROM TipoTransaccion WHERE nombre = 'Ajuste'), 
                                    (SELECT estado_transaccion_id FROM EstadoTransaccion WHERE nombre = 'Completado'), GETDATE(), @usuarioId)";
                                using (var cmdTransaccion = new SqlCommand(sqlTransaccion, conn, tx))
                                {
                                    cmdTransaccion.Parameters.AddWithValue("@cuentaId", cuentaId);
                                    cmdTransaccion.Parameters.AddWithValue("@monto", Math.Abs(diferencia));
                                    cmdTransaccion.Parameters.AddWithValue("@descripcion", descripcion);
                                    cmdTransaccion.Parameters.AddWithValue("@usuarioId", usuarioId);
                                    await cmdTransaccion.ExecuteNonQueryAsync().ConfigureAwait(false);
                                }
                            }
                        }

                        tx.Commit();
                        await LogAuditAsync("ActualizarSaldo", usuarioId, $"cuenta={numeroCuenta},nuevoSaldo={nuevoSaldo},motivo={motivo}").ConfigureAwait(false);
                        return (true, "Saldo actualizado correctamente");
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        return (false, $"Error al actualizar saldo: {ex.Message}");
                    }
                }
            }
        }

        public async Task<(bool Success, string Message)> EliminarClienteAsync(int clienteId, int usuarioIdAdmin)
        {
            using (var conn = new SqlConnection(_cs))
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Verificar que el cliente existe
                        const string sqlVerificar = @"SELECT COUNT(*) FROM Cliente WHERE cliente_id = @clienteId";
                        using (var cmdVerificar = new SqlCommand(sqlVerificar, conn, tx))
                        {
                            cmdVerificar.Parameters.AddWithValue("@clienteId", clienteId);
                            var existe = (int)await cmdVerificar.ExecuteScalarAsync().ConfigureAwait(false) > 0;
                            if (!existe)
                            {
                                return (false, $"El cliente con ID {clienteId} no existe");
                            }
                        }

                        // Verificar si el cliente tiene cuentas activas con saldo
                        const string sqlCuentasActivas = @"SELECT COUNT(*) FROM Cuenta WHERE cliente_id = @clienteId AND activa = 1 AND saldo > 0";
                        using (var cmdCuentas = new SqlCommand(sqlCuentasActivas, conn, tx))
                        {
                            cmdCuentas.Parameters.AddWithValue("@clienteId", clienteId);
                            var cuentasConSaldo = (int)await cmdCuentas.ExecuteScalarAsync().ConfigureAwait(false);
                            if (cuentasConSaldo > 0)
                            {
                                return (false, $"No se puede eliminar el cliente porque tiene {cuentasConSaldo} cuenta(s) activa(s) con saldo. Debe transferir o retirar los fondos primero.");
                            }
                        }

                        // Desactivar todas las cuentas del cliente
                        const string sqlDesactivarCuentas = @"UPDATE Cuenta SET activa = 0 WHERE cliente_id = @clienteId";
                        using (var cmdDesactivarCuentas = new SqlCommand(sqlDesactivarCuentas, conn, tx))
                        {
                            cmdDesactivarCuentas.Parameters.AddWithValue("@clienteId", clienteId);
                            await cmdDesactivarCuentas.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // Desactivar el cliente (soft delete)
                        const string sqlDesactivarCliente = @"UPDATE Cliente SET activo = 0 WHERE cliente_id = @clienteId";
                        using (var cmdDesactivarCliente = new SqlCommand(sqlDesactivarCliente, conn, tx))
                        {
                            cmdDesactivarCliente.Parameters.AddWithValue("@clienteId", clienteId);
                            await cmdDesactivarCliente.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // Bloquear todos los usuarios asociados al cliente
                        const string sqlBloquearUsuarios = @"UPDATE Usuario SET bloqueado = 1 WHERE cliente_id = @clienteId";
                        using (var cmdBloquearUsuarios = new SqlCommand(sqlBloquearUsuarios, conn, tx))
                        {
                            cmdBloquearUsuarios.Parameters.AddWithValue("@clienteId", clienteId);
                            await cmdBloquearUsuarios.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        tx.Commit();
                        await LogAuditAsync("EliminarCliente", usuarioIdAdmin, $"clienteId={clienteId}").ConfigureAwait(false);
                        return (true, "Cliente eliminado correctamente (desactivado)");
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        return (false, $"Error al eliminar cliente: {ex.Message}");
                    }
                }
            }
        }

        public async Task<bool> BloquearDesbloquearUsuarioAsync(int usuarioId, bool blocking)
        {
            const string sql = @"UPDATE Usuario SET bloqueado = @bloquear WHERE usuario_id = @id";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@bloquear", blocking ? 1 : 0);
                cmd.Parameters.AddWithValue("@id", usuarioId);
                await conn.OpenAsync().ConfigureAwait(false);
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                var ok = rows > 0;
                if (ok) { try { await LogAuditAsync("BloquearDesbloquearUsuario", usuarioId, $"usuarioId={usuarioId},bloquear={blocking}").ConfigureAwait(false); } catch { } }
                return ok;
            }
        }

        public async Task<bool> CrearUsuarioAdministradorAsync(object adminDto)
        {
            if (adminDto == null) throw new ArgumentNullException(nameof(adminDto));
            var doc = JsonDocument.Parse(JsonSerializer.Serialize(adminDto));
            var root = doc.RootElement;
            var username = GetString(root, "Username") ?? GetString(root, "username") ?? throw new ArgumentException("Username required");
            var password = GetString(root, "Password") ?? GetString(root, "password") ?? throw new ArgumentException("Password required");
            var nombre = GetString(root, "Nombre") ?? GetString(root, "nombre") ?? throw new ArgumentException("Nombre required");
            var email = GetString(root, "Email") ?? GetString(root, "email") ?? throw new ArgumentException("Email required");
            using (var conn = new SqlConnection(_cs))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var tx = conn.BeginTransaction())
                {
                    cmd.Transaction = tx;
                    try
                    {
                        cmd.CommandText = @"INSERT INTO Cliente (nombre, email, activo) VALUES (@nombre, @email, 1); SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@email", email);
                        var clienteIdObj = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                        var clienteId = Convert.ToInt32(clienteIdObj);
                        var salt = GenerateSalt();
                        var hash = ComputeSha256Base64(password + salt);
                        cmd.CommandText = @"INSERT INTO Usuario (cliente_id, username, password_hash, salt, es_admin) VALUES (@clienteId, @username, @passwordHash, @salt, 1)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@clienteId", clienteId);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@passwordHash", hash);
                        cmd.Parameters.AddWithValue("@salt", salt);
                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                        tx.Commit();
                        try { await LogAuditAsync("CrearAdmin", null, $"clienteId={clienteId},username={username},email={email}").ConfigureAwait(false); } catch { }
                        return true;
                    }
                    catch
                    {
                        try { tx.Rollback(); } catch { }
                        throw;
                    }
                }
            }
        }

        public async Task<(IEnumerable<IDictionary<string, object>> Items, int Total)> ObtenerClientesPaginadoAsync(int page, int pageSize, string search)
        {
            var offset = (page - 1) * pageSize;
            var where = string.IsNullOrWhiteSpace(search) ? string.Empty : "WHERE (c.nombre LIKE @search OR c.email LIKE @search OR u.username LIKE @search)";
            
            // Cambiar nombres de columnas para coincidir con el código C#
            var sql = $@"
                SELECT 
                    ISNULL(c.cliente_id, 0) AS 'ID Cliente', 
                    c.nombre AS 'Nombre', 
                    c.email AS 'Email', 
                    c.telefono AS 'Telefono', 
                    ISNULL(u.usuario_id, 0) AS 'ID Usuario', 
                    ISNULL(u.username, '') AS 'Usuario', 
                    ISNULL(u.es_admin, 0) AS 'Es Admin', 
                    ISNULL(u.bloqueado, 0) AS 'Bloqueado',
                    c.calle AS 'Calle',
                    c.numero AS 'Numero',
                    c.ciudad AS 'Ciudad',
                    c.estado_provincia AS 'Estado/Provincia',
                    c.codigo_postal AS 'Código Postal',
                    c.pais AS 'País'
                FROM Cliente c 
                LEFT JOIN Usuario u ON u.cliente_id = c.cliente_id 
                {where} 
                ORDER BY c.cliente_id 
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (!string.IsNullOrWhiteSpace(search)) cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                await conn.OpenAsync().ConfigureAwait(false);
                var items = new List<IDictionary<string, object>>();
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        for (var i = 0; i < reader.FieldCount; i++) 
                        { 
                            var name = reader.GetName(i); 
                            var val = await reader.IsDBNullAsync(i).ConfigureAwait(false) ? null : reader.GetValue(i);
                            // Ensure critical fields are never null
                            if (name == "UsuarioId" && (val == null || val == DBNull.Value))
                            {
                                val = 0;
                            }
                            if (name == "ClienteId" && (val == null || val == DBNull.Value))
                            {
                                val = 0;
                            }
                            row[name] = val;
                        }
                        items.Add(row);
                    }
                }
                
                var countSql = $@"SELECT COUNT(*) FROM Cliente c LEFT JOIN Usuario u ON u.cliente_id = c.cliente_id {(string.IsNullOrWhiteSpace(search) ? "" : "WHERE (c.nombre LIKE @search OR c.email LIKE @search OR u.username LIKE @search)")}";
                using (var cnt = new SqlCommand(countSql, conn)) 
                { 
                    if (!string.IsNullOrWhiteSpace(search)) cnt.Parameters.AddWithValue("@search", "%" + search + "%"); 
                    var total = Convert.ToInt32(await cnt.ExecuteScalarAsync().ConfigureAwait(false)); 
                    return (items, total); 
                }
            }
        }

        public async Task<(IEnumerable<IDictionary<string, object>> Items, int Total)> ObtenerTransaccionesPaginadasAsync(int page, int pageSize, DateTime? desde, DateTime? hasta, string q)
        {
            var offset = (page - 1) * pageSize;
            var whereClauses = new List<string>();
            if (desde.HasValue) whereClauses.Add("CAST(t.fecha AS DATE) >= @desde");
            if (hasta.HasValue) whereClauses.Add("CAST(t.fecha AS DATE) <= @hasta");
            if (!string.IsNullOrWhiteSpace(q)) whereClauses.Add("(t.descripcion LIKE @q OR tt.nombre LIKE @q)");
            var where = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;
            var sql = $@"SELECT t.transaccion_id, t.fecha, tt.nombre AS tipo_transaccion, et.nombre AS estado, t.monto, t.descripcion FROM Transaccion t LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id LEFT JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id {where} ORDER BY t.fecha DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (desde.HasValue) cmd.Parameters.AddWithValue("@desde", desde.Value);
                if (hasta.HasValue) cmd.Parameters.AddWithValue("@hasta", hasta.Value);
                if (!string.IsNullOrWhiteSpace(q)) cmd.Parameters.AddWithValue("@q", "%" + q + "%");
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                await conn.OpenAsync().ConfigureAwait(false);
                var items = new List<IDictionary<string, object>>();
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        for (var i = 0; i < reader.FieldCount; i++) { var name = reader.GetName(i); row[name] = await reader.IsDBNullAsync(i).ConfigureAwait(false) ? null : reader.GetValue(i); }
                        items.Add(row);
                    }
                }
                var countSql = $"SELECT COUNT(*) FROM Transaccion t LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id LEFT JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id {where}";
                using (var cnt = new SqlCommand(countSql, conn))
                {
                    if (desde.HasValue) cnt.Parameters.AddWithValue("@desde", desde.Value);
                    if (hasta.HasValue) cnt.Parameters.AddWithValue("@hasta", hasta.Value);
                    if (!string.IsNullOrWhiteSpace(q)) cnt.Parameters.AddWithValue("@q", "%" + q + "%");
                    var total = Convert.ToInt32(await cnt.ExecuteScalarAsync().ConfigureAwait(false));
                    return (items, total);
                }
            }
        }

        // NEW: Métodos para dropdowns de transferencias
        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerTodasLasCuentasAsync()
        {
            const string sql = @"
                SELECT 
                    c.cuenta_id AS 'ID Cuenta', 
                    c.numero_cuenta AS 'Número de Cuenta', 
                    CASE tc.nombre
                        WHEN 'AHORRO' THEN 'Cuenta de ahorros personal'
                        WHEN 'CORRIENTE' THEN 'Cuenta corriente personal'
                        WHEN 'CREDITO' THEN 'Tarjeta de crédito'
                        WHEN 'DEBITO' THEN 'Tarjeta de débito'
                        ELSE tc.nombre
                    END AS 'Tipo de Cuenta', 
                    c.saldo AS 'Saldo', 
                    cl.nombre AS 'Nombre del Cliente',
                    c.cliente_id AS 'ID Cliente'
                FROM Cuenta c 
                LEFT JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id 
                LEFT JOIN Cliente cl ON c.cliente_id = cl.cliente_id
                WHERE c.activa = 1 
                ORDER BY c.numero_cuenta";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerTodosLosClientesParaTransferenciaAsync()
        {
            const string sql = @"
                SELECT DISTINCT
                    c.cliente_id AS 'ID Cliente',
                    c.nombre AS 'Nombre Cliente',
                    c.email AS 'Email'
                FROM Cliente c
                INNER JOIN Cuenta cu ON c.cliente_id = cu.cliente_id
                WHERE c.activo = 1 AND cu.activa = 1
                ORDER BY c.nombre";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        private async Task<IEnumerable<IDictionary<string, object>>> ExecuteQueryAsync(string sql, params (string name, object value)[] parameters)
        {
            var result = new List<IDictionary<string, object>>();
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                foreach (var p in parameters) cmd.Parameters.AddWithValue(p.name, p.value ?? DBNull.Value);
                await conn.OpenAsync().ConfigureAwait(false);
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        for (var i = 0; i < reader.FieldCount; i++) { var name = reader.GetName(i); var val = await reader.IsDBNullAsync(i).ConfigureAwait(false) ? null : reader.GetValue(i); row[name] = val; }
                        result.Add(row);
                    }
                }
            }
            return result;
        }

        public async Task<bool> IsUsernameAdminAsync(string username)
        {
            const string sql = "SELECT TOP(1) es_admin FROM Usuario WHERE username = @username";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                await conn.OpenAsync().ConfigureAwait(false);
                var val = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                if (val == null || val == DBNull.Value) return false;
                return Convert.ToBoolean(val);
            }
        }

        public async Task<int?> GetClienteIdByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            const string sql = "SELECT TOP(1) cliente_id FROM Usuario WHERE username = @username";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                await conn.OpenAsync().ConfigureAwait(false);
                var val = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                
                // Debug logging
                Console.WriteLine($"DEBUG: GetClienteIdByUsernameAsync - username: {username}, result: {val ?? "NULL"}");
                
                if (val == null || val == DBNull.Value) return null;
                return Convert.ToInt32(val);
            }
        }

        public async Task<int?> GetUsuarioIdByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            const string sql = "SELECT TOP(1) usuario_id FROM Usuario WHERE username = @username";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                await conn.OpenAsync().ConfigureAwait(false);
                var val = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                if (val == null || val == DBNull.Value) return null;
                return Convert.ToInt32(val);
            }
        }

        private async Task LogAuditAsync(string accion, int? usuarioId, string detalle)
        {
            const string sql = @"INSERT INTO AuditLog (accion, usuario_id, detalle, fecha) VALUES (@accion, @usuarioId, @detalle, @fecha)";
            try
            {
                using (var conn = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@accion", accion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId.HasValue ? (object)usuarioId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@detalle", (object)detalle ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.UtcNow);
                    await conn.OpenAsync().ConfigureAwait(false);
                    await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch { }
        }

        private static string ComputeSha256Hex(string input)
        {
            var bytes = Encoding.Unicode.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        private static string ComputeSha256Base64(string input)
        {
            var bytes = Encoding.Unicode.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        private static string GenerateSalt()
        {
            var bytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(bytes);
        }

        private static string GetString(JsonElement root, string name)
        {
            if (root.TryGetProperty(name, out var prop) && prop.ValueKind != JsonValueKind.Null) return prop.GetString();
            return null;
        }

        private static bool TryGetInt(JsonElement root, string name, out int value)
        {
            value = 0;
            if (root.TryGetProperty(name, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out value)) return true;
                int tmp; if (int.TryParse(prop.GetString(), out tmp)) { value = tmp; return true; }
            }
            return false;
        }

        private static bool TryGetDouble(JsonElement root, string name, out double value)
        {
            value = 0;
            if (root.TryGetProperty(name, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDouble(out value)) return true;
                double tmp; if (double.TryParse(prop.GetString(), out tmp)) { value = tmp; return true; }
            }
            return false;
        }

        public async Task<IEnumerable<IDictionary<string, object>>> DiagnosticarTablasAsync()
        {
            const string sql = @"SELECT TABLE_NAME AS tabla_nombre, TABLE_TYPE AS tipo FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> DiagnosticarVistasAsync()
        {
            const string sql = @"SELECT TABLE_NAME AS vista_nombre FROM INFORMATION_SCHEMA.VIEWS ORDER BY TABLE_NAME";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<string> ProbarConsultaBasicaAsync()
        {
            try
            {
                const string sql = "SELECT TOP(1) 1 AS ok";
                await ExecuteQueryAsync(sql).ConfigureAwait(false);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}