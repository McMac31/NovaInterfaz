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
    /// Lógica de interacción para TotalFacturado.xaml
    /// </summary>
    public partial class TotalFacturado : Window
    {
        private static readonly HttpClient llamada = new HttpClient();
        public TotalFacturado()
        {
            InitializeComponent();
            this.Loaded += async (s, e) => await cargarTotalVentas();
        }
        public class numTotal
        {
            [JsonPropertyName("Total Facturado")]
            public decimal totalfact {  get; set; }
        }
        public async Task cargarTotalVentas()
        {
            string url = "https://apitechsolutions.duckdns.org/api/ventas/total";
            try
            {
                var dto = await llamada.GetFromJsonAsync<numTotal>(url);
                if (dto != null)
                {
                    txtFacturado.Content = dto.totalfact.ToString("C");
                }
                else
                {
                    txtFacturado.Content = "0";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error al obtener Total ventas consulte con el admin: " + err.Message);
            }
        }

    }
}
