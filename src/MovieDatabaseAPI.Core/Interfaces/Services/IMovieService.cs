using MovieDatabaseAPI.Core.DTOs;

namespace MovieDatabaseAPI.Core.Interfaces.Services;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
    Task<MovieDto> GetMovieByIdAsync(int id);
    Task<MovieDto> CreateMovieAsync(CreateMovieDto createMovieDto);
    Task UpdateMovieAsync(int id, UpdateMovieDto updateMovieDto);
    Task DeleteMovieAsync(int id);
    Task<IEnumerable<MovieDto>> GetMoviesByDirectorAsync(int directorId);
    Task<IEnumerable<MovieDto>> GetMoviesByGenreAsync(int genreId);
    Task<IEnumerable<MovieDto>> GetMoviesByActorAsync(int actorId);
    Task<IEnumerable<MovieDto>> SearchMoviesAsync(string searchTerm);
}