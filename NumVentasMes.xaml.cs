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
    public partial class NumVentasMes : Window
    {
        // Cliente HTTP para realizar llamadas a la API
        private static readonly HttpClient llamada = new HttpClient();

        // Valor por defecto si no se define un objetivo
        private const int ObjetivoPorDefecto = 100;

        public NumVentasMes()
        {
            InitializeComponent();

            // Al cargar la ventana llamamos a la función que obtiene los datos y actualiza la pagina
            this.Loaded += async (s, e) => await CargarYActualizarAsync();
        }

        // Asegura que el token esté en los headers
        private void EnsureAuthHeader()
        {
            if (!string.IsNullOrEmpty(MainWindow.jwtToken) && llamada.DefaultRequestHeaders.Authorization == null)
            {
                llamada.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", MainWindow.jwtToken);
            }
        }

        // Clase para mapear el JSON que devuelve el número de ventas
        public class numVentasObj
        {
            [JsonPropertyName("num_ventas")]
            public int Numvnts { get; set; }
        }

        // Función que obtiene el número de ventas y actualiza la pagina
        public async Task cargarNumVentas()
        {
            string url = "https://apitechsolutions.duckdns.org/api/ventas";

            try
            {
                EnsureAuthHeader();
                var dto = await llamada.GetFromJsonAsync<numVentasObj>(url);

                if (dto != null)
                {
                    // Actualizamos los elementos visuales con los datos recibidos
                    numVentas.Text = dto.Numvnts.ToString();
                    TxtActual.Text = dto.Numvnts.ToString();

                    // Leemos el objetivo de la pagina o usamos el valor por defecto
                    int objetivo = LeerObjetivoSeguro();
                    barraVentas.Maximum = objetivo > 0 ? objetivo : ObjetivoPorDefecto;

                    // Valor de la barra (no pasa del máximo)
                    double valorBarra = Math.Min(dto.Numvnts, barraVentas.Maximum);
                    barraVentas.Value = valorBarra;

                    // Calculamos porcentaje visual
                    double porcentaje = 0;
                    if (barraVentas.Maximum > 0)
                        porcentaje = (dto.Numvnts / (double)barraVentas.Maximum) * 100.0;

                    double porcentajeVisual = Math.Min(100.0, Math.Round(porcentaje, 1));
                    TxtPorcentaje.Text = $"{porcentajeVisual}%";
                    TxtObjetivo.Text = barraVentas.Maximum.ToString();
                }
                else
                {
                    // Valores por defecto si no hay datos
                    numVentas.Text = "0";
                    TxtActual.Text = "0";
                    barraVentas.Value = 0;
                    TxtPorcentaje.Text = "0%";
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Error de red
                MessageBox.Show("Error de red al obtener número de ventas: " + httpEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                // Error de lectura del JSON
                MessageBox.Show("Error al procesar la respuesta de la API: " + jsonEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Cualquier otro error
                MessageBox.Show("Error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Función que asegura que el objetivo tenga un valor válido
        private async Task CargarYActualizarAsync()
        {
            if (string.IsNullOrWhiteSpace(TxtObjetivo.Text))
                TxtObjetivo.Text = ObjetivoPorDefecto.ToString();

            await cargarNumVentas();
        }

        // Lee el objetivo desde la pagina y devuelve un valor seguro
        private int LeerObjetivoSeguro()
        {
            if (int.TryParse(TxtObjetivo.Text, out int objetivo) && objetivo > 0)
                return objetivo;

            return ObjetivoPorDefecto;
        }
    }
}