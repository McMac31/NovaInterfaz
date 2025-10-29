using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InterfazNova
{
    /// <summary>
    /// Lógica de interacción para PedidosPendientes.xaml
    /// </summary>
    public partial class PedidosPendientes : Window
    {
        private static readonly HttpClient llamada = new HttpClient();
        private List<PedidoPendienteDto> listaCompletaPendientes = new List<PedidoPendienteDto>();
        public PedidosPendientes()
        {
            InitializeComponent();
            Loaded += async (s, e) => await CargarPendientesAsync();
        }

        public class pedPndts
        {
            [JsonPropertyName("Num pedidos")]
            public int pendientes { get; set; }

            [JsonPropertyName("Pedidos Pendientes")]
            public List<PedidoPendienteDto> PedidosPendientes { get; set; }
        }
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
        private async Task CargarPendientesAsync()
        {
            string url = "https://apitechsolutions.duckdns.org/api/ventas/pendientes";
            try
            {
                var dto = await llamada.GetFromJsonAsync<pedPndts>(url);
                if (dto != null && dto.PedidosPendientes != null)
                {
                    listaCompletaPendientes = dto.PedidosPendientes;
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
                    TablaClientes.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pedidos pendientes: " + ex.Message);
            }
        }

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

    }
}
