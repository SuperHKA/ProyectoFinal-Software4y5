using System.Collections.Generic;

namespace ProyectoFinal_Software_4_y_5.Entidades.DTOs
{
    /// <summary>
    /// Data Transfer Object para respuestas paginadas de la API
    /// </summary>
    /// <typeparam name="T">Tipo de datos contenidos en la página</typeparam>
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> items { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}