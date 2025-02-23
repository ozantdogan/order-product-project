using OTD.Core.Enums;

namespace OTD.Core.Models.Responses
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public ProductCategory Category { get; set; }
        public Unit Unit { get; set; }
        public double UnitPrice { get; set; }
    }
}
