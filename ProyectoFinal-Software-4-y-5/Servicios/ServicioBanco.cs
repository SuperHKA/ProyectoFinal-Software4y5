using ProyectoFinal_Software_4_y_5.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ProyectoFinal_Software_4_y_5.Servicios
{
    public class ServicioBanco
    {
        private readonly ConexionBD _conexionBD;

        public ServicioBanco()
        {
            _conexionBD = ConexionBD.ObtenerInstancia();
        }

        // MÉTODOS EXISTENTES
        public List<Usuario> ObtenerTodosLosClientes()
        {
            var clientes = new List<Usuario>();
            const string sql = @"
                SELECT u.usuario_id, u.username, c.nombre, c.email, c.telefono, u.bloqueado, 
                       c.calle, c.numero, c.ciudad, c.estado_provincia, c.codigo_postal, c.pais 
                FROM Usuario u 
                JOIN Cliente c ON u.cliente_id = c.cliente_id";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var usuario = new Usuario(
                        reader.GetInt32(reader.GetOrdinal("usuario_id")),
                        reader.GetString(reader.GetOrdinal("username")),
                        null,
                        null,
                        reader.GetString(reader.GetOrdinal("nombre")),
                        reader.GetString(reader.GetOrdinal("email"))
                    )
                    {
                        Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString(reader.GetOrdinal("telefono")),
                        EstadoCuenta = reader.GetBoolean(reader.GetOrdinal("bloqueado")),
                        Calle = reader.IsDBNull(reader.GetOrdinal("calle")) ? "" : reader.GetString(reader.GetOrdinal("calle")),
                        Numero = reader.IsDBNull(reader.GetOrdinal("numero")) ? "" : reader.GetString(reader.GetOrdinal("numero")),
                        Ciudad = reader.IsDBNull(reader.GetOrdinal("ciudad")) ? "" : reader.GetString(reader.GetOrdinal("ciudad")),
                        EstadoProvincia = reader.IsDBNull(reader.GetOrdinal("estado_provincia")) ? "" : reader.GetString(reader.GetOrdinal("estado_provincia")),
                        CodigoPostal = reader.IsDBNull(reader.GetOrdinal("codigo_postal")) ? "" : reader.GetString(reader.GetOrdinal("codigo_postal")),
                        Pais = reader.IsDBNull(reader.GetOrdinal("pais")) ? "" : reader.GetString(reader.GetOrdinal("pais"))
                    };

                    clientes.Add(usuario);
                }
            }
            return clientes;
        }

        public List<Cuenta> ObtenerCuentasUsuario(int idUsuario)
        {
            var cuentas = new List<Cuenta>();
            const string sql = @"
                SELECT c.cuenta_id, c.cliente_id, tc.nombre AS tipo_cuenta_nombre, 
                       c.numero_cuenta, c.saldo, c.activa 
                FROM Cuenta c 
                JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id 
                WHERE c.cliente_id = @idUsuario AND c.activa = 1";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cuenta = new Cuenta(
                            reader.GetInt32(reader.GetOrdinal("cuenta_id")),
                            reader.GetInt32(reader.GetOrdinal("cliente_id")),
                            (Cuenta.TipoCuenta)Enum.Parse(typeof(Cuenta.TipoCuenta), reader.GetString(reader.GetOrdinal("tipo_cuenta_nombre"))),
                            reader.GetString(reader.GetOrdinal("numero_cuenta")),
                            reader.GetDouble(reader.GetOrdinal("saldo"))
                        )
                        {
                            Activo = reader.GetBoolean(reader.GetOrdinal("activa"))
                        };

                        cuentas.Add(cuenta);
                    }
                }
            }
            return cuentas;
        }

        public bool CrearCuentaBancaria(int idUsuario, Cuenta.TipoCuenta tipo, string numeroCuenta, double saldoInicial)
        {
            const string sql = @"
                INSERT INTO Cuenta (cliente_id, tipo_cuenta_id, numero_cuenta, saldo) 
                VALUES (@clienteId, (SELECT tipo_cuenta_id FROM TipoCuenta WHERE nombre = @tipo), @numeroCuenta, @saldo)";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@clienteId", idUsuario);
                cmd.Parameters.AddWithValue("@tipo", tipo.ToString());
                cmd.Parameters.AddWithValue("@numeroCuenta", numeroCuenta);
                cmd.Parameters.AddWithValue("@saldo", saldoInicial);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ActualizarSaldoCuenta(int idCuenta, double nuevoSaldo)
        {
            const string sql = "UPDATE Cuenta SET saldo = @saldo WHERE cuenta_id = @idCuenta";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@saldo", nuevoSaldo);
                cmd.Parameters.AddWithValue("@idCuenta", idCuenta);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool CambiarContrasena(int idUsuario, string nuevaContrasena)
        {
            var hasher = new Hasher();
            var nuevoSalt = hasher.GenerarSalt();
            var nuevoHash = hasher.GenerarHash(nuevaContrasena, nuevoSalt);

            const string sql = "UPDATE Usuario SET password_hash = @hash, salt = @salt WHERE usuario_id = @idUsuario";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@hash", Encoding.UTF8.GetBytes(nuevoHash));
                cmd.Parameters.AddWithValue("@salt", nuevoSalt);
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable ObtenerMovimientosDelDia()
        {
            var dt = new DataTable();
            const string sql = "SELECT * FROM vw_MovimientosDelDia";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
            return dt;
        }

        public DataTable ObtenerAnalisisTransacciones()
        {
            var dt = new DataTable();
            const string sql = "SELECT * FROM vw_AnalisisTransacciones";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
            return dt;
        }

        public DataTable ObtenerResumenClientes()
        {
            var dt = new DataTable();
            const string sql = "SELECT * FROM vw_ResumenCliente";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
            return dt;
        }

        // NUEVOS MÉTODOS PARA EL PANEL ADMINISTRATIVO

        public DataTable ObtenerEstadisticasGenerales()
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM Cliente WHERE activo = 1) as total_clientes,
                    (SELECT COUNT(*) FROM Cuenta WHERE activa = 1) as total_cuentas,
                    (SELECT COUNT(*) FROM Usuario WHERE bloqueado = 0) as usuarios_activos,
                    (SELECT COUNT(*) FROM Usuario WHERE bloqueado = 1) as usuarios_bloqueados,
                    (SELECT ISNULL(SUM(saldo), 0) FROM Cuenta WHERE activa = 1) as saldo_total_sistema,
                    (SELECT COUNT(*) FROM Transaccion WHERE CAST(fecha AS DATE) = CAST(GETDATE() AS DATE)) as transacciones_hoy,
                    (SELECT ISNULL(SUM(monto), 0) FROM Transaccion WHERE CAST(fecha AS DATE) = CAST(GETDATE() AS DATE)) as monto_total_hoy";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
            return dt;
        }

        public DataTable ObtenerTransaccionesRecientes(int top = 50)
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT TOP (@top)
                    t.transaccion_id,
                    t.fecha,
                    tt.nombre as tipo_transaccion,
                    t.monto,
                    et.nombre as estado,
                    c_origen.numero_cuenta as cuenta_origen,
                    c_destino.numero_cuenta as cuenta_destino,
                    cli.nombre as cliente_nombre,
                    u.username as usuario
                FROM Transaccion t
                JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id
                JOIN EstadoTransaccion et ON t.estado_transaccion_id = et.estado_transaccion_id
                LEFT JOIN Cuenta c_origen ON t.cuenta_origen = c_origen.cuenta_id
                LEFT JOIN Cuenta c_destino ON t.cuenta_destino = c_destino.cuenta_id
                LEFT JOIN Cliente cli ON c_origen.cliente_id = cli.cliente_id OR c_destino.cliente_id = cli.cliente_id
                LEFT JOIN Usuario u ON t.usuario_id = u.usuario_id
                ORDER BY t.fecha DESC";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@top", top);
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public bool BloquearDesbloquearUsuario(int usuarioId, bool bloquear)
        {
            const string sql = "UPDATE Usuario SET bloqueado = @bloqueado, intentos_fallidos = 0 WHERE usuario_id = @usuarioId";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@bloqueado", bloquear);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable ObtenerUsuariosBloqueados()
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT 
                    u.usuario_id,
                    u.username,
                    c.nombre as cliente_nombre,
                    c.email,
                    u.intentos_fallidos,
                    u.fecha_ultimo_login,
                    u.bloqueado
                FROM Usuario u
                JOIN Cliente c ON u.cliente_id = c.cliente_id
                WHERE u.bloqueado = 1";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
            return dt;
        }

        public bool EjecutarResumenesDiarios()
        {
            try
            {
                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand("sp_EnviarResumenesDiarios", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log del error si es necesario
                System.Diagnostics.Debug.WriteLine($"Error al ejecutar resúmenes diarios: {ex.Message}");
                return false;
            }
        }

        public DataTable ObtenerNotificacionesSistema()
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT 
                    n.notificacion_id,
                    c.nombre as cliente_nombre,
                    n.tipo,
                    n.asunto,
                    n.fecha_envio,
                    n.estado
                FROM Notificacion n
                LEFT JOIN Cliente c ON n.cliente_id = c.cliente_id
                ORDER BY n.fecha_envio DESC";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
            return dt;
        }

        public DataTable ObtenerReporteTransaccionesPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT 
                    t.fecha,
                    tt.nombre as tipo_transaccion,
                    COUNT(*) as cantidad,
                    SUM(t.monto) as monto_total,
                    AVG(t.monto) as monto_promedio
                FROM Transaccion t
                JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id
                WHERE t.fecha BETWEEN @fechaInicio AND @fechaFin
                GROUP BY CAST(t.fecha AS DATE), tt.nombre
                ORDER BY CAST(t.fecha AS DATE) DESC";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public bool CrearUsuarioAdministrador(string username, string password, string nombreCompleto, string email)
        {
            using (var conexion = _conexionBD.GetConexion())
            using (var transaction = conexion.BeginTransaction())
            {
                try
                {
                    // 1. Insertar en Cliente
                    const string sqlCliente = @"
                        INSERT INTO Cliente (nombre, email, activo) 
                        VALUES (@nombre, @email, 1);
                        SELECT SCOPE_IDENTITY();";

                    int clienteId;
                    using (var cmdCliente = new SqlCommand(sqlCliente, conexion, transaction))
                    {
                        cmdCliente.Parameters.AddWithValue("@nombre", nombreCompleto);
                        cmdCliente.Parameters.AddWithValue("@email", email);
                        clienteId = Convert.ToInt32(cmdCliente.ExecuteScalar());
                    }

                    // 2. Insertar en Usuario con rol de administrador
                    const string sqlUsuario = @"
                        INSERT INTO Usuario (cliente_id, username, password_hash, salt, es_admin) 
                        VALUES (@clienteId, @username, @passwordHash, @salt, 1)";

                    var hasher = new Hasher();
                    var salt = hasher.GenerarSalt();
                    var hash = hasher.GenerarHash(password, salt);

                    using (var cmdUsuario = new SqlCommand(sqlUsuario, conexion, transaction))
                    {
                        cmdUsuario.Parameters.AddWithValue("@clienteId", clienteId);
                        cmdUsuario.Parameters.AddWithValue("@username", username);
                        cmdUsuario.Parameters.AddWithValue("@passwordHash", Encoding.UTF8.GetBytes(hash));
                        cmdUsuario.Parameters.AddWithValue("@salt", salt);
                        cmdUsuario.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool CrearClienteCompleto(Usuario usuario, string contrasena)
        {
            using (var conexion = _conexionBD.GetConexion())
            using (var transaction = conexion.BeginTransaction())
            {
                try
                {
                    // 1. Insertar en Cliente
                    const string sqlCliente = @"
                        INSERT INTO Cliente (nombre, email, telefono, calle, numero, ciudad, estado_provincia, codigo_postal, pais) 
                        VALUES (@nombre, @email, @telefono, @calle, @numero, @ciudad, @estado_provincia, @codigo_postal, @pais);
                        SELECT SCOPE_IDENTITY();";

                    int clienteId;
                    using (var cmdCliente = new SqlCommand(sqlCliente, conexion, transaction))
                    {
                        cmdCliente.Parameters.AddWithValue("@nombre", usuario.NombreCompleto);
                        cmdCliente.Parameters.AddWithValue("@email", usuario.Email);
                        cmdCliente.Parameters.AddWithValue("@telefono", (object)usuario.Telefono ?? DBNull.Value);
                        cmdCliente.Parameters.AddWithValue("@calle", (object)usuario.Calle ?? DBNull.Value);
                        cmdCliente.Parameters.AddWithValue("@numero", (object)usuario.Numero ?? DBNull.Value);
                        cmdCliente.Parameters.AddWithValue("@ciudad", (object)usuario.Ciudad ?? DBNull.Value);
                        cmdCliente.Parameters.AddWithValue("@estado_provincia", (object)usuario.EstadoProvincia ?? DBNull.Value);
                        cmdCliente.Parameters.AddWithValue("@codigo_postal", (object)usuario.CodigoPostal ?? DBNull.Value);
                        cmdCliente.Parameters.AddWithValue("@pais", (object)usuario.Pais ?? DBNull.Value);
                        clienteId = Convert.ToInt32(cmdCliente.ExecuteScalar());
                    }

                    // 2. Insertar en Usuario
                    const string sqlUsuario = @"
                        INSERT INTO Usuario (cliente_id, username, password_hash, salt) 
                        VALUES (@clienteId, @username, @passwordHash, @salt)";

                    var hasher = new Hasher();
                    var salt = hasher.GenerarSalt();
                    var hash = hasher.GenerarHash(contrasena, salt);

                    using (var cmdUsuario = new SqlCommand(sqlUsuario, conexion, transaction))
                    {
                        cmdUsuario.Parameters.AddWithValue("@clienteId", clienteId);
                        cmdUsuario.Parameters.AddWithValue("@username", usuario.NombreUsuario);
                        cmdUsuario.Parameters.AddWithValue("@passwordHash", Encoding.UTF8.GetBytes(hash));
                        cmdUsuario.Parameters.AddWithValue("@salt", salt);
                        cmdUsuario.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool ActualizarCliente(Usuario usuario)
        {
            const string sql = @"
                UPDATE Cliente 
                SET nombre = @nombre, 
                    email = @email, 
                    telefono = @telefono, 
                    calle = @calle, 
                    numero = @numero, 
                    ciudad = @ciudad, 
                    estado_provincia = @estado_provincia, 
                    codigo_postal = @codigo_postal, 
                    pais = @pais 
                WHERE cliente_id = (SELECT cliente_id FROM Usuario WHERE usuario_id = @usuarioId)";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@usuarioId", usuario.IdUsuario);
                cmd.Parameters.AddWithValue("@nombre", usuario.NombreCompleto);
                cmd.Parameters.AddWithValue("@email", usuario.Email);
                cmd.Parameters.AddWithValue("@telefono", (object)usuario.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@calle", (object)usuario.Calle ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@numero", (object)usuario.Numero ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ciudad", (object)usuario.Ciudad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@estado_provincia", (object)usuario.EstadoProvincia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@codigo_postal", (object)usuario.CodigoPostal ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@pais", (object)usuario.Pais ?? DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool EliminarCliente(int usuarioId)
        {
            // La eliminación en cascada se manejará automáticamente por las FK
            const string sql = "DELETE FROM Usuario WHERE usuario_id = @usuarioId";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<Cuenta> ObtenerTodasLasCuentasConClientes()
        {
            var cuentas = new List<Cuenta>();
            const string sql = @"
                SELECT c.cuenta_id, c.cliente_id, c.tipo_cuenta_id, c.numero_cuenta, c.saldo, c.activa, 
                       cli.nombre AS nombre_cliente, tc.nombre AS tipo_cuenta_nombre 
                FROM Cuenta c 
                JOIN Cliente cli ON c.cliente_id = cli.cliente_id 
                JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id 
                WHERE c.activa = 1";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var cuenta = new Cuenta(
                        reader.GetInt32(reader.GetOrdinal("cuenta_id")),
                        reader.GetInt32(reader.GetOrdinal("cliente_id")),
                        (Cuenta.TipoCuenta)Enum.Parse(typeof(Cuenta.TipoCuenta), reader.GetString(reader.GetOrdinal("tipo_cuenta_nombre"))),
                        reader.GetString(reader.GetOrdinal("numero_cuenta")),
                        reader.GetDouble(reader.GetOrdinal("saldo"))
                    )
                    {
                        Activo = reader.GetBoolean(reader.GetOrdinal("activa")),
                        NombreCliente = reader.GetString(reader.GetOrdinal("nombre_cliente"))
                    };

                    cuentas.Add(cuenta);
                }
            }
            return cuentas;
        }

        public DataTable ObtenerTopClientesPorSaldo(int top = 10)
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT TOP (@top)
                    c.nombre as cliente_nombre,
                    c.email,
                    COUNT(cu.cuenta_id) as total_cuentas,
                    SUM(cu.saldo) as saldo_total
                FROM Cliente c
                JOIN Cuenta cu ON c.cliente_id = cu.cliente_id
                WHERE cu.activa = 1
                GROUP BY c.cliente_id, c.nombre, c.email
                ORDER BY saldo_total DESC";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@top", top);
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable ObtenerTransaccionesPorTipo(DateTime fechaInicio, DateTime fechaFin)
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT 
                    tt.nombre as tipo_transaccion,
                    COUNT(*) as cantidad,
                    SUM(t.monto) as monto_total,
                    AVG(t.monto) as monto_promedio,
                    MIN(t.fecha) as primera_transaccion,
                    MAX(t.fecha) as ultima_transaccion
                FROM Transaccion t
                JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id
                WHERE t.fecha BETWEEN @fechaInicio AND @fechaFin
                GROUP BY tt.nombre
                ORDER BY monto_total DESC";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}