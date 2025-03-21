using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using MovieDatabaseAPI.Core.Entities;

namespace MovieDatabaseAPI.Infrastructure.Data.Context;

public class AppContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Actors)
                .WithMany()
                .UsingEntity(j => j.ToTable("MovieActors"));

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Genres)
                .WithMany()
                .UsingEntity(j => j.ToTable("MovieGenres"));

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Director)
                .WithMany()
                .HasForeignKey(m => m.DirectorId);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }

}