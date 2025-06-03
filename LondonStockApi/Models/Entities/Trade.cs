using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LondonStockApi.Models.Entities
{
    public class Trade
    {
        [Key]
        public Guid TradeId { get; set; }

        [Required]
        [StringLength(10)]
        public string TickerSymbol { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Price must be positive.")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Shares must be positive.")]
        public decimal Shares { get; set; }

        [Required]
        [StringLength(50)]
        public string BrokerId { get; set; } = string.Empty; // Could be linked to JWT claims

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}