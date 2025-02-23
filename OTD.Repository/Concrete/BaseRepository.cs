using Microsoft.EntityFrameworkCore;
using OTD.Repository.Abstract;
using System.Linq.Expressions;

namespace OTD.Repository.Concrete
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<bool> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            return await SaveChangesAsync();
        }

        public async Task<bool> AddRange(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return await SaveChangesAsync();
        }

        public async Task<bool> Delete(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            return await SaveChangesAsync();
        }

        public async Task<bool> DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Deleted;
            }
            return await SaveChangesAsync();
        }

        public async Task<T?> Get(Expression<Func<T, bool>> condition)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(condition);
        }

        public async Task<List<T>> List(Expression<Func<T, bool>>? condition = null)
        {
            return condition == null
                ? await _context.Set<T>().ToListAsync()
                : await _context.Set<T>().Where(condition).ToListAsync();
        }

        public async Task<bool> Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await SaveChangesAsync();
        }

        private async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
