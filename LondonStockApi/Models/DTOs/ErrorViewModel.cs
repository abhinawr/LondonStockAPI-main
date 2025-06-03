namespace LondonStockApi.Models.DTOs
{
    public class ErrorViewModel
    {
        public string Message { get; set; }
        public string? Details { get; set; } 
        public ErrorViewModel(string message, string? details = null)
        {
            Message = message;
            Details = details;
        }
    }
}