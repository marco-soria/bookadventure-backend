using BookAdventure.Entities;
using BookAdventure.Persistence;
using BookAdventure.Services.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookAdventure.Services.Base;

/// <summary>
/// Servicio base que proporciona operaciones CRUD con soporte para eliminación lógica
/// </summary>
/// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
public abstract class BaseService<T> : IBaseService<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected BaseService(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Obtiene todas las entidades activas (no eliminadas)
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Obtiene todas las entidades incluyendo las eliminadas
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllIncludingDeletedAsync()
    {
        return await _context.IncludeDeleted<T>().ToListAsync();
    }

    /// <summary>
    /// Obtiene una entidad por ID (solo si está activa)
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Obtiene una entidad por ID incluyendo eliminadas
    /// </summary>
    public virtual async Task<T?> GetByIdIncludingDeletedAsync(int id)
    {
        return await _context.IncludeDeleted<T>().FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Busca entidades que coincidan con el filtro
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Crea una nueva entidad
    /// </summary>
    public virtual async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    public virtual async Task<T?> UpdateAsync(T entity)
    {
        var existing = await GetByIdAsync(entity.Id);
        if (existing == null)
            return null;

        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }

    /// <summary>
    /// Eliminación lógica - marca la entidad como eliminada
    /// </summary>
    public virtual async Task<bool> SoftDeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        entity.Status = EntityStatus.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Restaura una entidad eliminada lógicamente
    /// </summary>
    public virtual async Task<bool> RestoreAsync(int id)
    {
        var entity = await GetByIdIncludingDeletedAsync(id);
        if (entity == null || entity.Status != EntityStatus.Deleted)
            return false;

        entity.Status = EntityStatus.Active;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Eliminación física permanente - solo usar cuando sea absolutamente necesario
    /// </summary>
    public virtual async Task<bool> HardDeleteAsync(int id)
    {
        var entity = await GetByIdIncludingDeletedAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Verifica si una entidad existe y está activa
    /// </summary>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(x => x.Id == id);
    }

    /// <summary>
    /// Obtiene el conteo de entidades activas
    /// </summary>
    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    /// <summary>
    /// Obtiene el conteo total incluyendo eliminadas
    /// </summary>
    public virtual async Task<int> CountIncludingDeletedAsync()
    {
        return await _context.IncludeDeleted<T>().CountAsync();
    }
}
