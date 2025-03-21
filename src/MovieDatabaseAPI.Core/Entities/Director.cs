namespace MovieDatabaseAPI.Core.Entities;

public class Director
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string Bio { get; set; } = string.Empty;
}