using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
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
                    var attempts = reader.IsDBNull(reader.GetOrdinal("intentos_fallidos")) ? 0 : reader.GetInt32(reader.GetOrdinal("intentos_fallidos"));

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
    }
}