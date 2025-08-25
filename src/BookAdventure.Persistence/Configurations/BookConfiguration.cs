using BookAdventure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookAdventure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        // Primary Key
        builder.HasKey(x => x.Id);
        
        // Properties
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(x => x.Author)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(x => x.ISBN)
            .HasMaxLength(13)
            .IsUnicode(false);
            
        builder.Property(x => x.Description)
            .HasMaxLength(1000);
            
        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);
            
        // Relationships
        builder.HasOne(x => x.Genre)
            .WithMany(x => x.Books)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.Author);
        builder.HasIndex(x => x.ISBN)
            .IsUnique()
            .HasFilter("[ISBN] IS NOT NULL");
        builder.HasIndex(x => x.GenreId);
        
        // Query Filters - handled globally in ApplicationDbContext
        // builder.HasQueryFilter(x => x.Status != EntityStatus.Deleted);
        
        // Table Configuration
        builder.ToTable("Book", "BookAdventure");
    }
}
