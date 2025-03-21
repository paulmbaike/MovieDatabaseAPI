using MovieDatabaseAPI.Core.DTOs;

namespace MovieDatabaseAPI.Core.Interfaces.Services;

public interface IDirectorService
{
    Task<IEnumerable<DirectorDto>> GetAllDirectorsAsync();
    Task<DirectorDto> GetDirectorByIdAsync(int id);
    Task<DirectorDto> CreateDirectorAsync(CreateDirectorDto createDirectorDto);
    Task UpdateDirectorAsync(int id, UpdateDirectorDto updateDirectorDto);
    Task DeleteDirectorAsync(int id);
    Task<IEnumerable<DirectorDto>> SearchDirectorsAsync(string searchTerm);
    Task<IEnumerable<MovieDto>> GetMoviesByDirectorAsync(int directorId);
}