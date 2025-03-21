using Microsoft.EntityFrameworkCore;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using static MovieDatabaseAPI.Infrastructure.Data.Context.AppContext;

namespace MovieDatabaseAPI.Infrastructure.Data.Repositories;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    public MovieRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Movie>> GetMoviesByDirectorAsync(int directorId)
    {
        return await _dbSet
            .Where(m => m.DirectorId == directorId)
            .Include(m => m.Director)
            .Include(m => m.Genres)
            .Include(m => m.Actors)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(int genreId)
    {
        return await _dbSet
            .Where(m => m.Genres.Any(g => g.Id == genreId))
            .Include(m => m.Director)
            .Include(m => m.Genres)
            .Include(m => m.Actors)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetMoviesByActorAsync(int actorId)
    {
        return await _dbSet
            .Where(m => m.Actors.Any(a => a.Id == actorId))
            .Include(m => m.Director)
            .Include(m => m.Genres)
            .Include(m => m.Actors)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm)
    {
        return await _dbSet
            .Where(m => m.Title.Contains(searchTerm))
            .Include(m => m.Director)
            .Include(m => m.Genres)
            .Include(m => m.Actors)
            .ToListAsync();
    }


    public override async Task<Movie?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(m => m.Director)
            .Include(m => m.Genres)
            .Include(m => m.Actors)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public override async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _dbSet
            .Include(m => m.Director)
            .Include(m => m.Genres)
            .Include(m => m.Actors)
            .ToListAsync();
    }
}