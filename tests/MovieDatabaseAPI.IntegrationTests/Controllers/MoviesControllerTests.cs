using Microsoft.VisualStudio.TestPlatform.TestHost;
using MovieDatabaseAPI.Core.DTOs;
using System.Net.Http.Json;
using System.Net;

namespace MovieDatabaseAPI.IntegrationTests.Controllers;

public class MoviesControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private string _authToken;

    public MoviesControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        _authToken = await AuthHelper.GetJwtTokenAsync(_client);
        AuthHelper.AuthorizeClient(_client, _authToken);
    }

    [Fact]
    public async Task GetMovies_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/movies");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task GetMovie_WithValidId_ReturnsMovie()
    {
        // Arrange
        await AuthenticateAsync();
        var id = 1; // ID of seeded movie

        // Act
        var response = await _client.GetAsync($"/api/v1/movies/{id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var movie = await response.Content.ReadFromJsonAsync<MovieDto>();
        Assert.NotNull(movie);
        Assert.Equal("Inception", movie.Title);
        Assert.Equal(2010, movie.ReleaseYear);
    }

    [Fact]
    public async Task GetMovie_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        await AuthenticateAsync();
        var id = 999; // Non-existent ID

        // Act
        var response = await _client.GetAsync($"/api/v1/movies/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateMovie_WithValidData_ReturnsCreatedMovie()
    {
        // Arrange
        await AuthenticateAsync();
        var createMovieDto = new CreateMovieDto
        {
            Title = "The Dark Knight",
            ReleaseYear = 2008,
            Plot = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
            RuntimeMinutes = 152,
            PosterUrl = "https://example.com/dark-knight.jpg",
            DirectorId = 1,
            GenreIds = new List<int> { 1 }, // Action
            ActorIds = new List<int> { 2 }  // Tom Hardy
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/movies", createMovieDto);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdMovie = await response.Content.ReadFromJsonAsync<MovieDto>();
        Assert.NotNull(createdMovie);
        Assert.Equal("The Dark Knight", createdMovie.Title);
        Assert.Equal(2008, createdMovie.ReleaseYear);
    }

    [Fact]
    public async Task CreateMovie_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();
        var createMovieDto = new CreateMovieDto
        {
            // Missing required fields
            Title = "", // Empty title should fail validation
            ReleaseYear = 2030, // Future year that's not realistic
            Plot = "A test movie",
            RuntimeMinutes = 0, // Invalid runtime
            GenreIds = new List<int>(),
            ActorIds = new List<int>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/movies", createMovieDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMovie_WithValidData_ReturnsNoContent()
    {
        // Arrange
        await AuthenticateAsync();
        var id = 1; // ID of seeded movie
        var updateMovieDto = new UpdateMovieDto
        {
            Title = "Inception (Updated)",
            ReleaseYear = 2010,
            Plot = "Updated plot",
            RuntimeMinutes = 148,
            PosterUrl = "https://example.com/inception.jpg",
            DirectorId = 1,
            GenreIds = new List<int> { 1, 2 }, // Action, Comedy
            ActorIds = new List<int> { 1 }     // Leonardo DiCaprio
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/movies/{id}", updateMovieDto);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Check that the movie was updated
        var getResponse = await _client.GetAsync($"/api/v1/movies/{id}");
        getResponse.EnsureSuccessStatusCode();
        var updatedMovie = await getResponse.Content.ReadFromJsonAsync<MovieDto>();
        Assert.NotNull(updatedMovie);
        Assert.Equal("Inception (Updated)", updatedMovie.Title);
        Assert.Equal("Updated plot", updatedMovie.Plot);
    }

    [Fact]
    public async Task DeleteMovie_WithValidId_ReturnsNoContent()
    {
        // Arrange
        await AuthenticateAsync();

        // Create a movie to delete
        var createMovieDto = new CreateMovieDto
        {
            Title = "Movie to Delete",
            ReleaseYear = 2020,
            Plot = "This movie will be deleted",
            RuntimeMinutes = 120,
            DirectorId = 1,
            GenreIds = new List<int> { 1 },
            ActorIds = new List<int> { 1 }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/movies", createMovieDto);
        createResponse.EnsureSuccessStatusCode();
        var createdMovie = await createResponse.Content.ReadFromJsonAsync<MovieDto>();
        var id = createdMovie?.Id ?? 0;

        // Act
        var response = await _client.DeleteAsync($"/api/v1/movies/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the movie is deleted
        var getResponse = await _client.GetAsync($"/api/v1/movies/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetMoviesByDirector_ReturnsMoviesForDirector()
    {
        // Arrange
        await AuthenticateAsync();
        var directorId = 1; // Christopher Nolan

        // Act
        var response = await _client.GetAsync($"/api/v1/movies/director/{directorId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var movies = await response.Content.ReadFromJsonAsync<IEnumerable<MovieDto>>();
        Assert.NotNull(movies);
        Assert.All(movies, movie => Assert.Equal("Christopher Nolan", movie.DirectorName));
    }

    [Fact]
    public async Task SearchMovies_ReturnsMatchingMovies()
    {
        // Arrange
        await AuthenticateAsync();
        var searchTerm = "Incep";

        // Act
        var response = await _client.GetAsync($"/api/v1/movies/search?searchTerm={searchTerm}");

        // Assert
        response.EnsureSuccessStatusCode();
        var movies = await response.Content.ReadFromJsonAsync<IEnumerable<MovieDto>>();
        Assert.NotNull(movies);
        Assert.Contains(movies, movie => movie.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Unauthorized_CannotAccessProtectedEndpoints()
    {
        // Act - No authentication
        var response = await _client.GetAsync("/api/v1/movies");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}