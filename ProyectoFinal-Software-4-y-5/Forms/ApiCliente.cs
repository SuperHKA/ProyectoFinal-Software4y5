        // DTOs
        public class ClienteDto
        {
            public int usuario_id { get; set; } = 0; // Default to 0 if null
            public int? cliente_id { get; set; }
            public string username { get; set; } = ""; // Default to empty string if null
            public string nombre_completo { get; set; } = "";
            public string email { get; set; } = "";
            public string telefono { get; set; } = "";
            public bool es_admin { get; set; } = false;
            public bool bloqueado { get; set; } = false;
        }