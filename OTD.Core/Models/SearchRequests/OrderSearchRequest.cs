namespace OTD.Core.Models.SearchRequests
{
    public class OrderSearchRequest
    {
        public DateTime? CreatedOnMin { get; set; }
        public DateTime? CreatedOnMax { get; set; }
        public string? CustomerGSM { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public int? TotalAmountMin { get; set; }
        public int? TotalAmountMax { get; set; }
    }
}
