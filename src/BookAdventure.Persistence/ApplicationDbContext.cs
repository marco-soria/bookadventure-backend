using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookAdventure.Entities;
using BookAdventure.Persistence.Extensions;
using System.Linq.Expressions;

namespace BookAdventure.Persistence;

public class ApplicationDbContext : IdentityDbContext<BookAdventureUserIdentity>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    // DbSets
    public DbSet<Book> Books { get; set; } = default!;
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Genre> Genres { get; set; } = default!;
    public DbSet<RentalOrder> RentalOrders { get; set; } = default!;
    public DbSet<RentalOrderDetail> RentalOrderDetails { get; set; } = default!;

    //Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Configure soft delete filters and audit properties
        ConfigureSoftDeleteFilters(modelBuilder);
        
        // Identity table configurations
        modelBuilder.Entity<BookAdventureUserIdentity>(x => x.ToTable("User", "Identity"));
        modelBuilder.Entity<IdentityRole>(x => x.ToTable("Role", "Identity"));
        modelBuilder.Entity<IdentityUserRole<string>>(x => x.ToTable("UserRole", "Identity"));
        modelBuilder.Entity<IdentityUserClaim<string>>(x => x.ToTable("UserClaim", "Identity"));
        modelBuilder.Entity<IdentityUserLogin<string>>(x => x.ToTable("UserLogin", "Identity"));
        modelBuilder.Entity<IdentityUserToken<string>>(x => x.ToTable("UserToken", "Identity"));
        modelBuilder.Entity<IdentityRoleClaim<string>>(x => x.ToTable("RoleClaim", "Identity"));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }

    // Override SaveChanges to automatically set audit fields and handle soft delete
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Configura filtros globales para eliminación lógica
    /// Evita que las queries devuelvan entidades eliminadas automáticamente
    /// </summary>
    private void ConfigureSoftDeleteFilters(ModelBuilder modelBuilder)
    {
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
                
                // Configure Status property with sentinel value for proper default handling
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.Status))
                    .HasDefaultValue(EntityStatus.Active)
                    .HasSentinel(EntityStatus.Inactive); // Use Inactive as sentinel
                    
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreatedAt))
                    .HasDefaultValueSql("GETUTCDATE()");
            }
        }
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.Status = EntityStatus.Active;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // Prevent accidental modification of CreatedAt
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    break;

                case EntityState.Deleted:
                    // Implement soft delete - change state to Modified and mark as deleted
                    entry.State = EntityState.Modified;
                    entry.Entity.Status = EntityStatus.Deleted;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }

    /// <summary>
    /// Método para incluir entidades eliminadas en consultas específicas cuando sea necesario
    /// </summary>
    public IQueryable<T> IncludeDeleted<T>() where T : BaseEntity
    {
        return Set<T>().IgnoreQueryFilters();
    }

    /// <summary>
    /// Método para obtener solo entidades eliminadas
    /// </summary>
    public IQueryable<T> OnlyDeleted<T>() where T : BaseEntity
    {
        return Set<T>().IgnoreQueryFilters().Where(x => x.Status == EntityStatus.Deleted);
    }
}
