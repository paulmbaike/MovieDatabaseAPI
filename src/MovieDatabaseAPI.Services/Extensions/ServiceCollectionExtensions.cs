using Microsoft.Extensions.DependencyInjection;
using MovieDatabaseAPI.Core.Interfaces.Services;
using MovieDatabaseAPI.Services.Mapping;
using MovieDatabaseAPI.Services.Services;

namespace MovieDatabaseAPI.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IActorService, ActorService>();
        services.AddScoped<IDirectorService, DirectorService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IAuthService, AuthService>();

        // Register AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        return services;
    }
}