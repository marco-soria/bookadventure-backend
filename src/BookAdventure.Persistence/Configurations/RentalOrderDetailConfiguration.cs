using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookAdventure.Persistence.Configurations;

public class RentalOrderDetailConfiguration : IEntityTypeConfiguration<RentalOrderDetail>
{
    public void Configure(EntityTypeBuilder<RentalOrderDetail> builder)
    {
        // Primary Key
        builder.HasKey(x => x.Id);
        
        // Properties
        builder.Property(x => x.Notes)
            .HasMaxLength(200);
            
        // Relationships
        builder.HasOne(x => x.RentalOrder)
            .WithMany(x => x.RentalOrderDetails)
            .HasForeignKey(x => x.RentalOrderId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.Book)
            .WithMany(x => x.RentalOrderDetails)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(x => x.RentalOrderId);
        builder.HasIndex(x => x.BookId);
        builder.HasIndex(x => new { x.RentalOrderId, x.BookId })
            .IsUnique();
            
        // Query Filters - handled globally in ApplicationDbContext
        // builder.HasQueryFilter(x => x.Status != EntityStatus.Deleted);
        
        // Table Configuration
        builder.ToTable("RentalOrderDetail", "BookAdventure");
    }
}
