using ProyectoFinal_Software_4_y_5.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProyectoFinal_Software_4_y_5.Servicios
{
    // Adapter used by the WinForms UI to call the web API and return DataTable objects
    public class ServicioBancoApi
    {
        private readonly ApiCliente _api;

        // Default to the HTTP endpoint your API exposes on port 5202 (per console)
        public ServicioBancoApi(string baseUrl = "http://localhost:5202", bool allowInvalidSsl = false)
        {
            _api = new ApiCliente(baseUrl, allowInvalidSsl);
        }

        public DataTable ObtenerEstadisticasGenerales()
        {
            var items = _api.ObtenerEstadisticasAsync().GetAwaiter().GetResult();
            return ToDataTable(items);
        }

        public DataTable ObtenerMovimientosDelDia()
        {
            var items = _api.ObtenerMovimientosDelDiaAsync().GetAwaiter().GetResult();
            return ToDataTable(items);
        }

        public bool EjecutarResumenesDiarios()
        {
            return _api.EjecutarResumenesAsync().GetAwaiter().GetResult();
        }

        // Helper: convert a sequence of dictionary->object into a DataTable (columns = union of keys)
        private DataTable ToDataTable(IEnumerable<Dictionary<string, object>> items)
        {
            var dt = new DataTable();
            if (items == null) return dt;

            var list = items.ToList();
            if (list.Count == 0) return dt;

            // Build column set
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in list)
            {
                foreach (var k in row.Keys) columns.Add(k);
            }

            foreach (var col in columns)
            {
                dt.Columns.Add(col);
            }

            // Fill rows
            foreach (var row in list)
            {
                var dr = dt.NewRow();
                foreach (var col in columns)
                {
                    row.TryGetValue(col, out var val);
                    dr[col] = val ?? DBNull.Value;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}