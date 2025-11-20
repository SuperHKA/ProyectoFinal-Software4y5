using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_BancoFisclase.Servicios
{
    public interface IServicioBanco
    {
        // Authentication
        Task<LoginResultDto> AuthenticateAdminAsync(string username, string password);

        // Read-only queries
        Task<IEnumerable<IDictionary<string, object>>> ObtenerEstadisticasGeneralesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerMovimientosDelDiaAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerClientesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerTransaccionesRecientesAsync(int limit = 20);
        Task<IEnumerable<IDictionary<string, object>>> ObtenerAnalisisTransaccionesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerResumenClientesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerUsuariosBloqueadosAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerNotificacionesSistemaAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerReporteTransaccionesPorFechaAsync(DateTime desde, DateTime hasta);

        // Diagnostics
        Task<IEnumerable<IDictionary<string, object>>> DiagnosticarTablasAsync();
        Task<IEnumerable<IDictionary<string, object>>> DiagnosticarVistasAsync();
        Task<string> ProbarConsultaBasicaAsync();

        // Write operations / actions
        Task<bool> EjecutarResumenesDiariosAsync();
        Task<bool> CrearClienteCompletoAsync(object clienteDto, string password);
        Task<bool> ActualizarClienteAsync(int id, object clienteDto);
        Task<bool> CambiarContrasenaAsync(int usuarioId, string nuevaContrasena);
        Task<bool> CrearCuentaBancariaAsync(object cuentaDto);
        Task<bool> BloquearDesbloquearUsuarioAsync(int usuarioId, bool bloquear);
        Task<bool> CrearUsuarioAdministradorAsync(object adminDto);

        // Transfers
        // Perform an internal transfer between two account numbers (both accounts exist in the system)
        Task<(bool Success, string Message, int TransaccionId)> TransferenciaInternaAsync(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto, string descripcion, int usuarioId);

        // Perform an external transfer using origin account number and destination account number (destination may belong to another client)
        Task<(bool Success, string Message, int TransaccionId)> TransferenciaExternaAsync(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto, string descripcion, int usuarioId);
    }
}
