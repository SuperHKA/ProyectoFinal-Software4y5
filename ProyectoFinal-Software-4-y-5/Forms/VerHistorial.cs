using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinal_Software_4_y_5.Forms
{
    public partial class VerHistorial : Form
    {
        private CultureInfo culturaMoneda = new CultureInfo("es-PA");

        public VerHistorial()
        {
            InitializeComponent();

            // CONFIGURAR EVENTO DEL BOTÓN CERRAR
            btnCerrar.Click += BtnCerrar_Click;

            // CARGAR DATOS AL ABRIR EL FORMULARIO
            CargarHistorial();
        }

        // MÉTODO: Cargar historial de transacciones

        private void CargarHistorial()
        {
            // Limpiar filas existentes
            dgvHistorialTransacciones.Rows.Clear();

            var transacciones = new List<Transaccion>
            {
                // DEPÓSITOS 
                new Transaccion
                {
                    Fecha = new DateTime(2024, 12, 1, 10, 30, 0),
                    Monto = 2000.00m,
                    Descripcion = "Depósito en efectivo"
                },
                new Transaccion
                {
                    Fecha = new DateTime(2024, 12, 10, 9, 45, 0),
                    Monto = 3500.00m,
                    Descripcion = "Transferencia recibida"
                },
                
                // RETIROS 
                new Transaccion
                {
                    Fecha = new DateTime(2024, 12, 5, 14, 15, 0),
                    Monto = -500.00m,
                    Descripcion = "Retiro ATM"
                },
                new Transaccion
                {
                    Fecha = new DateTime(2024, 12, 15, 16, 20, 0),
                    Monto = -1200.00m,
                    Descripcion = "Pago de servicios"
                }
            };

            // Agregar cada transacción al DataGridView
            foreach (var trans in transacciones.OrderByDescending(t => t.Fecha))
            {
                int index = dgvHistorialTransacciones.Rows.Add();
                DataGridViewRow fila = dgvHistorialTransacciones.Rows[index];

                // Llenar las columnas
                fila.Cells["Fecha"].Value = trans.Fecha.ToString("dd/MM/yyyy HH:mm");
                fila.Cells["Monto"].Value = trans.Monto.ToString("C2", culturaMoneda);
                fila.Cells["Descripcion"].Value = trans.Descripcion;

                // Aplicar color según si es depósito (positivo) o retiro (negativo)
                if (trans.Monto >= 0)
                {
                    fila.Cells["Monto"].Style.ForeColor = Color.Green;
                }
                else
                {
                    fila.Cells["Monto"].Style.ForeColor = Color.Red;
                }
            }

            // Configurar apariencia del DataGridView
            ConfigurarDataGridView();
        }

        // MÉTODO: Configurar apariencia del DataGridView
        private void ConfigurarDataGridView()
        {
            // Ajustar columnas
            dgvHistorialTransacciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Deshabilitar edición
            dgvHistorialTransacciones.ReadOnly = true;

            // Selección de fila completa
            dgvHistorialTransacciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Estilo alternado de filas
            dgvHistorialTransacciones.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
        }

        // EVENTO: Cerrar ventana y volver a DetalleDeCuenta
        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // EVENTO: Click en celda del DataGridView
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar que no se hizo clic en el encabezado
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvHistorialTransacciones.Rows[e.RowIndex];

                string fecha = fila.Cells["Fecha"].Value.ToString();
                string monto = fila.Cells["Monto"].Value.ToString();
                string descripcion = fila.Cells["Descripcion"].Value.ToString();

                // Mostrar detalles de la transacción
                MessageBox.Show(
                    $"Detalles de la transacción:\n\n" +
                    $"Fecha: {fecha}\n" +
                    $"Monto: {monto}\n" +
                    $"Descripción: {descripcion}",
                    "Detalle de Transacción",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }

    // CLASE AUXILIAR: Transaccion
    public class Transaccion
    {
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
    }
}