using ProyectoFinal_Software_4_y_5.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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

        public DataTable DiagnosticarTablas()
        {
            var dt = new DataTable();
            const string sql = @"
        SELECT 
            TABLE_NAME as tabla_nombre,
            TABLE_TYPE as tipo
        FROM INFORMATION_SCHEMA.TABLES 
        WHERE TABLE_NAME IN ('Cliente', 'Usuario', 'Cuenta', 'Transaccion', 'Movimiento', 'Notificacion')";

            try
            {
                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand(sql, conexion))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                // Si hay error, crear una tabla con el error
                dt.Columns.Add("Error");
                dt.Rows.Add($"Error al diagnosticar tablas: {ex.Message}");
            }
            return dt;
        }

        public DataTable DiagnosticarVistas()
        {
            var dt = new DataTable();
            const string sql = @"
        SELECT 
            name as vista_nombre,
            type_desc as tipo
        FROM sys.views 
        WHERE name IN ('vw_MovimientosDelDia', 'vw_AnalisisTransacciones', 'vw_ResumenCliente')";

            try
            {
                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand(sql, conexion))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                dt.Columns.Add("Error");
                dt.Rows.Add($"Error al diagnosticar vistas: {ex.Message}");
            }
            return dt;
        }

        public string ProbarConsultaBasica()
        {
            try
            {
                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente", conexion))
                {
                    var resultado = cmd.ExecuteScalar();
                    return $"✅ Consulta básica exitosa. Clientes en BD: {resultado}";
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error en consulta básica: {ex.Message}";
            }
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
                        // saldo es DECIMAL en DB -> leer con GetDecimal y convertir a double
                        var saldoDecimal = reader.IsDBNull(reader.GetOrdinal("saldo")) 
                            ? 0m 
                            : reader.GetDecimal(reader.GetOrdinal("saldo"));

                        var cuenta = new Cuenta(
                            reader.GetInt32(reader.GetOrdinal("cuenta_id")),
                            reader.GetInt32(reader.GetOrdinal("cliente_id")),
                            (Cuenta.TipoCuenta)Enum.Parse(typeof(Cuenta.TipoCuenta), reader.GetString(reader.GetOrdinal("tipo_cuenta_nombre"))),
                            reader.GetString(reader.GetOrdinal("numero_cuenta")),
                            (double)saldoDecimal
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
                // Asegurar tipo compatible: usar decimal para la DB DECIMAL(18,2)
                cmd.Parameters.AddWithValue("@saldo", Convert.ToDecimal(nuevoSaldo));
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
            // Select and group by the date (CAST to DATE) to avoid GROUP BY error on datetime column
            const string sql = @"
                SELECT 
                    CAST(t.fecha AS DATE) as fecha,
                    tt.nombre as tipo_transaccion,
                    COUNT(*) as cantidad,
                    SUM(t.monto) as monto_total,
                    AVG(t.monto) as monto_promedio
                FROM Transaccion t
                JOIN TipoTransaccion tt ON t.tipo_transaccion_id = tt.tipo_transaccion_id
                WHERE CAST(t.fecha AS DATE) BETWEEN @fechaInicio AND @fechaFin
                GROUP BY CAST(t.fecha AS DATE), tt.nombre
                ORDER BY CAST(t.fecha AS DATE) DESC";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin.Date);
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
                    // Leer saldo como decimal y convertir a double
                    var saldoDecimal = reader.IsDBNull(reader.GetOrdinal("saldo"))
                        ? 0m
                        : reader.GetDecimal(reader.GetOrdinal("saldo"));

                    var cuenta = new Cuenta(
                        reader.GetInt32(reader.GetOrdinal("cuenta_id")),
                        reader.GetInt32(reader.GetOrdinal("cliente_id")),
                        (Cuenta.TipoCuenta)Enum.Parse(typeof(Cuenta.TipoCuenta), reader.GetString(reader.GetOrdinal("tipo_cuenta_nombre"))),
                        reader.GetString(reader.GetOrdinal("numero_cuenta")),
                        (double)saldoDecimal
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
        public List<Usuario> ObtenerTodosLosClientes()
        {
            var clientes = new List<Usuario>();

            try
            {
                Debug.WriteLine("[ServicioBanco] Ejecutando ObtenerTodosLosClientes...");

                const string sql = @"
            SELECT u.usuario_id, u.username, c.nombre, c.email, c.telefono, u.bloqueado, 
                   c.calle, c.numero, c.ciudad, c.estado_provincia, c.codigo_postal, c.pais 
            FROM Usuario u 
            JOIN Cliente c ON u.cliente_id = c.cliente_id";

                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand(sql, conexion))
                using (var reader = cmd.ExecuteReader())
                {
                    var filas = 0;
                    while (reader.Read())
                    {
                        filas++;
                        try
                        {
                            var usuario = new Usuario(
                                reader.IsDBNull(reader.GetOrdinal("usuario_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("usuario_id")),
                                reader.IsDBNull(reader.GetOrdinal("username")) ? string.Empty : reader.GetString(reader.GetOrdinal("username")),
                                null,
                                null,
                                reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre")),
                                reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email"))
                            )
                            {
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                                EstadoCuenta = reader.IsDBNull(reader.GetOrdinal("bloqueado")) ? false : reader.GetBoolean(reader.GetOrdinal("bloqueado")),
                                Calle = reader.IsDBNull(reader.GetOrdinal("calle")) ? null : reader.GetString(reader.GetOrdinal("calle")),
                                Numero = reader.IsDBNull(reader.GetOrdinal("numero")) ? null : reader.GetString(reader.GetOrdinal("numero")),
                                Ciudad = reader.IsDBNull(reader.GetOrdinal("ciudad")) ? null : reader.GetString(reader.GetOrdinal("ciudad")),
                                EstadoProvincia = reader.IsDBNull(reader.GetOrdinal("estado_provincia")) ? null : reader.GetString(reader.GetOrdinal("estado_provincia")),
                                CodigoPostal = reader.IsDBNull(reader.GetOrdinal("codigo_postal")) ? null : reader.GetString(reader.GetOrdinal("codigo_postal")),
                                Pais = reader.IsDBNull(reader.GetOrdinal("pais")) ? null : reader.GetString(reader.GetOrdinal("pais"))
                            };

                            clientes.Add(usuario);
                        }
                        catch (Exception ex)
                        {
                            // Registrar y continuar con demás filas
                            System.Diagnostics.Debug.WriteLine($"[ServicioBanco] Error procesando fila {filas}: {ex.Message}");
                        }
                    }

                    Debug.WriteLine($"[ServicioBanco] ObtenerTodosLosClientes - filas leidas: {filas}");
                }
            }
            catch (Exception ex)
            {
                // No vuelvas a encapsular con throw nuevo que oculte la traza — log y devolver lista vacía para que la UI no rompa.
                System.Diagnostics.Debug.WriteLine($"[ServicioBanco] Error al obtener clientes: {ex.Message}");
                // Opcional: MessageBox.Show(...) solo si quieres mostrar al usuario.
            }

            return clientes;
        }

        public DataTable ObtenerMovimientosDelDia()
        {
            var dt = new DataTable();

            try
            {
                const string sql = "SELECT * FROM vw_MovimientosDelDia";

                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand(sql, conexion))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                // Si no hay datos, crear estructura básica (limpiando columnas tipadas)
                if (dt.Rows.Count == 0)
                {
                    dt.Clear();
                    dt.Columns.Clear();
                    dt.Columns.Add("Mensaje", typeof(string));
                    dt.Rows.Add("No hay movimientos para hoy");
                }
            }
            catch (Exception ex)
            {
                // Si la vista no existe, crear una tabla con mensaje de error (limpia estructura previa)
                dt.Clear();
                dt.Columns.Clear();
                dt.Columns.Add("Error", typeof(string));
                dt.Rows.Add($"Error al cargar movimientos: {ex.Message}");
            }

            return dt;
        }

        public DataTable ObtenerAnalisisTransacciones()
        {
            var dt = new DataTable();

            try
            {
                const string sql = "SELECT * FROM vw_AnalisisTransacciones";

                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand(sql, conexion))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    dt.Clear();
                    dt.Columns.Clear();
                    dt.Columns.Add("Mensaje", typeof(string));
                    dt.Rows.Add("No hay datos de análisis de transacciones");
                }
            }
            catch (Exception ex)
            {
                dt.Clear();
                dt.Columns.Clear();
                dt.Columns.Add("Error", typeof(string));
                dt.Rows.Add($"Error al cargar análisis: {ex.Message}");
            }

            return dt;
        }

        public DataTable ObtenerResumenClientes()
        {
            var dt = new DataTable();

            try
            {
                const string sql = "SELECT * FROM vw_ResumenCliente";

                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand(sql, conexion))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    dt.Clear();
                    dt.Columns.Clear();
                    dt.Columns.Add("Mensaje", typeof(string));
                    dt.Rows.Add("No hay datos de resumen de clientes");
                }
            }
            catch (Exception ex)
            {
                dt.Clear();
                dt.Columns.Clear();
                dt.Columns.Add("Error", typeof(string));
                dt.Rows.Add($"Error al cargar resumen: {ex.Message}");
            }

            return dt;
        }

        public DataTable ObtenerEstadisticasGenerales()
        {
            var dt = new DataTable();

            try
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

                using (var conexion = _conexionBD.GetConexion())
                using (var cmd = new SqlCommand(sql, conexion))
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }

                // Asegurarse de que siempre haya una fila
                if (dt.Rows.Count == 0)
                {
                    dt.Columns.Add("total_clientes", typeof(int));
                    dt.Columns.Add("total_cuentas", typeof(int));
                    dt.Columns.Add("usuarios_activos", typeof(int));
                    dt.Columns.Add("usuarios_bloqueados", typeof(int));
                    dt.Columns.Add("saldo_total_sistema", typeof(decimal));
                    dt.Columns.Add("transacciones_hoy", typeof(int));
                    dt.Columns.Add("monto_total_hoy", typeof(decimal));

                    dt.Rows.Add(0, 0, 0, 0, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                // Crear estructura básica con valores por defecto
                dt.Columns.Add("total_clientes", typeof(int));
                dt.Columns.Add("total_cuentas", typeof(int));
                dt.Columns.Add("usuarios_activos", typeof(int));
                dt.Columns.Add("usuarios_bloqueados", typeof(int));
                dt.Columns.Add("saldo_total_sistema", typeof(decimal));
                dt.Columns.Add("transacciones_hoy", typeof(int));
                dt.Columns.Add("monto_total_hoy", typeof(decimal));
                dt.Columns.Add("Error", typeof(string));

                dt.Rows.Add(0, 0, 0, 0, 0, 0, 0, $"Error: {ex.Message}");
            }

            return dt;
        }

        public enum LoginResult
        {
            Success,
            UsuarioNoExiste,
            Bloqueado,
            ContrasenaIncorrecta,
            NoAdmin
        }

        public LoginResult AutenticarAdministrador(string username, string password, out Usuario usuario)
        {
            usuario = null;

            const string sql = @"
        SELECT u.usuario_id, u.username, u.password_hash, u.salt, u.intentos_fallidos, u.bloqueado, u.es_admin,
               c.nombre, c.email, c.telefono, c.calle, c.numero, c.ciudad, c.estado_provincia, c.codigo_postal, c.pais
        FROM Usuario u
        JOIN Cliente c ON u.cliente_id = c.cliente_id
        WHERE u.username = @username";

            using (var conexion = _conexionBD.GetConexion())
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@username", username);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return LoginResult.UsuarioNoExiste;
                    }

                    var usuarioId = reader.IsDBNull(reader.GetOrdinal("usuario_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("usuario_id"));
                    var dbUsername = reader.IsDBNull(reader.GetOrdinal("username")) ? string.Empty : reader.GetString(reader.GetOrdinal("username"));
                    var salt = reader.IsDBNull(reader.GetOrdinal("salt")) ? string.Empty : reader.GetString(reader.GetOrdinal("salt"));
                    var dbHash = reader.IsDBNull(reader.GetOrdinal("password_hash")) ? null : (byte[])reader["password_hash"];
                    var bloqueado = reader.IsDBNull(reader.GetOrdinal("bloqueado")) ? false : reader.GetBoolean(reader.GetOrdinal("bloqueado"));
                    var esAdmin = reader.IsDBNull(reader.GetOrdinal("es_admin")) ? false : reader.GetBoolean(reader.GetOrdinal("es_admin"));
                    var nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("nombre"));
                    var email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email"));
                    var telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono"));
                    var calle = reader.IsDBNull(reader.GetOrdinal("calle")) ? null : reader.GetString(reader.GetOrdinal("calle"));
                    var numero = reader.IsDBNull(reader.GetOrdinal("numero")) ? null : reader.GetString(reader.GetOrdinal("numero"));
                    var ciudad = reader.IsDBNull(reader.GetOrdinal("ciudad")) ? null : reader.GetString(reader.GetOrdinal("ciudad"));
                    var estadoProv = reader.IsDBNull(reader.GetOrdinal("estado_provincia")) ? null : reader.GetString(reader.GetOrdinal("estado_provincia"));
                    var codigoPostal = reader.IsDBNull(reader.GetOrdinal("codigo_postal")) ? null : reader.GetString(reader.GetOrdinal("codigo_postal"));
                    var pais = reader.IsDBNull(reader.GetOrdinal("pais")) ? null : reader.GetString(reader.GetOrdinal("pais"));

                    if (bloqueado)
                    {
                        return LoginResult.Bloqueado;
                    }

                    var hasher = new Hasher();
                    var computedHashString = hasher.GenerarHash(password, salt ?? string.Empty);
                    var computedHash = Encoding.UTF8.GetBytes(computedHashString);

                    var hashesMatch = dbHash != null && dbHash.Length == computedHash.Length && dbHash.SequenceEqual(computedHash);

                    if (!hashesMatch)
                    {
                        // close reader before executing update
                        reader.Close();

                        const string sqlUpdateAttempts = @"
                    UPDATE Usuario 
                    SET intentos_fallidos = intentos_fallidos + 1, 
                        bloqueado = CASE WHEN intentos_fallidos + 1 >= 3 THEN 1 ELSE 0 END 
                    WHERE usuario_id = @id";

                        using (var cmdUpdate = new SqlCommand(sqlUpdateAttempts, conexion))
                        {
                            cmdUpdate.Parameters.AddWithValue("@id", usuarioId);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        return LoginResult.ContrasenaIncorrecta;
                    }

                    if (!esAdmin)
                    {
                        return LoginResult.NoAdmin;
                    }

                    // Success: reset attempts and update last login
                    reader.Close();

                    const string sqlReset = @"
                UPDATE Usuario 
                SET intentos_fallidos = 0, fecha_ultimo_login = @fecha 
                WHERE usuario_id = @id";

                    using (var cmdReset = new SqlCommand(sqlReset, conexion))
                    {
                        cmdReset.Parameters.AddWithValue("@fecha", DateTime.Now);
                        cmdReset.Parameters.AddWithValue("@id", usuarioId);
                        cmdReset.ExecuteNonQuery();
                    }

                    // build Usuario object and set additional properties
                    usuario = new Usuario(usuarioId, dbUsername, dbHash, salt, nombre, email)
                    {
                        Telefono = telefono,
                        Calle = calle,
                        Numero = numero,
                        Ciudad = ciudad,
                        EstadoProvincia = estadoProv,
                        CodigoPostal = codigoPostal,
                        Pais = pais,
                        IntentosFallidos = 0,
                        EstadoCuenta = !bloqueado,
                        Admin = esAdmin,
                        UltimoLogin = DateTime.Now
                    };

                    // set session
                    ControladorSession.ObtenerInstancia().SetUsuarioActual(usuario);

                    return LoginResult.Success;
                }
            }
        }
    }
}