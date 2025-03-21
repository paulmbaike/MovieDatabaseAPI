using AutoMapper;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.Services.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IActorRepository _actorRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public MovieService(
        IMovieRepository movieRepository,
        IActorRepository actorRepository,
        IGenreRepository genreRepository,
        IMapper mapper)
    {
        _movieRepository = movieRepository;
        _actorRepository = actorRepository;
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
    {
        var movies = await _movieRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }

    public async Task<MovieDto> GetMovieByIdAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {id} not found");
        }

        return _mapper.Map<MovieDto>(movie);
    }

    public async Task<MovieDto> CreateMovieAsync(CreateMovieDto createMovieDto)
    {
        var movie = new Movie
        {
            Title = createMovieDto.Title,
            ReleaseYear = createMovieDto.ReleaseYear,
            Plot = createMovieDto.Plot,
            RuntimeMinutes = createMovieDto.RuntimeMinutes,
            PosterUrl = createMovieDto.PosterUrl,
            DirectorId = createMovieDto.DirectorId
        };

        // Add actors
        if (createMovieDto.ActorIds.Any())
        {
            foreach (var actorId in createMovieDto.ActorIds)
            {
                var actor = await _actorRepository.GetByIdAsync(actorId);
                if (actor != null)
                {
                    movie.Actors.Add(actor);
                }
            }
        }

        // Add genres
        if (createMovieDto.GenreIds.Any())
        {
            foreach (var genreId in createMovieDto.GenreIds)
            {
                var genre = await _genreRepository.GetByIdAsync(genreId);
                if (genre != null)
                {
                    movie.Genres.Add(genre);
                }
            }
        }

        await _movieRepository.AddAsync(movie);

        // Fetch the complete movie with relationships
        var createdMovie = await _movieRepository.GetByIdAsync(movie.Id);
        return _mapper.Map<MovieDto>(createdMovie);
    }

    public async Task UpdateMovieAsync(int id, UpdateMovieDto updateMovieDto)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {id} not found");
        }

        // Update basic properties
        movie.Title = updateMovieDto.Title;
        movie.ReleaseYear = updateMovieDto.ReleaseYear;
        movie.Plot = updateMovieDto.Plot;
        movie.RuntimeMinutes = updateMovieDto.RuntimeMinutes;
        movie.PosterUrl = updateMovieDto.PosterUrl;
        movie.DirectorId = updateMovieDto.DirectorId;

        // Update actors
        movie.Actors.Clear();
        if (updateMovieDto.ActorIds.Any())
        {
            foreach (var actorId in updateMovieDto.ActorIds)
            {
                var actor = await _actorRepository.GetByIdAsync(actorId);
                if (actor != null)
                {
                    movie.Actors.Add(actor);
                }
            }
        }

        // Update genres
        movie.Genres.Clear();
        if (updateMovieDto.GenreIds.Any())
        {
            foreach (var genreId in updateMovieDto.GenreIds)
            {
                var genre = await _genreRepository.GetByIdAsync(genreId);
                if (genre != null)
                {
                    movie.Genres.Add(genre);
                }
            }
        }

        await _movieRepository.UpdateAsync(movie);
    }

    public async Task DeleteMovieAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {id} not found");
        }

        await _movieRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<MovieDto>> GetMoviesByDirectorAsync(int directorId)
    {
        var movies = await _movieRepository.GetMoviesByDirectorAsync(directorId);
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }

    public async Task<IEnumerable<MovieDto>> GetMoviesByGenreAsync(int genreId)
    {
        var movies = await _movieRepository.GetMoviesByGenreAsync(genreId);
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }

    public async Task<IEnumerable<MovieDto>> GetMoviesByActorAsync(int actorId)
    {
        var movies = await _movieRepository.GetMoviesByActorAsync(actorId);
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }

    public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string searchTerm)
    {
        var movies = await _movieRepository.SearchMoviesAsync(searchTerm);
        return _mapper.Map<IEnumerable<MovieDto>>(movies);
    }
}