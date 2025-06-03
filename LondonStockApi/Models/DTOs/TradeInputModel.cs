using System.ComponentModel.DataAnnotations;

namespace LondonStockApi.Models.DTOs
{
    public class TradeInputModel
    {
        [Required(ErrorMessage = "Ticker symbol is required.")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Ticker symbol must be between 1 and 10 characters.")]
        public string TickerSymbol { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Number of shares is required.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Number of shares must be a positive value.")]
        public decimal Shares { get; set; }

        [Required(ErrorMessage = "Broker ID is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Broker ID must be between 1 and 50 characters.")]
        public string BrokerId { get; set; } = string.Empty;
    }
}