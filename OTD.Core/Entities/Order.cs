using System.ComponentModel.DataAnnotations;

namespace OTD.Core.Entities
{
    public class Order : BaseEntity
    {
        [Key]
        public Guid OrderId { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerGSM { get; set; } = string.Empty;
        public int TotalAmount { get; set; }
        public virtual IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
