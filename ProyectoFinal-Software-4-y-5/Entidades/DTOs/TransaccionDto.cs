using System;

namespace ProyectoFinal_Software_4_y_5.Entidades.DTOs
{
    /// <summary>
    /// Data Transfer Object para información de transacciones
    /// </summary>
    public class TransaccionDto
    {
        public int transaccion_id { get; set; } = 0;
        public DateTime fecha { get; set; } = DateTime.MinValue;
        public string tipo_transaccion { get; set; } = "";
        public string estado { get; set; } = "";
        public decimal monto { get; set; } = 0;
        public string descripcion { get; set; } = "";
    }
}