using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookAdventure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Primary Key
        builder.HasKey(x => x.Id);
        
        // Properties
        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired()
            .IsUnicode(false);

        builder.Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(x => x.DNI)
            .HasMaxLength(20)
            .IsRequired()
            .IsUnicode(false);
            
        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsUnicode(false);
            
        // Computed property - not mapped to database
        builder.Ignore(x => x.FullName);
            
        // Indexes with explicit names for better performance tracking
        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("IX_Customer_Email");
            
        builder.HasIndex(x => x.DNI)
            .IsUnique()
            .HasDatabaseName("IX_Customer_DNI"); // Crítico para búsqueda por DNI
            
        builder.HasIndex(x => new { x.FirstName, x.LastName })
            .HasDatabaseName("IX_Customer_FullName");
            
        // Índice adicional para optimizar JOIN con RentalOrders
        builder.HasIndex(x => new { x.DNI, x.Id })
            .HasDatabaseName("IX_Customer_DNI_Id");
        
        // Query Filters - will be handled globally in ApplicationDbContext
        // builder.HasQueryFilter(x => x.Status != EntityStatus.Deleted);

        // Table Configuration
        builder.ToTable("Customer", "BookAdventure");
    }
}