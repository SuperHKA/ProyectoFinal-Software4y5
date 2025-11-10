using System;
using System.Data.SqlClient;

namespace ProyectoFinal_Software_4_y_5.Servicios
{
    public class ConexionBD
    {
        private static readonly string ConnectionString = "Server=localhost;Database=BD_BancoDinamo;User Id=sa;Password=TheOne9016;TrustServerCertificate=true;";
        private static ConexionBD _instancia;
        private SqlConnection _conexion;

        private ConexionBD()
        {
            _conexion = new SqlConnection(ConnectionString);
        }

        public static ConexionBD ObtenerInstancia()
        {
            if (_instancia == null)
            {
                _instancia = new ConexionBD();
            }
            return _instancia;
        }

        public SqlConnection GetConexion()
        {
            if (_conexion.State != System.Data.ConnectionState.Open)
            {
                _conexion.Open();
            }
            return _conexion;
        }

        public static void CerrarConexion()
        {
            if (_instancia?._conexion != null)
            {
                _instancia._conexion.Close();
                _instancia._conexion.Dispose();
                _instancia = null;
            }
        }
    }
}