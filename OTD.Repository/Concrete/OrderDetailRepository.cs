using Microsoft.EntityFrameworkCore;
using OTD.Core.Entities;
using OTD.Repository.Abstract;

namespace OTD.Repository.Concrete
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
