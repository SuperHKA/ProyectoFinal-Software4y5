        // DTOs
using System;
using System.Collections.Generic;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    // Clase de Cuenta Bancaria
    public class CuentaBancaria
    {
        public string NumeroCuenta { get; set; }
        public string Titular { get; set; }
        public string TipoCuenta { get; set; }
        public decimal SaldoActual { get; set; }
        public string Estado { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? UltimoMovimiento { get; set; }

        // Resumen mensual
        public decimal DepositosMes { get; set; }
        public decimal RetirosMes { get; set; }
        public decimal SaldoNeto { get; set; }
    }
}