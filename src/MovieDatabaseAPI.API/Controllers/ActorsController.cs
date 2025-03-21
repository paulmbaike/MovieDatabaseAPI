using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.API.Controllers;

/// <summary>
/// API controller for managing actor resources
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/actors")]
[Authorize]
public class ActorsController : ControllerBase
{
    private readonly IActorService _actorService;
    private readonly ILogger<ActorsController> _logger;

    /// <summary>
    /// Initializes a new instance of the ActorsController
    /// </summary>
    /// <param name="actorService">Actor service for business logic operations</param>
    /// <param name="logger">Logger for diagnostic information</param>
    public ActorsController(IActorService actorService, ILogger<ActorsController> logger)
    {
        _actorService = actorService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all actors
    /// </summary>
    /// <returns>Collection of actors</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetActors()
    {
        var actors = await _actorService.GetAllActorsAsync();
        return Ok(actors);
    }

    /// <summary>
    /// Retrieves a specific actor by ID
    /// </summary>
    /// <param name="id">Actor ID</param>
    /// <returns>Actor details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActorDto>> GetActor(int id)
    {
        try
        {
            var actor = await _actorService.GetActorByIdAsync(id);
            return Ok(actor);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Actor not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new actor
    /// </summary>
    /// <param name="createActorDto">Actor creation data</param>
    /// <returns>Newly created actor</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ActorDto>> CreateActor(CreateActorDto createActorDto)
    {
        try
        {
            var actor = await _actorService.CreateActorAsync(createActorDto);
            return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating actor");
            return BadRequest(new { message = "Error creating actor", details = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing actor
    /// </summary>
    /// <param name="id">Actor ID</param>
    /// <param name="updateActorDto">Actor update data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateActor(int id, UpdateActorDto updateActorDto)
    {
        try
        {
            await _actorService.UpdateActorAsync(id, updateActorDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Actor not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating actor: {Id}", id);
            return BadRequest(new { message = "Error updating actor", details = ex.Message });
        }
    }

    /// <summary>
    /// Deletes an actor
    /// </summary>
    /// <param name="id">Actor ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteActor(int id)
    {
        try
        {
            await _actorService.DeleteActorAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Actor not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Searches for actors by name
    /// </summary>
    /// <param name="searchTerm">Search query string</param>
    /// <returns>Collection of actors matching the search term</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ActorDto>>> SearchActors([FromQuery] string searchTerm)
    {
        var actors = await _actorService.SearchActorsAsync(searchTerm);
        return Ok(actors);
    }

    /// <summary>
    /// Retrieves all movies featuring a specific actor
    /// </summary>
    /// <param name="id">Actor ID</param>
    /// <returns>Collection of movies featuring the actor</returns>
    [HttpGet("{id}/movies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetActorMovies(int id)
    {
        try
        {
            var actor = await _actorService.GetActorByIdAsync(id);
            var movies = await _actorService.GetMoviesByActorAsync(id);
            return Ok(movies);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Actor not found: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }
}