using Microsoft.EntityFrameworkCore;
using MovieMentorCore.Data;

namespace MovieMentorDatabase.DAL;

public class MovieContext : DbContext
{
    private readonly string _connectionString;

    private readonly ServerVersion _serverVersion;

    public DbSet<Movie> Movies { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Director> Directors { get; set; }

    public DbSet<Award> Awards { get; set; }

    public DbSet<Actor> Actors { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public MovieContext(string connectionString, ServerVersion serverVersion)
    {
        _connectionString = connectionString;
        _serverVersion = serverVersion;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Genres)
            .WithMany("Movies");

        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Actors)
            .WithMany("Movies");

        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Directors)
            .WithMany("Movies");

        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Awards)
            .WithMany("Movies");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(_connectionString, _serverVersion);
    }
}
