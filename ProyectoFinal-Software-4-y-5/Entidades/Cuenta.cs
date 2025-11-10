using System;

namespace ProyectoFinal_Software_4_y_5.Entidades
{
    public class Cuenta
    {
        public enum TipoCuenta
        {
            AHORRO,
            CORRIENTE,
            CREDITO,
            DEBITO
        }

        public int IdCuenta { get; set; }
        public int IdUsuario { get; set; }
        public TipoCuenta Tipo { get; set; }
        public string NumCuenta { get; set; }
        public double Saldo { get; set; }
        public DateTime FechaAcceso { get; set; }
        public bool Activo { get; set; }
        public string NombreCliente { get; set; }

        public Cuenta(int idCuenta, int idUsuario, TipoCuenta tipo, string numCuenta, double saldo)
        {
            IdCuenta = idCuenta;
            IdUsuario = idUsuario;
            Tipo = tipo;
            NumCuenta = numCuenta;
            Saldo = saldo;
            FechaAcceso = DateTime.Now;
            Activo = true;
        }

        public override string ToString()
        {
            return $"{NumCuenta} - {Tipo} (${Saldo:F2})";
        }
    }
}