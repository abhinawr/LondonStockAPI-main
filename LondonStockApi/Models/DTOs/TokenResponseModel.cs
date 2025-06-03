namespace LondonStockApi.Models.DTOs
{
    public class TokenResponseModel
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
    }
}