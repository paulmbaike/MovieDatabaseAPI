namespace MovieDatabaseAPI.Core.DTOs;

public class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateGenreDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateGenreDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}