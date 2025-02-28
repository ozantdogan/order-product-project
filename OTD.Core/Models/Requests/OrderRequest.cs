using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTD.Core.Models.Requests
{
    public class CreateOrderRequest
    {

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
