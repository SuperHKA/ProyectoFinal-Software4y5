using System;
using System.Windows.Forms;
using ProyectoFinal_Software_4_y_5.Forms;
using ProyectoFinal_Software_4_y_5.Servicios;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Diagnóstico rápido: muestra la ruta del config y la fuente detectada
        try
        {
            // Llamar al static ctor de ConexionBD para forzar inicialización
            var temp = ConexionBD.ObtenerInstancia();
            // Llamada de diagnóstico ya muestra Source y ConfigFile por MessageBox
            ConexionBD.DiagnosticarConexion();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error inicial: {ex.Message}", "Diagnóstico inicial", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        Application.Run(new PanelAdministrativo());
    }
}