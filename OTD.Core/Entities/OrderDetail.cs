using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTD.Core.Entities
{
    public class OrderDetail : BaseEntity
    {
        [Key]
        public Guid OrderDetailId { get; set; }

        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public double UnitPrice { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
