using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;


namespace ProyectoFinal_Software_4_y_5.Servicios
{
    public class ConexionBD
    {
        private static ConexionBD _instancia;
        private SqlConnection _conexion;

        // Metadata to help diagnosis
        private static readonly string ConnectionString;
        private static readonly string ConnectionStringSource;
        private static readonly string ConfigFilePath;

        // Static ctor to initialize and capture source info
        static ConexionBD()
        {
            ConfigFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            try
            {
                // Log all available connectionStrings for diagnosis (masked)
                Debug.WriteLine($"[ConexionBD] Runtime config file: {ConfigFilePath}");
                Debug.WriteLine("[ConexionBD] Available connectionStrings:");
                foreach (ConnectionStringSettings csSetting in ConfigurationManager.ConnectionStrings)
                {
                    var name = csSetting?.Name ?? "(null)";
                    var masked = MaskPassword(csSetting?.ConnectionString ?? "");
                    Debug.WriteLine($"  - Name: '{name}' => {masked}");
                }

                // Try specific expected names (add here any name you may have used historically)
                var expectedNames = new[] { "BD_BancoFisclase", "BD_BancoFisclase", "BD_BancoFisclase_Local", "DefaultConnection" };

                foreach (var expected in expectedNames)
                {
                    var cs = ConfigurationManager.ConnectionStrings[expected]?.ConnectionString;
                    if (!string.IsNullOrWhiteSpace(cs))
                    {
                        ConnectionString = cs;
                        ConnectionStringSource = $"connectionStrings (named: {expected})";
                        Debug.WriteLine($"[ConexionBD] Loaded connection string from {ConnectionStringSource}");
                        return;
                    }
                }

                // If none of the expected names found, pick the first non-empty connection string available
                foreach (ConnectionStringSettings csSetting in ConfigurationManager.ConnectionStrings)
                {
                    var cs = csSetting?.ConnectionString;
                    if (!string.IsNullOrWhiteSpace(cs))
                    {
                        ConnectionString = cs;
                        ConnectionStringSource = $"connectionStrings (first-found: {csSetting.Name})";
                        Debug.WriteLine($"[ConexionBD] Loaded connection string from {ConnectionStringSource}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ConexionBD] Error reading ConfigurationManager: " + ex.Message);
            }

            // Fallback literal
            ConnectionString = "Server=localhost;Database=BD_BancoFisclase;User Id=sa;Password=TheOne9016;TrustServerCertificate=true;Connection Timeout=30;";
            ConnectionStringSource = "fallback-literal";
            Debug.WriteLine($"[ConexionBD] Using fallback-literal connection string. ConfigFile={ConfigFilePath}");
        }

        private ConexionBD()
        {
            // Ensure the instance's SqlConnection is created with the resolved static ConnectionString.
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
            // If instance connection object is missing create it using the resolved static ConnectionString.
            if (_conexion == null)
            {
                _conexion = new SqlConnection(ConnectionString);
            }

            // Prefer the static resolved ConnectionString for validation.
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                Debug.WriteLine("[ConexionBD] ERROR: Resolved static ConnectionString vacío. Source=" + ConnectionStringSource + " ConfigFile=" + ConfigFilePath);
                throw new InvalidOperationException(
                    "La cadena de conexión no está configurada. Revisa el App.config y el archivo runtime .exe.config (ruta mostrada en Output).");
            }

            // Defensive: if the SqlConnection instance somehow has an empty ConnectionString, set it from the static one.
            if (string.IsNullOrWhiteSpace(_conexion.ConnectionString))
            {
                Debug.WriteLine("[ConexionBD] Warning: instance SqlConnection.ConnectionString vacío, assigning resolved ConnectionString.");
                _conexion.ConnectionString = ConnectionString;
            }

            if (_conexion.State != System.Data.ConnectionState.Open)
            {
                _conexion.Open();
            }

            return _conexion;
        }

        public static void DiagnosticarConexion()
        {
            try
            {
                var masked = MaskPassword(ConnectionString);
                Debug.WriteLine($"[ConexionBD] DiagnosticarConexion using: {masked} (source: {ConnectionStringSource}) ConfigFile={ConfigFilePath}");

                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    MessageBox.Show($"✅ Conexión exitosa a la base de datos\n\nSource: {ConnectionStringSource}\nConfigFile: {ConfigFilePath}",
                        "Diagnóstico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error de conexión:\n{ex.Message}\n\nSource: {ConnectionStringSource}\nConfigFile: {ConfigFilePath}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string MaskPassword(string cs)
        {
            if (string.IsNullOrWhiteSpace(cs)) return cs;
            try
            {
                var lower = cs.ToLowerInvariant();
                var idx = lower.IndexOf("password=", StringComparison.Ordinal);
                if (idx < 0) return cs;
                var semicolon = cs.IndexOf(';', idx);
                if (semicolon > idx)
                {
                    return cs.Substring(0, idx) + "Password=*****" + cs.Substring(semicolon);
                }
                return cs.Substring(0, idx) + "Password=*****";
            }
            catch
            {
                return "********";
            }
        }
    }
}