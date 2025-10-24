using Microsoft.EntityFrameworkCore;
using URLShortener.Shared.Models;

namespace URLShortener.UrlShortenerService.Data;

/// <summary>
/// DbContext cho UrlShortenerService
/// </summary>
public class UrlShortenerDbContext : DbContext
{
    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
        : base(options)
    {
    }

    public DbSet<UrlMapping> UrlMappings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UrlMapping>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OriginalUrl)
                .IsRequired()
                .HasColumnType("TEXT");

            entity.Property(e => e.ShortCode)
                .IsRequired()
                .HasMaxLength(8)
                .HasColumnType("VARCHAR(8)");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("DATETIME");

            // Index cho ShortCode để tìm kiếm nhanh
            entity.HasIndex(e => e.ShortCode)
                .IsUnique()
                .HasDatabaseName("IX_UrlMappings_ShortCode");
        });
    }
}
