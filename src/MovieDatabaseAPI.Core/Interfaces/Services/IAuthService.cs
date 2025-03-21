using MovieDatabaseAPI.Core.DTOs;

namespace MovieDatabaseAPI.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(AuthRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
}