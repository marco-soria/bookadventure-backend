using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BookAdventure.Entities;

namespace BookAdventure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Configura filtros globales para eliminación lógica (soft delete)
    /// Esto evita que las queries devuelvan entidades eliminadas de forma automática
    /// </summary>
    public static void ConfigureSoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        // Aplicar filtro global para todas las entidades que heredan de BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.Status));
                var filter = Expression.Lambda(
                    Expression.NotEqual(property, Expression.Constant(EntityStatus.Deleted)),
                    parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    /// <summary>
    /// Configura interceptores para auditoría automática
    /// </summary>
    public static void ConfigureAuditProperties(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreatedAt))
                    .HasDefaultValueSql("GETUTCDATE()");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.UpdatedAt))
                    .HasDefaultValueSql("GETUTCDATE()");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.Status))
                    .HasDefaultValue(EntityStatus.Active);
            }
        }
    }
}
