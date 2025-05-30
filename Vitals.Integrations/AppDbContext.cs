namespace Vitals.Integrations;

using Microsoft.EntityFrameworkCore;
using Vitals.Core;
using Vitals.Core.Models;

public sealed class AppDbContext : DbContext
{
    private readonly string databaseConnectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/>.
    /// </summary>
    public AppDbContext()
        : this("Data Source=D:\\vitals.db;")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/>.
    /// </summary>
    /// <param name="databaseConnectionString">The database connection string.</param>
    public AppDbContext(string databaseConnectionString)
    {
        this.databaseConnectionString = Guard.ThrowIfNullOrWhitespace(databaseConnectionString);
        this.Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Credential> Credentials { get; set; }

    public DbSet<Vote> Votes { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(this.databaseConnectionString);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Credential>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
