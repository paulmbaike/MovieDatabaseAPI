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
    public async Task GetAllMoviesAsync_ReturnsAllMovies()
    {
        // Arrange
        var movies = new List<Movie>
        {
            new Movie { Id = 1, Title = "Inception" },
            new Movie { Id = 2, Title = "The Dark Knight" }
        };

        var movieDtos = new List<MovieDto>
        {
            new MovieDto { Id = 1, Title = "Inception" },
            new MovieDto { Id = 2, Title = "The Dark Knight" }
        };

        _mockMovieRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(movies);
        _mockMapper.Setup(m => m.Map<IEnumerable<MovieDto>>(movies)).Returns(movieDtos);

        // Act
        var result = await _movieService.GetAllMoviesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Title == "Inception");
        Assert.Contains(result, m => m.Title == "The Dark Knight");
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
            Plot = "Test plot",
            RuntimeMinutes = 120,
            PosterUrl = "http://example.com/poster.jpg",
            DirectorId = 1,
            GenreIds = new List<int> { 1, 2 },
            ActorIds = new List<int> { 1, 2 }
        };

        var movie = new Movie
        {
            Id = 1,
            Title = "New Movie",
            ReleaseYear = 2023,
            Plot = "Test plot",
            RuntimeMinutes = 120,
            PosterUrl = "http://example.com/poster.jpg",
            DirectorId = 1
        };

        var actor1 = new Actor { Id = 1, Name = "Actor 1" };
        var actor2 = new Actor { Id = 2, Name = "Actor 2" };
        var genre1 = new Genre { Id = 1, Name = "Genre 1" };
        var genre2 = new Genre { Id = 2, Name = "Genre 2" };

        var movieDto = new MovieDto
        {
            Id = 1,
            Title = "New Movie",
            ReleaseYear = 2023,
            Plot = "Test plot",
            RuntimeMinutes = 120,
            PosterUrl = "http://example.com/poster.jpg"
        };

        _mockActorRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(actor1);
        _mockActorRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(actor2);
        _mockGenreRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genre1);
        _mockGenreRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(genre2);

        _mockMovieRepository.Setup(r => r.AddAsync(It.IsAny<Movie>())).ReturnsAsync(movie);

        _mockMovieRepository.Setup(r => r.GetByIdAsync(movie.Id)).ReturnsAsync(movie);

        _mockMapper.Setup(m => m.Map<MovieDto>(movie)).Returns(movieDto);


        // Act
        var result = await _movieService.CreateMovieAsync(createMovieDto);

        _mockMovieRepository.Verify(r => r.AddAsync(It.IsAny<Movie>()), Times.Once);
        _mockMovieRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockMapper.Verify(m => m.Map<MovieDto>(It.IsAny<Movie>()), Times.Once);

        // Assert
        _mockMovieRepository.Verify(r => r.AddAsync(It.IsAny<Movie>()), Times.Once);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Movie", result.Title);
    }

    [Fact]
    public async Task UpdateMovieAsync_WithValidData_UpdatesMovie()
    {
        // Arrange
        var movieId = 1;
        var updateMovieDto = new UpdateMovieDto
        {
            Title = "Updated Movie",
            ReleaseYear = 2024,
            Plot = "Updated plot",
            RuntimeMinutes = 130,
            PosterUrl = "http://example.com/updated-poster.jpg",
            DirectorId = 2,
            GenreIds = new List<int> { 3 },
            ActorIds = new List<int> { 3 }
        };

        var movie = new Movie
        {
            Id = movieId,
            Title = "Original Movie",
            ReleaseYear = 2023,
            Plot = "Original plot",
            RuntimeMinutes = 120,
            PosterUrl = "http://example.com/poster.jpg",
            DirectorId = 1,
            Actors = new List<Actor>(),
            Genres = new List<Genre>()
        };

        var actor = new Actor { Id = 3, Name = "Actor 3" };
        var genre = new Genre { Id = 3, Name = "Genre 3" };

        _mockMovieRepository.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync(movie);
        _mockActorRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(actor);
        _mockGenreRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(genre);

        // Act
        await _movieService.UpdateMovieAsync(movieId, updateMovieDto);

        // Assert
        _mockMovieRepository.Verify(r => r.UpdateAsync(It.IsAny<Movie>()), Times.Once);
        Assert.Equal("Updated Movie", movie.Title);
        Assert.Equal(2024, movie.ReleaseYear);
        Assert.Equal("Updated plot", movie.Plot);
        Assert.Equal(130, movie.RuntimeMinutes);
        Assert.Equal("http://example.com/updated-poster.jpg", movie.PosterUrl);
        Assert.Equal(2, movie.DirectorId);
    }

    [Fact]
    public async Task UpdateMovieAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var movieId = 999;
        var updateMovieDto = new UpdateMovieDto { Title = "Updated Movie" };

        _mockMovieRepository.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync((Movie)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _movieService.UpdateMovieAsync(movieId, updateMovieDto));
    }

    [Fact]
    public async Task DeleteMovieAsync_WithValidId_DeletesMovie()
    {
        // Arrange
        var movieId = 1;
        var movie = new Movie { Id = movieId, Title = "Test Movie" };

        _mockMovieRepository.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync(movie);

        // Act
        await _movieService.DeleteMovieAsync(movieId);

        // Assert
        _mockMovieRepository.Verify(r => r.DeleteAsync(movieId), Times.Once);
    }

    [Fact]
    public async Task DeleteMovieAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var movieId = 999;

        _mockMovieRepository.Setup(r => r.GetByIdAsync(movieId)).ReturnsAsync((Movie)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _movieService.DeleteMovieAsync(movieId));
    }

    [Fact]
    public async Task GetMoviesByDirectorAsync_ReturnsMoviesByDirector()
    {
        // Arrange
        var directorId = 1;
        var movies = new List<Movie>
        {
            new Movie { Id = 1, Title = "Movie 1", DirectorId = directorId },
            new Movie { Id = 2, Title = "Movie 2", DirectorId = directorId }
        };

        var movieDtos = new List<MovieDto>
        {
            new MovieDto { Id = 1, Title = "Movie 1" },
            new MovieDto { Id = 2, Title = "Movie 2" }
        };

        _mockMovieRepository.Setup(r => r.GetMoviesByDirectorAsync(directorId)).ReturnsAsync(movies);
        _mockMapper.Setup(m => m.Map<IEnumerable<MovieDto>>(movies)).Returns(movieDtos);

        // Act
        var result = await _movieService.GetMoviesByDirectorAsync(directorId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Title == "Movie 1");
        Assert.Contains(result, m => m.Title == "Movie 2");
    }

    [Fact]
    public async Task GetMoviesByGenreAsync_ReturnsMoviesByGenre()
    {
        // Arrange
        var genreId = 1;
        var movies = new List<Movie>
        {
            new Movie { Id = 1, Title = "Movie 1" },
            new Movie { Id = 2, Title = "Movie 2" }
        };

        var movieDtos = new List<MovieDto>
        {
            new MovieDto { Id = 1, Title = "Movie 1" },
            new MovieDto { Id = 2, Title = "Movie 2" }
        };

        _mockMovieRepository.Setup(r => r.GetMoviesByGenreAsync(genreId)).ReturnsAsync(movies);
        _mockMapper.Setup(m => m.Map<IEnumerable<MovieDto>>(movies)).Returns(movieDtos);

        // Act
        var result = await _movieService.GetMoviesByGenreAsync(genreId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Title == "Movie 1");
        Assert.Contains(result, m => m.Title == "Movie 2");
    }

    [Fact]
    public async Task GetMoviesByActorAsync_ReturnsMoviesByActor()
    {
        // Arrange
        var actorId = 1;
        var movies = new List<Movie>
        {
            new Movie { Id = 1, Title = "Movie 1" },
            new Movie { Id = 2, Title = "Movie 2" }
        };

        var movieDtos = new List<MovieDto>
        {
            new MovieDto { Id = 1, Title = "Movie 1" },
            new MovieDto { Id = 2, Title = "Movie 2" }
        };

        _mockMovieRepository.Setup(r => r.GetMoviesByActorAsync(actorId)).ReturnsAsync(movies);
        _mockMapper.Setup(m => m.Map<IEnumerable<MovieDto>>(movies)).Returns(movieDtos);

        // Act
        var result = await _movieService.GetMoviesByActorAsync(actorId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, m => m.Title == "Movie 1");
        Assert.Contains(result, m => m.Title == "Movie 2");
    }

    [Fact]
    public async Task SearchMoviesAsync_ReturnsMatchingMovies()
    {
        // Arrange
        var searchTerm = "Inception";
        var movies = new List<Movie>
        {
            new Movie { Id = 1, Title = "Inception" }
        };

        var movieDtos = new List<MovieDto>
        {
            new MovieDto { Id = 1, Title = "Inception" }
        };

        _mockMovieRepository.Setup(r => r.SearchMoviesAsync(searchTerm)).ReturnsAsync(movies);
        _mockMapper.Setup(m => m.Map<IEnumerable<MovieDto>>(movies)).Returns(movieDtos);

        // Act
        var result = await _movieService.SearchMoviesAsync(searchTerm);

        // Assert
        Assert.Single(result);
        Assert.Contains(result, m => m.Title == "Inception");
    }
}