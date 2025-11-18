using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public class ApiCliente
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public ApiCliente(string baseUrl)
        {
            _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));
            _http = new HttpClient { BaseAddress = new Uri(_baseUrl) };
            _http.Timeout = TimeSpan.FromSeconds(30);
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

        public async Task<IEnumerable<Dictionary<string, object>>> ObtenerEstadisticasAsync()
        {
            using (var res = await _http.GetAsync("/api/banco/estadisticas").ConfigureAwait(false))
            {
                res.EnsureSuccessStatusCode();
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(body);
            }
        }

        public async Task<bool> EjecutarResumenesAsync()
        {
            using (var res = await _http.PostAsync("/api/banco/ejecutar-resumenes", null).ConfigureAwait(false))
            {
                return res.IsSuccessStatusCode;
            }
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