using System.Collections.Generic;
using System.Threading.Tasks;

namespace API_BancoFisclase.Servicios
{
    public interface IServicioBanco
    {
        Task<LoginResultDto> AuthenticateAdminAsync(string username, string password);
        Task<IEnumerable<IDictionary<string, object>>> ObtenerEstadisticasGeneralesAsync();
        Task<IEnumerable<IDictionary<string, object>>> ObtenerMovimientosDelDiaAsync();
        Task<bool> EjecutarResumenesDiariosAsync();
    }
}
