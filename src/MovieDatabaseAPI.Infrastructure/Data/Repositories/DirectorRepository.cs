using Microsoft.EntityFrameworkCore;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Infrastructure.Data.Context;

namespace MovieDatabaseAPI.Infrastructure.Data.Repositories;

public class DirectorRepository : Repository<Director>, IDirectorRepository
{
    public DirectorRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Director>> SearchDirectorsAsync(string searchTerm)
    {
        return await _dbSet
            .Where(d => d.Name.Contains(searchTerm))
            .ToListAsync();
    }
}