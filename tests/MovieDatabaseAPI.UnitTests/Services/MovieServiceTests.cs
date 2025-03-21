using AutoMapper;
using Moq;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Services.Services;

namespace MovieDatabaseAPI.UnitTests.Services;

public class MovieServiceTests
{
    private readonly Mock<IMovieRepository> _mockMovieRepository;
    private readonly Mock<IActorRepository> _mockActorRepository;
    private readonly Mock<IGenreRepository> _mockGenreRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MovieService _movieService;

    public MovieServiceTests()
    {
        _mockMovieRepository = new Mock<IMovieRepository>();
        _mockActorRepository = new Mock<IActorRepository>();
        _mockGenreRepository = new Mock<IGenreRepository>();
        _mockMapper = new Mock<IMapper>();

        _movieService = new MovieService(
            _mockMovieRepository.Object,
            _mockActorRepository.Object,
            _mockGenreRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task GetMovieByIdAsync_WhenMovieExists_ReturnsMovieDto()
    {
        // Arrange
        var movieId = 1;
        var movie = new Movie { Id = movieId, Title = "Test Movie" };
        var movieDto = new MovieDto { Id = movieId, Title = "Test Movie" };

        _mockMovieRepository.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync(movie);
        _mockMapper.Setup(m => m.Map<MovieDto>(movie)).Returns(movieDto);

        // Act
        var result = await _movieService.GetMovieByIdAsync(movieId);

        // Assert
        Assert.Equal(movieId, result.Id);
        Assert.Equal("Test Movie", result.Title);
    }

    [Fact]
    public async Task GetMovieByIdAsync_WhenMovieDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var movieId = 1;

        _mockMovieRepository.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync((Movie)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _movieService.GetMovieByIdAsync(movieId));
    }

    [Fact]
    public async Task CreateMovieAsync_WithValidData_ReturnsCreatedMovie()
    {
        // Arrange
        var createMovieDto = new CreateMovieDto
        {
            Title = "New Movie",
            ReleaseYear = 2023,
            GenreIds = new List<int> { 1 },
            ActorIds = new List<int> { 1 }
        };

        var movie = new Movie { Id = 1, Title = "New Movie" };
        var movieDto = new MovieDto { Id = 1, Title = "New Movie" };

        _mockMapper.Setup(m => m.Map<Movie>(createMovieDto)).Returns(movie);
        _mockMovieRepository.Setup(r => r.AddAsync(movie)).ReturnsAsync(movie);
        _mockMovieRepository.Setup(r => r.GetByIdAsync(movie.Id)).ReturnsAsync(movie);
        _mockMapper.Setup(m => m.Map<MovieDto>(movie)).Returns(movieDto);

        // Act
        var result = await _movieService.CreateMovieAsync(createMovieDto);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("New Movie", result.Title);
    }
}