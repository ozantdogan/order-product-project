using Microsoft.EntityFrameworkCore;
using OTD.Core.Entities;

namespace OTD.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.DeleteFlag);
            modelBuilder.Entity<Order>().HasQueryFilter(o => !o.DeleteFlag);
            modelBuilder.Entity<OrderDetail>().HasQueryFilter(od => !od.DeleteFlag);
        }
    }
}
