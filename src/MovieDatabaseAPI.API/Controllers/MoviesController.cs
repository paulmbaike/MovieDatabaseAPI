using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.API.Controllers;


/// <summary>
/// API controller for managing movie resources
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;

    /// <summary>
    /// Initializes a new instance of the MoviesController
    /// </summary>
    /// <param name="movieService">Movie service for business logic operations</param>
    /// <param name="logger">Logger for diagnostic information</param>
    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all movies
    /// </summary>
    /// <returns>Collection of movies</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        return Ok(movies);
    }

    /// <summary>
    /// Retrieves a specific movie by ID
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <returns>Movie details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDto>> GetMovie(int id)
    {
        try
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            return Ok(movie);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Movie not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new movie
    /// </summary>
    /// <param name="createMovieDto">Movie creation data</param>
    /// <returns>Newly created movie</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieDto>> CreateMovie(CreateMovieDto createMovieDto)
    {
        try
        {
            var movie = await _movieService.CreateMovieAsync(createMovieDto);
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating movie");
            return BadRequest(new { message = "Error creating movie", details = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing movie
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="updateMovieDto">Movie update data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMovie(int id, UpdateMovieDto updateMovieDto)
    {
        try
        {
            await _movieService.UpdateMovieAsync(id, updateMovieDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Movie not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating movie: {Id}", id);
            return BadRequest(new { message = "Error updating movie", details = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a movie
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            await _movieService.DeleteMovieAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Movie not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves movies by director
    /// </summary>
    /// <param name="directorId">Director ID</param>
    /// <returns>Collection of movies by the specified director</returns>
    [HttpGet("director/{directorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesByDirector(int directorId)
    {
        var movies = await _movieService.GetMoviesByDirectorAsync(directorId);
        return Ok(movies);
    }

    /// <summary>
    /// Retrieves movies by genre
    /// </summary>
    /// <param name="genreId">Genre ID</param>
    /// <returns>Collection of movies in the specified genre</returns>
    [HttpGet("genre/{genreId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesByGenre(int genreId)
    {
        var movies = await _movieService.GetMoviesByGenreAsync(genreId);
        return Ok(movies);
    }

    /// <summary>
    /// Retrieves movies by actor
    /// </summary>
    /// <param name="actorId">Actor ID</param>
    /// <returns>Collection of movies featuring the specified actor</returns>
    [HttpGet("actor/{actorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesByActor(int actorId)
    {
        var movies = await _movieService.GetMoviesByActorAsync(actorId);
        return Ok(movies);
    }

    /// <summary>
    /// Searches for movies by title
    /// </summary>
    /// <param name="searchTerm">Search query string</param>
    /// <returns>Collection of movies matching the search term</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovieDto>>> SearchMovies([FromQuery] string searchTerm)
    {
        var movies = await _movieService.SearchMoviesAsync(searchTerm);
        return Ok(movies);
    }
}