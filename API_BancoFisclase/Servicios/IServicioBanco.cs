using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_BancoFisclase.Servicios
{
    /*
     Servicio que define las operaciones disponibles del API del banco.

     Descripción:
     - Define métodos para autenticación, consultas de lectura y operaciones de escritura.
     - Incluye métodos para transferencias internas y externas.
     - Sirve como contrato para las implementaciones del servicio de datos.
    */
    public interface IServicioBanco
    {
        // Autenticación
        Task<LoginResultDto> AuthenticateAdminAsync(string username, string password);
        Task<LoginResultDto> AuthenticateUserAsync(string username, string password);
        Task<bool> IsUsernameAdminAsync(string username);
        Task<int?> GetClienteIdByUsernameAsync(string username);
        Task<int?> GetUsuarioIdByUsernameAsync(string username);

        // Consultas de solo lectura
        Task<IEnumerable<IDictionary<string, object>>> ObtenerEstadisticasGeneralesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerMovimientosDelDiaAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerClientesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerTransaccionesRecientesAsync(int limit = 20);
        Task<IEnumerable<IDictionary<string, object>>> ObtenerAnalisisTransaccionesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerResumenClientesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerUsuariosBloqueadosAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerNotificacionesSistemaAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerReporteTransaccionesPorFechaAsync(DateTime desde, DateTime hasta);
        Task<IEnumerable<IDictionary<string, object>>> ObtenerCuentasPorClienteAsync(int clienteId);
        Task<IDictionary<string, object>> ObtenerClientePorIdAsync(int clienteId);
        Task<IEnumerable<IDictionary<string, object>>> ObtenerNotificacionesClienteAsync(int clienteId, int dias = 30);
        Task<(IEnumerable<IDictionary<string, object>> Items, int Total)> ObtenerClientesPaginadoAsync(int page, int pageSize, string search);
        Task<(IEnumerable<IDictionary<string, object>> Items, int Total)> ObtenerTransaccionesPaginadasAsync(int page, int pageSize, DateTime? desde, DateTime? hasta, string q);
        Task<IEnumerable<IDictionary<string, object>>> ObtenerTransaccionesClienteAsync(int clienteId, int limit = 20);

        // Métodos para dropdowns de transferencias
        Task<IEnumerable<IDictionary<string, object>>> ObtenerTodasLasCuentasAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerTodosLosClientesParaTransferenciaAsync();

        // Diagnósticos
        Task<IEnumerable<IDictionary<string, object>>> DiagnosticarTablasAsync();
        Task<IEnumerable<IDictionary<string, object>>> DiagnosticarVistasAsync();
        Task<string> ProbarConsultaBasicaAsync();

        // Operaciones de escritura / acciones
        Task<bool> EjecutarResumenesDiariosAsync();
        Task<bool> CrearClienteCompletoAsync(object clienteDto, string password);
        Task<bool> ActualizarClienteAsync(int id, object clienteDto);
        Task<bool> CambiarContrasenaAsync(int usuarioId, string nuevaContrasena);
        Task<bool> CrearCuentaBancariaAsync(object cuentaDto);
        Task<bool> BloquearDesbloquearUsuarioAsync(int usuarioId, bool bloquear);
        Task<bool> CrearUsuarioAdministradorAsync(object adminDto);

        // Transferencias
        // Realiza una transferencia interna entre dos números de cuenta (ambas cuentas existen en el sistema)
        Task<(bool Success, string Message, int TransaccionId)> TransferenciaInternaAsync(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto, string descripcion, int usuarioId);

        // Realiza una transferencia externa usando número de cuenta origen y número de cuenta destino (el destino puede pertenecer a otro cliente)
        Task<(bool Success, string Message, int TransaccionId)> TransferenciaExternaAsync(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto, string descripcion, int usuarioId);
    }
}
