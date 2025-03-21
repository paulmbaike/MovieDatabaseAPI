using MovieDatabaseAPI.Core.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MovieDatabaseAPI.IntegrationTests;

public static class AuthHelper
{
    public static async Task<string> GetJwtTokenAsync(HttpClient client)
    {
        var loginRequest = new AuthRequestDto
        {
            Username = "testuser",
            Password = "Password123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return authResponse?.Token ?? string.Empty;
    }

    public static void AuthorizeClient(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}