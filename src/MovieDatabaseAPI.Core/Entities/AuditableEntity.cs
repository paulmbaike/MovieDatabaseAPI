namespace MovieDatabaseAPI.Core.Entities;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public string? ModifiedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
}