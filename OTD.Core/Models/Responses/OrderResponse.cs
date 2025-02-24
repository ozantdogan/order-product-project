namespace OTD.Core.Models.Responses
{
    public class OrderResponse
    {
        public DateTime CreatedOn { get; set; }
        public string CustomerName { get; set; }
        public int TotalAmount { get; set; }
    }
}
