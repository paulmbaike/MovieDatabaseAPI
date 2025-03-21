namespace MovieDatabaseAPI.Core.DTOs;

public class DirectorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Bio { get; set; } = string.Empty;
}

public class CreateDirectorDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Bio { get; set; } = string.Empty;
}

public class UpdateDirectorDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Bio { get; set; } = string.Empty;
}