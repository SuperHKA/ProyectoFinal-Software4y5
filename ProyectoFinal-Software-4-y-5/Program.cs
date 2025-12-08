using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoFinal_Software_4_y_5.Forms;
using ProyectoFinal_Software_4_y_5.Utilidades;

namespace ProyectoFinal_Software_4_y_5
{
    internal static class Program
    {
        private static Forms.ApiAdmin _globalApiAdmin = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configurar manejo global de excepciones
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // Verificar procesos zombie al inicio
                var zombieCount = ProcessManager.CheckZombieProcesses();
                if (zombieCount > 1)
                {
                    System.Diagnostics.Debug.WriteLine($"WARN: Se encontraron {zombieCount} procesos, limpiando...");
                    ProcessManager.KillCurrentApplicationProcesses();
                    
                    // Esperar un poco después de la limpieza
                    System.Threading.Thread.Sleep(2000);
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Crear instancia global de ApiAdmin (opcional, para cleanup)
                _globalApiAdmin = new Forms.ApiAdmin("http://localhost:5202", allowInvalidSsl: false);

                // Configurar evento de salida de aplicación
                Application.ApplicationExit += Application_ApplicationExit;

                // Ejecutar la aplicación
                Application.Run(new FormLogin());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Error fatal en Main: {ex.Message}");
                MessageBox.Show($"Error fatal al iniciar la aplicación:\n{ex.Message}", 
                    "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Cleanup final
                CleanupAndExit();
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR: Thread exception: {e.Exception.Message}");
            
            var result = MessageBox.Show(
                $"Se produjo un error no controlado:\n{e.Exception.Message}\n\n¿Desea continuar ejecutando la aplicación?",
                "Error de Aplicación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);

            if (result == DialogResult.No)
            {
                CleanupAndExit(1);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"ERROR: Unhandled domain exception: {exception?.Message}");
            
            if (e.IsTerminating)
            {
                MessageBox.Show(
                    $"Error fatal no recuperable:\n{exception?.Message}\n\nLa aplicación se cerrará.",
                    "Error Fatal",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                
                CleanupAndExit(2);
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("INFO: Evento ApplicationExit disparado");
            CleanupAndExit();
        }

        private static void CleanupAndExit(int exitCode = 0)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"INFO: Ejecutando cleanup con código de salida {exitCode}");
                
                // Usar ProcessManager para cierre seguro
                ProcessManager.SafeApplicationExit(_globalApiAdmin, exitCode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Error en cleanup: {ex.Message}");
                
                // Último recurso: taskkill forzado
                _ = ProcessManager.TaskKillByNameAsync("ProyectoFinal-Software-4-y-5", true);
                Environment.Exit(exitCode);
            }
        }

        /// <summary>
        /// Método público para cerrar la aplicación desde cualquier formulario
        /// </summary>
        public static void ExitApplication(int exitCode = 0)
        {
            CleanupAndExit(exitCode);
        }
    }
}
