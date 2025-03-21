using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.API.Controllers;

/// <summary>
/// API controller for managing genre resources
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/genres")]
[Authorize]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenresController> _logger;

    /// <summary>
    /// Initializes a new instance of the GenresController
    /// </summary>
    /// <param name="genreService">Genre service for business logic operations</param>
    /// <param name="logger">Logger for diagnostic information</param>
    public GenresController(IGenreService genreService, ILogger<GenresController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all genres
    /// </summary>
    /// <returns>Collection of genres</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
    {
        var genres = await _genreService.GetAllGenresAsync();
        return Ok(genres);
    }

    /// <summary>
    /// Retrieves a specific genre by ID
    /// </summary>
    /// <param name="id">Genre ID</param>
    /// <returns>Genre details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDto>> GetGenre(int id)
    {
        try
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            return Ok(genre);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Genre not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new genre
    /// </summary>
    /// <param name="createGenreDto">Genre creation data</param>
    /// <returns>Newly created genre</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto createGenreDto)
    {
        try
        {
            var genre = await _genreService.CreateGenreAsync(createGenreDto);
            return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating genre");
            return BadRequest(new { message = "Error creating genre", details = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing genre
    /// </summary>
    /// <param name="id">Genre ID</param>
    /// <param name="updateGenreDto">Genre update data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateGenre(int id, UpdateGenreDto updateGenreDto)
    {
        try
        {
            await _genreService.UpdateGenreAsync(id, updateGenreDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Genre not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating genre: {Id}", id);
            return BadRequest(new { message = "Error updating genre", details = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a genre
    /// </summary>
    /// <param name="id">Genre ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        try
        {
            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Genre not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all movies in a specific genre
    /// </summary>
    /// <param name="id">Genre ID</param>
    /// <returns>Collection of movies in the genre</returns>
    [HttpGet("{id}/movies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetGenreMovies(int id)
    {
        try
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            var movies = await _genreService.GetMoviesByGenreAsync(id);
            return Ok(movies);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Genre not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }
}