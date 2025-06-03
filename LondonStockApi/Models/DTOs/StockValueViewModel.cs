namespace LondonStockApi.Models.DTOs
{
    public class StockValueViewModel
    {
        public string TickerSymbol { get; set; } = string.Empty;
        public decimal CurrentValue { get; set; }
    }
}