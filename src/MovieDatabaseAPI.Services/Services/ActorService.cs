using AutoMapper;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.Services.Services;

public class ActorService : IActorService
{
    private readonly IActorRepository _actorRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IMapper _mapper;

    public ActorService(
        IActorRepository actorRepository,
        IMovieRepository movieRepository,
        IMapper mapper)
    {
        _actorRepository = actorRepository;
        _movieRepository = movieRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ActorDto>> GetAllActorsAsync()
    {
        var actors = await _actorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ActorDto>>(actors);
    }

    public async Task<ActorDto> GetActorByIdAsync(int id)
    {
        var actor = await _actorRepository.GetByIdAsync(id);
        if (actor == null)
        {
            throw new KeyNotFoundException($"Actor with ID {id} not found");
        }

        return _mapper.Map<ActorDto>(actor);
    }

    public async Task<ActorDto> CreateActorAsync(CreateActorDto createActorDto)
    {
        var actor = _mapper.Map<Actor>(createActorDto);
        await _actorRepository.AddAsync(actor);

        return _mapper.Map<ActorDto>(actor);
    }

    public async Task UpdateActorAsync(int id, UpdateActorDto updateActorDto)
    {
        var actor = await _actorRepository.GetByIdAsync(id);
        if (actor == null)
        {
            throw new KeyNotFoundException($"Actor with ID {id} not found");
        }

        _mapper.Map(updateActorDto, actor);
        await _actorRepository.UpdateAsync(actor);
    }

    public async Task DeleteActorAsync(int id)
    {
        var actor = await _actorRepository.GetByIdAsync(id);
        if (actor == null)
        {
            throw new KeyNotFoundException($"Actor with ID {id} not found");
        }

        await _actorRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ActorDto>> SearchActorsAsync(string searchTerm)
    {
        var actors = await _actorRepository.SearchActorsAsync(searchTerm);
        return _mapper.Map<IEnumerable<ActorDto>>(actors);
    }

    public async Task<IEnumerable<MovieDto>> GetMoviesByActorAsync(int actorId)
    {
        var movies = await _movieRepository.GetMoviesByActorAsync(actorId);
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }
}