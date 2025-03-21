using MovieDatabaseAPI.Core.Entities;

namespace MovieDatabaseAPI.Core.Interfaces.Repositories;

public interface IActorRepository : IRepository<Actor>
{
    Task<IEnumerable<Actor>> SearchActorsAsync(string searchTerm);
}