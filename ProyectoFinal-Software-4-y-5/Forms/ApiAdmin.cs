using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    /*
     Cliente HTTP que consume el API del banco.

     Breve descripción:
     - Encapsula llamadas HTTP al API REST del banco.
     - Provee métodos para obtener datos, autenticar usuarios y ejecutar acciones remotas.
     - Diseñado para uso por la capa de UI WinForms (adapter ligero).
    */
    public class ApiAdmin
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        // allowInvalidSsl sólo para desarrollo/local (no usar en producción)
        public ApiAdmin(string baseUrl, bool allowInvalidSsl = false)
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

        // FIX: Corregido para manejar la respuesta directa del API
        public async Task<IEnumerable<ClienteDto>> ObtenerClientesAsync()
        {
            try
            {
                // El API devuelve directamente una lista sin paginación cuando no se especifican parámetros
                var rawData = await GetJsonAsync<IEnumerable<Dictionary<string, object>>>("/api/banco/clientes").ConfigureAwait(false);
                
                if (rawData == null)
                    return new List<ClienteDto>();

                // DEBUG: Logging detallado para ver qué datos llegan
                var rawList = rawData.ToList();
                System.Diagnostics.Debug.WriteLine($"DEBUG ApiAdmin: Recibidos {rawList.Count} registros del API");
                
                if (rawList.Count > 0)
                {
                    var firstItem = rawList.First();
                    System.Diagnostics.Debug.WriteLine("DEBUG ApiAdmin: Estructura del primer registro:");
                    foreach (var kvp in firstItem)
                    {
                        var valueType = kvp.Value?.GetType()?.Name ?? "null";
                        System.Diagnostics.Debug.WriteLine($"  {kvp.Key}: {kvp.Value} (Type: {valueType})");
                    }
                }

                // Convertir de Dictionary<string, object> a ClienteDto con mapeo mejorado
                return rawList.Select(item => new ClienteDto
                {
                    // Mapear IDs con múltiples posibilidades
                    usuario_id = GetIntValue(item, "usuario_id") 
                               ?? GetIntValue(item, "UsuarioId") 
                               ?? GetIntValue(item, "usuarioId")
                               ?? 0,
                    
                    cliente_id = GetIntValue(item, "cliente_id") 
                               ?? GetIntValue(item, "ClienteId") 
                               ?? GetIntValue(item, "clienteId"),
                    
                    // Mapear strings con múltiples posibilidades
                    username = GetStringValue(item, "username") 
                             ?? GetStringValue(item, "Username") 
                             ?? GetStringValue(item, "nombreUsuario")
                             ?? "",
                    
                    nombre_completo = GetStringValue(item, "nombre_completo") 
                                    ?? GetStringValue(item, "nombre") 
                                    ?? GetStringValue(item, "Nombre")
                                    ?? GetStringValue(item, "NombreCompleto")
                                    ?? "",
                    
                    email = GetStringValue(item, "email") 
                          ?? GetStringValue(item, "Email") 
                          ?? "",
                    
                    telefono = GetStringValue(item, "telefono") 
                             ?? GetStringValue(item, "Telefono") 
                             ?? "",
                    
                    // Mapear booleans con múltiples posibilidades
                    es_admin = GetBoolValue(item, "es_admin") 
                             ?? GetBoolValue(item, "esAdmin") 
                             ?? GetBoolValue(item, "IsAdmin")
                             ?? GetBoolValue(item, "Admin")
                             ?? false,
                    
                    bloqueado = GetBoolValue(item, "bloqueado") 
                              ?? GetBoolValue(item, "Bloqueado") 
                              ?? GetBoolValue(item, "EstadoCuenta")
                              ?? false
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerClientesAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<ClienteDto>();
            }
        }

        // Métodos helper para conversión segura de tipos con logging
        private static int? GetIntValue(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value) && value != null && value != DBNull.Value)
            {
                if (value is int intValue)
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: GetIntValue({key}) = {intValue} (int)");
                    return intValue;
                }
                if (int.TryParse(value.ToString(), out var parsed))
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: GetIntValue({key}) = {parsed} (parsed from {value})");
                    return parsed;
                }
                System.Diagnostics.Debug.WriteLine($"DEBUG: GetIntValue({key}) failed to parse '{value}' (type: {value.GetType().Name})");
            }
            return null;
        }

        private static string GetStringValue(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value) && value != null && value != DBNull.Value)
            {
                var result = value.ToString();
                System.Diagnostics.Debug.WriteLine($"DEBUG: GetStringValue({key}) = '{result}'");
                return result;
            }
            return null;
        }

        private static bool? GetBoolValue(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value) && value != null && value != DBNull.Value)
            {
                if (value is bool boolValue)
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: GetBoolValue({key}) = {boolValue} (bool)");
                    return boolValue;
                }
                if (bool.TryParse(value.ToString(), out var parsed))
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: GetBoolValue({key}) = {parsed} (parsed from {value})");
                    return parsed;
                }
                // Handle numeric boolean values (0 = false, 1 = true)
                if (int.TryParse(value.ToString(), out var intValue))
                {
                    var result = intValue != 0;
                    System.Diagnostics.Debug.WriteLine($"DEBUG: GetBoolValue({key}) = {result} (from int {intValue})");
                    return result;
                }
                System.Diagnostics.Debug.WriteLine($"DEBUG: GetBoolValue({key}) failed to parse '{value}' (type: {value.GetType().Name})");
            }
            return null;
        }

        public async Task<IEnumerable<TransaccionDto>> ObtenerTransaccionesRecientesAsync(int limit = 20)
        {
            try
            {
                var response = await GetJsonAsync<PaginatedResponse<TransaccionDto>>($"/api/banco/transacciones-recientes?pageSize={limit}").ConfigureAwait(false);
                return response?.items ?? new List<TransaccionDto>();
            }
            catch
            {
                // Fallback: si falla la deserialización paginada, intentar como lista simple
                try
                {
                    var rawData = await GetJsonAsync<IEnumerable<Dictionary<string, object>>>($"/api/banco/transacciones-recientes?pageSize={limit}").ConfigureAwait(false);
                    return rawData?.Select(item => new TransaccionDto
                    {
                        transaccion_id = GetIntValue(item, "transaccion_id") ?? 0,
                        fecha = DateTime.TryParse(GetStringValue(item, "fecha"), out var fechaParsed) ? fechaParsed : DateTime.MinValue,
                        tipo_transaccion = GetStringValue(item, "tipo_transaccion") ?? "",
                        estado = GetStringValue(item, "estado") ?? "",
                        monto = decimal.TryParse(GetStringValue(item, "monto"), out var montoParsed) ? montoParsed : 0,
                        descripcion = GetStringValue(item, "descripcion") ?? ""
                    }) ?? new List<TransaccionDto>();
                }
                catch
                {
                    return new List<TransaccionDto>();
                }
            }
        }

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

        // Obtener todas las transacciones (para filtrar localmente)
        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerTransaccionesAsync()
        {
            try
            {
                // Intentar obtener con paginación grande para obtener todas las transacciones
                var response = await GetJsonAsync<PaginatedResponse<Dictionary<string, object>>>("/api/banco/transacciones?pageSize=1000").ConfigureAwait(false);
                return response?.items ?? new List<Dictionary<string, object>>();
            }
            catch
            {
                // Fallback: intentar endpoint sin paginación
                try
                {
                    return await GetJsonAsync<IEnumerable<Dictionary<string, object>>>("/api/banco/transacciones").ConfigureAwait(false);
                }
                catch
                {
                    return new List<Dictionary<string, object>>();
                }
            }
        }

        // DTOs para respuesta paginada
        public class PaginatedResponse<T>
        {
            public IEnumerable<T> items { get; set; }
            public int total { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        // DTOs - FIX: Agregados valores por defecto para evitar problemas de deserialización
        public class ClienteDto
        {
            public int usuario_id { get; set; } = 0; // Default value
            public int? cliente_id { get; set; }
            public string username { get; set; } = ""; // Default empty string
            public string nombre_completo { get; set; } = ""; // Default empty string
            public string email { get; set; } = ""; // Default empty string
            public string telefono { get; set; } = ""; // Default empty string
            public bool es_admin { get; set; } = false; // Default value
            public bool bloqueado { get; set; } = false; // Default value
        }

        public class TransaccionDto
        {
            public int transaccion_id { get; set; } = 0; // Default value
            public DateTime fecha { get; set; } = DateTime.MinValue; // Default value
            public string tipo_transaccion { get; set; } = ""; // Default empty string
            public string estado { get; set; } = ""; // Default empty string
            public decimal monto { get; set; } = 0; // Default value
            public string descripcion { get; set; } = ""; // Default empty string
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