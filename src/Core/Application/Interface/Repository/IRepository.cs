using System.Linq.Expressions;

namespace Core.Application.Interface;

public interface IRepository<T> where T : class
{
    public Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    public Task<T> GetByIdAsync(Guid id);
    public Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    public Task<bool> CreateAsync(T entity);
    public Task<bool> UpdateAsync(T entity);
    public Task DeleteAsync(Guid id);
}
