using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Infrastructure.Data.Context;

namespace MovieDatabaseAPI.Infrastructure.Data;

public class Seeder
{
    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Seeder");

        try
        {
            await context.Database.MigrateAsync();

            if (!context.Genres.Any())
            {
                await SeedGenresAsync(context);
            }

            if (!context.Directors.Any())
            {
                await SeedDirectorsAsync(context);
            }

            if (!context.Actors.Any())
            {
                await SeedActorsAsync(context);
            }

            if (!context.Movies.Any())
            {
                await SeedMoviesAsync(context);
            }

            if (!context.Users.Any())
            {
                await SeedUsersAsync(context);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
        }
    }

    private static async Task SeedGenresAsync(AppDbContext context)
    {
        var genres = new List<Genre>
            {
                new Genre { Name = "Action", Description = "Action films" },
                new Genre { Name = "Comedy", Description = "Comedy films" },
                new Genre { Name = "Drama", Description = "Drama films" },
                new Genre { Name = "Science Fiction", Description = "Science Fiction films" },
                new Genre { Name = "Horror", Description = "Horror films" },
                new Genre { Name = "Thriller", Description = "Thriller films" },
                new Genre { Name = "Romance", Description = "Romance films" }
            };

        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();
    }

    private static async Task SeedDirectorsAsync(AppDbContext context)
    {
        var directors = new List<Director>
            {
                new Director { Name = "Christopher Nolan", DateOfBirth = new DateOnly(1970, 7, 30), Bio = "British-American film director known for mind-bending narratives" },
                new Director { Name = "Quentin Tarantino", DateOfBirth = new DateOnly(1963, 3, 27), Bio = "American film director known for nonlinear storytelling" },
                new Director { Name = "Steven Spielberg", DateOfBirth = new DateOnly(1946, 12, 18), Bio = "American film director and producer" }
            };

        await context.Directors.AddRangeAsync(directors);
        await context.SaveChangesAsync();
    }

    private static async Task SeedActorsAsync(AppDbContext context)
    {
        var actors = new List<Actor>
            {
                new Actor { Name = "Leonardo DiCaprio", DateOfBirth = new DateOnly(1974, 11, 11), Bio = "American actor known for intense performances" },
                new Actor { Name = "Tom Hanks", DateOfBirth = new DateOnly(1956, 7, 9), Bio = "American actor and filmmaker" },
                new Actor { Name = "Meryl Streep", DateOfBirth = new DateOnly(1949, 6, 22), Bio = "American actress known for versatility" },
                new Actor { Name = "Brad Pitt", DateOfBirth = new DateOnly(1963, 12, 18), Bio = "American actor and producer" }
            };

        await context.Actors.AddRangeAsync(actors);
        await context.SaveChangesAsync();
    }

    private static async Task SeedMoviesAsync(AppDbContext context)
    {
        var nolanId = context.Directors.Single(d => d.Name == "Christopher Nolan").Id;
        var leoId = context.Actors.Single(a => a.Name == "Leonardo DiCaprio").Id;
        var bradId = context.Actors.Single(a => a.Name == "Brad Pitt").Id;

        var sciFiId = context.Genres.Single(g => g.Name == "Science Fiction").Id;
        var dramaId = context.Genres.Single(g => g.Name == "Drama").Id;
        var thrillerID = context.Genres.Single(g => g.Name == "Thriller").Id;

        var leo = await context.Actors.FindAsync(leoId);
        var brad = await context.Actors.FindAsync(bradId);
        var sciFi = await context.Genres.FindAsync(sciFiId);
        var drama = await context.Genres.FindAsync(dramaId);
        var thriller = await context.Genres.FindAsync(thrillerID);

        var movies = new List<Movie>
            {
                new Movie {
                    Title = "Inception",
                    ReleaseYear = 2010,
                    Plot = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                    RuntimeMinutes = 148,
                    PosterUrl = "https://example.com/inception.jpg",
                    DirectorId = nolanId,
                    Actors = new List<Actor> { leo },
                    Genres = new List<Genre> { sciFi, thriller }
                },
                new Movie {
                    Title = "Interstellar",
                    ReleaseYear = 2014,
                    Plot = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                    RuntimeMinutes = 169,
                    PosterUrl = "https://example.com/interstellar.jpg",
                    DirectorId = nolanId,
                    Genres = new List<Genre> { sciFi, drama }
                }
            };

        await context.Movies.AddRangeAsync(movies);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        // Create a password hash and salt
        CreatePasswordHash("Password123", out byte[] passwordHash, out byte[] passwordSalt);

        var users = new List<User>
            {
                new User {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                }
            };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
}