using Microsoft.EntityFrameworkCore;
using OTD.Core.Entities;
using OTD.Repository.Abstract;

namespace OTD.Repository.Concrete
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
