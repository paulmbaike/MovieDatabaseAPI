using System.IO;

namespace MovieDatabaseAPI.Core.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Plot { get; set; } = string.Empty;
    public int RuntimeMinutes { get; set; }
    public string PosterUrl { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public Director? Director { get; set; }
    public int? DirectorId { get; set; }
}