namespace MovieDatabaseAPI.Core.DTOs;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Plot { get; set; } = string.Empty;
    public int RuntimeMinutes { get; set; }
    public string PosterUrl { get; set; } = string.Empty;
    public string DirectorName { get; set; } = string.Empty;
    public IEnumerable<string> Genres { get; set; } = new List<string>();
    public IEnumerable<string> Actors { get; set; } = new List<string>();
}

public class CreateOrUpdateMovieDto
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Plot { get; set; } = string.Empty;
    public int RuntimeMinutes { get; set; }
    public string PosterUrl { get; set; } = string.Empty;
    public int? DirectorId { get; set; }
    public List<int> GenreIds { get; set; } = new List<int>();
    public List<int> ActorIds { get; set; } = new List<int>();
}