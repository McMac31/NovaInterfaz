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
        private static readonly HttpClient llamada = new HttpClient();
        private const int ObjetivoPorDefecto = 100;

        public NumVentasMes()
        {
            InitializeComponent();
            this.Loaded += async (s, e) => await CargarYActualizarAsync();
        }
        public class numVentasObj
        {
            [JsonPropertyName("num_ventas")]
            public int Numvnts { get; set; }
        }
        public async Task cargarNumVentas()
        {
            string url = "https://apitechsolutions.duckdns.org/api/ventas";
            try
            {
                var dto = await llamada.GetFromJsonAsync<numVentasObj>(url);
                if (dto != null)
                {
                    numVentas.Text = dto.Numvnts.ToString();
                    TxtActual.Text = dto.Numvnts.ToString();
                    int objetivo = LeerObjetivoSeguro();
                    barraVentas.Maximum = objetivo > 0 ? objetivo : ObjetivoPorDefecto;

                    double valorBarra = Math.Min(dto.Numvnts, barraVentas.Maximum);
                    barraVentas.Value = valorBarra;

                    double porcentaje = 0;
                    if (barraVentas.Maximum > 0)
                        porcentaje = (dto.Numvnts / (double)barraVentas.Maximum) * 100.0;
                    double porcentajeVisual = Math.Min(100.0, Math.Round(porcentaje, 1));
                    TxtPorcentaje.Text = $"{porcentajeVisual}%";
                    TxtObjetivo.Text = (barraVentas.Maximum).ToString();
                }
                else
                {
                    numVentas.Text = "0";
                    TxtActual.Text = "0";
                    barraVentas.Value = 0;
                    TxtPorcentaje.Text = "0%";
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show("Error de red al obtener número de ventas: " + httpEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                MessageBox.Show("Error al procesar la respuesta de la API: " + jsonEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task CargarYActualizarAsync()
        {
     
            if (string.IsNullOrWhiteSpace(TxtObjetivo.Text))
                TxtObjetivo.Text = ObjetivoPorDefecto.ToString();

            await cargarNumVentas();
        }

        private int LeerObjetivoSeguro()
        {
            if (int.TryParse(TxtObjetivo.Text, out int objetivo) && objetivo > 0)
                return objetivo;

            return ObjetivoPorDefecto;
        }
    }
}
