using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.API.Controllers;

/// <summary>
/// API controller for managing director resources
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/directors")]
[Authorize]
public class DirectorsController : ControllerBase
{
    private readonly IDirectorService _directorService;
    private readonly ILogger<DirectorsController> _logger;

    /// <summary>
    /// Initializes a new instance of the DirectorsController
    /// </summary>
    /// <param name="directorService">Director service for business logic operations</param>
    /// <param name="logger">Logger for diagnostic information</param>
    public DirectorsController(IDirectorService directorService, ILogger<DirectorsController> logger)
    {
        _directorService = directorService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all directors
    /// </summary>
    /// <returns>Collection of directors</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DirectorDto>>> GetDirectors()
    {
        var directors = await _directorService.GetAllDirectorsAsync();
        return Ok(directors);
    }

    /// <summary>
    /// Retrieves a specific director by ID
    /// </summary>
    /// <param name="id">Director ID</param>
    /// <returns>Director details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DirectorDto>> GetDirector(int id)
    {
        try
        {
            var director = await _directorService.GetDirectorByIdAsync(id);
            return Ok(director);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Director not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new director
    /// </summary>
    /// <param name="createDirectorDto">Director creation data</param>
    /// <returns>Newly created director</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DirectorDto>> CreateDirector(CreateDirectorDto createDirectorDto)
    {
        try
        {
            var director = await _directorService.CreateDirectorAsync(createDirectorDto);
            return CreatedAtAction(nameof(GetDirector), new { id = director.Id }, director);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating director");
            return BadRequest(new { message = "Error creating director", details = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing director
    /// </summary>
    /// <param name="id">Director ID</param>
    /// <param name="updateDirectorDto">Director update data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDirector(int id, UpdateDirectorDto updateDirectorDto)
    {
        try
        {
            await _directorService.UpdateDirectorAsync(id, updateDirectorDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Director not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating director: {Id}", id);
            return BadRequest(new { message = "Error updating director", details = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a director
    /// </summary>
    /// <param name="id">Director ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDirector(int id)
    {
        try
        {
            await _directorService.DeleteDirectorAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Director not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Searches for directors by name
    /// </summary>
    /// <param name="searchTerm">Search query string</param>
    /// <returns>Collection of directors matching the search term</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DirectorDto>>> SearchDirectors([FromQuery] string searchTerm)
    {
        var directors = await _directorService.SearchDirectorsAsync(searchTerm);
        return Ok(directors);
    }

    /// <summary>
    /// Retrieves all movies by a specific director
    /// </summary>
    /// <param name="id">Director ID</param>
    /// <returns>Collection of movies by the director</returns>
    [HttpGet("{id}/movies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetDirectorMovies(int id)
    {
        try
        {
            var director = await _directorService.GetDirectorByIdAsync(id);
            var movies = await _directorService.GetMoviesByDirectorAsync(id);
            return Ok(movies);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Director not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }
}