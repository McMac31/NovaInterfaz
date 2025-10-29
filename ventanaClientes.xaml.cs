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
        private static readonly HttpClient llamada = new HttpClient();
        public ventanaClientes()
        {
            InitializeComponent();
            Loaded += async (s, e) => await CargarClientesDestacadosAsync();
        }
        public class ClienteDto
        {
            [JsonPropertyName("id")]
            public int ID { get; set; }
            [JsonPropertyName("name")]
            public string Nombre { get; set; }
            [JsonPropertyName("email")]
            public string Email { get; set; }
        }
        public class ClientesDestacadosResponse
        {
            [JsonPropertyName("Clientes destacados")]
            public List<ClienteDto> Clientes { get; set; }

            [JsonPropertyName("Numero de clientes destacados")]
            public int Numero { get; set; }
        }

        private async Task CargarClientesDestacadosAsync()
        {
            string url = "https://apitechsolutions.duckdns.org/api/clientes/destacados";

            try
            {
                var resp = await llamada.GetFromJsonAsync<ClientesDestacadosResponse>(url);

                if (resp != null && resp.Clientes != null)
                {
                    TablaClientes.ItemsSource = resp.Clientes;
                }
                else
                {
                    TablaClientes.ItemsSource = null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show("Error de red al obtener clientes: " + httpEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes destacados: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
