using AutoMapper;
using Moq;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Services.Services;

namespace MovieDatabaseAPI.UnitTests.Services;

public class DirectorServiceTests
{
    private readonly Mock<IDirectorRepository> _mockDirectorRepository;
    private readonly Mock<IMovieRepository> _mockMovieRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DirectorService _directorService;

    public DirectorServiceTests()
    {
        _mockDirectorRepository = new Mock<IDirectorRepository>();
        _mockMovieRepository = new Mock<IMovieRepository>();
        _mockMapper = new Mock<IMapper>();

        _directorService = new DirectorService(
            _mockDirectorRepository.Object,
            _mockMovieRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task GetDirectorByIdAsync_WhenDirectorExists_ReturnsDirectorDto()
    {
        // Arrange
        var directorId = 1;
        var director = new Director { Id = directorId, Name = "Christopher Nolan" };
        var directorDto = new DirectorDto { Id = directorId, Name = "Christopher Nolan" };

        _mockDirectorRepository.Setup(r => r.GetByIdAsync(directorId)).ReturnsAsync(director);
        _mockMapper.Setup(m => m.Map<DirectorDto>(director)).Returns(directorDto);

        // Act
        var result = await _directorService.GetDirectorByIdAsync(directorId);

        // Assert
        Assert.Equal(directorId, result.Id);
        Assert.Equal("Christopher Nolan", result.Name);
    }

    [Fact]
    public async Task GetDirectorByIdAsync_WhenDirectorDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var directorId = 1;

        _mockDirectorRepository.Setup(r => r.GetByIdAsync(directorId)).ReturnsAsync((Director)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _directorService.GetDirectorByIdAsync(directorId));
    }

    [Fact]
    public async Task CreateDirectorAsync_WithValidData_ReturnsCreatedDirector()
    {
        // Arrange
        var createDirectorDto = new CreateDirectorDto
        {
            Name = "Steven Spielberg",
            Bio = "American film director",
            DateOfBirth = new DateOnly(1946, 12, 18)
        };

        var director = new Director
        {
            Id = 1,
            Name = "Steven Spielberg",
            Bio = "American film director",
            DateOfBirth = new DateOnly(1946, 12, 18)
        };

        var directorDto = new DirectorDto
        {
            Id = 1,
            Name = "Steven Spielberg",
            Bio = "American film director",
            DateOfBirth = new DateOnly(1946, 12, 18)
        };

        _mockMapper.Setup(m => m.Map<Director>(createDirectorDto)).Returns(director);
        _mockDirectorRepository.Setup(r => r.AddAsync(director)).ReturnsAsync(director);
        _mockMapper.Setup(m => m.Map<DirectorDto>(director)).Returns(directorDto);

        // Act
        var result = await _directorService.CreateDirectorAsync(createDirectorDto);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Steven Spielberg", result.Name);
    }

    [Fact]
    public async Task UpdateDirectorAsync_WithValidData_UpdatesDirector()
    {
        // Arrange
        var directorId = 1;
        var updateDirectorDto = new UpdateDirectorDto
        {
            Name = "Steven Spielberg",
            Bio = "Updated bio"
        };

        var director = new Director { Id = directorId, Name = "Steven Spielberg", Bio = "Old bio" };

        _mockDirectorRepository.Setup(r => r.GetByIdAsync(directorId)).ReturnsAsync(director);
        _mockMapper.Setup(m => m.Map(updateDirectorDto, director)).Callback(() => {
            director.Name = updateDirectorDto.Name;
            director.Bio = updateDirectorDto.Bio;
        });

        // Act
        await _directorService.UpdateDirectorAsync(directorId, updateDirectorDto);

        // Assert
        _mockDirectorRepository.Verify(r => r.UpdateAsync(director), Times.Once);
        Assert.Equal("Updated bio", director.Bio);
    }

    [Fact]
    public async Task DeleteDirectorAsync_WhenDirectorExists_DeletesDirector()
    {
        // Arrange
        var directorId = 1;
        var director = new Director { Id = directorId, Name = "Steven Spielberg" };

        _mockDirectorRepository.Setup(r => r.GetByIdAsync(directorId)).ReturnsAsync(director);

        // Act
        await _directorService.DeleteDirectorAsync(directorId);

        // Assert
        _mockDirectorRepository.Verify(r => r.DeleteAsync(directorId), Times.Once);
    }

    [Fact]
    public async Task SearchDirectorsAsync_ReturnsMatchingDirectors()
    {
        // Arrange
        var searchTerm = "Steven";
        var directors = new List<Director> {
                new Director { Id = 1, Name = "Steven Spielberg" },
                new Director { Id = 2, Name = "Steven Soderbergh" }
            };

        var directorDtos = new List<DirectorDto> {
                new DirectorDto { Id = 1, Name = "Steven Spielberg" },
                new DirectorDto { Id = 2, Name = "Steven Soderbergh" }
            };

        _mockDirectorRepository.Setup(r => r.SearchDirectorsAsync(searchTerm)).ReturnsAsync(directors);
        _mockMapper.Setup(m => m.Map<IEnumerable<DirectorDto>>(directors)).Returns(directorDtos);

        // Act
        var result = await _directorService.SearchDirectorsAsync(searchTerm);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, d => d.Name == "Steven Spielberg");
        Assert.Contains(result, d => d.Name == "Steven Soderbergh");
    }

    [Fact]
    public async Task GetMoviesByDirectorAsync_ReturnsMoviesWithDirector()
    {
        // Arrange
        var directorId = 1;
        var movies = new List<Movie> {
                new Movie { Id = 1, Title = "Jaws" },
                new Movie { Id = 2, Title = "E.T." }
            };

        var movieDtos = new List<MovieDto> {
                new MovieDto { Id = 1, Title = "Jaws" },
                new MovieDto { Id = 2, Title = "E.T." }
            };

        _mockMovieRepository.Setup(r => r.GetMoviesByDirectorAsync(directorId)).ReturnsAsync(movies);
        _mockMapper.Setup(m => m.Map<IEnumerable<MovieDto>>(movies)).Returns(movieDtos);

        // Act
        var result = await _directorService.GetMoviesByDirectorAsync(directorId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Title == "Jaws");
        Assert.Contains(result, m => m.Title == "E.T.");
    }
}