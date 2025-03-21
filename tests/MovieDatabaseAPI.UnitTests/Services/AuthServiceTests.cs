using Microsoft.Extensions.Configuration;
using Moq;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Services.Services;

namespace MovieDatabaseAPI.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        var mockConfigSection = new Mock<IConfigurationSection>();

        mockConfigSection.Setup(x => x.Value).Returns("Test__U05$y0=qq1!gpJ5Mc*29-yk.ymlY8j(xsN(}&5Ro^o[y0IvK(@rY/n=JH@70ZX[");
        _mockConfiguration.Setup(x => x["JwtSettings:Secret"]).Returns("Test__U05$y0=qq1!gpJ5Mc*29-yk.ymlY8j(xsN(}&5Ro^o[y0IvK(@rY/n=JH@70ZX[");
        _mockConfiguration.Setup(x => x["JwtSettings:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(x => x["JwtSettings:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithNewUser_ReturnsAuthResponse()
    {
        // Arrange
        var registerDto = new RegisterRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123"
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(registerDto.Username)).ReturnsAsync((User)null);
        _mockUserRepository.Setup(r => r.GetByEmailAsync(registerDto.Email)).ReturnsAsync((User)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 1, Username = registerDto.Username });

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(registerDto.Username, result.Username);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var registerDto = new RegisterRequestDto
        {
            Username = "existinguser",
            Email = "test@example.com",
            Password = "Password123"
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(registerDto.Username))
            .ReturnsAsync(new User { Id = 1, Username = registerDto.Username });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var loginDto = new AuthRequestDto
        {
            Username = "testuser",
            Password = "Password123"
        };

        // Create password hash using same method as in service
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        var passwordSalt = hmac.Key;
        var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDto.Password));

        var user = new User
        {
            Id = 1,
            Username = loginDto.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(loginDto.Username, result.Username);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new AuthRequestDto
        {
            Username = "nonexistentuser",
            Password = "Password123"
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(loginDto.Username)).ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new AuthRequestDto
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        // Create hash for a different password
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        var passwordSalt = hmac.Key;
        var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("CorrectPassword"));

        var user = new User
        {
            Id = 1,
            Username = loginDto.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(loginDto));
    }
}