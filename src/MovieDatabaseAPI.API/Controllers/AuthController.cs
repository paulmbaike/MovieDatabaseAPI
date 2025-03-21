using Microsoft.AspNetCore.Mvc;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Interfaces.Services;

namespace MovieDatabaseAPI.API.Controllers;

/// <summary>
/// API controller for user authentication and registration
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="authService">Service for authentication operations</param>
    /// <param name="logger">Logger for diagnostic information</param>
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(AuthRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed for user: {Username}", request.Username);
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="request">User registration information</param>
    /// <returns>Authentication token and user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Login), new { }, response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for user: {Username}", request.Username);
            return BadRequest(new { message = ex.Message });
        }
    }
}