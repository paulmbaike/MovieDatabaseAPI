using MovieDatabaseAPI.Core.Entities;

namespace MovieDatabaseAPI.Core.Interfaces.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    Task<IEnumerable<Movie>> GetMoviesByDirectorAsync(int directorId);
    Task<IEnumerable<Movie>> GetMoviesByGenreAsync(int genreId);
    Task<IEnumerable<Movie>> GetMoviesByActorAsync(int actorId);
    Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm);
}