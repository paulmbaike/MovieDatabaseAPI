using AutoMapper;
using Moq;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Services.Services;

namespace MovieDatabaseAPI.UnitTests.Services;

public class ActorServiceTests
{
    private readonly Mock<IActorRepository> _mockActorRepository;
    private readonly Mock<IMovieRepository> _mockMovieRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ActorService _actorService;

    public ActorServiceTests()
    {
        _mockActorRepository = new Mock<IActorRepository>();
        _mockMovieRepository = new Mock<IMovieRepository>();
        _mockMapper = new Mock<IMapper>();

        _actorService = new ActorService(
            _mockActorRepository.Object,
            _mockMovieRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task GetActorByIdAsync_WhenActorExists_ReturnsActorDto()
    {
        // Arrange
        var actorId = 1;
        var actor = new Actor { Id = actorId, Name = "Tom Hanks" };
        var actorDto = new ActorDto { Id = actorId, Name = "Tom Hanks" };

        _mockActorRepository.Setup(r => r.GetByIdAsync(actorId)).ReturnsAsync(actor);
        _mockMapper.Setup(m => m.Map<ActorDto>(actor)).Returns(actorDto);

        // Act
        var result = await _actorService.GetActorByIdAsync(actorId);

        // Assert
        Assert.Equal(actorId, result.Id);
        Assert.Equal("Tom Hanks", result.Name);
    }

    [Fact]
    public async Task GetActorByIdAsync_WhenActorDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var actorId = 1;

        _mockActorRepository.Setup(r => r.GetByIdAsync(actorId)).ReturnsAsync((Actor)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _actorService.GetActorByIdAsync(actorId));
    }

    [Fact]
    public async Task CreateActorAsync_WithValidData_ReturnsCreatedActor()
    {
        // Arrange
        var createActorDto = new CreateActorDto
        {
            Name = "Brad Pitt",
            Bio = "American actor and producer",
            DateOfBirth = new DateTime(1963, 12, 18)
        };

        var actor = new Actor
        {
            Id = 1,
            Name = "Brad Pitt",
            Bio = "American actor and producer",
            DateOfBirth = new DateOnly(1963, 12, 18)
        };

        var actorDto = new ActorDto
        {
            Id = 1,
            Name = "Brad Pitt",
            Bio = "American actor and producer",
            DateOfBirth = new DateTime(1963, 12, 18)
        };

        _mockMapper.Setup(m => m.Map<Actor>(createActorDto)).Returns(actor);
        _mockActorRepository.Setup(r => r.AddAsync(actor)).ReturnsAsync(actor);
        _mockMapper.Setup(m => m.Map<ActorDto>(actor)).Returns(actorDto);

        // Act
        var result = await _actorService.CreateActorAsync(createActorDto);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Brad Pitt", result.Name);
    }

    [Fact]
    public async Task UpdateActorAsync_WithValidData_UpdatesActor()
    {
        // Arrange
        var actorId = 1;
        var updateActorDto = new UpdateActorDto
        {
            Name = "Brad Pitt",
            Bio = "Updated bio"
        };

        var actor = new Actor { Id = actorId, Name = "Brad Pitt", Bio = "Old bio" };

        _mockActorRepository.Setup(r => r.GetByIdAsync(actorId)).ReturnsAsync(actor);
        _mockMapper.Setup(m => m.Map(updateActorDto, actor)).Callback(() => {
            actor.Name = updateActorDto.Name;
            actor.Bio = updateActorDto.Bio;
        });

        // Act
        await _actorService.UpdateActorAsync(actorId, updateActorDto);

        // Assert
        _mockActorRepository.Verify(r => r.UpdateAsync(actor), Times.Once);
        Assert.Equal("Updated bio", actor.Bio);
    }

    [Fact]
    public async Task DeleteActorAsync_WhenActorExists_DeletesActor()
    {
        // Arrange
        var actorId = 1;
        var actor = new Actor { Id = actorId, Name = "Brad Pitt" };

        _mockActorRepository.Setup(r => r.GetByIdAsync(actorId)).ReturnsAsync(actor);

        // Act
        await _actorService.DeleteActorAsync(actorId);

        // Assert
        _mockActorRepository.Verify(r => r.DeleteAsync(actorId), Times.Once);
    }

    [Fact]
    public async Task SearchActorsAsync_ReturnsMatchingActors()
    {
        // Arrange
        var searchTerm = "Tom";
        var actors = new List<Actor> {
                new Actor { Id = 1, Name = "Tom Hanks" },
                new Actor { Id = 2, Name = "Tom Cruise" }
            };

        var actorDtos = new List<ActorDto> {
                new ActorDto { Id = 1, Name = "Tom Hanks" },
                new ActorDto { Id = 2, Name = "Tom Cruise" }
            };

        _mockActorRepository.Setup(r => r.SearchActorsAsync(searchTerm)).ReturnsAsync(actors);
        _mockMapper.Setup(m => m.Map<IEnumerable<ActorDto>>(actors)).Returns(actorDtos);

        // Act
        var result = await _actorService.SearchActorsAsync(searchTerm);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.Name == "Tom Hanks");
        Assert.Contains(result, a => a.Name == "Tom Cruise");
    }

    [Fact]
    public async Task GetMoviesByActorAsync_ReturnsMoviesWithActor()
    {
        // Arrange
        var actorId = 1;
        var movies = new List<Movie> {
                new Movie { Id = 1, Title = "Forrest Gump" },
                new Movie { Id = 2, Title = "Cast Away" }
            };

        var movieDtos = new List<MovieDto> {
                new MovieDto { Id = 1, Title = "Forrest Gump" },
                new MovieDto { Id = 2, Title = "Cast Away" }
            };

        _mockMovieRepository.Setup(r => r.GetMoviesByActorAsync(actorId)).ReturnsAsync(movies);
        _mockMapper.Setup(m => m.Map<IEnumerable<MovieDto>>(movies)).Returns(movieDtos);

        // Act
        var result = await _actorService.GetMoviesByActorAsync(actorId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Title == "Forrest Gump");
        Assert.Contains(result, m => m.Title == "Cast Away");
    }
}