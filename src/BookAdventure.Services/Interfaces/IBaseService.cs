using BookAdventure.Entities;
using System.Linq.Expressions;

namespace BookAdventure.Services.Interfaces.Base;

/// <summary>
/// Interfaz genérica para servicios CRUD con soporte para eliminación lógica
/// </summary>
/// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
public interface IBaseService<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllIncludingDeletedAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdIncludingDeletedAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(T entity);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> RestoreAsync(int id);
    Task<bool> HardDeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    Task<int> CountIncludingDeletedAsync();
}
