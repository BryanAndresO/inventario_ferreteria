namespace inventario_ferreteria.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalArticulos { get; set; }
        public int ArticulosConStockBajo { get; set; }
        public decimal ValorTotalInventario { get; set; }
    }
}
