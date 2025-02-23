using OTD.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace OTD.Core.Entities
{
    public class Product : BaseEntity
    {
        [Key]
        public Guid ProductId { get; set; }

        public string? Description { get; set; }
        public ProductStatus Status { get; set; }
        public ProductCategory Category { get; set; }
        public Unit Unit { get; set; }
        public double UnitPrice { get; set; }
    }
}
