using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ProyectoFinal_Software_4_y_5.Utilidades
{
    /// <summary>
    /// Utilidad para manejo de procesos y cierre seguro de aplicaciones
    /// </summary>
    public static class ProcessManager
    {
        /// <summary>
        /// Mata todos los procesos de la aplicación actual de forma segura
        /// </summary>
        public static void KillCurrentApplicationProcesses()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                var processName = currentProcess.ProcessName;
                
                // Log para debugging
                System.Diagnostics.Debug.WriteLine($"INFO: Intentando cerrar procesos de '{processName}'");
                
                // Obtener todos los procesos con el mismo nombre
                var processes = Process.GetProcessesByName(processName);
                
                foreach (var process in processes)
                {
                    try
                    {
                        if (process.Id != currentProcess.Id) // No matar el proceso actual desde sí mismo
                        {
                            System.Diagnostics.Debug.WriteLine($"INFO: Cerrando proceso {process.Id}");
                            process.CloseMainWindow();
                            
                            // Dar tiempo para cierre graceful
                            if (!process.WaitForExit(3000))
                            {
                                // Si no se cerró en 3 segundos, forzar cierre
                                System.Diagnostics.Debug.WriteLine($"WARN: Forzando cierre de proceso {process.Id}");
                                process.Kill();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ERROR: No se pudo cerrar proceso {process.Id}: {ex.Message}");
                    }
                    finally
                    {
                        process?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Error general en KillCurrentApplicationProcesses: {ex.Message}");
            }
        }

        /// <summary>
        /// Ejecuta taskkill para terminar procesos por nombre
        /// </summary>
        /// <param name="processName">Nombre del proceso a terminar</param>
        /// <param name="forceKill">Si true, usa /F para forzar el cierre</param>
        public static async Task<bool> TaskKillByNameAsync(string processName, bool forceKill = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(processName))
                    return false;

                var forceFlag = forceKill ? " /F" : "";
                var command = $"taskkill /IM \"{processName}.exe\"{forceFlag}";
                
                System.Diagnostics.Debug.WriteLine($"INFO: Ejecutando comando: {command}");
                
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processStartInfo))
                {
                    var output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                    var error = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
                    
                    await Task.Run(() => process.WaitForExit()).ConfigureAwait(false);

                    System.Diagnostics.Debug.WriteLine($"INFO: TaskKill output: {output}");
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        System.Diagnostics.Debug.WriteLine($"WARN: TaskKill error: {error}");
                    }

                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Error en TaskKillByNameAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Limpia recursos y cierra la aplicación de forma segura
        /// </summary>
        /// <param name="apiAdmin">Instancia de ApiAdmin para limpiar</param>
        /// <param name="exitCode">Código de salida de la aplicación</param>
        public static void SafeApplicationExit(Forms.ApiAdmin apiAdmin = null, int exitCode = 0)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("INFO: Iniciando cierre seguro de aplicación");

                // Limpiar recursos de API
                if (apiAdmin != null)
                {
                    try
                    {
                        apiAdmin.Dispose();
                        System.Diagnostics.Debug.WriteLine("INFO: ApiAdmin disposed correctamente");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"WARN: Error disposing ApiAdmin: {ex.Message}");
                    }
                }

                // Cerrar formularios abiertos de forma segura
                var openForms = System.Windows.Forms.Application.OpenForms;
                for (int i = openForms.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        var form = openForms[i];
                        if (form != null && !form.IsDisposed)
                        {
                            System.Diagnostics.Debug.WriteLine($"INFO: Cerrando formulario: {form.Name}");
                            form.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"WARN: Error cerrando formulario: {ex.Message}");
                    }
                }

                // Dar tiempo para que los formularios se cierren
                Thread.Sleep(1000);

                System.Diagnostics.Debug.WriteLine("INFO: Terminando aplicación");
                System.Windows.Forms.Application.Exit();
                Environment.Exit(exitCode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Error en SafeApplicationExit: {ex.Message}");
                
                // Como último recurso, forzar la salida
                try
                {
                    Environment.Exit(exitCode);
                }
                catch
                {
                    // Si todo falla, usar taskkill
                    var currentProcess = Process.GetCurrentProcess();
                    _ = TaskKillByNameAsync(currentProcess.ProcessName, true);
                }
            }
        }

        /// <summary>
        /// Verifica si hay procesos zombie de la aplicación
        /// </summary>
        /// <param name="processName">Nombre del proceso a verificar</param>
        /// <returns>Número de procesos encontrados</returns>
        public static int CheckZombieProcesses(string processName = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(processName))
                {
                    var currentProcess = Process.GetCurrentProcess();
                    processName = currentProcess.ProcessName;
                }

                var processes = Process.GetProcessesByName(processName);
                var count = processes.Length;
                
                System.Diagnostics.Debug.WriteLine($"INFO: Encontrados {count} procesos de '{processName}'");
                
                // Limpiar referencias
                foreach (var process in processes)
                {
                    process?.Dispose();
                }
                
                return count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Error verificando procesos zombie: {ex.Message}");
                return -1;
            }
        }
    }
}