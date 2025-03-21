namespace MovieDatabaseAPI.Core.DTOs;

public class ActorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string Bio { get; set; } = string.Empty;
}

public class CreateActorDto
{
    public string Name { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string Bio { get; set; } = string.Empty;
}

public class UpdateActorDto
{
    public string Name { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string Bio { get; set; } = string.Empty;
}