using System;

namespace ProyectoFinal_Software_4_y_5.Entidades
{
    public class Transaccion
    {
        public enum TipoTransaccion
        {
            TRANSFER_INTERNO,
            TRANSFER_EXTERNO,
            DEPOSITO,
            RETIRO
        }

        public enum EstadoTransaccion
        {
            COMPLETADO,
            RECHAZADO,
            PENDIENTE
        }

        public int IdTransaccion { get; set; }
        public TipoTransaccion Tipo { get; set; }
        public EstadoTransaccion Estado { get; set; }
        public double Monto { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int IdUsuario { get; set; }
        public int? IdCuentaOrigen { get; set; }
        public int? IdCuentaDestino { get; set; }

        public Transaccion(TipoTransaccion tipo, double monto, int idUsuario, int? idCuentaOrigen, int? idCuentaDestino)
        {
            Tipo = tipo;
            Monto = monto;
            IdUsuario = idUsuario;
            IdCuentaOrigen = idCuentaOrigen;
            IdCuentaDestino = idCuentaDestino;
            Fecha = DateTime.Now;
            Estado = EstadoTransaccion.PENDIENTE;
        }
    }
}