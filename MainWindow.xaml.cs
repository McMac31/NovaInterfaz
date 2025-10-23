using System.Windows;
using System.Windows.Input;

namespace InterfazNova
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}

