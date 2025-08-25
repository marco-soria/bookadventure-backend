using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookAdventure.Persistence.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        // Primary Key
        builder.HasKey(x => x.Id);
        
        // Properties
        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();
            
        // Indexes
        builder.HasIndex(x => x.Name)
            .IsUnique();
            
        // Query Filters - handled globally in ApplicationDbContext
        // builder.HasQueryFilter(x => x.Status != EntityStatus.Deleted);
        
        // Table Configuration
        builder.ToTable("Genre", "BookAdventure");
    }
}
