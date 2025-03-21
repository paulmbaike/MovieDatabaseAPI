using AutoMapper;
using Moq;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Services.Services;

namespace MovieDatabaseAPI.UnitTests.Services;

public class GenreServiceTests
{
    private readonly Mock<IGenreRepository> _mockGenreRepository;
    private readonly Mock<IMovieRepository> _mockMovieRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GenreService _genreService;

    public GenreServiceTests()
    {
        _mockGenreRepository = new Mock<IGenreRepository>();
        _mockMovieRepository = new Mock<IMovieRepository>();
        _mockMapper = new Mock<IMapper>();

        _genreService = new GenreService(
            _mockGenreRepository.Object,
            _mockMovieRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task GetGenreByIdAsync_WhenGenreExists_ReturnsGenreDto()
    {
        // Arrange
        var genreId = 1;
        var genre = new Genre { Id = genreId, Name = "Action", Description = "Action films" };
        var genreDto = new GenreDto { Id = genreId, Name = "Action", Description = "Action films" };

        _mockGenreRepository.Setup(r => r.GetByIdAsync(genreId)).ReturnsAsync(genre);
        _mockMapper.Setup(m => m.Map<GenreDto>(genre)).Returns(genreDto);

        // Act
        var result = await _genreService.GetGenreByIdAsync(genreId);

        // Assert
        Assert.Equal(genreId, result.Id);
        Assert.Equal("Action", result.Name);
    }

    [Fact]
    public async Task GetGenreByIdAsync_WhenGenreDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var genreId = 1;

        _mockGenreRepository.Setup(r => r.GetByIdAsync(genreId)).ReturnsAsync((Genre)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _genreService.GetGenreByIdAsync(genreId));
    }

    [Fact]
    public async Task CreateGenreAsync_WithValidData_ReturnsCreatedGenre()
    {
        // Arrange
        var createGenreDto = new CreateGenreDto
        {
            Name = "Horror",
            Description = "Horror films"
        };

        var genre = new Genre
        {
            Id = 1,
            Name = "Horror",
            Description = "Horror films"
        };

        var genreDto = new GenreDto
        {
            Id = 1,
            Name = "Horror",
            Description = "Horror films"
        };

        _mockMapper.Setup(m => m.Map<Genre>(createGenreDto)).Returns(genre);
        _mockGenreRepository.Setup(r => r.AddAsync(genre)).ReturnsAsync(genre);
        _mockMapper.Setup(m => m.Map<GenreDto>(genre)).Returns(genreDto);

        // Act
        var result = await _genreService.CreateGenreAsync(createGenreDto);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Horror", result.Name);
    }

    [Fact]
    public async Task UpdateGenreAsync_WithValidData_UpdatesGenre()
    {
        // Arrange
        var genreId = 1;
        var updateGenreDto = new UpdateGenreDto
        {
            Name = "Horror",
            Description = "Updated description"
        };

        var genre = new Genre { Id = genreId, Name = "Horror", Description = "Old description" };

        _mockGenreRepository.Setup(r => r.GetByIdAsync(genreId)).ReturnsAsync(genre);
        _mockMapper.Setup(m => m.Map(updateGenreDto, genre)).Callback(() => {
            genre.Name = updateGenreDto.Name;
            genre.Description = updateGenreDto.Description;
        });

        // Act
        await _genreService.UpdateGenreAsync(genreId, updateGenreDto);

        // Assert
        _mockGenreRepository.Verify(r => r.UpdateAsync(genre), Times.Once);
        Assert.Equal("Updated description", genre.Description);
    }

    [Fact]
    public async Task DeleteGenreAsync_WhenGenreExists_DeletesGenre()
    {
        // Arrange
        var genreId = 1;
        var genre = new Genre { Id = genreId, Name = "Horror" };

        _mockGenreRepository.Setup(r => r.GetByIdAsync(genreId)).ReturnsAsync(genre);

        // Act
        await _genreService.DeleteGenreAsync(genreId);

        // Assert
        _mockGenreRepository.Verify(r => r.DeleteAsync(genreId), Times.Once);
    }

    [Fact]
    public async Task GetMoviesByGenreAsync_ReturnsMoviesInGenre()
    {
        // Arrange
        var genreId = 1;
        var movies = new List<Movie> {
                new Movie { Id = 1, Title = "The Shining" },
                new Movie { Id = 2, Title = "Get Out" }
            };

        var movieDtos = new List<MovieDto> {
                new MovieDto { Id = 1, Title = "The Shining" },
                new MovieDto { Id = 2, Title = "Get Out" }
            };

        _mockMovieRepository.Setup(r => r.GetMoviesByGenreAsync(genreId)).ReturnsAsync(movies);
        _mockMapper.Setup(m => m.Map<IEnumerable<MovieDto>>(movies)).Returns(movieDtos);

        // Act
        var result = await _genreService.GetMoviesByGenreAsync(genreId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Title == "The Shining");
        Assert.Contains(result, m => m.Title == "Get Out");
    }
}