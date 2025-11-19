using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public class ApiCliente
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        // allowInvalidSsl sólo para desarrollo/local (no usar en producción)
        public ApiCliente(string baseUrl, bool allowInvalidSsl = false)
        {
            _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));

            // Forzar TLS 1.2 (compatibilidad en .NET Framework)
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            if (allowInvalidSsl)
            {
                // Sólo para desarrollo local: aceptar certificados autofirmados
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            }

            _http = new HttpClient { BaseAddress = new Uri(_baseUrl) };
            _http.Timeout = TimeSpan.FromSeconds(30);
        }

        private async Task<T> GetJsonAsync<T>(string url)
        {
            using (var res = await _http.GetAsync(url).ConfigureAwait(false))
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                res.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<T>(body);
            }
        }

        // Made public so callers in other classes/assemblies can use it
        public async Task<TResponse> PostJsonAsync<TRequest, TResponse>(string url, TRequest payload)
        {
            var json = JsonConvert.SerializeObject(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var res = await _http.PostAsync(url, content).ConfigureAwait(false))
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                res.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
        }

        // Implemented PutJsonAsync to match usage in ServicioBancoApi
        public async Task<TResponse> PutJsonAsync<TRequest, TResponse>(string url, TRequest payload)
        {
            var json = JsonConvert.SerializeObject(payload);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var res = await _http.PutAsync(url, content).ConfigureAwait(false))
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                res.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
        }

        public async Task<LoginResultDto> AutenticarAdministradorAsync(string username, string password)
        {
            var req = new { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(req);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var res = await _http.PostAsync("/api/banco/login-admin", content).ConfigureAwait(false))
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var dto = JsonConvert.DeserializeObject<LoginResultDto>(body);
                    return dto ?? new LoginResultDto { Status = EstatusLogin.Error, Message = "Empty response" };
                }
                catch
                {
                    return new LoginResultDto { Status = EstatusLogin.Error, Message = $"HTTP {(int)res.StatusCode}: {body}" };
                }
            }
        }

        // Health check method
        public async Task<bool> HealthAsync()
        {
            try
            {
                using (var res = await _http.GetAsync("/api/banco/health").ConfigureAwait(false))
                {
                    return res.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerEstadisticasAsync()
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>("/api/banco/estadisticas").ConfigureAwait(false);

        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerMovimientosDelDiaAsync()
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>("/api/banco/movimientos-del-dia").ConfigureAwait(false);

        public async Task<bool> EjecutarResumenesAsync()
        {
            using (var res = await _http.PostAsync("/api/banco/ejecutar-resumenes", null).ConfigureAwait(false))
            {
                return res.IsSuccessStatusCode;
            }
        }

        // Typed endpoints returning DTO collections
        public async Task<IEnumerable<ClienteDto>> ObtenerClientesAsync()
            => await GetJsonAsync<IEnumerable<ClienteDto>>("/api/banco/clientes").ConfigureAwait(false);

        public async Task<IEnumerable<TransaccionDto>> ObtenerTransaccionesRecientesAsync(int limit = 20)
            => await GetJsonAsync<IEnumerable<TransaccionDto>>($"/api/banco/transacciones-recientes?limit={limit}").ConfigureAwait(false);

        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerAnalisisAsync()
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>("/api/banco/analisis").ConfigureAwait(false);

        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerResumenClientesAsync()
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>("/api/banco/resumen-clientes").ConfigureAwait(false);

        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerUsuariosBloqueadosAsync()
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>("/api/banco/usuarios-bloqueados").ConfigureAwait(false);

        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerNotificacionesSistemaAsync()
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>("/api/banco/notificaciones-sistema").ConfigureAwait(false);

        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerReporteTransaccionesPorFechaAsync(DateTime desde, DateTime hasta)
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>($"/api/banco/reporte?desde={Uri.EscapeDataString(desde.ToString("o"))}&hasta={Uri.EscapeDataString(hasta.ToString("o"))}").ConfigureAwait(false);

        // DTOs
        public class ClienteDto
        {
            public int usuario_id { get; set; }
            public int? cliente_id { get; set; }
            public string username { get; set; }
            public string nombre_completo { get; set; }
            public string email { get; set; }
            public string telefono { get; set; }
            public bool es_admin { get; set; }
            public bool bloqueado { get; set; }
        }

        public class TransaccionDto
        {
            public int transaccion_id { get; set; }
            public DateTime fecha { get; set; }
            public string tipo_transaccion { get; set; }
            public string estado { get; set; }
            public decimal monto { get; set; }
            public string descripcion { get; set; }
        }
    }

    // DTOs must match API DTO shapes
    public enum EstatusLogin { Success, NotFound, Locked, BadCredentials, Forbidden, Error }

    public class LoginResultDto
    {
        public EstatusLogin Status { get; set; }
        public string Message { get; set; }
        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }
}