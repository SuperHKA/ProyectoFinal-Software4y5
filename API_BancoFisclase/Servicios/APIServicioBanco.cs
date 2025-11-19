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
            if (string.IsNullOrWhiteSpace(_cs))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        // Authentication (existing)
        public async Task<LoginResultDto> AuthenticateAdminAsync(string username, string password)
        {
            const string sql = @"
SELECT usuario_id, username, password_hash, salt, intentos_fallidos, bloqueado, es_admin
FROM Usuario
WHERE username = @username";

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                await conn.OpenAsync().ConfigureAwait(false);

                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (!await reader.ReadAsync().ConfigureAwait(false))
                    {
                        return new LoginResultDto { Status = EstatusLogin.NotFound, Message = "Usuario no encontrado" };
                    }

                    var id = reader.GetInt32(reader.GetOrdinal("usuario_id"));
                    var dbHash = reader.IsDBNull(reader.GetOrdinal("password_hash")) ? null : reader.GetString(reader.GetOrdinal("password_hash"));
                    var salt = reader.IsDBNull(reader.GetOrdinal("salt")) ? string.Empty : reader.GetString(reader.GetOrdinal("salt"));
                    var bloqueado = !reader.IsDBNull(reader.GetOrdinal("bloqueado")) && reader.GetBoolean(reader.GetOrdinal("bloqueado"));
                    var esAdmin = !reader.IsDBNull(reader.GetOrdinal("es_admin")) && reader.GetBoolean(reader.GetOrdinal("es_admin"));

                    if (bloqueado)
                        return new LoginResultDto { Status = EstatusLogin.Locked, Message = "Cuenta bloqueada" };

                    // Compute both hex and base64 forms of SHA256(password + salt) using Unicode encoding
                    var computedHex = ComputeSha256Hex(password + salt);
                    var computedBase64 = ComputeSha256Base64(password + salt);

                    // Compare against DB hash, accept either hex (64 hex chars) or base64
                    bool passwordMatches = false;
                    if (!string.IsNullOrEmpty(dbHash))
                    {
                        // hex: 64 hex characters
                        if (dbHash.Length == 64 && Regex.IsMatch(dbHash, @"\A[0-9a-fA-F]{64}\z"))
                        {
                            passwordMatches = string.Equals(computedHex, dbHash, StringComparison.OrdinalIgnoreCase);
                        }
                        else
                        {
                            // treat as base64 (default)
                            passwordMatches = string.Equals(computedBase64, dbHash, StringComparison.Ordinal);
                        }
                    }

                    if (!passwordMatches)
                    {
                        // increment attempts and possibly lock
                        reader.Close();

                        const string sqlUpdateAttempts = @"
UPDATE Usuario
SET intentos_fallidos = intentos_fallidos + 1,
    bloqueado = CASE WHEN intentos_fallidos + 1 >= 3 THEN 1 ELSE 0 END
WHERE usuario_id = @id";

                        using (var upd = new SqlCommand(sqlUpdateAttempts, conn))
                        {
                            upd.Parameters.AddWithValue("@id", id);
                            await upd.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        return new LoginResultDto { Status = EstatusLogin.BadCredentials, Message = "Credenciales incorrectas" };
                    }

                    if (!esAdmin)
                    {
                        return new LoginResultDto { Status = EstatusLogin.Forbidden, Message = "Usuario no es administrador" };
                    }

                    // success -> reset attempts and update last login
                    reader.Close();

                    const string sqlReset = @"
UPDATE Usuario
SET intentos_fallidos = 0, fecha_ultimo_login = @fecha
WHERE usuario_id = @id";

                    using (var reset = new SqlCommand(sqlReset, conn))
                    {
                        reset.Parameters.AddWithValue("@fecha", DateTime.Now);
                        reset.Parameters.AddWithValue("@id", id);
                        await reset.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }

                    return new LoginResultDto
                    {
                        Status = EstatusLogin.Success,
                        Message = "Login exitoso",
                        User = new UserDto { Id = id, Username = username }
                    };
                }
            }
        }

        // Read-only implementations using ExecuteQueryAsync
        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerEstadisticasGeneralesAsync()
        {
            const string sql = @"
SELECT 
    (SELECT COUNT(*) FROM Cliente WHERE activo = 1) as total_clientes,
    (SELECT COUNT(*) FROM Cuenta WHERE activa = 1) as total_cuentas,
    (SELECT COUNT(*) FROM Usuario WHERE bloqueado = 0) as usuarios_activos,
    (SELECT COUNT(*) FROM Usuario WHERE bloqueado = 1) as usuarios_bloqueados,
    (SELECT ISNULL(SUM(saldo), 0) FROM Cuenta WHERE activa = 1) as saldo_total_sistema,
    (SELECT COUNT(*) FROM Transaccion WHERE CAST(fecha AS DATE) = CAST(GETDATE() AS DATE)) as transacciones_hoy,
    (SELECT ISNULL(SUM(monto), 0) FROM Transaccion WHERE CAST(fecha AS DATE) = CAST(GETDATE() AS DATE)) as monto_total_hoy";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerMovimientosDelDiaAsync()
        {
            const string sql = "SELECT * FROM vw_MovimientosDelDia";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerClientesAsync()
        {
            const string sql = @"
SELECT u.usuario_id, u.cliente_id, u.username, c.nombre AS nombre_completo, c.email, c.telefono, u.es_admin, u.bloqueado
FROM Usuario u
LEFT JOIN Cliente c ON u.cliente_id = c.cliente_id
ORDER BY u.usuario_id";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerTransaccionesRecientesAsync(int limit = 20)
        {
            const string sqlTemplate = @"
SELECT TOP(@limit) t.transaccion_id, t.fecha, tt.nombre AS tipo_transaccion, et.nombre AS estado, t.monto, t.descripcion
FROM Transaccion t
LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id
LEFT JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id
ORDER BY t.fecha DESC";
            return await ExecuteQueryAsync(sqlTemplate, ("@limit", limit)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerAnalisisTransaccionesAsync()
        {
            const string sql = @"SELECT * FROM vw_AnalisisTransacciones";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerResumenClientesAsync()
        {
            const string sql = @"SELECT * FROM vw_ResumenCliente";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerUsuariosBloqueadosAsync()
        {
            const string sql = @"SELECT u.usuario_id, u.username, u.intentos_fallidos, u.bloqueado, c.nombre as cliente_nombre
FROM Usuario u
LEFT JOIN Cliente c ON u.cliente_id = c.cliente_id
WHERE u.bloqueado = 1";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerNotificacionesSistemaAsync()
        {
            const string sql = @"SELECT notificacion_id, cliente_id, tipo, asunto, fecha_envio, estado FROM Notificacion ORDER BY fecha_envio DESC";
            return await ExecuteQueryAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> ObtenerReporteTransaccionesPorFechaAsync(DateTime desde, DateTime hasta)
        {
            const string sql = @"
SELECT t.transaccion_id, tt.nombre AS tipo_transaccion, t.monto, et.nombre AS estado, t.descripcion, t.fecha, t.usuario_id
FROM Transaccion t
LEFT JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id
LEFT JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id
WHERE t.fecha BETWEEN @desde AND @hasta
ORDER BY t.fecha DESC";
            return await ExecuteQueryAsync(sql, ("@desde", desde), ("@hasta", hasta)).ConfigureAwait(false);
        }

        // Diagnostics
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
                const string sql = "SELECT TOP(1) 1 as ok";
                var result = await ExecuteQueryAsync(sql).ConfigureAwait(false);
                return "OK";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // Actions / writes
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

        // Implementations added: handle loosely-typed cliente/cuenta/admin DTOs by serializing to JSON and reading properties.
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

                        // password param or inside DTO
                        var pwd = string.IsNullOrEmpty(password) ? (GetString(root, "Password") ?? GetString(root, "password")) : password;
                        if (string.IsNullOrEmpty(pwd)) throw new ArgumentException("Password is required");

                        // Insert Cliente
                        cmd.CommandText = @"
INSERT INTO Cliente (nombre, email, telefono, activo)
VALUES (@nombre, @email, @telefono, 1);
SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@telefono", (object)telefono ?? DBNull.Value);

                        var clienteIdObj = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                        var clienteId = Convert.ToInt32(clienteIdObj);

                        // Create salt + hash
                        var salt = GenerateSalt();
                        var hash = ComputeSha256Base64(pwd + salt);

                        // Insert Usuario
                        cmd.CommandText = @"
INSERT INTO Usuario (cliente_id, username, password_hash, salt, es_admin)
VALUES (@clienteId, @username, @passwordHash, @salt, 0)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@clienteId", clienteId);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@passwordHash", hash);
                        cmd.Parameters.AddWithValue("@salt", salt);
                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        tx.Commit();
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
            using (var cmd = new SqlCommand(@"
UPDATE Cliente
SET nombre = @nombre, email = @email, telefono = @telefono, calle = @calle, numero = @numero, ciudad = @ciudad, estado_provincia = @estado_provincia, codigo_postal = @codigo_postal, pais = @pais
WHERE cliente_id = (SELECT cliente_id FROM Usuario WHERE usuario_id = @usuarioId)", conn))
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
                return rows > 0;
            }
        }

        public async Task<bool> CrearCuentaBancariaAsync(object cuentaDto)
        {
            if (cuentaDto == null) throw new ArgumentNullException(nameof(cuentaDto));
            var doc = JsonDocument.Parse(JsonSerializer.Serialize(cuentaDto));
            var root = doc.RootElement;

            if (!TryGetInt(root, "ClienteId", out var clienteId) && !TryGetInt(root, "clienteId", out clienteId))
                throw new ArgumentException("ClienteId is required");

            var tipo = GetString(root, "Tipo") ?? GetString(root, "tipo") ?? throw new ArgumentException("Tipo is required");
            var numero = GetString(root, "NumeroCuenta") ?? GetString(root, "numeroCuenta") ?? GetString(root, "Numero") ?? GetString(root, "numero") ?? throw new ArgumentException("NumeroCuenta is required");
            var saldoInicial = TryGetDouble(root, "SaldoInicial", out var temp) || TryGetDouble(root, "saldoInicial", out temp) ? temp : 0.0;

            const string sql = @"
INSERT INTO Cuenta (cliente_id, tipo_cuenta_id, numero_cuenta, saldo, activa)
VALUES (@clienteId, (SELECT tipo_cuenta_id FROM TipoCuenta WHERE nombre = @tipo), @numero, @saldo)";
            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@clienteId", clienteId);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@numero", numero);
                cmd.Parameters.AddWithValue("@saldo", Convert.ToDecimal(saldoInicial));
                await conn.OpenAsync().ConfigureAwait(false);
                var rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                return rows > 0;
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
                return rows > 0;
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
                        cmd.CommandText = @"
INSERT INTO Cliente (nombre, email, activo)
VALUES (@nombre, @email, 1);
SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@email", email);
                        var clienteIdObj = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                        var clienteId = Convert.ToInt32(clienteIdObj);

                        var salt = GenerateSalt();
                        var hash = ComputeSha256Base64(password + salt);

                        cmd.CommandText = @"
INSERT INTO Usuario (cliente_id, username, password_hash, salt, es_admin)
VALUES (@clienteId, @username, @passwordHash, @salt, 1)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@clienteId", clienteId);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@passwordHash", hash);
                        cmd.Parameters.AddWithValue("@salt", salt);

                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                        tx.Commit();
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

        // Reuse ExecuteQueryAsync helper for the SQL-based read methods
        private async Task<IEnumerable<IDictionary<string, object>>> ExecuteQueryAsync(string sql, params (string name, object value)[] parameters)
        {
            var result = new List<IDictionary<string, object>>();

            using (var conn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, conn))
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(p.name, p.value ?? DBNull.Value);
                }

                await conn.OpenAsync().ConfigureAwait(false);

                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            var val = await reader.IsDBNullAsync(i).ConfigureAwait(false) ? null : reader.GetValue(i);
                            row[name] = val;
                        }
                        result.Add(row);
                    }
                }
            }

            return result;
        }

        private static string ComputeSha256Hex(string input)
        {
            var bytes = Encoding.Unicode.GetBytes(input); // <-- Unicode (match SQL NVARCHAR)
            var hash = SHA256.HashData(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        private static string ComputeSha256Base64(string input)
        {
            var bytes = Encoding.Unicode.GetBytes(input); // <-- Unicode (match SQL NVARCHAR)
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        private static string GenerateSalt()
        {
            var bytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(bytes);
        }

        // JSON helpers
        private static string GetString(JsonElement root, string name)
        {
            if (root.TryGetProperty(name, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                return prop.GetString();
            }
            return null;
        }

        private static bool TryGetInt(JsonElement root, string name, out int value)
        {
            value = 0;
            if (root.TryGetProperty(name, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out value)) return true;
                int tmp;
                if (int.TryParse(prop.GetString(), out tmp)) { value = tmp; return true; }
            }
            return false;
        }

        private static bool TryGetDouble(JsonElement root, string name, out double value)
        {
            value = 0;
            if (root.TryGetProperty(name, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDouble(out value)) return true;
                double tmp;
                if (double.TryParse(prop.GetString(), out tmp)) { value = tmp; return true; }
            }
            return false;
        }
    }
}