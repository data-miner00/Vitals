namespace Vitals.Integrations;

using Microsoft.EntityFrameworkCore;
using Vitals.Core;
using Vitals.Core.Models;

public sealed class AppDbContext : DbContext
{
    private readonly string databaseConnectionString;

    public AppDbContext()
        : this("Data Source=D:\\vitals.db;")
    {
    }

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
}
