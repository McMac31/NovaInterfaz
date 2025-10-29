using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
    /// Lógica de interacción para NumVentasMes.xaml
    /// </summary>
    public partial class NumVentasMes : Window
    {
        private static readonly HttpClient llamada = new HttpClient();
        public NumVentasMes()
        {
            InitializeComponent();
            this.Loaded += async (s, e) => await cargarNumVentas();
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
                    barraVentas.Value=dto.Numvnts;
                }
                else
                {
                    numVentas.Text = "0";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener num ventas consulte con el admin: " + err.Message);
            }
        }


    }
}
