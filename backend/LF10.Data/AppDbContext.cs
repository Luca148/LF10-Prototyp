using LF10.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LF10.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<FilterCategoryEntity> FilterCategories { get; set; }
    public DbSet<FilterWordEntity> FilterWords { get; set; }
    public DbSet<FilterPhraseEntity> FilterPhrases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FilterCategoryEntity>(category =>
        {
            category.Property(c => c.Name).HasMaxLength(50).IsRequired();
            category.HasIndex(c => c.Name).IsUnique();
        });

        // The filter checks every message against all languages, and once a
        // word or phrase is replaced it can't match again. So a second row for
        // the same text (other category or language) would never be used.
        modelBuilder.Entity<FilterWordEntity>(word =>
        {
            word.Property(w => w.Language).HasMaxLength(5).IsRequired();
            word.Property(w => w.CurseWord).HasMaxLength(100).IsRequired();
            word.Property(w => w.FilterWord).HasMaxLength(100).IsRequired();
            word.HasIndex(w => w.CurseWord).IsUnique();
        });

        modelBuilder.Entity<FilterPhraseEntity>(phrase =>
        {
            phrase.Property(p => p.Language).HasMaxLength(5).IsRequired();
            phrase.Property(p => p.CursePhrase).HasMaxLength(100).IsRequired();
            phrase.Property(p => p.FilterPhrase).HasMaxLength(100).IsRequired();
            // Unique for the same reason as words.
            phrase.HasIndex(p => p.CursePhrase).IsUnique();
        });
    }
}
