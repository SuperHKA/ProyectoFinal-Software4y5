using System;

namespace ProyectoFinal_Software_4_y_5.Entidades
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public byte[] ContraHash { get; set; }
        public string Salt { get; set; }
        public int IntentosFallidos { get; set; }
        public bool EstadoCuenta { get; set; }
        public DateTime UltimoLogin { get; set; }
        public bool Admin { get; set; }

        // Info cliente
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Ciudad { get; set; }
        public string EstadoProvincia { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }

        public Usuario(int idUsuario, string nombreUsuario, byte[] contraHash, string salt, string nombreCompleto, string email)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            ContraHash = contraHash;
            Salt = salt;
            NombreCompleto = nombreCompleto;
            Email = email;
            IntentosFallidos = 0;
            EstadoCuenta = false;
        }

        public string GetDireccionCompleta()
        {
            return $"{Calle} {Numero}, {Ciudad}, {EstadoProvincia}, {CodigoPostal}, {Pais}";
        }

        public override string ToString()
        {
            return $"{NombreUsuario} - {NombreCompleto}";
        }
    }
}