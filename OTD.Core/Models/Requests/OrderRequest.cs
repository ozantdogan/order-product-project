using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTD.Core.Models.Requests
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerGSM { get; set; } = string.Empty;

        [Required] 
        public List<ProductDetail> ProductDetails { get; set; }
    }

    [NotMapped]
    public class ProductDetail
    {
        public Guid ProductId { get; set; }
        public double UnitPrice { get; set; }
        public int Amount { get; set; }
    }
}
