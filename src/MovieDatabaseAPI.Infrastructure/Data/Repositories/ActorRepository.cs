using Microsoft.EntityFrameworkCore;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Infrastructure.Data.Context;

namespace MovieDatabaseAPI.Infrastructure.Data.Repositories;

public class ActorRepository : Repository<Actor>, IActorRepository
{
    public ActorRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Actor>> SearchActorsAsync(string searchTerm)
    {
        return await _dbSet
            .Where(a => a.Name.Contains(searchTerm))
            .ToListAsync();
    }
}