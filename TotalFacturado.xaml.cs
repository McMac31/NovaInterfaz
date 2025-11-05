using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace InterfazNova
{
    /// <summary>
    /// </summary>
    public partial class TotalFacturado : Window
    {
        // Cliente HTTP para llamadas a la API
        private static readonly HttpClient llamada = new HttpClient();

        public TotalFacturado()
        {
            InitializeComponent();

            // Al cargar la ventana se obtiene el total facturado
            this.Loaded += async (s, e) => await cargarTotalVentas();
        }

        // Asegura que el token esté en los headers
        private void EnsureAuthHeader()
        {
            if (!string.IsNullOrEmpty(MainWindow.jwtToken) && llamada.DefaultRequestHeaders.Authorization == null)
            {
                llamada.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", MainWindow.jwtToken);
            }
        }

        // Clase para mapear la respuesta del JSON
        public class numTotal
        {
            [JsonPropertyName("Total Facturado")]
            public decimal totalfact { get; set; }
        }

        // Función que obtiene el total facturado desde la API
        public async Task cargarTotalVentas()
        {
            string url = "https://apitechsolutions.duckdns.org/api/ventas/total";

            try
            {
                EnsureAuthHeader();
                var dto = await llamada.GetFromJsonAsync<numTotal>(url);

                if (dto != null)
                {
                    // Mostramos el total formateado como moneda en la UI
                    txtFacturado.Content = dto.totalfact.ToString("C");
                }
                else
                {
                    txtFacturado.Content = "0"; // Valor por defecto si no hay datos
                }
            }
            catch (Exception err)
            {
                // Captura de errores y aviso al usuario
                MessageBox.Show("Error al obtener Total ventas consulte con el admin: " + err.Message);
            }
        }
    }
}