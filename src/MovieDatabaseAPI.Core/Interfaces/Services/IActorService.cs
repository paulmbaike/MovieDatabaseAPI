using MovieDatabaseAPI.Core.DTOs;

namespace MovieDatabaseAPI.Core.Interfaces.Services;

public interface IActorService
{
    Task<IEnumerable<ActorDto>> GetAllActorsAsync();
    Task<ActorDto> GetActorByIdAsync(int id);
    Task<ActorDto> CreateActorAsync(CreateActorDto createActorDto);
    Task UpdateActorAsync(int id, UpdateActorDto updateActorDto);
    Task DeleteActorAsync(int id);
    Task<IEnumerable<ActorDto>> SearchActorsAsync(string searchTerm);
    Task<IEnumerable<MovieDto>> GetMoviesByActorAsync(int actorId);
}