namespace BookAdventure.Entities;

/// <summary>
/// Enum para manejar el estado de las entidades (eliminación lógica)
/// </summary>
public enum EntityStatus
{
    /// <summary>
    /// Entidad activa y disponible
    /// </summary>
    Active = 1,

    /// <summary>
    /// Entidad inactiva (suspendida temporalmente)
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Entidad eliminada lógicamente
    /// </summary>
    Deleted = 3
}
