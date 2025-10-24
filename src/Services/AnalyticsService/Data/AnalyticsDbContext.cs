using Microsoft.EntityFrameworkCore;
using URLShortener.Shared.Models;

namespace URLShortener.AnalyticsService.Data;

/// <summary>
/// DbContext cho AnalyticsService
/// </summary>
public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
        : base(options)
    {
    }

    public DbSet<ClickEvent> ClickEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClickEvent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ShortCode)
                .IsRequired()
                .HasMaxLength(8)
                .HasColumnType("VARCHAR(8)");

            entity.Property(e => e.Timestamp)
                .IsRequired()
                .HasColumnType("DATETIME");

            entity.Property(e => e.UserAgent)
                .HasMaxLength(500)
                .HasColumnType("VARCHAR(500)");

            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnType("VARCHAR(45)");

            // Index cho ShortCode để query nhanh
            entity.HasIndex(e => e.ShortCode)
                .HasDatabaseName("IX_ClickEvents_ShortCode");

            // Index cho Timestamp để query theo thời gian
            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_ClickEvents_Timestamp");
        });
    }
}
