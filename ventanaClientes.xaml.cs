using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace InterfazNova
{
    /// <summary>
    /// </summary>
    public partial class ventanaClientes : Window
    {
        // Cliente HTTP para la API
        private static readonly HttpClient llamada = new HttpClient();

        public ventanaClientes()
        {
            InitializeComponent();

            // Al cargar la ventana se obtienen los clientes destacados
            Loaded += async (s, e) => await CargarClientesDestacadosAsync();
        }

        // Clase para mapear un cliente individual
        public class ClienteDto
        {
            [JsonPropertyName("id")]
            public int ID { get; set; }

            [JsonPropertyName("name")]
            public string Nombre { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }
        }

        // Clase para mapear la respuesta de la API con lista de clientes y cantidad
        public class ClientesDestacadosResponse
        {
            [JsonPropertyName("Clientes destacados")]
            public List<ClienteDto> Clientes { get; set; }

            [JsonPropertyName("Numero de clientes destacados")]
            public int Numero { get; set; }
        }

        // Función que obtiene los clientes destacados y los muestra en el DataGrid
        private async Task CargarClientesDestacadosAsync()
        {
            string url = "https://apitechsolutions.duckdns.org/api/clientes/destacados";

            try
            {
                var resp = await llamada.GetFromJsonAsync<ClientesDestacadosResponse>(url);

                if (resp != null && resp.Clientes != null)
                {
                    // Asignamos la lista de clientes al DataGrid
                    TablaClientes.ItemsSource = resp.Clientes;
                }
                else
                {
                    // Si no hay datos limpiamos el DataGrid
                    TablaClientes.ItemsSource = null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Error de red
                MessageBox.Show("Error de red al obtener clientes: " + httpEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Otros errores
                MessageBox.Show("Error al cargar clientes destacados: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
