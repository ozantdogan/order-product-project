using System.Linq.Expressions;

namespace OTD.Repository.Abstract
{
    public interface IBaseRepository<T> where T : class
    {
        Task<bool> Add(T entity);
        Task<bool> AddRange(IEnumerable<T> entities);
        Task<bool> Delete(T entity);
        Task<bool> DeleteRange(IEnumerable<T> entities);
        Task<T?> Get(Expression<Func<T, bool>> condition);
        Task<List<T>> List(Expression<Func<T, bool>>? condition = null);
        Task<bool> Update(T entity);
    }
}
