using System;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoFinal_Software_4_y_5.Servicios
{
    public class Hasher
    {
        public string GenerarHash(string contrasena, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // Match SQL Server NVARCHAR hashing (UTF-16 / little-endian)
                var contrasenaConSalt = contrasena + salt;
                var bytes = Encoding.Unicode.GetBytes(contrasenaConSalt);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerificarContrasena(string contrasenaIngresada, string salt, string hashAlmacenado)
        {
            var hashCalculado = GenerarHash(contrasenaIngresada, salt);
            return hashCalculado == hashAlmacenado;
        }

        public string GenerarSalt()
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
    }
}