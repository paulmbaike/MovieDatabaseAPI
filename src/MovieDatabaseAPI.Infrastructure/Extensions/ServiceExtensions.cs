using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieDatabaseAPI.Core.Interfaces.Repositories;
using MovieDatabaseAPI.Infrastructure.Data.Context;
using MovieDatabaseAPI.Infrastructure.Data.Repositories;

namespace MovieDatabaseAPI.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));


        services.AddScoped<IMovieRepository, MovieRepository>();
        return services;
    }
}