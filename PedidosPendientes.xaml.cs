using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace InterfazNova
{
    /// <summary>
    /// </summary>
    public partial class PedidosPendientes : Window
    {
        // Cliente HTTP para llamar la API
        private static readonly HttpClient llamada = new HttpClient();

        // Guardamos la lista completa en memoria para poder filtrar y restaurar
        private List<PedidoPendienteDto> listaCompletaPendientes = new List<PedidoPendienteDto>();

        public PedidosPendientes()
        {
            InitializeComponent();

            // Al cargar la ventana obtenemos los pedidos pendientes
            Loaded += async (s, e) => await CargarPendientesAsync();
        }

        // Asegura que el token esté en los headers
        private void EnsureAuthHeader()
        {
            if (!string.IsNullOrEmpty(MainWindow.jwtToken) && llamada.DefaultRequestHeaders.Authorization == null)
            {
                llamada.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", MainWindow.jwtToken);
            }
        }

        // Clase para mapear la respuesta general de la API
        public class pedPndts
        {
            [JsonPropertyName("Num pedidos")]
            public int pendientes { get; set; }

            [JsonPropertyName("Pedidos Pendientes")]
            public List<PedidoPendienteDto> PedidosPendientes { get; set; }
        }

        // Clase para mapear cada pedido pendiente individual
        public class PedidoPendienteDto
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("date_order")]
            public string DateOrder { get; set; }

            [JsonPropertyName("partner_id")]
            public JsonElement PartnerId { get; set; }

            [JsonPropertyName("amount_total")]
            public decimal AmountTotal { get; set; }

            [JsonPropertyName("picking_ids")]
            public JsonElement PickingIds { get; set; }
        }

        // Función para cargar los pedidos pendientes desde la API
        private async Task CargarPendientesAsync()
        {
            string url = "https://apitechsolutions.duckdns.org/api/ventas/pendientes";

            try
            {
                EnsureAuthHeader();
                var dto = await llamada.GetFromJsonAsync<pedPndts>(url);

                if (dto != null && dto.PedidosPendientes != null)
                {
                    // Guardamos la lista completa en memoria
                    listaCompletaPendientes = dto.PedidosPendientes;

                    // Transformamos la lista para que el DataGrid pueda mostrarla
                    var listaParaGrid = listaCompletaPendientes.Select(p => new
                    {
                        Pedido = p.Name,
                        Fecha = p.DateOrder,
                        Cliente = ExtraerNombreCliente(p.PartnerId),
                        Total = p.AmountTotal,
                        Estado = "Pendiente envío"
                    }).ToList();

                    TablaClientes.ItemsSource = listaParaGrid;
                }
                else
                {
                    // Si no hay pedidos, limpiamos el DataGrid
                    TablaClientes.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pedidos pendientes: " + ex.Message);
            }
        }

        // Función que extrae el nombre del cliente desde el JsonElement
        private string ExtraerNombreCliente(JsonElement partnerId)
        {
            try
            {
                if (partnerId.ValueKind == JsonValueKind.Array && partnerId.GetArrayLength() > 1)
                    return partnerId[1].GetString() ?? "";
            }
            catch { }

            return "";
        }

        // Filtra los pedidos por rango de fechas seleccionado
        private void FiltrarPorFecha_Click(object sender, RoutedEventArgs e)
        {
            if (FechaInicio.SelectedDate == null || FechaFin.SelectedDate == null)
            {
                MessageBox.Show("Seleccione ambas fechas.");
                return;
            }

            DateTime fechaIni = FechaInicio.SelectedDate.Value.Date;
            DateTime fechaFin = FechaFin.SelectedDate.Value.Date;

            var filtrados = listaCompletaPendientes
                .Where(p =>
                {
                    if (DateTime.TryParse(p.DateOrder, out DateTime fechaVenta))
                        return fechaVenta.Date >= fechaIni && fechaVenta.Date <= fechaFin;
                    return false;
                })
                .Select(p => new
                {
                    Pedido = p.Name,
                    Fecha = p.DateOrder,
                    Cliente = ExtraerNombreCliente(p.PartnerId),
                    Total = p.AmountTotal,
                    Estado = "Pendiente envío"
                })
                .ToList();

            TablaClientes.ItemsSource = filtrados;
        }

        // Limpia el filtro de fechas y restaura la lista completa
        private void LimpiarFiltro_Click(object sender, RoutedEventArgs e)
        {
            FechaInicio.SelectedDate = null;
            FechaFin.SelectedDate = null;

            var listaOriginal = listaCompletaPendientes.Select(p => new
            {
                Pedido = p.Name,
                Fecha = p.DateOrder,
                Cliente = ExtraerNombreCliente(p.PartnerId),
                Total = p.AmountTotal,
                Estado = "Pendiente envío"
            }).ToList();

            TablaClientes.ItemsSource = listaOriginal;
        }
    }
}