namespace BookAdventure.Entities;

/// <summary>
/// Enum para manejar el estado de las entidades (eliminación lógica)
/// </summary>
public enum EntityStatus
{
    /// <summary>
    /// Entidad activa y disponible para operaciones normales
    /// </summary>
    Active = 1,

    /// <summary>
    /// Entidad eliminada lógicamente (soft delete)
    /// </summary>
    Deleted = 2
}
