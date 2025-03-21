using MovieDatabaseAPI.Core.DTOs;

namespace MovieDatabaseAPI.Core.Interfaces.Services;

public interface IGenreService
{
    Task<IEnumerable<GenreDto>> GetAllGenresAsync();
    Task<GenreDto> GetGenreByIdAsync(int id);
    Task<GenreDto> CreateGenreAsync(CreateGenreDto createGenreDto);
    Task UpdateGenreAsync(int id, UpdateGenreDto updateGenreDto);
    Task DeleteGenreAsync(int id);
    Task<IEnumerable<MovieDto>> GetMoviesByGenreAsync(int genreId);
}