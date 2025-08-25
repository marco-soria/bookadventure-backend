namespace BookAdventure.Entities;

/// <summary>
/// Base entity class that provides common properties for all entities
/// Includes support for soft delete pattern and audit information
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    
    /// <summary>
    /// Entity status for soft delete functionality
    /// </summary>
    public EntityStatus Status { get; set; } = EntityStatus.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates if the entity is logically deleted
    /// </summary>
    public bool IsDeleted => Status == EntityStatus.Deleted;

    /// <summary>
    /// Indicates if the entity is active
    /// </summary>
    public bool IsActive => Status == EntityStatus.Active;
}
