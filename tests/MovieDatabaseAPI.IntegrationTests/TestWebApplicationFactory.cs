using Asp.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MovieDatabaseAPI.API.Extensions;
using MovieDatabaseAPI.Core.Entities;
using MovieDatabaseAPI.Infrastructure.Data.Context;

namespace MovieDatabaseAPI.IntegrationTests;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Find and remove DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryMovieDbForTesting");
            });

            services.AddVersioning();

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to get the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();

                try
                {
                    // Seed test data
                    SeedTestData(db);
                }
                catch (Exception ex)
                {
                    var logger = scopedServices.GetRequiredService<ILogger<TestWebApplicationFactory<TProgram>>>();
                    logger.LogError(ex, "An error occurred seeding the database with test data.");
                }
            }
        });
    }

    private void SeedTestData(AppDbContext context)
    {
        // Seed Genres
        var genres = new List<Genre>
        {
            new Genre { Id = 1, Name = "Action", Description = "Action movies" },
            new Genre { Id = 2, Name = "Comedy", Description = "Comedy movies" },
            new Genre { Id = 3, Name = "Drama", Description = "Drama movies" }
        };
        context.Genres.AddRange(genres);

        // Seed Directors
        var directors = new List<Director>
        {
            new Director
            {
                Id = 1, Name = "Christopher Nolan", DateOfBirth = new DateOnly(1970, 7, 30),
                Bio = "British-American filmmaker"
            }
        };
        context.Directors.AddRange(directors);

        // Seed Actors
        var actors = new List<Actor>
        {
            new Actor
            {
                Id = 1, Name = "Leonardo DiCaprio", DateOfBirth = new DateOnly(1974, 11, 11), Bio = "American actor"
            },
            new Actor { Id = 2, Name = "Tom Hardy", DateOfBirth = new DateOnly(1977, 9, 15), Bio = "British actor" }
        };
        context.Actors.AddRange(actors);

        // Seed Movies
        var movies = new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Inception",
                ReleaseYear = 2010,
                Plot =
                    "A thief who steals corporate secrets through dream-sharing technology is given the task of planting an idea into the mind of a C.E.O.",
                RuntimeMinutes = 148,
                PosterUrl = "https://example.com/inception.jpg",
                DirectorId = 1
            }
        };
        context.Movies.AddRange(movies);

        // Add relationships
        var movie = movies.First();
        movie.Actors.Add(actors[0]); // Leonardo DiCaprio
        movie.Actors.Add(actors[1]); // Tom Hardy
        movie.Genres.Add(genres[0]); // Action
        movie.Genres.Add(genres[2]); // Drama

        // Seed Users
        // Create a password hash for testing
        CreatePasswordHash("Password123", out byte[] passwordHash, out byte[] passwordSalt);

        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            }
        };
        context.Users.AddRange(users);

        context.SaveChanges();
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
}