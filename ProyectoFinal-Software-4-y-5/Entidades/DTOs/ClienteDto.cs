using System;

namespace ProyectoFinal_Software_4_y_5.Entidades.DTOs
{
    /// <summary>
    /// Data Transfer Object para información de cliente
    /// </summary>
    public class ClienteDto
    {
        public int usuario_id { get; set; } = 0;
        public int? cliente_id { get; set; }
        public string username { get; set; } = "";
        public string nombre_completo { get; set; } = "";
        public string email { get; set; } = "";
        public string telefono { get; set; } = "";
        public bool es_admin { get; set; } = false;
        public bool bloqueado { get; set; } = false;
    }
}