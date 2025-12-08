using ProyectoFinal_Software_4_y_5.Forms;
using ProyectoFinal_Software_4_y_5.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoFinal_Software_4_y_5.Servicios
{
    // Adapter used by the WinForms UI to call the web API and return DataTable / app types
    public class ServicioBancoApi
    {
        private readonly ApiAdmin _api;

        // Default to the HTTP endpoint your API exposes on port 5202 (per console)
        public ServicioBancoApi(string baseUrl = "http://localhost:5202", bool allowInvalidSsl = false)
        {
            _api = new ApiAdmin(baseUrl, allowInvalidSsl);
        }

        public async Task<DataTable> ObtenerEstadisticasGeneralesAsync()
        {
            var items = await _api.ObtenerEstadisticasAsync().ConfigureAwait(false);
            return ToDataTable(items.Select(d => d));
        }

        public async Task<DataTable> ObtenerMovimientosDelDiaAsync()
        {
            var items = await _api.ObtenerMovimientosDelDiaAsync().ConfigureAwait(false);
            return ToDataTable(items);
        }

        public async Task<bool> EjecutarResumenesDiariosAsync()
        {
            return await _api.EjecutarResumenesAsync().ConfigureAwait(false);
        }

        // New wrappers used by the UI (names match original ServicioBanco)
        public async Task<DataTable> ObtenerTransaccionesRecientesAsync(int top = 50)
        {
            var items = await _api.ObtenerTransaccionesRecientesAsync(top).ConfigureAwait(false);
            var list = items.Select(t => new Dictionary<string, object>
            {
                ["transaccion_id"] = t.transaccion_id,
                ["fecha"] = t.fecha,
                ["tipo_transaccion"] = t.tipo_transaccion,
                ["estado"] = t.estado,
                ["monto"] = t.monto,
                ["descripcion"] = t.descripcion
            });
            return ToDataTable(list);
        }

        public async Task<List<Usuario>> ObtenerTodosLosClientesAsync()
        {
            var items = await _api.ObtenerClientesAsync().ConfigureAwait(false);
            var list = new List<Usuario>();
            if (items == null) return list;

            foreach (var dto in items)
            {
                var u = new Usuario(
                    idUsuario: dto.usuario_id,
                    nombreUsuario: dto.username,
                    contraHash: null,
                    salt: null,
                    nombreCompleto: dto.nombre_completo,
                    email: dto.email)
                {
                    Telefono = dto.telefono,
                    Admin = dto.es_admin,
                    EstadoCuenta = dto.bloqueado
                };
                list.Add(u);
            }
            return list;
        }

        public async Task<DataTable> ObtenerAnalisisTransaccionesAsync()
        {
            var items = await _api.ObtenerAnalisisAsync().ConfigureAwait(false);
            return ToDataTable(items);
        }

        public async Task<DataTable> ObtenerResumenClientesAsync()
        {
            var items = await _api.ObtenerResumenClientesAsync().ConfigureAwait(false);
            return ToDataTable(items);
        }

        public async Task<DataTable> ObtenerUsuariosBloqueadosAsync()
        {
            var items = await _api.ObtenerUsuariosBloqueadosAsync().ConfigureAwait(false);
            return ToDataTable(items);
        }

        public async Task<DataTable> ObtenerNotificacionesSistemaAsync()
        {
            var items = await _api.ObtenerNotificacionesSistemaAsync().ConfigureAwait(false);
            return ToDataTable(items);
        }

        public async Task<bool> CrearClienteCompletoAsync(Usuario usuario, string password)
        {
            try
            {
                // Usar el nuevo método de ApiAdmin que maneja mejor los datos
                var clienteData = new
                {
                    IdUsuario = usuario.IdUsuario,
                    NombreUsuario = usuario.NombreUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    Telefono = usuario.Telefono,
                    Calle = usuario.Calle,
                    Numero = usuario.Numero,
                    Ciudad = usuario.Ciudad,
                    EstadoProvincia = usuario.EstadoProvincia,
                    CodigoPostal = usuario.CodigoPostal,
                    Pais = usuario.Pais
                };

                return await _api.CrearClienteAsync(clienteData, password).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR CrearClienteCompletoAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ActualizarClienteAsync(Usuario usuario)
        {
            try
            {
                // Usar el nuevo método de ApiAdmin que maneja mejor los datos
                var clienteData = new
                {
                    IdUsuario = usuario.IdUsuario,
                    NombreUsuario = usuario.NombreUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    Telefono = usuario.Telefono,
                    Calle = usuario.Calle,
                    Numero = usuario.Numero,
                    Ciudad = usuario.Ciudad,
                    EstadoProvincia = usuario.EstadoProvincia,
                    CodigoPostal = usuario.CodigoPostal,
                    Pais = usuario.Pais
                };

                return await _api.ActualizarClienteAsync(clienteData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR ActualizarClienteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CambiarContrasenaAsync(int usuarioId, string nuevaContrasena)
        {
            var res = await _api.PostJsonAsync<object, object>($"/api/banco/usuarios/{usuarioId}/cambiar-contrasena", new { Password = nuevaContrasena }).ConfigureAwait(false);
            return res != null;
        }

        public async Task<bool> CrearCuentaBancariaAsync(int clienteId, Cuenta.TipoCuenta tipo, string numeroCuenta, double saldoInicial)
        {
            var dto = new
            {
                ClienteId = clienteId,
                Tipo = tipo.ToString(),
                NumeroCuenta = numeroCuenta,
                SaldoInicial = saldoInicial
            };
            var res = await _api.PostJsonAsync<object, object>("/api/banco/cuentas", dto).ConfigureAwait(false);
            return res != null;
        }

        public async Task<bool> BloquearDesbloquearUsuarioAsync(int usuarioId, bool bloquear)
        {
            var res = await _api.PostJsonAsync<object, object>($"/api/banco/usuarios/{usuarioId}/bloquear", new { Bloquear = bloquear }).ConfigureAwait(false);
            return res != null;
        }

        public async Task<bool> CrearUsuarioAdministradorAsync(string username, string password, string nombreCompleto, string email)
        {
            var dto = new { Username = username, Password = password, Nombre = nombreCompleto, Email = email };
            var res = await _api.PostJsonAsync<object, object>("/api/banco/usuarios/admin", dto).ConfigureAwait(false);
            return res != null;
        }

        public async Task<DataTable> ObtenerReporteTransaccionesPorFechaAsync(DateTime desde, DateTime hasta)
        {
            var items = await _api.ObtenerReporteTransaccionesPorFechaAsync(desde, hasta).ConfigureAwait(false);
            return ToDataTable(items);
        }

        // Helper: convert a sequence of dictionary->object into a DataTable (columns = union of keys)
        private DataTable ToDataTable(IEnumerable<Dictionary<string, object>> items)
        {
            var dt = new DataTable();
            if (items == null) return dt;

            var list = items.ToList();
            if (list.Count == 0) return dt;

            // Build column set
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in list)
            {
                foreach (var k in row.Keys) columns.Add(k);
            }

            foreach (var col in columns)
            {
                dt.Columns.Add(col);
            }

            // Fill rows
            foreach (var row in list)
            {
                var dr = dt.NewRow();
                foreach (var col in columns)
                {
                    row.TryGetValue(col, out var val);
                    dr[col] = val ?? DBNull.Value;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}