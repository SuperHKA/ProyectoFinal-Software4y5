using ProyectoFinal_Software_4_y_5.Entidades;
using ProyectoFinal_Software_4_y_5.Servicios;
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
    }
}