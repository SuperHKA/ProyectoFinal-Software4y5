namespace API_BancoFisclase.DTOs
{
    /// <summary>
    /// Data Transfer Object para solicitudes de transferencia
    /// </summary>
    public class TransferRequestDto
    {
        /// <summary>
        /// Número de cuenta o ID de cuenta origen
        /// </summary>
        public string CuentaOrigen { get; set; }
        
        /// <summary>
        /// Número de cuenta destino (para transferencias internas) o ID de cliente (para transferencias externas)
        /// </summary>
        public string CuentaDestino { get; set; }
        
        /// <summary>
        /// Monto a transferir (debe ser mayor a 0)
        /// </summary>
        public decimal Monto { get; set; }
        
        /// <summary>
        /// Descripción opcional de la transferencia
        /// </summary>
        public string Descripcion { get; set; }
        
        /// <summary>
        /// ID del usuario que realiza la transferencia
        /// </summary>
        public int? UsuarioId { get; set; }
        
        /// <summary>
        /// Nombre de usuario que realiza la transferencia (alternativa a UsuarioId)
        /// </summary>
        public string Username { get; set; }
    }
}