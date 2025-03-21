using MovieDatabaseAPI.Core.Helpers;

namespace MovieDatabaseAPI.Core.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<PagedList<T>> GetAllPagedAsync(PaginationParams paginationParams);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}