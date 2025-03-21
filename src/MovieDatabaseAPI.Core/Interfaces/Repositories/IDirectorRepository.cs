using MovieDatabaseAPI.Core.Entities;

namespace MovieDatabaseAPI.Core.Interfaces.Repositories;

public interface IDirectorRepository : IRepository<Director>
{
    Task<IEnumerable<Director>> SearchDirectorsAsync(string searchTerm);
}