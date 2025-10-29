using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace InterfazNova
{
    /// <summary>
    /// </summary>
    public partial class ProductosBajoStock : Window
    {
        private static readonly HttpClient llamada = new HttpClient();

        public ProductosBajoStock()
        {
            InitializeComponent();
            Loaded += async (s, e) => await CargarProductosStockBajoAsync();
        }
        public class prodBajoStock
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Nombre { get; set; }

            [JsonPropertyName("qty_available")]
            public decimal cantidad { get; set; }
        }
        private class ProdGridItem
        {
            public int ID { get; set; }
            public string Nombre { get; set; }
            public decimal Stock { get; set; }
            public string NivelStock{ get; set; } 
        }

        private async Task CargarProductosStockBajoAsync()
        {
            string url = "https://apitechsolutions.duckdns.org/api/ventas/stockbajo";

            try
            {
                var dto = await llamada.GetFromJsonAsync<List<prodBajoStock>>(url);

                if (dto == null)
                {
                    TablaClientes.ItemsSource = null;
                    return;
                }
                var listaParaGrid = dto.Select(p => new ProdGridItem
                {
                    ID = p.Id,
                    Nombre = p.Nombre,
                    Stock = p.cantidad,
                    NivelStock = CalcularStockLevel(p.cantidad)
                }).ToList();

                TablaClientes.ItemsSource = listaParaGrid;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Error de red al obtener productos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos con stock bajo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string CalcularStockLevel(decimal qty)
        {
            if (qty <= 1m) return "Low";      
            if (qty <= 3m) return "Medium";    
            return "High";                    
        }
    }
}
