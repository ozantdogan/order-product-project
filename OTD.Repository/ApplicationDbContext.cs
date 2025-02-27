using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace OTD.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply soft delete query filter automatically
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                var deleteFlagProperty = clrType.GetProperty("DeleteFlag");

                if (deleteFlagProperty != null && deleteFlagProperty.PropertyType == typeof(bool))
                {
                    var parameter = Expression.Parameter(clrType, "e");
                    var filter = Expression.Lambda(
                        Expression.Not(Expression.Property(parameter, deleteFlagProperty)),
                        parameter
                    );

                    modelBuilder.Entity(clrType).HasQueryFilter(filter);
                }
            }
        }
    }
}
