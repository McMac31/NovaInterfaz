using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static InterfazNova.NumVentasMes;
using static InterfazNova.PedidosPendientes;
using static InterfazNova.ProductosBajoStock;
using static InterfazNova.TotalFacturado;

namespace InterfazNova
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient llamada = new HttpClient();
        private const string conexApiUrl = "https://apitechsolutions.duckdns.org/api/";
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += async (s, e) => await cargarNumVentas();
            this.Loaded += async (s, e) => await cargarTotalVentas();
            this.Loaded += async (s, e) => await cargarpedPendientes();
            this.Loaded += async (s, e) => await cargarNumStockBajo();
            this.Loaded += async (s, e) => await cargarVentasCompleto();
        }
        public class VentaApi
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Nombre { get; set; }

            [JsonPropertyName("date_order")]
            public string Fecha { get; set; }

            [JsonPropertyName("partner_id")]
            public JsonElement Cliente { get; set; }  

            [JsonPropertyName("amount_total")]
            public decimal Total { get; set; }

            [JsonPropertyName("state")]
            public string Estado { get; set; }
        }
        public class listadoVenta
        {
            [JsonPropertyName("ventas")]
            public List<VentaApi> Ventas { get; set; }
        }

        public async Task cargarNumVentas()
        {
            string endpoint = "ventas";
            string url = conexApiUrl + endpoint;
            try
            {
                var dto = await llamada.GetFromJsonAsync<numVentasObj>(url);
                if (dto != null)
                {
                    TxtNumVentas.Text = dto.Numvnts.ToString();
                }
                else
                {
                    TxtNumVentas.Text = "0";
                }
            }
            catch(Exception err)
            {
                MessageBox.Show("Error al obtener num ventas consulte con el admin: "+ err.Message); 
            }
        }
        public async Task cargarTotalVentas()
        {
            string endpoint = "ventas/total";
            string url = conexApiUrl + endpoint;
            try
            {
                var dto = await llamada.GetFromJsonAsync<numTotal>(url);
                if (dto != null)
                {
                    TxtNumTotal.Content = dto.totalfact.ToString("C");
                }
                else
                {
                    TxtNumTotal.Content = "0";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener Total ventas consulte con el admin: " + err.Message);
            }
        }

        public async Task cargarpedPendientes()
        {
            string endpoint = "ventas/pendientes";
            string url = conexApiUrl + endpoint;
            try
            {
                var dto = await llamada.GetFromJsonAsync<pedPndts>(url);
                if (dto != null)
                {
                    TxtPedidospndts.Text = dto.pendientes.ToString();
                }
                else
                {
                    TxtPedidospndts.Text = "0";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener Pendientes consulte con el admin: " + err.Message);
            }
        }
        public async Task cargarNumStockBajo()
        {
            string endpoint = "ventas/stockbajo";
            string url = conexApiUrl + endpoint;
            try
            {
                var dto = await llamada.GetFromJsonAsync<List<prodBajoStock>>(url);
                int cantidad = dto.Count;
                if (cantidad != 0)
                {
                    txtStockBajo.Text = cantidad.ToString();
                }
                else
                {
                    txtStockBajo.Text = "0";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener Productos con stock bajo consulte con el admin: " + err.Message);
            }
        }
        private List<VentaApi> listaCompletaVentas = new List<VentaApi>();
        public async Task cargarVentasCompleto()
        {
            string endpoint = "ventas/detalles";
            string url = conexApiUrl + endpoint;

            try
            {
                var dto = await llamada.GetFromJsonAsync<listadoVenta>(url);

                if (dto != null && dto.Ventas != null)
                {
                    listaCompletaVentas = dto.Ventas;

                    var listaPedidos = listaCompletaVentas.Select(v => new 
                    {
                        Pedido = v.Nombre,
                        Fecha = v.Fecha,
                        Cliente = (v.Cliente.ValueKind == JsonValueKind.Array && v.Cliente.GetArrayLength() > 1)? v.Cliente[1].GetString(): "",
                        Total = v.Total,
                        Estado = v.Estado
                    }).ToList();
                    TablaClientes.ItemsSource = listaPedidos;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al Cargar detalles de los pedidos: " + err.Message);
            }
        }


        private void AbrirCuadroVentas(object sender, MouseButtonEventArgs e)
        {
            NumVentasMes numVentas = new NumVentasMes();
            numVentas.ShowDialog();
        }

        private void AbrirTotal(object sender, MouseButtonEventArgs e)
        {
            TotalFacturado totalFacturado = new TotalFacturado();
            totalFacturado.ShowDialog();
        }

        private void AbrirPedidosPen(object sender, MouseButtonEventArgs e)
        {
            PedidosPendientes pedidosPendientes = new PedidosPendientes();
            pedidosPendientes.ShowDialog();
        }

        private void AbrirStockBajo(object sender, MouseEventArgs e)
        {
            ProductosBajoStock productosBajoStock = new ProductosBajoStock();
            productosBajoStock.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ventanaClientes ventanaClientes = new ventanaClientes();
            ventanaClientes.ShowDialog();
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

            var listaFiltrada = listaCompletaVentas.Where(v =>
            {
                if (DateTime.TryParse(v.Fecha, out DateTime fechaVenta))
                {
                    return fechaVenta.Date >= fechaIni && fechaVenta.Date <= fechaFin;
                }
                return false;
            })
            .Select(v => new
            {
                Pedido = v.Nombre,
                Fecha = v.Fecha,
                Cliente = (v.Cliente.ValueKind == JsonValueKind.Array && v.Cliente.GetArrayLength() > 1) ? v.Cliente[1].GetString() : "",
                Total = v.Total,
                Estado = v.Estado
            })
            .ToList();
            TablaClientes.ItemsSource = listaFiltrada;
        }
        private void LimpiarFiltro_Click(object sender, RoutedEventArgs e)
        {
            FechaInicio.SelectedDate = null;
            FechaFin.SelectedDate = null;
            var listaOriginal = listaCompletaVentas.Select(v => new
            {
                Pedido = v.Nombre,
                Fecha = v.Fecha,
                Cliente = (v.Cliente.ValueKind == JsonValueKind.Array && v.Cliente.GetArrayLength() > 1)? v.Cliente[1].GetString(): "",
                Total = v.Total,
                Estado = v.Estado
            }).ToList();
            TablaClientes.ItemsSource = listaOriginal;
        }



    }
}

