using AutoMapper;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.Services.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IMapper _mapper;

    public GenreService(
        IGenreRepository genreRepository,
        IMovieRepository movieRepository,
        IMapper mapper)
    {
        _genreRepository = genreRepository;
        _movieRepository = movieRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GenreDto>> GetAllGenresAsync()
    {
        var genres = await _genreRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GenreDto>>(genres);
    }

    public async Task<GenreDto> GetGenreByIdAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
        {
            throw new KeyNotFoundException($"Genre with ID {id} not found");
        }

        return _mapper.Map<GenreDto>(genre);
    }

    public async Task<GenreDto> CreateGenreAsync(CreateGenreDto createGenreDto)
    {
        var genre = _mapper.Map<Genre>(createGenreDto);
        await _genreRepository.AddAsync(genre);

        return _mapper.Map<GenreDto>(genre);
    }

    public async Task UpdateGenreAsync(int id, UpdateGenreDto updateGenreDto)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
        {
            throw new KeyNotFoundException($"Genre with ID {id} not found");
        }

        _mapper.Map(updateGenreDto, genre);
        await _genreRepository.UpdateAsync(genre);
    }

    public async Task DeleteGenreAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
        {
            throw new KeyNotFoundException($"Genre with ID {id} not found");
        }

        await _genreRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<MovieDto>> GetMoviesByGenreAsync(int genreId)
    {
        var movies = await _movieRepository.GetMoviesByGenreAsync(genreId);
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }
}