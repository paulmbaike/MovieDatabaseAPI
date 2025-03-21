using AutoMapper;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.Services.Services;

public class DirectorService : IDirectorService
{
    private readonly IDirectorRepository _directorRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IMapper _mapper;

    public DirectorService(
        IDirectorRepository directorRepository,
        IMovieRepository movieRepository,
        IMapper mapper)
    {
        _directorRepository = directorRepository;
        _movieRepository = movieRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DirectorDto>> GetAllDirectorsAsync()
    {
        var directors = await _directorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DirectorDto>>(directors);
    }

    public async Task<DirectorDto> GetDirectorByIdAsync(int id)
    {
        var director = await _directorRepository.GetByIdAsync(id);
        if (director == null)
        {
            throw new KeyNotFoundException($"Director with ID {id} not found");
        }

        return _mapper.Map<DirectorDto>(director);
    }

    public async Task<DirectorDto> CreateDirectorAsync(CreateDirectorDto createDirectorDto)
    {
        var director = _mapper.Map<Director>(createDirectorDto);
        await _directorRepository.AddAsync(director);

        return _mapper.Map<DirectorDto>(director);
    }

    public async Task UpdateDirectorAsync(int id, UpdateDirectorDto updateDirectorDto)
    {
        var director = await _directorRepository.GetByIdAsync(id);
        if (director == null)
        {
            throw new KeyNotFoundException($"Director with ID {id} not found");
        }

        _mapper.Map(updateDirectorDto, director);
        await _directorRepository.UpdateAsync(director);
    }

    public async Task DeleteDirectorAsync(int id)
    {
        var director = await _directorRepository.GetByIdAsync(id);
        if (director == null)
        {
            throw new KeyNotFoundException($"Director with ID {id} not found");
        }

        await _directorRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<DirectorDto>> SearchDirectorsAsync(string searchTerm)
    {
        var directors = await _directorRepository.SearchDirectorsAsync(searchTerm);
        return _mapper.Map<IEnumerable<DirectorDto>>(directors);
    }

    public async Task<IEnumerable<MovieDto>> GetMoviesByDirectorAsync(int directorId)
    {
        var movies = await _movieRepository.GetMoviesByDirectorAsync(directorId);
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }
}