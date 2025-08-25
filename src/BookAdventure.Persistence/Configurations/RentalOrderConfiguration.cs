using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookAdventure.Persistence.Configurations;

public class RentalOrderConfiguration : IEntityTypeConfiguration<RentalOrder>
{
    public void Configure(EntityTypeBuilder<RentalOrder> builder)
    {
        // Primary Key
        builder.HasKey(x => x.Id);
        
        // Properties
        builder.Property(x => x.OrderNumber)
            .HasMaxLength(20)
            .IsRequired()
            .IsUnicode(false);
            
        builder.Property(x => x.OrderStatus)
            .HasConversion<string>()
            .HasDefaultValue(OrderStatus.Pending)
            .HasSentinel(OrderStatus.Cancelled); // Use Cancelled as sentinel
            
        builder.Property(x => x.Notes)
            .HasMaxLength(500);
            
        // Relationships
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.RentalOrders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes with explicit names for better performance tracking
        builder.HasIndex(x => x.OrderNumber)
            .IsUnique()
            .HasDatabaseName("IX_RentalOrder_OrderNumber");
            
        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("IX_RentalOrder_CustomerId");
            
        builder.HasIndex(x => x.OrderDate)
            .HasDatabaseName("IX_RentalOrder_OrderDate");
            
        builder.HasIndex(x => x.OrderStatus)
            .HasDatabaseName("IX_RentalOrder_Status");
            
        builder.HasIndex(x => new { x.CustomerId, x.OrderDate })
            .HasDatabaseName("IX_RentalOrder_Customer_Date");
            
        // Índice crítico para consultas por DNI de customer y status
        builder.HasIndex(x => new { x.CustomerId, x.OrderStatus, x.OrderDate })
            .HasDatabaseName("IX_RentalOrder_Customer_Status_Date");
        
        // Query Filters - will be handled globally in ApplicationDbContext
        // builder.HasQueryFilter(x => x.Status != EntityStatus.Deleted);
        
        // Table Configuration
        builder.ToTable("RentalOrder", "BookAdventure");
    }
}
