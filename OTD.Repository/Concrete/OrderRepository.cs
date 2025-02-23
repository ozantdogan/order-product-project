using Microsoft.EntityFrameworkCore;
using OTD.Core.Entities;
using OTD.Repository.Abstract;

namespace OTD.Repository.Concrete
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
