using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Data.Entities;
using Plutus.Infrastructure.Common;

namespace Plutus.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DataMigration> DataMigrations { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Obligor> Obligors { get; set; }
    public DbSet<RevolutBalanceAudit> BalanceAudits { get; set; }
    public DbSet<GoCardlessRequest> GoCardlessRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set the default schema for all tables
        modelBuilder.HasDefaultSchema("plutus");

        // Configure specific entity if needed
        modelBuilder.Entity<Transaction>(entity =>
        {
            // Configure the Amount property
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 4)") // Specify the SQL Server column type
                .HasPrecision(18, 4); // Specify precision and scale
        });

        modelBuilder.Entity<RevolutBalanceAudit>(entity =>
        {
            // Configure the Amount property
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 4)") // Specify the SQL Server column type
                .HasPrecision(18, 4); // Specify precision and scale
        });

        modelBuilder.Entity<Obligor>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasData(
                    new Category
                    {
                        Id = AppConstants.UncategorizedCategoryId,

                        Name = "Uncategorized",
                    },
                    new Category
                    {
                        Id = AppConstants.FixedCategoryId,
                        Name = "Fixed",
                    }
            );
        });
    }
}
