using API_BancoFisclase.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API_BancoFisclase.Controllers
{
    [ApiController]
    [Route("api/banco")]
    public class ControladorBanco : ControllerBase
    {
        private readonly IServicioBanco _banco;

        public ControladorBanco(IServicioBanco banco)
        {
            _banco = banco;
        }

        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrEmpty(req.Password))
                return BadRequest("username and password required");

            var res = await _banco.AuthenticateAdminAsync(req.Username.Trim(), req.Password).ConfigureAwait(false);

            switch (res.Status)
            {
                case EstatusLogin.Success:
                    return Ok(res);
                case EstatusLogin.NotFound:
                    return NotFound(res);
                case EstatusLogin.Locked:
                    return StatusCode(423, res); // 423 Locked
                case EstatusLogin.BadCredentials:
                    return Unauthorized(res);
                case EstatusLogin.Forbidden:
                    return Forbid();
                default:
                    return StatusCode(500, res);
            }
        }

        [HttpGet("estadisticas")]
        public async Task<IActionResult> Estadisticas()
        {
            var data = await _banco.ObtenerEstadisticasGeneralesAsync().ConfigureAwait(false);
            return Ok(data);
        }

        [HttpGet("movimientos-del-dia")]
        public async Task<IActionResult> MovimientosDelDia()
        {
            var data = await _banco.ObtenerMovimientosDelDiaAsync().ConfigureAwait(false);
            return Ok(data);
        }

        [HttpPost("ejecutar-resumenes")]
        public async Task<IActionResult> EjecutarResumenes()
        {
            var ok = await _banco.EjecutarResumenesDiariosAsync().ConfigureAwait(false);
            return ok ? NoContent() : StatusCode(500, "Error ejecutando resúmenes");
        }
    }
}
