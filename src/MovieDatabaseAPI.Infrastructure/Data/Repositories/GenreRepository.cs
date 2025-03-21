using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Infrastructure.Data.Context;

namespace MovieDatabaseAPI.Infrastructure.Data.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    public GenreRepository(AppDbContext context) : base(context)
    {
    }
}