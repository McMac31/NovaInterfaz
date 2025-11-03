using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
//Imports para manejar la llamada a la API y manipular JSON
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
//Uso de las diferentes paginas 
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
        //Llamada a la API
        private static readonly HttpClient llamada = new HttpClient();
        //Enlace general para conectar la api
        private const string conexApiUrl = "https://apitechsolutions.duckdns.org/api/";
        public MainWindow()
        {
            InitializeComponent();
            // Aqui hacemos la llamada asyncrona para cargar cada metodo que hace la llamada a ala API y muestra diferentes parametros pedidos
            this.Loaded += async (s, e) => await cargarNumVentas();
            this.Loaded += async (s, e) => await cargarTotalVentas();
            this.Loaded += async (s, e) => await cargarpedPendientes();
            this.Loaded += async (s, e) => await cargarNumStockBajo();
            this.Loaded += async (s, e) => await cargarVentasCompleto();
        }
        //Clase que usaremos para capturar cada parametro del JSON
        public class VentaApi
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Nombre { get; set; } = "";

            [JsonPropertyName("date_order")]
            public string Fecha { get; set; } = "";

            [JsonPropertyName("partner_id")]
            //Uso de JSONElement para evitar errores 
            public JsonElement Cliente { get; set; }  

            [JsonPropertyName("amount_total")]
            public decimal Total { get; set; }

            [JsonPropertyName("state")]
            public string Estado { get; set; } = "";
        }
        //Clase para capturar el listado de ventas
        public class listadoVenta
        {
            [JsonPropertyName("ventas")]
            public List<VentaApi> Ventas { get; set; }= new List<VentaApi>();
        }
        //Tarea para cargar el numero de ventas
        public class numVentasObj
        {
            [JsonPropertyName("num_ventas")]
            public int Numvnts { get; set; }
        }
        public async Task cargarNumVentas()
        {  //Endpoint para sumar al enlace general
            string endpoint = "ventas";
            string url = conexApiUrl+endpoint;
            try //Control de errores
            {
                //Captura de los datos del json
                var dto = await llamada.GetFromJsonAsync<numVentasObj>(url);
                if (dto != null) //Controlamos que haya datos
                {
                    TxtNumVentas.Text = dto.Numvnts.ToString(); //Formateo y paso los datos a la parte visual
                }
                else
                {
                    TxtNumVentas.Text = "0"; //Si no hay datos muestro 0 por defecto
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener num ventas consulte con el admin: " + err.Message);  //Capturamos el error e indicamos pasos a seguir
            }
        }
        //Tarea para cargar numero total de ventas
        public async Task cargarTotalVentas()
        {
            //Endopoint para sumar al enlace general
            string endpoint = "ventas/total";
            string url = conexApiUrl + endpoint;
            try//Control de errores
            { //Captura de los datos del json
                var dto = await llamada.GetFromJsonAsync<numTotal>(url);
                if (dto != null) //Controlamos que haya datos
                {
                    TxtNumTotal.Content = dto.totalfact.ToString("C"); //Doble Formateo y paso los datos a la parte visual
                }
                else
                {
                    TxtNumTotal.Content = "0"; //Si no hay datos muestro 0 por defecto
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener Total ventas consulte con el admin: " + err.Message);  //Capturamos el error e indicamos pasos a seguir
            }
        }

        //Tarea para cargar numero pedidos pendientes
        public async Task cargarpedPendientes()
        {  //Endopoint para sumar al enlace general
            string endpoint = "ventas/pendientes";
            string url = conexApiUrl + endpoint;
            try //Control de errores
            { //Captura de los datos del json
                var dto = await llamada.GetFromJsonAsync<pedPndts>(url);
                if (dto != null) //Controlamos que haya datos
                {
                    TxtPedidospndts.Text = dto.pendientes.ToString(); //Formateo y paso los datos a la parte visual
                }
                else
                { 
                    TxtPedidospndts.Text = "0"; //Si no hay datos muestro 0 por defecto
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener Pendientes consulte con el admin: " + err.Message); //Capturamos el error e indicamos pasos a seguir
            }
        }
        //Tarea para cargar el numero de productos bajos de stock
        public async Task cargarNumStockBajo()
        {//Endopoint para sumar al enlace general
            string endpoint = "ventas/stockbajo";
            string url = conexApiUrl + endpoint;
            try //Control de errores
            { 
              //Captura de los datos del json
                var dto = await llamada.GetFromJsonAsync<List<prodBajoStock>>(url);
                int cantidad = dto.Count; //Hacemos un contador de objetos
                if (cantidad != 0) //control de que haya almenos 1 
                {
                    txtStockBajo.Text = cantidad.ToString(); //Formateo y visualizacion
                }
                else
                {
                    txtStockBajo.Text = "0"; //Establecer 0
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener Productos con stock bajo consulte con el admin: " + err.Message); //Capturamos el error e indicamos pasos a seguir
            }
        }


        private List<VentaApi> listaCompletaVentas = new List<VentaApi>(); //Listado para los pedidos
        //Tarea para Mostrar los datos en el DataGrid
        public async Task cargarVentasCompleto()
        {//Endopoint para sumar al enlace general
            string endpoint = "ventas/detalles";
            string url = conexApiUrl+endpoint;

            try //Control de errores
            {
                var dto = await llamada.GetFromJsonAsync<listadoVenta>(url); //Capturamos los datos 

                if (dto != null && dto.Ventas != null) //Controlamos que haya datos 
                {
                    listaCompletaVentas = dto.Ventas;//Metemos todo dentro de la lista

                    var listaPedidos = listaCompletaVentas.Select(v => new //Funcion lambda para ir metiendo cada dato en cada columna
                    {
                        Pedido = v.Nombre,
                        Fecha = v.Fecha,
                        Cliente = (v.Cliente.ValueKind == JsonValueKind.Array && v.Cliente.GetArrayLength() > 1) ? v.Cliente[1].GetString() : "", //Formateo y verificacion de datos para controlar el JSONElement
                        Total = v.Total,
                        Estado = v.Estado
                    }).ToList(); //Funcion para volver lista
                    TablaClientes.ItemsSource = listaPedidos; // Establecemos de donde sacara los datos la tabala
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al Cargar detalles de los pedidos: " + err.Message); //Capturamos el error e indicamos pasos a seguir
            }
        }


        private void AbrirCuadroVentas(object sender, MouseButtonEventArgs e) //Abrir Pagina de ventas
        {
            NumVentasMes numVentas = new NumVentasMes(); //Pagina 
            numVentas.ShowDialog(); //Mostrar pagina y bloquear accion hasta que se cierre
        }

        private void AbrirTotal(object sender, MouseButtonEventArgs e)  //Abrir Pagina de ventas
        {
            TotalFacturado totalFacturado = new TotalFacturado(); //Pagina 
            totalFacturado.ShowDialog(); //Mostrar pagina y bloquear accion hasta que se cierre
        }

        private void AbrirPedidosPen(object sender, MouseButtonEventArgs e) //Abrir Pagina de pedidos pendientes
        {
            PedidosPendientes pedidosPendientes = new PedidosPendientes(); //Pagina 
            pedidosPendientes.ShowDialog();//Mostrar pagina y bloquear accion hasta que se cierre
        }

        private void AbrirStockBajo(object sender, MouseEventArgs e) //Abrir pagina de Bajo Stock
        {
            ProductosBajoStock productosBajoStock = new ProductosBajoStock(); //Pagina
            productosBajoStock.ShowDialog(); //Mostrar pagina y bloquear accion hasta que se cierre
        }

        private void AbrirClientDestac_Click(object sender, RoutedEventArgs e) //Abrir pagina de clientes destacados
        {
            ventanaClientes ventanaClientes = new ventanaClientes(); //Pagina
            ventanaClientes.ShowDialog(); //Mostrar pagina y bloquear accion hasta que se cierre
        }

        private void FiltrarPorFecha_Click(object sender, RoutedEventArgs e) //Filtro para manipular el datagrid
        {
            if (FechaInicio.SelectedDate == null || FechaFin.SelectedDate == null) //Control de que se han elegido fechas
            {
                MessageBox.Show("Seleccione ambas fechas."); // Mensaje 
                return;
            }

            DateTime fechaIni = FechaInicio.SelectedDate.Value.Date; //Metemos dentro de una variable los datos introducidos
            DateTime fechaFin = FechaFin.SelectedDate.Value.Date;

            var listaFiltrada = listaCompletaVentas.Where(v => // Funcion lambda para ir añadiendo los datos a la lista creada
            {
                if (DateTime.TryParse(v.Fecha, out DateTime fechaVenta)) // Control y formateo de que los datos esten en formato DATETIME
                {
                    return fechaVenta.Date >= fechaIni && fechaVenta.Date <= fechaFin;  //Filtro de fecha 
                }
                return false;
            })
            .Select(v => new // funcion lambda
            {
                Pedido = v.Nombre, //Buscamos cada dato que cumpla el filtro de fecha
                Fecha = v.Fecha,
                Cliente = (v.Cliente.ValueKind == JsonValueKind.Array && v.Cliente.GetArrayLength() > 1) ? v.Cliente[1].GetString() : "",
                Total = v.Total,
                Estado = v.Estado
            })
            .ToList(); // Conversion a lista
            TablaClientes.ItemsSource = listaFiltrada;  //Cambiando de donde tomara los datos el DATAGRID
        }
        private void LimpiarFiltro_Click(object sender, RoutedEventArgs e) //Funcion para reestablecer los datos
        {
            FechaInicio.SelectedDate = null; // Limpia de los campos de fecha
            FechaFin.SelectedDate = null;
            var listaOriginal = listaCompletaVentas.Select(v => new // Funcion lamba para cada uno de los datos y columnas
            {
                Pedido = v.Nombre,
                Fecha = v.Fecha,
                Cliente = (v.Cliente.ValueKind == JsonValueKind.Array && v.Cliente.GetArrayLength() > 1)? v.Cliente[1].GetString(): "",
                Total = v.Total,
                Estado = v.Estado
            }).ToList(); //Conversion a lista
            TablaClientes.ItemsSource = listaOriginal; // Indicamos de donde sacara los datos el DATAGRID
        }



    }
}

