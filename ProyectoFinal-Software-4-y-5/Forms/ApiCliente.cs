using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    /*
     Cliente HTTP que consume el API del banco.

     Breve descripción:
     - Encapsula llamadas HTTP al API REST del banco.
     - Provee métodos para obtener datos, autenticar usuarios y ejecutar acciones remotas.
     - Diseñado para uso por la capa de UI WinForms (adapter ligero).
    */
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

        // Método público para POST con JSON (uso general desde la UI)
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

        // Implementado PutJsonAsync para uso en ServicioBancoApi
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

        // Autenticar administrador
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

        // Autenticar usuario general (clientes)
        public async Task<LoginResultDto> AutenticarUsuarioAsync(string username, string password)
        {
            var req = new { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(req);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var res = await _http.PostAsync("/api/banco/login", content).ConfigureAwait(false))
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

        // Health check
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

        // Endpoints tipados que devuelven colecciones DTO
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
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>(
                $"/api/banco/reporte?desde={Uri.EscapeDataString(desde.ToString("o"))}&hasta={Uri.EscapeDataString(hasta.ToString("o"))}").ConfigureAwait(false);

        // Nuevo: obtener cuentas por cliente
        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerCuentasPorClienteAsync(int clienteId)
            => await GetJsonAsync<IEnumerable<Dictionary<string, object>>>(
                $"/api/banco/clientes/{clienteId}/cuentas").ConfigureAwait(false);

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

    // DTOs deben coincidir con las formas devueltas por la API
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
        // ClienteId: the customer id associated to this user (if any)
        public int? ClienteId { get; set; }
        // IsAdmin flag reflected from DB
        public bool IsAdmin { get; set; }
    }
}